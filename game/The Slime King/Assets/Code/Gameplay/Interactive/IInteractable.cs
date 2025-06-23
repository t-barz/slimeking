using UnityEngine;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Define um objeto com o qual o jogador pode interagir
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Chamado quando o jogador interage com este objeto
        /// </summary>
        /// <param name="interactor">O GameObject que está interagindo (geralmente o jogador)</param>
        void Interact(GameObject interactor);

        /// <summary>
        /// Verifica se a interação é possível neste momento
        /// </summary>
        bool CanInteract(GameObject interactor);

        /// <summary>
        /// Retorna uma dica de interação para mostrar na UI
        /// </summary>
        string GetInteractionPrompt();

        /// <summary>
        /// Tipo do objeto interativo
        /// </summary>
        InteractionType GetInteractionType();

        /// <summary>
        /// O transform do objeto interativo
        /// </summary>
        Transform GetTransform();
    }

    /// <summary>
    /// Define os tipos de interação possíveis
    /// </summary>
    public enum InteractionType
    {
        /// <summary>Ativa ou desativa um objeto/mecanismo</summary>
        Activate,

        /// <summary>Coleta um item para o inventário</summary>
        Collect,

        /// <summary>Inicia uma conversa</summary>
        Talk,

        /// <summary>Ativa parte de um puzzle</summary>
        Puzzle,

        /// <summary>Ativa um teleporte</summary>
        Portal,

        /// <summary>Objeto que pode ser empurrado/puxado</summary>
        Push
    }
}
