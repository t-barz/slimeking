using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Controlador principal do NPC que gerencia referências de dados e integração com sistemas globais.
    /// Este componente deve ser adicionado a todos os GameObjects de NPC.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class NPCController : MonoBehaviour
    {
        [Header("NPC Data")]
        [Tooltip("Referência ao ScriptableObject com dados de configuração do NPC")]
        public NPCData npcData;

        private void Start()
        {
            // TODO: Registrar NPC no NPCManager quando o sistema for implementado
            // if (NPCManager.Instance != null)
            // {
            //     NPCManager.Instance.RegisterNPC(this);
            // }

            // Inicializar valores a partir do NPCData
            if (npcData != null)
            {
                InitializeFromData();
            }
            else
            {
                Debug.LogWarning($"⚠️ NPCController em '{gameObject.name}' não possui NPCData atribuído!", this);
            }
        }

        private void OnDestroy()
        {
            // TODO: Desregistrar NPC do NPCManager quando o sistema for implementado
            // if (NPCManager.Instance != null)
            // {
            //     NPCManager.Instance.UnregisterNPC(this);
            // }
        }

        /// <summary>
        /// Inicializa o NPC com valores do NPCData.
        /// Configura componentes e sistemas baseados nos dados do ScriptableObject.
        /// </summary>
        private void InitializeFromData()
        {
            // Configurar nome do GameObject se não estiver definido
            if (string.IsNullOrEmpty(gameObject.name) || gameObject.name == "GameObject")
            {
                gameObject.name = npcData.npcName;
            }

            // Configurar componente de comportamento se existir
            NPCBehavior behavior = GetComponent<NPCBehavior>();
            if (behavior != null)
            {
                behavior.behaviorType = npcData.behaviorType;
                behavior.detectionRange = npcData.detectionRange;
            }

            // Configurar componente de amizade se existir
            NPCFriendship friendship = GetComponent<NPCFriendship>();
            if (friendship != null && npcData.friendshipData != null)
            {
                friendship.friendshipData = npcData.friendshipData;
            }

            // Configurar componente de diálogo se existir
            NPCDialogue dialogue = GetComponent<NPCDialogue>();
            if (dialogue != null && npcData.dialogueData != null)
            {
                dialogue.dialogueData = npcData.dialogueData;
            }

            // Configurar componentes de IA baseados no tipo
            ConfigureAIComponent();

            Debug.Log($"✅ NPC '{npcData.npcName}' inicializado com sucesso!");
        }

        /// <summary>
        /// Configura o componente de IA apropriado baseado no AIType do NPCData.
        /// </summary>
        private void ConfigureAIComponent()
        {
            switch (npcData.aiType)
            {
                case AIType.Wander:
                    NPCWanderAI wanderAI = GetComponent<NPCWanderAI>();
                    if (wanderAI != null)
                    {
                        wanderAI.wanderRadius = npcData.wanderRadius;
                        wanderAI.wanderSpeed = npcData.wanderSpeed;
                    }
                    break;

                case AIType.Patrol:
                    NPCPatrolAI patrolAI = GetComponent<NPCPatrolAI>();
                    if (patrolAI != null)
                    {
                        patrolAI.patrolPoints = npcData.patrolPoints;
                        patrolAI.patrolSpeed = npcData.patrolSpeed;
                    }
                    break;

                case AIType.Static:
                    // NPCStaticAI não precisa de configuração adicional
                    break;
            }
        }

        /// <summary>
        /// Retorna o HP máximo do NPC a partir do NPCData.
        /// </summary>
        public int GetMaxHP()
        {
            return npcData != null ? npcData.maxHP : 100;
        }

        /// <summary>
        /// Retorna a velocidade de movimento do NPC a partir do NPCData.
        /// </summary>
        public float GetMoveSpeed()
        {
            return npcData != null ? npcData.moveSpeed : 2.5f;
        }
    }
}
