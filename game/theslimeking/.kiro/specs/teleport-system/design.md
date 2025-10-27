# Sistema de Teletransporte - Documento de Design Técnico

## Visão Geral

O sistema de teletransporte será implementado seguindo o princípio KISS, utilizando apenas os componentes essenciais para criar uma experiência fluida de teletransporte com transições visuais. O design prioriza simplicidade, reutilização de código existente e integração limpa com o Easy Transition.

## Arquitetura

### Componentes Principais

```
TeleportPoint (MonoBehaviour)
    ↓ detecta colisão
PlayerController (existente)
    ↓ desabilita controle
SceneTransitioner (Easy Transition)
    ↓ executa transição visual
TeleportPoint
    ↓ reposiciona Player + Câmera
    ↓ aguarda delay
SceneTransitioner
    ↓ completa transição
PlayerController
    ↓ reabilita controle
```

### Padrão Arquitetural

Seguindo as boas práticas do projeto, o sistema utilizará:

- **TeleportPoint**: Component (não é Manager, nem Controller global)
- **Comunicação**: Acesso direto ao PlayerController.Instance e SceneTransitioner.Instance
- **Eventos**: Opcional - pode disparar eventos para notificar outros sistemas

## Componentes e Interfaces

### 1. TeleportPoint Component

**Responsabilidade:** Detectar colisão do Player e orquestrar o processo de teletransporte.

**Campos Públicos (Inspector):**

```csharp
[Header("Teleport Configuration")]
[Tooltip("Posição de destino do teletransporte")]
[SerializeField] private Vector3 destinationPosition;

[Tooltip("Efeito de transição a ser utilizado (CircleEffect recomendado)")]
[SerializeField] private TransitionEffect transitionEffect;

[Tooltip("Tempo de espera após reposicionamento antes do fade in (segundos)")]
[SerializeField] private float delayBeforeFadeIn = 1f;

[Header("Trigger Configuration")]
[Tooltip("Tamanho do BoxCollider2D trigger")]
[SerializeField] private Vector2 triggerSize = new Vector2(1f, 1f);

[Tooltip("Offset do trigger em relação à posição do GameObject")]
[SerializeField] private Vector2 triggerOffset = Vector2.zero;

[Header("Debug")]
[Tooltip("Habilita logs de debug para este ponto de teletransporte")]
[SerializeField] private bool enableDebugLogs = false;

[Tooltip("Habilita visualização de Gizmos no Editor")]
[SerializeField] private bool enableGizmos = true;

[Tooltip("Cor do Gizmo de visualização")]
[SerializeField] private Color gizmoColor = Color.cyan;
```

**Campos Privados:**

```csharp
private BoxCollider2D triggerCollider;
private bool isTeleporting = false;
private Transform cameraTransform;
```

**Métodos Principais:**

```csharp
// Unity Lifecycle
private void Awake()
private void OnValidate()
private void OnTriggerEnter2D(Collider2D other)
private void OnDrawGizmos()

// Teleport Logic
private IEnumerator ExecuteTeleport()
private void RepositionPlayerAndCamera()
private bool ValidateTeleport()
private void UpdateTriggerSize()
```

### 2. Integração com PlayerController

**Métodos Necessários no PlayerController:**

O PlayerController já possui os métodos necessários:

- `DisableMovement()` - Desabilita movimento do player
- `EnableMovement()` - Reabilita movimento do player
- `Instance` - Singleton para acesso global

**Não é necessário modificar o PlayerController existente.**

**Rationale:** Reutilizar métodos existentes mantém a consistência do código e evita duplicação de lógica de controle de movimento, seguindo o princípio DRY (Don't Repeat Yourself) e os requisitos 6.1 e 6.3.

### 3. Integração com Easy Transition

**API Utilizada:**

```csharp
// Método principal do SceneTransitioner
SceneTransitioner.Instance.LoadScene(sceneName, effect);

// Para teletransporte na mesma cena, usaremos uma abordagem customizada:
// 1. Ativar manualmente o efeito de fade out
// 2. Reposicionar durante o fade
// 3. Ativar manualmente o efeito de fade in
```

**Nota:** Como o Easy Transition foi projetado para transições entre cenas, precisaremos de uma abordagem alternativa para teletransporte na mesma cena.

### 4. TeleportTransitionHelper (Novo Component)

**Responsabilidade:** Adaptar o Easy Transition para funcionar com teletransporte na mesma cena.

**Métodos:**

```csharp
public IEnumerator ExecuteTransition(TransitionEffect effect, System.Action onMidTransition, float delayBeforeFadeIn)
```

Este helper encapsula a lógica de:

1. Fade out usando o effect
2. Callback no meio da transição (reposicionamento)
3. Delay configurável
4. Fade in usando o effect

**Rationale:** Criar um helper dedicado permite reutilizar o Easy Transition sem modificá-lo (requisito 6.2), mantendo a separação de responsabilidades e facilitando testes isolados da lógica de transição.

## Data Models

### TeleportPoint Data

```csharp
public class TeleportPoint : MonoBehaviour
{
    // Configuração
    private Vector3 destinationPosition;
    private TransitionEffect transitionEffect;
    private float delayBeforeFadeIn;
    private Vector2 triggerSize;
    private Vector2 triggerOffset;
    
    // Debug
    private bool enableDebugLogs;
    private bool enableGizmos;
    private Color gizmoColor;
    
    // Estado
    private bool isTeleporting;
    
    // Referências
    private BoxCollider2D triggerCollider;
    private Transform cameraTransform;
}
```

### Não há necessidade de classes de dados adicionais ou ScriptableObjects

**Rationale:** Manter os dados diretamente no MonoBehaviour simplifica a configuração no Inspector (requisito 5.4) e evita complexidade desnecessária, seguindo o princípio KISS definido nas restrições técnicas.

## Fluxo de Execução Detalhado

### Sequência de Teletransporte

```
1. Player entra no trigger do TeleportPoint
   ↓
2. OnTriggerEnter2D detecta colisão
   ↓
3. Valida se é o Player e se não está teletransportando
   ↓
4. Inicia corrotina ExecuteTeleport()
   ↓
5. Define isTeleporting = true
   ↓
6. Desabilita movimento do Player (PlayerController.Instance.DisableMovement())
   ↓
7. Inicia TeleportTransitionHelper.ExecuteTransition()
   ↓
8. Helper executa fade out (circle fechando)
   ↓
9. Callback onMidTransition é chamado
   ↓
10. RepositionPlayerAndCamera() é executado
    ↓
11. Player.transform.position = destinationPosition
    ↓
12. Camera.transform.position = destinationPosition + offset
    ↓
13. Aguarda delayBeforeFadeIn (1 segundo padrão)
    ↓
14. Helper executa fade in (circle abrindo)
    ↓
15. Reabilita movimento do Player (PlayerController.Instance.EnableMovement())
    ↓
16. Define isTeleporting = false
    ↓
17. Teletransporte completo
```

### Diagrama de Estados

```
[Idle] 
  ↓ (Player entra no trigger)
[Validating]
  ↓ (Validação OK)
[Teleporting]
  ├─ [FadingOut]
  ├─ [Repositioning]
  ├─ [Waiting]
  └─ [FadingIn]
  ↓
[Idle]
```

## Error Handling

### Validações Necessárias

1. **Destino não configurado:**

   ```csharp
   if (destinationPosition == Vector3.zero)
   {
       Debug.LogWarning("TeleportPoint: Destination not configured!", this);
       return;
   }
   ```

2. **TransitionEffect não configurado:**

   ```csharp
   if (transitionEffect == null)
   {
       Debug.LogWarning("TeleportPoint: TransitionEffect not assigned!", this);
       return;
   }
   ```

3. **PlayerController não encontrado:**

   ```csharp
   if (PlayerController.Instance == null)
   {
       Debug.LogError("TeleportPoint: PlayerController.Instance not found!", this);
       return;
   }
   ```

4. **SceneTransitioner não encontrado:**

   ```csharp
   if (SceneTransitioner.Instance == null)
   {
       Debug.LogError("TeleportPoint: SceneTransitioner.Instance not found!", this);
       return;
   }
   ```

5. **Já está teletransportando:**

   ```csharp
   if (isTeleporting)
   {
       if (enableDebugLogs)
           Debug.Log("TeleportPoint: Already teleporting, ignoring trigger.");
       return;
   }
   ```

### Tratamento de Erros

- Todos os erros críticos devem usar `Debug.LogError`
- Avisos devem usar `Debug.LogWarning`
- Logs informativos devem usar `Debug.Log` e respeitar a flag `enableDebugLogs`
- Em caso de erro, o teletransporte deve ser cancelado e o controle do Player restaurado

## Testing Strategy

### Testes Manuais Necessários

1. **Teste Básico:**
   - Criar um TeleportPoint
   - Configurar destino
   - Atribuir CircleEffect
   - Testar colisão do Player

2. **Teste de Múltiplos Pontos:**
   - Criar 2+ TeleportPoints
   - Testar teletransporte sequencial
   - Verificar que não há interferência entre pontos

3. **Teste de Borda:**
   - Tentar teletransportar sem destino configurado
   - Tentar teletransportar sem effect configurado
   - Tentar teletransportar durante outro teletransporte

4. **Teste de Câmera:**
   - Verificar que a câmera segue o Player
   - Verificar que não há "saltos" visíveis
   - Testar em diferentes posições do mapa

5. **Teste de Controle:**
   - Verificar que inputs são ignorados durante teletransporte
   - Verificar que controle é restaurado após teletransporte
   - Tentar mover durante a transição

### Cenários de Teste

| Cenário | Entrada | Resultado Esperado |
|---------|---------|-------------------|
| Teletransporte Normal | Player colide com TeleportPoint configurado | Transição suave, reposicionamento, controle restaurado |
| Sem Destino | Player colide com TeleportPoint sem destino | Warning no console, teletransporte cancelado |
| Sem Effect | Player colide com TeleportPoint sem effect | Warning no console, teletransporte cancelado |
| Teletransporte Duplo | Player colide durante teletransporte ativo | Segundo teletransporte ignorado |
| Câmera | Teletransporte executado | Câmera reposicionada junto com Player |

## Visualização no Editor (Gizmos)

### Gizmos Implementados

1. **Área do Trigger:**
   - Desenhar wireframe do BoxCollider2D
   - Cor configurável (padrão: cyan)
   - Apenas quando enableGizmos = true

2. **Linha para Destino:**
   - Linha conectando origem ao destino
   - Seta indicando direção
   - Cor configurável

3. **Marcador de Destino:**
   - Esfera ou cubo no ponto de destino
   - Mesmo cor da linha
   - Tamanho proporcional ao trigger

### Código de Exemplo para Gizmos

```csharp
private void OnDrawGizmos()
{
    if (!enableGizmos) return;
    
    // Desenha área do trigger - Requisito 5.1
    Gizmos.color = gizmoColor;
    if (triggerCollider != null)
    {
        Gizmos.DrawWireCube(
            transform.position + (Vector3)triggerCollider.offset,
            triggerCollider.size
        );
    }
    else
    {
        // Desenha preview do trigger mesmo sem o collider configurado
        Gizmos.DrawWireCube(
            transform.position + (Vector3)triggerOffset,
            triggerSize
        );
    }
    
    // Desenha linha para destino - Requisito 5.2
    if (destinationPosition != Vector3.zero)
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawLine(transform.position, destinationPosition);
        
        // Desenha marcador no destino
        Gizmos.DrawWireSphere(destinationPosition, 0.5f);
        
        // Desenha seta indicando direção
        Vector3 direction = (destinationPosition - transform.position).normalized;
        Vector3 arrowTip = destinationPosition - direction * 0.5f;
        Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, 45) * -direction * 0.3f);
        Gizmos.DrawLine(arrowTip, arrowTip + Quaternion.Euler(0, 0, -45) * -direction * 0.3f);
    }
}

private void OnValidate()
{
    // Atualiza o trigger quando valores são modificados no Inspector - Requisito 5.4
    UpdateTriggerSize();
}

private void UpdateTriggerSize()
{
    if (triggerCollider == null)
        triggerCollider = GetComponent<BoxCollider2D>();
    
    if (triggerCollider != null)
    {
        triggerCollider.size = triggerSize;
        triggerCollider.offset = triggerOffset;
        triggerCollider.isTrigger = true;
    }
}
```

## Performance Considerations

### Otimizações

1. **Cache de Referências:**
   - Cache do BoxCollider2D no Awake
   - Cache da Transform da câmera na primeira utilização
   - Cache do PlayerController.Instance

2. **Evitar Alocações:**
   - Reutilizar corrotinas
   - Não criar novos objetos durante teletransporte
   - Usar yield return null ao invés de new WaitForSeconds quando possível

3. **Validações Eficientes:**
   - Early return em validações
   - Verificar isTeleporting antes de outras validações
   - CompareTag ao invés de tag == "string"

### Impacto Esperado

- **Memória:** Mínimo (apenas referências cacheadas)
- **CPU:** Baixo (apenas durante transição, ~2-3 segundos)
- **GPU:** Delegado ao Easy Transition (shader otimizado)

## Integração com Sistemas Existentes

### PlayerController

**Métodos Utilizados:**

- `PlayerController.Instance.DisableMovement()` - Requisito 3.1
- `PlayerController.Instance.EnableMovement()` - Requisito 3.3

**Não requer modificações no PlayerController.**

**Rationale:** Utilizar a API existente do PlayerController garante consistência com o resto do código (requisito 6.1) e evita duplicação de lógica de controle de movimento.

### Easy Transition

**Componentes Utilizados:**

- `SceneTransitioner.Instance`
- `TransitionEffect` (CircleEffect) - Requisito 2.1
- `Image` component para renderização

**Não requer modificações no Easy Transition.**

**Rationale:** Manter o Easy Transition intacto (requisito 6.2) garante compatibilidade com futuras atualizações do asset e evita problemas de manutenção.

### Sistema de Câmera

**Assumindo que existe um CameraFollow ou similar:**

- A câmera deve seguir o Player automaticamente
- Apenas reposicionamos o Player, a câmera segue naturalmente
- Se necessário, podemos reposicionar manualmente a câmera (requisito 4.1, 4.2)

**Rationale:** Reposicionar manualmente garante que não haja "saltos" visíveis (requisito 4.3) e mantém a distância/ângulo corretos da câmera.

### Sistema de Eventos (Opcional)

**Se o projeto possui um sistema de eventos:**

- Disparar evento `OnTeleportStarted` antes do fade out
- Disparar evento `OnTeleportCompleted` após o fade in
- Permitir que outros sistemas reajam ao teletransporte

**Rationale:** Seguir o requisito 6.4, permitindo que outros sistemas (como áudio, UI, ou efeitos de partículas) reajam ao teletransporte sem acoplamento direto.

## Extensibilidade Futura

### Possíveis Expansões

1. **Teletransporte Entre Cenas:**
   - Adicionar campo `string targetSceneName`
   - Usar `SceneTransitioner.LoadScene()` diretamente
   - Manter compatibilidade com teletransporte na mesma cena

2. **Condições de Ativação:**
   - Adicionar campo `bool requiresInteraction`
   - Adicionar campo `KeyCode interactionKey`
   - Mostrar prompt de interação

3. **Efeitos Sonoros:**
   - Adicionar campo `AudioClip teleportSound`
   - Tocar som durante transição
   - Integrar com AudioManager quando disponível

4. **Cooldown:**
   - Adicionar campo `float cooldownTime`
   - Prevenir uso repetido imediato
   - Mostrar feedback visual de cooldown

5. **Direção de Saída:**
   - Adicionar campo `Vector2 exitDirection`
   - Fazer Player sair do trigger automaticamente
   - Prevenir teletransporte em loop

### Pontos de Extensão

O design atual permite fácil extensão através de:

- Herança de TeleportPoint
- Eventos customizados
- Configuração via Inspector
- Métodos virtuais para override

## Decisões de Design e Rationale

### Por que não usar SceneTransitioner.LoadScene()?

**Decisão:** Criar TeleportTransitionHelper customizado.

**Rationale:**

- `LoadScene()` foi projetado para mudança de cenas
- Teletransporte na mesma cena requer controle mais fino
- Precisamos executar código no meio da transição
- Queremos delay configurável antes do fade in

### Por que não modificar o PlayerController?

**Decisão:** Usar métodos existentes do PlayerController.

**Rationale:**

- Princípio KISS - não adicionar complexidade desnecessária
- PlayerController já tem métodos de controle de movimento
- Manter separação de responsabilidades
- Facilitar manutenção futura

### Por que usar Corrotinas ao invés de async/await?

**Decisão:** Usar corrotinas do Unity.

**Rationale:**

- Padrão estabelecido no projeto
- Melhor integração com Unity lifecycle
- Easy Transition usa corrotinas
- Mais familiar para desenvolvedores Unity

### Por que não criar um TeleportManager?

**Decisão:** Cada TeleportPoint é independente.

**Rationale:**

- Princípio KISS - não criar Manager desnecessário
- Pontos de teletransporte são independentes
- Não há estado global a gerenciar
- Facilita level design (arrastar e configurar)
- Segue os padrões arquiteturais do projeto (requisito 6.3)

### Por que permitir configuração do trigger no Inspector?

**Decisão:** Expor triggerSize e triggerOffset como campos serializados.

**Rationale:**

- Atende requisito 5.4 explicitamente
- Permite ajuste fino sem recompilar código
- Facilita level design e iteração rápida
- Gizmos fornecem feedback visual imediato (requisito 5.1)

### Por que usar CompareTag ao invés de tag == "Player"?

**Decisão:** Usar CompareTag("Player") na detecção de colisão.

**Rationale:**

- Melhor performance (não aloca string)
- Evita erros de digitação
- Recomendação oficial da Unity
- Atende requisito 1.2 de forma otimizada

## Diagramas

### Diagrama de Componentes

```
┌─────────────────────┐
│   TeleportPoint     │
│  (MonoBehaviour)    │
│                     │
│ - destinationPos    │
│ - transitionEffect  │
│ - isTeleporting     │
│                     │
│ + OnTriggerEnter2D()│
│ + ExecuteTeleport() │
└──────────┬──────────┘
           │
           │ usa
           ↓
┌─────────────────────┐
│TeleportTransition   │
│      Helper         │
│                     │
│ + ExecuteTransition()│
└──────────┬──────────┘
           │
           │ usa
           ↓
┌─────────────────────┐
│  SceneTransitioner  │
│  (Easy Transition)  │
│                     │
│ + Instance          │
│ - transitionImage   │
└─────────────────────┘
```

### Diagrama de Sequência

```
Player    TeleportPoint    Helper    SceneTransitioner    PlayerController
  │             │            │              │                   │
  ├─ Colide ───>│            │              │                   │
  │             ├─ Valida    │              │                   │
  │             ├─ Desabilita movimento ───────────────────────>│
  │             ├─ Inicia ──>│              │                   │
  │             │            ├─ Fade Out ──>│                   │
  │             │            │              ├─ Anima           │
  │             │            │<─ Complete ──┤                   │
  │             │<─ Callback─┤              │                   │
  │             ├─ Reposiciona              │                   │
  │<─ Move ─────┤            │              │                   │
  │             ├─ Aguarda   │              │                   │
  │             ├─ Continua ─>│              │                   │
  │             │            ├─ Fade In ───>│                   │
  │             │            │              ├─ Anima           │
  │             │            │<─ Complete ──┤                   │
  │             ├─ Habilita movimento ──────────────────────────>│
  │             ├─ Completo  │              │                   │
```

## Conclusão

Este design segue rigorosamente o princípio KISS, reutilizando ao máximo os componentes existentes (PlayerController, Easy Transition) e criando apenas o mínimo necessário para implementar o sistema de teletransporte. A arquitetura é simples, extensível e fácil de manter, seguindo todas as boas práticas definidas no projeto.
