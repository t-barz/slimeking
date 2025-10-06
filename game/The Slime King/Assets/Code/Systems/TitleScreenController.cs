using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ExtraTools
{
    /// <summary>
    /// TitleScreenController - Controla a sequência de animações da tela inicial.
    /// Sequência: música (1s delay) → centerLogo fade in/out + background fade in → gameTitle fade in → 
    /// ping pong effect starts + pressStart text + inputButton icons fade in simultâneo → wsLogo fade in
    /// 
    /// Skip: Após a música iniciar, qualquer botão pula toda a animação diretamente para o estado final.
    /// </summary>
    public class TitleScreenController : MonoBehaviour
    {
        [Header("Debug / Logging")]
        [SerializeField] private bool verboseLogging = false; // Controla logs verbosos em runtime

        // Wrappers de logging para evitar custo quando desativado
        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private void Log(string message)
        {
            if (verboseLogging) Debug.Log(message);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private void LogWarning(string message)
        {
            if (verboseLogging) Debug.LogWarning(message);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR"), System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private void LogError(string message)
        {
            // Erros normalmente devem aparecer mesmo fora de verbose, mas mantemos controle aqui
            Debug.LogError(message);
        }
        #region UI References
        [Header("UI Elements")]
        [SerializeField] private Image centerLogo;
        [SerializeField] private Image wsLogo;
        [SerializeField] private Image background;
        [SerializeField] private Image gameTitle;
        [SerializeField] private TMPro.TextMeshProUGUI pressStart;
        [SerializeField] private GameObject inputButton;

        [Header("Input Icons")]
        [SerializeField] private GameObject gamepadIcon;
        [SerializeField] private GameObject playstationIcon;
        [SerializeField] private GameObject switchIcon;
        [SerializeField] private GameObject xboxIcon;
        [SerializeField] private GameObject keyboardIcon;
        // Cache de imagens filhas do inputButton para reduzir alocações em fades
        private Image[] _cachedInputButtonImages;
        #endregion

        #region Animation Settings
        [Header("Animation Timings")]
        [SerializeField] private float musicDelay = 1f;
        [SerializeField] private float centerLogoFadeInDuration = 1.5f;
        [SerializeField] private float centerLogoVisibleDuration = 2f;
        [SerializeField] private float centerLogoFadeOutDuration = 1.5f;
        [SerializeField] private float backgroundFadeInDuration = 2f;
        [SerializeField] private float gameTitleFadeInDuration = 1.5f;
        [SerializeField] private float wsLogoFadeInDuration = 1f;
        [SerializeField] private float pressStartFadeInDuration = 1f;

        // Cache de esperas para reduzir alocações GC em corrotinas
        private WaitForSecondsRealtime waitMusicDelay;
        private WaitForSecondsRealtime waitCenterLogoVisible;

        private void Awake()
        {
            // Inicializa caches de WaitForSecondsRealtime
            waitMusicDelay = new WaitForSecondsRealtime(musicDelay);
            waitCenterLogoVisible = new WaitForSecondsRealtime(centerLogoVisibleDuration);
        }

        private void OnValidate()
        {
            // Atualiza caches quando valores mudam no inspector (modo editor)
            if (musicDelay < 0f) musicDelay = 0f;
            if (centerLogoVisibleDuration < 0f) centerLogoVisibleDuration = 0f;

            waitMusicDelay = new WaitForSecondsRealtime(musicDelay);
            waitCenterLogoVisible = new WaitForSecondsRealtime(centerLogoVisibleDuration);
        }

        [Header("Animation Curves")]
        [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        #endregion

        #region Ping Pong Effect
        [Header("Ping Pong Effect")]
        [SerializeField] private float effectRange = 10f;
        [Tooltip("Velocidade do movimento (pixels por segundo)")]
        [SerializeField] private float effectSpeed = 20f;
        [Tooltip("Intervalo de pausa ao trocar de direção (segundos)")]
        [SerializeField] private float directionChangeDelay = 0.5f;
        [Tooltip("Ativa o efeito ping pong no gameTitle")]
        [SerializeField] private bool enablePingPongEffect = true;
        #endregion

        #region Control
        [Header("Control")]
        [SerializeField] private bool autoStart = true;
        [SerializeField] private bool skipOnInput = true;
        #endregion

        #region Post Processing
        [Header("Post Processing")]
        [Tooltip("Volume global usado para animar vinheta no fade de saída. Se vazio, será detectado automaticamente.")]
        [SerializeField] private Volume globalVolume;
        [Tooltip("Intensidade inicial (aberta) da vinheta antes do fade.")]
        [SerializeField] private float vignetteStartIntensity = 0.0f;
        [Tooltip("Intensidade final (fechada) da vinheta no término do fade.")]
        [SerializeField] private float vignetteEndIntensity = 0.55f;
        [Tooltip("Suavização (smoothness) final da vinheta para fechar mais suave.")]
        [SerializeField] private float vignetteEndSmoothness = 0.9f;
        private Vignette _vignette;
        #endregion

        #region Scene Preload
        [Header("Scene Preload")]
        [Tooltip("Nome da próxima cena a ser pré-carregada")]
        [SerializeField] private string nextSceneName = "MainMenu";
        [Tooltip("Duração do fade out antes de ativar a próxima cena")]
        [SerializeField] private float fadeOutDuration = 1f;
        private AsyncOperation preloadOperation;
        private bool preloadStarted = false;
        private bool isTransitioning = false; // Previne múltiplas transições
        #endregion

        private bool sequenceRunning = false;
        private bool sequenceCompleted = false;
        private bool skipAvailable = false; // Controla quando o skip está disponível (após música iniciar)

        // Variáveis para o efeito ping pong
        private Vector2 gameTitleInitialPosition;
        private bool pingPongEffectActive = false;
        private float pingPongDirection = 1f; // 1 para cima, -1 para baixo
        private float currentYOffset = 0f;
        private bool isWaitingDirectionChange = false; // Flag para controlar a pausa
        private float directionChangeTimer = 0f; // Timer para a pausa

        /// <summary>
        /// Evento disparado quando toda a sequência termina
        /// </summary>
        public System.Action OnSequenceCompleted;

        private void Start()
        {
            InitializeElements();
            // Tenta auto detectar volume se não atribuído
            if (globalVolume == null)
            {
#if UNITY_2023_1_OR_NEWER
                globalVolume = FindFirstObjectByType<Volume>();
                if (globalVolume == null)
                    globalVolume = FindAnyObjectByType<Volume>();
#else
                globalVolume = FindObjectOfType<Volume>(); // Versões antigas
#endif
                if (globalVolume != null)
                    Log("[TitleScreen] Volume global detectado automaticamente");
            }

            if (globalVolume != null)
            {
                if (globalVolume.profile != null && globalVolume.profile.TryGet(out _vignette))
                {
                    // Força valores iniciais
                    if (_vignette.intensity.overrideState == false) _vignette.intensity.overrideState = true;
                    if (_vignette.smoothness.overrideState == false) _vignette.smoothness.overrideState = true;
                    _vignette.intensity.value = vignetteStartIntensity;
                }
            }

            // Captura posição inicial do gameTitle para o efeito ping pong
            if (gameTitle != null)
            {
                RectTransform rt = gameTitle.GetComponent<RectTransform>();
                if (rt != null)
                {
                    gameTitleInitialPosition = rt.anchoredPosition;
                }
            }

            // Configura InputManager para contexto de TitleScreen
            if (InputManager.Instance != null)
            {
                Log("[TitleScreen] InputManager encontrado - configurando eventos");
                InputManager.Instance.SetTitleScreenContext();
                InputManager.Instance.OnSkip += HandleSkipInput;
                InputManager.Instance.OnDeviceChanged += HandleDeviceChanged;
                InputManager.Instance.OnAttack += HandleAttackInput;
                Log("[TitleScreen] Eventos OnSkip, OnDeviceChanged e OnAttack conectados com sucesso");

                // Força verificação inicial do dispositivo
                StartCoroutine(CheckInitialDevice());
            }
            else
            {
                LogWarning("[TitleScreen] InputManager.Instance é null - criando InputManager");
                CreateInputManager();
            }

            if (autoStart)
            {
                StartTitleSequence();
            }

            // Pré-carrega a próxima cena ao final do Start
            BeginPreload();
        }

        /// <summary>
        /// Cria o InputManager se ele não existir
        /// </summary>
        private void CreateInputManager()
        {
            GameObject inputManagerGO = new GameObject("InputManager");
            InputManager inputManager = inputManagerGO.AddComponent<InputManager>();
            Log("[TitleScreen] InputManager criado dinamicamente");

            // Aguarda um frame para o Awake ser chamado, então configura
            StartCoroutine(SetupInputManagerDelayed());
        }

        #region Preload Logic
        private void BeginPreload()
        {
            if (preloadStarted)
                return;

            if (string.IsNullOrEmpty(nextSceneName))
            {
                LogWarning("[TitleScreen] nextSceneName vazio - preload não iniciado");
                return;
            }

            preloadStarted = true;
            StartCoroutine(PreloadNextScene());
        }

        private IEnumerator PreloadNextScene()
        {
            Log($"[TitleScreen] Iniciando pré-carregamento da cena '{nextSceneName}'");
            preloadOperation = SceneManager.LoadSceneAsync(nextSceneName);
            if (preloadOperation == null)
            {
                LogError($"[TitleScreen] Falha ao iniciar LoadSceneAsync para '{nextSceneName}'");
                yield break;
            }
            preloadOperation.allowSceneActivation = false;
            while (preloadOperation.progress < 0.9f)
            {
                yield return null;
            }
            Log($"[TitleScreen] Cena '{nextSceneName}' pré-carregada (ready=0.9)");
        }

        /// <summary>
        /// Ativa a cena que foi pré-carregada ou carrega diretamente se não estiver pronta
        /// </summary>
        private void ActivatePreloadedScene()
        {
            if (isTransitioning) return; // Previne múltiplas ativações

            isTransitioning = true;
            Log("[TitleScreen] Iniciando transição com fade out");
            StartCoroutine(FadeOutAndActivateScene());
        }

        /// <summary>
        /// Fade out de todos os elementos e ativa a próxima cena
        /// </summary>
        private IEnumerator FadeOutAndActivateScene()
        {
            Log($"[TitleScreen] Fade out iniciado - duração: {fadeOutDuration}s");

            // Inicia fade out de todos os elementos simultaneamente
            StartCoroutine(FadeImageOut(background, fadeOutDuration));
            StartCoroutine(FadeImageOut(gameTitle, fadeOutDuration));
            StartCoroutine(FadeImageOut(wsLogo, fadeOutDuration));
            StartCoroutine(FadeTextOut(pressStart, fadeOutDuration));
            StartCoroutine(FadeInputButtonOut(fadeOutDuration));

            // Para o efeito ping pong durante o fade
            pingPongEffectActive = false;

            float elapsed = 0f;
            bool canAnimateVignette = _vignette != null;
            float initialIntensity = vignetteStartIntensity;
            float targetIntensity = vignetteEndIntensity;
            float initialSmooth = canAnimateVignette ? _vignette.smoothness.value : 0f;
            float targetSmooth = vignetteEndSmoothness;

            // Loop manual para permitir animar vinheta junto ao fade de UI
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                if (canAnimateVignette)
                {
                    _vignette.intensity.value = Mathf.Lerp(initialIntensity, targetIntensity, t);
                    _vignette.smoothness.value = Mathf.Lerp(initialSmooth, targetSmooth, t);
                }
                yield return null;
            }

            // Garante valores finais
            if (canAnimateVignette)
            {
                _vignette.intensity.value = targetIntensity;
                _vignette.smoothness.value = targetSmooth;
            }

            Log("[TitleScreen] Fade out concluído - ativando próxima cena");

            // Ativa a cena
            if (preloadOperation != null && preloadOperation.progress >= 0.9f)
            {
                Log($"[TitleScreen] Ativando cena pré-carregada '{nextSceneName}'");
                preloadOperation.allowSceneActivation = true;
            }
            else if (!string.IsNullOrEmpty(nextSceneName))
            {
                Log($"[TitleScreen] Preload não pronto, carregando '{nextSceneName}' diretamente");
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                LogError("[TitleScreen] Não foi possível navegar - nextSceneName vazio e preload indisponível");
                isTransitioning = false; // Reset em caso de erro
            }
        }
        #endregion

        /// <summary>
        /// Configura o InputManager após criação com delay
        /// </summary>
        private System.Collections.IEnumerator SetupInputManagerDelayed()
        {
            yield return null; // Espera um frame

            if (InputManager.Instance != null)
            {
                Log("[TitleScreen] Configurando InputManager após criação");
                InputManager.Instance.SetTitleScreenContext();
                InputManager.Instance.OnSkip += HandleSkipInput;
                InputManager.Instance.OnDeviceChanged += HandleDeviceChanged;
                InputManager.Instance.OnAttack += HandleAttackInput;
                Log("[TitleScreen] Eventos OnSkip, OnDeviceChanged e OnAttack conectados com sucesso (delayed)");

                // Força verificação inicial do dispositivo
                yield return CheckInitialDevice();
            }
        }

        /// <summary>
        /// Verifica o dispositivo inicial após um pequeno delay para garantir que o InputManager está totalmente inicializado
        /// </summary>
        private System.Collections.IEnumerator CheckInitialDevice()
        {
            yield return new WaitForSecondsRealtime(0.1f); // Pequeno delay para garantir inicialização

            if (InputManager.Instance != null)
            {
                // Força uma verificação manual do dispositivo atual
                InputDevice currentDevice = null;

                // Verifica teclado
                var keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    currentDevice = keyboard;
                }

                // Verifica gamepad se existe e está ativo
                var gamepad = Gamepad.current;
                if (gamepad != null)
                {
                    currentDevice = gamepad;
                }

                Log($"[TitleScreen] Dispositivo inicial detectado: {currentDevice?.displayName ?? "None"}");
                UpdateInputIcon(currentDevice);
            }
        }

        private void OnDestroy()
        {
            // Remove callback para evitar vazamentos
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnSkip -= HandleSkipInput;
                InputManager.Instance.OnDeviceChanged -= HandleDeviceChanged;
                InputManager.Instance.OnAttack -= HandleAttackInput;
            }
        }

        private void Update()
        {
            if (pingPongEffectActive && enablePingPongEffect)
            {
                UpdatePingPongEffect();
            }
        }

        /// <summary>
        /// Handler para input de skip via InputManager
        /// </summary>
        private void HandleSkipInput()
        {
            Log($"[TitleScreen] HandleSkipInput chamado - skipOnInput:{skipOnInput} skipAvailable:{skipAvailable} sequenceRunning:{sequenceRunning} sequenceCompleted:{sequenceCompleted}");

            if (skipOnInput && skipAvailable && sequenceRunning && !sequenceCompleted)
            {
                Log("[TitleScreen] Skip via InputManager - executando skip");
                SkipToEnd();
            }
            else
            {
                Log("[TitleScreen] Skip bloqueado - condições não atendidas");
            }
        }

        /// <summary>
        /// Handler para mudança de dispositivo via InputManager
        /// </summary>
        private void HandleDeviceChanged(InputDevice device)
        {
            UpdateInputIcon(device);
        }

        /// <summary>
        /// Handler para input de ataque via InputManager - ativa próxima cena quando sequência completa
        /// </summary>
        private void HandleAttackInput(InputAction.CallbackContext context)
        {
            // Aceita tanto started quanto performed para responsividade imediata
            if (!(context.started || context.performed)) return;

            Log($"[TitleScreen] HandleAttackInput chamado - sequenceCompleted:{sequenceCompleted} preloadOperation:{(preloadOperation != null ? "ready" : "null")}");

            if (sequenceCompleted)
            {
                Log("[TitleScreen] Ataque detectado - navegando para próxima cena");
                ActivatePreloadedScene();
            }
            else
            {
                Log("[TitleScreen] Ataque ignorado - sequência ainda não completada");
            }
        }

        /// <summary>
        /// Atualiza o ícone de input baseado no dispositivo detectado
        /// </summary>
        private void UpdateInputIcon(InputDevice device)
        {
            // Esconde todos os ícones primeiro
            SetGameObjectVisibility(gamepadIcon, false);
            SetGameObjectVisibility(playstationIcon, false);
            SetGameObjectVisibility(switchIcon, false);
            SetGameObjectVisibility(xboxIcon, false);
            SetGameObjectVisibility(keyboardIcon, false);

            // Determina qual ícone mostrar baseado no dispositivo
            if (device == null || device is Keyboard)
            {
                SetGameObjectVisibility(keyboardIcon, true);
                Log("[TitleScreen] Ícone alterado para: Teclado");
            }
            else if (device is Gamepad gamepad)
            {
                string deviceName = gamepad.displayName.ToLowerInvariant();

                if (deviceName.Contains("xbox") || deviceName.Contains("xinput"))
                {
                    SetGameObjectVisibility(xboxIcon, true);
                    Log("[TitleScreen] Ícone alterado para: Xbox");
                }
                else if (deviceName.Contains("dualshock") || deviceName.Contains("dualsense") ||
                         deviceName.Contains("playstation") || deviceName.Contains("ps4") || deviceName.Contains("ps5"))
                {
                    SetGameObjectVisibility(playstationIcon, true);
                    Log("[TitleScreen] Ícone alterado para: PlayStation");
                }
                else if (deviceName.Contains("pro controller") || deviceName.Contains("nintendo") ||
                         deviceName.Contains("switch"))
                {
                    SetGameObjectVisibility(switchIcon, true);
                    Log("[TitleScreen] Ícone alterado para: Switch");
                }
                else
                {
                    SetGameObjectVisibility(gamepadIcon, true);
                    Log("[TitleScreen] Ícone alterado para: Gamepad Genérico");
                }
            }
            else
            {
                // Fallback para teclado
                SetGameObjectVisibility(keyboardIcon, true);
                Log("[TitleScreen] Ícone alterado para: Teclado (fallback)");
            }
        }        /// <summary>
                 /// Inicializa todos os elementos como invisíveis
                 /// </summary>
        private void InitializeElements()
        {
            SetImageAlpha(centerLogo, 0f);
            SetImageAlpha(wsLogo, 0f);
            SetImageAlpha(background, 0f);
            SetImageAlpha(gameTitle, 0f);
            SetTextAlpha(pressStart, 0f);
            SetGameObjectVisibility(inputButton, true); // Mantém ativo para permitir fade

            // Inicializa ícones de input - mostra apenas teclado por padrão
            InitializeInputIcons();
            SetInputButtonAlpha(0f); // Mas invisível

            // Reset estados de controle
            sequenceRunning = false;
            sequenceCompleted = false;
            skipAvailable = false;

            Log("[TitleScreen] Elementos inicializados como invisíveis");
        }

        /// <summary>
        /// Inicializa os ícones de input - mostra apenas o teclado por padrão
        /// </summary>
        private void InitializeInputIcons()
        {
            // Busca os ícones automaticamente se não foram atribuídos no inspector
            if (inputButton != null)
            {
                if (gamepadIcon == null) gamepadIcon = inputButton.transform.Find("gamepad")?.gameObject;
                if (playstationIcon == null) playstationIcon = inputButton.transform.Find("playstation")?.gameObject;
                if (switchIcon == null) switchIcon = inputButton.transform.Find("switch")?.gameObject;
                if (xboxIcon == null) xboxIcon = inputButton.transform.Find("xbox")?.gameObject;
                if (keyboardIcon == null) keyboardIcon = inputButton.transform.Find("keyboard")?.gameObject;
                // Cache inicial (inclui inativos)
                _cachedInputButtonImages = inputButton.GetComponentsInChildren<Image>(includeInactive: true);
            }

            // Esconde todos os ícones
            SetGameObjectVisibility(gamepadIcon, false);
            SetGameObjectVisibility(playstationIcon, false);
            SetGameObjectVisibility(switchIcon, false);
            SetGameObjectVisibility(xboxIcon, false);
            SetGameObjectVisibility(keyboardIcon, false);

            // Mostra apenas o teclado por padrão
            SetGameObjectVisibility(keyboardIcon, true);

            Log("[TitleScreen] Ícones de input inicializados - mostrando teclado por padrão");
        }

        /// <summary>
        /// Inicia a sequência completa da tela de título
        /// </summary>
        public void StartTitleSequence()
        {
            if (sequenceRunning) return;

            StartCoroutine(TitleSequenceCoroutine());
        }

        /// <summary>
        /// Pula para o final da sequência (todos os elementos visíveis)
        /// </summary>
        public void SkipToEnd()
        {
            if (sequenceCompleted) return;

            StopAllCoroutines();

            // Garante que a música toque mesmo no skip
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuMusic(crossfade: true);
                Log("[TitleScreen] Música iniciada (via skip)");
            }

            SetImageAlpha(centerLogo, 0f);    // Logo central some
            SetImageAlpha(background, 1f);     // Background visível
            SetImageAlpha(gameTitle, 1f);      // Título visível
            SetImageAlpha(wsLogo, 1f);         // WS Logo visível
            SetTextAlpha(pressStart, 1f);      // Press Start visível
            SetInputButtonAlpha(1f);           // Input Button visível

            sequenceRunning = false;
            sequenceCompleted = true;
            skipAvailable = false; // Desabilita skip após pular

            // Inicia efeito ping pong também no skip
            StartPingPongEffect();

            Log("[TitleScreen] Sequência pulada - todos elementos finais visíveis");

            // Habilita input de ataque após o skip
            EnableAttackInput("após skip");

            OnSequenceCompleted?.Invoke();
        }

        /// <summary>
        /// Sequência principal: música → centerLogo → background + fadeOut centerLogo → gameTitle → wsLogo
        /// </summary>
        private IEnumerator TitleSequenceCoroutine()
        {
            sequenceRunning = true;
            sequenceCompleted = false;
            skipAvailable = false; // Skip não disponível no início

            Log("[TitleScreen] Iniciando sequência da tela de título");

            // Fase 1: Aguarda delay e inicia música
            yield return waitMusicDelay;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuMusic(crossfade: true);
                Log("[TitleScreen] Música iniciada");
            }

            // Habilita skip logo após a música começar
            skipAvailable = true;
            Log("[TitleScreen] Skip disponível - pressione qualquer botão para pular animação");

            // Fase 2: Center Logo fade in
            yield return StartCoroutine(FadeImageIn(centerLogo, centerLogoFadeInDuration));

            // Fase 3: Center Logo fica visível
            yield return waitCenterLogoVisible;

            // Fase 4: Background fade in + Center Logo fade out (simultâneo)
            StartCoroutine(FadeImageOut(centerLogo, centerLogoFadeOutDuration));
            yield return StartCoroutine(FadeImageIn(background, backgroundFadeInDuration));

            // Fase 5: Game Title fade in (quando background estiver totalmente visível)
            yield return StartCoroutine(FadeImageIn(gameTitle, gameTitleFadeInDuration));

            // Inicia efeito ping pong após gameTitle estar totalmente visível
            StartPingPongEffect();

            // Fase 5.5: Press Start e inputButton fade in (simultaneamente com o início do ping pong)
            StartCoroutine(FadePressStartIn(pressStartFadeInDuration));

            // Fase 6: WS Logo fade in (quando gameTitle estiver totalmente visível)
            yield return StartCoroutine(FadeImageIn(wsLogo, wsLogoFadeInDuration));

            // Sequência concluída
            sequenceRunning = false;
            sequenceCompleted = true;
            skipAvailable = false; // Desabilita skip após conclusão natural

            Log("[TitleScreen] Sequência de animação concluída");

            // Habilita input de ataque após sequência natural
            EnableAttackInput("após sequência - pressione Z para continuar");

            OnSequenceCompleted?.Invoke();
        }

        /// <summary>
        /// Centraliza habilitação do gameplay input (ataque) com logging
        /// </summary>
        private void EnableAttackInput(string contexto)
        {
            if (InputManager.Instance == null) return;
            InputManager.Instance.EnableGameplay();
            Log($"[TitleScreen] Input de ataque habilitado {contexto}");
        }

        /// <summary>
        /// Fade in de uma imagem
        /// </summary>
        private IEnumerator FadeImageIn(Image image, float duration)
        {
            if (image == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeInCurve.Evaluate(progress);

                SetImageAlpha(image, alpha);
                yield return null;
            }

            SetImageAlpha(image, 1f);
            Log($"[TitleScreen] {image.name} fade in concluído");
        }

        /// <summary>
        /// Fade in de um texto TextMeshPro
        /// </summary>
        private IEnumerator FadeTextIn(TMPro.TextMeshProUGUI text, float duration)
        {
            if (text == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeInCurve.Evaluate(progress);

                SetTextAlpha(text, alpha);
                yield return null;
            }

            SetTextAlpha(text, 1f);
            Log($"[TitleScreen] {text.name} fade in concluído");
        }

        /// <summary>
        /// Fade in do pressStart com seu inputButton
        /// </summary>
        private IEnumerator FadePressStartIn(float duration)
        {
            if (pressStart == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeInCurve.Evaluate(progress);

                // Aplica fade tanto no texto quanto no inputButton
                SetTextAlpha(pressStart, alpha);
                SetInputButtonAlpha(alpha);
                yield return null;
            }

            // Finaliza com alpha total
            SetTextAlpha(pressStart, 1f);
            SetInputButtonAlpha(1f);
            Log("[TitleScreen] pressStart e inputButton fade in concluído");
        }

        /// <summary>
        /// Fade out de uma imagem
        /// </summary>
        private IEnumerator FadeImageOut(Image image, float duration)
        {
            if (image == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeOutCurve.Evaluate(progress);

                SetImageAlpha(image, alpha);
                yield return null;
            }

            SetImageAlpha(image, 0f);
            Log($"[TitleScreen] {image.name} fade out concluído");
        }

        /// <summary>
        /// Fade out de um texto TextMeshPro
        /// </summary>
        private IEnumerator FadeTextOut(TMPro.TextMeshProUGUI text, float duration)
        {
            if (text == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeOutCurve.Evaluate(progress);

                SetTextAlpha(text, alpha);
                yield return null;
            }

            SetTextAlpha(text, 0f);
            Log($"[TitleScreen] {text.name} fade out concluído");
        }

        /// <summary>
        /// Fade out do inputButton
        /// </summary>
        private IEnumerator FadeInputButtonOut(float duration)
        {
            if (inputButton == null) yield break;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float alpha = fadeOutCurve.Evaluate(progress);

                SetInputButtonAlpha(alpha);
                yield return null;
            }

            SetInputButtonAlpha(0f);
            Log("[TitleScreen] inputButton fade out concluído");
        }

        /// <summary>
        /// Define alpha de uma imagem
        /// </summary>
        private void SetImageAlpha(Image image, float alpha)
        {
            if (image == null) return;

            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        /// <summary>
        /// Define alpha de todos os ícones dentro do inputButton
        /// </summary>
        private void SetInputButtonAlpha(float alpha)
        {
            if (inputButton == null) return;
            if (_cachedInputButtonImages == null || _cachedInputButtonImages.Length == 0)
            {
                _cachedInputButtonImages = inputButton.GetComponentsInChildren<Image>(includeInactive: true);
            }
            foreach (var img in _cachedInputButtonImages)
            {
                if (img == null) continue;
                var c = img.color; c.a = alpha; img.color = c;
            }
        }        /// <summary>
                 /// Define alpha de um texto TextMeshPro
                 /// </summary>
        private void SetTextAlpha(TMPro.TextMeshProUGUI text, float alpha)
        {
            if (text == null) return;

            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }

        /// <summary>
        /// Define visibilidade de um GameObject
        /// </summary>
        private void SetGameObjectVisibility(GameObject obj, bool visible)
        {
            if (obj == null) return;
            if (obj.activeSelf == visible) return; // Evita chamada redundante
            obj.SetActive(visible);
        }

        /// <summary>
        /// Atualiza o efeito ping pong do gameTitle com intervalo na troca de direção
        /// Move de inicial para +range, pausa, depois para -range, pausa, e repete
        /// </summary>
        private void UpdatePingPongEffect()
        {
            if (gameTitle == null) return;

            RectTransform rt = gameTitle.GetComponent<RectTransform>();
            if (rt == null) return;

            // Se está esperando troca de direção, conta o timer
            if (isWaitingDirectionChange)
            {
                directionChangeTimer += Time.unscaledDeltaTime;
                if (directionChangeTimer >= directionChangeDelay)
                {
                    // Fim da pausa, inverte direção
                    pingPongDirection *= -1f;
                    isWaitingDirectionChange = false;
                    directionChangeTimer = 0f;
                }
                return; // Não move durante a pausa
            }

            // Calcula movimento baseado na velocidade
            float deltaY = effectSpeed * Time.unscaledDeltaTime * pingPongDirection;
            currentYOffset += deltaY;

            // Verifica limites e inicia pausa para trocar direção
            if (pingPongDirection > 0 && currentYOffset >= effectRange)
            {
                currentYOffset = effectRange;
                isWaitingDirectionChange = true;
                directionChangeTimer = 0f;
            }
            else if (pingPongDirection < 0 && currentYOffset <= -effectRange)
            {
                currentYOffset = -effectRange;
                isWaitingDirectionChange = true;
                directionChangeTimer = 0f;
            }

            // Aplica nova posição
            Vector2 newPosition = gameTitleInitialPosition + new Vector2(0f, currentYOffset);
            rt.anchoredPosition = newPosition;
        }

        /// <summary>
        /// Inicia o efeito ping pong do gameTitle
        /// </summary>
        private void StartPingPongEffect()
        {
            if (enablePingPongEffect && gameTitle != null)
            {
                pingPongEffectActive = true;
                currentYOffset = 0f;
                pingPongDirection = 1f; // Começa subindo
                isWaitingDirectionChange = false;
                directionChangeTimer = 0f;
                Log("[TitleScreen] Efeito ping pong iniciado");
            }
        }


        #region Debug Helpers
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        [UnityEngine.ContextMenu("Test Music")]
        private void TestMusic()
        {
            if (Application.isPlaying)
            {
                Debug.Log("[TitleScreen] Testando música...");
                if (AudioManager.Instance != null)
                {
                    Debug.Log("[TitleScreen] AudioManager encontrado, tentando tocar música");
                    AudioManager.Instance.PlayMenuMusic(crossfade: false);
                }
                else
                {
                    Debug.LogError("[TitleScreen] AudioManager.Instance é null!");
                }
            }
        }

        [UnityEngine.ContextMenu("Test Title Sequence")]
        private void TestTitleSequence()
        {
            if (Application.isPlaying)
            {
                InitializeElements();
                StartTitleSequence();
            }
        }

        [UnityEngine.ContextMenu("Skip to End")]
        private void TestSkipToEnd()
        {
            if (Application.isPlaying)
                SkipToEnd();
        }

        [UnityEngine.ContextMenu("Reset Elements")]
        private void TestResetElements()
        {
            if (Application.isPlaying)
                InitializeElements();
        }
#endif
        #endregion
    }
}