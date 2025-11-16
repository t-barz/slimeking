using UnityEngine;

namespace SlimeKing.Debug
{
    /// <summary>
    /// Sistema de logging estático para SlimeKing.
    /// Compatível com o padrão usado no projeto: SlimeKing.Debug.Log(), etc.
    /// </summary>
    public static class Debug
    {
        #region Settings
        private static bool enableDebugLogs = true;
        private static bool enableWarnings = true;
        private static bool enableErrors = true;
        #endregion

        #region Public Logging Methods
        /// <summary>
        /// Log básico - SlimeKing.Debug.Debug.Log(message)
        /// Chamado pelos scripts como SlimeKing.Debug.Log()
        /// </summary>
        public static void Log(string message, Object context = null)
        {
            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log(message, context);
            }
        }

        /// <summary>
        /// Warning - SlimeKing.Debug.Debug.LogWarning(message)
        /// Chamado pelos scripts como SlimeKing.Debug.LogWarning()
        /// </summary>
        public static void LogWarning(string message, Object context = null)
        {
            if (enableWarnings)
            {
                UnityEngine.Debug.LogWarning(message, context);
            }
        }

        /// <summary>
        /// Error - SlimeKing.Debug.Debug.LogError(message)
        /// Chamado pelos scripts como SlimeKing.Debug.LogError()
        /// </summary>
        public static void LogError(string message, Object context = null)
        {
            if (enableErrors)
            {
                UnityEngine.Debug.LogError(message, context);
            }
        }
        #endregion

        #region Configuration
        /// <summary>
        /// Controla logs de debug globalmente
        /// </summary>
        public static void SetDebugEnabled(bool enabled)
        {
            enableDebugLogs = enabled;
            UnityEngine.Debug.Log($"[SlimeKing.Debug] Logs de debug {(enabled ? "habilitados" : "desabilitados")}");
        }

        /// <summary>
        /// Controla warnings globalmente
        /// </summary>
        public static void SetWarningsEnabled(bool enabled)
        {
            enableWarnings = enabled;
            UnityEngine.Debug.Log($"[SlimeKing.Debug] Warnings {(enabled ? "habilitados" : "desabilitados")}");
        }

        /// <summary>
        /// Controla errors globalmente
        /// </summary>
        public static void SetErrorsEnabled(bool enabled)
        {
            enableErrors = enabled;
            UnityEngine.Debug.Log($"[SlimeKing.Debug] Errors {(enabled ? "habilitados" : "desabilitados")}");
        }

        /// <summary>
        /// Status atual das configurações
        /// </summary>
        public static string GetStatus()
        {
            return $"Debug: {enableDebugLogs}, Warnings: {enableWarnings}, Errors: {enableErrors}";
        }
        #endregion
    }

    /// <summary>
    /// Classe auxiliar para compatibilidade com uso direto de Log
    /// Alguns scripts podem tentar usar SlimeKing.Debug.Log.Something()
    /// </summary>
    public static class Log
    {
        public static void Message(string message, Object context = null)
        {
            Debug.Log(message, context);
        }
    }

    /// <summary>
    /// Classe auxiliar para compatibilidade com uso direto de LogWarning
    /// </summary>
    public static class LogWarning
    {
        public static void Message(string message, Object context = null)
        {
            Debug.LogWarning(message, context);
        }
    }

    /// <summary>
    /// Classe auxiliar para compatibilidade com uso direto de LogError
    /// </summary>
    public static class LogError
    {
        public static void Message(string message, Object context = null)
        {
            Debug.LogError(message, context);
        }
    }
}