# Sistema de Save Game - The Slime King

## ✅ Status: Implementado

Sistema completo de save/load criado e adicionado às cenas InitialCave e InitialForest.

## 📦 Arquivos Criados

1. **SaveGameData.cs** (`Assets/_Code/Systems/SaveSystem/`)
   - Estruturas de dados serializáveis
   - PlayerSaveData, WorldSaveData, SceneSaveData, NPCSaveData, QuestSaveData, GameFlagsSaveData

2. **SaveGameManager.cs** (`Assets/_Code/Managers/`)
   - Manager principal com padrão Singleton
   - Auto-save ao abrir inventário
   - Auto-load ao abrir menu (opcional)

## 🎮 Como Funciona

### Auto-Save
- **Quando**: Ao pressionar botão de inventário (Tab)
- **O que salva**: Posição do player, inventário, cristais, stats, cena atual
- **Onde**: `Application.persistentDataPath/savegame.json`

### Auto-Load (Opcional)
- **Quando**: Ao pressionar botão de pause (Escape)
- **Configurável**: Pode ser desabilitado no Inspector
- **Comportamento**: 
  - Recarrega a cena salva automaticamente
  - Restaura posição exata do player
  - Restaura inventário e cristais (valores exatos, não acumulados)
  - Aguarda cena carregar completamente antes de aplicar dados

## 🔧 Configuração

No Inspector do SaveGameManager:

```
Save Settings:
  - Save File Name: savegame.json
  - Use Encryption: false (para debug)
  - Pretty Print: true (para debug)

Auto Save Settings:
  - Auto Save On Inventory Open: true
  - Auto Load On Pause Open: false (recomendado)

Manager Base Settings:
  - Persist Between Scenes: true
  - Enable Logs: false (ativar para debug)
```

## 📊 Estrutura de Dados

### PlayerSaveData
- Posição e cena atual
- Stats (health, attack, defense)
- Inventário completo (ID, quantidade, durabilidade)
- Equipamentos (mask, hat, cape)
- Habilidades desbloqueadas
- Moeda e cristais elementais

### WorldSaveData
- Dia, estação, hora, minuto
- Ciclos de estações completados
- Tempo total de jogo

### SceneSaveData
- Objetos destrutíveis (com contador de respawn)
- Baús e containers
- Portas
- Mudanças ambientais

### NPCSaveData
- Posição e cena
- Progresso de diálogo
- Nível de relacionamento
- Agenda de disponibilidade
- Inventário de mercador

### QuestSaveData
- Status (NotStarted, Active, Completed, Failed)
- Objetivos e progresso
- Recompensas coletadas
- Disponibilidade sazonal

### GameFlagsSaveData
- Eventos globais
- Áreas desbloqueadas
- Locais descobertos
- Progressão de história
- Conquistas

## 💻 API Pública

```csharp
// Salvar manualmente
SaveGameManager.Instance.SaveGame();

// Carregar manualmente
SaveGameManager.Instance.LoadGame();

// Verificar se existe save
bool hasSave = SaveGameManager.Instance.HasSaveGame();

// Obter informações do save
SaveGameInfo info = SaveGameManager.Instance.GetSaveInfo();

// Deletar save
SaveGameManager.Instance.DeleteSaveGame();

// Eventos
SaveGameManager.Instance.OnGameSaved += (data) => { };
SaveGameManager.Instance.OnGameLoaded += (data) => { };
SaveGameManager.Instance.OnSaveError += (error) => { };
```

## 🧪 Testando

1. Entrar em Play Mode na cena InitialForest
2. Coletar alguns itens/cristais (ex: 5 cristais Fire)
3. Pressionar Tab (inventário) → Auto-save
4. Verificar Console: "[SaveGameManager] Jogo salvo"
5. Coletar mais cristais (ex: mais 3 Fire = total 8)
6. Sair do Play Mode
7. Entrar em Play Mode novamente em qualquer cena
8. Chamar `SaveGameManager.Instance.LoadGame()` ou pressionar Escape se auto-load estiver ativo
9. Verificar que:
   - Cena InitialForest foi recarregada
   - Cristais voltaram para 5 (não 8!)
   - Posição do player foi restaurada
   - Itens do inventário foram restaurados

## 📁 Localização do Save

**Windows:**
```
%AppData%\..\LocalLow\[CompanyName]\[GameName]\savegame.json
```

Para abrir rapidamente:
```csharp
Application.OpenURL(Application.persistentDataPath);
```

## 🔄 Próximos Passos

### Fase 2 - Coleta Completa de Dados
- [ ] Sistema de IDs únicos para objetos destrutíveis
- [ ] Salvar estado de baús e containers
- [ ] Salvar estado de portas
- [ ] Salvar mudanças ambientais

### Fase 3 - Integração com Sistemas
- [ ] Sistema temporal (dia/noite, estações)
- [ ] Sistema de NPCs completo
- [ ] Sistema de quests completo

### Fase 4 - Melhorias
- [ ] Múltiplos slots de save
- [ ] UI de seleção de save
- [ ] Screenshots de saves
- [ ] Compressão de dados
- [ ] Encriptação real (AES)
- [ ] Cloud save

## 🐛 Troubleshooting

**Auto-save não funciona:**
- Verificar que Input Action "Gameplay/Inventory" existe
- Verificar que "Auto Save On Inventory Open" está marcado
- Ativar "Enable Logs" para debug

**Auto-load não funciona:**
- Verificar que Input Action "Gameplay/Pause" existe
- Verificar que "Auto Load On Pause Open" está marcado
- Ativar "Enable Logs" para debug

**Itens não carregam:**
- Verificar que ItemData está em `Resources/Items/`
- Verificar nome do item no JSON
- Ativar "Enable Logs" para debug

**SaveGameManager não encontrado:**
- Verificar que GameObject está na cena
- Verificar que componente está ativo
- Verificar logs no Console

## 📝 Notas Técnicas

- Sistema usa JSON para facilitar debug
- Encriptação atual é simples (Base64)
- SaveGameManager persiste entre cenas
- Auto-save/load pode ser desabilitado
- Logs podem ser ativados no Inspector
- **Cristais são zerados antes de restaurar** (evita acúmulo)
- **Cena é recarregada ao fazer load** (garante estado limpo)
- Aguarda 1 frame após carregar cena antes de aplicar dados

---

**Criado**: 2026-01-22  
**Versão**: 1.0.2  
**Status**: ✅ Funcional
