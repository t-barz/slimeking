using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Representa visualmente um quick slot individual no HUD.
    /// Exibe ícone do item e quantidade.
    /// </summary>
    public class QuickSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI quantityText;

        [Header("Quick Slot Configuration")]
        [SerializeField] private int slotDirection; // 0=Up, 1=Down, 2=Left, 3=Right

        /// <summary>
        /// Atualiza a visualização do quick slot com os dados atuais do InventoryManager.
        /// </summary>
        public void Refresh()
        {
            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("QuickSlotUI: InventoryManager.Instance não encontrado.");
                return;
            }

            ItemData item = InventoryManager.Instance.GetQuickSlotItem(slotDirection);

            if (item == null)
            {
                // Quick slot vazio
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
                // Quick slot com item
                if (iconImage != null)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = item.icon;
                }

                // Atualiza quantidade do item no inventário
                if (quantityText != null)
                {
                    int quantity = GetItemQuantityInInventory(item);
                    
                    if (quantity > 1)
                    {
                        quantityText.text = quantity.ToString();
                    }
                    else if (quantity == 1)
                    {
                        quantityText.text = "";
                    }
                    else
                    {
                        // Item não existe mais no inventário
                        quantityText.text = "";
                    }
                }
            }
        }

        /// <summary>
        /// Obtém a quantidade total de um item no inventário.
        /// </summary>
        /// <param name="item">Item a verificar</param>
        /// <returns>Quantidade total</returns>
        private int GetItemQuantityInInventory(ItemData item)
        {
            if (item == null || InventoryManager.Instance == null)
            {
                return 0;
            }

            int totalQuantity = 0;
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();

            foreach (InventorySlot slot in slots)
            {
                if (slot.item == item)
                {
                    totalQuantity += slot.quantity;
                }
            }

            return totalQuantity;
        }

        /// <summary>
        /// Define a direção do quick slot.
        /// </summary>
        /// <param name="direction">Direção (0=Up, 1=Down, 2=Left, 3=Right)</param>
        public void SetDirection(int direction)
        {
            slotDirection = direction;
            Refresh();
        }

        /// <summary>
        /// Obtém a direção do quick slot.
        /// </summary>
        /// <returns>Direção (0=Up, 1=Down, 2=Left, 3=Right)</returns>
        public int GetDirection()
        {
            return slotDirection;
        }
    }
}
