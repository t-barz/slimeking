# Design Document - Sistema de Inventário

## Overview

Sistema simples de inventário para The Slime King que permite ao jogador gerenciar 20 slots de itens, usar consumíveis, equipar até 3 itens de equipamento e atribuir 4 itens aos atalhos rápidos (direcionais do gamepad). O sistema é acessado via menu de pausa e mantém a filosofia cozy do jogo.

**Princípios:**

- **Simples:** Apenas o essencial, sem complexidade desnecessária
- **Intuitivo:** Interface clara e fácil de usar
- **Integrado:** Funciona harmoniosamente com outros sistemas do jogo

## Architecture

```
GameManager
    ↓
PauseMenu
    ↓
InventoryManager (Singleton)
    ├── InventoryData (20 slots)
    ├── EquipmentData (3 slots)
    └── QuickSlotData (4 slots)
    ↓
InventoryUI
    ├── GridUI (5x4)
    ├── EquipmentUI (3 slots)
    └── QuickSlotHUD (4 direcionais)
```

## Components and Interfaces

### 1. InventoryManager

Gerencia todo o estado do inventário.

```csharp
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    
    private InventorySlot[] slots = new InventorySlot[20];
    private ItemData[] equipment = new ItemData[3]; // Amulet, Ring, Cape
    private ItemData[] quickSlots = new ItemData[4]; // Up, Down, Left, Right
    
    // Core Methods
    public bool AddItem(ItemData item, int quantity = 1);
    public bool RemoveItem(ItemData item, int quantity = 1);
    public void UseItem(int slotIndex);
    public void EquipItem(ItemData item);
    public void DiscardItem(int slotIndex);
    public void AssignQuickSlot(ItemData item, int direction);
    public void UseQuickSlot(int direction);
}
```

### 2. InventorySlot

Estrutura de dados simples para cada slot.

```csharp
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;
    
    public bool IsEmpty => item == null;
    public bool CanStack(ItemData newItem) => 
        item == newItem && quantity < 99 && item.isStackable;
}
```

### 3. ItemData (ScriptableObject)

Define propriedades de cada item.

```csharp
[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public bool isStackable = true;
    
    // Consumable
    public int healAmount;
    public int staminaAmount;
    
    // Equipment
    public EquipmentType equipmentType;
    public int defenseBonus;
    public int speedBonus;
    
    // Quest
    public bool isQuestItem;
}

public enum ItemType { Consumable, Material, Quest, Equipment }
public enum EquipmentType { Amulet, Ring, Cape }
```

### 4. InventoryUI

Controla a interface visual.

```csharp
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private InventorySlotUI[] slotUIs = new InventorySlotUI[20];
    [SerializeField] private EquipmentSlotUI[] equipmentUIs = new EquipmentSlotUI[3];
    
    public void Show();
    public void Hide();
    public void RefreshAll();
    public void OnSlotClicked(int index);
}
```

### 5. InventorySlotUI

Representa visualmente um slot.

```csharp
public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;
    
    private int slotIndex;
    
    public void Setup(InventorySlot slot, int index);
    public void Refresh();
    public void OnClick();
}
```

### 6. QuickSlotManager

Gerencia os 4 atalhos rápidos.

```csharp
public class QuickSlotManager : MonoBehaviour
{
    [SerializeField] private QuickSlotUI[] quickSlotUIs = new QuickSlotUI[4];
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            InventoryManager.Instance.UseQuickSlot(0);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            InventoryManager.Instance.UseQuickSlot(1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            InventoryManager.Instance.UseQuickSlot(2);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            InventoryManager.Instance.UseQuickSlot(3);
    }
    
    public void RefreshUI();
}
```

## Data Models

### Save Data

```csharp
[System.Serializable]
public class InventorySaveData
{
    public ItemSaveData[] items;
    public string[] equipmentIDs = new string[3];
    public string[] quickSlotIDs = new string[4];
}

[System.Serializable]
public class ItemSaveData
{
    public string itemID;
    public int quantity;
    public int slotIndex;
}
```

## Error Handling

### Inventário Cheio

```csharp
public bool AddItem(ItemData item, int quantity = 1)
{
    // Tenta empilhar primeiro
    foreach (var slot in slots)
    {
        if (slot.CanStack(item))
        {
            slot.quantity += quantity;
            return true;
        }
    }
    
    // Procura slot vazio
    for (int i = 0; i < slots.Length; i++)
    {
        if (slots[i].IsEmpty)
        {
            slots[i].item = item;
            slots[i].quantity = quantity;
            return true;
        }
    }
    
    // Inventário cheio
    ShowNotification("Inventário Cheio!");
    return false;
}
```

### Proteção de Quest Items

```csharp
public void DiscardItem(int slotIndex)
{
    var slot = slots[slotIndex];
    
    if (slot.item.isQuestItem)
    {
        ShowNotification("Itens de quest não podem ser descartados");
        return;
    }
    
    ShowConfirmation($"Descartar {slot.item.itemName}?", () => {
        slot.item = null;
        slot.quantity = 0;
        RefreshUI();
    });
}
```

## Testing Strategy

### Testes Essenciais

1. **Adicionar item em inventário vazio**
2. **Empilhar item existente (até 99)**
3. **Adicionar quando inventário está cheio**
4. **Usar consumível (reduz quantidade)**
5. **Equipar item (3 slots)**
6. **Atribuir e usar quick slot**
7. **Descartar item normal (com confirmação)**
8. **Tentar descartar quest item (bloqueado)**

## UI/UX Design

### Layout do Inventário

```
┌────────────────────────────────────┐
│  INVENTÁRIO                   [X]  │
├────────────────────────────────────┤
│                                    │
│  ┌──────────┐  ┌──────────────┐   │
│  │  GRID    │  │ EQUIPAMENTOS │   │
│  │  5x4     │  │              │   │
│  │  ┌─┬─┬─┐ │  │  [Amuleto]   │   │
│  │  │ │ │ │ │  │  [Anel]      │   │
│  │  ├─┼─┼─┤ │  │  [Capa]      │   │
│  │  │ │ │ │ │  │              │   │
│  │  └─┴─┴─┘ │  └──────────────┘   │
│  └──────────┘                      │
│                                    │
│  Slots: 12/20                      │
└────────────────────────────────────┘
```

### Quick Slots no HUD

```
Canto inferior direito:
┌──┬──┐
│↑ │↓ │
├──┼──┤
│← │→ │
└──┴──┘
```

### Painel de Ações

Ao clicar em item:

```
┌──────────────┐
│ Poção de Cura│
├──────────────┤
│ [  Usar  ]   │
│ [ Atribuir ] │
│ [ Descartar ]│
└──────────────┘
```

## Performance

### Otimizações

1. **Object Pooling:** Reutilizar 20 InventorySlotUI
2. **Event-Driven:** UI só atualiza quando inventário muda
3. **Sprite Atlas:** Todos ícones em um atlas
4. **Cache:** Referências de componentes em Awake()

## Integration Points

### Com Save System

```csharp
public void SaveInventory()
{
    var saveData = new InventorySaveData();
    
    // Salvar itens
    List<ItemSaveData> itemList = new List<ItemSaveData>();
    for (int i = 0; i < slots.Length; i++)
    {
        if (!slots[i].IsEmpty)
        {
            itemList.Add(new ItemSaveData {
                itemID = slots[i].item.name,
                quantity = slots[i].quantity,
                slotIndex = i
            });
        }
    }
    saveData.items = itemList.ToArray();
    
    // Salvar equipamentos
    for (int i = 0; i < 3; i++)
        saveData.equipmentIDs[i] = equipment[i]?.name;
    
    // Salvar quick slots
    for (int i = 0; i < 4; i++)
        saveData.quickSlotIDs[i] = quickSlots[i]?.name;
    
    SaveManager.Instance.SaveInventory(saveData);
}
```

### Com Item Collection

```csharp
// Em ItemCollectable.cs
void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        if (InventoryManager.Instance.AddItem(itemData, quantity))
        {
            Destroy(gameObject);
        }
    }
}
```

### Com Player Stats

```csharp
// Aplicar buffs de equipamentos
void ApplyEquipmentStats()
{
    int totalDefense = 0;
    int totalSpeed = 0;
    
    foreach (var item in equipment)
    {
        if (item != null)
        {
            totalDefense += item.defenseBonus;
            totalSpeed += item.speedBonus;
        }
    }
    
    PlayerController.Instance.SetDefense(totalDefense);
    PlayerController.Instance.SetSpeedBonus(totalSpeed);
}
```

## Audio Design

| Ação | SFX |
|------|-----|
| Abrir Inventário | `inventory_open.wav` |
| Fechar Inventário | `inventory_close.wav` |
| Selecionar Item | `item_select.wav` |
| Usar Consumível | `item_use.wav` |
| Equipar Item | `item_equip.wav` |
| Descartar Item | `item_discard.wav` |
| Inventário Cheio | `error.wav` |
| Atribuir Quick Slot | `quickslot_assign.wav` |

## Accessibility

### Controles

- **Keyboard:** Tab (abrir), WASD (navegar), Enter (selecionar), Esc (fechar)
- **Gamepad:** Menu (abrir), D-Pad (navegar), A (selecionar), B (fechar)
- **Mouse:** Click para selecionar

### Feedback Visual

- Slot selecionado: borda amarela
- Hover: highlight sutil
- Ação confirmada: animação rápida

## Future Enhancements

Fora do escopo atual, mas possíveis no futuro:

1. Expansão para 40 slots
2. Tooltips detalhados
3. Filtros por categoria
4. Sistema de favoritos
5. Divisão de pilhas
6. Auto-sort
