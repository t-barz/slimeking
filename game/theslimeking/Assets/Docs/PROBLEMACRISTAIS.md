# Problema: Sistema de Cristais Elementais

**Data:** 15 de Novembro de 2025  
**Status:** ❌ CRÍTICO - Sistema não funcional  
**Impacto:** Alto - Afeta mecânicas core do jogo  
**Prioridade:** URGENTE - Necessário para Alpha 1

---

## 🚨 Problema Identificado

### Descrição

O sistema de cristais elementais **NÃO está funcionando corretamente**. Quando o slime absorve um `CollectableItem` do tipo `CristalElemental`, o contador de cristais **não está computando adequadamente**.

### Comportamento Atual (Incorreto)

- Cristais elementais são tratados como **itens comuns** do inventário
- **Ocupam slots** dos 20 espaços disponíveis no inventário
- **Não existe contador específico** por tipo de cristal (Nature, Fire, Water, etc.)
- **Sistema de habilidades futuro** ficará comprometido sem contagem adequada

### Comportamento Esperado (Correto)

- Cristais elementais devem ter **sistema dedicado de contagem**
- **NÃO devem ocupar** slots do inventário principal
- Cada **tipo elemental** deve ter contador separado
- **HUD deve mostrar** quantidades de cada cristal
- **Sistema de habilidades** deve consultar contadores para custos

---

## 📋 Análise Técnica

### Fluxo Atual (Problemático)

```
1. CristalElemental coletado
2. ItemCollectable.OnTriggerEnter2D()
3. InventoryManager.AddItem() 
4. Cristal armazenado como item comum (ERRO)
5. Ocupa slot do inventário (ERRO)
```

### Componentes Analisados

#### ✅ ItemCollectable.cs

- **Localização:** `Assets/💻 Code/Systems/ItemCollectable.cs`
- **Status:** Funcional mas genérico
- **Problema:** Não diferencia cristais de outros itens
- **Necessário:** Interceptar coleta de cristais antes do inventário

#### ✅ CollectableItemData.cs

- **Localização:** `Assets/💻 Code/Data/CollectableItemData.cs`
- **Status:** ScriptableObject bem estruturado
- **Funcional:** Define tipos de cristais corretamente
- **Compatível:** Pode ser usado pelo novo sistema

#### ✅ InventoryManager.cs

- **Localização:** `Assets/💻 Code/Systems/InventoryManager.cs`
- **Status:** Sistema de 20 slots funcionando
- **Problema:** Recebe cristais como itens normais
- **Solução:** Cristais devem ser interceptados antes

#### ❌ CrystalManager.cs

- **Localização:** NÃO EXISTE
- **Status:** **NÃO IMPLEMENTADO**
- **Impacto:** Sistema core ausente
- **Urgência:** Crítica para Alpha 1

---

## 🔧 Solução Proposta

### 1. Criar CrystalManager

```csharp
// Novo sistema seguindo padrão ManagerSingleton
namespace SlimeKing.Core
{
    public class CrystalManager : ManagerSingleton<CrystalManager>
    {
        // Contadores por tipo elemental
        private Dictionary<CrystalType, int> crystalCounts;
        
        // Eventos para UI
        public event Action<CrystalType, int> OnCrystalCollected;
        public event Action<CrystalType, int> OnCrystalSpent;
        
        // Métodos públicos
        public void AddCrystal(CrystalType type, int amount)
        public bool SpendCrystal(CrystalType type, int amount)
        public int GetCrystalCount(CrystalType type)
    }
}
```

### 2. Enum CrystalType

```csharp
public enum CrystalType
{
    Nature,   // Verde - Crescimento, cura
    Fire,     // Vermelho - Dano, explosão
    Water,    // Azul - Mobilidade, defesa
    Shadow,   // Roxo - Stealth, ilusão
    Earth,    // Marrom - Estruturas, proteção
    Air       // Branco - Velocidade, levitação
}
```

### 3. Modificar ItemCollectable

```csharp
// Adicionar verificação de cristais
if (collectableData.itemType == ItemType.CristalElemental)
{
    // Redirecionar para CrystalManager
    CrystalManager.Instance.AddCrystal(data.crystalType, 1);
    return; // NÃO enviar para InventoryManager
}

// Outros itens seguem para inventário normal
InventoryManager.Instance.AddItem(collectableData);
```

### 4. HUD de Cristais

```csharp
// Componente UI para mostrar contadores
public class CrystalCounterUI : MonoBehaviour
{
    [Header("Crystal Display")]
    public Image[] crystalIcons = new Image[6];
    public Text[] crystalCounts = new Text[6];
    
    // Escutar eventos do CrystalManager
    // Atualizar display em tempo real
}
```

---

## 📊 Impacto no Projeto

### Alpha 1 (CRÍTICO)

- ❌ **Sistema de habilidades** não pode ser implementado
- ❌ **Mecânica de cristais** não funciona
- ❌ **HUD de cristais** não existe
- ❌ **Quest de coleta** pode ter problemas

### Sistemas Afetados

1. **Sistema de Habilidades** - Depende de contadores de cristais
2. **Sistema de Quest** - Coleta de cristais mal contabilizada
3. **HUD/UI** - Falta feedback visual dos cristais
4. **Save/Load** - Cristais não salvos adequadamente
5. **Balanceamento** - Progressão de poder comprometida

---

## 🎯 Plano de Implementação

### Prioridade URGENTE (Esta semana)

1. **Criar CrystalManager.cs** (2h)
   - Implementar padrão ManagerSingleton
   - Sistema de contadores por tipo
   - Eventos para UI

2. **Modificar ItemCollectable.cs** (1h)
   - Detectar cristais elementais
   - Redirecionar para CrystalManager
   - Manter compatibilidade com outros itens

3. **Implementar CrystalType enum** (30min)
   - 6 tipos elementais
   - Documentação de cada tipo
   - Integração com CollectableItemData

4. **Criar HUD de Cristais** (3h)
   - CrystalCounterUI component
   - Layout visual no Canvas
   - Animações de coleta

5. **Testar Integração** (1h)
   - Coleta funcionando
   - Contadores atualizando
   - HUD responsivo
   - Save/Load preservando dados

### Total Estimado: **7.5 horas** (1 dia de trabalho)

---

## 🔍 Arquivos Relacionados

### Para Análise

- `Assets/💻 Code/Systems/ItemCollectable.cs`
- `Assets/💻 Code/Data/CollectableItemData.cs`
- `Assets/💻 Code/Systems/InventoryManager.cs`
- `Assets/External/AssetStore/SlimeMec/_Scripts/Gameplay/PlayerAttributesHandler.cs`

### Para Criação

- `Assets/💻 Code/Systems/CrystalManager.cs` ❌
- `Assets/💻 Code/Data/CrystalType.cs` ❌
- `Assets/💻 Code/Systems/UI/CrystalCounterUI.cs` ❌
- `Assets/💻 Code/Data/CrystalSaveData.cs` ❌

### Para Modificação

- `Assets/💻 Code/Systems/ItemCollectable.cs` ⚠️
- `Assets/💻 Code/Systems/SaveManager.cs` ⚠️

---

## 🎮 Teste Scenarios

### Cenário 1: Coleta Básica

1. Jogador toca cristal Nature
2. ✅ CrystalManager incrementa contador Nature
3. ✅ HUD mostra +1 Nature Crystal
4. ✅ Inventário NÃO ocupa slot

### Cenário 2: Sistema de Habilidades

1. Jogador usa habilidade Nature (custo: 10 cristais)
2. ✅ CrystalManager verifica quantidade suficiente
3. ✅ Subtrai cristais se disponível
4. ✅ HUD atualiza contador

### Cenário 3: Save/Load

1. Jogador coleta vários cristais
2. ✅ Save preserva contadores
3. ✅ Load restaura quantidades exatas
4. ✅ HUD mostra valores corretos

---

## 📝 Notas Técnicas

### Namespace

- `SlimeKing.Core` para CrystalManager (seguindo padrão)
- `SlimeKing.Core.UI` para CrystalCounterUI

### Eventos

- `OnCrystalCollected(CrystalType, int)` - Para UI e audio
- `OnCrystalSpent(CrystalType, int)` - Para feedback de gasto

### Performance

- Dictionary<CrystalType, int> para O(1) lookup
- Eventos Unity para UI responsiva
- Cache de sprites para performance

### Compatibilidade

- Manter ItemCollectable genérico
- CollectableItemData sem mudanças
- InventoryManager intocado para outros itens

---

**CONCLUSÃO:** Sistema crítico ausente que impede funcionalidade core do jogo. Implementação urgente necessária para Alpha 1.
