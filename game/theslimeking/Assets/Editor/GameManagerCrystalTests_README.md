# GameManager Crystal Tests

## Overview

This test suite validates the GameManager crystal system integration, ensuring that crystal collection, counting, and event dispatching work correctly.

## Requirements Validated

- **Requirement 2.2**: GameManager.AddCrystal increments the correct counter
- **Requirement 2.3**: OnCrystalCountChanged event is dispatched when crystals are added
- **Requirement 2.4**: GetCrystalCount returns the updated value after collection

## Test Cases

### Test 1: AddCrystal Increments Counter

**Purpose**: Verify that calling `AddCrystal` correctly increments the internal counter.

**Steps**:

1. Get initial crystal count for Fire type
2. Add 5 Fire crystals using `AddCrystal(CrystalType.Fire, 5)`
3. Get new crystal count
4. Verify count increased by exactly 5

**Expected Result**: Counter increments by the exact amount added.

---

### Test 2: OnCrystalCountChanged Event Fires

**Purpose**: Verify that the `OnCrystalCountChanged` event is dispatched when crystals are added.

**Steps**:

1. Subscribe to `OnCrystalCountChanged` event
2. Add 3 Water crystals
3. Verify event was fired with correct parameters (CrystalType.Water, new count)
4. Unsubscribe from event

**Expected Result**: Event fires with correct crystal type and updated count.

---

### Test 3: GetCrystalCount Returns Updated Value

**Purpose**: Verify that `GetCrystalCount` returns the correct value after crystals are added.

**Steps**:

1. Get initial count for Earth crystals
2. Add 7 Earth crystals
3. Call `GetCrystalCount(CrystalType.Earth)`
4. Verify returned value equals initial + 7

**Expected Result**: GetCrystalCount returns the updated value immediately after AddCrystal.

---

### Test 4: Multiple Collections Same Type

**Purpose**: Verify that multiple collections of the same crystal type accumulate correctly.

**Steps**:

1. Get initial count for Shadow crystals
2. Add 2 Shadow crystals
3. Add 3 Shadow crystals
4. Add 5 Shadow crystals
5. Verify final count equals initial + 10

**Expected Result**: Multiple AddCrystal calls accumulate correctly (2 + 3 + 5 = 10).

---

### Test 5: Collections Different Types

**Purpose**: Verify that different crystal types are tracked independently.

**Steps**:

1. Get initial counts for all 6 crystal types
2. Add different amounts to each type:
   - Nature: +1
   - Fire: +2
   - Water: +3
   - Shadow: +4
   - Earth: +5
   - Air: +6
3. Verify each type's count increased by the correct amount

**Expected Result**: Each crystal type maintains its own independent counter.

---

## How to Run Tests

### Method 1: Unity Menu (Recommended)

1. Enter Play Mode in Unity Editor
2. Go to menu: `SlimeKing > Tests > Run GameManager Crystal Tests`
3. View results in Console and dialog box

### Method 2: Manual Execution

1. Enter Play Mode
2. Open `Assets/Editor/GameManagerCrystalTests.cs`
3. Right-click on `RunCrystalTests()` method
4. Select "Run in Unity"

## Important Notes

⚠️ **Play Mode Required**: These tests MUST be run in Play Mode because they require GameManager.Instance to be initialized.

⚠️ **Test Order**: Tests are designed to be independent but run sequentially. Each test uses a different crystal type to avoid interference.

⚠️ **State Accumulation**: Tests add crystals to the GameManager, so counts will accumulate during the test run. This is expected behavior.

## Test Results Interpretation

### Success Output

```
=== Starting GameManager Crystal Tests ===
[Test 1] ✓ PASSED: Counter incremented correctly (Initial: 0, New: 5)
[Test 2] ✓ PASSED: Event fired correctly (Type: Water, Count: 3)
[Test 3] ✓ PASSED: GetCrystalCount returns updated value (Before: 0, After: 7)
[Test 4] ✓ PASSED: Multiple collections accumulated correctly (Initial: 0, Final: 10, Added: 10)
[Test 5] ✓ PASSED: All crystal types tracked independently
=== GameManager Crystal Tests Complete ===
✓ ALL TESTS PASSED
```

### Failure Output

If any test fails, you'll see detailed error messages like:

```
[Test 1] ✗ FAILED: Counter not incremented correctly (Expected: 5, Got: 3)
```

## Integration with Task 4

This test suite directly validates **Task 4: Validar integração com GameManager** from the implementation plan:

✅ Verificar que GameManager.AddCrystal incrementa contador correto
✅ Verificar que evento OnCrystalCountChanged é disparado
✅ Verificar que GetCrystalCount retorna valor atualizado
✅ Testar múltiplas coletas de cristais do mesmo tipo
✅ Testar coletas de cristais de tipos diferentes

## Next Steps

After all tests pass:

1. Mark Task 4 as complete in `tasks.md`
2. Proceed to Task 5: Validar integração com InventoryManager
3. Continue with integration tests (Task 6)

## Troubleshooting

### "Test requires Play Mode" Warning

**Solution**: Enter Play Mode before running tests.

### "GameManager.Instance is null" Error

**Solution**: Ensure GameManager is present in the scene and properly initialized.

### Tests Pass but HUD Doesn't Update

**Solution**: This test suite only validates GameManager. HUD integration is tested separately in `CrystalHUDIntegrationTests.cs`.

## Related Files

- `Assets/Code/Systems/Managers/GameManager.cs` - GameManager implementation
- `Assets/Editor/CrystalHUDIntegrationTests.cs` - HUD integration tests
- `.kiro/specs/collectable-items-restructure/tasks.md` - Implementation plan
- `.kiro/specs/collectable-items-restructure/requirements.md` - Requirements document
