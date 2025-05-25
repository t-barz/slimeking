using UnityEngine;

namespace SlimeKing.UI.Localization
{
    /// <summary>
    /// Gerencia o serviço de localização na cena
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        [Header("Configurações")]
        [SerializeField] private LocalizationData localizationData;
        [SerializeField] private bool overrideSystemLanguage;
        [SerializeField] private SystemLanguage forcedLanguage = SystemLanguage.English;

        public ILocalizationService LocalizationService { get; private set; }

        private void Awake()
        {
            if (localizationData == null)
            {
                Debug.LogError("LocalizationData not assigned!", this);
                enabled = false;
                return;
            }

            LocalizationService = new LocalizationService(localizationData, overrideSystemLanguage, forcedLanguage);
        }

        public void SetLanguage(SystemLanguage language)
        {
            (LocalizationService as LocalizationService)?.SetLanguage(language);
        }
    }
}
