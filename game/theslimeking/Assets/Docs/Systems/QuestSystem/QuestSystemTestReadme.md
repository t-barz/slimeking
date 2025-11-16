# Quest System Test Scene

## Quick Start

### Creating the Test Scene

In Unity Editor, go to:

```
SlimeKing > Quest System > Create Test Scene
```

This will automatically create a complete test scene at:

```
Assets/Game/Scenes/Tests/QuestSystemTest.unity
```

## What's Included

The test scene includes:

### Managers

- **QuestManager** - Core quest system manager
- **GameManager** - Handles reputation and game state

### NPC

- **NPC_QuestGiver** (Yellow Capsule) - Offers test quest
  - Has QuestGiverController component
  - Has NPCDialogueInteraction component
  - Shows visual indicators (! yellow when available, ! golden when ready)

### Quest

- **Test Quest: "Coletar Flores"**
  - Objective: Collect 3 Frutas de Cura
  - Rewards: 2x Cristal Elemental + 10 Reputation
  - Repeatable: Yes
  - Located at: `Assets/Data/Quests/TestQuest_CollectFlowers.asset`

### UI

- **QuestNotificationPanel** - Shows quest notifications
  - Quest accepted
  - Progress updates
  - Quest ready to turn in
  - Quest completed with rewards

### Player

- **Player** (Blue Capsule) - Simple player controller
  - WASD movement
  - InventoryManager component
  - Camera attached

### Environment

- **Ground** (Green Plane) - Simple test environment
- **Directional Light** - Scene lighting

## Testing

1. **Play the scene**
2. **Move** with WASD keys toward the yellow NPC
3. **Approach the NPC** to see the yellow (!) indicator
4. **Press E** to start dialogue
5. **Select "Aceitar Quest"** to accept the quest
6. **Add items to inventory** (manually via Inspector or collect in world)
7. **Watch progress update** automatically
8. **Return to NPC** when quest is complete (golden ! indicator)
9. **Press E** and select "Entregar Quest"
10. **Verify rewards** are received

## Quest Flow Diagram

```
[NPC with Yellow !]
       ↓
   Press E
       ↓
[Dialogue: "Aceitar Quest"]
       ↓
   Accept Quest
       ↓
[Quest Added to Active List]
       ↓
[Collect Items: 0/3 → 1/3 → 2/3 → 3/3]
       ↓
[Quest Ready - NPC shows Golden !]
       ↓
   Press E
       ↓
[Dialogue: "Entregar Quest"]
       ↓
   Turn In Quest
       ↓
[Items Removed, Rewards Added]
       ↓
[Quest Completed - Available Again if Repeatable]
```

## Key Features to Test

### ✅ Quest Acceptance

- Yellow (!) indicator appears when quest is available
- Dialogue integration works
- Quest is added to active list
- Notification appears

### ✅ Progress Tracking

- Progress updates automatically when items are collected
- Progress is displayed correctly (current/target)
- Notifications appear for progress updates

### ✅ Quest Completion

- Golden (!) indicator appears when quest is ready
- "Quest Pronta" notification appears
- Dialogue shows "Entregar Quest" option

### ✅ Quest Turn-In

- Items are removed from inventory
- Rewards are added to inventory
- Reputation is increased
- Quest is moved to completed list
- Notification shows rewards

### ✅ Repeatable Quests

- Quest becomes available again after turn-in
- Can be accepted and completed multiple times

### ✅ Visual Feedback

- Indicators change based on quest state
- Notifications appear at appropriate times
- Animations play smoothly

### ✅ Integration

- Works with Dialogue System
- Works with Inventory System
- Works with Save System (if implemented)
- Works with Reputation System

## Debug Tools

### QuestManager Inspector

When you select the QuestManager in the scene, you'll see:

- **Debug** section with:
  - Enable Debug Logs checkbox
  - Show Gizmos checkbox
- **Active Quests** list (runtime)
- **Completed Quests** list (runtime)
- **Debug Tools** section with buttons:
  - Force Complete Quest
  - Reset Quest
  - Clear All Quests

### Console Logs

Enable "Enable Debug Logs" in QuestManager to see detailed logs:

```
[QuestManager] Quest aceita: 'Coletar Flores' (ID: test_collect_flowers)
[QuestManager] Quest 'Coletar Flores' progresso: 1/3
[QuestManager] Quest 'Coletar Flores' progresso: 2/3
[QuestManager] Quest 'Coletar Flores' progresso: 3/3
[QuestManager] Quest 'Coletar Flores' pronta para entregar!
[QuestManager] Quest entregue: 'Coletar Flores'
[QuestManager] Recompensa recebida: Cristal Elemental x2
[QuestManager] Reputação recebida: +10
```

## Adding More Test Quests

### Method 1: Using Context Menu

1. Right-click in Project window
2. Select **Create > Quest System > Collect Quest**
3. Configure quest properties in Inspector
4. Assign quest to NPC's QuestGiverController

### Method 2: Using Menu Item

1. Go to **Tools > Quest System > Create Collect Quest**
2. Choose save location
3. Configure quest properties
4. Assign to NPC

### Quest Configuration

```
Quest Info:
- Quest ID: unique_quest_id
- Quest Name: Display name
- Description: Quest description text

Objective:
- Item To Collect: ItemData asset
- Quantity Required: Number of items

Rewards:
- Item Rewards: List of ItemReward (item + quantity)
- Reputation Reward: Amount of reputation

Requirements:
- Minimum Reputation: Required reputation to accept
- Prerequisite Quests: Quests that must be completed first

Settings:
- Is Repeatable: Can quest be repeated after completion
```

## Common Issues

### Issue: NPC doesn't show indicator

**Solution**:

- Check that quest is assigned in QuestGiverController
- Verify indicator GameObjects are assigned
- Check quest requirements are met

### Issue: Progress doesn't update

**Solution**:

- Verify InventoryManager is on Player
- Check that items match quest's itemToCollect
- Enable debug logs to see what's happening

### Issue: Can't turn in quest

**Solution**:

- Verify you have all required items
- Check that quest is marked as "ready to turn in"
- Verify DialogueChoiceHandler is working

### Issue: Rewards not received

**Solution**:

- Check inventory has space for rewards
- Verify GameManager exists for reputation
- Enable debug logs to see reward processing

## Documentation

- **Full Test Instructions**: `QUEST_SYSTEM_TEST_INSTRUCTIONS.md`
- **Design Document**: `.kiro/specs/quest-system-collect/design.md`
- **Requirements**: `.kiro/specs/quest-system-collect/requirements.md`
- **Tasks**: `.kiro/specs/quest-system-collect/tasks.md`

## Next Steps

1. **Test the basic flow** - Accept, progress, turn in
2. **Test repeatability** - Complete quest multiple times
3. **Test with multiple quests** - Create and test multiple quests
4. **Test requirements** - Create quests with reputation/prerequisite requirements
5. **Test save/load** - Save with active quest, load and verify
6. **Test edge cases** - Full inventory, missing items, etc.
7. **Integrate with real game** - Add quests to actual game scenes

---

**Last Updated**: 03/11/2025
