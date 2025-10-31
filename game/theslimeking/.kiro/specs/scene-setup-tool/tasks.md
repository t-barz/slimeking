# Implementation Plan

- [x] 1. Create core SceneSetupTool editor script structure
  - Create `Assets/Code/Editor/SceneSetupTool.cs` file
  - Implement basic class structure with logging methods
  - Add namespace and using statements
  - _Requirements: 1.1, 1.2, 1.3_

- [x] 2. Implement component detection logic
  - [x] 2.1 Implement FindGameObjectByName method
    - Search scene hierarchy for GameObject by name
    - Return null if not found
    - _Requirements: 2.1, 2.2_
  
  - [x] 2.2 Implement FindComponentInScene generic method
    - Use Object.FindFirstObjectByType to find component
    - Handle case when component doesn't exist
    - _Requirements: 2.1, 2.2_
  
  - [x] 2.3 Implement EnsureGameObject method
    - Check if GameObject exists by name
    - Create new GameObject if not found
    - Add specified component type
    - Set position to (0, 0, 0) for new objects
    - _Requirements: 2.2, 2.3_
  
  - [x] 2.4 Implement EnsureComponent generic method
    - Check if component exists on GameObject
    - Add component if missing
    - Return component reference
    - _Requirements: 2.2, 2.3_

- [x] 3. Implement individual component setup methods
  - [x] 3.1 Implement SetupGameManager method
    - Find or create "GameManager" GameObject
    - Ensure GameManager component is attached
    - Log whether created or found existing
    - Return setup result
    - _Requirements: 3.1, 3.2, 3.3, 3.4_
  
  - [x] 3.2 Implement SetupSceneTransitioner method
    - Find or create "SceneTransitioner" GameObject
    - Ensure SceneTransitionManager component is attached
    - Log whether created or found existing
    - Return setup result
    - _Requirements: 4.1, 4.2, 4.3, 4.4_
  
  - [x] 3.3 Implement SetupTeleportManager method
    - Find or create "TeleportManager" GameObject
    - Ensure TeleportManager component is attached
    - Log whether created or found existing
    - Return setup result
    - _Requirements: 5.1, 5.2, 5.3, 5.4_
  
  - [x] 3.4 Implement SetupEventSystem method
    - Find existing EventSystem component in scene
    - Create "EventSystem" GameObject if not found
    - Ensure EventSystem component is attached
    - Ensure InputSystemUIInputModule component is attached
    - Log whether created or found existing
    - Return setup result
    - _Requirements: 6.1, 6.2, 6.3, 6.4_

- [x] 4. Implement main menu item and orchestration

  - [x] 4.1 Create SetupSceneForTransitions menu item method

    - Add MenuItem attribute for "Extra Tools/Scene/🎬 Setup Scene for Transitions"
    - Validate that a scene is open
    - Log start of setup process
    - _Requirements: 1.1, 1.2, 1.3_
  
  - [x] 4.2 Orchestrate component setup sequence

    - Call SetupGameManager with out parameter

    - Call SetupSceneTransitioner with out parameter
    - Call SetupTeleportManager with out parameter
    - Call SetupEventSystem with out parameter
    - Track total components added and existing
    - _Requirements: 2.1, 2.4_
  
  - [x] 4.3 Implement summary and feedback

    - Create ShowSummary method to display results
    - Show EditorUtility.DisplayDialog with summary
    - Log final summary to console
    - Mark scene as dirty using EditorSceneManager.MarkSceneDirty
    - _Requirements: 1.4, 8.1, 8.2, 8.3, 8.4_

- [x] 5. Implement safety and validation
  - [x] 5.1 Add scene validation
    - Check if scene is open before setup
    - Display error if no scene is open
    - _Requirements: 7.1, 7.2, 7.3_
  
  - [x] 5.2 Add component preservation logic
    - Never destroy existing GameObjects
    - Never remove existing components
    - Only add missing components
    - _Requirements: 7.1, 7.2, 7.3, 7.4_
  
  - [x] 5.3 Implement error handling
    - Wrap component creation in try-catch blocks
    - Log errors without stopping entire process
    - Continue with next component if one fails
    - _Requirements: 7.1, 7.2, 7.3, 7.4_
-

- [x] 6. Integrate with UnifiedExtraTools window

  - [x] 6.1 Add Scene tab to UnifiedExtraTools

    - Update tabNames array to include "Scene" between "Camera" and "Project"
    - Add case 2 for Scene tab in switch statement (shift existing cases)
    - _Requirements: 1.1_
  
  - [x] 6.2 Implement DrawSceneSection method

    - Create button for "🎬 Setup Scene for Transitions"
    - Add help box with description "Configure cena com componentes essenciais para transições"
    - Call SceneSetupTool.SetupSceneForTransitions on button click
    - _Requirements: 1.1, 1.2_
  
  - [x] 6.3 Add menu item for Scene setup

    - Add MenuItem for "Extra Tools/Scene/🎬 Setup Scene for Transitions"
    - Call SceneSetupTool.SetupSceneForTransitions
    - _Requirements: 1.1_

- [x] 7. Add documentation and comments
  - [x] 7.1 Add XML documentation comments
    - Document all public methods
    - Document all private helper methods
    - Add summary, param, and returns tags
    - _Requirements: 8.1, 8.2, 8.3, 8.4_
  
  - [x] 7.2 Add inline comments for complex logic
    - Comment detection strategy
    - Comment creation logic
    - Comment error handling
    - _Requirements: 8.1, 8.2, 8.3, 8.4_
