# The Slime King - Referência Rápida para Desenvolvedores

## 🎮 Mecânicas Core

### Movimentação

```csharp
// Velocidade base
float baseSpeed = 3.5f; // Filhote
float crouchSpeedMultiplier = 0.4f; // 40% quando agachado

// Agachar
bool IsCrouching => Input.GetButton("Crouch"); // B/Circle/B/Ctrl
```

### Stealth

```csharp
// Detecção
bool IsDetectable()
{
    if (IsCrouching && HasCoverBetween(enemy, player))
        return false;
    return true;
}
```

### Combate

```csharp
// Ataque básico
int baseDamage = 5; // Filhote
float attackCooldown = 0.5f;
float attackRange = 1.5f;

// Esquiva
float dashDistance = 3f;
float dashInvulnerability = 0.3f;
int dashStaminaCost = 5;
```

---

## 🤖 Estados de IA

### Inimigos

```csharp
public enum EnemyState
{
    Idle,        // Parado ou patrulha pequena
    Patrol,      // Movimento entre waypoints
    Alert,       // Detectou algo suspeito
    Investigate, // Investigando posição
    Chase,       // Perseguindo jogador
    Attack,      // Atacando
    Search,      // Procurando jogador perdido
    Return,      // Retornando à posição original
    Flee,        // Fugindo
    Stunned      // Atordoado
}
```

### Transições Comuns

```
Idle → Alert (detectou som/movimento)
Alert → Chase (confirmou jogador)
Chase → Attack (entrou em alcance)
Chase → Search (perdeu visão)
Search → Return (não encontrou)
```

### Percepção

```csharp
// Visão
float visionRange = 10f;
float visionAngle = 90f;

// Audição
float hearingRange = 5f;
float playerNoiseLevel = velocity.magnitude * (IsCrouching ? 0.3f : 1.0f);

// Proximidade (sentidos aguçados)
float proximityRange = 2f;
```

---

## 📋 Sistema de Quests

### Tipos de Objetivos

```csharp
// Collect
CollectObjective { itemID, requiredAmount, currentAmount }

// Defeat
DefeatObjective { enemyID, requiredKills, currentKills }

// Deliver
DeliverObjective { itemID, targetNPCID, delivered }

// Explore
ExploreObjective { locationID, discovered }

// Interact
InteractObjective { targetIDs[], interacted[] }

// Escort
EscortObjective { npcID, destinationID, npcReachedDestination, npcDied }
```

### Uso Básico

```csharp
// Aceitar quest
QuestManager.Instance.AcceptQuest(quest);

// Atualizar objetivo
QuestManager.Instance.UpdateObjective(questID, objectiveIndex);

// Completar quest
QuestManager.Instance.CompleteQuest(quest);
```

---

## 🎬 Cutscenes

### Tipos

```csharp
// Dialogue - Conversa simples
DialogueCutscene { lines[], skippable }

// Cinematic - Com movimento de câmera
CinematicCutscene { actions[], cutsceneCamera, skippable, skipDelay }

// Ritual - Reconhecimento de Rei Monstro
RitualCutscene { king, auraToGrant, crystalToGrant, titleToGrant }

// Discovery - Descoberta de área
DiscoveryCutscene { areaName, panoramaPoints[], duration }
```

### Trigger

```csharp
public class CutsceneTrigger : MonoBehaviour
{
    public Cutscene cutsceneToPlay;
    public bool playOnce = true;
    public bool requiresCondition = false;
    public string conditionID;
}
```

---

## 🧩 Puzzles

### Categorias

1. **Elementais:** Usam habilidades elementais
2. **Stealth:** Usam agachar e detecção
3. **Ambientais:** Interação com objetos
4. **Lógica:** Padrões e sequências
5. **Física:** Peso e momentum

### Template Básico

```csharp
public abstract class Puzzle : MonoBehaviour
{
    public bool isComplete = false;
    public List<PuzzleObjective> objectives;
    
    public abstract void CheckCompletion();
    public abstract void ResetPuzzle();
    public abstract void CompletePuzzle();
}
```

### Sistema de Dicas

```csharp
// Dica após 2 minutos
if (timeWithoutProgress > 120f && currentHintIndex == 0)
    ShowHint(0);

// Dica após 5 minutos
if (timeWithoutProgress > 300f && currentHintIndex == 1)
    ShowHint(1);
```

---

## 💎 Cristais

### Cristais Elementais (Moeda)

```csharp
public enum CrystalType
{
    Green,   // Nature
    Brown,   // Earth
    White,   // Air
    Blue,    // Water
    Red,     // Fire
    Purple,  // Shadow
    Cyan     // Ice
}

// Não ocupam slots de inventário
// Exibidos como contador na UI
```

### Cristais de Pacto (Colecionáveis)

```csharp
public class PactCrystal : ScriptableObject
{
    public string crystalID;
    public string crystalName;
    public Color primaryColor;
    public Color secondaryColor;
    public Sprite sprite;
    public string buffDescription;
    public float buffValue;
}

// Não ocupam slots de inventário
// Podem ser instalados na Câmara dos Pactos
```

---

## 🏠 Expansões do Lar

```csharp
public enum HomeExpansion
{
    MainCave,           // Inicial
    CrystalGarden,      // +1 cristal/dia
    InternalLake,       // +5 HP/s regen
    PanoramicAttic,     // Previsão climática
    PactChamber         // Buffs de cristais
}
```

---

## ⏰ Sistemas Temporais

### Ciclo Dia/Noite

```csharp
// 24 minutos reais = 1 dia
float dayDuration = 1440f; // segundos

public enum TimeOfDay
{
    Dawn,      // 05:00-06:59
    Morning,   // 07:00-11:59
    Afternoon, // 12:00-17:59
    Dusk,      // 18:00-19:59
    Night      // 20:00-04:59
}
```

### Ciclo Sazonal

```csharp
// 7 dias reais = 1 estação
float seasonDuration = 7f; // dias

public enum Season
{
    Spring,
    Summer,
    Autumn,
    Winter
}
```

---

## 🎯 Evolução do Slime

### Estágios

```csharp
public enum SlimeStage
{
    Hatchling,      // 16x16px, sem aura
    Adult,          // 24x24px, 1 aura, 4 habilidades
    GreatSlime,     // 32x32px, 3 auras, habilidades avançadas
    KingSlime,      // 40x40px, 5+ auras, maestria
    ImmortalKing    // 56x56px, 10 auras, transcendência
}
```

### Reputação (Invisível)

```csharp
// Ganho de reputação
int CompleteQuest = 10-50;
int SolvePuzzle = 15;
int DefeatBoss = 100;
int ReceiveRitual = 200;
int ExpandHome = 30;
int NewFriendshipLevel = 20;

// Níveis
int Unknown = 0-100;
int Noticed = 101-300;
int Respected = 301-600;
int Influential = 601-1000;
int Legendary = 1001+;
```

---

## 🎨 Biomas

```csharp
public enum Biome
{
    SlimeNest,      // Tutorial
    CalmForest,     // Nature - Rainha Melífera
    MirrorLake,     // Water - Imperador Escavarrok
    RockyArea,      // Earth - Conde Castoro
    MistSwamp,      // Shadow - Rainha Formicida
    LavaChambers,   // Fire - Sultan Escamífero
    SnowyPeak       // Air/Ice - Nictófila, Fulgorante
}
```

---

## 🔧 Configurações de Performance

### Targets

```csharp
// PC
int targetFPS_PC = 60;
Vector2Int resolution_PC = new Vector2Int(1920, 1080);

// Switch
int targetFPS_Switch_Portable = 30;
int targetFPS_Switch_Docked = 60;
```

### Otimizações

```csharp
// Object Pooling
ObjectPool<Projectile> projectilePool;
ObjectPool<Particle> particlePool;

// IA Update Rate
float aiUpdateInterval = 0.2f; // 5 vezes por segundo

// Pathfinding Cache
Dictionary<Vector2Int, List<Vector2Int>> pathCache;
```

---

## 📁 Estrutura de Passos

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs
│   │   ├── PlayerCombat.cs
│   │   └── PlayerAbilities.cs
│   ├── AI/
│   │   ├── AIController.cs
│   │   ├── AIState.cs (abstract)
│   │   ├── States/ (Idle, Patrol, etc.)
│   │   └── AIPerception.cs
│   ├── Quests/
│   │   ├── QuestManager.cs
│   │   ├── Quest.cs (ScriptableObject)
│   │   └── QuestObjective.cs (abstract)
│   ├── Puzzles/
│   │   ├── Puzzle.cs (abstract)
│   │   └── [Specific Puzzles]/
│   ├── Cutscenes/
│   │   ├── CutsceneManager.cs
│   │   ├── Cutscene.cs (abstract)
│   │   └── [Cutscene Types]/
│   └── Managers/
│       ├── GameManager.cs
│       ├── UIManager.cs
│       └── SaveManager.cs
├── Prefabs/
├── ScriptableObjects/
│   ├── Quests/
│   ├── AIProfiles/
│   ├── Items/
│   └── Abilities/
└── Scenes/
```

---

## 🐛 Debug Commands

```csharp
// Teleporte para bioma
TeleportToBiome(Biome biome);

// Adicionar cristais
AddCrystals(CrystalType type, int amount);

// Forçar evolução
ForceEvolution(SlimeStage stage);

// Completar quest
CompleteQuest(string questID);

// Desbloquear todas habilidades
UnlockAllAbilities();

// Modo God
ToggleGodMode();
```

---

**Versão:** 8.0  
**Para:** Unity 6.2 com URP  
**Última Atualização:** 2025
