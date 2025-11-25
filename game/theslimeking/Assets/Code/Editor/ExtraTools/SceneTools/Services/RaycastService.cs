using UnityEngine;

namespace GameObjectSprayTool.Services
{
    /// <summary>
    /// Service responsible for detecting surfaces in the scene using raycasts
    /// and determining placement positions for GameObjects.
    /// </summary>
    public class RaycastService
    {
        private const float DEFAULT_PLACEMENT_DISTANCE = 10f;
        private const string WARNING_NO_SURFACE = "Nenhuma superfície detectada. Objetos serão posicionados a 10 unidades da câmera.";
        private bool hasLoggedNoSurfaceWarning = false;
        
        /// <summary>
        /// Gets the placement position for GameObjects based on a raycast from the mouse position.
        /// For 2D projects, intersects with XY plane at Z=0.
        /// </summary>
        /// <param name="ray">The ray to cast from the mouse position</param>
        /// <param name="sceneCamera">The scene view camera</param>
        /// <returns>The position where GameObjects should be placed</returns>
        public Vector3 GetPlacementPosition(Ray ray, Camera sceneCamera)
        {
            // For 2D projects, use a plane intersection at Z=0 for consistent results
            Plane xyPlane = new Plane(Vector3.forward, Vector3.zero);
            
            if (xyPlane.Raycast(ray, out float enter) && enter > 0)
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                return hitPoint;
            }

            // Fallback: Try 3D raycast
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hasLoggedNoSurfaceWarning = false;
                return hit.point;
            }

            // Log warning only once to avoid console spam
            if (!hasLoggedNoSurfaceWarning)
            {
                UnityEngine.Debug.LogWarning(WARNING_NO_SURFACE);
                hasLoggedNoSurfaceWarning = true;
            }

            // Final fallback: position at 10 units from the camera
            return ray.origin + ray.direction * DEFAULT_PLACEMENT_DISTANCE;
        }

        /// <summary>
        /// Resets the cached position. Call this when mouse is released.
        /// </summary>
        public void ResetPositionCache()
        {
            // Method kept for compatibility but no longer needed
        }


    }
}
