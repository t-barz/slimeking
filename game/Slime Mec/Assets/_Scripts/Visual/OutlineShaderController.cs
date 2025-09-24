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

        [Header("🔧 Debug")]
        [Tooltip("Mostra logs de debug no Console")]
        [SerializeField] private bool enableDebugLogs = false;

        [Tooltip("Desenha gizmos de debug na Scene")]
        [SerializeField] private bool showDebugGizmos = true;

        [Tooltip("Cor do gizmo de detecção")]
        [SerializeField] private Color gizmoColor = Color.green;

        [Tooltip("Cor do gizmo quando outline está ativo")]
        [SerializeField] private Color gizmoActiveColor = Color.red;
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

            // Validação de configuração
            ValidateCircleDetectionSettings();
        }

        private void Update()
        {
            if (enableCircleDetection)
            {
                HandleCircleDetection();
            }
        }

        private void OnDestroy()
        {
            CleanupMaterials();
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmos || !enableCircleDetection) return;

            Vector2 detectionPos = DetectionPosition;

            // Gizmo do círculo de detecção
            Gizmos.color = _outlineActive ? gizmoActiveColor : gizmoColor;
            Gizmos.DrawWireSphere(detectionPos, detectionRadius);

            // Gizmo do círculo de desativação (se diferente)
            if (deactivationRadius != detectionRadius)
            {
                Gizmos.color = Color.Lerp(gizmoColor, Color.white, 0.5f);
                Gizmos.DrawWireSphere(detectionPos, deactivationRadius);
            }

            // Ponto central
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(detectionPos, Vector3.one * 0.1f);

            // Linha do offset se houver
            if (detectionOffset != Vector2.zero)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, detectionPos);
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
            _currentDetectedObject = null;

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

            if (enableDebugLogs)
                Debug.Log($"OutlineShaderController: Raio de detecção alterado para {detectionRadius} em '{gameObject.name}'", this);
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
            float radiusToUse = _outlineActive ? deactivationRadius : detectionRadius;

            // Debug detalhado quando logs estão habilitados
            if (enableDebugLogs)
            {
                Debug.Log($"[DEBUG] CheckCircleOverlap - Pos: {detectionPos}, Raio: {radiusToUse}, LayerMask: {detectionLayerMask}, OutlineAtivo: {_outlineActive}", this);
            }

            // Usa OverlapCircleAll para verificar TODOS os objetos na área
            Collider2D[] allDetected = Physics2D.OverlapCircleAll(detectionPos, radiusToUse, detectionLayerMask);

            if (enableDebugLogs)
            {
                Debug.Log($"[DEBUG] Total de objetos detectados na área: {allDetected.Length}", this);
                for (int i = 0; i < allDetected.Length && i < 8; i++) // Mostra até 8 objetos
                {
                    var col = allDetected[i];
                    Debug.Log($"[DEBUG] Objeto {i + 1}: {col.name} - Layer: {col.gameObject.layer} - Tag: '{col.tag}'", this);
                }
            }

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
                if (enableDebugLogs)
                {
                    Debug.Log($"[DEBUG] Objeto ESCOLHIDO: {GetObjectInfo(finalDetected)} {(playerObject != null ? "(PLAYER)" : "(OUTROS)")}", this);
                }

                // Objeto válido detectado
                if (!_outlineActive)
                {
                    _currentDetectedObject = finalDetected;
                    EnableOutline();

                    if (enableDebugLogs)
                    {
                        string objectInfo = GetObjectInfo(finalDetected);
                        Debug.Log($"OutlineShaderController: Circle overlap - {objectInfo} ativou outline em '{gameObject.name}' (distância: {Vector2.Distance(detectionPos, finalDetected.transform.position):F2})", this);
                    }
                }
                else
                {
                    _currentDetectedObject = finalDetected;
                }
            }
            else
            {
                // Nenhum objeto válido detectado
                if (_outlineActive)
                {
                    DisableOutline();

                    if (enableDebugLogs)
                    {
                        string objectInfo = _currentDetectedObject != null ? GetObjectInfo(_currentDetectedObject) : "objeto desconhecido";
                        Debug.Log($"OutlineShaderController: Circle overlap - {objectInfo} saiu do raio, desativando outline em '{gameObject.name}'", this);
                    }
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
            if (enableDebugLogs)
            {
                Debug.Log($"[DEBUG] ValidateDetectedObject - Objeto: {detected.name}, Tag: '{detected.tag}', RequiredTag: '{requiredTag}', IsSelf: {detected.gameObject == gameObject}", this);
            }

            // Verifica se não é o próprio objeto
            if (detected.gameObject == gameObject)
            {
                if (enableDebugLogs)
                    Debug.Log($"[DEBUG] Objeto rejeitado: é o próprio objeto", this);
                return false;
            }

            // Verifica tag se especificada
            if (!string.IsNullOrEmpty(requiredTag) && !detected.CompareTag(requiredTag))
            {
                if (enableDebugLogs)
                    Debug.Log($"[DEBUG] Objeto rejeitado: Tag '{detected.tag}' não corresponde à tag necessária '{requiredTag}'", this);
                return false;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[DEBUG] Objeto APROVADO na validação: {GetObjectInfo(detected)}", this);
            }

            return true;
        }

        /// <summary>
        /// Retorna informações detalhadas sobre o objeto detectado para debug.
        /// </summary>
        /// <param name="collider">Collider2D detectado</param>
        /// <returns>String com informações do objeto</returns>
        private string GetObjectInfo(Collider2D collider)
        {
            if (collider == null) return "null";

            return collider switch
            {
                BoxCollider2D box => $"BoxCollider2D (size: {box.size}) em '{collider.name}' [Tag: {collider.tag}]",
                CircleCollider2D circle => $"CircleCollider2D (radius: {circle.radius}) em '{collider.name}' [Tag: {collider.tag}]",
                CapsuleCollider2D capsule => $"CapsuleCollider2D (size: {capsule.size}) em '{collider.name}' [Tag: {collider.tag}]",
                PolygonCollider2D polygon => $"PolygonCollider2D ({polygon.points.Length} pontos) em '{collider.name}' [Tag: {collider.tag}]",
                _ => $"{collider.GetType().Name} em '{collider.name}' [Tag: {collider.tag}]"
            };
        }

        /// <summary>
        /// Valida as configurações de circle detection.
        /// </summary>
        private void ValidateCircleDetectionSettings()
        {
            if (!enableCircleDetection)
            {
                Debug.LogWarning($"OutlineShaderController: Circle Detection está DESABILITADO em '{gameObject.name}'", this);
                return;
            }

            // Garante que deactivation radius seja maior ou igual ao detection radius
            if (deactivationRadius < detectionRadius)
            {
                deactivationRadius = detectionRadius + 0.5f;
                Debug.LogWarning($"OutlineShaderController: deactivationRadius ajustado para {deactivationRadius} " +
                               $"(deve ser >= detectionRadius {detectionRadius}) em '{gameObject.name}'", this);
            }

            // Valida LayerMask
            if (detectionLayerMask == 0)
            {
                Debug.LogError($"OutlineShaderController: detectionLayerMask está vazio em '{gameObject.name}'. " +
                               "NENHUM OBJETO SERÁ DETECTADO! Configure o LayerMask corretamente.", this);
            }

            // Valida raio mínimo
            if (detectionRadius < 0.1f)
            {
                Debug.LogWarning($"OutlineShaderController: detectionRadius muito pequeno ({detectionRadius}) em '{gameObject.name}'. " +
                               "Pode ser difícil detectar objetos.", this);
            }

            // Valida intervalo
            if (checkInterval > 0.5f)
            {
                Debug.LogWarning($"OutlineShaderController: checkInterval muito alto ({checkInterval}s) em '{gameObject.name}'. " +
                               "A detecção pode parecer lenta.", this);
            }

            if (enableDebugLogs)
            {
                Debug.Log($"OutlineShaderController: Circle detection configurado - " +
                         $"Raio: {detectionRadius}, Desativação: {deactivationRadius}, " +
                         $"LayerMask: {detectionLayerMask}, Tag: '{requiredTag}', " +
                         $"Intervalo: {checkInterval}s em '{gameObject.name}'", this);
            }
        }

        /// <summary>
        /// Diagnóstico completo dos problemas mais comuns.
        /// </summary>
        private void DiagnoseCommonIssues()
        {
            Debug.Log("=== DIAGNÓSTICO DE PROBLEMAS COMUNS ===");

            // 1. Circle Detection habilitado?
            if (!enableCircleDetection)
            {
                Debug.LogError("❌ PROBLEMA: Circle Detection está DESABILITADO!");
                return;
            }
            else
            {
                Debug.Log("✅ Circle Detection está habilitado");
            }

            // 2. LayerMask configurado?
            if (detectionLayerMask == 0)
            {
                Debug.LogError("❌ PROBLEMA: LayerMask está vazio (0). Nenhum objeto será detectado!");
                return;
            }
            else
            {
                Debug.Log($"✅ LayerMask configurado: {detectionLayerMask}");
            }

            // 3. Raio adequado?
            if (detectionRadius < 0.5f)
            {
                Debug.LogWarning($"⚠️ AVISO: Raio de detecção muito pequeno ({detectionRadius}). Considere aumentar.");
            }
            else
            {
                Debug.Log($"✅ Raio de detecção adequado: {detectionRadius}");
            }

            // 4. Procura por players na cena
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            if (playerObjects.Length == 0)
            {
                Debug.LogError("❌ PROBLEMA: Nenhum objeto com tag 'Player' encontrado na cena!");
                return;
            }
            else
            {
                Debug.Log($"✅ {playerObjects.Length} objeto(s) com tag 'Player' encontrado(s)");

                // Verifica se algum player tem Collider2D
                bool hasColliders = false;
                foreach (var player in playerObjects)
                {
                    if (player.GetComponent<Collider2D>() != null)
                    {
                        hasColliders = true;
                        break;
                    }
                }

                if (!hasColliders)
                {
                    Debug.LogError("❌ PROBLEMA: Nenhum Player tem Collider2D!");
                    return;
                }
                else
                {
                    Debug.Log("✅ Player(s) têm Collider2D");
                }
            }

            // 5. Verifica se o próprio objeto tem o componente inicializado
            if (!_isInitialized)
            {
                Debug.LogError("❌ PROBLEMA: Componente não foi inicializado corretamente!");
                return;
            }
            else
            {
                Debug.Log("✅ Componente inicializado");
            }

            Debug.Log("=== DIAGNÓSTICO CONCLUÍDO ===");
            Debug.Log("Se ainda não está funcionando, ative os Debug Logs e use 'Debug Detection Area' para mais detalhes.");
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
                      $"\n• Create Instance: {createMaterialInstance}" +
                      $"\n--- CIRCLE DETECTION ---" +
                      $"\n• Enable Circle Detection: {enableCircleDetection}" +
                      $"\n• Detection Radius: {detectionRadius}" +
                      $"\n• Deactivation Radius: {deactivationRadius}" +
                      $"\n• Detection LayerMask: {detectionLayerMask}" +
                      $"\n• Required Tag: '{requiredTag}'" +
                      $"\n• Check Interval: {checkInterval}s" +
                      $"\n• Detection Offset: {detectionOffset}" +
                      $"\n• Detection Position: {DetectionPosition}" +
                      $"\n• Current Detected Object: {(CurrentDetectedObject != null ? CurrentDetectedObject.name : "None")}" +
                      $"\n• Last Check Time: {_lastCheckTime}");
        }

        [ContextMenu("Test Force Circle Check")]
        private void TestForceCircleCheck()
        {
            if (Application.isPlaying)
            {
                ForceCircleCheck();
                Debug.Log("Circle check forçado!");
            }
            else
            {
                Debug.LogWarning("Só funciona no Play Mode");
            }
        }

        [ContextMenu("Debug Detection Area")]
        private void DebugDetectionArea()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Só funciona no Play Mode");
                return;
            }

            Vector2 detectionPos = DetectionPosition;
            float radiusToUse = _outlineActive ? deactivationRadius : detectionRadius;

            Debug.Log($"=== DEBUG DETECTION AREA ===");
            Debug.Log($"Detection Position: {detectionPos}");
            Debug.Log($"Detection Radius: {radiusToUse}");
            Debug.Log($"LayerMask: {detectionLayerMask}");
            Debug.Log($"Required Tag: '{requiredTag}'");

            // Verifica todos os colliders na área
            Collider2D[] allColliders = Physics2D.OverlapCircleAll(detectionPos, radiusToUse);
            Debug.Log($"Total colliders na área (todos layers): {allColliders.Length}");

            for (int i = 0; i < allColliders.Length; i++)
            {
                var col = allColliders[i];
                bool matchesLayer = ((1 << col.gameObject.layer) & detectionLayerMask) != 0;
                bool matchesTag = string.IsNullOrEmpty(requiredTag) || col.CompareTag(requiredTag);
                bool isSelf = col.gameObject == gameObject;

                Debug.Log($"Collider {i + 1}: {col.name}" +
                         $"\n  Layer: {col.gameObject.layer} (matches: {matchesLayer})" +
                         $"\n  Tag: '{col.tag}' (matches: {matchesTag})" +
                         $"\n  Is Self: {isSelf}" +
                         $"\n  Valid: {matchesLayer && matchesTag && !isSelf}");
            }

            // Testa especificamente com o LayerMask
            Collider2D detected = Physics2D.OverlapCircle(detectionPos, radiusToUse, detectionLayerMask);
            Debug.Log($"Physics2D.OverlapCircle result: {(detected != null ? detected.name : "NULL")}");
        }

        [ContextMenu("Find Player Objects")]
        private void FindPlayerObjects()
        {
            GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            Debug.Log("=== PROCURANDO OBJETOS COM TAG 'Player' ===");

            int playerCount = 0;
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("Player"))
                {
                    playerCount++;
                    Collider2D col = obj.GetComponent<Collider2D>();
                    Debug.Log($"Player encontrado: {obj.name}" +
                             $"\n  Layer: {obj.layer}" +
                             $"\n  Position: {obj.transform.position}" +
                             $"\n  Has Collider2D: {col != null}" +
                             $"\n  Distance to this: {Vector2.Distance(transform.position, obj.transform.position):F2}");
                }
            }

            if (playerCount == 0)
            {
                Debug.LogWarning("Nenhum objeto com tag 'Player' encontrado!");
            }
            else
            {
                Debug.Log($"Total de objetos Player encontrados: {playerCount}");
            }
        }

        [ContextMenu("🔍 Diagnose Problems")]
        private void DiagnoseProblems()
        {
            DiagnoseCommonIssues();
        }

        [ContextMenu("🎯 Search Players in Area")]
        private void SearchPlayersInArea()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("Só funciona no Play Mode");
                return;
            }

            Vector2 detectionPos = DetectionPosition;
            float radiusToUse = _outlineActive ? deactivationRadius : detectionRadius;

            Debug.Log("=== BUSCA POR PLAYERS NA ÁREA ===");
            Debug.Log($"Posição: {detectionPos}, Raio: {radiusToUse}");

            // Busca TODOS os objetos (sem LayerMask)
            Collider2D[] allObjects = Physics2D.OverlapCircleAll(detectionPos, radiusToUse);
            Debug.Log($"Total de objetos na área (todos layers): {allObjects.Length}");

            int playerCount = 0;
            int objectsInLayerCount = 0;

            foreach (var obj in allObjects)
            {
                bool isInLayer = ((1 << obj.gameObject.layer) & detectionLayerMask) != 0;
                if (isInLayer) objectsInLayerCount++;

                bool isPlayer = obj.CompareTag("Player");
                if (isPlayer) playerCount++;

                Debug.Log($"• {obj.name}: Layer {obj.gameObject.layer} {(isInLayer ? "✅" : "❌")} | Tag '{obj.tag}' {(isPlayer ? "🎯" : "")} | Distância: {Vector2.Distance(detectionPos, obj.transform.position):F2}");
            }

            Debug.Log($"Resumo:");
            Debug.Log($"• Players encontrados: {playerCount}");
            Debug.Log($"• Objetos no LayerMask correto: {objectsInLayerCount}");

            if (playerCount == 0)
            {
                Debug.LogError("❌ NENHUM PLAYER na área de detecção!");
            }
            else if (objectsInLayerCount == 0)
            {
                Debug.LogError("❌ Players estão na área mas no LAYER ERRADO!");
            }
            else
            {
                Debug.Log("✅ Players encontrados e no layer correto!");
            }
        }

        [ContextMenu("Toggle Debug Logs")]
        private void ToggleDebugLogs()
        {
            enableDebugLogs = !enableDebugLogs;
            Debug.Log($"Debug logs {(enableDebugLogs ? "ATIVADOS" : "DESATIVADOS")}");
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