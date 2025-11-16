# Inventory Manager Tests - Quick Start Guide

## 🚀 Quick Start (5 minutes)

### Step 1: Open Test Window

```
Menu: The Slime King > Tests > Inventory Manager Tests
```

### Step 2: Create Test Items (if needed)

**Stackable Item**:

1. Right-click `Assets/Data/Items/` (or any folder)
2. `Create > The Slime King > Item`
3. Name: `TestStackableItem`
4. Set `isStackable = true`

**Non-Stackable Item**:

1. Right-click `Assets/Data/Items/`
2. `Create > The Slime King > Item`
3. Name: `TestNonStackableItem`
4. Set `isStackable = false`

### Step 3: Configure Test Window

- Drag `TestStackableItem` to **Stackable Item** field
- Drag `TestNonStackableItem` to **Non-Stackable Item** field

### Step 4: Run Tests

1. **Enter Play Mode** (press Play button)
2. Click **"Run All Tests"** button
3. Wait for results (< 5 seconds)

### Step 5: Review Results

✅ **All Green** = InventoryManager working correctly!
❌ **Any Red** = Check logs for details

---

## 📋 What Gets Tested

| Test | Validates |
|------|-----------|
| **AddItem Adds Correctly** | Items are added with correct quantity |
| **Stacking Same Type** | Items stack in same slot (not new slots) |
| **New Slot at 99** | New slot created when stack reaches 99 |
| **Inventory Full Returns False** | AddItem returns false when full |
| **Item Behavior When Full** | Proper behavior for full inventory |

---

## ✅ Expected Results

```
=== TEST SUMMARY ===
Total: 5 tests
Passed: 5
Failed: 0
```

---

## 🔧 Troubleshooting

**"Enter Play Mode to run tests"**
→ Click Play button in Unity Editor

**"InventoryManager.Instance is null"**
→ Make sure scene has InventoryManager GameObject

**"Skipping: testStackableItem not assigned"**
→ Drag test items to window fields

---

## 📝 Requirements Validated

- ✅ **4.1**: InventoryManager checks for existing item
- ✅ **4.2**: InventoryManager increments quantity
- ✅ **4.3**: InventoryManager creates new slot if needed
- ✅ **4.4**: New slot at 99 stack limit
- ✅ **4.5**: AddItem returns true on success
- ✅ **5.1**: AddItem returns false when full
- ✅ **5.5**: System allows retry after freeing space

---

## 🎯 Next Steps

After all tests pass:

1. ✅ Mark Task 5 as complete
2. ➡️ Move to Task 6: Integration tests
3. ➡️ Test ItemCollectable with InventoryManager

---

## 💡 Pro Tips

- Keep test window open while developing
- Run tests after any InventoryManager changes
- Use test items for manual testing too
- Check Console for detailed logs

---

**Task**: 5. Validar integração com InventoryManager
**Status**: Ready to test
**Time**: ~5 minutes to setup and run
