using UnityEngine;

/// <summary>
/// Classe base abstrata para todos os objetos interativos do jogo.
/// Fornece detecção de proximidade do jogador e métodos para feedback visual.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
    [Header("Configurações Básicas")]
    [Tooltip("Sprite do botão de interação (ex: tecla 'E')")]
    [SerializeField] protected Sprite interactionButtonSprite;

    [Tooltip("Cor do contorno quando interagível")]
    [SerializeField] protected Color outlineColor = Color.white;

    // Propriedade pública somente leitura para saber se o jogador está próximo
    public bool PlayerInRange { get; protected set; }

    /// <summary>
    /// Detecta quando o jogador entra na área de interação.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = true;
            ShowVisualFeedback();
            // Registra este objeto como interativo atual no InteractionManager
            InteractionManager.Instance.RegisterInteractable(this);
        }
    }

    /// <summary>
    /// Detecta quando o jogador sai da área de interação.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInRange = false;
            HideVisualFeedback();
            // Remove este objeto do registro de interação
            InteractionManager.Instance.UnregisterInteractable();
        }
    }

    /// <summary>
    /// Método virtual para mostrar feedback visual ao jogador.
    /// Pode ser sobrescrito nas classes filhas.
    /// </summary>
    protected virtual void ShowVisualFeedback()
    {
        // Implementação base vazia. As filhas podem sobrescrever.
    }

    /// <summary>
    /// Método virtual para esconder feedback visual.
    /// Pode ser sobrescrito nas classes filhas.
    /// </summary>
    protected virtual void HideVisualFeedback()
    {
        // Implementação base vazia. As filhas podem sobrescrever.
    }

    /// <summary>
    /// Método abstrato que deve ser implementado nas classes filhas.
    /// Define o que acontece quando o jogador interage com o objeto.
    /// </summary>
    public abstract void Interact();
}
