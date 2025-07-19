using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PortalManager : MonoBehaviour
{
    [Header("Configurações do Portal")]
    [SerializeField] private string destinationScene;
    [SerializeField] private Vector2 spawnPosition;

    [Header("Efeito de Transição")]
    [SerializeField] private SquareTransition transitionEffect;

    private bool isTransitioning = false;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        // Busca SquareTransition na cena se não foi atribuído
        if (transitionEffect == null)
        {
            transitionEffect = FindAnyObjectByType<SquareTransition>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            StartCoroutine(LoadSceneAsync());
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        isTransitioning = true;

        // Salva dados do jogador antes da transição
        if (SavePlayerManager.Instance != null)
        {
            SavePlayerManager.Instance.AutoSavePlayerData();
        }

        // Salva a posição de spawn para a próxima cena
        PlayerPrefs.SetFloat("PortalDestinationX", spawnPosition.x);
        PlayerPrefs.SetFloat("PortalDestinationY", spawnPosition.y);
        PlayerPrefs.Save();

        // Inicia efeito de transição se disponível
        if (transitionEffect != null)
        {
            bool transitionCompleted = false;

            // Conecta ao evento de conclusão da transição
            System.Action onTransitionCompleted = () =>
            {
                transitionCompleted = true;
            };

            transitionEffect.onTransitionComplete += onTransitionCompleted;

            // Inicia a transição
            transitionEffect.StartTransition();

            // Aguarda a transição terminar
            while (!transitionCompleted)
            {
                yield return null;
            }

            // Remove o listener
            transitionEffect.onTransitionComplete -= onTransitionCompleted;
        }

        // Carrega a nova cena de forma assíncrona
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destinationScene);

        // Aguarda o carregamento terminar
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
