using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente básico de inimigo para funcionar com EnemyManager.
    /// Classe simples que pode ser estendida para comportamentos específicos.
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Info")]
        [SerializeField] private EnemyType enemyType;
        [SerializeField] private EnemyState currentState = EnemyState.Idle;

        // Dados do inimigo
        private EnemyData enemyData;
        private int currentHealth;
        private Transform target;

        // Properties
        public EnemyType EnemyType => enemyType;
        public EnemyState CurrentState => currentState;
        public bool IsDead => currentState == EnemyState.Dead;
        public int CurrentHealth => currentHealth;
        public Transform Target => target;

        /// <summary>
        /// Inicializa o inimigo com dados específicos
        /// </summary>
        public void Initialize(EnemyData data)
        {
            enemyData = data;
            enemyType = data.type;
            currentHealth = data.health;
            currentState = EnemyState.Idle;

            // Encontra o jogador automaticamente
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }

        /// <summary>
        /// Recebe dano
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Mata o inimigo
        /// </summary>
        public void Die()
        {
            if (IsDead) return;

            currentState = EnemyState.Dead;

            // Dá recompensas ao jogador
            if (PlayerManager.Instance != null && enemyData != null)
            {
                PlayerManager.Instance.AddExperience(enemyData.expReward);
            }

            // Retorna para o pool
            if (EnemyManager.Instance != null)
            {
                EnemyManager.Instance.ReturnEnemyToPool(gameObject, enemyType);
            }
        }

        /// <summary>
        /// Define o alvo do inimigo
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        /// <summary>
        /// Muda o estado do inimigo
        /// </summary>
        public void ChangeState(EnemyState newState)
        {
            currentState = newState;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Exemplo básico de detecção de colisão com projéteis
            if (other.CompareTag("PlayerProjectile"))
            {
                TakeDamage(10); // Dano padrão
                Destroy(other.gameObject); // Remove o projétil
            }
        }
    }
}