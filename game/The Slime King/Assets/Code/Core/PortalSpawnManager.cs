using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Gerencia o spawn do jogador quando entra em uma cena através de um portal
/// </summary>
public class PortalSpawnManager : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    // Singleton
    public static PortalSpawnManager Instance { get; private set; }

    // Estado interno
    private bool hasProcessedPortalSpawn = false;

    private void Awake()
    {
        // Implementar singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Processar spawn por portal se necessário
        StartCoroutine(ProcessPortalSpawnDelayed());
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Chamado quando uma nova cena é carregada
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasProcessedPortalSpawn = false;
        StartCoroutine(ProcessPortalSpawnDelayed());
    }

    /// <summary>
    /// Processa o spawn por portal com um pequeno delay
    /// </summary>
    private IEnumerator ProcessPortalSpawnDelayed()
    {
        // Aguardar um frame para garantir que tudo esteja inicializado
        yield return new WaitForEndOfFrame();

        // Aguardar mais um pouco para garantir que o jogador esteja inicializado
        yield return new WaitForSeconds(0.1f);

        ProcessPortalSpawn();
    }

    /// <summary>
    /// Processa o spawn do jogador se ele veio através de um portal
    /// </summary>
    private void ProcessPortalSpawn()
    {
        if (hasProcessedPortalSpawn)
        {
            return;
        }

        // Verificar se há dados de transição por portal
        if (PlayerPrefs.GetInt("PortalTransitionActive", 0) == 1)
        {
            hasProcessedPortalSpawn = true;

            // Obter dados do portal
            Vector3 spawnPosition = new Vector3(
                PlayerPrefs.GetFloat("PortalSpawnX", 0f),
                PlayerPrefs.GetFloat("PortalSpawnY", 0f),
                PlayerPrefs.GetFloat("PortalSpawnZ", 0f)
            );

            string sourcePortalId = PlayerPrefs.GetString("PortalSourceId", "Unknown");

            if (enableDebugLogs)
            {
                Debug.Log($"[PortalSpawnManager] Processando spawn por portal - Posição: {spawnPosition} | Portal origem: {sourcePortalId}");
            }

            // Posicionar jogador
            bool success = PositionPlayer(spawnPosition);

            if (success)
            {
                // Limpar dados temporários do portal
                ClearPortalTransitionData();

                // Atualizar respawn point se SavePlayerManager estiver disponível
                UpdateRespawnPoint(spawnPosition);

                if (enableDebugLogs)
                {
                    Debug.Log($"[PortalSpawnManager] Jogador posicionado com sucesso via portal na posição {spawnPosition}");
                }
            }
            else
            {
                Debug.LogWarning($"[PortalSpawnManager] Falha ao posicionar jogador na posição {spawnPosition}");

                // Não limpar dados do portal em caso de falha, para permitir retry
            }
        }
        else if (enableDebugLogs)
        {
            Debug.Log("[PortalSpawnManager] Nenhuma transição por portal detectada para esta cena");
        }
    }

    /// <summary>
    /// Posiciona o jogador na posição especificada
    /// </summary>
    private bool PositionPlayer(Vector3 position)
    {
        // Procurar pelo jogador na cena
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            // Tentar encontrar por nome se tag não funcionar
            player = GameObject.Find("Player");
        }

        if (player != null)
        {
            // Posicionar jogador
            player.transform.position = position;

            // Tentar parar qualquer movimento residual
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[PortalSpawnManager] Jogador posicionado em {position}");
            }

            return true;
        }
        else
        {
            Debug.LogError("[PortalSpawnManager] Jogador não encontrado na cena atual!");
            return false;
        }
    }

    /// <summary>
    /// Limpa os dados temporários de transição por portal
    /// </summary>
    private void ClearPortalTransitionData()
    {
        PlayerPrefs.DeleteKey("PortalSpawnX");
        PlayerPrefs.DeleteKey("PortalSpawnY");
        PlayerPrefs.DeleteKey("PortalSpawnZ");
        PlayerPrefs.DeleteKey("PortalSourceId");
        PlayerPrefs.DeleteKey("PortalTransitionActive");
        PlayerPrefs.Save();

        if (enableDebugLogs)
        {
            Debug.Log("[PortalSpawnManager] Dados temporários de portal limpos");
        }
    }

    /// <summary>
    /// Atualiza o ponto de respawn se SavePlayerManager estiver disponível
    /// </summary>
    private void UpdateRespawnPoint(Vector3 position)
    {
        if (SavePlayerManager.Instance != null)
        {
            SavePlayerManager.Instance.SetRespawnPoint(position);

            if (enableDebugLogs)
            {
                Debug.Log($"[PortalSpawnManager] Ponto de respawn atualizado para {position}");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.Log("[PortalSpawnManager] SavePlayerManager não disponível - respawn não atualizado");
        }
    }

    /// <summary>
    /// Método público para forçar processamento de spawn
    /// </summary>
    public void ForceProcessPortalSpawn()
    {
        hasProcessedPortalSpawn = false;
        ProcessPortalSpawn();
    }

    /// <summary>
    /// Verifica se há uma transição por portal pendente
    /// </summary>
    public bool HasPendingPortalTransition()
    {
        return PlayerPrefs.GetInt("PortalTransitionActive", 0) == 1;
    }

    /// <summary>
    /// Obtém a posição de spawn do portal se houver uma transição pendente
    /// </summary>
    public Vector3? GetPendingPortalSpawnPosition()
    {
        if (HasPendingPortalTransition())
        {
            return new Vector3(
                PlayerPrefs.GetFloat("PortalSpawnX", 0f),
                PlayerPrefs.GetFloat("PortalSpawnY", 0f),
                PlayerPrefs.GetFloat("PortalSpawnZ", 0f)
            );
        }

        return null;
    }

    /// <summary>
    /// Método público para limpar dados de portal (útil para debug)
    /// </summary>
    public void ClearPortalData()
    {
        ClearPortalTransitionData();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Método para debug no editor
    /// </summary>
    [UnityEditor.MenuItem("Extras/Portal Debug/Show Portal Transition Data")]
    public static void ShowPortalTransitionData()
    {
        bool hasTransition = PlayerPrefs.GetInt("PortalTransitionActive", 0) == 1;

        if (hasTransition)
        {
            Vector3 spawnPos = new Vector3(
                PlayerPrefs.GetFloat("PortalSpawnX", 0f),
                PlayerPrefs.GetFloat("PortalSpawnY", 0f),
                PlayerPrefs.GetFloat("PortalSpawnZ", 0f)
            );
            string sourceId = PlayerPrefs.GetString("PortalSourceId", "Unknown");

            UnityEditor.EditorUtility.DisplayDialog(
                "Portal Transition Data",
                $"Transição ativa: SIM\n" +
                $"Posição de spawn: {spawnPos}\n" +
                $"Portal origem: {sourceId}",
                "OK"
            );
        }
        else
        {
            UnityEditor.EditorUtility.DisplayDialog(
                "Portal Transition Data",
                "Nenhuma transição por portal ativa",
                "OK"
            );
        }
    }

    [UnityEditor.MenuItem("Extras/Portal Debug/Clear Portal Transition Data")]
    public static void ClearPortalTransitionDataFromMenu()
    {
        if (UnityEditor.EditorUtility.DisplayDialog(
            "Clear Portal Data",
            "Deseja limpar os dados de transição por portal?",
            "Sim", "Não"))
        {
            PlayerPrefs.DeleteKey("PortalSpawnX");
            PlayerPrefs.DeleteKey("PortalSpawnY");
            PlayerPrefs.DeleteKey("PortalSpawnZ");
            PlayerPrefs.DeleteKey("PortalSourceId");
            PlayerPrefs.DeleteKey("PortalTransitionActive");
            PlayerPrefs.Save();

            Debug.Log("[PortalSpawnManager] Dados de transição por portal limpos via menu");
        }
    }
#endif
}
