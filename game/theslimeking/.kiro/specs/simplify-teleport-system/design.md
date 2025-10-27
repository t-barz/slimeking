# Design Document - Simplificação do Sistema de Teleporte

## Overview

Este documento descreve o design para simplificar o sistema de teleporte, removendo o pré-carregamento de cenas e as validações de proximidade. O novo design foca em um fluxo direto: colisão → transição visual → carregamento de cena → posicionamento do personagem.

### Objetivos do Design

- Remover toda a lógica de pré-carregamento de cenas
- Eliminar validações de proximidade e zonas de pré-carregamento
- Simplificar o código do TeleportPoint e TeleportManager
- Manter a qualidade das transições visuais
- Reduzir a complexidade da configuração no Inspector

## Architecture

### Fluxo Simplificado de Teleporte

```
Player Colide com TeleportPoint
         ↓
OnTriggerEnter2D detecta Player
         ↓
Valida se não está teletransportando
         ↓
Desabilita movimento do Player
         ↓
Inicia ExecuteTeleport()
         ↓
    [Same-Scene]              [Cross-Scene]
         ↓                          ↓
  Fade Out                     Fade Out
         ↓                          ↓
  Reposiciona Player      Carrega Nova Cena
         ↓                          ↓
  Reposiciona Câmera      Reposiciona Player
         ↓                          ↓
  Fade In                     Reposiciona Câmera
         ↓                          ↓
  Reabilita Movimento         Fade In
                                   ↓
                            Reabilita Movimento
```

### Componentes Afetados

1. **TeleportPoint.cs**
   - Remover lógica de proximidade
   - Remover CircleCollider2D de pré-carregamento
   - Simplificar OnTriggerEnter2D
   - Remover OnTriggerExit2D
   - Remover campos de configuração de pré-carregamento

2. **TeleportManager.cs**
   - Remover sistema de pré-carregamento completo
   - Remover métodos: PreloadScene, IsScenePreloaded, CancelPreload, PreloadSceneAsync
   - Remover estruturas de dados: preloadOperations, sceneLastUsedTime
   - Simplificar ExecuteCrossSceneTeleport para carregar diretamente
   - Remover lógica de cache LRU

## Components and Interfaces

### TeleportPoint (Modificado)

#### Campos Removidos

```csharp
// REMOVER
[SerializeField] private bool enablePreloading = true;
[SerializeField] private float preloadProximityRadius = 5f;
private CircleCollider2D preloadTrigger;
private bool isInPreloadZone = false;
```

#### Campos Mantidos

```csharp
// Configuração básica de teleporte
[SerializeField] private Vector3 destinationPosition;
[SerializeField] private TransitionEffect transitionEffect;
[SerializeField] private float delayBeforeFadeIn = 1f;

// Configuração cross-scene
[SerializeField] private bool isCrossSceneTeleport = false;
[SerializeField] private string destinationSceneName = "";

// Áudio
[SerializeField] private AudioClip teleportStartSound;
[SerializeField] private AudioClip teleportMidSound;
[SerializeField] private AudioClip teleportEndSound;

// Trigger
[SerializeField] private Vector2 triggerSize = new Vector2(1f, 1f);
[SerializeField] private Vector2 triggerOffset = Vector2.zero;

// Debug
[SerializeField] private bool enableDebugLogs = false;
[SerializeField] private bool enableGizmos = true;
[SerializeField] private Color gizmoColor = Color.cyan;
```

#### Método OnTriggerEnter2D Simplificado

```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    // Valida se é o Player
    if (!other.CompareTag("Player"))
    {
        if (enableDebugLogs)
            Debug.Log($"TeleportPoint: Objeto '{other.name}' não é Player, ignorando.", this);
        return;
    }

    // Previne múltiplos teletransportes simultâneos
    if (isTeleporting)
    {
        if (enableDebugLogs)
            Debug.Log("TeleportPoint: Já está teletransportando, ignorando trigger.", this);
        return;
    }

    if (enableDebugLogs)
        Debug.Log($"TeleportPoint: Player detectado, iniciando teletransporte.", this);

    // Inicia o processo de teletransporte imediatamente
    StartCoroutine(ExecuteTeleport());
}
```

#### Métodos Removidos

```csharp
// REMOVER COMPLETAMENTE
private void OnTriggerExit2D(Collider2D other) { }
private void ConfigurePreloadTrigger() { }
```

### TeleportManager (Modificado)

#### Campos Removidos

```csharp
// REMOVER
[SerializeField] private int maxPreloadedScenes = 2;
[SerializeField] private float unloadDelay = 5f;
private Dictionary<string, AsyncOperation> preloadOperations;
private Dictionary<string, float> sceneLastUsedTime;
```

#### Campos Mantidos

```csharp
// Singleton
public static TeleportManager Instance { get; private set; }

// Audio
[SerializeField] private AudioSource audioSource;
[SerializeField] [Range(0f, 1f)] private float defaultVolume = 1f;

// Estado
private bool isTeleporting = false;
```

#### Métodos Removidos

```csharp
// REMOVER COMPLETAMENTE
public void PreloadScene(string sceneName) { }
public bool IsScenePreloaded(string sceneName) { }
public float GetPreloadProgress(string sceneName) { }
public void CancelPreload(string sceneName) { }
private IEnumerator UnloadSceneDelayed(string sceneName, float delay) { }
private void MarkSceneAsUsed(string sceneName) { }
private void EnforceCacheLimit() { }
private void UnloadLeastRecentlyUsedScene() { }
private IEnumerator PreloadSceneAsync(string sceneName) { }
```

#### Método LoadAndTransferPlayer Simplificado

```csharp
private IEnumerator LoadAndTransferPlayer(
    string destinationSceneName,
    Vector3 destinationPosition,
    string previousSceneName,
    AudioClip midSound,
    bool enableDebugLogs)
{
    // Reproduz som do meio da transição
    PlayTeleportSound(midSound);

    if (enableDebugLogs)
    {
        Debug.Log($"TeleportManager: Carregando cena '{destinationSceneName}'...");
    }

    // Carrega a nova cena diretamente (sem pré-carregamento)
    AsyncOperation loadOperation = SceneManager.LoadSceneAsync(destinationSceneName, LoadSceneMode.Single);

    // Aguarda carregamento completo
    while (!loadOperation.isDone)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"TeleportManager: Progresso de carregamento: {loadOperation.progress * 100f}%");
        }
        yield return null;
    }

    if (enableDebugLogs)
    {
        Debug.Log($"TeleportManager: Cena '{destinationSceneName}' carregada. Posicionando player...");
    }

    // Aguarda um frame para garantir que a cena foi inicializada
    yield return null;

    // Posiciona o player na nova cena
    if (PlayerController.Instance != null)
    {
        PlayerController.Instance.transform.position = destinationPosition;

        // Posiciona a câmera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calcula posição da câmera mantendo offset
            Vector3 cameraOffset = mainCamera.transform.position - PlayerController.Instance.transform.position;
            mainCamera.transform.position = destinationPosition + cameraOffset;
        }

        if (enableDebugLogs)
        {
            Debug.Log($"TeleportManager: Player posicionado em {destinationPosition}");
        }
    }
}
```

## Data Models

Não há mudanças nos modelos de dados. As estruturas de dados removidas são:

- `Dictionary<string, AsyncOperation> preloadOperations`
- `Dictionary<string, float> sceneLastUsedTime`

## Error Handling

### Validações Mantidas

1. **Validação de Player**: Continua verificando se o objeto que colidiu tem a tag "Player"
2. **Validação de Teleporte em Progresso**: Continua prevenindo múltiplos teletransportes simultâneos
3. **Validação de Configuração**: Continua validando se destinationPosition, transitionEffect e componentes necessários estão configurados
4. **Validação de Cena**: Continua verificando se a cena de destino existe nas Build Settings

### Validações Removidas

1. **Validação de Proximidade**: Não verifica mais distância do player ao TeleportPoint
2. **Validação de Pré-carregamento**: Não verifica mais se a cena está pré-carregada
3. **Validação de Zona de Proximidade**: Não verifica mais entrada/saída de zona de pré-carregamento

### Tratamento de Erros

O tratamento de erros permanece o mesmo:

- Logs de erro para configurações inválidas
- Logs de warning para situações não críticas
- Try-finally para garantir que o movimento do player seja reabilitado mesmo em caso de erro

## Testing Strategy

### Testes Manuais Necessários

1. **Teste de Teleporte Same-Scene**
   - Criar TeleportPoint com isCrossSceneTeleport = false
   - Verificar se o player é teletransportado imediatamente ao colidir
   - Verificar se a transição visual funciona corretamente
   - Verificar se o player é posicionado corretamente

2. **Teste de Teleporte Cross-Scene**
   - Criar TeleportPoint com isCrossSceneTeleport = true
   - Verificar se a nova cena é carregada ao colidir
   - Verificar se o player é posicionado corretamente na nova cena
   - Verificar se a câmera é posicionada corretamente

3. **Teste de Múltiplas Colisões**
   - Verificar se múltiplas colisões rápidas não causam problemas
   - Verificar se a flag isTeleporting previne teletransportes simultâneos

4. **Teste de Configuração do Inspector**
   - Verificar se os campos de pré-carregamento foram removidos
   - Verificar se a interface está mais limpa e simples
   - Verificar se os Gizmos continuam funcionando

5. **Teste de Áudio**
   - Verificar se os sons de teleporte são reproduzidos corretamente
   - Verificar se funciona mesmo sem sons configurados (graceful degradation)

### Casos de Teste Específicos

1. **Colisão Imediata**: Player colide e é teletransportado sem delay
2. **Cena Não Existe**: Verificar mensagem de erro apropriada
3. **Player Não Existe**: Verificar mensagem de erro apropriada
4. **TransitionEffect Não Configurado**: Verificar mensagem de erro apropriada
5. **Teleporte Durante Teleporte**: Verificar que segunda tentativa é ignorada

## Implementation Notes

### Ordem de Implementação

1. **Fase 1**: Modificar TeleportPoint.cs
   - Remover campos de pré-carregamento
   - Simplificar OnTriggerEnter2D
   - Remover OnTriggerExit2D
   - Remover ConfigurePreloadTrigger
   - Atualizar OnDrawGizmos para não mostrar zona de proximidade

2. **Fase 2**: Modificar TeleportManager.cs
   - Remover campos de pré-carregamento
   - Remover métodos de pré-carregamento
   - Simplificar LoadAndTransferPlayer
   - Remover lógica de cache LRU
   - Atualizar Awake para não inicializar estruturas removidas

3. **Fase 3**: Testes
   - Testar teleporte same-scene
   - Testar teleporte cross-scene
   - Verificar que não há erros de compilação
   - Verificar que não há referências a código removido

### Considerações de Performance

- **Carregamento Direto**: Sem pré-carregamento, pode haver um pequeno delay durante a transição visual enquanto a cena carrega
- **Simplicidade vs Performance**: Trade-off aceitável para simplificar o código
- **Transição Visual**: O fade out mascara o tempo de carregamento da cena

### Compatibilidade

- **Cenas Existentes**: TeleportPoints existentes continuarão funcionando, mas os campos de pré-carregamento serão ignorados
- **Prefabs**: Prefabs de TeleportPoint precisarão ser atualizados para remover campos obsoletos
- **Scripts Externos**: Nenhum script externo deve estar chamando métodos de pré-carregamento do TeleportManager

## Diagrams

### Sequência de Teleporte Cross-Scene Simplificada

```
Player → TeleportPoint: OnTriggerEnter2D
TeleportPoint → TeleportPoint: Valida Player
TeleportPoint → PlayerController: DisableMovement()
TeleportPoint → TeleportManager: ExecuteCrossSceneTeleport()
TeleportManager → TeleportManager: Valida configuração
TeleportManager → AudioSource: PlaySound(startSound)
TeleportManager → TransitionEffect: FadeOut()
TransitionEffect → TeleportManager: OnMidTransition callback
TeleportManager → SceneManager: LoadSceneAsync(destinationScene)
SceneManager → TeleportManager: Cena carregada
TeleportManager → PlayerController: SetPosition(destinationPosition)
TeleportManager → Camera: SetPosition(destinationPosition + offset)
TeleportManager → TransitionEffect: FadeIn()
TeleportManager → AudioSource: PlaySound(endSound)
TeleportManager → PlayerController: EnableMovement()
```

### Comparação: Antes vs Depois

**ANTES (Complexo)**

```
Player se aproxima → Pré-carrega cena → Player colide → Ativa cena pré-carregada
                   ↓
            Player se afasta → Cancela pré-carregamento → Descarrega cena
```

**DEPOIS (Simples)**

```
Player colide → Carrega e ativa cena → Posiciona player
```
