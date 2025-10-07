using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// Template especializado para cenas de gameplay.
    /// Configurações otimizadas para jogabilidade com pixel art.
    /// </summary>
    [CreateAssetMenu(fileName = "GameplaySceneTemplate", menuName = "Extra Tools/Templates/Gameplay Scene Template")]
    public class GameplaySceneTemplate : SceneSetupTemplate
    {
        [Header("Gameplay Specific Settings")]
        // Cinemachine support removido
        [SerializeField] public bool enableCinemachineSupport = false;
        [SerializeField] public bool enableRetroEffects = false;
        [SerializeField] public float gameplayOrthographicSize = 5f;

        private void Reset()
        {
            // Set default values for gameplay scenes
            templateName = "Gameplay Scene Template";
            description = "Optimized template for gameplay scenes with pixel art support, camera controls, and full post-processing.";
            targetSceneType = SceneType.Gameplay;

            // Enable all modules for gameplay
            enableManagerModule = true;
            enableCameraModule = false; // módulo de câmera removido
            enableInputModule = true;
            enablePostProcessModule = true;
            enableLightingModule = true;

            // Manager settings
            requireGameManager = true;
            requireAudioManager = true;
            requireInputManager = true;
            requireEventSystem = true;

            // Camera settings removidos

            // Post processing for visual appeal
            requireGlobalVolume = true;
            vignetteIntensity = 0.15f; // Subtle vignette for gameplay
            vignetteSmoothness = 0.4f;
            colorAdjustmentsContrast = 12f; // Higher contrast for pixel art
            colorAdjustmentsSaturation = 8f; // Vibrant colors for gameplay

            // Input settings for responsive gameplay
            inputRepeatDelay = 0.5f;
            inputRepeatRate = 0.1f;
            pixelDragThreshold = 15; // Less sensitive for gameplay

            // Lighting for 2D gameplay
            requireGlobalLight2D = true;
            globalLightIntensity = 1f;
            globalLightColor = new Color(1f, 0.95f, 0.9f, 1f); // Slightly warm
            ambientLightColor = new Color(0.15f, 0.15f, 0.15f, 1f); // Darker ambient
        }

        public override void ApplyToSceneSetupManager(SceneSetupManager setupManager)
        {
            base.ApplyToSceneSetupManager(setupManager);

            Debug.Log($"[GameplaySceneTemplate] Applied gameplay-specific configurations:");
            Debug.Log($"• Cinemachine Support: (removido) {enableCinemachineSupport}");
            Debug.Log($"• Retro Effects: {enableRetroEffects}");
            Debug.Log($"• Orthographic Size: {gameplayOrthographicSize}");
        }
    }
}