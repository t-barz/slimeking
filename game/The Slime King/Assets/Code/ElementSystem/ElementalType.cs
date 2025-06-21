using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// ScriptableObject que define as propriedades de um tipo elemental
    /// </summary>
    [CreateAssetMenu(fileName = "NewElementalType", menuName = "The Slime King/Elemental Type")]
    public class ElementalType : ScriptableObject
    {
        [Header("Informações Básicas")]
        [SerializeField] private string elementName;
        [SerializeField] private Color elementColor;
        [SerializeField][TextArea(3, 5)] private string description;

        [Header("Propriedades Elementais")]
        [SerializeField] private ElementType elementType;
        [SerializeField, Range(0.1f, 2.0f)] private float damageModifier = 1.0f;
        [SerializeField, Range(0.1f, 2.0f)] private float absorptionRate = 1.0f;

        [Header("Recursos Visuais")]
        [SerializeField] private Sprite elementIcon;
        [SerializeField] private ParticleSystem absorptionEffect;
        [SerializeField] private AudioClip absorptionSound;

        // Propriedades públicas
        public string ElementName => elementName;
        public Color ElementColor => elementColor;
        public string Description => description;
        public ElementType ElementType => elementType;
        public float DamageModifier => damageModifier;
        public float AbsorptionRate => absorptionRate;
        public Sprite ElementIcon => elementIcon;
        public ParticleSystem AbsorptionEffect => absorptionEffect;
        public AudioClip AbsorptionSound => absorptionSound;

        /// <summary>
        /// Calcula o dano modificado por esse elemento contra outro elemento
        /// </summary>
        /// <param name="targetElement">Elemento do alvo</param>
        /// <param name="baseDamage">Dano base antes da modificação</param>
        /// <returns>Dano modificado após cálculos elementais</returns>
        public float CalculateElementalDamage(ElementType targetElement, float baseDamage)
        {
            // Implementação básica - pode ser expandida com uma matriz de efetividade
            float modifier = 1.0f;

            // Terra é forte contra Ar, fraco contra Água
            if (elementType == ElementType.Earth)
            {
                if (targetElement == ElementType.Air) modifier = 1.5f;
                else if (targetElement == ElementType.Water) modifier = 0.75f;
            }
            // Água é forte contra Fogo, fraca contra Terra
            else if (elementType == ElementType.Water)
            {
                if (targetElement == ElementType.Fire) modifier = 1.5f;
                else if (targetElement == ElementType.Earth) modifier = 0.75f;
            }
            // Fogo é forte contra Terra, fraco contra Água
            else if (elementType == ElementType.Fire)
            {
                if (targetElement == ElementType.Earth) modifier = 1.5f;
                else if (targetElement == ElementType.Water) modifier = 0.75f;
            }
            // Ar é forte contra Água, fraco contra Terra
            else if (elementType == ElementType.Air)
            {
                if (targetElement == ElementType.Water) modifier = 1.5f;
                else if (targetElement == ElementType.Earth) modifier = 0.75f;
            }

            return baseDamage * modifier * damageModifier;
        }
    }
}
