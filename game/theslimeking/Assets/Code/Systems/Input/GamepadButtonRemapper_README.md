# 🎮 Gamepad Button Remapper

## Visão Geral

Sistema de remapeamento dinâmico de botões de gamepad para suportar diferentes layouts (Xbox, PlayStation, Nintendo Switch, gamepads genéricos com botões invertidos).

**Problema que resolve:** Gamepads com botões em posições diferentes (ex: Switch ou gamepads genéricos) disparam ações conflitantes quando os botões são invertidos.

## Layouts Suportados

### Presets Pré-configurados

1. **Xbox (Padrão)**
   - A (South), B (East), X (West), Y (North)
   - Sem remapeamento necessário

2. **PlayStation**
   - ✕ (South), ○ (East), □ (West), △ (North)
   - Sem remapeamento necessário (mesmo layout)

3. **Nintendo Switch**
   - B (East), A (South), Y (West), X (North)
   - Remapeia East↔South e North↔West

4. **Genérico Invertido**
   - Para gamepads que têm buttons em layout não-padrão
   - Remapeia East↔South

## Como Usar

### 1. Criar o System

**Opção A - Automático (Recomendado):**

```
Extra Tools → Setup → Create Gamepad Button Remapper
```

**Opção B - Manual:**

- Crie um GameObject vazio
- Adicione o componente `GamepadButtonRemapper`

### 2. Selecionar o Layout

No Inspector do `GamepadButtonRemapper`:

```
Selected Preset Index: 0 (Xbox) / 1 (PlayStation) / 2 (Switch) / 3 (Genérico Invertido)
```

### 3. Customização (Opcional)

Para layouts customizados, use código:

```csharp
if (GamepadButtonRemapper.Instance != null)
{
    // Switch para Nintendo Switch
    GamepadButtonRemapper.Instance.ApplyLayoutPreset(2);
    
    // Ou criar layout customizado
    GamepadButtonRemapper.Instance.ApplyCustomLayout(
        swapEastSouth: true,
        swapNorthWest: false
    );
}
```

## Outputs do Console

Quando ativado, você verá:

```
[GamepadButtonRemapper] Layout 'Nintendo Switch' aplicado
  Swap East/South: True
  Swap North/West: True
```

## Mapeamento de Botões

### Antes (Sem Remapeamento)

**Xbox Padrão:**

```
A (South)  → Interact
B (East)   → Attack
X (West)   → Crouch
Y (North)  → Open Inventory
```

**Nintendo Switch (COM PROBLEMAS):**

```
A (East)   → Interact (ERRADO, deveria ser Attack)
B (South)  → Attack (ERRADO, deveria ser Interact)
X (West)   → Crouch
Y (North)  → Open Inventory
```

### Depois (COM Remapeamento)

**Nintendo Switch (CORRIGIDO):**

```
A (East)   → Attack ✓
B (South)  → Interact ✓
X (West)   → Crouch ✓
Y (North)  → Open Inventory ✓
```

## Detecção Automática

Para detecção automática de gamepad no futuro:

```csharp
private void DetectGamepadLayout()
{
    var gamepad = Gamepad.current;
    if (gamepad == null) return;
    
    string layout = gamepad.layout;
    
    if (layout.Contains("Switch"))
    {
        GamepadButtonRemapper.Instance.ApplyLayoutPreset(2);
    }
    else if (layout.Contains("DualShock") || layout.Contains("DualSense"))
    {
        GamepadButtonRemapper.Instance.ApplyLayoutPreset(1);
    }
    else if (layout.Contains("XInput"))
    {
        GamepadButtonRemapper.Instance.ApplyLayoutPreset(0);
    }
}
```

## API Pública

```csharp
// Aplicar um preset
GamepadButtonRemapper.Instance.ApplyLayoutPreset(int presetIndex);

// Aplicar layout customizado
GamepadButtonRemapper.Instance.ApplyCustomLayout(
    bool swapEastSouth,
    bool swapNorthWest
);

// Obter informações do layout atual
GamepadLayoutPreset current = GamepadButtonRemapper.Instance.GetCurrentLayout();
string name = GamepadButtonRemapper.Instance.GetCurrentLayoutName();

// Verificar se um botão está mapeado
bool isSwapped = GamepadButtonRemapper.Instance.IsButtonSwapped("buttonEast");
```

## Localização

- **Script:** `Assets/Code/Systems/Input/GamepadButtonRemapper.cs`
- **Editor Tool:** `Assets/Code/Editor/ExtraTools/Setup/GamepadRemapperCreator.cs`
- **Documentação:** Este arquivo

## Notas

- Sistema funciona como `DontDestroyOnLoad` (persiste entre cenas)
- Singleton com instância única garantida
- Sem impacto de performance (apenas configurações)
- Compatível com novo Input System do Unity

## Troubleshooting

**Problema:** Remapeamento não está funcionando
**Solução:** Verifique se `GamepadButtonRemapper.Instance` foi criado antes dos inputs serem processados

**Problema:** Não vejo o menu "Create Gamepad Button Remapper"
**Solução:** Certifique-se que o arquivo `GamepadRemapperCreator.cs` está em `Assets/Code/Editor/`

**Problema:** Qual preset devo usar?
**Solução:** Use o inspector para testar cada um até encontrar o que funciona com seu gamepad
