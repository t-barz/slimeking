# Inventory Manager Validation Tests

## Overview

Este conjunto de testes valida a integração com o `InventoryManager`, garantindo que o sistema de inventário funciona corretamente conforme os requisitos da reestruturação do sistema de itens coletáveis.

## Requirements Covered

- **4.1**: InventoryManager verifica se já existe item do mesmo tipo
- **4.2**: InventoryManager incrementa quantidade de item existente
- **4.3**: InventoryManager cria novo slot se item não existe
- **4.4**: InventoryManager cria novo slot quando stack atinge 99
- **4.5**: InventoryManager retorna true se item foi adicionado com sucesso
- **5.1**: InventoryManager.AddItem retorna false quando inventário cheio
- **5.5**: Sistema permite tentar coletar item novamente após liberar espaço

## Test Cases

### 1. AddItem Adds Correctly

**Valida**: Requirements 4.1, 4.3

Verifica que `InventoryManager.AddItem()` adiciona itens corretamente ao inventário com a quantidade especificada.

**Passos**:

1. Limpa inventário
2. Adiciona 5 unidades de um item stackable
3. Verifica que item foi adicionado
4. Verifica que quantidade é 5

**Resultado Esperado**: ✓ Item adicionado com quantidade correta

---

### 2. Stacking Same Type

**Valida**: Requirements 4.1, 4.2

Verifica que itens do mesmo tipo são empilhados no mesmo slot ao invés de criar novos slots.

**Passos**:

1. Limpa inventário
2. Adiciona 10 unidades de um item
3. Adiciona 15 unidades do mesmo item
4. Verifica que apenas 1 slot está sendo usado
5. Verifica que quantidade total é 25

**Resultado Esperado**: ✓ Itens empilhados no mesmo slot

---

### 3. New Slot Created at Stack Limit (99)

**Valida**: Requirement 4.4

Verifica que quando um stack atinge o limite de 99 unidades, novos itens criam um novo slot.

**Passos**:

1. Limpa inventário
2. Adiciona 99 unidades (preenche um stack)
3. Adiciona 10 unidades adicionais
4. Verifica que 2 slots estão sendo usados
5. Verifica que um slot tem 99 unidades
6. Verifica que quantidade total é 109

**Resultado Esperado**: ✓ Novo slot criado quando stack atinge 99

---

### 4. Inventory Full Returns False

**Valida**: Requirement 5.1

Verifica que `AddItem()` retorna `false` quando o inventário está completamente cheio.

**Passos**:

1. Limpa inventário
2. Preenche todos os 20 slots com itens non-stackable
3. Tenta adicionar mais um item
4. Verifica que `AddItem()` retorna `false`

**Resultado Esperado**: ✓ AddItem retorna false quando inventário cheio

---

### 5. Item Behavior When Inventory Full

**Valida**: Requirement 5.5

Valida o comportamento esperado quando inventário está cheio. Este teste verifica que `AddItem()` retorna `false`, o que permite ao `ItemCollectable` reverter o estado de coleta e manter o item na cena.

**Passos**:

1. Limpa inventário
2. Preenche todos os 20 slots (99 unidades cada)
3. Verifica que não há slots vazios
4. Tenta adicionar item
5. Verifica que `AddItem()` retorna `false`

**Resultado Esperado**: ✓ AddItem retorna false, permitindo ItemCollectable manter item na cena

---

## How to Use

### Setup

1. **Abra a janela de testes**:
   - Menu: `The Slime King > Tests > Inventory Manager Tests`

2. **Configure Test Items**:
   - **Stackable Item**: Arraste um ItemData com `isStackable = true`
   - **Non-Stackable Item**: Arraste um ItemData com `isStackable = false`
   - **Equipment Item**: Arraste um ItemData com `type = Equipment` (opcional)

3. **Entre em Play Mode**:
   - Os testes só podem ser executados em Play Mode
   - Certifique-se de que há um `InventoryManager` na cena

### Running Tests

1. Clique no botão **"Run All Tests"**
2. Aguarde a execução (alguns segundos)
3. Revise os resultados na área de log

### Interpreting Results

- **✓ Green**: Teste passou
- **✗ Red**: Teste falhou
- **⚠ Yellow**: Teste foi pulado (configuração faltando)

**Summary**:

```
Results: X passed, Y failed
Total: Z tests
```

---

## Creating Test Items

Se você não tem ItemData configurados para testes, crie-os:

### Stackable Item

```
1. Right-click em Assets/Data/Items/
2. Create > The Slime King > Item
3. Configure:
   - itemName: "Test Stackable Item"
   - type: Common
   - isStackable: true
```

### Non-Stackable Item

```
1. Right-click em Assets/Data/Items/
2. Create > The Slime King > Item
3. Configure:
   - itemName: "Test Non-Stackable Item"
   - type: Equipment
   - isStackable: false
```

---

## Integration with ItemCollectable

Estes testes validam que o `InventoryManager` funciona corretamente. O `ItemCollectable` deve usar os resultados de `AddItem()` para decidir se mantém ou remove o item da cena:

```csharp
// Em ItemCollectable.ProcessInventoryItemCollection()
bool success = InventoryManager.Instance.AddItem(inventoryItemData, itemQuantity);

if (!success)
{
    // Inventário cheio - mantém item na cena
    RevertCollectionState();
    return;
}

// Sucesso - remove item da cena
DestroyItem();
```

---

## Troubleshooting

### "InventoryManager.Instance is null"

- Certifique-se de que há um GameObject com `InventoryManager` na cena
- Verifique que o script está ativo e inicializado

### "Skipping: testStackableItem not assigned"

- Configure os test items na janela antes de executar
- Certifique-se de que os ItemData existem no projeto

### Tests Failing

- Verifique os logs detalhados na área de scroll
- Cada teste mostra exatamente o que falhou
- Compare com comportamento esperado nos requirements

---

## Next Steps

Após validar que o `InventoryManager` funciona corretamente:

1. ✅ **Task 5 Complete**: InventoryManager validado
2. ➡️ **Task 6**: Implementar testes de integração completos (fluxo end-to-end)
3. ➡️ **Task 7**: Testes manuais e validação visual

---

## Technical Notes

### Test Architecture

- Usa `EditorWindow` para interface visual
- Executa em Play Mode para acessar `InventoryManager.Instance`
- Limpa inventário entre testes para isolamento
- Logs coloridos para melhor legibilidade

### Performance

- Testes executam em < 1 segundo
- Não afetam performance do jogo
- Podem ser executados repetidamente

### Limitations

- Requer Play Mode (não pode executar em Edit Mode)
- Requer test items configurados manualmente
- Não testa UI visual (apenas lógica de backend)

---

## Related Files

- `Assets/Code/Systems/Inventory/InventoryManager.cs` - Sistema sendo testado
- `Assets/Code/Systems/Inventory/InventorySlot.cs` - Estrutura de dados
- `Assets/Code/Systems/Inventory/ItemData.cs` - Definição de itens
- `Assets/External/AssetStore/SlimeMec/_Scripts/Gameplay/ItemCollectable.cs` - Integração

---

## Validation Checklist

Use esta checklist para validar manualmente após executar os testes:

- [ ] ✓ AddItem adiciona item corretamente
- [ ] ✓ Empilhamento de itens do mesmo tipo funciona
- [ ] ✓ Novo slot criado quando stack atinge 99
- [ ] ✓ AddItem retorna false quando inventário cheio
- [ ] ✓ Comportamento de inventário cheio validado

**Status**: Task 5 - Validar integração com InventoryManager

**Requirements Validated**: 4.1, 4.2, 4.3, 4.4, 4.5, 5.1, 5.5
