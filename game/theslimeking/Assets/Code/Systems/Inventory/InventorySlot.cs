namespace TheSlimeKing.Inventory
{
    [System.Serializable]
    public class InventorySlot
    {
        public ItemData item;
        public int quantity;

        public bool IsEmpty => item == null;

        public bool CanStack(ItemData newItem)
        {
            // Sistema não empilhável - sempre retorna false
            return false;
        }
    }
}
