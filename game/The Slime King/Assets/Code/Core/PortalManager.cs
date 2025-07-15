using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gerencia o comportamento de um portal, incluindo ativação/desativação e transição entre cenas.
/// </summary>
public class PortalManager : MonoBehaviour
{
    [Header("Configurações do Portal")]
    [Tooltip("Define se o portal está ativo ou não")]
    [SerializeField] private bool portalActive = true;

    [Tooltip("Raio do portal para colisões e visualização")]
    [SerializeField] private float portalRadius = 1f;

    [Header("Configurações de Destino")]
    [Tooltip("Nome da cena para onde o portal leva (deixe vazio para permanecer na mesma cena)")]
    [SerializeField] private string targetSceneName = "";

    [Tooltip("Posição X,Y de destino após atravessar o portal")]
    [SerializeField] private Vector2 targetPosition = Vector2.zero;

    // Flag que indica se uma transição está em andamento
    private static bool isTransitioning = false;

    /// <summary>
    /// Propriedade para ativar/desativar o portal externamente
    /// </summary>
    public bool IsActive
    {
        get { return portalActive; }
        set { SetPortalActive(value); }
    }

    /// <summary>
    /// Configura a inicialização do portal
    /// </summary>
    private void Start()
    {
        UpdatePortalState();

        // Adiciona um trigger collider se não tiver nenhum collider
        if (GetComponent<Collider2D>() == null)
        {
            CircleCollider2D collider = gameObject.AddComponent<CircleCollider2D>();
            collider.radius = portalRadius;
            collider.isTrigger = true;
        }
    }

    /// <summary>
    /// Ativa ou desativa o portal
    /// </summary>
    public void SetPortalActive(bool active)
    {
        if (portalActive != active)
        {
            portalActive = active;
            UpdatePortalState();
        }
    }

    /// <summary>
    /// Alterna o estado do portal (ativo/inativo)
    /// </summary>
    public void TogglePortalActive()
    {
        SetPortalActive(!portalActive);
    }

    /// <summary>
    /// Atualiza o estado visual e funcional do portal
    /// </summary>
    private void UpdatePortalState()
    {
        // Ativa/desativa os colliders
        Collider2D portalCollider = GetComponent<Collider2D>();
        if (portalCollider != null)
        {
            portalCollider.enabled = portalActive;
        }

        // Atualiza visuais do portal (se houver)
        Renderer portalRenderer = GetComponent<Renderer>();
        if (portalRenderer != null)
        {
            portalRenderer.enabled = portalActive;
        }

        // Aqui você pode adicionar efeitos visuais ou sonoros quando o estado muda
        if (portalActive)
        {
            Debug.Log("Portal ativado");
        }
        else
        {
            Debug.Log("Portal desativado");
        }
    }

    /// <summary>
    /// Detecta quando o jogador entra no portal
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o portal está ativo e se o objeto que entrou é o jogador
        if (portalActive && other.CompareTag("Player") && !isTransitioning)
        {
            // Transporta o jogador para o destino
            TransportPlayer(other.gameObject);
        }
    }

    /// <summary>
    /// Transporta o jogador para o destino configurado
    /// </summary>
    private void TransportPlayer(GameObject player)
    {
        // Verifica se estamos mudando de cena
        bool isChangingScene = !string.IsNullOrEmpty(targetSceneName) &&
                               targetSceneName != SceneManager.GetActiveScene().name;

        if (isChangingScene)
        {
            // Guarda a posição de destino para usar após carregar a cena
            PlayerPrefs.SetFloat("PortalDestinationX", targetPosition.x);
            PlayerPrefs.SetFloat("PortalDestinationY", targetPosition.y);
            PlayerPrefs.Save();

            // Inicia a transição para nova cena
            StartCoroutine(LoadNewScene());
        }
        else
        {
            // Apenas move o jogador dentro da mesma cena
            player.transform.position = targetPosition;

            // Opcional: Efeito visual ou sonoro ao teleportar
            Debug.Log($"Jogador teleportado para {targetPosition}");
        }
    }

    /// <summary>
    /// Carrega uma nova cena e posiciona o jogador no destino
    /// </summary>
    private IEnumerator LoadNewScene()
    {
        // Marca que estamos em transição
        isTransitioning = true;

        // Carrega a nova cena
        SceneManager.LoadScene(targetSceneName);

        // Espera um frame para garantir que a cena foi carregada
        yield return null;

        // Espera mais um pouco para garantir que tudo inicializou
        yield return new WaitForSeconds(0.2f);

        // Posiciona o jogador no ponto de destino
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = new Vector3(
                PlayerPrefs.GetFloat("PortalDestinationX", targetPosition.x),
                PlayerPrefs.GetFloat("PortalDestinationY", targetPosition.y),
                player.transform.position.z
            );
        }

        // Finaliza a transição
        isTransitioning = false;
    }
}
