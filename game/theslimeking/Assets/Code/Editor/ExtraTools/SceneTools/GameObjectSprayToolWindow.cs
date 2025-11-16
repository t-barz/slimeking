using UnityEngine;
using UnityEditor;
using GameObjectSprayTool.Services;

namespace ExtraTools.SceneTools
{
    public class GameObjectSprayToolWindow : EditorWindow
    {
        // Configuration fields
        private GameObject[] prefabSlots = new GameObject[5];
        private float brushRadius = 2.0f;
        private float density = 5.0f;
        private float minSpacing = 0.5f;
        private bool isBrushActive = false;
        private bool isEraserActive = false;
        private bool isSelectiveEraserActive = false;

        // Services
        private SceneViewHandler sceneViewHandler;
        private PlacementService placementService;
        private RaycastService raycastService;

        // Constants
        private const float MIN_RADIUS = 0.1f;
        private const float MAX_RADIUS = 5.0f;
        private const float MIN_DENSITY = 0.1f;
        private const float MAX_DENSITY = 50.0f;
        private const float MIN_SPACING = 0.01f;
        private const float MAX_SPACING = 10.0f;
        private const string ERROR_NO_PREFABS = "Adicione pelo menos um GameObject nos slots para usar a ferramenta.";
        private const string ERROR_INVALID_SCENE = "Não foi possível acessar a Scene View. Certifique-se de que uma cena está aberta.";

        // EditorPrefs Keys
        private const string PREF_PREFIX = "GameObjectSprayTool_";
        private const string PREF_RADIUS = PREF_PREFIX + "BrushRadius";
        private const string PREF_DENSITY = PREF_PREFIX + "Density";
        private const string PREF_SPACING = PREF_PREFIX + "MinSpacing";
        private const string PREF_BRUSH_ACTIVE = PREF_PREFIX + "BrushActive";
        private const string PREF_ERASER_ACTIVE = PREF_PREFIX + "EraserActive";
        private const string PREF_SELECTIVE_ERASER_ACTIVE = PREF_PREFIX + "SelectiveEraserActive";
        private const string PREF_PREFAB_1 = PREF_PREFIX + "Prefab1_InstanceID";
        private const string PREF_PREFAB_2 = PREF_PREFIX + "Prefab2_InstanceID";
        private const string PREF_PREFAB_3 = PREF_PREFIX + "Prefab3_InstanceID";
        private const string PREF_PREFAB_4 = PREF_PREFIX + "Prefab4_InstanceID";
        private const string PREF_PREFAB_5 = PREF_PREFIX + "Prefab5_InstanceID";

        // Default Values
        private const float DEFAULT_RADIUS = 2.0f;
        private const float DEFAULT_DENSITY = 5.0f;
        private const float DEFAULT_SPACING = 0.5f;

        [MenuItem("Extra Tools/Scene Tools/GameObject Spray Tool")]
        public static void ShowWindow()
        {
            GameObjectSprayToolWindow window = GetWindow<GameObjectSprayToolWindow>("GameObject Spray Tool");
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
            InitializeServices();
            RegisterSceneViewCallback();
        }

        private void OnDisable()
        {
            UnregisterSceneViewCallback();
        }

        /// <summary>
        /// Initializes all service dependencies.
        /// </summary>
        private void InitializeServices()
        {
            // Initialize services in dependency order
            raycastService = new RaycastService();
            placementService = new PlacementService();
            sceneViewHandler = new SceneViewHandler(raycastService, placementService);

            // Pass initial configuration to scene view handler
            UpdateSceneViewConfiguration();
        }

        /// <summary>
        /// Registers the Scene View callback for tool interaction.
        /// Validates that SceneView is available before registering.
        /// </summary>
        private void RegisterSceneViewCallback()
        {
            // Verify SceneView is available before registering
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.duringSceneGui += OnSceneGUI;
            }
            else
            {
                Debug.LogWarning(ERROR_INVALID_SCENE);
            }
        }

        /// <summary>
        /// Unregisters the Scene View callback.
        /// </summary>
        private void UnregisterSceneViewCallback()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        /// <summary>
        /// Scene View GUI callback that delegates to SceneViewHandler.
        /// </summary>
        private void OnSceneGUI(SceneView sceneView)
        {
            // Process brush mode
            if (isBrushActive && HasValidPrefabs())
            {
                UpdateSceneViewConfiguration();
                sceneViewHandler?.OnSceneGUI(sceneView);
                return;
            }

            // Process eraser mode
            if (isEraserActive)
            {
                HandleEraserMode(sceneView, false);
                return;
            }

            // Process selective eraser mode
            if (isSelectiveEraserActive)
            {
                HandleEraserMode(sceneView, true);
                return;
            }
        }

        /// <summary>
        /// Updates the SceneViewHandler with current configuration values.
        /// </summary>
        private void UpdateSceneViewConfiguration()
        {
            sceneViewHandler?.SetConfiguration(prefabSlots, brushRadius, density, minSpacing);
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();

            GUILayout.Label("GameObject Spray Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Tool Buttons (Brush and Eraser)
            DrawToolButtons();
            EditorGUILayout.Space();

            // Prefab Slots Section
            GUILayout.Label("Prefab Slots", EditorStyles.boldLabel);
            for (int i = 0; i < prefabSlots.Length; i++)
            {
                prefabSlots[i] = (GameObject)EditorGUILayout.ObjectField(
                    $"Slot {i + 1}",
                    prefabSlots[i],
                    typeof(GameObject),
                    false
                );
            }

            EditorGUILayout.Space();

            // Brush Settings Section
            GUILayout.Label("Brush Settings", EditorStyles.boldLabel);
            brushRadius = EditorGUILayout.Slider("Brush Radius", brushRadius, MIN_RADIUS, MAX_RADIUS);
            density = EditorGUILayout.Slider("Density", density, MIN_DENSITY, MAX_DENSITY);
            minSpacing = EditorGUILayout.Slider("Min Spacing", minSpacing, MIN_SPACING, MAX_SPACING);

            EditorGUILayout.Space();

            // Validation - Show HelpBox when all slots are empty
            if (!HasValidPrefabs())
            {
                EditorGUILayout.HelpBox(ERROR_NO_PREFABS, MessageType.Warning);
            }

            // Save settings when values change
            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
                UpdateSceneViewConfiguration();
            }
        }

        private bool HasValidPrefabs()
        {
            foreach (GameObject prefab in prefabSlots)
            {
                if (prefab != null)
                {
                    return true;
                }
            }
            return false;
        }

        private void DrawToolButtons()
        {
            // Create horizontal layout for centered buttons
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Brush Button
            GUIContent brushContent = new GUIContent(
                isBrushActive ? "Brush" : "Brush",
                EditorGUIUtility.IconContent("Grid.PaintTool").image,
                isBrushActive ? "Click to deactivate brush" : "Click to activate brush"
            );

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedHeight = 40;
            buttonStyle.fixedWidth = 100;

            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = isBrushActive ? new Color(0.3f, 0.8f, 1.0f) : Color.white;

            if (GUILayout.Button(brushContent, buttonStyle))
            {
                bool previousState = isBrushActive;
                isBrushActive = !isBrushActive;

                // Deactivate erasers when activating brush
                if (isBrushActive)
                {
                    isEraserActive = false;
                    isSelectiveEraserActive = false;
                    EditorPrefs.SetBool(PREF_ERASER_ACTIVE, false);
                    EditorPrefs.SetBool(PREF_SELECTIVE_ERASER_ACTIVE, false);
                }

                EditorPrefs.SetBool(PREF_BRUSH_ACTIVE, isBrushActive);

                if (!previousState && isBrushActive)
                {
                    EnsureParentObjectExists();
                }

                SceneView.RepaintAll();
            }

            GUI.backgroundColor = originalColor;

            // Space between buttons
            GUILayout.Space(10);

            // Eraser Button
            GUIContent eraserContent = new GUIContent(
                isEraserActive ? "Eraser" : "Eraser",
                EditorGUIUtility.IconContent("Grid.EraserTool").image,
                isEraserActive ? "Click to deactivate eraser" : "Click to activate eraser"
            );

            GUI.backgroundColor = isEraserActive ? new Color(1.0f, 0.4f, 0.4f) : Color.white;

            if (GUILayout.Button(eraserContent, buttonStyle))
            {
                isEraserActive = !isEraserActive;

                // Deactivate other modes when activating eraser
                if (isEraserActive)
                {
                    isBrushActive = false;
                    isSelectiveEraserActive = false;
                    EditorPrefs.SetBool(PREF_BRUSH_ACTIVE, false);
                    EditorPrefs.SetBool(PREF_SELECTIVE_ERASER_ACTIVE, false);
                }

                EditorPrefs.SetBool(PREF_ERASER_ACTIVE, isEraserActive);
                SceneView.RepaintAll();
            }

            GUI.backgroundColor = originalColor;

            // Space between buttons
            GUILayout.Space(10);

            // Selective Eraser Button
            GUIContent selectiveEraserContent = new GUIContent(
                isSelectiveEraserActive ? "Sel. Eraser" : "Sel. Eraser",
                EditorGUIUtility.IconContent("FilterSelectedOnly").image,
                isSelectiveEraserActive ? "Click to deactivate selective eraser" : "Click to activate selective eraser (erases only prefab types in slots)"
            );

            GUI.backgroundColor = isSelectiveEraserActive ? new Color(1.0f, 0.6f, 0.2f) : Color.white;

            if (GUILayout.Button(selectiveEraserContent, buttonStyle))
            {
                isSelectiveEraserActive = !isSelectiveEraserActive;

                // Deactivate other modes when activating selective eraser
                if (isSelectiveEraserActive)
                {
                    isBrushActive = false;
                    isEraserActive = false;
                    EditorPrefs.SetBool(PREF_BRUSH_ACTIVE, false);
                    EditorPrefs.SetBool(PREF_ERASER_ACTIVE, false);
                }

                EditorPrefs.SetBool(PREF_SELECTIVE_ERASER_ACTIVE, isSelectiveEraserActive);
                SceneView.RepaintAll();
            }

            GUI.backgroundColor = originalColor;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Ensures the parent object exists in the scene when brush is activated.
        /// Creates it at Vector2.zero if it doesn't exist.
        /// </summary>
        private void EnsureParentObjectExists()
        {
            if (placementService == null)
            {
                return;
            }

            // Use HierarchyOrganizer to check/create parent object
            HierarchyOrganizer organizer = new HierarchyOrganizer();
            Transform parent = organizer.GetOrCreateParentObject();

            if (parent != null)
            {
                // Ensure position is at Vector2.zero (which is Vector3.zero in 3D space)
                parent.position = Vector3.zero;
                Debug.Log($"Parent object '{parent.name}' is ready at position {parent.position}");
            }
        }

        /// <summary>
        /// Handles eraser mode in the Scene View.
        /// Erases GameObjects within the brush radius that are children of "SprayedObjects".
        /// </summary>
        /// <param name="sceneView">The scene view</param>
        /// <param name="selectiveMode">If true, only erases objects matching prefab slots</param>
        private void HandleEraserMode(SceneView sceneView, bool selectiveMode)
        {
            Event currentEvent = Event.current;

            // Get mouse world position
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);
            Vector3 mouseWorldPosition = raycastService.GetPlacementPosition(ray, sceneView.camera);

            // Draw eraser gizmo during Repaint
            if (currentEvent.type == EventType.Repaint)
            {
                DrawEraserGizmo(mouseWorldPosition, brushRadius, selectiveMode);
            }

            // Force repaint on mouse move to update gizmo position
            if (currentEvent.type == EventType.MouseMove)
            {
                sceneView.Repaint();
            }

            // Handle left mouse button for erasing
            if (currentEvent.button == 0 && currentEvent.type == EventType.MouseDown)
            {
                EraseObjectsInRadius(mouseWorldPosition, brushRadius, selectiveMode);
                currentEvent.Use();
                sceneView.Repaint();
            }
            else if (currentEvent.button == 0 && currentEvent.type == EventType.MouseDrag)
            {
                EraseObjectsInRadius(mouseWorldPosition, brushRadius, selectiveMode);
                currentEvent.Use();
                sceneView.Repaint();
            }
            else if (currentEvent.button == 0 && currentEvent.type == EventType.MouseUp)
            {
                raycastService.ResetPositionCache(); // Reset cache when mouse is released
                currentEvent.Use();
            }
        }

        /// <summary>
        /// Draws the eraser gizmo in the Scene View.
        /// </summary>
        private void DrawEraserGizmo(Vector3 position, float radius, bool selectiveMode)
        {
            if (selectiveMode)
            {
                // Orange color for selective eraser
                Handles.color = new Color(1.0f, 0.6f, 0.2f, 0.8f);
                Handles.DrawWireDisc(position, Vector3.forward, radius);

                Handles.color = new Color(1.0f, 0.6f, 0.2f, 0.1f);
                Handles.DrawSolidDisc(position, Vector3.forward, radius);
            }
            else
            {
                // Red color for normal eraser
                Handles.color = new Color(1.0f, 0.3f, 0.3f, 0.8f);
                Handles.DrawWireDisc(position, Vector3.forward, radius);

                Handles.color = new Color(1.0f, 0.3f, 0.3f, 0.1f);
                Handles.DrawSolidDisc(position, Vector3.forward, radius);
            }
        }

        /// <summary>
        /// Erases all GameObjects within the specified radius that are children of "SprayedObjects".
        /// </summary>
        /// <param name="center">Center position of the eraser</param>
        /// <param name="radius">Radius of the eraser</param>
        /// <param name="selectiveMode">If true, only erases objects matching prefab slots</param>
        private void EraseObjectsInRadius(Vector3 center, float radius, bool selectiveMode)
        {
            // Find the SprayedObjects parent
            GameObject parentObject = GameObject.Find("SprayedObjects");
            if (parentObject == null)
            {
                return;
            }

            // Find all children within radius
            Transform parentTransform = parentObject.transform;
            float radiusSquared = radius * radius;

            // Collect objects to delete (can't delete while iterating)
            System.Collections.Generic.List<GameObject> objectsToDelete = new System.Collections.Generic.List<GameObject>();

            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform child = parentTransform.GetChild(i);
                float sqrDistance = (child.position.x - center.x) * (child.position.x - center.x) +
                                   (child.position.y - center.y) * (child.position.y - center.y);

                if (sqrDistance <= radiusSquared)
                {
                    // In selective mode, check if object matches any prefab slot
                    if (selectiveMode)
                    {
                        if (IsObjectMatchingPrefabSlots(child.gameObject))
                        {
                            objectsToDelete.Add(child.gameObject);
                        }
                    }
                    else
                    {
                        objectsToDelete.Add(child.gameObject);
                    }
                }
            }

            // Delete objects with Undo support
            if (objectsToDelete.Count > 0)
            {
                Undo.SetCurrentGroupName(selectiveMode ? "Selective Erase GameObjects" : "Erase GameObjects");
                foreach (GameObject obj in objectsToDelete)
                {
                    Undo.DestroyObjectImmediate(obj);
                }
            }
        }

        /// <summary>
        /// Checks if a GameObject matches any of the prefab slots.
        /// </summary>
        private bool IsObjectMatchingPrefabSlots(GameObject obj)
        {
            // Get the prefab source of the object
            GameObject prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(obj);

            if (prefabSource == null)
            {
                return false;
            }

            // Check if it matches any of the prefab slots
            foreach (GameObject prefabSlot in prefabSlots)
            {
                if (prefabSlot != null && prefabSource == prefabSlot)
                {
                    return true;
                }
            }

            return false;
        }

        private void LoadSettings()
        {
            // Load brush radius with default value and validate with Mathf.Clamp
            brushRadius = EditorPrefs.GetFloat(PREF_RADIUS, DEFAULT_RADIUS);
            brushRadius = Mathf.Clamp(brushRadius, MIN_RADIUS, MAX_RADIUS);

            // Load density with default value and validate with Mathf.Clamp
            density = EditorPrefs.GetFloat(PREF_DENSITY, DEFAULT_DENSITY);
            density = Mathf.Clamp(density, MIN_DENSITY, MAX_DENSITY);

            // Load min spacing with default value and validate with Mathf.Clamp
            minSpacing = EditorPrefs.GetFloat(PREF_SPACING, DEFAULT_SPACING);
            minSpacing = Mathf.Clamp(minSpacing, MIN_SPACING, MAX_SPACING);

            // Load brush active state
            isBrushActive = EditorPrefs.GetBool(PREF_BRUSH_ACTIVE, false);

            // Load eraser active state
            isEraserActive = EditorPrefs.GetBool(PREF_ERASER_ACTIVE, false);

            // Load selective eraser active state
            isSelectiveEraserActive = EditorPrefs.GetBool(PREF_SELECTIVE_ERASER_ACTIVE, false);

            // Load prefab slots using InstanceID
            LoadPrefabSlot(0, PREF_PREFAB_1);
            LoadPrefabSlot(1, PREF_PREFAB_2);
            LoadPrefabSlot(2, PREF_PREFAB_3);
            LoadPrefabSlot(3, PREF_PREFAB_4);
            LoadPrefabSlot(4, PREF_PREFAB_5);
        }

        private void LoadPrefabSlot(int slotIndex, string prefKey)
        {
            int instanceID = EditorPrefs.GetInt(prefKey, 0);
            if (instanceID != 0)
            {
                GameObject loadedObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (loadedObject != null)
                {
                    prefabSlots[slotIndex] = loadedObject;
                }
            }
        }

        private void SaveSettings()
        {
            // Validate and save brush radius with Mathf.Clamp before saving
            brushRadius = Mathf.Clamp(brushRadius, MIN_RADIUS, MAX_RADIUS);
            EditorPrefs.SetFloat(PREF_RADIUS, brushRadius);

            // Validate and save density with Mathf.Clamp before saving
            density = Mathf.Clamp(density, MIN_DENSITY, MAX_DENSITY);
            EditorPrefs.SetFloat(PREF_DENSITY, density);

            // Validate and save min spacing with Mathf.Clamp before saving
            minSpacing = Mathf.Clamp(minSpacing, MIN_SPACING, MAX_SPACING);
            EditorPrefs.SetFloat(PREF_SPACING, minSpacing);

            // Save prefab slots using InstanceID
            SavePrefabSlot(0, PREF_PREFAB_1);
            SavePrefabSlot(1, PREF_PREFAB_2);
            SavePrefabSlot(2, PREF_PREFAB_3);
            SavePrefabSlot(3, PREF_PREFAB_4);
            SavePrefabSlot(4, PREF_PREFAB_5);
        }

        private void SavePrefabSlot(int slotIndex, string prefKey)
        {
            if (prefabSlots[slotIndex] != null)
            {
                int instanceID = prefabSlots[slotIndex].GetInstanceID();
                EditorPrefs.SetInt(prefKey, instanceID);
            }
            else
            {
                EditorPrefs.SetInt(prefKey, 0);
            }
        }
    }
}
