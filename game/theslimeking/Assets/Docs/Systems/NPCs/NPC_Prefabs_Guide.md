# Guia de Prefabs de NPCs

Este documento descreve os prefabs de exemplo disponíveis para o sistema de NPCs e como utilizá-los no jogo.

## Visão Geral do Sistema

O sistema de NPCs é baseado em três componentes principais:

1. **NPCController** - Controlador principal que gerencia comportamento, movimento, combate e interações
2. **NPCAttributesHandler** - Gerencia atributos (HP, Ataque, Defesa, Velocidade, Relacionamento)
3. **ScriptableObjects** - Dados configuráveis (NPCDropData, NPCInteractionData)

## Prefabs Disponíveis

### 1. NPC_Enemy_Example

**Categoria:** Enemy  
**Comportamento:** Hostil e agressivo

**Configuração Padrão:**

- HP: 50
- Ataque: 15
- Defesa: 5
- Velocidade: 3
- Relacionamento: -10 (Hostil)
- Movimento: PatrolPoints ou ChaseTarget
- Detection Range: 7m
- Attack Range: 1.5m

**Uso:**

```
1. Arraste o prefab para a cena
2. Configure os patrol points se usar PatrolPoints
3. Ajuste targetLayers para detectar o jogador
4. Configure drops em possibleDrops[]
```

**Comportamento:**

- Detecta o jogador automaticamente
- Persegue quando detectado
- Ataca quando em alcance
- Dropa itens ao morrer

---

### 2. NPC_Friendly_Example

**Categoria:** Friendly  
**Comportamento:** Amigável e não-agressivo

**Configuração Padrão:**

- HP: 100
- Ataque: 0 (não ataca)
- Defesa: 10
- Velocidade: 2
- Relacionamento: 15 (Amigável)
- Movimento: Idle ou CircularPatrol
- Interaction Range: 2m

**Uso:**

```
1. Arraste o prefab para a cena
2. Configure interactions[] com NPCInteractionData
3. Adicione diálogos, quests ou loja
4. Ajuste interactionRange conforme necessário
```

**Comportamento:**

- Não ataca o jogador
- Oferece interações (diálogo, quests, loja)
- Pode patrulhar área pequena
- Não dropa itens (geralmente)

---

### 3. NPC_Neutral_Example

**Categoria:** Neutral  
**Comportamento:** Neutro, ataca apenas quando provocado

**Configuração Padrão:**

- HP: 75
- Ataque: 12
- Defesa: 8
- Velocidade: 2.5
- Relacionamento: 5 (Neutro)
- Movimento: PatrolPoints ou Wander
- Detection Range: 5m
- Attack Range: 1.5m

**Uso:**

```
1. Arraste o prefab para a cena
2. Configure patrol points ou área de wander
3. Ajuste relacionamento inicial
4. Configure drops moderados
```

**Comportamento:**

- Ignora o jogador inicialmente
- Retalia quando atacado (relacionamento diminui)
- Pode se tornar amigável com interações positivas
- Dropa itens ao morrer

---

### 4. NPC_Boss_Example

**Categoria:** Boss  
**Comportamento:** Chefe com Behavior Graph complexo

**Configuração Padrão:**

- HP: 200
- Ataque: 25
- Defesa: 15
- Velocidade: 4
- Relacionamento: -50 (Muito Hostil)
- Movimento: Controlado por Behavior Graph
- Detection Range: 10m
- Attack Range: 2m

**Uso:**

```
1. Arraste o prefab para a cena
2. Configure behaviorGraphPrefab com Behavior Graph customizado
3. Configure drops especiais e raros
4. Ajuste atributos para dificuldade desejada
```

**Comportamento:**

- Usa Behavior Graph para IA complexa
- Múltiplas fases de combate
- Ataques especiais
- Drops raros e valiosos

---

## Criando NPCDropData

Para configurar drops de itens:

1. **Criar Asset:**
   - Clique direito na pasta Assets/Data/NPCs/
   - Create > Game > NPC > Drop Data

2. **Configurar:**

   ```
   Item Prefab: [Prefab do item a dropar]
   Drop Chance: 50% (0-100)
   Guaranteed Drop: false
   Min Quantity: 1
   Max Quantity: 3
   Drop Name: "Poção de Vida"
   ```

3. **Adicionar ao NPC:**
   - Selecione o NPC na cena
   - Adicione o NPCDropData ao array possibleDrops[]

---

## Criando NPCInteractionData

Para configurar interações:

1. **Criar Asset:**
   - Clique direito na pasta Assets/Data/NPCs/
   - Create > Game > NPC > Interaction Data

2. **Configurar Diálogo:**

   ```
   Interaction Type: Dialogue
   Interaction Name: "Conversar"
   Required Relationship Points: 0
   Dialogue ID: "npc_greeting_01"
   One Time Only: false
   ```

3. **Configurar Quest:**

   ```
   Interaction Type: QuestActivation
   Interaction Name: "Aceitar Quest"
   Required Relationship Points: 5
   Quest ID: "quest_forest_01"
   One Time Only: true
   ```

4. **Adicionar ao NPC:**
   - Selecione o NPC na cena
   - Adicione o NPCInteractionData ao array interactions[]

---

## Configurando Movimento

### Idle (Parado)

```
Movement Pattern: Idle
```

### Patrol Points (Patrulha entre pontos)

```
Movement Pattern: PatrolPoints
Patrol Points: [Array de Transforms]
Waypoint Reach Threshold: 0.5m
```

### Circular Patrol (Patrulha circular)

```
Movement Pattern: CircularPatrol
Patrol Center: (0, 0) ou deixar (0,0) para usar posição inicial
Patrol Radius: 5m
Circular Patrol Speed: 2
```

### Chase Target (Perseguir alvo)

```
Movement Pattern: ChaseTarget
(Configurado automaticamente durante combate)
```

---

## Configurando Combate

### Detection Range

- Distância em que o NPC detecta alvos
- Recomendado: 5-10m para inimigos, 0m para amigáveis

### Attack Range

- Distância em que o NPC pode atacar
- Recomendado: 1-2m para melee, 5-10m para ranged

### Target Layers

- Layers que o NPC pode detectar como alvos
- Geralmente: "Player" layer

### Attack Cooldown

- Tempo entre ataques em segundos
- Recomendado: 1-2s para inimigos normais, 0.5s para bosses

---

## Integrando com Behavior Graph (Unity 6.2)

Para NPCs Boss ou com IA complexa:

1. **Criar Behavior Graph:**
   - Window > AI > Behavior Graph
   - Crie estados: Idle, Patrol, Chase, Attack, Special
   - Configure transições baseadas em variáveis expostas

2. **Variáveis Expostas:**
   - `isAlive` (bool)
   - `canMove` (bool)
   - `canAttack` (bool)
   - `currentHealth` (int)
   - `relationshipPoints` (int)
   - `hasTarget` (bool)
   - `distanceToTarget` (float)
   - `targetInAttackRange` (bool)

3. **Callbacks Disponíveis:**
   - `OnStateEnter(string stateName)`
   - `OnStateExit(string stateName)`
   - `OnTransition(string fromState, string toState)`

4. **Eventos para Behavior Graph:**
   - `OnTargetDetected(Transform target)`
   - `OnTargetLost()`
   - `OnHealthLow(float percentage)`

---

## Debug e Visualização

### Gizmos no Scene View

Quando `enableDebugGizmos` está ativo, você verá:

- **Círculo Amarelo:** Detection Range
- **Círculo Vermelho:** Attack Range
- **Círculo Verde:** Interaction Range
- **Linhas Verdes:** Patrol Points conectados
- **Círculo Azul:** Circular Patrol área
- **Linha Vermelha:** Linha para alvo atual
- **Texto:** Informações do NPC (HP, Estado, Relacionamento)

### Logs de Debug

Ative `enableLogs` no Inspector para ver:

- Inicialização do NPC
- Mudanças de estado
- Detecção de alvos
- Ataques e dano
- Interações
- Drops

---

## Exemplos de Uso

### Inimigo Patrulhando

```
1. Use NPC_Enemy_Example
2. Crie GameObjects vazios como waypoints
3. Adicione ao array patrolPoints[]
4. Movement Pattern: PatrolPoints
5. Configure drops de itens comuns
```

### NPC de Quest

```
1. Use NPC_Friendly_Example
2. Crie NPCInteractionData para quest
3. Configure dialogueID e questID
4. Adicione ao array interactions[]
5. Movement Pattern: Idle
```

### Boss de Área

```
1. Use NPC_Boss_Example
2. Crie Behavior Graph customizado
3. Configure múltiplos drops raros
4. Ajuste HP e atributos para dificuldade
5. Adicione ataques especiais via Behavior Graph
```

---

## Troubleshooting

### NPC não se move

- Verifique se Rigidbody2D está configurado (Kinematic ou Dynamic)
- Verifique se patrol points estão atribuídos
- Verifique se attributesHandler.CurrentSpeed > 0

### NPC não ataca

- Verifique targetLayers no Inspector
- Verifique se detectionRange > 0
- Verifique se attackRange > 0
- Verifique se NPC não está em estado Interacting

### NPC não interage

- Verifique se interactions[] tem dados
- Verifique se interactionRange > 0
- Verifique relacionamento vs requiredRelationshipPoints
- Verifique se oneTimeOnly não foi consumido

### Drops não aparecem

- Verifique se possibleDrops[] tem dados
- Verifique se itemPrefab está atribuído
- Verifique dropChance (pode ser baixa)
- Verifique se NPC realmente morreu (HP = 0)

---

## Próximos Passos

1. Crie seus próprios NPCDropData e NPCInteractionData
2. Customize os prefabs para seu jogo
3. Crie Behavior Graphs para bosses
4. Integre com sistemas de diálogo e quest
5. Adicione animações e efeitos visuais

Para mais informações, consulte:

- `Assets/Docs/Systems/NPCs Editor/NPC.md`
- `.kiro/specs/npc-system/design.md`
- `.kiro/specs/npc-system/requirements.md`
