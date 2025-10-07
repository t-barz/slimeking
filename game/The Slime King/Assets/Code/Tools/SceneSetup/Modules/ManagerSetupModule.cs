using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// Módulo responsável por configurar e validar os Managers essenciais do jogo.
    /// Garante que GameManager, AudioManager e InputManager existam na cena.
    /// </summary>
    public static class ManagerSetupModule
    {
        /// <summary>
        /// Executa o setup completo dos managers baseado na configuração do SceneSetupManager
        /// </summary>
        /// <param name="setupManager">Referência ao SceneSetupManager com as configurações</param>
        public static void Setup(SceneSetupManager setupManager)
        {
            if (setupManager == null)
            {
                Debug.LogError("[ManagerSetup] SceneSetupManager é null");
                return;
            }

            Log(setupManager, "Iniciando setup dos managers...");

            if (setupManager.RequireGameManager)
            {
                EnsureGameManager(setupManager);
            }

            if (setupManager.RequireAudioManager)
            {
                EnsureAudioManager(setupManager);
            }

            if (setupManager.RequireInputManager)
            {
                EnsureInputManager(setupManager);
            }

            Log(setupManager, "Setup dos managers concluído");
        }

        /// <summary>
        /// Garante que o GameManager existe na cena
        /// </summary>
        private static void EnsureGameManager(SceneSetupManager setupManager)
        {
            if (GameManager.Instance == null)
            {
#if UNITY_EDITOR
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/System/GameManager.prefab");
                if (prefab != null)
                {
                    GameObject gameManagerGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
                    Log(setupManager, "GameManager instanciado a partir do prefab");
                }
                else
                {
                    Log(setupManager, "Prefab de GameManager não encontrado, criando vazio");
                    GameObject gameManagerGO = new GameObject("GameManager");
                    gameManagerGO.AddComponent<GameManager>();
                }
#else
                GameObject gameManagerGO = new GameObject("GameManager");
                gameManagerGO.AddComponent<GameManager>();
#endif
            }
            else
            {
                Log(setupManager, "GameManager já existe na cena");
            }
        }

        /// <summary>
        /// Garante que o AudioManager existe na cena
        /// </summary>
        private static void EnsureAudioManager(SceneSetupManager setupManager)
        {
            if (AudioManager.Instance == null)
            {
#if UNITY_EDITOR
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/System/AudioManager.prefab");
                if (prefab != null)
                {
                    GameObject audioManagerGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
                    var audioManager = audioManagerGO.GetComponent<AudioManager>();
                    if (audioManager == null)
                        audioManager = audioManagerGO.AddComponent<AudioManager>();
                    Log(setupManager, "AudioManager instanciado a partir do prefab");
                }
                else
                {
                    Log(setupManager, "Prefab de AudioManager não encontrado, criando vazio");
                    GameObject audioManagerGO = new GameObject("AudioManager");
                    var audioManager = audioManagerGO.AddComponent<AudioManager>();
                    if (audioManagerGO.GetComponent<AudioSource>() == null)
                        audioManagerGO.AddComponent<AudioSource>();
                }
#else
                GameObject audioManagerGO = new GameObject("AudioManager");
                var audioManager = audioManagerGO.AddComponent<AudioManager>();
                if (audioManagerGO.GetComponent<AudioSource>() == null)
                    audioManagerGO.AddComponent<AudioSource>();
#endif

                // Configurações básicas para diferentes tipos de cena
                ConfigureAudioManagerForSceneType(AudioManager.Instance, setupManager.SceneType);
                Log(setupManager, "AudioManager criado com configurações para " + setupManager.SceneType);
            }
            else
            {
                Log(setupManager, "AudioManager já existe na cena");
            }
        }

        /// <summary>
        /// Garante que o InputManager existe na cena
        /// </summary>
        private static void EnsureInputManager(SceneSetupManager setupManager)
        {
            if (InputManager.Instance == null)
            {
                #if UNITY_EDITOR
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/System/InputManager.prefab");
                if (prefab != null)
                {
                    GameObject inputManagerGO = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
                    var inputManager = inputManagerGO.GetComponent<InputManager>();
                    if (inputManager == null)
                        inputManager = inputManagerGO.AddComponent<InputManager>();
                    Log(setupManager, "InputManager instanciado a partir do prefab");
                }
                else
                {
                    Log(setupManager, "Prefab de InputManager não encontrado, criando vazio");
                    GameObject inputManagerGO = new GameObject("InputManager");
                    var inputManager = inputManagerGO.AddComponent<InputManager>();
                }
                #else
                GameObject inputManagerGO = new GameObject("InputManager");
                var inputManager = inputManagerGO.AddComponent<InputManager>();
                #endif

                // Configurações básicas para diferentes tipos de cena
                ConfigureInputManagerForSceneType(InputManager.Instance, setupManager.SceneType);
                Log(setupManager, "InputManager criado com configurações para " + setupManager.SceneType);
            }
            else
            {
                Log(setupManager, "InputManager já existe na cena");
            }
        }

        /// <summary>
        /// Configura o AudioManager baseado no tipo de cena
        /// </summary>
        private static void ConfigureAudioManagerForSceneType(AudioManager audioManager, SceneType sceneType)
        {
            // Configurações básicas que podem ser aplicadas aqui
            // Valores específicos podem ser ajustados no inspector posteriormente

            switch (sceneType)
            {
                case SceneType.Gameplay:
                    // Configurações para gameplay
                    // audioManager.MusicVolume = 0.7f;
                    // audioManager.SfxVolume = 0.8f;
                    break;

                case SceneType.Menu:
                    // Configurações para menus
                    // audioManager.MusicVolume = 0.8f;
                    // audioManager.SfxVolume = 0.6f;
                    break;

                case SceneType.Cutscene:
                    // Configurações para cutscenes
                    // audioManager.MusicVolume = 0.9f;
                    // audioManager.SfxVolume = 0.4f;
                    break;
            }
        }

        /// <summary>
        /// Configura o InputManager baseado no tipo de cena
        /// </summary>
        private static void ConfigureInputManagerForSceneType(InputManager inputManager, SceneType sceneType)
        {
            // Configurações específicas do InputManager podem ser aplicadas aqui
            // Por exemplo, diferentes contextos de input para diferentes tipos de cena

            switch (sceneType)
            {
                case SceneType.Gameplay:
                    // Para gameplay, todos os inputs são necessários
                    // inputManager.EnableGameplay();
                    break;

                case SceneType.Menu:
                    // Para menus, apenas UI e system inputs
                    // inputManager.SetTitleScreenContext();
                    break;

                case SceneType.Cutscene:
                    // Para cutscenes, apenas system inputs (skip)
                    // inputManager.SetCutsceneContext();
                    break;
            }
        }

        /// <summary>
        /// Valida se todos os managers necessários estão presentes na cena
        /// </summary>
        /// <param name="setupManager">Referência ao SceneSetupManager com as configurações</param>
        /// <returns>True se todos os managers necessários estão presentes</returns>
        public static bool ValidateManagers(SceneSetupManager setupManager)
        {
            bool isValid = true;

            if (setupManager.RequireGameManager && GameManager.Instance == null)
            {
                LogWarning(setupManager, "GameManager não encontrado na cena");
                isValid = false;
            }

            if (setupManager.RequireAudioManager && AudioManager.Instance == null)
            {
                LogWarning(setupManager, "AudioManager não encontrado na cena");
                isValid = false;
            }

            if (setupManager.RequireInputManager && InputManager.Instance == null)
            {
                LogWarning(setupManager, "InputManager não encontrado na cena");
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Remove managers da cena (útil para cleanup em testes)
        /// </summary>
        public static void CleanupManagers()
        {
            if (GameManager.Instance != null)
            {
                Object.DestroyImmediate(GameManager.Instance.gameObject);
            }

            if (AudioManager.Instance != null)
            {
                Object.DestroyImmediate(AudioManager.Instance.gameObject);
            }

            if (InputManager.Instance != null)
            {
                Object.DestroyImmediate(InputManager.Instance.gameObject);
            }
        }

        #region Logging Helpers
        private static void Log(SceneSetupManager setupManager, string message)
        {
            if (setupManager.VerboseLogging)
            {
                Debug.Log($"[ManagerSetup] {message}");
            }
        }

        private static void LogWarning(SceneSetupManager setupManager, string message)
        {
            if (setupManager.VerboseLogging)
            {
                Debug.LogWarning($"[ManagerSetup] {message}");
            }
        }

        private static void LogError(string message)
        {
            Debug.LogError($"[ManagerSetup] {message}");
        }
        #endregion
    }
}