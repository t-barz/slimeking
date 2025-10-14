using System;

using SlimeKing.Core;

namespace SlimeKing.Events
{
    /// <summary>
    /// Centralizador de todos os eventos globais do jogo.
    /// Usa eventos estáticos para comunicação desacoplada entre sistemas.
    /// </summary>
    public static class GameEvents
    {
        #region Game State Events

        /// <summary>
        /// Disparado quando o estado do jogo muda
        /// </summary>
        public static event Action<GameState> OnGameStateChanged;

        /// <summary>
        /// Disparado quando o jogo é pausado
        /// </summary>
        public static event Action OnGamePaused;

        /// <summary>
        /// Disparado quando o jogo é despausado
        /// </summary>
        public static event Action OnGameResumed;

        #endregion

        #region Time System Events

        /// <summary>
        /// Disparado a cada avanço do tempo no jogo
        /// Parâmetro: tempo atual em horas (float)
        /// </summary>
        public static event Action<float> OnTimeAdvanced;

        /// <summary>
        /// Disparado quando muda o período do dia
        /// </summary>
        public static event Action<TimeOfDay> OnTimeOfDayChanged;

        /// <summary>
        /// Disparado quando a estação muda
        /// </summary>
        public static event Action<Season> OnSeasonChanged;

        /// <summary>
        /// Disparado quando o clima muda
        /// </summary>
        public static event Action<WeatherType> OnWeatherChanged;

        /// <summary>
        /// Disparado quando um novo dia começa
        /// Parâmetro: número do dia
        /// </summary>
        public static event Action<int> OnNewDay;

        #endregion

        #region Slime Evolution Events

        /// <summary>
        /// Disparado quando o slime evolui para um novo estágio
        /// </summary>
        public static event Action<SlimeStage> OnSlimeEvolved;

        /// <summary>
        /// Disparado quando o slime ganha XP elemental
        /// Parâmetros: tipo do elemento, quantidade de XP
        /// </summary>
        public static event Action<ElementType, int> OnElementalXPGained;


        #endregion

        #region Scene Events

        /// <summary>
        /// Disparado quando uma nova cena/bioma é carregado
        /// Parâmetro: nome da cena
        /// </summary>
        public static event Action<string> OnSceneLoaded;

        /// <summary>
        /// Disparado quando o jogador entra em um bioma
        /// </summary>
        public static event Action<BiomeType> OnBiomeEntered;

        /// <summary>
        /// Disparado quando o jogador sai de um bioma
        /// </summary>
        public static event Action<BiomeType> OnBiomeExited;

        #endregion

        #region Utility Methods

        /// <summary>
        /// Invoca o evento de mudança de estado do jogo
        /// </summary>
        public static void TriggerGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        /// <summary>
        /// Invoca o evento de avanço do tempo
        /// </summary>
        public static void TriggerTimeAdvanced(float currentTime)
        {
            OnTimeAdvanced?.Invoke(currentTime);
        }

        /// <summary>
        /// Invoca o evento de evolução do slime
        /// </summary>
        public static void TriggerSlimeEvolved(SlimeStage newStage)
        {
            OnSlimeEvolved?.Invoke(newStage);
        }

        /// <summary>
        /// Invoca o evento de mudança de clima
        /// </summary>
        public static void TriggerWeatherChanged(WeatherType newWeather)
        {
            OnWeatherChanged?.Invoke(newWeather);
        }

        /// <summary>
        /// Invoca o evento de mudança de estação
        /// </summary>
        public static void TriggerSeasonChanged(Season newSeason)
        {
            OnSeasonChanged?.Invoke(newSeason);
        }

        /// <summary>
        /// Invoca o evento de entrada em bioma
        /// </summary>
        public static void TriggerBiomeEntered(BiomeType biome)
        {
            OnBiomeEntered?.Invoke(biome);
        }

        /// <summary>
        /// Invoca o evento de saída de bioma
        /// </summary>
        public static void TriggerBiomeExited(BiomeType biome)
        {
            OnBiomeExited?.Invoke(biome);
        }

        /// <summary>
        /// Invoca o evento de jogo pausado
        /// </summary>
        public static void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        /// <summary>
        /// Invoca o evento de jogo despausado
        /// </summary>
        public static void TriggerGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        /// <summary>
        /// Invoca o evento de novo dia
        /// </summary>
        public static void TriggerNewDay(int dayNumber)
        {
            OnNewDay?.Invoke(dayNumber);
        }

        /// <summary>
        /// Invoca o evento de mudança de período do dia
        /// </summary>
        public static void TriggerTimeOfDayChanged(TimeOfDay timeOfDay)
        {
            OnTimeOfDayChanged?.Invoke(timeOfDay);
        }

        /// <summary>
        /// Invoca o evento de ganho de XP elemental
        /// </summary>
        public static void TriggerElementalXPGained(ElementType element, int xp)
        {
            OnElementalXPGained?.Invoke(element, xp);
        }

        /// <summary>
        /// Invoca o evento de cena carregada
        /// </summary>
        public static void TriggerSceneLoaded(string sceneName)
        {
            OnSceneLoaded?.Invoke(sceneName);
        }

        #endregion
    }
}