# Diagrama de Arquitetura - Sistema de Combate Bidirecional

**Data**: 2026-01-31

## 🏗️ Visão Geral da Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                    SISTEMA DE COMBATE                            │
│                                                                   │
│  ┌──────────────────────┐         ┌──────────────────────┐     │
│  │   PLAYER → ENEMY     │         │   ENEMY → PLAYER     │     │
│  │      (Funciona)      │         │  (Fix Aplicado ✅)   │     │
│  └──────────────────────┘         └──────────────────────┘     │
└─────────────────────────────────────────────────────────────────┘
```

## 📊 Componentes e Relacionamentos

### Player → Enemy Combat Flow

```
┌─────────────────┐
│  PlayerSlime    │
│  ┌───────────┐  │
│  │ Tag:      │  │
│  │ "Player"  │  │
│  └───────────┘  │
│                 │
│  Components:    │
│  • PlayerController
│  • PlayerAttributesHandler (CurrentAttack = 1)
│  • CircleCollider2D (isTrigger: false)
│  • Rigidbody2D (Dynamic)
└────────┬────────┘
         │
         │ (1) Pressiona botão de ataque
         ↓
┌─────────────────┐
│ Attack01VFX     │
│ (Instanciado)   │
│                 │
│  Components:    │
│  • AttackHandler
│  • CapsuleCollider2D (isTrigger: true)
│  • destructableLayerMask: -1 (Everything)
└────────┬────────┘
         │
         │ (2) OnTriggerEnter2D detecta HurtBox
         ↓
┌─────────────────┐
│  BeeWorkerA     │
│  ┌───────────┐  │
│  │ HurtBox   │  │ ← (3) Collider detectado
│  │ Tag:      │  │
│  │ "Enemy"   │  │
│  └───────────┘  │
│                 │
│  Components:    │
│  • BeeWorkerBehaviorController
│    - defense: 5
│    - maxHealth: 3
│  • Rigidbody2D (Kinematic) ✅ FIX
└────────┬────────┘
         │
         │ (4) TakeDamageFromPlayer(playerAttack: 1)
         │ (5) Calcula: max(1, 1 - 5) = 1
         │ (6) TakeDamage(1)
         ↓
    Health: 3 → 2 → 1 → 0 (morte)
```

### Enemy → Player Combat Flow

```
┌─────────────────┐
│  BeeWorkerA     │
│                 │
│  Components:    │
│  • BeeWorkerBehaviorController
│    - attackDamage: 10
│    - attackRange: 1.5
│    - detectionRadius: 2.0
│  • Animator
│  • Rigidbody2D (Kinematic) ✅ FIX CRÍTICO
└────────┬────────┘
         │
         │ (1) Detecta player (CheckPlayerDetection)
         │ (2) Entra em Combat State
         │ (3) Aproxima-se do player
         │ (4) Dentro de attackRange → Trigger "Attack"
         ↓
┌─────────────────┐
│  Animation      │
│  "Attack"       │
│                 │
│  Events:        │
│  • EnableHitBox()  ← Frame X
│  • DisableHitBox() ← Frame Y
└────────┬────────┘
         │
         │ (5) EnableHitBox() chamado
         ↓
┌─────────────────┐
│  HitBox         │
│  (filho)        │
│                 │
│  Components:    │
│  • CapsuleCollider2D
│    - isTrigger: true
│    - enabled: false → true (via Animation Event)
│  • EnemyHitBox
│    - enableDebugLogs: true
└────────┬────────┘
         │
         │ (6) OnTriggerEnter2D detecta Player
         │     ↓
         │     Requer: Rigidbody2D no pai (BeeWorker) ✅
         │     Requer: Player com tag "Player" ✅
         │     Requer: Collider2D com isTrigger: true ✅
         │
         │ (7) Verifica tag "Player"
         │ (8) GetAttackDamage() → BeeWorkerBehaviorController
         ↓
┌─────────────────┐
│  PlayerSlime    │
│  ┌───────────┐  │
│  │ Tag:      │  │
│  │ "Player"  │  │
│  └───────────┘  │
│                 │
│  Components:    │
│  • PlayerAttributesHandler
│    - baseHealthPoints: 3
│    - baseDefense: 0
│  • CircleCollider2D (isTrigger: false)
│  • Rigidbody2D (Dynamic)
└────────┬────────┘
         │
         │ (9) TakeDamage(attackDamage: 10, ignoreDefense: false)
         │ (10) Calcula redução: (0 * 100) / (0 + 100) = 0%
         │ (11) finalDamage = 10 - (10 * 0 / 100) = 10
         │ (12) currentHealthPoints -= 10
         │ (13) Animator trigger "Hit"
         ↓
    Health: 3 → -7 (morte em 1 hit)
```

## 🔑 Componentes Chave

### 1. Rigidbody2D (CRÍTICO)

```
BeeWorkerA (Root)
├── Rigidbody2D ✅ FIX APLICADO
│   ├── bodyType: Kinematic
│   ├── gravityScale: 0
│   ├── freezeRotation: true
│   └── simulated: true
│
├── HitBox (filho)
│   ├── CapsuleCollider2D (isTrigger: true)
│   └── EnemyHitBox
│       └── OnTriggerEnter2D ← Funciona porque pai tem Rigidbody2D!
│
└── HurtBox (filho)
    ├── CapsuleCollider2D (isTrigger: true)
    └── Tag: "Enemy"
```

**Por que Kinematic?**
- ✅ Movimento controlado por script (BeeWorkerBehaviorController)
- ✅ Não afetado por gravidade ou forças físicas
- ✅ Não colide fisicamente com outros objetos
- ✅ Triggers funcionam perfeitamente
- ✅ Ideal para inimigos controlados por IA

### 2. EnemyHitBox Component

```csharp
namespace SlimeKing.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class EnemyHitBox : MonoBehaviour
    {
        // Detecta colisões com player
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // Obtém ataque do pai
                int enemyAttack = beeWorkerController.GetAttackDamage();
                
                // Aplica dano ao player
                playerAttributes.TakeDamage(enemyAttack, false);
            }
        }
    }
}
```

### 3. AttackHandler Component

```csharp
namespace SlimeKing.Gameplay
{
    public class AttackHandler : MonoBehaviour
    {
        private void PerformAttack()
        {
            // Detecta colliders na área de ataque
            int hitCount = Physics2D.OverlapCircleNonAlloc(...);
            
            foreach (var collider in results)
            {
                // Detecta inimigos
                if (collider.CompareTag("Enemy"))
                {
                    // Obtém ataque do player
                    int playerAttack = playerAttributes.CurrentAttack;
                    
                    // Aplica dano ao inimigo
                    enemy.TakeDamageFromPlayer(playerAttack);
                }
            }
        }
    }
}
```

## 🔄 Estados e Transições

### BeeWorker State Machine

```
┌─────────┐
│ Patrol  │ ← Estado inicial
└────┬────┘
     │
     │ Player detectado (dentro de detectionRadius)
     ↓
┌─────────┐
│ Combat  │
└────┬────┘
     │
     ├─→ Player dentro de attackRange → Ataca
     │
     ├─→ Player fora de attackRange → Chase
     │
     └─→ Player perdido ou em stealth → Volta para Patrol
     
     │ Recebe dano
     ↓
┌─────────┐
│   Hit   │ ← Knockback + Invulnerabilidade
└────┬────┘
     │
     │ Knockback completo
     ↓
Retorna para estado anterior (Patrol ou Combat)

     │ Health <= 0
     ↓
┌─────────┐
│  Dead   │ ← Animação de morte → Destroy
└─────────┘
```

## 📐 Ranges e Detecção

```
                    Player
                      ●
                      │
                      │
        ┌─────────────┼─────────────┐
        │             │             │
        │   ┌─────────┼─────────┐   │
        │   │         │         │   │
        │   │    BeeWorker      │   │
        │   │         ●         │   │
        │   │                   │   │
        │   └───────────────────┘   │
        │   Attack Range: 1.5       │
        │                           │
        └───────────────────────────┘
        Detection Radius: 2.0

Comportamento:
• Fora de 2.0: Patrol state
• Entre 1.5 e 2.0: Combat state (chase)
• Dentro de 1.5: Combat state (attack)
```

## 🎯 Requisitos do Unity para Triggers

### Configuração Mínima para OnTriggerEnter2D

```
GameObject A (BeeWorker)
├── Rigidbody2D ✅ OBRIGATÓRIO
│   └── Qualquer tipo (Dynamic, Kinematic, Static)
│
└── HitBox (filho)
    └── Collider2D
        └── isTrigger: true ✅ OBRIGATÓRIO

GameObject B (Player)
├── Rigidbody2D ✅ OBRIGATÓRIO (pelo menos um dos dois)
│   └── Qualquer tipo
│
└── Collider2D
    └── isTrigger: false ou true

Resultado: OnTriggerEnter2D é chamado! ✅
```

### Configuração Incorreta (Antes do Fix)

```
GameObject A (BeeWorker)
├── ❌ SEM Rigidbody2D ← PROBLEMA!
│
└── HitBox (filho)
    └── Collider2D
        └── isTrigger: true

GameObject B (Player)
├── Rigidbody2D ✅
│
└── Collider2D
    └── isTrigger: false

Resultado: OnTriggerEnter2D NÃO é chamado! ❌
```

## 📊 Fluxo de Dados

### Dano: Player → BeeWorker

```
PlayerAttributesHandler.CurrentAttack (1)
    ↓
AttackHandler.PerformAttack()
    ↓
BeeWorkerBehaviorController.TakeDamageFromPlayer(1)
    ↓
Calcula: max(1, 1 - 5) = 1
    ↓
BeeWorkerBehaviorController.TakeDamage(1)
    ↓
currentHealth -= 1
    ↓
Health: 3 → 2 → 1 → 0
```

### Dano: BeeWorker → Player

```
BeeWorkerBehaviorController.attackDamage (10)
    ↓
Animation Event: EnableHitBox()
    ↓
EnemyHitBox.OnTriggerEnter2D(Player)
    ↓
BeeWorkerBehaviorController.GetAttackDamage() → 10
    ↓
PlayerAttributesHandler.TakeDamage(10, false)
    ↓
Calcula redução: (0 * 100) / (0 + 100) = 0%
    ↓
finalDamage = 10 - (10 * 0 / 100) = 10
    ↓
currentHealthPoints -= 10
    ↓
Health: 3 → -7 (morte)
```

## 🔍 Debug e Troubleshooting

### Checklist de Verificação

```
┌─────────────────────────────────────────┐
│ OnTriggerEnter2D não é chamado?         │
├─────────────────────────────────────────┤
│                                          │
│ 1. ✅ BeeWorker tem Rigidbody2D?        │
│    └─→ Adicionar se não tiver           │
│                                          │
│ 2. ✅ Rigidbody2D é Kinematic?          │
│    └─→ Configurar bodyType               │
│                                          │
│ 3. ✅ HitBox tem isTrigger: true?       │
│    └─→ Ativar isTrigger                 │
│                                          │
│ 4. ✅ HitBox está ativo?                │
│    └─→ Verificar Animation Events       │
│                                          │
│ 5. ✅ Player tem tag "Player"?          │
│    └─→ Configurar tag                   │
│                                          │
│ 6. ✅ Layers podem colidir?             │
│    └─→ Verificar Physics2D matrix       │
│                                          │
└─────────────────────────────────────────┘
```

### Logs Esperados (Sistema Funcionando)

```
[BeeWorkerBehaviorController] Player detected at distance 1.23
[BeeWorkerBehaviorController] Transitioning from Patrol to Combat
[BeeWorkerBehaviorController] Chasing player at speed 4.50
[BeeWorkerBehaviorController] Triggering attack at distance 1.45
[BeeWorkerBehaviorController] HitBox enabled
[EnemyHitBox] OnTriggerEnter2D chamado! ✅
[EnemyHitBox] Player detectado! GameObject: PlayerSlime ✅
[EnemyHitBox] Dano aplicado ao player: 10 ✅
[BeeWorkerBehaviorController] HitBox disabled
```

## 🎓 Conclusão

O sistema de combate bidirecional está completamente implementado e funcional. O fix crítico (Rigidbody2D) foi aplicado, permitindo que os triggers funcionem corretamente.

**Componentes Essenciais**:
1. ✅ Rigidbody2D (Kinematic) no BeeWorker
2. ✅ EnemyHitBox para detecção de colisões
3. ✅ AttackHandler para ataques do player
4. ✅ Animation Events para ativar/desativar HitBox
5. ✅ Tags corretas ("Player", "Enemy")

**Pronto para**: Testes, balanceamento e expansão! 🚀

