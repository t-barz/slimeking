using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimeKing.Core.Inventory
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private int initialSize = 2;
        [SerializeField] private int maxSize = 6;

        private List<InventorySlot> slots;
        private int currentSize;

        public int Size => currentSize;
        public int MaxSize => maxSize;
        public IReadOnlyList<InventorySlot> Slots => slots.AsReadOnly();

        public event Action<InventorySlot> OnSlotUpdated;
        public event Action<int> OnSizeChanged;
        public event Action<ItemData, int> OnItemAdded;
        public event Action<ItemData, int> OnItemRemoved;

        private void Awake()
        {
            slots = new List<InventorySlot>();
            SetSize(initialSize);
        }

        public void SetSize(int newSize)
        {
            if (newSize < 0 || newSize > maxSize)
            {
                Debug.LogError($"Tamanho de inventário inválido: {newSize}. Deve estar entre 0 e {maxSize}");
                return;
            }

            // Se estiver diminuindo o tamanho, verifica se há itens nos slots que serão removidos
            if (newSize < currentSize)
            {
                for (int i = newSize; i < currentSize; i++)
                {
                    if (!slots[i].IsEmpty)
                    {
                        Debug.LogError("Não é possível diminuir o inventário enquanto houver itens nos slots");
                        return;
                    }
                }
            }

            // Ajusta o número de slots
            while (slots.Count > newSize)
            {
                slots.RemoveAt(slots.Count - 1);
            }
            while (slots.Count < newSize)
            {
                var slot = new InventorySlot();
                slot.OnUpdated += () => OnSlotUpdated?.Invoke(slot);
                slots.Add(slot);
            }

            currentSize = newSize;
            OnSizeChanged?.Invoke(currentSize);
        }

        public bool AddItem(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0)
                return false;

            // Primeiro, tenta empilhar em slots existentes se o item for empilhável
            if (item.IsStackable)
            {
                foreach (var slot in slots.Where(s => !s.IsEmpty && s.Item == item && s.Amount < item.MaxStackSize))
                {
                    int spaceInSlot = item.MaxStackSize - slot.Amount;
                    int amountToAdd = Mathf.Min(spaceInSlot, amount);

                    slot.AddAmount(amountToAdd);
                    amount -= amountToAdd;

                    if (amount <= 0)
                    {
                        OnItemAdded?.Invoke(item, amountToAdd);
                        return true;
                    }
                }
            }

            // Se ainda houver itens para adicionar, procura slots vazios
            foreach (var slot in slots.Where(s => s.IsEmpty))
            {
                int amountToAdd = item.IsStackable ? Mathf.Min(amount, item.MaxStackSize) : 1;
                slot.SetItem(item, amountToAdd);
                amount -= amountToAdd;

                OnItemAdded?.Invoke(item, amountToAdd);

                if (amount <= 0)
                    return true;

                if (!item.IsStackable)
                    break;
            }

            // Retorna true se todos os itens foram adicionados
            return amount <= 0;
        }

        public bool RemoveItem(ItemData item, int amount = 1)
        {
            if (item == null || amount <= 0)
                return false;

            int remainingToRemove = amount;

            // Remove dos slots que contêm o item
            foreach (var slot in slots.Where(s => !s.IsEmpty && s.Item == item).ToList())
            {
                int amountToRemove = Mathf.Min(remainingToRemove, slot.Amount);
                slot.RemoveAmount(amountToRemove);
                remainingToRemove -= amountToRemove;

                OnItemRemoved?.Invoke(item, amountToRemove);

                if (remainingToRemove <= 0)
                    return true;
            }

            // Retorna true se todos os itens foram removidos
            return remainingToRemove <= 0;
        }

        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= currentSize)
                return null;

            return slots[index];
        }

        public int GetItemCount(ItemData item)
        {
            if (item == null)
                return 0;

            return slots.Where(s => !s.IsEmpty && s.Item == item)
                       .Sum(s => s.Amount);
        }

        public bool HasItem(ItemData item, int amount = 1)
        {
            return GetItemCount(item) >= amount;
        }

        public void Clear()
        {
            foreach (var slot in slots)
            {
                if (!slot.IsEmpty)
                {
                    var item = slot.Item;
                    var amount = slot.Amount;
                    slot.Clear();
                    OnItemRemoved?.Invoke(item, amount);
                }
            }
        }
    }

    public class InventorySlot
    {
        private ItemData item;
        private int amount;

        public event Action OnUpdated;

        public ItemData Item => item;
        public int Amount => amount;
        public bool IsEmpty => item == null;

        public void SetItem(ItemData newItem, int newAmount)
        {
            if (newItem == null)
            {
                Clear();
                return;
            }

            item = newItem;
            amount = Mathf.Clamp(newAmount, 0, newItem.MaxStackSize);
            OnUpdated?.Invoke();
        }

        public void AddAmount(int amountToAdd)
        {
            if (IsEmpty || amountToAdd <= 0)
                return;

            amount = Mathf.Clamp(amount + amountToAdd, 0, item.MaxStackSize);
            OnUpdated?.Invoke();
        }

        public void RemoveAmount(int amountToRemove)
        {
            if (IsEmpty || amountToRemove <= 0)
                return;

            amount -= amountToRemove;

            if (amount <= 0)
                Clear();
            else
                OnUpdated?.Invoke();
        }

        public void Clear()
        {
            item = null;
            amount = 0;
            OnUpdated?.Invoke();
        }
    }
}
