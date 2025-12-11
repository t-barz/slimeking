using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using SlimeKing.Core;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia o menu de pausa do jogo.
    /// Controlado pelo PauseManager.
    /// 
    /// LOCALIZAÇÃO:
    /// Os textos dos botões devem usar LocalizeStringEvent nos componentes Text/TextMeshProUGUI do Inspector.
    /// Chaves esperadas na tabela UIMenus:
    /// - "pause_inventory" para botão Inventory
    /// - "pause_save" para botão Save
    /// - "pause_load" para botão Load
    /// - "pause_quit" para botão Quit
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        #region Inspector Settings

        [Header("UI Panels")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Buttons")]
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button quitButton;

        [Header("Selection Indicator")]
        [SerializeField] private Color selectedButtonColor = Color.yellow;
        [SerializeField] private Color deselectedButtonColor = Color.white;
        [SerializeField] private float selectedColorAlpha = 0.3f; // Opacidade da cor de seleção (0-1)

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Fields

        private Coroutine fadeCoroutine;
        private bool isVisible = false;
        private Button previouslySelectedButton;

        #endregion

        #region Properties

        public bool IsVisible => isVisible;

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
                    UnityEngine.Debug.LogError("[PauseMenu] CanvasGroup not found! Adding component.");
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            // Configura botões
            if (inventoryButton != null)
            {
                inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            }

            if (saveButton != null)
            {
                saveButton.onClick.AddListener(OnSaveButtonClicked);
            }

            if (loadButton != null)
            {
                loadButton.onClick.AddListener(OnLoadButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }

            // Configura navegação vertical dos botões
            ConfigureButtonNavigation();

            // Estado inicial: oculto
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            previouslySelectedButton = null;
        }

        private void OnEnable()
        {
            // Inicia corrotina de atualização da seta
            StartCoroutine(UpdateSelectionIndicator());
        }

        private void OnDisable()
        {
            // Para todas as corrotinas
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            // Remove listeners dos botões
            if (inventoryButton != null)
            {
                inventoryButton.onClick.RemoveListener(OnInventoryButtonClicked);
            }

            if (saveButton != null)
            {
                saveButton.onClick.RemoveListener(OnSaveButtonClicked);
            }

            if (loadButton != null)
            {
                loadButton.onClick.RemoveListener(OnLoadButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(OnQuitButtonClicked);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Mostra o menu de pausa com fade in.
        /// </summary>
        public void Show()
        {
            if (isVisible) return;

            Log("Showing pause menu");
            isVisible = true;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeIn());
        }

        /// <summary>
        /// Oculta o menu de pausa com fade out.
        /// </summary>
        public void Hide()
        {
            if (!isVisible) return;

            Log("Hiding pause menu");
            isVisible = false;

            // Para fade anterior se existir
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());
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

            // Fade do menu
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;

            // Seleciona o primeiro botão para navegação com gamepad/teclado
            SelectFirstButton();

            // Aguarda alguns frames para garantir que tudo está estável
            yield return null;
            yield return null;

            fadeCoroutine = null;
            Log("Fade in completed");
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

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            fadeCoroutine = null;
            Log("Fade out completed");
        }

        #endregion

        #region UI Navigation

        /// <summary>
        /// Configura a navegação explícita entre os botões do menu.
        /// </summary>
        private void ConfigureButtonNavigation()
        {
            // Ordem vertical: Inventory -> Save -> Load -> Quit
            if (inventoryButton != null && saveButton != null && loadButton != null && quitButton != null)
            {
                // Inventory: cima = Quit, baixo = Save
                SetButtonNavigation(inventoryButton, quitButton, saveButton);

                // Save: cima = Inventory, baixo = Load
                SetButtonNavigation(saveButton, inventoryButton, loadButton);

                // Load: cima = Save, baixo = Quit
                SetButtonNavigation(loadButton, saveButton, quitButton);

                // Quit: cima = Load, baixo = Inventory
                SetButtonNavigation(quitButton, loadButton, inventoryButton);

                Log("Button navigation configured");
            }
        }

        /// <summary>
        /// Define a navegação de um botão específico.
        /// </summary>
        private void SetButtonNavigation(Button button, Button upButton, Button downButton)
        {
            if (button == null) return;

            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnUp = upButton;
            nav.selectOnDown = downButton;
            button.navigation = nav;
        }

        /// <summary>
        /// Seleciona o botão Inventory como padrão para navegação.
        /// Facilita controle com gamepad/teclado.
        /// </summary>
        private void SelectFirstButton()
        {
            // Seleciona Inventory como padrão, depois tenta os outros
            Button firstButton = inventoryButton != null && inventoryButton.interactable ? inventoryButton :
                                saveButton != null && saveButton.interactable ? saveButton :
                                loadButton != null && loadButton.interactable ? loadButton :
                                quitButton != null && quitButton.interactable ? quitButton : null;

            if (firstButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
                Log($"Selected button: {firstButton.name}");
            }
        }

        /// <summary>
        /// Corrotina que atualiza a cor dos botões indicando seleção.
        /// Roda continuamente enquanto o menu está ativo.
        /// </summary>
        private IEnumerator UpdateSelectionIndicator()
        {
            while (true)
            {
                // Usa unscaledDeltaTime para funcionar durante pause
                yield return new WaitForSecondsRealtime(0.05f);

                // Se o menu não está visível, aguarda
                if (pauseMenuPanel == null || !pauseMenuPanel.activeSelf)
                {
                    continue;
                }

                // Pega o objeto selecionado pelo EventSystem
                GameObject selectedObj = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
                Button currentlySelectedButton = null;

                if (selectedObj != null)
                {
                    // Verifica se é um dos nossos botões
                    Button selectedButton = selectedObj.GetComponent<Button>();
                    if (selectedButton != null && IsMenuButton(selectedButton) && selectedButton.gameObject.activeInHierarchy)
                    {
                        currentlySelectedButton = selectedButton;
                    }
                }

                // Atualiza cores dos botões
                UpdateButtonColors(currentlySelectedButton);
            }
        }

        /// <summary>
        /// Verifica se o botão pertence a este menu.
        /// </summary>
        private bool IsMenuButton(Button button)
        {
            return button == inventoryButton || button == saveButton ||
                   button == loadButton || button == quitButton;
        }

        /// <summary>
        /// Atualiza as cores dos botões para indicar seleção.
        /// </summary>
        private void UpdateButtonColors(Button currentlySelectedButton)
        {
            // Atualiza cor de todos os botões
            UpdateButtonColor(inventoryButton, currentlySelectedButton);
            UpdateButtonColor(saveButton, currentlySelectedButton);
            UpdateButtonColor(loadButton, currentlySelectedButton);
            UpdateButtonColor(quitButton, currentlySelectedButton);

            previouslySelectedButton = currentlySelectedButton;
        }

        /// <summary>
        /// Atualiza a cor de um botão específico.
        /// </summary>
        private void UpdateButtonColor(Button button, Button selectedButton)
        {
            if (button == null) return;

            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage == null) return;

            // Define a cor baseado se é o botão selecionado
            if (button == selectedButton)
            {
                Color selected = selectedButtonColor;
                selected.a = selectedColorAlpha;
                buttonImage.color = selected;
            }
            else
            {
                buttonImage.color = deselectedButtonColor;
            }
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Handler do botão Inventory.
        /// Notifica o PauseManager.
        /// </summary>
        private void OnInventoryButtonClicked()
        {
            Log("Inventory button clicked");

            if (PauseManager.HasInstance)
            {
                PauseManager.Instance.OnInventoryButtonPressed();
            }
        }

        /// <summary>
        /// Handler do botão Save.
        /// TODO: Implementar quando SaveManager estiver pronto.
        /// </summary>
        private void OnSaveButtonClicked()
        {
            UnityEngine.Debug.Log("[PauseMenu] Save game - Not implemented yet");
        }

        /// <summary>
        /// Handler do botão Load.
        /// TODO: Implementar quando SaveManager estiver pronto.
        /// </summary>
        private void OnLoadButtonClicked()
        {
            UnityEngine.Debug.Log("[PauseMenu] Load game - Not implemented yet");
        }

        /// <summary>
        /// Handler do botão Quit.
        /// </summary>
        private void OnQuitButtonClicked()
        {
            Log("Quit to main menu");

            // Inicia fade out antes de mudar de cena
            StartCoroutine(QuitToMainMenuCoroutine());
        }

        /// <summary>
        /// Corrotina que faz fade out e então carrega a cena de título.
        /// </summary>
        private IEnumerator QuitToMainMenuCoroutine()
        {
            // Fade out do menu
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = StartCoroutine(FadeOut());

            // Aguarda o fade completar
            yield return new WaitForSecondsRealtime(fadeDuration);

            // Despausa o jogo antes de trocar de cena
            if (PauseManager.HasInstance)
            {
                // Força reset do pause stack
                while (PauseManager.Instance.IsPaused)
                {
                    PauseManager.Instance.Resume();
                }
            }

            // Carrega cena de título com transição
            if (SceneTransitionManager.HasInstance)
            {
                SceneTransitionManager.Instance.LoadSceneWithTransition("1_TitleScreen");
            }
            else
            {
                UnityEngine.Debug.LogError("[PauseMenu] SceneTransitionManager not found!");
            }
        }

        #endregion

        #region Logging

        private void Log(string message)
        {
            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[PauseMenu] {message}");
            }
        }

        #endregion
    }
}
