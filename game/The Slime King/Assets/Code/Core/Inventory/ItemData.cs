using UnityEngine;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Scriptable Object que define um item no jogo
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "The Slime King/Inventory/Item")]
    public class ItemData : ScriptableObject
    {
        [Header("Identificação")]
        [SerializeField] private string _itemId;
        [SerializeField] private string _nameKey;
        [SerializeField] private string _descriptionKey;
        [SerializeField] private Sprite _icon;
        [SerializeField] private GameObject _prefab;

        [Header("Propriedades")]
        [SerializeField] private ItemType _itemType;
        [SerializeField] private ItemRarity _rarity = ItemRarity.Common;
        [SerializeField] private int _maxStack = 10;
        [SerializeField] private bool _isConsumable = true;
        [SerializeField] private bool _isQuest = false;

        [Header("Efeitos")]
        [SerializeField] private AudioClip _useSound;
        [SerializeField] private GameObject _useEffectPrefab;

        // Propriedades de acesso
        public string ItemId => _itemId;
        public string NameKey => _nameKey;
        public string DescriptionKey => _descriptionKey;
        public Sprite Icon => _icon;
        public GameObject Prefab => _prefab;
        public ItemType Type => _itemType;
        public ItemRarity Rarity => _rarity;
        public int MaxStack => _maxStack;
        public bool IsConsumable => _isConsumable;
        public bool IsQuest => _isQuest;
        public AudioClip UseSound => _useSound;
        public GameObject UseEffectPrefab => _useEffectPrefab;

        // Validação de dados
        private void OnValidate()
        {
            // Garante que o ID seja único usando GUID se estiver vazio
            if (string.IsNullOrEmpty(_itemId))
            {
                _itemId = System.Guid.NewGuid().ToString();
            }

            // Garante que o stack máximo seja pelo menos 1
            _maxStack = Mathf.Max(1, _maxStack);
        }
    }

    /// <summary>
    /// Tipos de itens no jogo
    /// </summary>
    public enum ItemType
    {
        Consumable,
        Material,
        QuestItem,
        KeyItem
    }

    /// <summary>
    /// Raridadeidades de itens
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }
}
