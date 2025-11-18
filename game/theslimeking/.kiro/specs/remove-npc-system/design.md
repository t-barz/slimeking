# Design Document - Remoção do Sistema de NPCs

## Overview

Este documento descreve a abordagem para remover completamente o sistema de NPCs do projeto Unity "theslimeking". A remoção será feita de forma sistemática e segura, garantindo que nenhum código órfão ou referências quebradas permaneçam no projeto.

## Architecture

A remoção seguirá uma abordagem em camadas, começando pelos arquivos de código e terminando com a limpeza de metadados e especificações:

1. **Camada de Código**: Remoção de todos os arquivos C# relacionados a NPCs
2. **Camada de Metadados**: Remoção de arquivos .meta do Unity
3. **Camada de Documentação**: Remoção de documentos e guias
4. **Camada de Especificações**: Remoção do spec directory
5. **Verificação**: Validação de que não há dependências quebradas

## Components and Interfaces

### Arquivos de Código a Remover

#### Assets/Code/Gameplay/NPCs/

- NPCBehavior.cs
- NPCController.cs
- NPCDialogue.cs
- NPCDialogueInteraction.cs
- NPCEnums.cs
- NPCFriendship.cs

#### Assets/Code/Gameplay/NPCs/AI/

- NPCPatrolAI.cs
- NPCStaticAI.cs
- NPCWanderAI.cs

#### Assets/Code/Gameplay/NPCs/Data/

- DialogueData.cs
- FriendshipData.cs
- LocalizedDialogueData.cs
- NPCConfigData.cs
- NPCData.cs
- NPCEnums.cs (duplicado no Data)

#### Assets/Code/Systems/Data/

- NPCInteractionData.cs
- NPCDropData.cs

#### Assets/Code/Systems/Controllers/

- NPCAttributesHandler.cs

### Arquivos de Documentação a Remover

#### Assets/Docs/Tools/

- NPCQuickConfig-Testing-Guide.md
- NPCQuickConfig-Optimizations.md
- NPCQuickConfig-Performance-Summary.md

### Diretórios a Remover

- Assets/Code/Gameplay/NPCs/ (diretório completo incluindo subpastas)
- .kiro/specs/npc-system/ (diretório completo)

## Data Models

Não aplicável - esta é uma operação de remoção, não há novos modelos de dados.

## Error Handling

### Verificação de Dependências

Antes de considerar a remoção completa, o sistema deve:

1. Verificar se há referências a classes NPC em outros arquivos do projeto
2. Se encontradas, listar todas as referências para análise
3. Confirmar que o projeto compila após a remoção

### Tratamento de Erros de Compilação

Se após a remoção houver erros de compilação:

1. Identificar os arquivos que referenciam código NPC removido
2. Reportar ao desenvolvedor para decisão sobre como proceder
3. Não prosseguir com remoções adicionais até resolver dependências

### Arquivos .meta Órfãos

O Unity pode gerar warnings sobre arquivos .meta órfãos:

1. Remover arquivos .meta junto com seus arquivos correspondentes
2. Permitir que o Unity regenere metadados se necessário
3. Verificar que não há .meta files sem arquivos correspondentes

## Testing Strategy

### Verificação Pré-Remoção

1. Listar todos os arquivos que serão removidos
2. Buscar por referências a classes NPC em todo o codebase
3. Documentar o estado atual antes da remoção

### Verificação Pós-Remoção

1. Confirmar que todos os arquivos listados foram removidos
2. Verificar que os diretórios vazios foram removidos
3. Executar busca para confirmar que não há referências a NPCs restantes
4. Verificar que o projeto não tem erros de compilação

### Rollback Plan

Caso seja necessário reverter:

1. Usar controle de versão (git) para restaurar arquivos removidos
2. Documentar o motivo da reversão
3. Reavaliar a estratégia de remoção

## Implementation Notes

### Ordem de Remoção

A ordem recomendada para remoção é:

1. Arquivos de código em Assets/Code/Gameplay/NPCs/AI/
2. Arquivos de código em Assets/Code/Gameplay/NPCs/Data/
3. Arquivos de código em Assets/Code/Gameplay/NPCs/
4. Arquivos em Assets/Code/Systems/Data/ e Controllers/
5. Diretório completo Assets/Code/Gameplay/NPCs/
6. Arquivos de documentação em Assets/Docs/Tools/
7. Diretório de especificações .kiro/specs/npc-system/

### Considerações do Unity

- O Unity automaticamente remove arquivos .meta quando os arquivos correspondentes são deletados através do Editor
- Ao deletar via sistema de arquivos, os .meta devem ser removidos manualmente
- Após remoção, pode ser necessário reimportar assets no Unity

### Verificação Final

Após todas as remoções:

1. Executar busca global por "NPC" no código
2. Verificar se há assets de cena ou prefabs que referenciam componentes NPC
3. Confirmar que o projeto compila sem erros
4. Documentar o que foi removido
