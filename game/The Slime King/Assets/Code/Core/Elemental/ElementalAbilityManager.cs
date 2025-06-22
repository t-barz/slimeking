using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Core.Elemental
{
    /// <summary>
    /// Gerencia as habilidades elementais do jogador
    /// </summary>
    public class ElementalAbilityManager : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private ElementalEnergyManager _energyManager;

        [Header("Configurações")]
        [SerializeField] private float _globalCooldown = 0.5f;
        [SerializeField] private bool _showDebugMessages = false;

        [Header("Habilidades")]
        [SerializeField] private List<ElementalAbility> _abilities = new List<ElementalAbility>();

        // Cache para acesso rápido às habilidades por tipo
        private Dictionary<ElementalType, List<ElementalAbility>> _abilitiesByType = new Dictionary<ElementalType, List<ElementalAbility>>();

        // Cooldowns
        private Dictionary<string, float> _cooldowns = new Dictionary<string, float>();
        private float _globalCooldownTimer = 0f;

        private void Awake()
        {
            // Busca referência ao energy manager se não definida
            if (_energyManager == null)
                _energyManager = GetComponent<ElementalEnergyManager>();

            if (_energyManager == null)
                _energyManager = FindObjectOfType<ElementalEnergyManager>();

            // Organiza habilidades por tipo
            OrganizeAbilities();
        }

        private void Update()
        {
            // Atualiza cooldowns
            UpdateCooldowns();
        }

        /// <summary>
        /// Organiza as habilidades por tipo elemental para acesso mais rápido
        /// </summary>
        private void OrganizeAbilities()
        {
            _abilitiesByType.Clear();

            // Inicializa dicionários por tipo
            _abilitiesByType[ElementalType.Earth] = new List<ElementalAbility>();
            _abilitiesByType[ElementalType.Water] = new List<ElementalAbility>();
            _abilitiesByType[ElementalType.Fire] = new List<ElementalAbility>();
            _abilitiesByType[ElementalType.Air] = new List<ElementalAbility>();

            // Adiciona habilidades nas respectivas listas
            foreach (var ability in _abilities)
            {
                if (ability != null && ability.ElementType != ElementalType.None)
                {
                    _abilitiesByType[ability.ElementType].Add(ability);
                }
            }

            // Debug
            if (_showDebugMessages)
            {
                Debug.Log($"Organizadas {_abilities.Count} habilidades por tipo elemental:");
                Debug.Log($"Terra: {_abilitiesByType[ElementalType.Earth].Count}");
                Debug.Log($"Água: {_abilitiesByType[ElementalType.Water].Count}");
                Debug.Log($"Fogo: {_abilitiesByType[ElementalType.Fire].Count}");
                Debug.Log($"Ar: {_abilitiesByType[ElementalType.Air].Count}");
            }
        }

        /// <summary>
        /// Atualiza os cooldowns das habilidades
        /// </summary>
        private void UpdateCooldowns()
        {
            // Atualiza cooldown global
            if (_globalCooldownTimer > 0)
                _globalCooldownTimer -= Time.deltaTime;

            // Atualiza cooldowns individuais
            List<string> finishedCooldowns = new List<string>();

            foreach (var kvp in _cooldowns)
            {
                _cooldowns[kvp.Key] -= Time.deltaTime;

                if (_cooldowns[kvp.Key] <= 0)
                    finishedCooldowns.Add(kvp.Key);
            }

            // Remove cooldowns finalizados
            foreach (var key in finishedCooldowns)
            {
                _cooldowns.Remove(key);
            }
        }

        /// <summary>
        /// Tenta usar uma habilidade elemental específica
        /// </summary>
        public bool UseAbility(string abilityId)
        {
            // Verifica cooldown global
            if (_globalCooldownTimer > 0)
            {
                if (_showDebugMessages)
                    Debug.Log($"Habilidade em cooldown global: {_globalCooldownTimer:F1}s restantes");

                return false;
            }

            // Procura a habilidade pelo ID
            ElementalAbility ability = _abilities.Find(a => a.AbilityId == abilityId);

            if (ability == null)
            {
                if (_showDebugMessages)
                    Debug.LogWarning($"Habilidade não encontrada: {abilityId}");

                return false;
            }

            // Verifica cooldown específico
            if (_cooldowns.ContainsKey(abilityId) && _cooldowns[abilityId] > 0)
            {
                if (_showDebugMessages)
                    Debug.Log($"Habilidade {abilityId} em cooldown: {_cooldowns[abilityId]:F1}s restantes");

                return false;
            }

            // Verifica se há energia suficiente
            if (_energyManager != null)
            {
                if (_energyManager.GetElementalEnergy(ability.ElementType) < ability.EnergyCost)
                {
                    if (_showDebugMessages)
                        Debug.Log($"Energia insuficiente para usar {abilityId} ({ability.EnergyCost} necessários)");

                    return false;
                }

                // Consome energia
                bool energyUsed = _energyManager.UseElementalEnergy(ability.ElementType, ability.EnergyCost);

                if (!energyUsed)
                {
                    if (_showDebugMessages)
                        Debug.LogWarning($"Falha ao consumir energia para {abilityId}");

                    return false;
                }
            }

            // Aplica cooldowns
            _globalCooldownTimer = _globalCooldown;
            _cooldowns[abilityId] = ability.Cooldown;

            // Ativa a habilidade
            ability.Activate(transform.position);

            // Notifica sobre uso de habilidade
            ElementalEvents.OnElementalAbilityUsed?.Invoke(ability.ElementType, ability.EnergyCost, transform.position);

            if (_showDebugMessages)
                Debug.Log($"Habilidade {abilityId} ativada com sucesso!");

            return true;
        }

        /// <summary>
        /// Tenta usar habilidade baseado no input
        /// </summary>
        public bool UseAbilityFromInput(int abilityIndex)
        {
            if (abilityIndex < 0 || abilityIndex >= _abilities.Count)
                return false;

            return UseAbility(_abilities[abilityIndex].AbilityId);
        }

        /// <summary>
        /// Verifica se uma habilidade está disponível (fora de cooldown e com energia suficiente)
        /// </summary>
        public bool IsAbilityAvailable(string abilityId)
        {
            // Verifica cooldown global
            if (_globalCooldownTimer > 0)
                return false;

            // Procura a habilidade
            ElementalAbility ability = _abilities.Find(a => a.AbilityId == abilityId);

            if (ability == null)
                return false;

            // Verifica cooldown específico
            if (_cooldowns.ContainsKey(abilityId) && _cooldowns[abilityId] > 0)
                return false;

            // Verifica energia disponível
            if (_energyManager != null)
            {
                if (_energyManager.GetElementalEnergy(ability.ElementType) < ability.EnergyCost)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retorna o tempo restante de cooldown para uma habilidade
        /// </summary>
        public float GetRemainingCooldown(string abilityId)
        {
            if (_cooldowns.ContainsKey(abilityId))
                return _cooldowns[abilityId];

            return 0f;
        }

        /// <summary>
        /// Adquire uma nova habilidade
        /// </summary>
        public void AddAbility(ElementalAbility ability)
        {
            if (ability == null)
                return;

            // Verifica se já possui
            if (_abilities.Exists(a => a.AbilityId == ability.AbilityId))
                return;

            // Adiciona a habilidade
            _abilities.Add(ability);

            // Reorganiza
            OrganizeAbilities();
        }
    }

    /// <summary>
    /// Define uma habilidade elemental
    /// </summary>
    [System.Serializable]
    public class ElementalAbility
    {
        [Header("Identificação")]
        [SerializeField] private string _abilityId;
        [SerializeField] private string _abilityName;
        [SerializeField] private ElementalType _elementType;
        [SerializeField] private Sprite _abilityIcon;

        [Header("Gameplay")]
        [SerializeField] private int _energyCost = 10;
        [SerializeField] private float _cooldown = 3.0f;
        [SerializeField] private int _unlockStage = 1; // 0=Baby, 1=Young, 2=Adult, 3=Elder

        [Header("Efeitos")]
        [SerializeField] private GameObject _abilityEffectPrefab;
        [SerializeField] private AudioClip _activationSound;

        // Propriedades
        public string AbilityId => _abilityId;
        public string AbilityName => _abilityName;
        public ElementalType ElementType => _elementType;
        public Sprite AbilityIcon => _abilityIcon;
        public int EnergyCost => _energyCost;
        public float Cooldown => _cooldown;
        public int UnlockStage => _unlockStage;

        /// <summary>
        /// Ativa a habilidade na posição especificada
        /// </summary>
        public virtual void Activate(Vector3 position)
        {
            // Cria efeito visual
            if (_abilityEffectPrefab != null)
            {
                GameObject effect = GameObject.Instantiate(_abilityEffectPrefab, position, Quaternion.identity);
                GameObject.Destroy(effect, 2.0f);
            }

            // Reproduz som
            if (_activationSound != null)
            {
                AudioSource.PlayClipAtPoint(_activationSound, position, 0.7f);
            }
        }
    }
}
