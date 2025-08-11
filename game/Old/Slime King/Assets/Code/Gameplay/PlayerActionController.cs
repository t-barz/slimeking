using UnityEngine;
using System.Collections;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia as ações especiais do jogador (pulo e deslize).
    /// Responsável por controlar as animações e movimentações especiais.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerAudioManager))]
    public class PlayerActionController : MonoBehaviour
    {
        #region Campos Serializados
        [Header("Movement Settings")]
        [SerializeField] private float jumpHeight = 2f;
        #endregion

        #region Campos Privados
        private Animator animator;
        private PlayerAudioManager audioManager;
        private Collider2D[] playerColliders;

        private bool isJumping;
        private bool isSliding;
        #endregion

        #region Propriedades Públicas
        public bool IsJumping => isJumping;
        public bool IsSliding => isSliding;
        #endregion

        #region Métodos Unity
        private void Awake()
        {
            animator = GetComponent<Animator>();
            audioManager = GetComponent<PlayerAudioManager>();
            playerColliders = GetComponents<Collider2D>();
        }
        #endregion

        #region Sistema de Movimento
        public void Jump(Vector3 destination)
        {
            if (!isJumping)
            {
                audioManager.PlayJumpSound();
                StartCoroutine(JumpCoroutine(destination));
            }
        }

        public void Slide(Vector3 destination)
        {
            if (!isSliding)
            {
                audioManager.PlaySlideSound();
                StartCoroutine(SlideCoroutine(destination));
            }
        }

        private IEnumerator JumpCoroutine(Vector3 destination)
        {
            isJumping = true;
            const float MOVE_DURATION = 0.5f;

            animator.SetTrigger("Jump");
            Vector3 startPosition = transform.position;
            float startTime = Time.time;
            float totalDistance = Vector3.Distance(startPosition, destination);

            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                   !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                float elapsedTime = Time.time - startTime;
                float remainingTime = Mathf.Max(MOVE_DURATION - elapsedTime, Time.deltaTime);
                Vector3 currentPosition = transform.position;
                Vector3 toDestination = destination - currentPosition;

                if (toDestination.magnitude > 0.01f)
                {
                    float speed = toDestination.magnitude / remainingTime;
                    float progress = 1f - (toDestination.magnitude / totalDistance);
                    float verticalOffset = jumpHeight * Mathf.Sin(progress * Mathf.PI);

                    Vector3 targetPosition = Vector3.MoveTowards(
                        currentPosition,
                        destination,
                        speed * Time.deltaTime
                    );
                    targetPosition.y += verticalOffset;

                    transform.position = targetPosition;
                }

                yield return null;
            }

            transform.position = destination;
            isJumping = false;
        }

        private IEnumerator SlideCoroutine(Vector3 destination)
        {
            isSliding = true;
            const float MOVE_DURATION = 0.75f;
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            foreach (var collider in playerColliders)
            {
                collider.enabled = false;
            }

            animator.SetTrigger("Shrink");

            while (elapsedTime < MOVE_DURATION)
            {
                float t = elapsedTime / MOVE_DURATION;
                transform.position = Vector3.Lerp(startPosition, destination, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            foreach (var collider in playerColliders)
            {
                collider.enabled = true;
            }

            isSliding = false;
        }
        #endregion
    }
}
