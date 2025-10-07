# Plano de Implementação - Demo Alpha

**Data:** 07/Out/2025  
**Escopo:** Sistemas pendentes para Demo Alpha jogável  
**Estrutura:** Somente scripts novos na pasta Alpha/ (não modificar código existente)  

## 📋 Status Atual dos Sistemas

### ✅ Sistemas Concluídos (Não tocar)

- Movimentação & Animação Base (PlayerController)
- Vento (código legado)
- Árvores Reativas
- Grama & Arbustos
- Pedras Danificáveis (RockDestruct)
- Dropping Items
- Coleta Automática (Auto Pickup)
- Pontos Interativos
- Destaque (Outline)
- Combate Direcional (AttackHandler + PlayerController)
- PlayerAttributesSystem (base)

### 🔜 Sistemas Pendentes (Implementar na Alpha/)

1. **Coleta de Itens → Inventário** (Prioridade 1)
2. **Sistema de Inimigos** (Prioridade 1)
3. **Uso de Itens (Consumíveis)** (Prioridade 2)
4. **Interface/HUD Base** (Prioridade 2)
5. **Crescimento / Evolução do Slime** (Prioridade 3)
6. **Árvore de Habilidades** (Prioridade 3)
7. **Sistema de Diálogo Mínimo** (Prioridade 4)

### 💤 Sistemas Descartados da Alpha

- Special Movement (Shrink/Jump) - Movido para backlog
- **Câmera que Segue** - Cinemachine Follow já disponível
- **Ponto de Teletransporte** - Implementação futura específica

---

## 🎯 Estratégia de Implementação

### Princípios Fundamentais

1. **Zero modificações no código existente** - Toda funcionalidade nova vai para Alpha/
2. **Integração via eventos** - Usar eventos do PlayerAttributesSystem e outros sistemas existentes
3. **Wrappers/Adapters** - Criar adaptadores para integrar com código legado quando necessário
4. **MVP First** - Implementar versão mínima funcional primeiro
5. **Isolamento** - Cada sistema Alpha deve poder ser desabilitado independentemente

### Ordem de Implementação (Dependências)

```
Semana 1: Inventory Core + HUD Básico
Semana 2: Enemy System + Item Usage
Semana 3: Growth System + Skill Tree
Semana 4: Diálogo + Polish Final
Semana 5: Integração + Polish
Semana 6: Testes + Ajustes
```

---

## 📁 Estrutura de Arquivos Alpha

```
Assets/Alpha/
├── Scripts/
│   ├── Inventory/
│   │   ├── InventoryCore.cs ✅
│   │   ├── ItemUsageManager.cs ✅
│   │   ├── InventoryHUD.cs (novo)
│   │   └── AlphaItemAdapter.cs (novo)
│   ├── Enemy/
│   │   ├── EnemyController.cs ✅
│   │   ├── EnemySpawner.cs (novo)
│   │   └── AlphaEnemyIntegration.cs (novo)
│   ├── Progression/
│   │   ├── GrowthSystem.cs (novo)
│   │   ├── SkillTreeManager.cs (novo)
│   │   └── AlphaProgressionHUD.cs (novo)
│   ├── UI/
│   │   ├── AlphaHUDManager.cs (novo)
│   │   ├── DialogueController.cs (novo)
│   │   └── AlphaUINavigation.cs (novo)
├── Prefabs/
│   ├── AlphaHUD.prefab
│   ├── EnemyBasic.prefab
│   └── DialoguePanel.prefab
└── Docs/ (este diretório)
```

---

## 🔧 Padrões de Integração

### 1. Event-Driven Integration

```csharp
// Integrar com PlayerAttributesSystem via eventos
PlayerAttributesSystem.OnHealthChanged += UpdateHealthHUD;
PlayerAttributesSystem.OnLevelUp += GrowthSystem.OnPlayerLevelUp;
```

### 2. Adapter Pattern

```csharp
// Adaptar ItemCollectable existente para novo Inventory
public class AlphaItemAdapter : MonoBehaviour 
{
    void Start() {
        // Interceptar coleta e enviar para InventoryCore
        GetComponent<ItemCollectable>().OnCollected += 
            item => InventoryCore.Instance.AddItem(item);
    }
}
```

### 3. Singleton Pattern (Alpha Only)

```csharp
// Todos os managers Alpha usam singleton para facilitar acesso
public class InventoryCore : MonoBehaviour 
{
    public static InventoryCore Instance { get; private set; }
}
```

### 4. Component-Based Setup

```csharp
// Setup automático via componentes na cena
[RequireComponent(typeof(AlphaSetupComponent))]
public class AlphaManager : MonoBehaviour 
{
    // Auto-setup quando componente é adicionado
}
```

---

## ⚡ Pontos de Integração Críticos

### Com PlayerController

- **Não modificar PlayerController.cs**
- Usar eventos para detectar ações do player
- Criar AlphaPlayerIntegration.cs para bridging

### Com AttackHandler

- **Não modificar AttackHandler.cs**
- EnemyController deve detectar colisão com tag "Attack"
- Usar eventos OnHit para integração

### Com ItemCollectable

- **Não modificar ItemCollectable.cs**
- AlphaItemAdapter intercepta OnCollected
- Converte para formato do InventoryCore

### Com InputSystem

- **Usar Input Actions existentes**
- Não criar novos Input Actions
- Integrar com UseItem1-4 actions já definidas

---

## 📊 Métricas de Sucesso

### Técnicas

- Zero modificações em arquivos existentes fora de Alpha/
- Compilação limpa sem warnings
- FPS ≥ 60 com todos os sistemas ativos
- Menos de 10MB de overhead dos sistemas Alpha

### Funcionais

- Coleta → Inventário → Uso funcional
- Inimigo completa ciclo Patrol → Chase → Attack → Death
- Growth libera skill que afeta atributos
- HUD reflete estado atual em tempo real
**Validação final requer todos os critérios atendidos:**

- ~~Câmera segue sem jitter~~ ❌ Removido (Cinemachine)
- ~~Teleporte funciona sem clipping~~ ❌ Removido (futura implementação)

---

## 🚨 Riscos e Mitigações

### Risco: Conflito com código existente

**Mitigação:** Namespace SlimeMec.Alpha e prefixo Alpha em classes

### Risco: Performance degradation

**Mitigação:** Profiling obrigatório após cada sistema

### Risco: Complexidade de integração

**Mitigação:** Padrão adapter simples e eventos

### Risco: Input conflicts

**Mitigação:** Usar apenas Input Actions já existentes

---

## 📅 Cronograma Detalhado

Ver documentos específicos de cada sistema para detalhes de implementação.

**Próximo:** Documentação detalhada de cada sistema individual.
