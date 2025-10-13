using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlimeKing.Events;

namespace SlimeKing.Core
{
    /// <summary>
    /// Manager principal responsável pelo estado global do jogo e coordenação entre sistemas.
    /// Gerencia ciclo temporal, evolução do slime, estados do jogo e transições entre cenas.
    /// </summary>
    public class GameManager : ManagerSingleton<GameManager>
    {
        #region Game State

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Playing;
        [SerializeField] private bool isPaused = false;

        /// <summary>
        /// Estado atual do jogo
        /// </summary>
        public GameState CurrentState => currentState;

        /// <summary>
        /// Verifica se o jogo está pausado
        /// </summary>
        public bool IsPaused => isPaused;

        #endregion

        #region Time System

        [Header("Time System")]
        [SerializeField] private float gameTimeSpeed = 1f; // Multiplicador de velocidade do tempo
        [SerializeField] private bool autoAdvanceTime = true;

        [Header("Time Values")]
        [SerializeField] private float gameTime = 7f; // Começa às 7:00 da manhã
        [SerializeField] private int currentDay = 1;
        [SerializeField] private Season currentSeason = Season.Spring;
        [SerializeField] private TimeOfDay currentTimeOfDay = TimeOfDay.Morning;

        [Header("Weather System")]
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        [SerializeField] private float weatherChangeChance = 0.1f; // Chance por hora de mudar clima

        // Constantes de tempo
        private const float HOURS_PER_DAY = 24f;
        private const float REAL_SECONDS_PER_GAME_HOUR = 60f; // 1 hora do jogo = 1 minuto real
        private const int DAYS_PER_SEASON = 7; // 7 dias reais = 1 estação

        // Cache para otimização
        private TimeOfDay lastTimeOfDay;
        private Season lastSeason;
        private int lastDay;

        #endregion

        #region Slime Evolution

        [Header("Slime Evolution")]
        [SerializeField] private SlimeStage currentSlimeStage = SlimeStage.Baby;
        [SerializeField] private Dictionary<ElementType, int> elementalXP = new Dictionary<ElementType, int>();

        [Header("Evolution Thresholds")]
        [SerializeField] private int xpToAdult = 100;
        [SerializeField] private int xpToLarge = 300;
        [SerializeField] private int xpToKing = 500;

        /// <summary>
        /// Estágio atual de evolução do slime
        /// </summary>
        public SlimeStage CurrentSlimeStage => currentSlimeStage;

        /// <summary>
        /// XP elemental atual do slime
        /// </summary>
        public Dictionary<ElementType, int> ElementalXP => new Dictionary<ElementType, int>(elementalXP);

        #endregion

        #region Scene Management

        [Header("Scene Management")]
        [SerializeField] private BiomeType currentBiome = BiomeType.Nest;
        [SerializeField] private string currentSceneName = "NestScene";

        /// <summary>
        /// Bioma atual do jogador
        /// </summary>
        public BiomeType CurrentBiome => currentBiome;

        #endregion

        #region Unity Lifecycle

        protected override void Initialize()
        {
            Log("GameManager initialization started");

            // Inicializar dicionário de XP elemental
            InitializeElementalXP();

            // Cache dos valores iniciais
            lastTimeOfDay = currentTimeOfDay;
            lastSeason = currentSeason;
            lastDay = currentDay;

            // Disparar evento inicial de estado
            GameEvents.TriggerGameStateChanged(currentState);

            Log("GameManager initialization completed");
        }

        private void Start()
        {
            // Iniciar sistema de tempo se estiver em jogo
            if (currentState == GameState.Playing && autoAdvanceTime)
            {
                StartCoroutine(TimeAdvancementCoroutine());
            }
        }

        private void Update()
        {
            // Input de pause (pode ser configurado via Input System depois)
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        #endregion

        #region Game State Management

        /// <summary>
        /// Muda o estado do jogo
        /// </summary>
        public void ChangeGameState(GameState newState)
        {
            if (currentState == newState) return;

            Log($"Game state changing from {currentState} to {newState}");

            var previousState = currentState;
            currentState = newState;

            // Lógica específica por estado
            HandleStateTransition(previousState, newState);

            // Disparar evento
            GameEvents.TriggerGameStateChanged(newState);
        }

        /// <summary>
        /// Pausa ou despausa o jogo
        /// </summary>
        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
            }
            else if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
        }

        /// <summary>
        /// Pausa o jogo
        /// </summary>
        public void PauseGame()
        {
            if (currentState != GameState.Playing) return;

            isPaused = true;
            Time.timeScale = 0f;
            ChangeGameState(GameState.Paused);
            GameEvents.TriggerGamePaused();

            Log("Game paused");
        }

        /// <summary>
        /// Despausa o jogo
        /// </summary>
        public void ResumeGame()
        {
            if (currentState != GameState.Paused) return;

            isPaused = false;
            Time.timeScale = 1f;
            ChangeGameState(GameState.Playing);
            GameEvents.TriggerGameResumed();

            Log("Game resumed");
        }

        private void HandleStateTransition(GameState fromState, GameState toState)
        {
            // Lógica específica de transição de estados
            switch (toState)
            {
                case GameState.Playing:
                    if (autoAdvanceTime)
                        StartCoroutine(TimeAdvancementCoroutine());
                    break;

                case GameState.Paused:
                    StopAllCoroutines();
                    break;

                case GameState.Loading:
                    StopAllCoroutines();
                    break;
            }
        }

        #endregion

        #region Time System

        /// <summary>
        /// Corrotina principal do sistema de tempo
        /// </summary>
        private IEnumerator TimeAdvancementCoroutine()
        {
            while (currentState == GameState.Playing && autoAdvanceTime)
            {
                yield return new WaitForSeconds(REAL_SECONDS_PER_GAME_HOUR / gameTimeSpeed);

                if (!isPaused)
                {
                    AdvanceTime(1f); // Avançar 1 hora
                }
            }
        }

        /// <summary>
        /// Avança o tempo do jogo
        /// </summary>
        /// <param name="hours">Número de horas para avançar</param>
        public void AdvanceTime(float hours)
        {
            gameTime += hours;

            // Controle de dias
            if (gameTime >= HOURS_PER_DAY)
            {
                gameTime -= HOURS_PER_DAY;
                currentDay++;
                GameEvents.TriggerNewDay(currentDay);
                Log($"New day started: Day {currentDay}");
            }

            // Atualizar período do dia
            UpdateTimeOfDay();

            // Atualizar estação (baseado no dia)
            UpdateSeason();

            // Sistema de clima dinâmico
            UpdateWeather();

            // Disparar evento de avanço de tempo
            GameEvents.TriggerTimeAdvanced(gameTime);
        }

        private void UpdateTimeOfDay()
        {
            TimeOfDay newTimeOfDay = GetTimeOfDayFromHour(gameTime);

            if (newTimeOfDay != lastTimeOfDay)
            {
                currentTimeOfDay = newTimeOfDay;
                lastTimeOfDay = newTimeOfDay;
                GameEvents.TriggerTimeOfDayChanged(currentTimeOfDay);
                Log($"Time of day changed to: {currentTimeOfDay}");
            }
        }

        private TimeOfDay GetTimeOfDayFromHour(float hour)
        {
            if (hour >= 5f && hour < 7f) return TimeOfDay.Dawn;
            if (hour >= 7f && hour < 12f) return TimeOfDay.Morning;
            if (hour >= 12f && hour < 18f) return TimeOfDay.Afternoon;
            if (hour >= 18f && hour < 21f) return TimeOfDay.Evening;
            return TimeOfDay.Night;
        }

        private void UpdateSeason()
        {
            Season newSeason = GetSeasonFromDay(currentDay);

            if (newSeason != lastSeason)
            {
                currentSeason = newSeason;
                lastSeason = newSeason;
                GameEvents.TriggerSeasonChanged(currentSeason);
                Log($"Season changed to: {currentSeason}");
            }
        }

        private Season GetSeasonFromDay(int day)
        {
            int seasonCycle = (day - 1) / DAYS_PER_SEASON;
            return (Season)(seasonCycle % 4);
        }

        private void UpdateWeather()
        {
            // Sistema simples de mudança climática baseado em chance
            if (UnityEngine.Random.value < weatherChangeChance)
            {
                ChangeWeather();
            }
        }

        private void ChangeWeather()
        {
            var weatherValues = Enum.GetValues(typeof(WeatherType));
            WeatherType newWeather;

            do
            {
                newWeather = (WeatherType)weatherValues.GetValue(UnityEngine.Random.Range(0, weatherValues.Length));
            }
            while (newWeather == currentWeather);

            SetWeather(newWeather);
        }

        /// <summary>
        /// Define o clima atual
        /// </summary>
        public void SetWeather(WeatherType weather)
        {
            if (currentWeather == weather) return;

            currentWeather = weather;
            GameEvents.TriggerWeatherChanged(weather);
            Log($"Weather changed to: {weather}");
        }

        #endregion

        #region Slime Evolution

        private void InitializeElementalXP()
        {
            elementalXP.Clear();
            foreach (ElementType element in Enum.GetValues(typeof(ElementType)))
            {
                elementalXP[element] = 0;
            }
        }

        /// <summary>
        /// Adiciona XP elemental ao slime
        /// </summary>
        public void AddElementalXP(ElementType element, int amount)
        {
            if (amount <= 0) return;

            elementalXP[element] += amount;
            GameEvents.TriggerElementalXPGained(element, amount);

            Log($"Added {amount} XP to {element} element. Total: {elementalXP[element]}");

            // Verificar se pode evoluir
            CheckEvolution();
        }

        private void CheckEvolution()
        {
            int totalXP = GetTotalElementalXP();
            SlimeStage newStage = currentSlimeStage;

            if (totalXP >= xpToKing && currentSlimeStage != SlimeStage.King)
            {
                newStage = SlimeStage.King;
            }
            else if (totalXP >= xpToLarge && currentSlimeStage == SlimeStage.Adult)
            {
                newStage = SlimeStage.Large;
            }
            else if (totalXP >= xpToAdult && currentSlimeStage == SlimeStage.Baby)
            {
                newStage = SlimeStage.Adult;
            }

            if (newStage != currentSlimeStage)
            {
                EvolveSlime(newStage);
            }
        }

        /// <summary>
        /// Força a evolução do slime para um estágio específico
        /// </summary>
        public void EvolveSlime(SlimeStage newStage)
        {
            if (newStage == currentSlimeStage) return;

            Log($"Slime evolved from {currentSlimeStage} to {newStage}!");

            currentSlimeStage = newStage;
            GameEvents.TriggerSlimeEvolved(newStage);
        }

        /// <summary>
        /// Retorna o XP total de todos os elementos
        /// </summary>
        public int GetTotalElementalXP()
        {
            int total = 0;
            foreach (var xp in elementalXP.Values)
            {
                total += xp;
            }
            return total;
        }

        /// <summary>
        /// Retorna o XP de um elemento específico
        /// </summary>
        public int GetElementalXP(ElementType element)
        {
            return elementalXP.ContainsKey(element) ? elementalXP[element] : 0;
        }

        #endregion

        #region Scene Management

        /// <summary>
        /// Notifica mudança de bioma
        /// </summary>
        public void ChangeBiome(BiomeType newBiome, string sceneName = "")
        {
            if (newBiome == currentBiome) return;

            var previousBiome = currentBiome;
            currentBiome = newBiome;

            if (!string.IsNullOrEmpty(sceneName))
            {
                currentSceneName = sceneName;
            }

            GameEvents.TriggerBiomeExited(previousBiome);
            GameEvents.TriggerBiomeEntered(newBiome);

            Log($"Biome changed from {previousBiome} to {newBiome}");
        }

        #endregion

        #region Public Getters

        /// <summary>
        /// Retorna informações de tempo formatadas
        /// </summary>
        public string GetFormattedTime()
        {
            int hours = Mathf.FloorToInt(gameTime);
            int minutes = Mathf.FloorToInt((gameTime - hours) * 60);
            return $"{hours:00}:{minutes:00}";
        }

        /// <summary>
        /// Retorna o progresso do dia (0-1)
        /// </summary>
        public float GetDayProgress()
        {
            return gameTime / HOURS_PER_DAY;
        }

        /// <summary>
        /// Retorna informações completas do estado atual
        /// </summary>
        public string GetGameStateInfo()
        {
            return $"Day {currentDay}, {GetFormattedTime()}, {currentSeason}, {currentWeather}, {currentTimeOfDay}";
        }

        #endregion

        #region Debug & Editor

        [Header("Debug Tools")]
        [SerializeField] private bool showDebugInfo = false;

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!showDebugInfo) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Box("GameManager Debug Info");

            GUILayout.Label($"State: {currentState}");
            GUILayout.Label($"Time: {GetFormattedTime()}");
            GUILayout.Label($"Day: {currentDay}");
            GUILayout.Label($"Season: {currentSeason}");
            GUILayout.Label($"Weather: {currentWeather}");
            GUILayout.Label($"Time of Day: {currentTimeOfDay}");
            GUILayout.Label($"Slime Stage: {currentSlimeStage}");
            GUILayout.Label($"Total XP: {GetTotalElementalXP()}");
            GUILayout.Label($"Biome: {currentBiome}");

            GUILayout.EndArea();
        }

        [UnityEngine.ContextMenu("Add Test XP")]
        private void AddTestXP()
        {
            AddElementalXP(ElementType.Fire, 25);
        }

        [UnityEngine.ContextMenu("Force Evolution")]
        private void ForceEvolution()
        {
            if (currentSlimeStage != SlimeStage.King)
            {
                EvolveSlime((SlimeStage)((int)currentSlimeStage + 1));
            }
        }

        [UnityEngine.ContextMenu("Change Weather")]
        private void DebugChangeWeather()
        {
            ChangeWeather();
        }
#endif

        #endregion
    }
}