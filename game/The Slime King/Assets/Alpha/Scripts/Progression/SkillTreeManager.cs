using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlimeMec.Alpha.Progression
{
    /// <summary>
    /// Gerenciador de árvore de habilidades para Demo Alpha
    /// Sistema básico de nós e unlocks baseado no crescimento do Slime
    /// </summary>
    public class SkillTreeManager : MonoBehaviour
    {
        #region Singleton
        public static SkillTreeManager Instance { get; private set; }
        #endregion

        #region Events
        public static event Action<SkillNode> OnSkillUnlocked;
        public static event Action<SkillNode> OnSkillActivated;
        public static event Action OnSkillTreeUpdated;
        public static event Action<int> OnSkillPointsChanged;
        #endregion

        #region Serialized Fields
        [Header("Skill Tree Configuration")]
        [SerializeField] private int maxSkillPoints = 10;
        [SerializeField] private bool autoUnlockWithGrowth = true;
        [SerializeField] private bool allowSkillPointSpending = true;

        [Header("Integration")]
        [SerializeField] private bool integrateWithGrowthSystem = true;
        [SerializeField] private bool applySkillEffectsImmediately = true;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showGizmos = true;
        #endregion

        #region Private Fields
        private Dictionary<string, SkillNode> skillNodes = new Dictionary<string, SkillNode>();
        private List<SkillNode> unlockedSkills = new List<SkillNode>();
        private List<SkillNode> activeSkills = new List<SkillNode>();
        private int currentSkillPoints = 0;
        private bool isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSingleton();
            InitializeSkillTree();
        }

        private void Start()
        {
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

        private void InitializeSkillTree()
        {
            CreateSkillNodes();
            SetupSkillDependencies();
            
            currentSkillPoints = 0;
            isInitialized = true;

            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Initialized with {skillNodes.Count} skill nodes");
        }

        private void CreateSkillNodes()
        {
            // Tier 1 - Básico (disponível no estágio Small)
            AddSkillNode(new SkillNode
            {
                id = "health_boost_1",
                displayName = "Health Boost I",
                description = "Increases maximum health by 20%",
                tier = 1,
                requiredGrowthStage = GrowthStage.Small,
                skillPointCost = 1,
                effectType = SkillEffectType.HealthBoost,
                effectValue = 0.2f,
                isPassive = true
            });

            AddSkillNode(new SkillNode
            {
                id = "attack_boost_1",
                displayName = "Attack Boost I",
                description = "Increases attack damage by 15%",
                tier = 1,
                requiredGrowthStage = GrowthStage.Small,
                skillPointCost = 1,
                effectType = SkillEffectType.AttackBoost,
                effectValue = 0.15f,
                isPassive = true
            });

            AddSkillNode(new SkillNode
            {
                id = "speed_boost_1",
                displayName = "Speed Boost I",
                description = "Increases movement speed by 10%",
                tier = 1,
                requiredGrowthStage = GrowthStage.Small,
                skillPointCost = 1,
                effectType = SkillEffectType.SpeedBoost,
                effectValue = 0.1f,
                isPassive = true
            });

            // Tier 2 - Intermediário (disponível no estágio Medium)
            AddSkillNode(new SkillNode
            {
                id = "regeneration",
                displayName = "Regeneration",
                description = "Slowly regenerates health over time",
                tier = 2,
                requiredGrowthStage = GrowthStage.Medium,
                skillPointCost = 2,
                effectType = SkillEffectType.Regeneration,
                effectValue = 2f, // HP per second
                isPassive = true,
                dependencies = new List<string> { "health_boost_1" }
            });

            AddSkillNode(new SkillNode
            {
                id = "critical_strike",
                displayName = "Critical Strike",
                description = "10% chance to deal double damage",
                tier = 2,
                requiredGrowthStage = GrowthStage.Medium,
                skillPointCost = 2,
                effectType = SkillEffectType.CriticalChance,
                effectValue = 0.1f,
                isPassive = true,
                dependencies = new List<string> { "attack_boost_1" }
            });

            AddSkillNode(new SkillNode
            {
                id = "dash_ability",
                displayName = "Dash",
                description = "Quick dash ability with short cooldown",
                tier = 2,
                requiredGrowthStage = GrowthStage.Medium,
                skillPointCost = 2,
                effectType = SkillEffectType.DashAbility,
                effectValue = 5f, // Dash distance
                isPassive = false,
                cooldown = 3f,
                dependencies = new List<string> { "speed_boost_1" }
            });

            // Tier 3 - Avançado (disponível no estágio Large)
            AddSkillNode(new SkillNode
            {
                id = "area_attack",
                displayName = "Area Attack",
                description = "Attacks hit multiple enemies in an area",
                tier = 3,
                requiredGrowthStage = GrowthStage.Large,
                skillPointCost = 3,
                effectType = SkillEffectType.AreaAttack,
                effectValue = 2f, // Area radius multiplier
                isPassive = true,
                dependencies = new List<string> { "critical_strike" }
            });

            AddSkillNode(new SkillNode
            {
                id = "slime_shield",
                displayName = "Slime Shield",
                description = "Temporary damage immunity shield",
                tier = 3,
                requiredGrowthStage = GrowthStage.Large,
                skillPointCost = 3,
                effectType = SkillEffectType.Shield,
                effectValue = 5f, // Shield duration
                isPassive = false,
                cooldown = 15f,
                dependencies = new List<string> { "regeneration" }
            });
        }

        private void AddSkillNode(SkillNode node)
        {
            skillNodes[node.id] = node;
        }

        private void SetupSkillDependencies()
        {
            // As dependências já são configuradas na criação dos nós
            // Este método pode ser usado para validações adicionais
            
            foreach (var node in skillNodes.Values)
            {
                if (node.dependencies != null)
                {
                    foreach (var depId in node.dependencies)
                    {
                        if (!skillNodes.ContainsKey(depId))
                        {
                            Debug.LogError($"[SkillTreeManager] Dependency '{depId}' not found for skill '{node.id}'");
                        }
                    }
                }
            }
        }

        private void SetupIntegrations()
        {
            if (integrateWithGrowthSystem && GrowthSystem.Instance != null)
            {
                GrowthSystem.OnStageChanged += HandleGrowthStageChanged;
            }

            if (enableDebugLogs)
                Debug.Log("[SkillTreeManager] Integrations setup completed");
        }

        private void CleanupEvents()
        {
            if (GrowthSystem.Instance != null)
            {
                GrowthSystem.OnStageChanged -= HandleGrowthStageChanged;
            }

            OnSkillUnlocked = null;
            OnSkillActivated = null;
            OnSkillTreeUpdated = null;
            OnSkillPointsChanged = null;
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Adiciona pontos de habilidade
        /// </summary>
        public void AddSkillPoints(int points)
        {
            currentSkillPoints += points;
            OnSkillPointsChanged?.Invoke(currentSkillPoints);

            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Added {points} skill points. Total: {currentSkillPoints}");
        }

        /// <summary>
        /// Tenta desbloquear uma habilidade
        /// </summary>
        public bool UnlockSkill(string skillId)
        {
            if (!skillNodes.ContainsKey(skillId))
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[SkillTreeManager] Skill '{skillId}' not found");
                return false;
            }

            var skill = skillNodes[skillId];
            
            if (IsSkillUnlocked(skillId))
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[SkillTreeManager] Skill '{skillId}' already unlocked");
                return false;
            }

            if (!CanUnlockSkill(skillId))
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[SkillTreeManager] Cannot unlock skill '{skillId}' - requirements not met");
                return false;
            }

            // Consome pontos de habilidade
            if (allowSkillPointSpending)
            {
                currentSkillPoints -= skill.skillPointCost;
                OnSkillPointsChanged?.Invoke(currentSkillPoints);
            }

            // Desbloqueia a habilidade
            unlockedSkills.Add(skill);
            
            // Aplica efeitos se for passiva
            if (skill.isPassive && applySkillEffectsImmediately)
            {
                ActivateSkill(skillId);
            }

            OnSkillUnlocked?.Invoke(skill);
            OnSkillTreeUpdated?.Invoke();

            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Unlocked skill: {skill.displayName}");

            return true;
        }

        /// <summary>
        /// Ativa uma habilidade desbloqueada
        /// </summary>
        public bool ActivateSkill(string skillId)
        {
            if (!IsSkillUnlocked(skillId))
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[SkillTreeManager] Skill '{skillId}' not unlocked");
                return false;
            }

            var skill = skillNodes[skillId];
            
            if (IsSkillActive(skillId))
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"[SkillTreeManager] Skill '{skillId}' already active");
                return false;
            }

            activeSkills.Add(skill);
            ApplySkillEffect(skill);
            
            OnSkillActivated?.Invoke(skill);

            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Activated skill: {skill.displayName}");

            return true;
        }

        /// <summary>
        /// Verifica se uma habilidade pode ser desbloqueada
        /// </summary>
        public bool CanUnlockSkill(string skillId)
        {
            if (!skillNodes.ContainsKey(skillId)) return false;

            var skill = skillNodes[skillId];
            
            // Verifica se já está desbloqueada
            if (IsSkillUnlocked(skillId)) return false;

            // Verifica pontos de habilidade
            if (allowSkillPointSpending && currentSkillPoints < skill.skillPointCost) return false;

            // Verifica estágio de crescimento
            if (integrateWithGrowthSystem && GrowthSystem.Instance != null)
            {
                var currentStage = GrowthSystem.Instance.GetCurrentStage();
                if ((int)currentStage < (int)skill.requiredGrowthStage) return false;
            }

            // Verifica dependências
            if (skill.dependencies != null)
            {
                foreach (var depId in skill.dependencies)
                {
                    if (!IsSkillUnlocked(depId)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se uma habilidade está desbloqueada
        /// </summary>
        public bool IsSkillUnlocked(string skillId)
        {
            return unlockedSkills.Any(s => s.id == skillId);
        }

        /// <summary>
        /// Verifica se uma habilidade está ativa
        /// </summary>
        public bool IsSkillActive(string skillId)
        {
            return activeSkills.Any(s => s.id == skillId);
        }

        /// <summary>
        /// Obtém todas as habilidades desbloqueadas
        /// </summary>
        public List<SkillNode> GetUnlockedSkills()
        {
            return new List<SkillNode>(unlockedSkills);
        }

        /// <summary>
        /// Obtém todas as habilidades ativas
        /// </summary>
        public List<SkillNode> GetActiveSkills()
        {
            return new List<SkillNode>(activeSkills);
        }

        /// <summary>
        /// Obtém habilidades disponíveis para desbloqueio
        /// </summary>
        public List<SkillNode> GetAvailableSkills()
        {
            return skillNodes.Values.Where(s => CanUnlockSkill(s.id)).ToList();
        }

        /// <summary>
        /// Obtém pontos de habilidade atuais
        /// </summary>
        public int GetSkillPoints() => currentSkillPoints;

        /// <summary>
        /// Obtém informações de uma habilidade específica
        /// </summary>
        public SkillNode GetSkillInfo(string skillId)
        {
            return skillNodes.ContainsKey(skillId) ? skillNodes[skillId] : default;
        }
        #endregion

        #region Skill Effects
        private void ApplySkillEffect(SkillNode skill)
        {
            switch (skill.effectType)
            {
                case SkillEffectType.HealthBoost:
                    ApplyHealthBoost(skill.effectValue);
                    break;
                    
                case SkillEffectType.AttackBoost:
                    ApplyAttackBoost(skill.effectValue);
                    break;
                    
                case SkillEffectType.SpeedBoost:
                    ApplySpeedBoost(skill.effectValue);
                    break;
                    
                case SkillEffectType.Regeneration:
                    ApplyRegeneration(skill.effectValue);
                    break;
                    
                case SkillEffectType.CriticalChance:
                    ApplyCriticalChance(skill.effectValue);
                    break;
                    
                case SkillEffectType.DashAbility:
                    EnableDashAbility(skill.effectValue);
                    break;
                    
                case SkillEffectType.AreaAttack:
                    EnableAreaAttack(skill.effectValue);
                    break;
                    
                case SkillEffectType.Shield:
                    EnableShieldAbility(skill.effectValue);
                    break;
                    
                default:
                    if (enableDebugLogs)
                        Debug.LogWarning($"[SkillTreeManager] Unknown skill effect type: {skill.effectType}");
                    break;
            }
        }

        private void ApplyHealthBoost(float multiplier)
        {
            // TODO: Integrar com PlayerAttributesSystem
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Applied health boost: +{multiplier:P0}");
        }

        private void ApplyAttackBoost(float multiplier)
        {
            // TODO: Integrar com PlayerAttributesSystem
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Applied attack boost: +{multiplier:P0}");
        }

        private void ApplySpeedBoost(float multiplier)
        {
            // TODO: Integrar com PlayerController
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Applied speed boost: +{multiplier:P0}");
        }

        private void ApplyRegeneration(float healPerSecond)
        {
            // TODO: Implementar sistema de regeneração
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Applied regeneration: {healPerSecond} HP/s");
        }

        private void ApplyCriticalChance(float chance)
        {
            // TODO: Integrar com sistema de combate
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Applied critical chance: {chance:P0}");
        }

        private void EnableDashAbility(float distance)
        {
            // TODO: Integrar com PlayerController
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Enabled dash ability: {distance} units");
        }

        private void EnableAreaAttack(float areaMultiplier)
        {
            // TODO: Integrar com AttackHandler
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Enabled area attack: {areaMultiplier}x radius");
        }

        private void EnableShieldAbility(float duration)
        {
            // TODO: Implementar sistema de escudo
            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Enabled shield ability: {duration}s duration");
        }
        #endregion

        #region Event Handlers
        private void HandleGrowthStageChanged(GrowthStage from, GrowthStage to)
        {
            if (autoUnlockWithGrowth)
            {
                // Adiciona pontos de habilidade baseado no crescimento
                int pointsToAdd = (int)to - (int)from;
                AddSkillPoints(pointsToAdd);
            }

            if (enableDebugLogs)
                Debug.Log($"[SkillTreeManager] Growth stage changed: {from} -> {to}");
        }
        #endregion

        #region Debug & Editor
        [ContextMenu("Debug - Add 3 Skill Points")]
        private void DebugAddSkillPoints()
        {
            AddSkillPoints(3);
        }

        [ContextMenu("Debug - Unlock All Tier 1")]
        private void DebugUnlockTier1()
        {
            var tier1Skills = skillNodes.Values.Where(s => s.tier == 1).ToList();
            foreach (var skill in tier1Skills)
            {
                if (CanUnlockSkill(skill.id))
                {
                    UnlockSkill(skill.id);
                }
            }
        }

        [ContextMenu("Debug - Reset All Skills")]
        private void DebugResetSkills()
        {
            unlockedSkills.Clear();
            activeSkills.Clear();
            currentSkillPoints = 0;
            OnSkillTreeUpdated?.Invoke();
            OnSkillPointsChanged?.Invoke(currentSkillPoints);
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos || !enabled) return;

            if (Application.isPlaying && isInitialized)
            {
                Vector3 pos = transform.position + Vector3.up * 3f;
                
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(
                    pos,
                    $"Skill Points: {currentSkillPoints}\n" +
                    $"Unlocked: {unlockedSkills.Count}/{skillNodes.Count}\n" +
                    $"Active: {activeSkills.Count}"
                );
                #endif
            }
        }
        #endregion
    }

    /// <summary>
    /// Nó da árvore de habilidades
    /// </summary>
    [System.Serializable]
    public struct SkillNode
    {
        public string id;
        public string displayName;
        public string description;
        public int tier;
        public GrowthStage requiredGrowthStage;
        public int skillPointCost;
        public SkillEffectType effectType;
        public float effectValue;
        public bool isPassive;
        public float cooldown;
        public List<string> dependencies;
    }

    /// <summary>
    /// Tipos de efeitos de habilidades
    /// </summary>
    public enum SkillEffectType
    {
        HealthBoost,
        AttackBoost,
        SpeedBoost,
        Regeneration,
        CriticalChance,
        DashAbility,
        AreaAttack,
        Shield
    }
}