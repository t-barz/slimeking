# Implementation Plan: BeeWorker Enemy AI System

## Overview

This implementation plan breaks down the BeeWorker Enemy AI system into discrete, incremental coding tasks. Each task builds on previous work, with early validation through property-based tests to catch errors quickly. The implementation follows Unity best practices with performance optimization as a core requirement.

## Tasks

- [x] 1. Create core script structure and configuration
  - Create `BeeWorkerBehaviorController.cs` in `Assets/_Code/Gameplay/Enemies/`
  - Add namespace `TheSlimeKing.Gameplay`
  - Define `EnemyState` enum (Patrol, Combat, Hit, Dead)
  - Add all SerializeField configuration parameters with default values
  - Add Header attributes for Inspector organization
  - Implement region structure following project conventions
  - _Requirements: 1.1-1.17, 2.1_

- [x] 2. Implement static player caching and references
  - Add static `s_playerTransform` and `s_playerCached` fields
  - Implement `CachePlayerReference()` method using GameObject.FindGameObjectWithTag
  - Call caching in Awake (one-time operation)
  - Add validation for missing player with error logging
  - Cache Animator component reference
  - Cache HitBox and HurtBox Collider2D references
  - Cache visual Transform reference
  - _Requirements: 3.7, 13.8-13.10_

- [x] 3. Implement Animator parameter hashing
  - Define static readonly int fields for all Animator parameters
  - Use `Animator.StringToHash` for: IsWalking, Hit, Die, Attack
  - Add XML documentation explaining performance benefit
  - _Requirements: 10.1, 10.7_

- [x] 4. Implement state machine foundation
  - Add `currentState` and `previousState` private fields
  - Initialize `currentState` to `EnemyState.Patrol` in Awake
  - Initialize `currentHealth` to `maxHealth` in Awake
  - Create `Update()` method with switch statement for state handling
  - Create empty state handler methods: `UpdatePatrol()`, `UpdateCombat()`, `UpdateHit()`, `UpdateDead()`
  - _Requirements: 2.2_

- [ ]* 5. Write property test for initial state
  - **Property: Initialization starts in Patrol state**
  - **Validates: Requirements 2.2**
  - Test that newly instantiated BeeWorker starts in Patrol state
  - Test that currentHealth equals maxHealth on initialization
  - Test that HitBox is disabled on initialization

- [x] 6. Implement player detection system
  - Add `detectionTimer`, `detectionResults` array, and `playerDetected` boolean fields
  - Create `CheckPlayerDetection()` method using `Physics2D.OverlapCircleNonAlloc`
  - Configure LayerMask to detect only Player layer
  - Implement detection interval timing in Update
  - Add stealth mode check (access Player's `IsInStealthMode` or `IsCrouching` property)
  - Use `sqrMagnitude` for distance comparisons
  - _Requirements: 3.1-3.6, 3.8_

- [ ]* 7. Write property test for detection with stealth
  - **Property 5: Detection respects stealth mode**
  - **Validates: Requirements 3.5, 3.6**
  - Generate random positions within detection radius
  - Test detection occurs when player not in stealth
  - Test detection fails when player in stealth
  - Verify detection uses correct layer mask

- [x] 8. Implement patrol movement system
  - Add patrol state fields: `currentPatrolIndex`, `patrolWaitTimer`, `isWaitingAtPoint`, `velocity`
  - Implement `UpdatePatrol()` with null/empty patrol points check
  - Implement movement to current patrol point using `Vector2.SmoothDamp`
  - Implement arrival detection using sqrMagnitude
  - Implement wait timer at patrol points
  - Implement `SelectNextPatrolPoint()` with wraparound logic
  - Set `isWalking` animator parameter based on movement/waiting state
  - _Requirements: 4.1-4.8_

- [ ]* 9. Write property test for patrol waypoint sequence
  - **Property 6: Patrol follows sequential waypoints**
  - **Validates: Requirements 4.1, 4.4, 4.5**
  - Generate random patrol point configurations
  - Test sequential waypoint visitation
  - Test wraparound from last to first waypoint
  - Verify patrol continues indefinitely

- [ ]* 10. Write property test for patrol wait timing
  - **Property 7: Patrol wait time is respected**
  - **Validates: Requirements 4.3**
  - Generate random wait times
  - Test BeeWorker waits correct duration at each point
  - Verify movement resumes after wait completes

- [x] 11. Implement state transitions for detection
  - In `UpdatePatrol()`, check `playerDetected` flag
  - Transition to Combat state when player detected and not in stealth
  - In `UpdateCombat()`, check `playerDetected` flag
  - Transition to Patrol state when player lost or enters stealth
  - Store previous state before transitions
  - _Requirements: 2.3, 2.4, 2.5_

- [ ]* 12. Write property test for detection-based transitions
  - **Property 1: State transitions based on player detection**
  - **Validates: Requirements 2.3, 2.4, 2.5**
  - Generate random detection status changes
  - Test Patrol → Combat when player detected
  - Test Combat → Patrol when player lost
  - Test Combat → Patrol when player enters stealth

- [x] 13. Implement combat chase system
  - Implement `UpdateCombat()` movement toward player position
  - Calculate direction to player using normalized vector
  - Apply movement at `moveSpeed * chaseSpeedMultiplier`
  - Set `isWalking` animator parameter to true during chase
  - Check distance to player using sqrMagnitude
  - Trigger Attack animator parameter when within attack range
  - _Requirements: 5.1-5.4_

- [ ]* 14. Write property test for combat movement
  - **Property 9: Combat movement targets player**
  - **Validates: Requirements 5.1**
  - Generate random player positions
  - Test movement direction points toward player
  - Verify continuous tracking as player moves

- [ ]* 15. Write property test for combat speed
  - **Property 10: Combat speed is multiplied**
  - **Validates: Requirements 5.2**
  - Generate random base speeds and multipliers
  - Test actual movement speed equals base * multiplier
  - Verify speed is consistent during chase

- [x] 16. Implement visual bouncing system
  - Add bouncing fields: `isBouncingEnabled`, `visualOffset`
  - Create `UpdateBouncing()` method
  - Calculate X offset using `Mathf.Sin(Time.time * bouncingFrequency) * bouncingAmplitudeX`
  - Calculate Y offset using `Mathf.Cos(Time.time * bouncingFrequency * 1.3f) * bouncingAmplitudeY`
  - Apply offset to `visualTransform.localPosition`
  - Reset to Vector3.zero when bouncing disabled
  - Call `UpdateBouncing()` from Update
  - _Requirements: 7.1-7.4, 7.10_

- [x] 17. Implement bouncing state control
  - Enable bouncing in Patrol state
  - Enable bouncing in Combat state when not attacking
  - Create `EnableBouncing()` and `DisableBouncing()` public methods
  - Disable bouncing in Hit state
  - Disable bouncing in Dead state
  - _Requirements: 7.5-7.9_

- [ ]* 18. Write property test for bouncing state
  - **Property 13: Bouncing state matches game state**
  - **Validates: Requirements 6.2, 7.5, 7.6, 7.8, 7.9**
  - Test bouncing enabled in Patrol state
  - Test bouncing enabled in Combat when not attacking
  - Test bouncing disabled during Attack
  - Test bouncing disabled in Hit state
  - Test bouncing disabled in Dead state

- [x] 19. Checkpoint - Ensure movement and detection work
  - Ensure all tests pass, ask the user if questions arise.

- [x] 20. Implement damage reception system
  - Add damage fields: `isInvulnerable`, `knockbackCoroutine`, `invulnerabilityCoroutine`
  - Implement `OnTriggerEnter2D()` to detect PlayerAttack tag on HurtBox
  - Create `TakeDamage(float damage)` method
  - Check invulnerability flag and return early if invulnerable
  - Subtract damage from currentHealth
  - Trigger Hit animator parameter
  - Store previous state before transitioning to Hit
  - Call knockback and invulnerability coroutines
  - Check for death (currentHealth <= 0) and trigger Die animator parameter
  - Transition to Dead state if health depleted
  - _Requirements: 8.1-8.8_

- [ ]* 21. Write property test for damage processing
  - **Property 15: Damage processing triggers responses**
  - **Validates: Requirements 8.1, 8.2, 8.3, 8.4, 8.5**
  - Generate random damage amounts
  - Test health reduction is correct
  - Test Hit animator trigger is called
  - Test knockback is applied
  - Test invulnerability is activated

- [ ]* 22. Write property test for invulnerability
  - **Property 16: Invulnerability blocks damage**
  - **Validates: Requirements 8.6**
  - Apply damage to trigger invulnerability
  - Apply subsequent damage during invulnerability
  - Test that health does not decrease
  - Test that damage is processed after invulnerability expires

- [x] 23. Implement knockback system
  - Create `ApplyKnockback()` coroutine
  - Calculate knockback direction away from player position
  - Apply movement over knockbackDuration using Lerp or similar
  - Use knockbackForce to determine displacement
  - Prevent normal movement during knockback
  - Transition back to previous state after knockback completes
  - Stop any existing knockback coroutine before starting new one
  - _Requirements: 9.1-9.5, 2.7_

- [ ]* 24. Write property test for knockback direction
  - **Property 18: Knockback direction is away from player**
  - **Validates: Requirements 9.1, 9.2**
  - Generate random player and BeeWorker positions
  - Apply damage to trigger knockback
  - Test knockback direction is normalized vector away from player
  - Verify BeeWorker moves in correct direction

- [ ]* 25. Write property test for knockback timing
  - **Property 19: Knockback has correct duration and force**
  - **Validates: Requirements 9.3, 9.4**
  - Generate random knockback durations and forces
  - Test knockback lasts exactly the specified duration
  - Test displacement matches force parameter

- [x] 26. Implement invulnerability system
  - Create `ApplyInvulnerability()` coroutine
  - Set `isInvulnerable` flag to true
  - Wait for invulnerabilityDuration
  - Set `isInvulnerable` flag to false
  - Stop any existing invulnerability coroutine before starting new one
  - _Requirements: 8.5, 8.6_

- [x] 27. Implement HitBox control methods
  - Implement `EnableHitBox()` public method to enable HitBox collider
  - Implement `DisableHitBox()` public method to disable HitBox collider
  - Ensure HitBox is disabled in Awake
  - _Requirements: 6.3-6.7_

- [x] 28. Implement attack system
  - Add `isAttacking` boolean field
  - In `UpdateCombat()`, check if within attack range using sqrMagnitude
  - Trigger Attack animator parameter when in range
  - Set `isAttacking` flag when attack triggered
  - Call `DisableBouncing()` when attack starts
  - _Requirements: 6.1, 6.2_

- [ ]* 29. Write property test for attack triggering
  - **Property 12: Attack triggers within range**
  - **Validates: Requirements 5.4**
  - Generate random distances to player
  - Test Attack trigger activates when within attack range
  - Test Attack trigger does not activate when outside range
  - Verify bouncing is disabled during attack

- [x] 30. Implement death state
  - Create `UpdateDead()` method
  - Disable all colliders (main collider, HitBox, HurtBox)
  - Disable the BeeWorkerBehaviorController script
  - Stop all movement
  - Disable bouncing
  - Create `OnDeathAnimationComplete()` public method
  - Destroy GameObject in `OnDeathAnimationComplete()`
  - _Requirements: 14.1-14.5_

- [ ]* 31. Write property test for death irreversibility
  - **Property 4: Death is irreversible**
  - **Validates: Requirements 2.8, 2.9**
  - Apply lethal damage to BeeWorker
  - Test transition to Dead state
  - Attempt various state transitions
  - Verify BeeWorker remains in Dead state

- [ ]* 32. Write property test for dead state cleanup
  - **Property 21: Dead state disables systems**
  - **Validates: Requirements 14.1, 14.2, 14.5**
  - Transition BeeWorker to Dead state
  - Test all colliders are disabled
  - Test script is disabled
  - Test movement has stopped

- [x] 33. Implement debug Gizmos
  - Implement `OnDrawGizmosSelected()` method
  - Draw yellow wire sphere for detection radius
  - Draw red wire sphere for attack range
  - Draw cyan lines connecting patrol points
  - Draw cyan wire spheres at each patrol point
  - Draw colored cube above BeeWorker indicating current state (green=Patrol, red=Combat, white=Hit, black=Dead)
  - Wrap in `#if UNITY_EDITOR` preprocessor directive
  - _Requirements: 12.1-12.10_

- [x] 34. Configure BeeWorkerA prefab
  - Open `Assets/_Prefabs/Characters/BeeWorkerA.prefab`
  - Attach `BeeWorkerBehaviorController` script to root GameObject
  - Set root GameObject layer to Enemy (layer 7)
  - Add Rigidbody2D with Body Type = Dynamic, Gravity Scale = 0
  - Add CircleCollider2D for main collision
  - Add Animator component
  - Create child GameObject named "Visual" with SpriteRenderer
  - Create child GameObject with HurtBox CircleCollider2D (Is Trigger = true)
  - Create child GameObject with HitBox CapsuleCollider2D (Is Trigger = true, Enabled = false, Layer = EnemyAttack)
  - Assign all references in Inspector (animator, hitBox, hurtBox, visualTransform)
  - Set default configuration values
  - _Requirements: 13.1-13.10_

- [x] 35. Create patrol points in Testes scene
  - Open `Assets/_Scenes/Testes.unity`
  - Create empty GameObjects as patrol waypoints
  - Position waypoints in a patrol route
  - Assign waypoints to BeeWorkerA instance's patrolPoints array
  - Test patrol behavior in Play mode
  - _Requirements: 4.1_

- [x] 36. Configure Animation Events
  - Open BeeWorker Animator Controller
  - Add Animation Event to Attack animation start frame: call `DisableBouncing()`
  - Add Animation Event to Attack animation damage frame: call `EnableHitBox()`
  - Add Animation Event to Attack animation after damage frame: call `DisableHitBox()`
  - Add Animation Event to Attack animation end frame: call `EnableBouncing()`
  - Add Animation Event to Hit animation end frame: call `EnableBouncing()`
  - Add Animation Event to Die animation end frame: call `OnDeathAnimationComplete()`
  - _Requirements: 6.5, 6.6_

- [x] 37. Implement error handling and validation
  - Add validation in Awake for empty/null patrol points (log warning)
  - Add validation for missing references (log errors, disable script if critical)
  - Add validation for invalid layer configuration (log error)
  - Add null checks before player transform access
  - Add coroutine cleanup when stopping active coroutines
  - Clamp maxHealth to minimum value of 1
  - _Requirements: Error Handling section_

- [ ]* 38. Write unit tests for edge cases
  - Test empty patrol points array behavior
  - Test single patrol point behavior
  - Test player exactly at detection boundary
  - Test player exactly at attack boundary
  - Test zero damage
  - Test overkill damage (health goes negative)
  - Test missing player reference handling

- [x] 39. Final checkpoint - Integration testing
  - Ensure all tests pass, ask the user if questions arise.
  - Test complete patrol cycle in Testes scene
  - Test detection → chase → attack → kill sequence
  - Test stealth mode prevents detection
  - Test knockback moves BeeWorker away from player
  - Test multiple BeeWorkers don't interfere
  - Test Animation Events properly control HitBox
  - Verify no allocations in Update using Unity Profiler
  - Verify performance with 10+ active BeeWorkers

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties
- Unit tests validate specific examples and edge cases
- All code must follow project coding standards (regions, naming conventions, XML documentation)
- Performance requirements are critical - no allocations in Update, use caching and NonAlloc methods
