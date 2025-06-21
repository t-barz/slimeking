using TheSlimeKing.Gameplay;
using UnityEngine;
using TheSlimeKing.Entities;  // Add this namespace for EnemyStats

// Temporary placeholder until the actual EnemyStats class is implemented
namespace TheSlimeKing.Entities
{
    public class EnemyStats : MonoBehaviour
    {
        // This is a temporary implementation that will be replaced by the actual EnemyStats class
    }
}

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Enum que define os tipos de inimigos disponíveis no jogo
    /// </summary>
    public enum EnemyType
    {
        Normal,
        Elite,
        Miniboss,
        Boss
    }
    /// <summary>
    /// ScriptableObject que define configurações base para cada tipo de inimigo
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "The Slime King/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Informações Básicas")]
        [SerializeField] private string enemyName;
        [SerializeField][TextArea(2, 4)] private string description;
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private bool isBoss;

        [Header("Visual")]
        [SerializeField] private Sprite enemySprite;
        [SerializeField] private RuntimeAnimatorController animatorController;
        [SerializeField] private Color tintColor = Color.white;

        [Header("Atributos")]
        [SerializeField] private int baseHealth;
        [SerializeField] private int baseAttack;
        [SerializeField] private int baseSpecial;
        [SerializeField] private int baseDefense;
        [SerializeField] private int level = 1;
        [SerializeField] private int experienceValue;

        [Header("Comportamento")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float detectionRange = 5f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 1f;

        [SerializeField] private ElementType primaryElement;
        [SerializeField][Range(0, 1)] private float elementalDropChance = 0.8f;
        [SerializeField] private float elementalEnergyDropAmount = 10f;

        [Header("Drops")]
        [SerializeField] private GameObject[] possibleDrops;
        [SerializeField][Range(0, 1)] private float dropChance = 0.5f;

        // Propriedades públicas
        public string EnemyName => enemyName;
        public string Description => description;
        public EnemyType EnemyType => enemyType;
        public bool IsBoss => isBoss;
        public Sprite EnemySprite => enemySprite;
        public RuntimeAnimatorController AnimatorController => animatorController;
        public Color TintColor => tintColor;
        public int BaseHealth => baseHealth;
        public int BaseAttack => baseAttack;
        public int BaseSpecial => baseSpecial;
        public int BaseDefense => baseDefense;
        public int Level => level;
        public int ExperienceValue => experienceValue;
        public float MoveSpeed => moveSpeed;
        public float DetectionRange => detectionRange;
        public float AttackRange => attackRange;
        public ElementType PrimaryElement => primaryElement;
        public float ElementalEnergyDropAmount => elementalEnergyDropAmount;
        public GameObject[] PossibleDrops => possibleDrops;
        public float DropChance => dropChance;

        /// <summary>
        /// Aplica os dados deste ScriptableObject a um inimigo
        /// </summary>
        /// <param name="enemyStats">Componente EnemyStats para configurar</param>
        public virtual void ApplyToEnemy(EnemyStats enemyStats)
        {
            // Usar reflexão para configurar os campos privados diretamente
            // Não é a abordagem ideal, mas é um atalho para este exemplo
            var type = enemyStats.GetType();

            // Campos básicos
            type.GetField("enemyName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, enemyName);
            type.GetField("description", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, description);
            type.GetField("enemyType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, enemyType);
            type.GetField("isBoss", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, isBoss);
            type.GetField("experienceValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, experienceValue);

            // Campos de movimento
            type.GetField("moveSpeed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, moveSpeed);
            type.GetField("detectionRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, detectionRange);
            type.GetField("attackRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, attackRange);
            type.GetField("attackCooldown", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, attackCooldown);

            // Campos elementais
            type.GetField("primaryElement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, primaryElement);
            type.GetField("elementalDropChance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, elementalDropChance);
            type.GetField("elementalEnergyDropAmount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, elementalEnergyDropAmount);

            // Campos de drops
            type.GetField("possibleDrops", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, possibleDrops);
            type.GetField("dropChance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, dropChance);

            // Configure atributos base usando o método SetValue para o ModifiableStat
            type.GetField("baseHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, baseHealth);
            type.GetField("baseAttack", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, baseAttack);
            type.GetField("baseSpecial", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, baseSpecial);
            type.GetField("baseDefense", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, baseDefense);
            type.GetField("baseLevel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(enemyStats, level);

            // Configura o visual
            SpriteRenderer spriteRenderer = enemyStats.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && enemySprite != null)
            {
                spriteRenderer.sprite = enemySprite;
                spriteRenderer.color = tintColor;
            }

            // Configura animações
            Animator animator = enemyStats.GetComponent<Animator>();
            if (animator != null && animatorController != null)
            {
                animator.runtimeAnimatorController = animatorController;
            }
        }
    }
}
