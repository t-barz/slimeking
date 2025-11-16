#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Editor tool for automatically setting up scenes with essential components
    /// required for scene transitions and teleportation system.
    /// </summary>
    public class SceneSetupTool
    {
        #region Logging Methods

        /// <summary>
        /// Logs an informational message to the console.
        /// </summary>
        /// <param name="message">The message to log</param>
        private static void Log(string message)
        {
            UnityEngine.Debug.Log($"[SceneSetupTool] {message}");
        }

        /// <summary>
        /// Logs a warning message to the console.
        /// </summary>
        /// <param name="message">The warning message to log</param>
        private static void LogWarning(string message)
        {
            UnityEngine.Debug.LogWarning($"[SceneSetupTool] {message}");
        }

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="message">The error message to log</param>
        private static void LogError(string message)
        {
            UnityEngine.Debug.LogError($"[SceneSetupTool] {message}");
        }

        #endregion

        #region Component Detection Methods

        /// <summary>
        /// Searches the scene hierarchy for a GameObject by name.
        /// </summary>
        /// <param name="name">The name of the GameObject to find</param>
        /// <returns>The GameObject if found, null otherwise</returns>
        private static GameObject FindGameObjectByName(string name)
        {
            // Get all root GameObjects in the active scene
            Scene activeScene = SceneManager.GetActiveScene();
            GameObject[] rootObjects = activeScene.GetRootGameObjects();

            // Search through all GameObjects in the scene hierarchy
            foreach (GameObject rootObject in rootObjects)
            {
                // Check if the root object matches
                if (rootObject.name == name)
                {
                    return rootObject;
                }

                // Search children recursively
                Transform found = rootObject.transform.Find(name);
                if (found != null)
                {
                    return found.gameObject;
                }

                // Deep search through all descendants
                Transform[] allChildren = rootObject.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.gameObject.name == name)
                    {
                        return child.gameObject;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the first component of the specified type in the scene.
        /// </summary>
        /// <typeparam name="T">The type of component to find</typeparam>
        /// <returns>The component if found, null otherwise</returns>
        private static T FindComponentInScene<T>() where T : Component
        {
            // Use Unity's FindFirstObjectByType to locate the component
            T component = Object.FindFirstObjectByType<T>();
            return component;
        }

        /// <summary>
        /// Ensures a GameObject with the specified name and component exists in the scene.
        /// Creates a new GameObject if it doesn't exist.
        /// </summary>
        /// <param name="name">The name of the GameObject</param>
        /// <param name="componentType">The type of component to ensure on the GameObject</param>
        /// <returns>The GameObject with the specified component</returns>
        private static GameObject EnsureGameObject(string name, System.Type componentType)
        {
            // Check if GameObject already exists
            GameObject go = FindGameObjectByName(name);

            if (go == null)
            {
                // Create new GameObject at origin
                go = new GameObject(name);
                go.transform.position = Vector3.zero;
                Log($"Created new GameObject: {name}");
            }
            else
            {
                Log($"Found existing GameObject: {name}");
            }

            // Ensure the component is attached
            if (go.GetComponent(componentType) == null)
            {
                go.AddComponent(componentType);
                Log($"Added {componentType.Name} component to {name}");
            }
            else
            {
                Log($"{name} already has {componentType.Name} component");
            }

            return go;
        }

        /// <summary>
        /// Ensures a component of the specified type exists on the GameObject.
        /// Adds the component if it's missing.
        /// </summary>
        /// <typeparam name="T">The type of component to ensure</typeparam>
        /// <param name="go">The GameObject to check</param>
        /// <returns>The component reference</returns>
        private static T EnsureComponent<T>(GameObject go) where T : Component
        {
            // Check if component already exists
            T component = go.GetComponent<T>();

            if (component == null)
            {
                // Add the component
                component = go.AddComponent<T>();
                Log($"Added {typeof(T).Name} component to {go.name}");
            }
            else
            {
                Log($"{go.name} already has {typeof(T).Name} component");
            }

            return component;
        }

        #endregion

        #region Component Setup Methods

        /// <summary>
        /// Sets up the GameManager in the scene.
        /// Finds or creates "GameManager" GameObject and ensures GameManager component is attached.
        /// </summary>
        /// <param name="wasCreated">Output parameter indicating if the component was created (true) or already existed (false)</param>
        /// <returns>True if setup was successful, false otherwise</returns>
        private static bool SetupGameManager(out bool wasCreated)
        {
            wasCreated = false;

            try
            {
                Log("Setting up GameManager...");

                // Check if GameManager already exists in the scene
                GameObject gameManagerObj = FindGameObjectByName("GameManager");
                bool objectWasCreated = false;

                if (gameManagerObj == null)
                {
                    // Create new GameObject
                    gameManagerObj = new GameObject("GameManager");
                    gameManagerObj.transform.position = Vector3.zero;
                    objectWasCreated = true;
                    Log("Created new GameObject: GameManager");
                }
                else
                {
                    Log("Found existing GameObject: GameManager");
                }

                // Ensure GameManager component is attached
                GameManager gameManager = gameManagerObj.GetComponent<GameManager>();
                if (gameManager == null)
                {
                    gameManagerObj.AddComponent<GameManager>();
                    wasCreated = true;
                    Log("Added GameManager component");
                }
                else
                {
                    Log("GameManager component already exists");
                }

                // If we created the object but component existed, still count as created
                if (objectWasCreated)
                {
                    wasCreated = true;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to setup GameManager: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sets up the SceneTransitioner in the scene.
        /// Finds or creates "SceneTransitioner" GameObject and ensures SceneTransitionManager component is attached.
        /// </summary>
        /// <param name="wasCreated">Output parameter indicating if the component was created (true) or already existed (false)</param>
        /// <returns>True if setup was successful, false otherwise</returns>
        private static bool SetupSceneTransitioner(out bool wasCreated)
        {
            wasCreated = false;

            try
            {
                Log("Setting up SceneTransitioner...");

                // Check if SceneTransitioner already exists in the scene
                GameObject sceneTransitionerObj = FindGameObjectByName("SceneTransitioner");
                bool objectWasCreated = false;

                if (sceneTransitionerObj == null)
                {
                    // Create new GameObject
                    sceneTransitionerObj = new GameObject("SceneTransitioner");
                    sceneTransitionerObj.transform.position = Vector3.zero;
                    objectWasCreated = true;
                    Log("Created new GameObject: SceneTransitioner");
                }
                else
                {
                    Log("Found existing GameObject: SceneTransitioner");
                }

                // Ensure SceneTransitionManager component is attached
                SlimeKing.Core.SceneTransitionManager sceneTransitionManager = sceneTransitionerObj.GetComponent<SlimeKing.Core.SceneTransitionManager>();
                if (sceneTransitionManager == null)
                {
                    sceneTransitionerObj.AddComponent<SlimeKing.Core.SceneTransitionManager>();
                    wasCreated = true;
                    Log("Added SceneTransitionManager component");
                }
                else
                {
                    Log("SceneTransitionManager component already exists");
                }

                // If we created the object but component existed, still count as created
                if (objectWasCreated)
                {
                    wasCreated = true;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to setup SceneTransitioner: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sets up the TeleportManager in the scene.
        /// Finds or creates "TeleportManager" GameObject and ensures TeleportManager component is attached.
        /// </summary>
        /// <param name="wasCreated">Output parameter indicating if the component was created (true) or already existed (false)</param>
        /// <returns>True if setup was successful, false otherwise</returns>
        private static bool SetupTeleportManager(out bool wasCreated)
        {
            wasCreated = false;

            try
            {
                Log("Setting up TeleportManager...");

                // Check if TeleportManager already exists in the scene
                GameObject teleportManagerObj = FindGameObjectByName("TeleportManager");
                bool objectWasCreated = false;

                if (teleportManagerObj == null)
                {
                    // Create new GameObject
                    teleportManagerObj = new GameObject("TeleportManager");
                    teleportManagerObj.transform.position = Vector3.zero;
                    objectWasCreated = true;
                    Log("Created new GameObject: TeleportManager");
                }
                else
                {
                    Log("Found existing GameObject: TeleportManager");
                }

                // Ensure TeleportManager component is attached
                PixeLadder.EasyTransition.TeleportManager teleportManager = teleportManagerObj.GetComponent<PixeLadder.EasyTransition.TeleportManager>();
                if (teleportManager == null)
                {
                    teleportManagerObj.AddComponent<PixeLadder.EasyTransition.TeleportManager>();
                    wasCreated = true;
                    Log("Added TeleportManager component");
                }
                else
                {
                    Log("TeleportManager component already exists");
                }

                // If we created the object but component existed, still count as created
                if (objectWasCreated)
                {
                    wasCreated = true;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to setup TeleportManager: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sets up the EventSystem in the scene.
        /// Finds or creates "EventSystem" GameObject and ensures EventSystem and InputSystemUIInputModule components are attached.
        /// </summary>
        /// <param name="wasCreated">Output parameter indicating if the component was created (true) or already existed (false)</param>
        /// <returns>True if setup was successful, false otherwise</returns>
        private static bool SetupEventSystem(out bool wasCreated)
        {
            wasCreated = false;

            try
            {
                Log("Setting up EventSystem...");

                // First, try to find existing EventSystem component in the scene
                UnityEngine.EventSystems.EventSystem existingEventSystem = FindComponentInScene<UnityEngine.EventSystems.EventSystem>();
                GameObject eventSystemObj;
                bool objectWasCreated = false;

                if (existingEventSystem != null)
                {
                    // Use existing EventSystem GameObject
                    eventSystemObj = existingEventSystem.gameObject;
                    Log($"Found existing EventSystem on GameObject: {eventSystemObj.name}");
                }
                else
                {
                    // Check if there's a GameObject named "EventSystem" without the component
                    eventSystemObj = FindGameObjectByName("EventSystem");

                    if (eventSystemObj == null)
                    {
                        // Create new GameObject
                        eventSystemObj = new GameObject("EventSystem");
                        eventSystemObj.transform.position = Vector3.zero;
                        objectWasCreated = true;
                        Log("Created new GameObject: EventSystem");
                    }
                    else
                    {
                        Log("Found existing GameObject: EventSystem");
                    }

                    // Add EventSystem component
                    eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    wasCreated = true;
                    Log("Added EventSystem component");
                }

                // Ensure InputSystemUIInputModule component is attached
                UnityEngine.InputSystem.UI.InputSystemUIInputModule inputModule = eventSystemObj.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                if (inputModule == null)
                {
                    eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                    wasCreated = true;
                    Log("Added InputSystemUIInputModule component");
                }
                else
                {
                    Log("InputSystemUIInputModule component already exists");
                }

                // If we created the object but components existed, still count as created
                if (objectWasCreated)
                {
                    wasCreated = true;
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to setup EventSystem: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sets up the SimpleCameraFollow component on the Main Camera.
        /// Ensures the camera follows the player automatically.
        /// </summary>
        /// <param name="wasCreated">Output parameter indicating if the component was created (true) or already existed (false)</param>
        /// <returns>True if setup was successful, false otherwise</returns>
        private static bool SetupCameraFollow(out bool wasCreated)
        {
            wasCreated = false;

            try
            {
                Log("Setting up Camera Follow...");

                // Find the Main Camera
                Camera mainCamera = Camera.main;
                if (mainCamera == null)
                {
                    // Try to find any camera
                    mainCamera = Object.FindFirstObjectByType<Camera>();
                }

                if (mainCamera == null)
                {
                    LogWarning("No camera found in scene. Camera Follow not added.");
                    return false;
                }

                // Check if SimpleCameraFollow already exists
                var cameraFollow = mainCamera.GetComponent<SlimeKing.Core.SimpleCameraFollow>();
                if (cameraFollow == null)
                {
                    mainCamera.gameObject.AddComponent<SlimeKing.Core.SimpleCameraFollow>();
                    wasCreated = true;
                    Log($"Added SimpleCameraFollow component to {mainCamera.name}");
                }
                else
                {
                    Log($"{mainCamera.name} already has SimpleCameraFollow component");
                }

                return true;
            }
            catch (System.Exception ex)
            {
                LogError($"Failed to setup Camera Follow: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Main Menu Item and Orchestration

        /// <summary>
        /// Main menu item for setting up a scene with all essential components for transitions.
        /// Validates that a scene is open and orchestrates the setup process.
        /// </summary>
        [MenuItem("Extra Tools/Scene/🎬 Setup Scene for Transitions")]
        public static void SetupSceneForTransitions()
        {
            // Validate that a scene is open
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || string.IsNullOrEmpty(activeScene.name))
            {
                LogError("No scene is currently open. Please open a scene before running this tool.");
                EditorUtility.DisplayDialog(
                    "No Scene Open",
                    "Please open a scene before running the Scene Setup Tool.",
                    "OK"
                );
                return;
            }

            // Log start of setup process
            Log("========================================");
            Log($"Starting scene setup for: {activeScene.name}");
            Log("========================================");

            // Track components added and existing
            int totalAdded = 0;
            int totalExisting = 0;

            // Setup GameManager
            bool gameManagerCreated;
            if (SetupGameManager(out gameManagerCreated))
            {
                if (gameManagerCreated)
                    totalAdded++;
                else
                    totalExisting++;
            }

            // Setup SceneTransitioner
            bool sceneTransitionerCreated;
            if (SetupSceneTransitioner(out sceneTransitionerCreated))
            {
                if (sceneTransitionerCreated)
                    totalAdded++;
                else
                    totalExisting++;
            }

            // Setup TeleportManager
            bool teleportManagerCreated;
            if (SetupTeleportManager(out teleportManagerCreated))
            {
                if (teleportManagerCreated)
                    totalAdded++;
                else
                    totalExisting++;
            }

            // Setup EventSystem
            bool eventSystemCreated;
            if (SetupEventSystem(out eventSystemCreated))
            {
                if (eventSystemCreated)
                    totalAdded++;
                else
                    totalExisting++;
            }

            // Setup Camera Follow
            bool cameraFollowCreated;
            if (SetupCameraFollow(out cameraFollowCreated))
            {
                if (cameraFollowCreated)
                    totalAdded++;
                else
                    totalExisting++;
            }

            // Mark scene as dirty so changes can be saved
            EditorSceneManager.MarkSceneDirty(activeScene);

            // Show summary
            ShowSummary(totalAdded, totalExisting);

            Log("========================================");
            Log("Scene setup completed!");
            Log("========================================");
        }

        /// <summary>
        /// Displays a summary dialog and logs the final results of the setup process.
        /// </summary>
        /// <param name="added">Number of components that were added</param>
        /// <param name="existing">Number of components that already existed</param>
        private static void ShowSummary(int added, int existing)
        {
            // Build summary message
            string message = $"Setup da Cena Concluído!\n\n";
            message += $"✅ Componentes Adicionados: {added}\n";
            message += $"✅ Componentes Existentes: {existing}\n\n";
            message += "A cena está pronta para transições!\n";
            message += "A câmera seguirá o player automaticamente.";

            // Show dialog
            EditorUtility.DisplayDialog(
                "Scene Setup Complete",
                message,
                "OK"
            );

            // Log summary to console
            Log($"Summary: {added} component(s) added, {existing} component(s) already existed");
        }

        #endregion
    }
}
#endif
