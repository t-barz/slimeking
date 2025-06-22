using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Core.Elemental.Examples
{
    /// <summary>
    /// Script de exemplo para testar o sistema de absorção elemental
    /// </summary>
    public class ElementalSystemTester : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private ElementalEnergyManager _energyManager;
        [SerializeField] private ElementalFragmentSpawner _fragmentSpawner;
        [SerializeField] private ElementalAbilityManager _abilityManager;

        [Header("Configurações de Teste")]
        [SerializeField] private KeyCode _spawnEarthKey = KeyCode.Alpha1;
        [SerializeField] private KeyCode _spawnWaterKey = KeyCode.Alpha2;
        [SerializeField] private KeyCode _spawnFireKey = KeyCode.Alpha3;
        [SerializeField] private KeyCode _spawnAirKey = KeyCode.Alpha4;
        [SerializeField] private KeyCode _spawnRandomFragmentKey = KeyCode.R;

        [SerializeField] private int _testEnergyAmount = 10;
        [SerializeField] private ElementalFragmentSpawner.SpawnProfile _spawnProfile = ElementalFragmentSpawner.SpawnProfile.Balanced;
        [SerializeField] private int _minFragmentsToSpawn = 1;
        [SerializeField] private int _maxFragmentsToSpawn = 5;

        private void Update()
        {
            // Testa spawns
            if (Input.GetKeyDown(_spawnEarthKey))
                SpawnTestFragments(ElementalType.Earth);

            if (Input.GetKeyDown(_spawnWaterKey))
                SpawnTestFragments(ElementalType.Water);

            if (Input.GetKeyDown(_spawnFireKey))
                SpawnTestFragments(ElementalType.Fire);

            if (Input.GetKeyDown(_spawnAirKey))
                SpawnTestFragments(ElementalType.Air);

            if (Input.GetKeyDown(_spawnRandomFragmentKey))
                SpawnRandomFragments();
        }

        /// <summary>
        /// Spawna fragmentos para testes do tipo específico
        /// </summary>
        private void SpawnTestFragments(ElementalType elementType)
        {
            if (_fragmentSpawner == null)
            {
                Debug.LogWarning("FragmentSpawner não definido no ElementalSystemTester!");
                return;
            }

            // Configura parâmetros de spawn
            ElementalFragmentSpawner.SpawnConfig config = new ElementalFragmentSpawner.SpawnConfig
            {
                elementType = elementType,
                minAmount = _minFragmentsToSpawn,
                maxAmount = _maxFragmentsToSpawn,
                randomElement = false,
                profile = _spawnProfile
            };

            // Adiciona todos os tamanhos possíveis
            config.allowedSizes.Add(ElementalFragment.FragmentSize.Small);
            config.allowedSizes.Add(ElementalFragment.FragmentSize.Medium);
            config.allowedSizes.Add(ElementalFragment.FragmentSize.Large);

            // Realiza o spawn
            _fragmentSpawner.SpawnFragments(transform.position, config);

            Debug.Log($"Spawned {elementType} fragments for testing");
        }

        /// <summary>
        /// Spawna fragmentos de tipos aleatórios
        /// </summary>
        private void SpawnRandomFragments()
        {
            if (_fragmentSpawner == null)
                return;

            // Configura parâmetros de spawn
            ElementalFragmentSpawner.SpawnConfig config = new ElementalFragmentSpawner.SpawnConfig
            {
                elementType = ElementalType.None,
                minAmount = _minFragmentsToSpawn,
                maxAmount = _maxFragmentsToSpawn,
                randomElement = true,
                profile = _spawnProfile
            };

            // Adiciona todos os tamanhos possíveis
            config.allowedSizes.Add(ElementalFragment.FragmentSize.Small);
            config.allowedSizes.Add(ElementalFragment.FragmentSize.Medium);
            config.allowedSizes.Add(ElementalFragment.FragmentSize.Large);

            // Realiza o spawn
            _fragmentSpawner.SpawnFragments(transform.position, config);

            Debug.Log("Spawned random elemental fragments for testing");
        }

        /// <summary>
        /// Adiciona energia diretamente para testes
        /// </summary>
        public void AddTestEnergy(ElementalType elementType)
        {
            if (_energyManager == null)
                return;

            _energyManager.AddElementalEnergy(elementType, _testEnergyAmount);
            Debug.Log($"Added {_testEnergyAmount} energy of type {elementType} for testing");
        }
    }
}
