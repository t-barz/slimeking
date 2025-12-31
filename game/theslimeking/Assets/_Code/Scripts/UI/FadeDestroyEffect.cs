using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente que aplica efeito de fade out e destrói o GameObject após o desaparecimento
/// </summary>
public class FadeDestroyEffect : MonoBehaviour
{
    [Header("Fade Configuration")]
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private bool enableDebugLogs = false;

    [Tooltip("Tempo de espera após fade completo antes de destruir o objeto")]
    [SerializeField] private float destroyDelay = 0.5f;

    [Header("Curve Settings")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private bool _isDestroyed = false;
    private bool _fadeStarted = false;

    // Componentes que podem ter alpha
    private CanvasGroup _canvasGroup;
    private SpriteRenderer _spriteRenderer;
    private Image _image;
    private Text _text;

    #region Unity Lifecycle

    private void Awake()
    {
        CacheComponents();

        if (startOnAwake)
        {
            StartFadeDestroy();
        }
    }

    #endregion

    #region Public Interface

    /// <summary>
    /// Inicia o efeito de fade out e destruição
    /// </summary>
    public void StartFadeDestroy()
    {
        if (_fadeStarted || _isDestroyed)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"[FadeDestroyEffect] {gameObject.name} - Fade já iniciado ou objeto já destruído");
            }
            return;
        }

        _fadeStarted = true;
        StartCoroutine(FadeAndDestroy());
    }

    /// <summary>
    /// Destrói imediatamente o objeto sem fade
    /// </summary>
    public void DestroyImmediate()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;

        if (enableDebugLogs)
        {
            Debug.Log($"[FadeDestroyEffect] {gameObject.name} - Destruição imediata");
        }

        Destroy(gameObject);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Busca e armazena componentes que podem ser fadados
    /// </summary>
    private void CacheComponents()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _image = GetComponent<Image>();
        _text = GetComponent<Text>();

        // Se não encontrar nenhum componente apropriado, tenta nos filhos
        if (_canvasGroup == null && _spriteRenderer == null && _image == null && _text == null)
        {
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _image = GetComponentInChildren<Image>();
            _text = GetComponentInChildren<Text>();
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[FadeDestroyEffect] {gameObject.name} - Componentes encontrados: " +
                     $"CanvasGroup={_canvasGroup != null}, SpriteRenderer={_spriteRenderer != null}, " +
                     $"Image={_image != null}, Text={_text != null}");
        }
    }

    /// <summary>
    /// Corrotina que executa o fade out e destrói o objeto
    /// </summary>
    private IEnumerator FadeAndDestroy()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[FadeDestroyEffect] {gameObject.name} - Iniciando fade out (duração: {fadeDuration}s)");
        }

        float elapsedTime = 0f;
        float initialAlpha = GetCurrentAlpha();

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeDuration;

            // Usa a curva de animação para suavizar o fade
            float curveValue = fadeCurve.Evaluate(normalizedTime);
            float newAlpha = Mathf.Lerp(initialAlpha, 0f, curveValue);

            SetAlpha(newAlpha);

            yield return null;
        }

        // Garante que o alpha seja 0 no final
        SetAlpha(0f);

        if (enableDebugLogs)
        {
            Debug.Log($"[FadeDestroyEffect] {gameObject.name} - Fade concluído, aguardando {destroyDelay}s para destruir");
        }

        // Aguarda o delay antes de destruir
        yield return new WaitForSeconds(destroyDelay);

        if (enableDebugLogs)
        {
            Debug.Log($"[FadeDestroyEffect] {gameObject.name} - Delay concluído, destruindo objeto");
        }

        // Marca como destruído e destrói o GameObject
        _isDestroyed = true;
        Destroy(gameObject);
    }

    /// <summary>
    /// Obtém o valor atual do alpha dos componentes disponíveis
    /// </summary>
    private float GetCurrentAlpha()
    {
        if (_canvasGroup != null) return _canvasGroup.alpha;
        if (_spriteRenderer != null) return _spriteRenderer.color.a;
        if (_image != null) return _image.color.a;
        if (_text != null) return _text.color.a;

        return 1f; // Default se não encontrar nenhum componente
    }

    /// <summary>
    /// Define o valor do alpha nos componentes disponíveis
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = alpha;
        }

        if (_spriteRenderer != null)
        {
            Color color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }

        if (_image != null)
        {
            Color color = _image.color;
            color.a = alpha;
            _image.color = color;
        }

        if (_text != null)
        {
            Color color = _text.color;
            color.a = alpha;
            _text.color = color;
        }
    }

    #endregion
}
