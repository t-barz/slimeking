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
            return item == newItem && quantity < 99 && item != null && item.isStackable;
        }
    }
}
