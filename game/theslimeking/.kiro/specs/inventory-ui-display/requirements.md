# Requirements Document

## Introduction

Este documento especifica os requisitos para implementar a exibição visual de itens no sistema de inventário do jogo The Slime King. O sistema deve exibir itens coletados pelo jogador em uma grade de 12 slots (3 linhas x 4 colunas), onde cada item ocupa exatamente 1 slot, independentemente de ser do mesmo tipo que outros itens. O sistema deve seguir o princípio KISS (Keep It Simple, Stupid) e integrar-se perfeitamente com os sistemas existentes de `InventoryManager`, `InventoryUI` e `PickupItem`.

## Glossary

- **InventoryManager**: Sistema singleton que gerencia o estado do inventário, incluindo 12 slots de armazenamento de itens
- **InventoryUI**: Componente de UI que controla a exibição e ocultação do painel de inventário
- **InventorySlotUI**: Componente de UI que representa visualmente um único slot do inventário
- **ItemData**: ScriptableObject que contém os dados de um item (nome, ícone, tipo, propriedades)
- **InventorySlot**: Classe que representa um slot do inventário contendo um ItemData e quantidade
- **Slot**: Posição individual na grade do inventário que pode conter um item
- **Non-stackable**: Característica de itens que não podem ser empilhados, ocupando sempre 1 slot por unidade
- **Grid Layout**: Organização visual dos slots em formato de grade (3 linhas x 4 colunas = 12 slots)

## Requirements

### Requirement 1

**User Story:** Como jogador, eu quero ver os itens que coletei exibidos visualmente no inventário, para que eu possa identificar rapidamente quais itens possuo.

#### Acceptance Criteria

1. WHEN o jogador abre o inventário THEN o sistema SHALL exibir uma grade de 12 slots organizados em 3 linhas e 4 colunas
2. WHEN um slot contém um item THEN o sistema SHALL exibir o ícone do item no slot correspondente
3. WHEN um slot está vazio THEN o sistema SHALL exibir o slot sem ícone
4. WHEN o inventário é aberto THEN o sistema SHALL sincronizar automaticamente a exibição com o estado atual do InventoryManager
5. WHEN o InventoryManager dispara o evento OnInventoryChanged THEN o sistema SHALL atualizar a exibição de todos os slots afetados

### Requirement 2

**User Story:** Como jogador, eu quero que cada item coletado ocupe exatamente 1 slot no inventário, para que eu possa gerenciar meu espaço de inventário de forma clara e previsível.

#### Acceptance Criteria

1. WHEN o jogador coleta um item THEN o sistema SHALL adicionar o item ao primeiro slot vazio disponível
2. WHEN o jogador coleta múltiplos itens do mesmo tipo THEN o sistema SHALL colocar cada item em um slot separado
3. WHEN o inventário está cheio (12 slots ocupados) THEN o sistema SHALL impedir a coleta de novos itens
4. WHEN um item é removido do inventário THEN o sistema SHALL liberar o slot correspondente para novos itens
5. THE InventoryManager SHALL tratar todos os itens como não empilháveis (isStackable = false) para este sistema

### Requirement 3

**User Story:** Como jogador, eu quero que a interface do inventário responda imediatamente às mudanças no meu inventário, para que eu tenha feedback visual instantâneo das minhas ações.

#### Acceptance Criteria

1. WHEN um item é adicionado ao inventário THEN o sistema SHALL exibir o ícone do item no slot correspondente em menos de 0.1 segundos
2. WHEN um item é removido do inventário THEN o sistema SHALL limpar o ícone do slot correspondente imediatamente
3. WHEN o inventário é aberto THEN o sistema SHALL exibir o estado atual sem atrasos perceptíveis
4. WHEN múltiplos itens são adicionados em sequência THEN o sistema SHALL atualizar cada slot individualmente sem conflitos visuais
5. THE sistema SHALL utilizar o sistema de eventos do InventoryManager para sincronização automática

### Requirement 4

**User Story:** Como desenvolvedor, eu quero que o sistema de exibição do inventário seja simples e fácil de manter, para que futuras modificações sejam diretas e sem complexidade desnecessária.

#### Acceptance Criteria

1. THE sistema SHALL reutilizar os componentes existentes (InventoryManager, InventoryUI, InventorySlotUI) sem duplicação de código
2. THE sistema SHALL seguir o padrão de eventos para comunicação entre componentes
3. THE sistema SHALL utilizar o sistema de logs condicional definido nas boas práticas do projeto
4. THE sistema SHALL implementar apenas a funcionalidade necessária sem over-engineering
5. THE código SHALL seguir as convenções de nomenclatura e organização definidas em BoasPraticas.md
