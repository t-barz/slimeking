# Requirements Document - Sistema de NPCs

## Introduction

Este documento define os requisitos para o sistema de NPCs (Non-Playable Characters) do jogo The Slime King. O sistema permitirá a criação e execução de personagens não jogáveis com comportamentos complexos gerenciados pelo Behavior Graph da Unity 6.2, incluindo movimentação, combate, interações e sistema de relacionamento com o jogador.

## Glossary

- **NPC System**: Sistema completo de gerenciamento de personagens não jogáveis
- **Behavior Graph**: Sistema visual de comportamento da Unity 6.2 que orquestra estados e transições
- **NPC Controller**: Componente que controla o comportamento específico de um NPC
- **NPC Attributes Handler**: Componente que gerencia atributos dinâmicos do NPC (Health, Defense, Speed, Attack)
- **NPC Category**: Categoria de NPC que define qual Behavior Graph será utilizado (Enemy, Friendly, Neutral, Boss)
- **Movement Pattern**: Padrão de movimentação do NPC (Idle, PatrolPoints, CircularPatrol, ChaseTarget)
- **Relationship Points**: Sistema de pontos que determina a relação do NPC com o jogador
- **Drop System**: Sistema de geração de itens quando o NPC é derrotado
- **Quest System**: Sistema de missões que NPCs podem ativar ou completar
- **Interaction System**: Sistema que permite ao jogador interagir com NPCs

## Requirements

### Requirement 1

**User Story:** Como desenvolvedor, eu quero criar NPCs com atributos configuráveis no editor, para que cada NPC possa ter características únicas de combate e movimento.

#### Acceptance Criteria

1. WHEN o desenvolvedor adiciona o componente NPCAttributesHandler a um GameObject, THE NPC System SHALL inicializar os atributos base (Health, Defense, Speed, Attack) com valores padrão configuráveis no Inspector
2. WHEN os atributos do NPC são modificados durante o gameplay, THE NPC System SHALL disparar eventos notificando a mudança de estado
3. WHEN o Health do NPC chega a zero, THE NPC System SHALL disparar o evento OnNPCDied e executar a lógica de derrota
4. THE NPC System SHALL permitir valores inteiros para Defense (não porcentagem) conforme especificado na documentação
5. THE NPC System SHALL expor propriedades públicas para leitura dos atributos atuais (CurrentHealth, CurrentDefense, CurrentSpeed, CurrentAttack)

### Requirement 2

**User Story:** Como desenvolvedor, eu quero que NPCs tenham diferentes padrões de movimentação configuráveis, para criar comportamentos variados sem modificar código.

#### Acceptance Criteria

1. WHEN o desenvolvedor seleciona o padrão "Idle" no Inspector, THE NPC Controller SHALL manter o NPC parado na posição atual
2. WHEN o desenvolvedor seleciona o padrão "PatrolPoints" e define uma lista de Transform points, THE NPC Controller SHALL movimentar o NPC sequencialmente entre os pontos definidos
3. WHEN o desenvolvedor seleciona o padrão "CircularPatrol" e define centro e raio, THE NPC Controller SHALL movimentar o NPC em patrulha circular dentro da área especificada
4. WHEN o NPC detecta um alvo válido e está em modo "ChaseTarget", THE NPC Controller SHALL perseguir o alvo utilizando a velocidade configurada nos atributos
5. THE NPC Controller SHALL integrar com o Behavior Graph para executar transições de estado baseadas no padrão de movimentação

### Requirement 3

**User Story:** Como desenvolvedor, eu quero implementar um sistema de relacionamento entre NPCs e o jogador, para que as interações sejam influenciadas pela relação estabelecida.

#### Acceptance Criteria

1. WHEN o desenvolvedor configura Relationship Points no Inspector, THE NPC System SHALL armazenar o valor inicial de relacionamento
2. WHEN os Relationship Points são maiores que 10, THE NPC System SHALL considerar o NPC como amigável ao jogador
3. WHEN os Relationship Points são menores que 0, THE NPC System SHALL considerar o NPC como hostil ao jogador
4. WHEN os Relationship Points mudam durante o gameplay, THE NPC System SHALL disparar evento OnRelationshipChanged com o novo valor
5. THE NPC System SHALL permitir que o Behavior Graph acesse os Relationship Points para tomar decisões de comportamento

### Requirement 4

**User Story:** Como desenvolvedor, eu quero que NPCs possam interagir com o jogador através de diálogos e entregas de itens, para criar experiências de gameplay ricas.

#### Acceptance Criteria

1. WHEN o jogador pressiona a tecla de interação próximo a um NPC, THE NPC System SHALL verificar se há interação disponível baseada nos Relationship Points
2. WHEN uma interação de entrega de item é configurada no Inspector, THE NPC System SHALL transferir o item especificado para o inventário do jogador
3. WHEN uma interação de ativação de quest é configurada, THE NPC System SHALL disparar evento para o Quest System com os dados da missão
4. WHEN o Behavior Graph solicita execução de interação, THE NPC System SHALL processar a interação e notificar o resultado
5. THE NPC System SHALL permitir configurar múltiplas interações possíveis por NPC no Inspector

### Requirement 5

**User Story:** Como desenvolvedor, eu quero que NPCs derrotados gerem drops de itens configuráveis, para recompensar o jogador por combates.

#### Acceptance Criteria

1. WHEN o desenvolvedor configura uma lista de drops no Inspector, THE NPC System SHALL armazenar os dados de itens e probabilidades
2. WHEN o NPC é derrotado (Health chega a zero), THE NPC System SHALL processar a lista de drops e gerar itens baseados nas probabilidades configuradas
3. WHEN um item é selecionado para drop, THE NPC System SHALL instanciar o prefab do item na posição do NPC
4. THE NPC System SHALL permitir configurar múltiplos itens com diferentes probabilidades de drop
5. THE NPC System SHALL integrar com o sistema de itens coletáveis existente do jogo

### Requirement 6

**User Story:** Como desenvolvedor, eu quero que NPCs possam atacar o jogador quando hostis, para criar desafios de combate.

#### Acceptance Criteria

1. WHEN o NPC está em modo de ataque e detecta o jogador dentro do attack range, THE NPC Controller SHALL executar a animação de ataque
2. WHEN a animação de ataque atinge o frame de hit, THE NPC Controller SHALL aplicar dano ao jogador baseado no atributo CurrentAttack
3. WHEN o ataque é executado, THE NPC Controller SHALL reproduzir efeitos visuais e sonoros configurados
4. WHEN o NPC está em cooldown de ataque, THE NPC Controller SHALL aguardar o tempo configurado antes de permitir novo ataque
5. THE NPC Controller SHALL utilizar LayerMask configurável para detectar alvos válidos de ataque

### Requirement 7

**User Story:** Como desenvolvedor, eu quero que NPCs sigam as boas práticas de arquitetura do projeto, para manter consistência e facilitar manutenção.

#### Acceptance Criteria

1. THE NPC System SHALL utilizar o padrão Controller para NPCController conforme definido em BoasPraticas.md
2. THE NPC System SHALL utilizar o padrão Handler para NPCAttributesHandler conforme definido em BoasPraticas.md
3. THE NPC System SHALL implementar sistema de eventos para comunicação desacoplada entre componentes
4. THE NPC System SHALL utilizar regiões para organizar código em seções lógicas
5. THE NPC System SHALL incluir opções de debug logs e gizmos configuráveis no Inspector
6. THE NPC System SHALL utilizar camelCase para variáveis privadas e PascalCase para classes
7. THE NPC System SHALL incluir comentários em português para documentação do código
8. THE NPC System SHALL utilizar o novo Input System da Unity para detecção de interações

### Requirement 8

**User Story:** Como desenvolvedor, eu quero que NPCs sejam organizados em categorias com Behavior Graphs específicos, para facilitar a criação de diferentes tipos de comportamento.

#### Acceptance Criteria

1. WHEN o desenvolvedor configura a categoria do NPC no Inspector, THE NPC System SHALL carregar o Behavior Graph correspondente à categoria
2. THE NPC System SHALL suportar as categorias: Enemy, Friendly, Neutral e Boss
3. WHEN a categoria é "Enemy", THE NPC Controller SHALL utilizar o Behavior Graph configurado para comportamento hostil
4. WHEN a categoria é "Friendly", THE NPC Controller SHALL utilizar o Behavior Graph configurado para comportamento amigável
5. WHEN a categoria é "Neutral", THE NPC Controller SHALL utilizar o Behavior Graph configurado para comportamento neutro
6. WHEN a categoria é "Boss", THE NPC Controller SHALL utilizar o Behavior Graph configurado para comportamento de chefe
7. THE NPC System SHALL permitir que cada categoria tenha seu próprio conjunto de estados e transições no Behavior Graph

### Requirement 9

**User Story:** Como desenvolvedor, eu quero visualizar informações de debug dos NPCs no Scene View, para facilitar desenvolvimento e troubleshooting.

#### Acceptance Criteria

1. WHEN enableDebugGizmos está ativo, THE NPC System SHALL desenhar o attack range como esfera wireframe no Scene View
2. WHEN enableDebugGizmos está ativo e o padrão é PatrolPoints, THE NPC System SHALL desenhar linhas conectando os pontos de patrulha
3. WHEN enableDebugGizmos está ativo e o padrão é CircularPatrol, THE NPC System SHALL desenhar o círculo de patrulha no Scene View
4. WHEN enableDebugGizmos está ativo, THE NPC System SHALL exibir informações textuais (Health, State, Relationship Points) acima do NPC
5. WHEN enableLogs está ativo, THE NPC System SHALL registrar eventos importantes no Console (mudanças de estado, ataques, interações)
