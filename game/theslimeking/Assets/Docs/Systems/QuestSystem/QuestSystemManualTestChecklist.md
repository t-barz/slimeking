# Quest System - Manual Testing Checklist

**Task 14: Testar fluxo completo do sistema**

**Date**: 03/11/2025

---

## Overview

This document provides a comprehensive manual testing checklist for the Quest System. Each test should be performed and marked as complete to ensure all functionality works as expected.

## Pre-Test Setup

### Required Assets

- [ ] Test scene exists: `Assets/Game/Scenes/Tests/QuestSystemTest.unity`
- [ ] Test quest exists: `Assets/Data/Quests/TestQuest_CollectFlowers.asset`
- [ ] QuestManager exists in scene
- [ ] GameManager exists in scene
- [ ] NPC with QuestGiverController exists
- [ ] Player with InventoryManager exists
- [ ] QuestNotificationPanel exists in UI

### Automated Validation

Before manual testing, run the automated validator:

1. Go to: **SlimeKing > Quest System > Run Automated Tests**
2. Click "Run All Tests"
3. Verify all tests pass (or note failures)

---

## Test Categories

## 1. Quest Acceptance Tests

### 1.1 Basic Quest Acceptance

**Steps**:

1. Start the test scene
2. Move player toward NPC
3. Observe visual indicator (yellow !)
4. Press E to interact
5. Select "Aceitar Quest" option

**Expected Results**:

- [ ] Yellow (!) indicator visible above NPC before interaction
- [ ] Dialogue opens when pressing E
- [ ] "Aceitar Quest" option appears in dialogue
- [ ] Quest is added to active quests list
- [ ] Notification appears: "Quest Aceita: Coletar Flores"
- [ ] Console log (if debug enabled): "[QuestManager] Quest aceita: 'Coletar Flores'"
- [ ] Yellow indicator disappears after acceptance

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 1.2 Duplicate Quest Prevention

**Steps**:

1. Accept a quest
2. Try to interact with same NPC again
3. Check if "Aceitar Quest" option appears

**Expected Results**:

- [ ] "Aceitar Quest" option does NOT appear
- [ ] Quest is not duplicated in active list
- [ ] Console log (if debug enabled): Quest already active message

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 1.3 Quest Requirements Validation

**Steps**:

1. Create a quest with reputation requirement (e.g., 50)
2. Ensure player has less reputation than required
3. Assign quest to NPC
4. Try to accept quest

**Expected Results**:

- [ ] Quest does NOT appear as available
- [ ] No yellow indicator on NPC
- [ ] Console log (if debug enabled): Reputation insufficient message

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 1.4 Prerequisite Quest Validation

**Steps**:

1. Create Quest A (no prerequisites)
2. Create Quest B (requires Quest A)
3. Assign both to NPC
4. Try to accept Quest B before completing Quest A

**Expected Results**:

- [ ] Quest B does NOT appear as available
- [ ] Only Quest A shows yellow indicator
- [ ] After completing Quest A, Quest B becomes available

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 2. Progress Tracking Tests

### 2.1 Automatic Progress Tracking

**Steps**:

1. Accept quest "Coletar Flores" (requires 3 Frutas de Cura)
2. Open InventoryManager Inspector
3. Manually add 1 Fruta de Cura to inventory
4. Observe progress update

**Expected Results**:

- [ ] Progress updates automatically to 1/3
- [ ] Console log (if debug enabled): "[QuestManager] Quest 'Coletar Flores' progresso: 1/3"
- [ ] QuestEvents.OnQuestProgressChanged is fired
- [ ] No notification appears yet (not complete)

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 2.2 Progress Does Not Exceed Target

**Steps**:

1. Accept quest requiring 3 items
2. Add 5 items to inventory

**Expected Results**:

- [ ] Progress shows 3/3 (not 5/3)
- [ ] Quest is marked as ready to turn in
- [ ] Extra items remain in inventory

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 2.3 Multiple Active Quests Track Independently

**Steps**:

1. Create two different quests (different items)
2. Accept both quests
3. Add items for Quest 1
4. Add items for Quest 2

**Expected Results**:

- [ ] Quest 1 progress updates only when Quest 1 items added
- [ ] Quest 2 progress updates only when Quest 2 items added
- [ ] Both quests track correctly and independently
- [ ] No cross-contamination of progress

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 2.4 Progress Updates When Items Removed

**Steps**:

1. Accept quest requiring 3 items
2. Add 3 items to inventory (quest becomes ready)
3. Remove 1 item from inventory

**Expected Results**:

- [ ] Progress updates to 2/3
- [ ] Quest is no longer marked as ready to turn in
- [ ] Golden indicator changes back to yellow (or disappears)
- [ ] QuestEvents.OnQuestProgressChanged is fired

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 3. Quest Completion Tests

### 3.1 Quest Marked as Ready

**Steps**:

1. Accept quest requiring 3 items
2. Add 3 items to inventory

**Expected Results**:

- [ ] Progress shows 3/3
- [ ] Quest is marked as "ready to turn in"
- [ ] Console log (if debug enabled): "[QuestManager] Quest 'Coletar Flores' pronta para entregar!"
- [ ] QuestEvents.OnQuestReadyToTurnIn is fired
- [ ] Notification appears: "Quest Pronta: Coletar Flores"

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 3.2 Visual Indicator Changes

**Steps**:

1. Accept quest (yellow indicator)
2. Complete quest objectives

**Expected Results**:

- [ ] Yellow (!) indicator disappears
- [ ] Golden (!) indicator appears above NPC
- [ ] Indicator is clearly visible and distinct from yellow

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 4. Quest Turn-In Tests

### 4.1 Basic Quest Turn-In

**Steps**:

1. Complete quest (3/3 items)
2. Return to NPC
3. Press E to interact
4. Select "Entregar Quest" option

**Expected Results**:

- [ ] "Entregar Quest" option appears in dialogue
- [ ] 3 Frutas de Cura are removed from inventory
- [ ] 2 Cristal Elemental are added to inventory
- [ ] Reputation increases by 10
- [ ] Quest is removed from active list
- [ ] Quest is added to completed list
- [ ] Notification appears: "Quest Completada: Coletar Flores\nRecompensas: Cristal Elemental x2"
- [ ] Console logs (if debug enabled): Turn-in and rewards messages
- [ ] QuestEvents.OnQuestCompleted is fired
- [ ] QuestEvents.OnQuestTurnedIn is fired

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 4.2 Cannot Turn In Incomplete Quest

**Steps**:

1. Accept quest requiring 3 items
2. Add only 2 items to inventory
3. Try to interact with NPC

**Expected Results**:

- [ ] "Entregar Quest" option does NOT appear
- [ ] Golden indicator does NOT appear
- [ ] Quest remains in active list
- [ ] No rewards are given

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 4.3 Inventory Full - Cannot Turn In

**Steps**:

1. Fill inventory completely (no empty slots)
2. Complete quest with item rewards
3. Try to turn in quest

**Expected Results**:

- [ ] Turn-in fails or shows warning
- [ ] Console error: Inventory full message
- [ ] Quest remains in active list
- [ ] Items are not removed from inventory

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 4.4 Reputation Increases Correctly

**Steps**:

1. Note current reputation (check GameManager)
2. Complete and turn in quest with +10 reputation
3. Check reputation again

**Expected Results**:

- [ ] Reputation increases by exactly 10
- [ ] GameManager.OnReputationChanged event is fired
- [ ] Console log (if debug enabled): "[QuestManager] Reputação recebida: +10"

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 5. Repeatable Quest Tests

### 5.1 Repeatable Quest Cycle

**Steps**:

1. Complete and turn in repeatable quest
2. Interact with NPC again

**Expected Results**:

- [ ] Yellow (!) indicator reappears
- [ ] "Aceitar Quest" option appears again
- [ ] Quest can be accepted again
- [ ] Quest is removed from completed list (or stays if tracking history)
- [ ] Full cycle works: accept → complete → turn in → repeat

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 5.2 Non-Repeatable Quest

**Steps**:

1. Create quest with isRepeatable = false
2. Complete and turn in quest
3. Try to interact with NPC again

**Expected Results**:

- [ ] Yellow indicator does NOT reappear
- [ ] "Aceitar Quest" option does NOT appear
- [ ] Quest remains in completed list
- [ ] Cannot accept quest again

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 6. Visual Feedback Tests

### 6.1 Quest Available Indicator

**Steps**:

1. Ensure NPC has available quest
2. Observe indicator from different distances

**Expected Results**:

- [ ] Yellow (!) indicator is visible
- [ ] Indicator is positioned above NPC head
- [ ] Indicator has bounce/pulse animation (if configured)
- [ ] Indicator is visible from reasonable distance
- [ ] Indicator follows NPC if NPC moves

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 6.2 Quest Ready Indicator

**Steps**:

1. Complete quest objectives
2. Observe indicator change

**Expected Results**:

- [ ] Golden (!) indicator is visible
- [ ] Indicator is clearly different from yellow
- [ ] Indicator has bounce/pulse animation (if configured)
- [ ] Indicator is positioned above NPC head

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 6.3 Quest Accepted Notification

**Steps**:

1. Accept a quest
2. Observe notification

**Expected Results**:

- [ ] Notification panel appears
- [ ] Text shows: "Quest Aceita: [Quest Name]"
- [ ] Notification displays for ~3 seconds
- [ ] Notification disappears automatically
- [ ] Sound effect plays (if configured)

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 6.4 Quest Ready Notification

**Steps**:

1. Complete quest objectives
2. Observe notification

**Expected Results**:

- [ ] Notification panel appears
- [ ] Text shows: "Quest Pronta: [Quest Name]"
- [ ] Notification displays for ~3 seconds
- [ ] Sound effect plays (if configured)

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 6.5 Quest Completed Notification

**Steps**:

1. Turn in completed quest
2. Observe notification

**Expected Results**:

- [ ] Notification panel appears
- [ ] Text shows: "Quest Completada: [Quest Name]"
- [ ] Rewards are listed: "Recompensas: [Item] x[Quantity]"
- [ ] Notification displays for ~3 seconds
- [ ] Sound effect plays (if configured)
- [ ] Different sound from other notifications (if configured)

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 6.6 Multiple Notifications Queue

**Steps**:

1. Trigger multiple notifications quickly (e.g., complete multiple objectives)
2. Observe notification behavior

**Expected Results**:

- [ ] Notifications appear one at a time
- [ ] Each notification displays for full duration
- [ ] No notifications are skipped
- [ ] Notifications queue properly

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 7. Integration Tests

### 7.1 Dialogue System Integration

**Steps**:

1. Interact with NPC
2. Check dialogue options

**Expected Results**:

- [ ] Dialogue opens correctly
- [ ] "Aceitar Quest" option appears when quest available
- [ ] "Entregar Quest" option appears when quest ready
- [ ] Options are dynamic based on quest state
- [ ] Selecting option triggers correct action
- [ ] Dialogue closes after action

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 7.2 Inventory System Integration

**Steps**:

1. Accept quest
2. Add items via InventoryManager
3. Turn in quest

**Expected Results**:

- [ ] Items are tracked automatically when added
- [ ] Progress updates when inventory changes
- [ ] Items are removed correctly on turn-in
- [ ] Rewards are added correctly
- [ ] Stackable items are handled correctly
- [ ] Non-stackable items are handled correctly

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 7.3 Reputation System Integration

**Steps**:

1. Check initial reputation
2. Complete quest with reputation reward
3. Check reputation again

**Expected Results**:

- [ ] Reputation increases correctly
- [ ] GameManager.AddReputation() is called
- [ ] GameManager.OnReputationChanged event is fired
- [ ] Reputation persists across scenes (if applicable)

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 8. Save/Load Tests

### 8.1 Save Active Quest

**Steps**:

1. Accept quest
2. Make partial progress (e.g., 2/3 items)
3. Trigger save (if save system implemented)
4. Load save

**Expected Results**:

- [ ] Quest is still in active list
- [ ] Progress is restored correctly (2/3)
- [ ] Quest data is intact
- [ ] Can continue and complete quest

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 8.2 Save Completed Quest

**Steps**:

1. Complete and turn in quest
2. Trigger save
3. Load save

**Expected Results**:

- [ ] Quest is in completed list
- [ ] Quest is not in active list
- [ ] If non-repeatable, quest is not available again
- [ ] Rewards are still in inventory

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 8.3 Save Ready Quest

**Steps**:

1. Accept quest
2. Complete objectives (ready to turn in)
3. Trigger save (do NOT turn in)
4. Load save

**Expected Results**:

- [ ] Quest is still in active list
- [ ] Quest is marked as ready to turn in
- [ ] Golden indicator appears on NPC
- [ ] Can turn in quest immediately

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 8.4 Save Multiple Quests

**Steps**:

1. Accept 3 different quests
2. Make progress on each (different amounts)
3. Trigger save
4. Load save

**Expected Results**:

- [ ] All 3 quests are in active list
- [ ] Each quest has correct progress
- [ ] All quest data is intact
- [ ] Can continue all quests

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 9. Debug Tools Tests

### 9.1 QuestManager Inspector Debug Section

**Steps**:

1. Select QuestManager in Hierarchy
2. Observe Inspector

**Expected Results**:

- [ ] "Debug" section is visible
- [ ] "Enable Debug Logs" toggle exists
- [ ] "Show Gizmos" toggle exists
- [ ] Custom editor shows active quests list
- [ ] Custom editor shows completed quests list

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 9.2 Force Complete Quest

**Steps**:

1. Accept quest
2. In QuestManager Inspector, click "Force Complete Quest"
3. Observe result

**Expected Results**:

- [ ] Quest progress jumps to 3/3
- [ ] Quest is marked as ready to turn in
- [ ] Golden indicator appears on NPC
- [ ] Console log: "[DEBUG] Quest forced to complete"
- [ ] Can turn in quest immediately

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 9.3 Reset Quest

**Steps**:

1. Accept quest and make progress (e.g., 2/3)
2. In QuestManager Inspector, click "Reset Quest"
3. Observe result

**Expected Results**:

- [ ] Quest progress resets to 0/3
- [ ] Quest is no longer ready to turn in
- [ ] Yellow indicator reappears (if was golden)
- [ ] Console log: "[DEBUG] Quest reset"

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 9.4 Clear All Quests

**Steps**:

1. Accept multiple quests
2. Complete some quests
3. In QuestManager Inspector, click "Clear All Quests"
4. Observe result

**Expected Results**:

- [ ] All active quests are removed
- [ ] All completed quests are removed
- [ ] All indicators disappear
- [ ] Console log: "[DEBUG] All quests cleared (X active, Y completed)"

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 9.5 Debug Logs

**Steps**:

1. Enable "Enable Debug Logs" in QuestManager
2. Perform various quest actions

**Expected Results**:

- [ ] Console shows detailed logs for all actions
- [ ] Logs include: quest accepted, progress changed, quest ready, quest turned in
- [ ] Logs are clear and informative
- [ ] No spam or excessive logging

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 9.6 Gizmos Visualization

**Steps**:

1. Enable "Show Gizmos" in QuestGiverController
2. Select NPC in Hierarchy
3. Observe Scene view

**Expected Results**:

- [ ] Yellow wire sphere appears above NPC
- [ ] Sphere is positioned at NPC position + 2 units up
- [ ] Line connects NPC to sphere
- [ ] Gizmos are visible in Scene view
- [ ] Gizmos do NOT appear in Game view

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 10. Event System Tests

### 10.1 OnQuestAccepted Event

**Steps**:

1. Create test script that subscribes to QuestEvents.OnQuestAccepted
2. Accept a quest
3. Check if event is fired

**Expected Results**:

- [ ] Event is fired when quest is accepted
- [ ] Event passes correct CollectQuestData
- [ ] Event is fired AFTER quest is added to active list
- [ ] Multiple subscribers receive event

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 10.2 OnQuestProgressChanged Event

**Steps**:

1. Create test script that subscribes to QuestEvents.OnQuestProgressChanged
2. Accept quest and add items
3. Check if event is fired

**Expected Results**:

- [ ] Event is fired when progress changes
- [ ] Event passes correct questID, current, and target values
- [ ] Event is fired for each progress update
- [ ] Event is NOT fired if progress doesn't change

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 10.3 OnQuestReadyToTurnIn Event

**Steps**:

1. Create test script that subscribes to QuestEvents.OnQuestReadyToTurnIn
2. Complete quest objectives
3. Check if event is fired

**Expected Results**:

- [ ] Event is fired when quest becomes ready
- [ ] Event passes correct questID
- [ ] Event is fired AFTER quest is marked as ready
- [ ] Event is fired only once (not multiple times)

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 10.4 OnQuestCompleted Event

**Steps**:

1. Create test script that subscribes to QuestEvents.OnQuestCompleted
2. Turn in completed quest
3. Check if event is fired

**Expected Results**:

- [ ] Event is fired when quest is turned in
- [ ] Event passes correct CollectQuestData and rewards list
- [ ] Event is fired AFTER rewards are given
- [ ] Event is fired before OnQuestTurnedIn

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 10.5 OnQuestTurnedIn Event

**Steps**:

1. Create test script that subscribes to QuestEvents.OnQuestTurnedIn
2. Turn in completed quest
3. Check if event is fired

**Expected Results**:

- [ ] Event is fired when quest is turned in
- [ ] Event passes correct questID
- [ ] Event is fired AFTER quest is moved to completed list
- [ ] Event is fired after OnQuestCompleted

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 11. Edge Cases and Error Handling

### 11.1 Null Quest Data

**Steps**:

1. Try to accept null quest via code
2. Observe behavior

**Expected Results**:

- [ ] Error is logged: "Tentativa de aceitar quest nula"
- [ ] No exception is thrown
- [ ] System remains stable

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 11.2 Invalid Quest ID

**Steps**:

1. Try to get progress for non-existent quest ID
2. Observe behavior

**Expected Results**:

- [ ] Returns null (no exception)
- [ ] Warning is logged (if debug enabled)
- [ ] System remains stable

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 11.3 Missing Item Data

**Steps**:

1. Create quest with itemToCollect = null
2. Try to accept quest
3. Observe behavior

**Expected Results**:

- [ ] Validation warning in Inspector
- [ ] Quest may not function correctly
- [ ] No crash or exception

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 11.4 Negative Quantity

**Steps**:

1. Create quest with quantityRequired = -1
2. Observe validation

**Expected Results**:

- [ ] Validation resets to 1
- [ ] Warning in Inspector
- [ ] Quest functions with quantity = 1

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 11.5 Missing GameManager

**Steps**:

1. Remove GameManager from scene
2. Try to turn in quest with reputation reward
3. Observe behavior

**Expected Results**:

- [ ] Warning is logged: "GameManager não encontrado"
- [ ] Quest still turns in successfully
- [ ] Reputation is not added (but no crash)
- [ ] Other rewards are still given

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 11.6 Missing InventoryManager

**Steps**:

1. Remove InventoryManager from Player
2. Try to accept and complete quest
3. Observe behavior

**Expected Results**:

- [ ] Quest can be accepted
- [ ] Progress does not update (no inventory to track)
- [ ] Warning is logged when trying to track items
- [ ] System remains stable

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## 12. Performance Tests

### 12.1 Multiple Active Quests Performance

**Steps**:

1. Accept 10+ quests simultaneously
2. Add items to inventory
3. Observe performance

**Expected Results**:

- [ ] Frame rate remains stable (60 FPS)
- [ ] No noticeable lag when updating progress
- [ ] All quests track correctly
- [ ] Memory usage is reasonable

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 12.2 Rapid Item Addition

**Steps**:

1. Accept quest
2. Add many items rapidly (e.g., 100 items in quick succession)
3. Observe performance

**Expected Results**:

- [ ] Progress updates correctly
- [ ] No lag or freeze
- [ ] Events are fired correctly
- [ ] No duplicate event firing

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

### 12.3 Memory Leaks

**Steps**:

1. Accept and complete 50+ quests
2. Check memory usage in Profiler
3. Observe for memory leaks

**Expected Results**:

- [ ] Memory usage is stable
- [ ] No continuous memory growth
- [ ] Completed quests are properly cleaned up
- [ ] Event subscriptions are properly unsubscribed

**Status**: ⬜ Not Tested | ✅ Passed | ❌ Failed

**Notes**:

```
[Write any observations or issues here]
```

---

## Test Summary

### Overall Statistics

- **Total Tests**: 60+
- **Tests Passed**: ___
- **Tests Failed**: ___
- **Tests Not Run**: ___
- **Pass Rate**: ___%

### Critical Issues Found

```
[List any critical issues that prevent core functionality]
```

### Non-Critical Issues Found

```
[List any minor issues or improvements needed]
```

### Recommendations

```
[List any recommendations for improvements or next steps]
```

---

## Sign-Off

**Tester Name**: _______________

**Date**: _______________

**Status**: ⬜ All Tests Passed | ⬜ Some Tests Failed | ⬜ Testing Incomplete

**Notes**:

```
[Final notes and observations]
```

---

**Last Updated**: 03/11/2025
**Task**: 14. Testar fluxo completo do sistema
