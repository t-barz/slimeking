using UnityEngine;

/// <summary>
/// Implementação específica para objetos com efeito de contorno
/// </summary>
public class OutlineInteractable : Interactable
{
    [Header("Configurações do Contorno")]
    [Tooltip("Material com shader de contorno")]
    [SerializeField] private Material outlineMaterial;
    [Tooltip("Cor do contorno quando interagível")]
    [SerializeField] protected Color outlineColor = Color.white;
    
    private Material materialInstance;
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial; // Armazena o material original
    private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material; // Guarda o material original
        
        // Cria uma instância do material para este objeto específico
        materialInstance = new Material(outlineMaterial);
        materialInstance.SetColor(OutlineColorProperty, outlineColor);
    }

    private void OnDestroy()
    {
        // Limpa a instância do material quando o objeto for destruído
        if (materialInstance != null)
        {
            Destroy(materialInstance);
        }
    }

    protected override void ShowVisualFeedback()
    {
        base.ShowVisualFeedback();
        spriteRenderer.material = materialInstance;
    }

    protected override void HideVisualFeedback()
    {
        base.HideVisualFeedback();
        spriteRenderer.material = originalMaterial; // Restaura o material original
    }

    public override void Interact()
    {
        // Implemente aqui a lógica específica de interação
        Debug.Log("Interagiu com objeto de contorno: " + gameObject.name);
    }
}