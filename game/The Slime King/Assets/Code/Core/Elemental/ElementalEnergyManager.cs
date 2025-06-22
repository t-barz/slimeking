using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TheSlimeKing.Core.Elemental
{
    /// <summary>
    /// Gerencia o armazenamento e uso de energia elemental
    /// </summary>
    public class ElementalEnergyManager : MonoBehaviour
    {
        public static ElementalEnergyManager Instance { get; private set; }

        [Header("Configurações de Energia")]
        [SerializeField] private int _maxEnergyPerElement = 1000;
        [SerializeField] private float _passiveEffectInterval = 10.0f;

        [Header("Feedback")]
        [SerializeField] private bool _showDebugMessages = false;
        [SerializeField] private GameObject _energyGainEffectPrefab;
        [SerializeField] private AudioClip _energyGainSound;
        [SerializeField] private float _gainEffectThreshold = 5;

        [Header("UI Feedback")]
        [SerializeField] private float _barPulseAmount = 0.2f;
        [SerializeField] private float _barPulseDuration = 0.5f;

        // Eventos
        [System.Serializable]
        public class ElementalEnergyEvent : UnityEvent<ElementalType, int> { }

        public ElementalEnergyEvent OnEnergyAdded;
        public ElementalEnergyEvent OnEnergyUsed;
        public UnityEvent OnEnergyUpdated;
        public UnityEvent OnTotalEnergyThresholdReached;

        // Mapa de energia elemental por tipo
        private Dictionary<ElementalType, int> _elementalEnergy = new Dictionary<ElementalType, int>();

        // Histórico de absorções de energia
        private Dictionary<ElementalType, int> _totalAbsorbedEnergy = new Dictionary<ElementalType, int>();

        // Rastreamento para desbloqueio de crescimento
        private int _lastGrowthThresholdReached = 0;

        // Thresholds de crescimento (conforme a documentação)
        private readonly int[] _growthThresholds = { 0, 200, 600, 1200 };

        // Benefícios passivos por tipo
        private Dictionary<ElementalType, Action> _passiveEffects = new Dictionary<ElementalType, Action>();

        private float _passiveEffectTimer = 0f;

        private void Awake()
        {
            // Implementa padrão Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializa eventos
            if (OnEnergyAdded == null)
                OnEnergyAdded = new ElementalEnergyEvent();

            if (OnEnergyUsed == null)
                OnEnergyUsed = new ElementalEnergyEvent();

            if (OnEnergyUpdated == null)
                OnEnergyUpdated = new UnityEvent();

            if (OnTotalEnergyThresholdReached == null)
                OnTotalEnergyThresholdReached = new UnityEvent();

            // Inicializa dicionários
            InitializeDictionaries();

            // Configura efeitos passivos
            SetupPassiveEffects();
        }

        private void Start()
        {
            // Registra eventos
            ElementalEvents.OnFragmentAbsorbed += HandleFragmentAbsorbed;
        }

        private void OnDestroy()
        {
            // Remove listeners de eventos
            ElementalEvents.OnFragmentAbsorbed -= HandleFragmentAbsorbed;
        }

        private void Update()
        {
            // Processa efeitos passivos por intervalo
            _passiveEffectTimer += Time.deltaTime;

            if (_passiveEffectTimer >= _passiveEffectInterval)
            {
                ProcessPassiveEffects();
                _passiveEffectTimer = 0f;
            }
        }

        /// <summary>
        /// Inicializa os dicionários com valores padrão
        /// </summary>
        private void InitializeDictionaries()
        {
            // Configura energia inicial para cada tipo
            _elementalEnergy[ElementalType.Earth] = 0;
            _elementalEnergy[ElementalType.Water] = 0;
            _elementalEnergy[ElementalType.Fire] = 0;
            _elementalEnergy[ElementalType.Air] = 0;

            // Configura contadores de total absorvido
            _totalAbsorbedEnergy[ElementalType.Earth] = 0;
            _totalAbsorbedEnergy[ElementalType.Water] = 0;
            _totalAbsorbedEnergy[ElementalType.Fire] = 0;
            _totalAbsorbedEnergy[ElementalType.Air] = 0;
        }

        /// <summary>
        /// Configura efeitos passivos para cada elemento
        /// </summary>
        private void SetupPassiveEffects()
        {
            // Terra: +1 Defense a cada 10 pontos
            _passiveEffects[ElementalType.Earth] = () =>
            {
                int defenseBuff = _elementalEnergy[ElementalType.Earth] / 10;
                // Aplicar buff no sistema de status
                // Isso será implementado posteriormente quando o sistema de status estiver pronto
                // StatusManager.Instance?.SetElementalBuff("Defense", defenseBuff);

                if (_showDebugMessages)
                    Debug.Log($"Passive Earth Effect: +{defenseBuff} Defense");
            };

            // Água: +1 Regeneração a cada 10 pontos
            _passiveEffects[ElementalType.Water] = () =>
            {
                int regenBuff = _elementalEnergy[ElementalType.Water] / 10;
                // Aplicar buff de regeneração
                // StatusManager.Instance?.SetElementalBuff("Regeneration", regenBuff);

                if (_showDebugMessages)
                    Debug.Log($"Passive Water Effect: +{regenBuff} Regeneration");
            };

            // Fogo: +1 Attack a cada 10 pontos
            _passiveEffects[ElementalType.Fire] = () =>
            {
                int attackBuff = _elementalEnergy[ElementalType.Fire] / 10;
                // Aplicar buff no sistema de status
                // StatusManager.Instance?.SetElementalBuff("Attack", attackBuff);

                if (_showDebugMessages)
                    Debug.Log($"Passive Fire Effect: +{attackBuff} Attack");
            };

            // Ar: +1 Speed a cada 10 pontos
            _passiveEffects[ElementalType.Air] = () =>
            {
                int speedBuff = _elementalEnergy[ElementalType.Air] / 10;
                // Aplicar buff no sistema de status
                // StatusManager.Instance?.SetElementalBuff("Speed", speedBuff);

                if (_showDebugMessages)
                    Debug.Log($"Passive Air Effect: +{speedBuff} Speed");
            };
        }

        /// <summary>
        /// Processa os efeitos passivos de todos os elementos
        /// </summary>
        private void ProcessPassiveEffects()
        {
            foreach (var element in System.Enum.GetValues(typeof(ElementalType)))
            {
                ElementalType elementType = (ElementalType)element;
                if (elementType != ElementalType.None && _passiveEffects.ContainsKey(elementType))
                {
                    _passiveEffects[elementType]?.Invoke();
                }
            }
        }

        /// <summary>
        /// Trata o evento de absorção de um fragmento elemental
        /// </summary>
        private void HandleFragmentAbsorbed(ElementalType elementType, int amount, Vector3 position)
        {
            // Adiciona a energia recebida
            AddElementalEnergy(elementType, amount);

            // Efeitos visuais para ganhos significativos
            if (amount >= _gainEffectThreshold && _energyGainEffectPrefab != null)
            {
                GameObject effect = Instantiate(_energyGainEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 2.0f);
            }

            // Som para ganho de energia
            if (amount >= _gainEffectThreshold && _energyGainSound != null)
            {
                AudioSource.PlayClipAtPoint(_energyGainSound, position, 0.7f);
            }
        }

        /// <summary>
        /// Adiciona energia elemental do tipo especificado
        /// </summary>
        public void AddElementalEnergy(ElementalType elementType, int amount)
        {
            if (elementType == ElementalType.None || amount <= 0)
                return;

            // Limita ao máximo definido
            int currentEnergy = _elementalEnergy[elementType];
            int newEnergy = Mathf.Clamp(currentEnergy + amount, 0, _maxEnergyPerElement);

            // Atualiza energia atual
            _elementalEnergy[elementType] = newEnergy;

            // Atualiza contagem total (para progressão)
            _totalAbsorbedEnergy[elementType] += amount;

            // Dispara evento
            OnEnergyAdded?.Invoke(elementType, amount);
            OnEnergyUpdated?.Invoke();

            // Debug
            if (_showDebugMessages)
                Debug.Log($"Added {amount} {elementType} energy. New value: {newEnergy}");

            // Verifica se atingiu threshold de evolução
            CheckGrowthThreshold();
        }

        /// <summary>
        /// Consome energia elemental do tipo especificado
        /// </summary>
        public bool UseElementalEnergy(ElementalType elementType, int amount)
        {
            if (elementType == ElementalType.None || amount <= 0)
                return false;

            // Verifica se há energia suficiente
            int currentEnergy = _elementalEnergy[elementType];
            if (currentEnergy < amount)
                return false;

            // Reduz energia
            _elementalEnergy[elementType] = currentEnergy - amount;

            // Dispara evento
            OnEnergyUsed?.Invoke(elementType, amount);
            OnEnergyUpdated?.Invoke();

            // Debug
            if (_showDebugMessages)
                Debug.Log($"Used {amount} {elementType} energy. Remaining: {_elementalEnergy[elementType]}");

            return true;
        }

        /// <summary>
        /// Verifica se a energia total atingiu algum threshold de crescimento
        /// </summary>
        private void CheckGrowthThreshold()
        {
            int totalEnergy = GetTotalAbsorbedEnergy();
            int currentStage = GetCurrentGrowthStage();

            // Verifica se atingiu novo threshold
            if (currentStage > _lastGrowthThresholdReached)
            {
                _lastGrowthThresholdReached = currentStage;

                // Dispara evento de threshold atingido
                OnTotalEnergyThresholdReached?.Invoke();

                // Notifica via evento global
                ElementalEvents.OnElementalThresholdReached?.Invoke(currentStage);

                if (_showDebugMessages)
                    Debug.Log($"Growth threshold reached! Stage {currentStage}: {totalEnergy} total energy");
            }
        }

        /// <summary>
        /// Retorna o valor atual de energia para o tipo especificado
        /// </summary>
        public int GetElementalEnergy(ElementalType elementType)
        {
            if (elementType == ElementalType.None)
                return 0;

            return _elementalEnergy[elementType];
        }

        /// <summary>
        /// Retorna o percentual de energia para o tipo especificado (0-1)
        /// </summary>
        public float GetElementalEnergyPercentage(ElementalType elementType)
        {
            if (elementType == ElementalType.None)
                return 0f;

            return (float)_elementalEnergy[elementType] / _maxEnergyPerElement;
        }

        /// <summary>
        /// Retorna o total de energia absorvida desde o início do jogo
        /// </summary>
        public int GetTotalAbsorbedEnergy()
        {
            int total = 0;
            foreach (var kvp in _totalAbsorbedEnergy)
            {
                total += kvp.Value;
            }
            return total;
        }

        /// <summary>
        /// Retorna o total de energia absorvida para um elemento específico
        /// </summary>
        public int GetTotalAbsorbedEnergy(ElementalType elementType)
        {
            if (elementType == ElementalType.None)
                return 0;

            return _totalAbsorbedEnergy[elementType];
        }

        /// <summary>
        /// Retorna o estágio de crescimento atual baseado na energia total
        /// </summary>
        public int GetCurrentGrowthStage()
        {
            int totalEnergy = GetTotalAbsorbedEnergy();

            // Encontra o estágio correspondente à energia atual
            for (int i = _growthThresholds.Length - 1; i >= 0; i--)
            {
                if (totalEnergy >= _growthThresholds[i])
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// Retorna a energia necessária para o próximo estágio de crescimento
        /// </summary>
        public int GetEnergyForNextStage()
        {
            int currentStage = GetCurrentGrowthStage();

            // Se já está no estágio final
            if (currentStage >= _growthThresholds.Length - 1)
                return 0;

            int nextThreshold = _growthThresholds[currentStage + 1];
            int currentEnergy = GetTotalAbsorbedEnergy();

            return nextThreshold - currentEnergy;
        }

        /// <summary>
        /// Retorna o percentual de progresso para o próximo estágio (0-1)
        /// </summary>
        public float GetProgressToNextStage()
        {
            int currentStage = GetCurrentGrowthStage();

            // Se já está no estágio final
            if (currentStage >= _growthThresholds.Length - 1)
                return 1f;

            int currentThreshold = _growthThresholds[currentStage];
            int nextThreshold = _growthThresholds[currentStage + 1];
            int currentEnergy = GetTotalAbsorbedEnergy();

            float progress = (float)(currentEnergy - currentThreshold) / (nextThreshold - currentThreshold);
            return Mathf.Clamp01(progress);
        }

        /// <summary>
        /// Salva os dados de energia elemental
        /// </summary>
        public Dictionary<string, object> SaveData()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            // Salva valores atuais de energia
            data["earth_energy"] = _elementalEnergy[ElementalType.Earth];
            data["water_energy"] = _elementalEnergy[ElementalType.Water];
            data["fire_energy"] = _elementalEnergy[ElementalType.Fire];
            data["air_energy"] = _elementalEnergy[ElementalType.Air];

            // Salva totais absorvidos (para progressão)
            data["total_earth"] = _totalAbsorbedEnergy[ElementalType.Earth];
            data["total_water"] = _totalAbsorbedEnergy[ElementalType.Water];
            data["total_fire"] = _totalAbsorbedEnergy[ElementalType.Fire];
            data["total_air"] = _totalAbsorbedEnergy[ElementalType.Air];

            // Salva último threshold atingido
            data["last_threshold"] = _lastGrowthThresholdReached;

            return data;
        }

        /// <summary>
        /// Carrega dados de energia elemental
        /// </summary>
        public void LoadData(Dictionary<string, object> data)
        {
            if (data == null)
                return;

            // Carrega valores de energia
            if (data.ContainsKey("earth_energy"))
                _elementalEnergy[ElementalType.Earth] = Convert.ToInt32(data["earth_energy"]);

            if (data.ContainsKey("water_energy"))
                _elementalEnergy[ElementalType.Water] = Convert.ToInt32(data["water_energy"]);

            if (data.ContainsKey("fire_energy"))
                _elementalEnergy[ElementalType.Fire] = Convert.ToInt32(data["fire_energy"]);

            if (data.ContainsKey("air_energy"))
                _elementalEnergy[ElementalType.Air] = Convert.ToInt32(data["air_energy"]);

            // Carrega totais absorvidos
            if (data.ContainsKey("total_earth"))
                _totalAbsorbedEnergy[ElementalType.Earth] = Convert.ToInt32(data["total_earth"]);

            if (data.ContainsKey("total_water"))
                _totalAbsorbedEnergy[ElementalType.Water] = Convert.ToInt32(data["total_water"]);

            if (data.ContainsKey("total_fire"))
                _totalAbsorbedEnergy[ElementalType.Fire] = Convert.ToInt32(data["total_fire"]);

            if (data.ContainsKey("total_air"))
                _totalAbsorbedEnergy[ElementalType.Air] = Convert.ToInt32(data["total_air"]);

            // Carrega último threshold
            if (data.ContainsKey("last_threshold"))
                _lastGrowthThresholdReached = Convert.ToInt32(data["last_threshold"]);

            // Notifica UI e outros sistemas da atualização
            OnEnergyUpdated?.Invoke();
        }
    }
}
