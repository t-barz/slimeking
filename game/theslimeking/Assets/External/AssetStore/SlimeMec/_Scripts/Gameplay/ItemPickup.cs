using UnityEngine;
using SlimeMec.Items;
using TheSlimeKing.Inventory;
using SlimeKing.Gameplay;
using SlimeKing.Core;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Item que requer interação manual do jogador para ser coletado.
    /// Move-se até a posição do player e é absorvido após chegar no destino.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [Header("🎯 Configuração do Item")]
        [SerializeField] private CollectableItemData itemData;

        [Header("💼 Integração com Inventário")]
        [SerializeField] private ItemData inventoryItemData;
        [SerializeField] private int itemQuantity = 1;

        [Header("🤝 Sistema de Interação")]
        [SerializeField] private float interactionRadius = 2f;

        [Header("🎯 Movimento até Player")]
        [SerializeField] private Vector3 followOffset = new Vector3(0.5f, 0.5f, 0f);
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float arrivalThreshold = 0.1f;
        [SerializeField] private float absorptionDelay = 0.25f;

        [Header("⏱️ Delay de Ativação")]
        [SerializeField] private float activationDelay = 0.1f;

        [Header("⚡ Performance e Debug")]
        [SerializeField] private float checkInterval = 0.1f;
        [SerializeField] private bool enableDebugLogs = false;

        // Cache de componentes
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        private Transform _playerTransform;

        // Estados simplificados
        private bool _interactionEnabled = false;
        private bool _isMovingToPlayer = false;
        private bool _isAbsorbed = false;
        private bool _isWaitingAtDestination = false;
        private float _spawnTime;
        private float _lastCheck;
        private float _arrivalTime;

        // Cache de performance (reutiliza do ItemCollectable)
        private static readonly string PLAYER_TAG = "Player";
        private static Transform s_cachedPlayerTransform;

        #region Unity Lifecycle

        private void Awake()
        {
            CacheComponents();
            _spawnTime = Time.time;
        }

        private void Start()
        {
            FindPlayer();
            SetupItem();
        }

        private void Update()
        {
            if (_isAbsorbed) return;

            if (_isMovingToPlayer)
            {
                MoveToPlayerAndAbsorb();
                return;
            }

            // Verifica ativação apenas se necessário
            if (!_interactionEnabled)
            {
                CheckActivation();
            }
        }

        #endregion

        #region Initialization

        private void CacheComponents()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;

            // Auto-configura layer se necessário
            if (gameObject.layer == 0)
            {
                int interactableLayer = LayerMask.NameToLayer("Interactable");
                if (interactableLayer != -1)
                    gameObject.layer = interactableLayer;
            }
        }

        private void SetupItem()
        {
            if (itemData != null)
                interactionRadius = itemData.detectionRadius;

            // Desabilita collider se BounceHandler existir
            var bounceHandler = GetComponent<BounceHandler>();
            if (bounceHandler != null)
                _collider.enabled = false;
        }

        private void FindPlayer()
        {
            if (s_cachedPlayerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
                if (player != null)
                    s_cachedPlayerTransform = player.transform;
            }
            _playerTransform = s_cachedPlayerTransform;
        }

        private void CheckActivation()
        {
            float timeSinceSpawn = Time.time - _spawnTime;
            if (timeSinceSpawn < activationDelay) return;

            // Verifica BounceHandler se existir
            var bounceHandler = GetComponent<BounceHandler>();
            if (bounceHandler != null && !bounceHandler.IsReadyForCollection)
                return;

            ActivateInteraction();
        }

        private void ActivateInteraction()
        {
            _interactionEnabled = true;
            if (_collider != null)
                _collider.enabled = true;

            LogDebug($"Interação ativada para {GetItemName()}");
        }

        #endregion

        #region Movement and Absorption

        /// <summary>
        /// Move o item até o player e o absorve após delay quando chegar no destino
        /// </summary>
        private void MoveToPlayerAndAbsorb()
        {
            if (_playerTransform == null)
            {
                CompleteAbsorption();
                return;
            }

            Vector3 targetPosition = _playerTransform.position + followOffset;
            Vector3 currentPosition = transform.position;

            // Se ainda está se movendo
            if (!_isWaitingAtDestination)
            {
                // Move em direção ao target
                transform.position = Vector3.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);

                // Verifica se chegou no destino
                float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
                if (distanceToTarget <= arrivalThreshold)
                {
                    _isWaitingAtDestination = true;
                    _arrivalTime = Time.time;
                    LogDebug($"{GetItemName()} chegou no destino, aguardando {absorptionDelay}s para absorção");
                }
            }
            // Se está aguardando na posição destino
            else
            {
                // Mantém posição fixa no offset do player
                transform.position = targetPosition;

                // Verifica se delay de absorção passou
                if (Time.time - _arrivalTime >= absorptionDelay)
                {
                    CompleteAbsorption();
                }
            }
        }

        /// <summary>
        /// Completa o processo de absorção - executa FX e destrói o item
        /// </summary>
        private void CompleteAbsorption()
        {
            if (_isAbsorbed) return;

            _isAbsorbed = true;
            LogDebug($"Absorção completa de {GetItemName()}");

            // Executa efeitos de coleta
            PlayCollectionEffects();

            // Destrói o item após pequeno delay para permitir efeitos
            StartCoroutine(DestroyAfterEffects());
        }

        /// <summary>
        /// Executa efeitos visuais e sonoros da coleta
        /// </summary>
        private void PlayCollectionEffects()
        {
            if (itemData == null) return;

            // Efeito de partículas
            if (itemData.vfxPrefab != null)
            {
                var vfx = Instantiate(itemData.vfxPrefab, transform.position, Quaternion.identity);
                if (vfx != null)
                    Destroy(vfx, 2f);
            }

            // Efeito sonoro
            if (itemData.collectSound != null)
            {
                AudioSource.PlayClipAtPoint(itemData.collectSound, transform.position, 1f);
            }
        }

        /// <summary>
        /// Destrói o item após permitir que efeitos sejam reproduzidos
        /// </summary>
        private System.Collections.IEnumerator DestroyAfterEffects()
        {
            // Esconde visualmente o item imediatamente
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = false;

            if (_collider != null)
                _collider.enabled = false;

            // Aguarda um frame para garantir que efeitos sejam instanciados
            yield return null;
            yield return new WaitForSeconds(0.1f);

            Destroy(gameObject);
        }

        #endregion

        #region Collection System

        /// <summary>
        /// Inicia processo de coleta do item
        /// </summary>
        private void StartCollection()
        {
            if (_isMovingToPlayer || _isAbsorbed) return;

            LogDebug($"Iniciando coleta de {GetItemName()}");

            // Desabilita interação
            _interactionEnabled = false;
            if (_collider != null)
                _collider.enabled = false;

            // Adiciona ao inventário
            if (!TryAddToInventory())
            {
                RevertCollection();
                return;
            }

            // Aplica efeitos se configurado
            ApplyItemEffects();

            // Inicia movimento até player
            _isMovingToPlayer = true;
        }

        /// <summary>
        /// Adiciona item ao inventário
        /// </summary>
        private bool TryAddToInventory()
        {
            if (inventoryItemData == null || InventoryManager.Instance == null)
                return true;

            bool success = InventoryManager.Instance.AddItem(inventoryItemData, itemQuantity);

            if (success)
                LogDebug($"'{inventoryItemData.itemName}' adicionado ao inventário (x{itemQuantity})");
            else
                LogDebug($"Inventário cheio! '{inventoryItemData.itemName}' não foi coletado");

            return success;
        }

        /// <summary>
        /// Reverte coleta se falhou (ex: inventário cheio)
        /// </summary>
        private void RevertCollection()
        {
            _interactionEnabled = true;
            if (_collider != null)
                _collider.enabled = true;
        }

        /// <summary>
        /// Aplica efeitos do item ao player
        /// </summary>
        private void ApplyItemEffects()
        {
            if (itemData?.HasBuffEffect != true || _playerTransform == null)
                return;

            var buffHandler = _playerTransform.GetComponent<ItemBuffHandler>();
            if (buffHandler != null)
            {
                buffHandler.AddBuff(itemData);
                LogDebug($"Buffs aplicados de {itemData.itemName}");
            }
        }

        #endregion

        #region IInteractable Implementation

        public bool TryInteract(Transform player)
        {
            if (!CanInteract(player))
                return false;

            LogDebug($"Player interagiu com {GetItemName()}");
            StartCollection();
            return true;
        }

        public bool CanInteract(Transform player)
        {
            if (!_interactionEnabled || _isMovingToPlayer || _isAbsorbed)
                return false;

            float distance = Vector2.Distance(transform.position, player.position);
            return distance <= interactionRadius;
        }

        public string GetInteractionPrompt()
        {
            if (_isMovingToPlayer || _isAbsorbed) return "";
            return $"Pegar {GetItemName()}";
        }

        public int GetInteractionPriority() => 50;

        #endregion

        #region Public API

        public void SetItemData(CollectableItemData newItemData)
        {
            itemData = newItemData;
            SetupItem();
        }

        public void SetInventoryItemData(ItemData newItemData, int quantity = 1)
        {
            inventoryItemData = newItemData;
            itemQuantity = quantity;
        }

        public void ForceActivateInteraction()
        {
            ActivateInteraction();
        }

        #endregion

        #region Properties and Helpers

        public bool IsInteractionEnabled => _interactionEnabled;
        public bool IsMovingToPlayer => _isMovingToPlayer;
        public bool IsAbsorbed => _isAbsorbed;
        public bool IsWaitingAtDestination => _isWaitingAtDestination;

        private string GetItemName()
        {
            return itemData?.itemName ?? inventoryItemData?.itemName ?? "Item";
        }

        private void LogDebug(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[ItemPickup] {message}");
        }

        #endregion

        #region Debug

        private void OnDrawGizmosSelected()
        {
            // Raio de interação
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);

            // Posição de destino
            if (_playerTransform != null)
            {
                Vector3 targetPos = _playerTransform.position + followOffset;

                // Cor baseada no estado
                if (_isWaitingAtDestination)
                    Gizmos.color = Color.red; // Vermelho quando aguardando absorção
                else
                    Gizmos.color = Color.green; // Verde para posição destino normal

                Gizmos.DrawWireSphere(targetPos, arrivalThreshold);

                if (_isMovingToPlayer)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(transform.position, targetPos);
                }
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Force Interaction")]
        private void EditorForceInteraction()
        {
            if (!Application.isPlaying) return;
            if (_playerTransform != null)
                TryInteract(_playerTransform);
        }

        [ContextMenu("Debug Status")]
        private void EditorDebugStatus()
        {
            if (!Application.isPlaying) return;

            string status = $"[ItemPickup] {GetItemName()}\n";
            status += $"Interaction Enabled: {_interactionEnabled}\n";
            status += $"Moving to Player: {_isMovingToPlayer}\n";
            status += $"Waiting at Destination: {_isWaitingAtDestination}\n";
            status += $"Absorbed: {_isAbsorbed}\n";
            if (_isWaitingAtDestination)
            {
                float timeWaiting = Time.time - _arrivalTime;
                float timeRemaining = Mathf.Max(0f, absorptionDelay - timeWaiting);
                status += $"Time until absorption: {timeRemaining:F1}s\n";
            }
            status += $"Distance to Player: {(_playerTransform != null ? Vector2.Distance(transform.position, _playerTransform.position) : -1f):F2}";

            Debug.Log(status);
        }
#endif

        #endregion
    }
}