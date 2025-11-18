# Requirements Document

## Introduction

Este documento define os requisitos para a remoção completa do sistema de NPCs do projeto. O sistema atual foi considerado difícil de utilizar, difícil de entender e com resultados insatisfatórios. A remoção deve ser completa e limpa, eliminando todos os arquivos, código e documentação relacionados aos NPCs.

## Glossary

- **NPC System**: O conjunto completo de código, dados, documentação e especificações relacionados aos personagens não-jogáveis (NPCs) do jogo
- **Game Project**: O projeto Unity "theslimeking" do qual o sistema de NPCs será removido
- **Code Files**: Arquivos C# (.cs) que implementam funcionalidades de NPCs
- **Meta Files**: Arquivos .meta do Unity que contêm metadados sobre assets
- **Spec Directory**: Diretório .kiro/specs/npc-system contendo documentação de especificação
- **Documentation Files**: Arquivos markdown (.md) na pasta Assets/Docs/Tools relacionados a NPCs

## Requirements

### Requirement 1

**User Story:** Como desenvolvedor, quero remover todos os arquivos de código relacionados a NPCs, para que o projeto não contenha mais implementações de NPCs.

#### Acceptance Criteria

1. THE Game Project SHALL remove all C# files located in Assets/Code/Gameplay/NPCs directory
2. THE Game Project SHALL remove all C# files in Assets/Code/Gameplay/NPCs/AI subdirectory
3. THE Game Project SHALL remove all C# files in Assets/Code/Gameplay/NPCs/Data subdirectory
4. THE Game Project SHALL remove the file Assets/Code/Systems/Data/NPCInteractionData.cs
5. THE Game Project SHALL remove the file Assets/Code/Systems/Data/NPCDropData.cs
6. THE Game Project SHALL remove the file Assets/Code/Systems/Controllers/NPCAttributesHandler.cs

### Requirement 2

**User Story:** Como desenvolvedor, quero remover todos os arquivos meta do Unity relacionados a NPCs, para manter a consistência do projeto Unity.

#### Acceptance Criteria

1. THE Game Project SHALL remove all .meta files corresponding to deleted NPC code files
2. THE Game Project SHALL remove the Assets/Code/Gameplay/NPCs directory and its .meta file
3. WHEN all NPC files are deleted, THE Game Project SHALL ensure no orphaned .meta files remain

### Requirement 3

**User Story:** Como desenvolvedor, quero remover toda a documentação relacionada a NPCs, para que não haja confusão sobre funcionalidades removidas.

#### Acceptance Criteria

1. THE Game Project SHALL remove the file Assets/Docs/Tools/NPCQuickConfig-Testing-Guide.md
2. THE Game Project SHALL remove the file Assets/Docs/Tools/NPCQuickConfig-Optimizations.md
3. THE Game Project SHALL remove the file Assets/Docs/Tools/NPCQuickConfig-Performance-Summary.md
4. THE Game Project SHALL remove all .meta files corresponding to deleted documentation files

### Requirement 4

**User Story:** Como desenvolvedor, quero remover as especificações do sistema de NPCs, para limpar o histórico de planejamento de features removidas.

#### Acceptance Criteria

1. THE Game Project SHALL remove the directory .kiro/specs/npc-system
2. THE Game Project SHALL remove the file .kiro/specs/npc-system/requirements.md
3. THE Game Project SHALL remove the file .kiro/specs/npc-system/design.md
4. THE Game Project SHALL remove the file .kiro/specs/npc-system/tasks.md

### Requirement 5

**User Story:** Como desenvolvedor, quero verificar que não há dependências quebradas após a remoção, para garantir que o projeto continue compilando.

#### Acceptance Criteria

1. WHEN all NPC files are removed, THE Game Project SHALL verify no compilation errors exist
2. IF other code references NPC classes, THEN THE Game Project SHALL identify these references
3. THE Game Project SHALL provide a summary of all files and directories removed
