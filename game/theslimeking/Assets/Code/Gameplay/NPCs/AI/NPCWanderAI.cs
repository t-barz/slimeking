using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// IA de vagueio para NPCs que se movem aleatoriamente dentro de um raio.
    /// O NPC escolhe um ponto aleatório, move-se até ele, pausa, e repete.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class NPCWanderAI : MonoBehaviour
    {
        [Header("Wander Configuration")]
        [Tooltip("Raio de vagueio a partir da posição inicial (metros)")]
        public float wanderRadius = 5.0f;

        [Tooltip("Velocidade de movimento durante o vagueio (m/s)")]
        public float wanderSpeed = 2.0f;

        [Tooltip("Duração da pausa entre movimentos (segundos)")]
        public float pauseDuration = 2.0f;

        [Header("State")]
        [Tooltip("Posição inicial do NPC (centro do raio de vagueio)")]
        private Vector2 startPosition;

        [Tooltip("Ponto de destino atual")]
        private Vector2 targetPosition;

        [Tooltip("Indica se o NPC está atualmente pausado")]
        private bool isPaused = false;

        [Tooltip("Tempo restante da pausa atual")]
        private float pauseTimer = 0f;

        [Tooltip("Distância mínima para considerar que chegou ao destino")]
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
            // Armazenar posição inicial como centro do raio de vagueio
            startPosition = transform.position;

            // Escolher primeiro ponto de destino
            ChooseNewWanderPoint();
        }

        private void Update()
        {
            if (isPaused)
            {
                HandlePause();
            }
            else
            {
                MoveTowardsTarget();
            }
        }

        /// <summary>
        /// Move o NPC em direção ao ponto de destino atual.
        /// </summary>
        private void MoveTowardsTarget()
        {
            Vector2 currentPosition = transform.position;
            float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);

            // Verificar se chegou ao destino
            if (distanceToTarget <= ARRIVAL_THRESHOLD)
            {
                StartPause();
                return;
            }

            // Calcular direção e mover
            Vector2 direction = (targetPosition - currentPosition).normalized;
            Vector2 newPosition = currentPosition + direction * wanderSpeed * Time.deltaTime;

            // Aplicar movimento
            rb.MovePosition(newPosition);

            // Atualizar animação
            if (animator != null)
            {
                animator.SetFloat("Speed", wanderSpeed);
            }

            // Opcional: Rotacionar sprite baseado na direção
            // FlipSprite(direction.x);
        }

        /// <summary>
        /// Gerencia o estado de pausa do NPC.
        /// </summary>
        private void HandlePause()
        {
            pauseTimer -= Time.deltaTime;

            if (pauseTimer <= 0f)
            {
                EndPause();
            }
        }

        /// <summary>
        /// Inicia uma pausa no movimento.
        /// </summary>
        private void StartPause()
        {
            isPaused = true;
            pauseTimer = pauseDuration;

            // Parar animação de movimento
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }

        /// <summary>
        /// Encerra a pausa e escolhe novo ponto de destino.
        /// </summary>
        private void EndPause()
        {
            isPaused = false;
            ChooseNewWanderPoint();
        }

        /// <summary>
        /// Escolhe um novo ponto aleatório dentro do raio de vagueio.
        /// </summary>
        private void ChooseNewWanderPoint()
        {
            // Gerar ponto aleatório dentro do círculo
            Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
            targetPosition = startPosition + randomDirection;

            Debug.Log($"🎯 {gameObject.name} escolheu novo ponto de vagueio: {targetPosition}");
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
        /// Desenha gizmos no editor para visualizar o raio de vagueio.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Vector2 center = Application.isPlaying ? startPosition : (Vector2)transform.position;

            // Desenhar raio de vagueio
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(center, wanderRadius);

            // Desenhar ponto de destino atual (apenas durante gameplay)
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(targetPosition, 0.2f);

                // Desenhar linha até o destino
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetPosition);
            }
        }
    }
}
