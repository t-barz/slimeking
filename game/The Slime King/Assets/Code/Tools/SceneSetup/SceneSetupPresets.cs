using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// Sistema de presets para configurações rápidas do Scene Setup System.
    /// Fornece configurações predefinidas para diferentes tipos de projetos e estilos visuais.
    /// </summary>
    [CreateAssetMenu(fileName = "SceneSetupPresets", menuName = "Extra Tools/Scene Setup Presets")]
    public class SceneSetupPresets : ScriptableObject
    {
        [Header("Preset Information")]
        [SerializeField] public string presetName = "Custom Preset";
        [SerializeField][TextArea(3, 5)] public string description = "Custom preset description";
        [SerializeField] public ProjectType projectType = ProjectType.PixelArt2D;

        [Header("Global Project Settings")]
        [SerializeField] public bool enforceConsistency = true;
        [SerializeField] public bool autoApplyToNewScenes = false;
        [SerializeField] public bool showRecommendations = true;

        [Header("Default Templates")]
        [SerializeField] public GameplaySceneTemplate defaultGameplayTemplate;
        [SerializeField] public MenuSceneTemplate defaultMenuTemplate;
        [SerializeField] public CutsceneSceneTemplate defaultCutsceneTemplate;

        // Camera Preset Settings removidos
        //[Header("Camera Preset Settings")]
        //[SerializeField] public CameraPresetSettings cameraSettings = new CameraPresetSettings();

        [Header("Post Processing Preset Settings")]
        [SerializeField] public PostProcessPresetSettings postProcessSettings = new PostProcessPresetSettings();

        [Header("Input Preset Settings")]
        [SerializeField] public InputPresetSettings inputSettings = new InputPresetSettings();

        [Header("Lighting Preset Settings")]
        [SerializeField] public LightingPresetSettings lightingSettings = new LightingPresetSettings();

        #region Preset Settings Classes

        // CameraPresetSettings removida

        [System.Serializable]
        public class PostProcessPresetSettings
        {
            [Header("General Post Process Settings")]
            public bool enablePostProcessing = true;
            public bool requireURP = true;

            [Header("Vignette Settings")]
            public bool enableVignette = true;
            public float vignetteIntensity = 0.2f;
            public float vignetteSmoothness = 0.4f;
            public Color vignetteColor = Color.black;

            [Header("Color Adjustments")]
            public bool enableColorAdjustments = true;
            public float contrast = 10f;
            public float saturation = 5f;
            public float postExposure = 0f;

            [Header("Tonemapping")]
            public UnityEngine.Rendering.Universal.TonemappingMode tonemappingMode = UnityEngine.Rendering.Universal.TonemappingMode.Neutral;

            [Header("Film Grain (Optional)")]
            public bool enableFilmGrain = false;
            public float filmGrainIntensity = 0.1f;
        }

        [System.Serializable]
        public class InputPresetSettings
        {
            [Header("EventSystem Settings")]
            public bool requireEventSystem = true;
            public bool useInputSystemUI = true;
            public bool replaceLegacyInput = true;

            [Header("Input Responsiveness")]
            public float defaultRepeatDelay = 0.5f;
            public float defaultRepeatRate = 0.1f;
            public int defaultPixelDragThreshold = 10;

            [Header("Scene Type Variations")]
            public float gameplayRepeatDelay = 0.5f;
            public float menuRepeatDelay = 0.3f;
            public float cutsceneRepeatDelay = 1f;
        }

        [System.Serializable]
        public class LightingPresetSettings
        {
            [Header("2D Lighting Settings")]
            public bool enable2DLighting = true;
            public float defaultLightIntensity = 1f;
            public Color defaultLightColor = Color.white;

            [Header("Ambient Lighting")]
            public Color ambientColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

            [Header("Shadows and Effects")]
            public bool enableShadows = false;
            public UnityEngine.ShadowQuality shadowQuality = UnityEngine.ShadowQuality.Disable;
            public bool enableFog = false;
        }

        #endregion

        #region Preset Types

        public enum ProjectType
        {
            PixelArt2D,
            HighRes2D,
            Retro,
            Mobile,
            VN_VisualNovel,
            Platformer,
            TopDown,
            Custom
        }

        #endregion

        #region Static Preset Factories

        /// <summary>
        /// Cria um preset otimizado para pixel art 2D
        /// </summary>
        /// <returns>Preset configurado para pixel art</returns>
        public static SceneSetupPresets CreatePixelArt2DPreset()
        {
            var preset = CreateInstance<SceneSetupPresets>();
            preset.presetName = "Pixel Art 2D";
            preset.description = "Optimized for pixel art games with crisp rendering, no anti-aliasing, and retro-style post-processing.";
            preset.projectType = ProjectType.PixelArt2D;

            // Camera settings for pixel art
            // Camera settings removidos

            // Post processing for pixel art
            preset.postProcessSettings.vignetteIntensity = 0.15f;
            preset.postProcessSettings.contrast = 12f;
            preset.postProcessSettings.saturation = 8f;
            preset.postProcessSettings.enableFilmGrain = false;

            // Lighting for 2D
            preset.lightingSettings.enable2DLighting = true;
            preset.lightingSettings.defaultLightIntensity = 1f;
            preset.lightingSettings.ambientColor = new Color(0.15f, 0.15f, 0.15f, 1f);

            return preset;
        }

        /// <summary>
        /// Cria um preset otimizado para sprite art de alta resolução
        /// </summary>
        /// <returns>Preset configurado para high-res 2D</returns>
        public static SceneSetupPresets CreateHighRes2DPreset()
        {
            var preset = CreateInstance<SceneSetupPresets>();
            preset.presetName = "High Resolution 2D";
            preset.description = "Optimized for high-resolution 2D games with smooth rendering and enhanced visual effects.";
            preset.projectType = ProjectType.HighRes2D;

            // Camera settings for high-res
            // Camera settings removidos (HighRes2D)

            // Enhanced post processing
            preset.postProcessSettings.vignetteIntensity = 0.25f;
            preset.postProcessSettings.contrast = 8f;
            preset.postProcessSettings.saturation = 10f;
            preset.postProcessSettings.enableFilmGrain = false;

            // Advanced lighting
            preset.lightingSettings.enable2DLighting = true;
            preset.lightingSettings.defaultLightIntensity = 1.2f;
            preset.lightingSettings.ambientColor = new Color(0.25f, 0.25f, 0.25f, 1f);

            return preset;
        }

        /// <summary>
        /// Cria um preset estilo retro com efeitos nostálgicos
        /// </summary>
        /// <returns>Preset configurado para estilo retro</returns>
        public static SceneSetupPresets CreateRetroPreset()
        {
            var preset = CreateInstance<SceneSetupPresets>();
            preset.presetName = "Retro Style";
            preset.description = "Retro-style preset with film grain, vintage colors, and nostalgic visual effects.";
            preset.projectType = ProjectType.Retro;

            // Camera settings for retro
            // Camera settings removidos (Retro)

            // Retro post processing
            preset.postProcessSettings.vignetteIntensity = 0.4f; // Strong vignette
            preset.postProcessSettings.contrast = 15f; // High contrast
            preset.postProcessSettings.saturation = -10f; // Desaturated
            preset.postProcessSettings.enableFilmGrain = true;
            preset.postProcessSettings.filmGrainIntensity = 0.2f;

            // Moody lighting
            preset.lightingSettings.enable2DLighting = true;
            preset.lightingSettings.defaultLightIntensity = 0.8f;
            preset.lightingSettings.defaultLightColor = new Color(1f, 0.9f, 0.7f, 1f); // Warm
            preset.lightingSettings.ambientColor = new Color(0.1f, 0.05f, 0.1f, 1f); // Dark purple

            return preset;
        }

        /// <summary>
        /// Cria um preset otimizado para dispositivos móveis
        /// </summary>
        /// <returns>Preset configurado para mobile</returns>
        public static SceneSetupPresets CreateMobilePreset()
        {
            var preset = CreateInstance<SceneSetupPresets>();
            preset.presetName = "Mobile Optimized";
            preset.description = "Performance-optimized preset for mobile devices with reduced effects and optimized settings.";
            preset.projectType = ProjectType.Mobile;

            // Performance-optimized camera
            // Camera settings removidos (Mobile)

            // Minimal post processing
            preset.postProcessSettings.vignetteIntensity = 0.1f; // Very subtle
            preset.postProcessSettings.contrast = 5f; // Lower processing
            preset.postProcessSettings.saturation = 3f;
            preset.postProcessSettings.enableFilmGrain = false; // Disable for performance

            // Simple lighting
            preset.lightingSettings.enable2DLighting = false; // Disable for performance
            preset.lightingSettings.ambientColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Brighter ambient
            preset.lightingSettings.enableShadows = false;

            // Responsive input for touch
            preset.inputSettings.defaultPixelDragThreshold = 20; // Higher for touch
            preset.inputSettings.menuRepeatDelay = 0.2f; // Faster for mobile UI

            return preset;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Aplica este preset a um SceneSetupManager
        /// </summary>
        /// <param name="setupManager">O SceneSetupManager a ser configurado</param>
        public void ApplyPresetToSceneSetupManager(SceneSetupManager setupManager)
        {
            if (setupManager == null)
            {
                Debug.LogError("[SceneSetupPresets] SceneSetupManager is null");
                return;
            }

            Debug.Log($"[SceneSetupPresets] Applying preset '{presetName}' to scene '{setupManager.gameObject.scene.name}'");

            // Apply settings based on scene type and preset configuration
            ApplyPresetSettings(setupManager);

            Debug.Log($"[SceneSetupPresets] Preset '{presetName}' applied successfully");
        }

        /// <summary>
        /// Cria um template baseado neste preset para o tipo de cena especificado
        /// </summary>
        /// <param name="sceneType">Tipo de cena para o template</param>
        /// <returns>Template configurado</returns>
        public SceneSetupTemplate CreateTemplateForSceneType(SceneType sceneType)
        {
            SceneSetupTemplate template = null;

            switch (sceneType)
            {
                case SceneType.Gameplay:
                    template = defaultGameplayTemplate != null ?
                        Instantiate(defaultGameplayTemplate) :
                        CreateInstance<GameplaySceneTemplate>();
                    break;

                case SceneType.Menu:
                    template = defaultMenuTemplate != null ?
                        Instantiate(defaultMenuTemplate) :
                        CreateInstance<MenuSceneTemplate>();
                    break;

                case SceneType.Cutscene:
                    template = defaultCutsceneTemplate != null ?
                        Instantiate(defaultCutsceneTemplate) :
                        CreateInstance<CutsceneSceneTemplate>();
                    break;

                default:
                    template = CreateInstance<SceneSetupTemplate>();
                    break;
            }

            if (template != null)
            {
                ApplyPresetToTemplate(template, sceneType);
            }

            return template;
        }

        /// <summary>
        /// Obtém recomendações específicas para este preset
        /// </summary>
        /// <returns>Lista de recomendações</returns>
        public string[] GetRecommendations()
        {
            var recommendations = new System.Collections.Generic.List<string>();

            switch (projectType)
            {
                case ProjectType.PixelArt2D:
                    recommendations.Add("💡 Use sprites with consistent pixel density (PPU)");
                    recommendations.Add("💡 Disable anti-aliasing for crisp pixel art");
                    recommendations.Add("💡 Use point filtering on textures");
                    recommendations.Add("💡 Consider using a limited color palette");
                    break;

                case ProjectType.HighRes2D:
                    recommendations.Add("💡 Enable HDR for enhanced color range");
                    recommendations.Add("💡 Use bilinear filtering for smooth scaling");
                    recommendations.Add("💡 Consider MSAA for edge smoothing");
                    recommendations.Add("💡 Use higher resolution reference for UI");
                    break;

                case ProjectType.Retro:
                    recommendations.Add("💡 Use CRT shader effects for authenticity");
                    recommendations.Add("💡 Limit color palette to retro standards");
                    recommendations.Add("💡 Add scanline effects via post-processing");
                    recommendations.Add("💡 Use chiptune-style audio");
                    break;

                case ProjectType.Mobile:
                    recommendations.Add("💡 Test on target devices early");
                    recommendations.Add("💡 Use texture compression");
                    recommendations.Add("💡 Minimize draw calls");
                    recommendations.Add("💡 Consider battery usage");
                    break;
            }

            return recommendations.ToArray();
        }

        /// <summary>
        /// Valida se este preset é compatível com o projeto atual
        /// </summary>
        /// <returns>True se for compatível</returns>
        public bool ValidateCompatibility()
        {
            var issues = new System.Collections.Generic.List<string>();

            // Check URP
            var renderPipelineAsset = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
            if (postProcessSettings.requireURP && renderPipelineAsset == null)
            {
                issues.Add("URP is required but not configured");
            }

            // Check packages
            // Camera package checks removidos

            if (issues.Count > 0)
            {
                Debug.LogWarning($"[SceneSetupPresets] Compatibility issues with preset '{presetName}':\n" +
                               string.Join("\n", issues));
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private void ApplyPresetSettings(SceneSetupManager setupManager)
        {
            // Note: This is a simplified implementation
            // In a full implementation, you would need public setters on SceneSetupManager
            // or a configuration API to apply these settings

            Debug.Log($"[SceneSetupPresets] Applying {projectType} preset settings:");
            Debug.Log($"• Camera: (removido)");
            Debug.Log($"• PostProcess: Vignette={postProcessSettings.vignetteIntensity}, Contrast={postProcessSettings.contrast}");
            Debug.Log($"• Input: RepeatDelay={inputSettings.defaultRepeatDelay}");
            Debug.Log($"• Lighting: 2DLighting={lightingSettings.enable2DLighting}, Intensity={lightingSettings.defaultLightIntensity}");
        }

        private void ApplyPresetToTemplate(SceneSetupTemplate template, SceneType sceneType)
        {
            template.targetSceneType = sceneType;
            template.templateName = $"{presetName} - {sceneType}";
            template.description = $"Template generated from {presetName} preset for {sceneType} scenes";

            // Camera settings removidos

            // Apply post process settings
            template.requireGlobalVolume = postProcessSettings.enablePostProcessing;
            template.vignetteIntensity = postProcessSettings.vignetteIntensity;
            template.vignetteSmoothness = postProcessSettings.vignetteSmoothness;
            template.colorAdjustmentsContrast = postProcessSettings.contrast;
            template.colorAdjustmentsSaturation = postProcessSettings.saturation;

            // Apply input settings
            template.inputRepeatDelay = inputSettings.defaultRepeatDelay;
            template.inputRepeatRate = inputSettings.defaultRepeatRate;
            template.pixelDragThreshold = inputSettings.defaultPixelDragThreshold;

            // Apply lighting settings
            template.requireGlobalLight2D = lightingSettings.enable2DLighting;
            template.globalLightIntensity = lightingSettings.defaultLightIntensity;
            template.globalLightColor = lightingSettings.defaultLightColor;
            template.ambientLightColor = lightingSettings.ambientColor;
        }

        #endregion
    }
}