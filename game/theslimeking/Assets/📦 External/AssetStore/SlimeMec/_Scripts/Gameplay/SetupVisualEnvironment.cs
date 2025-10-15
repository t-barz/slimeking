using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Componente que aplica variações visuais aleatórias ao GameObject quando é colocado em cena.
    /// As configurações são aplicadas uma única vez e persistem durante toda a execução do jogo.
    /// Utiliza ExecuteInEditMode para garantir que as mudanças sejam aplicadas no Editor.
    /// </summary>
    [ExecuteInEditMode]
    public class SetupVisualEnvironment : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Master Control")]
        [SerializeField] private bool applyRandomVariations = false;
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
        [SerializeField] private Color colorTintRange = new Color(0.2f, 0.2f, 0.2f, 0f);
        [Tooltip("Variação máxima para cada canal de cor (R, G, B, A). Alpha = 0 mantém transparência original")]

        [Header("Debug")]
        [SerializeField] private bool showDebugLogs = false;
        [Tooltip("Mostra logs de debug das alterações aplicadas")]
        #endregion

        #region Private Fields
        private SpriteRenderer spriteRenderer;
        private bool hasBeenInitialized = false;

        // Valores originais para referência
        private Vector3 originalScale;
        private Color originalColor;
        private bool originalFlipX;
        private bool originalFlipY;

        // Seed baseada na posição para consistência
        private int seedValue;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeComponent();
        }

        private void Start()
        {
            // Só aplica variações em runtime quando o objeto foi colocado na cena
            if (Application.isPlaying && ShouldApplyVariations())
            {
                ApplyRandomVariations();
            }
        }

        // ExecuteInEditMode - aplicado também no Editor
        private void OnValidate()
        {
            // No Editor, só aplica se não estiver em modo Play
            if (Application.isPlaying) return;

            InitializeComponent();

            // Aplica variações no Editor apenas quando alguma flag estiver ativa
            if (ShouldApplyVariations())
            {
                ApplyRandomVariations();
            }
        }
        #endregion

        #region Initialization
        private void InitializeComponent()
        {
            // Cache do SpriteRenderer
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    if (showDebugLogs)
                        Debug.LogWarning($"SetupVisualEnvironment: SpriteRenderer não encontrado em {gameObject.name}");
                    return;
                }
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
        }

        private void GenerateSeed()
        {
            // Gera seed baseada na posição do objeto para consistência
            Vector3 pos = transform.position;
            seedValue = Mathf.Abs((pos.x + pos.y + pos.z).GetHashCode());

            if (showDebugLogs)
                Debug.Log($"SetupVisualEnvironment: Seed gerada para {gameObject.name}: {seedValue}");
        }

        /// <summary>
        /// Verifica se as variações devem ser aplicadas baseado no estado atual e nas flags.
        /// </summary>
        private bool ShouldApplyVariations()
        {
            // Não aplica se já foi inicializado ou se não há SpriteRenderer
            if (hasBeenInitialized || spriteRenderer == null)
                return false;

            // Verifica se alguma flag está ativa
            bool hasAnyFlagActive = applyRandomVariations || applyFlipVariations || applyScaleVariations || applyColorVariations;

            if (!hasAnyFlagActive)
            {
                if (showDebugLogs)
                    Debug.Log($"SetupVisualEnvironment: Nenhuma flag ativa para {gameObject.name}");
                return false;
            }

            // Se o master flag estiver ativo, verifica se pelo menos uma sub-flag está ativa
            if (applyRandomVariations)
            {
                bool hasSubFlagActive = applyFlipVariations || applyScaleVariations || applyColorVariations;
                if (!hasSubFlagActive)
                {
                    if (showDebugLogs)
                        Debug.Log($"SetupVisualEnvironment: Master flag ativo mas nenhuma sub-flag ativa para {gameObject.name}");
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
            if (spriteRenderer == null || hasBeenInitialized) return;

            // Usa a verificação centralizada
            if (!ShouldApplyVariations())
            {
                if (showDebugLogs)
                    Debug.Log($"SetupVisualEnvironment: Condições não atendidas para aplicar variações em {gameObject.name}");
                return;
            }

            // Usa o seed para randomização consistente
            Random.State originalRandomState = Random.state;
            Random.InitState(seedValue);

            try
            {
                if (showDebugLogs)
                    Debug.Log($"SetupVisualEnvironment: Aplicando variações em {gameObject.name}");

                // Aplica flip aleatório (verifica se a flag específica está ativa)
                if (ShouldApplyFlipVariations() && (enableFlipX || enableFlipY))
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
                    Debug.Log($"SetupVisualEnvironment: Variações aplicadas com sucesso em {gameObject.name}");
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
                Debug.Log($"SetupVisualEnvironment: {gameObject.name} resetado para valores originais");
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
                Debug.Log($"SetupVisualEnvironment: Flip aplicado em {gameObject.name} - X: {newFlipX}, Y: {newFlipY}");
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
                Debug.Log($"SetupVisualEnvironment: Escala aplicada em {gameObject.name} - {newScale}");
        }

        private void ApplyRandomColor()
        {
            Color newColor = originalColor;

            // Aplica variação em cada canal de cor
            float rVariation = Random.Range(-colorTintRange.r, colorTintRange.r);
            float gVariation = Random.Range(-colorTintRange.g, colorTintRange.g);
            float bVariation = Random.Range(-colorTintRange.b, colorTintRange.b);

            newColor.r = Mathf.Clamp01(originalColor.r + rVariation);
            newColor.g = Mathf.Clamp01(originalColor.g + gVariation);
            newColor.b = Mathf.Clamp01(originalColor.b + bVariation);

            // Alpha só é modificado se colorTintRange.a > 0
            if (colorTintRange.a > 0)
            {
                float aVariation = Random.Range(-colorTintRange.a, colorTintRange.a);
                newColor.a = Mathf.Clamp01(originalColor.a + aVariation);
            }

            spriteRenderer.color = newColor;

            if (showDebugLogs)
                Debug.Log($"SetupVisualEnvironment: Cor aplicada em {gameObject.name} - {newColor}");
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

        [ContextMenu("Force New Variations")]
        private void ContextForceNewVariations()
        {
            ForceNewVariations();
        }

        [ContextMenu("Debug Info")]
        private void ContextDebugInfo()
        {
            Debug.Log($"=== SetupVisualEnvironment Debug Info ===");
            Debug.Log($"GameObject: {gameObject.name}");
            Debug.Log($"Master Control - Apply Variations: {applyRandomVariations}");
            Debug.Log($"Individual Flags - Flip: {applyFlipVariations}, Scale: {applyScaleVariations}, Color: {applyColorVariations}");
            Debug.Log($"Has Been Initialized: {hasBeenInitialized}");
            Debug.Log($"Seed Value: {seedValue}");
            Debug.Log($"Original Scale: {originalScale}");
            Debug.Log($"Current Scale: {transform.localScale}");
            if (spriteRenderer != null)
            {
                Debug.Log($"Original Color: {originalColor}");
                Debug.Log($"Current Color: {spriteRenderer.color}");
                Debug.Log($"Original Flip: X={originalFlipX}, Y={originalFlipY}");
                Debug.Log($"Current Flip: X={spriteRenderer.flipX}, Y={spriteRenderer.flipY}");
            }

            // Mostra quais variações seriam aplicadas
            bool shouldApplyVariations = !hasBeenInitialized &&
                                       (applyRandomVariations || applyFlipVariations || applyScaleVariations || applyColorVariations);
            Debug.Log($"Would Apply Variations: {shouldApplyVariations}");
        }
        #endregion
    }
}
