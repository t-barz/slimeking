using UnityEngine;
using System.Collections.Generic;

namespace SlimeKing.Utils
{
    /// <summary>
    /// Pool manager for visual effects and particle systems
    /// </summary>
    public class EffectPool : MonoBehaviour
    {
        private static EffectPool instance;
        public static EffectPool Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("EffectPool");
                    instance = go.AddComponent<EffectPool>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private Dictionary<GameObject, Queue<ParticleSystem>> pools;
        private Dictionary<GameObject, Transform> poolContainers;
        private const int DEFAULT_POOL_SIZE = 5;

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
            pools = new Dictionary<GameObject, Queue<ParticleSystem>>();
            poolContainers = new Dictionary<GameObject, Transform>();
        }

        public ParticleSystem GetEffect(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            // Create pool for this prefab if it doesn't exist
            if (!pools.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            // Get or create effect
            ParticleSystem effect = pools[prefab].Count > 0 ?
                pools[prefab].Dequeue() :
                CreateNewEffect(prefab);

            // Activate and position effect
            Transform effectTransform = effect.transform;
            effectTransform.position = position;
            effectTransform.rotation = rotation;
            effect.gameObject.SetActive(true);

            // Play the effect
            effect.Clear();
            effect.Play();

            return effect;
        }

        public void ReturnToPool(ParticleSystem effect, GameObject prefab)
        {
            if (!pools.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            // Stop and deactivate effect
            effect.Stop();
            effect.gameObject.SetActive(false);
            effect.transform.SetParent(poolContainers[prefab]);

            // Return to pool
            pools[prefab].Enqueue(effect);
        }

        private void CreatePool(GameObject prefab)
        {
            // Create container
            GameObject container = new GameObject($"Pool_{prefab.name}");
            container.transform.SetParent(transform);
            poolContainers[prefab] = container.transform;

            // Initialize queue
            pools[prefab] = new Queue<ParticleSystem>();

            // Pre-instantiate effects
            for (int i = 0; i < DEFAULT_POOL_SIZE; i++)
            {
                ParticleSystem effect = CreateNewEffect(prefab);
                effect.gameObject.SetActive(false);
                pools[prefab].Enqueue(effect);
            }
        }

        private ParticleSystem CreateNewEffect(GameObject prefab)
        {
            GameObject instance = Instantiate(prefab);
            instance.transform.SetParent(poolContainers[prefab]);
            return instance.GetComponent<ParticleSystem>();
        }
    }
}
