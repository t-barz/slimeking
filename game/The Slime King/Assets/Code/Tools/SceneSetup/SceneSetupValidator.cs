using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace ExtraTools
{
    /// <summary>
    /// Validador avançado para o Scene Setup System.
    /// Utiliza os métodos de validação dos módulos especializados para análise detalhada.
    /// </summary>
    public static class SceneSetupValidator
    {
        #region Validation Results

        [System.Serializable]
        public class ValidationResult
        {
            public string sceneName;
            public string scenePath;
            public SceneType sceneType;
            public bool isValid;
            public int errorCount;
            public int warningCount;
            public List<ValidationIssue> issues = new List<ValidationIssue>();

            public ValidationResult(string name, string path)
            {
                sceneName = name;
                scenePath = path;
                isValid = true;
                errorCount = 0;
                warningCount = 0;
            }

            public void AddIssue(ValidationIssue issue)
            {
                issues.Add(issue);

                if (issue.severity == IssueSeverity.Error)
                {
                    errorCount++;
                    isValid = false;
                }
                else if (issue.severity == IssueSeverity.Warning)
                {
                    warningCount++;
                }
            }

            public string GetStatusIcon()
            {
                if (errorCount > 0) return "❌";
                if (warningCount > 0) return "⚠️";
                return "✅";
            }

            public string GetSummary()
            {
                if (errorCount == 0 && warningCount == 0)
                    return "All checks passed";

                return $"{errorCount} errors, {warningCount} warnings";
            }
        }

        [System.Serializable]
        public class ValidationIssue
        {
            public string module;
            public IssueSeverity severity;
            public string title;
            public string description;
            public string fixSuggestion;

            public ValidationIssue(string mod, IssueSeverity sev, string tit, string desc, string fix = "")
            {
                module = mod;
                severity = sev;
                title = tit;
                description = desc;
                fixSuggestion = fix;
            }

            public string GetIcon()
            {
                return severity == IssueSeverity.Error ? "❌" : "⚠️";
            }
        }

        public enum IssueSeverity
        {
            Warning,
            Error
        }

        #endregion

        #region Public Validation Methods

        /// <summary>
        /// Valida uma cena específica usando todos os módulos disponíveis
        /// </summary>
        /// <param name="scenePath">Caminho para a cena</param>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateScene(string scenePath)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            var result = new ValidationResult(sceneName, scenePath);

            Debug.Log($"[SceneSetupValidator] Starting validation of scene: {sceneName}");

            // Load scene additively for analysis
            Scene originalActiveScene = SceneManager.GetActiveScene();
            Scene sceneToValidate = default;

            try
            {
                sceneToValidate = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

                if (!sceneToValidate.IsValid())
                {
                    result.AddIssue(new ValidationIssue("System", IssueSeverity.Error,
                        "Scene Load Failed", "Could not load scene for validation"));
                    return result;
                }

                // Find SceneSetupManager
                SceneSetupManager setupManager = FindSceneSetupManagerInScene(sceneToValidate);

                if (setupManager == null)
                {
                    result.AddIssue(new ValidationIssue("Core", IssueSeverity.Error,
                        "Missing SceneSetupManager", "Scene does not have a SceneSetupManager component",
                        "Add SceneSetupManager via 'GameObject > Extra Tools > Add Scene Setup Manager'"));

                    // Try to detect scene type anyway
                    result.sceneType = DetectSceneTypeFromName(sceneName);
                }
                else
                {
                    result.sceneType = setupManager.SceneType;

                    // Validate using modules
                    ValidateWithModules(setupManager, result);
                }

                // Additional scene-level validations
                ValidateSceneStructure(sceneToValidate, result);

            }
            catch (System.Exception e)
            {
                result.AddIssue(new ValidationIssue("System", IssueSeverity.Error,
                    "Validation Exception", $"Error during validation: {e.Message}"));
            }
            finally
            {
                // Clean up - close the scene if it was opened for validation
                if (sceneToValidate.IsValid() && sceneToValidate != originalActiveScene)
                {
                    EditorSceneManager.CloseScene(sceneToValidate, true);
                }
            }

            Debug.Log($"[SceneSetupValidator] Validation completed for {sceneName}: {result.GetSummary()}");
            return result;
        }

        /// <summary>
        /// Valida todas as cenas do projeto
        /// </summary>
        /// <returns>Lista de resultados de validação</returns>
        public static List<ValidationResult> ValidateAllScenes()
        {
            Debug.Log("[SceneSetupValidator] Starting validation of all project scenes");

            var results = new List<ValidationResult>();
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

            for (int i = 0; i < sceneGuids.Length; i++)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                // Show progress
                EditorUtility.DisplayProgressBar("Validating Scenes",
                    $"Validating: {sceneName}", (float)i / sceneGuids.Length);

                var result = ValidateScene(scenePath);
                results.Add(result);
            }

            EditorUtility.ClearProgressBar();

            // Generate summary
            int totalScenes = results.Count;
            int validScenes = results.Count(r => r.isValid);
            int errorScenes = results.Count(r => r.errorCount > 0);
            int warningScenes = results.Count(r => r.warningCount > 0);

            Debug.Log($"[SceneSetupValidator] Validation Summary: {validScenes}/{totalScenes} valid, {errorScenes} with errors, {warningScenes} with warnings");

            return results;
        }

        /// <summary>
        /// Valida apenas a cena ativa no momento
        /// </summary>
        /// <returns>Resultado da validação</returns>
        public static ValidationResult ValidateActiveScene()
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (!activeScene.IsValid())
            {
                var result = new ValidationResult("Invalid Scene", "");
                result.AddIssue(new ValidationIssue("System", IssueSeverity.Error,
                    "No Active Scene", "No valid active scene found"));
                return result;
            }

            var validationResult = new ValidationResult(activeScene.name, activeScene.path);
            validationResult.sceneType = DetectSceneTypeFromName(activeScene.name);

            // Find SceneSetupManager in active scene
            SceneSetupManager setupManager = FindSceneSetupManagerInScene(activeScene);

            if (setupManager == null)
            {
                validationResult.AddIssue(new ValidationIssue("Core", IssueSeverity.Error,
                    "Missing SceneSetupManager", "Active scene does not have a SceneSetupManager component"));
            }
            else
            {
                validationResult.sceneType = setupManager.SceneType;
                ValidateWithModules(setupManager, validationResult);
            }

            ValidateSceneStructure(activeScene, validationResult);

            return validationResult;
        }

        #endregion

        #region Private Validation Methods

        private static void ValidateWithModules(SceneSetupManager setupManager, ValidationResult result)
        {
            // Validate Manager Module
            ValidateManagerModule(setupManager, result);

            // Camera module removido - sem validação de câmera

            // Validate Input Module
            ValidateInputModule(setupManager, result);

            // Validate Post Process Module
            ValidatePostProcessModule(setupManager, result);

            // Validate Lighting Module
            ValidateLightingModule(setupManager, result);
        }

        private static void ValidateManagerModule(SceneSetupManager setupManager, ValidationResult result)
        {
            // Check GameManager
            if (setupManager.RequireGameManager && GameManager.Instance == null)
            {
                result.AddIssue(new ValidationIssue("Manager", IssueSeverity.Error,
                    "Missing GameManager", "GameManager is required but not found in scene",
                    "Run scene setup or manually add GameManager"));
            }

            // Check AudioManager
            if (setupManager.RequireAudioManager && AudioManager.Instance == null)
            {
                result.AddIssue(new ValidationIssue("Manager", IssueSeverity.Error,
                    "Missing AudioManager", "AudioManager is required but not found in scene",
                    "Run scene setup or manually add AudioManager"));
            }

            // Check InputManager
            if (setupManager.RequireInputManager && InputManager.Instance == null)
            {
                result.AddIssue(new ValidationIssue("Manager", IssueSeverity.Error,
                    "Missing InputManager", "InputManager is required but not found in scene",
                    "Run scene setup or manually add InputManager"));
            }
        }

        // Método ValidateCameraModule removido

        private static void ValidateInputModule(SceneSetupManager setupManager, ValidationResult result)
        {
            if (setupManager.RequireEventSystem)
            {
                var eventSystem = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
                if (eventSystem == null)
                {
                    result.AddIssue(new ValidationIssue("Input", IssueSeverity.Error,
                        "Missing EventSystem", "EventSystem is required but not found in scene",
                        "Run scene setup or manually add EventSystem"));
                }
                else
                {
                    // Check for Input System UI Module
                    var inputModule = eventSystem.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    if (inputModule == null)
                    {
                        var legacyModule = eventSystem.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                        if (legacyModule != null)
                        {
                            result.AddIssue(new ValidationIssue("Input", IssueSeverity.Warning,
                                "Using Legacy Input Module", "StandaloneInputModule detected - consider upgrading to InputSystemUIInputModule",
                                "Run scene setup to upgrade to New Input System"));
                        }
                        else
                        {
                            result.AddIssue(new ValidationIssue("Input", IssueSeverity.Warning,
                                "Missing Input Module", "No input module found on EventSystem",
                                "Run scene setup or manually add InputSystemUIInputModule"));
                        }
                    }
                }
            }
        }

        private static void ValidatePostProcessModule(SceneSetupManager setupManager, ValidationResult result)
        {
            // Check URP
            var renderPipelineAsset = UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline;
            if (renderPipelineAsset == null)
            {
                result.AddIssue(new ValidationIssue("PostProcess", IssueSeverity.Error,
                    "URP Not Configured", "Universal Render Pipeline is not configured",
                    "Configure URP in Project Settings > Graphics"));
                return;
            }

            if (setupManager.RequireGlobalVolume)
            {
                var globalVolume = Object.FindFirstObjectByType<UnityEngine.Rendering.Volume>();
                if (globalVolume == null)
                {
                    result.AddIssue(new ValidationIssue("PostProcess", IssueSeverity.Warning,
                        "Missing Global Volume", "Global Volume is required but not found in scene",
                        "Run scene setup or manually add Global Volume"));
                }
                else if (!globalVolume.isGlobal)
                {
                    result.AddIssue(new ValidationIssue("PostProcess", IssueSeverity.Warning,
                        "Volume Not Global", "Volume found but not set as Global",
                        "Enable 'Is Global' on the Volume component"));
                }
            }
        }

        private static void ValidateLightingModule(SceneSetupManager setupManager, ValidationResult result)
        {
            if (setupManager.RequireGlobalLight2D)
            {
#if UNITY_2D_RENDERER
                var globalLights = Object.FindObjectsByType<UnityEngine.Rendering.Universal.Light2D>(FindObjectsSortMode.None);
                bool hasGlobalLight = globalLights.Any(light => light.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Global);
                
                if (!hasGlobalLight)
                {
                    result.AddIssue(new ValidationIssue("Lighting", IssueSeverity.Warning, 
                        "Missing Global Light 2D", "Global Light 2D is required but not found in scene",
                        "Run scene setup or manually add Global Light 2D"));
                }
#else
                result.AddIssue(new ValidationIssue("Lighting", IssueSeverity.Warning,
                    "2D Renderer Not Available", "2D Renderer is not available for Global Light 2D",
                    "Add 2D Renderer to URP Asset's Renderer List"));
#endif
            }
        }

        private static void ValidateSceneStructure(Scene scene, ValidationResult result)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();

            // Check for basic scene structure
            if (rootObjects.Length == 0)
            {
                result.AddIssue(new ValidationIssue("Structure", IssueSeverity.Warning,
                    "Empty Scene", "Scene has no root GameObjects",
                    "Add some content to the scene"));
            }

            // Check for multiple cameras
            var cameras = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)
                .Where(cam => cam.gameObject.scene == scene).ToArray();

            if (cameras.Length > 1)
            {
                result.AddIssue(new ValidationIssue("Structure", IssueSeverity.Warning,
                    "Multiple Cameras", $"Scene has {cameras.Length} cameras - consider if all are needed",
                    "Review camera setup for potential conflicts"));
            }
        }

        #endregion

        #region Utility Methods

        private static SceneSetupManager FindSceneSetupManagerInScene(Scene scene)
        {
            GameObject[] rootObjects = scene.GetRootGameObjects();

            foreach (var rootObject in rootObjects)
            {
                var setupManager = rootObject.GetComponentInChildren<SceneSetupManager>();
                if (setupManager != null)
                {
                    return setupManager;
                }
            }

            return null;
        }

        private static SceneType DetectSceneTypeFromName(string sceneName)
        {
            string lowerName = sceneName.ToLower();

            if (lowerName.Contains("title") || lowerName.Contains("menu") || lowerName.Contains("main"))
            {
                return SceneType.Menu;
            }
            else if (lowerName.Contains("cutscene") || lowerName.Contains("intro") || lowerName.Contains("ending"))
            {
                return SceneType.Cutscene;
            }
            else
            {
                return SceneType.Gameplay;
            }
        }

        /// <summary>
        /// Gera um relatório detalhado em formato texto
        /// </summary>
        /// <param name="results">Resultados da validação</param>
        /// <returns>Relatório formatado</returns>
        public static string GenerateReport(List<ValidationResult> results)
        {
            var report = new System.Text.StringBuilder();

            report.AppendLine("🛠️ SCENE SETUP VALIDATION REPORT");
            report.AppendLine("=" + new string('=', 50));
            report.AppendLine($"Generated: {System.DateTime.Now}");
            report.AppendLine();

            // Summary
            int totalScenes = results.Count;
            int validScenes = results.Count(r => r.isValid);
            int errorScenes = results.Count(r => r.errorCount > 0);
            int warningScenes = results.Count(r => r.warningCount > 0);

            report.AppendLine("📊 SUMMARY");
            report.AppendLine($"Total Scenes: {totalScenes}");
            report.AppendLine($"Valid Scenes: {validScenes} ({(float)validScenes / totalScenes * 100:F1}%)");
            report.AppendLine($"Scenes with Errors: {errorScenes}");
            report.AppendLine($"Scenes with Warnings: {warningScenes}");
            report.AppendLine();

            // Detailed results
            report.AppendLine("📋 DETAILED RESULTS");
            report.AppendLine("-" + new string('-', 50));

            foreach (var result in results.OrderBy(r => r.sceneName))
            {
                report.AppendLine($"{result.GetStatusIcon()} {result.sceneName} ({result.sceneType})");
                report.AppendLine($"   Path: {result.scenePath}");
                report.AppendLine($"   Status: {result.GetSummary()}");

                if (result.issues.Count > 0)
                {
                    foreach (var issue in result.issues)
                    {
                        report.AppendLine($"   {issue.GetIcon()} [{issue.module}] {issue.title}");
                        if (!string.IsNullOrEmpty(issue.fixSuggestion))
                        {
                            report.AppendLine($"      Fix: {issue.fixSuggestion}");
                        }
                    }
                }

                report.AppendLine();
            }

            return report.ToString();
        }

        #endregion
    }
}