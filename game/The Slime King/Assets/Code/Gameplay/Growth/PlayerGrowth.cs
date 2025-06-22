using UnityEngine;
using System.Collections;
using System;
using TheSlimeKing.Core.Elemental;
using UnityEngine.Events;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Gerencia o crescimento do Slime baseado na energia elemental acumulada
    /// </summary>
    public class PlayerGrowth : MonoBehaviour
    {
        #region Singleton
        private static PlayerGrowth _instance;
        public static PlayerGrowth Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<PlayerGrowth>();
                    if (_instance == null)
                    {
                        Debug.LogError("Nenhum PlayerGrowth encontrado na cena!");
                    }
                }
                return _instance;
            }
        }
        #endregion

        [Header("Configurações de Crescimento")]
        [SerializeField] private SlimeGrowthStage[] _growthStages;
        [SerializeField] private float _growthTransitionDuration = 2.0f;
        [SerializeField] private AnimationCurve _growthCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Feedback")]
        [SerializeField] private ParticleSystem _defaultGrowthParticles;
        [SerializeField] private AudioClip _defaultGrowthSound;
        [SerializeField] private GameObject _screenFlashEffect;

        [Header("Referências")]
        [SerializeField] private Transform _slimeTransform;
        [SerializeField] private SpriteRenderer _slimeRenderer;
        [SerializeField] private Animator _slimeAnimator;

        // Eventos
        [System.Serializable]
        public class GrowthEvent : UnityEvent<SlimeStage> { }

        public GrowthEvent OnGrowthStageChanged = new GrowthEvent();
        public UnityEvent OnGrowthStarted = new UnityEvent();
        public UnityEvent OnGrowthCompleted = new UnityEvent();

        // Estado atual
        private SlimeStage _currentStage = SlimeStage.Baby;
        private bool _isGrowing = false;
        private int _currentStageIndex = 0;

        private void Awake()
        {
            // Garantir singleton
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Validar configuração
            ValidateGrowthStages();
        }

        private void Start()
        {
            // Registrar no evento de threshold
            ElementalEvents.OnElementalThresholdReached += HandleElementalThreshold;

            // Inicializar com o primeiro estágio
            ApplyStageProperties(GetCurrentStageConfig());
        }

        private void OnDestroy()
        {
            // Desregistrar do evento
            ElementalEvents.OnElementalThresholdReached -= HandleElementalThreshold;
        }

        /// <summary>
        /// Manipula o evento de threshold elemental atingido
        /// </summary>
        private void HandleElementalThreshold(int stage)
        {
            EvaluateGrowthStage(stage);
        }

        /// <summary>
        /// Avalia se um crescimento deve ocorrer baseado no estágio atual e na energia elemental
        /// </summary>
        public void EvaluateGrowthStage(int stage = -1)
        {
            if (_isGrowing) return;

            // Se nenhum estágio for especificado, obtém do sistema elemental
            if (stage < 0)
            {
                try
                {
                    // Usa a mesma lógica do ElementalEnergyManager para obter o estágio
                    if (ElementalEnergyManager.Instance != null)
                    {
                        stage = ElementalEnergyManager.Instance.GetCurrentGrowthStage();
                    }
                    else
                    {
                        Debug.LogError("Não foi possível obter o estágio de crescimento: ElementalEnergyManager não encontrado");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Erro ao avaliar estágio de crescimento: {e.Message}");
                    return;
                }
            }

            // Verifica se deve crescer
            if (stage > (int)_currentStage)
            {
                int targetStageIndex = Mathf.Min(stage, _growthStages.Length - 1);
                SlimeStage newStage = (SlimeStage)targetStageIndex;

                // Inicia transição de crescimento
                StartGrowthTransition(newStage);
            }
        }

        /// <summary>
        /// Inicia a transição para um novo estágio de crescimento
        /// </summary>
        private void StartGrowthTransition(SlimeStage targetStage)
        {
            if (_isGrowing) return;

            _isGrowing = true;
            StartCoroutine(GrowthTransitionCoroutine(targetStage));
        }

        /// <summary>
        /// Coroutine que executa a animação de crescimento
        /// </summary>
        private IEnumerator GrowthTransitionCoroutine(SlimeStage targetStage)
        {
            // Encontrar configurações dos estágios atual e alvo
            SlimeGrowthStage currentConfig = GetCurrentStageConfig();
            int targetIndex = (int)targetStage;
            SlimeGrowthStage targetConfig = _growthStages[targetIndex];

            // Armazenar valores iniciais
            Vector3 startSize = _slimeTransform.localScale;
            Vector3 targetSize = Vector3.one * targetConfig.SizeMultiplier;

            // Notificar início do crescimento
            OnGrowthStarted?.Invoke();            // Desabilitar input do jogador temporariamente se necessário
            MonoBehaviour playerInput = GetComponent<MonoBehaviour>();
            bool wasControlEnabled = false;

            // Procura qualquer componente de input/controle do jogador
            // Isso é flexível para funcionar com qualquer sistema de controle implementado
            foreach (MonoBehaviour component in GetComponents<MonoBehaviour>())
            {
                if (component != null &&
                    (component.GetType().Name.Contains("Control") ||
                     component.GetType().Name.Contains("Input")))
                {
                    playerInput = component;
                    wasControlEnabled = playerInput.enabled;
                    playerInput.enabled = false;
                    break;
                }
            }

            // Feedback visual e sonoro
            PlayGrowthVFX(targetConfig);
            PlayGrowthSFX(targetConfig);

            if (_screenFlashEffect != null)
            {
                _screenFlashEffect.SetActive(true);
            }

            // Animação de crescimento
            float elapsedTime = 0f;

            while (elapsedTime < _growthTransitionDuration)
            {
                float t = _growthCurve.Evaluate(elapsedTime / _growthTransitionDuration);
                _slimeTransform.localScale = Vector3.Lerp(startSize, targetSize, t);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Garantir que o tamanho final está correto
            _slimeTransform.localScale = targetSize;

            // Atualizar estágio atual
            _currentStage = targetStage;
            _currentStageIndex = targetIndex;

            // Aplicar propriedades do novo estágio
            ApplyStageProperties(targetConfig);

            // Desativar efeito de flash
            if (_screenFlashEffect != null)
            {
                _screenFlashEffect.SetActive(false);
            }            // Reativar controle do jogador
            if (playerInput != null && wasControlEnabled)
            {
                playerInput.enabled = true;
            }

            // Invocar evento de mudança de estágio
            OnGrowthStageChanged?.Invoke(_currentStage);
            OnGrowthCompleted?.Invoke();

            // Término da transição
            _isGrowing = false;
        }

        /// <summary>
        /// Aplica as propriedades do estágio ao jogador
        /// </summary>
        private void ApplyStageProperties(SlimeGrowthStage stageConfig)
        {
            if (stageConfig == null) return;

            // Aplicar sprite se especificado
            if (_slimeRenderer != null && stageConfig.SlimeSprite != null)
            {
                _slimeRenderer.sprite = stageConfig.SlimeSprite;
            }

            // Aplicar controlador de animação se especificado
            if (_slimeAnimator != null && stageConfig.AnimatorController != null)
            {
                _slimeAnimator.runtimeAnimatorController = stageConfig.AnimatorController;
            }

            // Outros ajustes baseados nas habilidades do estágio
            // podem ser implementados conforme necessário

            // Registrar propriedades no sistema de status do jogador (se existir)
            PlayerStatus playerStatus = GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                playerStatus.UpdateBaseStats(
                    stageConfig.BaseHealth,
                    stageConfig.BaseAttack,
                    stageConfig.BaseDefense,
                    stageConfig.SpeedMultiplier
                );
            }

            Debug.Log($"Slime evoluiu para: {stageConfig.StageName} (Estágio {(int)stageConfig.StageType + 1})");
        }

        /// <summary>
        /// Reproduz efeitos visuais do crescimento
        /// </summary>
        private void PlayGrowthVFX(SlimeGrowthStage targetStage)
        {
            ParticleSystem effectToPlay =
                targetStage.GrowthEffect != null ?
                targetStage.GrowthEffect :
                _defaultGrowthParticles;

            if (effectToPlay != null)
            {
                ParticleSystem instance = Instantiate(
                    effectToPlay,
                    _slimeTransform.position,
                    Quaternion.identity
                );

                Destroy(instance.gameObject, 3f);
            }
        }

        /// <summary>
        /// Reproduz efeitos sonoros do crescimento
        /// </summary>
        private void PlayGrowthSFX(SlimeGrowthStage targetStage)
        {
            AudioClip soundToPlay =
                targetStage.GrowthSound != null ?
                targetStage.GrowthSound :
                _defaultGrowthSound;

            if (soundToPlay != null)
            {
                AudioSource.PlayClipAtPoint(soundToPlay, _slimeTransform.position);
            }
        }

        /// <summary>
        /// Retorna a configuração do estágio atual
        /// </summary>
        public SlimeGrowthStage GetCurrentStageConfig()
        {
            if (_growthStages == null || _growthStages.Length == 0)
                return null;

            return _growthStages[_currentStageIndex];
        }

        /// <summary>
        /// Retorna o estágio atual do Slime
        /// </summary>
        public SlimeStage GetCurrentStage()
        {
            return _currentStage;
        }

        /// <summary>
        /// Valida se os estágios de crescimento estão configurados corretamente
        /// </summary>
        private void ValidateGrowthStages()
        {
            if (_growthStages == null || _growthStages.Length == 0)
            {
                Debug.LogError("PlayerGrowth: Estágios de crescimento não configurados!");
                return;
            }

            // Verificar se temos pelo menos um estágio para cada valor da enum SlimeStage
            int enumCount = Enum.GetValues(typeof(SlimeStage)).Length;
            if (_growthStages.Length < enumCount)
            {
                Debug.LogWarning($"PlayerGrowth: Esperados {enumCount} estágios de crescimento, mas apenas {_growthStages.Length} foram configurados.");
            }

            // Verificar se os estágios estão na ordem correta
            for (int i = 0; i < _growthStages.Length; i++)
            {
                if (_growthStages[i] == null)
                {
                    Debug.LogError($"PlayerGrowth: Estágio na posição {i} é nulo!");
                }
                else if ((int)_growthStages[i].StageType != i)
                {
                    Debug.LogWarning($"PlayerGrowth: O estágio na posição {i} tem tipo {_growthStages[i].StageType}, mas deveria ser {(SlimeStage)i}");
                }
            }
        }

        /// <summary>
        /// Método para forçar crescimento (apenas para testes)
        /// </summary>
        public void ForceGrowth(SlimeStage targetStage)
        {
            if ((int)targetStage >= 0 && (int)targetStage < _growthStages.Length)
            {
                StartGrowthTransition(targetStage);
            }
        }
    }
}
