using System;

namespace SlimeKing.Core
{
    /// <summary>
    /// Eventos estáticos relacionados ao sistema de pause do jogo.
    /// Permite comunicação desacoplada entre PauseManager e outros sistemas.
    /// </summary>
    public static class PauseEvents
    {
        /// <summary>
        /// Disparado quando o jogo é pausado.
        /// </summary>
        public static event Action OnGamePaused;

        /// <summary>
        /// Disparado quando o jogo é despausado (resume).
        /// </summary>
        public static event Action OnGameResumed;

        /// <summary>
        /// Disparado quando o pause stack counter muda.
        /// Parâmetro: novo valor do pauseRefCount.
        /// </summary>
        public static event Action<int> OnPauseStackChanged;

        /// <summary>
        /// Invoca o evento OnGamePaused.
        /// Chamado internamente pelo PauseManager.
        /// </summary>
        internal static void InvokeGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        /// <summary>
        /// Invoca o evento OnGameResumed.
        /// Chamado internamente pelo PauseManager.
        /// </summary>
        internal static void InvokeGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        /// <summary>
        /// Invoca o evento OnPauseStackChanged.
        /// Chamado internamente pelo PauseManager.
        /// </summary>
        internal static void InvokePauseStackChanged(int newCount)
        {
            OnPauseStackChanged?.Invoke(newCount);
        }
    }
}
