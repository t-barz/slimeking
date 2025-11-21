using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SlimeKing.Core
{
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

        [Header("🔧 Ferramentas de Debug")]
        [Tooltip("Habilita logs detalhados no Console para debug de movimento")]
        [SerializeField] private bool enableDebugLogs = false;

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

            // VERIFICAÇÃO CRÍTICA: Rigidbody2D é obrigatório para movimento
            if (_rigidbody == null)
            {
                UnityEngine.Debug.LogError($"[NPCController] {gameObject.name} - RIGIDBODY2D NÃO ENCONTRADO! Adicione um componente Rigidbody2D para que o movimento funcione.");
            }
            else if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - Rigidbody2D encontrado com sucesso");
            }

            // Obtém componente opcional
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _attributesHandler = GetComponent<NPCAttributesHandler>();

            // Armazena posição inicial
            _initialPosition = transform.position;

            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} inicializado na posição {_initialPosition}");
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

            // Debug inicial
            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - Configurações iniciais: canMove={_canMove}, moveSpeed={moveSpeed}, hasRigidbody={_rigidbody != null}");
                if (_rigidbody != null)
                {
                    UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - Rigidbody: bodyType={_rigidbody.bodyType}, gravityScale={_rigidbody.gravityScale}, freezeRotation={_rigidbody.freezeRotation}");
                }
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
                if (enableDebugLogs)
                {
                    UnityEngine.Debug.Log($"[NPCController] {gameObject.name} conectado ao sistema de atributos");
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
                    if (enableDebugLogs)
                    {
                        UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - Velocidade sincronizada: {moveSpeed}");
                    }
                }
            }
        }

        private void InitializeVisualObjects()
        {
            // Auto-detecta objetos filhos se não foram configurados no inspector
            frontObject ??= transform.Find("front")?.gameObject;
            backObject ??= transform.Find("back")?.gameObject;
            sideObject ??= transform.Find("side")?.gameObject;
            vfxFrontObject ??= transform.Find("vfx_front")?.gameObject;
            vfxBackObject ??= transform.Find("vfx_back")?.gameObject;
            vfxSideObject ??= transform.Find("vfx_side")?.gameObject;

            // Define direção inicial como South (frente)
            SetVisualDirection(VisualDirection.South);

            if (enableDebugLogs)
            {
                var visualObjects = new[] { frontObject, backObject, sideObject };
                int objectsFound = visualObjects.Count(obj => obj != null);
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - {objectsFound} objetos visuais encontrados");
            }
        }

        #endregion

        #region Movement System

        private void HandleMovement()
        {
            if (!_canMove)
            {
                if (enableDebugLogs && _moveDirection.magnitude > 0.1f)
                {
                    UnityEngine.Debug.LogWarning($"[NPCController] {gameObject.name} - Movimento BLOQUEADO (_canMove = false)");
                }
                return;
            }

            if (enableDebugLogs && _moveDirection.magnitude > 0.1f)
            {
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - HandleMovement executando. Direção: {_moveDirection:F3}, isMoving: {_isMoving}");
            }

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
            if (_rigidbody == null)
            {
                UnityEngine.Debug.LogError($"[NPCController] {gameObject.name} - RIGIDBODY É NULL! Movimento impossível!");
                return;
            }

            Vector2 targetVelocity = _moveDirection * moveSpeed;
            float currentRate = _isMoving ? acceleration : deceleration;

            Vector2 oldVelocity = _rigidbody.linearVelocity;
            _rigidbody.linearVelocity = Vector2.MoveTowards(
                _rigidbody.linearVelocity,
                targetVelocity,
                currentRate * Time.fixedDeltaTime
            );

            if (enableDebugLogs && _moveDirection.magnitude > 0.1f)
            {
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - ApplySmoothMovement: target={targetVelocity:F3}, old={oldVelocity:F3}, new={_rigidbody.linearVelocity:F3}, moveSpeed={moveSpeed:F2}");
            }
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

            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - Direção alterada: {(_facingRight ? "Direita" : "Esquerda")}");
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

            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[NPCController] {gameObject.name} - Direção visual: {direction}");
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

        /// <summary>
        /// Define a direção de movimento do NPC (usado pelo NPCBehaviorController)
        /// </summary>
        /// <param name="direction">Direção normalizada do movimento</param>
        public void SetMoveDirection(Vector2 direction)
        {
            _moveDirection = direction;
            bool wasMoving = _isMoving;
            _isMoving = direction.magnitude > MOVEMENT_THRESHOLD;

            // Log detalhado sobre o movimento
            if (enableDebugLogs)
            {
                if (!wasMoving && _isMoving)
                {
                    UnityEngine.Debug.Log($"[NPCController] {gameObject.name} COMEÇOU A SE MOVER - Direção: {direction:F3}, Magnitude: {direction.magnitude:F3}, Velocidade: {moveSpeed}, canMove: {_canMove}");
                }
                else if (wasMoving && !_isMoving)
                {
                    UnityEngine.Debug.Log($"[NPCController] {gameObject.name} PAROU DE SE MOVER");
                }
                else if (_isMoving)
                {
                    UnityEngine.Debug.Log($"[NPCController] {gameObject.name} MOVENDO - Direção: {direction:F3}, Velocidade atual: {_rigidbody.linearVelocity.magnitude:F2}, canMove: {_canMove}");
                }
            }
        }        /// <summary>
                 /// Para o movimento do NPC imediatamente
                 /// </summary>
        public void StopMovement()
        {
            _moveDirection = Vector2.zero;
            _isMoving = false;
        }

        /// <summary>
        /// Habilita ou desabilita a capacidade de movimento
        /// </summary>
        /// <param name="canMove">Se o NPC pode se mover</param>
        public void SetMovementEnabled(bool canMove)
        {
            _canMove = canMove;
            if (!canMove)
            {
                StopMovement();
            }
        }

        #endregion
    }
}