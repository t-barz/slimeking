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

        [Header("Quick Slot Configuration")]
        [SerializeField] private int slotDirection; // 0=Up, 1=Down, 2=Left, 3=Right

        /// <summary>
        /// Atualiza a visualização do quick slot com os dados atuais do InventoryManager.
        /// Sistema não empilhável: cada slot é independente e mostra apenas o ícone.
        /// </summary>
        public void Refresh()
        {
            if (InventoryManager.Instance == null)
            {
                Debug.LogWarning("QuickSlotUI: InventoryManager.Instance não encontrado.");
                return;
            }

            ItemData item = InventoryManager.Instance.GetQuickSlotItem(slotDirection);

            Debug.Log($"[QuickSlotUI] Refresh slot {slotDirection}: item={(item != null ? item.itemName : "NULL")}");

            if (item == null)
            {
                // Quick slot vazio
                if (iconImage != null)
                {
                    iconImage.enabled = false;
                }
            }
            else
            {
                // Quick slot com item - sistema não empilhável: sempre mostra sem quantidade
                if (iconImage != null)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = item.icon;
                }
            }
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
