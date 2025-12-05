# Design Document - Inventory UI Display System

## Overview

O sistema de exibição do inventário é responsável por sincronizar visualmente o estado do `InventoryManager` com a interface do usuário. O sistema utiliza uma arquitetura baseada em eventos para garantir que a UI sempre reflita o estado atual do inventário, exibindo itens em uma grade de 12 slots (3 linhas x 4 colunas) onde cada item ocupa exatamente 1 slot.

A implementação segue o princípio KISS, reutilizando componentes existentes e evitando complexidade desnecessária. O sistema é composto por três componentes principais que já existem no projeto: `InventoryManager` (gerenciamento de estado), `InventoryUI` (controle de exibição) e `InventorySlotUI` (representação visual de slots individuais).

## Architecture

### Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      InventoryManager                        │
│                        (Singleton)                           │
│  - Gerencia 12 slots de inventário                          │
│  - Dispara eventos OnInventoryChanged                       │
│  - Adiciona/Remove itens (não empilháveis)                  │
└────────────────────┬────────────────────────────────────────┘
                     │ Events (OnInventoryChanged)
                     ↓
┌─────────────────────────────────────────────────────────────┐
│                        InventoryUI                           │
│  - Escuta eventos do InventoryManager                       │
│  - Cria e gerencia 12 InventorySlotUI                       │
│  - Sincroniza UI com estado do inventário                   │
└────────────────────┬────────────────────────────────────────┘
                     │ Manages
                     ↓
┌─────────────────────────────────────────────────────────────┐
│                     InventorySlotUI (x12)                    │
│  - Exibe ícone do item                                      │
│  - Mostra/oculta baseado no estado do slot                  │
│  - Atualiza quando notificado                               │
└─────────────────────────────────────────────────────────────┘
```

### Event Flow

```
Player Collects Item
        ↓
PickupItem.TryInteract()
        ↓
InventoryManager.AddItem()
        ↓
OnInventoryChanged Event
        ↓
InventoryUI.RefreshAllSlots()
        ↓
InventorySlotUI.Refresh() (x12)
        ↓
UI Updated
```

## Components and Interfaces

### InventoryManager (Existing - Requires Modification)

**Responsabilidades:**
- Gerenciar array de 12 slots (atualmente 20)
- Adicionar itens sem empilhamento
- Disparar eventos de mudança
- Fornecer acesso aos slots

**Modificações Necessárias:**
```csharp
// Alterar de 20 para 12 slots
private InventorySlot[] slots = new InventorySlot[12];

// Modificar AddItem para não empilhar
public bool AddItem(ItemData item, int quantity = 1)
{
    // Para cada unidade, adicionar em slot separado
    for (int i = 0; i < quantity; i++)
    {
        int emptySlot = FindEmptySlot();
        if (emptySlot == -1)
        {
            OnInventoryFull?.Invoke();
            return false;
        }
        
        slots[emptySlot].item = item;
        slots[emptySlot].quantity = 1; // Sempre 1
        OnInventoryChanged?.Invoke();
    }
    return true;
}
```

### InventoryUI (Existing - Requires Enhancement)

**Responsabilidades:**
- Criar 12 InventorySlotUI na inicialização
- Escutar OnInventoryChanged do InventoryManager
- Atualizar todos os slots quando necessário
- Gerenciar exibição/ocultação do painel

**Novos Métodos:**
```csharp
private InventorySlotUI[] slotUIComponents = new InventorySlotUI[12];

private void Start()
{
    InitializeSlots();
    SubscribeToEvents();
}

private void InitializeSlots()
{
    // Criar ou obter referências aos 12 slots
    // Configurar cada slot com seu índice
}

private void SubscribeToEvents()
{
    if (InventoryManager.Instance != null)
    {
        InventoryManager.Instance.OnInventoryChanged += RefreshAllSlots;
    }
}

private void RefreshAllSlots()
{
    for (int i = 0; i < slotUIComponents.Length; i++)
    {
        InventorySlot slot = InventoryManager.Instance.GetSlot(i);
        slotUIComponents[i].Setup(slot, i);
    }
}

public override void Show()
{
    base.Show();
    RefreshAllSlots(); // Sincroniza ao abrir
}
```

### InventorySlotUI (Existing - Minor Enhancement)

**Responsabilidades:**
- Exibir ícone do item quando slot não está vazio
- Ocultar ícone quando slot está vazio
- Atualizar visualização quando Setup() é chamado

**Comportamento Atual (Mantém):**
```csharp
public void Setup(InventorySlot slot, int index)
{
    currentSlot = slot;
    slotIndex = index;
    Refresh();
}

public void Refresh()
{
    if (currentSlot == null || currentSlot.IsEmpty)
    {
        iconImage.enabled = false;
        quantityText.text = "";
    }
    else
    {
        iconImage.enabled = true;
        iconImage.sprite = currentSlot.item.icon;
        quantityText.text = ""; // Sempre vazio pois não empilha
    }
}
```

## Data Models

### InventorySlot (Existing - No Changes)

```csharp
[System.Serializable]
public class InventorySlot
{
    public ItemData item;      // Item no slot (null se vazio)
    public int quantity;       // Sempre 1 para itens não empilháveis
    
    public bool IsEmpty => item == null;
    
    // CanStack não será usado neste sistema
    public bool CanStack(ItemData newItem) => false;
}
```

### ItemData (Existing - No Changes)

```csharp
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;        // Usado para exibição no slot
    public ItemType type;
    public bool isStackable;   // Será ignorado (tratado como false)
    // ... outras propriedades
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Slot display reflects inventory state

*For any* inventory state, when the UI is refreshed, each slot's visual representation (icon enabled/disabled and sprite) should match the corresponding InventorySlot state in the InventoryManager.

**Validates: Requirements 1.2, 1.3, 1.4**

### Property 2: Non-stacking behavior

*For any* sequence of item additions, items of the same type should always occupy different slots, with each slot containing exactly 1 item (quantity = 1).

**Validates: Requirements 2.2, 2.5**

### Property 3: First empty slot allocation

*For any* inventory state with at least one empty slot, when a new item is added, it should be placed in the slot with the lowest index that is currently empty.

**Validates: Requirements 2.1**

### Property 4: Slot liberation on removal

*For any* inventory state where a slot contains an item, when that item is removed, the slot should become empty (item = null) and available for new items.

**Validates: Requirements 2.4**

### Property 5: Event-driven synchronization

*For any* change to the inventory state that triggers OnInventoryChanged, all 12 slot UI components should be refreshed to reflect the new state.

**Validates: Requirements 1.5, 3.4**

### Property 6: UI state consistency on open

*For any* inventory state, when the inventory UI is opened (Show() is called), the displayed slots should immediately reflect the current state of all 12 slots in the InventoryManager.

**Validates: Requirements 1.4, 3.2**

## Error Handling

### Inventory Full Scenario

**Situação:** Jogador tenta coletar item quando todos os 12 slots estão ocupados.

**Tratamento:**
1. `InventoryManager.AddItem()` retorna `false`
2. Evento `OnInventoryFull` é disparado
3. `PickupItem` não destrói o item
4. Log de aviso é exibido (se logs habilitados)

```csharp
if (!InventoryManager.Instance.AddItem(itemData, 1))
{
    LogWarning("Inventário cheio. Não foi possível coletar item.");
    // Item permanece no mundo
    return false;
}
```

### Missing InventoryManager

**Situação:** InventoryUI tenta acessar InventoryManager.Instance que é null.

**Tratamento:**
1. Verificação null antes de acessar
2. Log de erro
3. Desabilita funcionalidade de atualização

```csharp
private void SubscribeToEvents()
{
    if (InventoryManager.Instance == null)
    {
        UnityEngine.Debug.LogError("[InventoryUI] InventoryManager.Instance não encontrado!");
        return;
    }
    InventoryManager.Instance.OnInventoryChanged += RefreshAllSlots;
}
```

### Invalid Slot Index

**Situação:** Tentativa de acessar slot com índice fora do range [0-11].

**Tratamento:**
1. Validação de índice
2. Log de warning
3. Retorna null ou ignora operação

```csharp
public InventorySlot GetSlot(int index)
{
    if (index < 0 || index >= 12)
    {
        UnityEngine.Debug.LogWarning($"[InventoryManager] Índice inválido: {index}");
        return null;
    }
    return slots[index];
}
```

### Missing Icon Sprite

**Situação:** ItemData não tem sprite de ícone configurado.

**Tratamento:**
1. Verificação null no InventorySlotUI
2. Usa sprite padrão ou desabilita imagem
3. Log de warning

```csharp
if (currentSlot.item.icon == null)
{
    UnityEngine.Debug.LogWarning($"[InventorySlotUI] Item {currentSlot.item.itemName} sem ícone!");
    iconImage.enabled = false;
}
else
{
    iconImage.sprite = currentSlot.item.icon;
}
```

## Testing Strategy

### Unit Testing

**Foco:** Testar componentes individuais isoladamente.

**Testes Recomendados:**
1. **InventoryManager.AddItem()** - Verifica que itens são adicionados ao primeiro slot vazio
2. **InventoryManager.RemoveItem()** - Verifica que slots são liberados corretamente
3. **InventorySlot.IsEmpty** - Verifica lógica de slot vazio
4. **InventorySlotUI.Refresh()** - Verifica que UI é atualizada baseado no estado do slot

**Exemplo:**
```csharp
[Test]
public void AddItem_WithEmptyInventory_AddsToFirstSlot()
{
    // Arrange
    var manager = CreateInventoryManager();
    var item = CreateTestItem();
    
    // Act
    bool result = manager.AddItem(item, 1);
    
    // Assert
    Assert.IsTrue(result);
    Assert.AreEqual(item, manager.GetSlot(0).item);
    Assert.AreEqual(1, manager.GetSlot(0).quantity);
}
```

### Property-Based Testing

**Framework:** Unity Test Framework com geração de dados aleatórios.

**Configuração:** Mínimo de 100 iterações por propriedade.

**Propriedades a Testar:**

1. **Property 1: Slot display reflects inventory state**
   - Gerar estado aleatório do inventário
   - Criar UI e chamar RefreshAllSlots()
   - Verificar que cada slot UI reflete o estado correto

2. **Property 2: Non-stacking behavior**
   - Gerar sequência aleatória de adições do mesmo item
   - Verificar que cada item ocupa um slot diferente
   - Verificar que quantity é sempre 1

3. **Property 3: First empty slot allocation**
   - Gerar inventário parcialmente cheio aleatório
   - Adicionar novo item
   - Verificar que foi para o primeiro slot vazio

4. **Property 4: Slot liberation on removal**
   - Gerar inventário aleatório
   - Remover item de slot aleatório
   - Verificar que slot ficou vazio

5. **Property 5: Event-driven synchronization**
   - Gerar mudança aleatória no inventário
   - Verificar que evento foi disparado
   - Verificar que UI foi atualizada

6. **Property 6: UI state consistency on open**
   - Gerar estado aleatório do inventário
   - Abrir UI
   - Verificar que todos os slots refletem o estado

**Exemplo de Teste de Propriedade:**
```csharp
[Test]
public void Property_NonStackingBehavior_AlwaysUsesSeperateSlots()
{
    for (int iteration = 0; iteration < 100; iteration++)
    {
        // Arrange
        var manager = CreateInventoryManager();
        var item = CreateTestItem();
        int itemsToAdd = Random.Range(2, 12);
        
        // Act
        for (int i = 0; i < itemsToAdd; i++)
        {
            manager.AddItem(item, 1);
        }
        
        // Assert
        int occupiedSlots = 0;
        for (int i = 0; i < 12; i++)
        {
            var slot = manager.GetSlot(i);
            if (!slot.IsEmpty)
            {
                Assert.AreEqual(item, slot.item);
                Assert.AreEqual(1, slot.quantity);
                occupiedSlots++;
            }
        }
        Assert.AreEqual(itemsToAdd, occupiedSlots);
    }
}
```

### Integration Testing

**Foco:** Testar fluxo completo de coleta até exibição.

**Cenários:**
1. Coletar item → Verificar que aparece na UI
2. Coletar 12 itens → Verificar que inventário fica cheio
3. Remover item → Verificar que slot fica vazio na UI
4. Abrir inventário → Verificar sincronização imediata

### Edge Cases

**Casos Importantes:**
1. Inventário completamente vazio (0/12)
2. Inventário completamente cheio (12/12)
3. Coletar item quando inventário está cheio
4. Remover último item do inventário
5. Adicionar múltiplos itens em sequência rápida

## Implementation Notes

### Performance Considerations

1. **Event Subscription:** Sempre fazer unsubscribe no OnDisable para evitar memory leaks
2. **Refresh Optimization:** RefreshAllSlots() atualiza todos os 12 slots - aceitável para inventário pequeno
3. **Sprite Loading:** Ícones devem estar pré-carregados no ItemData (ScriptableObject)

### Unity Editor Setup

1. **InventoryUI Prefab:** Deve conter 12 InventorySlotUI como filhos
2. **Grid Layout Group:** Configurar para 3 linhas x 4 colunas
3. **Slot Prefab:** Deve ter Image (ícone) e TextMeshProUGUI (quantidade - não usado)

### Logging Strategy

Seguir padrão definido em BoasPraticas.md:

```csharp
[Header("Debug")]
[SerializeField] private bool enableLogs = false;

private void Log(string message)
{
    if (enableLogs)
        UnityEngine.Debug.Log($"[InventoryUI] {message}");
}
```

**Logs Importantes:**
- Inicialização dos slots
- Eventos de sincronização
- Erros de referência null
- Inventário cheio

### Migration from Current System

**Passos:**
1. Alterar `InventoryManager.slots` de 20 para 12
2. Modificar `AddItem()` para não empilhar
3. Adicionar `InitializeSlots()` e `RefreshAllSlots()` no `InventoryUI`
4. Conectar eventos no `Start()` do `InventoryUI`
5. Testar fluxo completo de coleta

**Compatibilidade:**
- Sistema de save/load precisará ser atualizado para 12 slots
- Equipamentos e quick slots não são afetados
