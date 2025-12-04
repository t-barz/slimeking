using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using SlimeKing.Core;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia a interface do inventário.
    /// Exibe 12 slots (3 linhas x 4 colunas) centralizados na tela.
    /// Pode ser aberto via menu pause ou input direto.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        #region Inspector Settings

        [Header("UI Panels")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Input")]
        [SerializeField] private bool canOpenWithInput = true;

        [Header("Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Fields

        private Coroutine fadeCoroutine;
        private InputSystem_Actions inputActions;
        private bool isOpen = false;
        private bool isInputSubscribed = false;

        #endregion

        #region Properties

        public bool IsOpen => isOpen;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Valida referências
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    UnityEngine.Debug.LogError("[InventoryUI] CanvasGroup not found! Adding component.");
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            // Estado inicial: oculto
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void Start()
        {
            // Aguarda um frame para garantir que PlayerController está inicializado
            StartCoroutine(InitializeInputDelayed());
        }

        private void OnEnable()
        {
            // Resubscribe se já tínhamos subscrito antes
            if (inputActions != null && !isInputSubscribed && canOpenWithInput)
            {
                SubscribeToInput();
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromInput();
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            UnsubscribeFromInput();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializa input system após PlayerController estar pronto.
        /// </summary>
        private IEnumerator InitializeInputDelayed()
        {
            yield return null; // Aguarda um frame

            if (!canOpenWithInput) yield break;

            // Tenta obter referência ao input do PlayerController
            if (PlayerController.Instance != null)
            {
                inputActions = PlayerController.Instance.GetInputActions();
                
                if (inputActions != null)
                {
                    SubscribeToInput();
                    LogMessage("Input system connected successfully");
                }
                else
                {
                    LogWarningMessage("Failed to get InputActions from PlayerController");
                }
            }
            else
            {
                LogWarningMessage("PlayerController.Instance not found. Inventory input will not work.");
            }
        }

        #endregion

        #region Input Management

        /// <summary>
        /// Subscreve ao evento de Inventory no InputSystem.
        /// </summary>
        private void SubscribeToInput()
        {
            if (inputActions == null || isInputSubscribed) return;
            
            // Subscreve ao input de Inventory (assumindo que existe no Gameplay action map)
            // NOTA: Se o input "Inventory" não existir, esta parte será ignorada
            try
            {
                // Tenta acessar o input de Inventory
                var inventoryAction = inputActions.Gameplay.Inventory;
                if (inventoryAction != null)
                {
                    inventoryAction.performed += OnInventoryInput;
                    isInputSubscribed = true;
                    LogMessage("Subscribed to Inventory input");
                }
            }
            catch
            {
                LogWarningMessage("Inventory action not found in InputSystem_Actions. Add it manually if needed.");
            }
        }

        /// <summary>
        /// Remove subscrição ao evento de Inventory.
        /// </summary>
        private void UnsubscribeFromInput()
        {
            if (inputActions == null || !isInputSubscribed) return;

            try
            {
                var inventoryAction = inputActions.Gameplay.Inventory;
                if (inventoryAction != null)
                {
                    inventoryAction.performed -= OnInventoryInput;
                    isInputSubscribed = false;
                    LogMessage("Unsubscribed from Inventory input");
                }
            }
            catch
            {
                // Ignora se o input não existir
            }
        }

        /// <summary>
        /// Callback para input de Inventory.
        /// </summary>
        private void OnInventoryInput(InputAction.CallbackContext context)
        {
            ToggleInventory();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Abre o inventário.
        /// </summary>
        public void OpenInventory()
        {
            if (isOpen) return;

            LogMessage("Opening inventory");
            isOpen = true;

            // Oculta o PauseMenu se estiver aberto
            PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu != null)
            {
                pauseMenu.HideMenuForInventory();
            }

            // Pausa o jogo
            if (PauseManager.HasInstance)
            {
                PauseManager.Instance.Pause();
            }

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Fecha o inventário.
        /// </summary>
        public void CloseInventory()
        {
            if (!isOpen) return;

            LogMessage("Closing inventory");
            isOpen = false;

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());

            // Mostra o PauseMenu novamente se o jogo ainda estiver pausado
            if (PauseManager.HasInstance && PauseManager.Instance.IsPaused)
            {
                PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.ShowMenuAfterInventory();
                }
            }
            else if (PauseManager.HasInstance)
            {
                // Despausa o jogo se não estava pausado antes
                PauseManager.Instance.Resume();
            }
        }

        /// <summary>
        /// Alterna entre aberto e fechado.
        /// </summary>
        public void ToggleInventory()
        {
            if (isOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        #endregion

        #region Fade Animations

        /// <summary>
        /// Corrotina de fade in (alpha 0 → 1).
        /// Usa Time.unscaledDeltaTime para funcionar durante pause.
        /// </summary>
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            fadeCoroutine = null;
            LogMessage("Fade in completed");
        }

        /// <summary>
        /// Corrotina de fade out (alpha 1 → 0).
        /// Usa Time.unscaledDeltaTime para funcionar durante pause.
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;

            canvasGroup.interactable = false;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            fadeCoroutine = null;
            LogMessage("Fade out completed");
        }

        #endregion

        #region Slot Interaction

        /// <summary>
        /// Callback quando um slot é clicado.
        /// TODO: Implementar lógica de interação com slots.
        /// </summary>
        public void OnSlotClicked(int slotIndex)
        {
            LogMessage($"Slot {slotIndex} clicked");
            // TODO: Implementar lógica de uso/seleção de item
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[InventoryUI] {message}");
            }
        }

        private void LogWarningMessage(string message)
        {
            UnityEngine.Debug.LogWarning($"[InventoryUI] {message}");
        }

        #endregion
    }
}
