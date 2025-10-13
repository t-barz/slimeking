using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

namespace ExtraTools
{
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

        [MenuItem("Extra Tools/Ferramentas Extras")]
        public static void ShowWindow()
        {
            GetWindow<ExtraTools>("Ferramentas Extras");
        }

        #region Menu Items Diretos
        [MenuItem("Extra Tools/Debug/Liga/Desliga Logs")]
        public static void MenuToggleLogs()
        {
            ToggleLogs();
        }

        [MenuItem("Extra Tools/Debug/Exportar Estrutura da Cena")]
        public static void MenuExportSceneStructure()
        {
            ExportSceneStructure();
        }

        [MenuItem("Extra Tools/Post Processing/Setup Volume Global")]
        public static void MenuSetupGlobalVolume()
        {
            SetupGlobalVolumeInScene();
        }

        [MenuItem("Extra Tools/Post Processing/Setup Volume Floresta")]
        public static void MenuSetupForestVolume()
        {
            SetupBiomeVolume("ForestBiome_Volume");
        }

        [MenuItem("Extra Tools/Post Processing/Setup Volume Caverna")]
        public static void MenuSetupCaveVolume()
        {
            SetupBiomeVolume("CaveBiome_Volume");
        }

        [MenuItem("Extra Tools/Post Processing/Setup Volume Cristal")]
        public static void MenuSetupCrystalVolume()
        {
            SetupBiomeVolume("CrystalBiome_Volume");
        }

        [MenuItem("Extra Tools/Post Processing/Setup Efeitos de Gameplay")]
        public static void MenuSetupGameplayEffects()
        {
            SetupGameplayVolumeEffects();
        }
        #endregion

        private Vector2 scrollPosition;

        private void OnGUI()
        {
            GUILayout.Label("Ferramentas Extras - The Slime King", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(10);

            // Seção de Assets
            EditorGUILayout.LabelField("Gerenciamento de Assets", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Limpar Cache do Unity", GUILayout.Height(25)))
            {
                ClearUnityCache();
            }

            if (GUILayout.Button("Organizar Assets por Tipo", GUILayout.Height(25)))
            {
                OrganizeAssetsByType();
            }

            if (GUILayout.Button("Encontrar Assets Não Utilizados", GUILayout.Height(25)))
            {
                FindUnusedAssets();
            }

            GUILayout.Space(10);

            // Seção de Desenvolvimento
            EditorGUILayout.LabelField("Desenvolvimento", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Criar Script Template", GUILayout.Height(25)))
            {
                CreateScriptTemplate();
            }

            if (GUILayout.Button("Resetar PlayerPrefs", GUILayout.Height(25)))
            {
                ResetPlayerPrefs();
            }

            if (GUILayout.Button("Backup do Projeto", GUILayout.Height(25)))
            {
                CreateProjectBackup();
            }

            GUILayout.Space(10);

            // Seção de Debug e Logs
            EditorGUILayout.LabelField("Debug e Logs", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            // Status dos logs
            string logStatus = enableLogs ? "Desabilitar Logs" : "Habilitar Logs";
            if (GUILayout.Button(logStatus, GUILayout.Height(25)))
            {
                ToggleLogs();
            }

            if (GUILayout.Button("Exportar Estrutura da Cena", GUILayout.Height(25)))
            {
                ExportSceneStructure();
            }

            GUILayout.Space(10);

            // Seção de Post Processing
            EditorGUILayout.LabelField("Post Processing", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Setup Volume Global na Cena", GUILayout.Height(25)))
            {
                SetupGlobalVolumeInScene();
            }

            if (GUILayout.Button("Setup Volume Floresta", GUILayout.Height(25)))
            {
                SetupBiomeVolume("ForestBiome_Volume");
            }

            if (GUILayout.Button("Setup Volume Caverna", GUILayout.Height(25)))
            {
                SetupBiomeVolume("CaveBiome_Volume");
            }

            if (GUILayout.Button("Setup Volume Cristal", GUILayout.Height(25)))
            {
                SetupBiomeVolume("CrystalBiome_Volume");
            }

            if (GUILayout.Button("Setup Efeitos de Gameplay", GUILayout.Height(25)))
            {
                SetupGameplayVolumeEffects();
            }

            GUILayout.Space(10);

            // Seção de Debug
            EditorGUILayout.LabelField("Debug e Utilitários", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.Separator();

            if (GUILayout.Button("Mostrar Informações do Projeto", GUILayout.Height(25)))
            {
                ShowProjectInfo();
            }

            if (GUILayout.Button("Validar Configurações", GUILayout.Height(25)))
            {
                ValidateProjectSettings();
            }

            if (GUILayout.Button("Abrir Pasta de Logs", GUILayout.Height(25)))
            {
                OpenLogsFolder();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Ferramentas auxiliares para desenvolvimento do The Slime King", MessageType.Info);
        }

        private static void ClearUnityCache()
        {
            bool result = EditorUtility.DisplayDialog("Limpar Cache",
                "Isso irá limpar o cache do Unity e pode demorar um pouco para recompilar.\n\nContinuar?",
                "Sim", "Cancelar");

            if (result)
            {
                AssetDatabase.Refresh();

                // Limpar cache (removemos a linha problemática do ShaderUtil)
                EditorUtility.UnloadUnusedAssetsImmediate();

                Debug.Log("✅ Cache do Unity limpo!");
                EditorUtility.DisplayDialog("Cache Limpo", "Cache do Unity foi limpo com sucesso!", "OK");
            }
        }

        private static void OrganizeAssetsByType()
        {
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
            Debug.Log($"✅ {organized} assets organizados por tipo!");
        }

        private static string GetFolderForExtension(string extension)
        {
            switch (extension)
            {
                case ".png":
                case ".jpg":
                case ".jpeg":
                case ".psd":
                    return "Art/Sprites";
                case ".wav":
                case ".mp3":
                case ".ogg":
                    return "Audio";
                case ".cs":
                    return "Code";
                case ".prefab":
                    return "Game/Prefabs";
                case ".unity":
                    return "Game/Scenes";
                case ".mat":
                    return "Art/Materials";
                case ".anim":
                case ".controller":
                    return "Art/Animations";
                default:
                    return "";
            }
        }

        private static void FindUnusedAssets()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths()
                .Where(path => path.StartsWith("Assets/") && !AssetDatabase.IsValidFolder(path))
                .ToArray();

            string[] usedAssets = EditorUtility.CollectDependencies(
                AssetDatabase.LoadAllAssetsAtPath("Assets/Game/Scenes").ToArray())
                .Select(obj => AssetDatabase.GetAssetPath(obj))
                .Where(path => !string.IsNullOrEmpty(path))
                .ToArray();

            var unusedAssets = allAssets.Except(usedAssets).ToList();

            if (unusedAssets.Count > 0)
            {
                string message = $"Encontrados {unusedAssets.Count} assets não utilizados:\n\n";
                message += string.Join("\n", unusedAssets.Take(10));
                if (unusedAssets.Count > 10)
                {
                    message += $"\n... e mais {unusedAssets.Count - 10} assets.";
                }

                EditorUtility.DisplayDialog("Assets Não Utilizados", message, "OK");
                Debug.Log("Assets não utilizados encontrados - verificar console para lista completa");

                foreach (string asset in unusedAssets)
                {
                    Debug.Log($"Asset não utilizado: {asset}");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Assets", "Todos os assets estão sendo utilizados!", "OK");
            }
        }

        private static void CreateScriptTemplate()
        {
            string templatePath = "Assets/Code/ScriptTemplate.cs";
            string templateContent = @"using UnityEngine;

namespace SlimeKing
{
    public class NewScript : MonoBehaviour
    {
        [Header(""Configurações"")]
        [SerializeField] private float speed = 5f;

        private void Start()
        {
            // Inicialização
        }

        private void Update()
        {
            // Lógica por frame
        }

        private void OnValidate()
        {
            // Validação no editor
        }
    }
}";

            File.WriteAllText(templatePath, templateContent);
            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<MonoScript>(templatePath));
            Debug.Log("✅ Template de script criado!");
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
                Debug.Log("✅ PlayerPrefs resetados!");
                EditorUtility.DisplayDialog("PlayerPrefs", "PlayerPrefs foram resetados com sucesso!", "OK");
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

                    Debug.Log($"✅ Backup criado em: {backupFolder}");
                    EditorUtility.DisplayDialog("Backup", $"Backup criado com sucesso!\n\n{backupFolder}", "OK");
                    System.Diagnostics.Process.Start(backupFolder);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Erro ao criar backup: {e.Message}");
                    EditorUtility.DisplayDialog("Erro", $"Erro ao criar backup:\n{e.Message}", "OK");
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
            string info = $"Informações do Projeto - The Slime King\n\n";
            info += $"Unity Version: {Application.unityVersion}\n";
            info += $"Platform: {Application.platform}\n";
            info += $"Data Path: {Application.dataPath}\n";
            info += $"Persistent Data: {Application.persistentDataPath}\n";
            info += $"Temporary Cache: {Application.temporaryCachePath}\n\n";

            info += $"Configurações Atuais:\n";
            info += $"Screen Size: {Screen.width}x{Screen.height}\n";
            info += $"Target FPS: {Application.targetFrameRate}\n";
            info += $"VSync: {QualitySettings.vSyncCount}\n";
            info += $"Quality Level: {QualitySettings.GetQualityLevel()}\n";

            EditorUtility.DisplayDialog("Informações do Projeto", info, "OK");
        }

        private static void ValidateProjectSettings()
        {
            string validation = "Validação das Configurações:\n\n";
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

            // Verificar estrutura de pastas
            string[] requiredFolders = { "Assets/Art", "Assets/Audio", "Assets/Code", "Assets/Game", "Assets/Settings" };
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
                validation += "✅ Estrutura de pastas OK\n";
            }
            else
            {
                validation += "❌ Estrutura de pastas incompleta\n";
                allGood = false;
            }

            validation += "\n";
            if (allGood)
            {
                validation += "🎉 Projeto está bem configurado!";
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
                    EditorUtility.DisplayDialog("Logs", "Pasta de logs não encontrada.", "OK");
                }
            }
        }

        #region Funcionalidades Adicionais

        /// <summary>
        /// Alterna o estado dos logs do sistema
        /// </summary>
        private static void ToggleLogs()
        {
            enableLogs = !enableLogs;
            Log($"Logs {(enableLogs ? "habilitados" : "desabilitados")}");
            EditorUtility.DisplayDialog("Logs", $"Logs foram {(enableLogs ? "habilitados" : "desabilitados")}!", "OK");
        }

        /// <summary>
        /// Exporta a estrutura completa da cena ativa para análise
        /// </summary>
        private static void ExportSceneStructure()
        {
            Log("Exportando estrutura da cena ativa...");
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

            Log($"Estrutura da cena exportada para: {filePath}");
            EditorUtility.DisplayDialog("Exportação Concluída",
                $"Estrutura da cena '{scene.name}' exportada com sucesso!\n\nArquivo: {fileName}\nLocalização: {logsDir}", "OK");

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

        /// <summary>
        /// Configura Post Processing na cena ativa com Volume global
        /// </summary>
        private static void SetupGlobalVolumeInScene()
        {
            Log("Configurando Post Processing na cena ativa...");

            if (!ValidateURPSetup())
                return;

            // Verifica se já existe um Volume global na cena
            Volume existingVolume = Object.FindFirstObjectByType<Volume>();
            if (existingVolume != null && existingVolume.isGlobal)
            {
                Log("Volume global já existe na cena");
                EditorUtility.DisplayDialog("Post Processing", "Volume global já existe na cena!", "OK");
                return;
            }

            // Carrega o Global Volume Profile
            VolumeProfile globalProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(
                "Assets/Settings/PostProcessing/GlobalVolumeProfile.asset");

            if (globalProfile == null)
            {
                LogError("GlobalVolumeProfile.asset não encontrado! Crie o profile primeiro.");

                // Tenta criar um profile básico
                globalProfile = CreateBasicVolumeProfile("GlobalVolumeProfile");
                if (globalProfile == null)
                    return;
            }

            // Cria GameObject para o Volume
            GameObject volumeGO = new GameObject("Global Volume");
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

            Log("Volume global configurado com sucesso na cena!");
            EditorUtility.DisplayDialog("Post Processing Setup",
                "Volume global configurado com sucesso!\nO GameObject foi criado e selecionado na cena.", "OK");
        }

        /// <summary>
        /// Configura Volume específico para bioma
        /// </summary>
        private static void SetupBiomeVolume(string profileName)
        {
            if (!ValidateURPSetup())
                return;

            // Carrega o Volume Profile do bioma
            string profilePath = $"Assets/Settings/PostProcessing/Biomes/{profileName}.asset";
            VolumeProfile biomeProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);

            if (biomeProfile == null)
            {
                LogError($"Volume Profile '{profileName}' não encontrado em {profilePath}!");

                // Tenta criar um profile básico para o bioma
                biomeProfile = CreateBasicVolumeProfile(profileName, "Assets/Settings/PostProcessing/Biomes/");
                if (biomeProfile == null)
                    return;
            }

            // Cria GameObject para o Volume
            string biomeName = profileName.Replace("Biome_Volume", "").Replace("_", " ");
            GameObject volumeGO = new GameObject($"{biomeName} Volume");
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

            Log($"Volume '{biomeName}' configurado com sucesso!");
            EditorUtility.DisplayDialog("Biome Volume Setup",
                $"Volume '{biomeName}' configurado com sucesso!\n\nAjuste o Box Collider para delimitar a área do bioma.\nO Volume tem prioridade 1 (maior que global).", "OK");
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
                string message = "Efeitos de gameplay configurados:\n";
                if (hitCreated) message += "- Hit Effect Volume (prioridade 10)\n";
                if (evolutionCreated) message += "- Evolution Effect Volume (prioridade 15)\n";
                message += "\nOs volumes estão desabilitados por padrão.\nAtive via script quando necessário.";

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
            string profilePath = $"Assets/Settings/PostProcessing/Gameplay/{profileName}.asset";
            VolumeProfile effectProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(profilePath);

            if (effectProfile == null)
            {
                LogError($"Volume Profile '{profileName}' não encontrado em {profilePath}!");

                // Tenta criar um profile básico para o efeito
                effectProfile = CreateBasicVolumeProfile(profileName, "Assets/Settings/PostProcessing/Gameplay/");
                if (effectProfile == null)
                    return false;
            }

            // Cria GameObject para o Volume
            GameObject volumeGO = new GameObject($"{effectName} Volume");
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

            Log($"Volume '{effectName}' criado com sucesso!");
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
                    "Universal Render Pipeline não está configurado!\n\nPor favor, configure o URP antes de usar ferramentas de Post Processing.", "OK");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Cria um Volume Profile básico
        /// </summary>
        private static VolumeProfile CreateBasicVolumeProfile(string profileName, string folderPath = "Assets/Settings/PostProcessing/")
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

            Log($"Volume Profile básico criado: {fullPath}");
            return profile;
        }

        #endregion
    }
}