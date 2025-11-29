using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace TheSlimeKing.Dialogue
{
    /// <summary>
    /// Componente anexado aos NPCs que gerencia a interação e os dados do diálogo.
    /// Detecta proximidade do jogador e inicia diálogos quando solicitado.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueNPC : MonoBehaviour
    {
        #region Serialized Fields
        
        [Header("Dialogue Configuration")]
        [Tooltip("Lista de textos localizados que serão exibidos em sequência")]
        [SerializeField] private List<LocalizedString> dialogueTexts = new List<LocalizedString>();
        
        [Tooltip("Raio de interação com o jogador")]
        [SerializeField] private float interactionRadius = 1.5f;
        
        [Header("Events")]
        [Tooltip("Eventos invocados ao completar o diálogo (quests, cutscenes, itens, etc.)")]
        [SerializeField] private UnityEvent onDialogueComplete = new UnityEvent();
        
        [Header("Visual Feedback")]
        [Tooltip("Indicador visual de interação disponível (opcional)")]
        [SerializeField] private GameObject interactionIndicator;
        
        #endregion
        
        #region Private Fields
        
        private BoxCollider2D interactionCollider;
        private Transform playerTransform;
        private bool playerInRange;
        private DialogueUI dialogueUI;
        private bool isDialogueActive;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Configurar collider
            interactionCollider = GetComponent<BoxCollider2D>();
            if (interactionCollider != null)
            {
                interactionCollider.isTrigger = true;
            }
            
            // Validar configuração
            ValidateConfiguration();
            
            // Esconder indicador inicialmente
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }
        }
        
        private void Start()
        {
            // Buscar DialogueUI na cena
            dialogueUI = FindFirstObjectByType<DialogueUI>();
            
            if (dialogueUI == null)
            {
                Debug.LogError($"[DialogueNPC] DialogueUI não encontrado na cena! NPC: {gameObject.name}");
            }
        }
        
        private void Update()
        {
            // Atualizar visibilidade do indicador baseado na distância
            if (playerInRange && playerTransform != null)
            {
                UpdateInteractionIndicator();
            }
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerTransform = other.transform;
                playerInRange = true;
                UpdateInteractionIndicator();
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerTransform = null;
                playerInRange = false;
                
                if (interactionIndicator != null)
                {
                    interactionIndicator.SetActive(false);
                }
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Inicia o diálogo com este NPC.
        /// </summary>
        public void StartDialogue()
        {
            // Validar se pode iniciar diálogo
            if (isDialogueActive)
            {
                Debug.LogWarning($"[DialogueNPC] Diálogo já está ativo para {gameObject.name}");
                return;
            }
            
            if (dialogueTexts == null || dialogueTexts.Count == 0)
            {
                Debug.LogWarning($"[DialogueNPC] Nenhum texto de diálogo configurado para {gameObject.name}");
                return;
            }
            
            if (dialogueUI == null)
            {
                Debug.LogError($"[DialogueNPC] DialogueUI não está disponível. Não é possível iniciar diálogo.");
                return;
            }
            
            // Esconder indicador durante diálogo
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }
            
            // Marcar diálogo como ativo
            isDialogueActive = true;
            
            // Mostrar diálogo
            dialogueUI.Show(dialogueTexts, CreateDialogueCompleteEvent());
        }
        
        /// <summary>
        /// Verifica se o jogador está dentro do raio de interação.
        /// </summary>
        public bool IsPlayerInRange()
        {
            if (playerTransform == null) return false;
            
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            return distance <= interactionRadius;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Valida a configuração do NPC.
        /// </summary>
        private void ValidateConfiguration()
        {
            if (dialogueTexts == null || dialogueTexts.Count == 0)
            {
                Debug.LogWarning($"[DialogueNPC] Lista de textos vazia em {gameObject.name}. Configure textos no Inspector.");
            }
            
            if (interactionCollider == null)
            {
                Debug.LogError($"[DialogueNPC] BoxCollider2D não encontrado em {gameObject.name}");
            }
        }
        
        /// <summary>
        /// Atualiza a visibilidade do indicador de interação baseado na distância.
        /// </summary>
        private void UpdateInteractionIndicator()
        {
            if (interactionIndicator == null) return;
            if (isDialogueActive) return;
            
            bool shouldShow = IsPlayerInRange();
            interactionIndicator.SetActive(shouldShow);
        }
        
        /// <summary>
        /// Cria um UnityEvent que será invocado quando o diálogo completar.
        /// </summary>
        private UnityEvent CreateDialogueCompleteEvent()
        {
            UnityEvent completeEvent = new UnityEvent();
            
            // Adicionar callback para limpar estado
            completeEvent.AddListener(OnDialogueCompleted);
            
            // Adicionar eventos configurados pelo usuário
            if (onDialogueComplete != null)
            {
                for (int i = 0; i < onDialogueComplete.GetPersistentEventCount(); i++)
                {
                    // UnityEvents configurados no Inspector serão invocados automaticamente
                }
                
                // Adicionar listeners runtime
                completeEvent.AddListener(() => onDialogueComplete?.Invoke());
            }
            
            return completeEvent;
        }
        
        /// <summary>
        /// Chamado quando o diálogo é completado.
        /// </summary>
        private void OnDialogueCompleted()
        {
            isDialogueActive = false;
            
            // Restaurar indicador se jogador ainda está no raio
            if (playerInRange)
            {
                UpdateInteractionIndicator();
            }
        }
        
        #endregion
        
        #region Editor Helpers
        
        private void OnDrawGizmosSelected()
        {
            // Desenhar raio de interação no editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
        
        #endregion
    }
}
