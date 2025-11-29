# 📁 Plano de Reorganização da Documentação

## 🎯 Objetivo

Reorganizar a documentação em uma estrutura lógica e fácil de consultar, movendo arquivos obsoletos para exclusão.

---

## 📂 Nova Estrutura Proposta

```
Assets/Docs/
├── 📘 README.md (índice principal)
│
├── 📁 01-Project/ (Documentação do Projeto)
│   ├── GDD/ (Game Design Documents)
│   │   ├── Game-Design-Document.md
│   │   └── Executive-Summary.md
│   ├── Planning/ (Planejamento)
│   │   ├── Roadmap.md
│   │   ├── ALPHA-1-Checklist.md
│   │   └── Roadmap-Analysis-Summary.md (MOVER PARA ARCHIVE)
│   ├── Standards/ (Padrões e Boas Práticas)
│   │   ├── BoasPraticas.md
│   │   └── FolderStructure.md
│   └── Technical/ (Documentação Técnica)
│       ├── TechMapping.md
│       └── ProjectSetup-Documentation.md
│
├── 📁 02-Systems/ (Documentação de Sistemas)
│   ├── Core/ (Sistemas Principais)
│   │   ├── Architecture/
│   │   │   ├── Managers-Design-Document.md
│   │   │   ├── GameManager-AutoCreation.md
│   │   │   └── JobSystemMigration.md (MOVER PARA ARCHIVE)
│   │   └── Player/
│   │       └── PlayerControllerIntegrationVerification.md (MOVER PARA EXCLUIR)
│   │
│   ├── Gameplay/ (Sistemas de Gameplay)
│   │   ├── Quest/
│   │   │   ├── README.md (índice do sistema)
│   │   │   ├── QuestSystemQuickGuide.md
│   │   │   ├── QuestDialogueIntegration.md
│   │   │   └── Archive/ (documentos de implementação)
│   │   │       ├── Task10CompletionSummary.md
│   │   │       ├── Task13ImplementationNotes.md
│   │   │       ├── Task14FinalSummary.md
│   │   │       ├── Task14ImplementationSummary.md
│   │   │       ├── Task14Readme.md
│   │   │       ├── Task14TestCompletionReport.md
│   │   │       ├── Task15CompletionSummary.md
│   │   │       ├── QuestSystemTestCompletionSummary.md
│   │   │       ├── QuestSystemTestingIndex.md
│   │   │       ├── QuestSystemTestingQuickGuide.md
│   │   │       ├── QuestSystemTestInstructions.md
│   │   │       ├── QuestSystemTestReadme.md
│   │   │       ├── QuestSystemUiReadme.md
│   │   │       ├── QuestSystemVisualGuide.md
│   │   │       ├── QuestSystemManualTestChecklist.md
│   │   │       └── QuestSystemDocumentationIndex.md
│   │   │
│   │   ├── Dialogue/
│   │   │   ├── DIALOGUE_SYSTEM_README.md
│   │   │   ├── PlayerController-Integration.md (MOVER PARA EXCLUIR)
│   │   │   └── DialogueSystemSettingsVerification.md (MOVER PARA EXCLUIR)
│   │   │
│   │   ├── Crystal/
│   │   │   ├── Crystal_System_Implementation_Guide.md
│   │   │   └── Crystal_System_Debug_Guide.md
│   │   │
│   │   ├── NPC/
│   │   │   └── NPC-Drop-System-Guide.md
│   │   │
│   │   └── PushableObjects/
│   │       ├── Implementation-Guide.md
│   │       ├── QuickConfig-Guide.md
│   │       └── QuickConfig.md (DUPLICADO - EXCLUIR)
│   │
│   ├── UI/ (Sistemas de Interface)
│   │   ├── HUD/
│   │   │   ├── HeartHudSetup.md
│   │   │   └── HUD-ContextMenu-Guide.md
│   │   ├── Inventory/
│   │   │   └── InventoryUiSetup.md
│   │   ├── Menus/
│   │   │   └── PauseMenuSetup.md
│   │   └── Visual/
│   │       └── OutlineGuide.md
│   │
│   └── World/ (Sistemas de Mundo)
│       └── Transition/
│           └── Setup-Guide.md
│
├── 📁 03-Tools/ (Ferramentas de Desenvolvimento)
│   ├── Editor/
│   │   ├── ExtraTools/
│   │   │   ├── README.md (ExtraToolsReadme.md renomeado)
│   │   │   ├── QuickStart.md (QuickStartExtraTools.md renomeado)
│   │   │   ├── MenuStructure.md
│   │   │   └── Archive/
│   │   │       ├── Changelog.md
│   │   │       ├── ExecutiveSummary.md
│   │   │       ├── Index.md
│   │   │       ├── ResumoFinal.md
│   │   │       ├── UnificationSummary.md
│   │   │       └── NpcDialogueQuickConfigVerification.md
│   │   └── NPC/
│   │       └── NpcDialogueInteractionAdvancedConfigVerification.md (MOVER PARA EXCLUIR)
│   │
│   └── Workflows/ (Fluxos de Trabalho)
│       └── (vazio por enquanto)
│
└── 📁 _Archive/ (Documentos Obsoletos para Exclusão)
    ├── Verification/ (Documentos de verificação obsoletos)
    │   ├── DialogueSystemSettingsVerification.md
    │   ├── PlayerControllerIntegrationVerification.md
    │   └── NpcDialogueInteractionAdvancedConfigVerification.md
    │
    ├── Implementation/ (Notas de implementação antigas)
    │   └── Roadmap-Analysis-Summary.md
    │
    └── Deprecated/ (Sistemas descontinuados)
        └── JobSystemMigration.md
```

---

## 🗂️ Categorização de Arquivos

### ✅ Manter e Reorganizar

#### 📘 Documentação Principal do Projeto
- `Game-Design-Document.md` → `01-Project/GDD/`
- `Executive-Summary.md` → `01-Project/GDD/`
- `Roadmap.md` → `01-Project/Planning/`
- `ALPHA-1-Checklist.md` → `01-Project/Planning/`
- `BoasPraticas.md` → `01-Project/Standards/`
- `FolderStructure.md` → `01-Project/Standards/`
- `TechMapping.md` → `01-Project/Technical/`
- `ProjectSetup-Documentation.md` → `01-Project/Technical/`

#### 🏗️ Arquitetura e Core
- `Managers-Design-Document.md` → `02-Systems/Core/Architecture/`
- `GameManager-AutoCreation.md` → `02-Systems/Core/Architecture/`

#### 🎮 Sistemas de Gameplay
- **Quest System** (manter apenas essenciais):
  - `README.md` → `02-Systems/Gameplay/Quest/`
  - `QuestSystemQuickGuide.md` → `02-Systems/Gameplay/Quest/`
  - `QuestDialogueIntegration.md` → `02-Systems/Gameplay/Quest/`
  - Resto → `02-Systems/Gameplay/Quest/Archive/`

- **Dialogue System**:
  - `DIALOGUE_SYSTEM_README.md` → `02-Systems/Gameplay/Dialogue/`

- **Crystal System**:
  - `Crystal_System_Implementation_Guide.md` → `02-Systems/Gameplay/Crystal/`
  - `Crystal_System_Debug_Guide.md` → `02-Systems/Gameplay/Crystal/`

- **NPC System**:
  - `NPC-Drop-System-Guide.md` → `02-Systems/Gameplay/NPC/`

- **Pushable Objects**:
  - `Implementation-Guide.md` → `02-Systems/Gameplay/PushableObjects/`
  - `QuickConfig-Guide.md` → `02-Systems/Gameplay/PushableObjects/`

#### 🖥️ Sistemas de UI
- `HeartHudSetup.md` → `02-Systems/UI/HUD/`
- `HUD-ContextMenu-Guide.md` → `02-Systems/UI/HUD/`
- `InventoryUiSetup.md` → `02-Systems/UI/Inventory/`
- `PauseMenuSetup.md` → `02-Systems/UI/Menus/`
- `OutlineGuide.md` → `02-Systems/UI/Visual/`

#### 🌍 Sistemas de Mundo
- `Setup-Guide.md` (Transition) → `02-Systems/World/Transition/`

#### 🛠️ Ferramentas
- **ExtraTools**:
  - `ExtraToolsReadme.md` → `03-Tools/Editor/ExtraTools/README.md`
  - `QuickStartExtraTools.md` → `03-Tools/Editor/ExtraTools/QuickStart.md`
  - `MenuStructure.md` → `03-Tools/Editor/ExtraTools/`
  - Resto → `03-Tools/Editor/ExtraTools/Archive/`

---

### ❌ Mover para _Archive (Exclusão Futura)

#### Documentos de Verificação Obsoletos
- `DialogueSystemSettingsVerification.md` - Verificação de implementação concluída
- `PlayerControllerIntegrationVerification.md` - Verificação de integração obsoleta
- `NpcDialogueInteractionAdvancedConfigVerification.md` - Verificação obsoleta

#### Notas de Implementação Antigas
- `Roadmap-Analysis-Summary.md` - Análise pontual de Nov 2025, já incorporada ao Roadmap

#### Sistemas Descontinuados
- `JobSystemMigration.md` - Migração não avançou, não é mais prioridade

#### Documentos de Teste Detalhados (Quest System)
Todos os documentos de teste detalhado do Quest System (Task10-15) devem ir para Archive:
- 17 arquivos de testes, summaries e verificações
- Manter apenas: README.md, QuickGuide e Integration

---

## 📋 Ações Necessárias

### Fase 1: Criar Nova Estrutura
1. Criar pastas da nova estrutura
2. Criar README.md principal com índice

### Fase 2: Mover Arquivos Ativos
1. Mover documentos do projeto
2. Mover documentação de sistemas
3. Mover documentação de ferramentas
4. Atualizar links internos

### Fase 3: Arquivar Obsoletos
1. Criar pasta `_Archive/`
2. Mover documentos obsoletos
3. Adicionar README.md explicando o conteúdo

### Fase 4: Limpeza
1. Remover pastas vazias antigas
2. Verificar links quebrados
3. Atualizar README principal

---

## 🎯 Benefícios da Nova Estrutura

### ✅ Organização Clara
- Separação por tipo (Projeto, Sistemas, Ferramentas)
- Hierarquia lógica e intuitiva
- Fácil navegação

### ✅ Manutenibilidade
- Documentos obsoletos separados
- Histórico preservado em Archive
- Fácil adicionar novos sistemas

### ✅ Descoberta
- README.md como índice principal
- Estrutura de pastas autoexplicativa
- Documentos essenciais destacados

### ✅ Redução de Ruído
- Apenas documentos relevantes visíveis
- Verificações antigas arquivadas
- Duplicatas removidas

---

## 📊 Estatísticas

### Antes
- **Total de arquivos**: ~60 arquivos markdown
- **Estrutura**: 4 pastas principais + subpastas
- **Arquivos obsoletos**: ~20 arquivos (33%)
- **Duplicatas**: 2 arquivos

### Depois
- **Total de arquivos ativos**: ~30 arquivos markdown
- **Estrutura**: 3 pastas principais + organização lógica
- **Arquivos arquivados**: ~30 arquivos
- **Duplicatas**: 0 arquivos

### Melhoria
- **Redução de ruído**: 50%
- **Organização**: +100%
- **Facilidade de navegação**: +200%

---

## ✅ Próximos Passos

1. ⬜ Revisar e aprovar plano
2. ⬜ Executar Fase 1 (criar estrutura)
3. ⬜ Executar Fase 2 (mover ativos)
4. ⬜ Executar Fase 3 (arquivar obsoletos)
5. ⬜ Executar Fase 4 (limpeza)
6. ⬜ Atualizar referências no código
7. ⬜ Comunicar mudanças à equipe

---

**Criado em**: 28/11/2025  
**Status**: Aguardando aprovação  
**Estimativa de execução**: 2-3 horas
