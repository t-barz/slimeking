using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Sistema centralizado de logging para o projeto SlimeKing.
    /// Fornece métodos estáticos para log controlado por flags.
    /// </summary>
    public static class LogSystem
    {
        private static bool _isInitialized = false;

        /// <summary>
        /// Inicializa o sistema de logging.
        /// </summary>
        public static void Initialize()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                UnityEngine.Debug.Log("[LogSystem] Sistema de logging inicializado");
            }
        }

        /// <summary>
        /// Log normal com controle opcional por flag.
        /// </summary>
        /// <param name="message">Mensagem a ser logada</param>
        /// <param name="enableLog">Flag opcional para controlar se deve logar</param>
        public static void Log(string message, bool enableLog = true)
        {
            if (enableLog)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        /// <summary>
        /// Log de warning com controle opcional por flag.
        /// </summary>
        /// <param name="message">Mensagem a ser logada</param>
        /// <param name="enableLog">Flag opcional para controlar se deve logar</param>
        public static void LogWarning(string message, bool enableLog = true)
        {
            if (enableLog)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        /// <summary>
        /// Log de error com controle opcional por flag.
        /// </summary>
        /// <param name="message">Mensagem a ser logada</param>
        /// <param name="enableLog">Flag opcional para controlar se deve logar</param>
        public static void LogError(string message, bool enableLog = true)
        {
            if (enableLog)
            {
                UnityEngine.Debug.LogError(message);
            }
        }
    }
}