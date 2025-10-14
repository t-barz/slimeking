using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlimeKing.Events;

namespace SlimeKing.Core
{
    /// <summary>
    /// Manager central responsável pelo estado global do jogo, tempo e evolução do slime.
    /// Implementação simplificada seguindo princípios KISS.
    /// </summary>
    public class GameManager : ManagerSingleton<GameManager>
    {
        #region Game State

        [Header("Game State")]
        [SerializeField] private GameState currentGameState = GameState.MainMenu;
        [SerializeField] private bool isPaused = false;
        [SerializeField] private int currentLives = 3;
        [SerializeField] private int maxLives = 3;

        #endregion

        #region Time System

        [Header("Time System")]
        [SerializeField] private float gameTime = 7f; // Em horas de jogo (0-24), começar de manhã
        [SerializeField] private int currentDay = 1;
        [SerializeField] private Season currentSeason = Season.Spring;
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        [SerializeField] private float timeMultiplier = 1f; // 1 min real = 1 hora jogo

        #endregion

        #region Slime Evolution

        [Header("Slime Evolution")]
        [SerializeField] private SlimeStage currentSlimeStage = SlimeStage.Baby;
        [SerializeField] private int totalCrystalFragments = 0;
        [SerializeField] private Dictionary<ElementType, int> crystalFragments = new();
        [SerializeField] private int allyCount = 0;

        #endregion

        #region Scene Management

        [Header("Scene Management")]
        [SerializeField] private string currentBiome = "Ninho";
        [SerializeField] private BiomeType currentBiomeType = BiomeType.Nest;

        #endregion

        #region Settings

        [Header("Settings")]
        [SerializeField] private GameSettings gameSettings = new GameSettings();

        #endregion

        #region Private Variables

        private bool isTimeRunning = true;
        private Coroutine timeCoroutine;
        private WaitForSecondsRealtime timeWait;

        #endregion

        #region Properties

        public GameState CurrentGameState => currentGameState;
        public bool IsPaused => isPaused;
        public int CurrentLives => currentLives;
        public float GameTime => gameTime;
        public int CurrentDay => currentDay;
        public Season CurrentSeason => currentSeason;
        public WeatherType CurrentWeather => currentWeather;
        public SlimeStage CurrentSlimeStage => currentSlimeStage;
        public int TotalCrystalFragments => totalCrystalFragments;
        public int AllyCount => allyCount;
        public string CurrentBiome => currentBiome;
        public BiomeType CurrentBiomeType => currentBiomeType;
        public GameSettings Settings => gameSettings;

        #endregion

        #region Singleton Implementation

        protected override void Initialize()
        {
            Log("GameManager initialized successfully");
            InitializeCrystalFragments();
            InitializeTimeSystem();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy(); // garante execução de OnManagerDestroy da base
            StopTimeSystem();
        }

        #endregion

        #region Initialization

        private void InitializeCrystalFragments()
        {
            crystalFragments.Clear();
            foreach (ElementType element in Enum.GetValues(typeof(ElementType)))
            {
                crystalFragments[element] = 0;
            }
            Log("Crystal fragments dictionary initialized");
        }

        private void InitializeTimeSystem()
        {
            timeWait = new WaitForSecondsRealtime(60f); // 1 minuto real = 1 hora de jogo
            StartTimeSystem();
        }

        #endregion

        #region Game State Management

        public void ChangeGameState(GameState newState)
        {
            if (currentGameState == newState) return;

            GameState previousState = currentGameState;
            currentGameState = newState;

            Log($"Game state changed from {previousState} to {newState}");

            // Handle state-specific logic
            switch (newState)
            {
                case GameState.Playing:
                    ResumeTime();
                    break;
                case GameState.Paused:
                    PauseTime();
                    break;
                case GameState.MainMenu:
                case GameState.Loading:
                case GameState.Settings:
                    PauseTime();
                    break;
            }

            GameEvents.TriggerGameStateChanged(newState);
        }

        public void PauseGame()
        {
            if (!isPaused)
            {
                isPaused = true;
                PauseTime();
                GameEvents.TriggerGamePaused();
                Log("Game paused");
            }
        }

        public void ResumeGame()
        {
            if (isPaused)
            {
                isPaused = false;
                ResumeTime();
                GameEvents.TriggerGameResumed();
                Log("Game resumed");
            }
        }

        #endregion

        #region Time System

        private void StartTimeSystem()
        {
            if (timeCoroutine == null)
            {
                timeCoroutine = StartCoroutine(TimeLoop());
                isTimeRunning = true;
                Log("Time system started");
            }
        }

        private void StopTimeSystem()
        {
            if (timeCoroutine != null)
            {
                StopCoroutine(timeCoroutine);
                timeCoroutine = null;
                isTimeRunning = false;
                Log("Time system stopped");
            }
        }

        private void PauseTime()
        {
            isTimeRunning = false;
        }

        private void ResumeTime()
        {
            isTimeRunning = true;
        }

        private IEnumerator TimeLoop()
        {
            while (true)
            {
                yield return timeWait;

                if (isTimeRunning && currentGameState == GameState.Playing)
                {
                    AdvanceTime();
                }
            }
        }

        private void AdvanceTime()
        {
            gameTime += timeMultiplier;

            // Check for day change
            if (gameTime >= 24f)
            {
                gameTime = 0f;
                currentDay++;
                CheckSeasonChange();
                ChangeWeather();

                GameEvents.TriggerNewDay(currentDay);
                Log($"New day: {currentDay}");
            }

            GameEvents.TriggerTimeAdvanced(gameTime);
        }

        private void CheckSeasonChange()
        {
            // Cada estação dura 7 dias
            int seasonDay = (currentDay - 1) % 28;
            Season newSeason = (Season)(seasonDay / 7);

            if (newSeason != currentSeason)
            {
                currentSeason = newSeason;
                GameEvents.TriggerSeasonChanged(currentSeason);
                Log($"Season changed to {currentSeason}");
            }
        }

        private void ChangeWeather()
        {
            // Chance simples de mudança de clima
            if (UnityEngine.Random.Range(0f, 1f) < 0.3f) // 30% chance
            {
                Array weatherTypes = Enum.GetValues(typeof(WeatherType));
                WeatherType newWeather = (WeatherType)weatherTypes.GetValue(UnityEngine.Random.Range(0, weatherTypes.Length));

                if (newWeather != currentWeather)
                {
                    currentWeather = newWeather;
                    GameEvents.TriggerWeatherChanged(currentWeather);
                    Log($"Weather changed to {currentWeather}");
                }
            }
        }

        #endregion

        #region Slime Evolution

        public void AddCrystalFragment(ElementType elementType, int amount = 1)
        {
            crystalFragments[elementType] += amount;
            totalCrystalFragments += amount;

            Log($"Added {amount} {elementType} crystal fragment(s). Total: {crystalFragments[elementType]}");

            CheckEvolution();
            GameEvents.TriggerElementalXPGained(elementType, amount);
        }

        public int GetCrystalFragments(ElementType elementType)
        {
            return crystalFragments.ContainsKey(elementType) ? crystalFragments[elementType] : 0;
        }

        private void CheckEvolution()
        {
            SlimeStage newStage = CalculateEvolutionStage();

            if (newStage != currentSlimeStage)
            {
                currentSlimeStage = newStage;
                GameEvents.TriggerSlimeEvolved(currentSlimeStage);
                Log($"Slime evolved to {currentSlimeStage}!");
            }
        }

        private SlimeStage CalculateEvolutionStage()
        {
            // Requisitos baseados no Game Design Document
            if (totalCrystalFragments >= 50 && allyCount >= 10 && currentSlimeStage >= SlimeStage.Large)
            {
                return SlimeStage.King;
            }
            else if (totalCrystalFragments >= 25)
            {
                return SlimeStage.Large;
            }
            else if (totalCrystalFragments >= 10)
            {
                return SlimeStage.Adult;
            }

            return SlimeStage.Baby;
        }

        #endregion

        #region Player Management

        public void PlayerDied()
        {
            currentLives--;
            Log($"Player died. Lives remaining: {currentLives}");

            if (currentLives <= 0)
            {
                GameOver();
            }
        }

        public void AddLife()
        {
            if (currentLives < maxLives)
            {
                currentLives++;
                Log($"Life added. Current lives: {currentLives}");
            }
        }

        private void GameOver()
        {
            ChangeGameState(GameState.MainMenu);
            Log("Game Over");
        }

        #endregion

        #region Scene Management

        public void SetCurrentBiome(string biomeName, BiomeType biomeType)
        {
            currentBiome = biomeName;
            currentBiomeType = biomeType;

            Log($"Current biome set to: {biomeName} ({biomeType})");
            GameEvents.TriggerBiomeEntered(biomeType);
        }

        #endregion

        #region Friendship System

        public void AddAlly()
        {
            allyCount++;
            CheckEvolution(); // Pode desbloquear Rei Slime
            Log($"New ally added. Total allies: {allyCount}");
        }

        #endregion

        #region Settings Management

        public void UpdateSettings(GameSettings newSettings)
        {
            gameSettings = newSettings;
            Log("Game settings updated");
        }

        #endregion

        #region Public Utilities

        public TimeOfDay GetCurrentTimeOfDay()
        {
            if (gameTime >= 5f && gameTime < 7f) return TimeOfDay.Dawn;
            if (gameTime >= 7f && gameTime < 12f) return TimeOfDay.Morning;
            if (gameTime >= 12f && gameTime < 18f) return TimeOfDay.Afternoon;
            if (gameTime >= 18f && gameTime < 21f) return TimeOfDay.Evening;
            return TimeOfDay.Night;
        }

        public bool CanEvolve()
        {
            return CalculateEvolutionStage() > currentSlimeStage;
        }

        public void StartNewGame()
        {
            // Reset para estado inicial
            currentLives = maxLives;
            gameTime = 7f; // Começar de manhã
            currentDay = 1;
            currentSeason = Season.Spring;
            currentWeather = WeatherType.Clear;
            currentSlimeStage = SlimeStage.Baby;
            totalCrystalFragments = 0;
            allyCount = 0;

            InitializeCrystalFragments();
            ChangeGameState(GameState.Playing);

            Log("New game started");
        }

        /// <summary>
        /// Retorna o tempo formatado (HH:MM)
        /// </summary>
        public string GetFormattedTime()
        {
            int hours = Mathf.FloorToInt(gameTime);
            int minutes = Mathf.FloorToInt((gameTime - hours) * 60);
            return $"{hours:00}:{minutes:00}";
        }

        #endregion

        #region Debug Methods

#if UNITY_EDITOR
        [ContextMenu("Add Test Crystal")]
        private void AddTestCrystal()
        {
            AddCrystalFragment(ElementType.Fire, 5);
        }

        [ContextMenu("Force Evolution")]
        private void ForceEvolution()
        {
            if (currentSlimeStage != SlimeStage.King)
            {
                totalCrystalFragments += 15;
                CheckEvolution();
            }
        }

        [ContextMenu("Add Ally")]
        private void AddTestAlly()
        {
            AddAlly();
        }

        [ContextMenu("Skip Day")]
        private void SkipDay()
        {
            gameTime = 0f;
            AdvanceTime();
        }
#endif

        #endregion
    }

    #region GameSettings Serializable Class

    [System.Serializable]
    public class GameSettings
    {
        [Header("Audio")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.8f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        [Header("Graphics")]
        public bool fullscreen = true;
        public int resolutionIndex = 0;
        public bool vSync = true;

        [Header("Gameplay")]
        public bool showTutorials = true;
        public bool showDamageNumbers = true;
        [Range(0.5f, 2f)] public float uiScale = 1f;

        [Header("Accessibility")]
        public bool subtitles = false;
        public bool colorBlindMode = false;
        [Range(0.8f, 1.5f)] public float textSize = 1f;
    }

    #endregion
}