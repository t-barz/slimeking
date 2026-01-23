# Sistema de Save Game - Changelog

## Versão 1.0.3 - 2026-01-23

### 🐛 Correção Crítica: Dados Perdidos ao Carregar na Mesma Cena

**Problema:**
- Ao carregar save na mesma cena sem sair do Play mode:
  - ✅ Posição do player restaurada corretamente
  - ❌ Inventário ficava vazio
  - ❌ Cristais eram zerados

**Causa:**
- `LoadGame()` tinha dois caminhos diferentes:
  1. **Cena diferente**: Usava coroutine com delay de 2 frames
  2. **Mesma cena**: Chamava `ApplyAllData()` imediatamente
- A chamada imediata não dava tempo para managers estarem prontos

**Solução:**
- ✅ Ambos os caminhos agora usam coroutine com delay
- ✅ Nova coroutine `ApplyDataAfterDelay()` para mesma cena
- ✅ Garante que managers estejam prontos antes de aplicar dados

**Mudanças:**
```csharp
// LoadGame agora usa coroutine para mesma cena
else
{
    Log("Mesma cena - aguardando managers...");
    StartCoroutine(ApplyDataAfterDelay());
}

// Nova coroutine adicionada
private IEnumerator ApplyDataAfterDelay()
{
    yield return null;
    yield return null;
    
    Log("Aplicando dados do save (mesma cena)...");
    ApplyAllData();
    Log("Jogo carregado com sucesso!");
    OnGameLoaded?.Invoke(currentSaveData);
}
```

**Logs Adicionados:**
- "Mesma cena - aguardando managers..."
- "Aplicando dados do save (mesma cena)..."
- "Jogo carregado com sucesso!"

**Testes Validados:**
- ✅ Load na mesma cena sem sair do Play mode
- ✅ Load na mesma cena com saída do Play mode
- ✅ Load em cena diferente (já funcionava)

---

## Versão 1.0.2 - 2026-01-22

### 🐛 Correção Crítica: Dados Perdidos ao Recarregar Cena

**Problema:**
- Ao carregar save de cena diferente, inventário ficava vazio
- Cristais eram zerados
- Dados não eram aplicados após reload

**Causa:**
- `SceneManager.LoadScene()` destruía SaveGameManager
- Variáveis de instância eram perdidas antes de aplicar dados

**Solução:**
- ✅ Variável `static pendingLoadData` persiste entre reloads
- ✅ `Initialize()` detecta dados pendentes e aplica após reload
- ✅ Aguarda 2 frames para garantir inicialização de managers
- ✅ Logs detalhados para debug

**Mudanças:**
```csharp
// Variável static persiste entre destruição/criação
private static SaveGameData pendingLoadData;

// LoadGame salva dados antes de reload
if (savedScene != currentScene)
{
    pendingLoadData = currentSaveData;
    SceneManager.LoadScene(savedScene);
}

// Initialize detecta e aplica dados pendentes
if (pendingLoadData != null)
{
    currentSaveData = pendingLoadData;
    pendingLoadData = null;
    StartCoroutine(ApplyDataAfterSceneLoad());
}
```

**Logs Adicionados:**
- "Aplicando dados pendentes após reload de cena"
- "Aplicando PlayerData: X itens, Y tipos de cristais"
- "Item restaurado: ItemName xQuantity"
- "Cristal restaurado: CrystalType xQuantity"
- Warnings quando managers não são encontrados

**Nova Configuração:**
```
[Header("Debug")]
[SerializeField] private bool enableDetailedLogs = true;
```

---

## Versão 1.0.1 - 2026-01-22

### 🔧 Melhorias

**Correção no Sistema de Cristais**

- ✅ Cristais agora são zerados antes de restaurar valores salvos
- ✅ Evita acúmulo incorreto de cristais ao carregar save múltiplas vezes
- ✅ Garante que a quantidade salva é exatamente a quantidade restaurada

**Recarregamento de Cena ao Fazer Load**

- ✅ Sistema agora recarrega a cena salva automaticamente
- ✅ Se save foi feito em InitialCave e você está em InitialForest, ao carregar ele troca para InitialCave
- ✅ Aguarda a cena carregar completamente antes de aplicar dados
- ✅ Evita problemas de referências perdidas

**Mudanças no Código:**

1. **ApplyPlayerData()**
   - Zera todos os cristais antes de restaurar
   - Usa `RemoveCrystal()` para limpar contadores
   - Adiciona apenas os valores salvos

2. **LoadGame()**
   - Verifica se cena atual é diferente da cena salva
   - Carrega cena salva usando `LoadSceneAsync`
   - Aguarda carregamento completo
   - Aplica dados após cena estar pronta

3. **Novos Métodos**
   - `LoadSceneAndApplyData()` - Coroutine para carregar cena
   - `ApplyAllData()` - Aplica todos os dados em sequência

**Comportamento:**

```csharp
// Exemplo de uso:
// 1. Salvar em InitialCave com 5 cristais Fire
SaveGameManager.Instance.SaveGame();

// 2. Ir para InitialForest
// 3. Coletar mais 3 cristais Fire (total: 8)

// 4. Carregar save
SaveGameManager.Instance.LoadGame();
// → Recarrega InitialCave
// → Restaura 5 cristais Fire (não 8!)
// → Posição do player restaurada
```

---

## Versão 1.0.0 - 2026-01-22

### 🔧 Correções

**Substituição de Input Action Menu por Pause**

- ❌ Removido: `InputSystem_Actions.Gameplay.Menu` (não existe)
- ✅ Adicionado: `InputSystem_Actions.Gameplay.Pause` (correto)

**Mudanças no Código:**

1. **SaveGameManager.cs**
   - Variável `menuAction` → `pauseAction`
   - Método `OnMenuPressed()` → `OnPausePressed()`
   - Campo `autoLoadOnMenuOpen` → `autoLoadOnPauseOpen`
   - Input Action: `"Gameplay/Menu"` → `"Gameplay/Pause"`

2. **Documentação**
   - Atualizado SaveSystem_README.md
   - Corrigido referências de "Menu" para "Pause"

**Comportamento:**

- ✅ Auto-save ao pressionar Tab (Inventory) - **Inalterado**
- ✅ Auto-load ao pressionar Escape (Pause) - **Corrigido**

**Configuração no Inspector:**

```
Auto Save Settings:
  - Auto Save On Inventory Open: true
  - Auto Load On Pause Open: false  ← Nome atualizado
```

---

## Versão 0.9.0 - 2026-01-22

### 🎉 Lançamento Inicial

**Arquivos Criados:**
- SaveGameData.cs
- SaveGameManager.cs
- SaveSystem_README.md

**Funcionalidades:**
- Sistema completo de save/load
- Auto-save ao abrir inventário
- Auto-load ao pressionar pause (opcional)
- Estruturas de dados para Player, World, Scene, NPC, Quest, GameFlags
- Integração com InventoryManager e GameManager
- Suporte a JSON e encriptação opcional

**Cenas:**
- SaveGameManager adicionado à InitialForest
- SaveGameManager adicionado à InitialCave

---

## 📝 Notas de Migração

Se você já estava usando a versão 1.0.0, não é necessário fazer nada. O Unity irá recompilar automaticamente e o sistema continuará funcionando.

A única diferença é que agora o auto-load responde ao botão de Pause (Escape) ao invés de um botão de Menu inexistente.

---

**Última atualização**: 2026-01-23  
**Versão atual**: 1.0.3
