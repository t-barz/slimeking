using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Dados do progresso do jogador
    /// </summary>
    [Serializable]
    public class GameData
    {
        [Header("Player Progress")]
        public int playerLevel = 1;
        public float playerExp = 0f;
        public int currentStage = 1;
        public int highScore = 0;

        [Header("Unlockables")]
        public List<string> unlockedAchievements = new List<string>();
        public List<string> unlockedLevels = new List<string>();

        [Header("Statistics")]
        public float totalPlayTime = 0f;
        public int enemiesDefeated = 0;
        public int gamesPlayed = 0;

        [Header("Save Info")]
        public string lastSaveTime;
        public string version = "1.0.0";

        public GameData()
        {
            lastSaveTime = DateTime.Now.ToBinary().ToString();
        }

        /// <summary>
        /// Atualiza o timestamp do save
        /// </summary>
        public void UpdateSaveTime()
        {
            lastSaveTime = DateTime.Now.ToBinary().ToString();
        }

        /// <summary>
        /// Retorna a data do último save
        /// </summary>
        public DateTime GetLastSaveDate()
        {
            if (long.TryParse(lastSaveTime, out long fileTime))
            {
                return DateTime.FromBinary(fileTime);
            }
            return DateTime.Now;
        }
    }
}