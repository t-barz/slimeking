using UnityEngine;
using System.Collections;
using TheSlimeKing.Gameplay;

namespace TheSlimeKing.Gameplay.Movement
{
    /// <summary>
    /// Controla o movimento especial de encolher e esgueirar
    /// </summary>
    public class SlideController : MonoBehaviour, TheSlimeKing.Gameplay.IInteractable
    {
        [Header("Configurações")]
        [SerializeField] private Transform exitPoint;
        [SerializeField] private float movementDuration = 2f;
        [SerializeField] private float shrinkScale = 0.5f;
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Feedback")]
        [SerializeField] private AudioClip shrinkSound;
        [SerializeField] private AudioClip expandSound;
        [SerializeField] private GameObject interactionIcon;

        [Header("Requisitos")]
        [SerializeField] private float maxInteractionDistance = 1.5f;
        [SerializeField] private float maxInteractionAngle = 30f;
        [SerializeField] private string interactionPrompt = "Pressione E para esgueirar";
        [SerializeField] private int interactionPriority = 10;

        // Referências
        private GameObject _player;
        private IPlayerController _playerController;
        private AudioSource _audioSource;
        private SlimeAnimationController _animationController;

        // Estado
        private bool _isActive = false;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 1f;
            }

            // Certifique-se de que existe um ponto de saída
            if (exitPoint == null)
            {
                exitPoint = transform;
                Debug.LogWarning("Ponto de saída não configurado para EncolherEsgueirarController. Usando a posição do próprio objeto.");
            }
        }

        private void Start()

        {
            // Configura o ícone de interação
            if (interactionIcon != null)
            {
                interactionIcon.SetActive(false);
            }
        }

        #region IInteractable Implementation

        public void Interact(GameObject interactor)
        {
            if (_isActive)
                return;

            _player = interactor;
            _playerController = _player.GetComponent<IPlayerController>();
            _animationController = _player.GetComponent<SlimeAnimationController>();

            if (_playerController == null)
            {
                Debug.LogError("Jogador não implementa a interface IPlayerController");
                return;
            }

            // Dispara a trigger de Shrink no Animator do Player
            if (_animationController != null)
            {
                _animationController.PlayShrinkAnimation();
            }
            else
            {
                Debug.LogWarning("SlimeAnimationController não encontrado no jogador");
            }

            StartCoroutine(EncolherEsgueirarSequence());
        }

        public bool CanInteract()
        {
            if (_isActive)
                return false;

            // O SlimeInteractionController verifica a distância e ângulo por nós
            return true;
        }

        public void ShowInteractionPrompt()
        {
            if (interactionIcon != null)
                interactionIcon.SetActive(true);
        }

        public void HideInteractionPrompt()
        {
            if (interactionIcon != null)
                interactionIcon.SetActive(false);
        }

        public int GetInteractionPriority()
        {
            return interactionPriority;
        }

        public InteractionType GetInteractionType()
        {
            return InteractionType.Shrink;
        }

        #endregion

        private IEnumerator EncolherEsgueirarSequence()
        {
            _isActive = true;

            // 1. Desabilitar controles do jogador
            _playerController.DisableControl();            // 2. Reproduzir som de encolhimento
            if (shrinkSound != null && _audioSource != null)
                _audioSource.PlayOneShot(shrinkSound);

            yield return new WaitForSeconds(0.5f);

            // 4. Encolher o jogador
            float shrinkTime = 0.5f;
            float elapsed = 0;
            while (elapsed < shrinkTime)
            {
                float scale = Mathf.Lerp(1f, shrinkScale, elapsed / shrinkTime);
                _playerController.SetScale(scale);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _playerController.SetScale(shrinkScale);

            yield return new WaitForSeconds(0.2f);            // 5. Mover o jogador pelo caminho a partir da sua posição atual
            Vector3 startPosition = _player.transform.position;
            Vector3 targetPosition = exitPoint.position;

            elapsed = 0;
            while (elapsed < movementDuration)
            {
                float normalizedTime = elapsed / movementDuration;
                float curveValue = movementCurve.Evaluate(normalizedTime);
                Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);

                _playerController.MoveToPosition(newPosition);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _playerController.MoveToPosition(targetPosition);            // 6. Expandir o jogador de volta ao tamanho normal
            if (expandSound != null && _audioSource != null)
                _audioSource.PlayOneShot(expandSound);

            float expandTime = 0.5f;
            elapsed = 0;
            while (elapsed < expandTime)
            {
                float scale = Mathf.Lerp(shrinkScale, 1f, elapsed / expandTime);
                _playerController.SetScale(scale);
                elapsed += Time.deltaTime;
                yield return null;
            }
            _playerController.SetScale(1f);

            yield return new WaitForSeconds(0.5f);

            // 7. Retornar controle ao jogador
            _playerController.EnableControl();

            _isActive = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // Desenha apenas o ponto de saída e a área de interação
            if (exitPoint != null)
            {
                // Desenha área de interação
                Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
                Gizmos.DrawSphere(transform.position, maxInteractionDistance);

                // Desenha ponto de saída
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(exitPoint.position, 0.3f);

                // Desenha seta indicativa na direção do movimento
                Gizmos.color = Color.yellow;
                Vector3 direction = (exitPoint.position - transform.position).normalized;
                float arrowLength = Vector3.Distance(transform.position, exitPoint.position) * 0.8f;
                Gizmos.DrawRay(transform.position, direction * arrowLength);
            }
        }
#endif
    }
}
