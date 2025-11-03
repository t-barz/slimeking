# Inventory UI Setup Guide

## Overview

This guide explains how to set up the Inventory UI in Unity after implementing task 4.

## Components Created

### 1. InventorySlotUI.cs

- Represents a single inventory slot
- Displays item icon and quantity
- Handles click events

### 2. EquipmentSlotUI.cs

- Represents equipment slots (Amulet, Ring, Cape)
- Displays equipped item icon
- Handles unequip on click

### 3. InventoryUI.cs

- Main inventory panel controller
- Manages 20 inventory slots
- Manages 3 equipment slots
- Handles Show/Hide and pause/unpause

### 4. ItemActionPanel.cs

- Popup panel for item actions
- Shows Use/Equip, Assign, Discard buttons
- Adapts buttons based on item type

### 5. QuickSlotSelectionPanel.cs

- Panel to select which quick slot to assign an item
- Shows 4 directional buttons (Up, Down, Left, Right)

### 6. ConfirmationDialog.cs

- Generic confirmation dialog
- Used for discard confirmation

## Unity Setup Instructions

### Step 1: Create Inventory Slot Prefab

1. Create a new GameObject in the scene
2. Add `Image` component (for background)
3. Add child GameObject with `Image` component (for icon) - name it "Icon"
4. Add child GameObject with `TextMeshProUGUI` component (for quantity) - name it "Quantity"
5. Add `Button` component to root GameObject
6. Add `InventorySlotUI` component to root GameObject
7. Assign references:
   - Icon Image → Icon GameObject's Image
   - Quantity Text → Quantity GameObject's TextMeshProUGUI
   - Button → Root GameObject's Button
8. Save as prefab: `Assets/Prefabs/UI/InventorySlot.prefab`

### Step 2: Create Equipment Slot Prefab

1. Create a new GameObject
2. Add `Image` component (for background)
3. Add child GameObject with `Image` component (for icon) - name it "Icon"
4. Add child GameObject with `TextMeshProUGUI` component (for label) - name it "Label"
5. Add `Button` component to root GameObject
6. Add `EquipmentSlotUI` component to root GameObject
7. Assign references and set Equipment Type (Amulet, Ring, or Cape)
8. Save as prefab: `Assets/Prefabs/UI/EquipmentSlot.prefab`

### Step 3: Create Main Inventory Panel

1. Create Canvas if not exists
2. Create Panel GameObject under Canvas - name it "InventoryPanel"
3. Add child Panel for slots - name it "SlotsPanel"
4. Add `GridLayoutGroup` to SlotsPanel:
   - Constraint: Fixed Column Count = 5
   - Cell Size: 80x80 (adjust as needed)
   - Spacing: 10x10
5. Add child Panel for equipment - name it "EquipmentPanel"
6. Add 3 equipment slot instances to EquipmentPanel
7. Add Button for close - name it "CloseButton"
8. Add `InventoryUI` component to InventoryPanel
9. Assign references:
   - Inventory Panel → InventoryPanel GameObject
   - Slots Container → SlotsPanel GameObject
   - Slot Prefab → InventorySlot prefab
   - Amulet/Ring/Cape Slots → respective EquipmentSlotUI instances
   - Close Button → CloseButton

### Step 4: Create Action Panel

1. Create Panel GameObject under Canvas - name it "ItemActionPanel"
2. Add TextMeshProUGUI for item name - name it "ItemName"
3. Add 4 Buttons:
   - "UseButton" with TextMeshProUGUI child for text
   - "AssignButton" with text "Atribuir"
   - "DiscardButton" with text "Descartar"
   - "CancelButton" with text "Cancelar"
4. Add `ItemActionPanel` component
5. Assign all references
6. Set panel inactive by default

### Step 5: Create Quick Slot Selection Panel

1. Create Panel GameObject under Canvas - name it "QuickSlotSelectionPanel"
2. Add TextMeshProUGUI for title - name it "Title"
3. Add 4 Buttons for directions:
   - "UpButton" with text "↑"
   - "DownButton" with text "↓"
   - "LeftButton" with text "←"
   - "RightButton" with text "→"
4. Add "CancelButton" with text "Cancelar"
5. Add `QuickSlotSelectionPanel` component
6. Assign all references
7. Set panel inactive by default

### Step 6: Create Confirmation Dialog

1. Create Panel GameObject under Canvas - name it "ConfirmationDialog"
2. Add TextMeshProUGUI for message - name it "Message"
3. Add 2 Buttons:
   - "YesButton" with text "Sim"
   - "NoButton" with text "Não"
4. Add `ConfirmationDialog` component
5. Assign all references
6. Set panel inactive by default

### Step 7: Link to InventoryUI

1. Select InventoryPanel GameObject
2. In InventoryUI component, assign:
   - Action Panel → ItemActionPanel GameObject

## Layout Recommendations

### Inventory Panel Layout

```
┌─────────────────────────────────────┐
│  INVENTÁRIO                    [X]  │
├─────────────────────────────────────┤
│                                     │
│  ┌──────────────┐  ┌────────────┐  │
│  │  GRID 5x4    │  │ EQUIPMENT  │  │
│  │  [20 slots]  │  │            │  │
│  │              │  │ [Amuleto]  │  │
│  │              │  │ [Anel]     │  │
│  │              │  │ [Capa]     │  │
│  └──────────────┘  └────────────┘  │
│                                     │
└─────────────────────────────────────┘
```

### Slot Layout

- Background: Semi-transparent dark color
- Icon: Centered, 64x64
- Quantity: Bottom-right corner, small font

### Step 8: Create Quick Slot HUD (Task 5)

1. Create Panel GameObject under Canvas - name it "QuickSlotHUD"
2. Position in bottom-right corner of screen
3. Add `GridLayoutGroup` component:
   - Constraint: Fixed Column Count = 2
   - Cell Size: 60x60 (adjust as needed)
   - Spacing: 5x5
4. Create 4 QuickSlotUI instances:
   - Create GameObject with `Image` component (background)
   - Add child GameObject with `Image` component (icon) - name it "Icon"
   - Add child GameObject with `TextMeshProUGUI` component (quantity) - name it "Quantity"
   - Add `QuickSlotUI` component
   - Assign references (Icon Image, Quantity Text)
   - Set Slot Direction (0=Up, 1=Down, 2=Left, 3=Right)
   - Repeat for all 4 directions
5. Arrange in grid 2x2 order: Up(0), Down(1), Left(2), Right(3)
6. Create QuickSlotManager GameObject in scene
7. Add `QuickSlotManager` component
8. Assign all 4 QuickSlotUI references to the array
9. Keep QuickSlotHUD always active (visible during gameplay)

### Quick Slot HUD Layout

```
Canto inferior direito:
┌────┬────┐
│ ↑  │ ↓  │  (Up=0, Down=1)
├────┼────┤
│ ←  │ →  │  (Left=2, Right=3)
└────┴────┘
```

## Testing

1. Create an InventoryManager GameObject in the scene
2. Add InventoryManager component
3. Create some test ItemData ScriptableObjects
4. In Play mode, call `InventoryUI.Show()` to test
5. Add test items to inventory to verify display
6. Assign items to quick slots and verify HUD updates
7. Press arrow keys to use quick slot items

## Notes

- All panels start inactive except the main Canvas and QuickSlotHUD
- Time.timeScale is set to 0 when inventory is open (pauses game)
- InventoryManager must exist in scene as singleton
- TextMeshPro package must be installed
- QuickSlotHUD should always be visible during gameplay
- QuickSlotManager automatically subscribes to inventory events to update UI
