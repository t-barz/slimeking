using UnityEngine;
using SlimeMec.Systems;
using SlimeKing.Systems.UI;

namespace SlimeMec.Gameplay.NPCs
{
    /// <summary>
    /// Gerencia a detecção de proximidade do jogador e o início de diálogos com NPCs.
    /// Exibe ícone de interação quando o jogador está próximo e inicia diálogos via DialogueManager.
    /// 
    /// REQUISITOS DE CONFIGURAÇÃO:
    /// - O GameObject do jogador DEVE ter a tag "Player" para ser detectado pelo sistema
    /// - O NPC deve ter um CircleCollider2D configurado como trigger (adicionado automaticamente)
    /// - O interactionIconPrefab deve ser atribuído (pode ser feito via Quick Setup)
    /// 
    /// DETECÇÃO DE PROXIMIDADE:
    /// Este componente usa OnTriggerEnter2D/OnTriggerExit2D para detectar quando o jogador
    /// entra ou sai da área de interação. A detecção é baseada na tag "Player", portanto
    /// certifique-se de que o GameObject do jogador possui esta tag configurada.
    /// 
    /// INTEGRAÇÃO COM PLAYERCONTROLLER:
    /// Este componente funciona independentemente do PlayerController. A pausa do jogador
    /// durante diálogos é gerenciada pelo DialogueManager através da configuração
    /// 'pausePlayerDuringDialogue'. Veja a documentação do DialogueManager para mais detalhes
    /// sobre a integração pendente com o PlayerController.
    /// </summary>
    [RequireComponent(typeof(CircleCollider2D))]
    public class NPCDialogueInteraction : MonoBehaviour
    {
        #region Inspector Settings

        [Header("Dialogue Configuration")]
        [Tooltip("ID único do diálogo a ser carregado do JSON")]
        [SerializeField] private string dialogueId = "npc_default";

        [Tooltip("Raio de detecção para interação com o jogador. Se 0, usa o valor padrão do DialogueSystemSettings")]
        [SerializeField] private float interactionRadius = 2.5f;

        [Tooltip("Ponto de ancoragem para o ícone de interação (geralmente acima da cabeça do NPC)")]
        [SerializeField] private Transform iconAnchor;

        [Tooltip("Prefab do ícone de interação. Se não atribuído, usa o prefab padrão do DialogueSystemSettings")]
        [SerializeField] private GameObject interactionIconPrefab;

        [Header("Input")]
        [Tooltip("Nome do botão de interação configurado no Input Manager. Se vazio, usa o valor padrão do DialogueSystemSettings")]
        [SerializeField] private string interactionButton = "Interact";

        [Header("Visual Feedback")]
        [Tooltip("Offset do ícone em relação ao anchor (world space). Permite ajustar a posição vertical do ícone")]
        [SerializeField] private Vector3 iconOffset = new Vector3(0, 0.5f, 0);

        [Tooltip("Cor do gizmo de visualização no editor para o raio de interação")]
        [SerializeField] private Color gizmoColor = new Color(0, 1, 0, 0.3f);

        [Header("Advanced Configuration")]
        [Tooltip("Velocidade customizada do efeito typewriter (caracteres por segundo). Se 0, usa o valor padrão do DialogueSystemSettings. Valores negativos desabilitam o efeito")]
        [SerializeField] private float customTypewriterSpeed = 0f;

        [Tooltip("Se marcado, usa configuração customizada para pausar o jogador. Se desmarcado, usa o valor global do DialogueManager")]
        [SerializeField] private bool useCustomPauseSettings = false;

        [Tooltip("Se o jogador deve ser pausado durante este diálogo específico. Apenas usado se 'useCustomPauseSettings' estiver marcado")]
        [SerializeField] private bool pausePlayerDuringDialogue = true;

        #endregion

        #region Private Fields

        /// <summary>
        /// Referência ao collider de trigger para detecção de proximidade
        /// </summary>
        private CircleCollider2D interactionCollider;

        /// <summary>
        /// Instância do ícone de interação
        /// </summary>
        private GameObject interactionIconInstance;

        /// <summary>
        /// Componente InteractionIcon da instância
        /// </summary>
        private InteractionIcon interactionIcon;

        /// <summary>
        /// Flag indicando se o jogador está dentro da área de interação
        /// </summary>
        private bool isPlayerInRange;

        /// <summary>
        /// Flag indicando se o componente foi inicializado corretamente
        /// </summary>
        private bool isInitialized;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            InitializeComponent();
        }

        private void Start()
        {
            ValidateConfiguration();
            SetupInteractionIcon();
        }

        private void Update()
        {
            // Verifica input apenas se o jogador está na área e não há diálogo ativo
            if (isPlayerInRange && !IsDialogueActive())
            {
                CheckInteractionInput();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Detecta quando o jogador entra na área de interação
            if (other.CompareTag("Player"))
            {
                OnPlayerEnterRange();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Detecta quando o jogador sai da área de interação
            if (other.CompareTag("Player"))
            {
                OnPlayerExitRange();
            }
        }

        private void OnDestroy()
        {
            // Limpa instância do ícone ao destruir o componente
            if (interactionIconInstance != null)
            {
                Destroy(interactionIconInstance);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualiza o raio de interação no editor
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);

            // Desenha linha até o anchor do ícone se configurado
            if (iconAnchor != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, iconAnchor.position);
                Gizmos.DrawWireSphere(iconAnchor.position + iconOffset, 0.2f);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializa o componente e configura o collider de trigger.
        /// </summary>
        private void InitializeComponent()
        {
            // Obtém ou adiciona CircleCollider2D
            interactionCollider = GetComponent<CircleCollider2D>();
            if (interactionCollider == null)
            {
                Debug.LogWarning($"[NPCDialogueInteraction] CircleCollider2D not found on {gameObject.name}. Adding component.");
                interactionCollider = gameObject.AddComponent<CircleCollider2D>();
            }

            // Configura o collider como trigger
            interactionCollider.isTrigger = true;
            interactionCollider.radius = interactionRadius;

            isInitialized = true;
            Debug.Log($"[NPCDialogueInteraction] Initialized on {gameObject.name} with dialogue ID: {dialogueId}");
        }

        /// <summary>
        /// Valida a configuração do componente e loga warnings se necessário.
        /// </summary>
        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(dialogueId))
            {
                Debug.LogWarning($"[NPCDialogueInteraction] Dialogue ID is empty on {gameObject.name}. Dialogue will not work.");
            }

            if (interactionIconPrefab == null)
            {
                Debug.LogWarning($"[NPCDialogueInteraction] Interaction icon prefab is not assigned on {gameObject.name}. Icon will not be displayed.");
            }

            if (iconAnchor == null)
            {
                Debug.LogWarning($"[NPCDialogueInteraction] Icon anchor is not assigned on {gameObject.name}. Using NPC transform as anchor.");
                iconAnchor = transform;
            }

            if (!DialogueManager.HasInstance)
            {
                Debug.LogWarning($"[NPCDialogueInteraction] DialogueManager not found in scene. Dialogue system will not work.");
            }
        }

        /// <summary>
        /// Configura e instancia o ícone de interação.
        /// </summary>
        private void SetupInteractionIcon()
        {
            if (interactionIconPrefab == null)
            {
                Debug.LogWarning($"[NPCDialogueInteraction] Cannot setup interaction icon. Prefab is not assigned.");
                return;
            }

            // Instancia o ícone como filho de um Canvas (será gerenciado pelo prefab)
            interactionIconInstance = Instantiate(interactionIconPrefab);
            interactionIcon = interactionIconInstance.GetComponent<InteractionIcon>();

            if (interactionIcon == null)
            {
                Debug.LogError($"[NPCDialogueInteraction] Interaction icon prefab does not have InteractionIcon component!");
                Destroy(interactionIconInstance);
                interactionIconInstance = null;
                return;
            }

            // Configura o alvo do ícone
            Transform anchor = iconAnchor != null ? iconAnchor : transform;
            interactionIcon.SetTarget(anchor, iconOffset);

            // Inicia oculto
            interactionIcon.Hide();
            interactionIconInstance.SetActive(false);

            Debug.Log($"[NPCDialogueInteraction] Interaction icon setup complete for {gameObject.name}");
        }

        #endregion

        #region Public API

        /// <summary>
        /// Chamado quando o jogador entra na área de interação.
        /// Exibe o ícone de interação.
        /// </summary>
        public void OnPlayerEnterRange()
        {
            if (!isInitialized)
            {
                Debug.LogWarning($"[NPCDialogueInteraction] OnPlayerEnterRange called but component is not initialized.");
                return;
            }

            isPlayerInRange = true;

            // Não exibe o ícone se já há um diálogo ativo
            if (IsDialogueActive())
            {
                return;
            }

            // Exibe o ícone de interação
            if (interactionIcon != null)
            {
                interactionIcon.Show();
                Debug.Log($"[NPCDialogueInteraction] Player entered range of {gameObject.name}. Showing interaction icon.");
            }
        }

        /// <summary>
        /// Chamado quando o jogador sai da área de interação.
        /// Oculta o ícone de interação.
        /// </summary>
        public void OnPlayerExitRange()
        {
            if (!isInitialized)
            {
                return;
            }

            isPlayerInRange = false;

            // Oculta o ícone de interação
            if (interactionIcon != null)
            {
                interactionIcon.Hide();
                Debug.Log($"[NPCDialogueInteraction] Player exited range of {gameObject.name}. Hiding interaction icon.");
            }
        }

        /// <summary>
        /// Tenta iniciar o diálogo com o NPC.
        /// Verifica se o DialogueManager está disponível e se não há diálogo ativo.
        /// Aplica configurações customizadas se definidas.
        /// </summary>
        public void TryStartDialogue()
        {
            if (!isInitialized)
            {
                Debug.LogWarning($"[NPCDialogueInteraction] TryStartDialogue called but component is not initialized.");
                return;
            }

            if (string.IsNullOrEmpty(dialogueId))
            {
                Debug.LogError($"[NPCDialogueInteraction] Cannot start dialogue. Dialogue ID is empty on {gameObject.name}.");
                return;
            }

            if (!DialogueManager.HasInstance)
            {
                Debug.LogError($"[NPCDialogueInteraction] Cannot start dialogue. DialogueManager not found.");
                return;
            }

            // Verifica se já há um diálogo ativo
            if (IsDialogueActive())
            {
                Debug.LogWarning($"[NPCDialogueInteraction] Cannot start dialogue. Another dialogue is already active.");
                return;
            }

            Debug.Log($"[NPCDialogueInteraction] Starting dialogue '{dialogueId}' for {gameObject.name}");

            // Aplica configurações customizadas antes de iniciar o diálogo
            ApplyCustomSettings();

            // Oculta o ícone de interação
            if (interactionIcon != null)
            {
                interactionIcon.Hide();
            }

            // Inicia o diálogo via DialogueManager
            DialogueManager.Instance.StartDialogue(dialogueId);
        }

        /// <summary>
        /// Verifica se o jogador está dentro da área de interação.
        /// </summary>
        /// <returns>True se o jogador está na área</returns>
        public bool IsPlayerInRange()
        {
            return isPlayerInRange;
        }

        /// <summary>
        /// Define o ID do diálogo em runtime.
        /// </summary>
        /// <param name="newDialogueId">Novo ID do diálogo</param>
        public void SetDialogueId(string newDialogueId)
        {
            dialogueId = newDialogueId;
            Debug.Log($"[NPCDialogueInteraction] Dialogue ID changed to '{dialogueId}' on {gameObject.name}");
        }

        /// <summary>
        /// Define o raio de interação em runtime.
        /// </summary>
        /// <param name="radius">Novo raio de interação</param>
        public void SetInteractionRadius(float radius)
        {
            interactionRadius = Mathf.Max(0.1f, radius);
            if (interactionCollider != null)
            {
                interactionCollider.radius = interactionRadius;
            }
            Debug.Log($"[NPCDialogueInteraction] Interaction radius changed to {interactionRadius} on {gameObject.name}");
        }

        /// <summary>
        /// Obtém a velocidade customizada do typewriter.
        /// Retorna 0 se não há configuração customizada (deve usar o valor padrão).
        /// </summary>
        /// <returns>Velocidade customizada do typewriter ou 0 para usar padrão</returns>
        public float GetCustomTypewriterSpeed()
        {
            return customTypewriterSpeed;
        }

        /// <summary>
        /// Verifica se este NPC usa configurações customizadas de pausa do jogador.
        /// </summary>
        /// <returns>True se usa configurações customizadas</returns>
        public bool UsesCustomPauseSettings()
        {
            return useCustomPauseSettings;
        }

        /// <summary>
        /// Obtém a configuração customizada de pausa do jogador.
        /// Apenas relevante se UsesCustomPauseSettings() retorna true.
        /// </summary>
        /// <returns>True se deve pausar o jogador durante o diálogo</returns>
        public bool GetPausePlayerDuringDialogue()
        {
            return pausePlayerDuringDialogue;
        }

        /// <summary>
        /// Define a velocidade customizada do typewriter em runtime.
        /// </summary>
        /// <param name="speed">Velocidade em caracteres por segundo (0 para usar padrão)</param>
        public void SetCustomTypewriterSpeed(float speed)
        {
            customTypewriterSpeed = Mathf.Max(0f, speed);
            Debug.Log($"[NPCDialogueInteraction] Custom typewriter speed set to {customTypewriterSpeed} on {gameObject.name}");
        }

        /// <summary>
        /// Define se deve usar configurações customizadas de pausa do jogador.
        /// </summary>
        /// <param name="useCustom">True para usar configurações customizadas</param>
        /// <param name="shouldPause">Se deve pausar o jogador (apenas usado se useCustom é true)</param>
        public void SetCustomPauseSettings(bool useCustom, bool shouldPause = true)
        {
            useCustomPauseSettings = useCustom;
            pausePlayerDuringDialogue = shouldPause;
            Debug.Log($"[NPCDialogueInteraction] Custom pause settings: useCustom={useCustom}, shouldPause={shouldPause} on {gameObject.name}");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Verifica o input do jogador para iniciar a interação.
        /// </summary>
        private void CheckInteractionInput()
        {
            // Verifica se o botão de interação foi pressionado
            if (Input.GetButtonDown(interactionButton))
            {
                TryStartDialogue();
            }
        }

        /// <summary>
        /// Verifica se há um diálogo ativo no DialogueManager.
        /// </summary>
        /// <returns>True se há um diálogo ativo</returns>
        private bool IsDialogueActive()
        {
            return DialogueManager.HasInstance && DialogueManager.Instance.IsDialogueActive();
        }

        /// <summary>
        /// Aplica configurações customizadas ao DialogueManager e DialogueUI antes de iniciar o diálogo.
        /// Se configurações customizadas não estão definidas, usa os valores padrão do DialogueSystemSettings.
        /// </summary>
        private void ApplyCustomSettings()
        {
            if (!DialogueManager.HasInstance)
            {
                return;
            }

            // Aplica configuração customizada de pausa do jogador se definida
            if (useCustomPauseSettings)
            {
                DialogueManager.Instance.SetPausePlayerDuringDialogue(pausePlayerDuringDialogue);
                Debug.Log($"[NPCDialogueInteraction] Using custom pause setting: {pausePlayerDuringDialogue}");
            }

            // Aplica velocidade customizada do typewriter ao DialogueUI se definida
            if (customTypewriterSpeed > 0f)
            {
                // Tenta obter referência ao DialogueUI através do DialogueManager
                var dialogueUIObjects = FindObjectsOfType<SlimeKing.Systems.UI.DialogueUI>();
                if (dialogueUIObjects != null && dialogueUIObjects.Length > 0)
                {
                    dialogueUIObjects[0].SetTypewriterSpeed(customTypewriterSpeed);
                    Debug.Log($"[NPCDialogueInteraction] Custom typewriter speed applied: {customTypewriterSpeed} chars/sec");
                }
                else
                {
                    Debug.LogWarning($"[NPCDialogueInteraction] Could not find DialogueUI to apply custom typewriter speed");
                }
            }
        }

        #endregion

        #region Editor Utilities

        /// <summary>
        /// Valida e atualiza configurações quando valores são alterados no Inspector.
        /// </summary>
        private void OnValidate()
        {
            // Atualiza o raio do collider quando alterado no Inspector
            if (interactionCollider != null)
            {
                interactionCollider.radius = interactionRadius;
                interactionCollider.isTrigger = true;
            }

            // Garante que o raio seja positivo
            if (interactionRadius < 0.1f)
            {
                interactionRadius = 0.1f;
            }
        }

        #endregion
    }
}
