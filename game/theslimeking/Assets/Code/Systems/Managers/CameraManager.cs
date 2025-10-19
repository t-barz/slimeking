using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SlimeKing.Core
{
    public class CameraManager : ManagerSingleton<CameraManager>
    {
        [Header("Camera Management")]
        [SerializeField] private bool enableLogs = true;

        private Camera _mainCamera;
        private UniversalAdditionalCameraData _mainCameraData;

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

        public Camera GetMainCamera() => _mainCamera;

        public UniversalAdditionalCameraData GetMainCameraData() => _mainCameraData;

        private new void Log(string message)
        {
            if (enableLogs)
                Debug.Log($"[CameraManager] {message}");
        }

        private new void LogError(string message)
        {
            Debug.LogError($"[CameraManager] {message}");
        }

        private new void LogWarning(string message)
        {
            Debug.LogWarning($"[CameraManager] {message}");
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