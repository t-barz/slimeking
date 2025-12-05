# Guia de Teste - Sistema de Exibição do Inventário

## Visão Geral

Este documento descreve os passos para testar o sistema de exibição de itens no inventário com 12 slots não empilháveis.

## Pré-requisitos

1. **Configurar Slots no Unity Editor:**
   - Abra o Unity Editor
   - Vá em `Extra Tools > Inventory > Configure 12 Slots`
   - Isso conectará os 12 slots existentes ao InventoryUI
   - (Opcional) Vá em `Extra Tools > Inventory > Remove Quantity Text`
   - Isso desabilitará os textos de quantidade (não usados no sistema não empilhável)

2. **Verificar Estrutura da Cena:**
   - Certifique-se de que existe um GameObject `InventoryUI` na cena
   - Verifique que o `InventoryManager` está presente (geralmente em um GameObject persistente)

3. **Criar Itens de Teste:**
   - Crie pelo menos 2-3 ItemData diferentes em `Assets/Resources/Items/`
   - Configure ícones para cada item
   - Marque `isStackable = false` (ou deixe true, o sistema ignora)

## Testes de Funcionalidade

### Teste 1: Coleta de Item Único

**Objetivo:** Verificar que um item coletado aparece no primeiro slot vazio.

**Passos:**
1. Inicie o jogo
2. Abra o inventário (tecla configurada no PauseManager)
3. Verifique que todos os slots estão vazios
4. Feche o inventário
5. Colete 1 item usando o sistema de interação (tecla E)
6. Abra o inventário novamente
7. **Resultado Esperado:** O item deve aparecer no Slot 0 com seu ícone visível

### Teste 2: Coleta de Múltiplos Itens do Mesmo Tipo

**Objetivo:** Verificar que itens do mesmo tipo ocupam slots separados (não empilhável).

**Passos:**
1. Inicie o jogo com inventário vazio
2. Colete 3 itens do mesmo tipo
3. Abra o inventário
4. **Resultado Esperado:** 
   - 3 slots devem estar ocupados (Slots 0, 1, 2)
   - Cada slot deve mostrar o mesmo ícone
   - Nenhum slot deve mostrar número de quantidade

### Teste 3: Coleta de Itens Diferentes

**Objetivo:** Verificar que itens diferentes são adicionados sequencialmente.

**Passos:**
1. Inicie o jogo com inventário vazio
2. Colete Item A
3. Colete Item B
4. Colete Item C
5. Abra o inventário
6. **Resultado Esperado:**
   - Slot 0: Item A
   - Slot 1: Item B
   - Slot 2: Item C

### Teste 4: Inventário Cheio (12 Slots)

**Objetivo:** Verificar que não é possível coletar itens quando o inventário está cheio.

**Passos:**
1. Colete 12 itens (pode ser do mesmo tipo ou diferentes)
2. Abra o inventário
3. Verifique que todos os 12 slots estão ocupados
4. Feche o inventário
5. Tente coletar mais 1 item
6. **Resultado Esperado:**
   - O item não deve ser coletado
   - Deve aparecer log no console: "[InventoryManager] Inventário cheio"
   - O item deve permanecer no mundo

### Teste 5: Sincronização em Tempo Real

**Objetivo:** Verificar que a UI atualiza automaticamente quando itens são adicionados.

**Passos:**
1. Abra o inventário (deixe aberto)
2. Com o inventário aberto, colete um item
3. **Resultado Esperado:**
   - O item deve aparecer imediatamente no próximo slot vazio
   - Não deve ser necessário fechar e reabrir o inventário

### Teste 6: Abertura/Fechamento do Inventário

**Objetivo:** Verificar que o estado é mantido entre aberturas.

**Passos:**
1. Colete 3 itens
2. Abra o inventário - verifique que os 3 itens estão lá
3. Feche o inventário
4. Colete mais 2 itens
5. Abra o inventário novamente
6. **Resultado Esperado:**
   - Os 3 itens originais ainda devem estar nos mesmos slots
   - Os 2 novos itens devem estar nos próximos slots vazios (Slots 3 e 4)

### Teste 7: Item Sem Ícone

**Objetivo:** Verificar tratamento de erro para itens sem sprite configurado.

**Passos:**
1. Crie um ItemData sem configurar o campo `icon`
2. Colete esse item
3. Abra o inventário
4. **Resultado Esperado:**
   - O slot deve estar ocupado mas sem ícone visível
   - Deve aparecer warning no console: "[InventorySlotUI] Item 'NomeDoItem' não possui ícone configurado!"

## Testes de Performance

### Teste 8: Múltiplas Coletas Rápidas

**Objetivo:** Verificar que o sistema lida bem com coletas em sequência rápida.

**Passos:**
1. Crie uma cena de teste com 10 itens próximos
2. Colete todos rapidamente (spam da tecla E)
3. Abra o inventário
4. **Resultado Esperado:**
   - Todos os itens devem estar corretamente nos slots
   - Não deve haver duplicatas ou slots vazios entre itens

## Verificações de Log

Durante os testes, verifique os seguintes logs no console (se `enableLogs = true` no InventoryUI):

- `[InventoryUI] Inicializados 12 slots`
- `[InventoryUI] Inscrito nos eventos do InventoryManager`
- `[InventoryUI] Showing inventory`
- `[InventoryUI] Todos os slots atualizados`
- `[InventoryManager] Inventário cheio. Adicionados X/Y itens.` (quando cheio)

## Troubleshooting

### Problema: Slots não aparecem na UI

**Solução:**
1. Verifique que executou `Extra Tools > Inventory > Configure 12 Slots`
2. Verifique que o `slotsContainer` está configurado no InventoryUI
3. Verifique a hierarquia: `InventoryUI/InventoryPanel/SlotsContainer`

### Problema: Itens não aparecem após coleta

**Solução:**
1. Execute `Extra Tools > Inventory > Fix Slot References` para corrigir referências
2. Execute `Extra Tools > Inventory > Debug Inventory State` (em Play Mode) para ver o estado
3. Verifique que o InventoryManager está na cena
4. Verifique que o evento `OnInventoryChanged` está sendo disparado
5. Verifique logs: `enableLogs = true` no InventoryUI
6. Verifique que o ItemData tem um ícone configurado
7. Verifique que o `slotsContainer` está configurado no InventoryUI (Inspector)

### Problema: Itens estão empilhando

**Solução:**
1. Verifique que está usando a versão atualizada do InventoryManager
2. O método `AddItem()` deve adicionar cada item em um slot separado
3. Verifique que `quantity` é sempre 1 em cada slot

## Checklist de Validação Final

- [ ] 12 slots são exibidos na UI
- [ ] Slots vazios não mostram ícone
- [ ] Slots com itens mostram o ícone correto
- [ ] Nenhum slot mostra número de quantidade
- [ ] Itens do mesmo tipo ocupam slots separados
- [ ] Inventário cheio impede coleta de novos itens
- [ ] UI sincroniza automaticamente com mudanças
- [ ] Estado é mantido entre aberturas do inventário
- [ ] Logs de erro aparecem para itens sem ícone
- [ ] Não há erros no console durante uso normal

## Próximos Passos

Após validar todos os testes:
1. Desabilitar logs de debug (`enableLogs = false`)
2. Testar em diferentes resoluções de tela
3. Testar com diferentes tipos de itens (consumíveis, equipamentos, quest items)
4. Integrar com sistema de uso de itens (Task futura)
