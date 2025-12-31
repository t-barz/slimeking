using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Gameplay
{
    public class WindController : MonoBehaviour
{
    #region Inspector Configuration

    [Header("⚙️ Configurações de Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private MovementDirection direction = MovementDirection.Right;

    [Header("🌳 Detecção de Obstáculos")]
    [SerializeField] private float detectionDistance = 2f;
    [SerializeField] private Vector2 detectionBoxSize = new Vector2(0.5f, 1f);
    [SerializeField] private LayerMask windShakersLayerMask = 1 << 6;

    [Header("🌊 Animação de Shake")]
    [SerializeField] private string shakeTriggerName = "Shake";

    [Header("🎨 Configurações Visuais")]
    [SerializeField] private bool autoFlip = true;

    [Header("🔧 Debug")]
    [SerializeField] private bool showDebugInfo = false;

    [Header("🗑️ Auto Destruição")]
    [SerializeField] private float lifeTime = 10f;
    [SerializeField] private bool destroyWhenOffScreen = true;

    #endregion

    #region Private Variables

    public enum MovementDirection { Left, Right }

    // Cached components
    private Transform cachedTransform;
    private SpriteRenderer spriteRenderer;

    // Movement variables
    private Vector2 movementVector;
    private Vector2 movementDirection;
    private bool isMoving = true;

    // Detection variables - OTIMIZADO: Reduzido tamanho do array
    private Collider2D[] overlapResults = new Collider2D[3];
    private Vector2 detectionCenter;
    private ContactFilter2D contactFilter;

    // Shake tracking - OTIMIZADO: Evita triggers repetidos
    private HashSet<int> shakenObjects = new HashSet<int>();
    private float lastShakeCheck = 0f;
    private const float SHAKE_CHECK_INTERVAL = 0.1f; // Reduz frequência de checks

    // Static vectors for performance
    private static readonly Vector2 RightVector = Vector2.right;
    private static readonly Vector2 LeftVector = Vector2.left;

    // Auto destroy
    private float destroyTimer;

    #endregion

    #region Unity Lifecycle

    void Start()
    {
        InitializeComponents();
        SetupMovement();
        ApplyVisualSettings();

        // Inicializar timer de destruição
        destroyTimer = lifeTime;
    }

    void Update()
    {
        if (isMoving && moveSpeed > 0f)
        {
            MoveObject();

            // OTIMIZADO: Reduz frequência de detecção de shake
            if (Time.time >= lastShakeCheck + SHAKE_CHECK_INTERVAL)
            {
                CheckAndTriggerShake();
                lastShakeCheck = Time.time;
            }
        }

        // OTIMIZADO: Auto destruição para evitar acúmulo de objetos
        HandleAutoDestroy();
    }

    #endregion

    #region Initialization

    private void InitializeComponents()
    {
        cachedTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // OTIMIZADO: Configuração mais eficiente do ContactFilter
        contactFilter = new ContactFilter2D
        {
            layerMask = windShakersLayerMask,
            useTriggers = true,
            useLayerMask = true
        };
    }

    private void SetupMovement()
    {
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
            spriteRenderer.flipX = (direction == MovementDirection.Left);
        }
    }

    #endregion

    #region Movement System

    private void MoveObject()
    {
        cachedTransform.Translate(movementVector * Time.deltaTime);
    }

    #endregion

    #region Wind Shake System - OTIMIZADO

    /// <summary>
    /// OTIMIZADO: Verifica e ativa shake com controle de frequência
    /// </summary>
    private void CheckAndTriggerShake()
    {
        detectionCenter = (Vector2)cachedTransform.position + movementDirection * (detectionDistance * 0.5f);

        int hitCount = Physics2D.OverlapBox(
            detectionCenter,
            detectionBoxSize,
            0f,
            contactFilter,
            overlapResults
        );

        if (hitCount > 0)
        {
            TriggerShakeOnDetectedObjects(hitCount);
        }
    }

    /// <summary>
    /// OTIMIZADO: Evita triggers repetidos no mesmo objeto
    /// </summary>
    private void TriggerShakeOnDetectedObjects(int hitCount)
    {
        for (int i = 0; i < hitCount; i++)
        {
            if (overlapResults[i] != null)
            {
                int objectId = overlapResults[i].GetInstanceID();

                // OTIMIZADO: Só ativa shake se ainda não foi ativado neste objeto
                if (!shakenObjects.Contains(objectId))
                {
                    TriggerShakeOnObject(overlapResults[i]);
                    shakenObjects.Add(objectId);
                }
            }
        }
    }

    /// <summary>
    /// OTIMIZADO: Busca de animator mais eficiente
    /// </summary>
    private void TriggerShakeOnObject(Collider2D detectedCollider)
    {
        // OTIMIZADO: Busca hierárquica mais eficiente
        Animator animator = detectedCollider.GetComponent<Animator>() ??
                           detectedCollider.GetComponentInParent<Animator>();

        if (animator != null && !string.IsNullOrEmpty(shakeTriggerName))
        {
            animator.SetTrigger(shakeTriggerName);

            if (showDebugInfo)
            {}
        }
    }

    #endregion

    #region Auto Destroy System - NOVO

    /// <summary>
    /// NOVO: Sistema de auto destruição para evitar memory leaks
    /// </summary>
    private void HandleAutoDestroy()
    {
        destroyTimer -= Time.deltaTime;

        if (destroyTimer <= 0f)
        {
            DestroyWind();
            return;
        }

        // OTIMIZADO: Destruir quando sair da tela (aproximação simples)
        if (destroyWhenOffScreen && IsOffScreen())
        {
            DestroyWind();
        }
    }

    /// <summary>
    /// OTIMIZADO: Verificação simples se está fora da tela
    /// </summary>
    private bool IsOffScreen()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(cachedTransform.position);
        return screenPos.x < -0.1f || screenPos.x > 1.1f || screenPos.y < -0.1f || screenPos.y > 1.1f;
    }

    /// <summary>
    /// Destrói o objeto de vento de forma limpa
    /// </summary>
    private void DestroyWind()
    {
        // OTIMIZADO: Cleanup antes de destruir
        shakenObjects.Clear();

        if (showDebugInfo)
        {}

        Destroy(gameObject);
    }

    #endregion

    #region Public Methods - OTIMIZADO

    public void SetSpeed(float newSpeed)
    {
        if (Mathf.Approximately(moveSpeed, newSpeed)) return; // Evita recálculos desnecessários

        moveSpeed = Mathf.Max(0f, newSpeed);
        SetupMovement();
    }

    public void SetDirection(MovementDirection newDirection)
    {
        if (direction == newDirection) return; // Evita recálculos desnecessários

        direction = newDirection;
        SetupMovement();
        ApplyVisualSettings();
    }

    public void Stop() => isMoving = false;
    public void Resume() => isMoving = true;

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
    public float RemainingLifeTime => destroyTimer;

    #endregion

    #region Debug Visualization - OTIMIZADO

    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;

        Vector3 position = transform.position;
        Vector3 arrowDirection = (direction == MovementDirection.Right) ? Vector3.right : Vector3.left;

        // Arrow
        Gizmos.color = (direction == MovementDirection.Right) ? Color.green : Color.red;
        Vector3 arrowEnd = position + arrowDirection * 2f;
        Gizmos.DrawLine(position, arrowEnd);

        // Detection box
        if (Application.isPlaying)
        {
            Vector3 boxCenter = position + arrowDirection * (detectionDistance * 0.5f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(boxCenter, detectionBoxSize);
        }

#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.white;
        string info = $"Speed: {moveSpeed:F1}\nLife: {destroyTimer:F1}s\nShaken: {shakenObjects.Count}";
        UnityEditor.Handles.Label(position + Vector3.up * 1.5f, info);
#endif
    }

    #endregion
}
}
