using UnityEngine;

namespace TheSlimeKing.Gameplay.Interfaces
{
    /// <summary>
    /// Interface para objetos interagíveis pelo player.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Executa a interação com o player.
        /// </summary>
        /// <param name="player">Referência ao PlayerController que interagiu</param>
        void Interact(GameObject player);
    }
}