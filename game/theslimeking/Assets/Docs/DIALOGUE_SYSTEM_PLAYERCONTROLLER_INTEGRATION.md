# Integração do Sistema de Diálogos com PlayerController

## Status Atual

O sistema de diálogos está **completamente funcional** sem o PlayerController. No entanto, a funcionalidade de pausar o jogador durante diálogos está implementada como **stub methods** aguardando a implementação do PlayerController.

## Funcionalidade Atual (Sem PlayerController)

✅ **Funcionando:**

- Detecção de proximidade do jogador (usando tag "Player")
- Exibição do ícone de interação
- Início de diálogos ao pressionar botão de interação
- Exibição de texto com efeito typewriter
- Navegação entre páginas de diálogo
- Sistema de localização multilíngue
- Carregamento de diálogos de arquivos JSON

⚠️ **Limitação:**

- O jogador **não é pausado** durante diálogos (pode continuar se movendo)
- Os métodos `PausePlayer()` e `UnpausePlayer()` no DialogueManager são stubs

## Requisitos para Integração Completa

### 1. PlayerController Deve Existir

O PlayerController precisa ser implementado no projeto com as seguintes características:

```csharp
public class PlayerController : MonoBehaviour
{
    // Método para habilitar/desabilitar movimento
    public void SetMovementEnabled(bool enabled) { }
    
    // Método para habilitar/desabilitar input (opcional)
    public void SetInputEnabled(bool enabled) { }
    
    // Ou usar um padrão singleton se preferir
    public static PlayerController Instance { get; private set; }
}
```

### 2. Tag "Player" Configurada

O GameObject do jogador **DEVE** ter a tag "Player" configurada para que o sistema de detecção de proximidade funcione:

1. Selecione o GameObject do jogador na hierarquia
2. No Inspector, clique no dropdown "Tag" no topo
3. Selecione "Player" (ou crie a tag se não existir)

### 3. Atualizar DialogueManager

Quando o PlayerController estiver disponível, atualize os métodos no `DialogueManager.cs`:

#### Localização dos Métodos

`Assets/Code/Systems/Managers/DialogueManager.cs`

#### Método PausePlayer()

**Antes (Stub Atual):**

```csharp
private void PausePlayer()
{
    Log("Pausing player (integration with PlayerController pending)");
    // TODO: Integrar com PlayerController existente
}
```

**Depois (Com Integração):**

```csharp
private void PausePlayer()
{
    Log("Pausing player");
    
    // Opção 1: Se PlayerController usa singleton
    if (PlayerController.HasInstance)
    {
        PlayerController.Instance.SetMovementEnabled(false);
        PlayerController.Instance.SetInputEnabled(false);
    }
    
    // Opção 2: Se PlayerController é encontrado por tag
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetMovementEnabled(false);
            controller.SetInputEnabled(false);
        }
    }
}
```

#### Método UnpausePlayer()

**Antes (Stub Atual):**

```csharp
private void UnpausePlayer()
{
    Log("Unpausing player (integration with PlayerController pending)");
    // TODO: Integrar com PlayerController existente
}
```

**Depois (Com Integração):**

```csharp
private void UnpausePlayer()
{
    Log("Unpausing player");
    
    // Opção 1: Se PlayerController usa singleton
    if (PlayerController.HasInstance)
    {
        PlayerController.Instance.SetMovementEnabled(true);
        PlayerController.Instance.SetInputEnabled(true);
    }
    
    // Opção 2: Se PlayerController é encontrado por tag
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    if (player != null)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.SetMovementEnabled(true);
            controller.SetInputEnabled(true);
        }
    }
}
```

### 4. Configuração pausePlayerDuringDialogue

O DialogueManager possui um campo configurável no Inspector:

```csharp
[SerializeField] private bool pausePlayerDuringDialogue = true;
```

**Comportamento:**

- `true`: Chama `PausePlayer()` ao iniciar diálogo e `UnpausePlayer()` ao encerrar
- `false`: Não pausa o jogador, permitindo movimento durante diálogos

Esta configuração pode ser ajustada por NPC ou globalmente conforme necessário.

## Checklist de Integração

Quando implementar o PlayerController, siga este checklist:

- [ ] PlayerController implementado com métodos de controle de movimento
- [ ] GameObject do jogador possui tag "Player"
- [ ] Atualizar método `PausePlayer()` no DialogueManager
- [ ] Atualizar método `UnpausePlayer()` no DialogueManager
- [ ] Remover comentários TODO dos métodos
- [ ] Testar que o jogador é pausado ao iniciar diálogo
- [ ] Testar que o jogador é despausado ao encerrar diálogo
- [ ] Testar com `pausePlayerDuringDialogue = false` (jogador não deve pausar)
- [ ] Testar com `pausePlayerDuringDialogue = true` (jogador deve pausar)
- [ ] Verificar que não há erros se PlayerController não estiver disponível

## Testes Recomendados

### Teste 1: Pausa Durante Diálogo

1. Configure `pausePlayerDuringDialogue = true` no DialogueManager
2. Aproxime-se de um NPC
3. Inicie o diálogo
4. Tente mover o jogador → **Esperado:** Jogador não se move
5. Encerre o diálogo
6. Tente mover o jogador → **Esperado:** Jogador se move normalmente

### Teste 2: Sem Pausa Durante Diálogo

1. Configure `pausePlayerDuringDialogue = false` no DialogueManager
2. Aproxime-se de um NPC
3. Inicie o diálogo
4. Tente mover o jogador → **Esperado:** Jogador se move normalmente
5. Encerre o diálogo
6. Tente mover o jogador → **Esperado:** Jogador se move normalmente

### Teste 3: Múltiplos Diálogos

1. Inicie um diálogo com NPC A
2. Encerre o diálogo
3. Inicie um diálogo com NPC B
4. Verifique que o jogador é pausado/despausado corretamente em cada transição

### Teste 4: Interrupção de Diálogo

1. Inicie um diálogo
2. Force o encerramento do diálogo (ex: destruir o NPC)
3. Verifique que o jogador é despausado corretamente

## Arquitetura Atual

```
NPCDialogueInteraction (Detecção de Proximidade)
    ↓ (usa tag "Player")
OnTriggerEnter2D/Exit2D
    ↓
DialogueManager.StartDialogue()
    ↓
PausePlayer() [STUB - Aguardando PlayerController]
    ↓
Diálogo Exibido
    ↓
DialogueManager.EndDialogue()
    ↓
UnpausePlayer() [STUB - Aguardando PlayerController]
```

## Notas Importantes

1. **O sistema funciona sem PlayerController**: Todos os recursos de diálogo funcionam, exceto a pausa do jogador
2. **Não quebra o jogo**: Os stubs não causam erros, apenas logam mensagens informativas
3. **Fácil de integrar**: Apenas dois métodos precisam ser atualizados quando o PlayerController estiver pronto
4. **Configurável**: A pausa pode ser habilitada/desabilitada via Inspector
5. **Seguro**: Verifica se o PlayerController existe antes de tentar usá-lo (quando implementado)

## Referências

- **DialogueManager:** `Assets/Code/Systems/Managers/DialogueManager.cs`
- **NPCDialogueInteraction:** `Assets/Code/Gameplay/NPCs/NPCDialogueInteraction.cs`
- **Requirements:** `.kiro/specs/npc-dialogue-system/requirements.md` (Requirements 3.3, 3.4)
- **Design:** `.kiro/specs/npc-dialogue-system/design.md`

## Suporte

Se tiver dúvidas sobre a integração, consulte:

1. Os comentários TODO nos métodos `PausePlayer()` e `UnpausePlayer()`
2. A documentação da classe DialogueManager
3. Este documento de integração
