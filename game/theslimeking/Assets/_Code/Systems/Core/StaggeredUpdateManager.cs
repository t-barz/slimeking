using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Systems.Core
{
    /// <summary>
    /// Gerenciador que distribui updates ao longo de múltiplos frames.
    /// Reduz carga de CPU distribuindo trabalho ao invés de processar tudo em um frame.
    /// 
    /// PERFORMANCE: Distribui updates pesados ao longo de frames
    /// USO: Ideal para AI, pathfinding, checks de distância, etc.
    /// </summary>
    public class StaggeredUpdateManager : MonoBehaviour
    {
        private static StaggeredUpdateManager instance;
        public static StaggeredUpdateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = new GameObject("StaggeredUpdateManager");
                    instance = obj.AddComponent<StaggeredUpdateManager>();
                    DontDestroyOnLoad(obj);
                }
                return instance;
            }
        }
        
        [Header("Configuration")]
        [SerializeField] private int updatesPerFrame = 10;
        [SerializeField] private bool showDebug = false;
        
        private List<IStaggeredUpdate> registeredObjects = new List<IStaggeredUpdate>();
        private int currentIndex = 0;
        
        // Stats
        private int totalRegistered = 0;
        private int updatedThisFrame = 0;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Update()
        {
            updatedThisFrame = 0;
            
            if (registeredObjects.Count == 0) return;
            
            int updated = 0;
            while (updated < updatesPerFrame && registeredObjects.Count > 0)
            {
                // Wrap around
                if (currentIndex >= registeredObjects.Count)
                {
                    currentIndex = 0;
                }
                
                if (currentIndex < registeredObjects.Count)
                {
                    IStaggeredUpdate obj = registeredObjects[currentIndex];
                    
                    // Remove se null
                    if (obj == null || (obj is MonoBehaviour mb && mb == null))
                    {
                        registeredObjects.RemoveAt(currentIndex);
                        continue;
                    }
                    
                    obj.StaggeredUpdate();
                    currentIndex++;
                    updated++;
                    updatedThisFrame++;
                }
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Registra um objeto para receber updates escalonados
        /// </summary>
        public void Register(IStaggeredUpdate obj)
        {
            if (obj == null) return;
            
            if (!registeredObjects.Contains(obj))
            {
                registeredObjects.Add(obj);
                totalRegistered++;
                
                if (showDebug)
                {
                    UnityEngine.Debug.Log($"[StaggeredUpdateManager] Registrado: {obj}. Total: {registeredObjects.Count}");
                }
            }
        }
        
        /// <summary>
        /// Remove um objeto do sistema de updates escalonados
        /// </summary>
        public void Unregister(IStaggeredUpdate obj)
        {
            if (obj == null) return;
            
            int index = registeredObjects.IndexOf(obj);
            if (index >= 0)
            {
                registeredObjects.RemoveAt(index);
                
                // Ajusta índice se necessário
                if (currentIndex > index)
                {
                    currentIndex--;
                }
                
                if (showDebug)
                {
                    UnityEngine.Debug.Log($"[StaggeredUpdateManager] Removido: {obj}. Total: {registeredObjects.Count}");
                }
            }
        }
        
        /// <summary>
        /// Limpa todos os objetos registrados
        /// </summary>
        public void Clear()
        {
            registeredObjects.Clear();
            currentIndex = 0;
        }
        
        /// <summary>
        /// Retorna estatísticas do sistema
        /// </summary>
        public StaggeredStats GetStats()
        {
            return new StaggeredStats
            {
                totalRegistered = registeredObjects.Count,
                updatesPerFrame = updatesPerFrame,
                updatedThisFrame = updatedThisFrame,
                currentIndex = currentIndex
            };
        }
        
        #endregion
        
        #region Debug
        
        private void OnGUI()
        {
            if (!showDebug) return;
            
            StaggeredStats stats = GetStats();
            GUILayout.BeginArea(new Rect(10, 350, 300, 120));
            GUILayout.Label($"<b>Staggered Update Manager</b>");
            GUILayout.Label($"Registered: {stats.totalRegistered}");
            GUILayout.Label($"Updates/Frame: {stats.updatesPerFrame}");
            GUILayout.Label($"Updated This Frame: {stats.updatedThisFrame}");
            GUILayout.Label($"Current Index: {stats.currentIndex}");
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Data Structures
        
        public struct StaggeredStats
        {
            public int totalRegistered;
            public int updatesPerFrame;
            public int updatedThisFrame;
            public int currentIndex;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Interface para objetos que usam update escalonado
    /// </summary>
    public interface IStaggeredUpdate
    {
        /// <summary>
        /// Chamado pelo StaggeredUpdateManager ao invés de Update()
        /// </summary>
        void StaggeredUpdate();
    }
}
