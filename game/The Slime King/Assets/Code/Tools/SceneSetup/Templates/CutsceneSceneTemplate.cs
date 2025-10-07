using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// Template especializado para cenas de cutscene e cinemáticas.
    /// Configurações otimizadas para narrativa visual com efeitos dramáticos.
    /// </summary>
    [CreateAssetMenu(fileName = "CutsceneSceneTemplate", menuName = "Extra Tools/Templates/Cutscene Scene Template")]
    public class CutsceneSceneTemplate : SceneSetupTemplate
    {
        [Header("Cutscene Specific Settings")]
        [SerializeField] public bool enableCinematicEffects = true;
        [SerializeField] public bool enableAdvancedLighting = true;
        [SerializeField] public float cutsceneOrthographicSize = 5f;
        [SerializeField] public bool allowSkipping = true;

        private void Reset()
        {
            // Set default values for cutscene scenes
            templateName = "Cutscene Scene Template";
            description = "Optimized template for cutscenes with dramatic lighting, cinematic post-processing, and minimal input.";
            targetSceneType = SceneType.Cutscene;

            // Selective modules for cutscenes
            enableManagerModule = true;
            enableCameraModule = false; // módulo de câmera removido
            enableInputModule = true; // Limited input for skipping
            enablePostProcessModule = true;
            enableLightingModule = true; // Important for dramatic effect

            // Manager settings
            requireGameManager = false; // May not need game manager
            requireAudioManager = true; // Important for cutscene audio
            requireInputManager = true; // For skip functionality
            requireEventSystem = false; // Limited UI interaction

            // Camera settings removidos

            // Enhanced post processing for drama
            requireGlobalVolume = true;
            vignetteIntensity = 0.3f; // Strong vignette for cinematic feel
            vignetteSmoothness = 0.4f;
            colorAdjustmentsContrast = 15f; // High contrast for drama
            colorAdjustmentsSaturation = -5f; // Slightly desaturated for cinematic look

            // Input settings for cutscenes
            inputRepeatDelay = 1f; // Slower response
            inputRepeatRate = 0.2f; // Less responsive (intentional)
            pixelDragThreshold = 20; // Less sensitive

            // Dramatic lighting
            requireGlobalLight2D = true;
            globalLightIntensity = 0.9f; // Slightly dimmed
            globalLightColor = new Color(0.9f, 0.9f, 1f, 1f); // Cool lighting
            ambientLightColor = new Color(0.1f, 0.1f, 0.1f, 1f); // Very dark ambient
        }

        public override void ApplyToSceneSetupManager(SceneSetupManager setupManager)
        {
            base.ApplyToSceneSetupManager(setupManager);

            Debug.Log($"[CutsceneSceneTemplate] Applied cutscene-specific configurations:");
            Debug.Log($"• Cinematic Effects: {enableCinematicEffects}");
            Debug.Log($"• Advanced Lighting: {enableAdvancedLighting}");
            Debug.Log($"• Allow Skipping: {allowSkipping}");
            Debug.Log($"• Cutscene Orthographic Size: {cutsceneOrthographicSize}");
        }
    }
}