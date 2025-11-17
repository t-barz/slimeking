# 🛠️ ExtraTools - Unity Editor Extensions

Sistema unificado de ferramentas para desenvolvimento Unity, organizadas em categorias para facilitar produtividade e manutenção de projetos.

## 📁 Estrutura Organizada

```text
Assets/Code/Editor/ExtraTools/
├── UnifiedExtraTools.cs          # 🏠 Interface principal unificada
├── QuickConfig/                  # ⚡ Configurações rápidas de objetos
│   ├── BushQuickConfig.cs        # 🌿 Configuração automática de bushes
│   ├── ItemQuickConfig.cs        # 🪨 Configuração de itens coletáveis
│   ├── NPCDialogueQuickConfig.cs # 💬 Configuração de NPCs com diálogo
│   └── PushableObjectQuickConfig.cs # 📦 Objetos empurráveis
├── SceneTools/                   # 🎬 Ferramentas de cena
│   ├── SceneSetupTool.cs         # 🔧 Configuração automática de cenas
│   ├── GameObjectSprayToolWindow.cs # 💨 Spray de GameObjects na cena
│   └── Services/                 # 📚 Serviços de apoio (GameObjectSprayTool)
├── NPCTools/                     # 🎭 Ferramentas avançadas para NPCs
│   └── QuickWins/               # Batch de ferramentas NPC
├── Project/                      # 📁 Ferramentas de projeto
│   └── ProjectSettingsExporter.cs # ⚙️ Exportação de configurações
└── QuestSystem/                  # 🎯 Sistema de Quests
    └── Authoring/               # Ferramentas de criação de quests
        ├── QuestCreationTool.cs # 🎯 Criação de quests
        └── QuestSpriteGenerator.cs # 🎨 Geração de sprites
```

---

## 🎯 Como Usar

### 🏠 Interface Principal

**Menu:** `Extra Tools/🏠 Open Extra Tools Window`

Janela unificada que consolida todas as ferramentas em abas organizadas:

- **Quick Config**: Configurações rápidas de objetos
- **Scene Tools**: Ferramentas de manipulação de cena  
- **NPC Tools**: Ferramentas avançadas para NPCs
- **Quest System**: Sistema de quests e missões
- **Project Tools**: Configurações e utilitários de projeto

### 🎯 **NAMESPACE:** `ExtraTools.QuestSystem`

- **Arquivos:** [`Assets/Code/Editor/ExtraTools/QuestSystem/`](Assets/Code/Editor/ExtraTools/QuestSystem/)
- **Dependências:** `TheSlimeKing.Quest` (core do sistema)

---

## 🎮 **Como Usar o Sistema Quest**

O sistema Quest foi integrado ao ExtraTools e oferece ferramentas para criação e gerenciamento de quests.

### ⚡ Quick Config (Context Menus)

**Acesso:** Clique direito no GameObject → `Extra Tools/`

#### 🌿 Configure as Bush

- **Script:** `BushQuickConfig.cs`
- **Função:** Adiciona componentes e configurações automáticas para bushes destructibles
- **Componentes:** Animator, Colliders, Scripts de interação

#### 🪨 Configure as Item

- **Script:** `ItemQuickConfig.cs`
- **Função:** Configura objetos como itens coletáveis
- **Componentes:** Sistema de coleta, feedback visual

#### 💬 Configure as Dialogue NPC

- **Script:** `NPCDialogueQuickConfig.cs`
- **Função:** Configura NPCs com sistema de diálogo interativo
- **Componentes:** NPCDialogueInteraction, CircleCollider2D (trigger), Icon de interação

#### 📦 Configure as Pushable Object

- **Script:** `PushableObjectQuickConfig.cs`
- **Função:** Configura objetos empurráveis pelo player
- **Componentes:** Physics, constraints, feedback

### 🎬 Scene Tools

#### 🔧 Scene Setup Tool

- **Menu:** Via `UnifiedExtraTools.cs` (Scene Tools tab)
- **Função:** Configuração automática de cenas com componentes essenciais
- **Features:** Managers, Cameras, Lighting, Post-processing

#### 💨 GameObject Spray Tool

- **Menu:** `Extra Tools/Scene Tools/GameObject Spray Tool`
- **Script:** `GameObjectSprayToolWindow.cs`
- **Função:** Ferramenta tipo "brush" para colocar múltiplos objetos na cena
- **Features:** Controle de densidade, raio, espaçamento, modo eraser

### 🎭 NPC Tools

Ferramentas avançadas localizadas em `NPCTools/QuickWins/`:

- **NPCQuickConfig.cs**: Configuração rápida de NPCs
- **NPCBatchConfigurator.cs**: Configuração em lote
- **NPCValidator.cs**: Validação de componentes
- **NPCDataGenerator.cs**: Geração de dados automática
- **NPCAnimatorSetup.cs**: Configuração de animators
- **NPCGizmosDrawer.cs**: Gizmos visuais
- **NPCPerformanceProfiler.cs**: Análise de performance

### 🎯 Quest System

#### 🎯 Create Collect Quest

- **Script:** `QuestCreationTool.cs`
- **Menu:** `Extra Tools/Quest System/Authoring/🎯 Create Collect Quest`
- **Função:** Cria novos assets CollectQuestData via diálogo
- **Recursos:** Dialog picker, valores padrão configurados

#### 📁 Create Folder Structure

- **Script:** `QuestCreationTool.cs`
- **Menu:** `Extra Tools/Quest System/Authoring/📁 Create Folder Structure`
- **Função:** Cria estrutura de pastas `Assets/Data/Quests`
- **Recursos:** Verificação automática, criação condicional

#### 🎨 Generate UI Sprites

- **Script:** `QuestSpriteGenerator.cs`
- **Menu:** `Extra Tools/Quest System/Authoring/🎨 Generate UI Sprites`
- **Função:** Gera sprites de indicadores (!, etc.) para quests
- **Recursos:** Sprites amarelo/dourado, configuração automática

### 📁 Project Tools

#### ⚙️ Project Settings Exporter

- **Script:** `ProjectSettingsExporter.cs`
- **Função:** Exporta configurações do projeto para backup/sharing
- **Acesso:** Via interface principal

---

## ⚠️ POLÍTICA DE MENUS - OBRIGATÓRIA

### 🚫 REGRA FUNDAMENTAL: APENAS UM MENU PRINCIPAL

**TODOS** os menus e ferramentas de editor **DEVEM** estar organizados sob `"Extra Tools/"`.

**❌ NUNCA CRIAR MENUS SEPARADOS COMO:**

- `"SlimeKing/..."`
- `"The Slime King/..."`
- `"MyTool/..."`
- `"ProjectName/..."`

**✅ SEMPRE USAR A ESTRUTURA:**

- `"Extra Tools/Tests/..."` - Para todos os testes
- `"Extra Tools/Setup/..."` - Para ferramentas de configuração
- `"Extra Tools/NPC/..."` - Para ferramentas de NPC
- `"Extra Tools/Scene Tools/..."` - Para ferramentas de cena
- `"Extra Tools/Quest System/..."` - Para sistema de quests
- `"Assets/Create/Extra Tools/..."` - Para criação de assets

### 🎯 Motivação

1. **Organização**: Um único ponto de entrada para todas as ferramentas
2. **Consistência**: Interface uniforme para toda a equipe
3. **Manutenibilidade**: Fácil localização e gestão de ferramentas
4. **Reutilização**: Estrutura agnóstica ao projeto específico

### 🔍 Como Validar

Antes de criar qualquer `[MenuItem]`:

```csharp
// ❌ ERRADO
[MenuItem("MyTool/Do Something")]

// ✅ CORRETO  
[MenuItem("Extra Tools/Category/Do Something")]
```

### 🚨 Consequências do Não Cumprimento

- **Code Review**: PRs serão rejeitados
- **Refactoring**: Menus incorretos serão movidos sem aviso
- **Documentation**: Ferramentas fora do padrão não serão documentadas

---

## 🔧 Arquitetura Técnica

### Namespaces

- **`ExtraTools.Editor`**: Namespace principal para todas as ferramentas
- **`ExtraTools.Core`**: Utilitários core e configurações
- **`ExtraTools.SceneTools`**: Ferramentas específicas de cena
- **`ExtraTools.QuestSystem`**: Sistema de quests e criação de missões

### Padrões de Design

- **Singleton Pattern**: Para managers persistentes
- **MenuItem Attributes**: Para integração com menu do Unity
- **EditorWindow**: Para interfaces gráficas customizadas
- **SerializedProperty**: Para manipulação segura de dados

### Dependencies

- **Unity Core**: UnityEngine, UnityEditor
- **Project Core**: SlimeKing.Core, SlimeKing.Gameplay (quando necessário)
- **Quest System**: TheSlimeKing.Quest (para ferramentas de quest)
- **Third Party**: SlimeMec.Gameplay.NPCs (para NPCs)

---

## 🎨 Convenções de UI

### Emojis nos Menus

- 🏠 Interface principal
- ⚡ Quick Config / Ações rápidas
- 🎬 Scene Tools / Ferramentas de cena
- 🎭 NPCs
- 🎯 Quest System / Quests e missões
- 📁 Project / Projeto
- 🔧 Setup / Configuração
- 💨 Spray / Brush tools
- 💬 Diálogo
- 🌿 Bushes / Vegetação
- 🪨 Items / Objetos
- 📦 Pushable / Empurráveis
- 🎨 Sprites / Geração visual

### Nomenclatura

- **Classes**: `ToolNameConfig.cs` ou `ToolNameWindow.cs`
- **Menus**: `"Extra Tools/Category/Tool Name"`
- **Context Menus**: `"GameObject/Extra Tools/Action"`

---

## 🚀 Extensibilidade

### Adicionando Nova Ferramenta

1. **Criar script** na pasta apropriada (`QuickConfig/`, `SceneTools/`, etc.)
2. **Usar namespace** `ExtraTools.Editor`
3. **Definir MenuItem** seguindo padrão `"Extra Tools/Category/Tool"`
4. **Integrar** à interface principal se necessário

### Exemplo de Nova Ferramenta

```csharp
using UnityEngine;
using UnityEditor;

namespace ExtraTools.Editor
{
    public static class NewToolConfig
    {
        [MenuItem("Extra Tools/Quick Config/🔧 Configure as New Tool")]
        public static void ConfigureAsNewTool()
        {
            // Implementação da ferramenta
        }
        
        [MenuItem("Extra Tools/Quick Config/🔧 Configure as New Tool", true)]
        public static bool ValidateConfigureAsNewTool()
        {
            return Selection.activeGameObject != null;
        }
    }
}
```

---

## 🔍 Troubleshooting

### Problemas Comuns

1. **Menu não aparece**: Verifique namespace e MenuItem path
2. **Ferramenta não funciona**: Confirme dependências estão presentes
3. **Context menu vazio**: Valide seleção de GameObject
4. **Performance lenta**: Use ferramentas em pequenos lotes

### Logs e Debug

- Logs controlados por flags `enableLogs` nos scripts
- Use `UnityEngine.Debug.Log` para feedback
- Prefixos padronizados: `[Extra Tools]`, `[NPC Tools]`, etc.

---

## 📋 Checklist de Migração

Se estiver migrando de estrutura antiga:

- [ ] ✅ Scripts movidos para `Assets/Code/Editor/ExtraTools/`
- [ ] ✅ Namespaces atualizados para `ExtraTools.Editor`
- [ ] ✅ Menu paths usando `"Extra Tools/"`
- [ ] ✅ Referências específicas do jogo removidas
- [ ] ✅ Interface principal funcionando
- [ ] ✅ Context menus operacionais
- [ ] ✅ Documentação atualizada

---

## 🏷️ Versioning

**Versão:** 2.1  
**Data:** Novembro 2025  
**Changelog:**

- ✅ Unificação completa em namespace genérico `ExtraTools`
- ✅ Remoção de referências específicas do jogo
- ✅ Reorganização em estrutura modular
- ✅ Interface principal consolidada
- ✅ Sistema Quest integrado ao ExtraTools
- ✅ Documentação atualizada e completa

---

*Este sistema é projetado para ser reutilizável em múltiplos projetos Unity. Contribuições e melhorias são bem-vindas!*
