# Sistema de Diálogo Localizado para The Slime King
## Estrutura de Classes e Arquitetura (v3)

---

## 1. CAMADA DE DADOS (Data Layer)

### 1.1 DialogueEntry
**Responsabilidade**: Representar uma unidade individual de fala de um NPC.

**Atributos**:
- `id`: String única que identifica a fala (ex: "slime_king_greeting_001")
- `npcId`: Identificador do NPC que fala
- `textKey`: Chave para localização do texto (ex: "dialogue.slime_king.greeting_001")
- `audioClipKey`: Chave para recuperar um AudioClip único (ex: "dialogue.slime_king.sfx_001") - **não localizado**
- `prerequisites`: Lista de condições que devem ser atendidas para exibir esta fala
- `flags_to_activate`: Lista de flags ativadas após esta fala
- `flags_to_deactivate`: Lista de flags desativadas após esta fala
- `nextDialogueId`: ID da próxima fala na sequência (permite encadeamento linear)
- `branches`: Lista de diálogos alternativos baseados em escolhas do jogador
- `metadata`: Dicionário com informações adicionais (emoção, tom, etc.)

**Função Principal**: Encapsular os dados de uma única linha de diálogo com todas as metadadas necessárias, usando chaves para localização de texto e referências diretas para áudio único (não localizado).

---

### 1.2 DialogueDatabase
**Responsabilidade**: Gerenciar o armazenamento e recuperação de todos os diálogos do jogo.

**Atributos**:
- `dialogues`: Dicionário<string, DialogueEntry> mapeando IDs a entradas de diálogo
- `npcDialogues`: Dicionário<string, List<string>> mapeando NPC IDs a seus diálogos disponíveis
- `dialogueGroups`: Dicionário<string, List<DialogueEntry>> agrupando diálogos por contexto (quest, cena, etc.)

**Métodos Principais**:
- `GetDialogueEntry(dialogueId)`: Recupera uma entrada específica de diálogo
- `GetNPCDialogues(npcId)`: Retorna todos os diálogos disponíveis de um NPC
- `LoadFromJSON(filePath)`: Carrega os dados de um arquivo JSON (formato serializado)
- `LoadFromDatabase()`: Carrega diálogos de um banco de dados (opcional para projetos maiores)
- `GetDialoguesByGroup(groupId)`: Retorna diálogos de um grupo específico

**Função Principal**: Funcionar como repositório centralizado para todos os dados de diálogo, permitindo fácil acesso e atualização sem modificar código.

---

### 1.3 LocalizationManager
**Responsabilidade**: Gerenciar localização de textos através de Localization Tables (áudio não localizado).

**Atributos**:
- `currentLanguage`: Idioma atualmente ativo (ex: "pt_BR", "en_US", "es_ES")
- `supportedLanguages`: Lista de idiomas suportados
- `localizationTables`: Referência às Localization Tables do Unity (StringTable, StringTableCollection)
- `fallbackLanguage`: Idioma padrão quando tradução não está disponível
- `audioClips`: Dicionário<string, AudioClip> mapeando chaves de áudio a clipes (não localizado)

**Métodos Principais**:
- `SetLanguage(languageCode)`: Muda o idioma ativo
- `GetLocalizedText(textKey)`: Retorna texto localizado usando chave + idioma ativo
- `GetAudioClip(audioClipKey)`: Retorna clipe de áudio usando chave (não depende de idioma)
- `LoadLocalizationData(languageCode)`: Carrega dados de localização para idioma específico
- `GetSupportedLanguages()`: Retorna lista de idiomas disponíveis
- `GetNPCLocalizedName(npcId)`: Retorna nome do NPC localizado (chave: "npc.{npcId}.name")

**Integração com Localization Tables**:
- Usa StringTableEntry para acessar textos
- Áudio carregado uma única vez via Asset Tables (sem variantes por idioma)
- Suporta Plurals, Gender, Context se necessário para textos
- Fallback automático para fallbackLanguage se tradução não existir

**Função Principal**: Abstrair localização de textos usando Localization Tables nativas do Unity, enquanto gerencia áudio único para cada diálogo (não localizado).

---

### 1.4 ConditionEvaluator
**Responsabilidade**: Avaliar se as condições/pré-requisitos para exibir um diálogo estão atendidas.

**Atributos**:
- `gameStateManager`: Referência ao sistema de estado do jogo
- `questManager`: Referência ao gerenciador de quests
- `flagManager`: Referência ao sistema de flags
- `playerProgressData`: Dados de progresso do jogador

**Tipos de Condições Suportadas**:
- `FlagCondition`: Verifica se uma flag está ativa/inativa
- `QuestCondition`: Verifica estado de uma quest (ativa, concluída, não iniciada)
- `LevelCondition`: Verifica nível do jogador
- `ItemCondition`: Verifica inventário do jogador
- `VariableCondition`: Verifica variáveis customizadas
- `CompoundCondition`: Combina múltiplas condições com AND/OR

**Métodos Principais**:
- `EvaluatePrerequisites(prerequisites)`: Retorna true/false se todas as condições são atendidas
- `EvaluateCondition(condition)`: Avalia uma única condição
- `RegisterCustomCondition(type, evaluator)`: Permite registrar tipos customizados de condições
- `CanDisplayDialogue(dialogueEntry)`: Função de conveniência que avalia se um diálogo pode ser exibido

**Função Principal**: Determinar quais diálogos podem ser mostrados baseado no estado atual do jogo, permitindo narrativa dinâmica.

---

### 1.5 FlagManager
**Responsabilidade**: Gerenciar o sistema de flags/variáveis booleanas que rastreiam estado do jogo.

**Atributos**:
- `flags`: Dicionário<string, bool> armazenando estado de cada flag
- `flagMetadata`: Dicionário com informações sobre cada flag (descrição, categoria)
- `observers`: Lista de callbacks para notificar mudanças em flags

**Métodos Principais**:
- `SetFlag(flagId, value)`: Ativa ou desativa uma flag
- `GetFlag(flagId)`: Retorna estado de uma flag
- `ActivateFlag(flagId)`: Ativa uma flag (atalho para SetFlag(id, true))
- `DeactivateFlag(flagId)`: Desativa uma flag (atalho para SetFlag(id, false))
- `RegisterFlagChangeListener(flagId, callback)`: Registra callback para quando flag muda
- `SaveFlags()` / `LoadFlags()`: Persistência de flags em arquivo de save

**Função Principal**: Manter registro de decisões do jogador e eventos da narrativa para condicionar diálogos futuros.

---

## 2. CAMADA DE LÓGICA (Logic Layer)

### 2.1 DialogueState
**Responsabilidade**: Representar o estado atual de um diálogo em progresso.

**Atributos**:
- `currentDialogueId`: ID do diálogo sendo exibido
- `currentPageIndex`: Índice da página atual (para diálogos longos)
- `allPages`: Lista de strings com texto (já localizado) de cada página
- `isActive`: Se há diálogo em andamento
- `isPlayingTypingEffect`: Se animação de typing está em progresso
- `npcId`: Qual NPC está falando
- `npcLocalizedName`: Nome do NPC já localizado (recuperado na inicialização)
- `currentAudioClip`: AudioClip sendo tocado (único para todas as páginas deste diálogo)

**Métodos Principais**:
- `StartDialogue(dialogueId, npcLocalizedName, audioClip)`: Inicia um novo diálogo com nome e áudio únicos
- `GetCurrentPageText()`: Retorna texto localizado da página atual
- `GetNPCName()`: Retorna nome do NPC localizado
- `GetCurrentAudioClip()`: Retorna o AudioClip atual (válido para todas as páginas)
- `AdvanceToNextPage()`: Move para próxima página
- `IsLastPage()`: Verifica se é última página
- `EndDialogue()`: Finaliza diálogo atual
- `ResetState()`: Limpa o estado para próximo diálogo

**Função Principal**: Manter rastreamento do progresso no diálogo sendo exibido, armazenando textos localizados e referência ao AudioClip único.

---

### 2.2 DialogueSystem (Main Controller)
**Responsabilidade**: Orquestrar todo o fluxo do sistema de diálogo.

**Atributos**:
- `dialogueDatabase`: Referência ao banco de dados
- `localizationManager`: Gerenciador de localização
- `conditionEvaluator`: Avaliador de condições
- `dialogueState`: Estado atual
- `dialogueUI`: Referência à UI
- `audioSource`: AudioSource para reproduzir sons
- `currentDialogueQueue`: Fila de diálogos a exibir
- `isDialogueActive`: Flag global

**Métodos Principais**:
- `InitializeDialogue(npcId, firstDialogueId)`: Começa interação com NPC
  - Valida pré-requisitos
  - Recupera nome localizado do NPC
  - Recupera textos localizados
  - Recupera AudioClip único
  - Inicia exibição
- `ProcessNextInput()`: Processa quando jogador pressiona o botão de avançar
- `CanStartDialogue(npcId, firstDialogueId)`: Verifica se um diálogo pode começar
- `GetAvailableDialogues(npcId)`: Retorna quais diálogos estão disponíveis para um NPC
- `QueueDialogue(dialogueId)`: Adiciona um diálogo à fila
- `SkipDialogue()`: Pula para fim do diálogo atual (mostra texto inteiro)
- `CloseDialogue()`: Fecha a caixa de diálogo

**Fluxo Típico de Operação**:
1. Jogador interage com NPC
2. Sistema valida pré-requisitos com ConditionEvaluator
3. Se válido, recupera nome localizado, todos os textos localizados e áudio único
4. Inicia reprodução do áudio
5. Inicia exibição do diálogo com textos localizados
6. Aguarda input do jogador
7. Ao receber input: avança para próxima página ou fecha (áudio continua tocando)
8. Ao fechar, ativa/desativa flags apropriadas e para áudio

**Função Principal**: Ser o orquestrador central que coordina todas as operações do sistema, garantindo que textos estejam localizados e áudio (único, não localizado) seja reproduzido.

---

### 2.3 DialoguePageManager
**Responsabilidade**: Dividir textos longos em páginas que cabem na caixa de diálogo.

**Atributos**:
- `maxCharactersPerPage`: Limite de caracteres por página (configurável)
- `maxLinesPerPage`: Limite de linhas por página (configurável)
- `pageBreakCharacter`: Caractere especial para forçar quebra (ex: "|")
- `currentPageLines`: Linhas da página atual

**Métodos Principais**:
- `PaginateText(fullLocalizedText)`: Divide texto localizado em páginas
- `GetPages(fullLocalizedText)`: Retorna Lista<string> com cada página
- `GetPageCount(fullLocalizedText)`: Retorna número de páginas necessárias
- `FitTextInPage(text)`: Ajusta texto para caber em uma página
- `SetPageConstraints(maxChars, maxLines)`: Define limites por página
- `AddManualBreaks(text)`: Processa quebras manuais definidas no texto original

**Lógica de Paginação**:
- Respeita quebras de linha naturais do texto
- Tenta não quebrar palavras (hífenação)
- Considera comprimento variável de textos em diferentes idiomas
- Suporta quebras manuais explícitas no texto original

**Função Principal**: Garantir que qualquer texto localizado, independente do tamanho, caiba perfeitamente na caixa de diálogo sem parecer quebrado.

---

### 2.4 TextAnimator
**Responsabilidade**: Animar a aparição de texto (typing effect) com velocidade controlável.

**Atributos**:
- `charactersPerSecond`: Velocidade do typing effect (personalizável)
- `currentCharIndex`: Índice do caractere sendo exibido
- `elapsedTime`: Tempo decorrido desde início da animação
- `isAnimating`: Se animação está em progresso
- `fullText`: Texto localizado completo a ser animado
- `displayedText`: Texto atualmente visível (até currentCharIndex)
- `speedMultiplier`: Multiplicador de velocidade (para jogadores aumentar velocidade)

**Métodos Principais**:
- `StartAnimation(localizedText, charactersPerSecond)`: Inicia animação de um texto localizado
- `Update(deltaTime)`: Atualiza animação (chamado todo frame)
- `GetCurrentDisplayText()`: Retorna texto animado até ponto atual
- `SkipToEnd()`: Mostra texto inteiro imediatamente
- `SetSpeed(charactersPerSecond)`: Muda velocidade
- `SetSpeedMultiplier(multiplier)`: Multiplica velocidade (para input do jogador)
- `IsAnimationComplete()`: Verifica se animação terminou
- `RegisterAnimationCallback(callback)`: Registra callback para quando termina

**Detalhes de Implementação**:
- Suporta diferentes velocidades por caractere (ex: pontuação mais lenta)
- Respeita espaços em branco
- Funciona com qualquer idioma (suporta UTF-8, RTL, combinações diacríticas)

**Função Principal**: Criar efeito visual profissional de texto sendo "digitado" em tempo real, funcionando corretamente com textos localizados.

---

### 2.5 DialogueInputHandler
**Responsabilidade**: Gerenciar inputs do jogador durante diálogos (teclado, gamepad, mouse).

**Atributos**:
- `advanceButton`: Tecla/botão para avançar diálogo (configurável)
- `skipButton`: Tecla/botão para pular animação (configurável)
- `closeButton`: Tecla/botão para fechar diálogo (configurável)
- `isInputEnabled`: Se inputs estão sendo aceitos
- `inputDelay`: Delay mínimo entre inputs (evita múltiplos inputs acidentais)
- `timeSinceLastInput`: Tempo desde último input válido

**Métodos Principais**:
- `DetectInput()`: Verifica se algum input foi pressionado
- `SetAdvanceButton(inputCode)`: Define qual botão avança
- `SetSkipButton(inputCode)`: Define qual botão pula animação
- `IsAdvancePressed()`: Retorna true se botão de avançar foi pressionado
- `IsSkipPressed()`: Retorna true se botão de pular foi pressionado
- `EnableInput()` / `DisableInput()`: Ativa/desativa aceitação de inputs
- `SetInputDelay(delayInSeconds)`: Define delay mínimo entre inputs

**Suporte de Input**:
- Teclado (configurável qual tecla)
- Gamepad (botões do controle)
- Mouse (cliques)
- Touch (para mobile)

**Função Principal**: Capturar intenção do jogador de avanço ou pulo de diálogo de forma confiável.

---

## 3. CAMADA DE APRESENTAÇÃO (UI Layer)

### 3.1 DialogueUIController
**Responsabilidade**: Gerenciar a exibição visual simplificada da caixa de diálogo (apenas nome do NPC + texto).

**Atributos**:
- `dialoguePanel`: Painel raiz da caixa de diálogo
- `npcNameText`: UI Text para exibir nome do NPC (já localizado)
- `dialogueTextDisplay`: UI Text para exibir diálogo (animado, já localizado)
- `continueButton`: Botão para avançar
- `pageIndicator`: UI Text mostrando página atual (ex: "1/3")
- `dialogueBubbleAnimator`: Animator para animações da caixa
- `fadeInOutDuration`: Duração da animação de fade in/out

**Métodos Principais**:
- `ShowDialogueBox()`: Exibe a caixa de diálogo (com animação)
- `HideDialogueBox()`: Oculta a caixa de diálogo (com animação)
- `SetNPCName(npcLocalizedName)`: Define nome localizado do NPC
- `DisplayText(localizedText)`: Exibe um texto localizado (via TextAnimator)
- `UpdatePageIndicator(currentPage, totalPages)`: Mostra quantas páginas há
- `EnableContinueButton()` / `DisableContinueButton()`: Controla interatividade
- `AnimateTextAppearance()`: Anima entrada do texto na caixa

**Animações**:
- Fade in/out da caixa inteira
- Slide in/out lateral
- Pop (aparição com bounce)
- Fade/slide do texto individualmente

**Função Principal**: Ser a interface visual simplificada com o jogador, exibindo apenas nome do NPC e texto do diálogo (ambos já localizados) de forma polida.

---

### 3.2 DialogueChoiceUI (Opcional - para diálogos com escolhas)
**Responsabilidade**: Exibir e gerenciar opções de resposta localizadas do jogador.

**Atributos**:
- `choiceButtons`: Lista de botões de escolha
- `choicePanel`: Painel que contém os botões
- `maxVisibleChoices`: Número máximo de botões simultâneos
- `buttonPrefab`: Prefab para criar novos botões dinamicamente

**Métodos Principais**:
- `DisplayChoices(choiceList)`: Exibe opções de resposta (já localizadas)
- `RegisterChoiceCallback(choiceIndex, callback)`: Registra o que fazer ao selecionar
- `ClearChoices()`: Remove todas as opções
- `HighlightChoice(choiceIndex)`: Destaca uma opção
- `SetChoiceEnabled(choiceIndex, enabled)`: Ativa/desativa uma opção (baseado em pré-requisitos)
- `AnimateChoiceAppearance()`: Anima entrada das opções

**Função Principal**: Permitir que o jogador escolha respostas localizadas quando apropriado, com suporte a condicionalidade.

---

## 4. CLASSES DE SUPORTE

### 4.1 DialogueConfig
**Responsabilidade**: Armazenar configurações do sistema (não deve ser modificado em runtime).

**Atributos**:
- `defaultLanguage`: Idioma padrão do jogo
- `defaultTypingSpeed`: Velocidade padrão de typing effect
- `maxCharactersPerPage`: Limite padrão de caracteres
- `maxLinesPerPage`: Limite padrão de linhas
- `inputDelay`: Delay padrão entre inputs
- `fadeInOutDuration`: Duração padrão de animações
- `audioVolume`: Volume de áudio de diálogo
- `subtitlesEnabled`: Se legendas devem ser exibidas

**Métodos Principais**:
- `LoadConfigFromJSON(filePath)`: Carrega configuração de arquivo
- `ValidateConfig()`: Valida se configuração é válida

**Função Principal**: Centralizar todas as configurações em um único lugar.

---

### 4.2 DialogueLogger
**Responsabilidade**: Registrar eventos do sistema para debug e análise.

**Métodos Principais**:
- `LogDialogueStarted(npcId, dialogueId)`
- `LogDialogueEnded(npcId)`
- `LogFlagActivated(flagId)`
- `LogConditionFailed(requirement)`
- `LogPageAdvanced(pageNumber)`
- `ExportDialogueHistory()`: Exporta histórico de diálogos vistos

**Função Principal**: Facilitar debug e entender como jogador interage com sistema.

---

## 5. FLUXO DE INTERAÇÃO COMPLETO

### Cenário: Jogador interage com The Slime King pela primeira vez

1. **Detecção de Interação**
   - InputHandler detecta que jogador pressionou tecla de interação no NPC
   - DialogueSystem.InitializeDialogue("slime_king", "slime_king_greeting_001") é chamado

2. **Validação de Pré-requisitos**
   - ConditionEvaluator verifica se "slime_king_greeting_001" pode ser exibido
   - Verifica flags, estado de quests, nível do jogador, etc.
   - Se alguma condição falha, retorna null e DialogueSystem aborta

3. **Recuperação de Dados e Localização**
   - DialogueDatabase.GetDialogueEntry("slime_king_greeting_001") recupera metadados
   - LocalizationManager.GetNPCLocalizedName("slime_king") obtém nome localizado
   - LocalizationManager.GetLocalizedText("dialogue.slime_king.greeting_001") obtém texto localizado
   - LocalizationManager.GetAudioClip("dialogue.slime_king.sfx_001") obtém áudio **único** (não localizado)

4. **Paginação**
   - DialoguePageManager.PaginateText(textoLocalizado) divide em páginas se necessário
   - DialogueState.allPages recebe lista de páginas (todas já localizadas)

5. **Inicialização do Diálogo e Áudio**
   - DialogueState.StartDialogue("slime_king_greeting_001", nomeLocalizado, audioClip) inicializa
   - DialogueUIController.ShowDialogueBox() exibe caixa com animação
   - DialogueUIController.SetNPCName(nomeLocalizado) exibe nome
   - DialogueSystem.audioSource.PlayOneShot(audioClip) começa a tocar áudio único (toca até o fim ou até fechar)
   - DialogueUIController.DisplayText(primeiraPageLocalizada) começa exibição

6. **Animação de Typing Effect**
   - TextAnimator.StartAnimation(textoPageAtual, velocidade)
   - A cada frame, TextAnimator.Update() avança a animação
   - DialogueUIController.dialogueTextDisplay recebe texto atualizado
   - Áudio continua tocando simultaneamente

7. **Input do Jogador**
   - InputHandler aguarda pressão do botão de avançar
   - Ou InputHandler detecta botão de pular → TextAnimator.SkipToEnd()

8. **Avançar Página ou Fechar**
   - Se há próxima página: DialogueState.AdvanceToNextPage() vai para próxima
   - Se é última página: DialogueState.EndDialogue() finaliza
   - Volta para passo 6 com próxima página, ou segue para passo 9
   - Áudio **continua tocando** durante todas as páginas (não é retomado por página)

9. **Finalização**
   - FlagManager ativa/desativa flags apropriadas
   - DialogueSystem.audioSource.Stop() para a reprodução do áudio
   - DialogueUIController.HideDialogueBox() fecha caixa com animação
   - DialogueLogger.LogDialogueEnded("slime_king") registra conclusão

---

## 6. CONSIDERAÇÕES TÉCNICAS

### Localização com Localization Tables
- **Todos os textos** vêm de Localization Tables, nunca hardcoded
- **Chaves em DialogueEntry**: `textKey` (localizado), `audioClipKey` (não localizado)
- **LocalizationManager** gerencia único acesso às tabelas para textos
- **Textos pré-carregados**: DialogueSystem localiza TODOS os textos antes de exibir
- **Performance**: Evita delays durante gameplay por causa de lookups de localização
- **Suporte multi-idioma**: Trocar idioma recarrega novos textos automaticamente

### Áudio
- **Não localizado**: Um único arquivo de áudio por diálogo
- **Reprodução contínua**: Áudio toca uma vez para TODO o diálogo (todas as páginas)
- **Sincronização**: Áudio não é sincronizado com texto (toca em background)
- **Parado ao fechar**: DialogueSystem para áudio ao fechar diálogo

### Persistência
- **Flags**: Salvas no arquivo de save do jogo
- **Diálogos Vistos**: Opcionalmente salvo para rastrear quais diálogos jogador já viu
- **Localização**: Carregada ao iniciar jogo baseado em idioma selecionado

### Performance
- DialogueDatabase pode usar lazy loading para diálogos não-críticos
- TextAnimator é altamente otimizado (operações simples por frame)
- LocalizationManager cacheía traduções recentemente usadas
- Paginação feita UMA VEZ no DialogueSystem.InitializeDialogue(), não toda exibição
- AudioClips carregados via Asset References (lazy-loaded se necessário)

### Extensibilidade
- Novo tipo de condição? Herdar de Condition base e registrar com ConditionEvaluator
- Novos efeitos visuais? Estender DialogueUIController
- Novos tipos de paginação? Estender DialoguePageManager

### Mobile vs Desktop
- InputHandler detecta tipo de input (touch vs keyboard)
- DialogueUIController adapta layout para diferentes resoluções
- Botões maiores em mobile, texto ajustável

---

## 7. DIAGRAMA DE DEPENDÊNCIAS

```
DialogueSystem (Main Orchestrator)
├── DialogueDatabase
├── LocalizationManager (Localization Tables)
├── ConditionEvaluator
│   └── FlagManager
├── DialogueState
├── DialoguePageManager
├── TextAnimator
├── DialogueInputHandler
├── AudioSource (para reproduzir áudio único)
└── DialogueUIController

DialogueConfig (Configuração Global)
```

---

## Estrutura de Arquivo JSON para Diálogos

```json
{
  "dialogues": [
    {
      "id": "slime_king_greeting_001",
      "npcId": "slime_king",
      "textKey": "dialogue.slime_king.greeting_001",
      "audioClipKey": "dialogue.slime_king.sfx_001",
      "prerequisites": [
        { "type": "flag", "id": "game_started", "value": true },
        { "type": "quest", "id": "first_quest", "state": "not_started" }
      ],
      "flags_to_activate": ["slime_king_met"],
      "flags_to_deactivate": [],
      "nextDialogueId": "slime_king_greeting_002",
      "metadata": {
        "emotion": "friendly",
        "duration": 5.0
      }
    }
  ]
}
```

---

## Fluxo de Localização e Áudio

### Estrutura de Localization Tables

**String Table Collection: "DialogueTexts"**
```
- Table: "Dialogue_SlimeKing"
  - Entry: "greeting_001" → "Olá, bem-vindo ao meu reino!" (pt_BR)
  - Entry: "greeting_001" → "Hello, welcome to my kingdom!" (en_US)
  - Entry: "greeting_002" → "Você chegou no momento certo..." (pt_BR)
```

**String Table Collection: "NPCNames"**
```
- Table: "NPCNames"
  - Entry: "slime_king" → "O Rei Lodo" (pt_BR)
  - Entry: "slime_king" → "The Slime King" (en_US)
```

**Asset Table Collection: "DialogueAudio"** (para referências de AudioClip - não localizado)
```
- Table: "DialogueAudio"
  - Entry: "dialogue.slime_king.sfx_001" → AudioClip (arquivo único)
  - Entry: "dialogue.slime_king.sfx_002" → AudioClip (arquivo único)
```

### Fluxo de Acesso - Textos (Localizados)

```
DialogueEntry.textKey = "dialogue.slime_king.greeting_001"
              ↓
LocalizationManager.GetLocalizedText("dialogue.slime_king.greeting_001")
              ↓
Localization.GetLocalizedString("DialogueTexts", "Dialogue_SlimeKing", "greeting_001")
              ↓
Retorna texto em idioma ativo (ex: "Olá, bem-vindo ao meu reino!")
```

### Fluxo de Acesso - Áudio (Não Localizado)

```
DialogueEntry.audioClipKey = "dialogue.slime_king.sfx_001"
              ↓
LocalizationManager.GetAudioClip("dialogue.slime_king.sfx_001")
              ↓
Retorna AudioClip único (sem variação por idioma)
              ↓
DialogueSystem.audioSource.PlayOneShot(audioClip)
```

---

## Conclusão

Esta arquitetura oferece:
- ✅ **Modularidade**: Cada classe tem responsabilidade única
- ✅ **Reusabilidade**: Componentes podem ser reutilizados em diferentes partes
- ✅ **Escalabilidade**: Fácil adicionar novos idiomas via Localization Tables
- ✅ **Localização Robusta**: Integração nativa com Localization Tables do Unity para textos
- ✅ **Áudio Simplificado**: Um arquivo único por diálogo, sem overhead de localização
- ✅ **Manutenibilidade**: Dados (JSON) separados de lógica, UI separada de localização
- ✅ **Profissionalismo**: Suporta recursos como paginação, typing effect, áudio contínuo
- ✅ **Performance**: Pré-carregamento de textos, áudio lazy-loaded, evita delays em runtime
- ✅ **Simplicidade de UI**: Apenas nome do NPC e texto (clean and focused)
- ✅ **Experiência Imersiva**: Áudio toca durante toda a conversa, não interrompido por paginação
