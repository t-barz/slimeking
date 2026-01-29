# BeeWorker Animation Events Configuration Guide

## Overview

This document provides step-by-step instructions for configuring Animation Events on the BeeWorker enemy animations. Animation Events are used to synchronize gameplay logic with animation frames, ensuring proper timing for attack hitboxes, visual effects, and state transitions.

## Prerequisites

- BeeWorker Animator Controller must be created and configured
- The following animations must exist:
  - **Attack** - The attack animation
  - **Hit** - The damage reaction animation
  - **Die** - The death animation

## Required Animation Events

The `BeeWorkerBehaviorController` script provides the following public methods that must be called via Animation Events:

### Public Methods Available

1. **`EnableHitBox()`** - Enables the attack hitbox collider
2. **`DisableHitBox()`** - Disables the attack hitbox collider
3. **`EnableBouncing()`** - Enables the visual bouncing effect
4. **`DisableBouncing()`** - Disables the visual bouncing effect
5. **`OnDeathAnimationComplete()`** - Destroys the GameObject after death animation

## Animation Event Configuration

### Attack Animation

The Attack animation requires **4 Animation Events** to properly control the attack sequence:

| Frame | Event Method | Purpose |
|-------|--------------|---------|
| **Start Frame** (frame 0 or first frame) | `DisableBouncing()` | Stops the idle bouncing effect during attack |
| **Damage Frame** (when attack should hit) | `EnableHitBox()` | Activates the hitbox to deal damage to player |
| **After Damage Frame** (1-2 frames after damage) | `DisableHitBox()` | Deactivates the hitbox after damage window |
| **End Frame** (last frame) | `EnableBouncing()` | Resumes the idle bouncing effect |

**Configuration Steps:**
1. Open the BeeWorker Animator Controller in the Animation window
2. Select the **Attack** animation clip
3. In the Animation window, locate the Event timeline (bottom section)
4. Add events at the appropriate frames:
   - Click the frame number where you want to add an event
   - Click the "Add Event" button (or right-click → Add Animation Event)
   - In the Inspector, select the function name from the dropdown
   - Repeat for all 4 events

**Timing Recommendations:**
- **Start Frame**: Frame 0 (immediate)
- **Damage Frame**: Typically 40-60% through the animation (when the attack visually connects)
- **After Damage Frame**: 2-3 frames after the damage frame
- **End Frame**: Last frame of the animation

### Hit Animation

The Hit animation requires **1 Animation Event**:

| Frame | Event Method | Purpose |
|-------|--------------|---------|
| **End Frame** (last frame) | `EnableBouncing()` | Resumes bouncing after damage reaction completes |

**Configuration Steps:**
1. Select the **Hit** animation clip
2. Add an Animation Event on the last frame
3. Set the function to `EnableBouncing()`

**Note:** Bouncing is automatically disabled when the Hit state is entered via the `TakeDamage()` method.

### Die Animation

The Die animation requires **1 Animation Event**:

| Frame | Event Method | Purpose |
|-------|--------------|---------|
| **End Frame** (last frame) | `OnDeathAnimationComplete()` | Destroys the GameObject after death animation finishes |

**Configuration Steps:**
1. Select the **Die** animation clip
2. Add an Animation Event on the last frame
3. Set the function to `OnDeathAnimationComplete()`

**Important:** This event is critical - without it, the BeeWorker GameObject will remain in the scene after death.

## Verification Checklist

After configuring all Animation Events, verify the following:

- [ ] Attack animation has 4 events: `DisableBouncing()`, `EnableHitBox()`, `DisableHitBox()`, `EnableBouncing()`
- [ ] Hit animation has 1 event: `EnableBouncing()`
- [ ] Die animation has 1 event: `OnDeathAnimationComplete()`
- [ ] All event function names are spelled correctly (case-sensitive)
- [ ] Events are placed on appropriate frames
- [ ] Test in Play mode:
  - [ ] Attack hitbox only deals damage during the damage window
  - [ ] Bouncing stops during attack and resumes after
  - [ ] Bouncing resumes after taking damage
  - [ ] BeeWorker is destroyed after death animation completes

## Testing in Play Mode

1. **Test Attack Sequence:**
   - Enter Play mode
   - Trigger the BeeWorker's attack (get within attack range)
   - Observe that bouncing stops during attack
   - Verify the hitbox only deals damage during the damage frame window
   - Confirm bouncing resumes after attack completes

2. **Test Hit Reaction:**
   - Deal damage to the BeeWorker
   - Observe the Hit animation plays
   - Verify bouncing resumes after the Hit animation completes

3. **Test Death:**
   - Reduce BeeWorker health to zero
   - Observe the Die animation plays
   - Verify the GameObject is destroyed when the animation completes
   - Check the Console for the debug log: "Death animation complete. Destroying GameObject."

## Troubleshooting

### Issue: Animation Events not firing

**Possible Causes:**
- Function name is misspelled or has incorrect capitalization
- The BeeWorkerBehaviorController script is not attached to the GameObject
- The script is disabled
- The Animation Event is on the wrong GameObject in the hierarchy

**Solution:**
- Verify the function name matches exactly (case-sensitive)
- Ensure the script is attached to the root GameObject with the Animator
- Check that the script is enabled in the Inspector

### Issue: HitBox stays enabled after attack

**Possible Causes:**
- `DisableHitBox()` event is missing or on the wrong frame
- Animation is interrupted before the event fires

**Solution:**
- Verify the `DisableHitBox()` event exists and is after the `EnableHitBox()` event
- Ensure the animation plays to completion
- Add additional safety: call `DisableHitBox()` in the attack animation's exit transition

### Issue: BeeWorker not destroyed after death

**Possible Causes:**
- `OnDeathAnimationComplete()` event is missing
- Die animation is set to loop
- Animation is interrupted

**Solution:**
- Add the `OnDeathAnimationComplete()` event to the last frame
- Ensure the Die animation is NOT set to loop
- Verify the Die animation plays to completion

## Requirements Validation

This configuration satisfies the following requirements:

- **Requirement 6.5**: Animation Events control HitBox enable/disable timing
- **Requirement 6.6**: HitBox is properly synchronized with attack animation
- **Requirement 7.7**: Bouncing is disabled during Attack animation
- **Requirement 7.8**: Bouncing is disabled in Hit state and re-enabled after
- **Requirement 14.3**: GameObject is destroyed when Die animation completes

## Related Files

- **Script**: `Assets/_Code/Gameplay/Enemies/BeeWorkerBehaviorController.cs`
- **Prefab**: `Assets/_Prefabs/NPCs/BeeWorkerA.prefab`
- **Design Document**: `.kiro/specs/beeworker-enemy-ai/design.md`
- **Requirements**: `.kiro/specs/beeworker-enemy-ai/requirements.md`

## Notes

- Animation Events are stored in the Animation Clip asset, not in the Animator Controller
- Changes to Animation Events require saving the Animation Clip
- Animation Events can only call public methods with zero or one parameter
- The GameObject with the Animator component must have the script attached
- Animation Events are frame-based, so timing depends on animation frame rate

---

**Last Updated**: 2024
**Task**: BeeWorker Enemy AI - Task 36: Configure Animation Events
**Status**: Manual configuration required in Unity Editor
