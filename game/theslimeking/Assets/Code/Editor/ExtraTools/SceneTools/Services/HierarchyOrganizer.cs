using UnityEngine;

namespace GameObjectSprayTool.Services
{
    /// <summary>
    /// Manages the hierarchical organization of sprayed GameObjects in the scene.
    /// Creates and maintains a parent GameObject named "SprayedObjects" to keep the hierarchy organized.
    /// </summary>
    public class HierarchyOrganizer
    {
        private const string PARENT_OBJECT_NAME = "SprayedObjects";
        private Transform cachedParent;

        /// <summary>
        /// Gets the parent GameObject for sprayed objects, creating it if it doesn't exist.
        /// Uses caching for performance optimization with validation.
        /// </summary>
        /// <returns>Transform of the parent GameObject positioned at world origin</returns>
        public Transform GetOrCreateParentObject()
        {
            // Validate cached parent before using it (could have been deleted)
            if (cachedParent != null && cachedParent.gameObject != null)
            {
                return cachedParent;
            }

            // Clear invalid cache
            if (cachedParent != null && cachedParent.gameObject == null)
            {
                UnityEngine.Debug.LogWarning("Cached parent GameObject was destroyed. Creating new parent.");
                cachedParent = null;
            }

            // Try to find existing parent in the scene
            Transform existingParent = FindExistingParent();
            if (existingParent != null)
            {
                cachedParent = existingParent;
                return cachedParent;
            }

            // Create new parent if none exists
            cachedParent = CreateNewParent();
            return cachedParent;
        }

        /// <summary>
        /// Searches for an existing "SprayedObjects" GameObject in the scene.
        /// </summary>
        /// <returns>Transform of existing parent GameObject, or null if not found</returns>
        private Transform FindExistingParent()
        {
            GameObject existingParent = GameObject.Find(PARENT_OBJECT_NAME);
            return existingParent != null ? existingParent.transform : null;
        }

        /// <summary>
        /// Creates a new parent GameObject named "SprayedObjects" at world origin.
        /// </summary>
        /// <returns>Transform of the newly created parent GameObject</returns>
        private Transform CreateNewParent()
        {
            GameObject parentObject = new GameObject(PARENT_OBJECT_NAME);
            parentObject.transform.position = Vector3.zero;
            return parentObject.transform;
        }
    }
}
