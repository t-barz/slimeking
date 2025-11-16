# GameManager Crystal Tests - Quick Start Guide

## 🚀 Quick Start (3 Steps)

### Step 1: Enter Play Mode

Click the Play button in Unity Editor (or press `Ctrl+P` / `Cmd+P`)

### Step 2: Open Menu

Navigate to: **SlimeKing > Tests > Run GameManager Crystal Tests**

### Step 3: View Results

- Check the Console for detailed test output
- A dialog box will show the summary (Pass/Fail)

---

## ✅ Expected Success Output

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

**Dialog Box**: "All GameManager crystal tests passed successfully!"

---

## ❌ If Tests Fail

1. **Check Console** for detailed error messages
2. **Verify GameManager** is in the scene and initialized
3. **Ensure Play Mode** is active (tests won't run in Edit Mode)
4. **Review** `GameManagerCrystalTests_README.md` for troubleshooting

---

## 📋 What These Tests Validate

✅ **AddCrystal** increments counters correctly
✅ **OnCrystalCountChanged** event fires when crystals are added
✅ **GetCrystalCount** returns updated values
✅ **Multiple collections** of same type accumulate
✅ **Different crystal types** are tracked independently

---

## 📚 More Information

- **Full Documentation**: `Assets/Editor/GameManagerCrystalTests_README.md`
- **Implementation Summary**: `.kiro/specs/collectable-items-restructure/TASK_4_VALIDATION_SUMMARY.md`
- **Requirements**: `.kiro/specs/collectable-items-restructure/requirements.md` (Section 2)

---

## 🎯 Task Status

**Task 4: Validar integração com GameManager** - ✅ COMPLETED

All validation criteria met:

- ✅ GameManager.AddCrystal increments counter correto
- ✅ Evento OnCrystalCountChanged é disparado
- ✅ GetCrystalCount retorna valor atualizado
- ✅ Múltiplas coletas de cristais do mesmo tipo
- ✅ Coletas de cristais de tipos diferentes
