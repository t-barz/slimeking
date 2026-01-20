using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.Gameplay;

namespace SlimeKing.UI
{
    /// <summary>
    /// Controla a visibilidade do InventoryPanel e a troca dos action maps Gameplay/UI.
    /// </summary>
    public class InventoryPanelController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private PlayerInput playerInput;
        private InputActionAsset actionAsset;
        private InputAction inventoryAction;
        private InputAction exitAction;
        private Coroutine waitForInputCoroutine;
        private bool actionsSubscribed;
        private bool isInventoryOpen;
        private float previousTimeScale = 1f;
        private bool timeScalePaused;

        private void Awake()
        {
            ResolveInventoryPanelReference();
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            isInventoryOpen = false;
        }

        private void OnEnable()
        {
            if (!SetupInput())
            {
                waitForInputCoroutine = StartCoroutine(WaitForInputReferences());
                return;
            }

            SubscribeToActions();
        }

        private void OnDisable()
        {
            if (waitForInputCoroutine != null)
            {
                StopCoroutine(waitForInputCoroutine);
                waitForInputCoroutine = null;
            }

            UnsubscribeFromActions();

            playerInput?.SwitchCurrentActionMap("Gameplay");
            RestoreTimeScale();

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            isInventoryOpen = false;
        }

        private IEnumerator WaitForInputReferences()
        {
            while (!SetupInput())
            {
                yield return null;
            }

            SubscribeToActions();
            waitForInputCoroutine = null;
        }

        private bool SetupInput()
        {
            if (playerInput == null)
            {
                playerInput = FindObjectOfType<PlayerInput>();
            }

            if (playerInput == null)
            {
                return false;
            }

            if (actionAsset == null)
            {
                actionAsset = playerInput.actions;
            }

            if (actionAsset == null)
            {
                return false;
            }

            if (inventoryAction == null)
            {
                inventoryAction = actionAsset.FindAction("Gameplay/Inventory", throwIfNotFound: false) ?? actionAsset.FindAction("Inventory", throwIfNotFound: false);
            }

            if (exitAction == null)
            {
                exitAction = actionAsset.FindAction("UI/Exit", throwIfNotFound: false) ?? actionAsset.FindAction("Exit", throwIfNotFound: false);
            }

            return inventoryAction != null && exitAction != null;
        }

        private void SubscribeToActions()
        {
            if (actionsSubscribed)
            {
                return;
            }

            if (inventoryAction != null)
            {
                inventoryAction.performed += OnInventoryPerformed;
            }

            if (exitAction != null)
            {
                exitAction.performed += OnExitPerformed;
            }

            actionsSubscribed = true;
        }

        private void UnsubscribeFromActions()
        {
            if (!actionsSubscribed)
            {
                return;
            }

            if (inventoryAction != null)
            {
                inventoryAction.performed -= OnInventoryPerformed;
            }

            if (exitAction != null)
            {
                exitAction.performed -= OnExitPerformed;
            }

            actionsSubscribed = false;
        }

        private void OnInventoryPerformed(InputAction.CallbackContext ctx)
        {
            if (isInventoryOpen)
            {
                return;
            }

            OpenInventory();
        }

        private void OnExitPerformed(InputAction.CallbackContext ctx)
        {
            if (!isInventoryOpen)
            {
                return;
            }

            CloseInventory();
        }

        private void OpenInventory()
        {
            ResolveInventoryPanelReference();
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }
            else
            {
                return;
            }

            isInventoryOpen = true;

            PauseGameTime();
            playerInput?.SwitchCurrentActionMap("UI");
        }

        private void CloseInventory()
        {
            ResolveInventoryPanelReference();
            isInventoryOpen = false;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            RestoreTimeScale();
            playerInput?.SwitchCurrentActionMap("Gameplay");
        }

        private void PauseGameTime()
        {
            if (timeScalePaused)
            {
                return;
            }

            previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
            timeScalePaused = true;
        }

        private void RestoreTimeScale()
        {
            if (!timeScalePaused)
            {
                return;
            }

            Time.timeScale = previousTimeScale;
            timeScalePaused = false;
        }

        private void ResolveInventoryPanelReference()
        {
            if (inventoryPanel != null)
            {
                return;
            }

            Transform panelTransform = transform.Find("InventoryPanel");
            if (panelTransform != null)
            {
                inventoryPanel = panelTransform.gameObject;
                return;
            }

            InventoryPanelView panelView = FindObjectOfType<InventoryPanelView>(true);
            if (panelView != null)
            {
                inventoryPanel = panelView.gameObject;
                return;
            }

            GameObject panelObject = GameObject.Find("InventoryPanel");
            if (panelObject != null)
            {
                inventoryPanel = panelObject;
            }
        }
    }
}
