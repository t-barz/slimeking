using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Controlador principal do personagem jogador para o jogo SlimeKing.
/// 
/// RESPONSABILIDADES:
/// • Gerencia movimento suave com aceleração/desaceleração
/// • Controla sistema de combate com detecção de inimigos
/// • Integra com PlayerAttributesHandler para atributos dinâmicos
/// • Processa input através do novo Input System do Unity
/// • Controla animações e flip de sprite baseado na direção
/// • Gerencia sistema de direção visual com rotação de objetos de ataque
/// • Aplica flip automático em sprites laterais baseado na direção horizontal
/// • Controla VFX direcionais independentemente dos sprites principais
/// • Rotaciona objetos de ataque automaticamente baseado na direção atual (Sul = padrão)
/// • Posiciona objetos de ataque com offset dinâmico baseado na direção
/// • Fornece sistema extensível para interações e uso de inventário
/// 
/// DEPENDÊNCIAS:
/// • Rigidbody2D: Para física de movimento
/// • Animator: Para controle de animações
/// • SpriteRenderer: Para flip de direção
/// • PlayerAttributesHandler: Para sistema de atributos (opcional)
/// • InputSystem_Actions: Para processamento de input
/// 
/// NOTA: Esta classe segue o padrão de não usar Singleton para classes Player*
/// conforme diretrizes do projeto.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    #region Inspector Configuration

    [Header("⚙️ Configurações de Movimento")]
    [Tooltip("Velocidade máxima de movimento do jogador (será sobrescrita pelos atributos se PlayerAttributesHandler estiver presente)")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Velocidade de aceleração ao iniciar movimento (unidades por segundo)")]
    [SerializeField] private float acceleration = 10f;

    [Tooltip("Velocidade de desaceleração ao parar movimento (unidades por segundo)")]
    [SerializeField] private float deceleration = 10f;

    [Header("⚔️ Configurações de Combate")]
    [Tooltip("Raio de detecção de inimigos para o ataque (em unidades do mundo)")]
    [SerializeField] private float attackRange = 1f;

    [Tooltip("Layers que contêm inimigos que podem ser atacados")]
    [SerializeField] private LayerMask enemyLayers;

    [Tooltip("Prefab do GameObject que representa o ataque visual")]
    [SerializeField] private GameObject attackPrefab;

    [Tooltip("Duração do ataque em segundos")]
    [SerializeField] private float attackDuration = 0.5f;

    [Tooltip("Se verdadeiro, impede movimento durante ataques")]
    [SerializeField] private bool lockMovementDuringAttack = true;

    [Tooltip("Offset para posicionamento do objeto de ataque relativo ao transform do jogador")]
    [SerializeField] private Vector2 attackInstantiationOffset = Vector2.zero;

    [Header("🎨 Configurações Visuais")]
    [Tooltip("Referências aos GameObjects filhos para controle de direção visual")]
    [SerializeField] private GameObject frontObject;
    [SerializeField] private GameObject backObject;
    [SerializeField] private GameObject sideObject;
    [SerializeField] private GameObject vfxFrontObject;
    [SerializeField] private GameObject vfxBackObject;
    [SerializeField] private GameObject vfxSideObject;
    [SerializeField] private GameObject shadowObject;

    [Header("🔧 Ferramentas de Debug")]
    [Tooltip("Habilita logs detalhados no Console para debug de movimento e ações")]
    [SerializeField] private bool enableLogs = false;

    [Tooltip("Mostra gizmos no Scene View para visualizar ranges de ataque e informações de debug")]
    [SerializeField] private bool enableDebugGizmos = true;

    #endregion

    #region Private Variables

    // === COMPONENTES ESSENCIAIS ===
    // Estes componentes são obrigatórios para o funcionamento correto
    private Rigidbody2D _rigidbody;              // Física de movimento
    private Animator _animator;                   // Controle de animações
    private SpriteRenderer _spriteRenderer;      // Flip de sprite
    private PlayerAttributesHandler _attributesHandler; // Sistema de atributos (opcional)

    // === SISTEMA DE INPUT ===
    // Gerenciamento de entrada do usuário via novo Input System
    private InputSystem_Actions _inputActions;   // Ações de input geradas automaticamente
    private Vector2 _moveInput;                  // Input de movimento atual (-1 a 1 em X e Y)
    private bool _facingRight = true;            // Direção atual do sprite (true = direita, false = esquerda)

    // === ESTADO DO JOGADOR ===
    // Flags de controle que determinam o que o jogador pode fazer
    private bool _isMoving = false;              // Se o jogador está em movimento
    private bool _canMove = true;                // Se o movimento está habilitado
    private bool _canAttack = true;              // Se o ataque está disponível (sem cooldown)
    private bool _isAttacking = false;           // Se o jogador está executando um ataque
    private bool _isHiding = false;              // Se o jogador está escondido (Crouch pressionado)

    // === OTIMIZAÇÃO DE PERFORMANCE ===
    // Usando StringToHash para evitar overhead de strings nas chamadas do Animator
    // Estas constantes são calculadas uma vez no carregamento da classe
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsHiding = Animator.StringToHash("isHiding");
    private static readonly int Attack01 = Animator.StringToHash("Attack01");
    private static readonly int FacingRight = Animator.StringToHash("FacingRight");

    // === CONSTANTES DE CONFIGURAÇÃO ===
    // Valores mágicos extraídos para facilitar manutenção
    private const float MOVEMENT_THRESHOLD = 0.1f;  // Threshold mínimo para considerar movimento
    private const float DEBUG_LABEL_HEIGHT = 3f;    // Altura do label de debug no Scene View

    // === SISTEMA DE DIREÇÃO VISUAL ===
    /// <summary>
    /// Direções visuais do personagem para controle de sprites direcionais
    /// </summary>
    public enum VisualDirection
    {
        South,  // Frente (padrão)
        North,  // Costas
        Side    // Lateral (East/West)
    }

    private VisualDirection _currentVisualDirection = VisualDirection.South;
    private bool _vfxEnabled = false;

    #endregion

    #region Unity Lifecycle

    /// <summary>
    /// Inicialização de componentes e validações de dependências.
    /// Chamado antes de qualquer método Start() de outros scripts.
    /// </summary>
    private void Awake()
    {
        // Obtém componentes obrigatórios
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // Obtém componente opcional de atributos
        _attributesHandler = GetComponent<PlayerAttributesHandler>();

        // Validações críticas - sem estes componentes o jogador não funciona
        ValidateRequiredComponents();

        // Inicializa sistema de input
        InitializeInputSystem();
    }

    /// <summary>
    /// Ativa o sistema de input e subscreve aos eventos.
    /// Chamado automaticamente quando o GameObject fica ativo.
    /// </summary>
    private void OnEnable()
    {
        // Ativa o sistema de input (necessário para receber eventos)
        _inputActions.Enable();

        // Subscreve aos eventos de input - ordem não importa aqui
        SubscribeToInputEvents();
    }

    /// <summary>
    /// Desativa o sistema de input e remove subscrições aos eventos.
    /// Importante para evitar memory leaks e callbacks órfãos.
    /// </summary>
    private void OnDisable()
    {
        // Remove todas as subscrições antes de desativar
        UnsubscribeFromInputEvents();

        // Desativa o sistema de input
        _inputActions.Disable();
    }

    /// <summary>
    /// Configurações iniciais e subscrição aos eventos do sistema de atributos.
    /// Executado após todos os Awake() mas antes do primeiro Update().
    /// </summary>
    private void Start()
    {
        // Conecta com o sistema de atributos se disponível
        ConnectToAttributeSystem();

        // Sincroniza velocidade inicial com os atributos
        SynchronizeInitialSpeed();

        // Inicializa sistema visual direcional
        InitializeVisualObjects();

        // Valida parâmetros do Animator
        ValidateAnimatorParameters();

        // Log de inicialização bem-sucedida
        LogSuccessfulInitialization();
    }

    /// <summary>
    /// Atualização por frame - processa lógica que não depende de física.
    /// Frequência: ~60 FPS (dependente do framerate)
    /// </summary>
    private void Update()
    {
        UpdateAnimations();
    }

    /// <summary>
    /// Atualização de física - processa movimento com timestep fixo.
    /// Frequência: 50 Hz (padrão do Unity, independente do framerate)
    /// Garante movimento consistente independente da performance
    /// </summary>
    private void FixedUpdate()
    {
        HandleMovement();
    }

    #endregion

    #region Initialization Helper Methods

    /// <summary>
    /// Valida se todos os componentes obrigatórios estão presentes.
    /// Emite erros críticos se componentes essenciais estiverem ausentes.
    /// </summary>
    private void ValidateRequiredComponents()
    {
    }

    /// <summary>
    /// Inicializa o sistema de input do Unity.
    /// Cria uma nova instância das ações de input geradas automaticamente.
    /// </summary>
    private void InitializeInputSystem()
    {
        try
        {
            _inputActions = new InputSystem_Actions();
        }
        catch (System.Exception)
        {
        }
    }

    /// <summary>
    /// Subscreve aos eventos de input necessários para o gameplay.
    /// Conecta os callbacks aos eventos do Input System.
    /// </summary>
    private void SubscribeToInputEvents()
    {
        // Eventos de movimento - tanto performed quanto canceled para parada suave
        _inputActions.Gameplay.Move.performed += OnMoveInput;
        _inputActions.Gameplay.Move.canceled += OnMoveInput;

        // Eventos de ação - apenas performed pois são actions pontuais
        _inputActions.Gameplay.Attack.performed += OnAttackInput;
        _inputActions.Gameplay.Interact.performed += OnInteractInput;
        _inputActions.Gameplay.SpecialAttack.performed += OnSpecialAttackInput;

        // Eventos de esconderijo - tanto performed quanto canceled para controle contínuo
        _inputActions.Gameplay.Crouch.performed += OnCrouchInput;
        _inputActions.Gameplay.Crouch.canceled += OnCrouchInput;

        // Eventos de uso de itens do inventário (slots 1-4)
        _inputActions.Gameplay.UseItem1.performed += OnUseItem1Input;
        _inputActions.Gameplay.UseItem2.performed += OnUseItem2Input;
        _inputActions.Gameplay.UseItem3.performed += OnUseItem3Input;
        _inputActions.Gameplay.UseItem4.performed += OnUseItem4Input;
    }

    /// <summary>
    /// Remove subscrições aos eventos de input.
    /// Importante para evitar memory leaks e callbacks órfãos.
    /// </summary>
    private void UnsubscribeFromInputEvents()
    {
        // Remove eventos de movimento
        _inputActions.Gameplay.Move.performed -= OnMoveInput;
        _inputActions.Gameplay.Move.canceled -= OnMoveInput;

        // Remove eventos de ação
        _inputActions.Gameplay.Attack.performed -= OnAttackInput;
        _inputActions.Gameplay.Interact.performed -= OnInteractInput;
        _inputActions.Gameplay.SpecialAttack.performed -= OnSpecialAttackInput;

        // Remove eventos de esconderijo
        _inputActions.Gameplay.Crouch.performed -= OnCrouchInput;
        _inputActions.Gameplay.Crouch.canceled -= OnCrouchInput;

        // Remove eventos de uso de itens
        _inputActions.Gameplay.UseItem1.performed -= OnUseItem1Input;
        _inputActions.Gameplay.UseItem2.performed -= OnUseItem2Input;
        _inputActions.Gameplay.UseItem3.performed -= OnUseItem3Input;
        _inputActions.Gameplay.UseItem4.performed -= OnUseItem4Input;
    }

    /// <summary>
    /// Conecta-se ao sistema de atributos se disponível.
    /// Subscreve aos eventos de mudança de atributos para feedback visual.
    /// </summary>
    private void ConnectToAttributeSystem()
    {
        if (_attributesHandler != null)
        {
            _attributesHandler.OnPlayerDied += OnPlayerDied;
            _attributesHandler.OnHealthChanged += OnHealthChanged;

        }
    }

    /// <summary>
    /// Sincroniza a velocidade inicial com o sistema de atributos.
    /// Sobrescreve a velocidade padrão se atributos dinâmicos estiverem disponíveis.
    /// </summary>
    private void SynchronizeInitialSpeed()
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

    /// <summary>
    /// Registra no log que a inicialização foi bem-sucedida.
    /// Fornece informações úteis sobre o estado inicial do jogador.
    /// </summary>
    private void LogSuccessfulInitialization()
    {
    }

    /// <summary>
    /// Valida se os parâmetros do Animator Controller estão configurados corretamente.
    /// Ajuda a identificar problemas de configuração que podem afetar as animações.
    /// </summary>
    private void ValidateAnimatorParameters()
    {
        if (_animator == null || _animator.runtimeAnimatorController == null) return;

        // Lista de parâmetros que devem existir no Animator Controller
        string[] requiredBoolParams = { "isWalking", "isHiding", "FacingRight" };
        string[] requiredTriggerParams = { "Attack01" };

        // Verifica parâmetros bool
        foreach (string paramName in requiredBoolParams)
        {
            if (!HasAnimatorParameter(paramName, AnimatorControllerParameterType.Bool))
            {
            }
        }

        foreach (string paramName in requiredTriggerParams)
        {
            if (!HasAnimatorParameter(paramName, AnimatorControllerParameterType.Trigger))
            {
            }
        }

    }

    /// <summary>
    /// Verifica se um parâmetro específico existe no Animator Controller.
    /// </summary>
    /// <param name="paramName">Nome do parâmetro</param>
    /// <param name="paramType">Tipo do parâmetro</param>
    /// <returns>True se o parâmetro existe e é do tipo correto</returns>
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

    #region Debug Visualization

    /// <summary>
    /// Desenha gizmos no Scene View para visualização e debug.
    /// Só é executado no Editor, não impacta performance em build.
    /// </summary>

    /// <summary>
    /// Desenha gizmos no Scene View para visualização e debug.
    /// Só é executado no Editor, não impacta performance em build.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Early exit se debug estiver desabilitado
        if (!enableDebugGizmos) return;

        // Desenha range de ataque se attackPoint estiver configurado
        DrawAttackRange();

        // Desenha posição de instanciamento do objeto de ataque
        DrawAttackInstantiationPosition();

        // FUTURO: Desenhar range de interação quando implementado
        // DrawInteractionRange();

        // Desenha informações textuais de debug (apenas no Editor)
        DrawDebugLabels();
    }

    /// <summary>
    /// Desenha o range de ataque como uma esfera vermelha wireframe.
    /// </summary>
    private void DrawAttackRange()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    /// <summary>
    /// Desenha a posição onde o objeto de ataque será instanciado.
    /// Mostra um cubo pequeno na posição calculada com offset dinâmico.
    /// </summary>
    private void DrawAttackInstantiationPosition()
    {
        // Só desenha se houver offset configurado
        if (attackInstantiationOffset.magnitude > 0.001f)
        {
            Gizmos.color = Color.cyan;
            Vector3 instantiationPosition = GetAttackPosition();
            Gizmos.DrawWireCube(instantiationPosition, Vector3.one * 0.2f);

            // Desenha linha conectando o transform à posição de instanciamento
            Gizmos.color = Color.cyan * 0.5f;
            Gizmos.DrawLine(transform.position, instantiationPosition);
        }
    }

    /// <summary>
    /// Desenha informações de debug como texto no Scene View.
    /// Apenas disponível no Editor Unity.
    /// </summary>
    private void DrawDebugLabels()
    {
#if UNITY_EDITOR
        if (_attributesHandler != null)
        {
            Vector3 labelPosition = transform.position + Vector3.up * DEBUG_LABEL_HEIGHT;
            string debugInfo = $"Speed: {moveSpeed:F1}\n" +
                             $"Can Move: {_canMove}\n" +
                             $"Can Attack: {_canAttack}\n" +
                             $"Is Moving: {_isMoving}\n" +
                             $"Is Hiding: {_isHiding}\n" +
                             $"Facing Right: {_facingRight}\n" +
                             $"Visual Dir: {_currentVisualDirection}";

            UnityEditor.Handles.Label(labelPosition, debugInfo);

            // Desenha indicador visual da direção
            DrawVisualDirectionIndicator();
        }
#endif
    }

    /// <summary>
    /// Desenha um indicador visual da direção atual no Scene View
    /// </summary>
    private void DrawVisualDirectionIndicator()
    {
#if UNITY_EDITOR
        Vector3 center = transform.position;
        Vector3 direction = Vector3.zero;
        Color directionColor = Color.white;

        // Define direção e cor baseada na direção visual atual
        switch (_currentVisualDirection)
        {
            case VisualDirection.South:
                direction = Vector3.down;
                directionColor = Color.green;
                break;
            case VisualDirection.North:
                direction = Vector3.up;
                directionColor = Color.blue;
                break;
            case VisualDirection.Side:
                direction = _facingRight ? Vector3.right : Vector3.left;
                directionColor = Color.yellow;
                break;
        }

        // Desenha seta indicando direção
        Gizmos.color = directionColor;
        Vector3 arrowEnd = center + direction * 0.5f;
        Gizmos.DrawLine(center, arrowEnd);

        // Desenha ponta da seta
        Vector3 arrowHead1 = arrowEnd + (Quaternion.Euler(0, 0, 45) * -direction * 0.2f);
        Vector3 arrowHead2 = arrowEnd + (Quaternion.Euler(0, 0, -45) * -direction * 0.2f);
        Gizmos.DrawLine(arrowEnd, arrowHead1);
        Gizmos.DrawLine(arrowEnd, arrowHead2);
#endif
    }

    #endregion

    #region Input Handlers

    /// <summary>
    /// Processa input de movimento do jogador via WASD ou Arrow Keys.
    /// Chamado tanto no 'performed' (tecla pressionada) quanto no 'canceled' (tecla solta).
    /// 
    /// FLUXO:
    /// • Lê valores do contexto (-1 a 1 para cada eixo)
    /// • Determina se está em movimento baseado no threshold
    /// • Atualiza flags internas para uso em FixedUpdate
    /// </summary>
    /// <param name="context">Contexto de input contendo valores do Vector2 de movimento</param>
    private void OnMoveInput(InputAction.CallbackContext context)
    {
        // Lê o valor do input (Vector2 normalizado)
        _moveInput = context.ReadValue<Vector2>();

        // Debug detalhado do input

        // NOTA: _isMoving será atualizada em HandleMovement() para garantir consistência
        // Não atualizamos aqui para evitar problemas com input canceled
    }

    /// <summary>
    /// Processa input de ataque básico via Mouse Left Button ou Ctrl.
    /// Inicia a corrotina de ataque se o jogador estiver apto a atacar.
    /// 
    /// CONDIÇÕES:
    /// • _canAttack deve ser true (ataque disponível)
    /// • Não verifica se está em movimento - pode atacar enquanto anda
    /// </summary>
    /// <param name="context">Contexto de input do botão de ataque</param>
    private void OnAttackInput(InputAction.CallbackContext context)
    {

        // Verifica se pode atacar (não está em cooldown)
        if (_canAttack)
        {

            StartCoroutine(PerformAttack());
        }
    }

    /// <summary>
    /// Processa input de interação contextual via tecla E.
    /// Usado para coletar itens próximos e interagir com elementos do cenário.
    /// 
    /// FUTURAS IMPLEMENTAÇÕES:
    /// • Detecção de CollectibleItems próximos
    /// • Interação com NPCs (diálogo)
    /// • Ativação de switches/alavancas
    /// • Entrada em passagens especiais (Shrink/Jump)
    /// </summary>
    /// <param name="context">Contexto de input da tecla de interação</param>
    private void OnInteractInput(InputAction.CallbackContext context)
    {

    }

    /// <summary>
    /// Processa input de ataque especial via tecla Space.
    /// Reservado para ataques mais poderosos ou habilidades especiais.
    /// 
    /// PLANEJAMENTO:
    /// • Pode ter cooldown maior que ataque básico
    /// • Pode consumir mana/energia se implementado
    /// • Pode ter animações mais elaboradas
    /// </summary>
    /// <param name="context">Contexto de input do ataque especial</param>
    private void OnSpecialAttackInput(InputAction.CallbackContext context)
    {

    }

    /// <summary>
    /// Processa input de esconderijo via tecla de Crouch.
    /// Ativa/desativa o estado de esconderijo baseado no estado da tecla.
    /// 
    /// COMPORTAMENTO:
    /// • performed: Tecla pressionada - ativa esconderijo (_isHiding = true)
    /// • canceled: Tecla solta - desativa esconderijo (_isHiding = false)
    /// • Atualiza parâmetro do Animator para animações de esconderijo
    /// </summary>
    /// <param name="context">Contexto de input da tecla de esconderijo</param>
    private void OnCrouchInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Tecla pressionada - ativa esconderijo
            _isHiding = true;
        }
        else if (context.canceled)
        {
            // Tecla solta - desativa esconderijo
            _isHiding = false;
        }

        // Atualiza parâmetro do Animator
        if (_animator != null)
        {
            _animator.SetBool(IsHiding, _isHiding);
        }
    }

    // === MÉTODOS DE USO DE ITENS DO INVENTÁRIO ===
    // Cada método corresponde a um slot específico do inventário (1-4)
    // O jogador pode usar itens rapidamente durante o combate

    /// <summary>
    /// Usa item do slot 1 do inventário.
    /// Exemplo: Fruta de cura, poção de força, etc.
    /// </summary>
    /// <param name="context">Contexto de input do UseItem1</param>
    private void OnUseItem1Input(InputAction.CallbackContext context)
    {
        UseInventoryItem(1);
    }

    /// <summary>
    /// Usa item do slot 2 do inventário.
    /// </summary>
    /// <param name="context">Contexto de input do UseItem2</param>
    private void OnUseItem2Input(InputAction.CallbackContext context)
    {
        UseInventoryItem(2);
    }

    /// <summary>
    /// Usa item do slot 3 do inventário.
    /// </summary>
    /// <param name="context">Contexto de input do UseItem3</param>
    private void OnUseItem3Input(InputAction.CallbackContext context)
    {
        UseInventoryItem(3);
    }

    /// <summary>
    /// Usa item do slot 4 do inventário.
    /// </summary>
    /// <param name="context">Contexto de input do UseItem4</param>
    private void OnUseItem4Input(InputAction.CallbackContext context)
    {
        UseInventoryItem(4);
    }

    /// <summary>
    /// Método auxiliar unificado para uso de itens do inventário.
    /// Centraliza a lógica para evitar duplicação de código.
    /// </summary>
    /// <param name="slotNumber">Número do slot do inventário (1-4)</param>
    private void UseInventoryItem(int slotNumber)
    {

    }

    #endregion

    #region Movement System

    /// <summary>
    /// Sistema principal de movimento do jogador.
    /// Executado em FixedUpdate para garantir consistência independente do framerate.
    /// 
    /// FUNCIONALIDADES:
    /// • Sincronização automática com sistema de atributos
    /// • Aceleração/desaceleração suave para movimento natural
    /// • Integração com sistema de controle de movimento (pode ser desabilitado)
    /// • Gerenciamento automático da direção do sprite
    /// 
    /// OTIMIZAÇÕES:
    /// • Early exit se movimento estiver desabilitado
    /// • Atualização de velocidade apenas quando necessário
    /// • Uso de linearVelocity (Unity 2023+) para melhor performance
    /// </summary>
    private void HandleMovement()
    {
        // Early exit se movimento estiver desabilitado (ex: cutscenes, morte, etc.)
        if (!_canMove) return;

        // Se o jogador estiver atacando e movimento estiver bloqueado
        if (_isAttacking && lockMovementDuringAttack)
        {
            // Define movimento como falso para animator
            _isMoving = false;

            // Aplica velocidade zero para parar imediatamente
            _rigidbody.linearVelocity = Vector2.zero;

            // Atualiza animator para mostrar estado parado
            if (_animator != null)
            {
                _animator.SetBool(IsWalking, false);
            }

            return;
        }

        // Atualiza estado de movimento baseado no input atual
        _isMoving = _moveInput.magnitude > MOVEMENT_THRESHOLD;

        // Sincroniza velocidade com sistema de atributos (se disponível)
        SynchronizeSpeedWithAttributes();

        // Calcula velocidade alvo baseada no input atual
        Vector2 targetVelocity = CalculateTargetVelocity();

        // Aplica movimento suave com aceleração/desaceleração
        ApplySmoothMovement(targetVelocity);

        // Atualiza direção do sprite baseada no movimento
        HandleSpriteDirection();

        // Atualiza direção visual dos objetos direcionais
        UpdateVisualDirection();
    }

    /// <summary>
    /// Sincroniza a velocidade de movimento com o sistema de atributos.
    /// Permite que buffs/debuffs de velocidade sejam aplicados dinamicamente.
    /// </summary>
    private void SynchronizeSpeedWithAttributes()
    {
        if (_attributesHandler != null)
        {
            float attributeSpeed = _attributesHandler.CurrentSpeed;

            // Só atualiza se houve mudança para evitar processamento desnecessário
            if (attributeSpeed != moveSpeed)
            {
                moveSpeed = attributeSpeed;

            }
        }
    }

    /// <summary>
    /// Calcula a velocidade alvo baseada no input atual e velocidade máxima.
    /// </summary>
    /// <returns>Velocidade alvo como Vector2</returns>
    private Vector2 CalculateTargetVelocity()
    {
        return _moveInput * moveSpeed;
    }

    /// <summary>
    /// Aplica movimento suave usando interpolação para aceleração/desaceleração natural.
    /// Usa taxas diferentes para acelerar e desacelerar, criando sensação mais responsiva.
    /// </summary>
    /// <param name="targetVelocity">Velocidade desejada</param>
    private void ApplySmoothMovement(Vector2 targetVelocity)
    {
        // Se estiver atacando e movimento estiver bloqueado, força velocidade zero para parar o movimento
        if (_isAttacking && lockMovementDuringAttack)
        {
            targetVelocity = Vector2.zero;
        }

        // Escolhe taxa de interpolação baseada se está acelerando ou desacelerando
        float currentRate = _isMoving ? acceleration : deceleration;

        // Aplica movimento suave usando MoveTowards para controle preciso
        _rigidbody.linearVelocity = Vector2.MoveTowards(
            _rigidbody.linearVelocity,
            targetVelocity,
            currentRate * Time.fixedDeltaTime
        );
    }

    /// <summary>
    /// Controla a direção do sprite baseada no movimento horizontal.
    /// Usa threshold para evitar flips desnecessários com input muito pequeno.
    /// 
    /// LÓGICA:
    /// • Movimento para direita + virado para esquerda = flip para direita
    /// • Movimento para esquerda + virado para direita = flip para esquerda
    /// • Ignora movimento vertical para direção do sprite
    /// </summary>
    private void HandleSpriteDirection()
    {
        // Verifica movimento significativo para direita
        if (_moveInput.x > MOVEMENT_THRESHOLD && !_facingRight)
        {
            FlipSprite();
        }
        // Verifica movimento significativo para esquerda
        else if (_moveInput.x < -MOVEMENT_THRESHOLD && _facingRight)
        {
            FlipSprite();
        }
    }

    /// <summary>
    /// Inverte a direção do sprite e atualiza os sistemas relacionados.
    /// 
    /// ATUALIZAÇÕES:
    /// • Flag interna de direção (_facingRight)
    /// • SpriteRenderer.flipX para visualização
    /// • Parâmetro do Animator para animações direcionais
    /// 
    /// PERFORMANCE: Método é chamado apenas quando necessário (não todo frame)
    /// </summary>
    private void FlipSprite()
    {
        // Atualiza flag interna
        _facingRight = !_facingRight;

        // Aplica flip visual no sprite
        _spriteRenderer.flipX = !_facingRight;

        // Atualiza Animator para animações direcionais (se disponível)
        if (_animator != null)
            _animator.SetBool(FacingRight, _facingRight);

        // Se estiver na direção lateral, atualiza flip dos objetos laterais
        if (_currentVisualDirection == VisualDirection.Side)
        {
            ApplyFlipToSideObject(sideObject);
            // Só aplica flip no VFX se estiver ativo
            if (_vfxEnabled && vfxSideObject != null && vfxSideObject.activeInHierarchy)
            {
                ApplyFlipToSideObject(vfxSideObject);
            }
        }

        // Log para debug (apenas quando necessário)
    }

    /// <summary>
    /// Inicializa o sistema visual buscando os objetos filhos do prefab
    /// Configura automaticamente as referências caso não estejam definidas no inspector
    /// </summary>
    private void InitializeVisualObjects()
    {
        // Se não foram configurados no inspector, tenta encontrar automaticamente
        if (frontObject == null) frontObject = transform.Find("front")?.gameObject;
        if (backObject == null) backObject = transform.Find("back")?.gameObject;
        if (sideObject == null) sideObject = transform.Find("side")?.gameObject;
        if (vfxFrontObject == null) vfxFrontObject = transform.Find("vfx_front")?.gameObject;
        if (vfxBackObject == null) vfxBackObject = transform.Find("vfx_back")?.gameObject;
        if (vfxSideObject == null) vfxSideObject = transform.Find("vfx_side")?.gameObject;
        if (shadowObject == null) shadowObject = transform.Find("shadow")?.gameObject;

        // Define direção inicial como South (frente)
        SetVisualDirection(VisualDirection.South);
    }

    /// <summary>
    /// Define a direção visual do personagem e atualiza a visibilidade dos objetos
    /// </summary>
    /// <param name="direction">Nova direção visual</param>
    private void SetVisualDirection(VisualDirection direction)
    {
        _currentVisualDirection = direction;

        // Desativa todos os objetos primeiro
        if (frontObject != null) frontObject.SetActive(false);
        if (backObject != null) backObject.SetActive(false);
        if (sideObject != null) sideObject.SetActive(false);
        if (vfxFrontObject != null) vfxFrontObject.SetActive(false);
        if (vfxBackObject != null) vfxBackObject.SetActive(false);
        if (vfxSideObject != null) vfxSideObject.SetActive(false);

        // Ativa objetos baseado na direção
        switch (direction)
        {
            case VisualDirection.South:
                if (frontObject != null) frontObject.SetActive(true);
                // VFX só ativa se estiver habilitado
                if (vfxFrontObject != null && _vfxEnabled) vfxFrontObject.SetActive(true);
                break;

            case VisualDirection.North:
                if (backObject != null) backObject.SetActive(true);
                // VFX só ativa se estiver habilitado
                if (vfxBackObject != null && _vfxEnabled) vfxBackObject.SetActive(true);
                break;

            case VisualDirection.Side:
                if (sideObject != null)
                {
                    sideObject.SetActive(true);
                    // Aplica flip no objeto lateral baseado na direção atual
                    ApplyFlipToSideObject(sideObject);
                }
                // VFX só ativa se estiver habilitado
                if (vfxSideObject != null && _vfxEnabled)
                {
                    vfxSideObject.SetActive(true);
                    // Aplica flip no VFX lateral baseado na direção atual
                    ApplyFlipToSideObject(vfxSideObject);
                }
                break;
        }

        // Shadow sempre ativo (se existir)
        if (shadowObject != null) shadowObject.SetActive(true);
    }

    /// <summary>
    /// Aplica flip no objeto lateral baseado na direção atual do personagem.
    /// 
    /// COMPORTAMENTO:
    /// • Quando _facingRight = true (virado para direita): flipX = false (sprite normal)
    /// • Quando _facingRight = false (virado para esquerda): flipX = true (sprite espelhado)
    /// 
    /// BUSCA HIERÁRQUICA:
    /// • Primeiro tenta encontrar SpriteRenderer no próprio objeto
    /// • Se não encontrar, busca nos objetos filhos
    /// • Isso permite flexibilidade na estrutura do prefab
    /// </summary>
    /// <param name="sideObj">GameObject lateral que receberá o flip</param>
    private void ApplyFlipToSideObject(GameObject sideObj)
    {
        if (sideObj == null) return;

        // Busca SpriteRenderer no objeto lateral
        SpriteRenderer sideSpriteRenderer = sideObj.GetComponent<SpriteRenderer>();
        if (sideSpriteRenderer != null)
        {
            // Aplica flip: true quando facing left, false quando facing right
            sideSpriteRenderer.flipX = !_facingRight;

            return; // Encontrou e aplicou, pode retornar
        }

        // Busca SpriteRenderer nos filhos caso não esteja no objeto principal
        sideSpriteRenderer = sideObj.GetComponentInChildren<SpriteRenderer>();
        if (sideSpriteRenderer != null)
        {
            sideSpriteRenderer.flipX = !_facingRight;

        }
    }

    /// <summary>
    /// Atualiza a direção visual baseada no movimento atual
    /// </summary>
    private void UpdateVisualDirection()
    {
        if (_moveInput.magnitude < MOVEMENT_THRESHOLD) return;

        Vector2 direction = _moveInput.normalized;

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

    #endregion

    #region Combat System

    /// <summary>
    /// Executa ataque básico do jogador.
    /// </summary>
    private IEnumerator PerformAttack()
    {
        _canAttack = false;
        _isAttacking = true; // Bloqueia movimento durante o ataque (se configurado)

        // Ativa VFX de ataque baseado na direção atual
        ShowAttackVfx();

        // Instancia GameObject de ataque na posição calculada com offset (se o prefab estiver configurado)
        GameObject attackInstance = null;
        if (attackPrefab != null)
        {
            Vector3 attackPosition = GetAttackPosition();
            attackInstance = Instantiate(attackPrefab, attackPosition, Quaternion.identity);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (enableLogs)
            {
                Vector2 appliedOffset = GetDirectionalInstantiationOffset();
                Debug.Log($"PlayerController: Instanciando ataque - Direção: {_currentVisualDirection}, " +
                         $"FacingRight: {_facingRight}, Offset aplicado: {appliedOffset}, Posição final: {attackPosition}");
            }
#endif

            // Configura os visuais do ataque baseado na direção atual
            SetupAttackVisuals(attackInstance);
        }

        // Trigger da animação de ataque
        if (_animator != null)
        {
            _animator.SetTrigger(Attack01);
        }

        // Executa ataque usando AttackHandler (se disponível)
        SlimeMec.Gameplay.AttackHandler attackHandler = attackInstance?.GetComponent<SlimeMec.Gameplay.AttackHandler>();
        if (attackHandler != null)
        {
            // Determina se é um ataque lateral baseado na direção visual atual
            bool isAttackingSideways = (_currentVisualDirection == VisualDirection.Side);

            // Determina a direção específica para ajuste do offset
            SlimeMec.Gameplay.AttackDirection attackDirection = GetAttackDirection();

            attackHandler.PerformAttack(isAttackingSideways, attackDirection);
        }
        else
        {
            // Fallback: sistema antigo de detecção (manter compatibilidade)
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayers);

            // Aplica dano nos inimigos
            foreach (Collider2D enemy in hitEnemies)
            {
                // TODO: Criar classe EnemyHealth para gerenciar vida dos inimigos
                // var enemyHealth = enemy.GetComponent<EnemyHealth>();
                // if (enemyHealth != null && _attributesHandler != null)
                // {
                //     enemyHealth.TakeDamage(_attributesHandler.CurrentAttack);
                // }
            }
        }

        // Aguarda a duração do ataque
        yield return new WaitForSeconds(attackDuration);

        // Destrói o GameObject de ataque após a duração
        if (attackInstance != null)
        {
            Destroy(attackInstance);
        }

        ResetAttackState();

        yield break;
    }

    #endregion

    #region Animation Updates

    /// <summary>
    /// Atualiza parâmetros do Animator baseado no estado atual.
    /// </summary>
    private void UpdateAnimations()
    {
        if (_animator == null) return;

        // Calcula se deve estar andando
        bool shouldBeWalking = _isMoving && _canMove;

        // Atualiza parâmetro de movimento
        _animator.SetBool(IsWalking, shouldBeWalking);

        // Atualiza direção
        _animator.SetBool(FacingRight, _facingRight);

        // Atualiza estado de esconderijo
        _animator.SetBool(IsHiding, _isHiding);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Chamado quando o jogador morre.
    /// </summary>
    private void OnPlayerDied()
    {
        _canMove = false;
        _canAttack = false;


    }

    /// <summary>
    /// Chamado quando a vida do jogador muda.
    /// </summary>
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
    }

    #endregion

    #region Combat Helper Methods

    /// <summary>
    /// Calcula a posição onde o GameObject de ataque deve ser instanciado baseado na direção atual.
    /// Aplica offset dinâmico que se ajusta automaticamente conforme a direção do movimento.
    /// Usa as mesmas regras de transformação do AttackHandler para consistência.
    /// </summary>
    /// <returns>Posição final para instanciar o ataque</returns>
    private Vector3 GetAttackPosition()
    {
        Vector2 adjustedOffset = GetDirectionalInstantiationOffset();
        return transform.position + (Vector3)adjustedOffset;
    }

    /// <summary>
    /// Calcula o offset de instanciamento baseado na direção atual do ataque.
    /// Aplica as mesmas transformações do AttackHandler para manter consistência:
    /// • Sul: offset reduzido pela metade (50% do valor original)
    /// • Norte: inverte Y
    /// • Leste: troca X e Y
    /// • Oeste: troca X e Y + inverte X
    /// </summary>
    /// <returns>Offset ajustado para a direção atual</returns>
    private Vector2 GetDirectionalInstantiationOffset()
    {
        Vector2 adjustedOffset = attackInstantiationOffset;

        switch (_currentVisualDirection)
        {
            case VisualDirection.South:
                // Sul: reduz o offset pela metade
                adjustedOffset = attackInstantiationOffset * 0.5f;
                break;

            case VisualDirection.North:
                // Sul → Norte: inverte Y
                adjustedOffset.y = -attackInstantiationOffset.y;
                break;

            case VisualDirection.Side:
                // Vertical → Horizontal: troca X e Y
                if (_facingRight)
                {
                    // Leste: troca X e Y
                    adjustedOffset = new Vector2(-attackInstantiationOffset.y, attackInstantiationOffset.x);
                }
                else
                {
                    // Oeste: troca X e Y + inverte X
                    adjustedOffset = new Vector2(attackInstantiationOffset.y, attackInstantiationOffset.x);
                }
                break;
        }

        return adjustedOffset;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Estado atual dos VFX (readonly)
    /// IMPORTANTE: VFX de ataque são independentes do estado _vfxEnabled
    /// e são controlados automaticamente durante ataques.
    /// </summary>
    private bool VfxEnabled => _vfxEnabled;

    /// <summary>
    /// Ativa os efeitos visuais (VFX) direcionais.
    /// Aplica os VFX baseado na direção visual atual.
    /// </summary>
    private void EnableVfx()
    {
        _vfxEnabled = true;
        UpdateVfxVisibility();

    }

    /// <summary>
    /// Desativa todos os efeitos visuais (VFX) direcionais.
    /// </summary>
    private void DisableVfx()
    {
        _vfxEnabled = false;

        // Desativa todos os VFX
        if (vfxFrontObject != null) vfxFrontObject.SetActive(false);
        if (vfxBackObject != null) vfxBackObject.SetActive(false);
        if (vfxSideObject != null) vfxSideObject.SetActive(false);

    }

    /// <summary>
    /// Alterna o estado dos VFX (liga/desliga)
    /// </summary>
    private void ToggleVfx()
    {
        if (_vfxEnabled)
            DisableVfx();
        else
            EnableVfx();
    }

    /// <summary>
    /// Atualiza a visibilidade dos VFX baseado no estado atual e direção visual
    /// </summary>
    private void UpdateVfxVisibility()
    {
        if (!_vfxEnabled) return;

        // Desativa todos primeiro
        if (vfxFrontObject != null) vfxFrontObject.SetActive(false);
        if (vfxBackObject != null) vfxBackObject.SetActive(false);
        if (vfxSideObject != null) vfxSideObject.SetActive(false);

        // Ativa VFX baseado na direção atual
        switch (_currentVisualDirection)
        {
            case VisualDirection.South:
                if (vfxFrontObject != null) vfxFrontObject.SetActive(true);
                break;

            case VisualDirection.North:
                if (vfxBackObject != null) vfxBackObject.SetActive(true);
                break;

            case VisualDirection.Side:
                if (vfxSideObject != null)
                {
                    vfxSideObject.SetActive(true);
                    ApplyFlipToSideObject(vfxSideObject);
                }
                break;
        }
    }

    /// <summary>
    /// Exibe VFX de ataque baseado na direção visual atual
    /// </summary>
    private void ShowAttackVfx()
    {
        switch (_currentVisualDirection)
        {
            case VisualDirection.South:
                if (vfxFrontObject != null)
                {
                    vfxFrontObject.SetActive(true);
                }
                break;

            case VisualDirection.North:
                if (vfxBackObject != null)
                {
                    vfxBackObject.SetActive(true);
                }
                break;

            case VisualDirection.Side:
                if (vfxSideObject != null)
                {
                    vfxSideObject.SetActive(true);
                    ApplyFlipToSideObject(vfxSideObject);
                }
                break;
        }
    }

    /// <summary>
    /// Configura a rotação do objeto de ataque de acordo com a direção atual do personagem.
    /// A direção padrão (Sul) não aplica rotação. Sistema otimizado com cálculo direto de rotação.
    /// </summary>
    /// <param name="attackObj">Objeto de ataque recém-instanciado</param>
    private void SetupAttackVisuals(GameObject attackObj)
    {
        // Cálculo otimizado de rotação baseado na direção atual
        float rotationZ = _currentVisualDirection switch
        {
            VisualDirection.South => 0f,      // Direção padrão - sem rotação
            VisualDirection.North => 180f,    // Rotaciona 180 graus para apontar para cima
            VisualDirection.Side => _facingRight ? 90f : -90f, // 90 para direita, -90 para esquerda
            _ => 0f // Fallback para direção padrão
        };

        // Aplica a rotação no eixo Z - usa Quaternion direto para performance
        attackObj.transform.rotation = Quaternion.AngleAxis(rotationZ, Vector3.forward);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (enableLogs)
        {
            Debug.Log($"PlayerController: Configurando rotação de ataque - Direção: {_currentVisualDirection}, " +
                     $"FacingRight: {_facingRight}, Rotação Z: {rotationZ}°");
        }
#endif
    }

    /// <summary>
    /// Esconde todos os VFX de ataque
    /// </summary>
    private void HideAttackVfx()
    {
        if (vfxFrontObject != null)
        {
            vfxFrontObject.SetActive(false);
        }
        if (vfxBackObject != null)
        {
            vfxBackObject.SetActive(false);
        }
        if (vfxSideObject != null)
        {
            vfxSideObject.SetActive(false);
        }
    }

    /// <summary>
    /// Direção visual atual do personagem (readonly)
    /// </summary>
    private VisualDirection CurrentVisualDirection => _currentVisualDirection;

    /// <summary>
    /// Define manualmente a direção visual (útil para cutscenes/animações)
    /// </summary>
    /// <param name="direction">Nova direção visual</param>
    private void SetManualVisualDirection(VisualDirection direction)
    {
        SetVisualDirection(direction);

    }

    /// <summary>
    /// Desabilita temporariamente o movimento do jogador.
    /// </summary>
    private void DisableMovement(float duration = 0f)
    {
        _canMove = false;
        if (duration > 0f)
        {
            StartCoroutine(EnableMovementAfterDelay(duration));
        }
    }

    /// <summary>
    /// Habilita o movimento do jogador.
    /// </summary>
    private void EnableMovement()
    {
        _canMove = true;
    }

    /// <summary>
    /// Ativa/desativa logs de debug para troubleshooting.
    /// Útil para depuração de problemas de animação e movimento.
    /// </summary>
    /// <param name="enable">True para ativar logs, false para desativar</param>
    private void SetDebugLogs(bool enable)
    {
        enableLogs = enable;
        if (enable)
        {
        }
    }

    /// <summary>
    /// Reabilita movimento após um delay.
    /// </summary>
    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableMovement();
    }

    /// <summary>
    /// Força o jogador a olhar para uma direção específica.
    /// </summary>
    private void FaceDirection(bool faceRight)
    {
        if (_facingRight != faceRight)
        {
            FlipSprite();
        }
    }

    /// <summary>
    /// Força o reset do sistema de ataque (útil para debug).
    /// Use apenas se o ataque ficar "preso" e não funcionar mais.
    /// </summary>
    private void ResetAttackState()
    {
        _canAttack = true;
        _isAttacking = false; // Libera movimento após ataque

        // Esconde VFX de ataque
        HideAttackVfx();

        // Para todas as corrotinas de ataque em execução
        StopAllCoroutines();
    }

    /// <summary>
    /// Determina a direção específica do ataque baseada na direção visual atual e orientação do sprite.
    /// Converte VisualDirection para AttackDirection considerando o flip horizontal.
    /// </summary>
    /// <returns>Direção específica para ajuste do offset no AttackHandler</returns>
    private SlimeMec.Gameplay.AttackDirection GetAttackDirection()
    {
        switch (_currentVisualDirection)
        {
            case VisualDirection.South:
                return SlimeMec.Gameplay.AttackDirection.South;

            case VisualDirection.North:
                return SlimeMec.Gameplay.AttackDirection.North;

            case VisualDirection.Side:
                // Para ataques laterais, considera a direção do flip
                return _facingRight ? SlimeMec.Gameplay.AttackDirection.East : SlimeMec.Gameplay.AttackDirection.West;

            default:
                // Fallback para direção padrão
                return SlimeMec.Gameplay.AttackDirection.South;
        }
    }

    #endregion

    #region TODO: Sistemas a Implementar
    /*
    PRÓXIMOS PASSOS PARA IMPLEMENTAÇÃO:
    
    1. INPUT SYSTEM - CONFIGURAÇÃO FINAL:
       ✅ Arquivo InputSystem_Actions.inputactions criado
       ✅ Classe C# gerada automaticamente pelo Unity
       ✅ Event handlers implementados e corrigidos
       ✅ Mapeamento de teclas atualizado para sistema de inventário:
           • WASD/Arrow Keys: Movimento
           • Mouse/Ctrl: Ataque básico
           • E: Interação/Coleta de itens próximos
           • Space: Ataque especial
           • UseItem1-4: Usar itens dos slots 1-4 do inventário
       ✅ Erros de compilação resolvidos
    
    2. SISTEMA DE INTERAÇÕES E COLETA:
       - InteractionType enum (Shrink, Jump, Talk, CollectItem, Hide)
       - InteractableElement class (elementos do cenário que podem ser interagidos)
       - CollectibleItem class (itens coletáveis como frutas, poções, etc.)
       - Sistema de detecção de proximidade
       - UI de prompts de interação ("Pressione E para coletar")
       - Lógica de coleta: Interact próximo ao item → adiciona ao inventário
    
    3. SISTEMA DE INVENTÁRIO:
       - InventorySystem class (gerenciar 4 slots de itens)
       - ItemData ScriptableObject (dados dos itens: nome, efeitos, sprite)
       - UI do inventário (mostrar itens nos slots 1-4)
       - Sistema de uso de itens do inventário (UseItem1-4)
       - Efeitos dos itens (cura, buff de ataque, etc.)
       - Integração com PlayerAttributesHandler
    
    4. SISTEMA DE INIMIGOS:
       - EnemyHealth class (sistema de vida para inimigos)
       - Integração com o sistema de combate do PlayerController
    
    5. FUNCIONALIDADES ADICIONAIS:
       - Sistema de esconderijo (isHiding parameter)
       - Ações especiais ativadas por interação (Shrink, Jump)
       - Sistema de ataque especial
       - Feedback visual de dano/cura
    
    MAPEAMENTO DE FUNCIONALIDADES ATUAIS:
    ✅ Movimento básico com WASD/Arrow Keys
    ✅ Sistema de animação (isWalking, FacingRight)
    ✅ Combate básico (Attack01 trigger)
    ✅ Integração com PlayerAttributesHandler
    ✅ Sistema de flip de sprite
    ✅ Controles de movimento (enable/disable)
    ✅ Input System (totalmente implementado e funcional)
    ❌ Interações contextuais
    ❌ Sistema de coleta
    ❌ Detecção de inimigos
    ❌ Sistema de esconderijo
    
    MAPEAMENTO DE CONTROLES ATUALIZADO:
    🎮 WASD/Arrow Keys: Movimento
    🖱️ Mouse Left/Ctrl: Ataque básico
    ⌨️ E: Interação/Coleta (próximo de itens ou elementos interativos)
    ⌨️ Space: Ataque especial (preparado)
    ⌨️ UseItem1: Usar item do slot 1 do inventário
    ⌨️ UseItem2: Usar item do slot 2 do inventário  
    ⌨️ UseItem3: Usar item do slot 3 do inventário
    ⌨️ UseItem4: Usar item do slot 4 do inventário
    
    FLUXO DE GAMEPLAY:
    1. Jogador se aproxima de uma fruta/item coletável
    2. Aparece prompt "Pressione E para coletar"
    3. Jogador pressiona E → item vai para próximo slot livre do inventário
    4. Jogador pode usar o item pressionando UseItem1-4 conforme o slot
    5. Item é consumido e aplica efeito (cura, buff, etc.)
    */
    #endregion
}
