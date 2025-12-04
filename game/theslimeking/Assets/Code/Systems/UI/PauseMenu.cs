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
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;

        [Header("Selection Indicator")]
        [SerializeField] private GameObject selectionArrow;
        [SerializeField] private float arrowOffsetX = -50f;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Fields

        private Coroutine fadeCoroutine;
        private bool isVisible = false;

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

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }

            // Estado inicial: oculto
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            // Oculta seta inicialmente
            if (selectionArrow != null)
            {
                selectionArrow.SetActive(false);
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
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

            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
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

            // Inicia a seta com alpha 0
            CanvasGroup arrowCanvasGroup = null;
            if (selectionArrow != null)
            {
                arrowCanvasGroup = selectionArrow.GetComponent<CanvasGroup>();
                if (arrowCanvasGroup == null)
                {
                    arrowCanvasGroup = selectionArrow.AddComponent<CanvasGroup>();
                }
                arrowCanvasGroup.alpha = 0f;
            }

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

            // Fade da seta após o menu estar visível
            if (arrowCanvasGroup != null)
            {
                elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float t = Mathf.Clamp01(elapsed / fadeDuration);
                    arrowCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                    yield return null;
                }
                arrowCanvasGroup.alpha = 1f;
            }

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

            // Oculta a seta imediatamente
            if (selectionArrow != null)
            {
                CanvasGroup arrowCanvasGroup = selectionArrow.GetComponent<CanvasGroup>();
                if (arrowCanvasGroup != null)
                {
                    arrowCanvasGroup.alpha = 0f;
                }
                selectionArrow.SetActive(false);
            }

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
        /// Seleciona o primeiro botão disponível para navegação.
        /// Facilita controle com gamepad/teclado.
        /// </summary>
        private void SelectFirstButton()
        {
            // Tenta selecionar botões na ordem de prioridade
            Button firstButton = inventoryButton != null && inventoryButton.interactable ? inventoryButton :
                                resumeButton != null && resumeButton.interactable ? resumeButton :
                                quitButton != null && quitButton.interactable ? quitButton : null;

            if (firstButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
                Log($"Selected button: {firstButton.name}");
            }
        }

        /// <summary>
        /// Corrotina que atualiza a posição da seta indicadora.
        /// Roda continuamente enquanto o menu está ativo.
        /// </summary>
        private IEnumerator UpdateSelectionIndicator()
        {
            while (true)
            {
                // Usa unscaledDeltaTime para funcionar durante pause
                yield return new WaitForSecondsRealtime(0.1f);

                if (selectionArrow == null || pauseMenuPanel == null || !pauseMenuPanel.activeSelf)
                {
                    if (selectionArrow != null)
                    {
                        selectionArrow.SetActive(false);
                    }
                    continue;
                }

                // Pega o objeto selecionado pelo EventSystem
                GameObject selectedObj = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;

                if (selectedObj != null)
                {
                    // Verifica se é um dos nossos botões
                    Button selectedButton = selectedObj.GetComponent<Button>();
                    if (selectedButton != null && IsMenuButton(selectedButton) && selectedButton.gameObject.activeInHierarchy)
                    {
                        // Mostra e posiciona a seta
                        selectionArrow.SetActive(true);
                        PositionArrowAtButton(selectedButton);
                    }
                    else
                    {
                        selectionArrow.SetActive(false);
                    }
                }
                else
                {
                    selectionArrow.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Verifica se o botão pertence a este menu.
        /// </summary>
        private bool IsMenuButton(Button button)
        {
            return button == inventoryButton || button == saveButton ||
                   button == loadButton || button == resumeButton || button == quitButton;
        }

        /// <summary>
        /// Posiciona a seta ao lado esquerdo do botão selecionado.
        /// </summary>
        private void PositionArrowAtButton(Button button)
        {
            if (selectionArrow == null || button == null) return;

            RectTransform arrowRect = selectionArrow.GetComponent<RectTransform>();
            RectTransform buttonRect = button.GetComponent<RectTransform>();

            if (arrowRect != null && buttonRect != null)
            {
                // Copia a posição Y do botão, mantém offset X
                Vector2 newPos = new Vector2(
                    buttonRect.anchoredPosition.x + arrowOffsetX,
                    buttonRect.anchoredPosition.y
                );

                arrowRect.anchoredPosition = newPos;
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
        /// Handler do botão Resume.
        /// Notifica o PauseManager.
        /// </summary>
        private void OnResumeButtonClicked()
        {
            Log("Resume button clicked");

            if (PauseManager.HasInstance)
            {
                PauseManager.Instance.OnResumeButtonPressed();
            }
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
