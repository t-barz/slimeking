using UnityEngine;

namespace TheSlimeKing.NPCs
{
    /// <summary>
    /// Controller base para todos os NPCs.
    /// Gerencia comportamentos comuns como movimento, detecção de ataque e integração com NPCAttributesHandler.
    /// </summary>
    [RequireComponent(typeof(NPCAttributesHandler))]
    public class NPCBaseController : MonoBehaviour
    {

        #region Inspector Variables
        [Header("Debug")]
        [SerializeField] protected bool enableDebugLogs = false;
        [SerializeField] protected bool enableDebugGizmos = true;
        #endregion

        #region Protected Variables
        protected NPCAttributesHandler attributesHandler;
        protected Rigidbody2D rb;
        protected Animator animator;
        protected Transform playerTransform;
        #endregion

        #region Unity Lifecycle
        protected virtual void Awake()
        {
            // Cache de componentes
            attributesHandler = GetComponent<NPCAttributesHandler>();
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

        }

        protected virtual void Start()
        {
            // Encontra referência do player
            FindPlayer();

            if (enableDebugLogs)
            {
                Log($"NPC inicializado");
            }
        }

        protected virtual void Update()
        {
            if (attributesHandler.IsDead)
            {

                return;
            }

            UpdateState();
            UpdateMovement();
            UpdateAnimations();
        }

        protected virtual void FixedUpdate()
        {
            if (attributesHandler.IsDead) return;

            PerformMovement();
        }
        #endregion

        #region Combat Methods
        /// <summary>
        /// Método chamado quando o NPC recebe dano por ataque do player.
        /// </summary>
        public virtual void TakeDamage()
        {
            if (attributesHandler.IsDead) return;

            // Obtém valor de ataque do player
            int playerAttack = GetPlayerAttack();
            int damage = CalculateDamage(playerAttack, attributesHandler.CurrentDefense);

            if (damage > 0)
            {
                bool died = attributesHandler.TakeDamage(damage);

                // Log apenas quando efetivamente atingido
                if (enableDebugLogs)
                {
                    Log($"Recebeu {damage} de dano! Vida restante: {attributesHandler.CurrentHealthPoints}");
                }

                OnTakeDamage(damage, died);

                if (died)
                {
                    OnDeath();
                }
            }
            else
            {
                OnAttackBlocked();
            }
        }
        #endregion

        #region Protected Virtual Methods - Override em classes filhas
        /// <summary>
        /// Atualiza o estado atual do NPC baseado na situação.
        /// </summary>
        protected virtual void UpdateState()
        {
            // TODO: Implementar sistema de estados (Idle, Patrolling, Chasing, Attacking, Dead)
            // TODO: Adicionar detecção de distância do player
            // TODO: Implementar transições entre estados baseadas em ranges
        }

        /// <summary>
        /// Atualiza movimento baseado no estado atual.
        /// </summary>
        protected virtual void UpdateMovement()
        {
            // TODO: Implementar lógica de movimento baseada no estado atual
            // TODO: Adicionar patrulha, perseguição e posicionamento para ataque
            // TODO: Calcular targetPosition baseado no estado
        }

        /// <summary>
        /// Aplica movimento físico.
        /// </summary>
        protected virtual void PerformMovement()
        {
            // TODO: Implementar aplicação de movimento via Rigidbody2D
            // TODO: Usar attributesHandler.CurrentSpeed para velocidade
            // TODO: Calcular direção baseada em targetPosition
        }

        /// <summary>
        /// Atualiza animações baseado no estado.
        /// </summary>
        protected virtual void UpdateAnimations()
        {
            // TODO: Implementar controle de animações baseado no estado
            // TODO: Configurar parâmetros do Animator (IsMoving, Attack, Death)
            // TODO: Usar Animator.StringToHash para performance
        }

        /// <summary>
        /// Chamado quando o NPC recebe dano.
        /// </summary>
        protected virtual void OnTakeDamage(int damage, bool died)
        {
            if (enableDebugLogs)
            {
                Log($"Recebeu {damage} de dano. Vida restante: {attributesHandler.CurrentHealthPoints}");
            }
        }

        /// <summary>
        /// Chamado quando o ataque é bloqueado pela defesa.
        /// </summary>
        protected virtual void OnAttackBlocked()
        {
            if (enableDebugLogs)
            {
                Log("Ataque bloqueado pela defesa");
            }
        }

        /// <summary>
        /// Chamado quando o NPC morre.
        /// </summary>
        protected virtual void OnDeath()
        {
            // TODO: Implementar comportamento de morte
            // TODO: Parar movimento (rb.velocity = Vector2.zero)
            // TODO: Trigger animação de morte
            // TODO: Desativar colliders não-trigger
            // TODO: Mudar estado para Dead
        }
        #endregion

        #region Protected Helper Methods


        protected void FindPlayer()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                LogWarning("Player não encontrado! Verifique se o player tem a tag 'Player'");
            }
        }

        protected int GetPlayerAttack()
        {
            if (playerTransform == null) return 1;

            var playerAttributes = playerTransform.GetComponent<PlayerAttributesHandler>();
            if (playerAttributes != null)
            {
                return playerAttributes.CurrentAttack;
            }

            // Fallback
            return 1;
        }

        protected int CalculateDamage(int attackPower, int defense)
        {
            return Mathf.Max(0, attackPower - defense);
        }

        protected void Log(string message)
        {
            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"[{GetType().Name}] {gameObject.name} - {message}");
            }
        }

        protected void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[{GetType().Name}] {gameObject.name} - {message}");
        }
        #endregion

        #region Gizmos
        protected virtual void OnDrawGizmosSelected()
        {
            if (!enableDebugGizmos) return;

            // TODO: Implementar gizmos de debug
            // TODO: Desenhar range de detecção (círculo amarelo)
            // TODO: Desenhar range de ataque (círculo vermelho)  
            // TODO: Desenhar range de patrulha (círculo verde)
            // TODO: Mostrar posição alvo atual (cubo azul)
        }
        #endregion
    }
}