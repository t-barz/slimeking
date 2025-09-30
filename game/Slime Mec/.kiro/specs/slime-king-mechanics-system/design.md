# Design Document

## Overview

Este documento define a arquitetura técnica para implementar os 28 mecanismos pendentes do The Slime King, integrando-os com o código Unity 6.3 existente. O design segue os padrões arquiteturais já estabelecidos no projeto, utilizando componentes modulares, sistemas baseados em eventos e integração com o Input System moderno do Unity.

A arquitetura proposta mantém a separação de responsabilidades existente, com scripts organizados por namespace (SlimeMec.Gameplay, SlimeMec.Visual) e seguindo o padrão de não usar Singletons para classes Player*, conforme diretrizes do projeto.

## Architecture

### Core Architecture Principles

1. **Component-Based Design**: Cada mecanismo é implementado como componente Unity independente
2. **Event-Driven Communication**: Sistemas se comunicam via eventos para baixo acoplamento
3. **Modular Integration**: Novos sistemas se integram com existentes via interfaces bem definidas
4. **Performance-First**: Uso de object pooling, caching e otimizações para 60+ FPS
5. **Extensible Framework**: Arquitetura preparada para expansões futuras

### System Integration Map

```
PlayerController (Existing)
├── MovementSystem (New) → Shrink/Slide, Jump
├── InteractionSystem (New) → Collect, Talk
├── CombatSystem (Existing + Extensions)
└── AttributesHandler (Existing + Extensions)

EnemySystem (New)
├── EnemyController
├── EnemyAI
├── EnemyStatus
└── EnemySpawner

ProgressionSystem (New)
├── SlimeGrowthManager
├── SkillTreeManager
└── CrystalAbsorptionSystem

GameSystems (New)
├── QuestManager
├── MinionManager
├── SaveSystem
├── UIManager
└── AudioManager
```

## Components and Interfaces

### 1. Advanced Movement System

#### MovementController Component

```csharp
namespace SlimeMec.Gameplay
{
    public class MovementController : MonoBehaviour
    {
        // Integra com PlayerController existente
        // Gerencia Shrink and Slide, Jump
        // Interface com InteractivePointHandler
    }
}
```

**Key Features:**

- Extends existing PlayerController functionality
- Coroutine-based movement animations
- Collider management during special movements
- Integration with existing Input System

**Dependencies:**

- PlayerController (existing)
- InteractivePointHandler (existing)
- Animator component

#### ShrinkAndSlideHandler

```csharp
public class ShrinkAndSlideHandler : MonoBehaviour
{
    [SerializeField] private float shrinkDuration = 0.5f;
    [SerializeField] private float slideDuration = 1.0f;
    [SerializeField] private AnimationCurve movementCurve;
    
    public async Task ExecuteShrinkAndSlide(Vector3 targetPosition);
}
```

#### JumpHandler

```csharp
public class JumpHandler : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float jumpDuration = 0.8f;
    [SerializeField] private AnimationCurve jumpCurve;
    
    public async Task ExecuteJump(Vector3 targetPosition);
}
```

### 2. Collection and Dialogue System

#### CollectionManager

```csharp
namespace SlimeMec.Gameplay
{
    public class CollectionManager : MonoBehaviour
    {
        // Integra com ItemCollectable existente
        // Gerencia VFX de coleta
        // Atualiza inventário
        
        public event Action<CollectableItemData> OnItemCollected;
    }
}
```

#### DialogueSystem

```csharp
namespace SlimeMec.UI
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueUI dialogueUI;
        [SerializeField] private DialogueDatabase dialogueDatabase;
        
        public void StartDialogue(DialogueData dialogue);
        public void EndDialogue();
    }
}
```

**Integration Points:**

- Uses existing InteractivePointHandler for trigger detection
- Integrates with existing UI framework
- Pauses other interactions during dialogue

### 3. Enemy System Architecture

#### EnemyController (Base Class)

```csharp
namespace SlimeMec.Gameplay
{
    public abstract class EnemyController : MonoBehaviour
    {
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected EnemyAI aiController;
        [SerializeField] protected EnemyStatus statusHandler;
        
        protected virtual void Initialize();
        public abstract void TakeDamage(int damage, Vector3 hitPosition);
        public abstract void Die();
    }
}
```

#### EnemyAI System

```csharp
public class EnemyAI : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack, Hit, Dead }
    
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    
    private StateMachine<AIState> stateMachine;
}
```

#### EnemyStatus

```csharp
public class EnemyStatus : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private int attackPower = 2;
    [SerializeField] private int defense = 1;
    
    public event Action<int, int> OnHealthChanged;
    public event Action OnDeath;
}
```

**Integration with Existing Systems:**

- Uses existing AttackHandler for damage detection
- Integrates with existing VFX system (Attack Hit Effect, Attack Not Hit Effect)
- Follows existing tag system ("Enemy", "Destructible")

### 4. Slime Growth and Skill System

#### SlimeGrowthManager

```csharp
namespace SlimeMec.Gameplay
{
    public class SlimeGrowthManager : MonoBehaviour
    {
        public enum GrowthStage { Filhote, Adulto, GrandeSlime, ReiSlime }
        
        [SerializeField] private GrowthStage currentStage = GrowthStage.Filhote;
        [SerializeField] private int crystalsRequired = 10;
        [SerializeField] private int currentCrystals = 0;
        
        public event Action<GrowthStage> OnStageChanged;
        public event Action<int> OnCrystalsChanged;
    }
}
```

#### SkillTreeManager

```csharp
namespace SlimeMec.UI
{
    public class SkillTreeManager : MonoBehaviour
    {
        [SerializeField] private SkillTreeData skillTreeData;
        [SerializeField] private SkillTreeUI skillTreeUI;
        
        private Dictionary<string, SkillNode> unlockedSkills;
        
        public bool UnlockSkill(string skillId);
        public bool IsSkillUnlocked(string skillId);
    }
}
```

**Integration Points:**

- Extends existing PlayerAttributesHandler
- Uses existing event system for attribute changes
- Integrates with existing UI framework

### 5. Interface and Item Usage System

#### UIManager (Enhanced)

```csharp
namespace SlimeMec.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private HUDController hudController;
        [SerializeField] private MenuController menuController;
        [SerializeField] private InventoryUI inventoryUI;
        [SerializeField] private SkillTreeUI skillTreeUI;
        
        // Integra com Input System existente
        // Mantém estilo cozy definido no GDD
    }
}
```

#### ItemUsageManager

```csharp
namespace SlimeMec.Gameplay
{
    public class ItemUsageManager : MonoBehaviour
    {
        // Integra com PlayerController existente (UseItem1-4)
        // Gerencia buffs e efeitos especiais
        // Interface com ItemBuffHandler existente
        
        public void UseItem(int slotIndex);
        private void ApplyItemEffect(CollectableItemData item);
    }
}
```

### 6. Quest and Minion Systems

#### QuestManager

```csharp
namespace SlimeMec.Gameplay
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private QuestDatabase questDatabase;
        private List<Quest> activeQuests;
        private List<Quest> completedQuests;
        
        public event Action<Quest> OnQuestStarted;
        public event Action<Quest> OnQuestCompleted;
        public event Action<Quest> OnQuestUpdated;
    }
}
```

#### MinionManager

```csharp
namespace SlimeMec.Gameplay
{
    public class MinionManager : MonoBehaviour
    {
        [SerializeField] private List<MinionController> activeMinions;
        [SerializeField] private int maxMinions = 5;
        
        public bool RecruitMinion(MinionData minionData);
        public void DismissMinion(MinionController minion);
        public List<MinionController> GetActiveMinions();
    }
}
```

### 7. Advanced Game Systems

#### SaveSystem

```csharp
namespace SlimeMec.Core
{
    public class SaveSystem : MonoBehaviour
    {
        [SerializeField] private int maxSaveSlots = 3;
        private SaveData currentSaveData;
        
        public void SaveGame(int slotIndex);
        public SaveData LoadGame(int slotIndex);
        public void AutoSave();
    }
}
```

#### AudioManager (Enhanced)

```csharp
namespace SlimeMec.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer masterMixer;
        [SerializeField] private AudioSettings audioSettings;
        
        public void SetMasterVolume(float volume);
        public void SetMusicVolume(float volume);
        public void SetSFXVolume(float volume);
        public void ToggleRelaxMode(bool enabled);
    }
}
```

## Data Models

### Core Data Structures

#### EnemyData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "EnemyData", menuName = "SlimeKing/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Basic Stats")]
    public string enemyName;
    public int maxHealth;
    public int attackPower;
    public int defense;
    public float moveSpeed;
    
    [Header("AI Behavior")]
    public float detectionRange;
    public float attackRange;
    public float patrolRadius;
    
    [Header("Drops")]
    public DropTable dropTable;
}
```

#### QuestData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "QuestData", menuName = "SlimeKing/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questId;
    public string questName;
    public string description;
    public QuestType questType;
    public List<QuestObjective> objectives;
    public List<QuestReward> rewards;
    public List<string> prerequisites;
}
```

#### SkillData (ScriptableObject)

```csharp
[CreateAssetMenu(fileName = "SkillData", menuName = "SlimeKing/Skill Data")]
public class SkillData : ScriptableObject
{
    public string skillId;
    public string skillName;
    public string description;
    public Sprite skillIcon;
    public int requiredLevel;
    public List<string> prerequisites;
    public SkillEffect effect;
}
```

#### SaveData (Serializable)

```csharp
[System.Serializable]
public class SaveData
{
    [Header("Player Data")]
    public Vector3 playerPosition;
    public SlimeGrowthManager.GrowthStage currentStage;
    public PlayerAttributesData attributes;
    public List<string> unlockedSkills;
    
    [Header("World State")]
    public List<string> completedQuests;
    public List<string> collectedItems;
    public List<string> destroyedObjects;
    
    [Header("Game Settings")]
    public float masterVolume;
    public float musicVolume;
    public float sfxVolume;
    public int difficultyLevel;
}
```

## Error Handling

### Exception Management Strategy

1. **Graceful Degradation**: Systems continue functioning even if non-critical components fail
2. **Logging System**: Comprehensive logging for debugging and monitoring
3. **Fallback Mechanisms**: Default behaviors when preferred systems are unavailable
4. **Validation Layers**: Input validation and state checking at system boundaries

#### Error Handling Examples

```csharp
public class SafeSystemManager : MonoBehaviour
{
    private void SafeExecute(System.Action action, string systemName)
    {
        try
        {
            action?.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in {systemName}: {ex.Message}");
            // Implement fallback behavior
        }
    }
}
```

## Testing Strategy

### Unit Testing Framework

1. **Component Testing**: Individual component functionality
2. **Integration Testing**: System interaction validation
3. **Performance Testing**: Frame rate and memory usage validation
4. **Gameplay Testing**: User experience and game flow validation

#### Test Categories

**Core Systems Tests:**

- Movement system accuracy and smoothness
- Combat system damage calculations
- Save/Load data integrity
- UI responsiveness and navigation

**Integration Tests:**

- Player-Enemy interactions
- Quest system progression
- Skill tree unlocking logic
- Inventory management

**Performance Tests:**

- 60+ FPS maintenance with multiple enemies
- Memory usage optimization
- Loading time benchmarks
- Steam Deck compatibility validation

### Testing Tools Integration

```csharp
#if UNITY_EDITOR
[System.Serializable]
public class SystemTestSuite : MonoBehaviour
{
    [ContextMenu("Run All Tests")]
    public void RunAllTests()
    {
        TestMovementSystem();
        TestCombatSystem();
        TestProgressionSystem();
        TestSaveSystem();
    }
}
#endif
```

## Performance Considerations

### Optimization Strategies

1. **Object Pooling**: Enemies, projectiles, VFX particles
2. **Component Caching**: Frequently accessed components
3. **Event System Optimization**: Efficient event subscription/unsubscription
4. **LOD System**: Distance-based detail reduction
5. **Async Operations**: Non-blocking save/load operations

#### Memory Management

```csharp
public class PerformanceManager : MonoBehaviour
{
    [SerializeField] private ObjectPool<Enemy> enemyPool;
    [SerializeField] private ObjectPool<VFXParticle> vfxPool;
    
    private void Update()
    {
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
        {
            System.GC.Collect();
        }
    }
}
```

### Target Performance Metrics

- **PC High-end**: 120 FPS stable
- **PC Mid-range**: 60 FPS stable
- **Steam Deck**: 60 FPS stable
- **Memory Usage**: < 2GB RAM
- **Loading Times**: < 3 seconds between scenes

## Integration Guidelines

### Existing System Integration

1. **PlayerController Integration**: Extend existing functionality without breaking current behavior
2. **Input System Compatibility**: Use existing InputSystem_Actions mappings
3. **VFX System Integration**: Leverage existing particle systems and shaders
4. **Audio Integration**: Build upon existing audio framework

### Code Style Guidelines

```csharp
// Follow existing namespace conventions
namespace SlimeMec.Gameplay
{
    // Use existing documentation standards
    /// <summary>
    /// Component description following existing patterns
    /// </summary>
    public class NewComponent : MonoBehaviour
    {
        #region Inspector Configuration
        [Header("Configuration Section")]
        [SerializeField] private float configValue = 1.0f;
        #endregion
        
        #region Private Variables
        private bool _internalState;
        #endregion
        
        #region Unity Lifecycle
        private void Awake() { }
        private void Start() { }
        #endregion
        
        #region Public Methods
        public void PublicMethod() { }
        #endregion
        
        #region Private Methods
        private void PrivateMethod() { }
        #endregion
    }
}
```

### Migration Strategy

1. **Phase 1**: Core systems (Movement, Collection, Basic Enemy AI)
2. **Phase 2**: Progression systems (Growth, Skills, Quests)
3. **Phase 3**: Advanced features (Save, Audio, UI enhancements)
4. **Phase 4**: Future features (Multiplayer, Achievements, Platform adaptations)

Each phase includes:

- Implementation of new components
- Integration testing with existing systems
- Performance validation
- User experience testing
