# Sistema de Dano: Inimigo → Player

**Data**: 2026-01-31  
**Status**: ✅ Implementado e Configurado

## Resumo

Implementado sistema completo de dano do BeeWorker para o PlayerSlime. Quando o BeeWorker ataca e o HitBox colide com o player, o dano é aplicado automaticamente.

## Arquivos Criados/Modificados

### 1. Novo Script: EnemyHitBox.cs ✅
**Localização**: `Assets/_Code/Gameplay/Combat/EnemyHitBox.cs`

**Funcionalidade**:
- Detecta colisões do HitBox do inimigo com o player
- Usa OnTriggerEnter2D para detectar quando o HitBox (ativo) colide com objetos tag "Player"
- Obtém o valor de ataque do BeeWorkerBehaviorController pai
- Aplica dano usando PlayerAttributesHandler.TakeDamage()

**Características**:
- RequireComponent(typeof(Collider2D)) - garante que há um collider
- Valida automaticamente se o collider é trigger
- Obtém referência ao BeeWorkerBehaviorController no Awake
- Logs de debug habilitados por padrão

### 2. BeeWorkerBehaviorController.cs - Método Adicionado ✅
**Localização**: `Assets/_Code/Gameplay/Enemies/BeeWorkerBehaviorController.cs`

**Novo Método**:
```csharp
/// <summary>
/// Returns the attack damage value of this BeeWorker.
/// Used by EnemyHitBox to apply damage to the player.
/// </summary>
/// <returns>The attack damage value as an integer</returns>
public int GetAttackDamage()
{
    return Mathf.RoundToInt(attackDamage);
}
```

**Propósito**: Permite que o EnemyHitBox acesse o valor de attackDamage (que é privado) para aplicar o dano correto ao player.

## Configuração na Cena

### BeeWorkerA (Scene Instance)

**Hierarquia**:
```
BeeWorkerA (Root)
├── HurtBox (recebe dano do player)
├── HitBox (aplica dano ao player) ← CONFIGURADO
└── Visual
```

**HitBox GameObject**:
- ✅ Tag: "Untagged" (não precisa de tag específica)
- ✅ CapsuleCollider2D:
  - `isTrigger`: true
  - `size`: (0.5, 1.0)
  - `enabled`: false (ativado via Animation Event)
- ✅ **Componente EnemyHitBox** (NOVO):
  - `enableDebugLogs`: true

**BeeWorkerA Root**:
- ✅ BeeWorkerBehaviorController:
  - `attackDamage`: 10 (valor padrão)
  - `defense`: 5
  - `maxHealth`: 3

### PlayerSlime

**Requisitos** (já configurados):
- ✅ Tag: "Player"
- ✅ PlayerAttributesHandler:
  - `baseHealthPoints`: 3
  - `baseDefense`: 0
  - Método TakeDamage() disponível

## Como Funciona

### Fluxo de Execução:

1. **BeeWorker entra em Combat State**
   - Detecta o player
   - Aproxima-se até attackRange

2. **BeeWorker executa ataque**
   - Animator trigger "Attack" é acionado
   - Animação de ataque começa

3. **Animation Event: EnableHitBox()**
   - BeeWorkerBehaviorController.EnableHitBox() é chamado
   - HitBox Collider2D é ativado (enabled = true)

4. **Detecção de Colisão**
   - EnemyHitBox.OnTriggerEnter2D() detecta colisão com player
   - Verifica se o collider tem tag "Player"

5. **Aplicação de Dano**
   - EnemyHitBox obtém attackDamage via GetAttackDamage()
   - Obtém PlayerAttributesHandler do player
   - Chama PlayerAttributesHandler.TakeDamage(attackDamage, false)

6. **Cálculo de Dano no Player**
   - PlayerAttributesHandler calcula redução de dano baseada na defesa
   - Fórmula: `damageReduction = (defense * 100) / (defense + 100)`
   - Aplica dano final ao currentHealthPoints
   - Dispara evento OnHealthChanged

7. **Animation Event: DisableHitBox()**
   - BeeWorkerBehaviorController.DisableHitBox() é chamado
   - HitBox Collider2D é desativado (enabled = false)

## Valores de Dano

### BeeWorker → Player

**Ataque Base do BeeWorker**: 10

**Defesa do Player**: 0 (padrão)

**Cálculo**:
```
damageReduction = (0 * 100) / (0 + 100) = 0%
finalDamage = 10 - (10 * 0 / 100) = 10
```

**Resultado**: Player perde 10 HP por ataque (com defesa 0)

### Player → BeeWorker (já implementado)

**Ataque Base do Player**: 1

**Defesa do BeeWorker**: 5

**Cálculo**:
```
finalDamage = max(1, 1 - 5) = 1
```

**Resultado**: BeeWorker perde 1 HP por ataque

## Logs Esperados

Quando o BeeWorker atacar o player, você verá no Console:

```
[BeeWorkerBehaviorController] Triggering attack at distance X.XX
[EnemyHitBox] Dano aplicado ao player: 10
```

Se o PlayerAttributesHandler tiver logs habilitados, também verá:
```
(Logs do PlayerAttributesHandler sobre dano recebido)
```

## Testes Recomendados

### Teste 1: Dano Básico
1. Abrir cena Testes.unity
2. Entrar em Play Mode
3. Aproximar PlayerSlime do BeeWorkerA
4. Aguardar BeeWorker atacar
5. **Verificar**:
   - HitBox é ativado durante animação
   - Player recebe dano
   - Health do player diminui
   - Logs aparecem no Console

### Teste 2: Múltiplos Ataques
1. Deixar BeeWorker atacar várias vezes
2. **Verificar**:
   - Cada ataque aplica dano
   - Health do player diminui progressivamente
   - Player morre após perder toda a vida

### Teste 3: Invulnerabilidade (se implementada)
1. Se o player tiver sistema de invulnerabilidade após dano
2. **Verificar**:
   - Primeiro ataque aplica dano
   - Ataques subsequentes durante invulnerabilidade são ignorados

## Ajustes de Balanceamento

### Para reduzir dano do BeeWorker:

1. Selecionar BeeWorkerA no Hierarchy
2. No Inspector, BeeWorkerBehaviorController
3. Modificar `attackDamage`:
   - Atual: 10
   - Sugestão: 5 ou 3

### Para aumentar defesa do Player:

1. Selecionar PlayerSlime no Hierarchy
2. No Inspector, PlayerAttributesHandler
3. Modificar `baseDefense`:
   - Atual: 0
   - Sugestão: 2 ou 5

### Fórmula de Redução de Dano:

```
damageReduction% = (defense * 100) / (defense + 100)
finalDamage = attackDamage - (attackDamage * damageReduction% / 100)
```

**Exemplos**:
- Defense 0: 0% redução
- Defense 5: 4.76% redução
- Defense 10: 9.09% redução
- Defense 20: 16.67% redução
- Defense 50: 33.33% redução
- Defense 100: 50% redução

## Possíveis Melhorias Futuras

1. **Knockback no Player**
   - Adicionar knockback quando player recebe dano
   - Similar ao knockback do BeeWorker

2. **Invulnerabilidade Temporária**
   - Player fica invulnerável por X segundos após receber dano
   - Evita dano contínuo em múltiplas colisões

3. **Feedback Visual**
   - Flash branco/vermelho no sprite do player
   - Shake da câmera
   - Partículas de impacto

4. **Feedback de Áudio**
   - Som de dano ao player
   - Som de morte do player

5. **UI de Health**
   - Barra de vida que diminui visualmente
   - Indicador de dano recebido

## Arquitetura do Sistema

```
BeeWorker Attack Animation
    ↓
Animation Event: EnableHitBox()
    ↓
HitBox Collider2D enabled = true
    ↓
OnTriggerEnter2D (EnemyHitBox)
    ↓
Detecta tag "Player"
    ↓
GetAttackDamage() → BeeWorkerBehaviorController
    ↓
PlayerAttributesHandler.TakeDamage(damage)
    ↓
Calcula redução de dano
    ↓
Aplica dano ao currentHealthPoints
    ↓
Dispara evento OnHealthChanged
    ↓
Animation Event: DisableHitBox()
    ↓
HitBox Collider2D enabled = false
```

## Conclusão

O sistema de dano do inimigo para o player está completamente implementado e configurado. O BeeWorker agora pode causar dano ao PlayerSlime durante seus ataques, criando um sistema de combate bidirecional funcional.

**Status**: ✅ Pronto para teste e balanceamento
