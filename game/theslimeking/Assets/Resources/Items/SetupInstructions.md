# Setup Instructions for Example Items

## Quick Setup (Recommended)

The easiest way to create the 5 example items is to use the automated editor script:

### Steps

1. **Open Unity Editor** (if not already open)
2. Go to the menu: **Tools > The Slime King > Create Example Items**
3. Click **OK** in the confirmation dialog
4. The following 5 items will be created automatically:
   - `PocaoDeCura.asset` - Poção de Cura (Consumable, heals 30 HP)
   - `CristalMagico.asset` - Cristal Mágico (Material, stackable)
   - `ChaveAntiga.asset` - Chave Antiga (Quest Item, cannot be discarded)
   - `AmuletoDeProtecao.asset` - Amuleto de Proteção (Equipment - Amulet, +15 Defense)
   - `AnelDeVelocidade.asset` - Anel de Velocidade (Equipment - Ring, +2 Speed)

## Adding Icons (Optional)

After creating the items, you can assign icons to make them visually appealing:

1. Select an item in the Project window (e.g., `PocaoDeCura.asset`)
2. In the Inspector panel, find the **Icon** field
3. Drag and drop a sprite from your project (or click the circle icon to browse)
4. Repeat for all items

### Suggested Icon Locations

- Look in `Assets/Art/UI/` or `Assets/Art/Icons/` for existing sprites
- Or create simple placeholder sprites using Unity's built-in shapes

## Manual Creation (Alternative)

If you prefer to create items manually:

1. Right-click in the `Assets/Resources/Items/` folder
2. Select **Create > The Slime King > Item**
3. Name the asset (e.g., `MyNewItem`)
4. Configure properties in the Inspector:
   - **Item Name**: Display name shown in UI
   - **Icon**: Visual sprite
   - **Type**: Consumable, Material, Quest, or Equipment
   - **Is Stackable**: Whether items can stack (up to 99)
   - Configure type-specific properties (heal amount, defense bonus, etc.)

## Verification

To verify the items were created correctly:

1. Check that 5 `.asset` files exist in `Assets/Resources/Items/`
2. Select each item and verify properties in the Inspector
3. Try loading an item in code:

   ```csharp
   ItemData potion = Resources.Load<ItemData>("Items/PocaoDeCura");
   Debug.Log(potion.itemName); // Should print "Poção de Cura"
   ```

## Testing in Game

To test the items in the inventory system:

1. Add an `InventoryManager` to your scene (if not already present)
2. In a test script, add items to the inventory:

   ```csharp
   ItemData potion = Resources.Load<ItemData>("Items/PocaoDeCura");
   InventoryManager.Instance.AddItem(potion, 5);
   ```

3. Open the inventory UI to see the items

## Troubleshooting

**Items not appearing in Resources.Load:**

- Make sure the items are in `Assets/Resources/Items/` folder
- Check that file names match exactly (case-sensitive)
- Refresh the Asset Database: Right-click in Project > Refresh

**Script compilation errors:**

- Make sure `ItemData.cs` is in the correct namespace: `TheSlimeKing.Inventory`
- Verify all enum types exist: `ItemType`, `EquipmentType`

**Editor menu not showing:**

- Make sure `CreateExampleItems.cs` is in an `Editor` folder
- Check for compilation errors in the Console
- Restart Unity Editor if needed
