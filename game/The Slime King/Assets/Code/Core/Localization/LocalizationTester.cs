using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Componente de teste para o sistema de localização
    /// </summary>
    public class LocalizationTester : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _outputText;
        [SerializeField] private string _testKey = "ui_start_game";

        // Botões para teste
        public void TestGetText()
        {
            if (_outputText != null)
            {
                _outputText.text = $"Key: {_testKey}\nResult: {LocalizationManager.Instance.GetLocalizedText(_testKey)}";
            }
        }

        public void ChangeLanguage(string language)
        {
            LocalizationManager.Instance.SetLanguage(language);
            TestGetText();
        }

        public void GetCurrentLanguage()
        {
            if (_outputText != null)
            {
                _outputText.text = $"Current Language: {LocalizationManager.Instance.GetCurrentLanguage()}";
            }
        }

        public void ListSupportedLanguages()
        {
            if (_outputText != null)
            {
                string languages = string.Join(", ", LocalizationManager.Instance.GetSupportedLanguages());
                _outputText.text = $"Supported Languages: {languages}";
            }
        }
    }
}
