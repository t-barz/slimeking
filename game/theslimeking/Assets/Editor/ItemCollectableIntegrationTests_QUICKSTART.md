# ItemCollectable Integration Tests - Quick Start Guide

## 🚀 Quick Start (30 seconds)

### Step 1: Open Test Window

```
Unity Menu → The Slime King → Tests → ItemCollectable Integration Tests
```

### Step 2: Assign Test Data

In the test window, drag and drop:

- **Crystal Data**: Any crystal from `Assets/Data/Crystals/`
- **Item Data**: Any item from `Assets/Data/Items/`

### Step 3: Enter Play Mode

Press the Play button in Unity Editor

### Step 4: Run Tests

Click **"Run All Integration Tests"** button

### Step 5: Check Results

- ✅ Green = Passed
- ❌ Red = Failed
- Look for "X passed, Y failed" at the top

## 📋 What Gets Tested

| Test | What It Validates |
|------|-------------------|
| **6.1** | Crystal → GameManager → HUD (NOT inventory) |
| **6.2** | Item → InventoryManager (removed from scene) |
| **6.3** | Full inventory → Item stays in scene |
| **6.4** | Crystal priority > Inventory priority |

## ✅ Expected Result

```
Results: 18 passed, 0 failed
```

## ❌ Common Issues

### "Enter Play Mode to run tests"

→ Click the Play button first

### "testCrystalData not assigned"

→ Drag a crystal asset into the "Crystal Data" field

### "testItemData not assigned"

→ Drag an item asset into the "Item Data" field

### "GameManager.Instance is null"

→ Ensure GameManager exists in your scene

### "InventoryManager.Instance is null"

→ Ensure InventoryManager exists in your scene

## 🎯 Individual Tests

Want to run just one test? Use these buttons:

- **6.1: Test Crystal Complete Flow**
- **6.2: Test Item Complete Flow**
- **6.3: Test Inventory Full**
- **6.4: Test Type Prioritization**

## 📊 Test Coverage

These tests validate **18 different behaviors** across:

- Crystal collection system
- Inventory integration
- Event system
- HUD updates
- Error handling
- Type prioritization

## 🔍 Where to Find Test Data

### Crystal Data

```
Assets/Data/Crystals/
├── Crystal_Nature.asset
├── Crystal_Fire.asset
├── Crystal_Water.asset
├── Crystal_Shadow.asset
├── Crystal_Earth.asset
└── Crystal_Air.asset
```

### Item Data

```
Assets/Data/Items/
├── HealthPotion.asset
├── ManaPotion.asset
└── [other items]
```

## 📝 Next Steps After Tests Pass

1. ✅ All integration tests pass
2. → Proceed to **Task 7**: Manual testing
3. → Test in actual gameplay
4. → Verify VFX and SFX
5. → Test with real player movement

## 💡 Pro Tips

- **Run tests after ANY change** to ItemCollectable
- **Tests are fast** - run them frequently
- **Tests are safe** - they clean up after themselves
- **Tests are repeatable** - run as many times as needed

## 🆘 Need Help?

See full documentation: `ItemCollectableIntegrationTests_README.md`

## 🎉 Success Criteria

All 18 tests pass = Task 6 complete! ✅
