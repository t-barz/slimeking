using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

/// <summary>
/// Controla o movimento do jogador e seu estado visual baseado na direção usando o novo Input System.
/// Esta classe gerencia todas as mecânicas de movimento do personagem, incluindo:
/// - Movimentação básica em 8 direções
/// - Sistema de pulo com arco parabólico
/// - Sistema de deslize
/// - Sistema de ataque
/// - Gerenciamento de estados visuais (frente, costas, lateral)
/// - Sistema de áudio para diferentes ações
/// </summary>
/// <remarks>
/// Requer os seguintes componentes:
/// - Rigidbody2D
/// - Animator com os parâmetros: "isWalking", "Jump", "Shrink", "Attack01"
/// - Collider2D
/// - Input System com ações de movimento e ataque configuradas
/// </remarks>
public class PlayerMovement : MonoBehaviour
{
    #region Campos Serializados
    [Header("Input")]
    [Tooltip("Referência para a ação de movimento do Input System")]
    [SerializeField] private InputActionReference movementAction;
    [Tooltip("Referência para a ação de ataque do Input System")]
    [SerializeField] private InputActionReference attackAction;

    [Header("Movement Settings")]
    [Tooltip("Velocidade de movimento do personagem")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Altura máxima do pulo")]
    [SerializeField] private float jumpHeight = 2f;

    [Header("Attack Settings")]
    [Tooltip("Duração da animação de ataque")]
    [SerializeField] private float attackDuration = 0.5f;
    [Tooltip("Som reproduzido ao atacar")]
    [SerializeField] private AudioClip attackSound;

    [Header("Audio Settings")]
    [Tooltip("Som reproduzido durante a movimentação")]
    [SerializeField] private AudioClip walkingSound;
    [Tooltip("Som reproduzido ao pular")]
    [SerializeField] private AudioClip jumpSound;
    [Tooltip("Som reproduzido ao deslizar")]
    [SerializeField] private AudioClip slideSound;
    [Tooltip("Volume dos sons")]
    [Range(0, 1)]
    [SerializeField] private float soundVolume = 1f;
    #endregion

    #region Campos Privados
    private GameObject[] frontObjects;
    private GameObject[] backObjects;
    private GameObject[] sideObjects;
    private GameObject vfxFront;
    private GameObject vfxBack;
    private GameObject vfxSide;
    private Collider2D[] playerColliders;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private bool isFacingLeft = false;
    private bool isSliding = false;
    private bool isJumping = false;
    private bool isAttacking = false;
    private static readonly int Attack01 = Animator.StringToHash("Attack01");

    private float walkingSoundInterval = 0.5f;
    private float lastWalkingSoundTime;
    #endregion

    #region Métodos Unity
    /// <summary>
    /// Inicializa os componentes necessários e configura o sistema de áudio
    /// </summary>
    private void Awake()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerColliders = GetComponents<Collider2D>();

        // Initialize visual objects arrays
        frontObjects = GetObjectsByNameContains("front");
        backObjects = GetObjectsByNameContains("back");
        sideObjects = GetObjectsByNameContains("side");

        // Initialize VFX objects
        vfxFront = transform.Find("vfx_front")?.gameObject;
        vfxBack = transform.Find("vfx_back")?.gameObject;
        vfxSide = transform.Find("vfx_side")?.gameObject;

        // Ensure VFX objects are initially inactive
        if (vfxFront) vfxFront.SetActive(false);
        if (vfxBack) vfxBack.SetActive(false);
        if (vfxSide) vfxSide.SetActive(false);

        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = soundVolume;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // Set to 2D sound
    }

    /// <summary>
    /// Configura o estado visual inicial do personagem
    /// </summary>
    private void Start()
    {
        UpdateVisualState(Vector2.down);
    }

    /// <summary>
    /// Ativa as ações do Input System e inscreve os callbacks
    /// </summary>
    private void OnEnable()
    {
        if (movementAction != null)
        {
            movementAction.action.performed += OnMovementPerformed;
            movementAction.action.canceled += OnMovementCanceled;
            movementAction.action.Enable();
        }

        // NEW: Attack input subscription
        if (attackAction != null)
        {
            attackAction.action.performed += OnAttackPerformed;
            attackAction.action.Enable();
        }
    }

    /// <summary>
    /// Desativa as ações do Input System e remove os callbacks
    /// </summary>
    private void OnDisable()
    {
        if (movementAction != null)
        {
            movementAction.action.performed -= OnMovementPerformed;
            movementAction.action.canceled -= OnMovementCanceled;
            movementAction.action.Disable();
        }

        // NEW: Attack input cleanup
        if (attackAction != null)
        {
            attackAction.action.performed -= OnAttackPerformed;
            attackAction.action.Disable();
        }

        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Gerencia o movimento do personagem usando física
    /// </summary>
    private void FixedUpdate()
    {
        if (!isAttacking && !isSliding && !isJumping)
        {
            rb.linearVelocity = moveInput * moveSpeed * Time.fixedDeltaTime * 120f;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    #endregion

    #region Manipulação de Input
    /// <summary>
    /// Manipula o input de movimento quando uma ação é executada
    /// </summary>
    /// <param name="context">Contexto da ação de input</param>
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (moveInput != Vector2.zero && !isAttacking) // NEW: Check attack state
        {
            UpdateVisualState(moveInput);
            animator.SetBool("isWalking", true);

            // Try to play walking sound with interval check
            if (Time.time - lastWalkingSoundTime >= walkingSoundInterval)
            {
                PlayWalkingSound();
                lastWalkingSoundTime = Time.time;
            }
        }
    }

    /// <summary>
    /// Manipula o fim do input de movimento
    /// </summary>
    /// <param name="context">Contexto da ação de input</param>
    private void OnMovementCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
        animator.SetBool("isWalking", false);

        // Stop walking sound
        if (audioSource.isPlaying && audioSource.clip == walkingSound)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Manipula o input de ataque
    /// </summary>
    /// <param name="context">Contexto da ação de input</param>
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!isAttacking && !isJumping && !isSliding)
        {
            StartCoroutine(AttackCoroutine());
        }
    }
    #endregion

    #region Sistema de Movimento
    /// <summary>
    /// Inicia uma animação de deslize em direção a um destino específico
    /// </summary>
    /// <param name="destination">Posição final do deslize</param>
    public void Slide(Vector3 destination)
    {
        if (!isSliding && animator != null)
        {
            // Play slide sound
            if (slideSound != null && audioSource != null)
            {
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = slideSound;
                audioSource.Play();
            }
            StartCoroutine(SlideCoroutine(destination));
        }
    }

    /// <summary>
    /// Corrotina que executa a animação de deslize
    /// </summary>
    /// <param name="destination">Posição final do deslize</param>
    private IEnumerator SlideCoroutine(Vector3 destination)
    {
        isSliding = true;
        const float MOVE_DURATION = 0.75f;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // Disable colliders during slide
        foreach (var collider in playerColliders)
        {
            collider.enabled = false;
        }

        animator.SetTrigger("Shrink");


        while (elapsedTime < MOVE_DURATION)
        {
            // Calcula a fração do tempo decorrido
            float t = elapsedTime / MOVE_DURATION;
            // Interpola a posição entre o início e o destino
            transform.position = Vector3.Lerp(startPosition, destination, t);

            elapsedTime += Time.deltaTime;
            yield return null; // espera o próximo frame
        }

        // Garante que a posição final seja exatamente o destino
        //transform.position = destination;


        // Re-enable colliders
        foreach (var collider in playerColliders)
        {
            collider.enabled = true;
        }

        isSliding = false;
    }


    /// <summary>
    /// Inicia um pulo em direção a um destino específico
    /// </summary>
    /// <param name="destination">Posição final do pulo</param>
    public void Jump(Vector3 destination)
    {
        if (!isJumping && animator != null)
        {
            // Play jump sound
            if (jumpSound != null)
            {
                audioSource.loop = false;
                audioSource.clip = jumpSound;
                audioSource.Play();
            }
            StartCoroutine(JumpCoroutine(destination));
        }
    }

    /// <summary>
    /// Corrotina que executa a animação de pulo em arco
    /// </summary>
    /// <param name="destination">Posição final do pulo</param>
    private IEnumerator JumpCoroutine(Vector3 destination)
    {
        isJumping = true;
        const float MOVE_DURATION = 0.5f;

        animator.SetTrigger("Jump");
        Vector3 startPosition = transform.position;
        float startTime = Time.time;
        float totalDistance = Vector3.Distance(startPosition, destination);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
               !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            float elapsedTime = Time.time - startTime;
            float remainingTime = Mathf.Max(MOVE_DURATION - elapsedTime, Time.deltaTime);

            Vector3 currentPosition = transform.position;
            Vector3 toDestination = destination - currentPosition;

            if (toDestination.magnitude > 0.01f)
            {
                float speed = toDestination.magnitude / remainingTime;

                // Calculate progress for arc movement (0 to 1)
                float progress = 1f - (toDestination.magnitude / totalDistance);
                float verticalOffset = jumpHeight * Mathf.Sin(progress * Mathf.PI);

                // Move towards destination with arc
                Vector3 targetPosition = Vector3.MoveTowards(
                    currentPosition,
                    destination,
                    speed * Time.deltaTime
                );
                targetPosition.y += verticalOffset;

                transform.position = targetPosition;
            }

            yield return null;
        }

        transform.position = destination;
        isJumping = false;
    }


    /// <summary>
    /// Corrotina que executa a animação de ataque
    /// </summary>
    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        moveInput = Vector2.zero; // Stop movement during attack
        animator.SetBool("isWalking", false);

        // Determine which VFX to activate based on current visual state
        GameObject activeVfx = null;
        if (frontObjects[0]?.activeSelf == true && vfxFront != null)
        {
            activeVfx = vfxFront;
        }
        else if (backObjects[0]?.activeSelf == true && vfxBack != null)
        {
            activeVfx = vfxBack;
        }
        else if (sideObjects[0]?.activeSelf == true && vfxSide != null)
        {
            activeVfx = vfxSide;
            // Ajusta a escala do VFX lateral de acordo com a direção
            if (isFacingLeft)
            {
                activeVfx.transform.localScale = new Vector3(-Mathf.Abs(activeVfx.transform.localScale.x),
                                                           activeVfx.transform.localScale.y,
                                                           activeVfx.transform.localScale.z);
            }
            else
            {
                activeVfx.transform.localScale = new Vector3(Mathf.Abs(activeVfx.transform.localScale.x),
                                                           activeVfx.transform.localScale.y,
                                                           activeVfx.transform.localScale.z);
            }
        }

        // Activate the appropriate VFX
        if (activeVfx != null)
        {
            activeVfx.SetActive(true);
        }

        // Trigger attack animation
        animator.SetTrigger(Attack01);

        // Play attack sound
        if (attackSound != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = attackSound;
            audioSource.Play();
        }

        yield return new WaitForSeconds(attackDuration);

        // Deactivate VFX and re-enable movement
        if (activeVfx != null)
        {
            activeVfx.SetActive(false);
        }

        // Re-enable movement
        isAttacking = false;
        if (moveInput != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
            UpdateVisualState(moveInput);
        }
    }
    #endregion

    #region Gerenciamento Visual
    /// <summary>
    /// Atualiza o estado visual do personagem baseado na direção do movimento
    /// </summary>
    /// <param name="direction">Direção do movimento</param>
    private void UpdateVisualState(Vector2 direction)
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        if (absX > absY)
        {
            // Horizontal movement
            SetActiveObjects(sideObjects, true);
            SetActiveObjects(frontObjects, false);
            SetActiveObjects(backObjects, false);

            bool shouldFaceLeft = direction.x < 0;
            if (shouldFaceLeft != isFacingLeft)
            {
                isFacingLeft = shouldFaceLeft;
                FlipSideObjects(isFacingLeft);
            }
        }
        else
        {
            // Vertical movement
            if (direction.y > 0)
            {
                // Moving up
                SetActiveObjects(backObjects, true);
                SetActiveObjects(frontObjects, false);
                SetActiveObjects(sideObjects, false);
            }
            else
            {
                // Moving down
                SetActiveObjects(frontObjects, true);
                SetActiveObjects(backObjects, false);
                SetActiveObjects(sideObjects, false);
            }
        }
    }

    /// <summary>
    /// Ativa ou desativa um conjunto de objetos
    /// </summary>
    /// <param name="objects">Array de objetos para alterar o estado</param>
    /// <param name="active">Estado desejado (ativo/inativo)</param>
    private void SetActiveObjects(GameObject[] objects, bool active)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    /// <summary>
    /// Inverte a escala horizontal dos objetos laterais para simular mudança de direção
    /// </summary>
    /// <param name="faceLeft">True se deve virar para a esquerda, False para direita</param>
    private void FlipSideObjects(bool faceLeft)
    {
        foreach (var obj in sideObjects)
        {
            if (obj != null)
            {
                Vector3 scale = obj.transform.localScale;
                scale.x = faceLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
                obj.transform.localScale = scale;
            }
        }
    }
    #endregion

    #region Sistema de Áudio
    /// <summary>
    /// Reproduz o som de passos se não estiver já tocando
    /// </summary>
    public void PlayWalkingSound()
    {
        if (walkingSound != null && audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = false;
            audioSource.clip = walkingSound;
            audioSource.Play();
        }
    }
    #endregion

    #region Métodos Auxiliares
    /// <summary>
    /// Encontra objetos filhos cujos nomes contêm uma string específica
    /// </summary>
    /// <param name="nameContains">String que deve estar contida no nome do objeto</param>
    /// <returns>Array de GameObjects que correspondem ao critério</returns>
    private GameObject[] GetObjectsByNameContains(string nameContains)
    {
        // Get all child transforms
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        // Count objects that match the name criteria
        int matchCount = 0;
        foreach (Transform child in allChildren)
        {
            if (child != transform && child.name.ToLower().Contains(nameContains))
            {
                matchCount++;
            }
        }

        // Create and fill array with matching objects
        GameObject[] matchingObjects = new GameObject[matchCount];
        int index = 0;
        foreach (Transform child in allChildren)
        {
            if (child != transform && child.name.ToLower().Contains(nameContains))
            {
                matchingObjects[index] = child.gameObject;
                index++;
            }
        }

        return matchingObjects;
    }
    #endregion
}