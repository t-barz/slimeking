# Testes de Integração - Sistema de NPCs

Este documento descreve os testes de integração realizados para validar o sistema de NPCs.

## Visão Geral

O sistema de NPCs foi testado para garantir que todos os componentes funcionam corretamente em conjunto:

- NPCController
- NPCAttributesHandler
- NPCDropData
- NPCInteractionData
- Integração com sistemas existentes

## Ambiente de Teste

**Cena de Teste:** `Assets/Scenes/Tests/NPCSystemTest.unity` (a ser criada)

**Componentes Necessários:**

- NPCController com NPCAttributesHandler
- PlayerController com PlayerAttributesHandler
- GameManager (opcional)
- Objetos de teste (waypoints, itens, etc.)

---

## Teste 1: Comunicação NPCController ↔ NPCAttributesHandler

### Objetivo

Verificar se NPCController recebe e responde corretamente aos eventos do NPCAttributesHandler.

### Procedimento

1. Criar NPC na cena com ambos componentes
2. Configurar enableLogs = true em ambos
3. Usar Inspector para modificar HP do AttributesHandler
4. Observar logs de eventos

### Resultados Esperados

- ✅ OnHealthChanged dispara e NPCController recebe
- ✅ OnDamageTaken dispara quando TakeDamage() é chamado
- ✅ OnNPCDied dispara quando HP chega a 0
- ✅ NPCController muda para estado Dead
- ✅ ProcessDrops() é chamado automaticamente

### Status

⚠️ **Pendente** - Requer criação de cena de teste

---

## Teste 2: Transições de Estado

### Objetivo

Verificar se as transições de estado funcionam corretamente.

### Procedimento

1. Criar NPC Enemy com patrol points
2. Colocar jogador fora do detection range
3. Observar NPC patrulhando (Estado: Patrolling)
4. Aproximar jogador dentro do detection range
5. Observar NPC perseguindo (Estado: Chasing)
6. Aproximar mais para attack range
7. Observar NPC atacando (Estado: Attacking)

### Resultados Esperados

- ✅ Estado inicial: Idle ou Patrolling
- ✅ Transição para Chasing quando jogador detectado
- ✅ Transição para Attacking quando em alcance
- ✅ Transição de volta para Patrolling quando jogador sai do range
- ✅ Gizmos mostram ranges corretamente

### Status

⚠️ **Pendente** - Requer criação de cena de teste

---

## Teste 3: Interação com PlayerController

### Objetivo

Verificar se NPC detecta e ataca o jogador corretamente.

### Procedimento

1. Criar NPC Enemy com targetLayers incluindo "Player"
2. Criar PlayerController com PlayerAttributesHandler
3. Aproximar jogador do NPC
4. Observar detecção e ataque
5. Verificar se dano é aplicado ao jogador

### Resultados Esperados

- ✅ NPC detecta jogador no detection range
- ✅ NPC persegue jogador
- ✅ NPC ataca quando em alcance
- ✅ OnAttackAnimationHit() aplica dano ao PlayerAttributesHandler
- ✅ Dano é calculado com defesa do jogador
- ✅ PlayerAttributesHandler.OnHealthChanged dispara

### Status

⚠️ **Pendente** - Requer criação de cena de teste

---

## Teste 4: Sistema de Drops

### Objetivo

Verificar se itens são dropados corretamente quando NPC morre.

### Procedimento

1. Criar NPC com NPCDropData configurado
2. Configurar drops com diferentes chances (25%, 50%, 100%)
3. Configurar guaranteedDrop em um dos drops
4. Reduzir HP do NPC a 0
5. Observar itens spawados

### Resultados Esperados

- ✅ ProcessDrops() é chamado quando NPC morre
- ✅ Drops com guaranteedDrop sempre aparecem
- ✅ Drops com chance aparecem probabilisticamente
- ✅ Quantidade é aleatória entre min e max
- ✅ Itens têm offset aleatório para não sobrepor
- ✅ Itens têm força aplicada para espalhar

### Status

⚠️ **Pendente** - Requer criação de cena de teste

---

## Teste 5: Comportamento de Cada Categoria

### Objetivo

Verificar se cada categoria de NPC se comporta conforme esperado.

### 5.1 Enemy (Inimigo)

**Configuração:**

- NPCCategory: Enemy
- RelationshipPoints: -10
- Detection Range: 7m

**Comportamento Esperado:**

- ✅ Persegue jogador automaticamente
- ✅ Ataca quando em alcance
- ✅ Não oferece interações
- ✅ Dropa itens ao morrer

**Status:** ⚠️ Pendente

### 5.2 Friendly (Amigável)

**Configuração:**

- NPCCategory: Friendly
- RelationshipPoints: 15
- Attack: 0

**Comportamento Esperado:**

- ✅ Não ataca jogador
- ✅ Oferece interações (diálogo, quest)
- ✅ Pode patrulhar área pequena
- ✅ Não dropa itens (geralmente)

**Status:** ⚠️ Pendente

### 5.3 Neutral (Neutro)

**Configuração:**

- NPCCategory: Neutral
- RelationshipPoints: 5

**Comportamento Esperado:**

- ✅ Ignora jogador inicialmente
- ✅ Retalia quando atacado
- ✅ Relacionamento diminui quando atacado
- ✅ Pode se tornar hostil ou amigável

**Status:** ⚠️ Pendente

### 5.4 Boss (Chefe)

**Configuração:**

- NPCCategory: Boss
- BehaviorGraphPrefab: Configurado
- HP elevado, atributos elevados

**Comportamento Esperado:**

- ✅ Usa Behavior Graph para IA
- ✅ Múltiplas fases de combate
- ✅ Ataques especiais
- ✅ Drops raros

**Status:** ⚠️ Pendente (requer Behavior Graph)

---

## Teste 6: Integração com Behavior Graph

### Objetivo

Verificar se integração com Behavior Graph funciona (Unity 6.2).

### Procedimento

1. Criar Behavior Graph simples
2. Configurar behaviorGraphPrefab no NPC Boss
3. Expor variáveis (isAlive, hasTarget, etc.)
4. Testar callbacks (OnStateEnter, OnStateExit)
5. Testar eventos (OnTargetDetected, OnTargetLost)

### Resultados Esperados

- ✅ Behavior Graph é instanciado no Start()
- ✅ Variáveis expostas são acessíveis
- ✅ Callbacks são chamados corretamente
- ✅ Eventos são enviados ao Behavior Graph
- ✅ OnBehaviorGraphStateChanged sincroniza estados

### Status

⚠️ **Pendente** - Requer Unity 6.2 Behavior Graph

---

## Teste 7: Sistema de Movimento

### Objetivo

Verificar se todos os padrões de movimento funcionam.

### 7.1 Idle

**Procedimento:**

- Configurar MovementPattern: Idle
- Observar NPC

**Esperado:**

- ✅ NPC permanece parado
- ✅ Velocity = Vector2.zero

**Status:** ⚠️ Pendente

### 7.2 PatrolPoints

**Procedimento:**

- Configurar MovementPattern: PatrolPoints
- Adicionar 4 waypoints em quadrado
- Observar NPC patrulhando

**Esperado:**

- ✅ NPC move entre waypoints sequencialmente
- ✅ Waypoint atual destacado em gizmos
- ✅ Loop infinito pelos pontos

**Status:** ⚠️ Pendente

### 7.3 CircularPatrol

**Procedimento:**

- Configurar MovementPattern: CircularPatrol
- Definir radius = 5m
- Observar NPC

**Esperado:**

- ✅ NPC move em área circular
- ✅ Movimento suave e natural
- ✅ Gizmo azul mostra área

**Status:** ⚠️ Pendente

### 7.4 ChaseTarget

**Procedimento:**

- Configurar MovementPattern: ChaseTarget
- Definir currentTarget = Player
- Mover jogador

**Esperado:**

- ✅ NPC persegue jogador
- ✅ Velocidade baseada em CurrentSpeed
- ✅ Linha vermelha mostra conexão

**Status:** ⚠️ Pendente

---

## Teste 8: Sistema de Interação

### Objetivo

Verificar se interações funcionam corretamente.

### 8.1 Dialogue

**Procedimento:**

- Criar NPCInteractionData tipo Dialogue
- Configurar dialogueID
- Chamar Interact() do jogador

**Esperado:**

- ✅ CanInteractWith() retorna true
- ✅ ProcessDialogueInteraction() é chamado
- ✅ DialogueManager é notificado (quando disponível)

**Status:** ⚠️ Pendente

### 8.2 QuestActivation

**Procedimento:**

- Criar NPCInteractionData tipo QuestActivation
- Configurar questID
- Chamar Interact()

**Esperado:**

- ✅ ProcessQuestActivationInteraction() é chamado
- ✅ QuestManager é notificado (quando disponível)
- ✅ oneTimeOnly funciona corretamente

**Status:** ⚠️ Pendente

### 8.3 ItemDelivery

**Procedimento:**

- Criar NPCInteractionData tipo ItemDelivery
- Configurar requiredItemIDs
- Chamar Interact() com itens

**Esperado:**

- ✅ ProcessItemDeliveryInteraction() é chamado
- ✅ InventoryManager é consultado (quando disponível)
- ✅ Itens são removidos se entregues

**Status:** ⚠️ Pendente

---

## Teste 9: Sistema de Relacionamento

### Objetivo

Verificar se sistema de relacionamento funciona.

### Procedimento

1. Criar NPC Neutral com RelationshipPoints = 5
2. Chamar IncreaseRelationship(10)
3. Verificar se IsFriendly() retorna true
4. Chamar DecreaseRelationship(20)
5. Verificar se IsHostile() retorna true

### Resultados Esperados

- ✅ RelationshipPoints muda corretamente
- ✅ OnRelationshipChanged dispara
- ✅ IsHostile() retorna true quando < 0
- ✅ IsNeutral() retorna true quando 0-10
- ✅ IsFriendly() retorna true quando > 10
- ✅ Clamp em -100 a 100 funciona

### Status

⚠️ **Pendente** - Requer criação de cena de teste

---

## Teste 10: Debug Gizmos

### Objetivo

Verificar se gizmos de debug são desenhados corretamente.

### Procedimento

1. Criar NPC com enableDebugGizmos = true
2. Configurar todos os ranges
3. Adicionar patrol points
4. Definir currentTarget
5. Observar Scene View

### Resultados Esperados

- ✅ Detection range (amarelo) visível
- ✅ Attack range (vermelho) visível
- ✅ Interaction range (verde) visível
- ✅ Patrol points conectados (verde)
- ✅ Circular patrol área (azul)
- ✅ Linha para alvo (vermelho)
- ✅ Informações textuais acima do NPC

### Status

⚠️ **Pendente** - Requer criação de cena de teste

---

## Checklist de Validação Final

### Componentes Core

- [ ] NPCController inicializa corretamente
- [ ] NPCAttributesHandler inicializa corretamente
- [ ] Eventos entre componentes funcionam
- [ ] Estados transitam corretamente

### Movimento

- [ ] Idle funciona
- [ ] PatrolPoints funciona
- [ ] CircularPatrol funciona
- [ ] ChaseTarget funciona
- [ ] Aceleração/desaceleração suave

### Combate

- [ ] Detecção de alvos funciona
- [ ] Perseguição funciona
- [ ] Ataque funciona
- [ ] Dano é aplicado corretamente
- [ ] Cooldown de ataque funciona

### Interação

- [ ] CanInteractWith() valida corretamente
- [ ] Dialogue funciona
- [ ] QuestActivation funciona
- [ ] ItemDelivery funciona
- [ ] oneTimeOnly funciona

### Drops

- [ ] ProcessDrops() é chamado ao morrer
- [ ] Probabilidade funciona
- [ ] guaranteedDrop funciona
- [ ] Quantidade aleatória funciona
- [ ] Offset e força aplicados

### Categorias

- [ ] Enemy se comporta corretamente
- [ ] Friendly se comporta corretamente
- [ ] Neutral se comporta corretamente
- [ ] Boss usa Behavior Graph

### Integração

- [ ] PlayerController detectado
- [ ] Dano aplicado ao jogador
- [ ] GameManager integração (placeholder)
- [ ] AudioManager integração (placeholder)

### Debug

- [ ] Logs funcionam quando habilitados
- [ ] Gizmos desenham corretamente
- [ ] Informações textuais visíveis

---

## Próximos Passos

1. **Criar Cena de Teste**
   - Assets/Scenes/Tests/NPCSystemTest.unity
   - Adicionar todos os prefabs de exemplo
   - Configurar ambiente de teste

2. **Executar Testes Manuais**
   - Seguir procedimentos descritos
   - Documentar resultados
   - Corrigir bugs encontrados

3. **Testes Automatizados (Opcional)**
   - Criar scripts de teste unitário
   - Usar Unity Test Framework
   - Automatizar validações

4. **Integração com Sistemas Reais**
   - Conectar com DialogueManager real
   - Conectar com QuestManager real
   - Conectar com AudioManager real
   - Remover placeholders

5. **Performance Testing**
   - Testar com múltiplos NPCs (10, 50, 100)
   - Medir impacto de performance
   - Otimizar se necessário

---

## Notas de Implementação

### Limitações Conhecidas

- Behavior Graph requer Unity 6.2 (não testado ainda)
- Integrações com managers usam placeholders
- Prefabs não foram criados (apenas código)
- Cena de teste não foi criada

### Recomendações

- Criar cena de teste antes de usar em produção
- Testar cada categoria separadamente
- Validar integração com sistemas existentes
- Adicionar mais logs de debug se necessário

### Bugs Conhecidos

- Nenhum bug conhecido no momento
- Sistema não foi testado em runtime ainda

---

## Conclusão

O sistema de NPCs foi implementado seguindo todos os requisitos especificados. Todos os componentes foram criados e integrados, mas **testes práticos em runtime ainda são necessários** para validação completa.

**Status Geral:** ✅ Implementação Completa | ⚠️ Testes Pendentes

**Próximo Passo:** Criar cena de teste e executar validações manuais.
