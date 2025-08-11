using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlimeKing.Core.Inventory.UI
{
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private GameObject itemContainer;
        [SerializeField] private GameObject emptySlotOverlay;

        private InventorySlot slot;

        public void Initialize(InventorySlot inventorySlot)
        {
            if (slot != null)
                slot.OnUpdated -= UpdateUI;

            slot = inventorySlot;

            if (slot != null)
                slot.OnUpdated += UpdateUI;

            UpdateUI();
        }

        private void OnDestroy()
        {
            if (slot != null)
                slot.OnUpdated -= UpdateUI;
        }

        private void UpdateUI()
        {
            if (slot == null || slot.IsEmpty)
            {
                itemContainer.SetActive(false);
                emptySlotOverlay.SetActive(true);
                return;
            }

            itemContainer.SetActive(true);
            emptySlotOverlay.SetActive(false);

            iconImage.sprite = slot.Item.Icon;

            if (slot.Item.IsStackable && slot.Amount > 1)
            {
                amountText.gameObject.SetActive(true);
                amountText.text = slot.Amount.ToString();
            }
            else
            {
                amountText.gameObject.SetActive(false);
            }
        }
    }
}
