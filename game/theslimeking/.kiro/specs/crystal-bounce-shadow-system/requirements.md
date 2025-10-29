# Requirements Document

## Introduction

Este documento define os requisitos para reconstruir e corrigir o sistema de quicadas e sombra dinâmica para objetos coletáveis (cristais) no jogo. O sistema atual possui problemas na sincronização entre o objeto principal e sua sombra, além de questões na física de quicadas. O objetivo é criar um sistema robusto que simule corretamente o lançamento, quicadas e parada de objetos com uma sombra que acompanha visualmente a altura simulada do objeto.

## Requirements

### Requirement 1: Sistema de Lançamento de Objetos

**User Story:** Como desenvolvedor, quero que objetos coletáveis sejam lançados em uma direção aleatória quando instanciados, para criar uma experiência visual dinâmica e interessante.

#### Acceptance Criteria

1. WHEN um objeto com BounceHandler é instanciado THEN o sistema SHALL lançar o objeto automaticamente em uma direção aleatória
2. WHEN o lançamento ocorre THEN o sistema SHALL aplicar uma força entre os valores mínimo e máximo configurados
3. WHEN o ângulo de lançamento é calculado THEN o sistema SHALL escolher um ângulo aleatório entre os limites configurados (45° a 135°)
4. WHEN a força é aplicada THEN o sistema SHALL usar o Rigidbody2D com ForceMode2D.Impulse para movimento instantâneo
5. IF o objeto já foi lançado THEN o sistema SHALL ignorar tentativas subsequentes de lançamento
6. WHEN o lançamento é executado THEN o sistema SHALL salvar a posição inicial, direção e força para uso no sistema de quicadas

### Requirement 2: Sistema de Quicadas Sequenciais

**User Story:** Como desenvolvedor, quero que objetos lançados executem uma sequência de quicadas com força decrescente, para simular um comportamento físico realista de objetos quicando até parar.

#### Acceptance Criteria

1. WHEN o objeto é lançado THEN o sistema SHALL iniciar uma sequência de quicadas configurável
2. WHEN cada quicada ocorre THEN o sistema SHALL aplicar força reduzida baseada no fator de redução configurado (ex: 0.8 = 20% de redução)
3. WHEN o intervalo entre quicadas é calculado THEN o sistema SHALL reduzir o tempo pela metade a cada quicada
4. WHEN todas as quicadas são completadas THEN o sistema SHALL parar completamente o movimento do objeto
5. WHEN o movimento é parado THEN o sistema SHALL zerar velocidades linear e angular do Rigidbody2D
6. IF o número de quicadas configurado é 0 THEN o sistema SHALL apenas lançar o objeto sem quicadas subsequentes
7. WHEN a última quicada é processada THEN o sistema SHALL tornar o Rigidbody2D kinematic para evitar movimento adicional

### Requirement 3: Sistema de Sombra Dinâmica

**User Story:** Como jogador, quero ver uma sombra que acompanha o objeto coletável e muda de tamanho conforme a altura simulada, para ter feedback visual claro da posição do objeto no espaço 3D simulado.

#### Acceptance Criteria

1. WHEN um objeto shadowA é configurado como filho THEN o sistema SHALL reconhecer e ativar o sistema de sombra
2. WHEN o objeto principal está em movimento THEN o sistema SHALL atualizar a escala da sombra a cada frame
3. WHEN a velocidade vertical é positiva (subindo) THEN a sombra SHALL diminuir de tamanho proporcionalmente
4. WHEN a velocidade vertical é negativa (descendo) THEN a sombra SHALL aumentar de tamanho proporcionalmente
5. WHEN o objeto está parado (velocidade zero) THEN a sombra SHALL estar no tamanho máximo configurado
6. WHEN a escala da sombra é calculada THEN o sistema SHALL usar interpolação linear entre escala mínima e máxima
7. WHEN a posição da sombra é atualizada THEN o sistema SHALL aplicar o offset configurado relativo ao objeto principal
8. IF nenhum objeto de sombra é configurado THEN o sistema SHALL funcionar normalmente sem erros
9. WHEN o movimento para THEN a sombra SHALL retornar ao tamanho máximo (objeto no chão)

### Requirement 4: Sincronização entre Objeto e Sombra

**User Story:** Como desenvolvedor, quero que a sombra esteja sempre sincronizada com o objeto principal, para evitar desconexão visual entre os elementos.

#### Acceptance Criteria

1. WHEN o objeto principal se move THEN a sombra SHALL seguir a posição horizontal do objeto
2. WHEN a sombra é posicionada THEN o sistema SHALL manter o offset vertical configurado constante
3. WHEN o objeto é destruído THEN a sombra SHALL ser destruída junto (por ser filho)
4. WHEN o sistema de quicadas está ativo THEN a sombra SHALL responder imediatamente às mudanças de velocidade
5. IF o Rigidbody2D é nulo ou inválido THEN o sistema de sombra SHALL parar de atualizar sem causar erros

### Requirement 5: Integração com Sistema de Coleta

**User Story:** Como desenvolvedor, quero que o sistema de quicadas funcione em conjunto com o sistema de coleta de itens, para que objetos possam ser coletados após pararem de quicar.

#### Acceptance Criteria

1. WHEN o objeto está quicando THEN o componente ItemCollectable SHALL respeitar o delay de ativação configurado
2. WHEN o delay de ativação expira THEN o sistema de atração magnética SHALL ser habilitado
3. WHEN o objeto é coletado durante as quicadas THEN ambos os sistemas SHALL funcionar sem conflitos
4. WHEN o BounceHandler para o movimento THEN o ItemCollectable SHALL continuar funcionando normalmente
5. IF o objeto é coletado antes de parar THEN o sistema SHALL cancelar quicadas pendentes e executar a coleta

### Requirement 6: Configurabilidade e Debug

**User Story:** Como desenvolvedor, quero ter controle total sobre os parâmetros do sistema e ferramentas de debug, para ajustar o comportamento conforme necessário e diagnosticar problemas.

#### Acceptance Criteria

1. WHEN parâmetros são expostos no Inspector THEN todos os valores SHALL ter tooltips descritivos
2. WHEN debug está habilitado THEN o sistema SHALL logar eventos importantes (lançamento, quicadas, parada)
3. WHEN valores são alterados no Inspector THEN o sistema SHALL usar os novos valores no próximo lançamento
4. WHEN Gizmos são desenhados THEN o sistema SHALL mostrar raio de atração e conexões visuais relevantes
5. IF valores inválidos são configurados THEN o sistema SHALL usar valores padrão seguros e logar avisos
6. WHEN métodos públicos são expostos THEN o sistema SHALL permitir controle manual de lançamento, parada e reset

### Requirement 7: Controle de Colliders Durante Quicadas

**User Story:** Como desenvolvedor, quero que os colliders sejam desabilitados durante as quicadas e só sejam habilitados quando o objeto estiver completamente parado e pronto para coleta, para evitar colisões prematuras e comportamento inesperado.

#### Acceptance Criteria

1. WHEN o objeto é instanciado THEN todos os colliders (exceto do Rigidbody2D) SHALL ser desabilitados imediatamente
2. WHEN o lançamento ocorre THEN os colliders SHALL permanecer desabilitados durante todo o processo de quicadas
3. WHEN todas as quicadas são completadas e o movimento para THEN o sistema SHALL aguardar o delay de ativação configurado
4. WHEN o delay de ativação expira THEN o sistema SHALL habilitar os colliders para permitir coleta
5. IF o ItemCollectable tem um delay de ativação THEN o sistema SHALL sincronizar com esse delay
6. WHEN os colliders são habilitados THEN o sistema SHALL logar o evento se debug estiver ativo
7. IF o objeto é destruído antes de parar THEN o sistema SHALL cancelar a habilitação pendente dos colliders

### Requirement 8: Performance e Otimização

**User Story:** Como desenvolvedor, quero que o sistema seja eficiente e não cause problemas de performance, mesmo com múltiplos objetos na cena.

#### Acceptance Criteria

1. WHEN múltiplos objetos estão ativos THEN o sistema SHALL usar cache de componentes para evitar GetComponent repetidos
2. WHEN a sombra é atualizada THEN o sistema SHALL fazer cálculos apenas quando necessário (objeto em movimento)
3. WHEN logs de debug são gerados THEN o sistema SHALL limitar a frequência para evitar spam no console
4. WHEN o objeto para de se mover THEN o sistema SHALL parar de executar Update da sombra
5. IF o sistema de sombra não está configurado THEN o código relacionado SHALL ser completamente ignorado
