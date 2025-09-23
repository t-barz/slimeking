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
    /// • Detecção automática por trigger
    /// 
    /// SISTEMA DE SHADER:
    /// • Usa shader "SlimeMec/SpriteOutline"
    /// • Amostra 8 direções para outline suave
    /// • Preserva transparência original
    /// • Compatible com Sprite Atlas
    /// 
    /// SISTEMA DE TRIGGER:
    /// • OnTriggerEnter2D: Ativa outline automaticamente
    /// • OnTriggerExit2D: Desativa outline automaticamente
    /// • Validação por tipo de collider (CapsuleCollider2D)
    /// • Validação por tag (configurável)
    /// • Auto-setup de trigger collider se necessário
    /// 
    /// EXEMPLO DE USO:
    /// • Adicionar como component ao objeto 2D
    /// • Configurar cor e tamanho no Inspector
    /// • Ativar "Enable Trigger Detection" para modo automático
    /// • Ou chamar EnableOutline()/DisableOutline() manualmente
    /// 
    /// DEPENDÊNCIAS:
    /// • Shader "SlimeMec/SpriteOutline" deve existir
    /// • Objeto deve ter SpriteRenderer
    /// • Para trigger: Collider2D configurado como trigger
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

        [Header("🎯 Trigger Detection")]
        [Tooltip("Ativa detecção automática por trigger")]
        [SerializeField] private bool enableTriggerDetection = true;

        [Tooltip("Requere que seja CapsuleCollider2D para ativar")]
        [SerializeField] private bool requireCapsuleCollider = true;

        [Tooltip("Tag necessária no objeto que entra no trigger")]
        [SerializeField] private string requiredTag = "Player";

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

            // Configura trigger se necessário
            EnsureTriggerSetup();
        }

        private void OnDestroy()
        {
            CleanupMaterials();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!enableTriggerDetection) return;

            if (ValidateTriggerCollider(other))
            {
                Debug.Log($"OutlineShaderController: Trigger Enter - {GetColliderInfo(other)} ativou outline em '{gameObject.name}'", this);
                EnableOutline();

                if (enableDebugLogs)
                {
                    string colliderInfo = GetColliderInfo(other);
                    Debug.Log($"OutlineShaderController: Trigger Enter - {colliderInfo} ativou outline em '{gameObject.name}'", this);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!enableTriggerDetection) return;

            if (ValidateTriggerCollider(other))
            {
                DisableOutline();

                if (enableDebugLogs)
                {
                    string colliderInfo = GetColliderInfo(other);
                    Debug.Log($"OutlineShaderController: Trigger Exit - {colliderInfo} desativou outline em '{gameObject.name}'", this);
                }
            }
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

        /// <summary>
        /// Valida se o collider que entrou no trigger atende aos critérios.
        /// </summary>
        /// <param name="other">Collider2D que entrou no trigger</param>
        /// <returns>True se atende aos critérios, false caso contrário</returns>
        private bool ValidateTriggerCollider(Collider2D other)
        {
            // Verifica tag
            if (!other.CompareTag(requiredTag))
            {
                if (enableDebugLogs)
                    Debug.Log($"OutlineShaderController: Tag '{other.tag}' não corresponde à tag necessária '{requiredTag}'", this);
                return false;
            }

            // Verifica tipo de collider se necessário
            if (requireCapsuleCollider)
            {
                if (this.GetComponent<CapsuleCollider2D>() == null)
                {
                    if (enableDebugLogs)
                        Debug.Log($"OutlineShaderController: Objeto '{this.name}' não possui CapsuleCollider2D", this);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Retorna informações detalhadas sobre o collider para debug.
        /// </summary>
        /// <param name="collider">Collider2D para analisar</param>
        /// <returns>String com informações do collider</returns>
        private string GetColliderInfo(Collider2D collider)
        {
            return collider switch
            {
                BoxCollider2D box => $"BoxCollider2D (size: {box.size}) em '{collider.name}'",
                CircleCollider2D circle => $"CircleCollider2D (radius: {circle.radius}) em '{collider.name}'",
                CapsuleCollider2D capsule => $"CapsuleCollider2D (size: {capsule.size}, direction: {capsule.direction}) em '{collider.name}'",
                PolygonCollider2D polygon => $"PolygonCollider2D ({polygon.points.Length} pontos) em '{collider.name}'",
                EdgeCollider2D edge => $"EdgeCollider2D ({edge.points.Length} pontos) em '{collider.name}'",
                CompositeCollider2D composite => $"CompositeCollider2D ({composite.pathCount} paths) em '{collider.name}'",
                _ => $"{collider.GetType().Name} em '{collider.name}'"
            };
        }

        /// <summary>
        /// Garante que o objeto tem um collider configurado como trigger.
        /// </summary>
        private void EnsureTriggerSetup()
        {
            if (!enableTriggerDetection) return;

            Collider2D col = GetComponent<Collider2D>();
            if (col == null)
            {
                // Adiciona BoxCollider2D como trigger automaticamente
                BoxCollider2D box = gameObject.AddComponent<BoxCollider2D>();
                box.isTrigger = true;

                if (enableDebugLogs)
                    Debug.Log($"OutlineShaderController: BoxCollider2D adicionado automaticamente como trigger em '{gameObject.name}'", this);
            }
            else if (!col.isTrigger)
            {
                col.isTrigger = true;

                if (enableDebugLogs)
                    Debug.Log($"OutlineShaderController: Collider configurado como trigger em '{gameObject.name}'", this);
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
            Collider2D thisCollider = GetComponent<Collider2D>();

            Debug.Log($"OutlineShaderController Debug Info:" +
                      $"\n• GameObject: {gameObject.name}" +
                      $"\n• Outline Active: {_outlineActive}" +
                      $"\n• Initialized: {_isInitialized}" +
                      $"\n• Outline Color: {outlineColor}" +
                      $"\n• Outline Size: {outlineSize}" +
                      $"\n• SpriteRenderer: {(_spriteRenderer != null ? "OK" : "NULL")}" +
                      $"\n• Instance Material: {(_instanceMaterial != null ? _instanceMaterial.name : "NULL")}" +
                      $"\n• Shader: {(_instanceMaterial != null ? _instanceMaterial.shader.name : "NULL")}" +
                      $"\n• Create Instance: {createMaterialInstance}" +
                      $"\n--- TRIGGER DETECTION ---" +
                      $"\n• Enable Trigger Detection: {enableTriggerDetection}" +
                      $"\n• Require Capsule Collider: {requireCapsuleCollider}" +
                      $"\n• Required Tag: '{requiredTag}'" +
                      $"\n• This Collider: {(thisCollider != null ? thisCollider.GetType().Name : "NULL")}" +
                      $"\n• Is Trigger: {(thisCollider != null ? thisCollider.isTrigger : false)}");
        }

        [ContextMenu("Test Trigger Enter")]
        private void TestTriggerEnter()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("OutlineShaderController: Teste só funciona no Play Mode");
                return;
            }

            // Simula trigger enter
            EnableOutline();
            Debug.Log("OutlineShaderController: Trigger Enter simulado");
        }

        [ContextMenu("Test Trigger Exit")]
        private void TestTriggerExit()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("OutlineShaderController: Teste só funciona no Play Mode");
                return;
            }

            // Simula trigger exit
            DisableOutline();
            Debug.Log("OutlineShaderController: Trigger Exit simulado");
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