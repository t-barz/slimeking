using UnityEngine;
using UnityEngine.AI;

namespace SlimeMec.Alpha.Enemy
{
    /// <summary>
    /// Controller básico de inimigo para Demo Alpha
    /// Implementa FSM simples: Patrol → Chase → Attack → Death
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float patrolRange = 5f;
        [SerializeField] private float detectionRange = 8f;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float attackCooldown = 2f;
        [SerializeField] private int attackDamage = 10;

        [Header("References")]
        [SerializeField] private Transform attackPoint;
        [SerializeField] private GameObject deathVFX;
        [SerializeField] private GameObject[] dropItems;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool drawDebugGizmos = true;

        // Components
        private Rigidbody2D rb;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        // FSM State
        private EnemyState currentState = EnemyState.Patrol;
        private Transform player;
        private Vector3 patrolCenter;
        private Vector3 patrolTarget;
        private float lastAttackTime = 0f;

        // Health (integration with existing system)
        private int currentHealth;
        private int maxHealth = 50;

        // Events
        public System.Action<EnemyController> OnEnemyDeath;
        public System.Action<EnemyController, int> OnEnemyTakeDamage;

        #region Unity Lifecycle

        private void Awake()
        {
            // TODO: Get required components
            // TODO: Initialize health
            // TODO: Set patrol center

            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {name} initialized");
        }

        private void Start()
        {
            // TODO: Find player reference
            // TODO: Setup initial state
            // TODO: Generate first patrol target

            SetState(EnemyState.Patrol);
        }

        private void Update()
        {
            // TODO: Update FSM based on current state
            UpdateStateMachine();

            // TODO: Update animations
            UpdateAnimations();
        }

        private void FixedUpdate()
        {
            // TODO: Handle movement in FixedUpdate
            HandleMovement();
        }

        #endregion

        #region State Machine

        private void UpdateStateMachine()
        {
            switch (currentState)
            {
                case EnemyState.Patrol:
                    UpdatePatrolState();
                    break;
                case EnemyState.Chase:
                    UpdateChaseState();
                    break;
                case EnemyState.Attack:
                    UpdateAttackState();
                    break;
                case EnemyState.Hit:
                    UpdateHitState();
                    break;
                case EnemyState.Death:
                    UpdateDeathState();
                    break;
            }
        }

        private void UpdatePatrolState()
        {
            // TODO: Check for player in detection range
            // TODO: Move towards patrol target
            // TODO: Generate new patrol target when reached
            // TODO: Transition to Chase if player detected

            float distanceToPlayer = GetDistanceToPlayer();
            if (distanceToPlayer <= detectionRange)
            {
                SetState(EnemyState.Chase);
                return;
            }

            // Continue patrol logic
            // TODO: Implement patrol movement
        }

        private void UpdateChaseState()
        {
            // TODO: Move towards player
            // TODO: Check if player is in attack range
            // TODO: Check if player escaped (too far)
            // TODO: Transition to Attack or back to Patrol

            float distanceToPlayer = GetDistanceToPlayer();

            if (distanceToPlayer <= attackRange)
            {
                SetState(EnemyState.Attack);
            }
            else if (distanceToPlayer > detectionRange * 1.5f) // Hysteresis
            {
                SetState(EnemyState.Patrol);
            }
        }

        private void UpdateAttackState()
        {
            // TODO: Face player
            // TODO: Check attack cooldown
            // TODO: Execute attack if ready
            // TODO: Return to appropriate state after attack

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                ExecuteAttack();
                lastAttackTime = Time.time;
            }

            // Check if should continue attacking or chase
            float distanceToPlayer = GetDistanceToPlayer();
            if (distanceToPlayer > attackRange)
            {
                SetState(EnemyState.Chase);
            }
        }

        private void UpdateHitState()
        {
            // TODO: Handle hit reaction
            // TODO: Apply knockback if needed
            // TODO: Return to previous state after hit animation
        }

        private void UpdateDeathState()
        {
            // TODO: Handle death sequence
            // TODO: Disable components
            // TODO: Play death VFX
            // TODO: Drop items
            // TODO: Destroy or disable GameObject
        }

        private void SetState(EnemyState newState)
        {
            if (currentState == newState) return;

            // Exit current state
            ExitState(currentState);

            // Enter new state
            currentState = newState;
            EnterState(newState);

            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {name} state changed to: {newState}");
        }

        private void EnterState(EnemyState state)
        {
            // TODO: Handle state entry logic
            switch (state)
            {
                case EnemyState.Patrol:
                    // TODO: Reset patrol target
                    break;
                case EnemyState.Chase:
                    // TODO: Start chase logic
                    break;
                case EnemyState.Attack:
                    // TODO: Prepare attack
                    break;
                case EnemyState.Hit:
                    // TODO: Start hit reaction
                    break;
                case EnemyState.Death:
                    // TODO: Start death sequence
                    break;
            }
        }

        private void ExitState(EnemyState state)
        {
            // TODO: Handle state exit logic if needed
        }

        #endregion

        #region Movement & Animation

        private void HandleMovement()
        {
            // TODO: Apply movement based on current state
            // TODO: Use Rigidbody2D for physics-based movement
        }

        private void UpdateAnimations()
        {
            // TODO: Update animator parameters based on state
            // TODO: Set movement direction
            // TODO: Trigger attack animations
        }

        #endregion

        #region Combat System

        private void ExecuteAttack()
        {
            // TODO: Check if player is still in range
            // TODO: Apply damage to player
            // TODO: Play attack animation
            // TODO: Spawn attack VFX
            // TODO: Play attack sound

            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {name} executed attack");
        }

        public void TakeDamage(int damage)
        {
            // TODO: Reduce health
            // TODO: Trigger hit state
            // TODO: Play hit VFX
            // TODO: Check for death
            // TODO: Apply knockback if needed

            currentHealth -= damage;

            OnEnemyTakeDamage?.Invoke(this, damage);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                SetState(EnemyState.Hit);
            }

            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        }

        private void Die()
        {
            // TODO: Set death state
            // TODO: Disable colliders
            // TODO: Play death VFX
            // TODO: Drop items
            // TODO: Trigger death event

            SetState(EnemyState.Death);
            OnEnemyDeath?.Invoke(this);

            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {name} died");
        }

        #endregion

        #region Utility Methods

        private float GetDistanceToPlayer()
        {
            if (player == null) return float.MaxValue;
            return Vector3.Distance(transform.position, player.position);
        }

        private Vector3 GeneratePatrolTarget()
        {
            // TODO: Generate random point within patrol range
            // TODO: Ensure point is valid (not in walls, etc.)
            return patrolCenter;
        }

        #endregion

        #region Integration with Existing Systems

        /// <summary>
        /// Integração com AttackHandler existente
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // TODO: Check if hit by player attack
            // TODO: Integration with existing attack system
            if (other.CompareTag("Attack"))
            {
                // TODO: Get damage from AttackHandler
                TakeDamage(10); // Placeholder
            }
        }

        #endregion

        #region Debug & Editor

        private void OnDrawGizmosSelected()
        {
            if (!drawDebugGizmos) return;

            // Patrol range
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(patrolCenter, patrolRange);

            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Current target
            if (Application.isPlaying)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(patrolTarget, 0.2f);
            }
        }

        [ContextMenu("Debug - Force Death")]
        private void DebugForceDeath()
        {
            Die();
        }

        [ContextMenu("Debug - Take Damage")]
        private void DebugTakeDamage()
        {
            TakeDamage(10);
        }

        #endregion

        #region Integration Methods (for AlphaEnemyIntegration)

        /// <summary>
        /// Método chamado quando o inimigo recebe dano
        /// </summary>
        public void OnTakeDamage(float damage, Vector2 attackDirection, GameObject attacker)
        {
            // Efeito visual de dano pode ser implementado aqui
            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {gameObject.name} received {damage} damage from {attacker.name}");

            // Pode forçar estado de Hit se necessário
            // SetState(EnemyState.Hit);

            // Knockback simples (opcional)
            if (rb != null && attackDirection != Vector2.zero)
            {
                rb.AddForce(attackDirection.normalized * 5f, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Método chamado quando o inimigo morre
        /// </summary>
        public void OnDeath(GameObject killer)
        {
            SetState(EnemyState.Death);

            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {gameObject.name} killed by {killer.name}");

            // Desabilita colisão
            var collider = GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = false;

            // Para movimento
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }

        /// <summary>
        /// Reseta o inimigo para estado inicial
        /// </summary>
        public void ResetEnemy()
        {
            currentHealth = maxHealth;
            SetState(EnemyState.Patrol);

            // Reabilita colisão
            var collider = GetComponent<Collider2D>();
            if (collider != null)
                collider.enabled = true;

            if (enableDebugLogs)
                Debug.Log($"[EnemyController] {gameObject.name} reset to initial state");
        }

        #endregion
    }

    /// <summary>
    /// Estados da FSM do inimigo
    /// </summary>
    public enum EnemyState
    {
        Patrol,     // Patrulhando área
        Chase,      // Perseguindo player
        Attack,     // Atacando player
        Hit,        // Recebendo dano
        Death       // Morto
    }
}