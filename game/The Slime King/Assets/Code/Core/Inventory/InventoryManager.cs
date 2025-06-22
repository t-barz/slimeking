using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using TheSlimeKing.Core.Elemental;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Gerencia o sistema de inventário do jogador, incluindo slots, uso de itens e navegação
    /// </summary>
    public partial class InventoryManager : MonoBehaviour
    {
        #region Singleton
        private static InventoryManager _instance;
        public static InventoryManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<InventoryManager>();
                    if (_instance == null)
                    {
                        Debug.LogError("Nenhum InventoryManager encontrado na cena!");
                    }
                }
                return _instance;
            }
        }
        #endregion

        [Header("Configurações")]
        [SerializeField] private int _maxSlots = 4;
        [SerializeField] private AudioClip _itemPickupSound;
        [SerializeField] private AudioClip _itemUseSound;
        [SerializeField] private AudioClip _inventoryFullSound;

        [Header("Dependências")]
        [SerializeField] private Transform _slimeTransform;
        [SerializeField] private PlayerInput _playerInput;

        // Lista de itens no inventário
        private List<InventoryItem> _items = new List<InventoryItem>();
        // Índice do item selecionado atualmente
        private int _selectedSlotIndex = 0;
        // Número de slots disponíveis atualmente (baseado no estágio de crescimento)
        private int _availableSlots = 1;

        // Eventos
        public event Action<List<InventoryItem>> OnInventoryChanged;
        public event Action<int> OnSelectedSlotChanged;
        public event Action<ItemData, Vector3> OnItemUsed;
        public event Action<int> OnAvailableSlotsChanged;
        public event Action OnInventoryFull;

        private void Awake()
        {
            // Garantir que há apenas uma instância
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializar lista com slots vazios
            for (int i = 0; i < _maxSlots; i++)
            {
                _items.Add(null);
            }
        }
        private void Start()
        {
            // Registrar nos eventos de input
            RegisterInput();

            // Se estiver integrado com o sistema de crescimento
            ElementalEvents.OnElementalThresholdReached += CheckGrowthSlots;
        }

        private void OnDestroy()
        {
            // Cancelar registro nos eventos
            if (_playerInput != null)
            {
                UnregisterInput();
            }

            // Se estiver integrado com o sistema de crescimento
            ElementalEvents.OnElementalThresholdReached -= CheckGrowthSlots;
        }

        #region Input Handling

        private void RegisterInput()
        {
            if (_playerInput == null)
            {
                Debug.LogError("PlayerInput não atribuído no InventoryManager!");
                return;
            }

            // Associar ações de inventário
            try
            {
                _playerInput.actions["UseItem"].performed += OnUseItem;
                _playerInput.actions["ChangeItem"].performed += OnChangeItem;
                _playerInput.actions["Inventory"].performed += OnToggleInventory;
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao registrar inputs do inventário: {e.Message}");
            }
        }

        private void UnregisterInput()
        {
            if (_playerInput == null) return;

            try
            {
                _playerInput.actions["UseItem"].performed -= OnUseItem;
                _playerInput.actions["ChangeItem"].performed -= OnChangeItem;
                _playerInput.actions["Inventory"].performed -= OnToggleInventory;
            }
            catch (Exception) { }
        }

        private void OnUseItem(InputAction.CallbackContext context)
        {
            UseSelectedItem();
        }

        private void OnChangeItem(InputAction.CallbackContext context)
        {
            float scrollValue = context.ReadValue<float>();

            // D-Pad vertical ou mouse scroll
            if (Mathf.Abs(scrollValue) > 0.1f)
            {
                if (scrollValue > 0)
                    NextSlot();
                else
                    PreviousSlot();
            }
            // Tab
            else
            {
                NextSlot();
            }
        }

        private void OnToggleInventory(InputAction.CallbackContext context)
        {
            // Será implementado quando tivermos a UI completa do inventário
            // Por enquanto apenas notifica para debug
            Debug.Log("Alternando visualização do inventário");
        }

        #endregion

        #region Inventory Operations

        /// <summary>
        /// Adiciona um item ao inventário
        /// </summary>
        /// <returns>True se o item foi adicionado com sucesso</returns>
        public bool AddItem(ItemData itemData, int quantity = 1)
        {
            if (itemData == null || quantity <= 0)
                return false;

            // 1. Tenta empilhar com itens existentes do mesmo tipo
            for (int i = 0; i < _availableSlots; i++)
            {
                if (_items[i] != null && _items[i].ItemData == itemData && !_items[i].IsFull)
                {
                    int remainder = _items[i].AddQuantity(quantity);

                    // Se adicionou todos
                    if (remainder <= 0)
                    {
                        if (_itemPickupSound != null)
                            AudioSource.PlayClipAtPoint(_itemPickupSound, _slimeTransform.position);

                        OnInventoryChanged?.Invoke(_items);
                        return true;
                    }
                    // Se adicionou parcialmente
                    else
                    {
                        quantity = remainder; // Continua com o restante
                    }
                }
            }

            // 2. Tenta encontrar um slot vazio
            for (int i = 0; i < _availableSlots; i++)
            {
                if (_items[i] == null || _items[i].IsEmpty)
                {
                    _items[i] = new InventoryItem(itemData, quantity);

                    if (_itemPickupSound != null)
                        AudioSource.PlayClipAtPoint(_itemPickupSound, _slimeTransform.position);

                    OnInventoryChanged?.Invoke(_items);
                    return true;
                }
            }

            // 3. Sem espaço disponível
            if (_inventoryFullSound != null)
                AudioSource.PlayClipAtPoint(_inventoryFullSound, _slimeTransform.position);

            OnInventoryFull?.Invoke();
            return false;
        }

        /// <summary>
        /// Remove um item do inventário
        /// </summary>
        /// <returns>True se o item foi removido com sucesso</returns>
        public bool RemoveItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId) || quantity <= 0)
                return false;

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null && !_items[i].IsEmpty && _items[i].ItemData.ItemId == itemId)
                {
                    int removed = _items[i].RemoveQuantity(quantity);

                    // Se o item ficou vazio, redefina para null para manter consistência
                    if (_items[i].IsEmpty)
                    {
                        _items[i] = null;
                    }

                    // Reorganizar automaticamente se necessário
                    if (_items[_selectedSlotIndex] == null)
                    {
                        ReorganizeInventory();
                    }

                    OnInventoryChanged?.Invoke(_items);
                    return removed > 0;
                }
            }

            return false;
        }

        /// <summary>
        /// Usa o item atualmente selecionado
        /// </summary>
        public void UseSelectedItem()
        {
            // Verificar se o slot selecionado tem um item
            if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _availableSlots &&
                _items[_selectedSlotIndex] != null && !_items[_selectedSlotIndex].IsEmpty)
            {
                ItemData itemToUse = _items[_selectedSlotIndex].ItemData;

                // Verificar se é consumível
                if (itemToUse.IsConsumable)
                {
                    // Remover uma unidade
                    _items[_selectedSlotIndex].RemoveQuantity(1);

                    // Se o item acabou, limpar o slot
                    if (_items[_selectedSlotIndex].IsEmpty)
                    {
                        _items[_selectedSlotIndex] = null;
                        // Reorganizar automaticamente
                        ReorganizeInventory();
                    }
                }

                // Reproduzir som de uso
                if (_itemUseSound != null || itemToUse.UseSound != null)
                {
                    AudioClip soundToPlay = itemToUse.UseSound != null ? itemToUse.UseSound : _itemUseSound;
                    AudioSource.PlayClipAtPoint(soundToPlay, _slimeTransform.position);
                }

                // Instanciar efeito de uso, se houver
                if (itemToUse.UseEffectPrefab != null)
                {
                    Instantiate(itemToUse.UseEffectPrefab, _slimeTransform.position, Quaternion.identity);
                }

                // Notificar uso
                OnItemUsed?.Invoke(itemToUse, _slimeTransform.position);
                OnInventoryChanged?.Invoke(_items);
            }
            else
            {
                Debug.Log("Nenhum item para usar no slot selecionado");
            }
        }

        /// <summary>
        /// Seleciona o próximo slot disponível
        /// </summary>
        public void NextSlot()
        {
            int nextIndex = _selectedSlotIndex;

            // Encontrar próximo slot válido (com loop)
            do
            {
                nextIndex = (nextIndex + 1) % _availableSlots;
            }
            while (nextIndex != _selectedSlotIndex && (_items[nextIndex] == null || _items[nextIndex].IsEmpty));

            // Se encontrou, ou se voltou ao mesmo slot
            _selectedSlotIndex = nextIndex;
            OnSelectedSlotChanged?.Invoke(_selectedSlotIndex);
        }

        /// <summary>
        /// Seleciona o slot anterior disponível
        /// </summary>
        public void PreviousSlot()
        {
            int prevIndex = _selectedSlotIndex;

            // Encontrar slot anterior válido (com loop)
            do
            {
                prevIndex = (prevIndex - 1 + _availableSlots) % _availableSlots;
            }
            while (prevIndex != _selectedSlotIndex && (_items[prevIndex] == null || _items[prevIndex].IsEmpty));

            // Se encontrou, ou se voltou ao mesmo slot
            _selectedSlotIndex = prevIndex;
            OnSelectedSlotChanged?.Invoke(_selectedSlotIndex);
        }

        /// <summary>
        /// Reorganiza o inventário, removendo espaços vazios entre itens
        /// </summary>
        public void ReorganizeInventory()
        {
            // Lista temporária sem itens nulos
            List<InventoryItem> validItems = new List<InventoryItem>();

            // Coletar todos os itens válidos
            foreach (InventoryItem item in _items)
            {
                if (item != null && !item.IsEmpty)
                {
                    validItems.Add(item);
                }
            }

            // Limpar a lista atual
            for (int i = 0; i < _items.Count; i++)
            {
                _items[i] = null;
            }

            // Preencher com os itens válidos
            for (int i = 0; i < validItems.Count && i < _availableSlots; i++)
            {
                _items[i] = validItems[i];
            }

            // Ajustar o índice selecionado se necessário
            if (_selectedSlotIndex >= validItems.Count || validItems.Count == 0)
            {
                _selectedSlotIndex = validItems.Count > 0 ? 0 : _selectedSlotIndex;
            }

            OnInventoryChanged?.Invoke(_items);
            OnSelectedSlotChanged?.Invoke(_selectedSlotIndex);
        }        /// <summary>
                 /// Atualiza o número de slots disponíveis com base no estágio de crescimento
                 /// </summary>
                 /// <param name="stage">O estágio atual de crescimento do jogador</param>
        public void CheckGrowthSlots(int stage)
        {
            // Usando o estágio de crescimento fornecido pelo sistema elemental
            // para determinar quantos slots devem estar disponíveis

            // Calcular slots baseado no estágio (0-3)
            int newSlots = Mathf.Clamp(stage + 1, 1, _maxSlots);

            if (newSlots != _availableSlots)
            {
                _availableSlots = newSlots;
                OnAvailableSlotsChanged?.Invoke(_availableSlots);
                ReorganizeInventory();

                Debug.Log($"Inventário expandido para {_availableSlots} slots no estágio {stage}!");
            }
        }

        /// <summary>
        /// Atualiza o número de slots disponíveis com base na energia elemental total (método legado)
        /// </summary>
        private void UpdateSlotsBasedOnEnergyLevel()
        {
            // Método mantido para compatibilidade ou testes
            int totalEnergyLevel = 0;

            try
            {
                totalEnergyLevel = Mathf.FloorToInt(ElementalEnergyManager.Instance.GetTotalAbsorbedEnergy() / 200);
            }
            catch (Exception)
            {
                totalEnergyLevel = 0; // Se não houver sistema elemental
            }

            // Calcular slots baseado no nível (0-3)
            int newSlots = Mathf.Clamp(totalEnergyLevel + 1, 1, _maxSlots);

            if (newSlots != _availableSlots)
            {
                _availableSlots = newSlots;
                OnAvailableSlotsChanged?.Invoke(_availableSlots);
                ReorganizeInventory();

                Debug.Log($"Inventário expandido para {_availableSlots} slots!");
            }
        }

        /// <summary>
        /// Define manualmente o número de slots disponíveis (para testes)
        /// </summary>
        public void SetAvailableSlots(int slots)
        {
            _availableSlots = Mathf.Clamp(slots, 1, _maxSlots);
            OnAvailableSlotsChanged?.Invoke(_availableSlots);
            ReorganizeInventory();
        }

        #endregion

        #region Getters

        public InventoryItem GetItemAt(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                return _items[index];
            }
            return null;
        }

        public InventoryItem GetSelectedItem()
        {
            if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _items.Count)
            {
                return _items[_selectedSlotIndex];
            }
            return null;
        }

        public int GetSelectedSlotIndex()
        {
            return _selectedSlotIndex;
        }

        public int GetAvailableSlots()
        {
            return _availableSlots;
        }

        public List<InventoryItem> GetAllItems()
        {
            return new List<InventoryItem>(_items);
        }

        public bool HasItem(string itemId, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemId))
                return false;

            int totalCount = 0;

            foreach (InventoryItem item in _items)
            {
                if (item != null && !item.IsEmpty && item.ItemData.ItemId == itemId)
                {
                    totalCount += item.Quantity;
                    if (totalCount >= quantity)
                        return true;
                }
            }

            return false;
        }

        #endregion
    }
}
