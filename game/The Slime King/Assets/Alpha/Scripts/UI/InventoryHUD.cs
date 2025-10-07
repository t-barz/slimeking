using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SlimeMec.Alpha.Inventory;

namespace Alpha.UI
{
    /// <summary>
    /// Interface visual para o sistema de inventário Alpha.
    /// Exibe slots de itens e permite interação básica com o inventário.
    /// </summary>
    public class InventoryHUD : MonoBehaviour
    {
        #region Serialized Fields
        [Header("UI References")]
        [SerializeField] private Transform slotContainer;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private int maxVisibleSlots = 4;

        [Header("Slot Bindings")]
        [SerializeField] private KeyCode[] slotKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4 };

        [Header("Visual Settings")]
        [SerializeField] private Color normalSlotColor = Color.white;
        [SerializeField] private Color selectedSlotColor = Color.yellow;
        [SerializeField] private Color emptySlotColor = Color.gray;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Fields
        private List<InventorySlotUI> slots = new List<InventorySlotUI>();
        private int selectedSlotIndex = 0;
        private bool isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSlots();
        }

        private void OnEnable()
        {
            SubscribeToEvents();
            if (isInitialized)
                RefreshAllSlots();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        private void Update()
        {
            HandleSlotSelection();
        }
        #endregion

        #region Initialization
        private void InitializeSlots()
        {
            if (slotContainer == null || slotPrefab == null)
            {
                Debug.LogError("InventoryHUD: Missing required references!");
                return;
            }

            // Limpa slots existentes
            foreach (Transform child in slotContainer)
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
            slots.Clear();

            // Cria novos slots
            for (int i = 0; i < maxVisibleSlots; i++)
            {
                CreateSlot(i);
            }

            isInitialized = true;

            if (enableDebugLogs)
                Debug.Log($"InventoryHUD: Initialized {maxVisibleSlots} slots");
        }

        private void CreateSlot(int index)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            slotObj.name = $"InventorySlot_{index + 1}";

            InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
            if (slotUI == null)
            {
                slotUI = slotObj.AddComponent<InventorySlotUI>();
            }

            slotUI.Initialize(index, GetSlotKeyText(index));
            slots.Add(slotUI);

            // Configura cores iniciais
            UpdateSlotVisuals(index);
        }

        private string GetSlotKeyText(int index)
        {
            if (index < slotKeys.Length)
            {
                return (index + 1).ToString(); // Mostra 1, 2, 3, 4
            }
            return "";
        }
        #endregion

        #region Event Management
        private void SubscribeToEvents()
        {
            if (InventoryCore.Instance != null)
            {
                InventoryCore.OnInventoryChanged += HandleInventoryChanged;
            }

            SlimeMec.Alpha.Inventory.AlphaItemAdapter.OnItemCollectedForInventory += HandleItemCollected;
        }

        private void UnsubscribeFromEvents()
        {
            if (InventoryCore.Instance != null)
            {
                InventoryCore.OnInventoryChanged -= HandleInventoryChanged;
            }

            SlimeMec.Alpha.Inventory.AlphaItemAdapter.OnItemCollectedForInventory -= HandleItemCollected;
        }
        #endregion

        #region Event Handlers
        private void HandleInventoryChanged()
        {
            RefreshAllSlots();

            if (enableDebugLogs)
                Debug.Log("InventoryHUD: Inventory changed, refreshing display");
        }

        private void HandleItemCollected(string itemId, int quantity)
        {
            // Feedback visual de coleta pode ser implementado aqui
            if (enableDebugLogs)
                Debug.Log($"InventoryHUD: Item collected feedback - {quantity}x {itemId}");

            // TODO: Adicionar efeito visual de coleta (brilho, animação, etc.)
        }
        #endregion

        #region Slot Management
        private void RefreshAllSlots()
        {
            if (!isInitialized || InventoryCore.Instance == null) return;

            var inventoryItems = InventoryCore.Instance.GetAllItems();

            for (int i = 0; i < slots.Count; i++)
            {
                if (i < inventoryItems.Count)
                {
                    var item = inventoryItems[i];
                    slots[i].SetItem(item.id, item.quantity);
                }
                else
                {
                    slots[i].SetEmpty();
                }

                UpdateSlotVisuals(i);
            }
        }

        private void UpdateSlotVisuals(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Count) return;

            var slot = slots[slotIndex];

            // Determina a cor do slot
            Color targetColor;
            if (slotIndex == selectedSlotIndex)
            {
                targetColor = selectedSlotColor;
            }
            else if (slot.IsEmpty())
            {
                targetColor = emptySlotColor;
            }
            else
            {
                targetColor = normalSlotColor;
            }

            slot.SetBackgroundColor(targetColor);
        }

        private void HandleSlotSelection()
        {
            for (int i = 0; i < slotKeys.Length && i < maxVisibleSlots; i++)
            {
                if (Input.GetKeyDown(slotKeys[i]))
                {
                    SelectSlot(i);
                    UseSelectedSlot();
                    break;
                }
            }

            // Navegação com scroll
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0.1f)
            {
                SelectNextSlot();
            }
            else if (scroll < -0.1f)
            {
                SelectPreviousSlot();
            }
        }

        public void SelectSlot(int index)
        {
            if (index < 0 || index >= maxVisibleSlots) return;

            int previousIndex = selectedSlotIndex;
            selectedSlotIndex = index;

            // Atualiza visuais
            UpdateSlotVisuals(previousIndex);
            UpdateSlotVisuals(selectedSlotIndex);

            if (enableDebugLogs)
                Debug.Log($"InventoryHUD: Selected slot {selectedSlotIndex + 1}");
        }

        public void SelectNextSlot()
        {
            int nextIndex = (selectedSlotIndex + 1) % maxVisibleSlots;
            SelectSlot(nextIndex);
        }

        public void SelectPreviousSlot()
        {
            int prevIndex = (selectedSlotIndex - 1 + maxVisibleSlots) % maxVisibleSlots;
            SelectSlot(prevIndex);
        }

        private void UseSelectedSlot()
        {
            if (InventoryCore.Instance != null)
            {
                var items = InventoryCore.Instance.GetAllItems();
                if (selectedSlotIndex < items.Count)
                {
                    string itemId = items[selectedSlotIndex].id;

                    // Delega o uso para o ItemUsageManager
                    if (ItemUsageManager.Instance != null)
                    {
                        ItemUsageManager.Instance.UseItem(itemId);
                    }
                }
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Força a atualização da interface
        /// </summary>
        public void ForceRefresh()
        {
            RefreshAllSlots();
        }

        /// <summary>
        /// Obtém o índice do slot atualmente selecionado
        /// </summary>
        public int GetSelectedSlotIndex()
        {
            return selectedSlotIndex;
        }

        /// <summary>
        /// Verifica se um slot específico está vazio
        /// </summary>
        public bool IsSlotEmpty(int index)
        {
            if (index < 0 || index >= slots.Count) return true;
            return slots[index].IsEmpty();
        }
        #endregion

        #region Editor Helper
        [ContextMenu("Refresh Slots")]
        private void EditorRefreshSlots()
        {
            if (!Application.isPlaying)
            {
                InitializeSlots();
            }
            else
            {
                RefreshAllSlots();
            }
        }
        #endregion
    }

    /// <summary>
    /// Componente individual para cada slot do inventário
    /// </summary>
    [System.Serializable]
    public class InventorySlotUI : MonoBehaviour
    {
        #region UI Components
        private Image backgroundImage;
        private Image itemImage;
        private TextMeshProUGUI quantityText;
        private TextMeshProUGUI keyText;
        #endregion

        #region Private Fields
        private int slotIndex;
        private string currentItemId = "";
        private int currentQuantity = 0;
        private bool isEmpty = true;
        #endregion

        #region Initialization
        public void Initialize(int index, string keyBinding)
        {
            slotIndex = index;
            SetupUIComponents();
            SetKeyText(keyBinding);
            SetEmpty();
        }

        private void SetupUIComponents()
        {
            // Procura ou cria componentes necessários
            backgroundImage = GetComponent<Image>();
            if (backgroundImage == null)
                backgroundImage = gameObject.AddComponent<Image>();

            // Procura por componentes filhos
            itemImage = transform.Find("ItemImage")?.GetComponent<Image>();
            quantityText = transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>();
            keyText = transform.Find("KeyText")?.GetComponent<TextMeshProUGUI>();

            // Se não encontrou, cria estrutura básica
            if (itemImage == null)
            {
                GameObject itemObj = new GameObject("ItemImage");
                itemObj.transform.SetParent(transform);
                itemImage = itemObj.AddComponent<Image>();

                RectTransform rect = itemImage.GetComponent<RectTransform>();
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.one * 4;
                rect.offsetMax = Vector2.one * -4;
            }
        }
        #endregion

        #region Public Interface
        public void SetItem(string itemId, int quantity)
        {
            currentItemId = itemId;
            currentQuantity = quantity;
            isEmpty = false;

            // Atualiza UI
            if (itemImage != null)
            {
                itemImage.enabled = true;
                // TODO: Carregar sprite do item baseado no itemId
            }

            if (quantityText != null)
            {
                quantityText.text = quantity > 1 ? quantity.ToString() : "";
                quantityText.enabled = true;
            }
        }

        public void SetEmpty()
        {
            currentItemId = "";
            currentQuantity = 0;
            isEmpty = true;

            // Atualiza UI
            if (itemImage != null)
                itemImage.enabled = false;

            if (quantityText != null)
            {
                quantityText.text = "";
                quantityText.enabled = false;
            }
        }

        public void SetBackgroundColor(Color color)
        {
            if (backgroundImage != null)
                backgroundImage.color = color;
        }

        public void SetKeyText(string text)
        {
            if (keyText != null)
                keyText.text = text;
        }

        public bool IsEmpty() => isEmpty;
        public string GetItemId() => currentItemId;
        public int GetQuantity() => currentQuantity;
        #endregion
    }
}