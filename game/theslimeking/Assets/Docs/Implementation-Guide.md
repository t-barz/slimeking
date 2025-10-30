# The Slime King - Guia de Implementação

## 🎯 Ordem de Implementação Recomendada

### Fase 1: Fundação (Semanas 1-4)

#### 1.1 Setup do Projeto

- [ ] Criar projeto Unity 6.2
- [ ] Configurar URP
- [ ] Importar Input System
- [ ] Configurar layers de colisão
- [ ] Setup de pastas (ver Quick-Reference.md)

#### 1.2 Player Controller Básico

```csharp
// PlayerController.cs - Prioridade ALTA
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float crouchSpeedMultiplier = 0.4f;
    
    [Header("State")]
    public bool isCrouching = false;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;
    
    void Update()
    {
        // Input
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isCrouching = Input.GetButton("Crouch");
        
        // Animação
        UpdateAnimation();
    }
    
    void FixedUpdate()
    {
        // Movimento
        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        rb.velocity = moveInput.normalized * currentSpeed;
    }
}
```

#### 1.3 Sistema de Animação

- [ ] Criar Animator Controller
- [ ] Animações: Idle, Walk, Crouch, Attack
- [ ] Blend trees para 8 direções
- [ ] Transições suaves

#### 1.4 Câmera

- [ ] Instalar Cinemachine
- [ ] Virtual Camera seguindo player
- [ ] Dead zones configuradas
- [ ] Smooth damping

---

### Fase 2: IA Básica (Semanas 5-8)

#### 2.1 Estrutura Base de IA

```csharp
// AIController.cs - Prioridade ALTA
public class AIController : MonoBehaviour
{
    public AIBehaviorProfile profile;
    public AIState currentState;
    
    private AIPerception perception;
    private AIMemory memory;
    
    void Start()
    {
        perception = GetComponent<AIPerception>();
        memory = new AIMemory();
        TransitionToState(new IdleState());
    }
    
    void Update()
    {
        currentState?.UpdateState(this);
    }
    
    public void TransitionToState(AIState newState)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }
}
```

#### 2.2 Estados Prioritários

**Implementar nesta ordem:**

1. IdleState
2. PatrolState
3. ChaseState
4. AttackState

**Deixar para depois:**

- AlertState
- InvestigateState
- SearchState
- ReturnState
- FleeState
- StunnedState

#### 2.3 Sistema de Percepção

```csharp
// AIPerception.cs - Prioridade MÉDIA
public class AIPerception : MonoBehaviour
{
    public float visionRange = 10f;
    public float visionAngle = 90f;
    public float hearingRange = 5f;
    
    public bool CanSeeTarget(Transform target)
    {
        Vector2 directionToTarget = target.position - transform.position;
        float angleToTarget = Vector2.Angle(transform.up, directionToTarget);
        
        if (angleToTarget < visionAngle / 2f && directionToTarget.magnitude < visionRange)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToTarget, visionRange);
            
            if (hit.collider != null && hit.collider.transform == target)
            {
                // Verifica stealth
                PlayerController player = target.GetComponent<PlayerController>();
                if (player.isCrouching && HasCoverBetween(transform.position, target.position))
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }
    
    bool HasCoverBetween(Vector2 from, Vector2 to)
    {
        // Raycast para detectar objetos de cobertura
        RaycastHit2D[] hits = Physics2D.RaycastAll(from, (to - from).normalized, Vector2.Distance(from, to));
        
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Cover"))
            {
                return true;
            }
        }
        return false;
    }
}
```

---

### Fase 3: Combate e Habilidades (Semanas 9-12)

#### 3.1 Sistema de Combate

```csharp
// PlayerCombat.cs - Prioridade MÉDIA
public class PlayerCombat : MonoBehaviour
{
    public int baseDamage = 5;
    public float attackRange = 1.5f;
    public float attackCooldown = 0.5f;
    
    private float lastAttackTime;
    
    void Update()
    {
        if (Input.GetButtonDown("Attack") && Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }
    
    void PerformAttack()
    {
        // Detecta inimigos em alcance
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Health>()?.TakeDamage(baseDamage);
            }
        }
        
        // Animação e som
        GetComponent<Animator>().SetTrigger("Attack");
        AudioManager.Instance.PlaySFX("Attack");
    }
}
```

#### 3.2 Sistema de Habilidades

```csharp
// AbilitySystem.cs - Prioridade BAIXA (Alpha)
public class AbilitySystem : MonoBehaviour
{
    public Ability[] equippedAbilities = new Ability[4];
    
    void Update()
    {
        if (Input.GetButtonDown("Ability1")) UseAbility(0);
        if (Input.GetButtonDown("Ability2")) UseAbility(1);
        if (Input.GetButtonDown("Ability3")) UseAbility(2);
        if (Input.GetButtonDown("Ability4")) UseAbility(3);
    }
    
    void UseAbility(int slot)
    {
        if (equippedAbilities[slot] != null && equippedAbilities[slot].CanUse())
        {
            equippedAbilities[slot].Use(transform.position, transform.up);
        }
    }
}
```

---

### Fase 4: Quests (Semanas 13-16)

#### 4.1 Quest Manager

```csharp
// QuestManager.cs - Prioridade MÉDIA
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();
    
    public event Action<Quest> OnQuestAccepted;
    public event Action<Quest> OnQuestCompleted;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void AcceptQuest(Quest quest)
    {
        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            OnQuestAccepted?.Invoke(quest);
            Debug.Log($"Quest aceita: {quest.questName}");
        }
    }
    
    public void UpdateObjective(string questID, int objectiveIndex)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        if (quest != null)
        {
            if (quest.objectives[objectiveIndex].IsComplete())
            {
                CheckQuestCompletion(quest);
            }
        }
    }
    
    void CheckQuestCompletion(Quest quest)
    {
        bool allComplete = quest.objectives.All(obj => obj.IsComplete());
        
        if (allComplete)
        {
            CompleteQuest(quest);
        }
    }
    
    public void CompleteQuest(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        GiveRewards(quest);
        OnQuestCompleted?.Invoke(quest);
        Debug.Log($"Quest completada: {quest.questName}");
    }
    
    void GiveRewards(Quest quest)
    {
        // Adicionar reputação
        GameManager.Instance.AddReputation(quest.reputationReward);
        
        // Adicionar cristais
        foreach (var reward in quest.itemRewards)
        {
            InventoryManager.Instance.AddItem(reward.itemID, reward.amount);
        }
    }
}
```

#### 4.2 ScriptableObject de Quest

```csharp
// Quest.cs - Prioridade MÉDIA
[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea(3, 5)]
    public string description;
    
    public string giverNPCID;
    public List<QuestObjective> objectives;
    
    public int reputationReward;
    public List<ItemReward> itemRewards;
    
    public bool repeatable = false;
}

[System.Serializable]
public class ItemReward
{
    public string itemID;
    public int amount;
}
```

---

### Fase 5: Puzzles (Semanas 17-20)

#### 5.1 Base de Puzzle

```csharp
// Puzzle.cs - Prioridade BAIXA (Alpha)
public abstract class Puzzle : MonoBehaviour
{
    public string puzzleID;
    public bool isComplete = false;
    
    public event Action OnPuzzleComplete;
    
    public abstract void CheckCompletion();
    public abstract void ResetPuzzle();
    
    protected void CompletePuzzle()
    {
        if (!isComplete)
        {
            isComplete = true;
            OnPuzzleComplete?.Invoke();
            GiveRewards();
            Debug.Log($"Puzzle completado: {puzzleID}");
        }
    }
    
    protected virtual void GiveRewards()
    {
        // Override em puzzles específicos
    }
}
```

#### 5.2 Exemplo: Puzzle de Pressão

```csharp
// PressurePlatePuzzle.cs - Exemplo Simples
public class PressurePlatePuzzle : Puzzle
{
    public List<PressurePlate> plates;
    public Door puzzleDoor;
    
    void Update()
    {
        if (!isComplete)
        {
            CheckCompletion();
        }
    }
    
    public override void CheckCompletion()
    {
        bool allActivated = plates.All(plate => plate.IsActivated);
        
        if (allActivated)
        {
            puzzleDoor.Open();
            CompletePuzzle();
        }
    }
    
    public override void ResetPuzzle()
    {
        foreach (var plate in plates)
        {
            plate.Deactivate();
        }
        puzzleDoor.Close();
        isComplete = false;
    }
    
    protected override void GiveRewards()
    {
        GameManager.Instance.AddReputation(15);
        InventoryManager.Instance.AddCrystals(CrystalType.Green, 10);
    }
}
```

---

### Fase 6: Cutscenes (Semanas 21-24)

#### 6.1 Sistema Básico

```csharp
// CutsceneManager.cs - Prioridade BAIXA (Alpha)
public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance;
    
    private Cutscene currentCutscene;
    private bool isPlaying = false;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void PlayCutscene(Cutscene cutscene)
    {
        if (!isPlaying)
        {
            StartCoroutine(PlayCutsceneCoroutine(cutscene));
        }
    }
    
    IEnumerator PlayCutsceneCoroutine(Cutscene cutscene)
    {
        isPlaying = true;
        currentCutscene = cutscene;
        
        yield return StartCoroutine(cutscene.Play());
        
        isPlaying = false;
        currentCutscene = null;
    }
}
```

---

## 🎨 Assets Necessários

### Sprites

- [ ] Player (Filhote): 16x16px, 4 direções, 4 frames/animação
- [ ] Inimigos básicos: 16x16px ou 32x32px
- [ ] Tilesets para biomas
- [ ] Objetos interativos
- [ ] UI elements

### Audio

- [ ] Música ambiente (1 por bioma)
- [ ] SFX de movimento
- [ ] SFX de combate
- [ ] SFX de UI
- [ ] SFX ambientais

---

## 🧪 Testes Prioritários

### Alpha (Q4 2025)

1. **Movimento e Stealth**
   - Agachar funciona corretamente
   - Detecção de cobertura precisa
   - Velocidade reduzida ao agachar

2. **IA Básica**
   - Estados Idle, Patrol, Chase, Attack funcionam
   - Transições suaves entre estados
   - Percepção detecta jogador corretamente

3. **Combate**
   - Ataque básico causa dano
   - Cooldown funciona
   - Feedback visual e sonoro

4. **Quests**
   - Aceitar quest funciona
   - Objetivos atualizam corretamente
   - Recompensas são dadas

---

## 📊 Checklist de Funcionalidades

### Core Gameplay

- [ ] Movimento 8 direções
- [ ] Agachar (stealth)
- [ ] Ataque corpo-a-corpo
- [ ] Esquiva/Dash
- [ ] Sistema de HP e Stamina

### IA

- [ ] 4 estados básicos (Idle, Patrol, Chase, Attack)
- [ ] Percepção (visão e audição)
- [ ] Detecção de stealth
- [ ] Patrulha com waypoints

### Quests

- [ ] QuestManager funcional
- [ ] 3 tipos de objetivos (Collect, Defeat, Deliver)
- [ ] UI de quest log
- [ ] Sistema de recompensas

### Puzzles

- [ ] 2 puzzles funcionais
- [ ] Sistema de reset
- [ ] Feedback visual

### Cutscenes

- [ ] Dialogue cutscenes
- [ ] Sistema de skip
- [ ] Triggers funcionais

### UI

- [ ] HUD (HP, Stamina, Habilidades)
- [ ] Menu de pausa
- [ ] Inventário básico
- [ ] Quest tracker

### Mundo

- [ ] 2 biomas jogáveis (Ninho + Floresta)
- [ ] Transições entre áreas
- [ ] Objetos interativos
- [ ] NPCs básicos

---

## 🐛 Debug Tools Recomendadas

```csharp
// DebugManager.cs
public class DebugManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) ToggleGodMode();
        if (Input.GetKeyDown(KeyCode.F2)) AddCrystals(100);
        if (Input.GetKeyDown(KeyCode.F3)) CompleteCurrentQuest();
        if (Input.GetKeyDown(KeyCode.F4)) TeleportToNextBiome();
        if (Input.GetKeyDown(KeyCode.F5)) ShowAIDebugInfo();
    }
    
    void ToggleGodMode()
    {
        PlayerController.Instance.isInvincible = !PlayerController.Instance.isInvincible;
        Debug.Log($"God Mode: {PlayerController.Instance.isInvincible}");
    }
    
    void ShowAIDebugInfo()
    {
        foreach (var ai in FindObjectsOfType<AIController>())
        {
            Debug.Log($"{ai.name}: State = {ai.currentState.GetType().Name}");
        }
    }
}
```

---

## 📚 Recursos Úteis

### Unity Packages

- Cinemachine (câmera)
- Input System (controles)
- TextMeshPro (UI)
- 2D Sprite (sprites e animação)

### Assets Recomendados

- DOTween (animações de UI)
- Odin Inspector (editor customizado)
- Rewired (input avançado - opcional)

---

**Versão:** 8.0  
**Última Atualização:** 2025  
**Tempo Estimado:** 24 semanas (6 meses) para Alpha
