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
    /// 
    /// SISTEMA DE SHADER:
    /// • Usa shader "SlimeMec/SpriteOutline"
    /// • Amostra 8 direções para outline suave
    /// • Preserva transparência original
    /// • Compatible com Sprite Atlas
    /// 
    /// EXEMPLO DE USO:
    /// • Adicionar como component ao objeto 2D
    /// • Configurar cor e tamanho no Inspector
    /// • Chamar EnableOutline() para ativar
    /// • Chamar DisableOutline() para desativar
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

        [Header("🔧 Debug")]
        [Tooltip("Mostra logs de debug no Console")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion

        #region Private Fields
        private SpriteRenderer _spriteRenderer;
        private Material _originalMaterial;
        private Material _instanceMaterial;
        private bool _outlineActive = false;
        private bool _isInitialized = false;

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
            if (enableOnStart)
            {
                EnableOutline();
            }
        }

        private void OnDestroy()
        {
            CleanupMaterials();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Ativa o efeito de outline via shader.
        /// </summary>
        public void EnableOutline()
        {
            if (!_isInitialized)
            {
                Debug.LogError($"OutlineShaderController: Não foi possível inicializar em '{gameObject.name}'", this);
                return;
            }

            if (_outlineActive)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"OutlineShaderController: Outline já está ativo em '{gameObject.name}'", this);
                return;
            }

            SetOutlineProperties(true);
            _outlineActive = true;

            if (enableDebugLogs)
                Debug.Log($"OutlineShaderController: Outline ativado em '{gameObject.name}' - Cor: {outlineColor}, Tamanho: {outlineSize}", this);
        }

        /// <summary>
        /// Desativa o efeito de outline via shader.
        /// </summary>
        public void DisableOutline()
        {
            if (!_isInitialized)
                return;

            if (!_outlineActive)
            {
                if (enableDebugLogs)
                    Debug.LogWarning($"OutlineShaderController: Outline já está inativo em '{gameObject.name}'", this);
                return;
            }

            SetOutlineProperties(false);
            _outlineActive = false;

            if (enableDebugLogs)
                Debug.Log($"OutlineShaderController: Outline desativado em '{gameObject.name}'", this);
        }

        /// <summary>
        /// Alterna entre ativo/inativo.
        /// </summary>
        public void ToggleOutline()
        {
            if (_outlineActive)
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

            if (enableDebugLogs)
                Debug.Log($"OutlineShaderController: Cor alterada para {newColor} em '{gameObject.name}'", this);
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

            if (enableDebugLogs)
                Debug.Log($"OutlineShaderController: Tamanho alterado para {outlineSize} em '{gameObject.name}'", this);
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

            if (enableDebugLogs)
                Debug.Log($"OutlineShaderController: Inicializado em '{gameObject.name}'", this);
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

                if (enableDebugLogs)
                    Debug.Log($"OutlineShaderController: Material criado automaticamente com shader '{ShaderName}'", this);
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
        /// Define as propriedades do shader.
        /// </summary>
        /// <param name="enable">Se deve ativar o outline</param>
        private void SetOutlineProperties(bool enable)
        {
            if (_instanceMaterial == null) return;

            _instanceMaterial.SetFloat(EnableOutlineProperty, enable ? 1f : 0f);
            _instanceMaterial.SetColor(OutlineColorProperty, outlineColor);
            _instanceMaterial.SetFloat(OutlineSizeProperty, outlineSize);
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
        #endregion

        #region Context Menu (Editor Only)
#if UNITY_EDITOR
        [ContextMenu("Test Enable Outline")]
        private void TestEnableOutline()
        {
            if (Application.isPlaying)
            {
                EnableOutline();
            }
            else
            {
                Debug.LogWarning("OutlineShaderController: Teste só funciona no Play Mode");
            }
        }

        [ContextMenu("Test Disable Outline")]
        private void TestDisableOutline()
        {
            if (Application.isPlaying)
            {
                DisableOutline();
            }
            else
            {
                Debug.LogWarning("OutlineShaderController: Teste só funciona no Play Mode");
            }
        }

        [ContextMenu("Test Toggle Outline")]
        private void TestToggleOutline()
        {
            if (Application.isPlaying)
            {
                ToggleOutline();
            }
            else
            {
                Debug.LogWarning("OutlineShaderController: Teste só funciona no Play Mode");
            }
        }

        [ContextMenu("Debug Info")]
        private void DebugInfo()
        {
            Debug.Log($"OutlineShaderController Debug Info:" +
                      $"\n• GameObject: {gameObject.name}" +
                      $"\n• Outline Active: {_outlineActive}" +
                      $"\n• Initialized: {_isInitialized}" +
                      $"\n• Outline Color: {outlineColor}" +
                      $"\n• Outline Size: {outlineSize}" +
                      $"\n• SpriteRenderer: {(_spriteRenderer != null ? "OK" : "NULL")}" +
                      $"\n• Instance Material: {(_instanceMaterial != null ? _instanceMaterial.name : "NULL")}" +
                      $"\n• Shader: {(_instanceMaterial != null ? _instanceMaterial.shader.name : "NULL")}" +
                      $"\n• Create Instance: {createMaterialInstance}");
        }

        [ContextMenu("Force Recreate Material")]
        private void ForceRecreateMaterial()
        {
            if (Application.isPlaying)
            {
                CleanupMaterials();
                SetupOutlineMaterial();
                Debug.Log("Material recriado!");
            }
            else
            {
                Debug.LogWarning("Só funciona no Play Mode");
            }
        }
#endif
        #endregion
    }
}