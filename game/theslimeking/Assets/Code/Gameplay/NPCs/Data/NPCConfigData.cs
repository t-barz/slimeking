using System.Collections.Generic;
using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Estrutura de dados de configuração para NPCs.
    /// Usada pelo NPCQuickConfig para armazenar e transferir configurações.
    /// </summary>
    [System.Serializable]
    public class NPCConfigData
    {
        [Header("Basic Information")]
        [Tooltip("Nome do NPC")]
        public string npcName;

        [Tooltip("Nome da espécie (usado para sistema de amizade)")]
        public string speciesName;

        [Header("Behavior Configuration")]
        [Tooltip("Tipo de comportamento social do NPC")]
        public BehaviorType behaviorType;

        [Tooltip("Tipo de IA de movimento do NPC")]
        public AIType aiType;

        [Tooltip("Alcance de detecção do jogador (metros)")]
        public float detectionRange = 5.0f;

        [Header("System Settings")]
        [Tooltip("Habilita sistema de amizade para este NPC")]
        public bool friendshipEnabled;

        [Tooltip("Configurações do sistema de amizade")]
        public FriendshipSettings friendshipSettings;

        [Tooltip("Habilita sistema de diálogo para este NPC")]
        public bool dialogueEnabled;

        [Tooltip("Configurações do sistema de diálogo")]
        public DialogueSettings dialogueSettings;

        [Header("AI Configuration")]
        [Tooltip("Configurações de IA de movimento")]
        public AISettings aiSettings;

        /// <summary>
        /// Construtor padrão que inicializa as configurações com valores default.
        /// </summary>
        public NPCConfigData()
        {
            npcName = string.Empty;
            speciesName = string.Empty;
            behaviorType = BehaviorType.Passivo;
            aiType = AIType.Static;
            detectionRange = 5.0f;
            friendshipEnabled = false;
            dialogueEnabled = false;
            
            aiSettings = new AISettings();
            friendshipSettings = new FriendshipSettings();
            dialogueSettings = new DialogueSettings();
        }
    }

    /// <summary>
    /// Configurações de IA de movimento para NPCs.
    /// Contém parâmetros para Wander e Patrol AI.
    /// </summary>
    [System.Serializable]
    public class AISettings
    {
        [Header("Wander AI Settings")]
        [Tooltip("Raio de vagueio em metros")]
        public float wanderRadius = 5.0f;

        [Tooltip("Velocidade de movimento durante vagueio (m/s)")]
        public float wanderSpeed = 2.0f;

        [Tooltip("Duração da pausa entre movimentos (segundos)")]
        public float pauseDuration = 2.0f;

        [Header("Patrol AI Settings")]
        [Tooltip("Lista de pontos de patrulha (posições locais)")]
        public List<Vector2> patrolPoints = new List<Vector2>();

        [Tooltip("Velocidade de movimento durante patrulha (m/s)")]
        public float patrolSpeed = 2.5f;

        [Tooltip("Tempo de espera em cada ponto de patrulha (segundos)")]
        public float waitAtPoint = 1.0f;

        /// <summary>
        /// Construtor padrão que inicializa com valores default.
        /// </summary>
        public AISettings()
        {
            wanderRadius = 5.0f;
            wanderSpeed = 2.0f;
            pauseDuration = 2.0f;
            patrolPoints = new List<Vector2>();
            patrolSpeed = 2.5f;
            waitAtPoint = 1.0f;
        }
    }

    /// <summary>
    /// Configurações do sistema de amizade para NPCs.
    /// Define níveis inicial e máximo de amizade.
    /// </summary>
    [System.Serializable]
    public class FriendshipSettings
    {
        [Tooltip("Nível inicial de amizade (0 = Desconhecido)")]
        [Range(0, 5)]
        public int initialLevel = 0;

        [Tooltip("Nível máximo de amizade (5 = Companheiro Leal)")]
        [Range(1, 5)]
        public int maxLevel = 5;

        /// <summary>
        /// Construtor padrão que inicializa com valores default.
        /// </summary>
        public FriendshipSettings()
        {
            initialLevel = 0;
            maxLevel = 5;
        }
    }

    /// <summary>
    /// Configurações do sistema de diálogo para NPCs.
    /// Define como o diálogo é acionado e o alcance de trigger.
    /// </summary>
    [System.Serializable]
    public class DialogueSettings
    {
        [Tooltip("Tipo de trigger do diálogo")]
        public DialogueTriggerType triggerType = DialogueTriggerType.Interaction;

        [Tooltip("Alcance do trigger de proximidade (metros)")]
        public float triggerRange = 2.0f;

        /// <summary>
        /// Construtor padrão que inicializa com valores default.
        /// </summary>
        public DialogueSettings()
        {
            triggerType = DialogueTriggerType.Interaction;
            triggerRange = 2.0f;
        }
    }
}
