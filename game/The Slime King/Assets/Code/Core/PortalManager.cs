using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gerencia o comportamento de um portal, incluindo ativação/desativação e transição entre cenas.
/// </summary>
public class PortalManager : MonoBehaviour
{
    [Header("Configurações de Destino")]
    [Tooltip("Nome da cena para onde o portal leva (deixe vazio para permanecer na mesma cena)")]
    [SerializeField] private string targetSceneName = "";

    [Tooltip("Posição X,Y de destino após atravessar o portal")]
    [SerializeField] private Vector2 targetPosition = Vector2.zero;

    [Header("Configurações de Transição")]
    [Tooltip("Referência ao componente SquareTransition para efeito visual")]
    [SerializeField] private SquareTransition transitionEffect;

    // Flag que indica se uma transição está em andamento
    private static bool isTransitioning = false;

    /// <summary>
    /// Configura a inicialização do portal
    /// </summary>
    private void Start()
    {

        // Busca o componente de transição se não estiver configurado
        if (transitionEffect == null)
        {
            // Procura no mesmo objeto
            transitionEffect = GetComponent<SquareTransition>();
        }
    }

    /// <summary>
    /// Detecta quando o jogador entra no portal
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o portal está ativo e se o objeto que entrou é o jogador
        if (other.CompareTag("Player") && !isTransitioning)
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

        // Inicia o efeito de transição visual se disponível
        bool transitionCompleted = false;
        if (transitionEffect != null)
        {
            // Inscreve no evento de conclusão
            void OnTransitionCompleted()
            {
                transitionCompleted = true;
            }
            // Aguarda o evento de conclusão
            while (!transitionCompleted)
                yield return null;
        }

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

        // Se temos um efeito de transição, inicia o efeito reverso para revelar a cena
        transitionCompleted = false;
        if (transitionEffect != null)
        {
            void OnTransitionCompletedReverse()
            {
                transitionCompleted = true;
            }
            while (!transitionCompleted)
                yield return null;
        }

        isTransitioning = false;
    }
}
