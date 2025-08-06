using UnityEngine;
using System.Collections;

/// <summary>
/// Controlador de comportamento para criaturas inimigas
/// Utiliza exclusivamente EntityStatus para stats e configurações
/// 
/// ESTRUTURA DE SPRITES REQUERIDA:
/// GameObject Principal (com este script)
///   ├── side (sprites laterais - leste/oeste)
///   ├── back (sprites traseiros - norte)
///   ├── front (sprites frontais - sul)
///   ├── vfx_side (efeitos visuais laterais)
///   ├── vfx_back (efeitos visuais traseiros)
///   └── vfx_front (efeitos visuais frontais)
/// 
/// ANIMATOR PARÂMETROS:
/// - isWalking (bool): True quando o inimigo está se movendo
/// - isAttacking (bool): True quando o inimigo está atacando
/// 
/// DETECÇÃO DE PLAYER:
/// - Só detecta objetos com a tag "Player"
/// - Usa Physics2D.OverlapCircle com playerLayer
/// - Validação dupla: layer + tag para máxima precisão
/// </summary>
[RequireComponent(typeof(EntityStatus))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBehaviorController : MonoBehaviour
{
    #region Enums
    /// <summary>
    /// Estados possíveis do inimigo
    /// </summary>
    public enum EnemyState
    {
        Idle = 0,
        Patrol = 1,
        Chase = 2,
        Attack = 3,
        Hit = 4,
        Dead = 5
    }

    /// <summary>
    /// Tipos de comportamento do inimigo
    /// </summary>
    public enum BehaviorType
    {
        Passive = 0,    // Não ataca, apenas foge
        Defensive = 1,  // Ataca apenas quando atacado
        Aggressive = 2, // Persegue e ataca o player
        Guardian = 3    // Protege uma área específica
    }

    /// <summary>
    /// Direções possíveis para o inimigo
    /// </summary>
    private enum FacingDirection
    {
        North,
        South,
        East,
        West
    }
    #endregion

    #region Configurações de Comportamento
    [Header("🧠 Configurações de Comportamento")]
    [Tooltip("Tipo de comportamento do inimigo")]
    [SerializeField] private BehaviorType behaviorType = BehaviorType.Aggressive;

    [Tooltip("Estado inicial do inimigo")]
    [SerializeField] private EnemyState initialState = EnemyState.Idle;
    #endregion

    #region Configurações de Detecção
    [Header("👁️ Sistema de Detecção")]
    [Tooltip("Raio de detecção do player")]
    [SerializeField, Range(1f, 20f)] private float detectionRadius = 5f;

    [Tooltip("Raio de perda do target")]
    [SerializeField, Range(1f, 25f)] private float loseTargetRadius = 7f;

    [Tooltip("Layer do player")]
    [SerializeField] private LayerMask playerLayer = -1;

    [Tooltip("Layers que bloqueiam visão")]
    [SerializeField] private LayerMask obstacleLayer = -1;

    [Tooltip("Requer linha de visão para detectar")]
    [SerializeField] private bool requireLineOfSight = false;

    [Tooltip("Tempo para esquecer o player após perder de vista")]
    [SerializeField, Range(0.5f, 10f)] private float forgetTime = 3f;
    #endregion

    #region Configurações de Movimento
    [Header("🏃 Sistema de Movimento")]
    [Tooltip("Multiplicador de velocidade durante perseguição")]
    [SerializeField, Range(1f, 3f)] private float chaseSpeedMultiplier = 1.5f;

    [Tooltip("Multiplicador de velocidade durante patrulha")]
    [SerializeField, Range(0.1f, 1f)] private float patrolSpeedMultiplier = 0.6f;

    [Tooltip("Distância para parar próximo ao target")]
    [SerializeField, Range(0.1f, 3f)] private float stoppingDistance = 0.8f;

    [Tooltip("Suavização do movimento")]
    [SerializeField, Range(0.1f, 1f)] private float movementSmoothing = 0.7f;
    #endregion

    #region Configurações de Patrulha
    [Header("🚶 Sistema de Patrulha")]
    [Tooltip("Ativar patrulhamento quando idle")]
    [SerializeField] private bool enablePatrol = true;

    [Tooltip("Raio máximo de patrulha da posição inicial")]
    [SerializeField, Range(1f, 15f)] private float patrolRadius = 5f;

    [Tooltip("Tempo de espera em cada ponto de patrulha")]
    [SerializeField, Range(0.5f, 10f)] private float patrolWaitTime = 2f;

    [Tooltip("Número de pontos de patrulha")]
    [SerializeField, Range(2, 8)] private int patrolPointsCount = 3;

    [Tooltip("Patrulha aleatória ou sequencial")]
    [SerializeField] private bool randomPatrol = true;

    [Tooltip("Tempo limite para chegar ao destino (0 = desabilitado)")]
    [SerializeField, Range(0f, 30f)] private float patrolTimeout = 10f;
    #endregion

    #region Configurações de Combate
    [Header("⚔️ Sistema de Combate")]
    [Tooltip("Raio de ataque")]
    [SerializeField, Range(0.5f, 5f)] private float attackRange = 1.5f;

    [Tooltip("Força do knockback aplicado ao player")]
    [SerializeField, Range(0f, 20f)] private float knockbackForce = 5f;

    [Tooltip("Pode atacar durante movimento")]
    [SerializeField] private bool canAttackWhileMoving = false;
    #endregion

    #region Configurações de Reação
    [Header("💥 Sistema de Reação")]
    [Tooltip("Duração do estado de hit")]
    [SerializeField, Range(0.1f, 3f)] private float hitDuration = 0.5f;

    [Tooltip("Força do knockback recebido")]
    [SerializeField, Range(0f, 10f)] private float hitKnockbackForce = 3f;

    [Tooltip("Tempo de invencibilidade após hit")]
    [SerializeField, Range(0f, 3f)] private float invulnerabilityTime = 0.2f;

    [Tooltip("Piscar durante invencibilidade")]
    [SerializeField] private bool blinkOnHit = true;

    [Tooltip("Velocidade do piscar")]
    [SerializeField, Range(5f, 30f)] private float blinkSpeed = 15f;
    #endregion

    #region Configurações Avançadas
    [Header("⚙️ Configurações Avançadas")]
    [Tooltip("Retornar à posição inicial quando não há target")]
    [SerializeField] private bool returnToStart = true;

    [Tooltip("Distância máxima da posição inicial")]
    [SerializeField, Range(5f, 50f)] private float maxDistanceFromStart = 20f;
    #endregion

    #region Configurações de Sprites
    [Header("🎨 Referências dos Sprites")]
    [Tooltip("GameObject com o sprite frontal")]
    [SerializeField] private GameObject front;

    [Tooltip("GameObject com os efeitos visuais frontais")]
    [SerializeField] private GameObject vfx_front;

    [Tooltip("GameObject com o sprite traseiro")]
    [SerializeField] private GameObject back;

    [Tooltip("GameObject com os efeitos visuais traseiros")]
    [SerializeField] private GameObject vfx_back;

    [Tooltip("GameObject com o sprite lateral")]
    [SerializeField] private GameObject side;

    [Tooltip("GameObject com os efeitos visuais laterais")]
    [SerializeField] private GameObject vfx_side;
    #endregion

    #region Configurações de Debug
    [Header("🐛 Debug")]
    [Tooltip("Mostrar gizmos de debug")]
    [SerializeField] private bool showDebugGizmos = true;

    [Tooltip("Logs detalhados no console")]
    [SerializeField] private bool enableDetailedLogs = false;

    [Tooltip("Cor dos gizmos de detecção")]
    [SerializeField] private Color detectionColor = Color.yellow;

    [Tooltip("Cor dos gizmos de ataque")]
    [SerializeField] private Color attackColor = Color.red;

    [Tooltip("Cor dos gizmos de patrulha")]
    [SerializeField] private Color patrolColor = Color.blue;
    #endregion

    #region Propriedades Públicas
    /// <summary>
    /// Estado atual do inimigo
    /// </summary>
    public EnemyState CurrentState { get; private set; }

    /// <summary>
    /// Target atual (geralmente o player)
    /// </summary>
    public Transform CurrentTarget { get; private set; }

    /// <summary>
    /// Distância até o target atual
    /// </summary>
    public float DistanceToTarget { get; private set; }

    /// <summary>
    /// Inimigo está vivo? (usa EntityStatus)
    /// </summary>
    public bool IsAlive => entityStatus != null && entityStatus.IsAlive();

    /// <summary>
    /// Tem um target válido?
    /// </summary>
    public bool HasTarget => CurrentTarget != null;

    /// <summary>
    /// Está no range de ataque?
    /// </summary>
    public bool IsInAttackRange => HasTarget && DistanceToTarget <= attackRange;

    /// <summary>
    /// Pode atacar agora? (usa cooldown do EntityStatus)
    /// </summary>
    public bool CanAttack => IsInAttackRange && entityStatus != null && entityStatus.IsBasicAttackAvailable();

    /// <summary>
    /// Está em movimento?
    /// </summary>
    public bool IsMoving { get; private set; }

    /// <summary>
    /// Posição inicial do inimigo
    /// </summary>
    public Vector2 StartPosition { get; private set; }

    /// <summary>
    /// Velocidade atual (usa EntityStatus.GetSpeed())
    /// </summary>
    public float CurrentSpeed => entityStatus != null ? entityStatus.GetSpeed() : 0f;

    /// <summary>
    /// Dano atual (usa EntityStatus.GetAttack())
    /// </summary>
    public int CurrentDamage => entityStatus != null ? entityStatus.GetAttack() : 0;

    /// <summary>
    /// HP atual (usa EntityStatus)
    /// </summary>
    public int CurrentHP => entityStatus != null ? entityStatus.currentHP : 0;

    /// <summary>
    /// HP máximo (usa EntityStatus)
    /// </summary>
    public int MaxHP => entityStatus != null ? entityStatus.GetMaxHP() : 0;
    #endregion

    #region Cache de Componentes
    private EntityStatus entityStatus;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform cachedTransform;

    // Referências para controle de sprites laterais
    private SpriteRenderer sideRenderer;
    private SpriteRenderer vfxSideRenderer;
    #endregion

    #region Variáveis Privadas
    // Controle de estado
    private EnemyState previousState;
    private float stateTimer;
    private float targetLostTime;

    // Movimento e patrulha
    private Vector2[] patrolPoints;
    private int currentPatrolIndex;
    private Vector2 currentDestination;
    private bool isReturningToStart;
    private Vector2 lastDirection = Vector2.down; // Direção padrão
    private FacingDirection lastFacingDirection = FacingDirection.South; // Direção visual padrão

    // Controle de patrulha
    private bool isWaitingAtPatrolPoint;
    private float patrolWaitTimer;
    private bool hasReachedPatrolDestination;
    private float patrolMoveTimer; // Timer para controlar timeout do movimento

    // Detecção
    private bool hasLineOfSight;
    private float lastDetectionTime;

    // Efeitos visuais
    private bool isBlinking;
    private Color originalColor;

    // Cache de WaitForSeconds
    private WaitForSeconds hitWait;
    private WaitForSeconds patrolWait;

    // Constantes
    private const float POSITION_TOLERANCE = 0.1f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
        CacheWaitObjects();
        RegisterEntityEvents();
    }

    private void Start()
    {
        InitializeStartPosition();
        GeneratePatrolPoints();
        SetState(initialState);

        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name} inicializado");
            Debug.Log($"[EnemyBehavior] Stats - HP: {MaxHP}, Attack: {CurrentDamage}, Speed: {CurrentSpeed:F1}");
        }
    }

    private void Update()
    {
        // Só atualiza se estiver vivo
        if (!IsAlive)
        {
            if (CurrentState != EnemyState.Dead)
                SetState(EnemyState.Dead);
            return;
        }

        UpdateBehavior();
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        DrawDebugGizmos();
    }

    private void OnDestroy()
    {
        UnregisterEntityEvents();
    }

    private void OnDisable()
    {
        UnregisterEntityEvents();
    }
    #endregion

    #region Inicialização
    /// <summary>
    /// Inicializa componentes essenciais
    /// </summary>
    private void InitializeComponents()
    {
        cachedTransform = transform;
        entityStatus = GetComponent<EntityStatus>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Validações críticas
        if (entityStatus == null)
        {
            Debug.LogError($"[EnemyBehavior] EntityStatus não encontrado em {name}!");
            enabled = false;
            return;
        }

        if (rb == null)
        {
            Debug.LogError($"[EnemyBehavior] Rigidbody2D não encontrado em {name}!");
            enabled = false;
            return;
        }

        // Inicializa referências dos sprites laterais para controle de flip
        if (side != null) sideRenderer = side.GetComponent<SpriteRenderer>();
        if (vfx_side != null) vfxSideRenderer = vfx_side.GetComponent<SpriteRenderer>();

        // Salva cor original para efeitos
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Configura direção inicial dos sprites
        UpdateSpriteDirection(lastFacingDirection);
    }

    /// <summary>
    /// Cache de WaitForSeconds para performance
    /// </summary>
    private void CacheWaitObjects()
    {
        hitWait = new WaitForSeconds(hitDuration);
        patrolWait = new WaitForSeconds(patrolWaitTime);
    }

    /// <summary>
    /// Registra eventos do EntityStatus
    /// </summary>
    private void RegisterEntityEvents()
    {
        if (entityStatus != null)
        {
            entityStatus.OnDeath += OnEntityDeath;
        }
    }

    /// <summary>
    /// Remove eventos do EntityStatus
    /// </summary>
    private void UnregisterEntityEvents()
    {
        if (entityStatus != null)
        {
            entityStatus.OnDeath -= OnEntityDeath;
        }
    }

    /// <summary>
    /// Define posição inicial
    /// </summary>
    private void InitializeStartPosition()
    {
        StartPosition = cachedTransform.position;
    }

    /// <summary>
    /// Gera pontos de patrulha aleatórios
    /// </summary>
    private void GeneratePatrolPoints()
    {
        if (!enablePatrol) return;

        patrolPoints = new Vector2[patrolPointsCount];

        for (int i = 0; i < patrolPointsCount; i++)
        {
            Vector2 randomPoint = StartPosition + Random.insideUnitCircle * patrolRadius;
            patrolPoints[i] = randomPoint;
        }

        currentPatrolIndex = 0;

        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {patrolPointsCount} pontos de patrulha gerados");
        }
    }

    /// <summary>
    /// Atualiza qual conjunto de sprites deve estar ativo baseado na direção
    /// </summary>
    private void UpdateSpriteDirection(FacingDirection direction)
    {
        switch (direction)
        {
            case FacingDirection.East:
            case FacingDirection.West:
                SetActiveSprites(side, vfx_side);
                if (sideRenderer != null)
                    sideRenderer.flipX = direction == FacingDirection.West;
                break;

            case FacingDirection.North:
                SetActiveSprites(back, vfx_back);
                break;

            case FacingDirection.South:
                SetActiveSprites(front, vfx_front);
                break;
        }

        lastFacingDirection = direction;
    }

    /// <summary>
    /// Ativa os sprites especificados e desativa os outros
    /// </summary>
    private void SetActiveSprites(GameObject mainSprite, GameObject vfxSprite)
    {
        // Desativa todos os sprites
        if (side != null) side.SetActive(false);
        if (back != null) back.SetActive(false);
        if (front != null) front.SetActive(false);
        if (vfx_side != null) vfx_side.SetActive(false);
        if (vfx_back != null) vfx_back.SetActive(false);
        if (vfx_front != null) vfx_front.SetActive(false);

        // Ativa apenas os sprites da direção atual
        if (mainSprite != null) mainSprite.SetActive(true);
        if (vfxSprite != null) vfxSprite.SetActive(true);
    }

    /// <summary>
    /// Determina a direção baseada no movimento
    /// </summary>
    private FacingDirection GetDirectionFromMovement(Vector2 movement)
    {
        if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
        {
            return movement.x > 0 ? FacingDirection.East : FacingDirection.West;
        }
        else
        {
            return movement.y > 0 ? FacingDirection.North : FacingDirection.South;
        }
    }
    #endregion

    #region Eventos EntityStatus
    /// <summary>
    /// Callback quando EntityStatus reporta morte
    /// </summary>
    private void OnEntityDeath()
    {
        SetState(EnemyState.Dead);

        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name} morreu");
        }
    }
    #endregion

    #region Métodos Públicos
    /// <summary>
    /// Define novo tipo de comportamento
    /// </summary>
    public void SetBehaviorType(BehaviorType newBehavior)
    {
        behaviorType = newBehavior;

        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name} mudou comportamento: {newBehavior}");
        }
    }

    /// <summary>
    /// Define novo target
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        // Validação: só aceita targets com tag "Player"
        if (newTarget != null && !newTarget.CompareTag("Player"))
        {
            if (enableDetailedLogs)
            {
                Debug.LogWarning($"[EnemyBehavior] {name} tentou definir target inválido: {newTarget.name} (tag: {newTarget.tag})");
            }
            return;
        }

        CurrentTarget = newTarget;
        targetLostTime = 0f;
        lastDetectionTime = Time.time;

        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name} novo target: {(newTarget ? newTarget.name : "null")}");
        }
    }

    /// <summary>
    /// Remove target atual
    /// </summary>
    public void ClearTarget()
    {
        CurrentTarget = null;
        targetLostTime = Time.time;

        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name} perdeu o target");
        }
    }

    /// <summary>
    /// Força mudança de estado
    /// </summary>
    public void SetState(EnemyState newState)
    {
        if (CurrentState == newState) return;

        previousState = CurrentState;
        CurrentState = newState;
        stateTimer = 0f;

        OnStateChanged(previousState, CurrentState);
    }

    /// <summary>
    /// Aplica dano externo com knockback
    /// </summary>
    public void TakeDamageWithKnockback(int damage, Vector2 knockbackDirection)
    {
        if (!IsAlive) return;

        // Aplica dano via EntityStatus
        int actualDamage = entityStatus.TakeDamage(damage);

        // Se o dano foi efetivo e ainda está vivo, entra em estado de hit
        if (actualDamage > 0 && IsAlive)
        {
            SetState(EnemyState.Hit);

            // Aplica knockback
            if (knockbackDirection != Vector2.zero && rb != null)
            {
                rb.AddForce(knockbackDirection.normalized * hitKnockbackForce, ForceMode2D.Impulse);
            }

            // Inicia efeito de piscar
            if (blinkOnHit)
            {
                StartCoroutine(BlinkEffect());
            }

            if (enableDetailedLogs)
            {
                Debug.Log($"[EnemyBehavior] {name} recebeu {actualDamage} dano. HP: {CurrentHP}/{MaxHP}");
            }
        }
    }

    /// <summary>
    /// Executa ataque no target atual
    /// </summary>
    public void PerformAttack()
    {
        if (!CanAttack || !HasTarget) return;

        // Usa o sistema de ataque do EntityStatus
        entityStatus.UseBasicAttack();

        // Aplica dano no target se for o player
        if (CurrentTarget.CompareTag("Player"))
        {
            EntityStatus targetStatus = CurrentTarget.GetComponent<EntityStatus>();
            if (targetStatus != null)
            {
                int damage = CurrentDamage;
                targetStatus.TakeDamage(damage);

                // Aplica knockback no player
                if (knockbackForce > 0)
                {
                    Vector2 knockDirection = (CurrentTarget.position - cachedTransform.position).normalized;
                    Rigidbody2D targetRb = CurrentTarget.GetComponent<Rigidbody2D>();
                    if (targetRb != null)
                    {
                        targetRb.AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);
                    }
                }

                if (enableDetailedLogs)
                {
                    Debug.Log($"[EnemyBehavior] {name} atacou {CurrentTarget.name} - {damage} dano");
                }
            }
        }

        // Animação de ataque
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isAttacking", true);
            StartCoroutine(EndAttackAnimation());
        }
    }

    /// <summary>
    /// Move o inimigo na direção especificada
    /// </summary>
    public void MoveInDirection(Vector2 direction, float speedMultiplier = 1f)
    {
        if (rb == null || !IsAlive) return;

        Vector2 velocity = direction.normalized * CurrentSpeed * speedMultiplier;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, velocity, movementSmoothing);

        IsMoving = velocity.magnitude > 0.1f;

        // Atualiza direção visual se houver movimento significativo
        if (IsMoving)
        {
            FacingDirection newDirection = GetDirectionFromMovement(direction);
            if (newDirection != lastFacingDirection)
            {
                UpdateSpriteDirection(newDirection);
            }
            lastDirection = direction.normalized;
        }

        // Atualiza animação
        if (animator != null)
        {
            animator.SetBool("isWalking", IsMoving);
        }
    }

    /// <summary>
    /// Para o movimento
    /// </summary>
    public void StopMovement()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        IsMoving = false;

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }

    /// <summary>
    /// Define pontos de patrulha customizados
    /// </summary>
    public void SetCustomPatrolPoints(Vector2[] customPoints)
    {
        if (customPoints == null || customPoints.Length == 0)
        {
            if (enableDetailedLogs)
            {
                Debug.LogWarning($"[EnemyBehavior] {name} tentou definir pontos de patrulha vazios");
            }
            return;
        }

        patrolPoints = new Vector2[customPoints.Length];
        for (int i = 0; i < customPoints.Length; i++)
        {
            patrolPoints[i] = customPoints[i];
        }

        currentPatrolIndex = 0;
        isWaitingAtPatrolPoint = false;
        patrolWaitTimer = 0f;
        patrolMoveTimer = 0f; // Reset do timer de movimento
        hasReachedPatrolDestination = false;

        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name} definiu {customPoints.Length} pontos de patrulha customizados");
        }

        // Se está em patrulha, reinicia com novos pontos
        if (CurrentState == EnemyState.Patrol)
        {
            SetNextPatrolDestination();
        }
    }

    /// <summary>
    /// Gera novos pontos de patrulha aleatórios
    /// </summary>
    public void RegeneratePatrolPoints()
    {
        GeneratePatrolPoints();

        // Se está em patrulha, reinicia com novos pontos
        if (CurrentState == EnemyState.Patrol)
        {
            isWaitingAtPatrolPoint = false;
            patrolWaitTimer = 0f;
            patrolMoveTimer = 0f; // Reset do timer de movimento
            hasReachedPatrolDestination = false;
            SetNextPatrolDestination();
        }
    }

    /// <summary>
    /// Define o timeout para chegar ao destino de patrulha
    /// </summary>
    public void SetPatrolTimeout(float timeout)
    {
        patrolTimeout = Mathf.Max(0f, timeout);

        if (enableDetailedLogs)
        {
            if (patrolTimeout > 0)
            {
                Debug.Log($"[EnemyBehavior] {name} timeout de patrulha definido para {patrolTimeout:F1}s");
            }
            else
            {
                Debug.Log($"[EnemyBehavior] {name} timeout de patrulha desabilitado");
            }
        }
    }
    #endregion

    #region Corrotinas
    /// <summary>
    /// Efeito de piscar quando recebe dano
    /// </summary>
    private IEnumerator BlinkEffect()
    {
        if (spriteRenderer == null) yield break;

        isBlinking = true;
        float elapsed = 0f;
        // Usa o tempo de invencibilidade configurado localmente
        float invulnerabilityTime = this.invulnerabilityTime;

        while (elapsed < invulnerabilityTime)
        {
            float alpha = Mathf.PingPong(elapsed * blinkSpeed, 1f);
            Color blinkColor = originalColor;
            blinkColor.a = alpha;
            spriteRenderer.color = blinkColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;
        isBlinking = false;
    }

    /// <summary>
    /// Finaliza animação de ataque
    /// </summary>
    private IEnumerator EndAttackAnimation()
    {
        yield return new WaitForSeconds(0.8f); // Duração padrão da animação

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    /// <summary>
    /// Sai do estado de hit após duração
    /// </summary>
    private IEnumerator ExitHitState()
    {
        yield return hitWait;

        if (CurrentState == EnemyState.Hit && IsAlive)
        {
            EnemyState nextState = HasTarget ? EnemyState.Chase : EnemyState.Idle;
            SetState(nextState);
        }
    }
    #endregion

    #region Métodos Principais
    /// <summary>
    /// Atualização principal do comportamento
    /// </summary>
    private void UpdateBehavior()
    {
        stateTimer += Time.deltaTime;
        UpdateTargetDistance();

        // Detecta player se não estiver em states que bloqueiam detecção
        if (CurrentState == EnemyState.Idle || CurrentState == EnemyState.Patrol)
        {
            UpdatePlayerDetection();
        }

        // Atualiza comportamento baseado no estado atual
        switch (CurrentState)
        {
            case EnemyState.Idle:
                UpdateIdleBehavior();
                break;
            case EnemyState.Patrol:
                UpdatePatrolBehavior();
                break;
            case EnemyState.Chase:
                UpdateChaseBehavior();
                break;
            case EnemyState.Attack:
                UpdateAttackBehavior();
                break;
            case EnemyState.Hit:
                // Estado gerenciado por corrotina
                break;
            case EnemyState.Dead:
                // Estado final, sem atualização
                break;
        }
    }

    /// <summary>
    /// Detecta o player dentro do raio de detecção
    /// </summary>
    private void UpdatePlayerDetection()
    {
        // Não detecta se já tem target
        if (HasTarget) return;

        // Busca por objetos na layer do player dentro do raio de detecção
        Collider2D playerCollider = Physics2D.OverlapCircle(cachedTransform.position, detectionRadius, playerLayer);

        if (playerCollider != null)
        {
            // IMPORTANTE: Verifica se realmente é o player pela tag
            if (!playerCollider.CompareTag("Player"))
            {
                return; // Não é o player, ignora
            }

            Transform potentialTarget = playerCollider.transform;

            // Verifica se requer linha de visão
            if (requireLineOfSight)
            {
                if (!HasLineOfSightToTarget(potentialTarget))
                {
                    return;
                }
            }

            // Player detectado! Define como target
            SetTarget(potentialTarget);

            // Se for agressivo, muda para perseguição
            if (behaviorType == BehaviorType.Aggressive)
            {
                SetState(EnemyState.Chase);

                if (enableDetailedLogs)
                {
                    Debug.Log($"[EnemyBehavior] {name} detectou player e iniciou perseguição!");
                }
            }
            // Se for defensivo, só fica alerta mas não persegue
            else if (behaviorType == BehaviorType.Defensive)
            {
                if (enableDetailedLogs)
                {
                    Debug.Log($"[EnemyBehavior] {name} detectou player mas permanece defensivo");
                }
            }
        }
    }

    /// <summary>
    /// Verifica se há linha de visão até o target
    /// </summary>
    private bool HasLineOfSightToTarget(Transform target)
    {
        if (target == null) return false;

        Vector2 direction = target.position - cachedTransform.position;
        float distance = direction.magnitude;

        // Raycast para verificar obstáculos
        RaycastHit2D hit = Physics2D.Raycast(cachedTransform.position, direction.normalized, distance, obstacleLayer);

        // Se não atingiu nada, há linha de visão livre
        return hit.collider == null;
    }

    /// <summary>
    /// Comportamento no estado Idle
    /// </summary>
    private void UpdateIdleBehavior()
    {
        // Se a patrulha está habilitada, muda para patrulha
        if (enablePatrol && patrolPoints != null && patrolPoints.Length > 0)
        {
            SetState(EnemyState.Patrol);
            return;
        }

        // Para o movimento se estiver se movendo
        if (IsMoving)
        {
            StopMovement();
        }
    }

    /// <summary>
    /// Comportamento no estado Patrol
    /// </summary>
    private void UpdatePatrolBehavior()
    {
        if (!enablePatrol || patrolPoints == null || patrolPoints.Length == 0)
        {
            SetState(EnemyState.Idle);
            return;
        }

        // Se detectou um target e é agressivo, muda para perseguição
        if (HasTarget && behaviorType == BehaviorType.Aggressive)
        {
            SetState(EnemyState.Chase);
            return;
        }

        // Se está esperando no ponto de patrulha
        if (isWaitingAtPatrolPoint)
        {
            patrolWaitTimer += Time.deltaTime;

            // Para o movimento enquanto espera
            if (IsMoving)
            {
                StopMovement();
            }

            // Verifica se terminou o tempo de espera
            if (patrolWaitTimer >= patrolWaitTime)
            {
                isWaitingAtPatrolPoint = false;
                patrolWaitTimer = 0f;

                // Define próximo ponto de patrulha
                SetNextPatrolDestination();

                if (enableDetailedLogs)
                {
                    Debug.Log($"[EnemyBehavior] {name} indo para próximo ponto de patrulha: {currentDestination}");
                }
            }
            return;
        }

        // Se ainda não tem destino, define o primeiro ponto
        if (currentDestination == Vector2.zero)
        {
            SetNextPatrolDestination();
        }

        // Atualiza timer de movimento (apenas quando está se movendo)
        if (!isWaitingAtPatrolPoint)
        {
            patrolMoveTimer += Time.deltaTime;
        }

        // Verifica timeout do movimento (se habilitado)
        if (patrolTimeout > 0 && patrolMoveTimer >= patrolTimeout)
        {
            if (enableDetailedLogs)
            {
                Debug.Log($"[EnemyBehavior] {name} timeout ao ir para ponto de patrulha, buscando novo destino");
            }

            // Timeout atingido, busca novo destino
            SetNextPatrolDestination();
            return;
        }

        // Move em direção ao destino atual
        Vector2 directionToDestination = (currentDestination - (Vector2)cachedTransform.position).normalized;
        float distanceToDestination = Vector2.Distance(cachedTransform.position, currentDestination);

        // Verifica se chegou no destino
        if (distanceToDestination <= POSITION_TOLERANCE)
        {
            // Chegou no ponto, inicia espera
            isWaitingAtPatrolPoint = true;
            patrolWaitTimer = 0f;
            patrolMoveTimer = 0f; // Reset do timer de movimento
            hasReachedPatrolDestination = true;

            StopMovement();

            if (enableDetailedLogs)
            {
                Debug.Log($"[EnemyBehavior] {name} chegou no ponto de patrulha, esperando {patrolWaitTime}s");
            }
        }
        else
        {
            // Move em direção ao destino
            MoveInDirection(directionToDestination, patrolSpeedMultiplier);
        }
    }

    /// <summary>
    /// Comportamento no estado Chase
    /// </summary>
    private void UpdateChaseBehavior()
    {
        // Verifica se ainda tem target válido
        if (!HasTarget || CurrentTarget == null)
        {
            if (enableDetailedLogs)
            {
                Debug.Log($"[EnemyBehavior] {name} perdeu target (null ou inválido)");
            }

            ClearTarget();
            SetState(enablePatrol ? EnemyState.Patrol : EnemyState.Idle);
            return;
        }

        // Recalcula distância em tempo real para garantir precisão
        float currentDistance = Vector2.Distance(cachedTransform.position, CurrentTarget.position);
        DistanceToTarget = currentDistance;

        // Verifica se o target está muito longe (perdeu de vista)
        if (currentDistance > loseTargetRadius)
        {
            if (enableDetailedLogs)
            {
                Debug.Log($"[EnemyBehavior] {name} perdeu target por distância ({currentDistance:F1} > {loseTargetRadius})");
            }

            ClearTarget();
            SetState(enablePatrol ? EnemyState.Patrol : EnemyState.Idle);
            return;
        }

        // Verifica linha de visão se necessário
        if (requireLineOfSight && !HasLineOfSightToTarget(CurrentTarget))
        {
            // Inicia contagem de tempo sem linha de visão
            if (Time.time - lastDetectionTime > forgetTime)
            {
                if (enableDetailedLogs)
                {
                    Debug.Log($"[EnemyBehavior] {name} perdeu target por linha de visão");
                }

                ClearTarget();
                SetState(enablePatrol ? EnemyState.Patrol : EnemyState.Idle);
                return;
            }
        }
        else
        {
            // Atualiza tempo da última detecção
            lastDetectionTime = Time.time;
        }

        // Calcula direção para o target em tempo real
        Vector2 directionToTarget = (CurrentTarget.position - cachedTransform.position).normalized;

        // Verifica se está no range de ataque
        if (currentDistance <= attackRange)
        {
            // Dentro do range de ataque - pode parar ou continuar se movendo
            if (currentDistance <= stoppingDistance)
            {
                // Muito perto, para o movimento
                StopMovement();
            }
            else if (!canAttackWhileMoving)
            {
                // Não pode atacar em movimento, para para atacar
                StopMovement();
            }

            // Verifica se pode atacar
            if (CanAttack)
            {
                SetState(EnemyState.Attack);
                return;
            }

            // Se pode atacar em movimento e não está muito perto, continua se movendo
            if (canAttackWhileMoving && currentDistance > stoppingDistance)
            {
                MoveInDirection(directionToTarget, chaseSpeedMultiplier * 0.5f); // Velocidade reduzida no range de ataque
            }
        }
        else
        {
            // Fora do range de ataque - move em direção ao target
            MoveInDirection(directionToTarget, chaseSpeedMultiplier);

            // Verifica se pode atacar mesmo em movimento (se permitido e estiver no range)
            if (canAttackWhileMoving && CanAttack)
            {
                SetState(EnemyState.Attack);
                return;
            }
        }

        // Debug: Log periódico da perseguição
        if (enableDetailedLogs && Time.time % 2f < Time.deltaTime)
        {
            Debug.Log($"[EnemyBehavior] {name} perseguindo - Distância: {currentDistance:F1}, Direção: {directionToTarget}");
        }
    }

    /// <summary>
    /// Comportamento no estado Attack
    /// </summary>
    private void UpdateAttackBehavior()
    {
        // Verifica se ainda tem target válido
        if (!HasTarget || CurrentTarget == null)
        {
            if (enableDetailedLogs)
            {
                Debug.Log($"[EnemyBehavior] {name} perdeu target durante ataque");
            }

            ClearTarget();
            SetState(enablePatrol ? EnemyState.Patrol : EnemyState.Idle);
            return;
        }

        // Recalcula distância para verificar se ainda está no range
        float currentDistance = Vector2.Distance(cachedTransform.position, CurrentTarget.position);
        DistanceToTarget = currentDistance;

        if (currentDistance > attackRange)
        {
            // Target saiu do range de ataque, volta para chase
            SetState(EnemyState.Chase);
            return;
        }

        // Para o movimento para atacar
        StopMovement();

        // Executa ataque se disponível
        if (CanAttack)
        {
            PerformAttack();
        }
    }

    /// <summary>
    /// Define o próximo destino de patrulha
    /// </summary>
    private void SetNextPatrolDestination()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (randomPatrol)
        {
            // Patrulha aleatória
            currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        }
        else
        {
            // Patrulha sequencial
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        currentDestination = patrolPoints[currentPatrolIndex];
        hasReachedPatrolDestination = false;
        patrolMoveTimer = 0f; // Reset do timer de movimento ao definir novo destino
    }

    /// <summary>
    /// Atualiza distância até o target
    /// </summary>
    private void UpdateTargetDistance()
    {
        if (HasTarget && CurrentTarget != null)
        {
            DistanceToTarget = Vector2.Distance(cachedTransform.position, CurrentTarget.position);
        }
        else
        {
            DistanceToTarget = float.MaxValue;
        }
    }

    /// <summary>
    /// Callback de mudança de estado
    /// </summary>
    private void OnStateChanged(EnemyState oldState, EnemyState newState)
    {
        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name}: {oldState} → {newState}");
        }

        UpdateAnimatorState(newState);

        switch (newState)
        {
            case EnemyState.Dead:
                OnEnterDeadState();
                break;
            case EnemyState.Hit:
                OnEnterHitState();
                break;
            case EnemyState.Patrol:
                OnEnterPatrolState();
                break;
            case EnemyState.Chase:
                OnEnterChaseState();
                break;
        }
    }

    /// <summary>
    /// Atualiza parâmetros do animator
    /// </summary>
    private void UpdateAnimatorState(EnemyState state)
    {
        if (animator == null) return;

        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        switch (state)
        {
            case EnemyState.Patrol:
            case EnemyState.Chase:
                animator.SetBool("isWalking", true);
                break;
            case EnemyState.Dead:
                animator.SetTrigger("dieDestroy");
                break;
            case EnemyState.Hit:
                animator.SetTrigger("takeDamage");
                break;
        }
    }

    /// <summary>
    /// Lógica ao entrar no estado Dead
    /// </summary>
    private void OnEnterDeadState()
    {
        StopMovement();

        // Desabilita colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var col in colliders)
        {
            if (col != null) col.enabled = false;
        }

        Destroy(gameObject, 3f);
    }

    /// <summary>
    /// Lógica ao entrar no estado Hit
    /// </summary>
    private void OnEnterHitState()
    {
        if (rb != null)
        {
            rb.linearVelocity *= 0.5f; // Reduz velocidade
        }

        StartCoroutine(ExitHitState());
    }

    /// <summary>
    /// Lógica ao entrar no estado Patrol
    /// </summary>
    private void OnEnterPatrolState()
    {
        // Reseta variáveis de patrulha
        isWaitingAtPatrolPoint = false;
        patrolWaitTimer = 0f;
        patrolMoveTimer = 0f; // Reset do timer de movimento
        hasReachedPatrolDestination = false;

        // Se não há pontos de patrulha, gera novos
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            GeneratePatrolPoints();
        }

        // Define primeiro destino de patrulha
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            SetNextPatrolDestination();

            if (enableDetailedLogs)
            {
                Debug.Log($"[EnemyBehavior] {name} iniciou patrulha com {patrolPoints.Length} pontos");
            }
        }
    }

    /// <summary>
    /// Lógica ao entrar no estado Chase
    /// </summary>
    private void OnEnterChaseState()
    {
        // Para movimento de patrulha anterior
        StopMovement();

        // Verifica se tem target válido
        if (!HasTarget || CurrentTarget == null)
        {
            if (enableDetailedLogs)
            {
                Debug.LogWarning($"[EnemyBehavior] {name} entrou em Chase sem target válido!");
            }
            SetState(enablePatrol ? EnemyState.Patrol : EnemyState.Idle);
            return;
        }

        // Calcula direção inicial para o target
        Vector2 directionToTarget = (CurrentTarget.position - cachedTransform.position).normalized;

        // Atualiza direção visual imediatamente
        if (directionToTarget != Vector2.zero)
        {
            FacingDirection chaseDirection = GetDirectionFromMovement(directionToTarget);
            if (chaseDirection != lastFacingDirection)
            {
                UpdateSpriteDirection(chaseDirection);
            }
        }

        // Recalcula distância para o target
        DistanceToTarget = Vector2.Distance(cachedTransform.position, CurrentTarget.position);

        // Log informativo
        if (enableDetailedLogs)
        {
            Debug.Log($"[EnemyBehavior] {name} iniciou perseguição! Distância: {DistanceToTarget:F1}");
        }
    }
    #endregion

    #region Debug
    /// <summary>
    /// Desenha gizmos de debug
    /// </summary>
    private void DrawDebugGizmos()
    {
        Vector3 position = cachedTransform ? cachedTransform.position : transform.position;

        // Raio de detecção
        Gizmos.color = detectionColor;
        Gizmos.DrawWireSphere(position, detectionRadius);

        // Raio de perda de target (mais claro)
        Gizmos.color = new Color(detectionColor.r, detectionColor.g, detectionColor.b, 0.3f);
        Gizmos.DrawWireSphere(position, loseTargetRadius);

        // Raio de ataque
        Gizmos.color = attackColor;
        Gizmos.DrawWireSphere(position, attackRange);

        // Pontos de patrulha
        if (enablePatrol && patrolPoints != null)
        {
            Gizmos.color = patrolColor;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                Vector2 point = patrolPoints[i];

                // Desenha esfera do ponto
                Gizmos.DrawWireSphere(point, 0.3f);

                // Destaca o ponto atual
                if (i == currentPatrolIndex && CurrentState == EnemyState.Patrol)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireSphere(point, 0.5f);
                    Gizmos.color = patrolColor;
                }
            }

            // Linha até o destino atual se estiver patrulhando
            if (CurrentState == EnemyState.Patrol && currentDestination != Vector2.zero)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(position, currentDestination);
            }
        }

        // Linha até o target
        if (HasTarget && CurrentTarget != null)
        {
            // Linha verde se há linha de visão, vermelha se não há
            if (requireLineOfSight)
            {
                bool hasLoS = HasLineOfSightToTarget(CurrentTarget);
                Gizmos.color = hasLoS ? Color.green : Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawLine(position, CurrentTarget.position);

            // Desenha informação sobre distância
            float distance = Vector2.Distance(position, CurrentTarget.position);
            if (distance > loseTargetRadius)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(CurrentTarget.position, 0.5f);
            }
        }

#if UNITY_EDITOR
        // Verifica se entityStatus está disponível antes de acessar suas propriedades
        if (entityStatus != null)
        {
            // Info de debug
            string info = $"Estado: {CurrentState}\n" +
                         $"HP: {CurrentHP}/{MaxHP}\n" +
                         $"Damage: {CurrentDamage}\n" +
                         $"Speed: {CurrentSpeed:F1}";

            // Informações específicas de patrulha
            if (CurrentState == EnemyState.Patrol && patrolPoints != null)
            {
                info += $"\nPonto atual: {currentPatrolIndex + 1}/{patrolPoints.Length}";
                if (isWaitingAtPatrolPoint)
                {
                    info += $"\nEsperando: {patrolWaitTimer:F1}s/{patrolWaitTime:F1}s";
                }
                else
                {
                    float distToDest = Vector2.Distance(position, currentDestination);
                    info += $"\nDist. destino: {distToDest:F1}";

                    // Mostra informações de timeout se habilitado
                    if (patrolTimeout > 0)
                    {
                        float timeLeft = patrolTimeout - patrolMoveTimer;
                        info += $"\nTimeout: {timeLeft:F1}s";
                    }
                }
            }

            UnityEditor.Handles.Label(position + Vector3.up * 2f, info);
        }
        else
        {
            // Info básica quando entityStatus não está disponível
            string basicInfo = $"Estado: {CurrentState}\n" +
                              $"EntityStatus: NULL\n" +
                              $"Inicializando...";

            UnityEditor.Handles.Label(position + Vector3.up * 2f, basicInfo);
        }
#endif
    }
    #endregion
}