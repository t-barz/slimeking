using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SlimeKing.Core;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Menu unificado de ferramentas extras para The Slime King
    /// Consolida todas as funcionalidades em um único lugar
    /// </summary>
    public class UnifiedExtraTools : EditorWindow
    {
        #region Debug & Logs
        [SerializeField] private static bool enableLogs = true;
        [SerializeField] private static bool enableDebug = false;

        private static void Log(string message)
        {
            if (enableLogs)
                Debug.Log($"[Extra Tools] {message}");
        }

        private static void LogWarning(string message)
        {
            if (enableLogs)
                Debug.LogWarning($"[Extra Tools] {message}");
        }

        private static void LogError(string message)
        {
            if (enableLogs)
                Debug.LogError($"[Extra Tools] {message}");
        }

        private static void DebugLog(string message)
        {
            if (enableDebug)
                Debug.Log($"[Extra Tools DEBUG] {message}");
        }
        #endregion

        #region Window Management
        [MenuItem("Extra Tools/🏠 Open Extra Tools Window")]
        public static void ShowWindow()
        {
            GetWindow<UnifiedExtraTools>("Extra Tools");
        }
        #endregion

        #region Menu Items - NPC Configuration
        [MenuItem("Extra Tools/NPC/🎭 NPC Quick Config")]
        public static void MenuNPCQuickConfig()
        {
            NPCQuickConfig.ShowWindow();
        }

        [MenuItem("Extra Tools/NPC/📊 NPC Batch Configurator")]
        public static void MenuNPCBatchConfig()
        {
            EditorWindow.GetWindow(System.Type.GetType("SlimeKing.Editor.NPCBatchConfigurator, Assembly-CSharp-Editor"));
        }
        #endregion

        #region Menu Items - Camera Setup
        [MenuItem("Extra Tools/Camera/📷 Add Camera Manager")]
        public static void MenuAddCameraManager()
        {
            var existing = Object.FindFirstObjectByType<CameraManager>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Camera Manager",
                    $"Camera Manager já existe: {existing.gameObject.name}", "OK");
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            var cameraManagerObj = new GameObject("Camera Manager");
            cameraManagerObj.AddComponent<CameraManager>();
            cameraManagerObj.transform.SetAsFirstSibling();
            Selection.activeGameObject = cameraManagerObj;

            Log("Camera Manager adicionado à cena");
            EditorUtility.DisplayDialog("Camera Manager", "✅ Camera Manager adicionado!", "OK");
        }

        [MenuItem("Extra Tools/Camera/✅ Add Scene Validator")]
        public static void MenuAddSceneValidator()
        {
            var existing = Object.FindFirstObjectByType<SceneSetupValidator>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Scene Validator",
                    $"Scene Validator já existe: {existing.gameObject.name}", "OK");
                Selection.activeGameObject = existing.gameObject;
                return;
            }

            var validatorObj = new GameObject("Scene Validator");
            validatorObj.AddComponent<SceneSetupValidator>();
            validatorObj.transform.SetAsFirstSibling();
            Selection.activeGameObject = validatorObj;

            Log("Scene Validator adicionado à cena");
            EditorUtility.DisplayDialog("Scene Validator", "✅ Scene Validator adicionado!", "OK");
        }

        [MenuItem("Extra Tools/Camera/🎬 Setup Complete Scene")]
        public static void MenuSetupCompleteScene()
        {
            bool addedCameraManager = false;
            bool addedValidator = false;

            var existingCameraManager = Object.FindFirstObjectByType<CameraManager>();
            if (existingCameraManager == null)
            {
                var cameraManagerObj = new GameObject("Camera Manager");
                cameraManagerObj.AddComponent<CameraManager>();
                cameraManagerObj.transform.SetAsFirstSibling();
                addedCameraManager = true;
            }

            var existingValidator = Object.FindFirstObjectByType<SceneSetupValidator>();
            if (existingValidator == null)
            {
                var validatorObj = new GameObject("Scene Validator");
                validatorObj.AddComponent<SceneSetupValidator>();
                validatorObj.transform.SetAsFirstSibling();
                addedValidator = true;
            }

            string message = "Setup da cena concluído!\n\n";
            message += addedCameraManager ? "✓ Camera Manager adicionado\n" : "✓ Camera Manager já existia\n";
            message += addedValidator ? "✓ Scene Validator adicionado\n" : "✓ Scene Validator já existia\n";

            Log(message.Replace("\n", " "));
            EditorUtility.DisplayDialog("Setup Completo", message, "OK");
        }
        #endregion

        #region Menu Items - Project Structure
        [MenuItem("Extra Tools/Project/📁 Create Folder Structure")]
        public static void MenuCreateFolderStructure()
        {
            CreateProjectFolderStructure();
        }

        [MenuItem("Extra Tools/Project/🔄 Reorganize Assets")]
        public static void MenuReorganizeAssets()
        {
            ReorganizeExistingAssets();
        }

        [MenuItem("Extra Tools/Project/✨ Complete Setup")]
        public static void MenuCompleteSetup()
        {
            CompleteProjectSetup();
        }
        #endregion

        #region Menu Items - Post Processing
        [MenuItem("Extra Tools/Post Processing/🌐 Setup Global Volume")]
        public static void MenuSetupGlobalVolume()
        {
            SetupGlobalVolumeInScene();
        }

        [MenuItem("Extra Tools/Post Processing/🌲 Setup Forest Volume")]
        public static void MenuSetupForestVolume()
        {
            SetupBiomeVolume("ForestBiome_Volume");
        }

        [MenuItem("Extra Tools/Post Processing/🏔️ Setup Cave Volume")]
        public static void MenuSetupCaveVolume()
        {
            SetupBiomeVolume("CaveBiome_Volume");
        }

        [MenuItem("Extra Tools/Post Processing/💎 Setup Crystal Volume")]
        public static void MenuSetupCrystalVolume()
        {
            SetupBiomeVolume("CrystalBiome_Volume");
        }

        [MenuItem("Extra Tools/Post Processing/⚡ Setup Gameplay Effects")]
        public static void MenuSetupGameplayEffects()
        {
            SetupGameplayVolumeEffects();
        }
        #endregion

        #region Menu Items - Debug
        [MenuItem("Extra Tools/Debug/🔊 Toggle Logs")]
        public static void MenuToggleLogs()
        {
            ToggleLogs();
        }

        [MenuItem("Extra Tools/Debug/📊 Export Scene Structure")]
        public static void MenuExportSceneStructure()
        {
            ExportSceneStructure();
        }

        [MenuItem("Extra Tools/Debug/⚙️ Export Project Settings")]
        public static void MenuExportProjectSettings()
        {
            SlimeKing.Core.EditorTools.ProjectSettingsExporterWindow.Open();
        }
        #endregion


        #region Window GUI
        private Vector2 scrollPosition;
        private int selectedTab = 0;
        private readonly string[] tabNames = { "NPC", "Camera", "Scene", "Project", "Post Processing", "Debug" };

        private void OnGUI()
        {
            DrawHeader();
            
            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            switch (selectedTab)
            {
                case 0: DrawNPCSection(); break;
                case 1: DrawCameraSection(); break;
                case 2: DrawSceneSection(); break;
                case 3: DrawProjectSection(); break;
                case 4: DrawPostProcessingSection(); break;
                case 5: DrawDebugSection(); break;
            }
            
            EditorGUILayout.EndScrollView();
            
            DrawFooter();
        }

        private void DrawHeader()
        {
            GUILayout.Label("🎮 Extra Tools - The Slime King", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Ferramentas unificadas de desenvolvimento", MessageType.Info);
            GUILayout.Space(10);
        }

        private void DrawNPCSection()
        {
            EditorGUILayout.LabelField("🎭 Configuração de NPCs", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("🎭 NPC Quick Config", GUILayout.Height(30)))
            {
                MenuNPCQuickConfig();
            }

            if (GUILayout.Button("📊 NPC Batch Configurator", GUILayout.Height(30)))
            {
                MenuNPCBatchConfig();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Configure NPCs individualmente ou em lote", MessageType.Info);
        }

        private void DrawCameraSection()
        {
            EditorGUILayout.LabelField("📷 Setup de Câmera", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("📷 Add Camera Manager", GUILayout.Height(30)))
            {
                MenuAddCameraManager();
            }

            if (GUILayout.Button("✅ Add Scene Validator", GUILayout.Height(30)))
            {
                MenuAddSceneValidator();
            }

            if (GUILayout.Button("🎬 Setup Complete Scene", GUILayout.Height(30)))
            {
                MenuSetupCompleteScene();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Configure câmera e validação de cena", MessageType.Info);
        }

        private void DrawSceneSection()
        {
            EditorGUILayout.LabelField("🎬 Scene Setup", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("🎬 Setup Scene for Transitions", GUILayout.Height(30)))
            {
                SlimeKing.Core.EditorTools.SceneSetupTool.SetupSceneForTransitions();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Configure cena com componentes essenciais para transições", MessageType.Info);
        }

        private void DrawProjectSection()
        {
            EditorGUILayout.LabelField("📁 Estrutura do Projeto", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("📁 Create Folder Structure", GUILayout.Height(30)))
            {
                MenuCreateFolderStructure();
            }

            if (GUILayout.Button("🔄 Reorganize Assets", GUILayout.Height(30)))
            {
                MenuReorganizeAssets();
            }

            if (GUILayout.Button("✨ Complete Setup", GUILayout.Height(30)))
            {
                MenuCompleteSetup();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Organize a estrutura de pastas do projeto", MessageType.Info);
        }

        private void DrawPostProcessingSection()
        {
            EditorGUILayout.LabelField("🎨 Post Processing", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("🌐 Setup Global Volume", GUILayout.Height(30)))
            {
                MenuSetupGlobalVolume();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🌲 Floresta", GUILayout.Height(25)))
            {
                MenuSetupForestVolume();
            }
            if (GUILayout.Button("🏔️ Caverna", GUILayout.Height(25)))
            {
                MenuSetupCaveVolume();
            }
            if (GUILayout.Button("💎 Cristal", GUILayout.Height(25)))
            {
                MenuSetupCrystalVolume();
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("⚡ Setup Gameplay Effects", GUILayout.Height(30)))
            {
                MenuSetupGameplayEffects();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Configure volumes de post-processing", MessageType.Info);
        }

        private void DrawDebugSection()
        {
            EditorGUILayout.LabelField("🐛 Debug e Logs", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("🔊 Toggle Logs", GUILayout.Height(30)))
            {
                MenuToggleLogs();
            }

            if (GUILayout.Button("📊 Export Scene Structure", GUILayout.Height(30)))
            {
                MenuExportSceneStructure();
            }

            if (GUILayout.Button("⚙️ Export Project Settings", GUILayout.Height(30)))
            {
                MenuExportProjectSettings();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Ferramentas de debug e análise", MessageType.Info);
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("🎮 The Slime King - Unity 6.2+", MessageType.None);
        }
        #endregion


        #region Project Structure Implementation
        private static void CreateProjectFolderStructure()
        {
            Log("🚀 Iniciando criação da estrutura de pastas com emojis...");

            string[] folders = {
                "Assets/🎨 Art", "Assets/🎨 Art/Sprites", "Assets/🎨 Art/Materials",
                "Assets/🎨 Art/Animations", "Assets/🎨 Art/Animations/Controllers", "Assets/🎨 Art/Animations/Clips",
                "Assets/🔊 Audio", "Assets/🔊 Audio/Music", "Assets/🔊 Audio/SFX",
                "Assets/💻 Code", "Assets/💻 Code/Gameplay", "Assets/💻 Code/Systems", "Assets/💻 Code/Editor",
                "Assets/🎮 Game", "Assets/🎮 Game/Scenes", "Assets/🎮 Game/Prefabs", "Assets/🎮 Game/Data",
                "Assets/⚙️ Settings", "Assets/⚙️ Settings/PostProcessing",
                "Assets/📦 External", "Assets/📦 External/AssetStore", "Assets/📦 External/Plugins",
                "Assets/📦 External/Libraries", "Assets/📦 External/Tools"
            };

            int foldersCreated = 0;
            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    CreateFolder(folder);
                    string gitkeepPath = Path.Combine(folder, ".gitkeep");
                    File.WriteAllText(gitkeepPath, "# Estrutura Organizacional - The Slime King");
                    foldersCreated++;
                    Log($"📂 Pasta criada: {folder}");
                }
            }

            AssetDatabase.Refresh();

            if (foldersCreated > 0)
            {
                Log($"✅ Estrutura criada! {foldersCreated} pastas.");
                EditorUtility.DisplayDialog("Estrutura Criada", $"✅ {foldersCreated} pastas criadas com sucesso!", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Estrutura Existente", "ℹ️ A estrutura já existe.", "OK");
            }
        }

        private static void CreateFolder(string folderPath)
        {
            string[] pathParts = folderPath.Split('/');
            string currentPath = pathParts[0];

            for (int i = 1; i < pathParts.Length; i++)
            {
                string newPath = currentPath + "/" + pathParts[i];
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                }
                currentPath = newPath;
            }
        }

        private static void ReorganizeExistingAssets()
        {
            Log("🔄 Reorganizando assets...");
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            int movedAssets = 0;

            foreach (string assetPath in allAssets)
            {
                if (assetPath.StartsWith("Packages/") || assetPath.StartsWith("Library/") ||
                    assetPath.StartsWith("ProjectSettings/") || assetPath.StartsWith("UserSettings/"))
                    continue;

                if (IsInCorrectFolder(assetPath))
                    continue;

                string newPath = GetNewPathForAsset(assetPath);
                if (!string.IsNullOrEmpty(newPath) && newPath != assetPath)
                {
                    string result = AssetDatabase.MoveAsset(assetPath, newPath);
                    if (string.IsNullOrEmpty(result))
                        movedAssets++;
                }
            }

            AssetDatabase.Refresh();
            Log($"✅ {movedAssets} assets reorganizados.");
            EditorUtility.DisplayDialog("Reorganização", $"✅ {movedAssets} assets reorganizados!", "OK");
        }

        private static bool IsInCorrectFolder(string assetPath)
        {
            string[] correctFolders = {
                "Assets/🎨 Art/", "Assets/🔊 Audio/", "Assets/💻 Code/",
                "Assets/🎮 Game/", "Assets/📦 External/", "Assets/⚙️ Settings/"
            };
            return correctFolders.Any(folder => assetPath.StartsWith(folder));
        }

        private static string GetNewPathForAsset(string assetPath)
        {
            string fileName = Path.GetFileName(assetPath);
            string extension = Path.GetExtension(assetPath).ToLower();

            if (extension == ".unity") return "Assets/🎮 Game/Scenes/" + fileName;
            if (extension == ".cs")
            {
                if (assetPath.Contains("Editor") || fileName.Contains("Editor"))
                    return "Assets/💻 Code/Editor/" + fileName;
                if (fileName.Contains("Manager") || fileName.Contains("System"))
                    return "Assets/💻 Code/Systems/" + fileName;
                return "Assets/💻 Code/Gameplay/" + fileName;
            }
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                return "Assets/🎨 Art/Sprites/" + fileName;
            if (extension == ".wav" || extension == ".mp3" || extension == ".ogg")
                return fileName.ToLower().Contains("music") ? "Assets/🔊 Audio/Music/" + fileName : "Assets/🔊 Audio/SFX/" + fileName;
            if (extension == ".mat") return "Assets/🎨 Art/Materials/" + fileName;
            if (extension == ".prefab") return "Assets/🎮 Game/Prefabs/" + fileName;
            if (extension == ".anim") return "Assets/🎨 Art/Animations/Clips/" + fileName;
            if (extension == ".controller") return "Assets/🎨 Art/Animations/Controllers/" + fileName;
            if (extension == ".asset") return "Assets/🎮 Game/Data/" + fileName;

            return "";
        }

        private static void CompleteProjectSetup()
        {
            Log("✨ Setup completo do projeto...");
            CreateProjectFolderStructure();
            ReorganizeExistingAssets();
            EditorUtility.DisplayDialog("Setup Completo", "🎉 Projeto configurado com sucesso!", "OK");
        }
        #endregion


        #region Debug Implementation
        private static void ToggleLogs()
        {
            enableLogs = !enableLogs;
            Log($"Logs {(enableLogs ? "habilitados" : "desabilitados")}");
            EditorUtility.DisplayDialog("Logs", $"🔊 Logs {(enableLogs ? "habilitados" : "desabilitados")}!", "OK");
        }

        private static void ExportSceneStructure()
        {
            Log("📊 Exportando estrutura da cena...");
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                LogError("Nenhuma cena ativa!");
                return;
            }

            var fileName = $"SceneStructure_{scene.name}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var logsDir = Path.Combine(Application.dataPath, "..", "Logs");
            if (!Directory.Exists(logsDir))
                Directory.CreateDirectory(logsDir);

            var filePath = Path.Combine(logsDir, fileName);

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"ESTRUTURA DA CENA: {scene.name}");
                writer.WriteLine($"Exportado em: {System.DateTime.Now}");
                writer.WriteLine($"Total de GameObjects: {scene.rootCount}");
                writer.WriteLine();

                var rootObjects = scene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    writer.WriteLine($"[{i + 1}] {rootObjects[i].name}");
                    WriteGameObjectHierarchy(rootObjects[i].transform, writer, 1);
                }
            }

            Log($"✅ Estrutura exportada: {filePath}");
            EditorUtility.DisplayDialog("Exportação", $"📊 Estrutura exportada!\n{fileName}", "OK");
            EditorUtility.RevealInFinder(filePath);
        }

        private static void WriteGameObjectHierarchy(Transform transform, StreamWriter writer, int level)
        {
            string indent = new string(' ', level * 2);
            foreach (Transform child in transform)
            {
                writer.WriteLine($"{indent}└─ {child.name}");
                if (child.childCount > 0)
                    WriteGameObjectHierarchy(child, writer, level + 1);
            }
        }
        #endregion


        #region Post Processing Implementation
        private static void SetupGlobalVolumeInScene()
        {
            Log("🎨 Configurando Post Processing...");
            
            var existing = Object.FindFirstObjectByType<Volume>();
            if (existing != null && existing.gameObject.name == "Global Volume")
            {
                EditorUtility.DisplayDialog("Global Volume", "Global Volume já existe!", "OK");
                return;
            }

            var volumeObj = new GameObject("Global Volume");
            var volume = volumeObj.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 0;

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            string profilePath = "Assets/⚙️ Settings/PostProcessing/GlobalVolume_Profile.asset";
            
            if (!AssetDatabase.IsValidFolder("Assets/⚙️ Settings"))
                AssetDatabase.CreateFolder("Assets", "⚙️ Settings");
            if (!AssetDatabase.IsValidFolder("Assets/⚙️ Settings/PostProcessing"))
                AssetDatabase.CreateFolder("Assets/⚙️ Settings", "PostProcessing");

            AssetDatabase.CreateAsset(profile, profilePath);
            volume.profile = profile;

            Log("✅ Global Volume criado!");
            EditorUtility.DisplayDialog("Post Processing", "✅ Global Volume configurado!", "OK");
        }

        private static void SetupBiomeVolume(string biomeName)
        {
            Log($"🎨 Configurando volume para {biomeName}...");
            
            var volumeObj = new GameObject(biomeName);
            var volume = volumeObj.AddComponent<Volume>();
            volume.isGlobal = false;
            volume.priority = 1;

            var collider = volumeObj.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(10, 10, 10);

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            string profilePath = $"Assets/⚙️ Settings/PostProcessing/{biomeName}_Profile.asset";
            
            if (!AssetDatabase.IsValidFolder("Assets/⚙️ Settings/PostProcessing"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/⚙️ Settings"))
                    AssetDatabase.CreateFolder("Assets", "⚙️ Settings");
                AssetDatabase.CreateFolder("Assets/⚙️ Settings", "PostProcessing");
            }

            AssetDatabase.CreateAsset(profile, profilePath);
            volume.profile = profile;

            Log($"✅ {biomeName} criado!");
            EditorUtility.DisplayDialog("Biome Volume", $"✅ {biomeName} configurado!", "OK");
        }

        private static void SetupGameplayVolumeEffects()
        {
            Log("⚡ Configurando efeitos de gameplay...");
            
            var effectsObj = new GameObject("Gameplay Effects");
            var volume = effectsObj.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 10;
            volume.weight = 0;

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            string profilePath = "Assets/⚙️ Settings/PostProcessing/GameplayEffects_Profile.asset";
            
            if (!AssetDatabase.IsValidFolder("Assets/⚙️ Settings/PostProcessing"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/⚙️ Settings"))
                    AssetDatabase.CreateFolder("Assets", "⚙️ Settings");
                AssetDatabase.CreateFolder("Assets/⚙️ Settings", "PostProcessing");
            }

            AssetDatabase.CreateAsset(profile, profilePath);
            volume.profile = profile;

            Log("✅ Gameplay Effects criado!");
            EditorUtility.DisplayDialog("Gameplay Effects", "✅ Efeitos de gameplay configurados!", "OK");
        }
        #endregion
    }
}
