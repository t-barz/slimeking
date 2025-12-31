using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Painel popup que exibe ações disponíveis para um item selecionado.
    /// Mostra botões: Usar/Equipar, Atribuir, Descartar.
    /// </summary>
    public class ItemActionPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI itemNameText;

        [Header("Action Buttons")]
        [SerializeField] private Button useButton;
        [SerializeField] private TextMeshProUGUI useButtonText;
        [SerializeField] private Button assignButton;
        [SerializeField] private Button discardButton;
        [SerializeField] private Button cancelButton;

        private ItemData currentItem;
        private int currentSlotIndex;

        private void Awake()
        {
            // Configura os listeners dos botões
            if (useButton != null)
            {
                useButton.onClick.AddListener(OnUseClicked);
            }

            if (assignButton != null)
            {
                assignButton.onClick.AddListener(OnAssignClicked);
            }

            if (discardButton != null)
            {
                discardButton.onClick.AddListener(OnDiscardClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(Hide);
            }

            // Começa oculto
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        /// <summary>
        /// Mostra o painel de ações para um item específico.
        /// </summary>
        /// <param name="item">Item selecionado</param>
        /// <param name="slotIndex">Índice do slot do item</param>
        public void Show(ItemData item, int slotIndex)
        {
            if (item == null)
            {
                return;
            }

            currentItem = item;
            currentSlotIndex = slotIndex;

            // Atualiza o nome do item
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName;
            }

            // Configura os botões baseado no tipo de item
            ConfigureButtons(item);

            // Mostra o painel
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        /// <summary>
        /// Oculta o painel de ações.
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }

            currentItem = null;
            currentSlotIndex = -1;
        }

        /// <summary>
        /// Configura a visibilidade e texto dos botões baseado no tipo de item.
        /// </summary>
        private void ConfigureButtons(ItemData item)
        {
            // Botão Usar/Equipar
            if (useButton != null)
            {
                if (item.type == ItemType.Consumable)
                {
                    useButton.gameObject.SetActive(true);
                    if (useButtonText != null)
                    {
                        useButtonText.text = "Usar";
                    }
                }
                else if (item.type == ItemType.Equipment)
                {
                    useButton.gameObject.SetActive(true);
                    if (useButtonText != null)
                    {
                        useButtonText.text = "Equipar";
                    }
                }
                else
                {
                    // Material e Quest items não podem ser usados diretamente
                    useButton.gameObject.SetActive(false);
                }
            }

            // Botão Atribuir (apenas para consumíveis)
            if (assignButton != null)
            {
                assignButton.gameObject.SetActive(item.type == ItemType.Consumable);
            }

            // Botão Descartar (desabilitado para quest items)
            if (discardButton != null)
            {
                discardButton.gameObject.SetActive(true);
                discardButton.interactable = !item.isQuestItem;
            }
        }

        /// <summary>
        /// Callback quando o botão Usar/Equipar é clicado.
        /// </summary>
        private void OnUseClicked()
        {
            if (currentItem == null || InventoryManager.Instance == null)
            {
                return;
            }

            if (currentItem.type == ItemType.Consumable)
            {
                // Usa o item consumível
                InventoryManager.Instance.UseItem(currentSlotIndex);
            }
            else if (currentItem.type == ItemType.Equipment)
            {
                // Equipa o item
                InventoryManager.Instance.EquipItem(currentItem);
            }

            Hide();
        }

        /// <summary>
        /// Callback quando o botão Atribuir é clicado.
        /// Abre o painel de seleção de quick slot.
        /// </summary>
        private void OnAssignClicked()
        {
            if (currentItem == null)
            {
                return;
            }

            // Abre o painel de seleção de quick slot
            QuickSlotSelectionPanel selectionPanel = FindObjectOfType<QuickSlotSelectionPanel>();
            if (selectionPanel != null)
            {
                selectionPanel.Show(currentItem);
            }
            else
            {
                Debug.LogWarning("ItemActionPanel: QuickSlotSelectionPanel não encontrado na cena.");
            }

            Hide();
        }

        /// <summary>
        /// Callback quando o botão Descartar é clicado.
        /// Mostra confirmação antes de descartar.
        /// </summary>
        private void OnDiscardClicked()
        {
            if (currentItem == null || InventoryManager.Instance == null)
            {
                return;
            }

            // Verifica se é quest item (não deveria chegar aqui, mas por segurança)
            if (currentItem.isQuestItem)
            {
                Debug.LogWarning($"ItemActionPanel: Tentativa de descartar quest item '{currentItem.itemName}'.");
                Hide();
                return;
            }

            // Mostra confirmação
            ConfirmationDialog confirmDialog = FindObjectOfType<ConfirmationDialog>();
            if (confirmDialog != null)
            {
                confirmDialog.Show(
                    $"Descartar {currentItem.itemName}?",
                    () => {
                        // Confirmado - descarta o item
                        InventoryManager.Instance.DiscardItem(currentSlotIndex);
                    }
                );
            }
            else
            {
                // Se não houver diálogo de confirmação, descarta diretamente
                InventoryManager.Instance.DiscardItem(currentSlotIndex);
            }

            Hide();
        }

        private void OnDestroy()
        {
            if (useButton != null)
            {
                useButton.onClick.RemoveListener(OnUseClicked);
            }

            if (assignButton != null)
            {
                assignButton.onClick.RemoveListener(OnAssignClicked);
            }

            if (discardButton != null)
            {
                discardButton.onClick.RemoveListener(OnDiscardClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(Hide);
            }
        }
    }
}
