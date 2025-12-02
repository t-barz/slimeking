# 🎮 Sistema de Pause - Guia de Configuração

## ✅ Código Implementado

Os seguintes arquivos foram criados/atualizados:

1. **PauseEvents.cs** - Eventos estáticos para comunicação
2. **PauseManager.cs** - Manager singleton com pause stack e audio ducking
3. **PlayerController.cs** - Adicionado método GetInputActions() e DisableGameplayInput/EnableGameplayInput
4. **PauseMenu.cs** - UI com fade animations e integração com PauseManager

## 🔧 Configuração no Unity Editor

### Passo 1: Criar Action Map "UI" (InputSystem_Actions.inputactions)

**IMPORTANTE**: Atualmente o sistema está usando o Menu action do Gameplay map. Você deve criar um action map separado "UI".

1. Abra `Assets/Settings/InputSystem_Actions.inputactions` no Unity
2. Clique em "+" para adicionar novo Action Map
3. Nomeie como "UI"
4. Adicione as seguintes actions:

#### Action: Menu (Button)

- **Keyboard**: Escape, Tab
- **Gamepad**: Start

#### Action: Navigate (Value, Vector2)

- **Keyboard**: Arrow Keys, WASD
- **Gamepad**: D-Pad, Left Stick

#### Action: Submit (Button)

- **Keyboard**: Enter, Space
- **Gamepad**: Button South (A/X)

#### Action: Cancel (Button)

- **Keyboard**: Escape
- **Gamepad**: Button East (B/Circle)

5. **Mova o Menu action do Gameplay para UI**
6. Clique em "Generate C# Class" no Inspector
7. Aguarde recompilação

### Passo 2: Atualizar PauseManager.cs

Após criar o UI action map, atualize estas linhas em `PauseManager.cs`:

```csharp
// Linha ~132 - SubscribeToMenuInput()
// ANTES (temporário):
inputActions.Gameplay.Menu.performed += OnMenuInput;

// DEPOIS:
inputActions.UI.Enable();
inputActions.UI.Menu.performed += OnMenuInput;

// Linha ~147 - UnsubscribeFromMenuInput()
// ANTES (temporário):
inputActions.Gameplay.Menu.performed -= OnMenuInput;

// DEPOIS:
inputActions.UI.Menu.performed -= OnMenuInput;
```

### Passo 3: Criar Hierarquia de UI

Na cena `3_InitialForest`:

#### 3.1 Criar PauseCanvas

```
Hierarchy → Right Click → UI → Canvas
Rename: "PauseCanvas"
```

**Canvas Component:**

- Render Mode: Screen Space - Overlay
- Sorting Order: 9999

**Canvas Scaler:**

- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1920 x 1080
- Match Width Or Height: 0.5

**Graphic Raycaster:**

- (Adicionado automaticamente)

**CanvasGroup Component:**

- Alpha: 0
- Interactable: ✓
- Block Raycasts: ✓

#### 3.2 Criar Panel de Fundo

```
PauseCanvas → Right Click → UI → Panel
Rename: "PauseMenuPanel"
```

**RectTransform:**

- Anchor Presets: Stretch (Alt+Shift+Click)
- Left: 0, Right: 0, Top: 0, Bottom: 0

**Image:**

- Color: RGBA(0, 0, 0, 200) - semi-transparente

#### 3.3 Criar Container de Botões

```
PauseMenuPanel → Right Click → UI → Empty
Rename: "ButtonContainer"
Add Component: Vertical Layout Group
```

**RectTransform:**

- Anchor: Center Middle
- Width: 400
- Height: 300

**Vertical Layout Group:**

- Spacing: 20
- Child Alignment: Middle Center
- Child Force Expand Width: ✓
- Child Force Expand Height: ✓

#### 3.4 Criar Botões

Dentro de `ButtonContainer`, crie 4 botões:

##### Botão 1: Inventário

```
Right Click → UI → Button - TextMeshPro
Rename: "InventoryButton"
Text: "Inventário"
```

**Layout Element** (Add Component):

- Preferred Height: 60

**Navigation:**

- Mode: Explicit
- Select On Down: SaveButton

##### Botão 2: Salvar

```
Right Click → UI → Button - TextMeshPro
Rename: "SaveButton"
Text: "Salvar"
```

**Button:**

- Interactable: ✗ (disabled)

**Layout Element:**

- Preferred Height: 60

**Navigation:**

- Mode: Explicit
- Select On Up: InventoryButton
- Select On Down: LoadButton

**Tooltip** (opcional): "Em breve"

##### Botão 3: Carregar

```
Right Click → UI → Button - TextMeshPro
Rename: "LoadButton"
Text: "Carregar"
```

**Button:**

- Interactable: ✗ (disabled)

**Layout Element:**

- Preferred Height: 60

**Navigation:**

- Mode: Explicit
- Select On Up: SaveButton
- Select On Down: ResumeButton

**Tooltip** (opcional): "Em breve"

##### Botão 4: Continuar

```
Right Click → UI → Button - TextMeshPro
Rename: "ResumeButton"
Text: "Continuar"
```

**Layout Element:**

- Preferred Height: 60

**Navigation:**

- Mode: Explicit
- Select On Up: LoadButton
- Select On Down: QuitButton

##### Botão 5: Sair

```
Right Click → UI → Button - TextMeshPro
Rename: "QuitButton"
Text: "Sair"
```

**Layout Element:**

- Preferred Height: 60

**Navigation:**

- Mode: Explicit
- Select On Up: ResumeButton
- Select On Down: InventoryButton (wrap)

#### 3.5 Criar Indicador de Seleção (Seta)

```
ButtonContainer → Right Click → UI → Image
Rename: "SelectionArrow"
```

**RectTransform:**

- Anchor: Left Middle
- Width: 32
- Height: 32
- Pos X: -50 (ajustar conforme necessário)

**Image:**

- Source Image: Sprite de seta (→ ou ►)
- Color: Branco ou cor de destaque
- Raycast Target: ✗ (desabilitar)

**Nota:** Se não tiver sprite de seta, pode usar TextMeshProUGUI com texto "►" ou "→"

### Passo 4: Configurar PauseMenu Component

Selecione `PauseCanvas` e:

1. Add Component → Pause Menu

2. Configure as referências:
   - **Pause Menu Panel**: Arraste `PauseMenuPanel`
   - **Canvas Group**: Arraste `PauseCanvas` (ou será auto-detectado)
   - **Inventory Button**: Arraste `InventoryButton`
   - **Save Button**: Arraste `SaveButton`
   - **Load Button**: Arraste `LoadButton`
   - **Resume Button**: Arraste `ResumeButton`
   - **Quit Button**: Arraste `QuitButton`
   - **Selection Arrow**: Arraste `SelectionArrow`
   - **Arrow Offset X**: -50 (ajustar distância da seta ao botão)

3. Configure settings:
   - **Fade Duration**: 0.3
   - **Enable Logs**: ✓ (para debug)

### Passo 5: Configurar Managers na Cena

Certifique-se de que existem na cena:

#### GameManager

```
Hierarchy → Create Empty
Rename: "GameManager"
Add Component: Game Manager
```

#### SceneTransitionManager

```
Hierarchy → Create Empty
Rename: "SceneTransitionManager"
Add Component: Scene Transition Manager
```

#### PauseManager

```
Hierarchy → Create Empty
Rename: "PauseManager"
Add Component: Pause Manager
```

**Configure PauseManager:**

- Paused Audio Volume: 0.2
- Resumed Audio Volume: 1.0
- Audio Fade Duration: 0.5
- Enable Logs: ✓ (para debug)

### Passo 6: Verificar EventSystem

Certifique-se de que há um EventSystem na cena:

```
Hierarchy → Right Click → UI → Event System
```

Se já existir, não precisa criar outro.

## 🎮 Controles

### Teclado

- **Escape** ou **Tab**: Abre/fecha menu de pause
- **Arrow Keys** ou **WASD**: Navega entre botões
- **Enter** ou **Space**: Confirma seleção

### Gamepad

- **Start**: Abre/fecha menu de pause
- **D-Pad** ou **Left Stick**: Navega entre botões
- **Button A/X**: Confirma seleção
- **Button B/Circle**: Cancela/volta

## 🔄 Fluxo de Funcionamento

```
Gameplay
   ↓ (Esc/Tab/Start)
PauseManager.Pause()
   ↓
pauseRefCount++ (stack)
   ↓
Time.timeScale = 0
   ↓
Audio fade to 0.2f (ducking)
   ↓
Gameplay input disabled
   ↓
PauseMenu recebe evento
   ↓
Fade in visual (0.3s)
   ↓
Menu visível + primeiro botão selecionado
   ├─ Inventário → Abre InventoryUI
   ├─ Salvar → (desabilitado)
   ├─ Carregar → (desabilitado)
   ├─ Continuar → Resume
   └─ Sair → Fade out + volta para 1_TitleScreen
      ↓ (Resume)
PauseManager.Resume()
   ↓
pauseRefCount--
   ↓ (se = 0)
Time.timeScale = 1
   ↓
Audio fade to 1.0f
   ↓
Gameplay input enabled
   ↓
Fade out visual (0.3s)
   ↓
Gameplay
```

## 🐛 Troubleshooting

### Menu não abre ao pressionar Esc

- Verifique se `PauseManager` está na cena
- Verifique se `PlayerController.Instance` existe
- Ative `Enable Logs` no PauseManager e veja o console
- Certifique-se de que o UI action map foi criado

### Jogo não despausa

- Verifique se `pauseRefCount` chegou a zero (pode haver múltiplos pause ativos)
- Ative logs no PauseManager para ver o stack count
- Verifique se DialogueManager não está mantendo pause ativo

### Fade não funciona

- Verifique se `CanvasGroup` está atribuído no PauseMenu
- Certifique-se de que `Fade Duration` > 0
- Verifique se não há erros no console interrompendo corrotinas

### Botões não respondem a gamepad

- Verifique se `EventSystem` existe na cena
- Certifique-se de que `Navigation` dos botões está configurado
- Verifique se o primeiro botão está sendo selecionado (logs)

### Áudio não faz ducking

- Verifique se há um `AudioListener` na cena (geralmente na Camera)
- Ajuste `Paused Audio Volume` no PauseManager (0.2 = 20% do volume)
- Verifique `Audio Fade Duration` (0.5s recomendado)

## ✨ Próximos Passos

1. **Implementar Save/Load System** - Habilitar botões Salvar e Carregar
2. **Integrar DialogueManager** - Fazer diálogos usarem PauseManager.Pause/Resume
3. **Adicionar UI action map** - Mover Menu do Gameplay para UI
4. **Adicionar easing curves** - AnimationCurve para fades mais suaves
5. **Persistência de volume** - Salvar/restaurar volume original do AudioListener

## 📝 Notas Importantes

- **Pause Stack**: Múltiplos sistemas podem pausar simultaneamente sem conflitos
- **Audio Ducking**: Volume reduzido (não mutado) mantém feedback sonoro
- **Unscaled Time**: Fades usam `Time.unscaledDeltaTime` para funcionar durante pause
- **Navigation**: Botões configurados com Explicit Navigation para controle preciso
- **EventSystem**: Necessário para navegação com gamepad/teclado funcionar
