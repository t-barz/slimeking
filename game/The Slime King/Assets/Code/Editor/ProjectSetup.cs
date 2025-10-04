using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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

        #region Post Processing Tools
        /// <summary>
        /// Configura Post Processing na cena ativa com Volume global
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Setup Volume in Scene")]
        public static void SetupVolumeInScene()
        {
            Log("Configurando Post Processing na cena ativa...");
            SetupGlobalVolumeInScene();
        }

        /// <summary>
        /// Configura Volume específico para bioma Floresta
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Setup Forest Biome")]
        public static void SetupForestBiome()
        {
            Log("Configurando Volume para Floresta...");
            SetupBiomeVolume("ForestBiome_Volume");
        }

        /// <summary>
        /// Configura Volume específico para bioma Caverna
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Setup Cave Biome")]
        public static void SetupCaveBiome()
        {
            Log("Configurando Volume para Caverna...");
            SetupBiomeVolume("CaveBiome_Volume");
        }

        /// <summary>
        /// Configura Volume específico para bioma Cristal
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Setup Crystal Biome")]
        public static void SetupCrystalBiome()
        {
            Log("Configurando Volume para Cristal...");
            SetupBiomeVolume("CrystalBiome_Volume");
        }

        /// <summary>
        /// Configura efeitos de gameplay (Hit Effect e Evolution Effect)
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Setup Gameplay Effects")]
        public static void SetupGameplayEffects()
        {
            Log("Configurando efeitos de gameplay...");
            SetupGameplayVolumeEffects();
        }

        /// <summary>
        /// Configura câmera principal otimizada para pixel art com Post Processing
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Setup Pixel Perfect Camera")]
        public static void SetupPixelPerfectCamera()
        {
            Log("Configurando câmera para pixel art...");
            SetupOptimalPixelArtCamera();
        }

        /// <summary>
        /// Configura Global Light 2D otimizada para pixel art
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Setup Global Light 2D")]
        public static void SetupGlobalLight2D()
        {
            Log("Configurando Global Light 2D...");
            SetupOptimalGlobalLight();
        }

        /// <summary>
        /// Configuração completa: Câmera + Luz + Post Processing
        /// </summary>
        [MenuItem("Extra Tools/Post Processing/Complete Camera Setup")]
        public static void CompleteCameraSetup()
        {
            Log("Executando configuração completa de câmera...");
            SetupOptimalPixelArtCamera();
            SetupOptimalGlobalLight();
            SetupGlobalVolumeInScene();
            Log("Configuração completa de câmera finalizada!");
            EditorUtility.DisplayDialog("Camera Setup Completo",
                "Configuração completa finalizada:\n\n" +
                "✅ Pixel Perfect Camera configurada\n" +
                "✅ Global Light 2D otimizada\n" +
                "✅ Post Processing Volume aplicado\n" +
                "✅ Cinemachine Brain configurado", "OK");
        }
        #endregion
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

        #region Post Processing Implementation
        /// <summary>
        /// Configura Volume global na cena ativa
        /// </summary>
        private static void SetupGlobalVolumeInScene()
        {
            if (!ValidateURPSetup())
                return;

            // Verifica se já existe um Volume global na cena
            Volume existingVolume = Object.FindFirstObjectByType<Volume>();
            if (existingVolume != null && existingVolume.isGlobal)
            {
                Log("Volume global já existe na cena");
                return;
            }

            // Carrega o Global Volume Profile
            VolumeProfile globalProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(
                "Assets/Settings/PostProcessing/GlobalVolumeProfile.asset");

            if (globalProfile == null)
            {
                LogError("GlobalVolumeProfile.asset não encontrado! Execute 'Extra Tools/Project Setup/Complete Setup' primeiro.");
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
                return false;
            }

            // Cria GameObject para o Volume
            GameObject volumeGO = new GameObject($"{effectName} Volume");
            Volume volume = volumeGO.AddComponent<Volume>();

            // Configura o Volume
            volume.isGlobal = true;
            volume.priority = priority;
            volume.profile = effectProfile;
            volume.weight = 0f; // Inicia desabilitado

            // Desabilita o GameObject por padrão
            volumeGO.SetActive(false);

            // Adiciona tag para fácil identificação
            volumeGO.tag = "EditorOnly"; // Se existir, senão será "Untagged"

            // Registra para Undo
            Undo.RegisterCreatedObjectUndo(volumeGO, $"Create {effectName} Volume");

            Log($"Volume '{effectName}' criado com prioridade {priority}");
            return true;
        }

        /// <summary>
        /// Valida se o URP está configurado corretamente
        /// </summary>
        private static bool ValidateURPSetup()
        {
            // Verifica se o URP está ativo
            if (GraphicsSettings.defaultRenderPipeline == null)
            {
                LogError("Universal Render Pipeline não está configurado!\nVá em Project Settings > Graphics > Scriptable Render Pipeline Settings");
                EditorUtility.DisplayDialog("URP não configurado",
                    "Universal Render Pipeline não está ativo!\n\nPara usar Post Processing:\n1. Vá em Project Settings > Graphics\n2. Configure o Scriptable Render Pipeline Settings", "OK");
                return false;
            }

            // Verifica se é URP
            if (!(GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset))
            {
                LogError("Render Pipeline ativo não é URP!");
                EditorUtility.DisplayDialog("Render Pipeline Incorreto",
                    "O Render Pipeline ativo não é Universal Render Pipeline!\n\nEste sistema de Post Processing requer URP.", "OK");
                return false;
            }

            // Verifica se a pasta de Volume Profiles existe
            if (!AssetDatabase.IsValidFolder("Assets/Settings/PostProcessing"))
            {
                LogError("Pasta de Post Processing não encontrada! Execute 'Extra Tools/Project Setup/Complete Setup' primeiro.");
                EditorUtility.DisplayDialog("Setup Incompleto",
                    "Pasta Assets/Settings/PostProcessing não encontrada!\n\nExecute 'Extra Tools/Project Setup/Complete Setup' primeiro.", "OK");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Configura a câmera principal com componentes otimizados para pixel art
        /// </summary>
        private static void SetupOptimalPixelArtCamera()
        {
            if (!ValidateURPSetup())
                return;

            // Encontra ou cria Main Camera
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                GameObject cameraGO = new GameObject("Main Camera");
                cameraGO.tag = "MainCamera";
                mainCamera = cameraGO.AddComponent<Camera>();
                cameraGO.transform.position = new Vector3(0, 0, -10);

                Undo.RegisterCreatedObjectUndo(cameraGO, "Create Main Camera");
            }

            // Configurações básicas da câmera
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 5f; // Valor padrão, será sobrescrito pelo Pixel Perfect
            mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f); // Cinza escuro
            mainCamera.cullingMask = -1; // Renderiza todas as layers

            // Adiciona Pixel Perfect Camera (via reflection para evitar dependency issues)
            if (!mainCamera.TryGetComponent(out PixelPerfectCamera pixelPerfect))
            {
                try
                {
                    var pixelPerfectType = System.Type.GetType("UnityEngine.U2D.PixelPerfectCamera, Unity.2D.PixelPerfect");
                    if (pixelPerfectType != null)
                    {
                        pixelPerfect = mainCamera.gameObject.AddComponent(pixelPerfectType) as PixelPerfectCamera;
                    }
                }
                catch (System.Exception e)
                {
                    LogWarning($"Não foi possível adicionar PixelPerfectCamera: {e.Message}");
                }
            }

            // Configura Pixel Perfect Camera
            if (pixelPerfect != null)
            {
                var so = new SerializedObject(pixelPerfect);

                var assetsPPU = so.FindProperty("m_AssetsPPU");
                if (assetsPPU != null) assetsPPU.intValue = 16; // 16 pixels per unit (padrão pixel art)

                var refResX = so.FindProperty("m_RefResolutionX");
                if (refResX != null) refResX.intValue = 320; // Resolução referência 320x240 (estilo retro)

                var refResY = so.FindProperty("m_RefResolutionY");
                if (refResY != null) refResY.intValue = 240;

                var upscaleRT = so.FindProperty("m_UpscaleRT");
                if (upscaleRT != null) upscaleRT.boolValue = false; // Melhor performance

                var pixelSnapping = so.FindProperty("m_PixelSnapping");
                if (pixelSnapping != null) pixelSnapping.boolValue = true; // Evita pixels "borrados"

                var cropFrameX = so.FindProperty("m_CropFrameX");
                if (cropFrameX != null) cropFrameX.boolValue = false; // Permite letterboxing

                var cropFrameY = so.FindProperty("m_CropFrameY");
                if (cropFrameY != null) cropFrameY.boolValue = false;

                var stretchFill = so.FindProperty("m_StretchFill");
                if (stretchFill != null) stretchFill.boolValue = false; // Mantém aspect ratio

                so.ApplyModifiedProperties();
                Log("Pixel Perfect Camera configurado: 16 PPU, 320x240 referência");
            }

            // Adiciona Cinemachine Brain (via reflection)
            Component brain = mainCamera.GetComponent("CinemachineBrain");
            if (brain == null)
            {
                try
                {
                    var brainType = System.Type.GetType("Unity.Cinemachine.CinemachineBrain, Unity.Cinemachine");
                    if (brainType != null)
                    {
                        brain = mainCamera.gameObject.AddComponent(brainType);
                    }
                }
                catch (System.Exception e)
                {
                    LogWarning($"Não foi possível adicionar CinemachineBrain: {e.Message}");
                }
            }

            // Configura Cinemachine Brain
            if (brain != null)
            {
                var so = new SerializedObject(brain);

                // Blend settings otimizados para pixel art
                var blendTime = so.FindProperty("m_DefaultBlend.m_Time");
                if (blendTime != null) blendTime.floatValue = 1.0f; // Transições suaves

                var blendStyle = so.FindProperty("m_DefaultBlend.m_Style");
                if (blendStyle != null) blendStyle.intValue = 1; // EaseInOut

                var updateMethod = so.FindProperty("m_UpdateMethod");
                if (updateMethod != null) updateMethod.intValue = 1; // LateUpdate para sincronização

                var blendUpdateMethod = so.FindProperty("m_BlendUpdateMethod");
                if (blendUpdateMethod != null) blendUpdateMethod.intValue = 1; // LateUpdate

                so.ApplyModifiedProperties();
                Log("Cinemachine Brain configurado com blends suaves");
            }

            // Configura render pipeline específico para a câmera
            if (mainCamera.TryGetComponent(out UniversalAdditionalCameraData urpCameraData))
            {
                urpCameraData.renderType = CameraRenderType.Base;
                urpCameraData.cameraStack.Clear(); // Remove overlay cameras
                urpCameraData.renderPostProcessing = true; // Habilita Post Processing
                urpCameraData.antialiasing = AntialiasingMode.None; // Sem anti-aliasing para pixel art
                urpCameraData.antialiasingQuality = AntialiasingQuality.Low;

                Log("URP Camera Data configurado: Post Processing ON, Anti-aliasing OFF");
            }

            // Marca como modificado para salvar
            EditorUtility.SetDirty(mainCamera.gameObject);

            // Seleciona a câmera
            Selection.activeGameObject = mainCamera.gameObject;

            Log("Câmera pixel art configurada com sucesso!");
        }

        /// <summary>
        /// Configura Global Light 2D otimizada para pixel art
        /// </summary>
        private static void SetupOptimalGlobalLight()
        {
            // Procura por Global Light 2D existente
            Light2D globalLight = Object.FindFirstObjectByType<Light2D>();

            if (globalLight == null)
            {
                // Cria novo GameObject para Global Light 2D
                GameObject lightGO = new GameObject("Global Light 2D");
                globalLight = lightGO.AddComponent<Light2D>();

                Undo.RegisterCreatedObjectUndo(lightGO, "Create Global Light 2D");
                Log("Global Light 2D criado");
            }

            // Configura as propriedades da luz (via reflection para evitar dependency issues)
            var so = new SerializedObject(globalLight);

            // Configura como Global Light
            var lightTypeProperty = so.FindProperty("m_LightType");
            if (lightTypeProperty != null)
                lightTypeProperty.intValue = 3; // Global = 3

            // Intensidade otimizada para pixel art
            var intensityProperty = so.FindProperty("m_Intensity");
            if (intensityProperty != null)
                intensityProperty.floatValue = 1.0f; // Intensidade padrão

            // Cor ligeiramente quente para pixel art
            var colorProperty = so.FindProperty("m_Color");
            if (colorProperty != null)
                colorProperty.colorValue = new Color(1.0f, 0.95f, 0.9f, 1.0f); // Branco ligeiramente quente

            // Volume light settings para performance
            var volumeOpacityProperty = so.FindProperty("m_VolumeOpacity");
            if (volumeOpacityProperty != null)
                volumeOpacityProperty.floatValue = 0.0f; // Sem volume light para performance

            so.ApplyModifiedProperties();

            // Marca como modificado
            EditorUtility.SetDirty(globalLight.gameObject);

            Log("Global Light 2D configurado: Intensidade 1.0, cor quente, sem volume light");
        }
        #endregion
    }
}
