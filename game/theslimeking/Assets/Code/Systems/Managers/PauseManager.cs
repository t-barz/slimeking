using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.UI;

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
        /// Flag indicando se já subscrevemos aos inputs.
        /// </summary>
        private bool isInputSubscribed = false;

        /// <summary>
        /// Referência ao PauseMenu.
        /// </summary>
        private PauseMenu pauseMenu;

        /// <summary>
        /// Referência ao InventoryUI.
        /// </summary>
        private InventoryUI inventoryUI;

        /// <summary>
        /// Estado atual da navegação.
        /// </summary>
        private enum NavigationState
        {
            Gameplay,
            PauseMenu,
            Inventory
        }

        private NavigationState currentState = NavigationState.Gameplay;

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

            // Encontra referências aos componentes de UI
            pauseMenu = FindObjectOfType<SlimeKing.UI.PauseMenu>();
            inventoryUI = FindObjectOfType<SlimeKing.UI.InventoryUI>();

            if (pauseMenu == null)
            {
                LogWarning("PauseMenu not found in scene!");
            }

            if (inventoryUI == null)
            {
                LogWarning("InventoryUI not found in scene!");
            }

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
                    SubscribeToInputs();
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
            if (inputActions != null && !isInputSubscribed)
            {
                SubscribeToInputs();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromInputs();
        }

        protected override void OnManagerDestroy()
        {
            UnsubscribeFromInputs();

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
        /// Subscreve aos eventos de input.
        /// </summary>
        private void SubscribeToInputs()
        {
            if (inputActions == null || isInputSubscribed) return;

            // Gameplay inputs
            inputActions.Gameplay.PauseGame.performed += OnGameplayMenuInput;
            inputActions.Gameplay.OpenInventory.performed += OnGameplayInventoryInput;

            // Inventory Navigation inputs
            inputActions.InventoryNavigation.Cancel.performed += OnUICancelInput;
            inputActions.InventoryNavigation.SelectItem.performed += OnUISubmitInput;

            isInputSubscribed = true;
            Log("Subscribed to all inputs");
        }

        /// <summary>
        /// Remove subscrição aos eventos de input.
        /// </summary>
        private void UnsubscribeFromInputs()
        {
            if (inputActions == null || !isInputSubscribed) return;

            // Gameplay inputs
            inputActions.Gameplay.PauseGame.performed -= OnGameplayMenuInput;
            inputActions.Gameplay.OpenInventory.performed -= OnGameplayInventoryInput;

            // Inventory Navigation inputs
            inputActions.InventoryNavigation.Cancel.performed -= OnUICancelInput;
            inputActions.InventoryNavigation.SelectItem.performed -= OnUISubmitInput;

            isInputSubscribed = false;
            Log("Unsubscribed from all inputs");
        }

        /// <summary>
        /// Callback para Gameplay.Menu (Start/Esc/Tab).
        /// Gameplay → PauseMenu
        /// </summary>
        private void OnGameplayMenuInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.Gameplay)
            {
                Log("Gameplay → PauseMenu");
                TransitionToPauseMenu();
            }
        }

        /// <summary>
        /// Callback para Gameplay.Inventory (Select/I).
        /// Gameplay → Inventory
        /// </summary>
        private void OnGameplayInventoryInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.Gameplay)
            {
                Log("Gameplay → Inventory");
                TransitionToInventory();
            }
        }

        /// <summary>
        /// Callback para UI.Menu (Start/Esc/Tab).
        /// PauseMenu → Gameplay
        /// </summary>
        private void OnUIMenuInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.PauseMenu)
            {
                Log("PauseMenu → Gameplay (Menu button)");
                TransitionToGameplay();
            }
        }

        /// <summary>
        /// Callback para UI.Inventory (Select/I).
        /// Inventory → Gameplay
        /// </summary>
        private void OnUIInventoryInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.Inventory)
            {
                Log("Inventory → Gameplay (Inventory button)");
                TransitionToGameplay();
            }
        }

        /// <summary>
        /// Callback para UI.Cancel (B/Escape).
        /// PauseMenu → Gameplay
        /// Inventory → PauseMenu
        /// </summary>
        private void OnUICancelInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.PauseMenu)
            {
                Log("PauseMenu → Gameplay (Cancel button)");
                TransitionToGameplay();
            }
            else if (currentState == NavigationState.Inventory)
            {
                Log("Inventory → PauseMenu (Cancel button)");
                TransitionToPauseMenu();
            }
        }

        /// <summary>
        /// Callback para UI.Submit (A/Enter/Space).
        /// Usado para confirmar seleções no menu.
        /// </summary>
        private void OnUISubmitInput(InputAction.CallbackContext context)
        {
            // Submit é tratado pelo EventSystem e pelos botões
            // Não precisa de lógica adicional aqui
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
        /// Callback chamado quando o botão Inventory do PauseMenu é pressionado.
        /// PauseMenu → Inventory
        /// </summary>
        public void OnInventoryButtonPressed()
        {
            if (currentState == NavigationState.PauseMenu)
            {
                Log("PauseMenu → Inventory (button clicked)");
                TransitionToInventory();
            }
        }

        /// <summary>
        /// Callback chamado quando o botão Resume do PauseMenu é pressionado.
        /// PauseMenu → Gameplay
        /// </summary>
        public void OnResumeButtonPressed()
        {
            if (currentState == NavigationState.PauseMenu)
            {
                Log("PauseMenu → Gameplay (Resume button)");
                TransitionToGameplay();
            }
        }

        #endregion

        #region Navigation Transitions

        /// <summary>
        /// Transição: Gameplay → PauseMenu
        /// </summary>
        private void TransitionToPauseMenu()
        {
            // Pausa o jogo se não estava pausado
            if (!IsPaused)
            {
                Pause();
            }

            // Oculta inventário se estiver aberto
            if (inventoryUI != null && inventoryUI.IsOpen)
            {
                inventoryUI.Hide();
            }

            // Mostra o PauseMenu
            if (pauseMenu != null)
            {
                pauseMenu.Show();
            }

            currentState = NavigationState.PauseMenu;
        }

        /// <summary>
        /// Transição: Gameplay → Inventory ou PauseMenu → Inventory
        /// </summary>
        private void TransitionToInventory()
        {
            // Pausa o jogo se não estava pausado
            if (!IsPaused)
            {
                Pause();
            }

            // Oculta o PauseMenu se estiver aberto
            if (pauseMenu != null && pauseMenu.IsVisible)
            {
                pauseMenu.Hide();
            }

            // Mostra o Inventory
            if (inventoryUI != null)
            {
                inventoryUI.Show();
            }

            currentState = NavigationState.Inventory;
        }

        /// <summary>
        /// Transição: PauseMenu → Gameplay ou Inventory → Gameplay
        /// Protegido contra fechar inventário que foi aberto muito recentemente.
        /// </summary>
        private void TransitionToGameplay()
        {
            // Oculta o PauseMenu se estiver aberto
            if (pauseMenu != null && pauseMenu.IsVisible)
            {
                pauseMenu.Hide();
            }

            // Oculta o Inventory se estiver aberto
            // Hide() retorna false se foi aberto muito recentemente, neste caso não transiciona ainda
            if (inventoryUI != null && inventoryUI.IsOpen)
            {
                bool didHide = inventoryUI.Hide();
                if (!didHide)
                {
                    // Inventário foi aberto muito recentemente, aborta a transição
                    Log("TransitionToGameplay aborted: Inventory was just opened");
                    return;
                }
            }

            // Despausa o jogo
            if (IsPaused)
            {
                Resume();
            }

            currentState = NavigationState.Gameplay;
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
