using UnityEngine;
using UnityEngine.Localization;

namespace SlimeKing.Items
{
    /// <summary>
    /// Define os tipos de itens disponíveis no jogo.
    /// </summary>
    public enum ItemType
    {
        Consumable,  // Itens que podem ser usados/consumidos
        Material,    // Itens para construção/crafting
        Equipment    // Equipamento (máscaras, chapéus, capas)
    }

    /// <summary>
    /// Representa um efeito de buff ou debuff que um item pode aplicar.
    /// Apenas modifica os atributos existentes do PlayerAttributesHandler.
    /// </summary>
    [System.Serializable]
    public class ItemEffect
    {
        [Header("Effect Configuration")]
        [SerializeField] public string effectName = "New Effect";
        [SerializeField] public string description = "";

        [Header("Attribute Modifiers")]
        [SerializeField] public int healthBonus = 0;
        [SerializeField] public int attackBonus = 0;
        [SerializeField] public int defenseBonus = 0;
        [SerializeField] public int speedBonus = 0;
        [SerializeField] public int skillPointsBonus = 0;

        /// <summary>
        /// Verifica se este efeito tem alguma modificação
        /// </summary>
        public bool HasEffect()
        {
            return healthBonus != 0 || attackBonus != 0 || defenseBonus != 0 || 
                   speedBonus != 0 || skillPointsBonus != 0;
        }
    }

    /// <summary>
    /// ScriptableObject que define as propriedades de um item no jogo.
    /// Pode ser um consumível (buff/debuff), material (crafting) ou equipamento (máscaras, chapéus, capas).
    /// </summary>
    public class ItemData : ScriptableObject
    {
        [Header("Item Identity")]
        [SerializeField] public LocalizedString itemName;
        [SerializeField] public LocalizedString description;
        [SerializeField] public Sprite icon;

        [Header("Item Type")]
        [SerializeField] public ItemType itemType = ItemType.Consumable;

        [Header("Item Effect")]
        [SerializeField] public ItemEffect itemEffect;

        [Header("Consumable Settings")]
        [SerializeField] public bool isOneTime = true;
        [Tooltip("Duração do efeito em segundos (0 = permanente)")]
        [SerializeField] public float effectDuration = 0f;

        [Header("Equipment Settings")]
        [Tooltip("Nome técnico do slot de equipamento (mask, hat, cape, etc)")]
        [SerializeField] public string equipmentSlot = "";

        /// <summary>
        /// Verifica se este item tem algum efeito associado
        /// </summary>
        public bool HasEffect()
        {
            return itemEffect != null && itemEffect.HasEffect();
        }

        /// <summary>
        /// Obtém uma descrição completa do item incluindo efeitos
        /// </summary>
        public string GetFullDescription()
        {
            string full = GetLocalizedDescription();

            if (HasEffect() && itemEffect != null)
            {
                full += $"\n\n<b>Efeitos:</b>\n";
                
                if (itemEffect.healthBonus != 0)
                    full += $"HP: {(itemEffect.healthBonus > 0 ? "+" : "")}{itemEffect.healthBonus}\n";
                if (itemEffect.attackBonus != 0)
                    full += $"Ataque: {(itemEffect.attackBonus > 0 ? "+" : "")}{itemEffect.attackBonus}\n";
                if (itemEffect.defenseBonus != 0)
                    full += $"Defesa: {(itemEffect.defenseBonus > 0 ? "+" : "")}{itemEffect.defenseBonus}\n";
                if (itemEffect.speedBonus != 0)
                    full += $"Velocidade: {(itemEffect.speedBonus > 0 ? "+" : "")}{itemEffect.speedBonus}\n";
                if (itemEffect.skillPointsBonus != 0)
                    full += $"Pontos de Habilidade: {(itemEffect.skillPointsBonus > 0 ? "+" : "")}{itemEffect.skillPointsBonus}";
            }

            return full;
        }

        /// <summary>
        /// Retorna o nome do item localizado.
        /// </summary>
        public string GetLocalizedName()
        {
            return itemName.IsEmpty ? string.Empty : itemName.GetLocalizedString();
        }

        /// <summary>
        /// Retorna a descrição localizada do item.
        /// </summary>
        public string GetLocalizedDescription()
        {
            return description.IsEmpty ? string.Empty : description.GetLocalizedString();
        }
    }
}
