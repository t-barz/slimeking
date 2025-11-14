using UnityEngine;

namespace TheSlimeKing.Inventory
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "The Slime King/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName;
        public Sprite icon;
        public ItemType type;
        public bool isStackable = true;

        [Header("Consumable Properties")]
        public int healAmount;

        [Header("Equipment Properties")]
        public EquipmentType equipmentType;
        public int defenseBonus;
        public int speedBonus;

        [Header("Quest Properties")]
        public bool isQuestItem;
    }
}
