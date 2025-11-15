using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace SlimeKing.Core.AI
{
    /// <summary>
    /// Job paralelo para atualizar IA de múltiplos inimigos simultaneamente
    /// Aproveita múltiplos cores da Unity 6.2 para máxima performance
    /// </summary>
    public struct EnemyAIUpdateJob : IJobParallelFor
    {
        #region Job Data
        /// <summary>Array de dados de IA dos inimigos (read/write)</summary>
        public NativeArray<EnemyAIData> enemyData;

        /// <summary>Posição atual do player (read-only)</summary>
        [ReadOnly] public float3 playerPosition;

        /// <summary>Delta time escalado para taxa de atualização da IA (read-only)</summary>
        [ReadOnly] public float deltaTime;

        /// <summary>Se o player está em modo stealth (read-only)</summary>
        [ReadOnly] public bool isPlayerInStealth;

        /// <summary>Timestamp atual para cálculos de timing (read-only)</summary>
        [ReadOnly] public float currentTime;
        #endregion

        #region Job Execution
        /// <summary>
        /// Executa a atualização de IA para um inimigo específico
        /// </summary>
        /// <param name="index">Índice do inimigo no array</param>
        public void Execute(int index)
        {
            var enemy = enemyData[index];

            // Pula inimigos inativos ou mortos
            if (!enemy.isActive || enemy.currentState == AIState.Dead || enemy.health <= 0)
                return;

            // Atualiza timers
            UpdateTimers(ref enemy);

            // Verifica percepção do player
            UpdatePerception(ref enemy);

            // Atualiza estado da máquina de estados
            UpdateStateMachine(ref enemy);

            // Calcula movimento baseado no estado atual
            UpdateMovement(ref enemy);

            // Salva mudanças de volta no array
            enemyData[index] = enemy;
        }
        #endregion

        #region Timer Updates
        /// <summary>
        /// Atualiza todos os timers do inimigo
        /// </summary>
        private void UpdateTimers(ref EnemyAIData enemy)
        {
            enemy.stateTimer += deltaTime;
            enemy.alertTimer = math.max(0, enemy.alertTimer - deltaTime);
            enemy.attackTimer = math.max(0, enemy.attackTimer - deltaTime);
            enemy.patrolWaitTimer = math.max(0, enemy.patrolWaitTimer - deltaTime);

            if (enemy.isStunned)
            {
                enemy.stunDuration = math.max(0, enemy.stunDuration - deltaTime);
                if (enemy.stunDuration <= 0)
                {
                    enemy.isStunned = false;
                }
            }

            // Atualiza tempo desde que viu o player
            if (!enemy.canSeePlayer)
            {
                enemy.timeSincePlayerSeen += deltaTime;
            }
            else
            {
                enemy.timeSincePlayerSeen = 0;
                enemy.lastKnownPlayerPosition = playerPosition;
            }
        }
        #endregion

        #region Perception System
        /// <summary>
        /// Atualiza percepção do inimigo (visão e audição)
        /// </summary>
        private void UpdatePerception(ref EnemyAIData enemy)
        {
            float distanceToPlayer = math.distance(enemy.position, playerPosition);

            // Verifica visão
            enemy.canSeePlayer = CanSeePlayer(enemy, distanceToPlayer);

            // Verifica audição
            enemy.canHearPlayer = CanHearPlayer(enemy, distanceToPlayer);
        }

        /// <summary>
        /// Verifica se o inimigo pode ver o player
        /// </summary>
        private bool CanSeePlayer(EnemyAIData enemy, float distance)
        {
            if (distance > enemy.detectionRange)
                return false;

            // Aplica redução de stealth se o player estiver escondido
            float effectiveRange = enemy.detectionRange;
            if (isPlayerInStealth)
            {
                effectiveRange *= enemy.stealthDetectionMultiplier;
            }

            return distance <= effectiveRange;
        }

        /// <summary>
        /// Verifica se o inimigo pode ouvir o player
        /// </summary>
        private bool CanHearPlayer(EnemyAIData enemy, float distance)
        {
            float hearingRange = enemy.detectionRange * 0.7f; // Audição é 70% da visão

            // Stealth reduz drasticamente a audição
            if (isPlayerInStealth)
            {
                hearingRange *= 0.2f;
            }

            return distance <= hearingRange;
        }
        #endregion

        #region State Machine
        /// <summary>
        /// Atualiza a máquina de estados do inimigo
        /// </summary>
        private void UpdateStateMachine(ref EnemyAIData enemy)
        {
            // Stun sempre tem prioridade
            if (enemy.isStunned && enemy.currentState != AIState.Stunned)
            {
                ChangeState(ref enemy, AIState.Stunned);
                return;
            }

            if (enemy.currentState == AIState.Stunned && !enemy.isStunned)
            {
                ChangeState(ref enemy, AIState.Idle);
                return;
            }

            // Máquina de estados principal
            AIState newState = CalculateNewState(enemy);

            if (newState != enemy.currentState)
            {
                ChangeState(ref enemy, newState);
            }
        }

        /// <summary>
        /// Calcula o novo estado baseado no estado atual e condições
        /// </summary>
        private AIState CalculateNewState(EnemyAIData enemy)
        {
            float distanceToPlayer = math.distance(enemy.position, playerPosition);

            switch (enemy.currentState)
            {
                case AIState.Idle:
                    if (enemy.canSeePlayer || enemy.canHearPlayer)
                        return AIState.Alert;
                    if (enemy.stateTimer >= enemy.idleTimeout && HasPatrolPoints(enemy))
                        return AIState.Patrol;
                    break;

                case AIState.Patrol:
                    if (enemy.canSeePlayer || enemy.canHearPlayer)
                        return AIState.Alert;
                    if (IsAtCurrentPatrolPoint(enemy))
                    {
                        if (enemy.patrolWaitTimer <= 0)
                            return AIState.Idle;
                    }
                    break;

                case AIState.Alert:
                    if (distanceToPlayer <= enemy.attackRange && enemy.attackTimer <= 0)
                        return AIState.Attack;
                    if (enemy.canSeePlayer && distanceToPlayer > enemy.attackRange)
                        return AIState.Chase;
                    if (enemy.alertTimer <= 0 && !enemy.canSeePlayer)
                        return AIState.Return;
                    break;

                case AIState.Chase:
                    if (distanceToPlayer <= enemy.attackRange && enemy.attackTimer <= 0)
                        return AIState.Attack;
                    if (distanceToPlayer > enemy.chaseRange && enemy.timeSincePlayerSeen > 2f)
                        return AIState.Return;
                    if (!enemy.canSeePlayer && enemy.timeSincePlayerSeen > 5f)
                        return AIState.Return;
                    break;

                case AIState.Attack:
                    if (enemy.stateTimer >= 1f) // Duração do ataque
                    {
                        if (distanceToPlayer <= enemy.chaseRange)
                            return AIState.Chase;
                        else
                            return AIState.Return;
                    }
                    break;

                case AIState.Return:
                    float distanceToHome = math.distance(enemy.position, enemy.homePosition);
                    if (distanceToHome <= 0.5f)
                        return AIState.Idle;
                    if (enemy.canSeePlayer && distanceToPlayer <= enemy.detectionRange)
                        return AIState.Alert;
                    break;
            }

            return enemy.currentState;
        }

        /// <summary>
        /// Muda o estado do inimigo e reseta timers necessários
        /// </summary>
        private void ChangeState(ref EnemyAIData enemy, AIState newState)
        {
            enemy.previousState = enemy.currentState;
            enemy.currentState = newState;
            enemy.stateTimer = 0;

            // Configurações específicas por estado
            switch (newState)
            {
                case AIState.Alert:
                    enemy.alertTimer = enemy.maxAlertTime;
                    break;

                case AIState.Attack:
                    enemy.attackTimer = enemy.attackCooldown;
                    break;

                case AIState.Patrol:
                    AdvancePatrolPoint(ref enemy);
                    break;
            }
        }
        #endregion

        #region Movement System
        /// <summary>
        /// Atualiza o movimento do inimigo baseado no estado atual
        /// </summary>
        private void UpdateMovement(ref EnemyAIData enemy)
        {
            switch (enemy.currentState)
            {
                case AIState.Idle:
                case AIState.Stunned:
                case AIState.Dead:
                    enemy.targetPosition = enemy.position;
                    enemy.moveDirection = float3.zero;
                    break;

                case AIState.Alert:
                case AIState.Chase:
                    enemy.targetPosition = playerPosition;
                    break;

                case AIState.Return:
                    enemy.targetPosition = enemy.homePosition;
                    break;

                case AIState.Patrol:
                    if (HasPatrolPoints(enemy))
                    {
                        enemy.targetPosition = GetCurrentPatrolPoint(enemy);

                        if (IsAtCurrentPatrolPoint(enemy))
                        {
                            if (enemy.patrolWaitTimer <= 0)
                            {
                                enemy.patrolWaitTimer = enemy.patrolWaitTime;
                            }
                        }
                    }
                    break;

                case AIState.Attack:
                    // Para na posição durante o ataque
                    enemy.targetPosition = enemy.position;
                    enemy.moveDirection = float3.zero;
                    break;
            }

            // Calcula direção de movimento
            if (enemy.currentState != AIState.Idle &&
                enemy.currentState != AIState.Stunned &&
                enemy.currentState != AIState.Dead &&
                enemy.currentState != AIState.Attack)
            {
                float3 direction = enemy.targetPosition - enemy.position;
                float distance = math.length(direction);

                if (distance > 0.1f)
                {
                    enemy.moveDirection = math.normalize(direction);
                }
                else
                {
                    enemy.moveDirection = float3.zero;
                }
            }
        }
        #endregion

        #region Patrol System
        /// <summary>
        /// Verifica se o inimigo tem pontos de patrulha configurados
        /// </summary>
        private bool HasPatrolPoints(EnemyAIData enemy)
        {
            return enemy.patrolPoints.IsCreated && enemy.patrolPoints.Length > 0;
        }

        /// <summary>
        /// Obtém o ponto de patrulha atual
        /// </summary>
        private float3 GetCurrentPatrolPoint(EnemyAIData enemy)
        {
            if (!enemy.patrolPoints.IsCreated || enemy.patrolPoints.Length == 0)
                return enemy.homePosition;

            int index = math.clamp(enemy.currentPatrolIndex, 0, enemy.patrolPoints.Length - 1);
            return enemy.patrolPoints[index];
        }

        /// <summary>
        /// Verifica se está próximo do ponto de patrulha atual
        /// </summary>
        private bool IsAtCurrentPatrolPoint(EnemyAIData enemy)
        {
            if (!enemy.patrolPoints.IsCreated || enemy.patrolPoints.Length == 0)
                return true;

            float3 currentPoint = GetCurrentPatrolPoint(enemy);
            return math.distance(enemy.position, currentPoint) <= 0.5f;
        }

        /// <summary>
        /// Avança para o próximo ponto de patrulha
        /// </summary>
        private void AdvancePatrolPoint(ref EnemyAIData enemy)
        {
            if (!enemy.patrolPoints.IsCreated || enemy.patrolPoints.Length <= 1)
                return;

            enemy.currentPatrolIndex++;

            if (enemy.currentPatrolIndex >= enemy.patrolPoints.Length)
            {
                if (enemy.loopPatrol)
                {
                    enemy.currentPatrolIndex = 0;
                }
                else
                {
                    enemy.currentPatrolIndex = enemy.patrolPoints.Length - 1;
                }
            }
        }
        #endregion
    }
}