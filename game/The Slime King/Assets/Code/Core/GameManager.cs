using UnityEngine;

/// <summary>
/// Gerenciador principal do jogo - Singleton que persiste entre cenas
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Instância única do GameManager
    /// </summary>
    public static GameManager Instance { get; private set; }
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Inicialização do Singleton
    /// </summary>
    private void Awake()
    {
        // Implementa padrão Singleton
        if (Instance != null && Instance != this)
        {
            Debug.Log("[GameManager] Instância duplicada detectada - destruindo...");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log("[GameManager] Singleton inicializado");
    }

    /// <summary>
    /// Inicialização após Awake
    /// </summary>
    void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// Atualização por frame
    /// </summary>
    void Update()
    {

    }

    /// <summary>
    /// Limpeza ao destruir
    /// </summary>
    private void OnDestroy()
    {
        // Remove referência se esta instância está sendo destruída
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Inicializa o jogo
    /// </summary>
    private void InitializeGame()
    {
        Debug.Log("[GameManager] Jogo inicializado");

        // Aqui você pode adicionar lógica de inicialização do jogo:
        // - Configurações iniciais
        // - Carregamento de dados salvos
        // - Inicialização de sistemas
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Pausa o jogo
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0f;
        Debug.Log("[GameManager] Jogo pausado");
    }

    /// <summary>
    /// Retoma o jogo
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameManager] Jogo retomado");
    }

    /// <summary>
    /// Verifica se o jogo está pausado
    /// </summary>
    public bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }

    public void ChangeScene(string sceneName, Vector2 targetPosition)
    {
        // Aqui você pode adicionar lógica para carregar uma nova cena
        Debug.Log($"[GameManager] Mudando para a cena: {sceneName} na posição {targetPosition}");
        // Escurece a cena atual
        // Exibe UI de carregamento
        // Salva o jogo
        // Destroy o GameObject do Player
        // Carrega a nova cena em asynchronous mode
    }
    #endregion
}
