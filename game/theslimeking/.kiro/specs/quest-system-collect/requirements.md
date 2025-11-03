# Requirements Document - Quest System (Collect Type)

## Introduction

O Quest System é um sistema fundamental para The Slime King que permite criar missões orgânicas e emergentes onde NPCs apresentam problemas reais que o jogador pode escolher resolver. Esta primeira implementação foca no tipo de quest mais básico: **Collect** (Coletar itens).

O sistema deve ser fácil de configurar no Editor Unity, permitindo que designers criem quests rapidamente através de ScriptableObjects e ferramentas visuais. O sistema deve integrar-se perfeitamente com os sistemas existentes de Inventário, Diálogo e Save/Load.

## Glossary

- **Quest System**: Sistema completo de gerenciamento de missões do jogo
- **Quest**: Uma missão individual com objetivos específicos e recompensas
- **Quest Objective**: Um objetivo específico dentro de uma quest (ex: coletar 5 flores)
- **Quest Giver**: NPC que oferece a quest ao jogador
- **Quest Manager**: Singleton que gerencia todas as quests ativas e completadas
- **Quest Data**: ScriptableObject que define os dados de uma quest
- **Collect Quest**: Tipo de quest onde o jogador deve coletar X quantidade de um item Y
- **Reputation**: Sistema de pontos invisíveis que aumenta conforme jogador completa quests
- **Item Reward**: Recompensa em forma de item ao completar quest (inclui cristais elementais, consumíveis, materiais, etc)

## Requirements

### Requirement 1

**User Story:** Como designer de conteúdo, quero criar quests de coleta facilmente no Editor Unity, para que eu possa adicionar missões ao jogo sem programar.

#### Acceptance Criteria

1. WHEN o designer clica com botão direito na pasta Project, THE Quest System SHALL exibir opção "Create > Quest System > Collect Quest" no menu contextual
2. WHEN o designer cria um novo Collect Quest ScriptableObject, THE Quest System SHALL gerar arquivo com campos pré-configurados (questID, questName, description, itemToCollect, quantityRequired, rewards)
3. WHEN o designer preenche os campos do Quest ScriptableObject, THE Quest System SHALL validar automaticamente se itemToCollect existe no sistema de inventário
4. WHEN o designer salva o Quest ScriptableObject, THE Quest System SHALL atribuir automaticamente um questID único se não fornecido
5. WHERE o designer configura recompensas, THE Quest System SHALL permitir adicionar múltiplos itens como recompensa (todos tratados como ItemData do sistema de inventário)

### Requirement 2

**User Story:** Como jogador, quero receber quests de NPCs através de diálogos naturais, para que a experiência seja orgânica e imersiva.

#### Acceptance Criteria

1. WHEN o jogador interage com um Quest Giver NPC, THE Quest System SHALL verificar se há quests disponíveis para aquele NPC
2. IF o NPC tem quest disponível e jogador atende requisitos, THEN THE Quest System SHALL exibir opção de diálogo "Aceitar Quest" no sistema de diálogo
3. WHEN o jogador aceita uma quest, THE Quest System SHALL adicionar quest à lista de quests ativas do jogador
4. WHEN uma quest é aceita, THE Quest System SHALL disparar evento QuestEvents.OnQuestAccepted com dados da quest
5. WHEN o jogador já completou uma quest não-repetível, THE Quest System SHALL ocultar opção de aceitar quest novamente

### Requirement 3

**User Story:** Como jogador, quero que o sistema rastreie meu progresso nas quests automaticamente, para que eu possa completar objetivos naturalmente durante a exploração.

#### Acceptance Criteria

1. WHEN o jogador coleta um item relacionado a quest ativa, THE Quest System SHALL atualizar contador de progresso automaticamente
2. WHEN o progresso de uma quest muda, THE Quest System SHALL disparar evento QuestEvents.OnQuestProgressChanged com questID e progresso atual
3. WHEN o jogador completa todos objetivos de uma quest, THE Quest System SHALL marcar quest como "Pronta para Entregar"
4. WHEN quest está pronta para entregar, THE Quest System SHALL exibir indicador visual acima do Quest Giver NPC (ícone de exclamação dourado)
5. WHERE jogador tem múltiplas quests ativas, THE Quest System SHALL rastrear progresso de todas simultaneamente

### Requirement 4

**User Story:** Como jogador, quero entregar quests completadas aos NPCs e receber recompensas, para que eu sinta progressão e recompensa pelo esforço.

#### Acceptance Criteria

1. WHEN o jogador retorna ao Quest Giver NPC com quest completada, THE Quest System SHALL exibir opção de diálogo "Entregar Quest"
2. WHEN o jogador entrega quest, THE Quest System SHALL remover itens coletados do inventário do jogador
3. WHEN quest é entregue, THE Quest System SHALL adicionar recompensas ao inventário do jogador (todos itens via sistema de inventário)
4. WHEN quest é entregue, THE Quest System SHALL aumentar reputação do jogador conforme configurado na quest
5. WHEN quest é entregada, THE Quest System SHALL disparar evento QuestEvents.OnQuestCompleted com dados da quest e recompensas
6. WHEN quest é entregue, THE Quest System SHALL mover quest para lista de "Quests Completadas"
7. IF quest é repetível, THEN THE Quest System SHALL permitir aceitar quest novamente após entrega

### Requirement 5

**User Story:** Como desenvolvedor, quero que o Quest System se integre com o Save System, para que progresso de quests seja salvo automaticamente.

#### Acceptance Criteria

1. WHEN o jogo salva, THE Quest System SHALL serializar todas quests ativas com progresso atual
2. WHEN o jogo salva, THE Quest System SHALL serializar lista de quests completadas
3. WHEN o jogo carrega, THE Quest System SHALL restaurar quests ativas com progresso correto
4. WHEN o jogo carrega, THE Quest System SHALL restaurar lista de quests completadas
5. WHEN quest é completada, THE Quest System SHALL disparar auto-save se configurado

### Requirement 6

**User Story:** Como desenvolvedor, quero ferramentas de debug no Editor para testar quests, para que eu possa validar comportamento sem jogar o jogo completo.

#### Acceptance Criteria

1. WHERE Quest Manager está no Inspector, THE Quest System SHALL exibir seção "Debug Tools" com opções de teste
2. WHEN desenvolvedor clica "Force Complete Quest" no Inspector, THE Quest System SHALL completar quest selecionada instantaneamente
3. WHEN desenvolvedor clica "Reset Quest" no Inspector, THE Quest System SHALL resetar progresso da quest selecionada
4. WHEN desenvolvedor clica "Clear All Quests" no Inspector, THE Quest System SHALL remover todas quests ativas e completadas
5. WHERE modo debug está ativo, THE Quest System SHALL exibir logs detalhados de eventos de quest no Console

### Requirement 7

**User Story:** Como designer, quero configurar requisitos para quests, para que certas quests só apareçam após condições específicas.

#### Acceptance Criteria

1. WHERE Quest ScriptableObject tem campo "Requirements", THE Quest System SHALL permitir configurar requisitos (nível de reputação mínimo, quests anteriores completadas)
2. WHEN NPC verifica quests disponíveis, THE Quest System SHALL filtrar quests baseado em requisitos configurados
3. IF jogador não atende requisitos, THEN THE Quest System SHALL ocultar quest do NPC
4. WHEN requisitos são atendidos, THE Quest System SHALL tornar quest disponível automaticamente
5. WHERE quest tem quest anterior como requisito, THE Quest System SHALL verificar se quest anterior está na lista de completadas

### Requirement 8

**User Story:** Como jogador, quero receber feedback visual e sonoro ao completar objetivos, para que a experiência seja satisfatória e clara.

#### Acceptance Criteria

1. WHEN o jogador completa um objetivo de quest, THE Quest System SHALL exibir notificação visual simples (ex: "Objetivo Completado: Colete 5 Flores")
2. WHEN objetivo é completado, THE Quest System SHALL reproduzir som de feedback positivo
3. WHEN todos objetivos de uma quest são completados, THE Quest System SHALL exibir notificação "Quest Pronta para Entregar"
4. WHEN quest é entregue, THE Quest System SHALL exibir notificação de recompensas recebidas
5. WHERE recompensa inclui múltiplos itens, THE Quest System SHALL listar todos itens recebidos na notificação

### Requirement 9

**User Story:** Como desenvolvedor, quero que o Quest System use eventos para comunicação, para que outros sistemas possam reagir a mudanças de quest sem acoplamento direto.

#### Acceptance Criteria

1. WHEN quest é aceita, THE Quest System SHALL disparar evento estático QuestEvents.OnQuestAccepted(Quest quest)
2. WHEN progresso de quest muda, THE Quest System SHALL disparar evento estático QuestEvents.OnQuestProgressChanged(string questID, int currentProgress, int targetProgress)
3. WHEN quest é completada, THE Quest System SHALL disparar evento estático QuestEvents.OnQuestCompleted(Quest quest, List<Reward> rewards)
4. WHEN quest é entregue, THE Quest System SHALL disparar evento estático QuestEvents.OnQuestTurnedIn(Quest quest)
5. WHEN objetivo individual é completado, THE Quest System SHALL disparar evento estático QuestEvents.OnObjectiveCompleted(string questID, int objectiveIndex)
