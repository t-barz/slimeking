# ItemCollectable Integration Tests

## Overview

This test suite validates the complete integration of the ItemCollectable system with GameManager, InventoryManager, and CrystalHUDController.

## Test Coverage

### Test 6.1: Crystal Complete Flow

**Requirements:** 2.1, 2.2, 2.3, 2.4, 2.5, 2.6, 2.7, 3.1, 3.2, 3.3, 3.4, 3.5

Validates the complete flow of crystal collection:

- ✅ Spawns crystal in scene
- ✅ Simulates slime collecting crystal
- ✅ Verifies GameManager.GetCrystalCount increased
- ✅ Verifies OnCrystalCountChanged event fired
- ✅ Verifies HUD controller is present (should display "x{count}")
- ✅ Verifies crystal removed from scene
- ✅ Verifies crystal NOT in inventory

### Test 6.2: Item Complete Flow

**Requirements:** 1.1, 1.2, 1.3, 1.4, 1.5, 4.1, 4.2, 4.3, 4.4, 4.5

Validates the complete flow of item collection:

- ✅ Spawns item in scene
- ✅ Simulates slime collecting item
- ✅ Verifies InventoryManager contains item
- ✅ Verifies correct quantity (5 items)
- ✅ Verifies item removed from scene

### Test 6.3: Inventory Full Behavior

**Requirements:** 5.1, 5.2, 5.3, 5.4, 5.5

Validates graceful handling of full inventory:

- ✅ Fills inventory (20 slots)
- ✅ Spawns item in scene
- ✅ Attempts to collect with full inventory
- ✅ Verifies item remains in scene
- ✅ Verifies collection state reverted (IsCollected = false)
- ✅ Frees space in inventory
- ✅ Collects again successfully
- ✅ Verifies item removed after successful collection

### Test 6.4: Type Prioritization

**Requirements:** 6.1, 6.2, 6.3, 6.4, 6.5, 6.6

Validates correct prioritization of data types:

- ✅ Creates item with BOTH crystalData AND inventoryItemData
- ✅ Verifies processed as crystal (priority)
- ✅ Verifies NOT added to inventory
- ✅ Creates item with ONLY inventoryItemData
- ✅ Verifies added to inventory

## How to Run Tests

### Prerequisites

1. **Enter Play Mode** - Tests require runtime systems to be active
2. **Assign Test Data** in the test window:
   - **Crystal Data**: Any CrystalElementalData ScriptableObject
   - **Item Data**: Any ItemData ScriptableObject
   - **Item Prefab**: (Optional) A prefab with ItemCollectable component

### Running Tests

1. Open Unity Editor
2. Go to menu: **The Slime King > Tests > ItemCollectable Integration Tests**
3. Assign test data in the window
4. Enter Play Mode
5. Click one of:
   - **Individual test buttons** (6.1, 6.2, 6.3, 6.4)
   - **Run All Integration Tests** (runs all 4 tests)

### Reading Results

The test window displays:

- ✅ **Green text**: Test passed
- ❌ **Red text**: Test failed
- ⚠️ **Yellow text**: Warning or skipped test

Results are also logged to the Unity Console with `[IntegrationTests]` prefix.

## Test Data Setup

### Recommended Test Data

**Crystal Data:**

- Use any existing crystal (Nature, Fire, Water, Shadow, Earth, Air)
- Example: `Assets/Data/Crystals/Crystal_Fire.asset`

**Item Data:**

- Use any stackable item
- Example: `Assets/Data/Items/HealthPotion.asset`

**Item Prefab:**

- Optional - tests create items programmatically
- Can be used for visual verification

## Expected Results

### All Tests Passing

```
=== ITEMCOLLECTABLE INTEGRATION TESTS ===

✓ GameManager.Instance found
✓ InventoryManager.Instance found
✓ CrystalHUDController found

=== TEST 6.1: CRYSTAL COMPLETE FLOW ===
✓ Crystal spawned at (0.0, 0.0, 0.0)
✓ GameManager count increased correctly: 0 → 1
✓ OnCrystalCountChanged event fired correctly
✓ HUD controller present (should display 'x1')
✓ Crystal removed from scene
✓ Crystal NOT in inventory (correct behavior)

=== TEST 6.2: ITEM COMPLETE FLOW ===
✓ Item spawned at (0.0, 0.0, 0.0)
✓ Item found in inventory
✓ Correct quantity in inventory: 5
✓ Item removed from scene

=== TEST 6.3: INVENTORY FULL BEHAVIOR ===
✓ Inventory filled (20 slots)
✓ Item spawned with full inventory
✓ Item remains in scene (inventory full)
✓ Collection state reverted (IsCollected = false)
✓ Item collected successfully after freeing space

=== TEST 6.4: TYPE PRIORITIZATION ===
✓ Processed as crystal (priority correct)
✓ NOT added to inventory (correct)
✓ Added to inventory (correct)

=== TEST SUMMARY ===
Total: 18 tests
Passed: 18
```

## Troubleshooting

### "GameManager.Instance is null"

- Ensure GameManager exists in the scene
- Check that GameManager has proper singleton setup

### "InventoryManager.Instance is null"

- Ensure InventoryManager exists in the scene
- Check that InventoryManager has proper singleton setup

### "CrystalHUDController not found"

- Warning only - tests will continue
- Add CrystalHUDController to CanvasHUD for full validation

### "testCrystalData not assigned"

- Assign a CrystalElementalData asset in the test window
- Use any crystal from `Assets/Data/Crystals/`

### "testItemData not assigned"

- Assign an ItemData asset in the test window
- Use any item from `Assets/Data/Items/`

### Tests fail in Edit Mode

- Tests MUST be run in Play Mode
- Enter Play Mode before running tests

## Integration with Task List

These tests validate **Task 6** from the implementation plan:

- ✅ **Task 6.1**: Crystal complete flow
- ✅ **Task 6.2**: Item complete flow
- ✅ **Task 6.3**: Inventory full behavior
- ✅ **Task 6.4**: Type prioritization

## Next Steps

After all integration tests pass:

1. Proceed to **Task 7**: Manual tests and visual validation
2. Test in actual gameplay scenarios
3. Verify VFX and SFX work correctly
4. Test with multiple crystal types
5. Test with various item types

## Notes

- Tests create GameObjects programmatically (no prefabs required)
- Tests clean up after themselves (destroy test objects)
- Tests use reflection to set private fields (inventoryItemData, itemQuantity)
- Tests are non-destructive (clear inventory before each test)
- Tests validate both success and failure paths

## Performance

- Each test runs in < 1 second
- All 4 tests complete in < 5 seconds
- No memory leaks (proper cleanup)
- Safe to run repeatedly

## Maintenance

When updating ItemCollectable:

1. Run these tests to ensure no regressions
2. Update tests if behavior changes
3. Add new tests for new features
4. Keep test data assignments up to date
