using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente responsável por detectar colisões do HitBox do inimigo com o player.
    /// Aplica dano ao player quando o HitBox (ativado durante animação de ataque) colide com ele.
    /// 
    /// FUNCIONAMENTO:
    /// • Anexado ao GameObject HitBox do inimigo
    /// • HitBox é ativado/desativado via Animation Events (EnableHitBox/DisableHitBox)
    /// • Quando ativo, detecta colisões com objetos que têm tag "Player"
    /// • Obtém o valor de ataque do BeeWorkerBehaviorController pai
    /// • Aplica dano usando PlayerAttributesHandler.TakeDamage()
    /// 
    /// REQUISITOS:
    /// • GameObject deve ter Collider2D configurado como Trigger
    /// • GameObject pai deve ter BeeWorkerBehaviorController
    /// • Player deve ter tag "Player" e componente PlayerAttributesHandler
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class EnemyHitBox : MonoBehaviour
    {
        #region Settings / Configuration

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        #endregion

        #region Private Variables

        private TheSlimeKing.Gameplay.BeeWorkerBehaviorController beeWorkerController;
        private Collider2D hitBoxCollider;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Obtém referência ao Collider2D
            hitBoxCollider = GetComponent<Collider2D>();

            // Valida que o collider é trigger
            if (hitBoxCollider != null && !hitBoxCollider.isTrigger)
            {
                Debug.LogWarning("[EnemyHitBox] Collider2D não está configurado como Trigger! Configurando automaticamente.", this);
                hitBoxCollider.isTrigger = true;
            }

            // Obtém referência ao BeeWorkerBehaviorController no pai
            beeWorkerController = GetComponentInParent<TheSlimeKing.Gameplay.BeeWorkerBehaviorController>();

            if (beeWorkerController == null)
            {
                Debug.LogError("[EnemyHitBox] BeeWorkerBehaviorController não encontrado no GameObject pai! O HitBox não funcionará.", this);
                enabled = false;
            }

            if (enableDebugLogs)
            {
                Debug.Log("[EnemyHitBox] Inicializado com sucesso.", this);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[EnemyHitBox] OnTriggerEnter2D chamado! Collider: {other.gameObject.name}, Tag: {other.tag}, Layer: {other.gameObject.layer}", this);
            }

            // Verifica se colidiu com o player
            if (other.CompareTag("Player"))
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[EnemyHitBox] Player detectado! GameObject: {other.gameObject.name}", this);
                }

                // Obtém o PlayerAttributesHandler
                var playerAttributes = other.GetComponent<SlimeKing.Gameplay.PlayerAttributesHandler>();

                if (playerAttributes != null && beeWorkerController != null)
                {
                    // Obtém o valor de ataque do inimigo
                    int enemyAttack = beeWorkerController.GetAttackDamage();

                    // Aplica dano ao player
                    playerAttributes.TakeDamage(enemyAttack, false);

                    if (enableDebugLogs)
                    {
                        Debug.Log($"[EnemyHitBox] Dano aplicado ao player: {enemyAttack}", this);
                    }
                }
                else
                {
                    if (enableDebugLogs)
                    {
                        Debug.LogWarning($"[EnemyHitBox] PlayerAttributesHandler não encontrado no GameObject {other.name} ou beeWorkerController é null", this);
                    }
                }
            }
            else
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"[EnemyHitBox] Collider não é Player. Tag: {other.tag}", this);
                }
            }
        }

        #endregion
    }
}
