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
        private Image slotBackgroundImage;

        [Header("Selection Settings")]
        [SerializeField] private Color selectedColor = new Color(1f, 0.9f, 0.5f, 0.95f);  // Amarelo claro
        [SerializeField] private Color deselectedColor = new Color(1f, 0.8f, 0.37f, 0.78f); // Amarelo original

        private int slotIndex;
        private InventorySlot currentSlot;
        private bool isSelected = false;

        private void Awake()
        {
            // Obtém a imagem de fundo do slot
            slotBackgroundImage = GetComponent<Image>();

            // Auto-configura referências se não estiverem atribuídas
            if (iconImage == null)
            {
                // Procura por um componente Image filho chamado "Icon" ou o primeiro Image encontrado
                Image[] images = GetComponentsInChildren<Image>(true);
                foreach (var img in images)
                {
                    // Ignora a imagem de fundo do próprio slot
                    if (img.gameObject != gameObject)
                    {
                        iconImage = img;
                        UnityEngine.Debug.Log($"[InventorySlotUI] Auto-configurado iconImage: {img.gameObject.name}");
                        break;
                    }
                }

                if (iconImage == null)
                {
                    UnityEngine.Debug.LogError($"[InventorySlotUI] Não foi possível encontrar Image para iconImage em {gameObject.name}!");
                }
            }

            if (quantityText == null)
            {
                quantityText = GetComponentInChildren<TextMeshProUGUI>(true);
                if (quantityText != null)
                {
                    UnityEngine.Debug.Log($"[InventorySlotUI] Auto-configurado quantityText: {quantityText.gameObject.name}");
                }
            }

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
        /// Sistema não empilhável: quantidade nunca é exibida.
        /// </summary>
        public void Refresh()
        {
            // Tenta reconfigurar iconImage se estiver null
            if (iconImage == null)
            {
                Image[] images = GetComponentsInChildren<Image>(true);
                foreach (var img in images)
                {
                    if (img.gameObject != gameObject)
                    {
                        iconImage = img;
                        UnityEngine.Debug.LogWarning($"[InventorySlotUI] iconImage estava NULL, reconfigurado para: {img.gameObject.name}");
                        break;
                    }
                }
            }

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
                    // Verifica se o item tem ícone
                    if (currentSlot.item.icon != null)
                    {
                        iconImage.enabled = true;
                        iconImage.sprite = currentSlot.item.icon;
                    }
                    else
                    {
                        // Item sem ícone - oculta imagem
                        iconImage.enabled = false;
                        UnityEngine.Debug.LogWarning($"[InventorySlotUI] ⚠️ Item '{currentSlot.item.itemName}' não possui ícone configurado!");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"[InventorySlotUI] ❌ iconImage é NULL no slot {slotIndex} e não foi possível reconfigurar! Verifique a hierarquia do prefab.");
                }

                if (quantityText != null)
                {
                    // Sistema não empilhável: nunca exibe quantidade
                    quantityText.text = "";
                }
            }
        }

        /// <summary>
        /// Define o estado de seleção visual do slot.
        /// </summary>
        public void SetSelected(bool selected)
        {
            isSelected = selected;

            if (slotBackgroundImage != null)
            {
                slotBackgroundImage.color = selected ? selectedColor : deselectedColor;
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
