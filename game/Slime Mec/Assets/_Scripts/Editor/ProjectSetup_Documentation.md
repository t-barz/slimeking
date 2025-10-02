# 📖 Documentação Completa - Estrutura de Pastas Simplificada Unity 2D

## 📋 Visão Geral

Esta documentação descreve a **nova estrutura simplificada** para projetos Unity 2D, projetada para maximizar produtividade com mínima complexidade. A estrutura reduz de 12 para 5 pastas principais, mantendo toda funcionalidade necessária.

## 🎯 Filosofia de Design Simplificada

### **Princípios Fundamentais**
- **Máxima Simplicidade**: Apenas 5 pastas principais essenciais
- **Workflow-Oriented**: Organizada por fluxo de trabalho, não por tipo técnico
- **Baixa Manutenção**: Estrutura que se auto-organiza
- **Escalabilidade Inteligente**: Cresce conforme necessário sem reorganização

---

## 📁 Nova Estrutura Simplificada

```
Assets/
├── Art/              # Todo conteúdo visual
│   ├── Sprites/      # Todas as imagens
│   ├── Materials/    # Materiais Unity
│   └── Animations/   # Sistema de animação completo
├── Audio/            # Todo conteúdo sonoro
│   ├── Music/        # Trilhas sonoras
│   └── SFX/          # Efeitos sonoros
├── Code/             # Todo código fonte
│   ├── Editor/       # Ferramentas desenvolvimento
│   ├── Gameplay/     # Lógica do jogo
│   └── Systems/      # Sistemas base
├── Game/             # Conteúdo específico do jogo
│   ├── Scenes/       # Todas as cenas
│   ├── Prefabs/      # Todos os prefabs
│   └── Data/         # Dados e configurações
└── External/         # Conteúdo terceiros
```

---

## 🎨 **Art/** - Todo Conteúdo Visual

### **Propósito**
Centraliza **TODOS** os assets visuais em uma única localização intuitiva.

### **Subpastas Detalhadas**

#### **Sprites/**
Todas as imagens, sem subcategorização forçada - organize por projeto conforme necessário.

**Tipos de Arquivo:**
```
.png, .jpg, .jpeg, .tga, .psd, .svg
```

**Convenção de Nomenclatura:**
```
Padrão: spr_[categoria][nome][variacao]

Exemplos de Categorias Flexíveis:
• spr_chrPlayerIdle.png        (character - player idle)
• spr_chrEnemyGoblin01.png     (character - enemy goblin frame 1)
• spr_envBushA.png             (environment - bush type A)
• spr_envRockCracked.png       (environment - rock cracked)
• spr_itmCoinGold.png          (item - gold coin)
• spr_uiButtonPlay.png         (ui - play button)
• spr_uiHealthBar.png          (ui - health bar)
• spr_bgForestLayer01.png      (background - forest layer 1)
```

#### **Materials/**
Todos os materiais Unity para sprites, efeitos e shaders.

**Convenção de Nomenclatura:**
```
Padrão: mat_[funcao][nome]

Exemplos:
• mat_spriteDefault.mat        (sprite padrão)
• mat_spritePixelPerfect.mat   (sprite pixel perfect)
• mat_effectGlow.mat           (efeito brilho)
• mat_uiBlur.mat               (UI blur)
```

#### **Animations/**
Todo sistema de animação centralizado em uma pasta.

**Suborganização Interna:**
- **Controllers/**: Animator Controllers (.controller)
- **Clips/**: Animation Clips (.anim)
- **Timelines/**: Timeline assets (se usado)

**Convenção de Nomenclatura:**
```
Controllers: ctrl_[objeto]
Clips:       anim_[objeto]_[acao]

Exemplos:
• ctrl_player.controller       (controller do player)
• ctrl_bush.controller         (controller da moita)
• anim_player_idle.anim        (animação idle do player)
• anim_player_walk.anim        (animação walk do player)
• anim_bush_shake.anim         (animação shake da moita)
• anim_bush_destroy.anim       (animação destroy da moita)
```

---

## 🔊 **Audio/** - Todo Conteúdo Sonoro

### **Propósito**
Organização simples de áudio sem complexidade desnecessária.

#### **Music/**
Todas as trilhas sonoras e música de fundo.

**Convenção de Nomenclatura:**
```
Padrão: mus_[contexto][numero]

Exemplos:
• mus_menuMain.wav            (menu principal)
• mus_gameplayLoop.wav        (gameplay loop)
• mus_level01.wav             (nível 1 específico)
• mus_bossTheme.wav           (tema do boss)
• mus_victory.wav             (vitória)
```

#### **SFX/**
Todos os efeitos sonoros, organizados por prefixo para fácil busca.

**Convenção de Nomenclatura:**
```
Padrão: sfx_[categoria]_[acao]

Categorias Recomendadas:
• sfx_player_jump.wav         (ações do player)
• sfx_player_attack.wav
• sfx_enemy_hit.wav           (ações de inimigos)
• sfx_enemy_death.wav
• sfx_env_bushDestroy.wav     (ambiente)
• sfx_env_rockBreak.wav
• sfx_ui_click.wav            (interface)
• sfx_ui_hover.wav
• sfx_weapon_sword.wav        (armas/itens)
• sfx_item_pickup.wav
```

---

## 💻 **Code/** - Todo Código Fonte

### **Propósito**
Organização simples de código por **responsabilidade**, não por tipo técnico.

#### **Editor/**
Scripts que executam apenas no Unity Editor.

**Exemplos:**
```
• ProjectSetupTool.cs         (ferramentas de setup)
• BushConfigEditor.cs         (editors customizados)
• AssetProcessor.cs           (processamento de assets)
```

#### **Gameplay/**
Toda lógica específica do jogo e mecânicas.

**Exemplos:**
```
• PlayerController.cs         (controle do player)
• BushDestruct.cs            (destruição de moitas)
• WindEmulator.cs            (sistema de vento)
• EnemyAI.cs                 (inteligência artificial)
• DropController.cs          (sistema de drops)
```

#### **Systems/**
Sistemas base, managers e arquitetura.

**Exemplos:**
```
• GameManager.cs             (gerenciador principal)
• AudioManager.cs            (sistema de áudio)
• SaveSystem.cs              (sistema de save)
• InputManager.cs            (sistema de input)
• SceneManager.cs            (gerenciamento de cenas)
```

**Convenção de Nomenclatura para Scripts:**
```
MANTÉM NOMES ORIGINAIS - Scripts não são renomeados por segurança
```

---

## 🎮 **Game/** - Conteúdo Específico do Jogo

### **Propósito**
Tudo que define diretamente o jogo final.

#### **Scenes/**
Todas as cenas, organizadas por prefixo para facilitar ordenação.

**Convenção de Nomenclatura:**
```
Padrão: scn_[tipo]_[nome]

Exemplos:
• scn_menu_main.unity         (menu principal)
• scn_menu_settings.unity     (menu configurações)
• scn_lvl_forest01.unity      (nível floresta 1)
• scn_lvl_cave02.unity        (nível caverna 2)
• scn_test_mechanics.unity    (teste mecânicas)
• scn_test_performance.unity  (teste performance)
```

#### **Prefabs/**
Todos os prefabs, organizados por prefixo para agrupamento automático.

**Convenção de Nomenclatura:**
```
Padrão: prf_[categoria]_[nome]

Exemplos:
• prf_chr_playerMain.prefab   (character - player principal)
• prf_chr_enemyGoblin.prefab  (character - inimigo goblin)
• prf_env_bushDestructible.prefab (environment - moita destrutível)
• prf_env_rockBreakable.prefab (environment - rocha quebrável)
• prf_ui_healthBar.prefab     (ui - barra de vida)
• prf_ui_pauseMenu.prefab     (ui - menu pause)
• prf_sys_gameManager.prefab  (system - game manager)
```

#### **Data/**
ScriptableObjects, configurações, fontes e dados do jogo.

**Tipos de Arquivo:**
```
.asset, .txt, .json, .xml, .ttf, .otf
```

**Convenção de Nomenclatura:**
```
ScriptableObjects: data_[tipo]_[nome]
Configurações:     cfg_[sistema]
Fontes:           font_[uso]

Exemplos:
• data_chr_playerStats.asset  (dados do player)
• data_itm_swordIron.asset    (dados da espada)
• cfg_gameSettings.asset      (configurações gerais)
• cfg_audioMixer.asset        (mixer de áudio)
• font_pixelMain.ttf          (fonte principal)
• font_uiRegular.otf          (fonte UI)
```

---

## 📦 **External/** - Conteúdo de Terceiros

### **Propósito**
Isolamento completo de assets externos para facilitar updates e manutenção.

**Organização Interna:**
```
External/
├── AssetStore/     # Assets da Unity Asset Store
├── Plugins/        # Plugins de terceiros
├── Libraries/      # Bibliotecas externas
└── Tools/          # Ferramentas e utilitários externos
```

**Convenção de Nomenclatura:**
```
Manter nomes originais dos desenvolvedores para facilitar updates
```

---

## 🔧 Código de Implementação

### **Nova Estrutura no ProjectSetup.cs**

```csharp
void CreateProjectStructure()
{
    // Estrutura simplificada para projetos 2D
    Dictionary<string, string[]> folderStructure = new Dictionary<string, string[]>
    {
        { "Art", new string[] { "Sprites", "Materials", "Animations" } },
        { "Art/Animations", new string[] { "Controllers", "Clips" } },
        { "Audio", new string[] { "Music", "SFX" } },
        { "Code", new string[] { "Editor", "Gameplay", "Systems" } },
        { "Game", new string[] { "Scenes", "Prefabs", "Data" } },
        { "External", new string[] { "AssetStore", "Plugins", "Libraries", "Tools" } }
    };

    // Cria as pastas
    foreach (var folder in folderStructure)
    {
        string mainFolder = "Assets/" + folder.Key;

        // Cria a pasta principal se não existir
        if (!AssetDatabase.IsValidFolder(mainFolder))
        {
            CreateFolder(mainFolder);
        }

        // Cria as subpastas
        foreach (var subFolder in folder.Value)
        {
            string subFolderPath = mainFolder + "/" + subFolder;
            if (!AssetDatabase.IsValidFolder(subFolderPath))
            {
                CreateFolder(subFolderPath);
            }
        }
    }

    AssetDatabase.Refresh();
    Debug.Log("Estrutura simplificada criada com sucesso!");
    EditorUtility.DisplayDialog("Concluído", "Estrutura simplificada criada com sucesso!", "OK");
}
```

### **Novo Sistema de Prefixos Simplificado**

```csharp
string GetPrefixForPath(string assetPath)
{
    // Mapeamento simplificado para nova estrutura
    Dictionary<string, string> folderPrefixes = new Dictionary<string, string>
    {
        // Art
        { "Assets/Art/Sprites", "spr" },
        { "Assets/Art/Materials", "mat" },
        { "Assets/Art/Animations/Controllers", "ctrl" },
        { "Assets/Art/Animations/Clips", "anim" },
        { "Assets/Art/Animations", "anim" },

        // Audio
        { "Assets/Audio/Music", "mus" },
        { "Assets/Audio/SFX", "sfx" },

        // Game
        { "Assets/Game/Scenes", "scn" },
        { "Assets/Game/Prefabs", "prf" },
        { "Assets/Game/Data", "data" },

        // External
        { "Assets/External", "ext" }
    };

    // Procura o prefixo mais específico
    string bestMatch = "";
    string bestPrefix = "";

    foreach (var kvp in folderPrefixes)
    {
        if (assetPath.StartsWith(kvp.Key + "/") && kvp.Key.Length > bestMatch.Length)
        {
            bestMatch = kvp.Key;
            bestPrefix = kvp.Value;
        }
    }

    return bestPrefix ?? "gen";
}
```

### **Reorganização Automática Atualizada**

```csharp
string GetNewPathForAsset(string assetPath)
{
    string fileName = Path.GetFileName(assetPath);
    string extension = Path.GetExtension(assetPath).ToLower();

    // Cenas
    if (extension == ".unity")
    {
        if (fileName.ToLower().Contains("menu"))
            return "Assets/Game/Scenes/" + fileName;
        else if (fileName.ToLower().Contains("test"))
            return "Assets/Game/Scenes/" + fileName;
        else
            return "Assets/Game/Scenes/" + fileName;
    }

    // Scripts - não movemos, mantemos estrutura existente se já estiver organizada
    if (extension == ".cs")
    {
        if (assetPath.Contains("Editor") || fileName.Contains("Editor"))
            return "Assets/Code/Editor/" + fileName;
        else if (fileName.Contains("Manager") || fileName.Contains("System"))
            return "Assets/Code/Systems/" + fileName;
        else
            return "Assets/Code/Gameplay/" + fileName;
    }

    // Imagens e Sprites - TODOS vão para Art/Sprites
    if (extension == ".png" || extension == ".jpg" || extension == ".jpeg" ||
        extension == ".tga" || extension == ".psd")
    {
        return "Assets/Art/Sprites/" + fileName;
    }

    // Áudio
    if (extension == ".wav" || extension == ".mp3" || extension == ".ogg")
    {
        if (fileName.ToLower().Contains("music") || fileName.ToLower().Contains("bgm"))
            return "Assets/Audio/Music/" + fileName;
        else
            return "Assets/Audio/SFX/" + fileName;
    }

    // Materiais
    if (extension == ".mat")
    {
        return "Assets/Art/Materials/" + fileName;
    }

    // Prefabs - TODOS vão para Game/Prefabs
    if (extension == ".prefab")
    {
        return "Assets/Game/Prefabs/" + fileName;
    }

    // Animações
    if (extension == ".anim")
    {
        return "Assets/Art/Animations/Clips/" + fileName;
    }

    // Animators
    if (extension == ".controller")
    {
        return "Assets/Art/Animations/Controllers/" + fileName;
    }

    // Fontes e dados
    if (extension == ".ttf" || extension == ".otf" || extension == ".asset")
    {
        return "Assets/Game/Data/" + fileName;
    }

    // Arquivos não reconhecidos vão para External
    return "Assets/External/" + fileName;
}
```

---

## 🎯 Diretrizes de Uso

### **Regra de Ouro: "Quando em Dúvida"**

1. **Visual?** → `Art/`
2. **Som?** → `Audio/`
3. **Código?** → `Code/`
4. **Conteúdo do Jogo?** → `Game/`
5. **Externo?** → `External/`

### **Workflow Recomendado**

1. **Setup Inicial:**
   - Execute `CreateProjectStructure()`
   - Aplique otimizações PixelArt

2. **Desenvolvimento Diário:**
   - Novos assets vão diretamente na pasta apropriada
   - Use prefixos automáticos para organização
   - Agrupe assets relacionados por nome

3. **Manutenção:**
   - Execute reorganização automática periodicamente
   - Use nomenclatura consistente
   - Mantenha External/ limpo e atualizado

---

## 🚀 Benefícios Específicos da Simplificação

### **Para Desenvolvedores Solo**
- ✅ Menos tempo organizando, mais tempo criando
- ✅ Estrutura intuitiva que não precisa ser lembrada
- ✅ Fácil de manter conforme projeto cresce

---

## 🔧 Configurações Automáticas de PixelArt

### **Otimizações Aplicadas**
O sistema aplica automaticamente:

#### **Câmera**
- Projeção ortográfica
- Anti-aliasing desabilitado

#### **Qualidade**
- Anti-aliasing: 0
- Anisotropic Filtering: Disabled
- V-Sync: 1

#### **Physics2D**
- Velocity Iterations: 6
- Position Iterations: 2
- Gravity: (0, -9.81)

---