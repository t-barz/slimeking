# Design Document: BeeWorker Enemy AI System

## Overview

The BeeWorker Enemy AI System implements a complete, performance-optimized enemy behavior system for The Slime King. The system uses a finite state machine architecture with four states (Patrol, Combat, Hit, Dead) and integrates seamlessly with Unity's Animator, Physics2D, and Transform systems.

The design prioritizes performance through strategic caching, interval-based detection, and NonAlloc physics methods to support multiple active enemies without frame rate degradation. Visual polish is achieved through a continuous bouncing effect applied to a child transform, maintaining organic movement without affecting collision detection.

Key architectural decisions:
- **State Machine Pattern**: Clear separation of behaviors with explicit state transitions
- **Static Player Caching**: Single cached reference shared across all BeeWorker instances
- **Child Transform Bouncing**: Visual effects isolated from physics calculations
- **Interval-Based Detection**: Reduces physics queries from 60/sec to 5/sec per enemy
- **Animator Parameter Hashing**: Eliminates string allocations during animation control

## Architecture

### Component Structure

```
BeeWorkerA (Root GameObject)
├── BeeWorkerBehaviorController (MonoBehaviour)
├── Rigidbody2D (Dynamic, Gravity Scale = 0)
├── CircleCollider2D (Main collision)
└── Animator

    Visual (Child GameObject)
    └── SpriteRenderer
    
    HurtBox (Child GameObject)
    └── CircleCollider2D (Trigger, receives damage)
    
    HitBox (Child GameObject)
    └── CapsuleCollider2D (Trigger, deals damage, initially disabled)
```

### State Machine Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      State Machine                           │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌─────────┐  Player Detected    ┌─────────┐               │
│  │ Patrol  │ ──────────────────> │ Combat  │               │
│  │         │ <────────────────── │         │               │
│  └─────────┘  Player Lost/Stealth└─────────┘               │
│       │                                │                     │
│       │ Take Damage                    │ Take Damage        │
│       ▼                                ▼                     │
│  ┌─────────┐                      ┌─────────┐               │
│  │   Hit   │ ──────────────────> │   Hit   │               │
│  │(Patrol) │  Knockback Complete │(Combat) │               │
│  └─────────┘                      └─────────┘               │
│       │                                │                     │
│       │ HP <= 0                        │ HP <= 0            │
│       ▼                                ▼                     │
│  ┌──────────────────────────────────────┐                   │
│  │              Dead                     │                   │
│  └──────────────────────────────────────┘                   │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

### System Dependencies

- **Unity Physics2D**: Player detection via OverlapCircleNonAlloc
- **Unity Animator**: Animation state control and parameter management
- **Unity Transform**: Position manipulation for movement and bouncing
- **Unity Rigidbody2D**: Physics-based movement and collision
- **Player System**: Stealth state query and position tracking

## Components and Interfaces

### BeeWorkerBehaviorController

Primary MonoBehaviour component managing all enemy behavior.

#### Public Interface

```csharp
public class BeeWorkerBehaviorController : MonoBehaviour
{
    // Animation Event Callbacks
    public void EnableHitBox();
    public void DisableHitBox();
    public void EnableBouncing();
    public void DisableBouncing();
    public void OnDeathAnimationComplete();
}
```

#### Configuration Fields

```csharp
[Header("Core Attributes")]
[SerializeField] private int maxHealth = 3;
[SerializeField] private float attackDamage = 10f;
[SerializeField] private float defense = 5f;
[SerializeField] private float moveSpeed = 3f;

[Header("Detection")]
[SerializeField] private float detectionRadius = 5f;
[SerializeField] private float detectionInterval = 0.2f;
[SerializeField] private LayerMask playerLayer;

[Header("Combat")]
[SerializeField] private float attackRange = 1.5f;
[SerializeField] private float chaseSpeedMultiplier = 1.5f;

[Header("Damage System")]
[SerializeField] private float invulnerabilityDuration = 0.5f;
[SerializeField] private float knockbackForce = 5f;
[SerializeField] private float knockbackDuration = 0.3f;

[Header("Patrol")]
[SerializeField] private Transform[] patrolPoints;
[SerializeField] private float patrolWaitTime = 2f;
[SerializeField] private float smoothTime = 0.3f;

[Header("Visual Bouncing")]
[SerializeField] private Transform visualTransform;
[SerializeField] private float bouncingAmplitudeX = 0.1f;
[SerializeField] private float bouncingAmplitudeY = 0.15f;
[SerializeField] private float bouncingFrequency = 2f;

[Header("References")]
[SerializeField] private Animator animator;
[SerializeField] private Collider2D hitBox;
[SerializeField] private Collider2D hurtBox;
```

#### Internal State

```csharp
private enum EnemyState { Patrol, Combat, Hit, Dead }

private EnemyState currentState = EnemyState.Patrol;
private EnemyState previousState = EnemyState.Patrol;
private int currentHealth;

// Detection
private static Transform s_playerTransform;
private static bool s_playerCached = false;
private float detectionTimer = 0f;
private Collider2D[] detectionResults = new Collider2D[1];
private bool playerDetected = false;

// Patrol
private int currentPatrolIndex = 0;
private float patrolWaitTimer = 0f;
private bool isWaitingAtPoint = false;
private Vector2 velocity = Vector2.zero;

// Combat
private bool isAttacking = false;

// Damage
private bool isInvulnerable = false;
private Coroutine knockbackCoroutine;
private Coroutine invulnerabilityCoroutine;

// Bouncing
private bool isBouncingEnabled = true;
private Vector3 visualOffset;

// Animator Parameters (cached)
private static readonly int IsWalking = Animator.StringToHash("isWalking");
private static readonly int Hit = Animator.StringToHash("Hit");
private static readonly int Die = Animator.StringToHash("Die");
private static readonly int Attack = Animator.StringToHash("Attack");
```

### Player Interface Requirements

The system requires the Player to expose:

```csharp
// Player must have a method or property to check stealth state
public bool IsInStealthMode { get; }
// OR
public bool IsCrouching { get; }
```

### Animation Event Integration

The Animator must call these methods via Animation Events:

- **Attack Animation Start**: `DisableBouncing()`
- **Attack Animation (damage frame)**: `EnableHitBox()`
- **Attack Animation (after damage frame)**: `DisableHitBox()`
- **Attack Animation End**: `EnableBouncing()`
- **Hit Animation End**: `EnableBouncing()`
- **Die Animation End**: `OnDeathAnimationComplete()`

## Data Models

### EnemyState Enum

```csharp
private enum EnemyState
{
    Patrol,   // Moving between patrol points
    Combat,   // Chasing and attacking player
    Hit,      // Taking damage with knockback
    Dead      // Death animation and cleanup
}
```

### Detection Result

```csharp
// Reusable array for Physics2D.OverlapCircleNonAlloc
private Collider2D[] detectionResults = new Collider2D[1];
```

### Patrol State Data

```csharp
private int currentPatrolIndex;      // Current waypoint index
private float patrolWaitTimer;       // Time spent waiting at current point
private bool isWaitingAtPoint;       // Whether currently waiting
private Vector2 velocity;            // SmoothDamp velocity reference
```

### Combat State Data

```csharp
private bool isAttacking;            // Whether attack animation is playing
```

### Damage State Data

```csharp
private bool isInvulnerable;         // Temporary invulnerability flag
private Coroutine knockbackCoroutine;     // Active knockback coroutine
private Coroutine invulnerabilityCoroutine; // Active invulnerability coroutine
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*


### Property 1: State Transitions Based on Player Detection

*For any* BeeWorker in Patrol or Combat state, when the player detection status changes (enters/exits detection radius or enters/exits stealth mode), the BeeWorker should transition to the appropriate state: Combat when player is detected and not in stealth, Patrol when player is lost or enters stealth.

**Validates: Requirements 2.3, 2.4, 2.5**

### Property 2: Damage Triggers Hit State Transition

*For any* BeeWorker in any non-Dead state, when damage is received and currentHealth remains above zero, the BeeWorker should transition to Hit state and store the previous state for restoration after knockback.

**Validates: Requirements 2.6**

### Property 3: Hit State Returns to Previous State

*For any* BeeWorker in Hit state, when knockback completes, the BeeWorker should transition back to the state it was in before taking damage (either Patrol or Combat).

**Validates: Requirements 2.7**

### Property 4: Death is Irreversible

*For any* BeeWorker, once currentHealth reaches zero or below and the BeeWorker transitions to Dead state, it should remain in Dead state with no further state transitions until GameObject destruction.

**Validates: Requirements 2.8, 2.9**

### Property 5: Detection Respects Stealth Mode

*For any* player position within the detection radius, the BeeWorker should detect the player if and only if the player is not in stealth mode.

**Validates: Requirements 3.5, 3.6**

### Property 6: Patrol Follows Sequential Waypoints

*For any* BeeWorker with a non-empty patrol points array, the patrol system should visit waypoints in sequential order, looping back to the first waypoint after reaching the last.

**Validates: Requirements 4.1, 4.4, 4.5**

### Property 7: Patrol Wait Time is Respected

*For any* BeeWorker at a patrol point, the system should wait for exactly patrolWaitTime seconds before moving to the next waypoint.

**Validates: Requirements 4.3**

### Property 8: Animator Walking State Matches Movement

*For any* BeeWorker in Patrol state, the isWalking animator parameter should be true when moving between waypoints and false when waiting at a waypoint.

**Validates: Requirements 4.6, 4.7**

### Property 9: Combat Movement Targets Player

*For any* BeeWorker in Combat state, the movement direction should always point toward the current player position.

**Validates: Requirements 5.1**

### Property 10: Combat Speed is Multiplied

*For any* BeeWorker in Combat state, the movement speed should equal moveSpeed multiplied by chaseSpeedMultiplier.

**Validates: Requirements 5.2**

### Property 11: Combat Walking State is Active

*For any* BeeWorker in Combat state while moving, the isWalking animator parameter should be true.

**Validates: Requirements 5.3**

### Property 12: Attack Triggers Within Range

*For any* BeeWorker in Combat state, when the distance to the player is less than or equal to attackRange, the Attack animator trigger should be activated.

**Validates: Requirements 5.4**

### Property 13: Bouncing State Matches Game State

*For any* BeeWorker, bouncing should be enabled in Patrol state and in Combat state when not attacking, and should be disabled during Attack animation, Hit state, and Dead state.

**Validates: Requirements 6.2, 7.5, 7.6, 7.8, 7.9**

### Property 14: Bouncing Reset on Disable

*For any* BeeWorker, when bouncing is disabled, the visual transform's localPosition should be reset to Vector3.zero.

**Validates: Requirements 7.10**

### Property 15: Damage Processing Triggers Responses

*For any* collision with an object tagged "PlayerAttack" on the HurtBox, the system should subtract damage from currentHealth, trigger the Hit animator parameter, apply knockback, and activate invulnerability.

**Validates: Requirements 8.1, 8.2, 8.3, 8.4, 8.5**

### Property 16: Invulnerability Blocks Damage

*For any* BeeWorker in invulnerable state, subsequent damage events should be ignored and should not reduce currentHealth.

**Validates: Requirements 8.6**

### Property 17: Death Triggers Die Animation

*For any* BeeWorker, when currentHealth reaches zero or below, the Die animator trigger should be activated.

**Validates: Requirements 8.7**

### Property 18: Knockback Direction is Away from Player

*For any* damage event, the knockback direction should be calculated as the normalized vector from the player position to the BeeWorker position.

**Validates: Requirements 9.1, 9.2**

### Property 19: Knockback Has Correct Duration and Force

*For any* knockback event, the BeeWorker should move in the knockback direction for exactly knockbackDuration seconds with displacement determined by knockbackForce.

**Validates: Requirements 9.3, 9.4**

### Property 20: Knockback Blocks Normal Movement

*For any* BeeWorker during active knockback, normal patrol and combat movement should be prevented.

**Validates: Requirements 9.5**

### Property 21: Dead State Disables Systems

*For any* BeeWorker in Dead state, all colliders should be disabled, the script should be disabled, and all movement should stop.

**Validates: Requirements 14.1, 14.2, 14.5**

## Error Handling

### Invalid Configuration

**Empty Patrol Points**:
- Detection: Check `patrolPoints == null || patrolPoints.Length == 0` in Awake
- Response: Log warning and remain stationary in Patrol state
- Graceful degradation: System continues to function for detection and combat

**Missing References**:
- Detection: Validate all SerializeField references in Awake
- Response: Log errors for missing critical references (animator, hitBox, hurtBox, visualTransform)
- Fail-fast: Disable script if critical references are missing

**Invalid Layer Configuration**:
- Detection: Check `playerLayer.value == 0` in Awake
- Response: Log error indicating player layer not configured
- Impact: Detection will not function, but system won't crash

### Runtime Errors

**Player Reference Not Found**:
- Detection: Static caching fails to find player GameObject
- Response: Log error once, continue attempting to cache on subsequent frames
- Graceful degradation: Detection disabled until player found

**Null Player Transform**:
- Detection: Check `s_playerTransform == null` before distance calculations
- Response: Skip detection and combat logic for current frame
- Recovery: Attempt to re-cache player reference

**Animation State Errors**:
- Detection: Animator parameter not found (StringToHash returns valid hash even for non-existent parameters)
- Response: Unity logs warning, animation continues with default behavior
- Impact: Visual feedback may be incorrect but gameplay continues

**Coroutine Interruption**:
- Detection: State change while knockback or invulnerability coroutine active
- Response: Stop active coroutines before starting new ones
- Cleanup: Reset flags (isInvulnerable, knockback state) when coroutines stop

### Edge Cases

**Zero Health Initialization**:
- Detection: maxHealth set to 0 or negative in Inspector
- Response: Clamp to minimum value of 1 in Awake
- Log: Warning about invalid configuration

**Overlapping Patrol Points**:
- Detection: Multiple patrol points at same position
- Response: System functions normally, appears stationary at overlapping points
- Impact: Visual only, no functional issues

**Player Exactly at Detection Boundary**:
- Detection: Player distance equals detectionRadius exactly
- Response: Use `<=` comparison to include boundary
- Consistent behavior: Player at boundary is detected

**Simultaneous State Transitions**:
- Detection: Multiple conditions trigger state changes in same frame
- Response: Process in priority order: Dead > Hit > Combat > Patrol
- Deterministic: Always choose highest priority state

## Testing Strategy

### Dual Testing Approach

The BeeWorker AI system requires both unit tests and property-based tests for comprehensive coverage:

**Unit Tests** focus on:
- Specific initialization examples (starting in Patrol state, HitBox disabled)
- Edge cases (empty patrol points, zero health, boundary conditions)
- Integration points (Animation Event callbacks, collision detection)
- Error conditions (missing references, invalid configuration)

**Property-Based Tests** focus on:
- Universal state transition rules across all possible inputs
- Detection behavior with randomized positions and stealth states
- Movement calculations with randomized speeds and positions
- Damage calculations with randomized health and damage values
- Knockback behavior with randomized forces and directions

Together, these approaches provide comprehensive coverage: unit tests catch concrete bugs in specific scenarios, while property tests verify general correctness across the input space.

### Property-Based Testing Configuration

**Framework**: Use Unity Test Framework with a C# property-based testing library such as:
- **FsCheck** (recommended): Mature F# library with C# support
- **CsCheck**: Native C# property-based testing library
- **Hedgehog**: C# port of Haskell's Hedgehog library

**Configuration**:
- Minimum 100 iterations per property test (due to randomization)
- Each property test must reference its design document property
- Tag format: `// Feature: beeworker-enemy-ai, Property {number}: {property_text}`

**Example Property Test Structure**:

```csharp
[Test]
public void Property_4_Death_Is_Irreversible()
{
    // Feature: beeworker-enemy-ai, Property 4: Death is Irreversible
    // For any BeeWorker, once in Dead state, should remain Dead
    
    Prop.ForAll<int, float>(
        (initialHealth, damageAmount) =>
        {
            // Arrange: Create BeeWorker with random health
            var bee = CreateBeeWorker(initialHealth);
            
            // Act: Apply lethal damage
            bee.TakeDamage(damageAmount);
            
            // Assert: Should be in Dead state
            Assert.AreEqual(EnemyState.Dead, bee.CurrentState);
            
            // Act: Attempt various state transitions
            bee.DetectPlayer();
            bee.LosePlayer();
            bee.TakeDamage(10f);
            
            // Assert: Should still be in Dead state
            Assert.AreEqual(EnemyState.Dead, bee.CurrentState);
        }
    ).QuickCheckThrowOnFailure(iterations: 100);
}
```

### Unit Test Coverage

**Initialization Tests**:
- Verify starting state is Patrol
- Verify HitBox is initially disabled
- Verify currentHealth equals maxHealth
- Verify bouncing is initially enabled

**State Transition Tests**:
- Test Patrol → Combat transition with player detection
- Test Combat → Patrol transition with player loss
- Test any state → Hit transition with damage
- Test Hit → previous state transition after knockback
- Test any state → Dead transition with lethal damage

**Movement Tests**:
- Test patrol waypoint progression
- Test patrol wait timing
- Test combat chase direction
- Test combat speed multiplier
- Test movement stops in Dead state

**Damage Tests**:
- Test health reduction calculation
- Test knockback direction calculation
- Test invulnerability duration
- Test damage ignored during invulnerability
- Test death at zero health

**Animation Tests**:
- Test isWalking parameter during patrol movement
- Test isWalking parameter during patrol waiting
- Test isWalking parameter during combat
- Test Hit trigger on damage
- Test Die trigger on death
- Test Attack trigger within range

**Edge Case Tests**:
- Test empty patrol points array
- Test single patrol point
- Test player exactly at detection boundary
- Test player exactly at attack boundary
- Test zero damage
- Test overkill damage (health goes negative)

### Integration Tests

**Scene-Based Tests** (using Testes.unity):
- Test complete patrol cycle with multiple waypoints
- Test detection → chase → attack → kill sequence
- Test stealth mode prevents detection
- Test knockback moves BeeWorker away from player
- Test multiple BeeWorkers don't interfere with each other
- Test Animation Events properly enable/disable HitBox

### Performance Tests

**Profiling Validation**:
- Verify no allocations in Update loop (use Unity Profiler)
- Verify detection interval reduces physics queries
- Verify static player caching works across multiple instances
- Measure frame time with 10+ active BeeWorkers
- Verify no GameObject.Find calls during gameplay

**Benchmarks**:
- Target: 20+ BeeWorkers at 60 FPS on mid-range hardware
- Detection overhead: < 0.1ms per BeeWorker per frame
- State machine overhead: < 0.05ms per BeeWorker per frame

### Test Execution

**Continuous Integration**:
- Run all unit tests on every commit
- Run property tests on pull requests
- Run integration tests before release builds
- Run performance tests weekly

**Manual Testing**:
- Playtest in Testes.unity scene
- Verify visual polish (bouncing, animations)
- Verify audio feedback (if implemented)
- Verify feel of combat (knockback, invulnerability timing)
