using UnityEngine;
using System.Collections;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia o sistema de combate do jogador.
    /// Responsável por controlar os ataques e seus efeitos visuais.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerVisualManager))]
    [RequireComponent(typeof(PlayerAudioManager))]
    public class PlayerCombat : MonoBehaviour
    {
        #region Campos Serializados
        [Header("Attack Settings")]
        [Tooltip("Duração do ataque em segundos")]
        [SerializeField] private float attackDuration = 0.1f;

        [Tooltip("Dano causado pelo ataque")]
        [SerializeField] private float attackDamage = 1f;

        [Tooltip("Alcance do ataque em unidades")]
        [SerializeField] private float attackRange = 1f;

        [Tooltip("Camadas que podem ser atingidas pelo ataque")]
        [SerializeField] private LayerMask targetLayers = Physics2D.AllLayers;

        [Tooltip("Deslocamento do ponto de origem do ataque")]
        [SerializeField] private Vector2 attackOffset = Vector2.zero;

        [Header("Debug")]
        [Tooltip("Mostra o alcance do ataque na cena")]
        [SerializeField] private bool showAttackRange = true;
        #endregion

        #region Campos Privados
        private Animator animator;
        private PlayerVisualManager visualManager;
        private PlayerAudioManager audioManager;
        private bool isAttacking;
        private static readonly int Attack01 = Animator.StringToHash("Attack01");
        private readonly Collider2D[] attackResults = new Collider2D[8]; // Cache para otimização
        #endregion

        #region Propriedades Públicas
        public bool IsAttacking => isAttacking;
        #endregion

        #region Métodos Unity
        private void Awake()
        {
            animator = GetComponent<Animator>();
            visualManager = GetComponent<PlayerVisualManager>();
            audioManager = GetComponent<PlayerAudioManager>();
        }

        private void OnDrawGizmosSelected()
        {
            if (showAttackRange)
            {
                Gizmos.color = Color.red;
                Vector2 attackPosition = (Vector2)transform.position +
                    (visualManager != null ? GetAttackOffset() : attackOffset);
                Gizmos.DrawWireSphere(attackPosition, attackRange);
            }
        }
        #endregion

        #region Sistema de Combate
        public void TryAttack()
        {
            if (!isAttacking)
            {
                Attack();
            }
        }

        private void Attack()
        {
            isAttacking = true;

            // Sempre ativa o VFX do ataque atual, independente do estado
            GameObject attackVfx = visualManager.GetActiveVfx();
            if (attackVfx != null)
            {
                attackVfx.SetActive(true);
            }

            // Detecta e aplica dano aos alvos
            CheckAndDamageTargets();

            animator.SetTrigger(Attack01);
            audioManager.PlayAttackSound();
        }

        public void OnAttackAnimationEnd()
        {
            isAttacking = false;
            visualManager.DisableAllVfx();
        }
        #endregion

        #region Detecção e Dano
        private void CheckAndDamageTargets()
        {
            // Calcula a posição do ataque baseado na direção do jogador
            Vector2 attackPosition = (Vector2)transform.position + GetAttackOffset();

            // Detecta alvos na área de ataque
            int hitCount = Physics2D.OverlapCircleNonAlloc(
                attackPosition,
                attackRange,
                attackResults,
                targetLayers
            );

            // Aplica dano a todos os alvos detectados
            for (int i = 0; i < hitCount; i++)
            {
                var damageable = attackResults[i].GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage, gameObject);
                }
            }
        }

        private Vector2 GetAttackOffset()
        {
            // Ajusta o offset do ataque baseado na direção que o jogador está olhando
            return visualManager.IsFacingLeft ?
                new Vector2(-attackOffset.x, attackOffset.y) :
                attackOffset;
        }
        #endregion
    }
}
