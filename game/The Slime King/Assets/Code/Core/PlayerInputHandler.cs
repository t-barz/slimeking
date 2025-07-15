using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Responsável por processar os inputs do jogador e gerenciar o movimento e animações do personagem.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerInputHandler : MonoBehaviour
{
    #region Componentes e Referências
    /// <summary>
    /// Referência ao componente Animator para controlar as animações do personagem.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Referência ao componente Rigidbody2D para movimentação física.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Referência ao componente EntityStatus para obter atributos como velocidade.
    /// </summary>
    private EntityStatus entityStatus;

    /// <summary>
    /// Cache da velocidade do personagem, atualizado periodicamente para otimização.
    /// </summary>
    private float cachedSpeed;
    #endregion

    #region Estado do Movimento
    /// <summary>
    /// Direção atual do movimento do personagem baseada nos inputs.
    /// </summary>
    private Vector2 moveInput = Vector2.zero;

    /// <summary>
    /// Última direção válida do movimento, usada para manter a orientação visual quando parado.
    /// </summary>
    private Vector2 lastDirection = Vector2.down; // Sul por padrão

    /// <summary>
    /// Indica se o personagem está atualmente realizando um ataque.
    /// </summary>
    private bool isAttacking = false;
    #endregion

    #region Inicialização de Input
    /// <summary>
    /// Indica se os eventos de input já foram registrados com sucesso.
    /// </summary>
    private bool isInputInitialized = false;

    /// <summary>
    /// Timestamp para a próxima tentativa de inicialização de input.
    /// </summary>
    private float nextInputInitAttempt = 0f;

    /// <summary>
    /// Intervalo entre tentativas de inicialização do input em segundos.
    /// </summary>
    private const float INPUT_RETRY_INTERVAL = 0.5f;
    #endregion

    #region Configurações de Sprites
    /// <summary>
    /// GameObject com o sprite frontal do personagem.
    /// </summary>
    [Header("Referências dos Sprites")]
    [SerializeField] private GameObject front;

    /// <summary>
    /// GameObject com os efeitos visuais frontais.
    /// </summary>
    [SerializeField] private GameObject vfx_front;

    /// <summary>
    /// GameObject com o sprite traseiro do personagem.
    /// </summary>
    [SerializeField] private GameObject back;

    /// <summary>
    /// GameObject com os efeitos visuais traseiros.
    /// </summary>
    [SerializeField] private GameObject vfx_back;

    /// <summary>
    /// GameObject com o sprite lateral do personagem.
    /// </summary>
    [SerializeField] private GameObject side;

    /// <summary>
    /// GameObject com os efeitos visuais laterais.
    /// </summary>
    [SerializeField] private GameObject vfx_side;

    /// <summary>
    /// GameObject com a sombra do personagem.
    /// </summary>
    [SerializeField] private GameObject shadow;

    /// <summary>
    /// Referência ao SpriteRenderer do sprite lateral para controle de flip horizontal.
    /// </summary>
    private SpriteRenderer sideRenderer;

    /// <summary>
    /// Referência ao SpriteRenderer dos efeitos visuais laterais para controle de flip horizontal.
    /// </summary>
    private SpriteRenderer vfxSideRenderer;
    #endregion

    #region Configurações de Ataque
    /// <summary>
    /// Prefab do objeto de ataque a ser instanciado
    /// </summary>
    [Header("Ataque")]
    [SerializeField] private GameObject attackPrefab;

    /// <summary>
    /// Duração do ataque em segundos
    /// </summary>
    [SerializeField] private float attackDuration = 0.5f;

    /// <summary>
    /// Offset de posicionamento do ataque em relação ao jogador quando olhando para frente.
    /// </summary>
    [SerializeField] private Vector2 attackOffsetFront = new Vector2(0f, 0.5f);

    /// <summary>
    /// Offset de posicionamento do ataque em relação ao jogador quando olhando para trás.
    /// </summary>
    [SerializeField] private Vector2 attackOffsetBack = new Vector2(0f, 0.5f);

    /// <summary>
    /// Offset de posicionamento do ataque em relação ao jogador quando olhando para os lados.
    /// </summary>
    [SerializeField] private Vector2 attackOffsetSide = new Vector2(0.5f, 0f);
    #endregion

    #region Inicialização e Ciclo de Vida
    /// <summary>
    /// Inicializa os componentes, referências e configurações iniciais do personagem.
    /// </summary>
    private void Awake()
    {
        // Obtém referências de componentes
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        entityStatus = GetComponent<EntityStatus>();

        // Cache de componentes para performance
        if (side != null) sideRenderer = side.GetComponent<SpriteRenderer>();
        if (vfx_side != null) vfxSideRenderer = vfx_side.GetComponent<SpriteRenderer>();

        // Configura direção inicial
        UpdateSpriteDirection(lastDirection);

        // Cache da velocidade inicial
        UpdateSpeedCache();
    }

    /// <summary>
    /// Registra os callbacks para os eventos de input quando o objeto é habilitado.
    /// </summary>
    private void OnEnable()
    {
        TryInitializeInput();
    }

    /// <summary>
    /// Remove os callbacks dos eventos de input quando o objeto é desabilitado.
    /// </summary>
    private void OnDisable()
    {
        UnsubscribeInputEvents();
        isInputInitialized = false;
    }
    #endregion

    #region Gerenciamento de Input
    /// <summary>
    /// Tenta inicializar e registrar os callbacks de input.
    /// </summary>
    /// <returns>True se inicializou com sucesso, False se falhou</returns>
    private bool TryInitializeInput()
    {
        if (InputManager.Instance == null)
        {
            nextInputInitAttempt = Time.time + INPUT_RETRY_INTERVAL;
            return false;
        }

        UnsubscribeInputEvents();

        InputManager.Instance.MoveAction.performed += OnMovePerformed;
        InputManager.Instance.MoveAction.canceled += OnMoveCanceled;
        InputManager.Instance.AttackAction.performed += OnAttackPerformed;
        InputManager.Instance.CrouchAction.performed += OnCrouchPerformed;
        InputManager.Instance.CrouchAction.canceled += OnCrouchCanceled;

        isInputInitialized = true;
        return true;
    }

    /// <summary>
    /// Remove todos os callbacks de eventos de input.
    /// </summary>
    private void UnsubscribeInputEvents()
    {
        if (InputManager.Instance == null) return;

        InputManager.Instance.MoveAction.performed -= OnMovePerformed;
        InputManager.Instance.MoveAction.canceled -= OnMoveCanceled;
        InputManager.Instance.AttackAction.performed -= OnAttackPerformed;
        InputManager.Instance.CrouchAction.performed -= OnCrouchPerformed;
        InputManager.Instance.CrouchAction.canceled -= OnCrouchCanceled;
    }
    #endregion

    #region Callbacks de Input
    /// <summary>
    /// Callback executado quando o jogador inicia um movimento.
    /// Atualiza a direção do movimento, ativa a animação de caminhada e atualiza os sprites visíveis.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("isWalking", isMoving);

        // Atualiza a direção apenas se estiver realmente movendo
        if (isMoving)
        {
            lastDirection = moveInput.normalized;
            UpdateSpriteDirection(lastDirection);
        }
    }

    /// <summary>
    /// Callback executado quando o jogador para de se movimentar.
    /// Reseta a direção do movimento, desativa a animação de caminhada mas mantém a direção visual.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        animator.SetBool("isWalking", false);
        // Mantém a última direção visual
    }

    /// <summary>
    /// Callback executado quando o jogador realiza um ataque.
    /// Verifica o cooldown, ativa a trigger de ataque no Animator e instancia o objeto de ataque.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        // Verifica se já tem um ataque em andamento
        if (isAttacking)
            return;

        // Verifica se o EntityStatus existe
        if (entityStatus == null)
        {
            Debug.LogWarning("EntityStatus não encontrado no PlayerInputHandler");
            return;
        }

        // Verifica se o ataque está disponível (cooldown)
        if (!entityStatus.IsBasicAttackAvailable())
        {
            // Opcional: Feedback visual/sonoro de que o ataque está em cooldown
            float remainingCooldown = entityStatus.GetBasicAttackCooldownRemaining();
            Debug.Log($"Ataque em cooldown: {remainingCooldown:F1}s restantes");

            // Aqui poderia mostrar um efeito visual para o jogador
            // Ex: animator.SetTrigger("AttackBlocked");

            return;
        }

        // Se chegou aqui, pode atacar
        animator.SetTrigger("Attack01");

        if (attackPrefab != null)
        {
            // Marca o início do ataque
            isAttacking = true;

            // Instancia o ataque
            SpawnAttack();

            // Inicia o cooldown no EntityStatus
            entityStatus.UseBasicAttack();
        }
    }

    /// <summary>
    /// Callback executado quando o jogador pressiona o botão de agachar/esconder.
    /// Ativa o parâmetro isHiding no Animator.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        animator.SetBool("isHiding", true);
    }

    /// <summary>
    /// Callback executado quando o jogador solta o botão de agachar/esconder.
    /// Desativa o parâmetro isHiding no Animator.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        animator.SetBool("isHiding", false);
    }
    #endregion

    #region Atualização e Movimento
    /// <summary>
    /// Atualiza o personagem a cada frame. Tenta inicializar o input, atualiza a velocidade e gerencia o movimento.
    /// </summary>
    private void Update()
    {
        // Tenta inicializar input se necessário
        if (!isInputInitialized && Time.time >= nextInputInitAttempt)
        {
            TryInitializeInput();
        }

        // Atualiza cache de velocidade a cada segundo
        if (Time.frameCount % 30 == 0)
        {
            UpdateSpeedCache();
        }

        UpdateMovement();
    }

    /// <summary>
    /// Atualiza o cache de velocidade do personagem para otimização.
    /// </summary>
    private void UpdateSpeedCache()
    {
        cachedSpeed = entityStatus != null ? entityStatus.GetSpeed() : 0f;
    }

    /// <summary>
    /// Atualiza a posição do personagem com base no input de movimento e velocidade.
    /// Não movimenta o personagem se estiver agachado/escondido ou atacando.
    /// </summary>
    private void UpdateMovement()
    {
        // Não move se estiver escondido ou atacando
        if (animator.GetBool("isHiding") || isAttacking)
        {
            if (isAttacking && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        // Move o personagem
        if (rb != null)
        {
            rb.linearVelocity = moveInput * cachedSpeed;
        }
        else
        {
            transform.position += (Vector3)(moveInput * cachedSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Gerenciamento Visual
    /// <summary>
    /// Ativa/desativa os sprites corretos conforme a direção do movimento.
    /// Controla qual conjunto de sprites deve estar visível (frontal, traseiro ou lateral)
    /// e aplica flip horizontal quando movendo para a esquerda.
    /// </summary>
    /// <param name="direction">Direção do movimento</param>
    private void UpdateSpriteDirection(Vector2 direction)
    {
        if (shadow != null) shadow.SetActive(true);

        FacingDirection facing = GetFacingDirection(direction);

        switch (facing)
        {
            case FacingDirection.South:
                SetActiveSprites(front: true, vfxFront: true, back: false, vfxBack: false, side: false, vfxSide: false);
                break;
            case FacingDirection.North:
                SetActiveSprites(front: false, vfxFront: false, back: true, vfxBack: true, side: false, vfxSide: false);
                break;
            case FacingDirection.East:
                SetActiveSprites(front: false, vfxFront: false, back: false, vfxBack: false, side: true, vfxSide: true);
                if (sideRenderer != null) sideRenderer.flipX = false;
                if (vfxSideRenderer != null) vfxSideRenderer.flipX = false;
                break;
            case FacingDirection.West:
                SetActiveSprites(front: false, vfxFront: false, back: false, vfxBack: false, side: true, vfxSide: true);
                if (sideRenderer != null) sideRenderer.flipX = true;
                if (vfxSideRenderer != null) vfxSideRenderer.flipX = true;
                break;
        }
    }

    /// <summary>
    /// Ativa/desativa os sub-objetos de sprites conforme a direção.
    /// Método utilitário para gerenciar a visibilidade dos sprites.
    /// </summary>
    /// <param name="front">Se o sprite frontal deve estar visível</param>
    /// <param name="vfxFront">Se os efeitos visuais frontais devem estar visíveis</param>
    /// <param name="back">Se o sprite traseiro deve estar visível</param>
    /// <param name="vfxBack">Se os efeitos visuais traseiros devem estar visíveis</param>
    /// <param name="side">Se o sprite lateral deve estar visível</param>
    /// <param name="vfxSide">Se os efeitos visuais laterais devem estar visíveis</param>
    private void SetActiveSprites(bool front, bool vfxFront, bool back, bool vfxBack, bool side, bool vfxSide)
    {
        if (this.front != null) this.front.SetActive(front);
        if (this.vfx_front != null) this.vfx_front.SetActive(vfxFront);
        if (this.back != null) this.back.SetActive(back);
        if (this.vfx_back != null) this.vfx_back.SetActive(vfxBack);
        if (this.side != null) this.side.SetActive(side);
        if (this.vfx_side != null) this.vfx_side.SetActive(vfxSide);
    }
    #endregion

    #region Sistema de Ataque
    /// <summary>
    /// Instancia o objeto de ataque na direção que o jogador está olhando.
    /// </summary>
    private void SpawnAttack()
    {
        // Verifica se o prefab de ataque existe
        if (attackPrefab == null)
        {
            Debug.LogWarning("Prefab de ataque não configurado!");
            isAttacking = false;
            return;
        }

        // Calcula a posição do ataque baseada na direção
        Vector3 attackPosition = CalculateAttackPosition();

        // Instancia o objeto de ataque
        GameObject attackObject = Instantiate(attackPrefab, attackPosition, Quaternion.identity);

        // Configura a visualização do ataque
        SetupAttackVisuals(attackObject);

        // Destrói o objeto após a duração especificada
        Destroy(attackObject, attackDuration);

        // Agenda a liberação do estado de ataque
        StartCoroutine(ResetAttackStateAfterDelay());
    }

    /// <summary>
    /// Reseta o estado de ataque após um período determinado.
    /// </summary>
    private IEnumerator ResetAttackStateAfterDelay()
    {
        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    /// <summary>
    /// Calcula a posição onde o ataque será instanciado com base na direção atual.
    /// </summary>
    /// <returns>Posição para instanciar o ataque</returns>
    private Vector3 CalculateAttackPosition()
    {
        Vector3 position = transform.position;
        FacingDirection facing = GetFacingDirection(lastDirection);

        switch (facing)
        {
            case FacingDirection.South:
                position += new Vector3(attackOffsetFront.x, attackOffsetFront.y, 0);
                break;
            case FacingDirection.North:
                position += new Vector3(attackOffsetBack.x, attackOffsetBack.y, 0);
                break;
            case FacingDirection.East:
                position += new Vector3(attackOffsetSide.x, attackOffsetSide.y, 0);
                break;
            case FacingDirection.West:
                position += new Vector3(-attackOffsetSide.x, attackOffsetSide.y, 0);
                break;
        }

        return position;
    }

    /// <summary>
    /// Configura os sprites do objeto de ataque de acordo com a direção atual do personagem.
    /// Ativa apenas o sprite de ataque apropriado para a direção atual e aplica flip quando necessário.
    /// </summary>
    /// <param name="attackObj">Objeto de ataque recém-instanciado</param>
    private void SetupAttackVisuals(GameObject attackObj)
    {
        // Obtém sub-objetos do ataque
        Transform attackBack = attackObj.transform.Find("attack_back");
        Transform attackSide = attackObj.transform.Find("attack_side");
        Transform attackFront = attackObj.transform.Find("attack_front");

        // Desativa todos inicialmente
        if (attackBack != null) attackBack.gameObject.SetActive(false);
        if (attackSide != null) attackSide.gameObject.SetActive(false);
        if (attackFront != null) attackFront.gameObject.SetActive(false);

        // Ativa apenas o correto para a direção atual
        FacingDirection facing = GetFacingDirection(lastDirection);

        switch (facing)
        {
            case FacingDirection.South:
                if (attackFront != null) attackFront.gameObject.SetActive(true);
                break;
            case FacingDirection.North:
                if (attackBack != null) attackBack.gameObject.SetActive(true);
                break;
            case FacingDirection.West:
            case FacingDirection.East:
                if (attackSide != null)
                {
                    attackSide.gameObject.SetActive(true);
                    SpriteRenderer attackSideRenderer = attackSide.GetComponent<SpriteRenderer>();
                    CapsuleCollider2D attackSideCollider = attackSide.GetComponent<CapsuleCollider2D>();

                    bool isFlipped = facing == FacingDirection.East;

                    // Configura o flip do sprite
                    if (attackSideRenderer != null)
                    {
                        attackSideRenderer.flipX = isFlipped;
                    }

                    // Ajusta o collider para acompanhar o flip
                    if (attackSideCollider != null && isFlipped)
                    {
                        // Inverte a posição do offset no eixo X quando virado
                        Vector2 currentOffset = attackSideCollider.offset;
                        attackSideCollider.offset = new Vector2(-currentOffset.x, currentOffset.y);
                    }
                }
                break;
        }
    }
    #endregion

    #region Utilidades de Direção
    /// <summary>
    /// Direções possíveis para o personagem.
    /// </summary>
    private enum FacingDirection
    {
        North,
        South,
        East,
        West
    }

    /// <summary>
    /// Determina a direção que o personagem está olhando com base no vetor de direção.
    /// Converte um vetor de direção (Vector2) para uma direção cardeal (FacingDirection).
    /// </summary>
    /// <param name="direction">Vetor de direção normalizado</param>
    /// <returns>Direção cardeal (Norte, Sul, Leste, Oeste)</returns>
    private FacingDirection GetFacingDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return FacingDirection.South; // Padrão

        // Verifica se é movimento vertical ou horizontal
        if (Mathf.Abs(direction.y) >= Mathf.Abs(direction.x))
        {
            return direction.y > 0 ? FacingDirection.North : FacingDirection.South;
        }
        else
        {
            return direction.x > 0 ? FacingDirection.East : FacingDirection.West;
        }
    }
    #endregion

    #region Métodos Públicos
    /// <summary>
    /// Reseta o estado de ataque. Pode ser chamado por eventos de animação
    /// quando a animação de ataque terminar para permitir novos movimentos.
    /// </summary>
    public void ResetAttack()
    {
        isAttacking = false;
    }
    #endregion
}
