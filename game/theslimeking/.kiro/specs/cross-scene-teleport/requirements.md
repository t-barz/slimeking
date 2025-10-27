# Sistema de Teletransporte Entre Cenas - Documento de Requisitos

## Introdução

Este documento define os requisitos para evoluir o sistema de teletransporte existente do jogo The Slime King, adicionando suporte para teletransporte entre diferentes cenas (mapas), pré-carregamento de cenas para transições mais rápidas, e efeitos sonoros durante as transições.

## Requisitos

### Requisito 1: Teletransporte Entre Cenas

**User Story:** Como jogador, quero poder usar pontos de teletransporte para me mover entre diferentes mapas/cenas do jogo, para que eu possa explorar diferentes áreas do mundo sem interrupções na experiência.

#### Acceptance Criteria

1. WHEN um TeleportPoint é configurado com uma cena de destino THEN o sistema SHALL carregar a cena especificada durante o teletransporte
2. WHEN o teletransporte entre cenas é iniciado THEN o sistema SHALL manter o estado do Player (vida, itens, etc.)
3. WHEN a nova cena é carregada THEN o Player SHALL aparecer na posição de destino especificada
4. WHEN o TeleportPoint é configurado no Inspector THEN SHALL ser possível escolher entre teletransporte na mesma cena ou para outra cena
5. IF a cena de destino não existir ou não estiver nas Build Settings THEN o sistema SHALL exibir um erro no Console e SHALL NOT executar o teletransporte

### Requisito 2: Pré-carregamento de Cenas

**User Story:** Como jogador, quero que as transições entre cenas sejam rápidas e fluidas, para que eu não precise esperar longos tempos de carregamento ao usar pontos de teletransporte.

#### Acceptance Criteria

1. WHEN o Player se aproxima de um TeleportPoint que leva a outra cena THEN o sistema SHALL iniciar o pré-carregamento da cena de destino em background
2. WHEN o pré-carregamento é iniciado THEN SHALL NOT afetar a performance do jogo ou causar stuttering
3. WHEN o Player ativa o teletransporte e a cena já está pré-carregada THEN a transição SHALL ser instantânea (sem tempo de espera adicional)
4. WHEN o Player se afasta do TeleportPoint antes de usá-lo THEN o sistema SHALL poder descarregar a cena pré-carregada para liberar memória
5. IF múltiplos TeleportPoints estão próximos THEN o sistema SHALL gerenciar o pré-carregamento de forma inteligente para não sobrecarregar a memória
6. WHEN o pré-carregamento está em progresso THEN SHALL ser possível visualizar o status no Inspector (modo debug)

### Requisito 3: Efeitos Sonoros de Transição

**User Story:** Como jogador, quero ouvir efeitos sonoros durante o teletransporte, para que a experiência seja mais imersiva e eu tenha feedback auditivo da ação.

#### Acceptance Criteria

1. WHEN o teletransporte é iniciado THEN o sistema SHALL reproduzir um som de "início de teletransporte"
2. WHEN o Player está sendo reposicionado (meio da transição) THEN o sistema SHALL reproduzir um som de "whoosh" ou "portal"
3. WHEN o teletransporte é completado THEN o sistema SHALL reproduzir um som de "chegada"
4. WHEN os sons são configurados no Inspector THEN SHALL ser possível atribuir AudioClips diferentes para cada fase da transição
5. IF um AudioClip não estiver configurado THEN o sistema SHALL continuar funcionando normalmente sem reproduzir aquele som específico
6. WHEN os sons são reproduzidos THEN SHALL respeitar as configurações de volume do jogo

### Requisito 4: Zona de Proximidade para Pré-carregamento

**User Story:** Como desenvolvedor, quero poder configurar a distância em que o pré-carregamento é ativado, para que eu possa otimizar o uso de memória e performance em diferentes situações.

#### Acceptance Criteria

1. WHEN um TeleportPoint é criado THEN SHALL ter um campo configurável para definir o raio de proximidade para pré-carregamento
2. WHEN o Player entra na zona de proximidade THEN o sistema SHALL detectar através de um trigger maior que o trigger de ativação
3. WHEN o raio de proximidade é modificado no Inspector THEN os Gizmos SHALL refletir visualmente a nova área
4. WHEN o modo debug está ativo THEN SHALL logar quando o Player entra/sai da zona de proximidade
5. IF o raio de proximidade for zero THEN o pré-carregamento SHALL ser desabilitado para aquele TeleportPoint

### Requisito 5: Gerenciamento de Memória

**User Story:** Como desenvolvedor, quero que o sistema gerencie automaticamente a memória das cenas pré-carregadas, para evitar problemas de performance ou falta de memória.

#### Acceptance Criteria

1. WHEN uma cena é pré-carregada THEN SHALL ser carregada de forma aditiva (sem descarregar a cena atual)
2. WHEN o Player usa o teletransporte THEN a cena anterior SHALL ser descarregada automaticamente
3. WHEN o Player se afasta de um TeleportPoint THEN a cena pré-carregada SHALL ser descarregada após um delay configurável
4. WHEN múltiplas cenas estão pré-carregadas THEN o sistema SHALL manter apenas um número máximo configurável de cenas em memória
5. IF o limite de cenas pré-carregadas for atingido THEN o sistema SHALL descarregar a cena pré-carregada há mais tempo

### Requisito 6: Compatibilidade com Sistema Existente

**User Story:** Como desenvolvedor, quero que as novas funcionalidades sejam compatíveis com o sistema de teletransporte existente, para que eu não precise refatorar código já implementado.

#### Acceptance Criteria

1. WHEN o sistema é atualizado THEN os TeleportPoints existentes (mesma cena) SHALL continuar funcionando sem modificações
2. WHEN um TeleportPoint não tem cena de destino configurada THEN SHALL funcionar como teletransporte na mesma cena (comportamento atual)
3. WHEN o Easy Transition é utilizado THEN SHALL continuar funcionando da mesma forma para ambos os tipos de teletransporte
4. WHEN o código é escrito THEN SHALL seguir os mesmos padrões arquiteturais do sistema existente
5. IF eventos são disparados THEN SHALL ser compatível com o sistema de eventos existente

### Requisito 7: Feedback Visual de Pré-carregamento

**User Story:** Como jogador, quero ter feedback visual quando uma cena está sendo pré-carregada, para que eu saiba que o sistema está funcionando.

#### Acceptance Criteria

1. WHEN o pré-carregamento está em progresso THEN SHALL ser possível exibir um indicador visual opcional (partículas, brilho, etc.)
2. WHEN o pré-carregamento é completado THEN o indicador visual SHALL mudar para indicar que o teletransporte está pronto
3. WHEN o indicador visual é configurado no Inspector THEN SHALL ser possível habilitar/desabilitar para cada TeleportPoint
4. WHEN o modo debug está ativo THEN SHALL exibir informações de progresso do carregamento
5. IF nenhum indicador visual estiver configurado THEN o sistema SHALL funcionar normalmente sem feedback visual

## Casos de Uso Especiais

### Caso 1: Teletransporte Bidirecional Entre Cenas

- Dois TeleportPoints em cenas diferentes podem apontar um para o outro
- Ambas as cenas podem ser pré-carregadas quando o Player está próximo de qualquer um dos pontos
- O sistema gerencia automaticamente qual cena descarregar

### Caso 2: Hub Central com Múltiplos Destinos

- Uma cena "hub" pode ter múltiplos TeleportPoints para diferentes cenas
- O sistema prioriza o pré-carregamento baseado na proximidade do Player
- Apenas as cenas mais próximas são mantidas pré-carregadas

### Caso 3: Teletransporte em Cadeia

- Player pode usar múltiplos teletransportes em sequência
- Cada transição descarrega a cena anterior
- O pré-carregamento continua funcionando para o próximo destino

### Caso 4: Teletransporte Sem Pré-carregamento

- Desenvolvedores podem desabilitar pré-carregamento para TeleportPoints específicos
- Útil para cenas grandes ou situações onde memória é limitada
- O teletransporte ainda funciona, mas com tempo de carregamento visível

## Restrições Técnicas

1. O sistema deve manter compatibilidade com o sistema de teletransporte existente
2. O pré-carregamento deve usar Unity's SceneManager.LoadSceneAsync com LoadSceneMode.Additive
3. O sistema deve funcionar com Unity 6.2+
4. O código deve estar em inglês, comentários em português
5. O sistema deve seguir o princípio KISS (Keep It Simple and Straightforward)
6. O pré-carregamento não deve causar stuttering ou queda de FPS perceptível

## Dependências

1. Sistema de teletransporte existente (TeleportPoint, TeleportTransitionHelper)
2. Easy Transition asset
3. Unity SceneManager
4. PlayerController existente
5. Sistema de áudio do Unity (AudioSource)
6. Cenas devem estar configuradas nas Build Settings

## Métricas de Sucesso

1. Tempo de transição entre cenas com pré-carregamento: < 0.5 segundos
2. Tempo de transição entre cenas sem pré-carregamento: < 3 segundos
3. Impacto no FPS durante pré-carregamento: < 5% de queda
4. Uso de memória adicional: < 200MB por cena pré-carregada
5. Compatibilidade: 100% dos TeleportPoints existentes continuam funcionando

## Priorização

### Must Have (MVP)

- Requisito 1: Teletransporte Entre Cenas
- Requisito 2: Pré-carregamento de Cenas (básico)
- Requisito 6: Compatibilidade com Sistema Existente

### Should Have

- Requisito 3: Efeitos Sonoros de Transição
- Requisito 4: Zona de Proximidade para Pré-carregamento
- Requisito 5: Gerenciamento de Memória

### Nice to Have

- Requisito 7: Feedback Visual de Pré-carregamento
