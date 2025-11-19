using UnityEngine;
using System;
using System.Collections.Generic;

namespace SlimeKing.Core
{
    /// <summary>
    /// Tipos de comportamento de NPCs baseados no relacionamento.
    /// </summary>
    public enum NPCBehaviorType
    {
        Aggressive,  // < 0: NPCs atacam o player
        Neutral,     // 0-10: NPCs são neutros
        Friendly     // > 10: NPCs são amigáveis e podem ajudar
    }

    /// <summary>
    /// Gerencia o sistema de relacionamento entre o Player e os diferentes tipos de NPCs.
    /// Níveis menores que 0 indicam comportamento agressivo, acima de 10 comportamento amigável.
    /// </summary>
    public class NPCManager : MonoBehaviour
    {
        #region Singleton

        public static NPCManager Instance { get; private set; }

        #endregion

        #region Inspector Configuration

        [Header("Configurações de Relacionamento")]
        [SerializeField] private int defaultRelationshipLevel = 5;
        [SerializeField] private int minRelationshipLevel = -10;
        [SerializeField] private int maxRelationshipLevel = 20;

        [Header("Configurações de Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Variables

        private Dictionary<NPCType, int> relationshipLevels = new Dictionary<NPCType, int>();
        private Dictionary<NPCType, int> interactionCounts = new Dictionary<NPCType, int>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Nível mínimo de relacionamento possível.
        /// </summary>
        public int MinRelationshipLevel => minRelationshipLevel;

        /// <summary>
        /// Nível máximo de relacionamento possível.
        /// </summary>
        public int MaxRelationshipLevel => maxRelationshipLevel;

        #endregion

        #region Events

        /// <summary>
        /// Evento disparado quando o nível de relacionamento com um tipo de NPC muda.
        /// Parâmetros: (npcType, oldLevel, newLevel)
        /// </summary>
        public static event Action<NPCType, int, int> OnRelationshipLevelChanged;

        /// <summary>
        /// Evento disparado quando o comportamento de um tipo de NPC muda (agressivo/neutro/amigável).
        /// Parâmetros: (npcType, newBehavior)
        /// </summary>
        public static event Action<NPCType, NPCBehaviorType> OnBehaviorChanged;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeRelationships();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (enableLogs)
            {
                UnityEngine.Debug.Log("[NPCManager] Sistema de relacionamento com NPCs inicializado.");
                LogAllRelationships();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Obtém o nível de relacionamento atual com um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <returns>Nível de relacionamento</returns>
        public int GetRelationshipLevel(NPCType npcType)
        {
            if (relationshipLevels.ContainsKey(npcType))
            {
                return relationshipLevels[npcType];
            }

            // Se não existe, inicializa com valor padrão
            SetRelationshipLevel(npcType, defaultRelationshipLevel);
            return defaultRelationshipLevel;
        }

        /// <summary>
        /// Define o nível de relacionamento com um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <param name="level">Novo nível de relacionamento</param>
        public void SetRelationshipLevel(NPCType npcType, int level)
        {
            int clampedLevel = Mathf.Clamp(level, minRelationshipLevel, maxRelationshipLevel);
            int oldLevel = relationshipLevels.ContainsKey(npcType) ? relationshipLevels[npcType] : defaultRelationshipLevel;

            relationshipLevels[npcType] = clampedLevel;

            if (oldLevel != clampedLevel)
            {
                if (enableLogs)
                {
                    UnityEngine.Debug.Log($"[NPCManager] Relacionamento com {npcType} alterado: {oldLevel} → {clampedLevel}");
                }

                OnRelationshipLevelChanged?.Invoke(npcType, oldLevel, clampedLevel);

                // Verifica se houve mudança de comportamento
                NPCBehaviorType oldBehavior = GetBehaviorType(oldLevel);
                NPCBehaviorType newBehavior = GetBehaviorType(clampedLevel);

                if (oldBehavior != newBehavior)
                {
                    OnBehaviorChanged?.Invoke(npcType, newBehavior);

                    if (enableLogs)
                    {
                        UnityEngine.Debug.Log($"[NPCManager] Comportamento de {npcType} mudou para: {newBehavior}");
                    }
                }
            }
        }

        /// <summary>
        /// Modifica o nível de relacionamento com um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <param name="change">Quantidade a ser adicionada/subtraída</param>
        public void ModifyRelationshipLevel(NPCType npcType, int change)
        {
            int currentLevel = GetRelationshipLevel(npcType);
            SetRelationshipLevel(npcType, currentLevel + change);
        }

        /// <summary>
        /// Obtém o tipo de comportamento baseado no nível de relacionamento.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <returns>Tipo de comportamento</returns>
        public NPCBehaviorType GetBehaviorType(NPCType npcType)
        {
            int level = GetRelationshipLevel(npcType);
            return GetBehaviorType(level);
        }

        /// <summary>
        /// Registra uma interação positiva com um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <param name="relationshipGain">Quantidade de relacionamento ganho</param>
        public void RegisterPositiveInteraction(NPCType npcType, int relationshipGain = 1)
        {
            ModifyRelationshipLevel(npcType, relationshipGain);
            IncrementInteractionCount(npcType);

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCManager] Interação positiva com {npcType} (+{relationshipGain})");
            }
        }

        /// <summary>
        /// Registra uma interação negativa com um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <param name="relationshipLoss">Quantidade de relacionamento perdido</param>
        public void RegisterNegativeInteraction(NPCType npcType, int relationshipLoss = 1)
        {
            ModifyRelationshipLevel(npcType, -relationshipLoss);
            IncrementInteractionCount(npcType);

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCManager] Interação negativa com {npcType} (-{relationshipLoss})");
            }
        }

        /// <summary>
        /// Obtém o número total de interações com um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <returns>Número de interações</returns>
        public int GetInteractionCount(NPCType npcType)
        {
            return interactionCounts.ContainsKey(npcType) ? interactionCounts[npcType] : 0;
        }

        /// <summary>
        /// Verifica se o Player é hostil para um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <returns>True se for hostil</returns>
        public bool IsHostile(NPCType npcType)
        {
            return GetBehaviorType(npcType) == NPCBehaviorType.Aggressive;
        }

        /// <summary>
        /// Verifica se o Player é amigável para um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        /// <returns>True se for amigável</returns>
        public bool IsFriendly(NPCType npcType)
        {
            return GetBehaviorType(npcType) == NPCBehaviorType.Friendly;
        }

        /// <summary>
        /// Obtém um resumo de todos os relacionamentos atuais.
        /// </summary>
        /// <returns>String formatada com todos os relacionamentos</returns>
        public string GetRelationshipSummary()
        {
            string summary = "=== Relacionamentos com NPCs ===\n";

            foreach (NPCType npcType in System.Enum.GetValues(typeof(NPCType)))
            {
                int level = GetRelationshipLevel(npcType);
                NPCBehaviorType behavior = GetBehaviorType(level);
                int interactions = GetInteractionCount(npcType);

                summary += $"{npcType}: Nível {level} ({behavior}) - {interactions} interações\n";
            }

            return summary;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Inicializa todos os tipos de NPC com relacionamento padrão.
        /// </summary>
        private void InitializeRelationships()
        {
            foreach (NPCType npcType in System.Enum.GetValues(typeof(NPCType)))
            {
                relationshipLevels[npcType] = defaultRelationshipLevel;
                interactionCounts[npcType] = 0;
            }

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[NPCManager] Relacionamentos inicializados com nível padrão: {defaultRelationshipLevel}");
            }
        }

        /// <summary>
        /// Obtém o tipo de comportamento baseado no nível numérico.
        /// </summary>
        /// <param name="level">Nível de relacionamento</param>
        /// <returns>Tipo de comportamento</returns>
        private NPCBehaviorType GetBehaviorType(int level)
        {
            if (level < 0)
                return NPCBehaviorType.Aggressive;
            else if (level > 10)
                return NPCBehaviorType.Friendly;
            else
                return NPCBehaviorType.Neutral;
        }

        /// <summary>
        /// Incrementa o contador de interações com um tipo de NPC.
        /// </summary>
        /// <param name="npcType">Tipo do NPC</param>
        private void IncrementInteractionCount(NPCType npcType)
        {
            if (interactionCounts.ContainsKey(npcType))
                interactionCounts[npcType]++;
            else
                interactionCounts[npcType] = 1;
        }

        /// <summary>
        /// Exibe no log todos os relacionamentos atuais (apenas para debug).
        /// </summary>
        private void LogAllRelationships()
        {
            if (!enableLogs) return;

            UnityEngine.Debug.Log($"[NPCManager] Estado atual dos relacionamentos:\n{GetRelationshipSummary()}");
        }

        #endregion
    }
}