using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExtraTools
{
    /// <summary>
    /// GameManager - Núcleo central do The Slime King.
    /// Responsável por:
    /// 1. Gerenciar o ciclo de vida do jogo (TitleScreen -> Menus -> Carregamento -> Gameplay -> Estados especiais).
    /// 2. Orquestrar transições de <see cref="GameState"/> com validação e histórico de navegação.
    /// 3. Controlar progressão do Slime (estágio evolutivo, fragmentos de cristal, vidas).
    /// 4. Gerenciar sistemas sociais (amizades e expansões do lar) e seus desbloqueios.
    /// 5. Centralizar aplicação de configurações (gráficas, áudio - futuramente) via <see cref="GameSettings"/>.
    /// 6. Cronometrar tempo total e tempo de sessão para métricas e gameplay.
    /// 7. Emitir eventos globais (via GameManagerEvents.Raise...) para desacoplamento com UI e outros sistemas.
    /// 8. Expor uma API pública limpa para outros scripts interagirem sem conhecer detalhes internos.
    ///
    /// Padrões adotados:
    /// - Singleton (lifetime persistente entre cenas via DontDestroyOnLoad).
    /// - Event-driven architecture para reduzir dependências diretas.
    /// - Separação de responsabilidades por regiões (State, Progressão, Amizade, Configurações, Fluxo, etc.).
    /// - Métodos privados coesos e pequenos; somente o que precisa é exposto publicamente.
    ///
    /// Boas práticas:
    /// - Nunca altere estados diretamente: use <see cref="ChangeGameState"/>.
    /// - Nunca invoque eventos diretamente: use métodos Raise das classes de eventos.
    /// - Evite colocar lógica externa aqui: este manager coordena, não implementa features de domínio ricas (isso pertence a outros sistemas específicos).
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        /// <summary>
        /// Instância única ativa do GameManager. Criada no primeiro Awake encontrado.
        /// </summary>
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
        /// <summary>
        /// Estado atual do jogo. Nunca setar diretamente fora deste script; usar <see cref="ChangeGameState"/>.
        /// </summary>
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        /// <summary>
        /// Último estado antes da transição mais recente. Útil para UIs contextuais e retorno.
        /// </summary>
        [SerializeField] private GameState previousGameState = GameState.MainMenu;
        /// <summary>
        /// Histórico de estados visitados (stack LIFO) para permitir retorno contextual (<see cref="GoToPreviousState"/>).
        /// </summary>
        [SerializeField] private Stack<GameState> stateHistory = new Stack<GameState>();

        public GameState CurrentGameState => currentGameState;
        public GameState PreviousGameState => previousGameState;
        #endregion

        #region Scene Configuration
        [Header("Scene Configuration")]
        [Tooltip("Nome da cena de menu/título (ex: TitleScreen ou MainMenu).")]
        [SerializeField] private string mainMenuSceneName = "TitleScreen";
        [Tooltip("Usar carregamento assíncrono para cenas (futuro). Atualmente usa LoadScene")]
        [SerializeField] private bool useAsyncSceneLoading = false;
        #endregion

        #region Game Data
        [Header("Slime Progression")]
        /// <summary>Estágio evolutivo atual do Slime.</summary>
        [SerializeField] private SlimeStage currentSlimeStage = SlimeStage.Filhote;
        /// <summary>Quantidade atual de vidas do jogador (limite em <see cref="maxLives"/>).</summary>
        [SerializeField] private int currentLives = 3;
        /// <summary>Tempo total acumulado (todas as sessões) desde o começo desta execução.</summary>
        [SerializeField] private float gameTime = 0f;
        /// <summary>Tempo da sessão atual (reinicia ao iniciar jogo ou carregar progresso).</summary>
        [SerializeField] private float sessionTime = 0f;
        /// <summary>Nome do bioma atual em exploração.</summary>
        [SerializeField] private string currentBiome = "Ninho";

        // Dicionários para dados complexos
        /// <summary>Quantidade de fragmentos por elemento coletados.</summary>
        private Dictionary<ElementType, int> crystalFragments = new Dictionary<ElementType, int>();
        /// <summary>Nível de amizade por criatura (string = identificador/nome).</summary>
        private Dictionary<string, int> friendshipLevels = new Dictionary<string, int>();
        /// <summary>Lista de expansões do lar já desbloqueadas.</summary>
        private List<string> unlockedHomeExpansions = new List<string>();

        // Configurações do jogo
        /// <summary>Objeto mutável contendo configurações correntes do jogo.</summary>
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
        [SerializeField] private int maxLives = 5; // Cap superior de vidas
        [SerializeField] private int startingLives = 3; // Vidas iniciais ao começar novo jogo
        [SerializeField] private float respawnDelay = 2f; // Delay entre morte e respawn (se ainda houver vidas)
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableDebugMode = false;

        #endregion

        #region Evolution Configuration
        [Header("Evolution Requirements")]
        [SerializeField] private int fragmentsForAdulto = 10;
        [SerializeField] private int fragmentsForGrandeSlime = 25;
        [SerializeField] private int fragmentsForReiSlime = 50;
        [SerializeField] private int aliadosRequiredForRei = 10;
        #endregion

        #region Debug & Logs
        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[GameManager] {message}");
        }

        private void LogWarning(string message)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"[GameManager] {message}");
        }

        private void LogError(string message)
        {
            if (enableDebugLogs)
                Debug.LogError($"[GameManager] {message}");
        }

        private void DebugLog(string message)
        {
            if (enableDebugMode)
                Debug.Log($"[GameManager DEBUG] {message}");
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Inicializa subsistemas internos: inscrição em eventos, dicionários base e reset da sessão.
        /// Chamado apenas uma vez ao criar a instância singleton.
        /// </summary>
        private void InitializeGameManager()
        {
            SubscribeToEvents();
            InitializeDataStructures();
            ResetSessionData();

            Log("Inicializado para The Slime King");

            // Garante execução das ações de entrada do estado inicial
            EnterState(currentGameState);
        }

        private void InitializeDataStructures()
        {
            // Inicializa dicionário de cristais
            crystalFragments.Clear();
            foreach (ElementType element in System.Enum.GetValues(typeof(ElementType)))
            {
                crystalFragments[element] = 0;
            }

            // Inicializa amizades
            friendshipLevels.Clear();

            // Inicializa expansões
            unlockedHomeExpansions.Clear();
        }

        /// <summary>
        /// Inscreve handlers locais nos eventos globais de UI, gameplay, criaturas, biomas e fluxo.
        /// Mantém GameManager reativo sem dependências diretas fortes.
        /// </summary>
        private void SubscribeToEvents()
        {
            // UI/Menu Events
            UIEvents.OnStartGameRequested += HandleStartGame;
            UIEvents.OnContinueGameRequested += HandleContinueGame;
            UIEvents.OnOptionsRequested += HandleOptionsOpen;
            UIEvents.OnCreditsRequested += HandleCreditsOpen;
            UIEvents.OnMainMenuRequested += HandleMainMenuRequest;
            UIEvents.OnQuitRequested += HandleQuitGame;

            // Gameplay Events
            UIEvents.OnPauseRequested += HandlePauseRequest;
            UIEvents.OnResumeRequested += HandleResumeRequest;
            UIEvents.OnRestartRequested += HandleRestartRequest;
            UIEvents.OnInventoryOpened += HandleInventoryOpen;
            UIEvents.OnInventoryClosed += HandleInventoryClose;
            UIEvents.OnSkillTreeOpened += HandleSkillTreeOpen;
            UIEvents.OnSkillTreeClosed += HandleSkillTreeClose;

            // Player Events
            PlayerEvents.OnPlayerDeath += HandlePlayerDeath;
            PlayerEvents.OnPlayerRespawn += HandlePlayerRespawn;

            // Slime Events
            SlimeEvents.OnCrystalAbsorbed += HandleCrystalAbsorbed;
            SlimeEvents.OnEvolutionTriggered += HandleSlimeEvolution;

            // Creature Events
            CreatureEvents.OnFriendshipIncreased += HandleFriendshipChange;

            // Biome Events
            BiomeEvents.OnBiomeEntered += HandleBiomeChange;

            // Game Flow Events
            GameFlowEvents.OnAreaCompleted += HandleAreaCompletion;
            GameFlowEvents.OnBiomeTransition += HandleBiomeTransition;
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        /// <summary>
        /// Remove inscrições de eventos para evitar memory leaks quando objeto for destruído.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            // UI/Menu Events
            UIEvents.OnStartGameRequested -= HandleStartGame;
            UIEvents.OnContinueGameRequested -= HandleContinueGame;
            UIEvents.OnOptionsRequested -= HandleOptionsOpen;
            UIEvents.OnCreditsRequested -= HandleCreditsOpen;
            UIEvents.OnMainMenuRequested -= HandleMainMenuRequest;
            UIEvents.OnQuitRequested -= HandleQuitGame;

            // Gameplay Events
            UIEvents.OnPauseRequested -= HandlePauseRequest;
            UIEvents.OnResumeRequested -= HandleResumeRequest;
            UIEvents.OnRestartRequested -= HandleRestartRequest;
            UIEvents.OnInventoryOpened -= HandleInventoryOpen;
            UIEvents.OnInventoryClosed -= HandleInventoryClose;
            UIEvents.OnSkillTreeOpened -= HandleSkillTreeOpen;
            UIEvents.OnSkillTreeClosed -= HandleSkillTreeClose;

            // Player Events
            PlayerEvents.OnPlayerDeath -= HandlePlayerDeath;
            PlayerEvents.OnPlayerRespawn -= HandlePlayerRespawn;

            // Slime Events
            SlimeEvents.OnCrystalAbsorbed -= HandleCrystalAbsorbed;
            SlimeEvents.OnEvolutionTriggered -= HandleSlimeEvolution;

            // Creature Events
            CreatureEvents.OnFriendshipIncreased -= HandleFriendshipChange;

            // Biome Events
            BiomeEvents.OnBiomeEntered -= HandleBiomeChange;

            // Game Flow Events
            GameFlowEvents.OnAreaCompleted -= HandleAreaCompletion;
            GameFlowEvents.OnBiomeTransition -= HandleBiomeTransition;
        }
        #endregion

        #region Game State Management
        /// <summary>
        /// Realiza transição de estado do jogo com validação e disparo de evento global.
        /// </summary>
        /// <param name="newState">Novo estado desejado.</param>
        /// <param name="addToHistory">Se true, empilha estado anterior para retorno futuro.</param>
        public void ChangeGameState(GameState newState, bool addToHistory = true)
        {
            if (currentGameState == newState) return;

            // Validação de transições permitidas
            if (!IsValidStateTransition(currentGameState, newState))
            {
                LogWarning($"Transição inválida: {currentGameState} → {newState}");
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
            GameManagerEvents.RaiseGameStateChanged(newState);

            Log($"Estado alterado: {previousGameState} → {newState}");
        }

        /// <summary>
        /// Define regra de transições válidas entre estados.
        /// </summary>
        private bool IsValidStateTransition(GameState from, GameState to)
        {
            // Define transições válidas baseadas no estado atual
            return from switch
            {
                GameState.MainMenu => to is GameState.Options or GameState.Credits or GameState.Loading,
                GameState.Options => to is GameState.MainMenu or GameState.Exploring,
                GameState.Credits => to == GameState.MainMenu,
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
                _ => false
            };
        }

        /// <summary>
        /// Executa lógica de saída (limpeza / suspensão) do estado anterior antes da troca.
        /// </summary>
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

        /// <summary>
        /// Executa lógica de entrada (setup) do novo estado após a troca.
        /// </summary>
        private void EnterState(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    // Inicia música de menu se ainda não estiver tocando
                    if (AudioManager.Instance != null)
                    {
                        if (!AudioManager.Instance.IsPlayingMenuMusic)
                        {
                            Log("[GameManager] Iniciando música de menu no MainMenu");
                            AudioManager.Instance.PlayMenuMusic(crossfade: false);
                        }
                        else
                        {
                            Log("[GameManager] Música de menu já está tocando");
                        }
                    }
                    else
                    {
                        LogWarning("[GameManager] AudioManager.Instance é null - música não será tocada");
                    }
                    LoadMenuScene();
                    break;

                case GameState.Options:
                    // UI Manager cuida da interface
                    break;

                case GameState.Credits:
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

        /// <summary>
        /// Retorna ao último estado armazenado no histórico (se existir).
        /// </summary>
        public void GoToPreviousState()
        {
            if (stateHistory.Count > 0)
            {
                GameState previousState = stateHistory.Pop();
                ChangeGameState(previousState, false); // Não adiciona ao histórico
            }
        }
        #endregion

        #region Initialization Sequences
        /// <summary>
        /// Sequência de carregamento de jogo (reset de sessão, carga de progresso e transição para gameplay).
        /// </summary>
        private IEnumerator LoadGameSequence()
        {
            Log("Iniciando carregamento do jogo");

            // Reseta dados de sessão
            ResetSessionData();

            // Carrega dados salvos do progresso
            LoadGameProgress();

            // Simula loading (pode carregar assets, etc.)
            yield return new WaitForSecondsRealtime(1f);

            // Carrega cena de jogo
            LoadGameScene();

            // Transiciona para exploração
            ChangeGameState(GameState.Exploring);

            // Dispara evento de jogo iniciado
            GameManagerEvents.RaiseGameStarted();
        }

        /// <summary>
        /// Carrega cena de menu principal (placeholder até SceneManager dedicado).
        /// </summary>
        private void LoadMenuScene()
        {
            // Se estamos em modo async e já existe operação de preload, não recarregar
            if (useAsyncSceneLoading && _preloadOp != null)
            {
                Log("[SceneActivation] Carregamento assíncrono já em andamento - evitando LoadScene duplicado");
                return;
            }
            if (string.IsNullOrWhiteSpace(mainMenuSceneName))
            {
                LogWarning("[SceneActivation] mainMenuSceneName não configurado - abortando carregamento de menu");
                return;
            }

            Log($"[SceneActivation] Carregando cena de menu '{mainMenuSceneName}' (async={useAsyncSceneLoading})");

            if (useAsyncSceneLoading)
            {
                StartCoroutine(LoadMenuSceneAsync());
            }
            else
            {
                try
                {
                    SceneManager.LoadScene(mainMenuSceneName);
                }
                catch (System.Exception ex)
                {
                    LogError($"Falha ao carregar cena '{mainMenuSceneName}': {ex.Message}");
                }
            }
        }

        private IEnumerator LoadMenuSceneAsync()
        {
            AsyncOperation op = null;
            try
            {
                op = SceneManager.LoadSceneAsync(mainMenuSceneName);
            }
            catch (System.Exception ex)
            {
                LogError($"Falha ao iniciar carregamento assíncrono da cena '{mainMenuSceneName}': {ex.Message}");
                yield break;
            }

            if (op == null)
                yield break;

            while (!op.isDone)
            {
                // Poderíamos expor progresso via evento futuramente
                yield return null;
            }

            Log($"[SceneActivation] Cena '{mainMenuSceneName}' carregada (Async)");
        }

        /// <summary>
        /// Carrega cena principal de gameplay (placeholder até SceneManager dedicado).
        /// </summary>
        private void LoadGameScene()
        {
            // Transiciona para música de gameplay
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGameplayMusic(crossfade: true);
            }

            // TODO: Implementar carregamento de cena do jogo
            DebugLog("Carregando cena do jogo");
        }

        /// <summary>
        /// Pré-carrega próxima cena em background para transição suave
        /// </summary>
        private IEnumerator PreloadNextScene()
        {
            if (!useAsyncSceneLoading)
            {
                Log("Iniciando pré-carregamento simulado (modo não assíncrono)");
                yield return new WaitForSecondsRealtime(1f);
                Log("[ScenePreload] (Simulado) Próxima cena pré-carregada");
                _nextSceneReady = true;
                yield break;
            }

            if (string.IsNullOrWhiteSpace(mainMenuSceneName))
            {
                LogWarning("[ScenePreload] Nome da cena de menu não configurado - abortando preload async");
                yield break;
            }

            if (_preloadOp != null)
            {
                LogWarning("[ScenePreload] Já existe operação de preload em andamento");
                yield break;
            }

            Log($"[ScenePreload] Iniciando preload assíncrono da cena '{mainMenuSceneName}'");
            try
            {
                _preloadOp = SceneManager.LoadSceneAsync(mainMenuSceneName);
            }
            catch (System.Exception ex)
            {
                LogError($"[ScenePreload] Falha ao iniciar preload: {ex.Message}");
                _preloadOp = null;
                yield break;
            }

            if (_preloadOp == null)
            {
                LogError("[ScenePreload] AsyncOperation retornou null");
                yield break;
            }

            _preloadOp.allowSceneActivation = false;
            Log("[ScenePreload] allowSceneActivation = false (aguardando preload)");

            // Progresso vai até ~0.9 enquanto espera ativação
            while (!_preloadOp.isDone)
            {
                float reportedProgress = Mathf.Clamp01(_preloadOp.progress / 0.9f);
                if (reportedProgress >= 1f - 0.001f)
                {
                    Log("[ScenePreload] Pré-carregamento completo (aguardando ativação)");
                    _nextSceneReady = true;
                    break;
                }
                yield return null;
            }
        }
        #endregion

        #region Scene Transition Helpers
        private bool _nextSceneReady = false;
        private AsyncOperation _preloadOp = null; // Operação assíncrona de preload

        /// <summary>
        /// Ativa cena previamente pré-carregada. Placeholder até implementação real com SceneManager/Addressables.
        /// </summary>
        private void ActivatePreloadedScene()
        {
            if (useAsyncSceneLoading)
            {
                if (_preloadOp == null)
                {
                    LogWarning("[SceneActivation] Não há operação de preload para ativar. Carregando cena diretamente.");
                    LoadMenuScene();
                    return;
                }

                if (!_nextSceneReady)
                {
                    LogWarning("[SceneActivation] Ativação solicitada antes do preload completo - forçando ativação mesmo assim");
                }

                Log("[SceneActivation] Liberando allowSceneActivation = true");
                _preloadOp.allowSceneActivation = true;
            }
            else
            {
                // Modo síncrono: carrega no momento da ativação
                LoadMenuScene();
            }
        }
        #endregion

        #region Game Data Management
        /// <summary>
        /// Limpa dados apenas da sessão atual (não mexe em progressão persistente).
        /// </summary>
        private void ResetSessionData()
        {
            sessionTime = 0f;

            Log("Dados de sessão resetados");
        }

        /// <summary>
        /// Reseta progressão global (usado em Novo Jogo ou Reinício total).
        /// </summary>
        private void ResetGameData()
        {
            currentSlimeStage = SlimeStage.Filhote;
            currentLives = startingLives;
            gameTime = 0f;
            sessionTime = 0f;
            currentBiome = "Ninho";

            // Reset cristais
            InitializeDataStructures();

            // Dispara eventos de atualização
            GameManagerEvents.RaiseSlimeEvolved(currentSlimeStage);
            GameManagerEvents.RaiseLivesChanged(currentLives);
            GameManagerEvents.RaiseBiomeChanged(currentBiome);

            Log("Dados do jogo resetados");
        }

        /// <summary>
        /// Concede uma vida extra se ainda não atingiu o máximo.
        /// </summary>
        public void AddLife()
        {
            if (currentLives < maxLives)
            {
                currentLives++;
                GameManagerEvents.RaiseLivesChanged(currentLives);

                Log($"Vida extra! Total: {currentLives}");
            }
        }

        /// <summary>
        /// Remove uma vida e dispara lógica de morte se chegar a zero.
        /// </summary>
        public void LoseLife()
        {
            currentLives--;
            GameManagerEvents.RaiseLivesChanged(currentLives);

            if (currentLives <= 0)
            {
                ChangeGameState(GameState.Death);
            }

            Log($"Vida perdida. Restantes: {currentLives}");
        }

        /// <summary>
        /// Retorna true se ainda existem vidas restantes.
        /// </summary>
        public bool HasLives()
        {
            return currentLives > 0;
        }
        #endregion

        #region Crystal and Evolution System
        /// <summary>
        /// Soma fragmentos de cristal a um elemento, verifica desbloqueios e evolução.
        /// </summary>
        public void AddCrystalFragments(ElementType element, int amount)
        {
            crystalFragments[element] += amount;
            GameManagerEvents.RaiseCrystalFragmentsChanged(element, crystalFragments[element]);

            // Verifica se desbloqueou elemento pela primeira vez
            if (crystalFragments[element] == amount) // Primeira vez coletando este elemento
            {
                GameManagerEvents.RaiseNewElementUnlocked(element);
            }

            // Verifica se pode evoluir
            CheckEvolutionRequirements();

            Log($"Fragmentos {element}: +{amount} (Total: {crystalFragments[element]})");
        }

        /// <summary>
        /// Retorna quantos fragmentos foram coletados de um elemento específico.
        /// </summary>
        public int GetCrystalFragments(ElementType element)
        {
            return crystalFragments.ContainsKey(element) ? crystalFragments[element] : 0;
        }

        /// <summary>
        /// Soma total de todos os fragmentos coletados (todas as naturezas).
        /// </summary>
        public int GetTotalCrystalFragments()
        {
            int total = 0;
            foreach (var fragments in crystalFragments.Values)
            {
                total += fragments;
            }
            return total;
        }

        /// <summary>
        /// Verifica se os requisitos de evolução foram atingidos e dispara processo.
        /// </summary>
        private void CheckEvolutionRequirements()
        {
            int totalFragments = GetTotalCrystalFragments();
            SlimeStage newStage = currentSlimeStage;

            // Determina próximo estágio baseado em fragmentos
            if (totalFragments >= fragmentsForReiSlime && GetAlliedCreatures() >= aliadosRequiredForRei)
            {
                newStage = SlimeStage.ReiSlime;
            }
            else if (totalFragments >= fragmentsForGrandeSlime)
            {
                newStage = SlimeStage.GrandeSlime;
            }
            else if (totalFragments >= fragmentsForAdulto)
            {
                newStage = SlimeStage.Adulto;
            }

            // Se evoluiu, inicia processo de evolução
            if (newStage != currentSlimeStage)
            {
                TriggerEvolution(newStage);
            }
        }

        /// <summary>
        /// Inicia sequência de evolução trocando estado e iniciando coroutine.
        /// </summary>
        private void TriggerEvolution(SlimeStage newStage)
        {
            ChangeGameState(GameState.Evolution);
            StartCoroutine(EvolutionSequence(newStage));
        }

        /// <summary>
        /// Coroutine que simula processo de evolução (placeholder para efeitos/áudio/animações).
        /// </summary>
        private IEnumerator EvolutionSequence(SlimeStage newStage)
        {
            Log($"Iniciando evolução: {currentSlimeStage} → {newStage}");

            // Aqui poderia ter animações de evolução
            yield return new WaitForSecondsRealtime(2f);

            currentSlimeStage = newStage;
            GameManagerEvents.RaiseSlimeEvolved(currentSlimeStage);

            // Volta ao estado de exploração
            ChangeGameState(GameState.Exploring);

            Log($"Evolução concluída! Agora é: {currentSlimeStage}");
        }
        #endregion

        #region Friendship and Home System
        /// <summary>
        /// Aumenta nível de amizade de uma criatura e verifica expansões do lar.
        /// </summary>
        public void IncreaseFriendship(string creatureName, int amount = 1)
        {
            if (!friendshipLevels.ContainsKey(creatureName))
            {
                friendshipLevels[creatureName] = 0;
            }

            friendshipLevels[creatureName] += amount;
            GameManagerEvents.RaiseFriendshipChanged(creatureName, friendshipLevels[creatureName]);

            // Verifica desbloqueios de expansões do lar
            CheckHomeExpansions(creatureName);

            Log($"Amizade com {creatureName}: +{amount} (Nível: {friendshipLevels[creatureName]})");
        }

        /// <summary>
        /// Retorna o nível de amizade atual com a criatura.
        /// </summary>
        public int GetFriendshipLevel(string creatureName)
        {
            return friendshipLevels.ContainsKey(creatureName) ? friendshipLevels[creatureName] : 0;
        }

        /// <summary>
        /// Conta quantas criaturas têm nível de amizade considerado aliado (>=5).
        /// </summary>
        public int GetAlliedCreatures()
        {
            int allies = 0;
            foreach (var friendship in friendshipLevels.Values)
            {
                if (friendship >= 5) // Considera aliado com nível 5+ de amizade
                {
                    allies++;
                }
            }
            return allies;
        }

        /// <summary>
        /// Avalia se amizades alcançaram critérios para novas expansões do lar.
        /// </summary>
        private void CheckHomeExpansions(string creatureName)
        {
            // Lógica específica baseada no GDD para desbloqueios
            string expansion = "";

            // Exemplos baseados no GDD
            if (creatureName.Contains("Cervo") || creatureName.Contains("Esquilo") || creatureName.Contains("Ourico"))
            {
                if (GetFriendshipLevel(creatureName) >= 3 && !unlockedHomeExpansions.Contains("JardimCristais"))
                {
                    expansion = "JardimCristais";
                }
            }
            else if (creatureName.Contains("Castor"))
            {
                if (GetFriendshipLevel(creatureName) >= 5 && !unlockedHomeExpansions.Contains("LagoInterno"))
                {
                    expansion = "LagoInterno";
                }
            }
            else if (creatureName.Contains("Borboleta"))
            {
                if (GetFriendshipLevel(creatureName) >= 10 && !unlockedHomeExpansions.Contains("SotaoPanoramico"))
                {
                    expansion = "SotaoPanoramico";
                }
            }

            if (!string.IsNullOrEmpty(expansion))
            {
                UnlockHomeExpansion(expansion);
            }
        }

        /// <summary>
        /// Registra expansão recém desbloqueada e emite evento.
        /// </summary>
        private void UnlockHomeExpansion(string expansionName)
        {
            if (!unlockedHomeExpansions.Contains(expansionName))
            {
                unlockedHomeExpansions.Add(expansionName);
                GameManagerEvents.RaiseHomeExpansionUnlocked(expansionName);

                Log($"Expansão desbloqueada: {expansionName}");
            }
        }

        /// <summary>
        /// Verifica se uma expansão já foi desbloqueada.
        /// </summary>
        public bool IsHomeExpansionUnlocked(string expansionName)
        {
            return unlockedHomeExpansions.Contains(expansionName);
        }
        #endregion

        #region Settings Management
        /// <summary>
        /// Aplica configurações atuais (gráficas imediatas e emite evento; áudio futuramente).
        /// </summary>
        public void ApplySettings()
        {
            // Aplica configurações de áudio via AudioManager
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
            GameManagerEvents.RaiseSettingsChanged(gameSettings);

            Log("Configurações aplicadas");
        }

        /// <summary>
        /// Atualiza uma configuração via reflexão (nome deve coincidir com campo público de <see cref="GameSettings"/>).
        /// </summary>
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

        /// <summary>
        /// Carrega configurações persistidas (placeholder até SaveManager).
        /// </summary>
        private void LoadGameSettings()
        {
            // TODO: Implementar quando SaveManager estiver disponível
            DebugLog("Carregando configurações do jogo");
        }

        /// <summary>
        /// Persiste configurações atuais (placeholder até SaveManager).
        /// </summary>
        private void SaveGameSettings()
        {
            // TODO: Implementar quando SaveManager estiver disponível
            DebugLog("Salvando configurações do jogo");
        }

        /// <summary>
        /// Carrega progresso salvo (placeholder até SaveManager).
        /// </summary>
        private void LoadGameProgress()
        {
            // TODO: Implementar quando SaveManager estiver disponível
            DebugLog("Carregando progresso do jogo");
        }
        #endregion

        #region Timer Management
        /// <summary>
        /// Reinicia contador de tempo da sessão.
        /// </summary>
        private void StartGameTimer()
        {
            sessionTime = 0f;
        }

        /// <summary>
        /// Loop de atualização: acumula timers e emite eventos de tempo enquanto explorando.
        /// </summary>
        private void Update()
        {
            if (currentGameState == GameState.Exploring)
            {
                // Atualiza timers apenas durante exploração
                gameTime += Time.deltaTime;
                sessionTime += Time.deltaTime;
                GameManagerEvents.RaiseTimeChanged(sessionTime);
            }
        }
        #endregion

        #region Event Handlers

        // === UI/MENU EVENT HANDLERS ===
        /// <summary>
        /// Handler: iniciar novo jogo a partir do menu.
        /// </summary>
        private void HandleStartGame()
        {
            Log("Iniciando novo jogo");
            ResetGameData();
            ChangeGameState(GameState.Loading);
        }

        /// <summary>
        /// Handler: continuar jogo salvo.
        /// </summary>
        private void HandleContinueGame()
        {
            Log("Continuando jogo salvo");
            ChangeGameState(GameState.Loading);
        }

        /// <summary>Abrir menu de opções.</summary>
        private void HandleOptionsOpen()
        {
            ChangeGameState(GameState.Options);
        }

        /// <summary>Abrir créditos.</summary>
        private void HandleCreditsOpen()
        {
            ChangeGameState(GameState.Credits);
        }

        /// <summary>Retornar ao menu principal.</summary>
        private void HandleMainMenuRequest()
        {
            Log("Retornando ao menu principal");
            ChangeGameState(GameState.MainMenu);
            GameManagerEvents.RaiseReturnToMainMenu();
        }

        /// <summary>Encerrar aplicação (ou parar Play Mode no editor).</summary>
        private void HandleQuitGame()
        {
            Log("Encerrando jogo");
            SaveGameSettings();
            // TODO: Salvar progresso quando SaveManager estiver disponível

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        // === GAMEPLAY EVENT HANDLERS ===
        /// <summary>Pausar o jogo (se em exploração).</summary>
        private void HandlePauseRequest()
        {
            if (currentGameState == GameState.Exploring)
            {
                ChangeGameState(GameState.Paused);
                GameManagerEvents.RaiseGamePaused();
            }
        }

        /// <summary>Retomar jogo a partir do pause.</summary>
        private void HandleResumeRequest()
        {
            if (currentGameState == GameState.Paused)
            {
                ChangeGameState(GameState.Exploring);
                GameManagerEvents.RaiseGameResumed();
            }
        }

        /// <summary>Reiniciar completamente a partida.</summary>
        private void HandleRestartRequest()
        {
            Log("Reiniciando jogo");
            ResetGameData();
            ChangeGameState(GameState.Loading);
        }

        /// <summary>Abrir inventário (pausa lógica sem interromper persistência global).</summary>
        private void HandleInventoryOpen()
        {
            if (currentGameState == GameState.Exploring)
            {
                ChangeGameState(GameState.Inventory);
            }
        }

        /// <summary>Fechar inventário e retornar à exploração.</summary>
        private void HandleInventoryClose()
        {
            if (currentGameState == GameState.Inventory)
            {
                ChangeGameState(GameState.Exploring);
            }
        }

        /// <summary>Abrir árvore de habilidades.</summary>
        private void HandleSkillTreeOpen()
        {
            if (currentGameState == GameState.Exploring)
            {
                ChangeGameState(GameState.SkillTree);
            }
        }

        /// <summary>Fechar árvore de habilidades.</summary>
        private void HandleSkillTreeClose()
        {
            if (currentGameState == GameState.SkillTree)
            {
                ChangeGameState(GameState.Exploring);
            }
        }

        // === PLAYER EVENT HANDLERS ===
        /// <summary>Notificação de morte do player: consome vida e avalia respawn.</summary>
        private void HandlePlayerDeath()
        {
            LoseLife();

            if (currentLives > 0)
            {
                StartCoroutine(RespawnPlayerCoroutine());
            }

            Log($"Slime foi derrotado. Vidas restantes: {currentLives}");
        }

        /// <summary>Confirmação de respawn do player.</summary>
        private void HandlePlayerRespawn()
        {
            Log("Slime renasceu");
        }

        // === SLIME EVENT HANDLERS ===
        /// <summary>Recebe evento de coleta de cristais e roteia para progressão.</summary>
        private void HandleCrystalAbsorbed(ElementType element, int fragments)
        {
            AddCrystalFragments(element, fragments);
        }

        /// <summary>Recebe pedido externo de evolução (altar / evento especial).</summary>
        private void HandleSlimeEvolution(SlimeStage newStage)
        {
            TriggerEvolution(newStage);
        }

        // === CREATURE EVENT HANDLERS ===
        /// <summary>Incrementa amizade ao receber evento de interação social.</summary>
        private void HandleFriendshipChange(string creatureName, int newLevel)
        {
            IncreaseFriendship(creatureName, 1);
        }

        // === BIOME EVENT HANDLERS ===
        /// <summary>Atualiza bioma atual ao entrar em novo ambiente.</summary>
        private void HandleBiomeChange(string biomeName)
        {
            currentBiome = biomeName;
            GameManagerEvents.RaiseBiomeChanged(currentBiome);

            Log($"Entrando no bioma: {biomeName}");
        }

        // === GAME FLOW EVENT HANDLERS ===
        /// <summary>Processa conclusão de área/objetivo.</summary>
        private void HandleAreaCompletion(string areaName)
        {
            Log($"Área completada: {areaName}");
            ChangeGameState(GameState.Victory);
        }

        /// <summary>Processa transição de bioma em fluxo amplo (ex: portal).</summary>
        private void HandleBiomeTransition(string newBiome)
        {
            Log($"Transição para bioma: {newBiome}");
            currentBiome = newBiome;
            GameManagerEvents.RaiseBiomeChanged(currentBiome);
        }
        #endregion

        #region Game Flow
        /// <summary>
        /// Coroutine de respawn: aguarda delay, solicita respawn ao Player e atualiza estado.
        /// </summary>
        private IEnumerator RespawnPlayerCoroutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            // Notifica o PlayerController para fazer respawn
            PlayerEvents.RaiseRespawnRequested();
            GameManagerEvents.RaisePlayerRespawn();

            // Volta ao estado de exploração
            ChangeGameState(GameState.Exploring);
        }

        /// <summary>
        /// Lógica final de morte (todas as vidas esgotadas) - prepara transição ou tela final.
        /// </summary>
        private void HandleSlimeDeath()
        {
            GameManagerEvents.RaisePlayerDeath();

            Log($"O Slime foi completamente derrotado! Estágio: {currentSlimeStage}");
        }

        /// <summary>
        /// Lógica placeholder para vitória/local concluído (expandir futuramente para recompensas/fluxo).
        /// </summary>
        private void HandleAreaVictory()
        {
            Log("Área completada com sucesso!");
            // TODO: Implementar lógica de transição para próxima área
        }

        /// <summary>
        /// API externa para marcar área como concluída (ex: boss defeated / objetivos completos).
        /// </summary>
        public void CompleteArea()
        {
            Log("Área completada!");
            ChangeGameState(GameState.Victory);
        }
        #endregion

        #region Public API

        // === CONTROLE DE ESTADO ===
        /// <summary>API externa: inicia novo jogo (atalho para evento de UI).</summary>
        public void StartNewGame()
        {
            HandleStartGame();
        }

        /// <summary>API externa: continuar jogo salvo.</summary>
        public void ContinueGame()
        {
            HandleContinueGame();
        }

        /// <summary>API externa: abrir opções.</summary>
        public void OpenOptions()
        {
            HandleOptionsOpen();
        }

        /// <summary>API externa: abrir créditos.</summary>
        public void OpenCredits()
        {
            HandleCreditsOpen();
        }

        /// <summary>API externa: pausar jogo.</summary>
        public void PauseGame()
        {
            HandlePauseRequest();
        }

        /// <summary>API externa: retomar jogo.</summary>
        public void ResumeGame()
        {
            HandleResumeRequest();
        }

        /// <summary>API externa: reiniciar jogo.</summary>
        public void RestartGame()
        {
            HandleRestartRequest();
        }

        /// <summary>API externa: retornar ao menu principal.</summary>
        public void ReturnToMainMenu()
        {
            HandleMainMenuRequest();
        }

        /// <summary>API externa: sair do jogo.</summary>
        public void QuitGame()
        {
            HandleQuitGame();
        }

        /// <summary>
        /// Verifica previamente se uma transição de estado seria válida (sem executá-la).
        /// </summary>
        public bool CanTransitionTo(GameState newState)
        {
            return IsValidStateTransition(currentGameState, newState);
        }

        // === PROGRESSÃO DO SLIME ===
        /// <summary>Retorna se o Slime atende requisitos para próxima evolução.</summary>
        public bool CanEvolve()
        {
            int totalFragments = GetTotalCrystalFragments();

            return currentSlimeStage switch
            {
                SlimeStage.Filhote => totalFragments >= fragmentsForAdulto,
                SlimeStage.Adulto => totalFragments >= fragmentsForGrandeSlime,
                SlimeStage.GrandeSlime => totalFragments >= fragmentsForReiSlime && GetAlliedCreatures() >= aliadosRequiredForRei,
                SlimeStage.ReiSlime => false, // Já no estágio final
                _ => false
            };
        }

        /// <summary>Determina próximo estágio de evolução (idempotente se já estiver no final).</summary>
        public SlimeStage GetNextEvolutionStage()
        {
            return currentSlimeStage switch
            {
                SlimeStage.Filhote => SlimeStage.Adulto,
                SlimeStage.Adulto => SlimeStage.GrandeSlime,
                SlimeStage.GrandeSlime => SlimeStage.ReiSlime,
                SlimeStage.ReiSlime => SlimeStage.ReiSlime, // Já no máximo
                _ => SlimeStage.Filhote
            };
        }

        /// <summary>True se estágio atual é o máximo (ReiSlime).</summary>
        public bool IsKingSlime()
        {
            return currentSlimeStage == SlimeStage.ReiSlime;
        }
        #endregion

        #region Debug
        [Header("Debug")]
        [SerializeField] private bool showDebugGUI = false;

        private void OnGUI()
        {
            if (!enableDebugMode || !showDebugGUI) return;

            GUILayout.BeginArea(new Rect(10, 10, 400, 350));
            GUILayout.Label("=== THE SLIME KING - DEBUG ===");
            GUILayout.Label($"Estado: {currentGameState}");
            GUILayout.Label($"Estágio: {currentSlimeStage}");
            GUILayout.Label($"Vidas: {currentLives}");
            GUILayout.Label($"Bioma: {currentBiome}");
            GUILayout.Label($"Tempo Sessão: {sessionTime:F1}s");
            GUILayout.Label($"Tempo Total: {gameTime:F1}s");
            GUILayout.Label($"Fragmentos Totais: {GetTotalCrystalFragments()}");
            GUILayout.Label($"Aliados: {GetAlliedCreatures()}");

            GUILayout.Space(10);

            if (GUILayout.Button("Add Fire Fragments"))
                AddCrystalFragments(ElementType.Fire, 5);

            if (GUILayout.Button("Add Life"))
                AddLife();

            if (GUILayout.Button("Trigger Evolution"))
                TriggerEvolution(GetNextEvolutionStage());

            if (GUILayout.Button("Death"))
                ChangeGameState(GameState.Death);

            if (GUILayout.Button("Main Menu"))
                ChangeGameState(GameState.MainMenu);

            if (GUILayout.Button("Test Friendship"))
                IncreaseFriendship("TestCervo", 1);

            GUILayout.EndArea();
        }
        #endregion
    }
}