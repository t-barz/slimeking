using UnityEngine;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Extensão do InventoryManager para adicionar funcionalidade específica de seleção
    /// </summary>
    public partial class InventoryManager : MonoBehaviour
    {
        /// <summary>
        /// Seleciona um slot específico do inventário
        /// </summary>
        public void SelectSlot(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < _availableSlots)
            {
                _selectedSlotIndex = slotIndex;
                OnSelectedSlotChanged?.Invoke(_selectedSlotIndex);
            }
        }
    }
}
