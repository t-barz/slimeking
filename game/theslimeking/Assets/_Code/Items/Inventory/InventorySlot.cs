using SlimeKing.Items;

namespace TheSlimeKing.Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int quantity;

        /// <summary>
        /// ID único da instância do item (6 dígitos: 100000-999999).
        /// Cada item no inventário tem um ID único, mesmo se forem do mesmo tipo.
        /// 0 = slot vazio (sem ID atribuído)
        /// </summary>
        public int instanceID;

        public bool IsEmpty => item == null;

        public bool CanStack(ItemData newItem)
        {
            // Sistema não empilhável - sempre retorna false
            return false;
        }
    }
}
