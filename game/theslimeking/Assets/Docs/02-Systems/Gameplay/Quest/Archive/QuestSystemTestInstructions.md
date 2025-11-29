# Quest System Test Scene - Instructions

## Creating the Test Scene

### Option 1: Using the Editor Menu (Recommended)

1. In Unity, go to the menu: **SlimeKing > Quest System > Create Test Scene**
2. The scene will be automatically created at `Assets/Game/Scenes/Tests/QuestSystemTest.unity`
3. The scene will include:
   - QuestManager and GameManager
   - NPC Quest Giver with test quest
   - Quest notification UI
   - Player with inventory system
   - Test quest: "Coletar Flores" (Collect 3 Frutas de Cura)

### Option 2: Manual Setup

If you prefer to set up manually:

1. Create a new scene in Unity
2. Add a GameObject named "--- MANAGERS ---" with:
   - QuestManager component
   - GameManager component (or use prefab)
3. Add a Canvas GameObject named "--- UI ---"
4. Drag the QuestNotificationPanel prefab from `Assets/Game/Prefabs/UI/` into the Canvas
5. Create an NPC with QuestGiverController and NPCDialogueInteraction components
6. Create visual indicators (! yellow and ! golden sprites)
7. Create a Player GameObject with tag "Player", InventoryManager, and movement script
8. Create a test quest using **Assets > Create > Quest System > Collect Quest**

## Test Quest Configuration

The test scene includes a pre-configured quest:

### Quest: "Coletar Flores"

- **Quest ID**: `test_collect_flowers`
- **Objective**: Collect 3 Frutas de Cura (healing fruits)
- **Rewards**:
  - 2x Cristal Elemental
  - +10 Reputation
- **Requirements**: None (minimum reputation: 0)
- **Repeatable**: Yes

## How to Test

### 1. Basic Quest Flow

1. **Play the scene** (press Play button)
2. **Move the player** using WASD keys toward the yellow NPC
3. **Approach the NPC** - a yellow exclamation mark (!) should appear above their head
4. **Press E** to interact and start dialogue
5. **Accept the quest** by selecting the "Aceitar Quest" dialogue option
6. **Verify** that a notification appears: "Quest Aceita: Coletar Flores"

### 2. Progress Tracking

1. **Open the Inventory** (check your inventory system's hotkey)
2. **Add Frutas de Cura** to your inventory:
   - You can manually add items via the InventoryManager Inspector (if in debug mode)
   - Or collect items in the game world if available
3. **Watch the progress update** automatically as you collect items
4. **Verify notifications** appear for each item collected
5. **When you collect 3 items**, a notification should appear: "Quest Pronta: Coletar Flores"

### 3. Quest Turn-In

1. **Return to the NPC** after collecting all 3 items
2. **Notice the indicator changed** from yellow (!) to golden (!) - quest is ready to turn in
3. **Press E** to interact with the NPC
4. **Select "Entregar Quest"** dialogue option
5. **Verify**:
   - Notification appears: "Quest Completada: Coletar Flores"
   - Rewards are listed in the notification
   - 3 Frutas de Cura are removed from inventory
   - 2 Cristal Elemental are added to inventory
   - Reputation increased by 10

### 4. Repeatable Quest

1. **After turning in the quest**, interact with the NPC again
2. **Verify** the yellow (!) indicator appears again (quest is available)
3. **Accept the quest again** to test repeatability
4. **Complete the quest** again to verify the full cycle works

## Test Checklist

Use this checklist to verify all features:

### Quest Acceptance

- [ ] Yellow (!) indicator appears above NPC when quest is available
- [ ] Indicator disappears when quest is accepted
- [ ] Dialogue system shows "Aceitar Quest" option
- [ ] Quest is added to active quests list
- [ ] Notification appears when quest is accepted
- [ ] QuestEvents.OnQuestAccepted is fired

### Progress Tracking

- [ ] Progress updates automatically when items are collected
- [ ] Progress is tracked correctly (current/target)
- [ ] Progress doesn't exceed target quantity
- [ ] Multiple active quests track independently
- [ ] QuestEvents.OnQuestProgressChanged is fired
- [ ] Notifications appear for progress updates

### Quest Completion

- [ ] Quest is marked as "ready to turn in" when complete
- [ ] Golden (!) indicator appears above NPC
- [ ] Notification appears: "Quest Pronta"
- [ ] QuestEvents.OnQuestReadyToTurnIn is fired

### Quest Turn-In

- [ ] Dialogue system shows "Entregar Quest" option
- [ ] Items are removed from inventory correctly
- [ ] Rewards are added to inventory correctly
- [ ] Reputation is increased correctly
- [ ] Quest is moved to completed list
- [ ] Quest is removed from active list
- [ ] Notification shows completion and rewards
- [ ] QuestEvents.OnQuestCompleted is fired
- [ ] QuestEvents.OnQuestTurnedIn is fired

### Repeatable Quests

- [ ] Quest becomes available again after turn-in
- [ ] Yellow (!) indicator reappears
- [ ] Quest can be accepted again
- [ ] Full cycle works multiple times

### Requirements Validation

- [ ] Quest with reputation requirement is hidden if player doesn't meet it
- [ ] Quest with prerequisite is hidden if prerequisite not completed
- [ ] Non-repeatable quest is hidden after completion

### Visual Indicators

- [ ] Yellow (!) indicator for available quest
- [ ] Golden (!) indicator for ready to turn in
- [ ] Indicators have bounce/pulse animation
- [ ] Indicators follow NPC position
- [ ] Indicators are visible from distance

### Notifications

- [ ] Quest accepted notification
- [ ] Progress update notifications
- [ ] Quest ready notification
- [ ] Quest completed notification with rewards
- [ ] Notifications display for appropriate duration
- [ ] Notifications have sound effects
- [ ] Multiple notifications queue properly

### Integration with Dialogue System

- [ ] "Aceitar Quest" option appears in dialogue
- [ ] "Entregar Quest" option appears when ready
- [ ] Dialogue options are dynamic based on quest state
- [ ] Dialogue closes after accepting/turning in quest

### Integration with Inventory System

- [ ] Items are tracked automatically when added to inventory
- [ ] Items are removed correctly on turn-in
- [ ] Rewards are added correctly
- [ ] Inventory space is validated before turn-in
- [ ] Stackable items are handled correctly

### Integration with Save System

- [ ] Active quests are saved with progress
- [ ] Completed quests list is saved
- [ ] Quest state is restored on load
- [ ] Progress is restored correctly

### Debug Tools

- [ ] QuestManager Inspector shows debug section
- [ ] "Force Complete Quest" button works
- [ ] "Reset Quest" button works
- [ ] "Clear All Quests" button works
- [ ] Debug logs appear when enabled
- [ ] Gizmos appear in Scene view when enabled

## Expected Behavior

### Quest Acceptance

When you accept the quest:

```
[QuestManager] Quest aceita: 'Coletar Flores' (ID: test_collect_flowers)
```

Notification appears:

```
Quest Aceita: Coletar Flores
```

### Progress Updates

As you collect items:

```
[QuestManager] Quest 'Coletar Flores' progresso: 1/3
[QuestManager] Quest 'Coletar Flores' progresso: 2/3
[QuestManager] Quest 'Coletar Flores' progresso: 3/3
[QuestManager] Quest 'Coletar Flores' pronta para entregar!
```

Notification appears:

```
Quest Pronta: Coletar Flores
```

### Quest Turn-In

When you turn in the quest:

```
[QuestManager] Quest entregue: 'Coletar Flores'
[QuestManager] Recompensa recebida: Cristal Elemental x2
[QuestManager] Reputação recebida: +10
```

Notification appears:

```
Quest Completada: Coletar Flores
Recompensas: Cristal Elemental x2
```

## Troubleshooting

### Scene Creation Fails

- **Issue**: Menu item doesn't appear or throws error
- **Solution**: Make sure all required scripts are compiled without errors
- **Check**: QuestManager, QuestGiverController, CollectQuestData, QuestEvents

### NPC Doesn't Show Indicators

- **Issue**: No exclamation mark appears above NPC
- **Solution**:
  - Check that QuestGiverController has quest assigned in Inspector
  - Verify indicator GameObjects are assigned
  - Check that quest requirements are met
  - Enable debug logs in QuestGiverController

### Quest Not Tracking Progress

- **Issue**: Collecting items doesn't update quest progress
- **Solution**:
  - Verify InventoryManager is on Player
  - Check that items match the quest's itemToCollect
  - Enable debug logs in QuestManager
  - Check console for errors

### Can't Turn In Quest

- **Issue**: "Entregar Quest" option doesn't appear
- **Solution**:
  - Verify quest is marked as "ready to turn in"
  - Check that you have all required items in inventory
  - Verify DialogueChoiceHandler is working correctly
  - Check console for errors

### Rewards Not Received

- **Issue**: Items or reputation not added after turn-in
- **Solution**:
  - Check that inventory has space for rewards
  - Verify GameManager exists for reputation
  - Enable debug logs in QuestManager
  - Check console for errors

### Save/Load Issues

- **Issue**: Quest progress not saved/loaded
- **Solution**:
  - Verify SaveEvents are being fired
  - Check that QuestManager is subscribed to save events
  - Enable debug logs in QuestManager
  - Check console for errors

## Performance Testing

### Frame Rate

- Monitor FPS during quest interactions
- Should maintain 60 FPS with multiple active quests

### Memory

- Check memory usage when accepting/completing quests
- Verify no memory leaks after multiple quest cycles

### Event System

- Verify events are unsubscribed properly
- Check for null reference exceptions
- Monitor event listener count

## Advanced Testing

### Multiple Quests

1. Create additional test quests
2. Assign multiple quests to the same NPC
3. Accept multiple quests simultaneously
4. Verify all quests track independently
5. Complete quests in different orders

### Quest Requirements

1. Create a quest with reputation requirement
2. Verify quest is hidden when requirement not met
3. Increase reputation and verify quest appears
4. Create a quest with prerequisite
5. Verify quest appears after prerequisite is completed

### Edge Cases

1. Try to accept the same quest twice
2. Try to turn in quest without required items
3. Try to turn in quest with full inventory
4. Remove items from inventory while quest is active
5. Save/load with active quest
6. Save/load with completed quest

## Next Steps

After testing:

1. **Report Issues**: Document any bugs or unexpected behavior
2. **Suggest Improvements**: Note any UX issues or feature requests
3. **Integration**: Test with actual game scenes and NPCs
4. **Expansion**: Add more quest types (Kill, Escort, Delivery, etc.)
5. **Polish**: Adjust timing, animations, and visual feedback
6. **Localization**: Add multi-language support for quest text

## Additional Resources

- **Design Document**: `.kiro/specs/quest-system-collect/design.md`
- **Requirements**: `.kiro/specs/quest-system-collect/requirements.md`
- **Tasks**: `.kiro/specs/quest-system-collect/tasks.md`
- **Quest Dialogue Integration**: `Assets/Code/Systems/QuestSystem/QUEST_DIALOGUE_INTEGRATION.md`

## Manual Testing Scenarios

### Scenario 1: Happy Path

1. Start scene
2. Approach NPC
3. Accept quest
4. Collect 3 items
5. Return to NPC
6. Turn in quest
7. Verify rewards received

**Expected**: All steps work smoothly, notifications appear, rewards are correct

### Scenario 2: Partial Progress Save

1. Accept quest
2. Collect 2 items (not complete)
3. Save game
4. Load game
5. Verify progress is 2/3
6. Collect 1 more item
7. Turn in quest

**Expected**: Progress is saved and restored correctly

### Scenario 3: Repeatable Quest

1. Complete quest once
2. Verify quest becomes available again
3. Accept quest again
4. Complete quest again
5. Repeat 3-4 times

**Expected**: Quest can be repeated indefinitely

### Scenario 4: Inventory Full

1. Fill inventory completely
2. Accept quest
3. Collect items
4. Try to turn in quest

**Expected**: Error message or warning about full inventory

### Scenario 5: Multiple Active Quests

1. Create 3 different quests
2. Accept all 3 quests
3. Collect items for quest 2 first
4. Turn in quest 2
5. Collect items for quest 1
6. Turn in quest 1
7. Collect items for quest 3
8. Turn in quest 3

**Expected**: All quests track independently and complete correctly

---

**Last Updated**: 03/11/2025
