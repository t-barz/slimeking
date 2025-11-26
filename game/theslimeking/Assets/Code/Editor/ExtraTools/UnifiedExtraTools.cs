using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SlimeKing.Core;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Menu unificado de ferramentas extras para desenvolvimento Unity
    /// Consolida todas as funcionalidades em um único lugar
    /// 
    /// ⚠️ POLÍTICA DE MENUS - OBRIGATÓRIA:
    /// TODOS os menus de editor DEVEM estar sob "Extra Tools/"
    /// NUNCA criar menus separados como "SlimeKing/", "The Slime King/", etc.
    /// 
    /// Estrutura obrigatória:
    /// - "Extra Tools/Tests/..." - Para todos os testes
    /// - "Extra Tools/Setup/..." - Para ferramentas de configuração  
    /// - "Extra Tools/NPC/..." - Para ferramentas de NPC
    /// - "Extra Tools/Scene Tools/..." - Para ferramentas de cena
    /// - "Extra Tools/Quest System/..." - Para sistema de quests
    /// - "Assets/Create/Extra Tools/..." - Para criação de assets
    /// 
    /// Ver README.md para detalhes completos da política.
    /// </summary>
    public class UnifiedExtraTools : EditorWindow
    {
        #region Debug & Logs
        [SerializeField] private static bool enableLogs = true;
        [SerializeField] private static bool enableDebug = false;

        private static void Log(string message)
        {
            if (enableLogs)
                UnityEngine.Debug.Log($"[Extra Tools] {message}");
        }

        private static void LogWarning(string message)
        {
            if (enableLogs)
                UnityEngine.Debug.LogWarning($"[Extra Tools] {message}");
        }

        private static void LogError(string message)
        {
            if (enableLogs)
                UnityEngine.Debug.LogError($"[Extra Tools] {message}");
        }

        private static void DebugLog(string message)
        {
            if (enableDebug)
                UnityEngine.Debug.Log($"[Extra Tools DEBUG] {message}");
        }
        #endregion

        #region Window Management
        [MenuItem("Extra Tools/🏠 Open Extra Tools Window")]
        public static void ShowWindow()
        {
            GetWindow<UnifiedExtraTools>("Extra Tools");
        }
        #endregion

        #region Menu Items - Setup
        [MenuItem("Extra Tools/Setup/🎭 Setup as NPC")]
        public static void MenuSetupAsNPC()
        {
            if (Selection.activeGameObject == null)
            {
                UnityEngine.Debug.LogWarning("[UnifiedExtraTools] Nenhum GameObject selecionado.");
                return;
            }

            ExtraTools.Editor.NPCSetupTool.SetupAsNPC();
        }

        [MenuItem("Extra Tools/Setup/🎭 Setup as NPC", true)]
        public static bool ValidateMenuSetupAsNPC()
        {
            return Selection.activeGameObject != null;
        }
        #endregion

        #region Menu Items - Camera Setup
        [MenuItem("Extra Tools/Camera/📷 Add Camera Manager")]
        public static void MenuAddCameraManager()
        {
            var existing = UnityEngine.Object.FindFirstObjectByType<CameraManager>();
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
            var existing = UnityEngine.Object.FindFirstObjectByType<SceneSetupValidator>();
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

            var existingCameraManager = UnityEngine.Object.FindFirstObjectByType<CameraManager>();
            if (existingCameraManager == null)
            {
                var cameraManagerObj = new GameObject("Camera Manager");
                cameraManagerObj.AddComponent<CameraManager>();
                cameraManagerObj.transform.SetAsFirstSibling();
                addedCameraManager = true;
            }

            var existingValidator = UnityEngine.Object.FindFirstObjectByType<SceneSetupValidator>();
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

        #region Menu Items - Scene Tools
        [MenuItem("Extra Tools/Scene Tools/🎬 Setup Scene for Transitions")]
        public static void MenuSetupSceneForTransitions()
        {
            ExtraTools.Editor.SceneSetupTool.SetupSceneForTransitions();
        }

        [MenuItem("Extra Tools/Scene Tools/🖌️ GameObject Brush")]
        public static void MenuOpenGameObjectBrush()
        {
            GameObjectBrushTool.ShowWindow();
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

        [MenuItem("Extra Tools/Debug/🎭 Export Animator Configuration")]
        public static void MenuExportAnimatorConfigurationDebug()
        {
            ExportAnimatorConfiguration();
        }

        [MenuItem("Extra Tools/Debug/🎭 Export Animator Configuration", true)]
        public static bool ValidateExportAnimatorConfigurationDebug()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Animator>() != null;
        }

        [MenuItem("Extra Tools/Debug/⚙️ Export Project Settings")]
        public static void MenuExportProjectSettings()
        {
            ExtraTools.Core.ProjectSettingsExporterWindow.Open();
        }

        [MenuItem("GameObject/Extra Tools/📊 Export GameObject Structure", false, 10)]
        public static void MenuExportGameObjectStructure()
        {
            ExportGameObjectStructure();
        }

        [MenuItem("GameObject/Extra Tools/📊 Export GameObject Structure", true)]
        public static bool ValidateExportGameObjectStructure()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Extra Tools/🎭 Export Animator Configuration", false, 11)]
        public static void MenuExportAnimatorConfiguration()
        {
            ExportAnimatorConfiguration();
        }

        [MenuItem("GameObject/Extra Tools/🎭 Export Animator Configuration", true)]
        public static bool ValidateExportAnimatorConfiguration()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Animator>() != null;
        }
        #endregion

        #region Menu Items - Quest System
        [MenuItem("Extra Tools/Quest System/Authoring/🎯 Create Collect Quest")]
        public static void MenuCreateCollectQuest()
        {
            ExtraTools.QuestSystem.QuestCreationTool.CreateCollectQuest();
        }

        [MenuItem("Extra Tools/Quest System/Authoring/📁 Create Folder Structure")]
        public static void MenuCreateQuestFolderStructure()
        {
            ExtraTools.QuestSystem.QuestCreationTool.CreateQuestFolderStructure();
        }

        [MenuItem("Extra Tools/Quest System/Authoring/🎨 Generate UI Sprites")]
        public static void MenuGenerateQuestSprites()
        {
            ExtraTools.QuestSystem.QuestSpriteGenerator.GenerateQuestSprites();
        }
        #endregion


        #region Window GUI
        private Vector2 scrollPosition;
        private int selectedTab = 0;
        private readonly string[] tabNames = { "Camera", "Scene", "Project", "Post Processing", "Quest System", "Debug" };

        private void OnGUI()
        {
            DrawHeader();

            selectedTab = GUILayout.Toolbar(selectedTab, tabNames);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            switch (selectedTab)
            {
                case 0: DrawCameraSection(); break;
                case 1: DrawSceneSection(); break;
                case 2: DrawProjectSection(); break;
                case 3: DrawPostProcessingSection(); break;
                case 4: DrawQuestSystemSection(); break;
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
                ExtraTools.Editor.SceneSetupTool.SetupSceneForTransitions();
            }

            if (GUILayout.Button("🖌️ GameObject Brush", GUILayout.Height(30)))
            {
                MenuOpenGameObjectBrush();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Configure cena com componentes essenciais para transições e use o brush para posicionar objetos", MessageType.Info);
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
            var auxTempDir = Path.Combine(Application.dataPath, "AuxTemp");
            if (!Directory.Exists(auxTempDir))
                Directory.CreateDirectory(auxTempDir);

            var filePath = Path.Combine(auxTempDir, fileName);

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

        private static void ExportAnimatorConfiguration()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                LogError("Nenhum GameObject selecionado.");
                EditorUtility.DisplayDialog("Erro", "Selecione um GameObject para exportar a configuração do Animator.", "OK");
                return;
            }

            Animator animator = selectedObject.GetComponent<Animator>();
            if (animator == null)
            {
                LogError("O GameObject selecionado não possui um componente Animator.");
                EditorUtility.DisplayDialog("Erro", "O GameObject selecionado não possui um componente Animator.", "OK");
                return;
            }

            Log($"🎭 Exportando configuração completa do Animator de '{selectedObject.name}'...");

            string fileName = $"AnimatorConfig_{selectedObject.name}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string auxTempDir = Path.Combine(Application.dataPath, "AuxTemp");
            if (!Directory.Exists(auxTempDir))
                Directory.CreateDirectory(auxTempDir);

            string filePath = Path.Combine(auxTempDir, fileName);

            using (var writer = new StreamWriter(filePath))
            {
                WriteAnimatorConfigurationReport(animator, selectedObject, writer);
            }

            Log($"✅ Configuração do Animator exportada: {filePath}");
            EditorUtility.DisplayDialog("Exportação Completa",
                $"🎭 Configuração do Animator exportada!\n{fileName}", "OK");
            EditorUtility.RevealInFinder(filePath);
        }

        private static void ExportGameObjectStructure()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                LogError("Nenhum GameObject selecionado.");
                EditorUtility.DisplayDialog("Erro", "Selecione um GameObject para exportar sua estrutura.", "OK");
                return;
            }

            Log($"📊 Exportando estrutura detalhada de '{selectedObject.name}'...");

            string fileName = $"GameObject_{selectedObject.name}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string auxTempDir = Path.Combine(Application.dataPath, "AuxTemp");
            if (!Directory.Exists(auxTempDir))
                Directory.CreateDirectory(auxTempDir);

            string filePath = Path.Combine(auxTempDir, fileName);

            using (var writer = new StreamWriter(filePath))
            {
                // Cabeçalho
                writer.WriteLine("════════════════════════════════════════════════════════");
                writer.WriteLine($"  ESTRUTURA DETALHADA DO GAMEOBJECT: {selectedObject.name}");
                writer.WriteLine("════════════════════════════════════════════════════════");
                writer.WriteLine($"Caminho completo: {GetGameObjectPath(selectedObject)}");
                writer.WriteLine($"Exportado em: {System.DateTime.Now}");
                writer.WriteLine($"Unity Version: {Application.unityVersion}");
                writer.WriteLine();

                // Informações básicas do GameObject
                WriteBasicObjectInfo(selectedObject, writer);
                writer.WriteLine();

                // Transform detalhado
                WriteTransformDetails(selectedObject.transform, writer);
                writer.WriteLine();

                // Componentes detalhados
                WriteDetailedComponents(selectedObject, writer);
                writer.WriteLine();

                // Layer e Tag
                WriteLayerAndTagInfo(selectedObject, writer);
                writer.WriteLine();

                // Hierarquia de filhos
                WriteChildrenHierarchy(selectedObject, writer);
            }

            Log($"✅ Estrutura detalhada do GameObject '{selectedObject.name}' exportada: {filePath}");
            EditorUtility.DisplayDialog("Exportação",
                $"📊 Estrutura detalhada exportada!\n{fileName}", "OK");
            EditorUtility.RevealInFinder(filePath);
        }

        private static string GetGameObjectPath(GameObject obj)
        {
            string path = obj.name;
            Transform parent = obj.transform.parent;

            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }

        private static int GetChildCount(Transform transform)
        {
            int count = transform.childCount;
            for (int i = 0; i < transform.childCount; i++)
            {
                count += GetChildCount(transform.GetChild(i));
            }
            return count;
        }

        #region Detailed Export Functions

        private static void WriteBasicObjectInfo(GameObject obj, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ INFORMAÇÕES BÁSICAS");
            writer.WriteLine("└─────────────────────────────────────────────────────────");
            writer.WriteLine($"Nome: {obj.name}");
            writer.WriteLine($"Ativo na Hierarquia: {(obj.activeInHierarchy ? "Sim" : "Não")}");
            writer.WriteLine($"Ativo Localmente: {(obj.activeSelf ? "Sim" : "Não")}");
            writer.WriteLine($"Static: {(obj.isStatic ? "Sim" : "Não")}");
            writer.WriteLine($"Total de Componentes: {obj.GetComponents<Component>().Length}");
            writer.WriteLine($"Total de Filhos: {GetChildCount(obj.transform)}");
            writer.WriteLine($"Total de Filhos Diretos: {obj.transform.childCount}");

            // Informações da cena
            if (obj.scene.IsValid())
            {
                writer.WriteLine($"Cena: {obj.scene.name}");
                writer.WriteLine($"Índice da Cena: {obj.scene.buildIndex}");
            }
        }

        private static void WriteTransformDetails(Transform transform, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ TRANSFORM");
            writer.WriteLine("└─────────────────────────────────────────────────────────");
            writer.WriteLine($"Posição Local: {transform.localPosition}");
            writer.WriteLine($"Posição Mundial: {transform.position}");
            writer.WriteLine($"Rotação Local: {transform.localRotation.eulerAngles}");
            writer.WriteLine($"Rotação Mundial: {transform.rotation.eulerAngles}");
            writer.WriteLine($"Escala Local: {transform.localScale}");
            writer.WriteLine($"Escala com Lossy: {transform.lossyScale}");

            if (transform.parent != null)
            {
                writer.WriteLine($"Parent: {transform.parent.name}");
                writer.WriteLine($"Sibling Index: {transform.GetSiblingIndex()}");
            }
            else
            {
                writer.WriteLine("Parent: (Root Object)");
            }
        }

        private static void WriteDetailedComponents(GameObject obj, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ COMPONENTES DETALHADOS");
            writer.WriteLine("└─────────────────────────────────────────────────────────");

            Component[] components = obj.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null)
                {
                    writer.WriteLine($"{i + 1}. [MISSING SCRIPT]");
                    continue;
                }

                writer.WriteLine($"{i + 1}. {component.GetType().Name}");
                writer.WriteLine($"    ├─ Namespace: {component.GetType().Namespace ?? "Global"}");
                writer.WriteLine($"    ├─ Assembly: {component.GetType().Assembly.GetName().Name}");
                writer.WriteLine($"    ├─ Habilitado: {GetComponentEnabledState(component)}");

                // Detalhes específicos por tipo de componente
                WriteComponentSpecificDetails(component, writer);

                if (i < components.Length - 1)
                    writer.WriteLine("    │");
            }
        }

        private static void WriteLayerAndTagInfo(GameObject obj, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ LAYER & TAG");
            writer.WriteLine("└─────────────────────────────────────────────────────────");
            writer.WriteLine($"Tag: {obj.tag}");
            writer.WriteLine($"Layer: {obj.layer} ({LayerMask.LayerToName(obj.layer)})");

            // Informações sobre colisões
            int layerMask = 1 << obj.layer;
            writer.WriteLine($"Layer Mask (bit): {layerMask}");

            // Lista outras camadas que podem colidir com esta
            List<string> collidingLayers = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                if (!Physics2D.GetIgnoreLayerCollision(obj.layer, i))
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(layerName))
                    {
                        collidingLayers.Add($"{i} ({layerName})");
                    }
                }
            }

            if (collidingLayers.Count > 0)
            {
                writer.WriteLine($"Pode colidir com: {string.Join(", ", collidingLayers)}");
            }
        }

        private static void WriteChildrenHierarchy(GameObject obj, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ HIERARQUIA DE FILHOS");
            writer.WriteLine("└─────────────────────────────────────────────────────────");

            if (obj.transform.childCount > 0)
            {
                WriteDetailedGameObjectHierarchy(obj.transform, writer, 0);
            }
            else
            {
                writer.WriteLine("(Nenhum filho)");
            }
        }

        private static void WriteDetailedGameObjectHierarchy(Transform transform, StreamWriter writer, int level)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                string indent = new string(' ', level * 2);
                string prefix = i == transform.childCount - 1 ? "└─" : "├─";

                // Informações básicas do filho
                writer.WriteLine($"{indent}{prefix} {child.name} " +
                    $"[{(child.gameObject.activeInHierarchy ? "Ativo" : "Inativo")}] " +
                    $"({child.GetComponents<Component>().Length - 1} componentes)");

                // Detalhes do Transform do filho
                string childIndent = indent + (i == transform.childCount - 1 ? "   " : "│  ");
                writer.WriteLine($"{childIndent}├─ Posição: {child.localPosition}");
                writer.WriteLine($"{childIndent}├─ Rotação: {child.localRotation.eulerAngles}");
                writer.WriteLine($"{childIndent}├─ Escala: {child.localScale}");
                writer.WriteLine($"{childIndent}├─ Tag: {child.tag}");
                writer.WriteLine($"{childIndent}├─ Layer: {child.gameObject.layer} ({LayerMask.LayerToName(child.gameObject.layer)})");

                // Lista componentes principais do filho
                Component[] childComponents = child.GetComponents<Component>();
                List<string> componentNames = new List<string>();
                foreach (var comp in childComponents)
                {
                    if (comp != null && !(comp is Transform))
                    {
                        componentNames.Add(comp.GetType().Name);
                    }
                }

                if (componentNames.Count > 0)
                {
                    writer.WriteLine($"{childIndent}└─ Componentes: {string.Join(", ", componentNames)}");
                }
                else
                {
                    writer.WriteLine($"{childIndent}└─ Componentes: (Apenas Transform)");
                }

                // Recursivamente processa filhos dos filhos
                if (child.childCount > 0)
                {
                    writer.WriteLine($"{childIndent}");
                    WriteDetailedGameObjectHierarchy(child, writer, level + 1);
                }

                // Adiciona espaço entre irmãos para legibilidade
                if (i < transform.childCount - 1)
                {
                    writer.WriteLine($"{childIndent}");
                }
            }
        }
        private static string GetComponentEnabledState(Component component)
        {
            // Verifica se o componente tem propriedade "enabled"
            var enabledProperty = component.GetType().GetProperty("enabled");
            if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
            {
                bool isEnabled = (bool)enabledProperty.GetValue(component);
                return isEnabled ? "Sim" : "Não";
            }
            return "N/A";
        }

        private static void WriteComponentSpecificDetails(Component component, StreamWriter writer)
        {
            switch (component)
            {
                case Renderer renderer:
                    writer.WriteLine($"    ├─ Material: {(renderer.sharedMaterial ? renderer.sharedMaterial.name : "None")}");
                    writer.WriteLine($"    ├─ Sorting Layer: {renderer.sortingLayerName}");
                    writer.WriteLine($"    ├─ Sorting Order: {renderer.sortingOrder}");
                    break;

                case Collider2D collider2D:
                    writer.WriteLine($"    ├─ Is Trigger: {collider2D.isTrigger}");
                    writer.WriteLine($"    ├─ Material: {(collider2D.sharedMaterial ? collider2D.sharedMaterial.name : "None")}");
                    writer.WriteLine($"    ├─ Bounds: {collider2D.bounds}");
                    break;

                case Rigidbody2D rb2D:
                    writer.WriteLine($"    ├─ Body Type: {rb2D.bodyType}");
                    writer.WriteLine($"    ├─ Mass: {rb2D.mass}");
                    writer.WriteLine($"    ├─ Gravity Scale: {rb2D.gravityScale}");
                    writer.WriteLine($"    ├─ Freeze Position: {rb2D.constraints}");
                    break;

                case Animator animator:
                    WriteAnimatorDetails(animator, writer);
                    break;

                case Canvas canvas:
                    writer.WriteLine($"    ├─ Render Mode: {canvas.renderMode}");
                    writer.WriteLine($"    ├─ Sort Order: {canvas.sortingOrder}");
                    writer.WriteLine($"    ├─ World Camera: {(canvas.worldCamera ? canvas.worldCamera.name : "None")}");
                    break;

                case Camera camera:
                    writer.WriteLine($"    ├─ Projection: {camera.orthographic}");
                    writer.WriteLine($"    ├─ Size/FOV: {(camera.orthographic ? camera.orthographicSize.ToString() : camera.fieldOfView.ToString())}");
                    writer.WriteLine($"    ├─ Depth: {camera.depth}");
                    writer.WriteLine($"    ├─ Culling Mask: {camera.cullingMask}");
                    break;

                case Light light:
                    writer.WriteLine($"    ├─ Type: {light.type}");
                    writer.WriteLine($"    ├─ Color: {light.color}");
                    writer.WriteLine($"    ├─ Intensity: {light.intensity}");
                    writer.WriteLine($"    ├─ Range: {light.range}");
                    break;

                case AudioSource audioSource:
                    writer.WriteLine($"    ├─ Clip: {(audioSource.clip ? audioSource.clip.name : "None")}");
                    writer.WriteLine($"    ├─ Volume: {audioSource.volume}");
                    writer.WriteLine($"    ├─ Pitch: {audioSource.pitch}");
                    writer.WriteLine($"    ├─ Loop: {audioSource.loop}");
                    break;
            }
        }

        private static void WriteAnimatorDetails(Animator animator, StreamWriter writer)
        {
            writer.WriteLine($"    ├─ Controller: {(animator.runtimeAnimatorController ? animator.runtimeAnimatorController.name : "None")}");
            writer.WriteLine($"    ├─ Culling Mode: {animator.cullingMode}");
            writer.WriteLine($"    ├─ Update Mode: {animator.updateMode}");
            writer.WriteLine($"    ├─ Apply Root Motion: {animator.applyRootMotion}");
            writer.WriteLine($"    ├─ Animate Physics: {animator.animatePhysics}");

            if (animator.runtimeAnimatorController != null)
            {
                writer.WriteLine($"    ├─ Layer Count: {animator.layerCount}");
                writer.WriteLine($"    ├─ Parameter Count: {animator.parameterCount}");

                // Informações do estado atual
                if (animator.layerCount > 0)
                {
                    try
                    {
                        var currentState = animator.GetCurrentAnimatorStateInfo(0);
                        writer.WriteLine($"    ├─ Current State: {GetStateName(animator, 0, currentState)}");
                        writer.WriteLine($"    ├─ Current Time: {currentState.normalizedTime:F2}");
                        writer.WriteLine($"    ├─ State Length: {currentState.length:F2}s");
                        writer.WriteLine($"    ├─ In Transition: {animator.IsInTransition(0)}");

                        if (animator.IsInTransition(0))
                        {
                            try
                            {
                                var transitionInfo = animator.GetAnimatorTransitionInfo(0);
                                writer.WriteLine($"    ├─ Transition Progress: {transitionInfo.normalizedTime:F2}");
                            }
                            catch (System.Exception ex)
                            {
                                writer.WriteLine($"    ├─ Transition Progress: (Error - {ex.Message})");
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        writer.WriteLine($"    ├─ Current State: (Error - {ex.Message})");
                    }
                }

                // Lista parâmetros
                writer.WriteLine($"    ├─ Parameters:");
                try
                {
                    if (animator.parameterCount > 0)
                    {
                        for (int i = 0; i < animator.parameterCount; i++)
                        {
                            try
                            {
                                var param = animator.GetParameter(i);
                                if (param != null)
                                {
                                    string value = GetParameterValue(animator, param);
                                    string paramType = param.type.ToString();
                                    writer.WriteLine($"    │   ├─ {param.name} ({paramType}): {value}");
                                }
                                else
                                {
                                    writer.WriteLine($"    │   ├─ Parameter [{i}]: (Null)");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                writer.WriteLine($"    │   ├─ Parameter [{i}]: (Error - {ex.Message})");
                            }
                        }
                    }
                    else
                    {
                        writer.WriteLine($"    │   ├─ No parameters found");
                    }
                }
                catch (System.Exception ex)
                {
                    writer.WriteLine($"    │   ├─ Error reading parameters: {ex.Message}");
                }

                // Lista todos os estados da máquina de estados
                writer.WriteLine($"    ├─ All States:");
                WriteAllAnimatorStates(animator, writer);

                // Lista layers se houver mais de 1
                if (animator.layerCount > 1)
                {
                    writer.WriteLine($"    ├─ Layers:");
                    try
                    {
                        for (int i = 0; i < animator.layerCount; i++)
                        {
                            try
                            {
                                string layerName = animator.GetLayerName(i);
                                float layerWeight = animator.GetLayerWeight(i);
                                var layerState = animator.GetCurrentAnimatorStateInfo(i);
                                string layerStateName = GetStateName(animator, i, layerState);
                                writer.WriteLine($"    │   ├─ [{i}] {layerName} (Weight: {layerWeight:F2}) - State: {layerStateName}");

                                // Lista estados específicos desta camada
                                if (animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController animController)
                                {
                                    try
                                    {
                                        if (i < animController.layers.Length)
                                        {
                                            var layer = animController.layers[i];
                                            writer.WriteLine($"    │   │   ├─ States in Layer:");
                                            WriteLayerStates(layer.stateMachine, writer, "    │   │   │   ");
                                        }
                                        else
                                        {
                                            writer.WriteLine($"    │   │   ├─ Layer index out of range");
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        writer.WriteLine($"    │   │   ├─ Error reading layer states: {ex.Message}");
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                writer.WriteLine($"    │   ├─ Layer [{i}]: (Error - {ex.Message})");
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        writer.WriteLine($"    │   ├─ Error reading layers: {ex.Message}");
                    }
                }
            }
        }

        private static string GetStateName(Animator animator, int layerIndex, AnimatorStateInfo stateInfo)
        {
            // Verifica se os índices são válidos
            if (animator == null || layerIndex < 0 || layerIndex >= animator.layerCount)
            {
                return $"Invalid_Layer_{layerIndex}";
            }

            // Tenta obter o nome do estado através do hash
            var controller = animator.runtimeAnimatorController;
            if (controller != null && controller is UnityEditor.Animations.AnimatorController animController)
            {
                try
                {
                    // Verifica se o índice da camada é válido
                    if (layerIndex >= 0 && layerIndex < animController.layers.Length)
                    {
                        var layer = animController.layers[layerIndex];
                        if (layer.stateMachine != null && layer.stateMachine.states != null)
                        {
                            foreach (var state in layer.stateMachine.states)
                            {
                                if (state.state != null && state.state.nameHash == stateInfo.shortNameHash)
                                {
                                    return state.state.name;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    return $"Error_Reading_State_{stateInfo.shortNameHash}_{ex.Message.Substring(0, Math.Min(20, ex.Message.Length))}";
                }
            }
            return $"State_{stateInfo.shortNameHash}";
        }

        private static string GetParameterValue(Animator animator, AnimatorControllerParameter param)
        {
            try
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        return animator.GetBool(param.name).ToString();
                    case AnimatorControllerParameterType.Float:
                        return animator.GetFloat(param.name).ToString("F2");
                    case AnimatorControllerParameterType.Int:
                        return animator.GetInteger(param.name).ToString();
                    case AnimatorControllerParameterType.Trigger:
                        return animator.GetBool(param.name) ? "Triggered" : "Not Triggered";
                    default:
                        return "Unknown";
                }
            }
            catch
            {
                return "Error";
            }
        }

        private static void WriteAllAnimatorStates(Animator animator, StreamWriter writer)
        {
            if (animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController animController)
            {
                try
                {
                    for (int layerIndex = 0; layerIndex < animController.layers.Length; layerIndex++)
                    {
                        var layer = animController.layers[layerIndex];
                        writer.WriteLine($"    │   ├─ Layer [{layerIndex}] {layer.name}:");
                        WriteLayerStates(layer.stateMachine, writer, "    │   │   ");
                    }
                }
                catch (System.Exception ex)
                {
                    writer.WriteLine($"    │   ├─ Error reading states: {ex.Message}");
                }
            }
            else
            {
                writer.WriteLine($"    │   ├─ No AnimatorController found or invalid type");
            }
        }

        private static void WriteLayerStates(UnityEditor.Animations.AnimatorStateMachine stateMachine, StreamWriter writer, string indent)
        {
            if (stateMachine == null)
            {
                writer.WriteLine($"{indent}├─ (Null StateMachine)");
                return;
            }

            // Lista todos os estados
            foreach (var stateInfo in stateMachine.states)
            {
                var state = stateInfo.state;
                if (state == null) continue;

                string stateDetails = GetStateDetails(state);
                writer.WriteLine($"{indent}├─ {state.name}{stateDetails}");

                // Lista transições do estado
                if (state.transitions.Length > 0)
                {
                    writer.WriteLine($"{indent}│   ├─ Transitions ({state.transitions.Length}):");
                    foreach (var transition in state.transitions)
                    {
                        if (transition.destinationState != null)
                        {
                            writer.WriteLine($"{indent}│   │   ├─ → {transition.destinationState.name}");
                        }
                        else if (transition.isExit)
                        {
                            writer.WriteLine($"{indent}│   │   ├─ → (Exit)");
                        }
                    }
                }
            }

            // Lista sub-máquinas de estado recursivamente
            foreach (var subStateMachine in stateMachine.stateMachines)
            {
                writer.WriteLine($"{indent}├─ SubStateMachine: {subStateMachine.stateMachine.name}");
                WriteLayerStates(subStateMachine.stateMachine, writer, indent + "│   ");
            }
        }

        private static string GetStateDetails(UnityEditor.Animations.AnimatorState state)
        {
            var details = new System.Collections.Generic.List<string>();

            if (state.motion != null)
            {
                details.Add($"Motion: {state.motion.name}");
            }

            if (state.speed != 1.0f)
            {
                details.Add($"Speed: {state.speed:F2}");
            }

            if (state.cycleOffset != 0.0f)
            {
                details.Add($"Offset: {state.cycleOffset:F2}");
            }

            if (state.tag != "")
            {
                details.Add($"Tag: {state.tag}");
            }

            return details.Count > 0 ? $" ({string.Join(", ", details)})" : "";
        }

        #endregion
        #endregion


        #region Post Processing Implementation
        private static void SetupGlobalVolumeInScene()
        {
            Log("🎨 Configurando Post Processing...");

            var existing = UnityEngine.Object.FindFirstObjectByType<Volume>();
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

        #region GUI Sections
        private void DrawQuestSystemSection()
        {
            EditorGUILayout.LabelField("🎯 Quest System Tools", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            if (GUILayout.Button("🎯 Create Collect Quest", GUILayout.Height(30)))
            {
                ExtraTools.QuestSystem.QuestCreationTool.CreateCollectQuest();
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("📁 Create Folder Structure", GUILayout.Height(30)))
            {
                ExtraTools.QuestSystem.QuestCreationTool.CreateQuestFolderStructure();
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("🎨 Generate UI Sprites", GUILayout.Height(30)))
            {
                ExtraTools.QuestSystem.QuestSpriteGenerator.GenerateQuestSprites();
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Ferramentas para criação e configuração de quests", MessageType.Info);
        }

        private static void WriteAnimatorConfigurationReport(Animator animator, GameObject gameObject, StreamWriter writer)
        {
            // Cabeçalho
            writer.WriteLine("════════════════════════════════════════════════════════");
            writer.WriteLine($"  CONFIGURAÇÃO COMPLETA DO ANIMATOR");
            writer.WriteLine("════════════════════════════════════════════════════════");
            writer.WriteLine($"GameObject: {gameObject.name}");
            writer.WriteLine($"Caminho: {GetGameObjectPath(gameObject)}");
            writer.WriteLine($"Exportado em: {System.DateTime.Now}");
            writer.WriteLine($"Unity Version: {Application.unityVersion}");
            writer.WriteLine();

            // Informações básicas do Animator
            WriteAnimatorBasicInfo(animator, writer);
            writer.WriteLine();

            // Controller Information
            WriteAnimatorControllerInfo(animator, writer);
            writer.WriteLine();

            // Parâmetros detalhados
            WriteAnimatorParametersDetail(animator, writer);
            writer.WriteLine();

            // Estados e Layers
            WriteAnimatorStatesAndLayers(animator, writer);
            writer.WriteLine();

            // Transições
            WriteAnimatorTransitions(animator, writer);
            writer.WriteLine();

            // Estado atual
            WriteCurrentAnimatorState(animator, writer);
        }

        private static void WriteAnimatorBasicInfo(Animator animator, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ INFORMAÇÕES BÁSICAS DO ANIMATOR");
            writer.WriteLine("└─────────────────────────────────────────────────────────");
            writer.WriteLine($"Controller: {(animator.runtimeAnimatorController ? animator.runtimeAnimatorController.name : "None")}");
            writer.WriteLine($"Avatar: {(animator.avatar ? animator.avatar.name : "None")}");
            writer.WriteLine($"Culling Mode: {animator.cullingMode}");
            writer.WriteLine($"Update Mode: {animator.updateMode}");
            writer.WriteLine($"Apply Root Motion: {animator.applyRootMotion}");
            writer.WriteLine($"Animate Physics: {animator.animatePhysics}");
            writer.WriteLine($"Is Human: {animator.isHuman}");
            writer.WriteLine($"Has Root Motion: {animator.hasRootMotion}");
            writer.WriteLine($"Is Optimizable: {animator.isOptimizable}");
            writer.WriteLine($"Layer Count: {animator.layerCount}");
            writer.WriteLine($"Parameter Count: {animator.parameterCount}");
        }

        private static void WriteAnimatorControllerInfo(Animator animator, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ CONTROLLER INFORMATION");
            writer.WriteLine("└─────────────────────────────────────────────────────────");

            if (animator.runtimeAnimatorController == null)
            {
                writer.WriteLine("❌ Nenhum Animator Controller configurado");
                return;
            }

            var controller = animator.runtimeAnimatorController;
            writer.WriteLine($"Nome: {controller.name}");
            writer.WriteLine($"Tipo: {controller.GetType().Name}");

            if (controller is UnityEditor.Animations.AnimatorController animController)
            {
                writer.WriteLine($"Layers: {animController.layers.Length}");
                writer.WriteLine($"Parameters: {animController.parameters.Length}");
                writer.WriteLine($"Asset Path: {AssetDatabase.GetAssetPath(animController)}");

                // Informações adicionais do controller
                writer.WriteLine($"Animation Clips Count: {animController.animationClips.Length}");
                if (animController.animationClips.Length > 0)
                {
                    writer.WriteLine("Animation Clips:");
                    for (int i = 0; i < animController.animationClips.Length; i++)
                    {
                        var clip = animController.animationClips[i];
                        writer.WriteLine($"  {i + 1}. {clip.name} ({clip.length:F2}s, FPS: {clip.frameRate})");
                    }
                }
            }
        }

        private static void WriteAnimatorParametersDetail(Animator animator, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ PARÂMETROS DETALHADOS");
            writer.WriteLine("└─────────────────────────────────────────────────────────");

            if (animator.parameterCount == 0)
            {
                writer.WriteLine("❌ Nenhum parâmetro configurado");
                return;
            }

            try
            {
                for (int i = 0; i < animator.parameterCount; i++)
                {
                    var param = animator.GetParameter(i);
                    if (param != null)
                    {
                        writer.WriteLine($"{i + 1}. {param.name}");
                        writer.WriteLine($"   ├─ Tipo: {param.type}");
                        writer.WriteLine($"   ├─ Valor Padrão: {GetParameterDefaultValue(param)}");
                        writer.WriteLine($"   ├─ Valor Atual: {GetParameterValue(animator, param)}");
                        writer.WriteLine($"   ├─ Hash: {param.nameHash}");

                        if (i < animator.parameterCount - 1)
                            writer.WriteLine("   │");
                    }
                }
            }
            catch (System.Exception ex)
            {
                writer.WriteLine($"❌ Erro ao ler parâmetros: {ex.Message}");
            }
        }

        private static void WriteAnimatorStatesAndLayers(Animator animator, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ ESTADOS E LAYERS");
            writer.WriteLine("└─────────────────────────────────────────────────────────");

            if (animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController animController)
            {
                try
                {
                    for (int layerIndex = 0; layerIndex < animController.layers.Length; layerIndex++)
                    {
                        var layer = animController.layers[layerIndex];
                        writer.WriteLine($"Layer [{layerIndex}]: {layer.name}");
                        writer.WriteLine($"├─ Peso Padrão: {layer.defaultWeight}");
                        writer.WriteLine($"├─ Peso Atual: {animator.GetLayerWeight(layerIndex)}");
                        writer.WriteLine($"├─ Sync Parâmetros: {(layer.syncedLayerIndex >= 0 ? "Sim (Layer " + layer.syncedLayerIndex + ")" : "Não")}");
                        writer.WriteLine($"├─ IK Pass: {layer.iKPass}");
                        writer.WriteLine($"├─ Avatar Mask: {(layer.avatarMask ? layer.avatarMask.name : "None")}");
                        writer.WriteLine($"├─ Blending Mode: {layer.blendingMode}");

                        if (layer.stateMachine != null)
                        {
                            writer.WriteLine($"├─ State Machine: {layer.stateMachine.name}");
                            writer.WriteLine($"├─ Estados:");
                            WriteLayerStatesDetailed(layer.stateMachine, writer, "│  ");

                            writer.WriteLine($"├─ Sub-State Machines:");
                            WriteSubStateMachines(layer.stateMachine, writer, "│  ");
                        }

                        if (layerIndex < animController.layers.Length - 1)
                            writer.WriteLine("│");
                    }
                }
                catch (System.Exception ex)
                {
                    writer.WriteLine($"❌ Erro ao ler layers: {ex.Message}");
                }
            }
        }

        private static void WriteAnimatorTransitions(Animator animator, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ TRANSIÇÕES");
            writer.WriteLine("└─────────────────────────────────────────────────────────");

            if (animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController animController)
            {
                try
                {
                    for (int layerIndex = 0; layerIndex < animController.layers.Length; layerIndex++)
                    {
                        var layer = animController.layers[layerIndex];
                        writer.WriteLine($"Layer [{layerIndex}]: {layer.name}");

                        WriteStateTransitions(layer.stateMachine, writer, "├─ ");

                        if (layerIndex < animController.layers.Length - 1)
                            writer.WriteLine("│");
                    }
                }
                catch (System.Exception ex)
                {
                    writer.WriteLine($"❌ Erro ao ler transições: {ex.Message}");
                }
            }
        }

        private static void WriteCurrentAnimatorState(Animator animator, StreamWriter writer)
        {
            writer.WriteLine("┌─────────────────────────────────────────────────────────");
            writer.WriteLine("│ ESTADO ATUAL (RUNTIME)");
            writer.WriteLine("└─────────────────────────────────────────────────────────");

            try
            {
                for (int i = 0; i < animator.layerCount; i++)
                {
                    writer.WriteLine($"Layer [{i}]:");
                    var currentState = animator.GetCurrentAnimatorStateInfo(i);
                    writer.WriteLine($"├─ Estado Atual: {GetStateName(animator, i, currentState)}");
                    writer.WriteLine($"├─ Tempo Normalizado: {currentState.normalizedTime:F3}");
                    writer.WriteLine($"├─ Duração: {currentState.length:F3}s");
                    writer.WriteLine($"├─ Velocidade: {currentState.speed:F3}");
                    writer.WriteLine($"├─ Tag: {currentState.tagHash}");
                    writer.WriteLine($"├─ Em Transição: {animator.IsInTransition(i)}");

                    if (animator.IsInTransition(i))
                    {
                        var transitionInfo = animator.GetAnimatorTransitionInfo(i);
                        writer.WriteLine($"├─ Transição:");
                        writer.WriteLine($"│  ├─ Progresso: {transitionInfo.normalizedTime:F3}");
                        writer.WriteLine($"│  ├─ Duração: {transitionInfo.duration:F3}s");
                        writer.WriteLine($"│  └─ Para: Hash {transitionInfo.nameHash}");
                    }

                    if (i < animator.layerCount - 1)
                        writer.WriteLine("│");
                }
            }
            catch (System.Exception ex)
            {
                writer.WriteLine($"❌ Erro ao ler estado atual: {ex.Message}");
            }
        }

        private static string GetParameterDefaultValue(AnimatorControllerParameter param)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    return param.defaultBool.ToString();
                case AnimatorControllerParameterType.Float:
                    return param.defaultFloat.ToString("F2");
                case AnimatorControllerParameterType.Int:
                    return param.defaultInt.ToString();
                case AnimatorControllerParameterType.Trigger:
                    return "False (Trigger)";
                default:
                    return "Unknown";
            }
        }

        private static void WriteLayerStatesDetailed(UnityEditor.Animations.AnimatorStateMachine stateMachine, StreamWriter writer, string prefix)
        {
            if (stateMachine.states == null) return;

            foreach (var stateInfo in stateMachine.states)
            {
                if (stateInfo.state == null) continue;

                var state = stateInfo.state;
                writer.WriteLine($"{prefix}├─ {state.name}");
                writer.WriteLine($"{prefix}│  ├─ Tag: {state.tag}");
                writer.WriteLine($"{prefix}│  ├─ Velocidade: {state.speed}");
                writer.WriteLine($"{prefix}│  ├─ Motion: {(state.motion ? state.motion.name : "None")}");
                writer.WriteLine($"{prefix}│  ├─ Posição: {stateInfo.position}");
                writer.WriteLine($"{prefix}│  ├─ Transições: {state.transitions.Length}");

                if (state.motion is AnimationClip clip)
                {
                    writer.WriteLine($"{prefix}│  ├─ Clip Info:");
                    writer.WriteLine($"{prefix}│  │  ├─ Duração: {clip.length:F2}s");
                    writer.WriteLine($"{prefix}│  │  ├─ FPS: {clip.frameRate}");
                    writer.WriteLine($"{prefix}│  │  ├─ Loop: {clip.isLooping}");
                    writer.WriteLine($"{prefix}│  │  └─ Legacy: {clip.legacy}");
                }

                writer.WriteLine($"{prefix}│  │");
            }
        }

        private static void WriteSubStateMachines(UnityEditor.Animations.AnimatorStateMachine stateMachine, StreamWriter writer, string prefix)
        {
            if (stateMachine.stateMachines == null) return;

            foreach (var subStateMachineInfo in stateMachine.stateMachines)
            {
                if (subStateMachineInfo.stateMachine == null) continue;

                var subSM = subStateMachineInfo.stateMachine;
                writer.WriteLine($"{prefix}├─ {subSM.name}");
                writer.WriteLine($"{prefix}│  ├─ Estados: {subSM.states.Length}");
                writer.WriteLine($"{prefix}│  ├─ Sub-Máquinas: {subSM.stateMachines.Length}");
                writer.WriteLine($"{prefix}│  └─ Posição: {subStateMachineInfo.position}");
            }
        }

        private static void WriteStateTransitions(UnityEditor.Animations.AnimatorStateMachine stateMachine, StreamWriter writer, string prefix)
        {
            if (stateMachine.states == null) return;

            foreach (var stateInfo in stateMachine.states)
            {
                if (stateInfo.state == null || stateInfo.state.transitions == null) continue;

                var state = stateInfo.state;
                if (state.transitions.Length > 0)
                {
                    writer.WriteLine($"{prefix}Estado: {state.name}");
                    foreach (var transition in state.transitions)
                    {
                        string destinationName = transition.destinationState ? transition.destinationState.name :
                                                transition.destinationStateMachine ? transition.destinationStateMachine.name :
                                                "Exit";

                        writer.WriteLine($"{prefix}├─ → {destinationName}");
                        writer.WriteLine($"{prefix}│  ├─ Duração: {transition.duration:F3}");
                        writer.WriteLine($"{prefix}│  ├─ Offset: {transition.offset:F3}");
                        writer.WriteLine($"{prefix}│  ├─ Exit Time: {transition.exitTime:F3}");
                        writer.WriteLine($"{prefix}│  ├─ Has Exit Time: {transition.hasExitTime}");
                        writer.WriteLine($"{prefix}│  ├─ Fixed Duration: {transition.hasFixedDuration}");
                        writer.WriteLine($"{prefix}│  ├─ Interrupt Source: {transition.interruptionSource}");
                        writer.WriteLine($"{prefix}│  ├─ Ordered Interruption: {transition.orderedInterruption}");
                        writer.WriteLine($"{prefix}│  ├─ Can Transition To Self: {transition.canTransitionToSelf}");

                        if (transition.conditions != null && transition.conditions.Length > 0)
                        {
                            writer.WriteLine($"{prefix}│  ├─ Condições:");
                            foreach (var condition in transition.conditions)
                            {
                                writer.WriteLine($"{prefix}│  │  └─ {condition.parameter} {condition.mode} {condition.threshold}");
                            }
                        }
                        else
                        {
                            writer.WriteLine($"{prefix}│  ├─ Condições: Nenhuma");
                        }
                        writer.WriteLine($"{prefix}│  │");
                    }
                }
            }

            // Any State Transitions
            if (stateMachine.anyStateTransitions != null && stateMachine.anyStateTransitions.Length > 0)
            {
                writer.WriteLine($"{prefix}Any State Transitions:");
                foreach (var transition in stateMachine.anyStateTransitions)
                {
                    string destinationName = transition.destinationState ? transition.destinationState.name : "Exit";
                    writer.WriteLine($"{prefix}├─ → {destinationName}");
                    writer.WriteLine($"{prefix}│  ├─ Duração: {transition.duration:F3}");

                    if (transition.conditions != null && transition.conditions.Length > 0)
                    {
                        writer.WriteLine($"{prefix}│  ├─ Condições:");
                        foreach (var condition in transition.conditions)
                        {
                            writer.WriteLine($"{prefix}│  │  └─ {condition.parameter} {condition.mode} {condition.threshold}");
                        }
                    }
                }
            }

            // Entry Transitions
            if (stateMachine.entryTransitions != null && stateMachine.entryTransitions.Length > 0)
            {
                writer.WriteLine($"{prefix}Entry Transitions:");
                foreach (var transition in stateMachine.entryTransitions)
                {
                    string destinationName = transition.destinationState ? transition.destinationState.name : "Exit";
                    writer.WriteLine($"{prefix}├─ → {destinationName}");

                    if (transition.conditions != null && transition.conditions.Length > 0)
                    {
                        writer.WriteLine($"{prefix}│  ├─ Condições:");
                        foreach (var condition in transition.conditions)
                        {
                            writer.WriteLine($"{prefix}│  │  └─ {condition.parameter} {condition.mode} {condition.threshold}");
                        }
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
