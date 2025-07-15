using UnityEngine;

/// <summary>
/// Define as propriedades e comportamentos de um fragmento elemental coletável.
/// </summary>
public class ElementalFragmentStatus : MonoBehaviour
{
    /// <summary>
    /// Tipos de elementos disponíveis para fragmentos elementais.
    /// </summary>
    public enum ElementType
    {
        Fire,
        Water,
        Earth,
        Air
    }

    [Header("Configurações do Fragmento")]
    [Tooltip("O tipo de elemento deste fragmento")]
    [SerializeField] private ElementType elementType = ElementType.Fire;

    [Tooltip("Quantidade de pontos que este fragmento vale")]
    [SerializeField] private int pointValue = 1;

    [Tooltip("Efeito visual ao coletar (opcional)")]
    [SerializeField] private GameObject collectEffect;

    [Header("Configurações de Aparência")]
    [Tooltip("Variação máxima de matiz da cor (0-1)")]
    [Range(0f, 0.1f)]
    [SerializeField] private float hueVariation = 0.05f;

    [Tooltip("Variação máxima de saturação da cor (0-1)")]
    [Range(0f, 0.2f)]
    [SerializeField] private float saturationVariation = 0.1f;

    [Tooltip("Variação máxima de brilho da cor (0-1)")]
    [Range(0f, 0.2f)]
    [SerializeField] private float valueVariation = 0.1f;

    [Header("Configurações de Debug")]
    [Tooltip("Ativa logs de debug para este fragmento")]
    [SerializeField] private bool enableDebugLogs = false;

    /// <summary>
    /// Obtém o tipo de elemento deste fragmento.
    /// </summary>
    public ElementType Type => elementType;

    /// <summary>
    /// Obtém a quantidade de pontos que este fragmento vale.
    /// </summary>
    public int Points => pointValue;

    /// <summary>
    /// Inicializa o fragmento com uma pequena variação de cor
    /// </summary>
    private void Start()
    {
        ApplyColorVariation();
    }

    /// <summary>
    /// Aplica uma pequena variação aleatória na cor dos sprites do objeto
    /// </summary>
    private void ApplyColorVariation()
    {
        // Obtém todos os SpriteRenderer do objeto e filhos
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        if (renderers.Length == 0)
            return;

        foreach (SpriteRenderer renderer in renderers)
        {
            // Obtém a cor original do sprite
            Color originalColor = renderer.color;

            // Converte para HSV para fazer ajustes mais naturais
            Color.RGBToHSV(originalColor, out float h, out float s, out float v);

            // Aplica variações aleatórias dentro dos limites definidos
            h += Random.Range(-hueVariation, hueVariation);
            s += Random.Range(-saturationVariation, saturationVariation);
            v += Random.Range(-valueVariation, valueVariation);

            // Mantém os valores dentro de limites válidos (0-1)
            h = Mathf.Repeat(h, 1f); // Hue é cíclico, então usamos Repeat
            s = Mathf.Clamp01(s);
            v = Mathf.Clamp01(v);

            // Converte de volta para RGB
            Color variedColor = Color.HSVToRGB(h, s, v);

            // Mantém o alpha original
            variedColor.a = originalColor.a;

            // Aplica a nova cor
            renderer.color = variedColor;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"Variação de cor aplicada ao fragmento {elementType}");
        }
    }

    /// <summary>
    /// Configura o tipo e valor do fragmento.
    /// </summary>
    public void SetFragmentValues(ElementType type, int points)
    {
        elementType = type;
        pointValue = points;

        if (enableDebugLogs)
        {
            Debug.Log($"Fragmento configurado: {elementType}, {pointValue} pontos");
        }

        // Aplica variação de cor quando os valores são alterados programaticamente
        ApplyColorVariation();
    }

    /// <summary>
    /// Chamado quando o fragmento é coletado.
    /// </summary>
    public void OnCollect()
    {
        // Spawna efeito de coleta se configurado
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (enableDebugLogs)
        {
            Debug.Log($"Fragmento {elementType} coletado: +{pointValue} pontos");
        }

        // Destrói o fragmento após ser coletado
        Destroy(gameObject);
    }
}
