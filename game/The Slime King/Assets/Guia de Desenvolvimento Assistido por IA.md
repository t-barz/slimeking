# Guia de Desenvolvimento Assistido por IA - The Slime King

## DOCUMENTOS DE REFERÊNCIA

- **DOC_TECNICO**: Documento-de-Regras-Tecnicas.md
- **DOC_DESIGN**: Game-Design-Document.md


## ESPECIFICAÇÕES TÉCNICAS BASE

- **Engine**: Unity 6.0+
- **Render Pipeline**: URP 2D
- **Art Style**: Pixel Art
- **View**: Top-Down 2D
- **Resolution**: 1920x1080 base
- **Input System**: Unity Input System (New)

---

## FASE 1: FUNDAMENTOS

### SISTEMA LOCALIZAÇÃO

**REFERÊNCIA**: DOC_TECNICO - Seção 1 "Sistema de Localização de Textos"

```
CRIAR: LocalizationManager : MonoBehaviour, Singleton
ARQUIVO: localization.csv
FORMATO CSV: Key,EN,PT_BR,ES,FR,DE,JA,ZH_CN
MÉTODOS: GetLocalizedText(string key), DetectSystemLanguage(), LoadLanguageCSV()
FALLBACKS: User Config > System Language > Regional > EN
CONFIG: config.json em PersistentDataPath

CONVENÇÕES NOMENCLATURA (DOC_TECNICO - Seção 1.1):
- Interface: ui_[categoria]_[elemento]
- Diálogos: dialog_[npc]_[contexto]_[numero]
- Descrições: desc_[objeto]_[tipo]
- Mensagens: msg_[categoria]_[tipo]

HIERARQUIA DETECÇÃO (DOC_TECNICO - Seção 1.2):
1. Configuração do Usuário (config.json)
2. Idioma do Sistema
3. Fallback Regional
4. Fallback Global (EN)
```


### SISTEMA ÍCONE SUPERIOR

**REFERÊNCIA**: DOC_TECNICO - Seção 2 "Sistema de Ícone Superior"

```
CRIAR: IconManager : MonoBehaviour, Singleton
DETECTAR: Keyboard, Xbox, PlayStation, Switch, Generic
COMPONENTE: DeviceDetector com UnityEvent OnDeviceChanged
PREFABS: Icon_Keyboard, Icon_Xbox, Icon_PlayStation, Icon_Switch
ATIVAR: Apenas ícone do dispositivo atual
ANIMAÇÃO: FadeIn/FadeOut em 0.3s

COMPORTAMENTO EXIBIÇÃO (DOC_TECNICO - Seção 2.2):
- Aparecer com fade-in suave
- Permanecer visível durante interação possível
- Desaparecer com fade-out quando slime se afasta
- Trocar instantaneamente quando dispositivo muda
```


---

## FASE 2: PERSONAGEM MOVIMENTO

### ESTRUTURA SLIME

**REFERÊNCIA**: DOC_TECNICO - Seção 3 "Sistema de Personagem"

```
HIERARQUIA GameObject "slimeBaby" (DOC_TECNICO - Seção 3.1):
├── back (SpriteRenderer)
├── vfx_back (ParticleSystem)
├── vfx_front (ParticleSystem)  
├── front (SpriteRenderer)
├── side (SpriteRenderer)
├── vfx_side (ParticleSystem)
└── shadow (SpriteRenderer)

COMPONENTES OBRIGATÓRIOS (DOC_TECNICO - Seção 3.3):
- PlayerMovement : MonoBehaviour
- PlayerGrowth : MonoBehaviour  
- PlayerVisualManager : MonoBehaviour
- ElementalEnergyManager : MonoBehaviour
- InventoryManager : MonoBehaviour
- Rigidbody2D (Freeze Z rotation)
- Collider2D
```


### SISTEMA MOVIMENTO

**REFERÊNCIA**: DOC_TECNICO - Seção 4 "Sistema de Movimento"

```
INPUT ACTIONS (DOC_TECNICO - Seção 4.1):
Move: WASD/LeftStick, Attack: Space/B, Interact: E/A, Crouch: Q/X
UseItem: Alt/Y, ChangeItem: Tab/DPad, Abilities: 1-4/LB-RT
Menu: Enter/Menu, Inventory: RShift/View

CONFIGURAÇÃO FÍSICA POR ESTÁGIO (DOC_TECNICO - Seção 4.2):
Baby: velocity=3.0, mass=0.5, drag=5.0
Young: velocity=4.0, mass=1.0, drag=4.0  
Adult: velocity=4.5, mass=1.5, drag=3.0
Elder: velocity=5.0, mass=2.0, drag=2.0
```


### SISTEMA ANIMAÇÃO

**REFERÊNCIA**: DOC_TECNICO - Seção 5 "Sistema de Animação"

```
ANIMATOR PARÂMETROS (DOC_TECNICO - Seção 5.1):
isSleeping(bool), isHiding(bool), isWalking(bool)
Shrink(trigger), Jump(trigger), Attack01(trigger), Attack02(trigger)

REGRA CRÍTICA (DOC_TECNICO - Seção 5.2): 
isHiding=true → movimento BLOQUEADO
TRANSIÇÕES: Any State → estados via triggers têm prioridade
```


### VISIBILIDADE DIRECIONAL

**REFERÊNCIA**: DOC_TECNICO - Seção 3.2 "Regras de Visibilidade"

```
REGRAS PlayerVisualManager:
- Início: ativar "front" + "shadow"
- Norte(Y>0): ativar "back" + "shadow"  
- Sul(Y<0): ativar "front" + "shadow"
- Leste(X>0): ativar "side" + "shadow", flipX=false
- Oeste(X<0): ativar "side" + "shadow", flipX=true
- Diagonal: priorizar eixo maior magnitude, empate = vertical
```


---

## FASE 3: PROGRESSÃO

### SISTEMA ELEMENTAL

**REFERÊNCIA**: DOC_TECNICO - Seção 6 "Sistema de Absorção Elemental" + DOC_DESIGN - Seção "Sistema de Absorção Elemental"

```
ELEMENTOS (DOC_TECNICO - Seção 6.1):
Terra: #8B4513/#DEB887, +1 Defense/10pts, quebrar rochas
Água: #4169E1/#87CEEB, +1 Regeneração/10pts, nadar/crescer plantas
Fogo: #FF4500/#FFA500, +1 Attack/10pts, iluminar/derreter
Ar: #E6E6FA/#F0F8FF, +1 Speed/10pts, planar/ativar eólicos

FRAGMENTOS PREFAB (DOC_TECNICO - Seção 6.2):
- Três sprites: Small(1pt), Medium(3pts), Large(7pts)
- Color.Lerp(colorA, colorB, Random.value)
- DropTable configurável por objeto origem
- Atração magnética: raio=2.0, velocidade=8.0
```


### SISTEMA CRESCIMENTO

**REFERÊNCIA**: DOC_TECNICO - Seção 7 "Sistema de Crescimento" + DOC_DESIGN - Seção "Progressão e Crescimento"

```
ESTÁGIOS (DOC_TECNICO - Seção 7.1):
Baby=0, Young=200, Adult=600, Elder=1200

SEQUÊNCIA EVOLUÇÃO (DOC_TECNICO - Seção 7.2):
1. Energia >= threshold → desabilitar input
2. Partículas + screen flash + som harmônico
3. Sprite/escala change over 2s
4. Invulnerabilidade temporária
5. Reabilitar input

CARACTERÍSTICAS POR ESTÁGIO (DOC_DESIGN - Seção "Sistema de Crescimento em 4 Estágios"):
Baby: 0.5x tamanho, 1 slot, 0 seguidores, Squeeze
Young: 1x tamanho, 2 slots, 1 seguidor, Bounce + elementais básicas
Adult: 1.5x tamanho, 3 slots, 2-3 seguidores, Divisão
Elder: 2x tamanho, 4 slots, 3-4 seguidores, Grande Divisão
```


### SISTEMA INVENTÁRIO

**REFERÊNCIA**: DOC_TECNICO - Seção 8 "Sistema de Inventário" + DOC_DESIGN - Seção "Sistema de Inventário Evolutivo"

```
SLOTS POR ESTÁGIO (DOC_TECNICO - Seção 8.1): 
Baby=1, Young=2, Adult=3, Elder=4
STACK MÁXIMO: 10 por item
NAVEGAÇÃO: D-pad cyclic, uso imediato
AUTO-REORGANIZAÇÃO: remover vazios

MECÂNICA SLOTS (DOC_DESIGN):
- Slot único por tipo de item
- Acumulação até 10 unidades
- Seleção visual com ícone e quantidade
- Uso com botão dedicado
```


---

## FASE 4: COMBATE INTERAÇÃO

### SISTEMA COMBATE

**REFERÊNCIA**: DOC_TECNICO - Seção 9 "Sistema de Combate" + DOC_DESIGN - Seção "Sistema de Combate Cozy"

```
ATAQUES (DOC_TECNICO - Seção 9.1):
Basic: Space/B, 10+level dmg, 1.0 range, 0.5s cooldown
Dash: Hold Attack, 15+level dmg, 2.0 range, 1.0s cooldown  
Special: Special/RT, 20+special dmg, 1.5 range, 2.0s cooldown

DANO (DOC_TECNICO - Seção 9.2): 
realDamage = max(baseDamage - defense, 1)
FEEDBACK: hit flash branco 0.1s, knockback, invulnerabilidade 0.5s

FILOSOFIA COZY (DOC_DESIGN):
- Neutralização ao invés de destruição
- Transformar energia hostil em poder elemental
- Sistema de Parry para contra-ataques
```


### OBJETOS INTERATIVOS

**REFERÊNCIA**: DOC_TECNICO - Seção 10 "Sistema de Objetos Interativos"

```
COMPONENTES BASE (DOC_TECNICO - Seção 10.1):
- Collider2D trigger para detecção
- InteractiveObject : MonoBehaviour
- OutlineRenderer (opcional)
- IconDisplay (opcional)

TIPOS: Activate, Collect, Dialog, Puzzle, Portal
FEEDBACK (DOC_TECNICO - Seção 10.2): Outline colorido + ícone superior
DETECÇÃO: OverlapCircle contínuo
```


### OBJETOS DESTRUTÍVEIS

**REFERÊNCIA**: DOC_TECNICO - Seção 10.3 "Objetos Destrutíveis" + DOC_DESIGN - Seção "Ambiente Destrutível"

```
ATRIBUTOS (DOC_TECNICO): defense(int), maxHP(int)
DANO VISUAL: sprite pisca vermelho
DROP SYSTEM: DropTable com chances configuráveis
FRAGMENTS: tamanhos permitidos + probabilidades

DESTRUIÇÃO COOPERATIVA (DOC_DESIGN):
- Obstáculos grandes requerem ataques simultâneos
- Coordenação entre slime principal e seguidores
- Recompensas especiais para destruição cooperativa
```


### OBJETOS MÓVEIS

**REFERÊNCIA**: DOC_TECNICO - Seção 10.4 "Objetos Móveis"

```
REQUISITOS: proximidade ≤1.5, ângulo ≤45°, input interact
MOVIMENTO: Vector2 offset + float duration via Lerp
RASTRO: prefab instanciado cada X metros (opcional)
BLOQUEIO: sem nova interação durante movimento
```


---

## FASE 5: NARRATIVA NAVEGAÇÃO

### SISTEMA DIÁLOGOS

**REFERÊNCIA**: DOC_TECNICO - Seção 11 "Sistema de Diálogos" + DOC_DESIGN - Seção "Mecânicas de Interação"

```
UI COMPONENTES (DOC_TECNICO - Seção 11.1):
- Canvas overlay sempre instanciado
- Background semitransparente
- Text area com typewriter effect
- Speaker name field
- Portrait image (opcional)
- Continue indicator

TYPEWRITER (DOC_TECNICO - Seção 11.3): 
caracteres/segundo configurável, skip disponível
INTEGRAÇÃO: LocalizationManager.GetLocalizedText()
CONTROLES: pausar movimento durante diálogo
```


### SISTEMA PORTAL

**REFERÊNCIA**: DOC_TECNICO - Seção 12 "Sistema de Portal"

```
TIPOS ATIVAÇÃO (DOC_TECNICO - Seção 12.1): 
OnTouch (automático), OnInteraction (botão)
CONFIGURAÇÃO: origem/destino, mesma cena ou inter-cena

SEQUÊNCIA (DOC_TECNICO - Seção 12.2/12.3):
1. Efeito visual entrada
2. Teleporte instantâneo ou scene load
3. Efeito visual saída  
4. Desabilitar portal destino temporariamente

INTER-CENA: salvar estado → AsyncSceneLoad → restaurar estado
```


### MOVIMENTO ESPECIAL

**REFERÊNCIA**: DOC_TECNICO - Seção 13 "Sistema de Movimento Especial" + DOC_DESIGN - Seção "Sistema de Movimento Avançado"

```
SHRINK/SQUEEZE (DOC_TECNICO - Seção 13.1):
- Pontos específicos com InteractiveObject
- Requisitos: proximidade + direcionamento ≤30°
- Sequência: trigger Shrink → movimento suave → restore size

JUMP (DOC_TECNICO - Seção 13.2):
- Pontos específicos + verificação trajetória
- Movimento parabólico calculado
- Aterrissagem com efeito visual

HABILIDADES MOVIMENTO (DOC_DESIGN):
- Squeeze: passar por frestas pequenas
- Bounce: pular obstáculos, altura aumenta com crescimento
```


---

## FASE 6: SISTEMAS AVANÇADOS

### SISTEMA STEALTH

**REFERÊNCIA**: DOC_TECNICO - Seção 14 "Sistema de Stealth" + DOC_DESIGN - Seção "Sistema de Stealth e Crouch"

```
ESTADOS (DOC_TECNICO - Seção 14.1):
Normal: detectável, movimento livre
Crouched: movimento BLOQUEADO, verificar cobertura
Hidden: não detectável + cobertura válida  
Exposed: detectável + sem cobertura

COBERTURA (DOC_TECNICO - Seção 14.2): 
tags "Grass","Bush","Rock","Tree"
DETECÇÃO: OverlapCircle raio=1.0

STEALTH COOPERATIVO (DOC_DESIGN):
- Seguidores se escondem com slime principal
- Requer cobertura suficiente para todo o grupo
- Abordagem estratégica e pacífica
```


### SISTEMA UI

**REFERÊNCIA**: DOC_TECNICO - Seção 15 "Sistema de UI" + DOC_DESIGN - Seção "Interface do Usuário"

```
HUD LAYOUT (DOC_TECNICO - Seção 15.1):
- Health: top-left
- Elemental bars: top-right (4 cores)
- Inventory: bottom-center (1-4 slots)
- Followers: bottom-left (quando ativo)
- Growth bar: top-center (quando próximo evolução)

CANVAS (DOC_TECNICO - Seção 15.2): 
Scale With Screen Size, 1920x1080, Match=0.5

DESIGN VISUAL (DOC_DESIGN):
- Estética Cozy e Pixel Art
- Paleta tons pastéis
- Bordas orgânicas
- Animações suaves
```


### SEGUIDORES MINI-SLIMES

**REFERÊNCIA**: DOC_TECNICO - Seção 16 "Sistema de Seguidores e Mini-Slimes" + DOC_DESIGN - Seção "Sistema de Seguidores e Companheiros"

```
FOLLOWERS (DOC_TECNICO - Seção 16.1):
Bird(quest): switches altos | Rabbit(quest): túneis
Fish(quest): água | Crystal(quest): iluminar

AI ESTADOS (DOC_TECNICO - Seção 16.2): 
Following, Commanded, Cooperative, Resting, Distressed
FORMAÇÃO: manter distância, evitar overlap

MINI-SLIMES (DOC_TECNICO - Seção 16.3):
Young=1(45s,30%,45cd), Adult=2(60s,25%,30cd), Elder=3(90s,20%,20cd)
ELEMENTAL: Terra(+peso), Água(+swim), Fogo(+luz), Ar(+speed)

FILOSOFIA SEGUIDORES (DOC_DESIGN):
- Conexões emocionais da jornada
- Parceiros cooperativos em desafios
- Sistema seguimento tipo "snake"
- Limite 3-4 seguidores para clareza
- Habilidade Divisão disponível Adult+
```


---

## FASE 7: POLISH

### SISTEMA ÁUDIO

**REFERÊNCIA**: DOC_TECNICO - Seção 17 "Sistema de Áudio" + DOC_DESIGN - Seção "Áudio e Atmosfera"

```
TRIGGERS (DOC_TECNICO - Seção 17.1):
Movement: squish 0.3vol ±0.1pitch
Fragment: musical tone 0.5vol por elemento
Growth: harmony 0.8vol
Attack: whoosh 0.4vol ±0.2pitch

MIXER (DOC_TECNICO - Seção 17.2): 
Master(Music 0.7, SFX 0.8(Player 0.6, Followers 0.4, Env 0.5), UI 0.5)

DESIGN SONORO COZY (DOC_DESIGN):
- Ambient orchestral com instrumentos orgânicos
- Piano suave, violão acústico, flauta, cordas
- Dinâmica emocional responsiva
- Harmonias de grupo com seguidores
- Sons específicos para cada tipo de seguidor
```


### EFEITOS VISUAIS

**REFERÊNCIA**: DOC_TECNICO - Seção 18 "Sistema de Efeitos Visuais" + DOC_DESIGN - Seção "Sistema de Efeitos Visuais"

```
PARTÍCULAS (DOC_TECNICO - Seção 18.1):
Absorption: trail+burst por cor elemento
Growth: espiral ascendente 2s
Stealth: fade particles 0.5s

PÓS-PROCESSAMENTO (DOC_TECNICO - Seção 18.2):
Bloom: 0.3 sempre ativo
Vignette: 0.2 quando isHiding=true

SHADERS CUSTOMIZADOS (DOC_TECNICO - Seção 18.3):
- Outline Shader: objetos interativos
- Dissolve Shader: crescimento/transformação
- Water Shader: áreas aquáticas
- Crystal Shader: elementos cristalinos

SISTEMA VFX (DOC_DESIGN):
- Efeitos específicos para absorção elemental
- Partículas para atividades de seguidores
- Outlines adaptativos para objetos interativos
```


---

## FASE 8: PERSISTÊNCIA

### SAVE SYSTEM

**REFERÊNCIA**: DOC_TECNICO - Seção 19 "Sistema de Persistência" + DOC_DESIGN - Seção "Aspectos Técnicos"

```
ESTRUTURA SaveData (DOC_TECNICO - Seção 19.1):
- PlayerData: stage, position, elementalEnergy[], inventory
- WorldData: destroyedObjects[], completedQuests[], followers[]  
- Settings: volumes, language, uiScale

AUTO-SAVE (DOC_TECNICO - Seção 19.2): 
crescimento, nova região, quest completa, 5min timer
FORMATO: JSON em PersistentDataPath
BACKUP: sistema automático anti-corrupção

DADOS SEGUIDORES (DOC_DESIGN):
- Estado de relacionamentos
- Progresso de quests de ajuda
- Configurações de grupo e formação
```


---

## CONFIGURAÇÕES UNITY ESPECÍFICAS

### URP 2D SETTINGS

**REFERÊNCIA**: DOC_DESIGN - Seção "Aspectos Técnicos"

```
RENDER PIPELINE: Universal Render Pipeline
2D RENDERER: enabled
POST-PROCESSING: enabled para bloom/vignette
LIGHTING: 2D lights para efeitos elementais
```


### PIXEL ART SETUP

**REFERÊNCIA**: DOC_DESIGN - Seção "Pipeline de Renderização"

```
IMPORT SETTINGS sprites:
Filter Mode: Point
Compression: None  
Generate Mip Maps: false
Pixels Per Unit: baseado no grid do jogo

CAMERA:
Projection: Orthographic
Pixel Perfect Camera component
Reference Resolution: baseado no tile size
```


### INPUT SYSTEM

**REFERÊNCIA**: DOC_TECNICO - Seções de Input + DOC_DESIGN - Seção "Plataformas"

```
INPUT ACTIONS ASSET: PlayerInputActions
SCHEMES: Keyboard, Gamepad (Xbox, PlayStation, Switch)
AUTO-SWITCHING: enabled
DEVICE DETECTION: via InputSystem.devices
```


### PERFORMANCE

**REFERÊNCIA**: DOC_DESIGN - Seção "Performance e Otimização"

```
OBJECT POOLING: fragmentos, efeitos, projéteis
CULLING: desabilitar componentes off-screen
BATCHING: sprites mesmo material
LOD: animações simplificadas distância
FOLLOWER OPTIMIZATION: IA batching, culling específico
```


---

## ARQUITETURA EVENTOS

**REFERÊNCIA**: DOC_TECNICO - Seções de Eventos + DOC_DESIGN - Seção "Arquitetura de Eventos"

```
SISTEMA DESACOPLADO:
OnEnergyAbsorbed(int), OnGrowthStageChanged(int)
OnFollowerAdded(FollowerData), OnLanguageChanged(string)
OnInputDeviceChanged(InputDevice), OnPortalUsed(string,string)
OnObjectInteracted(string), OnDialogCompleted(string)

IMPLEMENTAÇÃO: UnityEvent com ScriptableObject para configuração
```


---

## NOMENCLATURA OBRIGATÓRIA

**REFERÊNCIA**: DOC_TECNICO - Seção "Convenções de Nomenclatura"

```
GameObjects: camelCase
Scripts: PascalCase  
Variáveis públicas: camelCase
Variáveis privadas: _camelCase
Constantes: UPPER_CASE
Localization keys: snake_case
System IDs: snake_case
```


---

## WORLD DESIGN E REGIÕES

**REFERÊNCIA**: DOC_DESIGN - Seção "Design de Mundo e Níveis"

```
ESTRUTURA METROIDVANIA:
- Mapa interconectado
- Crescimento abre novas possibilidades
- Seguidores desbloqueiam puzzles cooperativos
- Múltiplos caminhos por estágio/habilidades

REGIÕES PRINCIPAIS:
1. Caverna Inicial: Tutorial, Terra, escape
2. Floresta Sussurrante: Primeira liberdade, Ar/Natureza, primeiro seguidor
3. Cavernas Cristalinas: Terra/Cristal, exploração não-linear
4. Lagos Serenos: Água/Gelo, mecânicas aquáticas
5. Picos Ventosos: Ar, desafios verticais
6. Núcleo Elemental: Todos elementos, desafio final

QUEST SYSTEM: NPCs em necessidade, diferentes tipos de ajuda, recompensas variadas
```

<div style="text-align: center">⁂</div>

[^1]: Documento-de-Regras-Tecnicas.md

[^2]: Game-Design-Document.md

