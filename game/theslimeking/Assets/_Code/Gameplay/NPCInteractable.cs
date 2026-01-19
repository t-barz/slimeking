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
        [SerializeField] private string dialogueTextKey; // Direct localization key (e.g., "DialogueTexts/NPC001")
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
        #endregion

        #region Getters
        public string NpcNameKey => npcNameKey;
        public string DialogueTextKey => dialogueTextKey;
        #endregion
    }
}
