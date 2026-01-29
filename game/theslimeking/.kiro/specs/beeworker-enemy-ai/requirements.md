# Requirements Document: BeeWorker Enemy AI System

## Introduction

The BeeWorker Enemy AI System implements a complete enemy behavior system for the BeeWorker enemy character in The Slime King. This system provides intelligent patrol and combat behaviors, player detection with stealth awareness, visual movement effects, and a complete damage/death system. The implementation must be performance-optimized following Unity best practices to ensure smooth gameplay even with multiple enemies active simultaneously.

## Glossary

- **BeeWorker**: A flying enemy character that patrols designated areas and attacks the player when detected
- **System**: The BeeWorkerBehaviorController component and its associated systems
- **Player**: The player-controlled slime character
- **Stealth_Mode**: The player's crouched state where they cannot be detected by enemies
- **Detection_Radius**: The circular area around the BeeWorker where it can detect the player
- **Attack_Range**: The distance at which the BeeWorker can execute an attack
- **Patrol_Point**: A Transform position that defines a waypoint in the patrol route
- **HitBox**: A trigger collider that deals damage to the player when active
- **HurtBox**: A trigger collider that receives damage from player attacks
- **Bouncing**: A continuous visual oscillation effect that makes movement appear organic
- **Knockback**: A forced movement effect applied when taking damage
- **Invulnerability**: A temporary state where the BeeWorker cannot take damage

## Requirements

### Requirement 1: Core Attributes and Configuration

**User Story:** As a game designer, I want to configure enemy attributes through the Inspector, so that I can balance gameplay without modifying code.

#### Acceptance Criteria

1. THE System SHALL expose maxHealth (default: 3) as a SerializeField parameter
2. THE System SHALL expose attackDamage (default: 10) as a SerializeField parameter
3. THE System SHALL expose defense (default: 5) as a SerializeField parameter
4. THE System SHALL expose moveSpeed (default: 3) as a SerializeField parameter
5. THE System SHALL initialize currentHealth to maxHealth on Awake
6. THE System SHALL expose detectionRadius as a SerializeField parameter
7. THE System SHALL expose detectionInterval (default: 0.2) as a SerializeField parameter
8. THE System SHALL expose attackRange as a SerializeField parameter
9. THE System SHALL expose chaseSpeedMultiplier (default: 1.5) as a SerializeField parameter
10. THE System SHALL expose invulnerabilityDuration as a SerializeField parameter
11. THE System SHALL expose knockbackForce as a SerializeField parameter
12. THE System SHALL expose knockbackDuration as a SerializeField parameter
13. THE System SHALL expose patrolPoints as a SerializeField Transform array
14. THE System SHALL expose patrolWaitTime as a SerializeField parameter
15. THE System SHALL expose bouncingAmplitudeX as a SerializeField parameter
16. THE System SHALL expose bouncingAmplitudeY as a SerializeField parameter
17. THE System SHALL expose bouncingFrequency as a SerializeField parameter

### Requirement 2: State Machine Implementation

**User Story:** As a developer, I want a clear state machine architecture, so that enemy behavior is predictable and maintainable.

#### Acceptance Criteria

1. THE System SHALL implement four distinct states: Patrol, Combat, Hit, and Dead
2. WHEN initialized, THE System SHALL start in the Patrol state
3. WHEN in Patrol state AND Player enters Detection_Radius AND Player is not in Stealth_Mode, THE System SHALL transition to Combat state
4. WHEN in Combat state AND Player exits Detection_Radius, THE System SHALL transition to Patrol state
5. WHEN in Combat state AND Player enters Stealth_Mode, THE System SHALL transition to Patrol state
6. WHEN in any state AND BeeWorker takes damage AND currentHealth is greater than zero, THE System SHALL transition to Hit state
7. WHEN in Hit state AND knockback completes, THE System SHALL transition to the previous state (Patrol or Combat)
8. WHEN currentHealth reaches zero or below, THE System SHALL transition to Dead state
9. WHEN in Dead state, THE System SHALL remain in Dead state until GameObject destruction

### Requirement 3: Player Detection System

**User Story:** As a player, I want enemies to detect me realistically, so that stealth gameplay feels meaningful.

#### Acceptance Criteria

1. THE System SHALL implement 360-degree circular detection using Physics2D.OverlapCircleNonAlloc
2. THE System SHALL check for player detection at intervals defined by detectionInterval
3. THE System SHALL use a reusable Collider2D array for detection to avoid allocations
4. THE System SHALL configure detection to use only the Player layer via LayerMask
5. WHEN Player is within Detection_Radius AND Player is not in Stealth_Mode, THE System SHALL detect the Player
6. WHEN Player is within Detection_Radius AND Player is in Stealth_Mode, THE System SHALL not detect the Player
7. THE System SHALL cache the Player Transform reference statically to avoid repeated Find operations
8. THE System SHALL use sqrMagnitude for distance comparisons to avoid expensive square root calculations

### Requirement 4: Patrol Movement System

**User Story:** As a game designer, I want enemies to patrol designated areas, so that levels feel alive and populated.

#### Acceptance Criteria

1. WHEN in Patrol state AND patrolPoints array is not empty, THE System SHALL move between patrol points sequentially
2. WHEN moving to a Patrol_Point, THE System SHALL use Vector2.SmoothDamp for smooth movement
3. WHEN BeeWorker reaches a Patrol_Point, THE System SHALL wait for patrolWaitTime seconds
4. WHEN wait time completes, THE System SHALL select the next Patrol_Point in the array
5. WHEN reaching the last Patrol_Point, THE System SHALL loop back to the first Patrol_Point
6. WHEN moving between patrol points, THE System SHALL set Animator parameter isWalking to true
7. WHEN waiting at a Patrol_Point, THE System SHALL set Animator parameter isWalking to false
8. WHEN patrolPoints array is empty or null, THE System SHALL remain stationary

### Requirement 5: Combat Chase System

**User Story:** As a player, I want enemies to chase me when detected, so that combat feels engaging and threatening.

#### Acceptance Criteria

1. WHEN in Combat state, THE System SHALL move toward the Player position
2. WHEN in Combat state, THE System SHALL move at speed equal to moveSpeed multiplied by chaseSpeedMultiplier
3. WHEN in Combat state AND moving, THE System SHALL set Animator parameter isWalking to true
4. WHEN in Combat state AND distance to Player is less than or equal to Attack_Range, THE System SHALL trigger the Attack animation
5. WHEN in Combat state, THE System SHALL continuously update movement direction toward Player position

### Requirement 6: Attack System

**User Story:** As a player, I want enemy attacks to be clearly telegraphed, so that I can react and avoid damage.

#### Acceptance Criteria

1. WHEN BeeWorker is within Attack_Range of Player, THE System SHALL trigger the Attack Animator parameter
2. WHEN Attack animation begins, THE System SHALL disable bouncing visual effect
3. THE System SHALL provide a public EnableHitBox method
4. THE System SHALL provide a public DisableHitBox method
5. WHEN EnableHitBox is called, THE System SHALL enable the HitBox Collider2D
6. WHEN DisableHitBox is called, THE System SHALL disable the HitBox Collider2D
7. THE System SHALL initialize with HitBox disabled

### Requirement 7: Visual Bouncing System

**User Story:** As a player, I want enemy movement to feel organic and alive, so that the game world feels polished.

#### Acceptance Criteria

1. THE System SHALL apply continuous oscillating movement to a child visual Transform
2. THE System SHALL use Mathf.Sin for X-axis oscillation with bouncingAmplitudeX
3. THE System SHALL use Mathf.Cos for Y-axis oscillation with bouncingAmplitudeY
4. THE System SHALL use bouncingFrequency to control oscillation speed
5. WHEN in Patrol state, THE System SHALL enable bouncing
6. WHEN in Combat state AND not attacking, THE System SHALL enable bouncing
7. WHEN Attack animation is playing, THE System SHALL disable bouncing
8. WHEN in Hit state, THE System SHALL disable bouncing
9. WHEN in Dead state, THE System SHALL disable bouncing
10. WHEN bouncing is disabled, THE System SHALL reset visual Transform localPosition to zero

### Requirement 8: Damage Reception System

**User Story:** As a player, I want my attacks to visibly affect enemies, so that combat feels impactful.

#### Acceptance Criteria

1. WHEN an object with tag "PlayerAttack" collides with the HurtBox, THE System SHALL process damage
2. WHEN processing damage, THE System SHALL subtract attack damage from currentHealth
3. WHEN processing damage, THE System SHALL trigger the Hit Animator parameter
4. WHEN processing damage, THE System SHALL apply knockback away from the Player
5. WHEN processing damage, THE System SHALL activate invulnerability for invulnerabilityDuration seconds
6. WHEN invulnerable, THE System SHALL ignore subsequent damage
7. WHEN currentHealth reaches zero or below, THE System SHALL trigger the Die Animator parameter
8. WHEN currentHealth reaches zero or below, THE System SHALL transition to Dead state

### Requirement 9: Knockback System

**User Story:** As a player, I want enemies to react physically to my attacks, so that combat feels responsive.

#### Acceptance Criteria

1. WHEN knockback is applied, THE System SHALL calculate direction away from Player position
2. WHEN knockback is applied, THE System SHALL move the BeeWorker in the knockback direction
3. WHEN knockback is applied, THE System SHALL apply movement for knockbackDuration seconds
4. WHEN knockback is applied, THE System SHALL use knockbackForce to determine movement distance
5. WHEN knockback is active, THE System SHALL prevent normal movement behavior

### Requirement 10: Animator Integration

**User Story:** As an animator, I want the code to properly control animation states, so that visual feedback matches gameplay.

#### Acceptance Criteria

1. THE System SHALL cache all Animator parameters using Animator.StringToHash
2. THE System SHALL define isWalking as a boolean Animator parameter
3. THE System SHALL define Hit as a trigger Animator parameter
4. THE System SHALL define Die as a trigger Animator parameter
5. THE System SHALL define Attack as a trigger Animator parameter
6. THE System SHALL store parameter hashes as static readonly integers
7. WHEN setting Animator parameters, THE System SHALL use cached hashes instead of strings

### Requirement 11: Performance Optimization

**User Story:** As a developer, I want the enemy AI to be performant, so that multiple enemies can exist without frame rate drops.

#### Acceptance Criteria

1. THE System SHALL NOT use GameObject.Find or FindObjectOfType in Update or FixedUpdate methods
2. THE System SHALL use Physics2D.OverlapCircleNonAlloc with a reusable array for detection
3. THE System SHALL implement detection checks at intervals rather than every frame
4. THE System SHALL use sqrMagnitude for all distance comparisons
5. THE System SHALL cache the Player Transform reference statically
6. THE System SHALL use Animator.StringToHash for all Animator parameter access
7. THE System SHALL apply bouncing to a child Transform to avoid affecting collision
8. THE System SHALL configure LayerMask to detect only the Player layer

### Requirement 12: Debug Visualization

**User Story:** As a developer, I want visual debugging tools, so that I can quickly identify and fix behavior issues.

#### Acceptance Criteria

1. THE System SHALL implement OnDrawGizmosSelected for debug visualization
2. WHEN selected in Editor, THE System SHALL draw a yellow wire sphere representing Detection_Radius
3. WHEN selected in Editor, THE System SHALL draw a red wire sphere representing Attack_Range
4. WHEN selected in Editor AND patrolPoints is not empty, THE System SHALL draw cyan lines connecting patrol points
5. WHEN selected in Editor AND patrolPoints is not empty, THE System SHALL draw cyan wire spheres at each patrol point
6. WHEN selected in Editor AND in Play mode, THE System SHALL draw a colored cube above the BeeWorker indicating current state
7. WHEN in Patrol state, THE System SHALL draw the state indicator in green
8. WHEN in Combat state, THE System SHALL draw the state indicator in red
9. WHEN in Hit state, THE System SHALL draw the state indicator in white
10. WHEN in Dead state, THE System SHALL draw the state indicator in black

### Requirement 13: Scene Configuration

**User Story:** As a level designer, I want to easily place and configure enemies in scenes, so that I can create varied encounters.

#### Acceptance Criteria

1. THE System SHALL be attached to the BeeWorkerA prefab at Assets/_Prefabs/Characters/BeeWorkerA.prefab
2. THE BeeWorkerA prefab SHALL have a root GameObject with Rigidbody2D, main Collider2D, and the System script
3. THE BeeWorkerA prefab SHALL have a child GameObject named "Visual" for bouncing effects
4. THE BeeWorkerA prefab SHALL have a child GameObject with HurtBox trigger Collider2D
5. THE BeeWorkerA prefab SHALL have a child GameObject with HitBox trigger Collider2D (initially disabled)
6. THE BeeWorkerA root GameObject SHALL be on the Enemy layer (layer 7)
7. THE HitBox GameObject SHALL be on the EnemyAttack layer (layer 9)
8. THE System SHALL reference the visual Transform via SerializeField
9. THE System SHALL reference the HitBox Collider2D via SerializeField
10. THE System SHALL reference the HurtBox Collider2D via SerializeField

### Requirement 14: Death and Cleanup

**User Story:** As a player, I want defeated enemies to be removed properly, so that the game remains performant.

#### Acceptance Criteria

1. WHEN in Dead state, THE System SHALL disable all colliders
2. WHEN in Dead state, THE System SHALL disable the System script
3. WHEN Die animation completes, THE System SHALL destroy the GameObject
4. WHEN in Dead state, THE System SHALL prevent all state transitions except destruction
5. WHEN in Dead state, THE System SHALL stop all movement
