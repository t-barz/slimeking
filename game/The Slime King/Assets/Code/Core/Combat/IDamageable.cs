using UnityEngine;

namespace TheSlimeKing.Core.Combat
{
    /// <summary>
    /// Interface para entidades que podem receber dano
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Aplica dano à entidade
        /// </summary>
        /// <param name="damage">Quantidade de dano bruto</param>
        /// <param name="attacker">Referência ao atacante (opcional)</param>
        /// <param name="hitPoint">Ponto onde ocorreu o impacto (opcional)</param>
        /// <returns>Quantidade de dano real aplicado após defesas</returns>
        int TakeDamage(int damage, GameObject attacker = null, Vector3? hitPoint = null);

        /// <summary>
        /// Verifica se a entidade está morta
        /// </summary>
        bool IsDead();

        /// <summary>
        /// Obtém a saúde atual da entidade
        /// </summary>
        int GetCurrentHealth();

        /// <summary>
        /// Obtém a saúde máxima da entidade
        /// </summary>
        int GetMaxHealth();
    }
}
