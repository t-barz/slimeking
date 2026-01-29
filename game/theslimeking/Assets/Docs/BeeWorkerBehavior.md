# BeeWorker Behavior Specification

Documento de especificação técnica do inimigo **BeeWorker**.

---

## 📊 Atributos Básicos

| Atributo | Tipo | Valor Padrão | Descrição |
|----------|------|--------------|-----------|
| `maxHealth` | `int` | `3` | HP máximo do inimigo |
| `currentHealth` | `int` | - | HP atual (inicializado com `maxHealth`) |
| `attackDamage` | `float` | `10f` | Dano causado pelo ataque |
| `defense` | `float` | `5f` | Defesa do inimigo |
| `moveSpeed` | `float` | `3f` | Velocidade normal de deslocamento |

---

## 🎬 Animator

### Variáveis do Animator

| Nome | Tipo | Descrição |
|------|------|-----------|
| `isWalking` | `bool` | `true` quando o inimigo está se deslocando |
| `Hit` | `Trigger` | Chamada quando o inimigo sofrer dano |
| `Die` | `Trigger` | Chamada quando o inimigo chegar a 0 HP |
| `Attack` | `Trigger` | Usada quando o inimigo está ao alcance do Player |

### Estados do Animator

1. **Idle** - Estado parado/aguardando
2. **Walk** - Estado de movimentação
3. **Hit** - Estado ao sofrer dano
4. **Attack** - Estado de ataque
5. **Die** - Estado de morte

---

## 🔄 Máquina de Estados

O BeeWorker possui uma máquina de estados simples com dois estados principais:

### Estado: Patrulha

- **Comportamento**: O inimigo se move entre pontos de patrulha configurados
- **Sistema de Pontos**:
  - Lista de Transform/Vector3 configurável no Inspector (pontos de patrulha)
  - Aguarda X segundos (configurável) ao chegar em um ponto
  - Seleciona o próximo ponto após o tempo de espera
  - Move-se até o próximo ponto na velocidade normal (`moveSpeed`)
- **Transição para Combate**: Quando o Player entra no campo de visão E não está agachado (stealth)
- **Animator**: `isWalking = true` durante movimento entre pontos, `isWalking = false` enquanto aguarda no ponto

### Estado: Combate

- **Comportamento**: O inimigo persegue o Player com velocidade aumentada (1.5x)
- **Transição para Patrulha**: Quando o Player sai do campo de visão OU entra em modo stealth (agachado)
- **Objetivo**: Alcançar distância de ataque e executar ataque
- **Animator**: `isWalking = true` durante perseguição, trigger `Attack` ao alcance

### Lógica de Transição

```text
Patrulha (padrão)
  └─> Player no campo de visão + NÃO agachado
      └─> COMBATE
          ├─> Player sai do campo de visão → Volta para PATRULHA
          ├─> Player entra em stealth (agachado) → Volta para PATRULHA
          └─> Ao alcance de ataque → Executa Attack (permanece em COMBATE)
```

---

## 🤖 Sistema de IA

### Campo de Visão

- **Tipo**: 360° (circular)
- **Comportamento**: 
  - Quando o Player entrar no campo de visão e **não estiver agachado (stealth)**: transição para estado **Combate**
  - Quando o Player sair do campo de visão ou entrar em stealth: transição para estado **Patrulha**

### Perseguição (Estado: Combate)

- **Velocidade**: `1.5x` a velocidade normal de deslocamento (`moveSpeed * 1.5f`)
- **Objetivo**: Mover-se em direção ao Player para atacá-lo
- **Animação**: Ativar `isWalking = true` no Animator
- **Cancelamento**: Retorna para estado **Patrulha** se Player sair do campo de visão ou entrar em stealth

### Ataque

- **Condição**: Quando o inimigo está ao alcance de ataque do Player
- **Ação**: Chamar trigger `Attack` do Animator
- **HitBox**: A HitBox do inimigo deve ser ativada durante a animação de ataque

---

## 🎨 Movimento Visual (Bouncing)

Para tornar o movimento mais natural e orgânico, a abelha possui um sistema de bouncing suave contínuo:

### Características

- **Comportamento**: Movimento oscilatório suave nos eixos X e Y aplicado constantemente
- **Não-interferência**: O bouncing é um efeito visual que não afeta a lógica de movimento (patrulha/perseguição)
- **Amplitude Configurável**: Controle separado para intensidade nos eixos X e Y
- **Frequência Configurável**: Velocidade da oscilação
- **Desabilitado durante Ataque**: Durante a animação de Attack, o bouncing deve ser pausado

### Implementação Técnica

- Usar `Mathf.Sin` e `Mathf.Cos` com `Time.time` para criar oscilação suave
- Aplicar offset à posição do sprite/visual, não ao transform principal
- Alternativamente, usar transform filho para aplicar bouncing sem afetar colisores

### Estados e Bouncing

| Estado | Bouncing Ativo |
|--------|----------------|
| Patrulha | ✅ Sim |
| Combate (Perseguição) | ✅ Sim |
| Combate (Attack) | ❌ Não - Desabilitado durante animação |
| Hit | ❌ Não - Desabilitado durante animação |
| Die | ❌ Não |

---

## ⚔️ Sistema de Combate

### Receber Dano

**Condição de Ativação**:
- Objeto com Tag `PlayerAttack` atinge a **HurtBox** do inimigo

**Processo**:
1. Subtrair do `currentHealth` o valor de dano do ataque
2. Chamar trigger `Hit` do Animator
3. Aplicar knockback: mover o inimigo para longe do Player
4. Ativar invulnerabilidade temporária (tempo configurável via `SerializeField`)
5. Se `currentHealth <= 0`, chamar trigger `Die` e iniciar processo de morte

### HitBox do Inimigo

- **Estado Inicial**: Inativa/desabilitada
- **Controle**: Funções públicas para ativar/desativar
  - `public void EnableHitBox()`
  - `public void DisableHitBox()`
- **Uso**: Ativada durante a animação de ataque via Animation Events

---

## 🔧 Parâmetros Configuráveis

Parâmetros que devem estar disponíveis no Inspector (via `[SerializeField]`):

| Parâmetro | Tipo | Descrição |
|-----------|------|-----------|
| `detectionRadius` | `float` | Raio do campo de visão 360° |
| `detectionInterval` | `float` | Intervalo em segundos para checar detecção do Player (padrão: `0.2f`) |
| `attackRange` | `float` | Distância mínima para iniciar ataque |
| `chaseSpeedMultiplier` | `float` | Multiplicador de velocidade durante perseguição (padrão: `1.5f`) |
| `invulnerabilityDuration` | `float` | Duração da invulnerabilidade após sofrer dano |
| `knockbackForce` | `float` | Força do knockback ao sofrer dano |
| `knockbackDuration` | `float` | Duração do efeito de knockback |
| `patrolPoints` | `Transform[]` ou `List<Transform>` | Lista de pontos de patrulha (Transforms vazios na cena) |
| `patrolWaitTime` | `float` | Tempo de espera (em segundos) ao chegar em cada ponto de patrulha |
| `patrolSpeed` | `float` | Velocidade durante patrulha (opcional, se diferente de `moveSpeed`) |
| `bouncingAmplitudeX` | `float` | Amplitude do movimento de bouncing no eixo X |
| `bouncingAmplitudeY` | `float` | Amplitude do movimento de bouncing no eixo Y |
| `bouncingFrequency` | `float` | Frequência (velocidade) do movimento de bouncing |

---

## 📋 Checklist de Implementação

### Fase 1: Estrutura Básica
- [ ] Adicionar campos de detecção (`detectionRadius`, `attackRange`, `detectionInterval`)
- [ ] Adicionar referências ao Animator
- [ ] Criar hashes para parâmetros do Animator (usando `Animator.StringToHash`)
- [ ] Adicionar referências à HurtBox e HitBox
- [ ] Implementar Gizmos de debug (`OnDrawGizmosSelected`)

### Fase 2: Sistema de Detecção e Estados
- [ ] Implementar máquina de estados (Patrulha/Combate)
- [ ] Implementar detecção 360° do Player com intervalo (`detectionInterval`)
- [ ] Verificar estado de stealth do Player
- [ ] Sistema de transição entre estados baseado em detecção
- [ ] Lógica de retorno para Patrulha quando Player sai do campo de visão

### Fase 3: Sistema de Movimento
- [ ] Implementar sistema de patrulha com pontos configuráveis
- [ ] Implementar lógica de seleção do próximo ponto de patrulha
- [ ] Implementar movimento suave com `Vector2.SmoothDamp` na patrulha
- [ ] Implementar tempo de espera em cada ponto (`patrolWaitTime`)
- [ ] Implementar deslocamento entre pontos de patrulha
- [ ] Implementar perseguição com velocidade aumentada (estado Combate)
- [ ] Implementar movimento visual de bouncing (eixos X e Y)
- [ ] Desabilitar bouncing durante animação de ataque
- [ ] Controlar parâmetro `isWalking` do Animator em ambos os estados
- [ ] Verificar distância para ataque durante estado Combate

### Fase 4: Sistema de Combate
- [ ] Implementar trigger de Attack quando ao alcance
- [ ] Implementar detecção de colisão com `PlayerAttack`
- [ ] Sistema de cálculo de dano
- [ ] Implementar knockback

### Fase 5: Sistema de Dano/Morte
- [ ] Implementar trigger `Hit` do Animator
- [ ] Sistema de invulnerabilidade temporária
- [ ] Implementar trigger `Die` quando HP <= 0
- [ ] Lógica de destruição do inimigo após animação de morte

### Fase 6: HitBox Control
- [ ] Implementar `EnableHitBox()` público
- [ ] Implementar `DisableHitBox()` público
- [ ] Configurar Animation Events para ativar/desativar HitBox

---

## 🎯 Fluxo de Estados

```text
PATRULHA (estado inicial)
  │
  ├─> [Detecção] Player no campo de visão + NÃO agachado
  │   └─> COMBATE
  │       │
  │       ├─> Walk (perseguição com velocidade 1.5x)
  │       │   ├─> [Ao alcance de ataque]
  │       │   │   └─> Attack (Animator trigger)
  │       │   │       └─> Retorna para Walk (perseguição)
  │       │   │
  │       │   └─> [Player sai do campo de visão OU entra em stealth]
  │       │       └─> Retorna para PATRULHA
  │       │
  │       └─> [Sofre Dano no estado Combate]
  │           └─> Hit (knockback + invulnerabilidade)
  │               ├─> HP > 0: Retorna para COMBATE ou PATRULHA
  │               └─> HP <= 0: Die (fim)
  │
  └─> [Sofre Dano no estado Patrulha]
      └─> Hit (knockback + invulnerabilidade)
          ├─> HP > 0: Retorna para PATRULHA
          └─> HP <= 0: Die (fim)
```

### Estados da Máquina

| Estado | Comportamento | Transições |
|--------|---------------|------------|
| **Patrulha** | Movimento em padrão predefinido | → Combate: Player detectado + não-stealth |
| **Combate** | Perseguição e ataque ao Player | → Patrulha: Player sai do campo ou entra em stealth |
| **Hit** | Knockback e invulnerabilidade | → Patrulha ou Combate: baseado no estado anterior |
| **Die** | Animação de morte | → Destruição do GameObject |

---

## 💡 Observações Técnicas

1. **Máquina de Estados**: Implementar usando enum `EnemyState { Patrol, Combat, Hit, Dead }` e switch/case no Update
2. **Patrulha**: Usar corrotina ou timer para controlar tempo de espera em cada ponto; armazenar índice do ponto atual
3. **Bouncing Visual**: Aplicar bouncing em transform filho (sprite) ou usar offset visual; não afetar transform raiz para evitar problemas de colisão
4. **Performance**: Usar `Physics2D.OverlapCircleNonAlloc` para detecção 360° e reusar array; implementar intervalo de detecção (0.2s) ao invés de checar todo frame
5. **Animator Hashing**: Cachear todos os parâmetros do Animator usando `Animator.StringToHash`
6. **Layer Masks**: Configurar LayerMask para otimizar detecção (apenas Player layer)
7. **Detecção de Stealth**: Acessar propriedade/método do PlayerController para verificar estado agachado
8. **Knockback**: Usar corrotina para controlar movimento de knockback
9. **Invulnerabilidade**: Usar timer ou corrotina para controlar duração
10. **HitBox**: Usar Collider2D desabilitado inicialmente, controlado por Animation Events
11. **Retorno para Patrulha**: Implementar lógica para retomar patrulha do ponto mais próximo após perder Player

---

## 📚 Referências de Código

### Exemplo: Detecção Otimizada com Intervalo
```csharp
[Header("Detection")]
[SerializeField] private float detectionRadius = 5f;
[SerializeField] private float detectionInterval = 0.2f;
[SerializeField] private LayerMask playerLayer;

private float detectionTimer = 0f;
private Collider2D[] detectionResults = new Collider2D[1];
private bool playerDetected = false;

private void Update()
{
    detectionTimer += Time.deltaTime;
    
    if (detectionTimer >= detectionInterval)
    {
        detectionTimer = 0f;
        CheckPlayerDetection();
    }
}

private void CheckPlayerDetection()
{
    int count = Physics2D.OverlapCircleNonAlloc(
        transform.position,
        detectionRadius,
        detectionResults,
        playerLayer
    );
    
    playerDetected = count > 0;
}
```

### Exemplo: Movimento Suave (Patrulha)
```csharp
[Header("Patrol")]
[SerializeField] private Transform[] patrolPoints;
[SerializeField] private float patrolWaitTime = 2f;
[SerializeField] private float smoothTime = 0.3f;

private int currentPatrolIndex = 0;
private float patrolWaitTimer = 0f;
private bool isWaitingAtPoint = false;
private Vector2 velocity = Vector2.zero;

private void UpdatePatrol()
{
    if (patrolPoints == null || patrolPoints.Length == 0) return;
    
    Transform targetPoint = patrolPoints[currentPatrolIndex];
    
    if (isWaitingAtPoint)
    {
        patrolWaitTimer += Time.deltaTime;
        if (patrolWaitTimer >= patrolWaitTime)
        {
            SelectNextPatrolPoint();
            isWaitingAtPoint = false;
            patrolWaitTimer = 0f;
        }
    }
    else
    {
        // Movimento suave com SmoothDamp ao invés de movimento direto
        Vector2 smoothPosition = Vector2.SmoothDamp(
            transform.position,
            targetPoint.position,
            ref velocity,
            smoothTime
        );
        
        transform.position = smoothPosition;
        
        // Checar distância ao quadrado para performance
        float sqrDistance = ((Vector2)targetPoint.position - (Vector2)transform.position).sqrMagnitude;
        if (sqrDistance < 0.01f) // 0.1 * 0.1
        {
            isWaitingAtPoint = true;
            animator.SetBool(IsWalking, false);
            velocity = Vector2.zero; // Resetar velocidade
        }
    }
}

private void SelectNextPatrolPoint()
{
    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
}
```

### Exemplo: Gizmos de Debug
```csharp
#if UNITY_EDITOR
private void OnDrawGizmosSelected()
{
    // Campo de visão (detecção)
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);
    
    // Alcance de ataque
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
    
    // Linha de patrulha
    if (patrolPoints != null && patrolPoints.Length > 1)
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            int next = (i + 1) % patrolPoints.Length;
            if (patrolPoints[i] != null && patrolPoints[next] != null)
            {
                Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[next].position);
                Gizmos.DrawWireSphere(patrolPoints[i].position, 0.2f);
            }
        }
    }
    
    // Indicador de estado atual
    if (Application.isPlaying)
    {
        Gizmos.color = currentState switch
        {
            EnemyState.Patrol => Color.green,
            EnemyState.Combat => Color.red,
            EnemyState.Hit => Color.white,
            EnemyState.Dead => Color.black,
            _ => Color.gray
        };
        Gizmos.DrawWireCube(transform.position + Vector3.up * 1.5f, Vector3.one * 0.3f);
    }
}
#endif
```

### Exemplo: Enum de Estados
```csharp
private enum EnemyState
{
    Patrol,
    Combat,
    Hit,
    Dead
}

private EnemyState currentState = EnemyState.Patrol;
```

### Exemplo: Animator Parameter Hashing
```csharp
private static readonly int IsWalking = Animator.StringToHash("isWalking");
private static readonly int Hit = Animator.StringToHash("Hit");
private static readonly int Die = Animator.StringToHash("Die");
private static readonly int Attack = Animator.StringToHash("Attack");
```

### Exemplo: Sistema de Bouncing
```csharp
[Header("Visual Bouncing")]
[SerializeField] private Transform visualTransform; // Transform do sprite/visual
[SerializeField] private float bouncingAmplitudeX = 0.1f;
[SerializeField] private float bouncingAmplitudeY = 0.15f;
[SerializeField] private float bouncingFrequency = 2f;

private bool isBouncingEnabled = true;
private Vector3 visualOffset;

private void Update()
{
    if (isBouncingEnabled && visualTransform != null)
    {
        float offsetX = Mathf.Sin(Time.time * bouncingFrequency) * bouncingAmplitudeX;
        float offsetY = Mathf.Cos(Time.time * bouncingFrequency * 1.3f) * bouncingAmplitudeY;
        
        visualOffset = new Vector3(offsetX, offsetY, 0f);
        visualTransform.localPosition = visualOffset;
    }
    else if (visualTransform != null)
    {
        visualTransform.localPosition = Vector3.zero;
    }
}

// Desabilitar bouncing durante ataque (chamar via Animation Event)
public void DisableBouncing()
{
    isBouncingEnabled = false;
    if (visualTransform != null)
        visualTransform.localPosition = Vector3.zero;
}

public void EnableBouncing()
{
    isBouncingEnabled = true;
}
```

### Exemplo: Sistema de Patrulha
```csharp
[Header("Patrol")]
[SerializeField] private Transform[] patrolPoints;
[SerializeField] private float patrolWaitTime = 2f;

private int currentPatrolIndex = 0;
private float patrolWaitTimer = 0f;
private bool isWaitingAtPoint = false;

private void UpdatePatrol()
{
    if (patrolPoints == null || patrolPoints.Length == 0) return;
    
    Transform targetPoint = patrolPoints[currentPatrolIndex];
    
    if (isWaitingAtPoint)
    {
        patrolWaitTimer += Time.deltaTime;
        if (patrolWaitTimer >= patrolWaitTime)
        {
            SelectNextPatrolPoint();
            isWaitingAtPoint = false;
            patrolWaitTimer = 0f;
        }
    }
    else
    {
        MoveTowards(targetPoint.position, moveSpeed);
        
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            isWaitingAtPoint = true;
            animator.SetBool(IsWalking, false);
        }
    }
}

private void SelectNextPatrolPoint()
{
    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
}
```

### Exemplo: HitBox Control
```csharp
[SerializeField] private Collider2D hitBox;

public void EnableHitBox() => hitBox.enabled = true;
public void DisableHitBox() => hitBox.enabled = false;
```

---

## ⚙️ Configuração de Layers e Physics

### Layers Necessários

Configurar os seguintes layers no projeto (Edit > Project Settings > Tags and Layers):

| Layer | Nome | Descrição | Usado Para |
|-------|------|-----------|------------|
| 6 | `Player` | Camada do jogador | Detecção do Player pelo inimigo |
| 7 | `Enemy` | Camada de inimigos | Colisão e separação de inimigos |
| 8 | `PlayerAttack` | Ataques do jogador | HurtBox detectar ataques |
| 9 | `EnemyAttack` | Ataques de inimigos | HitBox causar dano ao Player |

### Collision Matrix

Configurar interações de colisão (Edit > Project Settings > Physics 2D):

| Layer | Player | Enemy | PlayerAttack | EnemyAttack |
|-------|--------|-------|--------------|-------------|
| **Player** | ❌ | ✅ | ❌ | ✅ |
| **Enemy** | ✅ | ✅ | ✅ | ❌ |
| **PlayerAttack** | ❌ | ✅ | ❌ | ❌ |
| **EnemyAttack** | ✅ | ❌ | ❌ | ❌ |

### Configuração do Prefab BeeWorkerA

**GameObject Raiz** (`BeeWorkerA`):
- Layer: `Enemy`
- Collider2D: `CircleCollider2D` ou `CapsuleCollider2D`
- Rigidbody2D: `Body Type = Dynamic`, `Gravity Scale = 0`

**HurtBox** (filho):
- Layer: `Enemy` (herda da raiz)
- Collider2D: `CircleCollider2D` com `Is Trigger = true`
- Tag: Não necessária (script verifica tag do objeto colidido)

**HitBox** (filho):
- Layer: `EnemyAttack`
- Collider2D: `CapsuleCollider2D` com `Is Trigger = true`
- Enabled: `false` (ativado via Animation Events)

### LayerMask no Script

```csharp
[Header("Detection")]
[SerializeField] private LayerMask playerLayer = 1 << 6; // Layer 6 = Player

private void Awake()
{
    // Validar configuração
    if (playerLayer.value == 0)
    {
        Debug.LogError($"[{name}] PlayerLayer não configurado!");
    }
}
```

---

## 🛠️ Configuração e Testes

### Prefab Principal

- **Arquivo**: `Assets/_Prefabs/Characters/BeeWorkerA.prefab`
- **Responsabilidade**: Deve ser configurado para atender todos os requisitos descritos neste documento
- **Script**: `BeeWorkerBehaviorController.cs` anexado ao prefab
- **Hierarquia do Prefab**:
  - `BeeWorkerA` (raiz) - Contém Rigidbody2D, Collider2D principal, Animator e script
  - `Visual` (filho) - Transform para aplicar bouncing sem afetar colisão
  - `HurtBox` (filho) - Collider2D como trigger para detectar ataques do Player
  - `HitBox` (filho) - Collider2D desabilitado, ativado durante ataque via Animation Events

### Cena de Testes

- **Arquivo**: `Assets/_Scenes/Testes.unity`
- **Instâncias disponíveis**:
  - Prefab `BeeWorkerA` já instanciado na cena
  - Objeto `Player` disponível para testes de detecção e combate
- **Uso**: Utilize esta cena para validar comportamentos durante desenvolvimento
- **Configuração de Pontos de Patrulha**:
  - Criar GameObjects vazios na cena como pontos de patrulha
  - Arrastar para o array `patrolPoints` do BeeWorker no Inspector

### Requisitos de Performance

⚠️ **IMPORTANTE**: Seguir rigorosamente as diretrizes de performance do [CodingStandards.md](CodingStandards.md):

#### Regras Críticas

1. **NUNCA usar `GameObject.Find()` ou `FindObjectOfType()` em loops ou Update()**
   - Use cache estático para referência ao Player
   - Use referências serializadas sempre que possível

2. **Detecção de Player**:
   - Usar `Physics2D.OverlapCircleNonAlloc` para detecção 360°
   - Reutilizar array de resultados (não alocar a cada frame)
   - Configurar LayerMask para detectar apenas camada do Player

3. **Distâncias**:
   - Usar `sqrMagnitude` ao invés de `Distance()` para evitar raiz quadrada
   - Exemplo: `(target - position).sqrMagnitude < range * range`

4. **Animator**:
   - Cachear parâmetros usando `Animator.StringToHash` (static readonly)
   - Nunca usar strings diretamente em `SetBool()`, `SetTrigger()`, etc.

5. **Corrotinas**:
   - Reutilizar `WaitForSeconds` ao invés de criar novos a cada frame
   - Armazenar referências de corrotinas ativas para cancelamento

#### Exemplo de Cache de Player

```csharp
private static Transform s_playerTransform;
private static bool s_playerCached = false;

private void CachePlayerReference()
{
    if (!s_playerCached)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            s_playerTransform = playerObj.transform;
            s_playerCached = true;
        }
    }
}
```

#### Checklist de Performance

- [ ] Nenhum `Find` ou `FindObjectOfType` em `Update()` ou loops
- [ ] Usar `Physics2D.OverlapCircleNonAlloc` com array reutilizável
- [ ] Implementar intervalo de detecção (0.2s) ao invés de checar todo frame
- [ ] Todos os parâmetros do Animator em cache com `StringToHash`
- [ ] Usar `sqrMagnitude` para comparações de distância
- [ ] LayerMask configurado corretamente para detecção
- [ ] Layers do projeto configurados (Player, Enemy, PlayerAttack, EnemyAttack)
- [ ] Collision Matrix configurada corretamente no Physics 2D
- [ ] Bouncing aplicado em transform filho, não na raiz
- [ ] Corrotinas com `WaitForSeconds` reutilizáveis
- [ ] Gizmos de debug implementados para facilitar desenvolvimento

---

**Última Atualização**: 29/01/2026  
**Responsável**: Documentação técnica do projeto SlimeKing
