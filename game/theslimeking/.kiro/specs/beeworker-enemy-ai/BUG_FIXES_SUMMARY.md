# Bug Fixes Summary - BeeWorker Combat System

**Data:** 2026-01-31
**Problemas Corrigidos:** 3 bugs críticos no sistema de combate

---

## 🐛 Problema 1: Abelhas não detectam player após reload da cena

### Sintoma
Após o player morrer e a cena ser recarregada, as abelhas ficam em Patrol mas nunca detectam o player novamente.

### Causa Raiz
O cache estático `s_playerCached` permanecia `true` após o reload da cena, mas o `s_playerTransform` ficava `null` porque o GameObject foi destruído. O código não tentava re-cachear a referência.

### Solução Implementada
**Arquivo:** `Assets/_Code/Gameplay/Enemies/BeeWorkerBehaviorController.cs`

```csharp
private void CachePlayerReference()
{
    // Re-cache if player reference is null (handles scene reloads)
    if (s_playerTransform == null)
    {
        s_playerCached = false;
    }

    // Only cache once across all instances
    if (!s_playerCached)
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject != null)
        {
            s_playerTransform = playerObject.transform;
            s_playerCached = true;
            
            if (enableDebugLogs)
            {
                Debug.Log("[BeeWorkerBehaviorController] Player reference cached successfully.", this);
            }
        }
        else
        {
            Debug.LogError("[BeeWorkerBehaviorController] Player GameObject not found! Ensure the player has the 'Player' tag assigned.", this);
        }
    }
}
```

**Resultado:** Abelhas agora detectam o player corretamente após reload da cena.

---

## 🐛 Problema 2: Abelhas atacam apenas uma vez e ficam em Idle

### Sintoma
Quando o player fica parado, as abelhas atacam uma única vez e depois ficam em estado Idle, sem atacar novamente até que o player as ataque.

### Causa Raiz
1. `OnTriggerEnter2D` só dispara uma vez quando o player entra no trigger
2. Não havia cooldown entre ataques
3. Não havia `OnTriggerStay2D` para ataques contínuos

### Solução Implementada
**Arquivo:** `Assets/_Code/Gameplay/Combat/EnemyHitBox.cs`

#### Mudanças:

1. **Adicionado sistema de cooldown:**
```csharp
[Header("Attack Settings")]
[SerializeField] private float attackCooldown = 1.5f;

private float lastAttackTime = -999f;
```

2. **Implementado OnTriggerStay2D:**
```csharp
private void OnTriggerStay2D(Collider2D other)
{
    // Continua tentando atacar enquanto o player estiver no range
    if (other.CompareTag("Player"))
    {
        TryAttackPlayer(other);
    }
}
```

3. **Criado método TryAttackPlayer com controle de cooldown:**
```csharp
private void TryAttackPlayer(Collider2D playerCollider)
{
    // Verifica cooldown
    if (Time.time - lastAttackTime < attackCooldown)
    {
        return;
    }

    // ... aplica dano ...

    // Atualiza o tempo do último ataque
    lastAttackTime = Time.time;
}
```

**Resultado:** Abelhas agora atacam continuamente enquanto o player estiver no range, respeitando o cooldown de 1.5s entre ataques.

---

## 🐛 Problema 3: Animação só muda quando o slime ataca

### Sintoma
As abelhas não transitam corretamente entre estados (Patrol → Combat → Attack). A animação só muda quando o player ataca a abelha.

### Causa Raiz
A flag `isAttacking` nunca era resetada após completar a animação de ataque. Ela só era resetada quando a abelha saía do range de ataque, causando um deadlock no estado de combate.

### Solução Implementada
**Arquivo:** `Assets/_Code/Gameplay/Enemies/BeeWorkerBehaviorController.cs`

Adicionado método público para ser chamado via Animation Event:

```csharp
/// <summary>
/// Resets the attacking state. Called via Animation Event when attack animation completes.
/// </summary>
public void OnAttackAnimationComplete()
{
    isAttacking = false;
    
    if (enableDebugLogs)
    {
        Debug.Log("[BeeWorkerBehaviorController] Attack animation complete. Ready for next attack.", this);
    }
}
```

**Resultado:** Abelhas agora podem executar múltiplos ataques consecutivos e transitar corretamente entre estados.

---

## ⚠️ Ação Necessária: Configurar Animation Event

Para que o Problema 3 seja completamente resolvido, é necessário adicionar um **Animation Event** na animação de ataque da abelha:

### Passos:

1. Abra o **Animation Window** no Unity
2. Selecione a animação de **Attack** do BeeWorker
3. Vá até o **último frame** da animação (ou logo antes do loop)
4. Clique no botão **Add Event** (ícone de marcador)
5. No Inspector, configure:
   - **Function:** `OnAttackAnimationComplete`
   - **Sem parâmetros**

### Localização da Animação:
- Procure em: `Assets/Art/Animations/` ou onde as animações do BeeWorker estão armazenadas
- Nome provável: `BeeWorker_Attack.anim` ou similar

---

## 📊 Resumo das Mudanças

| Arquivo | Mudanças | Linhas Modificadas |
|---------|----------|-------------------|
| `BeeWorkerBehaviorController.cs` | Cache de player + método OnAttackAnimationComplete | ~15 linhas |
| `EnemyHitBox.cs` | Sistema de cooldown + OnTriggerStay2D | ~40 linhas |

---

## ✅ Testes Recomendados

1. **Teste de Reload:**
   - Deixe o player morrer
   - Verifique se as abelhas detectam o player após reload
   - ✅ Esperado: Abelhas devem detectar e atacar normalmente

2. **Teste de Ataque Contínuo:**
   - Fique parado perto de uma abelha
   - Verifique se ela ataca múltiplas vezes
   - ✅ Esperado: Ataques a cada 1.5s enquanto no range

3. **Teste de Transição de Estados:**
   - Observe as abelhas patrulhando
   - Aproxime-se para ativar Combat
   - Fique no range de ataque
   - ✅ Esperado: Patrol → Combat → Attack → Combat (loop)

4. **Teste de HealthDisplay:**
   - Tome dano das abelhas
   - Verifique se os corações diminuem na UI
   - ✅ Esperado: UI atualiza em tempo real

---

## 🔧 Configurações Ajustáveis

### EnemyHitBox
- `attackCooldown` (1.5s padrão) - Tempo entre ataques consecutivos

### BeeWorkerBehaviorController
- `detectionRadius` (2f padrão) - Distância de detecção do player
- `attackRange` (1.5f padrão) - Distância para iniciar ataque
- `chaseSpeedMultiplier` (1.5f padrão) - Multiplicador de velocidade durante chase

---

## 📝 Notas Adicionais

### HealthDisplay
Também foi corrigido o sistema de UI de vida:
- `HealthDisplay.cs` agora se conecta automaticamente ao `PlayerAttributesHandler`
- Usa o evento `OnHealthChanged` para atualizar em tempo real
- Suporta busca automática do player na cena

### Performance
Todas as soluções mantêm as otimizações de performance:
- Cache estático de player (compartilhado entre instâncias)
- Detecção por intervalo (0.2s)
- Uso de `sqrMagnitude` para comparações de distância
- Animator parameter hashing

---

## 🎯 Status Final

| Bug | Status | Requer Ação Manual |
|-----|--------|-------------------|
| Player não detectado após reload | ✅ Corrigido | Não |
| Ataque único | ✅ Corrigido | Não |
| Animação travada | ⚠️ Parcialmente | Sim - Animation Event |
| HealthDisplay não atualiza | ✅ Corrigido | Não |

**Próximo Passo:** Adicionar Animation Event `OnAttackAnimationComplete` na animação de ataque do BeeWorker.
