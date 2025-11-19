using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controlador básico para NPCs no jogo SlimeKing.
/// 
/// RESPONSABILIDADES:
/// • Gerencia movimento autônomo simples
/// • Controla animações básicas (isWalking)
/// • Integra com NPCAttributesHandler para atributos
/// • Controla direção visual baseado em movimento
/// • Sistema de direção visual simples (front/back/side)
/// 
/// DEPENDÊNCIAS:
/// • Rigidbody2D: Para física de movimento
/// • Animator: Para controle de animações
/// • SpriteRenderer: Para flip de direção (não obrigatório)
/// • NPCAttributesHandler: Para sistema de atributos (opcional)
/// 
/// ESTRUTURA ESPERADA (baseada em NPCTemplate):
/// • GameObject principal com Animator, SortingGroup, NPCAttributesHandler
/// • Objetos filhos: front, back, side, vfx_front, vfx_back, vfx_side
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class NPCController : MonoBehaviour
{
    #region Inspector Configuration

    [Header("⚙️ Configurações de Movimento")]
    [Tooltip("Velocidade máxima de movimento do NPC (será sobrescrita pelos atributos se NPCAttributesHandler estiver presente)")]
    [SerializeField] private float moveSpeed = 2f;

    [Tooltip("Velocidade de aceleração ao iniciar movimento (unidades por segundo)")]
    [SerializeField] private float acceleration = 8f;

    [Tooltip("Velocidade de desaceleração ao parar movimento (unidades por segundo)")]
    [SerializeField] private float deceleration = 8f;

    [Header("🎨 Configurações Visuais")]
    [Tooltip("Referências aos GameObjects filhos para controle de direção visual")]
    [SerializeField] private GameObject frontObject;
    [SerializeField] private GameObject backObject;
    [SerializeField] private GameObject sideObject;
    [SerializeField] private GameObject vfxFrontObject;
    [SerializeField] private GameObject vfxBackObject;
    [SerializeField] private GameObject vfxSideObject;

    [Header("🤖 Configurações de IA")]
    [Tooltip("Tipo de movimento do NPC")]
    [SerializeField] private MovementType movementType = MovementType.Idle;

    [Tooltip("Tempo mínimo parado antes de se mover (para movimento randômico)")]
    [SerializeField] private float minIdleTime = 2f;

    [Tooltip("Tempo máximo parado antes de se mover (para movimento randômico)")]
    [SerializeField] private float maxIdleTime = 5f;

    [Tooltip("Tempo mínimo em movimento (para movimento randômico)")]
    [SerializeField] private float minMoveTime = 1f;

    [Tooltip("Tempo máximo em movimento (para movimento randômico)")]
    [SerializeField] private float maxMoveTime = 3f;

    [Tooltip("Raio de movimento randômico em torno da posição inicial")]
    [SerializeField] private float wanderRadius = 3f;

    [Header("🔧 Ferramentas de Debug")]
    [Tooltip("Habilita logs detalhados no Console para debug de movimento")]
    [SerializeField] private bool enableLogs = false;

    [Tooltip("Mostra gizmos no Scene View para visualizar informações de debug")]
    [SerializeField] private bool enableDebugGizmos = true;

    #endregion

    #region Private Variables

    // === COMPONENTES ESSENCIAIS ===
    private Rigidbody2D _rigidbody;              // Física de movimento
    private Animator _animator;                   // Controle de animações
    private SpriteRenderer _spriteRenderer;      // Flip de sprite (opcional)
    private NPCAttributesHandler _attributesHandler; // Sistema de atributos (opcional)

    // === ESTADO DO MOVIMENTO ===
    private Vector2 _moveDirection;              // Direção de movimento atual
    private bool _isMoving = false;              // Se o NPC está em movimento
    private bool _canMove = true;                // Se o movimento está habilitado
    private bool _facingRight = true;            // Direção atual do sprite

    // === SISTEMA DE IA ===
    private Vector3 _initialPosition;            // Posição inicial para wandering
    private Coroutine _aiCoroutine = null;       // Corrotina principal da IA
    private Vector2 _targetPosition;             // Posição alvo atual

    // === OTIMIZAÇÃO DE PERFORMANCE ===
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int FacingRight = Animator.StringToHash("FacingRight");

    // === CONSTANTES ===
    private const float MOVEMENT_THRESHOLD = 0.1f;
    private const float TARGET_REACH_THRESHOLD = 0.3f;

    // === SISTEMA DE DIREÇÃO VISUAL ===
    public enum VisualDirection
    {
        South,  // Frente (padrão)
        North,  // Costas
        Side    // Lateral (East/West)
    }

    public enum MovementType
    {
        Idle,           // Parado (sem movimento)
        Wander,         // Movimento randômico
        Patrol,         // Patrulha entre pontos (futuro)
        Follow          // Segue um alvo (futuro)
    }

    private VisualDirection _currentVisualDirection = VisualDirection.South;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Obtém componentes obrigatórios
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();

        // Obtém componente opcional
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _attributesHandler = GetComponent<NPCAttributesHandler>();

        // Armazena posição inicial
        _initialPosition = transform.position;

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} inicializado na posição {_initialPosition}");
        }
    }

    private void Start()
    {
        // Conecta com o sistema de atributos se disponível
        ConnectToAttributeSystem();

        // Sincroniza velocidade inicial com os atributos
        SynchronizeInitialSpeed();

        // Inicializa sistema visual direcional
        InitializeVisualObjects();

        // Inicia comportamento de IA
        StartAIBehavior();

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} - Tipo de movimento: {movementType}, Velocidade: {moveSpeed}");
        }
    }

    private void Update()
    {
        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnDestroy()
    {
        // Para corrotina de IA se estiver rodando
        if (_aiCoroutine != null)
        {
            StopCoroutine(_aiCoroutine);
        }
    }

    #endregion

    #region Initialization Methods

    private void ConnectToAttributeSystem()
    {
        if (_attributesHandler != null)
        {
            // Aqui podem ser adicionados eventos futuros do NPCAttributesHandler
            if (enableLogs)
            {
                Debug.Log($"[NPCController] {gameObject.name} conectado ao sistema de atributos");
            }
        }
    }

    private void SynchronizeInitialSpeed()
    {
        if (_attributesHandler != null)
        {
            float attributeSpeed = _attributesHandler.CurrentSpeed;
            if (attributeSpeed != moveSpeed)
            {
                moveSpeed = attributeSpeed;
                if (enableLogs)
                {
                    Debug.Log($"[NPCController] {gameObject.name} - Velocidade sincronizada: {moveSpeed}");
                }
            }
        }
    }

    private void InitializeVisualObjects()
    {
        // Auto-detecta objetos filhos se não foram configurados no inspector
        if (frontObject == null) frontObject = transform.Find("front")?.gameObject;
        if (backObject == null) backObject = transform.Find("back")?.gameObject;
        if (sideObject == null) sideObject = transform.Find("side")?.gameObject;
        if (vfxFrontObject == null) vfxFrontObject = transform.Find("vfx_front")?.gameObject;
        if (vfxBackObject == null) vfxBackObject = transform.Find("vfx_back")?.gameObject;
        if (vfxSideObject == null) vfxSideObject = transform.Find("vfx_side")?.gameObject;

        // Define direção inicial como South (frente)
        SetVisualDirection(VisualDirection.South);

        if (enableLogs)
        {
            int objectsFound = 0;
            if (frontObject != null) objectsFound++;
            if (backObject != null) objectsFound++;
            if (sideObject != null) objectsFound++;
            Debug.Log($"[NPCController] {gameObject.name} - {objectsFound} objetos visuais encontrados");
        }
    }

    #endregion

    #region Movement System

    private void HandleMovement()
    {
        if (!_canMove) return;

        // Sincroniza velocidade com sistema de atributos
        SynchronizeSpeedWithAttributes();

        // Aplica movimento suave
        ApplySmoothMovement();

        // Atualiza direção do sprite baseada no movimento
        HandleSpriteDirection();

        // Atualiza direção visual dos objetos direcionais
        UpdateVisualDirection();
    }

    private void SynchronizeSpeedWithAttributes()
    {
        if (_attributesHandler != null)
        {
            float attributeSpeed = _attributesHandler.CurrentSpeed;
            if (attributeSpeed != moveSpeed)
            {
                moveSpeed = attributeSpeed;
            }
        }
    }

    private void ApplySmoothMovement()
    {
        Vector2 targetVelocity = _moveDirection * moveSpeed;

        // Escolhe taxa de interpolação baseada se está acelerando ou desacelerando
        float currentRate = _isMoving ? acceleration : deceleration;

        // Aplica movimento suave
        _rigidbody.linearVelocity = Vector2.MoveTowards(
            _rigidbody.linearVelocity,
            targetVelocity,
            currentRate * Time.fixedDeltaTime
        );
    }

    private void HandleSpriteDirection()
    {
        if (_moveDirection.x > MOVEMENT_THRESHOLD && !_facingRight)
        {
            FlipSprite();
        }
        else if (_moveDirection.x < -MOVEMENT_THRESHOLD && _facingRight)
        {
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        _facingRight = !_facingRight;

        // Aplica flip visual no sprite se disponível
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = !_facingRight;
        }

        // Atualiza Animator para animações direcionais (se disponível)
        if (_animator != null)
        {
            _animator.SetBool(FacingRight, _facingRight);
        }

        // Se estiver na direção lateral, atualiza flip dos objetos laterais
        if (_currentVisualDirection == VisualDirection.Side)
        {
            ApplyFlipToSideObject(sideObject);
        }

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} - Direção alterada: {(_facingRight ? "Direita" : "Esquerda")}");
        }
    }

    #endregion

    #region AI Behavior System

    private void StartAIBehavior()
    {
        if (_aiCoroutine != null)
        {
            StopCoroutine(_aiCoroutine);
        }

        switch (movementType)
        {
            case MovementType.Idle:
                // Não inicia nenhuma corrotina - fica parado
                SetMoveDirection(Vector2.zero);
                break;

            case MovementType.Wander:
                _aiCoroutine = StartCoroutine(WanderBehavior());
                break;

            case MovementType.Patrol:
                // TODO: Implementar sistema de patrulha
                Debug.LogWarning($"[NPCController] {gameObject.name} - Patrol ainda não implementado, usando Idle");
                SetMoveDirection(Vector2.zero);
                break;

            case MovementType.Follow:
                // TODO: Implementar sistema de seguir alvo
                Debug.LogWarning($"[NPCController] {gameObject.name} - Follow ainda não implementado, usando Idle");
                SetMoveDirection(Vector2.zero);
                break;
        }
    }

    private IEnumerator WanderBehavior()
    {
        while (true)
        {
            // FASE 1: IDLE - Fica parado por um tempo
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            SetMoveDirection(Vector2.zero);

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {gameObject.name} - Idle por {idleTime:F1}s");
            }

            yield return new WaitForSeconds(idleTime);

            // FASE 2: MOVIMENTO - Move para uma posição randômica
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            Vector3 targetPosition = _initialPosition + (Vector3)(randomDirection * Random.Range(0.5f, wanderRadius));
            _targetPosition = targetPosition;

            float moveTime = Random.Range(minMoveTime, maxMoveTime);
            float elapsedTime = 0f;

            if (enableLogs)
            {
                Debug.Log($"[NPCController] {gameObject.name} - Movendo para {targetPosition} por até {moveTime:F1}s");
            }

            // Move em direção ao alvo por até moveTime segundos
            while (elapsedTime < moveTime)
            {
                Vector2 directionToTarget = ((Vector2)targetPosition - (Vector2)transform.position).normalized;
                float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

                // Se chegou próximo do alvo, para o movimento
                if (distanceToTarget < TARGET_REACH_THRESHOLD)
                {
                    if (enableLogs)
                    {
                        Debug.Log($"[NPCController] {gameObject.name} - Alvo alcançado!");
                    }
                    break;
                }

                SetMoveDirection(directionToTarget);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void SetMoveDirection(Vector2 direction)
    {
        _moveDirection = direction;
        _isMoving = direction.magnitude > MOVEMENT_THRESHOLD;

        if (enableLogs && _isMoving != (_moveDirection.magnitude > MOVEMENT_THRESHOLD))
        {
            Debug.Log($"[NPCController] {gameObject.name} - Movimento: {(_isMoving ? "Iniciado" : "Parado")}");
        }
    }

    #endregion

    #region Visual Direction System

    private void UpdateVisualDirection()
    {
        if (_moveDirection.magnitude < MOVEMENT_THRESHOLD) return;

        Vector2 direction = _moveDirection.normalized;

        // Determina direção visual baseada no movimento
        if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            // Movimento mais vertical
            SetVisualDirection(direction.y > 0 ? VisualDirection.North : VisualDirection.South);
        }
        else
        {
            // Movimento mais horizontal
            SetVisualDirection(VisualDirection.Side);
        }
    }

    private void SetVisualDirection(VisualDirection direction)
    {
        if (_currentVisualDirection == direction) return;

        _currentVisualDirection = direction;

        // Desativa todos os objetos primeiro
        if (frontObject != null) frontObject.SetActive(false);
        if (backObject != null) backObject.SetActive(false);
        if (sideObject != null) sideObject.SetActive(false);

        // VFX sempre desativados (NPCs básicos não usam VFX por padrão)
        if (vfxFrontObject != null) vfxFrontObject.SetActive(false);
        if (vfxBackObject != null) vfxBackObject.SetActive(false);
        if (vfxSideObject != null) vfxSideObject.SetActive(false);

        // Ativa objeto baseado na direção
        switch (direction)
        {
            case VisualDirection.South:
                if (frontObject != null) frontObject.SetActive(true);
                break;

            case VisualDirection.North:
                if (backObject != null) backObject.SetActive(true);
                break;

            case VisualDirection.Side:
                if (sideObject != null)
                {
                    sideObject.SetActive(true);
                    ApplyFlipToSideObject(sideObject);
                }
                break;
        }

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} - Direção visual: {direction}");
        }
    }

    private void ApplyFlipToSideObject(GameObject sideObj)
    {
        if (sideObj == null) return;

        SpriteRenderer sideSpriteRenderer = sideObj.GetComponent<SpriteRenderer>();
        if (sideSpriteRenderer != null)
        {
            sideSpriteRenderer.flipX = !_facingRight;
            return;
        }

        // Busca nos filhos se não encontrou no objeto principal
        sideSpriteRenderer = sideObj.GetComponentInChildren<SpriteRenderer>();
        if (sideSpriteRenderer != null)
        {
            sideSpriteRenderer.flipX = !_facingRight;
        }
    }

    #endregion

    #region Animation Updates

    private void UpdateAnimations()
    {
        if (_animator == null) return;

        // Atualiza parâmetro de movimento
        _animator.SetBool(IsWalking, _isMoving && _canMove);

        // Atualiza direção (se o parâmetro existir)
        if (HasAnimatorParameter("FacingRight", AnimatorControllerParameterType.Bool))
        {
            _animator.SetBool(FacingRight, _facingRight);
        }
    }

    private bool HasAnimatorParameter(string paramName, AnimatorControllerParameterType paramType)
    {
        if (_animator == null) return false;

        foreach (AnimatorControllerParameter param in _animator.parameters)
        {
            if (param.name == paramName && param.type == paramType)
                return true;
        }
        return false;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Altera o tipo de movimento do NPC em runtime
    /// </summary>
    /// <param name="newMovementType">Novo tipo de movimento</param>
    public void SetMovementType(MovementType newMovementType)
    {
        if (movementType == newMovementType) return;

        movementType = newMovementType;

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} - Tipo de movimento alterado para: {movementType}");
        }

        // Reinicia comportamento de IA
        StartAIBehavior();
    }

    /// <summary>
    /// Desabilita o movimento do NPC
    /// </summary>
    public void DisableMovement()
    {
        _canMove = false;
        SetMoveDirection(Vector2.zero);

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} - Movimento desabilitado");
        }
    }

    /// <summary>
    /// Reabilita o movimento do NPC
    /// </summary>
    public void EnableMovement()
    {
        _canMove = true;

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} - Movimento reabilitado");
        }
    }

    /// <summary>
    /// Define uma nova posição inicial para movimento randômico
    /// </summary>
    /// <param name="newPosition">Nova posição de referência</param>
    public void SetWanderCenter(Vector3 newPosition)
    {
        _initialPosition = newPosition;

        if (enableLogs)
        {
            Debug.Log($"[NPCController] {gameObject.name} - Nova posição de referência: {_initialPosition}");
        }
    }

    /// <summary>
    /// Força o NPC a olhar para uma direção específica
    /// </summary>
    /// <param name="faceRight">True para direita, false para esquerda</param>
    public void FaceDirection(bool faceRight)
    {
        if (_facingRight != faceRight)
        {
            FlipSprite();
        }
    }

    /// <summary>
    /// Obtém a direção visual atual do NPC
    /// </summary>
    /// <returns>Direção visual atual</returns>
    public VisualDirection GetCurrentVisualDirection()
    {
        return _currentVisualDirection;
    }

    /// <summary>
    /// Verifica se o NPC está em movimento
    /// </summary>
    /// <returns>True se estiver se movendo</returns>
    public bool IsMoving()
    {
        return _isMoving && _canMove;
    }

    #endregion

    #region Debug Visualization

    private void OnDrawGizmos()
    {
        if (!enableDebugGizmos) return;

        // Desenha raio de movimento randômico
        if (movementType == MovementType.Wander)
        {
            Gizmos.color = Color.yellow;
            Vector3 center = Application.isPlaying ? _initialPosition : transform.position;
            Gizmos.DrawWireSphere(center, wanderRadius);

            // Desenha linha para posição inicial
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _initialPosition);
            }
        }

        // Desenha direção de movimento atual
        if (Application.isPlaying && _isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _moveDirection * 1f);
        }

        // Desenha informações de debug
        DrawDebugInfo();
    }

    private void DrawDebugInfo()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Vector3 labelPosition = transform.position + Vector3.up * 2f;
            string debugInfo = $"{gameObject.name}\n" +
                             $"Type: {movementType}\n" +
                             $"Speed: {moveSpeed:F1}\n" +
                             $"Moving: {_isMoving}\n" +
                             $"Can Move: {_canMove}\n" +
                             $"Facing: {(_facingRight ? "Right" : "Left")}\n" +
                             $"Visual: {_currentVisualDirection}";

            UnityEditor.Handles.Label(labelPosition, debugInfo);
        }
#endif
    }

    #endregion
}