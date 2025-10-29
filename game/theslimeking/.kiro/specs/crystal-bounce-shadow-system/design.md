# Design Document - Sistema de Quicadas e Sombra Dinâmica

## Overview

Este documento descreve o design técnico para reconstruir o sistema de quicadas e sombra dinâmica para objetos coletáveis. O sistema atual possui problemas de sincronização e lógica que serão corrigidos através de uma refatoração focada em:

1. **Separação de responsabilidades** - Dividir lógica de física, sombra e coleta
2. **Máquina de estados clara** - Gerenciar transições entre lançamento, quicadas e parada
3. **Sincronização precisa** - Garantir que sombra e colliders respondam corretamente ao estado do objeto
4. **Integração robusta** - Coordenar BounceHandler e ItemCollectable sem conflitos

## Architecture

### Diagrama de Estados

```
[Instanciado] 
    ↓
[Inicializado] → Colliders desabilitados
    ↓
[Lançado] → Aplica força inicial
    ↓
[Quicando] → Sequência de quicadas com força decrescente
    ↓           ↓ (a cada frame)
    ↓       [Atualiza Sombra] → Escala baseada em velocidade Y
    ↓
[Parado] → Zera velocidades, torna Kinematic
    ↓
[Aguardando Delay] → Espera tempo configurado
    ↓
[Pronto para Coleta] → Habilita colliders, ativa atração
```

### Fluxo de Dados

```
BounceHandler (Física e Quicadas)
    ↓ Estado do movimento
    ↓ Velocidade vertical
    ↓
ShadowController (Sombra)
    ↓ Escala calculada
    ↓ Posição atualizada
    ↓
ItemCollectable (Coleta)
    ↓ Delay de ativação
    ↓ Atração magnética
```

## Components and Interfaces

### 1. BounceHandler (Refatorado)

**Responsabilidades:**

- Gerenciar lançamento inicial
- Executar sequência de quicadas
- Controlar estado do movimento
- Coordenar habilitação de colliders
- Notificar mudanças de estado

**Principais Mudanças:**

- Adicionar enum `BounceState` para estados claros
- Separar lógica de sombra em método dedicado
- Adicionar controle de colliders
- Melhorar sincronização com ItemCollectable
- Corrigir cálculo de altura simulada da sombra

**Estados:**

```csharp
public enum BounceState
{
    NotLaunched,    // Objeto criado mas não lançado
    Launching,      // Aplicando força inicial
    Bouncing,       // Executando quicadas
    Stopping,       // Parando movimento
    Stopped,        // Completamente parado
    ReadyForCollection // Colliders habilitados, pronto para coleta
}
```

**Interface Pública:**

```csharp
// Propriedades
BounceState CurrentState { get; }
bool IsMoving { get; }
bool IsReadyForCollection { get; }

// Métodos
void LaunchItem();
void LaunchItem(float force, float angle);
void StopMovementManually();
void ResetLaunch();
void EnableColliders();
void DisableColliders();
```

### 2. ShadowController (Novo Componente Opcional)

**Responsabilidades:**

- Atualizar escala da sombra baseada em altura simulada
- Manter posição sincronizada com objeto pai
- Gerenciar transições suaves de escala

**Vantagens da Separação:**

- Código mais limpo e focado
- Reutilizável em outros contextos
- Mais fácil de testar e debugar
- Pode ser desabilitado independentemente

**Interface Pública:**

```csharp
// Configuração
void Initialize(Transform parentObject, Rigidbody2D parentRigidbody);
void SetScaleRange(float min, float max);
void SetOffset(Vector2 offset);

// Controle
void UpdateShadow();
void ResetToMaxScale();
void SetEnabled(bool enabled);
```

### 3. ItemCollectable (Ajustes Mínimos)

**Mudanças Necessárias:**

- Adicionar método público `SetColliderEnabled(bool enabled)`
- Expor propriedade `ActivationDelay` para sincronização
- Adicionar callback opcional quando delay expira
- Melhorar integração com BounceHandler

**Nova Interface:**

```csharp
// Propriedades
float ActivationDelay { get; }
bool IsActivationDelayComplete { get; }

// Métodos
void SetColliderEnabled(bool enabled);
void OnActivationDelayComplete(Action callback);
```

## Data Models

### BounceData (Struct para dados de quicada)

```csharp
[System.Serializable]
public struct BounceData
{
    public Vector2 initialDirection;
    public float initialForce;
    public int currentBounceIndex;
    public float currentBounceInterval;
    public Vector3 initialPosition;
    
    public void Reset()
    {
        initialDirection = Vector2.zero;
        initialForce = 0f;
        currentBounceIndex = 0;
        currentBounceInterval = 0f;
        initialPosition = Vector3.zero;
    }
}
```

### ShadowData (Struct para configuração de sombra)

```csharp
[System.Serializable]
public struct ShadowData
{
    public GameObject shadowObject;
    public float minScale;
    public float maxScale;
    public Vector2 offset;
    public float maxSimulatedHeight;
    public Vector3 initialScale;
    
    public bool IsValid => shadowObject != null;
}
```

## Error Handling

### Validações no Awake/Start

1. **BounceHandler:**
   - Verificar presença de Rigidbody2D (RequireComponent garante)
   - Validar valores de configuração (min < max, valores positivos)
   - Verificar se shadowObject é filho do objeto principal
   - Logar warnings para configurações inválidas

2. **ShadowController:**
   - Verificar se objeto pai existe
   - Validar Rigidbody2D do pai
   - Garantir que escala inicial é válida

3. **ItemCollectable:**
   - Verificar presença de Collider2D
   - Validar ItemData configurado
   - Garantir que player existe na cena

### Tratamento de Erros em Runtime

1. **Null Reference Protection:**
   - Sempre verificar componentes antes de usar
   - Usar operador `?.` para chamadas seguras
   - Retornar early se componentes críticos faltam

2. **Estado Inválido:**
   - Prevenir múltiplos lançamentos
   - Ignorar comandos em estados incorretos
   - Logar warnings para operações inválidas

3. **Sincronização:**
   - Cancelar Invokes pendentes ao destruir
   - Limpar callbacks ao desabilitar
   - Resetar estados ao reinicializar

## Testing Strategy

### Testes Manuais no Editor

1. **Teste de Lançamento Básico:**
   - Instanciar objeto
   - Verificar lançamento automático
   - Observar trajetória e quicadas
   - Confirmar parada após quicadas

2. **Teste de Sombra:**
   - Observar escala da sombra durante movimento
   - Verificar se diminui ao subir
   - Verificar se aumenta ao descer
   - Confirmar tamanho máximo ao parar

3. **Teste de Colliders:**
   - Verificar que colliders estão desabilitados ao instanciar
   - Tentar coletar durante quicadas (deve falhar)
   - Aguardar parada e delay
   - Confirmar coleta após habilitação

4. **Teste de Integração:**
   - Instanciar múltiplos objetos
   - Verificar que todos funcionam independentemente
   - Testar coleta de objetos em diferentes estados
   - Confirmar ausência de erros no console

### Cenários de Teste

| Cenário | Configuração | Resultado Esperado |
|---------|--------------|-------------------|
| Lançamento padrão | 2 quicadas, delay 0.5s | Lança, quica 2x, para, habilita após 0.5s |
| Sem quicadas | 0 quicadas, delay 0s | Lança, para imediatamente, habilita |
| Sem sombra | shadowObject = null | Funciona normalmente sem erros |
| Força customizada | LaunchItem(10f, 90f) | Lança com força 10 a 90° |
| Reset durante quicada | ResetLaunch() chamado | Cancela quicadas, reseta estado |
| Coleta prematura | Tentar coletar antes de parar | Ignora coleta |
| Múltiplos objetos | 5 objetos simultâneos | Todos funcionam independentemente |

### Debug Tools

1. **Gizmos Visuais:**
   - Desenhar trajetória prevista
   - Mostrar raio de atração
   - Indicar estado atual com cores

2. **Logs Estruturados:**
   - Prefixo `[BounceHandler]` para identificação
   - Incluir nome do objeto e timestamp
   - Níveis: Info, Warning, Error

3. **Context Menu Actions:**
   - `[ContextMenu("Test Launch")]` - Testar lançamento
   - `[ContextMenu("Force Stop")]` - Parar imediatamente
   - `[ContextMenu("Enable Colliders")]` - Habilitar colliders
   - `[ContextMenu("Debug State")]` - Mostrar estado atual

## Implementation Notes

### Problema Identificado no Código Atual

Analisando o código existente, os principais problemas são:

1. **Cálculo de Sombra Incorreto:**

   ```csharp
   // PROBLEMA: Usa velocidade diretamente sem considerar direção
   float simulatedHeight = Mathf.Max(0f, verticalVelocity / maxSimulatedHeight);
   ```

   - Velocidade negativa (descendo) resulta em altura negativa
   - Mathf.Max(0f, ...) força zero quando descendo
   - Sombra não aumenta ao descer

2. **Falta de Controle de Colliders:**
   - Colliders ficam ativos desde o início
   - Permite coleta durante quicadas
   - Não sincroniza com delay de ativação

3. **Sincronização com ItemCollectable:**
   - Sistemas funcionam independentemente
   - Delay de ativação não considera estado de movimento
   - Pode ativar atração enquanto ainda está quicando

### Solução Proposta

1. **Corrigir Cálculo de Sombra:**

   ```csharp
   // Usar valor absoluto da velocidade para altura
   float speed = Mathf.Abs(_rigidbody2D.linearVelocity.y);
   float normalizedHeight = Mathf.Clamp01(speed / maxSimulatedHeight);
   float shadowScale = Mathf.Lerp(maxShadowScale, minShadowScale, normalizedHeight);
   ```

2. **Adicionar Controle de Colliders:**

   ```csharp
   private void DisableAllColliders()
   {
       var colliders = GetComponents<Collider2D>();
       foreach (var col in colliders)
       {
           col.enabled = false;
       }
   }
   
   private void EnableAllColliders()
   {
       var colliders = GetComponents<Collider2D>();
       foreach (var col in colliders)
       {
           col.enabled = true;
       }
   }
   ```

3. **Sincronizar com ItemCollectable:**

   ```csharp
   private void OnMovementStopped()
   {
       _currentState = BounceState.Stopped;
       
       // Aguarda delay antes de habilitar colliders
       var itemCollectable = GetComponent<ItemCollectable>();
       float delay = itemCollectable != null ? itemCollectable.ActivationDelay : 0f;
       
       if (delay > 0f)
       {
           Invoke(nameof(EnableCollidersAndNotify), delay);
       }
       else
       {
           EnableCollidersAndNotify();
       }
   }
   ```

### Ordem de Implementação

1. **Fase 1: Corrigir BounceHandler**
   - Adicionar enum BounceState
   - Corrigir cálculo de sombra
   - Adicionar controle de colliders
   - Melhorar máquina de estados

2. **Fase 2: Ajustar ItemCollectable**
   - Adicionar métodos públicos para controle de collider
   - Expor propriedades necessárias
   - Adicionar callback de delay

3. **Fase 3: Integração**
   - Sincronizar delays
   - Testar fluxo completo
   - Ajustar valores no Inspector

4. **Fase 4: Polimento**
   - Adicionar debug tools
   - Melhorar logs
   - Documentar parâmetros

### Considerações de Performance

1. **Update da Sombra:**
   - Só executar quando `_hasBeenLaunched && !IsMovementStopped`
   - Cachear componentes no Awake
   - Evitar cálculos desnecessários

2. **Colliders:**
   - Cachear array de colliders no Awake
   - Evitar GetComponents repetidos
   - Desabilitar quando não necessário

3. **Invokes:**
   - Usar CancelInvoke ao destruir
   - Limpar callbacks pendentes
   - Evitar múltiplos Invokes do mesmo método

## Diagrama de Sequência

```
Instanciação → Awake → Start → LaunchItem
                ↓        ↓         ↓
         Cache comps  Disable   Apply force
                              colliders
                                  ↓
                            [Loop de Quicadas]
                                  ↓
                            ProcessNextBounce
                                  ↓
                            (repete N vezes)
                                  ↓
                            StopMovement
                                  ↓
                         Aguarda Delay
                                  ↓
                         EnableColliders
                                  ↓
                      Notifica ItemCollectable
                                  ↓
                         Ativa Atração
```

## Configuration Examples

### Configuração Padrão (Cristal Pequeno)

```
minLaunchForce: 2
maxLaunchForce: 5
bounceCount: 2
timeToBounce: 0.1
forceReductionFactor: 0.8
activationDelay: 0.5
minShadowScale: 0.5
maxShadowScale: 1.0
```

### Configuração Alternativa (Cristal Grande)

```
minLaunchForce: 3
maxLaunchForce: 7
bounceCount: 3
timeToBounce: 0.15
forceReductionFactor: 0.75
activationDelay: 0.8
minShadowScale: 0.4
maxShadowScale: 1.2
```

### Configuração Sem Quicadas (Drop Direto)

```
minLaunchForce: 1
maxLaunchForce: 2
bounceCount: 0
timeToBounce: 0
forceReductionFactor: 1.0
activationDelay: 0.2
```
