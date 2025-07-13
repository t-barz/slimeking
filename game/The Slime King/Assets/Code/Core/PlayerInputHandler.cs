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
    private Animator animator;
    private Rigidbody2D rb;
    private EntityStatus entityStatus;
    private float cachedSpeed;
    #endregion

    #region Estado do Movimento
    private Vector2 moveInput = Vector2.zero;
    private Vector2 lastDirection = Vector2.down; // Sul por padrão
    private bool isAttacking = false;
    #endregion

    #region Inicialização de Input
    private bool isInputInitialized = false;
    private float nextInputInitAttempt = 0f;
    private const float INPUT_RETRY_INTERVAL = 0.5f;
    #endregion

    #region Configurações de Sprites
    [Header("Referências dos Sprites")]
    [SerializeField] private GameObject front;
    [SerializeField] private GameObject vfx_front;
    [SerializeField] private GameObject back;
    [SerializeField] private GameObject vfx_back;
    [SerializeField] private GameObject side;
    [SerializeField] private GameObject vfx_side;
    [SerializeField] private GameObject shadow;
    
    // Cache para evitar GetComponent repetidos
    private SpriteRenderer sideRenderer;
    private SpriteRenderer vfxSideRenderer;
    #endregion

    #region Configurações de Ataque
    [Header("Ataque")]
    [SerializeField] private GameObject attackPrefab;
    [SerializeField] private Vector2 attackOffsetFront = new Vector2(0f, 0.5f);
    [SerializeField] private Vector2 attackOffsetBack = new Vector2(0f, 0.5f);
    [SerializeField] private Vector2 attackOffsetSide = new Vector2(0.5f, 0f);
    #endregion

    #region Inicialização e Ciclo de Vida
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

    private void OnEnable()
    {
        TryInitializeInput();
    }

    private void OnDisable()
    {
        UnsubscribeInputEvents();
        isInputInitialized = false;
    }
    #endregion

    #region Gerenciamento de Input
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

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        animator.SetBool("isWalking", false);
        // Mantém a última direção visual
    }

    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        animator.SetTrigger("Attack01");
        isAttacking = true;
        
        if (attackPrefab != null)
        {
            SpawnAttack();
        }
    }

    private void OnCrouchPerformed(InputAction.CallbackContext ctx)
    {
        animator.SetBool("isHiding", true);
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        animator.SetBool("isHiding", false);
    }
    #endregion

    #region Atualização e Movimento
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

    private void UpdateSpeedCache()
    {
        cachedSpeed = entityStatus != null ? entityStatus.GetSpeed() : 0f;
    }

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
    private void SpawnAttack()
    {
        Vector3 spawnPosition = CalculateAttackPosition();
        
        // Instancia o ataque
        GameObject attackObj = Instantiate(attackPrefab, spawnPosition, Quaternion.identity);
        
        // Configura a visualização do ataque
        SetupAttackVisuals(attackObj);
    }

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
            case FacingDirection.East:
            case FacingDirection.West:
                if (attackSide != null)
                {
                    attackSide.gameObject.SetActive(true);
                    SpriteRenderer attackSideRenderer = attackSide.GetComponent<SpriteRenderer>();
                    if (attackSideRenderer != null)
                    {
                        attackSideRenderer.flipX = facing == FacingDirection.West;
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
    /// </summary>
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
    /// Reseta o estado de ataque. Deve ser chamado por eventos de animação.
    /// </summary>
    public void ResetAttack()
    {
        isAttacking = false;
    }
    #endregion
}
