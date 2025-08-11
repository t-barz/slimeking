using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ferramenta de depuração para encontrar e corrigir problemas com InteractionHandlers
/// </summary>
public class InteractionDebugger : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Ativar depuração visual de todos os InteractionHandlers")]
    [SerializeField] private bool enableVisualDebugging = true;

    [Tooltip("Raio dos gizmos de depuração")]
    [SerializeField] private float debugRadius = 0.5f;

    [Tooltip("Atualizar todos os InteractionHandlers na cena")]
    [SerializeField] private bool refreshAllHandlers = true;

    // Armazena todos os handlers na cena
    private List<InteractionHandler> allHandlers = new List<InteractionHandler>();

    private void Start()
    {
        // Encontra todos os handlers na cena
        if (refreshAllHandlers)
        {
            FindAllHandlers();
        }
    }

    /// <summary>
    /// Encontra todos os InteractionHandlers na cena
    /// </summary>
    [ContextMenu("Encontrar Todos os Handlers")]
    public void FindAllHandlers()
    {
        allHandlers.Clear();
        InteractionHandler[] handlers = FindObjectsByType<InteractionHandler>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (var handler in handlers)
        {
            allHandlers.Add(handler);
        }

        Debug.Log($"Encontrados {allHandlers.Count} InteractionHandlers na cena");
    }

    /// <summary>
    /// Testa a conexão com o InputManager em todos os handlers
    /// </summary>
    [ContextMenu("Testar Conexão de Todos")]
    public void TestAllConnections()
    {
        if (allHandlers.Count == 0)
        {
            FindAllHandlers();
        }

        foreach (var handler in allHandlers)
        {
            handler.TestInputConnection();
        }
    }

    /// <summary>
    /// Força a verificação de colisão em todos os handlers
    /// </summary>
    [ContextMenu("Verificar Colisões Manualmente")]
    public void ForceAllCollisionChecks()
    {
        if (allHandlers.Count == 0)
        {
            FindAllHandlers();
        }

        foreach (var handler in allHandlers)
        {
            handler.ForceCollisionCheck();
        }
    }

    /// <summary>
    /// Visualização para depuração
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!enableVisualDebugging) return;

        // Atualiza a lista se necessário
        if (allHandlers.Count == 0 || refreshAllHandlers)
        {
            FindAllHandlers();
        }

        // Desenha gizmos para cada handler
        foreach (var handler in allHandlers)
        {
            if (handler == null) continue;

            if (handler.IsPlayerInRange())
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(handler.transform.position, debugRadius);
                Gizmos.DrawLine(handler.transform.position, handler.transform.position + Vector3.up * 1.5f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(handler.transform.position, debugRadius * 0.8f);
            }
        }
    }

    private void Update()
    {
        // Verificação periódica se a depuração estiver ativada
        if (enableVisualDebugging && Time.frameCount % 30 == 0)
        {
            // Atualiza a lista para incluir novos objetos ou remover destruídos
            if (refreshAllHandlers)
            {
                FindAllHandlers();
            }
        }
    }
}
