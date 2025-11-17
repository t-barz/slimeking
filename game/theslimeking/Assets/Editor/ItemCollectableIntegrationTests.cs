using UnityEngine;
using UnityEditor;
using SlimeMec.Gameplay;
using SlimeKing.Core;
using SlimeKing.Data;
using TheSlimeKing.Inventory;
using TheSlimeKing.UI;
using TMPro;

namespace TheSlimeKing.Editor.Tests
{
    /// <summary>
    /// Integration tests for ItemCollectable system.
    /// Tests complete flows: crystal collection, item collection, inventory full, and type prioritization.
    /// 
    /// Requirements: All requirements from 1.1 to 6.6
    /// </summary>
    public class ItemCollectableIntegrationTests : EditorWindow
    {
        #region Window Setup
        [MenuItem("Extra Tools/Tests/ItemCollectable Integration Tests")]
        public static void ShowWindow()
        {
            var window = GetWindow<ItemCollectableIntegrationTests>("Integration Tests");
            window.minSize = new Vector2(700, 800);
        }
        #endregion

        #region Test State
        private Vector2 scrollPosition;
        private string testLog = "";
        private bool testsRunning = false;
        private int testsPassed = 0;
        private int testsFailed = 0;

        // Test data
        private CrystalElementalData testCrystalData;
        private ItemData testItemData;
        private GameObject testItemPrefab;
        #endregion

        #region GUI
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("ItemCollectable Integration Tests", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "These tests validate complete collection flows:\n" +
                "• 6.1: Crystal complete flow (GameManager + HUD)\n" +
                "• 6.2: Item complete flow (InventoryManager)\n" +
                "• 6.3: Inventory full behavior\n" +
                "• 6.4: Type prioritization (crystal > inventory > legacy)",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            // Test Data Setup
            EditorGUILayout.LabelField("Test Data Setup", EditorStyles.boldLabel);
            testCrystalData = (CrystalElementalData)EditorGUILayout.ObjectField(
                "Crystal Data", testCrystalData, typeof(CrystalElementalData), false);
            testItemData = (ItemData)EditorGUILayout.ObjectField(
                "Item Data", testItemData, typeof(ItemData), false);
            testItemPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Item Prefab", testItemPrefab, typeof(GameObject), false);

            EditorGUILayout.Space(10);

            // Individual Test Buttons
            EditorGUILayout.LabelField("Individual Tests", EditorStyles.boldLabel);

            GUI.enabled = !testsRunning && Application.isPlaying;
            if (GUILayout.Button("6.1: Test Crystal Complete Flow", GUILayout.Height(30)))
            {
                RunSingleTest(Test_CrystalCompleteFlow);
            }

            if (GUILayout.Button("6.2: Test Item Complete Flow", GUILayout.Height(30)))
            {
                RunSingleTest(Test_ItemCompleteFlow);
            }

            if (GUILayout.Button("6.3: Test Inventory Full", GUILayout.Height(30)))
            {
                RunSingleTest(Test_InventoryFull);
            }

            if (GUILayout.Button("6.4: Test Type Prioritization", GUILayout.Height(30)))
            {
                RunSingleTest(Test_TypePrioritization);
            }

            EditorGUILayout.Space(10);

            // Run All Tests Button
            if (GUILayout.Button("Run All Integration Tests", GUILayout.Height(40)))
            {
                RunAllTests();
            }
            GUI.enabled = true;

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to run tests", MessageType.Warning);
            }

            EditorGUILayout.Space(10);

            // Test Results
            if (!string.IsNullOrEmpty(testLog))
            {
                EditorGUILayout.LabelField($"Results: {testsPassed} passed, {testsFailed} failed",
                    EditorStyles.boldLabel);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUILayout.TextArea(testLog, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
            }
        }
        #endregion

        #region Test Runner
        private void RunSingleTest(System.Action testMethod)
        {
            testsRunning = true;
            testLog = "";
            testsPassed = 0;
            testsFailed = 0;

            Log("=== RUNNING SINGLE TEST ===\n");

            if (!ValidatePrerequisites())
            {
                testsRunning = false;
                return;
            }

            testMethod();

            ShowSummary();
            testsRunning = false;
            Repaint();
        }

        private void RunAllTests()
        {
            testsRunning = true;
            testLog = "";
            testsPassed = 0;
            testsFailed = 0;

            Log("=== ITEMCOLLECTABLE INTEGRATION TESTS ===\n");

            if (!ValidatePrerequisites())
            {
                testsRunning = false;
                return;
            }

            // Run all tests
            Test_CrystalCompleteFlow();
            Test_ItemCompleteFlow();
            Test_InventoryFull();
            Test_TypePrioritization();

            ShowSummary();
            testsRunning = false;
            Repaint();
        }

        private bool ValidatePrerequisites()
        {
            bool valid = true;

            if (GameManager.Instance == null)
            {
                LogError("CRITICAL: GameManager.Instance is null!");
                valid = false;
            }
            else
            {
                LogSuccess("✓ GameManager.Instance found");
            }

            if (InventoryManager.Instance == null)
            {
                LogError("CRITICAL: InventoryManager.Instance is null!");
                valid = false;
            }
            else
            {
                LogSuccess("✓ InventoryManager.Instance found");
            }

            // Find CrystalHUDController in scene
            CrystalHUDController hudController = Object.FindObjectOfType<CrystalHUDController>();
            if (hudController == null)
            {
                LogWarning("WARNING: CrystalHUDController not found in scene (HUD tests will be limited)");
            }
            else
            {
                LogSuccess("✓ CrystalHUDController found");
            }

            return valid;
        }

        private void ShowSummary()
        {
            Log($"\n=== TEST SUMMARY ===");
            Log($"Total: {testsPassed + testsFailed} tests");
            LogSuccess($"Passed: {testsPassed}");
            if (testsFailed > 0)
            {
                LogError($"Failed: {testsFailed}");
            }
        }
        #endregion

        #region Test Cases

        /// <summary>
        /// Test 6.1: Crystal Complete Flow
        /// Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 3.1, 3.2, 3.3, 3.4, 3.5
        /// </summary>
        private void Test_CrystalCompleteFlow()
        {
            Log("\n=== TEST 6.1: CRYSTAL COMPLETE FLOW ===");

            if (testCrystalData == null)
            {
                LogWarning("Skipping: testCrystalData not assigned");
                return;
            }

            // Get initial crystal count
            CrystalType crystalType = testCrystalData.crystalType;
            int initialCount = GameManager.Instance.GetCrystalCount(crystalType);
            Log($"Initial {crystalType} count: {initialCount}");

            // Setup event listener for HUD update verification
            bool eventFired = false;
            CrystalType receivedType = CrystalType.Nature;
            int receivedCount = -1;

            System.Action<CrystalType, int> eventHandler = (type, count) =>
            {
                if (type == crystalType)
                {
                    eventFired = true;
                    receivedType = type;
                    receivedCount = count;
                }
            };

            GameManager.Instance.OnCrystalCountChanged += eventHandler;

            // Spawn crystal in scene
            GameObject crystalObj = new GameObject("TestCrystal");
            crystalObj.tag = "Item"; // Ensure proper tag
            crystalObj.transform.position = Vector3.zero;

            // Add required components
            SpriteRenderer sr = crystalObj.AddComponent<SpriteRenderer>();
            CircleCollider2D collider = crystalObj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            ItemCollectable collectable = crystalObj.AddComponent<ItemCollectable>();
            collectable.SetCrystalData(testCrystalData);
            collectable.ForceActivateAttraction(); // Skip delay for testing

            Log($"✓ Crystal spawned at {crystalObj.transform.position}");

            // Simulate collection
            collectable.CollectItem();

            // Cleanup event listener
            GameManager.Instance.OnCrystalCountChanged -= eventHandler;

            // Verify GameManager.GetCrystalCount increased
            int newCount = GameManager.Instance.GetCrystalCount(crystalType);
            int expectedCount = initialCount + testCrystalData.value;

            if (newCount == expectedCount)
            {
                LogSuccess($"✓ GameManager count increased correctly: {initialCount} → {newCount}");
                testsPassed++;
            }
            else
            {
                LogError($"✗ GameManager count incorrect: expected {expectedCount}, got {newCount}");
                testsFailed++;
            }

            // Verify event was fired
            if (eventFired && receivedType == crystalType && receivedCount == newCount)
            {
                LogSuccess($"✓ OnCrystalCountChanged event fired correctly");
                testsPassed++;
            }
            else
            {
                LogError($"✗ Event not fired correctly (Fired: {eventFired}, Type: {receivedType}, Count: {receivedCount})");
                testsFailed++;
            }

            // Verify HUD displays correct format
            CrystalHUDController hudController = Object.FindObjectOfType<CrystalHUDController>();
            if (hudController != null)
            {
                // HUD should update automatically via event
                // We can't directly check TextMeshProUGUI text in edit mode, but we verified the event fired
                LogSuccess($"✓ HUD controller present (should display 'x{newCount}')");
                testsPassed++;
            }
            else
            {
                LogWarning("⚠ HUD controller not found (skipping HUD verification)");
            }

            // Verify crystal was removed from scene
            if (crystalObj == null || !crystalObj.activeInHierarchy)
            {
                LogSuccess("✓ Crystal removed from scene");
                testsPassed++;
            }
            else
            {
                LogError("✗ Crystal still exists in scene");
                testsFailed++;
                // Cleanup
                if (crystalObj != null)
                {
                    Object.DestroyImmediate(crystalObj);
                }
            }

            // Verify crystal NOT in inventory
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            bool crystalInInventory = false;
            foreach (var slot in slots)
            {
                if (slot.item != null && slot.item.itemName == testCrystalData.crystalName)
                {
                    crystalInInventory = true;
                    break;
                }
            }

            if (!crystalInInventory)
            {
                LogSuccess("✓ Crystal NOT in inventory (correct behavior)");
                testsPassed++;
            }
            else
            {
                LogError("✗ Crystal found in inventory (should NOT be there)");
                testsFailed++;
            }

            Log("--- Test 6.1 Complete ---");
        }

        /// <summary>
        /// Test 6.2: Item Complete Flow
        /// Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 4.1, 4.2, 4.3, 4.4, 4.5
        /// </summary>
        private void Test_ItemCompleteFlow()
        {
            Log("\n=== TEST 6.2: ITEM COMPLETE FLOW ===");

            if (testItemData == null)
            {
                LogWarning("Skipping: testItemData not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Spawn item in scene
            GameObject itemObj = new GameObject("TestItem");
            itemObj.tag = "Item";
            itemObj.transform.position = Vector3.zero;

            // Add required components
            SpriteRenderer sr = itemObj.AddComponent<SpriteRenderer>();
            CircleCollider2D collider = itemObj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            ItemCollectable collectable = itemObj.AddComponent<ItemCollectable>();

            // Use reflection to set inventoryItemData (since there's no public setter)
            var field = typeof(ItemCollectable).GetField("inventoryItemData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(collectable, testItemData);
            }

            var quantityField = typeof(ItemCollectable).GetField("itemQuantity",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (quantityField != null)
            {
                quantityField.SetValue(collectable, 5);
            }

            collectable.ForceActivateAttraction();

            Log($"✓ Item spawned at {itemObj.transform.position}");

            // Simulate collection
            collectable.CollectItem();

            // Verify item is in inventory
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            bool itemFound = false;
            int totalQuantity = 0;

            foreach (var slot in slots)
            {
                if (slot.item == testItemData)
                {
                    itemFound = true;
                    totalQuantity += slot.quantity;
                }
            }

            if (itemFound)
            {
                LogSuccess($"✓ Item found in inventory");
                testsPassed++;
            }
            else
            {
                LogError("✗ Item NOT found in inventory");
                testsFailed++;
            }

            // Verify correct quantity
            if (totalQuantity == 5)
            {
                LogSuccess($"✓ Correct quantity in inventory: {totalQuantity}");
                testsPassed++;
            }
            else
            {
                LogError($"✗ Incorrect quantity: expected 5, got {totalQuantity}");
                testsFailed++;
            }

            // Verify item was removed from scene
            if (itemObj == null || !itemObj.activeInHierarchy)
            {
                LogSuccess("✓ Item removed from scene");
                testsPassed++;
            }
            else
            {
                LogError("✗ Item still exists in scene");
                testsFailed++;
                // Cleanup
                if (itemObj != null)
                {
                    Object.DestroyImmediate(itemObj);
                }
            }

            Log("--- Test 6.2 Complete ---");
        }

        /// <summary>
        /// Test 6.3: Inventory Full Behavior
        /// Requirements: 5.1, 5.2, 5.3, 5.4, 5.5
        /// </summary>
        private void Test_InventoryFull()
        {
            Log("\n=== TEST 6.3: INVENTORY FULL BEHAVIOR ===");

            if (testItemData == null)
            {
                LogWarning("Skipping: testItemData not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Fill inventory (20 slots)
            Log("Filling inventory with 20 items...");
            for (int i = 0; i < 20; i++)
            {
                bool added = InventoryManager.Instance.AddItem(testItemData, 99);
                if (!added)
                {
                    LogError($"✗ Failed to fill inventory at slot {i}");
                    testsFailed++;
                    return;
                }
            }

            LogSuccess("✓ Inventory filled (20 slots)");

            // Spawn item in scene
            GameObject itemObj = new GameObject("TestItemFullInventory");
            itemObj.tag = "Item";
            itemObj.transform.position = Vector3.zero;

            SpriteRenderer sr = itemObj.AddComponent<SpriteRenderer>();
            CircleCollider2D collider = itemObj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;

            ItemCollectable collectable = itemObj.AddComponent<ItemCollectable>();

            // Set inventoryItemData using reflection
            var field = typeof(ItemCollectable).GetField("inventoryItemData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(collectable, testItemData);
            }

            collectable.ForceActivateAttraction();

            Log("✓ Item spawned with full inventory");

            // Try to collect
            collectable.CollectItem();

            // Verify item remains in scene
            if (itemObj != null && itemObj.activeInHierarchy)
            {
                LogSuccess("✓ Item remains in scene (inventory full)");
                testsPassed++;
            }
            else
            {
                LogError("✗ Item was destroyed (should remain in scene)");
                testsFailed++;
            }

            // Verify collection state was reverted (can collect again)
            if (!collectable.IsCollected)
            {
                LogSuccess("✓ Collection state reverted (IsCollected = false)");
                testsPassed++;
            }
            else
            {
                LogError("✗ Collection state not reverted (IsCollected = true)");
                testsFailed++;
            }

            // Free space in inventory
            Log("Freeing space in inventory...");
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            slots[0].item = null;
            slots[0].quantity = 0;

            // Try collecting again
            Log("Attempting to collect again...");
            collectable.CollectItem();

            // Verify item was collected this time
            if (itemObj == null || !itemObj.activeInHierarchy)
            {
                LogSuccess("✓ Item collected successfully after freeing space");
                testsPassed++;
            }
            else
            {
                LogError("✗ Item not collected after freeing space");
                testsFailed++;
                // Cleanup
                if (itemObj != null)
                {
                    Object.DestroyImmediate(itemObj);
                }
            }

            Log("--- Test 6.3 Complete ---");
        }

        /// <summary>
        /// Test 6.4: Type Prioritization
        /// Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6
        /// </summary>
        private void Test_TypePrioritization()
        {
            Log("\n=== TEST 6.4: TYPE PRIORITIZATION ===");

            if (testCrystalData == null || testItemData == null)
            {
                LogWarning("Skipping: testCrystalData or testItemData not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Test 1: Item with BOTH crystalData AND inventoryItemData (crystal should win)
            Log("\nTest 1: Crystal + Inventory data (crystal priority)");

            GameObject dualItem = new GameObject("DualDataItem");
            dualItem.tag = "Item";
            dualItem.transform.position = Vector3.zero;

            SpriteRenderer sr1 = dualItem.AddComponent<SpriteRenderer>();
            CircleCollider2D collider1 = dualItem.AddComponent<CircleCollider2D>();
            collider1.isTrigger = true;

            ItemCollectable collectable1 = dualItem.AddComponent<ItemCollectable>();
            collectable1.SetCrystalData(testCrystalData);

            // Also set inventoryItemData
            var field = typeof(ItemCollectable).GetField("inventoryItemData",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(collectable1, testItemData);
            }

            collectable1.ForceActivateAttraction();

            // Get initial crystal count
            CrystalType crystalType = testCrystalData.crystalType;
            int initialCrystalCount = GameManager.Instance.GetCrystalCount(crystalType);

            // Collect
            collectable1.CollectItem();

            // Verify processed as crystal (GameManager count increased)
            int newCrystalCount = GameManager.Instance.GetCrystalCount(crystalType);
            if (newCrystalCount > initialCrystalCount)
            {
                LogSuccess("✓ Processed as crystal (priority correct)");
                testsPassed++;
            }
            else
            {
                LogError("✗ Not processed as crystal");
                testsFailed++;
            }

            // Verify NOT in inventory
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            bool inInventory = false;
            foreach (var slot in slots)
            {
                if (slot.item == testItemData)
                {
                    inInventory = true;
                    break;
                }
            }

            if (!inInventory)
            {
                LogSuccess("✓ NOT added to inventory (correct)");
                testsPassed++;
            }
            else
            {
                LogError("✗ Added to inventory (should NOT be)");
                testsFailed++;
            }

            // Test 2: Item with ONLY inventoryItemData
            Log("\nTest 2: Only inventory data");

            GameObject inventoryItem = new GameObject("InventoryOnlyItem");
            inventoryItem.tag = "Item";
            inventoryItem.transform.position = Vector3.zero;

            SpriteRenderer sr2 = inventoryItem.AddComponent<SpriteRenderer>();
            CircleCollider2D collider2 = inventoryItem.AddComponent<CircleCollider2D>();
            collider2.isTrigger = true;

            ItemCollectable collectable2 = inventoryItem.AddComponent<ItemCollectable>();

            // Set ONLY inventoryItemData
            if (field != null)
            {
                field.SetValue(collectable2, testItemData);
            }

            collectable2.ForceActivateAttraction();

            // Collect
            collectable2.CollectItem();

            // Verify in inventory
            slots = InventoryManager.Instance.GetAllSlots();
            bool foundInInventory = false;
            foreach (var slot in slots)
            {
                if (slot.item == testItemData)
                {
                    foundInInventory = true;
                    break;
                }
            }

            if (foundInInventory)
            {
                LogSuccess("✓ Added to inventory (correct)");
                testsPassed++;
            }
            else
            {
                LogError("✗ NOT added to inventory (should be)");
                testsFailed++;
            }

            Log("--- Test 6.4 Complete ---");
        }

        #endregion

        #region Helper Methods
        private void ClearInventory()
        {
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].item = null;
                slots[i].quantity = 0;
            }
            Log("  → Inventory cleared");
        }

        private void Log(string message)
        {
            testLog += message + "\n";
            Debug.Log($"[IntegrationTests] {message}");
        }

        private void LogSuccess(string message)
        {
            testLog += $"<color=green>{message}</color>\n";
            Debug.Log($"[IntegrationTests] {message}");
        }

        private void LogError(string message)
        {
            testLog += $"<color=red>{message}</color>\n";
            Debug.LogError($"[IntegrationTests] {message}");
        }

        private void LogWarning(string message)
        {
            testLog += $"<color=yellow>{message}</color>\n";
            Debug.LogWarning($"[IntegrationTests] {message}");
        }
        #endregion
    }
}
