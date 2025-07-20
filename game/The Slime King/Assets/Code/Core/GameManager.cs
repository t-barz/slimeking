using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gerenciador principal do jogo - Singleton que persiste entre cenas
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Instância única do GameManager
    /// </summary>
    public static GameManager Instance { get; private set; }
    #endregion

    #region Configurações
    [Header("Configurações de Transição")]
    [Tooltip("Duração do efeito de fade-in ao escurecer a cena (em segundos)")]
    [Range(0.1f, 3.0f)]
    [SerializeField] private float fadeDuration = 0.25f;

    [Tooltip("Tempo de espera com a tela escura após o fade-in (em segundos)")]
    [Range(0.1f, 2.0f)]
    [SerializeField] private float darkScreenDuration = 0.5f;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Inicialização do Singleton
    /// </summary>
    private void Awake()
    {
        // Implementa padrão Singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("[GameManager] Instância duplicada detectada - destruindo...");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[GameManager] Singleton inicializado");
    }

    /// <summary>
    /// Inicialização após Awake
    /// </summary>
    void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// Atualização por frame
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Limpeza ao destruir
    /// </summary>
    private void OnDestroy()
    {
        // Remove referência se esta instância está sendo destruída
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Inicializa o jogo
    /// </summary>
    private void InitializeGame()
    {
        Debug.Log("[GameManager] Jogo inicializado");

        // Aqui você pode adicionar lógica de inicialização do jogo:
        // - Configurações iniciais
        // - Carregamento de dados salvos
        // - Inicialização de sistemas
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Pausa o jogo
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("[GameManager] Jogo pausado");
    }

    /// <summary>
    /// Retoma o jogo
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameManager] Jogo retomado");
    }

    /// <summary>
    /// Verifica se o jogo está pausado
    /// </summary>
    public bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }

    /// <summary>
    /// Muda para uma nova cena escurecendo a atual primeiro
    /// </summary>
    public void ChangeScene(string sceneName, Vector2 targetPosition = default, Color darkenColor = default)
    {
        // Define cor padrão como preto se não especificada
        if (darkenColor == default)
            darkenColor = Color.black;

        Debug.Log($"[GameManager] Mudando para a cena: {sceneName} na posição {targetPosition}");
        StartCoroutine(ChangeSceneCoroutine(sceneName, targetPosition, darkenColor));
    }

    /// <summary>
    /// Corrotina para mudança de cena com escurecimento
    /// </summary>
    private IEnumerator ChangeSceneCoroutine(string sceneName, Vector2 targetPosition, Color darkenColor)
    {
        // 1. Escurece a cena atual
        yield return StartCoroutine(DarkenScene(darkenColor));

        Debug.Log("[GameManager] Cena escurecida com sucesso");

        // 2. Exibe UI de carregamento
        // 3. Salva o jogo
        // 4. Carrega a nova cena em asynchronous mode
        yield return StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// Carrega a nova cena de forma assíncrona
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Debug.Log($"[GameManager] Iniciando carregamento assíncrono da cena: {sceneName}");

        // Inicia o carregamento assíncrono da nova cena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Aguarda o carregamento completar
        while (!asyncLoad.isDone)
        {
            // Opcionalmente, pode exibir progresso do carregamento
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"[GameManager] Progresso do carregamento: {progress * 100:F1}%");

            yield return null;
        }

        Debug.Log($"[GameManager] Cena {sceneName} carregada com sucesso!");

        // Aguarda alguns frames para garantir que a nova cena esteja sendo exibida
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        // Sempre executa fade-out na nova cena
        yield return StartCoroutine(FadeOutScene());
    }

    /// <summary>
    /// Executa fade-out para remover o overlay escuro da nova cena
    /// </summary>
    private IEnumerator FadeOutScene()
    {
        Debug.Log("[GameManager] Iniciando fade-out da nova cena");

        // Procura por overlay escuro existente
        GameObject existingOverlay = GameObject.Find("DarkenOverlay");

        if (existingOverlay != null)
        {
            SpriteRenderer overlayRenderer = existingOverlay.GetComponent<SpriteRenderer>();

            if (overlayRenderer != null)
            {
                Color currentColor = overlayRenderer.color;
                Color transparentColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);

                Debug.Log($"[GameManager] Fazendo fade-out do overlay (duração: {fadeDuration}s)");

                float elapsedTime = 0f;

                // Loop do fade-out
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);

                    // Interpola a cor de opaca para transparente
                    Color fadeColor = Color.Lerp(transparentColor, currentColor, alpha);
                    overlayRenderer.color = fadeColor;

                    yield return null;
                }

                // Garante que seja completamente transparente
                overlayRenderer.color = transparentColor;

                Debug.Log("[GameManager] Fade-out concluído - removendo overlay");

                // Remove o overlay da cena
                Destroy(existingOverlay);
            }
        }
        else
        {
            Debug.Log("[GameManager] Nenhum overlay encontrado para fade-out - nova cena já visível");
        }

        Debug.Log("[GameManager] Transição de cena completamente finalizada!");
    }

    /// <summary>
    /// Escurece a cena atual com a cor especificada
    /// </summary>
    private IEnumerator DarkenScene(Color color)
    {
        Debug.Log($"[GameManager] Escurecendo cena com a cor: {color}");

        // Cria um overlay que cobre toda a tela
        GameObject darkenOverlay = new GameObject("DarkenOverlay");
        SpriteRenderer overlayRenderer = darkenOverlay.AddComponent<SpriteRenderer>();

        // Cria uma textura 1x1 com a cor especificada
        Texture2D darkenTexture = new Texture2D(1, 1);
        darkenTexture.SetPixel(0, 0, color);
        darkenTexture.Apply();

        // Cria o sprite a partir da textura
        Sprite darkenSprite = Sprite.Create(darkenTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
        overlayRenderer.sprite = darkenSprite;

        // Busca a câmera ativa (funciona com CinemachineCamera)
        Camera activeCamera = Camera.main;
        if (activeCamera == null)
        {
            activeCamera = FindAnyObjectByType<Camera>();
        }

        if (activeCamera != null)
        {
            // Usa orthographicSize para calcular o tamanho correto
            float cameraHeight = activeCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * activeCamera.aspect;

            // Posiciona no centro da câmera
            darkenOverlay.transform.position = new Vector3(
                activeCamera.transform.position.x,
                activeCamera.transform.position.y,
                0f
            );

            // Escala muito maior para garantir cobertura total
            darkenOverlay.transform.localScale = new Vector3(cameraWidth * 200f, cameraHeight * 200f, 1f);

            Debug.Log($"[GameManager] Overlay dimensionado - CameraSize: {activeCamera.orthographicSize}, Scale: {cameraWidth * 200f}x{cameraHeight * 200f}");
        }
        else
        {
            Debug.LogWarning("[GameManager] Nenhuma câmera encontrada - usando escala muito grande");
            // Fallback com escala muito grande
            darkenOverlay.transform.localScale = new Vector3(50000f, 50000f, 1f);
        }

        // Configura a sorting layer e order
        overlayRenderer.sortingLayerName = "UIGamePlay";
        overlayRenderer.sortingOrder = 9999;

        // Efeito de Fade-In
        Color targetColor = color;
        Color transparentColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        overlayRenderer.color = transparentColor; // Começa transparente

        Debug.Log($"[GameManager] Iniciando fade-in do overlay para cor {targetColor} (duração: {fadeDuration}s)");

        // Usa a duração configurável do fade-in
        float elapsedTime = 0f;

        // Loop do fade-in
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Interpola a cor de transparente para opaca
            Color currentColor = Color.Lerp(transparentColor, targetColor, alpha);
            overlayRenderer.color = currentColor;

            yield return null;
        }

        // Garante que a cor final seja exatamente a desejada
        overlayRenderer.color = targetColor;

        Debug.Log($"[GameManager] Fade-in concluído - Overlay de escurecimento criado com cor {color} na sorting layer UIGamePlay");

        // Aguarda um tempo adicional configurável com a tela completamente escura
        yield return new WaitForSeconds(darkScreenDuration);

        Debug.Log("[GameManager] Escurecimento da cena concluído");

        // Nota: O overlay será destruído automaticamente quando a cena mudar
    }

    /// <summary>
    /// Configura a duração do fade-in programaticamente
    /// </summary>
    /// <param name="duration">Nova duração em segundos</param>
    public void SetFadeDuration(float duration)
    {
        fadeDuration = Mathf.Clamp(duration, 0.1f, 3.0f);
        Debug.Log($"[GameManager] Duração do fade ajustada para: {fadeDuration}s");
    }

    /// <summary>
    /// Configura o tempo de tela escura programaticamente
    /// </summary>
    /// <param name="duration">Nova duração em segundos</param>
    public void SetDarkScreenDuration(float duration)
    {
        darkScreenDuration = Mathf.Clamp(duration, 0.1f, 2.0f);
        Debug.Log($"[GameManager] Duração da tela escura ajustada para: {darkScreenDuration}s");
    }
    #endregion
}
