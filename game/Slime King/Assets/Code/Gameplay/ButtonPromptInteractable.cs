using UnityEngine;

/// <summary>
/// Implementação para objetos que exigem prompt de botão na UI
/// </summary>
public class ButtonPromptInteractable : Interactable
{
    [Header("Configurações de UI")]
    [Tooltip("Prefab com elemento UI do botão")]
    [SerializeField] private GameObject buttonPromptPrefab;
    
    [Tooltip("Offset de posição do prompt")]
    [SerializeField] private Vector2 uiOffset = new Vector2(0, 1.5f);

    private GameObject currentPrompt;

    protected override void ShowVisualFeedback()
    {
        base.ShowVisualFeedback();
        if (buttonPromptPrefab != null)
        {
            Vector3 spawnPosition = transform.position + (Vector3)uiOffset;
            currentPrompt = Instantiate(buttonPromptPrefab, spawnPosition, Quaternion.identity);
        }
    }

    protected override void HideVisualFeedback()
    {
        base.HideVisualFeedback();
        if (currentPrompt != null)
        {
            Destroy(currentPrompt);
        }
    }

    public override void Interact()
    {
        // Exemplo de implementação de diálogo
        DialogueSystem.Instance.StartDialogue(new[] {
            "Isso é um exemplo de diálogo!",
            "Pressione ESPAÇO para continuar..."
        });
    }
}
