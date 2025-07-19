using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SquareTransition : MonoBehaviour
{
    /// <summary>
    /// Define a direção de onde os quadrados começarão a aparecer
    /// </summary>
    public enum TransitionDirection
    {
        LeftToRight,
        RightToLeft
    }

    [Header("Configurações Básicas")]
    [Tooltip("Prefab do quadrado que será instanciado durante a transição")]
    [SerializeField] private GameObject squarePrefab;

    [Tooltip("Escala do quadrado nos eixos X e Y")]
    [SerializeField] private Vector2 squareScale = new Vector2(1f, 1f);

    [Tooltip("Se ativado, inicia a animação automaticamente no Start")]
    [SerializeField] private bool playOnStart = false;

    [Tooltip("Se true, começa com a tela preenchida e vai esvaziando; se false, começa vazia e vai preenchendo")]
    [SerializeField] private bool startFilled = false;

    [Tooltip("Direção de onde os quadrados começam a aparecer")]
    [SerializeField] private TransitionDirection direction = TransitionDirection.LeftToRight;

    [Header("Configurações de Timing")]
    [Tooltip("Delay em segundos antes de iniciar o efeito de transição")]
    [Range(0f, 5f)]
    [SerializeField] private float startDelay = 0f;

    [Header("Configurações de Performance")]
    [Tooltip("Tamanho do pool de objetos para reutilização")]
    [Range(50, 500)]
    [SerializeField] private int poolSize = 200;

    [Tooltip("Processar quadrados em lotes para reduzir alocações")]
    [Range(5, 50)]
    [SerializeField] private int batchSize = 20;

    [Header("Configurações de Aparência")]
    [Tooltip("Fator de sobreposição entre quadrados (0.95 = 5% de sobreposição)")]
    [Range(-1f, 1.0f)]
    [SerializeField] private float overlapFactor = 0.98f;

    [Tooltip("Margem extra de quadrados nas bordas da tela")]
    [Range(-6, 6)]
    [SerializeField] private int edgeMargin = 2;

    // Cache de valores calculados
    private Vector2Int gridSize;
    private float squareWidth, squareHeight;
    private float effectiveWidth, effectiveHeight;
    private float startX, startY;

    // Pool de objetos para reutilização
    private Queue<GameObject> squarePool = new Queue<GameObject>();
    private List<GameObject> activeSquares = new List<GameObject>();

    // Cache de componentes
    private Camera mainCamera;
    private SpriteRenderer prefabSpriteRenderer;

    // Flags de controle
    private bool isTransitioning = false;
    private bool isInitialized = false;

    // Eventos
    public System.Action onTransitionComplete;

    // WaitForSeconds cache para evitar alocações
    private readonly WaitForSeconds smallDelay = new WaitForSeconds(0.01f);
    private readonly WaitForSeconds mediumDelay = new WaitForSeconds(0.05f);

    void Awake()
    {
        InitializeCache();
        InitializePool();
    }

    void Start()
    {
        if (!isInitialized)
        {
            InitializeCache();
            InitializePool();
        }

        // Calcula o tamanho do grid necessário
        CalculateGridParameters();

        // Verifica se deve iniciar automaticamente
        if (playOnStart)
        {
            StartTransition();
        }
    }

    /// <summary>
    /// Inicializa cache de componentes e valores
    /// </summary>
    private void InitializeCache()
    {
        mainCamera = Camera.main;
        if (squarePrefab != null)
        {
            prefabSpriteRenderer = squarePrefab.GetComponent<SpriteRenderer>();
        }
        isInitialized = true;
    }

    /// <summary>
    /// Inicializa o pool de objetos para reutilização
    /// </summary>
    private void InitializePool()
    {
        squarePool.Clear();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject square = Instantiate(squarePrefab, transform);
            square.SetActive(false);
            squarePool.Enqueue(square);
        }
    }

    /// <summary>
    /// Calcula e cached todos os parâmetros necessários para o grid
    /// </summary>
    private void CalculateGridParameters()
    {
        if (mainCamera == null)
        {
            Debug.LogError("SquareTransition: Não foi possível encontrar a câmera principal");
            gridSize = new Vector2Int(10, 10);
            return;
        }

        // Obtém as dimensões da visão da câmera em unidades de mundo
        float cameraHeight = 2f * mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calcula o tamanho real do quadrado do prefab
        if (prefabSpriteRenderer != null && prefabSpriteRenderer.sprite != null)
        {
            squareWidth = prefabSpriteRenderer.sprite.bounds.size.x;
            squareHeight = prefabSpriteRenderer.sprite.bounds.size.y;
        }
        else
        {
            squareWidth = 1f;
            squareHeight = 1f;
        }

        // Aplica a escala do usuário ao tamanho do quadrado
        squareWidth *= squareScale.x;
        squareHeight *= squareScale.y;

        // Usa o fator de sobreposição configurável para eliminar espaçamentos
        effectiveWidth = squareWidth * overlapFactor;
        effectiveHeight = squareHeight * overlapFactor;

        // Calcula quantos quadrados são necessários para cobrir completamente a tela
        int horizontalCount = Mathf.CeilToInt(cameraWidth / effectiveWidth) + (edgeMargin * 2);
        int verticalCount = Mathf.CeilToInt(cameraHeight / effectiveHeight) + (edgeMargin * 2);

        gridSize = new Vector2Int(horizontalCount, verticalCount);

        // Calcula a posição inicial para centralizar o grid na tela
        float totalGridWidth = (gridSize.x - 1) * effectiveWidth;
        float totalGridHeight = (gridSize.y - 1) * effectiveHeight;

        Vector3 cameraCenter = mainCamera.transform.position;

        startX = cameraCenter.x - (totalGridWidth * 0.5f);
        startY = cameraCenter.y - (totalGridHeight * 0.5f);

        Debug.Log($"SquareTransition: Grid calculado: {horizontalCount}x{verticalCount} quadrados");
        Debug.Log($"Tamanho do quadrado: {squareWidth:F2}x{squareHeight:F2}, Espaçamento efetivo: {effectiveWidth:F2}x{effectiveHeight:F2}");
        Debug.Log($"Fator de sobreposição: {overlapFactor:F2} ({(1f - overlapFactor) * 100f:F1}% de sobreposição)");
    }

    /// <summary>
    /// Obtém um quadrado do pool ou cria um novo se necessário
    /// </summary>
    private GameObject GetSquareFromPool()
    {
        if (squarePool.Count > 0)
        {
            GameObject square = squarePool.Dequeue();
            square.SetActive(true);
            return square;
        }

        // Se o pool estiver vazio, cria um novo
        return Instantiate(squarePrefab, transform);
    }

    /// <summary>
    /// Retorna um quadrado ao pool
    /// </summary>
    private void ReturnSquareToPool(GameObject square)
    {
        if (square != null)
        {
            square.SetActive(false);
            square.transform.localScale = Vector3.one; // Reset para o próximo uso
            squarePool.Enqueue(square);
        }
    }

    public void StartTransition()
    {
        if (squarePrefab == null)
        {
            Debug.LogWarning("SquareTransition: squarePrefab não foi atribuído");
            return;
        }

        if (isTransitioning)
        {
            Debug.LogWarning("SquareTransition: Uma transição já está em andamento");
            return;
        }

        // Inicia a corrotina com delay
        StartTransitionWithDelay();
    }

    /// <summary>
    /// Inicia a transição após o delay configurado
    /// </summary>
    private void StartTransitionWithDelay()
    {
        // Limpa quadrados anteriores
        ClearSquares();

        // Recalcula parâmetros se necessário
        CalculateGridParameters();

        // Inicia a coroutine apropriada
        if (startFilled)
        {
            StartCoroutine(EmptyScreenCoroutine());
        }
        else
        {
            StartCoroutine(FillScreenCoroutine());
        }
    }

    /// <summary>
    /// Limpa todos os quadrados ativos e retorna ao pool
    /// </summary>
    private void ClearSquares()
    {
        foreach (GameObject square in activeSquares)
        {
            ReturnSquareToPool(square);
        }
        activeSquares.Clear();
    }

    /// <summary>
    /// Coroutine otimizada que anima o preenchimento da tela
    /// </summary>
    private IEnumerator FillScreenCoroutine()
    {
        isTransitioning = true;

        int startColumn = (direction == TransitionDirection.LeftToRight) ? 0 : gridSize.x - 1;
        int endColumn = (direction == TransitionDirection.LeftToRight) ? gridSize.x : 0;
        int increment = (direction == TransitionDirection.LeftToRight) ? 1 : -1;

        // Pre-aloca lista de índices para evitar alocações repetidas
        List<int> rowIndices = new List<int>(gridSize.y);
        List<GameObject> batchSquares = new List<GameObject>(batchSize);

        int column = startColumn;
        while ((direction == TransitionDirection.LeftToRight && column < endColumn) ||
               (direction == TransitionDirection.RightToLeft && column > endColumn))
        {
            // Limpa e preenche lista de índices
            rowIndices.Clear();
            for (int r = 0; r < gridSize.y; r++)
            {
                rowIndices.Add(r);
            }

            // Embaralha usando Fisher-Yates (mais eficiente)
            for (int i = rowIndices.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                int temp = rowIndices[i];
                rowIndices[i] = rowIndices[randomIndex];
                rowIndices[randomIndex] = temp;
            }

            // Processa em lotes para reduzir yields
            for (int batchStart = 0; batchStart < gridSize.y; batchStart += batchSize)
            {
                batchSquares.Clear();
                int batchEnd = Mathf.Min(batchStart + batchSize, gridSize.y);

                // Cria lote de quadrados
                for (int i = batchStart; i < batchEnd; i++)
                {
                    int row = rowIndices[i];

                    // Calcula posição usando valores cached
                    float x = startX + (column * effectiveWidth);
                    float y = startY + (row * effectiveHeight);

                    GameObject square = GetSquareFromPool();
                    square.transform.position = new Vector3(x, y, 0);
                    square.transform.localScale = new Vector3(squareScale.x, squareScale.y, 1f);

                    activeSquares.Add(square);
                    batchSquares.Add(square);
                }

                // Anima o lote
                foreach (GameObject square in batchSquares)
                {
                    StartCoroutine(AnimateSquareScaleOptimized(square));
                }

                yield return smallDelay;
            }

            column += increment;
            yield return null; // Um frame entre colunas
        }

        isTransitioning = false;
        onTransitionComplete?.Invoke();
    }

    /// <summary>
    /// Coroutine otimizada para esvaziar a tela
    /// </summary>
    private IEnumerator EmptyScreenCoroutine()
    {
        isTransitioning = true;

        // Preenche toda a tela primeiro
        FillEntireScreenOptimized();

        // Aguarda o delay configurado antes de iniciar
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        int startColumn = (direction == TransitionDirection.LeftToRight) ? 0 : gridSize.x - 1;
        int endColumn = (direction == TransitionDirection.LeftToRight) ? gridSize.x : 0;
        int increment = (direction == TransitionDirection.LeftToRight) ? 1 : -1;

        // Cria um array 2D para acesso rápido aos quadrados
        GameObject[,] squareGrid = new GameObject[gridSize.x, gridSize.y];

        // Preenche o array baseado na posição dos quadrados
        foreach (GameObject square in activeSquares)
        {
            Vector2Int gridPos = GetGridPositionFromWorldPositionOptimized(square.transform.position);
            if (gridPos.x >= 0 && gridPos.x < gridSize.x && gridPos.y >= 0 && gridPos.y < gridSize.y)
            {
                squareGrid[gridPos.x, gridPos.y] = square;
            }
        }

        List<int> rowIndices = new List<int>(gridSize.y);

        int column = startColumn;
        while ((direction == TransitionDirection.LeftToRight && column < endColumn) ||
               (direction == TransitionDirection.RightToLeft && column > endColumn))
        {
            // Prepara índices de linha
            rowIndices.Clear();
            for (int r = 0; r < gridSize.y; r++)
            {
                rowIndices.Add(r);
            }

            // Embaralha
            for (int i = rowIndices.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                int temp = rowIndices[i];
                rowIndices[i] = rowIndices[randomIndex];
                rowIndices[randomIndex] = temp;
            }

            // Remove quadrados em lotes
            for (int batchStart = 0; batchStart < gridSize.y; batchStart += batchSize)
            {
                int batchEnd = Mathf.Min(batchStart + batchSize, gridSize.y);

                for (int i = batchStart; i < batchEnd; i++)
                {
                    int row = rowIndices[i];
                    GameObject square = squareGrid[column, row];

                    if (square != null)
                    {
                        StartCoroutine(AnimateSquareFadeOptimized(square));
                        squareGrid[column, row] = null;
                    }
                }

                yield return smallDelay;
            }

            column += increment;
            yield return null;
        }

        // Limpa quadrados restantes
        ClearSquares();

        isTransitioning = false;
        onTransitionComplete?.Invoke();
    }

    /// <summary>
    /// Versão otimizada do preenchimento completo da tela
    /// </summary>
    private void FillEntireScreenOptimized()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                float posX = startX + (x * effectiveWidth);
                float posY = startY + (y * effectiveHeight);

                GameObject square = GetSquareFromPool();
                square.transform.position = new Vector3(posX, posY, 0);
                square.transform.localScale = new Vector3(squareScale.x, squareScale.y, 1f);

                activeSquares.Add(square);
            }
        }
    }

    /// <summary>
    /// Versão otimizada da conversão de posição mundial para grid
    /// </summary>
    private Vector2Int GetGridPositionFromWorldPositionOptimized(Vector3 worldPos)
    {
        int gridX = Mathf.RoundToInt((worldPos.x - startX) / effectiveWidth);
        int gridY = Mathf.RoundToInt((worldPos.y - startY) / effectiveHeight);

        return new Vector2Int(gridX, gridY);
    }

    /// <summary>
    /// Animação otimizada de escala usando valores cached
    /// </summary>
    private IEnumerator AnimateSquareScaleOptimized(GameObject square)
    {
        const float duration = 0.2f;
        float elapsed = 0;
        Vector3 targetScale = new Vector3(squareScale.x, squareScale.y, 1f);

        square.transform.localScale = Vector3.zero;

        while (elapsed < duration && square.activeInHierarchy)
        {
            float t = elapsed / duration;
            square.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (square.activeInHierarchy)
        {
            square.transform.localScale = targetScale;
        }
    }

    /// <summary>
    /// Animação otimizada de fade
    /// </summary>
    private IEnumerator AnimateSquareFadeOptimized(GameObject square)
    {
        const float duration = 0.2f;
        float elapsed = 0;

        SpriteRenderer renderer = square.GetComponent<SpriteRenderer>();
        Color originalColor = renderer != null ? renderer.color : Color.white;
        Vector3 originalScale = square.transform.localScale;

        while (elapsed < duration && square.activeInHierarchy)
        {
            float t = elapsed / duration;
            square.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

            if (renderer != null)
            {
                renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f - t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Remove da lista ativa e retorna ao pool
        activeSquares.Remove(square);
        ReturnSquareToPool(square);

        // Restaura cor original para o próximo uso
        if (renderer != null)
        {
            renderer.color = originalColor;
        }
    }

    private void OnDestroy()
    {
        // Limpa pool ao destruir o objeto
        while (squarePool.Count > 0)
        {
            GameObject square = squarePool.Dequeue();
            if (square != null)
            {
                Destroy(square);
            }
        }
    }
}
