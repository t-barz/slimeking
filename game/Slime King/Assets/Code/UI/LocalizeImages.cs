using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Gerencia a localização de imagens em múltiplos idiomas.
/// </summary>
public class LocalizeImages : MonoBehaviour
{
    [System.Serializable]
    public class LocalizedImage
    {
        [Tooltip("Imagem para inglês")]
        public Sprite english;
        
        [Tooltip("Imagem para português")]
        public Sprite portuguese;
        
        [Tooltip("Imagem para espanhol")]
        public Sprite spanish;
        
        [Tooltip("Imagem para russo")]
        public Sprite russian;
        
        [Tooltip("Imagem para chinês (simplificado e tradicional)")]
        public Sprite mandarin;
    }

    [Header("Configurações")]
    [Tooltip("Imagens para diferentes idiomas")]
    [SerializeField] private LocalizedImage localizedImage;

    [Tooltip("Sobrescrever idioma do sistema")]
    [SerializeField] private bool overrideSystemLanguage = false;
    
    [Tooltip("Idioma forçado (se override ativo)")]
    [SerializeField] private SystemLanguage forcedLanguage = SystemLanguage.English;

    private Image imageComponent;
    private Dictionary<SystemLanguage, Sprite> languageMap;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("Image component not found!");
            enabled = false;
            return;
        }

        InitializeLanguageMap();
        UpdateImage();
    }

    private void InitializeLanguageMap()
    {
        languageMap = new Dictionary<SystemLanguage, Sprite>
        {
            { SystemLanguage.English, localizedImage.english },
            { SystemLanguage.Portuguese, localizedImage.portuguese },
            { SystemLanguage.Spanish, localizedImage.spanish },
            { SystemLanguage.Russian, localizedImage.russian },
            { SystemLanguage.Chinese, localizedImage.mandarin },
            { SystemLanguage.ChineseSimplified, localizedImage.mandarin },
            { SystemLanguage.ChineseTraditional, localizedImage.mandarin }
        };
    }

    private void UpdateImage()
    {
        SystemLanguage currentLanguage = overrideSystemLanguage 
            ? forcedLanguage 
            : Application.systemLanguage;

        // Tratamento especial para variantes do chinês
        if (currentLanguage == SystemLanguage.Chinese)
        {
            currentLanguage = SystemLanguage.ChineseSimplified;
        }

        if (languageMap.TryGetValue(currentLanguage, out Sprite localizedSprite))
        {
            imageComponent.sprite = localizedSprite;
        }
        else
        {
            // Fallback para inglês se o idioma não for suportado
            imageComponent.sprite = localizedImage.english;
            Debug.LogWarning($"Language {currentLanguage} not supported. Using English sprite as fallback.");
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying && imageComponent != null)
        {
            UpdateImage();
        }
    }
}