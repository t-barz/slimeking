using UnityEngine;
using UnityEditor;
using TMPro;
using TheSlimeKing.UI;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Editor utility to integrate CrystalHUDController into the CanvasHUD prefab.
    /// This script adds the component and automatically configures all TextMeshProUGUI references.
    /// </summary>
    public class CrystalHUDIntegration : EditorWindow
    {
        [MenuItem("Extra Tools/Setup/Integrate CrystalHUDController")]
        public static void IntegrateCrystalHUD()
        {
            // Load the CanvasHUD prefab
            string prefabPath = "Assets/Game/Prefabs/UI/CanvasHUD.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"[CrystalHUDIntegration] CanvasHUD prefab not found at {prefabPath}");
                return;
            }

            // Check if component already exists
            CrystalHUDController existingController = prefab.GetComponent<CrystalHUDController>();
            if (existingController != null)
            {
                Debug.LogWarning("[CrystalHUDIntegration] CrystalHUDController already exists on CanvasHUD prefab. Updating references...");
            }

            // Open prefab for editing
            string assetPath = AssetDatabase.GetAssetPath(prefab);
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(assetPath);

            try
            {
                // Add or get the CrystalHUDController component
                CrystalHUDController controller = prefabInstance.GetComponent<CrystalHUDController>();
                if (controller == null)
                {
                    controller = prefabInstance.AddComponent<CrystalHUDController>();
                    Debug.Log("[CrystalHUDIntegration] Added CrystalHUDController component to CanvasHUD");
                }

                // Find CrystalContainer
                Transform crystalContainer = prefabInstance.transform.Find("CrystalContainer");
                if (crystalContainer == null)
                {
                    Debug.LogError("[CrystalHUDIntegration] CrystalContainer not found in CanvasHUD");
                    return;
                }

                // Auto-configure text references using reflection
                AutoConfigureTextReferences(controller, crystalContainer);

                // Save the prefab
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, assetPath);
                Debug.Log($"[CrystalHUDIntegration] Successfully integrated CrystalHUDController into {prefabPath}");
                Debug.Log("[CrystalHUDIntegration] All TextMeshProUGUI references configured automatically");
            }
            finally
            {
                // Unload the prefab instance
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }

            // Refresh the asset database
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog(
                "Integration Complete",
                "CrystalHUDController has been successfully integrated into CanvasHUD prefab.\n\n" +
                "All TextMeshProUGUI references have been configured automatically.\n\n" +
                "You can now test the integration in Play Mode.",
                "OK"
            );
        }

        private static void AutoConfigureTextReferences(CrystalHUDController controller, Transform crystalContainer)
        {
            // Use SerializedObject to modify private fields
            SerializedObject serializedController = new SerializedObject(controller);

            // Configure each crystal type
            ConfigureCrystalText(serializedController, "natureCountText", crystalContainer, "Crystal_Nature");
            ConfigureCrystalText(serializedController, "fireCountText", crystalContainer, "Crystal_Fire");
            ConfigureCrystalText(serializedController, "waterCountText", crystalContainer, "Crystal_Water");
            ConfigureCrystalText(serializedController, "shadowCountText", crystalContainer, "Crystal_Shadow");
            ConfigureCrystalText(serializedController, "earthCountText", crystalContainer, "Crystal_Earth");
            ConfigureCrystalText(serializedController, "airCountText", crystalContainer, "Crystal_Air");

            // Apply changes
            serializedController.ApplyModifiedProperties();
        }

        private static void ConfigureCrystalText(SerializedObject serializedController, string fieldName, Transform crystalContainer, string crystalName)
        {
            Transform crystalTransform = crystalContainer.Find(crystalName);
            if (crystalTransform == null)
            {
                Debug.LogWarning($"[CrystalHUDIntegration] {crystalName} not found in CrystalContainer");
                return;
            }

            Transform countTextTransform = crystalTransform.Find("Count_Text");
            if (countTextTransform == null)
            {
                Debug.LogWarning($"[CrystalHUDIntegration] Count_Text not found in {crystalName}");
                return;
            }

            TextMeshProUGUI textComponent = countTextTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent == null)
            {
                Debug.LogWarning($"[CrystalHUDIntegration] TextMeshProUGUI component not found on Count_Text in {crystalName}");
                return;
            }

            // Set the field value
            SerializedProperty property = serializedController.FindProperty(fieldName);
            if (property != null)
            {
                property.objectReferenceValue = textComponent;
                Debug.Log($"[CrystalHUDIntegration] Configured {fieldName} -> {crystalName}/Count_Text");
            }
            else
            {
                Debug.LogWarning($"[CrystalHUDIntegration] Field {fieldName} not found in CrystalHUDController");
            }
        }

        [MenuItem("Extra Tools/Setup/Verify CrystalHUDController Integration")]
        public static void VerifyIntegration()
        {
            // Load the CanvasHUD prefab
            string prefabPath = "Assets/Game/Prefabs/UI/CanvasHUD.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"[CrystalHUDIntegration] CanvasHUD prefab not found at {prefabPath}");
                return;
            }

            CrystalHUDController controller = prefab.GetComponent<CrystalHUDController>();
            if (controller == null)
            {
                Debug.LogError("[CrystalHUDIntegration] CrystalHUDController not found on CanvasHUD prefab");
                EditorUtility.DisplayDialog(
                    "Verification Failed",
                    "CrystalHUDController component not found on CanvasHUD prefab.\n\n" +
                    "Please run 'SlimeKing > Setup > Integrate CrystalHUDController' first.",
                    "OK"
                );
                return;
            }

            // Verify all references using SerializedObject
            SerializedObject serializedController = new SerializedObject(controller);
            bool allReferencesValid = true;
            string[] fieldNames = { "natureCountText", "fireCountText", "waterCountText", "shadowCountText", "earthCountText", "airCountText" };

            foreach (string fieldName in fieldNames)
            {
                SerializedProperty property = serializedController.FindProperty(fieldName);
                if (property == null || property.objectReferenceValue == null)
                {
                    Debug.LogError($"[CrystalHUDIntegration] {fieldName} is not configured!");
                    allReferencesValid = false;
                }
                else
                {
                    Debug.Log($"[CrystalHUDIntegration] ✓ {fieldName} is configured");
                }
            }

            if (allReferencesValid)
            {
                Debug.Log("[CrystalHUDIntegration] ✓ All references are properly configured!");
                EditorUtility.DisplayDialog(
                    "Verification Successful",
                    "CrystalHUDController is properly integrated!\n\n" +
                    "All 6 TextMeshProUGUI references are configured:\n" +
                    "✓ Nature\n✓ Fire\n✓ Water\n✓ Shadow\n✓ Earth\n✓ Air\n\n" +
                    "You can now test in Play Mode.",
                    "OK"
                );
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "Verification Failed",
                    "Some TextMeshProUGUI references are missing.\n\n" +
                    "Please check the Console for details and run the integration again.",
                    "OK"
                );
            }
        }
    }
}
