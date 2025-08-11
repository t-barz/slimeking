# Guia de Performance no Slime King

## Índice
1. [Sistemas de Pooling](#sistemas-de-pooling)
2. [Otimização de Componentes](#otimização-de-componentes)
3. [Sistema de Interação](#sistema-de-interação)
4. [Efeitos e Partículas](#efeitos-e-partículas)
5. [Sistema de Drops](#sistema-de-drops)
6. [Depuração e Profiling](#depuração-e-profiling)

## Sistemas de Pooling

### ItemPool
Gerencia pools de objetos para minimizar alocação/destruição de memória.

#### Configuração:
```
GameObject: ItemPool
└── ItemPool.cs
    ├── Default Pool Size: 10
    └── Max Pool Size: 20
```

#### Uso em Prefabs:
1. Adicione `ICollectable` aos itens dropáveis:
```csharp
public class CollectableItem : MonoBehaviour, ICollectable
{
    public event Action OnCollected;
    
    void Collect()
    {
        // Lógica de coleta
        OnCollected?.Invoke();
    }
}
```

2. Configure física (opcional):
```
Rigidbody2D
├── Body Type: Dynamic
├── Collision Detection: Continuous
└── Interpolate: Interpolate
```

### CombatEffectsPool
Gerencia efeitos visuais e sonoros.

#### Configuração:
```
GameObject: CombatEffectsManager
└── CombatEffectsManager.cs
    ├── Pool Initial Size: 10
    ├── Pool Max Size: 20
    ├── Hit Effect Prefab
    ├── Hit Sounds[]
    └── Critical Hit Sounds[]
```

## Otimização de Componentes

### Práticas Recomendadas

1. Caching de Componentes:
```csharp
private Transform cachedTransform;
private Dictionary<GameObject, Rigidbody2D> cachedRigidbodies;

void Awake()
{
    cachedTransform = transform;
    cachedRigidbodies = new Dictionary<GameObject, Rigidbody2D>();
}
```

2. Pré-alocação de Coleções:
```csharp
// Melhor performance
var list = new List<T>(expectedSize);
var dict = new Dictionary<K,V>(expectedCapacity);
```

3. Minimização de GetComponent:
```csharp
// Ruim
void Update() {
    GetComponent<T>(); // chamada todo frame
}

// Bom
private T cachedComponent;
void Awake() {
    cachedComponent = GetComponent<T>(); // uma vez só
}
```

## Sistema de Interação

### Hierarquia de Objetos:
```
InteractiveWorldObject
├── Sprite Renderer
├── Animator
├── Collider2D
├── Rigidbody2D (se necessário)
└── Particle System (opcional)
```

### Configuração do Animator:
```
Parameters:
├── Shake (Trigger)
└── Destroy (Trigger)

States:
├── Idle
├── Shake
└── Destroy
```

## Efeitos e Partículas

### Otimização de Partículas:
1. Configuração Recomendada:
```
Particle System
├── Max Particles: ≤ 50
├── Simulation Space: World
└── Prewarm: False
```

2. Culling:
```
Particle System
└── Culling Mode: Pause and Catch-up
```

### Hierarquia de Efeitos:
```
Effect Prefab
├── Main Particle System
└── Sub Systems
    ├── Uses Auto-destruction
    └── Shared Materials
```

## Sistema de Drops

### Configuração de LootItem:
```csharp
[System.Serializable]
public struct LootItem
{
    public GameObject prefab;    // Referência ao item
    [Range(0, 100)]
    public float dropWeight;     // Peso no sorteio
    [Min(1)]
    public int maxQuantity;     // Quantidade máxima
}
```

### Exemplo de Configuração:
```
Drops:
├── Common (peso: 100)
│   └── Max Quantity: 5
├── Uncommon (peso: 60)
│   └── Max Quantity: 3
└── Rare (peso: 20)
    └── Max Quantity: 1
```

## Depuração e Profiling

### Ferramentas Recomendadas:
1. Unity Profiler
   - Memory Profiling
   - CPU Usage
   - Physics Performance

2. Frame Debugger
   - Batching
   - Draw Calls
   - Render Pipeline

### Comandos Úteis:
```csharp
[Conditional("UNITY_EDITOR")]
private void DebugDrop(GameObject item, Vector3 position)
{
    Debug.DrawLine(transform.position, position, Color.yellow, 1f);
    Debug.Log($"Drop: {item.name} at {position}");
}
```

### Performance Targets:
- CPU: < 16ms por frame
- Drawcalls: < 100 por frame
- Batches: < 50 por frame
- Alocações: < 1KB por frame

### Checklist de Otimização:
- [ ] Usar Object Pooling
- [ ] Cachear componentes
- [ ] Pré-alocar coleções
- [ ] Minimizar física
- [ ] Otimizar partículas
- [ ] Monitorar GC
