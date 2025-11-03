# NPCQuickConfig Testing Guide

## Overview

This document provides step-by-step instructions for manually testing the NPCQuickConfig feature. Follow each test case in order to verify all functionality works correctly.

---

## Prerequisites

Before starting the tests, ensure:

- Unity Editor is open
- The project is loaded
- All NPCQuickConfig scripts are compiled without errors
- Template assets exist in `Assets/Data/QuickWins/Templates/NPCTemplates/`

---

## Test 11.1: Create Test Scene with Empty GameObjects

### Steps

1. **Create a new test scene:**
   - In Unity, go to `File > New Scene`
   - Choose "Basic (Built-in)" or "2D (URP)" template
   - Save the scene as `Assets/Game/Scenes/NPCQuickConfig_TestScene.unity`

2. **Create test GameObjects:**
   - In the Hierarchy window, right-click and select `Create Empty`
   - Name it `TestNPC_CervoBroto`
   - Repeat to create `TestNPC_Esquilo`
   - Repeat to create `TestNPC_Abelha`

3. **Position the GameObjects:**
   - Select `TestNPC_CervoBroto` and set Position to `(0, 0, 0)`
   - Select `TestNPC_Esquilo` and set Position to `(5, 0, 0)`
   - Select `TestNPC_Abelha` and set Position to `(10, 0, 0)`

4. **Save the scene:**
   - Press `Ctrl+S` or `File > Save`

### Expected Result

✅ Test scene created with 3 empty GameObjects positioned in a row

---

## Test 11.2: Test Cervo-Broto Template Configuration

### Steps

1. **Select the GameObject:**
   - In the Hierarchy, click on `TestNPC_CervoBroto`

2. **Open NPCQuickConfig window:**
   - Go to menu `QuickWins > NPC Quick Config`
   - The window should open and display the selected GameObject name

3. **Select Cervo-Broto template:**
   - In the Template dropdown, select `Cervo-Broto`
   - Verify the description shows: "Passivo, wander behavior, amizade habilitada"

4. **Verify template values loaded:**
   - Behavior Type: `Passivo`
   - AI Type: `Wander`
   - Friendship Enabled: `✓` (checked)
   - Dialogue Enabled: `☐` (unchecked)
   - Wander Radius: `5.0`
   - Wander Speed: `2.0`
   - Pause Duration: `2.0`

5. **Apply configuration:**
   - Click the `Apply Configuration` button
   - Wait for the success message

6. **Verify components added:**
   - Select `TestNPC_CervoBroto` in Hierarchy
   - In the Inspector, verify these components exist:
     - ✅ `SpriteRenderer` (Sorting Layer: "Characters", Order: 10)
     - ✅ `Animator` (with controller assigned)
     - ✅ `CircleCollider2D` (Is Trigger: False, Radius: 0.5)
     - ✅ `Rigidbody2D` (Body Type: Dynamic, Gravity Scale: 0, Freeze Rotation Z)
     - ✅ `NPCController` (with NPCData reference)
     - ✅ `NPCBehavior` (Behavior Type: Passivo)
     - ✅ `NPCWanderAI` (Wander Radius: 5.0, Wander Speed: 2.0, Pause Duration: 2.0)
     - ✅ `NPCFriendship` (with FriendshipData reference)

7. **Verify ScriptableObjects created:**
   - Navigate to `Assets/Data/NPCs/`
   - Verify `TestNPC_CervoBrotoData.asset` exists
   - Click on it and verify fields are populated correctly
   - Navigate to `Assets/Data/NPCs/Friendship/`
   - Verify `CervoFriendshipData.asset` exists
   - Click on it and verify 5 friendship levels exist with Portuguese names

8. **Verify Animator Controller created:**
   - Navigate to `Assets/Art/Animations/NPCs/`
   - Verify `TestNPC_CervoBrotoController.controller` exists
   - Double-click to open Animator window
   - Verify states exist: Idle, Walk, Death
   - Verify parameters exist: Speed (float), IsDead (bool)
   - Verify transitions are configured

9. **Verify references:**
   - Select `TestNPC_CervoBroto` in Hierarchy
   - In Inspector, click on `NPCController` component
   - Verify `NPCData` field references `TestNPC_CervoBrotoData`
   - Click on `NPCFriendship` component
   - Verify `FriendshipData` field references `CervoFriendshipData`

### Expected Result

✅ All components added correctly
✅ All ScriptableObjects created in correct directories
✅ Animator Controller created with proper states and transitions
✅ All references are correctly assigned

---

## Test 11.3: Test Esquilo Coletor Template Configuration

### Steps

1. **Select the GameObject:**
   - In the Hierarchy, click on `TestNPC_Esquilo`

2. **Open NPCQuickConfig window:**
   - If not already open, go to `QuickWins > NPC Quick Config`

3. **Select Esquilo Coletor template:**
   - In the Template dropdown, select `Esquilo Coletor`
   - Verify the description shows: "Quest giver, static, diálogo habilitado"

4. **Verify template values loaded:**
   - Behavior Type: `QuestGiver`
   - AI Type: `Static`
   - Friendship Enabled: `✓` (checked)
   - Dialogue Enabled: `✓` (checked)
   - Dialogue Trigger Type: `Interaction`
   - Trigger Range: `2.0`

5. **Apply configuration:**
   - Click the `Apply Configuration` button
   - Wait for the success message

6. **Verify components added:**
   - Select `TestNPC_Esquilo` in Hierarchy
   - In the Inspector, verify these components exist:
     - ✅ `NPCStaticAI` (Static AI component)
     - ✅ `NPCDialogue` (Trigger Type: Interaction, Trigger Range: 2.0)
     - ✅ `NPCBehavior` (Behavior Type: QuestGiver)
     - ✅ `NPCFriendship` (with FriendshipData reference)

7. **Verify DialogueData created:**
   - Navigate to `Assets/Data/NPCs/Dialogues/`
   - Verify `TestNPC_EsquiloDialogueData.asset` exists
   - Click on it and verify placeholder dialogue exists in Portuguese

8. **Verify QuestGiver behavior:**
   - Check the Console window for a Debug.Log message about QuestGiver
   - Note: QuestGiver component is a placeholder (not implemented yet)

9. **Verify Animator includes Talk state:**
   - Navigate to `Assets/Art/Animations/NPCs/`
   - Open `TestNPC_EsquiloController.controller`
   - Verify states exist: Idle, Walk, Talk, Death
   - Verify parameter exists: IsTalking (bool)

### Expected Result

✅ Static AI component added
✅ Dialogue component added with correct settings
✅ DialogueData created with placeholder dialogue
✅ Animator includes Talk state and IsTalking parameter
✅ QuestGiver behavior type set (component is placeholder)

---

## Test 11.4: Test Abelha Cristalina Template Configuration

### Steps

1. **Select the GameObject:**
   - In the Hierarchy, click on `TestNPC_Abelha`

2. **Open NPCQuickConfig window:**
   - If not already open, go to `QuickWins > NPC Quick Config`

3. **Select Abelha Cristalina template:**
   - In the Template dropdown, select `Abelha Cristalina`
   - Verify the description shows: "Neutro, patrol behavior, detecção básica"

4. **Verify template values loaded:**
   - Behavior Type: `Neutro`
   - AI Type: `Patrol`
   - Detection Range: `5.0`
   - Patrol Speed: `2.5`
   - Wait at Point: `1.0`
   - Friendship Enabled: `✓` (checked)
   - Dialogue Enabled: `☐` (unchecked)

5. **Apply configuration:**
   - Click the `Apply Configuration` button
   - Wait for the success message

6. **Verify Patrol AI component:**
   - Select `TestNPC_Abelha` in Hierarchy
   - In the Inspector, find `NPCPatrolAI` component
   - Verify `Patrol Points` list has 4 points auto-generated
   - Verify points form a square pattern around the initial position
   - Verify `Patrol Speed`: 2.5
   - Verify `Wait At Point`: 1.0

7. **Verify detection range:**
   - Find `NPCBehavior` component
   - Verify `Detection Range`: 5.0
   - Verify `Behavior Type`: Neutro

8. **Verify Neutro behavior:**
   - The NPC should have neutral behavior (retaliates when attacked)

### Expected Result

✅ Patrol AI component added with 4 auto-generated patrol points
✅ Patrol points form a square pattern (radius ~3 units)
✅ Detection range set to 5.0
✅ Neutro behavior type configured

---

## Test 11.5: Test Validation Errors

### Test Case A: No GameObject Selected

1. **Deselect all GameObjects:**
   - Click on empty space in Hierarchy or Scene view

2. **Open NPCQuickConfig window:**
   - Go to `QuickWins > NPC Quick Config`

3. **Try to validate:**
   - Click `Validate Configuration` button

4. **Verify error message:**
   - Error should display: "Nenhum GameObject selecionado"
   - Apply Configuration button should be disabled or show error

### Test Case B: Missing Species Name with Friendship Enabled

1. **Select a GameObject:**
   - Create a new empty GameObject named `TestValidation`

2. **Configure with missing species:**
   - In NPCQuickConfig window, leave Template as `Custom`
   - Set NPC Name: `TestNPC`
   - Enable `Friendship System` checkbox
   - Leave `Species Name` field empty

3. **Try to validate:**
   - Click `Validate Configuration` button

4. **Verify error message:**
   - Error should display about Species Name being required
   - Message should be in Portuguese

### Expected Result

✅ Error displayed when no GameObject selected
✅ Error displayed when Species Name is empty with Friendship enabled
✅ Error messages are clear and in Portuguese
✅ Configuration is blocked until errors are fixed

---

## Test 11.6: Test Gizmo Visualization

### Test Case A: Wander Radius Gizmo

1. **Select configured NPC with Wander AI:**
   - Select `TestNPC_CervoBroto` in Hierarchy

2. **Open NPCQuickConfig window:**
   - Go to `QuickWins > NPC Quick Config`

3. **Enable gizmos:**
   - Check the `Show Gizmos in Scene View` checkbox

4. **View Scene View:**
   - Switch to Scene view (or have it visible alongside Game view)
   - Verify a yellow wire circle appears around the NPC
   - Verify the circle radius matches the Wander Radius (5.0 units)
   - Verify a label displays "Wander Radius: 5.0m" or similar

### Test Case B: Patrol Path Gizmo

1. **Select configured NPC with Patrol AI:**
   - Select `TestNPC_Abelha` in Hierarchy

2. **Enable gizmos:**
   - In NPCQuickConfig window, ensure `Show Gizmos in Scene View` is checked

3. **View Scene View:**
   - Verify cyan lines connect the 4 patrol points
   - Verify small spheres appear at each patrol point
   - Verify arrows or direction indicators show patrol direction
   - Verify labels show "Patrol Point 0", "Patrol Point 1", etc.

### Test Case C: Detection Range Gizmo

1. **Select NPC with detection range:**
   - Select `TestNPC_Abelha` (Neutro behavior with detection range)

2. **View Scene View:**
   - Verify a red wire circle appears around the NPC
   - Verify the circle has transparency (alpha)
   - Verify the radius matches Detection Range (5.0 units)
   - Verify a label displays "Detection: 5.0m" or similar

### Test Case D: Dialogue Trigger Range Gizmo

1. **Select NPC with dialogue enabled:**
   - Select `TestNPC_Esquilo` (has dialogue enabled)

2. **View Scene View:**
   - Verify a green wire circle appears around the NPC
   - Verify the circle has transparency (alpha)
   - Verify the radius matches Trigger Range (2.0 units)
   - Verify a label displays "Dialogue: 2.0m" or similar

### Expected Result

✅ Wander radius draws as yellow circle with label
✅ Patrol path draws with cyan lines, spheres, and labels
✅ Detection range draws as red circle with alpha
✅ Dialogue trigger range draws as green circle with alpha
✅ All gizmos are visible in Scene View when enabled

---

## Test 11.7: Test Custom Configuration

### Steps

1. **Create a new GameObject:**
   - In Hierarchy, create empty GameObject named `TestCustomNPC`

2. **Open NPCQuickConfig window:**
   - Go to `QuickWins > NPC Quick Config`

3. **Configure manually (don't select a template):**
   - Leave Template as `Custom`
   - Set NPC Name: `CustomNPC`
   - Set Behavior Type: `Neutro`
   - Set AI Type: `Wander`
   - Set Detection Range: `7.0`
   - Enable `Friendship System`
   - Set Species Name: `CustomSpecies`
   - Set Wander Radius: `10.0`
   - Set Wander Speed: `3.0`
   - Set Pause Duration: `1.5`

4. **Apply configuration:**
   - Click `Apply Configuration` button

5. **Verify custom values applied:**
   - Select `TestCustomNPC` in Hierarchy
   - Verify `NPCBehavior` has Detection Range: 7.0
   - Verify `NPCWanderAI` has Wander Radius: 10.0, Wander Speed: 3.0, Pause Duration: 1.5
   - Verify `NPCData` asset created with name "CustomNPC"
   - Verify `FriendshipData` asset created with species "CustomSpecies"

6. **Verify gizmo reflects custom values:**
   - Enable `Show Gizmos in Scene View`
   - Verify wander radius circle is 10.0 units (larger than template NPCs)
   - Verify detection range circle is 7.0 units

### Expected Result

✅ Custom configuration applied without using template
✅ All custom values correctly set on components
✅ ScriptableObjects created with custom names
✅ Gizmos reflect custom values

---

## Test 11.8: Test Asset Overwrite Handling

### Test Case A: Overwrite Existing Asset

1. **Configure an NPC:**
   - Create empty GameObject named `DuplicateTest`
   - Open NPCQuickConfig window
   - Set NPC Name: `DuplicateTest`
   - Select any template (e.g., Cervo-Broto)
   - Click `Apply Configuration`
   - Verify success message

2. **Try to apply again without changing name:**
   - With `DuplicateTest` still selected
   - Click `Apply Configuration` again

3. **Verify dialog appears:**
   - A dialog should appear asking: "NPCData already exists. Overwrite?"
   - Options should be: "Yes" / "No" / "Create New"

4. **Test "Overwrite" option:**
   - Click "Yes" to overwrite
   - Verify the existing asset is replaced
   - Verify no new asset is created
   - Verify success message

### Test Case B: Create New Asset with Suffix

1. **Try to apply again:**
   - With `DuplicateTest` still selected
   - Click `Apply Configuration` again

2. **Choose "Create New":**
   - When dialog appears, click "Create New"

3. **Verify new asset created:**
   - Navigate to `Assets/Data/NPCs/`
   - Verify `DuplicateTestData_1.asset` exists
   - Verify original `DuplicateTestData.asset` still exists
   - Verify the GameObject now references `DuplicateTestData_1`

4. **Test multiple suffixes:**
   - Apply configuration again and choose "Create New"
   - Verify `DuplicateTestData_2.asset` is created
   - Continue to verify suffix increments correctly

### Test Case C: Cancel Operation

1. **Try to apply again:**
   - With `DuplicateTest` still selected
   - Click `Apply Configuration` again

2. **Choose "Cancel":**
   - When dialog appears, click "No" or "Cancel"

3. **Verify operation cancelled:**
   - No new assets created
   - No changes made to GameObject
   - No error messages displayed

### Expected Result

✅ Dialog appears when asset already exists
✅ "Overwrite" option replaces existing asset
✅ "Create New" option creates asset with "_1", "_2", etc. suffix
✅ "Cancel" option aborts operation without changes

---

## Summary Checklist

After completing all tests, verify:

- [ ] Test scene created with 3 test GameObjects
- [ ] Cervo-Broto template configuration works correctly
- [ ] Esquilo Coletor template configuration works correctly
- [ ] Abelha Cristalina template configuration works correctly
- [ ] Validation errors display correctly
- [ ] All gizmos draw correctly in Scene View
- [ ] Custom configuration works without templates
- [ ] Asset overwrite handling works correctly

---

## Troubleshooting

### Common Issues

**Issue: NPCQuickConfig menu item not appearing**

- Solution: Check for compilation errors in Console. Fix any errors and wait for recompilation.

**Issue: Template dropdown is empty**

- Solution: Verify template assets exist in `Assets/Data/QuickWins/Templates/NPCTemplates/`

**Issue: Components not being added**

- Solution: Check Console for error messages. Verify GameObject is not a prefab (unpack if needed).

**Issue: Gizmos not drawing**

- Solution: Ensure "Show Gizmos in Scene View" is checked. Verify Scene View has gizmos enabled (button in Scene View toolbar).

**Issue: ScriptableObjects not created**

- Solution: Check Console for file system errors. Verify directories exist and are writable.

---

## Reporting Issues

If you encounter any issues during testing:

1. Note the exact steps to reproduce
2. Capture screenshots of error messages
3. Check Unity Console for error logs
4. Document expected vs actual behavior
5. Report to development team with all details

---

## Next Steps

After completing all manual tests:

1. Document any bugs or issues found
2. Create bug reports for critical issues
3. Verify all requirements are met
4. Mark task 11 as complete in tasks.md
5. Proceed with any necessary bug fixes or improvements
