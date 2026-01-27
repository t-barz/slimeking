# Technical Stack & Build System

## Unity Version

**Unity 6000.4.0b5** (Beta)

## Core Technologies

### Rendering
- **Universal Render Pipeline (URP)** 17.4.0
- 2D Renderer with post-processing support
- 2D Lighting system
- Pixel Perfect Camera support

### Key Unity Packages
- **2D Animation** 14.0.2 - Character and sprite animation
- **2D Tilemap** 1.0.0 + Extras 7.0.0 - Level design
- **2D Sprite Shape** 14.0.0 - Organic terrain shapes
- **Aseprite Importer** 4.0.0 - Sprite workflow
- **Input System** 1.17.0 - Modern input handling (keyboard/gamepad only)
- **Cinemachine** 3.1.5 - Camera control
- **Localization** 1.5.9 - Multi-language support (EN, ES, FR, IT, JA, KO, NL, PT-BR, RU, ZH)
- **Timeline** 1.8.10 - Cutscenes and sequences
- **Behavior** 1.0.14 - AI behavior trees
- **Test Framework** 1.6.0 - Unit testing

### External Assets
- **Fast Script Reload** - Hot reload during development
- **Hierarchy Decorator** - Enhanced hierarchy view
- **Modern Water 2D** - Water effects
- **Easy Transition** - Scene transitions

### Development Tools
- **AI Game Developer MCP** 0.41.1 - AI-assisted development
- Visual Studio 2022 integration

## Project Structure

```
Assets/
├── _Code/              # All C# scripts (namespace: TheSlimeKing.*)
│   ├── Controllers/    # Scene-specific controllers
│   ├── Dialogue/       # Dialogue system
│   ├── Environments/   # Environment/ambience scripts
│   ├── Gameplay/       # Core gameplay mechanics
│   ├── Items/          # Item system and ScriptableObjects
│   ├── Managers/       # Singleton managers (Game, Camera, Inventory, etc.)
│   ├── Performance/    # Performance optimization systems
│   ├── Systems/        # Reusable systems (AI, Input, SaveSystem, etc.)
│   └── UI/             # UI components and controllers
├── _Prefabs/           # Game prefabs (organized by type)
├── _Scenes/            # Unity scenes
├── Art/                # Sprites, animations, materials, shaders, tiles
├── Audio/              # Music and SFX
├── Data/               # ScriptableObject data (Items, NPCs, Crystals, Settings)
├── Docs/               # Documentation and worklogs
├── Editor/             # Editor tools and extensions
├── External Assets/    # Third-party packages (DO NOT MODIFY)
├── Localization/       # Localization tables and assets
├── Resources/          # Runtime-loaded resources
└── Settings/           # Project settings and configurations
```

## Common Commands

### Unity Editor
- **Play Mode**: F5 or Ctrl+P
- **Pause**: Ctrl+Shift+P
- **Step Frame**: Ctrl+Alt+P

### Testing
```bash
# Run all tests
Unity.exe -runTests -batchmode -projectPath . -testResults results.xml

# Run specific test assembly
Unity.exe -runTests -batchmode -projectPath . -testPlatform EditMode
```

### Build
```bash
# Build for Windows
Unity.exe -quit -batchmode -projectPath . -buildWindows64Player "Builds/TheSlimeKing.exe"
```

### Asset Management
- **Refresh Assets**: Ctrl+R
- **Reimport All**: Right-click Assets folder → Reimport All

## Coding Conventions

### Namespace Structure
All scripts use namespace: `TheSlimeKing.{Category}`
- `TheSlimeKing.UI`
- `TheSlimeKing.Gameplay`
- `TheSlimeKing.Managers`
- `TheSlimeKing.Systems`

### Naming Conventions
- **Classes**: PascalCase with semantic suffixes (`Manager`, `Controller`, `System`)
- **Methods**: PascalCase, descriptive action names
- **Fields**: camelCase for private, PascalCase for public properties
- **Serialized Fields**: Always private with `[SerializeField]`
- **Constants**: UPPER_SNAKE_CASE

### Class Structure (Regions)
1. Settings / Configuration
2. References
3. Debug
4. Private Variables
5. Unity Lifecycle (Awake, Start, Update, etc.)
6. Initialization
7. Public Methods
8. Private Methods
9. Utilities

## Performance Considerations

- Use object pooling for frequently instantiated objects
- Implement LOD systems for distant objects
- Cache component references in Awake/OnEnable
- Use `sqrMagnitude` instead of `Distance()` when possible
- Avoid `Find()` and `FindObjectsOfType()` in loops
- Use Dictionary for frequent lookups

## Input System

- **NO mouse or touch support** - keyboard and gamepad only
- Input actions defined in `Assets/Settings/InputSystem_Actions.inputactions`
- Generated C# class: `InputSystem_Actions.cs`

## Localization

Supported languages: English (default), Spanish, French, Italian, Japanese, Korean, Dutch, Portuguese (Brazil), Russian, Chinese (Simplified)

Localization tables:
- `Items` - Item names and descriptions
- `NPCs` - NPC dialogue
- `NPCsNames` - NPC names
- `UI` - UI text and labels
