using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Gerencia o sistema de diálogo do NPC.
    /// Controla quando e como o diálogo é acionado pelo jogador.
    /// </summary>
    public class NPCDialogue : MonoBehaviour
    {
        [Header("Dialogue Configuration")]
        [Tooltip("Referência ao ScriptableObject com dados de diálogo do NPC")]
        public DialogueData dialogueData;

        [Tooltip("Tipo de acionamento do diálogo (Proximidade ou Interação)")]
        public DialogueTriggerType triggerType = DialogueTriggerType.Interaction;

        [Tooltip("Alcance de acionamento do diálogo (metros)")]
        public float triggerRange = 2.0f;

        [Header("State")]
        [Tooltip("Indica se o diálogo está atualmente ativo")]
        private bool isDialogueActive = false;

        [Tooltip("Indica se o jogador está dentro do alcance de acionamento")]
        private bool playerInRange = false;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            if (dialogueData == null)
            {
                Debug.LogWarning($"⚠️ NPCDialogue em '{gameObject.name}' não possui DialogueData atribuído!", this);
            }
        }

        private void Update()
        {
            // Verificar acionamento por proximidade
            if (triggerType == DialogueTriggerType.Proximity && !isDialogueActive)
            {
                CheckProximityTrigger();
            }

            // TODO: Verificar input de interação quando triggerType == Interaction
            // if (triggerType == DialogueTriggerType.Interaction && playerInRange && Input.GetKeyDown(KeyCode.E))
            // {
            //     StartDialogue();
            // }
        }

        /// <summary>
        /// Verifica se o jogador está dentro do alcance de proximidade.
        /// </summary>
        private void CheckProximityTrigger()
        {
            // TODO: Obter referência ao jogador quando PlayerController for implementado
            // if (PlayerController.Instance != null)
            // {
            //     float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            //     
            //     if (distanceToPlayer <= triggerRange)
            //     {
            //         if (!playerInRange)
            //         {
            //             playerInRange = true;
            //             ShowDialoguePrompt();
            //         }
            //         
            //         // Auto-iniciar diálogo por proximidade
            //         if (!isDialogueActive)
            //         {
            //             StartDialogue();
            //         }
            //     }
            //     else
            //     {
            //         if (playerInRange)
            //         {
            //             playerInRange = false;
            //             HideDialoguePrompt();
            //         }
            //     }
            // }
        }

        /// <summary>
        /// Inicia o diálogo com o NPC.
        /// </summary>
        public void StartDialogue()
        {
            if (dialogueData == null)
            {
                Debug.LogWarning($"⚠️ Não é possível iniciar diálogo: DialogueData não atribuído em '{gameObject.name}'");
                return;
            }

            if (isDialogueActive)
            {
                Debug.Log($"💬 Diálogo com '{gameObject.name}' já está ativo.");
                return;
            }

            isDialogueActive = true;

            // TODO: Integrar com DialogueManager quando implementado
            // if (DialogueManager.Instance != null)
            // {
            //     DialogueManager.Instance.StartDialogue(dialogueData);
            // }

            // Atualizar animação para estado de conversa
            if (animator != null)
            {
                animator.SetBool("IsTalking", true);
            }

            Debug.Log($"💬 Iniciando diálogo com '{dialogueData.npcName}'");
        }

        /// <summary>
        /// Encerra o diálogo com o NPC.
        /// </summary>
        public void EndDialogue()
        {
            if (!isDialogueActive)
            {
                return;
            }

            isDialogueActive = false;

            // Resetar animação para estado normal
            if (animator != null)
            {
                animator.SetBool("IsTalking", false);
            }

            Debug.Log($"💬 Diálogo com '{dialogueData.npcName}' encerrado.");
        }

        /// <summary>
        /// Exibe prompt visual indicando que o jogador pode interagir.
        /// </summary>
        public void ShowDialoguePrompt()
        {
            // TODO: Integrar com sistema de UI quando implementado
            // UIManager.Instance.ShowInteractionPrompt("Pressione E para conversar");
            
            Debug.Log($"💬 Prompt de diálogo exibido para '{gameObject.name}'");
        }

        /// <summary>
        /// Esconde o prompt visual de interação.
        /// </summary>
        public void HideDialoguePrompt()
        {
            // TODO: Integrar com sistema de UI quando implementado
            // UIManager.Instance.HideInteractionPrompt();
            
            Debug.Log($"💬 Prompt de diálogo escondido para '{gameObject.name}'");
        }

        /// <summary>
        /// Retorna se o diálogo está atualmente ativo.
        /// </summary>
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }

        /// <summary>
        /// Retorna se o jogador está dentro do alcance de interação.
        /// </summary>
        public bool IsPlayerInRange()
        {
            return playerInRange;
        }

        /// <summary>
        /// Desenha gizmos no editor para visualizar o alcance de acionamento.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Desenhar alcance de acionamento
            Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, triggerRange);
        }
    }
}
