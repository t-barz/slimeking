# Requirements Document

## Introduction

The Slime King é um RPG de aventura 2D top-down desenvolvido em Unity 6.3 que possui mecanismos já implementados (status "OK" e "WIP") e 28 mecanismos pendentes de desenvolvimento (status "TO DO") conforme catalogados no arquivo MecanismosMacroAtividades.csv. Estes mecanismos pendentes precisam ser implementados seguindo as especificações do Game Design Document (The-Slime-King-GDD-v3.0-Complete.md) e integrando corretamente com o código Unity existente na pasta Assets.

O objetivo é desenvolver todos os mecanismos pendentes de forma que funcionem harmoniosamente com os sistemas já implementados, mantendo a arquitetura Unity existente e atendendo às especificações de gameplay, controles, interface e experiência cozy definidas no GDD.

## Requirements

### Requirement 1

**User Story:** Como desenvolvedor Unity, eu quero implementar os mecanismos de movimento avançado do Slime (Shrink and Slide, Jump), para que o jogador possa navegar pelo mundo usando as mecânicas de interação especificadas no GDD.

#### Acceptance Criteria

1. WHEN o jogador pressiona o botão de interação próximo a um ponto interativo THEN o sistema SHALL executar a animação de Shrink and Slide e mover o player até o destino
2. WHEN o movimento Shrink and Slide está ativo THEN o sistema SHALL desabilitar colisores do player para evitar travamento no cenário
3. WHEN o jogador interage com pontos de Jump THEN o sistema SHALL executar animação de Jump antes de deslocar o personagem
4. WHEN os movimentos especiais são executados THEN o sistema SHALL integrar com o sistema de Interactive Point já implementado

### Requirement 2

**User Story:** Como desenvolvedor, eu quero implementar o sistema de coleta e diálogo, para que o jogador possa interagir com itens e NPCs conforme especificado no GDD.

#### Acceptance Criteria

1. WHEN o jogador interage com itens coletáveis THEN o sistema SHALL atualizar o inventário e disparar VFX de coleta
2. WHEN o jogador interage com NPCs THEN o sistema SHALL iniciar diálogo com caixa de texto e opções de resposta
3. WHEN itens são coletados THEN o sistema SHALL integrar com o sistema de Auto collect já implementado
4. WHEN diálogos são iniciados THEN o sistema SHALL pausar outras interações e focar na conversa

### Requirement 3

**User Story:** Como desenvolvedor de gameplay, eu quero implementar todos os sistemas de inimigos (status, movimento, perseguição, ataque, hit, morte), para que o combate funcione conforme as especificações do GDD.

#### Acceptance Criteria

1. WHEN inimigos são spawned THEN o sistema SHALL gerenciar vida, atributos e variações conforme Enemy Status
2. WHEN inimigos estão ativos THEN o sistema SHALL controlar patrulha e movimentação no cenário
3. WHEN inimigos detectam o Slime THEN o sistema SHALL ativar perseguição conforme Enemy chasing
4. WHEN inimigos atacam THEN o sistema SHALL gerar ataques com animações e VFX apropriados
5. WHEN inimigos recebem dano THEN o sistema SHALL exibir efeitos e atualizar status
6. WHEN inimigos morrem THEN o sistema SHALL gerenciar morte, efeitos de destruição e dropping de itens
7. WHEN sistemas de inimigos são ativados THEN o sistema SHALL integrar com sistemas de combate já existentes (Attack Hit Effect, Attack Not Hit Effect)

### Requirement 4

**User Story:** Como desenvolvedor de progressão, eu quero implementar o Slime Growth System e Skill Tree, para que o jogador possa evoluir o personagem conforme especificado no GDD.

#### Acceptance Criteria

1. WHEN o Slime absorve cristais elementais THEN o sistema SHALL controlar evolução através dos quatro estágios (Filhote, Adulto, Grande Slime, Rei Slime)
2. WHEN o Slime evolui THEN o sistema SHALL desbloquear habilidades elementais e combinações avançadas
3. WHEN habilidades são desbloqueadas THEN o sistema SHALL permitir gerenciamento no Skill Tree com seleção no menu
4. WHEN o sistema de crescimento é ativo THEN o sistema SHALL integrar com o sistema de atributos definido no GDD (PV, Defesa, Ataque Básico, Ataque Especial, Nível)

### Requirement 5

**User Story:** Como desenvolvedor de sistemas, eu quero implementar Item Usage e Interface, para que o jogador possa usar itens e navegar pelos menus conforme especificado no GDD.

#### Acceptance Criteria

1. WHEN itens consumíveis são usados THEN o sistema SHALL gerenciar uso via botões LB, LT, RB, RT conforme esquema de controle do GDD
2. WHEN itens são consumidos THEN o sistema SHALL aplicar buffs e efeitos especiais apropriados
3. WHEN a interface é acessada THEN o sistema SHALL exibir HUD, menus, status do Slime, inventário e mapas
4. WHEN o sistema de interface é ativo THEN o sistema SHALL manter o estilo cozy com tons suaves, bordas orgânicas e navegação universal

### Requirement 6

**User Story:** Como desenvolvedor de conteúdo, eu quero implementar Minion System e Quest System, para que o jogador possa recrutar aliados e completar missões conforme especificado no GDD.

#### Acceptance Criteria

1. WHEN o jogador recruta aliados THEN o sistema SHALL gerenciar comportamento e habilidades dos minions/seguidores
2. WHEN quests são ativadas THEN o sistema SHALL gerenciar missões principais e secundárias com objetivos, progresso e recompensas
3. WHEN puzzles são integrados THEN o sistema SHALL garantir diversidade de objetivos através de Puzzle Quests
4. WHEN sistemas de minion e quest são ativos THEN o sistema SHALL integrar com o sistema de progressão para Rei Slime (requisito de aliados)

### Requirement 7

**User Story:** Como desenvolvedor de sistemas avançados, eu quero implementar Save System, Difficulty Change, Audio Options e Fishing System, para que o jogo tenha funcionalidades completas conforme especificado no GDD.

#### Acceptance Criteria

1. WHEN o jogador salva o jogo THEN o sistema SHALL gerenciar 3 slots de save manual e 1 slot de autosave
2. WHEN a dificuldade é alterada THEN o sistema SHALL ajustar regras e inimigos em tempo real
3. WHEN opções de áudio são acessadas THEN o sistema SHALL permitir customização de volumes, trilhas e efeitos
4. WHEN o sistema de pesca é usado THEN o sistema SHALL implementar minigame relaxante com VFX customizados
5. WHEN sistemas avançados são implementados THEN o sistema SHALL manter compatibilidade com Steam Deck conforme especificado

### Requirement 8

**User Story:** Como desenvolvedor de funcionalidades futuras, eu quero implementar sistemas de versões posteriores (Local Multiplayer, Harvest System, Base Customize, Rewards, Achievements), para que o jogo tenha conteúdo expandido.

#### Acceptance Criteria

1. WHEN multiplayer local é ativado THEN o sistema SHALL permitir split screen/couch co-op com múltiplos jogadores
2. WHEN harvest system é usado THEN o sistema SHALL gerenciar coleta e cultivo de recursos, plantas e minérios
3. WHEN base customize é acessado THEN o sistema SHALL permitir editar/decorar o lar do Slime conforme sistema de expansão do GDD
4. WHEN rewards e achievements são implementados THEN o sistema SHALL gerenciar conquistas com integração Steam e notificações
5. WHEN funcionalidades futuras são desenvolvidas THEN o sistema SHALL manter arquitetura extensível para Remote Multiplayer, Photo Mode e adaptações para outras plataformas
