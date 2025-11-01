using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Códigos de idioma suportados pelo sistema de diálogos.
    /// </summary>
    public enum LanguageCode
    {
        BR, // Português Brasil
        EN, // Inglês
        ES, // Espanhol
        CH, // Chinês
        RS, // Russo
        FR, // Francês
        IT, // Italiano
        DT, // Alemão
        JP, // Japonês
        KR  // Coreano
    }

    /// <summary>
    /// Representa uma página individual de diálogo.
    /// </summary>
    [System.Serializable]
    public class DialoguePage
    {
        [Tooltip("Texto da página de diálogo")]
        [TextArea(3, 6)]
        public string text;
    }

    /// <summary>
    /// Representa o texto localizado para um idioma específico.
    /// </summary>
    [System.Serializable]
    public class LocalizedText
    {
        [Tooltip("Código do idioma (BR, EN, ES, etc.)")]
        public string language;

        [Tooltip("Lista de páginas de diálogo para este idioma")]
        public List<DialoguePage> pages = new List<DialoguePage>();
    }

    /// <summary>
    /// ScriptableObject que armazena dados de diálogo localizados.
    /// Suporta múltiplos idiomas e paginação de texto.
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizedDialogueData", menuName = "Game/Localized Dialogue Data")]
    public class LocalizedDialogueData : ScriptableObject
    {
        [Header("Dialogue Identification")]
        [Tooltip("ID único do diálogo")]
        public string dialogueId;

        [Tooltip("Descrição curta do diálogo (para referência no editor)")]
        public string shortDescription;

        [Header("Localized Content")]
        [Tooltip("Lista de textos localizados para diferentes idiomas")]
        public List<LocalizedText> localizations = new List<LocalizedText>();

        /// <summary>
        /// Obtém as páginas de diálogo para o idioma especificado.
        /// Implementa lógica de fallback: idioma solicitado -> EN -> primeiro disponível.
        /// </summary>
        /// <param name="languageCode">Código do idioma desejado (ex: "BR", "EN")</param>
        /// <returns>Lista de páginas de diálogo ou lista vazia se nenhum idioma disponível</returns>
        public List<DialoguePage> GetPages(string languageCode)
        {
            if (localizations == null || localizations.Count == 0)
            {
                Debug.LogWarning($"[LocalizedDialogueData] No localizations found for dialogue '{dialogueId}'");
                return new List<DialoguePage>();
            }

            // Tenta encontrar o idioma solicitado
            var localization = localizations.FirstOrDefault(l => 
                l.language.Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));

            if (localization != null && localization.pages != null)
            {
                return localization.pages;
            }

            // Fallback para inglês (EN)
            Debug.LogWarning($"[LocalizedDialogueData] Language '{languageCode}' not found for dialogue '{dialogueId}'. Trying fallback to EN.");
            localization = localizations.FirstOrDefault(l => 
                l.language.Equals("EN", System.StringComparison.OrdinalIgnoreCase));

            if (localization != null && localization.pages != null)
            {
                return localization.pages;
            }

            // Fallback para o primeiro idioma disponível
            Debug.LogWarning($"[LocalizedDialogueData] EN not found for dialogue '{dialogueId}'. Using first available language.");
            var firstLocalization = localizations.FirstOrDefault(l => l.pages != null && l.pages.Count > 0);

            if (firstLocalization != null)
            {
                return firstLocalization.pages;
            }

            // Nenhum idioma disponível
            Debug.LogError($"[LocalizedDialogueData] No valid localizations found for dialogue '{dialogueId}'");
            return new List<DialoguePage>();
        }

        /// <summary>
        /// Obtém as páginas de diálogo usando um enum LanguageCode.
        /// </summary>
        /// <param name="languageCode">Enum do código de idioma</param>
        /// <returns>Lista de páginas de diálogo</returns>
        public List<DialoguePage> GetPages(LanguageCode languageCode)
        {
            return GetPages(languageCode.ToString());
        }

        /// <summary>
        /// Verifica se o diálogo possui localização para o idioma especificado.
        /// </summary>
        /// <param name="languageCode">Código do idioma</param>
        /// <returns>True se o idioma está disponível</returns>
        public bool HasLanguage(string languageCode)
        {
            return localizations != null && 
                   localizations.Any(l => l.language.Equals(languageCode, System.StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Obtém a lista de idiomas disponíveis para este diálogo.
        /// </summary>
        /// <returns>Lista de códigos de idioma disponíveis</returns>
        public List<string> GetAvailableLanguages()
        {
            if (localizations == null)
                return new List<string>();

            return localizations.Select(l => l.language).ToList();
        }
    }
}
