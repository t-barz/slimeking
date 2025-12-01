# Implementation Plan - Sistema de Inventário

## Overview

Este plano de implementação detalha as tarefas necessárias para criar um sistema de inventário completo e funcional para The Slime King. A implementação seguirá uma abordagem incremental, construindo primeiro as estruturas de dados, depois a lógica de negócio, e finalmente a interface de usuário.

## Tasks

- [ ] 1. Configurar Input System com ActionMap Menu
  - Adicionar novo ActionMap "Menu" ao InputSystem_Actions.inputactions
  - Mapear ações: Move, Attack (confirmar), Inventory (toggle)
  - Configurar bindings para Keyboard (WASD/Arrows, Space, I) e Gamepad (D-Pad/Stick, A, Select)
  - Regenerar classe InputSystem_Actions.cs
  - _Requirements: 1.1, 4.1_

- [ ] 2. Criar estruturas de dados base
- [ ] 2.1 Criar ItemData ScriptableObject
  - Implementar classe ItemData com campos: itemID, itemName, description, icon, maxStack
  - Adicionar campos de consumível: isConsumable, healAmount, buffDuration, buffMultiplier
  - Adicionar campo useSound (AudioClip)
  - Implementar método virtual Use() para aplicar efeitos
  - Criar menu CreateAssetMenu em "Inventory/Item Data"
  - _Requirements: 3.1, 8.1_

- [ ] 2.2 Criar classe InventorySlot
  - Implementar classe serializable com campos: ItemData item, int quantity
  - Implementar propriedades: IsEmpty, IsFull
  - Implementar método AddToStack(int amount) com validação de maxStack
  - Implementar método RemoveFromStack(int amount) com auto-clear quando zero
  - Implementar método Clear()
  - _Requirements: 3.1, 3.2, 3.4_

- [ ] 2.3 Criar InventorySaveData
  - Implementar classe serializable InventorySaveData
  - Criar classe interna SlotSaveData com itemID (string) e quantity (int)
  - Adicionar campos: List<SlotSaveData> slots, SlotSaveData[] equippedSlots (4), int maxSlots
  - _Requirements: 10.1, 10.2_

- [ ] 3. Implementar InventoryManager (Singleton)
- [ ] 3.1 Criar estrutura base do InventoryManager
  - Implementar padrão Singleton com Instance estática
  - Criar propriedades: MaxSlots, List<InventorySlot> Slots, InventorySlot[] EquippedSlots (4)
  - Implementar Awake() com inicialização de Singleton e DontDestroyOnLoad
  - Inicializar Slots e EquippedSlots com capacidade padrão (4 slots para Filhote)
  - Adicionar flag [SerializeField] bool enableLogs para debug condicional
  - _Requirements: 2.1_

- [ ] 3.2 Implementar sistema de eventos
  - Declarar eventos estáticos: OnItemAdded, OnItemRemoved, OnItemUsed, OnItemEquipped, OnItemUnequipped, OnInventoryChanged, OnInventoryFull
  - Implementar métodos privados para disparar eventos com logs condicionais
  - _Requirements: 1.3, 5.2, 6.2_

- [ ] 3.3 Implementar AddItem()
  - Validar ItemData não-nulo
  - Buscar slot existente com mesmo item e espaço disponível (empilhamento)
  - Se não encontrar, buscar primeiro slot vazio
  - Se inventário cheio, disparar OnInventoryFull e retornar false
  - Adicionar item ao slot, disparar OnItemAdded e OnInventoryChanged
  - Retornar true em sucesso
  - _Requirements: 3.1, 3.2, 2.5_

- [ ] 3.4 Implementar RemoveItem()
  - Validar ItemData não-nulo
  - Buscar slot com o item
  - Chamar RemoveFromStack() no slot
  - Disparar OnItemRemoved e OnInventoryChanged
  - Retornar true se removeu, false se não encontrou
  - _Requirements: 3.4, 6.2_

- [ ] 3.5 Implementar EquipItem()
  - Validar índices de slot de inventário e slot equipado
  - Verificar se slot de inventário contém item consumível
  - Verificar se slot equipado está vazio
  - Mover item do inventário para slot equipado
  - Disparar OnItemEquipped e OnInventoryChanged
  - Retornar true em sucesso, false em falha
  - _Requirements: 5.1, 5.2_

- [ ] 3.6 Implementar UnequipItem()
  - Validar índice de slot equipado
  - Verificar se slot equipado contém item
  - Buscar slot vazio no inventário
  - Se inventário cheio, retornar false
  - Mover item de volta ao inventário
  - Disparar OnItemUnequipped e OnInventoryChanged
  - Retornar true em sucesso
  - _Requirements: 5.4, 5.5_

- [ ] 3.7 Implementar UseEquippedItem()
  - Validar índice de slot equipado
  - Verificar se slot contém item
  - Chamar item.Use() para aplicar efeitos
  - Decrementar quantidade do slot
  - Disparar OnItemUsed e OnInventoryChanged
  - Retornar true em sucesso
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [ ] 3.8 Implementar DiscardItem()
  - Validar índice de slot
  - Chamar Clear() no slot
  - Disparar OnItemRemoved e OnInventoryChanged
  - _Requirements: 6.2, 6.3_

- [ ] 3.9 Implementar UpdateCapacity()
  - Receber EvolutionStage como parâmetro
  - Mapear estágio para capacidade: Filhote=4, Adulto=8, Grande+=12
  - Expandir lista Slots se necessário (preservar itens existentes)
  - Atualizar MaxSlots
  - Disparar OnInventoryChanged
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [ ] 3.10 Implementar GetSaveData() e LoadSaveData()
  - GetSaveData(): Serializar todos slots e slots equipados em InventorySaveData
  - LoadSaveData(): Deserializar dados, buscar ItemData por itemID, restaurar slots
  - Validar dados corrompidos e aplicar valores padrão se necessário
  - _Requirements: 10.1, 10.2, 10.3, 10.5_

- [ ] 4. Checkpoint - Testar InventoryManager isoladamente
  - Criar cena de teste temporária
  - Criar 3-5 ItemData de exemplo (poções, comida)
  - Testar AddItem, RemoveItem, EquipItem, UnequipItem, UseEquippedItem
  - Verificar logs de eventos
  - Testar UpdateCapacity com diferentes estágios
  - Testar GetSaveData/LoadSaveData
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 5. Criar UI do Inventário
- [ ] 5.1 Criar prefab InventorySlotUI
  - Criar GameObject com Image (background usando ui_dialogBackground.png)
  - Adicionar Image filho para ícone do item
  - Adicionar TextMeshProUGUI para quantidade (canto inferior direito)
  - Adicionar Image para highlight de seleção (borda amarela, inicialmente desativada)
  - Criar script InventorySlotUI com métodos: SetItem(), SetQuantity(), SetSelected(), Clear()
  - _Requirements: 9.1, 9.2, 9.5_

- [ ] 5.2 Criar InventoryUI MonoBehaviour
  - Criar GameObject InventoryPanel (Canvas filho do CanvasHUD)
  - Adicionar Image de fundo (ui_dialogBackground.png)
  - Criar GridLayoutGroup para slots (4 colunas, spacing adequado)
  - Criar container separado para slots equipados (4 slots horizontais)
  - Adicionar referências serializadas: inventoryPanel, slotsContainer, equippedSlotsContainer, slotPrefab
  - Inicialmente desativar inventoryPanel
  - _Requirements: 9.3, 9.4_

- [ ] 5.3 Implementar OpenInventory() e CloseInventory()
  - OpenInventory(): Ativar inventoryPanel, Time.timeScale = 0, desabilitar Gameplay ActionMap, habilitar Menu ActionMap
  - CloseInventory(): Desativar inventoryPanel, Time.timeScale = 1, habilitar Gameplay ActionMap, desabilitar Menu ActionMap
  - Adicionar sons de abertura/fechamento
  - _Requirements: 1.1, 1.2, 1.5_

- [ ] 5.4 Implementar RefreshUI()
  - Limpar slots existentes (destruir ou retornar ao pool)
  - Instanciar InventorySlotUI para cada slot do InventoryManager
  - Chamar SetItem() e SetQuantity() em cada InventorySlotUI
  - Fazer o mesmo para slots equipados
  - Atualizar contador de capacidade (ex: "4/12")
  - _Requirements: 1.3, 9.5_

- [ ] 5.5 Implementar navegação de slots
  - Criar variáveis: currentSlotIndex, isNavigatingEquipped
  - Implementar NavigateSlots(Vector2 direction) que calcula próximo índice baseado em grid 4 colunas
  - Atualizar highlight visual do slot selecionado
  - Reproduzir som de navegação
  - Permitir navegação entre inventário principal e slots equipados
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5_

- [ ] 5.6 Implementar menu de confirmação
  - Criar GameObject ConfirmationMenu (painel com 3 botões: Equipar, Descartar, Cancelar)
  - Implementar ShowConfirmationMenu(InventorySlot slot) que exibe menu e configura callbacks
  - Para slots equipados, mostrar apenas Desequipar e Cancelar
  - Implementar OnEquipSelected(), OnDiscardSelected(), OnUnequipSelected(), OnCancelSelected()
  - Adicionar sons de confirmação/cancelamento
  - _Requirements: 5.1, 5.3, 5.4, 6.1, 6.4_

- [ ] 5.7 Implementar HUD de slots equipados
  - Criar 4 InventorySlotUI no CanvasHUD (canto inferior direito)
  - Posicionar verticalmente com labels (Q, E, Z, X)
  - Implementar UpdateHUD() que atualiza ícones e quantidades dos slots equipados
  - Subscrever a OnInventoryChanged para atualizar HUD automaticamente
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5_

- [ ] 5.8 Conectar inputs do Menu ActionMap
  - Subscrever Menu.Inventory.performed para OnInventoryToggle()
  - Subscrever Menu.Move.performed para NavigateSlots()
  - Subscrever Menu.Attack.performed para SelectCurrentSlot()
  - Implementar SelectCurrentSlot() que chama ShowConfirmationMenu()
  - _Requirements: 1.1, 1.2, 4.1_

- [ ] 6. Implementar uso rápido de itens equipados
- [ ] 6.1 Conectar inputs de UseItem1-4
  - Subscrever Gameplay.UseItem1.performed para UseEquippedSlot(0)
  - Subscrever Gameplay.UseItem2.performed para UseEquippedSlot(1)
  - Subscrever Gameplay.UseItem3.performed para UseEquippedSlot(2)
  - Subscrever Gameplay.UseItem4.performed para UseEquippedSlot(3)
  - _Requirements: 8.1, 8.2, 8.3, 8.4_

- [ ] 6.2 Implementar UseEquippedSlot(int index)
  - Chamar InventoryManager.Instance.UseEquippedItem(index)
  - Se retornar true, reproduzir som do item e animação visual no HUD
  - Se retornar false (slot vazio), ignorar silenciosamente
  - _Requirements: 8.5_

- [ ] 7. Integrar com sistema de coleta de itens
- [ ] 7.1 Atualizar ItemCollectable para usar InventoryManager
  - Modificar OnTriggerEnter2D para chamar InventoryManager.Instance.AddItem()
  - Se retornar true, destruir GameObject do item
  - Se retornar false (inventário cheio), exibir notificação "Inventário Cheio" e manter item no mundo
  - _Requirements: 2.5_

- [ ] 8. Integrar com SaveManager
- [ ] 8.1 Adicionar InventorySaveData ao SaveData global
  - Modificar SaveData para incluir campo InventorySaveData inventory
  - Atualizar SaveManager.Save() para chamar InventoryManager.Instance.GetSaveData()
  - Atualizar SaveManager.Load() para chamar InventoryManager.Instance.LoadSaveData()
  - _Requirements: 10.1, 10.2, 10.3_

- [ ] 9. Integrar com sistema de evolução
- [ ] 9.1 Conectar evento de evolução
  - Subscrever ao evento de evolução do GameManager/EvolutionManager
  - Chamar InventoryManager.Instance.UpdateCapacity(newStage) quando jogador evolui
  - Testar expansão de 4→8→12 slots
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [ ] 10. Polimento e feedback visual
- [ ] 10.1 Adicionar animações de UI
  - Item adicionado: Fade in + scale bounce (0.3s)
  - Item removido: Fade out + scale shrink (0.2s)
  - Item usado: Flash branco + particle effect (0.15s)
  - Navegação: Smooth lerp do highlight (0.1s)
  - _Requirements: 9.5_

- [ ] 10.2 Adicionar sons de UI
  - Som de abrir inventário (soft whoosh)
  - Som de fechar inventário (soft whoosh reverse)
  - Som de navegação (subtle click)
  - Som de seleção (confirmation beep)
  - Som de equipar (equip sound)
  - Som de descartar (trash sound)
  - _Requirements: 4.4_

- [ ] 11. Checkpoint Final - Testes de integração
  - Testar fluxo completo: coletar item → abrir inventário → equipar → usar
  - Testar empilhamento de itens idênticos
  - Testar inventário cheio
  - Testar descartar itens
  - Testar save/load do inventário
  - Testar expansão de capacidade ao evoluir
  - Testar uso rápido de itens equipados durante gameplay
  - Ensure all tests pass, ask the user if questions arise.

- [ ]* 12. Testes unitários (Opcional)
- [ ]* 12.1 Criar testes para InventoryManager
  - AddItem_WithEmptySlot_AddsSuccessfully()
  - AddItem_WithFullInventory_ReturnsFalse()
  - AddItem_StackableItem_StacksCorrectly()
  - RemoveItem_ExistingItem_RemovesSuccessfully()
  - EquipItem_ValidSlot_EquipsSuccessfully()
  - UnequipItem_EquippedItem_UnequipsSuccessfully()
  - UseEquippedItem_ValidItem_ConsumesAndAppliesEffect()
  - DiscardItem_ValidSlot_RemovesItem()
  - UpdateCapacity_Evolution_ExpandsCorrectly()
  - _Requirements: All_

- [ ]* 12.2 Criar testes para InventorySlot
  - AddToStack_WithSpace_AddsCorrectly()
  - AddToStack_AtMaxStack_ReturnsFalse()
  - RemoveFromStack_ValidAmount_RemovesCorrectly()
  - RemoveFromStack_ToZero_ClearsSlot()
  - _Requirements: 3.1, 3.2, 3.4_

- [ ]* 13. Testes baseados em propriedades (Opcional)
- [ ]* 13.1 Implementar gerador de dados aleatórios
  - Criar classe RandomInventoryGenerator
  - Métodos: GenerateRandomItem(), GenerateRandomInventory(), GenerateRandomEvolutionStage()
  - _Requirements: All_

- [ ]* 13.2 Escrever property tests
  - **Property 1: Inventário toggle preserva estado de gameplay**
  - **Property 2: Capacidade de inventário corresponde ao estágio de evolução**
  - **Property 3: Expansão de capacidade preserva itens**
  - **Property 4: Empilhamento prioriza slots existentes**
  - **Property 5: Slots vazios têm quantidade zero**
  - **Property 6: Navegação move cursor para posição válida**
  - **Property 7: Equipar usa primeiro slot vazio**
  - **Property 8: Descartar remove item completamente**
  - **Property 9: Usar item atualiza quantidade e UI**
  - **Property 10: Save/Load preserva estado completo**
  - Configurar cada teste para rodar 100+ iterações
  - _Requirements: All_

## Notes

- Tarefas marcadas com * são opcionais e focadas em testes
- Checkpoints são momentos para validar que tudo está funcionando antes de prosseguir
- Seguir ordem das tarefas para garantir dependências corretas
- Consultar design.md para detalhes de implementação
- Usar logs condicionais com flag enableLogs para debug
- Todos os logs devem usar UnityEngine.Debug.Log com formato "[InventoryManager] mensagem"
