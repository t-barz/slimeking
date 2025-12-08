using UnityEngine;
using System;

namespace TheSlimeKing.Inventory
{
    /// <summary>
    /// Gerenciador central do sistema de inventário.
    /// Controla 12 slots de inventário (não empilháveis), 3 slots de equipamento e 4 quick slots.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        #region Events
        public event Action OnInventoryChanged;
        public event Action OnInventoryFull;
        public event Action OnEquipmentChanged;
        public event Action OnQuickSlotsChanged;
        #endregion

        #region Private Fields
        private InventorySlot[] slots = new InventorySlot[12];
        private ItemData[] equipment = new ItemData[3]; // Amulet(0), Ring(1), Cape(2)
        private ItemData[] quickSlots = new ItemData[4]; // Up(0), Down(1), Left(2), Right(3)
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Implementar padrão Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializar arrays de slots
            InitializeSlots();
        }
        #endregion

        #region Initialization
        private void InitializeSlots()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = new InventorySlot();
            }
        }
        #endregion

        #region Public Methods - Item Management
        /// <summary>
        /// Adiciona um item ao inventário sem empilhamento.
        /// Cada item ocupa exatamente 1 slot, independentemente do tipo.
        /// </summary>
        /// <param name="item">Item a ser adicionado</param>
        /// <param name="quantity">Quantidade a adicionar (padrão: 1)</param>
        /// <returns>True se o item foi adicionado com sucesso, False se inventário está cheio</returns>
        public bool AddItem(ItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] Tentativa de adicionar item nulo ou quantidade inválida.");
                return false;
            }

            // Sistema não empilhável: cada item ocupa 1 slot
            for (int i = 0; i < quantity; i++)
            {
                int emptySlotIndex = FindEmptySlot();

                if (emptySlotIndex == -1)
                {
                    // Inventário cheio
                    OnInventoryFull?.Invoke();
                    UnityEngine.Debug.LogWarning($"[InventoryManager] Inventário cheio. Adicionados {i}/{quantity} itens.");
                    return i > 0; // Retorna true se pelo menos 1 item foi adicionado
                }

                // Adiciona ao slot vazio (sempre quantidade 1)
                slots[emptySlotIndex].item = item;
                slots[emptySlotIndex].quantity = 1;

                UnityEngine.Debug.Log($"[InventoryManager] ✅ Item '{item.itemName}' adicionado ao slot {emptySlotIndex}");

                OnInventoryChanged?.Invoke();
            }

            return true;
        }

        /// <summary>
        /// Encontra o primeiro slot vazio no inventário.
        /// </summary>
        /// <returns>Índice do slot vazio ou -1 se não houver</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Remove uma quantidade específica de um item do inventário.
        /// </summary>
        /// <param name="item">Item a ser removido</param>
        /// <param name="quantity">Quantidade a remover (padrão: 1)</param>
        /// <returns>True se removeu com sucesso</returns>
        public bool RemoveItem(ItemData item, int quantity = 1)
        {
            if (item == null || quantity <= 0)
            {
                return false;
            }

            int remainingToRemove = quantity;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item && remainingToRemove > 0)
                {
                    // Remove o item do slot (sempre limpa completamente)
                    slots[i].item = null;
                    slots[i].quantity = 0;
                    remainingToRemove--;

                    OnInventoryChanged?.Invoke();
                }
            }

            return remainingToRemove == 0;
        }

        /// <summary>
        /// Usa um item consumível do slot especificado.
        /// Aplica efeitos de cura e remove 1 unidade.
        /// </summary>
        /// <param name="slotIndex">Índice do slot</param>
        public void UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Índice de slot inválido: {slotIndex}");
                return;
            }

            InventorySlot slot = slots[slotIndex];

            if (slot.IsEmpty)
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] Tentativa de usar item de slot vazio.");
                return;
            }

            ItemData item = slot.item;

            // Verifica se é um item consumível
            if (item.type != ItemType.Consumable)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{item.itemName}' não é consumível.");
                return;
            }

            // Aplica efeitos do consumível
            ApplyConsumableEffects(item);

            // Remove o item do slot (não empilhável, sempre remove completamente)
            slot.item = null;
            slot.quantity = 0;

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Aplica os efeitos de um item consumível ao jogador.
        /// Integra com PlayerAttributesHandler para aplicar cura.
        /// </summary>
        /// <param name="item">Item consumível</param>
        private void ApplyConsumableEffects(ItemData item)
        {
            // Aplica cura usando PlayerAttributesHandler
            if (item.healAmount > 0)
            {
                // Busca PlayerAttributesHandler no PlayerController
                if (PlayerController.Instance != null)
                {
                    PlayerAttributesHandler attributesHandler = PlayerController.Instance.GetComponent<PlayerAttributesHandler>();

                    if (attributesHandler != null)
                    {
                        int actualHeal = attributesHandler.Heal(item.healAmount);
                        UnityEngine.Debug.Log($"[InventoryManager] Curado {actualHeal} pontos de vida usando '{item.itemName}'.");
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("[InventoryManager] PlayerAttributesHandler não encontrado no PlayerController. Cura não aplicada.");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("[InventoryManager] PlayerController.Instance não encontrado. Cura não aplicada.");
                }
            }
        }

        /// <summary>
        /// Descarta um item do inventário.
        /// Verifica se é quest item (bloqueia) e limpa o slot.
        /// </summary>
        /// <param name="slotIndex">Índice do slot</param>
        /// <param name="onConfirm">Callback de confirmação (para UI)</param>
        public void DiscardItem(int slotIndex, Action onConfirm = null)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Índice de slot inválido: {slotIndex}");
                return;
            }

            InventorySlot slot = slots[slotIndex];

            if (slot.IsEmpty)
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] Tentativa de descartar item de slot vazio.");
                return;
            }

            // Verifica se é quest item (bloqueia descarte)
            if (slot.item.isQuestItem)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Itens de quest não podem ser descartados: {slot.item.itemName}");
                return;
            }

            // Executa callback de confirmação se fornecido
            onConfirm?.Invoke();

            // Remove item do quick slot se estiver atribuído
            RemoveFromQuickSlots(slot.item);

            // Limpa o slot
            slot.item = null;
            slot.quantity = 0;

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Remove um item de todos os quick slots onde estiver atribuído.
        /// </summary>
        /// <param name="item">Item a ser removido</param>
        private void RemoveFromQuickSlots(ItemData item)
        {
            for (int i = 0; i < quickSlots.Length; i++)
            {
                if (quickSlots[i] == item)
                {
                    quickSlots[i] = null;
                }
            }
        }

        /// <summary>
        /// Equipa um item de equipamento no slot apropriado.
        /// Desequipa item anterior se existir e retorna ao inventário.
        /// </summary>
        /// <param name="item">Item de equipamento a equipar</param>
        /// <returns>True se equipou com sucesso</returns>
        public bool EquipItem(ItemData item)
        {
            if (item == null)
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] Tentativa de equipar item nulo.");
                return false;
            }

            // Verifica se é um item de equipamento
            if (item.type != ItemType.Equipment)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{item.itemName}' não é equipamento.");
                return false;
            }

            // Determina o índice do slot de equipamento baseado no tipo
            int equipmentSlotIndex = (int)item.equipmentType;

            // Desequipa item anterior se existir
            if (equipment[equipmentSlotIndex] != null)
            {
                ItemData previousItem = equipment[equipmentSlotIndex];

                // Tenta retornar ao inventário
                if (!AddItem(previousItem, 1))
                {
                    UnityEngine.Debug.LogWarning($"[InventoryManager] Inventário cheio. Não foi possível desequipar '{previousItem.itemName}'.");
                    return false;
                }
            }

            // Equipa o novo item
            equipment[equipmentSlotIndex] = item;

            // Remove do inventário
            RemoveItem(item, 1);

            // Aplica buffs de equipamento
            ApplyEquipmentStats();

            OnEquipmentChanged?.Invoke();
            OnInventoryChanged?.Invoke();

            UnityEngine.Debug.Log($"[InventoryManager] Equipado '{item.itemName}' no slot {item.equipmentType}.");
            return true;
        }

        /// <summary>
        /// Desequipa um item de equipamento e retorna ao inventário.
        /// </summary>
        /// <param name="equipmentType">Tipo de equipamento a desequipar</param>
        /// <returns>True se desequipou com sucesso</returns>
        public bool UnequipItem(EquipmentType equipmentType)
        {
            int equipmentSlotIndex = (int)equipmentType;

            if (equipment[equipmentSlotIndex] == null)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Nenhum item equipado no slot {equipmentType}.");
                return false;
            }

            ItemData itemToUnequip = equipment[equipmentSlotIndex];

            // Tenta adicionar ao inventário
            if (!AddItem(itemToUnequip, 1))
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Inventário cheio. Não foi possível desequipar '{itemToUnequip.itemName}'.");
                return false;
            }

            // Remove do slot de equipamento
            equipment[equipmentSlotIndex] = null;

            // Recalcula buffs de equipamento
            ApplyEquipmentStats();

            OnEquipmentChanged?.Invoke();
            OnInventoryChanged?.Invoke();

            UnityEngine.Debug.Log($"[InventoryManager] Desequipado '{itemToUnequip.itemName}' do slot {equipmentType}.");
            return true;
        }

        /// <summary>
        /// Aplica os buffs de todos os equipamentos equipados ao PlayerController.
        /// </summary>
        private void ApplyEquipmentStats()
        {
            int totalDefense = 0;
            int totalSpeed = 0;

            // Soma os buffs de todos os equipamentos
            foreach (ItemData item in equipment)
            {
                if (item != null)
                {
                    totalDefense += item.defenseBonus;
                    totalSpeed += item.speedBonus;
                }
            }

            // Aplica ao PlayerController se disponível
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.SetDefenseBonus(totalDefense);
                PlayerController.Instance.SetSpeedBonus(totalSpeed);
                UnityEngine.Debug.Log($"[InventoryManager] Buffs aplicados - Defesa: {totalDefense}, Velocidade: {totalSpeed}");
            }
            else
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] PlayerController.Instance não encontrado. Buffs não aplicados.");
            }
        }

        /// <summary>
        /// Obtém o item equipado em um slot específico.
        /// </summary>
        /// <param name="equipmentType">Tipo de equipamento</param>
        /// <returns>Item equipado ou null</returns>
        public ItemData GetEquippedItem(EquipmentType equipmentType)
        {
            int equipmentSlotIndex = (int)equipmentType;
            return equipment[equipmentSlotIndex];
        }
        #endregion

        #region Public Methods - Quick Slots
        /// <summary>
        /// Atribui um item a um dos 4 direcionais (quick slots).
        /// </summary>
        /// <param name="item">Item a ser atribuído</param>
        /// <param name="direction">Direção (0=Up, 1=Down, 2=Left, 3=Right)</param>
        public void AssignQuickSlot(ItemData item, int direction)
        {
            if (direction < 0 || direction >= quickSlots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Direção de quick slot inválida: {direction}");
                return;
            }

            if (item == null)
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] Tentativa de atribuir item nulo ao quick slot.");
                return;
            }

            // Verifica se o item existe no inventário
            bool itemExists = false;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item)
                {
                    itemExists = true;
                    break;
                }
            }

            if (!itemExists)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{item.itemName}' não está no inventário.");
                return;
            }

            // Atribui o item ao quick slot
            quickSlots[direction] = item;
            OnQuickSlotsChanged?.Invoke();

            UnityEngine.Debug.Log($"[InventoryManager] Item '{item.itemName}' atribuído ao quick slot {direction}.");
        }

        /// <summary>
        /// Usa o item atribuído a um direcional específico.
        /// Reduz quantidade e remove do quick slot se chegar a zero.
        /// </summary>
        /// <param name="direction">Direção (0=Up, 1=Down, 2=Left, 3=Right)</param>
        public void UseQuickSlot(int direction)
        {
            if (direction < 0 || direction >= quickSlots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Direção de quick slot inválida: {direction}");
                return;
            }

            ItemData item = quickSlots[direction];

            if (item == null)
            {
                UnityEngine.Debug.Log($"[InventoryManager] Nenhum item atribuído ao quick slot {direction}.");
                return;
            }

            // Encontra o slot do item no inventário
            int slotIndex = -1;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item)
                {
                    slotIndex = i;
                    break;
                }
            }

            if (slotIndex == -1)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{item.itemName}' não encontrado no inventário.");
                // Remove do quick slot pois não existe mais no inventário
                quickSlots[direction] = null;
                OnQuickSlotsChanged?.Invoke();
                return;
            }

            // Verifica se é um item consumível
            if (item.type != ItemType.Consumable)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{item.itemName}' não é consumível.");
                return;
            }

            // Aplica efeitos do consumível
            ApplyConsumableEffects(item);

            // Remove o item do slot (não empilhável)
            slots[slotIndex].item = null;
            slots[slotIndex].quantity = 0;
            quickSlots[direction] = null;
            OnQuickSlotsChanged?.Invoke();

            OnInventoryChanged?.Invoke();
            UnityEngine.Debug.Log($"[InventoryManager] Usado item '{item.itemName}' do quick slot {direction}.");
        }

        /// <summary>
        /// Obtém o item atribuído a um quick slot específico.
        /// </summary>
        /// <param name="direction">Direção (0=Up, 1=Down, 2=Left, 3=Right)</param>
        /// <returns>Item atribuído ou null</returns>
        public ItemData GetQuickSlotItem(int direction)
        {
            if (direction < 0 || direction >= quickSlots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Direção de quick slot inválida: {direction}");
                return null;
            }

            return quickSlots[direction];
        }
        #endregion

        #region Public Methods - Slot Access
        /// <summary>
        /// Obtém um slot do inventário pelo índice.
        /// </summary>
        /// <param name="index">Índice do slot (0-11)</param>
        /// <returns>Slot do inventário ou null se índice inválido</returns>
        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= 12)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Índice de slot inválido: {index}");
                return null;
            }

            return slots[index];
        }

        /// <summary>
        /// Obtém todos os slots do inventário.
        /// </summary>
        /// <returns>Array de slots</returns>
        public InventorySlot[] GetAllSlots()
        {
            return slots;
        }

        /// <summary>
        /// Troca o conteúdo de dois slots do inventário.
        /// Funciona com slots vazios também.
        /// </summary>
        /// <param name="slotIndex1">Índice do primeiro slot</param>
        /// <param name="slotIndex2">Índice do segundo slot</param>
        /// <returns>True se o swap foi bem-sucedido</returns>
        public bool SwapSlots(int slotIndex1, int slotIndex2)
        {
            // Validar índices
            if (slotIndex1 < 0 || slotIndex1 >= slots.Length ||
                slotIndex2 < 0 || slotIndex2 >= slots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Índices de slot inválidos para swap: {slotIndex1}, {slotIndex2}");
                return false;
            }

            // Não faz nada se os índices são iguais
            if (slotIndex1 == slotIndex2)
            {
                return false;
            }

            // Troca os conteúdos dos slots
            InventorySlot temp = new InventorySlot
            {
                item = slots[slotIndex1].item,
                quantity = slots[slotIndex1].quantity
            };

            slots[slotIndex1].item = slots[slotIndex2].item;
            slots[slotIndex1].quantity = slots[slotIndex2].quantity;

            slots[slotIndex2].item = temp.item;
            slots[slotIndex2].quantity = temp.quantity;

            UnityEngine.Debug.Log($"[InventoryManager] ✓ Swap entre slots {slotIndex1} e {slotIndex2} concluído.");
            OnInventoryChanged?.Invoke();

            return true;
        }
        #endregion

        #region Public Methods - Save/Load
        /// <summary>
        /// Salva o estado atual do inventário.
        /// Serializa todos os slots não-vazios, equipamentos e quick slots.
        /// </summary>
        /// <returns>Dados serializados do inventário</returns>
        public InventorySaveData SaveInventory()
        {
            InventorySaveData saveData = new InventorySaveData();

            // Serializar todos os slots não-vazios
            System.Collections.Generic.List<ItemSaveData> itemList = new System.Collections.Generic.List<ItemSaveData>();

            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsEmpty)
                {
                    ItemSaveData itemData = new ItemSaveData
                    {
                        itemID = slots[i].item.name, // Nome do ScriptableObject
                        quantity = slots[i].quantity,
                        slotIndex = i
                    };
                    itemList.Add(itemData);
                }
            }

            saveData.items = itemList.ToArray();

            // Salvar IDs dos equipamentos
            for (int i = 0; i < equipment.Length; i++)
            {
                saveData.equipmentIDs[i] = equipment[i] != null ? equipment[i].name : string.Empty;
            }

            // Salvar IDs dos quick slots
            for (int i = 0; i < quickSlots.Length; i++)
            {
                saveData.quickSlotIDs[i] = quickSlots[i] != null ? quickSlots[i].name : string.Empty;
            }

            UnityEngine.Debug.Log($"[InventoryManager] Inventário salvo com {saveData.items.Length} itens.");
            return saveData;
        }

        /// <summary>
        /// Carrega o estado do inventário a partir de dados salvos.
        /// Recria todos os slots, equipamentos e quick slots.
        /// </summary>
        /// <param name="saveData">Dados salvos do inventário</param>
        public void LoadInventory(InventorySaveData saveData)
        {
            if (saveData == null)
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] Tentativa de carregar dados nulos.");
                return;
            }

            // Limpar inventário atual
            InitializeSlots();
            equipment = new ItemData[3];
            quickSlots = new ItemData[4];

            // Carregar itens
            if (saveData.items != null)
            {
                foreach (ItemSaveData itemData in saveData.items)
                {
                    // Carregar o ItemData do Resources
                    ItemData item = Resources.Load<ItemData>($"Items/{itemData.itemID}");

                    if (item == null)
                    {
                        UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{itemData.itemID}' não encontrado em Resources/Items/");
                        continue;
                    }

                    // Validar índice do slot (0-11)
                    if (itemData.slotIndex < 0 || itemData.slotIndex >= 12)
                    {
                        UnityEngine.Debug.LogWarning($"[InventoryManager] Índice de slot inválido: {itemData.slotIndex}");
                        continue;
                    }

                    // Recriar slot com item correto (sempre quantidade 1)
                    slots[itemData.slotIndex].item = item;
                    slots[itemData.slotIndex].quantity = 1;
                }
            }

            // Restaurar equipamentos
            if (saveData.equipmentIDs != null)
            {
                for (int i = 0; i < saveData.equipmentIDs.Length && i < equipment.Length; i++)
                {
                    if (!string.IsNullOrEmpty(saveData.equipmentIDs[i]))
                    {
                        ItemData item = Resources.Load<ItemData>($"Items/{saveData.equipmentIDs[i]}");

                        if (item != null)
                        {
                            equipment[i] = item;
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning($"[InventoryManager] Equipamento '{saveData.equipmentIDs[i]}' não encontrado em Resources/Items/");
                        }
                    }
                }
            }

            // Restaurar quick slots
            if (saveData.quickSlotIDs != null)
            {
                for (int i = 0; i < saveData.quickSlotIDs.Length && i < quickSlots.Length; i++)
                {
                    if (!string.IsNullOrEmpty(saveData.quickSlotIDs[i]))
                    {
                        ItemData item = Resources.Load<ItemData>($"Items/{saveData.quickSlotIDs[i]}");

                        if (item != null)
                        {
                            quickSlots[i] = item;
                        }
                        else
                        {
                            UnityEngine.Debug.LogWarning($"[InventoryManager] Quick slot item '{saveData.quickSlotIDs[i]}' não encontrado em Resources/Items/");
                        }
                    }
                }
            }

            // Aplicar buffs de equipamentos
            ApplyEquipmentStats();

            // Notificar mudanças
            OnInventoryChanged?.Invoke();
            OnEquipmentChanged?.Invoke();
            OnQuickSlotsChanged?.Invoke();

            UnityEngine.Debug.Log($"[InventoryManager] Inventário carregado com {saveData.items?.Length ?? 0} itens.");
        }
        #endregion
    }
}
