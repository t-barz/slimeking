using System.Collections.Generic;
using UnityEngine;
using SlimeMec.Gameplay;

namespace SlimeKing.Editor
{
    /// <summary>
    /// ScriptableObject que armazena templates pré-configurados de NPCs.
    /// Usado pelo NPCQuickConfig para carregar configurações rapidamente.
    /// </summary>
    [CreateAssetMenu(fileName = "NPCTemplate", menuName = "QuickWinds/NPC Template")]
    public class NPCTemplateData : ScriptableObject
    {
        [Header("Template Information")]
        [Tooltip("Nome do template (ex: Cervo-Broto, Esquilo Coletor)")]
        public string templateName;

        [Tooltip("Descrição do template e uso recomendado")]
        [TextArea(2, 4)]
        public string description;

        [Tooltip("Ícone do template (opcional)")]
        public Sprite iconSprite;

        [Header("Basic Configuration")]
        [Tooltip("Nome padrão do NPC")]
        public string npcName;

        [Tooltip("Nome da espécie para sistema de amizade")]
        public string speciesName;

        [Header("Behavior Configuration")]
        [Tooltip("Tipo de comportamento social do NPC")]
        public BehaviorType behaviorType;

        [Tooltip("Tipo de IA de movimento do NPC")]
        public AIType aiType;

        [Tooltip("Alcance de detecção do jogador (metros)")]
        public float detectionRange = 5.0f;

        [Header("AI Settings - Wander")]
        [Tooltip("Raio de vagueio para AI tipo Wander (metros)")]
        public float wanderRadius = 5.0f;

        [Tooltip("Velocidade de vagueio para AI tipo Wander (m/s)")]
        public float wanderSpeed = 2.0f;

        [Tooltip("Duração da pausa entre movimentos (segundos)")]
        public float pauseDuration = 2.0f;

        [Header("AI Settings - Patrol")]
        [Tooltip("Pontos de patrulha para AI tipo Patrol")]
        public List<Vector2> patrolPoints = new List<Vector2>();

        [Tooltip("Velocidade de patrulha para AI tipo Patrol (m/s)")]
        public float patrolSpeed = 2.5f;

        [Tooltip("Tempo de espera em cada ponto de patrulha (segundos)")]
        public float waitAtPoint = 1.0f;

        [Header("System Configuration")]
        [Tooltip("Habilita sistema de amizade para este NPC")]
        public bool friendshipEnabled;

        [Tooltip("Nível inicial de amizade (0 = Desconhecido)")]
        [Range(0, 5)]
        public int initialFriendshipLevel = 0;

        [Tooltip("Nível máximo de amizade (5 = Companheiro Leal)")]
        [Range(1, 5)]
        public int maxFriendshipLevel = 5;

        [Tooltip("Habilita sistema de diálogo para este NPC")]
        public bool dialogueEnabled;

        [Tooltip("Tipo de trigger do diálogo")]
        public DialogueTriggerType dialogueTriggerType = DialogueTriggerType.Interaction;

        [Tooltip("Alcance do trigger de proximidade (metros)")]
        public float triggerRange = 2.0f;

        [Header("Component References")]
        [Tooltip("Animator Controller padrão para este template (opcional)")]
        public RuntimeAnimatorController defaultAnimatorController;

        [Tooltip("Material padrão para o SpriteRenderer (opcional)")]
        public Material defaultMaterial;
    }
}
