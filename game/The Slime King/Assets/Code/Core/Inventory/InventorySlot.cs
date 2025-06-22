using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Core;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Representa um slot visual no inventário
    /// </summary>
    public class InventorySlot : MonoBehaviour
    {
        [Header("Componentes UI")]
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _quantityText;
        [SerializeField] private GameObject _selectionIndicator;
        [SerializeField] private GameObject _emptySlotIndicator;
        [SerializeField] private LocalizedText _tooltipText;

        [Header("Efeitos Visuais")]
        [SerializeField] private Image _rarityBorder;
        [SerializeField] private Image _slotBackground;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _selectedColor = Color.yellow;
        [SerializeField] private float _pulseSpeed = 2f;
        [SerializeField] private float _pulseAmount = 0.2f;

        [Header("Configuração")]
        [SerializeField] private int _slotIndex;
        [SerializeField] private bool _isInteractable = true;

        // Referências internas
        private InventoryItem _currentItem;
        private bool _isSelected;
        private bool _isPulsing;
        private float _pulseTimer;

        private void OnEnable()
        {
            if (InventoryManager.Instance != null)
            {
                // Registra nos eventos do inventário
                InventoryManager.Instance.OnInventoryChanged += HandleInventoryChanged;
                InventoryManager.Instance.OnSelectedSlotChanged += HandleSelectedSlotChanged;

                // Atualiza no início
                InitializeSlot();
            }
        }

        private void OnDisable()
        {
            if (InventoryManager.Instance != null)
            {
                // Cancela registro nos eventos
                InventoryManager.Instance.OnInventoryChanged -= HandleInventoryChanged;
                InventoryManager.Instance.OnSelectedSlotChanged -= HandleSelectedSlotChanged;
            }
        }

        private void Update()
        {
            // Efeito de pulsação para slot selecionado
            if (_isPulsing && _isSelected)
            {
                _pulseTimer += Time.deltaTime * _pulseSpeed;
                float pulse = 1f + Mathf.Sin(_pulseTimer) * _pulseAmount;

                // Aplica escala de pulsação
                _selectionIndicator.transform.localScale = new Vector3(pulse, pulse, 1f);
            }
        }

        /// <summary>
        /// Inicializa o slot com valores do InventoryManager
        /// </summary>
        public void InitializeSlot()
        {
            if (InventoryManager.Instance == null)
                return;

            // Atualiza conteúdo do slot
            _currentItem = InventoryManager.Instance.GetItemAt(_slotIndex);
            UpdateVisuals();

            // Verifica se o slot está selecionado
            bool isSelected = InventoryManager.Instance.GetSelectedSlotIndex() == _slotIndex;
            SetSelected(isSelected);
        }

        /// <summary>
        /// Atualiza os elementos visuais do slot baseado no item atual
        /// </summary>
        private void UpdateVisuals()
        {
            // Verifica se há um item no slot
            bool hasItem = _currentItem != null && !_currentItem.IsEmpty;

            // Atualiza ícone
            _itemIcon.sprite = hasItem ? _currentItem.ItemData.Icon : null;
            _itemIcon.enabled = hasItem;

            // Atualiza quantidade
            _quantityText.text = hasItem && _currentItem.Quantity > 1 ? _currentItem.Quantity.ToString() : string.Empty;
            _quantityText.enabled = hasItem && _currentItem.Quantity > 1;

            // Atualiza marcador de slot vazio
            if (_emptySlotIndicator != null)
            {
                _emptySlotIndicator.SetActive(!hasItem);
            }

            // Atualiza borda de raridade se houver
            if (_rarityBorder != null && hasItem)
            {
                _rarityBorder.enabled = true;
                // Cor baseada na raridade
                switch (_currentItem.ItemData.Rarity)
                {
                    case ItemRarity.Common:
                        _rarityBorder.color = new Color(0.7f, 0.7f, 0.7f);
                        break;
                    case ItemRarity.Uncommon:
                        _rarityBorder.color = new Color(0.0f, 0.8f, 0.0f);
                        break;
                    case ItemRarity.Rare:
                        _rarityBorder.color = new Color(0.0f, 0.4f, 0.8f);
                        break;
                    case ItemRarity.Epic:
                        _rarityBorder.color = new Color(0.6f, 0.0f, 0.8f);
                        break;
                    case ItemRarity.Legendary:
                        _rarityBorder.color = new Color(1.0f, 0.5f, 0.0f);
                        break;
                }
            }
            else if (_rarityBorder != null)
            {
                _rarityBorder.enabled = false;
            }

            // Atualiza tooltip se houver
            if (_tooltipText != null && hasItem)
            {
                _tooltipText.SetKey(_currentItem.ItemData.NameKey);
            }
        }

        /// <summary>
        /// Define o estado de seleção do slot
        /// </summary>
        public void SetSelected(bool selected)
        {
            _isSelected = selected;

            if (_selectionIndicator != null)
            {
                _selectionIndicator.SetActive(selected);

                // Inicia a pulsação
                if (selected)
                {
                    _isPulsing = true;
                    _pulseTimer = 0f;
                }
                else
                {
                    _isPulsing = false;
                    _selectionIndicator.transform.localScale = Vector3.one;
                }
            }

            // Atualiza cor do background se necessário
            if (_slotBackground != null)
            {
                _slotBackground.color = selected ? _selectedColor : _normalColor;
            }
        }

        #region Event Handlers

        private void HandleInventoryChanged(System.Collections.Generic.List<InventoryItem> items)
        {
            if (_slotIndex < items.Count)
            {
                _currentItem = items[_slotIndex];
                UpdateVisuals();
            }
        }

        private void HandleSelectedSlotChanged(int selectedIndex)
        {
            SetSelected(_slotIndex == selectedIndex);
        }

        #endregion

        #region UI Interaction

        /// <summary>
        /// Chamado quando o slot é clicado
        /// </summary>
        public void OnSlotClicked()
        {
            if (!_isInteractable || InventoryManager.Instance == null)
                return;

            // Seleciona este slot
            InventoryManager.Instance.SelectSlot(_slotIndex);
        }

        /// <summary>
        /// Chamado quando o jogador faz double-click ou pressiona botão de usar
        /// </summary>
        public void OnSlotUseClicked()
        {
            if (!_isInteractable || InventoryManager.Instance == null)
                return;

            // Seleciona e usa este slot
            InventoryManager.Instance.SelectSlot(_slotIndex);
            InventoryManager.Instance.UseSelectedItem();
        }

        /// <summary>
        /// Chamado quando o mouse entra no slot (hover)
        /// </summary>
        public void OnPointerEnter()
        {
            // Adicionar código para mostrar tooltip ou highlight
        }

        /// <summary>
        /// Chamado quando o mouse sai do slot
        /// </summary>
        public void OnPointerExit()
        {
            // Adicionar código para esconder tooltip ou highlight
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Define o índice deste slot
        /// </summary>
        public void SetSlotIndex(int index)
        {
            _slotIndex = index;
            InitializeSlot();
        }

        /// <summary>
        /// Define se o slot pode ser interagido
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;

            // Adicionar visual de slot desabilitado se necessário
        }

        #endregion
    }
}
