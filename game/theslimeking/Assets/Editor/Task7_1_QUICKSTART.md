# Task 7.1 Quick Start Guide

## 🎯 Goal

Validate that crystal HUD counters update correctly with format "x{number}", appropriate colors, and correct icons.

---

## ⚡ Quick Start (5 Minutes)

### Step 1: Open the Validator Tool

1. Open Unity Editor
2. Go to menu: `The Slime King > Tests > Task 7.1 - HUD Validator`
3. A new window will open

### Step 2: Enter Play Mode

1. Press `Ctrl/Cmd + P` or click Play button
2. Wait for scene to load

### Step 3: Run Automated Validation

1. In the HUD Validator window, expand "Automated Validation"
2. Click "Run Automated Validation" button
3. Review results in "Validation Results" section

**Expected:** All 6 crystal types show ✓ (green checkmark)

### Step 4: Test Quick Actions

1. Expand "Quick Actions" section
2. Click "Add 1 to All Crystals"
3. **Look at Game View** - verify all counters show "x1"
4. Click "Add 10 to All Crystals"
5. **Look at Game View** - verify all counters show "x11"
6. Click "Reset All Counters"
7. **Look at Game View** - verify all counters show "x0"

### Step 5: Test Individual Crystals

1. In "Quick Actions", find the individual crystal tests
2. For each crystal type (Nature, Fire, Water, Shadow, Earth, Air):
   - Click "+1" button
   - **Look at Game View** - verify counter updates
   - **Check color** - verify it matches crystal theme
   - **Check icon** - verify it's visible and correct

### Step 6: Test Multiple Collections

1. Click "Reset All Counters"
2. Click "+1" for Nature crystal 5 times
3. **Look at Game View** - verify counter shows "x5"
4. Verify format is "x5" (not "5" or "5x")

### Step 7: Test Mixed Collections

1. Click "Reset All Counters"
2. Click "+1" for different crystals in random order
3. **Look at Game View** - verify each counter updates independently
4. Verify no cross-contamination (Fire doesn't affect Water, etc.)

---

## ✅ Pass Criteria

Task 7.1 passes if:

- ✓ Automated validation shows all green checkmarks
- ✓ All counters use format "x{number}"
- ✓ All counters update immediately when crystals added
- ✓ Each counter is independent (no cross-contamination)
- ✓ Colors match crystal themes (green, red, blue, purple, brown, light blue)
- ✓ All icons are visible and correct

---

## 🐛 Common Issues

### Issue: Automated validation fails

**Solution:**

1. Check that CrystalHUDController is attached to CanvasHUD
2. Right-click CrystalHUDController in Inspector
3. Select "Auto-Find Text References"
4. Run validation again

### Issue: Counters don't update

**Solution:**

1. Check Console for errors
2. Verify GameManager is in scene
3. Verify CrystalHUDController is enabled
4. Check that you're in Play Mode

### Issue: Wrong format (shows "10" instead of "x10")

**Solution:**

1. Select CrystalHUDController in Inspector
2. Check "Count Format" field - should be "x{0}"
3. If wrong, change to "x{0}" and test again

### Issue: Colors are all white/black

**Solution:**

1. Check Count_Text components in CrystalContainer
2. Verify each has appropriate color set
3. Colors should be:
   - Nature: Green (#00FF00 or similar)
   - Fire: Red/Orange (#FF4500 or similar)
   - Water: Blue (#0080FF or similar)
   - Shadow: Purple/Dark (#8000FF or similar)
   - Earth: Brown (#8B4513 or similar)
   - Air: Light Blue/White (#87CEEB or similar)

---

## 📊 Test Results Template

Copy this to document your results:

```
Task 7.1 Test Results
Date: [DATE]
Tester: [NAME]

Automated Validation: ⬜ Pass | ⬜ Fail
Quick Actions Test: ⬜ Pass | ⬜ Fail
Individual Crystals: ⬜ Pass | ⬜ Fail
Multiple Collections: ⬜ Pass | ⬜ Fail
Mixed Collections: ⬜ Pass | ⬜ Fail

Issues Found:
[List any issues]

Overall Status: ⬜ Pass | ⬜ Fail
Ready for Task 7.2: ⬜ Yes | ⬜ No
```

---

## 📚 Additional Resources

- **Full Test Report:** `Assets/Editor/Task7_1_TestReport.md`
- **Manual Testing Guide:** `Assets/Editor/ManualTestingGuide_Task7.md`
- **Manual Testing Checklist:** `Assets/Editor/ManualTestingChecklist_Task7.md`
- **Requirements:** `.kiro/specs/collectable-items-restructure/requirements.md` (3.1, 3.2, 3.3, 3.4)
- **Design:** `.kiro/specs/collectable-items-restructure/design.md`

---

## 🎮 In-Game Testing (Optional)

After passing all validator tests, test with actual gameplay:

1. Load scene `2_InitialCave` (or your test scene)
2. Enter Play Mode
3. Find or spawn crystal collectables in scene
4. Move player to collect crystals
5. Verify HUD updates correctly during gameplay

---

## ✨ Tips

- **Use Game View:** Always watch the Game View (not Scene View) to see HUD updates
- **Maximize Game View:** Click "Maximize on Play" for better visibility
- **Check Console:** Watch Console for any errors or warnings
- **Take Screenshots:** Document any issues with screenshots
- **Test Thoroughly:** Don't rush - verify each crystal type carefully

---

## 🚀 Next Steps

After Task 7.1 is complete:

1. Mark task as complete in `tasks.md`
2. Update test report with results
3. Proceed to **Task 7.2: Validar Atração Magnética**
4. Use `Manual Testing Helper` for Task 7.2

---

**Good luck! 🎮**
