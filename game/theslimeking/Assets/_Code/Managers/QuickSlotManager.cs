using System;
using UnityEngine;
using SlimeKing.Core;
using SlimeKing.Items;

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
    /// </summary>
    public class QuickSlotManager : ManagerSingleton<QuickSlotManager>
    {
        public const int SlotCount = 4;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private QuickSlotData[] quickSlots = new QuickSlotData[SlotCount];

        /// <summary>
        /// Evento disparado quando um quickslot é alterado.
        /// Parâmetros: índice do quickslot (0-3), item atribuído (null se removido)
        /// </summary>
        public event Action<int, ItemData> OnQuickSlotChanged;

        protected override void Initialize()
        {
            quickSlots = new QuickSlotData[SlotCount];
            for (int i = 0; i < SlotCount; i++)
            {
                quickSlots[i] = QuickSlotData.Empty;
            }
            Log("QuickSlotManager inicializado");
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
