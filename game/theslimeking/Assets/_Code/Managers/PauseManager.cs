using System;
using System.Collections;
using UnityEngine;
using SlimeKing.Gameplay;
using UnityEngine.InputSystem;
using SlimeKing.UI;

namespace SlimeKing.Core
{
    /// <summary>
    /// Manages game pause state with pause stack support.
    /// Controls Time.timeScale, audio ducking, and action map switching.
    /// 
    /// PAUSE STACK:
    /// Allows multiple systems (DialogueManager, PauseMenu) to pause simultaneously
    /// without conflicts. Game only unpauses when all systems call Resume().
    /// 
    /// AUDIO DUCKING:
    /// Reduces AudioListener volume to 0.2f during pause (instead of muting),
    /// maintaining smooth audio feedback.
    /// </summary>
    public class PauseManager : ManagerSingleton<PauseManager>
    {
        #region Constants
        
        private const int INITIAL_PAUSE_COUNT = 0;
        private const int FIRST_PAUSE_THRESHOLD = 1;
        
        #endregion

        #region Inspector Settings

        [Header("Audio Settings")]
        [Tooltip("AudioListener volume when paused (ducking)")]
        [SerializeField] private float pausedAudioVolume = 0.2f;

        [Tooltip("AudioListener volume when unpaused")]
        [SerializeField] private float resumedAudioVolume = 1.0f;

        [Tooltip("Audio fade duration in seconds")]
        [SerializeField] private float audioFadeDuration = 0.5f;

        [Header("UI References")]
        [Tooltip("Reference to PauseMenu component")]
        [SerializeField] private PauseMenu pauseMenu;

        [Tooltip("Reference to InventoryUI component")]
        [SerializeField] private InventoryUI inventoryUI;

        #endregion

        #region Events

        /// <summary>
        /// Event fired when pause state changes.
        /// Parameter: true if paused, false if unpaused.
        /// </summary>
        public static event Action<bool> OnPauseStateChanged;

        #endregion

        #region Private Fields

        /// <summary>
        /// Pause reference counter.
        /// Increments on pause, decrements on resume.
        /// Game is paused when > 0.
        /// </summary>
        private int pauseRefCount = INITIAL_PAUSE_COUNT;

        /// <summary>
        /// Reference to PlayerController input system.
        /// </summary>
        private InputSystem_Actions inputActions;

        /// <summary>
        /// Active audio fade coroutine.
        /// </summary>
        private Coroutine audioFadeCoroutine;

        /// <summary>
        /// Flag indicating if we've subscribed to inputs.
        /// </summary>
        private bool isInputSubscribed = false;

        /// <summary>
        /// Current navigation state.
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
        /// Returns true if game is paused.
        /// </summary>
        public bool IsPaused => pauseRefCount > INITIAL_PAUSE_COUNT;

        /// <summary>
        /// Returns current pause stack counter value.
        /// </summary>
        public int PauseStackCount => pauseRefCount;

        #endregion

        #region Unity Lifecycle

        protected override void Initialize()
        {
            // Ensure AudioListener is at normal volume
            AudioListener.volume = resumedAudioVolume;

            // Validate UI references
            ValidateUIReferences();

            // Initialize input system
            InitializeInputSystem();
        }

        private void OnEnable()
        {
            // Resubscribe if we had subscribed before
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

            // Ensure game doesn't stay paused when destroying manager
            if (IsPaused)
            {
                ForceUnpause();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Validates UI component references.
        /// </summary>
        private void ValidateUIReferences()
        {
            if (pauseMenu == null)
            {
                pauseMenu = FindObjectOfType<PauseMenu>();
            }

            if (inventoryUI == null)
            {
                inventoryUI = FindObjectOfType<InventoryUI>();
            }
        }

        /// <summary>
        /// Initializes input system connection.
        /// </summary>
        private void InitializeInputSystem()
        {
            if (PlayerController.Instance != null)
            {
                inputActions = PlayerController.Instance.GetInputActions();

                if (inputActions != null)
                {
                    SubscribeToInputs();
                }
               
            }
        }

        #endregion

        #region Input Management

        /// <summary>
        /// Subscribes to input events.
        /// </summary>
        private void SubscribeToInputs()
        {
            if (inputActions == null || isInputSubscribed) return;

            // Gameplay inputs
            inputActions.Gameplay.Pause.performed += OnGameplayMenuInput;
            inputActions.Gameplay.Inventory.performed += OnGameplayInventoryInput;

            // Menu inputs
            inputActions.Menu.Back.performed += OnMenuBackInput;
            inputActions.Menu.Confirm.performed += OnMenuConfirmInput;

            // Inventory Navigation inputs
            inputActions.UI.Cancel.performed += OnUICancelInput;
            inputActions.UI.SelectItem.performed += OnUISubmitInput;

            isInputSubscribed = true;
        }

        /// <summary>
        /// Unsubscribes from input events.
        /// </summary>
        private void UnsubscribeFromInputs()
        {
            if (inputActions == null || !isInputSubscribed) return;

            // Gameplay inputs
            inputActions.Gameplay.Pause.performed -= OnGameplayMenuInput;
            inputActions.Gameplay.Inventory.performed -= OnGameplayInventoryInput;

            // Menu inputs
            inputActions.Menu.Back.performed -= OnMenuBackInput;
            inputActions.Menu.Confirm.performed -= OnMenuConfirmInput;

            // Inventory Navigation inputs
            inputActions.UI.Cancel.performed -= OnUICancelInput;
            inputActions.UI.SelectItem.performed -= OnUISubmitInput;

            isInputSubscribed = false;
        }

        #endregion

        #region Input Callbacks

        private void OnGameplayMenuInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.Gameplay)
            {
                ShowUI(UIType.PauseMenu);
            }
        }

        private void OnGameplayInventoryInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.Gameplay)
            {
                ShowUI(UIType.Inventory);
            }
        }

        private void OnMenuBackInput(InputAction.CallbackContext context)
        {
            if (currentState != NavigationState.Gameplay)
            {
                Cancel();
            }
        }

        private void OnMenuConfirmInput(InputAction.CallbackContext context)
        {
            // Handled by EventSystem/Buttons
        }

        private void OnUICancelInput(InputAction.CallbackContext context)
        {
            if (currentState == NavigationState.Inventory)
            {
                Cancel();
            }
        }

        private void OnUISubmitInput(InputAction.CallbackContext context)
        {
            // Handled by EventSystem and buttons
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses the game. Increments pause stack counter.
        /// If first pause, effectively pauses the game.
        /// </summary>
        public void Pause()
        {
            pauseRefCount++;

            // Only pause effectively on first call
            if (pauseRefCount == FIRST_PAUSE_THRESHOLD)
            {
                ExecutePause();
            }

            // Notify stack change
            PauseEvents.InvokePauseStackChanged(pauseRefCount);
        }

        /// <summary>
        /// Unpauses the game. Decrements pause stack counter.
        /// If counter reaches zero, effectively unpauses the game.
        /// </summary>
        public void Resume()
        {
            if (pauseRefCount <= INITIAL_PAUSE_COUNT)
            {
                return;
            }

            pauseRefCount--;

            // Only unpause effectively when stack reaches zero
            if (pauseRefCount == INITIAL_PAUSE_COUNT)
            {
                ExecuteResume();
            }

            // Notify stack change
            PauseEvents.InvokePauseStackChanged(pauseRefCount);
        }

        /// <summary>
        /// Callback for Inventory button press from PauseMenu.
        /// </summary>
        public void OnInventoryButtonPressed()
        {
            if (currentState == NavigationState.PauseMenu)
            {
                ShowUI(UIType.Inventory);
            }
        }

        /// <summary>
        /// Callback for Resume button press from PauseMenu.
        /// </summary>
        public void OnResumeButtonPressed()
        {
            if (currentState == NavigationState.PauseMenu)
            {
                Cancel();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// UI type enumeration for cleaner transitions.
        /// </summary>
        private enum UIType
        {
            PauseMenu,
            Inventory
        }

        /// <summary>
        /// Shows specified UI and manages state transitions.
        /// </summary>
        private void ShowUI(UIType uiType)
        {
            // Pause game if not already paused
            if (!IsPaused)
            {
                Pause();
            }

            // Hide other UI
            HideAllUI();

            // Show requested UI and update state
            switch (uiType)
            {
                case UIType.PauseMenu:
                    if (pauseMenu != null)
                    {
                        pauseMenu.Show();
                    }
                    currentState = NavigationState.PauseMenu;
                    break;

                case UIType.Inventory:
                    if (inventoryUI != null)
                    {
                        inventoryUI.Show();
                    }
                    currentState = NavigationState.Inventory;
                    break;
            }
        }

        /// <summary>
        /// Hides all UI elements.
        /// </summary>
        private void HideAllUI()
        {
            if (pauseMenu != null && pauseMenu.IsVisible)
            {
                pauseMenu.Hide();
            }

            if (inventoryUI != null && inventoryUI.IsOpen)
            {
                inventoryUI.Hide();
            }
        }

        /// <summary>
        /// Returns to gameplay, hiding all UI.
        /// </summary>
        private void Cancel()
        {
            // Hide inventory with recent-open protection
            if (inventoryUI != null && inventoryUI.IsOpen)
            {
                bool didHide = inventoryUI.Hide();
                if (!didHide)
                {
                    return;
                }
            }

            // Hide pause menu
            if (pauseMenu != null && pauseMenu.IsVisible)
            {
                pauseMenu.Hide();
            }

            // Resume game
            if (IsPaused)
            {
                Resume();
            }

            currentState = NavigationState.Gameplay;
        }

        /// <summary>
        /// Executes pause operations.
        /// </summary>
        private void ExecutePause()
        {

            // Pause game time
            Time.timeScale = 0f;

            // Start audio fade to reduced volume
            StartAudioFade(pausedAudioVolume);

            // Switch action maps: disable Gameplay, enable Menu
            SwitchToMenuInputs();

            // Fire events
            OnPauseStateChanged?.Invoke(true);
            PauseEvents.InvokeGamePaused();

        }

        /// <summary>
        /// Executes resume operations.
        /// </summary>
        private void ExecuteResume()
        {

            // Restore game time
            Time.timeScale = 1f;

            // Start audio fade to normal volume
            StartAudioFade(resumedAudioVolume);

            // Switch action maps: enable Gameplay, disable Menu
            SwitchToGameplayInputs();

            // Fire events
            OnPauseStateChanged?.Invoke(false);
            PauseEvents.InvokeGameResumed();

        }

        /// <summary>
        /// Switches to menu input action maps.
        /// </summary>
        private void SwitchToMenuInputs()
        {
            if (inputActions != null)
            {
                inputActions.Gameplay.Disable();
                inputActions.Menu.Enable();
            }
        }

        /// <summary>
        /// Switches to gameplay input action maps.
        /// </summary>
        private void SwitchToGameplayInputs()
        {
            if (inputActions != null)
            {
                inputActions.Gameplay.Enable();
                inputActions.Menu.Disable();
            }
        }

        /// <summary>
        /// Forces unpause without stack management (for cleanup).
        /// </summary>
        private void ForceUnpause()
        {
            Time.timeScale = 1f;
            AudioListener.volume = resumedAudioVolume;
        }

        /// <summary>
        /// Starts audio fade to target volume.
        /// </summary>
        private void StartAudioFade(float targetVolume)
        {
            // Stop previous fade if exists
            if (audioFadeCoroutine != null)
            {
                StopCoroutine(audioFadeCoroutine);
            }

            audioFadeCoroutine = StartCoroutine(FadeAudioListener(AudioListener.volume, targetVolume, audioFadeDuration));
        }

        /// <summary>
        /// Coroutine that smoothly fades AudioListener volume.
        /// Uses Time.unscaledDeltaTime to work during pause.
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

            // Ensure exact final value
            AudioListener.volume = toVolume;
            audioFadeCoroutine = null;
        }

        #endregion

    }
}