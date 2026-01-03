using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// [DEPRECATED] Use SetupVisualEnvironment ao invés desta classe.
    /// 
    /// Esta classe foi unificada com SetupVisualEnvironment que oferece:
    /// - Todas as funcionalidades de RandomStyle
    /// - Sistema de flags hierárquicas melhor
    /// - Aplicação em tempo de edição (ExecuteInEditMode)
    /// - Context menus para rerrolagem de variações individuais
    /// - Suporte a Renderer genérico (não apenas SpriteRenderer)
    /// - Melhor controle de cor (paleta, interpolação e tint)
    /// 
    /// Componente deprecado será removido na próxima versão.
    /// </summary>
    [System.Obsolete("RandomStyle está deprecado. Use SetupVisualEnvironment ao invés.", false)]
    public class RandomStyle : MonoBehaviour
{
    [Header("📏 Configurações de Tamanho")]
    [Tooltip("Se true, randomiza o tamanho do objeto")]
    [SerializeField] private bool randomizeScale = true;

    [Tooltip("Tamanho mínimo (escala) do objeto")]
    [SerializeField] private float minScale = 0.8f;

    [Tooltip("Tamanho máximo (escala) do objeto")]
    [SerializeField] private float maxScale = 1.2f;

    [Tooltip("Se true, mantém proporção uniforme (X, Y, Z iguais)")]
    [SerializeField] private bool uniformScale = true;

    [Header("Color Configuration")]
    [Tooltip("Se true, randomiza a cor do objeto")]
    [SerializeField] private bool randomizeColor = true;

    [Tooltip("Lista de cores possíveis para randomização")]
    [SerializeField] private Color[] possibleColors = { Color.white, Color.red, Color.green, Color.blue, Color.yellow };

    [Tooltip("Se true, usa interpolação entre cores ao invés de seleção direta")]
    [SerializeField] private bool useColorInterpolation = false;

    [Tooltip("Cor base para interpolação (quando useColorInterpolation = true)")]
    [SerializeField] private Color baseColor = Color.white;

    [Tooltip("Variação máxima de cada canal de cor (0-1)")]
    [SerializeField] private float colorVariation = 0.3f;

    [Header("Advanced Options")]
    [Tooltip("Se true, aplica randomização automaticamente no Start()")]
    [SerializeField] private bool applyOnStart = true;

    [Tooltip("Se true, randomiza também o canal Alpha da cor")]
    [SerializeField] private bool randomizeAlpha = false;

    [Tooltip("Valor mínimo para o canal Alpha")]
    [SerializeField] private float minAlpha = 0.7f;

    [Tooltip("Valor máximo para o canal Alpha")]
    [SerializeField] private float maxAlpha = 1f;

    [Header("🔀 Direção Aleatória")]
    [Tooltip("Permite direção aleatória no eixo X (flipX)")]
    [SerializeField] private bool allowRandomFlipX = false;

    [Tooltip("Permite direção aleatória no eixo Y (flipY)")]
    [SerializeField] private bool allowRandomFlipY = false;

    [Header("🔍 Debug")]
    [Tooltip("Se true, mostra logs das alterações aplicadas")]
    [SerializeField] private bool enableLogs = false;

    // Componentes necessários (cache para performance)
    private SpriteRenderer spriteRenderer;
    private Renderer meshRenderer;
    private bool hasValidRenderer = false;

    // Valores originais para referência
    private Vector3 originalScale;
    private Color originalColor;

    // Cache de valores calculados para otimização
    private Vector3 randomScaleVector;
    private Color randomColorValue;

    /// <summary>
    /// Inicialização - guarda valores originais e aplica randomização se configurado
    /// OTIMIZADO: Cache de componentes e validação única
    /// </summary>
    void Start()
    {
        // Cache componentes de renderização uma única vez
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<Renderer>();

        // Valida se tem pelo menos um renderer
        hasValidRenderer = (spriteRenderer != null || meshRenderer != null);

        if (!hasValidRenderer)
        {return;
        }

        // Guarda valores originais
        originalScale = transform.localScale;
        originalColor = GetCurrentColor();

        // Aplica randomização se configurado
        if (applyOnStart)
        {
            ApplyRandomStyle();
        }
    }

    /// <summary>
    /// Aplica randomização de tamanho e cor baseada nas configurações
    /// OTIMIZADO: Early exit se não tem renderer válido
    /// </summary>
    public void ApplyRandomStyle()
    {
        // Early exit se não tem renderer válido
        if (!hasValidRenderer) return;

        if (randomizeScale)
        {
            ApplyRandomScale();
        }

        if (randomizeColor)
        {
            ApplyRandomColor();
        }

        if (enableLogs)
        {
            // Logging removed to fix compilation errors
        }
    }

    private void OnValidate()
    {
        // Flip aleatório só no editor, quando o objeto é colocado ou alterado na cena
        if (allowRandomFlipX)
        {
            float flipX = Random.value < 0.5f ? 1f : -1f;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * flipX, transform.localScale.y, transform.localScale.z);
        }
        if (allowRandomFlipY)
        {
            float flipY = Random.value < 0.5f ? 1f : -1f;
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y) * flipY, transform.localScale.z);
        }
    }

    /// <summary>
    /// Aplica uma escala aleatória ao objeto baseada nos valores configurados
    /// </summary>
    private void ApplyRandomScale()
    {
        if (uniformScale)
        {
            // Escala uniforme - mesmo valor para X, Y, Z
            float randomScale = Random.Range(minScale, maxScale);
            Vector3 newScale = originalScale * randomScale;
            transform.localScale = newScale;
        }
        else
        {
            // Escala não uniforme - valores independentes para cada eixo
            float scaleX = Random.Range(minScale, maxScale);
            float scaleY = Random.Range(minScale, maxScale);
            float scaleZ = Random.Range(minScale, maxScale);

            Vector3 newScale = new Vector3(
                originalScale.x * scaleX,
                originalScale.y * scaleY,
                originalScale.z * scaleZ
            );
            transform.localScale = newScale;
        }
    }

    /// <summary>
    /// Aplica uma cor aleatória ao objeto baseada nas configurações
    /// </summary>
    private void ApplyRandomColor()
    {
        Color newColor;

        if (useColorInterpolation)
        {
            // Usa interpolação baseada na cor base com variação
            newColor = GenerateInterpolatedColor();
        }
        else
        {
            // Seleciona uma cor da lista de cores possíveis
            newColor = SelectRandomColor();
        }

        // Aplica alpha aleatório se configurado
        if (randomizeAlpha)
        {
            float randomAlpha = Random.Range(minAlpha, maxAlpha);
            newColor.a = randomAlpha;
        }

        // Aplica a cor ao componente apropriado
        SetCurrentColor(newColor);
    }

    /// <summary>
    /// Gera uma cor por interpolação baseada na cor base com variação aleatória
    /// </summary>
    private Color GenerateInterpolatedColor()
    {
        float r = Mathf.Clamp01(baseColor.r + Random.Range(-colorVariation, colorVariation));
        float g = Mathf.Clamp01(baseColor.g + Random.Range(-colorVariation, colorVariation));
        float b = Mathf.Clamp01(baseColor.b + Random.Range(-colorVariation, colorVariation));

        return new Color(r, g, b, baseColor.a);
    }

    /// <summary>
    /// Seleciona uma cor aleatória da lista de cores possíveis
    /// </summary>
    private Color SelectRandomColor()
    {
        if (possibleColors == null || possibleColors.Length == 0)
        {
            if (enableLogs)
            {}
            return originalColor;
        }

        int randomIndex = Random.Range(0, possibleColors.Length);
        return possibleColors[randomIndex];
    }

    /// <summary>
    /// Obtém a cor atual do objeto (SpriteRenderer ou Material)
    /// </summary>
    private Color GetCurrentColor()
    {
        if (spriteRenderer != null)
        {
            return spriteRenderer.color;
        }
        else if (meshRenderer != null)
        {
            return meshRenderer.material.color;
        }

        return Color.white; // Fallback
    }

    /// <summary>
    /// Define a cor do objeto (SpriteRenderer ou Material)
    /// </summary>
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
        else if (enableLogs)
        {}
    }

    /// <summary>
    /// Restaura os valores originais de tamanho e cor
    /// </summary>
    public void RestoreOriginalStyle()
    {
        transform.localScale = originalScale;
        SetCurrentColor(originalColor);

        if (enableLogs)
        {}
    }

    /// <summary>
    /// Força uma nova randomização, mesmo que applyOnStart seja false
    /// </summary>
    public void ForceRandomize()
    {
        ApplyRandomStyle();
    }

    /// <summary>
    /// Define uma escala específica (útil para controle externo)
    /// </summary>
    public void SetCustomScale(float scale)
    {
        Vector3 newScale = originalScale * scale;
        transform.localScale = newScale;
    }

    /// <summary>
    /// Define uma cor específica (útil para controle externo)
    /// </summary>
    public void SetCustomColor(Color color)
    {
        SetCurrentColor(color);
    }

    /// <summary>
    /// Aplica apenas randomização de tamanho
    /// </summary>
    public void RandomizeScaleOnly()
    {
        if (randomizeScale)
        {
            ApplyRandomScale();
        }
    }

    /// <summary>
    /// Aplica apenas randomização de cor
    /// </summary>
    public void RandomizeColorOnly()
    {
        if (randomizeColor)
        {
            ApplyRandomColor();
        }
    }

    /// <summary>
    /// Propriedades públicas para acesso externo
    /// </summary>
    public Vector3 OriginalScale => originalScale;
    public Color OriginalColor => originalColor;
    public Vector3 CurrentScale => transform.localScale;
    public Color CurrentColor => GetCurrentColor();
}
}
