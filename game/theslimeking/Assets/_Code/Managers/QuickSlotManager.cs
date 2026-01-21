using System;
using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.Core;
using SlimeKing.Items;
using SlimeKing.Gameplay;

namespace SlimeKing.Core
{
    /// <summary>
    /// Dados de um quickslot: índice do slot no inventário e referência ao item.
    /// </summary>
    [Serializable]
    public struct QuickSlotData
    {
        public int inventorySlotIndex;
        public ItemData item;

        public bool IsEmpty => inventorySlotIndex < 0 || item == null;

        public static QuickSlotData Empty => new QuickSlotData { inventorySlotIndex = -1, item = null };
    }

    /// <summary>
    /// Gerencia os 4 Quick Slots do jogador.
    /// Armazena referências aos itens por índice do slot do inventário para diferenciar instâncias do mesmo tipo.
    /// Também gerencia o uso de itens consumíveis via Skill1-4.
    /// </summary>
    public class QuickSlotManager : ManagerSingleton<QuickSlotManager>
    {
        public const int SlotCount = 4;

        [Header("References")]
        [SerializeField] private PlayerInput playerInput;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private QuickSlotData[] quickSlots = new QuickSlotData[SlotCount];

        private InputAction skill1Action;
        private InputAction skill2Action;
        private InputAction skill3Action;
        private InputAction skill4Action;
        private bool actionsSubscribed;

        /// <summary>
        /// Evento disparado quando um quickslot é alterado.
        /// Parâmetros: índice do quickslot (0-3), item atribuído (null se removido)
        /// </summary>
        public event Action<int, ItemData> OnQuickSlotChanged;

        /// <summary>
        /// Evento disparado quando um item é usado de um quickslot.
        /// Parâmetros: índice do quickslot (0-3), item usado
        /// </summary>
        public event Action<int, ItemData> OnQuickSlotUsed;

        protected override void Initialize()
        {
            quickSlots = new QuickSlotData[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                quickSlots[i] = QuickSlotData.Empty;
            }
            Log("QuickSlotManager inicializado");
        }

        private void OnEnable()
        {
            SetupSkillInputs();
        }

        private void OnDisable()
        {
            UnsubscribeSkillInputs();
        }

        private void SetupSkillInputs()
        {
            if (actionsSubscribed)
            {
                return;
            }

            if (playerInput == null)
            {
                playerInput = FindObjectOfType<PlayerInput>();
            }

            if (playerInput == null || playerInput.actions == null)
            {
                return;
            }

            InputActionAsset actionAsset = playerInput.actions;

            skill1Action = actionAsset.FindAction("Gameplay/Skill1", throwIfNotFound: false);
            skill2Action = actionAsset.FindAction("Gameplay/Skill2", throwIfNotFound: false);
            skill3Action = actionAsset.FindAction("Gameplay/Skill3", throwIfNotFound: false);
            skill4Action = actionAsset.FindAction("Gameplay/Skill4", throwIfNotFound: false);

            if (skill1Action != null)
            {
                skill1Action.performed += OnSkill1;
            }

            if (skill2Action != null)
            {
                skill2Action.performed += OnSkill2;
            }

            if (skill3Action != null)
            {
                skill3Action.performed += OnSkill3;
            }

            if (skill4Action != null)
            {
                skill4Action.performed += OnSkill4;
            }

            actionsSubscribed = true;
            Log("Skill inputs configurados");
        }

        private void UnsubscribeSkillInputs()
        {
            if (!actionsSubscribed)
            {
                return;
            }

            if (skill1Action != null)
            {
                skill1Action.performed -= OnSkill1;
            }

            if (skill2Action != null)
            {
                skill2Action.performed -= OnSkill2;
            }

            if (skill3Action != null)
            {
                skill3Action.performed -= OnSkill3;
            }

            if (skill4Action != null)
            {
                skill4Action.performed -= OnSkill4;
            }

            actionsSubscribed = false;
        }

        private void OnSkill1(InputAction.CallbackContext ctx)
        {
            UseQuickSlot(0);
        }

        private void OnSkill2(InputAction.CallbackContext ctx)
        {
            UseQuickSlot(1);
        }

        private void OnSkill3(InputAction.CallbackContext ctx)
        {
            UseQuickSlot(2);
        }

        private void OnSkill4(InputAction.CallbackContext ctx)
        {
            UseQuickSlot(3);
        }

        /// <summary>
        /// Usa o item no quickslot especificado.
        /// Se for consumível, aplica efeitos e remove do inventário.
        /// </summary>
        private void UseQuickSlot(int quickSlotIndex)
        {
            if (!IsValidSlotIndex(quickSlotIndex))
            {
                return;
            }

            QuickSlotData slotData = quickSlots[quickSlotIndex];
            if (slotData.IsEmpty)
            {
                Log($"QuickSlot {quickSlotIndex + 1}: vazio, nada a usar");
                return;
            }

            ItemData item = slotData.item;

            // Apenas consumíveis podem ser usados
            if (item.itemType != ItemType.Consumable)
            {
                Log($"QuickSlot {quickSlotIndex + 1}: {item.GetLocalizedName()} não é consumível");
                return;
            }

            // Aplica efeitos do item
            ApplyItemEffects(item);

            // Remove do inventário
            if (InventoryManager.HasInstance)
            {
                InventoryManager.Instance.RemoveItem(item, 1);
            }

            // Limpa o quickslot
            quickSlots[quickSlotIndex] = QuickSlotData.Empty;

            Log($"QuickSlot {quickSlotIndex + 1}: {item.GetLocalizedName()} usado e removido");

            OnQuickSlotUsed?.Invoke(quickSlotIndex, item);
            OnQuickSlotChanged?.Invoke(quickSlotIndex, null);
        }

        /// <summary>
        /// Aplica os efeitos de um item consumível ao jogador.
        /// </summary>
        private void ApplyItemEffects(ItemData item)
        {
            if (item == null)
            {
                return;
            }

            PlayerAttributesHandler player = FindObjectOfType<PlayerAttributesHandler>();
            if (player == null)
            {
                LogWarning("PlayerAttributesHandler não encontrado para aplicar efeitos");
                return;
            }

            // Instancia VFX se configurado
            if (item.consumeVFX != null)
            {
                GameObject vfx = Instantiate(item.consumeVFX, player.transform.position, Quaternion.identity, player.transform);
                Log($"VFX instanciado: {item.consumeVFX.name}");
            }

            if (!item.HasEffect())
            {
                return;
            }

            ItemEffect effect = item.itemEffect;

            if (effect.healthBonus > 0)
            {
                player.Heal(effect.healthBonus);
                Log($"Curou {effect.healthBonus} HP");
            }
            else if (effect.healthBonus < 0)
            {
                player.TakeDamage(-effect.healthBonus, true);
                Log($"Causou {-effect.healthBonus} de dano");
            }

            if (effect.attackBonus != 0)
            {
                player.CurrentAttack += effect.attackBonus;
                Log($"Ataque modificado em {effect.attackBonus}");
            }

            if (effect.defenseBonus != 0)
            {
                player.CurrentDefense += effect.defenseBonus;
                Log($"Defesa modificada em {effect.defenseBonus}");
            }

            if (effect.speedBonus != 0)
            {
                player.CurrentSpeed += effect.speedBonus;
                Log($"Velocidade modificada em {effect.speedBonus}");
            }

            if (effect.skillPointsBonus != 0)
            {
                player.AddSkillPoints(effect.skillPointsBonus);
                Log($"Pontos de habilidade adicionados: {effect.skillPointsBonus}");
            }
        }

        /// <summary>
        /// Atribui um item a um quickslot específico.
        /// Usa o índice do slot do inventário para identificar a instância específica.
        /// </summary>
        /// <param name="quickSlotIndex">Índice do quickslot (0-3)</param>
        /// <param name="inventorySlotIndex">Índice do slot no inventário</param>
        /// <param name="item">Item a atribuir (null para limpar)</param>
        public void SetQuickSlot(int quickSlotIndex, int inventorySlotIndex, ItemData item)
        {
            if (!IsValidSlotIndex(quickSlotIndex))
            {
                LogWarning($"Índice de quickslot inválido: {quickSlotIndex}");
                return;
            }

            // Remove do slot anterior se este inventorySlotIndex já estiver em outro quickslot
            if (item != null && inventorySlotIndex >= 0)
            {
                int existingQuickSlot = FindQuickSlotByInventoryIndex(inventorySlotIndex);
                if (existingQuickSlot >= 0 && existingQuickSlot != quickSlotIndex)
                {
                    Log($"QuickSlot {existingQuickSlot + 1}: removendo (movido para quickslot {quickSlotIndex + 1})");
                    quickSlots[existingQuickSlot] = QuickSlotData.Empty;
                    OnQuickSlotChanged?.Invoke(existingQuickSlot, null);
                }
            }

            QuickSlotData previousData = quickSlots[quickSlotIndex];
            
            if (item != null)
            {
                quickSlots[quickSlotIndex] = new QuickSlotData
                {
                    inventorySlotIndex = inventorySlotIndex,
                    item = item
                };
                
                Log($"QuickSlot {quickSlotIndex + 1}: {item.GetLocalizedName()} (inv slot {inventorySlotIndex}) atribuído" +
                    (!previousData.IsEmpty ? $" (substituiu {previousData.item?.GetLocalizedName()})" : ""));
            }
            else
            {
                quickSlots[quickSlotIndex] = QuickSlotData.Empty;
                Log($"QuickSlot {quickSlotIndex + 1}: limpo");
            }

            OnQuickSlotChanged?.Invoke(quickSlotIndex, item);
        }

        /// <summary>
        /// Obtém o item atribuído a um quickslot.
        /// </summary>
        public ItemData GetQuickSlotItem(int quickSlotIndex)
        {
            if (!IsValidSlotIndex(quickSlotIndex))
            {
                return null;
            }

            return quickSlots[quickSlotIndex].item;
        }

        /// <summary>
        /// Obtém os dados completos de um quickslot.
        /// </summary>
        public QuickSlotData GetQuickSlotData(int quickSlotIndex)
        {
            if (!IsValidSlotIndex(quickSlotIndex))
            {
                return QuickSlotData.Empty;
            }

            return quickSlots[quickSlotIndex];
        }

        /// <summary>
        /// Limpa um quickslot específico.
        /// </summary>
        public void ClearQuickSlot(int quickSlotIndex)
        {
            SetQuickSlot(quickSlotIndex, -1, null);
        }

        /// <summary>
        /// Limpa todos os quickslots.
        /// </summary>
        public void ClearAllQuickSlots()
        {
            for (int i = 0; i < SlotCount; i++)
            {
                ClearQuickSlot(i);
            }
        }

        /// <summary>
        /// Encontra qual quickslot contém um determinado índice de inventário.
        /// </summary>
        /// <param name="inventorySlotIndex">Índice do slot no inventário</param>
        /// <returns>Índice do quickslot (0-3) ou -1 se não encontrado</returns>
        public int FindQuickSlotByInventoryIndex(int inventorySlotIndex)
        {
            if (inventorySlotIndex < 0)
            {
                return -1;
            }

            for (int i = 0; i < SlotCount; i++)
            {
                if (quickSlots[i].inventorySlotIndex == inventorySlotIndex)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Obtém todos os itens dos quickslots.
        /// </summary>
        public ItemData[] GetAllQuickSlotItems()
        {
            ItemData[] items = new ItemData[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                items[i] = quickSlots[i].item;
            }
            return items;
        }

        private bool IsValidSlotIndex(int index)
        {
            return index >= 0 && index < SlotCount;
        }

        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[QuickSlotManager] {message}");
            }
        }

        private void LogWarning(string message)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"[QuickSlotManager] {message}");
            }
        }
    }
}
