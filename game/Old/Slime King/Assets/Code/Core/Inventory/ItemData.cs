using UnityEngine;
using System;

namespace SlimeKing.Core.Inventory
{
    [CreateAssetMenu(fileName = "New Item", menuName = "SlimeKing/Inventory/Item")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] private string itemName;
        [SerializeField] private Sprite icon;
        [SerializeField] private string description;
        [SerializeField] private bool isStackable;
        [SerializeField] private int maxStackSize = 1;
        [SerializeField] private ItemType itemType;

        public string ItemName { get => itemName; set => itemName = value; }
        public Sprite Icon { get => icon; set => icon = value; }
        public string Description { get => description; set => description = value; }
        public bool IsStackable { get => isStackable; set => isStackable = value; }
        public int MaxStackSize { get => maxStackSize; set => maxStackSize = value; }
        public ItemType Type { get => itemType; set => itemType = value; }
    }

    public enum ItemType
    {
        Consumable,
        Equipment,
        Key,
        Resource
    }
}
