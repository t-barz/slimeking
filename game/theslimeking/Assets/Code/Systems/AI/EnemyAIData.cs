using Unity.Mathematics;
using Unity.Collections;

namespace SlimeKing.Core.AI
{
    /// <summary>
    /// Dados de IA otimizados para Job System
    /// Struct para performance máxima na Unity 6.2
    /// </summary>
    [System.Serializable]
    public struct EnemyAIData
    {
        #region State Management
        /// <summary>Estado atual da IA do inimigo</summary>
        public AIState currentState;

        /// <summary>Tempo no estado atual (em segundos)</summary>
        public float stateTimer;

        /// <summary>Estado anterior para detecção de mudanças</summary>
        public AIState previousState;
        #endregion

        #region Position Data
        /// <summary>Posição atual do inimigo</summary>
        public float3 position;

        /// <summary>Posição inicial/home do inimigo</summary>
        public float3 homePosition;

        /// <summary>Posição alvo atual</summary>
        public float3 targetPosition;

        /// <summary>Direção de movimento normalizada</summary>
        public float3 moveDirection;
        #endregion

        #region Behavior Configuration
        /// <summary>Alcance de detecção do player</summary>
        public float detectionRange;

        /// <summary>Alcance de perseguição</summary>
        public float chaseRange;

        /// <summary>Alcance de ataque</summary>
        public float attackRange;

        /// <summary>Velocidade de movimento</summary>
        public float moveSpeed;

        /// <summary>Ângulo de visão em graus (para IA mais avançada)</summary>
        public float visionAngle;
        #endregion

        #region Timers and Cooldowns
        /// <summary>Tempo restante em estado de alerta</summary>
        public float alertTimer;

        /// <summary>Tempo máximo em estado de alerta</summary>
        public float maxAlertTime;

        /// <summary>Cooldown entre ataques</summary>
        public float attackCooldown;

        /// <summary>Tempo restante do cooldown de ataque</summary>
        public float attackTimer;

        /// <summary>Tempo para retornar ao idle após patrulha</summary>
        public float idleTimeout;
        #endregion

        #region Patrol System
        /// <summary>Pontos de patrulha (máximo 8 pontos para performance)</summary>
        public NativeArray<float3> patrolPoints;

        /// <summary>Índice atual do ponto de patrulha</summary>
        public int currentPatrolIndex;

        /// <summary>Se deve fazer patrulha em loop</summary>
        public bool loopPatrol;

        /// <summary>Tempo de espera em cada ponto de patrulha</summary>
        public float patrolWaitTime;

        /// <summary>Timer atual de espera na patrulha</summary>
        public float patrolWaitTimer;
        #endregion

        #region Combat Data
        /// <summary>Vida atual</summary>
        public float health;

        /// <summary>Vida máxima</summary>
        public float maxHealth;

        /// <summary>Dano que causa ao player</summary>
        public float attackDamage;

        /// <summary>Se está atordoado</summary>
        public bool isStunned;

        /// <summary>Duração restante do atordoamento</summary>
        public float stunDuration;
        #endregion

        #region Status Flags
        /// <summary>Se a IA está ativa</summary>
        public bool isActive;

        /// <summary>Se pode ver o player atualmente</summary>
        public bool canSeePlayer;

        /// <summary>Se pode ouvir o player atualmente</summary>
        public bool canHearPlayer;

        /// <summary>ID único do inimigo</summary>
        public int enemyId;

        /// <summary>Tipo do inimigo</summary>
        public EnemyType enemyType;
        #endregion

        #region Stealth Integration
        /// <summary>Fator de redução de detecção quando player está em stealth</summary>
        public float stealthDetectionMultiplier;

        /// <summary>Última posição conhecida do player</summary>
        public float3 lastKnownPlayerPosition;

        /// <summary>Tempo desde que viu o player pela última vez</summary>
        public float timeSincePlayerSeen;

        /// <summary>Nível de confiança/suspeita sobre o player (0-1)</summary>
        public float confidenceLevel;
        #endregion
    }

    /// <summary>
    /// Estados da máquina de estados da IA
    /// </summary>
    public enum AIState : byte
    {
        Idle = 0,       // Parado, aguardando
        Patrol = 1,     // Patrulhando pontos definidos
        Alert = 2,      // Detectou algo suspeito
        Chase = 3,      // Perseguindo o player
        Attack = 4,     // Atacando o player
        Return = 5,     // Retornando para posição inicial
        Stunned = 6,    // Atordoado temporariamente
        Dead = 7        // Morto
    }

    /// <summary>
    /// Tipos de inimigos suportados
    /// </summary>
    public enum EnemyType : byte
    {
        AbelhAgressiva = 0,
        ArbustoEspinhoso = 1,
        LoboSelvagem = 2,
        VespaGigante = 3
    }
}