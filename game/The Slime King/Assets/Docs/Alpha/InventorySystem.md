# Sistema de Inventário - Implementação Alpha

## 📋 Status

- **InventoryCore.cs:** ✅ Esqueleto criado
- **ItemUsageManager.cs:** ✅ Esqueleto criado  
- **Integração com código existente:** 🔜 Pendente
- **HUD Integration:** 🔜 Pendente

## 🎯 Objetivo

Integrar sistema de coleta existente (ItemCollectable) com novo sistema de inventário Alpha, sem modificar código existente.

## 🔧 Implementação

### Scripts Necessários (todos novos na Alpha/)

#### 1. AlphaItemAdapter.cs (NOVO)

```csharp
// Intercepta coleta do sistema existente e envia para InventoryCore
// Anexar em GameObjects que têm ItemCollectable
```

#### 2. InventoryHUD.cs (NOVO)  

```csharp
// UI para mostrar 4 slots de consumíveis
// Integra com InventoryCore via eventos
```

#### 3. AlphaInventorySetup.cs (NOVO)

```csharp
// Setup automático do sistema na cena
// Encontra todos ItemCollectable e adiciona AlphaItemAdapter
```

### Fluxo de Integração

1. **Coleta (SEM MODIFICAR código existente)**

   ```
   Player toca ItemCollectable → 
   ItemCollectable.OnCollected (existente) → 
   AlphaItemAdapter.OnCollected (novo) → 
   InventoryCore.AddItem() (novo)
   ```

2. **Exibição**

   ```
   InventoryCore.OnItemAdded → 
   InventoryHUD.UpdateSlot() → 
   UI atualizada
   ```

3. **Uso**

   ```
   Input UseItem1-4 (existente) → 
   ItemUsageManager.UseItemFromSlot() (novo) → 
   Effect aplicado + item removido
   ```

## 📝 TODOs Específicos

### InventoryCore.cs (completar TODOs existentes)

- [ ] Implementar singleton pattern
- [ ] Conectar com AlphaItemAdapter
- [ ] Sistema de slots (4 consumíveis)
- [ ] Eventos para HUD

### ItemUsageManager.cs (completar TODOs existentes)  

- [ ] Bind com Input Actions UseItem1-4
- [ ] Implementar efeitos básicos (+HP, +Speed temporário)
- [ ] Integração com PlayerAttributesSystem

### AlphaItemAdapter.cs (criar novo)

- [ ] Component que se anexa a ItemCollectable
- [ ] Escuta OnCollected event
- [ ] Converte para InventoryItem e envia para InventoryCore

### InventoryHUD.cs (criar novo)

- [ ] UI Canvas com 4 slots
- [ ] Escuta eventos do InventoryCore  
- [ ] Feedback visual de coleta/uso

## 🔗 Pontos de Integração

### Com ItemCollectable (NÃO MODIFICAR)

- Usar event OnCollected se existir
- Se não existir, usar MonoBehaviour.OnTriggerEnter intercept

### Com Input System (USAR EXISTENTE)

- UseItem1, UseItem2, UseItem3, UseItem4 actions
- Não criar novos Input Actions

### Com PlayerAttributesSystem (NÃO MODIFICAR)  

- Usar eventos para aplicar efeitos de itens
- PlayerAttributesSystem.ModifyHealth(), etc.

## ⚙️ Configuração na Cena

### Setup Automático via Extra Tools > Alpha

1. Encontra todos GameObjects com ItemCollectable
2. Adiciona AlphaItemAdapter component
3. Cria InventoryCore singleton na cena
4. Configura InventoryHUD

### Setup Manual (fallback)

1. Adicionar AlphaInventorySetup.cs em GameObject vazio
2. Configurar InventoryHUD prefab na cena
3. InventoryCore será criado automaticamente

## 🧪 Teste de Validação

1. **Coleta:** Tocar item → aparece no slot HUD
2. **Uso:** Pressionar 1-4 → efeito aplicado + item removido  
3. **Integração:** Sem modificar ItemCollectable existente
4. **Performance:** Sem impacto no sistema existente

## 📊 Métricas de Sucesso

- [ ] 4 slots visíveis no HUD
- [ ] Coleta automática funciona via adapter
- [ ] Uso via teclado (1-4) e gamepad (UseItem actions)
- [ ] Efeitos aplicados (+HP visível no PlayerAttributesSystem)
- [ ] Zero modificações no código existente
