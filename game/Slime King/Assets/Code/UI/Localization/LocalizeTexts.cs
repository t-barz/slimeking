using UnityEngine;
using TMPro;
using UnityEngine.UI;
using SlimeKing.UI.Localization;

namespace SlimeKing.UI.Localization
{
    /// <summary>
    /// Componente para localização de textos em TextMeshPro UI
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizeTexts : MonoBehaviour
    {
        [Header("Configurações de Localização")]
        [Tooltip("Chave do texto no sistema de localização")]
        [SerializeField] private string textKey;

        private TextMeshProUGUI tmpText;
        private ILocalizationService localizationService;

        private void Awake()
        {
            tmpText = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            if (localizationService != null)
            {
                localizationService.OnLanguageChanged += UpdateText;
            }
        }

        private void OnDisable()
        {
            if (localizationService != null)
            {
                localizationService.OnLanguageChanged -= UpdateText;
            }
        }
        private void Start()
        {
            // Usando o novo método recomendado do Unity 6
            var manager = Object.FindFirstObjectByType<LocalizationManager>();
            if (manager != null)
            {
                localizationService = manager.LocalizationService;
                if (localizationService != null)
                {
                    localizationService.OnLanguageChanged += UpdateText;
                    UpdateText();
                }
                else
                {
                    Debug.LogError($"[LocalizeTexts] LocalizationService not initialized in LocalizationManager!", this);
                    enabled = false;
                }
            }
            else
            {
                Debug.LogError($"[LocalizeTexts] No LocalizationManager found in scene! Make sure to add one to your scene.", this);
                enabled = false;
            }
        }

        private void UpdateText()
        {
            if (string.IsNullOrEmpty(textKey))
            {
                Debug.LogWarning("Text key is empty!", this);
                return;
            }

            if (tmpText != null && localizationService != null)
            {
                tmpText.text = localizationService.GetLocalizedText(textKey);
            }
        }

        // Editor method to preview text changes
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                tmpText = GetComponent<TextMeshProUGUI>();
                if (tmpText != null && !string.IsNullOrEmpty(textKey))
                {
                    tmpText.text = $"[{textKey}]";
                }
            }
            else if (localizationService != null)
            {
                UpdateText();
            }
        }
#endif
    }
}