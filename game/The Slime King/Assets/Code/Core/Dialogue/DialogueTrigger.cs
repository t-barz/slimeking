using UnityEngine;
using TheSlimeKing.Gameplay.Interactive;

namespace TheSlimeKing.Core.Dialogue
{
    /// <summary>
    /// Componente para objetos que iniciam diálogos quando interagidos pelo jogador.
    /// Implementa IInteractable para integração com o sistema de objetos interativos.
    /// </summary>
    public class DialogueTrigger : InteractableObject
    {
        [Header("Configuração do Diálogo")]
        [SerializeField] private DialogueData _dialogueData;
        [SerializeField] private bool _showOnce = false;
        [SerializeField] private bool _disableAfterDialogue = false;

        private bool _dialogueInProgress = false;

        /// <summary>
        /// Chamado quando o jogador interage com este objeto.
        /// </summary>
        public override void Interact(GameObject interactor)
        {
            if (_dialogueInProgress) return;

            // Verifica se o diálogo deve ser mostrado apenas uma vez
            if (_showOnce && _dialogueData.HasBeenShown)
            {
                return;
            }

            // Inicia o diálogo
            _dialogueInProgress = true;
            DialogueManager.Instance.StartDialogue(_dialogueData, OnDialogueComplete);

            // Marca o diálogo como já exibido
            if (_showOnce)
            {
                _dialogueData.HasBeenShown = true;
            }
        }

        /// <summary>
        /// Retorna se o objeto pode ser interagido atualmente.
        /// </summary>
        public override bool CanInteract(GameObject interactor)
        {
            // Não pode interagir se um diálogo já estiver em progresso
            if (_dialogueInProgress) return false;

            // Não pode interagir se for para mostrar apenas uma vez e já foi mostrado
            if (_showOnce && _dialogueData.HasBeenShown) return false;

            return base.CanInteract(interactor);
        }

        /// <summary>
        /// Retorna o texto a ser exibido no prompt de interação.
        /// </summary>
        public override string GetInteractionPrompt()
        {
            return "dialog_interact_prompt";
        }

        /// <summary>
        /// Callback executado quando o diálogo é concluído.
        /// </summary>
        private void OnDialogueComplete()
        {
            _dialogueInProgress = false;

            // Desativa o objeto se configurado para tal
            if (_disableAfterDialogue)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
