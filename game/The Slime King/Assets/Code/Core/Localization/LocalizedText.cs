using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Componente para automatizar a localização de elementos de UI
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip("Chave de localização para este texto")]
        [SerializeField] private string _localizationKey;

        [Tooltip("Atualizar automaticamente quando o idioma mudar")]
        [SerializeField] private bool _autoUpdate = true;

        private TextMeshProUGUI _textComponent;

        private void Awake()
        {
            _textComponent = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            UpdateText();
        }

        /// <summary>
        /// Atualiza o texto com a tradução atual
        /// </summary>
        public void UpdateText()
        {
            if (_textComponent != null && !string.IsNullOrEmpty(_localizationKey))
            {
                _textComponent.text = LocalizationManager.Instance.GetLocalizedText(_localizationKey);
            }
        }

        /// <summary>
        /// Define uma nova chave de localização e atualiza o texto
        /// </summary>
        public void SetKey(string key)
        {
            _localizationKey = key;
            UpdateText();
        }
    }
}
