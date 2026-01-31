using System.Collections;
using UnityEngine;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Controls the behavior of the BeeWorker enemy character.
    /// 
    /// Implements a finite state machine with four states: Patrol, Combat, Hit, and Dead.
    /// Provides intelligent patrol and combat behaviors, player detection with stealth awareness,
    /// visual movement effects, and a complete damage/death system.
    /// 
    /// Performance-optimized using static player caching, interval-based detection,
    /// NonAlloc physics methods, and animator parameter hashing.
    /// </summary>
    public class BeeWorkerBehaviorController : MonoBehaviour
    {
        #region Settings / Configuration

        [Header("Core Attributes")]
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private float attackDamage = 10f;
        [SerializeField] private float defense = 5f;
        [SerializeField] private float moveSpeed = 3f;

        [Header("Detection")]
        [SerializeField] private float detectionRadius = 5f;
        [SerializeField] private float detectionInterval = 0.2f;
        [SerializeField] private LayerMask playerLayer;

        [Header("Combat")]
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float chaseSpeedMultiplier = 1.5f;

        [Header("Damage System")]
        [SerializeField] private float invulnerabilityDuration = 0.5f;
        [SerializeField] private float knockbackForce = 5f;
        [SerializeField] private float knockbackDuration = 0.3f;

        [Header("Patrol")]
        [SerializeField] private Transform[] patrolPoints;
        [SerializeField] private float patrolWaitTime = 2f;
        [SerializeField] private float smoothTime = 0.3f;

        [Header("Visual Bouncing")]
        [SerializeField] private Transform visualTransform;
        [SerializeField] private float bouncingAmplitudeX = 0.1f;
        [SerializeField] private float bouncingAmplitudeY = 0.15f;
        [SerializeField] private float bouncingFrequency = 2f;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private Collider2D hitBox;
        [SerializeField] private Collider2D hurtBox;

        #endregion

        #region Debug

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool showGizmos = false;

        #endregion

        #region Private Variables

        /// <summary>
        /// Defines the four possible states of the BeeWorker enemy.
        /// </summary>
        private enum EnemyState
        {
            Patrol,   // Moving between patrol points
            Combat,   // Chasing and attacking player
            Hit,      // Taking damage with knockback
            Dead      // Death animation and cleanup
        }

        // State Machine
        private EnemyState currentState = EnemyState.Patrol;
        private EnemyState previousState = EnemyState.Patrol;
        private int currentHealth;

        // Detection
        private static Transform s_playerTransform;
        private static bool s_playerCached = false;
        private float detectionTimer = 0f;
        private Collider2D[] detectionResults = new Collider2D[1];
        private bool playerDetected = false;

        // Patrol
        private int currentPatrolIndex = 0;
        private float patrolWaitTimer = 0f;
        private bool isWaitingAtPoint = false;
        private Vector2 velocity = Vector2.zero;

        // Combat
        private bool isAttacking = false;

        // Damage
        private bool isInvulnerable = false;
        private Coroutine knockbackCoroutine;
        private Coroutine invulnerabilityCoroutine;

        // Bouncing
        private bool isBouncingEnabled = true;

        /// <summary>
        /// Cached Animator parameter hashes for performance optimization.
        /// Using Animator.StringToHash converts parameter names to integer hashes at initialization,
        /// eliminating string allocations and hash calculations during runtime animation control.
        /// This is critical for performance when multiple enemies are active simultaneously.
        /// </summary>
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int Attack = Animator.StringToHash("Attack");

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Initialize health
            currentHealth = maxHealth;

            // Validate and clamp health
            if (maxHealth <= 0)
            {
                Debug.LogWarning($"[BeeWorkerBehaviorController] maxHealth is set to {maxHealth}. Clamping to minimum value of 1.", this);
                maxHealth = 1;
                currentHealth = 1;
            }

            // Cache player reference (one-time static operation)
            CachePlayerReference();

            // Validate critical references
            ValidateReferences();

            // Ensure HitBox is disabled on start
            if (hitBox != null)
            {
                hitBox.enabled = false;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[BeeWorkerBehaviorController] Initialized with {currentHealth} health in {currentState} state.", this);
            }
        }

        private void Start()
        {
            // Placeholder for future initialization
        }

        private void Update()
        {
            // State machine update - handle current state
            switch (currentState)
            {
                case EnemyState.Patrol:
                    UpdatePatrol();
                    break;
                case EnemyState.Combat:
                    UpdateCombat();
                    break;
                case EnemyState.Hit:
                    UpdateHit();
                    break;
                case EnemyState.Dead:
                    UpdateDead();
                    break;
            }
            
            // Update visual bouncing effect
            UpdateBouncing();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Caches the player Transform reference statically for performance optimization.
        /// This is a one-time operation shared across all BeeWorker instances to avoid repeated Find operations.
        /// Uses GameObject.FindGameObjectWithTag for reliable player detection.
        /// </summary>
        private void CachePlayerReference()
        {
            // Only cache once across all instances
            if (!s_playerCached)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                
                if (playerObject != null)
                {
                    s_playerTransform = playerObject.transform;
                    s_playerCached = true;
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log("[BeeWorkerBehaviorController] Player reference cached successfully.", this);
                    }
                }
                else
                {
                    Debug.LogError("[BeeWorkerBehaviorController] Player GameObject not found! Ensure the player has the 'Player' tag assigned.", this);
                }
            }
        }

        /// <summary>
        /// Validates all required SerializeField references and logs errors for missing critical components.
        /// </summary>
        private void ValidateReferences()
        {
            bool hasErrors = false;

            if (animator == null)
            {
                Debug.LogError("[BeeWorkerBehaviorController] Animator reference is missing! Please assign in Inspector.", this);
                hasErrors = true;
            }

            if (hitBox == null)
            {
                Debug.LogError("[BeeWorkerBehaviorController] HitBox Collider2D reference is missing! Please assign in Inspector.", this);
                hasErrors = true;
            }

            if (hurtBox == null)
            {
                Debug.LogError("[BeeWorkerBehaviorController] HurtBox Collider2D reference is missing! Please assign in Inspector.", this);
                hasErrors = true;
            }

            if (visualTransform == null)
            {
                Debug.LogError("[BeeWorkerBehaviorController] Visual Transform reference is missing! Please assign in Inspector.", this);
                hasErrors = true;
            }

            // Validate patrol points (warning only, not critical)
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                Debug.LogWarning("[BeeWorkerBehaviorController] Patrol points array is empty or null. BeeWorker will remain stationary in Patrol state.", this);
            }

            // Validate layer configuration
            if (playerLayer.value == 0)
            {
                Debug.LogError("[BeeWorkerBehaviorController] Player layer is not configured! Detection will not function.", this);
                hasErrors = true;
            }

            // Disable script if critical references are missing
            if (hasErrors)
            {
                Debug.LogError("[BeeWorkerBehaviorController] Critical references missing. Disabling script.", this);
                enabled = false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Enables the HitBox collider. Called via Animation Event during attack animation.
        /// </summary>
        public void EnableHitBox()
        {
            if (hitBox != null)
            {
                hitBox.enabled = true;
                if (enableDebugLogs)
                {
                    Debug.Log("[BeeWorkerBehaviorController] HitBox enabled.", this);
                }
            }
        }

        /// <summary>
        /// Disables the HitBox collider. Called via Animation Event after attack animation.
        /// </summary>
        public void DisableHitBox()
        {
            if (hitBox != null)
            {
                hitBox.enabled = false;
                if (enableDebugLogs)
                {
                    Debug.Log("[BeeWorkerBehaviorController] HitBox disabled.", this);
                }
            }
        }

        /// <summary>
        /// Enables the visual bouncing effect.
        /// </summary>
        public void EnableBouncing()
        {
            isBouncingEnabled = true;
            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Bouncing enabled.", this);
            }
        }

        /// <summary>
        /// Disables the visual bouncing effect and resets visual transform position.
        /// </summary>
        public void DisableBouncing()
        {
            isBouncingEnabled = false;
            if (visualTransform != null)
            {
                visualTransform.localPosition = Vector3.zero;
            }
            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Bouncing disabled.", this);
            }
        }

        /// <summary>
        /// Called via Animation Event when death animation completes. Destroys the GameObject.
        /// </summary>
        public void OnDeathAnimationComplete()
        {
            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Death animation complete. Destroying GameObject.", this);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Receives damage from player attacks.
        /// Calculates final damage based on player attack value minus enemy defense.
        /// </summary>
        /// <param name="playerAttack">The attack value from the player</param>
        public void TakeDamageFromPlayer(int playerAttack)
        {
            // Calculate damage: playerAttack - defense (minimum 1 damage)
            float calculatedDamage = Mathf.Max(1f, playerAttack - defense);

            if (enableDebugLogs)
            {
                Debug.Log($"[BeeWorkerBehaviorController] Receiving player attack: {playerAttack}, Defense: {defense}, Final damage: {calculatedDamage}", this);
            }

            // Apply the calculated damage
            TakeDamage(calculatedDamage);
        }

        /// <summary>
        /// Destroys the BeeWorker GameObject immediately.
        /// Can be called from external systems or animation events.
        /// </summary>
        public void DestroyBeeWorker()
        {
            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] BeeWorker destroyed via public method.", this);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// Sets the invulnerability state to true.
        /// Prevents the BeeWorker from taking damage while invulnerable.
        /// </summary>
        public void SetInvulnerable()
        {
            isInvulnerable = true;
            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Invulnerability set to true.", this);
            }
        }

        /// <summary>
        /// Sets the invulnerability state to false.
        /// Allows the BeeWorker to take damage again.
        /// </summary>
        public void SetVulnerable()
        {
            isInvulnerable = false;
            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Invulnerability set to false.", this);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles behavior when in Patrol state.
        /// Moves between patrol points and checks for player detection.
        /// </summary>
        private void UpdatePatrol()
        {
            // Check for player detection at intervals
            CheckPlayerDetection();
            
            // Transition to Combat state when player detected
            if (playerDetected)
            {
                previousState = currentState;
                currentState = EnemyState.Combat;
                
                if (enableDebugLogs)
                {
                    Debug.Log("[BeeWorkerBehaviorController] Transitioning from Patrol to Combat - player detected", this);
                }
                
                return;
            }
            
            // Check if patrol points are configured
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                // No patrol points - remain stationary
                animator.SetBool(IsWalking, false);
                return;
            }
            
            // Get current patrol target
            Transform targetPoint = patrolPoints[currentPatrolIndex];
            
            if (targetPoint == null)
            {
                // Invalid patrol point - skip to next
                SelectNextPatrolPoint();
                return;
            }
            
            // Check if waiting at patrol point
            if (isWaitingAtPoint)
            {
                // Update wait timer
                patrolWaitTimer += Time.deltaTime;
                
                // Set animator to not walking while waiting
                animator.SetBool(IsWalking, false);
                
                // Check if wait time has elapsed
                if (patrolWaitTimer >= patrolWaitTime)
                {
                    // Wait complete - move to next patrol point
                    isWaitingAtPoint = false;
                    patrolWaitTimer = 0f;
                    SelectNextPatrolPoint();
                }
                
                return;
            }
            
            // Move toward current patrol point using SmoothDamp
            Vector2 currentPosition = transform.position;
            Vector2 targetPosition = targetPoint.position;
            
            // Calculate new position with smooth damping
            Vector2 newPosition = Vector2.SmoothDamp(
                currentPosition,
                targetPosition,
                ref velocity,
                smoothTime,
                moveSpeed
            );
            
            // Apply movement
            transform.position = newPosition;
            
            // Set animator to walking while moving
            animator.SetBool(IsWalking, true);
            
            // Check if arrived at patrol point using sqrMagnitude
            Vector2 toTarget = targetPosition - currentPosition;
            float sqrDistanceToTarget = toTarget.sqrMagnitude;
            float arrivalThreshold = 0.1f;
            float sqrArrivalThreshold = arrivalThreshold * arrivalThreshold;
            
            if (sqrDistanceToTarget <= sqrArrivalThreshold)
            {
                // Arrived at patrol point - start waiting
                isWaitingAtPoint = true;
                patrolWaitTimer = 0f;
                velocity = Vector2.zero; // Reset velocity for next movement
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[BeeWorkerBehaviorController] Arrived at patrol point {currentPatrolIndex}", this);
                }
            }
        }
        
        /// <summary>
        /// Selects the next patrol point in the sequence with wraparound logic.
        /// </summary>
        private void SelectNextPatrolPoint()
        {
            if (patrolPoints == null || patrolPoints.Length == 0)
            {
                return;
            }
            
            // Increment index with wraparound
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[BeeWorkerBehaviorController] Moving to patrol point {currentPatrolIndex}", this);
            }
        }
        
        /// <summary>
        /// Checks for player detection using Physics2D.OverlapCircleNonAlloc.
        /// Uses interval-based detection to reduce physics queries and improve performance.
        /// Respects player stealth mode - player cannot be detected while in stealth.
        /// Uses sqrMagnitude for distance comparisons to avoid expensive square root calculations.
        /// </summary>
        private void CheckPlayerDetection()
        {
            // Update detection timer
            detectionTimer += Time.deltaTime;
            
            // Only check at intervals to reduce performance overhead
            if (detectionTimer < detectionInterval)
            {
                return;
            }
            
            // Reset timer
            detectionTimer = 0f;
            
            // Validate player reference
            if (s_playerTransform == null)
            {
                playerDetected = false;
                
                // Attempt to re-cache player reference
                if (!s_playerCached)
                {
                    CachePlayerReference();
                }
                
                return;
            }
            
            // Use NonAlloc physics method to avoid allocations
            int hitCount = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                detectionRadius,
                detectionResults,
                playerLayer
            );
            
            // Check if player was detected
            if (hitCount > 0)
            {
                // Verify the detected collider belongs to the player
                // Use sqrMagnitude for distance comparison (avoids expensive sqrt)
                Vector2 toPlayer = s_playerTransform.position - transform.position;
                float sqrDistance = toPlayer.sqrMagnitude;
                float sqrDetectionRadius = detectionRadius * detectionRadius;
                
                if (sqrDistance <= sqrDetectionRadius)
                {
                    // Check if player is in stealth mode
                    bool playerInStealth = false;
                    
                    // Access GameManager to check stealth state
                    if (SlimeKing.Core.GameManager.HasInstance)
                    {
                        playerInStealth = SlimeKing.Core.GameManager.Instance.IsPlayerInStealth();
                    }
                    
                    // Player is detected only if not in stealth
                    playerDetected = !playerInStealth;
                    
                    if (enableDebugLogs && playerDetected)
                    {
                        Debug.Log($"[BeeWorkerBehaviorController] Player detected at distance {Mathf.Sqrt(sqrDistance):F2}", this);
                    }
                    else if (enableDebugLogs && playerInStealth)
                    {
                        Debug.Log("[BeeWorkerBehaviorController] Player in stealth - not detected", this);
                    }
                }
                else
                {
                    playerDetected = false;
                }
            }
            else
            {
                playerDetected = false;
            }
        }

        /// <summary>
        /// Handles behavior when in Combat state.
        /// Chases player and executes attacks when in range.
        /// </summary>
        private void UpdateCombat()
        {
            // Check for player detection at intervals
            CheckPlayerDetection();
            
            // Transition to Patrol state when player lost or enters stealth
            if (!playerDetected)
            {
                previousState = currentState;
                currentState = EnemyState.Patrol;
                
                if (enableDebugLogs)
                {
                    Debug.Log("[BeeWorkerBehaviorController] Transitioning from Combat to Patrol - player lost or in stealth", this);
                }
                
                return;
            }
            
            // Validate player reference
            if (s_playerTransform == null)
            {
                if (enableDebugLogs)
                {
                    Debug.LogWarning("[BeeWorkerBehaviorController] Player transform is null in Combat state", this);
                }
                return;
            }
            
            // Calculate direction to player using normalized vector
            Vector2 currentPosition = transform.position;
            Vector2 playerPosition = s_playerTransform.position;
            Vector2 directionToPlayer = (playerPosition - currentPosition).normalized;
            
            // Check distance to player using sqrMagnitude
            Vector2 toPlayer = playerPosition - currentPosition;
            float sqrDistanceToPlayer = toPlayer.sqrMagnitude;
            float sqrAttackRange = attackRange * attackRange;
            
            // Check if within attack range
            if (sqrDistanceToPlayer <= sqrAttackRange)
            {
                // Within attack range - trigger attack ONLY when in range
                if (!isAttacking)
                {
                    isAttacking = true;
                    animator.SetTrigger(Attack);
                    DisableBouncing();
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log($"[BeeWorkerBehaviorController] Triggering attack at distance {Mathf.Sqrt(sqrDistanceToPlayer):F2}", this);
                    }
                }
                
                // Stop moving while attacking
                animator.SetBool(IsWalking, false);
            }
            else
            {
                // Outside attack range - chase player instead of attacking
                isAttacking = false;
                
                // Apply movement at moveSpeed * chaseSpeedMultiplier
                float chaseSpeed = moveSpeed * chaseSpeedMultiplier;
                Vector2 movement = directionToPlayer * chaseSpeed * Time.deltaTime;
                transform.position = currentPosition + movement;
                
                // Set isWalking animator parameter to true during chase
                animator.SetBool(IsWalking, true);
                
                // Enable bouncing during chase (when not attacking)
                if (!isBouncingEnabled)
                {
                    EnableBouncing();
                }
                
                if (enableDebugLogs)
                {
                    Debug.Log($"[BeeWorkerBehaviorController] Chasing player at speed {chaseSpeed:F2}, distance {Mathf.Sqrt(sqrDistanceToPlayer):F2}", this);
                }
            }
        }

        /// <summary>
        /// Handles behavior when in Hit state.
        /// Processes knockback and damage reactions.
        /// </summary>
        private void UpdateHit()
        {
            // Bouncing is disabled in Hit state (handled in TakeDamage transition)
            // Knockback is handled via coroutine
            // State will transition back to previous state when knockback completes
        }

        /// <summary>
        /// Handles behavior when in Dead state.
        /// Ensures all systems are disabled and awaits destruction.
        /// </summary>
        private void UpdateDead()
        {
            // Dead state - no updates needed, awaiting destruction
        }

        #endregion

        #region Damage System

        /// <summary>
        /// Processes damage to the BeeWorker.
        /// Applies knockback, invulnerability, and transitions to Hit or Dead state.
        /// </summary>
        /// <param name="damage">The amount of damage to apply.</param>
        private void TakeDamage(float damage)
        {
            // Ignore damage if invulnerable
            if (isInvulnerable)
            {
                if (enableDebugLogs)
                {
                    Debug.Log("[BeeWorkerBehaviorController] Damage ignored - invulnerable", this);
                }
                return;
            }

            // Ignore damage if already dead
            if (currentState == EnemyState.Dead)
            {
                return;
            }

            // Subtract damage from health
            currentHealth -= Mathf.RoundToInt(damage);

            if (enableDebugLogs)
            {
                Debug.Log($"[BeeWorkerBehaviorController] Took {damage} damage. Health: {currentHealth}/{maxHealth}", this);
            }

            // Check for death
            if (currentHealth <= 0)
            {
                // Trigger death
                currentHealth = 0;
                animator.SetTrigger(Die);
                
                // Transition to Dead state
                previousState = currentState;
                currentState = EnemyState.Dead;
                
                // Disable bouncing in Dead state
                DisableBouncing();
                
                // Disable all colliders
                if (hitBox != null) hitBox.enabled = false;
                if (hurtBox != null) hurtBox.enabled = false;
                
                // Get main collider and disable it
                Collider2D mainCollider = GetComponent<Collider2D>();
                if (mainCollider != null) mainCollider.enabled = false;
                
                // Stop all movement
                animator.SetBool(IsWalking, false);
                
                
                // Disable the BeeWorkerBehaviorController script
                enabled = false;
                
                if (enableDebugLogs)
                {
                    Debug.Log("[BeeWorkerBehaviorController] BeeWorker died", this);
                }
                
                return;
            }

            // Trigger Hit animation
            animator.SetTrigger(Hit);

            // Store previous state and transition to Hit state
            previousState = currentState;
            currentState = EnemyState.Hit;
            
            // Disable bouncing in Hit state
            DisableBouncing();

            // Apply knockback
            if (knockbackCoroutine != null)
            {
                StopCoroutine(knockbackCoroutine);
            }
            knockbackCoroutine = StartCoroutine(ApplyKnockback());

            // Apply invulnerability
            if (invulnerabilityCoroutine != null)
            {
                StopCoroutine(invulnerabilityCoroutine);
            }
            invulnerabilityCoroutine = StartCoroutine(ApplyInvulnerability());
        }

        /// <summary>
        /// Applies knockback movement away from the player.
        /// </summary>
        private IEnumerator ApplyKnockback()
        {
            // Validate player reference
            if (s_playerTransform == null)
            {
                yield break;
            }

            // Calculate knockback direction away from player
            Vector2 currentPosition = transform.position;
            Vector2 playerPosition = s_playerTransform.position;
            Vector2 knockbackDirection = (currentPosition - playerPosition).normalized;

            // Apply knockback over duration
            float elapsedTime = 0f;
            Vector2 startPosition = currentPosition;
            Vector2 targetPosition = startPosition + (knockbackDirection * knockbackForce);

            while (elapsedTime < knockbackDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / knockbackDuration;

                // Lerp position from start to target
                transform.position = Vector2.Lerp(startPosition, targetPosition, t);

                yield return null;
            }

            // Knockback complete - transition back to previous state
            currentState = previousState;
            
            // Re-enable bouncing when returning to Patrol or Combat state
            if (currentState == EnemyState.Patrol || currentState == EnemyState.Combat)
            {
                EnableBouncing();
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[BeeWorkerBehaviorController] Knockback complete. Returning to {currentState} state", this);
            }

            knockbackCoroutine = null;
        }

        /// <summary>
        /// Applies temporary invulnerability after taking damage.
        /// </summary>
        private IEnumerator ApplyInvulnerability()
        {
            isInvulnerable = true;

            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Invulnerability activated", this);
            }

            yield return new WaitForSeconds(invulnerabilityDuration);

            isInvulnerable = false;

            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Invulnerability expired", this);
            }

            invulnerabilityCoroutine = null;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Updates the visual bouncing effect applied to the child visual transform.
        /// Creates organic movement using sine and cosine oscillations.
        /// Bouncing is disabled during attacks, hit reactions, and death.
        /// </summary>
        private void UpdateBouncing()
        {
            if (visualTransform == null) return;
            
            if (isBouncingEnabled)
            {
                // Calculate oscillating offsets using time-based sine/cosine
                float xOffset = Mathf.Sin(Time.time * bouncingFrequency) * bouncingAmplitudeX;
                float yOffset = Mathf.Cos(Time.time * bouncingFrequency * 1.3f) * bouncingAmplitudeY;
                
                // Apply offset to visual transform
                visualTransform.localPosition = new Vector3(xOffset, yOffset, 0f);
            }
            else
            {
                // Reset to zero when bouncing is disabled
                visualTransform.localPosition = Vector3.zero;
            }
        }
        
        /// <summary>
        /// Logs a debug message if debug logging is enabled.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[BeeWorkerBehaviorController] {message}", this);
            }
        }

        #endregion

        #region Gizmos

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            // Draw detection radius (yellow)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Draw attack range (red)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Draw patrol points and connections (cyan)
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                Gizmos.color = Color.cyan;

                // Draw spheres at each patrol point
                foreach (Transform point in patrolPoints)
                {
                    if (point != null)
                    {
                        Gizmos.DrawWireSphere(point.position, 0.3f);
                    }
                }

                // Draw lines connecting patrol points
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] != null)
                    {
                        Transform nextPoint = patrolPoints[(i + 1) % patrolPoints.Length];
                        if (nextPoint != null)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, nextPoint.position);
                        }
                    }
                }
            }

            // Draw state indicator cube (only in Play mode)
            if (Application.isPlaying)
            {
                Vector3 indicatorPosition = transform.position + Vector3.up * 1.5f;
                
                switch (currentState)
                {
                    case EnemyState.Patrol:
                        Gizmos.color = Color.green;
                        break;
                    case EnemyState.Combat:
                        Gizmos.color = Color.red;
                        break;
                    case EnemyState.Hit:
                        Gizmos.color = Color.white;
                        break;
                    case EnemyState.Dead:
                        Gizmos.color = Color.black;
                        break;
                }

                Gizmos.DrawCube(indicatorPosition, Vector3.one * 0.3f);
            }
        }
#endif

        #endregion
    }
}
