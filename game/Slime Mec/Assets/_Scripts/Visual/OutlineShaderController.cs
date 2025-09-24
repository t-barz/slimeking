using UnityEngine;

namespace SlimeMec.Visual
{
    /// <summary>
    /// Controlador para shader de outline em sprites 2D.
    /// Utiliza shader customizado para criar efeito de outline com alta performance.
    /// 
    /// FUNCIONALIDADES:
    /// • Outline via shader (performance superior)
    /// • Controle dinâmico de cor e tamanho
    /// • Ativação/desativação em tempo real
    /// • Integração automática com SpriteRenderer
    /// • Material instance management
    /// • Detecção automática por circle overlap
    /// 
    /// SISTEMA DE SHADER:
    /// • Usa shader "SlimeMec/SpriteOutline"
    /// • Amostra 8 direções para outline suave
    /// • Preserva transparência original
    /// • Compatible com Sprite Atlas
    /// 
    /// SISTEMA DE CIRCLE OVERLAP:
    /// • Physics2D.OverlapCircle para detecção contínua
    /// • Controle preciso de distância de ativação
    /// • Validação por LayerMask e Tag
    /// • Update otimizado com intervalo configurável
    /// • Hysteresis para evitar flickering
    /// 
    /// EXEMPLO DE USO:
    /// • Adicionar como component ao objeto 2D
    /// • Configurar cor, tamanho e raio de detecção
    /// • Ativar "Enable Circle Detection" para modo automático
    /// • Ou chamar EnableOutline()/DisableOutline() manualmente
    /// 
    /// DEPENDÊNCIAS:
    /// • Shader "SlimeMec/SpriteOutline" deve existir
    /// • Objeto deve ter SpriteRenderer
    /// • Material será criado automaticamente
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class OutlineShaderController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("🔲 Configurações de Outline")]
        [Tooltip("Cor do outline")]
        [SerializeField] private Color outlineColor = Color.white;

        [Tooltip("Tamanho do outline (0 a 0.5)")]
        [SerializeField, Range(0f, 0.5f)] private float outlineSize = 0.01f; [Tooltip("Material com shader de outline (opcional - será criado automaticamente)")]
        [SerializeField] private Material outlineMaterial;

        [Tooltip("Ativar outline automaticamente no Start")]
        [SerializeField] private bool enableOnStart = false;

        [Header("⚡ Performance")]
        [Tooltip("Criar instância do material (recomendado para múltiplos objetos)")]
        [SerializeField] private bool createMaterialInstance = true;

        [Header("🎯 Circle Overlap Detection")]
        [Tooltip("Ativa detecção automática por circle overlap")]
        [SerializeField] private bool enableCircleDetection = true;

        [Tooltip("Raio do círculo para detecção (em unidades)")]
        [SerializeField, Range(0.1f, 10f)] private float detectionRadius = 1f;

        [Tooltip("Raio de desativação (deve ser maior que detectionRadius para hysteresis)")]
        [SerializeField, Range(0.1f, 15f)] private float deactivationRadius = 1.5f;

        [Tooltip("LayerMask dos objetos que podem ativar o outline")]
        [SerializeField] private LayerMask detectionLayerMask = -1;

        [Tooltip("Tag necessária no objeto detectado (deixar vazio para qualquer tag)")]
        [SerializeField] private string requiredTag = "Player";

        [Tooltip("Intervalo entre verificações (em segundos - menor = mais responsivo)")]
        [SerializeField, Range(0.01f, 1f)] private float checkInterval = 0.1f;

        [Tooltip("Offset da posição para detecção (relativo ao transform)")]
        [SerializeField] private Vector2 detectionOffset = Vector2.zero;

        [Header("🎬 Fade Animation")]
        [Tooltip("Ativa animação de fade in/out")]
        [SerializeField] private bool enableFadeAnimation = true;

        [Tooltip("Duração do fade in (segundos)")]
        [SerializeField, Range(0.05f, 3f)] private float fadeInDuration = 0.3f;

        [Tooltip("Duração do fade out (segundos)")]
        [SerializeField, Range(0.05f, 3f)] private float fadeOutDuration = 0.2f;

        [Tooltip("Curva de animação para fade in")]
        [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Tooltip("Curva de animação para fade out")]
        [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);


        #endregion

        #region Private Fields
        private SpriteRenderer _spriteRenderer;
        private Material _originalMaterial;
        private Material _instanceMaterial;
        private bool _outlineActive = false;
        private bool _isInitialized = false;

        // Circle detection
        private float _lastCheckTime = 0f;
        private Collider2D _currentDetectedObject = null;
        private Vector2 DetectionPosition => (Vector2)transform.position + detectionOffset;

        // Fade animation
        private bool _isFading = false;
        private bool _targetOutlineState = false;
        private float _fadeStartTime = 0f;
        private float _fadeDuration = 0f;
        private AnimationCurve _currentFadeCurve;
        private float _fadeStartAlpha = 0f;
        private float _fadeTargetAlpha = 0f;
        private float _currentOutlineAlpha = 0f;
        private float _fadeProgress = 0f;
        private Coroutine _fadeCoroutine = null;

        // Property IDs para performance
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineSizeProperty = Shader.PropertyToID("_OutlineSize");
        private static readonly int EnableOutlineProperty = Shader.PropertyToID("_EnableOutline");

        // Nome do shader
        private const string ShaderName = "SlimeMec/SpriteOutline";
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            // Validação de configuração
            ValidateCircleDetectionSettings();
            ValidateFadeSettings();

            if (enableOnStart)
            {
                EnableOutline();
            }
        }

        private void Update()
        {
            if (enableCircleDetection)
            {
                HandleCircleDetection();
            }

            // Atualiza fade animation se não estiver usando Coroutine
            if (enableFadeAnimation && _isFading && _fadeCoroutine == null)
            {
                UpdateFadeAnimation();
            }
        }

        private void OnDestroy()
        {
            CleanupMaterials();
        }


        #endregion

        #region Public Methods
        /// <summary>
        /// Ativa o efeito de outline com fade in opcional.
        /// </summary>
        public void EnableOutline()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"OutlineShaderController: Não foi possível inicializar em '{gameObject.name}'", this);
                return;
            }

            if (_outlineActive && !_isFading)
            {
                return;
            }
            _targetOutlineState = true;

            if (enableFadeAnimation)
            {
                StartFadeAnimation(true);
            }
            else
            {
                // Ativação instantânea
                SetOutlineProperties(true, 1f);
                _outlineActive = true;
                _currentOutlineAlpha = 1f;
            }


        }

        /// <summary>
        /// Desativa o efeito de outline com fade out opcional.
        /// </summary>
        public void DisableOutline()
        {
            if (!_isInitialized)
                return;

            if (!_outlineActive && !_isFading)
            {
                return;
            }
            _targetOutlineState = false;

            if (enableFadeAnimation && _currentOutlineAlpha > 0f)
            {
                StartFadeAnimation(false);
            }
            else
            {
                // Desativação instantânea
                SetOutlineProperties(false, 0f);
                _outlineActive = false;
                _currentDetectedObject = null;
                _currentOutlineAlpha = 0f;
            }


        }

        /// <summary>
        /// Alterna entre ativo/inativo com fade.
        /// </summary>
        public void ToggleOutline()
        {
            if (_targetOutlineState || (_outlineActive && !_isFading))
                DisableOutline();
            else
                EnableOutline();
        }

        /// <summary>
        /// Atualiza a cor do outline em tempo real.
        /// </summary>
        /// <param name="newColor">Nova cor para o outline</param>
        public void SetOutlineColor(Color newColor)
        {
            outlineColor = newColor;

            if (_isInitialized && _instanceMaterial != null)
            {
                _instanceMaterial.SetColor(OutlineColorProperty, outlineColor);
            }


        }

        /// <summary>
        /// Define o alpha do outline diretamente (usado para animações).
        /// </summary>
        /// <param name="alpha">Valor alpha (0-1)</param>
        public void SetOutlineAlpha(float alpha)
        {
            _currentOutlineAlpha = Mathf.Clamp01(alpha);

            if (_isInitialized && _instanceMaterial != null)
            {
                Color colorWithAlpha = new Color(outlineColor.r, outlineColor.g, outlineColor.b, _currentOutlineAlpha);
                _instanceMaterial.SetColor(OutlineColorProperty, colorWithAlpha);
            }
        }

        /// <summary>
        /// Inicia fade animation com duração customizada.
        /// </summary>
        /// <param name="fadeIn">True para fade in, false para fade out</param>
        /// <param name="customDuration">Duração customizada (opcional)</param>
        public void StartCustomFade(bool fadeIn, float customDuration = -1f)
        {
            if (!_isInitialized) return;

            _targetOutlineState = fadeIn;

            // Usa o método interno que aceita duração personalizada
            StartFadeAnimation(fadeIn, customDuration);
        }

        /// <summary>
        /// Para imediatamente qualquer animação em curso.
        /// </summary>
        public void StopFadeAnimation()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            _isFading = false;


        }

        /// <summary>
        /// Força o outline para um estado específico sem animação.
        /// </summary>
        /// <param name="active">Estado desejado</param>
        /// <param name="alpha">Alpha desejado</param>
        public void ForceOutlineState(bool active, float alpha = -1f)
        {
            StopFadeAnimation();

            _outlineActive = active;
            _targetOutlineState = active;

            if (alpha < 0f)
                alpha = active ? 1f : 0f;

            _currentOutlineAlpha = alpha;

            SetOutlineProperties(active, alpha);


        }

        /// <summary>
        /// Atualiza o tamanho do outline em tempo real.
        /// </summary>
        /// <param name="newSize">Novo tamanho para o outline (0-0.5)</param>
        public void SetOutlineSize(float newSize)
        {
            outlineSize = Mathf.Clamp(newSize, 0f, 0.5f);

            if (_isInitialized && _instanceMaterial != null)
            {
                _instanceMaterial.SetFloat(OutlineSizeProperty, outlineSize);
            }


        }

        /// <summary>
        /// Atualiza o raio de detecção em tempo real.
        /// </summary>
        /// <param name="newRadius">Novo raio de detecção</param>
        public void SetDetectionRadius(float newRadius)
        {
            detectionRadius = Mathf.Max(0.1f, newRadius);

            // Garante que deactivation radius seja sempre maior ou igual
            if (deactivationRadius < detectionRadius)
                deactivationRadius = detectionRadius + 0.5f;


        }

        /// <summary>
        /// Força uma verificação imediata de circle overlap.
        /// </summary>
        public void ForceCircleCheck()
        {
            if (enableCircleDetection)
            {
                CheckCircleOverlap();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Inicializa os componentes necessários e validações.
        /// </summary>
        private void InitializeComponents()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (_spriteRenderer == null)
            {
                Debug.LogError($"OutlineShaderController: SpriteRenderer não encontrado em '{gameObject.name}'", this);
                enabled = false;
                return;
            }

            _originalMaterial = _spriteRenderer.material;

            // Cria ou usa material de outline
            if (!SetupOutlineMaterial())
            {
                enabled = false;
                return;
            }

            _isInitialized = true;


        }

        /// <summary>
        /// Configura o material de outline.
        /// </summary>
        private bool SetupOutlineMaterial()
        {
            Material materialToUse = null;

            // Usa material fornecido ou cria um novo
            if (outlineMaterial != null)
            {
                materialToUse = outlineMaterial;
            }
            else
            {
                // Procura pelo shader
                Shader outlineShader = Shader.Find(ShaderName);
                if (outlineShader == null)
                {
                    Debug.LogError($"OutlineShaderController: Shader '{ShaderName}' não encontrado! " +
                                   "Certifique-se de que o shader está no projeto.", this);
                    return false;
                }

                // Cria material com o shader
                materialToUse = new Material(outlineShader);


            }

            // Cria instância se necessário
            if (createMaterialInstance)
            {
                _instanceMaterial = new Material(materialToUse);
                _spriteRenderer.material = _instanceMaterial;
            }
            else
            {
                _instanceMaterial = materialToUse;
                _spriteRenderer.material = _instanceMaterial;
            }

            // Configura propriedades iniciais
            SetOutlineProperties(false); // Inicia desativado

            return true;
        }

        /// <summary>
        /// Define as propriedades do shader com controle de alpha.
        /// </summary>
        /// <param name="enable">Se deve ativar o outline</param>
        /// <param name="alpha">Valor alpha (0-1)</param>
        private void SetOutlineProperties(bool enable, float alpha = 1f)
        {
            if (_instanceMaterial == null) return;

            _instanceMaterial.SetFloat(EnableOutlineProperty, enable ? 1f : 0f);

            // Aplica cor com alpha específico
            Color colorWithAlpha = new Color(outlineColor.r, outlineColor.g, outlineColor.b, alpha);
            _instanceMaterial.SetColor(OutlineColorProperty, colorWithAlpha);

            _instanceMaterial.SetFloat(OutlineSizeProperty, outlineSize);

            _currentOutlineAlpha = alpha;
        }

        /// <summary>
        /// Limpa materiais criados para evitar memory leaks.
        /// </summary>
        private void CleanupMaterials()
        {
            if (createMaterialInstance && _instanceMaterial != null)
            {
                if (Application.isPlaying)
                    Destroy(_instanceMaterial);
                else
                    DestroyImmediate(_instanceMaterial);

                _instanceMaterial = null;
            }

            // Restaura material original
            if (_spriteRenderer != null && _originalMaterial != null)
            {
                _spriteRenderer.material = _originalMaterial;
            }
        }

        /// <summary>
        /// Inicia a animação de fade.
        /// </summary>
        /// <param name="fadeIn">True para fade in, false para fade out</param>
        private void StartFadeAnimation(bool fadeIn)
        {
            StartFadeAnimation(fadeIn, -1f);
        }

        /// <summary>
        /// Inicia a animação de fade com duração personalizada.
        /// </summary>
        /// <param name="fadeIn">True para fade in, false para fade out</param>
        /// <param name="customDuration">Duração personalizada (usar -1 para usar a duração padrão)</param>
        private void StartFadeAnimation(bool fadeIn, float customDuration)
        {
            // Para animação anterior se existir
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _isFading = true;
            _fadeStartTime = Time.time;

            // Usa duração personalizada se fornecida, senão usa a configuração padrão
            if (customDuration > 0f)
            {
                _fadeDuration = customDuration;
            }
            else
            {
                _fadeDuration = fadeIn ? fadeInDuration : fadeOutDuration;
            }

            _currentFadeCurve = fadeIn ? fadeInCurve : fadeOutCurve;

            _fadeStartAlpha = _currentOutlineAlpha;
            _fadeTargetAlpha = fadeIn ? 1f : 0f;

            // Se está fazendo fade in, ativa o outline imediatamente (mas com alpha atual)
            if (fadeIn && !_outlineActive)
            {
                _outlineActive = true;
                SetOutlineProperties(true, _fadeStartAlpha);
            }

            // Usa Coroutine para animação mais suave
            _fadeCoroutine = StartCoroutine(FadeCoroutine());


        }

        /// <summary>
        /// Coroutine para animação de fade suave.
        /// </summary>
        private System.Collections.IEnumerator FadeCoroutine()
        {
            while (_isFading)
            {
                float elapsed = Time.time - _fadeStartTime;
                float progress = Mathf.Clamp01(elapsed / _fadeDuration);

                // Avalia a curva de animação
                float curveValue = _currentFadeCurve.Evaluate(progress);

                // Interpola o alpha
                float currentAlpha = Mathf.Lerp(_fadeStartAlpha, _fadeTargetAlpha, curveValue);

                // Aplica o alpha atual
                SetOutlineAlpha(currentAlpha);

                // Verifica se a animação terminou
                if (progress >= 1f)
                {
                    _isFading = false;
                    _fadeCoroutine = null;

                    // Se foi fade out completo, desativa o outline
                    if (_fadeTargetAlpha == 0f)
                    {
                        _outlineActive = false;
                        _currentDetectedObject = null;
                        SetOutlineProperties(false, 0f);
                    }



                    yield break;
                }

                yield return null; // Espera próximo frame
            }
        }

        /// <summary>
        /// Atualiza animação de fade no Update (fallback se não usar Coroutine).
        /// </summary>
        private void UpdateFadeAnimation()
        {
            if (!_isFading) return;

            float elapsed = Time.time - _fadeStartTime;
            float progress = Mathf.Clamp01(elapsed / _fadeDuration);
            _fadeProgress = progress;

            // Avalia a curva de animação
            float curveValue = _currentFadeCurve.Evaluate(progress);

            // Interpola o alpha
            float currentAlpha = Mathf.Lerp(_fadeStartAlpha, _fadeTargetAlpha, curveValue);
            _currentOutlineAlpha = currentAlpha;

            // Aplica o alpha atual
            SetOutlineAlpha(currentAlpha);

            // Verifica se a animação terminou
            if (progress >= 1f)
            {
                _isFading = false;

                // Se foi fade out completo, desativa o outline
                if (_fadeTargetAlpha == 0f)
                {
                    _outlineActive = false;
                    _currentDetectedObject = null;
                    SetOutlineProperties(false, 0f);
                }


            }
        }

        /// <summary>
        /// Gerencia a detecção por circle overlap no Update.
        /// </summary>
        private void HandleCircleDetection()
        {
            // Verifica intervalo de tempo
            if (Time.time - _lastCheckTime < checkInterval)
                return;

            _lastCheckTime = Time.time;
            CheckCircleOverlap();
        }

        /// <summary>
        /// Executa a verificação de circle overlap.
        /// </summary>
        private void CheckCircleOverlap()
        {
            Vector2 detectionPos = DetectionPosition;

            // Usa raio apropriado baseado no estado atual (hysteresis)
            float radiusToUse = (_outlineActive || _targetOutlineState) ? deactivationRadius : detectionRadius;

            // Usa OverlapCircleAll para verificar TODOS os objetos na área
            Collider2D[] allDetected = Physics2D.OverlapCircleAll(detectionPos, radiusToUse, detectionLayerMask);

            // Procura especificamente por um objeto válido (prioritiza Player)
            Collider2D validObject = null;
            Collider2D playerObject = null;

            foreach (var detected in allDetected)
            {
                if (ValidateDetectedObject(detected))
                {
                    validObject = detected;

                    // Se encontrou um Player, usa prioritariamente
                    if (!string.IsNullOrEmpty(requiredTag) && detected.CompareTag(requiredTag))
                    {
                        playerObject = detected;
                        break; // Para a busca, Player tem prioridade
                    }
                }
            }

            // Usa Player se encontrado, senão usa qualquer objeto válido
            Collider2D finalDetected = playerObject ?? validObject;

            // Valida objeto detectado
            if (finalDetected != null)
            {
                // Objeto válido detectado
                if (!_targetOutlineState)
                {
                    _currentDetectedObject = finalDetected;
                    EnableOutline();
                }
                else
                {
                    _currentDetectedObject = finalDetected;
                }
            }
            else
            {
                // Nenhum objeto válido detectado
                if (_targetOutlineState)
                {
                    DisableOutline();
                }
            }
        }

        /// <summary>
        /// Valida se o objeto detectado atende aos critérios.
        /// </summary>
        /// <param name="detected">Collider2D detectado</param>
        /// <returns>True se atende aos critérios, false caso contrário</returns>
        private bool ValidateDetectedObject(Collider2D detected)
        {
            // Verifica se não é o próprio objeto
            if (detected.gameObject == gameObject)
            {
                return false;
            }

            // Verifica tag se especificada
            if (!string.IsNullOrEmpty(requiredTag) && !detected.CompareTag(requiredTag))
            {
                return false;
            }

            return true;
        }



        /// <summary>
        /// Valida as configurações de circle detection.
        /// </summary>
        private void ValidateCircleDetectionSettings()
        {
            if (!enableCircleDetection)
            {
                return;
            }

            // Garante que deactivation radius seja maior ou igual ao detection radius
            if (deactivationRadius < detectionRadius)
            {
                deactivationRadius = detectionRadius + 0.5f;
            }
        }

        /// <summary>
        /// Valida configurações de fade animation.
        /// </summary>
        private void ValidateFadeSettings()
        {
            if (!enableFadeAnimation)
            {
                return;
            }

            // Valida durações mínimas
            if (fadeInDuration < 0.05f)
            {
                fadeInDuration = 0.05f;
            }

            if (fadeOutDuration < 0.05f)
            {
                fadeOutDuration = 0.05f;
            }

            // Valida curvas de animação
            if (fadeInCurve == null || fadeInCurve.keys.Length == 0)
            {
                fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            }

            if (fadeOutCurve == null || fadeOutCurve.keys.Length == 0)
            {
                fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            }
        }


        #endregion

        #region Properties
        /// <summary>
        /// Verifica se o outline está ativo.
        /// </summary>
        public bool IsOutlineActive => _outlineActive;

        /// <summary>
        /// Cor atual do outline.
        /// </summary>
        public Color OutlineColor => outlineColor;

        /// <summary>
        /// Tamanho atual do outline.
        /// </summary>
        public float OutlineSize => outlineSize;

        /// <summary>
        /// Verifica se foi inicializado corretamente.
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// Material instance sendo usado.
        /// </summary>
        public Material InstanceMaterial => _instanceMaterial;

        /// <summary>
        /// Raio atual de detecção.
        /// </summary>
        public float DetectionRadius => detectionRadius;

        /// <summary>
        /// Objeto atualmente detectado (pode ser null).
        /// </summary>
        public Collider2D CurrentDetectedObject => _currentDetectedObject;

        /// <summary>
        /// Posição mundial do centro de detecção.
        /// </summary>
        public Vector2 WorldDetectionPosition => DetectionPosition;

        /// <summary>
        /// Verifica se está fazendo fade no momento.
        /// </summary>
        public bool IsFading => _isFading;

        /// <summary>
        /// Estado-alvo do outline (true = fade in, false = fade out).
        /// </summary>
        public bool TargetOutlineState => _targetOutlineState;

        /// <summary>
        /// Alpha atual do outline (0.0 = invisível, 1.0 = totalmente visível).
        /// </summary>
        public float CurrentOutlineAlpha => _currentOutlineAlpha;

        /// <summary>
        /// Progresso atual do fade (0.0 a 1.0).
        /// </summary>
        public float FadeProgress => _fadeProgress;
        #endregion


    }
}