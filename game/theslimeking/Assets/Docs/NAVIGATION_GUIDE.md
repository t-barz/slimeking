# 🧭 Guia de Navegação da Documentação

Este guia rápido ajuda você a encontrar o que precisa na nova estrutura de documentação.

---

## 🚀 Início Rápido

### Sou Novo no Projeto

1. **[README.md](README.md)** - Comece aqui! Visão geral completa
2. **[BoasPraticas.md](01-Project/Standards/BoasPraticas.md)** - Padrões de código
3. **[Game-Design-Document.md](01-Project/GDD/Game-Design-Document.md)** - Entenda o jogo
4. **[Roadmap.md](01-Project/Planning/Roadmap.md)** - Veja o planejamento

### Vou Implementar uma Feature

1. **[Roadmap.md](01-Project/Planning/Roadmap.md)** - Verifique prioridades
2. **[ALPHA-1-Checklist.md](01-Project/Planning/ALPHA-1-Checklist.md)** - Veja o que falta
3. **[TechMapping.md](01-Project/Technical/TechMapping.md)** - Entenda a arquitetura
4. **[BoasPraticas.md](01-Project/Standards/BoasPraticas.md)** - Siga os padrões

### Preciso Consultar um Sistema

- **Quest System**: [02-Systems/Gameplay/Quest/README.md](02-Systems/Gameplay/Quest/README.md)
- **Dialogue System**: [02-Systems/Gameplay/Dialogue/DIALOGUE_SYSTEM_README.md](02-Systems/Gameplay/Dialogue/DIALOGUE_SYSTEM_README.md)
- **Crystal System**: [02-Systems/Gameplay/Crystal/Crystal_System_Implementation_Guide.md](02-Systems/Gameplay/Crystal/Crystal_System_Implementation_Guide.md)
- **UI Systems**: [02-Systems/UI/](02-Systems/UI/)

### Preciso de Ferramentas

- **ExtraTools**: [03-Tools/Editor/ExtraTools/README.md](03-Tools/Editor/ExtraTools/README.md)
- **Quick Start**: [03-Tools/Editor/ExtraTools/QuickStart.md](03-Tools/Editor/ExtraTools/QuickStart.md)

---

## 📂 Estrutura por Categoria

### 📘 Documentação do Projeto

**Quando usar**: Entender o projeto, planejamento, padrões

```
01-Project/
├── GDD/ ..................... Game Design
│   ├── Game-Design-Document.md
│   └── Executive-Summary.md
├── Planning/ ................ Roadmap e Checklists
│   ├── Roadmap.md ⭐
│   └── ALPHA-1-Checklist.md
├── Standards/ ............... Padrões de Código
│   ├── BoasPraticas.md ⭐
│   └── FolderStructure.md
└── Technical/ ............... Arquitetura Técnica
    ├── TechMapping.md ⭐
    └── ProjectSetup-Documentation.md
```

### 🎮 Documentação de Sistemas

**Quando usar**: Implementar ou consultar sistemas específicos

```
02-Systems/
├── Core/ .................... Sistemas Principais
│   └── Architecture/
│       ├── Managers-Design-Document.md
│       └── GameManager-AutoCreation.md
├── Gameplay/ ................ Mecânicas de Jogo
│   ├── Quest/ ⭐
│   ├── Dialogue/ ⭐
│   ├── Crystal/
│   ├── NPC/
│   └── PushableObjects/
├── UI/ ...................... Interface
│   ├── HUD/
│   ├── Inventory/
│   ├── Menus/
│   └── Visual/
└── World/ ................... Sistemas de Mundo
    └── Transition/
```

### 🛠️ Ferramentas

**Quando usar**: Acelerar desenvolvimento com ferramentas de editor

```
03-Tools/
└── Editor/
    └── ExtraTools/ ⭐
        ├── README.md
        ├── QuickStart.md
        └── MenuStructure.md
```

---

## 🔍 Encontrar por Tipo de Documento

### 📖 Guias de Implementação

- [Crystal System Implementation](02-Systems/Gameplay/Crystal/Crystal_System_Implementation_Guide.md)
- [Pushable Objects Implementation](02-Systems/Gameplay/PushableObjects/Implementation-Guide.md)

### ⚙️ Guias de Setup

- [Heart HUD Setup](02-Systems/UI/HUD/HeartHudSetup.md)
- [Inventory UI Setup](02-Systems/UI/Inventory/InventoryUiSetup.md)
- [Pause Menu Setup](02-Systems/UI/Menus/PauseMenuSetup.md)
- [Transition System Setup](02-Systems/World/Transition/Setup-Guide.md)

### 🚀 Guias Rápidos (Quick Guides)

- [Quest System Quick Guide](02-Systems/Gameplay/Quest/QuestSystemQuickGuide.md)
- [ExtraTools Quick Start](03-Tools/Editor/ExtraTools/QuickStart.md)
- [Pushable Objects Quick Config](02-Systems/Gameplay/PushableObjects/QuickConfig-Guide.md)

### 🐛 Guias de Debug

- [Crystal System Debug Guide](02-Systems/Gameplay/Crystal/Crystal_System_Debug_Guide.md)

### 🔗 Guias de Integração

- [Quest Dialogue Integration](02-Systems/Gameplay/Quest/QuestDialogueIntegration.md)

### 📋 READMEs de Sistema

- [Quest System README](02-Systems/Gameplay/Quest/README.md)
- [Dialogue System README](02-Systems/Gameplay/Dialogue/DIALOGUE_SYSTEM_README.md)
- [ExtraTools README](03-Tools/Editor/ExtraTools/README.md)

---

## 🎯 Encontrar por Tarefa

### Implementar Nova Feature

1. Consulte **[Roadmap.md](01-Project/Planning/Roadmap.md)** - Está planejado?
2. Veja **[ALPHA-1-Checklist.md](01-Project/Planning/ALPHA-1-Checklist.md)** - É prioridade?
3. Leia **[BoasPraticas.md](01-Project/Standards/BoasPraticas.md)** - Padrões
4. Consulte **[TechMapping.md](01-Project/Technical/TechMapping.md)** - Arquitetura

### Configurar Sistema Existente

1. Vá para **[02-Systems/](02-Systems/)** - Encontre o sistema
2. Leia o **README.md** ou **Guide.md** do sistema
3. Siga o **Setup.md** se disponível
4. Use **Quick Guide** para referência rápida

### Usar Ferramenta de Editor

1. Vá para **[03-Tools/Editor/](03-Tools/Editor/)**
2. Leia **[ExtraTools/README.md](03-Tools/Editor/ExtraTools/README.md)**
3. Siga **[QuickStart.md](03-Tools/Editor/ExtraTools/QuickStart.md)**
4. Consulte **[MenuStructure.md](03-Tools/Editor/ExtraTools/MenuStructure.md)**

### Debugar Problema

1. Consulte **Debug Guide** do sistema específico
2. Veja **[BoasPraticas.md](01-Project/Standards/BoasPraticas.md)** - Sistema de logs
3. Use **[TechMapping.md](01-Project/Technical/TechMapping.md)** - Entenda dependências

### Entender Decisão de Design

1. Leia **[Game-Design-Document.md](01-Project/GDD/Game-Design-Document.md)**
2. Consulte **[Roadmap.md](01-Project/Planning/Roadmap.md)** - Contexto
3. Veja **[_Archive/](\_Archive/)** - Decisões históricas (se necessário)

---

## 📊 Documentos por Prioridade

### ⭐ Essenciais (Leia Primeiro)

1. **[README.md](README.md)** - Índice principal
2. **[BoasPraticas.md](01-Project/Standards/BoasPraticas.md)** - Padrões obrigatórios
3. **[Roadmap.md](01-Project/Planning/Roadmap.md)** - Planejamento atual
4. **[TechMapping.md](01-Project/Technical/TechMapping.md)** - Arquitetura

### 🔥 Importantes (Consulte Frequentemente)

1. **[ALPHA-1-Checklist.md](01-Project/Planning/ALPHA-1-Checklist.md)** - Tarefas atuais
2. **[Quest System README](02-Systems/Gameplay/Quest/README.md)** - Sistema principal
3. **[Dialogue System README](02-Systems/Gameplay/Dialogue/DIALOGUE_SYSTEM_README.md)** - Sistema principal
4. **[ExtraTools README](03-Tools/Editor/ExtraTools/README.md)** - Ferramentas úteis

### 📖 Referência (Consulte Quando Necessário)

1. **[Game-Design-Document.md](01-Project/GDD/Game-Design-Document.md)** - Design completo
2. **[FolderStructure.md](01-Project/Standards/FolderStructure.md)** - Organização
3. **Guias de Setup** - Configuração de sistemas
4. **Guias de Debug** - Solução de problemas

### 🗃️ Arquivo (Raramente Necessário)

1. **[_Archive/](\_Archive/)** - Documentos históricos
2. **Quest/Archive/** - Testes detalhados
3. **ExtraTools/Archive/** - Documentos antigos

---

## 💡 Dicas de Navegação

### Use o README Principal

O **[README.md](README.md)** tem links para tudo. Marque como favorito!

### Busque por Palavra-Chave

Use Ctrl+F no README para encontrar rapidamente:
- Nome do sistema (ex: "Quest", "Dialogue")
- Tipo de documento (ex: "Setup", "Guide", "README")
- Categoria (ex: "Gameplay", "UI", "Tools")

### Siga a Hierarquia

```
README.md (índice)
  ↓
Categoria (01-Project, 02-Systems, 03-Tools)
  ↓
Subcategoria (GDD, Planning, Gameplay, UI)
  ↓
Sistema específico (Quest, Dialogue, Crystal)
  ↓
Documento específico (README, Guide, Setup)
```

### Use os Símbolos

- ⭐ = Documento essencial
- ✅ = Sistema completo
- 🚧 = Em desenvolvimento
- 📋 = Planejado

---

## 🔗 Links Rápidos

### Mais Consultados

- [Roadmap](01-Project/Planning/Roadmap.md)
- [Boas Práticas](01-Project/Standards/BoasPraticas.md)
- [Quest System](02-Systems/Gameplay/Quest/README.md)
- [Dialogue System](02-Systems/Gameplay/Dialogue/DIALOGUE_SYSTEM_README.md)
- [ExtraTools](03-Tools/Editor/ExtraTools/README.md)

### Por Sistema

- [Quest](02-Systems/Gameplay/Quest/)
- [Dialogue](02-Systems/Gameplay/Dialogue/)
- [Crystal](02-Systems/Gameplay/Crystal/)
- [NPC](02-Systems/Gameplay/NPC/)
- [UI](02-Systems/UI/)

### Por Tipo

- [Guias de Setup](02-Systems/UI/)
- [Guias Rápidos](02-Systems/Gameplay/Quest/QuestSystemQuickGuide.md)
- [Ferramentas](03-Tools/Editor/)

---

## ❓ Não Encontrou o que Procura?

1. **Verifique o [README.md](README.md)** - Índice completo
2. **Busque no [TechMapping.md](01-Project/Technical/TechMapping.md)** - Mapeamento técnico
3. **Consulte o [Roadmap.md](01-Project/Planning/Roadmap.md)** - Pode estar planejado
4. **Veja o [_Archive/](\_Archive/)** - Pode estar arquivado

---

**Última Atualização**: 28/11/2025  
**Versão**: 1.0  
**Mantido por**: Equipe The Slime King
