using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using PixeLadder.EasyTransition;
using SlimeKing.Core; // acesso ao GameManager

/// <summary>
/// Controller específico para a tela inicial, gerencia a sequência de apresentação
/// seguindo o padrão: logo empresa → fundo jogo → logo jogo → elementos menu
/// </summary>
public class TitleScreenController : MonoBehaviour
{
    #region UI Elements - GameObjects da Cena
    [Header("UI Elements - GameObjects da Cena")]
    [SerializeField] private GameObject companyLogo;        // ui_wslogo
    [SerializeField] private GameObject background;         // ui_background
    [SerializeField] private GameObject gameLogo;          // ui_gamelogo
    [SerializeField] private GameObject companySmallLogo;  // ui_wssmalllogo
    [SerializeField] private GameObject pressStart;        // ui_pressstart
    #endregion

    #region Input UI Elements
    [Header("Input UI Elements")]
    [SerializeField] private GameObject inputKeyboard;     // ui_keyboard
    [SerializeField] private GameObject inputGamepad;      // ui_gamepad
    [SerializeField] private GameObject inputXbox;         // ui_xbox
    [SerializeField] private GameObject inputPlaystation;  // ui_playstation
    [SerializeField] private GameObject inputSwitch;       // ui_switch
    #endregion

    #region Sequence Settings
    [Header("Sequence Settings")]
    [SerializeField] private float musicStartDelay = 1f;
    [SerializeField] private float companyLogoVisibleTime = 4f;
    #endregion

    #region Fade Durations
    [Header("Fade Durations")]
    [SerializeField] private float companyLogoFadeInDuration = 1f;
    [SerializeField] private float companyLogoFadeOutDuration = 1f;
    [SerializeField] private float backgroundFadeInDuration = 1f;
    [SerializeField] private float gameLogoFadeInDuration = 1f;
    [SerializeField] private float menuElementsFadeInDuration = 1f;
    #endregion

    #region Sequence Delays
    [Header("Sequence Delays")]
    [SerializeField] private float delayBeforeGameLogo = 0.5f;
    [SerializeField] private float delayBeforeMenuElements = 0.3f;
    #endregion

    #region Game Logo Animation
    [Header("Game Logo Animation")]
    [SerializeField] private bool enableLogoAnimation = true;
    [SerializeField] private float logoMoveUpDistance = 10f;
    [SerializeField] private float logoMoveDownDistance = 15f;
    [SerializeField] private float logoMoveDuration = 1.5f;
    #endregion

    #region Audio
    [Header("Audio")]
    [SerializeField] private AudioClip titleMusic;
    #endregion

    #region Scene Transition
    [Header("Scene Transition")]
    [SerializeField] private string gameSceneName = "InitialCave";
    [SerializeField] private bool useCustomTransitionEffect = false;
    [SerializeField] private TransitionEffect customTransitionEffect;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Private Variables
    private TitleScreenState currentState = TitleScreenState.Initial;
    private bool canSkip = false;
    private bool hasSkipped = false;
    private bool inputReceived = false;
    private GameObject currentInputUI = null;
    private InputType lastKnownInputType = InputType.Keyboard;

    // Logo Animation
    private Vector3 gameLogoOriginalPosition;
    private Coroutine logoAnimationCoroutine;
    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        InitializeScreen();
        StartSequence();
    }

    private void Update()
    {
        // Bloqueia completamente a detecção de input durante a transição
        if (currentState == TitleScreenState.TransitionStarted)
        {
            return;
        }

        CheckForAnyInput();
        UpdateInputUIIfChanged();

        if (inputReceived)
        {
            HandleInput();
            inputReceived = false;
        }
    }

    private void OnDestroy()
    {
        // Para a animação do logo quando o objeto for destruído
        StopGameLogoAnimation();
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

        // Esconde todos os elementos inicialmente
        SetGameObjectVisibility(companyLogo, false);
        SetGameObjectVisibility(background, false);
        SetGameObjectVisibility(gameLogo, false);
        SetGameObjectVisibility(companySmallLogo, false);
        SetGameObjectVisibility(pressStart, false);

        // Desativa todos os input UIs
        DeactivateAllInputUIs();

        currentState = TitleScreenState.Initial;
        Log("Title screen initialized");
    }

    /// <summary>
    /// Desativa todos os input UIs
    /// </summary>
    private void DeactivateAllInputUIs()
    {
        SetInputUIActive(inputKeyboard, false);
        SetInputUIActive(inputGamepad, false);
        SetInputUIActive(inputXbox, false);
        SetInputUIActive(inputPlaystation, false);
        SetInputUIActive(inputSwitch, false);
        currentInputUI = null;
    }

    /// <summary>
    /// Inicializa o input UI correto após o menu estar pronto
    /// </summary>
    private void InitializeInputUI()
    {
        DetectCurrentInputType();
        UpdateInputUI();
        Log($"Input UI initialized with type: {lastKnownInputType}");
    }

    #endregion

    #region Input Detection

    /// <summary>
    /// Detecta qualquer input usando o Input System
    /// </summary>
    private void CheckForAnyInput()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            inputReceived = true;
            return;
        }

        if (Gamepad.current != null)
        {
            foreach (var control in Gamepad.current.allControls)
            {
                if (control is UnityEngine.InputSystem.Controls.ButtonControl button && button.wasPressedThisFrame)
                {
                    inputReceived = true;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Detecta o tipo de input atual baseado nos dispositivos conectados
    /// </summary>
    private void DetectCurrentInputType()
    {
        if (Gamepad.current != null)
        {
            lastKnownInputType = InputType.Gamepad;
        }
        else
        {
            lastKnownInputType = InputType.Keyboard;
        }
    }

    /// <summary>
    /// Atualiza o input UI se o tipo mudou
    /// </summary>
    private void UpdateInputUIIfChanged()
    {
        if (currentState != TitleScreenState.MenuReady) return;

        DetectCurrentInputType();
        GameObject targetInputUI = GetCurrentInputUI();

        if (currentInputUI != targetInputUI)
        {
            UpdateInputUI();
        }
    }

    /// <summary>
    /// Atualiza a UI do input baseado no dispositivo atual
    /// </summary>
    private void UpdateInputUI()
    {
        GameObject targetInputUI = GetCurrentInputUI();

        if (currentInputUI != targetInputUI)
        {
            // Desativa o UI anterior
            if (currentInputUI != null)
            {
                SetInputUIActive(currentInputUI, false);
            }

            // Ativa o novo UI
            currentInputUI = targetInputUI;
            if (currentInputUI != null)
            {
                SetInputUIActive(currentInputUI, true);
                SetGameObjectVisibility(currentInputUI, true);
            }
        }
    }

    /// <summary>
    /// Retorna o GameObject de input UI apropriado para o tipo atual
    /// </summary>
    private GameObject GetCurrentInputUI()
    {
        return lastKnownInputType switch
        {
            InputType.Keyboard => inputKeyboard,
            InputType.Xbox => inputXbox ?? inputGamepad,
            InputType.PlayStation => inputPlaystation ?? inputGamepad,
            InputType.Switch => inputSwitch ?? inputGamepad,
            InputType.Gamepad => inputGamepad,
            _ => inputKeyboard // Fallback
        };
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

        // Aguarda tempo inicial e inicia música
        yield return new WaitForSeconds(musicStartDelay);
        PlayTitleMusic();

        // Sequência do logo da empresa
        yield return StartCoroutine(ShowCompanyLogo());
        if (hasSkipped) yield break;

        yield return new WaitForSeconds(companyLogoVisibleTime);
        if (hasSkipped) yield break;

        yield return StartCoroutine(HideCompanyLogo());
        if (hasSkipped) yield break;

        // Sequência dos elementos do jogo
        yield return StartCoroutine(ShowGameElements());
        if (hasSkipped) yield break;

        yield return StartCoroutine(ShowMenuElements());

        // Finaliza sequência
        currentState = TitleScreenState.MenuReady;
        canSkip = false;
        InitializeInputUI();

        Log("Title sequence completed");
    }

    /// <summary>
    /// Mostra o logo da empresa com fade
    /// </summary>
    private IEnumerator ShowCompanyLogo()
    {
        currentState = TitleScreenState.CompanyLogo;
        canSkip = true;
        Log("Showing company logo");

        // Pré-carrega a cena principal assim que a primeira animação inicia
        GameManager.Instance?.PreloadScene(gameSceneName);

        yield return StartCoroutine(FadeGameObject(companyLogo, 1f, companyLogoFadeInDuration));
    }

    /// <summary>
    /// Esconde o logo da empresa com fade
    /// </summary>
    private IEnumerator HideCompanyLogo()
    {
        currentState = TitleScreenState.Transition;
        Log("Hiding company logo");

        yield return StartCoroutine(FadeGameObject(companyLogo, 0f, companyLogoFadeOutDuration));
    }

    /// <summary>
    /// Mostra o fundo e logo do jogo sequencialmente
    /// </summary>
    private IEnumerator ShowGameElements()
    {
        currentState = TitleScreenState.GameTitle;
        Log("Showing game elements");

        // Mostra o background primeiro
        yield return StartCoroutine(FadeGameObject(background, 1f, backgroundFadeInDuration));

        // Aguarda delay e mostra o logo do jogo
        yield return new WaitForSeconds(delayBeforeGameLogo);
        yield return StartCoroutine(FadeGameObject(gameLogo, 1f, gameLogoFadeInDuration));

        // Inicia a animação do logo se habilitada
        if (enableLogoAnimation && gameLogo != null)
        {
            StartGameLogoAnimation();
        }
    }

    /// <summary>
    /// Mostra os elementos finais do menu
    /// </summary>
    private IEnumerator ShowMenuElements()
    {
        Log("Showing menu elements");

        yield return new WaitForSeconds(delayBeforeMenuElements);

        // Mostra elementos do menu simultaneamente
        StartCoroutine(FadeGameObject(companySmallLogo, 1f, menuElementsFadeInDuration));
        yield return StartCoroutine(FadeGameObject(pressStart, 1f, menuElementsFadeInDuration));
    }

    /// <summary>
    /// Inicia a animação de ping pong do logo do jogo
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
    /// Loop contínuo da animação ping pong do logo usando movimento senoidal suave
    /// </summary>
    private IEnumerator GameLogoAnimationLoop()
    {
        if (gameLogo == null) yield break;

        float time = 0f;
        float frequency = 1f / logoMoveDuration; // Frequência baseada na duração configurada
        float amplitude = (logoMoveUpDistance + logoMoveDownDistance) * 0.5f; // Amplitude média

        while (true)
        {
            time += Time.deltaTime;

            // Usa função senoidal para movimento suave e contínuo
            float yOffset = Mathf.Sin(time * frequency * 2f * Mathf.PI) * amplitude;

            // Aplica o offset à posição original
            Vector3 newPosition = gameLogoOriginalPosition + Vector3.up * yOffset;
            gameLogo.transform.localPosition = newPosition;

            yield return null;
        }
    }

    #endregion

    #region Input Handling

    /// <summary>
    /// Processa input do usuário
    /// </summary>
    private void HandleInput()
    {
        // Bloqueia inputs durante a transição
        if (currentState == TitleScreenState.TransitionStarted)
        {
            Log("Input blocked - transition in progress");
            return;
        }

        if (canSkip && !hasSkipped && currentState != TitleScreenState.MenuReady)
        {
            SkipToMenu();
        }
        else if (currentState == TitleScreenState.MenuReady)
        {
            StartGame();
        }
    }

    /// <summary>
    /// Pula a sequência direto para o menu
    /// </summary>
    private void SkipToMenu()
    {
        hasSkipped = true;
        canSkip = false;
        Log("Skipping to menu");

        StopAllCoroutines();
        StartCoroutine(SkipSequence());
    }

    /// <summary>
    /// Sequência rápida para mostrar o menu quando pulado
    /// </summary>
    private IEnumerator SkipSequence()
    {
        // Para qualquer animação do logo
        StopGameLogoAnimation();

        // Esconde logo da empresa
        SetGameObjectVisibility(companyLogo, false);

        // Mostra elementos do jogo instantaneamente
        SetGameObjectVisibility(background, true);
        SetGameObjectVisibility(gameLogo, true);

        // Inicia a animação do logo se habilitada
        if (enableLogoAnimation && gameLogo != null)
        {
            StartGameLogoAnimation();
        }

        yield return new WaitForSeconds(0.1f);

        // Mostra elementos do menu
        SetGameObjectVisibility(companySmallLogo, true);
        SetGameObjectVisibility(pressStart, true);

        currentState = TitleScreenState.MenuReady;
        InitializeInputUI();

        Log("Skip sequence completed");
    }

    /// <summary>
    /// Inicia o jogo principal usando Easy Transition
    /// </summary>
    private void StartGame()
    {
        // Bloqueia novos inputs durante a transição
        currentState = TitleScreenState.TransitionStarted;
        Log("Starting game with Easy Transition - inputs blocked");
        // Se a cena foi pré-carregada, usa ativação coordenada
        if (GameManager.Instance != null && GameManager.Instance.HasPreloadedScene(gameSceneName))
        {
            TransitionEffect effectToUse = useCustomTransitionEffect ? customTransitionEffect : null;
            if (SceneTransitioner.Instance != null)
            {
                // Usa nova API que realiza fade out, ativa cena pré-carregada e fade in
                SceneTransitioner.Instance.ActivatePreloadedWithTransition(
                    gameSceneName,
                    () => GameManager.Instance.ActivatePreloadedScene(() => { StartCoroutine(UnloadTitleSceneAfterActivation()); }),
                    effectToUse
                );
            }
            else
            {
                Log("SceneTransitioner não encontrado; ativando preload diretamente");
                GameManager.Instance.ActivatePreloadedScene(() => { StartCoroutine(UnloadTitleSceneAfterActivation()); });
            }
        }
        else
        {
            // Caminho original sem preload
            if (SceneTransitioner.Instance != null)
            {
                TransitionEffect effectToUse = useCustomTransitionEffect ? customTransitionEffect : null;
                SceneTransitioner.Instance.LoadScene(gameSceneName, effectToUse);
            }
            else
            {
                Log("SceneTransitioner instance not found, loading scene directly");
                SceneManager.LoadScene(gameSceneName);
            }
        }
    }

    #endregion

    #region Audio Management

    /// <summary>
    /// Inicia a música da tela de título
    /// </summary>
    private void PlayTitleMusic()
    {
        if (titleMusic != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = titleMusic;
            audioSource.loop = true;
            audioSource.Play();

            Log("Title music started");
        }
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Executa efeito de transição (se disponível) e ativa a cena pré-carregada.
    /// </summary>
    // Removido: lógica substituída por SceneTransitioner.ActivatePreloadedWithTransition

    /// <summary>
    /// Descarrega a cena de título após ativação da cena principal.
    /// </summary>
    private IEnumerator UnloadTitleSceneAfterActivation()
    {
        yield return null; // espera um frame para garantir cena ativa definida
        var titleScene = SceneManager.GetActiveScene();
        // Evita descarregar se já mudamos para a cena principal
        if (titleScene.name == gameSceneName)
        {
            Log("Título já não é a cena ativa; nenhuma descarrega necessária");
            yield break;
        }

        Log($"Descarregando cena de título: {titleScene.name}");
        var unloadOp = SceneManager.UnloadSceneAsync(titleScene);
        if (unloadOp != null)
        {
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }
        Log("Cena de título descarregada");
    }

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
    /// Ativa/desativa um input UI de forma segura
    /// </summary>
    private void SetInputUIActive(GameObject inputUI, bool active)
    {
        if (inputUI != null)
        {
            inputUI.SetActive(active);
        }
    }

    /// <summary>
    /// Fade usando CanvasGroup ou fallback manual
    /// </summary>
    private IEnumerator FadeGameObject(GameObject obj, float targetAlpha, float duration)
    {
        if (obj == null) yield break;

        // Verifica se tem CanvasGroup (recomendado para Easy Transition)
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
                color.a = Mathf.Lerp(startAlphas[index++], targetAlpha, progress);
                img.color = color;
            }

            foreach (var text in texts)
            {
                Color color = text.color;
                color.a = Mathf.Lerp(startAlphas[index++], targetAlpha, progress);
                text.color = color;
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
    CompanyLogo,
    Transition,
    GameTitle,
    MenuReady,
    TransitionStarted
}

/// <summary>
/// Tipos de input suportados
/// </summary>
public enum InputType
{
    Keyboard,
    Gamepad,
    Xbox,
    PlayStation,
    Switch
}

#endregion
