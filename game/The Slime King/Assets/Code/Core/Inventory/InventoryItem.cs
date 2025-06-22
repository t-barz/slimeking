using UnityEngine;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Representa uma instância de um item no inventário
    /// </summary>
    [System.Serializable]
    public class InventoryItem
    {
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _quantity;

        // Construtor
        public InventoryItem(ItemData itemData, int quantity = 1)
        {
            _itemData = itemData;
            _quantity = Mathf.Clamp(quantity, 0, itemData != null ? itemData.MaxStack : 1);
        }

        // Propriedades de acesso
        public ItemData ItemData => _itemData;
        public int Quantity => _quantity;
        public bool IsEmpty => _itemData == null || _quantity <= 0;
        public bool IsFull => _itemData != null && _quantity >= _itemData.MaxStack;

        /// <summary>
        /// Adiciona uma quantidade ao item
        /// </summary>
        /// <returns>Quantidade que não pôde ser adicionada (se ultrapassar max stack)</returns>
        public int AddQuantity(int amount)
        {
            if (_itemData == null || amount <= 0)
                return amount;

            int maxToAdd = _itemData.MaxStack - _quantity;
            int actuallyAdded = Mathf.Min(amount, maxToAdd);

            _quantity += actuallyAdded;
            return amount - actuallyAdded; // Retorna o excedente
        }

        /// <summary>
        /// Remove uma quantidade do item
        /// </summary>
        /// <returns>Quantidade efetivamente removida</returns>
        public int RemoveQuantity(int amount)
        {
            if (_itemData == null || amount <= 0)
                return 0;

            int toRemove = Mathf.Min(amount, _quantity);
            _quantity -= toRemove;
            return toRemove;
        }

        /// <summary>
        /// Verifica se pode combinar com outro item (mesmo tipo)
        /// </summary>
        public bool CanStackWith(InventoryItem other)
        {
            if (other == null || other.IsEmpty || IsEmpty || IsFull)
                return false;

            return _itemData == other.ItemData;
        }
    }
}
