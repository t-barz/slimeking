using UnityEngine;
using UnityEngine.Events;
using SlimeKing.Core;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Manager que controla o estado e progresso do jogador.
    /// Gerencia stats, nível, experiência e posição.
    /// </summary>
    public class PlayerManager : ManagerBase<PlayerManager>
    {
        [Header("Player References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private PlayerStats baseStats;

        [Header("Level System")]
        [SerializeField] private bool autoLevelUp = true;
        [SerializeField] private float expMultiplier = 1f;

        [Header("Events")]
        public UnityEvent<int> OnHealthChanged = new UnityEvent<int>();
        public UnityEvent<int> OnManaChanged = new UnityEvent<int>();
        public UnityEvent<float> OnExperienceChanged = new UnityEvent<float>();
        public UnityEvent<int> OnLevelUp = new UnityEvent<int>();
        public UnityEvent OnPlayerDied = new UnityEvent();
        public UnityEvent OnPlayerRespawned = new UnityEvent();

        // Stats atuais (cópia modificável das base stats)
        private PlayerStats currentStats;

        // Estado do jogador
        private bool isDead = false;
        private Vector3 lastSafePosition;

        // Properties
        public PlayerStats CurrentStats => currentStats;
        public bool IsDead => isDead;
        public Vector3 PlayerPosition => playerTransform != null ? playerTransform.position : Vector3.zero;
        public Transform PlayerTransform => playerTransform;

        protected override void Initialize()
        {
            InitializePlayerStats();
            LoadPlayerData();
            Log("PlayerManager initialized");
        }

        #region Setup

        /// <summary>
        /// Inicializa as estatísticas do jogador
        /// </summary>
        private void InitializePlayerStats()
        {
            if (baseStats == null)
            {
                baseStats = new PlayerStats();
            }

            currentStats = baseStats.Clone();

            // Se não há player transform, tenta encontrar
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }

            lastSafePosition = PlayerPosition;
        }

        /// <summary>
        /// Carrega dados do jogador do SaveManager
        /// </summary>
        private void LoadPlayerData()
        {
            if (SaveManager.Instance != null)
            {
                var gameData = SaveManager.Instance.CurrentGameData;
                if (gameData != null)
                {
                    currentStats.CalculateStatsForLevel(gameData.playerLevel);
                    currentStats.experience = gameData.playerExp;

                    Log($"Player data loaded - Level: {currentStats.level}, Exp: {currentStats.experience}");
                }
            }
        }

        #endregion

        #region Health System

        /// <summary>
        /// Recebe dano
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDead) return;

            int actualDamage = Mathf.Max(1, damage - currentStats.defense);
            currentStats.health = Mathf.Max(0, currentStats.health - actualDamage);

            OnHealthChanged?.Invoke(currentStats.health);
            Log($"Player took {actualDamage} damage. Health: {currentStats.health}/{currentStats.maxHealth}");

            if (currentStats.health <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Cura o jogador
        /// </summary>
        public void Heal(int amount)
        {
            if (isDead) return;

            int oldHealth = currentStats.health;
            currentStats.health = Mathf.Min(currentStats.maxHealth, currentStats.health + amount);

            if (currentStats.health > oldHealth)
            {
                OnHealthChanged?.Invoke(currentStats.health);
                Log($"Player healed {currentStats.health - oldHealth}. Health: {currentStats.health}/{currentStats.maxHealth}");
            }
        }

        /// <summary>
        /// Cura completa
        /// </summary>
        public void FullHeal()
        {
            currentStats.health = currentStats.maxHealth;
            currentStats.mana = currentStats.maxMana;
            OnHealthChanged?.Invoke(currentStats.health);
            OnManaChanged?.Invoke(currentStats.mana);
            Log("Player fully healed");
        }

        #endregion

        #region Mana System

        /// <summary>
        /// Consome mana
        /// </summary>
        public bool ConsumeMana(int amount)
        {
            if (currentStats.mana >= amount)
            {
                currentStats.mana -= amount;
                OnManaChanged?.Invoke(currentStats.mana);
                Log($"Consumed {amount} mana. Mana: {currentStats.mana}/{currentStats.maxMana}");
                return true;
            }

            LogWarning("Not enough mana");
            return false;
        }

        /// <summary>
        /// Restaura mana
        /// </summary>
        public void RestoreMana(int amount)
        {
            int oldMana = currentStats.mana;
            currentStats.mana = Mathf.Min(currentStats.maxMana, currentStats.mana + amount);

            if (currentStats.mana > oldMana)
            {
                OnManaChanged?.Invoke(currentStats.mana);
                Log($"Restored {currentStats.mana - oldMana} mana. Mana: {currentStats.mana}/{currentStats.maxMana}");
            }
        }

        #endregion

        #region Experience System

        /// <summary>
        /// Adiciona experiência
        /// </summary>
        public void AddExperience(float amount)
        {
            if (isDead) return;

            float expToAdd = amount * expMultiplier;
            currentStats.experience += expToAdd;

            OnExperienceChanged?.Invoke(currentStats.experience);
            Log($"Gained {expToAdd} experience. Total: {currentStats.experience}");

            // Salva no SaveManager
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.AddExperience(expToAdd);
            }

            // Verifica level up
            if (autoLevelUp && currentStats.experience >= currentStats.experienceToNext)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// Sobe de nível
        /// </summary>
        public void LevelUp()
        {
            int oldLevel = currentStats.level;
            currentStats.level++;

            // Atualiza stats baseado no novo nível
            currentStats.CalculateStatsForLevel(currentStats.level);

            // Salva no SaveManager
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.SetPlayerLevel(currentStats.level);
            }

            OnLevelUp?.Invoke(currentStats.level);
            Log($"Level up! New level: {currentStats.level}");

            // Verifica se pode subir mais níveis
            if (currentStats.experience >= currentStats.experienceToNext)
            {
                LevelUp();
            }
        }

        #endregion

        #region Life and Death

        /// <summary>
        /// Mata o jogador
        /// </summary>
        private void Die()
        {
            if (isDead) return;

            isDead = true;
            OnPlayerDied?.Invoke();
            Log("Player died");

            // Para o jogo ou inicia respawn
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeGameState(GameState.GameOver);
            }
        }

        /// <summary>
        /// Respawna o jogador
        /// </summary>
        public void Respawn(Vector3? respawnPosition = null)
        {
            isDead = false;

            // Restaura stats
            currentStats.health = currentStats.maxHealth;
            currentStats.mana = currentStats.maxMana;

            // Move para posição de respawn
            Vector3 spawnPos = respawnPosition ?? lastSafePosition;
            SetPlayerPosition(spawnPos);

            OnHealthChanged?.Invoke(currentStats.health);
            OnManaChanged?.Invoke(currentStats.mana);
            OnPlayerRespawned?.Invoke();

            Log($"Player respawned at {spawnPos}");
        }

        #endregion

        #region Position Management

        /// <summary>
        /// Define a posição do jogador
        /// </summary>
        public void SetPlayerPosition(Vector3 position)
        {
            if (playerTransform != null)
            {
                playerTransform.position = position;
                lastSafePosition = position;
            }
        }

        /// <summary>
        /// Salva a posição atual como segura
        /// </summary>
        public void SaveSafePosition()
        {
            lastSafePosition = PlayerPosition;
            Log($"Safe position saved: {lastSafePosition}");
        }

        /// <summary>
        /// Volta para a última posição segura
        /// </summary>
        public void ReturnToSafePosition()
        {
            SetPlayerPosition(lastSafePosition);
            Log("Returned to safe position");
        }

        #endregion

        #region Player Reference

        /// <summary>
        /// Define a referência do transform do jogador
        /// </summary>
        public void SetPlayerTransform(Transform newPlayerTransform)
        {
            playerTransform = newPlayerTransform;
            if (playerTransform != null)
            {
                lastSafePosition = playerTransform.position;
                Log("Player transform reference updated");
            }
        }

        #endregion

        #region Save/Load Integration

        /// <summary>
        /// Salva dados atuais do jogador
        /// </summary>
        public void SavePlayerData()
        {
            if (SaveManager.Instance != null)
            {
                var gameData = SaveManager.Instance.CurrentGameData;
                if (gameData != null)
                {
                    gameData.playerLevel = currentStats.level;
                    gameData.playerExp = currentStats.experience;
                    SaveManager.Instance.SaveGameData();
                }
            }
        }

        #endregion
    }
}