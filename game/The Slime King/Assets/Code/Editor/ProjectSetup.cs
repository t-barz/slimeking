using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace ExtraTools
{
    /// <summary>
    /// Ferramenta para configuração automatizada de projetos Unity 2D
    /// Cria estrutura de pastas simplificada e aplica otimizações para PixelArt
    /// </summary>
    public class ProjectSetup : EditorWindow
    {
        #region Debug & Logs
        [SerializeField] private static bool enableLogs = true;
        [SerializeField] private static bool enableDebug = false;

        private static void Log(string message)
        {
            if (enableLogs)
                Debug.Log($"[ProjectSetup] {message}");
        }

        private static void LogWarning(string message)
        {
            if (enableLogs)
                Debug.LogWarning($"[ProjectSetup] {message}");
        }

        private static void LogError(string message)
        {
            if (enableLogs)
                Debug.LogError($"[ProjectSetup] {message}");
        }

        private static void DebugLog(string message)
        {
            if (enableDebug)
                Debug.Log($"[ProjectSetup DEBUG] {message}");
        }
        #endregion

        #region Menu Items
        /// <summary>
        /// Menu principal Extra Tools no Unity Editor
        /// </summary>
        [MenuItem("Extra Tools/Project Setup/Create Folder Structure")]
        public static void CreateFolderStructure()
        {
            Log("Iniciando criação da estrutura de pastas...");
            CreateProjectStructure();
        }

        [MenuItem("Extra Tools/Project Setup/Apply PixelArt Settings")]
        public static void ApplyPixelArtSettings()
        {
            Log("Aplicando configurações PixelArt...");
            ConfigurePixelArtSettings();
        }

        [MenuItem("Extra Tools/Project Setup/Reorganize Assets")]
        public static void ReorganizeAssets()
        {
            Log("Iniciando reorganização automática de assets...");
            ReorganizeExistingAssets();
        }

        [MenuItem("Extra Tools/Project Setup/Complete Setup")]
        public static void CompleteSetup()
        {
            Log("Executando setup completo do projeto...");
            CreateProjectStructure();
            ConfigurePixelArtSettings();
            ReorganizeExistingAssets();
            Log("Setup completo finalizado!");
            EditorUtility.DisplayDialog("Concluído", "Setup completo do projeto finalizado com sucesso!", "OK");
        }

        [MenuItem("Extra Tools/Debug/Toggle Logs")]
        public static void ToggleLogs()
        {
            enableLogs = !enableLogs;
            Log($"Logs {(enableLogs ? "habilitados" : "desabilitados")}");
        }

        [MenuItem("Extra Tools/Debug/Toggle Debug")]
        public static void ToggleDebug()
        {
            enableDebug = !enableDebug;
            DebugLog($"Debug {(enableDebug ? "habilitado" : "desabilitado")}");
        }
        #endregion

        #region Folder Structure Creation
        /// <summary>
        /// Cria a estrutura simplificada de pastas conforme documentação
        /// </summary>
        private static void CreateProjectStructure()
        {
            // Estrutura simplificada para projetos 2D
            Dictionary<string, string[]> folderStructure = new Dictionary<string, string[]>
            {
                { "Art", new string[] { "Sprites", "Materials", "Animations" } },
                { "Art/Animations", new string[] { "Controllers", "Clips" } },
                { "Audio", new string[] { "Music", "SFX" } },
                { "Code", new string[] { "Editor", "Gameplay", "Systems" } },
                { "Game", new string[] { "Scenes", "Prefabs", "Data" } },
                { "External", new string[] { "AssetStore", "Plugins", "Libraries", "Tools" } }
            };

            int foldersCreated = 0;

            // Cria as pastas
            foreach (var folder in folderStructure)
            {
                string mainFolder = "Assets/" + folder.Key;

                // Cria a pasta principal se não existir
                if (!AssetDatabase.IsValidFolder(mainFolder))
                {
                    CreateFolder(mainFolder);
                    foldersCreated++;
                    DebugLog($"Pasta principal criada: {mainFolder}");
                }

                // Cria as subpastas
                foreach (var subFolder in folder.Value)
                {
                    string subFolderPath = mainFolder + "/" + subFolder;
                    if (!AssetDatabase.IsValidFolder(subFolderPath))
                    {
                        CreateFolder(subFolderPath);
                        foldersCreated++;
                        DebugLog($"Subpasta criada: {subFolderPath}");
                    }
                }
            }

            AssetDatabase.Refresh();

            if (foldersCreated > 0)
            {
                Log($"Estrutura criada com sucesso! {foldersCreated} pastas criadas.");
                EditorUtility.DisplayDialog("Concluído",
                    $"Estrutura simplificada criada com sucesso!\n{foldersCreated} pastas criadas.", "OK");
            }
            else
            {
                Log("Estrutura já existe. Nenhuma pasta foi criada.");
                EditorUtility.DisplayDialog("Informação",
                    "Estrutura de pastas já existe. Nenhuma alteração foi necessária.", "OK");
            }
        }

        /// <summary>
        /// Cria uma pasta no projeto Unity
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
                    AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                    DebugLog($"Pasta criada: {newPath}");
                }

                currentPath = newPath;
            }
        }
        #endregion

        #region Asset Reorganization
        /// <summary>
        /// Reorganiza assets existentes para a nova estrutura
        /// </summary>
        private static void ReorganizeExistingAssets()
        {
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            int movedAssets = 0;

            foreach (string assetPath in allAssets)
            {
                // Pula pastas do sistema Unity
                if (assetPath.StartsWith("Packages/") ||
                    assetPath.StartsWith("Library/") ||
                    assetPath.StartsWith("ProjectSettings/") ||
                    assetPath.StartsWith("UserSettings/"))
                    continue;

                // Pula se já está na estrutura correta
                if (IsInCorrectFolder(assetPath))
                    continue;

                string newPath = GetNewPathForAsset(assetPath);

                if (!string.IsNullOrEmpty(newPath) && newPath != assetPath)
                {
                    string result = AssetDatabase.MoveAsset(assetPath, newPath);
                    if (string.IsNullOrEmpty(result))
                    {
                        movedAssets++;
                        DebugLog($"Asset movido: {assetPath} → {newPath}");
                    }
                    else
                    {
                        LogWarning($"Erro ao mover {assetPath}: {result}");
                    }
                }
            }

            AssetDatabase.Refresh();
            Log($"Reorganização concluída! {movedAssets} assets reorganizados.");

            if (movedAssets > 0)
            {
                EditorUtility.DisplayDialog("Reorganização Concluída",
                    $"{movedAssets} assets foram reorganizados para a nova estrutura.", "OK");
            }
        }

        /// <summary>
        /// Verifica se o asset já está na pasta correta
        /// </summary>
        private static bool IsInCorrectFolder(string assetPath)
        {
            string[] correctFolders = {
                "Assets/Art/", "Assets/Audio/", "Assets/Code/",
                "Assets/Game/", "Assets/External/"
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
                return "Assets/Game/Scenes/" + fileName;
            }

            // Scripts - mantém estrutura existente se já organizada
            if (extension == ".cs")
            {
                if (assetPath.Contains("Editor") || fileName.Contains("Editor"))
                    return "Assets/Code/Editor/" + fileName;
                else if (fileName.Contains("Manager") || fileName.Contains("System"))
                    return "Assets/Code/Systems/" + fileName;
                else
                    return "Assets/Code/Gameplay/" + fileName;
            }

            // Imagens e Sprites
            if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" ||
                extension == ".tga" || extension == ".psd" || extension == ".svg")
            {
                return "Assets/Art/Sprites/" + fileName;
            }

            // Áudio
            if (extension == ".wav" || extension == ".mp3" || extension == ".ogg")
            {
                if (fileName.ToLower().Contains("music") || fileName.ToLower().Contains("bgm") ||
                    fileName.ToLower().Contains("mus_"))
                    return "Assets/Audio/Music/" + fileName;
                else
                    return "Assets/Audio/SFX/" + fileName;
            }

            // Materiais
            if (extension == ".mat")
            {
                return "Assets/Art/Materials/" + fileName;
            }

            // Prefabs
            if (extension == ".prefab")
            {
                return "Assets/Game/Prefabs/" + fileName;
            }

            // Animações
            if (extension == ".anim")
            {
                return "Assets/Art/Animations/Clips/" + fileName;
            }

            // Controllers de Animação
            if (extension == ".controller")
            {
                return "Assets/Art/Animations/Controllers/" + fileName;
            }

            // Fontes e dados
            if (extension == ".ttf" || extension == ".otf" || extension == ".asset")
            {
                return "Assets/Game/Data/" + fileName;
            }

            // Arquivos não reconhecidos vão para External
            if (assetPath.StartsWith("Assets/") && !IsInCorrectFolder(assetPath))
            {
                return "Assets/External/" + fileName;
            }

            return "";
        }
        #endregion

        #region Prefix System
        /// <summary>
        /// Retorna o prefixo apropriado baseado no caminho do asset
        /// </summary>
        private static string GetPrefixForPath(string assetPath)
        {
            Dictionary<string, string> folderPrefixes = new Dictionary<string, string>
            {
                // Art
                { "Assets/Art/Sprites", "spr" },
                { "Assets/Art/Materials", "mat" },
                { "Assets/Art/Animations/Controllers", "ctrl" },
                { "Assets/Art/Animations/Clips", "anim" },
                { "Assets/Art/Animations", "anim" },

                // Audio
                { "Assets/Audio/Music", "mus" },
                { "Assets/Audio/SFX", "sfx" },

                // Game
                { "Assets/Game/Scenes", "scn" },
                { "Assets/Game/Prefabs", "prf" },
                { "Assets/Game/Data", "data" },

                // External
                { "Assets/External", "ext" }
            };

            // Procura o prefixo mais específico
            string bestMatch = "";
            string bestPrefix = "";

            foreach (var kvp in folderPrefixes)
            {
                if (assetPath.StartsWith(kvp.Key + "/") && kvp.Key.Length > bestMatch.Length)
                {
                    bestMatch = kvp.Key;
                    bestPrefix = kvp.Value;
                }
            }

            return bestPrefix ?? "gen";
        }
        #endregion

        #region PixelArt Configuration
        /// <summary>
        /// Aplica configurações otimizadas para jogos PixelArt
        /// </summary>
        private static void ConfigurePixelArtSettings()
        {
            // Configurações de Qualidade
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.vSyncCount = 1;

            DebugLog("Configurações de qualidade aplicadas");

            // Configurações de Physics2D
            Physics2D.velocityIterations = 6;
            Physics2D.positionIterations = 2;
            Physics2D.gravity = new Vector2(0, -9.81f);

            DebugLog("Configurações de Physics2D aplicadas");

            // Configurações de renderização para URP (se disponível)
            ConfigureURPSettings();

            Log("Configurações PixelArt aplicadas com sucesso!");
            EditorUtility.DisplayDialog("PixelArt Settings",
                "Configurações otimizadas para PixelArt aplicadas com sucesso!", "OK");
        }

        /// <summary>
        /// Configura settings específicos do Universal Render Pipeline para PixelArt
        /// </summary>
        private static void ConfigureURPSettings()
        {
            try
            {
                // Busca pelo asset do URP
                string[] urpAssets = AssetDatabase.FindAssets("t:UniversalRenderPipelineAsset");

                if (urpAssets.Length > 0)
                {
                    foreach (string guid in urpAssets)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        var urpAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset>(path);

                        if (urpAsset != null)
                        {
                            // Aplicar configurações via SerializedObject para acessar propriedades privadas
                            SerializedObject serializedUrp = new SerializedObject(urpAsset);

                            // Desabilitar anti-aliasing
                            SerializedProperty msaaSampleCount = serializedUrp.FindProperty("m_MSAA");
                            if (msaaSampleCount != null)
                                msaaSampleCount.intValue = 1; // 1 = No Anti-aliasing

                            serializedUrp.ApplyModifiedProperties();
                            EditorUtility.SetDirty(urpAsset);

                            DebugLog($"URP Asset configurado: {path}");
                        }
                    }

                    AssetDatabase.SaveAssets();
                    Log("Configurações URP aplicadas para PixelArt");
                }
                else
                {
                    DebugLog("Nenhum asset URP encontrado");
                }
            }
            catch (System.Exception e)
            {
                LogWarning($"Erro ao configurar URP: {e.Message}");
            }
        }
        #endregion
    }
}
