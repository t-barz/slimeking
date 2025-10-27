# Sistema de Teletransporte - Documento de Design Técnico v2

## Revisão: Integração com Managers Existentes

Esta versão do design foi revisada para integrar corretamente com os Managers centralizados do projeto, especialmente o **CameraManager** e considerar o **SceneTransitionManager** existente.

## Visão Geral

O sistema de teletransporte será implementado seguindo o princípio KISS, utilizando os Managers existentes do projeto e integrando com o Easy Transition para transições visuais suaves.

## Arquitetura Revisada

### Componentes Principais

```
TeleportPoint (MonoBehaviour)
    ↓ detecta colisão
PlayerController (existente)
    ↓ desabilita controle
SceneTransitioner (Easy Transition)
    ↓ executa transição visual (fade out)
TeleportPoint
    ↓ reposiciona Player
CameraManager (existente)
    ↓ obtém câmera e reposiciona
    ↓ aguarda delay
SceneTransitioner
    ↓ completa transição (fade in)
PlayerController
    ↓ reabilita controle
```

### Integração com Managers

#### 1. CameraManager (Existente - Usar)

**Localização:** `Assets/Code/Systems/Managers/CameraManager.cs`

**Responsabilidade:** Gerenciar câmera principal e configurações URP.

**API Utilizada:**

```csharp
// Singleton
CameraManager.Instance

// Métodos relevantes
public Camera GetMainCamera()
public void ForceRefresh()
```

**Integração no Teletransporte:**

- Usar `GetMainCamera()` para obter referência da câmera
- Reposicionar a câmera junto com o Player
- Opcionalmente chamar `ForceRefresh()` após reposicionamento

#### 2. SceneTransitionManager (Existente - Não Usar Diretamente)

**Localização:** `Assets/Code/Systems/Managers/SceneTransitionManager.cs`

**Responsabilidade:** Transições entre cenas com efeito cellular.

**Nota:** Este manager é específico para mudança de cenas. Para teletransporte na mesma cena, usaremos o Easy Transition diretamente. No futuro, podemos expandir o SceneTransitionManager para suportar teletransporte.

**Decisão:** Não modificar o SceneTransitionManager. Usar Easy Transition diretamente para teletransporte.

## Componentes e Interfaces

### 1. TeleportPoint Component (NOVO - Simplificado)

**Responsabilidade:** Detectar colisão do Player e orquestrar o processo de teletransporte usando Managers existentes.

**Campos Públicos (Inspector):**

```csharp
[Header("Teleport Configuration")]
[Tooltip("Posição de destino do teletransporte")]
[SerializeField] private Vector3 destinationPosition;

[Tooltip("Efeito de transição a ser utilizado (CircleEffect recomendado)")]
[SerializeField] private TransitionEffect transitionEffect;

[Tooltip("Tempo de espera após reposicionamento antes do fade in (segundos)")]
[SerializeField] private float delayBeforeFadeIn = 1f;

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
```

**Métodos Principais:**

```csharp
// Unity Lifecycle
private void Awake()
private void OnTriggerEnter2D(Collider2D other)
private void OnDrawGizmos()

// Teleport Logic
private IEnumerator ExecuteTeleport()
private void RepositionPlayerAndCamera()
private bool ValidateTeleport()
```

### 2. Integração com PlayerController (Existente)

**Métodos Utilizados:**

- `PlayerController.Instance.DisableMovement()` - Desabilita movimento
- `PlayerController.Instance.EnableMovement()` - Reabilita movimento

**Não requer modificações.**

### 3. Integração com CameraManager (Existente)

**Métodos Utilizados:**

```csharp
// Obter câmera principal
Camera mainCamera = CameraManager.Instance.GetMainCamera();

// Reposicionar câmera
mainCamera.transform.position = newPosition;

// Opcional: forçar refresh após reposicionamento
CameraManager.Instance.ForceRefresh();
```

**Não requer modificações.**

### 4. Integração com Easy Transition

**Abordagem Simplificada:**

Ao invés de criar um TeleportTransitionHelper complexo, vamos usar o Easy Transition de forma mais direta, aproveitando que ele já tem um sistema de eventos:

```csharp
// Usar o evento OnSceneLoaded do SceneTransitioner
SceneTransitioner.OnSceneLoaded += OnTransitionMidpoint;

// Iniciar transição "fake" para mesma cena
// Isso ativa o fade out, mas não carrega nova cena
```

**Alternativa Mais Simples:**

Criar uma corrotina que:

1. Ativa manualmente a Image de transição do SceneTransitioner
2. Anima o material do efeito
3. Reposiciona no meio
4. Anima de volta

## Fluxo de Execução Detalhado (Revisado)

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
6. Desabilita movimento do Player
   PlayerController.Instance.DisableMovement()
   ↓
7. Obtém referência da câmera
   Camera mainCamera = CameraManager.Instance.GetMainCamera()
   ↓
8. Executa fade out manual usando Easy Transition
   (Ativa Image, anima Material)
   ↓
9. Reposiciona Player
   PlayerController.Instance.transform.position = destinationPosition
   ↓
10. Reposiciona Câmera
    mainCamera.transform.position = destinationPosition + offset
    ↓
11. Opcional: Força refresh da câmera
    CameraManager.Instance.ForceRefresh()
    ↓
12. Aguarda delayBeforeFadeIn (1 segundo padrão)
    ↓
13. Executa fade in manual usando Easy Transition
    (Anima Material de volta, desativa Image)
    ↓
14. Reabilita movimento do Player
    PlayerController.Instance.EnableMovement()
    ↓
15. Define isTeleporting = false
    ↓
16. Teletransporte completo
```

## Implementação Simplificada

### Abordagem Final (KISS)

Após análise, a abordagem mais simples é:

**Não criar TeleportTransitionHelper separado.**

**Implementar tudo no TeleportPoint:**

```csharp
private IEnumerator ExecuteTeleport()
{
    isTeleporting = true;
    
    // 1. Desabilitar controle
    PlayerController.Instance.DisableMovement();
    
    // 2. Obter câmera
    Camera mainCamera = CameraManager.Instance.GetMainCamera();
    Vector3 cameraOffset = mainCamera.transform.position - PlayerController.Instance.transform.position;
    
    // 3. Fade Out usando Easy Transition
    // Acessar componentes internos do SceneTransitioner
    // OU usar método público se disponível
    yield return StartCoroutine(FadeOut());
    
    // 4. Reposicionar
    PlayerController.Instance.transform.position = destinationPosition;
    mainCamera.transform.position = destinationPosition + cameraOffset;
    
    // 5. Aguardar
    yield return new WaitForSeconds(delayBeforeFadeIn);
    
    // 6. Fade In
    yield return StartCoroutine(FadeIn());
    
    // 7. Reabilitar controle
    PlayerController.Instance.EnableMovement();
    
    isTeleporting = false;
}
```

### Métodos de Fade

**Opção 1: Usar SceneTransitioner diretamente (se possível)**

Verificar se podemos acessar os componentes internos do SceneTransitioner para reutilizar a lógica de fade.

**Opção 2: Implementar fade simples no TeleportPoint**

Se não conseguirmos acessar internals do SceneTransitioner, implementar fade simples:

```csharp
private IEnumerator FadeOut()
{
    // Ativar Image de transição
    // Animar alpha de 0 para 1
    // Ou animar material do effect
}

private IEnumerator FadeIn()
{
    // Animar alpha de 1 para 0
    // Desativar Image
}
```

**Opção 3: Usar PixeLadder.EasyTransition diretamente**

Estudar o código do Easy Transition para ver se há métodos públicos que podemos usar sem modificar o código original.

## Decisões de Design Revisadas

### Por que não criar TeleportTransitionHelper?

**Decisão:** Implementar tudo no TeleportPoint.

**Rationale:**

- Princípio KISS - menos classes = mais simples
- TeleportPoint já orquestra o processo
- Não há necessidade de reutilizar a lógica em outros lugares
- Reduz complexidade e dependências

### Por que usar CameraManager ao invés de Camera.main?

**Decisão:** Usar CameraManager.Instance.GetMainCamera().

**Rationale:**

- CameraManager já gerencia a câmera principal
- Garante que estamos usando a câmera correta
- Mantém consistência com arquitetura do projeto
- Permite usar ForceRefresh() se necessário

### Por que não expandir SceneTransitionManager?

**Decisão:** Não modificar SceneTransitionManager, usar Easy Transition diretamente.

**Rationale:**

- SceneTransitionManager é específico para mudança de cenas
- Teletransporte é funcionalidade diferente
- Evita adicionar complexidade ao Manager
- Mantém separação de responsabilidades
- Pode ser refatorado no futuro se necessário

## Estrutura Final de Componentes

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
│ + FadeOut()         │
│ + FadeIn()          │
└──────────┬──────────┘
           │
           │ usa
           ↓
┌─────────────────────┐
│  PlayerController   │
│    (Singleton)      │
│                     │
│ + Instance          │
│ + DisableMovement() │
│ + EnableMovement()  │
└─────────────────────┘
           │
           │ usa
           ↓
┌─────────────────────┐
│   CameraManager     │
│    (Singleton)      │
│                     │
│ + Instance          │
│ + GetMainCamera()   │
│ + ForceRefresh()    │
└─────────────────────┘
           │
           │ usa
           ↓
┌─────────────────────┐
│  SceneTransitioner  │
│  (Easy Transition)  │
│                     │
│ + Instance          │
│ - transitionImage   │
│ (componentes        │
│  internos)          │
└─────────────────────┘
```

## Estimativa Revisada

### Complexidade Reduzida

Com a integração dos Managers existentes e simplificação da arquitetura:

- **TeleportPoint:** ~250-300 linhas (inclui fade out/in)
- **Sem TeleportTransitionHelper:** 0 linhas (removido)
- **Total:** ~250-300 linhas

### Tempo Estimado Revisado

- **Implementação:** 3-4 horas (reduzido de 5-7)
- **Testes:** 2 horas
- **Documentação:** 1 hora
- **Total:** 6-7 horas (reduzido de 8-11)

## Próximos Passos

1. ✅ Documentação revisada
2. ⏳ Estudar código do Easy Transition para entender como acessar componentes
3. ⏳ Implementar TeleportPoint com integração aos Managers
4. ⏳ Testar integração com CameraManager
5. ⏳ Criar prefab e cena de teste
6. ⏳ Validar e documentar

## Conclusão

A revisão do design considerando os Managers existentes resultou em uma arquitetura mais simples e alinhada com o projeto. A integração com CameraManager garante consistência, e a decisão de não criar TeleportTransitionHelper reduz complexidade mantendo o princípio KISS.

**Estimativa Revisada:** 6-7 horas de trabalho  
**Complexidade:** Baixa  
**Risco:** Baixo  
**Viabilidade:** Alta ✅
