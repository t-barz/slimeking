using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using PixeLadder.EasyTransition;

/// <summary>
/// Controller principal da cena TitleScreen.
/// Gerencia a inicialização e comportamento específico desta cena.
/// Sequência: Background fade in → Logo do jogo fade in → Demais elementos
/// </summary>
public class TitleScreenController : MonoBehaviour
{
    #region UI Elements - GameObjects da Cena
    [Header("UI Elements - GameObjects da Cena")]
    [SerializeField] private GameObject background;              // ui_background
    [SerializeField] private GameObject gameLogo;               // ui_gameLogo
    [SerializeField] private GameObject wsSmallLogo;            // ui_wsSmallLogo
    [SerializeField] private GameObject textElement;            // ui_txt
    #endregion

    #region Scene Transition
    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = "InitialCave";
    [SerializeField] private TransitionEffect transitionEffect;
    #endregion

    #region Input
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    #endregion

    #region Sequence Settings
    [Header("Sequence Settings")]
    [SerializeField] private float delayBeforeGameLogo = 0.5f;
    [SerializeField] private float delayBeforeOtherElements = 0.3f;
    #endregion

    #region Fade Durations
    [Header("Fade Durations")]
    [SerializeField] private float backgroundFadeInDuration = 1.5f;
    [SerializeField] private float gameLogoFadeInDuration = 1f;
    [SerializeField] private float otherElementsFadeInDuration = 0.8f;
    #endregion

    #region Logo Animation
    [Header("Logo Animation")]
    [SerializeField] private bool enableLogoAnimation = true;
    [SerializeField] private float logoBouncingHeight = 15f;
    [SerializeField] private float logoBouncingDuration = 2f;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Private Variables
    private TitleScreenState currentState = TitleScreenState.Initial;
    private Vector3 gameLogoOriginalPosition;
    private Coroutine logoAnimationCoroutine;
    private InputAction anyButtonAction;
    private bool canProceed = false;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        InitializeScreen();
        SetupInput();
        StartSequence();
    }

    private void OnEnable()
    {
        if (anyButtonAction != null)
        {
            anyButtonAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (anyButtonAction != null)
        {
            anyButtonAction.Disable();
        }
    }

    private void OnDestroy()
    {
        if (anyButtonAction != null)
        {
            anyButtonAction.performed -= OnAnyButtonPressed;
            anyButtonAction.Dispose();
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Configura o estado inicial da tela, escondendo todos os elementos
    /// </summary>
    private void InitializeScreen()
    {
        Log("=== Initializing Title Screen ===");

        // Guarda a posição original do logo do jogo
        if (gameLogo != null)
        {
            gameLogoOriginalPosition = gameLogo.transform.localPosition;
        }

        // Esconde todos os elementos inicialmente (alpha 0)
        SetGameObjectVisibility(background, false);
        SetGameObjectVisibility(gameLogo, false);
        SetGameObjectVisibility(wsSmallLogo, false);
        SetGameObjectVisibility(textElement, false);

        currentState = TitleScreenState.Initial;
        Log("Title screen initialized - all elements hidden");
    }

    /// <summary>
    /// Configura o input para detectar qualquer botão pressionado.
    /// NOTA: Este jogo suporta APENAS Teclado e Gamepad. Mouse e Touch NÃO são suportados.
    /// </summary>
    private void SetupInput()
    {
        // Cria uma ação que detecta qualquer input
        anyButtonAction = new InputAction("AnyButton", InputActionType.PassThrough);

        // Adiciona bindings APENAS para Teclado e Gamepad
        // IMPORTANTE: Mouse e Touchscreen NÃO são suportados neste jogo
        anyButtonAction.AddBinding("<Keyboard>/anyKey");
        anyButtonAction.AddBinding("<Gamepad>/buttonSouth");
        anyButtonAction.AddBinding("<Gamepad>/buttonNorth");
        anyButtonAction.AddBinding("<Gamepad>/buttonEast");
        anyButtonAction.AddBinding("<Gamepad>/buttonWest");
        anyButtonAction.AddBinding("<Gamepad>/start");

        // Registra o callback
        anyButtonAction.performed += OnAnyButtonPressed;
        anyButtonAction.Enable();

        Log("Input system configured");
    }
    #endregion

    #region Sequence Management
    /// <summary>
    /// Inicia a sequência principal da tela inicial
    /// </summary>
    private void StartSequence()
    {
        StartCoroutine(TitleSequence());
    }

    /// <summary>
    /// Coroutine principal que executa toda a sequência da tela inicial
    /// </summary>
    private IEnumerator TitleSequence()
    {
        Log("Starting title sequence");

        // 1. Background aparece com fade
        currentState = TitleScreenState.ShowingBackground;
        yield return StartCoroutine(FadeGameObject(background, 1f, backgroundFadeInDuration));
        Log("Background fade completed");

        // 2. Aguarda delay e mostra o logo do jogo
        yield return new WaitForSeconds(delayBeforeGameLogo);
        currentState = TitleScreenState.ShowingGameLogo;
        yield return StartCoroutine(FadeGameObject(gameLogo, 1f, gameLogoFadeInDuration));
        Log("Game logo fade completed");

        // Inicia a animação de bouncing do logo
        if (enableLogoAnimation && gameLogo != null)
        {
            StartGameLogoAnimation();
        }

        // 3. Aguarda delay e mostra os demais elementos
        yield return new WaitForSeconds(delayBeforeOtherElements);
        currentState = TitleScreenState.ShowingOtherElements;

        // Mostra elementos restantes simultaneamente
        StartCoroutine(FadeGameObject(wsSmallLogo, 1f, otherElementsFadeInDuration));
        yield return StartCoroutine(FadeGameObject(textElement, 1f, otherElementsFadeInDuration));
        Log("Other elements fade completed");

        currentState = TitleScreenState.SequenceComplete;
        canProceed = true;
        Log("Title sequence completed - ready for input");
    }
    #endregion

    #region Input Handling
    /// <summary>
    /// Callback chamado quando qualquer botão é pressionado
    /// </summary>
    private void OnAnyButtonPressed(InputAction.CallbackContext context)
    {
        if (!canProceed || currentState != TitleScreenState.SequenceComplete)
        {
            Log("Input received but not ready to proceed");
            return;
        }

        Log("Starting scene transition");
        LoadNextScene();
    }

    /// <summary>
    /// Carrega a próxima cena usando Easy Transition
    /// </summary>
    private void LoadNextScene()
    {
        canProceed = false; // Previne múltiplos inputs

        if (SceneTransitioner.Instance != null && transitionEffect != null)
        {
            Log($"Loading scene '{nextSceneName}' with Circle transition effect");
            SceneTransitioner.Instance.LoadScene(nextSceneName, transitionEffect);
        }
        else if (SceneTransitioner.Instance != null)
        {
            Log($"Loading scene '{nextSceneName}' without transition effect");
            SceneTransitioner.Instance.LoadScene(nextSceneName, null);
        }
        else
        {
            Log("SceneTransitioner instance not found, loading scene directly");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
    #endregion

    #region Logo Animation
    /// <summary>
    /// Inicia a animação de bouncing do logo do jogo
    /// </summary>
    private void StartGameLogoAnimation()
    {
        if (logoAnimationCoroutine != null)
        {
            StopCoroutine(logoAnimationCoroutine);
        }

        logoAnimationCoroutine = StartCoroutine(GameLogoAnimationLoop());
        Log("Game logo animation started");
    }

    /// <summary>
    /// Para a animação do logo do jogo
    /// </summary>
    private void StopGameLogoAnimation()
    {
        if (logoAnimationCoroutine != null)
        {
            StopCoroutine(logoAnimationCoroutine);
            logoAnimationCoroutine = null;
        }

        // Retorna o logo para a posição original
        if (gameLogo != null)
        {
            gameLogo.transform.localPosition = gameLogoOriginalPosition;
        }

        Log("Game logo animation stopped");
    }

    /// <summary>
    /// Loop contínuo da animação de bouncing usando movimento senoidal suave
    /// </summary>
    private IEnumerator GameLogoAnimationLoop()
    {
        if (gameLogo == null) yield break;

        float time = 0f;
        float frequency = 1f / logoBouncingDuration;

        while (true)
        {
            time += Time.deltaTime;

            // Usa função senoidal para movimento suave e contínuo
            float yOffset = Mathf.Sin(time * frequency * 2f * Mathf.PI) * logoBouncingHeight;

            // Aplica o offset à posição original
            Vector3 newPosition = gameLogoOriginalPosition + Vector3.up * yOffset;
            gameLogo.transform.localPosition = newPosition;

            yield return null;
        }
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Define a visibilidade de um GameObject manipulando o alpha
    /// </summary>
    private void SetGameObjectVisibility(GameObject obj, bool visible)
    {
        if (obj == null) return;

        float targetAlpha = visible ? 1f : 0f;

        // Tenta usar CanvasGroup primeiro (mais eficiente)
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = targetAlpha;
            return;
        }

        // Fallback para componentes individuais
        var images = obj.GetComponentsInChildren<UnityEngine.UI.Image>();
        foreach (var img in images)
        {
            Color color = img.color;
            color.a = targetAlpha;
            img.color = color;
        }

        var texts = obj.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (var text in texts)
        {
            Color color = text.color;
            color.a = targetAlpha;
            text.color = color;
        }
    }

    /// <summary>
    /// Fade usando CanvasGroup ou fallback manual
    /// </summary>
    private IEnumerator FadeGameObject(GameObject obj, float targetAlpha, float duration)
    {
        if (obj == null) yield break;

        // Verifica se tem CanvasGroup
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }
        else
        {
            // Fallback manual para objetos sem CanvasGroup
            yield return StartCoroutine(FadeGameObjectManual(obj, targetAlpha, duration));
        }
    }

    /// <summary>
    /// Fallback para fade manual quando CanvasGroup não está disponível
    /// </summary>
    private IEnumerator FadeGameObjectManual(GameObject obj, float targetAlpha, float duration)
    {
        if (obj == null) yield break;

        var images = obj.GetComponentsInChildren<UnityEngine.UI.Image>();
        var texts = obj.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

        float[] startAlphas = new float[images.Length + texts.Length];
        int index = 0;

        // Guarda valores iniciais
        foreach (var img in images) startAlphas[index++] = img.color.a;
        foreach (var text in texts) startAlphas[index++] = text.color.a;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            index = 0;

            foreach (var img in images)
            {
                Color color = img.color;
                color.a = Mathf.Lerp(startAlphas[index], targetAlpha, progress);
                img.color = color;
                index++;
            }

            foreach (var text in texts)
            {
                Color color = text.color;
                color.a = Mathf.Lerp(startAlphas[index], targetAlpha, progress);
                text.color = color;
                index++;
            }

            yield return null;
        }

        // Define valores finais
        foreach (var img in images)
        {
            Color color = img.color;
            color.a = targetAlpha;
            img.color = color;
        }

        foreach (var text in texts)
        {
            Color color = text.color;
            color.a = targetAlpha;
            text.color = color;
        }
    }
    #endregion

    #region Debug
    /// <summary>
    /// Log condicional para debug
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[TitleScreenController] {message}");
        }
    }
    #endregion
}

#region Enums
/// <summary>
/// Estados da tela inicial
/// </summary>
public enum TitleScreenState
{
    Initial,
    ShowingBackground,
    ShowingGameLogo,
    ShowingOtherElements,
    SequenceComplete
}
#endregion
