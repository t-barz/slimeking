using System.Collections.Generic;
using UnityEngine;

namespace GameObjectSprayTool.Services
{
    /// <summary>
    /// Service responsible for generating random positions within a circular area
    /// with minimum distance validation between positions.
    /// Uses spatial hashing for O(1) collision detection instead of O(n).
    /// </summary>
    public class RandomDistributionService
    {
        private readonly System.Random random;
        
        // Spatial hash grid for fast collision detection
        private Dictionary<Vector2Int, List<Vector3>> spatialGrid;
        private float cellSize;

        public RandomDistributionService()
        {
            random = new System.Random();
        }

        /// <summary>
        /// Generates a list of random positions within a circular area.
        /// Uses spatial hashing for O(1) collision detection.
        /// </summary>
        /// <param name="center">The center point of the circular area</param>
        /// <param name="radius">The radius of the circular area</param>
        /// <param name="count">The number of positions to generate (density)</param>
        /// <param name="minSpacing">The minimum spacing between positions</param>
        /// <returns>List of Vector3 positions within the specified radius</returns>
        public List<Vector3> GenerateRandomPositions(Vector3 center, float radius, int count, float minSpacing)
        {
            // Validate parameters
            if (radius <= 0)
            {
                UnityEngine.Debug.LogWarning("Invalid radius for position generation. Radius must be greater than 0.");
                return new List<Vector3>();
            }

            if (count <= 0)
            {
                UnityEngine.Debug.LogWarning("Invalid count for position generation. Count must be greater than 0.");
                return new List<Vector3>();
            }

            // Initialize spatial grid with cell size equal to minSpacing for optimal performance
            cellSize = Mathf.Max(minSpacing, 0.1f);
            spatialGrid = new Dictionary<Vector2Int, List<Vector3>>();
            
            List<Vector3> positions = new List<Vector3>(count);

            // Reduce max attempts for better performance
            int maxAttempts = Mathf.Min(20, count * 2);

            // Generate the exact number of positions as specified by density
            for (int i = 0; i < count; i++)
            {
                Vector3 position = GenerateRandomPointInCircle(center, radius);
                
                // Ensure minimum distance between positions using spatial hashing
                int attempts = 0;
                
                while (IsPositionTooCloseFast(position, minSpacing) && attempts < maxAttempts)
                {
                    position = GenerateRandomPointInCircle(center, radius);
                    attempts++;
                }
                
                positions.Add(position);
                AddToSpatialGrid(position);
            }

            return positions;
        }

        /// <summary>
        /// Generates a random point within a circle using Unity's Random.insideUnitCircle.
        /// </summary>
        /// <param name="center">The center point of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <returns>A random Vector3 position within the circle</returns>
        private Vector3 GenerateRandomPointInCircle(Vector3 center, float radius)
        {
            // Use Unity's Random.insideUnitCircle for uniform distribution
            Vector2 randomPoint = Random.insideUnitCircle * radius;
            
            // Convert 2D point to 3D for XY plane (2D projects), maintaining the center's Z coordinate
            return new Vector3(
                center.x + randomPoint.x,
                center.y + randomPoint.y,
                center.z
            );
        }

        /// <summary>
        /// Converts a world position to a grid cell coordinate.
        /// </summary>
        private Vector2Int GetGridCell(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / cellSize),
                Mathf.FloorToInt(position.y / cellSize)
            );
        }

        /// <summary>
        /// Adds a position to the spatial grid for fast lookup.
        /// </summary>
        private void AddToSpatialGrid(Vector3 position)
        {
            Vector2Int cell = GetGridCell(position);
            
            if (!spatialGrid.ContainsKey(cell))
            {
                spatialGrid[cell] = new List<Vector3>();
            }
            
            spatialGrid[cell].Add(position);
        }

        /// <summary>
        /// Fast collision detection using spatial hashing.
        /// Only checks positions in nearby grid cells instead of all positions.
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <param name="minSpacing">The minimum allowed spacing between positions</param>
        /// <returns>True if the position is too close to any existing position</returns>
        private bool IsPositionTooCloseFast(Vector3 position, float minSpacing)
        {
            Vector2Int cell = GetGridCell(position);
            float minSpacingSquared = minSpacing * minSpacing; // Use squared distance to avoid sqrt
            
            // Check only the 9 surrounding cells (3x3 grid)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int neighborCell = new Vector2Int(cell.x + x, cell.y + y);
                    
                    if (spatialGrid.TryGetValue(neighborCell, out List<Vector3> positions))
                    {
                        foreach (Vector3 existingPosition in positions)
                        {
                            // Use squared distance for performance (avoids sqrt calculation)
                            float sqrDistance = (position.x - existingPosition.x) * (position.x - existingPosition.x) +
                                              (position.y - existingPosition.y) * (position.y - existingPosition.y);
                            
                            if (sqrDistance < minSpacingSquared)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            
            return false;
        }
    }
}
