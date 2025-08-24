using UnityEngine;

public class WindController : MonoBehaviour
{
    #region Inspector Configuration

    [Header("⚙️ Configurações de Movimento")]
    [Tooltip("Velocidade de movimento horizontal (unidades por segundo)")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Direção do movimento horizontal")]
    [SerializeField] private MovementDirection direction = MovementDirection.Right;

    [Header("🌳 Detecção de Obstáculos")]
    [Tooltip("Distância da detecção para obstáculos")]
    [SerializeField] private float detectionDistance = 2f;

    [Tooltip("Tamanho da área de detecção")]
    [SerializeField] private Vector2 detectionBoxSize = new Vector2(0.5f, 1f);

    [Tooltip("LayerMask para objetos WindShakers")]
    [SerializeField] private LayerMask windShakersLayerMask = 1 << 6; // Default para layer WindShakers

    [Header("🌊 Animação de Shake")]
    [Tooltip("Nome do trigger do animator para ativar o shake")]
    [SerializeField] private string shakeTriggerName = "Shake";

    [Header("🎨 Configurações Visuais")]
    [Tooltip("Aplica flip automático no sprite baseado na direção")]
    [SerializeField] private bool autoFlip = true;

    [Header("🔧 Debug")]
    [Tooltip("Mostra informações de debug no Scene View")]
    [SerializeField] private bool showDebugInfo = false;

    #endregion

    #region Private Variables

    public enum MovementDirection
    {
        Left,
        Right
    }

    // Cached components
    private Transform cachedTransform;
    private SpriteRenderer spriteRenderer;

    // Movement variables
    private Vector2 movementVector;
    private Vector2 movementDirection;
    private bool isMoving = true;

    // Detection variables
    private Collider2D[] overlapResults = new Collider2D[5]; // Pool de resultados para evitar GC
    private Vector2 detectionCenter;
    private ContactFilter2D contactFilter;

    // Static vectors for performance (zero allocation)
    private static readonly Vector2 RightVector = Vector2.right;
    private static readonly Vector2 LeftVector = Vector2.left;

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        InitializeComponents();
        SetupMovement();
        ApplyVisualSettings();
    }

    void Update()
    {
        if (isMoving && moveSpeed > 0f)
        {
            // Sempre mover o objeto
            MoveObject();

            // Verificar se há objetos para fazer shake
            if (CheckForObstacle())
            {
                TriggerShakeOnDetectedObjects();
            }
        }
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        // Cache components for performance
        cachedTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Setup contact filter for collision detection
        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(windShakersLayerMask);
        contactFilter.useTriggers = true;
    }

    private void SetupMovement()
    {
        // Calculate movement vector and direction based on direction
        switch (direction)
        {
            case MovementDirection.Right:
                movementVector = RightVector * moveSpeed;
                movementDirection = RightVector;
                break;
            case MovementDirection.Left:
                movementVector = LeftVector * moveSpeed;
                movementDirection = LeftVector;
                break;
        }
    }

    private void ApplyVisualSettings()
    {
        if (autoFlip && spriteRenderer != null)
        {
            // Flip sprite based on direction
            spriteRenderer.flipX = (direction == MovementDirection.Left);
        }
    }

    #endregion

    #region Movement System

    private void MoveObject()
    {
        // Move using cached transform and pre-calculated vector
        cachedTransform.Translate(movementVector * Time.deltaTime);
    }

    #endregion

    #region Obstacle Detection

    private bool CheckForObstacle()
    {
        // Calculate detection box center position
        detectionCenter = (Vector2)cachedTransform.position + movementDirection * (detectionDistance * 0.5f);

        // Use OverlapBox com ContactFilter2D para evitar garbage collection
        int hitCount = Physics2D.OverlapBox(
            detectionCenter,
            detectionBoxSize,
            0f,
            contactFilter,
            overlapResults
        );

        return hitCount > 0;
    }

    private void HandleObstacleDetection()
    {
        // Ativar shake nos objetos detectados antes de reverter direção
        TriggerShakeOnDetectedObjects();

        // Reverter direção quando encontrar obstáculo WindShaker
        ReverseDirection();
    }

    #endregion

    #region Wind Shake System

    /// <summary>
    /// Ativa o trigger de shake em todos os objetos WindShakers detectados
    /// </summary>
    private void TriggerShakeOnDetectedObjects()
    {
        // Recalcular detecção para obter objetos atuais
        detectionCenter = (Vector2)cachedTransform.position + movementDirection * (detectionDistance * 0.5f);

        int hitCount = Physics2D.OverlapBox(
            detectionCenter,
            detectionBoxSize,
            0f,
            contactFilter,
            overlapResults
        );

        // Ativar shake em cada objeto detectado
        for (int i = 0; i < hitCount; i++)
        {
            if (overlapResults[i] != null)
            {
                TriggerShakeOnObject(overlapResults[i]);
            }
        }
    }

    /// <summary>
    /// Ativa o trigger de shake em um objeto específico
    /// </summary>
    /// <param name="detectedCollider">Collider do objeto detectado</param>
    private void TriggerShakeOnObject(Collider2D detectedCollider)
    {
        // Buscar animator no próprio objeto
        Animator animator = detectedCollider.GetComponent<Animator>();

        // Se não encontrou, buscar nos objetos pai
        if (animator == null)
        {
            animator = detectedCollider.GetComponentInParent<Animator>();
        }

        // Se não encontrou, buscar nos objetos filhos
        if (animator == null)
        {
            animator = detectedCollider.GetComponentInChildren<Animator>();
        }

        // Ativar o trigger se animator foi encontrado
        if (animator != null && !string.IsNullOrEmpty(shakeTriggerName))
        {
            try
            {
                animator.SetTrigger(shakeTriggerName);

                // Debug info
                if (showDebugInfo)
                {
                    Debug.Log($"[WindController] Shake trigger '{shakeTriggerName}' ativado em: {detectedCollider.name}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[WindController] Erro ao ativar trigger '{shakeTriggerName}' em {detectedCollider.name}: {e.Message}");
            }
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning($"[WindController] Animator não encontrado em: {detectedCollider.name}");
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Define a velocidade de movimento
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = Mathf.Max(0f, newSpeed);
        SetupMovement(); // Recalculate movement vector
    }

    /// <summary>
    /// Define a direção de movimento
    /// </summary>
    public void SetDirection(MovementDirection newDirection)
    {
        direction = newDirection;
        SetupMovement(); // Recalculate movement vector
        ApplyVisualSettings(); // Update sprite flip
    }

    /// <summary>
    /// Para o movimento
    /// </summary>
    public void Stop()
    {
        isMoving = false;
    }

    /// <summary>
    /// Retoma o movimento
    /// </summary>
    public void Resume()
    {
        isMoving = true;
    }

    /// <summary>
    /// Inverte a direção atual
    /// </summary>
    public void ReverseDirection()
    {
        MovementDirection newDirection = (direction == MovementDirection.Right)
            ? MovementDirection.Left
            : MovementDirection.Right;
        SetDirection(newDirection);
    }

    /// <summary>
    /// Define o nome do trigger de shake
    /// </summary>
    public void SetShakeTriggerName(string triggerName)
    {
        shakeTriggerName = triggerName;
    }

    #endregion

    #region Properties

    public float Speed => moveSpeed;
    public MovementDirection Direction => direction;
    public bool IsMoving => isMoving;
    public string ShakeTriggerName => shakeTriggerName;

    #endregion

    #region Debug Visualization

    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;

        // Draw movement direction arrow
        Vector3 position = transform.position;
        Vector3 arrowDirection = (direction == MovementDirection.Right) ? Vector3.right : Vector3.left;

        // Arrow color based on direction
        Gizmos.color = (direction == MovementDirection.Right) ? Color.green : Color.red;

        // Draw arrow
        Vector3 arrowEnd = position + arrowDirection * 2f;
        Gizmos.DrawLine(position, arrowEnd);
        Gizmos.DrawWireSphere(arrowEnd, 0.2f);

        // Draw detection box
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Vector3 boxCenter = position + arrowDirection * (detectionDistance * 0.5f);
            Gizmos.DrawWireCube(boxCenter, detectionBoxSize);

            // Show obstacle detection
            if (CheckForObstacle())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(boxCenter, detectionBoxSize);

                // Draw detected objects
                detectionCenter = (Vector2)position + movementDirection * (detectionDistance * 0.5f);
                int hitCount = Physics2D.OverlapBox(detectionCenter, detectionBoxSize, 0f, contactFilter, overlapResults);

                Gizmos.color = Color.cyan;
                for (int i = 0; i < hitCount; i++)
                {
                    if (overlapResults[i] != null)
                    {
                        Gizmos.DrawWireSphere(overlapResults[i].transform.position, 0.3f);
                    }
                }
            }
        }

        // Draw speed info
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        string info = $"Speed: {moveSpeed:F1}\nDirection: {direction}\nMoving: {isMoving}";
        if (Application.isPlaying)
        {
            info += $"\nDetecting: {CheckForObstacle()}";
        }
        info += $"\nShake Trigger: {shakeTriggerName}";
        UnityEditor.Handles.Label(position + Vector3.up * 1.5f, info);
#endif
    }

    #endregion
}
