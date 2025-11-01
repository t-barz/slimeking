# Implementation Plan

- [x] 1. Criar estruturas de dados para diálogos localizados
  - Criar `LocalizedDialogueData.cs` com estrutura para páginas e múltiplos idiomas
  - Implementar método `GetPages(languageCode)` com lógica de fallback
  - Adicionar enum para códigos de idioma suportados
  - _Requirements: 5.2, 5.3, 5.5, 5.6_

- [x] 2. Implementar LocalizationManager
  - Criar classe `LocalizationManager` herdando de `ManagerSingleton`
  - Implementar carregamento de arquivos JSON do diretório de diálogos
  - Implementar sistema de cache para diálogos carregados
  - Implementar método `GetLocalizedDialogue(dialogueId)` com fallback de idioma
  - Implementar métodos `SetLanguage()` e `GetCurrentLanguage()`
  - Adicionar tratamento de erros para JSON malformado ou não encontrado
  - _Requirements: 5.1, 5.4, 5.5, 5.6, 5.7_

- [x] 3. Implementar DialogueManager
  - Criar classe `DialogueManager` herdando de `ManagerSingleton`
  - Implementar método `StartDialogue(dialogueId)` que carrega dados via LocalizationManager
  - Implementar controle de paginação com `NextPage()` e rastreamento de página atual
  - Implementar método `EndDialogue()` que limpa estado e notifica componentes
  - Adicionar flag `IsDialogueActive()` para controle de estado global
  - Implementar integração com DialogueUI para exibir conteúdo
  - _Requirements: 3.1, 3.2, 3.3, 3.4, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_

- [x] 4. Criar componente InteractionIcon
  - Criar script `InteractionIcon.cs` para ícone de interação
  - Implementar métodos `Show()` e `Hide()` com animações de fade
  - Implementar `SetTarget()` para seguir posição do NPC em world space
  - Adicionar animação de bounce/pulse usando Animator ou código
  - Implementar conversão de world space para screen space para posicionamento
  - _Requirements: 2.1, 2.2, 2.5_

- [x] 5. Criar prefab do InteractionIcon
  - Criar prefab com Canvas e Image para o ícone
  - Adicionar componente `InteractionIcon`
  - Configurar sprite do ícone (botão "E" ou símbolo de interação)
  - Adicionar Animator com animação de bounce
  - Configurar posicionamento e escala adequados
  - _Requirements: 2.1, 2.5_

- [x] 6. Implementar DialogueUI com efeito typewriter
  - Criar script `DialogueUI.cs` para gerenciar UI da caixa de diálogo
  - Implementar método `ShowDialogue(text, hasMorePages)` que exibe a caixa
  - Implementar efeito typewriter usando coroutine que revela texto caractere por caractere
  - Implementar `CompleteCurrentText()` para pular animação e mostrar texto completo
  - Adicionar indicador visual de continuação quando há mais páginas
  - Implementar `HideDialogue()` com animação de fade out
  - Adicionar propriedades configuráveis para velocidade do typewriter
  - _Requirements: 3.1, 3.5, 4.1, 4.2, 4.3, 4.4, 4.5, 6.5_

- [x] 7. Criar arquivos JSON de exemplo
  - Criar diretório `Assets/Data/Dialogues/`
  - Criar arquivo JSON de exemplo com estrutura completa
  - Incluir múltiplos idiomas (BR, EN, ES) no exemplo
  - Incluir exemplo com página única e exemplo com múltiplas páginas
  - Adicionar documentação README.md no diretório
  - _Requirements: 5.1, 5.2, 5.3_

- [x] 8. Criar prefab da DialogueUI
  - Criar Canvas com caixa de diálogo usando UI do Unity
  - Adicionar TextMeshProUGUI para exibir o texto do diálogo
  - Adicionar GameObject para indicador de continuação (seta ou ícone)
  - Adicionar componente `DialogueUI` e conectar referências
  - Configurar layout responsivo e posicionamento na tela
  - Adicionar painel de fundo semi-transparente com CanvasGroup
  - Salvar prefab em `Assets/Game/Prefabs/UI/DialogueUI.prefab`
  - _Requirements: 3.1, 3.5_

- [x] 9. Implementar NPCDialogueInteraction

  - Criar script `NPCDialogueInteraction.cs` em `Assets/Code/Gameplay/NPCs/`
  - Implementar detecção de proximidade usando CircleCollider2D trigger
  - Implementar `OnTriggerEnter2D()` e `OnTriggerExit2D()` para detectar jogador (tag "Player")
  - Adicionar controle do ícone de interação (instanciar, mostrar, ocultar)
  - Implementar verificação de input para iniciar diálogo via DialogueManager
  - Adicionar propriedades configuráveis: dialogueId, interactionRadius, iconAnchor, interactionIconPrefab
  - Implementar gizmos para visualizar raio de interação no editor
  - Adicionar integração com DialogueManager para pausar detecção durante diálogo ativo
  - _Requirements: 1.3, 2.1, 2.2, 2.3, 2.4, 3.1, 7.4_

- [x] 10. Implementar NPCDialogueQuickConfig (Editor Tool)
  - Criar script de editor `NPCDialogueQuickConfig.cs` em `Assets/Code/Editor/`
  - Adicionar menu item "GameObject/SlimeKing/Configure as Dialogue NPC"
  - Implementar `ConfigureAsDialogueNPC()` que adiciona componentes necessários
  - Adicionar `NPCDialogueInteraction` ao GameObject selecionado
  - Adicionar `CircleCollider2D` configurado como trigger com raio padrão
  - Carregar e atribuir prefab do InteractionIcon de `Assets/Game/Prefabs/UI/`
  - Configurar valores padrão (raio de interação 2.5f, referências)
  - Implementar validação para não duplicar componentes existentes
  - Seguir padrão similar a `BushQuickConfig` e `ItemQuickConfig` existentes
  - _Requirements: 1.1, 1.2, 1.3, 1.4_

- [x] 11. Criar DialogueSystemSettings ScriptableObject
  - Criar `DialogueSystemSettings.cs` em `Assets/Code/Systems/`
  - Adicionar campos: dialoguesPath, defaultTypewriterSpeed, defaultInteractionRadius, defaultLanguage
  - Adicionar referências: dialogueUIPrefab, interactionIconPrefab
  - Adicionar campo: interactionButtonName
  - Criar instância padrão do asset em `Assets/Data/Settings/DialogueSystemSettings.asset`
  - Configurar referências aos prefabs criados (DialogueUI e InteractionIcon)
  - Adicionar validação de configurações no editor usando OnValidate
  - _Requirements: 7.1, 7.2, 7.3_

- [x] 12. Integrar sistema de input para interação
  - Verificar configuração do Input Manager do Unity para botão "Interact"
  - Se não existir, adicionar configuração para tecla "E" como botão "Interact"
  - Atualizar `NPCDialogueInteraction` para usar Input.GetButtonDown("Interact")
  - Verificar que `DialogueUI` já está usando Input.GetButtonDown para avançar páginas
  - Adicionar configuração do nome do botão via DialogueSystemSettings
  - Testar input em modo Play
  - _Requirements: 2.3, 4.3, 4.4, 6.2, 6.3, 6.4_

- [x] 13. Implementar integração com PlayerController
  - Identificar se PlayerController existe no projeto (atualmente não existe)
  - Se não existir, criar stub methods em DialogueManager para pausar/despausar jogador
  - Adicionar comentários TODO indicando onde integrar com PlayerController futuro
  - Implementar detecção de proximidade em NPCDialogueInteraction usando tag "Player"
  - Adicionar configuração opcional `pausePlayerDuringDialogue` (já existe em DialogueManager)
  - Documentar que integração completa requer PlayerController implementado
  - _Requirements: 3.3, 3.4_

- [x] 14. Adicionar suporte para configurações avançadas

  - Adicionar campo opcional `customTypewriterSpeed` em NPCDialogueInteraction

  - Adicionar campo `pausePlayerDuringDialogue` em NPCDialogueInteraction (override do global)
  - Implementar lógica para usar configurações customizadas quando definidas, senão usar defaults
  - Adicionar tooltips explicativos para todos os campos configuráveis
  - _Requirements: 2.4, 4.5, 7.1, 7.2_

- [x] 15. Criar cena de teste e documentação final

  - Criar cena de teste `DialogueSystemTest` em `Assets/Game/Scenes/Tests/`
  - Adicionar 2-3 NPCs configurados com diálogos diferentes
  - Configurar um NPC com diálogo de página única
  - Configurar um NPC com diálogo de múltiplas páginas
  - Adicionar GameObject com DialogueManager e LocalizationManager
  - Testar troca de idiomas (BR, EN, ES)
  - Criar arquivo `DIALOGUE_SYSTEM_README.md` em `Assets/Docs/` com:
    - Instruções de uso do Quick Setup
    - Como criar novos diálogos JSON
    - Como configurar NPCs manualmente
    - Troubleshooting comum
  - Atualizar README.md do projeto com link para documentação do sistema de diálogos
  - _Requirements: 1.1, 1.2, 5.1, 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_
