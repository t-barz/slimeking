using UnityEngine;
using UnityEngine.Localization;

namespace TheSlimeKing.Inventory
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Extra Tools/Items/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName;
        [TextArea(3, 6)]
        public string description;
        public Sprite icon;
        public ItemType type;

        [Header("Localization")]
        [Tooltip("Chave da tabela Items para o nome localizado")]
        public string localizationKey;

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
