using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controls NPC movement and behavior, including patrol, path following, and player interaction.
    /// NPCs can be classified as Enemy, Neutral, or Ally, and react to the player based on their type and field of view.
    /// - Enemy: Attacks the player when in sight.
    /// - Neutral: Ignores the player unless attacked.
    /// - Ally: Approaches the player when in sight.
    /// NPCs can follow a set path, perform actions at each point, and patrol between points.
    /// The NPC only reacts to the player if the player is within its field of view and not hiding.
    /// </summary>
    public enum NPCType
    {
        Enemy,
        Neutral,
        Ally
    }

    public class NPCController : MonoBehaviour
    {
        /// <summary>
        /// Defines the NPC type: Enemy, Neutral, or Ally.
        /// </summary>
        [Header("General Settings")]
        public NPCType npcType;

        /// <summary>
        /// Movement speed of the NPC.
        /// </summary>
        public float speed = 2f;

        /// <summary>
        /// Maximum distance the NPC can see the player.
        /// </summary>
        public float visionRange = 5f;

        /// <summary>
        /// Field of view angle (in degrees) for player detection.
        /// </summary>
        public float visionAngle = 90f;

        /// <summary>
        /// Points that define the NPC's movement path.
        /// </summary>
        public Transform[] pathPoints;

        /// <summary>
        /// If true, the NPC will patrol back and forth between path points.
        /// </summary>
        public bool patrolMode = false;

        /// <summary>
        /// Layer mask used to detect the player.
        /// </summary>
        public LayerMask playerMask;

        /// <summary>
        /// Layer mask used to detect obstacles that block vision.
        /// </summary>
        public LayerMask obstacleMask;

        [Header("Actions per point")]
        /// <summary>
        /// List of actions to execute at each path point.
        /// </summary>
        public List<NPCAction> actionsPerPoint = new List<NPCAction>();

        // Internal state
        private int currentPointIndex = 0;
        private Transform player;
        private bool movingForward = true;
        private bool playerInSight = false;
        private PlayerMovement playerMovement;

        /// <summary>
        /// Initializes NPC by finding and referencing the player.
        /// </summary>
        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
                playerMovement = player.GetComponent<PlayerMovement>();
        }

        /// <summary>
        /// Updates NPC movement and behavior every frame.
        /// Handles path following, patrol movement, and player detection.
        /// </summary>
        void Update()
        {
            if (patrolMode && pathPoints.Length > 1)
                Patrol();
            else if (pathPoints.Length > 0)
                FollowPath();

            CheckPlayer();
            ReactToPlayer();
        }

        /// <summary>
        /// Moves the NPC along a circular path, following waypoints in sequence.
        /// When reaching the last point, it loops back to the first one.
        /// </summary>
        void FollowPath()
        {
            if (pathPoints.Length == 0) return;
            Transform target = pathPoints[currentPointIndex];
            MoveTo(target.position);
            if (Vector2.Distance(transform.position, target.position) < 0.2f)
            {
                ExecuteActionAtPoint(currentPointIndex);
                currentPointIndex = (currentPointIndex + 1) % pathPoints.Length;
            }
        }

        /// <summary>
        /// Moves the NPC back and forth between path points.
        /// Changes direction when reaching the first or last point.
        /// </summary>
        void Patrol()
        {
            if (pathPoints.Length == 0) return;
            Transform target = pathPoints[currentPointIndex];
            MoveTo(target.position);
            if (Vector2.Distance(transform.position, target.position) < 0.2f)
            {
                ExecuteActionAtPoint(currentPointIndex);
                if (movingForward)
                {
                    if (currentPointIndex < pathPoints.Length - 1)
                        currentPointIndex++;
                    else
                    {
                        movingForward = false;
                        currentPointIndex--;
                    }
                }
                else
                {
                    if (currentPointIndex > 0)
                        currentPointIndex--;
                    else
                    {
                        movingForward = true;
                        currentPointIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Moves the NPC towards a target position at the defined speed.
        /// </summary>
        /// <param name="destination">Target position to move towards.</param>
        void MoveTo(Vector2 destination)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }

        /// <summary>
        /// Executes a custom action when the NPC reaches a specific point.
        /// </summary>
        /// <param name="index">Index of the point in the path where the action should be executed.</param>
        void ExecuteActionAtPoint(int index)
        {
            if (index < actionsPerPoint.Count && actionsPerPoint[index] != null)
                actionsPerPoint[index].Execute(this);
        }

        /// <summary>
        /// Checks if the player is within the NPC's field of view.
        /// Uses raycasting to detect if the player is visible and not hiding behind obstacles.
        /// Takes into account vision range, angle, and whether the player is hiding.
        /// </summary>
        void CheckPlayer()
        {
            playerInSight = false;
            if (player == null) return;
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, player.position);
            float angle = Vector2.Angle(transform.right, directionToPlayer);
            if (distance <= visionRange && angle <= visionAngle / 2f)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distance, playerMask | obstacleMask);
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Player"))
                {
                    // Check if player is hiding
                    if (playerMovement != null && playerMovement.IsHiding)
                        return;
                    playerInSight = true;
                }
            }
        }

        /// <summary>
        /// Determines the NPC's reaction to the player based on its type.
        /// Only reacts if the player is in sight and not hiding.
        /// </summary>
        void ReactToPlayer()
        {
            if (!playerInSight) return;
            switch (npcType)
            {
                case NPCType.Enemy:
                    AttackPlayer();
                    break;
                case NPCType.Neutral:
                    // Only attack if attacked (implement aggression logic)
                    break;
                case NPCType.Ally:
                    ApproachPlayer();
                    break;
            }
        }

        /// <summary>
        /// Implements attack behavior for enemy NPCs.
        /// Called when an enemy NPC detects the player.
        /// </summary>
        void AttackPlayer()
        {
            // Logic to attack the player
        }

        /// <summary>
        /// Implements approach behavior for ally NPCs.
        /// Called when an ally NPC detects the player.
        /// </summary>
        void ApproachPlayer()
        {
            if (player != null)
                MoveTo(player.position);
        }
    }

    /// <summary>
    /// Represents a custom action that an NPC can perform at specific points in its path.
    /// Can be extended to create various behaviors such as waiting, playing animations, 
    /// changing direction, spawning objects, etc.
    /// </summary>
    [System.Serializable]
    public class NPCAction
    {
        /// <summary>
        /// Descriptive name of the action for the Unity Inspector.
        /// </summary>
        public string actionName;

        /// <summary>
        /// Executes the action for the specified NPC.
        /// Override this method in derived classes to implement specific behaviors.
        /// </summary>
        /// <param name="npc">The NPC controller that will perform the action.</param>
        public virtual void Execute(NPCController npc)
        {
            // Implement custom actions per point
        }
    }
}
