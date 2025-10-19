using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Classe base abstrata para todos os Managers Singleton do projeto.
    /// Garante que apenas uma instância exista e persista entre cenas.
    /// </summary>
    /// <typeparam name="T">Tipo do Manager que herda desta classe</typeparam>
    public abstract class ManagerSingleton<T> : MonoBehaviour where T : ManagerSingleton<T>
    {
        #region Singleton Properties

        /// <summary>
        /// Instância única do Manager
        /// </summary>
        public static T Instance { get; private set; }

        /// <summary>
        /// Verifica se existe uma instância válida do Manager
        /// </summary>
        public static bool HasInstance => Instance != null;

        #endregion

        #region Inspector Settings

        [Header("Manager Base Settings")]
        [SerializeField] protected bool enableDebugLogs = false;
        [SerializeField] protected bool persistBetweenScenes = true;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            // Implementação do padrão Singleton
            if (Instance == null)
            {
                Instance = (T)this;

                // Persistir entre cenas se necessário
                if (persistBetweenScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }

                // Chamar inicialização específica do Manager
                Initialize();

                Log($"{typeof(T).Name} initialized successfully");
            }
            else if (Instance != this)
            {
                Log($"Duplicate {typeof(T).Name} detected. Destroying duplicate instance.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Log($"{typeof(T).Name} is being destroyed");
                OnManagerDestroy();
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Método chamado na inicialização do Manager.
        /// Implementar toda lógica de setup específica aqui.
        /// </summary>
        protected abstract void Initialize();

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Método chamado quando o Manager está sendo destruído.
        /// Override para fazer cleanup específico.
        /// </summary>
        protected virtual void OnManagerDestroy() { }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Sistema de log centralizado para todos os Managers.
        /// Só funciona se enableDebugLogs estiver ativo.
        /// </summary>
        /// <param name="message">Mensagem a ser logada</param>
        protected void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{typeof(T).Name}] {message}");
            }
        }

        /// <summary>
        /// Log de warning centralizado para todos os Managers.
        /// </summary>
        /// <param name="message">Mensagem de warning</param>
        protected void LogWarning(string message)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"[{typeof(T).Name}] {message}");
            }
        }

        /// <summary>
        /// Log de erro centralizado para todos os Managers.
        /// </summary>
        /// <param name="message">Mensagem de erro</param>
        protected void LogError(string message)
        {
            Debug.LogError($"[{typeof(T).Name}] {message}");
        }

        #endregion
    }
}