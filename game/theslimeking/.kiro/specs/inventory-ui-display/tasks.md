# Implementation Plan

- [x] 1. Atualizar InventoryManager para 12 slots não empilháveis


  - Alterar array de slots de 20 para 12
  - Modificar método AddItem() para não empilhar itens (cada item ocupa 1 slot)
  - Atualizar método FindEmptySlot() se necessário
  - Atualizar métodos de save/load para 12 slots
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5_

- [ ]* 1.1 Escrever teste de propriedade para alocação de primeiro slot vazio
  - **Property 3: First empty slot allocation**
  - **Validates: Requirements 2.1**

- [ ]* 1.2 Escrever teste de propriedade para comportamento não empilhável
  - **Property 2: Non-stacking behavior**
  - **Validates: Requirements 2.2, 2.5**

- [ ]* 1.3 Escrever teste de propriedade para liberação de slot
  - **Property 4: Slot liberation on removal**
  - **Validates: Requirements 2.4**

- [x] 2. Implementar inicialização e sincronização de slots no InventoryUI


  - Criar array de 12 InventorySlotUI
  - Implementar método InitializeSlots() para obter referências aos slots UI
  - Implementar método SubscribeToEvents() para escutar OnInventoryChanged
  - Implementar método RefreshAllSlots() para atualizar todos os slots
  - Adicionar chamada RefreshAllSlots() no método Show()
  - Implementar OnDisable() para fazer unsubscribe dos eventos
  - _Requirements: 1.4, 1.5, 3.4, 3.5_

- [ ]* 2.1 Escrever teste de propriedade para sincronização via eventos
  - **Property 5: Event-driven synchronization**
  - **Validates: Requirements 1.5, 3.4**

- [ ]* 2.2 Escrever teste de propriedade para consistência ao abrir UI
  - **Property 6: UI state consistency on open**
  - **Validates: Requirements 1.4, 3.2**

- [x] 3. Garantir que InventorySlotUI exibe corretamente itens e slots vazios


  - Verificar que método Refresh() oculta ícone quando slot está vazio
  - Verificar que método Refresh() exibe ícone correto quando slot tem item
  - Garantir que quantityText sempre fica vazio (não empilhável)
  - Adicionar tratamento para ItemData sem sprite de ícone
  - _Requirements: 1.2, 1.3_

- [ ]* 3.1 Escrever teste de propriedade para reflexão de estado na UI
  - **Property 1: Slot display reflects inventory state**
  - **Validates: Requirements 1.2, 1.3, 1.4**

- [x] 4. Configurar prefab do InventoryUI no Unity Editor


  - Criar ou atualizar prefab com 12 InventorySlotUI como filhos
  - Configurar Grid Layout Group para 3 linhas x 4 colunas
  - Conectar referências dos slots no InventoryUI
  - Testar visualmente a grade de slots
  - _Requirements: 1.1_

- [x] 5. Implementar tratamento de erros e logs


  - Adicionar verificação null para InventoryManager.Instance
  - Adicionar logs condicionais para inicialização e sincronização
  - Implementar tratamento para inventário cheio
  - Adicionar logs de warning para ícones faltantes
  - _Requirements: 4.3_

- [ ]* 5.1 Escrever testes unitários para casos de erro
  - Testar comportamento quando InventoryManager é null
  - Testar comportamento quando inventário está cheio
  - Testar comportamento quando ItemData não tem ícone
  - _Requirements: 2.3_

- [x] 6. Checkpoint - Garantir que todos os testes passam


  - Ensure all tests pass, ask the user if questions arise.

- [x] 7. Testar integração completa do fluxo de coleta


  - Criar cena de teste com player e itens coletáveis
  - Testar coleta de 1 item e verificar exibição na UI
  - Testar coleta de múltiplos itens do mesmo tipo
  - Testar coleta até inventário ficar cheio (12 itens)
  - Testar abertura/fechamento do inventário
  - Verificar sincronização em tempo real
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 2.1, 2.2, 2.3, 3.1, 3.2, 3.3, 3.4_

- [x] 8. Checkpoint Final - Garantir que todos os testes passam



  - Ensure all tests pass, ask the user if questions arise.
