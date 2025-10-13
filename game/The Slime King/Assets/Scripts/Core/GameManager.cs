using UnityEngine;
using UnityEngine.Events;

namespace SlimeKing.Core
{
    /// <summary>
    /// Manager principal que coordena o estado geral do jogo.
    /// Controla pausas, transições de estado e coordenação entre outros managers.
    /// </summary>
    public class GameManager : ManagerBase<GameManager>
    {
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        [SerializeField] private bool isPaused = false;

        [Header("Game Data")]
        [SerializeField] private float gameTime = 0f;
        [SerializeField] private int currentLevel = 1;

        [Header("Events")]
        public UnityEvent<GameState> OnGameStateChanged = new UnityEvent<GameState>();
        public UnityEvent OnGamePaused = new UnityEvent();
        public UnityEvent OnGameResumed = new UnityEvent();
        public UnityEvent OnGameStarted = new UnityEvent();
        public UnityEvent OnGameEnded = new UnityEvent();

        // Properties
        public GameState CurrentState => currentState;
        public bool IsPaused => isPaused;
        public float GameTime => gameTime;
        public int CurrentLevel => currentLevel;

        protected override void Initialize()
        {
            Log("GameManager initialized");
        }

        private void Update()
        {
            if (currentState == GameState.Playing && !isPaused)
            {
                gameTime += Time.deltaTime;
            }
        }

        #region Game State Management

        /// <summary>
        /// Muda o estado do jogo
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            if (currentState != newState)
            {
                GameState previousState = currentState;
                currentState = newState;

                OnStateChanged(previousState, newState);
                OnGameStateChanged?.Invoke(newState);

                Log($"State changed from {previousState} to {newState}");
            }
        }

        /// <summary>
        /// Lógica executada quando o estado muda
        /// </summary>
        private void OnStateChanged(GameState previousState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Playing:
                    if (previousState == GameState.MainMenu)
                    {
                        StartGame();
                    }
                    break;

                case GameState.Paused:
                    PauseGame();
                    break;

                case GameState.GameOver:
                    EndGame();
                    break;
            }
        }

        #endregion

        #region Game Control

        /// <summary>
        /// Inicia o jogo
        /// </summary>
        public void StartGame()
        {
            gameTime = 0f;
            isPaused = false;
            Time.timeScale = 1f;

            OnGameStarted?.Invoke();
            Log("Game started");
        }

        /// <summary>
        /// Pausa o jogo
        /// </summary>
        public void PauseGame()
        {
            if (!isPaused && currentState == GameState.Playing)
            {
                isPaused = true;
                Time.timeScale = 0f;

                OnGamePaused?.Invoke();
                Log("Game paused");
            }
        }

        /// <summary>
        /// Resume o jogo
        /// </summary>
        public void ResumeGame()
        {
            if (isPaused)
            {
                isPaused = false;
                Time.timeScale = 1f;

                if (currentState == GameState.Paused)
                {
                    ChangeGameState(GameState.Playing);
                }

                OnGameResumed?.Invoke();
                Log("Game resumed");
            }
        }

        /// <summary>
        /// Termina o jogo
        /// </summary>
        public void EndGame()
        {
            isPaused = false;
            Time.timeScale = 1f;

            OnGameEnded?.Invoke();
            Log("Game ended");
        }

        /// <summary>
        /// Reinicia o jogo
        /// </summary>
        public void RestartGame()
        {
            Log("Restarting game");

            // Reset do estado
            gameTime = 0f;
            isPaused = false;
            Time.timeScale = 1f;

            // Volta para o estado inicial
            ChangeGameState(GameState.Playing);
            StartGame();
        }

        /// <summary>
        /// Sai do jogo
        /// </summary>
        public void QuitGame()
        {
            Log("Quitting game");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        #endregion

        #region Level Management

        /// <summary>
        /// Avança para o próximo nível
        /// </summary>
        public void NextLevel()
        {
            currentLevel++;
            Log($"Advanced to level {currentLevel}");
        }

        /// <summary>
        /// Define o nível atual
        /// </summary>
        public void SetLevel(int level)
        {
            currentLevel = Mathf.Max(1, level);
            Log($"Level set to {currentLevel}");
        }

        #endregion

        #region Unity Events

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && currentState == GameState.Playing)
            {
                PauseGame();
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && currentState == GameState.Playing)
            {
                PauseGame();
            }
        }

        #endregion
    }
}