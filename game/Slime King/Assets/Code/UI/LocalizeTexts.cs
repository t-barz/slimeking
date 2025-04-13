using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Gerencia a localização de textos em múltiplos idiomas usando TextMeshPro.
/// </summary>
public class LocalizeTexts : MonoBehaviour
{
    [System.Serializable]
    public class LocalizedText
    {
        public string english;
        public string portuguese;
        public string spanish;
        public string russian;
        public string mandarin;
    }

    [Header("Configurações")]
    [Tooltip("Texto em diferentes idiomas")]
    [SerializeField] private LocalizedText localizedText;

    [Tooltip("Sobrescrever idioma do sistema")]
    [SerializeField] private bool overrideSystemLanguage = false;
    
    [Tooltip("Idioma forçado (se override ativo)")]
    [SerializeField] private SystemLanguage forcedLanguage = SystemLanguage.English;

    private TextMeshProUGUI tmpText;
    private Dictionary<SystemLanguage, string> languageMap;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("TextMeshProUGUI component not found!");
            enabled = false;
            return;
        }

        InitializeLanguageMap();
        UpdateText();
    }

    private void InitializeLanguageMap()
    {
        languageMap = new Dictionary<SystemLanguage, string>
        {
            { SystemLanguage.English, localizedText.english },
            { SystemLanguage.Portuguese, localizedText.portuguese },
            { SystemLanguage.Spanish, localizedText.spanish },
            { SystemLanguage.Russian, localizedText.russian },
            { SystemLanguage.Chinese, localizedText.mandarin },
            { SystemLanguage.ChineseSimplified, localizedText.mandarin },
            { SystemLanguage.ChineseTraditional, localizedText.mandarin }
        };
    }

    private void UpdateText()
    {
        SystemLanguage currentLanguage = overrideSystemLanguage 
            ? forcedLanguage 
            : Application.systemLanguage;

        if (languageMap.TryGetValue(currentLanguage, out string translatedText))
        {
            tmpText.text = translatedText;
        }
        else
        {
            // Fallback para inglês se o idioma não for suportado
            tmpText.text = localizedText.english;
            Debug.LogWarning($"Language {currentLanguage} not supported. Using English as fallback.");
        }
    }

    // Editor method to update text when values change in inspector
    private void OnValidate()
    {
        if (Application.isPlaying && tmpText != null)
        {
            UpdateText();
        }
    }
}