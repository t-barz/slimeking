using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Static utility class for drawing NPC-related gizmos in the Scene View.
    /// Provides visual feedback for AI behavior ranges, patrol paths, and interaction zones.
    /// Includes gizmo caching for performance optimization.
    /// </summary>
    public static class NPCGizmosDrawer
    {
        #region Gizmo Caching (Performance Optimization)

        /// <summary>
        /// Cache structure for gizmo data to reduce Scene View overhead.
        /// </summary>
        private class GizmoCache
        {
            public Vector3 position;
            public float wanderRadius;
            public float detectionRange;
            public float dialogueRange;
            public List<Vector2> patrolPoints;
            public bool isDirty;
            public double lastUpdateTime;

            public GizmoCache()
            {
                patrolPoints = new List<Vector2>();
                isDirty = true;
                lastUpdateTime = EditorApplication.timeSinceStartup;
            }

            public bool NeedsUpdate(Vector3 newPosition, float newWanderRadius, float newDetectionRange, 
                                   float newDialogueRange, List<Vector2> newPatrolPoints)
            {
                // Check if cache is stale (older than 0.5 seconds)
                if (EditorApplication.timeSinceStartup - lastUpdateTime > 0.5)
                {
                    return true;
                }

                // Check if values changed
                if (position != newPosition || 
                    wanderRadius != newWanderRadius || 
                    detectionRange != newDetectionRange || 
                    dialogueRange != newDialogueRange)
                {
                    return true;
                }

                // Check patrol points
                if (newPatrolPoints != null && patrolPoints.Count != newPatrolPoints.Count)
                {
                    return true;
                }

                if (newPatrolPoints != null)
                {
                    for (int i = 0; i < patrolPoints.Count; i++)
                    {
                        if (patrolPoints[i] != newPatrolPoints[i])
                        {
                            return true;
                        }
                    }
                }

                return isDirty;
            }

            public void Update(Vector3 newPosition, float newWanderRadius, float newDetectionRange, 
                             float newDialogueRange, List<Vector2> newPatrolPoints)
            {
                position = newPosition;
                wanderRadius = newWanderRadius;
                detectionRange = newDetectionRange;
                dialogueRange = newDialogueRange;
                
                patrolPoints.Clear();
                if (newPatrolPoints != null)
                {
                    patrolPoints.AddRange(newPatrolPoints);
                }

                isDirty = false;
                lastUpdateTime = EditorApplication.timeSinceStartup;
            }
        }

        private static Dictionary<int, GizmoCache> gizmoCache = new Dictionary<int, GizmoCache>();

        /// <summary>
        /// Clears the gizmo cache. Call this when you want to force a refresh.
        /// </summary>
        public static void ClearCache()
        {
            gizmoCache.Clear();
        }

        /// <summary>
        /// Gets or creates a gizmo cache for a specific GameObject instance.
        /// </summary>
        private static GizmoCache GetOrCreateCache(int instanceId)
        {
            if (!gizmoCache.ContainsKey(instanceId))
            {
                gizmoCache[instanceId] = new GizmoCache();
            }
            return gizmoCache[instanceId];
        }

        #endregion

        #region Gizmo Drawing Methods
        /// <summary>
        /// Draws a yellow wire circle representing the wander radius for NPCs with Wander AI.
        /// </summary>
        /// <param name="position">Center position of the wander area</param>
        /// <param name="radius">Radius of the wander area in meters</param>
        public static void DrawWanderRadius(Vector3 position, float radius)
        {
            if (radius <= 0) return;

            Handles.color = new Color(1f, 1f, 0f, 0.3f); // Yellow with alpha
            Handles.DrawWireDisc(position, Vector3.forward, radius);
            
            // Draw label
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.yellow;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            
            Vector3 labelPosition = position + Vector3.up * (radius + 0.5f);
            Handles.Label(labelPosition, $"Wander Radius: {radius:F1}m", labelStyle);
        }

        /// <summary>
        /// Draws cyan lines connecting patrol points with arrows and spheres for NPCs with Patrol AI.
        /// </summary>
        /// <param name="position">Center position of the NPC (for relative patrol points)</param>
        /// <param name="points">List of patrol points to connect</param>
        public static void DrawPatrolPath(Vector3 position, List<Vector2> points)
        {
            if (points == null || points.Count < 2) return;

            Handles.color = Color.cyan;

            // Draw lines connecting patrol points
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 currentPoint = new Vector3(points[i].x, points[i].y, 0);
                Vector3 nextPoint = new Vector3(points[(i + 1) % points.Count].x, points[(i + 1) % points.Count].y, 0);

                // Draw line
                Handles.DrawLine(currentPoint, nextPoint);

                // Draw arrow in the middle of the line
                Vector3 midPoint = (currentPoint + nextPoint) * 0.5f;
                Vector3 direction = (nextPoint - currentPoint).normalized;
                DrawArrow(midPoint, direction, 0.3f);

                // Draw sphere at patrol point
                Handles.SphereHandleCap(0, currentPoint, Quaternion.identity, 0.2f, EventType.Repaint);

                // Draw label for patrol point
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.normal.textColor = Color.cyan;
                labelStyle.fontStyle = FontStyle.Bold;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                
                Vector3 labelPosition = currentPoint + Vector3.up * 0.5f;
                Handles.Label(labelPosition, $"Patrol Point {i + 1}", labelStyle);
            }
        }

        /// <summary>
        /// Draws a red wire circle representing the detection range for NPCs.
        /// </summary>
        /// <param name="position">Center position of the NPC</param>
        /// <param name="range">Detection range in meters</param>
        public static void DrawDetectionRange(Vector3 position, float range)
        {
            if (range <= 0) return;

            Handles.color = new Color(1f, 0f, 0f, 0.2f); // Red with alpha
            Handles.DrawWireDisc(position, Vector3.forward, range);
            
            // Draw label
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = new Color(1f, 0.5f, 0.5f);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            
            Vector3 labelPosition = position + Vector3.right * (range * 0.7f);
            Handles.Label(labelPosition, $"Detection: {range:F1}m", labelStyle);
        }

        /// <summary>
        /// Draws a green wire circle representing the dialogue trigger range for NPCs.
        /// </summary>
        /// <param name="position">Center position of the NPC</param>
        /// <param name="range">Dialogue trigger range in meters</param>
        public static void DrawDialogueTriggerRange(Vector3 position, float range)
        {
            if (range <= 0) return;

            Handles.color = new Color(0f, 1f, 0f, 0.2f); // Green with alpha
            Handles.DrawWireDisc(position, Vector3.forward, range);
            
            // Draw label
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = new Color(0.5f, 1f, 0.5f);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.alignment = TextAnchor.MiddleCenter;
            
            Vector3 labelPosition = position + Vector3.left * (range * 0.7f);
            Handles.Label(labelPosition, $"Dialogue: {range:F1}m", labelStyle);
        }

        /// <summary>
        /// Helper method to draw an arrow at a specific position pointing in a direction.
        /// </summary>
        /// <param name="position">Position to draw the arrow</param>
        /// <param name="direction">Direction the arrow should point</param>
        /// <param name="size">Size of the arrow</param>
        private static void DrawArrow(Vector3 position, Vector3 direction, float size)
        {
            Vector3 right = Vector3.Cross(direction, Vector3.forward).normalized;
            Vector3 arrowTip = position + direction * size;
            Vector3 arrowLeft = position - direction * size * 0.3f + right * size * 0.5f;
            Vector3 arrowRight = position - direction * size * 0.3f - right * size * 0.5f;

            Handles.DrawLine(position, arrowTip);
            Handles.DrawLine(arrowTip, arrowLeft);
            Handles.DrawLine(arrowTip, arrowRight);
        }

        #endregion

        #region Optimized Batch Drawing (Performance Optimization)

        /// <summary>
        /// Draws all gizmos for an NPC with caching for performance optimization.
        /// This method checks the cache and only redraws if values have changed.
        /// </summary>
        /// <param name="instanceId">GameObject instance ID for cache lookup</param>
        /// <param name="position">Position of the NPC</param>
        /// <param name="wanderRadius">Wander radius (0 if not using wander AI)</param>
        /// <param name="detectionRange">Detection range (0 if not applicable)</param>
        /// <param name="dialogueRange">Dialogue trigger range (0 if not using dialogue)</param>
        /// <param name="patrolPoints">Patrol points (null if not using patrol AI)</param>
        public static void DrawNPCGizmosCached(int instanceId, Vector3 position, float wanderRadius, 
                                               float detectionRange, float dialogueRange, List<Vector2> patrolPoints)
        {
            GizmoCache cache = GetOrCreateCache(instanceId);

            // Check if cache needs update
            if (!cache.NeedsUpdate(position, wanderRadius, detectionRange, dialogueRange, patrolPoints))
            {
                // Cache is valid, use cached values for drawing
                position = cache.position;
                wanderRadius = cache.wanderRadius;
                detectionRange = cache.detectionRange;
                dialogueRange = cache.dialogueRange;
                patrolPoints = cache.patrolPoints;
            }
            else
            {
                // Update cache with new values
                cache.Update(position, wanderRadius, detectionRange, dialogueRange, patrolPoints);
            }

            // Draw gizmos
            if (wanderRadius > 0)
            {
                DrawWanderRadius(position, wanderRadius);
            }

            if (patrolPoints != null && patrolPoints.Count >= 2)
            {
                DrawPatrolPath(position, patrolPoints);
            }

            if (detectionRange > 0)
            {
                DrawDetectionRange(position, detectionRange);
            }

            if (dialogueRange > 0)
            {
                DrawDialogueTriggerRange(position, dialogueRange);
            }
        }

        #endregion
    }
}
