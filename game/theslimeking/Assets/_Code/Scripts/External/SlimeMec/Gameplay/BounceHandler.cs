using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlimeKing.External.SlimeMec.Gameplay
{
    /// <summary>
    /// Estados possíveis do sistema de quicadas.
    /// </summary>
    public enum BounceState
{
    NotLaunched,        // Objeto criado mas não lançado
    Launching,          // Aplicando força inicial
    Bouncing,           // Executando quicadas
    Stopping,           // Parando movimento
    Stopped,            // Completamente parado
    ReadyForCollection  // Colliders habilitados, pronto para coleta
}

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

    // Gerenciamento de estados
    private BounceState _currentState = BounceState.NotLaunched;

    // Variáveis para controle de quicadas
    private Vector2 _initialLaunchDirection;
    private float _initialLaunchForce;
    private int _currentBounceIndex = 0;
    private float _currentBounceInterval;

    // Variáveis para controle da sombra
    private Vector3 _initialPosition;
    private Vector3 _initialShadowScale;
    private bool _hasShadow = false;

    // Variáveis para controle de colliders
    private Collider2D[] _colliders;
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
            // Debug: Rigidbody2D não encontrado
        }

        // Cachear colliders para performance
        _colliders = GetComponents<Collider2D>();

        // Inicializa sistema de sombra
        InitializeShadowSystem();

        // Desabilita todos os colliders após inicialização
        DisableAllColliders();
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
        // Só atualiza a sombra se o objeto foi lançado e não está parado ou pronto para coleta
        if (_hasShadow && _hasBeenLaunched && 
            _currentState != BounceState.Stopped && 
            _currentState != BounceState.ReadyForCollection)
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
            if (enableDebugLogs)return;
        }

        if (_rigidbody2D == null)
        {return;
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

        // Atualiza estado para Launching
        _currentState = BounceState.Launching;

        // Aplica impulso ao Rigidbody2D
        _rigidbody2D.AddForce(launchVelocity, ForceMode2D.Impulse);

        // Marca como lançado
        _hasBeenLaunched = true;

        // Atualiza estado para Bouncing se há quicadas configuradas
        if (bounceCount > 0 && timeToBounce > 0f)
        {
            _currentState = BounceState.Bouncing;
            Invoke(nameof(ProcessNextBounce), _currentBounceInterval);
        }
        else
        {
            // Se não há quicadas, vai direto para Stopping
            _currentState = BounceState.Stopping;
        }

        // Log de debug
        if (enableDebugLogs)
        {}
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
            if (enableDebugLogs)return;
        }

        if (_rigidbody2D == null)
        {return;
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

        // Atualiza estado para Launching
        _currentState = BounceState.Launching;

        // Aplica força
        Vector2 launchVelocity = launchDirection.normalized * force;
        _rigidbody2D.AddForce(launchVelocity, ForceMode2D.Impulse);

        _hasBeenLaunched = true;

        // Atualiza estado para Bouncing se há quicadas configuradas
        if (bounceCount > 0 && timeToBounce > 0f)
        {
            _currentState = BounceState.Bouncing;
            Invoke(nameof(ProcessNextBounce), _currentBounceInterval);
        }
        else
        {
            // Se não há quicadas, vai direto para Stopping
            _currentState = BounceState.Stopping;
        }

        if (enableDebugLogs)
        {}
    }

    /// <summary>
    /// Para manualmente o movimento do objeto.
    /// Pode ser chamado a qualquer momento para interromper o movimento.
    /// </summary>
    public void StopMovementManually()
    {
        // Atualiza estado para Stopping
        _currentState = BounceState.Stopping;

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

        // Reseta estado para NotLaunched
        _currentState = BounceState.NotLaunched;

        // Cancela invokes agendados
        CancelInvoke(nameof(ProcessNextBounce));
        CancelInvoke(nameof(EnableCollidersAndNotify));

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
        {
            // Launch reset
        }
    }

    /// <summary>
    /// Habilita todos os colliders do objeto.
    /// Método público para controle externo.
    /// </summary>
    public void EnableColliders()
    {
        EnableAllColliders();
    }

    /// <summary>
    /// Desabilita todos os colliders do objeto.
    /// Método público para controle externo.
    /// </summary>
    public void DisableColliders()
    {
        DisableAllColliders();
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
            {}
        }
        else
        {
            _hasShadow = false;

            if (enableDebugLogs)
            {}
        }
    }

    /// <summary>
    /// Atualiza o efeito de sombra baseado na velocidade vertical do objeto.
    /// A sombra diminui quando o objeto está em movimento (subindo ou descendo)
    /// e aumenta quando está parado ou próximo ao chão.
    /// </summary>
    private void UpdateShadowEffect()
    {
        if (!_hasShadow || shadowObject == null)
            return;

        // Verificação de null para _rigidbody2D antes de acessar velocidade
        if (_rigidbody2D == null)
            return;

        // Usa o valor absoluto da velocidade vertical para simular altura
        // Isso garante que a sombra diminui tanto ao subir quanto ao descer,
        // representando a distância do objeto em relação ao chão
        float speed = Mathf.Abs(_rigidbody2D.linearVelocity.y);

        // Normaliza a velocidade para um valor entre 0 e 1
        // 0 = parado (no chão, sombra máxima)
        // 1 = velocidade máxima (altura máxima, sombra mínima)
        float normalizedHeight = Mathf.Clamp01(speed / maxSimulatedHeight);

        // Interpola a escala da sombra baseada na altura normalizada
        // Quando normalizedHeight = 0 (parado), shadowScale = maxShadowScale
        // Quando normalizedHeight = 1 (altura máxima), shadowScale = minShadowScale
        float shadowScale = Mathf.Lerp(maxShadowScale, minShadowScale, normalizedHeight);

        // Aplica escala mantendo proporção original
        Vector3 newScale = _initialShadowScale * shadowScale;
        shadowObject.transform.localScale = newScale;

        // Atualiza posição da sombra com offset
        Vector3 shadowPosition = transform.position + (Vector3)shadowOffset;
        shadowObject.transform.position = shadowPosition;

        // Log de debug detalhado (apenas se muito verboso)
        if (enableDebugLogs && Time.frameCount % 60 == 0) // Log a cada 60 frames
        {}
    }

    /// <summary>
    /// Restaura a sombra ao tamanho máximo (objeto no chão).
    /// Chamado quando o movimento para completamente.
    /// </summary>
    private void ResetShadowToMaxScale()
    {
        if (!_hasShadow || shadowObject == null)
            return;

        // Restaura a sombra ao tamanho máximo
        Vector3 maxScale = _initialShadowScale * maxShadowScale;
        shadowObject.transform.localScale = maxScale;

        // Atualiza posição da sombra com offset
        Vector3 shadowPosition = transform.position + (Vector3)shadowOffset;
        shadowObject.transform.position = shadowPosition;

        if (enableDebugLogs)
        {}
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
            // Todas as quicadas foram processadas, atualiza estado e para o movimento
            _currentState = BounceState.Stopping;
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
            // Debug: Quicada processada
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

            // Atualiza estado para Stopped
            _currentState = BounceState.Stopped;

            // Reseta a sombra ao tamanho máximo quando o movimento para
            ResetShadowToMaxScale();

            if (enableDebugLogs)
            {}

            // Chama método de sincronização com ItemCollectable
            OnMovementStopped();
        }
    }

    /// <summary>
    /// Chamado quando o movimento para completamente.
    /// Sincroniza com ItemCollectable para aguardar o delay de ativação antes de habilitar colliders.
    /// </summary>
    private void OnMovementStopped()
    {
        // Busca componente ItemCollectable no objeto
        var itemCollectable = GetComponent<ItemCollectable>();
        
        // Obtém o delay de ativação do ItemCollectable se existir
        float delay = 0f;
        if (itemCollectable != null)
        {
            // Usa reflexão para acessar o campo privado activationDelay
            var field = itemCollectable.GetType().GetField("activationDelay", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (field != null)
            {
                delay = (float)field.GetValue(itemCollectable);
                
                if (enableDebugLogs)
                {}
            }
        }
        
        // Usa Invoke para agendar habilitação de colliders após o delay
        if (delay > 0f)
        {
            Invoke(nameof(EnableCollidersAndNotify), delay);
            
            if (enableDebugLogs)
            {}
        }
        else
        {
            // Se não há delay, habilita imediatamente
            EnableCollidersAndNotify();
        }
    }

    /// <summary>
    /// Habilita colliders e atualiza estado para ReadyForCollection.
    /// Chamado após o delay de ativação do ItemCollectable.
    /// </summary>
    private void EnableCollidersAndNotify()
    {
        // Habilita todos os colliders
        EnableAllColliders();
        
        // Atualiza estado para ReadyForCollection
        _currentState = BounceState.ReadyForCollection;
        
        if (enableDebugLogs)
        {}
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

    /// <summary>
    /// Desabilita todos os Collider2D do objeto.
    /// </summary>
    private void DisableAllColliders()
    {
        if (_colliders == null || _colliders.Length == 0)
            return;

        foreach (var collider in _colliders)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        if (enableDebugLogs)
        {
            // Debug: Colliders desabilitados
        }
    }

    /// <summary>
    /// Habilita todos os Collider2D do objeto.
    /// </summary>
    private void EnableAllColliders()
    {
        if (_colliders == null || _colliders.Length == 0)
            return;

        foreach (var collider in _colliders)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        if (enableDebugLogs)
        {
            // Debug: Colliders habilitados
        }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Estado atual do sistema de quicadas.
    /// </summary>
    public BounceState CurrentState => _currentState;

    /// <summary>
    /// Verifica se o objeto está em movimento (Launching ou Bouncing).
    /// </summary>
    public bool IsMoving => _currentState == BounceState.Launching || _currentState == BounceState.Bouncing;

    /// <summary>
    /// Verifica se o objeto está pronto para coleta (ReadyForCollection).
    /// </summary>
    public bool IsReadyForCollection => _currentState == BounceState.ReadyForCollection;

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

    #region Gizmos & Debug Visualization
    /// <summary>
    /// Desenha Gizmos para visualização no editor.
    /// Mostra trajetória prevista, estado atual e raio de atração.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Desenha indicador de estado com cor
        DrawStateIndicator();
    }

    /// <summary>
    /// Desenha Gizmos detalhados quando o objeto está selecionado.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Desenha trajetória prevista se não foi lançado ainda
        if (!_hasBeenLaunched && Application.isPlaying)
        {
            DrawPredictedTrajectory();
        }

        // Desenha raio de atração do ItemCollectable se existir
        DrawAttractionRadius();

        // Desenha label com estado atual
        DrawStateLabel();
    }

    /// <summary>
    /// Desenha indicador visual do estado atual com cores diferentes.
    /// Verde = pronto para coleta, Amarelo = quicando, Vermelho = parado, Cinza = não lançado
    /// </summary>
    private void DrawStateIndicator()
    {
        // Define cor baseada no estado
        Color stateColor = _currentState switch
        {
            BounceState.NotLaunched => Color.gray,
            BounceState.Launching => new Color(1f, 0.5f, 0f), // Laranja
            BounceState.Bouncing => Color.yellow,
            BounceState.Stopping => new Color(1f, 0.3f, 0f), // Laranja escuro
            BounceState.Stopped => Color.red,
            BounceState.ReadyForCollection => Color.green,
            _ => Color.white
        };

        Gizmos.color = stateColor;

        // Desenha esfera pequena acima do objeto
        Vector3 indicatorPosition = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawSphere(indicatorPosition, 0.15f);

        // Desenha linha conectando ao objeto
        Gizmos.DrawLine(transform.position, indicatorPosition);
    }

    /// <summary>
    /// Desenha trajetória prevista baseada em força e ângulo configurados.
    /// </summary>
    private void DrawPredictedTrajectory()
    {
        // Usa valores médios de força e ângulo
        float avgForce = (minLaunchForce + maxLaunchForce) / 2f;
        float avgAngle = (minAngle + maxAngle) / 2f;

        // Converte ângulo para direção
        Vector2 direction = AngleToVector2(avgAngle);
        direction.y *= verticalForceMultiplier;
        direction.Normalize();

        // Calcula velocidade inicial
        Vector2 velocity = direction * avgForce;

        // Simula trajetória
        Vector3 currentPos = transform.position;
        Vector3 previousPos = currentPos;
        float timeStep = 0.05f;
        int steps = 50;

        Gizmos.color = new Color(0f, 1f, 1f, 0.5f); // Ciano semi-transparente

        for (int i = 0; i < steps; i++)
        {
            float time = i * timeStep;

            // Calcula posição usando física básica: p = p0 + v*t + 0.5*g*t^2
            Vector2 gravity = Physics2D.gravity;
            currentPos = transform.position + (Vector3)(velocity * time + 0.5f * gravity * time * time);

            // Desenha linha entre pontos
            Gizmos.DrawLine(previousPos, currentPos);

            // Desenha pontos de quicada previstos
            if (i > 0 && bounceCount > 0)
            {
                float bounceTime = timeToBounce;
                for (int b = 0; b < bounceCount; b++)
                {
                    if (Mathf.Abs(time - bounceTime) < timeStep)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(currentPos, 0.2f);
                        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
                    }
                    bounceTime += timeToBounce / Mathf.Pow(2f, b + 1);
                }
            }

            previousPos = currentPos;

            // Para se atingir o chão (aproximação)
            if (currentPos.y < transform.position.y - 2f)
                break;
        }

        // Desenha indicadores de ângulo mínimo e máximo
        DrawAngleIndicators();
    }

    /// <summary>
    /// Desenha indicadores visuais dos ângulos mínimo e máximo de lançamento.
    /// </summary>
    private void DrawAngleIndicators()
    {
        float indicatorLength = 1.5f;

        // Ângulo mínimo (verde)
        Vector2 minDir = AngleToVector2(minAngle);
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)minDir * indicatorLength);

        // Ângulo máximo (azul)
        Vector2 maxDir = AngleToVector2(maxAngle);
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)maxDir * indicatorLength);

        // Desenha arco entre os ângulos
        DrawArc(transform.position, indicatorLength, minAngle, maxAngle, 20);
    }

    /// <summary>
    /// Desenha um arco entre dois ângulos.
    /// </summary>
    private void DrawArc(Vector3 center, float radius, float startAngle, float endAngle, int segments)
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);

        float angleStep = (endAngle - startAngle) / segments;
        Vector3 previousPoint = center + (Vector3)AngleToVector2(startAngle) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector3 currentPoint = center + (Vector3)AngleToVector2(angle) * radius;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    /// <summary>
    /// Desenha raio de atração do ItemCollectable se existir.
    /// </summary>
    private void DrawAttractionRadius()
    {
        var itemCollectable = GetComponent<ItemCollectable>();
        if (itemCollectable == null) return;

        // Usa reflexão para acessar o campo privado attractionRadius
        var field = itemCollectable.GetType().GetField("attractionRadius",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            float attractionRadius = (float)field.GetValue(itemCollectable);

            // Cor baseada no estado de prontidão
            if (_currentState == BounceState.ReadyForCollection)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.3f); // Verde semi-transparente
            }
            else
            {
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f); // Laranja semi-transparente
            }

            // Desenha esfera de atração
            Gizmos.DrawWireSphere(transform.position, attractionRadius);

            // Desenha círculo preenchido no chão
            DrawCircleOnGround(transform.position, attractionRadius);
        }
    }

    /// <summary>
    /// Desenha um círculo no chão para melhor visualização do raio de atração.
    /// </summary>
    private void DrawCircleOnGround(Vector3 center, float radius)
    {
        int segments = 32;
        float angleStep = 360f / segments;

        Vector3 previousPoint = center + new Vector3(Mathf.Cos(0), 0, 0) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 currentPoint = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }
    }

    /// <summary>
    /// Desenha label com estado atual e contador de quicadas usando Handles.
    /// </summary>
    private void DrawStateLabel()
    {
#if UNITY_EDITOR
        // Posição do label acima do objeto
        Vector3 labelPosition = transform.position + Vector3.up * 1f;

        // Monta texto do label
        string stateText = $"Estado: {_currentState}";
        
        if (_hasBeenLaunched && bounceCount > 0)
        {
            stateText += $"\nQuicadas: {_currentBounceIndex}/{bounceCount}";
        }

        if (_currentState == BounceState.Bouncing || _currentState == BounceState.Launching)
        {
            if (_rigidbody2D != null)
            {
                stateText += $"\nVel: {_rigidbody2D.linearVelocity.magnitude:F1}";
            }
        }

        // Define estilo do label
        UnityEditor.Handles.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 11;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;

        // Adiciona sombra ao texto para melhor legibilidade
        GUIStyle shadowStyle = new GUIStyle(style);
        shadowStyle.normal.textColor = Color.black;

        // Desenha sombra
        UnityEditor.Handles.Label(labelPosition + Vector3.right * 0.02f + Vector3.down * 0.02f, stateText, shadowStyle);
        
        // Desenha texto principal
        UnityEditor.Handles.Label(labelPosition, stateText, style);
#endif
    }
    #endregion
}
}
