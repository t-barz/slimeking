using UnityEngine;
using SlimeKing.Core;

/// <summary>
/// Controlador de comportamento específico para abelhas do tipo Normal.
/// 
/// RESPONSABILIDADES:
/// • Comportamento Idle com efeito de bouncing (movimento pelo vento)
/// • Integração com NPCController e NPCBehaviorController
/// • Padrões de movimento únicos para abelhas normais
/// 
/// DEPENDÊNCIAS:
/// • NPCController: Para controle de movimento
/// • NPCBehaviorController: Para estados e detecção
/// • Transform: Para posicionamento e movimento
/// </summary>
[RequireComponent(typeof(NPCController))]
public class NormalBeeBehaviorController : MonoBehaviour
{
    #region Inspector Configuration

    [Header("🐝 Comportamento Idle - Bouncing")]
    [Tooltip("Amplitude do movimento horizontal (distância máxima do centro)")]
    [SerializeField, Range(0.5f, 3f)] private float bounceAmplitude = 1.5f;

    [Tooltip("Velocidade do movimento bouncing (ciclos por segundo)")]
    [SerializeField, Range(0.1f, 2f)] private float bounceSpeed = 0.5f;

    [Tooltip("Amplitude do movimento vertical (oscilação sutil)")]
    [SerializeField, Range(0f, 1f)] private float verticalOscillation = 0.3f;

    [Tooltip("Velocidade da oscilação vertical")]
    [SerializeField, Range(0.1f, 3f)] private float verticalSpeed = 1f;

    [Header("🎯 Configurações de Posicionamento")]
    [Tooltip("Distância máxima da posição inicial antes de retornar")]
    [SerializeField, Range(2f, 10f)] private float maxDistanceFromCenter = 3f;

    [Header("🔍 Sistema de Detecção e Combate")]
    [Tooltip("Raio de detecção do player")]
    [SerializeField, Range(1f, 10f)] private float detectionRange = 4f;

    [Tooltip("Raio de ataque quando próximo ao player")]
    [SerializeField, Range(0.5f, 3f)] private float attackRange = 1.5f;

    [Tooltip("Velocidade de perseguição quando detecta o player")]
    [SerializeField, Range(1f, 8f)] private float chaseSpeed = 3f;

    [Tooltip("Velocidade de ataque quando próximo")]
    [SerializeField, Range(0.5f, 5f)] private float attackSpeed = 2f;

    [Tooltip("Tempo entre ataques em segundos")]
    [SerializeField, Range(0.5f, 3f)] private float attackCooldown = 1f;

    [Header("🎬 Efeitos de Detecção")]
    [Tooltip("Prefab do efeito visual a ser instanciado quando detecta o player")]
    [SerializeField] private GameObject vfxDetectionPrefab;

    [Tooltip("Som reproduzido quando detecta o player")]
    [SerializeField] private AudioClip sfxDetection;

    [Tooltip("Volume do som de detecção")]
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    [Tooltip("Offset da posição do VFX de detecção em relação à abelha (Y positivo = acima)")]
    [SerializeField] private Vector3 vfxDetectionOffset = new Vector3(0f, 1f, 0f);

    [Header("🔧 Debug")]
    [Tooltip("Habilita logs detalhados no Console")]
    [SerializeField] private bool enableDebugLogs = false;

    [Tooltip("Mostra gizmos de debug no Scene View")]
    [SerializeField] private bool enableDebugGizmos = true;

    #endregion

    #region Private Variables

    // === COMPONENTES ===
    private NPCController _npcController;
    private NPCBehaviorController _behaviorController;
    private Transform _transform;
    private Transform _playerTransform;

    // === ESTADO DO MOVIMENTO ===
    private Vector3 _centerPosition;      // Posição central de referência
    private float _bounceTime = 0f;       // Tempo para cálculo do bouncing
    private Vector2 _currentDirection;    // Direção atual do movimento
    private bool _isInitialized = false;

    // === SISTEMA DE DETECÇÃO E COMBATE ===
    private bool _playerDetected = false;
    private bool _isChasing = false;
    private bool _isAttacking = false;
    private float _lastAttackTime = 0f;
    private float _playerDistance = float.MaxValue;

    // === SISTEMA DE EFEITOS ===
    private GameObject _currentVfxDetection;  // Instância atual do VFX
    private AudioSource _audioSource;        // Componente de áudio

    // === ESTADOS CUSTOMIZADOS ===
    public enum BeeState
    {
        Idle,           // Bouncing normal
        Chasing,        // Perseguindo player
        Attacking,      // Atacando player
        Returning       // Retornando ao centro (futuro)
    }

    private BeeState _currentBeeState = BeeState.Idle;

    // === CONFIGURAÇÕES DE MOVIMENTO ===
    private const float MOVEMENT_SMOOTHNESS = 2f;  // Suavidade do movimento
    private const float DIRECTION_CHANGE_THRESHOLD = 0.1f;  // Limiar para mudança de direção

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Obtém componentes essenciais
        _npcController = GetComponent<NPCController>();
        _behaviorController = GetComponent<NPCBehaviorController>();
        _transform = transform;

        // Encontra o player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerTransform = playerObject.transform;
        }

        // Configura ou cria AudioSource
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1f; // 3D sound
            _audioSource.volume = sfxVolume;
        }

        if (_npcController == null)
        {
            Debug.LogError($"[NormalBeeBehavior] {gameObject.name} - NPCController não encontrado!");
        }

        if (_playerTransform == null)
        {
            Debug.LogWarning($"[NormalBeeBehavior] {gameObject.name} - Player não encontrado!");
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Componentes inicializados");
        }
    }

    private void Start()
    {
        InitializeBehavior();
    }

    private void Update()
    {
        if (!_isInitialized) return;

        // Atualiza detecção do player
        UpdatePlayerDetection();

        // Atualiza comportamento baseado no estado atual
        switch (_currentBeeState)
        {
            case BeeState.Idle:
                UpdateIdleBouncing();
                break;
            case BeeState.Chasing:
                UpdateChasing();
                break;
            case BeeState.Attacking:
                UpdateAttacking();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        if (!enableDebugGizmos || !_isInitialized) return;

        // Desenha área de bouncing
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_centerPosition, new Vector3(bounceAmplitude * 2f, verticalOscillation * 2f, 0f));

        // Desenha raio de detecção
        Gizmos.color = _playerDetected ? Color.orange : Color.cyan;
        Gizmos.DrawWireSphere(_transform.position, detectionRange);

        // Desenha raio de ataque
        Gizmos.color = _isAttacking ? Color.red : new Color(1f, 0.5f, 0.5f, 0.5f);
        Gizmos.DrawWireSphere(_transform.position, attackRange);

        // Desenha limite máximo
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_centerPosition, maxDistanceFromCenter);

        // Desenha posição atual e direção
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_transform.position, 0.1f);

        if (_currentDirection.magnitude > 0.1f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_transform.position, _currentDirection * 0.5f);
        }

        // Desenha linha para o player se detectado
        if (_playerDetected && _playerTransform != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_transform.position, _playerTransform.position);
        }
    }

    #endregion

    #region Player Detection and State Management

    private void UpdatePlayerDetection()
    {
        if (_playerTransform == null) return;

        // Calcula distância ao player
        _playerDistance = Vector3.Distance(_transform.position, _playerTransform.position);

        // Verifica se player está no raio de detecção
        bool wasDetected = _playerDetected;
        _playerDetected = _playerDistance <= detectionRange;

        // Gerencia transições de estado
        if (_playerDetected && !wasDetected)
        {
            // Player entrou no raio de detecção
            ChangeToChaseState();
            TriggerDetectionEffects(); // Adiciona efeitos de detecção
        }
        else if (!_playerDetected && wasDetected)
        {
            // Player saiu do raio de detecção
            ChangeToIdleState();
        }
        else if (_playerDetected)
        {
            // Player ainda detectado, verifica transição attack/chase
            if (_playerDistance <= attackRange && _currentBeeState == BeeState.Chasing)
            {
                ChangeToAttackState();
            }
            else if (_playerDistance > attackRange && _currentBeeState == BeeState.Attacking)
            {
                ChangeToChaseState();
            }
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Distance: {_playerDistance:F2}, " +
                     $"Detected: {_playerDetected}, State: {_currentBeeState}");
        }
    }

    private void ChangeToIdleState()
    {
        _currentBeeState = BeeState.Idle;
        _isChasing = false;
        _isAttacking = false;

        // Remove efeito visual de detecção
        RemoveDetectionEffects();

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Mudou para estado IDLE");
        }
    }

    private void ChangeToChaseState()
    {
        _currentBeeState = BeeState.Chasing;
        _isChasing = true;
        _isAttacking = false;

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Mudou para estado CHASE");
        }
    }

    private void ChangeToAttackState()
    {
        _currentBeeState = BeeState.Attacking;
        _isChasing = false;
        _isAttacking = true;

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Mudou para estado ATTACK");
        }
    }

    #endregion

    #region Behavior Updates

    private void UpdateChasing()
    {
        if (_playerTransform == null) return;

        // Calcula direção para o player
        Vector3 directionToPlayer = (_playerTransform.position - _transform.position).normalized;

        // Aplica velocidade de perseguição
        _currentDirection = Vector2.Lerp(_currentDirection, directionToPlayer,
            Time.deltaTime * MOVEMENT_SMOOTHNESS);

        // Define velocidade de perseguição no NPC Controller
        _npcController?.SetMoveDirection(_currentDirection * chaseSpeed);

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Perseguindo player: direction={directionToPlayer}");
        }
    }

    private void UpdateAttacking()
    {
        if (_playerTransform == null) return;

        // Verifica se pode atacar (cooldown)
        if (Time.time >= _lastAttackTime + attackCooldown)
        {
            PerformAttack();
        }

        // Movimento de ataque mais agressivo e próximo
        Vector3 directionToPlayer = (_playerTransform.position - _transform.position).normalized;
        _currentDirection = Vector2.Lerp(_currentDirection, directionToPlayer,
            Time.deltaTime * MOVEMENT_SMOOTHNESS * 1.5f);

        // Velocidade de ataque (pode ser diferente da perseguição)
        _npcController?.SetMoveDirection(_currentDirection * attackSpeed);

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Atacando player: direction={directionToPlayer}");
        }
    }

    private void PerformAttack()
    {
        _lastAttackTime = Time.time;

        // TODO: Implementar lógica de dano ao player
        // Por enquanto só registra o ataque
        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - ATAQUE EXECUTADO!");
        }

        // Aqui você pode adicionar:
        // - Efeitos visuais de ataque
        // - Aplicar dano ao player
        // - Sons de ataque
        // - Animações especiais
    }

    #endregion

    #region Initialization

    private void InitializeBehavior()
    {
        // Define posição central baseada na posição inicial
        _centerPosition = _transform.position;

        // Randomiza o tempo inicial para variar o comportamento entre abelhas
        _bounceTime = Random.Range(0f, 2f * Mathf.PI);

        _isInitialized = true;

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Comportamento inicializado na posição {_centerPosition}");
        }
    }

    #endregion

    #region State Detection

    private bool IsInIdleState()
    {
        // Se não tem behaviorController, assume estado Idle
        if (_behaviorController == null) return true;

        // Por enquanto, verifica se não está detectando player (comportamento Idle)
        // TODO: Implementar acesso público ao estado no NPCBehaviorController
        return !_behaviorController.IsPlayerDetected();
    }

    #endregion

    #region Idle Bouncing Behavior

    private void UpdateIdleBouncing()
    {
        // Incrementa tempo do bouncing
        _bounceTime += Time.deltaTime * bounceSpeed;

        // Calcula movimento horizontal (bouncing principal)
        float horizontalOffset = Mathf.Sin(_bounceTime) * bounceAmplitude;

        // Calcula movimento vertical (oscilação sutil)
        float verticalOffset = Mathf.Sin(_bounceTime * verticalSpeed) * verticalOscillation;

        // Calcula posição alvo
        Vector3 targetPosition = _centerPosition + new Vector3(horizontalOffset, verticalOffset, 0f);

        // Verifica se não está muito longe da posição central
        float distanceFromCenter = Vector3.Distance(targetPosition, _centerPosition);
        if (distanceFromCenter > maxDistanceFromCenter)
        {
            // Se muito longe, move suavemente de volta ao centro
            targetPosition = Vector3.MoveTowards(targetPosition, _centerPosition,
                (distanceFromCenter - maxDistanceFromCenter) * Time.deltaTime);
        }

        // Calcula direção de movimento para o NPCController
        Vector2 moveDirection = (targetPosition - _transform.position).normalized;

        // Aplica suavização para movimento mais natural
        _currentDirection = Vector2.Lerp(_currentDirection, moveDirection,
            Time.deltaTime * MOVEMENT_SMOOTHNESS);

        // Aplica movimento apenas se a direção é significativa
        if (_currentDirection.magnitude > DIRECTION_CHANGE_THRESHOLD)
        {
            _npcController?.SetMoveDirection(_currentDirection);
        }
        else
        {
            _npcController?.SetMoveDirection(Vector2.zero);
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Bouncing: time={_bounceTime:F2}, " +
                     $"horizontal={horizontalOffset:F2}, vertical={verticalOffset:F2}, " +
                     $"direction={_currentDirection}");
        }
    }

    #endregion

    #region Detection Effects

    /// <summary>
    /// Ativa efeitos visuais e sonoros quando o player é detectado
    /// </summary>
    private void TriggerDetectionEffects()
    {
        if (_currentVfxDetection != null) return; // Já tem VFX ativo

        // Instancia VFX de detecção se definido
        if (vfxDetectionPrefab != null)
        {
            // Calcula posição com offset
            Vector3 vfxPosition = _transform.position + vfxDetectionOffset;
            _currentVfxDetection = Instantiate(vfxDetectionPrefab, vfxPosition, Quaternion.identity, _transform);
            _currentVfxDetection.name = "vfxDetection";

            if (enableDebugLogs)
            {
                Debug.Log($"[NormalBeeBehavior] {gameObject.name} - VFX de detecção instanciado na posição {vfxPosition}");
            }
        }

        // Reproduz som de detecção
        if (sfxDetection != null && _audioSource != null)
        {
            _audioSource.clip = sfxDetection;
            _audioSource.volume = sfxVolume;
            _audioSource.Play();

            if (enableDebugLogs)
            {
                Debug.Log($"[NormalBeeBehavior] {gameObject.name} - SFX de detecção reproduzido");
            }
        }
    }

    /// <summary>
    /// Remove efeitos visuais de detecção
    /// </summary>
    private void RemoveDetectionEffects()
    {
        if (_currentVfxDetection != null)
        {
            Destroy(_currentVfxDetection);
            _currentVfxDetection = null;

            if (enableDebugLogs)
            {
                Debug.Log($"[NormalBeeBehavior] {gameObject.name} - VFX de detecção removido");
            }
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// Redefine a posição central do bouncing
    /// </summary>
    /// <param name="newCenter">Nova posição central</param>
    public void SetBounceCenter(Vector3 newCenter)
    {
        _centerPosition = newCenter;

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Centro do bouncing alterado para {_centerPosition}");
        }
    }

    /// <summary>
    /// Obtém a posição central atual do bouncing
    /// </summary>
    /// <returns>Posição central</returns>
    public Vector3 GetBounceCenter()
    {
        return _centerPosition;
    }

    /// <summary>
    /// Força um reset do comportamento de bouncing
    /// </summary>
    public void ResetBouncing()
    {
        _bounceTime = Random.Range(0f, 2f * Mathf.PI);
        _centerPosition = _transform.position;

        if (enableDebugLogs)
        {
            Debug.Log($"[NormalBeeBehavior] {gameObject.name} - Bouncing resetado");
        }
    }

    #endregion
}
