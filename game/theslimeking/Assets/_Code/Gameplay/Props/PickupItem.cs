using UnityEngine;
using SlimeKing.Core;
using TheSlimeKing.Inventory;
using SlimeKing.Gameplay;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Item coletável que pode ser pego pelo jogador.
    /// Implementa IInteractable para integração com o sistema de interação.
    /// Quando coletado, pausa o movimento do player temporariamente.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class PickupItem : MonoBehaviour, IInteractable
    {
        [Header("Item Configuration")]
        [Tooltip("Dados do item a ser coletado")]
        [SerializeField] private ItemData itemData;

        [Tooltip("Quantidade do item a ser adicionada ao inventário")]
        [SerializeField] private int quantity = 1;

        [Header("Pause Configuration")]
        [Tooltip("Duração da pausa do movimento ao coletar (em segundos)")]
        [SerializeField] private float pauseDuration = 0.5f;

        [Header("Visual Configuration")]
        [Tooltip("Prompt de interação exibido ao jogador")]
        [SerializeField] private string interactionPrompt = "Pressione E para coletar";

        [Tooltip("Prioridade de interação (maior = mais prioritário)")]
        [SerializeField] private int interactionPriority = 10;

        [Header("Debug")]
        [Tooltip("Ativar logs de debug")]
        [SerializeField] private bool enableDebugLogs = false;

        private bool _hasBeenCollected = false;

        #region IInteractable Implementation

        public bool TryInteract(Transform player)
        {
            UnityEngine.Debug.Log($"[PickupItem] TryInteract chamado para {gameObject.name}");
            
            if (_hasBeenCollected)
            {
                UnityEngine.Debug.Log($"[PickupItem] Item {gameObject.name} já foi coletado anteriormente");
                return false;
            }

            if (itemData == null)
            {
                UnityEngine.Debug.LogWarning($"[PickupItem] ItemData não configurado em {gameObject.name}");
                return false;
            }

            // Tenta adicionar ao inventário
            if (InventoryManager.Instance != null)
            {
                UnityEngine.Debug.Log($"[PickupItem] Tentando adicionar {itemData.itemName} ao inventário");
                bool addedSuccessfully = InventoryManager.Instance.AddItem(itemData, quantity);

                if (addedSuccessfully)
                {
                    UnityEngine.Debug.Log($"[PickupItem] Item {itemData.itemName} coletado com sucesso");

                    // Pausa o movimento do player ANTES de destruir
                    PausePlayerMovement(player);

                    // Marca como coletado e destrói o objeto
                    _hasBeenCollected = true;
                    Destroy(gameObject);

                    return true;
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"[PickupItem] Inventário cheio. Não foi possível coletar {itemData.itemName}");
                    return false;
                }
            }
            else
            {
                UnityEngine.Debug.LogError("[PickupItem] InventoryManager.Instance não encontrado!");
                return false;
            }
        }

        public bool CanInteract(Transform player)
        {
            return !_hasBeenCollected && itemData != null;
        }

        public string GetInteractionPrompt()
        {
            if (itemData != null)
            {
                return $"{interactionPrompt}: {itemData.itemName}";
            }
            return interactionPrompt;
        }

        public int GetInteractionPriority()
        {
            return interactionPriority;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Pausa o movimento do player temporariamente
        /// </summary>
        private void PausePlayerMovement(Transform player)
        {
            UnityEngine.Debug.Log($"[PickupItem] PausePlayerMovement chamado");
            
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                UnityEngine.Debug.Log($"[PickupItem] PlayerController encontrado, pausando movimento por {pauseDuration}s");
                
                // Pausa o movimento imediatamente
                playerController.SetCanMove(false);

                // Agenda o retorno do movimento no PlayerController (não neste objeto que será destruído)
                playerController.StartCoroutine(ResumeMovementAfterDelay(playerController, pauseDuration));
            }
            else
            {
                UnityEngine.Debug.LogWarning("[PickupItem] PlayerController não encontrado no player!");
            }
        }

        /// <summary>
        /// Corrotina que retoma o movimento do player após o delay.
        /// IMPORTANTE: Esta corrotina roda no PlayerController, não no PickupItem.
        /// </summary>
        private System.Collections.IEnumerator ResumeMovementAfterDelay(PlayerController playerController, float delay)
        {
            UnityEngine.Debug.Log($"[PickupItem] Aguardando {delay}s antes de retomar movimento");
            yield return new WaitForSeconds(delay);

            if (playerController != null)
            {
                UnityEngine.Debug.Log("[PickupItem] Retomando movimento do player");
                playerController.SetCanMove(true);
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Garante que o Collider2D está configurado como Trigger
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null && !collider.isTrigger)
            {
                collider.isTrigger = true;
                UnityEngine.Debug.Log($"[PickupItem] Collider2D de {gameObject.name} configurado como Trigger automaticamente");
            }
        }

        private void Start()
        {
            UnityEngine.Debug.Log($"[PickupItem] {gameObject.name} inicializado - ItemData: {(itemData != null ? itemData.itemName : "NULL")}");
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            // Desenha ícone visual no Scene View
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            // Desenha nome do item se configurado
            if (itemData != null)
            {
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    transform.position + Vector3.up * 0.7f,
                    $"Pickup: {itemData.itemName} x{quantity}"
                );
#endif
            }
        }

        #endregion
    }
}
