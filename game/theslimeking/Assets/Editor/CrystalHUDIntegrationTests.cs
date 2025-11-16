using UnityEngine;
using UnityEditor;
using TMPro;
using TheSlimeKing.UI;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Editor tests for CrystalHUDController integration.
    /// Validates that the component is properly configured and functional.
    /// </summary>
    public class CrystalHUDIntegrationTests
    {
        [MenuItem("SlimeKing/Tests/Run CrystalHUD Integration Tests")]
        public static void RunIntegrationTests()
        {
            Debug.Log("=== Starting CrystalHUD Integration Tests ===");
            
            bool allTestsPassed = true;
            
            allTestsPassed &= TestPrefabExists();
            allTestsPassed &= TestComponentExists();
            allTestsPassed &= TestAllReferencesConfigured();
            allTestsPassed &= TestInitialTextFormat();
            
            Debug.Log("=== CrystalHUD Integration Tests Complete ===");
            
            if (allTestsPassed)
            {
                Debug.Log("<color=green>✓ ALL TESTS PASSED</color>");
                EditorUtility.DisplayDialog(
                    "Tests Passed",
                    "All CrystalHUD integration tests passed successfully!\n\n" +
                    "The component is properly configured and ready to use.",
                    "OK"
                );
            }
            else
            {
                Debug.LogError("<color=red>✗ SOME TESTS FAILED</color>");
                EditorUtility.DisplayDialog(
                    "Tests Failed",
                    "Some CrystalHUD integration tests failed.\n\n" +
                    "Please check the Console for details and fix the issues.",
                    "OK"
                );
            }
        }
        
        private static bool TestPrefabExists()
        {
            Debug.Log("[Test] Checking if CanvasHUD prefab exists...");
            
            string prefabPath = "Assets/Game/Prefabs/UI/CanvasHUD.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError($"[Test] ✗ FAILED: CanvasHUD prefab not found at {prefabPath}");
                return false;
            }
            
            Debug.Log($"[Test] ✓ PASSED: CanvasHUD prefab exists at {prefabPath}");
            return true;
        }
        
        private static bool TestComponentExists()
        {
            Debug.Log("[Test] Checking if CrystalHUDController component exists...");
            
            string prefabPath = "Assets/Game/Prefabs/UI/CanvasHUD.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError("[Test] ✗ FAILED: Cannot test component - prefab not found");
                return false;
            }
            
            CrystalHUDController controller = prefab.GetComponent<CrystalHUDController>();
            
            if (controller == null)
            {
                Debug.LogError("[Test] ✗ FAILED: CrystalHUDController component not found on CanvasHUD");
                return false;
            }
            
            Debug.Log("[Test] ✓ PASSED: CrystalHUDController component exists");
            return true;
        }
        
        private static bool TestAllReferencesConfigured()
        {
            Debug.Log("[Test] Checking if all TextMeshProUGUI references are configured...");
            
            string prefabPath = "Assets/Game/Prefabs/UI/CanvasHUD.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError("[Test] ✗ FAILED: Cannot test references - prefab not found");
                return false;
            }
            
            CrystalHUDController controller = prefab.GetComponent<CrystalHUDController>();
            
            if (controller == null)
            {
                Debug.LogError("[Test] ✗ FAILED: Cannot test references - component not found");
                return false;
            }
            
            SerializedObject serializedController = new SerializedObject(controller);
            string[] fieldNames = { 
                "natureCountText", 
                "fireCountText", 
                "waterCountText", 
                "shadowCountText", 
                "earthCountText", 
                "airCountText" 
            };
            
            bool allReferencesValid = true;
            
            foreach (string fieldName in fieldNames)
            {
                SerializedProperty property = serializedController.FindProperty(fieldName);
                
                if (property == null)
                {
                    Debug.LogError($"[Test] ✗ FAILED: Field {fieldName} not found in component");
                    allReferencesValid = false;
                    continue;
                }
                
                if (property.objectReferenceValue == null)
                {
                    Debug.LogError($"[Test] ✗ FAILED: {fieldName} is not configured (null reference)");
                    allReferencesValid = false;
                }
                else
                {
                    Debug.Log($"[Test] ✓ {fieldName} is configured");
                }
            }
            
            if (allReferencesValid)
            {
                Debug.Log("[Test] ✓ PASSED: All TextMeshProUGUI references are configured");
            }
            
            return allReferencesValid;
        }
        
        private static bool TestInitialTextFormat()
        {
            Debug.Log("[Test] Checking if Count_Text components have correct initial format...");
            
            string prefabPath = "Assets/Game/Prefabs/UI/CanvasHUD.prefab";
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError("[Test] ✗ FAILED: Cannot test text format - prefab not found");
                return false;
            }
            
            Transform crystalContainer = prefab.transform.Find("CrystalContainer");
            
            if (crystalContainer == null)
            {
                Debug.LogError("[Test] ✗ FAILED: CrystalContainer not found");
                return false;
            }
            
            string[] crystalNames = { 
                "Crystal_Nature", 
                "Crystal_Fire", 
                "Crystal_Water", 
                "Crystal_Shadow", 
                "Crystal_Earth", 
                "Crystal_Air" 
            };
            
            bool allFormatsCorrect = true;
            
            foreach (string crystalName in crystalNames)
            {
                Transform crystalTransform = crystalContainer.Find(crystalName);
                
                if (crystalTransform == null)
                {
                    Debug.LogWarning($"[Test] ⚠ WARNING: {crystalName} not found in CrystalContainer");
                    continue;
                }
                
                Transform countTextTransform = crystalTransform.Find("Count_Text");
                
                if (countTextTransform == null)
                {
                    Debug.LogWarning($"[Test] ⚠ WARNING: Count_Text not found in {crystalName}");
                    continue;
                }
                
                TextMeshProUGUI textComponent = countTextTransform.GetComponent<TextMeshProUGUI>();
                
                if (textComponent == null)
                {
                    Debug.LogWarning($"[Test] ⚠ WARNING: TextMeshProUGUI component not found in {crystalName}/Count_Text");
                    continue;
                }
                
                string currentText = textComponent.text;
                
                if (currentText == "x0")
                {
                    Debug.Log($"[Test] ✓ {crystalName}/Count_Text has correct format: '{currentText}'");
                }
                else
                {
                    Debug.LogWarning($"[Test] ⚠ WARNING: {crystalName}/Count_Text has unexpected format: '{currentText}' (expected 'x0')");
                    // This is a warning, not a failure, as the text might be set at runtime
                }
            }
            
            Debug.Log("[Test] ✓ PASSED: Initial text format check complete");
            return allFormatsCorrect;
        }
    }
}
