# Requirements Document - Sistema de Inventário

## Introduction

O Sistema de Inventário é um componente essencial do The Slime King que permite ao jogador coletar, armazenar, gerenciar e usar itens consumíveis durante sua jornada. O sistema deve ser intuitivo, responsivo e integrado com o sistema de evolução do slime, expandindo a capacidade de armazenamento conforme o jogador progride.

## Glossary

- **Inventory (Inventário)**: Sistema de armazenamento de itens do jogador
- **Slot**: Espaço individual para armazenar um item ou pilha de itens
- **Stack (Pilha)**: Múltiplas unidades do mesmo item ocupando um único slot
- **Equipped Slot (Slot Equipado)**: Slot especial para itens prontos para uso rápido
- **HUD**: Heads-Up Display - interface sempre visível durante o gameplay
- **Evolution Stage (Estágio de Evolução)**: Fase atual do slime (Filhote, Adulto, Grande, Rei, Transcendente)
- **Consumable Item (Item Consumível)**: Item que pode ser usado e é removido do inventário após uso
- **InputSystem**: Sistema de entrada do Unity para capturar comandos do jogador
- **Canvas**: Elemento de UI do Unity que contém elementos de interface

## Requirements

### Requirement 1: Abertura e Fechamento do Inventário

**User Story:** Como jogador, quero abrir e fechar o inventário facilmente, para que eu possa gerenciar meus itens sem interromper o fluxo do jogo.

#### Acceptance Criteria

1. WHEN o jogador pressiona o botão Inventory (Select/I) THEN o sistema SHALL abrir a interface do inventário e pausar o gameplay
2. WHEN o inventário está aberto e o jogador pressiona o botão Inventory novamente THEN o sistema SHALL fechar o inventário e retomar o gameplay
3. WHEN o inventário está aberto THEN o sistema SHALL exibir todos os slots disponíveis, itens armazenados e slots equipados
4. WHEN o inventário abre THEN o sistema SHALL posicionar o cursor no primeiro slot não-vazio ou no primeiro slot disponível
5. WHEN o inventário fecha THEN o sistema SHALL manter o estado dos itens e equipamentos

### Requirement 2: Capacidade de Armazenamento Progressiva

**User Story:** Como jogador, quero que minha capacidade de inventário aumente conforme evoluo, para que eu possa carregar mais itens à medida que progrido no jogo.

#### Acceptance Criteria

1. WHEN o slime está no estágio Filhote THEN o sistema SHALL disponibilizar 4 slots de inventário
2. WHEN o slime evolui para Adulto THEN o sistema SHALL expandir para 8 slots de inventário
3. WHEN o slime evolui para Grande Slime ou estágios superiores THEN o sistema SHALL disponibilizar 12 slots de inventário
4. WHEN a capacidade do inventário aumenta THEN o sistema SHALL preservar todos os itens já armazenados
5. WHEN o jogador tenta coletar um item com inventário cheio THEN o sistema SHALL exibir notificação visual indicando inventário cheio

### Requirement 3: Sistema de Empilhamento de Itens

**User Story:** Como jogador, quero que itens idênticos sejam empilhados automaticamente, para que eu possa otimizar o espaço do inventário.

#### Acceptance Criteria

1. WHEN o jogador coleta um item idêntico a um já existente no inventário THEN o sistema SHALL adicionar à pilha existente ao invés de ocupar novo slot
2. WHEN uma pilha de itens atinge a quantidade máxima (99 unidades) THEN o sistema SHALL criar nova pilha em slot vazio
3. WHEN o jogador usa um item de uma pilha THEN o sistema SHALL decrementar a quantidade em 1 unidade
4. WHEN a quantidade de uma pilha chega a zero THEN o sistema SHALL remover o item do slot e liberar o espaço
5. WHEN o inventário exibe um slot com pilha THEN o sistema SHALL mostrar a quantidade numérica no canto inferior direito do slot

### Requirement 4: Navegação pelo Inventário

**User Story:** Como jogador, quero navegar facilmente pelos slots do inventário usando controles intuitivos, para que eu possa selecionar itens rapidamente.

#### Acceptance Criteria

1. WHEN o inventário está aberto e o jogador usa Move (WASD/Analógico/D-Pad) THEN o sistema SHALL mover o cursor para o slot adjacente na direção correspondente
2. WHEN o cursor está em um slot e não há slot adjacente na direção pressionada THEN o sistema SHALL manter o cursor na posição atual
3. WHEN um slot é selecionado THEN o sistema SHALL exibir destaque visual (borda brilhante ou mudança de cor)
4. WHEN o cursor se move THEN o sistema SHALL reproduzir som de navegação sutil
5. WHEN o jogador navega para os slots equipados THEN o sistema SHALL permitir navegação entre os 4 slots equipados usando Move

### Requirement 5: Equipar e Desequipar Itens

**User Story:** Como jogador, quero equipar itens consumíveis em slots de acesso rápido, para que eu possa usá-los instantaneamente durante o gameplay sem abrir o inventário.

#### Acceptance Criteria

1. WHEN o jogador seleciona um item consumível no inventário e pressiona Attack THEN o sistema SHALL exibir menu de confirmação com opções "Equipar", "Descartar" e "Cancelar"
2. WHEN o jogador escolhe "Equipar" THEN o sistema SHALL mover o item para o primeiro slot equipado vazio disponível
3. WHEN todos os 4 slots equipados estão ocupados e o jogador tenta equipar novo item THEN o sistema SHALL exibir notificação "Slots equipados cheios"
4. WHEN o jogador seleciona um slot equipado e pressiona Attack THEN o sistema SHALL exibir menu com opções "Desequipar" e "Cancelar"
5. WHEN o jogador escolhe "Desequipar" THEN o sistema SHALL retornar o item ao inventário principal no primeiro slot vazio

### Requirement 6: Descartar Itens

**User Story:** Como jogador, quero poder descartar itens indesejados, para que eu possa liberar espaço no inventário para itens mais úteis.

#### Acceptance Criteria

1. WHEN o jogador seleciona um item no inventário e escolhe "Descartar" no menu THEN o sistema SHALL exibir confirmação "Descartar [Nome do Item]? (Sim/Não)"
2. WHEN o jogador confirma o descarte THEN o sistema SHALL remover o item completamente do inventário
3. WHEN o item descartado é uma pilha THEN o sistema SHALL remover toda a pilha de uma vez
4. WHEN o jogador cancela o descarte THEN o sistema SHALL manter o item no inventário e fechar o menu de confirmação
5. WHEN um item é descartado THEN o sistema SHALL reproduzir som de confirmação e animação visual de remoção

### Requirement 7: Exibição de Slots Equipados no HUD

**User Story:** Como jogador, quero ver meus itens equipados constantemente na tela durante o gameplay, para que eu saiba quais itens tenho disponíveis para uso rápido.

#### Acceptance Criteria

1. WHEN o jogo está em modo gameplay (inventário fechado) THEN o sistema SHALL exibir os 4 slots equipados no canto inferior direito do HUD
2. WHEN um slot equipado contém um item THEN o sistema SHALL exibir o ícone do item e sua quantidade
3. WHEN um slot equipado está vazio THEN o sistema SHALL exibir o slot com aparência esmaecida ou vazia
4. WHEN o jogador usa um item equipado THEN o sistema SHALL atualizar a quantidade exibida no HUD imediatamente
5. WHEN a quantidade de um item equipado chega a zero THEN o sistema SHALL esvaziar o slot visualmente no HUD

### Requirement 8: Uso Rápido de Itens Equipados

**User Story:** Como jogador, quero usar itens equipados pressionando botões de ombro/gatilho, para que eu possa consumir itens rapidamente durante combate ou exploração.

#### Acceptance Criteria

1. WHEN o jogador pressiona UseItem1 (L/Q/LeftShoulder) THEN o sistema SHALL consumir o item no slot equipado 1
2. WHEN o jogador pressiona UseItem2 (LT/E/LeftTrigger) THEN o sistema SHALL consumir o item no slot equipado 2
3. WHEN o jogador pressiona UseItem3 (R/Z/RightShoulder) THEN o sistema SHALL consumir o item no slot equipado 3
4. WHEN o jogador pressiona UseItem4 (RT/X/RightTrigger) THEN o sistema SHALL consumir o item no slot equipado 4
5. WHEN um slot equipado está vazio e o jogador pressiona o botão correspondente THEN o sistema SHALL ignorar o input sem efeito

### Requirement 9: Interface Visual do Inventário

**User Story:** Como jogador, quero uma interface de inventário clara e visualmente agradável, para que eu possa entender facilmente o estado dos meus itens.

#### Acceptance Criteria

1. WHEN o inventário é exibido THEN o sistema SHALL usar ui_dialogBackground.png como imagem de fundo para o painel principal
2. WHEN cada slot é renderizado THEN o sistema SHALL usar ui_dialogBackground.png como imagem de fundo do slot
3. WHEN o inventário está aberto THEN o sistema SHALL organizar os slots em grid de 4 colunas
4. WHEN os slots equipados são exibidos THEN o sistema SHALL posicioná-los separadamente abaixo do inventário principal com espaçamento visível
5. WHEN um item é exibido em um slot THEN o sistema SHALL mostrar o ícone do item centralizado e a quantidade no canto inferior direito

### Requirement 10: Persistência de Dados do Inventário

**User Story:** Como jogador, quero que meu inventário seja salvo automaticamente, para que eu não perca meus itens ao fechar o jogo.

#### Acceptance Criteria

1. WHEN o jogador coleta, usa, equipa ou descarta um item THEN o sistema SHALL atualizar os dados de save automaticamente
2. WHEN o jogo é carregado THEN o sistema SHALL restaurar todos os itens do inventário nas posições corretas
3. WHEN o jogo é carregado THEN o sistema SHALL restaurar todos os itens equipados nos slots corretos
4. WHEN o estágio de evolução é carregado THEN o sistema SHALL aplicar a capacidade de inventário correspondente
5. WHEN há inconsistência nos dados salvos THEN o sistema SHALL aplicar valores padrão seguros e registrar erro no log
