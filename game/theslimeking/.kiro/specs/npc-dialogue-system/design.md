# Design Document - NPC Dialogue System

## Overview

O NPC Dialogue System é um sistema simples e direto para exibir diálogos de NPCs no jogo The Slime King. O sistema segue o princípio KISS (Keep It Simple, Stupid) e consiste em apenas 3 componentes principais:

1. **DialogueNPC** - Componente anexado aos NPCs que gerencia a interação e os dados do diálogo
2. **DialogueUI** - Componente que gerencia a interface visual do diálogo (Canvas)
3. **TypewriterEffect** - Componente responsável pelo efeito de digitação letra por letra

O sistema utiliza Unity Localization para textos, TextMeshPro para renderização, e UnityEvents para extensibilidade futura.

## Architecture

### Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                         Game Scene                           │
│                                                               │
│  ┌──────────────┐                    ┌──────────────────┐   │
│  │     NPC      │                    │   Player         │   │
│  │  GameObject  │◄───interaction────►│   GameObject     │   │
│  │              │    (proximity)     │                  │   │
│  │ ┌──────────┐ │                    └──────────────────┘   │
│  │ │Dialogue  │ │                                            │
│  │ │   NPC    │ │                                            │
│  │ └────┬─────┘ │                                            │
│  │      │       │                                            │
│  │      │ triggers                                           │
│  │      ▼       │                                            │
│  └──────────────┘                                            │
│         │                                                     │
│         │                                                     │
│         ▼                                                     │
│  ┌─────────────────────────────────────────────────┐        │
│  │            Dialogue Canvas (UI)                  │        │
│  │  ┌────────────────────────────────────────┐     │        │
│  │  │         DialogueUI Component           │     │        │
│  │  │  ┌──────────────────────────────┐      │     │        │
│  │  │  │  TypewriterEffect Component  │      │     │        │
│  │  │  └──────────────────────────────┘      │     │        │
│  │  │  - Background Image                    │     │        │
│  │  │  - TextMeshPro Text                    │     │        │
│  │  │  - Continue Indicator                  │     │        │
│  │  └────────────────────────────────────────┘     │        │
│  └─────────────────────────────────────────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

### Data Flow

```
1. Player enters NPC interaction radius
   └─> DialogueNPC detects player
       └─> Shows interaction indicator

2. Player presses interaction button
   └─> DialogueNPC.StartDialogue()
       └─> DialogueUI.Show(dialogueData)
           └─> TypewriterEffect.StartTyping(firstText)
               └─> Characters appear one by one

3. Player presses continue button
   ├─> If typewriter is running:
   │   └─> TypewriterEffect.CompleteInstantly()
   │
   └─> If typewriter is complete:
       ├─> If more texts exist:
       │   └─> TypewriterEffect.StartTyping(nextText)
       │
       └─> If no more texts:
           └─> DialogueUI.Hide()
               └─> Invoke dialogue events
               └─> Restore player control
```

## Components and Interfaces

### 1. DialogueNPC Component

**Responsibility:** Gerencia a interação do jogador com o NPC e armazena os dados do diálogo.

**Public Fields:**
```csharp
[Header("Dialogue Configuration")]
public List<LocalizedString> dialogueTexts;  // Lista de textos localizados
public float interactionRadius = 1.5f;        // Raio de interação

[Header("Events")]
public UnityEvent onDialogueComplete;         // Eventos ao final do diálogo

[Header("Visual Feedback")]
public GameObject interactionIndicator;       // Indicador visual (opcional)
```

**Public Methods:**
```csharp
public void StartDialogue()                   // Inicia o diálogo
public bool IsPlayerInRange()                 // Verifica se jogador está no raio
```

**Private Methods:**
```csharp
private void Update()                         // Verifica proximidade do jogador
private void OnTriggerEnter2D(Collider2D)    // Detecta entrada do jogador
private void OnTriggerExit2D(Collider2D)     // Detecta saída do jogador
```

### 2. DialogueUI Component

**Responsibility:** Gerencia a interface visual do diálogo e a navegação entre textos.

**Public Fields:**
```csharp
[Header("UI References")]
public GameObject dialoguePanel;              // Panel principal do diálogo
public Image backgroundImage;                 // Imagem de fundo
public TextMeshProUGUI dialogueText;         // Texto do diálogo
public GameObject continueIndicator;          // Indicador de "pressione para continuar"

[Header("Settings")]
public int sortingOrder = 100;                // Sorting order do Canvas
```

**Public Methods:**
```csharp
public void Show(List<LocalizedString> texts, UnityEvent onComplete)  // Mostra o diálogo
public void Hide()                                                      // Esconde o diálogo
public void OnContinuePressed()                                        // Chamado quando jogador pressiona continuar
```

**Private Fields:**
```csharp
private List<string> currentTexts;            // Textos atuais (já localizados)
private int currentTextIndex;                 // Índice do texto atual
private TypewriterEffect typewriter;          // Referência ao typewriter
private UnityEvent onDialogueComplete;        // Callback ao completar
```

**Private Methods:**
```csharp
private void ShowNextText()                   // Exibe o próximo texto
private void CompleteDialogue()               // Completa o diálogo
private IEnumerator LoadLocalizedTexts()      // Carrega textos localizados
```

### 3. TypewriterEffect Component

**Responsibility:** Implementa o efeito de digitação letra por letra.

**Public Fields:**
```csharp
[Header("Typewriter Settings")]
public float charactersPerSecond = 30f;       // Velocidade de digitação
public float punctuationDelay = 0.1f;         // Delay extra para pontuação
public bool skipSpaces = true;                // Pular espaços sem delay

[Header("Audio (Optional)")]
public AudioClip typingSound;                 // Som de digitação
public float typingSoundVolume = 0.5f;        // Volume do som
```

**Public Methods:**
```csharp
public void StartTyping(string text, Action onComplete)  // Inicia digitação
public void CompleteInstantly()                          // Completa texto instantaneamente
public bool IsTyping { get; }                            // Verifica se está digitando
```

**Private Fields:**
```csharp
private Coroutine typingCoroutine;            // Coroutine da digitação
private bool isTyping;                        // Flag de estado
private TextMeshProUGUI targetText;           // Referência ao texto
```

**Private Methods:**
```csharp
private IEnumerator TypeText(string text, Action onComplete)  // Coroutine de digitação
private float GetCharacterDelay(char c)                       // Calcula delay por caractere
private void PlayTypingSound()                                // Reproduz som de digitação
```

## Data Models

### DialogueData (Implicit)

O sistema não usa uma classe separada de DialogueData para manter a simplicidade. Os dados são armazenados diretamente no componente DialogueNPC:

```csharp
// Dados do diálogo armazenados no DialogueNPC
public class DialogueNPC : MonoBehaviour
{
    // Lista de textos localizados
    public List<LocalizedString> dialogueTexts;
    
    // Eventos ao completar
    public UnityEvent onDialogueComplete;
    
    // Configurações de interação
    public float interactionRadius;
    public GameObject interactionIndicator;
}
```

### Player Detection

O sistema usa um simples sistema de detecção baseado em:
- **BoxCollider2D** configurado como trigger no NPC
- **Tag "Player"** no GameObject do jogador
- **Cálculo de distância** para mostrar/esconder indicador

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Interaction Radius Consistency

*For any* NPC with a configured interaction radius and any player position, the interaction indicator should be visible if and only if the distance between player and NPC is less than or equal to the interaction radius.

**Validates: Requirements 1.1**

### Property 2: Dialogue Opening Behavior

*For any* NPC with configured dialogue texts, when the StartDialogue() method is called, the dialogue Canvas should become active and the first text should begin displaying.

**Validates: Requirements 1.2**

### Property 3: Player Control State During Dialogue

*For any* active dialogue, the player movement state should be paused or limited while the dialogue Canvas is active.

**Validates: Requirements 1.3**

### Property 4: Dialogue Closing Restores Control

*For any* completed dialogue, after the last text is displayed and the dialogue closes, the player control should be fully restored and the Canvas should be inactive.

**Validates: Requirements 1.4**

### Property 5: Typewriter Sequential Display

*For any* string of text, when the typewriter effect starts, each character should appear in sequence with the configured time interval between them.

**Validates: Requirements 2.1**

### Property 6: Instant Completion During Typing

*For any* text being typed, calling CompleteInstantly() should immediately display the full text and stop the typing animation.

**Validates: Requirements 2.2**

### Property 7: Continue Indicator Visibility

*For any* dialogue text, the continue indicator should be hidden while the typewriter is running and visible when the typewriter completes.

**Validates: Requirements 3.1**

### Property 8: Text Navigation Forward

*For any* dialogue with multiple texts at index i (where i < total texts - 1), pressing continue should advance to text at index i+1.

**Validates: Requirements 3.2**

### Property 9: Current Text Index Invariant

*For any* active dialogue with N texts, the current text index should always be >= 0 and < N.

**Validates: Requirements 3.4**

### Property 10: Setup Idempotence

*For any* GameObject, running the "Setup Dialogue NPC" tool multiple times should result in the same final state without component duplication.

**Validates: Requirements 5.4**

### Property 11: Event Invocation Order

*For any* dialogue with configured events, when the dialogue completes, all events should be invoked in the order they were added before the Canvas closes.

**Validates: Requirements 6.2**

## Error Handling

### Missing References

**Problem:** DialogueUI não encontra referências necessárias (TextMeshPro, Panel, etc.)

**Solution:**
- Validação no Awake() com mensagens de erro claras
- Editor script para validar configuração
- Setup tool cria todas as referências automaticamente

### Empty Dialogue Texts

**Problem:** NPC configurado sem textos de diálogo

**Solution:**
- Validação no StartDialogue() - não abre diálogo se lista vazia
- Warning no console com nome do NPC
- Editor mostra aviso visual se lista está vazia

### Localization Failures

**Problem:** LocalizedString falha ao carregar

**Solution:**
- Usar fallback do Unity Localization System
- Log de warning com chave que falhou
- Continuar com próximo texto se disponível

### Player Reference Lost

**Problem:** Referência ao jogador é perdida

**Solution:**
- Re-buscar jogador por tag quando necessário
- Cache da referência com validação
- Graceful degradation - desabilitar interação se jogador não encontrado

### Multiple Dialogues Simultaneously

**Problem:** Tentar abrir múltiplos diálogos ao mesmo tempo

**Solution:**
- DialogueUI mantém flag isActive
- Ignorar novas requisições se já ativo
- Log de warning informando que diálogo já está ativo

## Testing Strategy

### Unit Tests

**DialogueNPC Tests:**
- Verificar detecção de jogador no raio de interação
- Verificar que StartDialogue() não funciona com lista vazia
- Verificar que indicador aparece/desaparece corretamente

**DialogueUI Tests:**
- Verificar navegação entre textos
- Verificar que Hide() limpa estado corretamente
- Verificar invocação de eventos ao completar

**TypewriterEffect Tests:**
- Verificar que CompleteInstantly() funciona durante digitação
- Verificar cálculo de delay para diferentes caracteres
- Verificar que IsTyping retorna estado correto

### Property-Based Tests

Seguindo a análise de prework, implementaremos property-based tests para as propriedades identificadas usando **NUnit** com geração de dados aleatórios:

**Property Tests a Implementar:**
1. Interaction radius consistency (Property 1)
2. Dialogue opening behavior (Property 2)
3. Player control state (Property 3)
4. Dialogue closing behavior (Property 4)
5. Typewriter sequential display (Property 5)
6. Instant completion (Property 6)
7. Continue indicator visibility (Property 7)
8. Text navigation (Property 8)
9. Index invariant (Property 9)
10. Setup idempotence (Property 10)
11. Event invocation order (Property 11)

**Test Configuration:**
- Mínimo de 100 iterações por property test
- Geração de strings aleatórias para textos
- Geração de posições aleatórias para testes de distância
- Geração de listas de tamanhos variados para navegação

### Integration Tests

**End-to-End Dialogue Flow:**
- Criar NPC em cena de teste
- Simular aproximação do jogador
- Simular pressionar botão de interação
- Simular navegação por todos os textos
- Verificar que eventos são disparados
- Verificar que UI fecha corretamente

**Localization Integration:**
- Testar com múltiplos idiomas
- Verificar fallback quando tradução não existe
- Verificar que mudança de idioma atualiza textos

### Editor Tool Tests

**Setup Dialogue NPC Tool:**
- Verificar criação de componentes
- Verificar configuração de collider
- Verificar criação/referência de Canvas
- Verificar idempotência (executar múltiplas vezes)

## Implementation Notes

### Princípio KISS Aplicado

1. **Apenas 3 Componentes:** DialogueNPC, DialogueUI, TypewriterEffect
2. **Sem Abstrações Desnecessárias:** Não usar interfaces, classes abstratas ou padrões complexos
3. **Dados Inline:** Não criar ScriptableObjects ou classes de dados separadas
4. **Dependências Mínimas:** Usar apenas Unity Localization e TextMeshPro (já no projeto)

### Performance Considerations

- **Object Pooling:** Não necessário - apenas um Canvas de diálogo ativo por vez
- **Coroutines:** Usar para typewriter effect - simples e eficiente
- **Caching:** Cache referência do jogador após primeira busca
- **Update Loop:** Apenas DialogueNPC usa Update() para verificar distância

### Unity Localization Integration

```csharp
// Exemplo de uso de LocalizedString
public LocalizedString dialogueText;

// Carregar texto localizado
string localizedText = await dialogueText.GetLocalizedStringAsync().Task;

// Ou usar operação síncrona se necessário
string localizedText = dialogueText.GetLocalizedString();
```

### Input System Integration

O sistema deve funcionar com ambos Input Systems:
- **Old Input System:** Input.GetButtonDown("Interact")
- **New Input System:** InputAction callback

Implementação sugerida:
```csharp
// Usar UnityEvent para desacoplar do input system
public UnityEvent onInteractPressed;

// No PlayerInput ou InputManager
void Update()
{
    if (Input.GetButtonDown("Interact"))
        onInteractPressed?.Invoke();
}
```

### Canvas Setup

**Configuração Recomendada:**
- **Render Mode:** Screen Space - Overlay
- **Sorting Order:** 100 (acima de outros UI)
- **Canvas Scaler:** Scale With Screen Size
- **Reference Resolution:** 1920x1080
- **Match:** 0.5 (balance entre width e height)

**Background Image:**
- **Sprite:** ui_dialogBackground.png (já existe no projeto)
- **Image Type:** Sliced (para 9-slice scaling)
- **Configurar 9-slice borders** no import settings do sprite se necessário

### Prefab Structure

```
DialogueCanvas (Canvas)
├── DialogueUI (Component)
├── DialoguePanel (Image)
│   ├── BackgroundImage (Image)
│   ├── DialogueText (TextMeshProUGUI)
│   │   └── TypewriterEffect (Component)
│   └── ContinueIndicator (Image/GameObject)
│       └── Animation (Animator - opcional)
```

## Migration from Old System

### Step 1: Identify Old Components

Buscar na cena e no projeto:
- NPCDialogueController
- Qualquer script relacionado ao sistema antigo

### Step 2: Create Migration Tool

Editor script que:
1. Encontra todos GameObjects com componentes antigos
2. Extrai dados relevantes (textos, configurações)
3. Remove componentes antigos
4. Adiciona novos componentes
5. Migra dados quando possível
6. Gera relatório de migração

### Step 3: Manual Cleanup

- Deletar scripts antigos do projeto
- Remover referências quebradas
- Atualizar documentação

### Step 4: Validation

- Testar todos NPCs migrados
- Verificar que não há erros no console
- Confirmar que diálogos funcionam corretamente

## Future Extensibility

O sistema está preparado para futuras extensões através de UnityEvents:

### Quest System Integration
```csharp
// No Inspector do DialogueNPC
onDialogueComplete.AddListener(() => {
    QuestManager.Instance.StartQuest("quest_id");
});
```

### Cutscene Trigger
```csharp
onDialogueComplete.AddListener(() => {
    CutsceneManager.Instance.PlayCutscene("cutscene_name");
});
```

### Item Delivery
```csharp
onDialogueComplete.AddListener(() => {
    Inventory.Instance.AddItem("item_id", 1);
});
```

### Dialogue Choices (Future)

Se no futuro precisarmos de escolhas de diálogo:
1. Adicionar classe DialogueChoice com texto e UnityEvent
2. Modificar DialogueUI para mostrar botões de escolha
3. Manter simplicidade - apenas adicionar o necessário

## File Structure

```
Assets/
├── Code/
│   ├── Dialogue/
│   │   ├── DialogueNPC.cs
│   │   ├── DialogueUI.cs
│   │   └── TypewriterEffect.cs
│   └── Editor/
│       └── ExtraTools/
│           └── DialogueSetupTool.cs (adicionar ao UnifiedExtraTools.cs)
├── Game/
│   └── Prefabs/
│       └── UI/
│           └── DialogueCanvas.prefab
└── Art/
    └── UI/
        └── ui_dialogBackground.png (já existe - será configurado)
```

## Setup Tool Implementation

### Menu Item

Adicionar ao UnifiedExtraTools.cs:
```csharp
[MenuItem("Extra Tools/Setup/💬 Setup Dialogue NPC")]
public static void MenuSetupDialogueNPC()
{
    if (Selection.activeGameObject == null)
    {
        Debug.LogWarning("Nenhum GameObject selecionado.");
        return;
    }
    
    DialogueSetupTool.SetupDialogueNPC(Selection.activeGameObject);
}
```

### Setup Logic

1. Adicionar DialogueNPC component se não existir
2. Configurar BoxCollider2D como trigger
3. Buscar ou criar DialogueCanvas na cena
4. Configurar background image com ui_dialogBackground.png
5. Configurar referências
6. Adicionar entrada de localização padrão
7. Log de sucesso

## Documentation Requirements

### Code Comments

- XML documentation em todos os métodos públicos
- Comentários inline para lógica complexa
- Exemplos de uso em headers de classes

### README

Criar `Assets/Code/Dialogue/README.md` com:
- Overview do sistema
- Quick start guide
- Exemplos práticos
- Troubleshooting comum
- Links para documentação Unity Localization

### Inspector Tooltips

Adicionar tooltips em todos os campos públicos:
```csharp
[Tooltip("Lista de textos localizados que serão exibidos em sequência")]
public List<LocalizedString> dialogueTexts;
```
