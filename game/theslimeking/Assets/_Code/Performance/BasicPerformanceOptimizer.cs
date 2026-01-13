using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SlimeKing.Performance
{
    /// <summary>
    /// Basic performance optimizer that applies simple optimizations to the InitialCave scene.
    /// This script focuses on the most impactful optimizations without complex dependencies.
    /// </summary>
    public class BasicPerformanceOptimizer : MonoBehaviour
    {
        [Header("Optimization Settings")]
        [SerializeField] private bool enablePhysicsOptimization = true;
        [SerializeField] private bool enableCullingOptimization = true;
        [SerializeField] private float optimizationInterval = 1f;
        [SerializeField] private float cullingDistance = 20f;
        [SerializeField] private float physicsDistance = 15f;
        
        [Header("Debug")]
        [SerializeField] private bool showLogs = true;

        private Transform playerTransform;
        private List<GameObject> destructibleObjects = new List<GameObject>();
        private List<Rigidbody2D> rigidbodies = new List<Rigidbody2D>();
        private Coroutine optimizationCoroutine;

        private void Start()
        {
            InitializeOptimizer();
        }

        private void InitializeOptimizer()
        {
            // Find player
            var player = FindObjectOfType<SlimeKing.Gameplay.PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }

            // Find all destructible objects
            GameObject[] destructibles = GameObject.FindGameObjectsWithTag("Destructable");
            destructibleObjects.AddRange(destructibles);

            // Cache rigidbodies
            foreach (GameObject obj in destructibleObjects)
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rigidbodies.Add(rb);
                }
            }

            // Start optimization coroutine
            optimizationCoroutine = StartCoroutine(OptimizationLoop());
        }

        private IEnumerator OptimizationLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(optimizationInterval);

                if (playerTransform != null)
                {
                    OptimizeObjects();
                }
            }
        }

        private void OptimizeObjects()
        {
            Vector3 playerPos = playerTransform.position;
            int culledCount = 0;
            int physicsDisabledCount = 0;

            for (int i = 0; i < destructibleObjects.Count; i++)
            {
                GameObject obj = destructibleObjects[i];
                if (obj == null) continue;

                float distance = Vector3.Distance(playerPos, obj.transform.position);

                // Culling optimization
                if (enableCullingOptimization)
                {
                    bool shouldBeActive = distance <= cullingDistance;
                    if (obj.activeInHierarchy != shouldBeActive)
                    {
                        obj.SetActive(shouldBeActive);
                        if (!shouldBeActive) culledCount++;
                    }
                }

                // Physics optimization
                if (enablePhysicsOptimization && i < rigidbodies.Count)
                {
                    Rigidbody2D rb = rigidbodies[i];
                    if (rb != null)
                    {
                        bool shouldBeEnabled = distance <= physicsDistance;
                        if (rb.simulated != shouldBeEnabled)
                        {
                            rb.simulated = shouldBeEnabled;
                            if (!shouldBeEnabled) physicsDisabledCount++;
                        }
                    }
                }
            }

        }

        private void OnDestroy()
        {
            if (optimizationCoroutine != null)
            {
                StopCoroutine(optimizationCoroutine);
            }
        }

        [ContextMenu("Force Optimization")]
        public void ForceOptimization()
        {
            if (playerTransform != null)
            {
                OptimizeObjects();
            }
        }

        [ContextMenu("Reset All Objects")]
        public void ResetAllObjects()
        {
            // Reactivate all objects
            foreach (GameObject obj in destructibleObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }

            // Re-enable all rigidbodies
            foreach (Rigidbody2D rb in rigidbodies)
            {
                if (rb != null)
                {
                    rb.simulated = true;
                }
            }

        }
    }
}