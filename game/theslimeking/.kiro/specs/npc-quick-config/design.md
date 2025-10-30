# Design Document - NPCQuickConfig

## Overview

O **NPCQuickConfig** é uma ferramenta de editor Unity que acelera a criação de NPCs para The Slime King, seguindo o padrão estabelecido por `BushQuickConfig` e `ItemQuickConfig`. A ferramenta fornece uma interface EditorWindow intuitiva com templates pré-configurados, configuração automática de componentes Unity, e geração de ScriptableObjects de dados.

**Objetivos de Design:**

- Reduzir tempo de criação de NPCs de 2 horas para 30 minutos (75% economia)
- Manter consistência entre NPCs através de templates baseados no GDD
- Integrar perfeitamente com sistemas existentes do jogo
- Fornecer interface intuitiva que não requer conhecimento de código
- Suportar configuração individual e em lote

**Inspiração:**

- `BushQuickConfig`: Padrão de configuração automática de componentes
- `ItemQuickConfig`: Estrutura de menu e validação
- Ambos usam MenuItem, Undo system, e reflection para configurar componentes

---

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    NPCQuickConfig                           │
│                   (EditorWindow)                            │
└──────────────────┬──────────────────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
        ▼                     ▼
┌───────────────┐    ┌────────────────┐
│   Template    │    │   Component    │
│   System      │    │  Configurator  │
└───────┬───────┘    └────────┬───────┘
        │                     │
        │                     ▼
        │            ┌────────────────┐
        │            │  ScriptableObj │
        │            │   Generator    │
        │            └────────┬───────┘
        │                     │
        └─────────┬───────────┘
                  │
                  ▼
        ┌─────────────────┐
        │   Validation    │
        │   & Preview     │
        └─────────────────┘
```

### Component Diagram

```
NPCQuickConfig (EditorWindow)
├── NPCTemplateData (ScriptableObject) - Template definitions
├── NPCComponentConfigurator (Static Class) - Component setup
├── NPCDataGenerator (Static Class) - ScriptableObject creation
├── NPCAnimatorSetup (Static Class) - Animator configuration
├── NPCGizmosDrawer (Static Class) - Scene view visualization
└── NPCValidator (Static Class) - Configuration validation
```

---

## Components and Interfaces

### 1. NPCQuickConfig (EditorWindow)

**Responsibility:** Main editor window interface for NPC configuration

**Key Fields:**

```csharp
private GameObject targetObject;
private NPCTemplate selectedTemplate;
private string npcName;
private string speciesName;

// Behavior Settings
private BehaviorType behaviorType;
private AIType aiType;
private float detectionRange;

// AI Settings
private float wanderRadius;
private float wanderSpeed;
private float pauseDuration;
private List<Vector2> patrolPoints;
private float patrolSpeed;
private float waitAtPoint;

// System Toggles
private bool friendshipEnabled;
private int initialFriendshipLevel;
private int maxFriendshipLevel;
private bool dialogueEnabled;
private DialogueTriggerType dialogueTriggerType;
private float triggerRange;

// Preview
private bool showPreview;
private bool showGizmos;
```

**Key Methods:**

```csharp
[MenuItem("QuickWinds/NPC Quick Config")]
public static void ShowWindow();

void OnGUI(); // Render interface
void OnSceneGUI(SceneView sceneView); // Draw gizmos
void ApplyConfiguration(); // Main configuration method
void ValidateConfiguration(); // Validation before apply
void LoadTemplate(NPCTemplate template); // Load template data
void ResetToDefaults(); // Reset all fields
```

### 2. NPCTemplateData (ScriptableObject)

**Responsibility:** Store template configurations for reusable NPC types

**Structure:**

```csharp
[CreateAssetMenu(fileName = "NPCTemplate", menuName = "QuickWinds/NPC Template")]
public class NPCTemplateData : ScriptableObject
{
    public string templateName;
    public string description;
    public Sprite iconSprite;
    
    // Behavior Configuration
    public BehaviorType behaviorType;
    public AIType aiType;
    public float detectionRange;
    
    // AI Configuration
    public float wanderRadius;
    public float wanderSpeed;
    public float pauseDuration;
    public float patrolSpeed;
    public float waitAtPoint;
    
    // System Configuration
    public bool friendshipEnabled;
    public int initialFriendshipLevel;
    public int maxFriendshipLevel;
    public bool dialogueEnabled;
    public DialogueTriggerType dialogueTriggerType;
    public float triggerRange;
    
    // Component References
    public RuntimeAnimatorController defaultAnimatorController;
    public Material defaultMaterial;
}
```

**Templates to Create:**

- CervoBrotoTemplate.asset
- EsquiloColetorTemplate.asset
- AbelhaCristalineTemplate.asset

### 3. NPCComponentConfigurator (Static Class)

**Responsibility:** Configure Unity components on GameObject

**Key Methods:**

```csharp
public static class NPCComponentConfigurator
{
    public static void ConfigureBasicComponents(GameObject target);
    public static void ConfigureSpriteRenderer(GameObject target, Material material);
    public static void ConfigureAnimator(GameObject target, RuntimeAnimatorController controller);
    public static void ConfigureCollider(GameObject target, float radius);
    public static void ConfigureRigidbody(GameObject target);
    public static void ConfigureAIComponent(GameObject target, AIType aiType, AISettings settings);
    public static void ConfigureBehaviorComponent(GameObject target, BehaviorType behaviorType, float detectionRange);
    public static void ConfigureFriendshipComponent(GameObject target, string speciesName, int initialLevel, int maxLevel);
    public static void ConfigureDialogueComponent(GameObject target, DialogueTriggerType triggerType, float range);
    public static void SetTagsAndLayers(GameObject target, BehaviorType behaviorType);
}
```

**Component Configuration Details:**

**SpriteRenderer:**

- Sorting Layer: "Characters"
- Order in Layer: 10
- Material: sprite_lit_default or custom
- Color: White

**Animator:**

- Update Mode: Normal
- Culling Mode: CullUpdateTransforms
- Runtime Animator Controller: Template-specific or default

**CircleCollider2D:**

- Is Trigger: False
- Radius: 0.5f (adjustable based on sprite size)
- Offset: Vector2.zero

**Rigidbody2D:**

- Body Type: Dynamic
- Gravity Scale: 0
- Constraints: Freeze Rotation Z
- Collision Detection: Continuous

### 4. NPCDataGenerator (Static Class)

**Responsibility:** Generate and manage ScriptableObject data files

**Key Methods:**

```csharp
public static class NPCDataGenerator
{
    public static NPCData CreateNPCData(string npcName, NPCConfigData configData);
    public static FriendshipData CreateFriendshipData(string speciesName);
    public static DialogueData CreateDialogueData(string npcName, BehaviorType behaviorType);
    public static string GetOrCreateDirectory(string path);
    public static T LoadOrCreateAsset<T>(string path) where T : ScriptableObject;
    public static void SaveAsset(ScriptableObject asset, string path);
}
```

**ScriptableObject Structure:**

**NPCData.cs:**

```csharp
[CreateAssetMenu(fileName = "NPCData", menuName = "Game/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public string species;
    public BehaviorType behaviorType;
    public AIType aiType;
    
    [Header("Stats")]
    public int maxHP = 100;
    public float moveSpeed = 2.5f;
    public float detectionRange = 5.0f;
    
    [Header("Systems")]
    public FriendshipData friendshipData;
    public DialogueData dialogueData;
    
    [Header("AI Settings")]
    public float wanderRadius;
    public float wanderSpeed;
    public List<Vector2> patrolPoints;
    public float patrolSpeed;
}
```

**FriendshipData.cs:**

```csharp
[CreateAssetMenu(fileName = "FriendshipData", menuName = "Game/Friendship Data")]
public class FriendshipData : ScriptableObject
{
    public string speciesName;
    public int maxLevel = 5;
    public List<FriendshipLevel> levels;
}

[System.Serializable]
public class FriendshipLevel
{
    public int level;
    public string title;
    public string description;
    public List<string> unlockedBenefits;
}
```

**DialogueData.cs:**

```csharp
[CreateAssetMenu(fileName = "DialogueData", menuName = "Game/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public List<DialogueNode> dialogueNodes;
}

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string text;
    public Sprite portrait;
    public List<DialogueChoice> choices;
}
```

### 5. NPCAnimatorSetup (Static Class)

**Responsibility:** Configure Animator Controller with states and transitions

**Key Methods:**

```csharp
public static class NPCAnimatorSetup
{
    public static AnimatorController CreateOrLoadController(string npcName);
    public static void ConfigureStates(AnimatorController controller, bool includeDialogue);
    public static void ConfigureTransitions(AnimatorController controller);
    public static void ConfigureParameters(AnimatorController controller, bool includeDialogue);
    public static AnimatorState FindOrCreateState(AnimatorStateMachine stateMachine, string stateName);
    public static void AddTransitionIfNotExists(AnimatorState from, AnimatorState to, string condition, bool hasExitTime);
}
```

**Animator Structure:**

**States:**

- Idle (default state)
- Walk (when Speed > 0.1)
- Talk (when IsTalking = true, if dialogue enabled)
- Death (when IsDead = true)

**Parameters:**

- Speed (float) - Movement speed
- IsDead (bool) - Death trigger
- IsTalking (bool) - Dialogue trigger (if dialogue enabled)

**Transitions:**

- Idle → Walk: Speed > 0.1
- Walk → Idle: Speed < 0.1
- Idle → Talk: IsTalking = true (if dialogue enabled)
- Talk → Idle: IsTalking = false (if dialogue enabled)
- Any State → Death: IsDead = true

**Default Animation Clips:**

- Use placeholder animations from "Assets/Art/Animations/Placeholders/"
- If not available, create empty animation clips

### 6. NPCGizmosDrawer (Static Class)

**Responsibility:** Draw visual gizmos in Scene View for preview

**Key Methods:**

```csharp
public static class NPCGizmosDrawer
{
    public static void DrawWanderRadius(Vector3 position, float radius);
    public static void DrawPatrolPath(List<Vector2> points);
    public static void DrawDetectionRange(Vector3 position, float range);
    public static void DrawDialogueTriggerRange(Vector3 position, float range);
    public static void DrawStealthIndicator(Vector3 position, bool isHidden);
}
```

**Gizmo Styles:**

**Wander Radius:**

- Color: Yellow with 30% alpha
- Style: Wire circle
- Label: "Wander Radius: {radius}m"

**Patrol Path:**

- Color: Cyan
- Style: Lines connecting points with arrows
- Points: Small spheres at each patrol point
- Label: "Patrol Point {index}"

**Detection Range:**

- Color: Red with 20% alpha
- Style: Wire circle
- Label: "Detection: {range}m"

**Dialogue Trigger Range:**

- Color: Green with 20% alpha
- Style: Wire circle
- Label: "Dialogue: {range}m"

**Stealth Indicator:**

- Color: Blue (hidden) or Red (visible)
- Style: Eye icon above NPC
- Label: "Hidden" or "Visible"

### 7. NPCValidator (Static Class)

**Responsibility:** Validate configuration before applying

**Key Methods:**

```csharp
public static class NPCValidator
{
    public static ValidationResult ValidateConfiguration(NPCConfigData config);
    public static bool ValidateGameObject(GameObject target);
    public static bool ValidateSpeciesName(string speciesName);
    public static bool ValidatePatrolPoints(List<Vector2> points);
    public static bool ValidateComponentDependencies(GameObject target);
    public static List<string> GetWarnings(NPCConfigData config);
    public static List<string> GetErrors(NPCConfigData config);
}

public class ValidationResult
{
    public bool IsValid;
    public List<string> Errors;
    public List<string> Warnings;
}
```

**Validation Rules:**

**Errors (Block Configuration):**

- No GameObject selected
- Species Name empty when Friendship enabled
- NPC Name empty
- GameObject is prefab (must unpack first)
- Required components cannot be added

**Warnings (Allow but Notify):**

- Patrol points < 2 (will auto-generate)
- Detection range = 0 for Neutro/Agressivo
- No sprite assigned to SpriteRenderer
- No Animator Controller found
- Dialogue enabled but no DialogueData exists

---

## Data Models

### Enums

```csharp
public enum NPCTemplate
{
    Custom,
    CervoBroto,
    EsquiloColetor,
    AbelhaCristalina
}

public enum BehaviorType
{
    Passivo,    // Flees when attacked
    Neutro,     // Retaliates when attacked
    Agressivo,  // Attacks player on sight
    QuestGiver  // Static, dialogue-focused
}

public enum AIType
{
    Static,  // No movement
    Wander,  // Random wandering
    Patrol   // Fixed patrol path
}

public enum DialogueTriggerType
{
    Proximity,    // Triggers when player is near
    Interaction   // Triggers on player input
}
```

### Configuration Data Structure

```csharp
public class NPCConfigData
{
    public string npcName;
    public string speciesName;
    public BehaviorType behaviorType;
    public AIType aiType;
    public float detectionRange;
    
    // AI Settings
    public AISettings aiSettings;
    
    // System Settings
    public bool friendshipEnabled;
    public FriendshipSettings friendshipSettings;
    public bool dialogueEnabled;
    public DialogueSettings dialogueSettings;
}

public class AISettings
{
    // Wander
    public float wanderRadius;
    public float wanderSpeed;
    public float pauseDuration;
    
    // Patrol
    public List<Vector2> patrolPoints;
    public float patrolSpeed;
    public float waitAtPoint;
}

public class FriendshipSettings
{
    public int initialLevel;
    public int maxLevel;
}

public class DialogueSettings
{
    public DialogueTriggerType triggerType;
    public float triggerRange;
}
```

---

## Error Handling

### Error Categories

**1. User Input Errors:**

- Missing required fields (NPC Name, Species Name)
- Invalid values (negative ranges, empty patrol points)
- **Handling:** Display error message in EditorWindow, prevent Apply

**2. Asset Errors:**

- Missing template assets
- Missing animator controllers
- Missing materials
- **Handling:** Log warning, use fallback defaults, continue configuration

**3. Component Errors:**

- Cannot add component (prefab locked)
- Component already exists with conflicting settings
- **Handling:** Display error dialog, offer solutions (unpack prefab, overwrite settings)

**4. File System Errors:**

- Cannot create directory
- Cannot save ScriptableObject
- Asset already exists
- **Handling:** Display error dialog with path, offer retry or manual intervention

### Error Recovery Strategies

**Undo System:**

```csharp
Undo.RegisterCompleteObjectUndo(targetObject, "Configure NPC");
// If error occurs during configuration:
Undo.PerformUndo(); // Revert all changes
```

**Partial Configuration:**

- If some components succeed and others fail, keep successful ones
- Display summary: "5/7 components configured successfully"
- List failed components with reasons

**Fallback Defaults:**

- If template asset missing, use hardcoded defaults
- If animator controller missing, create minimal controller
- If material missing, use Unity default sprite material

---

## Testing Strategy

### Unit Testing

**Test Classes:**

- NPCValidatorTests
- NPCDataGeneratorTests
- NPCComponentConfiguratorTests

**Test Cases:**

**NPCValidatorTests:**

```csharp
[Test] public void ValidateConfiguration_NoGameObject_ReturnsError()
[Test] public void ValidateConfiguration_EmptySpeciesName_ReturnsError()
[Test] public void ValidateConfiguration_ValidConfig_ReturnsSuccess()
[Test] public void ValidatePatrolPoints_LessThan2_ReturnsWarning()
[Test] public void ValidatePatrolPoints_Valid_ReturnsSuccess()
```

**NPCDataGeneratorTests:**

```csharp
[Test] public void CreateNPCData_ValidInput_CreatesAsset()
[Test] public void CreateNPCData_ExistingAsset_PromptOverwrite()
[Test] public void CreateFriendshipData_ValidSpecies_CreatesWithDefaults()
[Test] public void CreateDialogueData_QuestGiver_IncludesQuestDialogue()
```

**NPCComponentConfiguratorTests:**

```csharp
[Test] public void ConfigureBasicComponents_AddsAllRequired()
[Test] public void ConfigureSpriteRenderer_SetsCorrectLayer()
[Test] public void ConfigureAnimator_CreatesController()
[Test] public void ConfigureAIComponent_Wander_AddsWanderScript()
[Test] public void ConfigureAIComponent_Patrol_AddsPatrolScript()
```

### Integration Testing

**Test Scenarios:**

**Scenario 1: Create Cervo-Broto from Template**

1. Open NPCQuickConfig window
2. Select empty GameObject
3. Select "Cervo-Broto" template
4. Click "Apply Configuration"
5. Verify: All components added, NPCData created, Animator configured

**Scenario 2: Create Custom NPC**

1. Open NPCQuickConfig window
2. Select GameObject
3. Configure manually: Neutro, Patrol, Friendship enabled
4. Add 4 patrol points
5. Click "Apply Configuration"
6. Verify: Patrol path created, FriendshipData exists

**Scenario 3: Batch Configuration**

1. Select 3 GameObjects
2. Open NPCQuickConfig
3. Select "Abelha Cristalina" template
4. Click "Apply to All"
5. Verify: All 3 NPCs configured identically with unique NPCData

### Manual Testing

**Test Checklist:**

**UI Testing:**

- [ ] Window opens from menu "QuickWinds/NPC Quick Config"
- [ ] All fields render correctly
- [ ] Dropdowns show correct options
- [ ] Tooltips display on hover
- [ ] Help button opens documentation
- [ ] Preview updates when template changes

**Template Testing:**

- [ ] Cervo-Broto template loads correct values
- [ ] Esquilo Coletor template loads correct values
- [ ] Abelha Cristalina template loads correct values
- [ ] Custom template allows manual configuration

**Component Testing:**

- [ ] SpriteRenderer added with correct settings
- [ ] Animator added with controller
- [ ] Collider added with correct size
- [ ] Rigidbody2D added with correct constraints
- [ ] AI scripts added based on AIType
- [ ] Behavior scripts added based on BehaviorType

**ScriptableObject Testing:**

- [ ] NPCData created in correct directory
- [ ] FriendshipData created when enabled
- [ ] DialogueData created when enabled
- [ ] Assets reference each other correctly

**Gizmo Testing:**

- [ ] Wander radius draws in Scene View
- [ ] Patrol path draws with arrows
- [ ] Detection range draws correctly
- [ ] Dialogue trigger range draws when enabled
- [ ] Gizmos update when values change

**Validation Testing:**

- [ ] Error shown when no GameObject selected
- [ ] Error shown when Species Name empty (friendship enabled)
- [ ] Warning shown when patrol points < 2
- [ ] Validation passes with correct configuration

**Batch Testing:**

- [ ] Multiple GameObjects detected
- [ ] "Apply to All" button appears
- [ ] Progress bar shows during batch
- [ ] Summary displays after completion
- [ ] Each NPC gets unique NPCData

### Performance Testing

**Metrics to Measure:**

- Time to configure single NPC: Target < 500ms
- Time to configure 10 NPCs (batch): Target < 5 seconds
- Scene View gizmo render time: Target < 16ms per frame
- Memory allocation during configuration: Target < 10MB

**Performance Optimization Strategies:**

**1. Asset Database Optimization:**

```csharp
AssetDatabase.StartAssetEditing();
try
{
    // Create multiple assets
    CreateNPCData();
    CreateFriendshipData();
    CreateDialogueData();
}
finally
{
    AssetDatabase.StopAssetEditing();
}
AssetDatabase.Refresh();
```

**2. Gizmo Optimization:**

```csharp
// Cache gizmo data, only recalculate when values change
private static Dictionary<GameObject, GizmoCache> gizmoCache;

public static void DrawGizmos(GameObject target)
{
    if (!gizmoCache.ContainsKey(target) || gizmoCache[target].IsDirty)
    {
        gizmoCache[target] = CalculateGizmoData(target);
    }
    DrawCachedGizmos(gizmoCache[target]);
}
```

**3. Batch Processing Optimization:**

```csharp
// Process in parallel when possible
Parallel.ForEach(selectedObjects, obj =>
{
    ConfigureNPC(obj);
});
```

**4. Undo System Optimization:**

```csharp
// Group undo operations
Undo.SetCurrentGroupName("Configure NPCs");
int undoGroup = Undo.GetCurrentGroup();

// Perform all operations
ConfigureAllNPCs();

// Collapse into single undo
Undo.CollapseUndoOperations(undoGroup);
```

---

## File Structure

```
Assets/
├── Code/
│   └── Editor/
│       └── QuickWinds/
│           ├── NPCQuickConfig.cs (EditorWindow)
│           ├── NPCComponentConfigurator.cs
│           ├── NPCDataGenerator.cs
│           ├── NPCAnimatorSetup.cs
│           ├── NPCGizmosDrawer.cs
│           └── NPCValidator.cs
│
├── Code/
│   └── Gameplay/
│       └── NPCs/
│           ├── NPCController.cs (MonoBehaviour - main controller)
│           ├── NPCBehavior.cs (MonoBehaviour - behavior logic)
│           ├── NPCFriendship.cs (MonoBehaviour - friendship system)
│           ├── NPCDialogue.cs (MonoBehaviour - dialogue system)
│           ├── AI/
│           │   ├── NPCStaticAI.cs
│           │   ├── NPCWanderAI.cs
│           │   └── NPCPatrolAI.cs
│           └── Data/
│               ├── NPCData.cs (ScriptableObject)
│               ├── FriendshipData.cs (ScriptableObject)
│               └── DialogueData.cs (ScriptableObject)
│
├── Data/
│   └── QuickWinds/
│       └── Templates/
│           └── NPCTemplates/
│               ├── CervoBrotoTemplate.asset
│               ├── EsquiloColetorTemplate.asset
│               └── AbelhaCristalineTemplate.asset
│
├── Data/
│   └── NPCs/
│       ├── {NPCName}Data.asset (generated)
│       ├── Friendship/
│       │   └── {SpeciesName}FriendshipData.asset (generated)
│       └── Dialogues/
│           └── {NPCName}DialogueData.asset (generated)
│
└── Art/
    └── Animations/
        └── NPCs/
            └── {NPCName}Controller.controller (generated)
```

---

## Integration with Existing Systems

### 1. Combat System Integration

**NPCBehavior.cs** must integrate with existing combat:

```csharp
public class NPCBehavior : MonoBehaviour
{
    public BehaviorType behaviorType;
    public float detectionRange;
    
    private void OnTakeDamage(int damage)
    {
        switch (behaviorType)
        {
            case BehaviorType.Passivo:
                // Flee logic
                if (GetCurrentHP() < GetMaxHP() * 0.3f)
                    StartFleeing();
                break;
                
            case BehaviorType.Neutro:
                // Retaliate logic
                SetTarget(attacker);
                StartAttacking();
                break;
                
            case BehaviorType.Agressivo:
                // Already attacking, increase aggression
                IncreaseAggression();
                break;
                
            case BehaviorType.QuestGiver:
                // Quest givers don't fight
                break;
        }
    }
}
```

### 2. Friendship System Integration

**NPCFriendship.cs** must integrate with global friendship manager:

```csharp
public class NPCFriendship : MonoBehaviour
{
    public FriendshipData friendshipData;
    private int currentLevel;
    
    private void Start()
    {
        // Register with global FriendshipManager
        if (FriendshipManager.Instance != null)
        {
            FriendshipManager.Instance.RegisterNPC(this);
            currentLevel = FriendshipManager.Instance.GetFriendshipLevel(friendshipData.speciesName);
        }
    }
    
    public void IncreaseFriendship(int amount)
    {
        currentLevel = Mathf.Min(currentLevel + amount, friendshipData.maxLevel);
        FriendshipManager.Instance.UpdateFriendship(friendshipData.speciesName, currentLevel);
        OnFriendshipLevelChanged?.Invoke(currentLevel);
    }
}
```

### 3. Dialogue System Integration

**NPCDialogue.cs** must integrate with existing dialogue system:

```csharp
public class NPCDialogue : MonoBehaviour
{
    public DialogueData dialogueData;
    public DialogueTriggerType triggerType;
    public float triggerRange;
    
    private void Update()
    {
        if (triggerType == DialogueTriggerType.Proximity)
        {
            CheckProximityTrigger();
        }
    }
    
    private void CheckProximityTrigger()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        if (distanceToPlayer <= triggerRange && !DialogueManager.Instance.IsDialogueActive)
        {
            ShowDialoguePrompt();
        }
    }
    
    public void StartDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueData);
            GetComponent<Animator>()?.SetBool("IsTalking", true);
        }
    }
    
    public void EndDialogue()
    {
        GetComponent<Animator>()?.SetBool("IsTalking", false);
    }
}
```

### 4. Quest System Integration

**QuestGiver Component** (added when BehaviorType = QuestGiver):

```csharp
public class QuestGiver : MonoBehaviour
{
    public List<QuestData> availableQuests;
    public bool hasActiveQuest;
    
    private void Start()
    {
        // Register with QuestManager
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.RegisterQuestGiver(this);
        }
    }
    
    public void OfferQuest()
    {
        if (availableQuests.Count > 0 && !hasActiveQuest)
        {
            QuestData nextQuest = availableQuests[0];
            QuestManager.Instance.OfferQuest(nextQuest);
        }
    }
}
```

### 5. NPCManager Registration

**NPCController.cs** must register with global NPCManager:

```csharp
public class NPCController : MonoBehaviour
{
    public NPCData npcData;
    
    private void Start()
    {
        // Register with NPCManager
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.RegisterNPC(this);
        }
        
        // Initialize from NPCData
        InitializeFromData();
    }
    
    private void OnDestroy()
    {
        // Unregister from NPCManager
        if (NPCManager.Instance != null)
        {
            NPCManager.Instance.UnregisterNPC(this);
        }
    }
}
```

---

## UI/UX Design

### EditorWindow Layout

```
┌─────────────────────────────────────────────────────────┐
│  NPC Quick Config                              [?] [X]  │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  Selected GameObject: [CervoBroto_01        ]          │
│                                                         │
│  ┌─ Template Selection ─────────────────────────────┐  │
│  │  Template: [Cervo-Broto ▼]                       │  │
│  │  📝 Passivo, wander behavior, amizade habilitada │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌─ Basic Configuration ────────────────────────────┐  │
│  │  NPC Name:     [Cervo-Broto           ]          │  │
│  │  Species Name: [Cervo                 ]          │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌─ Behavior Settings ──────────────────────────────┐  │
│  │  Behavior Type: [Passivo ▼]                      │  │
│  │  AI Type:       [Wander ▼]                       │  │
│  │  Detection Range: [5.0] meters                   │  │
│  │                                                   │  │
│  │  ┌─ Wander Settings ─────────────────────────┐   │  │
│  │  │  Wander Radius: [5.0] meters              │   │  │
│  │  │  Wander Speed:  [2.0] m/s                 │   │  │
│  │  │  Pause Duration: [2.0] seconds            │   │  │
│  │  └───────────────────────────────────────────┘   │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌─ System Configuration ───────────────────────────┐  │
│  │  ☑ Friendship System                             │  │
│  │    Initial Level: [0]  Max Level: [5]           │  │
│  │                                                   │  │
│  │  ☐ Dialogue System                               │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  ┌─ Preview ────────────────────────────────────────┐  │
│  │  ☑ Show Gizmos in Scene View                     │  │
│  │  Components to add: 8                            │  │
│  │  ScriptableObjects to create: 2                  │  │
│  └──────────────────────────────────────────────────┘  │
│                                                         │
│  [Validate Configuration]  [Apply Configuration]       │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

### Color Scheme

**Following Unity Editor Theme:**

- Background: Unity default gray
- Headers: Slightly darker gray with border
- Active fields: White background
- Disabled fields: Light gray background
- Buttons: Unity default blue
- Warnings: Yellow
- Errors: Red
- Success: Green

### Tooltips

**All fields should have helpful tooltips:**

- Template: "Select a pre-configured NPC template or choose Custom for manual configuration"
- NPC Name: "Unique name for this NPC instance"
- Species Name: "Species identifier used for friendship system (e.g., 'Cervo', 'Esquilo')"
- Behavior Type: "How the NPC reacts to the player"
- AI Type: "Movement pattern for the NPC"
- Wander Radius: "Maximum distance NPC will wander from spawn point"
- Detection Range: "Distance at which NPC detects player"
- Friendship System: "Enable to allow player to build relationship with this NPC"
- Dialogue System: "Enable to allow player to talk with this NPC"

---

## Implementation Notes

### Following Existing Patterns

**1. MenuItem Pattern (from BushQuickConfig):**

```csharp
[MenuItem("QuickWinds/NPC Quick Config")]
public static void ShowWindow()
{
    var window = GetWindow<NPCQuickConfig>("NPC Quick Config");
    window.minSize = new Vector2(400, 600);
    window.Show();
}
```

**2. Undo System (from BushQuickConfig):**

```csharp
Undo.RegisterCompleteObjectUndo(targetObject, "Configure NPC");
// Perform modifications
EditorUtility.SetDirty(targetObject);
```

**3. Reflection for Private Fields (from BushQuickConfig):**

```csharp
private static void SetPrivateField(Object target, string fieldName, object value)
{
    var field = target.GetType().GetField(fieldName,
        System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.Instance);
    
    if (field != null)
        field.SetValue(target, value);
}
```

**4. Asset Loading (from BushQuickConfig):**

```csharp
Material material = AssetDatabase.LoadAssetAtPath<Material>(MATERIAL_PATH);
if (material != null)
    spriteRenderer.material = material;
else
    Debug.LogWarning($"Material not found at: {MATERIAL_PATH}");
```

### Key Differences from BushQuickConfig

**1. EditorWindow vs Static Methods:**

- BushQuickConfig uses static methods with MenuCommand
- NPCQuickConfig uses EditorWindow for richer UI
- Reason: NPCs have more configuration options requiring interactive UI

**2. Template System:**

- BushQuickConfig has single configuration
- NPCQuickConfig has multiple templates
- Reason: Different NPC types have significantly different configurations

**3. ScriptableObject Generation:**

- BushQuickConfig doesn't generate data assets
- NPCQuickConfig generates NPCData, FriendshipData, DialogueData
- Reason: NPCs need persistent data for gameplay systems

**4. Gizmo Visualization:**

- BushQuickConfig doesn't draw gizmos
- NPCQuickConfig draws AI ranges and paths
- Reason: Visual feedback crucial for AI configuration

---

## Dependencies

### Unity Packages Required

- Unity Editor (built-in)
- Unity UI (built-in)
- Unity 2D (built-in)

### Project Dependencies

- Existing gameplay scripts (if any):
  - PlayerController (for dialogue proximity checks)
  - FriendshipManager (for friendship system integration)
  - DialogueManager (for dialogue system integration)
  - QuestManager (for quest giver integration)
  - NPCManager (for NPC registration)

**Note:** If these managers don't exist yet, NPCQuickConfig will create placeholder components that can be integrated later.

### Asset Dependencies

- Sprite assets for NPCs (optional, can be assigned later)
- Animation clips for Animator (optional, will use placeholders)
- Materials (optional, will use Unity default)

---

## Future Enhancements

### Phase 2 Features (Post-Alpha 1)

**1. Advanced AI Behaviors:**

- Flee behavior for Passivo NPCs
- Chase behavior for Agressivo NPCs
- Return to spawn behavior
- Group behavior (flocking, formations)

**2. Visual Editor for Patrol Paths:**

- Click in Scene View to add patrol points
- Drag points to reposition
- Insert/delete points
- Save patrol path as reusable asset

**3. Dialogue Editor Integration:**

- Visual dialogue tree editor
- Branching conversations
- Condition-based dialogue
- Voice line integration

**4. Animation Setup Wizard:**

- Import sprite sheets
- Auto-slice sprites
- Generate animation clips
- Configure Animator Controller

**5. Prefab Variants:**

- Save configured NPC as prefab variant
- Library of reusable NPC prefabs
- One-click instantiation

**6. Batch Operations:**

- Find and replace NPC configurations
- Update all NPCs of same species
- Bulk property editing

**7. Testing Tools:**

- Play mode testing in editor
- AI behavior visualization
- Dialogue flow testing
- Friendship progression simulation

---

## Conclusion

O NPCQuickConfig fornece uma solução completa para criação rápida de NPCs em The Slime King, reduzindo drasticamente o tempo de desenvolvimento enquanto mantém consistência e qualidade. A arquitetura modular permite fácil extensão e manutenção, e a integração com sistemas existentes garante que NPCs funcionem perfeitamente no jogo.

**Próximos Passos:**

1. Revisar e aprovar este design
2. Criar task list detalhada para implementação
3. Implementar componentes core (EditorWindow, Configurator, Generator)
4. Criar templates para Alpha 1 (Cervo-Broto, Esquilo Coletor, Abelha Cristalina)
5. Testar com criação dos 3 NPCs do Alpha 1
6. Iterar baseado em feedback
