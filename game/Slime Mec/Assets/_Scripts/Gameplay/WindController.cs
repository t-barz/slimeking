using UnityEngine;

public class WindController : MonoBehaviour
{
    #region Inspector Configuration

    [Header("⚙️ Configurações de Movimento")]
    [Tooltip("Velocidade de movimento horizontal (unidades por segundo)")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("Direção do movimento horizontal")]
    [SerializeField] private MovementDirection direction = MovementDirection.Right;

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
    private bool isMoving = true;

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
            MoveObject();
        }
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        // Cache components for performance
        cachedTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SetupMovement()
    {
        // Calculate movement vector based on direction
        switch (direction)
        {
            case MovementDirection.Right:
                movementVector = RightVector * moveSpeed;
                break;
            case MovementDirection.Left:
                movementVector = LeftVector * moveSpeed;
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

    #endregion

    #region Properties

    public float Speed => moveSpeed;
    public MovementDirection Direction => direction;
    public bool IsMoving => isMoving;

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

        // Draw speed info
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        string info = $"Speed: {moveSpeed:F1}\nDirection: {direction}\nMoving: {isMoving}";
        UnityEditor.Handles.Label(position + Vector3.up * 1.5f, info);
#endif
    }

    #endregion
}
