using UnityEngine;
using System;
using System.Collections.Generic;

namespace SlimeMec.Alpha.Progression
{
    /// <summary>
    /// Sistema de crescimento/evolução do Slime para Demo Alpha
    /// Gerencia estágios de evolução, atributos e transformações visuais
    /// </summary>
    public class GrowthSystem : MonoBehaviour
    {
        #region Singleton
        public static GrowthSystem Instance { get; private set; }
        #endregion

        #region Events
        public static event Action<GrowthStage, GrowthStage> OnStageChanged; // from, to
        public static event Action<float> OnExperienceGained;
        public static event Action<GrowthStage> OnEvolutionAvailable;
        public static event Action OnAttributesUpdated;
        #endregion

        #region Serialized Fields
        [Header("Growth Configuration")]
        [SerializeField] private GrowthStage startingStage = GrowthStage.Tiny;
        [SerializeField] private bool autoEvolve = false;
        [SerializeField] private float baseExperienceRequired = 100f;
        [SerializeField] private float experienceMultiplier = 1.5f;

        [Header("Visual Integration")]
        [SerializeField] private bool updatePlayerVisuals = true;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;

        [Header("Attribute Modifiers")]
        [SerializeField] private List<StageAttributeModifier> stageModifiers = new List<StageAttributeModifier>();

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showGizmos = true;
        #endregion

        #region Private Fields
        private GrowthStage currentStage;
        private float currentExperience = 0f;
        private float experienceToNextStage;
        private Dictionary<GrowthStage, StageData> stageDatabase;
        private bool isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSingleton();
            InitializeStageDatabase();
        }

        private void Start()
        {
            InitializeGrowthSystem();
            SetupIntegrations();
        }

        private void OnDestroy()
        {
            CleanupEvents();
        }
        #endregion

        #region Initialization
        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeStageDatabase()
        {
            stageDatabase = new Dictionary<GrowthStage, StageData>();

            // Define dados para cada estágio
            stageDatabase[GrowthStage.Tiny] = new StageData
            {
                stage = GrowthStage.Tiny,
                displayName = "Tiny Slime",
                description = "A small, curious slime just starting its journey",
                scaleMultiplier = 0.7f,
                healthMultiplier = 0.8f,
                attackMultiplier = 0.9f,
                speedMultiplier = 1.1f,
                experienceRequired = 0f
            };

            stageDatabase[GrowthStage.Small] = new StageData
            {
                stage = GrowthStage.Small,
                displayName = "Small Slime",
                description = "Growing stronger and more confident",
                scaleMultiplier = 1.0f,
                healthMultiplier = 1.0f,
                attackMultiplier = 1.0f,
                speedMultiplier = 1.0f,
                experienceRequired = baseExperienceRequired
            };

            stageDatabase[GrowthStage.Medium] = new StageData
            {
                stage = GrowthStage.Medium,
                displayName = "Medium Slime",
                description = "A balanced slime with moderate abilities",
                scaleMultiplier = 1.3f,
                healthMultiplier = 1.3f,
                attackMultiplier = 1.2f,
                speedMultiplier = 0.95f,
                experienceRequired = baseExperienceRequired * experienceMultiplier
            };

            stageDatabase[GrowthStage.Large] = new StageData
            {
                stage = GrowthStage.Large,
                displayName = "Large Slime",
                description = "A powerful slime with impressive strength",
                scaleMultiplier = 1.6f,
                healthMultiplier = 1.6f,
                attackMultiplier = 1.5f,
                speedMultiplier = 0.9f,
                experienceRequired = baseExperienceRequired * experienceMultiplier * experienceMultiplier
            };

            stageDatabase[GrowthStage.Giant] = new StageData
            {
                stage = GrowthStage.Giant,
                displayName = "Giant Slime",
                description = "The ultimate form of slime evolution",
                scaleMultiplier = 2.0f,
                healthMultiplier = 2.0f,
                attackMultiplier = 1.8f,
                speedMultiplier = 0.8f,
                experienceRequired = baseExperienceRequired * Mathf.Pow(experienceMultiplier, 3)
            };
        }

        private void InitializeGrowthSystem()
        {
            currentStage = startingStage;
            currentExperience = 0f;
            experienceToNextStage = GetExperienceRequiredForStage(GetNextStage());

            ApplyStageEffects(currentStage);
            isInitialized = true;

            if (enableDebugLogs)
                Debug.Log($"[GrowthSystem] Initialized at stage: {currentStage}");
        }

        private void SetupIntegrations()
        {
            FindPlayerReferences();
            SetupAttributeSystemIntegration();
        }

        private void FindPlayerReferences()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                    playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
                }
            }
        }

        private void SetupAttributeSystemIntegration()
        {
            // TODO: Integrar com PlayerAttributesSystem quando disponível
            // Exemplo: PlayerAttributesSystem.OnAttributesChanged += HandleAttributeChanges;
        }

        private void CleanupEvents()
        {
            OnStageChanged = null;
            OnExperienceGained = null;
            OnEvolutionAvailable = null;
            OnAttributesUpdated = null;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Adiciona experiência ao sistema
        /// </summary>
        public void GainExperience(float amount)
        {
            if (!isInitialized) return;

            currentExperience += amount;
            OnExperienceGained?.Invoke(amount);

            if (enableDebugLogs)
                Debug.Log($"[GrowthSystem] Gained {amount} XP. Total: {currentExperience}");

            CheckForEvolution();
        }

        /// <summary>
        /// Força evolução para próximo estágio (se possível)
        /// </summary>
        public bool EvolveToNextStage()
        {
            GrowthStage nextStage = GetNextStage();
            if (nextStage == currentStage)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[GrowthSystem] Already at maximum stage!");
                return false;
            }

            if (!autoEvolve && currentExperience < experienceToNextStage)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[GrowthSystem] Not enough experience! Need {experienceToNextStage - currentExperience} more XP");
                return false;
            }

            return ChangeStage(nextStage);
        }

        /// <summary>
        /// Força mudança para um estágio específico
        /// </summary>
        public bool ChangeStage(GrowthStage targetStage)
        {
            if (targetStage == currentStage) return true;

            GrowthStage previousStage = currentStage;
            currentStage = targetStage;

            // Atualiza experiência necessária para próximo estágio
            GrowthStage nextStage = GetNextStage();
            experienceToNextStage = GetExperienceRequiredForStage(nextStage);

            // Aplica efeitos do novo estágio
            ApplyStageEffects(currentStage);

            // Notifica mudança
            OnStageChanged?.Invoke(previousStage, currentStage);
            OnAttributesUpdated?.Invoke();

            if (enableDebugLogs)
                Debug.Log($"[GrowthSystem] Stage changed: {previousStage} -> {currentStage}");

            return true;
        }

        /// <summary>
        /// Obtém estágio atual
        /// </summary>
        public GrowthStage GetCurrentStage() => currentStage;

        /// <summary>
        /// Obtém experiência atual
        /// </summary>
        public float GetCurrentExperience() => currentExperience;

        /// <summary>
        /// Obtém experiência necessária para próximo estágio
        /// </summary>
        public float GetExperienceToNextStage() => experienceToNextStage;

        /// <summary>
        /// Obtém progresso percentual para próximo estágio
        /// </summary>
        public float GetEvolutionProgress()
        {
            if (GetNextStage() == currentStage) return 1f; // Máximo alcançado

            float currentStageExp = GetExperienceRequiredForStage(currentStage);
            float nextStageExp = GetExperienceRequiredForStage(GetNextStage());
            float totalNeeded = nextStageExp - currentStageExp;
            float currentProgress = currentExperience - currentStageExp;

            return Mathf.Clamp01(currentProgress / totalNeeded);
        }

        /// <summary>
        /// Verifica se evolução está disponível
        /// </summary>
        public bool CanEvolve()
        {
            return GetNextStage() != currentStage && currentExperience >= experienceToNextStage;
        }

        /// <summary>
        /// Obtém dados do estágio atual
        /// </summary>
        public StageData GetCurrentStageData()
        {
            return stageDatabase.ContainsKey(currentStage) ? stageDatabase[currentStage] : default;
        }

        /// <summary>
        /// Obtém multiplicadores de atributos atuais
        /// </summary>
        public AttributeMultipliers GetCurrentMultipliers()
        {
            var stageData = GetCurrentStageData();
            return new AttributeMultipliers
            {
                health = stageData.healthMultiplier,
                attack = stageData.attackMultiplier,
                speed = stageData.speedMultiplier,
                scale = stageData.scaleMultiplier
            };
        }
        #endregion

        #region Helper Methods
        private GrowthStage GetNextStage()
        {
            switch (currentStage)
            {
                case GrowthStage.Tiny: return GrowthStage.Small;
                case GrowthStage.Small: return GrowthStage.Medium;
                case GrowthStage.Medium: return GrowthStage.Large;
                case GrowthStage.Large: return GrowthStage.Giant;
                case GrowthStage.Giant: return GrowthStage.Giant; // Máximo
                default: return GrowthStage.Small;
            }
        }

        private float GetExperienceRequiredForStage(GrowthStage stage)
        {
            return stageDatabase.ContainsKey(stage) ? stageDatabase[stage].experienceRequired : 0f;
        }

        private void CheckForEvolution()
        {
            if (CanEvolve())
            {
                OnEvolutionAvailable?.Invoke(GetNextStage());

                if (autoEvolve)
                {
                    EvolveToNextStage();
                }
            }
        }

        private void ApplyStageEffects(GrowthStage stage)
        {
            var stageData = GetCurrentStageData();

            // Aplica efeitos visuais
            if (updatePlayerVisuals)
            {
                ApplyVisualEffects(stageData);
            }

            // Aplica modificadores de atributos
            ApplyAttributeModifiers(stageData);

            if (enableDebugLogs)
                Debug.Log($"[GrowthSystem] Applied effects for stage: {stage}");
        }

        private void ApplyVisualEffects(StageData stageData)
        {
            if (playerTransform != null)
            {
                Vector3 newScale = Vector3.one * stageData.scaleMultiplier;
                playerTransform.localScale = newScale;
            }

            // TODO: Adicionar outros efeitos visuais (cor, partículas, etc.)
        }

        private void ApplyAttributeModifiers(StageData stageData)
        {
            // TODO: Integrar com PlayerAttributesSystem
            // Exemplo:
            // var attributes = PlayerAttributesSystem.Instance;
            // attributes.SetHealthMultiplier(stageData.healthMultiplier);
            // attributes.SetAttackMultiplier(stageData.attackMultiplier);
            // attributes.SetSpeedMultiplier(stageData.speedMultiplier);

            if (enableDebugLogs)
            {
                Debug.Log($"[GrowthSystem] Attribute modifiers applied: " +
                         $"Health x{stageData.healthMultiplier}, " +
                         $"Attack x{stageData.attackMultiplier}, " +
                         $"Speed x{stageData.speedMultiplier}");
            }
        }
        #endregion

        #region Debug & Editor
        [ContextMenu("Debug - Gain 50 XP")]
        private void DebugGainExperience()
        {
            GainExperience(50f);
        }

        [ContextMenu("Debug - Evolve Next Stage")]
        private void DebugEvolveNext()
        {
            EvolveToNextStage();
        }

        [ContextMenu("Debug - Reset to Tiny")]
        private void DebugResetToTiny()
        {
            currentExperience = 0f;
            ChangeStage(GrowthStage.Tiny);
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos || !enabled) return;

            if (Application.isPlaying && isInitialized)
            {
                // Desenha barra de progresso de evolução
                Vector3 pos = transform.position + Vector3.up * 2f;
                float progress = GetEvolutionProgress();

                Gizmos.color = Color.blue;
                Gizmos.DrawLine(pos - Vector3.right, pos + Vector3.right);

                Gizmos.color = Color.cyan;
                Vector3 progressEnd = pos - Vector3.right + Vector3.right * 2f * progress;
                Gizmos.DrawLine(pos - Vector3.right, progressEnd);

                // Label com informações
#if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    pos + Vector3.up * 0.3f,
                    $"Stage: {currentStage}\nXP: {currentExperience:F0}/{experienceToNextStage:F0}\nProgress: {progress:P0}"
                );
#endif
            }
        }
        #endregion
    }

    /// <summary>
    /// Estágios de crescimento do Slime
    /// </summary>
    public enum GrowthStage
    {
        Tiny,
        Small,
        Medium,
        Large,
        Giant
    }

    /// <summary>
    /// Dados de um estágio de crescimento
    /// </summary>
    [System.Serializable]
    public struct StageData
    {
        public GrowthStage stage;
        public string displayName;
        public string description;
        public float scaleMultiplier;
        public float healthMultiplier;
        public float attackMultiplier;
        public float speedMultiplier;
        public float experienceRequired;
    }

    /// <summary>
    /// Modificador de atributos por estágio
    /// </summary>
    [System.Serializable]
    public struct StageAttributeModifier
    {
        public GrowthStage stage;
        public float healthModifier;
        public float attackModifier;
        public float speedModifier;
        public float scaleModifier;
    }

    /// <summary>
    /// Multiplicadores de atributos
    /// </summary>
    [System.Serializable]
    public struct AttributeMultipliers
    {
        public float health;
        public float attack;
        public float speed;
        public float scale;
    }
}