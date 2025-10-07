# Sistema de UI/HUD - Implementação Alpha

## 📋 Status

- **HUD Manager:** 🔜 Não iniciado
- **Event System UI Navigation:** 🔜 Não iniciado
- **Dialogue System:** 🔜 Não iniciado

## 🎯 Objetivo

Criar interface unificada que mostra vida, slots de inventário e navegação por teclado/gamepad, sem modificar código existente.

## 🔧 Implementação

### Scripts Necessários (todos novos na Alpha/)

#### 1. AlphaHUDManager.cs (NOVO)

```csharp
// HUD centralizado: vida + slots inventário + progression info
// Integra com PlayerAttributesSystem e InventoryCore via eventos
```

#### 2. AlphaUINavigation.cs (NOVO)  

```csharp
// Configura EventSystem + InputSystemUIInputModule
// Gerencia navegação UI com teclado/gamepad
```

#### 3. DialogueController.cs (NOVO)

```csharp
// Sistema mínimo de diálogo para Alpha
// Bloqueia input de gameplay durante conversa
```

#### 4. AlphaUISetup.cs (NOVO)

```csharp
// Setup automático de toda UI da Alpha
// Configura Canvas, EventSystem, etc.
```

### Fluxo de Integração

1. **Health Display**

   ```
   PlayerAttributesSystem.OnHealthChanged →
   AlphaHUDManager.UpdateHealthBar() →
   UI atualizada
   ```

2. **Inventory Slots**

   ```
   InventoryCore.OnConsumableSlotChanged →
   AlphaHUDManager.UpdateInventorySlot() →
   Sprite e quantidade atualizados
   ```

3. **Dialogue Flow**

   ```
   Trigger/NPC interaction →
   DialogueController.StartDialogue() →
   Input gameplay bloqueado →
   Submit avança texto →
   Fim: input gameplay retorna
   ```

## 📝 TODOs Específicos

### AlphaHUDManager.cs (criar novo)

- [ ] Health bar/text display
- [ ] 4 slots de inventário com sprites
- [ ] Progression info (stage + skills ativas)
- [ ] Subscribe a eventos dos sistemas Alpha

### AlphaUINavigation.cs (criar novo)

- [ ] Configurar EventSystem se não existir
- [ ] Setup InputSystemUIInputModule
- [ ] Navigation highlighting para elementos UI
- [ ] Integração com Input Actions (Navigate, Submit, Cancel)

### DialogueController.cs (criar novo)

- [ ] Panel com texto simples
- [ ] Array de strings para diálogo
- [ ] Avanço com Submit action
- [ ] Bloqueio de input via InputManager ou PlayerController events

### AlphaUISetup.cs (criar novo)

- [ ] Auto-criação de Canvas se não existir
- [ ] Configuração de EventSystem
- [ ] Setup de prefabs HUD
- [ ] Integração automática com sistemas Alpha

## 🔗 Pontos de Integração

### Com PlayerAttributesSystem (NÃO MODIFICAR)

- Subscribe OnHealthChanged, OnMaxHealthChanged
- Display valores atuais na HUD

### Com Input System (USAR EXISTENTE)

- Navigate action para UI navigation
- Submit/Cancel para diálogos
- Não criar novos Input Actions

### Com InventoryCore (Alpha Integration)

- Subscribe eventos de mudança de slots
- Display ícones e quantidades

### Com InputManager (NÃO MODIFICAR)

- Usar eventos para detectar quando bloquear/desbloquear input

## ⚙️ Configuração na Cena

### Setup Automático via Extra Tools > Alpha

1. Cria Canvas principal se não existir
2. Configura EventSystem + InputSystemUIInputModule
3. Instancia AlphaHUD prefab
4. Setup DialoguePanel prefab
5. Conecta todos os eventos automaticamente

### Prefabs Necessários

- **AlphaHUD.prefab:** Health bar + 4 inventory slots + progression info
- **DialoguePanel.prefab:** Background + Text + Continue indicator

## 🧪 Teste de Validação

1. **Health Display:** Dano ao player → barra diminui
2. **Inventory Slots:** Coletar item → aparece no slot
3. **UI Navigation:** Tab/D-Pad navega elementos
4. **Dialogue:** Trigger → abre dialogue → Submit avança → fecha
5. **Input Block:** Durante dialogue, movimento player não funciona

## 📊 MVP para Alpha

### HUD Elements

- Health: barra simples ou "HP: 80/100"
- Inventory: 4 slots com ícones (ou placeholder se vazio)
- Progression: "Stage: Adulto" + "Skills: 2 active"

### Dialogue

- Panel simples com texto
- Array de strings pré-definidas
- Indicator visual "Press Enter to continue"
- Auto-close no final

### Navigation

- Tab/Arrow keys navegam entre elementos
- Enter/Space ativam botões
- Escape fecha panels

## 📊 Métricas de Sucesso

- [ ] Health atualizada em tempo real
- [ ] Inventory slots mostram itens coletados
- [ ] Navigation funciona com teclado e gamepad
- [ ] Dialogue bloqueia input de gameplay
- [ ] UI responsiva em diferentes resoluções
- [ ] Zero modificações no código existente
