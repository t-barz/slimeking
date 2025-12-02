using UnityEngine;
using UnityEngine.UI;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Controla a interface visual do inventário.
    /// Gerencia grid de 20 slots, 3 slots de equipamento e painel de ações.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject actionPanel;

        [Header("Inventory Slots")]
        [SerializeField] private Transform slotsContainer;
        [SerializeField] private GameObject slotPrefab;
        private InventorySlotUI[] slotUIs = new InventorySlotUI[20];

        [Header("Equipment Slots")]
        [SerializeField] private EquipmentSlotUI amuletSlot;
        [SerializeField] private EquipmentSlotUI ringSlot;
        [SerializeField] private EquipmentSlotUI capeSlot;

        [Header("Close Button")]
        [SerializeField] private Button closeButton;

        private bool isInitialized = false;

        private void Awake()
        {
            // Configura o botão de fechar
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            // Inicializa os slots
            InitializeSlots();

            // Começa oculto
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            if (actionPanel != null)
            {
                actionPanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Inscreve-se nos eventos do InventoryManager
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += RefreshAll;
                InventoryManager.Instance.OnEquipmentChanged += RefreshEquipment;
            }
        }

        private void OnDisable()
        {
            // Desinscreve-se dos eventos
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= RefreshAll;
                InventoryManager.Instance.OnEquipmentChanged -= RefreshEquipment;
            }
        }

        /// <summary>
        /// Inicializa os 20 slots do inventário.
        /// </summary>
        private void InitializeSlots()
        {
            if (isInitialized || slotsContainer == null || slotPrefab == null)
            {
                return;
            }

            // Cria 20 slots
            for (int i = 0; i < 20; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
                InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();

                if (slotUI != null)
                {
                    slotUIs[i] = slotUI;
                }
                else
                {
                    Debug.LogError($"InventoryUI: Slot prefab não possui componente InventorySlotUI!");
                }
            }

            isInitialized = true;
        }

        /// <summary>
        /// Mostra o inventário e pausa o jogo.
        /// </summary>
        public void Show()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }

            // Pausa o jogo
            Time.timeScale = 0f;

            // Atualiza todos os slots
            RefreshAll();
        }

        /// <summary>
        /// Oculta o inventário e despausa o jogo.
        /// </summary>
        public void Hide()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            if (actionPanel != null)
            {
                actionPanel.SetActive(false);
            }

            // Despausa o jogo
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Atualiza todos os slots do inventário e equipamentos.
        /// </summary>
        public void RefreshAll()
        {
            if (!isInitialized)
            {
                InitializeSlots();
            }

            // Atualiza slots do inventário
            for (int i = 0; i < slotUIs.Length; i++)
            {
                if (slotUIs[i] != null && InventoryManager.Instance != null)
                {
                    // Obtém o slot do InventoryManager via reflection ou método público
                    // Por enquanto, vamos criar um método público no InventoryManager
                    slotUIs[i].Setup(GetInventorySlot(i), i);
                }
            }

            // Atualiza equipamentos
            RefreshEquipment();
        }

        /// <summary>
        /// Atualiza apenas os slots de equipamento.
        /// </summary>
        private void RefreshEquipment()
        {
            if (InventoryManager.Instance == null)
            {
                return;
            }

            if (amuletSlot != null)
            {
                amuletSlot.Refresh(InventoryManager.Instance.GetEquippedItem(EquipmentType.Amulet));
            }

            if (ringSlot != null)
            {
                ringSlot.Refresh(InventoryManager.Instance.GetEquippedItem(EquipmentType.Ring));
            }

            if (capeSlot != null)
            {
                capeSlot.Refresh(InventoryManager.Instance.GetEquippedItem(EquipmentType.Cape));
            }
        }

        /// <summary>
        /// Callback quando um slot é clicado.
        /// Abre o painel de ações para o item.
        /// </summary>
        /// <param name="slotIndex">Índice do slot clicado</param>
        public void OnSlotClicked(int slotIndex)
        {
            InventorySlot slot = GetInventorySlot(slotIndex);

            if (slot == null || slot.IsEmpty)
            {
                return;
            }

            // Abre o painel de ações
            ItemActionPanel actionPanelComponent = actionPanel?.GetComponent<ItemActionPanel>();
            if (actionPanelComponent != null)
            {
                actionPanelComponent.Show(slot.item, slotIndex);
            }
        }

        /// <summary>
        /// Obtém um slot do inventário pelo índice.
        /// </summary>
        private InventorySlot GetInventorySlot(int index)
        {
            if (InventoryManager.Instance == null)
            {
                return null;
            }

            return InventoryManager.Instance.GetSlot(index);
        }

        private void OnDestroy()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveListener(Hide);
            }
        }
    }
}
