using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SlimeKing.Core
{
    /// <summary>
    /// Valida e corrige automaticamente configurações de cena
    /// </summary>
    [System.Serializable]
    public class SceneSetupValidator : MonoBehaviour
    {
        [Header("Validação Automática")]
        [SerializeField] private bool autoValidateOnStart = true;
        [SerializeField] private bool enableLogs = true;

        private void Start()
        {
            if (autoValidateOnStart)
            {
                ValidateScene();
            }
        }

        [ContextMenu("Validar Cena")]
        public void ValidateScene()
        {
            Log("Iniciando validação da cena...");

            ValidateCameras();
            ValidateGlobalVolumes();
            ValidateURPSettings();

            Log("Validação concluída!");
        }

        private void ValidateCameras()
        {
            var cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            var mainCameras = 0;

            foreach (var cam in cameras)
            {
                if (cam.tag == "MainCamera") mainCameras++;

                // Garante que câmeras principais tenham URP data
                if (cam.tag == "MainCamera")
                {
                    var urpData = cam.GetComponent<UniversalAdditionalCameraData>();
                    if (urpData == null)
                    {
                        urpData = cam.gameObject.AddComponent<UniversalAdditionalCameraData>();
                        Log($"Adicionado UniversalAdditionalCameraData em {cam.name}");
                    }

                    // Configurações recomendadas
                    urpData.renderType = CameraRenderType.Base;
                    urpData.renderPostProcessing = true;
                }
            }

            if (mainCameras > 1)
            {
                LogWarning($"Múltiplas câmeras principais detectadas ({mainCameras}). Isso pode causar problemas no Post Processing.");
            }
            else if (mainCameras == 0)
            {
                LogWarning("Nenhuma câmera principal encontrada. Atribua a tag 'MainCamera' à câmera principal.");
            }
        }

        private void ValidateGlobalVolumes()
        {
            var volumes = FindObjectsByType<Volume>(FindObjectsSortMode.None);
            var globalVolumes = 0;

            foreach (var volume in volumes)
            {
                if (volume.isGlobal)
                {
                    globalVolumes++;

                    // Valida prioridade
                    if (volume.priority <= 0)
                    {
                        volume.priority = 1;
                        Log($"Prioridade do Global Volume '{volume.name}' ajustada para 1");
                    }
                }
            }

            if (globalVolumes > 1)
            {
                LogWarning($"Múltiplos Global Volumes detectados ({globalVolumes}). Verifique as prioridades.");
            }
        }

        private void ValidateURPSettings()
        {
            var pipeline = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (pipeline == null)
            {
                LogError("URP não está configurado como pipeline padrão!");
                return;
            }

            Log("URP configurado corretamente");
        }

        private void Log(string message)
        {
            if (enableLogs)
                UnityEngine.Debug.Log($"[SceneValidator] {message}");
        }

        private void LogWarning(string message)
        {
            if (enableLogs)
                UnityEngine.Debug.LogWarning($"[SceneValidator] {message}");
        }

        private void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[SceneValidator] {message}");
        }
    }
}
