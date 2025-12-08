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
        [SerializeField] private GameObject selectionArrowPrefab;
        [SerializeField] private Transform arrowParent;
        [SerializeField] private float arrowOffsetX = -50f;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Fields

        private Coroutine fadeCoroutine;
        private bool isVisible = false;
        private bool allowArrowUpdate = false;
        private GameObject selectionArrowInstance;

        #endregion

        #region Properties

        public bool IsVisible => isVisible;
        private GameObject selectionArrow => selectionArrowInstance;

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

            // Valida parent da seta
            if (arrowParent == null)
            {
                arrowParent = pauseMenuPanel != null ? pauseMenuPanel.transform : transform;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            allowArrowUpdate = false;
            selectionArrowInstance = null;
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
            allowArrowUpdate = false; // Desabilita atualização da seta durante o fade

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
            allowArrowUpdate = false; // Desabilita atualização da seta durante o fade

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

            // Instancia a seta APÓS o menu estar completamente visível
            if (selectionArrowPrefab != null && selectionArrowInstance == null)
            {
                selectionArrowInstance = Instantiate(selectionArrowPrefab, arrowParent);
                Log("Selection arrow instantiated");
            }

            // Habilita atualização da seta - ela aparecerá já na posição correta
            allowArrowUpdate = true;

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

            // Destrói a seta imediatamente
            if (selectionArrowInstance != null)
            {
                Destroy(selectionArrowInstance);
                selectionArrowInstance = null;
                Log("Selection arrow destroyed");
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
                yield return new WaitForSecondsRealtime(0.05f);

                // Só atualiza a seta se o fade estiver completo
                if (!allowArrowUpdate || selectionArrow == null || pauseMenuPanel == null || !pauseMenuPanel.activeSelf)
                {
                    // Não desativa a seta aqui se allowArrowUpdate for false
                    // Isso evita que ela pisque durante transições
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
                        if (!selectionArrow.activeSelf)
                        {
                            selectionArrow.SetActive(true);
                        }
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
