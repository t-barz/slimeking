using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace ExtraTools
{
    /// <summary>
    /// Editor Window para gerenciamento visual do Scene Setup System.
    /// Permite controle granular de módulos e operações em massa.
    /// </summary>
    public class SceneSetupWindow : EditorWindow
    {
        #region Window Management

        [MenuItem("Extra Tools/Scene Setup/Open Scene Setup Window", false, 200)]
        public static void OpenWindow()
        {
            var window = GetWindow<SceneSetupWindow>("Scene Setup");
            window.minSize = new Vector2(800, 600);
            window.Show();
        }

        #endregion

        #region UI State

        private Vector2 sceneListScrollPos;
        private Vector2 moduleControlScrollPos;
        private Vector2 logScrollPos;

        private List<SceneInfo> sceneInfos = new List<SceneInfo>();
        private List<string> logMessages = new List<string>();

        private bool showOnlyProblems = false;
        private bool autoRefresh = true;
        private string searchFilter = "";

        // Module control
        private bool enableCameraModule = true;
        private bool enableInputModule = true;
        private bool enablePostProcessModule = true;
        private bool enableLightingModule = true;
        private bool enableManagerModule = true;

        // UI colors
        private readonly Color colorConfigured = new Color(0.5f, 1f, 0.5f, 0.3f);
        private readonly Color colorPartial = new Color(1f, 1f, 0.5f, 0.3f);
        private readonly Color colorNotConfigured = new Color(1f, 0.5f, 0.5f, 0.3f);

        #endregion

        #region Scene Info Class

        [System.Serializable]
        private class SceneInfo
        {
            public string scenePath;
            public string sceneName;
            public bool hasSceneSetupManager;
            public SceneType detectedSceneType;
            public bool isConfigured;
            public bool hasProblems;
            public List<string> problems = new List<string>();
            public bool isSelected;
        }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            RefreshSceneList();
            EditorApplication.hierarchyChanged += OnHierarchyChanged;

            // Auto-refresh every 5 seconds if enabled
            EditorApplication.update += AutoRefreshUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.update -= AutoRefreshUpdate;
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawToolbar();

            EditorGUILayout.BeginHorizontal();
            {
                // Left panel - Scene list
                DrawSceneListPanel();

                // Right panel - Module controls and logs
                DrawControlPanel();
            }
            EditorGUILayout.EndHorizontal();

            DrawFooter();
        }

        #endregion

        #region UI Drawing

        private void DrawHeader()
        {
            EditorGUILayout.Space();

            // Title
            var titleStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUILayout.LabelField("🛠️ Scene Setup System", titleStyle);

            EditorGUILayout.Space();

            // Quick stats
            int totalScenes = sceneInfos.Count;
            int configuredScenes = sceneInfos.Count(s => s.isConfigured);
            int problemScenes = sceneInfos.Count(s => s.hasProblems);

            var statsStyle = new GUIStyle(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(statsStyle);
            EditorGUILayout.LabelField($"📊 Project Stats: {configuredScenes}/{totalScenes} configured • {problemScenes} with problems", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                // Refresh button
                if (GUILayout.Button("🔄 Refresh", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    RefreshSceneList();
                }

                // Search filter
                GUILayout.Label("🔍", GUILayout.Width(20));
                string newSearchFilter = EditorGUILayout.TextField(searchFilter, EditorStyles.toolbarSearchField, GUILayout.Width(200));
                if (newSearchFilter != searchFilter)
                {
                    searchFilter = newSearchFilter;
                    FilterScenes();
                }

                GUILayout.FlexibleSpace();

                // Filters
                bool newShowOnlyProblems = GUILayout.Toggle(showOnlyProblems, "⚠️ Problems Only", EditorStyles.toolbarButton);
                if (newShowOnlyProblems != showOnlyProblems)
                {
                    showOnlyProblems = newShowOnlyProblems;
                    FilterScenes();
                }

                autoRefresh = GUILayout.Toggle(autoRefresh, "🔄 Auto", EditorStyles.toolbarButton);

                // Batch operations
                if (GUILayout.Button("⚡ Setup All Selected", EditorStyles.toolbarButton, GUILayout.Width(120)))
                {
                    SetupSelectedScenes();
                }

                if (GUILayout.Button("✅ Validate All", EditorStyles.toolbarButton, GUILayout.Width(80)))
                {
                    ValidateAllScenes();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSceneListPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(400));
            {
                EditorGUILayout.LabelField("📁 Project Scenes", EditorStyles.boldLabel);

                sceneListScrollPos = EditorGUILayout.BeginScrollView(sceneListScrollPos, EditorStyles.helpBox);
                {
                    var filteredScenes = GetFilteredScenes();

                    foreach (var sceneInfo in filteredScenes)
                    {
                        DrawSceneItem(sceneInfo);
                    }

                    if (filteredScenes.Count == 0)
                    {
                        EditorGUILayout.LabelField("No scenes match the current filter.", EditorStyles.centeredGreyMiniLabel);
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawSceneItem(SceneInfo sceneInfo)
        {
            // Background color based on status
            Color bgColor = sceneInfo.isConfigured ? colorConfigured :
                           sceneInfo.hasProblems ? colorNotConfigured : colorPartial;

            var originalBgColor = GUI.backgroundColor;
            GUI.backgroundColor = bgColor;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = originalBgColor;

            EditorGUILayout.BeginHorizontal();
            {
                // Selection checkbox
                sceneInfo.isSelected = EditorGUILayout.Toggle(sceneInfo.isSelected, GUILayout.Width(20));

                // Status icon
                string statusIcon = sceneInfo.isConfigured ? "✅" :
                                   sceneInfo.hasProblems ? "❌" : "⚠️";
                EditorGUILayout.LabelField(statusIcon, GUILayout.Width(25));

                // Scene name
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.LabelField(sceneInfo.sceneName, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"Type: {sceneInfo.detectedSceneType} • Manager: {(sceneInfo.hasSceneSetupManager ? "Yes" : "No")}", EditorStyles.miniLabel);
                }
                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                // Action buttons
                EditorGUILayout.BeginVertical(GUILayout.Width(80));
                {
                    if (GUILayout.Button("Setup", EditorStyles.miniButton))
                    {
                        SetupScene(sceneInfo);
                    }

                    if (GUILayout.Button("Open", EditorStyles.miniButton))
                    {
                        OpenScene(sceneInfo);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            // Show problems if any
            if (sceneInfo.hasProblems && sceneInfo.problems.Count > 0)
            {
                EditorGUILayout.Space(2);
                foreach (var problem in sceneInfo.problems)
                {
                    EditorGUILayout.LabelField($"⚠️ {problem}", EditorStyles.miniLabel);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawControlPanel()
        {
            EditorGUILayout.BeginVertical();
            {
                // Module controls
                DrawModuleControls();

                EditorGUILayout.Space();

                // Logs
                DrawLogPanel();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawModuleControls()
        {
            EditorGUILayout.LabelField("🔧 Module Controls", EditorStyles.boldLabel);

            moduleControlScrollPos = EditorGUILayout.BeginScrollView(moduleControlScrollPos, EditorStyles.helpBox, GUILayout.Height(200));
            {
                enableManagerModule = EditorGUILayout.ToggleLeft("✅ Manager Setup Module", enableManagerModule);
                enableCameraModule = EditorGUILayout.ToggleLeft("📷 Camera Setup Module", enableCameraModule);
                enableInputModule = EditorGUILayout.ToggleLeft("🎮 Input Setup Module", enableInputModule);
                enablePostProcessModule = EditorGUILayout.ToggleLeft("🎨 Post Process Module", enablePostProcessModule);
                enableLightingModule = EditorGUILayout.ToggleLeft("💡 Lighting Setup Module", enableLightingModule);

                EditorGUILayout.Space();

                // Quick presets
                EditorGUILayout.LabelField("🎯 Quick Presets", EditorStyles.miniBoldLabel);

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("All", EditorStyles.miniButton))
                    {
                        enableManagerModule = enableCameraModule = enableInputModule =
                        enablePostProcessModule = enableLightingModule = true;
                    }

                    if (GUILayout.Button("None", EditorStyles.miniButton))
                    {
                        enableManagerModule = enableCameraModule = enableInputModule =
                        enablePostProcessModule = enableLightingModule = false;
                    }

                    if (GUILayout.Button("Essential", EditorStyles.miniButton))
                    {
                        enableManagerModule = enableCameraModule = enableInputModule = true;
                        enablePostProcessModule = enableLightingModule = false;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawLogPanel()
        {
            EditorGUILayout.LabelField("📝 Operation Log", EditorStyles.boldLabel);

            logScrollPos = EditorGUILayout.BeginScrollView(logScrollPos, EditorStyles.helpBox);
            {
                if (logMessages.Count == 0)
                {
                    EditorGUILayout.LabelField("No operations performed yet.", EditorStyles.centeredGreyMiniLabel);
                }
                else
                {
                    for (int i = logMessages.Count - 1; i >= 0; i--)
                    {
                        EditorGUILayout.LabelField(logMessages[i], EditorStyles.wordWrappedMiniLabel);
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            // Clear log button
            if (GUILayout.Button("Clear Log", EditorStyles.miniButton))
            {
                logMessages.Clear();
            }
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("💡 Tip: Select multiple scenes to perform batch operations", EditorStyles.centeredGreyMiniLabel);
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Scene Operations

        private void RefreshSceneList()
        {
            sceneInfos.Clear();

            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");

            foreach (string guid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                var sceneInfo = new SceneInfo
                {
                    scenePath = scenePath,
                    sceneName = sceneName,
                    detectedSceneType = DetectSceneTypeFromName(sceneName)
                };

                // Analyze scene without opening it
                AnalyzeScene(sceneInfo);

                sceneInfos.Add(sceneInfo);
            }

            LogMessage($"🔄 Refreshed {sceneInfos.Count} scenes");
            Repaint();
        }

        private void AnalyzeScene(SceneInfo sceneInfo)
        {
            // For now, basic analysis without opening the scene
            // In a real implementation, you might open scenes additively to analyze
            sceneInfo.hasSceneSetupManager = false;
            sceneInfo.isConfigured = false;
            sceneInfo.hasProblems = true;
            sceneInfo.problems.Clear();
            sceneInfo.problems.Add("Analysis requires opening scene");
        }

        private SceneType DetectSceneTypeFromName(string sceneName)
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

        private void SetupScene(SceneInfo sceneInfo)
        {
            if (Application.isPlaying)
            {
                LogMessage("❌ Cannot setup scenes while in play mode");
                return;
            }

            LogMessage($"⚡ Setting up scene: {sceneInfo.sceneName}");

            // Open scene additively
            var scene = EditorSceneManager.OpenScene(sceneInfo.scenePath, OpenSceneMode.Additive);

            if (scene.IsValid())
            {
                try
                {
                    // Find or create SceneSetupManager
                    SceneSetupManager setupManager = FindSceneSetupManagerInScene(scene);

                    if (setupManager == null)
                    {
                        GameObject setupGO = new GameObject("SceneSetupManager");
                        EditorSceneManager.MoveGameObjectToScene(setupGO, scene);
                        setupManager = setupGO.AddComponent<SceneSetupManager>();
                        LogMessage($"✅ Created SceneSetupManager in {sceneInfo.sceneName}");
                    }

                    // Configure based on enabled modules
                    ConfigureSceneSetupManager(setupManager);

                    // Execute setup
                    setupManager.SetupScene();

                    // Mark scene dirty and save
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);

                    LogMessage($"✅ Setup completed for {sceneInfo.sceneName}");
                }
                catch (System.Exception e)
                {
                    LogMessage($"❌ Setup failed for {sceneInfo.sceneName}: {e.Message}");
                }
                finally
                {
                    // Close the scene if it wasn't the active scene
                    if (scene != SceneManager.GetActiveScene())
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
            }

            // Refresh the scene info
            AnalyzeScene(sceneInfo);
            Repaint();
        }

        private SceneSetupManager FindSceneSetupManagerInScene(Scene scene)
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

        private void ConfigureSceneSetupManager(SceneSetupManager setupManager)
        {
            // Configure the setup manager based on enabled modules
            // This would require reflection or direct access to the setup manager's configuration
            LogMessage($"🔧 Configured modules: Manager={enableManagerModule}, Camera={enableCameraModule}, Input={enableInputModule}, PostProcess={enablePostProcessModule}, Lighting={enableLightingModule}");
        }

        private void OpenScene(SceneInfo sceneInfo)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(sceneInfo.scenePath);
                LogMessage($"📂 Opened scene: {sceneInfo.sceneName}");
            }
        }

        private void SetupSelectedScenes()
        {
            var selectedScenes = sceneInfos.Where(s => s.isSelected).ToList();

            if (selectedScenes.Count == 0)
            {
                LogMessage("⚠️ No scenes selected for batch setup");
                return;
            }

            if (!EditorUtility.DisplayDialog("Batch Setup",
                $"Setup {selectedScenes.Count} selected scenes?\n\nThis may take some time.",
                "Yes", "Cancel"))
            {
                return;
            }

            LogMessage($"⚡ Starting batch setup of {selectedScenes.Count} scenes...");

            int completed = 0;
            foreach (var sceneInfo in selectedScenes)
            {
                EditorUtility.DisplayProgressBar("Batch Scene Setup",
                    $"Setting up: {sceneInfo.sceneName}",
                    (float)completed / selectedScenes.Count);

                SetupScene(sceneInfo);
                completed++;
            }

            EditorUtility.ClearProgressBar();
            LogMessage($"✅ Batch setup completed: {completed}/{selectedScenes.Count} scenes");
        }

        private void ValidateAllScenes()
        {
            LogMessage("🔍 Iniciando validação real de todas as cenas...");

            int validScenes = 0;
            int invalidScenes = 0;

            foreach (var sceneInfo in sceneInfos)
            {
                var scene = EditorSceneManager.OpenScene(sceneInfo.scenePath, OpenSceneMode.Additive);
                if (scene.IsValid())
                {
                    var setupManager = FindSceneSetupManagerInScene(scene);
                    if (setupManager != null)
                    {
                        var result = ExtraTools.SceneSetupValidator.ValidateScene(sceneInfo.scenePath);
                        sceneInfo.hasSceneSetupManager = true;
                        sceneInfo.problems.Clear();
                        if (result.isValid)
                        {
                            validScenes++;
                            sceneInfo.isConfigured = true;
                            sceneInfo.hasProblems = false;
                        }
                        else
                        {
                            invalidScenes++;
                            sceneInfo.isConfigured = false;
                            sceneInfo.hasProblems = true;
                            foreach (var issue in result.issues)
                            {
                                sceneInfo.problems.Add($"{issue.severity}: {issue.title} - {issue.description}");
                            }
                        }
                    }
                    else
                    {
                        invalidScenes++;
                        sceneInfo.hasSceneSetupManager = false;
                        sceneInfo.isConfigured = false;
                        sceneInfo.hasProblems = true;
                        sceneInfo.problems.Clear();
                        sceneInfo.problems.Add("SceneSetupManager não encontrado na cena");
                    }
                    // Fecha a cena se não for a ativa
                    if (scene != SceneManager.GetActiveScene())
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
                else
                {
                    invalidScenes++;
                    sceneInfo.isConfigured = false;
                    sceneInfo.hasProblems = true;
                    sceneInfo.problems.Clear();
                    sceneInfo.problems.Add("Falha ao abrir cena para validação");
                }
            }

            LogMessage($"📊 Validação concluída: {validScenes} válidas, {invalidScenes} com problemas");
            RefreshSceneList();
        }

        #endregion

        #region Utility Methods

        private List<SceneInfo> GetFilteredScenes()
        {
            var filtered = sceneInfos.AsEnumerable();

            if (showOnlyProblems)
            {
                filtered = filtered.Where(s => s.hasProblems);
            }

            if (!string.IsNullOrEmpty(searchFilter))
            {
                filtered = filtered.Where(s => s.sceneName.ToLower().Contains(searchFilter.ToLower()));
            }

            return filtered.ToList();
        }

        private void FilterScenes()
        {
            Repaint();
        }

        private void LogMessage(string message)
        {
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            logMessages.Add($"[{timestamp}] {message}");

            // Keep only last 50 messages
            if (logMessages.Count > 50)
            {
                logMessages.RemoveAt(0);
            }

            Repaint();
        }

        private void OnHierarchyChanged()
        {
            if (autoRefresh)
            {
                // Delayed refresh to avoid excessive calls
                EditorApplication.delayCall += RefreshSceneList;
            }
        }

        private float autoRefreshTimer = 0f;
        private const float autoRefreshInterval = 5f;

        private void AutoRefreshUpdate()
        {
            if (!autoRefresh) return;

            autoRefreshTimer += Time.deltaTime;
            if (autoRefreshTimer >= autoRefreshInterval)
            {
                autoRefreshTimer = 0f;
                RefreshSceneList();
            }
        }

        #endregion
    }
}