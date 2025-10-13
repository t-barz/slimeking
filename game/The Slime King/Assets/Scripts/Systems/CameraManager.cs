using System.Collections;
using UnityEngine;
using SlimeKing.Core;

namespace SlimeKing.Systems
{
    /// <summary>
    /// Manager para controle da câmera principal do jogo.
    /// Suporta follow, zoom, shake e diferentes modos de câmera.
    /// </summary>
    public class CameraManager : ManagerBase<CameraManager>
    {
        [Header("Camera References")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform cameraTransform;

        [Header("Target Settings")]
        [SerializeField] private Transform currentTarget;
        [SerializeField] private CameraMode currentMode = CameraMode.Follow;

        [Header("Camera Configuration")]
        [SerializeField] private CameraSettings settings;

        // Shake effect
        private Vector3 shakeOffset = Vector3.zero;
        private float currentShakeIntensity = 0f;
        private Coroutine shakeCoroutine;

        // Zoom effect
        private float targetZoom;
        private float initialZoom;
        private Coroutine zoomCoroutine;

        // Follow system
        private Vector3 velocity = Vector3.zero;
        private Vector3 basePosition;

        // Properties
        public Camera MainCamera => mainCamera;
        public Transform CurrentTarget => currentTarget;
        public CameraMode CurrentMode => currentMode;
        public float CurrentZoom => mainCamera != null ? mainCamera.orthographicSize : 5f;
        public Vector3 CameraPosition => cameraTransform != null ? cameraTransform.position : Vector3.zero;

        protected override void Initialize()
        {
            SetupCamera();
            SetupSettings();
            FindPlayerAsTarget();
            Log("CameraManager initialized");
        }

        #region Setup

        /// <summary>
        /// Configura a câmera principal
        /// </summary>
        private void SetupCamera()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    mainCamera = FindFirstObjectByType<Camera>();
                }
            }

            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
                initialZoom = mainCamera.orthographicSize;
                targetZoom = initialZoom;
            }
            else
            {
                LogError("No camera found! Creating a basic camera.");
                CreateBasicCamera();
            }
        }

        /// <summary>
        /// Cria uma câmera básica se nenhuma for encontrada
        /// </summary>
        private void CreateBasicCamera()
        {
            GameObject cameraGO = new GameObject("Main Camera");
            cameraGO.tag = "MainCamera";
            mainCamera = cameraGO.AddComponent<Camera>();
            cameraTransform = cameraGO.transform;

            // Configuração básica para 2D
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5f;
            cameraTransform.position = new Vector3(0, 0, -10);

            initialZoom = mainCamera.orthographicSize;
            targetZoom = initialZoom;
        }

        /// <summary>
        /// Configura as configurações padrão
        /// </summary>
        private void SetupSettings()
        {
            if (settings == null)
            {
                settings = new CameraSettings();
            }
        }

        /// <summary>
        /// Tenta encontrar o jogador automaticamente
        /// </summary>
        private void FindPlayerAsTarget()
        {
            if (currentTarget == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    SetTarget(player.transform);
                }
            }
        }

        #endregion

        #region Update

        private void LateUpdate()
        {
            if (mainCamera == null || cameraTransform == null) return;

            UpdateCameraMode();
            UpdateShake();
            UpdateZoom();
        }

        /// <summary>
        /// Atualiza o comportamento baseado no modo atual
        /// </summary>
        private void UpdateCameraMode()
        {
            switch (currentMode)
            {
                case CameraMode.Follow:
                    UpdateFollowMode();
                    break;
                case CameraMode.Fixed:
                    // Não faz nada, mantém posição fixa
                    break;
                case CameraMode.Cinematic:
                    // Pode ser implementado para cutscenes
                    break;
                case CameraMode.FreeRoam:
                    // Pode ser implementado para movimento livre
                    break;
            }
        }

        /// <summary>
        /// Atualiza o modo de seguir alvo
        /// </summary>
        private void UpdateFollowMode()
        {
            if (currentTarget == null) return;

            Vector3 targetPosition = currentTarget.position + settings.defaultOffset;

            // Aplica bounds se habilitado
            if (settings.useBounds)
            {
                targetPosition = ClampToBounds(targetPosition);
            }

            // Movimento suave ou instantâneo
            if (settings.smoothFollow)
            {
                basePosition = Vector3.SmoothDamp(cameraTransform.position, targetPosition,
                    ref velocity, 1f / settings.followSpeed);
            }
            else
            {
                basePosition = targetPosition;
            }

            // Aplica posição final com shake
            cameraTransform.position = basePosition + shakeOffset;
        }

        /// <summary>
        /// Restringe posição aos bounds definidos
        /// </summary>
        private Vector3 ClampToBounds(Vector3 position)
        {
            Bounds bounds = settings.cameraBounds;

            position.x = Mathf.Clamp(position.x, bounds.min.x, bounds.max.x);
            position.y = Mathf.Clamp(position.y, bounds.min.y, bounds.max.y);
            position.z = Mathf.Clamp(position.z, bounds.min.z, bounds.max.z);

            return position;
        }

        #endregion

        #region Target Management

        /// <summary>
        /// Define o alvo da câmera
        /// </summary>
        public void SetTarget(Transform target)
        {
            currentTarget = target;
            Log($"Camera target set to: {(target != null ? target.name : "null")}");
        }

        /// <summary>
        /// Remove o alvo atual
        /// </summary>
        public void ClearTarget()
        {
            currentTarget = null;
            Log("Camera target cleared");
        }

        /// <summary>
        /// Muda o modo da câmera
        /// </summary>
        public void SetCameraMode(CameraMode mode)
        {
            currentMode = mode;
            Log($"Camera mode changed to: {mode}");
        }

        #endregion

        #region Shake Effect

        /// <summary>
        /// Inicia o efeito de shake
        /// </summary>
        public void ShakeCamera(float intensity, float duration)
        {
            intensity = Mathf.Clamp(intensity, 0f, settings.maxShakeIntensity);

            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            shakeCoroutine = StartCoroutine(ShakeCoroutine(intensity, duration));
            Log($"Camera shake started - Intensity: {intensity}, Duration: {duration}s");
        }

        /// <summary>
        /// Para o shake imediatamente
        /// </summary>
        public void StopShake()
        {
            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                shakeCoroutine = null;
            }

            shakeOffset = Vector3.zero;
            currentShakeIntensity = 0f;
        }

        /// <summary>
        /// Corrotina do efeito de shake
        /// </summary>
        private IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            currentShakeIntensity = intensity;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                // Diminui a intensidade ao longo do tempo
                currentShakeIntensity = Mathf.Lerp(intensity, 0f, progress);

                yield return null;
            }

            currentShakeIntensity = 0f;
            shakeOffset = Vector3.zero;
            shakeCoroutine = null;
        }

        /// <summary>
        /// Atualiza o offset do shake
        /// </summary>
        private void UpdateShake()
        {
            if (currentShakeIntensity > 0f)
            {
                shakeOffset = Random.insideUnitSphere * currentShakeIntensity;
                shakeOffset.z = 0f; // Mantém Z zero para câmeras 2D

                // Aplica decay natural
                currentShakeIntensity = Mathf.Lerp(currentShakeIntensity, 0f,
                    settings.shakeDecay * Time.deltaTime);
            }
            else
            {
                shakeOffset = Vector3.zero;
            }
        }

        #endregion

        #region Zoom System

        /// <summary>
        /// Faz zoom para um valor específico
        /// </summary>
        public void ZoomTo(float zoomLevel, float duration = 1f)
        {
            if (!settings.enableZoom || mainCamera == null) return;

            zoomLevel = Mathf.Clamp(zoomLevel, settings.minZoom, settings.maxZoom);

            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

            if (duration > 0f)
            {
                zoomCoroutine = StartCoroutine(ZoomCoroutine(zoomLevel, duration));
            }
            else
            {
                mainCamera.orthographicSize = zoomLevel;
                targetZoom = zoomLevel;
            }

            Log($"Zooming to {zoomLevel} over {duration}s");
        }

        /// <summary>
        /// Zoom relativo (adiciona/subtrai do zoom atual)
        /// </summary>
        public void ZoomBy(float zoomDelta, float duration = 1f)
        {
            float newZoom = targetZoom + zoomDelta;
            ZoomTo(newZoom, duration);
        }

        /// <summary>
        /// Reseta o zoom para o valor inicial
        /// </summary>
        public void ResetZoom(float duration = 1f)
        {
            ZoomTo(initialZoom, duration);
        }

        /// <summary>
        /// Corrotina do zoom
        /// </summary>
        private IEnumerator ZoomCoroutine(float targetZoomLevel, float duration)
        {
            float startZoom = mainCamera.orthographicSize;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoomLevel, progress);
                yield return null;
            }

            mainCamera.orthographicSize = targetZoomLevel;
            targetZoom = targetZoomLevel;
            zoomCoroutine = null;
        }

        /// <summary>
        /// Atualiza o sistema de zoom
        /// </summary>
        private void UpdateZoom()
        {
            // Implementação adicional se necessário
        }

        #endregion

        #region Bounds System

        /// <summary>
        /// Define os bounds da câmera
        /// </summary>
        public void SetCameraBounds(Bounds bounds)
        {
            settings.cameraBounds = bounds;
            settings.useBounds = true;
            Log($"Camera bounds set to: {bounds}");
        }

        /// <summary>
        /// Habilita/desabilita o uso de bounds
        /// </summary>
        public void SetUseBounds(bool useBounds)
        {
            settings.useBounds = useBounds;
            Log($"Camera bounds {(useBounds ? "enabled" : "disabled")}");
        }

        #endregion

        #region Utility

        /// <summary>
        /// Converte posição da tela para posição do mundo
        /// </summary>
        public Vector3 ScreenToWorldPoint(Vector3 screenPosition)
        {
            if (mainCamera != null)
            {
                return mainCamera.ScreenToWorldPoint(screenPosition);
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Converte posição do mundo para posição da tela
        /// </summary>
        public Vector3 WorldToScreenPoint(Vector3 worldPosition)
        {
            if (mainCamera != null)
            {
                return mainCamera.WorldToScreenPoint(worldPosition);
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Verifica se um ponto está visível na câmera
        /// </summary>
        public bool IsPointVisible(Vector3 worldPosition)
        {
            if (mainCamera == null) return false;

            Vector3 screenPoint = mainCamera.WorldToViewportPoint(worldPosition);
            return screenPoint.x >= 0 && screenPoint.x <= 1 &&
                   screenPoint.y >= 0 && screenPoint.y <= 1 &&
                   screenPoint.z > 0;
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (settings != null && settings.useBounds)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(settings.cameraBounds.center, settings.cameraBounds.size);
            }
        }

        #endregion
    }
}