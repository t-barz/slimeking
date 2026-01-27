# Guia de Aplicação das Otimizações de Performance

**Data**: 2026-01-26  
**Cena**: InitialForest

## Status Atual

✅ **Sistemas Criados** (Fase 1, 2 e 3):
- `PlayerCache.cs` - Cache estático para referências do Player
- `UnifiedLODSystem.cs` - Sistema LOD consolidado e otimizado
- `ObjectPool.cs` - Sistema de pooling genérico
- `StaggeredUpdateManager.cs` - Sistema de updates escalonados
- `ItemCollectable.cs` - Otimizado com trigger-based attraction
- `PlayerController.cs` - Intervalo de stealth aumentado
- `CameraManager.cs` - Update() removido

❌ **Pendente de Aplicação na Cena**:
- Substituir sistemas LOD antigos
- Adicionar StaggeredUpdateManager
- Criar pools de cristais

---

## Instruções de Aplicação Manual

### 1. Substituir Sistema LOD Antigo

**Localização**: GameObject `= SYSTEMS/TreeLODSystem` (já foi removido)

**Passos**:
1. No Unity Editor, vá para a cena `InitialForest`
2. Na hierarquia, clique com botão direito em `= SYSTEMS`
3. Selecione `Create Empty` e renomeie para `UnifiedLODSystem`
4. Com o GameObject selecionado, clique em `Add Component`
5. Digite `UnifiedLODSystem` e adicione o componente
6. Configure os parâmetros no Inspector:
   - **Enable LOD**: ✓ (marcado)
   - **Update Interval**: 0.5
   - **Near Distance**: 15
   - **Medium Distance**: 25
   - **Far Distance**: 35
   - **Cull Distance**: 50
   - **Target Tags**: Adicione 2 elementos:
     - Element 0: `WindShaker`
     - Element 1: `Prop`
   - **Use Spatial Partitioning**: ☐ (desmarcado, ativa automaticamente se >100 objetos)
   - **Show Debug**: ☐ (desmarcado)
   - **Show Gizmos**: ☐ (desmarcado)

### 2. Adicionar Staggered Update Manager

**Passos**:
1. Na hierarquia, clique com botão direito em `= SYSTEMS`
2. Selecione `Create Empty` e renomeie para `StaggeredUpdateManager`
3. Com o GameObject selecionado, clique em `Add Component`
4. Digite `StaggeredUpdateManager` e adicione o componente
5. Configure os parâmetros no Inspector:
   - **Updates Per Frame**: 10
   - **Show Debug**: ☐ (desmarcado)

**Nota**: Este manager será automaticamente `DontDestroyOnLoad` quando o jogo iniciar.

### 3. Criar Pool de Cristais

**Passos**:
1. Na hierarquia, clique com botão direito na raiz
2. Selecione `Create Empty` e renomeie para `CrystalPool`
3. Com o GameObject selecionado, clique em `Add Component`
4. Digite `ObjectPool` e adicione o componente
5. Configure os parâmetros no Inspector:
   - **Prefab**: Arraste um prefab de cristal de `Assets/_Prefabs/Items/`
   - **Initial Size**: 20
   - **Max Size**: 50
   - **Expand If Needed**: ✓ (marcado)
   - **Show Debug**: ☐ (desmarcado)

**Nota**: Você pode criar múltiplos pools para diferentes tipos de cristais se necessário.

---

## Usando o Menu de Ferramentas (Alternativa Automática)

Se preferir aplicar automaticamente, use o menu do Unity:

1. **Menu**: `Extra Tools > Performance > Apply All Optimizations to Scene`

Este comando irá:
- Substituir automaticamente sistemas LOD antigos
- Adicionar StaggeredUpdateManager se não existir
- Criar pool de cristais automaticamente

---

## Verificação Pós-Aplicação

Após aplicar as otimizações, verifique se tudo está funcionando:

### 1. Verificar LOD System
- Entre em Play Mode
- Mova o player pela cena
- Observe árvores e props ficando menos detalhados à distância
- Pressione `F` para ver Gizmos (se Show Gizmos estiver ativado)

### 2. Verificar Staggered Update
- Entre em Play Mode
- Abra o Profiler (Window > Analysis > Profiler)
- Observe que updates estão distribuídos ao longo dos frames

### 3. Verificar Object Pool
- Entre em Play Mode
- Colete alguns cristais
- Observe no Inspector do CrystalPool:
  - Active Objects aumenta quando cristais aparecem
  - Available Objects aumenta quando cristais são retornados

---

## Performance Report

Para ver um relatório completo das otimizações aplicadas:

**Menu**: `Extra Tools > Performance > Show Performance Report`

Este comando mostra:
- Quantos sistemas LOD estão ativos
- Quantos pools existem
- Status do Staggered Update Manager
- Recomendações de otimizações pendentes

---

## Ganhos Esperados de Performance

| Otimização | Ganho FPS | Status |
|------------|-----------|--------|
| PlayerCache | +5-8 | ✅ Implementado |
| Trigger-based Attraction | +2-4 | ✅ Implementado |
| Stealth Interval | +1-2 | ✅ Implementado |
| CameraManager Update | +0.5-1 | ✅ Implementado |
| Unified LOD | +5-8 | ⏳ Pendente aplicação |
| Object Pooling | +2-4 | ⏳ Pendente aplicação |
| Staggered Updates | +3-5 | ⏳ Pendente aplicação |
| **TOTAL** | **+20-35** | - |

---

## Próximos Passos (Opcional)

### Migrar AI/NPCs para Staggered Update

Para otimizar ainda mais, você pode migrar sistemas de AI para usar updates escalonados:

1. Abra o script do AI/NPC
2. Implemente a interface `IStaggeredUpdate`:

```csharp
using SlimeKing.Systems.Core;

public class MyAI : MonoBehaviour, IStaggeredUpdate
{
    private void Start()
    {
        // Registra no manager
        StaggeredUpdateManager.Instance.Register(this);
    }
    
    // Substitui Update() por StaggeredUpdate()
    public void StaggeredUpdate()
    {
        // Lógica pesada de AI aqui
        // Será chamado a cada N frames ao invés de todo frame
    }
    
    private void OnDestroy()
    {
        // Remove do manager
        if (StaggeredUpdateManager.Instance != null)
        {
            StaggeredUpdateManager.Instance.Unregister(this);
        }
    }
}
```

**Ganho adicional**: +2-3 FPS por sistema migrado

---

## Troubleshooting

### "Componente não encontrado"
- Certifique-se de que o Unity compilou os scripts
- Verifique se não há erros de compilação no Console
- Tente fechar e reabrir o Unity

### "LOD não está funcionando"
- Verifique se as tags `WindShaker` e `Prop` estão configuradas nos objetos
- Certifique-se de que o Player tem a tag `Player`
- Verifique se `Enable LOD` está marcado

### "Pool não está reutilizando objetos"
- Certifique-se de que o código de spawn está usando `pool.Get()`
- Certifique-se de que o código de destruição está usando `pool.Return()`
- Verifique se o prefab está configurado corretamente

---

## Referências

- **Worklog**: `Assets/Docs/Worklogs/2026-01-26-performance-optimization-phase-all.md`
- **Coding Standards**: `Assets/Docs/CodingStandards.md` (seção Performance)
- **Scripts Criados**:
  - `Assets/_Code/Systems/Core/PlayerCache.cs`
  - `Assets/_Code/Performance/UnifiedLODSystem.cs`
  - `Assets/_Code/Systems/Core/ObjectPool.cs`
  - `Assets/_Code/Systems/Core/StaggeredUpdateManager.cs`
- **Editor Tools**:
  - `Assets/Editor/PerformanceOptimizationHelper.cs`
  - `Assets/Editor/ApplyPerformanceOptimizations.cs`

---

## Suporte

Se encontrar problemas ao aplicar as otimizações, verifique:
1. Console do Unity para erros
2. Profiler para métricas de performance
3. Logs do sistema (Window > Analysis > Console)

**Importante**: Sempre teste em Play Mode após aplicar otimizações para garantir que tudo funciona corretamente!
