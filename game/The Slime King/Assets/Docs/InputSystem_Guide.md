# 🎮 Input System Guide - The Slime King

## 📋 Visão Geral

Este guia documenta a implementação do **Unity Input System** no projeto The Slime King, incluindo estrutura de mapas, padrões de nomenclatura, fluxo de integração e sistema de rebinding.

### 🏗️ Arquitetura Híbrida

O projeto utiliza uma **arquitetura híbrida** otimizada:

- **PlayerController**: Usa Input System **nativo** para máxima performance
- **InputManager**: Wrapper para **UI e System** (flexibilidade e reutilização)
- **TitleScreenController**: Usa InputManager para compatibilidade

## 📂 Estrutura de Arquivos

```
Assets/
├── InputSystem_Actions.inputactions          # Asset principal do Input System
├── InputSystem_Actions.cs                    # Código gerado automaticamente
├── Code/
│   ├── Systems/InputManager.cs               # Wrapper para UI/System
│   ├── Gameplay/PlayerController.cs          # Usa Input System nativo
│   └── Systems/TitleScreenController.cs      # Usa InputManager
└── Docs/
    ├── InputSystem_Guide.md                  # Este guia
    └── BoasPraticas.md                       # Padrões de nomenclatura
```

## 🗂️ Estrutura de Mapas

### 📋 Mapas Organizados

O Input Action Asset está dividido em **3 mapas principais**:

#### 1. 🎮 **UI Map** - Navegação de Interface

```
Navigate    (Vector2)  # WASD, Arrows, D-Pad, Left Stick
Submit      (Button)   # Enter, Space, Gamepad South (A)
Cancel      (Button)   # Esc, Backspace, Gamepad East (B)
Point       (Vector2)  # Mouse Position (opcional)
Click       (Button)   # Mouse Left Click (opcional)
```

#### 2. 🕹️ **Gameplay Map** - Ações do Jogador

```
Move            (Vector2)  # WASD, Arrows, D-Pad, Left Stick
Attack          (Button)   # Z, Space, Gamepad South (A)
Interact        (Button)   # E, Enter, Gamepad West (X)
SpecialAttack   (Button)   # X, Shift, Gamepad North (Y)
Crouch          (Button)   # S, Down Arrow, Gamepad East (B)
UseItem1        (Button)   # 1, Gamepad D-Pad Up
UseItem2        (Button)   # 2, Gamepad D-Pad Right
UseItem3        (Button)   # 3, Gamepad D-Pad Down
UseItem4        (Button)   # 4, Gamepad D-Pad Left
```

#### 3. ⚙️ **System Map** - Controles Globais

```
Menu       (Button)  # Esc, Tab, Gamepad Menu (Start)
Inventory  (Button)  # I, Tab, Gamepad View (Select)
Skip       (Button)  # Any Key, Any Gamepad Button
```

### 🎯 Esquemas de Controle

#### **Keyboard&Mouse** (Principal)

- Bindings primários para teclado e mouse
- Usado como referência para documentação

#### **Gamepad** (Secundário)

- Compatível com Xbox, PlayStation, Generic gamepads
- Dead zones configuradas automaticamente

#### **Extras** (Preparação Futura)

- Touch, Joystick, XR (já configurados no asset)

## 📝 Padrões de Nomenclatura

### 🏷️ Convenções de Nomes

#### **Actions (Ações)**

```csharp
// ✅ Correto - PascalCase, verbos de ação
Move, Attack, Interact, Submit, Cancel

// ❌ Evitar - snake_case, nomes vagos
move_player, action1, button_press
```

#### **Maps (Mapas)**

```csharp
// ✅ Correto - PascalCase, categoria clara
UI, Gameplay, System

// ❌ Evitar - Lower case, nomes genéricos
ui, player, controls
```

#### **Bindings (Vinculações)**

```csharp
// ✅ Correto - Descritivo, específico
<Keyboard>/w, <Gamepad>/leftStick/up

// ❌ Evitar - Caminhos genéricos
<Keyboard>/anyKey, <Gamepad>/*button
```

### 🎮 Padrão de Gamepad

Seguindo convenção **Xbox** como referência:

- **South (A)**: Ação principal (Attack, Submit)
- **East (B)**: Cancelar, Voltar (Cancel, Crouch)
- **West (X)**: Interação (Interact)
- **North (Y)**: Ação secundária (SpecialAttack)

## 🔧 Como Adicionar Nova Ação

### 📋 Passo a Passo

#### 1. **Determinar o Mapa Correto**

```
UI       → Navegação de menus e interfaces
Gameplay → Ações do personagem jogador
System   → Controles globais (pause, inventory)
```

#### 2. **Adicionar no Input Action Asset**

1. Abrir `InputSystem_Actions.inputactions`
2. Selecionar o mapa apropriado
3. Clicar **"+"** para adicionar ação
4. Configurar nome (PascalCase)
5. Definir tipo (Button/Value/PassThrough)
6. Adicionar bindings para Keyboard&Mouse e Gamepad

#### 3. **Implementar no Código**

##### **Para UI/System** (usar InputManager)

```csharp
// Em InputManager.cs
public static event Action OnNovaAcao;

private void OnEnable()
{
    _inputActions.System.NovaAcao.performed += OnNovaAcaoPerformed;
}

private void OnNovaAcaoPerformed(InputAction.CallbackContext context)
{
    OnNovaAcao?.Invoke();
}
```

##### **Para Gameplay** (usar PlayerController)

```csharp
// Em PlayerController.cs
private void OnEnable()
{
    _inputActions.Gameplay.NovaAcao.performed += OnNovaAcaoPerformed;
}

private void OnNovaAcaoPerformed(InputAction.CallbackContext context)
{
    // Implementar lógica específica
    Debug.Log("Nova ação executada!");
}
```

#### 4. **Atualizar Documentação**

- Adicionar entrada neste guia
- Atualizar comentários no código
- Documentar comportamento esperado

### ⚠️ Considerações Importantes

#### **Regeneração Automática**

- O arquivo `InputSystem_Actions.cs` é **gerado automaticamente**
- **Nunca editar** este arquivo diretamente
- Toda lógica deve ir em InputManager ou PlayerController

#### **Performance**

- PlayerController usa Input System **nativo** (zero overhead)
- InputManager adiciona uma camada de wrapper (aceitável para UI)
- Prefer Input System nativo para ações críticas de gameplay

## 🔄 Sistema de Rebinding

### 🎯 Implementação Básica

#### **Estrutura Recomendada**

```csharp
public class RebindingManager : MonoBehaviour
{
    [Header("Rebinding Settings")]
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string bindingPath = "binding_overrides.json";
    
    public void StartRebind(string actionName, int bindingIndex)
    {
        var action = inputActions.FindAction(actionName);
        if (action == null) return;
        
        var rebindOperation = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => OnRebindComplete(operation))
            .OnCancel(operation => OnRebindCancel(operation));
            
        rebindOperation.Start();
    }
    
    private void OnRebindComplete(InputActionRebindingExtensions.RebindingOperation operation)
    {
        SaveBindingOverrides();
        operation.Dispose();
    }
}
```

#### **Persistência de Dados**

```csharp
public void SaveBindingOverrides()
{
    string json = inputActions.SaveBindingOverridesAsJson();
    PlayerPrefs.SetString("InputBindings", json);
    PlayerPrefs.Save();
}

public void LoadBindingOverrides()
{
    string json = PlayerPrefs.GetString("InputBindings", "");
    if (!string.IsNullOrEmpty(json))
    {
        inputActions.LoadBindingOverridesFromJson(json);
    }
}
```

### 📋 Fluxo de Rebinding

#### **1. Preparação**

```
1. Pausar input atual
2. Mostrar UI de rebinding
3. Indicar qual ação está sendo rebindada
4. Mostrar binding atual
```

#### **2. Execução**

```
1. Chamar PerformInteractiveRebinding()
2. Filtrar controles indesejados (Mouse, etc.)
3. Aguardar input do usuário
4. Validar se binding é válido
```

#### **3. Finalização**

```
1. Aplicar novo binding
2. Salvar em PlayerPrefs/JSON
3. Atualizar UI com novo binding
4. Reativar input normal
```

### ⚙️ Configurações Avançadas

#### **Exclusão de Controles**

```csharp
.WithControlsExcluding("Mouse")          // Excluir mouse
.WithControlsExcluding("<Keyboard>/escape") // Excluir ESC
.WithControlsHaving("<Gamepad>")         // Apenas gamepad
```

#### **Validação de Conflitos**

```csharp
private bool IsBindingConflicting(string actionName, string newBinding)
{
    foreach (var action in inputActions.actionMaps.SelectMany(map => map.actions))
    {
        if (action.name == actionName) continue;
        
        foreach (var binding in action.bindings)
        {
            if (binding.effectivePath == newBinding)
                return true;
        }
    }
    return false;
}
```

## 🧪 Testing e Debug

### 🔍 Ferramentas de Debug

#### **Input Debugger**

```
Window → Analysis → Input Debugger
- Visualizar ações em tempo real
- Verificar bindings ativos
- Monitorar valores de input
```

#### **Logs Customizados**

```csharp
// Em PlayerController.cs
#if UNITY_EDITOR || DEVELOPMENT_BUILD
private void OnMovePerformed(InputAction.CallbackContext context)
{
    Vector2 input = context.ReadValue<Vector2>();
    if (enableLogs)
        Debug.Log($"[PlayerController] Move Input: {input}");
}
#endif
```

### ✅ Checklist de Testes

#### **Keyboard & Mouse**

- [ ] Todas as ações respondem corretamente
- [ ] Bindings múltiplos funcionam (WASD + Arrows)
- [ ] Navegação UI responsiva
- [ ] Skip funciona com "qualquer tecla"

#### **Gamepad**

- [ ] Dead zones configuradas corretamente
- [ ] D-Pad e Analog Stick funcionam
- [ ] Botões mapeados seguem convenção Xbox
- [ ] Desconexão/reconexão não quebra input

#### **Sistemas**

- [ ] PlayerController funciona independente
- [ ] InputManager não interfere no gameplay
- [ ] TitleScreen responde via InputManager
- [ ] Transições entre mapas funcionam

## 📚 Referências e Links Úteis

### 📖 Documentação Unity

- [Input System Package](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Input Action Assets](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/ActionAssets.html)
- [Rebinding](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/ActionBindings.html#interactive-rebinding)

### 🎮 Convenções de Gamepad

- [Xbox Controller Layout](https://docs.microsoft.com/en-us/gaming/xbox-live/get-started/setup-ide/managed-partners/unity-win10/input-in-unity)
- [PlayStation Controller](https://partner.steamgames.com/doc/api/steam_input)

### 🔧 Ferramentas

- [Input System Visualizer](https://github.com/Unity-Technologies/InputSystem/tree/develop/Packages/com.unity.inputsystem/InputSystem/Editor/Tools)

## 🐛 Problemas Comuns e Soluções

### ❌ Input Não Responde

**Problema**: Ação não executa quando tecla é pressionada

**Soluções**:

1. Verificar se Input Action Asset está ativado
2. Confirmar se mapa correto está habilitado
3. Checar se método está subscrito ao evento
4. Validar binding path no Input Debugger

### ❌ Conflitos de Input

**Problema**: Múltiplas ações disparando simultaneamente

**Soluções**:

1. Reorganizar prioridades dos mapas
2. Usar `interactions` para diferenciação
3. Implementar sistema de contexto
4. Revisar bindings conflitantes

### ❌ Performance Ruim

**Problema**: Framerate baixo durante input intenso

**Soluções**:

1. Usar Input System nativo no PlayerController
2. Evitar wrapper desnecessário
3. Implementar pooling para eventos
4. Usar `performed` ao invés de `started`

### ❌ Gamepad Não Detectado

**Problema**: Controle não responde

**Soluções**:

1. Verificar se gamepad está conectado no Input Debugger
2. Conferir se esquema Gamepad está ativo
3. Testar com controle diferente
4. Verificar drivers do sistema

## 📊 Métricas e Performance

### 🎯 Alvos de Performance

| Métrica | Alvo | Máximo Aceitável |
|---------|------|------------------|
| **Input Lag** | < 1ms | < 5ms |
| **Memory Allocation** | 0 KB/frame | < 1 KB/frame |
| **Event Frequency** | 60 Hz | 120 Hz |

### 📈 Monitoramento

```csharp
// Exemplo de profiling de input
public class InputProfiler : MonoBehaviour
{
    private float lastInputTime;
    private int inputEventsThisFrame;
    
    private void LateUpdate()
    {
        if (inputEventsThisFrame > 10)
            Debug.LogWarning($"High input frequency: {inputEventsThisFrame} events");
        
        inputEventsThisFrame = 0;
    }
    
    public void OnInputEvent()
    {
        inputEventsThisFrame++;
        float currentTime = Time.realtimeSinceStartup;
        float inputLag = currentTime - lastInputTime;
        
        if (inputLag > 0.005f) // 5ms
            Debug.LogWarning($"Input lag detected: {inputLag * 1000:F1}ms");
            
        lastInputTime = currentTime;
    }
}
```

## 🚀 Roadmap Futuro

### 📋 Próximas Implementações

#### **Curto Prazo** (1-2 semanas)

- [ ] EventSystem + InputSystemUIInputModule
- [ ] Rebinding básico para controles principais
- [ ] Validação completa de gamepad

#### **Médio Prazo** (1 mês)

- [ ] Sistema de profiles múltiplos
- [ ] Configurações avançadas (dead zones, sensibilidade)
- [ ] Input hints dinâmicos na UI

#### **Longo Prazo** (2+ meses)

- [ ] Suporte a controles especializados
- [ ] Analytics de uso de input
- [ ] Acessibilidade avançada

### 🎯 Melhorias Planejadas

1. **Accessibility**: Suporte a controles adaptativos
2. **Mobile**: Implementar touch controls
3. **VR**: Preparação para controles VR
4. **Analytics**: Telemetria de uso de controles

---

## 📝 Changelog

### v1.0.0 (Atual)

- ✅ Estrutura de 3 mapas implementada
- ✅ PlayerController com Input System nativo
- ✅ InputManager wrapper funcional
- ✅ TitleScreen integrado

### v1.1.0 (Planejado)

- 🔄 EventSystem UI Navigation
- 🔄 Rebinding básico
- 🔄 Validação completa de gamepad

---

**📍 Localização**: `Assets/Docs/InputSystem_Guide.md`
**🔄 Última Atualização**: Outubro 2025
**👤 Autor**: Equipe The Slime King
**📚 Versão**: 1.0.0
