# Dialogue System Test Scene

## Quick Start

### Creating the Test Scene

In Unity Editor, go to:

```
SlimeKing > Dialogue System > Create Test Scene
```

This will automatically create a complete test scene at:

```
Assets/Game/Scenes/Tests/DialogueSystemTest.unity
```

## What's Included

The test scene includes:

### NPCs

- **Merchant** (Yellow) - Single page dialogue
- **Guard** (Red) - Multi-page dialogue (3 pages)
- **Villager** (Green) - Multi-page dialogue (2 pages)

### Managers

- DialogueManager
- LocalizationManager

### UI

- DialogueUI (dialogue box with typewriter effect)
- Language Switcher (BR, EN, ES buttons)

### Player

- Simple player controller with WASD movement
- Interaction with E key

## Testing

1. **Play the scene**
2. **Move** with WASD keys
3. **Approach an NPC** to see the interaction icon
4. **Press E** to start dialogue
5. **Press E again** to advance/complete text
6. **Click language buttons** to test localization

## Documentation

- **Full Guide**: `Assets/Docs/DIALOGUE_SYSTEM_README.md`
- **Test Instructions**: `DIALOGUE_SYSTEM_TEST_INSTRUCTIONS.md`
- **Completion Summary**: `TASK_15_COMPLETION_SUMMARY.md`

## Dialogue Files

Test dialogues are located in:

```
Assets/Data/Dialogues/
├── npc_merchant_greeting.json (single page)
├── npc_guard_warning.json (multi-page)
└── test_dialogue.json (multi-page)
```

All dialogues support BR, EN, and ES languages.

---

**Note**: The scene is created programmatically. If you need to modify it, you can either:

1. Create it once and save your changes
2. Modify the `DialogueSystemTestSceneSetup.cs` script

---

**Last Updated**: 31/10/2025
