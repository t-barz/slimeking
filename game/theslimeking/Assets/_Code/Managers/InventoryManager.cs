using UnityEngine;
using SlimeKing.Gameplay;
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

        // Quick slots armazenam o ID único da instância do item
        // Isso permite múltiplos itens do mesmo tipo em quick slots diferentes
        // 0 = slot vazio
        private int[] quickSlots = new int[4] { 0, 0, 0, 0 }; // Up(0), Down(1), Left(2), Right(3)

        // HashSet para garantir IDs únicos
        private System.Collections.Generic.HashSet<int> usedInstanceIDs = new System.Collections.Generic.HashSet<int>();
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

        /// <summary>
        /// Gera um ID único de 6 dígitos (100000-999999) para uma instância de item.
        /// Garante que o ID gerado não está em uso.
        /// </summary>
        /// <returns>ID único de 6 dígitos</returns>
        private int GenerateUniqueInstanceID()
        {
            int newID;
            int attempts = 0;
            const int maxAttempts = 1000;

            do
            {
                // Gera número aleatório entre 100000 e 999999 (6 dígitos)
                newID = UnityEngine.Random.Range(100000, 1000000);
                attempts++;

                if (attempts >= maxAttempts)
                {
                    UnityEngine.Debug.LogError("[InventoryManager] Não foi possível gerar ID único após 1000 tentativas!");
                    return 0;
                }
            }
            while (usedInstanceIDs.Contains(newID));

            usedInstanceIDs.Add(newID);
            return newID;
        }

        /// <summary>
        /// Remove um ID da lista de IDs em uso (quando item é removido do inventário).
        /// </summary>
        /// <param name="instanceID">ID da instância a ser liberado</param>
        private void ReleaseInstanceID(int instanceID)
        {
            if (instanceID > 0)
            {
                usedInstanceIDs.Remove(instanceID);
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

                // Gera ID único para esta instância do item
                int uniqueID = GenerateUniqueInstanceID();

                // Adiciona ao slot vazio (sempre quantidade 1)
                slots[emptySlotIndex].item = item;
                slots[emptySlotIndex].quantity = 1;
                slots[emptySlotIndex].instanceID = uniqueID;

                UnityEngine.Debug.Log($"[InventoryManager] ✅ Item '{item.itemName}' adicionado ao slot {emptySlotIndex} com ID {uniqueID}");

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
                    // Libera o ID único desta instância
                    ReleaseInstanceID(slots[i].instanceID);

                    // Remove o item do slot (sempre limpa completamente)
                    slots[i].item = null;
                    slots[i].quantity = 0;
                    slots[i].instanceID = 0;
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

            // Remove item do quick slot se estiver atribuído (usando instanceID)
            RemoveFromQuickSlots(slot.instanceID);

            // Libera o ID único
            ReleaseInstanceID(slot.instanceID);

            // Limpa o slot
            slot.item = null;
            slot.quantity = 0;
            slot.instanceID = 0;

            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Remove referências de quick slots para um instanceID específico.
        /// Deve ser chamado quando um item é removido do inventário ou quando um slot é esvaziado.
        /// </summary>
        /// <param name="instanceID">ID da instância do item que foi removido</param>
        private void RemoveFromQuickSlots(int instanceID)
        {
            if (instanceID == 0) return;

            // Percorre todos os quick slots e remove referências ao instanceID
            for (int i = 0; i < quickSlots.Length; i++)
            {
                if (quickSlots[i] == instanceID)
                {
                    quickSlots[i] = 0;
                }
            }
        }

        /// <summary>
        /// Remove referências de quick slots baseado no ItemData (busca todos os instanceIDs deste item).
        /// Sobrecarga para manter compatibilidade com código existente.
        /// </summary>
        /// <param name="item">Item que foi removido</param>
        private void RemoveFromQuickSlots(ItemData item)
        {
            if (item == null) return;

            // Percorre todos os quick slots e remove referências a instanceIDs deste item
            for (int i = 0; i < quickSlots.Length; i++)
            {
                int instanceID = quickSlots[i];

                if (instanceID > 0)
                {
                    // Busca o slot que contém este instanceID
                    for (int s = 0; s < slots.Length; s++)
                    {
                        if (slots[s].instanceID == instanceID && slots[s].item == item)
                        {
                            quickSlots[i] = 0;
                            break;
                        }
                    }
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
        /// Atribui um item específico (identificado por instanceID único) a um dos 4 direcionais (quick slots).
        /// Sistema não empilhável: cada instanceID só pode estar em UM quick slot por vez.
        /// Se o mesmo instanceID já estiver em outro quick slot, ele será movido (slot anterior fica vazio).
        /// Esta é a sobrecarga que recebe instanceID diretamente (recomendada).
        /// </summary>
        /// <param name="instanceID">ID único da instância do item</param>
        /// <param name="direction">Direção (0=Up, 1=Down, 2=Left, 3=Right)</param>
        public void AssignQuickSlot(int instanceID, int direction)
        {
            if (direction < 0 || direction >= quickSlots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Direção de quick slot inválida: {direction}");
                return;
            }

            if (instanceID <= 0)
            {
                UnityEngine.Debug.LogWarning("[InventoryManager] Tentativa de atribuir instanceID inválido ao quick slot.");
                return;
            }

            // Busca o slot que contém este instanceID
            ItemData item = null;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].instanceID == instanceID)
                {
                    item = slots[i].item;
                    break;
                }
            }

            if (item == null)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Nenhum item com instanceID {instanceID} encontrado no inventário.");
                return;
            }

            // Verifica se este instanceID já está em outro quick slot
            // Se sim, remove do slot antigo (move para o novo)
            for (int q = 0; q < quickSlots.Length; q++)
            {
                if (quickSlots[q] == instanceID && q != direction)
                {
                    UnityEngine.Debug.Log($"[InventoryManager] Item '{item.itemName}' (ID {instanceID}) movido do quick slot {q} para {direction}.");
                    quickSlots[q] = 0;
                    break;
                }
            }

            // Atribui o instanceID ao quick slot de destino
            quickSlots[direction] = instanceID;
            OnQuickSlotsChanged?.Invoke();

            UnityEngine.Debug.Log($"[InventoryManager] Item '{item.itemName}' (ID {instanceID}) atribuído ao quick slot {direction}.");
        }

        /// <summary>
        /// Atribui um item por ItemData - encontra o PRIMEIRO instanceID disponível (não atribuído).
        /// Sistema não empilhável: cada instanceID só pode estar em UM quick slot por vez.
        /// NOTA: Use a sobrecarga AssignQuickSlot(int instanceID, int direction) se souber o instanceID específico.
        /// Esta versão é útil apenas para atribuir qualquer item disponível do tipo.
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

            // Encontra o PRIMEIRO slot do inventário que contém este item e NÃO está atribuído a nenhum quick slot
            int targetInstanceID = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == item && slots[i].instanceID > 0)
                {
                    // Verifica se este instanceID já está atribuído a algum quick slot
                    bool isAlreadyAssigned = false;
                    for (int q = 0; q < quickSlots.Length; q++)
                    {
                        if (quickSlots[q] == slots[i].instanceID)
                        {
                            isAlreadyAssigned = true;
                            break;
                        }
                    }

                    if (!isAlreadyAssigned)
                    {
                        targetInstanceID = slots[i].instanceID;
                        break;
                    }
                }
            }

            if (targetInstanceID == 0)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{item.itemName}' não encontrado no inventário ou todos já estão atribuídos a quick slots.");
                return;
            }

            // Delega para a sobrecarga de instanceID
            AssignQuickSlot(targetInstanceID, direction);
        }

        /// <summary>
        /// Usa o item atribuído a um direcional específico.
        /// Reduz quantidade e remove do quick slot se o item for consumido.
        /// </summary>
        /// <param name="direction">Direção (0=Up, 1=Down, 2=Left, 3=Right)</param>
        public void UseQuickSlot(int direction)
        {
            if (direction < 0 || direction >= quickSlots.Length)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Direção de quick slot inválida: {direction}");
                return;
            }

            int instanceID = quickSlots[direction];

            if (instanceID == 0)
            {
                UnityEngine.Debug.Log($"[InventoryManager] Nenhum item atribuído ao quick slot {direction}.");
                return;
            }

            // Busca o slot que contém o item com este instanceID
            int slotIndex = -1;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].instanceID == instanceID)
                {
                    slotIndex = i;
                    break;
                }
            }

            if (slotIndex == -1 || slots[slotIndex].item == null)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item com ID {instanceID} não encontrado, mas quick slot {direction} ainda referencia ele.");
                quickSlots[direction] = 0;
                OnQuickSlotsChanged?.Invoke();
                return;
            }

            ItemData item = slots[slotIndex].item;

            // Verifica se é um item consumível
            if (item.type != ItemType.Consumable)
            {
                UnityEngine.Debug.LogWarning($"[InventoryManager] Item '{item.itemName}' não é consumível.");
                return;
            }

            // Aplica efeitos do consumível
            ApplyConsumableEffects(item);

            // Libera o ID único
            ReleaseInstanceID(slots[slotIndex].instanceID);

            // Remove o item do slot (sistema não empilhável)
            slots[slotIndex].item = null;
            slots[slotIndex].quantity = 0;
            slots[slotIndex].instanceID = 0;
            quickSlots[direction] = 0;
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

            int instanceID = quickSlots[direction];

            if (instanceID == 0)
            {
                return null;
            }

            // Busca o slot que contém o item com este instanceID
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].instanceID == instanceID)
                {
                    return slots[i].item;
                }
            }

            return null;
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

            // Troca os conteúdos dos slots (incluindo instanceID)
            InventorySlot temp = new InventorySlot
            {
                item = slots[slotIndex1].item,
                quantity = slots[slotIndex1].quantity,
                instanceID = slots[slotIndex1].instanceID
            };

            slots[slotIndex1].item = slots[slotIndex2].item;
            slots[slotIndex1].quantity = slots[slotIndex2].quantity;
            slots[slotIndex1].instanceID = slots[slotIndex2].instanceID;

            slots[slotIndex2].item = temp.item;
            slots[slotIndex2].quantity = temp.quantity;
            slots[slotIndex2].instanceID = temp.instanceID;

            UnityEngine.Debug.Log($"[InventoryManager] ✓ Swap entre slots {slotIndex1} e {slotIndex2} concluído.");
            OnInventoryChanged?.Invoke();
            OnQuickSlotsChanged?.Invoke(); // Quick slots podem referenciar os instanceIDs que mudaram de posição

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
                        slotIndex = i,
                        instanceID = slots[i].instanceID
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

            // Salvar índices dos quick slots
            for (int i = 0; i < quickSlots.Length; i++)
            {
                saveData.quickSlotIndices[i] = quickSlots[i];
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
            quickSlots = new int[4] { 0, 0, 0, 0 };
            usedInstanceIDs.Clear();

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

                    // Recriar slot com item correto (sempre quantidade 1) e instanceID
                    slots[itemData.slotIndex].item = item;
                    slots[itemData.slotIndex].quantity = 1;
                    slots[itemData.slotIndex].instanceID = itemData.instanceID;

                    // Adiciona instanceID ao HashSet
                    if (itemData.instanceID > 0)
                    {
                        usedInstanceIDs.Add(itemData.instanceID);
                    }
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
            if (saveData.quickSlotIndices != null)
            {
                for (int i = 0; i < saveData.quickSlotIndices.Length && i < quickSlots.Length; i++)
                {
                    quickSlots[i] = saveData.quickSlotIndices[i];
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
