using UnityEngine;
using System.Collections.Generic;
using System;

namespace SlimeMec.Alpha.Inventory
{
    /// <summary>
    /// Sistema core de inventário para Demo Alpha
    /// Gerencia estrutura básica de itens, coleta e integração com HUD
    /// </summary>
    public class InventoryCore : MonoBehaviour
    {
        #region Singleton
        public static InventoryCore Instance { get; private set; }
        #endregion

        #region Events
        public static event Action OnInventoryChanged;
        public static event Action<InventoryItem> OnItemAdded;
        public static event Action<InventoryItem> OnItemRemoved;
        public static event Action<InventoryItem> OnItemUsed;
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private int maxConsumableSlots = 4;
        [SerializeField] private bool autoPickupEnabled = true;
        [SerializeField] private float autoPickupRange = 2f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        #endregion

        #region Private Fields
        private List<InventoryItem> consumableSlots = new List<InventoryItem>();
        private Dictionary<string, InventoryItemData> itemDatabase = new Dictionary<string, InventoryItemData>();
        private bool isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSingleton();
            InitializeInventory();
        }

        private void Start()
        {
            SetupIntegrations();
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

        private void InitializeInventory()
        {
            // Inicializa slots consumíveis
            consumableSlots = new List<InventoryItem>(maxConsumableSlots);
            for (int i = 0; i < maxConsumableSlots; i++)
            {
                consumableSlots.Add(null);
            }

            // Inicializa database básico de itens
            InitializeItemDatabase();

            isInitialized = true;

            if (enableDebugLogs)
                Debug.Log($"[InventoryCore] Initialized with {maxConsumableSlots} consumable slots");
        }

        private void InitializeItemDatabase()
        {
            // Itens básicos para demonstração
            RegisterItem("health_potion", "Health Potion", "Restores 50 HP", ItemType.Consumable);
            RegisterItem("mana_potion", "Mana Potion", "Restores 30 MP", ItemType.Consumable);
            RegisterItem("speed_boost", "Speed Boost", "Increases speed for 10s", ItemType.Consumable);
            RegisterItem("crystal", "Crystal", "Valuable crystal", ItemType.Material);
        }

        private void RegisterItem(string id, string displayName, string description, ItemType type)
        {
            var itemData = new InventoryItemData
            {
                id = id,
                displayName = displayName,
                description = description,
                type = type
            };

            itemDatabase[id] = itemData;
        }

        private void SetupIntegrations()
        {
            // Setup integração com sistemas existentes
            SetupItemCollectionIntegration();
            SetupHUDIntegration();
        }

        private void SetupItemCollectionIntegration()
        {
            // Integração com sistema de coleta será feita via AlphaItemAdapter
        }

        private void SetupHUDIntegration()
        {
            // HUD vai se conectar via eventos
        }

        private void CleanupEvents()
        {
            OnInventoryChanged = null;
            OnItemAdded = null;
            OnItemRemoved = null;
            OnItemUsed = null;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Adiciona item ao inventário
        /// </summary>
        public bool AddItem(string itemId, int quantity = 1)
        {
            if (!itemDatabase.ContainsKey(itemId))
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[InventoryCore] Unknown item ID: {itemId}");
                return false;
            }

            var itemData = itemDatabase[itemId];

            // Procura slot vazio ou existente com o mesmo item
            int targetSlot = FindAvailableSlot(itemId);
            if (targetSlot == -1)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[InventoryCore] No available slot for item: {itemId}");
                return false;
            }

            // Adiciona ao slot
            if (consumableSlots[targetSlot] == null)
            {
                consumableSlots[targetSlot] = new InventoryItem
                {
                    id = itemId,
                    displayName = itemData.displayName,
                    description = itemData.description,
                    type = itemData.type,
                    quantity = quantity
                };
            }
            else
            {
                consumableSlots[targetSlot].quantity += quantity;
            }

            // Notifica mudanças
            OnItemAdded?.Invoke(consumableSlots[targetSlot]);
            OnInventoryChanged?.Invoke();

            if (enableDebugLogs)
                Debug.Log($"[InventoryCore] Added {quantity}x {itemId} to slot {targetSlot}");

            return true;
        }

        /// <summary>
        /// Remove item do inventário
        /// </summary>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            int slot = FindItemSlot(itemId);
            if (slot == -1) return false;

            var item = consumableSlots[slot];
            if (item.quantity <= quantity)
            {
                // Remove completamente
                OnItemRemoved?.Invoke(item);
                consumableSlots[slot] = null;
            }
            else
            {
                // Remove parcialmente
                item.quantity -= quantity;
            }

            OnInventoryChanged?.Invoke();

            if (enableDebugLogs)
                Debug.Log($"[InventoryCore] Removed {quantity}x {itemId}");

            return true;
        }

        /// <summary>
        /// Usa item consumível
        /// </summary>
        public bool UseItem(string itemId)
        {
            int slot = FindItemSlot(itemId);
            if (slot == -1) return false;

            var item = consumableSlots[slot];
            if (item.type != ItemType.Consumable)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[InventoryCore] Item {itemId} is not consumable");
                return false;
            }

            // Notifica uso antes de remover
            OnItemUsed?.Invoke(item);

            // Remove uma unidade
            return RemoveItem(itemId, 1);
        }

        /// <summary>
        /// Pega item de slot específico
        /// </summary>
        public InventoryItem GetConsumableItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxConsumableSlots) return null;
            return consumableSlots[slotIndex];
        }

        /// <summary>
        /// Retorna todos os itens no inventário
        /// </summary>
        public List<InventoryItem> GetAllItems()
        {
            var items = new List<InventoryItem>();
            foreach (var item in consumableSlots)
            {
                if (item != null)
                    items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// Verifica se tem espaço disponível
        /// </summary>
        public bool HasAvailableSpace()
        {
            return FindEmptySlot() != -1;
        }

        /// <summary>
        /// Conta quantidade de um item específico
        /// </summary>
        public int GetItemCount(string itemId)
        {
            int total = 0;
            foreach (var item in consumableSlots)
            {
                if (item != null && item.id == itemId)
                    total += item.quantity;
            }
            return total;
        }
        #endregion

        #region Helper Methods
        private int FindAvailableSlot(string itemId)
        {
            // Primeiro procura slot com o mesmo item (stackable)
            for (int i = 0; i < consumableSlots.Count; i++)
            {
                var item = consumableSlots[i];
                if (item != null && item.id == itemId)
                    return i;
            }

            // Se não encontrou, procura slot vazio
            return FindEmptySlot();
        }

        private int FindEmptySlot()
        {
            for (int i = 0; i < consumableSlots.Count; i++)
            {
                if (consumableSlots[i] == null)
                    return i;
            }
            return -1;
        }

        private int FindItemSlot(string itemId)
        {
            for (int i = 0; i < consumableSlots.Count; i++)
            {
                var item = consumableSlots[i];
                if (item != null && item.id == itemId)
                    return i;
            }
            return -1;
        }
        #endregion

        #region Debug & Editor
        [ContextMenu("Debug - Add Test Item")]
        private void DebugAddTestItem()
        {
            AddItem("health_potion", 1);
        }

        [ContextMenu("Debug - Clear Inventory")]
        private void DebugClearInventory()
        {
            for (int i = 0; i < consumableSlots.Count; i++)
            {
                consumableSlots[i] = null;
            }
            OnInventoryChanged?.Invoke();

            if (enableDebugLogs)
                Debug.Log("[InventoryCore] Inventory cleared");
        }

        private void OnDrawGizmosSelected()
        {
            if (autoPickupEnabled)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, autoPickupRange);
            }
        }
        #endregion
    }

    /// <summary>
    /// Estrutura básica de item do inventário
    /// </summary>
    [System.Serializable]
    public class InventoryItem
    {
        public string id;
        public string displayName;
        public string description;
        public ItemType type;
        public int quantity = 1;
        public Sprite icon;
    }

    /// <summary>
    /// Dados base de um tipo de item
    /// </summary>
    [System.Serializable]
    public class InventoryItemData
    {
        public string id;
        public string displayName;
        public string description;
        public ItemType type;
        public Sprite defaultIcon;
    }

    /// <summary>
    /// Tipos de itens suportados
    /// </summary>
    public enum ItemType
    {
        Consumable,     // Itens que podem ser usados (slots 1-4)
        Material,       // Materiais de crafting (futuro)
        QuestItem,      // Itens de quest
        Equipment       // Equipamentos (futuro)
    }
}