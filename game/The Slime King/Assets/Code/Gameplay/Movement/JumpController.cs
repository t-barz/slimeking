using UnityEngine;
using System.Collections;
using TheSlimeKing.Gameplay;

namespace TheSlimeKing.Gameplay.Movement
{
    /// <summary>
    /// Controla o movimento especial de pulo
    /// </summary>
    public class JumpController : MonoBehaviour, TheSlimeKing.Gameplay.IInteractable
    {
        [Header("Configurações de Pulo")]
        [SerializeField] private Transform landingPoint;
        [SerializeField] private float jumpHeight = 3f;
        [SerializeField] private float jumpDuration = 1f;
        [SerializeField] private AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve heightCurve;  // Curva parabólica para altura do pulo

        [Header("Feedback")]
        [SerializeField] private ParticleSystem takeoffEffect;
        [SerializeField] private ParticleSystem landingEffect;
        [SerializeField] private AudioClip takeoffSound;
        [SerializeField] private AudioClip landingSound;
        [SerializeField] private GameObject interactionIcon;

        [Header("Requisitos")]
        [SerializeField] private float maxInteractionDistance = 1.5f;
        [SerializeField] private float maxInteractionAngle = 30f;
        [SerializeField] private string interactionPrompt = "Pressione E para pular";
        [SerializeField] private int interactionPriority = 10;
        [SerializeField] private LayerMask obstacleLayerMask;  // Camadas que bloqueiam o pulo

        // Referências
        private GameObject _player;
        private IPlayerController _playerController;
        private AudioSource _audioSource;

        // Estado
        private bool _isActive = false;

        private void Awake()
        {
            // Inicializa curva de altura parabólica se não for definida
            if (heightCurve == null || heightCurve.keys.Length == 0)
            {
                heightCurve = new AnimationCurve();
                heightCurve.AddKey(0, 0);
                heightCurve.AddKey(0.5f, 1);
                heightCurve.AddKey(1, 0);
            }

            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 1f;
            }

            // Certifique-se de que existe um ponto de pouso
            if (landingPoint == null)
            {
                landingPoint = transform;
                Debug.LogError("Ponto de pouso não configurado para JumpController. Isso causará comportamentos imprevisíveis!");
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

            if (_playerController == null)
            {
                Debug.LogError("Jogador não implementa a interface IPlayerController");
                return;
            }

            // Verifica se o caminho de pulo está livre
            if (!IsJumpPathClear())
            {
                Debug.Log("Caminho de pulo bloqueado!");
                return;
            }

            StartCoroutine(JumpSequence());
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
            return InteractionType.Jump;
        }

        #endregion

        private bool IsJumpPathClear()
        {
            // Verifica se há obstáculos no caminho do pulo
            Vector2 startPoint = transform.position;
            Vector2 endPoint = landingPoint.position;
            Vector2 direction = (endPoint - startPoint).normalized;
            float distance = Vector2.Distance(startPoint, endPoint);

            // Traça um arco com vários raycasts para simular a trajetória do pulo
            int steps = 10;
            for (int i = 0; i < steps; i++)
            {
                float t = i / (float)(steps - 1);
                Vector2 point = Vector2.Lerp(startPoint, endPoint, t);

                // Adiciona altura ao longo do arco
                point.y += heightCurve.Evaluate(t) * jumpHeight;

                // Verifica colisão no ponto atual
                if (Physics2D.OverlapCircle(point, 0.5f, obstacleLayerMask))
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerator JumpSequence()
        {
            _isActive = true;

            // 1. Desabilitar controles do jogador
            _playerController.DisableControl();

            // 2. Preparação para o pulo
            _playerController.MoveToPosition(transform.position);
            _playerController.SetDirection((landingPoint.position - transform.position).normalized);

            yield return new WaitForSeconds(0.3f);

            // 3. Efeito de decolagem
            if (takeoffEffect != null)
                takeoffEffect.Play();

            if (takeoffSound != null && _audioSource != null)
                _audioSource.PlayOneShot(takeoffSound);

            yield return new WaitForSeconds(0.2f);

            // 4. Realizar o pulo em arco
            Vector2 startPoint = transform.position;
            Vector2 endPoint = landingPoint.position;

            float elapsed = 0;
            while (elapsed < jumpDuration)
            {
                float t = elapsed / jumpDuration;
                float curveValue = jumpCurve.Evaluate(t);

                // Interpola a posição horizontal
                Vector2 newPosition = Vector2.Lerp(startPoint, endPoint, curveValue);

                // Adiciona a altura usando a curva parabólica
                newPosition.y += heightCurve.Evaluate(curveValue) * jumpHeight;

                _playerController.MoveToPosition(newPosition);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // 5. Garante que o jogador chegue exatamente ao ponto de pouso
            _playerController.MoveToPosition(endPoint);

            // 6. Efeitos de aterrissagem
            if (landingEffect != null)
                landingEffect.Play();

            if (landingSound != null && _audioSource != null)
                _audioSource.PlayOneShot(landingSound);

            yield return new WaitForSeconds(0.5f);

            // 7. Retornar controle ao jogador
            _playerController.EnableControl();

            _isActive = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (landingPoint != null)
            {
                // Desenha linha direta entre pontos
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, landingPoint.position);

                // Desenha trajetória do pulo
                Gizmos.color = Color.yellow;
                Vector3 lastPoint = transform.position;

                int steps = 20;
                for (int i = 1; i <= steps; i++)
                {
                    float t = i / (float)steps;
                    Vector3 horizontalPos = Vector3.Lerp(transform.position, landingPoint.position, t);

                    // Aplica curva de altura
                    float height = heightCurve != null && heightCurve.keys.Length > 0
                        ? heightCurve.Evaluate(t) * jumpHeight
                        : Mathf.Sin(t * Mathf.PI) * jumpHeight;

                    Vector3 curPoint = horizontalPos + new Vector3(0, height, 0);
                    Gizmos.DrawLine(lastPoint, curPoint);
                    lastPoint = curPoint;
                }

                // Desenha área de interação
                Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
                Gizmos.DrawSphere(transform.position, maxInteractionDistance);

                // Desenha ponto de pouso
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(landingPoint.position, 0.3f);
            }
        }
#endif
    }
}
