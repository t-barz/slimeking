using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Gameplay
{
    using Core;

    /// <summary>
    /// Gerencia os atributos e características específicas do Slime controlado pelo jogador
    /// </summary>
    public class SlimeStats : BaseCharacterStats
    {
        [Header("Configuração dos Estágios")]
        [SerializeField] private SlimeGrowthStage[] growthStages;
        [SerializeField] private SlimeStage currentStage = SlimeStage.Baby;
        [SerializeField] private int currentStageIndex = 0;

        [Header("Energia Elemental")]
        [SerializeField] private int earthEnergy;
        [SerializeField] private int waterEnergy;
        [SerializeField] private int fireEnergy;
        [SerializeField] private int airEnergy;
        [SerializeField] private int totalElementalEnergy;
        [SerializeField] private int energyRequiredForNextStage;

        // Referências de componentes
        private Transform slimeTransform;
        private SpriteRenderer spriteRenderer;
        private Animator animator;

        // Inventário Evolutivo
        private int maxInventorySlots = 1; // Começa com 1 slot (Baby)

        // Eventos
        public event Action<SlimeStage> OnStageChanged;
        public event Action<ElementType, float> OnElementalEnergyChanged;
        public event Action<int> OnInventorySlotsChanged;
        public event Action<float, float> OnElementalEnergyProgress; // (current, required)

        // Propriedades públicas
        public SlimeStage CurrentStage => currentStage;
        public int CurrentStageIndex => currentStageIndex;
        public SlimeGrowthStage CurrentStageConfig => growthStages[currentStageIndex];
        public float EarthEnergy => earthEnergy;
        public float WaterEnergy => waterEnergy;
        public float FireEnergy => fireEnergy;
        public float AirEnergy => airEnergy;
        public float TotalElementalEnergy => totalElementalEnergy;
        public float EnergyRequiredForNextStage => energyRequiredForNextStage;
        public int MaxInventorySlots => maxInventorySlots;

        // Propriedades de habilidades disponíveis
        public bool CanSqueeze => CurrentStageConfig.CanSqueeze;
        public bool CanBounce => CurrentStageConfig.CanBounce;
        public bool CanClimb => CurrentStageConfig.CanClimb;
        public bool CanUsePowerfulAbilities => CurrentStageConfig.CanUsePowerfulAbilities;

        protected override void Awake()
        {
            base.Awake();

            // Obtém referências de componentes
            slimeTransform = transform;
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // Configura o estágio inicial
            InitializeStage();
        }

        protected override void Update()
        {
            base.Update();

            // Verifica condições para evolução
            CheckForGrowthProgression();
        }

        /// <summary>
        /// Inicializa as configurações com base no estágio atual
        /// </summary>
        private void InitializeStage()
        {
            if (growthStages == null || growthStages.Length == 0)
            {
                Debug.LogError("SlimeStats: Nenhum estágio de crescimento configurado!");
                return;
            }

            // Encontra o index do estágio atual
            for (int i = 0; i < growthStages.Length; i++)
            {
                if (growthStages[i].StageType == currentStage)
                {
                    currentStageIndex = i;
                    break;
                }
            }

            // Aplica as configurações do estágio atual
            ApplyStageConfiguration(false);
        }

        /// <summary>
        /// Aplica a configuração do estágio atual ao Slime
        /// </summary>
        /// <param name="showEffects">Se deve mostrar efeitos de transição</param>
        private void ApplyStageConfiguration(bool showEffects = true)
        {
            SlimeGrowthStage config = CurrentStageConfig;

            // Configura atributos base
            maxHealth.BaseValue = config.BaseHealth;
            attack.BaseValue = config.BaseAttack;
            special.BaseValue = config.BaseSpecial;
            defense.BaseValue = config.BaseDefense;

            // Aplica escala
            slimeTransform.localScale = Vector3.one * config.SizeMultiplier;

            // Atualiza visual
            if (spriteRenderer != null && config.SlimeSprite != null)
            {
                spriteRenderer.sprite = config.SlimeSprite;
            }

            // Atualiza animações
            if (animator != null && config.AnimatorController != null)
            {
                animator.runtimeAnimatorController = config.AnimatorController;
            }

            // Atualiza slots de inventário
            int newSlots = config.InventorySlots;
            if (newSlots != maxInventorySlots)
            {
                maxInventorySlots = newSlots;
                OnInventorySlotsChanged?.Invoke(maxInventorySlots);
            }

            // Define a energia necessária para o próximo estágio
            if (currentStageIndex < growthStages.Length - 1)
            {
                energyRequiredForNextStage = growthStages[currentStageIndex + 1].RequiredElementalEnergy;
            }

            // Reproduz efeitos de crescimento
            if (showEffects)
            {
                // Visual
                if (config.GrowthEffect != null)
                {
                    Instantiate(config.GrowthEffect, transform.position, Quaternion.identity);
                }

                // Áudio
                if (config.GrowthSound != null)
                {
                    AudioSource.PlayClipAtPoint(config.GrowthSound, transform.position);
                }
            }
        }

        /// <summary>
        /// Verifica se o Slime pode evoluir para o próximo estágio
        /// </summary>
        private void CheckForGrowthProgression()
        {
            // Se já está no último estágio, não faz nada
            if (currentStageIndex >= growthStages.Length - 1)
                return;

            // Verifica se possui energia suficiente para evoluir
            if (totalElementalEnergy >= energyRequiredForNextStage)
            {
                EvolveTotNextStage();
            }
            else
            {
                // Notifica o progresso atual
                OnElementalEnergyProgress?.Invoke(totalElementalEnergy, energyRequiredForNextStage);
            }
        }

        /// <summary>
        /// Evolui para o próximo estágio de crescimento
        /// </summary>
        public void EvolveTotNextStage()
        {
            // Garante que não está no último estágio
            if (currentStageIndex >= growthStages.Length - 1)
                return;

            // Evolui para o próximo estágio
            currentStageIndex++;
            currentStage = growthStages[currentStageIndex].StageType;

            // Aplica nova configuração
            ApplyStageConfiguration(true);

            // Notifica mudança
            OnStageChanged?.Invoke(currentStage);
        }

        /// <summary>
        /// Adiciona energia elemental de um tipo específico
        /// </summary>
        /// <param name="type">Tipo da energia elemental</param>
        /// <param name="amount">Quantidade de energia</param>
        public void AddElementalEnergy(ElementType type, int amount)
        {
            int previousTotal = totalElementalEnergy;

            // Adiciona ao tipo específico
            switch (type)
            {
                case ElementType.Earth:
                    earthEnergy += amount;
                    break;
                case ElementType.Water:
                    waterEnergy += amount;
                    break;
                case ElementType.Fire:
                    fireEnergy += amount;
                    break;
                case ElementType.Air:
                    airEnergy += amount;
                    break;
                default:
                    return; // Não faz nada para tipo None
            }

            // Atualiza o total
            totalElementalEnergy = earthEnergy + waterEnergy + fireEnergy + airEnergy;

            // Notifica mudanças
            OnElementalEnergyChanged?.Invoke(type, amount);

            if (previousTotal != totalElementalEnergy)
            {
                OnElementalEnergyProgress?.Invoke(totalElementalEnergy, energyRequiredForNextStage);
            }
        }

        /// <summary>
        /// Obtém a quantidade de energia de um elemento específico
        /// </summary>
        /// <param name="type">Tipo do elemento</param>
        /// <returns>Quantidade de energia</returns>
        public int GetElementalEnergy(ElementType type)
        {
            switch (type)
            {
                case ElementType.Earth:
                    return earthEnergy;
                case ElementType.Water:
                    return waterEnergy;
                case ElementType.Fire:
                    return fireEnergy;
                case ElementType.Air:
                    return airEnergy;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Consome energia elemental para usar habilidades
        /// </summary>
        /// <param name="type">Tipo do elemento</param>
        /// <param name="amount">Quantidade a consumir</param>
        /// <returns>Se foi possível consumir</returns>
        public bool ConsumeElementalEnergy(ElementType type, int amount)
        {
            int currentAmount = GetElementalEnergy(type);

            // Verifica se tem energia suficiente
            if (currentAmount < amount)
                return false;

            // Consome a energia
            switch (type)
            {
                case ElementType.Earth:
                    earthEnergy -= amount;
                    break;
                case ElementType.Water:
                    waterEnergy -= amount;
                    break;
                case ElementType.Fire:
                    fireEnergy -= amount;
                    break;
                case ElementType.Air:
                    airEnergy -= amount;
                    break;
                default:
                    return false;
            }

            // Atualiza o total
            totalElementalEnergy = earthEnergy + waterEnergy + fireEnergy + airEnergy;

            // Notifica mudanças
            OnElementalEnergyChanged?.Invoke(type, -amount);
            OnElementalEnergyProgress?.Invoke(totalElementalEnergy, energyRequiredForNextStage);

            return true;
        }

        /// <summary>
        /// Redefine o estágio do Slime para desenvolvimento e debug
        /// </summary>
        /// <param name="stage">Estágio desejado</param>
        public void SetStageForDebug(SlimeStage stage)
        {
            for (int i = 0; i < growthStages.Length; i++)
            {
                if (growthStages[i].StageType == stage)
                {
                    currentStageIndex = i;
                    currentStage = stage;
                    ApplyStageConfiguration(false);
                    OnStageChanged?.Invoke(currentStage);
                    return;
                }
            }
        }
    }
}
