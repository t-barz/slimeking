# Quest System UI Assets

Este documento descreve os assets de UI criados para o Quest System.

## Prefabs Criados

### 1. QuestNotificationPanel.prefab

**Localização:** `Assets/Game/Prefabs/UI/QuestNotificationPanel.prefab`

**Descrição:** Painel de notificação que aparece no topo da tela para informar o jogador sobre progresso de quests.

**Componentes:**

- **Canvas Panel** - Painel escuro semi-transparente (10% opacidade)
- **NotificationText** - TextMeshPro para exibir mensagens
- **QuestNotificationController** - Script que controla exibição e áudio

**Configuração:**

- Posição: Topo da tela (Anchor: Top Center)
- Tamanho: 600x100 pixels (ajusta automaticamente com ContentSizeFitter)
- Texto: Fonte NotoSans-ExtraBold, tamanho 24, centralizado, bold

**Como Usar:**

1. Adicione o prefab à sua cena (pode ser filho do Canvas principal ou ter seu próprio Canvas)
2. Configure as listas de AudioClips no Inspector:
   - `objectiveCompleteSounds` - Sons para objetivos completados
   - `questCompleteSounds` - Sons para quests completadas
3. O painel começa desativado e aparece automaticamente quando eventos de quest são disparados

### 2. QuestIndicatorAvailable.prefab

**Localização:** `Assets/Game/Prefabs/UI/QuestIndicatorAvailable.prefab`

**Descrição:** Indicador visual (exclamação amarela) que aparece acima de NPCs com quests disponíveis.

**Componentes:**

- **SpriteRenderer** - Renderiza sprite de exclamação amarela
- **Animator** - Animação de bounce contínua

**Configuração:**

- Cor: Amarelo (RGB: 255, 230, 0)
- Posição: 1.5 unidades acima do NPC
- Sorting Order: 100 (aparece acima de outros sprites)
- Animação: Bounce vertical suave (0.5s loop)

**Como Usar:**

1. Instancie como filho do GameObject do NPC
2. Ajuste posição Y conforme altura do NPC
3. Atribua sprite de exclamação quando disponível
4. Configure no QuestGiverController como `questAvailableIndicator`

### 3. QuestIndicatorReady.prefab

**Localização:** `Assets/Game/Prefabs/UI/QuestIndicatorReady.prefab`

**Descrição:** Indicador visual (exclamação dourada) que aparece acima de NPCs quando quest está pronta para entregar.

**Componentes:**

- **SpriteRenderer** - Renderiza sprite de exclamação dourada
- **Animator** - Animação de bounce contínua

**Configuração:**

- Cor: Dourado (RGB: 255, 179, 0)
- Posição: 1.5 unidades acima do NPC
- Sorting Order: 100 (aparece acima de outros sprites)
- Animação: Bounce vertical suave (0.5s loop)

**Como Usar:**

1. Instancie como filho do GameObject do NPC
2. Ajuste posição Y conforme altura do NPC
3. Atribue sprite de exclamação quando disponível
4. Configure no QuestGiverController como `questReadyIndicator`

## Animações

### QuestIndicatorBounce.anim

**Localização:** `Assets/Art/Animations/QuestIndicatorBounce.anim`

**Descrição:** Animação de bounce vertical para indicadores de quest.

**Características:**

- Duração: 0.5 segundos
- Loop: Contínuo
- Movimento: Y position de 0 → 0.2 → 0
- Curva: Suave (ease in/out)

### QuestIndicatorAnimator.controller

**Localização:** `Assets/Art/Animations/QuestIndicatorAnimator.controller`

**Descrição:** Animator Controller para indicadores de quest.

**Estados:**

- **Bounce** (default) - Reproduz animação de bounce em loop

## Sprites

### Gerando Sprites de Indicadores

Os sprites de exclamação podem ser gerados automaticamente usando a ferramenta do Editor:

**Menu:** `Tools > Quest System > Generate Quest Indicator Sprites`

Isso criará:

- `quest_indicator_available.png` - Exclamação amarela (32x32)
- `quest_indicator_ready.png` - Exclamação dourada (32x32)

**Localização:** `Assets/Art/Sprites/UI/`

**Configuração Recomendada:**

- Texture Type: Sprite (2D and UI)
- Pixels Per Unit: 32
- Filter Mode: Point (no filter) - para pixel art
- Compression: None ou Low Quality
- Max Size: 32

### Sprites Customizados

Se preferir usar sprites customizados:

1. Crie sprites 32x32 pixels com exclamação (!)
2. Salve em `Assets/Art/Sprites/UI/`
3. Configure como Sprite (2D and UI)
4. Arraste para os prefabs QuestIndicatorAvailable e QuestIndicatorReady

## Audio

### Sons Necessários

Consulte: `Assets/Audio/SFX/QuestSounds_README.md`

**Resumo:**

- 3-5 sons de objetivo completado (curtos, positivos)
- 3-5 sons de quest completada (mais elaborados, triunfantes)

**Configuração:**

1. Adicione AudioClips em `Assets/Audio/SFX/`
2. Configure no QuestNotificationPanel prefab
3. O sistema escolhe aleatoriamente para evitar repetição

## Integração com Cena

### Setup Básico

```
Hierarchy:
├── Canvas (ou UI Root)
│   └── QuestNotificationPanel (prefab)
└── NPCs
    └── QuestGiverNPC
        ├── (sprite, collider, etc)
        ├── QuestGiverController
        ├── QuestIndicatorAvailable (prefab, desativado)
        └── QuestIndicatorReady (prefab, desativado)
```

### Configuração do QuestGiverController

No Inspector do NPC:

1. Adicione quests disponíveis na lista `availableQuests`
2. Arraste QuestIndicatorAvailable para `questAvailableIndicator`
3. Arraste QuestIndicatorReady para `questReadyIndicator`
4. Os indicadores serão ativados/desativados automaticamente

## Customização

### Cores dos Indicadores

Edite os prefabs e ajuste a cor no SpriteRenderer:

- **Available** (padrão): RGB(255, 230, 0) - Amarelo brilhante
- **Ready** (padrão): RGB(255, 179, 0) - Dourado

### Animação de Bounce

Edite `QuestIndicatorBounce.anim`:

- Ajuste altura do bounce (padrão: 0.2 unidades)
- Ajuste velocidade (padrão: 0.5s por ciclo)
- Modifique curva de animação para bounce mais/menos suave

### Painel de Notificação

Edite `QuestNotificationPanel.prefab`:

- **Posição:** Ajuste Anchored Position (padrão: topo, -100Y)
- **Tamanho:** Ajuste Size Delta (padrão: 600x100)
- **Cor de Fundo:** Ajuste cor do Image component
- **Texto:** Ajuste fonte, tamanho, cor no TextMeshPro
- **Duração:** Ajuste `displayDuration` no QuestNotificationController (padrão: 3s)

## Troubleshooting

### Indicadores não aparecem

- Verifique se QuestGiverController está configurado corretamente
- Verifique se indicadores estão atribuídos no Inspector
- Verifique se QuestManager está na cena
- Ative `enableDebugLogs` no QuestGiverController

### Notificações não aparecem

- Verifique se QuestNotificationPanel está na cena
- Verifique se referências estão configuradas no Inspector
- Verifique se QuestManager está disparando eventos
- Ative `enableDebugLogs` no QuestNotificationController

### Sons não tocam

- Verifique se AudioClips estão atribuídos nas listas
- Verifique se AudioSource foi criado (automático se não configurado)
- Ajuste `soundVolume` no Inspector
- Verifique se AudioListener está na cena

### Animação não funciona

- Verifique se Animator está no GameObject
- Verifique se AnimatorController está atribuído
- Verifique se animação está configurada como Loop

## Próximos Passos

1. ✅ Gerar sprites de indicadores via menu Tools
2. ✅ Adicionar QuestNotificationPanel à cena principal
3. ⬜ Adicionar AudioClips de exemplo
4. ⬜ Configurar indicadores nos NPCs existentes
5. ⬜ Testar fluxo completo de quest com feedback visual/sonoro
