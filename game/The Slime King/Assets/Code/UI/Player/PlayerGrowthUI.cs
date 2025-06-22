using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Core;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Exibe o estágio atual de crescimento do slime na UI
    /// </summary>
    public class PlayerGrowthUI : MonoBehaviour
    {
        [Header("Referências UI")]
        [SerializeField] private Image _stageIcon;
        [SerializeField] private TextMeshProUGUI _stageName;
        [SerializeField] private GameObject _growthEffectOverlay;
        [SerializeField] private float _overlayDuration = 2.0f;

        [Header("Ícones de Estágios")]
        [SerializeField] private Sprite _babyIcon;
        [SerializeField] private Sprite _youngIcon;
        [SerializeField] private Sprite _adultIcon;
        [SerializeField] private Sprite _kingIcon;

        private SlimeStage _currentStage = SlimeStage.Baby;
        private float _overlayTimer = 0f;
        private bool _showingOverlay = false;

        private void Start()
        {
            // Encontra o sistema de crescimento e registra nos eventos
            if (PlayerGrowth.Instance != null)
            {
                PlayerGrowth.Instance.OnGrowthStageChanged.AddListener(OnGrowthStageChanged);
                PlayerGrowth.Instance.OnGrowthStarted.AddListener(OnGrowthStarted);
                PlayerGrowth.Instance.OnGrowthCompleted.AddListener(OnGrowthCompleted);

                // Inicializa com o estágio atual
                SlimeStage currentStage = PlayerGrowth.Instance.GetCurrentStage();
                UpdateStageUI(currentStage);
            }
            else
            {
                Debug.LogWarning("PlayerGrowthUI: PlayerGrowth instance not found.");
            }

            // Inicializa overlay como desativado
            if (_growthEffectOverlay != null)
            {
                _growthEffectOverlay.SetActive(false);
            }
        }

        private void Update()
        {
            // Controle de temporizador para o overlay de crescimento
            if (_showingOverlay)
            {
                _overlayTimer -= Time.deltaTime;

                if (_overlayTimer <= 0f)
                {
                    if (_growthEffectOverlay != null)
                    {
                        _growthEffectOverlay.SetActive(false);
                    }
                    _showingOverlay = false;
                }
            }
        }

        private void OnDestroy()
        {
            // Desregistra dos eventos
            if (PlayerGrowth.Instance != null)
            {
                PlayerGrowth.Instance.OnGrowthStageChanged.RemoveListener(OnGrowthStageChanged);
                PlayerGrowth.Instance.OnGrowthStarted.RemoveListener(OnGrowthStarted);
                PlayerGrowth.Instance.OnGrowthCompleted.RemoveListener(OnGrowthCompleted);
            }
        }

        /// <summary>
        /// Atualiza os elementos da UI com base no estágio atual
        /// </summary>
        private void UpdateStageUI(SlimeStage stage)
        {
            _currentStage = stage;

            // Atualiza ícone
            if (_stageIcon != null)
            {
                switch (stage)
                {
                    case SlimeStage.Baby:
                        _stageIcon.sprite = _babyIcon;
                        break;
                    case SlimeStage.Young:
                        _stageIcon.sprite = _youngIcon;
                        break;
                    case SlimeStage.Adult:
                        _stageIcon.sprite = _adultIcon;
                        break;
                    case SlimeStage.King:
                        _stageIcon.sprite = _kingIcon;
                        break;
                }
            }

            // Atualiza nome do estágio
            if (_stageName != null)
            {
                _stageName.text = GetStageName(stage);
            }
        }

        /// <summary>
        /// Retorna o nome amigável de exibição para o estágio
        /// </summary>
        private string GetStageName(SlimeStage stage)
        {
            switch (stage)
            {
                case SlimeStage.Baby:
                    return "Baby Slime";
                case SlimeStage.Young:
                    return "Young Slime";
                case SlimeStage.Adult:
                    return "Adult Slime";
                case SlimeStage.King:
                    return "Slime King";
                default:
                    return stage.ToString();
            }
        }

        /// <summary>
        /// Handler de evento quando o estágio muda
        /// </summary>
        private void OnGrowthStageChanged(SlimeStage newStage)
        {
            UpdateStageUI(newStage);
        }

        /// <summary>
        /// Handler de evento quando o crescimento inicia
        /// </summary>
        private void OnGrowthStarted()
        {
            // Ativa overlay
            if (_growthEffectOverlay != null)
            {
                _growthEffectOverlay.SetActive(true);
                _showingOverlay = true;
                _overlayTimer = _overlayDuration;
            }
        }

        /// <summary>
        /// Handler de evento quando o crescimento é concluído
        /// </summary>
        private void OnGrowthCompleted()
        {
            // Opcional: Adicionar algum efeito de conclusão
        }
    }
}
