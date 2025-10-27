# Limitações Conhecidas - Sistema de Teletransporte

## Visão Geral

Este documento detalha as limitações conhecidas do Sistema de Teletransporte versão 1.0.0, incluindo explicações técnicas, workarounds e planos futuros para cada limitação.

## Índice

1. [Teletransporte Apenas na Mesma Cena](#1-teletransporte-apenas-na-mesma-cena)
2. [Um Destino Por TeleportPoint](#2-um-destino-por-teleportpoint)
3. [Sem Suporte para Condições de Ativação](#3-sem-suporte-para-condições-de-ativação)
4. [Sem Interação Manual](#4-sem-interação-manual)
5. [Sem Cooldown Entre Teletransportes](#5-sem-cooldown-entre-teletransportes)
6. [Sem Efeitos Sonoros Integrados](#6-sem-efeitos-sonoros-integrados)
7. [Sem Direção de Saída](#7-sem-direção-de-saída)
8. [Performance com Muitos TeleportPoints](#8-performance-com-muitos-teleportpoints)
9. [Sem Suporte para Multiplayer](#9-sem-suporte-para-multiplayer)
10. [Dependência do Easy Transition](#10-dependência-do-easy-transition)

---

## 1. Teletransporte Apenas na Mesma Cena

### Descrição

O sistema atual funciona apenas para teletransporte dentro da mesma cena Unity. Não há suporte para mudança de cena durante o teletransporte.

### Razão Técnica

O `TeleportTransitionHelper` foi projetado para adaptar o Easy Transition para teletransporte sem mudança de cena. Ele manipula diretamente o material de transição e executa callbacks durante a transição, o que não é compatível com o fluxo de mudança de cena do Unity.

### Impacto

- Não é possível teletransportar entre diferentes níveis/cenas
- Todas as áreas conectadas por teletransporte devem estar na mesma cena
- Pode aumentar o tamanho e complexidade de cenas grandes

### Workaround

Para teletransporte entre cenas, use o método padrão do Easy Transition:

```csharp
// Em um script customizado
public void TeleportToOtherScene(string sceneName)
{
    SceneTransitioner.Instance.LoadScene(sceneName, circleEffect);
}
```

### Plano Futuro (v1.1.0)

Adicionar campo opcional `targetSceneName`:

- Se vazio: teletransporte na mesma cena (comportamento atual)
- Se preenchido: usa `SceneTransitioner.LoadScene()` para mudança de cena

---

## 2. Um Destino Por TeleportPoint

### Descrição

Cada TeleportPoint pode ter apenas um destino configurado. Não é possível ter múltiplos destinos baseados em condições.

### Razão Técnica

O design segue o princípio KISS (Keep It Simple and Straightforward). Um destino único simplifica a configuração e reduz a complexidade do código.

### Impacto

- Para múltiplos destinos, é necessário criar múltiplos TeleportPoints
- Pode aumentar o número de GameObjects na cena
- Não é possível ter destinos dinâmicos baseados em estado do jogo

### Workaround

Criar múltiplos TeleportPoints próximos ou sobrepostos:

```
Exemplo: Hub com 4 destinos
- TeleportPoint_ToNorth (posição ligeiramente diferente)
- TeleportPoint_ToSouth (posição ligeiramente diferente)
- TeleportPoint_ToEast (posição ligeiramente diferente)
- TeleportPoint_ToWest (posição ligeiramente diferente)
```

Ou criar uma subclasse customizada:

```csharp
public class MultiDestinationTeleportPoint : TeleportPoint
{
    [SerializeField] private Vector3[] destinations;
    [SerializeField] private int currentDestinationIndex = 0;
    
    protected Vector3 GetDestination()
    {
        return destinations[currentDestinationIndex];
    }
}
```

### Plano Futuro (v1.2.0)

Adicionar suporte para múltiplos destinos com seleção baseada em:

- Índice configurável
- Condições customizáveis
- Random entre opções

---

## 3. Sem Suporte para Condições de Ativação

### Descrição

O teletransporte é ativado automaticamente quando o Player colide com o trigger. Não há sistema integrado para condições (ex: ter uma chave, completar quest, etc.).

### Razão Técnica

Condições de ativação variam muito entre projetos. Implementar um sistema genérico seria complexo e poderia não atender necessidades específicas.

### Impacto

- Todos os teletransportes são sempre acessíveis
- Não é possível bloquear teletransportes até cumprir requisitos
- Dificulta implementação de progressão baseada em teletransportes

### Workaround

Criar uma subclasse que adiciona validações customizadas:

```csharp
public class ConditionalTeleportPoint : TeleportPoint
{
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredKeyID = "";
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
            
        // Validação customizada
        if (requiresKey && !PlayerInventory.HasKey(requiredKeyID))
        {
            ShowMessage("Você precisa de uma chave!");
            return;
        }
        
        // Continua com teletransporte normal
        base.OnTriggerEnter2D(other);
    }
}
```

### Plano Futuro (v1.2.0)

Adicionar sistema de condições configurável:

- Interface `ITeleportCondition`
- Lista de condições no Inspector
- Mensagens de erro customizáveis

---

## 4. Sem Interação Manual

### Descrição

O teletransporte é ativado automaticamente ao entrar no trigger. Não há opção para requerer input do jogador (ex: pressionar E para teletransportar).

### Razão Técnica

Interação manual adiciona complexidade de UI (mostrar prompt), input handling, e gerenciamento de estado. O design atual prioriza simplicidade.

### Impacto

- Player pode teletransportar acidentalmente
- Não há controle sobre quando o teletransporte ocorre
- Dificulta implementação de teletransportes opcionais

### Workaround

Criar uma subclasse com interação manual:

```csharp
public class InteractiveTeleportPoint : TeleportPoint
{
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private GameObject promptUI;
    
    private bool playerInRange = false;
    
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HidePrompt();
        }
    }
    
    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
        {
            HidePrompt();
            StartCoroutine(ExecuteTeleport());
        }
    }
}
```

### Plano Futuro (v1.1.0)

Adicionar campos opcionais:

- `bool requiresInteraction`
- `KeyCode interactionKey`
- `GameObject promptPrefab`

---

## 5. Sem Cooldown Entre Teletransportes

### Descrição

Após completar um teletransporte, o Player pode imediatamente usar outro TeleportPoint. Não há período de cooldown global ou por ponto.

### Razão Técnica

Cooldown adiciona complexidade de gerenciamento de tempo e estado. A flag `isTeleporting` já previne múltiplos teletransportes simultâneos.

### Impacto

- Player pode usar teletransportes em sequência rápida
- Pode permitir exploits ou sequências não intencionais
- Não há controle sobre frequência de uso

### Workaround

Implementar cooldown global:

```csharp
public class TeleportCooldownManager : MonoBehaviour
{
    public static TeleportCooldownManager Instance { get; private set; }
    
    [SerializeField] private float cooldownTime = 2f;
    private float lastTeleportTime = -999f;
    
    private void Awake()
    {
        Instance = this;
    }
    
    public bool CanTeleport()
    {
        return Time.time >= lastTeleportTime + cooldownTime;
    }
    
    public void RegisterTeleport()
    {
        lastTeleportTime = Time.time;
    }
}

// No TeleportPoint customizado:
protected override IEnumerator ExecuteTeleport()
{
    if (!TeleportCooldownManager.Instance.CanTeleport())
    {
        Debug.Log("Cooldown ativo!");
        yield break;
    }
    
    yield return base.ExecuteTeleport();
    TeleportCooldownManager.Instance.RegisterTeleport();
}
```

### Plano Futuro (v1.1.0)

Adicionar campo opcional:

- `float cooldownTime` (0 = sem cooldown)
- Cooldown visual no UI
- Som de cooldown

---

## 6. Sem Efeitos Sonoros Integrados

### Descrição

O sistema não toca sons automaticamente durante o teletransporte. Efeitos sonoros devem ser adicionados manualmente.

### Razão Técnica

Sistemas de áudio variam muito entre projetos (AudioSource, FMOD, Wwise, etc.). Integrar um sistema específico limitaria a flexibilidade.

### Impacto

- Teletransportes são silenciosos por padrão
- Requer trabalho adicional para adicionar áudio
- Não há feedback sonoro para o jogador

### Workaround

Adicionar AudioSource ao TeleportPoint:

```csharp
[SerializeField] private AudioClip teleportSound;
[SerializeField] private AudioClip arrivalSound;

protected override IEnumerator ExecuteTeleport()
{
    // Toca som de início
    if (teleportSound != null)
    {
        AudioSource.PlayClipAtPoint(teleportSound, transform.position);
    }
    
    yield return base.ExecuteTeleport();
    
    // Toca som de chegada
    if (arrivalSound != null)
    {
        AudioSource.PlayClipAtPoint(arrivalSound, destinationPosition);
    }
}
```

Ou usar sistema de eventos:

```csharp
// No TeleportPoint
public UnityEvent onTeleportStart;
public UnityEvent onTeleportComplete;

// No AudioManager
public void PlayTeleportSound()
{
    audioSource.PlayOneShot(teleportClip);
}
```

### Plano Futuro (v1.1.0)

Adicionar campos opcionais:

- `AudioClip teleportStartSound`
- `AudioClip teleportEndSound`
- `float soundVolume`

---

## 7. Sem Direção de Saída

### Descrição

Após teletransportar, o Player pode imediatamente reteletransportar se o destino estiver dentro de outro trigger. Não há mecanismo para empurrar o Player para fora do trigger.

### Razão Técnica

Implementar direção de saída requer manipulação de física ou movimento forçado, o que pode conflitar com o PlayerController existente.

### Impacto

- Risco de loops infinitos de teletransporte
- Requer cuidado extra no posicionamento de destinos
- Pode causar comportamento inesperado

### Workaround

**Solução 1: Posicionamento Cuidadoso**

```
Sempre posicione destinos FORA de áreas de trigger:

TeleportPoint A (0, 0) → Destino (20, 0)
TeleportPoint B (22, 0) → Destino (0, 0)

Distância entre destino e próximo trigger: 2+ unidades
```

**Solução 2: Delay com Desativação Temporária**

```csharp
protected override IEnumerator ExecuteTeleport()
{
    yield return base.ExecuteTeleport();
    
    // Desativa trigger por 1 segundo
    triggerCollider.enabled = false;
    yield return new WaitForSeconds(1f);
    triggerCollider.enabled = true;
}
```

**Solução 3: Empurrar Player**

```csharp
[SerializeField] private Vector2 exitDirection = Vector2.right;
[SerializeField] private float exitForce = 2f;

private void RepositionPlayerAndCamera()
{
    base.RepositionPlayerAndCamera();
    
    // Empurra Player na direção de saída
    Rigidbody2D rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.velocity = exitDirection.normalized * exitForce;
    }
}
```

### Plano Futuro (v1.2.0)

Adicionar campos opcionais:

- `Vector2 exitDirection`
- `float exitForce`
- `bool autoExit`

---

## 8. Performance com Muitos TeleportPoints

### Descrição

Em cenas com 50+ TeleportPoints, os Gizmos podem impactar a performance do Editor. O sistema em runtime é eficiente, mas a visualização no Editor pode ficar lenta.

### Razão Técnica

`OnDrawGizmos()` é chamado para todos os TeleportPoints visíveis no Scene view, incluindo cálculos de linhas, setas e labels.

### Impacto

- Editor pode ficar lento com muitos TeleportPoints
- Scene view pode ter framerate reduzido
- Dificulta trabalho em cenas grandes

### Workaround

**Solução 1: Desabilitar Gizmos Globalmente**

```
No Editor: Gizmos dropdown (canto superior direito do Scene view)
Desmarque "TeleportPoint"
```

**Solução 2: Desabilitar Gizmos Individualmente**

```
Selecione TeleportPoints que não precisa visualizar
Desmarque "Enable Gizmos" no Inspector
```

**Solução 3: Usar OnDrawGizmosSelected**

```csharp
// Mostra Gizmos apenas quando selecionado
private void OnDrawGizmosSelected()
{
    // Código de visualização aqui
}
```

**Solução 4: LOD para Gizmos**

```csharp
private void OnDrawGizmos()
{
    if (!enableGizmos) return;
    
    // Calcula distância da câmera do Scene view
    float distance = Vector3.Distance(transform.position, SceneView.lastActiveSceneView.camera.transform.position);
    
    // Só desenha se estiver próximo
    if (distance > 50f) return;
    
    // Código de visualização aqui
}
```

### Plano Futuro (v2.0.0)

- Sistema de LOD para Gizmos
- Gizmos simplificados para distâncias grandes
- Editor customizado com preview otimizado

---

## 9. Sem Suporte para Multiplayer

### Descrição

O sistema foi projetado para single-player. Não há sincronização de rede, validação server-side, ou replicação de estado para multiplayer.

### Razão Técnica

Multiplayer adiciona complexidade significativa (networking, sincronização, validação, latência). O escopo inicial é single-player.

### Impacto

- Não funciona em jogos multiplayer
- Não há sincronização entre clientes
- Não há validação server-side

### Workaround

Para multiplayer, seria necessário:

1. **Sincronização de Rede**

```csharp
[Command]
public void CmdTeleport(Vector3 destination)
{
    // Validação server-side
    if (CanTeleport())
    {
        RpcTeleport(destination);
    }
}

[ClientRpc]
public void RpcTeleport(Vector3 destination)
{
    // Executa teletransporte em todos os clientes
}
```

2. **Validação Server-Side**

```csharp
// Servidor valida se teletransporte é permitido
// Previne cheating
```

3. **Replicação de Posição**

```csharp
// Sincroniza posição do Player entre clientes
```

### Plano Futuro

Multiplayer está fora do escopo atual. Considere usar soluções de networking existentes (Mirror, Netcode for GameObjects) e adaptar o sistema.

---

## 10. Dependência do Easy Transition

### Descrição

O sistema requer o Easy Transition para funcionar. Não é possível usar sem o asset ou com sistema de transição customizado.

### Razão Técnica

O `TeleportTransitionHelper` foi projetado especificamente para adaptar o Easy Transition. Ele acessa campos privados via reflexão e usa a API do Easy Transition.

### Impacto

- Requer Easy Transition no projeto
- Não funciona com outros sistemas de transição
- Dependência de asset de terceiros

### Workaround

Para remover a dependência, seria necessário:

1. **Criar Interface de Transição**

```csharp
public interface ITransitionSystem
{
    IEnumerator ExecuteTransition(Action onMidTransition, float delay);
}
```

2. **Implementar para Easy Transition**

```csharp
public class EasyTransitionAdapter : ITransitionSystem
{
    // Implementação atual
}
```

3. **Implementar Sistema Customizado**

```csharp
public class CustomTransitionSystem : ITransitionSystem
{
    public IEnumerator ExecuteTransition(Action onMidTransition, float delay)
    {
        // Sua implementação customizada
    }
}
```

4. **Modificar TeleportPoint**

```csharp
[SerializeField] private ITransitionSystem transitionSystem;
```

### Plano Futuro (v2.0.0)

- Abstrair sistema de transição
- Suportar múltiplos backends (Easy Transition, customizado, etc.)
- Manter Easy Transition como padrão

---

## Resumo de Limitações

| # | Limitação | Severidade | Workaround | Plano Futuro |
|---|-----------|------------|------------|--------------|
| 1 | Mesma cena apenas | Média | Usar LoadScene | v1.1.0 |
| 2 | Um destino | Baixa | Múltiplos pontos | v1.2.0 |
| 3 | Sem condições | Média | Subclasse | v1.2.0 |
| 4 | Sem interação manual | Média | Subclasse | v1.1.0 |
| 5 | Sem cooldown | Baixa | Manager global | v1.1.0 |
| 6 | Sem áudio | Baixa | AudioSource | v1.1.0 |
| 7 | Sem direção saída | Média | Posicionamento | v1.2.0 |
| 8 | Performance Gizmos | Baixa | Desabilitar | v2.0.0 |
| 9 | Sem multiplayer | Alta | Reescrever | Fora do escopo |
| 10 | Dependência Easy Transition | Média | Interface | v2.0.0 |

---

## Conclusão

Estas limitações são conhecidas e documentadas. A maioria pode ser contornada com workarounds ou extensões customizadas. Versões futuras do sistema abordarão as limitações mais impactantes.

Para necessidades específicas não cobertas pelo sistema atual, considere:

1. Criar subclasses customizadas
2. Usar sistema de eventos para integração
3. Contribuir com melhorias para versões futuras

**Lembre-se:** O sistema foi projetado seguindo o princípio KISS. Adicionar todas as funcionalidades aumentaria significativamente a complexidade. Use extensões quando necessário!
