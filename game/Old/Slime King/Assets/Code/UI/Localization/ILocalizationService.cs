using UnityEngine;

namespace SlimeKing.UI.Localization
{
    /// <summary>
    /// Interface para o serviço de localização
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Evento disparado quando o idioma é alterado
        /// </summary>
        event System.Action OnLanguageChanged;

        /// <summary>
        /// Obtém o texto localizado para a chave especificada
        /// </summary>
        /// <param name="key">Chave do texto no sistema de localização</param>
        /// <returns>Texto traduzido ou a chave se não encontrado</returns>
        string GetLocalizedText(string key);

        /// <summary>
        /// Idioma atual do sistema
        /// </summary>
        SystemLanguage CurrentLanguage { get; }
    }
}
