using System;

namespace SlimeKing.Gameplay.Items
{
    /// <summary>
    /// Interface para itens coletáveis que podem ser adicionados ao ItemPool
    /// </summary>
    public interface ICollectable
    {
        /// <summary>
        /// Evento disparado quando o item for coletado
        /// </summary>
        event Action OnCollected;
    }
}