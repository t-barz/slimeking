using UnityEngine;
using SlimeMec.Alpha.Inventory;
using System;

namespace SlimeMec.Alpha.Inventory
{
    /// <summary>
    /// Gerencia uso de itens consumíveis para Demo Alpha
    /// Integra com Input System (UseItem1-4) e aplica efeitos
    /// </summary>
    public class ItemUsageManager : MonoBehaviour
    {
        #region Singleton
        public static ItemUsageManager Instance { get; private set; }
        #endregion

        #region Events
        public static event Action<int, InventoryItem> OnItemUsed; // slot index, item used
        public static event Action<string, float> OnItemEffect; // effect type, value
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private bool enableCooldown = true;
        [SerializeField] private float globalCooldown = 0.5f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        #endregion

        #region Private Fields
        private InventoryCore inventoryCore;
        private float lastUsageTime = 0f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSingleton();
        }

        private void Start()
        {
            FindReferences();
            SetupIntegrations();
        }

        private void Update()
        {
            HandleManualInput();
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }
        #endregion

        #region Initialization
        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void FindReferences()
        {
            inventoryCore = InventoryCore.Instance;
            if (inventoryCore == null && enableDebugLogs)
            {
                Debug.LogWarning("[ItemUsageManager] InventoryCore instance not found!");
            }
        }

        private void SetupIntegrations()
        {
            if (inventoryCore != null)
            {
                InventoryCore.OnInventoryChanged += HandleInventoryChanged;
            }

            if (enableDebugLogs)
                Debug.Log("[ItemUsageManager] Integrations setup completed");
        }

        private void CleanupEvents()
        {
            if (inventoryCore != null)
            {
                InventoryCore.OnInventoryChanged -= HandleInventoryChanged;
            }
        }
        #endregion

        #region Input Handling
        private void HandleManualInput()
        {
            // Input manual para teste (substitui Input System temporariamente)
            if (Input.GetKeyDown(KeyCode.Alpha1))
                UseItemFromSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                UseItemFromSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                UseItemFromSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                UseItemFromSlot(3);
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Usa item de um slot específico
        /// </summary>
        public bool UseItemFromSlot(int slotIndex)
        {
            if (!CanUseItem())
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[ItemUsageManager] Cannot use item - cooldown active");
                return false;
            }

            if (inventoryCore == null)
            {
                if (enableDebugLogs)
                    Debug.LogError("[ItemUsageManager] InventoryCore reference is null!");
                return false;
            }

            var item = inventoryCore.GetConsumableItem(slotIndex);
            if (item == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[ItemUsageManager] No item in slot {slotIndex}");
                return false;
            }

            return UseItem(item, slotIndex);
        }

        /// <summary>
        /// Usa item específico por ID
        /// </summary>
        public bool UseItem(string itemId)
        {
            if (inventoryCore == null) return false;

            // Encontra o item no inventário
            var allItems = inventoryCore.GetAllItems();
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i].id == itemId)
                {
                    return UseItem(allItems[i], i);
                }
            }

            if (enableDebugLogs)
                Debug.LogWarning($"[ItemUsageManager] Item {itemId} not found in inventory");
            
            return false;
        }

        /// <summary>
        /// Usa item específico
        /// </summary>
        public bool UseItem(InventoryItem item, int slotIndex = -1)
        {
            if (item == null || item.type != ItemType.Consumable)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[ItemUsageManager] Item is null or not consumable");
                return false;
            }

            if (!CanUseItem())
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[ItemUsageManager] Cannot use item - cooldown active");
                return false;
            }

            // Aplica efeito do item
            ApplyItemEffect(item);

            // Remove item do inventário
            if (inventoryCore != null)
            {
                inventoryCore.UseItem(item.id);
            }

            // Atualiza cooldown
            lastUsageTime = Time.time;

            // Notifica uso
            OnItemUsed?.Invoke(slotIndex, item);

            if (enableDebugLogs)
                Debug.Log($"[ItemUsageManager] Used item: {item.displayName}");

            return true;
        }
        #endregion

        #region Item Effects
        private void ApplyItemEffect(InventoryItem item)
        {
            switch (item.id)
            {
                case "health_potion":
                    ApplyHealthEffect(50f);
                    break;
                    
                case "mana_potion":
                    ApplyManaEffect(30f);
                    break;
                    
                case "speed_boost":
                    ApplySpeedBoostEffect(10f);
                    break;
                    
                default:
                    ApplyGenericEffect(item);
                    break;
            }
        }

        private void ApplyHealthEffect(float amount)
        {
            // TODO: Integrar com PlayerAttributesSystem quando disponível
            OnItemEffect?.Invoke("health", amount);
            
            if (enableDebugLogs)
                Debug.Log($"[ItemUsageManager] Applied health effect: +{amount} HP");
        }

        private void ApplyManaEffect(float amount)
        {
            // TODO: Integrar com sistema de mana quando disponível
            OnItemEffect?.Invoke("mana", amount);
            
            if (enableDebugLogs)
                Debug.Log($"[ItemUsageManager] Applied mana effect: +{amount} MP");
        }

        private void ApplySpeedBoostEffect(float duration)
        {
            // TODO: Integrar com PlayerController para speed boost temporário
            OnItemEffect?.Invoke("speed_boost", duration);
            
            if (enableDebugLogs)
                Debug.Log($"[ItemUsageManager] Applied speed boost: {duration}s");
        }

        private void ApplyGenericEffect(InventoryItem item)
        {
            // Efeito genérico para itens não especificados
            OnItemEffect?.Invoke("generic", 1f);
            
            if (enableDebugLogs)
                Debug.Log($"[ItemUsageManager] Applied generic effect for: {item.displayName}");
        }
        #endregion

        #region Helper Methods
        private bool CanUseItem()
        {
            if (!enableCooldown) return true;
            return Time.time - lastUsageTime >= globalCooldown;
        }

        private void HandleInventoryChanged()
        {
            // Reação a mudanças no inventário se necessário
            if (enableDebugLogs)
                Debug.Log("[ItemUsageManager] Inventory changed notification received");
        }
        #endregion

        #region Input System Integration (Future)
        // Métodos para integração futura com Input System
        public void OnUseItem1()
        {
            UseItemFromSlot(0);
        }

        public void OnUseItem2()
        {
            UseItemFromSlot(1);
        }

        public void OnUseItem3()
        {
            UseItemFromSlot(2);
        }

        public void OnUseItem4()
        {
            UseItemFromSlot(3);
        }
        #endregion

        #region Debug & Editor
        [ContextMenu("Debug - Use Slot 1")]
        private void DebugUseSlot1()
        {
            UseItemFromSlot(0);
        }

        [ContextMenu("Debug - Use Slot 2")]
        private void DebugUseSlot2()
        {
            UseItemFromSlot(1);
        }

        [ContextMenu("Debug - Use Slot 3")]
        private void DebugUseSlot3()
        {
            UseItemFromSlot(2);
        }

        [ContextMenu("Debug - Use Slot 4")]
        private void DebugUseSlot4()
        {
            UseItemFromSlot(3);
        }
        #endregion
    }
}