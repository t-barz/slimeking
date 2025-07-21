using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

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

    [Tooltip("Tempo de espera antes de iniciar o fade-out na nova cena (em segundos)")]
    [Range(0.0f, 5.0f)]
    [SerializeField] private float fadeOutDelay = 1.0f;
    #endregion

    #region Private Variables
    /// <summary>
    /// Overlay persistente que se mantém entre as cenas
    /// </summary>
    private GameObject persistentOverlay;
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
            if (persistentOverlay != null)
            {
                Destroy(persistentOverlay);
            }
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
        // 4. Carrega a nova cena em asynchronous mode (overlay permanece)
        yield return StartCoroutine(LoadSceneAsync(sceneName, targetPosition));
    }

    /// <summary>
    /// Carrega a nova cena de forma assíncrona
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName, Vector2 targetPosition)
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

        // Posiciona o Player na posição de destino
        PositionPlayerAtTarget(targetPosition);

        // Reposiciona o overlay para a nova câmera
        UpdateOverlayPosition();

        // Aguarda o delay configurado antes de iniciar o fade-out
        if (fadeOutDelay > 0f)
        {
            Debug.Log($"[GameManager] Aguardando {fadeOutDelay}s antes do fade-out");
            yield return new WaitForSeconds(fadeOutDelay);
        }

        // Sempre executa fade-out na nova cena
        yield return StartCoroutine(FadeOutScene());
    }

    /// <summary>
    /// Posiciona o objeto Player na posição de destino
    /// </summary>
    private void PositionPlayerAtTarget(Vector2 targetPosition)
    {
        // Procura o objeto com tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Define a posição do Player (mantém Z original)
            Vector3 newPosition = new Vector3(targetPosition.x, targetPosition.y, player.transform.position.z);
            player.transform.position = newPosition;

            Debug.Log($"[GameManager] Player posicionado na posição: {newPosition}");

            // Para qualquer movimento residual se houver Rigidbody2D
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                Debug.Log("[GameManager] Velocidade do Player resetada");
            }

        }
        else
        {
            Debug.LogWarning("[GameManager] Objeto com tag 'Player' não encontrado na nova cena");
        }
    }

    /// <summary>
    /// Executa fade-out para remover o overlay escuro da nova cena
    /// </summary>
    private IEnumerator FadeOutScene()
    {
        Debug.Log("[GameManager] Iniciando fade-out da nova cena");

        // Usa o overlay persistente
        if (persistentOverlay != null)
        {
            SpriteRenderer overlayRenderer = persistentOverlay.GetComponent<SpriteRenderer>();

            if (overlayRenderer != null)
            {
                // Garante que o overlay comece completamente opaco
                Color currentColor = overlayRenderer.color;
                Color opaqueColor = new Color(currentColor.r, currentColor.g, currentColor.b, 1f);
                Color transparentColor = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);

                // Define como completamente opaco no início
                overlayRenderer.color = opaqueColor;

                Debug.Log($"[GameManager] Fazendo fade-out do overlay persistente (duração: {fadeDuration}s)");

                float elapsedTime = 0f;

                // Loop do fade-out - vai de opaco para transparente
                while (elapsedTime < fadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);

                    // Interpola a cor de opaca para transparente
                    Color fadeColor = new Color(opaqueColor.r, opaqueColor.g, opaqueColor.b, alpha);
                    overlayRenderer.color = fadeColor;

                    yield return null;
                }

                // Garante que seja completamente transparente
                overlayRenderer.color = transparentColor;

                Debug.Log("[GameManager] Fade-out concluído - overlay agora transparente");
            }
        }

        Debug.Log("[GameManager] Transição de cena completamente finalizada!");
    }

    /// <summary>
    /// Atualiza a posição do overlay para a nova câmera
    /// </summary>
    private void UpdateOverlayPosition()
    {
        if (persistentOverlay == null) return;

        // Busca a câmera ativa na nova cena
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

            // Posiciona no centro da nova câmera
            persistentOverlay.transform.position = new Vector3(
                activeCamera.transform.position.x,
                activeCamera.transform.position.y,
                0f
            );

            // Atualiza a escala para a nova câmera
            persistentOverlay.transform.localScale = new Vector3(cameraWidth * 1000f, cameraHeight * 1000f, 1f);

            Debug.Log($"[GameManager] Overlay reposicionado para nova câmera - Scale: {cameraWidth * 1000f}x{cameraHeight * 1000f}");
        }
    }

    /// <summary>
    /// Escurece a cena atual com a cor especificada
    /// </summary>
    private IEnumerator DarkenScene(Color color)
    {
        Debug.Log($"[GameManager] Escurecendo cena com a cor: {color}");

        // Cria overlay persistente se não existir
        if (persistentOverlay == null)
        {
            persistentOverlay = CreatePersistentOverlay(color);
        }
        else
        {
            // Atualiza a cor do overlay existente
            SpriteRenderer overlayRenderer = persistentOverlay.GetComponent<SpriteRenderer>();
            if (overlayRenderer != null)
            {
                // Atualiza a textura com a nova cor
                Texture2D darkenTexture = new Texture2D(1, 1);
                darkenTexture.SetPixel(0, 0, color);
                darkenTexture.Apply();

                Sprite darkenSprite = Sprite.Create(darkenTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f);
                overlayRenderer.sprite = darkenSprite;
            }
        }

        SpriteRenderer renderer = persistentOverlay.GetComponent<SpriteRenderer>();

        // Efeito de Fade-In
        Color targetColor = color;
        Color transparentColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        renderer.color = transparentColor; // Começa transparente

        Debug.Log($"[GameManager] Iniciando fade-in do overlay persistente para cor {targetColor} (duração: {fadeDuration}s)");

        // Usa a duração configurável do fade-in
        float elapsedTime = 0f;

        // Loop do fade-in
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            // Interpola a cor de transparente para opaca
            Color currentColor = Color.Lerp(transparentColor, targetColor, alpha);
            renderer.color = currentColor;

            yield return null;
        }

        // Garante que a cor final seja exatamente a desejada
        renderer.color = targetColor;

        Debug.Log($"[GameManager] Fade-in concluído - Overlay persistente escuro");

        // Aguarda um tempo adicional configurável com a tela completamente escura
        yield return new WaitForSeconds(darkScreenDuration);

        Debug.Log("[GameManager] Escurecimento da cena concluído");
    }

    /// <summary>
    /// Cria um overlay persistente que sobrevive às mudanças de cena
    /// </summary>
    private GameObject CreatePersistentOverlay(Color color)
    {
        // Cria um overlay que cobre toda a tela
        GameObject overlay = new GameObject("PersistentDarkenOverlay");
        SpriteRenderer overlayRenderer = overlay.AddComponent<SpriteRenderer>();

        // Torna o overlay persistente entre cenas
        DontDestroyOnLoad(overlay);

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
            overlay.transform.position = new Vector3(
                activeCamera.transform.position.x,
                activeCamera.transform.position.y,
                0f
            );

            // Escala muito maior para garantir cobertura total
            overlay.transform.localScale = new Vector3(cameraWidth * 200f, cameraHeight * 200f, 1f);

            Debug.Log($"[GameManager] Overlay persistente criado - CameraSize: {activeCamera.orthographicSize}, Scale: {cameraWidth * 200f}x{cameraHeight * 200f}");
        }
        else
        {
            Debug.LogWarning("[GameManager] Nenhuma câmera encontrada - usando escala muito grande");
            // Fallback com escala muito grande
            overlay.transform.localScale = new Vector3(50000f, 50000f, 1f);
        }

        // Configura a sorting layer e order
        overlayRenderer.sortingLayerName = "UIGamePlay";
        overlayRenderer.sortingOrder = 9999;

        Debug.Log($"[GameManager] Overlay persistente criado com cor {color}");

        return overlay;
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

    /// <summary>
    /// Configura o delay antes do fade-out programaticamente
    /// </summary>
    /// <param name="delay">Novo delay em segundos</param>
    public void SetFadeOutDelay(float delay)
    {
        fadeOutDelay = Mathf.Clamp(delay, 0.0f, 5.0f);
        Debug.Log($"[GameManager] Delay do fade-out ajustado para: {fadeOutDelay}s");
    }
    #endregion
}
