using System;
using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Estatísticas base do jogador
    /// </summary>
    [Serializable]
    public class PlayerStats
    {
        [Header("Combat Stats")]
        public int health = 100;
        public int maxHealth = 100;
        public int mana = 50;
        public int maxMana = 50;
        public int attack = 10;
        public int defense = 5;
        
        [Header("Movement Stats")]
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        
        [Header("Level Stats")]
        public int level = 1;
        public float experience = 0f;
        public float experienceToNext = 100f;
        
        /// <summary>
        /// Clona as stats
        /// </summary>
        public PlayerStats Clone()
        {
            return (PlayerStats)MemberwiseClone();
        }
        
        /// <summary>
        /// Reseta para valores padrão
        /// </summary>
        public void Reset()
        {
            health = maxHealth;
            mana = maxMana;
        }
        
        /// <summary>
        /// Calcula stats baseado no nível
        /// </summary>
        public void CalculateStatsForLevel(int targetLevel)
        {
            level = targetLevel;
            maxHealth = 100 + (level - 1) * 20;
            maxMana = 50 + (level - 1) * 10;
            attack = 10 + (level - 1) * 2;
            defense = 5 + (level - 1) * 1;
            experienceToNext = level * 100f;
            
            health = maxHealth;
            mana = maxMana;
        }
    }
}