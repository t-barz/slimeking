# Design Document - Quest System (Collect Type)

## Overview

O Quest System para The Slime King é um sistema modular e extensível que permite criar missões de coleta de forma simples através de ScriptableObjects. Esta versão alpha foca no essencial: aceitar quests via diálogo, rastrear progresso automaticamente, e entregar quests com recompensas.

**Princípios de Design:**

- **Simplicidade**: Configuração rápida via ScriptableObjects
- **Modularidade**: Fácil adicionar novos tipos de quest no futuro
- **Desacoplamento**: Comunicação via eventos (padrão do projeto)
- **Performance**: Rastreamento eficiente sem overhead de UI
- **Integração**: Funciona perfeitamente com sistemas existentes
- **Boas Práticas**: Seguindo padrões Manager/Controller/Handler do projeto

## Architecture

### Padrões Arquiteturais Aplicados

**Manager (Singleton):**

- `QuestManager` - Coordena todo o sistema de quests

**Controller (Component):**

- `QuestGiverController` - Controla comportamento de NPCs que oferecem quests
- `QuestNotificationController` - Controla UI de notificações

**Data (ScriptableObject):**

- `CollectQuestData` - Define dados de quests de coleta

**Events (Static):**

- `QuestEvents` - Eventos globais do sistema

### Componentes Principais

```
QuestManager (Singleton Manager)
    ├── Gerencia quests ativas
    ├── Gerencia quests completadas
    ├── Escuta eventos de inventário
    └── Dispara eventos de quest

CollectQuestData (ScriptableObject)
    ├── Define dados da quest
    ├── Item a coletar
    ├── Quantidade necessária
    └── Recompensas

QuestGiverController (MonoBehaviour)
    ├── Anexado a NPCs
    ├── Oferece quests
    └── Recebe quests completadas

QuestEvents (Static Class)
    └── Eventos globais do sistema
```

### Fluxo de Dados

```
1. Designer cria CollectQuestData (ScriptableObject)
2. QuestGiverController NPC referencia CollectQuestData
3. Jogador interage com NPC → Diálogo oferece quest
4. Jogador aceita → QuestManager adiciona à lista ativa
5. Jogador coleta item → InventorySystem dispara evento
6. QuestManager escuta evento → Atualiza progresso
7. Progresso completo → QuestManager marca como "Pronta"
8. Jogador retorna ao NPC → Diálogo oferece entrega
9. Jogador entrega → QuestManager remove itens, adiciona recompensas
10. Quest movida para lista de completadas
```

### Hierarquia de Comunicação (Padrão do Projeto)

```
QuestManager (Coordena - Manager)
    ↓ (eventos)
QuestGiverController (Oferece/Recebe - Controller)
    ↓ (integração)
DialogueSystem (Apresenta opções - System)
    ↓ (eventos)
InventorySystem (Rastreia itens - System)
```

## Components and Interfaces

### 1. QuestManager (Singleton Manager)

**Padrão:** Manager - Gerenciador de Sistema Global

**Responsabilidades:**

- Gerenciar lista de quests ativas
- Gerenciar lista de quests completadas
- Rastrear progresso de quests
- Validar requisitos de quests
- Integrar com Save System
- Coordenar comunicação via eventos

**Estrutura (Seguindo Boas Práticas):**

```csharp
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    
    #region Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    
    #region Inspector Variables
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;
    [SerializeField] private bool showGizmos = true;
    #endregion
    
    #region Private Variables
    private List<QuestProgress> activeQuests = new();
    private List<string> completedQuestIDs = new();
    private Dictionary<string, QuestProgress> activeQuestsDict = new(); // Cache para performance
    private Dictionary<string, QuestGiverController> questGiverCache = new(); // Cache de givers
    #endregion
    
    #region Unity Lifecycle
    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion
    
    #region Initialization
    private void Initialize()
    {
        activeQuests = new List<QuestProgress>();
        completedQuestIDs = new List<string>();
        activeQuestsDict = new Dictionary<string, QuestProgress>();
        questGiverCache = new Dictionary<string, QuestGiverController>();
        
        if (enableDebugLogs)
            Debug.Log("[QuestManager] Initialized");
    }
    #endregion
    
    #region Event Subscription
    private void SubscribeToEvents()
    {
        InventoryEvents.OnItemAdded += OnItemCollected;
        SaveEvents.OnGameSaving += SaveQuestData;
        SaveEvents.OnGameLoading += LoadQuestData;
    }
    
    private void UnsubscribeFromEvents()
    {
        InventoryEvents.OnItemAdded -= OnItemCollected;
        SaveEvents.OnGameSaving -= SaveQuestData;
        SaveEvents.OnGameLoading -= LoadQuestData;
    }
    #endregion
    
    #region Public Methods
    public bool AcceptQuest(CollectQuestData quest);
    public bool CanAcceptQuest(CollectQuestData quest);
    public bool IsQuestActive(string questID);
    public bool IsQuestCompleted(string questID);
    public bool IsQuestReadyToTurnIn(string questID);
    public bool TurnInQuest(string questID);
    public QuestProgress GetQuestProgress(string questID);
    public List<QuestProgress> GetActiveQuests();
    public void RegisterQuestGiver(QuestGiverController giver);
    #endregion
    
    #region Private Methods
    private void OnItemCollected(ItemData item, int quantity);
    private void UpdateQuestProgress(string questID, int newProgress);
    private void CheckQuestCompletion(QuestProgress progress);
    private void GiveRewards(CollectQuestData quest);
    private bool ValidateQuestID(string questID);
    private CollectQuestData FindQuestDataByID(string questID);
    #endregion
    
    #region Save/Load
    private QuestSaveData SaveQuestData();
    private void LoadQuestData(QuestSaveData saveData);
    #endregion
    
    #region Debug Methods
    public void ForceCompleteQuest(string questID);
    public void ResetQuest(string questID);
    public void ClearAllQuests();
    #endregion
    
    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        // Desenhar informações de debug
    }
    #endregion
}
```

### 2. CollectQuestData (ScriptableObject)

**Padrão:** Data - Armazenamento de Dados

**Responsabilidades:**

- Armazenar dados da quest
- Definir item a coletar
- Definir quantidade necessária
- Definir recompensas
- Definir requisitos
- Validação automática

**Estrutura (Seguindo Boas Práticas):**

```csharp
[CreateAssetMenu(fileName = "CollectQuest", menuName = "Quest System/Collect Quest")]
public class CollectQuestData : ScriptableObject
{
    #region Quest Info
    [Header("Quest Info")]
    public string questID;
    public string questName;
    [TextArea(3, 6)]
    public string description;
    #endregion
    
    #region Objective
    [Header("Objective")]
    public ItemData itemToCollect;
    public int quantityRequired = 1;
    #endregion
    
    #region Rewards
    [Header("Rewards")]
    public List<ItemReward> itemRewards = new();
    public int reputationReward = 10;
    #endregion
    
    #region Requirements
    [Header("Requirements")]
    public int minimumReputation = 0;
    public List<CollectQuestData> prerequisiteQuests = new();
    #endregion
    
    #region Settings
    [Header("Settings")]
    public bool isRepeatable = false;
    #endregion
    
    #region Validation
    private void OnValidate()
    {
        // Gera ID único se não fornecido
        if (string.IsNullOrEmpty(questID))
        {
            questID = $"quest_{name}_{GetInstanceID()}";
        }
        
        // Valida item to collect
        if (itemToCollect == null)
        {
            Debug.LogWarning($"[Quest: {questName}] Item to collect não configurado!");
        }
        
        // Valida quantidade
        if (quantityRequired <= 0)
        {
            Debug.LogWarning($"[Quest: {questName}] Quantity required deve ser maior que 0!");
            quantityRequired = 1;
        }
    }
    #endregion
}

[System.Serializable]
public class ItemReward
{
    public ItemData item;
    public int quantity = 1;
}
```

### 3. QuestGiverController (MonoBehaviour Controller)

**Padrão:** Controller - Controlador de Entidade

**Responsabilidades:**

- Anexado a NPCs
- Oferece quests ao jogador
- Recebe quests completadas
- Exibe indicadores visuais
- Registra-se no QuestManager

**Estrutura (Seguindo Boas Práticas):**

```csharp
public class QuestGiverController : MonoBehaviour
{
    #region Inspector Variables
    [Header("Quest Configuration")]
    [SerializeField] private List<CollectQuestData> availableQuests = new();
    
    [Header("Visual Indicators")]
    [SerializeField] private GameObject questAvailableIndicator; // ! amarelo
    [SerializeField] private GameObject questReadyIndicator; // ! dourado
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;
    [SerializeField] private bool showGizmos = true;
    #endregion
    
    #region Unity Lifecycle
    private void Start()
    {
        RegisterWithQuestManager();
        UpdateIndicators();
    }
    
    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion
    
    #region Initialization
    private void RegisterWithQuestManager()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.RegisterQuestGiver(this);
            
            if (enableDebugLogs)
                Debug.Log($"[QuestGiver] {gameObject.name} registered with QuestManager");
        }
    }
    #endregion
    
    #region Event Subscription
    private void SubscribeToEvents()
    {
        QuestEvents.OnQuestAccepted += OnQuestStateChanged;
        QuestEvents.OnQuestReadyToTurnIn += OnQuestReadyChanged;
        QuestEvents.OnQuestTurnedIn += OnQuestStateChanged;
    }
    
    private void UnsubscribeFromEvents()
    {
        QuestEvents.OnQuestAccepted -= OnQuestStateChanged;
        QuestEvents.OnQuestReadyToTurnIn -= OnQuestReadyChanged;
        QuestEvents.OnQuestTurnedIn -= OnQuestStateChanged;
    }
    #endregion
    
    #region Public Methods
    public List<CollectQuestData> GetAvailableQuests()
    {
        return availableQuests;
    }
    
    public bool HasQuestAvailable()
    {
        return availableQuests.Any(q => QuestManager.Instance.CanAcceptQuest(q));
    }
    
    public bool HasQuestReadyToTurnIn()
    {
        return availableQuests.Any(q => QuestManager.Instance.IsQuestReadyToTurnIn(q.questID));
    }
    #endregion
    
    #region Private Methods
    private void OnQuestStateChanged(string questID)
    {
        // Atualiza indicadores apenas se quest pertence a este giver
        if (availableQuests.Any(q => q.questID == questID))
        {
            UpdateIndicators();
        }
    }
    
    private void OnQuestReadyChanged(string questID)
    {
        OnQuestStateChanged(questID);
    }
    
    private void UpdateIndicators()
    {
        bool hasAvailable = HasQuestAvailable();
        bool hasReady = HasQuestReadyToTurnIn();
        
        if (questAvailableIndicator != null)
            questAvailableIndicator.SetActive(hasAvailable && !hasReady);
        
        if (questReadyIndicator != null)
            questReadyIndicator.SetActive(hasReady);
        
        if (enableDebugLogs)
            Debug.Log($"[QuestGiver] {gameObject.name} - Available: {hasAvailable}, Ready: {hasReady}");
    }
    #endregion
    
    #region Gizmos
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // Desenha esfera indicando quest giver
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.5f);
    }
    #endregion
}
```

### 4. QuestProgress (Data Class)

**Padrão:** Data - Classe de Dados

**Responsabilidades:**

- Armazenar progresso de quest ativa
- Rastrear quantidade coletada
- Marcar se está pronta para entrega
- Calcular porcentagem de progresso

**Estrutura:**

```csharp
[System.Serializable]
public class QuestProgress
{
    #region Public Fields
    public string questID;
    public CollectQuestData questData;
    public int currentProgress;
    public int targetProgress;
    public bool isReadyToTurnIn;
    #endregion
    
    #region Constructor
    public QuestProgress(CollectQuestData data)
    {
        questID = data.questID;
        questData = data;
        currentProgress = 0;
        targetProgress = data.quantityRequired;
        isReadyToTurnIn = false;
    }
    #endregion
    
    #region Public Methods
    public float GetProgressPercentage()
    {
        if (targetProgress == 0) return 0f;
        return (float)currentProgress / targetProgress;
    }
    
    public bool IsComplete()
    {
        return currentProgress >= targetProgress;
    }
    #endregion
}
```

### 5. QuestEvents (Static Event Class)

**Padrão:** Events - Comunicação Desacoplada

**Responsabilidades:**

- Centralizar eventos do Quest System
- Permitir comunicação desacoplada
- Facilitar integração com outros sistemas
- Seguir padrão de eventos do projeto

**Estrutura:**

```csharp
public static class QuestEvents
{
    #region Quest Lifecycle Events
    // Eventos de ciclo de vida da quest
    public static event Action<CollectQuestData> OnQuestAccepted;
    public static event Action<string, int, int> OnQuestProgressChanged; // questID, current, target
    public static event Action<string> OnQuestReadyToTurnIn; // questID
    public static event Action<CollectQuestData, List<ItemReward>> OnQuestCompleted;
    public static event Action<string> OnQuestTurnedIn; // questID
    #endregion
    
    #region Helper Methods
    // Métodos helper para disparar eventos com null-conditional operator
    public static void QuestAccepted(CollectQuestData quest)
    {
        OnQuestAccepted?.Invoke(quest);
    }
    
    public static void QuestProgressChanged(string questID, int current, int target)
    {
        OnQuestProgressChanged?.Invoke(questID, current, target);
    }
    
    public static void QuestReadyToTurnIn(string questID)
    {
        OnQuestReadyToTurnIn?.Invoke(questID);
    }
    
    public static void QuestCompleted(CollectQuestData quest, List<ItemReward> rewards)
    {
        OnQuestCompleted?.Invoke(quest, rewards);
    }
    
    public static void QuestTurnedIn(string questID)
    {
        OnQuestTurnedIn?.Invoke(questID);
    }
    #endregion
}
```

### 6. QuestNotificationController (MonoBehaviour Controller)

**Padrão:** Controller - Controlador de UI

**Responsabilidades:**

- Controlar exibição de notificações de quest
- Feedback visual ao completar objetivos
- Feedback ao receber recompensas
- Reproduzir sons aleatórios (evitar repetição)

**Estrutura (Seguindo Boas Práticas):**

```csharp
public class QuestNotificationController : MonoBehaviour
{
    #region Inspector Variables
    [Header("UI References")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMPro.TextMeshProUGUI notificationText;
    [SerializeField] private float displayDuration = 3f;
    
    [Header("Audio - Multiple Sounds")]
    [SerializeField] private List<AudioClip> objectiveCompleteSounds = new();
    [SerializeField] private List<AudioClip> questCompleteSounds = new();
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = false;
    #endregion
    
    #region Private Variables
    private Coroutine currentNotificationCoroutine;
    #endregion
    
    #region Unity Lifecycle
    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }
    #endregion
    
    #region Event Subscription
    private void SubscribeToEvents()
    {
        QuestEvents.OnQuestReadyToTurnIn += OnQuestReadyToTurnIn;
        QuestEvents.OnQuestCompleted += OnQuestCompleted;
    }
    
    private void UnsubscribeFromEvents()
    {
        QuestEvents.OnQuestReadyToTurnIn -= OnQuestReadyToTurnIn;
        QuestEvents.OnQuestCompleted -= OnQuestCompleted;
    }
    #endregion
    
    #region Event Handlers
    private void OnQuestReadyToTurnIn(string questID)
    {
        var progress = QuestManager.Instance.GetQuestProgress(questID);
        if (progress != null)
        {
            ShowQuestReadyToTurnIn(progress.questData.questName);
        }
    }
    
    private void OnQuestCompleted(CollectQuestData quest, List<ItemReward> rewards)
    {
        ShowQuestCompleted(quest.questName, rewards);
    }
    #endregion
    
    #region Public Methods
    public void ShowObjectiveComplete(string questName)
    {
        string message = $"Objetivo Completado: {questName}";
        ShowNotification(message, objectiveCompleteSounds);
    }
    
    public void ShowQuestReadyToTurnIn(string questName)
    {
        string message = $"Quest Pronta: {questName}";
        ShowNotification(message, objectiveCompleteSounds);
    }
    
    public void ShowQuestCompleted(string questName, List<ItemReward> rewards)
    {
        string rewardText = GetRewardText(rewards);
        string message = $"Quest Completada: {questName}\n{rewardText}";
        ShowNotification(message, questCompleteSounds);
    }
    #endregion
    
    #region Private Methods
    private void ShowNotification(string message, List<AudioClip> sounds)
    {
        if (currentNotificationCoroutine != null)
        {
            StopCoroutine(currentNotificationCoroutine);
        }
        
        currentNotificationCoroutine = StartCoroutine(ShowNotificationCoroutine(message, sounds));
    }
    
    private IEnumerator ShowNotificationCoroutine(string message, List<AudioClip> sounds)
    {
        // Exibe notificação
        notificationPanel.SetActive(true);
        notificationText.text = message;
        
        // Reproduz som aleatório
        AudioClip sound = GetRandomSound(sounds);
        if (sound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sound);
        }
        
        if (enableDebugLogs)
            Debug.Log($"[QuestNotification] {message}");
        
        // Aguarda duração
        yield return new WaitForSeconds(displayDuration);
        
        // Esconde notificação
        notificationPanel.SetActive(false);
        currentNotificationCoroutine = null;
    }
    
    private AudioClip GetRandomSound(List<AudioClip> sounds)
    {
        if (sounds == null || sounds.Count == 0) return null;
        
        int randomIndex = UnityEngine.Random.Range(0, sounds.Count);
        return sounds[randomIndex];
    }
    
    private string GetRewardText(List<ItemReward> rewards)
    {
        if (rewards == null || rewards.Count == 0)
            return "Sem recompensas";
        
        string text = "Recompensas: ";
        for (int i = 0; i < rewards.Count; i++)
        {
            text += $"{rewards[i].item.itemName} x{rewards[i].quantity}";
            if (i < rewards.Count - 1)
                text += ", ";
        }
        return text;
    }
    #endregion
}
```

## Data Models

### Quest Save Data

```csharp
[System.Serializable]
public class QuestSaveData
{
    public List<QuestProgressData> activeQuests = new();
    public List<string> completedQuestIDs = new();
}

[System.Serializable]
public class QuestProgressData
{
    public string questID;
    public int currentProgress;
    public bool isReadyToTurnIn;
}
```

## Integration with Existing Systems

### 1. Integração com Dialogue System

O QuestGiverController se integra com o DialogueSystem existente para oferecer opções de aceitar e entregar quests.

### 2. Integração com Inventory System

O QuestManager escuta eventos do InventorySystem para rastrear itens coletados automaticamente.

### 3. Integração com Save System

O QuestManager se registra nos eventos do SaveSystem para serializar/deserializar progresso de quests.

### 4. Integração com Reputation System

Ao entregar quest, QuestManager adiciona reputação via GameManager.

## File Organization (Seguindo Padrão do Projeto)

```
Assets/
├── Code/
│   ├── Systems/
│   │   └── QuestSystem/
│   │       ├── QuestManager.cs (Manager)
│   │       ├── QuestEvents.cs (Events)
│   │       └── QuestSaveData.cs (Data)
│   ├── Gameplay/
│   │   └── Quest/
│   │       ├── QuestGiverController.cs (Controller)
│   │       ├── QuestNotificationController.cs (Controller)
│   │       └── QuestProgress.cs (Data)
│   └── Data/
│       └── Quest/
│           ├── CollectQuestData.cs (ScriptableObject)
│           └── ItemReward.cs (Data)
├── Editor/
│   └── QuestSystem/
│       ├── QuestManagerEditor.cs
│       ├── ItemRewardDrawer.cs
│       └── QuestCreationTool.cs
├── Data/
│   └── Quests/
│       └── (CollectQuestData assets)
└── Prefabs/
    └── UI/
        └── QuestNotificationPanel.prefab
```

## Summary

O Quest System foi redesenhado seguindo rigorosamente as Boas Práticas do projeto:

✅ **Padrões Arquiteturais**: Manager/Controller/Handler aplicados corretamente
✅ **Regiões**: Código organizado em regiões lógicas
✅ **Debug/Logs**: Todas classes têm opção de debug
✅ **Gizmos**: Visualização no Editor com opção de desativar
✅ **Eventos**: Comunicação desacoplada via eventos estáticos
✅ **Performance**: Cache e otimizações implementadas
✅ **Sons Aleatórios**: Listas de sons para evitar repetição
✅ **Singleton**: QuestManager implementado corretamente
✅ **Nomenclatura**: camelCase/PascalCase seguindo padrão
