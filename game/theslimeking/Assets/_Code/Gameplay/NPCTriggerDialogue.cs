using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Triggers NPC dialogue based on configurable behavior mode.
    /// Closes dialogue if the player leaves the trigger area.
    /// Attach to an NPC GameObject with a BoxCollider2D (trigger) and NPCInteractable.
    /// </summary>
    public class NPCTriggerDialogue : MonoBehaviour
    {
        #region Enums
        public enum DialogueStartMode
        {
            AutoStart,
            InteractToStart
        }
        #endregion

        #region Fields
        [Header("Dialogue Settings")]
        [SerializeField] private DialogueStartMode startMode = DialogueStartMode.AutoStart;
        [SerializeField] private string playerTag = "Player";

        private NPCInteractable npcInteractable;
        private InputSystem_Actions inputActions;
        private bool playerInTrigger;
        private bool dialogueWasActive;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void Start()
        {
            npcInteractable = GetComponent<NPCInteractable>();
            if (npcInteractable == null)
            {
                enabled = false;
            }
        }

        private void OnEnable()
        {
            inputActions.Gameplay.Enable();
            inputActions.Gameplay.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            inputActions.Gameplay.Interact.performed -= OnInteractPerformed;
            inputActions.Gameplay.Disable();
        }

        private void OnDestroy()
        {
            inputActions?.Dispose();
        }

        private void LateUpdate()
        {
            dialogueWasActive = DialogueManager.HasInstance && DialogueManager.Instance.IsDialogueActive;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag(playerTag) || npcInteractable == null) return;

            playerInTrigger = true;

            if (startMode == DialogueStartMode.AutoStart)
            {
                npcInteractable.StartDialogueFromInspector();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag(playerTag)) return;

            playerInTrigger = false;

            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.CloseDialogue();
            }
        }
        #endregion

        #region Input Handlers
        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (startMode != DialogueStartMode.InteractToStart) return;
            if (!playerInTrigger || npcInteractable == null) return;
            
            // Prevent restarting dialogue on the same frame it was closed
            if (dialogueWasActive) return;
            if (DialogueManager.HasInstance && DialogueManager.Instance.IsDialogueActive) return;

            npcInteractable.StartDialogueFromInspector();
        }
        #endregion
    }
}
