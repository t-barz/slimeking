# Implementation Plan - Sistema de Teletransporte Entre Cenas

## Overview

Este plano de implementação detalha as tarefas necessárias para evoluir o sistema de teletransporte existente, adicionando suporte para teletransporte entre cenas, pré-carregamento inteligente, e efeitos sonoros. O plano segue uma abordagem incremental, construindo funcionalidade core primeiro e adicionando features progressivamente.

## Tasks

- [x] 1. Extend TeleportPoint for cross-scene support

  - Add new serialized fields for cross-scene configuration (isCrossSceneTeleport, destinationSceneName)
  - Add preloading configuration fields (enablePreloading, preloadProximityRadius)
  - Add audio configuration fields (teleportStartSound, teleportMidSound, teleportEndSound)
  - Update OnValidate to configure preload trigger when needed
  - Add method to determine teleport type (same-scene vs cross-scene)
  - _Requirements: 1.4, 4.1, 3.4_

- [x] 2. Create TeleportManager singleton

  - Create TeleportManager.cs with singleton pattern
  - Add configuration fields (maxPreloadedScenes, unloadDelay, audioSource, defaultVolume)
  - Implement Awake for singleton initialization
  - Add IsTeleporting property to prevent concurrent teleports
  - _Requirements: 1.1, 5.4, 6.4_

- [x] 3. Implement scene validation and error handling

  - Create IsSceneInBuildSettings method to validate scene names
  - Create ValidateCrossSceneTeleport method with comprehensive checks
  - Add error logging for missing scenes, invalid configurations
  - Ensure graceful degradation when validation fails
  - _Requirements: 1.5, 6.5_

- [x] 4. Implement scene preloading system

- [x] 4.1 Create preload data structures and core methods

  - Add Dictionary<string, AsyncOperation> for preload operations
  - Add Dictionary<string, float> for LRU cache tracking
  - Implement PreloadScene public method
  - Implement IsScenePreloaded public method
  - Implement GetPreloadProgress public method
  - _Requirements: 2.1, 2.6_

- [x] 4.2 Implement async scene loading

  - Create PreloadSceneAsync coroutine using LoadSceneAsync with LoadSceneMode.Additive
  - Set allowSceneActivation = false to prevent premature activation
  - Set operation.priority to ThreadPriority.Low for performance
  - Track load progress and mark scene as fully loaded at 90%
  - _Requirements: 2.1, 2.2_

- [x] 4.3 Implement LRU cache management

  - Create MarkSceneAsUsed method to update sceneLastUsedTime
  - Create EnforceCacheLimit method to check maxPreloadedScenes
  - Create UnloadLeastRecentlyUsedScene method using LINQ OrderBy
  - Integrate cache enforcement into PreloadScene workflow
  - _Requirements: 2.5, 5.1, 5.4, 5.5_

- [x] 4.4 Implement delayed unload on proximity exit

  - Create CancelPreload public method
  - Create UnloadSceneDelayed coroutine with configurable delay
  - Check if player returned before actually unloading
  - Integrate with TeleportPoint proximity exit
  - _Requirements: 2.4, 5.3_

- [x] 5. Implement proximity-based preload triggers

  - Update OnTriggerEnter2D to detect preload zone entry and distinguish from activation trigger
  - Call TeleportManager.Instance.PreloadScene when player enters preload zone
  - Update OnTriggerExit2D to detect preload zone exit
  - Call TeleportManager.Instance.CancelPreload when player leaves preload zone
  - Track preload zone state with isInPreloadZone flag
  - _Requirements: 2.1, 4.1, 4.2, 4.3, 4.4, 4.5_

- [x] 6. Implement cross-scene teleport orchestration

- [x] 6.1 Create ExecuteCrossSceneTeleport method

  - Accept all necessary parameters (scene name, position, transition effect, audio clips, debug flag)
  - Implement teleport lock to prevent concurrent teleports
  - Call ValidateCrossSceneTeleport before proceeding
  - Zero player Rigidbody2D velocity
  - _Requirements: 1.1, 1.2, 6.1, 6.2_

- [x] 6.2 Implement TeleportSequence coroutine

  - Play start sound using PlayTeleportSound
  - Execute fade out using TeleportTransitionHelper.ExecuteTransition
  - In callback: wait for scene load if not preloaded, then call TransferPlayerToScene
  - Play mid sound after player transfer
  - Unload previous scene using UnloadPreviousScene
  - Execute fade in (handled by TeleportTransitionHelper)
  - Play end sound after fade in
  - Clear teleport lock in finally block
  - _Requirements: 1.1, 1.2, 1.3, 3.1, 3.2, 3.3, 6.3_

- [x] 6.3 Implement scene transfer logic

  - Create TransferPlayerToScene method
  - Apply DontDestroyOnLoad to PlayerController.Instance.gameObject
  - Get target scene using SceneManager.GetSceneByName
  - Use SceneManager.MoveGameObjectToScene to move player
  - Set player transform.position to destinationPosition
  - Calculate and maintain camera offset during transfer
  - Update camera position to new location with same offset
  - _Requirements: 1.2, 1.3_

- [x] 6.4 Implement scene unloading

  - Create UnloadPreviousScene method
  - Get current active scene before transfer
  - Unload it using SceneManager.UnloadSceneAsync after transfer
  - Call Resources.UnloadUnusedAssets on completion
  - _Requirements: 5.2_

- [x] 7. Implement audio system
  - Create PlayTeleportSound public method accepting AudioClip
  - Create private PlaySound method with volume calculation
  - Create GetGameVolume method to integrate with game's audio settings
  - Implement graceful degradation (null check before playing)
  - Ensure sounds are non-blocking (use PlayOneShot)
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 3.5, 3.6_

- [x] 8. Update TeleportPoint to route teleports correctly

  - Modify ExecuteTeleport coroutine to check isCrossSceneTeleport flag
  - If false or destinationSceneName empty: use existing same-scene logic
  - If true and destinationSceneName valid: call TeleportManager.Instance.ExecuteCrossSceneTeleport
  - Pass all configuration (scene, position, effect, audio clips, debug flag)
  - Ensure existing same-scene teleports continue working unchanged
  - _Requirements: 1.1, 1.4, 6.1, 6.2, 6.3_

- [x] 9. Enhance Gizmos for cross-scene teleports
  - Update OnDrawGizmos to show preload proximity zone (wire sphere)
  - Add visual distinction for cross-scene teleports (different color or label)
  - Display destination scene name in editor label
  - Show both trigger box and preload zone simultaneously
  - _Requirements: 4.3_

- [x] 10. Add PlayerController movement control methods

  - Add public DisableMovement() method to PlayerController
  - Add public EnableMovement() method to PlayerController
  - Ensure methods properly control player input and movement
  - Update TeleportPoint same-scene teleport to use these methods
  - Update TeleportManager cross-scene teleport to use these methods
  - _Requirements: 1.2, 6.2_
