# Project Structure & Organization

## File Organization Principles

### Code Organization (`Assets/_Code/`)

All game scripts follow namespace `TheSlimeKing.{Category}` and are organized by functional area:

- **Controllers/** - Scene-specific controllers (e.g., `TitleScreenController`, `InitialCaveController`)
- **Dialogue/** - Dialogue system (database, entries, state management)
- **Environments/** - Environment and ambience scripts
- **Gameplay/** - Core gameplay mechanics
  - Combat/, Creatures/, Environment/, Interaction/, Items/, NPCs/, Player/, Props/
- **Items/** - Item system, ScriptableObjects, inventory logic
- **Managers/** - Singleton managers (GameManager, CameraManager, InventoryManager, etc.)
- **Performance/** - Performance optimization systems (LOD, culling, FPS counter)
- **Systems/** - Reusable systems (AI/, Camera/, Core/, Debug/, Events/, Input/, SaveSystem/, Validators/, Visual/)
- **UI/** - UI components and controllers

### Asset Organization

- **_Prefabs/** - All prefabs organized by type (Characters/, NPCs/, Items/, Props/, FX/, UI/)
- **_Scenes/** - Unity scenes (TitleScreen, InitialCave, InitialForest, etc.)
- **Art/** - Visual assets (Animations/, Materials/, Shaders/, Sprites/, Tiles/)
- **Audio/** - Sound assets (Music/, SFX/)
- **Data/** - ScriptableObject data (Crystals/, Items/, NPCs/, QuickWins/, Settings/)
- **Docs/** - Documentation (GDD.md, CodingStandards.md, Worklogs/)
- **Editor/** - Editor tools and extensions
- **External Assets/** - Third-party packages (DO NOT MODIFY)
- **Localization/** - Localization tables for all supported languages
- **Resources/** - Runtime-loaded resources
- **Settings/** - Project settings (InputActions/, Lighting/, PostProcessing/, Scenes/)

## Naming Conventions

### Prefabs
- **PascalCase** without spaces or underscores
- **NO redundant prefixes** (folder structure provides context)
- **Semantic suffixes** for technical function:
  - `Manager` - Singleton managers (GameManager, CameraManager)
  - `Controller` - Gameplay controllers (PlayerController, BossController)
  - `VFX` - Visual effects (ExplosionVFX, HealVFX)
  - `HUD` - UI overlay elements (HealthBarHUD, MiniMapHUD)
  - `Canvas` - Complete UI canvases (MainMenuCanvas, PauseCanvas)
  - `NPC` - Non-player characters (MerchantNPC, VillagerNPC)
  - `Point` - Reference markers (SpawnPoint, TeleportPoint)

Examples:
- ✅ `PlayerSlime.prefab`, `CrystalRed.prefab`, `TreeOak.prefab`
- ❌ `player_slime.prefab`, `item_crystal.prefab`, `art_tree.prefab`

### Scenes
- **PascalCase** without spaces, underscores, or numeric prefixes
- **Descriptive names** indicating purpose or location
- **NO developer prefixes** (use Git branches for personal work)

Examples:
- ✅ `TitleScreen.unity`, `InitialCave.unity`, `AncientTemple.unity`
- ❌ `1_TitleScreen.unity`, `ERICK_InitialForest.unity`, `title_screen.unity`

### Scripts
- **PascalCase** matching class name
- **Semantic suffixes**: `Manager`, `Controller`, `System`, `Service`, `Settings`
- **NO emojis** in filenames (only in UI strings)

## Scene Hierarchy Structure

Every scene must follow this standardized hierarchy:

```
Root Scene Hierarchy:
├── --- SYSTEMS ---         # Separator (empty GameObject, disabled)
├── GameManager
├── CameraManager
├── EventSystem
├── [Other Managers]
├── --- ENVIRONMENT ---
├── Background              # Parallax layers, sky
├── Grid                    # Tilemaps
├── Scenario                # Props, decoration, obstacles
├── --- GAMEPLAY ---
├── Player
├── NPCs
├── Enemies
├── Items
├── --- MECHANICS ---
├── Mechanics               # Puzzles, interactables
├── SpawnPoints
├── Triggers
├── --- EFFECTS ---
├── Lighting
├── ParticleSystems
├── PostProcessing
├── --- UI ---
├── CanvasHUD
└── CanvasDebug
```

### Hierarchy Rules
- Use `--- CATEGORY ---` separators (disabled GameObjects) for visual organization
- **PascalCase** for all GameObject names
- Remove Unity's automatic `(Clone)` suffixes
- Add descriptive suffixes for multiple instances: `_01`, `_Patrol`, `_Guard`
- Group similar objects in sub-folders when count exceeds 20

## Editor Tools Structure

Custom editor tools follow modular architecture:

```
Assets/Editor/[ToolName]/
├── [ToolName]Window.cs      # EditorWindow main UI
├── [ToolName]Settings.cs    # Configuration and EditorPrefs
├── [Feature]Service.cs      # Business logic
└── [Helper]Utility.cs       # Helper functions
```

### Menu Organization
- **Main menu**: `Extra Tools/[Category]/[Feature Name]`
- **Context menu**: `GameObject/Quick Tools/[Category]/[Feature Name]`

Categories: Setup/, Organize/, Scene Tools/, Debug/

## Documentation Standards

### XML Documentation
All public classes and methods require XML documentation:

```csharp
/// <summary>
/// Brief description of purpose
/// 
/// Additional details about usage and functionality
/// 
/// Access: Menu > Extra Tools > [Category]
/// </summary>
```

### Worklogs
Significant implementations require worklogs in `Assets/Docs/Worklogs/`:
- Format: `YYYY-MM-DD-feature-name.md`
- Include: objective, technical decisions, modified files

## Class Structure Template

```csharp
using statements

namespace TheSlimeKing.{Category}
{
    /// <summary>XML documentation</summary>
    public class ClassName : MonoBehaviour
    {
        #region Settings / Configuration
        [Header("Configuration")]
        [SerializeField] private float setting;
        #endregion

        #region References
        private Component reference;
        #endregion

        #region Debug
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showGizmos = false;
        #endregion

        #region Private Variables
        private int privateVar;
        #endregion

        #region Unity Lifecycle
        private void Awake() { }
        private void Start() { }
        private void Update() { }
        #endregion

        #region Initialization
        private void Initialize() { }
        #endregion

        #region Public Methods
        public void PublicMethod() { }
        #endregion

        #region Private Methods
        private void PrivateMethod() { }
        #endregion

        #region Utilities
        private void Log(string message) { }
        #endregion

        #region Gizmos (Optional)
        private void OnDrawGizmos()
        {
            if (!showGizmos) return;
            // Draw debug visualization
        }
        #endregion
    }
}
```

## Gizmos Guidelines

Systems with ranges, areas, or spatial relationships should implement optional Gizmos:

```csharp
[SerializeField] private bool showGizmos = false;

private void OnDrawGizmos()
{
    if (!showGizmos) return;
    
    // Use semantic colors:
    // Green - active/safe, Yellow - medium/alert
    // Red - critical/danger, Blue - info/neutral
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, radius);
}
```

## Anti-Patterns to Avoid

### File Naming
- ❌ Emojis in filenames or folders
- ❌ snake_case or kebab-case for code files
- ❌ Spaces in any filenames
- ❌ Numeric prefixes on scenes

### Code Organization
- ❌ Commented-out code (use Git history)
- ❌ Magic numbers (use named constants)
- ❌ Methods over 50 lines
- ❌ Classes over 500 lines (refactor into services)
- ❌ `GameObject.Find()` in loops
- ❌ Public fields (use `[SerializeField] private` instead)

### Scene Hierarchy
- ❌ Flat hierarchy with 50+ root objects
- ❌ GameObject names like `GameObject (15)` or `prefab (Clone)`
- ❌ camelCase or snake_case GameObject names
- ❌ Redundant prefixes (env_, art_, prop_)

## Version Control

- **DO commit**: Scripts, prefabs, scenes, settings, documentation
- **DO NOT commit**: Library/, Temp/, Logs/, obj/, bin/, .vs/
- **Scene changes**: Commit separately from code changes when possible
- **Renaming**: Use Unity Editor (not file system) and commit separately
