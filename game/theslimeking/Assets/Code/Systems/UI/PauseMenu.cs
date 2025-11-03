using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Gerencia o menu de pausa do jogo.
    /// Permite acesso ao inventário e outras opções de pausa.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private InventoryUI inventoryUI;

        [Header("Buttons")]
        [SerializeField] private Button inventoryButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;

        [Header("Input")]
        [SerializeField] private InputActionReference pauseAction;
        [SerializeField] private InputActionReference cancelAction;

        private bool isPaused = false;
        private bool isInventoryOpen = false;

        private void Awake()
        {
            // Configura os botões
            if (inventoryButton != null)
            {
                inventoryButton.onClick.AddListener(OpenInventory);
            }

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(Resume);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitToMainMenu);
            }

            // Começa oculto
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }

        private void OnEnable()
        {
            // Habilita as ações de input
            if (pauseAction != null && pauseAction.action != null)
            {
                pauseAction.action.Enable();
                pauseAction.action.performed += OnPausePressed;
            }

            if (cancelAction != null && cancelAction.action != null)
            {
                cancelAction.action.Enable();
                cancelAction.action.performed += OnCancelPressed;
            }
        }

        private void OnDisable()
        {
            // Desabilita as ações de input
            if (pauseAction != null && pauseAction.action != null)
            {
                pauseAction.action.performed -= OnPausePressed;
                pauseAction.action.Disable();
            }

            if (cancelAction != null && cancelAction.action != null)
            {
                cancelAction.action.performed -= OnCancelPressed;
                cancelAction.action.Disable();
            }
        }

        private void Update()
        {
            // Fallback para input legado caso InputSystem não esteja configurado
            if (pauseAction == null || pauseAction.action == null)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
                {
                    TogglePause();
                }
            }

            // Detecta botão de voltar quando inventário está aberto
            if (isInventoryOpen && (cancelAction == null || cancelAction.action == null))
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B))
                {
                    CloseInventoryAndShowPauseMenu();
                }
            }
        }

        /// <summary>
        /// Callback para ação de pausa do Input System.
        /// </summary>
        private void OnPausePressed(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        /// <summary>
        /// Callback para ação de cancelar do Input System.
        /// </summary>
        private void OnCancelPressed(InputAction.CallbackContext context)
        {
            if (isInventoryOpen)
            {
                CloseInventoryAndShowPauseMenu();
            }
            else if (isPaused)
            {
                Resume();
            }
        }

        /// <summary>
        /// Alterna entre pausado e não pausado.
        /// </summary>
        private void TogglePause()
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        /// <summary>
        /// Pausa o jogo e mostra o menu de pausa.
        /// </summary>
        private void Pause()
        {
            isPaused = true;
            isInventoryOpen = false;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }

            Time.timeScale = 0f;
        }

        /// <summary>
        /// Resume o jogo e oculta o menu de pausa.
        /// </summary>
        private void Resume()
        {
            isPaused = false;
            isInventoryOpen = false;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            // Garante que o inventário também está fechado
            if (inventoryUI != null)
            {
                inventoryUI.Hide();
            }

            Time.timeScale = 1f;
        }

        /// <summary>
        /// Abre o inventário a partir do menu de pausa.
        /// </summary>
        private void OpenInventory()
        {
            if (inventoryUI == null)
            {
                Debug.LogWarning("PauseMenu: InventoryUI não está atribuído!");
                return;
            }

            // Oculta o menu de pausa
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }

            // Mostra o inventário
            inventoryUI.Show();
            isInventoryOpen = true;
        }

        /// <summary>
        /// Fecha o inventário e volta ao menu de pausa.
        /// </summary>
        private void CloseInventoryAndShowPauseMenu()
        {
            if (inventoryUI != null)
            {
                inventoryUI.Hide();
            }

            isInventoryOpen = false;

            // Mostra o menu de pausa novamente
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }

            // Mantém o jogo pausado
            Time.timeScale = 0f;
        }

        /// <summary>
        /// Sai para o menu principal.
        /// </summary>
        private void QuitToMainMenu()
        {
            Time.timeScale = 1f;
            // Aqui você pode adicionar lógica para voltar ao menu principal
            // Por exemplo: SceneManager.LoadScene("MainMenu");
            Debug.Log("PauseMenu: Quit to main menu (não implementado)");
        }

        private void OnDestroy()
        {
            // Remove listeners dos botões
            if (inventoryButton != null)
            {
                inventoryButton.onClick.RemoveListener(OpenInventory);
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
    }
}
