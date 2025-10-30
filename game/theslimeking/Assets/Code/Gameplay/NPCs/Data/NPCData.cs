using System.Collections.Generic;
using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// ScriptableObject que armazena todos os dados de configuração de um NPC.
    /// Criado automaticamente pelo NPCQuickConfig ou manualmente pelo desenvolvedor.
    /// </summary>
    [CreateAssetMenu(fileName = "NPCData", menuName = "Game/NPC Data")]
    public class NPCData : ScriptableObject
    {
        [Header("Basic Information")]
        [Tooltip("Nome do NPC (ex: Cervo-Broto, Esquilo Coletor)")]
        public string npcName;

        [Tooltip("Espécie do NPC para sistema de amizade (ex: Cervo, Esquilo)")]
        public string species;

        [Header("Behavior Configuration")]
        [Tooltip("Tipo de comportamento social do NPC")]
        public BehaviorType behaviorType;

        [Tooltip("Tipo de IA de movimento do NPC")]
        public AIType aiType;

        [Header("Stats")]
        [Tooltip("HP máximo do NPC")]
        public int maxHP = 100;

        [Tooltip("Velocidade de movimento do NPC (m/s)")]
        public float moveSpeed = 2.5f;

        [Tooltip("Alcance de detecção do jogador (metros)")]
        public float detectionRange = 5.0f;

        [Header("AI Settings - Wander")]
        [Tooltip("Raio de vagueio para AI tipo Wander (metros)")]
        public float wanderRadius = 5.0f;

        [Tooltip("Velocidade de vagueio para AI tipo Wander (m/s)")]
        public float wanderSpeed = 2.0f;

        [Header("AI Settings - Patrol")]
        [Tooltip("Pontos de patrulha para AI tipo Patrol")]
        public List<Vector2> patrolPoints = new List<Vector2>();

        [Tooltip("Velocidade de patrulha para AI tipo Patrol (m/s)")]
        public float patrolSpeed = 2.5f;

        [Header("System References")]
        [Tooltip("Dados de amizade da espécie (se sistema de amizade habilitado)")]
        public FriendshipData friendshipData;

        [Tooltip("Dados de diálogo do NPC (se sistema de diálogo habilitado)")]
        public DialogueData dialogueData;
    }
}
