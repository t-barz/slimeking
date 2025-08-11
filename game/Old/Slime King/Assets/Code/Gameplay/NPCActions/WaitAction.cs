using UnityEngine;
using System.Collections;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Action that makes an NPC wait for a specified duration at a waypoint.
    /// Can be used to create patrols with pauses at observation points.
    /// </summary>
    [System.Serializable]
    public class WaitAction : NPCAction
    {
        /// <summary>
        /// Duration in seconds that the NPC will wait.
        /// </summary>
        [Tooltip("How many seconds the NPC will wait")]
        public float waitDuration = 2f;

        /// <summary>
        /// If true, the NPC will look around while waiting.
        /// </summary>
        [Tooltip("Should the NPC look around while waiting")]
        public bool lookAround = false;

        /// <summary>
        /// Executes the wait action on the specified NPC.
        /// </summary>
        /// <param name="npc">The NPC that will perform the wait action.</param>
        public override void Execute(NPCController npc)
        {
            npc.StartCoroutine(WaitCoroutine(npc));
        }

        /// <summary>
        /// Coroutine that handles the waiting behavior.
        /// </summary>
        /// <param name="npc">The NPC that's waiting.</param>
        /// <returns>IEnumerator for the coroutine.</returns>
        private IEnumerator WaitCoroutine(NPCController npc)
        {
            npc.enabled = false; // Temporarily disable NPC movement
            
            if (lookAround)
            {
                // Implement look around behavior here
                // Could rotate the NPC back and forth to simulate looking
            }
            
            yield return new WaitForSeconds(waitDuration);
            npc.enabled = true; // Re-enable NPC movement
        }
    }
}
