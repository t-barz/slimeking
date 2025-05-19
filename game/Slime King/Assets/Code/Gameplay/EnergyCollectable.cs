using UnityEngine;

/// <summary>
/// Implementa um coletável de energia elemental com quantidade específica
/// </summary>
public class EnergyCollectable : CollectableInteractable
{
    /// <summary>
    /// Define os tipos de energia elemental disponíveis
    /// </summary>
    public enum ElementType
    {
        Water,  // Energia de água
        Air,    // Energia de ar
        Earth,  // Energia de terra
        Fire    // Energia de fogo
    }

    [Header("Configurações de Energia")]
    [Tooltip("Tipo de energia elemental")]
    [SerializeField] private ElementType elementType = ElementType.Water;
    
    [Tooltip("Quantidade de energia para coletar")]
    [SerializeField] private int energyAmount = 1;

    [Header("Configurações Visuais")]
    [Tooltip("Cores para cada tipo de elemento")]
    [SerializeField] private Color[] elementColors = new Color[]
    {
        new Color(0, 0.4f, 1f),    // Água: Azul
        new Color(0.8f, 0.8f, 1f),  // Ar: Branco azulado
        new Color(0.4f, 0.2f, 0),   // Terra: Marrom
        new Color(1f, 0.4f, 0)      // Fogo: Vermelho
    };

    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();
        
        // Configura a cor baseada no tipo de elemento
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = elementColors[(int)elementType];
        }
    }

    protected override void CollectItem()
    {
        // Adiciona a energia ao jogador ou sistema relevante
        // Por exemplo: PlayerEnergy.Instance.AddEnergy(elementType, energyAmount);
        
        Debug.Log($"Coletou {energyAmount} de energia {elementType}");
        
        // Chama a implementação base para efeitos visuais e destruição
        base.CollectItem();
    }

    // Getters para acesso externo
    public ElementType Element => elementType;
    public int Amount => energyAmount;
}