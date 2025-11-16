# Manual Testing Checklist - Task 7

Quick reference checklist for manual testing. Check off items as you complete them.

---

## 🎯 Task 7.1: HUD Crystal Validation

### Initial Setup

- [ ] Unity Editor open
- [ ] Test scene loaded
- [ ] HUD visible in Game view
- [ ] All 6 crystal counters showing "x0"

### Crystal Type Tests

- [ ] **Nature Crystal:** Counter updates to "x1", green color, correct icon
- [ ] **Fire Crystal:** Counter updates to "x1", red/orange color, correct icon
- [ ] **Water Crystal:** Counter updates to "x1", blue color, correct icon
- [ ] **Shadow Crystal:** Counter updates to "x1", dark/purple color, correct icon
- [ ] **Earth Crystal:** Counter updates to "x1", brown color, correct icon
- [ ] **Air Crystal:** Counter updates to "x1", light blue/white color, correct icon

### Format Validation

- [ ] All counters use "x{number}" format (e.g., "x10", not "10")
- [ ] Multiple collections increment correctly (x1 → x2 → x3)
- [ ] Mixed collections update independently

### Visual Quality

- [ ] No text overflow or clipping
- [ ] Colors are distinct and readable
- [ ] Icons are clear and recognizable
- [ ] Updates are immediate (no lag)

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Complete

---

## 🧲 Task 7.2: Magnetic Attraction Validation

### Basic Attraction

- [ ] Crystals remain still outside attraction radius
- [ ] Crystals move toward player inside radius
- [ ] Items remain still outside attraction radius
- [ ] Items move toward player inside radius

### Movement Quality

- [ ] Movement is smooth (no jittering)
- [ ] Speed is appropriate (not too fast/slow)
- [ ] Acceleration feels natural
- [ ] Collection happens at correct distance (≤ 0.2 units)

### Activation Delay

- [ ] Delay prevents immediate attraction
- [ ] Items start moving after configured delay
- [ ] Delay is consistent across item types

### Absorption Animation

- [ ] Scale up animation is visible
- [ ] Fade out animation is smooth
- [ ] Animation completes before destruction
- [ ] No sudden disappearances

### Multiple Items

- [ ] 10+ items can be attracted simultaneously
- [ ] No collision issues between items
- [ ] All items collected successfully
- [ ] No performance drops

### Edge Cases

- [ ] Items stop when player moves away
- [ ] Items resume when player returns
- [ ] Rapid player movement handled smoothly

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Complete

---

## 🎨 Task 7.3: VFX and SFX Validation

### Crystal VFX

- [ ] **Nature:** Green particles/leaves spawn and play
- [ ] **Fire:** Red/orange flames/sparks spawn and play
- [ ] **Water:** Blue droplets/splash spawn and play
- [ ] **Shadow:** Dark smoke/purple wisps spawn and play
- [ ] **Earth:** Brown dust/rocks spawn and play
- [ ] **Air:** White/light blue wind/swirls spawn and play

### Crystal SFX

- [ ] **Nature:** Nature-themed sound plays
- [ ] **Fire:** Crackling/whoosh sound plays
- [ ] **Water:** Splash/bubble sound plays
- [ ] **Shadow:** Whisper/dark tone plays
- [ ] **Earth:** Thud/rumble sound plays
- [ ] **Air:** Whoosh/wind chime sound plays

### Item VFX & SFX

- [ ] Regular items have distinct VFX
- [ ] Regular items have distinct SFX
- [ ] Different item types have appropriate effects

### Animation Quality

- [ ] Scale up is smooth and noticeable
- [ ] Fade out is gradual and complete
- [ ] Scale and fade happen simultaneously
- [ ] Item is invisible before destruction

### Effect Synchronization

- [ ] Attraction → Scale/Fade → VFX → SFX → Update → Destroy
- [ ] Timing feels natural and satisfying
- [ ] No visual glitches or overlaps

### Rapid Collection

- [ ] 10+ items collected rapidly
- [ ] All VFX spawn correctly
- [ ] All SFX play (may overlap)
- [ ] No missing effects
- [ ] No performance issues
- [ ] No audio clipping/distortion

### Error Handling

- [ ] Missing VFX reference doesn't crash
- [ ] Missing SFX reference doesn't crash
- [ ] Warnings logged for missing references

**Status:** ⬜ Not Started | ⏳ In Progress | ✅ Complete

---

## 📊 Overall Progress

- **Task 7.1 (HUD):** ⬜ Not Started | ⏳ In Progress | ✅ Complete
- **Task 7.2 (Attraction):** ⬜ Not Started | ⏳ In Progress | ✅ Complete
- **Task 7.3 (VFX/SFX):** ⬜ Not Started | ⏳ In Progress | ✅ Complete

**Task 7 Overall:** ⬜ Not Started | ⏳ In Progress | ✅ Complete

---

## 🐛 Issues Found

Document any issues here:

```
[Issue 1]
Test: 
Severity: 
Description: 
Status: 

[Issue 2]
Test: 
Severity: 
Description: 
Status: 
```

---

## ✅ Sign-Off

- **Tester Name:** _______________
- **Date:** _______________
- **All Tests Passed:** ⬜ Yes | ⬜ No (see issues above)
- **Ready for Next Task:** ⬜ Yes | ⬜ No

---

## 📝 Notes

Additional observations or comments:

```
[Your notes here]
```
