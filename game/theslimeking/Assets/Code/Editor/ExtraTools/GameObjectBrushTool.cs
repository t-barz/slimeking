using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta de brush para posicionar GameObjects na cena
    /// Baseado na versão anterior que funcionava corretamente
    /// 
    /// Uso:
    /// • Ativar modo Brush e clicar para pintar
    /// • Ativar modo Eraser e clicar para apagar tudo
    /// • Ativar modo Sel.Eraser e clicar para apagar apenas prefabs dos slots
    /// • Teclas 1-3 = Trocar prefab ativo
    /// 
    /// Seguindo a política obrigatória de menus: Extra Tools/Scene Tools/
    /// </summary>
    public class GameObjectBrushTool : EditorWindow
    {
        #region Window Management
        [MenuItem("Extra Tools/Scene Tools/🖌️ GameObject Brush")]
        public static void ShowWindow()
        {
            var window = GetWindow<GameObjectBrushTool>("GameObject Brush");
            window.minSize = new Vector2(320, 500);
            window.Show();
        }
        #endregion

        #region Fields
        // Tool modes
        private bool isBrushActive = false;
        private bool isEraserActive = false;
        private bool isSelectiveEraserActive = false;

        // Brush settings
        private float brushRadius = 2f;
        private float brushDensity = 1f;
        private float minSpacing = 0.5f;
        private bool paintAllSlots = false; // Paint all valid slots instead of just selected one

        // Prefab slots (dynamic system with unlimited slots)
        private List<GameObject> prefabSlots = new List<GameObject> { null }; // Start with 1 slot
        private int selectedPrefabIndex = 0;

        // Property to ensure selectedPrefabIndex is always valid
        private int SafeSelectedPrefabIndex
        {
            get
            {
                if (selectedPrefabIndex < 0 || selectedPrefabIndex >= prefabSlots.Count)
                    selectedPrefabIndex = 0;
                return selectedPrefabIndex;
            }
            set
            {
                selectedPrefabIndex = Mathf.Clamp(value, 0, prefabSlots.Count - 1);
            }
        }

        // Randomization
        private bool randomRotationY = true;
        private bool randomRotationX = false;
        private bool randomRotationZ = false;
        private bool randomScale = false;
        private Vector2 scaleRange = new Vector2(0.8f, 1.2f);

        // Debug
        private bool enableDebugLogs = false;

        // Parent organization
        private string parentObjectName = "PaintedObjects";
        private bool groupByType = true;

        // Private
        private Vector2 scrollPosition;
        private GameObject sprayedObjectsParent;
        private Vector3 lastPaintPosition = Vector3.zero;
        private Dictionary<GameObject, GameObject> prefabParents = new Dictionary<GameObject, GameObject>();
        #endregion

        #region Unity Lifecycle
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        #endregion

        #region GUI
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            DrawHeader();
            EditorGUILayout.Space();

            DrawModeButtons();
            EditorGUILayout.Space();

            DrawPrefabSlots();
            EditorGUILayout.Space();

            DrawBrushSettings();
            EditorGUILayout.Space();

            DrawRandomizationSettings();
            EditorGUILayout.Space();

            DrawDebugSettings();
            EditorGUILayout.Space();

            DrawInstructions();

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.Label("🖌️ GameObject Brush Tool", EditorStyles.largeLabel);
            EditorGUILayout.HelpBox("Ferramenta de brush baseada na versão anterior que funcionava.", MessageType.Info);
        }

        private void DrawModeButtons()
        {
            GUILayout.Label("🎮 Tool Mode", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            // Brush Button
            GUI.backgroundColor = isBrushActive ? Color.green : Color.white;
            if (GUILayout.Button(isBrushActive ? "🖌️ Brush ON" : "🖌️ Brush", GUILayout.Height(30)))
            {
                isBrushActive = !isBrushActive;
                if (isBrushActive)
                {
                    isEraserActive = false;
                    isSelectiveEraserActive = false;
                    GetMainParent(); // Initialize main parent
                }
                SceneView.RepaintAll();
            }

            // Eraser Button
            GUI.backgroundColor = isEraserActive ? Color.red : Color.white;
            if (GUILayout.Button(isEraserActive ? "🗑️ Eraser ON" : "🗑️ Eraser", GUILayout.Height(30)))
            {
                isEraserActive = !isEraserActive;
                if (isEraserActive)
                {
                    isBrushActive = false;
                    isSelectiveEraserActive = false;
                }
                SceneView.RepaintAll();
            }

            // Selective Eraser Button
            GUI.backgroundColor = isSelectiveEraserActive ? new Color(1.0f, 0.6f, 0.2f) : Color.white;
            if (GUILayout.Button(isSelectiveEraserActive ? "🎯 Sel.Eraser ON" : "🎯 Sel.Eraser", GUILayout.Height(30)))
            {
                isSelectiveEraserActive = !isSelectiveEraserActive;
                if (isSelectiveEraserActive)
                {
                    isBrushActive = false;
                    isEraserActive = false;
                }
                SceneView.RepaintAll();
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            // Status
            EditorGUILayout.Space(5);
            string currentMode = "Nenhum";
            if (isBrushActive) currentMode = "🖌️ Pintar";
            else if (isEraserActive) currentMode = "🗑️ Apagar Tudo";
            else if (isSelectiveEraserActive) currentMode = "🎯 Apagar Seletivo";

            EditorGUILayout.HelpBox($"Modo Ativo: {currentMode}", MessageType.Info);
        }

        private void DrawPrefabSlots()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"📦 Prefab Slots ({prefabSlots.Count})", EditorStyles.boldLabel);

            GUILayout.FlexibleSpace();

            // Add slot button
            if (GUILayout.Button("+", GUILayout.Width(25), GUILayout.Height(20)))
            {
                prefabSlots.Add(null);
            }

            // Remove slot button (only if more than 1 slot)
            GUI.enabled = prefabSlots.Count > 1;
            if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(20)))
            {
                if (prefabSlots.Count > 1)
                {
                    prefabSlots.RemoveAt(prefabSlots.Count - 1);
                    // Adjust selectedPrefabIndex if needed
                    if (selectedPrefabIndex >= prefabSlots.Count)
                        selectedPrefabIndex = prefabSlots.Count - 1;
                }
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            for (int i = 0; i < prefabSlots.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(20));

                bool isSelected = i == SafeSelectedPrefabIndex;
                Color originalBgColor = GUI.backgroundColor;
                if (isSelected) GUI.backgroundColor = Color.green;

                if (GUILayout.Button($"[{i + 1}]", GUILayout.Width(40)))
                {
                    SafeSelectedPrefabIndex = i;
                }

                GUI.backgroundColor = originalBgColor;

                // Miniature/Icon for the GameObject
                if (prefabSlots[i] != null)
                {
                    Texture2D icon = AssetPreview.GetMiniThumbnail(prefabSlots[i]);
                    if (icon != null)
                    {
                        GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
                    }
                    else
                    {
                        // Fallback to default GameObject icon
                        GUIContent content = EditorGUIUtility.ObjectContent(prefabSlots[i], typeof(GameObject));
                        GUILayout.Label(content.image, GUILayout.Width(20), GUILayout.Height(20));
                    }
                }
                else
                {
                    // Empty slot - show placeholder
                    GUILayout.Label("", GUILayout.Width(20), GUILayout.Height(20));
                }

                prefabSlots[i] = (GameObject)EditorGUILayout.ObjectField(prefabSlots[i], typeof(GameObject), false);

                // Individual remove button for each slot (only if more than 1 slot)
                GUI.enabled = prefabSlots.Count > 1;
                if (GUILayout.Button("×", GUILayout.Width(25)))
                {
                    prefabSlots.RemoveAt(i);
                    // Adjust selectedPrefabIndex if needed
                    if (selectedPrefabIndex >= i && selectedPrefabIndex > 0)
                        selectedPrefabIndex--;
                    else if (selectedPrefabIndex >= prefabSlots.Count)
                        selectedPrefabIndex = prefabSlots.Count - 1;
                    break; // Exit loop since we modified the list
                }
                GUI.enabled = true;

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(3);

            // Paint mode toggle
            EditorGUILayout.BeginHorizontal();
            paintAllSlots = EditorGUILayout.Toggle("🎨 Pintar Todos os Slots", paintAllSlots);
            EditorGUILayout.EndHorizontal();

            if (paintAllSlots)
            {
                int validSlots = 0;
                foreach (var slot in prefabSlots)
                {
                    if (slot != null) validSlots++;
                }
                EditorGUILayout.LabelField($"Modo: Pintar aleatoriamente entre {validSlots} slots válidos", EditorStyles.miniLabel);
            }
            else
            {
                // Safe bounds check for selectedPrefabIndex
                if (selectedPrefabIndex >= 0 && selectedPrefabIndex < prefabSlots.Count && prefabSlots[selectedPrefabIndex] != null)
                {
                    EditorGUILayout.LabelField($"Prefab Ativo: {prefabSlots[selectedPrefabIndex].name}", EditorStyles.miniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField($"Prefab Ativo: Slot {selectedPrefabIndex + 1} vazio", EditorStyles.miniLabel);
                }
            }
        }

        private void DrawBrushSettings()
        {
            GUILayout.Label("⚙️ Brush Settings", EditorStyles.boldLabel);

            brushRadius = EditorGUILayout.Slider("Brush Radius", brushRadius, 0.5f, 20f);
            brushDensity = EditorGUILayout.Slider("Density", brushDensity, 0.1f, 3f);
            minSpacing = EditorGUILayout.Slider("Min Spacing", minSpacing, 0.1f, 5f);

            EditorGUILayout.Space(5);
            GUILayout.Label("📁 Organization", EditorStyles.boldLabel);

            parentObjectName = EditorGUILayout.TextField("Parent Name", parentObjectName);
            groupByType = EditorGUILayout.Toggle("Group by Object Type", groupByType);

            if (groupByType)
            {
                EditorGUILayout.HelpBox("Objects will be grouped under: [Parent]/[ObjectName]", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("All objects will be placed directly under: [Parent]", MessageType.Info);
            }
        }

        private void DrawRandomizationSettings()
        {
            GUILayout.Label("🎲 Randomization", EditorStyles.boldLabel);

            EditorGUILayout.LabelField("Rotation:", EditorStyles.boldLabel);
            randomRotationY = EditorGUILayout.Toggle("  Y Axis (Yaw)", randomRotationY);
            randomRotationX = EditorGUILayout.Toggle("  X Axis (Pitch)", randomRotationX);
            randomRotationZ = EditorGUILayout.Toggle("  Z Axis (Roll)", randomRotationZ);

            EditorGUILayout.Space(5);
            randomScale = EditorGUILayout.Toggle("Random Scale", randomScale);
            if (randomScale)
            {
                scaleRange = EditorGUILayout.Vector2Field("  Scale Range", scaleRange);
            }
        }

        private void DrawDebugSettings()
        {
            GUILayout.Label("🔧 Debug", EditorStyles.boldLabel);
            enableDebugLogs = EditorGUILayout.Toggle("Enable Debug Logs", enableDebugLogs);
        }

        private void DrawInstructions()
        {
            GUILayout.Label("📖 Instructions", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "🎮 CONTROLES:\n" +
                "• Ativar modo e clicar na Scene View\n" +
                "• Arrastar para pintura/apagar contínuo\n" +
                "• Teclas 1-3 = Trocar prefab ativo\n\n" +
                "🎯 MODOS:\n" +
                "• Brush: Pinta objetos do slot ativo\n" +
                "• Eraser: Apaga qualquer objeto pintado\n" +
                "• Sel.Eraser: Apaga apenas objetos dos slots\n\n" +
                "💡 DICAS:\n" +
                "• Gizmos coloridos mostram área de ação\n" +
                "• Objetos são organizados em 'SprayedObjects'\n" +
                "• Use Min Spacing para evitar sobreposição",
                MessageType.Info
            );

            // Quick test button
            EditorGUILayout.Space(5);
            if (GUILayout.Button("🧪 Criar Setup de Teste"))
            {
                CreateTestSetup();
            }
        }

        private void CreateTestSetup()
        {
            // Create test prefab
            GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            testCube.name = "TestCube";
            testCube.transform.localScale = Vector3.one * 0.5f;

            // Add to first empty slot
            for (int i = 0; i < 3; i++)
            {
                if (prefabSlots[i] == null)
                {
                    prefabSlots[i] = testCube;
                    selectedPrefabIndex = i;
                    break;
                }
            }

            // Create ground plane if needed
            if (GameObject.Find("TestGround") == null)
            {
                GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
                ground.name = "TestGround";
                ground.transform.localScale = Vector3.one * 5f;
            }

            DebugLog("Setup de teste criado!");
        }
        #endregion

        #region Scene GUI
        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isBrushActive && !isEraserActive && !isSelectiveEraserActive) return;

            Event currentEvent = Event.current;
            Vector3 mouseWorldPosition = GetMouseWorldPosition();

            HandleKeyboardInput(currentEvent);

            // Handle different modes
            if (isBrushActive)
            {
                HandleBrushMode(sceneView, mouseWorldPosition, currentEvent);
            }
            else if (isEraserActive)
            {
                HandleEraserMode(sceneView, mouseWorldPosition, currentEvent, false);
            }
            else if (isSelectiveEraserActive)
            {
                HandleEraserMode(sceneView, mouseWorldPosition, currentEvent, true);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);

            // Try to hit something in the scene
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.point;
            }

            // If no hit, project onto Z=0 plane (2D XY plane)
            Plane plane = new Plane(Vector3.forward, Vector3.zero);
            if (plane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            return ray.GetPoint(10f);
        }

        private void HandleKeyboardInput(Event e)
        {
            if (e.type == EventType.KeyDown)
            {
                // Switch prefab with number keys 1-9 (dynamic based on slot count)
                if (e.keyCode >= KeyCode.Alpha1 && e.keyCode <= KeyCode.Alpha9)
                {
                    int index = (int)e.keyCode - (int)KeyCode.Alpha1;
                    if (index < prefabSlots.Count && prefabSlots[index] != null)
                    {
                        SafeSelectedPrefabIndex = index;
                        DebugLog($"Prefab ativo: {prefabSlots[SafeSelectedPrefabIndex].name}");
                        e.Use();
                    }
                }
            }
        }

        private void HandleBrushMode(SceneView sceneView, Vector3 mouseWorldPosition, Event currentEvent)
        {
            // Draw brush gizmo
            if (currentEvent.type == EventType.Repaint)
            {
                DrawBrushGizmo(mouseWorldPosition, brushRadius);
            }

            // Force repaint on mouse move
            if (currentEvent.type == EventType.MouseMove)
            {
                sceneView.Repaint();
            }

            // Handle painting
            if (currentEvent.button == 0 && (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag))
            {
                PaintObjects(mouseWorldPosition);
                currentEvent.Use();
                sceneView.Repaint();
            }
        }

        private void HandleEraserMode(SceneView sceneView, Vector3 mouseWorldPosition, Event currentEvent, bool selectiveMode)
        {
            // Draw eraser gizmo
            if (currentEvent.type == EventType.Repaint)
            {
                DrawEraserGizmo(mouseWorldPosition, brushRadius, selectiveMode);
            }

            // Force repaint on mouse move
            if (currentEvent.type == EventType.MouseMove)
            {
                sceneView.Repaint();
            }

            // Handle erasing
            if (currentEvent.button == 0 && (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag))
            {
                EraseObjectsInRadius(mouseWorldPosition, brushRadius, selectiveMode);
                currentEvent.Use();
                sceneView.Repaint();
            }
        }

        private void DrawBrushGizmo(Vector3 position, float radius)
        {
            // Blue/cyan color for brush
            Handles.color = new Color(0.2f, 0.8f, 1.0f, 0.8f);
            Handles.DrawWireDisc(position, Vector3.forward, radius);

            Handles.color = new Color(0.2f, 0.8f, 1.0f, 0.1f);
            Handles.DrawSolidDisc(position, Vector3.forward, radius);

            // Draw center point
            Handles.color = new Color(0.2f, 0.8f, 1.0f, 1.0f);
            Handles.DrawWireDisc(position, Vector3.forward, 0.2f);
        }

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
        #endregion

        #region Painting Logic
        private void PaintObjects(Vector3 center)
        {
            List<GameObject> validPrefabs = new List<GameObject>();

            if (paintAllSlots)
            {
                // Get all valid prefabs (non-null)
                validPrefabs = prefabSlots.Where(slot => slot != null).ToList();
                if (validPrefabs.Count == 0)
                {
                    DebugLog("Nenhum prefab válido encontrado nos slots");
                    return;
                }
            }
            else
            {
                // Use only selected prefab
                int safeIndex = SafeSelectedPrefabIndex;
                if (safeIndex >= prefabSlots.Count || prefabSlots[safeIndex] == null)
                {
                    DebugLog("Nenhum prefab válido selecionado");
                    return;
                }
                validPrefabs.Add(prefabSlots[safeIndex]);
            }

            // Check spacing from last paint position
            if (lastPaintPosition != Vector3.zero && Vector3.Distance(center, lastPaintPosition) < minSpacing)
            {
                return;
            }

            lastPaintPosition = center;

            // Calculate number of objects based on density
            int objectCount = Mathf.RoundToInt(brushDensity * brushRadius * 0.5f);
            objectCount = Mathf.Max(1, objectCount);

            for (int i = 0; i < objectCount; i++)
            {
                // Choose random prefab from valid ones
                GameObject prefab = validPrefabs[Random.Range(0, validPrefabs.Count)];

                // Get appropriate parent (main or type-specific)
                GameObject parentForPrefab = GetOrCreateParentForPrefab(prefab);

                // Random position within brush radius (2D XY plane)
                Vector2 randomCircle = Random.insideUnitCircle * brushRadius;
                Vector3 spawnPosition = center + new Vector3(randomCircle.x, randomCircle.y, 0);

                // Create object
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.position = spawnPosition;
                instance.transform.SetParent(parentForPrefab.transform);

                // Apply randomization
                ApplyRandomization(instance, prefab);

                // Register for undo
                Undo.RegisterCreatedObjectUndo(instance, "Paint GameObject");
            }
            if (paintAllSlots)
            {
                DebugLog($"Pintados {objectCount} objetos aleatórios entre {validPrefabs.Count} prefabs");
            }
            else
            {
                DebugLog($"Pintados {objectCount} objetos de {validPrefabs[0].name}");
            }
        }

        private void ApplyRandomization(GameObject instance, GameObject prefab)
        {
            // Random rotation
            if (randomRotationX || randomRotationY || randomRotationZ)
            {
                Vector3 rotation = Vector3.zero;
                if (randomRotationY) rotation.y = Random.Range(0f, 360f);
                if (randomRotationX) rotation.x = Random.Range(0f, 360f);
                if (randomRotationZ) rotation.z = Random.Range(0f, 360f);
                instance.transform.rotation = Quaternion.Euler(rotation);
            }

            // Random scale
            if (randomScale)
            {
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                instance.transform.localScale = prefab.transform.localScale * scale;
            }
        }

        private void EraseObjectsInRadius(Vector3 center, float radius, bool selectiveMode)
        {
            // Find the parent object using the configured name
            GameObject parentObject = GameObject.Find(parentObjectName);
            if (parentObject == null)
            {
                // If specific parent doesn't exist, try to find any painted objects in the scene
                parentObject = FindAnyPaintedObjectsParent();
                if (parentObject == null)
                {
                    DebugLog($"Nenhum objeto pai '{parentObjectName}' encontrado para apagar");
                    return;
                }
            }

            List<GameObject> objectsToDelete = new List<GameObject>();
            float radiusSquared = radius * radius;

            // If groupByType is enabled, check all child group containers
            if (groupByType)
            {
                // Check all children of the main parent (these are group containers)
                foreach (Transform groupContainer in parentObject.transform)
                {
                    // Check all children of each group container
                    foreach (Transform child in groupContainer.transform)
                    {
                        CheckAndAddForDeletion(child, center, radiusSquared, selectiveMode, objectsToDelete);
                    }
                }
            }
            else
            {
                // Check all children of the main parent directly
                foreach (Transform child in parentObject.transform)
                {
                    CheckAndAddForDeletion(child, center, radiusSquared, selectiveMode, objectsToDelete);
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
                DebugLog($"Apagados {objectsToDelete.Count} objetos");
            }
            else
            {
                DebugLog("Nenhum objeto encontrado no raio para apagar");
            }
        }

        private GameObject FindAnyPaintedObjectsParent()
        {
            // Try common parent names
            string[] commonNames = { "PaintedObjects", "SprayedObjects", "BrushedObjects", "PlacedObjects" };

            foreach (string name in commonNames)
            {
                GameObject found = GameObject.Find(name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private void CheckAndAddForDeletion(Transform child, Vector3 center, float radiusSquared, bool selectiveMode, List<GameObject> objectsToDelete)
        {
            Vector3 childPosition = child.position;
            float sqrDistance = (childPosition - center).sqrMagnitude;

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

        private GameObject GetOrCreateParentForPrefab(GameObject prefab)
        {
            if (!groupByType)
            {
                // Use main parent only
                return GetMainParent();
            }

            // Check if we already have a parent for this prefab type
            if (prefabParents.ContainsKey(prefab) && prefabParents[prefab] != null)
            {
                return prefabParents[prefab];
            }

            // Create or find type-specific parent
            GameObject mainParent = GetMainParent();
            string typeName = prefab.name.Replace("(Clone)", "").Trim();

            GameObject typeParent = null;
            foreach (Transform child in mainParent.transform)
            {
                if (child.name == typeName)
                {
                    typeParent = child.gameObject;
                    break;
                }
            }

            if (typeParent == null)
            {
                typeParent = new GameObject(typeName);
                typeParent.transform.SetParent(mainParent.transform);
                DebugLog($"Criado parent para tipo: {typeName}");
            }

            prefabParents[prefab] = typeParent;
            return typeParent;
        }

        private GameObject GetMainParent()
        {
            if (sprayedObjectsParent == null || sprayedObjectsParent.name != parentObjectName)
            {
                sprayedObjectsParent = GameObject.Find(parentObjectName);
                if (sprayedObjectsParent == null)
                {
                    sprayedObjectsParent = new GameObject(parentObjectName);
                    DebugLog($"Criado parent principal: {parentObjectName}");
                }
            }
            return sprayedObjectsParent;
        }
        #endregion

        #region Utility Methods
        private void DebugLog(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[GameObjectBrush] {message}");
            }
        }
        #endregion
    }
}