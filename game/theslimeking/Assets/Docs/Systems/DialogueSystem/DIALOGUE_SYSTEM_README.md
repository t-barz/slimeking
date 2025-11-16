# Sistema de Diálogos com NPCs - Guia Completo

## Índice

1. [Visão Geral](#visão-geral)
2. [Quick Setup - Configuração Rápida](#quick-setup---configuração-rápida)
3. [Como Criar Novos Diálogos JSON](#como-criar-novos-diálogos-json)
4. [Configuração Manual de NPCs](#configuração-manual-de-npcs)
5. [Configurações Avançadas](#configurações-avançadas)
6. [Troca de Idiomas](#troca-de-idiomas)
7. [Troubleshooting](#troubleshooting)

---

## Visão Geral

O Sistema de Diálogos com NPCs permite criar interações conversacionais com personagens não-jogáveis de forma simples e eficiente. O sistema oferece:

- ✅ Configuração rápida via editor (Quick Setup)
- ✅ Detecção automática de proximidade do jogador
- ✅ Ícone de interação visual acima do NPC
- ✅ Caixa de diálogo com efeito typewriter (texto letra por letra)
- ✅ Suporte a múltiplos idiomas (BR, EN, ES, CH, RS, FR, IT, DT, JP, KR)
- ✅ Paginação de diálogos longos
- ✅ Sistema de fallback de idiomas

---

## Quick Setup - Configuração Rápida

A maneira mais rápida de adicionar diálogos a um NPC:

### Passo 1: Selecione o GameObject do NPC

No Hierarchy, selecione o GameObject que representa seu NPC.

### Passo 2: Use o Menu de Contexto

Clique com o botão direito no GameObject e selecione:

```
GameObject > SlimeKing > Configure as Dialogue NPC
```

### Passo 3: Configure o Dialogue ID

No Inspector, você verá o componente `NPCDialogueInteraction` adicionado automaticamente. Configure:

- **Dialogue Id**: O ID do diálogo (ex: `npc_merchant_greeting`)
- **Interaction Radius**: Distância para ativar a interação (padrão: 2.5)

### O que o Quick Setup faz automaticamente

- ✅ Adiciona o componente `NPCDialogueInteraction`
- ✅ Adiciona um `CircleCollider2D` configurado como trigger
- ✅ Instancia o prefab do ícone de interação como filho do NPC
- ✅ Configura valores padrão para todos os parâmetros

---

## Como Criar Novos Diálogos JSON

### Estrutura do Arquivo JSON

Crie um novo arquivo `.json` no diretório `Assets/Data/Dialogues/` com a seguinte estrutura:

```json
{
    "dialogueId": "seu_dialogo_id",
    "shortDescription": "Descrição curta do diálogo",
    "localizations": [
        {
            "language": "EN",
            "pages": [
                {
                    "text": "First page of dialogue in English."
                },
                {
                    "text": "Second page of dialogue in English."
                }
            ]
        },
        {
            "language": "BR",
            "pages": [
                {
                    "text": "Primeira página do diálogo em Português."
                },
                {
                    "text": "Segunda página do diálogo em Português."
                }
            ]
        }
    ]
}
```

### Campos Obrigatórios

- **dialogueId**: Identificador único do diálogo (usado no componente NPCDialogueInteraction)
- **shortDescription**: Descrição para facilitar identificação no editor
- **localizations**: Array com traduções para diferentes idiomas
  - **language**: Código do idioma (EN, BR, ES, CH, RS, FR, IT, DT, JP, KR)
  - **pages**: Array de páginas do diálogo
    - **text**: Texto da página

### Exemplos

#### Diálogo de Página Única

```json
{
    "dialogueId": "npc_merchant_greeting",
    "shortDescription": "Merchant greeting - single page",
    "localizations": [
        {
            "language": "EN",
            "pages": [
                {
                    "text": "Welcome, traveler! I have the finest wares!"
                }
            ]
        },
        {
            "language": "BR",
            "pages": [
                {
                    "text": "Bem-vindo, viajante! Tenho as melhores mercadorias!"
                }
            ]
        }
    ]
}
```

#### Diálogo com Múltiplas Páginas

```json
{
    "dialogueId": "npc_guard_warning",
    "shortDescription": "Guard warning - multi-page",
    "localizations": [
        {
            "language": "EN",
            "pages": [
                {
                    "text": "Halt! Who goes there?"
                },
                {
                    "text": "These lands are dangerous."
                },
                {
                    "text": "Be careful out there, adventurer!"
                }
            ]
        }
    ]
}
```

### Boas Práticas

1. **Sempre inclua pelo menos o idioma EN** (inglês) como fallback
2. **Use IDs descritivos** (ex: `npc_blacksmith_intro` ao invés de `dialogue_001`)
3. **Mantenha páginas curtas** (2-3 linhas por página para melhor legibilidade)
4. **Teste todos os idiomas** antes de fazer commit
5. **Use caracteres especiais com cuidado** (certifique-se de que o JSON está válido)

---

## Configuração Manual de NPCs

Se você preferir configurar manualmente ao invés de usar o Quick Setup:

### Passo 1: Adicione o Componente NPCDialogueInteraction

```
Add Component > NPCDialogueInteraction
```

### Passo 2: Configure os Parâmetros

**Dialogue Configuration:**

- **Dialogue Id**: ID do diálogo no JSON
- **Interaction Radius**: Raio de detecção (padrão: 2.5)
- **Icon Anchor**: Transform onde o ícone será posicionado (geralmente a cabeça do NPC)
- **Interaction Icon Prefab**: Referência ao prefab `InteractionIcon`

**Input:**

- **Interaction Button**: Nome do botão no Input Manager (padrão: "Interact")

**Advanced Configuration (Opcional):**

- **Custom Typewriter Speed**: Velocidade customizada do efeito typewriter (0 = usar padrão)
- **Pause Player During Dialogue**: Se deve pausar o jogador durante o diálogo

### Passo 3: Adicione um CircleCollider2D

```
Add Component > Circle Collider 2D
```

Configure:

- ✅ **Is Trigger**: Marcado
- **Radius**: Mesmo valor do Interaction Radius (ex: 2.5)

### Passo 4: Adicione o Ícone de Interação

Arraste o prefab `Assets/Game/Prefabs/UI/InteractionIcon.prefab` como filho do NPC.

Posicione o ícone acima da cabeça do NPC e atribua seu Transform ao campo **Icon Anchor**.

---

## Configurações Avançadas

### DialogueSystemSettings

Configurações globais do sistema estão em:

```
Assets/Data/Settings/DialogueSystemSettings.asset
```

**Paths:**

- **Dialogues Path**: Diretório dos arquivos JSON (padrão: `Assets/Data/Dialogues/`)

**Default Values:**

- **Default Typewriter Speed**: Velocidade padrão do efeito typewriter (caracteres/segundo)
- **Default Interaction Radius**: Raio padrão de interação
- **Default Language**: Idioma padrão do sistema

**Prefabs:**

- **Dialogue UI Prefab**: Referência ao prefab da UI de diálogo
- **Interaction Icon Prefab**: Referência ao prefab do ícone de interação

**Input:**

- **Interaction Button Name**: Nome do botão no Input Manager

### Velocidade do Typewriter

Você pode configurar a velocidade do efeito typewriter em três níveis:

1. **Global**: Em `DialogueSystemSettings.defaultTypewriterSpeed`
2. **Por NPC**: Em `NPCDialogueInteraction.customTypewriterSpeed` (0 = usar global)
3. **Desabilitar**: Configure como 0 para exibir texto instantaneamente

### Pausar Jogador Durante Diálogo

Configure se o jogador deve ser pausado durante diálogos:

1. **Global**: Em `DialogueSystemSettings` (futuro)
2. **Por NPC**: Em `NPCDialogueInteraction.pausePlayerDuringDialogue`

---

## Troca de Idiomas

### Em Runtime (Código)

```csharp
using UnityEngine;

public class LanguageSwitcher : MonoBehaviour
{
    public void SetLanguageToPortuguese()
    {
        LocalizationManager.Instance.SetLanguage(SystemLanguage.Portuguese);
    }
    
    public void SetLanguageToEnglish()
    {
        LocalizationManager.Instance.SetLanguage(SystemLanguage.English);
    }
    
    public void SetLanguageToSpanish()
    {
        LocalizationManager.Instance.SetLanguage(SystemLanguage.Spanish);
    }
}
```

### Sistema de Fallback

O sistema usa a seguinte ordem de fallback:

1. **Idioma configurado** (ex: Português)
2. **Inglês (EN)** se o idioma configurado não estiver disponível
3. **Primeiro idioma disponível** no JSON se EN não existir

### Idiomas Suportados

| Código | Idioma |
|--------|--------|
| BR | Português (Brasil) |
| EN | Inglês |
| ES | Espanhol |
| CH | Chinês |
| RS | Russo |
| FR | Francês |
| IT | Italiano |
| DT | Alemão |
| JP | Japonês |
| KR | Coreano |

---

## Troubleshooting

### Problema: Ícone de interação não aparece

**Possíveis causas:**

1. **CircleCollider2D não está configurado como trigger**
   - Solução: Marque "Is Trigger" no CircleCollider2D

2. **Jogador não tem a tag "Player"**
   - Solução: Adicione a tag "Player" ao GameObject do jogador

3. **Prefab do ícone não está atribuído**
   - Solução: Atribua o prefab em `NPCDialogueInteraction.interactionIconPrefab`

4. **Icon Anchor não está configurado**
   - Solução: Atribua o Transform do ícone ao campo `iconAnchor`

### Problema: Diálogo não inicia ao pressionar botão

**Possíveis causas:**

1. **Botão "Interact" não está configurado no Input Manager**
   - Solução: Vá em Edit > Project Settings > Input Manager e adicione o botão "Interact" (tecla E)

2. **Dialogue ID está incorreto**
   - Solução: Verifique se o ID em `NPCDialogueInteraction` corresponde ao `dialogueId` no JSON

3. **Arquivo JSON não foi encontrado**
   - Solução: Verifique o console para erros de carregamento e confirme que o arquivo está em `Assets/Data/Dialogues/`

### Problema: Texto aparece como "[Dialogue not found]"

**Possíveis causas:**

1. **Arquivo JSON não existe**
   - Solução: Crie o arquivo JSON com o ID correto em `Assets/Data/Dialogues/`

2. **JSON está malformado**
   - Solução: Valide o JSON em um validador online (jsonlint.com)

3. **dialogueId não corresponde**
   - Solução: Certifique-se de que o ID no componente é exatamente igual ao do JSON

### Problema: Texto não aparece no idioma correto

**Possíveis causas:**

1. **Idioma não está disponível no JSON**
   - Solução: Adicione a localização para o idioma desejado ou o sistema usará fallback

2. **Código de idioma está incorreto**
   - Solução: Use os códigos corretos (BR, EN, ES, etc.)

### Problema: Efeito typewriter está muito rápido/lento

**Soluções:**

1. **Ajustar velocidade global**: Modifique `DialogueSystemSettings.defaultTypewriterSpeed`
2. **Ajustar por NPC**: Configure `NPCDialogueInteraction.customTypewriterSpeed`
3. **Desabilitar efeito**: Configure velocidade como 0

### Problema: Múltiplos NPCs ativam ao mesmo tempo

**Solução:**

Ajuste o `interactionRadius` de cada NPC para evitar sobreposição de áreas de detecção.

### Problema: Diálogo não fecha na última página

**Possíveis causas:**

1. **Estrutura de páginas incorreta no JSON**
   - Solução: Verifique se todas as páginas estão dentro do array `pages`

2. **DialogueManager não está na cena**
   - Solução: Adicione um GameObject com o componente `DialogueManager`

### Problema: Console mostra erros de JSON parsing

**Solução:**

1. Valide o JSON em jsonlint.com
2. Verifique se todos os colchetes e chaves estão fechados
3. Certifique-se de que não há vírgulas extras
4. Use aspas duplas (") ao invés de aspas simples (')

### Problema: Performance ruim com muitos NPCs

**Soluções:**

1. Reduza o `interactionRadius` dos NPCs
2. Use Object Pooling para ícones de interação
3. Desabilite NPCs que estão muito longe do jogador

---

## Cena de Teste

Uma cena de teste completa está disponível em:

```
Assets/Game/Scenes/Tests/DialogueSystemTest.unity
```

A cena inclui:

- 3 NPCs com diferentes configurações de diálogo
- Exemplos de diálogo de página única e múltiplas páginas
- Botões para testar troca de idiomas
- Managers configurados (DialogueManager e LocalizationManager)

---

## Suporte e Contribuições

Para reportar bugs ou sugerir melhorias, consulte a documentação principal do projeto.

### Arquivos Relacionados

- **Design Document**: `.kiro/specs/npc-dialogue-system/design.md`
- **Requirements**: `.kiro/specs/npc-dialogue-system/requirements.md`
- **Tasks**: `.kiro/specs/npc-dialogue-system/tasks.md`

---

**Última atualização**: 31/10/2025
