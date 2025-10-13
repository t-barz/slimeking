using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Classe base para todos os Managers do jogo.
    /// Implementa o padrão Singleton e funcionalidades comuns.
    /// </summary>
    public abstract class ManagerBase<T> : MonoBehaviour where T : ManagerBase<T>
    {
        public static T Instance { get; private set; }

        [Header("Manager Settings")]
        [SerializeField] protected bool enableDebugLogs = false;
        [SerializeField] protected bool persistBetweenScenes = true;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T)this;

                if (persistBetweenScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }

                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Método chamado durante a inicialização do Manager.
        /// Deve ser implementado pelos Managers filhos.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Log com prefixo do Manager
        /// </summary>
        protected virtual void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[{typeof(T).Name}] {message}");
            }
        }

        /// <summary>
        /// Log de erro com prefixo do Manager
        /// </summary>
        protected virtual void LogError(string message)
        {
            Debug.LogError($"[{typeof(T).Name}] {message}");
        }

        /// <summary>
        /// Log de warning com prefixo do Manager
        /// </summary>
        protected virtual void LogWarning(string message)
        {
            Debug.LogWarning($"[{typeof(T).Name}] {message}");
        }
    }
}