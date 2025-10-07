using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// Template especializado para cenas de menu e interface.
    /// Configurações otimizadas para UI com foco em nitidez e responsividade.
    /// </summary>
    [CreateAssetMenu(fileName = "MenuSceneTemplate", menuName = "Extra Tools/Templates/Menu Scene Template")]
    public class MenuSceneTemplate : SceneSetupTemplate
    {
        [Header("Menu Specific Settings")]
        [SerializeField] public bool optimizeForUI = true;
        [SerializeField] public bool enableUIAnimations = true;
        [SerializeField] public float menuOrthographicSize = 1f;

        private void Reset()
        {
            // Set default values for menu scenes
            templateName = "Menu Scene Template";
            description = "Optimized template for menu and UI scenes with high-resolution support and responsive input.";
            targetSceneType = SceneType.Menu;

            // Enable essential modules for menus
            enableManagerModule = true;
            enableCameraModule = false; // módulo de câmera removido
            enableInputModule = true;
            enablePostProcessModule = true;
            enableLightingModule = false; // Usually not needed for UI

            // Manager settings
            requireGameManager = true;
            requireAudioManager = true;
            requireInputManager = true;
            requireEventSystem = true;

            // Camera settings removidos para menus

            // Post processing for visual polish
            requireGlobalVolume = true;
            vignetteIntensity = 0.25f; // More prominent vignette for focus
            vignetteSmoothness = 0.4f;
            colorAdjustmentsContrast = 5f; // Lower contrast for UI readability
            colorAdjustmentsSaturation = 0f; // Neutral colors for UI

            // Input settings for responsive UI
            inputRepeatDelay = 0.3f; // Faster response for menus
            inputRepeatRate = 0.05f; // Very responsive
            pixelDragThreshold = 5; // More sensitive for precise UI interaction

            // Minimal lighting for UI
            requireGlobalLight2D = false;
            globalLightIntensity = 0.8f;
            globalLightColor = Color.white; // Neutral lighting
            ambientLightColor = new Color(0.25f, 0.25f, 0.25f, 1f); // Brighter ambient for UI
        }

        public override void ApplyToSceneSetupManager(SceneSetupManager setupManager)
        {
            base.ApplyToSceneSetupManager(setupManager);

            Debug.Log($"[MenuSceneTemplate] Applied menu-specific configurations:");
            Debug.Log($"• UI Optimization: {optimizeForUI}");
            Debug.Log($"• UI Animations: {enableUIAnimations}");
            Debug.Log($"• Menu Orthographic Size: {menuOrthographicSize}");
        }
    }
}