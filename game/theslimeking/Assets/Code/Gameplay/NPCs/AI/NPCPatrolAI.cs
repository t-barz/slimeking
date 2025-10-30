using System.Collections.Generic;
using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// IA de patrulha para NPCs que se movem entre pontos fixos.
    /// O NPC move-se em sequência através de uma lista de pontos, esperando em cada um.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class NPCPatrolAI : MonoBehaviour
    {
        [Header("Patrol Configuration")]
        [Tooltip("Lista de pontos de patrulha (posições no mundo)")]
        public List<Vector2> patrolPoints = new List<Vector2>();

        [Tooltip("Velocidade de movimento durante a patrulha (m/s)")]
        public float patrolSpeed = 2.5f;

        [Tooltip("Tempo de espera em cada ponto de patrulha (segundos)")]
        public float waitAtPoint = 1.0f;

        [Header("State")]
        [Tooltip("Índice do ponto de patrulha atual")]
        private int currentPointIndex = 0;

        [Tooltip("Indica se o NPC está atualmente esperando em um ponto")]
        private bool isWaiting = false;

        [Tooltip("Tempo restante de espera no ponto atual")]
        private float waitTimer = 0f;

        [Tooltip("Distância mínima para considerar que chegou ao ponto")]
        private const float ARRIVAL_THRESHOLD = 0.1f;

        private Rigidbody2D rb;
        private Animator animator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            // Validar pontos de patrulha
            if (patrolPoints == null || patrolPoints.Count < 2)
            {
                Debug.LogWarning($"⚠️ NPCPatrolAI em '{gameObject.name}' precisa de pelo menos 2 pontos de patrulha!", this);
                enabled = false;
                return;
            }

            Debug.Log($"🚶 {gameObject.name} iniciou patrulha com {patrolPoints.Count} pontos");
        }

        private void Update()
        {
            if (patrolPoints == null || patrolPoints.Count < 2)
            {
                return;
            }

            if (isWaiting)
            {
                HandleWait();
            }
            else
            {
                MoveTowardsCurrentPoint();
            }
        }

        /// <summary>
        /// Move o NPC em direção ao ponto de patrulha atual.
        /// </summary>
        private void MoveTowardsCurrentPoint()
        {
            Vector2 currentPosition = transform.position;
            Vector2 targetPosition = patrolPoints[currentPointIndex];
            float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

            // Verificar se chegou ao ponto
            if (distanceToTarget <= ARRIVAL_THRESHOLD)
            {
                OnReachedPatrolPoint();
                return;
            }

            // Calcular direção e mover
            Vector2 direction = (targetPosition - currentPosition).normalized;
            Vector2 newPosition = currentPosition + direction * patrolSpeed * Time.deltaTime;

            // Aplicar movimento
            rb.MovePosition(newPosition);

            // Atualizar animação
            if (animator != null)
            {
                animator.SetFloat("Speed", patrolSpeed);
            }

            // Opcional: Rotacionar sprite baseado na direção
            // FlipSprite(direction.x);
        }

        /// <summary>
        /// Gerencia o estado de espera em um ponto de patrulha.
        /// </summary>
        private void HandleWait()
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                EndWait();
            }
        }

        /// <summary>
        /// Chamado quando o NPC chega a um ponto de patrulha.
        /// </summary>
        private void OnReachedPatrolPoint()
        {
            Debug.Log($"📍 {gameObject.name} chegou ao ponto de patrulha {currentPointIndex + 1}/{patrolPoints.Count}");

            // Iniciar espera
            StartWait();
        }

        /// <summary>
        /// Inicia a espera em um ponto de patrulha.
        /// </summary>
        private void StartWait()
        {
            isWaiting = true;
            waitTimer = waitAtPoint;

            // Parar animação de movimento
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        /// <summary>
        /// Encerra a espera e avança para o próximo ponto.
        /// </summary>
        private void EndWait()
        {
            isWaiting = false;
            MoveToNextPoint();
        }

        /// <summary>
        /// Avança para o próximo ponto de patrulha na sequência.
        /// </summary>
        private void MoveToNextPoint()
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
            Debug.Log($"🎯 {gameObject.name} indo para ponto de patrulha {currentPointIndex + 1}/{patrolPoints.Count}");
        }

        /// <summary>
        /// Opcional: Inverte o sprite baseado na direção do movimento.
        /// </summary>
        private void FlipSprite(float directionX)
        {
            if (directionX != 0)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = directionX < 0;
                }
            }
        }

        /// <summary>
        /// Retorna o ponto de patrulha atual.
        /// </summary>
        public Vector2 GetCurrentPatrolPoint()
        {
            if (patrolPoints != null && patrolPoints.Count > 0)
            {
                return patrolPoints[currentPointIndex];
            }
            return Vector2.zero;
        }

        /// <summary>
        /// Retorna o índice do ponto de patrulha atual.
        /// </summary>
        public int GetCurrentPointIndex()
        {
            return currentPointIndex;
        }

        /// <summary>
        /// Define um novo conjunto de pontos de patrulha.
        /// </summary>
        public void SetPatrolPoints(List<Vector2> newPoints)
        {
            if (newPoints != null && newPoints.Count >= 2)
            {
                patrolPoints = new List<Vector2>(newPoints);
                currentPointIndex = 0;
                isWaiting = false;
                Debug.Log($"🔄 {gameObject.name} recebeu novos pontos de patrulha: {patrolPoints.Count} pontos");
            }
            else
            {
                Debug.LogWarning($"⚠️ Tentativa de definir pontos de patrulha inválidos em '{gameObject.name}'");
            }
        }

        /// <summary>
        /// Desenha gizmos no editor para visualizar o caminho de patrulha.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (patrolPoints == null || patrolPoints.Count < 2)
            {
                return;
            }

            // Desenhar pontos de patrulha
            Gizmos.color = Color.cyan;
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                Gizmos.DrawSphere(patrolPoints[i], 0.3f);

                // Desenhar número do ponto
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(patrolPoints[i] + Vector2.up * 0.5f, $"P{i + 1}");
                #endif
            }

            // Desenhar linhas conectando os pontos
            Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
            for (int i = 0; i < patrolPoints.Count; i++)
            {
                int nextIndex = (i + 1) % patrolPoints.Count;
                Gizmos.DrawLine(patrolPoints[i], patrolPoints[nextIndex]);

                // Desenhar seta indicando direção
                Vector2 direction = (patrolPoints[nextIndex] - patrolPoints[i]).normalized;
                Vector2 midPoint = (patrolPoints[i] + patrolPoints[nextIndex]) / 2f;
                DrawArrow(midPoint, direction, 0.5f);
            }

            // Destacar ponto atual durante gameplay
            if (Application.isPlaying && currentPointIndex < patrolPoints.Count)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(patrolPoints[currentPointIndex], 0.4f);

                // Desenhar linha até o ponto atual
                Gizmos.DrawLine(transform.position, patrolPoints[currentPointIndex]);
            }
        }

        /// <summary>
        /// Desenha uma seta no gizmo para indicar direção.
        /// </summary>
        private void DrawArrow(Vector2 position, Vector2 direction, float size)
        {
            Vector2 right = new Vector2(-direction.y, direction.x);
            Vector2 arrowTip = position + direction * size * 0.5f;
            Vector2 arrowLeft = arrowTip - direction * size * 0.3f - right * size * 0.2f;
            Vector2 arrowRight = arrowTip - direction * size * 0.3f + right * size * 0.2f;

            Gizmos.DrawLine(arrowTip, arrowLeft);
            Gizmos.DrawLine(arrowTip, arrowRight);
        }
    }
}
