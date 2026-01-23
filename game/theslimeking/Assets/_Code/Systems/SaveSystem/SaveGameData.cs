using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeKing.Systems.SaveSystem
{
    /// <summary>
    /// Estrutura principal do save game.
    /// Contém todas as categorias de dados salvos.
    /// </summary>
    [Serializable]
    public class SaveGameData
    {
        public string saveVersion = "1.0.0";
        public DateTime saveTimestamp;
        public string currentSceneName;
        
        public PlayerSaveData playerData;
        public WorldSaveData worldData;
        public List<SceneSaveData> scenesData;
        public List<NPCSaveData> npcsData;
        public List<QuestSaveData> questsData;
        public GameFlagsSaveData gameFlagsData;

        public SaveGameData()
        {
            saveTimestamp = DateTime.Now;
            playerData = new PlayerSaveData();
            worldData = new WorldSaveData();
            scenesData = new List<SceneSaveData>();
            npcsData = new List<NPCSaveData>();
            questsData = new List<QuestSaveData>();
            gameFlagsData = new GameFlagsSaveData();
        }
    }

    #region Player Data

    /// <summary>
    /// Dados do personagem jogável.
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        // Posição e cena
        public string currentScene;
        public Vector3 position;
        public bool facingRight = true;

        // Stats
        public int currentHealth;
        public int maxHealth;
        public int attackPower;
        public int defense;
        
        // Inventário
        public List<InventoryItemSaveData> inventory;
        
        // Equipamentos
        public string equippedMask;
        public string equippedHat;
        public string equippedCape;
        
        // Habilidades desbloqueadas
        public List<string> unlockedAbilities;
        
        // Moeda
        public int coins;
        
        // Cristais elementais
        public Dictionary<string, int> crystalCounts;

        public PlayerSaveData()
        {
            inventory = new List<InventoryItemSaveData>();
            unlockedAbilities = new List<string>();
            crystalCounts = new Dictionary<string, int>();
            currentHealth = 6;
            maxHealth = 6;
            attackPower = 1;
            defense = 0;
            coins = 0;
        }
    }

    [Serializable]
    public class InventoryItemSaveData
    {
        public string itemID;
        public int quantity;
        public float durability;

        public InventoryItemSaveData(string id, int qty, float dur = 100f)
        {
            itemID = id;
            quantity = qty;
            durability = dur;
        }
    }

    #endregion

    #region World Data

    /// <summary>
    /// Dados do sistema temporal e mundo.
    /// </summary>
    [Serializable]
    public class WorldSaveData
    {
        public int currentDay = 1;
        public string currentSeason = "Spring";
        public int currentHour = 8;
        public int currentMinute = 0;
        public int seasonCyclesCompleted = 0;
        public float totalPlayTime = 0f;
    }

    #endregion

    #region Scene Data

    /// <summary>
    /// Dados de estado de uma cena específica.
    /// </summary>
    [Serializable]
    public class SceneSaveData
    {
        public string sceneName;
        public List<DestructibleObjectSaveData> destructibleObjects;
        public List<ContainerSaveData> containers;
        public List<DoorSaveData> doors;
        public List<EnvironmentChangeSaveData> environmentChanges;

        public SceneSaveData(string name)
        {
            sceneName = name;
            destructibleObjects = new List<DestructibleObjectSaveData>();
            containers = new List<ContainerSaveData>();
            doors = new List<DoorSaveData>();
            environmentChanges = new List<EnvironmentChangeSaveData>();
        }
    }

    [Serializable]
    public class DestructibleObjectSaveData
    {
        public string objectID;
        public bool isDestroyed;
        public int seasonsUntilRespawn;

        public DestructibleObjectSaveData(string id, bool destroyed, int seasons)
        {
            objectID = id;
            isDestroyed = destroyed;
            seasonsUntilRespawn = seasons;
        }
    }

    [Serializable]
    public class ContainerSaveData
    {
        public string containerID;
        public bool isOpened;
        public bool wasLooted;
        public List<InventoryItemSaveData> remainingItems;

        public ContainerSaveData(string id)
        {
            containerID = id;
            remainingItems = new List<InventoryItemSaveData>();
        }
    }

    [Serializable]
    public class DoorSaveData
    {
        public string doorID;
        public bool isLocked;
        public bool isOpen;

        public DoorSaveData(string id, bool locked, bool open)
        {
            doorID = id;
            isLocked = locked;
            isOpen = open;
        }
    }

    [Serializable]
    public class EnvironmentChangeSaveData
    {
        public string changeID;
        public string changeType;
        public string changeData;

        public EnvironmentChangeSaveData(string id, string type, string data)
        {
            changeID = id;
            changeType = type;
            changeData = data;
        }
    }

    #endregion

    #region NPC Data

    /// <summary>
    /// Dados de um NPC individual.
    /// </summary>
    [Serializable]
    public class NPCSaveData
    {
        public string npcID;
        public string currentScene;
        public Vector3 position;
        public int dialogueProgress;
        public List<string> completedDialogues;
        public int relationshipLevel;
        public string availableSeason;
        public string availableTimeOfDay;
        public bool isMerchant;
        public List<InventoryItemSaveData> merchantInventory;

        public NPCSaveData(string id)
        {
            npcID = id;
            completedDialogues = new List<string>();
            merchantInventory = new List<InventoryItemSaveData>();
        }
    }

    #endregion

    #region Quest Data

    /// <summary>
    /// Dados de uma quest.
    /// </summary>
    [Serializable]
    public class QuestSaveData
    {
        public string questID;
        public QuestStatus status;
        public List<QuestObjectiveSaveData> objectives;
        public bool rewardCollected;
        public string availableSeason;
        public bool isSeasonalQuest;

        public QuestSaveData(string id)
        {
            questID = id;
            status = QuestStatus.NotStarted;
            objectives = new List<QuestObjectiveSaveData>();
        }
    }

    [Serializable]
    public enum QuestStatus
    {
        NotStarted,
        Active,
        Completed,
        Failed
    }

    [Serializable]
    public class QuestObjectiveSaveData
    {
        public string objectiveID;
        public bool isCompleted;
        public int currentProgress;
        public int targetProgress;

        public QuestObjectiveSaveData(string id, int current, int target)
        {
            objectiveID = id;
            currentProgress = current;
            targetProgress = target;
        }
    }

    #endregion

    #region Game Flags

    /// <summary>
    /// Flags globais e eventos do jogo.
    /// </summary>
    [Serializable]
    public class GameFlagsSaveData
    {
        public List<string> triggeredEvents;
        public List<string> unlockedAreas;
        public List<string> discoveredLocations;
        public int storyProgress;
        public string currentChapter;
        public List<string> unlockedAchievements;

        public GameFlagsSaveData()
        {
            triggeredEvents = new List<string>();
            unlockedAreas = new List<string>();
            discoveredLocations = new List<string>();
            unlockedAchievements = new List<string>();
            storyProgress = 0;
            currentChapter = "Prologue";
        }
    }

    #endregion
}
