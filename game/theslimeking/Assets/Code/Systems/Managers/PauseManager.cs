using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SlimeKing.Core
{
    /// <summary>
    /// Gerencia o estado de pause do jogo com suporte a pause stack.
    /// Controla Time.timeScale, audio ducking, e switching de action maps.
    /// 
    /// PAUSE STACK:
    /// Permite múltiplos sistemas (DialogueManager, PauseMenu) pausarem simultaneamente
    /// sem conflitos. O jogo só despausa quando todos os sistemas chamarem Resume().
    /// 
    /// AUDIO DUCKING:
    /// Reduz volume do AudioListener para 0.2f durante pause (ao invés de mutar),
    /// mantendo feedback sonoro suave.
    /// </summary>
    public class PauseManager : ManagerSingleton<PauseManager>
    {
        #region Inspector Settings

        [Header("Audio Settings")]
        [Tooltip("Volume do AudioListener quando pausado (ducking)")]
        [SerializeField] private float pausedAudioVolume = 0.2f;

        [Tooltip("Volume do AudioListener quando despausado")]
        [SerializeField] private float resumedAudioVolume = 1.0f;

        [Tooltip("Duração do fade de áudio em segundos")]
        [SerializeField] private float audioFadeDuration = 0.5f;

        #endregion

        #region Events

        /// <summary>
        /// Evento disparado quando o estado de pause muda.
        /// Parâmetro: true se pausado, false se despausado.
        /// </summary>
        public static event Action<bool> OnPauseStateChanged;

        #endregion

        #region Private Fields

        /// <summary>
        /// Contador de referências de pause.
        /// Incrementa ao pausar, decrementa ao despausar.
        /// Jogo está pausado quando > 0.
        /// </summary>
        private int pauseRefCount = 0;

        /// <summary>
        /// Referência ao sistema de input do PlayerController.
        /// </summary>
        private InputSystem_Actions inputActions;

        /// <summary>
        /// Corrotina ativa de fade de áudio.
        /// </summary>
        private Coroutine audioFadeCoroutine;

        /// <summary>
        /// Flag indicando se já subscrevemos ao input de Menu.
        /// </summary>
        private bool isMenuInputSubscribed = false;

        #endregion

        #region Properties

        /// <summary>
        /// Retorna true se o jogo está pausado.
        /// </summary>
        public bool IsPaused => pauseRefCount > 0;

        /// <summary>
        /// Retorna o valor atual do pause stack counter.
        /// </summary>
        public int PauseStackCount => pauseRefCount;

        #endregion

        #region Initialization

        protected override void Initialize()
        {
            Log("PauseManager initialized");

            // Garante que AudioListener está no volume normal
            AudioListener.volume = resumedAudioVolume;

            // Aguarda um frame para garantir que PlayerController está inicializado
            StartCoroutine(InitializeInputDelayed());
        }

        /// <summary>
        /// Inicializa input system após PlayerController estar pronto.
        /// </summary>
        private IEnumerator InitializeInputDelayed()
        {
            yield return null; // Aguarda um frame

            // Tenta obter referência ao input do PlayerController
            if (PlayerController.Instance != null)
            {
                inputActions = PlayerController.Instance.GetInputActions();
                
                if (inputActions != null)
                {
                    SubscribeToMenuInput();
                    Log("Input system connected successfully");
                }
                else
                {
                    LogWarning("Failed to get InputActions from PlayerController");
                }
            }
            else
            {
                LogWarning("PlayerController.Instance not found. Menu input will not work.");
            }
        }

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            // Resubscribe se já tínhamos subscrito antes
            if (inputActions != null && !isMenuInputSubscribed)
            {
                SubscribeToMenuInput();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromMenuInput();
        }

        protected override void OnManagerDestroy()
        {
            UnsubscribeFromMenuInput();
            
            // Garante que o jogo não fica pausado ao destruir o manager
            if (IsPaused)
            {
                Time.timeScale = 1f;
                AudioListener.volume = resumedAudioVolume;
            }
        }

        #endregion

        #region Input Management

        /// <summary>
        /// Subscreve ao evento de Menu.
        /// NOTA: Menu action está atualmente no Gameplay map, mas deve ser movido para UI map.
        /// Após criar o UI action map no InputSystem_Actions.inputactions, atualizar para inputActions.UI.Menu
        /// </summary>
        private void SubscribeToMenuInput()
        {
            if (inputActions == null || isMenuInputSubscribed) return;
            
            // TEMPORÁRIO: Menu está no Gameplay action map
            // TODO: Mover Menu para UI action map no InputSystem_Actions.inputactions
            inputActions.Gameplay.Menu.performed += OnMenuInput;
            isMenuInputSubscribed = true;

            Log("Subscribed to Menu input");
        }

        /// <summary>
        /// Remove subscrição ao evento de Menu.
        /// </summary>
        private void UnsubscribeFromMenuInput()
        {
            if (inputActions == null || !isMenuInputSubscribed) return;

            // TEMPORÁRIO: Menu está no Gameplay action map
            inputActions.Gameplay.Menu.performed -= OnMenuInput;
            isMenuInputSubscribed = false;

            Log("Unsubscribed from Menu input");
        }

        /// <summary>
        /// Callback para input de Menu (Esc/Tab/Start).
        /// </summary>
        private void OnMenuInput(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pausa o jogo. Incrementa o pause stack counter.
        /// Se for a primeira pausa, pausa o jogo efetivamente.
        /// </summary>
        public void Pause()
        {
            pauseRefCount++;

            Log($"Pause() called. Stack count: {pauseRefCount}");

            // Só pausa efetivamente na primeira chamada
            if (pauseRefCount == 1)
            {
                PauseGame();
            }

            // Notifica mudança no stack
            PauseEvents.InvokePauseStackChanged(pauseRefCount);
        }

        /// <summary>
        /// Despausa o jogo. Decrementa o pause stack counter.
        /// Se counter chegar a zero, despausa o jogo efetivamente.
        /// </summary>
        public void Resume()
        {
            if (pauseRefCount <= 0)
            {
                LogWarning("Resume() called but game is not paused");
                return;
            }

            pauseRefCount--;

            Log($"Resume() called. Stack count: {pauseRefCount}");

            // Só despausa efetivamente quando stack chega a zero
            if (pauseRefCount == 0)
            {
                ResumeGame();
            }

            // Notifica mudança no stack
            PauseEvents.InvokePauseStackChanged(pauseRefCount);
        }

        /// <summary>
        /// Alterna entre pausado e despausado.
        /// </summary>
        public void TogglePause()
        {
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Pausa o jogo efetivamente.
        /// </summary>
        private void PauseGame()
        {
            Log("Pausing game");

            // Pausa o tempo do jogo
            Time.timeScale = 0f;

            // Inicia fade de áudio para volume reduzido
            StartAudioFade(pausedAudioVolume);

            // Switch action maps: disable Gameplay, UI já está enabled
            if (inputActions != null)
            {
                inputActions.Gameplay.Disable();
                Log("Gameplay input disabled");
            }

            // Dispara eventos
            OnPauseStateChanged?.Invoke(true);
            PauseEvents.InvokeGamePaused();

            Log("Game paused");
        }

        /// <summary>
        /// Despausa o jogo efetivamente.
        /// </summary>
        private void ResumeGame()
        {
            Log("Resuming game");

            // Restaura o tempo do jogo
            Time.timeScale = 1f;

            // Inicia fade de áudio para volume normal
            StartAudioFade(resumedAudioVolume);

            // Switch action maps: enable Gameplay, mantém UI enabled
            if (inputActions != null)
            {
                inputActions.Gameplay.Enable();
                Log("Gameplay input enabled");
            }

            // Dispara eventos
            OnPauseStateChanged?.Invoke(false);
            PauseEvents.InvokeGameResumed();

            Log("Game resumed");
        }

        /// <summary>
        /// Inicia fade de áudio para o volume alvo.
        /// </summary>
        private void StartAudioFade(float targetVolume)
        {
            // Para fade anterior se existir
            if (audioFadeCoroutine != null)
            {
                StopCoroutine(audioFadeCoroutine);
            }

            audioFadeCoroutine = StartCoroutine(FadeAudioListener(AudioListener.volume, targetVolume, audioFadeDuration));
        }

        /// <summary>
        /// Corrotina que faz fade suave do volume do AudioListener.
        /// Usa Time.unscaledDeltaTime para funcionar durante pause.
        /// </summary>
        private IEnumerator FadeAudioListener(float fromVolume, float toVolume, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                AudioListener.volume = Mathf.Lerp(fromVolume, toVolume, t);
                yield return null;
            }

            // Garante que chegue exatamente no valor final
            AudioListener.volume = toVolume;
            audioFadeCoroutine = null;
        }

        #endregion
    }
}
