using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gerenciador principal do jogo - Versão Ultra-Otimizada para Carregamento Rápido
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Configurações
    [Header("Configurações de Transição - OTIMIZADAS")]
    [Tooltip("Duração do fade (0.1s = ultra rápido, 0.5s = normal)")]
    [Range(0.05f, 1.0f)]
    [SerializeField] private float fadeDuration = 0.1f; // Reduzido para ser mais rápido

    [Tooltip("Tempo de tela escura (0.1s = mínimo necessário)")]
    [Range(0.05f, 0.5f)]
    [SerializeField] private float darkScreenDuration = 0.1f; // Reduzido drasticamente

    [Tooltip("Delay antes do fade-out (0 = instantâneo)")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float fadeOutDelay = 0.0f; // Removido para velocidade máxima

    [Header("Otimizações de Performance")]
    [Tooltip("Pré-carregar próximas cenas em background")]
    [SerializeField] private bool enableScenePreloading = true;

    [Tooltip("Cache de overlays para reutilização")]
    [SerializeField] private bool enableOverlayCache = true;

    [Tooltip("Modo de carregamento ultra-rápido (pode causar micro-stutters)")]
    [SerializeField] private bool ultraFastMode = false;
    #endregion

    #region Cache Avançado
    // Cache de overlays reutilizáveis
    private readonly System.Collections.Generic.Dictionary<Color, GameObject> overlayCache =
        new System.Collections.Generic.Dictionary<Color, GameObject>();

    // Pre-cached textures para cores comuns
    private readonly System.Collections.Generic.Dictionary<Color, Texture2D> textureCache =
        new System.Collections.Generic.Dictionary<Color, Texture2D>();

    // Cache de scenes pré-carregadas
    private readonly System.Collections.Generic.Dictionary<string, AsyncOperation> preloadedScenes =
        new System.Collections.Generic.Dictionary<string, AsyncOperation>();

    // Cache de componentes principais
    private Camera mainCamera;
    private GameObject currentPlayer;
    private SpriteRenderer activeOverlayRenderer;

    // WaitForSeconds ultra-otimizados
    private WaitForSeconds ultraFastDarkWait;
    private WaitForSeconds ultraFastFadeWait;
    private WaitForEndOfFrame cachedEndFrame;
    private WaitForFixedUpdate cachedFixedUpdate;

    // Constantes para performance
    private const string OVERLAY_PREFIX = "FastOverlay_";
    private const string PLAYER_TAG = "Player";
    private const string UI_LAYER = "UI_Gameplay";
    private const int OVERLAY_SORTING_ORDER = 9999;
    private const float TEXTURE_SIZE = 1f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        // Singleton ultra-rápido
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFastCache();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PreloadCommonTextures();
        if (enableScenePreloading)
        {
            StartCoroutine(PreloadCommonScenes());
        }
    }
    #endregion

    #region Inicialização Ultra-Rápida
    /// <summary>
    /// Inicializa cache otimizado para velocidade máxima
    /// </summary>
    private void InitializeFastCache()
    {
        // Cache de WaitForSeconds com valores mínimos
        ultraFastDarkWait = new WaitForSeconds(darkScreenDuration);
        ultraFastFadeWait = new WaitForSeconds(fadeDuration);
        cachedEndFrame = new WaitForEndOfFrame();
        cachedFixedUpdate = new WaitForFixedUpdate();

        // Cache da câmera principal
        RefreshCameraCache();

        Debug.Log("[GameManager] Cache ultra-rápido inicializado");
    }

    /// <summary>
    /// Pré-carrega texturas para cores comuns
    /// </summary>
    private void PreloadCommonTextures()
    {
        Color[] commonColors = { Color.black, Color.white, new Color(0, 0, 0, 0.8f) };

        foreach (Color color in commonColors)
        {
            CreateCachedTexture(color);
        }

        Debug.Log($"[GameManager] {commonColors.Length} texturas pré-carregadas");
    }

    /// <summary>
    /// Cria e armazena texture no cache
    /// </summary>
    private Texture2D CreateCachedTexture(Color color)
    {
        if (textureCache.ContainsKey(color))
            return textureCache[color];

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();

        textureCache[color] = texture;
        return texture;
    }
    #endregion

    #region Sistema de Carregamento Ultra-Rápido
    /// <summary>
    /// Mudança de cena ultra-otimizada
    /// </summary>
    public void ChangeScene(string sceneName, Vector2 targetPosition = default, Color darkenColor = default)
    {
        if (darkenColor == default) darkenColor = Color.black;

        Debug.Log($"[GameManager] FAST: Carregando {sceneName} | Ultra: {ultraFastMode}");

        if (ultraFastMode)
        {
            StartCoroutine(UltraFastSceneChange(sceneName, targetPosition, darkenColor));
        }
        else
        {
            StartCoroutine(OptimizedSceneChange(sceneName, targetPosition, darkenColor));
        }
    }

    /// <summary>
    /// Carregamento ultra-rápido (pode causar micro-stutters)
    /// </summary>
    private IEnumerator UltraFastSceneChange(string sceneName, Vector2 targetPosition, Color darkenColor)
    {
        // 1. Overlay instantâneo
        SetupInstantOverlay(darkenColor);

        // 2. Carregamento direto sem delays
        yield return StartCoroutine(LoadSceneInstant(sceneName, targetPosition));

        // 3. Fade-out instantâneo ou muito rápido
        yield return StartCoroutine(InstantFadeOut());
    }

    /// <summary>
    /// Carregamento otimizado (equilibrio entre velocidade e suavidade)
    /// </summary>
    private IEnumerator OptimizedSceneChange(string sceneName, Vector2 targetPosition, Color darkenColor)
    {
        // 1. Fade-in ultra-rápido
        yield return StartCoroutine(FastDarkenScene(darkenColor));

        // 2. Carregamento assíncrono com preload
        yield return StartCoroutine(FastLoadScene(sceneName, targetPosition));

        // 3. Fade-out sem delays desnecessários
        yield return StartCoroutine(FastFadeOut());
    }
    #endregion

    #region Overlay Ultra-Otimizado
    /// <summary>
    /// Configura overlay instantâneo usando cache
    /// </summary>
    private void SetupInstantOverlay(Color color)
    {
        GameObject overlay = GetCachedOverlay(color);
        activeOverlayRenderer = overlay.GetComponent<SpriteRenderer>();
        activeOverlayRenderer.color = color; // Opaco instantâneo

        // Posiciona na câmera atual
        if (mainCamera != null)
        {
            overlay.transform.position = mainCamera.transform.position;
        }
    }

    /// <summary>
    /// Obtém overlay do cache ou cria novo
    /// </summary>
    private GameObject GetCachedOverlay(Color color)
    {
        if (enableOverlayCache && overlayCache.ContainsKey(color))
        {
            GameObject cached = overlayCache[color];
            if (cached != null)
            {
                cached.SetActive(true);
                return cached;
            }
        }

        // Cria novo overlay otimizado
        GameObject overlay = CreateFastOverlay(color);

        if (enableOverlayCache)
        {
            overlayCache[color] = overlay;
        }

        return overlay;
    }

    /// <summary>
    /// Cria overlay otimizado para velocidade
    /// </summary>
    private GameObject CreateFastOverlay(Color color)
    {
        GameObject overlay = new GameObject($"{OVERLAY_PREFIX}{color}");
        SpriteRenderer renderer = overlay.AddComponent<SpriteRenderer>();

        DontDestroyOnLoad(overlay);

        // Usa texture do cache
        Texture2D texture = CreateCachedTexture(color);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, TEXTURE_SIZE, TEXTURE_SIZE), Vector2.one * 0.5f);
        renderer.sprite = sprite;

        // Configura para cobertura total ultra-rápida
        RefreshCameraCache();
        if (mainCamera != null)
        {
            float cameraSize = mainCamera.orthographicSize;
            float aspectRatio = mainCamera.aspect;

            overlay.transform.localScale = new Vector3(
                cameraSize * aspectRatio * 200f,
                cameraSize * 200f,
                1f
            );
        }
        else
        {
            overlay.transform.localScale = Vector3.one * 50000f; // Fallback gigante
        }

        renderer.sortingLayerName = UI_LAYER;
        renderer.sortingOrder = OVERLAY_SORTING_ORDER;

        return overlay;
    }
    #endregion

    #region Carregamento Assíncrono Otimizado
    /// <summary>
    /// Carregamento instantâneo para ultra-fast mode
    /// </summary>
    private IEnumerator LoadSceneInstant(string sceneName, Vector2 targetPosition)
    {
        // Verifica se já está pré-carregada
        if (preloadedScenes.ContainsKey(sceneName))
        {
            AsyncOperation preloaded = preloadedScenes[sceneName];
            preloaded.allowSceneActivation = true;

            while (!preloaded.isDone)
            {
                yield return null;
            }

            preloadedScenes.Remove(sceneName);
            Debug.Log($"[GameManager] Cena pré-carregada ativada: {sceneName}");
        }
        else
        {
            // Carregamento direto
            SceneManager.LoadScene(sceneName);
            yield return null; // Apenas 1 frame de espera
        }

        // Posicionamento instantâneo
        PositionPlayerInstant(targetPosition);
    }

    /// <summary>
    /// Carregamento rápido com preload
    /// </summary>
    private IEnumerator FastLoadScene(string sceneName, Vector2 targetPosition)
    {
        AsyncOperation asyncLoad = null;

        // Usa preload se disponível
        if (preloadedScenes.ContainsKey(sceneName))
        {
            asyncLoad = preloadedScenes[sceneName];
            asyncLoad.allowSceneActivation = true;
            preloadedScenes.Remove(sceneName);
            Debug.Log($"[GameManager] Usando cena pré-carregada: {sceneName}");
        }
        else
        {
            asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        }

        // Loop otimizado de carregamento
        while (!asyncLoad.isDone)
        {
            // Sem logs de progresso para máxima velocidade
            yield return null;
        }

        // Posicionamento rápido do player
        yield return StartCoroutine(PositionPlayerFast(targetPosition));

        // Atualiza overlay para nova câmera
        UpdateOverlayForNewScene();
    }

    /// <summary>
    /// Posicionamento instantâneo do player
    /// </summary>
    private void PositionPlayerInstant(Vector2 targetPosition)
    {
        if (targetPosition == Vector2.zero) return;

        // Cache do player
        if (currentPlayer == null)
        {
            currentPlayer = GameObject.FindGameObjectWithTag(PLAYER_TAG);
        }

        if (currentPlayer != null)
        {
            currentPlayer.transform.position = targetPosition;

            // Desabilita física temporariamente para evitar conflitos
            Rigidbody2D rb = currentPlayer.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    /// <summary>
    /// Posicionamento rápido com validações
    /// </summary>
    private IEnumerator PositionPlayerFast(Vector2 targetPosition)
    {
        if (targetPosition == Vector2.zero) yield break;

        // Aguarda 1 frame para nova cena estar ativa
        yield return null;

        PositionPlayerInstant(targetPosition);

        Debug.Log($"[GameManager] Player posicionado em: {targetPosition}");
    }
    #endregion

    #region Fade Otimizado
    /// <summary>
    /// Escurecimento ultra-rápido
    /// </summary>
    private IEnumerator FastDarkenScene(Color color)
    {
        SetupInstantOverlay(Color.clear); // Começa transparente

        Color targetColor = color;
        float elapsed = 0f;

        // Loop de fade otimizado
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Usa unscaled para ignorar timeScale
            float alpha = elapsed / fadeDuration;

            activeOverlayRenderer.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
            yield return null;
        }

        activeOverlayRenderer.color = targetColor;
        yield return ultraFastDarkWait; // Mínimo necessário
    }

    /// <summary>
    /// Fade-out ultra-rápido
    /// </summary>
    private IEnumerator FastFadeOut()
    {
        if (activeOverlayRenderer == null) yield break;

        Color startColor = activeOverlayRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = 1f - (elapsed / fadeDuration);

            activeOverlayRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        activeOverlayRenderer.color = targetColor;

        // Desativa overlay se usando cache
        if (enableOverlayCache && activeOverlayRenderer.gameObject != null)
        {
            activeOverlayRenderer.gameObject.SetActive(false);
        }

        Debug.Log("[GameManager] Transição ultra-rápida concluída!");
    }

    /// <summary>
    /// Fade-out instantâneo para ultra-fast mode
    /// </summary>
    private IEnumerator InstantFadeOut()
    {
        yield return null; // 1 frame mínimo

        if (activeOverlayRenderer != null)
        {
            activeOverlayRenderer.color = Color.clear;

            if (enableOverlayCache)
            {
                activeOverlayRenderer.gameObject.SetActive(false);
            }
        }

        Debug.Log("[GameManager] Fade instantâneo concluído!");
    }
    #endregion

    #region Sistema de Preload
    /// <summary>
    /// Pré-carrega cenas comuns em background
    /// </summary>
    private IEnumerator PreloadCommonScenes()
    {
        string[] commonScenes = { "MainMenu", "GameScene", "Level1", "Level2" }; // Configure conforme necessário

        foreach (string sceneName in commonScenes)
        {
            if (!preloadedScenes.ContainsKey(sceneName) && sceneName != SceneManager.GetActiveScene().name)
            {
                AsyncOperation preload = SceneManager.LoadSceneAsync(sceneName);
                preload.allowSceneActivation = false; // Não ativa automaticamente

                preloadedScenes[sceneName] = preload;

                Debug.Log($"[GameManager] Pré-carregando: {sceneName}");

                // Aguarda um frame entre preloads para não travar
                yield return null;
            }
        }

        Debug.Log($"[GameManager] {preloadedScenes.Count} cenas pré-carregadas");
    }

    /// <summary>
    /// Pré-carrega uma cena específica
    /// </summary>
    public void PreloadScene(string sceneName)
    {
        if (!preloadedScenes.ContainsKey(sceneName))
        {
            StartCoroutine(PreloadSpecificScene(sceneName));
        }
    }

    private IEnumerator PreloadSpecificScene(string sceneName)
    {
        AsyncOperation preload = SceneManager.LoadSceneAsync(sceneName);
        preload.allowSceneActivation = false;

        preloadedScenes[sceneName] = preload;

        Debug.Log($"[GameManager] Cena {sceneName} pré-carregada");
        yield return null;
    }
    #endregion

    #region Métodos Utilitários
    /// <summary>
    /// Atualiza cache da câmera
    /// </summary>
    private void RefreshCameraCache()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindAnyObjectByType<Camera>();
            }
        }
    }

    /// <summary>
    /// Atualiza overlay para nova cena
    /// </summary>
    private void UpdateOverlayForNewScene()
    {
        RefreshCameraCache();
        currentPlayer = null; // Reset player cache

        if (activeOverlayRenderer != null && mainCamera != null)
        {
            activeOverlayRenderer.transform.position = mainCamera.transform.position;
        }
    }

    /// <summary>
    /// Limpa caches desnecessários
    /// </summary>
    public void ClearCaches()
    {
        // Limpa preloads antigos
        foreach (var kvp in preloadedScenes)
        {
            if (kvp.Value.isDone)
            {
                preloadedScenes.Remove(kvp.Key);
            }
        }

        // Reset cache de objetos
        currentPlayer = null;
        mainCamera = null;

        Debug.Log("[GameManager] Caches limpos");
    }
    #endregion

    #region Configuração Dinâmica
    /// <summary>
    /// Ativa modo ultra-rápido (pode causar stutters)
    /// </summary>
    public void EnableUltraFastMode(bool enable)
    {
        ultraFastMode = enable;
        Debug.Log($"[GameManager] Modo ultra-rápido: {(enable ? "ATIVADO" : "DESATIVADO")}");
    }

    /// <summary>
    /// Configura velocidade de fade dinamicamente
    /// </summary>
    public void SetFadeSpeed(float duration)
    {
        fadeDuration = Mathf.Clamp(duration, 0.05f, 1.0f);
        ultraFastFadeWait = new WaitForSeconds(fadeDuration);
        Debug.Log($"[GameManager] Velocidade de fade: {fadeDuration}s");
    }
    #endregion
}
