using UnityEngine;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controls Tipy NPC animator based on dialogue state.
    /// Sets "isTalking" animator parameter when dialogue is active.
    /// </summary>
    public class NPCTipyBehavior : MonoBehaviour
    {
        #region Fields
        private Animator animator;
        private static readonly int IsTalkingHash = Animator.StringToHash("isTalking");
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (animator == null) return;

            bool isTalking = DialogueManager.HasInstance && DialogueManager.Instance.IsDialogueActive;
            animator.SetBool(IsTalkingHash, isTalking);
        }
        #endregion
    }
}
