using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace TheSlimeKing.Core.Elemental.UI
{
    /// <summary>
    /// Interface de usuário para exibição da energia elemental
    /// </summary>
    public class ElementalEnergyUI : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private ElementalEnergyManager _energyManager;

        [Header("Barras de Energia")]
        [SerializeField] private ElementalEnergyBar _earthBar;
        [SerializeField] private ElementalEnergyBar _waterBar;
        [SerializeField] private ElementalEnergyBar _fireBar;
        [SerializeField] private ElementalEnergyBar _airBar;

        [Header("Configurações de Animação")]
        [SerializeField] private float _updateSpeed = 0.5f;
        [SerializeField] private float _gainPulseDuration = 0.3f;
        [SerializeField] private float _gainPulseScale = 1.15f;
        [SerializeField] private float _usePulseDuration = 0.3f;
        [SerializeField] private float _usePulseScale = 0.85f;

        [Header("Efeitos")]
        [SerializeField] private GameObject _energyGainEffectPrefab;
        [SerializeField] private GameObject _energyUseEffectPrefab;

        private void Awake()
        {
            // Busca o energy manager se não estiver definido
            if (_energyManager == null)
                _energyManager = FindObjectOfType<ElementalEnergyManager>();

            // Configura as barras
            SetupBars();
        }

        private void OnEnable()
        {
            // Registra eventos
            if (_energyManager != null)
            {
                _energyManager.OnEnergyAdded.AddListener(HandleEnergyAdded);
                _energyManager.OnEnergyUsed.AddListener(HandleEnergyUsed);
                _energyManager.OnEnergyUpdated.AddListener(HandleEnergyUpdated);
            }
        }

        private void OnDisable()
        {
            // Remove eventos
            if (_energyManager != null)
            {
                _energyManager.OnEnergyAdded.RemoveListener(HandleEnergyAdded);
                _energyManager.OnEnergyUsed.RemoveListener(HandleEnergyUsed);
                _energyManager.OnEnergyUpdated.RemoveListener(HandleEnergyUpdated);
            }
        }

        private void Start()
        {
            // Atualiza UI com valores iniciais
            UpdateAllBars(false);
        }

        /// <summary>
        /// Configura referências às barras e verifica se estão presentes
        /// </summary>
        private void SetupBars()
        {
            // Se as barras não estiverem definidas no Inspector, tenta encontrá-las
            if (_earthBar == null)
                _earthBar = transform.Find("EarthEnergyBar")?.GetComponent<ElementalEnergyBar>();

            if (_waterBar == null)
                _waterBar = transform.Find("WaterEnergyBar")?.GetComponent<ElementalEnergyBar>();

            if (_fireBar == null)
                _fireBar = transform.Find("FireEnergyBar")?.GetComponent<ElementalEnergyBar>();

            if (_airBar == null)
                _airBar = transform.Find("AirEnergyBar")?.GetComponent<ElementalEnergyBar>();
        }

        /// <summary>
        /// Trata o evento de adição de energia
        /// </summary>
        private void HandleEnergyAdded(ElementalType type, int amount)
        {
            // Atualiza a barra correspondente com animação
            UpdateBar(type, true, true);
        }

        /// <summary>
        /// Trata o evento de uso de energia
        /// </summary>
        private void HandleEnergyUsed(ElementalType type, int amount)
        {
            // Atualiza a barra correspondente com animação
            UpdateBar(type, true, false);
        }

        /// <summary>
        /// Trata o evento de atualização geral de energia
        /// </summary>
        private void HandleEnergyUpdated()
        {
            // Atualiza todas as barras sem animação
            UpdateAllBars(false);
        }

        /// <summary>
        /// Atualiza uma barra específica com valores do ElementalEnergyManager
        /// </summary>
        private void UpdateBar(ElementalType type, bool animate, bool isGain)
        {
            if (_energyManager == null)
                return;

            ElementalEnergyBar bar = GetBarForType(type);

            if (bar == null)
                return;

            // Obtém valor atual e percentual
            int currentValue = _energyManager.GetElementalEnergy(type);
            float fillAmount = _energyManager.GetElementalEnergyPercentage(type);

            // Atualiza a barra
            if (animate)
            {
                bar.AnimateToValue(currentValue, fillAmount, _updateSpeed);

                // Aplica efeito de pulse
                if (isGain)
                    PulseBar(bar.gameObject, _gainPulseDuration, _gainPulseScale);
                else
                    PulseBar(bar.gameObject, _usePulseDuration, _usePulseScale);
            }
            else
            {
                bar.SetValue(currentValue, fillAmount);
            }
        }

        /// <summary>
        /// Atualiza todas as barras de energia
        /// </summary>
        private void UpdateAllBars(bool animate)
        {
            UpdateBar(ElementalType.Earth, animate, false);
            UpdateBar(ElementalType.Water, animate, false);
            UpdateBar(ElementalType.Fire, animate, false);
            UpdateBar(ElementalType.Air, animate, false);
        }

        /// <summary>
        /// Aplica efeito de pulse na barra especificada
        /// </summary>
        private void PulseBar(GameObject barObject, float duration, float scale)
        {
            if (barObject == null)
                return;

            // Sequência de animação: escala aumenta e depois retorna
            barObject.transform.DOScale(scale, duration / 2f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    barObject.transform.DOScale(1f, duration / 2f)
                        .SetEase(Ease.InQuad);
                });
        }

        /// <summary>
        /// Retorna a barra correspondente ao tipo elemental
        /// </summary>
        private ElementalEnergyBar GetBarForType(ElementalType type)
        {
            switch (type)
            {
                case ElementalType.Earth:
                    return _earthBar;
                case ElementalType.Water:
                    return _waterBar;
                case ElementalType.Fire:
                    return _fireBar;
                case ElementalType.Air:
                    return _airBar;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Componente para uma barra de energia elemental individual
    /// </summary>
    [System.Serializable]
    public class ElementalEnergyBar : MonoBehaviour
    {
        [Header("Referencias UI")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Image _iconImage;

        [Header("Configurações")]
        [SerializeField] private ElementalType _elementType;
        [SerializeField] private Color _barColor = Color.white;
        [SerializeField] private bool _showValueAsText = true;

        /// <summary>
        /// Define o valor e preenchimento da barra instantaneamente
        /// </summary>
        public void SetValue(int value, float fillAmount)
        {
            // Atualiza a barra de preenchimento
            if (_fillImage != null)
                _fillImage.fillAmount = fillAmount;

            // Atualiza texto se necessário
            if (_valueText != null && _showValueAsText)
                _valueText.text = value.ToString();
        }

        /// <summary>
        /// Anima a barra até o valor especificado
        /// </summary>
        public void AnimateToValue(int value, float fillAmount, float duration)
        {
            // Anima a barra de preenchimento
            if (_fillImage != null)
                _fillImage.DOFillAmount(fillAmount, duration);

            // Atualiza texto se necessário
            if (_valueText != null && _showValueAsText)
                _valueText.text = value.ToString();
        }

        /// <summary>
        /// Configura a cor da barra
        /// </summary>
        public void SetColor(Color color)
        {
            _barColor = color;

            if (_fillImage != null)
                _fillImage.color = color;
        }

        /// <summary>
        /// Define o ícone da barra
        /// </summary>
        public void SetIcon(Sprite icon)
        {
            if (_iconImage != null)
                _iconImage.sprite = icon;
        }
    }
}
