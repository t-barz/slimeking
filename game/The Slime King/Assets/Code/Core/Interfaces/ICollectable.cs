using System;
using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Interface para objetos que podem ser coletados pelo jogador
    /// </summary>
    public interface ICollectable
    {
        /// <summary>
        /// Evento disparado quando o item é coletado
        /// </summary>
        event Action OnCollected;

        /// <summary>
        /// Método chamado quando o item é coletado pelo jogador
        /// </summary>
        /// <param name="collector">O GameObject que coletou o item</param>
        void Collect(GameObject collector);
    }
}