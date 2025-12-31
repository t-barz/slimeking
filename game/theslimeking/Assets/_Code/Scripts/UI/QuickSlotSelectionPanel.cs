using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Painel para selecionar qual quick slot (direcional) atribuir um item.
    /// </summary>
    public class QuickSlotSelectionPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;

        [Header("Direction Buttons")]
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button cancelButton;

        private ItemData currentItem;

        private void Awake()
        {
            // Configura os listeners dos botões
            if (upButton != null)
            {
                upButton.onClick.AddListener(() => OnDirectionSelected(0));
            }

            if (downButton != null)
            {
                downButton.onClick.AddListener(() => OnDirectionSelected(1));
            }

            if (leftButton != null)
            {
                leftButton.onClick.AddListener(() => OnDirectionSelected(2));
            }

            if (rightButton != null)
            {
                rightButton.onClick.AddListener(() => OnDirectionSelected(3));
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
        /// Mostra o painel de seleção para um item.
        /// </summary>
        /// <param name="item">Item a ser atribuído</param>
        public void Show(ItemData item)
        {
            if (item == null)
            {
                return;
            }

            currentItem = item;

            // Atualiza o título
            if (titleText != null)
            {
                titleText.text = $"Atribuir {item.itemName} a:";
            }

            // Mostra o painel
            if (panel != null)
            {
                panel.SetActive(true);
            }
        }

        /// <summary>
        /// Oculta o painel de seleção.
        /// </summary>
        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }

            currentItem = null;
        }

        /// <summary>
        /// Callback quando uma direção é selecionada.
        /// </summary>
        /// <param name="direction">Direção (0=Up, 1=Down, 2=Left, 3=Right)</param>
        private void OnDirectionSelected(int direction)
        {
            if (currentItem == null || InventoryManager.Instance == null)
            {
                return;
            }

            // Atribui o item ao quick slot
            InventoryManager.Instance.AssignQuickSlot(currentItem, direction);

            Hide();
        }

        private void OnDestroy()
        {
            if (upButton != null)
            {
                upButton.onClick.RemoveAllListeners();
            }

            if (downButton != null)
            {
                downButton.onClick.RemoveAllListeners();
            }

            if (leftButton != null)
            {
                leftButton.onClick.RemoveAllListeners();
            }

            if (rightButton != null)
            {
                rightButton.onClick.RemoveAllListeners();
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(Hide);
            }
        }
    }
}
