using System;
using UnityEngine;
using TMPro;
using System.Globalization;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Gerenciador de linguagem para os menus do jogo.
    /// </summary>
    public class LanguageMenu : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TMP_Dropdown _languageDropdown;

        [Header("Settings")]
        [SerializeField] private bool _populateOnStart = true;

        private void Start()
        {
            if (_populateOnStart)
            {
                PopulateLanguageDropdown();
                SetDropdownValue();
            }
        }

        /// <summary>
        /// Popula o dropdown com os idiomas suportados
        /// </summary>
        public void PopulateLanguageDropdown()
        {
            if (_languageDropdown == null) return;

            _languageDropdown.ClearOptions();

            var languages = TheSlimeKing.Core.LocalizationManager.Instance.GetSupportedLanguages();

            foreach (var lang in languages)
            {
                try
                {
                    // Tenta obter o nome nativo do idioma
                    string langName = GetLanguageNativeName(lang);
                    _languageDropdown.options.Add(new TMP_Dropdown.OptionData(langName));
                }
                catch
                {
                    // Fallback para o código do idioma
                    _languageDropdown.options.Add(new TMP_Dropdown.OptionData(lang));
                }
            }

            _languageDropdown.RefreshShownValue();
        }

        /// <summary>
        /// Define o valor do dropdown com base no idioma atual
        /// </summary>
        public void SetDropdownValue()
        {
            if (_languageDropdown == null) return;

            var languages = TheSlimeKing.Core.LocalizationManager.Instance.GetSupportedLanguages();
            string currentLanguage = TheSlimeKing.Core.LocalizationManager.Instance.GetCurrentLanguage();

            for (int i = 0; i < languages.Length; i++)
            {
                if (languages[i] == currentLanguage)
                {
                    _languageDropdown.value = i;
                    _languageDropdown.RefreshShownValue();
                    return;
                }
            }
        }

        /// <summary>
        /// Callback para quando o idioma é alterado no dropdown
        /// </summary>
        public void OnLanguageChanged(int index)
        {
            var languages = TheSlimeKing.Core.LocalizationManager.Instance.GetSupportedLanguages();

            if (index >= 0 && index < languages.Length)
            {
                TheSlimeKing.Core.LocalizationManager.Instance.SetLanguage(languages[index]);

                // Atualiza todos os textos localizados na cena
                var localizedTexts = FindObjectsByType<TheSlimeKing.Core.LocalizedText>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                foreach (var text in localizedTexts)
                {
                    text.UpdateText();
                }
            }
        }

        /// <summary>
        /// Obtém o nome nativo do idioma
        /// </summary>
        private string GetLanguageNativeName(string langCode)
        {
            try
            {
                // Processa códigos especiais
                if (langCode == "EN") langCode = "en-US";
                else if (langCode == "PT_BR") langCode = "pt-BR";
                else if (langCode == "ZH_CN") langCode = "zh-CN";
                else langCode = langCode.ToLower();

                // Converte para CultureInfo e obtém o nome nativo
                CultureInfo cultureInfo = new CultureInfo(langCode);
                return cultureInfo.NativeName;
            }
            catch
            {
                // Fallback para o código original
                return langCode;
            }
        }
    }
}
