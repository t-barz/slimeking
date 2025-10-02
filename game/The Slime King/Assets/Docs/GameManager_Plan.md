# 🎯 GameManager - Planejamento Completo - The Slime King

## 📋 Visão Geral

O GameManager é o **núcleo central** de **The Slime King**, responsável por coordenar todos os estados do jogo desde a inicialização até a experiência completa de gameplay. Este documento detalha o design completo, incluindo o fluxo de telas iniciais.

## 🔄 Estados do Jogo (Revisado com Telas Iniciais)

```csharp
public enum GameState
{
    // === ESTADOS INICIAIS ===
    Splash,         // Tela de splash (logos, loading inicial)
    MainMenu,       // Menu principal/tela inicial
    Options,        // Menu de configurações/opções
    Credits,        // Tela de créditos
    
    // === ESTADOS DE JOGO ===
    Loading,        // Carregando bioma/área de jogo
    Exploring,      // Explorando mundo (estado principal)
    Interacting,    // Dialogando com NPCs/criaturas
    
    // === ESTADOS DE INTERFACE ===
    Paused,         // Jogo pausado (menu pause)
    Inventory,      // Menu de inventário aberto
    SkillTree,      // Árvore de habilidades aberta
    
    // === ESTADOS ESPECIAIS ===
    Death,          // Slime foi derrotado
    Evolution,      // Processo de evolução do Slime
    Victory         // Área/objetivo completado
}
```

## 🎯 Fluxo de Estados Detalhado

### **Fluxo de Inicialização:**

```text
Splash → MainMenu → [Options] → Loading → Exploring
```

### **Fluxo de Gameplay:**

```text
Exploring ↔ Interacting
    ↓
Paused/Inventory/SkillTree → volta para Exploring
    ↓
Death → [Respawn] → Exploring
    ↓
Evolution → Exploring
    ↓
Victory → [Next Area] → Loading → Exploring
```

### **Fluxo de Retorno:**

```text
Qualquer Estado → MainMenu → [Options] → [Quit/Load]
```

## 📊 Sistema de Dados e Progressão

### **Dados Principais:**

- **Lives** (vidas do Slime - 3 iniciais, máximo 5)
- **SlimeStage** (estágio evolutivo atual)
- **CrystalFragments** (fragmentos de cristais coletados por elemento)
- **GameTime** (tempo total de jogo)
- **CurrentBiome** (bioma atual)
- **FriendshipLevels** (níveis de amizade com criaturas)
- **HomeExpansions** (expansões desbloqueadas do lar)
- **GameSettings** (configurações do jogador)

### **Sistema de Progressão:**

- Crescimento baseado em absorção de cristais
- Desbloqueio de habilidades por estágio evolutivo
- Expansão do lar através de amizade com criaturas
- Persistência de progresso entre sessões
- Sem sistema de pontuação tradicional

## 🎮 Eventos e Comunicação

### **Eventos Disparados pelo GameManager:**

```csharp
public static class GameManagerEvents
{
    // === ESTADOS DO JOGO ===
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameStarted;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerRespawn;
    public static event Action OnReturnToMainMenu;
    
    // === PROGRESSÃO DO SLIME ===
    public static event Action<SlimeStage> OnSlimeEvolved;
    public static event Action<int> OnLivesChanged;
    public static event Action<float> OnTimeChanged;
    public static event Action<string> OnBiomeChanged;
    
    // === SISTEMA DE CRISTAIS ===
    public static event Action<ElementType, int> OnCrystalFragmentsChanged;
    public static event Action<ElementType> OnNewElementUnlocked;
    
    // === AMIZADE E LAR ===
    public static event Action<string, int> OnFriendshipChanged;
    public static event Action<string> OnHomeExpansionUnlocked;
    
    // === CONFIGURAÇÕES ===
    public static event Action<GameSettings> OnSettingsChanged;
}
```

### **Eventos Escutados pelo GameManager:**

```csharp
// De UI/Menu Controllers
UIEvents.OnStartGameRequested += HandleStartGame;
UIEvents.OnOptionsRequested += HandleOptionsOpen;
UIEvents.OnMainMenuRequested += HandleMainMenuRequest;
UIEvents.OnQuitRequested += HandleQuitGame;

// De PlayerController
PlayerEvents.OnPlayerDeath += HandlePlayerDeath;
PlayerEvents.OnPlayerRespawn += HandlePlayerRespawn;

// De SlimeGrowthSystem
SlimeEvents.OnCrystalAbsorbed += HandleCrystalAbsorbed;
SlimeEvents.OnEvolutionTriggered += HandleSlimeEvolution;

// De CreatureController (Amizade)
CreatureEvents.OnFriendshipIncreased += HandleFriendshipChange;

// De Game Flow
GameFlowEvents.OnAreaCompleted += HandleAreaCompletion;
GameFlowEvents.OnBiomeTransition += HandleBiomeTransition;
```

## 🏗️ Arquitetura e Comunicação

```text
GameManager (Singleton - DontDestroyOnLoad)
    ├── Coordena → UIManager (Menus e HUD)
    ├── Coordena → AudioManager (Música e SFX)
    ├── Coordena → SaveManager (Persistência)
    ├── Coordena → SceneManager (Transições)
    ├── Coordena → SettingsManager (Configurações)
    ├── Escuta ← PlayerController (via eventos)
    ├── Escuta ← SlimeGrowthSystem (via eventos)
    └── Dispara → Todos os sistemas (via eventos)
```

## 🎯 Implementação Detalhada

### **Estrutura Principal:**

```csharp
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGameManager();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    #region Game State
    [Header("Game State")]
    [SerializeField] private GameState currentGameState = GameState.Splash;
    [SerializeField] private GameState previousGameState = GameState.Splash;
    [SerializeField] private Stack<GameState> stateHistory = new Stack<GameState>();
    
    public GameState CurrentGameState => currentGameState;
    public GameState PreviousGameState => previousGameState;
    #endregion
    
    #region Game Data
    [Header("Slime Progression")]
    [SerializeField] private SlimeStage currentSlimeStage = SlimeStage.Filhote;
    [SerializeField] private int currentLives = 3;
    [SerializeField] private float gameTime = 0f;
    [SerializeField] private float sessionTime = 0f;
    [SerializeField] private string currentBiome = "Ninho";
    
    // Dicionários para dados complexos
    private Dictionary<ElementType, int> crystalFragments = new Dictionary<ElementType, int>();
    private Dictionary<string, int> friendshipLevels = new Dictionary<string, int>();
    private List<string> unlockedHomeExpansions = new List<string>();
    
    // Configurações do jogo
    private GameSettings gameSettings = new GameSettings();
    
    // Propriedades públicas readonly
    public SlimeStage CurrentSlimeStage => currentSlimeStage;
    public int CurrentLives => currentLives;
    public float GameTime => gameTime;
    public float SessionTime => sessionTime;
    public string CurrentBiome => currentBiome;
    public GameSettings Settings => gameSettings;
    #endregion
    
    #region Configuration
    [Header("Configuration")]
    [SerializeField] private int maxLives = 5;
    [SerializeField] private int startingLives = 3;
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private float splashDuration = 3f;
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool enableDebugMode = false;
    [SerializeField] private bool skipSplash = false; // Para desenvolvimento
    #endregion
    
    #region Evolution Configuration
    [Header("Evolution Requirements")]
    [SerializeField] private int fragmentsForAdulto = 10;
    [SerializeField] private int fragmentsForGrandeSlime = 25;
    [SerializeField] private int fragmentsForReiSlime = 50;
    [SerializeField] private int aliadosRequiredForRei = 10;
    #endregion
}
```

### **Gerenciamento de Estados Avançado:**

```csharp
#region Game State Management
public void ChangeGameState(GameState newState, bool addToHistory = true)
{
    if (currentGameState == newState) return;
    
    // Validação de transições permitidas
    if (!IsValidStateTransition(currentGameState, newState))
    {
        if (enableDebugLogs)
            Debug.LogWarning($"[GameManager] Transição inválida: {currentGameState} → {newState}");
        return;
    }
    
    // Adiciona estado atual ao histórico
    if (addToHistory)
    {
        stateHistory.Push(currentGameState);
    }
    
    // Executa ações de saída do estado atual
    ExitState(currentGameState);
    
    previousGameState = currentGameState;
    currentGameState = newState;
    
    // Executa ações de entrada do novo estado
    EnterState(newState);
    
    // Dispara evento global
    OnGameStateChanged?.Invoke(newState);
    
    if (enableDebugLogs)
        Debug.Log($"[GameManager] Estado alterado: {previousGameState} → {newState}");
}

private bool IsValidStateTransition(GameState from, GameState to)
{
    // Define transições válidas baseadas no estado atual
    return from switch
    {
        GameState.Splash => to == GameState.MainMenu,
        GameState.MainMenu => to is GameState.Options or GameState.Credits or GameState.Loading,
        GameState.Options => to is GameState.MainMenu or GameState.Exploring,
        GameState.Loading => to == GameState.Exploring,
        GameState.Exploring => to is GameState.Paused or GameState.Inventory or GameState.SkillTree 
                                or GameState.Interacting or GameState.Death or GameState.Evolution 
                                or GameState.Victory or GameState.MainMenu,
        GameState.Paused => to is GameState.Exploring or GameState.Options or GameState.MainMenu,
        GameState.Inventory => to == GameState.Exploring,
        GameState.SkillTree => to == GameState.Exploring,
        GameState.Interacting => to == GameState.Exploring,
        GameState.Death => to is GameState.Exploring or GameState.MainMenu,
        GameState.Evolution => to == GameState.Exploring,
        GameState.Victory => to is GameState.Loading or GameState.MainMenu,
        GameState.Credits => to == GameState.MainMenu,
        _ => false
    };
}

private void ExitState(GameState state)
{
    switch (state)
    {
        case GameState.Exploring:
            // Pausa timers, salva posição, etc.
            Time.timeScale = 0f;
            break;
            
        case GameState.Paused:
        case GameState.Inventory:
        case GameState.SkillTree:
            // Restaura timeScale se voltando para exploring
            if (currentGameState == GameState.Exploring)
                Time.timeScale = 1f;
            break;
    }
}

private void EnterState(GameState state)
{
    switch (state)
    {
        case GameState.Splash:
            StartCoroutine(SplashSequence());
            break;
            
        case GameState.MainMenu:
            Time.timeScale = 1f;
            // Carrega cena do menu se necessário
            LoadMenuScene();
            break;
            
        case GameState.Options:
            // UI Manager cuida da interface
            break;
            
        case GameState.Loading:
            StartCoroutine(LoadGameSequence());
            break;
            
        case GameState.Exploring:
            Time.timeScale = 1f;
            StartGameTimer();
            break;
            
        case GameState.Paused:
        case GameState.Inventory:
        case GameState.SkillTree:
            Time.timeScale = 0f;
            break;
            
        case GameState.Death:
            Time.timeScale = 0f;
            HandleSlimeDeath();
            break;
            
        case GameState.Evolution:
            Time.timeScale = 0f;
            // Evolution sequence será iniciada por trigger externo
            break;
            
        case GameState.Victory:
            Time.timeScale = 0f;
            HandleAreaVictory();
            break;
    }
}

public void GoToPreviousState()
{
    if (stateHistory.Count > 0)
    {
        GameState previousState = stateHistory.Pop();
        ChangeGameState(previousState, false); // Não adiciona ao histórico
    }
}
#endregion
```

### **Sequências de Inicialização:**

```csharp
#region Initialization Sequences
private IEnumerator SplashSequence()
{
    if (enableDebugLogs)
        Debug.Log("[GameManager] Iniciando sequência de splash");
    
    // Carrega configurações salvas
    LoadGameSettings();
    
    // Aplica configurações de áudio/gráficos
    ApplySettings();
    
    if (skipSplash)
    {
        yield return null;
    }
    else
    {
        // Aguarda duração do splash
        yield return new WaitForSecondsRealtime(splashDuration);
    }
    
    // Transiciona para menu principal
    ChangeGameState(GameState.MainMenu);
}

private IEnumerator LoadGameSequence()
{
    if (enableDebugLogs)
        Debug.Log("[GameManager] Iniciando carregamento do jogo");
    
    // Reseta dados de sessão
    ResetSessionData();
    
    // Carrega dados salvos do progresso
    LoadGameProgress();
    
    // Simula loading (pode carregar assets, etc.)
    yield return new WaitForSecondsRealtime(1f);
    
    // Carrega cena de jogo
    if (SceneManager.Instance != null)
    {
        SceneManager.Instance.LoadScene("scn_lvl_ninho");
    }
    
    // Transiciona para exploração
    ChangeGameState(GameState.Exploring);
    
    // Dispara evento de jogo iniciado
    OnGameStarted?.Invoke();
}

private void LoadMenuScene()
{
    if (SceneManager.Instance != null)
    {
        SceneManager.Instance.LoadScene("scn_menu_main");
    }
}
#endregion
```

## 🎮 Sistema de Configurações

### **Estrutura de GameSettings:**

```csharp
[System.Serializable]
public class GameSettings
{
    [Header("Audio")]
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    
    [Header("Graphics")]
    public bool fullscreen = true;
    public int resolutionIndex = 0;
    public bool vSync = true;
    
    [Header("Gameplay")]
    public bool showTutorials = true;
    public bool showDamageNumbers = true;
    public float uiScale = 1f;
    
    [Header("Controls")]
    public float mouseSensitivity = 1f;
    public bool invertYAxis = false;
    
    [Header("Accessibility")]
    public bool subtitles = false;
    public bool colorBlindMode = false;
    public float textSize = 1f;
}
```

### **Gerenciamento de Configurações:**

```csharp
#region Settings Management
public void ApplySettings()
{
    // Aplica configurações de áudio
    if (AudioManager.Instance != null)
    {
        AudioManager.Instance.SetMasterVolume(gameSettings.masterVolume);
        AudioManager.Instance.SetMusicVolume(gameSettings.musicVolume);
        AudioManager.Instance.SetSFXVolume(gameSettings.sfxVolume);
    }
    
    // Aplica configurações gráficas
    Screen.fullScreen = gameSettings.fullscreen;
    QualitySettings.vSyncCount = gameSettings.vSync ? 1 : 0;
    
    // Dispara evento de configurações alteradas
    OnSettingsChanged?.Invoke(gameSettings);
    
    if (enableDebugLogs)
        Debug.Log("[GameManager] Configurações aplicadas");
}

public void UpdateSetting<T>(string settingName, T value)
{
    // Sistema reflexivo para atualizar configurações
    var field = typeof(GameSettings).GetField(settingName);
    if (field != null && field.FieldType == typeof(T))
    {
        field.SetValue(gameSettings, value);
        ApplySettings();
        SaveGameSettings();
    }
}

private void LoadGameSettings()
{
    if (SaveManager.Instance != null)
    {
        gameSettings = SaveManager.Instance.LoadSettings();
    }
}

private void SaveGameSettings()
{
    if (SaveManager.Instance != null)
    {
        SaveManager.Instance.SaveSettings(gameSettings);
    }
}
#endregion
```

## 📋 APIs Públicas Importantes

### **Controle de Estado:**

```csharp
// Transições de estado
public void StartNewGame();
public void ContinueGame();
public void OpenOptions();
public void OpenCredits();
public void PauseGame();
public void ResumeGame();
public void RestartGame();
public void ReturnToMainMenu();
public void QuitGame();

// Navegação
public void GoToPreviousState();
public bool CanTransitionTo(GameState newState);
```

### **Progressão do Slime:**

```csharp
// Cristais e evolução
public void AddCrystalFragments(ElementType element, int amount);
public int GetCrystalFragments(ElementType element);
public int GetTotalCrystalFragments();
public bool CanEvolve();
public SlimeStage GetNextEvolutionStage();

// Vidas
public void AddLife();
public void LoseLife();
public bool HasLives();
```

### **Sistema Social:**

```csharp
// Amizade e lar
public void IncreaseFriendship(string creatureName, int amount = 1);
public int GetFriendshipLevel(string creatureName);
public int GetAlliedCreatures();
public bool IsHomeExpansionUnlocked(string expansionName);
```

## 🚀 Benefícios do Design Revisado

1. **Fluxo Completo**: Desde splash até gameplay
2. **Flexibilidade**: Sistema de estados robusto com validação
3. **Persistência**: Configurações e progresso salvos
4. **Escalabilidade**: Fácil adição de novos estados
5. **Manutenibilidade**: Código organizado e bem documentado
6. **Performance**: Otimizado para experiência cozy
7. **Acessibilidade**: Sistema de configurações abrangente

## 🎯 Próximos Passos de Implementação

1. **Fase 1**: Implementar estrutura básica e estados iniciais
2. **Fase 2**: Adicionar sistema de progressão e cristais
3. **Fase 3**: Implementar sistema social e lar
4. **Fase 4**: Adicionar persistência e configurações
5. **Fase 5**: Polimento e otimizações

Este GameManager servirá como a **base sólida** para toda a experiência de **The Slime King**, proporcionando uma jornada fluida desde a primeira inicialização até se tornar o Rei Slime! 🎮✨
