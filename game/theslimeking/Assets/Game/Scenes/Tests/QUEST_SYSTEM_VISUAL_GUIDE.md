# Quest System Test Scene - Visual Guide

## 🎬 Scene Layout

```
                    [Directional Light]
                           ☀️
                           
                           
        [NPC_QuestGiver]
             🟡 !          ← Yellow indicator (quest available)
             👤            ← Yellow capsule NPC
             
             
             
        [Player]
             🎮            ← Blue capsule player
             📦            ← Has InventoryManager
             
             
             
    ═══════════════════════════════════
    ║                                 ║
    ║         [Ground]                ║
    ║      Green Plane 5x5            ║
    ║                                 ║
    ═══════════════════════════════════
```

---

## 🎯 Quest Flow Visualization

### Step 1: Quest Available

```
    NPC_QuestGiver
         🟡 !
         👤
         
    Status: Quest available
    Indicator: Yellow exclamation mark
    Action: Press E to interact
```

### Step 2: Accept Quest

```
    📋 Dialogue Box
    ┌─────────────────────────┐
    │ "Aceitar Quest"         │
    │ [Select this option]    │
    └─────────────────────────┘
    
    Result: Quest added to active list
    Notification: "Quest Aceita: Coletar Flores"
```

### Step 3: Collect Items

```
    📦 Inventory
    ┌─────────────────────────┐
    │ Slot 1: Frutas de Cura  │
    │ Quantity: 1/3           │
    └─────────────────────────┘
    
    Progress: 1/3 → 2/3 → 3/3
    Notification: Progress updates
```

### Step 4: Quest Ready

```
    NPC_QuestGiver
         🟠 !
         👤
         
    Status: Quest ready to turn in
    Indicator: Golden exclamation mark
    Action: Press E to turn in
```

### Step 5: Turn In Quest

```
    📋 Dialogue Box
    ┌─────────────────────────┐
    │ "Entregar Quest"        │
    │ [Select this option]    │
    └─────────────────────────┘
    
    Result: Quest completed
    Notification: "Quest Completada: Coletar Flores"
                  "Recompensas: Cristal Elemental x2"
```

### Step 6: Repeatable

```
    NPC_QuestGiver
         🟡 !
         👤
         
    Status: Quest available again
    Indicator: Yellow exclamation mark
    Action: Can accept again
```

---

## 🎨 Color Coding

### NPCs

- 🟡 **Yellow Capsule** = Quest Giver NPC

### Player

- 🔵 **Blue Capsule** = Player character

### Environment

- 🟢 **Green Plane** = Ground/walkable area

### Indicators

- 🟡 **Yellow !** = Quest available
- 🟠 **Golden !** = Quest ready to turn in

---

## 📊 UI Elements

### Quest Notification Panel

```
┌─────────────────────────────────┐
│  Quest Notification             │
│                                 │
│  Quest Aceita: Coletar Flores   │
│                                 │
└─────────────────────────────────┘

Position: Top-center of screen
Duration: 3 seconds
Sound: Quest accepted sound
```

### Progress Notification

```
┌─────────────────────────────────┐
│  Quest Progress                 │
│                                 │
│  Coletar Flores: 2/3            │
│                                 │
└─────────────────────────────────┘

Position: Top-center of screen
Duration: 3 seconds
Sound: Progress update sound
```

### Completion Notification

```
┌─────────────────────────────────┐
│  Quest Completada               │
│                                 │
│  Coletar Flores                 │
│  Recompensas:                   │
│  - Cristal Elemental x2         │
│                                 │
└─────────────────────────────────┘

Position: Top-center of screen
Duration: 3 seconds
Sound: Quest completed sound
```

---

## 🎮 Controls Visualization

```
    Keyboard Layout
    
    ┌───┐
    │ W │  ← Move Forward
    └───┘
┌───┬───┬───┐
│ A │ S │ D │  ← Move Left/Back/Right
└───┴───┴───┘

    ┌───┐
    │ E │  ← Interact with NPC
    └───┘
```

---

## 📦 Inventory Visualization

### Before Quest Turn-In

```
Inventory Slots:
┌─────┬─────┬─────┬─────┐
│  🌸 │     │     │     │
│  3  │     │     │     │
└─────┴─────┴─────┴─────┘
  Frutas de Cura (3x)
```

### After Quest Turn-In

```
Inventory Slots:
┌─────┬─────┬─────┬─────┐
│  💎 │     │     │     │
│  2  │     │     │     │
└─────┴─────┴─────┴─────┘
  Cristal Elemental (2x)
  
Items Removed: Frutas de Cura (3x)
Items Added: Cristal Elemental (2x)
Reputation: +10
```

---

## 🔍 Inspector Views

### QuestManager Inspector

```
┌─────────────────────────────────┐
│ QuestManager                    │
├─────────────────────────────────┤
│ Debug                           │
│ ☑ Enable Debug Logs             │
│ ☑ Show Gizmos                   │
├─────────────────────────────────┤
│ Active Quests (Runtime)         │
│ • Coletar Flores (2/3)          │
├─────────────────────────────────┤
│ Completed Quests (Runtime)      │
│ • (none)                        │
├─────────────────────────────────┤
│ Debug Tools                     │
│ [Force Complete Quest]          │
│ [Reset Quest]                   │
│ [Clear All Quests]              │
└─────────────────────────────────┘
```

### QuestGiverController Inspector

```
┌─────────────────────────────────┐
│ QuestGiverController            │
├─────────────────────────────────┤
│ Quest Configuration             │
│ Available Quests (1)            │
│ • TestQuest_CollectFlowers      │
├─────────────────────────────────┤
│ Visual Indicators               │
│ Quest Available Indicator       │
│ • QuestIndicatorAvailable       │
│ Quest Ready Indicator           │
│ • QuestIndicatorReady           │
├─────────────────────────────────┤
│ Debug                           │
│ ☐ Enable Debug Logs             │
│ ☑ Show Gizmos                   │
└─────────────────────────────────┘
```

### Player Inspector

```
┌─────────────────────────────────┐
│ Player                          │
├─────────────────────────────────┤
│ Tag: Player                     │
├─────────────────────────────────┤
│ InventoryManager                │
│ Slots (20)                      │
│ • Slot 0: Frutas de Cura (3)    │
│ • Slot 1: Empty                 │
│ • ...                           │
├─────────────────────────────────┤
│ SimplePlayerMovement            │
│ Move Speed: 5                   │
│ Rotation Speed: 720             │
└─────────────────────────────────┘
```

---

## 🎯 State Diagram

```
┌─────────────────┐
│  Quest Created  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   Available     │ ← Yellow ! indicator
│  (Not Accepted) │
└────────┬────────┘
         │ Press E + Accept
         ▼
┌─────────────────┐
│     Active      │ ← No indicator
│   (Progress)    │
└────────┬────────┘
         │ Collect items
         ▼
┌─────────────────┐
│  Ready to Turn  │ ← Golden ! indicator
│      In         │
└────────┬────────┘
         │ Press E + Turn In
         ▼
┌─────────────────┐
│   Completed     │
└────────┬────────┘
         │ If Repeatable
         ▼
┌─────────────────┐
│   Available     │ ← Yellow ! indicator
│     Again       │
└─────────────────┘
```

---

## 📈 Progress Tracking

### Visual Progress Indicator

```
Quest: Coletar Flores
Target: 3 Frutas de Cura

Progress Bar:
0/3  [░░░░░░░░░░] 0%
1/3  [███░░░░░░░] 33%
2/3  [██████░░░░] 67%
3/3  [██████████] 100% ✓
```

---

## 🎬 Animation States

### Quest Indicator Animations

#### Yellow ! (Available)

```
Frame 1:  !     ← Base position
Frame 2:  !     ← Move up slightly
Frame 3:  !     ← Move down slightly
Frame 4:  !     ← Back to base
(Repeat - Bounce effect)
```

#### Golden ! (Ready)

```
Frame 1:  !     ← Base position
Frame 2:  !     ← Move up slightly
Frame 3:  !     ← Move down slightly
Frame 4:  !     ← Back to base
(Repeat - Bounce effect)
+ Glow effect
```

---

## 🔊 Audio Cues

### Quest Events

```
Quest Accepted:     🔊 "Quest_Accept.wav"
Progress Update:    🔊 "Quest_Progress.wav"
Quest Ready:        🔊 "Quest_Ready.wav"
Quest Completed:    🔊 "Quest_Complete.wav"
```

---

## 🎯 Testing Checklist (Visual)

```
☐ Scene Creation
  ├─ ☐ Menu item appears
  ├─ ☐ Scene is created
  ├─ ☐ Success dialog shows
  └─ ☐ All GameObjects present

☐ Visual Elements
  ├─ ☐ Yellow NPC visible
  ├─ ☐ Blue Player visible
  ├─ ☐ Green Ground visible
  ├─ ☐ Yellow ! indicator visible
  └─ ☐ Golden ! indicator (when ready)

☐ Interactions
  ├─ ☐ Can move player (WASD)
  ├─ ☐ Can interact with NPC (E)
  ├─ ☐ Dialogue appears
  ├─ ☐ Can accept quest
  └─ ☐ Can turn in quest

☐ Notifications
  ├─ ☐ Quest accepted notification
  ├─ ☐ Progress notifications
  ├─ ☐ Quest ready notification
  └─ ☐ Quest completed notification

☐ Inventory
  ├─ ☐ Can add items
  ├─ ☐ Progress updates
  ├─ ☐ Items removed on turn-in
  └─ ☐ Rewards added

☐ Indicators
  ├─ ☐ Yellow ! when available
  ├─ ☐ No indicator when active
  ├─ ☐ Golden ! when ready
  └─ ☐ Yellow ! again if repeatable
```

---

## 🎨 Scene Hierarchy

```
QuestSystemTest
├── --- MANAGERS ---
│   ├── QuestManager
│   └── GameManager
├── --- UI ---
│   └── Canvas
│       └── QuestNotificationPanel
├── NPC_QuestGiver
│   ├── QuestIndicatorAvailable
│   └── QuestIndicatorReady
├── Player
│   └── Main Camera
├── Ground
└── Directional Light
```

---

## 💡 Visual Tips

1. **Yellow = Available** - Quest can be accepted
2. **Golden = Ready** - Quest can be turned in
3. **No Indicator = Active** - Quest in progress
4. **Blue Player** - Easy to spot in scene
5. **Yellow NPC** - Quest giver identification
6. **Green Ground** - Walkable area

---

## 🎯 Quick Visual Reference

```
🟡 ! = Quest Available (Accept)
🟠 ! = Quest Ready (Turn In)
🔵 = Player
🟡 = NPC Quest Giver
🟢 = Ground
📦 = Inventory
📋 = Dialogue
🔊 = Sound Effect
✅ = Completed
```

---

**Use this guide for quick visual reference during testing!**
