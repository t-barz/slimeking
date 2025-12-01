# Design Document - Sistema de Inventário

## Overview

O Sistema de Inventário do The Slime King é um sistema modular e escalável que gerencia coleta, armazenamento, equipamento e uso de itens consumíveis. O design prioriza simplicidade de uso, feedback visual claro e integração perfeita com outros sistemas do jogo (evolução, save/load, input).

**Princípios de Design:**
- **Modularidade**: Componentes independentes e reutilizáveis
- **Escalabilidade**: Fácil adicionar novos tipos de itens
- **Performance**: Operações O(1) para ações frequentes
- **Feedback Visual**: Animações e sons para todas as ações
- **Integração**: Comunicação via eventos com outros sistemas

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     InventoryManager                         │
│                      (Singleton)                             │
│  - Gerencia estado global do inventário                     │
│  - Coordena comunicação entre componentes                   │
│  - Dispara eventos de mudança de estado                     │
└──────────────┬──────────────────────────────────────────────┘
               │
       ┌───────┴───────┐
       │               │
┌──────▼──────┐  ┌────▼─────────┐
│InventoryData│  │InventoryUI   │
│(ScriptableO)│  │(MonoBehaviour)│
│- Capacidade │  │- Renderização│
│- Slots      │  │- Navegação   │
│- Equipados  │  │- Animações   │
└─────────────┘  └──────────────┘
       │               │
       └───────┬───────┘
               │
    ┌──────────▼──────────┐
    │   InventorySlot     │
    │  (Classe de Dados)  │
    │  - Item             │
    │  - Quantidade       │
    │  - Tipo             │
    └─────────────────────┘
```

### Component Responsibilities

**InventoryManager (Singleton)**
- Ponto central de acesso ao inventário
- Gerencia adição/remoção de itens
- Controla equipamento/desequipamento
- Dispara eventos para UI e outros sistemas
- Integra com SaveManager

**InventoryData (ScriptableObject)**
- Define capacidade por estágio de evolução
- Armazena configurações de UI
- Referências a sprites e sons

**InventoryUI (MonoBehaviour)**
- Renderiza interface do inventário
- Gerencia navegação e seleção
- Exibe menus de confirmação
- Atualiza HUD de slots equipados

**InventorySlot (Data Class)**
- Representa um slot individual
- Armazena item e quantidade
- Métodos para empilhamento

**ItemData (ScriptableObject)**
- Define propriedades de cada item
- Ícone, nome, descrição
- Efeitos ao usar
- Stack máximo

## Components and Interfaces

### 1. InventoryManager

```csharp
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    
    // Events
    public static event Action<ItemData, int> OnItemAdded;
    public static event Action<ItemData, int> OnItemRemoved;
    public static event Action<ItemData, int> OnItemUsed;
    public static event Action<int, ItemData> OnItemEquipped;
    public static event Action<int> OnItemUnequipped;
    public static event Action OnInventoryChanged;
    
    // Properties
    public int MaxSlots { get; private set; }
    public List<InventorySlot> Slots { get; private set; }
    public InventorySlot[] EquippedSlots { get; private set; } // 4 slots
    
    // Public Methods
    public bool AddItem(ItemData item, int quantity = 1);
    public bool RemoveItem(ItemData item, int quantity = 1);
    public bool EquipItem(int inventorySlotIndex, int equippedSlotIndex);
    public bool UnequipItem(int equippedSlotIndex);
    public bool UseEquippedItem(int equippedSlotIndex);
    public void DiscardItem(int slotIndex);
    public void UpdateCapacity(EvolutionStage stage);
    
    // Save/Load
    public InventorySaveData GetSaveData();
    public void LoadSaveData(InventorySaveData data);
}
```

### 2. InventoryUI

```csharp
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform equippedSlotsContainer;
    [SerializeField] private GameObject confirmationMenu;
    
    [Header("HUD")]
    [SerializeField] private Transform hudEquippedSlots;
    
    [Header("Navigation")]
    private int currentSlotIndex = 0;
    private bool isNavigatingEquipped = false;
    
    // Public Methods
    public void OpenInventory();  // Pausa o jogo (Time.timeScale = 0)
    public void CloseInventory(); // Retoma o jogo (Time.timeScale = 1)
    public void NavigateSlots(Vector2 direction);
    public void SelectCurrentSlot();
    public void ShowConfirmationMenu(InventorySlot slot);
    public void RefreshUI();
    public void UpdateHUD();
}
```

### 3. InventorySlot (Data Class)

```csharp
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;
    public bool IsEmpty => item == null || quantity <= 0;
    public bool IsFull => quantity >= (item?.maxStack ?? 99);
    
    public bool AddToStack(int amount)
    {
        if (IsEmpty || item == null) return false;
        
        int maxStack = item.maxStack;
        int spaceAvailable = maxStack - quantity;
        
        if (spaceAvailable <= 0) return false;
        
        int amountToAdd = Mathf.Min(amount, spaceAvailable);
        quantity += amountToAdd;
        return true;
    }
    
    public bool RemoveFromStack(int amount)
    {
        if (IsEmpty || quantity < amount) return false;
        
        quantity -= amount;
        if (quantity <= 0)
        {
            Clear();
        }
        return true;
    }
    
    public void Clear()
    {
        item = null;
        quantity = 0;
    }
}
```

### 4. ItemData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "ItemData", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemID;
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    
    [Header("Stack Settings")]
    public int maxStack = 99;
    
    [Header("Consumable Settings")]
    public bool isConsumable = true;
    public int healAmount = 0;
    public float buffDuration = 0f;
    public float buffMultiplier = 1f;
    
    [Header("Audio")]
    public AudioClip useSound;
    
    // Method called when item is used
    public virtual void Use()
    {
        if (healAmount > 0)
        {
            // Heal player
            PlayerHealth.Instance?.Heal(healAmount);
        }
        
        if (buffDuration > 0)
        {
            // Apply buff
            PlayerStats.Instance?.ApplyBuff(buffMultiplier, buffDuration);
        }
        
        // Play sound
        if (useSound != null)
        {
            AudioManager.Instance?.PlaySFX(useSound);
        }
    }
}
```

### 5. InventorySaveData

```csharp
[System.Serializable]
public class InventorySaveData
{
    public List<SlotSaveData> slots;
    public SlotSaveData[] equippedSlots; // 4 slots
    public int maxSlots;
    
    [System.Serializable]
    public class SlotSaveData
    {
        public string itemID;
        public int quantity;
    }
}
```

## Data Models

### Inventory Capacity by Evolution Stage

| Evolution Stage | Max Slots | Unlock Condition |
|:--|:--|:--|
| Filhote | 4 | Inicial |
| Adulto | 8 | Após primeiro Ritual de Reconhecimento |
| Grande Slime | 12 | Após 3 Rituais de Reconhecimento |
| Rei Slime | 12 | Mantém 12 slots |
| Rei Slime Transcendente | 12 | Mantém 12 slots |

### Slot Layout

**Inventário Principal:**
```
Grid 4 colunas × N linhas (baseado em capacidade)

Filhote (4 slots):
[0] [1] [2] [3]

Adulto (8 slots):
[0] [1] [2] [3]
[4] [5] [6] [7]

Grande+ (12 slots):
[0] [1] [2] [3]
[4] [5] [6] [7]
[8] [9] [10] [11]
```

**Slots Equipados (sempre 4):**
```
[Eq0] [Eq1] [Eq2] [Eq3]
  Q     E     Z     X    (Keyboard)
  L     LT    R     RT   (Gamepad)
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Acceptance Criteria Testing Prework

**1.1 WHEN o jogador pressiona o botão Inventory (Select/I) THEN o sistema SHALL abrir a interface do inventário e pausar o gameplay**
Thoughts: Este é um comportamento que deve ocorrer para qualquer estado válido do jogo. Podemos testar gerando estados aleatórios do jogo e verificando que pressionar Inventory sempre abre o inventário e pausa.
Testable: yes - property

**1.2 WHEN o inventário está aberto e o jogador pressiona o botão Inventory novamente THEN o sistema SHALL fechar o inventário e retomar o gameplay**
Thoughts: Este é um round-trip property - abrir e fechar deve retornar ao estado original de gameplay.
Testable: yes - property

**2.1 WHEN o slime está no estágio Filhote THEN o sistema SHALL disponibilizar 4 slots de inventário**
Thoughts: Para qualquer slime no estágio Filhote, a capacidade deve ser exatamente 4.
Testable: yes - property

**2.4 WHEN a capacidade do inventário aumenta THEN o sistema SHALL preservar todos os itens já armazenados**
Thoughts: Invariante - itens devem ser preservados após expansão de capacidade.
Testable: yes - property

**3.1 WHEN o jogador coleta um item idêntico a um já existente no inventário THEN o sistema SHALL adicionar à pilha existente ao invés de ocupar novo slot**
Thoughts: Para qualquer item empilhável, adicionar deve sempre tentar empilhar primeiro.
Testable: yes - property

**3.4 WHEN a quantidade de uma pilha chega a zero THEN o sistema SHALL remover o item do slot e liberar o espaço**
Thoughts: Invariante - slots com quantidade zero devem estar vazios.
Testable: yes - property

**4.1 WHEN o inventário está aberto e o jogador usa Move THEN o sistema SHALL mover o cursor para o slot adjacente na direção correspondente**
Thoughts: Para qualquer posição válida do cursor e direção válida, deve mover corretamente.
Testable: yes - property

**5.2 WHEN o jogador escolhe "Equipar" THEN o sistema SHALL mover o item para o primeiro slot equipado vazio disponível**
Thoughts: Para qualquer item consumível, equipar deve sempre usar o primeiro slot vazio.
Testable: yes - property

**6.2 WHEN o jogador confirma o descarte THEN o sistema SHALL remover o item completamente do inventário**
Thoughts: Para qualquer item, descartar deve resultar em remoção completa.
Testable: yes - property

**7.4 WHEN o jogador usa um item equipado THEN o sistema SHALL atualizar a quantidade exibida no HUD imediatamente**
Thoughts: Para qualquer item equipado, usar deve decrementar quantidade visível.
Testable: yes - property

**8.1 WHEN o jogador pressiona UseItem1 THEN o sistema SHALL consumir o item no slot equipado 1**
Thoughts: Para qualquer item no slot 1, pressionar UseItem1 deve consumir.
Testable: yes - property

**10.2 WHEN o jogo é carregado THEN o sistema SHALL restaurar todos os itens do inventário nas posições corretas**
Thoughts: Round-trip property - salvar e carregar deve preservar estado completo.
Testable: yes - property

### Property Reflection

Analisando as propriedades identificadas:

**Redundâncias Identificadas:**
- Propriedade 2.1 (capacidade por estágio) pode ser combinada com outras verificações de capacidade em uma propriedade mais abrangente
- Propriedades 7.4 e 8.1 são relacionadas - usar item deve sempre atualizar UI, podemos combinar

**Propriedades Consolidadas:**
- Combinar 1.1 e 1.2 em uma propriedade de "toggle do inventário"
- Combinar 7.4 e 8.1 em "uso de item atualiza estado"
- Combinar 10.2 com 2.4 em "persistência preserva estado"

### Correctness Properties

**Property 1: Inventário toggle preserva estado de gameplay**
*For any* estado válido do jogo, abrir e fechar o inventário deve retornar ao mesmo estado de gameplay (não-pausado)
**Validates: Requirements 1.1, 1.2**

**Property 2: Capacidade de inventário corresponde ao estágio de evolução**
*For any* estágio de evolução válido, a capacidade do inventário deve corresponder exatamente ao valor definido (Filhote=4, Adulto=8, Grande+=12)
**Validates: Requirements 2.1, 2.2, 2.3**

**Property 3: Expansão de capacidade preserva itens**
*For any* inventário com itens, expandir a capacidade deve manter todos os itens nas mesmas posições
**Validates: Requirements 2.4**

**Property 4: Empilhamento prioriza slots existentes**
*For any* item empilhável já presente no inventário, adicionar mais unidades deve sempre tentar empilhar antes de criar novo slot
**Validates: Requirements 3.1**

**Property 5: Slots vazios têm quantidade zero**
*For any* slot no inventário, se a quantidade é zero então o slot deve estar vazio (item = null)
**Validates: Requirements 3.4**

**Property 6: Navegação move cursor para posição válida**
*For any* posição válida do cursor e direção de movimento, o cursor deve mover para slot adjacente válido ou permanecer na posição atual
**Validates: Requirements 4.1, 4.2**

**Property 7: Equipar usa primeiro slot vazio**
*For any* item consumível sendo equipado, o sistema deve colocá-lo no primeiro slot equipado vazio (índice mais baixo)
**Validates: Requirements 5.2**

**Property 8: Descartar remove item completamente**
*For any* item no inventário, confirmar descarte deve resultar em slot vazio e item não presente em nenhum outro slot
**Validates: Requirements 6.2**

**Property 9: Usar item atualiza quantidade e UI**
*For any* item equipado com quantidade > 0, usar o item deve decrementar quantidade em 1 e atualizar HUD imediatamente
**Validates: Requirements 7.4, 8.1**

**Property 10: Save/Load preserva estado completo**
*For any* estado válido do inventário, salvar e carregar deve restaurar exatamente o mesmo estado (itens, quantidades, posições, equipados)
**Validates: Requirements 10.2, 10.3**

## Error Handling

### Error Scenarios

**1. Inventário Cheio**
- **Trigger**: Tentar adicionar item quando todos slots estão ocupados
- **Handling**: 
  - Retornar false em AddItem()
  - Disparar evento OnInventoryFull
  - UI exibe notificação "Inventário Cheio"
  - Item não é coletado (permanece no mundo)

**2. Slot Equipado Inválido**
- **Trigger**: Tentar equipar item em índice fora do range [0-3]
- **Handling**:
  - Log de erro com UnityEngine.Debug.LogError
  - Retornar false
  - Não modificar estado do inventário

**3. Item Não Encontrado**
- **Trigger**: Tentar remover item que não existe no inventário
- **Handling**:
  - Log de warning
  - Retornar false
  - Não modificar estado

**4. Dados de Save Corrompidos**
- **Trigger**: LoadSaveData recebe dados inválidos
- **Handling**:
  - Log de erro detalhado
  - Aplicar valores padrão (inventário vazio, capacidade baseada em evolução)
  - Notificar SaveManager sobre corrupção

**5. Item Data Nulo**
- **Trigger**: Tentar adicionar item com ItemData = null
- **Handling**:
  - Log de erro
  - Retornar false imediatamente
  - Não modificar inventário

### Validation Methods

```csharp
private bool ValidateSlotIndex(int index)
{
    if (index < 0 || index >= MaxSlots)
    {
        UnityEngine.Debug.LogError($"[InventoryManager] Índice de slot inválido: {index}");
        return false;
    }
    return true;
}

private bool ValidateEquippedSlotIndex(int index)
{
    if (index < 0 || index >= 4)
    {
        UnityEngine.Debug.LogError($"[InventoryManager] Índice de slot equipado inválido: {index}");
        return false;
    }
    return true;
}

private bool ValidateItemData(ItemData item)
{
    if (item == null)
    {
        UnityEngine.Debug.LogError("[InventoryManager] ItemData é nulo");
        return false;
    }
    return true;
}
```

## Testing Strategy

### Unit Testing

**InventoryManager Tests:**
- `AddItem_WithEmptySlot_AddsSuccessfully()`
- `AddItem_WithFullInventory_ReturnsFalse()`
- `AddItem_StackableItem_StacksCorrectly()`
- `RemoveItem_ExistingItem_RemovesSuccessfully()`
- `EquipItem_ValidSlot_EquipsSuccessfully()`
- `UnequipItem_EquippedItem_UnequipsSuccessfully()`
- `UseEquippedItem_ValidItem_ConsumesAndAppliesEffect()`
- `DiscardItem_ValidSlot_RemovesItem()`
- `UpdateCapacity_Evolution_ExpandsCorrectly()`

**InventorySlot Tests:**
- `AddToStack_WithSpace_AddsCorrectly()`
- `AddToStack_AtMaxStack_ReturnsFalse()`
- `RemoveFromStack_ValidAmount_RemovesCorrectly()`
- `RemoveFromStack_ToZero_ClearsSlot()`

### Property-Based Testing

**Framework**: Unity Test Framework com gerador customizado de dados aleatórios

**Property Tests:**

**Test 1: Inventory Toggle Round Trip**
- **Property 1: Inventário toggle preserva estado de gameplay**
- Gerar estado aleatório do jogo
- Abrir inventário (pausar)
- Fechar inventário
- Verificar que gameplay está não-pausado
- Mínimo 100 iterações

**Test 2: Capacity Matches Evolution**
- **Property 2: Capacidade de inventário corresponde ao estágio de evolução**
- Gerar estágio de evolução aleatório
- Aplicar estágio ao inventário
- Verificar capacidade corresponde ao esperado
- Mínimo 100 iterações

**Test 3: Expansion Preserves Items**
- **Property 3: Expansão de capacidade preserva itens**
- Gerar inventário aleatório com itens
- Expandir capacidade
- Verificar todos itens ainda presentes nas mesmas posições
- Mínimo 100 iterações

**Test 4: Stacking Priority**
- **Property 4: Empilhamento prioriza slots existentes**
- Gerar inventário com item empilhável
- Adicionar mais unidades do mesmo item
- Verificar que empilhou ao invés de criar novo slot
- Mínimo 100 iterações

**Test 5: Empty Slots Have Zero Quantity**
- **Property 5: Slots vazios têm quantidade zero**
- Gerar inventário aleatório
- Para cada slot, verificar: quantidade == 0 ⟺ item == null
- Mínimo 100 iterações

**Test 6: Navigation Moves to Valid Position**
- **Property 6: Navegação move cursor para posição válida**
- Gerar posição aleatória do cursor
- Gerar direção aleatória
- Navegar
- Verificar cursor está em posição válida
- Mínimo 100 iterações

**Test 7: Equip Uses First Empty Slot**
- **Property 7: Equipar usa primeiro slot vazio**
- Gerar configuração aleatória de slots equipados
- Equipar novo item
- Verificar item está no primeiro slot vazio (menor índice)
- Mínimo 100 iterações

**Test 8: Discard Removes Completely**
- **Property 8: Descartar remove item completamente**
- Gerar inventário com item aleatório
- Descartar item
- Verificar item não existe em nenhum slot
- Mínimo 100 iterações

**Test 9: Use Item Updates Quantity and UI**
- **Property 9: Usar item atualiza quantidade e UI**
- Gerar item equipado com quantidade aleatória > 0
- Usar item
- Verificar quantidade decrementou em 1
- Verificar UI foi atualizada
- Mínimo 100 iterações

**Test 10: Save/Load Round Trip**
- **Property 10: Save/Load preserva estado completo**
- Gerar inventário aleatório completo
- Salvar estado
- Carregar estado
- Verificar estado idêntico ao original
- Mínimo 100 iterações

### Integration Testing

- Integração com PlayerController (coleta de itens)
- Integração com SaveManager (persistência)
- Integração com InputSystem (controles)
- Integração com AudioManager (sons)
- Integração com sistema de evolução (capacidade)

## UI/UX Design

### Game Pause Behavior

**IMPORTANTE: Quando o inventário é aberto, o jogo é pausado completamente.**

**Implementação:**
```csharp
public void OpenInventory()
{
    inventoryPanel.SetActive(true);
    Time.timeScale = 0f; // Pausa o jogo
    // Desabilita inputs de gameplay
    inputActions.Gameplay.Disable();
    // Habilita inputs de menu (navegação)
    inputActions.Menu.Enable();
}

public void CloseInventory()
{
    inventoryPanel.SetActive(false);
    Time.timeScale = 1f; // Retoma o jogo
    // Habilita inputs de gameplay
    inputActions.Gameplay.Enable();
    // Desabilita inputs de menu
    inputActions.Menu.Disable();
}
```

**Comportamento Durante Pausa:**
- ✅ Animações de UI continuam funcionando (usam `unscaledDeltaTime`)
- ✅ Navegação e seleção de itens funcionam normalmente
- ✅ Sons de UI são reproduzidos normalmente
- ❌ Movimento do jogador é bloqueado
- ❌ Inimigos param de se mover
- ❌ Timers de gameplay são pausados
- ❌ Física do jogo é pausada

### Visual Layout

**Inventário Aberto (Jogo Pausado):**
```
┌─────────────────────────────────────────┐
│         INVENTÁRIO (4/12)               │
├─────────────────────────────────────────┤
│  [Slot0] [Slot1] [Slot2] [Slot3]       │
│  [Slot4] [Slot5] [Slot6] [Slot7]       │
│  [Slot8] [Slot9] [Slot10] [Slot11]     │
│                                         │
│  ─────── EQUIPADOS ───────              │
│  [Eq0]   [Eq1]   [Eq2]   [Eq3]         │
│   Q       E       Z       X             │
└─────────────────────────────────────────┘
```

**HUD (Sempre Visível):**
```
                                    [Eq0] Q
                                    [Eq1] E
                                    [Eq2] Z
                                    [Eq3] X
```

### Visual Feedback

**Slot States:**
- **Empty**: Background escurecido, sem ícone
- **Occupied**: Ícone do item, quantidade no canto
- **Selected**: Borda brilhante amarela/dourada
- **Equipped**: Borda verde sutil

**Animations:**
- **Item Added**: Fade in + scale bounce (0.3s)
- **Item Removed**: Fade out + scale shrink (0.2s)
- **Item Used**: Flash branco + particle effect (0.15s)
- **Navigation**: Smooth lerp do highlight (0.1s)

**Audio:**
- **Open Inventory**: Soft whoosh
- **Close Inventory**: Soft whoosh (reverse)
- **Navigate**: Subtle click
- **Select**: Confirmation beep
- **Equip**: Equip sound
- **Discard**: Trash sound
- **Use Item**: Item-specific sound

## Performance Considerations

### Optimization Strategies

**1. Object Pooling para UI**
- Pool de 12 SlotUI GameObjects
- Reutilizar ao invés de Instantiate/Destroy
- Reduz garbage collection

**2. Caching de Referências**
- Cache InputSystem actions no Awake
- Cache Transform components
- Cache AudioClips

**3. Event Throttling**
- OnInventoryChanged dispara no máximo 1x por frame
- Usar dirty flag ao invés de atualizar UI imediatamente

**4. Lazy Loading de Sprites**
- Carregar ícones de itens apenas quando necessário
- Usar Addressables para itens raros

**5. Batch UI Updates**
- Atualizar todos slots de uma vez ao invés de individualmente
- Usar Canvas.ForceUpdateCanvases() apenas quando necessário

### Memory Management

**Estimated Memory Usage:**
- InventoryManager: ~2 KB
- 12 InventorySlots: ~1 KB
- 4 Equipped Slots: ~0.3 KB
- UI Elements: ~50 KB (sprites, textures)
- **Total**: ~53 KB (negligível)

## Integration Points

### 1. SaveManager Integration

```csharp
// InventoryManager
public InventorySaveData GetSaveData()
{
    var data = new InventorySaveData
    {
        maxSlots = MaxSlots,
        slots = new List<SlotSaveData>(),
        equippedSlots = new SlotSaveData[4]
    };
    
    foreach (var slot in Slots)
    {
        data.slots.Add(new SlotSaveData
        {
            itemID = slot.item?.itemID ?? "",
            quantity = slot.quantity
        });
    }
    
    for (int i = 0; i < 4; i++)
    {
        data.equippedSlots[i] = new SlotSaveData
        {
            itemID = EquippedSlots[i].item?.itemID ?? "",
            quantity = EquippedSlots[i].quantity
        };
    }
    
    return data;
}
```

### 2. Evolution System Integration

```csharp
// GameManager ou EvolutionManager
private void OnPlayerEvolved(EvolutionStage newStage)
{
    InventoryManager.Instance.UpdateCapacity(newStage);
}
```

### 3. Input System Integration

**IMPORTANTE: Criar novo ActionMap "Menu" no InputSystem_Actions.inputactions**

O sistema atual só tem ActionMap "Gameplay". Precisamos adicionar ActionMap "Menu" para inputs durante pausa.

```csharp
// InventoryUI
private InputSystem_Actions inputActions;

private void Awake()
{
    inputActions = new InputSystem_Actions();
}

private void OnEnable()
{
    // Inventory toggle funciona em ambos os modos
    inputActions.Gameplay.Inventory.performed += OnInventoryToggle;
    inputActions.Menu.Inventory.performed += OnInventoryToggle;
    
    // Navegação apenas no menu
    inputActions.Menu.Move.performed += OnNavigate;
    inputActions.Menu.Attack.performed += OnSelect;
}

private void OnDisable()
{
    inputActions.Gameplay.Inventory.performed -= OnInventoryToggle;
    inputActions.Menu.Inventory.performed -= OnInventoryToggle;
    inputActions.Menu.Move.performed -= OnNavigate;
    inputActions.Menu.Attack.performed -= OnSelect;
}

private void OnInventoryToggle(InputAction.CallbackContext context)
{
    if (inventoryPanel.activeSelf)
    {
        CloseInventory(); // Time.timeScale = 1
    }
    else
    {
        OpenInventory(); // Time.timeScale = 0
    }
}
```

### 4. Item Collection Integration

```csharp
// ItemCollectable
private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        bool added = InventoryManager.Instance.AddItem(itemData, quantity);
        if (added)
        {
            Destroy(gameObject);
        }
        else
        {
            // Show "Inventory Full" message
        }
    }
}
```

## Future Extensibility

### Planned Extensions

1. **Item Categories**: Separar consumíveis, materiais, quest items
2. **Sorting**: Ordenar por tipo, nome, quantidade
3. **Filtering**: Filtrar por categoria
4. **Drag & Drop**: Reorganizar itens manualmente
5. **Item Comparison**: Comparar stats de equipamentos
6. **Quick Stack**: Empilhar todos itens idênticos automaticamente
7. **Favorites**: Marcar itens favoritos para não descartar acidentalmente

### Extension Points

```csharp
// ItemData pode ser estendido
public class EquipmentItemData : ItemData
{
    public int defense;
    public int attack;
    // ...
}

// InventoryManager pode ter filtros customizados
public List<InventorySlot> GetItemsByCategory(ItemCategory category)
{
    return Slots.Where(s => !s.IsEmpty && s.item.category == category).ToList();
}
```

## Conclusion

O design do Sistema de Inventário prioriza simplicidade, performance e extensibilidade. A arquitetura modular permite fácil manutenção e adição de novas funcionalidades. O uso de eventos desacopla componentes e facilita integração com outros sistemas. As propriedades de corretude garantem comportamento consistente e confiável em todas as situações.
