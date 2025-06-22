using UnityEngine;
using System.Collections.Generic;

namespace TheSlimeKing.Core.Elemental
{
    /// <summary>
    /// ScriptableObject para configuração de geração de fragmentos elementais
    /// </summary>
    [CreateAssetMenu(fileName = "NovoFragmentoConfig", menuName = "The Slime King/Elemental/Fragmento Config")]
    public class ElementalFragmentConfig : ScriptableObject
    {
        [Header("Configurações Visuais")]
        [SerializeField] private Sprite _smallSprite;
        [SerializeField] private Sprite _mediumSprite;
        [SerializeField] private Sprite _largeSprite;

        [Header("Cores")]
        [SerializeField] private Color _earthPrimary = new Color(0.545f, 0.271f, 0.075f); // #8B4513
        [SerializeField] private Color _earthSecondary = new Color(0.871f, 0.722f, 0.529f); // #DEB887
        [SerializeField] private Color _waterPrimary = new Color(0.255f, 0.412f, 0.882f); // #4169E1
        [SerializeField] private Color _waterSecondary = new Color(0.529f, 0.808f, 0.922f); // #87CEEB
        [SerializeField] private Color _firePrimary = new Color(1.000f, 0.271f, 0.000f); // #FF4500
        [SerializeField] private Color _fireSecondary = new Color(1.000f, 0.647f, 0.000f); // #FFA500
        [SerializeField] private Color _airPrimary = new Color(0.902f, 0.902f, 0.980f); // #E6E6FA
        [SerializeField] private Color _airSecondary = new Color(0.941f, 0.973f, 1.000f); // #F0F8FF

        [Header("Configurações de Geração")]
        [SerializeField] private List<DropConfig> _dropConfigs = new List<DropConfig>();

        /// <summary>
        /// Configuração de drop por tipo de objeto
        /// </summary>
        [System.Serializable]
        public class DropConfig
        {
            public string sourceType; // Nome do tipo de objeto que pode dropar (ex: "GrassPlant", "FireCrystal", etc)
            public ElementalType fixedElementType = ElementalType.None; // Tipo fixo ou None para aleatório
            public bool randomElement = false; // Se verdadeiro, ignora fixedElementType e usa elemento aleatório

            [Header("Quantidades")]
            public int minDropCount = 1;
            public int maxDropCount = 3;

            [Header("Tamanhos Permitidos")]
            public bool allowSmall = true;
            public bool allowMedium = true;
            public bool allowLarge = false;

            [Header("Chances por Tamanho")]
            [Range(0f, 1f)]
            public float smallChance = 0.7f;
            [Range(0f, 1f)]
            public float mediumChance = 0.25f;
            [Range(0f, 1f)]
            public float largeChance = 0.05f;

            /// <summary>
            /// Retorna a lista de tamanhos permitidos baseado nas configurações
            /// </summary>
            public List<ElementalFragment.FragmentSize> GetAllowedSizes()
            {
                List<ElementalFragment.FragmentSize> sizes = new List<ElementalFragment.FragmentSize>();

                if (allowSmall)
                    sizes.Add(ElementalFragment.FragmentSize.Small);

                if (allowMedium)
                    sizes.Add(ElementalFragment.FragmentSize.Medium);

                if (allowLarge)
                    sizes.Add(ElementalFragment.FragmentSize.Large);

                // Garante que pelo menos um tamanho será retornado
                if (sizes.Count == 0)
                    sizes.Add(ElementalFragment.FragmentSize.Small);

                return sizes;
            }

            /// <summary>
            /// Seleciona um tamanho de fragmento baseado nas configurações de chance
            /// </summary>
            public ElementalFragment.FragmentSize SelectRandomFragmentSize()
            {
                // Verifica tamanhos permitidos
                List<ElementalFragment.FragmentSize> allowedSizes = GetAllowedSizes();

                if (allowedSizes.Count == 1)
                    return allowedSizes[0];

                // Calcula soma total das chances para normalização
                float totalChance = 0f;
                foreach (var size in allowedSizes)
                {
                    switch (size)
                    {
                        case ElementalFragment.FragmentSize.Small:
                            totalChance += smallChance;
                            break;
                        case ElementalFragment.FragmentSize.Medium:
                            totalChance += mediumChance;
                            break;
                        case ElementalFragment.FragmentSize.Large:
                            totalChance += largeChance;
                            break;
                    }
                }

                // Caso total seja 0, atribui chances iguais
                if (totalChance <= 0f)
                {
                    float equalChance = 1f / allowedSizes.Count;

                    if (allowedSizes.Contains(ElementalFragment.FragmentSize.Small))
                        smallChance = equalChance;
                    if (allowedSizes.Contains(ElementalFragment.FragmentSize.Medium))
                        mediumChance = equalChance;
                    if (allowedSizes.Contains(ElementalFragment.FragmentSize.Large))
                        largeChance = equalChance;

                    totalChance = 1f;
                }

                // Escolhe tamanho baseado nas chances relativas
                float randomValue = Random.Range(0f, totalChance);
                float cumulativeChance = 0f;

                // Testa small
                if (allowedSizes.Contains(ElementalFragment.FragmentSize.Small))
                {
                    cumulativeChance += smallChance;
                    if (randomValue <= cumulativeChance)
                        return ElementalFragment.FragmentSize.Small;
                }

                // Testa medium
                if (allowedSizes.Contains(ElementalFragment.FragmentSize.Medium))
                {
                    cumulativeChance += mediumChance;
                    if (randomValue <= cumulativeChance)
                        return ElementalFragment.FragmentSize.Medium;
                }

                // Se chegou aqui e large está permitido, retorna large
                if (allowedSizes.Contains(ElementalFragment.FragmentSize.Large))
                    return ElementalFragment.FragmentSize.Large;

                // Fallback para small
                return ElementalFragment.FragmentSize.Small;
            }
        }

        /// <summary>
        /// Retorna a configuração de drop para o tipo de objeto especificado
        /// </summary>
        public DropConfig GetDropConfigForType(string sourceType)
        {
            foreach (var config in _dropConfigs)
            {
                if (config.sourceType == sourceType)
                    return config;
            }

            // Retorna configuração padrão se não encontrar específica
            return GetDefaultDropConfig();
        }

        /// <summary>
        /// Retorna uma configuração de drop padrão
        /// </summary>
        public DropConfig GetDefaultDropConfig()
        {
            return new DropConfig
            {
                sourceType = "Default",
                fixedElementType = ElementalType.None,
                randomElement = true,
                minDropCount = 1,
                maxDropCount = 3,
                allowSmall = true,
                allowMedium = true,
                allowLarge = false,
                smallChance = 0.7f,
                mediumChance = 0.25f,
                largeChance = 0.05f
            };
        }

        /// <summary>
        /// Retorna o sprite apropriado para o tamanho de fragmento
        /// </summary>
        public Sprite GetSpriteForSize(ElementalFragment.FragmentSize size)
        {
            switch (size)
            {
                case ElementalFragment.FragmentSize.Small:
                    return _smallSprite;
                case ElementalFragment.FragmentSize.Medium:
                    return _mediumSprite;
                case ElementalFragment.FragmentSize.Large:
                    return _largeSprite;
                default:
                    return _smallSprite;
            }
        }

        /// <summary>
        /// Retorna as cores para o tipo elemental
        /// </summary>
        public (Color primary, Color secondary) GetColorsForElementalType(ElementalType type)
        {
            switch (type)
            {
                case ElementalType.Earth:
                    return (_earthPrimary, _earthSecondary);
                case ElementalType.Water:
                    return (_waterPrimary, _waterSecondary);
                case ElementalType.Fire:
                    return (_firePrimary, _fireSecondary);
                case ElementalType.Air:
                    return (_airPrimary, _airSecondary);
                default:
                    return (Color.white, Color.white);
            }
        }
    }
}
