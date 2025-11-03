# Quest System - Dialogue Integration

## Overview

O Quest System está integrado com o Dialogue System, permitindo que NPCs ofereçam e recebam quests através de diálogos naturais.

## Componentes

### 1. NPCDialogueInteraction (Modificado)

- Detecta automaticamente se há `QuestGiverController` no mesmo GameObject
- Registra o QuestGiver com o `DialogueChoiceHandler` ao iniciar diálogo
- Mantém todas as funcionalidades originais de diálogo

### 2. DialogueChoiceHandler (Novo)

- Singleton que gerencia opções dinâmicas de quest em diálogos
- Gera automaticamente opções de "Aceitar Quest" e "Entregar Quest"
- Executa ações de quest quando o jogador seleciona uma opção
- Limpa automaticamente quando o diálogo termina

### 3. QuestChoiceUI (Novo)

- Componente UI que exibe botões de escolha de quest
- Exibe automaticamente após o diálogo terminar se há opções disponíveis
- Permite ao jogador aceitar ou entregar quests
- Botão de fechar para sair sem escolher nada

### 4. DialogueManager (Modificado)

- Verifica se há opções de quest antes de fechar completamente o diálogo
- Mantém o jogador pausado se há opções de quest para escolher
- Integra-se perfeitamente com o DialogueChoiceHandler

## Como Usar

### Setup Básico

1. **Criar NPC com Quest:**

   ```
   GameObject NPC
   ├── NPCDialogueInteraction (já existente)
   ├── QuestGiverController (adicionar)
   └── CircleCollider2D (trigger)
   ```

2. **Configurar QuestGiverController:**
   - Adicionar CollectQuestData à lista `availableQuests`
   - Configurar indicadores visuais (opcional)

3. **Configurar DialogueChoiceHandler na cena:**
   - Criar GameObject vazio chamado "DialogueChoiceHandler"
   - Adicionar componente `DialogueChoiceHandler`
   - Marcar como DontDestroyOnLoad (automático)

4. **Configurar QuestChoiceUI na cena:**
   - Criar Canvas com QuestChoiceUI
   - Configurar referências:
     - `choicePanel`: GameObject do painel
     - `choiceButtonContainer`: Transform onde botões serão criados
     - `choiceButtonPrefab`: Prefab do botão (deve ter Button e TextMeshProUGUI)
     - `closeButton`: Botão para fechar sem escolher

### Fluxo de Funcionamento

1. **Jogador interage com NPC:**
   - NPCDialogueInteraction detecta QuestGiverController
   - Registra QuestGiver com DialogueChoiceHandler
   - Inicia diálogo normalmente

2. **Diálogo termina:**
   - DialogueManager verifica se há opções de quest
   - DialogueChoiceHandler gera opções baseado no estado:
     - "Aceitar Quest: [Nome]" se quest pode ser aceita
     - "Entregar Quest: [Nome]" se quest está pronta
   - QuestChoiceUI exibe botões de escolha

3. **Jogador escolhe opção:**
   - DialogueChoiceHandler executa ação (aceitar ou entregar)
   - QuestManager processa a ação
   - Opções são atualizadas (pode mudar após ação)
   - Se não há mais opções, painel fecha automaticamente

4. **Jogador fecha painel:**
   - Clica no botão "Fechar"
   - Painel fecha e jogador é despausado

## Requisitos Atendidos

✅ **2.1**: NPCs verificam quests disponíveis através do QuestGiverController
✅ **2.2**: Opção "Aceitar Quest" exibida quando quest disponível e requisitos atendidos
✅ **2.3**: Quest adicionada à lista ativa via QuestManager.AcceptQuest()
✅ **4.1**: Opção "Entregar Quest" exibida quando quest está pronta

## Notas de Implementação

- **Não invasivo**: Modificações mínimas em componentes existentes
- **Desacoplado**: Usa eventos e singletons para comunicação
- **Extensível**: Fácil adicionar novos tipos de escolha no futuro
- **Debug**: Todas as classes têm opção `enableDebugLogs` para facilitar debug

## Próximos Passos

Para usar o sistema:

1. Criar prefab do botão de escolha (Button + TextMeshProUGUI)
2. Configurar QuestChoiceUI na cena de teste
3. Adicionar DialogueChoiceHandler à cena
4. Testar fluxo completo: aceitar quest → coletar item → entregar quest

## Exemplo de Configuração

```csharp
// No Inspector do NPC:
// NPCDialogueInteraction:
//   - dialogueId: "npc_farmer"
//   - interactionRadius: 2.5
//
// QuestGiverController:
//   - availableQuests: [CollectFlowersQuest]
//   - questAvailableIndicator: YellowExclamation
//   - questReadyIndicator: GoldenExclamation
```

## Troubleshooting

**Opções de quest não aparecem:**

- Verificar se DialogueChoiceHandler está na cena
- Verificar se QuestChoiceUI está configurado corretamente
- Verificar se QuestGiverController tem quests na lista
- Ativar `enableDebugLogs` para ver logs detalhados

**Botões não funcionam:**

- Verificar se prefab do botão tem componente Button
- Verificar se QuestManager está na cena
- Verificar se quest atende requisitos (CanAcceptQuest)

**Jogador não despausa:**

- Verificar se botão de fechar está configurado
- Verificar se QuestChoiceUI.HideQuestChoices() é chamado
