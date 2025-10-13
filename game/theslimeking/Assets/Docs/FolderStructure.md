# 📁 Estrutura de Pastas - The Slime King

## 🎯 Visão Geral

Este documento detalha a organização completa do projeto **The Slime King** seguindo as boas práticas estabelecidas. A estrutura utiliza emojis para facilitar a navegação visual e está otimizada para projetos Unity 2D.

## 📂 Estrutura Completa

```
Assets/
├── 🎨 Art/                  # Todo conteúdo visual
│   ├── Sprites/             # Todas as imagens e texturas
│   ├── Materials/           # Materiais Unity para rendering
│   └── Animations/          # Sistema de animação completo
│       ├── Controllers/     # Animator Controllers (.controller)
│       └── Clips/          # Animation Clips (.anim)
├── 🔊 Audio/                # Todo conteúdo sonoro
│   ├── Music/               # Background music, trilhas sonoras
│   └── SFX/                 # Sound effects, feedbacks sonoros
├── 💻 Code/                 # Scripts organizados por função
│   ├── Gameplay/            # PlayerController, inimigos, mecânicas
│   ├── Systems/             # Managers, sistemas globais, Input System
│   └── Editor/              # Ferramentas de desenvolvimento
├── 🎮 Game/                 # Conteúdo específico do jogo
│   ├── Scenes/              # Todas as cenas do jogo (.unity)
│   ├── Prefabs/             # GameObjects pré-configurados
│   └── Data/                # ScriptableObjects, configurações
├── ⚙️ Settings/             # Configurações do Unity
│   └── PostProcessing/      # Volume Profiles, efeitos visuais
└── 📦 External/             # Assets de terceiros
    ├── AssetStore/          # Assets da Unity Asset Store
    ├── Plugins/             # Plugins externos
    ├── Libraries/           # Bibliotecas de terceiros
    └── Tools/               # Ferramentas externas
```

## 🎨 **Art/** - Conteúdo Visual

### **Sprites/**

- Texturas de personagens, inimigos, cenários
- UI elements, ícones, botões
- Sprites para animações 2D
- Tilesets para cenários

### **Materials/**

- Materiais Unity para 2D e 3D
- Shaders customizados
- Materiais de UI
- Materiais de efeitos visuais

### **Animations/**

- **Controllers/**: Animator Controllers que gerenciam as máquinas de estado
- **Clips/**: Animation Clips individuais para cada animação

## 🔊 **Audio/** - Conteúdo Sonoro

### **Music/**

- Background music das diferentes fases
- Trilhas sonoras temáticas por bioma
- Música de menu e cutscenes
- Loops musicais

### **SFX/**

- Efeitos sonoros de gameplay
- Feedbacks de UI
- Sons ambientes
- Efeitos de impacto e explosões

## 💻 **Code/** - Scripts Organizados

### **Gameplay/**

- **Controllers**: `PlayerController`, `EnemyController`, etc.
- **Mecânicas específicas**: Pulo, combate, coleta
- **Lógica de gameplay**: Progressão, power-ups
- **Interações**: Objetos interativos, NPCs

### **Systems/**

- **Managers**: `GameManager`, `AudioManager`, `SaveManager`
- **Handlers**: `InputHandler`, `CollisionHandler`
- **Sistemas globais**: Input System, eventos
- **Arquitetura base**: Singletons, patterns

### **Editor/**

- **ExtraTools**: Ferramentas de desenvolvimento
- **Custom Inspectors**: Editores customizados
- **Build Scripts**: Automatização de build
- **Utilities**: Utilitários de desenvolvimento

## 🎮 **Game/** - Conteúdo Específico

### **Scenes/**

- **MainMenu.unity**: Cena do menu principal
- **GameScene.unity**: Cenas de gameplay
- **Loading.unity**: Cenas de carregamento
- **Cutscenes**: Cenas de história

### **Prefabs/**

- **Player/**: Prefabs do jogador e variações
- **Enemies/**: Prefabs de todos os inimigos
- **UI/**: Prefabs de interface de usuário
- **Environment/**: Objetos de cenário
- **Pickups/**: Itens coletáveis

### **Data/**

- **ScriptableObjects**: Dados configuráveis
- **Save Data**: Estruturas de save/load
- **Game Config**: Configurações de balanceamento
- **Localization**: Arquivos de localização

## ⚙️ **Settings/** - Configurações Unity

### **PostProcessing/**

- **Global Profiles**: Volume profiles globais
- **Biome Profiles**: Profiles específicos por bioma
- **Gameplay Effects**: Efeitos de hit, evolução, etc.
- **Custom Effects**: Efeitos customizados

## 📦 **External/** - Assets Terceiros

### **AssetStore/**

- Assets baixados da Unity Asset Store
- Mantenha a estrutura original dos assets
- Documente a origem e versão

### **Plugins/**

- Plugins de terceiros
- SDKs externos
- Bibliotecas compiladas

### **Libraries/**

- Bibliotecas de código
- Frameworks externos
- Dependências

### **Tools/**

- Ferramentas de desenvolvimento
- Utilities externos
- Scripts de build externos

## 🛠️ Como Usar

### **1. Ferramenta Automática**

Execute a ferramenta **ExtraTools** no Unity:

```
Menu: Extra Tools > Projeto > Criar Estrutura de Pastas
```

Ou através da janela:

```
Menu: Extra Tools > Ferramentas Extras
```

### **2. Organização Manual**

Se preferir organizar manualmente:

1. **Crie a estrutura base** usando os nomes exatos com emojis
2. **Mova os assets existentes** para suas respectivas pastas
3. **Mantenha a consistência** na nomenclatura

### **3. Reorganização Automática**

Para reorganizar assets existentes:

```
Menu: Extra Tools > Projeto > Reorganizar Assets
```

## 📋 Boas Práticas de Organização

### **✅ Faça:**

1. **Use a estrutura consistentemente** - Sempre coloque arquivos nas pastas corretas
2. **Mantenha nomenclatura clara** - Use nomes descritivos em inglês
3. **Organize por funcionalidade** - Agrupe assets relacionados
4. **Use subpastas quando necessário** - Para organizar melhor assets numerosos
5. **Documente assets especiais** - Adicione comentários em assets importantes

### **❌ Evite:**

1. **Misturar tipos de assets** - Code em Art/, sprites em Audio/, etc.
2. **Nomes genéricos** - "Untitled", "New", "Test", etc.
3. **Pastas na raiz do Assets** - Use sempre a estrutura estabelecida
4. **Assets órfãos** - Sempre organize assets novos imediatamente
5. **Estrutura inconsistente** - Siga sempre o padrão estabelecido

## 🔄 Migração de Projetos Existentes

### **Passo 1: Backup**

```
Menu: Extra Tools > Projeto > Backup do Projeto
```

### **Passo 2: Criar Estrutura**

```
Menu: Extra Tools > Projeto > Criar Estrutura de Pastas
```

### **Passo 3: Reorganizar**

```
Menu: Extra Tools > Projeto > Reorganizar Assets
```

### **Passo 4: Validar**

```
Menu: Extra Tools > Debug > Validar Configurações
```

## 🎯 Benefícios da Organização

### **🚀 Desenvolvimento**

- **Navegação mais rápida** com emojis visuais
- **Encontrar assets facilmente** com estrutura lógica
- **Colaboração eficiente** com padrões claros
- **Manutenção simplificada** com organização consistente

### **📊 Performance**

- **Builds mais rápidos** com assets organizados
- **Carregamento otimizado** com estrutura clara
- **Gerenciamento de memória** melhor
- **Debugging facilitado** com estrutura lógica

### **🔧 Manutenção**

- **Refatoração segura** com referências organizadas
- **Atualizações controladas** com estrutura clara
- **Backup eficiente** com organização lógica
- **Controle de versão** otimizado

## 🎮 Específico para The Slime King

### **Gameplay Scripts**

- `PlayerController` → `💻 Code/Gameplay/`
- `SlimeManager` → `💻 Code/Systems/`
- `InputHandler` → `💻 Code/Systems/`

### **Assets Visuais**

- Sprites do Slime → `🎨 Art/Sprites/Player/`
- Animações do Slime → `🎨 Art/Animations/Player/`
- UI do jogo → `🎨 Art/Sprites/UI/`

### **Áudio**

- Trilha principal → `🔊 Audio/Music/`
- Sons do Slime → `🔊 Audio/SFX/Player/`
- Efeitos ambientes → `🔊 Audio/SFX/Environment/`

## 🔍 Validação e Manutenção

### **Validação Regular**

Execute periodicamente:

```
Menu: Extra Tools > Debug > Validar Configurações
```

### **Limpeza de Assets**

Para encontrar assets não utilizados:

```
Menu: Extra Tools > Gerenciamento de Assets > Encontrar Assets Não Utilizados
```

### **Organização Automática**

Para organizar por tipo:

```
Menu: Extra Tools > Gerenciamento de Assets > Organizar Assets por Tipo
```

## 📞 Suporte

Para dúvidas sobre a organização:

1. **Consulte este documento** primeiro
2. **Execute a validação** para verificar inconsistências
3. **Use as ferramentas automáticas** sempre que possível
4. **Mantenha a consistência** com a estrutura estabelecida

---

**📝 Nota**: Esta estrutura foi criada seguindo as boas práticas estabelecidas no documento `BoasPraticas.md` e é específica para o projeto **The Slime King** desenvolvido em Unity 6.2+.
