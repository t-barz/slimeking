# Test Scenes

This folder contains automated test scenes for various game systems.

## Available Test Scenes

### 1. Dialogue System Test Scene

#### Creating the Test Scene

In Unity Editor, go to:

```
SlimeKing > Dialogue System > Create Test Scene
```

This will automatically create a complete test scene at:

```
Assets/Game/Scenes/Tests/DialogueSystemTest.unity
```

### 2. Quest System Test Scene

#### Creating the Test Scene

In Unity Editor, go to:

```
SlimeKing > Quest System > Create Test Scene
```

This will automatically create a complete test scene at:

```
Assets/Game/Scenes/Tests/QuestSystemTest.unity
```

## Dialogue System Test Scene

### What's Included

- **NPCs**: Merchant (Yellow), Guard (Red), Villager (Green)
- **Managers**: DialogueManager, LocalizationManager
- **UI**: DialogueUI with typewriter effect, Language Switcher
- **Player**: Simple controller with WASD movement

### Testing

1. Play the scene
2. Move with WASD keys
3. Approach an NPC to see interaction icon
4. Press E to start dialogue
5. Press E again to advance/complete text
6. Click language buttons to test localization

### Documentation

- **Test Instructions**: `DIALOGUE_SYSTEM_TEST_INSTRUCTIONS.md`
- **Completion Summary**: `TASK_15_COMPLETION_SUMMARY.md`

## Quest System Test Scene

### What's Included

- **Managers**: QuestManager, GameManager
- **NPC**: Quest Giver with visual indicators (! yellow/golden)
- **Quest**: "Coletar Flores" - Collect 3 Frutas de Cura
- **UI**: QuestNotificationPanel
- **Player**: Simple controller with InventoryManager

### Testing Tools

#### 1. Automated Tests

**Access**: Menu → **SlimeKing > Quest System > Run Automated Tests**

- Validates all components exist
- Tests integration points
- Checks event system
- Provides visual pass/fail results
- 14+ automated tests

#### 2. Manual Test Checklist

**File**: `QUEST_SYSTEM_MANUAL_TEST_CHECKLIST.md`

- 60+ detailed test cases
- Step-by-step instructions
- Expected results
- Pass/fail checkboxes

#### 3. Quick Testing Guide

**File**: `QUEST_SYSTEM_TESTING_QUICK_GUIDE.md`

- 5-minute quick start
- Common issues and solutions
- Fast reference

### Testing

1. Play the scene
2. Move with WASD keys toward yellow NPC
3. Press E to interact and accept quest
4. Add items to inventory (manually via Inspector)
5. Watch progress update automatically
6. Return to NPC when complete (golden ! appears)
7. Press E and turn in quest
8. Verify rewards received

### Documentation

- **Quick Guide**: `QUEST_SYSTEM_TESTING_QUICK_GUIDE.md` ⭐ Start here!
- **Test Instructions**: `QUEST_SYSTEM_TEST_INSTRUCTIONS.md`
- **Manual Checklist**: `QUEST_SYSTEM_MANUAL_TEST_CHECKLIST.md`
- **Test Report**: `TASK_14_TEST_COMPLETION_REPORT.md`
- **Quick Start**: `QUEST_SYSTEM_TEST_README.md`
- **Completion Summary**: `QUEST_SYSTEM_TEST_COMPLETION_SUMMARY.md`

---

## Notes

- All test scenes are created programmatically
- Scenes can be modified after creation and saved
- Scene creation scripts are located in `Assets/Editor/`
- Test data is created in `Assets/Data/` folders

## Test Data Locations

### Dialogue System

```
Assets/Data/Dialogues/
├── npc_merchant_greeting.json
├── npc_guard_warning.json
└── test_dialogue.json
```

### Quest System

```
Assets/Data/Quests/
└── TestQuest_CollectFlowers.asset
```

---

**Last Updated**: 03/11/2025
