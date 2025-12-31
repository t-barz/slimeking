using UnityEngine;
using UnityEngine.InputSystem;
using TheSlimeKing.Dialogue;

namespace TheSlimeKing.Dialogue
{
    /// <summary>
    /// Componente que conecta o Input System com o sistema de diálogo.
    /// Deve ser anexado ao jogador.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class DialogueInputHandler : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Raio de busca por NPCs próximos")]
        [SerializeField] private float npcSearchRadius = 2f;
        
        [Tooltip("Layer dos NPCs")]
        [SerializeField] private LayerMask npcLayer = -1;
        
        private PlayerInput playerInput;
        private InputAction interactAction;
        private DialogueUI dialogueUI;
        private DialogueNPC currentNearbyNPC;
        
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            
            if (playerInput == null)
            {
                Debug.LogError("[DialogueInputHandler] PlayerInput não encontrado!");
                return;
            }
            
            // Obter a ação Interact
            interactAction = playerInput.actions["Interact"];
            
            if (interactAction == null)
            {
                Debug.LogError("[DialogueInputHandler] Ação 'Interact' não encontrada no InputSystem_Actions!");
                return;
            }
        }
        
        private void Start()
        {
            // Buscar DialogueUI na cena
            dialogueUI = FindFirstObjectByType<DialogueUI>();
            
            if (dialogueUI == null)
            {
                Debug.LogWarning("[DialogueInputHandler] DialogueUI não encontrado na cena!");
            }
        }
        
        private void OnEnable()
        {
            if (interactAction != null)
            {
                interactAction.performed += OnInteractPerformed;
            }
        }
        
        private void OnDisable()
        {
            if (interactAction != null)
            {
                interactAction.performed -= OnInteractPerformed;
            }
        }
        
        private void Update()
        {
            // Buscar NPC mais próximo
            FindNearestNPC();
        }
        
        /// <summary>
        /// Chamado quando o jogador pressiona o botão de interação.
        /// </summary>
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            // Se o diálogo está ativo, avançar/completar texto
            if (dialogueUI != null && dialogueUI.IsActive)
            {
                dialogueUI.OnContinuePressed();
                return;
            }
            
            // Se há um NPC próximo, iniciar diálogo
            if (currentNearbyNPC != null && currentNearbyNPC.IsPlayerInRange())
            {
                currentNearbyNPC.StartDialogue();
            }
        }
        
        /// <summary>
        /// Busca o NPC mais próximo dentro do raio de busca.
        /// </summary>
        private void FindNearestNPC()
        {
            // Se o diálogo está ativo, não buscar NPCs
            if (dialogueUI != null && dialogueUI.IsActive)
            {
                currentNearbyNPC = null;
                return;
            }
            
            // Buscar todos os colliders com tag NPC no raio
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, npcSearchRadius, npcLayer);
            
            DialogueNPC nearestNPC = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("NPC"))
                {
                    DialogueNPC npc = collider.GetComponent<DialogueNPC>();
                    
                    if (npc != null && npc.IsPlayerInRange())
                    {
                        float distance = Vector2.Distance(transform.position, collider.transform.position);
                        
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestNPC = npc;
                        }
                    }
                }
            }
            
            currentNearbyNPC = nearestNPC;
        }
        
        private void OnDrawGizmosSelected()
        {
            // Desenhar raio de busca no editor
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, npcSearchRadius);
        }
    }
}
