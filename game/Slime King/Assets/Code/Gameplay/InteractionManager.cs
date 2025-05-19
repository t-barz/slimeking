using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controla todas as interações do jogo usando o novo Input System
/// </summary>
public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; private set; }

    [Header("Configurações de Input")]
    [Tooltip("Referência à ação de interação")]
    [SerializeField] private InputActionReference interactAction;

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
    /// Registra novo objeto interativo como atual
    /// </summary>
    public void RegisterInteractable(Interactable newInteractable)
    {
        currentInteractable = newInteractable;
    }

    /// <summary>
    /// Remove registro do objeto interativo atual
    /// </summary>
    public void UnregisterInteractable()
    {
        currentInteractable = null;
    }

    private void OnDestroy()
    {
        interactAction.action.performed -= OnInteract;
    }
}
