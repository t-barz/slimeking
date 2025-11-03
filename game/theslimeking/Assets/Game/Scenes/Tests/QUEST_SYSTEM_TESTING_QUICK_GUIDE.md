# Quest System Testing - Quick Guide

**Fast reference for testing the Quest System**

---

## 🚀 Quick Start (5 Minutes)

### Step 1: Create Test Scene

```
Unity Menu → SlimeKing → Quest System → Create Test Scene
```

✅ Scene created at: `Assets/Game/Scenes/Tests/QuestSystemTest.unity`

---

### Step 2: Run Automated Tests

```
Unity Menu → SlimeKing → Quest System → Run Automated Tests
```

✅ Click "Run All Tests" button
✅ Verify all tests pass (green checkmarks)

---

### Step 3: Basic Manual Test

```
1. Press Play ▶️
2. Move to NPC (WASD keys)
3. Press E to interact
4. Select "Aceitar Quest"
5. Open InventoryManager Inspector
6. Add 3x "Frutas de Cura" manually
7. Return to NPC (golden ! appears)
8. Press E and select "Entregar Quest"
9. Verify rewards received
```

✅ **If all steps work**: Quest System is functional!

---

## 📋 Testing Checklist

### Core Flow (Must Test)

- [ ] Accept quest via dialogue
- [ ] Progress updates when items added
- [ ] Quest marked as ready when complete
- [ ] Turn in quest and receive rewards
- [ ] Repeatable quest works again

### Visual Feedback (Must Test)

- [ ] Yellow (!) indicator when quest available
- [ ] Golden (!) indicator when quest ready
- [ ] Notifications appear for all actions
- [ ] Indicators disappear/change correctly

### Integration (Should Test)

- [ ] Dialogue system shows quest options
- [ ] Inventory tracks items automatically
- [ ] Reputation increases on turn-in
- [ ] Save/load preserves quest state

### Debug Tools (Should Test)

- [ ] Force Complete Quest works
- [ ] Reset Quest works
- [ ] Clear All Quests works
- [ ] Debug logs appear when enabled

---

## 🔧 Testing Tools

### 1. Automated Validator

**Location**: Menu → SlimeKing → Quest System → Run Automated Tests

**What it tests**:

- ✅ All components exist
- ✅ Integration points configured
- ✅ Event system setup
- ✅ Quest data valid

**When to use**: After code changes, before committing

---

### 2. Manual Test Checklist

**Location**: `Assets/Game/Scenes/Tests/QUEST_SYSTEM_MANUAL_TEST_CHECKLIST.md`

**What it tests**:

- ✅ 60+ detailed test cases
- ✅ All functionality
- ✅ Edge cases
- ✅ Performance

**When to use**: Before release, for QA testing

---

### 3. Test Scene

**Location**: `Assets/Game/Scenes/Tests/QuestSystemTest.unity`

**What's included**:

- ✅ QuestManager + GameManager
- ✅ NPC with test quest
- ✅ Player with inventory
- ✅ UI notifications

**When to use**: For all manual testing

---

## 🐛 Common Issues

### Issue: No yellow indicator on NPC

**Solution**:

- Check quest is assigned in QuestGiverController
- Verify quest requirements are met
- Enable debug logs in QuestGiverController

---

### Issue: Progress not updating

**Solution**:

- Verify InventoryManager is on Player
- Check item matches quest's itemToCollect
- Enable debug logs in QuestManager
- Check console for errors

---

### Issue: Can't turn in quest

**Solution**:

- Verify you have all required items
- Check quest is marked as "ready to turn in"
- Verify DialogueChoiceHandler exists
- Check console for errors

---

### Issue: Rewards not received

**Solution**:

- Check inventory has space
- Verify GameManager exists (for reputation)
- Enable debug logs in QuestManager
- Check console for errors

---

## 📊 Test Results

### Quick Test Results

**Date**: _______________

**Basic Flow**: ⬜ Pass | ⬜ Fail

**Visual Feedback**: ⬜ Pass | ⬜ Fail

**Integration**: ⬜ Pass | ⬜ Fail

**Notes**:

```
[Quick notes here]
```

---

## 📚 Full Documentation

For complete testing documentation, see:

1. **Automated Tests**: `QuestSystemTestValidator.cs`
2. **Manual Checklist**: `QUEST_SYSTEM_MANUAL_TEST_CHECKLIST.md`
3. **Test Report**: `TASK_14_TEST_COMPLETION_REPORT.md`
4. **Test Instructions**: `QUEST_SYSTEM_TEST_INSTRUCTIONS.md`

---

## ✅ Sign-Off

**Tester**: _______________

**Date**: _______________

**Status**: ⬜ All Tests Passed | ⬜ Issues Found

---

**Last Updated**: 03/11/2025
