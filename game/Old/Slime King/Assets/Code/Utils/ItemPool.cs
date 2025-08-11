using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Core.Utils
{
    /// <summary>
    /// Singleton pool manager for game items to reduce instantiation/destruction overhead
    /// </summary>
    public class ItemPool : MonoBehaviour
    {
        private static ItemPool instance;
        public static ItemPool Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("ItemPool");
                    instance = go.AddComponent<ItemPool>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private Dictionary<GameObject, Queue<GameObject>> pools;
        private Dictionary<GameObject, Transform> poolContainers;
        private const int DEFAULT_POOL_SIZE = 10;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializePools()
        {
            pools = new Dictionary<GameObject, Queue<GameObject>>();
            poolContainers = new Dictionary<GameObject, Transform>();
        }

        public GameObject GetItem(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            // Create pool for this prefab if it doesn't exist
            if (!pools.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            // Get or create item
            GameObject item = pools[prefab].Count > 0 ?
                pools[prefab].Dequeue() :
                CreateNewItem(prefab);

            // Activate and position item
            item.transform.position = position;
            item.transform.rotation = rotation;
            item.SetActive(true);

            return item;
        }

        public void ReturnToPool(GameObject item, GameObject prefab)
        {
            if (!pools.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            // Deactivate and reset item
            item.SetActive(false);
            item.transform.SetParent(poolContainers[prefab]);

            // Return to pool
            pools[prefab].Enqueue(item);
        }

        private void CreatePool(GameObject prefab)
        {
            // Create container
            GameObject container = new GameObject($"Pool_{prefab.name}");
            container.transform.SetParent(transform);
            poolContainers[prefab] = container.transform;

            // Initialize queue
            pools[prefab] = new Queue<GameObject>();

            // Pre-instantiate items
            for (int i = 0; i < DEFAULT_POOL_SIZE; i++)
            {
                GameObject item = CreateNewItem(prefab);
                item.SetActive(false);
                pools[prefab].Enqueue(item);
            }
        }

        private GameObject CreateNewItem(GameObject prefab)
        {
            GameObject item = Instantiate(prefab);
            item.transform.SetParent(poolContainers[prefab]);
            return item;
        }
    }
}
