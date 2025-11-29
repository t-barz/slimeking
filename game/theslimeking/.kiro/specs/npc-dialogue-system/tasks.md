# Implementation Plan - NPC Dialogue System

## Overview

Este plano de implementação detalha as tarefas necessárias para criar o sistema de diálogo de NPCs seguindo o princípio KISS. O sistema será implementado em etapas incrementais, com testes ao longo do caminho.

---

## Tasks

- [x] 1. Remover sistema antigo e preparar estrutura


  - Identificar e remover componentes do sistema antigo (NPCDialogueController)
  - Criar estrutura de pastas: Assets/Code/Dialogue/
  - Criar estrutura de testes: Assets/Code/Dialogue/Tests/
  - _Requirements: 9.1, 9.3_

- [x] 2. Implementar TypewriterEffect component


  - Criar classe TypewriterEffect.cs com campos configuráveis (charactersPerSecond, punctuationDelay)
  - Implementar método StartTyping() com coroutine para exibir caracteres sequencialmente
  - Implementar método CompleteInstantly() para pular animação
  - Implementar propriedade IsTyping para verificar estado
  - Adicionar suporte opcional para som de digitação
  - _Requirements: 2.1, 2.2, 2.4_

- [ ]* 2.1 Escrever property test para TypewriterEffect
  - **Property 5: Typewriter Sequential Display**
  - **Validates: Requirements 2.1**

- [ ]* 2.2 Escrever property test para instant completion
  - **Property 6: Instant Completion During Typing**
  - **Validates: Requirements 2.2**

- [x] 3. Implementar DialogueUI component


  - Criar classe DialogueUI.cs com referências UI (panel, text, indicator)
  - Implementar método Show() para exibir diálogo com lista de textos
  - Implementar método Hide() para fechar diálogo e limpar estado
  - Implementar método OnContinuePressed() para navegação
  - Implementar lógica de navegação entre textos (índice atual)
  - Integrar com TypewriterEffect para exibição de textos
  - Implementar controle de visibilidade do continue indicator
  - _Requirements: 1.2, 3.1, 3.2, 3.3, 3.4_

- [ ]* 3.1 Escrever property test para continue indicator
  - **Property 7: Continue Indicator Visibility**
  - **Validates: Requirements 3.1**

- [ ]* 3.2 Escrever property test para navegação de textos
  - **Property 8: Text Navigation Forward**
  - **Validates: Requirements 3.2**

- [ ]* 3.3 Escrever property test para invariante de índice
  - **Property 9: Current Text Index Invariant**
  - **Validates: Requirements 3.4**

- [x] 4. Implementar DialogueNPC component


  - Criar classe DialogueNPC.cs com lista de LocalizedString
  - Implementar campo interactionRadius e UnityEvent onDialogueComplete
  - Implementar método StartDialogue() para iniciar diálogo
  - Implementar detecção de proximidade do jogador (OnTriggerEnter2D/Exit2D)
  - Implementar controle de visibilidade do interaction indicator
  - Adicionar validação para lista vazia de textos
  - _Requirements: 1.1, 1.2, 4.1, 4.4, 6.1_

- [ ]* 4.1 Escrever property test para raio de interação
  - **Property 1: Interaction Radius Consistency**
  - **Validates: Requirements 1.1**

- [ ]* 4.2 Escrever property test para abertura de diálogo
  - **Property 2: Dialogue Opening Behavior**
  - **Validates: Requirements 1.2**

- [x] 5. Criar DialogueCanvas prefab


  - Criar Canvas com Screen Space Overlay e sorting order 100
  - Adicionar Canvas Scaler com reference resolution 1920x1080
  - Criar DialoguePanel com Image component
  - Configurar ui_dialogBackground.png como sprite do background
  - Adicionar TextMeshProUGUI para texto do diálogo
  - Criar continue indicator (imagem ou GameObject animado)
  - Adicionar componente DialogueUI ao Canvas
  - Adicionar componente TypewriterEffect ao texto
  - Configurar referências entre componentes
  - Salvar como prefab em Assets/Game/Prefabs/UI/DialogueCanvas.prefab
  - _Requirements: 7.1, 7.2, 7.3, 7.4_

- [x] 6. Implementar integração com Unity Localization


  - Adicionar suporte para carregar LocalizedString de forma assíncrona
  - Implementar fallback para textos não localizados
  - Testar com múltiplos idiomas (se disponível)
  - Adicionar logs de warning para falhas de localização
  - _Requirements: 4.1, 4.2, 4.3_

- [x] 7. Implementar controle de player durante diálogo


  - Adicionar lógica para pausar/limitar movimento do jogador quando diálogo abre
  - Adicionar lógica para restaurar controle quando diálogo fecha
  - Implementar através de evento ou referência direta ao player controller
  - _Requirements: 1.3, 1.4_

- [ ]* 7.1 Escrever property test para controle do player
  - **Property 3: Player Control State During Dialogue**
  - **Validates: Requirements 1.3**

- [ ]* 7.2 Escrever property test para restauração de controle
  - **Property 4: Dialogue Closing Restores Control**
  - **Validates: Requirements 1.4**

- [x] 8. Implementar sistema de eventos ao completar diálogo


  - Adicionar invocação de UnityEvent ao final do diálogo
  - Garantir que eventos são invocados antes de fechar o Canvas
  - Adicionar tratamento para lista vazia de eventos
  - Testar ordem de invocação de múltiplos eventos
  - _Requirements: 6.2, 6.3, 6.4_

- [ ]* 8.1 Escrever property test para ordem de eventos
  - **Property 11: Event Invocation Order**
  - **Validates: Requirements 6.2**

- [x] 9. Criar ferramenta Setup Dialogue NPC no Extra Tools


  - Adicionar menu item "Extra Tools >> Setup >> 💬 Setup Dialogue NPC"
  - Implementar DialogueSetupTool.SetupDialogueNPC() method
  - Adicionar DialogueNPC component ao GameObject selecionado
  - Configurar BoxCollider2D como trigger com tamanho apropriado
  - Buscar DialogueCanvas na cena ou criar se não existir
  - Configurar referências entre NPC e Canvas
  - Adicionar entrada de localização padrão
  - Implementar validação para evitar duplicação de componentes
  - Adicionar logs informativos de sucesso/erro
  - _Requirements: 5.1, 5.2, 5.3, 5.4_

- [ ]* 9.1 Escrever property test para idempotência do setup
  - **Property 10: Setup Idempotence**
  - **Validates: Requirements 5.4**

- [x] 10. Configurar NPC art_rickA como primeiro exemplo


  - Abrir cena 3_InitialForest
  - Selecionar GameObject art_rickA
  - Executar "Extra Tools >> Setup >> 💬 Setup Dialogue NPC"
  - Configurar textos de diálogo localizados para art_rickA
  - Testar interação completa com o NPC
  - Verificar que todos os componentes funcionam corretamente
  - _Requirements: Todos (teste end-to-end)_

- [x] 11. Criar ferramenta de migração/limpeza do sistema antigo


  - Criar script editor para encontrar NPCs com componentes antigos
  - Implementar lógica para remover NPCDialogueController
  - Implementar lógica para migrar dados quando possível
  - Gerar relatório de migração com lista de NPCs afetados
  - Adicionar menu item "Extra Tools >> Setup >> 🔄 Migrate Old Dialogue System"
  - _Requirements: 9.1, 9.2, 9.3, 9.4_

- [x] 12. Checkpoint - Garantir que todos os testes passam



  - Executar todos os property tests
  - Executar todos os unit tests
  - Verificar que não há erros no console
  - Testar interação completa com art_rickA
  - Ensure all tests pass, ask the user if questions arise.

- [ ] 13. Criar documentação do sistema

  - Criar README.md em Assets/Code/Dialogue/
  - Documentar quick start guide
  - Adicionar exemplos práticos de uso
  - Documentar troubleshooting comum
  - Adicionar XML documentation em todos os métodos públicos
  - Adicionar tooltips em todos os campos do Inspector
  - _Requirements: 8.4_

- [ ]* 14. Testes de integração end-to-end
  - Criar cena de teste com NPC configurado
  - Testar fluxo completo: aproximação → interação → navegação → eventos → fechamento
  - Testar com múltiplos NPCs na mesma cena
  - Testar edge cases (lista vazia, eventos null, etc.)
  - Testar mudança de idioma durante diálogo
  - _Requirements: Todos_

---

## Notes

### Testing Strategy

- **Property-based tests** serão implementados usando NUnit com geração de dados aleatórios
- Cada property test deve executar no mínimo **100 iterações**
- Testes marcados com * são opcionais mas recomendados para garantir qualidade

### Implementation Order

As tarefas estão ordenadas para permitir desenvolvimento incremental:
1. Remover sistema antigo (limpeza)
2. Implementar componentes core (TypewriterEffect → DialogueUI → DialogueNPC)
3. Criar UI (DialogueCanvas prefab)
4. Integrar sistemas (Localization, Player Control, Events)
5. Criar ferramentas (Setup Tool, Migration Tool)
6. Testar e documentar

### KISS Principle

Manter simplicidade em todas as implementações:
- Evitar abstrações desnecessárias
- Usar apenas 3 componentes principais
- Código direto e fácil de entender
- Sem over-engineering

### Dependencies

- Unity Localization Package (já instalado)
- TextMeshPro (já instalado)
- ui_dialogBackground.png (já existe no projeto)

### Success Criteria

O sistema estará completo quando:
- ✅ art_rickA exibe diálogo com typewriter effect
- ✅ Navegação entre textos funciona corretamente
- ✅ Textos são localizados
- ✅ Setup tool funciona automaticamente
- ✅ Sistema antigo foi completamente removido
- ✅ Todos os testes passam
- ✅ Documentação está completa
