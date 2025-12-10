# Atualização de Controles - GDD v10.1

**Data:** 10 de Dezembro de 2025  
**Versão:** 10.1.1  
**Objetivo:** Sincronizar mapeamentos de controles do GDD com InputSystem_Actions configurado

## Mudanças Realizadas

### Seção 16.1 - Mapeamento de Controles (Gameplay)

#### Teclado

| Ação | Antes | Depois | Motivo |
|------|-------|--------|--------|
| **Attack/Confirm** | Mouse Esquerdo ou Espaço | **Espaço** | Confirmado em InputSystem_Actions |
| **Interact/Cancel** | E ou Mouse Direito | **E** | Mouse removido do InputSystem |
| **Open/Close Inventory** | I | **I** | Confirmado (sem mudanças) |

#### Gamepad (Xbox)

| Ação | Antes | Depois | Motivo |
|------|-------|--------|--------|
| **Attack/Confirm** | A | **B (East)** | Confirmado como buttonEast |
| **Crouch/Discard Item** | X | **X (West)** | Confirmado como buttonWest |
| **Interact/Cancel** | B | **A (South)** | Confirmado como buttonSouth |
| **Open Inventory** | Y ou View | **Select** | Confirmado como select button |
| **Pause Game** | Menu | **Start** | Confirmado como start button |

**Nota:** Nomes descritivos (West/East/South/North) foram adicionados para clareza, já que os buttons variam entre plataformas.

### Seção 16.1 - Inventory Navigation

#### Teclado

| Ação | Antes | Depois | Motivo |
|------|-------|--------|--------|
| **Select Item** | Enter ou Mouse Esquerdo | **Enter ou Espaço** | Mouse removido, Espaço adicionado |
| **Close Inventory** | I | **I ou Esc** | Ambas opções agora documentadas |

#### Gamepad (Xbox)

| Ação | Antes | Depois | Motivo |
|------|-------|--------|--------|
| **Select Item** | A | **B (East)** | Alinhado com mapeamento Gameplay |

### Seção 16.2 - Menus (Corrigido de 13.2)

#### Número de Seção

- **Antes:** 13.2 HUD
- **Depois:** 16.2 HUD (alinhado com seção 16 - Controles e Interface)

#### Teclado

| Ação | Antes | Depois | Motivo |
|------|-------|--------|--------|
| **Confirm/Forward** | A | **B ou Espaço** | Alinhado com padrão de Confirm |
| **Back/Return** | B | **A ou Esc** | Alinhado com padrão de Back |

#### Gamepad (Xbox)

| Ação | Antes | Depois | Motivo |
|------|-------|--------|--------|
| **Confirm/Forward** | A | **B (East)** | Nomes descritivos adicionados |
| **Back/Return to Gameplay** | B | **A (South)** | Nomes descritivos adicionados |
| **Return to Gameplay (Menu Button)** | Menu | **Start** | Confirmado como start |

### HUD (Seção 16.2)

- Adicionado elemento "Quick Slots (4 slots)" à lista de elementos mínimos

## Fontes de Verdade

**InputSystem_Actions.inputactions** (configurado):

- Move: Gamepad leftStick + dpad, Keyboard WASD + Arrows
- Attack: Keyboard Space, Gamepad buttonEast
- Crouch: Keyboard X, Gamepad buttonWest
- Interact: Keyboard E, Gamepad buttonSouth
- OpenInventory: Keyboard I, Gamepad Select
- UseItem1-4: Keyboard 1-4, Gamepad LB/LT/RB/RT
- PauseGame: Keyboard Escape, Gamepad Start

## Impacto

✅ GDD agora sincronizado com implementação real  
✅ Documentação mais clara com nomenclatura descritiva (West/East/South/North)  
✅ Nenhum botão "fictício" (como Mouse Direito ou Menu genérico)  
✅ Seção de Menu agora numerada corretamente (16.2)  
✅ Quick Slots mencionados no HUD

## Próximos Passos

1. ✅ Validar se PlayerController.cs usa os mapeamentos corretos
2. ⚠️ Validar se InventoryUI.cs usa os mapeamentos corretos
3. ⚠️ Testar todos os mapeamentos em-game
4. ⚠️ Atualizar documentação de onboarding (se houver)

---

**Versão Anterior:** GDD v10.1  
**Versão Atual:** GDD v10.1.1 (atualização de Controles)
