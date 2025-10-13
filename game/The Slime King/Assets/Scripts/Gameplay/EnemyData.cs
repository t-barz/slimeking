using System;
using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Tipos de inimigos no jogo
    /// </summary>
    public enum EnemyType
    {
        BasicSlime,
        SpeedSlime,
        TankSlime,
        BossSlime
    }
    
    /// <summary>
    /// Estados dos inimigos
    /// </summary>
    public enum EnemyState
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        Dead,
        Returning
    }
    
    /// <summary>
    /// Dados básicos de um inimigo
    /// </summary>
    [Serializable]
    public class EnemyData
    {
        [Header("Identity")]
        public EnemyType type;
        public string name;
        
        [Header("Stats")]
        public int health = 50;
        public int attack = 10;
        public float moveSpeed = 3f;
        public float detectionRange = 5f;
        public float attackRange = 1.5f;
        
        [Header("Rewards")]
        public float expReward = 25f;
        public int scoreReward = 100;
        
        [Header("Prefab")]
        public GameObject prefab;
    }
    
    /// <summary>
    /// Dados de spawn de um inimigo
    /// </summary>
    [Serializable]
    public class EnemySpawnData
    {
        public EnemyType enemyType;
        public Vector3 spawnPosition;
        public int count = 1;
        public float spawnDelay = 0f;
    }
}