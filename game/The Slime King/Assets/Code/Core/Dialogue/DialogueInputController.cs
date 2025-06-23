using UnityEngine;
using UnityEngine.InputSystem;
using TheSlimeKing.Gameplay;

namespace TheSlimeKing.Core.Dialogue
{
    /// <summary>
    /// Gerencia inputs para o sistema de diálogos, permitindo navegação e interação.
    /// </summary>
    public class DialogueInputController : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private DialogueManager _dialogueManager;

        private InputAction _interactAction;
        private PlayerInput _playerInput;
        private bool _inputEnabled = true;

        private void Awake()
        {
            // Busca referências se necessário
            if (_dialogueManager == null)
            {
                _dialogueManager = DialogueManager.Instance;
            }

            // Busca o componente PlayerInput para monitorar ações de input
            _playerInput = GetComponent<PlayerInput>();
            if (_playerInput != null)
            {
                _interactAction = _playerInput.actions["Interact"];
            }
            else
            {
                Debug.LogWarning("PlayerInput não encontrado no DialogueInputController.");
            }
        }

        private void OnEnable()
        {
            if (_dialogueManager != null)
            {
                _dialogueManager.OnDialogueStart += DisablePlayerControls;
                _dialogueManager.OnDialogueEnd += EnablePlayerControls;
            }
        }

        private void OnDisable()
        {
            if (_dialogueManager != null)
            {
                _dialogueManager.OnDialogueStart -= DisablePlayerControls;
                _dialogueManager.OnDialogueEnd -= EnablePlayerControls;
            }
        }

        private void Update()
        {
            // Processa o input somente se um diálogo estiver ativo
            if (_dialogueManager.IsDialogueActive() && _inputEnabled)
            {
                // Verifica a tecla de interação (Interact)
                if (_interactAction != null && _interactAction.triggered)
                {
                    _dialogueManager.ContinueDialogue();
                }
                // Fallback para tecla de espaço se não houver InputAction
                else if (_interactAction == null && Input.GetKeyDown(KeyCode.Space))
                {
                    _dialogueManager.ContinueDialogue();
                }
            }
        }

        /// <summary>
        /// Desabilita os controles do jogador durante diálogos.
        /// </summary>
        private void DisablePlayerControls()
        {
            // Busca pelo controle do jogador e o desativa
            var playerController = FindFirstObjectByType<SlimeInputHandler>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        /// <summary>
        /// Reativa os controles do jogador após o fim dos diálogos.
        /// </summary>
        private void EnablePlayerControls()
        {
            // Busca pelo controle do jogador e o reativa
            var playerController = FindFirstObjectByType<SlimeInputHandler>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }

        /// <summary>
        /// Habilita ou desabilita o input para o sistema de diálogos.
        /// </summary>
        /// <param name="enabled">Se o input deve ser habilitado ou não</param>
        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
        }
    }
}
