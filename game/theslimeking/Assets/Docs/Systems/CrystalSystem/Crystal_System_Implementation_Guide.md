# 💎 Sistema de Cristais - Guia de Implementação Completa

## 🎯 Visão Geral

Sistema completo de coleta de cristais com contadores automáticos na UI, seguindo os princípios KISS e YAGNI do projeto SlimeKing.

## 📁 Arquivos Implementados

### 1. **CrystalType.cs** - Enum de Tipos de Cristais

```csharp
// Localização: Assets/Code/Systems/Types/CrystalType.cs
public enum CrystalType
{
    Nature = 0,  // Verde
    Fire = 1,    // Vermelho
    Water = 2,   // Azul
    Shadow = 3,  // Roxo
    Earth = 4,   // Marrom
    Air = 5      // Cinza/Branco
}
```

### 2. **CrystalElementalData.cs** - ScriptableObject de Configuração

```csharp
// Localização: Assets/Code/Systems/Items/CrystalElementalData.cs
// Herda de ItemCollectable para reutilizar sistema existente
// Configurações: cores, VFX, velocidade de atração, logs
```

### 3. **GameManager.cs** - Sistema Central Atualizado

- ✅ Adicionado `Dictionary<CrystalType, int> crystalCounters`
- ✅ Eventos `OnCrystalCountChanged` e `OnCrystalCollected`
- ✅ Métodos `AddCrystal()`, `GetCrystalCount()`, `SetCrystalCount()`
- ✅ Logs controlados por `enableCrystalLogs`

### 4. **ItemCollectable.cs** - Extensão para Cristais

- ✅ Detecção automática de `CrystalElementalData`
- ✅ Prioridade: Cristais > Inventário > Sistema legado
- ✅ Integração com `GameManager.AddCrystal()`
- ✅ Logs detalhados do processo de coleta

### 5. **CrystalCounterUI.cs** - UI Automática

```csharp
// Localização: Assets/Code/Systems/UI/CrystalCounterUI.cs
// Namespace: SlimeKing.UI
// Conecta eventos do GameManager aos textos da UI
```

### 6. **SlimeKing.Debug.cs** - Sistema de Logs

```csharp
// Localização: Assets/Code/Systems/Debug/Log.cs
// Namespace: SlimeKing.Debug
// Uso: SlimeKing.Debug.Debug.Log(), LogWarning(), LogError()
```

### 7. **CrystalSystemTester.cs** - Ferramenta de Teste

- ✅ Testes via Context Menu no Inspector
- ✅ Adicionar cristal específico
- ✅ Adicionar todos os tipos
- ✅ Ver contadores atuais
- ✅ Verificar conexões da UI

## 🚀 Como Usar o Sistema

### 1. **Configurar CrystalCounterUI na Cena**

1. **Abrir cena `2_InitialCave`** (já tem CanvasHUD)

2. **Adicionar Componente CrystalCounterUI:**

   ```
   CanvasHUD (GameObject existente)
   └── Adicionar Component: CrystalCounterUI
   ```

3. **Configurar GameObjects dos Contadores:**
   - Criar 6 GameObjects filhos com nomes:
     - `Crystal_Nature`
     - `Crystal_Fire`
     - `Crystal_Water`
     - `Crystal_Shadow`
     - `Crystal_Earth`
     - `Crystal_Air`

4. **Adicionar TextMeshProUGUI:**
   - Cada GameObject precisa de componente `TextMeshProUGUI`
   - Texto inicial: "0"

### 2. **Criar ScriptableObjects de Cristais**

```csharp
// No menu: Assets > Create > SlimeKing > Items > Crystal Elemental Data
// Configurar para cada tipo:
// - Crystal Type: Nature/Fire/Water/Shadow/Earth/Air
// - Colors, VFX, Attraction Speed, etc.
```

### 3. **Testar o Sistema**

#### Método 1: CrystalSystemTester (Recomendado)

1. **Adicionar CrystalSystemTester** a qualquer GameObject na cena
2. **No Inspector, usar Context Menu:**
   - `Test Add Crystal` - Adiciona cristal específico
   - `Test Add All Crystals` - Adiciona todos os tipos
   - `Show Current Counters` - Mostra valores atuais
   - `Test UI Connection` - Verifica se UI está conectada

#### Método 2: ItemCollectable

1. **Criar GameObject** com `ItemCollectable`
2. **Configurar Item Data** com `CrystalElementalData`
3. **Player coleta automaticamente** quando próximo

## 🔧 Configurações e Flags

### GameManager

```csharp
[Header("Crystal System")]
public bool enableCrystalLogs = true;  // Logs de coleta
```

### CrystalCounterUI

```csharp
[Header("Debug Settings")]
public bool enableDebugLogs = true;    // Logs de UI
```

### SlimeKing.Debug

```csharp
// Controle global de logs
SlimeKing.Debug.Debug.SetDebugEnabled(true);
SlimeKing.Debug.Debug.SetWarningsEnabled(true);
SlimeKing.Debug.Debug.SetErrorsEnabled(true);
```

## 🎮 Fluxo de Funcionamento

1. **Player coleta cristal** (ItemCollectable detecta CrystalElementalData)
2. **ItemCollectable chama** `GameManager.AddCrystal(type, 1)`
3. **GameManager atualiza contador** e dispara evento `OnCrystalCountChanged`
4. **CrystalCounterUI escuta evento** e atualiza texto correspondente
5. **UI mostra novo valor** automaticamente

## 📋 Checklist de Verificação

- [ ] **Compilação sem erros** ✅ (Resolvido)
- [ ] **CrystalSystemTester** adicionado à cena
- [ ] **CanvasHUD** tem componente `CrystalCounterUI`
- [ ] **6 GameObjects Crystal_*** criados como filhos
- [ ] **TextMeshProUGUI** em cada contador
- [ ] **Teste via Context Menu** funciona
- [ ] **Contadores atualizam** automaticamente

## 🐛 Solução de Problemas

### Erro: "Non-invocable member 'Log' cannot be used like a method"

**Solução:** Usar `SlimeKing.Debug.Debug.Log()` em vez de `Debug.Log()`

### UI não atualiza

1. **Verificar** se `CrystalCounterUI` está no CanvasHUD
2. **Verificar** se GameObjects têm nomes corretos (`Crystal_Nature`, etc.)
3. **Verificar** se `enableDebugLogs = true` no CrystalCounterUI
4. **Usar** `Test UI Connection` no CrystalSystemTester

### Cristal não é coletado

1. **Verificar** se ItemCollectable tem `CrystalElementalData`
2. **Verificar** se `enableCrystalLogs = true` no GameManager
3. **Verificar** console para logs de coleta

## 🏗️ Extensões Futuras

- **Sistema de craft** usando cristais como recursos
- **Diferentes raridades** de cristais
- **Efeitos visuais** na coleta (já preparado via VFX no ScriptableObject)
- **Persistência** dos contadores entre sessões
- **Conquistas** baseadas em coleta de cristais

---

## 📞 Suporte

Se encontrar problemas, verificar:

1. **Console do Unity** para erros de compilação
2. **Logs do sistema** com flags de debug ativadas
3. **CrystalSystemTester** para diagnósticos automáticos

**Sistema implementado seguindo princípios KISS e YAGNI do SlimeKing.**
