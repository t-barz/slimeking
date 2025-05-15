using UnityEngine;

/// <summary>
/// Implementação específica para objetos com efeito de contorno
/// </summary>
public class OutlineInteractable : Interactable
{
    [Header("Configurações do Contorno")]
    [Tooltip("Material com shader de contorno")]
    [SerializeField] private Material outlineMaterial;
    
    private Material originalMaterial; // Material original do objeto
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    protected override void ShowVisualFeedback()
    {
        base.ShowVisualFeedback();
        spriteRenderer.material = outlineMaterial;
    }

    protected override void HideVisualFeedback()
    {
        base.HideVisualFeedback();
        spriteRenderer.material = originalMaterial;
    }

    public override void Interact()
    {
        // Implemente aqui a lógica específica de interação
        Debug.Log("Interagiu com objeto de contorno: " + gameObject.name);
    }
}
