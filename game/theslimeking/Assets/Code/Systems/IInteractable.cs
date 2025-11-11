using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Interface para objetos que podem ser interagidos pelo Player
    /// 
    /// RESPONSABILIDADES:
    /// • Define contrato para interações do Player
    /// • Permite verificar se interação está disponível
    /// • Fornece informações sobre o tipo de interação
    /// 
    /// IMPLEMENTAÇÃO:
    /// • Objetos que implementam esta interface podem ser detectados pelo Player
    /// • O Player irá chamar TryInteract() quando o input de interação for pressionado
    /// • Cada objeto decide sua própria lógica de interação
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Tenta realizar a interação com o objeto
        /// </summary>
        /// <param name="player">Referência do Player que está tentando interagir</param>
        /// <returns>True se a interação foi bem-sucedida, false caso contrário</returns>
        bool TryInteract(Transform player);
        
        /// <summary>
        /// Verifica se a interação está disponível no momento
        /// </summary>
        /// <param name="player">Referência do Player</param>
        /// <returns>True se pode interagir, false caso contrário</returns>
        bool CanInteract(Transform player);
        
        /// <summary>
        /// Texto de prompt para exibir ao Player (opcional)
        /// </summary>
        string GetInteractionPrompt();
        
        /// <summary>
        /// Prioridade da interação (maior valor = maior prioridade)
        /// Útil quando múltiplos objetos interativos estão próximos
        /// </summary>
        int GetInteractionPriority();
    }
}