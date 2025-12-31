using System;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Eventos globais do Save System.
    /// Permite que sistemas se registrem para salvar/carregar dados automaticamente.
    /// </summary>
    public static class SaveEvents
    {
        #region Save/Load Events
        /// <summary>
        /// Disparado quando o jogo está salvando.
        /// Sistemas devem retornar seus dados de save.
        /// </summary>
        public static event Func<object> OnGameSaving;
        
        /// <summary>
        /// Disparado quando o jogo está carregando.
        /// Sistemas devem restaurar seus dados a partir do save.
        /// </summary>
        public static event Action<object> OnGameLoading;
        #endregion
        
        #region Helper Methods
        /// <summary>
        /// Dispara evento de salvamento e coleta dados de todos os sistemas.
        /// </summary>
        /// <returns>Dados coletados de todos os sistemas</returns>
        public static object TriggerGameSaving()
        {
            return OnGameSaving?.Invoke();
        }
        
        /// <summary>
        /// Dispara evento de carregamento e distribui dados para todos os sistemas.
        /// </summary>
        /// <param name="saveData">Dados a serem carregados</param>
        public static void TriggerGameLoading(object saveData)
        {
            OnGameLoading?.Invoke(saveData);
        }
        #endregion
    }
}
