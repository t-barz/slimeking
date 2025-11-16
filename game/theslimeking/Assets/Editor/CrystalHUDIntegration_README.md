# CrystalHUDController Integration Guide

## Overview

This guide explains how to integrate the `CrystalHUDController` component into the CanvasHUD prefab using the automated Editor tools.

## Automated Integration (Recommended)

### Step 1: Run the Integration Tool

1. Open Unity Editor
2. Go to menu: **SlimeKing > Setup > Integrate CrystalHUDController**
3. Wait for the process to complete
4. A dialog will confirm successful integration

### Step 2: Verify the Integration

1. Go to menu: **SlimeKing > Setup > Verify CrystalHUDController Integration**
2. Check the Console for verification results
3. A dialog will show the status of all references

### What the Integration Tool Does

The automated integration tool performs the following actions:

1. **Loads the CanvasHUD prefab** from `Assets/Game/Prefabs/UI/CanvasHUD.prefab`
2. **Adds CrystalHUDController component** to the root GameObject
3. **Auto-configures all TextMeshProUGUI references**:
   - `natureCountText` → CrystalContainer/Crystal_Nature/Count_Text
   - `fireCountText` → CrystalContainer/Crystal_Fire/Count_Text
   - `waterCountText` → CrystalContainer/Crystal_Water/Count_Text
   - `shadowCountText` → CrystalContainer/Crystal_Shadow/Count_Text
   - `earthCountText` → CrystalContainer/Crystal_Earth/Count_Text
   - `airCountText` → CrystalContainer/Crystal_Air/Count_Text
4. **Saves the prefab** with all changes applied

## Manual Integration (Alternative)

If you prefer to integrate manually or need to troubleshoot:

### Step 1: Add Component to Prefab

1. Open `Assets/Game/Prefabs/UI/CanvasHUD.prefab` in Prefab Mode
2. Select the root `CanvasHUD` GameObject
3. Click **Add Component**
4. Search for `CrystalHUDController`
5. Add the component

### Step 2: Configure References

1. With `CanvasHUD` selected, find the `CrystalHUDController` component in the Inspector
2. Expand the **Crystal UI References** section
3. Manually drag each Count_Text component to the corresponding field:
   - **Nature Count Text**: CrystalContainer/Crystal_Nature/Count_Text
   - **Fire Count Text**: CrystalContainer/Crystal_Fire/Count_Text
   - **Water Count Text**: CrystalContainer/Crystal_Water/Count_Text
   - **Shadow Count Text**: CrystalContainer/Crystal_Shadow/Count_Text
   - **Earth Count Text**: CrystalContainer/Crystal_Earth/Count_Text
   - **Air Count Text**: CrystalContainer/Crystal_Air/Count_Text

### Step 3: Use Auto-Find Helper (Optional)

1. Right-click on the `CrystalHUDController` component header
2. Select **Auto-Find Text References** from the context menu
3. This will automatically find and assign all references

## Testing the Integration

### In Editor (Play Mode)

1. Open the scene `Assets/Game/Scenes/2_InitialCave.unity`
2. Enter Play Mode
3. Check the Console for initialization logs:

   ```
   [CrystalHUDController] Contadores de cristais inicializados
   ```

4. Verify all counters display "x0" initially

### Testing Crystal Collection

1. In Play Mode, use the GameManager to add crystals:

   ```csharp
   GameManager.Instance.AddCrystal(CrystalType.Nature, 5);
   ```

2. Verify the HUD updates to show "x5" for Nature crystals
3. Check the Console for update logs:

   ```
   [CrystalHUDController] Cristal Nature atualizado: 5
   ```

### Using the Test Context Menu

1. In Play Mode, select the CanvasHUD GameObject in the Hierarchy
2. Right-click on the `CrystalHUDController` component
3. Select **Test Update All Counters**
4. This will set random values (0-99) for all crystal types
5. Verify all counters update correctly

## Validation Checklist

After integration, verify the following:

- [ ] CrystalHUDController component exists on CanvasHUD prefab
- [ ] All 6 TextMeshProUGUI references are configured (not null)
- [ ] Initial format shows "x0" for all counters
- [ ] GameManager events trigger HUD updates
- [ ] No errors in Console during initialization
- [ ] Subscribe/Unsubscribe events work correctly (OnEnable/OnDisable)

## Troubleshooting

### "CrystalContainer not found" Error

**Problem**: The integration tool cannot find the CrystalContainer GameObject.

**Solution**:

- Verify the CanvasHUD prefab has a child named "CrystalContainer"
- Check the scene structure matches the expected hierarchy

### "TextComponent is null" Warning

**Problem**: One or more Count_Text references are not configured.

**Solution**:

- Run the verification tool to identify which references are missing
- Manually configure the missing references in the Inspector
- Re-run the integration tool

### "GameManager.Instance is null" Warning

**Problem**: GameManager is not available when CrystalHUDController initializes.

**Solution**:

- Ensure GameManager prefab exists in the scene
- Check that GameManager initializes before CanvasHUD
- Verify GameManager.Instance is properly set up

### Events Not Firing

**Problem**: HUD doesn't update when crystals are collected.

**Solution**:

- Verify GameManager.OnCrystalCountChanged event is being dispatched
- Check that CrystalHUDController is subscribed to the event (OnEnable)
- Enable debug logs in CrystalHUDController settings
- Check Console for event handler logs

## Configuration Options

### Debug Logs

Toggle debug logging in the Inspector:

- **Enable Debug Logs**: Shows initialization and update logs
- Useful for debugging event flow and updates

### Count Format

Customize the counter display format:

- **Count Format**: Default is "x{0}" (e.g., "x10")
- Can be changed to other formats like "{0}", "×{0}", etc.

## Scene Integration

The CanvasHUD prefab is used in the following scenes:

- `2_InitialCave.unity`
- `3_InitialForest.unity`
- Other game scenes

After modifying the prefab, all scenes using it will automatically receive the updates.

## Next Steps

After successful integration:

1. ✅ Task 2 Complete: CrystalHUDController integrated into scene
2. ➡️ Task 3: Modify ItemCollectable for intelligent routing
3. ➡️ Task 4: Validate integration with GameManager
4. ➡️ Task 5: Validate integration with InventoryManager

## Support

If you encounter issues not covered in this guide:

1. Check the Console for detailed error messages
2. Review the CrystalHUDController.cs source code
3. Verify the scene structure matches the expected hierarchy
4. Run the verification tool to identify specific problems
