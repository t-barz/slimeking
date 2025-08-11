using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.UI.Localization
{
    /// <summary>
    /// Implementação do serviço de localização usando ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizationData", menuName = "SlimeKing/Localization/LocalizationData")]
    public class LocalizationData : ScriptableObject
    {
        [System.Serializable]
        public class TranslationEntry
        {
            public string key;
            public string english;
            public string portuguese;
            public string spanish;
            public string russian;
            public string mandarin;
        }

        public List<TranslationEntry> translations = new();
    }

    /// <summary>
    /// Implementação do serviço de localização
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<SystemLanguage, string>> translationMap = new();
        private SystemLanguage currentLanguage;
        private readonly LocalizationData localizationData;

        public event System.Action OnLanguageChanged;

        public SystemLanguage CurrentLanguage => currentLanguage;

        public LocalizationService(LocalizationData data, bool overrideSystemLanguage = false, SystemLanguage? forcedLanguage = null)
        {
            localizationData = data;
            InitializeTranslations();

            currentLanguage = overrideSystemLanguage && forcedLanguage.HasValue
                ? forcedLanguage.Value
                : Application.systemLanguage;
        }

        private void InitializeTranslations()
        {
            foreach (var entry in localizationData.translations)
            {
                var languageMap = new Dictionary<SystemLanguage, string>
                {
                    { SystemLanguage.English, entry.english },
                    { SystemLanguage.Portuguese, entry.portuguese },
                    { SystemLanguage.Spanish, entry.spanish },
                    { SystemLanguage.Russian, entry.russian },
                    { SystemLanguage.Chinese, entry.mandarin },
                    { SystemLanguage.ChineseSimplified, entry.mandarin },
                    { SystemLanguage.ChineseTraditional, entry.mandarin }
                };

                translationMap[entry.key] = languageMap;
            }
        }

        public string GetLocalizedText(string key)
        {
            if (translationMap.TryGetValue(key, out var languageMap))
            {
                if (languageMap.TryGetValue(currentLanguage, out string translatedText))
                {
                    return translatedText;
                }

                // Fallback para inglês se o idioma atual não tiver tradução
                if (languageMap.TryGetValue(SystemLanguage.English, out string englishText))
                {
                    Debug.LogWarning($"Translation not found for key '{key}' in {currentLanguage}. Using English fallback.");
                    return englishText;
                }
            }

            Debug.LogError($"Translation key '{key}' not found!");
            return key;
        }

        public void SetLanguage(SystemLanguage newLanguage)
        {
            if (currentLanguage != newLanguage)
            {
                currentLanguage = newLanguage;
                OnLanguageChanged?.Invoke();
            }
        }
    }
}
