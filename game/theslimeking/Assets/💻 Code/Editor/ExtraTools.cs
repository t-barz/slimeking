using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace TheSlimeKing.Tools
{
    /// <summary>
    /// Ferramentas Extras para desenvolvimento do The Slime King
    /// Seguindo as boas práticas estabelecidas no projeto
    /// </summary>
    public class ExtraTools : EditorWindow
    {
        #region Debug & Logs
        [SerializeField] private static bool enableLogs = true;
        [SerializeField] private static bool enableDebug = false;

        private static void Log(string message)
        {
            if (enableLogs)
                Debug.Log($"[ExtraTools] {message}");
        }

        private static void LogWarning(string message)
        {
            if (enableLogs)
                Debug.LogWarning($"[ExtraTools] {message}");
        }

        private static void LogError(string message)
        {
            if (enableLogs)
                Debug.LogError($"[ExtraTools] {message}");
        }

        private static void DebugLog(string message)
        {
            if (enableDebug)
                Debug.Log($"[ExtraTools DEBUG] {message}");
        }
        #endregion

        #region Window Management
        [MenuItem("The Slime King/Tools/Extra Tools Window")]
        public static void ShowWindow()
        {
            GetWindow<ExtraTools>("Extra Tools - The Slime King");
        }
        #endregion

        #region Menu Items - Quick Access
        [MenuItem("The Slime King/Project/Create Folder Structure")]
        public static void MenuCreateFolderStructure()
        {
            CreateProjectFolderStructure();
        }

        [MenuItem("The Slime King/Project/Reorganize Assets")]
        public static void MenuReorganizeAssets()
        {
            ReorganizeExistingAssets();
        }

        [MenuItem("The Slime King/Project/Complete Setup")]
        public static void MenuCompleteSetup()
        {
            CompleteProjectSetup();
        }

        [MenuItem("The Slime King/Debug/Toggle Logs")]
        public static void MenuToggleLogs()
        {
            ToggleLogs();
        }

        [MenuItem("The Slime King/Debug/Export Scene Structure")]
        public static void MenuExportSceneStructure()
        {
            ExportSceneStructure();
        }

        [MenuItem("The Slime King/Post Processing/Setup Global Volume")]
        public static void MenuSetupGlobalVolume()
        {
            SetupGlobalVolumeInScene();
        }

        [MenuItem("The Slime King/Post Processing/Setup Forest Volume")]
        public static void MenuSetupForestVolume()
        {
            SetupBiomeVolume("ForestBiome_Volume");
        }

        [MenuItem("The Slime King/Post Processing/Setup Cave Volume")]
        public static void MenuSetupCaveVolume()
        {
            SetupBiomeVolume("CaveBiome_Volume");
        }

        [MenuItem("The Slime King/Post Processing/Setup Crystal Volume")]
        public static void MenuSetupCrystalVolume()
        {
            SetupBiomeVolume("CrystalBiome_Volume");
        }

        [MenuItem("The Slime King/Post Processing/Setup Gameplay Effects")]
        public static void MenuSetupGameplayEffects()
        {
            SetupGameplayVolumeEffects();
        }
        #endregion

        #region Window GUI
        private Vector2 scrollPosition;

        private void OnGUI()
        {
            DrawHeader();
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            DrawProjectSection();
            DrawAssetsSection();
            DrawDevelopmentSection();
            DrawDebugSection();
            DrawPostProcessingSection();
            DrawUtilitiesSection();
            
            EditorGUILayout.EndScrollView();
            
            DrawFooter();
        }

        private void DrawHeader()
        {
            GUILayout.Label("🎮 Extra Tools - The Slime King", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Ferramentas de desenvolvimento seguindo as boas práticas estabelecidas", MessageType.Info);
            GUILayout.Space(10);
        }

        private void DrawProjectSection()
        {
            EditorGUILayout.LabelField("📁 Estrutura do Projeto", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("🚀 Criar Estrutura de Pastas com Emojis", GUILayout.Height(25)))
            {
                CreateProjectFolderStructure();
            }

            if (GUILayout.Button("🔄 Reorganizar Assets Existentes", GUILayout.Height(25)))
            {
                ReorganizeExistingAssets();
            }

            if (GUILayout.Button("✨ Setup Completo do Projeto", GUILayout.Height(25)))
            {
                CompleteProjectSetup();
            }

            GUILayout.Space(10);
        }

        private void DrawAssetsSection()
        {
            EditorGUILayout.LabelField("📦 Gerenciamento de Assets", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("🧹 Limpar Cache do Unity", GUILayout.Height(25)))
            {
                ClearUnityCache();
            }

            if (GUILayout.Button("📋 Organizar Assets por Tipo", GUILayout.Height(25)))
            {
                OrganizeAssetsByType();
            }

            if (GUILayout.Button("🔍 Encontrar Assets Não Utilizados", GUILayout.Height(25)))
            {
                FindUnusedAssets();
            }

            GUILayout.Space(10);
        }

        private void DrawDevelopmentSection()
        {
            EditorGUILayout.LabelField("⚙️ Desenvolvimento", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("📝 Criar Script Template", GUILayout.Height(25)))
            {
                CreateScriptTemplate();
            }

            if (GUILayout.Button("🗑️ Resetar PlayerPrefs", GUILayout.Height(25)))
            {
                ResetPlayerPrefs();
            }

            if (GUILayout.Button("💾 Backup do Projeto", GUILayout.Height(25)))
            {
                CreateProjectBackup();
            }

            GUILayout.Space(10);
        }

        private void DrawDebugSection()
        {
            EditorGUILayout.LabelField("🐛 Debug e Logs", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            string logStatus = enableLogs ? "🔇 Desabilitar Logs" : "🔊 Habilitar Logs";
            if (GUILayout.Button(logStatus, GUILayout.Height(25)))
            {
                ToggleLogs();
            }

            if (GUILayout.Button("📊 Exportar Estrutura da Cena", GUILayout.Height(25)))
            {
                ExportSceneStructure();
            }

            GUILayout.Space(10);
        }

        private void DrawPostProcessingSection()
        {
            EditorGUILayout.LabelField("🎨 Post Processing", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("🌐 Setup Volume Global", GUILayout.Height(25)))
            {
                SetupGlobalVolumeInScene();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🌲 Floresta", GUILayout.Height(22)))
            {
                SetupBiomeVolume("ForestBiome_Volume");
            }
            if (GUILayout.Button("🏔️ Caverna", GUILayout.Height(22)))
            {
                SetupBiomeVolume("CaveBiome_Volume");
            }
            if (GUILayout.Button("💎 Cristal", GUILayout.Height(22)))
            {
                SetupBiomeVolume("CrystalBiome_Volume");
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("⚡ Setup Efeitos de Gameplay", GUILayout.Height(25)))
            {
                SetupGameplayVolumeEffects();
            }

            GUILayout.Space(10);
        }

        private void DrawUtilitiesSection()
        {
            EditorGUILayout.LabelField("🔧 Utilitários", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("ℹ️ Informações do Projeto", GUILayout.Height(25)))
            {
                ShowProjectInfo();
            }

            if (GUILayout.Button("✅ Validar Configurações", GUILayout.Height(25)))
            {
                ValidateProjectSettings();
            }

            if (GUILayout.Button("📁 Abrir Pasta de Logs", GUILayout.Height(25)))
            {
                OpenLogsFolder();
            }

            GUILayout.Space(10);
        }

        private void DrawFooter()
        {
            EditorGUILayout.HelpBox("🎮 The Slime King - Desenvolvido seguindo as boas práticas Unity 6.2+", MessageType.None);
        }
        #endregion

        #region Project Structure Methods
        /// <summary>
        /// Cria a estrutura de pastas com emojis conforme boas práticas
        /// </summary>
        private static void CreateProjectFolderStructure()
        {
            Log("🚀 Iniciando criação da estrutura de pastas com emojis...");

            string[] folders = {
                // 🎨 Art - Todo visual
                "Assets/🎨 Art",
                "Assets/🎨 Art/Sprites",
                "Assets/🎨 Art/Materials", 
                "Assets/🎨 Art/Animations",
                "Assets/🎨 Art/Animations/Controllers",
                "Assets/🎨 Art/Animations/Clips",
                
                // 🔊 Audio - Todo sonoro  
                "Assets/🔊 Audio",
                "Assets/🔊 Audio/Music", 
                "Assets/🔊 Audio/SFX",
                
                // 💻 Code - Scripts organizados por função
                "Assets/💻 Code",
                "Assets/💻 Code/Gameplay",
                "Assets/💻 Code/Systems",
                "Assets/💻 Code/Editor", 
                
                // 🎮 Game - Conteúdo específico
                "Assets/🎮 Game",
                "Assets/🎮 Game/Scenes",
                "Assets/🎮 Game/Prefabs",
                "Assets/🎮 Game/Data",
                
                // ⚙️ Settings - Configurações Unity
                "Assets/⚙️ Settings",
                "Assets/⚙️ Settings/PostProcessing",
                
                // 📦 External - Assets terceiros
                "Assets/📦 External",
                "Assets/📦 External/AssetStore",
                "Assets/📦 External/Plugins",
                "Assets/📦 External/Libraries", 
                "Assets/📦 External/Tools"
            };

            int foldersCreated = 0;

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    CreateFolder(folder);
                    
                    // Cria arquivo .gitkeep para manter pasta no Git
                    string gitkeepPath = Path.Combine(folder, ".gitkeep");
                    string gitkeepContent = "# 📁 Estrutura Organizacional - The Slime King\n" +
                                          "# Mantém esta pasta no controle de versão mesmo quando vazia\n" +
                                          "# Estrutura criada seguindo as boas práticas do projeto";
                    File.WriteAllText(gitkeepPath, gitkeepContent);
                    
                    foldersCreated++;
                    Log($"📂 Pasta criada: {folder}");
                }
                else
                {
                    DebugLog($"📂 Pasta já existe: {folder}");
                }
            }

            AssetDatabase.Refresh();

            if (foldersCreated > 0)
            {
                Log($"✅ Estrutura criada com sucesso! {foldersCreated} pastas criadas.");
                EditorUtility.DisplayDialog("Estrutura Criada - The Slime King",
                    $"✅ Estrutura de pastas com emojis criada!\n\n" +
                    $"📊 {foldersCreated} pastas criadas\n" +
                    $"📁 Arquivos .gitkeep adicionados\n\n" +
                    $"Estrutura criada:\n" +
                    $"🎨 Art/ - Todo visual\n" +
                    $"🔊 Audio/ - Todo sonoro\n" +
                    $"💻 Code/ - Scripts organizados\n" +
                    $"🎮 Game/ - Conteúdo específico\n" +
                    $"⚙️ Settings/ - Configurações Unity\n" +
                    $"📦 External/ - Assets terceiros",
                    "OK");
            }
            else
            {
                Log("ℹ️ Estrutura já existe. Nenhuma pasta foi criada.");
                EditorUtility.DisplayDialog("Estrutura Existente",
                    "ℹ️ A estrutura de pastas com emojis já existe no projeto.\nNenhuma alteração foi necessária.",
                    "OK");
            }
        }

        /// <summary>
        /// Cria uma pasta no projeto Unity recursivamente
        /// </summary>
        private static void CreateFolder(string folderPath)
        {
            string[] pathParts = folderPath.Split('/');
            string currentPath = pathParts[0];

            for (int i = 1; i < pathParts.Length; i++)
            {
                string newPath = currentPath + "/" + pathParts[i];

                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    string result = AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                    DebugLog($"Criando pasta: {newPath} -> {result}");
                }

                currentPath = newPath;
            }
        }

        /// <summary>
        /// Reorganiza assets existentes para a nova estrutura
        /// </summary>
        private static void ReorganizeExistingAssets()
        {
            Log("🔄 Iniciando reorganização automática de assets...");

            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            int movedAssets = 0;

            foreach (string assetPath in allAssets)
            {
                // Ignora pastas do sistema
                if (assetPath.StartsWith("Packages/") ||
                    assetPath.StartsWith("Library/") ||
                    assetPath.StartsWith("ProjectSettings/") ||
                    assetPath.StartsWith("UserSettings/"))
                    continue;

                // Ignora se já está na pasta correta
                if (IsInCorrectFolder(assetPath))
                    continue;

                string newPath = GetNewPathForAsset(assetPath);

                if (!string.IsNullOrEmpty(newPath) && newPath != assetPath)
                {
                    string result = AssetDatabase.MoveAsset(assetPath, newPath);
                    if (string.IsNullOrEmpty(result))
                    {
                        movedAssets++;
                        DebugLog($"Asset movido: {assetPath} -> {newPath}");
                    }
                    else
                    {
                        LogWarning($"Erro ao mover asset {assetPath}: {result}");
                    }
                }
            }

            AssetDatabase.Refresh();
            Log($"✅ Reorganização concluída! {movedAssets} assets reorganizados.");

            if (movedAssets > 0)
            {
                EditorUtility.DisplayDialog("Reorganização Concluída",
                    $"✅ {movedAssets} assets foram reorganizados para a nova estrutura.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Reorganização",
                    "ℹ️ Todos os assets já estão organizados corretamente!", "OK");
            }
        }

        /// <summary>
        /// Verifica se o asset já está na pasta correta
        /// </summary>
        private static bool IsInCorrectFolder(string assetPath)
        {
            string[] correctFolders = {
                "Assets/🎨 Art/", "Assets/🔊 Audio/", "Assets/💻 Code/",
                "Assets/🎮 Game/", "Assets/📦 External/", "Assets/⚙️ Settings/"
            };

            foreach (string folder in correctFolders)
            {
                if (assetPath.StartsWith(folder))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determina o novo caminho para um asset baseado em sua extensão e nome
        /// </summary>
        private static string GetNewPathForAsset(string assetPath)
        {
            string fileName = Path.GetFileName(assetPath);
            string extension = Path.GetExtension(assetPath).ToLower();

            // Cenas
            if (extension == ".unity")
            {
                return "Assets/🎮 Game/Scenes/" + fileName;
            }

            // Scripts - organização por função seguindo as boas práticas
            if (extension == ".cs")
            {
                if (assetPath.Contains("Editor") || fileName.Contains("Editor"))
                    return "Assets/💻 Code/Editor/" + fileName;
                else if (fileName.Contains("Manager") || fileName.Contains("System") || fileName.Contains("Handler"))
                    return "Assets/💻 Code/Systems/" + fileName;
                else
                    return "Assets/💻 Code/Gameplay/" + fileName;
            }

            // Imagens e Sprites
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" ||
                extension == ".tga" || extension == ".psd" || extension == ".svg")
            {
                return "Assets/🎨 Art/Sprites/" + fileName;
            }

            // Áudio
            if (extension == ".wav" || extension == ".mp3" || extension == ".ogg")
            {
                if (fileName.ToLower().Contains("music") || fileName.ToLower().Contains("bgm") ||
                    fileName.ToLower().Contains("mus_"))
                    return "Assets/🔊 Audio/Music/" + fileName;
                else
                    return "Assets/🔊 Audio/SFX/" + fileName;
            }

            // Materiais
            if (extension == ".mat")
            {
                return "Assets/🎨 Art/Materials/" + fileName;
            }

            // Prefabs
            if (extension == ".prefab")
            {
                return "Assets/🎮 Game/Prefabs/" + fileName;
            }

            // Animações
            if (extension == ".anim")
            {
                return "Assets/🎨 Art/Animations/Clips/" + fileName;
            }

            // Controllers de Animação
            if (extension == ".controller")
            {
                return "Assets/🎨 Art/Animations/Controllers/" + fileName;
            }

            // Fontes e dados - ScriptableObjects
            if (extension == ".ttf" || extension == ".otf" || extension == ".asset")
            {
                return "Assets/🎮 Game/Data/" + fileName;
            }

            // Input Actions - configurações do projeto
            if (extension == ".inputactions")
            {
                return "Assets/⚙️ Settings/" + fileName;
            }

            // Volume Profiles - Post Processing
            if (fileName.Contains("Volume") || fileName.Contains("Profile"))
            {
                return "Assets/⚙️ Settings/PostProcessing/" + fileName;
            }

            // Arquivos não reconhecidos vão para External
            if (assetPath.StartsWith("Assets/") && !IsInCorrectFolder(assetPath))
            {
                return "Assets/📦 External/" + fileName;
            }

            return "";
        }

        /// <summary>
        /// Executa setup completo do projeto: cria estrutura + reorganiza assets
        /// </summary>
        private static void CompleteProjectSetup()
        {
            Log("✨ Executando setup completo do projeto...");

            CreateProjectFolderStructure();
            ReorganizeExistingAssets();

            Log("🎉 Setup completo finalizado!");
            EditorUtility.DisplayDialog("Setup Completo - The Slime King",
                "🎉 Setup completo do projeto finalizado com sucesso!\n\n" +
                "✅ Estrutura de pastas criada\n" +
                "✅ Assets reorganizados\n" +
                "✅ Arquivos .gitkeep adicionados\n\n" +
                "O projeto está pronto para desenvolvimento!",
                "OK");
        }
        #endregion

        #region Utility Methods
        private static void ClearUnityCache()
        {
            bool result = EditorUtility.DisplayDialog("Limpar Cache",
                "Isso irá limpar o cache do Unity e pode demorar um pouco para recompilar.\n\nContinuar?",
                "Sim", "Cancelar");

            if (result)
            {
                AssetDatabase.Refresh();
                EditorUtility.UnloadUnusedAssetsImmediate();

                Log("✅ Cache do Unity limpo!");
                EditorUtility.DisplayDialog("Cache Limpo", "✅ Cache do Unity foi limpo com sucesso!", "OK");
            }
        }

        private static void OrganizeAssetsByType()
        {
            Log("📋 Organizando assets por tipo...");
            
            string[] guids = AssetDatabase.FindAssets("");
            int organized = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.StartsWith("Assets/") && !path.Contains("/"))
                {
                    string extension = Path.GetExtension(path).ToLower();
                    string targetFolder = GetFolderForExtension(extension);

                    if (!string.IsNullOrEmpty(targetFolder))
                    {
                        string newPath = $"Assets/{targetFolder}/{Path.GetFileName(path)}";
                        string error = AssetDatabase.MoveAsset(path, newPath);
                        if (string.IsNullOrEmpty(error))
                        {
                            organized++;
                        }
                    }
                }
            }

            AssetDatabase.Refresh();
            Log($"✅ {organized} assets organizados por tipo!");
            EditorUtility.DisplayDialog("Assets Organizados", $"✅ {organized} assets organizados por tipo!", "OK");
        }

        private static string GetFolderForExtension(string extension)
        {
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".psd":
                    return "🎨 Art/Sprites";
                case ".wav":
                case ".mp3":
                case ".ogg":
                    return "🔊 Audio/SFX";
                case ".cs":
                    return "💻 Code/Gameplay";
                case ".prefab":
                    return "🎮 Game/Prefabs";
                case ".unity":
                    return "🎮 Game/Scenes";
                case ".mat":
                    return "🎨 Art/Materials";
                case ".anim":
                    return "🎨 Art/Animations/Clips";
                case ".controller":
                    return "🎨 Art/Animations/Controllers";
                default:
                    return "";
            }
        }

        private static void FindUnusedAssets()
        {
            Log("🔍 Procurando assets não utilizados...");
            
            string[] allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/") && !AssetDatabase.IsValidFolder(path))
                .ToArray();

            // Simplificada: só verifica se estão em cenas
            var scenePaths = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid));

            HashSet<string> usedAssets = new HashSet<string>();

            foreach (string scenePath in scenePaths)
            {
                string[] dependencies = AssetDatabase.GetDependencies(scenePath);
                foreach (string dep in dependencies)
                {
                    usedAssets.Add(dep);
                }
            }

            var unusedAssets = allAssets.Except(usedAssets).ToList();

            if (unusedAssets.Count > 0)
            {
                string message = $"🔍 Encontrados {unusedAssets.Count} assets possivelmente não utilizados:\n\n";
                message += string.Join("\n", unusedAssets.Take(10));
                if (unusedAssets.Count > 10)
                {
                    message += $"\n... e mais {unusedAssets.Count - 10} assets.";
                }
                message += "\n\n⚠️ ATENÇÃO: Verifique manualmente antes de deletar!";

                EditorUtility.DisplayDialog("Assets Não Utilizados", message, "OK");
                Log($"⚠️ {unusedAssets.Count} assets possivelmente não utilizados encontrados");
            }
            else
            {
                EditorUtility.DisplayDialog("Assets", "✅ Todos os assets parecem estar sendo utilizados!", "OK");
                Log("✅ Nenhum asset não utilizado encontrado");
            }
        }

        private static void CreateScriptTemplate()
        {
            string templatePath = "Assets/💻 Code/ScriptTemplate.cs";
            string templateContent = @"using UnityEngine;

namespace TheSlimeKing
{
    /// <summary>
    /// Template de script seguindo as boas práticas do The Slime King
    /// </summary>
    public class NewScript : MonoBehaviour
    {
        #region Debug & Logs
        [Header(""Debug"")]
        [SerializeField] private bool enableLogs = true;
        [SerializeField] private bool enableDebug = false;
        
        private void Log(string message)
        {
            if (enableLogs)
                Debug.Log($""[{GetType().Name}] {message}"");
        }
        #endregion
        
        #region Configuration
        [Header(""Configurações"")]
        [SerializeField] private float speed = 5f;
        #endregion
        
        #region Unity Lifecycle
        private void Start()
        {
            Log(""Script inicializado"");
        }

        private void Update()
        {
            // Lógica por frame
        }
        
        private void OnValidate()
        {
            // Validação no editor
        }
        #endregion
    }
}";

            File.WriteAllText(templatePath, templateContent);
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<MonoScript>(templatePath));
            Log("✅ Template de script criado seguindo as boas práticas!");
            EditorUtility.DisplayDialog("Template Criado", "✅ Template de script criado seguindo as boas práticas do The Slime King!", "OK");
        }

        private static void ResetPlayerPrefs()
        {
            bool result = EditorUtility.DisplayDialog("Reset PlayerPrefs",
                "Isso irá apagar todos os PlayerPrefs salvos.\n\nContinuar?",
                "Sim", "Cancelar");

            if (result)
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                Log("✅ PlayerPrefs resetados!");
                EditorUtility.DisplayDialog("PlayerPrefs", "✅ PlayerPrefs foram resetados com sucesso!", "OK");
            }
        }

        private static void CreateProjectBackup()
        {
            string backupPath = EditorUtility.SaveFolderPanel("Escolher pasta para backup", "", "");
            if (!string.IsNullOrEmpty(backupPath))
            {
                string projectName = Path.GetFileName(Application.dataPath.Replace("/Assets", ""));
                string backupFolder = Path.Combine(backupPath, $"{projectName}_Backup_{System.DateTime.Now:yyyyMMdd_HHmmss}");

                try
                {
                    Directory.CreateDirectory(backupFolder);

                    // Copiar apenas pastas essenciais
                    string[] essentialFolders = { "Assets", "ProjectSettings", "Packages" };

                    foreach (string folder in essentialFolders)
                    {
                        string sourcePath = Path.Combine(Application.dataPath.Replace("/Assets", ""), folder);
                        string destPath = Path.Combine(backupFolder, folder);

                        if (Directory.Exists(sourcePath))
                        {
                            CopyDirectory(sourcePath, destPath);
                        }
                    }

                    Log($"✅ Backup criado em: {backupFolder}");
                    EditorUtility.DisplayDialog("Backup", $"✅ Backup criado com sucesso!\n\n{backupFolder}", "OK");
                    System.Diagnostics.Process.Start(backupFolder);
                }
                catch (System.Exception e)
                {
                    LogError($"Erro ao criar backup: {e.Message}");
                    EditorUtility.DisplayDialog("Erro", $"❌ Erro ao criar backup:\n{e.Message}", "OK");
                }
            }
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, destSubDir);
            }
        }

        private static void ShowProjectInfo()
        {
            string info = $"📊 Informações do Projeto - The Slime King\n\n";
            info += $"Unity Version: {Application.unityVersion}\n";
            info += $"Platform: {Application.platform}\n";
            info += $"Data Path: {Application.dataPath}\n";
            info += $"Persistent Data: {Application.persistentDataPath}\n";
            info += $"Temporary Cache: {Application.temporaryCachePath}\n\n";

            info += $"⚙️ Configurações Atuais:\n";
            info += $"Screen Size: {Screen.width}x{Screen.height}\n";
            info += $"Target FPS: {Application.targetFrameRate}\n";
            info += $"VSync: {QualitySettings.vSyncCount}\n";
            info += $"Quality Level: {QualitySettings.GetQualityLevel()}\n";

            EditorUtility.DisplayDialog("Informações do Projeto", info, "OK");
        }

        private static void ValidateProjectSettings()
        {
            string validation = "✅ Validação das Configurações:\n\n";
            bool allGood = true;

            // Verificar URP
            if (QualitySettings.renderPipeline == null)
            {
                validation += "❌ URP não está configurado\n";
                allGood = false;
            }
            else
            {
                validation += "✅ URP configurado\n";
            }

            // Verificar Input System
            bool newInputSystem = false;
#if ENABLE_INPUT_SYSTEM
            newInputSystem = true;
#endif

            if (newInputSystem)
            {
                validation += "✅ New Input System ativo\n";
            }
            else
            {
                validation += "❌ New Input System não está ativo\n";
                allGood = false;
            }

            // Verificar estrutura de pastas com emojis
            string[] requiredFolders = { 
                "Assets/🎨 Art", "Assets/🔊 Audio", "Assets/💻 Code", 
                "Assets/🎮 Game", "Assets/⚙️ Settings" 
            };
            bool foldersOk = true;

            foreach (string folder in requiredFolders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    foldersOk = false;
                    break;
                }
            }

            if (foldersOk)
            {
                validation += "✅ Estrutura de pastas com emojis OK\n";
            }
            else
            {
                validation += "❌ Estrutura de pastas com emojis incompleta\n";
                allGood = false;
            }

            validation += "\n";
            if (allGood)
            {
                validation += "🎉 Projeto está bem configurado seguindo as boas práticas!";
            }
            else
            {
                validation += "⚠️ Algumas configurações precisam de atenção.";
            }

            EditorUtility.DisplayDialog("Validação do Projeto", validation, "OK");
        }

        private static void OpenLogsFolder()
        {
            string logPath = "";

            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    logPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Unity", "Editor", "Editor.log");
                    break;
                case RuntimePlatform.OSXEditor:
                    logPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Library", "Logs", "Unity", "Editor.log");
                    break;
                case RuntimePlatform.LinuxEditor:
                    logPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".config", "unity3d", "Editor.log");
                    break;
            }

            if (!string.IsNullOrEmpty(logPath) && File.Exists(logPath))
            {
                EditorUtility.RevealInFinder(logPath);
            }
            else
            {
                // Fallback - abrir pasta de logs do projeto
                string projectLogPath = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Logs");
                if (Directory.Exists(projectLogPath))
                {
                    EditorUtility.RevealInFinder(projectLogPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Logs", "📁 Pasta de logs não encontrada.", "OK");
                }
            }
        }
        #endregion

        #region Debug Methods
        /// <summary>
        /// Alterna o estado dos logs do sistema
        /// </summary>
        private static void ToggleLogs()
        {
            enableLogs = !enableLogs;
            Log($"Logs {(enableLogs ? "habilitados" : "desabilitados")}");
            EditorUtility.DisplayDialog("Logs", $"🔊 Logs foram {(enableLogs ? "habilitados" : "desabilitados")}!", "OK");
        }

        /// <summary>
        /// Exporta a estrutura completa da cena ativa para análise
        /// </summary>
        private static void ExportSceneStructure()
        {
            Log("📊 Exportando estrutura da cena ativa...");
            ExportCurrentSceneStructure();
        }

        /// <summary>
        /// Implementação da exportação da estrutura da cena
        /// </summary>
        private static void ExportCurrentSceneStructure()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                LogError("Nenhuma cena ativa encontrada!");
                return;
            }

            var fileName = $"SceneStructure_{scene.name}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var filePath = Path.Combine(Application.dataPath, "..", "Logs", fileName);

            // Cria diretório Logs se não existir
            var logsDir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(logsDir))
                Directory.CreateDirectory(logsDir);

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"======================================");
                writer.WriteLine($"ESTRUTURA DA CENA: {scene.name}");
                writer.WriteLine($"Exportado em: {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine($"Caminho da cena: {scene.path}");
                writer.WriteLine($"Total de GameObjects: {scene.rootCount}");
                writer.WriteLine($"======================================");
                writer.WriteLine();

                // Analisa cada GameObject raiz
                var rootObjects = scene.GetRootGameObjects();
                for (int i = 0; i < rootObjects.Length; i++)
                {
                    writer.WriteLine($"[{i + 1}/{rootObjects.Length}] ROOT OBJECT:");
                    AnalyzeGameObject(rootObjects[i], writer, 0);
                    writer.WriteLine();
                }

                writer.WriteLine($"======================================");
                writer.WriteLine($"RESUMO DA EXPORTAÇÃO");
                writer.WriteLine($"Total de objetos raiz: {rootObjects.Length}");
                writer.WriteLine($"Arquivo gerado: {fileName}");
                writer.WriteLine($"======================================");
            }

            Log($"✅ Estrutura da cena exportada para: {filePath}");
            EditorUtility.DisplayDialog("Exportação Concluída",
                $"📊 Estrutura da cena '{scene.name}' exportada com sucesso!\n\nArquivo: {fileName}\nLocalização: {logsDir}", "OK");

            // Abre a pasta dos logs
            EditorUtility.RevealInFinder(filePath);
        }

        /// <summary>
        /// Analisa recursivamente um GameObject e seus filhos
        /// </summary>
        private static void AnalyzeGameObject(GameObject go, StreamWriter writer, int depth)
        {
            string indent = new string(' ', depth * 2);
            string prefix = depth == 0 ? "├── " : "│   ├── ";

            writer.WriteLine($"{indent}{prefix}{go.name}");
            writer.WriteLine($"{indent}│   │   Active: {go.activeInHierarchy}");
            writer.WriteLine($"{indent}│   │   Tag: {go.tag}");
            writer.WriteLine($"{indent}│   │   Layer: {LayerMask.LayerToName(go.layer)} ({go.layer})");

            // Analisa Transform
            var transform = go.transform;
            if (transform is RectTransform rectTransform)
            {
                writer.WriteLine($"{indent}│   │   Transform: RectTransform");
                writer.WriteLine($"{indent}│   │   │   Position: {rectTransform.anchoredPosition}");
                writer.WriteLine($"{indent}│   │   │   Size: {rectTransform.sizeDelta}");
                writer.WriteLine($"{indent}│   │   │   Anchors: {rectTransform.anchorMin} - {rectTransform.anchorMax}");
                writer.WriteLine($"{indent}│   │   │   Pivot: {rectTransform.pivot}");
                writer.WriteLine($"{indent}│   │   │   Rotation: {rectTransform.localEulerAngles}");
                writer.WriteLine($"{indent}│   │   │   Scale: {rectTransform.localScale}");
            }
            else
            {
                writer.WriteLine($"{indent}│   │   Transform: Transform");
                writer.WriteLine($"{indent}│   │   │   Position: {transform.localPosition}");
                writer.WriteLine($"{indent}│   │   │   Rotation: {transform.localEulerAngles}");
                writer.WriteLine($"{indent}│   │   │   Scale: {transform.localScale}");
            }

            // Analisa componentes
            var components = go.GetComponents<Component>();
            writer.WriteLine($"{indent}│   │   Components: ({components.Length})");

            foreach (var component in components)
            {
                if (component == null) continue;

                var componentType = component.GetType();
                writer.WriteLine($"{indent}│   │   │   ├── {componentType.Name}");

                // Informações específicas para componentes comuns
                AnalyzeSpecificComponent(component, writer, indent + "│   │   │   │   ");
            }

            // Analisa filhos recursivamente
            if (transform.childCount > 0)
            {
                writer.WriteLine($"{indent}│   │   Children: ({transform.childCount})");
                for (int i = 0; i < transform.childCount; i++)
                {
                    AnalyzeGameObject(transform.GetChild(i).gameObject, writer, depth + 2);
                }
            }
        }

        /// <summary>
        /// Analisa componentes específicos com informações detalhadas
        /// </summary>
        private static void AnalyzeSpecificComponent(Component component, StreamWriter writer, string indent)
        {
            // Canvas
            if (component is Canvas canvas)
            {
                writer.WriteLine($"{indent}Render Mode: {canvas.renderMode}");
                writer.WriteLine($"{indent}Sort Order: {canvas.sortingOrder}");
                writer.WriteLine($"{indent}Pixel Perfect: {canvas.pixelPerfect}");
            }
            // Image
            else if (component is UnityEngine.UI.Image image)
            {
                writer.WriteLine($"{indent}Sprite: {(image.sprite ? image.sprite.name : "None")}");
                writer.WriteLine($"{indent}Color: {image.color}");
                writer.WriteLine($"{indent}Material: {(image.material ? image.material.name : "None")}");
                writer.WriteLine($"{indent}Raycast Target: {image.raycastTarget}");
            }
            // Camera
            else if (component is Camera cam)
            {
                writer.WriteLine($"{indent}Clear Flags: {cam.clearFlags}");
                writer.WriteLine($"{indent}Projection: {cam.orthographic}");
                writer.WriteLine($"{indent}Size/FOV: {(cam.orthographic ? cam.orthographicSize.ToString() : cam.fieldOfView.ToString())}");
                writer.WriteLine($"{indent}Near/Far: {cam.nearClipPlane}/{cam.farClipPlane}");
                writer.WriteLine($"{indent}Depth: {cam.depth}");
            }
            // Light
            else if (component is Light light)
            {
                writer.WriteLine($"{indent}Type: {light.type}");
                writer.WriteLine($"{indent}Color: {light.color}");
                writer.WriteLine($"{indent}Intensity: {light.intensity}");
                writer.WriteLine($"{indent}Range: {light.range}");
            }
            // AudioSource
            else if (component is AudioSource audioSource)
            {
                writer.WriteLine($"{indent}Clip: {(audioSource.clip ? audioSource.clip.name : "None")}");
                writer.WriteLine($"{indent}Volume: {audioSource.volume}");
                writer.WriteLine($"{indent}Loop: {audioSource.loop}");
                writer.WriteLine($"{indent}Play On Awake: {audioSource.playOnAwake}");
            }
            // Volume (Post Processing)
            else if (component is Volume volume)
            {
                writer.WriteLine($"{indent}Is Global: {volume.isGlobal}");
                writer.WriteLine($"{indent}Priority: {volume.priority}");
                writer.WriteLine($"{indent}Profile: {(volume.profile ? volume.profile.name : "None")}");
            }
        }
        #endregion

        #region Post Processing Methods
        /// <summary>
        /// Configura Post Processing na cena ativa com Volume global
        /// </summary>
        private static void SetupGlobalVolumeInScene()
        {
            Log("🎨 Configurando Post Processing na cena ativa...");

            if (!ValidateURPSetup())
                return;

            // Verifica se já existe um Volume global na cena
            Volume existingVolume = Object.FindFirstObjectByType<Volume>();
            if (existingVolume != null && existingVolume.isGlobal)
            {
                Log("Volume global já existe na cena");
                EditorUtility.DisplayDialog("Post Processing", "ℹ️ Volume global já existe na cena!", "OK");
                return;
            }

            // Carrega o Global Volume Profile
            VolumeProfile globalProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(
                "Assets/⚙️ Settings/PostProcessing/GlobalVolumeProfile.asset");

            if (globalProfile == null)
            {
                LogError("GlobalVolumeProfile.asset não encontrado! Criando profile básico...");

                // Cria um profile básico
                globalProfile = CreateBasicVolumeProfile("GlobalVolumeProfile");
                if (globalProfile == null)
                    return;
            }

            // Cria GameObject para o Volume
            GameObject volumeGO = new GameObject("🌐 Global Volume");
            Volume volume = volumeGO.AddComponent<Volume>();

            // Configura o Volume
            volume.isGlobal = true;
            volume.priority = 0;
            volume.profile = globalProfile;

            // Posiciona na origem
            volumeGO.transform.position = Vector3.zero;

            // Registra para Undo
            Undo.RegisterCreatedObjectUndo(volumeGO, "Create Global Volume");

            // Seleciona o objeto criado
            Selection.activeGameObject = volumeGO;

            Log("✅ Volume global configurado com sucesso na cena!");
            EditorUtility.DisplayDialog("Post Processing Setup",
                "✅ Volume global configurado com sucesso!\nO GameObject foi criado e selecionado na cena.", "OK");
        }

        /// <summary>
        /// Configura Volume específico para bioma
        /// </summary>
        private static void SetupBiomeVolume(string profileName)
        {
            if (!ValidateURPSetup())
                return;

            // Carrega o Volume Profile do bioma
            string profilePath = $"Assets/⚙️ Settings/PostProcessing/{profileName}.asset";
            VolumeProfile biomeProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);

            if (biomeProfile == null)
            {
                LogError($"Volume Profile '{profileName}' não encontrado! Criando profile básico...");

                // Cria um profile básico para o bioma
                biomeProfile = CreateBasicVolumeProfile(profileName);
                if (biomeProfile == null)
                    return;
            }

            // Cria GameObject para o Volume
            string biomeName = profileName.Replace("Biome_Volume", "").Replace("_", " ");
            string biomeEmoji = GetBiomeEmoji(profileName);
            GameObject volumeGO = new GameObject($"{biomeEmoji} {biomeName} Volume");
            Volume volume = volumeGO.AddComponent<Volume>();

            // Configura o Volume
            volume.isGlobal = false;
            volume.priority = 1; // Maior que global
            volume.profile = biomeProfile;

            // Adiciona Box Collider como trigger para delimitar área
            BoxCollider boxCollider = volumeGO.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(20, 20, 20); // Tamanho padrão

            // Posiciona na origem
            volumeGO.transform.position = Vector3.zero;

            // Registra para Undo
            Undo.RegisterCreatedObjectUndo(volumeGO, $"Create {biomeName} Volume");

            // Seleciona o objeto criado
            Selection.activeGameObject = volumeGO;

            Log($"✅ Volume '{biomeName}' configurado com sucesso!");
            EditorUtility.DisplayDialog("Biome Volume Setup",
                $"✅ Volume '{biomeName}' configurado com sucesso!\n\nAjuste o Box Collider para delimitar a área do bioma.\nO Volume tem prioridade 1 (maior que global).", "OK");
        }

        /// <summary>
        /// Configura efeitos de gameplay (Hit e Evolution)
        /// </summary>
        private static void SetupGameplayVolumeEffects()
        {
            if (!ValidateURPSetup())
                return;

            bool hitCreated = CreateGameplayEffectVolume("HitEffect_Volume", "Hit Effect", 10);
            bool evolutionCreated = CreateGameplayEffectVolume("EvolutionEffect_Volume", "Evolution Effect", 15);

            if (hitCreated || evolutionCreated)
            {
                string message = "⚡ Efeitos de gameplay configurados:\n";
                if (hitCreated) message += "✅ Hit Effect Volume (prioridade 10)\n";
                if (evolutionCreated) message += "✅ Evolution Effect Volume (prioridade 15)\n";
                message += "\nℹ️ Os volumes estão desabilitados por padrão.\nAtive via script quando necessário.";

                EditorUtility.DisplayDialog("Gameplay Effects Setup", message, "OK");
            }
        }

        /// <summary>
        /// Cria um Volume para efeito de gameplay específico
        /// </summary>
        private static bool CreateGameplayEffectVolume(string profileName, string effectName, int priority)
        {
            // Verifica se já existe
            GameObject existing = GameObject.Find($"{effectName} Volume");
            if (existing != null)
            {
                Log($"Volume '{effectName}' já existe na cena");
                return false;
            }

            // Carrega o Volume Profile
            string profilePath = $"Assets/⚙️ Settings/PostProcessing/{profileName}.asset";
            VolumeProfile effectProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);

            if (effectProfile == null)
            {
                LogError($"Volume Profile '{profileName}' não encontrado! Criando profile básico...");

                // Cria um profile básico para o efeito
                effectProfile = CreateBasicVolumeProfile(profileName);
                if (effectProfile == null)
                    return false;
            }

            // Cria GameObject para o Volume
            string effectEmoji = effectName.Contains("Hit") ? "💥" : "✨";
            GameObject volumeGO = new GameObject($"{effectEmoji} {effectName} Volume");
            Volume volume = volumeGO.AddComponent<Volume>();

            // Configura o Volume
            volume.isGlobal = true;
            volume.priority = priority;
            volume.profile = effectProfile;

            // Desabilita o GameObject por padrão (será ativado via script)
            volumeGO.SetActive(false);

            // Posiciona na origem
            volumeGO.transform.position = Vector3.zero;

            // Registra para Undo
            Undo.RegisterCreatedObjectUndo(volumeGO, $"Create {effectName} Volume");

            Log($"✅ Volume '{effectName}' criado com sucesso!");
            return true;
        }

        /// <summary>
        /// Valida se URP está configurado corretamente
        /// </summary>
        private static bool ValidateURPSetup()
        {
            if (QualitySettings.renderPipeline == null)
            {
                LogError("URP não está configurado! Configure o URP primeiro.");
                EditorUtility.DisplayDialog("Erro URP",
                    "❌ Universal Render Pipeline não está configurado!\n\nPor favor, configure o URP antes de usar ferramentas de Post Processing.", "OK");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cria um Volume Profile básico
        /// </summary>
        private static VolumeProfile CreateBasicVolumeProfile(string profileName, string folderPath = "Assets/⚙️ Settings/PostProcessing/")
        {
            // Cria diretório se não existir
            if (!AssetDatabase.IsValidFolder(folderPath.TrimEnd('/')))
            {
                string[] pathParts = folderPath.TrimEnd('/').Split('/');
                string currentPath = "";

                for (int i = 0; i < pathParts.Length; i++)
                {
                    if (i == 0)
                    {
                        currentPath = pathParts[i];
                    }
                    else
                    {
                        string newPath = currentPath + "/" + pathParts[i];
                        if (!AssetDatabase.IsValidFolder(newPath))
                        {
                            AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                        }
                        currentPath = newPath;
                    }
                }
            }

            // Cria o Volume Profile
            VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
            string fullPath = folderPath + profileName + ".asset";

            AssetDatabase.CreateAsset(profile, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Log($"✅ Volume Profile básico criado: {fullPath}");
            return profile;
        }

        /// <summary>
        /// Retorna emoji apropriado para o bioma
        /// </summary>
        private static string GetBiomeEmoji(string profileName)
        {
            if (profileName.Contains("Forest")) return "🌲";
            if (profileName.Contains("Cave")) return "🏔️";
            if (profileName.Contains("Crystal")) return "💎";
            return "🌍";
        }
        #endregion
    }
}