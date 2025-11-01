# NPCDialogueInteraction - Advanced Configuration Verification

## Task 14: Adicionar suporte para configurações avançadas

### Implementation Summary

Successfully added advanced configuration support to NPCDialogueInteraction with the following features:

#### 1. Custom Typewriter Speed

- **Field Added**: `customTypewriterSpeed` (float, default: 0)
- **Tooltip**: "Velocidade customizada do efeito typewriter (caracteres por segundo). Se 0, usa o valor padrão do DialogueSystemSettings. Valores negativos desabilitam o efeito"
- **Behavior**:
  - When set to 0, uses default from DialogueSystemSettings
  - When > 0, applies custom speed to DialogueUI before starting dialogue
  - Applied via `ApplyCustomSettings()` method

#### 2. Custom Pause Player Settings

- **Fields Added**:
  - `useCustomPauseSettings` (bool, default: false)
  - `pausePlayerDuringDialogue` (bool, default: true)
- **Tooltips**:
  - useCustomPauseSettings: "Se marcado, usa configuração customizada para pausar o jogador. Se desmarcado, usa o valor global do DialogueManager"
  - pausePlayerDuringDialogue: "Se o jogador deve ser pausado durante este diálogo específico. Apenas usado se 'useCustomPauseSettings' estiver marcado"
- **Behavior**:
  - When `useCustomPauseSettings` is false, uses global DialogueManager setting
  - When `useCustomPauseSettings` is true, applies custom pause setting to DialogueManager

#### 3. Enhanced Tooltips

All existing fields now have improved tooltips:

- `dialogueId`: "ID único do diálogo a ser carregado do JSON"
- `interactionRadius`: "Raio de detecção para interação com o jogador. Se 0, usa o valor padrão do DialogueSystemSettings"
- `iconAnchor`: "Ponto de ancoragem para o ícone de interação (geralmente acima da cabeça do NPC)"
- `interactionIconPrefab`: "Prefab do ícone de interação. Se não atribuído, usa o prefab padrão do DialogueSystemSettings"
- `interactionButton`: "Nome do botão de interação configurado no Input Manager. Se vazio, usa o valor padrão do DialogueSystemSettings"
- `iconOffset`: "Offset do ícone em relação ao anchor (world space). Permite ajustar a posição vertical do ícone"
- `gizmoColor`: "Cor do gizmo de visualização no editor para o raio de interação"

#### 4. Public API Methods Added

**Getters:**

- `float GetCustomTypewriterSpeed()` - Returns custom typewriter speed (0 if using default)
- `bool UsesCustomPauseSettings()` - Returns true if using custom pause settings
- `bool GetPausePlayerDuringDialogue()` - Returns custom pause setting value

**Setters:**

- `void SetCustomTypewriterSpeed(float speed)` - Sets custom typewriter speed at runtime
- `void SetCustomPauseSettings(bool useCustom, bool shouldPause)` - Sets custom pause configuration at runtime

#### 5. Private Methods Added

- `void ApplyCustomSettings()` - Applies custom settings to DialogueManager and DialogueUI before starting dialogue
  - Applies custom pause settings if `useCustomPauseSettings` is true
  - Applies custom typewriter speed if `customTypewriterSpeed` > 0
  - Logs configuration changes for debugging

### DialogueUI Updates

Added methods to support custom typewriter speed:

- `void SetTypewriterSpeed(float speed)` - Sets typewriter speed
- `float GetTypewriterSpeed()` - Gets current typewriter speed

### Integration Flow

1. When `TryStartDialogue()` is called:
   - `ApplyCustomSettings()` is invoked first
   - Custom pause setting is applied to DialogueManager (if enabled)
   - Custom typewriter speed is applied to DialogueUI (if > 0)
   - Dialogue is started via DialogueManager

2. Settings Priority:
   - Custom settings (if defined) > DialogueSystemSettings defaults
   - Allows per-NPC customization while maintaining global defaults

### Requirements Satisfied

✅ **Requirement 2.4**: Proximity detection and interaction icon with configurable parameters
✅ **Requirement 4.5**: Typewriter effect with configurable speed
✅ **Requirement 7.1**: Configuration of dialogue ID, interaction distance, typewriter speed
✅ **Requirement 7.2**: Global system configuration with defaults

### Testing Recommendations

1. **Custom Typewriter Speed**:
   - Set `customTypewriterSpeed` to 100 on one NPC
   - Set `customTypewriterSpeed` to 20 on another NPC
   - Leave `customTypewriterSpeed` at 0 on a third NPC
   - Verify each NPC uses the correct speed

2. **Custom Pause Settings**:
   - Enable `useCustomPauseSettings` and set `pausePlayerDuringDialogue` to false on one NPC
   - Leave `useCustomPauseSettings` disabled on another NPC
   - Verify player can move during first NPC's dialogue but not the second

3. **Tooltips**:
   - Hover over each field in the Inspector
   - Verify tooltips are clear and helpful

### Files Modified

1. `Assets/Code/Gameplay/NPCs/NPCDialogueInteraction.cs`
   - Added 3 new inspector fields with tooltips
   - Enhanced tooltips for all existing fields
   - Added 5 new public API methods
   - Added 1 new private method
   - Updated `TryStartDialogue()` to apply custom settings

2. `Assets/Code/Systems/UI/DialogueUI.cs`
   - Added `SetTypewriterSpeed()` method
   - Added `GetTypewriterSpeed()` method

### Notes

- Custom settings are applied per-dialogue, allowing fine-grained control
- Default values from DialogueSystemSettings are used when custom settings are not defined
- All custom settings can be modified at runtime via public API methods
- Debug logs help track which settings are being applied
