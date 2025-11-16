# Manual Testing Guide - Task 7: Visual Validation

## Overview

This guide provides step-by-step instructions for manually testing and validating the collectable items restructure system. These tests require human observation and interaction with the Unity Editor.

**Prerequisites:**

- Unity Editor is open
- Scene with test items is loaded (or use the test scene setup below)
- Player character (Slime) is controllable
- All previous tasks (1-6) are completed

---

## Test Setup Instructions

### Creating a Test Scene

1. **Open or Create Test Scene:**
   - Open scene: `Assets/Game/Scenes/2_InitialCave.unity` (or your main test scene)
   - Or create new scene: `File > New Scene`

2. **Ensure Required GameObjects Exist:**
   - CanvasHUD with CrystalHUDController component
   - Player character (Slime)
   - GameManager in scene
   - InventoryManager in scene

3. **Spawn Test Items:**
   - Use the ItemCollectable prefabs from your project
   - Place them in visible locations near the player spawn
   - Ensure you have at least one of each crystal type:
     - Nature Crystal
     - Fire Crystal
     - Water Crystal
     - Shadow Crystal
     - Earth Crystal
     - Air Crystal
   - Also place some regular inventory items for comparison

---

## Task 7.1: Validar HUD de Cristais

**Requirements Tested:** 3.1, 3.2, 3.3, 3.4

### Test Checklist

#### Setup

- [ ] Open Unity Editor
- [ ] Load test scene with crystal items
- [ ] Locate the HUD CrystalContainer in the Hierarchy
- [ ] Verify all 6 crystal count texts are visible in Game view
- [ ] Note initial counter values (should be "x0" for all)

#### Test Procedure

**Test 7.1.1: Nature Crystal Collection**

- [ ] Position player near Nature crystal
- [ ] Collect the Nature crystal
- [ ] **Verify:** Nature counter updates (e.g., "x0" → "x1")
- [ ] **Verify:** Format is correct: "x{number}" (e.g., "x1", "x5", "x10")
- [ ] **Verify:** Only Nature counter changed, others remain unchanged
- [ ] **Verify:** Counter color matches Nature theme (green tones)
- [ ] **Verify:** Nature icon is visible and correct

**Test 7.1.2: Fire Crystal Collection**

- [ ] Position player near Fire crystal
- [ ] Collect the Fire crystal
- [ ] **Verify:** Fire counter updates correctly
- [ ] **Verify:** Format is "x{number}"
- [ ] **Verify:** Counter color matches Fire theme (red/orange tones)
- [ ] **Verify:** Fire icon is visible and correct

**Test 7.1.3: Water Crystal Collection**

- [ ] Position player near Water crystal
- [ ] Collect the Water crystal
- [ ] **Verify:** Water counter updates correctly
- [ ] **Verify:** Format is "x{number}"
- [ ] **Verify:** Counter color matches Water theme (blue tones)
- [ ] **Verify:** Water icon is visible and correct

**Test 7.1.4: Shadow Crystal Collection**

- [ ] Position player near Shadow crystal
- [ ] Collect the Shadow crystal
- [ ] **Verify:** Shadow counter updates correctly
- [ ] **Verify:** Format is "x{number}"
- [ ] **Verify:** Counter color matches Shadow theme (dark/purple tones)
- [ ] **Verify:** Shadow icon is visible and correct

**Test 7.1.5: Earth Crystal Collection**

- [ ] Position player near Earth crystal
- [ ] Collect the Earth crystal
- [ ] **Verify:** Earth counter updates correctly
- [ ] **Verify:** Format is "x{number}"
- [ ] **Verify:** Counter color matches Earth theme (brown/tan tones)
- [ ] **Verify:** Earth icon is visible and correct

**Test 7.1.6: Air Crystal Collection**

- [ ] Position player near Air crystal
- [ ] Collect the Air crystal
- [ ] **Verify:** Air counter updates correctly
- [ ] **Verify:** Format is "x{number}"
- [ ] **Verify:** Counter color matches Air theme (light blue/white tones)
- [ ] **Verify:** Air icon is visible and correct

**Test 7.1.7: Multiple Collections**

- [ ] Spawn 5 Nature crystals
- [ ] Collect all 5 Nature crystals
- [ ] **Verify:** Counter increments correctly (x1 → x2 → x3 → x4 → x5)
- [ ] **Verify:** Each collection updates the counter immediately
- [ ] **Verify:** No lag or delay in counter updates

**Test 7.1.8: Mixed Collections**

- [ ] Collect crystals in random order (Fire, Nature, Water, Shadow, etc.)
- [ ] **Verify:** Each counter updates independently
- [ ] **Verify:** No cross-contamination (Fire collection doesn't affect Water counter)
- [ ] **Verify:** All counters maintain correct values

#### Expected Results

✅ All crystal counters display format "x{number}"
✅ Counters update immediately upon collection
✅ Each counter is independent and accurate
✅ Colors and icons match crystal types
✅ No visual glitches or text overflow

#### Failure Scenarios

❌ Counter doesn't update → Check CrystalHUDController event subscription
❌ Wrong format (e.g., "10" instead of "x10") → Check countFormat in CrystalHUDController
❌ Wrong counter updates → Check CrystalType mapping in crystalTextMap
❌ Counter shows "x0" after collection → Check GameManager.AddCrystal is being called

---

## Task 7.2: Validar Atração Magnética

**Requirements Tested:** 8.1, 8.2, 8.3, 8.4, 8.5

### Test Checklist

#### Setup

- [ ] Spawn multiple crystals at varying distances from player
- [ ] Spawn multiple inventory items at varying distances
- [ ] Note the attraction radius configured in ItemCollectable (default: 3 units)

#### Test Procedure

**Test 7.2.1: Crystal Magnetic Attraction**

- [ ] Position player 5 units away from a crystal (outside attraction radius)
- [ ] **Verify:** Crystal remains stationary
- [ ] Move player to 2.5 units away (inside attraction radius)
- [ ] **Verify:** Crystal starts moving toward player
- [ ] **Verify:** Movement is smooth and continuous
- [ ] **Verify:** Crystal accelerates as it gets closer
- [ ] Stand still and let crystal reach player
- [ ] **Verify:** Crystal is collected when it reaches player (distance ≤ 0.2 units)

**Test 7.2.2: Item Magnetic Attraction**

- [ ] Position player 5 units away from an inventory item
- [ ] **Verify:** Item remains stationary
- [ ] Move player to 2.5 units away
- [ ] **Verify:** Item starts moving toward player
- [ ] **Verify:** Movement speed matches crystal attraction speed
- [ ] **Verify:** Item is collected upon reaching player

**Test 7.2.3: Activation Delay**

- [ ] Spawn a crystal with activationDelay = 0.5s
- [ ] Move player into attraction radius
- [ ] **Verify:** Crystal doesn't move immediately
- [ ] Wait 0.5 seconds
- [ ] **Verify:** Crystal starts moving after delay
- [ ] Repeat with activationDelay = 1.0s
- [ ] **Verify:** Delay is respected

**Test 7.2.4: Absorption Animation**

- [ ] Position player to collect a crystal
- [ ] Watch the final moments before collection
- [ ] **Verify:** Crystal scales up slightly before collection
- [ ] **Verify:** Crystal fades out (alpha decreases)
- [ ] **Verify:** Animation is smooth (no sudden jumps)
- [ ] **Verify:** Crystal disappears completely after animation

**Test 7.2.5: Multiple Items Attraction**

- [ ] Spawn 10 crystals in a circle around player (all within attraction radius)
- [ ] Stand still
- [ ] **Verify:** All crystals move toward player simultaneously
- [ ] **Verify:** No collision issues between crystals
- [ ] **Verify:** All crystals are collected successfully
- [ ] **Verify:** No performance issues (smooth framerate)

**Test 7.2.6: Edge Cases**

- [ ] Move player away while crystal is being attracted
- [ ] **Verify:** Crystal stops moving when outside radius
- [ ] Move player back into radius
- [ ] **Verify:** Crystal resumes attraction
- [ ] Move player rapidly in circles
- [ ] **Verify:** Crystals follow smoothly without jittering

#### Expected Results

✅ Crystals and items are attracted within configured radius
✅ Attraction is smooth and natural-looking
✅ Activation delay works correctly
✅ Absorption animation plays before collection
✅ Multiple items can be attracted simultaneously
✅ No performance issues with many items

#### Failure Scenarios

❌ Items don't move → Check attraction radius and CheckPlayerDistance coroutine
❌ Items jitter → Check attraction speed and Time.deltaTime usage
❌ No delay → Check activationDelay configuration
❌ No animation → Check absorption animation coroutine

---

## Task 7.3: Validar Efeitos Visuais e Sonoros

**Requirements Tested:** 7.1, 7.2, 7.3, 7.4, 7.5

### Test Checklist

#### Setup

- [ ] Ensure audio is enabled in Unity Editor (unmute)
- [ ] Set Game view to maximize for better VFX visibility
- [ ] Prepare crystals and items with configured VFX and SFX

#### Test Procedure

**Test 7.3.1: Crystal VFX**

- [ ] Collect a Nature crystal
- [ ] **Verify:** VFX spawns at crystal position
- [ ] **Verify:** VFX matches Nature theme (green particles, leaves, etc.)
- [ ] **Verify:** VFX plays completely before disappearing
- [ ] **Verify:** VFX doesn't persist in scene after collection
- [ ] Repeat for each crystal type:
  - [ ] Fire: Red/orange flames, sparks
  - [ ] Water: Blue droplets, splash
  - [ ] Shadow: Dark smoke, purple wisps
  - [ ] Earth: Brown dust, rocks
  - [ ] Air: White/light blue wind, swirls

**Test 7.3.2: Crystal SFX**

- [ ] Collect a Nature crystal
- [ ] **Verify:** Sound plays immediately upon collection
- [ ] **Verify:** Sound matches Nature theme (chime, nature sounds)
- [ ] **Verify:** Sound volume is appropriate (not too loud/quiet)
- [ ] **Verify:** Sound doesn't cut off prematurely
- [ ] Repeat for each crystal type:
  - [ ] Fire: Crackling, whoosh
  - [ ] Water: Splash, bubble
  - [ ] Shadow: Whisper, dark tone
  - [ ] Earth: Thud, rumble
  - [ ] Air: Whoosh, wind chime

**Test 7.3.3: Item VFX**

- [ ] Collect a regular inventory item
- [ ] **Verify:** VFX spawns at item position
- [ ] **Verify:** VFX is distinct from crystal VFX
- [ ] **Verify:** VFX plays completely
- [ ] Collect different item types
- [ ] **Verify:** Each item type has appropriate VFX

**Test 7.3.4: Item SFX**

- [ ] Collect a regular inventory item
- [ ] **Verify:** Sound plays upon collection
- [ ] **Verify:** Sound is distinct from crystal sounds
- [ ] **Verify:** Sound volume is consistent with crystal sounds
- [ ] Collect different item types
- [ ] **Verify:** Each item type has appropriate SFX

**Test 7.3.5: Scale Up Animation**

- [ ] Position camera close to a crystal
- [ ] Collect the crystal
- [ ] **Verify:** Crystal scales up (grows larger) before disappearing
- [ ] **Verify:** Scale animation is smooth (no sudden jumps)
- [ ] **Verify:** Scale increase is noticeable but not excessive
- [ ] Repeat with inventory item
- [ ] **Verify:** Same scale animation behavior

**Test 7.3.6: Fade Out Animation**

- [ ] Collect a crystal while watching closely
- [ ] **Verify:** Crystal fades out (alpha decreases to 0)
- [ ] **Verify:** Fade is smooth and gradual
- [ ] **Verify:** Fade happens simultaneously with scale up
- [ ] **Verify:** Crystal is completely invisible before being destroyed
- [ ] Repeat with inventory item
- [ ] **Verify:** Same fade behavior

**Test 7.3.7: Combined Effects**

- [ ] Collect a crystal
- [ ] **Verify:** All effects happen in correct order:
  1. Attraction animation (movement toward player)
  2. Scale up + Fade out (absorption animation)
  3. VFX spawns
  4. SFX plays
  5. HUD counter updates (for crystals)
  6. Item disappears from scene
- [ ] **Verify:** Timing feels natural and satisfying
- [ ] **Verify:** No visual glitches or overlapping issues

**Test 7.3.8: Rapid Collection**

- [ ] Spawn 10 crystals close together
- [ ] Collect all rapidly (within 2 seconds)
- [ ] **Verify:** All VFX spawn correctly
- [ ] **Verify:** All SFX play (may overlap, but all audible)
- [ ] **Verify:** No missing effects
- [ ] **Verify:** No performance issues (framerate stays smooth)
- [ ] **Verify:** Audio doesn't clip or distort

**Test 7.3.9: Edge Cases**

- [ ] Collect crystal with missing VFX reference
- [ ] **Verify:** Collection still works (no crash)
- [ ] **Verify:** Warning logged in console
- [ ] Collect crystal with missing SFX reference
- [ ] **Verify:** Collection still works (no crash)
- [ ] **Verify:** Warning logged in console

#### Expected Results

✅ VFX spawns and plays correctly for all item types
✅ SFX plays immediately upon collection
✅ Scale up animation is smooth and noticeable
✅ Fade out animation is smooth and complete
✅ All effects are synchronized and feel polished
✅ No performance issues with multiple simultaneous collections
✅ Missing effects are handled gracefully

#### Failure Scenarios

❌ No VFX → Check collectVFX reference in CrystalElementalData/ItemData
❌ No SFX → Check collectSound reference and AudioSource
❌ VFX persists → Check VFX lifetime or destroy call
❌ No scale/fade → Check absorption animation coroutine
❌ Effects out of sync → Check timing in collection sequence

---

## Reporting Issues

### Issue Template

When you find an issue, document it using this template:

```
**Issue ID:** [Unique identifier, e.g., T7.1-001]
**Test:** [Which test, e.g., 7.1.3 Water Crystal Collection]
**Severity:** [Critical / High / Medium / Low]
**Description:** [What went wrong]
**Steps to Reproduce:**
1. [Step 1]
2. [Step 2]
3. [Step 3]
**Expected Result:** [What should happen]
**Actual Result:** [What actually happened]
**Screenshots/Video:** [If applicable]
**Console Logs:** [Any relevant errors or warnings]
```

### Example Issue

```
**Issue ID:** T7.1-001
**Test:** 7.1.3 Water Crystal Collection
**Severity:** High
**Description:** Water crystal counter shows "10" instead of "x10"
**Steps to Reproduce:**
1. Spawn Water crystal
2. Collect Water crystal
3. Check HUD counter
**Expected Result:** Counter shows "x1"
**Actual Result:** Counter shows "1"
**Console Logs:** None
```

---

## Completion Criteria

Task 7 is considered complete when:

✅ **Task 7.1:** All 6 crystal types update HUD correctly with "x{number}" format
✅ **Task 7.2:** Magnetic attraction works smoothly for all item types
✅ **Task 7.3:** All VFX and SFX play correctly for all item types
✅ All checklist items are marked complete
✅ All issues found are documented
✅ Critical and High severity issues are resolved

---

## Quick Reference

### Console Commands for Testing

```csharp
// Spawn test crystals (if you have a spawn command)
SpawnCrystal(CrystalType.Nature, Vector3.zero);

// Reset crystal counters
GameManager.Instance.ResetAllCrystals();

// Check current crystal count
Debug.Log(GameManager.Instance.GetCrystalCount(CrystalType.Nature));
```

### Useful Unity Shortcuts

- **Play Mode:** Ctrl/Cmd + P
- **Pause:** Ctrl/Cmd + Shift + P
- **Frame Advance:** Ctrl/Cmd + Alt + P
- **Scene View:** Ctrl/Cmd + 1
- **Game View:** Ctrl/Cmd + 2
- **Console:** Ctrl/Cmd + Shift + C

---

## Notes

- Take your time with each test - manual testing requires careful observation
- Use Unity's Frame Debugger (Window > Analysis > Frame Debugger) to inspect VFX rendering
- Use Unity's Profiler (Window > Analysis > Profiler) to check performance
- Record videos of tests for documentation if needed
- Test in both Editor and Build if possible

---

**Good luck with testing! 🎮**
