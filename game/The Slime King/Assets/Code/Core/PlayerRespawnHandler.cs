using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton responsável por gerenciar o respawn do jogador e observar mudanças de cena.
/// Detecta quando uma nova cena é carregada e gerencia a posição de destino do Player.
/// </summary>
public class PlayerRespawnHandler : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Instância única do PlayerRespawnHandler
    /// </summary>
    public static PlayerRespawnHandler Instance { get; private set; }
    #endregion

    #region Configurações
    [Header("Configurações")]
    [Tooltip("Ativa logs de debug")]
    [SerializeField] private bool enableDebugLogs = true;
    #endregion

    #region Posição de Destino
    /// <summary>
    /// Posição de destino onde o jogador deve aparecer
    /// </summary>
    [Header("Posição de Destino do Player")]
    [SerializeField] private Vector3 playerDestinationPosition = Vector3.zero;

    /// <summary>
    /// Indica se existe uma posição de destino válida para aplicar
    /// </summary>
    private bool hasDestinationPosition = false;
    #endregion

    #region Propriedades Públicas
    /// <summary>
    /// Obtém ou define a posição de destino do jogador
    /// </summary>
    public Vector3 PlayerDestinationPosition
    {
        get => playerDestinationPosition;
        set
        {
            playerDestinationPosition = value;
            hasDestinationPosition = true;

            if (enableDebugLogs)
            {
                Debug.Log($"[PlayerRespawnHandler] Nova posição de destino definida: {value}");
            }
        }
    }

    /// <summary>
    /// Verifica se há uma posição de destino válida
    /// </summary>
    public bool HasDestinationPosition => hasDestinationPosition;
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Inicialização do Singleton
    /// </summary>
    private void Awake()
    {
        // Implementa Singleton
        if (Instance != null && Instance != this)
        {
            if (enableDebugLogs)
            {
                Debug.Log("[PlayerRespawnHandler] Instância duplicada detectada - destruindo...");
            }
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Registra eventos de mudança de cena
        SceneManager.activeSceneChanged += OnActiveSceneChanged;

        // Se não há posição destino definida, define Vector3.Zero e move o Player
        if (!hasDestinationPosition)
        {
            SetPlayerDestination(Vector3.zero);
            StartCoroutine(MovePlayerToDefaultPositionAfterDelay());
        }

        if (enableDebugLogs)
        {
            Debug.Log("[PlayerRespawnHandler] Singleton inicializado e observando mudanças de cena");
        }
    }

    /// <summary>
    /// Limpeza quando o objeto é destruído
    /// </summary>
    private void OnDestroy()
    {
        // Remove listeners para evitar vazamentos de memória
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;

        if (enableDebugLogs)
        {
            Debug.Log("[PlayerRespawnHandler] Listeners de cena removidos");
        }
    }
    #endregion

    #region Event Handlers
    /// <summary>
    /// Callback executado quando a cena ativa muda
    /// </summary>
    /// <param name="previousScene">Cena anterior</param>
    /// <param name="newScene">Nova cena ativa</param>
    private void OnActiveSceneChanged(Scene previousScene, Scene newScene)
    {
        if (enableDebugLogs)
        {
            string prevName = previousScene.IsValid() ? previousScene.name : "None";
            Debug.Log($"[PlayerRespawnHandler] Cena ativa mudou de '{prevName}' para '{newScene.name}'");
        }

        // Processa o respawn se houver posição de destino
        if (hasDestinationPosition)
        {
            StartCoroutine(ProcessPlayerRespawnAfterDelay());
        }
    }
    #endregion

    #region Player Management
    /// <summary>
    /// Move o jogador para a posição padrão (Vector3.Zero) após um pequeno delay
    /// </summary>
    private System.Collections.IEnumerator MovePlayerToDefaultPositionAfterDelay()
    {
        // Aguarda para garantir que o jogador foi instanciado
        yield return new WaitForSeconds(0.1f);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Para qualquer movimento residual
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
            }

            // Move para Vector3.Zero
            player.transform.position = Vector3.zero;

            if (enableDebugLogs)
            {
                Debug.Log("[PlayerRespawnHandler] Jogador movido para posição padrão (0,0,0) no Awake");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("[PlayerRespawnHandler] Jogador com tag 'Player' não encontrado no Awake para mover para posição padrão");
        }
    }

    /// <summary>
    /// Processa o respawn do jogador após um pequeno delay
    /// </summary>
    private System.Collections.IEnumerator ProcessPlayerRespawnAfterDelay()
    {
        // Aguarda para garantir que o jogador foi instanciado
        yield return new WaitForSeconds(0.2f);

        ApplyDestinationPosition();
    }

    /// <summary>
    /// Aplica a posição de destino ao jogador
    /// </summary>
    private void ApplyDestinationPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Para qualquer movimento residual
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
            }

            // Aplica a nova posição
            player.transform.position = playerDestinationPosition;

            if (enableDebugLogs)
            {
                Debug.Log($"[PlayerRespawnHandler] Jogador posicionado em: {playerDestinationPosition}");
            }

            // Limpa a posição de destino após aplicar
            ClearDestinationPosition();
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("[PlayerRespawnHandler] Jogador com tag 'Player' não encontrado para aplicar posição de destino");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Define a posição de destino do jogador
    /// </summary>
    /// <param name="position">Posição onde o jogador deve aparecer</param>
    public void SetPlayerDestination(Vector3 position)
    {
        PlayerDestinationPosition = position;

        if (enableDebugLogs)
        {
            Debug.Log($"[PlayerRespawnHandler] Destino definido: {position}");
        }
    }

    /// <summary>
    /// Limpa a posição de destino
    /// </summary>
    public void ClearDestinationPosition()
    {
        hasDestinationPosition = false;

        if (enableDebugLogs)
        {
            Debug.Log("[PlayerRespawnHandler] Posição de destino limpa");
        }
    }

    /// <summary>
    /// Força a aplicação da posição de destino imediatamente
    /// </summary>
    public void ForceApplyDestination()
    {
        if (hasDestinationPosition)
        {
            ApplyDestinationPosition();
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("[PlayerRespawnHandler] Nenhuma posição de destino definida para aplicar");
        }
    }

    /// <summary>
    /// Obtém o nome da cena ativa atual
    /// </summary>
    /// <returns>Nome da cena ativa</returns>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    #endregion

    #region Debug Methods
    /// <summary>
    /// Método para debug - mostra informações no console
    /// </summary>
    [ContextMenu("Show Debug Info")]
    public void ShowDebugInfo()
    {
        string info = $"PlayerRespawnHandler Debug Info:\n" +
                     $"- Cena Atual: {GetCurrentSceneName()}\n" +
                     $"- Tem Destino: {hasDestinationPosition}\n" +
                     $"- Posição Destino: {playerDestinationPosition}";

        Debug.Log($"[PlayerRespawnHandler] {info}");
    }
    #endregion
}
