using UnityEngine;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Interface para controle do jogador durante movimento especial
    /// </summary>
    public interface IPlayerController
    {
        /// <summary>
        /// Desativa temporariamente o controle do jogador
        /// </summary>
        void DisableControl();

        /// <summary>
        /// Reativa o controle do jogador
        /// </summary>
        void EnableControl();

        /// <summary>
        /// Move o jogador para uma posição específica
        /// </summary>
        /// <param name="position">Posição de destino</param>
        /// <param name="immediate">Se verdadeiro, teleporta instantaneamente</param>
        void MoveToPosition(Vector2 position, bool immediate = false);

        /// <summary>
        /// Define a escala visual do jogador
        /// </summary>
        /// <param name="scale">Nova escala (1.0 = tamanho normal)</param>
        void SetScale(float scale);

        /// <summary>
        /// Atualiza a direção visual do jogador
        /// </summary>
        /// <param name="direction">Nova direção</param>
        void SetDirection(Vector2 direction);

        /// <summary>
        /// Obtém a posição atual do jogador
        /// </summary>
        Vector2 GetPosition();

        /// <summary>
        /// Obtém a direção atual do jogador
        /// </summary>
        Vector2 GetDirection();
    }
}
