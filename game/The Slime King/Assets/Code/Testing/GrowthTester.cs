using UnityEngine;
using UnityEngine.UI;
using TheSlimeKing.Core;
using TMPro;

namespace TheSlimeKing.Testing
{
    /// <summary>
    /// Ferramenta de teste para o sistema de crescimento
    /// </summary>
    public class GrowthTester : MonoBehaviour
    {
        [Header("Interface de Teste")]
        [SerializeField] private Button _babyButton;
        [SerializeField] private Button _youngButton;
        [SerializeField] private Button _adultButton;
        [SerializeField] private Button _kingButton;
        [SerializeField] private TextMeshProUGUI _currentStageText;

        [Header("Debug")]
        [SerializeField] private bool _showDebugLogs = true;

        private PlayerGrowth _playerGrowth;
        private int _debugCounter = 0;

        private void Start()
        {
            // Encontra referência ao sistema de crescimento
            _playerGrowth = FindFirstObjectByType<PlayerGrowth>();

            if (_playerGrowth == null)
            {
                Debug.LogError("GrowthTester: PlayerGrowth não encontrado na cena!");
                return;
            }

            // Configura os botões
            SetupButtons();

            // Registra evento para atualizar UI
            _playerGrowth.OnGrowthStageChanged.AddListener(UpdateUI);

            // Atualiza UI inicial
            UpdateUI(_playerGrowth.GetCurrentStage());

            // Debug
            LogMessage("GrowthTester iniciado");
        }

        private void OnDestroy()
        {
            if (_playerGrowth != null)
            {
                _playerGrowth.OnGrowthStageChanged.RemoveListener(UpdateUI);
            }
        }

        /// <summary>
        /// Configura os listeners dos botões de teste
        /// </summary>
        private void SetupButtons()
        {
            if (_babyButton != null)
                _babyButton.onClick.AddListener(() => ForceGrowth(SlimeStage.Baby));

            if (_youngButton != null)
                _youngButton.onClick.AddListener(() => ForceGrowth(SlimeStage.Young));

            if (_adultButton != null)
                _adultButton.onClick.AddListener(() => ForceGrowth(SlimeStage.Adult));

            if (_kingButton != null)
                _kingButton.onClick.AddListener(() => ForceGrowth(SlimeStage.King));
        }

        /// <summary>
        /// Força o crescimento para um estágio específico
        /// </summary>
        private void ForceGrowth(SlimeStage targetStage)
        {
            if (_playerGrowth != null)
            {
                LogMessage($"Forçando crescimento para estágio: {targetStage}");
                _playerGrowth.ForceGrowth(targetStage);
            }
        }

        /// <summary>
        /// Atualiza a interface com o estágio atual
        /// </summary>
        private void UpdateUI(SlimeStage currentStage)
        {
            if (_currentStageText != null)
            {
                SlimeGrowthStage config = _playerGrowth.GetCurrentStageConfig();
                string stageName = config != null ? config.StageName : currentStage.ToString();

                _currentStageText.text = $"Estágio: {stageName}\n({currentStage})";
            }
        }

        /// <summary>
        /// Registra mensagem de debug
        /// </summary>
        private void LogMessage(string message)
        {
            if (!_showDebugLogs) return;

            _debugCounter++;
            Debug.Log($"[GrowthTest #{_debugCounter}] {message}");
        }
    }
}
