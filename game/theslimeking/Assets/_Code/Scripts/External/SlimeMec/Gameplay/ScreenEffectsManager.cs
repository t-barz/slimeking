using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace SlimeKing.External.SlimeMec.Gameplay
{
    /// <summary>
    /// Singleton responsável por gerenciar todos os efeitos de tela do jogo.
    /// 
    /// FUNCIONALIDADES:
    /// • Efeitos de vinheta (para transições, dano, etc.)
    /// • Efeitos de fade in/out
    /// • Efeitos de distorção
    /// • Efeitos de saturação/dessaturação
    /// • Sistema centralizado e reutilizável
    /// 
    /// USO:
    /// ScreenEffectsManager.Instance.PlayVignetteTransition(intensity, duration);
    /// </summary>
    public class ScreenEffectsManager : MonoBehaviour
    {
        #region Singleton
        private static ScreenEffectsManager _instance;
        public static ScreenEffectsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<ScreenEffectsManager>();

                    if (_instance == null)
                    {
                        // Cria um GameObject com o ScreenEffectsManager se não existir
                        GameObject managerObj = new GameObject("ScreenEffectsManager");
                        _instance = managerObj.AddComponent<ScreenEffectsManager>();
                        DontDestroyOnLoad(managerObj);
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Serialized Fields
        [Header("🎨 Post-Processing References")]
        [Tooltip("Global Volume para efeitos de pós-processamento (será encontrado automaticamente se vazio)")]
        [SerializeField] private Volume globalVolume;

        [Header("🌀 Vignette Settings")]
        [Tooltip("Intensidade máxima da vinheta durante transições")]
        [SerializeField, Range(0f, 1f)] private float maxVignetteIntensity = 1f;

        [Tooltip("Suavidade da vinheta durante transições")]
        [SerializeField, Range(0f, 1f)] private float vignetteSmoothness = 0.5f;

        [Tooltip("Duração padrão das transições de vinheta")]
        [SerializeField] private float defaultVignetteTransitionTime = 1f;
        #endregion

        #region Private Variables
        // === POST-PROCESSING ===
        private Volume _activeGlobalVolume;         // Volume ativo para efeitos
        private Vignette _vignetteEffect;           // Efeito de vinheta
        private float _originalVignetteIntensity;   // Intensidade original da vinheta
        private bool _hasVignetteEffect = false;    // Se o efeito de vinheta está disponível

        // === CONTROLE DE ESTADO ===
        private bool _vignetteTransitionInProgress = false;
        private Coroutine _vignetteTransitionCoroutine = null;

        // === DADOS DE TELETRANSPORTE PENDENTE ===
        private bool _hasPendingTeleport = false;
        private Vector3 _pendingPlayerPosition;
        private GameObject _pendingPlayerPrefab;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Implementa padrão Singleton
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePostProcessing();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            ValidateConfiguration();
        }
        #endregion

        #region Public Methods - Vignette Effects

        /// <summary>
        /// Executa uma transição completa de vinheta (fecha e abre).
        /// </summary>
        /// <param name="duration">Duração total da transição (ida e volta)</param>
        /// <param name="maxIntensity">Intensidade máxima da vinheta</param>
        /// <param name="onHalfway">Callback executado quando vinheta está totalmente fechada</param>
        /// <returns>Corrotina da transição</returns>
        public Coroutine PlayVignetteTransition(float duration = -1f, float maxIntensity = -1f, System.Action onHalfway = null)
        {
            if (duration < 0) duration = defaultVignetteTransitionTime;
            if (maxIntensity < 0) maxIntensity = this.maxVignetteIntensity;

            if (_vignetteTransitionInProgress)
            {StopVignetteTransition();
            }

            _vignetteTransitionCoroutine = StartCoroutine(VignetteTransitionCoroutine(duration, maxIntensity, onHalfway));
            return _vignetteTransitionCoroutine;
        }

        /// <summary>
        /// Fecha a vinheta gradualmente.
        /// </summary>
        /// <param name="duration">Duração do fechamento</param>
        /// <param name="maxIntensity">Intensidade máxima</param>
        /// <returns>Corrotina do efeito</returns>
        /// <summary>
        /// Fecha a vinheta gradualmente, com callback opcional ao final.
        /// </summary>
        /// <param name="duration">Duração do fechamento</param>
        /// <param name="maxIntensity">Intensidade máxima</param>
        /// <param name="onClosed">Callback chamado ao finalizar o fechamento</param>
        /// <returns>Corrotina do efeito</returns>
        public Coroutine CloseVignette(float duration = -1f, float maxIntensity = -1f, System.Action onClosed = null)
        {
            if (duration < 0) duration = defaultVignetteTransitionTime * 0.5f;
            if (maxIntensity < 0) maxIntensity = this.maxVignetteIntensity;

            return StartCoroutine(VignetteEffectCoroutine(true, duration, maxIntensity, onClosed));
        }

        /// <summary>
        /// Abre a vinheta gradualmente.
        /// </summary>
        /// <param name="duration">Duração da abertura</param>
        /// <returns>Corrotina do efeito</returns>
        public Coroutine OpenVignette(float duration = -1f)
        {
            if (duration < 0) duration = defaultVignetteTransitionTime * 0.5f;

            return StartCoroutine(VignetteEffectCoroutine(false, duration, _originalVignetteIntensity));
        }

        /// <summary>
        /// Para qualquer transição de vinheta em progresso.
        /// </summary>
        public void StopVignetteTransition()
        {
            if (_vignetteTransitionCoroutine != null)
            {
                StopCoroutine(_vignetteTransitionCoroutine);
                _vignetteTransitionCoroutine = null;
                _vignetteTransitionInProgress = false;
            }
        }

        /// <summary>
        /// Restaura a vinheta para o estado original.
        /// </summary>
        public void ResetVignette()
        {
            StopVignetteTransition();

            if (_hasVignetteEffect && _vignetteEffect != null)
            {
                _vignetteEffect.intensity.value = _originalVignetteIntensity;}
        }

        /// <summary>
        /// Verifica se há um efeito de vinheta em progresso.
        /// </summary>
        /// <returns>True se há transição em progresso</returns>
        public bool IsVignetteTransitionInProgress()
        {
            return _vignetteTransitionInProgress;
        }

        #endregion

        #region Private Methods - Vignette Implementation

        /// <summary>
        /// Corrotina para transição completa de vinheta.
        /// </summary>
        private IEnumerator VignetteTransitionCoroutine(float duration, float maxIntensity, System.Action onHalfway)
        {
            _vignetteTransitionInProgress = true;
            float halfDuration = duration * 0.5f;

            // FASE 1: Fecha vinheta
            yield return StartCoroutine(VignetteEffectCoroutine(true, halfDuration, maxIntensity));

            // Executa callback no meio da transição
            onHalfway?.Invoke();

            // FASE 2: Abre vinheta
            yield return StartCoroutine(VignetteEffectCoroutine(false, halfDuration, _originalVignetteIntensity));

            _vignetteTransitionInProgress = false;
            _vignetteTransitionCoroutine = null;}

        /// <summary>
        /// Corrotina para efeito de vinheta (abrir ou fechar).
        /// </summary>
        private IEnumerator VignetteEffectCoroutine(bool closing, float duration, float targetIntensity, System.Action onComplete = null)
        {
            if (!_hasVignetteEffect || _vignetteEffect == null)
            {yield return new WaitForSeconds(duration);
                onComplete?.Invoke();
                yield break;
            }

            float startIntensity = _vignetteEffect.intensity.value;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;

                // Interpola intensidade da vinheta
                float currentIntensity = Mathf.Lerp(startIntensity, targetIntensity, progress);
                _vignetteEffect.intensity.value = currentIntensity;

                // Atualiza suavidade da vinheta
                _vignetteEffect.smoothness.value = vignetteSmoothness;

                yield return null;
            }

            // Garante valor final
            _vignetteEffect.intensity.value = targetIntensity;onComplete?.Invoke();
        }

        #endregion

        #region Private Methods - Initialization

        /// <summary>
        /// Inicializa sistema de pós-processamento.
        /// </summary>
        private void InitializePostProcessing()
        {
            // Encontra Global Volume se não estiver definido
            if (globalVolume == null)
            {
                _activeGlobalVolume = FindFirstObjectByType<Volume>();

                if (_activeGlobalVolume == null)
                {return;
                }
            }
            else
            {
                _activeGlobalVolume = globalVolume;
            }

            // Verifica e configura efeito de vinheta
            if (_activeGlobalVolume.profile != null)
            {
                if (_activeGlobalVolume.profile.TryGet<Vignette>(out _vignetteEffect))
                {
                    _hasVignetteEffect = true;
                    _originalVignetteIntensity = _vignetteEffect.intensity.value;}
                else
                {}
            }
            else
            {}
        }

        /// <summary>
        /// Valida a configuração dos efeitos de tela.
        /// </summary>
        private void ValidateConfiguration()
        {
            bool hasWarnings = false;

            // Validações de warning
            if (defaultVignetteTransitionTime <= 0f)
            {defaultVignetteTransitionTime = 1f;
            }

            if (!_hasVignetteEffect)
            {hasWarnings = true;
            }

            if (!hasWarnings)
            {}
        }

        #endregion

        #region Public Methods - Utility

        /// <summary>
        /// Reinicializa o sistema de pós-processamento (útil após mudanças de cena).
        /// </summary>
        public void ReinitializePostProcessing()
        {InitializePostProcessing();

            // Processa teletransporte pendente se houver
            if (_hasPendingTeleport)
            {
                ProcessPendingTeleport();
            }
        }

        /// <summary>
        /// Verifica se os efeitos de vinheta estão disponíveis.
        /// </summary>
        /// <returns>True se vinheta está disponível</returns>
        public bool IsVignetteAvailable()
        {
            return _hasVignetteEffect && _vignetteEffect != null;
        }

        /// <summary>
        /// Define dados de teletransporte pendente para instanciar Player na nova cena.
        /// </summary>
        /// <param name="playerPrefab">Prefab do Player</param>
        /// <param name="position">Posição de destino</param>
        public void SetPendingTeleport(GameObject playerPrefab, Vector3 position)
        {
            if (playerPrefab == null)
            {_hasPendingTeleport = false;
                _pendingPlayerPrefab = null;
                _pendingPlayerPosition = Vector3.zero;
                return;
            }
            _hasPendingTeleport = true;
            _pendingPlayerPrefab = playerPrefab;
            _pendingPlayerPosition = position;}

        /// <summary>
        /// Processa teletransporte pendente (chama na nova cena).
        /// </summary>
        /// <returns>Player instanciado ou null</returns>
        public GameObject ProcessPendingTeleport()
        {
            if (!_hasPendingTeleport)
            {
                return null;
            }GameObject newPlayer = null;
            if (_pendingPlayerPrefab != null)
            {
                newPlayer = Instantiate(_pendingPlayerPrefab, _pendingPlayerPosition, Quaternion.identity);// Garante que o Player está na cena ativa
                var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(newPlayer, activeScene);}
            else
            {}

            // Limpa dados pendentes
            _hasPendingTeleport = false;
            _pendingPlayerPrefab = null;
            _pendingPlayerPosition = Vector3.zero;

            return newPlayer;
        }

        /// <summary>
        /// Verifica se há teletransporte pendente.
        /// </summary>
        /// <returns>True se há teletransporte pendente</returns>
        public bool HasPendingTeleport()
        {
            return _hasPendingTeleport;
        }

        #endregion
    }
}