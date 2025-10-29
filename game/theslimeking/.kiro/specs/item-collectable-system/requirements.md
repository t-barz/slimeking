# Requirements Document

## Introduction

This spec addresses a compilation error in the BounceHandler class where it cannot find the ItemCollectable type. The ItemCollectable class already exists in the `SlimeMec.Gameplay` namespace, but BounceHandler is missing the proper `using` directive to access it.

The issue manifests as two compilation errors:

- Line 581: `error CS0246: The type or namespace name 'ItemCollectable' could not be found`
- Line 930: `error CS0246: The type or namespace name 'ItemCollectable' could not be found`

The fix is straightforward: add the missing `using SlimeMec.Gameplay;` directive to BounceHandler.cs so it can properly reference the ItemCollectable class.

## Requirements

### Requirement 1: Fix Missing Using Directive

**User Story:** As a developer, I want the BounceHandler class to compile without errors, so that the game can build and run successfully.

#### Acceptance Criteria

1. WHEN BounceHandler.cs is compiled THEN it SHALL not produce CS0246 errors for ItemCollectable
2. WHEN the using directive is added THEN it SHALL be `using SlimeMec.Gameplay;`
3. WHEN the fix is applied THEN both line 581 and line 930 SHALL compile successfully
4. WHEN BounceHandler accesses ItemCollectable via GetComponent THEN it SHALL find the component correctly
5. WHEN the project is built THEN there SHALL be no compilation errors related to ItemCollectable
