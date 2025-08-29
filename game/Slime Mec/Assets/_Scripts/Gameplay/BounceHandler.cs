using UnityEngine;

/// <summary>
/// Controlador para simulação de efeito de quicadas em objetos com sistema de sombra.
/// Quando ativado, lança o objeto em uma direção aleatória com força variável,
/// seguido por quicadas sequenciais com força e intervalo decrescentes.
/// Inclui sistema opcional de sombra que escala conforme a altura simulada.
/// 
/// FUNCIONALIDADES:
/// • Lançamento automático em direção aleatória ao ser instanciado
/// • Força aleatória entre valores mínimo e máximo configuráveis
/// • Sistema de quicadas sequenciais com redução de força configurável
/// • Intervalo decrescente entre quicadas (metade do tempo anterior)
/// • Parada automática após todas as quicadas
/// • Controle de multiplicador vertical para arco de movimento
/// • Auto-destruição opcional após tempo configurado
/// • Sistema de sombra dinâmica que escala com altura simulada
/// 
/// SISTEMA DE SOMBRA:
/// • GameObject filho opcional para representar a sombra
/// • Escala automática baseada na altura simulada do objeto
/// • Posicionamento automático com offset configurável
/// • Funciona sem sombra se não configurada
/// 
/// EXEMPLO DE USO:
/// • Objeto lançado com força 4 em 45°, 2 quicadas, intervalo inicial 0.1s, fator 0.8
/// • T=0.0s: Lança com força 4, sombra em escala máxima
/// • T=0.1s: Primeira quicada com força 3.2, sombra reduzida conforme altura
/// • T=0.15s: Segunda quicada com força 2.56, sombra continua acompanhando
/// • T=0.175s: Para o movimento, sombra volta ao tamanho original
/// 
/// DEPENDÊNCIAS:
/// • Rigidbody2D para física 2D
/// • GameObject filho para sombra (opcional)
/// • Collider para interação com o ambiente (opcional)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class BounceHandler : MonoBehaviour
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

    [Header("🌑 Configurações de Sombra")]
    [Tooltip("GameObject filho que representa a sombra (opcional)")]
    [SerializeField] private GameObject shadowObject;

    [Tooltip("Escala mínima da sombra quando objeto está no ponto mais alto")]
    [SerializeField] private float minShadowScale = 0.5f;

    [Tooltip("Escala máxima da sombra quando objeto está no chão")]
    [SerializeField] private float maxShadowScale = 1.0f;

    [Tooltip("Offset da sombra relativo ao objeto principal")]
    [SerializeField] private Vector2 shadowOffset = new Vector2(0.1f, -0.2f);

    [Tooltip("Velocidade vertical máxima para normalização da sombra")]
    [SerializeField] private float maxSimulatedHeight = 8.0f;

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

    // Variáveis para controle da sombra
    private Vector3 _initialPosition;
    private Vector3 _initialShadowScale;
    private bool _hasShadow = false;
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
            Debug.LogError($"BounceHandler em '{gameObject.name}': Rigidbody2D não encontrado!", this);
        }

        // Inicializa sistema de sombra
        InitializeShadowSystem();
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

    /// <summary>
    /// Atualiza a sombra baseada na posição atual do objeto.
    /// </summary>
    private void Update()
    {
        if (_hasShadow && _hasBeenLaunched)
        {
            UpdateShadowEffect();
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
                Debug.LogWarning($"BounceHandler: Item '{gameObject.name}' já foi lançado!", this);
            return;
        }

        if (_rigidbody2D == null)
        {
            Debug.LogError($"BounceHandler: Não é possível lançar '{gameObject.name}' - Rigidbody2D ausente!", this);
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

        // Salva posição inicial para cálculo da sombra
        _initialPosition = transform.position;

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
            Debug.Log($"BounceHandler: '{gameObject.name}' lançado! " +
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
                Debug.LogWarning($"BounceHandler: Item '{gameObject.name}' já foi lançado!", this);
            return;
        }

        if (_rigidbody2D == null)
        {
            Debug.LogError($"BounceHandler: Não é possível lançar '{gameObject.name}' - Rigidbody2D ausente!", this);
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

        // Salva posição inicial para cálculo da sombra
        _initialPosition = transform.position;

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
            Debug.Log($"BounceHandler: '{gameObject.name}' lançado com parâmetros customizados! " +
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

        // Reseta sombra para estado inicial
        if (_hasShadow && shadowObject != null)
        {
            shadowObject.transform.localScale = _initialShadowScale;
            shadowObject.transform.position = transform.position + (Vector3)shadowOffset;
        }

        if (enableDebugLogs)
            Debug.Log($"BounceHandler: Estado de lançamento resetado para '{gameObject.name}'", this);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Inicializa o sistema de sombra verificando se existe objeto de sombra configurado.
    /// </summary>
    private void InitializeShadowSystem()
    {
        if (shadowObject != null)
        {
            _hasShadow = true;
            _initialShadowScale = shadowObject.transform.localScale;

            if (enableDebugLogs)
            {
                Debug.Log($"BounceHandler: Sistema de sombra inicializado para '{gameObject.name}' com objeto '{shadowObject.name}'", this);
            }
        }
        else
        {
            _hasShadow = false;

            if (enableDebugLogs)
            {
                Debug.Log($"BounceHandler: Nenhum objeto de sombra configurado para '{gameObject.name}'", this);
            }
        }
    }

    /// <summary>
    /// Atualiza o efeito de sombra baseado na velocidade vertical do objeto.
    /// A sombra diminui quando o objeto está subindo (velocidade Y positiva)
    /// e aumenta quando está descendo (velocidade Y negativa).
    /// </summary>
    private void UpdateShadowEffect()
    {
        if (!_hasShadow || shadowObject == null || _rigidbody2D == null)
            return;

        // Obtém a velocidade vertical atual
        float verticalVelocity = _rigidbody2D.linearVelocity.y;

        // Calcula altura simulada baseada na velocidade vertical
        // Velocidade positiva = subindo = altura maior
        // Velocidade negativa = descendo = altura menor
        float simulatedHeight = Mathf.Max(0f, verticalVelocity / maxSimulatedHeight);

        // Normaliza a altura (0 = no chão, 1 = altura máxima)
        float normalizedHeight = Mathf.Clamp01(Mathf.Abs(simulatedHeight));

        // Calcula escala da sombra (inversa à altura)
        // Quando objeto está alto (subindo), sombra fica pequena
        // Quando objeto está baixo (descendo/parado), sombra fica grande
        float shadowScale = Mathf.Lerp(maxShadowScale, minShadowScale, normalizedHeight);

        // Aplica escala mantendo proporção original
        Vector3 newScale = _initialShadowScale * shadowScale;
        shadowObject.transform.localScale = newScale;

        // Atualiza posição da sombra com offset
        Vector3 shadowPosition = transform.position + (Vector3)shadowOffset;
        shadowObject.transform.position = shadowPosition;

        // Log de debug detalhado (apenas se muito verboso)
        if (enableDebugLogs && Time.frameCount % 60 == 0) // Log a cada 60 frames
        {
            Debug.Log($"BounceHandler: Sombra atualizada - VelY: {verticalVelocity:F2}, " +
                     $"Altura Simulada: {simulatedHeight:F2}, Altura Norm: {normalizedHeight:F2}, " +
                     $"Escala: {shadowScale:F2}", this);
        }
    }    /// <summary>
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
            Debug.Log($"BounceHandler: Quicada {_currentBounceIndex}/{bounceCount} - " +
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
                Debug.Log($"BounceHandler: Movimento interrompido para '{gameObject.name}' após as quicadas", this);
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

    /// <summary>
    /// Verifica se o sistema de sombra está ativo.
    /// </summary>
    public bool HasShadow => _hasShadow;

    /// <summary>
    /// GameObject configurado como sombra (pode ser nulo).
    /// </summary>
    public GameObject ShadowObject => shadowObject;

    /// <summary>
    /// Velocidade vertical máxima configurada para normalização da sombra.
    /// </summary>
    public float MaxSimulatedHeight => maxSimulatedHeight;
    #endregion
}
