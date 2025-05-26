# Configuração de Objetos Interativos

Este guia explica como configurar objetos interativos no Slime King, incluindo árvores, arbustos e outros elementos destrutíveis.

## Estrutura Básica

### Hierarquia do Objeto
```
InteractiveObject
├── Sprite
├── Animator
├── Particle System
├── Collider2D
└── InteractiveWorldObject.cs
```

### Componentes Necessários

1. **Animator**
   - Controller com estados: Idle, Shake, Destroy
   - Parameters:
     ```
     Shake (Trigger)
     Destroy (Trigger)
     ```

2. **Collider2D**
   - Trigger ou não, dependendo da interação desejada
   - Composite Collider para formas complexas

3. **Particle System** (opcional)
   - Localizado em um child object "particulas"
   - Auto-destruction habilitado
   - Prewarm desabilitado

## Configuração no Inspector

### Health Settings
```
Max Health: 1-100
└── Quantidade de dano que o objeto pode receber
```

### Interaction Settings
```
Interactor Tags
├── Player
└── Creature
```

### Destruction Settings
```
Is Destructible: true/false
Hit Resistance: 1+
```

### Drop Settings
```
Drop Rate: 0-100%
Max Drops: 1+
Drop Force: 2 (padrão)
```

### Visual Effects
```
Hit Effect Prefab
Particle System
```

### Audio
```
Interaction Sound
Volume: 0-1
```

## LootTable Configuration

### Estrutura de Drops
```csharp
[System.Serializable]
public struct LootItem
{
    public GameObject prefab;
    [Range(0, 100)]
    public float dropWeight;
    [Min(1)]
    public int maxQuantity;
}
```

### Exemplo de Configuração
```
Common Item (Moedas)
├── Drop Weight: 100
└── Max Quantity: 5

Uncommon Item (Materiais)
├── Drop Weight: 60
└── Max Quantity: 3

Rare Item (Poções)
├── Drop Weight: 20
└── Max Quantity: 1
```

## Animação

### States
```
Idle
├── Loop: true
└── Transitions:
    ├── -> Shake (via Trigger)
    └── -> Destroy (via Trigger)

Shake
├── Duration: 0.5s
└── Transitions:
    └── -> Idle (quando terminar)

Destroy
├── Duration: 1s
└── Behavior:
    └── Destroy object when done
```

### Parâmetros
```yaml
Triggers:
  - Shake
  - Destroy

Floats:
  - ShakeIntensity
```

## Scripts Relacionados

### InteractiveWorldObject.cs
```csharp
[RequireComponent(typeof(Animator))]
public class InteractiveWorldObject : Interactable, IDamageable
{
    // Configure no Inspector
}
```

### Exemplo de Uso
```csharp
// Damage
interactiveObject.TakeDamage(10f);

// Interaction
interactiveObject.OnInteract();

// Manual Destruction
interactiveObject.InitiateDestruction();
```

## Prefab Setup

1. Crie o objeto base
2. Adicione componentes
3. Configure valores
4. Configure animações
5. Teste interações
6. Crie prefab

## Best Practices

### Performance
- Use object pooling para drops
- Otimize partículas
- Cache componentes
- Minimize física

### Design
- Mantenha consistência visual
- Use feedback claro
- Teste diferentes configurações

### Audio
- Use sons curtos
- Aplique variação de pitch
- Configure spatialização

## Debug

### Gizmos
```csharp
void OnDrawGizmos()
{
    // Draw interaction range
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, interactionRange);
}
```

### Logs
```csharp
// Em modo debug
Debug.Log($"Damage taken: {damage}, Health: {currentHealth}");
Debug.Log($"Spawned item: {item.name} at {position}");
```
