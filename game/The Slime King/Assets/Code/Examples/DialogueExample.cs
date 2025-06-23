using UnityEngine;
using TheSlimeKing.Core.Dialogue;
using UnityEngine.UI;
using TMPro;

namespace TheSlimeKing.Core.Examples
{
    /// <summary>
    /// Exemplo de como usar o Sistema de Diálogos
    /// </summary>
    public class DialogueExample : MonoBehaviour
    {
        [Header("Diálogo de Exemplo")]
        [SerializeField] private DialogueData _exampleDialogue;
        [SerializeField] private Button _startDialogueButton;
        [SerializeField] private TextMeshProUGUI _statusText;

        private DialogueManager _dialogueManager;

        private void Start()
        {
            // Obtém a referência para o DialogueManager
            _dialogueManager = DialogueManager.Instance;

            if (_dialogueManager == null)
            {
                Debug.LogError("DialogueManager não encontrado na cena!");
                return;
            }

            // Configura o botão para iniciar o diálogo
            if (_startDialogueButton != null)
            {
                _startDialogueButton.onClick.AddListener(StartExampleDialogue);
            }

            // Registra nos eventos do sistema de diálogo
            _dialogueManager.OnDialogueStart += HandleDialogueStart;
            _dialogueManager.OnDialogueLine += HandleDialogueLine;
            _dialogueManager.OnDialogueEnd += HandleDialogueEnd;

            // Atualiza o texto de status
            UpdateStatus("Clique no botão para iniciar o diálogo de exemplo");
        }

        private void OnDestroy()
        {
            // Remove os registros de eventos para evitar memory leaks
            if (_dialogueManager != null)
            {
                _dialogueManager.OnDialogueStart -= HandleDialogueStart;
                _dialogueManager.OnDialogueLine -= HandleDialogueLine;
                _dialogueManager.OnDialogueEnd -= HandleDialogueEnd;
            }

            // Remove o listener do botão
            if (_startDialogueButton != null)
            {
                _startDialogueButton.onClick.RemoveListener(StartExampleDialogue);
            }
        }

        /// <summary>
        /// Inicia o diálogo de exemplo.
        /// </summary>
        public void StartExampleDialogue()
        {
            if (_dialogueManager != null && _exampleDialogue != null)
            {
                _dialogueManager.StartDialogue(_exampleDialogue, OnDialogueCompleted);
            }
            else
            {
                Debug.LogError("DialogueManager ou diálogo de exemplo não configurado!");
            }
        }

        /// <summary>
        /// Callback para o evento de início do diálogo.
        /// </summary>
        private void HandleDialogueStart()
        {
            UpdateStatus("Diálogo iniciado");
        }

        /// <summary>
        /// Callback para cada linha do diálogo.
        /// </summary>
        private void HandleDialogueLine()
        {
            UpdateStatus("Nova linha de diálogo");
        }

        /// <summary>
        /// Callback para o evento de fim do diálogo.
        /// </summary>
        private void HandleDialogueEnd()
        {
            UpdateStatus("Diálogo finalizado");
        }

        /// <summary>
        /// Callback para quando todo o diálogo é concluído.
        /// </summary>
        private void OnDialogueCompleted()
        {
            UpdateStatus("Diálogo concluído - Pronto para iniciar outro");
        }

        /// <summary>
        /// Atualiza o texto de status.
        /// </summary>
        /// <param name="message">Mensagem a ser exibida</param>
        private void UpdateStatus(string message)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
            }
        }
    }
}
