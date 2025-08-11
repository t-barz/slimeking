using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Controla todas as interações do jogo usando o novo Input System
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("Configurações de Input")]
    [Tooltip("Referência à ação de interação")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Configurações de Interação")]
    [Tooltip("Distância máxima para interação quando há múltiplos objetos interativos")]
    [SerializeField] private float interactionPriority = 2f;
    [Tooltip("Mostrar área de debug para interações")]
    [SerializeField] private bool showDebugInfo = false;

    // Lista de interactables dentro da área de detecção
    private List<Interactable> nearbyInteractables = new List<Interactable>();
    private Interactable currentInteractable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            interactAction.action.Enable();
            interactAction.action.performed += OnInteract;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Callback executado quando o botão de interação é pressionado
    /// </summary>
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteractable != null && currentInteractable.PlayerInRange) // Acessando via propriedade
        {
            currentInteractable.Interact();
        }
    }
    /// <summary>
    /// Registra um objeto interativo na lista de proximidade
    /// </summary>
    public void RegisterInteractable(Interactable newInteractable)
    {
        if (!nearbyInteractables.Contains(newInteractable))
        {
            nearbyInteractables.Add(newInteractable);
            UpdateCurrentInteractable();
        }
    }

    /// <summary>
    /// Remove um objeto interativo da lista de proximidade
    /// </summary>
    public void UnregisterInteractable(Interactable interactable)
    {
        if (nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Remove(interactable);
            UpdateCurrentInteractable();
        }
    }

    /// <summary>
    /// Atualiza qual é o objeto interativo atual baseado na proximidade
    /// </summary>
    private void UpdateCurrentInteractable()
    {
        if (nearbyInteractables.Count == 0)
        {
            currentInteractable = null;
            return;
        }

        // Encontra o interactable mais próximo do player
        Transform playerTransform = Camera.main.transform;
        float closestDistance = float.MaxValue;
        Interactable closestInteractable = null;

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable != null && interactable.PlayerInRange)
            {
                float distance = Vector2.Distance(playerTransform.position, interactable.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }

        currentInteractable = closestInteractable;
    }

    /// <summary>
    /// Limpa a lista de objetos interativos
    /// </summary>
    public void ClearInteractables()
    {
        nearbyInteractables.Clear();
        currentInteractable = null;
    }

    private void OnDestroy()
    {
        interactAction.action.performed -= OnInteract;
    }

    private void OnDrawGizmos()
    {
        if (showDebugInfo && nearbyInteractables != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var interactable in nearbyInteractables)
            {
                if (interactable != null)
                {
                    Gizmos.DrawLine(Camera.main.transform.position, interactable.transform.position);
                    Gizmos.DrawWireSphere(interactable.transform.position, 0.5f);
                }
            }

            if (currentInteractable != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(currentInteractable.transform.position, 0.7f);
            }
        }
    }

    /// <summary>
    /// Retorna o objeto interativo atual
    /// </summary>
    public Interactable GetCurrentInteractable()
    {
        return currentInteractable;
    }

    /// <summary>
    /// Verifica se há algum objeto interativo próximo
    /// </summary>
    public bool HasInteractableNearby()
    {
        return currentInteractable != null;
    }
}
