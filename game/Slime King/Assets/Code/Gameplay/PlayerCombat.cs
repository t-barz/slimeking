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
        [SerializeField] private float attackDuration = 0.1f;
        #endregion

        #region Campos Privados
        private Animator animator;
        private PlayerVisualManager visualManager;
        private PlayerAudioManager audioManager;
        private bool isAttacking;
        private static readonly int Attack01 = Animator.StringToHash("Attack01");
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

            animator.SetTrigger(Attack01);
            audioManager.PlayAttackSound();
        }

        public void OnAttackAnimationEnd()
        {
            isAttacking = false;
            visualManager.DisableAllVfx();
        }
        #endregion
    }
}
