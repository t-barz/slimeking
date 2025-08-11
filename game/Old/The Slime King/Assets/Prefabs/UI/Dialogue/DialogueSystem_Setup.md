# Prefab DialogueSystem

Este arquivo descreve como configurar o prefab do sistema de diálogo no Unity.

## Hierarquia do Prefab

```
DialogueSystem
├── DialogueManager
├── DialogueCanvas (Canvas)
│   ├── DialoguePanel (Panel)
│   │   ├── Background (Image)
│   │   ├── NamePanel (Panel)
│   │   │   └── NameText (TMP_Text)
│   │   ├── SpeakerImage (Image)
│   │   ├── DialogueText (TMP_Text)
│   │   └── ContinueIndicator (Image)
│   └── ChoicesPanel (Panel)
│       └── ChoiceButtonContainer (LayoutGroup)
│           └── ChoiceButton (Button) [Template]
└── DialogueInputController
```

## Configuração dos Componentes

### DialogueManager
- Adicionar o script `DialogueManager.cs`
- Configurar referências para:
  - `_dialoguePanel` → DialoguePanel
  - `_dialogueText` → DialogueText
  - `_nameText` → NameText
  - `_speakerImage` → SpeakerImage
  - `_continueIndicator` → ContinueIndicator
- Configurar valores para:
  - `_charactersPerSecond` → 30
  - `_commaDelay` → 0.1
  - `_periodDelay` → 0.3
  - `_typingSoundVolume` → 0.2

### DialogueCanvas
- Canvas com `renderMode` → Screen Space - Camera
- Canvas Scaler com `UI Scale Mode` → Scale With Screen Size
- Referência de `Canvas Camera` → Camera principal
- Sort Order → 100 (para ficar acima da maioria dos elementos)

### DialoguePanel
- Imagem com Alpha ~0.8 para fundos
- Ancorado na parte inferior central da tela
- Altura de aproximadamente 1/4 da tela

### NamePanel
- Posicionado no canto superior esquerdo do DialoguePanel
- Cor distinta para destacar do painel principal

### DialogueText
- TMP_Text com wrapping ativado
- Tamanho de fonte apropriado (22-26)
- Margem interna para não encostar nas bordas do painel

### ContinueIndicator
- Imagem de seta ou ícone animado
- Posicionado no canto inferior direito do DialoguePanel

### ChoicesPanel
- Inicialmente desativado
- Layout vertical para botões de escolha
- Posicionado acima do DialoguePanel

### ChoiceButton (Template)
- Botão com texto TMP
- Cores distintas para estados normal/hover/pressed

### DialogueInputController
- Adicionar o script `DialogueInputController.cs`
- Configurar referência para `_dialogueManager`

## Scripts Adicionais

### DialogueUI
- Adicionar ao DialogueCanvas
- Configurar referências para:
  - `_dialogueCanvasGroup` → CanvasGroup do DialogueCanvas
  - `_dialoguePanel` → RectTransform do DialoguePanel
  - `_namePanel` → RectTransform do NamePanel
  - `_continueIndicator` → RectTransform do ContinueIndicator
- Configurar valores para animações

### DialogueSequenceManager
- Adicionar ao GameObject DialogueSystem
- Configurar referências para:
  - `_choicesPanel` → ChoicesPanel
  - `_choiceButtonPrefab` → ChoiceButton
