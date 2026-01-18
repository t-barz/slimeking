using UnityEngine;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Enables an NPC to trigger dialogue on player interaction.
    /// Wire the interaction via collision or UI button, then call StartDialogue().
    /// </summary>
    public class NPCInteractable : MonoBehaviour
    {
        #region Fields
        [SerializeField] private string npcNameKey = "NPCs/npc_default"; // Localization key for NPC name (e.g., "NPCs/Tipy")
        [SerializeField] private string dialogueTextKey; // Direct localization key (e.g., "NPCs/NPC001")
        [SerializeField] private string dialogueEntryId; // Optional: if using DialogueDatabase entries
        #endregion

        #region Public Methods
        /// <summary>
        /// Trigger dialogue with localized text key (no database lookup).
        /// </summary>
        public void StartDialogueByTextKey(string textKey)
        {
            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.StartDialogueByTextKey(npcNameKey, textKey);
            }
        }

        /// <summary>
        /// Trigger dialogue using textKey set in inspector.
        /// </summary>
        public void StartDialogueFromInspector()
        {
            if (!string.IsNullOrEmpty(dialogueTextKey))
            {
                StartDialogueByTextKey(dialogueTextKey);
            }
        }

        /// <summary>
        /// Trigger dialogue using database entry (requires DialogueDatabase setup).
        /// </summary>
        public void StartDialogueByEntryId(string entryId)
        {
            if (DialogueManager.HasInstance)
            {
                DialogueManager.Instance.StartDialogue(npcNameKey, entryId);
            }
        }
        #endregion

        #region Getters
        public string NpcNameKey => npcNameKey;
        public string DialogueTextKey => dialogueTextKey;
        public string DialogueEntryId => dialogueEntryId;
        #endregion
    }
}
