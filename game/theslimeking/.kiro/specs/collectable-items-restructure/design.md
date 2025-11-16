# Design Document - Reestruturação do Sistema de Itens Coletáveis

## Overview

Este documento descreve o design técnico para reestruturar o sistema de itens coletáveis do The Slime King. O sistema será modificado para unificar o comportamento de coleta, garantindo que itens comuns e equipáveis sejam adicionados ao inventário, enquanto cristais elementais atualizam contadores específicos no HUD sem ocupar espaço no inventário.

## Architecture

### High-Level Flow

```
┌─────────────────┐
│  ItemCollectable │
│   (MonoBehaviour)│
└────────┬─────────┘
         │
         ├─── crystalData? ──YES──> GameManager.AddCrystal()
         │                                    │
         │                                    v
         │                          ┌──────────────────┐
         │                          │ OnCrystalCountChanged │
         │                          └─────────┬────────┘
         │                                    │
         │                                    v
         │                          ┌──────────────────┐
         │                          │  CrystalHUDManager │
         │                          │  (Atualiza UI)    │
         │                          └──────────────────┘
         │
         └─── inventoryItemData? ──YES──> InventoryManager.AddItem()
                                                   │
                                                   v
                                          ┌──────────────────┐
                                          │  Inventário      │
                                          │  (Slots 0-39)    │
                                          └──────────────────┘
```

### Component Relationships

```
ItemCollectable
    ├── Usa: CrystalElementalData (ScriptableObject)
    ├── Usa: ItemData (ScriptableObject)
    ├── Usa: CollectableItemData (ScriptableObject - legado)
    ├── Comunica: GameManager (para cristais)
    └── Comunica: InventoryManager (para itens)

GameManager
    ├── Mantém: Dictionary<CrystalType, int> crystalCounts
    ├── Dispara: OnCrystalCountChanged event
    └── Dispara: OnCrystalCollected event

CrystalHUDManager (NOVO)
    ├── Escuta: GameManager.OnCrystalCountChanged
    ├── Atualiza: Count_Text de cada cristal
    └── Formata: "x{quantidade}"

InventoryManager
    ├── Mantém: List<InventorySlot> slots
    ├── Gerencia: Empilhamento de itens
    └── Retorna: bool (sucesso/falha ao adicionar)
```

## Components and Interfaces

### 1. ItemCollectable (Modificado)

**Responsabilidade:** Gerenciar coleta de itens e roteamento para sistemas apropriados

**Modificações Necessárias:**

```csharp
public class ItemCollectable : MonoBehaviour
{
    // Configurações existentes mantidas
    [Header("Item Configuration")]
    [SerializeField] private CollectableItemData itemData;

    [Header("Crystal Configuration")]
    [SerializeField] private CrystalElementalData crystalData;

    [Header("Inventory Integration")]
    [SerializeField] private ItemData inventoryItemData;
    [SerializeField] private int itemQuantity = 1;

    // Método principal modificado
    public void CollectItem(GameObject collector = null)
    {
        // Proteção contra múltiplas coletas
        if (_isCollected) return;
        _isCollected = true;

        // PRIORIDADE 1: Cristais Elementais (NÃO vão para inventário)
        if (crystalData != null)
        {
            ProcessCrystalCollection();
            return;
        }

        // PRIORIDADE 2: Itens de Inventário
        if (inventoryItemData != null)
        {
            ProcessInventoryItemCollection();
            return;
        }

        // PRIORIDADE 3: Sistema Legado
        if (itemData != null)
        {
            ProcessLegacyItemCollection(collector);
            return;
        }

        // Nenhum dado configurado
        LogWarning("Nenhum ItemData configurado!");
        RevertCollectionState();
    }

    private void ProcessCrystalCollection()
    {
        // Valida GameManager
        if (GameManager.Instance == null)
        {
            LogError("GameManager.Instance é null!");
            RevertCollectionState();
            return;
        }

        // Adiciona cristal ao contador (NÃO ao inventário)
        GameManager.Instance.AddCrystal(crystalData.crystalType, crystalData.value);

        // Efeitos visuais e sonoros
        PlayCrystalCollectionEffects();

        // Remove da cena
        DestroyItem();
    }

    private void ProcessInventoryItemCollection()
    {
        // Valida InventoryManager
        if (InventoryManager.Instance == null)
        {
            LogError("InventoryManager.Instance é null!");
            RevertCollectionState();
            return;
        }

        // Tenta adicionar ao inventário
        bool success = InventoryManager.Instance.AddItem(inventoryItemData, itemQuantity);

        if (!success)
        {
            // Inventário cheio - mantém item na cena
            Log($"Inventário cheio! Item '{inventoryItemData.itemName}' não coletado.");
            RevertCollectionState();
            // TODO: Mostrar notificação "Inventário Cheio!"
            return;
        }

        // Sucesso - efeitos e remoção
        Log($"Item '{inventoryItemData.itemName}' adicionado ao inventário (x{itemQuantity})");
        PlayCollectionEffects();
        DestroyItem();
    }

    private void RevertCollectionState()
    {
        _isCollected = false;
        if (_collider != null)
        {
            _collider.enabled = true;
        }
    }
}
```

### 2. CrystalHUDController (NOVO)

**Responsabilidade:** Controlar atualização visual dos contadores de cristais no HUD

**Localização:** `Assets/Code/Gameplay/UI/CrystalHUDController.cs`

**Justificativa de Nomenclatura:**

- Usa sufixo **Controller** (não Manager) pois controla comportamento específico de UI
- Managers são para sistemas globais únicos; este é um controller de UI específico
- Segue padrão: `UIController` para controle de painéis específicos de UI

```csharp
using UnityEngine;
using TMPro;
using SlimeKing.Data;
using System.Collections.Generic;

namespace SlimeKing.UI
{
    /// <summary>
    /// Controla a atualização visual dos contadores de cristais elementais no HUD.
    /// Escuta eventos do GameManager e atualiza os textos correspondentes.
    /// Segue padrão Controller para controle de UI específica.
    /// </summary>
    public class CrystalHUDController : MonoBehaviour
    {
        [Header("Crystal UI References")]
        [SerializeField] private TextMeshProUGUI natureCountText;
        [SerializeField] private TextMeshProUGUI fireCountText;
        [SerializeField] private TextMeshProUGUI waterCountText;
        [SerializeField] private TextMeshProUGUI shadowCountText;
        [SerializeField] private TextMeshProUGUI earthCountText;
        [SerializeField] private TextMeshProUGUI airCountText;

        [Header("Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private string countFormat = "x{0}"; // Formato: "x10"

        #region Private Variables
        // Mapeamento de tipos para textos
        private Dictionary<CrystalType, TextMeshProUGUI> crystalTextMap;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeCrystalTextMap();
        }

        private void OnEnable()
        {
            // Inscreve no evento do GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCrystalCountChanged += HandleCrystalCountChanged;
                
                // Inicializa contadores com valores atuais
                InitializeCounters();
            }
            else
            {
                Debug.LogWarning("[CrystalHUDManager] GameManager.Instance é null no OnEnable");
            }
        }

        private void OnDisable()
        {
            // Desinscreve do evento
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCrystalCountChanged -= HandleCrystalCountChanged;
            }
        }
        #endregion

        #region Initialization
        private void InitializeCrystalTextMap()
        {
            crystalTextMap = new Dictionary<CrystalType, TextMeshProUGUI>
            {
                { CrystalType.Nature, natureCountText },
                { CrystalType.Fire, fireCountText },
                { CrystalType.Water, waterCountText },
                { CrystalType.Shadow, shadowCountText },
                { CrystalType.Earth, earthCountText },
                { CrystalType.Air, airCountText }
            };

            // Valida referências
            foreach (var kvp in crystalTextMap)
            {
                if (kvp.Value == null)
                {
                    Debug.LogError($"[CrystalHUDManager] Referência de texto para {kvp.Key} não configurada!");
                }
            }
        }

        private void InitializeCounters()
        {
            // Obtém contadores atuais do GameManager
            var allCounts = GameManager.Instance.GetAllCrystalCounts();

            foreach (var kvp in allCounts)
            {
                UpdateCrystalText(kvp.Key, kvp.Value);
            }

            Log("Contadores de cristais inicializados");
        }
        #endregion

        #region Event Handlers
        private void HandleCrystalCountChanged(CrystalType crystalType, int newCount)
        {
            UpdateCrystalText(crystalType, newCount);
            Log($"Cristal {crystalType} atualizado: {newCount}");
        }
        #endregion

        #region UI Update
        private void UpdateCrystalText(CrystalType crystalType, int count)
        {
            if (!crystalTextMap.TryGetValue(crystalType, out TextMeshProUGUI textComponent))
            {
                Debug.LogWarning($"[CrystalHUDManager] Tipo de cristal {crystalType} não mapeado");
                return;
            }

            if (textComponent == null)
            {
                Debug.LogWarning($"[CrystalHUDManager] TextComponent para {crystalType} é null");
                return;
            }

            // Formata texto: "x10"
            textComponent.text = string.Format(countFormat, count);
        }
        #endregion

        #region Logging
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[CrystalHUDController] {message}");
            }
        }
        #endregion

        #region Editor Helpers
#if UNITY_EDITOR
        [ContextMenu("Test Update All Counters")]
        private void EditorTestUpdateCounters()
        {
            if (!Application.isPlaying) return;

            foreach (CrystalType type in System.Enum.GetValues(typeof(CrystalType)))
            {
                int randomCount = Random.Range(0, 100);
                UpdateCrystalText(type, randomCount);
            }
        }

        [ContextMenu("Auto-Find Text References")]
        private void EditorAutoFindReferences()
        {
            // Busca automaticamente os textos no CrystalContainer
            Transform crystalContainer = transform.Find("CrystalContainer");
            if (crystalContainer == null)
            {
                Debug.LogError("CrystalContainer não encontrado como filho deste GameObject");
                return;
            }

            natureCountText = FindCountText(crystalContainer, "Crystal_Nature");
            fireCountText = FindCountText(crystalContainer, "Crystal_Fire");
            waterCountText = FindCountText(crystalContainer, "Crystal_Water");
            shadowCountText = FindCountText(crystalContainer, "Crystal_Shadow");
            earthCountText = FindCountText(crystalContainer, "Crystal_Earth");
            airCountText = FindCountText(crystalContainer, "Crystal_Air");

            Debug.Log("[CrystalHUDController] Referências encontradas automaticamente");
        }

        private TextMeshProUGUI FindCountText(Transform parent, string crystalName)
        {
            Transform crystalTransform = parent.Find(crystalName);
            if (crystalTransform == null)
            {
                Debug.LogWarning($"{crystalName} não encontrado");
                return null;
            }

            Transform countTextTransform = crystalTransform.Find("Count_Text");
            if (countTextTransform == null)
            {
                Debug.LogWarning($"Count_Text não encontrado em {crystalName}");
                return null;
            }

            return countTextTransform.GetComponent<TextMeshProUGUI>();
        }
#endif
        #endregion
    }
}
```

### 3. GameManager (Já Implementado)

**Status:** ✅ Já possui sistema de cristais completo

**Métodos Relevantes:**

- `AddCrystal(CrystalType, int)` - Adiciona cristais ao contador
- `GetCrystalCount(CrystalType)` - Obtém quantidade de um tipo
- `GetAllCrystalCounts()` - Obtém todos os contadores
- `RemoveCrystal(CrystalType, int)` - Remove cristais (para crafting futuro)

**Eventos:**

- `OnCrystalCountChanged` - Disparado quando contador muda
- `OnCrystalCollected` - Disparado quando cristal é coletado

### 4. InventoryManager (Assumido Existente)

**Métodos Necessários:**

- `AddItem(ItemData, int quantity)` → `bool` - Adiciona item ao inventário
- `RemoveItem(ItemData, int quantity)` → `bool` - Remove item
- `HasSpace()` → `bool` - Verifica se há espaço disponível

**Comportamento Esperado:**

- Empilha itens do mesmo tipo automaticamente
- Retorna `false` se inventário estiver cheio
- Respeita limite de 99 unidades por stack

## Data Models

### CrystalElementalData (Existente)

```csharp
[CreateAssetMenu(fileName = "CrystalData", menuName = "SlimeKing/Crystal Elemental Data")]
public class CrystalElementalData : ScriptableObject
{
    [Header("Crystal Identity")]
    public string crystalName;
    public CrystalType crystalType;
    public int value = 1; // Quantidade ao coletar

    [Header("Visual")]
    public Sprite crystalSprite;
    public Color crystalTint = Color.white;

    [Header("Collection")]
    public float attractionRadius = 3f;
    public float attractionSpeed = 5f;
    public float activationDelay = 0.5f;

    [Header("Effects")]
    public GameObject collectVFX;
    public AudioClip collectSound;
}
```

### ItemData (Assumido Existente)

```csharp
[CreateAssetMenu(fileName = "ItemData", menuName = "SlimeKing/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType; // Common, Equipable, Consumable
    public int maxStackSize = 99;
    
    // Outros campos específicos do item
}
```

### CrystalType Enum (Existente)

```csharp
public enum CrystalType
{
    Nature,
    Fire,
    Water,
    Shadow,
    Earth,
    Air
}
```

## Error Handling

### Cenários de Erro e Tratamento

1. **GameManager.Instance é null ao coletar cristal**
   - Ação: Registrar erro, reverter estado de coleta
   - Log: `"[ItemCollectable] GameManager.Instance é null! Cristal não pode ser processado."`
   - Resultado: Item permanece na cena, pode ser coletado novamente

2. **InventoryManager.Instance é null ao coletar item**
   - Ação: Registrar erro, reverter estado de coleta
   - Log: `"[ItemCollectable] InventoryManager.Instance é null!"`
   - Resultado: Item permanece na cena

3. **Inventário cheio**
   - Ação: Reverter estado de coleta, manter item visível
   - Log: `"[ItemCollectable] Inventário cheio! Item não coletado."`
   - Resultado: Jogador pode tentar novamente após liberar espaço
   - TODO: Mostrar notificação visual "Inventário Cheio!"

4. **Nenhum dado de item configurado**
   - Ação: Registrar warning, não processar coleta
   - Log: `"[ItemCollectable] Nenhum ItemData configurado!"`
   - Resultado: Item não é coletado

5. **Referência de texto no HUD é null**
   - Ação: Registrar warning, não atualizar UI
   - Log: `"[CrystalHUDController] TextComponent para {type} é null"`
   - Resultado: Contador não é atualizado visualmente (mas valor é mantido no GameManager)

## Testing Strategy

### Unit Tests

**ItemCollectable:**

- ✅ Cristal com crystalData válido chama GameManager.AddCrystal
- ✅ Cristal NÃO é adicionado ao inventário
- ✅ Item com inventoryItemData válido chama InventoryManager.AddItem
- ✅ Item é adicionado ao inventário com quantidade correta
- ✅ Inventário cheio reverte estado de coleta
- ✅ Priorização correta: crystalData > inventoryItemData > itemData

**CrystalHUDController:**

- ✅ Inicialização carrega contadores atuais do GameManager
- ✅ Evento OnCrystalCountChanged atualiza texto correto
- ✅ Formato "x{count}" é aplicado corretamente
- ✅ Referências null são tratadas graciosamente
- ✅ Usa regiões para organizar código (Unity Lifecycle, Initialization, Event Handlers, UI Update, Logging, Editor Helpers)

**GameManager:**

- ✅ AddCrystal incrementa contador correto
- ✅ Evento OnCrystalCountChanged é disparado
- ✅ GetCrystalCount retorna valor correto
- ✅ Múltiplas adições acumulam corretamente

### Integration Tests

1. **Fluxo Completo de Cristal:**
   - Spawnar cristal na cena
   - Slime coleta cristal
   - Verificar GameManager.GetCrystalCount aumentou
   - Verificar HUD exibe "x{count}" correto
   - Verificar cristal foi removido da cena
   - Verificar inventário NÃO contém cristal

2. **Fluxo Completo de Item:**
   - Spawnar item na cena
   - Slime coleta item
   - Verificar InventoryManager contém item
   - Verificar quantidade correta
   - Verificar item foi removido da cena

3. **Inventário Cheio:**
   - Encher inventário (20 slots)
   - Spawnar item na cena
   - Tentar coletar
   - Verificar item permanece na cena
   - Verificar estado de coleta foi revertido
   - Liberar espaço no inventário
   - Coletar novamente
   - Verificar sucesso

### Manual Tests

1. **Teste Visual de HUD:**
   - Coletar cristais de cada tipo
   - Verificar contadores atualizam corretamente
   - Verificar formato "x10" está correto
   - Verificar cores e ícones estão corretos

2. **Teste de Atração Magnética:**
   - Verificar cristais são atraídos normalmente
   - Verificar itens são atraídos normalmente
   - Verificar delay de ativação funciona

3. **Teste de Efeitos:**
   - Verificar VFX de coleta de cristais
   - Verificar SFX de coleta de cristais
   - Verificar VFX de coleta de itens
   - Verificar SFX de coleta de itens

## Performance Considerations

### Otimizações

1. **Event Subscription:**
   - CrystalHUDController se inscreve em OnEnable e desinscreve em OnDisable
   - Evita memory leaks
   - Segue padrão recomendado de subscribe/unsubscribe

2. **Dictionary Lookup:**
   - Mapeamento de CrystalType → TextMeshProUGUI usa Dictionary
   - O(1) lookup time

3. **Caching:**
   - ItemCollectable cacheia componentes em Awake
   - Player transform é cacheado estaticamente

4. **Lazy Initialization:**
   - HUD só atualiza quando evento é disparado
   - Não há polling ou Update loops

### Memory Management

- Eventos são limpos em OnDisable/OnDestroy
- Coroutines são paradas antes de destruir objetos
- Referências são nullificadas quando apropriado

## Migration Path

### Fase 1: Preparação (Sem Breaking Changes)

1. Criar CrystalHUDController (seguindo padrão Controller para UI)
2. Adicionar ao CanvasHUD na cena
3. Configurar referências de texto usando helper "Auto-Find Text References"
4. Testar eventos do GameManager

### Fase 2: Modificação do ItemCollectable

1. Adicionar método ProcessCrystalCollection
2. Adicionar método ProcessInventoryItemCollection
3. Modificar CollectItem para usar nova lógica
4. Manter compatibilidade com sistema legado

### Fase 3: Testes e Validação

1. Testar coleta de cristais
2. Testar coleta de itens
3. Testar inventário cheio
4. Testar HUD atualiza corretamente

### Fase 4: Migração de Itens Existentes

1. Converter itens antigos para usar inventoryItemData
2. Remover dependências de CollectableItemData
3. Limpar código legado (opcional)

## Future Enhancements

### Possíveis Melhorias

1. **Notificação de Inventário Cheio:**
   - UI popup temporário
   - Som de erro
   - Shake do ícone de inventário

2. **Animação de Contador:**
   - Contador "pula" quando atualizado
   - Efeito de brilho temporário
   - Som de "ding" ao coletar

3. **Tooltip de Cristais:**
   - Hover sobre ícone mostra nome do cristal
   - Descrição do uso futuro

4. **Sistema de Conquistas:**
   - "Coletou 100 cristais de fogo"
   - "Coletou todos os tipos de cristais"

5. **Crafting System:**
   - Usar cristais para criar itens
   - Usar GameManager.RemoveCrystal

## Conclusion

Este design mantém a arquitetura existente enquanto adiciona funcionalidade clara e bem definida. A separação entre cristais (GameManager) e itens (InventoryManager) é explícita e fácil de entender. O sistema é extensível e permite futuras melhorias sem grandes refatorações.

**Pontos-chave:**

- ✅ Cristais NÃO vão para inventário
- ✅ HUD atualiza automaticamente via eventos
- ✅ Formato "x10" implementado
- ✅ Compatibilidade com sistema legado mantida
- ✅ Tratamento robusto de erros
- ✅ Performance otimizada

## Best Practices Followed

Este design segue rigorosamente as boas práticas documentadas em `Assets/Docs/Project/BoasPraticas.md`:

### Nomenclatura e Arquitetura

✅ **CrystalHUDController** (não Manager)

- Usa sufixo **Controller** pois controla UI específica
- Managers são para sistemas globais únicos
- Segue padrão: Controllers para comportamento de entidades/UI

✅ **Comunicação via Eventos**

- GameManager dispara `OnCrystalCountChanged`
- CrystalHUDController escuta eventos
- Baixo acoplamento entre sistemas
- Subscribe em OnEnable, Unsubscribe em OnDisable

### Organização de Código

✅ **Uso de Regiões**

- `#region Unity Lifecycle` - Awake, OnEnable, OnDisable
- `#region Initialization` - Setup inicial
- `#region Event Handlers` - Métodos que respondem a eventos
- `#region UI Update` - Atualização de interface
- `#region Logging` - Sistema de logs
- `#region Editor Helpers` - Ferramentas de editor

✅ **Sistema de Logs Configurável**

- Flag `enableDebugLogs` para controlar verbosidade
- Logs detalhados para debugging
- Prefixo consistente `[ClassName]`

### Performance

✅ **Event-Driven Architecture**

- Sem polling ou Update loops
- Atualização apenas quando necessário
- Dictionary lookup O(1)

✅ **Memory Management**

- Subscribe/Unsubscribe balanceados
- Evita memory leaks
- Caching apropriado

### Compatibilidade

✅ **Unity 6.2+ e C# 10**

- Usa features modernas
- Evita práticas obsoletas
- TextMeshPro para UI

✅ **Sem Over-Engineering**

- Implementa apenas o necessário
- Código claro e direto
- Fácil de manter e estender

### Priorização de Cristais

✅ **Configuração Correta de Cristais**

- Apenas `crystalData` preenchido (não vai para inventário)
- Sistema de prioridades: crystalData > inventoryItemData > itemData
- Logs claros para debugging de configuração
- Documentação inline sobre comportamento esperado
