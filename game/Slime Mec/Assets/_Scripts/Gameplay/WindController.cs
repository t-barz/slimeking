using UnityEngine;

/// <summary>
/// Controlador de vento otimizado para jogos 2D com URP.
/// Move objetos horizontalmente com máxima performance.
/// 
/// OTIMIZAÇÕES APLICADAS:
/// - Vector2 para jogos 2D (mais eficiente que Vector3)
/// - Cache de componentes (Transform, SpriteRenderer)
/// - Vetores estáticos pré-alocados (zero garbage collection)
/// - Early exit patterns
/// - Flags de estado para evitar cálculos desnecessários
/// 
/// NOTA: Para física mais realista, considere usar Rigidbody2D.velocity
/// em vez de Transform.Translate para melhor integração com o sistema de física 2D.
/// </summary>
public class WindController : MonoBehaviour
{
    [Header("🌪️ Configurações de Movimento")]
    [Tooltip("Velocidade de deslocamento horizontal do vento (unidades por segundo)")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("🧭 Configurações de Direção")]
    [Tooltip("Direção do movimento do vento")]
    [SerializeField] private WindDirection direction = WindDirection.LeftToRight;

    [Header("🎨 Configurações Visuais")]
    [Tooltip("Se true, aplica flip horizontal automaticamente baseado na direção")]
    [SerializeField] private bool autoFlip = true;

    [Tooltip("Se true, mostra gizmos de debug no Scene View")]
    [SerializeField] private bool showDebugGizmos = true;

    /// <summary>
    /// Direções possíveis para o movimento do vento
    /// </summary>
    public enum WindDirection
    {
        LeftToRight,    // Esquerda para direita (padrão)
        RightToLeft     // Direita para esquerda
    }

    // Componentes necessários (cache para performance)
    private SpriteRenderer spriteRenderer;
    private Transform cachedTransform;

    // Otimização 2D específica - Vector2 é mais eficiente que Vector3 para jogos 2D
    private Vector2 movementVector2D;
    private bool isMoving = true;

    // Cache de deltaTime e valores estáticos para melhor performance
    private float deltaTime;

    // Vetores estáticos pré-alocados para evitar garbage collection
    private static readonly Vector2 ZeroVector2D = Vector2.zero;
    private static readonly Vector2 RightVector2D = Vector2.right;
    private static readonly Vector2 LeftVector2D = Vector2.left;

    /// <summary>
    /// Inicialização dos componentes e configuração inicial
    /// OTIMIZADO: Cache de componentes essenciais
    /// </summary>
    void Start()
    {
        // Cache do Transform para evitar chamadas GetComponent repetidas
        cachedTransform = transform;

        // Cache do SpriteRenderer para flip otimizado (removido duplicata)
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Calcula o vetor de movimento baseado na direção
        UpdateMovementVector();

        // Aplica flip inicial se necessário
        ApplyVisualFlip();
    }

    /// <summary>
    /// Atualiza o movimento do vento a cada frame
    /// OTIMIZADO: Usa cache de deltaTime e early exit quando parado
    /// </summary>
    void Update()
    {
        // Early exit se não estiver se movendo
        if (!isMoving) return;

        // Cache deltaTime para evitar múltiplas chamadas à propriedade
        deltaTime = Time.deltaTime;

        // Move o objeto na direção configurada
        MoveWind();
    }

    /// <summary>
    /// Move o objeto do vento baseado na velocidade e direção configuradas
    /// OTIMIZADO: Usa Transform cacheado e Vector2 para jogos 2D
    /// </summary>
    private void MoveWind()
    {
        // Usa Transform cacheado e deltaTime cacheado com Vector2 otimizado para 2D
        cachedTransform.Translate(movementVector2D * deltaTime);
    }

    /// <summary>
    /// Atualiza o vetor de movimento baseado na direção atual
    /// OTIMIZADO: Usa Vector2 para jogos 2D e vetores estáticos pré-alocados
    /// </summary>
    private void UpdateMovementVector()
    {
        // Usa vetores estáticos pré-alocados para eliminar garbage collection
        switch (direction)
        {
            case WindDirection.LeftToRight:
                movementVector2D = RightVector2D * moveSpeed;
                break;
            case WindDirection.RightToLeft:
                movementVector2D = LeftVector2D * moveSpeed;
                break;
        }

        // Atualiza flag de movimento para otimização no Update
        isMoving = (moveSpeed > 0f);
    }

    /// <summary>
    /// Aplica flip horizontal na imagem baseado na direção do vento
    /// </summary>
    private void ApplyVisualFlip()
    {
        if (!autoFlip || spriteRenderer == null) return;

        // Flip quando a direção é da direita para esquerda
        bool shouldFlip = (direction == WindDirection.RightToLeft);
        spriteRenderer.flipX = shouldFlip;
    }

    /// <summary>
    /// Altera a direção do vento em tempo de execução
    /// </summary>
    /// <param name="newDirection">Nova direção do vento</param>
    public void SetDirection(WindDirection newDirection)
    {
        direction = newDirection;
        UpdateMovementVector();
        ApplyVisualFlip();
    }

    /// <summary>
    /// Altera a velocidade do vento em tempo de execução
    /// </summary>
    /// <param name="newSpeed">Nova velocidade do vento</param>
    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        UpdateMovementVector();
    }

    /// <summary>
    /// Inverte a direção atual do vento
    /// </summary>
    public void ReverseDirection()
    {
        WindDirection newDirection = (direction == WindDirection.LeftToRight)
            ? WindDirection.RightToLeft
            : WindDirection.LeftToRight;

        SetDirection(newDirection);
    }

    /// <summary>
    /// Para temporariamente o movimento do vento
    /// OTIMIZADO: Usa Vector2 estático pré-alocado
    /// </summary>
    public void Stop()
    {
        movementVector2D = ZeroVector2D;
        isMoving = false;
    }

    /// <summary>
    /// Retoma o movimento do vento com as configurações atuais
    /// OTIMIZADO: Atualiza flag de movimento junto
    /// </summary>
    public void Resume()
    {
        UpdateMovementVector();
        isMoving = (moveSpeed > 0f);
    }

    /// <summary>
    /// Desenha gizmos de debug no Scene View
    /// OTIMIZADO: Reduz cálculos e allocations desnecessárias
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // Cache da posição para evitar múltiplas chamadas
        Vector3 startPos = transform.position;

        // Calcula direção uma única vez
        bool isLeftToRight = (direction == WindDirection.LeftToRight);
        Vector3 directionVector = isLeftToRight ? Vector3.right : Vector3.left;
        Vector3 endPos = startPos + directionVector * 2f;

        // Cor da seta baseada na direção (cache de cores)
        Gizmos.color = isLeftToRight ? Color.green : Color.red;

        // Desenha linha principal
        Gizmos.DrawLine(startPos, endPos);

        // Desenha ponta da seta (cálculos otimizados)
        const float arrowSize = 0.3f;
        const float arrowLength = 0.5f;

        Vector3 arrowBase = endPos - directionVector * arrowLength;
        Gizmos.DrawLine(endPos, arrowBase + Vector3.up * arrowSize);
        Gizmos.DrawLine(endPos, arrowBase + Vector3.down * arrowSize);

#if UNITY_EDITOR
        // Label otimizado (só calcula se necessário)
        if (UnityEditor.Selection.activeGameObject == gameObject)
        {
            Vector3 labelPos = startPos + Vector3.up * 1.5f;
            string label = $"Wind: {direction}\nSpeed: {moveSpeed:F1}";
            UnityEditor.Handles.Label(labelPos, label);
        }
#endif
    }

    /// <summary>
    /// Propriedades públicas para acesso externo
    /// OTIMIZADO: Usa comparação de Vector2 e flag cached
    /// </summary>
    public WindDirection Direction => direction;
    public float Speed => moveSpeed;
    public bool IsMoving => isMoving && movementVector2D != ZeroVector2D;
}
