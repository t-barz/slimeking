# Troubleshooting - Sistema de Inventário

## Problemas Identificados nos Logs

### ❌ Problema 1: Slots não encontrados
```
[InventoryUI] Esperado 12 slots, encontrados 0!
```

**Causa:** Os GameObjects dos slots não têm o componente `InventorySlotUI` anexado.

**Solução:**
1. Abra a cena no Unity
2. Navegue até `InventoryUI/InventoryPanel/SlotsContainer`
3. Selecione cada slot filho
4. Adicione o componente `InventorySlotUI` (Add Component > Scripts > TheSlimeKing.UI > InventorySlotUI)
5. **OU** Execute `Extra Tools > Inventory > Fix Slot References` para tentar corrigir automaticamente

### ❌ Problema 2: InventoryManager não encontrado
```
[InventoryUI] InventoryManager.Instance não encontrado!
```

**Causa:** O GameObject com o `InventoryManager` não está na cena ou não está ativo.

**Solução:**
1. Verifique se existe um GameObject com o componente `InventoryManager` na cena
2. Se não existir, crie um:
   - GameObject > Create Empty
   - Renomeie para "InventoryManager"
   - Add Component > Scripts > TheSlimeKing.Inventory > InventoryManager
3. Certifique-se de que o GameObject está ativo (checkbox marcado no Inspector)

### ❌ Problema 3: InventoryManager.Instance não disponível para refresh
```
[InventoryManager] InventoryManager.Instance não disponível para refresh
```

**Causa:** O InventoryManager ainda não foi inicializado quando o InventoryUI tenta acessá-lo.

**Solução:**
1. Verifique a ordem de execução dos scripts (Script Execution Order)
2. O InventoryManager deve ser inicializado ANTES do InventoryUI
3. Ou adicione verificação null no código

## Checklist de Verificação

### ✅ Estrutura da Cena

- [ ] Existe GameObject `InventoryManager` na cena
- [ ] InventoryManager está ativo
- [ ] Existe GameObject `InventoryUI` na cena
- [ ] InventoryUI está ativo
- [ ] Estrutura: `InventoryUI/InventoryPanel/SlotsContainer`
- [ ] SlotsContainer tem 12 filhos (slots)

### ✅ Componentes dos Slots

Para cada um dos 12 slots:
- [ ] Tem componente `InventorySlotUI`
- [ ] Tem componente `Image` (background)
- [ ] Tem filho com `Image` para o ícone
- [ ] Tem componente `Button`
- [ ] Referências configuradas no `InventorySlotUI`:
  - [ ] `iconImage` apontando para a Image do ícone
  - [ ] `quantityText` apontando para o TextMeshProUGUI (pode estar desabilitado)
  - [ ] `button` apontando para o Button

### ✅ Configuração do InventoryUI

- [ ] Campo `slotsContainer` preenchido no Inspector
- [ ] Campo `inventoryPanel` preenchido no Inspector
- [ ] Campo `canvasGroup` preenchido no Inspector

### ✅ Itens de Teste

- [ ] Existe pelo menos 1 ItemData criado
- [ ] ItemData tem sprite configurado no campo `icon`
- [ ] ItemData está em `Assets/Resources/Items/` (se usar sistema de save/load)

## Scripts de Correção Automática

### 1. Configure 12 Slots
**Menu:** `Extra Tools > Inventory > Configure 12 Slots`

**O que faz:**
- Conecta a referência do `slotsContainer` no InventoryUI
- Valida que existem 12 slots

### 2. Fix Slot References
**Menu:** `Extra Tools > Inventory > Fix Slot References`

**O que faz:**
- Encontra automaticamente as Images e TextMeshProUGUI em cada slot
- Conecta as referências no componente InventorySlotUI
- Corrige referências quebradas

### 3. Remove Quantity Text
**Menu:** `Extra Tools > Inventory > Remove Quantity Text`

**O que faz:**
- Desabilita os textos de quantidade (não usados no sistema não empilhável)

### 4. Debug Inventory State (Play Mode)
**Menu:** `Extra Tools > Inventory > Debug Inventory State`

**O que faz:**
- Mostra o estado completo do inventário
- Lista todos os itens nos slots
- Verifica referências da UI
- Identifica problemas

## Passo a Passo para Corrigir

### Se os slots não aparecem:

1. **Verifique a estrutura:**
   ```
   InventoryUI
   └── InventoryPanel
       └── SlotsContainer
           ├── Slot (ou similar) x12
   ```

2. **Adicione InventorySlotUI aos slots:**
   - Selecione cada slot
   - Add Component > InventorySlotUI

3. **Execute Fix Slot References:**
   - `Extra Tools > Inventory > Fix Slot References`

4. **Execute Configure 12 Slots:**
   - `Extra Tools > Inventory > Configure 12 Slots`

### Se os itens não aparecem após coleta:

1. **Verifique que InventoryManager existe:**
   - Procure na Hierarchy por "InventoryManager"
   - Se não existir, crie um GameObject vazio e adicione o componente

2. **Habilite logs para debug:**
   - Selecione InventoryUI
   - Marque `Enable Inventory Logs = true`
   - Entre em Play Mode e veja os logs

3. **Execute Debug em Play Mode:**
   - Entre em Play Mode
   - Colete um item
   - `Extra Tools > Inventory > Debug Inventory State`
   - Veja o relatório completo

4. **Verifique o ItemData:**
   - Selecione o ItemData do item
   - Verifique se o campo `icon` tem um sprite

## Logs Importantes

### ✅ Logs de Sucesso:
```
[InventoryUI] Inicializados 12 slots
[InventoryUI] Inscrito nos eventos do InventoryManager
[InventoryUI] Todos os slots atualizados
[InventoryManager] Inventário salvo com X itens
```

### ❌ Logs de Erro:
```
[InventoryUI] Slots container não configurado!
[InventoryUI] Esperado 12 slots, encontrados X!
[InventoryUI] InventoryManager.Instance não encontrado!
[InventorySlotUI] Item 'X' não possui ícone configurado!
```

## Contato para Suporte

Se após seguir todos os passos o problema persistir:
1. Habilite `Enable Inventory Logs = true` no InventoryUI
2. Execute `Debug Inventory State` em Play Mode
3. Copie todos os logs do Console
4. Documente o problema com screenshots
