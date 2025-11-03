# Requirements Document - Sistema de Inventário

## Introduction

O Sistema de Inventário é um componente fundamental do The Slime King que permite ao jogador gerenciar itens consumíveis, materiais de crafting, itens de quest e equipamentos. O sistema deve ser intuitivo, acessível através do menu de pausa, e permitir acesso rápido a itens através de atalhos (direcionais do gamepad).

## Glossary

- **Inventory System**: Sistema completo de gerenciamento de itens do jogador
- **Item Slot**: Espaço individual no grid do inventário que pode conter um item
- **Quick Slot**: Slot de atalho rápido mapeado aos direcionais do gamepad (4 slots)
- **Stack**: Agrupamento de itens idênticos em um único slot
- **Consumable**: Item que pode ser usado e é removido do inventário
- **Equipment**: Item que pode ser equipado para fornecer buffs passivos
- **Material**: Item usado para crafting
- **Quest Item**: Item necessário para completar quests

## Requirements

### Requirement 1

**User Story:** Como jogador, eu quero acessar meu inventário através do menu de pausa, para que eu possa gerenciar meus itens quando necessário

#### Acceptance Criteria

1. WHEN o jogador pressiona o botão de pausa (Tab/Esc/Menu), THE Inventory System SHALL exibir o menu de pausa com opção de inventário
2. WHEN o jogador seleciona a opção de inventário no menu de pausa, THE Inventory System SHALL abrir a tela de inventário completa
3. WHILE a tela de inventário está aberta, THE Inventory System SHALL pausar o jogo completamente
4. WHEN o jogador pressiona o botão de voltar (Esc/B/Circle), THE Inventory System SHALL fechar o inventário e retornar ao menu de pausa
5. THE Inventory System SHALL exibir todos os itens organizados em um grid visual de 5x4 (20 slots iniciais)

### Requirement 2

**User Story:** Como jogador, eu quero organizar meus itens em um grid visual, para que eu possa ver claramente o que possuo

#### Acceptance Criteria

1. THE Inventory System SHALL exibir um grid de 5 colunas por 4 linhas (20 slots totais)
2. WHEN um slot contém um item, THE Inventory System SHALL exibir o ícone do item, nome e quantidade (se aplicável)
3. WHEN um slot está vazio, THE Inventory System SHALL exibir um fundo semi-transparente indicando disponibilidade
4. THE Inventory System SHALL permitir drag and drop para reorganizar itens entre slots
5. WHEN itens idênticos são coletados, THE Inventory System SHALL empilhá-los automaticamente no mesmo slot (máximo 99 unidades)

### Requirement 3

**User Story:** Como jogador, eu quero usar itens consumíveis diretamente do inventário, para que eu possa curar ou aplicar buffs quando necessário

#### Acceptance Criteria

1. WHEN o jogador seleciona um item consumível no inventário, THE Inventory System SHALL exibir opções "Usar", "Atribuir a Quick Slot" e "Descartar"
2. WHEN o jogador escolhe "Usar" em um item consumível, THE Inventory System SHALL aplicar o efeito do item imediatamente
3. WHEN um item consumível é usado, THE Inventory System SHALL reduzir a quantidade em 1 unidade
4. IF a quantidade de um item chega a zero, THEN THE Inventory System SHALL remover o item do slot
5. THE Inventory System SHALL exibir feedback visual (partículas, som) quando um item é usado

### Requirement 4

**User Story:** Como jogador, eu quero atribuir até 4 itens aos atalhos rápidos (direcionais), para que eu possa usá-los rapidamente durante o gameplay sem pausar

#### Acceptance Criteria

1. THE Inventory System SHALL fornecer 4 Quick Slots mapeados aos direcionais do gamepad (Cima, Baixo, Esquerda, Direita)
2. WHEN o jogador seleciona "Atribuir a Quick Slot" em um item, THE Inventory System SHALL permitir escolher qual dos 4 direcionais usar
3. WHILE no gameplay (fora do menu), WHEN o jogador pressiona um direcional, THE Inventory System SHALL usar o item atribuído àquele direcional
4. THE Inventory System SHALL exibir os 4 Quick Slots no HUD durante o gameplay com ícones dos itens atribuídos
5. WHEN um item em Quick Slot é usado e sua quantidade chega a zero, THE Inventory System SHALL remover o item do Quick Slot automaticamente

### Requirement 5

**User Story:** Como jogador, eu quero descartar itens que não preciso mais, para que eu possa liberar espaço no inventário

#### Acceptance Criteria

1. WHEN o jogador seleciona "Descartar" em um item, THE Inventory System SHALL exibir confirmação "Descartar [Nome do Item]? (Sim/Não)"
2. IF o item é um Quest Item, THEN THE Inventory System SHALL bloquear a ação de descartar e exibir mensagem "Itens de quest não podem ser descartados"
3. WHEN o jogador confirma descarte, THE Inventory System SHALL remover o item do inventário permanentemente
4. WHERE o item está em um Quick Slot, WHEN descartado, THE Inventory System SHALL remover também do Quick Slot
5. THE Inventory System SHALL reproduzir som de confirmação ao descartar item

### Requirement 6

**User Story:** Como jogador, eu quero equipar itens de equipamento para receber buffs passivos, para que eu possa customizar meu slime

#### Acceptance Criteria

1. THE Inventory System SHALL permitir equipar até 3 itens de equipamento simultaneamente (Amuleto, Anel, Capa)
2. WHEN o jogador seleciona "Equipar" em um item de equipamento, THE Inventory System SHALL equipá-lo no slot correspondente
3. IF já existe um item equipado no mesmo slot, THEN THE Inventory System SHALL desequipar o item anterior e retorná-lo ao inventário
4. THE Inventory System SHALL exibir área dedicada mostrando os 3 slots de equipamento e itens equipados
5. WHEN um item é equipado, THE Inventory System SHALL aplicar seus buffs imediatamente ao jogador

### Requirement 7

**User Story:** Como jogador, eu quero ser notificado quando meu inventário está cheio, para que eu saiba que preciso gerenciar espaço

#### Acceptance Criteria

1. WHEN o jogador tenta coletar um item e o inventário está cheio (20/20 slots), THE Inventory System SHALL exibir notificação "Inventário Cheio!"
2. THE Inventory System SHALL oferecer opções: "Abrir Inventário" ou "Descartar Item Coletado"
3. WHEN o jogador escolhe "Abrir Inventário", THE Inventory System SHALL pausar o jogo e abrir a tela de inventário
4. WHEN o jogador escolhe "Descartar Item Coletado", THE Inventory System SHALL remover o item do mundo sem adicioná-lo ao inventário
5. THE Inventory System SHALL reproduzir som de erro quando inventário está cheio

### Requirement 8

**User Story:** Como jogador, eu quero que itens idênticos se empilhem automaticamente, para que eu economize espaço no inventário

#### Acceptance Criteria

1. WHEN o jogador coleta um item que já existe no inventário, THE Inventory System SHALL adicionar à pilha existente
2. THE Inventory System SHALL permitir empilhamento máximo de 99 unidades por slot
3. IF a pilha existente está cheia (99 unidades), THEN THE Inventory System SHALL criar nova pilha em slot vazio
4. THE Inventory System SHALL exibir número de unidades no canto inferior direito do ícone do item
5. WHEN o jogador usa um item de uma pilha, THE Inventory System SHALL reduzir o contador em 1 unidade
