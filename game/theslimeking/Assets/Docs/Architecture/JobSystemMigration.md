# Job System Migration Guide - NPCBehaviorController

## 🎯 Visão Geral

Este documento detalha como migrar o sistema de comportamento de NPCs (`NPCBehaviorController`) para o Unity Job System e ECS (Entity Component System) para máxima performance e escalabilidade.

## 📋 Estado Atual vs Futuro

### Estado Atual (MonoBehaviour)

- ✅ Implementado: Sistema de comportamento baseado em `MonoBehaviour`
- ✅ Otimizado: LOD system, StringToHash, NonAlloc APIs
- ✅ Performance: Suporta 100+ NPCs com boa performance
- ❌ Limitação: Single-threaded execution
- ❌ Limitação: Garbage collection em cenários extremos

### Estado Futuro (Job System + ECS)

- 🚀 Multi-threaded: Processamento paralelo de múltiplos NPCs
- 🚀 Data-Oriented: Estruturas otimizadas para cache CPU
- 🚀 Burst Compilation: Código C# compilado para assembly nativo
- 🚀 Escalabilidade: Suporte a 1000+ NPCs simultâneos

## 🏗️ Estrutura de Migração

### Fase 1: Preparação de Dados (Já Implementada)

O código atual já está estruturado de forma compatível com ECS:

```csharp
// ✅ Struct para configuração (ECS-friendly)
[System.Serializable]
public struct NPCBehaviorConfig
{
    public float visionRange;
    public float visionAngle;
    // ... outros campos primitivos
}

// ✅ Enum para estados (Burst-compatible)
public enum NPCBehaviorState
{
    Idle = 0,
    Patrol = 1,
    // ... outros estados
}
```

### Fase 2: Componentes ECS

Criar componentes ECS equivalentes:

```csharp
using Unity.Entities;
using Unity.Mathematics;

// Componente de configuração (read-only)
public struct NPCBehaviorConfigComponent : IComponentData
{
    public float visionRange;
    public float visionAngle;
    public float attackRange;
    public float chaseSpeed;
    // ... outros campos
}

// Componente de estado atual
public struct NPCBehaviorStateComponent : IComponentData
{
    public NPCBehaviorState currentState;
    public NPCBehaviorState previousState;
    public float stateChangeTime;
}

// Componente de detecção
public struct NPCDetectionComponent : IComponentData
{
    public bool playerDetected;
    public bool hasLineOfSight;
    public float3 lastKnownPlayerPosition;
    public float lastDetectionTime;
}

// Componente de timing
public struct NPCTimingComponent : IComponentData
{
    public float nextAttackTime;
    public float alertStartTime;
    public float returnStartTime;
    public float nextUpdateTime;
}

// Componente LOD
public struct NPCLODComponent : IComponentData
{
    public LODLevel currentLOD;
    public float playerDistanceSqr;
    public int updateCounter;
}
```

### Fase 3: Systems ECS

#### 3.1 Detection System

```csharp
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
public partial struct NPCDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = GetSingleton<PlayerComponent>().entity;
        var playerTransform = GetComponent<LocalTransform>(playerEntity);
        
        var detectionJob = new NPCDetectionJob
        {
            playerPosition = playerTransform.Position,
            physicsWorld = GetSingleton<PhysicsWorldSingleton>(),
            deltaTime = SystemAPI.Time.DeltaTime
        };
        
        state.Dependency = detectionJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct NPCDetectionJob : IJobEntity
{
    [ReadOnly] public float3 playerPosition;
    [ReadOnly] public PhysicsWorld physicsWorld;
    [ReadOnly] public float deltaTime;
    
    public void Execute(
        ref NPCDetectionComponent detection,
        in NPCBehaviorConfigComponent config,
        in LocalTransform transform,
        in NPCLODComponent lod)
    {
        // LOD check
        if (lod.currentLOD == LODLevel.Disabled) return;
        
        // Distance check
        float3 directionToPlayer = playerPosition - transform.Position;
        float distanceSqr = math.lengthsq(directionToPlayer);
        
        if (distanceSqr > config.visionRange * config.visionRange)
        {
            detection.playerDetected = false;
            return;
        }
        
        // Vision cone check
        float3 forward = math.forward(transform.Rotation);
        float dot = math.dot(math.normalize(directionToPlayer), forward);
        float angleThreshold = math.cos(math.radians(config.visionAngle * 0.5f));
        
        if (dot < angleThreshold)
        {
            detection.playerDetected = false;
            return;
        }
        
        // Raycast check
        var raycastInput = new RaycastInput
        {
            Start = transform.Position,
            End = playerPosition,
            Filter = CollisionFilter.Default
        };
        
        detection.hasLineOfSight = !physicsWorld.CastRay(raycastInput, out _);
        detection.playerDetected = detection.hasLineOfSight;
        
        if (detection.playerDetected)
        {
            detection.lastKnownPlayerPosition = playerPosition;
            detection.lastDetectionTime = time;
        }
    }
}
```

#### 3.2 State Machine System

```csharp
[BurstCompile]
public partial struct NPCStateMachineSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var stateMachineJob = new NPCStateMachineJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
            time = (float)SystemAPI.Time.ElapsedTime
        };
        
        state.Dependency = stateMachineJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct NPCStateMachineJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float time;
    
    public void Execute(
        ref NPCBehaviorStateComponent stateComp,
        ref NPCTimingComponent timing,
        in NPCDetectionComponent detection,
        in NPCBehaviorConfigComponent config,
        in LocalTransform transform)
    {
        // State machine logic using switch
        switch (stateComp.currentState)
        {
            case NPCBehaviorState.Idle:
                UpdateIdleState(ref stateComp, in detection, in config);
                break;
                
            case NPCBehaviorState.Alert:
                UpdateAlertState(ref stateComp, ref timing, in detection, in config, time);
                break;
                
            case NPCBehaviorState.Chase:
                UpdateChaseState(ref stateComp, in detection, in config, in transform);
                break;
                
            case NPCBehaviorState.Attack:
                UpdateAttackState(ref stateComp, ref timing, in detection, in config, time);
                break;
                
            case NPCBehaviorState.Return:
                UpdateReturnState(ref stateComp, ref timing, in detection, in transform, time);
                break;
        }
    }
    
    [BurstCompile]
    private static void UpdateIdleState(
        ref NPCBehaviorStateComponent stateComp,
        in NPCDetectionComponent detection,
        in NPCBehaviorConfigComponent config)
    {
        if (detection.playerDetected)
        {
            stateComp.previousState = stateComp.currentState;
            stateComp.currentState = NPCBehaviorState.Alert;
            stateComp.stateChangeTime = time;
        }
    }
    
    // ... outras funções de estado
}
```

#### 3.3 Movement System

```csharp
[BurstCompile]
public partial struct NPCMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var movementJob = new NPCMovementJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        
        state.Dependency = movementJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct NPCMovementJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    
    public void Execute(
        ref PhysicsVelocity velocity,
        in NPCBehaviorStateComponent stateComp,
        in NPCDetectionComponent detection,
        in NPCBehaviorConfigComponent config,
        in LocalTransform transform)
    {
        float3 targetVelocity = float3.zero;
        
        switch (stateComp.currentState)
        {
            case NPCBehaviorState.Chase:
                float3 direction = math.normalize(detection.lastKnownPlayerPosition - transform.Position);
                targetVelocity = direction * config.chaseSpeed;
                break;
                
            case NPCBehaviorState.Return:
                // Logic for returning to initial position
                break;
                
            // ... outros estados
        }
        
        velocity.Linear = targetVelocity;
    }
}
```

### Fase 4: LOD System ECS

```csharp
[BurstCompile]
public partial struct NPCLODSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var playerEntity = GetSingleton<PlayerComponent>().entity;
        var playerPosition = GetComponent<LocalTransform>(playerEntity).Position;
        
        var lodJob = new NPCLODJob
        {
            playerPosition = playerPosition
        };
        
        state.Dependency = lodJob.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct NPCLODJob : IJobEntity
{
    [ReadOnly] public float3 playerPosition;
    
    public void Execute(
        ref NPCLODComponent lod,
        in LocalTransform transform)
    {
        float distanceSqr = math.lengthsq(playerPosition - transform.Position);
        lod.playerDistanceSqr = distanceSqr;
        
        // Determina LOD level baseado na distância
        const float disableDistSqr = 25f * 25f;  // 25m
        const float maxBehaviorDistSqr = 20f * 20f;  // 20m
        const float reducedUpdateDistSqr = 15f * 15f;  // 15m
        
        if (distanceSqr >= disableDistSqr)
            lod.currentLOD = LODLevel.Disabled;
        else if (distanceSqr >= maxBehaviorDistSqr)
            lod.currentLOD = LODLevel.Minimal;
        else if (distanceSqr >= reducedUpdateDistSqr)
            lod.currentLOD = LODLevel.Low;
        else if (distanceSqr >= (reducedUpdateDistSqr * 0.5f))
            lod.currentLOD = LODLevel.Medium;
        else
            lod.currentLOD = LODLevel.Full;
    }
}
```

## 🚀 Plano de Migração Passo-a-Passo

### Etapa 1: Setup do Projeto

1. Instalar Unity.Entities package
2. Instalar Unity.Physics package
3. Instalar Unity.Collections package
4. Configurar Assembly Definitions

### Etapa 2: Conversão Híbrida

1. Manter `NPCBehaviorController` existente
2. Adicionar componentes ECS em paralelo
3. Implementar authoring components
4. Testar sistema híbrido

### Etapa 3: Migration Gradual

1. Migrar sistema de detecção primeiro
2. Migrar state machine
3. Migrar sistema de movimento
4. Migrar sistema LOD

### Etapa 4: Otimização Final

1. Tune job batch sizes
2. Implementar job dependencies otimizadas
3. Profile e otimizar hot paths
4. Remover MonoBehaviour legacy

### Etapa 5: Cleanup

1. Remover código MonoBehaviour antigo
2. Cleanup assembly references
3. Documentar nova arquitetura

## 📊 Performance Esperada

### Benchmark Targets

| Métrica | MonoBehaviour | Job System | Melhoria |
|---------|---------------|------------|----------|
| NPCs Simultâneos | 100 | 1000+ | 10x |
| CPU Usage | 60-80% | 30-40% | ~50% |
| Memory Allocations | 2-3 MB/s | <0.5 MB/s | 80% |
| Update Rate (100 NPCs) | 30-45 FPS | 60+ FPS | +50% |

### Memory Layout Optimization

```csharp
// Data layout otimizado para cache CPU
[StructLayout(LayoutKind.Sequential)]
public struct NPCDataChunk
{
    // Hot data (acessado todo frame)
    public float3 position;
    public NPCBehaviorState state;
    public bool playerDetected;
    
    // Warm data (acessado menos frequentemente)
    public float visionRange;
    public float attackRange;
    
    // Cold data (acessado raramente)
    public NPCType npcType;
    public int entityId;
}
```

## 🔧 Ferramentas de Debug

### Entity Debugger

1. Unity > Window > Entities > Entity Debugger
2. Filtrar por NPCBehaviorStateComponent
3. Monitorar performance em tempo real

### Job Profiler

1. Unity Profiler > Jobs tab
2. Monitorar job execution times
3. Identificar bottlenecks

### Memory Profiler

1. Package Manager > Memory Profiler
2. Capturar snapshots
3. Analisar memory layout

## 📝 Considerações Importantes

### Limitações do Job System

- Não pode acessar MonoBehaviour diretamente
- Requer estruturas Blittable
- Debugging mais complexo
- Learning curve acentuada

### Hybrid Workflow

- GameObjects para rendering/audio
- Entities para logic/data
- Conversion systems para bridge

### Best Practices

- Minimize job dependencies
- Batch similar operations
- Use NativeContainers eficientemente
- Profile constantemente

## 🎯 Conclusão

Esta migração permitirá:

- **Escalabilidade**: 10x mais NPCs simultâneos
- **Performance**: 50% menos uso de CPU
- **Futuro-proof**: Preparado para DOTS roadmap
- **Manutenibilidade**: Código mais limpo e testável

O sistema atual já está estruturado de forma ECS-friendly, facilitando a migração gradual sem quebrar funcionalidades existentes.
