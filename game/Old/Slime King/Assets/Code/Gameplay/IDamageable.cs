using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Interface para objetos que podem receber dano
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Aplica dano ao objeto
        /// </summary>
        /// <param name="damage">Quantidade de dano a ser aplicada</param>
        /// <param name="source">Objeto que causou o dano (opcional)</param>
        /// <returns>True se o dano foi aplicado com sucesso</returns>
        bool TakeDamage(float damage, GameObject source = null);

        /// <summary>
        /// Pontos de vida/resistência atual
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// Pontos de vida/resistência máxima
        /// </summary>
        float MaxHealth { get; }
    }
}
