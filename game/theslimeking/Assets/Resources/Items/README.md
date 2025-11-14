# Items Folder

This folder contains ItemData ScriptableObjects for the inventory system.

## Creating Example Items

To create the 5 example items automatically:

1. Open Unity Editor
2. Go to menu: **Tools > The Slime King > Create Example Items**
3. The following items will be created:
   - **Poção de Cura** (Consumable) - Heals 30 HP
   - **Cristal Mágico** (Material) - Stackable crafting material
   - **Chave Antiga** (Quest Item) - Cannot be discarded
   - **Amuleto de Proteção** (Equipment - Amulet) - +15 Defense
   - **Anel de Velocidade** (Equipment - Ring) - +2 Speed

## Adding Icons

After creating the items, you can assign icons to them:

1. Select an item in the Project window
2. In the Inspector, drag a sprite to the **Icon** field
3. Icons should be placed in `Assets/Art/UI/Icons/` or similar

## Creating New Items

To create a new item manually:

1. Right-click in this folder
2. Select **Create > The Slime King > Item**
3. Configure the item properties in the Inspector:
   - **Item Name**: Display name
   - **Icon**: Visual representation
   - **Type**: Consumable, Material, Quest, or Equipment
   - **Is Stackable**: Can multiple items stack in one slot?
   - **Heal Amount**: HP restored (for consumables)
   - **Equipment Type**: Amulet, Ring, or Cape (for equipment)
   - **Defense Bonus**: Defense increase (for equipment)
   - **Speed Bonus**: Speed increase (for equipment)
   - **Is Quest Item**: Cannot be discarded if true

## Item Types

### Consumable

Items that can be used to restore health. They are removed after use.

### Material

Stackable items used for crafting (future feature).

### Quest

Special items required for quests. Cannot be discarded.

### Equipment

Items that can be equipped to provide passive bonuses:

- **Amulet**: Slot 0
- **Ring**: Slot 1
- **Cape**: Slot 2

## Loading Items in Code

Items are loaded from this folder using Resources.Load:

```csharp
ItemData item = Resources.Load<ItemData>("Items/PocaoDeCura");
```

Make sure the file name matches exactly (case-sensitive).
