# Inventory UI Setup Guide

## 📋 Visão Geral

Este guia explica como configurar a interface do inventário com 12 slots (3 linhas x 4 colunas) centralizada na tela.

## 🎯 Estrutura da UI

```
InventoryCanvas (Canvas)
└── InventoryPanel (Panel)
    ├── Background (Image - painel marrom)
    ├── Title (TextMeshPro - "INVENTÁRIO")
    ├── CloseButton (Button - X no canto superior direito)
    └── SlotsContainer (Grid Layout Group)
        ├── Slot_0 (Image - slot vazio)
        ├── Slot_1 (Image - slot vazio)
        ├── Slot_2 (Image - slot vazio)
        ├── Slot_3 (Image - slot vazio)
        ├── Slot_4 (Image - slot vazio)
        ├── Slot_5 (Image - slot vazio)
        ├── Slot_6 (Image - slot vazio)
        ├── Slot_7 (Image - slot vazio)
        ├── Slot_8 (Image - slot vazio)
        ├── Slot_9 (Image - slot vazio)
        ├── Slot_10 (Image - slot vazio)
        └── Slot_11 (Image - slot vazio)
```

## 🛠️ Passo a Passo

### 1. Criar Canvas do Inventário

1. **Hierarchy** → Right Click → **UI → Canvas**
2. Rename: `InventoryCanvas`
3. Configure:
   - **Render Mode**: Screen Space - Overlay
   - **Canvas Scaler**:
     - UI Scale Mode: Scale With Screen Size
     - Reference Resolution: 1920 x 1080
     - Match: 0.5 (Width/Height)
   - **Sorting Layer**: UI
   - **Order in Layer**: 10 (acima do PauseCanvas)

### 2. Adicionar CanvasGroup ao Canvas

1. Select `InventoryCanvas`
2. **Add Component** → **Canvas Group**
3. Configure:
   - Alpha: 1
   - Interactable: ✓
   - Block Raycasts: ✓

### 3. Criar Panel Principal

1. Right Click `InventoryCanvas` → **UI → Panel**
2. Rename: `InventoryPanel`
3. Configure **RectTransform**:
   - Anchors: Center-Middle
   - Pivot: (0.5, 0.5)
   - Width: 600
   - Height: 450
   - Pos X: 0
   - Pos Y: 0

4. Configure **Image** (Background):
   - Color: Marrom escuro (#5C4033 ou similar)
   - Material: None
   - Raycast Target: ✓

### 4. Criar Título

1. Right Click `InventoryPanel` → **UI → Text - TextMeshPro**
2. Rename: `Title`
3. Configure **RectTransform**:
   - Anchors: Top-Center
   - Pivot: (0.5, 1)
   - Width: 500
   - Height: 60
   - Pos X: 0
   - Pos Y: -10

4. Configure **TextMeshPro**:
   - Text: "INVENTÁRIO"
   - Font Size: 36
   - Alignment: Center-Middle
   - Color: Branco (#FFFFFF)
   - Font Style: Bold

### 5. Criar Botão de Fechar

1. Right Click `InventoryPanel` → **UI → Button - TextMeshPro**
2. Rename: `CloseButton`
3. Configure **RectTransform**:
   - Anchors: Top-Right
   - Pivot: (1, 1)
   - Width: 40
   - Height: 40
   - Pos X: -10
   - Pos Y: -10

4. Configure **Button**:
   - Interactable: ✓
   - Transition: Color Tint
   - Normal Color: Vermelho (#FF4444)
   - Highlighted Color: Vermelho claro (#FF6666)
   - Pressed Color: Vermelho escuro (#CC0000)

5. Configure **Text** (child):
   - Text: "X"
   - Font Size: 24
   - Alignment: Center-Middle
   - Color: Branco

### 6. Criar Container dos Slots

1. Right Click `InventoryPanel` → **UI → Empty** (Create Empty)
2. Rename: `SlotsContainer`
3. Configure **RectTransform**:
   - Anchors: Center-Middle
   - Pivot: (0.5, 0.5)
   - Width: 520
   - Height: 330
   - Pos X: 0
   - Pos Y: -30

4. **Add Component** → **Grid Layout Group**
5. Configure **Grid Layout Group**:
   - Cell Size: (120, 100)
   - Spacing: (10, 10)
   - Start Corner: Upper Left
   - Start Axis: Horizontal
   - Child Alignment: Middle Center
   - Constraint: Fixed Column Count
   - Constraint Count: 4

### 7. Criar Slots (12x)

Para cada slot (0 a 11):

1. Right Click `SlotsContainer` → **UI → Image**
2. Rename: `Slot_0` (incrementar número)
3. Configure **Image**:
   - Color: Marrom médio (#8B6F47 ou similar)
   - Material: None
   - Raycast Target: ✓

4. **Add Component** → **Outline** (opcional, para borda)
   - Effect Color: Branco (#FFFFFF)
   - Effect Distance: (2, -2)

**Dica**: Após criar o primeiro slot, duplique-o 11 vezes (Ctrl+D) e renomeie.

### 8. Adicionar Script InventoryUI

1. Select `InventoryCanvas`
2. **Add Component** → **Inventory UI** (script)
3. Configure:
   - **Inventory Panel**: Arraste `InventoryPanel`
   - **Canvas Group**: Arraste `InventoryCanvas` (ou será auto-detectado)
   - **Fade Duration**: 0.3
   - **Can Open With Input**: ✓
   - **Enable Logs**: ✓ (para debug)

### 9. Configurar Botão de Fechar

1. Select `CloseButton`
2. No Inspector, na seção **Button → On Click()**:
   - Click no **+**
   - Arraste `InventoryCanvas` para o campo de objeto
   - Selecione: **InventoryUI → CloseInventory()**

### 10. Adicionar Input Action (Opcional)

Se quiser abrir o inventário com uma tecla:

1. Abra `Assets/Settings/InputSystem_Actions.inputactions`
2. No Action Map **Gameplay**, adicione:
   - **Name**: Inventory
   - **Action Type**: Button
   - **Binding**: Keyboard → I (ou outra tecla)
   - **Binding**: Gamepad → Y Button (ou outro botão)

3. Salve e regenere o código (se necessário)

## 🎨 Customização Visual

### Cores Sugeridas (baseado na imagem)

- **Background Panel**: #5C4033 (marrom escuro)
- **Slots**: #8B6F47 (marrom médio)
- **Slot Border**: #FFFFFF (branco)
- **Title**: #FFFFFF (branco)
- **Close Button**: #FF4444 (vermelho)

### Sprites Customizados

Para usar sprites customizados ao invés de cores sólidas:

1. Importe sprites de UI (painel, slots, etc.)
2. Configure como **Sprite (2D and UI)**
3. No componente **Image**, arraste o sprite para **Source Image**
4. Ajuste **Image Type** conforme necessário:
   - **Simple**: Para sprites simples
   - **Sliced**: Para sprites com 9-slice (bordas)

## 🔧 Integração com PauseMenu

O PauseMenu já está configurado para abrir o InventoryUI quando o botão "Inventário" é pressionado.

### Fluxo:
1. Player pressiona Menu (Esc/Tab/Start)
2. PauseMenu abre
3. Player seleciona "Inventário"
4. PauseMenu fecha
5. InventoryUI abre (e pausa novamente)

## 📝 Notas Importantes

- O inventário pausa o jogo automaticamente quando aberto
- O inventário pode ser aberto via:
  - Botão no PauseMenu
  - Input direto (tecla I ou botão Y do gamepad)
- O inventário fecha ao pressionar o botão X ou o input novamente
- A funcionalidade de adicionar/remover itens será implementada posteriormente

## 🐛 Troubleshooting

**Problema**: Inventário não abre
- **Solução**: Verifique se o script InventoryUI está no InventoryCanvas
- **Solução**: Verifique se o Input Action "Inventory" existe no InputSystem_Actions

**Problema**: Botão de fechar não funciona
- **Solução**: Verifique se o evento OnClick está configurado corretamente

**Problema**: Slots não aparecem em grid
- **Solução**: Verifique se o Grid Layout Group está configurado corretamente
- **Solução**: Verifique se os slots são filhos diretos do SlotsContainer

**Problema**: UI não aparece centralizada
- **Solução**: Verifique os anchors e pivot do InventoryPanel
- **Solução**: Verifique se o Canvas Scaler está configurado corretamente
