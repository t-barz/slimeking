# Coding Standards - The Slime King

## 📋 Estrutura Geral

### Organização de Arquivos

- **Editor tools**: `Assets/Editor/[ToolName]/`
- **Scripts do jogo**: `Assets/_Code/Scripts/`
- **Prefabs**: `Assets/_Prefabs/`
- **Cenas**: `Assets/_Scenes/`
- **Documentação**: `Assets/Docs/Worklogs/` (para worklogs e logs de implementação)
- **Assets externos**: `Assets/External Assets/` (NÃO MODIFICAR)

#### 🗂️ Mapa de Diretórios (Atual)

Estrutura principal do projeto e propósito de cada pasta/arquivo relevante:

```text
Raiz/
├── Assets/
│   ├── _Code/                     # Scripts do jogo (namespace organizado)
│   │   ├── Managers/              # Singletons e controladores globais
│   │   ├── UI/                    # Componentes de UI com Input System
│   │   ├── Items/                 # ScriptableObjects de itens e enums
│   │   └── Environments/          # Scripts de ambientes/cenas
│   ├── _Prefabs/                  # Prefabs de GameObjects
│   ├── _Scenes/                   # Cenas (TitleScreen, InitialCave, etc.)
│   ├── AddressableAssetsData/     # Configurações do Addressables
│   ├── Docs/                      # Documentação (inclui CodingStandards e Worklogs)
│   ├── Editor/                    # Ferramentas de Editor (MenuItem, Windows)
│   ├── External Assets/           # Pacotes/recursos de terceiros (NÃO MODIFICAR)
│   ├── Resources/                 # Recursos carregados em runtime
│   ├── Screenshots/               # Capturas de tela do projeto
│   ├── Settings/                  # ScriptableObjects de configurações
│   ├── Tests/                     # Arquivos de teste temporários (excluir após uso)
│   ├── TextMesh Pro/              # Dados do TMP
│   ├── InputSystem_Actions.inputactions   # Mapa do Input System
│   ├── UniversalRenderPipelineGlobalSettings.asset  # Config global URP
│   └── DefaultVolumeProfile.asset  # Perfil de pós-processamento padrão (URP)
├── Packages/                      # Manifesto e lock de pacotes (UPM)
├── ProjectSettings/               # Configurações do projeto (Editor, Graphics, etc.)
├── Library/                       # Cache do Unity (gerado automaticamente, não versionar)
├── Logs/                          # Logs de execução/edição
├── Temp/                          # Arquivos temporários de build
└── UserSettings/                  # Preferências do usuário/editor
```

Notas importantes:
- Não modificar conteúdos em `Assets/External Assets/`.
- Editor tools sempre em `Assets/Editor/[ToolName]/` seguindo estrutura modular.
- Testes devem ser temporários e removidos imediatamente após execução.
- Todos paths devem ser relativos a `Assets/` nas operações de Editor.

### Estrutura de Classes

```csharp
// Ordem obrigatória:
1. using statements
2. namespace
3. XML documentation
4. class declaration
5. #region Fields
6. #region Unity Lifecycle (Awake, Start, OnEnable, OnDisable, Update, etc)
7. #region Public Methods
8. #region Private Methods
9. #region Utility Methods
```

---

## 🎯 Convenções de Nomenclatura

### Classes e Métodos

- **PascalCase** para classes, métodos, propriedades
- **camelCase** para campos privados
- **UPPER_CASE** para constantes

```csharp
public class GameObjectBrushTool  // ✅ PascalCase
private float brushRadius;        // ✅ camelCase
private const string VERSION = "1.0"; // ✅ UPPER_CASE
```

### Prefixos e Sufixos

- Editor Windows: `*Window.cs` ou `*Tool.cs`
- Services: `*Service.cs`
- Settings: `*Settings.cs`
- Managers: `*Manager.cs`

### Nomes de Arquivos e Pastas

- ❌ **NUNCA** usar emojis em nomes de arquivos ou pastas
- ✅ Usar apenas caracteres alfanuméricos, hífens e underscores
- ✅ PascalCase para arquivos de código
- ✅ kebab-case ou snake_case para documentação

---

## 📝 Documentação

### XML Documentation Obrigatória

Toda classe pública deve ter:

```csharp
/// <summary>
/// Descrição breve do propósito da classe
/// 
/// Detalhes adicionais sobre uso, funcionalidades, etc.
/// 
/// Acesso: Menu > Extra Tools > [Category]
/// </summary>
```

### Comentários de Métodos Complexos

```csharp
/// <summary>
/// Descrição do que o método faz
/// </summary>
/// <param name="name">Descrição do parâmetro</param>
/// <returns>Descrição do retorno</returns>
```

### Logs de Implementação

- Toda implementação significativa deve gerar um worklog em `Assets/Docs/Worklogs/`
- Formato: `YYYY-MM-DD-feature-name.md`
- Incluir: objetivo, decisões técnicas, arquivos modificados

---

## 🎨 Menu Structure (Unity Editor)

### Hierarquia Obrigatória de Menus

#### Extra Tools (Menu Principal)

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
```

#### Quick Tools (Menu de Contexto)

```text
Quick Tools/
└── Debug/
    └── Export Object Structure
```

### MenuItem Format

```csharp
// Menu principal
[MenuItem("Extra Tools/[Category]/[Feature Name]")]

// Menu de contexto
[MenuItem("GameObject/Quick Tools/[Category]/[Feature Name]")]

// Validação de menu de contexto
[MenuItem("GameObject/Quick Tools/[Category]/[Feature Name]", true)]
```

---

## � Arquitetura de Cenas

### Scene Controllers

Toda cena deve ter uma classe Controller responsável por questões específicas daquela cena:

- **Padrão de nomenclatura**: `[NomeDaScene]Controller.cs`
- **Localização**: `Assets/_Code/Scripts/Controllers/` ou `Assets/_Code/Gameplay/`
- **Responsabilidades**: Inicialização da cena, gerenciamento de estado, coordenação de sistemas

```csharp
/// <summary>
/// Controller principal da cena MainMenu.
/// Gerencia a inicialização e comportamento específico desta cena.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // Inicialização e lógica específica da cena MainMenu
}
```

**Exemplos de nomenclatura:**

- `MainMenuController.cs` - Controller da cena "MainMenu"
- `GameplayController.cs` - Controller da cena "Gameplay"
- `Level1Controller.cs` - Controller da cena "Level1"

---

## �🏗️ Arquitetura de Editor Tools

### Estrutura Modular

Ferramentas complexas devem ser divididas em:

1. **Window** - UI e orquestração
2. **Settings** - Configurações e persistência
3. **Services** - Lógica de negócio
4. **Utilities** - Funções auxiliares

```text
Assets/Editor/[ToolName]/
├── [ToolName]Window.cs      // EditorWindow principal
├── [ToolName]Settings.cs    // Configurações e EditorPrefs
├── [Feature]Service.cs      // Lógica específica
└── [Helper]Utility.cs       // Funções auxiliares
```

---

## ⚡ Performance

### Unity Editor

- ✅ Cachear referências em `OnEnable()`
- ✅ Usar `sqrMagnitude` ao invés de `Distance()` quando possível
- ✅ Usar operações batch com Undo
- ✅ Evitar `Find()`, `FindObjectsOfType()` em loops
- ✅ Usar `Dictionary` para lookups frequentes
- ❌ Não usar `Resources.Load()` no Editor

### Serialização

```csharp
// Preferir EditorPrefs para configurações de editor
EditorPrefs.SetFloat("ToolName_BrushRadius", brushRadius);

// Usar JsonUtility para estruturas complexas
string json = JsonUtility.ToJson(data, true);
File.WriteAllText(path, json);
```

---

## 🎮 Unity Específico

### Campos Serializados

```csharp
[SerializeField] private float speed;    // ✅ Preferir
public float speed;                      // ❌ Evitar expor desnecessariamente
```

### Undo/Redo

```csharp
// Sempre registrar operações destrutivas
Undo.RecordObject(target, "Operation Name");
Undo.DestroyObjectImmediate(obj);
Undo.RegisterCreatedObjectUndo(instance, "Create Object");

// Para múltiplas operações
Undo.SetCurrentGroupName("Batch Operation");
```

### Asset Management

```csharp
// Sempre refresh após modificar assets
AssetDatabase.Refresh();

// Usar paths relativos
string relativePath = "Assets/Docs/Temp/file.json";
```

---

## 🎨 UI Guidelines (Editor)

### Cores e Feedback Visual

```csharp
// Botões de modo com cores semânticas
GUI.backgroundColor = Color.green;      // Ativo/Sucesso
GUI.backgroundColor = Color.red;        // Perigo/Eraser
GUI.backgroundColor = new Color(1f, 0.6f, 0.2f); // Alerta/Seletivo
GUI.backgroundColor = Color.white;      // Reset
```

### Emojis para Melhor UX

```csharp
// ✅ Permitido APENAS em strings de UI
"🖌️ GameObject Brush Tool"  // Títulos de janelas
"📦 Prefab Slots"            // Seções
"⚙️ Settings"                // Configurações
"🎲 Randomization"           // Features especiais
"🔧 Debug"                   // Ferramentas de debug

// ❌ NUNCA em nomes de arquivos ou pastas
// Errado: "GameObject Brush Tool 🖌️.cs"
// Certo:  "GameObjectBrushTool.cs"
```

### HelpBox

```csharp
EditorGUILayout.HelpBox("Mensagem informativa", MessageType.Info);
EditorGUILayout.HelpBox("Atenção!", MessageType.Warning);
EditorGUILayout.HelpBox("Erro crítico", MessageType.Error);
```

---

## 🔒 Segurança e Validação

### Sempre Validar

```csharp
// Verificar nulls
if (obj == null) return;

// Verificar bounds
if (index < 0 || index >= list.Count) return;

// Usar properties com validação
private int SafeSelectedIndex
{
    get => Mathf.Clamp(selectedIndex, 0, list.Count - 1);
    set => selectedIndex = Mathf.Clamp(value, 0, list.Count - 1);
}
```

### EditorUtility.DisplayDialog

```csharp
// Confirmar ações destrutivas
bool confirmed = EditorUtility.DisplayDialog(
    "Confirmar Ação",
    "Esta operação não pode ser desfeita. Continuar?",
    "Sim",
    "Cancelar"
);
```

---

## 📊 Debugging

### Debug Logs Opcionais

```csharp
private bool enableDebugLogs = false;

private void DebugLog(string message)
{
    if (enableDebugLogs)
    {
        Debug.Log($"[{GetType().Name}] {message}");
    }
}
```

---

## 🚫 Evitar

- ❌ Código comentado (usar Git para histórico)
- ❌ Magic numbers (usar constantes nomeadas)
- ❌ Métodos com mais de 50 linhas
- ❌ Classes com mais de 500 linhas (refatorar em services)
- ❌ `GameObject.Find()` ou `FindObjectsOfType()` em loops
- ❌ Operações de I/O sem tratamento de exceção
- ❌ Emojis em nomes de arquivos ou pastas
- ❌ Autor e data de criação em XML documentation

---

## 📖 Referências

- Unity Editor Scripting: <https://docs.unity3d.com/ScriptReference/Editor.html>
- MenuItem Attribute: <https://docs.unity3d.com/ScriptReference/MenuItem.html>
- EditorPrefs: <https://docs.unity3d.com/ScriptReference/EditorPrefs.html>
- Undo System: <https://docs.unity3d.com/ScriptReference/Undo.html>
