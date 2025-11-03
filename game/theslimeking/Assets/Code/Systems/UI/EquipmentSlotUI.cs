using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Representa visualmente um slot de equipamento (Amuleto, Anel, Capa).
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private Button button;

        [Header("Configuration")]
        [SerializeField] private EquipmentType equipmentType;

        private ItemData currentItem;

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

            // Define o label baseado no tipo
            if (labelText != null)
            {
                labelText.text = GetEquipmentLabel(equipmentType);
            }
        }

        /// <summary>
        /// Atualiza a visualização do slot de equipamento.
        /// </summary>
        /// <param name="item">Item equipado ou null se vazio</param>
        public void Refresh(ItemData item)
        {
            currentItem = item;

            if (iconImage != null)
            {
                if (item != null)
                {
                    iconImage.enabled = true;
                    iconImage.sprite = item.icon;
                }
                else
                {
                    iconImage.enabled = false;
                }
            }
        }

        /// <summary>
        /// Callback quando o slot de equipamento é clicado.
        /// </summary>
        private void OnClick()
        {
            if (currentItem != null)
            {
                // Desequipa o item
                InventoryManager.Instance.UnequipItem(equipmentType);
            }
        }

        /// <summary>
        /// Retorna o label apropriado para cada tipo de equipamento.
        /// </summary>
        private string GetEquipmentLabel(EquipmentType type)
        {
            switch (type)
            {
                case EquipmentType.Amulet:
                    return "Amuleto";
                case EquipmentType.Ring:
                    return "Anel";
                case EquipmentType.Cape:
                    return "Capa";
                default:
                    return "";
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
