using UnityEngine;
using SlimeMec.Visual;

namespace SlimeMec.Examples
{
    /// <summary>
    /// Script de exemplo demonstrando como usar o sistema de Outline via Shader.
    /// 
    /// EXEMPLO DE IMPLEMENTAÇÃO:
    /// • Demonstra configuração básica do OutlineShaderController
    /// • Mostra como ativar/desativar outline dinamicamente
    /// • Exemplifica mudança de cor e tamanho em tempo real
    /// • Testa diferentes configurações de performance
    /// 
    /// FUNCIONALIDADES DEMONSTRADAS:
    /// • Outline pulsante (cor e tamanho)
    /// • Mudança de cor por proximidade
    /// • Toggle manual via teclas
    /// • Teste de performance com múltiplos objetos
    /// 
    /// CONTROLES DE TESTE:
    /// • Tecla O: Toggle outline on/off
    /// • Tecla P: Teste de pulsação
    /// • Tecla C: Cicla entre cores
    /// • Tecla R: Reset para configuração inicial
    /// 
    /// DEPENDÊNCIAS:
    /// • OutlineShaderController no mesmo GameObject
    /// • SpriteRenderer com sprite configurado
    /// • Material com shader "SlimeMec/SpriteOutline"
    /// </summary>
    [RequireComponent(typeof(OutlineShaderController))]
    public class OutlineExampleController : MonoBehaviour
    {
        #region Serialized Fields
        [Header("🔲 Configurações de Exemplo")]
        [Tooltip("Ativar teste automático no Start")]
        [SerializeField] private bool autoTestOnStart = true;

        [Tooltip("Cores para ciclar no teste")]
        [SerializeField]
        private Color[] testColors = {
            Color.white,
            Color.red,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.cyan,
            Color.magenta
        };

        [Header("⚡ Efeitos de Teste")]
        [Tooltip("Velocidade da pulsação")]
        [SerializeField, Range(0.1f, 5f)] private float pulseSpeed = 2f;

        [Tooltip("Tamanho mínimo da pulsação")]
        [SerializeField, Range(0f, 0.25f)] private float minPulseSize = 0.005f;

        [Tooltip("Tamanho máximo da pulsação")]
        [SerializeField, Range(0.01f, 0.5f)] private float maxPulseSize = 0.15f; [Header("🔧 Debug")]
        [Tooltip("Mostrar informações no Console")]
        [SerializeField] private bool showDebugInfo = false;
        #endregion

        #region Private Fields
        private OutlineShaderController _outlineController;
        private int _currentColorIndex = 0;
        private bool _isPulsing = false;
        private float _originalOutlineSize;
        private Color _originalOutlineColor;
        private bool _isInitialized = false;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponents();
        }

        private void Start()
        {
            if (autoTestOnStart && _isInitialized)
            {
                StartBasicTest();
            }
        }

        private void Update()
        {
            HandleInput();
            HandlePulseEffect();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Inicia teste básico de funcionalidades.
        /// </summary>
        public void StartBasicTest()
        {
            if (!_isInitialized) return;

            _outlineController.EnableOutline();
            LogDebug("Teste básico iniciado - Outline ativado");
        }

        /// <summary>
        /// Inicia efeito de pulsação.
        /// </summary>
        public void StartPulseEffect()
        {
            if (!_isInitialized) return;

            _isPulsing = true;
            _outlineController.EnableOutline();
            LogDebug("Efeito de pulsação iniciado");
        }

        /// <summary>
        /// Para efeito de pulsação.
        /// </summary>
        public void StopPulseEffect()
        {
            _isPulsing = false;
            _outlineController.SetOutlineSize(_originalOutlineSize);
            LogDebug("Efeito de pulsação parado");
        }

        /// <summary>
        /// Cicla para a próxima cor de teste.
        /// </summary>
        public void CycleToNextColor()
        {
            if (!_isInitialized || testColors.Length == 0) return;

            _currentColorIndex = (_currentColorIndex + 1) % testColors.Length;
            _outlineController.SetOutlineColor(testColors[_currentColorIndex]);
            LogDebug($"Cor alterada para: {testColors[_currentColorIndex]}");
        }

        /// <summary>
        /// Reseta para configurações originais.
        /// </summary>
        public void ResetToOriginal()
        {
            if (!_isInitialized) return;

            StopPulseEffect();
            _outlineController.SetOutlineColor(_originalOutlineColor);
            _outlineController.SetOutlineSize(_originalOutlineSize);
            _currentColorIndex = 0;
            LogDebug("Configurações resetadas para o original");
        }

        /// <summary>
        /// Toggle do outline on/off.
        /// </summary>
        public void ToggleOutline()
        {
            if (!_isInitialized) return;

            _outlineController.ToggleOutline();
            LogDebug($"Outline toggled - Ativo: {_outlineController.IsOutlineActive}");
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Inicializa componentes e validações.
        /// </summary>
        private void InitializeComponents()
        {
            _outlineController = GetComponent<OutlineShaderController>();

            if (_outlineController == null)
            {
                Debug.LogError($"OutlineExampleController: OutlineShaderController não encontrado em '{gameObject.name}'", this);
                enabled = false;
                return;
            }

            // Aguarda a inicialização do controller
            StartCoroutine(WaitForControllerInitialization());
        }

        /// <summary>
        /// Aguarda o controller ser inicializado.
        /// </summary>
        private System.Collections.IEnumerator WaitForControllerInitialization()
        {
            while (!_outlineController.IsInitialized)
            {
                yield return null;
            }

            // Salva configurações originais
            _originalOutlineColor = _outlineController.OutlineColor;
            _originalOutlineSize = _outlineController.OutlineSize;
            _isInitialized = true;

            LogDebug("OutlineExampleController inicializado com sucesso");
        }

        /// <summary>
        /// Processa input do teclado para testes.
        /// </summary>
        private void HandleInput()
        {
            if (!_isInitialized) return;

            if (Input.GetKeyDown(KeyCode.O))
            {
                ToggleOutline();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_isPulsing)
                    StopPulseEffect();
                else
                    StartPulseEffect();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                CycleToNextColor();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetToOriginal();
            }
        }

        /// <summary>
        /// Controla o efeito de pulsação.
        /// </summary>
        private void HandlePulseEffect()
        {
            if (!_isPulsing || !_isInitialized) return;

            float pulseValue = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;
            float currentSize = Mathf.Lerp(minPulseSize, maxPulseSize, pulseValue);

            _outlineController.SetOutlineSize(currentSize);
        }

        /// <summary>
        /// Helper para debug condicional.
        /// </summary>
        private void LogDebug(string message)
        {
            if (showDebugInfo)
            {
                Debug.Log($"OutlineExample [{gameObject.name}]: {message}", this);
            }
        }
        #endregion

        #region Context Menu (Editor Only)
#if UNITY_EDITOR
        [ContextMenu("Start Basic Test")]
        private void EditorStartBasicTest()
        {
            if (Application.isPlaying)
                StartBasicTest();
            else
                Debug.LogWarning("Só funciona no Play Mode");
        }

        [ContextMenu("Start Pulse Effect")]
        private void EditorStartPulseEffect()
        {
            if (Application.isPlaying)
                StartPulseEffect();
            else
                Debug.LogWarning("Só funciona no Play Mode");
        }

        [ContextMenu("Cycle Color")]
        private void EditorCycleColor()
        {
            if (Application.isPlaying)
                CycleToNextColor();
            else
                Debug.LogWarning("Só funciona no Play Mode");
        }

        [ContextMenu("Reset Original")]
        private void EditorResetOriginal()
        {
            if (Application.isPlaying)
                ResetToOriginal();
            else
                Debug.LogWarning("Só funciona no Play Mode");
        }

        [ContextMenu("Show Controls")]
        private void ShowControls()
        {
            Debug.Log("CONTROLES DE TESTE:" +
                      "\n• Tecla O: Toggle outline on/off" +
                      "\n• Tecla P: Toggle efeito de pulsação" +
                      "\n• Tecla C: Cicla entre cores de teste" +
                      "\n• Tecla R: Reset para configuração original" +
                      "\n\nCertifique-se de estar no Play Mode para usar os controles.");
        }
#endif
        #endregion
    }
}