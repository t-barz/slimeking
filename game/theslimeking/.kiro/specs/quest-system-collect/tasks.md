# Implementation Plan - Quest System (Collect Type)

## Overview

Este plano de implementação detalha as tarefas necessárias para desenvolver o Quest System básico (tipo Collect) para The Slime King. O sistema será implementado de forma incremental, começando pelas estruturas de dados, depois a lógica core, e finalmente as integrações e UI.

## Tasks

- [x] 1. Criar estruturas de dados e ScriptableObjects

  - Criar classe `ItemReward` para representar recompensas
  - Criar ScriptableObject `CollectQuestData` com validação automática
  - Criar classe `QuestProgress` para rastrear progresso
  - Criar classe `QuestSaveData` para serialização
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5_

- [x] 2. Implementar sistema de eventos

  - Criar classe estática `QuestEvents` com todos eventos necessários
  - Implementar métodos helper para disparar eventos com null-conditional operator
  - Documentar quando cada evento deve ser disparado
  - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5_

- [x] 3. Implementar QuestManager (core logic)

- [x] 3.1 Criar estrutura base do QuestManager

  - Implementar Singleton pattern corretamente
  - Criar regiões organizadas (Singleton, Inspector Variables, Private Variables, etc)
  - Implementar Initialize() e lifecycle methods
  - Adicionar opções de debug (enableDebugLogs, showGizmos)
  - _Requirements: 6.1, 6.2, 7.5_

- [x] 3.2 Implementar gerenciamento de quests ativas

  - Implementar método `AcceptQuest(CollectQuestData quest)`
  - Implementar método `CanAcceptQuest(CollectQuestData quest)` com validação de requisitos
  - Implementar método `IsQuestActive(string questID)`
  - Implementar método `GetQuestProgress(string questID)`
  - Adicionar quest à lista ativa e Dictionary cache
  - Disparar evento `QuestEvents.OnQuestAccepted`
  - _Requirements: 2.1, 2.2, 2.3, 2.4, 2.5, 8.1_

- [x] 3.3 Implementar rastreamento de progresso

  - Implementar método `OnItemCollected(ItemData item, int quantity)`
  - Implementar método `UpdateQuestProgress(string questID, int newProgress)`
  - Implementar método `CheckQuestCompletion(QuestProgress progress)`
  - Verificar se item coletado pertence a quest ativa
  - Atualizar progresso sem ultrapassar target
  - Disparar evento `QuestEvents.OnQuestProgressChanged`
  - Marcar quest como pronta quando completa
  - Disparar evento `QuestEvents.OnQuestReadyToTurnIn`
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5_

- [x] 3.4 Implementar entrega de quests

  - Implementar método `TurnInQuest(string questID)`
  - Implementar método `IsQuestReadyToTurnIn(string questID)`
  - Implementar método `GiveRewards(CollectQuestData quest)`
  - Remover itens coletados do inventário
  - Adicionar recompensas ao inventário
  - Adicionar reputação via GameManager
  - Mover quest para lista de completadas
  - Disparar eventos `QuestEvents.OnQuestCompleted` e `QuestEvents.OnQuestTurnedIn`
  - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7_

- [x] 3.5 Implementar validações e error handling

  - Implementar método `ValidateQuestID(string questID)` para evitar duplicatas
  - Implementar método `FindQuestDataByID(string questID)` com cache
  - Validar se inventário tem espaço para recompensas
  - Adicionar logs de erro para casos inválidos
  - _Requirements: 7.1, 7.2, 7.3, 7.4, 7.5_

- [x] 4. Implementar QuestGiverController

- [x] 4.1 Criar estrutura base do QuestGiverController

  - Criar regiões organizadas
  - Adicionar Inspector Variables (availableQuests, indicators, debug)
  - Implementar lifecycle methods (Start, OnEnable, OnDisable)
  - Adicionar opções de debug e gizmos
  - _Requirements: 2.1, 7.5_

- [x] 4.2 Implementar registro e integração com QuestManager

  - Implementar método `RegisterWithQuestManager()`
  - Implementar método `GetAvailableQuests()`
  - Implementar método `RegisterQuestGiver()` no QuestManager
  - Adicionar QuestGiver ao cache do QuestManager
  - _Requirements: 2.1, 8.1_

- [x] 4.3 Implementar sistema de indicadores visuais
  - Implementar método `UpdateIndicators()`
  - Implementar método `HasQuestAvailable()`
  - Implementar método `HasQuestReadyToTurnIn()`
  - Ativar/desativar indicadores baseado em estado
  - Escutar eventos de quest para atualizar indicadores
  - _Requirements: 3.4, 8.1_

- [x] 4.4 Implementar Gizmos para visualização no Editor
  - Desenhar esfera amarela indicando Quest Giver
  - Adicionar opção showGizmos para desativar
  - _Requirements: 7.5_

- [x] 5. Implementar QuestNotificationController

- [x] 5.1 Criar estrutura base do QuestNotificationController

  - Criar regiões organizadas
  - Adicionar Inspector Variables (UI references, audio lists, debug)
  - Implementar lifecycle methods e event subscription
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 5.2 Implementar sistema de notificações

  - Implementar método `ShowObjectiveComplete(string questName)`
  - Implementar método `ShowQuestReadyToTurnIn(string questName)`
  - Implementar método `ShowQuestCompleted(string questName, List<ItemReward> rewards)`
  - Implementar coroutine `ShowNotificationCoroutine()`
  - Implementar método `GetRewardText()` para formatar recompensas
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 5.3 Implementar sistema de áudio aleatório

  - Implementar método `GetRandomSound(List<AudioClip> sounds)`
  - Integrar com AudioManager para reproduzir sons
  - Usar listas de sons para evitar repetição
  - _Requirements: 8.2_

- [x] 6. Implementar integração com Inventory System
  - Escutar evento `InventoryManager.OnInventoryChanged` no QuestManager
  - Implementar lógica de rastreamento automático de itens
  - Verificar se item pertence a quest ativa
  - Atualizar progresso quando item é coletado
  - _Requirements: 3.1, 3.2_

- [x] 7. Implementar integração com Save System

- [x] 7.1 Implementar serialização de quests

  - Implementar método `SaveQuestData()` no QuestManager
  - Serializar quests ativas com progresso
  - Serializar lista de quests completadas
  - Escutar evento `SaveEvents.OnGameSaving`
  - _Requirements: 5.1, 5.2_

- [x] 7.2 Implementar deserialização de quests

  - Implementar método `LoadQuestData(QuestSaveData saveData)` no QuestManager
  - Restaurar quests ativas com progresso correto
  - Restaurar lista de quests completadas
  - Buscar QuestData original por ID
  - Escutar evento `SaveEvents.OnGameLoading`
  - _Requirements: 5.3, 5.4, 5.5_

- [x] 8. Implementar integração com Dialogue System

  - Modificar NPCDialogueInteraction para verificar QuestGiverController no mesmo GameObject
  - Adicionar lógica para verificar se há quest disponível antes de iniciar diálogo
  - Modificar DialogueManager ou criar DialogueChoiceHandler para adicionar opções dinâmicas
  - Adicionar opção "Aceitar Quest" quando quest disponível
  - Adicionar opção "Entregar Quest" quando quest pronta
  - Chamar métodos do QuestManager ao selecionar opções
  - _Requirements: 2.1, 2.2, 2.3, 4.1_
-

- [x] 9. Criar ferramentas de Editor

- [x] 9.1 Criar Custom Inspector para QuestManager

  - Criar classe `QuestManagerEditor` com CustomEditor attribute
  - Adicionar seção "Debug Tools" no Inspector
  - Implementar botão "Clear All Quests"
  - Listar quests ativas com botões "Complete" e "Reset"
  - _Requirements: 6.1, 6.2, 6.3, 6.4_

- [x] 9.2 Criar Property Drawer para ItemReward

  - Criar classe `ItemRewardDrawer` com CustomPropertyDrawer attribute
  - Dividir em duas colunas (Item 70%, Quantity 30%)
  - Melhorar visualização no Inspector
  - _Requirements: 1.5_

- [x] 9.3 Criar Menu Item para criação rápida de quests

  - Criar classe `QuestCreationTool` com MenuItem attribute
  - Adicionar opção "Tools/Quest System/Create Collect Quest"
  - Abrir SaveFilePanelInProject para escolher localização
  - Criar e selecionar novo CollectQuestData
  - _Requirements: 1.1_
-

- [x] 10. Criar prefabs e assets de UI

  - Criar prefab `QuestNotificationPanel` com Canvas e TextMeshPro
  - Criar sprites para indicadores (! amarelo e ! dourado)
  - Configurar animações de bounce para indicadores
  - Adicionar AudioClips de exemplo para notificações
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [x] 11. Implementar debug methods no QuestManager

  - Implementar método `ForceCompleteQuest(string questID)` para marcar quest como completa instantaneamente
  - Implementar método `ResetQuest(string questID)` para resetar progresso de quest ativa
  - Implementar método `ClearAllQuests()` para limpar todas quests ativas e completadas
  - Implementar método `DebugLogQuestState()` para logar estado atual de todas quests
  - Adicionar logs detalhados quando enableDebugLogs está ativo
  - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5_

- [x] 12. Implementar sistema de reputação no GameManager

  - Adicionar campo privado `reputation` no GameManager
  - Implementar método `AddReputation(int amount)` no GameManager
  - Implementar método `GetReputation()` no GameManager
  - Implementar evento `OnReputationChanged` no GameManager
  - Atualizar QuestManager para integrar com GameManager.AddReputation()
  - Atualizar QuestManager.CanAcceptQuest() para verificar minimumReputation
  - _Requirements: 4.4, 7.1, 7.2_

- [x] 13. Criar cena de teste do Quest System

  - Criar cena "QuestSystemTest" em Assets/Game/Scenes/Tests/
  - Adicionar QuestManager GameObject à cena
  - Criar CollectQuestData de teste via menu contextual
  - Configurar quest com item válido do inventário e recompensas
  - Criar NPC de teste com QuestGiverController e NPCDialogueInteraction
  - Criar indicadores visuais (sprites ! amarelo e ! dourado)
  - Adicionar QuestNotificationController à cena
  - Configurar player com InventoryManager para testar coleta
  - _Requirements: Todos_

- [x] 14. Testar fluxo completo do sistema

  - Testar aceitar quest via diálogo
  - Testar rastreamento automático ao coletar item no inventário
  - Testar indicadores visuais no NPC (disponível vs pronta)
  - Testar notificações de progresso e conclusão
  - Testar entrega de quest e recebimento de recompensas
  - Testar remoção de itens do inventário ao entregar
  - Testar quest repetível
  - Testar requisitos de quest (reputação, prerequisite)
  - Testar save/load com quest ativa
  - Testar debug tools no Inspector do QuestManager
  - Validar que todos eventos são disparados corretamente
  - _Requirements: Todos_
