using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameObjectSprayTool.Services
{
    /// <summary>
    /// Service responsible for coordinating the placement of GameObjects in the scene.
    /// Integrates RandomDistributionService and HierarchyOrganizer to spray multiple GameObjects
    /// with proper Undo support.
    /// </summary>
    public class PlacementService
    {
        private readonly RandomDistributionService distributionService;
        private readonly HierarchyOrganizer hierarchyOrganizer;

        public PlacementService()
        {
            distributionService = new RandomDistributionService();
            hierarchyOrganizer = new HierarchyOrganizer();
        }

        /// <summary>
        /// Sprays GameObjects at random positions within a circular area.
        /// </summary>
        /// <param name="centerPosition">The center position where objects will be sprayed</param>
        /// <param name="prefabSlots">Array of prefab slots (can contain null entries)</param>
        /// <param name="radius">The radius of the spray area</param>
        /// <param name="density">The number of GameObjects to instantiate</param>
        /// <param name="minSpacing">The minimum spacing between GameObjects</param>
        public void SprayGameObjects(
            Vector3 centerPosition,
            GameObject[] prefabSlots,
            float radius,
            float density,
            float minSpacing)
        {
            // Validate parameters
            if (radius <= 0)
            {
                UnityEngine.Debug.LogWarning("Invalid brush radius. Radius must be greater than 0.");
                return;
            }

            if (density <= 0)
            {
                UnityEngine.Debug.LogWarning("Invalid density. Density must be greater than 0.");
                return;
            }

            // Validate that we have at least one valid prefab
            if (!HasValidPrefabs(prefabSlots))
            {
                UnityEngine.Debug.LogWarning("No valid prefabs available for spraying.");
                return;
            }

            // Get or create the parent GameObject for organization
            Transform parent = hierarchyOrganizer.GetOrCreateParentObject();

            // Validate parent was successfully created/retrieved
            if (parent == null)
            {
                UnityEngine.Debug.LogWarning("Failed to create or retrieve parent GameObject for organization.");
                return;
            }

            // Generate random positions within the radius
            // Convert density to int for position generation
            int densityCount = Mathf.RoundToInt(density);
            List<Vector3> positions = distributionService.GenerateRandomPositions(
                centerPosition,
                radius,
                densityCount,
                minSpacing
            );

            // Group all instantiation operations into a single Undo group
            Undo.SetCurrentGroupName("Spray GameObjects");
            int undoGroup = Undo.GetCurrentGroup();

            // Instantiate GameObjects at each position
            foreach (Vector3 position in positions)
            {
                GameObject selectedPrefab = SelectRandomPrefab(prefabSlots);
                if (selectedPrefab != null)
                {
                    InstantiateWithUndo(selectedPrefab, position, parent);
                }
            }

            // Collapse the Undo group
            Undo.CollapseUndoOperations(undoGroup);
        }

        /// <summary>
        /// Checks if the prefab slots array contains at least one valid (non-null) prefab.
        /// </summary>
        /// <param name="prefabSlots">Array of prefab slots to validate</param>
        /// <returns>True if at least one valid prefab exists</returns>
        private bool HasValidPrefabs(GameObject[] prefabSlots)
        {
            if (prefabSlots == null || prefabSlots.Length == 0)
            {
                return false;
            }

            foreach (GameObject prefab in prefabSlots)
            {
                if (prefab != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Selects a random prefab from the non-empty slots.
        /// </summary>
        /// <param name="prefabSlots">Array of prefab slots (can contain null entries)</param>
        /// <returns>A randomly selected non-null prefab, or null if none available</returns>
        private GameObject SelectRandomPrefab(GameObject[] prefabSlots)
        {
            // Build list of valid (non-null) prefabs
            List<GameObject> validPrefabs = new List<GameObject>();
            foreach (GameObject prefab in prefabSlots)
            {
                if (prefab != null)
                {
                    validPrefabs.Add(prefab);
                }
            }

            // Return null if no valid prefabs
            if (validPrefabs.Count == 0)
            {
                return null;
            }

            // Select random prefab from valid ones
            int randomIndex = Random.Range(0, validPrefabs.Count);
            return validPrefabs[randomIndex];
        }

        /// <summary>
        /// Instantiates a GameObject with Undo support.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate</param>
        /// <param name="position">The world position for the instantiated object</param>
        /// <param name="parent">The parent transform for hierarchy organization</param>
        private void InstantiateWithUndo(GameObject prefab, Vector3 position, Transform parent)
        {
            // Use PrefabUtility.InstantiatePrefab to maintain prefab connection
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            
            if (instance != null)
            {
                // Set position and parent
                instance.transform.position = position;
                instance.transform.SetParent(parent, true); // worldPositionStays = true
                
                // Register with Undo system
                Undo.RegisterCreatedObjectUndo(instance, "Spray GameObject");
            }
        }
    }
}
