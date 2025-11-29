using UnityEngine;
using TheSlimeKing.Gameplay.Interfaces;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace TheSlimeKing.NPCs
{
    /// <summary>
    /// NPC que exibe diálogo localizado ao interagir com o player.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class NPCDialogueController : NPCBaseController, IInteractable
    {
        [Header("Diálogo")]
        [Tooltip("Texto localizado do diálogo principal.")]
        public LocalizedString dialogueText;

        [Header("Configuração")]
        [SerializeField] private bool enableLogs = false;
        [SerializeField] private float interactionRadius = 1.5f;

        private bool _playerInRange = false;
        private GameObject _player;

        private void Reset()
        {
            // Configuração padrão do collider
            var col = GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = true;
                _player = other.gameObject;
                // Exibir prompt de interação (UI)
                // DialogueUIManager.Instance.ShowPrompt(transform.position, GetInteractionPrompt());
                if (enableLogs)
                    UnityEngine.Debug.Log($"[NPCDialogueController] Player entrou no raio de interação.");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                _player = null;
                // Ocultar prompt de interação (UI)
                // DialogueUIManager.Instance.HidePrompt();
                if (enableLogs)
                    UnityEngine.Debug.Log($"[NPCDialogueController] Player saiu do raio de interação.");
            }
        }

        // === IInteractable Implementation ===
        public bool CanInteract(Transform interactor)
        {
            // Só pode interagir se o player estiver no range
            return _playerInRange && _player != null && _player.transform == interactor;
        }

        public bool TryInteract(Transform interactor)
        {
            if (!CanInteract(interactor))
                return false;
            Interact(_player);
            return true;
        }

        public string GetInteractionPrompt()
        {
            // Retorna um prompt padrão ou localizado
            return "Falar"; // Pode ser substituído por texto localizado
        }

        public int GetInteractionPriority()
        {
            // Prioridade padrão para NPCs de diálogo
            return 10;
        }

        public void Interact(GameObject player)
        {
            if (enableLogs)
                UnityEngine.Debug.Log($"[NPCDialogueController] Interagindo com o player.");
            ShowDialogue();
        }

        private void ShowDialogue()
        {
            // Busca o texto localizado e exibe na UI
            if (dialogueText != null)
            {
                dialogueText.StringChanged += OnDialogueTextReady;
                dialogueText.RefreshString();
            }
            else
            {
                UnityEngine.Debug.LogWarning("[NPCDialogueController] dialogueText não configurado!");
            }
        }

        private void OnDialogueTextReady(string localizedText)
        {
            // Exibe o texto na UI de diálogo
            var ui = SlimeKing.UI.DialogueUIManager.Instance;
            if (ui != null)
            {
                ui.ShowDialogue(localizedText);
            }
            else
            {
                UnityEngine.Debug.LogWarning("[NPCDialogueController] DialogueUIManager.Instance não encontrado!");
            }
            UnityEngine.Debug.Log($"[NPCDialogueController] Diálogo: {localizedText}");
            dialogueText.StringChanged -= OnDialogueTextReady;
        }
    }
}