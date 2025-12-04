using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Inventory;
using SlimeKing.UI;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Representa visualmente um slot individual do inventário.
    /// Exibe ícone do item e quantidade.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI quantityText;
        [SerializeField] private Button button;

        private int slotIndex;
        private InventorySlot currentSlot;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }

        /// <summary>
        /// Configura o slot com dados e índice.
        /// </summary>
        /// <param name="slot">Dados do slot</param>
        /// <param name="index">Índice do slot no inventário</param>
        public void Setup(InventorySlot slot, int index)
        {
            currentSlot = slot;
            slotIndex = index;
            Refresh();
        }

        /// <summary>
        /// Atualiza a visualização do slot com os dados atuais.
        /// </summary>
        public void Refresh()
        {
            if (currentSlot == null || currentSlot.IsEmpty)
            {
                // Slot vazio
                if (iconImage != null)
                {
                    iconImage.enabled = false;
                }

                if (quantityText != null)
                {
                    quantityText.text = "";
                }
            }
            else
            {
                // Slot com item
                if (iconImage != null)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = currentSlot.item.icon;
                }

                if (quantityText != null)
                {
                    // Mostra quantidade apenas se for maior que 1
                    if (currentSlot.quantity > 1)
                    {
                        quantityText.text = currentSlot.quantity.ToString();
                    }
                    else
                    {
                        quantityText.text = "";
                    }
                }
            }
        }

        /// <summary>
        /// Callback quando o slot é clicado.
        /// </summary>
        private void OnClick()
        {
            // Notifica o InventoryUI que este slot foi clicado
            InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
            if (inventoryUI != null)
            {
                inventoryUI.OnSlotClicked(slotIndex);
            }
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClick);
            }
        }
    }
}
