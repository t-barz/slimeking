using UnityEditor;
using UnityEngine;

namespace GameObjectSprayTool.Services
{
    /// <summary>
    /// Handles Scene View interaction for the GameObject Spray Tool.
    /// Manages mouse events, gizmo rendering, and coordinates with placement services.
    /// </summary>
    public class SceneViewHandler
    {
        private readonly RaycastService raycastService;
        private readonly PlacementService placementService;
        
        // Mouse state tracking
        private bool isMouseDown;
        private Vector3 lastSprayPosition;
        private double lastSprayTime;
        
        // Throttling configuration (increased for better performance)
        private const double SPRAY_THROTTLE_MS = 150.0;
        
        // Distance threshold to avoid spraying at same position
        private const float MIN_SPRAY_DISTANCE = 0.1f;
        
        // Configuration (set by the window)
        private GameObject[] prefabSlots;
        private float brushRadius;
        private float density;
        private float minSpacing;
        
        public SceneViewHandler(RaycastService raycastService, PlacementService placementService)
        {
            this.raycastService = raycastService;
            this.placementService = placementService;
        }
        
        /// <summary>
        /// Sets the current tool configuration with validation.
        /// </summary>
        public void SetConfiguration(GameObject[] prefabSlots, float brushRadius, float density, float minSpacing)
        {
            // Validate parameters
            if (brushRadius <= 0)
            {
                Debug.LogWarning("Invalid brush radius in configuration. Using minimum value.");
                brushRadius = 0.1f;
            }

            if (density <= 0)
            {
                Debug.LogWarning("Invalid density in configuration. Using minimum value.");
                density = 1;
            }

            if (minSpacing < 0)
            {
                Debug.LogWarning("Invalid min spacing in configuration. Using minimum value.");
                minSpacing = 0.01f;
            }

            this.prefabSlots = prefabSlots;
            this.brushRadius = brushRadius;
            this.density = density;
            this.minSpacing = minSpacing;
        }
        
        /// <summary>
        /// Main Scene View GUI callback. Handles mouse events and gizmo rendering.
        /// Optimized to only draw gizmo during Repaint events.
        /// </summary>
        public void OnSceneGUI(SceneView sceneView)
        {
            // Get current event
            Event currentEvent = Event.current;
            
            // Get mouse world position
            Vector3? mouseWorldPosition = GetMouseWorldPosition(currentEvent, sceneView);
            
            // Only draw gizmo during Repaint event to avoid redundant drawing
            if (currentEvent.type == EventType.Repaint && mouseWorldPosition.HasValue)
            {
                DrawBrushGizmo(mouseWorldPosition.Value, brushRadius);
            }
            
            // Force repaint on mouse move to update gizmo position
            if (currentEvent.type == EventType.MouseMove)
            {
                sceneView.Repaint();
            }
            
            // Handle input events
            HandleInput(currentEvent, mouseWorldPosition, sceneView);
        }
        
        /// <summary>
        /// Handles mouse input events (click and drag).
        /// </summary>
        private void HandleInput(Event currentEvent, Vector3? mouseWorldPosition, SceneView sceneView)
        {
            // Only process left mouse button events
            if (currentEvent.button != 0)
            {
                return;
            }
            
            // Check if we have valid configuration
            if (!HasValidConfiguration())
            {
                return;
            }
            
            // Handle mouse down event
            if (currentEvent.type == EventType.MouseDown && mouseWorldPosition.HasValue)
            {
                isMouseDown = true;
                SprayAtPosition(mouseWorldPosition.Value);
                
                // Consume the event to prevent default Scene View behavior
                currentEvent.Use();
                sceneView.Repaint();
            }
            // Handle mouse drag event
            else if (currentEvent.type == EventType.MouseDrag && isMouseDown && mouseWorldPosition.HasValue)
            {
                // Apply throttling to limit spray frequency during drag
                double currentTime = EditorApplication.timeSinceStartup * 1000.0; // Convert to milliseconds
                
                // Check both time and distance to avoid redundant sprays
                float distanceFromLast = Vector3.Distance(mouseWorldPosition.Value, lastSprayPosition);
                
                if (currentTime - lastSprayTime >= SPRAY_THROTTLE_MS && distanceFromLast >= MIN_SPRAY_DISTANCE)
                {
                    SprayAtPosition(mouseWorldPosition.Value);
                    lastSprayTime = currentTime;
                }
                
                // Consume the event
                currentEvent.Use();
                sceneView.Repaint();
            }
            // Handle mouse up event
            else if (currentEvent.type == EventType.MouseUp && currentEvent.button == 0)
            {
                isMouseDown = false;
                raycastService.ResetPositionCache(); // Reset cache to prevent issues on next click
                currentEvent.Use();
            }
        }
        
        /// <summary>
        /// Converts mouse position to world coordinates using raycast.
        /// </summary>
        private Vector3? GetMouseWorldPosition(Event currentEvent, SceneView sceneView)
        {
            // Convert GUI point to world ray using HandleUtility
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            
            // Use RaycastService to get placement position
            Vector3 placementPosition = raycastService.GetPlacementPosition(ray, sceneView.camera);
            
            return placementPosition;
        }
        
        /// <summary>
        /// Draws the circular brush gizmo in the Scene View.
        /// </summary>
        private void DrawBrushGizmo(Vector3 position, float radius)
        {
            // Set gizmo color
            Handles.color = new Color(0.3f, 0.8f, 1.0f, 0.8f); // Light blue with transparency
            
            // Draw wire disc at the position
            // Using Vector3.forward as the normal for XY plane (2D projects)
            Handles.DrawWireDisc(position, Vector3.forward, radius);
            
            // Optionally draw a filled disc with transparency for better visibility
            Handles.color = new Color(0.3f, 0.8f, 1.0f, 0.1f);
            Handles.DrawSolidDisc(position, Vector3.forward, radius);
        }
        
        /// <summary>
        /// Triggers the spray operation at the specified position.
        /// </summary>
        private void SprayAtPosition(Vector3 position)
        {
            lastSprayPosition = position;
            placementService.SprayGameObjects(position, prefabSlots, brushRadius, density, minSpacing);
        }
        
        /// <summary>
        /// Checks if the current configuration is valid for spraying.
        /// </summary>
        private bool HasValidConfiguration()
        {
            if (prefabSlots == null || prefabSlots.Length == 0)
            {
                return false;
            }
            
            // Check if at least one prefab slot is not null
            foreach (GameObject prefab in prefabSlots)
            {
                if (prefab != null)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
