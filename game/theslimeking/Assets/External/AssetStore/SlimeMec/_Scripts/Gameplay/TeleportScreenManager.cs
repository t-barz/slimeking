using UnityEngine;

/// <summary>
/// Singleton responsável por gerenciar a lógica de transição de tela durante o teletransporte.
/// Controla a ordem: efeito de vinheta, troca de cena/posição, destruição e instância do Player.
/// </summary>
public class TeleportScreenManager : MonoBehaviour
{
    private AsyncOperation preloadedSceneOp = null;
    private string preloadedSceneName = null;
    public static TeleportScreenManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Exemplo de método para iniciar uma transição de teleporte

    public void PreloadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        if (preloadedSceneOp != null && preloadedSceneName == sceneName)
        {
            Debug.Log($"[TeleportScreenManager] Cena '{sceneName}' já está pré-carregada.");
            return;
        }
        Debug.Log($"[TeleportScreenManager] Pré-carregando cena '{sceneName}'...");
        preloadedSceneOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        preloadedSceneOp.allowSceneActivation = false;
        preloadedSceneName = sceneName;
    }

    public void ActivatePreloadedSceneAndPlacePlayer(TeleportPointData teleportData, GameObject playerObject)
    {
        if (preloadedSceneOp != null && preloadedSceneName == teleportData.destinationSceneName)
        {
            Debug.Log($"[TeleportScreenManager] Ativando cena pré-carregada '{preloadedSceneName}'...");
            StartCoroutine(ActivateAndPlacePlayerCoroutine(teleportData, playerObject));
        }
        else
        {
            Debug.LogWarning($"[TeleportScreenManager] Nenhuma cena pré-carregada encontrada para '{teleportData.destinationSceneName}'. Carregando normalmente...");
            StartCoroutine(LoadSceneAndPlacePlayer(teleportData, playerObject));
        }
    }

    public void StartTeleportTransition(TeleportPointData teleportData, GameObject playerObject)
    {
        Debug.Log($"[TeleportScreenManager] Iniciando transição de teleporte:");
        Debug.Log($"  - Point ID: {teleportData.pointId}");
        Debug.Log($"  - Point Name: {teleportData.pointName}");
        Debug.Log($"  - Destino: {teleportData.destinationSceneName} @ {teleportData.destinationPosition}");
        Debug.Log($"  - Player: {(playerObject != null ? playerObject.name : "null")}");
        Debug.Log($"  - Layers: {teleportData.playerLayers}");

        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(teleportData.destinationSceneName) && teleportData.destinationSceneName != currentScene)
        {
            // Ativa a cena pré-carregada se existir, senão carrega normalmente
            ActivatePreloadedSceneAndPlacePlayer(teleportData, playerObject);
        }
        else
        {
            // Mesma cena: só move
            if (playerObject != null)
            {
                playerObject.transform.position = teleportData.destinationPosition;
                Debug.Log($"[TeleportScreenManager] Player movido para {teleportData.destinationPosition}");
            }
            else
            {
                Debug.LogWarning("[TeleportScreenManager] PlayerObject é nulo, não foi possível mover.");
            }
        }
    }


    private System.Collections.IEnumerator LoadSceneAndPlacePlayer(TeleportPointData teleportData, GameObject playerObject)
    {
        var asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(teleportData.destinationSceneName);
        while (!asyncLoad.isDone)
            yield return null;

        // Após carregar a cena, tenta encontrar o PlayerController singleton
        var player = PlayerController.Instance;
        if (player != null)
        {
            player.transform.position = teleportData.destinationPosition;
            Debug.Log($"[TeleportScreenManager] Player movido para {teleportData.destinationPosition} após troca de cena");
        }
        else if (playerObject != null)
        {
            // Fallback: tenta mover o player passado
            playerObject.transform.position = teleportData.destinationPosition;
            Debug.Log($"[TeleportScreenManager] Player passado movido para {teleportData.destinationPosition} após troca de cena");
        }
        else
        {
            Debug.LogWarning("[TeleportScreenManager] Player não encontrado após troca de cena.");
        }
    }

    private System.Collections.IEnumerator ActivateAndPlacePlayerCoroutine(TeleportPointData teleportData, GameObject playerObject)
    {
        if (preloadedSceneOp == null)
            yield break;

        preloadedSceneOp.allowSceneActivation = true;
        while (!preloadedSceneOp.isDone)
            yield return null;

        preloadedSceneOp = null;
        preloadedSceneName = null;

        // Após ativar a cena, posiciona o player
        var player = PlayerController.Instance;
        if (player != null)
        {
            player.transform.position = teleportData.destinationPosition;
            Debug.Log($"[TeleportScreenManager] Player movido para {teleportData.destinationPosition} após ativação da cena pré-carregada");
        }
        else if (playerObject != null)
        {
            playerObject.transform.position = teleportData.destinationPosition;
            Debug.Log($"[TeleportScreenManager] Player passado movido para {teleportData.destinationPosition} após ativação da cena pré-carregada");
        }
        else
        {
            Debug.LogWarning("[TeleportScreenManager] Player não encontrado após ativação da cena pré-carregada.");
        }
    }
}
