using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Systems.Core
{
    /// <summary>
    /// Sistema genérico de Object Pooling para reutilização de GameObjects.
    /// Evita Instantiate/Destroy frequentes que causam garbage collection.
    /// 
    /// PERFORMANCE: Reduz alocações e GC spikes
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        [Header("Pool Configuration")]
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 10;
        [SerializeField] private int maxSize = 50;
        [SerializeField] private bool expandIfNeeded = true;
        
        [Header("Debug")]
        [SerializeField] private bool showDebug = false;
        
        private Queue<GameObject> availableObjects = new Queue<GameObject>();
        private HashSet<GameObject> activeObjects = new HashSet<GameObject>();
        private Transform poolContainer;
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            CreatePoolContainer();
            InitializePool();
        }
        
        #endregion
        
        #region Initialization
        
        private void CreatePoolContainer()
        {
            poolContainer = new GameObject($"Pool_{prefab.name}").transform;
            poolContainer.SetParent(transform);
        }
        
        private void InitializePool()
        {
            for (int i = 0; i < initialSize; i++)
            {
                CreateNewObject();
            }
            
            if (showDebug)
            {
                Debug.Log($"[ObjectPool] Pool '{prefab.name}' inicializado com {initialSize} objetos");
            }
        }
        
        private GameObject CreateNewObject()
        {
            GameObject obj = Instantiate(prefab, poolContainer);
            obj.SetActive(false);
            availableObjects.Enqueue(obj);
            return obj;
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Obtém um objeto do pool
        /// </summary>
        public GameObject Get()
        {
            GameObject obj;
            
            if (availableObjects.Count > 0)
            {
                obj = availableObjects.Dequeue();
            }
            else if (expandIfNeeded && activeObjects.Count < maxSize)
            {
                obj = CreateNewObject();
                if (showDebug)
                {
                    Debug.Log($"[ObjectPool] Pool '{prefab.name}' expandido. Total: {activeObjects.Count + availableObjects.Count + 1}");
                }
            }
            else
            {
                if (showDebug)
                {
                    Debug.LogWarning($"[ObjectPool] Pool '{prefab.name}' esgotado! Criando objeto temporário.");
                }
                return Instantiate(prefab);
            }
            
            obj.SetActive(true);
            activeObjects.Add(obj);
            return obj;
        }
        
        /// <summary>
        /// Obtém um objeto do pool em uma posição específica
        /// </summary>
        public GameObject Get(Vector3 position)
        {
            GameObject obj = Get();
            obj.transform.position = position;
            return obj;
        }
        
        /// <summary>
        /// Obtém um objeto do pool em uma posição e rotação específicas
        /// </summary>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj = Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            return obj;
        }
        
        /// <summary>
        /// Retorna um objeto ao pool
        /// </summary>
        public void Return(GameObject obj)
        {
            if (obj == null) return;
            
            // Se não é do pool, apenas destroi
            if (!activeObjects.Contains(obj))
            {
                Destroy(obj);
                return;
            }
            
            obj.SetActive(false);
            obj.transform.SetParent(poolContainer);
            activeObjects.Remove(obj);
            availableObjects.Enqueue(obj);
        }
        
        /// <summary>
        /// Retorna um objeto ao pool após um delay
        /// </summary>
        public void ReturnDelayed(GameObject obj, float delay)
        {
            if (obj != null)
            {
                StartCoroutine(ReturnDelayedCoroutine(obj, delay));
            }
        }
        
        private System.Collections.IEnumerator ReturnDelayedCoroutine(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Return(obj);
        }
        
        /// <summary>
        /// Limpa todos os objetos do pool
        /// </summary>
        public void Clear()
        {
            foreach (GameObject obj in activeObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            
            while (availableObjects.Count > 0)
            {
                GameObject obj = availableObjects.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            
            activeObjects.Clear();
            availableObjects.Clear();
        }
        
        /// <summary>
        /// Retorna estatísticas do pool
        /// </summary>
        public PoolStats GetStats()
        {
            return new PoolStats
            {
                prefabName = prefab.name,
                totalObjects = activeObjects.Count + availableObjects.Count,
                activeObjects = activeObjects.Count,
                availableObjects = availableObjects.Count,
                maxSize = maxSize
            };
        }
        
        #endregion
        
        #region Debug
        
        private void OnGUI()
        {
            if (!showDebug) return;
            
            PoolStats stats = GetStats();
            GUILayout.BeginArea(new Rect(10, 250, 300, 100));
            GUILayout.Label($"<b>Pool: {stats.prefabName}</b>");
            GUILayout.Label($"Total: {stats.totalObjects} | Active: {stats.activeObjects}");
            GUILayout.Label($"Available: {stats.availableObjects} | Max: {stats.maxSize}");
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Data Structures
        
        public struct PoolStats
        {
            public string prefabName;
            public int totalObjects;
            public int activeObjects;
            public int availableObjects;
            public int maxSize;
        }
        
        #endregion
    }
}
