# Dialogue System Test Scene - Instructions

## Creating the Test Scene

### Option 1: Using the Editor Menu (Recommended)

1. In Unity, go to the menu: **SlimeKing > Dialogue System > Create Test Scene**
2. The scene will be automatically created at `Assets/Game/Scenes/Tests/DialogueSystemTest.unity`
3. The scene will include:
   - DialogueManager and LocalizationManager
   - 3 NPCs with different dialogue configurations
   - Language switcher UI (BR, EN, ES buttons)
   - Simple player controller for testing

### Option 2: Manual Setup

If you prefer to set up manually:

1. Create a new scene in Unity
2. Add a GameObject named "--- MANAGERS ---" with:
   - DialogueManager component
   - LocalizationManager component
3. Add a Canvas GameObject named "--- UI ---"
4. Drag the DialogueUI prefab from `Assets/Game/Prefabs/UI/` into the Canvas
5. Create 3 NPCs using the Quick Setup (see below)
6. Create a Player GameObject with tag "Player" and simple movement

## Test NPCs Configuration

The test scene includes 3 NPCs:

### 1. NPC_Merchant (Yellow)

- **Position**: (-3, 0, 0)
- **Dialogue ID**: `npc_merchant_greeting`
- **Type**: Single page dialogue
- **Purpose**: Test simple, one-page dialogue flow

### 2. NPC_Guard (Red)

- **Position**: (0, 0, 0)
- **Dialogue ID**: `npc_guard_warning`
- **Type**: Multi-page dialogue (3 pages)
- **Purpose**: Test pagination and page navigation

### 3. NPC_Villager (Green)

- **Position**: (3, 0, 0)
- **Dialogue ID**: `test_dialogue`
- **Type**: Multi-page dialogue (2 pages)
- **Purpose**: Test multi-page dialogue with different content

## How to Test

### Basic Interaction

1. **Play the scene** (press Play button)
2. **Move the player** using WASD keys
3. **Approach an NPC** - an interaction icon should appear above their head
4. **Press E** to start the dialogue
5. **Read the text** as it appears with typewriter effect
6. **Press E again** to:
   - Complete the typewriter effect (if still typing)
   - Advance to the next page (if text is complete and there are more pages)
   - Close the dialogue (if on the last page)

### Language Testing

1. **Click the BR button** in the top-left corner to switch to Portuguese
2. **Interact with an NPC** - dialogue should appear in Portuguese
3. **Click the EN button** to switch to English
4. **Interact with an NPC** - dialogue should appear in English
5. **Click the ES button** to switch to Spanish
6. **Interact with an NPC** - dialogue should appear in Spanish

### Test Checklist

Use this checklist to verify all features:

#### Proximity Detection

- [ ] Icon appears when player approaches NPC
- [ ] Icon disappears when player moves away
- [ ] Icon follows NPC position
- [ ] Icon has bounce/pulse animation

#### Dialogue Display

- [ ] Dialogue box appears when pressing E near NPC
- [ ] Text appears with typewriter effect
- [ ] Typewriter speed is appropriate (not too fast/slow)
- [ ] Dialogue box has proper styling and positioning

#### Typewriter Effect

- [ ] Text appears character by character
- [ ] Pressing E during typewriter completes the text instantly
- [ ] After text is complete, pressing E advances to next page

#### Pagination

- [ ] Single-page dialogue closes after pressing E
- [ ] Multi-page dialogue shows continuation indicator
- [ ] Pressing E advances to next page
- [ ] Last page closes the dialogue
- [ ] Previous text is cleared when advancing pages

#### Localization

- [ ] BR button switches to Portuguese
- [ ] EN button switches to English
- [ ] ES button switches to Spanish
- [ ] Dialogue content changes based on selected language
- [ ] Language persists across different NPCs

#### Error Handling

- [ ] No errors in console during normal operation
- [ ] Missing dialogue ID shows appropriate error message
- [ ] Invalid JSON is handled gracefully

## Expected Behavior

### Merchant (Single Page)

- **EN**: "Welcome, traveler! I have the finest wares in all the land!"
- **BR**: "Bem-vindo, viajante! Tenho as melhores mercadorias de toda a terra!"
- **ES**: "¡Bienvenido, viajero! ¡Tengo las mejores mercancías de toda la tierra!"

### Guard (Multi-Page)

**Page 1:**

- **EN**: "Halt! Who goes there?"
- **BR**: "Alto lá! Quem vem aí?"
- **ES**: "¡Alto! ¿Quién va?"

**Page 2:**

- **EN**: "These lands are dangerous. Many monsters lurk in the shadows."
- **BR**: "Estas terras são perigosas. Muitos monstros espreitam nas sombras."
- **ES**: "Estas tierras son peligrosas. Muchos monstruos acechan en las sombras."

**Page 3:**

- **EN**: "Be careful out there, adventurer. And stay safe!"
- **BR**: "Tome cuidado por aí, aventureiro. E fique seguro!"
- **ES**: "¡Ten cuidado ahí fuera, aventurero! ¡Y mantente a salvo!"

### Villager (Multi-Page)

**Page 1:**

- **EN**: "Hello, traveler! Welcome to our village."
- **BR**: "Olá, viajante! Bem-vindo à nossa vila."
- **ES**: "¡Hola, viajero! Bienvenido a nuestro pueblo."

**Page 2:**

- **EN**: "I hope you enjoy your stay here."
- **BR**: "Espero que você aproveite sua estadia aqui."
- **ES**: "Espero que disfrutes tu estancia aquí."

## Troubleshooting

### Scene Creation Fails

- **Issue**: Menu item doesn't appear or throws error
- **Solution**: Make sure all required scripts are compiled without errors
- **Check**: DialogueManager, LocalizationManager, NPCDialogueInteraction, NPCDialogueQuickConfig

### NPCs Don't Respond

- **Issue**: Pressing E near NPC doesn't start dialogue
- **Solution**:
  - Check that Player has "Player" tag
  - Verify Input Manager has "Interact" button configured (Edit > Project Settings > Input Manager)
  - Check console for errors

### Dialogue Not Found

- **Issue**: "[Dialogue not found]" appears in dialogue box
- **Solution**:
  - Verify JSON files exist in `Assets/Data/Dialogues/`
  - Check that dialogue IDs match exactly (case-sensitive)
  - Validate JSON syntax using jsonlint.com

### Language Buttons Don't Work

- **Issue**: Clicking language buttons doesn't change dialogue
- **Solution**:
  - Check that LocalizationManager is in the scene
  - Verify that JSON files have all language localizations
  - Check console for errors

### Typewriter Effect Issues

- **Issue**: Text appears too fast/slow or instantly
- **Solution**:
  - Adjust `defaultTypewriterSpeed` in DialogueSystemSettings
  - Check `customTypewriterSpeed` in NPCDialogueInteraction (0 = use default)

## Performance Testing

### Frame Rate

- Monitor FPS during dialogue interactions
- Should maintain 60 FPS with multiple NPCs

### Memory

- Check memory usage when loading dialogues
- Verify no memory leaks after multiple dialogue interactions

### Loading Time

- First dialogue load may take slightly longer (JSON parsing)
- Subsequent dialogues should be instant (cached)

## Next Steps

After testing:

1. **Report Issues**: Document any bugs or unexpected behavior
2. **Suggest Improvements**: Note any UX issues or feature requests
3. **Integration**: Test with actual game scenes and NPCs
4. **Localization**: Add more languages if needed
5. **Polish**: Adjust timing, animations, and visual feedback

## Additional Resources

- **Full Documentation**: `Assets/Docs/DIALOGUE_SYSTEM_README.md`
- **Design Document**: `.kiro/specs/npc-dialogue-system/design.md`
- **Requirements**: `.kiro/specs/npc-dialogue-system/requirements.md`

---

**Last Updated**: 31/10/2025
