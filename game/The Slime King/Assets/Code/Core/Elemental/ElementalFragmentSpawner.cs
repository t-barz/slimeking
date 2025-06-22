using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Core.Elemental
{
    /// <summary>
    /// Gera fragmentos elementais no mundo com base em configurações
    /// </summary>
    public class ElementalFragmentSpawner : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private GameObject _smallFragmentPrefab;
        [SerializeField] private GameObject _mediumFragmentPrefab;
        [SerializeField] private GameObject _largeFragmentPrefab;

        [Header("Configurações de Spawn")]
        [SerializeField] private float _minSpawnRadius = 1.0f;
        [SerializeField] private float _maxSpawnRadius = 3.0f;
        [SerializeField] private float _minVerticalOffset = 0.5f;
        [SerializeField] private float _maxVerticalOffset = 1.5f;

        /// <summary>
        /// Enum que define o perfil de spawn de fragmentos
        /// </summary>
        public enum SpawnProfile
        {
            Balanced,   // Mix equilibrado de tamanhos
            SmallOnly,  // Apenas fragmentos pequenos
            MediumFocus, // Foco em fragmentos médios
            LargeFocus,  // Foco em fragmentos grandes
            Random      // Totalmente aleatório
        }

        /// <summary>
        /// Configuração para spawn de fragmentos
        /// </summary>
        [System.Serializable]
        public class SpawnConfig
        {
            public ElementalType elementType = ElementalType.None;
            public int minAmount = 1;
            public int maxAmount = 3;
            public bool randomElement = false;
            public SpawnProfile profile = SpawnProfile.Balanced;
            public List<ElementalFragment.FragmentSize> allowedSizes = new List<ElementalFragment.FragmentSize>();
        }

        /// <summary>
        /// Gera fragmentos elementais no ponto especificado com as configurações dadas
        /// </summary>
        public void SpawnFragments(Vector3 origin, SpawnConfig config)
        {
            if (config == null)
                return;

            // Determina quantidade a spawnar
            int amount = Random.Range(config.minAmount, config.maxAmount + 1);

            // Tamanhos permitidos
            List<ElementalFragment.FragmentSize> sizesToSpawn = new List<ElementalFragment.FragmentSize>();

            // Se tamanhos específicos definidos, usa eles
            if (config.allowedSizes != null && config.allowedSizes.Count > 0)
            {
                sizesToSpawn = config.allowedSizes;
            }
            else
            {
                // Senão configura baseado no perfil
                switch (config.profile)
                {
                    case SpawnProfile.Balanced:
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Small);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Medium);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Large);
                        break;

                    case SpawnProfile.SmallOnly:
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Small);
                        break;

                    case SpawnProfile.MediumFocus:
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Small);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Medium);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Medium);
                        break;

                    case SpawnProfile.LargeFocus:
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Medium);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Large);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Large);
                        break;

                    case SpawnProfile.Random:
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Small);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Medium);
                        sizesToSpawn.Add(ElementalFragment.FragmentSize.Large);
                        break;
                }
            }

            // Gera os fragmentos
            for (int i = 0; i < amount; i++)
            {
                // Seleciona tamanho do fragmento
                ElementalFragment.FragmentSize size = sizesToSpawn[Random.Range(0, sizesToSpawn.Count)];

                // Define tipo elemental
                ElementalType elementType = config.elementType;

                // Se for elemento aleatório, seleciona um
                if (config.randomElement)
                {
                    ElementalType[] types = {
                        ElementalType.Earth,
                        ElementalType.Water,
                        ElementalType.Fire,
                        ElementalType.Air
                    };

                    elementType = types[Random.Range(0, types.Length)];
                }

                // Posição de spawn com offset aleatório
                Vector2 randomDir = Random.insideUnitCircle.normalized;
                float randomDistance = Random.Range(_minSpawnRadius, _maxSpawnRadius);
                float randomHeight = Random.Range(_minVerticalOffset, _maxVerticalOffset);

                Vector3 spawnPosition = origin + new Vector3(randomDir.x * randomDistance, randomHeight, randomDir.y * randomDistance);

                // Cria o fragmento
                SpawnFragmentAt(spawnPosition, elementType, size);
            }
        }

        /// <summary>
        /// Gera um único fragmento elemental na posição especificada
        /// </summary>
        public GameObject SpawnFragmentAt(Vector3 position, ElementalType elementType, ElementalFragment.FragmentSize size)
        {
            // Seleciona prefab baseado no tamanho
            GameObject prefab = null;

            switch (size)
            {
                case ElementalFragment.FragmentSize.Small:
                    prefab = _smallFragmentPrefab;
                    break;
                case ElementalFragment.FragmentSize.Medium:
                    prefab = _mediumFragmentPrefab;
                    break;
                case ElementalFragment.FragmentSize.Large:
                    prefab = _largeFragmentPrefab;
                    break;
            }

            // Usa prefab padrão se não tiver específico
            if (prefab == null)
            {
                if (_smallFragmentPrefab != null)
                    prefab = _smallFragmentPrefab;
                else
                {
                    Debug.LogError("Não há prefabs de fragmentos definidos no ElementalFragmentSpawner!");
                    return null;
                }
            }

            // Instancia o prefab
            GameObject fragmentObj = Instantiate(prefab, position, Quaternion.identity);

            // Configura propriedades do fragmento
            ElementalFragment fragment = fragmentObj.GetComponent<ElementalFragment>();

            if (fragment != null)
            {
                // Define cores baseado no tipo elemental
                Color primaryColor = Color.white;
                Color secondaryColor = Color.white;

                switch (elementType)
                {
                    case ElementalType.Earth:
                        primaryColor = HexToColor("#8B4513");
                        secondaryColor = HexToColor("#DEB887");
                        break;
                    case ElementalType.Water:
                        primaryColor = HexToColor("#4169E1");
                        secondaryColor = HexToColor("#87CEEB");
                        break;
                    case ElementalType.Fire:
                        primaryColor = HexToColor("#FF4500");
                        secondaryColor = HexToColor("#FFA500");
                        break;
                    case ElementalType.Air:
                        primaryColor = HexToColor("#E6E6FA");
                        secondaryColor = HexToColor("#F0F8FF");
                        break;
                }

                // Inicializa o fragmento
                fragment.Initialize(elementType, size, primaryColor, secondaryColor);
            }

            return fragmentObj;
        }

        /// <summary>
        /// Converte cor em formato hex para Color
        /// </summary>
        private Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
                return color;

            return Color.white;
        }
    }
}
