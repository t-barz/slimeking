using UnityEngine;
using System.Collections;

/// <summary>
/// Gerencia portais para transição entre cenas
/// Versão otimizada com cache de componentes e validações melhoradas
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PortalManager : MonoBehaviour
{
    [Header("Configurações do Portal")]
    [SerializeField] private string portalID = "";
    [SerializeField] private string destinationScene;
    [SerializeField] private Vector2 targetPosition = Vector2.zero;

    [Header("Configurações Visuais")]
    [SerializeField] private Color portalColor = Color.black;

    [Header("Configurações de Depuração")]
    [SerializeField] private bool showGizmos = true;

    private bool isTransitioning = false;

    // Cache para otimização
    private Collider2D cachedCollider;
    private GameManager cachedGameManager;

    // Constantes para evitar string allocations
    private const string PLAYER_TAG = "Player";

    #region Unity Lifecycle
    private void Start()
    {
        InitializePortal();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PLAYER_TAG) && !isTransitioning && CanUsePortal())
        {
            StartCoroutine(UsePortal());
        }
    }

    private void OnDrawGizmos()
    {
        if (showGizmos)
        {
            DrawPortalGizmos();
        }
    }
    #endregion

    #region Portal Logic
    /// <summary>
    /// Inicializa o portal com cache de componentes
    /// </summary>
    private void InitializePortal()
    {
        // Cache do Collider2D
        cachedCollider = GetComponent<Collider2D>();
        cachedCollider.isTrigger = true;

        // Gera ID automaticamente se não estiver definido
        if (string.IsNullOrEmpty(portalID))
        {
            portalID = $"Portal_{gameObject.name}_{GetInstanceID()}";
        }

        // Cache do GameManager
        cachedGameManager = GameManager.Instance;

        ValidateConfiguration();

        Debug.Log($"[PortalManager] Portal '{portalID}' inicializado - Destino: {destinationScene} | Posição: {targetPosition}");
    }

    /// <summary>
    /// Valida a configuração do portal
    /// </summary>
    private void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(destinationScene))
        {
            Debug.LogWarning($"[PortalManager] Portal '{portalID}' não tem cena de destino configurada!");
        }
    }

    /// <summary>
    /// Verifica se o portal pode ser usado
    /// </summary>
    private bool CanUsePortal()
    {
        // Usa cache do GameManager
        if (cachedGameManager == null)
        {
            cachedGameManager = GameManager.Instance;
        }

        if (cachedGameManager == null)
        {
            Debug.LogError($"[PortalManager] GameManager não encontrado! Portal: {portalID}");
            return false;
        }

        // Verifica se a cena de destino foi definida
        if (string.IsNullOrEmpty(destinationScene))
        {
            Debug.LogError($"[PortalManager] Cena de destino não definida! Portal: {portalID}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Usa o portal para transportar o jogador
    /// </summary>
    private IEnumerator UsePortal()
    {
        isTransitioning = true;

        Debug.Log($"[PortalManager] Portal '{portalID}' ativado - Transportando para {destinationScene}");

        try
        {
            // Usa cache do GameManager para melhor performance
            cachedGameManager.ChangeScene(destinationScene, targetPosition, portalColor);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[PortalManager] Erro ao usar portal '{portalID}': {e.Message}");
            isTransitioning = false;
            yield break; // Exit the coroutine if there's an exception
        }

        // Aguarda um frame para evitar múltiplas ativações
        yield return null;
    }
    #endregion

    #region Debug & Gizmos
    /// <summary>
    /// Desenha gizmos para visualização no editor
    /// </summary>
    private void DrawPortalGizmos()
    {
        // Desenha o portal atual
        Gizmos.color = portalColor;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);

        // Desenha linha para posição de destino (se não for zero)
        if (targetPosition != Vector2.zero)
        {
            Gizmos.color = Color.green;
            Vector3 targetPos3D = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
            Gizmos.DrawLine(transform.position, targetPos3D);
            Gizmos.DrawSphere(targetPos3D, 0.3f);
        }

        // Label com informações incluindo ID
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1f,
            $"ID: {portalID}\nPortal: {destinationScene}\nPos: {targetPosition}");
#endif
    }

    /// <summary>
    /// Força reset do estado de transição (para debug)
    /// </summary>
    [ContextMenu("Reset Transition State")]
    public void ResetTransitionState()
    {
        isTransitioning = false;
        Debug.Log($"[PortalManager] Estado de transição resetado para portal '{portalID}'");
    }

    /// <summary>
    /// Gera um novo ID único para o portal
    /// </summary>
    [ContextMenu("Generate New Portal ID")]
    public void GenerateNewPortalID()
    {
        portalID = $"Portal_{gameObject.name}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";
        Debug.Log($"[PortalManager] Novo ID gerado: {portalID}");
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Obtém o ID do portal
    /// </summary>
    public string GetPortalID()
    {
        return portalID;
    }

    /// <summary>
    /// Define novo ID para o portal
    /// </summary>
    public void SetPortalID(string id)
    {
        portalID = id;
        Debug.Log($"[PortalManager] ID do portal alterado para: {id}");
    }

    /// <summary>
    /// Define nova cena de destino
    /// </summary>
    public void SetDestinationScene(string sceneName)
    {
        destinationScene = sceneName;
        Debug.Log($"[PortalManager] Portal '{portalID}' - Cena de destino alterada para: {sceneName}");
    }

    /// <summary>
    /// Define nova posição de destino
    /// </summary>
    public void SetTargetPosition(Vector2 position)
    {
        targetPosition = position;
        Debug.Log($"[PortalManager] Portal '{portalID}' - Posição de destino alterada para: {position}");
    }

    /// <summary>
    /// Define cor do portal
    /// </summary>
    public void SetPortalColor(Color color)
    {
        portalColor = color;
        Debug.Log($"[PortalManager] Portal '{portalID}' - Cor alterada para: {color}");
    }
    #endregion
}
