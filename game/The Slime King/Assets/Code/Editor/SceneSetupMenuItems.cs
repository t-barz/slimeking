using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace ExtraTools
{
    /// <summary>
    /// Menu items para o Scene Setup System integrado ao Extra Tools.
    /// Fornece acesso rápido às funcionalidades de configuração de cena.
    /// </summary>
    public static class SceneSetupMenuItems
    {
        #region Scene Setup Menu Items

        /// <summary>
        /// Executa o setup da cena atual
        /// </summary>
        [MenuItem("Extra Tools/Scene Setup/Setup Current Scene", false, 100)]
        public static void SetupCurrentScene()
        {
            var setupManager = FindFirstSceneSetupManager();

            if (setupManager == null)
            {
                if (EditorUtility.DisplayDialog("Scene Setup Manager",
                    "Nenhum SceneSetupManager encontrado na cena atual.\n\nDeseja criar um?",
                    "Sim", "Cancelar"))
                {
                    AddSceneSetupManager();
                    setupManager = FindFirstSceneSetupManager();
                }
                else
                {
                    return;
                }
            }

            if (setupManager != null)
            {
                setupManager.SetupScene();
                Debug.Log($"[SceneSetup] Setup executado na cena '{SceneManager.GetActiveScene().name}'");

                // Marcar a cena como modificada para salvar as mudanças
                EditorUtility.SetDirty(setupManager);
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// Força o setup da cena atual mesmo se já foi executado
        /// </summary>
        [MenuItem("Extra Tools/Scene Setup/Force Setup Current Scene", false, 101)]
        public static void ForceSetupCurrentScene()
        {
            var setupManager = FindFirstSceneSetupManager();

            if (setupManager == null)
            {
                Debug.LogWarning("[SceneSetup] Nenhum SceneSetupManager encontrado na cena atual");
                return;
            }

            setupManager.ForceSetup();
            Debug.Log($"[SceneSetup] Force setup executado na cena '{SceneManager.GetActiveScene().name}'");

            // Marcar a cena como modificada
            EditorUtility.SetDirty(setupManager);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Valida a configuração da cena atual
        /// </summary>
        [MenuItem("Extra Tools/Scene Setup/Validate Current Scene", false, 102)]
        public static void ValidateCurrentScene()
        {
            var setupManager = FindFirstSceneSetupManager();

            if (setupManager == null)
            {
                Debug.LogWarning("[SceneSetup] Nenhum SceneSetupManager encontrado na cena atual");
                return;
            }

            setupManager.ValidateScene();
        }

        /// <summary>
        /// Valida todas as cenas do projeto
        /// </summary>
        [MenuItem("Extra Tools/Scene Setup/Validate All Scenes", false, 103)]
        public static void ValidateAllScenes()
        {
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
            int totalScenes = sceneGuids.Length;
            int validScenes = 0;
            int invalidScenes = 0;
            int scenesWithoutSetup = 0;

            Debug.Log($"[SceneSetup] Iniciando validação de {totalScenes} cenas...");

            for (int i = 0; i < sceneGuids.Length; i++)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                // Mostrar progresso
                if (EditorUtility.DisplayCancelableProgressBar("Validando Cenas",
                    $"Validando: {sceneName}", (float)i / totalScenes))
                {
                    break;
                }

                // Não validar a cena atual (pode ter mudanças não salvas)
                if (scenePath == SceneManager.GetActiveScene().path)
                {
                    Debug.Log($"[SceneSetup] Pulando cena atual: {sceneName}");
                    continue;
                }

                // Carregar cena de forma assíncrona
                var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath,
                    UnityEditor.SceneManagement.OpenSceneMode.Additive);

                if (scene.IsValid())
                {
                    // Procurar SceneSetupManager na cena
                    GameObject[] rootObjects = scene.GetRootGameObjects();
                    SceneSetupManager setupManager = null;

                    foreach (var rootObject in rootObjects)
                    {
                        setupManager = rootObject.GetComponentInChildren<SceneSetupManager>();
                        if (setupManager != null) break;
                    }

                    if (setupManager != null)
                    {
                        setupManager.ValidateScene();
                        validScenes++;
                    }
                    else
                    {
                        Debug.LogWarning($"[SceneSetup] Cena '{sceneName}' não possui SceneSetupManager");
                        scenesWithoutSetup++;
                    }

                    // Fechar a cena
                    UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
                }
                else
                {
                    Debug.LogError($"[SceneSetup] Não foi possível carregar a cena: {sceneName}");
                    invalidScenes++;
                }
            }

            EditorUtility.ClearProgressBar();

            // Relatório final
            Debug.Log($"[SceneSetup] Validação concluída:\n" +
                     $"• Total de cenas: {totalScenes}\n" +
                     $"• Cenas com setup: {validScenes}\n" +
                     $"• Cenas sem setup: {scenesWithoutSetup}\n" +
                     $"• Cenas com erro: {invalidScenes}");

            if (scenesWithoutSetup > 0)
            {
                if (EditorUtility.DisplayDialog("Scene Setup Validation",
                    $"Encontradas {scenesWithoutSetup} cenas sem SceneSetupManager.\n\n" +
                    "Deseja abrir uma janela para gerenciar o setup das cenas?",
                    "Sim", "Não"))
                {
                    // TODO: Abrir SceneSetupWindow quando implementado na Fase 3
                    Debug.Log("[SceneSetup] SceneSetupWindow será implementado na Fase 3");
                }
            }
        }

        #endregion

        #region GameObject Menu Items

        /// <summary>
        /// Adiciona um SceneSetupManager à cena atual
        /// </summary>
        [MenuItem("GameObject/Extra Tools/Add Scene Setup Manager", false, 0)]
        public static void AddSceneSetupManager()
        {
            // Verificar se já existe um SceneSetupManager na cena
            var existingManager = FindFirstSceneSetupManager();
            if (existingManager != null)
            {
                if (EditorUtility.DisplayDialog("Scene Setup Manager",
                    "Já existe um SceneSetupManager na cena.\n\nDeseja selecionar o existente?",
                    "Sim", "Cancelar"))
                {
                    Selection.activeGameObject = existingManager.gameObject;
                    EditorGUIUtility.PingObject(existingManager.gameObject);
                }
                return;
            }

            // Criar novo SceneSetupManager
            GameObject setupManagerGO = new GameObject("SceneSetupManager");
            var setupManager = setupManagerGO.AddComponent<SceneSetupManager>();

            // Selecionar o objeto criado
            Selection.activeGameObject = setupManagerGO;

            // Executar Undo para permitir desfazer a criação
            Undo.RegisterCreatedObjectUndo(setupManagerGO, "Create Scene Setup Manager");

            Debug.Log("[SceneSetup] SceneSetupManager criado na cena");

            // Perguntar se quer executar o setup imediatamente
            if (EditorUtility.DisplayDialog("Scene Setup Manager",
                "SceneSetupManager criado com sucesso!\n\nDeseja executar o setup da cena agora?",
                "Sim", "Não"))
            {
                setupManager.SetupScene();
            }
        }

        /// <summary>
        /// Remove o SceneSetupManager da cena atual
        /// </summary>
        [MenuItem("GameObject/Extra Tools/Remove Scene Setup Manager", false, 1)]
        public static void RemoveSceneSetupManager()
        {
            var setupManager = FindFirstSceneSetupManager();

            if (setupManager == null)
            {
                Debug.LogWarning("[SceneSetup] Nenhum SceneSetupManager encontrado na cena atual");
                return;
            }

            if (EditorUtility.DisplayDialog("Remove Scene Setup Manager",
                "Tem certeza que deseja remover o SceneSetupManager?\n\n" +
                "Esta ação não pode ser desfeita.",
                "Sim", "Cancelar"))
            {
                Undo.DestroyObjectImmediate(setupManager.gameObject);
                Debug.Log("[SceneSetup] SceneSetupManager removido da cena");
            }
        }

        #endregion

        #region Validation Menu Items

        /// <summary>
        /// Valida se o menu item Setup Current Scene deve estar habilitado
        /// </summary>
        [MenuItem("Extra Tools/Scene Setup/Setup Current Scene", true)]
        public static bool ValidateSetupCurrentScene()
        {
            return !Application.isPlaying;
        }

        /// <summary>
        /// Valida se o menu item Force Setup Current Scene deve estar habilitado
        /// </summary>
        [MenuItem("Extra Tools/Scene Setup/Force Setup Current Scene", true)]
        public static bool ValidateForceSetupCurrentScene()
        {
            return !Application.isPlaying && FindFirstSceneSetupManager() != null;
        }

        /// <summary>
        /// Valida se o menu item Validate Current Scene deve estar habilitado
        /// </summary>
        [MenuItem("Extra Tools/Scene Setup/Validate Current Scene", true)]
        public static bool ValidateValidateCurrentScene()
        {
            return FindFirstSceneSetupManager() != null;
        }

        /// <summary>
        /// Valida se o menu item Remove Scene Setup Manager deve estar habilitado
        /// </summary>
        [MenuItem("GameObject/Extra Tools/Remove Scene Setup Manager", true)]
        public static bool ValidateRemoveSceneSetupManager()
        {
            return !Application.isPlaying && FindFirstSceneSetupManager() != null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Encontra o primeiro SceneSetupManager na cena atual
        /// </summary>
        /// <returns>SceneSetupManager encontrado ou null se não houver</returns>
        private static SceneSetupManager FindFirstSceneSetupManager()
        {
            return Object.FindFirstObjectByType<SceneSetupManager>();
        }

        #endregion
    }
}