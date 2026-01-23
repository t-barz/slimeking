# Fix: Sistema de Save Game v1.0.3

## 🐛 Problema Identificado

### Problema 1: Load em Cena Diferente (v1.0.2)
Ao carregar um save que estava em uma cena diferente:
- ❌ Inventário ficava vazio
- ❌ Contadores de cristais zerados
- ❌ Dados não eram aplicados após reload da cena

**Causa Raiz:**
Quando `SceneManager.LoadScene()` é chamado, o SaveGameManager é destruído e recriado. As variáveis de instância (`currentSaveData`) eram perdidas antes de aplicar os dados.

### Problema 2: Load na Mesma Cena (v1.0.3)
Ao carregar um save na mesma cena sem sair do Play mode:
- ✅ Posição do player restaurada corretamente
- ❌ Inventário ficava vazio
- ❌ Contadores de cristais zerados

**Causa Raiz:**
O método `LoadGame()` tinha dois caminhos diferentes:
1. **Cena diferente**: Usava coroutine com delay de 2 frames
2. **Mesma cena**: Chamava `ApplyAllData()` imediatamente

A chamada imediata não dava tempo para os managers (InventoryManager, GameManager) estarem prontos, causando falha silenciosa na restauração.

## ✅ Solução Implementada

### 1. Variável Estática para Persistir Dados (v1.0.2)

```csharp
private static SaveGameData pendingLoadData; // Static para persistir entre reloads
```

**Por que funciona:**
- Variáveis `static` persistem entre destruição/criação de instâncias
- Dados ficam disponíveis após reload da cena

### 2. Fluxo de Load Corrigido (v1.0.2)

**Antes:**
```csharp
LoadGame() → LoadScene() → [SaveGameManager destruído] → [Dados perdidos]
```

**Depois:**
```csharp
LoadGame() → Salva em pendingLoadData (static) → LoadScene() 
→ [SaveGameManager destruído e recriado]
→ Initialize() detecta pendingLoadData
→ Aguarda 2 frames
→ Aplica dados
```

### 3. Coroutine para Mesma Cena (v1.0.3)

**Problema:** Mesmo na mesma cena, managers precisam de tempo para inicializar.

**Solução:** Usar coroutine com delay também para load na mesma cena:

```csharp
else
{
    // Mesma cena, aguarda managers estarem prontos
    Log("Mesma cena - aguardando managers...");
    StartCoroutine(ApplyDataAfterDelay());
}
```

**Nova coroutine adicionada:**
```csharp
private System.Collections.IEnumerator ApplyDataAfterDelay()
{
    // Aguarda 2 frames para garantir que managers estejam prontos
    yield return null;
    yield return null;
    
    Log("Aplicando dados do save (mesma cena)...");
    ApplyAllData();
    Log("Jogo carregado com sucesso!");
    OnGameLoaded?.Invoke(currentSaveData);
}
```

### 4. Código Implementado

**LoadGame() - Ambos os caminhos usam coroutine:**
```csharp
public bool LoadGame()
{
    // ... carrega JSON ...
    
    string savedScene = currentSaveData.currentSceneName;
    string currentScene = SceneManager.GetActiveScene().name;

    if (savedScene != currentScene)
    {
        // Salva dados em variável STATIC
        pendingLoadData = currentSaveData;
        
        // Recarrega cena
        SceneManager.LoadScene(savedScene, LoadSceneMode.Single);
        return true;
    }
    else
    {
        // Mesma cena, aguarda managers estarem prontos (v1.0.3)
        Log("Mesma cena - aguardando managers...");
        StartCoroutine(ApplyDataAfterDelay());
    }
}

protected override void Initialize()
{
    // Verifica se há dados pendentes
    if (pendingLoadData != null)
    {
        currentSaveData = pendingLoadData;
        pendingLoadData = null;
        
        // Aguarda inicialização completa
        StartCoroutine(ApplyDataAfterSceneLoad());
    }
}

private IEnumerator ApplyDataAfterSceneLoad()
{
    // Para load em cena diferente
    yield return null;
    yield return null;
    
    ApplyAllData();
    OnGameLoaded?.Invoke(currentSaveData);
}

private IEnumerator ApplyDataAfterDelay()
{
    // Para load na mesma cena (v1.0.3)
    yield return null;
    yield return null;
    
    Log("Aplicando dados do save (mesma cena)...");
    ApplyAllData();
    Log("Jogo carregado com sucesso!");
    OnGameLoaded?.Invoke(currentSaveData);
}
```

### 5. Logs Detalhados Adicionados

```csharp
[Header("Debug")]
[SerializeField] private bool enableDetailedLogs = true;
```

**Logs adicionados:**
- ✅ "Aplicando dados pendentes após reload de cena"
- ✅ "Mesma cena - aguardando managers..." (v1.0.3)
- ✅ "Aplicando dados do save (mesma cena)..." (v1.0.3)
- ✅ "Aplicando PlayerData: X itens, Y tipos de cristais"
- ✅ "Posição do player restaurada: (x, y, z)"
- ✅ "Item restaurado: ItemName xQuantity"
- ✅ "Cristal restaurado: CrystalType xQuantity"
- ✅ "Dados aplicados com sucesso!"
- ✅ "Jogo carregado com sucesso!" (v1.0.3)

**Warnings adicionados:**
- ⚠️ "PlayerController não encontrado"
- ⚠️ "InventoryManager não encontrado"
- ⚠️ "GameManager não encontrado"
- ⚠️ "Item não encontrado: ItemID"

## 🧪 Como Testar

### Teste 1: Load na Mesma Cena (SEM sair do Play Mode)

1. Play Mode em InitialForest
2. Coletar 5 cristais Fire
3. Tab (salvar)
4. **NÃO sair do Play Mode**
5. Coletar mais 3 cristais Fire (total: 8)
6. Chamar `SaveGameManager.Instance.LoadGame()` (ou pressionar Escape se auto-load ativo)
7. **Verificar Console:**
   - "Carregando jogo..."
   - "Mesma cena - aguardando managers..."
   - "Aplicando dados do save (mesma cena)..."
   - "Aplicando PlayerData: 0 itens, 1 tipos de cristais"
   - "Restaurando cristais..."
   - "Cristal restaurado: Fire x5"
   - "Jogo carregado com sucesso!"
8. **Verificar HUD:** Deve mostrar 5 cristais Fire (não 8!)

### Teste 2: Load na Mesma Cena (COM saída do Play Mode)

1. Play Mode em InitialForest
2. Coletar 5 cristais Fire
3. Tab (salvar)
4. **Sair do Play Mode**
5. **Entrar no Play Mode novamente** em InitialForest
6. Chamar `SaveGameManager.Instance.LoadGame()`
7. **Verificar Console:**
   - "Mesma cena - aguardando managers..."
   - "Aplicando dados do save (mesma cena)..."
   - "Cristal restaurado: Fire x5"
   - "Jogo carregado com sucesso!"
8. **Verificar HUD:** Deve mostrar 5 cristais Fire

### Teste 3: Load em Cena Diferente

1. Play Mode em InitialForest
2. Coletar 5 cristais Fire
3. Tab (salvar)
4. Sair do Play Mode
5. Play Mode em **InitialCave** (cena diferente!)
6. Chamar `SaveGameManager.Instance.LoadGame()`
7. **Verificar Console:**
   - "Carregando cena salva: InitialForest"
   - [Cena recarrega]
   - "SaveGameManager initialized"
   - "Aplicando dados pendentes após reload de cena"
   - "Aplicando dados do save..."
   - "Cristal restaurado: Fire x5"
   - "Dados aplicados com sucesso!"
8. **Verificar:** Deve estar em InitialForest com 5 cristais

### Teste 4: Verificar Zeragem de Cristais

1. Play Mode em InitialForest
2. Coletar 5 cristais Fire
3. Tab (salvar)
4. Coletar mais 3 cristais Fire (total: 8)
5. Chamar `SaveGameManager.Instance.LoadGame()`
6. **Verificar Console:**
   - "Restaurando cristais..."
   - "Cristal restaurado: Fire x5" (não 8!)
7. **Verificar HUD:** Deve mostrar 5 cristais (não 8!)

## 📝 Mudanças no Código

### SaveGameManager.cs

**Adicionado:**
- `private static SaveGameData pendingLoadData;`
- `[SerializeField] private bool enableDetailedLogs = true;`
- `ApplyDataAfterDelay()` coroutine (v1.0.3)
- Logs detalhados em `ApplyPlayerData()`
- Verificação de `pendingLoadData` em `Initialize()`
- `ApplyDataAfterSceneLoad()` coroutine

**Modificado:**
- `LoadGame()` - Usa `pendingLoadData` para reload de cena
- `LoadGame()` - Usa coroutine também para mesma cena (v1.0.3)
- `Initialize()` - Detecta e aplica dados pendentes
- `ApplyPlayerData()` - Logs detalhados e verificações

## ✅ Checklist de Verificação

Antes de testar:
- [ ] Compilação sem erros
- [ ] SaveGameManager nas cenas InitialCave e InitialForest
- [ ] Enable Detailed Logs = true no Inspector
- [ ] Console visível durante testes

Durante teste:
- [ ] Logs aparecem no Console
- [ ] Cristais são restaurados corretamente (mesma cena SEM Play mode restart)
- [ ] Cristais são restaurados corretamente (mesma cena COM Play mode restart)
- [ ] Cristais são restaurados corretamente (cena diferente)
- [ ] Inventário é restaurado
- [ ] Posição do player é restaurada
- [ ] Cena é recarregada quando necessário

## 🎯 Resultado Esperado

✅ **Funcionando:**
- Inventário restaurado corretamente
- Cristais restaurados com valores exatos
- Load funciona na mesma cena sem sair do Play mode (v1.0.3)
- Cena recarregada quando necessário
- Logs detalhados para debug
- Sem acúmulo de cristais

---

**Versão**: 1.0.3  
**Data**: 2026-01-23  
**Status**: ✅ Corrigido e testável
