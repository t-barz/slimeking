# Design Document

## Overview

Esta ferramenta adiciona um menu item no Unity Editor chamado "Setup Scene for Transitions" que automaticamente configura uma cena com todos os componentes essenciais necessários para permitir transições do slime entre diferentes cenas do jogo. A ferramenta detecta componentes existentes e adiciona apenas o que está faltando, garantindo uma configuração completa e não destrutiva.

## Architecture

### High-Level Architecture

```
Unity Editor Menu
       ↓
SceneSetupTool (Editor Script)
       ↓
   ┌───────────────────────────────┐
   │  Component Detection Logic    │
   └───────────────────────────────┘
       ↓
   ┌───────────────────────────────┐
   │  Component Creation Logic     │
   └───────────────────────────────┘
       ↓
   Scene Configured
```

### Integration Points

A ferramenta se integra com:

- **UnifiedExtraTools**: Menu principal de ferramentas do projeto
- **GameManager**: Sistema de gerenciamento do jogo
- **SceneTransitionManager**: Sistema de transições entre cenas
- **TeleportManager**: Sistema de teleporte
- **EventSystem**: Sistema de input e UI do Unity

## Components and Interfaces

### SceneSetupTool (Editor Script)

Classe principal que implementa a ferramenta de configuração de cena.

**Localização**: `Assets/Code/Editor/SceneSetupTool.cs`

**Responsabilidades**:

- Adicionar menu item no "Extra Tools"
- Detectar componentes existentes na cena
- Criar componentes faltantes
- Fornecer feedback via logs

**Métodos Principais**:

```csharp
// Menu item principal
[MenuItem("Extra Tools/Scene/🎬 Setup Scene for Transitions")]
public static void SetupSceneForTransitions()

// Detecta se um GameObject com nome específico existe
private static GameObject FindGameObjectByName(string name)

// Detecta se um componente específico existe na cena
private static T FindComponentInScene<T>() where T : Component

// Cria ou obtém GameObject com componente específico
private static GameObject EnsureGameObject(string name, System.Type componentType)

// Adiciona componente se não existir
private static T EnsureComponent<T>(GameObject go) where T : Component

// Configura GameManager
private static void SetupGameManager()

// Configura SceneTransitioner
private static void SetupSceneTransitioner()

// Configura TeleportManager
private static void SetupTeleportManager()

// Configura EventSystem
private static void SetupEventSystem()

// Exibe resumo final
private static void ShowSummary(int added, int existing)
```

### Component Detection Strategy

A ferramenta usa duas estratégias de detecção:

1. **Por Nome do GameObject**: Para componentes que tradicionalmente têm nomes específicos
   - GameManager
   - SceneTransitioner
   - TeleportManager
   - EventSystem

2. **Por Tipo do Componente**: Para componentes que podem estar em qualquer GameObject
   - EventSystem (fallback)
   - InputSystemUIInputModule

### Component Creation Strategy

Para cada componente essencial:

1. **Verificar Existência**: Buscar por nome ou tipo
2. **Reutilizar ou Criar**: Se existe, reutilizar; se não, criar novo GameObject
3. **Adicionar Componentes**: Garantir que todos os componentes necessários estão presentes
4. **Configurar Posição**: Posicionar em (0, 0, 0) para novos GameObjects
5. **Registrar Ação**: Log indicando se foi criado ou já existia

## Data Models

### SetupResult

Estrutura para rastrear resultados da configuração:

```csharp
private struct SetupResult
{
    public int ComponentsAdded;
    public int ComponentsExisting;
    public List<string> AddedComponents;
    public List<string> ExistingComponents;
}
```

### ComponentInfo

Informações sobre cada componente a ser configurado:

```csharp
private struct ComponentInfo
{
    public string Name;              // Nome do GameObject
    public System.Type ComponentType; // Tipo do componente principal
    public Vector3 Position;         // Posição padrão
    public bool IsRequired;          // Se é obrigatório
}
```

## Error Handling

### Estratégia de Error Handling

A ferramenta implementa "graceful degradation":

1. **Validação de Cena Ativa**: Verifica se há uma cena aberta
2. **Validação de Componentes**: Verifica se os tipos de componentes existem
3. **Logs Informativos**: Fornece feedback claro sobre cada ação
4. **Não Destrutivo**: Nunca remove ou sobrescreve componentes existentes
5. **Marca Cena como Dirty**: Permite que o desenvolvedor salve as mudanças

### Casos de Erro

| Erro | Tratamento |
|------|-----------|
| Nenhuma cena aberta | Exibir erro e abortar |
| Componente já existe | Reutilizar e registrar no log |
| Falha ao criar GameObject | Registrar erro e continuar com próximo |
| Tipo de componente não encontrado | Registrar warning e pular |

## Testing Strategy

### Manual Testing Checklist

**Cenário 1: Cena Vazia**

- [ ] Executar ferramenta em cena completamente vazia
- [ ] Verificar se todos os 4 componentes foram criados
- [ ] Verificar posições dos GameObjects
- [ ] Verificar logs de criação

**Cenário 2: Cena Parcialmente Configurada**

- [ ] Criar GameManager manualmente
- [ ] Executar ferramenta
- [ ] Verificar se GameManager foi reutilizado
- [ ] Verificar se outros 3 componentes foram criados

**Cenário 3: Cena Completamente Configurada**

- [ ] Configurar todos os 4 componentes manualmente
- [ ] Executar ferramenta
- [ ] Verificar se nenhum componente foi duplicado
- [ ] Verificar logs indicando componentes existentes

**Cenário 4: Componentes com Nomes Diferentes**

- [ ] Criar GameObject "MyGameManager" com componente GameManager
- [ ] Executar ferramenta
- [ ] Verificar se detectou o componente existente
- [ ] Verificar se não criou duplicata

**Cenário 5: EventSystem do Unity**

- [ ] Criar EventSystem via menu Unity (GameObject > UI > Event System)
- [ ] Executar ferramenta
- [ ] Verificar se reutilizou o EventSystem existente
- [ ] Verificar se adicionou InputSystemUIInputModule se necessário

### Integration Testing

**Teste 1: Transição Entre Cenas**

1. Configurar duas cenas usando a ferramenta
2. Adicionar TeleportPoint em cada cena
3. Configurar cross-scene teleport
4. Testar transição do slime entre as cenas
5. Verificar se todos os managers funcionam corretamente

**Teste 2: Compatibilidade com Cenas Existentes**

1. Abrir cena existente do projeto (ex: InitialCave)
2. Executar ferramenta
3. Verificar se não quebrou configurações existentes
4. Testar gameplay na cena

## Implementation Details

### Menu Integration

A ferramenta será adicionada ao menu "Extra Tools" seguindo o padrão existente:

```csharp
[MenuItem("Extra Tools/Scene/🎬 Setup Scene for Transitions")]
public static void SetupSceneForTransitions()
```

Também será adicionada à janela UnifiedExtraTools na aba "Scene" (nova aba):

```csharp
// Em UnifiedExtraTools.cs
private readonly string[] tabNames = { "NPC", "Camera", "Scene", "Project", "Post Processing", "Debug" };

private void DrawSceneSection()
{
    EditorGUILayout.LabelField("🎬 Scene Setup", EditorStyles.boldLabel);
    EditorGUILayout.Space(5);

    if (GUILayout.Button("🎬 Setup Scene for Transitions", GUILayout.Height(30)))
    {
        SceneSetupTool.SetupSceneForTransitions();
    }

    EditorGUILayout.Space(10);
    EditorGUILayout.HelpBox("Configure cena com componentes essenciais para transições", MessageType.Info);
}
```

### Component Configuration Details

#### GameManager

- **GameObject Name**: "GameManager"
- **Components**: GameManager
- **Position**: (0, 0, 0)
- **Notes**: Singleton que gerencia estado do jogo

#### SceneTransitioner

- **GameObject Name**: "SceneTransitioner"  
- **Components**: SceneTransitionManager (ou SceneTransitioner se existir)
- **Position**: (0, 0, 0)
- **Notes**: Gerencia transições visuais entre cenas

#### TeleportManager

- **GameObject Name**: "TeleportManager"
- **Components**: TeleportManager
- **Position**: (0, 0, 0)
- **Notes**: Gerencia sistema de teleporte cross-scene

#### EventSystem

- **GameObject Name**: "EventSystem"
- **Components**: EventSystem, InputSystemUIInputModule
- **Position**: (0, 0, 0)
- **Notes**: Sistema de input do Unity, pode já existir

### Logging Strategy

A ferramenta usa o padrão de logging estabelecido no projeto:

```csharp
private static void Log(string message)
{
    Debug.Log($"[SceneSetupTool] {message}");
}

private static void LogWarning(string message)
{
    Debug.LogWarning($"[SceneSetupTool] {message}");
}

private static void LogError(string message)
{
    Debug.LogError($"[SceneSetupTool] {message}");
}
```

### Summary Dialog

Ao final da execução, exibe um dialog com resumo:

```
Setup da Cena Concluído!

✅ Componentes Adicionados: 2
  • SceneTransitioner
  • TeleportManager

✅ Componentes Existentes: 2
  • GameManager
  • EventSystem

A cena está pronta para transições entre cenas!
```

## Design Decisions and Rationales

### Decisão 1: Detecção por Nome vs Tipo

**Decisão**: Usar detecção por nome como estratégia primária, com fallback para tipo.

**Rationale**:

- Os componentes do projeto seguem convenção de nomes consistente
- Detecção por nome é mais rápida
- Fallback por tipo garante robustez
- Permite detectar componentes mesmo se renomeados

### Decisão 2: Não Destrutivo

**Decisão**: Nunca remover ou sobrescrever componentes existentes.

**Rationale**:

- Preserva configurações customizadas do desenvolvedor
- Reduz risco de perda de dados
- Permite execução múltipla sem efeitos colaterais
- Segue princípio de "First, do no harm"

### Decisão 3: Posição Padrão (0, 0, 0)

**Decisão**: Criar todos os managers na posição (0, 0, 0).

**Rationale**:

- Managers não têm representação visual
- Posição não afeta funcionalidade
- Facilita localização no Hierarchy
- Consistente com padrão do projeto

### Decisão 4: Integração com UnifiedExtraTools

**Decisão**: Adicionar à janela UnifiedExtraTools além do menu.

**Rationale**:

- Consistente com outras ferramentas do projeto
- Facilita descoberta da ferramenta
- Centraliza ferramentas de desenvolvimento
- Melhora UX do desenvolvedor

### Decisão 5: Logs Detalhados

**Decisão**: Fornecer logs detalhados de cada ação.

**Rationale**:

- Transparência sobre o que foi modificado
- Facilita debugging
- Permite auditoria de mudanças
- Educa desenvolvedor sobre estrutura da cena

## Future Enhancements

### Possíveis Melhorias Futuras

1. **Configuração Customizável**: Permitir escolher quais componentes adicionar
2. **Templates de Cena**: Salvar/carregar configurações de cena
3. **Validação de Cena**: Verificar se cena está corretamente configurada
4. **Auto-Setup on Scene Creation**: Executar automaticamente ao criar nova cena
5. **Undo Support**: Permitir desfazer mudanças via Ctrl+Z
6. **Batch Processing**: Configurar múltiplas cenas de uma vez
7. **Configuration Profiles**: Diferentes perfis para diferentes tipos de cena

### Extensibilidade

A ferramenta é projetada para ser facilmente extensível:

```csharp
// Adicionar novo componente é simples:
private static void SetupNewComponent()
{
    var result = EnsureGameObject("NewComponent", typeof(NewComponentType));
    // ... configuração adicional
}

// E adicionar à sequência principal:
public static void SetupSceneForTransitions()
{
    // ... componentes existentes
    SetupNewComponent();
    // ...
}
```
