# 🛠️ Setup Técnico - The Slime King

## 🚀 Setup Rápido

### 1️⃣ Menu Extra Tools

```text
Extra Tools > Setup Projeto Completo
```

### 2️⃣ Configuração Manual

1. **Estrutura de Pastas + Unity Settings:** `Extra Tools > Setup Projeto Completo`
2. **URP:** Configurar se ainda não ativo
3. **Input System (Opcional):** Ativar manualmente nas Player Settings (caso deseje migrar do antigo)

## 📁 Estrutura Final do Projeto

```
Assets/
├── 🎨 Art/                  # Todo visual
│   ├── Sprites/             # Todas as imagens
│   ├── Materials/           # Materiais Unity
│   └── Animations/          # Controllers + Clips
│       ├── Controllers/     # .controller files
│       └── Clips/          # .anim files
├── 🔊 Audio/                # Todo sonoro
│   ├── Music/               # BGM e trilhas
│   └── SFX/                 # Efeitos sonoros
├── 💻 Code/                 # Scripts organizados
│   ├── Gameplay/            # PlayerController, criaturas
│   ├── Systems/             # Managers, Input, Audio
│   └── Editor/              # Ferramentas Extra Tools
├── 🎮 Game/                 # Conteúdo específico
│   ├── Scenes/              # Todas as cenas
│   ├── Prefabs/             # Todos os prefabs
│   └── Data/                # ScriptableObjects, configs
├── ⚙️ Settings/             # Configurações Unity
│   └── PostProcessing/      # Volume Profiles
└── 📦 External/             # Assets terceiros
    ├── AssetStore/
    ├── Plugins/
    ├── Libraries/
    └── Tools/
```

## 🎯 Convenções de Nomenclatura

### 📝 Prefixos Automáticos por Pasta

| Pasta | Prefixo | Exemplo |
|:--|:--|:--|
| Art/Sprites | `spr` | `spr_playerIdle.png` |
| Art/Materials | `mat` | `mat_spriteDefault.mat` |
| Art/Animations/Controllers | `ctrl` | `ctrl_player.controller` |
| Art/Animations/Clips | `anim` | `anim_player_walk.anim` |
| Audio/Music | `mus` | `mus_forestTheme.wav` |
| Audio/SFX | `sfx` | `sfx_player_jump.wav` |
| Game/Scenes | `scn` | `scn_forestCalm.unity` |
| Game/Prefabs | `prf` | `prf_chr_player.prefab` |
| Game/Data | `data` | `data_playerStats.asset` |

### 🏗️ Padrões de Classes

| Tipo | Sufixo | Exemplo | Uso |
|:--|:--|:--|:--|
| Gerenciadores | `Manager` | `GameManager` | Sistemas globais únicos |
| Controladores | `Controller` | `PlayerController` | Controle de entidades |
| Manipuladores | `Handler` | `InputHandler` | Processamento específico |
| Sistemas | `System` | `HealthSystem` | Funcionalidades modulares |

## 🎮 Input System (Planejamento Futuro)

Atualmente o projeto utiliza placeholders baseados no antigo sistema (`Input.GetAxis`, etc.).
Quando decidir migrar para o novo Input System:

1. Instalar/Ativar novo Input System via Package Manager (Unity Input System)
2. Reiniciar o Unity quando solicitado
3. Criar um Input Actions Asset em `Assets/Game/Data` (ex: `PlayerControls.inputactions`)
4. Configurar Action Maps sugeridos:
    - UI: Navegação de menus
    - Gameplay: Movimento, Ataque, Interagir
    - System: Pausar, Abrir Inventário
5. Gerar C# class a partir do asset (botão Generate C# Class)
6. Atualizar `InputManager` para usar `PlayerInput` e callbacks

Enquanto não migrar, o `InputManager` funciona como camada simples para leitura de input clássico.

## ⚙️ Configurações Unity 6 Aplicadas

### 🎨 Pixel Art Otimizado

```csharp
// Player Settings (4K)
PlayerSettings.defaultScreenWidth = 3840;
PlayerSettings.defaultScreenHeight = 2160;

// Quality Settings
QualitySettings.antiAliasing = 0;
QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
QualitySettings.vSyncCount = 1;

// Physics2D
Physics2D.velocityIterations = 6;
Physics2D.positionIterations = 2;
Physics2D.gravity = new Vector2(0, -9.81f);
```

### 🎪 URP + Post Processing

- Universal Render Pipeline configurado
- Post Processing habilitado
- Pasta Settings/PostProcessing criada

## 🔧 Scripts Base Criados

### 🎮 GameManager

- Sistema de estados do jogo
- Events para comunicação
- Singleton persistente

### 🔊 AudioManager  

- Controle de música e SFX
- Sistema de volume por categoria
- Singleton persistente

### ⌨️ InputManager

- Placeholder para futura integração com novo Input System
- Wrapper unificado para inputs (mesmo usando API antiga por enquanto)
- Singleton persistente

### 🕹️ PlayerController

- Movimento top-down suave
- Integração com GameManager
- Sistema de aceleração/desaceleração

## 🔄 Próximos Passos

### 1️⃣ Após Setup

1. **Verificar URP** ativo no projeto
2. **Testar scripts** base criados

### 2️⃣ (Opcional) Migrar para Novo Input System

1. Ativar pacote e reiniciar Unity
2. Criar Input Actions Asset e mapas (UI / Gameplay / System)
3. Atualizar `InputManager` com bindings reais

### 3️⃣ Primeiro Desenvolvimento

1. Criar cena de teste
2. Setup do Player com sprite
3. Implementar movimento básico
4. Adicionar câmera que segue player

## 🎯 Comandos Úteis Extra Tools

```text
Extra Tools/
├── Setup Projeto Completo     # Setup automático (estrutura + settings + scripts)
├── Criar Estrutura            # Apenas estrutura de pastas
└── Configurar Unity           # Apenas configurações Unity
```

## ⚡ Performance desde o Início

### ✅ Otimizações Aplicadas

- Physics2D configurado para 2D
- Anti-aliasing desabilitado (pixel art)
- Anisotropic filtering desabilitado
- Componentes cacheados nos scripts
- Singleton pattern para managers

### 🎯 Preparado para

- Object Pooling (quando necessário)
- Sistema de eventos desacoplado
- Pronto para migrar para Input System moderno (opcional)
- Post Processing otimizado

## 🔧 Scripts de Automação

### 📂 Estrutura de Pastas Automática

O script `ProjectSetupTool.cs` cria automaticamente toda a estrutura de pastas recomendada.

### ⚙️ Configurações Unity Automáticas

Aplicação automática de:

- Configurações de Player
- Quality Settings otimizados
- Physics2D para jogos 2D
    (Input System novo não é ativado automaticamente)

### 🎮 Scripts Base Gerados

Templates completos para:

- GameManager com sistema de estados
- AudioManager com controle de volume
- InputManager com placeholders
- PlayerController com movimento suave

## 📋 Checklist de Setup

### ✅ Executar Extra Tools

- [ ] Executar `Extra Tools > Setup Projeto Completo`
- [ ] Verificar se URP está ativo
- [ ] Testar scripts base funcionando

### ✅ Validação do Setup

- [ ] GameManager inicializa corretamente
- [ ] AudioManager reproduz áudio
- [ ] InputManager captura input (placeholders funcionando)
- [ ] PlayerController move na cena
- [ ] Estrutura de pastas criada

### ✅ Primeiro Desenvolvimento

- [ ] Criar cena de teste
- [ ] Adicionar Player GameObject
- [ ] Configurar câmera seguindo player
- [ ] Testar movimento básico
- [ ] Validar sistemas funcionando

## 🎯 Próximas Implementações

### 🔄 Sistema de Input Completo

Após o setup básico, implementar:

1. Input Actions Asset completo
2. Mapas de input específicos
3. Bindings para múltiplas plataformas
4. Sistema de rebinding de teclas

### 🎨 Arte e Animação

Preparação para:

1. Sprites do slime
2. Sistema de animação
3. Efeitos visuais
4. Post processing por bioma

### 🔊 Sistema de Áudio

Expansão do AudioManager:

1. Multiple audio sources
2. Audio mixing groups
3. Sistema de música adaptativa
4. SFX com variações aleatórias

## 📖 Documentação Relacionada

- [`BoasPraticas.md`](BoasPraticas.md) - Padrões de desenvolvimento
- [`GameDesign.md`](GameDesign.md) - Design e visão do jogo
- [`Roadmap.md`](Roadmap.md) - Tarefas e prioridades
