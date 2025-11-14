# Implementation Plan - Sistema de Inventário

## Tarefas de Implementação

- [x] 1. Criar estruturas de dados base
  - Criar ScriptableObject ItemData com propriedades essenciais (nome, ícone, tipo, stackable, heal amounts, equipment stats, isQuestItem)
  - Criar classe InventorySlot com item, quantity, IsEmpty e CanStack
  - Criar enums ItemType (Consumable, Material, Quest, Equipment) e EquipmentType (Amulet, Ring, Cape)
  - _Requirements: 1.1, 1.2, 2.1_

- [x] 2. Implementar InventoryManager core
  - [x] 2.1 Criar singleton InventoryManager com arrays de slots (20), equipment (3) e quickSlots (4)
    - Implementar padrão Singleton no Awake
    - Inicializar arrays de slots, equipment e quickSlots
    - _Requirements: 1.1, 2.1, 8.1_
  
  - [x] 2.2 Implementar método AddItem com lógica de empilhamento
    - Tentar empilhar em slot existente primeiro (até 99)
    - Se não conseguir empilhar, procurar slot vazio
    - Se inventário cheio, retornar false e disparar evento OnInventoryFull
    - _Requirements: 2.5, 7.1, 8.1_
  
  - [x] 2.3 Implementar métodos RemoveItem, UseItem e DiscardItem
    - RemoveItem: reduzir quantidade ou limpar slot se chegar a zero
    - UseItem: aplicar efeitos de consumível (heal) e remover 1 unidade
    - DiscardItem: verificar se é quest item (bloquear), mostrar confirmação e limpar slot
    - _Requirements: 3.1, 3.2, 3.3, 5.2_
  
  - [x] 2.4 Implementar sistema de equipamentos
    - Método EquipItem: verificar tipo, desequipar item anterior se existir, equipar novo item
    - Método UnequipItem: remover item do slot de equipamento e retornar ao inventário
    - Aplicar buffs de equipamentos ao PlayerController (defesa, velocidade)
    - _Requirements: 6.1, 6.2, 6.3, 6.4_

- [x] 3. Implementar sistema de Quick Slots
  - [x] 3.1 Criar QuickSlotManager com detecção de input dos direcionais
    - Detectar input dos 4 direcionais (Up, Down, Left, Right)
    - Chamar InventoryManager.UseQuickSlot(direction) quando pressionado
    - _Requirements: 4.1, 4.3_
  
  - [x] 3.2 Implementar métodos AssignQuickSlot e UseQuickSlot no InventoryManager
    - AssignQuickSlot: atribuir item a um dos 4 direcionais
    - UseQuickSlot: usar item atribuído, reduzir quantidade, remover se chegar a zero
    - _Requirements: 4.2, 4.5_

- [x] 4. Criar UI do inventário
  - [x] 4.1 Criar prefab InventorySlotUI com Image (ícone) e TextMeshProUGUI (quantidade)
    - Configurar layout com ícone centralizado e texto no canto inferior direito
    - Adicionar componente Button para detecção de cliques
    - _Requirements: 2.2, 2.3_
  
  - [x] 4.2 Criar InventoryUI com grid 5x4 de slots
    - Criar Canvas com painel de inventário
    - Instanciar 20 InventorySlotUI em GridLayoutGroup (5 colunas)
    - Adicionar área de equipamentos com 3 slots (Amuleto, Anel, Capa)
    - Adicionar botão de fechar (X)
    - _Requirements: 1.5, 2.1, 6.5_
  
  - [x] 4.3 Implementar lógica de InventoryUI
    - Método Show: ativar painel e pausar jogo
    - Método Hide: desativar painel e despausar jogo
    - Método RefreshAll: atualizar todos os slots com dados do InventoryManager
    - Método OnSlotClicked: abrir painel de ações (Usar, Atribuir, Descartar)
    - _Requirements: 1.2, 1.3, 1.4, 3.1_
  
  - [x] 4.4 Criar painel de ações de item
    - Criar painel popup com 3 botões: Usar, Atribuir, Descartar
    - Mostrar/ocultar botões baseado no tipo de item (consumível mostra Usar, equipment mostra Equipar)
    - Desabilitar Descartar se for quest item
    - _Requirements: 3.1, 5.1, 5.2_

- [x] 5. Criar Quick Slot HUD
  - [x] 5.1 Criar prefab QuickSlotUI com Image (ícone) e TextMeshProUGUI (quantidade)
    - Layout simples com ícone e contador
    - _Requirements: 4.4_

  - [x] 5.2 Posicionar 4 QuickSlotUI no HUD (canto inferior direito)
    - Organizar em grid 2x2 (↑↓ em cima, ←→ embaixo)
    - Sempre visível durante gameplay
    - _Requirements: 4.4_
  
  - [x] 5.3 Implementar RefreshUI no QuickSlotManager
    - Atualizar ícones e quantidades dos 4 quick slots
    - Limpar slot se item não existir mais
    - _Requirements: 4.5_

- [x] 6. Integrar com PauseMenu
  - Adicionar opção "Inventário" no menu de pausa
  - Ao selecionar, chamar InventoryUI.Show()
  - Permitir voltar ao menu de pausa com botão de voltar (Esc/B)
  - _Requirements: 1.1, 1.2, 1.4_

- [ ]* 7. Implementar sistema de notificações
  - Criar NotificationUI para exibir mensagens temporárias
  - Mostrar "Inventário Cheio!" quando AddItem retorna false
  - Mostrar "Itens de quest não podem ser descartados" ao tentar descartar quest item
  - Criar diálogo de confirmação para descarte de itens
  - _Requirements: 5.2, 7.1, 7.2, 7.3_

- [x] 8. Integrar com Save System
  - [x] 8.1 Criar estruturas InventorySaveData, ItemSaveData
    - InventorySaveData: arrays de items, equipmentIDs, quickSlotIDs
    - ItemSaveData: itemID, quantity, slotIndex
    - _Requirements: 8.5_
  
  - [x] 8.2 Implementar SaveInventory no InventoryManager
    - Serializar todos os slots não-vazios
    - Salvar IDs dos equipamentos e quick slots
    - Chamar SaveManager.Instance.SaveInventory(saveData)
    - _Requirements: 8.5_
  
  - [x] 8.3 Implementar LoadInventory no InventoryManager
    - Carregar dados salvos
    - Recriar slots com itens corretos
    - Restaurar equipamentos e quick slots
    - _Requirements: 8.5_

- [x] 9. Integrar com ItemCollectable
  - Modificar ItemCollectable para chamar InventoryManager.Instance.AddItem ao coletar
  - Se AddItem retornar false (inventário cheio), mostrar notificação e não destruir item
  - Se AddItem retornar true, destruir GameObject do item coletável
  - _Requirements: 7.1, 7.2_

- [x] 10. Integrar consumíveis com PlayerAttributesHandler

  - Modificar ApplyConsumableEffects no InventoryManager para integrar com PlayerAttributesHandler
  - Aplicar healAmount usando PlayerAttributesHandler.Heal() ou método equivalente
  - _Requirements: 3.2_

- [x] 11. Criar Resources/Items folder e itens de exemplo

  - Criar pasta Assets/Resources/Items/ para armazenar ItemData ScriptableObjects
  - Criar 3-5 ItemData de exemplo: poção de cura (consumível), material de crafting, quest item, amuleto (equipamento), anel (equipamento)
  - Criar ou atribuir sprites simples para ícones dos itens
  - Configurar propriedades de cada item (healAmount, defenseBonus, speedBonus, isQuestItem, etc.)
  - _Requirements: Todos_

- [ ]* 12. Adicionar audio e feedback visual
  - Criar AudioManager ou usar existente para tocar SFX
  - Adicionar sons: inventory_open, inventory_close, item_select, item_use, item_equip, item_discard, error
  - Adicionar animações simples: slot selecionado (borda amarela), hover (highlight), confirmação (scale bounce)
  - _Requirements: 3.5, 5.3, 7.5_

- [ ]* 13. Testar fluxo completo
  - Testar adicionar itens até encher inventário (20 slots)
  - Testar empilhamento automático (até 99)
  - Testar usar consumível (reduz quantidade)
  - Testar equipar/desequipar itens e verificar buffs aplicados
  - Testar atribuir e usar quick slots
  - Testar descartar item normal (com confirmação)
  - Testar tentar descartar quest item (bloqueado)
  - Testar save/load do inventário
  - _Requirements: Todos_
