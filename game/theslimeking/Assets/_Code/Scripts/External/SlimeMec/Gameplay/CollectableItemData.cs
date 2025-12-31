using UnityEngine;

namespace SlimeKing.External.SlimeMec.Items
{
    /// <summary>
    /// Dados de configuração para itens coletáveis.
    /// Design otimizado para performance e simplicidade.
    /// </summary>
    [CreateAssetMenu(fileName = "CollectableItem", menuName = "SlimeMec/Collectable Item")]
    public class CollectableItemData : ScriptableObject
    {
        [Header("📋 Visual")]
        public string itemName = "Item";
        public Color itemTint = Color.white;

        [Header("💚 Cura (0 = desabilitado)")]
        [Range(0, 100)] public int healAmount = 0;
        public bool isFullHeal = false;

        [Header("⭐ Experiência (0 = desabilitado)")]
        [Range(0, 1000)] public int xpPoints = 0;
        [Range(0, 10)] public int skillPoints = 0;

        [Header("🚀 Buffs Temporários (0 = desabilitado)")]
        [Range(0, 10)] public int attackBuff = 0;
        [Range(0, 10)] public int defenseBuff = 0;
        [Range(0, 10)] public int speedBuff = 0;
        [Range(0f, 300f)] public float buffDuration = 30f;

        [Header("⚙️ Comportamento")]
        [Range(0.5f, 5f)] public float detectionRadius = 2f;
        [Range(5f, 20f)] public float attractSpeed = 10f;

        [Header("🎨 Efeitos")]
        public GameObject vfxPrefab;
        public AudioClip collectSound;

        // Propriedades de performance (calculadas uma vez)
        public bool HasHealEffect => healAmount > 0 || isFullHeal;
        public bool HasXPEffect => xpPoints > 0 || skillPoints > 0;
        public bool HasBuffEffect => attackBuff > 0 || defenseBuff > 0 || speedBuff > 0;
        public bool HasAnyEffect => HasHealEffect || HasXPEffect || HasBuffEffect;
    }
}
