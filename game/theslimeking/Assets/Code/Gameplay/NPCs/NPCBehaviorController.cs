using UnityEngine;
using System;
using System.Collections.Generic;
using SlimeKing.Core;

/// <summary>
/// Sistema de comportamento avançado para NPCs com máquina de estados otimizada.
/// 
/// RESPONSABILIDADES:
/// • Máquina de estados (Idle/Patrol/Alert/Chase/Attack/Return)
/// • Sistema de detecção com cone de visão e integração com stealth
/// • Configurações específicas por tipo de NPC
/// • Sistema LOD para performance em cenários complexos
/// • Integração com áudio e sistema de eventos
/// • Preparado para migração futura ao Job System
/// 
/// DEPENDÊNCIAS:
/// • NPCController: Para movimento e animações
/// • NPCAttributesHandler: Para tipo de NPC e atributos
/// • NPCManager: Para verificação de relacionamento
/// • PlayerController: Para detecção e stealth
/// 
/// OTIMIZAÇÕES:
/// • StringToHash para Animator parameters
/// • Squared distances para performance
/// • NonAlloc Physics queries
/// • Jump table state machine
/// • Object pooling para arrays
/// • Sistema LOD baseado em distância
/// </summary>
[RequireComponent(typeof(NPCController), typeof(NPCAttributesHandler))]
public class NPCBehaviorController : MonoBehaviour
{
    #region Inspector Configuration

    [Header("⚙️ Sistema de Comportamento")]
    [Tooltip("Habilita o sistema de comportamento avançado (detecção, perseguição, ataque)")]
    [SerializeField] private bool enableBehaviorSystem = true;

    [Tooltip("Força o comportamento agressivo para testes (ignora relacionamento)")]
    [SerializeField] private bool forceAggressiveBehavior = true; // Habilitado para testes

    [Header("🔍 Sistema de Detecção")]
    [Tooltip("LayerMask contendo apenas o Player para otimizar detecção")]
    [SerializeField] private LayerMask playerLayerMask = 128; // Layer 7 = Player

    [Tooltip("LayerMask para obstáculos que bloqueiam linha de visão (Scenario/Walls)")]
    [SerializeField] private LayerMask obstacleLayerMask = 64; // Layer 6 = Scenario

    [Tooltip("Desabilita verificação de linha de visão para testes (permite detecção através de obstáculos)")]
    [SerializeField] private bool disableLineOfSightCheck = false;

    [Header("👥 Comportamento em Grupo")]
    [Tooltip("Habilita comportamento em grupo (alerta NPCs próximos)")]
    [SerializeField] private bool enableGroupBehavior = false;

    [Tooltip("Alcance para alertar outros NPCs do grupo")]
    [SerializeField] private float groupAlertRange = 10f;

    [Header("🔊 Sistema de Áudio")]
    [Tooltip("Volume dos sons reproduzidos por este NPC")]
    [SerializeField, Range(0f, 1f)] private float audioVolume = 1f;

    [Header("📊 Sistema LOD")]
    [Tooltip("Distância máxima para comportamento completo")]
    [SerializeField] private float maxBehaviorDistance = 20f;

    [Tooltip("Distância para reduzir update rate")]
    [SerializeField] private float reducedUpdateDistance = 15f;

    [Tooltip("Distância para desativar comportamento")]
    [SerializeField] private float disableDistance = 25f;

    [Header("🔧 Ferramentas de Debug")]
    [Tooltip("Habilita logs detalhados no Console")]
    [SerializeField] private bool enableDetailedLogs = true; // Habilitado para debug de movimento

    [Tooltip("Mostra gizmos de debug no Scene View")]
    [SerializeField] private bool enableDebugGizmos = true;

    [Tooltip("Mostra debug apenas quando selecionado")]
    [SerializeField] private bool debugOnlyWhenSelected = true;

    [Header("🔍 Configurações de Detecção")]
    [Tooltip("Sobrescreve o raio de detecção padrão do NPC (0 = usar padrão)")]
    [SerializeField, Range(0f, 20f)] private float customVisionRange = 1f;

    [Tooltip("Sobrescreve o ângulo de visão padrão do NPC (0 = usar padrão, 360 = círculo completo)")]
    [SerializeField, Range(0f, 360f)] private float customVisionAngle = 0f;

    [Tooltip("Multiplica a velocidade de rotação do cone de visão")]
    [SerializeField, Range(0.1f, 20f)] private float visionSpeedMultiplier = 1f;

    #endregion

    #region Enums

    /// <summary>
    /// Estados da máquina de comportamento NPC.
    /// Ordem otimizada para jump table performance.
    /// </summary>
    public enum NPCBehaviorState
    {
        Idle = 0,       // Comportamento padrão, sem alvos detectados
        Patrol = 1,     // Patrulhando área designada (futuro)
        Alert = 2,      // Detectou algo suspeito, investigando
        Chase = 3,      // Perseguindo o player ativamente
        Attack = 4,     // Atacando o player
        Return = 5      // Retornando à posição/patrulha inicial
    }

    /// <summary>
    /// Níveis de LOD para otimização de performance.
    /// </summary>
    public enum LODLevel
    {
        Full = 0,       // Update completo (60 FPS)
        Medium = 1,     // Update reduzido (30 FPS)
        Low = 2,        // Update baixo (10 FPS)
        Minimal = 3,    // Update mínimo (1 FPS)
        Disabled = 4    // Completamente desabilitado
    }

    #endregion

    #region Configuration Structs

    /// <summary>
    /// Configuração de comportamento específica por tipo de NPC.
    /// Usando struct para otimização de memória.
    /// </summary>
    [System.Serializable]
    public struct NPCBehaviorConfig
    {
        [Header("Detecção")]
        public float visionRange;           // Alcance de visão
        public float visionAngle;           // Ângulo de visão em graus
        public float hearingRange;          // Alcance de audição

        [Header("Combate")]
        public float attackRange;           // Alcance de ataque
        public float attackCooldown;        // Tempo entre ataques
        public float chaseSpeed;            // Velocidade durante perseguição

        [Header("Estados")]
        public float alertDuration;         // Tempo em estado de alerta
        public float returnTimeout;         // Tempo para retornar após perder alvo

        [Header("Áudio")]
        public AudioClip alertSound;        // Som de alerta
        public AudioClip chaseSound;        // Som de perseguição
        public AudioClip attackSound;       // Som de ataque
        public AudioClip returnSound;       // Som de retorno

        // Configurações específicas por tipo
        public static NPCBehaviorConfig GetConfig(NPCType npcType)
        {
            if (!ConfigCache.ContainsKey(npcType))
            {
                ConfigCache[npcType] = CreateDefaultConfig(npcType);
            }
            return ConfigCache[npcType];
        }

        private static NPCBehaviorConfig CreateDefaultConfig(NPCType npcType)
        {
            return npcType switch
            {
                NPCType.Abelha => new NPCBehaviorConfig
                {
                    visionRange = 8f,
                    visionAngle = 60f,
                    hearingRange = 5f,
                    attackRange = 0.5f,
                    attackCooldown = 2f,
                    chaseSpeed = 3.5f,
                    alertDuration = 3f,
                    returnTimeout = 5f
                },
                NPCType.SlimeAmarelo or NPCType.SlimeAzul or NPCType.SlimeVerde or NPCType.SlimeVermelho => new NPCBehaviorConfig
                {
                    visionRange = 6f,
                    visionAngle = 45f,
                    hearingRange = 4f,
                    attackRange = 1.5f,
                    attackCooldown = 1.5f,
                    chaseSpeed = 2.5f,
                    alertDuration = 2f,
                    returnTimeout = 4f
                },
                NPCType.Helpy => new NPCBehaviorConfig
                {
                    visionRange = 10f,
                    visionAngle = 90f,
                    hearingRange = 6f,
                    attackRange = 1f,
                    attackCooldown = 1f,
                    chaseSpeed = 4f,
                    alertDuration = 4f,
                    returnTimeout = 6f
                },
                _ => new NPCBehaviorConfig
                {
                    visionRange = 5f,
                    visionAngle = 45f,
                    hearingRange = 3f,
                    attackRange = 1.5f,
                    attackCooldown = 2f,
                    chaseSpeed = 2f,
                    alertDuration = 2f,
                    returnTimeout = 4f
                }
            };
        }
    }

    #endregion

    #region Static Cache

    // Cache estático compartilhado entre instâncias para otimização de memória
    private static readonly Dictionary<NPCType, NPCBehaviorConfig> ConfigCache = new Dictionary<NPCType, NPCBehaviorConfig>();

    // Arrays pooled para Physics queries (evita garbage collection)
    private static readonly Collider2D[] OverlapResultsPool = new Collider2D[16];

    #endregion

    #region Private Variables

    // === COMPONENTES ESSENCIAIS ===
    private NPCController _npcController;
    private NPCAttributesHandler _attributesHandler;
    private Animator _animator;
    private Transform _playerTransform;

    // === SISTEMA DE ESTADOS ===
    private NPCBehaviorState _currentState = NPCBehaviorState.Idle;
    private NPCBehaviorState _previousState = NPCBehaviorState.Idle;
    private readonly System.Action[] _stateMethods = new System.Action[6];

    // === DADOS DE CONFIGURAÇÃO ===
    private NPCBehaviorConfig _config;
    private NPCType _npcType;

    // === SISTEMA DE DETECÇÃO ===
    private bool _hasLineOfSight = false;
    private bool _playerDetected = false;
    private bool _isPlayerInStealth = false;
    private Vector2 _lastKnownPlayerPosition;
    private Vector2 _lookDirection = Vector2.down; // Direção que o NPC está olhando

    // === SISTEMA LOD ===
    private LODLevel _currentLOD = LODLevel.Full;
    private float _playerDistanceSqr = float.MaxValue;
    private int _updateCounter = 0;

    // === SISTEMA DE TIMING ===
    private float _nextAttackTime = 0f;
    private float _alertStartTime = 0f;
    private float _returnStartTime = 0f;
    private float _nextUpdateTime = 0f;

    // === POSIÇÕES IMPORTANTES ===
    private Vector3 _initialPosition;
    private Vector3 _lastPatrolPosition;

    // === OTIMIZAÇÃO DE PERFORMANCE ===
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Alert = Animator.StringToHash("Alert");
    private static readonly int IsChasing = Animator.StringToHash("IsChasing");
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    // Cache de parâmetros disponíveis no Animator
    private bool _hasAttackParameter = false;
    private bool _hasAlertParameter = false;
    private bool _hasChasingParameter = false;

    // === CONSTANTES DE PERFORMANCE ===
    private const float DETECTION_UPDATE_RATE = 0.1f;      // 10 FPS para detecção
    private const float LOD_UPDATE_RATE = 0.25f;           // 4 FPS para LOD
    private const float VISION_CONE_DOT_THRESHOLD = 0.7071f; // cos(45°) pré-calculado
    private const float ATTACK_RANGE_BUFFER = 0.2f;        // Buffer para alcance de ataque
    private const float RETURN_THRESHOLD = 2f;             // Distância para considerar "retornado"

    // === DEBUG COLORS (CONSTANTES) ===
    private static readonly Color DEBUG_VISION_IDLE = new Color(0f, 1f, 0f, 0.3f);
    private static readonly Color DEBUG_VISION_ALERT = new Color(1f, 1f, 0f, 0.3f);
    private static readonly Color DEBUG_VISION_CHASE = new Color(1f, 0.5f, 0f, 0.3f);
    private static readonly Color DEBUG_VISION_ATTACK = new Color(1f, 0f, 0f, 0.5f);
    private static readonly Color DEBUG_ATTACK_RANGE = new Color(1f, 0f, 0f, 0.2f);

    // === SISTEMA DE RASTREAMENTO VISUAL ===
    private Vector2 _visionDirection = Vector2.down;           // Direção atual do cone de visão
    private Vector2 _defaultLookDirection = Vector2.down;      // Direção padrão quando não rastreando
    private bool _shouldTrackPlayer = false;                  // Se deve rastrear o player ativamente
    private float _visionRotationSpeed = 24f;                 // Velocidade de rotação do cone (duplicada para máxima responsividade)
    private Vector2 _targetVisionDirection = Vector2.down;    // Direção alvo para interpolação
    private bool _instantRotation = false;                    // Para rotação instantânea em certas situações

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Cache componentes obrigatórios
        _npcController = GetComponent<NPCController>();
        _attributesHandler = GetComponent<NPCAttributesHandler>();
        _animator = GetComponent<Animator>();

        // Verifica parâmetros disponíveis no Animator
        CacheAnimatorParameters();

        // Armazena posição inicial
        _initialPosition = transform.position;
        _lastPatrolPosition = _initialPosition;

        // Inicializa jump table para estados
        InitializeStateMachine();

        // Cache dados de configuração
        _npcType = _attributesHandler.NPCType;
        _config = NPCBehaviorConfig.GetConfig(_npcType);

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} inicializado - Tipo: {_npcType}");
        }
    }

    private void Start()
    {
        // Cache referência do player (performance)
        var playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            _playerTransform = playerController.transform;
            if (enableDetailedLogs)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - PlayerController encontrado: {playerController.gameObject.name}");
                UnityEngine.Debug.Log($"[NPCBehaviorController] Player layer: {playerController.gameObject.layer}, LayerMask configurado: {playerLayerMask.value}");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"[NPCBehaviorController] {gameObject.name} - PLAYER NÃO ENCONTRADO! Verifique se há um GameObject com PlayerController na cena.");
        }

        // Subscreve aos eventos de stealth
        StealthEvents.OnPlayerEnteredStealth += OnPlayerEnteredStealth;
        StealthEvents.OnPlayerExitedStealth += OnPlayerExitedStealth;

        // Inicializa direção do cone de visão
        _defaultLookDirection = Vector2.down; // Direção padrão para frente (sul)
        _visionDirection = _defaultLookDirection;
        _targetVisionDirection = _defaultLookDirection;

        // Inicializa em estado Idle
        ChangeState(NPCBehaviorState.Idle);

        if (enableDetailedLogs)
        {
            float effectiveRange = GetEffectiveVisionRange();
            float effectiveAngle = GetEffectiveVisionAngle();
            float effectiveSpeed = GetEffectiveVisionSpeed();

            string rangeInfo = customVisionRange > 0f ? $"{effectiveRange}m (CUSTOM)" : $"{effectiveRange}m (padrão)";
            string angleInfo = customVisionAngle > 0f ? $"{effectiveAngle}° (CUSTOM)" : $"{effectiveAngle}° (padrão)";

            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} configurado - Visão: {rangeInfo}, Ângulo: {angleInfo}, Velocidade: {effectiveSpeed:F1}x");
        }
    }

    private void Update()
    {
        if (!enableBehaviorSystem) return;

        // Sistema LOD - determina se deve fazer update
        if (!ShouldUpdate()) return;

        // Atualiza sistema de detecção
        UpdateDetection();

        // Atualiza direção do cone de visão
        UpdateVisionDirection();

        // Executa estado atual via jump table
        _stateMethods[(int)_currentState]?.Invoke();

        // Atualiza LOD baseado na distância
        UpdateLODSystem();
    }

    private void OnEnable()
    {
        // Reabilita comportamento quando objeto fica ativo
        if (_playerTransform == null)
        {
            var playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                _playerTransform = playerController.transform;
            }
        }
    }

    private void OnDisable()
    {
        // Para comportamento quando objeto fica inativo
        if (_currentState == NPCBehaviorState.Chase || _currentState == NPCBehaviorState.Attack)
        {
            ChangeState(NPCBehaviorState.Idle);
        }
    }

    private void OnDestroy()
    {
        // Remove subscrições de eventos
        StealthEvents.OnPlayerEnteredStealth -= OnPlayerEnteredStealth;
        StealthEvents.OnPlayerExitedStealth -= OnPlayerExitedStealth;
    }

    #endregion

    #region State Machine Initialization

    /// <summary>
    /// Inicializa jump table da máquina de estados para máxima performance.
    /// </summary>
    private void InitializeStateMachine()
    {
        _stateMethods[(int)NPCBehaviorState.Idle] = UpdateIdleState;
        _stateMethods[(int)NPCBehaviorState.Patrol] = UpdatePatrolState;
        _stateMethods[(int)NPCBehaviorState.Alert] = UpdateAlertState;
        _stateMethods[(int)NPCBehaviorState.Chase] = UpdateChaseState;
        _stateMethods[(int)NPCBehaviorState.Attack] = UpdateAttackState;
        _stateMethods[(int)NPCBehaviorState.Return] = UpdateReturnState;
    }

    #endregion

    #region Animator Parameter Cache

    /// <summary>
    /// Verifica e faz cache dos parâmetros disponíveis no Animator.
    /// </summary>
    private void CacheAnimatorParameters()
    {
        if (_animator == null) return;

        _hasAttackParameter = HasAnimatorParameter("Attack", AnimatorControllerParameterType.Trigger);
        _hasAlertParameter = HasAnimatorParameter("Alert", AnimatorControllerParameterType.Trigger);
        _hasChasingParameter = HasAnimatorParameter("IsChasing", AnimatorControllerParameterType.Bool);

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Parâmetros do Animator: Attack={_hasAttackParameter}, Alert={_hasAlertParameter}, IsChasing={_hasChasingParameter}");
        }
    }

    /// <summary>
    /// Verifica se o Animator possui um parâmetro específico.
    /// </summary>
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

    #region LOD System

    /// <summary>
    /// Determina se este NPC deve fazer update baseado no sistema LOD.
    /// </summary>
    private bool ShouldUpdate()
    {
        // Sempre processa se não há player
        if (_playerTransform == null) return true;

        // Calcula distância squared para performance
        _playerDistanceSqr = (_playerTransform.position - transform.position).sqrMagnitude;

        // Determina LOD baseado na distância
        LODLevel newLOD = GetLODLevel(_playerDistanceSqr);

        if (newLOD != _currentLOD)
        {
            _currentLOD = newLOD;
            if (enableDetailedLogs)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} LOD mudou para: {_currentLOD}");
            }
        }

        // Verifica se deve fazer update baseado no LOD
        return _currentLOD switch
        {
            LODLevel.Full => true,
            LODLevel.Medium => (_updateCounter % 2) == 0, // 30 FPS
            LODLevel.Low => (_updateCounter % 6) == 0,    // 10 FPS
            LODLevel.Minimal => (_updateCounter % 60) == 0, // 1 FPS
            LODLevel.Disabled => false,
            _ => true
        };
    }

    /// <summary>
    /// Determina o nível LOD baseado na distância squared.
    /// </summary>
    private LODLevel GetLODLevel(float distanceSqr)
    {
        float disableDistSqr = disableDistance * disableDistance;
        float maxBehaviorDistSqr = maxBehaviorDistance * maxBehaviorDistance;
        float reducedUpdateDistSqr = reducedUpdateDistance * reducedUpdateDistance;

        if (distanceSqr >= disableDistSqr) return LODLevel.Disabled;
        if (distanceSqr >= maxBehaviorDistSqr) return LODLevel.Minimal;
        if (distanceSqr >= reducedUpdateDistSqr) return LODLevel.Low;
        if (distanceSqr >= (reducedUpdateDistSqr * 0.5f)) return LODLevel.Medium;

        return LODLevel.Full;
    }

    /// <summary>
    /// Atualiza sistema LOD e contadores.
    /// </summary>
    private void UpdateLODSystem()
    {
        _updateCounter++;
        if (_updateCounter >= 3600) // Reset a cada minuto para evitar overflow
        {
            _updateCounter = 0;
        }
    }

    #endregion

    #region Detection System

    // === CONFIGURAÇÕES CUSTOMIZÁVEIS ===

    /// <summary>
    /// Obtém o raio de visão efetivo (customizado ou padrão).
    /// </summary>
    private float GetEffectiveVisionRange()
    {
        return customVisionRange > 0f ? customVisionRange : _config.visionRange;
    }

    /// <summary>
    /// Obtém o ângulo de visão efetivo (customizado ou padrão).
    /// </summary>
    private float GetEffectiveVisionAngle()
    {
        return customVisionAngle > 0f ? customVisionAngle : _config.visionAngle;
    }

    /// <summary>
    /// Obtém a velocidade de rotação efetiva com multiplicador aplicado.
    /// </summary>
    private float GetEffectiveVisionSpeed()
    {
        return _visionRotationSpeed * visionSpeedMultiplier;
    }

    /// <summary>
    /// Sistema de detecção otimizado com cone de visão e verificação de obstáculos.
    /// </summary>
    private void UpdateDetection()
    {
        // Throttling baseado em LOD
        if (Time.time < _nextUpdateTime) return;

        _nextUpdateTime = Time.time + (_currentLOD switch
        {
            LODLevel.Full => DETECTION_UPDATE_RATE,
            LODLevel.Medium => DETECTION_UPDATE_RATE * 2f,
            LODLevel.Low => DETECTION_UPDATE_RATE * 4f,
            LODLevel.Minimal => DETECTION_UPDATE_RATE * 8f,
            _ => DETECTION_UPDATE_RATE
        });

        _playerDetected = false;
        _hasLineOfSight = false;

        // Early exit se player muito longe ou em LOD mínimo
        if (_playerTransform == null || _currentLOD == LODLevel.Disabled)
        {
            if (enableDetailedLogs && _playerTransform == null)
            {
                UnityEngine.Debug.LogWarning($"[NPCBehaviorController] {gameObject.name} - _playerTransform é null! Player não foi encontrado na cena.");
            }
            return;
        }

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Iniciando detecção. Player pos: {_playerTransform.position}, NPC pos: {transform.position}");
        }

        // Verifica se player está no alcance
        Vector2 playerPosition = _playerTransform.position;
        Vector2 npcPosition = transform.position;
        Vector2 directionToPlayer = playerPosition - npcPosition;

        float effectiveVisionRange = GetEffectiveVisionRange();
        float visionRangeSqr = effectiveVisionRange * effectiveVisionRange;
        if (directionToPlayer.sqrMagnitude > visionRangeSqr)
        {
            if (enableDetailedLogs)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Player fora do alcance. Distância: {directionToPlayer.magnitude:F2}m, Alcance: {effectiveVisionRange}m");
            }
            return;
        }

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Player no alcance! Distância: {directionToPlayer.magnitude:F2}m");
        }        // Verifica cone de visão usando produto escalar otimizado
        if (!IsPlayerInVisionCone(directionToPlayer))
        {
            if (enableDetailedLogs)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Player fora do cone de visão");
            }
            return;
        }

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Player no cone de visão!");
        }

        // Verifica linha de visão (raycast)
        if (!disableLineOfSightCheck && !HasClearLineOfSight(npcPosition, playerPosition))
        {
            if (enableDetailedLogs)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Linha de visão bloqueada por obstáculo");
            }
            return;
        }

        if (enableDetailedLogs)
        {
            if (disableLineOfSightCheck)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Verificação de linha de visão DESABILITADA");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Linha de visão livre!");
            }
        }

        // Player detectado!
        bool wasPlayerDetected = _playerDetected;
        _playerDetected = true;
        _hasLineOfSight = true;
        _lastKnownPlayerPosition = playerPosition;

        // Rotação instantânea na primeira detecção para resposta imediata
        if (!wasPlayerDetected && _shouldTrackPlayer)
        {
            _instantRotation = true;
        }

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - 🎯 PLAYER DETECTADO! Posição: {playerPosition}");
        }

        // Verifica se deve ignorar por stealth
        if (_isPlayerInStealth && IsPlayerEffectivelyHidden())
        {
            _playerDetected = false;
            if (enableDetailedLogs)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} player em stealth - ignorando detecção");
            }
        }
    }

    /// <summary>
    /// Verifica se o player está no cone de visão usando produto escalar otimizado.
    /// </summary>
    private bool IsPlayerInVisionCone(Vector2 directionToPlayer)
    {
        // Usa direção dinâmica do cone de visão (atualizada em UpdateVisionDirection)
        _lookDirection = _visionDirection;

        // Calcula produto escalar normalizado
        float dot = Vector2.Dot(directionToPlayer.normalized, _lookDirection);

        // Compara com threshold baseado no ângulo efetivo
        float effectiveVisionAngle = GetEffectiveVisionAngle();
        float angleThreshold = Mathf.Cos(effectiveVisionAngle * 0.5f * Mathf.Deg2Rad);

        bool inCone = dot >= angleThreshold;

        if (enableDetailedLogs && inCone)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Player no cone de visão! Direção cone: {_lookDirection:F2}, Direção player: {directionToPlayer.normalized:F2}");
        }

        return inCone;
    }

    // === SISTEMA DE RASTREAMENTO VISUAL ===

    /// <summary>
    /// Obtém a direção do cone de visão (dinâmica baseada no estado).
    /// </summary>
    private Vector2 GetVisionDirection()
    {
        if (_shouldTrackPlayer && _playerDetected)
        {
            // Cone aponta para o player
            Vector2 directionToPlayer = (_lastKnownPlayerPosition - (Vector2)transform.position).normalized;
            return directionToPlayer;
        }
        else if (_shouldTrackPlayer && !_playerDetected && (_currentState == NPCBehaviorState.Alert || _currentState == NPCBehaviorState.Chase))
        {
            // Cone aponta para última posição conhecida
            Vector2 directionToLastKnown = (_lastKnownPlayerPosition - (Vector2)transform.position).normalized;
            return directionToLastKnown;
        }

        // Cone em direção padrão baseada na direção visual do NPC
        return GetNPCVisualDirection();
    }

    /// <summary>
    /// Obtém a direção visual do NPC baseada no movimento (apenas para referência).
    /// </summary>
    private Vector2 GetNPCVisualDirection()
    {
        var visualDirection = _npcController.GetCurrentVisualDirection();
        return visualDirection switch
        {
            NPCController.VisualDirection.North => Vector2.up,
            NPCController.VisualDirection.South => Vector2.down,
            NPCController.VisualDirection.Side => _npcController.IsMoving() ?
                (transform.position.x > _lastKnownPlayerPosition.x ? Vector2.left : Vector2.right) : Vector2.down,
            _ => Vector2.down
        };
    }

    /// <summary>
    /// Atualiza a direção do cone de visão com rotação suave ou instantânea.
    /// </summary>
    private void UpdateVisionDirection()
    {
        _targetVisionDirection = GetVisionDirection();

        if (_instantRotation || !_shouldTrackPlayer)
        {
            // Rotação instantânea para mudanças críticas
            _visionDirection = _targetVisionDirection;
            _instantRotation = false; // Reset flag
        }
        else
        {
            // Rotação suave com velocidade customizável
            float baseSpeed = GetEffectiveVisionSpeed();
            float rotationSpeed = _shouldTrackPlayer ? baseSpeed * 1.5f : baseSpeed;
            _visionDirection = Vector2.Lerp(_visionDirection, _targetVisionDirection,
                rotationSpeed * Time.deltaTime);
        }

        // Normaliza para evitar drift
        _visionDirection = _visionDirection.normalized;

        if (enableDetailedLogs && Vector2.Angle(_visionDirection, _targetVisionDirection) > 3f)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Rotacionando cone: {_visionDirection:F2} → {_targetVisionDirection:F2}");
        }
    }

    /// <summary>
    /// Configura o modo de rastreamento baseado no estado.
    /// </summary>
    private void SetVisionTrackingMode(bool shouldTrack)
    {
        if (_shouldTrackPlayer != shouldTrack)
        {
            _shouldTrackPlayer = shouldTrack;

            // Rotação instantânea quando ativar rastreamento para resposta imediata
            if (shouldTrack)
            {
                _instantRotation = true;
            }

            if (enableDetailedLogs)
            {
                string mode = shouldTrack ? "ATIVO" : "DESATIVO";
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Rastreamento visual: {mode}");
            }
        }
    }

    /// <summary>
    /// Obtém a direção que o NPC está olhando baseada na direção visual.
    /// </summary>
    private Vector2 GetNPCLookDirection()
    {
        var visualDirection = _npcController.GetCurrentVisualDirection();
        return visualDirection switch
        {
            NPCController.VisualDirection.North => Vector2.up,
            NPCController.VisualDirection.South => Vector2.down,
            NPCController.VisualDirection.Side => _npcController.IsMoving() ?
                (transform.position.x > _lastKnownPlayerPosition.x ? Vector2.left : Vector2.right) : Vector2.down,
            _ => Vector2.down
        };
    }

    /// <summary>
    /// Verifica linha de visão usando raycast otimizado.
    /// </summary>
    private bool HasClearLineOfSight(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        float distance = direction.magnitude;

        // Raycast otimizado com LayerMask específico
        RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance, obstacleLayerMask);

        bool hasLineOfSight = hit.collider == null;

        if (enableDetailedLogs)
        {
            if (!hasLineOfSight)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - RAYCAST BLOQUEADO por: {hit.collider.gameObject.name} (Layer: {hit.collider.gameObject.layer}) em posição {hit.point}");
                UnityEngine.Debug.Log($"[NPCBehaviorController] ObstacleLayerMask: {obstacleLayerMask.value}, Hit distance: {hit.distance:F2}m");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - Raycast livre! Distância total: {distance:F2}m");
            }
        }

        return hasLineOfSight;
    }

    /// <summary>
    /// Verifica se o player está efetivamente escondido (stealth + cobertura).
    /// </summary>
    private bool IsPlayerEffectivelyHidden()
    {
        // TODO: Implementar verificação mais sofisticada de stealth
        // Por enquanto, considera que stealth sempre esconde o player
        return _isPlayerInStealth;
    }

    #endregion

    #region State Management

    /// <summary>
    /// Muda o estado atual com validação e callbacks.
    /// </summary>
    private void ChangeState(NPCBehaviorState newState)
    {
        if (_currentState == newState) return;

        // Exit do estado anterior
        ExitState(_currentState);

        _previousState = _currentState;
        _currentState = newState;

        // Enter do novo estado
        EnterState(_currentState);

        // Sempre loga mudanças de estado importantes
        UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} mudou estado: {_previousState} → {_currentState}");
    }

    /// <summary>
    /// Lógica de entrada em um estado.
    /// </summary>
    private void EnterState(NPCBehaviorState state)
    {
        switch (state)
        {
            case NPCBehaviorState.Alert:
                _alertStartTime = Time.time;
                PlaySound(_config.alertSound);
                SetAnimatorTrigger(Alert);
                SetVisionTrackingMode(true); // Ativa rastreamento visual

                // TODO: Implementar sistema de grupo
                if (enableGroupBehavior)
                {
                    AlertNearbyNPCs();
                }
                break;

            case NPCBehaviorState.Chase:
                PlaySound(_config.chaseSound);
                SetAnimatorBool(IsChasing, true);
                SetVisionTrackingMode(true); // Mantém rastreamento ativo
                break;

            case NPCBehaviorState.Attack:
                SetVisionTrackingMode(true); // Rastreamento ativo durante ataque
                // Preparar para atacar
                break;

            case NPCBehaviorState.Return:
                _returnStartTime = Time.time;
                PlaySound(_config.returnSound);
                SetAnimatorBool(IsChasing, false);
                SetVisionTrackingMode(false); // Desativa rastreamento
                break;

            case NPCBehaviorState.Idle:
                SetAnimatorBool(IsChasing, false);
                SetVisionTrackingMode(false); // Desativa rastreamento
                break;
        }
    }

    /// <summary>
    /// Lógica de saída de um estado.
    /// </summary>
    private void ExitState(NPCBehaviorState state)
    {
        switch (state)
        {
            case NPCBehaviorState.Chase:
                SetAnimatorBool(IsChasing, false);
                break;

            case NPCBehaviorState.Attack:
                // Limpar estado de ataque
                break;
        }
    }

    #endregion

    #region State Updates

    /// <summary>
    /// Atualização do estado Idle.
    /// </summary>
    private void UpdateIdleState()
    {
        // Verifica se deve ser agressivo
        if (ShouldBeAggressive() && _playerDetected)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} IDLE → ALERT - Player detectado com comportamento agressivo!");
            ChangeState(NPCBehaviorState.Alert);
            return;
        }

        // Para movimento
        SetNPCMovement(Vector2.zero);

        // TODO: Implementar wandering behavior
    }

    /// <summary>
    /// Atualização do estado Patrol (futuro).
    /// </summary>
    private void UpdatePatrolState()
    {
        // TODO: Implementar patrulhamento
        // Por enquanto, volta para Idle
        ChangeState(NPCBehaviorState.Idle);
    }

    /// <summary>
    /// Atualização do estado Alert.
    /// </summary>
    private void UpdateAlertState()
    {
        UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - UpdateAlertState: agressivo={ShouldBeAggressive()}, detectado={_playerDetected}");

        // Se detectou player agressivamente, vai para Chase
        if (ShouldBeAggressive() && _playerDetected)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} ALERT → CHASE - Condições atendidas!");
            ChangeState(NPCBehaviorState.Chase);
            return;
        }

        // Timeout do estado de alerta
        if (Time.time - _alertStartTime >= _config.alertDuration)
        {
            if (_playerDetected)
            {
                // Ainda vê o player, mas não é agressivo
                ChangeState(NPCBehaviorState.Idle);
            }
            else
            {
                // Perdeu o player, retorna
                ChangeState(NPCBehaviorState.Return);
            }
            return;
        }

        // Movimento investigativo
        if (_playerDetected)
        {
            // Move em direção ao player lentamente
            Vector2 directionToPlayer = (_lastKnownPlayerPosition - (Vector2)transform.position).normalized;
            SetNPCMovement(directionToPlayer * 0.5f);

            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} ALERT - Movendo em direção ao Player");
        }
        else
        {
            // Para e observa
            SetNPCMovement(Vector2.zero);
        }
    }

    /// <summary>
    /// Atualização do estado Chase.
    /// </summary>
    private void UpdateChaseState()
    {
        UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - UpdateChaseState: agressivo={ShouldBeAggressive()}, detectado={_playerDetected}");

        // Verifica se ainda deve ser agressivo
        if (!ShouldBeAggressive())
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} CHASE → RETURN - Não é mais agressivo");
            ChangeState(NPCBehaviorState.Return);
            return;
        }

        // Perdeu o player
        if (!_playerDetected)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} CHASE → RETURN - Player não detectado");
            ChangeState(NPCBehaviorState.Return);
            return;
        }

        // Verifica se está no alcance de ataque
        float distanceToPlayerSqr = (_lastKnownPlayerPosition - (Vector2)transform.position).sqrMagnitude;
        float attackRangeSqr = (_config.attackRange + ATTACK_RANGE_BUFFER) * (_config.attackRange + ATTACK_RANGE_BUFFER);

        if (distanceToPlayerSqr <= attackRangeSqr)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} CHASE → ATTACK - Player no alcance de ataque!");
            ChangeState(NPCBehaviorState.Attack);
            return;
        }

        // Move em direção ao player com velocidade de chase
        Vector2 directionToPlayer = (_lastKnownPlayerPosition - (Vector2)transform.position).normalized;

        // Usa a velocidade configurada de chase diretamente
        SetNPCMovement(directionToPlayer);

        float currentDistance = Mathf.Sqrt(distanceToPlayerSqr);
        UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} PERSEGUINDO Player - Distância: {currentDistance:F2}m");
    }

    /// <summary>
    /// Atualização do estado Attack.
    /// </summary>
    private void UpdateAttackState()
    {
        // Para movimento durante ataque
        SetNPCMovement(Vector2.zero);

        // Verifica se ainda deve atacar
        if (!ShouldBeAggressive() || !_playerDetected)
        {
            ChangeState(NPCBehaviorState.Return);
            return;
        }

        // Verifica se player saiu do alcance
        float distanceToPlayerSqr = (_lastKnownPlayerPosition - (Vector2)transform.position).sqrMagnitude;
        float attackRangeSqr = _config.attackRange * _config.attackRange;

        if (distanceToPlayerSqr > attackRangeSqr)
        {
            ChangeState(NPCBehaviorState.Chase);
            return;
        }

        // Executa ataque se cooldown permitir
        if (Time.time >= _nextAttackTime)
        {
            ExecuteAttack();
            _nextAttackTime = Time.time + _config.attackCooldown;
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} ATACOU o Player!");
        }
    }

    /// <summary>
    /// Atualização do estado Return.
    /// </summary>
    private void UpdateReturnState()
    {
        // Se detectou player novamente e deve ser agressivo
        if (ShouldBeAggressive() && _playerDetected)
        {
            ChangeState(NPCBehaviorState.Alert);
            return;
        }

        // Timeout de retorno
        if (Time.time - _returnStartTime >= _config.returnTimeout)
        {
            ChangeState(NPCBehaviorState.Idle);
            return;
        }

        // Move de volta à posição inicial
        Vector2 directionToHome = (_initialPosition - transform.position).normalized;
        float distanceToHomeSqr = (_initialPosition - transform.position).sqrMagnitude;

        if (distanceToHomeSqr <= RETURN_THRESHOLD * RETURN_THRESHOLD)
        {
            ChangeState(NPCBehaviorState.Idle);
            return;
        }

        SetNPCMovement(directionToHome);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Verifica se o NPC deve ter comportamento agressivo.
    /// </summary>
    private bool ShouldBeAggressive()
    {
        if (forceAggressiveBehavior) return true;

        // Verifica relacionamento via NPCManager
        if (NPCManager.Instance != null)
        {
            return NPCManager.Instance.IsHostile(_npcType);
        }

        return false;
    }

    /// <summary>
    /// Define movimento do NPC através do NPCController.
    /// </summary>
    private void SetNPCMovement(Vector2 direction)
    {
        if (_npcController != null)
        {
            // Ajusta velocidade baseada no estado atual
            Vector2 finalDirection = direction;

            if (_currentState == NPCBehaviorState.Chase)
            {
                // Aplica velocidade de chase
                float speedMultiplier = _config.chaseSpeed / _attributesHandler.CurrentSpeed;
                finalDirection = direction * speedMultiplier;
            }

            _npcController.SetMoveDirection(finalDirection);

            if (direction.magnitude > 0.1f)
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - MOVIMENTO: direção={direction:F3}, final={finalDirection:F3}, estado={_currentState}");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} - PARADO (direção zero)");
            }
        }
    }

    /// <summary>
    /// Executa ataque no player.
    /// </summary>
    private void ExecuteAttack()
    {
        SetAnimatorTrigger(Attack);
        PlaySound(_config.attackSound);

        // TODO: Implementar dano ao player
        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} atacou o player!");
        }
    }

    /// <summary>
    /// Alerta NPCs próximos (comportamento em grupo).
    /// </summary>
    private void AlertNearbyNPCs()
    {
        // TODO: Implementar sistema de alertas em grupo
        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} alertou NPCs próximos");
        }
    }

    /// <summary>
    /// Reproduz som com otimizações.
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, audioVolume);
        }
    }

    /// <summary>
    /// Define trigger do Animator com verificação de performance e existência.
    /// </summary>
    private void SetAnimatorTrigger(int triggerHash)
    {
        if (_animator != null && _animator.isActiveAndEnabled)
        {
            // Verifica se o parâmetro existe antes de tentar usar
            bool canUseTrigger = false;

            if (triggerHash == Attack && _hasAttackParameter)
                canUseTrigger = true;
            else if (triggerHash == Alert && _hasAlertParameter)
                canUseTrigger = true;

            if (canUseTrigger)
            {
                _animator.SetTrigger(triggerHash);
            }
            // Silenciosamente ignora se parâmetro não existe
        }
    }

    /// <summary>
    /// Define boolean do Animator com verificação de performance e existência.
    /// </summary>
    private void SetAnimatorBool(int boolHash, bool value)
    {
        if (_animator != null && _animator.isActiveAndEnabled)
        {
            // Verifica se o parâmetro existe antes de tentar usar
            bool canUseBool = false;

            if (boolHash == IsChasing && _hasChasingParameter)
                canUseBool = true;

            if (canUseBool)
            {
                _animator.SetBool(boolHash, value);
            }
            // Silenciosamente ignora se parâmetro não existe
        }
    }

    #endregion

    #region Stealth Event Handlers

    /// <summary>
    /// Handler para quando player entra em stealth.
    /// </summary>
    private void OnPlayerEnteredStealth()
    {
        _isPlayerInStealth = true;

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} detectou player em stealth");
        }
    }

    /// <summary>
    /// Handler para quando player sai de stealth.
    /// </summary>
    private void OnPlayerExitedStealth()
    {
        _isPlayerInStealth = false;

        if (enableDetailedLogs)
        {
            UnityEngine.Debug.Log($"[NPCBehaviorController] {gameObject.name} detectou player fora de stealth");
        }
    }

    #endregion

    #region Debug Visualization

    /// <summary>
    /// Desenha gizmos de debug otimizados.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!enableDebugGizmos) return;
        if (debugOnlyWhenSelected && !UnityEditor.Selection.Contains(gameObject)) return;

        // Use configuração cached se disponível
        var config = _config.visionRange > 0 ? _config : NPCBehaviorConfig.GetConfig(_npcType);

        DrawVisionCone(config);
        DrawAttackRange(config);
        DrawStateInfo();
        DrawLODInfo();
    }

    /// <summary>
    /// Desenha cone de visão colorido baseado no estado.
    /// </summary>
    private void DrawVisionCone(NPCBehaviorConfig config)
    {
        Vector3 position = transform.position;
        Vector3 lookDir = _visionDirection; // Usa direção dinâmica do cone

        // Usa valores efetivos (customizados ou padrão)
        float effectiveRange = GetEffectiveVisionRange();
        float effectiveAngle = GetEffectiveVisionAngle();

        // Cor baseada no estado atual
        Color visionColor = _currentState switch
        {
            NPCBehaviorState.Idle => DEBUG_VISION_IDLE,
            NPCBehaviorState.Alert => DEBUG_VISION_ALERT,
            NPCBehaviorState.Chase => DEBUG_VISION_CHASE,
            NPCBehaviorState.Attack => DEBUG_VISION_ATTACK,
            _ => DEBUG_VISION_IDLE
        };

        Gizmos.color = visionColor;

        // Desenha cone de visão com valores efetivos
        float halfAngle = effectiveAngle * 0.5f;
        Vector3 leftBound = Quaternion.AngleAxis(-halfAngle, Vector3.forward) * lookDir;
        Vector3 rightBound = Quaternion.AngleAxis(halfAngle, Vector3.forward) * lookDir;

        Gizmos.DrawLine(position, position + leftBound * effectiveRange);
        Gizmos.DrawLine(position, position + rightBound * effectiveRange);

        // Desenha arco do cone
        UnityEditor.Handles.color = visionColor;
        UnityEditor.Handles.DrawWireArc(position, Vector3.forward, leftBound, effectiveAngle, effectiveRange);

        // Mostra direção de rastreamento
        if (_shouldTrackPlayer)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(position, position + (Vector3)_visionDirection * effectiveRange * 0.3f);
            Gizmos.DrawWireSphere(position + (Vector3)_visionDirection * effectiveRange * 0.3f, 0.1f);
        }

        // Linha para player se detectado
        if (_playerDetected && _hasLineOfSight)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(position, _lastKnownPlayerPosition);
        }
    }

    /// <summary>
    /// Desenha alcance de ataque.
    /// </summary>
    private void DrawAttackRange(NPCBehaviorConfig config)
    {
        Gizmos.color = DEBUG_ATTACK_RANGE;
        Gizmos.DrawWireSphere(transform.position, config.attackRange);
    }

    /// <summary>
    /// Mostra informações do estado atual.
    /// </summary>
    private void DrawStateInfo()
    {
#if UNITY_EDITOR
        Vector3 labelPosition = transform.position + Vector3.up * 2f;
        string stateInfo = $"Estado: {_currentState}\n" +
                          $"LOD: {_currentLOD}\n" +
                          $"Player: {(_playerDetected ? "Detectado" : "Não detectado")}\n" +
                          $"Stealth: {(_isPlayerInStealth ? "Sim" : "Não")}";

        UnityEditor.Handles.Label(labelPosition, stateInfo);
#endif
    }

    /// <summary>
    /// Mostra informações do sistema LOD.
    /// </summary>
    private void DrawLODInfo()
    {
        if (_playerTransform == null) return;

        // Desenha círculos de LOD
        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, reducedUpdateDistance);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, maxBehaviorDistance);

        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawWireSphere(transform.position, disableDistance);
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// Força uma mudança de estado (para debugging/testing).
    /// </summary>
    public void ForceState(NPCBehaviorState state)
    {
        ChangeState(state);
    }

    /// <summary>
    /// Obtém o estado atual do comportamento.
    /// </summary>
    public NPCBehaviorState GetCurrentState()
    {
        return _currentState;
    }

    /// <summary>
    /// Verifica se o player está sendo detectado.
    /// </summary>
    public bool IsPlayerDetected()
    {
        return _playerDetected;
    }

    /// <summary>
    /// Obtém o nível LOD atual.
    /// </summary>
    public LODLevel GetCurrentLOD()
    {
        return _currentLOD;
    }

    /// <summary>
    /// Obtém a configuração de comportamento para este tipo de NPC.
    /// </summary>
    public NPCBehaviorConfig GetConfig()
    {
        return _config;
    }

    #endregion
}
