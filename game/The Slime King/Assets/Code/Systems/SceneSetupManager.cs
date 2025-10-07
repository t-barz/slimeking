using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExtraTools
{
    /// <summary>
    /// Scene Setup Manager - Automatiza a configuração de cenas com os sistemas essenciais.
    /// Detecta o tipo de cena automaticamente e configura os componentes necessários.
    /// </summary>
    public class SceneSetupManager : MonoBehaviour
    {
        #region Scene Configuration
        [Header("Scene Configuration")]
        [SerializeField] private SceneType sceneType = SceneType.Auto;
        [Tooltip("Executa setup automaticamente no Awake")]
        [SerializeField] private bool setupOnAwake = true;
        [Tooltip("Mostra logs detalhados do processo de setup")]
        [SerializeField] private bool verboseLogging = true;
        #endregion

        #region Required Systems
        [Header("Required Systems")]
        [Tooltip("Garante que GameManager existe na cena")]
        [SerializeField] private bool requireGameManager = true;
        [Tooltip("Garante que AudioManager existe na cena")]
        [SerializeField] private bool requireAudioManager = true;
        [Tooltip("Garante que InputManager existe na cena")]
        [SerializeField] private bool requireInputManager = true;
        [Tooltip("Garante que EventSystem existe na cena")]
        [SerializeField] private bool requireEventSystem = true;
        #endregion

        // Camera setup removido conforme solicitação (sem configuração automática de câmera)

        #region Lighting & Post-Process
        [Header("Lighting & Post-Process")]
        [Tooltip("Garante que Global Volume existe na cena")]
        [SerializeField] private bool requireGlobalVolume = true;
        [Tooltip("Adiciona Global Light 2D para cenas 2D")]
        [SerializeField] private bool requireGlobalLight2D = false;
        #endregion

        #region Runtime Variables
        private bool setupCompleted = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            if (setupOnAwake && !setupCompleted)
            {
                StartCoroutine(SetupSceneDelayed());
            }
        }

        private IEnumerator SetupSceneDelayed()
        {
            yield return null; // Aguarda um frame para garantir que outros Awake executem primeiro
            SetupScene();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Executa o setup completo da cena baseado nas configurações
        /// </summary>
        [ContextMenu("Setup Scene")]
        public void SetupScene()
        {
            if (setupCompleted)
            {
                LogWarning("Setup já foi executado nesta cena. Use 'Force Setup' se necessário.");
                return;
            }

            Log("Iniciando setup da cena...");

            // Detectar tipo de cena se for Auto
            if (sceneType == SceneType.Auto)
            {
                sceneType = DetectSceneType();
                Log($"Tipo de cena detectado automaticamente: {sceneType}");
            }

            // Aplicar configurações baseadas no tipo de cena
            ApplySceneTypeConfiguration();

            // Executar módulos de setup (Fase 2 - Módulos Especializados)
            SetupManagers();
            SetupInput();
            SetupPostProcessing();
            SetupLighting();

            setupCompleted = true;
            Log($"Setup da cena '{gameObject.scene.name}' concluído como {sceneType}");
        }

        /// <summary>
        /// Força o setup mesmo se já foi executado
        /// </summary>
        [ContextMenu("Force Setup")]
        public void ForceSetup()
        {
            setupCompleted = false;
            SetupScene();
        }

        /// <summary>
        /// Valida se a cena está configurada corretamente
        /// </summary>
        [ContextMenu("Validate Scene")]
        public void ValidateScene()
        {
            Log("Validando configuração da cena...");

            int issues = 0;

            // Validar managers
            if (requireGameManager && GameManager.Instance == null)
            {
                LogWarning("GameManager não encontrado na cena");
                issues++;
            }

            if (requireAudioManager && AudioManager.Instance == null)
            {
                LogWarning("AudioManager não encontrado na cena");
                issues++;
            }

            if (requireInputManager && InputManager.Instance == null)
            {
                LogWarning("InputManager não encontrado na cena");
                issues++;
            }

            // Validar EventSystem
            if (requireEventSystem && FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                LogWarning("EventSystem não encontrado na cena");
                issues++;
            }

            // Validar câmera
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                LogWarning("Main Camera não encontrada na cena");
                issues++;
            }

            if (issues == 0)
            {
                Log("✅ Cena validada com sucesso - todas as configurações estão corretas");
            }
            else
            {
                LogWarning($"⚠️ Encontrados {issues} problemas na configuração da cena");
            }
        }
        #endregion

        #region Private Methods
        private SceneType DetectSceneType()
        {
            string sceneName = gameObject.scene.name.ToLower();

            if (sceneName.Contains("title") || sceneName.Contains("menu") || sceneName.Contains("main"))
            {
                return SceneType.Menu;
            }
            else if (sceneName.Contains("cutscene") || sceneName.Contains("intro") || sceneName.Contains("ending"))
            {
                return SceneType.Cutscene;
            }
            else
            {
                return SceneType.Gameplay;
            }
        }

        private void ApplySceneTypeConfiguration()
        {
            switch (sceneType)
            {
                case SceneType.Gameplay:
                    requireGlobalLight2D = true;
                    requireGlobalVolume = true;
                    break;
                case SceneType.Menu:
                    requireGlobalLight2D = false;
                    requireGlobalVolume = true;
                    break;
                case SceneType.Cutscene:
                    requireGlobalLight2D = false;
                    requireGlobalVolume = true;
                    break;
            }

            Log($"Configurações aplicadas para tipo de cena: {sceneType}");
        }

        private void SetupManagers()
        {
            Log("Configurando managers...");
            ManagerSetupModule.Setup(this);
        }

        // Método de setup de câmera removido

        private void SetupInput()
        {
            Log("Configurando sistema de input avançado...");
            // TODO: Habilitar após compilação: InputSetupModule.Setup(this);

            // Implementação temporária básica
            if (requireEventSystem)
            {
                var eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
                if (eventSystem == null)
                {
                    GameObject eventSystemGO = new GameObject("EventSystem");
                    eventSystemGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    eventSystemGO.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    Log("EventSystem criado com InputSystemUIInputModule");
                }
            }
        }

        private void SetupPostProcessing()
        {
            Log("Configurando post processing avançado...");
            // TODO: Habilitar após compilação: PostProcessSetupModule.Setup(this);

            // Implementação temporária básica
            if (requireGlobalVolume)
            {
                var globalVolume = FindFirstObjectByType<UnityEngine.Rendering.Volume>();
                if (globalVolume == null)
                {
                    GameObject volumeGO = new GameObject("Global Volume");
                    var volume = volumeGO.AddComponent<UnityEngine.Rendering.Volume>();
                    volume.isGlobal = true;
                    Log("Global Volume criado");
                }
            }
        }

        private void SetupLighting()
        {
            Log("Configurando iluminação avançada...");
            // TODO: Habilitar após compilação: LightingSetupModule.Setup(this);

            // Implementação temporária básica
            if (requireGlobalLight2D && sceneType == SceneType.Gameplay)
            {
#if UNITY_2D_RENDERER
                var globalLight = FindFirstObjectByType<UnityEngine.Rendering.Universal.Light2D>();
                if (globalLight == null)
                {
                    GameObject lightGO = new GameObject("Global Light 2D");
                    var light = lightGO.AddComponent<UnityEngine.Rendering.Universal.Light2D>();
                    light.lightType = UnityEngine.Rendering.Universal.Light2D.LightType.Global;
                    light.intensity = 1f;
                    Log("Global Light 2D criado");
                }
#else
                LogWarning("2D Renderer não está disponível. Global Light 2D não foi criado.");
#endif
            }
        }

        private void Log(string message)
        {
            if (verboseLogging)
            {
                Debug.Log($"[SceneSetup] {message}");
            }
        }

        private void LogWarning(string message)
        {
            if (verboseLogging)
            {
                Debug.LogWarning($"[SceneSetup] {message}");
            }
        }

        private void LogError(string message)
        {
            Debug.LogError($"[SceneSetup] {message}");
        }
        #endregion

        #region Public Properties
        public SceneType SceneType => sceneType;
        public bool VerboseLogging => verboseLogging;
        public bool RequireGameManager => requireGameManager;
        public bool RequireAudioManager => requireAudioManager;
        public bool RequireInputManager => requireInputManager;
        public bool RequireEventSystem => requireEventSystem;
        // Propriedades de câmera removidas
        public bool RequireGlobalVolume => requireGlobalVolume;
        public bool RequireGlobalLight2D => requireGlobalLight2D;
        public bool SetupCompleted => setupCompleted;
        #endregion

        #region Debug Helpers
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool showDebugInfo = false;

        private void OnDrawGizmos()
        {
            if (!showDebugInfo) return;

            // Mostrar informações de debug sobre o setup
            UnityEditor.Handles.BeginGUI();

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Scene Setup Manager", UnityEditor.EditorStyles.boldLabel);
            GUILayout.Label($"Type: {sceneType}");
            GUILayout.Label($"Setup Completed: {setupCompleted}");
            GUILayout.Label($"Verbose Logging: {verboseLogging}");

            if (GUILayout.Button("Setup Scene"))
            {
                SetupScene();
            }

            if (GUILayout.Button("Validate Scene"))
            {
                ValidateScene();
            }

            GUILayout.EndArea();

            UnityEditor.Handles.EndGUI();
        }
#endif
        #endregion
    }

    /// <summary>
    /// Tipos de cena suportados pelo sistema de setup
    /// </summary>
    public enum SceneType
    {
        [Tooltip("Detecta automaticamente baseado no nome da cena")]
        Auto,

        [Tooltip("Cenas de gameplay principal (InitialCave, etc.)")]
        Gameplay,

        [Tooltip("Cenas de interface e menus (TitleScreen, etc.)")]
        Menu,

        [Tooltip("Cenas cinematográticas e cutscenes")]
        Cutscene
    }
}