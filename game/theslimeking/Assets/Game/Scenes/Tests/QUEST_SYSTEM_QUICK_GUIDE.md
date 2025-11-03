# Quest System Test - Quick Guide

## 🚀 Quick Start (30 seconds)

1. **Unity Menu** → `SlimeKing` → `Quest System` → `Create Test Scene`
2. **Press Play** ▶️
3. **Move to NPC** (WASD keys)
4. **Press E** to accept quest
5. **Add items** via Inspector (see below)
6. **Return to NPC** when complete
7. **Press E** to turn in quest

---

## 📦 How to Add Items for Testing

### Method 1: Via Inspector (Easiest)

1. **Play the scene**
2. **Select Player** in Hierarchy
3. **Find InventoryManager** component
4. **Expand "Slots" array**
5. **Find empty slot** (or create new)
6. **Set**:
   - `item` = Frutas de Cura (drag from Assets)
   - `quantity` = 3
7. **Watch quest progress** update automatically!

### Method 2: Via Debug Console

```csharp
// In Unity Console, type:
InventoryManager.Instance.AddItem(itemData, 3);
```

### Method 3: Create Collectible Items

1. Create a GameObject in scene
2. Add a script that calls:

   ```csharp
   InventoryManager.Instance.AddItem(itemData, 1);
   ```

3. Trigger on collision with Player

---

## 🎯 What to Look For

### ✅ Quest Available

- Yellow **!** above NPC
- Dialogue shows "Aceitar Quest"

### ✅ Quest Accepted

- Notification: "Quest Aceita: Coletar Flores"
- Yellow **!** disappears

### ✅ Progress Updates

- Add items → Progress updates automatically
- Notification shows progress (1/3, 2/3, 3/3)

### ✅ Quest Ready

- Golden **!** above NPC
- Notification: "Quest Pronta: Coletar Flores"
- Dialogue shows "Entregar Quest"

### ✅ Quest Completed

- Notification: "Quest Completada: Coletar Flores"
- Shows rewards: "Cristal Elemental x2"
- Items removed from inventory
- Rewards added to inventory
- Reputation +10

### ✅ Repeatable

- Yellow **!** appears again
- Can accept quest again

---

## 🐛 Debug Tools

### QuestManager Inspector

Select **QuestManager** in Hierarchy:

- ☑️ **Enable Debug Logs** - See detailed console output
- ☑️ **Show Gizmos** - See visual debug in Scene view

### Debug Buttons

- **Force Complete Quest** - Instantly complete active quest
- **Reset Quest** - Reset progress to 0
- **Clear All Quests** - Remove all quests

---

## 🎮 Controls

| Key | Action |
|-----|--------|
| **W** | Move Forward |
| **A** | Move Left |
| **S** | Move Backward |
| **D** | Move Right |
| **E** | Interact with NPC |

---

## 📊 Quest Flow

```
🟡 Yellow ! (Available)
    ↓ Press E
📝 Accept Quest
    ↓ Collect Items
📦 Progress: 0/3 → 1/3 → 2/3 → 3/3
    ↓ Complete
🟠 Golden ! (Ready)
    ↓ Press E
🎁 Turn In Quest
    ↓ Receive Rewards
✅ Quest Completed
    ↓ (If Repeatable)
🟡 Yellow ! (Available Again)
```

---

## 🎯 Test Checklist (5 minutes)

- [ ] Scene created successfully
- [ ] Yellow ! appears above NPC
- [ ] Can accept quest via dialogue
- [ ] Notification appears on accept
- [ ] Progress updates when items added
- [ ] Golden ! appears when complete
- [ ] Can turn in quest via dialogue
- [ ] Items removed from inventory
- [ ] Rewards added to inventory
- [ ] Notification shows rewards
- [ ] Quest becomes available again

---

## 🆘 Common Issues

### No indicator above NPC?

→ Check quest is assigned in QuestGiverController Inspector

### Progress not updating?

→ Make sure you're adding the correct item (Frutas de Cura)

### Can't turn in quest?

→ Verify you have 3 items in inventory

### No rewards received?

→ Check inventory has space for rewards

---

## 📚 Full Documentation

- **Detailed Instructions**: `QUEST_SYSTEM_TEST_INSTRUCTIONS.md`
- **Complete Guide**: `QUEST_SYSTEM_TEST_README.md`
- **Implementation Details**: `QUEST_SYSTEM_TEST_COMPLETION_SUMMARY.md`

---

## 💡 Tips

- Enable **Debug Logs** in QuestManager for detailed output
- Use **Force Complete Quest** button to skip item collection
- Check **Console** for any errors or warnings
- **Scene view** shows gizmos when enabled
- Quest is **repeatable** - test multiple times!

---

**Happy Testing! 🎮**
