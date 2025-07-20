using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PortalManager : MonoBehaviour
{
    [Header("Configurações do Portal")]
    [SerializeField] private string destinationScene;

    private bool isTransitioning = false;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            //StartCoroutine(LoadSceneAsync());
            GameManager.Instance.ChangeScene(destinationScene, Vector2.zero, Color.black);
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        isTransitioning = true;



        // Carrega a nova cena de forma assíncrona
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destinationScene);

        // Aguarda o carregamento terminar
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }
}
