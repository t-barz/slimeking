using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TheSlimeKing.Core;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Controla a interface de usuário do inventário
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("Configuração")]
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private Transform _slotsContainer;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private GameObject _quickSlotsPanel;

        [Header("Animação")]
        [SerializeField] private float _openDuration = 0.3f;
        [SerializeField] private AnimationCurve _openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Localização")]
        [SerializeField] private LocalizedText _titleText;
        [SerializeField] private string _titleKey = "ui_inventory_title";

        [Header("Áudio")]
        [SerializeField] private AudioClip _openSound;
        [SerializeField] private AudioClip _closeSound;
        [SerializeField] private AudioClip _selectSound;

        // Variáveis internas
        private bool _isInventoryOpen = false;
        private List<InventorySlot> _slots = new List<InventorySlot>();
        private List<InventorySlot> _quickSlots = new List<InventorySlot>();
        private Coroutine _animationCoroutine;

        private void Awake()
        {
            // Garantir que o inventário comece fechado
            if (_inventoryPanel != null)
            {
                _inventoryPanel.SetActive(false);
            }
        }

        private void Start()
        {
            // Registrar nos eventos do InventoryManager
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged += HandleInventoryChanged;
                InventoryManager.Instance.OnSelectedSlotChanged += HandleSelectedSlotChanged;
                InventoryManager.Instance.OnAvailableSlotsChanged += HandleAvailableSlotsChanged;

                // Inicializar slots baseado no número disponível
                InitializeSlots(InventoryManager.Instance.GetAvailableSlots());
            }
            else
            {
                Debug.LogError("InventoryManager não encontrado na cena! O InventoryUI não funcionará corretamente.");
            }

            // Configurar texto localizado
            if (_titleText != null)
            {
                _titleText.SetKey(_titleKey);
            }
        }

        private void OnDestroy()
        {
            // Cancelar registro dos eventos
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged -= HandleInventoryChanged;
                InventoryManager.Instance.OnSelectedSlotChanged -= HandleSelectedSlotChanged;
                InventoryManager.Instance.OnAvailableSlotsChanged -= HandleAvailableSlotsChanged;
            }
        }

        #region UI Management

        /// <summary>
        /// Inicializa os slots do inventário
        /// </summary>
        private void InitializeSlots(int availableSlots)
        {
            // Limpar slots existentes
            ClearSlots();

            // Criar slots principais do inventário
            if (_slotsContainer != null && _slotPrefab != null)
            {
                for (int i = 0; i < availableSlots; i++)
                {
                    GameObject slotObj = Instantiate(_slotPrefab, _slotsContainer);
                    InventorySlot slot = slotObj.GetComponent<InventorySlot>();

                    if (slot != null)
                    {
                        slot.SetSlotIndex(i);
                        _slots.Add(slot);
                    }
                }
            }

            // Criar quick slots (geralmente são os mesmos slots, mas na barra de acesso rápido)
            if (_quickSlotsPanel != null && _slotPrefab != null)
            {
                Transform quickSlotsContainer = _quickSlotsPanel.transform;

                // Limpar quick slots existentes
                foreach (Transform child in quickSlotsContainer)
                {
                    Destroy(child.gameObject);
                }

                // Criar novos quick slots
                for (int i = 0; i < availableSlots; i++)
                {
                    GameObject slotObj = Instantiate(_slotPrefab, quickSlotsContainer);
                    InventorySlot slot = slotObj.GetComponent<InventorySlot>();

                    if (slot != null)
                    {
                        slot.SetSlotIndex(i);
                        _quickSlots.Add(slot);
                    }
                }

                // Ajustar layout se necessário
                AdjustQuickSlotsLayout(availableSlots);
            }
        }

        /// <summary>
        /// Limpa todos os slots existentes
        /// </summary>
        private void ClearSlots()
        {
            // Limpar slots principais
            if (_slotsContainer != null)
            {
                foreach (Transform child in _slotsContainer)
                {
                    Destroy(child.gameObject);
                }
            }

            _slots.Clear();
            _quickSlots.Clear();
        }

        /// <summary>
        /// Ajusta o layout dos quick slots baseado no número disponível
        /// </summary>
        private void AdjustQuickSlotsLayout(int availableSlots)
        {
            // Se tiver alguma lógica específica de layout para diferentes quantidades de slots
            // Este é o local para implementar

            // Exemplo: ajustar posicionamento para centralizar
            RectTransform panelRect = _quickSlotsPanel.GetComponent<RectTransform>();
            if (panelRect != null)
            {
                // Lógica de ajuste de tamanho ou posicionamento
            }
        }

        /// <summary>
        /// Atualiza visualmente todos os slots
        /// </summary>
        private void UpdateAllSlots()
        {
            foreach (InventorySlot slot in _slots)
            {
                slot.InitializeSlot();
            }

            foreach (InventorySlot slot in _quickSlots)
            {
                slot.InitializeSlot();
            }
        }

        #endregion

        #region Inventory Toggle

        /// <summary>
        /// Alterna a visibilidade do inventário
        /// </summary>
        public void ToggleInventory()
        {
            if (_isInventoryOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        /// <summary>
        /// Abre o inventário com animação
        /// </summary>
        public void OpenInventory()
        {
            if (_isInventoryOpen || _inventoryPanel == null)
                return;

            // Interromper animação em andamento
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            // Tocar som de abertura
            if (_openSound != null)
            {
                AudioSource.PlayClipAtPoint(_openSound, Camera.main.transform.position);
            }

            // Ativar painel e iniciar animação
            _inventoryPanel.SetActive(true);
            _animationCoroutine = StartCoroutine(AnimateInventory(true));

            _isInventoryOpen = true;

            // Notificar o jogo para pausar ou desabilitar certas ações se necessário
            // GameManager.Instance?.SetPlayerInputEnabled(false);
        }

        /// <summary>
        /// Fecha o inventário com animação
        /// </summary>
        public void CloseInventory()
        {
            if (!_isInventoryOpen || _inventoryPanel == null)
                return;

            // Interromper animação em andamento
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
            }

            // Tocar som de fechamento
            if (_closeSound != null)
            {
                AudioSource.PlayClipAtPoint(_closeSound, Camera.main.transform.position);
            }

            // Iniciar animação de fechamento
            _animationCoroutine = StartCoroutine(AnimateInventory(false));

            _isInventoryOpen = false;

            // Notificar o jogo para retomar ações normais
            // GameManager.Instance?.SetPlayerInputEnabled(true);
        }

        /// <summary>
        /// Anima a abertura/fechamento do inventário
        /// </summary>
        private IEnumerator AnimateInventory(bool opening)
        {
            RectTransform panelRect = _inventoryPanel.GetComponent<RectTransform>();
            float time = 0;

            // Valores de início e fim
            Vector2 startScale = opening ? Vector2.zero : Vector2.one;
            Vector2 endScale = opening ? Vector2.one : Vector2.zero;

            // Configurar valor inicial
            panelRect.localScale = startScale;

            // Animar ao longo do tempo
            while (time < _openDuration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / _openDuration);
                float curveValue = _openCurve.Evaluate(t);

                // Aplicar escala
                panelRect.localScale = Vector2.Lerp(startScale, endScale, curveValue);

                yield return null;
            }

            // Garantir valor final exato
            panelRect.localScale = endScale;

            // Se estiver fechando, desativar o objeto
            if (!opening)
            {
                _inventoryPanel.SetActive(false);
            }

            _animationCoroutine = null;
        }

        #endregion

        #region Event Handlers

        private void HandleInventoryChanged(List<InventoryItem> items)
        {
            UpdateAllSlots();
        }

        private void HandleSelectedSlotChanged(int selectedIndex)
        {
            // Tocar som de seleção
            if (_selectSound != null)
            {
                AudioSource.PlayClipAtPoint(_selectSound, Camera.main.transform.position, 0.5f);
            }
        }

        private void HandleAvailableSlotsChanged(int availableSlots)
        {
            // Reconstruir slots com a nova quantidade
            InitializeSlots(availableSlots);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Define visibilidade direta dos quick slots
        /// </summary>
        public void SetQuickSlotsVisible(bool visible)
        {
            if (_quickSlotsPanel != null)
            {
                _quickSlotsPanel.SetActive(visible);
            }
        }

        /// <summary>
        /// Seleciona um slot pelo índice via UI
        /// </summary>
        public void SelectSlotByIndex(int index)
        {
            if (InventoryManager.Instance != null)
            {
                if (index >= 0 && index < InventoryManager.Instance.GetAvailableSlots())
                {
                    InventoryManager.Instance.SelectSlot(index);
                }
            }
        }

        #endregion
    }
}
