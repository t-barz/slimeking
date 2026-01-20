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

        private static T _instance;

        /// <summary>
        /// Instância única do Manager
        /// </summary>
        public static T Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    // Tenta encontrar uma instância existente na cena
                    // Usa FindObjectsOfType para encontrar todas as instâncias e pegar a primeira válida
                    T[] instances = FindObjectsOfType<T>();
                    
                    if (instances != null && instances.Length > 0)
                    {
                        _instance = instances[0];
                        
                        // Se houver mais de uma instância, destroi as duplicatas
                        if (instances.Length > 1)
                        {
                            UnityEngine.Debug.LogWarning($"[{typeof(T).Name}] Múltiplas instâncias encontradas ({instances.Length}). Mantendo apenas a primeira.");
                            for (int i = 1; i < instances.Length; i++)
                            {
                                Destroy(instances[i].gameObject);
                            }
                        }
                    }
                    
                    // Se não encontrou, cria uma nova instância
                    if (_instance == null)
                    {
                        GameObject go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                        
                        // Log da criação automática
                        UnityEngine.Debug.Log($"[{typeof(T).Name}] Instância criada automaticamente");
                    }
                }
                return _instance;
            }
            private set => _instance = value;
        }

        /// <summary>
        /// Verifica se existe uma instância válida do Manager
        /// </summary>
        public static bool HasInstance => _instance != null;

        #endregion

        #region Inspector Settings

        [Header("Manager Base Settings")]
        [SerializeField] protected bool persistBetweenScenes = true;

        [Header("Configurações de Debug")]
        [SerializeField] protected bool enableLogs = false;

        #endregion

        #region Unity Lifecycle

        protected virtual void Awake()
        {
            // Initialize the logging system first
            LogSystem.Initialize();

            // Implementação do padrão Singleton
            if (_instance == null)
            {
                _instance = (T)this;

                // Persistir entre cenas se necessário
                if (persistBetweenScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }

                // Chamar inicialização específica do Manager
                Initialize();

                Log($"initialized successfully");
            }
            else if (_instance != this)
            {
                Log($"Duplicate detected. Destroying duplicate instance.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                Log($"is being destroyed");
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

        #region Protected Logging Methods

        /// <summary>
        /// Log normal controlado pela flag enableLogs do Manager.
        /// </summary>
        /// <param name="message">Mensagem a ser logada</param>
        protected void Log(string message)
        {
            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[{typeof(T).Name}] {message}");
            }
        }

        /// <summary>
        /// Log de warning controlado pela flag enableLogs do Manager.
        /// </summary>
        /// <param name="message">Mensagem a ser logada</param>
        protected void LogWarning(string message)
        {
            if (enableLogs)
            {
                UnityEngine.Debug.LogWarning($"[{typeof(T).Name}] {message}");
            }
        }

        /// <summary>
        /// Log de error - sempre ativo, independente da flag enableLogs.
        /// </summary>
        /// <param name="message">Mensagem a ser logada</param>
        protected void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[{typeof(T).Name}] {message}");
        }

        #endregion
    }
}