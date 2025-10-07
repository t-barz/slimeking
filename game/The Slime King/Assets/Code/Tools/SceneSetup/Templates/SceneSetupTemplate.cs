using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// ScriptableObject base para templates de configuração de cenas.
    /// Define configurações padrão que podem ser aplicadas a múltiplas cenas.
    /// </summary>
    [CreateAssetMenu(fileName = "SceneSetupTemplate", menuName = "Extra Tools/Scene Setup Template")]
    public class SceneSetupTemplate : ScriptableObject
    {
        [Header("Template Information")]
        [SerializeField] public string templateName = "New Template";
        [SerializeField][TextArea(3, 5)] public string description = "Template description";
        [SerializeField] public SceneType targetSceneType = SceneType.Gameplay;

        [Header("Module Configuration")]
        [SerializeField] public bool enableManagerModule = true;
        // Camera module removido
        [SerializeField] public bool enableCameraModule = false;
        [SerializeField] public bool enableInputModule = true;
        [SerializeField] public bool enablePostProcessModule = true;
        [SerializeField] public bool enableLightingModule = true;

        [Header("Manager Settings")]
        [SerializeField] public bool requireGameManager = true;
        [SerializeField] public bool requireAudioManager = true;
        [SerializeField] public bool requireInputManager = true;
        [SerializeField] public bool requireEventSystem = true;

        // Camera Settings removidos

        [Header("Post Processing Settings")]
        [SerializeField] public bool requireGlobalVolume = true;
        [SerializeField] public float vignetteIntensity = 0.2f;
        [SerializeField] public float vignetteSmoothness = 0.4f;
        [SerializeField] public float colorAdjustmentsContrast = 10f;
        [SerializeField] public float colorAdjustmentsSaturation = 5f;

        [Header("Input Settings")]
        [SerializeField] public float inputRepeatDelay = 0.5f;
        [SerializeField] public float inputRepeatRate = 0.1f;
        [SerializeField] public int pixelDragThreshold = 10;

        [Header("Lighting Settings")]
        [SerializeField] public bool requireGlobalLight2D = false;
        [SerializeField] public float globalLightIntensity = 1f;
        [SerializeField] public Color globalLightColor = Color.white;
        [SerializeField] public Color ambientLightColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        /// <summary>
        /// Aplica as configurações deste template a um SceneSetupManager
        /// </summary>
        /// <param name="setupManager">O SceneSetupManager a ser configurado</param>
        public virtual void ApplyToSceneSetupManager(SceneSetupManager setupManager)
        {
            if (setupManager == null)
            {
                Debug.LogError("[SceneSetupTemplate] SceneSetupManager is null");
                return;
            }

            Debug.Log($"[SceneSetupTemplate] Applying template '{templateName}' to scene");

            // Note: This requires the SceneSetupManager to have public setters or a configuration method
            // For now, we'll log what would be applied

            LogTemplateApplication();
        }

        /// <summary>
        /// Cria um novo template baseado nas configurações atuais de um SceneSetupManager
        /// </summary>
        /// <param name="setupManager">O SceneSetupManager de onde extrair as configurações</param>
        /// <returns>Novo template com as configurações extraídas</returns>
        public static SceneSetupTemplate CreateFromSceneSetupManager(SceneSetupManager setupManager)
        {
            if (setupManager == null)
            {
                Debug.LogError("[SceneSetupTemplate] Cannot create template from null SceneSetupManager");
                return null;
            }

            var template = CreateInstance<SceneSetupTemplate>();
            template.templateName = $"Template from {setupManager.gameObject.scene.name}";
            template.description = $"Auto-generated template from scene '{setupManager.gameObject.scene.name}'";
            template.targetSceneType = setupManager.SceneType;

            // Extract current settings
            template.requireGameManager = setupManager.RequireGameManager;
            template.requireAudioManager = setupManager.RequireAudioManager;
            template.requireInputManager = setupManager.RequireInputManager;
            template.requireEventSystem = setupManager.RequireEventSystem;

            // Camera settings removidos

            template.requireGlobalVolume = setupManager.RequireGlobalVolume;
            template.requireGlobalLight2D = setupManager.RequireGlobalLight2D;

            Debug.Log($"[SceneSetupTemplate] Created template from {setupManager.gameObject.scene.name}");

            return template;
        }

        /// <summary>
        /// Valida se este template é compatível com o tipo de cena especificado
        /// </summary>
        /// <param name="sceneType">Tipo de cena a validar</param>
        /// <returns>True se for compatível</returns>
        public bool IsCompatibleWithSceneType(SceneType sceneType)
        {
            return targetSceneType == SceneType.Auto || targetSceneType == sceneType;
        }

        /// <summary>
        /// Obtém uma descrição resumida das configurações do template
        /// </summary>
        /// <returns>String com resumo das configurações</returns>
        public string GetConfigurationSummary()
        {
            var enabledModules = 0;
            if (enableManagerModule) enabledModules++;
            if (enableCameraModule) enabledModules++;
            if (enableInputModule) enabledModules++;
            if (enablePostProcessModule) enabledModules++;
            if (enableLightingModule) enabledModules++;

            return $"Target: {targetSceneType} | Modules: {enabledModules}/5 enabled | " +
                   $"PixelPerfect: (removido) | " +
                   $"PostProcess: {(requireGlobalVolume ? "Yes" : "No")} | " +
                   $"2D Lighting: {(requireGlobalLight2D ? "Yes" : "No")}";
        }

        private void LogTemplateApplication()
        {
            Debug.Log($"[SceneSetupTemplate] Template Configuration Summary:\n" +
                     $"• Target Scene Type: {targetSceneType}\n" +
                     $"• Modules Enabled: Manager={enableManagerModule}, Camera={enableCameraModule}, Input={enableInputModule}, PostProcess={enablePostProcessModule}, Lighting={enableLightingModule}\n" +
                     $"• Camera: (configuração removida)\n" +
                     $"• Post Process: GlobalVolume={requireGlobalVolume}, Vignette={vignetteIntensity}, Contrast={colorAdjustmentsContrast}\n" +
                     $"• Lighting: GlobalLight2D={requireGlobalLight2D}, Intensity={globalLightIntensity}\n" +
                     $"• Input: RepeatDelay={inputRepeatDelay}, DragThreshold={pixelDragThreshold}");
        }
    }
}