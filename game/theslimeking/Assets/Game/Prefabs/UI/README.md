# UI Prefabs

## DialogueUI Prefab

### Overview

The DialogueUI prefab is the main dialogue box that displays NPC dialogue text with a typewriter effect. It appears at the bottom of the screen when a dialogue is active.

### Structure

- **DialogueUI** (Root GameObject)
  - Canvas (Screen Space - Overlay, Sorting Order: 10)
  - Canvas Scaler (Scale with Screen Size, Reference Resolution: 1920x1080)
  - Graphic Raycaster
  - DialogueUI Component (script)
  
  - **DialogueBox** (Child GameObject)
    - RectTransform (anchored to bottom 10-90% width, 5-25% height)
    - Image (semi-transparent dark background, alpha: 0.9)
    - CanvasGroup (for fade in/out effects)
    - Content Size Fitter

    - **DialogueText** (Child GameObject)
      - RectTransform (fills parent with 40px padding)
      - TextMeshProUGUI (displays the dialogue text)
      - Font Size: 24
      - Alignment: Left, Top
      - Word Wrapping: Enabled
      - Color: White

    - **ContinueIndicator** (Child GameObject)
      - RectTransform (anchored to bottom-right corner)
      - TextMeshProUGUI (displays "▼" arrow)
      - Font Size: 32
      - Alignment: Center
      - Color: White

### Components

#### DialogueUI Script

- **dialogueText**: Reference to the DialogueText TextMeshProUGUI component
- **dialogueBox**: Reference to the DialogueBox GameObject
- **continueIndicator**: Reference to the ContinueIndicator GameObject
- **typewriterSpeed**: Characters per second for typewriter effect (default: 50)
- **skipOnInput**: Allow skipping typewriter animation (default: true)
- **fadeInDuration**: Duration of fade in animation in seconds (default: 0.3)
- **fadeOutDuration**: Duration of fade out animation in seconds (default: 0.3)
- **interactionButton**: Input button name for advancing dialogue (default: "Interact")

### Usage

This prefab is automatically managed by the DialogueManager. When a dialogue starts, the DialogueManager displays this UI and feeds it the dialogue text. The UI handles:

1. Fading in when dialogue starts
2. Displaying text with typewriter effect
3. Showing/hiding the continue indicator
4. Handling player input to skip typewriter or advance pages
5. Fading out when dialogue ends

### Customization

- **Background**: Adjust the DialogueBox Image color and alpha for different visual styles
- **Text Style**: Modify the DialogueText font, size, color, and alignment
- **Position**: Change the DialogueBox anchors to position the dialogue box differently (top, middle, etc.)
- **Size**: Adjust the DialogueBox anchor percentages to make the box larger or smaller
- **Continue Indicator**: Change the ContinueIndicator text to use a different symbol or replace with an Image component for a sprite-based indicator
- **Typewriter Speed**: Adjust in the DialogueUI component or per-NPC in NPCDialogueInteraction

### Notes

- The Canvas uses Screen Space - Overlay mode to ensure it appears above game elements
- The CanvasGroup on DialogueBox enables smooth fade in/out transitions
- The continue indicator only appears when there are more pages to display
- The prefab starts inactive and is activated by the DialogueManager
- TextMeshPro is required for the text components

---

## InteractionIcon Prefab

## Overview

The InteractionIcon prefab is a UI element that displays an interaction prompt above NPCs when the player is in range.

## Structure

- **InteractionIcon** (Root GameObject)
  - Canvas (Screen Space - Overlay)
  - Canvas Scaler
  - Graphic Raycaster
  - CanvasGroup (for fade effects)
  - InteractionIcon Component (script)
  
  - **Icon** (Child GameObject)
    - RectTransform (64x64 size)
    - Image (displays the interaction sprite)
    - Animator (plays bounce animation)

## Components

### InteractionIcon Script

- **Canvas Group**: Reference to the CanvasGroup for fade in/out effects
- **Icon Transform**: Reference to the Icon child's RectTransform
- **Fade Speed**: Speed of fade in/out animation (default: 5)
- **Bounce Speed**: Speed of the bounce animation (default: 2)
- **Bounce Height**: Height of the bounce in pixels (default: 10)
- **World Offset**: Offset from the NPC position in world space (default: 0, 1.5, 0)

### Animator

The Icon child has an Animator component that plays the "Bounce" animation continuously, creating a subtle up-and-down motion.

## Usage

This prefab is automatically instantiated by the NPCDialogueInteraction component when an NPC is configured for dialogue. You typically don't need to manually place this in scenes.

## Customization

- **Sprite**: Change the sprite in the Icon's Image component to use a different interaction icon
- **Size**: Adjust the Icon's RectTransform size to make the icon larger or smaller
- **Animation**: Modify the InteractionIconBounce animation to change the bounce behavior
- **Colors**: Adjust the Image color to tint the icon

## Animation

The prefab includes:

- **InteractionIconBounce.anim**: A looping animation that moves the icon up and down
- **InteractionIcon.controller**: The animator controller that plays the bounce animation

## Notes

- The Canvas is set to Screen Space - Overlay with a sorting order of 100 to ensure it appears above most UI elements
- The icon automatically follows the NPC's position in world space and converts it to screen space
- The icon fades in when shown and fades out when hidden
- The GameObject is automatically deactivated when fully invisible for performance optimization
