# Requirements Document

## Introduction

Este documento define os requisitos para simplificar o sistema de teletransporte, removendo o pré-carregamento de cenas e as validações de proximidade. O objetivo é criar um sistema de teleporte instantâneo que carrega e posiciona o personagem imediatamente ao colidir com o TeleportPoint, sem necessidade de zonas de proximidade ou pré-carregamento.

## Requirements

### Requirement 1: Teleporte Instantâneo ao Colidir

**User Story:** Como jogador, quero ser teletransportado imediatamente quando meu personagem colidir com um ponto de teleporte, para que a transição entre áreas seja rápida e direta.

#### Acceptance Criteria

1. WHEN o personagem entra em colisão (OnTriggerEnter2D) com um TeleportPoint THEN o sistema SHALL iniciar imediatamente o processo de carregamento da cena de destino
2. WHEN o processo de teleporte é iniciado THEN o sistema SHALL NOT verificar proximidade ou distância do personagem
3. WHEN o personagem colide com o TeleportPoint THEN o sistema SHALL executar a transição visual e carregar a nova cena em uma única operação

### Requirement 2: Remoção do Sistema de Pré-carregamento

**User Story:** Como desenvolvedor, quero remover o sistema de pré-carregamento de cenas para simplificar o código e reduzir a complexidade do sistema de teleporte.

#### Acceptance Criteria

1. WHEN o TeleportPoint é configurado THEN o sistema SHALL NOT ter opções de pré-carregamento (enablePreloading, preloadProximityRadius)
2. WHEN o TeleportManager é inicializado THEN o sistema SHALL NOT manter estruturas de dados para pré-carregamento (preloadOperations, sceneLastUsedTime)
3. WHEN uma cena precisa ser carregada THEN o sistema SHALL carregar diretamente durante a transição visual
4. WHEN o CircleCollider2D de proximidade existe THEN o sistema SHALL remover este componente

### Requirement 3: Remoção de Validações de Proximidade

**User Story:** Como desenvolvedor, quero remover todas as validações de proximidade para que o teleporte funcione apenas com colisão direta.

#### Acceptance Criteria

1. WHEN o OnTriggerEnter2D é chamado THEN o sistema SHALL NOT calcular distância entre o personagem e o TeleportPoint
2. WHEN o OnTriggerEnter2D é chamado THEN o sistema SHALL NOT verificar se está dentro de um raio de proximidade
3. WHEN o OnTriggerExit2D é chamado THEN o sistema SHALL NOT executar lógica de cancelamento de pré-carregamento
4. IF o collider que entra é do Player THEN o sistema SHALL iniciar o teleporte imediatamente

### Requirement 4: Posicionamento Imediato do Personagem

**User Story:** Como jogador, quero que meu personagem seja posicionado corretamente na nova cena assim que ela for carregada, sem delays adicionais.

#### Acceptance Criteria

1. WHEN a nova cena é carregada THEN o sistema SHALL posicionar o personagem na posição de destino definida
2. WHEN o personagem é posicionado THEN o sistema SHALL posicionar a câmera mantendo o offset correto
3. WHEN o posicionamento é concluído THEN o sistema SHALL executar o fade in da transição visual
4. WHEN todo o processo é concluído THEN o sistema SHALL reabilitar o movimento do personagem

### Requirement 5: Simplificação da Interface do Inspector

**User Story:** Como designer de níveis, quero uma interface simplificada no Inspector com apenas as configurações essenciais para o teleporte.

#### Acceptance Criteria

1. WHEN o TeleportPoint é visualizado no Inspector THEN o sistema SHALL NOT exibir campos de pré-carregamento
2. WHEN o TeleportPoint é visualizado no Inspector THEN o sistema SHALL exibir apenas: destinationPosition, destinationSceneName, transitionEffect, delayBeforeFadeIn, e configurações de áudio
3. WHEN o TeleportPoint é visualizado no Inspector THEN o sistema SHALL manter as configurações de debug e gizmos
4. WHEN isCrossSceneTeleport é false THEN o sistema SHALL ocultar o campo destinationSceneName

### Requirement 6: Manutenção da Funcionalidade de Transição Visual

**User Story:** Como jogador, quero continuar tendo transições visuais suaves durante o teleporte, mesmo com o sistema simplificado.

#### Acceptance Criteria

1. WHEN o teleporte é iniciado THEN o sistema SHALL executar o fade out usando o TransitionEffect configurado
2. WHEN o fade out está completo THEN o sistema SHALL carregar a nova cena
3. WHEN a nova cena está carregada e o personagem posicionado THEN o sistema SHALL executar o fade in
4. WHEN sons de teleporte estão configurados THEN o sistema SHALL reproduzir os sons nos momentos apropriados (início, meio, fim)
