using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Define as configurações para cada estágio de crescimento do Slime
    /// </summary>
    [CreateAssetMenu(fileName = "NewSlimeStage", menuName = "The Slime King/Slime Growth Stage")]
    public class SlimeGrowthStage : ScriptableObject
    {
        [Header("Informações Básicas")]
        [SerializeField] private SlimeStage stageType;
        [SerializeField] private string stageName;
        [SerializeField][TextArea(2, 4)] private string description;
        [SerializeField] private int requiredElementalEnergy;

        [Header("Atributos Base")]
        [SerializeField] private int baseHealth;
        [SerializeField] private int baseAttack;
        [SerializeField] private int baseSpecial;
        [SerializeField] private int baseDefense;
        [SerializeField, Range(0.5f, 2.0f)] private float sizeMultiplier;
        [SerializeField, Range(0.5f, 2.0f)] private float speedMultiplier;

        [Header("Funcionalidades")]
        [SerializeField] private int inventorySlots;
        [SerializeField] private bool canSqueeze;
        [SerializeField] private bool canBounce;
        [SerializeField] private bool canClimb;
        [SerializeField] private bool canUsePowerfulAbilities;

        [Header("Visualização")]
        [SerializeField] private Sprite slimeSprite;
        [SerializeField] private RuntimeAnimatorController animatorController;
        [SerializeField] private ParticleSystem growthEffect;
        [SerializeField] private AudioClip growthSound;

        // Propriedades públicas
        public SlimeStage StageType => stageType;
        public string StageName => stageName;
        public string Description => description;
        public int RequiredElementalEnergy => requiredElementalEnergy;
        public int BaseHealth => baseHealth;
        public int BaseAttack => baseAttack;
        public int BaseSpecial => baseSpecial;
        public int BaseDefense => baseDefense;
        public float SizeMultiplier => sizeMultiplier;
        public float SpeedMultiplier => speedMultiplier;
        public int InventorySlots => inventorySlots;
        public bool CanSqueeze => canSqueeze;
        public bool CanBounce => canBounce;
        public bool CanClimb => canClimb;
        public bool CanUsePowerfulAbilities => canUsePowerfulAbilities;
        public Sprite SlimeSprite => slimeSprite;
        public RuntimeAnimatorController AnimatorController => animatorController;
        public ParticleSystem GrowthEffect => growthEffect;
        public AudioClip GrowthSound => growthSound;
    }
}
