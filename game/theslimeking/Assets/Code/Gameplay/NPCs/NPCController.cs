using UnityEngine;
using SlimeMec.Systems.Controllers;
using SlimeMec.Systems.Data;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Controlador principal do NPC que gerencia comportamento, movimento, combate e interações.
    /// Integra-se com NPCAttributesHandler e Behavior Graph da Unity 6.2.
    /// Este componente deve ser adicionado a todos os GameObjects de NPC.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(NPCAttributesHandler))]
    public class NPCController : MonoBehaviour
    {
        #region Category Settings

        [Header("Category Settings")]
        [Tooltip("Categoria do NPC que define comportamento geral")]
        [SerializeField] private NPCCategory npcCategory = NPCCategory.Neutral;

        [Tooltip("Nome do NPC para identificação")]
        [SerializeField] private string npcName = "NPC";

        #endregion

        #region Movement Settings

        [Header("Movement Settings")]
        [Tooltip("Padrão de movimentação do NPC")]
        [SerializeField] private MovementPattern movementPattern = MovementPattern.Idle;

        [Tooltip("Pontos de patrulha (usado com PatrolPoints)")]
        [SerializeField] private Transform[] patrolPoints;

        [Tooltip("Centro da patrulha circular (usado com CircularPatrol)")]
        [SerializeField] private Vector2 patrolCenter;

        [Tooltip("Raio da patrulha circular (usado com CircularPatrol)")]
        [SerializeField] private float patrolRadius = 5f;

        [Tooltip("Velocidade de patrulha circular")]
        [SerializeField] private float circularPatrolSpeed = 2f;

        [Tooltip("Distância para considerar waypoint alcançado")]
        [SerializeField] private float waypointReachThreshold = 0.5f;

        [Tooltip("Aceleração do movimento")]
        [SerializeField] private float acceleration = 5f;

        [Tooltip("Desaceleração do movimento")]
        [SerializeField] private float deceleration = 5f;

        #endregion

        #region Combat Settings

        [Header("Combat Settings")]
        [Tooltip("Alcance de detecção de alvos")]
        [SerializeField] private float detectionRange = 5f;

        [Tooltip("Alcance de ataque")]
        [SerializeField] private float attackRange = 1.5f;

        [Tooltip("Cooldown entre ataques em segundos")]
        [SerializeField] private float attackCooldown = 1.5f;

        [Tooltip("Layers que podem ser detectados como alvos")]
        [SerializeField] private LayerMask targetLayers;

        [Tooltip("Prefab de efeito visual de ataque")]
        [SerializeField] private GameObject attackVFX;

        [Tooltip("Sons de ataque (seleção aleatória)")]
        [SerializeField] private AudioClip[] attackSounds;

        #endregion

        #region Interaction Settings

        [Header("Interaction Settings")]
        [Tooltip("Alcance de interação com jogador")]
        [SerializeField] private float interactionRange = 2f;

        [Tooltip("Dados de interações disponíveis")]
        [SerializeField] private NPCInteractionData[] interactions;

        #endregion

        #region Drop Settings

        [Header("Drop Settings")]
        [Tooltip("Dados de drops ao ser derrotado")]
        [SerializeField] private NPCDropData[] possibleDrops;

        #endregion

        #region Behavior Graph Settings

        [Header("Behavior Graph Settings")]
        [Tooltip("Prefab do Behavior Graph (Unity 6.2)")]
        [SerializeField] private GameObject behaviorGraphPrefab;

        #endregion

        #region Debug Settings

        [Header("Debug Settings")]
        [Tooltip("Habilitar logs de debug")]
        [SerializeField] private bool enableLogs = false;

        [Tooltip("Habilitar gizmos de debug")]
        [SerializeField] private bool enableDebugGizmos = true;

        #endregion

        #region Private Variables

        private NPCState currentState = NPCState.Idle;
        private NPCAttributesHandler attributesHandler;
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private Transform currentTarget;
        private int currentPatrolIndex = 0;
        private float lastAttackTime = 0f;
        private Vector2 currentVelocity;
        private GameObject behaviorGraphInstance;

        #endregion

        #region Public Properties

        /// <summary>
        /// Retorna o estado atual do NPC.
        /// </summary>
        public NPCState GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Retorna a categoria do NPC.
        /// </summary>
        public NPCCategory GetCategory()
        {
            return npcCategory;
        }

        /// <summary>
        /// Retorna se o NPC está vivo.
        /// </summary>
        public bool IsAlive()
        {
            return currentState != NPCState.Dead;
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Cache de componentes obrigatórios
            rb = GetComponent<Rigidbody2D>();
            attributesHandler = GetComponent<NPCAttributesHandler>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Validação de componentes obrigatórios
            if (rb == null)
            {
                Debug.LogError($"[NPCController] {gameObject.name} não possui Rigidbody2D!", this);
            }

            if (attributesHandler == null)
            {
                Debug.LogError($"[NPCController] {gameObject.name} não possui NPCAttributesHandler!", this);
            }

            // Validação de configuração de patrulha
            if (movementPattern == MovementPattern.PatrolPoints)
            {
                if (patrolPoints == null || patrolPoints.Length == 0)
                {
                    Debug.LogWarning($"[NPCController] {gameObject.name} está configurado para PatrolPoints mas não possui pontos definidos!", this);
                    movementPattern = MovementPattern.Idle;
                }
            }

            // Validação de Behavior Graph
            if (behaviorGraphPrefab == null && npcCategory == NPCCategory.Boss)
            {
                Debug.LogWarning($"[NPCController] {gameObject.name} é um Boss mas não possui Behavior Graph atribuído!", this);
            }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} inicializado. Categoria: {npcCategory}, Movimento: {movementPattern}");
            }
        }

        private void Start()
        {
            // Subscribe aos eventos do AttributesHandler
            if (attributesHandler != null)
            {
                attributesHandler.OnHealthChanged += HandleHealthChanged;
                attributesHandler.OnNPCDied += HandleNPCDied;
                attributesHandler.OnDamageTaken += HandleDamageTaken;
            }

            // Configurar nome do GameObject
            if (string.IsNullOrEmpty(gameObject.name) || gameObject.name == "GameObject")
            {
                gameObject.name = npcName;
            }

            // Inicializar centro de patrulha circular se não definido
            if (movementPattern == MovementPattern.CircularPatrol && patrolCenter == Vector2.zero)
            {
                patrolCenter = transform.position;
            }

            // Carregar Behavior Graph se disponível
            LoadBehaviorGraph();

            // Registrar no GameManager se disponível
            RegisterWithGameManager();

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} pronto para ação!");
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe dos eventos
            if (attributesHandler != null)
            {
                attributesHandler.OnHealthChanged -= HandleHealthChanged;
                attributesHandler.OnNPCDied -= HandleNPCDied;
                attributesHandler.OnDamageTaken -= HandleDamageTaken;
            }

            // Desregistrar do GameManager
            UnregisterFromGameManager();
        }

        #endregion

        #region Event Handlers

        private void HandleHealthChanged(int currentHealth, int maxHealth)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} HP mudou: {currentHealth}/{maxHealth}");
            }
        }

        private void HandleNPCDied()
        {
            currentState = NPCState.Dead;
            
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} morreu!");
            }

            // Parar movimento
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Atualizar animação se disponível
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }

            // Processar drops
            ProcessDrops();
        }

        private void HandleDamageTaken(int damage)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} recebeu {damage} de dano!");
            }
        }

        #endregion

        #region Movement System

        private void FixedUpdate()
        {
            if (!IsAlive() || currentState == NPCState.Dead)
            {
                return;
            }

            // Executar movimento baseado no padrão atual
            switch (movementPattern)
            {
                case MovementPattern.Idle:
                    HandleIdleMovement();
                    break;

                case MovementPattern.PatrolPoints:
                    HandlePatrolPointsMovement();
                    break;

                case MovementPattern.CircularPatrol:
                    HandleCircularPatrolMovement();
                    break;

                case MovementPattern.ChaseTarget:
                    HandleChaseTargetMovement();
                    break;
            }

            // Atualizar animação de velocidade se disponível
            if (animator != null)
            {
                animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            }
        }

        /// <summary>
        /// Movimento Idle - NPC parado.
        /// </summary>
        private void HandleIdleMovement()
        {
            // Aplicar desaceleração suave
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb.linearVelocity = currentVelocity;
        }

        /// <summary>
        /// Movimento PatrolPoints - NPC patrulha entre waypoints.
        /// </summary>
        private void HandlePatrolPointsMovement()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                HandleIdleMovement();
                return;
            }

            Transform targetPoint = patrolPoints[currentPatrolIndex];
            if (targetPoint == null)
            {
                HandleIdleMovement();
                return;
            }

            Vector2 targetPosition = targetPoint.position;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            float distance = Vector2.Distance(transform.position, targetPosition);

            // Verificar se alcançou o waypoint
            if (distance <= waypointReachThreshold)
            {
                // Avançar para o próximo waypoint
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                
                if (enableLogs)
                {
                    Debug.Log($"[NPCController] {npcName} alcançou waypoint {currentPatrolIndex}");
                }
            }
            else
            {
                // Mover em direção ao waypoint
                float moveSpeed = attributesHandler != null ? attributesHandler.CurrentSpeed : 2f;
                Vector2 targetVelocity = direction * moveSpeed;
                currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
                rb.linearVelocity = currentVelocity;
            }
        }

        /// <summary>
        /// Movimento CircularPatrol - NPC patrulha em área circular.
        /// </summary>
        private void HandleCircularPatrolMovement()
        {
            // Calcular posição aleatória dentro do círculo
            float angle = Time.time * circularPatrolSpeed;
            float randomOffset = Mathf.PerlinNoise(Time.time * 0.5f, 0f) * patrolRadius;
            
            Vector2 targetPosition = patrolCenter + new Vector2(
                Mathf.Cos(angle) * randomOffset,
                Mathf.Sin(angle) * randomOffset
            );

            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            float moveSpeed = attributesHandler != null ? attributesHandler.CurrentSpeed : 2f;
            
            Vector2 targetVelocity = direction * moveSpeed * 0.5f; // Movimento mais lento em patrulha circular
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = currentVelocity;
        }

        /// <summary>
        /// Movimento ChaseTarget - NPC persegue um alvo.
        /// </summary>
        private void HandleChaseTargetMovement()
        {
            if (currentTarget == null)
            {
                HandleIdleMovement();
                return;
            }

            Vector2 targetPosition = currentTarget.position;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            float moveSpeed = attributesHandler != null ? attributesHandler.CurrentSpeed : 2f;

            Vector2 targetVelocity = direction * moveSpeed;
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = currentVelocity;
        }

        /// <summary>
        /// Define o padrão de movimentação do NPC.
        /// </summary>
        public void SetMovementPattern(MovementPattern pattern)
        {
            movementPattern = pattern;
            
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} padrão de movimento alterado para {pattern}");
            }
        }

        /// <summary>
        /// Define os pontos de patrulha para PatrolPoints.
        /// </summary>
        public void SetPatrolPoints(Transform[] points)
        {
            patrolPoints = points;
            currentPatrolIndex = 0;
            
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} pontos de patrulha definidos: {points.Length} pontos");
            }
        }

        /// <summary>
        /// Define a configuração de patrulha circular.
        /// </summary>
        public void SetCircularPatrol(Vector2 center, float radius, float speed)
        {
            patrolCenter = center;
            patrolRadius = radius;
            circularPatrolSpeed = speed;
            
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} patrulha circular configurada: Centro={center}, Raio={radius}");
            }
        }

        /// <summary>
        /// Define um alvo para perseguir.
        /// </summary>
        public void ChaseTarget(Transform target)
        {
            currentTarget = target;
            movementPattern = MovementPattern.ChaseTarget;
            
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} perseguindo {target.name}");
            }
        }

        /// <summary>
        /// Para o movimento do NPC.
        /// </summary>
        public void StopMovement()
        {
            movementPattern = MovementPattern.Idle;
            currentTarget = null;
            
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} movimento parado");
            }
        }

        #endregion

        #region Combat System

        private void Update()
        {
            if (!IsAlive() || currentState == NPCState.Dead)
            {
                return;
            }

            // Detecção de alvos
            DetectTargets();

            // Lógica de combate baseada no estado
            if (currentState == NPCState.Chasing || currentState == NPCState.Attacking)
            {
                HandleCombatLogic();
            }
        }

        /// <summary>
        /// Detecta alvos próximos usando Physics2D.OverlapCircle.
        /// </summary>
        private void DetectTargets()
        {
            // Apenas detectar se não estiver morto ou interagindo
            if (currentState == NPCState.Dead || currentState == NPCState.Interacting)
            {
                return;
            }

            // Detectar alvos no alcance
            Collider2D[] detectedColliders = Physics2D.OverlapCircleAll(transform.position, detectionRange, targetLayers);

            if (detectedColliders.Length > 0)
            {
                // Pegar o alvo mais próximo
                Transform closestTarget = null;
                float closestDistance = float.MaxValue;

                foreach (Collider2D col in detectedColliders)
                {
                    float distance = Vector2.Distance(transform.position, col.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = col.transform;
                    }
                }

                // Definir alvo e mudar para estado de perseguição
                if (closestTarget != null && currentTarget != closestTarget)
                {
                    currentTarget = closestTarget;
                    currentState = NPCState.Chasing;
                    ChaseTarget(currentTarget);

                    if (enableLogs)
                    {
                        Debug.Log($"[NPCController] {npcName} detectou alvo: {currentTarget.name}");
                    }
                }
            }
            else
            {
                // Sem alvos detectados, voltar para patrulha
                if (currentState == NPCState.Chasing || currentState == NPCState.Attacking)
                {
                    currentTarget = null;
                    currentState = NPCState.Patrolling;
                    
                    if (enableLogs)
                    {
                        Debug.Log($"[NPCController] {npcName} perdeu o alvo");
                    }
                }
            }
        }

        /// <summary>
        /// Lógica de combate quando há um alvo.
        /// </summary>
        private void HandleCombatLogic()
        {
            if (currentTarget == null)
            {
                return;
            }

            // Verificar se alvo está no alcance de ataque
            if (IsTargetInRange(currentTarget, attackRange))
            {
                currentState = NPCState.Attacking;
                StopMovement();
                PerformAttack();
            }
            else
            {
                currentState = NPCState.Chasing;
            }
        }

        /// <summary>
        /// Verifica se um alvo está dentro de um alcance específico.
        /// </summary>
        private bool IsTargetInRange(Transform target, float range)
        {
            if (target == null) return false;
            
            float distance = Vector2.Distance(transform.position, target.position);
            return distance <= range;
        }

        /// <summary>
        /// Executa um ataque se o cooldown permitir.
        /// </summary>
        private void PerformAttack()
        {
            // Verificar cooldown
            if (Time.time - lastAttackTime < attackCooldown)
            {
                return;
            }

            lastAttackTime = Time.time;

            // Trigger animação de ataque
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            // Reproduzir som de ataque
            PlayRandomSound(attackSounds);

            // Instanciar efeito visual
            if (attackVFX != null)
            {
                Instantiate(attackVFX, transform.position, Quaternion.identity);
            }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} atacou!");
            }

            // Nota: O dano real é aplicado no OnAttackAnimationHit() chamado pelo Animation Event
        }

        /// <summary>
        /// Chamado por Animation Event para aplicar dano no momento correto da animação.
        /// </summary>
        public void OnAttackAnimationHit()
        {
            if (currentTarget == null || !IsAlive())
            {
                return;
            }

            // Verificar se alvo ainda está no alcance
            if (!IsTargetInRange(currentTarget, attackRange))
            {
                return;
            }

            // Tentar aplicar dano ao alvo
            // Verificar se é o jogador
            if (currentTarget.CompareTag("Player"))
            {
                PlayerAttributesHandler playerAttributes = currentTarget.GetComponent<PlayerAttributesHandler>();
                if (playerAttributes != null && attributesHandler != null)
                {
                    int damage = attributesHandler.CurrentAttack;
                    playerAttributes.TakeDamage(damage);

                    if (enableLogs)
                    {
                        Debug.Log($"[NPCController] {npcName} causou {damage} de dano ao jogador!");
                    }
                }
            }
            else
            {
                // Tentar aplicar dano a outro NPC
                NPCAttributesHandler targetAttributes = currentTarget.GetComponent<NPCAttributesHandler>();
                if (targetAttributes != null && attributesHandler != null)
                {
                    int damage = attributesHandler.CurrentAttack;
                    targetAttributes.TakeDamage(damage);

                    if (enableLogs)
                    {
                        Debug.Log($"[NPCController] {npcName} causou {damage} de dano a {currentTarget.name}!");
                    }
                }
            }
        }

        #endregion

        #region Interaction System

        /// <summary>
        /// Processa uma interação com o jogador.
        /// </summary>
        /// <param name="interactor">GameObject que está interagindo (geralmente o jogador)</param>
        public void Interact(GameObject interactor)
        {
            if (!CanInteractWith(interactor))
            {
                if (enableLogs)
                {
                    Debug.Log($"[NPCController] {npcName} não pode interagir com {interactor.name}");
                }
                return;
            }

            // Mudar para estado de interação
            NPCState previousState = currentState;
            currentState = NPCState.Interacting;
            StopMovement();

            // Processar cada interação disponível
            if (interactions != null && interactions.Length > 0)
            {
                foreach (NPCInteractionData interaction in interactions)
                {
                    if (interaction == null || !interaction.CanBeUsed())
                    {
                        continue;
                    }

                    // Verificar requisitos de relacionamento
                    if (attributesHandler != null && 
                        !interaction.MeetsRelationshipRequirement(attributesHandler.RelationshipPoints))
                    {
                        if (enableLogs)
                        {
                            Debug.Log($"[NPCController] {npcName} interação '{interaction.interactionName}' requer relacionamento {interaction.requiredRelationshipPoints}");
                        }
                        continue;
                    }

                    // Processar interação baseada no tipo
                    ProcessInteraction(interaction, interactor);
                }
            }

            // Voltar ao estado anterior após interação
            currentState = previousState;
        }

        /// <summary>
        /// Verifica se pode interagir com um GameObject.
        /// </summary>
        /// <param name="interactor">GameObject que deseja interagir</param>
        /// <returns>True se pode interagir</returns>
        public bool CanInteractWith(GameObject interactor)
        {
            if (!IsAlive() || currentState == NPCState.Dead)
            {
                return false;
            }

            // Verificar distância
            float distance = Vector2.Distance(transform.position, interactor.transform.position);
            if (distance > interactionRange)
            {
                return false;
            }

            // Verificar se tem interações disponíveis
            if (interactions == null || interactions.Length == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Processa uma interação específica baseada no tipo.
        /// </summary>
        private void ProcessInteraction(NPCInteractionData interaction, GameObject interactor)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} processando interação: {interaction.interactionName} ({interaction.interactionType})");
            }

            switch (interaction.interactionType)
            {
                case InteractionType.Dialogue:
                    ProcessDialogueInteraction(interaction);
                    break;

                case InteractionType.ItemDelivery:
                    ProcessItemDeliveryInteraction(interaction, interactor);
                    break;

                case InteractionType.QuestActivation:
                    ProcessQuestActivationInteraction(interaction);
                    break;

                case InteractionType.QuestCompletion:
                    ProcessQuestCompletionInteraction(interaction);
                    break;

                case InteractionType.Shop:
                    ProcessShopInteraction(interaction);
                    break;
            }

            // Marcar como consumida se for oneTimeOnly
            interaction.MarkAsConsumed();
        }

        /// <summary>
        /// Processa interação de diálogo.
        /// </summary>
        private void ProcessDialogueInteraction(NPCInteractionData interaction)
        {
            if (string.IsNullOrEmpty(interaction.dialogueID))
            {
                Debug.LogWarning($"[NPCController] {npcName} interação de diálogo sem dialogueID!");
                return;
            }

            // TODO: Integrar com DialogueManager quando disponível
            // DialogueManager.Instance.StartDialogue(interaction.dialogueID);
            
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} iniciando diálogo: {interaction.dialogueID}");
            }
        }

        /// <summary>
        /// Processa interação de entrega de itens.
        /// </summary>
        private void ProcessItemDeliveryInteraction(NPCInteractionData interaction, GameObject interactor)
        {
            if (interaction.requiredItemIDs == null || interaction.requiredItemIDs.Length == 0)
            {
                Debug.LogWarning($"[NPCController] {npcName} interação de entrega sem itens requeridos!");
                return;
            }

            // TODO: Integrar com InventoryManager quando disponível
            // bool hasAllItems = InventoryManager.Instance.HasItems(interaction.requiredItemIDs);
            // if (hasAllItems)
            // {
            //     InventoryManager.Instance.RemoveItems(interaction.requiredItemIDs);
            //     // Dar recompensa ou avançar quest
            // }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} processando entrega de {interaction.requiredItemIDs.Length} itens");
            }
        }

        /// <summary>
        /// Processa interação de ativação de quest.
        /// </summary>
        private void ProcessQuestActivationInteraction(NPCInteractionData interaction)
        {
            if (string.IsNullOrEmpty(interaction.questID))
            {
                Debug.LogWarning($"[NPCController] {npcName} interação de quest sem questID!");
                return;
            }

            // TODO: Integrar com QuestManager quando disponível
            // QuestManager.Instance.ActivateQuest(interaction.questID);

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} ativando quest: {interaction.questID}");
            }
        }

        /// <summary>
        /// Processa interação de conclusão de quest.
        /// </summary>
        private void ProcessQuestCompletionInteraction(NPCInteractionData interaction)
        {
            if (string.IsNullOrEmpty(interaction.questID))
            {
                Debug.LogWarning($"[NPCController] {npcName} interação de conclusão sem questID!");
                return;
            }

            // TODO: Integrar com QuestManager quando disponível
            // QuestManager.Instance.CompleteQuest(interaction.questID);

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} completando quest: {interaction.questID}");
            }
        }

        /// <summary>
        /// Processa interação de loja.
        /// </summary>
        private void ProcessShopInteraction(NPCInteractionData interaction)
        {
            if (string.IsNullOrEmpty(interaction.shopID))
            {
                Debug.LogWarning($"[NPCController] {npcName} interação de loja sem shopID!");
                return;
            }

            // TODO: Integrar com ShopManager quando disponível
            // ShopManager.Instance.OpenShop(interaction.shopID);

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} abrindo loja: {interaction.shopID}");
            }
        }

        #endregion

        #region Drop System

        /// <summary>
        /// Processa os drops quando o NPC morre.
        /// </summary>
        private void ProcessDrops()
        {
            if (possibleDrops == null || possibleDrops.Length == 0)
            {
                if (enableLogs)
                {
                    Debug.Log($"[NPCController] {npcName} não possui drops configurados");
                }
                return;
            }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} processando {possibleDrops.Length} possíveis drops");
            }

            foreach (NPCDropData drop in possibleDrops)
            {
                if (drop == null || drop.itemPrefab == null)
                {
                    continue;
                }

                // Verificar se deve dropar baseado na chance
                if (!drop.ShouldDrop())
                {
                    if (enableLogs)
                    {
                        Debug.Log($"[NPCController] {npcName} não dropou {drop.dropName} (chance: {drop.dropChance}%)");
                    }
                    continue;
                }

                // Determinar quantidade a dropar
                int quantity = drop.GetRandomQuantity();

                if (enableLogs)
                {
                    Debug.Log($"[NPCController] {npcName} dropando {quantity}x {drop.dropName}");
                }

                // Instanciar os itens
                for (int i = 0; i < quantity; i++)
                {
                    SpawnDropItem(drop.itemPrefab);
                }
            }
        }

        /// <summary>
        /// Instancia um item dropado na posição do NPC com offset aleatório.
        /// </summary>
        private void SpawnDropItem(GameObject itemPrefab)
        {
            // Adicionar offset aleatório para evitar sobreposição
            Vector2 randomOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            GameObject droppedItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} spawnou item {itemPrefab.name} em {spawnPosition}");
            }

            // Adicionar pequena força para espalhar os itens
            Rigidbody2D itemRb = droppedItem.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                Vector2 randomForce = Random.insideUnitCircle * 2f;
                itemRb.AddForce(randomForce, ForceMode2D.Impulse);
            }
        }

        #endregion

        #region Behavior Graph Integration

        /// <summary>
        /// Carrega e instancia o Behavior Graph baseado na categoria do NPC.
        /// </summary>
        private void LoadBehaviorGraph()
        {
            if (behaviorGraphPrefab == null)
            {
                if (enableLogs)
                {
                    Debug.Log($"[NPCController] {npcName} não possui Behavior Graph configurado");
                }
                return;
            }

            // Instanciar o Behavior Graph como filho do NPC
            behaviorGraphInstance = Instantiate(behaviorGraphPrefab, transform);
            behaviorGraphInstance.name = "BehaviorGraph";

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} Behavior Graph carregado");
            }

            // TODO: Integrar com BehaviorGraphAgent da Unity 6.2 quando disponível
            // BehaviorGraphAgent agent = behaviorGraphInstance.GetComponent<BehaviorGraphAgent>();
            // if (agent != null)
            // {
            //     InitializeBehaviorGraphVariables(agent);
            // }
        }

        /// <summary>
        /// Inicializa variáveis expostas para o Behavior Graph.
        /// </summary>
        private void InitializeBehaviorGraphVariables()
        {
            // TODO: Expor variáveis para o Behavior Graph quando disponível
            // Variáveis a expor:
            // - currentState (NPCState)
            // - isAlive (bool)
            // - canMove (bool)
            // - canAttack (bool)
            // - currentHealth (int)
            // - relationshipPoints (int)
            // - hasTarget (bool)
            // - distanceToTarget (float)
            // - targetInAttackRange (bool)
            // - currentVelocity (Vector2)
            // - moveDirection (Vector2)
        }

        /// <summary>
        /// Callback chamado quando o Behavior Graph entra em um estado.
        /// </summary>
        public void OnStateEnter(string stateName)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} Behavior Graph entrou no estado: {stateName}");
            }

            // Processar mudanças baseadas no estado do Behavior Graph
            OnBehaviorGraphStateChanged(stateName);
        }

        /// <summary>
        /// Callback chamado quando o Behavior Graph sai de um estado.
        /// </summary>
        public void OnStateExit(string stateName)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} Behavior Graph saiu do estado: {stateName}");
            }
        }

        /// <summary>
        /// Callback chamado quando o Behavior Graph faz uma transição.
        /// </summary>
        public void OnTransition(string fromState, string toState)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} Behavior Graph transição: {fromState} -> {toState}");
            }
        }

        /// <summary>
        /// Envia evento de alvo detectado para o Behavior Graph.
        /// </summary>
        public void OnTargetDetected(Transform target)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} enviando evento TargetDetected para Behavior Graph");
            }

            // TODO: Enviar evento para Behavior Graph quando disponível
            // behaviorGraphAgent.SendEvent("TargetDetected", target);
        }

        /// <summary>
        /// Envia evento de alvo perdido para o Behavior Graph.
        /// </summary>
        public void OnTargetLost()
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} enviando evento TargetLost para Behavior Graph");
            }

            // TODO: Enviar evento para Behavior Graph quando disponível
            // behaviorGraphAgent.SendEvent("TargetLost");
        }

        /// <summary>
        /// Envia evento de vida baixa para o Behavior Graph.
        /// </summary>
        public void OnHealthLow(float percentage)
        {
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} enviando evento HealthLow ({percentage}%) para Behavior Graph");
            }

            // TODO: Enviar evento para Behavior Graph quando disponível
            // behaviorGraphAgent.SendEvent("HealthLow", percentage);
        }

        /// <summary>
        /// Sincroniza o estado do NPC com o estado do Behavior Graph.
        /// </summary>
        public void OnBehaviorGraphStateChanged(string stateName)
        {
            // Mapear estados do Behavior Graph para NPCState
            switch (stateName.ToLower())
            {
                case "idle":
                    currentState = NPCState.Idle;
                    break;

                case "patrolling":
                case "patrol":
                    currentState = NPCState.Patrolling;
                    break;

                case "chasing":
                case "chase":
                    currentState = NPCState.Chasing;
                    break;

                case "attacking":
                case "attack":
                    currentState = NPCState.Attacking;
                    break;

                case "interacting":
                case "interact":
                    currentState = NPCState.Interacting;
                    break;

                case "dead":
                case "death":
                    currentState = NPCState.Dead;
                    break;

                default:
                    if (enableLogs)
                    {
                        Debug.LogWarning($"[NPCController] {npcName} estado desconhecido do Behavior Graph: {stateName}");
                    }
                    break;
            }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} estado sincronizado: {currentState}");
            }
        }

        #region Behavior Graph Exposed Properties

        /// <summary>
        /// Propriedades expostas para o Behavior Graph.
        /// Estas propriedades podem ser lidas pelo Behavior Graph para tomar decisões.
        /// </summary>

        public bool IsAliveProperty => IsAlive();
        public bool CanMove => IsAlive() && currentState != NPCState.Dead && currentState != NPCState.Interacting;
        public bool CanAttack => IsAlive() && currentState != NPCState.Dead && currentState != NPCState.Interacting;
        public int CurrentHealthProperty => attributesHandler != null ? attributesHandler.CurrentHealth : 0;
        public int RelationshipPointsProperty => attributesHandler != null ? attributesHandler.RelationshipPoints : 0;
        public bool HasTarget => currentTarget != null;
        public float DistanceToTarget => currentTarget != null ? Vector2.Distance(transform.position, currentTarget.position) : float.MaxValue;
        public bool TargetInAttackRange => currentTarget != null && IsTargetInRange(currentTarget, attackRange);
        public Vector2 CurrentVelocityProperty => currentVelocity;
        public Vector2 MoveDirection => currentVelocity.normalized;

        #endregion

        #endregion

        #region Debug Gizmos

        private void OnDrawGizmos()
        {
            if (!enableDebugGizmos)
            {
                return;
            }

            Vector3 position = transform.position;

            // Desenhar detection range (amarelo)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, detectionRange);

            // Desenhar attack range (vermelho)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, attackRange);

            // Desenhar interaction range (verde)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, interactionRange);

            // Desenhar patrol points (verde com linhas conectando)
            if (movementPattern == MovementPattern.PatrolPoints && patrolPoints != null && patrolPoints.Length > 0)
            {
                Gizmos.color = Color.green;
                
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] == null) continue;

                    // Desenhar esfera no waypoint
                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);

                    // Desenhar linha para o próximo waypoint
                    int nextIndex = (i + 1) % patrolPoints.Length;
                    if (patrolPoints[nextIndex] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                    }

                    // Destacar waypoint atual
                    if (i == currentPatrolIndex)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(patrolPoints[i].position, 0.5f);
                        Gizmos.color = Color.green;
                    }
                }
            }

            // Desenhar circular patrol (azul)
            if (movementPattern == MovementPattern.CircularPatrol)
            {
                Gizmos.color = Color.blue;
                DrawCircle(patrolCenter, patrolRadius, 32);
            }

            // Desenhar linha para o alvo atual (vermelho)
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, currentTarget.position);
                
                // Desenhar esfera no alvo
                Gizmos.DrawWireSphere(currentTarget.position, 0.5f);
            }

#if UNITY_EDITOR
            // Exibir informações textuais
            Vector3 labelPosition = position + Vector3.up * 2f;
            string info = $"{npcName}\n" +
                         $"State: {currentState}\n" +
                         $"Category: {npcCategory}";

            if (attributesHandler != null)
            {
                info += $"\nHP: {attributesHandler.CurrentHealth}/{attributesHandler.MaxHealth}\n" +
                       $"REL: {attributesHandler.RelationshipPoints}";
            }

            UnityEditor.Handles.Label(labelPosition, info);
#endif
        }

        /// <summary>
        /// Desenha um círculo usando Gizmos.
        /// </summary>
        private void DrawCircle(Vector2 center, float radius, int segments)
        {
            float angleStep = 360f / segments;
            Vector3 prevPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

            for (int i = 1; i <= segments; i++)
            {
                float angle = Mathf.Deg2Rad * (angleStep * i);
                Vector3 newPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                Gizmos.DrawLine(prevPoint, newPoint);
                prevPoint = newPoint;
            }
        }

        #endregion

        #region Game Systems Integration

        /// <summary>
        /// Registra o NPC no GameManager.
        /// </summary>
        private void RegisterWithGameManager()
        {
            // TODO: Implementar registro quando GameManager tiver método RegisterNPC
            // if (GameManager.HasInstance)
            // {
            //     GameManager.Instance.RegisterNPC(this);
            // }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} registrado no GameManager (placeholder)");
            }
        }

        /// <summary>
        /// Desregistra o NPC do GameManager.
        /// </summary>
        private void UnregisterFromGameManager()
        {
            // TODO: Implementar desregistro quando GameManager tiver método UnregisterNPC
            // if (GameManager.HasInstance)
            // {
            //     GameManager.Instance.UnregisterNPC(this);
            // }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} desregistrado do GameManager (placeholder)");
            }
        }

        /// <summary>
        /// Reproduz um som usando AudioManager se disponível.
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            // TODO: Integrar com AudioManager quando disponível
            // if (AudioManager.HasInstance)
            // {
            //     AudioManager.Instance.PlaySFX(clip);
            // }

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {npcName} reproduzindo som: {clip.name} (placeholder)");
            }
        }

        /// <summary>
        /// Reproduz um som aleatório de uma lista.
        /// </summary>
        private void PlayRandomSound(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return;

            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            PlaySound(randomClip);
        }

        #endregion
    }
}
