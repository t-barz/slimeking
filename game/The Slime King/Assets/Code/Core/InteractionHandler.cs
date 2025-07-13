using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Monitora eventos de interação e registra logs no console.
/// Pode ser usado como componente independente ou como base para sistemas de interação.
/// </summary>
public class InteractionHandler : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Mensagem personalizada para exibir no log quando a interação ocorrer")]
    [SerializeField] private string logMessage = "Ação de interação detectada!";

    [Tooltip("Exibir informações adicionais no log")]
    [SerializeField] private bool detailedLog = true;

    [Tooltip("Exibir mensagem apenas na primeira interação")]
    [SerializeField] private bool logOnlyOnce = false;

    // Controle interno
    private bool firstInteractionLogged = false;

    private void OnEnable()
    {
        // Registra o callback para a ação de interação
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractAction.performed += OnInteractPerformed;
        }
        else
        {
            Debug.LogWarning("InputManager não encontrado. InteractionHandler não funcionará corretamente.");
        }
    }

    private void OnDisable()
    {
        // Remove o callback quando o objeto for desativado
        if (InputManager.Instance != null)
        {
            InputManager.Instance.InteractAction.performed -= OnInteractPerformed;
        }
    }

    /// <summary>
    /// Chamado quando o jogador realiza uma ação de interação.
    /// </summary>
    /// <param name="ctx">Contexto do evento de input</param>
    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        // Verifica se deve registrar apenas uma vez
        if (logOnlyOnce && firstInteractionLogged)
        {
            return;
        }

        // Registra o log básico
        if (detailedLog)
        {
            Debug.Log($"{logMessage} | Valor: {ctx.ReadValueAsObject()} | Fase: {ctx.phase} | Timestamp: {Time.time:F2}s");
        }
        else
        {
            Debug.Log(logMessage);
        }

        // Marca que a primeira interação foi registrada
        firstInteractionLogged = true;

        // Chama o método virtual que pode ser sobrescrito por classes derivadas
        OnInteractionDetected();
    }

    /// <summary>
    /// Método virtual que pode ser sobrescrito por classes derivadas para
    /// adicionar comportamentos personalizados quando uma interação for detectada.
    /// </summary>
    protected virtual void OnInteractionDetected()
    {
        // Implementação vazia por padrão
        // Classes derivadas podem implementar comportamentos específicos aqui
    }

    /// <summary>
    /// Reseta o controle de primeira interação, permitindo que
    /// logs sejam exibidos novamente se a opção logOnlyOnce estiver ativa.
    /// </summary>
    public void ResetInteractionControl()
    {
        firstInteractionLogged = false;
    }
}