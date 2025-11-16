# Pause Menu Setup Guide

Este guia explica como configurar o PauseMenu no Unity Editor.

## Estrutura de UI Necessária

### 1. Criar Canvas Principal (se não existir)

1. Hierarchy → Right Click → UI → Canvas
2. Renomear para "GameUI"
3. Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080

### 2. Criar Pause Menu Panel

1. Dentro do Canvas, criar: Right Click → UI → Panel
2. Renomear para "PauseMenuPanel"
3. Configurar RectTransform:
   - Anchor: Stretch (preencher tela toda)
   - Left: 0, Top: 0, Right: 0, Bottom: 0
4. Configurar Image:
   - Color: Semi-transparente (ex: R:0, G:0, B:0, A:200)

### 3. Criar Container de Botões

1. Dentro do PauseMenuPanel, criar: Right Click → UI → Vertical Layout Group
2. Renomear para "ButtonContainer"
3. Configurar RectTransform:
   - Anchor: Center
   - Width: 400, Height: 300
4. Configurar Vertical Layout Group:
   - Spacing: 20
   - Child Alignment: Middle Center
   - Child Force Expand: Width e Height

### 4. Criar Botões

Dentro do ButtonContainer, criar 3 botões:

#### Botão Inventário

1. Right Click → UI → Button - TextMeshPro
2. Renomear para "InventoryButton"
3. Texto: "Inventário"
4. Height: 60

#### Botão Continuar

1. Right Click → UI → Button - TextMeshPro
2. Renomear para "ResumeButton"
3. Texto: "Continuar"
4. Height: 60

#### Botão Sair

1. Right Click → UI → Button - TextMeshPro
2. Renomear para "QuitButton"
3. Texto: "Sair"
4. Height: 60

### 5. Adicionar PauseMenu Component

1. Selecionar o Canvas (ou criar um GameObject vazio "PauseMenuManager")
2. Add Component → PauseMenu
3. Configurar referências:
   - **Pause Menu Panel**: Arrastar PauseMenuPanel
   - **Inventory UI**: Arrastar o GameObject que contém InventoryUI
   - **Inventory Button**: Arrastar InventoryButton
   - **Resume Button**: Arrastar ResumeButton
   - **Quit Button**: Arrastar QuitButton

### 6. Configurar Input Actions (Opcional)

Se estiver usando o novo Input System:

1. Criar ou abrir o Input Actions asset
2. Adicionar ações:
   - **Pause**: Keyboard Escape, Keyboard Tab, Gamepad Start
   - **Cancel**: Keyboard Escape, Gamepad B/Circle

3. No PauseMenu component:
   - **Pause Action**: Selecionar a ação "Pause"
   - **Cancel Action**: Selecionar a ação "Cancel"

### 7. Configurar InventoryUI

1. Selecionar o GameObject com InventoryUI
2. Verificar se tem referência ao PauseMenu (será configurado automaticamente)

## Controles

### Teclado

- **Escape** ou **Tab**: Abre/fecha o menu de pausa
- **Escape** (quando inventário aberto): Volta ao menu de pausa

### Gamepad

- **Start/Menu**: Abre/fecha o menu de pausa
- **B/Circle** (quando inventário aberto): Volta ao menu de pausa

## Fluxo de Navegação

```
Gameplay
   ↓ (Esc/Tab/Start)
Menu de Pausa
   ├─ Inventário → Abre InventoryUI
   │     ↓ (Esc/B)
   │  Menu de Pausa (volta)
   ├─ Continuar → Volta ao gameplay
   └─ Sair → Menu principal (a implementar)
```

## Troubleshooting

### Menu não abre

- Verificar se PauseMenuPanel está atribuído
- Verificar se o Canvas está ativo na cena
- Verificar se há EventSystem na cena

### Inventário não abre

- Verificar se InventoryUI está atribuído no PauseMenu
- Verificar se InventoryUI.Show() está funcionando isoladamente

### Botão de voltar não funciona

- Verificar se Cancel Action está configurado
- Verificar se Input System está instalado (ou usar fallback de Input.GetKeyDown)

### Jogo não despausa

- Verificar se Time.timeScale está sendo resetado para 1f
- Verificar se não há múltiplos scripts controlando Time.timeScale

## Integração com Save System

Quando implementar o save system (Task 8), o PauseMenu pode ser expandido para incluir:

- Botão "Salvar Jogo"
- Botão "Carregar Jogo"
- Indicador de último save

## Próximos Passos

Após configurar o PauseMenu:

1. Testar abertura/fechamento do menu
2. Testar navegação para o inventário
3. Testar botão de voltar do inventário
4. Implementar Task 7 (Sistema de Notificações)
5. Implementar Task 8 (Integração com Save System)
