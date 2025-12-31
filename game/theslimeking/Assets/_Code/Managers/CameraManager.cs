using System.Collections;
using UnityEngine;
using SlimeKing.Gameplay;
using UnityEngine.Rendering.Universal;
using Unity.Cinemachine;

namespace SlimeKing.Core
{
    public class CameraManager : ManagerSingleton<CameraManager>
    {
        [Header("Camera Management")]
        [SerializeField] private bool enableLogs = true;

        private Camera _mainCamera;
        private UniversalAdditionalCameraData _mainCameraData;
        private CinemachineCamera _cinemachineCamera;

        protected override void Initialize()
        {
            Log("Inicializando Camera Manager...");
            RefreshMainCamera();
        }

        private void Start()
        {
            // Executa após todos os Awake() para garantir que objetos estejam prontos
            RefreshMainCamera();
        }

        /// <summary>
        /// Força a atualização da câmera principal e configurações URP
        /// </summary>
        public void RefreshMainCamera()
        {
            Log("Atualizando configurações da câmera principal...");

            // Encontra todas as câmeras ativas
            var allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            Camera selectedCamera = null;
            float highestDepth = float.MinValue;

            // Seleciona a câmera com maior depth que esteja ativa
            foreach (var cam in allCameras)
            {
                if (cam.gameObject.activeInHierarchy && cam.enabled)
                {
                    if (cam.depth > highestDepth)
                    {
                        highestDepth = cam.depth;
                        selectedCamera = cam;
                    }
                }
            }

            if (selectedCamera == null)
            {
                LogError("Nenhuma câmera ativa encontrada!");
                return;
            }

            SetMainCamera(selectedCamera);
            CleanupDuplicateCameras(selectedCamera);
            SetupCinemachineCamera();
            ForcePostProcessingRefresh();
        }

        private void SetMainCamera(Camera camera)
        {
            _mainCamera = camera;
            _mainCameraData = camera.GetComponent<UniversalAdditionalCameraData>();

            if (_mainCameraData == null)
            {
                Log("Adicionando UniversalAdditionalCameraData à câmera principal...");
                _mainCameraData = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            }

            // Configura como câmera principal
            camera.tag = "MainCamera";

            // Garante configurações URP corretas
            _mainCameraData.renderType = CameraRenderType.Base;
            _mainCameraData.renderPostProcessing = true;
            _mainCameraData.antialiasing = AntialiasingMode.None; // Configurável se necessário
            _mainCameraData.antialiasingQuality = AntialiasingQuality.High;

            Log($"Câmera principal definida: {camera.name} (Depth: {camera.depth})");
        }

        private void SetupCinemachineCamera()
        {
            // Encontra a câmera Cinemachine na cena
            _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

            if (_cinemachineCamera == null)
            {
                Log("Nenhuma Cinemachine Camera encontrada na cena");
                return;
            }

            // Garante que a Cinemachine Camera esteja seguindo o Player
            EnsureCinemachineFollowsPlayer();

            Log($"Cinemachine Camera configurada: {_cinemachineCamera.name}");
        }

        private void EnsureCinemachineFollowsPlayer()
        {
            if (_cinemachineCamera == null) return;

            // Encontra o Player na cena
            Transform playerTransform = GetPlayerTransform();

            if (playerTransform == null)
            {
                LogWarning("Player não encontrado - Cinemachine Camera não pode ser configurada para seguir");
                return;
            }

            // Configura Follow se não estiver configurado ou estiver seguindo objeto incorreto
            if (_cinemachineCamera.Follow != playerTransform)
            {
                _cinemachineCamera.Follow = playerTransform;
                Log($"Cinemachine Camera agora segue o Player: {playerTransform.name}");
            }

            // Configura LookAt se não estiver configurado
            if (_cinemachineCamera.LookAt == null)
            {
                _cinemachineCamera.LookAt = playerTransform;
                Log("Cinemachine Camera LookAt configurado para o Player");
            }
        }

        private Transform GetPlayerTransform()
        {
            // Tenta via PlayerController.Instance primeiro
            if (PlayerController.Instance != null)
            {
                return PlayerController.Instance.transform;
            }

            // Fallback: busca por tag
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                return player.transform;
            }

            // Fallback: busca por nome
            player = GameObject.Find("chr_whiteslime");
            if (player != null)
            {
                return player.transform;
            }

            return null;
        }

        private void CleanupDuplicateCameras(Camera mainCamera)
        {
            var allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            int disabledCount = 0;

            foreach (var cam in allCameras)
            {
                if (cam != mainCamera && cam.gameObject.activeInHierarchy && cam.enabled)
                {
                    // Se for uma câmera duplicada (mesma tag ou função similar)
                    if (cam.tag == "MainCamera" || cam.name.Contains("Main Camera"))
                    {
                        Log($"Desabilitando câmera duplicada: {cam.name}");
                        cam.enabled = false;
                        disabledCount++;
                    }
                }
            }

            if (disabledCount > 0)
            {
                Log($"Desabilitadas {disabledCount} câmera(s) duplicada(s)");
            }
        }

        private void ForcePostProcessingRefresh()
        {
            if (_mainCamera == null || _mainCameraData == null) return;

            // Força refresh do post processing
            _mainCameraData.renderPostProcessing = false;
            _mainCameraData.renderPostProcessing = true;

            // Força refresh dos Global Volumes
            var volumes = FindObjectsByType<UnityEngine.Rendering.Volume>(FindObjectsSortMode.None);
            foreach (var volume in volumes)
            {
                if (volume.isGlobal)
                {
                    volume.enabled = false;
                    volume.enabled = true;
                    Log($"Global Volume refrescado: {volume.name}");
                }
            }

            Log("Post Processing forçadamente atualizado");
        }

        /// <summary>
        /// Chama após carregar nova cena para garantir configuração correta
        /// </summary>
        public void OnSceneLoaded()
        {
            Log("Cena carregada - atualizando configurações de câmera...");

            // Aguarda um frame para garantir que todos os objetos estejam prontos
            StartCoroutine(DelayedCameraRefresh());
        }

        private IEnumerator DelayedCameraRefresh()
        {
            yield return null; // Aguarda 1 frame
            yield return new WaitForEndOfFrame(); // Aguarda fim do frame

            RefreshMainCamera();
        }

        /// <summary>
        /// API pública para outros sistemas forçarem refresh
        /// </summary>
        public void ForceRefresh()
        {
            RefreshMainCamera();
        }

        /// <summary>
        /// Força a configuração da Cinemachine Camera para seguir o Player
        /// Útil quando o Player muda de posição ou após teletransportes
        /// </summary>
        public void ForceCinemachineSetup()
        {
            if (_cinemachineCamera == null)
            {
                _cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
            }

            EnsureCinemachineFollowsPlayer();
        }

        public Camera GetMainCamera() => _mainCamera;

        public UniversalAdditionalCameraData GetMainCameraData() => _mainCameraData;

        public CinemachineCamera GetCinemachineCamera() => _cinemachineCamera;

        private new void Log(string message)
        {
            if (enableLogs)
                UnityEngine.Debug.Log($"[CameraManager] {message}");
        }

        private new void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[CameraManager] {message}");
        }

        private new void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[CameraManager] {message}");
        }

        // Debug via Inspector
        [System.Serializable]
        private class DebugInfo
        {
            public Camera currentMainCamera;
            public bool hasURPData;
            public bool postProcessingEnabled;
            public int totalActiveCameras;
        }

        [Header("Debug Info (Read Only)")]
        [SerializeField] private DebugInfo debugInfo = new DebugInfo();

        private void Update()
        {
            if (enableLogs)
            {
                debugInfo.currentMainCamera = _mainCamera;
                debugInfo.hasURPData = _mainCameraData != null;
                debugInfo.postProcessingEnabled = _mainCameraData?.renderPostProcessing ?? false;
                debugInfo.totalActiveCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None).Length;
            }
        }
    }
}
