using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Action that triggers an animation on an NPC when it reaches a waypoint.
    /// </summary>
    [System.Serializable]
    public class AnimationAction : NPCAction
    {
        /// <summary>
        /// Name of the animation trigger to activate.
        /// </summary>
        [Tooltip("Animation trigger name in the Animator")]
        public string animationTrigger;
        
        /// <summary>
        /// Whether to wait for the animation to complete before continuing.
        /// </summary>
        [Tooltip("Should the NPC wait for the animation to complete")]
        public bool waitForCompletion = true;
        
        /// <summary>
        /// Approximately how long the animation takes to complete.
        /// Only used if waitForCompletion is true.
        /// </summary>
        [Tooltip("Duration of the animation in seconds")]
        public float animationDuration = 1f;

        /// <summary>
        /// Executes the animation action on the specified NPC.
        /// </summary>
        /// <param name="npc">The NPC that will perform the animation.</param>
        public override void Execute(NPCController npc)
        {
            Animator animator = npc.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger(animationTrigger);
                
                if (waitForCompletion)
                {
                    npc.enabled = false; // Temporarily disable NPC movement
                    npc.StartCoroutine(WaitForAnimation(npc));
                }
            }
            else
            {
                Debug.LogWarning("AnimationAction: No Animator component found on NPC " + npc.name);
            }
        }

        /// <summary>
        /// Coroutine that waits for the animation to complete before continuing.
        /// </summary>
        /// <param name="npc">The NPC performing the animation.</param>
        /// <returns>IEnumerator for the coroutine.</returns>
        private System.Collections.IEnumerator WaitForAnimation(NPCController npc)
        {
            yield return new WaitForSeconds(animationDuration);
            npc.enabled = true; // Re-enable NPC movement
        }
    }
}
