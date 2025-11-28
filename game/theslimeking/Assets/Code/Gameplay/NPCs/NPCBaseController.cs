using UnityEngine;
using System.Linq;
using SlimeMec.Gameplay;

namespace TheSlimeKing.NPCs
{
    /// <summary>
    /// Controller base para todos os NPCs.
    /// Gerencia comportamentos comuns como movimento, detecção de ataque e integração com NPCAttributesHandler.
    /// </summary>
    [RequireComponent(typeof(NPCAttributesHandler))]
    public class NPCBaseController : MonoBehaviour
    {
        /// <summary>
        /// Retorna o valor de ataque atual do NPC (usado por hitboxes de ataque).
        /// </summary>
        public int GetAttackValue()
        {
            return attributesHandler != null ? attributesHandler.CurrentAttack : 1;
        }

        #region Inspector Variables
        [Header("Knockback")]
        [SerializeField] protected float knockbackDuration = 0.3f;

        [Header("Player Detection")]
        [SerializeField] protected float detectionRange = 3.0f;
        [SerializeField] protected bool enablePlayerDetection = true;
        [SerializeField] protected GameObject vfxDetectionAlert;
        [SerializeField] protected Vector3 vfxOffset = Vector3.up;
        [SerializeField] protected float responseTime = 1f;

        [Header("Attack System")]
        [SerializeField] protected float attackRange = 0.8f; // Distância para atacar
        [SerializeField] protected float attackCooldown = 2.0f; // Tempo entre ataques
        [SerializeField] protected bool enableAttack = true;

        [Header("Attack Hitbox Timing")]
        [Tooltip("Delay em segundos para ativar o hitbox após o início do ataque.")]
        [SerializeField] protected float attackHitboxDelay = 1.0f;

        [Header("Idle Movement")]
        [SerializeField] protected bool enableIdleBouncing = true;
        [SerializeField] protected float bouncingRange = 1.5f; // Distância horizontal do bouncing
        [SerializeField] protected float bouncingSpeed = 1.0f; // Velocidade do movimento horizontal
        [SerializeField] protected float verticalAmplitude = 0.3f; // Altura do movimento vertical
        [SerializeField] protected float verticalFrequency = 2.0f; // Frequência do movimento vertical

        [Header("Debug")]
        [SerializeField] protected bool enableDebugLogs = false;
        [SerializeField] protected bool enableDebugGizmos = true;
        #endregion

        #region Protected Variables
        protected NPCAttributesHandler attributesHandler;
        protected Rigidbody2D rb;
        protected Animator animator;
        protected Transform playerTransform;
        protected SpriteRenderer[] spriteRenderers;
        protected bool facingRight = true;

        // Knockback system
        protected bool isInKnockback = false;
        protected float knockbackEndTime;

        // Player detection system
        protected bool playerInRange = false;
        protected float lastDetectionTime = 0f;
        protected float detectionCooldown = 1f; // Log apenas a cada segundo para evitar spam
        protected GameObject currentVfxInstance;

        // Movement and chase system
        protected bool isChasing = false;
        protected Vector2 targetPosition;
        protected Vector2 moveDirection;

        // Idle bouncing system
        protected Vector3 initialPosition;
        protected bool movingRight = true;
        protected float bouncingTime = 0f;
        protected bool isIdleBouncing = false;

        // Attack system
        protected bool isInAttackRange = false;
        protected float lastAttackTime = 0f;
        protected bool isAttacking = false;

        // Animator parameters cache
        private static readonly int HitTrigger = Animator.StringToHash("Hit");
        private static readonly int IsDying = Animator.StringToHash("isDying");
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            // Cache de componentes
            attributesHandler = GetComponent<NPCAttributesHandler>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            // Pega todos os SpriteRenderers do objeto e filhos (inclusive desativados)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);


        }

        protected virtual void Start()
        {
            // Encontra referência do player
            FindPlayer();

            // Armazena posição inicial para bouncing
            initialPosition = transform.position;

            if (enableDebugLogs)
            {
                Log($"NPC inicializado");
            }
        }

        protected virtual void Update()
        {
            if (attributesHandler.IsDead)
            {
                return;
            }

            UpdateKnockback();
            UpdatePlayerDetection();
            UpdateState();
            UpdateMovement();
            UpdateAnimations();
            UpdateSpriteFlip();
        }
        /// <summary>
        /// Atualiza o flip do sprite conforme a posição do player.
        /// </summary>
        protected virtual void UpdateSpriteFlip()
        {
            if (spriteRenderers == null || playerTransform == null) return;

            // Flipa se estiver perseguindo, atacando ou em idle/bouncing
            if (isChasing || isIdleBouncing || isAttacking)
            {
                float playerX = playerTransform.position.x;
                float npcX = transform.position.x;
                bool shouldFaceRight = playerX > npcX;
                if (shouldFaceRight != facingRight)
                {
                    // Aplica flipX em todos os SpriteRenderers filhos
                    foreach (var sr in spriteRenderers)
                    {
                        sr.flipX = !shouldFaceRight;
                    }
                    facingRight = shouldFaceRight;
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (attributesHandler.IsDead) return;

            PerformMovement();
        }
        #endregion

        #region Combat Methods
        /// <summary>
        /// Método chamado quando o NPC recebe dano por ataque do player.
        /// </summary>
        public virtual void TakeDamage()
        {
            if (attributesHandler.IsDead) return;

            // Obtém valor de ataque do player
            int playerAttack = GetPlayerAttack();
            int damage = CalculateDamage(playerAttack, attributesHandler.CurrentDefense);

            if (damage > 0)
            {
                bool died = attributesHandler.TakeDamage(damage);

                // Log apenas quando efetivamente atingido
                if (enableDebugLogs)
                {
                    Log($"Recebeu {damage} de dano! Vida restante: {attributesHandler.CurrentHealthPoints}");
                }

                OnTakeDamage(damage, died);
            }
            else
            {
                OnAttackBlocked();
            }
        }
        #endregion

        #region Protected Virtual Methods - Override em classes filhas
        /// <summary>
        /// Atualiza o estado atual do NPC baseado na situação.
        /// </summary>
        protected virtual void UpdateState()
        {
            if (playerInRange && playerTransform != null)
            {
                // Inicia perseguição se player está no range
                if (!isChasing)
                {
                    StartChasing();
                }

                // Atualiza posição alvo para a posição atual do player
                targetPosition = playerTransform.position;

                // Verifica se está no range de ataque
                UpdateAttackRange();
            }
            else
            {
                // Para perseguição se player saiu do range
                if (isChasing)
                {
                    StopChasing();
                }

                // Para ataque se player saiu do range
                if (isInAttackRange)
                {
                    isInAttackRange = false;
                }

                // Inicia movimento Idle se não está perseguindo
                if (!isChasing && enableIdleBouncing)
                {
                    if (!isIdleBouncing)
                    {
                        StartIdleBouncing();
                    }
                }
            }
        }

        /// <summary>
        /// Atualiza movimento baseado no estado atual.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            // Não se move se estiver atacando
            if (isAttacking)
            {
                moveDirection = Vector2.zero;
                return;
            }

            if (isChasing && playerTransform != null)
            {
                // Para de se mover se estiver no range de ataque
                if (isInAttackRange)
                {
                    moveDirection = Vector2.zero;
                    return;
                }

                // Calcula direção para o player
                Vector2 currentPosition = transform.position;
                moveDirection = (targetPosition - currentPosition).normalized;

                // Verifica se chegou próximo o suficiente do player (para evitar vibração)
                float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
                if (distanceToTarget < 0.1f)
                {
                    moveDirection = Vector2.zero;
                }
            }
            else if (isIdleBouncing)
            {
                // Calcula movimento de bouncing
                UpdateBouncingMovement();
            }
            else
            {
                moveDirection = Vector2.zero;
            }
        }

        /// <summary>
        /// Aplica movimento físico.
        /// </summary>
        protected virtual void PerformMovement()
        {
            if (rb == null || isInKnockback) return;

            // Aplica movimento se estiver perseguindo
            if (isChasing && moveDirection != Vector2.zero)
            {
                float currentSpeed = attributesHandler.CurrentSpeed;
                Vector2 velocity = moveDirection * currentSpeed;
                rb.linearVelocity = velocity;

                if (enableDebugLogs)
                {
                    Log($"Movimento aplicado - Direção: {moveDirection}, Velocidade: {currentSpeed}");
                }
            }
            else if (isIdleBouncing)
            {
                // Aplica movimento de bouncing
                ApplyBouncingMovement();
            }
            else
            {
                // Para o movimento se não está perseguindo nem fazendo bouncing
                rb.linearVelocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Atualiza animações baseado no estado.
        /// </summary>
        protected virtual void UpdateAnimations()
        {
            if (animator == null) return;

            // Controla parâmetro isWalking baseado no estado de perseguição (mas não durante ataque)
            bool shouldWalk = isChasing && moveDirection != Vector2.zero && !isAttacking;
            animator.SetBool(IsWalking, shouldWalk);

            if (enableDebugLogs && shouldWalk)
            {
                Log($"Animação isWalking = {shouldWalk}");
            }
        }

        /// <summary>
        /// Chamado quando o NPC recebe dano.
        /// </summary>
        protected virtual void OnTakeDamage(int damage, bool died)
        {
            if (enableDebugLogs)
            {
                Log($"Recebeu {damage} de dano. Vida restante: {attributesHandler.CurrentHealthPoints}");
            }

            // Aciona animação de Hit
            if (animator != null)
            {
                // Configura isDying baseado no status de morte
                animator.SetBool(IsDying, died);

                // Aciona trigger Hit
                animator.SetTrigger(HitTrigger);

                if (enableDebugLogs)
                {
                    Log($"Animator configurado - Hit: acionado, isDying: {died}");
                }
            }

            // Aplica knockback se não morreu
            if (!died)
            {
                ApplyKnockback(damage);
            }
        }

        /// <summary>
        /// Chamado quando o ataque é bloqueado pela defesa.
        /// </summary>
        protected virtual void OnAttackBlocked()
        {
            if (enableDebugLogs)
            {
                Log("Ataque bloqueado pela defesa - aplicando knockback reverso no player");
            }

            // Aplica knockback reverso no player
            ApplyPlayerKnockback();
        }
        #endregion

        #region Protected Helper Methods


        protected void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                LogWarning("Player não encontrado! Verifique se o player tem a tag 'Player'");
            }
        }

        protected int GetPlayerAttack()
        {
            if (playerTransform == null) return 1;

            var playerAttributes = playerTransform.GetComponent<PlayerAttributesHandler>();
            if (playerAttributes != null)
            {
                return playerAttributes.CurrentAttack;
            }

            // Fallback
            return 1;
        }

        protected int CalculateDamage(int attackPower, int defense)
        {
            return Mathf.Max(0, attackPower - defense);
        }

        /// <summary>
        /// Aplica knockback reverso no player quando ataque é bloqueado.
        /// </summary>
        protected virtual void ApplyPlayerKnockback()
        {
            if (playerTransform == null) return;

            // Busca o PlayerController
            var playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Aplica knockback usando a posição deste NPC como origem
                playerController.ApplyKnockback(transform.position);

                if (enableDebugLogs)
                {
                    Log($"Knockback reverso aplicado no player - Origem: {transform.position}");
                }
            }
            else
            {
                LogWarning("PlayerController não encontrado para aplicar knockback reverso!");
            }
        }

        /// <summary>
        /// Aplica efeito de knockback no NPC baseado no dano recebido.
        /// </summary>
        protected virtual void ApplyKnockback(int damageValue)
        {
            if (rb == null || playerTransform == null) return;

            // Calcula força do knockback baseada no dano (ataque do slime - defesa do NPC)
            float knockbackForce = Mathf.Max(0f, damageValue);

            // Se a força é zero, não aplica knockback
            if (knockbackForce <= 0f) return;

            // Calcula direção do knockback (oposta ao player)
            Vector2 knockbackDirection = (transform.position - playerTransform.position).normalized;

            // Aplica força de knockback
            rb.linearVelocity = knockbackDirection * knockbackForce;

            // Define estado de knockback
            isInKnockback = true;
            knockbackEndTime = Time.time + knockbackDuration;

            if (enableDebugLogs)
            {
                Log($"Knockback aplicado - Direção: {knockbackDirection}, Força: {knockbackForce} (baseada no dano)");
            }
        }

        /// <summary>
        /// Atualiza sistema de knockback.
        /// </summary>
        protected virtual void UpdateKnockback()
        {
            if (!isInKnockback) return;

            // Verifica se o knockback terminou
            if (Time.time >= knockbackEndTime)
            {
                isInKnockback = false;

                // Para o movimento imediatamente quando o knockback termina
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }

                if (enableDebugLogs)
                {
                    Log("Knockback finalizado");
                }
            }
            else
            {
                // Durante o knockback, reduz gradualmente a velocidade
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 2f);
                }
            }
        }

        /// <summary>
        /// Atualiza sistema de detecção do player.
        /// </summary>
        protected virtual void UpdatePlayerDetection()
        {
            if (!enablePlayerDetection || playerTransform == null) return;

            // Calcula distância para o player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool playerCurrentlyInRange = distanceToPlayer <= detectionRange;

            // Detecta mudança de estado (entrou no range)
            if (playerCurrentlyInRange && !playerInRange)
            {
                OnPlayerEnterRange();
            }
            // Detecta mudança de estado (saiu do range)
            else if (!playerCurrentlyInRange && playerInRange)
            {
                OnPlayerExitRange();
            }

            // Atualiza estado
            playerInRange = playerCurrentlyInRange;

            // Log periódico quando player está no range (evita spam)
            if (playerInRange && enableDebugLogs && (Time.time - lastDetectionTime) >= detectionCooldown)
            {
                Log($"Player detectado no range - Distância: {distanceToPlayer:F2}m");
                lastDetectionTime = Time.time;
            }
        }

        /// <summary>
        /// Chamado quando o player entra no range de detecção.
        /// </summary>
        protected virtual void OnPlayerEnterRange()
        {
            if (enableDebugLogs)
            {
                Log("Player ENTROU no range de detecção!");
            }

            // Instancia VFX de alerta se configurado
            if (vfxDetectionAlert != null && currentVfxInstance == null)
            {
                Vector3 vfxPosition = transform.position + vfxOffset;
                currentVfxInstance = Instantiate(vfxDetectionAlert, vfxPosition, Quaternion.identity, transform);

                if (enableDebugLogs)
                {
                    Log($"VFX de detecção instanciado na posição: {vfxPosition}");
                }
            }
        }

        /// <summary>
        /// Chamado quando o player sai do range de detecção.
        /// </summary>
        protected virtual void OnPlayerExitRange()
        {
            if (enableDebugLogs)
            {
                Log("Player SAIU do range de detecção!");
            }

            // Destrói VFX de alerta se existir
            if (currentVfxInstance != null)
            {
                Destroy(currentVfxInstance);
                currentVfxInstance = null;

                if (enableDebugLogs)
                {
                    Log("VFX de detecção destruído");
                }
            }

            // Para perseguição quando perde o player de vista
            if (isChasing)
            {
                StopChasing();
            }
        }

        protected void Log(string message)
        {
            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[{GetType().Name}] {gameObject.name} - {message}");
            }
        }

        protected void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[{GetType().Name}] {gameObject.name} - {message}");
        }

        /// <summary>
        /// Destrói o objeto do NPC.
        /// </summary>
        public virtual void DestroyNPC()
        {
            if (enableDebugLogs)
            {
                Log("Destruindo NPC");
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Inicia perseguição do player.
        /// </summary>
        protected virtual void StartChasing()
        {
            isChasing = true;

            // Para movimento de bouncing quando começa a perseguir
            if (isIdleBouncing)
            {
                StopIdleBouncing();
            }

            if (enableDebugLogs)
            {
                Log("Iniciou perseguição do player");
            }
        }

        /// <summary>
        /// Para perseguição do player.
        /// </summary>
        protected virtual void StopChasing()
        {
            isChasing = false;
            moveDirection = Vector2.zero;

            // Para movimento imediatamente
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (enableDebugLogs)
            {
                Log("Parou perseguição do player");
            }
        }

        /// <summary>
        /// Inicia movimento de bouncing no estado Idle.
        /// </summary>
        protected virtual void StartIdleBouncing()
        {
            isIdleBouncing = true;
            bouncingTime = 0f;

            if (enableDebugLogs)
            {
                Log("Iniciou movimento de bouncing Idle");
            }
        }

        /// <summary>
        /// Para movimento de bouncing.
        /// </summary>
        protected virtual void StopIdleBouncing()
        {
            isIdleBouncing = false;
            bouncingTime = 0f;

            if (enableDebugLogs)
            {
                Log("Parou movimento de bouncing Idle");
            }
        }

        /// <summary>
        /// Atualiza cálculos do movimento de bouncing.
        /// </summary>
        protected virtual void UpdateBouncingMovement()
        {
            bouncingTime += Time.deltaTime;

            // Calcula posição horizontal (movimento de ida e volta)
            float horizontalOffset = Mathf.Sin(bouncingTime * bouncingSpeed) * bouncingRange;

            // Calcula posição vertical (movimento de sobe e desce)
            float verticalOffset = Mathf.Abs(Mathf.Sin(bouncingTime * verticalFrequency)) * verticalAmplitude;

            // Calcula posição alvo
            Vector3 targetPos = new Vector3(
                initialPosition.x + horizontalOffset,
                initialPosition.y + verticalOffset,
                initialPosition.z
            );

            // Calcula direção do movimento
            Vector2 currentPosition = transform.position;
            moveDirection = (Vector2)(targetPos - transform.position).normalized;
        }

        /// <summary>
        /// Aplica movimento físico do bouncing.
        /// </summary>
        protected virtual void ApplyBouncingMovement()
        {
            if (rb == null) return;

            // Aplica velocidade mais suave para o bouncing
            float bouncingVelocity = attributesHandler.CurrentSpeed * 0.5f; // Metade da velocidade normal
            Vector2 velocity = moveDirection * bouncingVelocity;
            rb.linearVelocity = velocity;

            if (enableDebugLogs && bouncingTime % 2f < 0.1f) // Log a cada 2 segundos aproximadamente
            {
                Log($"Bouncing - Direção: {moveDirection}, Velocidade: {bouncingVelocity}");
            }
        }

        /// <summary>
        /// Atualiza range de ataque e executa ataque se possível.
        /// </summary>
        protected virtual void UpdateAttackRange()
        {
            if (!enableAttack || playerTransform == null) return;

            // Calcula distância para o player
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            bool playerInAttackRange = distanceToPlayer <= attackRange;

            // Detecta mudança de estado do range de ataque
            if (playerInAttackRange && !isInAttackRange)
            {
                isInAttackRange = true;
                OnPlayerEnterAttackRange();
            }
            else if (!playerInAttackRange && isInAttackRange)
            {
                isInAttackRange = false;
                OnPlayerExitAttackRange();
            }

            // Executa ataque se estiver no range e cooldown passou
            if (isInAttackRange && !isAttacking && CanAttack())
            {
                PerformAttack();
            }
        }

        /// <summary>
        /// Verifica se pode atacar (cooldown).
        /// </summary>
        protected virtual bool CanAttack()
        {
            return Time.time - lastAttackTime >= attackCooldown;
        }

        /// <summary>
        /// Executa ataque ativando a trigger Attack.
        /// </summary>
        protected virtual void PerformAttack()
        {
            if (animator == null) return;

            isAttacking = true;
            lastAttackTime = Time.time;

            // Ativa trigger de ataque
            animator.SetTrigger(AttackTrigger);

            if (enableDebugLogs)
            {
                Log("Executando ataque - trigger Attack ativada");
            }

            // Agora o dano será aplicado com delay na corrotina EndAttackAfterDelay

            // Agenda o fim do ataque (baseado na duração da animação)
            // Nota: Em uma implementação mais robusta, isso seria controlado por eventos de animação
            StartCoroutine(EndAttackAfterDelay());
        }

        /// <summary>
        /// Corrotina para finalizar ataque após delay.
        /// </summary>
        protected virtual System.Collections.IEnumerator EndAttackAfterDelay()
        {
            // Aguarda o delay configurado para simular o tempo do hitbox
            if (attackHitboxDelay > 0f)
                yield return new WaitForSeconds(attackHitboxDelay);

            // Após o delay, verifica se o player ainda está no alcance e aplica dano
            if (playerTransform != null)
            {
                float distance = Vector2.Distance(transform.position, playerTransform.position);
                if (distance <= attackRange)
                {
                    var playerAttributes = playerTransform.GetComponent<PlayerAttributesHandler>();
                    if (playerAttributes != null)
                    {
                        playerAttributes.TakeDamage(GetAttackValue());
                    }
                }
            }

            // Aguarda o restante da animação (duração total = 1s padrão)
            float totalAttackDuration = 1.0f;
            float remaining = Mathf.Max(0f, totalAttackDuration - attackHitboxDelay);
            if (remaining > 0f)
                yield return new WaitForSeconds(remaining);

            isAttacking = false;

            if (enableDebugLogs)
            {
                Log($"Ataque finalizado. Dano aplicado após {attackHitboxDelay}s.");
            }
        }

        /// <summary>
        /// Chamado quando player entra no range de ataque.
        /// </summary>
        protected virtual void OnPlayerEnterAttackRange()
        {
            if (enableDebugLogs)
            {
                Log("Player ENTROU no range de ataque!");
            }
        }

        /// <summary>
        /// Chamado quando player sai do range de ataque.
        /// </summary>
        protected virtual void OnPlayerExitAttackRange()
        {
            if (enableDebugLogs)
            {
                Log("Player SAIU do range de ataque!");
            }
        }
        #endregion

        #region Gizmos
        protected virtual void OnDrawGizmosSelected()
        {
            if (!enableDebugGizmos) return;

            // Desenha range de detecção do player
            if (enablePlayerDetection)
            {
                Gizmos.color = playerInRange ? Color.red : Color.yellow;
                Gizmos.DrawWireSphere(transform.position, detectionRange);

                // Desenha linha para o player se estiver no range
                if (playerInRange && playerTransform != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(transform.position, playerTransform.position);
                }
            }

            // Desenha range de ataque
            if (enableAttack)
            {
                Gizmos.color = isInAttackRange ? Color.red : Color.orange;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }

            // Desenha range de bouncing se habilitado
            if (enableIdleBouncing)
            {
                Vector3 basePos = Application.isPlaying ? initialPosition : transform.position;

                Gizmos.color = Color.cyan;
                // Linha horizontal mostrando range de bouncing
                Vector3 leftPoint = new Vector3(basePos.x - bouncingRange, basePos.y, basePos.z);
                Vector3 rightPoint = new Vector3(basePos.x + bouncingRange, basePos.y, basePos.z);
                Gizmos.DrawLine(leftPoint, rightPoint);

                // Linha vertical mostrando amplitude
                Vector3 bottomPoint = new Vector3(basePos.x, basePos.y, basePos.z);
                Vector3 topPoint = new Vector3(basePos.x, basePos.y + verticalAmplitude, basePos.z);
                Gizmos.DrawLine(bottomPoint, topPoint);

                // Marca posição inicial
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(basePos, Vector3.one * 0.1f);
            }

            // TODO: Implementar outros gizmos de debug
            // TODO: Desenhar range de ataque (círculo vermelho)  
            // TODO: Desenhar range de patrulha (círculo verde)
            // TODO: Mostrar posição alvo atual (cubo azul)
        }

        protected virtual void OnDestroy()
        {
            // Limpa VFX de detecção se existir
            if (currentVfxInstance != null)
            {
                Destroy(currentVfxInstance);
                currentVfxInstance = null;
            }

            UnityEngine.Debug.LogError($"[DESTRUIÇÃO DETECTADA] {gameObject.name} está sendo destruído!");
            UnityEngine.Debug.LogError($"StackTrace: {System.Environment.StackTrace}");
        }

        protected virtual void OnDisable()
        {
            UnityEngine.Debug.LogError($"[DESATIVAÇÃO DETECTADA] {gameObject.name} está sendo desativado!");
            UnityEngine.Debug.LogError($"StackTrace: {System.Environment.StackTrace}");
        }
        #endregion
    }
}