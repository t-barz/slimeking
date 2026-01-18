using UnityEngine;
using SlimeKing.Gameplay;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Automatically triggers NPC dialogue when the player collides with the trigger.
    /// Attach to an NPC GameObject with a BoxCollider2D (trigger) and NPCInteractable.
    /// </summary>
    public class NPCTriggerDialogue : MonoBehaviour
    {
        #region Fields
        private NPCInteractable npcInteractable;
        [SerializeField] private string playerTag = "Player";
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            npcInteractable = GetComponent<NPCInteractable>();
            if (npcInteractable == null)
            {
                enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(playerTag) && npcInteractable != null)
            {
                npcInteractable.StartDialogueFromInspector();
            }
        }
        #endregion
    }
}
