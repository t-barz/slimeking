# Lista de Verificação de Diagnóstico - Sistema de Combate

**Data**: 2026-01-30  
**Status**: 🔍 Diagnóstico em Andamento

## Configuração Verificada ✅

### 1. PlayerController
- ✅ `attackPrefab` configurado: Attack01VFX (instanceID: 75332)
- ✅ `enableLogs` ativado para debug
- ✅ Código instancia o prefab corretamente
- ✅ Código chama `attackHandler.PerformAttack()`

### 2. Attack01VFX Prefab
- ✅ Tem componente AttackHandler
- ✅ `destructableLayerMask` = -1 (Everything)
- ✅ `enableDebugLogs` = true
- ✅ `showDebugGizmos` = true
- ✅ `attackSize` = (0.75, 0.4)
- ✅ `attackOffset` = (0, -0.15)

### 3. BeeWorkerA (Scene)
- ✅ Root: tag "Enemy", layer 11
- ✅ HurtBox: tag "Enemy", layer 0 (Default)
- ✅ HurtBox tem CircleCollider2D
- ✅ Apenas 1 BeeWorkerBehaviorController (duplicatas removidas)

### 4. PlayerSlime
- ✅ Tag "Player"
- ✅ Tem PlayerAttributesHandler
- ✅ baseAttack = 1

## Próximos Passos de Teste

### Teste 1: Verificar se o Ataque está sendo Executado
1. Entre em Play Mode
2. Pressione o botão de ataque
3. **Verifique no Console**:
   - Deve aparecer log do PlayerController sobre instanciação do ataque
   - Deve aparecer "AttackHandler: Ataque ... executado"

**Se NÃO aparecer**:
- O input não está sendo detectado
- O PlayerController não está executando o ataque
- Verificar configuração do Input System

### Teste 2: Verificar Detecção de Colliders
1. Execute o ataque próximo ao BeeWorker
2. **Verifique no Console**:
   - "AttackHandler: X objetos detectados"
   - "AttackHandler: Collider detectado: HurtBox, Tag: Enemy, Layer: 0"

**Se detectar 0 objetos**:
- O attackSize pode ser muito pequeno
- O attackOffset pode estar deslocado
- O HurtBox pode estar fora da área de detecção

**Se detectar objetos mas não o HurtBox**:
- Verificar se o HurtBox tem Collider2D ativo
- Verificar se o Collider2D está marcado como "Is Trigger"

### Teste 3: Verificar Aplicação de Dano
1. Se o HurtBox for detectado
2. **Verifique no Console**:
   - "AttackHandler: Dano aplicado ao inimigo HurtBox com ataque 1"
   - "[BeeWorkerBehaviorController] Receiving player attack: 1, Defense: 5, Final damage: 1"
   - "[BeeWorkerBehaviorController] Took 1 damage. Health: 2/3"

**Se NÃO aplicar dano**:
- GetComponentInParent não está encontrando o BeeWorkerBehaviorController
- Verificar hierarquia: HurtBox deve ser filho de BeeWorkerA

## Possíveis Problemas e Soluções

### Problema 1: Ataque não é executado
**Sintoma**: Nenhum log aparece ao pressionar o botão de ataque

**Causas Possíveis**:
- Input não configurado
- PlayerController desabilitado
- Cooldown de ataque ativo

**Solução**:
1. Verificar Input System Actions
2. Verificar se `_canAttack` está true
3. Verificar se não há erros de compilação

### Problema 2: AttackHandler não detecta nada
**Sintoma**: Log mostra "0 objetos detectados"

**Causas Possíveis**:
- attackSize muito pequeno
- attackOffset deslocado
- Attack01VFX instanciado longe do inimigo

**Solução**:
1. Aumentar attackSize para (1.5, 1.0)
2. Ajustar attackOffset
3. Verificar posição de instanciação do Attack01VFX
4. Habilitar showDebugGizmos para visualizar área de ataque

### Problema 3: Detecta mas não aplica dano
**Sintoma**: Log mostra collider detectado mas sem dano aplicado

**Causas Possíveis**:
- Tag do HurtBox não é "Enemy"
- GetComponentInParent não encontra BeeWorkerBehaviorController
- Namespace incorreto

**Solução**:
1. Verificar tag do HurtBox: deve ser "Enemy"
2. Verificar hierarquia: HurtBox deve ser filho direto de BeeWorkerA
3. Verificar se BeeWorkerBehaviorController está no GameObject pai

### Problema 4: HurtBox não tem Collider2D ativo
**Sintoma**: Nenhum collider detectado mesmo próximo ao inimigo

**Solução**:
1. Selecionar HurtBox no Hierarchy
2. Verificar se tem CircleCollider2D ou BoxCollider2D
3. Verificar se o Collider2D está enabled
4. Verificar se "Is Trigger" está marcado

## Comandos de Debug

### Verificar se Attack01VFX está sendo instanciado
```csharp
// No Console do Unity (durante Play Mode)
GameObject.Find("Attack01VFX(Clone)")
```

### Verificar posição do Attack01VFX
```csharp
GameObject.Find("Attack01VFX(Clone)").transform.position
```

### Verificar se HurtBox tem collider
```csharp
GameObject.Find("HurtBox").GetComponent<Collider2D>()
```

### Verificar health do BeeWorker
```csharp
GameObject.Find("BeeWorkerA").GetComponent<TheSlimeKing.Gameplay.BeeWorkerBehaviorController>()
```

## Ajustes Recomendados

### Se o ataque não alcançar o inimigo:

1. **Aumentar attackSize**:
   - Abrir Attack01VFX prefab
   - Modificar AttackHandler.attackSize para (1.5, 1.0)

2. **Ajustar attackOffset**:
   - Testar com (0, 0) primeiro
   - Depois ajustar conforme necessário

3. **Verificar posição de instanciação**:
   - PlayerController.attackInstantiationOffset
   - Pode estar muito longe do player

### Se o HurtBox não for detectado:

1. **Verificar Collider2D**:
   - Selecionar HurtBox no Inspector
   - Verificar se tem Collider2D
   - Verificar se está enabled
   - Verificar se "Is Trigger" está marcado

2. **Verificar Layer**:
   - HurtBox deve estar em uma layer incluída no LayerMask
   - Atualmente LayerMask = -1 (Everything), então qualquer layer funciona

## Status Atual

- ✅ Código está correto
- ✅ Configurações estão corretas
- ✅ Logs estão habilitados
- ⏳ Aguardando teste em Play Mode para diagnóstico

## Próxima Ação

**TESTE EM PLAY MODE** e verifique os logs no Console para identificar em qual etapa o sistema está falhando.
