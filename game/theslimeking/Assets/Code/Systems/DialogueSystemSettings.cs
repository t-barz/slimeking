using UnityEngine;

namespace SlimeMec.Systems
{
    /// <summary>
    /// ScriptableObject que contém configurações globais do sistema de diálogos.
    /// Permite configurar caminhos, valores padrão, prefabs e input.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueSystemSettings", menuName = "SlimeKing/Dialogue System Settings")]
    public class DialogueSystemSettings : ScriptableObject
    {
        #region Paths

        [Header("Paths")]
        [Tooltip("Caminho do diretório onde os arquivos JSON de diálogos estão localizados")]
        [SerializeField] private string dialoguesPath = "Assets/Data/Dialogues/";

        #endregion

        #region Default Values

        [Header("Default Values")]
        [Tooltip("Velocidade padrão do efeito typewriter (caracteres por segundo)")]
        [SerializeField] private float defaultTypewriterSpeed = 50f;

        [Tooltip("Raio padrão de detecção de interação com NPCs")]
        [SerializeField] private float defaultInteractionRadius = 2.5f;

        [Tooltip("Idioma padrão do sistema")]
        [SerializeField] private SystemLanguage defaultLanguage = SystemLanguage.English;

        #endregion

        #region Prefabs

        [Header("Prefabs")]
        [Tooltip("Prefab da UI de diálogo")]
        [SerializeField] private GameObject dialogueUIPrefab;

        [Tooltip("Prefab do ícone de interação")]
        [SerializeField] private GameObject interactionIconPrefab;

        #endregion

        #region Input

        [Header("Input")]
        [Tooltip("Nome do botão de interação configurado no Input Manager")]
        [SerializeField] private string interactionButtonName = "Interact";

        #endregion

        #region Properties

        /// <summary>
        /// Caminho do diretório onde os arquivos JSON de diálogos estão localizados.
        /// </summary>
        public string DialoguesPath => dialoguesPath;

        /// <summary>
        /// Velocidade padrão do efeito typewriter (caracteres por segundo).
        /// </summary>
        public float DefaultTypewriterSpeed => defaultTypewriterSpeed;

        /// <summary>
        /// Raio padrão de detecção de interação com NPCs.
        /// </summary>
        public float DefaultInteractionRadius => defaultInteractionRadius;

        /// <summary>
        /// Idioma padrão do sistema.
        /// </summary>
        public SystemLanguage DefaultLanguage => defaultLanguage;

        /// <summary>
        /// Prefab da UI de diálogo.
        /// </summary>
        public GameObject DialogueUIPrefab => dialogueUIPrefab;

        /// <summary>
        /// Prefab do ícone de interação.
        /// </summary>
        public GameObject InteractionIconPrefab => interactionIconPrefab;

        /// <summary>
        /// Nome do botão de interação configurado no Input Manager.
        /// </summary>
        public string InteractionButtonName => interactionButtonName;

        #endregion

        #region Validation

        /// <summary>
        /// Valida as configurações quando o asset é modificado no editor.
        /// </summary>
        private void OnValidate()
        {
            // Valida velocidade do typewriter
            if (defaultTypewriterSpeed < 0f)
            {
                Debug.LogWarning($"[DialogueSystemSettings] defaultTypewriterSpeed cannot be negative. Setting to 0.");
                defaultTypewriterSpeed = 0f;
            }

            if (defaultTypewriterSpeed > 1000f)
            {
                Debug.LogWarning($"[DialogueSystemSettings] defaultTypewriterSpeed is very high ({defaultTypewriterSpeed}). Consider using a lower value.");
            }

            // Valida raio de interação
            if (defaultInteractionRadius <= 0f)
            {
                Debug.LogWarning($"[DialogueSystemSettings] defaultInteractionRadius must be positive. Setting to 1.0.");
                defaultInteractionRadius = 1.0f;
            }

            if (defaultInteractionRadius > 20f)
            {
                Debug.LogWarning($"[DialogueSystemSettings] defaultInteractionRadius is very large ({defaultInteractionRadius}). Consider using a smaller value.");
            }

            // Valida caminho de diálogos
            if (string.IsNullOrWhiteSpace(dialoguesPath))
            {
                Debug.LogWarning($"[DialogueSystemSettings] dialoguesPath is empty. Setting to default.");
                dialoguesPath = "Assets/Data/Dialogues/";
            }

            // Normaliza o caminho para garantir que termina com /
            if (!string.IsNullOrWhiteSpace(dialoguesPath) && !dialoguesPath.EndsWith("/"))
            {
                dialoguesPath += "/";
            }

            // Valida nome do botão de interação
            if (string.IsNullOrWhiteSpace(interactionButtonName))
            {
                Debug.LogWarning($"[DialogueSystemSettings] interactionButtonName is empty. Setting to default 'Interact'.");
                interactionButtonName = "Interact";
            }

            // Valida prefabs
            if (dialogueUIPrefab == null)
            {
                Debug.LogWarning($"[DialogueSystemSettings] dialogueUIPrefab is not assigned. Dialogue UI will not be displayed.");
            }

            if (interactionIconPrefab == null)
            {
                Debug.LogWarning($"[DialogueSystemSettings] interactionIconPrefab is not assigned. Interaction icons will not be displayed.");
            }
        }

        #endregion

        #region Editor Helpers

#if UNITY_EDITOR
        /// <summary>
        /// Cria uma instância padrão do DialogueSystemSettings.
        /// </summary>
        [UnityEditor.MenuItem("Assets/Create/SlimeKing/Dialogue System Settings (Default)", priority = 1)]
        private static void CreateDefaultSettings()
        {
            // Verifica se já existe um asset em Data/Settings
            string assetPath = "Assets/Data/Settings/DialogueSystemSettings.asset";
            
            if (UnityEditor.AssetDatabase.LoadAssetAtPath<DialogueSystemSettings>(assetPath) != null)
            {
                Debug.LogWarning($"[DialogueSystemSettings] Asset already exists at {assetPath}");
                UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<DialogueSystemSettings>(assetPath);
                UnityEditor.EditorGUIUtility.PingObject(UnityEditor.Selection.activeObject);
                return;
            }

            // Cria os diretórios se não existirem
            string directory = "Assets/Data/Settings";
            if (!UnityEditor.AssetDatabase.IsValidFolder(directory))
            {
                string parentFolder = "Assets/Data";
                if (!UnityEditor.AssetDatabase.IsValidFolder(parentFolder))
                {
                    UnityEditor.AssetDatabase.CreateFolder("Assets", "Data");
                }
                UnityEditor.AssetDatabase.CreateFolder(parentFolder, "Settings");
            }

            // Cria a instância
            DialogueSystemSettings settings = CreateInstance<DialogueSystemSettings>();
            
            // Configura valores padrão
            settings.dialoguesPath = "Assets/Data/Dialogues/";
            settings.defaultTypewriterSpeed = 50f;
            settings.defaultInteractionRadius = 2.5f;
            settings.defaultLanguage = SystemLanguage.English;
            settings.interactionButtonName = "Interact";

            // Tenta carregar prefabs existentes
            settings.dialogueUIPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/UI/DialogueUI.prefab");
            settings.interactionIconPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/UI/InteractionIcon.prefab");

            // Salva o asset
            UnityEditor.AssetDatabase.CreateAsset(settings, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            // Seleciona o asset criado
            UnityEditor.Selection.activeObject = settings;
            UnityEditor.EditorGUIUtility.PingObject(settings);

            Debug.Log($"[DialogueSystemSettings] Created default settings at {assetPath}");
        }
#endif

        #endregion
    }
}
