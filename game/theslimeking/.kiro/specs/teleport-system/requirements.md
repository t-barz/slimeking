# Sistema de Teletransporte - Documento de Requisitos

## Introdução

Este documento define os requisitos para o sistema de teletransporte do jogo The Slime King. O sistema permitirá que o jogador se mova instantaneamente entre diferentes pontos do mapa através de pontos de teletransporte, com transições visuais suaves utilizando o asset Easy Transition.

## Requisitos

### Requisito 1: Ponto de Teletransporte Básico

**User Story:** Como jogador, quero poder interagir com pontos de teletransporte para me mover rapidamente entre diferentes áreas do mapa, para que eu possa explorar o mundo de forma mais eficiente.

**Acceptance Criteria:**

1. WHEN o jogador colide com um TeleportPoint THEN o sistema SHALL detectar a colisão através de um BoxCollider2D configurado como Trigger
2. WHEN a colisão é detectada THEN o sistema SHALL verificar se o GameObject possui a tag "Player"
3. WHEN o Player é detectado THEN o sistema SHALL iniciar automaticamente o processo de teletransporte
4. WHEN o TeleportPoint é criado THEN ele SHALL ter um campo configurável no Inspector para definir a posição de destino (Vector3)

### Requisito 2: Transição Visual com Easy Transition

**User Story:** Como jogador, quero ver uma transição visual suave durante o teletransporte, para que a mudança de posição não seja abrupta e mantenha a imersão no jogo.

**Acceptance Criteria:**

1. WHEN o teletransporte é iniciado THEN o sistema SHALL ativar o efeito Circle do Easy Transition para cobrir a tela
2. WHEN o efeito de fade out (circle fechando) estiver completo THEN o Player e a câmera SHALL ser reposicionados para o destino
3. WHEN o reposicionamento estiver completo THEN o sistema SHALL aguardar 1 segundo antes de iniciar o fade in
4. WHEN o fade in (circle abrindo) for iniciado THEN o efeito SHALL revelar gradualmente a nova posição
5. WHEN o fade in estiver completo THEN o controle do Player SHALL ser restaurado

### Requisito 3: Controle do Player Durante Teletransporte

**User Story:** Como jogador, quero que meu controle seja bloqueado durante o teletransporte, para que eu não possa me mover enquanto a transição está acontecendo.

**Acceptance Criteria:**

1. WHEN o teletransporte é iniciado THEN o movimento do Player SHALL ser desabilitado
2. WHEN o teletransporte é iniciado THEN os inputs do Player SHALL ser ignorados
3. WHEN a transição visual estiver completa THEN o controle do Player SHALL ser restaurado automaticamente
4. WHEN o Player está sendo teletransportado THEN ele SHALL NOT poder iniciar outro teletransporte

### Requisito 4: Reposicionamento da Câmera

**User Story:** Como jogador, quero que a câmera me siga durante o teletransporte, para que eu sempre tenha uma visão clara da minha nova posição.

**Acceptance Criteria:**

1. WHEN o Player é reposicionado THEN a câmera SHALL ser reposicionada junto com ele
2. WHEN a câmera é reposicionada THEN ela SHALL manter a mesma distância e ângulo em relação ao Player
3. WHEN o reposicionamento ocorre THEN SHALL NOT haver "saltos" visíveis da câmera durante a transição

### Requisito 5: Configuração e Debug

**User Story:** Como desenvolvedor, quero poder configurar facilmente os pontos de teletransporte e visualizar suas áreas de ativação no Editor, para facilitar o level design.

**Acceptance Criteria:**

1. WHEN um TeleportPoint é selecionado no Editor THEN SHALL ser possível visualizar a área do trigger através de Gizmos
2. WHEN um TeleportPoint é selecionado THEN SHALL ser possível visualizar uma linha conectando o ponto de origem ao destino
3. WHEN o sistema está em modo debug THEN SHALL logar informações sobre o início e fim do teletransporte
4. WHEN o TeleportPoint é configurado THEN SHALL ter opções para ajustar o tamanho do trigger no Inspector
5. IF o destino não estiver configurado THEN o sistema SHALL exibir um aviso no Console e SHALL NOT executar o teletransporte

### Requisito 6: Integração com Sistema Existente

**User Story:** Como desenvolvedor, quero que o sistema de teletransporte se integre perfeitamente com o PlayerController existente, para manter a consistência do código.

**Acceptance Criteria:**

1. WHEN o teletransporte é iniciado THEN SHALL utilizar os métodos existentes do PlayerController para desabilitar movimento
2. WHEN o Easy Transition é utilizado THEN SHALL NOT modificar os scripts originais do asset
3. WHEN o sistema é implementado THEN SHALL seguir os padrões arquiteturais definidos em BoasPraticas.md
4. IF eventos são disparados THEN SHALL utilizar o sistema de eventos do projeto (se existente)
5. WHEN o código é escrito THEN SHALL utilizar regiões para organização e comentários em português

## Casos de Uso Especiais

### Caso 1: Teletransporte Bidirecional

- Dois TeleportPoints podem apontar um para o outro, criando um sistema de ida e volta
- Cada ponto funciona independentemente

### Caso 2: Múltiplos Destinos

- Um TeleportPoint pode ter apenas um destino por vez
- Para múltiplos destinos, criar múltiplos TeleportPoints

### Caso 3: Teletransporte Entre Cenas

- **Fora do escopo inicial** - O sistema atual funciona apenas dentro da mesma cena
- Pode ser expandido futuramente para suportar mudança de cenas

## Restrições Técnicas

1. O sistema deve utilizar o Easy Transition sem modificar seus scripts originais
2. O sistema deve ser compatível com Unity 6.2+
3. O sistema deve funcionar com o PlayerController existente
4. O sistema deve seguir o princípio KISS (Keep It Simple and Straightforward)
5. O código deve estar em inglês, comentários em português

## Dependências

1. Easy Transition asset (já presente no projeto)
2. PlayerController existente (Assets/External/AssetStore/SlimeMec/_Scripts/Gameplay/PlayerController.cs)
3. Sistema de tags do Unity (tag "Player" deve existir)
4. CircleEffect do Easy Transition (Assets/External/AssetStore/Easy Transition/Transition Effects/CircleEffect.asset)
