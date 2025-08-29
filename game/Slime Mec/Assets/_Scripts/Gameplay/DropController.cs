using UnityEngine;

/// <summary>
/// Controlador para simulação de efeito de drop de itens com sistema de quicadas.
/// Quando instanciado, lança o objeto em uma direção aleatória com força variável,
/// seguido por quicadas sequenciais com força e intervalo decrescentes.
/// 
/// FUNCIONALIDADES:
/// • Lançamento automático em direção aleatória ao ser instanciado
/// • Força aleatória entre valores mínimo e máximo configuráveis
/// • Sistema de quicadas sequenciais com redução de força configurável
/// • Intervalo decrescente entre quicadas (metade do tempo anterior)
/// • Parada automática após todas as quicadas
/// • Suporte a diferentes tipos de movimento (2D/3D)
/// • Controle opcional de bounce e fricção
/// 
/// EXEMPLO DE USO:
/// • Objeto lançado com força 4 em 45°, 2 quicadas, intervalo inicial 0.1s, fator 0.8
/// • T=0.0s: Lança com força 4
/// • T=0.1s: Primeira quicada com força 3.2 (4 × 0.8¹)
/// • T=0.15s: Segunda quicada com força 2.56 (4 × 0.8²)
/// • T=0.175s: Para o movimento
/// 
/// DEPENDÊNCIAS:
/// • Rigidbody2D ou Rigidbody para física
/// • Collider para interação com o ambiente
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class DropController : MonoBehaviour
{
    #region Serialized Fields
    [Header("⚡ Configurações de Lançamento")]
    [Tooltip("Força mínima do lançamento")]
    [SerializeField] private float minLaunchForce = 2f;

    [Tooltip("Força máxima do lançamento")]
    [SerializeField] private float maxLaunchForce = 5f;

    [Tooltip("Multiplicador de força vertical (para criar arco de lançamento)")]
    [SerializeField] private float verticalForceMultiplier = 1.2f;

    [Header("🎯 Configurações de Direção")]
    [Tooltip("Ângulo mínimo de lançamento em graus (0 = direita, 90 = cima)")]
    [SerializeField] private float minAngle = 45f;

    [Tooltip("Ângulo máximo de lançamento em graus (0 = direita, 90 = cima)")]
    [SerializeField] private float maxAngle = 135f;

    [Header("🏀 Configurações de Quicadas")]
    [Tooltip("Número de quicadas que o objeto fará após o lançamento inicial")]
    [SerializeField] private int bounceCount = 2;

    [Tooltip("Tempo em segundos até a primeira quicada após o lançamento")]
    [SerializeField] private float timeToBounce = 0.1f;

    [Tooltip("Percentual de redução da força a cada quicada (0.8 = redução de 20%)")]
    [SerializeField][Range(0.1f, 1.0f)] private float forceReductionFactor = 0.8f;

    [Header("⚙️ Configurações Opcionais")]
    [Tooltip("Se verdadeiro, executa o lançamento automaticamente no Start")]
    [SerializeField] private bool launchOnStart = true;

    [Tooltip("Tempo em segundos após o qual o objeto será destruído (0 = nunca)")]
    [SerializeField] private float autoDestroyTime = 0f;

    [Header("🔧 Debug")]
    [Tooltip("Mostra logs de debug no Console")]
    [SerializeField] private bool enableDebugLogs = false;
    #endregion

    #region Private Variables
    private Rigidbody2D _rigidbody2D;
    private bool _hasBeenLaunched = false;

    // Variáveis para controle de quicadas
    private Vector2 _initialLaunchDirection;
    private float _initialLaunchForce;
    private int _currentBounceIndex = 0;
    private float _currentBounceInterval;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Inicializa componentes e valida dependências.
    /// </summary>
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        // Validação de componente
        if (_rigidbody2D == null)
        {
            Debug.LogError($"DropController em '{gameObject.name}': Rigidbody2D não encontrado!", this);
        }
    }

    /// <summary>
    /// Executa lançamento automático se configurado.
    /// </summary>
    private void Start()
    {
        if (launchOnStart)
        {
            LaunchItem();
        }

        // Configura auto-destruição se especificado
        if (autoDestroyTime > 0f)
        {
            Destroy(gameObject, autoDestroyTime);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Lança o item em uma direção aleatória com força aleatória.
    /// Pode ser chamado externamente ou automaticamente no Start.
    /// </summary>
    public void LaunchItem()
    {
        // Evita múltiplos lançamentos
        if (_hasBeenLaunched)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"DropController: Item '{gameObject.name}' já foi lançado!", this);
            return;
        }

        if (_rigidbody2D == null)
        {
            Debug.LogError($"DropController: Não é possível lançar '{gameObject.name}' - Rigidbody2D ausente!", this);
            return;
        }

        // Gera valores aleatórios
        float randomForce = Random.Range(minLaunchForce, maxLaunchForce);
        float randomAngle = Random.Range(minAngle, maxAngle);

        // Converte ângulo para direção vetorial
        Vector2 launchDirection = AngleToVector2(randomAngle);

        // Aplica multiplicador vertical para criar arco mais natural
        launchDirection.y *= verticalForceMultiplier;

        // Normaliza e aplica força
        Vector2 launchVelocity = launchDirection.normalized * randomForce;

        // Salva dados iniciais para o sistema de quicadas
        _initialLaunchDirection = launchDirection.normalized;
        _initialLaunchForce = randomForce;
        _currentBounceIndex = 0;
        _currentBounceInterval = timeToBounce;

        // Aplica impulso ao Rigidbody2D
        _rigidbody2D.AddForce(launchVelocity, ForceMode2D.Impulse);

        // Marca como lançado
        _hasBeenLaunched = true;

        // Inicia sistema de quicadas se configurado
        if (bounceCount > 0 && timeToBounce > 0f)
        {
            Invoke(nameof(ProcessNextBounce), _currentBounceInterval);
        }

        // Log de debug
        if (enableDebugLogs)
        {
            Debug.Log($"DropController: '{gameObject.name}' lançado! " +
                     $"Força: {randomForce:F2}, Ângulo: {randomAngle:F1}°, " +
                     $"Direção: {launchDirection}, Velocidade: {launchVelocity}", this);
        }
    }

    /// <summary>
    /// Lança o item com parâmetros customizados.
    /// </summary>
    /// <param name="force">Força do lançamento</param>
    /// <param name="angle">Ângulo em graus</param>
    public void LaunchItem(float force, float angle)
    {
        if (_hasBeenLaunched)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"DropController: Item '{gameObject.name}' já foi lançado!", this);
            return;
        }

        if (_rigidbody2D == null)
        {
            Debug.LogError($"DropController: Não é possível lançar '{gameObject.name}' - Rigidbody2D ausente!", this);
            return;
        }

        // Converte ângulo para direção vetorial
        Vector2 launchDirection = AngleToVector2(angle);
        launchDirection.y *= verticalForceMultiplier;

        // Salva dados iniciais para o sistema de quicadas
        _initialLaunchDirection = launchDirection.normalized;
        _initialLaunchForce = force;
        _currentBounceIndex = 0;
        _currentBounceInterval = timeToBounce;

        // Aplica força
        Vector2 launchVelocity = launchDirection.normalized * force;
        _rigidbody2D.AddForce(launchVelocity, ForceMode2D.Impulse);

        _hasBeenLaunched = true;

        // Inicia sistema de quicadas se configurado
        if (bounceCount > 0 && timeToBounce > 0f)
        {
            Invoke(nameof(ProcessNextBounce), _currentBounceInterval);
        }

        if (enableDebugLogs)
        {
            Debug.Log($"DropController: '{gameObject.name}' lançado com parâmetros customizados! " +
                     $"Força: {force:F2}, Ângulo: {angle:F1}°", this);
        }
    }

    /// <summary>
    /// Para manualmente o movimento do objeto.
    /// Pode ser chamado a qualquer momento para interromper o movimento.
    /// </summary>
    public void StopMovementManually()
    {
        // Cancela o invoke automático se estiver agendado
        CancelInvoke(nameof(ProcessNextBounce));

        // Para o movimento imediatamente
        StopMovement();
    }

    /// <summary>
    /// Reseta o estado de lançamento, permitindo novo lançamento.
    /// </summary>
    public void ResetLaunch()
    {
        _hasBeenLaunched = false;

        // Cancela invokes agendados
        CancelInvoke(nameof(ProcessNextBounce));

        // Reseta variáveis de quicada
        _currentBounceIndex = 0;
        _initialLaunchDirection = Vector2.zero;
        _initialLaunchForce = 0f;
        _currentBounceInterval = 0f;

        // Restaura o corpo rígido para Dynamic se estava Kinematic
        if (_rigidbody2D != null)
        {
            _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
        }

        if (enableDebugLogs)
            Debug.Log($"DropController: Estado de lançamento resetado para '{gameObject.name}'", this);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Processa a próxima quicada no sistema de quicadas sequenciais.
    /// Cada quicada tem força reduzida conforme o fator configurável e ocorre na metade do tempo da anterior.
    /// </summary>
    private void ProcessNextBounce()
    {
        if (_rigidbody2D == null)
            return;

        _currentBounceIndex++;

        // Verifica se ainda há quicadas para processar
        if (_currentBounceIndex > bounceCount)
        {
            // Todas as quicadas foram processadas, para o movimento
            StopMovement();
            return;
        }

        // Calcula a força da quicada atual usando o fator de redução configurável
        // Força atual = força inicial * (fator_de_redução)^índice_da_quicada
        float currentBounceForce = _initialLaunchForce * Mathf.Pow(forceReductionFactor, _currentBounceIndex);

        // Aplica a força na mesma direção do lançamento inicial
        Vector2 bounceVelocity = _initialLaunchDirection * currentBounceForce;
        _rigidbody2D.AddForce(bounceVelocity, ForceMode2D.Impulse);

        // Calcula o próximo intervalo (metade do tempo anterior)
        _currentBounceInterval = timeToBounce / Mathf.Pow(2f, _currentBounceIndex);

        // Log de debug
        if (enableDebugLogs)
        {
            float reductionPercentage = (1f - forceReductionFactor) * 100f;
            Debug.Log($"DropController: Quicada {_currentBounceIndex}/{bounceCount} - " +
                     $"Força: {currentBounceForce:F2} (redução {reductionPercentage:F0}%), " +
                     $"Próximo intervalo: {_currentBounceInterval:F3}s", this);
        }

        // Agenda a próxima quicada ou para o movimento se for a última
        if (_currentBounceIndex < bounceCount)
        {
            Invoke(nameof(ProcessNextBounce), _currentBounceInterval);
        }
        else
        {
            // Agenda a parada do movimento após o último intervalo
            Invoke(nameof(StopMovement), _currentBounceInterval);
        }
    }

    /// <summary>
    /// Para completamente o movimento do objeto.
    /// Chamado automaticamente após as quicadas ou manualmente.
    /// </summary>
    private void StopMovement()
    {
        if (_rigidbody2D != null)
        {
            // Para toda velocidade linear e angular
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;

            // Opcional: tornar o objeto kinematic para evitar que se mova novamente
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

            if (enableDebugLogs)
            {
                Debug.Log($"DropController: Movimento interrompido para '{gameObject.name}' após as quicadas", this);
            }
        }
    }

    /// <summary>
    /// Converte um ângulo em graus para um Vector2 direcionado.
    /// </summary>
    /// <param name="angleInDegrees">Ângulo em graus</param>
    /// <returns>Vector2 normalizado na direção do ângulo</returns>
    private Vector2 AngleToVector2(float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
    }
    #endregion

    #region Properties
    /// <summary>
    /// Verifica se o item já foi lançado.
    /// </summary>
    public bool HasBeenLaunched => _hasBeenLaunched;

    /// <summary>
    /// Força mínima configurada para lançamento.
    /// </summary>
    public float MinLaunchForce => minLaunchForce;

    /// <summary>
    /// Força máxima configurada para lançamento.
    /// </summary>
    public float MaxLaunchForce => maxLaunchForce;

    /// <summary>
    /// Verifica se o movimento foi interrompido (objeto está kinematic).
    /// </summary>
    public bool IsMovementStopped => _rigidbody2D != null && _rigidbody2D.bodyType == RigidbodyType2D.Kinematic;

    /// <summary>
    /// Número de quicadas configuradas.
    /// </summary>
    public int BounceCount => bounceCount;

    /// <summary>
    /// Tempo configurado até a primeira quicada.
    /// </summary>
    public float TimeToBounce => timeToBounce;

    /// <summary>
    /// Fator de redução da força a cada quicada (0.8 = redução de 20%).
    /// </summary>
    public float ForceReductionFactor => forceReductionFactor;

    /// <summary>
    /// Índice da quicada atual (0 = lançamento inicial).
    /// </summary>
    public int CurrentBounceIndex => _currentBounceIndex;
    #endregion
}
