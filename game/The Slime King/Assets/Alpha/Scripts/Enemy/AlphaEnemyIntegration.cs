using UnityEngine;
using System;

namespace SlimeMec.Alpha.Enemy
{
    /// <summary>
    /// Adapter que integra o sistema Alpha de inimigos com o AttackHandler existente
    /// sem modificar o código original. Atua como bridge entre os sistemas.
    /// </summary>
    public class AlphaEnemyIntegration : MonoBehaviour
    {
        #region Events
        public static event Action<GameObject, float> OnEnemyTakeDamage;
        public static event Action<GameObject> OnEnemyDeath;
        public static event Action<GameObject, GameObject> OnEnemyAttackPlayer;
        #endregion

        #region Serialized Fields
        [Header("Integration Configuration")]
        [SerializeField] private bool autoDetectAttackHandler = true;
        [SerializeField] private bool enableHealthSystem = true;
        [SerializeField] private float maxHealth = 100f;
        
        [Header("Damage Integration")]
        [SerializeField] private bool receivePlayerDamage = true;
        [SerializeField] private float damageMultiplier = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        [SerializeField] private bool showGizmos = true;
        #endregion

        #region Components
        private EnemyController enemyController;
        private Collider2D enemyCollider;
        private SpriteRenderer spriteRenderer;
        #endregion

        #region Private Fields
        private float currentHealth;
        private bool isDead = false;
        private bool isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
            InitializeHealth();
        }

        private void OnEnable()
        {
            SetupEventListeners();
        }

        private void OnDisable()
        {
            CleanupEventListeners();
        }

        private void Start()
        {
            SetupIntegrations();
        }
        #endregion

        #region Initialization
        private void InitializeComponents()
        {
            enemyController = GetComponent<EnemyController>();
            if (enemyController == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"AlphaEnemyIntegration: EnemyController not found on {gameObject.name}");
            }

            enemyCollider = GetComponent<Collider2D>();
            if (enemyCollider == null)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"AlphaEnemyIntegration: Collider2D not found on {gameObject.name}");
            }

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void InitializeHealth()
        {
            if (enableHealthSystem)
            {
                currentHealth = maxHealth;
            }

            isInitialized = true;
        }

        private void SetupEventListeners()
        {
            // TODO: Conectar aos eventos do AttackHandler quando disponíveis
            // Exemplo: AttackHandler.OnAttackHit += HandlePlayerAttack;
        }

        private void CleanupEventListeners()
        {
            // TODO: Desconectar eventos
            // Exemplo: AttackHandler.OnAttackHit -= HandlePlayerAttack;
        }

        private void SetupIntegrations()
        {
            if (autoDetectAttackHandler)
            {
                DetectAndSetupAttackHandlerIntegration();
            }

            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: Integrations setup completed for {gameObject.name}");
        }

        private void DetectAndSetupAttackHandlerIntegration()
        {
            // TODO: Procurar AttackHandler na cena e configurar integração
            // Esta será a ponte com o sistema de combate existente
        }
        #endregion

        #region Damage System Integration
        /// <summary>
        /// Método que deve ser chamado quando o AttackHandler atinge este inimigo
        /// </summary>
        public void OnAttackHandlerHit(float damage, Vector2 attackDirection, GameObject attacker)
        {
            if (!receivePlayerDamage || isDead) return;

            float finalDamage = damage * damageMultiplier;
            TakeDamage(finalDamage, attacker);

            // Notifica o EnemyController sobre o ataque
            if (enemyController != null)
            {
                enemyController.OnTakeDamage(finalDamage, attackDirection, attacker);
            }

            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: {gameObject.name} took {finalDamage} damage from {attacker.name}");
        }

        /// <summary>
        /// Aplica dano ao inimigo
        /// </summary>
        public void TakeDamage(float damage, GameObject source = null)
        {
            if (!enableHealthSystem || isDead) return;

            currentHealth -= damage;
            OnEnemyTakeDamage?.Invoke(gameObject, damage);

            // Visual feedback
            ShowDamageEffect();

            if (currentHealth <= 0f)
            {
                Die(source);
            }

            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: {gameObject.name} health: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Mata o inimigo
        /// </summary>
        public void Die(GameObject killer = null)
        {
            if (isDead) return;

            isDead = true;
            currentHealth = 0f;

            // Notifica morte
            OnEnemyDeath?.Invoke(gameObject);

            // Notifica o EnemyController
            if (enemyController != null)
            {
                enemyController.OnDeath(killer);
            }

            // Efeito visual de morte
            ShowDeathEffect();

            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: {gameObject.name} died");
        }
        #endregion

        #region Attack Integration
        /// <summary>
        /// Método para ser chamado quando o inimigo ataca o player
        /// Integra com o sistema de dano do player
        /// </summary>
        public void AttackPlayer(GameObject player, float damage)
        {
            if (isDead) return;

            OnEnemyAttackPlayer?.Invoke(gameObject, player);

            // TODO: Integrar com PlayerAttributesSystem para aplicar dano
            // Exemplo: PlayerAttributesSystem.Instance.TakeDamage(damage);

            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: {gameObject.name} attacked player for {damage} damage");
        }
        #endregion

        #region Visual Effects
        private void ShowDamageEffect()
        {
            // Efeito visual simples de dano
            if (spriteRenderer != null)
            {
                StartCoroutine(DamageFlashCoroutine());
            }
        }

        private void ShowDeathEffect()
        {
            // Efeito visual de morte
            if (spriteRenderer != null)
            {
                StartCoroutine(DeathEffectCoroutine());
            }
        }

        private System.Collections.IEnumerator DamageFlashCoroutine()
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            
            yield return new WaitForSeconds(0.1f);
            
            spriteRenderer.color = originalColor;
        }

        private System.Collections.IEnumerator DeathEffectCoroutine()
        {
            Color originalColor = spriteRenderer.color;
            float fadeTime = 1f;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            // Destroy após fade
            Destroy(gameObject);
        }
        #endregion

        #region Collision Integration
        /// <summary>
        /// Integração com sistema de colisão para detecção de ataques
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isDead) return;

            // Verifica se é um ataque do player
            if (other.CompareTag("PlayerAttack") || other.CompareTag("Attack"))
            {
                // TODO: Extrair dados de dano do collider ou componente associado
                float damage = 25f; // Valor padrão
                Vector2 attackDirection = (transform.position - other.transform.position).normalized;
                
                OnAttackHandlerHit(damage, attackDirection, other.gameObject);
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Verifica se o inimigo está morto
        /// </summary>
        public bool IsDead => isDead;

        /// <summary>
        /// Obtém a vida atual do inimigo
        /// </summary>
        public float GetCurrentHealth() => currentHealth;

        /// <summary>
        /// Obtém a vida máxima do inimigo
        /// </summary>
        public float GetMaxHealth() => maxHealth;

        /// <summary>
        /// Obtém o percentual de vida
        /// </summary>
        public float GetHealthPercentage() => enableHealthSystem ? currentHealth / maxHealth : 1f;

        /// <summary>
        /// Cura o inimigo
        /// </summary>
        public void Heal(float amount)
        {
            if (!enableHealthSystem || isDead) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: {gameObject.name} healed for {amount}. Health: {currentHealth}/{maxHealth}");
        }

        /// <summary>
        /// Reseta o inimigo para estado inicial
        /// </summary>
        public void ResetEnemy()
        {
            isDead = false;
            currentHealth = maxHealth;
            
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, 1f);
            }

            if (enemyController != null)
            {
                enemyController.ResetEnemy();
            }

            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: {gameObject.name} reset to initial state");
        }
        #endregion

        #region Integration Helpers
        /// <summary>
        /// Configura integração manual com AttackHandler específico
        /// </summary>
        public void SetupAttackHandlerIntegration(GameObject attackHandler)
        {
            // TODO: Configurar conexão específica com AttackHandler
            if (enableDebugLogs)
                Debug.Log($"AlphaEnemyIntegration: Manual integration setup with {attackHandler.name}");
        }

        /// <summary>
        /// Obtém dados do inimigo para outros sistemas
        /// </summary>
        public EnemyData GetEnemyData()
        {
            return new EnemyData
            {
                gameObject = this.gameObject,
                currentHealth = currentHealth,
                maxHealth = maxHealth,
                isDead = isDead,
                position = transform.position
            };
        }
        #endregion

        #region Gizmos
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos || !enabled) return;

            // Desenha barra de vida
            if (enableHealthSystem && Application.isPlaying)
            {
                Vector3 pos = transform.position + Vector3.up * 1.5f;
                float healthPercentage = GetHealthPercentage();
                
                Gizmos.color = Color.red;
                Gizmos.DrawLine(pos - Vector3.right * 0.5f, pos + Vector3.right * 0.5f);
                
                Gizmos.color = Color.green;
                Vector3 healthEnd = pos - Vector3.right * 0.5f + Vector3.right * healthPercentage;
                Gizmos.DrawLine(pos - Vector3.right * 0.5f, healthEnd);
            }

            // Desenha área de detecção
            if (enemyCollider != null)
            {
                Gizmos.color = isDead ? Color.gray : Color.yellow;
                Gizmos.DrawWireCube(transform.position, enemyCollider.bounds.size);
            }
        }
        #endregion
    }

    /// <summary>
    /// Estrutura de dados do inimigo para integração
    /// </summary>
    [System.Serializable]
    public struct EnemyData
    {
        public GameObject gameObject;
        public float currentHealth;
        public float maxHealth;
        public bool isDead;
        public Vector3 position;
    }
}