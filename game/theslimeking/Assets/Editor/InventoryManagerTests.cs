using UnityEngine;
using UnityEditor;
using TheSlimeKing.Inventory;

namespace TheSlimeKing.Editor.Tests
{
    /// <summary>
    /// Testes de validação para InventoryManager.
    /// Valida comportamento de adição, empilhamento e inventário cheio.
    /// 
    /// Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 5.1, 5.5
    /// </summary>
    public class InventoryManagerTests : EditorWindow
    {
        #region Window Setup
        [MenuItem("Extra Tools/Tests/Inventory Manager Tests")]
        public static void ShowWindow()
        {
            var window = GetWindow<InventoryManagerTests>("Inventory Tests");
            window.minSize = new Vector2(600, 700);
        }
        #endregion

        #region Test State
        private Vector2 scrollPosition;
        private string testLog = "";
        private bool testsRunning = false;
        private int testsPassed = 0;
        private int testsFailed = 0;

        // Test items
        private ItemData testStackableItem;
        private ItemData testNonStackableItem;
        private ItemData testEquipmentItem;
        #endregion

        #region GUI
        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Inventory Manager Validation Tests", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Estes testes validam a integração com InventoryManager:\n" +
                "• AddItem adiciona itens corretamente\n" +
                "• Empilhamento de itens do mesmo tipo\n" +
                "• Criação de novo slot quando stack atinge 99\n" +
                "• Retorno false quando inventário cheio\n" +
                "• Item permanece na cena quando inventário cheio",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            // Test Items Setup
            EditorGUILayout.LabelField("Test Items Setup", EditorStyles.boldLabel);
            testStackableItem = (ItemData)EditorGUILayout.ObjectField(
                "Stackable Item", testStackableItem, typeof(ItemData), false);
            testNonStackableItem = (ItemData)EditorGUILayout.ObjectField(
                "Non-Stackable Item", testNonStackableItem, typeof(ItemData), false);
            testEquipmentItem = (ItemData)EditorGUILayout.ObjectField(
                "Equipment Item", testEquipmentItem, typeof(ItemData), false);

            EditorGUILayout.Space(10);

            // Run Tests Button
            GUI.enabled = !testsRunning && Application.isPlaying;
            if (GUILayout.Button("Run All Tests", GUILayout.Height(40)))
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
        private void RunAllTests()
        {
            testsRunning = true;
            testLog = "";
            testsPassed = 0;
            testsFailed = 0;

            Log("=== INVENTORY MANAGER VALIDATION TESTS ===\n");

            // Validate InventoryManager exists
            if (InventoryManager.Instance == null)
            {
                LogError("CRITICAL: InventoryManager.Instance is null! Cannot run tests.");
                testsRunning = false;
                return;
            }

            LogSuccess("✓ InventoryManager.Instance found");

            // Run tests
            Test_AddItem_AddsCorrectly();
            Test_Stacking_SameType();
            Test_Stacking_NewSlotAt99();
            Test_InventoryFull_ReturnsFalse();
            Test_InventoryFull_ItemRemainsInScene();

            // Summary
            Log($"\n=== TEST SUMMARY ===");
            Log($"Total: {testsPassed + testsFailed} tests");
            LogSuccess($"Passed: {testsPassed}");
            if (testsFailed > 0)
            {
                LogError($"Failed: {testsFailed}");
            }

            testsRunning = false;
            Repaint();
        }
        #endregion

        #region Test Cases

        /// <summary>
        /// Requirement 4.1, 4.3: Verifica que AddItem adiciona item corretamente
        /// </summary>
        private void Test_AddItem_AddsCorrectly()
        {
            Log("\n--- Test: AddItem Adds Correctly ---");

            if (testStackableItem == null)
            {
                LogWarning("Skipping: testStackableItem not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Add item
            bool result = InventoryManager.Instance.AddItem(testStackableItem, 5);

            if (!result)
            {
                LogError("✗ AddItem returned false (should be true)");
                testsFailed++;
                return;
            }

            // Verify item was added
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            bool itemFound = false;
            int totalQuantity = 0;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == testStackableItem)
                {
                    itemFound = true;
                    totalQuantity += slots[i].quantity;
                }
            }

            if (!itemFound)
            {
                LogError("✗ Item not found in inventory after AddItem");
                testsFailed++;
                return;
            }

            if (totalQuantity != 5)
            {
                LogError($"✗ Expected quantity 5, got {totalQuantity}");
                testsFailed++;
                return;
            }

            LogSuccess("✓ AddItem adds item correctly with correct quantity");
            testsPassed++;
        }

        /// <summary>
        /// Requirement 4.1, 4.2: Verifica empilhamento de itens do mesmo tipo
        /// </summary>
        private void Test_Stacking_SameType()
        {
            Log("\n--- Test: Stacking Same Type ---");

            if (testStackableItem == null)
            {
                LogWarning("Skipping: testStackableItem not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Add item first time
            InventoryManager.Instance.AddItem(testStackableItem, 10);

            // Add same item again
            bool result = InventoryManager.Instance.AddItem(testStackableItem, 15);

            if (!result)
            {
                LogError("✗ AddItem returned false when stacking");
                testsFailed++;
                return;
            }

            // Verify items were stacked (should be in same slot)
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            int slotsUsed = 0;
            int totalQuantity = 0;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == testStackableItem)
                {
                    slotsUsed++;
                    totalQuantity += slots[i].quantity;
                }
            }

            if (slotsUsed != 1)
            {
                LogError($"✗ Expected 1 slot used, got {slotsUsed} (items should stack)");
                testsFailed++;
                return;
            }

            if (totalQuantity != 25)
            {
                LogError($"✗ Expected total quantity 25, got {totalQuantity}");
                testsFailed++;
                return;
            }

            LogSuccess("✓ Items of same type stack correctly in same slot");
            testsPassed++;
        }

        /// <summary>
        /// Requirement 4.4: Verifica criação de novo slot quando stack atinge 99
        /// </summary>
        private void Test_Stacking_NewSlotAt99()
        {
            Log("\n--- Test: New Slot Created at Stack Limit (99) ---");

            if (testStackableItem == null)
            {
                LogWarning("Skipping: testStackableItem not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Add 99 items (fills one stack)
            InventoryManager.Instance.AddItem(testStackableItem, 99);

            // Add 10 more (should create new slot)
            bool result = InventoryManager.Instance.AddItem(testStackableItem, 10);

            if (!result)
            {
                LogError("✗ AddItem returned false when creating new stack");
                testsFailed++;
                return;
            }

            // Verify two slots are used
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            int slotsUsed = 0;
            int totalQuantity = 0;
            bool hasFullStack = false;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == testStackableItem)
                {
                    slotsUsed++;
                    totalQuantity += slots[i].quantity;
                    if (slots[i].quantity == 99)
                    {
                        hasFullStack = true;
                    }
                }
            }

            if (slotsUsed != 2)
            {
                LogError($"✗ Expected 2 slots used, got {slotsUsed}");
                testsFailed++;
                return;
            }

            if (!hasFullStack)
            {
                LogError("✗ No stack with 99 items found");
                testsFailed++;
                return;
            }

            if (totalQuantity != 109)
            {
                LogError($"✗ Expected total quantity 109, got {totalQuantity}");
                testsFailed++;
                return;
            }

            LogSuccess("✓ New slot created when stack reaches 99 limit");
            testsPassed++;
        }

        /// <summary>
        /// Requirement 5.1: Verifica que AddItem retorna false quando inventário cheio
        /// </summary>
        private void Test_InventoryFull_ReturnsFalse()
        {
            Log("\n--- Test: Inventory Full Returns False ---");

            if (testNonStackableItem == null)
            {
                LogWarning("Skipping: testNonStackableItem not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Fill all 20 slots with non-stackable items
            for (int i = 0; i < 20; i++)
            {
                bool added = InventoryManager.Instance.AddItem(testNonStackableItem, 1);
                if (!added)
                {
                    LogError($"✗ Failed to fill inventory at slot {i}");
                    testsFailed++;
                    return;
                }
            }

            // Try to add one more item (should fail)
            bool result = InventoryManager.Instance.AddItem(testNonStackableItem, 1);

            if (result)
            {
                LogError("✗ AddItem returned true when inventory is full (should be false)");
                testsFailed++;
                return;
            }

            LogSuccess("✓ AddItem returns false when inventory is full");
            testsPassed++;
        }

        /// <summary>
        /// Requirement 5.5: Simula que item permanece na cena quando inventário cheio
        /// (Este teste valida o comportamento esperado do ItemCollectable)
        /// </summary>
        private void Test_InventoryFull_ItemRemainsInScene()
        {
            Log("\n--- Test: Item Behavior When Inventory Full ---");

            if (testStackableItem == null)
            {
                LogWarning("Skipping: testStackableItem not assigned");
                return;
            }

            // Clear inventory
            ClearInventory();

            // Fill inventory
            for (int i = 0; i < 20; i++)
            {
                InventoryManager.Instance.AddItem(testStackableItem, 99);
            }

            // Verify inventory is full
            InventorySlot[] slots = InventoryManager.Instance.GetAllSlots();
            int emptySlots = 0;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].IsEmpty)
                {
                    emptySlots++;
                }
            }

            if (emptySlots > 0)
            {
                LogError($"✗ Inventory not full (has {emptySlots} empty slots)");
                testsFailed++;
                return;
            }

            // Try to add item (should fail)
            bool result = InventoryManager.Instance.AddItem(testStackableItem, 1);

            if (result)
            {
                LogError("✗ AddItem succeeded when inventory full");
                testsFailed++;
                return;
            }

            Log("  → Verified: AddItem returns false when inventory full");
            Log("  → Expected: ItemCollectable should revert collection state");
            Log("  → Expected: Item should remain in scene (not destroyed)");
            Log("  → Expected: Player can try collecting again after freeing space");

            LogSuccess("✓ Inventory full behavior validated (AddItem returns false)");
            testsPassed++;
        }

        #endregion

        #region Helper Methods
        private void ClearInventory()
        {
            // Get all slots and clear them
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
            Debug.Log($"[InventoryTests] {message}");
        }

        private void LogSuccess(string message)
        {
            testLog += $"<color=green>{message}</color>\n";
            Debug.Log($"[InventoryTests] {message}");
        }

        private void LogError(string message)
        {
            testLog += $"<color=red>{message}</color>\n";
            Debug.LogError($"[InventoryTests] {message}");
        }

        private void LogWarning(string message)
        {
            testLog += $"<color=yellow>{message}</color>\n";
            Debug.LogWarning($"[InventoryTests] {message}");
        }
        #endregion
    }
}
