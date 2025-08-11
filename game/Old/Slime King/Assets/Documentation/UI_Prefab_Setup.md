# UI Prefab Setup Guide

## Creating the Inventory UI Prefab

### 1. Basic Setup
1. Create a new Canvas in the hierarchy
   - Right-click in Hierarchy > UI > Canvas
   - Set Canvas Scaler to "Scale with Screen Size"
   - Reference resolution: 1920x1080

2. Create the Inventory Panel
   - Create an empty GameObject as a child of the Canvas
   - Name it "InventoryPanel"
   - Add a RectTransform component
   - Add an InventoryUI component

### 2. Slot Container Setup
1. Create a Grid Layout Group
   - Add a child GameObject to InventoryPanel named "SlotContainer"
   - Add Grid Layout Group component
   - Settings:
     - Cell Size: 100x100
     - Spacing: 10x10
     - Constraint: Fixed Column Count
     - Column Count: 3

2. Configure InventorySlotUI Prefab
   - Create a new UI GameObject named "InventorySlotUI"
   - Components needed:
     - Image (for background)
     - Button (for interaction)
     - InventorySlotUI script
   - Child Objects:
     - ItemIcon (Image)
     - StackCount (TextMeshProUGUI)

### 3. References Setup
1. In InventoryUI component:
   - Assign the SlotContainer reference
   - Assign the InventorySlotUI prefab
   - Link to the Inventory component

### 4. Optional UI Elements
1. Add a Title Text
   - Add TextMeshProUGUI component
   - Set font, size, and color as needed

2. Add Close Button
   - Add Button component
   - Add close functionality in your UI manager

## Example Layout Structure
```
Canvas
├── InventoryPanel
│   ├── Title
│   ├── CloseButton
│   └── SlotContainer
│       ├── Slot1
│       ├── Slot2
│       └── Slot3 (etc.)
```

## Recommended Settings

### Canvas
- Render Mode: Screen Space - Overlay
- Pixel Perfect: True
- Sort Order: 100 (adjust as needed)

### InventoryPanel
- Anchor: Center
- Size: 800x600 (adjust as needed)
- Image component with semi-transparent background

### SlotContainer
- Padding: 10,10,10,10
- Start Corner: Upper Left
- Start Axis: Horizontal
- Child Alignment: Upper Left

### InventorySlotUI
- Size: 100x100
- Normal Color: White with 0.8 alpha
- Highlighted Color: White
- Pressed Color: Light Gray
