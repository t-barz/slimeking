using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using SlimeKing.Core;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia o menu de pausa do jogo.
    /// Integra com PauseManager para controle de pause via eventos.
    /// Implementa fade animations e navegação com gamepad/teclado.
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
                inventoryButton.onClick.AddListener(OpenInventory);
            }

            if (saveButton != null)
            {
                saveButton.onClick.AddListener(SaveGame);
            }

            if (loadButton != null)
            {
                loadButton.onClick.AddListener(LoadGame);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(Resume);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitToMainMenu);
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
            // Subscreve ao evento de mudança de pause do PauseManager
            PauseManager.OnPauseStateChanged += HandlePauseStateChanged;

            Log("Subscribed to PauseManager events");

            // Inicia corrotina de atualização da seta
            StartCoroutine(UpdateSelectionIndicator());
        }

        private void OnDisable()
        {
            // Desinscreve do evento
            PauseManager.OnPauseStateChanged -= HandlePauseStateChanged;

            Log("Unsubscribed from PauseManager events");

            // Para todas as corrotinas
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            // Remove listeners dos botões
            if (inventoryButton != null)
            {
                inventoryButton.onClick.RemoveListener(OpenInventory);
            }

            if (saveButton != null)
            {
                saveButton.onClick.RemoveListener(SaveGame);
            }

            if (loadButton != null)
            {
                loadButton.onClick.RemoveListener(LoadGame);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveListener(Resume);
            }

            if (quitButton != null)
            {
                quitButton.onClick.RemoveListener(QuitToMainMenu);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handler para mudança de estado de pause.
        /// </summary>
        private void HandlePauseStateChanged(bool isPaused)
        {
            if (isPaused)
            {
                ShowMenu();
            }
            else
            {
                HideMenu();
            }
        }

        #endregion

        #region Menu Control

        /// <summary>
        /// Mostra o menu de pausa com fade in.
        /// </summary>
        private void ShowMenu()
        {
            Log("Showing pause menu");

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
        private void HideMenu()
        {
            Log("Hiding pause menu");

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
                // Usa o mesmo parent para garantir posicionamento correto
                if (arrowRect.parent != buttonRect.parent)
                {
                    Log($"Warning: Arrow and button have different parents!");
                }

                // Copia a posição Y do botão, mantém offset X
                Vector2 newPos = new Vector2(
                    buttonRect.anchoredPosition.x + arrowOffsetX,
                    buttonRect.anchoredPosition.y
                );

                arrowRect.anchoredPosition = newPos;

                if (enableLogs)
                {
                    Log($"Arrow positioned at button '{button.name}': {newPos}");
                }
            }
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Abre o inventário a partir do menu de pausa.
        /// </summary>
        private void OpenInventory()
        {
            Log("Opening inventory - closing pause menu");

            // Apenas retoma do pause menu
            // InventoryUI deve pausar novamente se necessário
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.Resume();
            }
        }

        /// <summary>
        /// Salva o jogo.
        /// TODO: Implementar quando SaveManager estiver pronto.
        /// </summary>
        private void SaveGame()
        {
            UnityEngine.Debug.Log("[PauseMenu] Save game - Not implemented yet");
        }

        /// <summary>
        /// Carrega um save do jogo.
        /// TODO: Implementar quando SaveManager estiver pronto.
        /// </summary>
        private void LoadGame()
        {
            UnityEngine.Debug.Log("[PauseMenu] Load game - Not implemented yet");
        }

        /// <summary>
        /// Resume o jogo através do PauseManager.
        /// </summary>
        private void Resume()
        {
            Log("Resume button pressed");

            if (PauseManager.HasInstance)
            {
                PauseManager.Instance.Resume();
            }
            else
            {
                UnityEngine.Debug.LogError("[PauseMenu] PauseManager.Instance not found!");
            }
        }

        /// <summary>
        /// Sai para o menu principal (cena 1_TitleScreen).
        /// </summary>
        private void QuitToMainMenu()
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
