using SlimeKing.Core;
using SlimeKing.Data;
using UnityEngine;
using System.Collections; // Necessário para IEnumerator em corrotinas
using System.Collections.Generic; // Para Dictionary de cristais
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // para limpeza de múltiplos EventSystems
using UnityEngine.Rendering.Universal; // para Light2D (URP 2D)

public class GameManager : ManagerSingleton<GameManager>
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    // Referência à operação de carregamento assíncrono da cena pré-carregada
    private AsyncOperation preloadedSceneOperation;
    private string preloadedSceneName;
    private Coroutine pendingActivationCoroutine;
    public event System.Action<string> OnPreloadedSceneActivated; // evento disparado após ativação

    // Propriedades de consulta simples (KISS)
    public string PreloadedSceneName => preloadedSceneName;
    public bool IsPreloadReady => preloadedSceneOperation != null && preloadedSceneOperation.progress >= 0.9f;
    public bool HasPreloadedScene(string sceneName) => preloadedSceneOperation != null && preloadedSceneName == sceneName;

    #region Reputation System
    // Sistema de reputação para quests
    private int reputation = 0;
    public event System.Action<int> OnReputationChanged; // evento disparado quando reputação muda
    #endregion

    #region Player Stealth State System
    // Estado de stealth do jogador para consulta por sistemas de IA
    private bool isPlayerInStealth = false;
    private bool isPlayerCrouching = false;
    private bool hasPlayerCover = false;

    public event System.Action<bool> OnPlayerStealthStateChanged; // evento disparado quando estado stealth muda
    #endregion

    #region Crystal System
    // Sistema de contadores de cristais elementais
    private Dictionary<CrystalType, int> crystalCounts = new Dictionary<CrystalType, int>();
    public event System.Action<CrystalType, int> OnCrystalCountChanged; // evento disparado quando contador de cristal muda
    public event System.Action<CrystalType, int> OnCrystalCollected; // evento disparado quando cristal é coletado
    #endregion

    // Inicialização mínima seguindo KISS: define estado inicial e aplica configurações básicas de runtime.
    protected override void Initialize()
    {
        // Configurações básicas do runtime (podem ser ajustadas depois por outros managers)
        Application.targetFrameRate = 60; // manter consistente
        Time.timeScale = 1f;              // garantir tempo normal

        // Garante que CameraManager seja inicializado
        if (CameraManager.HasInstance)
        {
            CameraManager.Instance.OnSceneLoaded();
        }

        // Subscreve aos eventos de stealth
        StealthEvents.OnPlayerEnteredStealth += HandlePlayerEnteredStealth;
        StealthEvents.OnPlayerExitedStealth += HandlePlayerExitedStealth;
        StealthEvents.OnPlayerCoverStateChanged += HandlePlayerCoverStateChanged;

        // Inicializa contadores de cristais
        InitializeCrystalSystem();

        // Estado inicial simples (usar GameState se definido em enums do projeto)
        // Como este GameManager está reduzido, apenas registra o bootstrap.
        Log("GameManager bootstrap concluído");
    }

    #region Logging
    /// <summary>
    /// Log condicional baseado na flag enableDebugLogs
    /// </summary>
    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[GameManager] {message}");
        }
    }

    /// <summary>
    /// Warning sempre ativo independente da flag
    /// </summary>
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[GameManager] {message}");
    }

    /// <summary>
    /// Error sempre ativo independente da flag
    /// </summary>
    private void LogError(string message)
    {
        Debug.LogError($"[GameManager] {message}");
    }
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Scene Preload API

    /// <summary>
    /// OBSOLETO: Pré-carregamento não é mais suportado com LoadSceneMode.Single.
    /// Este método foi mantido para compatibilidade mas não faz pré-carregamento real.
    /// A cena será carregada normalmente quando ActivatePreloadedScene() for chamado.
    /// </summary>
    /// <param name="sceneName">Nome da cena a pré-carregar.</param>
    public void PreloadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Log("PreloadScene chamado com nome inválido.");
            return;
        }

        // Apenas armazena o nome da cena para carregar depois
        if (preloadedSceneName == sceneName)
        {
            Log($"Cena '{sceneName}' já está marcada para carregamento.");
            return;
        }

        Log($"Cena '{sceneName}' marcada para carregamento (pré-carregamento não suportado com LoadSceneMode.Single).");
        preloadedSceneName = sceneName;
        // Não faz carregamento real aqui - será feito em ActivatePreloadedScene()
    }

    /// <summary>
    /// Carrega e ativa a cena marcada para carregamento usando LoadSceneMode.Single.
    /// Isso substitui todas as cenas anteriores automaticamente.
    /// </summary>
    public void ActivatePreloadedScene(System.Action onActivated = null)
    {
        if (string.IsNullOrEmpty(preloadedSceneName))
        {
            Log("Nenhuma cena marcada para carregamento.");
            return;
        }

        // Se já preparando para ativar, não duplicar
        if (pendingActivationCoroutine != null)
        {
            Log("Ativação já pendente; ignorando chamada duplicada.");
            return;
        }

        Log($"Carregando cena '{preloadedSceneName}' usando LoadSceneMode.Single...");
        pendingActivationCoroutine = StartCoroutine(LoadAndActivateScene(onActivated));
    }

    /// <summary>
    /// Coroutine que carrega a cena usando LoadSceneMode.Single.
    /// Isso automaticamente descarrega todas as cenas anteriores.
    /// </summary>
    private IEnumerator LoadAndActivateScene(System.Action onActivated)
    {
        string sceneToLoad = preloadedSceneName;

        // Carrega a cena usando Single mode (descarrega tudo automaticamente)
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);

        if (loadOperation == null)
        {
            Log($"Falha ao iniciar carregamento da cena '{sceneToLoad}'.");
            yield break;
        }

        // Aguarda carregamento completo
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        Log($"Cena '{sceneToLoad}' carregada com sucesso usando LoadSceneMode.Single.");

        // Atualiza configurações de câmera após carregamento da cena
        if (CameraManager.HasInstance)
        {
            CameraManager.Instance.OnSceneLoaded();
        }

        // Dispara eventos
        OnPreloadedSceneActivated?.Invoke(sceneToLoad);
        onActivated?.Invoke();

        // Limpa referências internas
        preloadedSceneOperation = null;
        preloadedSceneName = null;
        pendingActivationCoroutine = null;
    }

    #endregion

    #region Cleanup Helpers (OBSOLETO com LoadSceneMode.Single)
    // NOTA: Estes métodos não são mais necessários com LoadSceneMode.Single
    // pois o Unity automaticamente descarrega todas as cenas anteriores.
    // Mantidos apenas para compatibilidade caso algum código antigo os chame.

    /// <summary>
    /// OBSOLETO: Não é mais necessário com LoadSceneMode.Single.
    /// </summary>
    private void CleanupDuplicateEventSystems()
    {
        // Não faz nada - LoadSceneMode.Single já garante que não há duplicados
    }

    /// <summary>
    /// OBSOLETO: Não é mais necessário com LoadSceneMode.Single.
    /// </summary>
    private void CleanupDuplicateGlobalLights()
    {
        // Não faz nada - LoadSceneMode.Single já garante que não há duplicados
    }
    #endregion

    #region Player Stealth State Methods

    /// <summary>
    /// Verifica se o jogador está atualmente em estado de stealth.
    /// Usado por sistemas de IA para determinar detectabilidade.
    /// </summary>
    /// <returns>True se o jogador está em stealth (semi-transparente e indetectável)</returns>
    public bool IsPlayerInStealth()
    {
        return isPlayerInStealth;
    }

    /// <summary>
    /// Verifica se o jogador está agachado.
    /// </summary>
    /// <returns>True se o jogador está agachado</returns>
    public bool IsPlayerCrouching()
    {
        return isPlayerCrouching;
    }

    /// <summary>
    /// Verifica se o jogador tem cobertura válida.
    /// </summary>
    /// <returns>True se o jogador está atrás de um objeto com collider</returns>
    public bool HasPlayerCover()
    {
        return hasPlayerCover;
    }

    /// <summary>
    /// Atualiza o estado de agachamento do jogador.
    /// Chamado pelo PlayerController quando o estado muda.
    /// </summary>
    /// <param name="isCrouching">Se o jogador está agachado</param>
    public void SetPlayerCrouchingState(bool isCrouching)
    {
        if (isPlayerCrouching != isCrouching)
        {
            isPlayerCrouching = isCrouching;
            Log($"Player crouch state: {isCrouching}");
        }
    }

    private void HandlePlayerEnteredStealth()
    {
        if (!isPlayerInStealth)
        {
            isPlayerInStealth = true;
            OnPlayerStealthStateChanged?.Invoke(true);
            Log("Player entered stealth mode");
        }
    }

    private void HandlePlayerExitedStealth()
    {
        if (isPlayerInStealth)
        {
            isPlayerInStealth = false;
            OnPlayerStealthStateChanged?.Invoke(false);
            Log("Player exited stealth mode");
        }
    }

    private void HandlePlayerCoverStateChanged(bool hasCover)
    {
        hasPlayerCover = hasCover;
        Log($"Player cover state: {hasCover}");
    }

    #endregion

    #region Reputation System Methods
    /// <summary>
    /// Adiciona reputação ao jogador.
    /// Usado principalmente pelo Quest System ao completar quests.
    /// </summary>
    /// <param name="amount">Quantidade de reputação a adicionar (pode ser negativa)</param>
    public void AddReputation(int amount)
    {
        reputation += amount;

        // Garante que reputação não fique negativa
        if (reputation < 0)
        {
            reputation = 0;
        }

        // Dispara evento de mudança de reputação
        OnReputationChanged?.Invoke(reputation);

        Log($"Reputação alterada: {amount:+#;-#;0} (Total: {reputation})");
    }

    /// <summary>
    /// Obtém a reputação atual do jogador.
    /// </summary>
    /// <returns>Valor atual de reputação</returns>
    public int GetReputation()
    {
        return reputation;
    }
    #endregion

    #region Crystal System Methods
    /// <summary>
    /// Inicializa o sistema de contadores de cristais.
    /// </summary>
    private void InitializeCrystalSystem()
    {
        // Inicializa todos os tipos de cristais com contador zerado
        foreach (CrystalType crystalType in System.Enum.GetValues(typeof(CrystalType)))
        {
            crystalCounts[crystalType] = 0;
        }

        Log("Sistema de cristais inicializado com todos os contadores zerados");
    }

    /// <summary>
    /// Adiciona cristais do tipo especificado.
    /// </summary>
    /// <param name="crystalType">Tipo do cristal a ser adicionado</param>
    /// <param name="amount">Quantidade a adicionar (padrão: 1)</param>
    public void AddCrystal(CrystalType crystalType, int amount = 1)
    {
        if (amount <= 0)
        {
            LogWarning($"Tentativa de adicionar quantidade inválida de cristais: {amount}");
            return;
        }

        // Adiciona ao contador
        if (!crystalCounts.ContainsKey(crystalType))
        {
            crystalCounts[crystalType] = 0;
        }

        crystalCounts[crystalType] += amount;
        int newCount = crystalCounts[crystalType];

        // Dispara eventos
        OnCrystalCollected?.Invoke(crystalType, amount);
        OnCrystalCountChanged?.Invoke(crystalType, newCount);

        Log($"Cristal {crystalType} adicionado: +{amount} (Total: {newCount})");
    }

    /// <summary>
    /// Obtém a quantidade atual de cristais do tipo especificado.
    /// </summary>
    /// <param name="crystalType">Tipo do cristal</param>
    /// <returns>Quantidade atual</returns>
    public int GetCrystalCount(CrystalType crystalType)
    {
        return crystalCounts.ContainsKey(crystalType) ? crystalCounts[crystalType] : 0;
    }

    /// <summary>
    /// Obtém todos os contadores de cristais.
    /// </summary>
    /// <returns>Dicionário com todos os contadores</returns>
    public Dictionary<CrystalType, int> GetAllCrystalCounts()
    {
        return new Dictionary<CrystalType, int>(crystalCounts);
    }

    /// <summary>
    /// Remove cristais do tipo especificado (para uso futuro em crafting/consumo).
    /// </summary>
    /// <param name="crystalType">Tipo do cristal</param>
    /// <param name="amount">Quantidade a remover</param>
    /// <returns>True se foi possível remover, false se não há cristais suficientes</returns>
    public bool RemoveCrystal(CrystalType crystalType, int amount = 1)
    {
        if (amount <= 0)
        {
            LogWarning($"Tentativa de remover quantidade inválida de cristais: {amount}");
            return false;
        }

        int currentCount = GetCrystalCount(crystalType);
        if (currentCount < amount)
        {
            LogWarning($"Cristais insuficientes: {crystalType} (Atual: {currentCount}, Necessário: {amount})");
            return false;
        }

        crystalCounts[crystalType] -= amount;
        int newCount = crystalCounts[crystalType];

        // Dispara evento de mudança
        OnCrystalCountChanged?.Invoke(crystalType, newCount);

        Log($"Cristal {crystalType} removido: -{amount} (Total: {newCount})");
        return true;
    }

    /// <summary>
    /// Obtém o total de cristais de todos os tipos.
    /// </summary>
    /// <returns>Soma de todos os cristais</returns>
    public int GetTotalCrystalCount()
    {
        int total = 0;
        foreach (var count in crystalCounts.Values)
        {
            total += count;
        }
        return total;
    }
    #endregion

    protected override void OnDestroy()
    {
        // Desinscreve dos eventos de stealth
        StealthEvents.OnPlayerEnteredStealth -= HandlePlayerEnteredStealth;
        StealthEvents.OnPlayerExitedStealth -= HandlePlayerExitedStealth;
        StealthEvents.OnPlayerCoverStateChanged -= HandlePlayerCoverStateChanged;

        base.OnDestroy();
    }
}
