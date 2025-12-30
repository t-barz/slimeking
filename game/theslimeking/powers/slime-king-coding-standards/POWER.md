---
name: "slime-king-coding-standards"
displayName: "Slime King Coding Standards"
description: "Enforces KISS principles and comprehensive coding standards for The Slime King Unity project, including file organization, naming conventions, Unity-specific best practices, and architectural guidelines."
keywords: ["kiss", "coding-standards", "unity", "slime-king", "best-practices", "architecture", "editor-tools"]
author: "The Slime King Development Team"
---

# Slime King Coding Standards

## Overview

This power provides comprehensive coding standards and KISS (Keep It Simple, Stupid) principle enforcement for The Slime King Unity project. It covers everything from file organization and naming conventions to Unity-specific best practices and architectural guidelines.

The standards ensure code maintainability, consistency, and simplicity across the entire project, making it easier for developers to understand, modify, and extend the codebase.

## Core KISS Principles

### 1. Simplicity Over Complexity
- **Prefer simple solutions** over complex ones, even if they seem less "elegant"
- **One responsibility per class** - if a class does too many things, split it
- **Avoid premature optimization** - write clear code first, optimize later if needed
- **Use descriptive names** instead of clever abbreviations

### 2. Readability First
- **Code should read like documentation** - anyone should understand what it does
- **Avoid deep nesting** - prefer early returns and guard clauses
- **Keep methods short** - maximum 50 lines per method
- **Use meaningful variable names** - `playerHealth` instead of `pH`

### 3. Consistency Over Cleverness
- **Follow established patterns** in the codebase
- **Use standard Unity patterns** instead of inventing new ones
- **Maintain consistent naming** throughout the project
- **Stick to the agreed architecture** - don't create exceptions

## File Organization Standards

### Directory Structure

```text
Assets/
├── _Code/                     # Game scripts (namespace organized)
│   ├── Managers/              # Singletons and global controllers
│   ├── UI/                    # UI components with Input System
│   ├── Items/                 # ScriptableObjects and enums
│   └── Environments/          # Scene-specific scripts
├── _Prefabs/                  # GameObject prefabs
├── _Scenes/                   # Scene files
├── Docs/                      # Documentation and worklogs
├── Editor/                    # Editor tools and windows
├── External Assets/           # Third-party assets (DO NOT MODIFY)
├── Resources/                 # Runtime-loaded resources
├── Settings/                  # ScriptableObject configurations
└── Tests/                     # Temporary test files (delete after use)
```

### File Naming Rules

**✅ DO:**
- Use **PascalCase** for code files: `GameObjectBrushTool.cs`
- Use **kebab-case** or **snake_case** for documentation: `coding-standards.md`
- Use only alphanumeric characters, hyphens, and underscores
- Match class name with file name exactly

**❌ DON'T:**
- Never use emojis in file or folder names
- Avoid spaces in file names
- Don't use special characters (@, #, $, etc.)

## Class Structure Standards

### Mandatory Class Organization

```csharp
// 1. Using statements
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// 2. Namespace
namespace SlimeKing.Editor.Tools
{
    // 3. XML Documentation
    /// <summary>
    /// Brief description of class purpose
    /// 
    /// Additional details about usage, functionality, etc.
    /// 
    /// Access: Menu > Extra Tools > [Category]
    /// </summary>
    // 4. Class declaration
    public class GameObjectBrushTool : EditorWindow
    {
        // 5. #region Fields
        #region Fields
        [SerializeField] private float brushRadius = 5f;
        private bool isActive = false;
        private const string VERSION = "1.0";
        #endregion

        // 6. #region Unity Lifecycle
        #region Unity Lifecycle
        private void Awake() { }
        private void Start() { }
        private void OnEnable() { }
        private void OnDisable() { }
        private void Update() { }
        #endregion

        // 7. #region Public Methods
        #region Public Methods
        public void Initialize() { }
        #endregion

        // 8. #region Private Methods
        #region Private Methods
        private void ProcessInput() { }
        #endregion

        // 9. #region Utility Methods
        #region Utility Methods
        private bool IsValidTarget(GameObject target) { return target != null; }
        #endregion
    }
}
```

## Naming Conventions

### Classes and Methods
- **PascalCase** for classes, methods, properties: `GameObjectBrushTool`, `ProcessInput()`
- **camelCase** for private fields: `brushRadius`, `isActive`
- **UPPER_CASE** for constants: `VERSION`, `MAX_ITEMS`

### File Suffixes
- Editor Windows: `*Window.cs` or `*Tool.cs`
- Services: `*Service.cs`
- Settings: `*Settings.cs`
- Managers: `*Manager.cs`
- Controllers: `*Controller.cs`

### Scene Controllers
Every scene must have a dedicated controller:
- **Pattern**: `[SceneName]Controller.cs`
- **Location**: `Assets/_Code/Scripts/Controllers/`
- **Purpose**: Scene initialization, state management, system coordination

```csharp
/// <summary>
/// Controller for the MainMenu scene.
/// Manages initialization and scene-specific behavior.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // Scene-specific initialization and logic
}
```

## Unity Editor Menu Structure

### Required Menu Hierarchy

```text
Extra Tools/
├── Setup/
│   └── Create Folders
├── Organize/
│   └── Organize Prefabs
├── Scene Tools/
│   └── GameObject Brush Tool
└── Debug/
    └── Export Scene Structure

Quick Tools/ (Context Menu)
└── Debug/
    └── Export Object Structure
```

### MenuItem Implementation

```csharp
// Main menu
[MenuItem("Extra Tools/Scene Tools/GameObject Brush Tool")]
public static void ShowWindow()
{
    GetWindow<GameObjectBrushTool>("🖌️ GameObject Brush Tool");
}

// Context menu
[MenuItem("GameObject/Quick Tools/Debug/Export Object Structure")]
public static void ExportObjectStructure()
{
    // Implementation
}

// Context menu validation
[MenuItem("GameObject/Quick Tools/Debug/Export Object Structure", true)]
public static bool ValidateExportObjectStructure()
{
    return Selection.activeGameObject != null;
}
```

## Editor Tools Architecture

### Modular Structure for Complex Tools

```text
Assets/Editor/[ToolName]/
├── [ToolName]Window.cs      # Main EditorWindow
├── [ToolName]Settings.cs    # Configuration and EditorPrefs
├── [Feature]Service.cs      # Business logic
└── [Helper]Utility.cs       # Helper functions
```

### Example Implementation

```csharp
// GameObjectBrushToolWindow.cs - Main window
public class GameObjectBrushToolWindow : EditorWindow
{
    private GameObjectBrushToolSettings settings;
    private PrefabPlacementService placementService;
    
    private void OnEnable()
    {
        settings = GameObjectBrushToolSettings.Instance;
        placementService = new PrefabPlacementService();
    }
}

// GameObjectBrushToolSettings.cs - Settings management
public class GameObjectBrushToolSettings : ScriptableObject
{
    public static GameObjectBrushToolSettings Instance { get; private set; }
    
    [SerializeField] private float brushRadius = 5f;
    
    public float BrushRadius
    {
        get => brushRadius;
        set => brushRadius = Mathf.Clamp(value, 0.1f, 50f);
    }
}
```

## Performance Best Practices

### Unity Editor Performance

**✅ DO:**
- Cache references in `OnEnable()`
- Use `sqrMagnitude` instead of `Distance()` when possible
- Use batch operations with Undo system
- Use `Dictionary` for frequent lookups
- Prefer `EditorPrefs` for editor settings

**❌ DON'T:**
- Use `Find()` or `FindObjectsOfType()` in loops
- Use `Resources.Load()` in editor code
- Perform expensive operations in `OnGUI()`

### Code Examples

```csharp
// ✅ Good - Cached references
private void OnEnable()
{
    cachedTransform = transform;
    cachedRenderer = GetComponent<Renderer>();
}

// ✅ Good - Square magnitude for distance checks
if ((transform.position - target.position).sqrMagnitude < radiusSquared)
{
    // Process nearby object
}

// ✅ Good - Batch undo operations
Undo.SetCurrentGroupName("Batch Operation");
foreach (var obj in objects)
{
    Undo.DestroyObjectImmediate(obj);
}
```

## Unity-Specific Standards

### Serialized Fields

```csharp
// ✅ Preferred - Explicit serialization
[SerializeField] private float speed = 5f;
[SerializeField] private GameObject targetPrefab;

// ❌ Avoid - Unnecessary public exposure
public float speed = 5f;
public GameObject targetPrefab;
```

### Undo/Redo System

```csharp
// Always register destructive operations
Undo.RecordObject(target, "Modify Target");
target.transform.position = newPosition;

// For object destruction
Undo.DestroyObjectImmediate(gameObject);

// For object creation
GameObject instance = Instantiate(prefab);
Undo.RegisterCreatedObjectUndo(instance, "Create Object");

// For multiple operations
Undo.SetCurrentGroupName("Complex Operation");
// ... perform multiple operations
```

### Asset Management

```csharp
// Always refresh after asset modifications
AssetDatabase.Refresh();

// Use relative paths
string relativePath = "Assets/Docs/Temp/file.json";
File.WriteAllText(relativePath, jsonData);
AssetDatabase.ImportAsset(relativePath);
```

## Documentation Standards

### XML Documentation (Required)

```csharp
/// <summary>
/// Brief description of what the class does
/// 
/// Additional details about usage, functionality, etc.
/// 
/// Access: Menu > Extra Tools > [Category]
/// </summary>
public class ExampleTool : EditorWindow
{
    /// <summary>
    /// Processes the input and updates the brush settings
    /// </summary>
    /// <param name="inputEvent">The input event to process</param>
    /// <returns>True if input was handled, false otherwise</returns>
    private bool ProcessInput(Event inputEvent)
    {
        // Implementation
    }
}
```

### Implementation Logs

- Every significant implementation must generate a worklog in `Assets/Docs/Worklogs/`
- Format: `YYYY-MM-DD-feature-name.md`
- Include: objective, technical decisions, modified files

## UI Guidelines for Editor Tools

### Visual Feedback

```csharp
// Semantic colors for different states
GUI.backgroundColor = Color.green;      // Active/Success
GUI.backgroundColor = Color.red;        // Danger/Eraser mode
GUI.backgroundColor = new Color(1f, 0.6f, 0.2f); // Warning/Selective
GUI.backgroundColor = Color.white;      // Reset to default
```

### Emojis in UI (Allowed)

```csharp
// ✅ Allowed in UI strings only
"🖌️ GameObject Brush Tool"  // Window titles
"📦 Prefab Slots"            // Section headers
"⚙️ Settings"                // Configuration sections
"🎲 Randomization"           // Special features
"🔧 Debug"                   // Debug tools

// ❌ Never in file or folder names
// Wrong: "GameObject Brush Tool 🖌️.cs"
// Right: "GameObjectBrushTool.cs"
```

### Help Messages

```csharp
EditorGUILayout.HelpBox("Informational message", MessageType.Info);
EditorGUILayout.HelpBox("Warning message", MessageType.Warning);
EditorGUILayout.HelpBox("Error message", MessageType.Error);
```

## Security and Validation

### Always Validate Input

```csharp
// Null checks
if (obj == null) 
{
    Debug.LogWarning("Object is null, skipping operation");
    return;
}

// Bounds checking
if (index < 0 || index >= list.Count) 
{
    Debug.LogError($"Index {index} out of bounds for list of size {list.Count}");
    return;
}

// Safe properties with validation
private int SafeSelectedIndex
{
    get => Mathf.Clamp(selectedIndex, 0, Mathf.Max(0, list.Count - 1));
    set => selectedIndex = Mathf.Clamp(value, 0, Mathf.Max(0, list.Count - 1));
}
```

### Confirmation Dialogs

```csharp
// Confirm destructive actions
bool confirmed = EditorUtility.DisplayDialog(
    "Confirm Action",
    "This operation cannot be undone. Continue?",
    "Yes",
    "Cancel"
);

if (confirmed)
{
    // Perform destructive operation
}
```

## Debugging Standards

### Optional Debug Logging

```csharp
public class ExampleTool : EditorWindow
{
    [SerializeField] private bool enableDebugLogs = false;

    private void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[{GetType().Name}] {message}");
        }
    }

    private void ProcessAction()
    {
        DebugLog("Starting action processing");
        // Implementation
        DebugLog("Action processing completed");
    }
}
```

## What to Avoid

### Code Quality Issues
- ❌ Commented-out code (use Git for history)
- ❌ Magic numbers (use named constants)
- ❌ Methods longer than 50 lines
- ❌ Classes longer than 500 lines (refactor into services)
- ❌ Deep nesting (prefer early returns)

### Performance Issues
- ❌ `GameObject.Find()` or `FindObjectsOfType()` in loops
- ❌ I/O operations without exception handling
- ❌ Expensive operations in Update() or OnGUI()

### Naming Issues
- ❌ Emojis in file or folder names
- ❌ Author and creation date in XML documentation
- ❌ Abbreviations that aren't universally understood

## KISS Principle Checklist

Before committing code, ask yourself:

### Simplicity Check
- [ ] Can this be implemented more simply?
- [ ] Does this class have a single, clear responsibility?
- [ ] Are the method names self-explanatory?
- [ ] Would a new developer understand this code?

### Complexity Check
- [ ] Is this the simplest solution that works?
- [ ] Have I avoided premature optimization?
- [ ] Are there any unnecessary abstractions?
- [ ] Can any complex logic be broken down further?

### Consistency Check
- [ ] Does this follow the established patterns?
- [ ] Are naming conventions consistent?
- [ ] Does this fit the existing architecture?
- [ ] Have I followed the file organization standards?

## Quick Reference

### File Extensions and Purposes
- `.cs` - C# scripts (PascalCase names)
- `.md` - Documentation (kebab-case names)
- `.asset` - ScriptableObject assets
- `.prefab` - GameObject prefabs
- `.unity` - Scene files

### Common Patterns
- **Singleton Manager**: For global game state
- **ScriptableObject Settings**: For configuration data
- **Controller per Scene**: For scene-specific logic
- **Service Classes**: For reusable business logic
- **Utility Classes**: For static helper methods

### Essential Unity APIs
- `EditorPrefs` - Editor settings persistence
- `AssetDatabase` - Asset management operations
- `Undo` - Undo/Redo system integration
- `EditorUtility` - Editor dialogs and progress bars
- `Selection` - Current editor selection

---

**Remember**: The goal is to write code that is simple, readable, and maintainable. When in doubt, choose the simpler solution that follows these established patterns and conventions.