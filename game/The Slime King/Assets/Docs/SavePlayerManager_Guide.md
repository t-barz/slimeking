# Guia de Uso - SavePlayerManager

## Visão Geral

O `SavePlayerManager` é um sistema completo de salvamento e carregamento que gerencia:

- **EntityStatus** (HP, ataque, defesa, velocidade, nível)
- **Posição do jogador** e cena atual
- **Pontos de respawn** customizáveis
- **Salvamento automático** e 3 slots manuais

## Configuração Inicial

### 1. Adicionar à Cena

1. Crie um GameObject vazio chamado "SavePlayerManager"
2. Adicione o script `SavePlayerManager`
3. Configure no Inspector:
   - **Enable Auto Save**: ✅ (ativado)
   - **Auto Save Interval**: 30 (segundos)
   - **Player Tag**: "Player"
   - **Enable Debug Logs**: ✅ (para desenvolvimento)

### 2. Singleton Automático

- O componente se torna um Singleton automaticamente
- Persiste entre cenas com `DontDestroyOnLoad`
- Apenas uma instância existirá no jogo

## Funcionalidades Principais

### Salvamento Automático

```csharp
// O auto-save funciona automaticamente a cada 30 segundos
// Para forçar um save imediato:
SavePlayerManager.Instance.ForceSave();
```

### Salvamento Manual (Slots)

```csharp
// Salvar no Slot 0, 1 ou 2
SavePlayerManager.Instance.SavePlayerData(0);
SavePlayerManager.Instance.SavePlayerData(1);
SavePlayerManager.Instance.SavePlayerData(2);
```

### Carregamento

```csharp
// Carregar auto-save (padrão)
SavePlayerManager.Instance.LoadPlayerData();

// Carregar slot específico
SavePlayerManager.Instance.LoadPlayerData(0);
```

### Gerenciamento de Respawn

```csharp
// Definir novo ponto de respawn
Vector3 checkpointPosition = new Vector3(10f, 5f, 0f);
SavePlayerManager.Instance.SetRespawnPoint(checkpointPosition);

// Respawnar jogador (após morte)
SavePlayerManager.Instance.RespawnPlayer();
```

### Verificar Slots

```csharp
// Verificar se slot tem dados salvos
bool hasData = SavePlayerManager.Instance.HasSaveData(0);

// Obter informações do slot sem carregar
var slotInfo = SavePlayerManager.Instance.GetSaveSlotInfo(0);
if (slotInfo != null)
{
    Debug.Log($"Save do dia: {slotInfo.saveTime}");
    Debug.Log($"Cena: {slotInfo.currentSceneName}");
    Debug.Log($"Nível: {slotInfo.playerLevel}");
}

// Deletar save
SavePlayerManager.Instance.DeleteSaveData(0);
```

## Dados Salvos

### EntityStatus

- **Vida atual** (currentHP)
- **Vida máxima** (calculada com base no nível)
- **Velocidade** (speed)
- **Dano de ataque** (attack)
- **Poder de defesa** (defense)
- **Nível atual** (level)

### Posição e Mundo

- **Cena atual** (nome da scene)
- **Posição do jogador** (x, y, z)
- **Ponto de respawn** (cena e posição)

### Metadados

- **Data/hora do save**
- **Tempo de jogo** (play time)

## Integração com Outros Sistemas

### Com PortalManager

O sistema funciona automaticamente com transições de cena:

- Salva a posição atual antes de mudar de cena
- Carrega dados salvos ao entrar em nova cena

### Com Sistema de Morte

```csharp
// Quando o jogador morre, chame:
SavePlayerManager.Instance.RespawnPlayer();
```

### Com Checkpoints

```csharp
// Em pontos de checkpoint, defina novo respawn:
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SavePlayerManager.Instance.SetRespawnPoint(transform.position);
        }
    }
}
```

## Interface para UI de Menu

### Listar Slots de Save

```csharp
for (int i = 0; i < 3; i++)
{
    var slotInfo = SavePlayerManager.Instance.GetSaveSlotInfo(i);
    if (slotInfo != null)
    {
        // Exibir informações do slot na UI
        string displayText = $"Slot {i}: {slotInfo.currentSceneName} - {slotInfo.saveTime:dd/MM/yyyy HH:mm}";
    }
    else
    {
        // Slot vazio
        string displayText = $"Slot {i}: Vazio";
    }
}
```

### Botões de Menu

```csharp
// Botão "Salvar Jogo"
public void OnSaveButtonClick(int slotNumber)
{
    SavePlayerManager.Instance.SavePlayerData(slotNumber);
}

// Botão "Carregar Jogo"
public void OnLoadButtonClick(int slotNumber)
{
    SavePlayerManager.Instance.LoadPlayerData(slotNumber);
}

// Botão "Deletar Save"
public void OnDeleteButtonClick(int slotNumber)
{
    SavePlayerManager.Instance.DeleteSaveData(slotNumber);
}
```

## Eventos de Ciclo de Vida

### Inicialização Automática

- **OnSceneLoaded**: Carrega dados salvos automaticamente
- **Start**: Carrega auto-save se disponível
- **Update**: Executa auto-save periódico

### Ao Fechar o Jogo

```csharp
// No evento ApplicationPause ou ApplicationFocus:
private void OnApplicationPause(bool pauseStatus)
{
    if (pauseStatus)
    {
        SavePlayerManager.Instance.ForceSave();
    }
}
```

## Logs de Debug

Com debug ativado, você verá:

```
[SavePlayerManager] Cena carregada: InitialForest
[SavePlayerManager] Dados carregados do Auto-Save
[SavePlayerManager] Posição aplicada: (10.5, 5.2, 0.0)
[SavePlayerManager] Dados salvos no Slot 1
[SavePlayerManager] Novo ponto de respawn definido: (15.0, 8.0, 0.0) na cena InitialCave
```

## Resolução de Problemas

### Dados não são carregados

1. Verifique se o jogador tem a tag "Player"
2. Confirme que o SavePlayerManager está na cena
3. Ative debug logs para verificar o processo

### Auto-save não funciona

1. Verifique se "Enable Auto Save" está ativado
2. Confirme o intervalo de auto-save
3. Teste com ForceSave() manualmente

### Respawn não funciona

1. Verifique se SetRespawnPoint() foi chamado
2. Confirme se há dados de respawn salvos
3. Teste com logs para ver o processo

## Notas Técnicas

- Utiliza `PlayerPrefs` para persistência entre sessões
- Serialização JSON para estruturas complexas
- Thread-safe e otimizado para performance
- Compatível com build para todas as plataformas Unity
