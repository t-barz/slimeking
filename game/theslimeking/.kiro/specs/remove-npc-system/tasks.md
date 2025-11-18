# Implementation Plan - Remoção do Sistema de NPCs

- [x] 1. Verificar dependências antes da remoção

  - Executar busca por referências a classes NPC em todo o codebase (excluindo a pasta NPCs)
  - Verificar se há prefabs ou cenas que usam componentes NPC
  - Documentar todas as dependências encontradas
  - _Requirements: 5.2_
-

- [x] 2. Remover arquivos de código NPC em subdiretórios

  - Deletar todos os arquivos .cs em Assets/Code/Gameplay/NPCs/AI/
  - Deletar todos os arquivos .cs em Assets/Code/Gameplay/NPCs/Data/
  - Deletar todos os arquivos .meta correspondentes nas subpastas
  - _Requirements: 1.2, 1.3, 2.1_

- [x] 3. Remover arquivos de código NPC principais

  - Deletar NPCBehavior.cs e seu .meta
  - Deletar NPCController.cs e seu .meta
  - Deletar NPCDialogue.cs e seu .meta
  - Deletar NPCDialogueInteraction.cs e seu .meta
  - Deletar NPCEnums.cs e seu .meta
  - Deletar NPCFriendship.cs e seu .meta
  - _Requirements: 1.1, 2.1_
-

- [x] 4. Remover arquivos NPC em Systems

  - Deletar Assets/Code/Systems/Data/NPCInteractionData.cs e seu .meta
  - Deletar Assets/Code/Systems/Data/NPCDropData.cs e seu .meta
  - Deletar Assets/Code/Systems/Controllers/NPCAttributesHandler.cs e seu .meta
  - _Requirements: 1.4, 1.5, 1.6, 2.1_

- [x] 5. Remover diretório NPCs completo

  - Deletar o diretório Assets/Code/Gameplay/NPCs/ e seu .meta
  - Verificar que não há arquivos órfãos restantes
  - _Requirements: 2.2, 2.3_

- [x] 6. Remover documentação de NPCs

  - Deletar Assets/Docs/Tools/NPCQuickConfig-Testing-Guide.md e seu .meta
  - Deletar Assets/Docs/Tools/NPCQuickConfig-Optimizations.md e seu .meta
  - Deletar Assets/Docs/Tools/NPCQuickConfig-Performance-Summary.md e seu .meta
  - _Requirements: 3.1, 3.2, 3.3, 3.4_
-

- [x] 7. Remover especificações do sistema NPC

  - Deletar .kiro/specs/npc-system/requirements.md
  - Deletar .kiro/specs/npc-system/design.md
  - Deletar .kiro/specs/npc-system/tasks.md
  - Deletar o diretório .kiro/specs/npc-system/
  - _Requirements: 4.1, 4.2, 4.3, 4.4_
- [x] 8. Verificar integridade do projeto após remoção

- [ ] 8. Verificar integridade do projeto após remoção

  - Executar busca global por "NPC" para confirmar remoção completa
  - Verificar se há erros de compilação no projeto
  - Listar todos os arquivos e diretórios removidos
  - Confirmar que não há referências quebradas
  - _Requirements: 5.1, 5.2, 5.3_
