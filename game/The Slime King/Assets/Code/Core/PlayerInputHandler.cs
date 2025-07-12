using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Responsável por conectar os inputs do InputManager ao Animator do personagem.
/// Gerencia os parâmetros de animação de movimento, ataque e esconder/abaixar,
/// reagindo aos eventos de input do jogador, movimentando o personagem e exibindo os sprites corretos.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerInputHandler : MonoBehaviour
{
    /// <summary>
    /// Referência ao componente Animator para controlar as animações do personagem.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Direção atual do movimento do personagem.
    /// </summary>
    private Vector2 moveInput = Vector2.zero;

    /// <summary>
    /// Referência ao componente Rigidbody2D para movimentação física.
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Referência ao componente EntityStatus para obter atributos como velocidade.
    /// </summary>
    private EntityStatus entityStatus;

    /// <summary>
    /// Indica se os eventos de input já foram registrados.
    /// </summary>
    private bool isInputInitialized = false;

    /// <summary>
    /// Timestamp da próxima tentativa de inicialização do input.
    /// </summary>
    private float nextInputInitAttempt = 0f;

    // Referências aos sub-objetos de sprites
    [Header("Referências dos Sprites do Slime")]
    [Tooltip("GameObject com o sprite frontal do slime")]
    [SerializeField] private GameObject front;

    [Tooltip("GameObject com os efeitos visuais frontais")]
    [SerializeField] private GameObject vfx_front;

    [Tooltip("GameObject com o sprite traseiro do slime")]
    [SerializeField] private GameObject back;

    [Tooltip("GameObject com os efeitos visuais traseiros")]
    [SerializeField] private GameObject vfx_back;

    [Tooltip("GameObject com o sprite lateral do slime")]
    [SerializeField] private GameObject side;

    [Tooltip("GameObject com os efeitos visuais laterais")]
    [SerializeField] private GameObject vfx_side;

    [Tooltip("GameObject com a sombra do slime")]
    [SerializeField] private GameObject shadow;

    // Para flip lateral
    /// <summary>
    /// Referência ao SpriteRenderer do sprite lateral para flip horizontal.
    /// </summary>
    private SpriteRenderer sideRenderer;

    /// <summary>
    /// Referência ao SpriteRenderer dos efeitos visuais laterais para flip horizontal.
    /// </summary>
    private SpriteRenderer vfxSideRenderer;

    private bool isAttacking = false;

    /// <summary>
    /// Inicializa os componentes necessários e referências para o personagem.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        entityStatus = GetComponent<EntityStatus>();
        if (entityStatus == null)
            Debug.LogWarning("EntityStatus não encontrado no GameObject. O movimento usará velocidade 0.");

        // Busca SpriteRenderers para flip lateral
        if (side != null) sideRenderer = side.GetComponent<SpriteRenderer>();
        if (vfx_side != null) vfxSideRenderer = vfx_side.GetComponent<SpriteRenderer>();

        // Inicializa olhando para o sul (front ativo)
        UpdateSpriteDirection(Vector2.zero);
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

    /// <summary>
    /// Tenta inicializar e registrar os callbacks de input.
    /// </summary>
    /// <returns>True se inicializou com sucesso, False se falhou</returns>
    private bool TryInitializeInput()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("InputManager.Instance não encontrado! Tentando novamente em breve...");
            nextInputInitAttempt = Time.time + 0.5f;
            return false;
        }

        // Remove callbacks antigos para evitar duplicação
        UnsubscribeInputEvents();

        // Registra novos callbacks
        InputManager.Instance.MoveAction.performed += OnMovePerformed;
        InputManager.Instance.MoveAction.canceled += OnMoveCanceled;
        InputManager.Instance.AttackAction.performed += OnAttackPerformed;
        InputManager.Instance.CrouchAction.performed += OnCrouchPerformed;
        InputManager.Instance.CrouchAction.canceled += OnCrouchCanceled;

        isInputInitialized = true;
        Debug.Log("Input inicializado com sucesso!");
        return true;
    }

    /// <summary>
    /// Remove todos os callbacks de eventos de input.
    /// </summary>
    private void UnsubscribeInputEvents()
    {
        if (InputManager.Instance == null)
            return;

        InputManager.Instance.MoveAction.performed -= OnMovePerformed;
        InputManager.Instance.MoveAction.canceled -= OnMoveCanceled;
        InputManager.Instance.AttackAction.performed -= OnAttackPerformed;
        InputManager.Instance.CrouchAction.performed -= OnCrouchPerformed;
        InputManager.Instance.CrouchAction.canceled -= OnCrouchCanceled;
    }

    /// <summary>
    /// Callback executado quando o jogador inicia um movimento.
    /// Atualiza a direção do movimento, ativa a animação de caminhada e atualiza os sprites visíveis.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        animator.SetBool("isWalking", moveInput.sqrMagnitude > 0.01f);
        UpdateSpriteDirection(moveInput);
    }

    /// <summary>
    /// Callback executado quando o jogador para de se movimentar.
    /// Reseta a direção do movimento, desativa a animação de caminhada e atualiza os sprites visíveis.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        animator.SetBool("isWalking", false);
        UpdateSpriteDirection(Vector2.zero);
    }

    /// <summary>
    /// Callback executado quando o jogador realiza um ataque.
    /// Ativa a trigger de ataque no Animator.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        animator.SetTrigger("Attack01");
        isAttacking = true;
    }

    public void ResetAttack()
    {
        isAttacking = false;
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

    /// <summary>
    /// Atualiza a posição do personagem com base no input de movimento e velocidade.
    /// Não movimenta o personagem se estiver agachado/escondido ou atacando.
    /// Tenta inicializar o input se ainda não estiver inicializado.
    /// </summary>
    private void Update()
    {
        // Tenta inicializar o input se não estiver inicializado ainda
        if (!isInputInitialized && Time.time >= nextInputInitAttempt)
        {
            TryInitializeInput();
        }

        // Não movimenta se estiver escondido ou atacando
        if (animator.GetBool("isHiding") || isAttacking)
        {
            // Para o movimento imediatamente se estiver atacando
            if (isAttacking && rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        float speed = entityStatus != null ? entityStatus.GetSpeed() : 0f;

        // Movimento via Rigidbody2D (se existir)
        if (rb != null)
        {
            rb.linearVelocity = moveInput * speed;
        }
        else // Movimento via Transform (fallback)
        {
            transform.position += (Vector3)(moveInput * speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Ativa/desativa os sprites corretos conforme a direção do movimento.
    /// Controla qual conjunto de sprites deve estar visível (frontal, traseiro ou lateral)
    /// e aplica flip horizontal quando movendo para a esquerda.
    /// </summary>
    /// <param name="direction">Direção do movimento</param>
    private void UpdateSpriteDirection(Vector2 direction)
    {
        // Sempre exibe a sombra
        if (shadow != null) shadow.SetActive(true);

        // Parado ou Sul (baixo)
        if (direction == Vector2.zero || Mathf.Abs(direction.y) >= Mathf.Abs(direction.x) && direction.y <= 0)
        {
            SetActiveSprites(front: true, vfxFront: true, back: false, vfxBack: false, side: false, vfxSide: false);
        }
        // Norte (cima)
        else if (Mathf.Abs(direction.y) >= Mathf.Abs(direction.x) && direction.y > 0)
        {
            SetActiveSprites(front: false, vfxFront: false, back: true, vfxBack: true, side: false, vfxSide: false);
        }
        // Lateral (esquerda/direita)
        else
        {
            SetActiveSprites(front: false, vfxFront: false, back: false, vfxBack: false, side: true, vfxSide: true);

            // Flip horizontal se for para esquerda
            bool isLeft = direction.x < 0;
            if (sideRenderer != null) sideRenderer.flipX = isLeft;
            if (vfxSideRenderer != null) vfxSideRenderer.flipX = isLeft;
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
}
