using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente que aplica variações visuais aleatórias a criaturas de forma permanente.
    /// Diferente de SetupVisualEnvironment, este componente aplica variações também em Runtime.
    /// Cada instância de criatura recebe variações únicas baseadas em seu InstanceID.
    /// </summary>
    [ExecuteInEditMode]
    public class SetupVisualCreature : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Master Control")]
        [SerializeField] private bool applyRandomVariations = true;
        [Tooltip("Ativa/desativa todas as variações visuais aleatórias")]

        [Header("Flip Settings")]
        [SerializeField] private bool applyFlipVariations = false;
        [Tooltip("Ativa/desativa variações de flip")]
        [SerializeField] private bool enableFlipX = true;
        [Tooltip("Permite flip horizontal aleatório")]
        [SerializeField] private bool enableFlipY = false;
        [Tooltip("Permite flip vertical aleatório")]

        [Header("Scale Settings")]
        [SerializeField] private bool applyScaleVariations = false;
        [Tooltip("Ativa/desativa variações de escala")]
        [SerializeField] private bool enableScaleVariation = true;
        [Tooltip("Permite variação aleatória de tamanho")]
        [SerializeField] private Vector2 scaleRange = new Vector2(0.9f, 1.1f);
        [Tooltip("Faixa de variação do tamanho (min, max)")]
        [SerializeField] private bool uniformScale = true;
        [Tooltip("Se verdadeiro, aplica a mesma escala em X e Y")]

        [Header("Color Settings")]
        [SerializeField] private bool applyColorVariations = false;
        [Tooltip("Ativa/desativa variações de cor")]
        [SerializeField] private bool enableColorVariation = true;
        [Tooltip("Permite variação aleatória de cor")]
        
        [SerializeField] private bool useColorPalette = false;
        [Tooltip("Se true, seleciona cores de uma lista predefinida ao invés de usar tint")]
        [SerializeField] private Color[] colorPalette = { Color.white, Color.red, Color.green, Color.blue, Color.yellow };
        [Tooltip("Lista de cores possíveis para seleção aleatória")]
        
        [SerializeField] private bool useColorInterpolation = false;
        [Tooltip("Se true, usa interpolação de cor ao invés de paleta ou tint")]
        [SerializeField] private Color baseColor = Color.white;
        [Tooltip("Cor base para interpolação")]
        [SerializeField] private float colorVariation = 0.3f;
        [Tooltip("Variação máxima de cada canal de cor (0-1)")]
        
        [SerializeField] private Color colorTintRange = new Color(0.2f, 0.2f, 0.2f, 0f);
        [Tooltip("Variação máxima para cada canal de cor (R, G, B, A). Usado quando não usa paleta/interpolação")]
        
        [SerializeField] private bool randomizeAlpha = false;
        [Tooltip("Se true, randomiza também o canal Alpha da cor")]
        [SerializeField] private float minAlpha = 0.7f;
        [Tooltip("Valor mínimo para o canal Alpha")]
        [SerializeField] private float maxAlpha = 1f;
        [Tooltip("Valor máximo para o canal Alpha")]

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;
        [Tooltip("Mostra logs de debug das alterações aplicadas")]
        #endregion

        #region Private Fields
        private SpriteRenderer spriteRenderer;
        private Renderer meshRenderer;
        
        [SerializeField, HideInInspector]
        private bool hasBeenInitialized = false;

        // Valores originais para referência
        private Vector3 originalScale;
        private Color originalColor;
        private bool originalFlipX;
        private bool originalFlipY;

        // Seed baseada em InstanceID para consistência
        private int seedValue;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponent();
            
            // Aplica variações quando a criatura é instanciada (em runtime)
            if (Application.isPlaying && ShouldApplyVariationsAtRuntime())
            {
                ApplyRandomVariations();
            }
        }

        // ExecuteInEditMode - aplicado também no Editor
        private void OnValidate()
        {
            // No Editor, aplica variações quando é uma instância de cena (não o prefab base)
            if (Application.isPlaying) return;
            
            // Não aplica se já foi inicializado (evita reaplicar ao entrar em Play Mode)
            if (hasBeenInitialized) return;

            InitializeComponent();

#if UNITY_EDITOR
            // Verifica se é uma instância de cena (não o prefab base)
            if (IsSceneInstance() && ShouldApplyVariationsAtEditor())
            {
                ApplyRandomVariations();
            }
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Verifica se este objeto é uma instância na cena (não o prefab base).
        /// </summary>
        private bool IsSceneInstance()
        {
            // Se não está em uma cena válida, não é uma instância
            if (!gameObject.scene.IsValid())
                return false;

            // Verifica se é uma instância de prefab na cena
            var prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);
            return prefabStatus == PrefabInstanceStatus.Connected || 
                   prefabStatus == PrefabInstanceStatus.Disconnected ||
                   prefabStatus == PrefabInstanceStatus.NotAPrefab;
        }
#endif
        #endregion

        #region Initialization
        private void InitializeComponent()
        {
            // Cache dos Renderers
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (meshRenderer == null)
            {
                meshRenderer = GetComponent<Renderer>();
                // Se achou SpriteRenderer, não precisa de outro Renderer
                if (spriteRenderer != null)
                {
                    meshRenderer = null;
                }
            }

            if (spriteRenderer == null && meshRenderer == null)
            {
                if (showDebugLogs) { }
                return;
            }

            // Armazena valores originais na primeira inicialização
            if (!hasBeenInitialized)
            {
                StoreOriginalValues();
                GenerateSeed();
            }
        }

        private void StoreOriginalValues()
        {
            if (spriteRenderer != null)
            {
                originalScale = transform.localScale;
                originalColor = spriteRenderer.color;
                originalFlipX = spriteRenderer.flipX;
                originalFlipY = spriteRenderer.flipY;
            }
            else if (meshRenderer != null)
            {
                originalScale = transform.localScale;
                originalColor = meshRenderer.material.color;
            }
        }

        private void GenerateSeed()
        {
            // Gera seed baseada no InstanceID para garantir unicidade por instância
            // Combina InstanceID com posição para variação adicional
            int instanceId = GetInstanceID();
            Vector3 pos = transform.position;
            seedValue = Mathf.Abs((instanceId.GetHashCode() + pos.GetHashCode()));

            if (showDebugLogs)
            {
                // Seed generated
            }
        }

        /// <summary>
        /// Verifica se as variações devem ser aplicadas no Editor.
        /// </summary>
        private bool ShouldApplyVariationsAtEditor()
        {
            // No Editor, verifica se alguma flag está ativa
            bool hasAnyFlagActive = applyRandomVariations || applyFlipVariations || applyScaleVariations || applyColorVariations;

            if (!hasAnyFlagActive)
            {
                return false;
            }

            // Se o master flag estiver ativo, verifica se pelo menos uma sub-flag está ativa
            if (applyRandomVariations)
            {
                bool hasSubFlagActive = applyFlipVariations || applyScaleVariations || applyColorVariations;
                if (!hasSubFlagActive)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se as variações devem ser aplicadas em runtime (quando criatura é instanciada).
        /// </summary>
        private bool ShouldApplyVariationsAtRuntime()
        {
            // Em runtime, verifica se alguma flag está ativa e ainda não foi inicializado
            if (hasBeenInitialized)
            {
                return false;
            }

            bool hasAnyFlagActive = applyRandomVariations || applyFlipVariations || applyScaleVariations || applyColorVariations;

            if (!hasAnyFlagActive)
            {
                return false;
            }

            // Se o master flag estiver ativo, verifica se pelo menos uma sub-flag está ativa
            if (applyRandomVariations)
            {
                bool hasSubFlagActive = applyFlipVariations || applyScaleVariations || applyColorVariations;
                if (!hasSubFlagActive)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se as variações de flip devem ser aplicadas.
        /// </summary>
        private bool ShouldApplyFlipVariations()
        {
            return applyRandomVariations ? applyFlipVariations : applyFlipVariations;
        }

        /// <summary>
        /// Verifica se as variações de escala devem ser aplicadas.
        /// </summary>
        private bool ShouldApplyScaleVariations()
        {
            return applyRandomVariations ? applyScaleVariations : applyScaleVariations;
        }

        /// <summary>
        /// Verifica se as variações de cor devem ser aplicadas.
        /// </summary>
        private bool ShouldApplyColorVariations()
        {
            return applyRandomVariations ? applyColorVariations : applyColorVariations;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Aplica todas as variações visuais aleatórias baseadas nas configurações.
        /// </summary>
        public void ApplyRandomVariations()
        {
            if ((spriteRenderer == null && meshRenderer == null) || hasBeenInitialized)
            {
                return;
            }

            // Usa o seed para randomização consistente
            Random.State originalRandomState = Random.state;
            Random.InitState(seedValue);

            try
            {
                // Aplica flip aleatório (verifica se a flag específica está ativa)
                if (ShouldApplyFlipVariations() && (enableFlipX || enableFlipY) && spriteRenderer != null)
                {
                    ApplyRandomFlip();
                }

                // Aplica variação de escala (verifica se a flag específica está ativa)
                if (ShouldApplyScaleVariations() && enableScaleVariation)
                {
                    ApplyRandomScale();
                }

                // Aplica variação de cor (verifica se a flag específica está ativa)
                if (ShouldApplyColorVariations() && enableColorVariation)
                {
                    ApplyRandomColor();
                }

                // Marca como inicializado
                hasBeenInitialized = true;

                if (showDebugLogs)
                {
                    // Random variations applied
                }
            }
            finally
            {
                // Restaura o estado original do Random
                Random.state = originalRandomState;
            }
        }

        /// <summary>
        /// Reseta todas as variações para os valores originais.
        /// </summary>
        public void ResetToOriginal()
        {
            if (spriteRenderer == null) return;

            transform.localScale = originalScale;
            spriteRenderer.color = originalColor;
            spriteRenderer.flipX = originalFlipX;
            spriteRenderer.flipY = originalFlipY;

            hasBeenInitialized = false;

            if (showDebugLogs)
            {
                // Reset to original
            }
        }

        /// <summary>
        /// Força uma nova aplicação de variações (gera nova seed).
        /// </summary>
        public void ForceNewVariations()
        {
            hasBeenInitialized = false;
            GenerateSeed();
            ApplyRandomVariations();
        }
        #endregion

        #region Private Methods
        private void ApplyRandomFlip()
        {
            bool newFlipX = originalFlipX;
            bool newFlipY = originalFlipY;

            if (enableFlipX)
            {
                newFlipX = Random.value > 0.5f;
            }

            if (enableFlipY)
            {
                newFlipY = Random.value > 0.5f;
            }

            spriteRenderer.flipX = newFlipX;
            spriteRenderer.flipY = newFlipY;

            if (showDebugLogs && (newFlipX != originalFlipX || newFlipY != originalFlipY))
            {
                // Flip applied
            }
        }

        private void ApplyRandomScale()
        {
            Vector3 newScale = originalScale;

            if (uniformScale)
            {
                // Escala uniforme
                float scaleMultiplier = Random.Range(scaleRange.x, scaleRange.y);
                newScale = new Vector3(
                    originalScale.x * scaleMultiplier,
                    originalScale.y * scaleMultiplier,
                    originalScale.z
                );
            }
            else
            {
                // Escala independente para X e Y
                float scaleX = Random.Range(scaleRange.x, scaleRange.y);
                float scaleY = Random.Range(scaleRange.x, scaleRange.y);
                newScale = new Vector3(
                    originalScale.x * scaleX,
                    originalScale.y * scaleY,
                    originalScale.z
                );
            }

            transform.localScale = newScale;

            if (showDebugLogs)
            {
                // Scale applied
            }
        }

        private void ApplyRandomColor()
        {
            Color newColor = originalColor;

            if (useColorPalette && colorPalette != null && colorPalette.Length > 0)
            {
                // Seleciona cor da paleta
                int randomIndex = Random.Range(0, colorPalette.Length);
                newColor = colorPalette[randomIndex];
            }
            else if (useColorInterpolation)
            {
                // Interpolação de cor com base em cor base
                float r = Mathf.Clamp01(baseColor.r + Random.Range(-colorVariation, colorVariation));
                float g = Mathf.Clamp01(baseColor.g + Random.Range(-colorVariation, colorVariation));
                float b = Mathf.Clamp01(baseColor.b + Random.Range(-colorVariation, colorVariation));
                newColor = new Color(r, g, b, baseColor.a);
            }
            else
            {
                // Tint range (padrão)
                float rVariation = Random.Range(-colorTintRange.r, colorTintRange.r);
                float gVariation = Random.Range(-colorTintRange.g, colorTintRange.g);
                float bVariation = Random.Range(-colorTintRange.b, colorTintRange.b);

                newColor.r = Mathf.Clamp01(originalColor.r + rVariation);
                newColor.g = Mathf.Clamp01(originalColor.g + gVariation);
                newColor.b = Mathf.Clamp01(originalColor.b + bVariation);

                // Alpha só é modificado se colorTintRange.a > 0
                if (colorTintRange.a > 0 && randomizeAlpha)
                {
                    float aVariation = Random.Range(-colorTintRange.a, colorTintRange.a);
                    newColor.a = Mathf.Clamp01(originalColor.a + aVariation);
                }
            }

            // Aplica alpha aleatório se ativado e paleta selecionada
            if (randomizeAlpha && useColorPalette)
            {
                float randomAlpha = Random.Range(minAlpha, maxAlpha);
                newColor.a = randomAlpha;
            }

            SetCurrentColor(newColor);

            if (showDebugLogs)
            {
                // Color applied
            }
        }

        private void GetCurrentColor(out Color color)
        {
            if (spriteRenderer != null)
            {
                color = spriteRenderer.color;
            }
            else if (meshRenderer != null)
            {
                color = meshRenderer.material.color;
            }
            else
            {
                color = Color.white;
            }
        }

        private void SetCurrentColor(Color color)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
            else if (meshRenderer != null)
            {
                meshRenderer.material.color = color;
            }
            else if (showDebugLogs)
            {
            }
        }
        #endregion

        #region Context Menu (Editor Only)
        [ContextMenu("Apply Random Variations")]
        private void ContextApplyVariations()
        {
            InitializeComponent();
            hasBeenInitialized = false;
            ApplyRandomVariations();
        }

        [ContextMenu("Reset to Original")]
        private void ContextResetToOriginal()
        {
            ResetToOriginal();
        }

        [ContextMenu("Rerroll Variations")]
        private void ContextForceNewVariations()
        {
            ForceNewVariations();
        }

        [ContextMenu("Rerroll Scale Only")]
        private void ContextRerrollScaleOnly()
        {
            if (!hasBeenInitialized) InitializeComponent();
            
            Random.State originalRandomState = Random.state;
            Random.InitState(seedValue);
            
            if (enableScaleVariation)
            {
                ApplyRandomScale();
            }
            
            Random.state = originalRandomState;
        }

        [ContextMenu("Rerroll Color Only")]
        private void ContextRerrollColorOnly()
        {
            if (!hasBeenInitialized) InitializeComponent();
            
            Random.State originalRandomState = Random.state;
            Random.InitState(seedValue);
            
            if (enableColorVariation)
            {
                ApplyRandomColor();
            }
            
            Random.state = originalRandomState;
        }

        [ContextMenu("Rerroll Flip Only")]
        private void ContextRerrollFlipOnly()
        {
            if (!hasBeenInitialized) InitializeComponent();
            
            Random.State originalRandomState = Random.state;
            Random.InitState(seedValue);
            
            if ((enableFlipX || enableFlipY) && spriteRenderer != null)
            {
                ApplyRandomFlip();
            }
            
            Random.state = originalRandomState;
        }

        [ContextMenu("Debug Info")]
        private void ContextDebugInfo()
        {
            if (spriteRenderer != null)
            {
            }

            // Mostra quais variações seriam aplicadas
            bool shouldApplyVariations = !hasBeenInitialized &&
                                       (applyRandomVariations || applyFlipVariations || applyScaleVariations || applyColorVariations);
        }
        #endregion

        #region Public Properties
        public Vector3 OriginalScale => originalScale;
        public Color OriginalColor => originalColor;
        public Vector3 CurrentScale => transform.localScale;
        public Color CurrentColor 
        { 
            get 
            { 
                GetCurrentColor(out Color color);
                return color;
            } 
        }
        #endregion
    }
}
