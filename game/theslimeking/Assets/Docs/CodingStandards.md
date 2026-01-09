# Coding Standards - The Slime King

## 📋 Estrutura Geral

### Organização de Arquivos

- **Editor tools**: `Assets/Editor/[ToolName]/`
- **Scripts do jogo**: `Assets/_Code/Scripts/`
- **Prefabs**: `Assets/_Prefabs/`
- **Cenas**: `Assets/_Scenes/`
- **Documentação**: `Assets/Docs/Worklogs/` (para worklogs e logs de implementação)
- **Assets externos**: `Assets/External Assets/` (NÃO MODIFICAR)

#### 🗂️ Mapa de Diretórios (Atual)

Estrutura principal do projeto e propósito de cada pasta/arquivo relevante:

```text
Raiz/
├── Assets/
│   ├── _Code/                     # Scripts do jogo (namespace organizado)
│   │   ├── Managers/              # Singletons e controladores globais
│   │   ├── UI/                    # Componentes de UI com Input System
│   │   ├── Items/                 # ScriptableObjects de itens e enums
│   │   └── Environments/          # Scripts de ambientes/cenas
│   ├── _Prefabs/                  # Prefabs de GameObjects
│   ├── _Scenes/                   # Cenas (TitleScreen, InitialCave, etc.)
│   ├── AddressableAssetsData/     # Configurações do Addressables
│   ├── Docs/                      # Documentação (inclui CodingStandards e Worklogs)
│   ├── Editor/                    # Ferramentas de Editor (MenuItem, Windows)
│   ├── External Assets/           # Pacotes/recursos de terceiros (NÃO MODIFICAR)
│   ├── Resources/                 # Recursos carregados em runtime
│   ├── Screenshots/               # Capturas de tela do projeto
│   ├── Settings/                  # ScriptableObjects de configurações
│   ├── Tests/                     # Arquivos de teste temporários (excluir após uso)
│   ├── TextMesh Pro/              # Dados do TMP
│   ├── InputSystem_Actions.inputactions   # Mapa do Input System
│   ├── UniversalRenderPipelineGlobalSettings.asset  # Config global URP
│   └── DefaultVolumeProfile.asset  # Perfil de pós-processamento padrão (URP)
├── Packages/                      # Manifesto e lock de pacotes (UPM)
├── ProjectSettings/               # Configurações do projeto (Editor, Graphics, etc.)
├── Library/                       # Cache do Unity (gerado automaticamente, não versionar)
├── Logs/                          # Logs de execução/edição
├── Temp/                          # Arquivos temporários de build
└── UserSettings/                  # Preferências do usuário/editor
```

Notas importantes:
- Não modificar conteúdos em `Assets/External Assets/`.
- NUNCA adicionar logs à não ser que seja explicitamente solicitado.
- Editor tools sempre em `Assets/Editor/[ToolName]/` seguindo estrutura modular.
- Testes devem ser temporários e removidos imediatamente após execução.
- Todos paths devem ser relativos a `Assets/` nas operações de Editor.

### Estrutura de Classes

```csharp
// Ordem obrigatória:
1. using statements
2. namespace
3. XML documentation
4. class declaration
5. #region Fields
6. #region Unity Lifecycle (Awake, Start, OnEnable, OnDisable, Update, etc)
7. #region Public Methods
8. #region Private Methods
9. #region Utility Methods
```

---

## 🎯 Convenções de Nomenclatura

### Classes e Métodos

- **PascalCase** para classes, métodos, propriedades
- **camelCase** para campos privados
- **UPPER_CASE** para constantes

```csharp
public class GameObjectBrushTool  // ✅ PascalCase
private float brushRadius;        // ✅ camelCase
private const string VERSION = "1.0"; // ✅ UPPER_CASE
```

### Prefixos e Sufixos

- Editor Windows: `*Window.cs` ou `*Tool.cs`
- Services: `*Service.cs`
- Settings: `*Settings.cs`
- Managers: `*Manager.cs`

### Nomes de Arquivos e Pastas

- ❌ **NUNCA** usar emojis em nomes de arquivos ou pastas
- ✅ Usar apenas caracteres alfanuméricos, hífens e underscores
- ✅ PascalCase para arquivos de código
- ✅ kebab-case ou snake_case para documentação

### Nomenclatura de Prefabs

Esta seção define padrões consistentes para nomenclatura de prefabs, eliminando ambiguidades e facilitando navegação no projeto.

#### Regras Gerais

**PascalCase obrigatório**
- Todos os prefabs devem usar PascalCase sem espaços ou underscores
- **Razão**: Consistência com nomenclatura de classes C# e melhor legibilidade no Project Window
- Exemplo: `GameManager.prefab`, não `Game_Manager.prefab` ou `game manager.prefab`

**Sem prefixos redundantes**
- A estrutura de pastas já categoriza os assets (`Assets/_Prefabs/Items/`, `Assets/_Prefabs/Characters/`)
- **Não usar**: `item_`, `art_`, `prop_`, `char_`, etc.
- **Razão**: Evita redundância visual e facilita refatoração/reorganização futura
- ✅ `Assets/_Prefabs/Items/Apple.prefab`
- ❌ `Assets/_Prefabs/Items/item_apple.prefab`

**Sufixos semânticos**
- Use sufixos para indicar **função/tipo técnico**, não categoria de conteúdo
- Sufixos clarificam propósito em contextos onde o prefab aparece sozinho (Inspector, Search)
- **Quando usar**: Managers, VFX, Canvas, HUD, NPC, Point, Controller
- **Quando NÃO usar**: Para indicar que é item/prop (a pasta já faz isso)

**Variantes: descritivas primeiro, letras como último recurso**
- **Preferir**: Nomes descritivos (`SlimeGreen`, `EnemyElite`, `TreeOak`)
- **Aceitar**: Letras (A/B/C) ou números (01/02/03) apenas para variações artísticas mínimas
- **Razão**: Nomes descritivos são auto-documentados; letras exigem memorização
- ✅ `CrystalRed.prefab`, `CrystalBlue.prefab` (descritivo)
- 🟡 `TreeOakA.prefab`, `TreeOakB.prefab` (aceitável se visualmente idênticos)
- ❌ `Crystal1.prefab`, `Crystal2.prefab` (não descritivo)

#### Sufixos Padrão

Sufixos indicam **arquitetura técnica** ou **função no jogo**, não conteúdo visual.

| Sufixo | Quando Usar | Exemplo | Motivo |
|--------|-------------|---------|--------|
| `Manager` | Singletons globais persistentes entre cenas | `GameManager`, `AudioManager` | Indica padrão Singleton |
| `Controller` | Controladores de gameplay localizados | `PlayerController`, `BossController` | Diferencia de Managers |
| `VFX` | Sistemas de partículas / efeitos visuais | `ExplosionVFX`, `HealVFX` | Clarifica que não é sprite estático |
| `SFX` | Prefabs de audio com AudioSource | `FootstepSFX`, `AmbientSFX` | Diferencia de clipes de audio puros |
| `HUD` | Elementos UI overlay (sem Canvas próprio) | `HealthBarHUD`, `MiniMapHUD` | Indica que é UI de jogo |
| `Canvas` | Canvas UI completos e autônomos | `MainMenuCanvas`, `PauseCanvas` | Diferencia de elementos HUD |
| `NPC` | Personagens não-jogáveis com IA/diálogo | `VillagerNPC`, `MerchantNPC` | Diferencia de decoração animada |
| `Point` | Transforms de referência/marcadores | `SpawnPoint`, `TeleportPoint` | Indica GameObject vazio ou marker |

#### Categorias por Pasta

**🎮 Systems & Managers** (`Assets/_Prefabs/`)

Prefabs técnicos que gerenciam sistemas globais do jogo.

- **Nomenclatura**: `[Nome]Manager.prefab` ou `[Sistema].prefab`
- **Sem espaços**: `CameraManager`, não `Camera Manager`
- **Razão**: Managers são código, não arte visual
- **Exemplos**:
  - ✅ `GameManager.prefab` - Singleton principal do jogo
  - ✅ `CameraManager.prefab` - Gerenciamento de câmera
  - ✅ `TeleportManager.prefab` - Sistema de teleporte
  - ✅ `SceneTransitioner.prefab` - Sistema de transição
  - ✅ `EventSystem.prefab` - Input System do Unity

**🎭 Characters** (`Assets/_Prefabs/Characters/`)

Personagens jogáveis e inimigos com comportamento/animação.

- **Nomenclatura**: `[Nome][Variante].prefab`
- **Sem prefixo `art_`**: A pasta já indica que é personagem
- **Variantes descritivas**: Use cores, tipos ou roles quando aplicável
- **Exemplos**:
  - ✅ `PlayerSlime.prefab` - Personagem principal (Player + tipo)
  - ✅ `BeeWorker.prefab` - Abelha trabalhadora
  - ✅ `BeeQueen.prefab` - Abelha rainha
  - ✅ `Gobu.prefab` - Inimigo goblin
  - ✅ `Butterfly.prefab` - Borboleta
  - 🟡 `BeeWorkerA.prefab`, `BeeWorkerB.prefab` - Variantes artísticas (aceitável)

**🧙 NPCs** (`Assets/_Prefabs/NPCs/`)

Non-player characters com IA, diálogo ou interação específica.

- **Nomenclatura**: `[Nome]NPC.prefab`
- **Sufixo obrigatório**: `NPC` diferencia de decoração ou enemies
- **Razão**: NPCs têm scripts de diálogo/quest; decoração não
- **Exemplos**:
  - ✅ `HelpyNPC.prefab` - NPC que dá ajuda
  - ✅ `RickNPC.prefab` - NPC chamado Rick
  - ✅ `MerchantNPC.prefab` - NPC vendedor
  - ❌ `NPC_helpy.prefab` - Prefixo ao invés de sufixo

**✨ FX** (`Assets/_Prefabs/FX/`)

Efeitos visuais usando Particle System ou animação.

- **Nomenclatura**: `[Ação]VFX.prefab`
- **Sufixo obrigatório**: `VFX` clarifica que não é sprite estático
- **Numeração**: Use apenas para variações da mesma ação (Attack01, Attack02)
- **Exemplos**:
  - ✅ `AbsorbVFX.prefab` - Efeito de absorção
  - ✅ `Attack01VFX.prefab` - Primeiro ataque visual
  - ✅ `Hit01VFX.prefab` - Efeito de impacto
  - ✅ `ExclamationVFX.prefab` - ! animado
  - ✅ `WindVFX.prefab` - Efeito de vento
  - ❌ `absorve_vfx.prefab` - snake_case
  - ❌ `vfx_exclamation.prefab` - Prefixo ao invés de sufixo

**🎒 Items** (`Assets/_Prefabs/Items/`)

Itens coletáveis, consumíveis ou equipáveis.

- **Nomenclatura**: `[Nome][Variante].prefab`
- **Sem prefixo `item_`**: Pasta já categoriza como item
- **Variantes descritivas**: Cores, tipos, qualidade (Red, Rare, Large)
- **Exemplos**:
  - ✅ `Apple.prefab` - Maçã genérica
  - ✅ `CrystalRed.prefab` - Cristal vermelho
  - ✅ `CrystalBlue.prefab` - Cristal azul
  - ✅ `FireStar.prefab` - Estrela de fogo
  - ✅ `Mushroom.prefab` - Cogumelo
  - ✅ `PotionHealth.prefab` - Poção de vida
  - ❌ `item_apple.prefab` - Prefixo redundante
  - ❌ `appleA.prefab` - Variante sem significado

**🏗️ Props** (`Assets/_Prefabs/Props/`)

Objetos decorativos ou interativos do cenário.

- **Nomenclatura**: `[Objeto][Especificador].prefab`
- **Especificador**: Tipo, material, tamanho ou localização
- **Razão**: Props costumam ter múltiplas variantes visuais
- **Exemplos**:
  - ✅ `TreeOak.prefab` - Árvore de carvalho
  - ✅ `TreePine.prefab` - Árvore de pinheiro
  - ✅ `RockLarge.prefab` - Pedra grande
  - ✅ `RockSmall.prefab` - Pedra pequena
  - ✅ `ChestWooden.prefab` - Baú de madeira
  - ✅ `TorchWall.prefab` - Tocha de parede
  - ✅ `BarrelBroken.prefab` - Barril quebrado

**🎨 UI** (`Assets/_Prefabs/UI/`)

Elementos de interface do usuário.

- **Nomenclatura**: Distinguir entre Canvas completo e elementos HUD
- **Canvas**: Telas completas autônomas → `[Nome]Canvas.prefab`
- **HUD**: Elementos overlay de gameplay → `[Nome]HUD.prefab`
- **Razão**: Facilita busca e organização hierárquica
- **Exemplos**:
  - ✅ `MainMenuCanvas.prefab` - Menu principal completo
  - ✅ `PauseCanvas.prefab` - Tela de pausa
  - ✅ `InventoryCanvasHUD.prefab` - Canvas de inventário overlay
  - ✅ `HealthBarHUD.prefab` - Barra de vida overlay
  - ✅ `MiniMapHUD.prefab` - Mini-mapa
  - ✅ `DialogueBox.prefab` - Caixa de diálogo genérica

**🔧 Debug & Utilities**

Ferramentas de desenvolvimento não usadas em build final.

- **Nomenclatura**: `[Debug] [Nome].prefab` (prefixo com colchetes)
- **Único caso de prefixo permitido**: Facilita filtro visual no Editor
- **Razão**: Deve ser óbvio que não é conteúdo de produção
- **Exemplos**:
  - ✅ `[Debug] InputLoggingSystem.prefab`
  - ✅ `[Debug] CollisionVisualizer.prefab`
  - ✅ `[Debug] PerformanceMonitor.prefab`

#### Guia de Decisão: Variantes

**Quando usar nomes descritivos** (PREFERIR):
- ✅ Cores: `SlimeRed`, `CrystalBlue`, `MushroomPoisonous`
- ✅ Tamanhos: `RockSmall`, `TreeLarge`, `ChestMedium`
- ✅ Materiais: `DoorWooden`, `SwordIron`, `ShieldSteel`
- ✅ Estados: `ChestOpen`, `ChestClosed`, `BarrelBroken`
- ✅ Roles: `EnemyBasic`, `EnemyElite`, `EnemyBoss`

**Quando usar letras (A/B/C)** (ACEITÁVEL):
- 🟡 Variações artísticas sutis sem diferença funcional clara
- 🟡 Múltiplas versões de mesmo sprite com pequenas mudanças
- Exemplo: `TreeOakA`, `TreeOakB` (galhos ligeiramente diferentes)

**Quando usar números (01/02/03)** (EVITAR):
- ⚠️ Apenas para sequências lógicas (Attack01, Attack02, Level01)
- ❌ NÃO para variantes aleatórias: prefira nomes descritivos

#### ❌ Anti-padrões (Evitar)

**Snake_case e prefixos redundantes**:
```text
❌ player_Slime.prefab         → ✅ PlayerSlime.prefab
❌ item_appleA.prefab          → ✅ Apple.prefab ou AppleRed.prefab
❌ art_beeB.prefab             → ✅ BeeWorker.prefab ou BeeWorkerB.prefab
❌ prop_rock_large.prefab      → ✅ RockLarge.prefab
```

**Prefixos ao invés de sufixos**:
```text
❌ vfx_explosion.prefab        → ✅ ExplosionVFX.prefab
❌ sfx_footstep.prefab         → ✅ FootstepSFX.prefab
❌ npc_merchant.prefab         → ✅ MerchantNPC.prefab
```

**Espaços e camelCase**:
```text
❌ Camera Manager.prefab       → ✅ CameraManager.prefab
❌ Main Camera.prefab          → ✅ MainCamera.prefab
❌ teleportPoint.prefab        → ✅ TeleportPoint.prefab
❌ healthBar.prefab            → ✅ HealthBarHUD.prefab
```

**Kebab-case e variantes sem significado**:
```text
❌ npc-rick-version2.prefab    → ✅ RickNPC.prefab
❌ enemy-type-1.prefab         → ✅ EnemyBasic.prefab
❌ crystal-a-red.prefab        → ✅ CrystalRed.prefab
```

### Nomenclatura de Cenas (Scenes)

Esta seção define padrões para nomenclatura de arquivos `.unity` (cenas do jogo).

#### Regras Gerais

**PascalCase obrigatório**
- Todas as cenas devem usar PascalCase sem espaços, underscores ou prefixos numéricos
- **Razão**: Consistência com nomenclatura de código e melhor legibilidade
- Exemplo: `TitleScreen.unity`, não `1_TitleScreen.unity` ou `title_screen.unity`

**Sem prefixos de desenvolvedor**
- Não usar nomes de pessoas como prefixo (ERICK_, JOAO_, etc.)
- **Razão**: Cenas são do projeto, não de indivíduos; use branches Git para trabalho pessoal
- ❌ `ERICK_InitialForest.unity`
- ✅ `InitialForest.unity` ou `InitialForestTest.unity` (se for temporária)

**Nomenclatura descritiva e hierárquica**
- Use nomes que descrevam o **propósito** ou **localização** da cena
- Para níveis sequenciais, use nomes descritivos ao invés de números
- **Quando usar números**: Apenas para níveis claramente sequenciais após o nome descritivo

#### Categorias de Cenas

**🎮 Cenas de Sistema/UI**
```text
✅ TitleScreen.unity          # Tela inicial do jogo
✅ MainMenu.unity             # Menu principal
✅ OptionsMenu.unity          # Menu de opções/configurações
✅ Credits.unity              # Créditos
✅ LoadingScreen.unity        # Tela de carregamento
✅ PauseMenu.unity            # Menu de pausa (se for cena separada)
```

**🗺️ Cenas de Gameplay (Levels/Áreas)**
```text
Preferir nomes descritivos:
✅ InitialCave.unity          # Primeira caverna
✅ InitialForest.unity        # Primeira floresta
✅ AncientTemple.unity        # Templo antigo
✅ DarkDungeon.unity          # Masmorra escura
✅ ThroneRoom.unity           # Sala do trono

Com números quando houver progressão clara:
✅ CaveLevel01.unity          # Caverna nível 1
✅ CaveLevel02.unity          # Caverna nível 2
✅ ForestArea01.unity         # Área da floresta 1

Ou combinando localização + número:
✅ Cave01.unity               # Se houver múltiplas cavernas numeradas
✅ Cave02.unity
✅ Forest01.unity
✅ Forest02.unity
```

**🧪 Cenas de Teste/Debug**
```text
✅ TestArena.unity            # Arena de testes
✅ TestPhysics.unity          # Teste de físicas
✅ TestCombat.unity           # Teste de combate
✅ SandboxPlayer.unity        # Sandbox para testar player

Com prefixo [Test] se for temporária:
✅ [Test] NewMechanic.unity   # Teste temporário de mecânica
✅ [Test] LightingSetup.unity # Teste temporário de iluminação
```

**🎬 Cenas Especiais**
```text
✅ Cutscene01.unity           # Cutscene numerada
✅ BossFightDragon.unity      # Luta contra boss específico
✅ TutorialBasics.unity       # Tutorial de mecânicas básicas
✅ EndGameSequence.unity      # Sequência final do jogo
```

#### Padrão de Organização Build Settings

No Build Settings, cenas devem aparecer em ordem lógica:

```text
0. TitleScreen.unity
1. MainMenu.unity
2. InitialCave.unity
3. InitialForest.unity
4. AncientTemple.unity
...
```

**Sem prefixos numéricos nos nomes dos arquivos**. A ordem é definida pela posição no Build Settings, não pelo nome do arquivo.

#### ❌ Anti-padrões (Evitar)

```text
❌ 1_TitleScreen.unity         → ✅ TitleScreen.unity
❌ 2_InitialCave.unity         → ✅ InitialCave.unity
❌ ERICK_InitialForest.unity   → ✅ InitialForest.unity ou InitialForestTest.unity
❌ title_screen.unity          → ✅ TitleScreen.unity
❌ Title Screen.unity          → ✅ TitleScreen.unity
❌ level-1.unity               → ✅ Level01.unity ou CaveLevel01.unity
❌ scene_test_01.unity         → ✅ TestArena.unity ou [Test] NewFeature.unity
```

#### Renomeação de Cenas Existentes

**IMPORTANTE**: Ao renomear cenas no Unity:
1. Use o Unity Editor (Project Window) ao invés de renomear arquivos diretamente
2. Verifique e atualize o Build Settings após renomear
3. Comunique mudanças ao time (pode quebrar referências em branches)
4. Faça commit separado apenas com renomeação de cenas

---

## 📝 Documentação

### XML Documentation Obrigatória

Toda classe pública deve ter:

```csharp
/// <summary>
/// Descrição breve do propósito da classe
/// 
/// Detalhes adicionais sobre uso, funcionalidades, etc.
/// 
/// Acesso: Menu > Extra Tools > [Category]
/// </summary>
```

### Comentários de Métodos Complexos

```csharp
/// <summary>
/// Descrição do que o método faz
/// </summary>
/// <param name="name">Descrição do parâmetro</param>
/// <returns>Descrição do retorno</returns>
```

### Logs de Implementação

- Toda implementação significativa deve gerar um worklog em `Assets/Docs/Worklogs/`
- Formato: `YYYY-MM-DD-feature-name.md`
- Incluir: objetivo, decisões técnicas, arquivos modificados

---

## �️ Organização de Hierarquia de Cenas

### Padrão de Estrutura

Toda cena deve seguir uma hierarquia organizada e padronizada para facilitar navegação, manutenção e trabalho em equipe.

#### Estrutura Raiz Obrigatória

Toda cena deve ter os seguintes GameObjects raiz organizadores (em ordem):

```text
Root Scene Hierarchy:
├── --- SYSTEMS ---         # Separador visual (GameObject vazio desativado)
├── GameManager             # Singleton global (se necessário nesta cena)
├── CameraManager           # Sistema de câmera (se necessário)
├── EventSystem             # Input System do Unity
├── TeleportManager         # Outros managers específicos da cena
├── --- ENVIRONMENT ---     # Separador visual
├── Background              # Camadas de parallax e céu
├── Grid                    # Tilemap e tiles
├── Scenario                # Props, decoração, obstáculos
├── --- GAMEPLAY ---        # Separador visual
├── Player                  # Personagem jogável (spawn point ou instância)
├── NPCs                    # Non-player characters
├── Enemies                 # Inimigos da cena
├── Items                   # Itens coletáveis na cena
├── --- MECHANICS ---       # Separador visual
├── Mechanics               # Puzzles, interactables, teleports
├── SpawnPoints             # Pontos de spawn (player, enemies, items)
├── Triggers                # Triggers de eventos
├── --- EFFECTS ---         # Separador visual
├── Lighting                # Iluminação global e point lights
├── ParticleSystems         # Efeitos de partículas ambientais
├── PostProcessing          # Volumes de pós-processamento
├── --- UI ---              # Separador visual
├── CanvasHUD               # UI de gameplay (vida, mana, etc.)
└── CanvasDebug             # UI de debug (FPS, logs)
```

#### Regras de Nomenclatura na Hierarquia

**Separadores Visuais**
- Use `--- CATEGORIA ---` para separar seções principais
- GameObject vazio com `activeSelf = false` (aparece desabilitado no Editor)
- **Razão**: Facilita navegação visual sem impacto em runtime

**GameObjects Organizadores**
- Use **PascalCase** para todos os GameObjects organizadores
- Evite números ou underscores: `Scenario`, não `scenario_01` ou `Scenario_1`
- Mantenha nomes genéricos para organizadores: `Background`, `Scenario`, `Mechanics`

**Instâncias de Prefabs**
- Mantenha nome original do prefab ou adicione sufixo descritivo
- ✅ `BeeWorkerB` ou `BeeWorkerB_Patrol01`
- ❌ `art_beeB (3)` ou `GameObject (15)`
- Use sufixo quando houver múltiplas instâncias com roles diferentes

**Objetos com Numeração**
- Use underscore + número quando necessário: `RockLarge_01`, `Tree_05`
- **Evitar**: Numeração automática do Unity `(Clone)`, `(1)`, `(2)`
- **Exceção**: Decoração repetitiva sem role específico pode manter números

#### Detalhamento por Categoria

**🖥️ SYSTEMS**

Managers e sistemas técnicos que não são visíveis no mundo do jogo.

```text
✅ GameManager              # Singleton principal
✅ CameraManager            # Controle de câmera
✅ EventSystem              # Input System
✅ TeleportManager          # Sistema de teleporte
✅ QuestManager             # Sistema de quests (se houver)
✅ AudioManager             # Gerenciamento de audio (se instanciado na cena)
```

**🌄 ENVIRONMENT**

Elementos visuais e estruturais do cenário.

**Background**: Camadas de paralaxe, céu, montanhas distantes
```text
Background/
├── Sky_back               # Camada mais distante
├── Sky_middle             # Camada intermediária
├── Sky_front              # Camada mais próxima
└── Sky_clouds             # Nuvens (se separadas)
```

**Grid**: Tilemaps e estrutura de tiles
```text
Grid/
├── GroundTilemap          # Chão principal
├── WallsTilemap           # Paredes e colisão
├── DecorationTilemap      # Decoração em tiles
└── PropsTilemap           # Props em tilemap (se houver)
```

**Scenario**: Objetos decorativos e props do cenário
```text
Scenario/
├── Rocks/                 # Sub-categoria de rocks (opcional)
│   ├── RockLarge_01
│   ├── RockSmall_01
│   └── RockSmall_02
├── Vegetation/            # Sub-categoria de vegetação (opcional)
│   ├── GrassA_01
│   ├── MushroomA_01
│   └── TreeOak_01
├── Props/                 # Props diversos (opcional)
│   ├── ChestWooden_01
│   ├── TorchWall_01
│   └── CaveEntrance
└── Puddles/               # Efeitos decorativos
    └── prop_puddle_01
```

**Organização de Scenario**:
- **Pequena quantidade (<20 objetos)**: Manter todos direto em `Scenario/`
- **Quantidade média (20-50)**: Agrupar por tipo (Rocks/, Vegetation/, Props/)
- **Grande quantidade (50+)**: Agrupar por área da cena (Area01/, Area02/)

**🎮 GAMEPLAY**

Elementos interativos do jogo.

**Player**: Personagem jogável ou spawn point
```text
✅ Player                  # Instância do player (se spawnar na cena)
✅ PlayerSpawnPoint        # Ponto de spawn (se player for instanciado depois)
```

**NPCs**: Personagens não-jogáveis
```text
NPCs/
├── RickNPC                # NPC específico
├── MerchantNPC            # Vendedor
└── VillagerNPC_01         # Vilão genérico
```

**Enemies**: Inimigos da cena
```text
Enemies/
├── GobuPatrol_01          # Inimigo em patrol
├── GobuGuard_01           # Inimigo guardando área
└── BeeWorkerSwarm_01      # Grupo de inimigos
```

**Items**: Itens coletáveis
```text
Items/
├── Apple_01               # Item específico posicionado
├── CrystalRed_01          # Cristais espalhados
└── HealthPotion_01        # Poções
```

**🔧 MECHANICS**

Mecânicas, puzzles e interações.

```text
Mechanics/
├── Puzzles/               # Puzzles da cena
│   ├── PuzzleRoom
│   └── RollingRockPuzzle
├── Interactables/         # Objetos interativos
│   ├── ShrinkPointA
│   ├── ShrinkPointB
│   └── LeverPuzzle
├── Teleports/             # Pontos de teleporte
│   ├── TeleportPointExit
│   ├── TeleportPointSecret
│   └── TeleportPointPuzzleRoom
└── Destructibles/         # Objetos destrutíveis (se não estiverem em Scenario)
    └── BreakableWall_01
```

**📍 SPAWNPOINTS**

Pontos de spawn organizados por categoria.

```text
SpawnPoints/
├── Player/
│   └── PlayerSpawnMain
├── Enemies/
│   ├── EnemySpawn_01
│   └── EnemySpawn_02
└── Items/
    ├── ItemSpawn_01
    └── ItemSpawn_02
```

**⚡ TRIGGERS**

Zonas de trigger para eventos.

```text
Triggers/
├── CutsceneTrigger_01
├── DialogueTrigger_Rick
├── BossFightTrigger
└── CheckpointTrigger_01
```

**✨ EFFECTS**

Efeitos visuais, iluminação e pós-processamento.

**Lighting**: Iluminação da cena
```text
Lighting/
├── GlobalLight2D          # Luz global (se houver)
├── PointLight_Torch01     # Luzes pontuais
└── AreaLight_Cave         # Luzes de área
```

**ParticleSystems**: Partículas ambientais
```text
ParticleSystems/
├── FogAmbient             # Neblina ambiental
├── DustParticles          # Poeira/atmosfera
└── WaterDrops_Cave        # Gotas de água
```

**PostProcessing**: Volumes de pós-processamento
```text
PostProcessing/
├── GlobalVolume           # Volume global da cena
└── CaveVolume             # Volume específico de área
```

**🎨 UI**

Elementos de interface.

```text
UI/
├── CanvasHUD              # HUD principal (vida, mana, etc.)
├── CanvasInventory        # Inventário (se instanciado na cena)
└── CanvasDebug            # Debug UI (FPS counter, etc.)
```

#### ❌ Anti-padrões (Evitar)

**Hierarquia desorganizada**:
```text
❌ Root desorganizado:
    ├── art_rickA
    ├── Particle System          # Nome genérico
    ├── Camera Manager           # Espaços no nome
    ├── Scenario
    ├── GameObject (15)          # Numeração automática
    ├── TeleportManager
    └── Grid
```

**Nomes de GameObject ruins**:
```text
❌ env_brown_rockA2 (4)          → ✅ RockBrownLarge_04
❌ art_rickA                     → ✅ RickNPC
❌ teleportPoint                 → ✅ TeleportPointExit
❌ shrinkPointA                  → ✅ ShrinkPoint_01
❌ caveEntrance (3)              → ✅ CaveEntrance_03
❌ GameObject (15)               → ✅ [Nome descritivo]
```

**Scenario mal organizado**:
```text
❌ Scenario com 80+ objetos direto na raiz sem sub-categorias
❌ Usar camelCase: mushroomA, grassB
❌ Prefixos desnecessários: env_, prop_, art_
❌ Numeração do Unity: (1), (2), (Clone)
```

#### Workflow de Organização

**1. Antes de adicionar novos objetos**:
- Identifique a categoria correta (ENVIRONMENT, GAMEPLAY, MECHANICS, etc.)
- Use o organizador apropriado (Scenario, NPCs, Mechanics)
- Nomeie descritivamente antes de posicionar

**2. Ao instanciar prefabs**:
- Remova sufixo `(Clone)` automático
- Adicione sufixo descritivo se necessário: `_Patrol`, `_Guard`, `_01`
- Posicione no organizador correto imediatamente

**3. Limpeza periódica**:
- Remova numeração automática `(1)`, `(2)`, `(3)`
- Renomeie GameObjects genéricos
- Reorganize objetos soltos na raiz
- Verifique hierarquia com `Ctrl+Shift+H` (hierarchy search)

**4. Antes de commit**:
- Verifique hierarquia raiz seguindo estrutura padrão
- Confirme que não há `GameObject (X)` ou `prefabName (Clone)`
- Valide nomes em PascalCase
- Teste que a cena funciona após reorganização

---

## �🎨 Menu Structure (Unity Editor)

### Hierarquia Obrigatória de Menus

#### Extra Tools (Menu Principal)

```text
Extra Tools/
├── Setup/
│   └── Create Folders
├── Organize/
│   └── Organize Prefabs
├── Scene Tools/
│   └── GameObject Brush Tool
└── Debug/
    └── Export Scene Structure
```

#### Quick Tools (Menu de Contexto)

```text
Quick Tools/
└── Debug/
    └── Export Object Structure
```

### MenuItem Format

```csharp
// Menu principal
[MenuItem("Extra Tools/[Category]/[Feature Name]")]

// Menu de contexto
[MenuItem("GameObject/Quick Tools/[Category]/[Feature Name]")]

// Validação de menu de contexto
[MenuItem("GameObject/Quick Tools/[Category]/[Feature Name]", true)]
```

---

## � Arquitetura de Cenas

### Scene Controllers

Toda cena deve ter uma classe Controller responsável por questões específicas daquela cena:

- **Padrão de nomenclatura**: `[NomeDaScene]Controller.cs`
- **Localização**: `Assets/_Code/Scripts/Controllers/` ou `Assets/_Code/Gameplay/`
- **Responsabilidades**: Inicialização da cena, gerenciamento de estado, coordenação de sistemas

```csharp
/// <summary>
/// Controller principal da cena MainMenu.
/// Gerencia a inicialização e comportamento específico desta cena.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // Inicialização e lógica específica da cena MainMenu
}
```

**Exemplos de nomenclatura:**

- `MainMenuController.cs` - Controller da cena "MainMenu"
- `GameplayController.cs` - Controller da cena "Gameplay"
- `Level1Controller.cs` - Controller da cena "Level1"

---

## �🏗️ Arquitetura de Editor Tools

### Estrutura Modular

Ferramentas complexas devem ser divididas em:

1. **Window** - UI e orquestração
2. **Settings** - Configurações e persistência
3. **Services** - Lógica de negócio
4. **Utilities** - Funções auxiliares

```text
Assets/Editor/[ToolName]/
├── [ToolName]Window.cs      // EditorWindow principal
├── [ToolName]Settings.cs    // Configurações e EditorPrefs
├── [Feature]Service.cs      // Lógica específica
└── [Helper]Utility.cs       // Funções auxiliares
```

---

## ⚡ Performance

### Unity Editor

- ✅ Cachear referências em `OnEnable()`
- ✅ Usar `sqrMagnitude` ao invés de `Distance()` quando possível
- ✅ Usar operações batch com Undo
- ✅ Evitar `Find()`, `FindObjectsOfType()` em loops
- ✅ Usar `Dictionary` para lookups frequentes
- ❌ Não usar `Resources.Load()` no Editor

### Serialização

```csharp
// Preferir EditorPrefs para configurações de editor
EditorPrefs.SetFloat("ToolName_BrushRadius", brushRadius);

// Usar JsonUtility para estruturas complexas
string json = JsonUtility.ToJson(data, true);
File.WriteAllText(path, json);
```

---

## 🎮 Unity Específico

### Campos Serializados

```csharp
[SerializeField] private float speed;    // ✅ Preferir
public float speed;                      // ❌ Evitar expor desnecessariamente
```

### Undo/Redo

```csharp
// Sempre registrar operações destrutivas
Undo.RecordObject(target, "Operation Name");
Undo.DestroyObjectImmediate(obj);
Undo.RegisterCreatedObjectUndo(instance, "Create Object");

// Para múltiplas operações
Undo.SetCurrentGroupName("Batch Operation");
```

### Asset Management

```csharp
// Sempre refresh após modificar assets
AssetDatabase.Refresh();

// Usar paths relativos
string relativePath = "Assets/Docs/Temp/file.json";
```

---

## 🎨 UI Guidelines (Editor)

### Cores e Feedback Visual

```csharp
// Botões de modo com cores semânticas
GUI.backgroundColor = Color.green;      // Ativo/Sucesso
GUI.backgroundColor = Color.red;        // Perigo/Eraser
GUI.backgroundColor = new Color(1f, 0.6f, 0.2f); // Alerta/Seletivo
GUI.backgroundColor = Color.white;      // Reset
```

### Emojis para Melhor UX

```csharp
// ✅ Permitido APENAS em strings de UI
"🖌️ GameObject Brush Tool"  // Títulos de janelas
"📦 Prefab Slots"            // Seções
"⚙️ Settings"                // Configurações
"🎲 Randomization"           // Features especiais
"🔧 Debug"                   // Ferramentas de debug

// ❌ NUNCA em nomes de arquivos ou pastas
// Errado: "GameObject Brush Tool 🖌️.cs"
// Certo:  "GameObjectBrushTool.cs"
```

### HelpBox

```csharp
EditorGUILayout.HelpBox("Mensagem informativa", MessageType.Info);
EditorGUILayout.HelpBox("Atenção!", MessageType.Warning);
EditorGUILayout.HelpBox("Erro crítico", MessageType.Error);
```

---

## 🔒 Segurança e Validação

### Sempre Validar

```csharp
// Verificar nulls
if (obj == null) return;

// Verificar bounds
if (index < 0 || index >= list.Count) return;

// Usar properties com validação
private int SafeSelectedIndex
{
    get => Mathf.Clamp(selectedIndex, 0, list.Count - 1);
    set => selectedIndex = Mathf.Clamp(value, 0, list.Count - 1);
}
```

### EditorUtility.DisplayDialog

```csharp
// Confirmar ações destrutivas
bool confirmed = EditorUtility.DisplayDialog(
    "Confirmar Ação",
    "Esta operação não pode ser desfeita. Continuar?",
    "Sim",
    "Cancelar"
);
```

---

## 📊 Debugging

### Debug Logs Opcionais
Os logs devem sempre ser opcionais e nunca devem ser implementados sem que sejam explicitamente solicitados.
```csharp
private bool enableDebugLogs = false;

private void DebugLog(string message)
{
    if (enableDebugLogs)
    {
        Debug.Log($"[{GetType().Name}] {message}");
    }
}
```

---

## 🚫 Evitar

- ❌ Código comentado (usar Git para histórico)
- ❌ Magic numbers (usar constantes nomeadas)
- ❌ Métodos com mais de 50 linhas
- ❌ Classes com mais de 500 linhas (refatorar em services)
- ❌ `GameObject.Find()` ou `FindObjectsOfType()` em loops
- ❌ Operações de I/O sem tratamento de exceção
- ❌ Emojis em nomes de arquivos ou pastas
- ❌ Autor e data de criação em XML documentation

---

## 📖 Referências

- Unity Editor Scripting: <https://docs.unity3d.com/ScriptReference/Editor.html>
- MenuItem Attribute: <https://docs.unity3d.com/ScriptReference/MenuItem.html>
- EditorPrefs: <https://docs.unity3d.com/ScriptReference/EditorPrefs.html>
- Undo System: <https://docs.unity3d.com/ScriptReference/Undo.html>
