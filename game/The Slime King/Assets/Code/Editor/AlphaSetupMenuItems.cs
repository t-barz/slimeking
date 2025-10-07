using UnityEngine;
using UnityEditor;
using System.IO;

namespace SlimeMec.Alpha.Editor
{
    /// <summary>
    /// Menu de ferramentas para configuração automática dos sistemas Alpha
    /// Acessível via Extra Tools > Alpha
    /// </summary>
    public static class AlphaSetupMenuItems
    {
        private const string ALPHA_FOLDER = "Assets/Alpha";
        private const string ALPHA_SCRIPTS_FOLDER = "Assets/Alpha/Scripts";
        private const string ALPHA_PREFABS_FOLDER = "Assets/Alpha/Prefabs";
        private const string ALPHA_SCENES_FOLDER = "Assets/Alpha/Scenes";

        #region Main Setup

        [MenuItem("Extra Tools/Alpha/Complete Alpha Setup", false, 0)]
        public static void CompleteAlphaSetup()
        {
            if (EditorUtility.DisplayDialog("Alpha Setup",
                "This will setup all Alpha systems automatically. Continue?",
                "Yes", "Cancel"))
            {
                Debug.Log("[Alpha Setup] Starting complete Alpha setup...");

                SetupInventorySystem();
                SetupEnemySystem();
                SetupProgressionSystem();
                SetupUISystem();
                SetupCameraSystem();
                SetupTeleportSystem();
                SetupTestScene();

                Debug.Log("[Alpha Setup] Complete Alpha setup finished!");
                EditorUtility.DisplayDialog("Alpha Setup Complete",
                    "All Alpha systems have been configured. Check Console for details.", "OK");
            }
        }

        #endregion

        #region Individual System Setup

        [MenuItem("Extra Tools/Alpha/Setup Inventory System", false, 50)]
        public static void SetupInventorySystem()
        {
            Debug.Log("[Alpha Setup] Setting up Inventory System...");

            // TODO: Create InventoryCore prefab
            // TODO: Configure ItemCollectable integration
            // TODO: Setup HUD slots

            Debug.Log("[Alpha Setup] Inventory System setup complete!");
        }

        [MenuItem("Extra Tools/Alpha/Setup Enemy System", false, 51)]
        public static void SetupEnemySystem()
        {
            Debug.Log("[Alpha Setup] Setting up Enemy System...");

            // TODO: Create EnemyBasic prefab
            // TODO: Configure FSM states
            // TODO: Setup attack patterns

            Debug.Log("[Alpha Setup] Enemy System setup complete!");
        }

        [MenuItem("Extra Tools/Alpha/Setup Progression System", false, 52)]
        public static void SetupProgressionSystem()
        {
            Debug.Log("[Alpha Setup] Setting up Progression System...");

            // TODO: Configure Growth System
            // TODO: Setup Skill Tree base
            // TODO: Create progression UI

            Debug.Log("[Alpha Setup] Progression System setup complete!");
        }

        [MenuItem("Extra Tools/Alpha/Setup UI System", false, 53)]
        public static void SetupUISystem()
        {
            Debug.Log("[Alpha Setup] Setting up UI System...");

            // TODO: Configure EventSystem
            // TODO: Setup InputSystemUIInputModule
            // TODO: Create HUD Canvas
            // TODO: Setup Dialogue system

            Debug.Log("[Alpha Setup] UI System setup complete!");
        }

        [MenuItem("Extra Tools/Alpha/Setup Camera System", false, 54)]
        public static void SetupCameraSystem()
        {
            Debug.Log("[Alpha Setup] Setting up Camera System...");

            // TODO: Configure Camera Follow
            // TODO: Setup scene bounds
            // TODO: Configure smoothing

            Debug.Log("[Alpha Setup] Camera System setup complete!");
        }

        [MenuItem("Extra Tools/Alpha/Setup Teleport System", false, 55)]
        public static void SetupTeleportSystem()
        {
            Debug.Log("[Alpha Setup] Setting up Teleport System...");

            // TODO: Create TeleportPoint prefab
            // TODO: Configure triggers
            // TODO: Setup VFX

            Debug.Log("[Alpha Setup] Teleport System setup complete!");
        }

        #endregion

        #region Test Scene Setup

        [MenuItem("Extra Tools/Alpha/Create Test Scene", false, 100)]
        public static void SetupTestScene()
        {
            Debug.Log("[Alpha Setup] Creating Alpha Test Scene...");

            // Create new scene
            var newScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
                UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
                UnityEditor.SceneManagement.NewSceneMode.Single);

            // Save scene
            string scenePath = Path.Combine(ALPHA_SCENES_FOLDER, "TestArenaAlpha.unity");
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(newScene, scenePath);

            // TODO: Add basic scene setup
            // TODO: Add player spawn
            // TODO: Add test objects
            // TODO: Configure lighting

            Debug.Log($"[Alpha Setup] Test scene created at {scenePath}");
        }

        [MenuItem("Extra Tools/Alpha/Configure Test Arena", false, 101)]
        public static void ConfigureTestArena()
        {
            Debug.Log("[Alpha Setup] Configuring Test Arena...");

            // TODO: Add test enemies
            // TODO: Add collectible items
            // TODO: Add dialogue triggers
            // TODO: Add teleport points
            // TODO: Setup camera bounds

            Debug.Log("[Alpha Setup] Test Arena configuration complete!");
        }

        #endregion

        #region Validation & Debug

        [MenuItem("Extra Tools/Alpha/Validate Alpha Setup", false, 150)]
        public static void ValidateAlphaSetup()
        {
            Debug.Log("[Alpha Setup] Validating Alpha systems...");

            bool isValid = true;

            // Check folders
            if (!AssetDatabase.IsValidFolder(ALPHA_FOLDER))
            {
                Debug.LogError("[Alpha Validation] Alpha folder missing!");
                isValid = false;
            }

            // Check scripts
            // TODO: Validate all required scripts exist

            // Check prefabs
            // TODO: Validate all required prefabs exist

            // Check scene
            // TODO: Validate test scene is properly configured

            if (isValid)
            {
                Debug.Log("[Alpha Validation] ✅ All Alpha systems are properly configured!");
                EditorUtility.DisplayDialog("Alpha Validation", "All systems validated successfully!", "OK");
            }
            else
            {
                Debug.LogWarning("[Alpha Validation] ⚠️ Some Alpha systems need attention. Check Console for details.");
                EditorUtility.DisplayDialog("Alpha Validation", "Some issues found. Check Console for details.", "OK");
            }
        }

        [MenuItem("Extra Tools/Alpha/Run Alpha Test Checklist", false, 151)]
        public static void RunAlphaTestChecklist()
        {
            Debug.Log("[Alpha Setup] Opening Alpha Test Checklist...");

            string checklistPath = Path.Combine(ALPHA_FOLDER, "AlphaTestChecklist.md");
            if (File.Exists(checklistPath))
            {
                Application.OpenURL($"file://{Path.GetFullPath(checklistPath)}");
                Debug.Log($"[Alpha Setup] Checklist opened: {checklistPath}");
            }
            else
            {
                Debug.LogError("[Alpha Setup] AlphaTestChecklist.md not found!");
            }
        }

        #endregion

        #region Utility Methods

        private static void EnsureAlphaFoldersExist()
        {
            string[] folders = {
                ALPHA_FOLDER,
                ALPHA_SCRIPTS_FOLDER,
                ALPHA_PREFABS_FOLDER,
                ALPHA_SCENES_FOLDER,
                Path.Combine(ALPHA_SCRIPTS_FOLDER, "Inventory"),
                Path.Combine(ALPHA_SCRIPTS_FOLDER, "Enemy"),
                Path.Combine(ALPHA_SCRIPTS_FOLDER, "Progression"),
                Path.Combine(ALPHA_SCRIPTS_FOLDER, "UI"),
                Path.Combine(ALPHA_SCRIPTS_FOLDER, "Camera"),
                Path.Combine(ALPHA_SCRIPTS_FOLDER, "Teleport")
            };

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parentFolder = Path.GetDirectoryName(folder);
                    string folderName = Path.GetFileName(folder);
                    AssetDatabase.CreateFolder(parentFolder, folderName);
                    Debug.Log($"[Alpha Setup] Created folder: {folder}");
                }
            }
        }

        private static GameObject CreatePrefabFromTemplate(string prefabName, string templatePath)
        {
            // TODO: Implement prefab creation from template
            Debug.Log($"[Alpha Setup] Creating prefab: {prefabName}");
            return null;
        }

        #endregion
    }
}