# Sistema de Inimigos - Implementação Alpha

## 📋 Status

- **EnemyController.cs:** ✅ Esqueleto criado com FSM completa
- **Integração com AttackHandler:** 🔜 Pendente
- **Spawn System:** 🔜 Pendente
- **Drop Integration:** 🔜 Pendente

## 🎯 Objetivo

Criar sistema de inimigos que integra com AttackHandler e PlayerController existentes, sem modificar código existente.

## 🔧 Implementação

### Scripts Necessários (todos novos na Alpha/)

#### 1. AlphaEnemyIntegration.cs (NOVO)

```csharp
// Ponte entre AttackHandler existente e EnemyController novo
// Detecta colisão de "Attack" tag e aplica dano ao enemy
```

#### 2. EnemySpawner.cs (NOVO)

```csharp  
// Spawna inimigos na cena em pontos definidos
// Gerencia quantidade máxima e respawn
```

#### 3. AlphaEnemySetup.cs (NOVO)

```csharp
// Setup automático de inimigos na cena
// Configura componentes necessários para integração
```

### Fluxo de Integração

1. **Player Ataca (SEM MODIFICAR código existente)**

   ```
   PlayerController ataca →
   AttackHandler cria área de dano →
   AlphaEnemyIntegration detecta colisão →
   EnemyController.TakeDamage()
   ```

2. **Enemy Ataca Player**

   ```
   EnemyController proximidade do player →
   Enemy ataca →
   Detecta PlayerController tag →
   Aplica dano via PlayerAttributesSystem events
   ```

3. **Enemy Morre**

   ```
   EnemyController health <= 0 →
   Spawna drop items (usando sistema existente) →
   Destroy/disable enemy GameObject
   ```

## 📝 TODOs Específicos

### EnemyController.cs (completar TODOs existentes)

- [ ] Implementar FSM completa (Patrol, Chase, Attack, Hit, Death)
- [ ] Integração com Rigidbody2D para movimento
- [ ] Sistema de detecção de player (range-based)
- [ ] Attack patterns básicos

### AlphaEnemyIntegration.cs (criar novo)

- [ ] OnTriggerEnter2D para detectar "Attack" tag
- [ ] Ponte para EnemyController.TakeDamage()
- [ ] Feedback VFX quando recebe dano

### EnemySpawner.cs (criar novo)

- [ ] Spawn points configuráveis
- [ ] Limite máximo de inimigos simultâneos
- [ ] Respawn timer opcional

### AlphaEnemySetup.cs (criar novo)

- [ ] Auto-adiciona AlphaEnemyIntegration em EnemyController
- [ ] Configura layers de colisão
- [ ] Setup de referências necessárias

## 🔗 Pontos de Integração

### Com AttackHandler (NÃO MODIFICAR)

- AlphaEnemyIntegration detecta colisão com tag "Attack"
- Usa informações do AttackHandler para calcular dano

### Com PlayerController (NÃO MODIFICAR)

- EnemyController detecta player via tag "Player"
- Aplica dano ao player via PlayerAttributesSystem events

### Com Dropping Items (USAR EXISTENTE)

- Quando enemy morre, spawna drop usando sistema existente
- Integração via GameObject.Instantiate de drop prefabs

## ⚙️ Configuração na Cena

### Setup Automático via Extra Tools > Alpha

1. Cria EnemySpawner com spawn points
2. Configura EnemyBasic prefab
3. Adiciona AlphaEnemyIntegration automaticamente
4. Setup de layers de colisão

### Prefab EnemyBasic

- EnemyController component
- AlphaEnemyIntegration component
- Rigidbody2D + Collider2D
- SpriteRenderer + Animator
- Configurações de movimento e ataque

## 🧪 Teste de Validação

1. **Detecção:** Enemy detecta player e inicia chase
2. **Combate:** Player ataca enemy → enemy perde HP
3. **AI:** Enemy persegue e ataca player
4. **Morte:** Enemy morre e solta drop
5. **Performance:** Múltiplos enemies sem lag

## 📊 Métricas de Sucesso

- [ ] FSM transitions funcionando (Patrol → Chase → Attack)
- [ ] Recebe dano do AttackHandler sem modificações
- [ ] Aplica dano ao player via events
- [ ] Drop items ao morrer
- [ ] Performance estável com 5+ enemies
