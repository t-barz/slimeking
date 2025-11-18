using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SlimeMec.Systems
{
    /// <summary>
    /// ScriptableObject que armazena dados de diálogo localizados.
    /// Contém múltiplas localizações (idiomas) para um mesmo diálogo.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Slime King/Dialogue/Localized Dialogue Data")]
    [Serializable]
    public class LocalizedDialogueData : ScriptableObject
    {
        /// <summary>
        /// ID único do diálogo.
        /// </summary>
        public string dialogueId;

        /// <summary>
        /// Descrição curta do diálogo (para organização no editor).
        /// </summary>
        public string shortDescription;

        /// <summary>
        /// Lista de localizações disponíveis para este diálogo.
        /// </summary>
        public List<LocalizedText> localizations;

        /// <summary>
        /// Idioma padrão de fallback caso o idioma solicitado não exista.
        /// </summary>
        public string defaultLanguage = "EN";

        public LocalizedDialogueData()
        {
            dialogueId = string.Empty;
            shortDescription = string.Empty;
            localizations = new List<LocalizedText>();
        }

        /// <summary>
        /// Obtém as páginas de diálogo para o idioma especificado.
        /// Se o idioma não existir, retorna o idioma padrão.
        /// </summary>
        /// <param name="languageCode">Código do idioma (ex: "EN", "BR")</param>
        /// <returns>Lista de páginas ou null se não encontrado</returns>
        public List<DialoguePage> GetPages(string languageCode)
        {
            if (localizations == null || localizations.Count == 0)
            {
                Debug.LogWarning($"Dialogue '{dialogueId}' has no localizations");
                return null;
            }

            // Tenta encontrar o idioma solicitado
            LocalizedText localization = localizations.FirstOrDefault(l => l.language == languageCode);

            // Se não encontrou, tenta o idioma padrão
            if (localization == null)
            {
                Debug.LogWarning($"Language '{languageCode}' not found for dialogue '{dialogueId}'. Using default language '{defaultLanguage}'");
                localization = localizations.FirstOrDefault(l => l.language == defaultLanguage);
            }

            // Se ainda não encontrou, usa o primeiro disponível
            if (localization == null)
            {
                Debug.LogWarning($"Default language '{defaultLanguage}' not found for dialogue '{dialogueId}'. Using first available language");
                localization = localizations[0];
            }

            return localization?.pages;
        }

        /// <summary>
        /// Verifica se o diálogo possui localização para o idioma especificado.
        /// </summary>
        /// <param name="languageCode">Código do idioma</param>
        /// <returns>True se o idioma existe</returns>
        public bool HasLanguage(string languageCode)
        {
            return localizations != null && localizations.Any(l => l.language == languageCode);
        }

        /// <summary>
        /// Obtém a lista de idiomas disponíveis para este diálogo.
        /// </summary>
        /// <returns>Lista de códigos de idioma</returns>
        public List<string> GetAvailableLanguages()
        {
            if (localizations == null)
                return new List<string>();

            return localizations.Select(l => l.language).ToList();
        }
    }
}
