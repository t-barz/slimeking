# Checklist de Testes - Sistema de Combate Bidirecional

**Data**: 2026-01-31  
**Scene**: Testes.unity

## ✅ Pré-requisitos Verificados

### BeeWorkerA Configuration
- [x] Rigidbody2D (Kinematic) presente
- [x] HitBox com CapsuleCollider2D (isTrigger: true)
- [x] HitBox com EnemyHitBox component
- [x] HurtBox com tag "Enemy"
- [x] BeeWorkerBehaviorController configurado

### PlayerSlime Configuration
- [x] Tag "Player"
- [x] CircleCollider2D (isTrigger: false)
- [x] Rigidbody2D (Dynamic)
- [x] PlayerAttributesHandler configurado
- [x] PlayerController configurado

## 🧪 Testes a Executar

### Teste 1: Player Ataca BeeWorker ✅ (já funcionava)

**Passos**:
1. Entrar em Play Mode
2. Aproximar player do BeeWorker
3. Pressionar botão de ataque (Space/Gamepad)

**Resultado Esperado**:
- [ ] Attack01VFX é instanciado
- [ ] BeeWorker entra em Hit state
- [ ] BeeWorker recebe 1 de dano
- [ ] Health do BeeWorker diminui (3 → 2 → 1 → 0)
- [ ] BeeWorker morre após 3 ataques

**Logs Esperados**:
```
AttackHandler: Ataque frontal executado, 1 objetos detectados
AttackHandler: Collider detectado: HurtBox, Tag: Enemy
AttackHandler: Dano aplicado ao inimigo HurtBox com ataque 1
[BeeWorkerBehaviorController] Receiving player attack: 1, Defense: 5, Final damage: 1
[BeeWorkerBehaviorController] Took 1 damage. Health: 2/3
```

---

### Teste 2: BeeWorker Ataca Player ⚠️ (FIX APLICADO - TESTAR)

**Passos**:
1. Entrar em Play Mode
2. Aproximar player do BeeWorker
3. Aguardar BeeWorker detectar e atacar

**Resultado Esperado**:
- [ ] BeeWorker entra em Combat state
- [ ] BeeWorker se aproxima do player
- [ ] BeeWorker executa animação de ataque
- [ ] HitBox é ativado durante animação
- [ ] **OnTriggerEnter2D é chamado** ← CRÍTICO
- [ ] Player recebe 10 de dano
- [ ] Health do player diminui
- [ ] Animator do player executa trigger "Hit"

**Logs Esperados**:
```
[BeeWorkerBehaviorController] Player detected at distance X.XX
[BeeWorkerBehaviorController] Transitioning from Patrol to Combat - player detected
[BeeWorkerBehaviorController] Chasing player at speed X.XX
[BeeWorkerBehaviorController] Triggering attack at distance X.XX
[BeeWorkerBehaviorController] HitBox enabled
[EnemyHitBox] OnTriggerEnter2D chamado! Collider: PlayerSlime, Tag: Player
[EnemyHitBox] Player detectado! GameObject: PlayerSlime
[EnemyHitBox] Dano aplicado ao player: 10
[BeeWorkerBehaviorController] HitBox disabled
```

**Se OnTriggerEnter2D NÃO for chamado**:
- ❌ Verificar se BeeWorker tem Rigidbody2D
- ❌ Verificar se Rigidbody2D está configurado como Kinematic
- ❌ Verificar se HitBox Collider2D tem isTrigger: true
- ❌ Verificar se Player tem tag "Player"
- ❌ Verificar Physics2D collision matrix

---

### Teste 3: Combate Completo

**Passos**:
1. Entrar em Play Mode
2. Trocar ataques entre player e BeeWorker

**Resultado Esperado**:
- [ ] Player pode atacar BeeWorker
- [ ] BeeWorker pode atacar Player
- [ ] Ambos recebem dano corretamente
- [ ] BeeWorker morre após 3 ataques do player
- [ ] Player morre após 1 ataque do BeeWorker (com health 3 e dano 10)

---

### Teste 4: Múltiplos Ataques do BeeWorker

**Passos**:
1. Entrar em Play Mode
2. Deixar BeeWorker atacar várias vezes
3. Não atacar de volta

**Resultado Esperado**:
- [ ] Cada ataque aplica dano
- [ ] Health do player diminui progressivamente
- [ ] Player morre após perder toda a vida
- [ ] Sistema funciona consistentemente

---

### Teste 5: Detecção e Stealth (se implementado)

**Passos**:
1. Entrar em Play Mode
2. Aproximar player do BeeWorker
3. Ativar stealth (se disponível)

**Resultado Esperado**:
- [ ] BeeWorker detecta player quando não está em stealth
- [ ] BeeWorker NÃO detecta player quando está em stealth
- [ ] BeeWorker retorna a Patrol state quando player entra em stealth

---

## 🐛 Troubleshooting

### Problema: OnTriggerEnter2D não é chamado

**Verificar**:
1. BeeWorker tem Rigidbody2D? → Adicionar se não tiver
2. Rigidbody2D é Kinematic? → Configurar bodyType
3. HitBox Collider2D tem isTrigger: true? → Ativar isTrigger
4. HitBox está ativo durante ataque? → Verificar Animation Events
5. Player tem tag "Player"? → Configurar tag
6. Layers podem colidir? → Verificar Physics2D collision matrix

### Problema: Dano não é aplicado

**Verificar**:
1. OnTriggerEnter2D é chamado? → Ver logs
2. PlayerAttributesHandler existe? → Verificar component
3. GetAttackDamage() retorna valor correto? → Ver logs
4. TakeDamage() é chamado? → Ver logs

### Problema: BeeWorker não ataca

**Verificar**:
1. BeeWorker detecta player? → Ver logs de detecção
2. BeeWorker entra em Combat state? → Ver logs de transição
3. Distância está dentro de attackRange? → Ver logs de chase
4. Animator trigger "Attack" é acionado? → Ver Animator window

---

## 📊 Valores de Referência

### Dano
- **Player → BeeWorker**: 1 dano (após defesa)
- **BeeWorker → Player**: 10 dano (sem defesa)

### Health
- **Player**: 3 HP (padrão)
- **BeeWorker**: 3 HP

### Defesa
- **Player**: 0 (padrão)
- **BeeWorker**: 5

### Ranges
- **Detection Radius**: 2.0
- **Attack Range**: 1.5

---

## ✅ Critérios de Sucesso

O sistema está funcionando corretamente quando:

1. ✅ Player pode atacar e causar dano ao BeeWorker
2. ✅ BeeWorker pode atacar e causar dano ao Player
3. ✅ OnTriggerEnter2D é chamado quando HitBox colide com Player
4. ✅ Dano é calculado e aplicado corretamente
5. ✅ Ambos os personagens morrem quando health chega a 0
6. ✅ Logs aparecem no Console confirmando cada etapa
7. ✅ Sistema funciona consistentemente em múltiplos testes

---

## 📝 Notas de Teste

**Data do Teste**: ___________

**Testador**: ___________

**Resultados**:
- [ ] Teste 1: Player Ataca BeeWorker - PASSOU / FALHOU
- [ ] Teste 2: BeeWorker Ataca Player - PASSOU / FALHOU
- [ ] Teste 3: Combate Completo - PASSOU / FALHOU
- [ ] Teste 4: Múltiplos Ataques - PASSOU / FALHOU
- [ ] Teste 5: Detecção e Stealth - PASSOU / FALHOU

**Problemas Encontrados**:
___________________________________________
___________________________________________
___________________________________________

**Ajustes Necessários**:
___________________________________________
___________________________________________
___________________________________________

