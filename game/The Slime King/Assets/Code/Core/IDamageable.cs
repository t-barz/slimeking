using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Interface para objetos que podem receber dano
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Método para aplicar dano ao objeto
        /// </summary>
        /// <param name="damage">Valor do dano a ser aplicado</param>
        /// <param name="attacker">Objeto que causou o dano</param>
        /// <returns>Valor do dano realmente aplicado</returns>
        float TakeDamage(int damage, GameObject attacker);
    }
}
