using UnityEngine;
using UnityEngine.Events;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Classe base para todos os objetos com os quais o jogador pode interagir
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        [Header("Configurações Básicas")]
        [SerializeField] private InteractionType _interactionType = InteractionType.Activate;
        [SerializeField] private string _interactionPrompt = "Pressione E para interagir";
        [SerializeField] private bool _isInteractable = true;
        [SerializeField] protected float _interactionRadius = 1.5f;
        [SerializeField] private bool _requireFacing = true;
        [SerializeField] private float _maxInteractionAngle = 45f;

        [Header("Feedback Visual")]
        [SerializeField] private bool _useOutline = true;
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField] private bool _useIcon = true;
        [SerializeField] private Sprite _interactionIcon;

        [Header("Feedback Sonoro")]
        [SerializeField] private AudioClip _interactSound;
        [SerializeField] private float _interactSoundVolume = 0.7f;

        [Header("Eventos")]
        [SerializeField] private UnityEvent<GameObject> _onInteract;
        [SerializeField] private UnityEvent<GameObject> _onInteractionStart;
        [SerializeField] private UnityEvent<GameObject> _onInteractionEnd;

        // Componentes
        protected Collider2D _collider;
        protected OutlineVisualEffect _outlineEffect;

        // Estado
        protected bool _isPlayerNearby = false;

        protected virtual void Awake()
        {
            _collider = GetComponent<Collider2D>();

            if (_useOutline)
            {
                // Adiciona o efeito de outline se ainda não tiver
                _outlineEffect = GetComponent<OutlineVisualEffect>();
                if (_outlineEffect == null)
                {
                    _outlineEffect = gameObject.AddComponent<OutlineVisualEffect>();
                }

                // Configura as propriedades
                _outlineEffect.SetColor(_outlineColor);
                _outlineEffect.SetEnabled(false);  // Começa desligado até o jogador se aproximar
            }

            // Garante que collider seja um trigger
            if (_collider != null && !_collider.isTrigger)
            {
                _collider.isTrigger = true;
            }
        }

        #region IInteractable Implementation

        /// <summary>
        /// Executa a interação com este objeto
        /// </summary>
        public virtual void Interact(GameObject interactor)
        {
            if (!_isInteractable)
                return;

            // Toca som de interação
            if (_interactSound != null)
            {
                AudioSource.PlayClipAtPoint(_interactSound, transform.position, _interactSoundVolume);
            }

            // Invoca eventos
            _onInteract?.Invoke(interactor);

            Debug.Log($"Interação executada com: {gameObject.name}");
        }

        /// <summary>
        /// Verifica se a interação é possível neste momento
        /// </summary>
        public virtual bool CanInteract(GameObject interactor)
        {
            if (!_isInteractable)
                return false;

            // Verifica distância
            float distance = Vector3.Distance(transform.position, interactor.transform.position);
            if (distance > _interactionRadius)
                return false;

            // Verifica se o jogador está olhando para o objeto (opcional)
            if (_requireFacing)
            {
                Vector3 directionToObject = (transform.position - interactor.transform.position).normalized;
                Vector3 interactorForward = interactor.transform.right * Mathf.Sign(interactor.transform.localScale.x);

                float angle = Vector3.Angle(interactorForward, directionToObject);
                if (angle > _maxInteractionAngle)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retorna o prompt de interação
        /// </summary>
        public virtual string GetInteractionPrompt()
        {
            return _interactionPrompt;
        }

        /// <summary>
        /// Retorna o tipo de interação deste objeto
        /// </summary>
        public InteractionType GetInteractionType()
        {
            return _interactionType;
        }

        /// <summary>
        /// Retorna o Transform deste objeto
        /// </summary>
        public Transform GetTransform()
        {
            return transform;
        }

        #endregion

        #region Trigger Events

        /// <summary>
        /// Detecta quando o jogador se aproxima do objeto
        /// </summary>
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = true;

                if (_useOutline && _outlineEffect != null)
                {
                    _outlineEffect.SetEnabled(true);
                    _outlineEffect.FadeIn();
                }

                _onInteractionStart?.Invoke(other.gameObject);
            }
        }

        /// <summary>
        /// Detecta quando o jogador se afasta do objeto
        /// </summary>
        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerNearby = false;

                if (_useOutline && _outlineEffect != null)
                {
                    _outlineEffect.FadeOut();
                }

                _onInteractionEnd?.Invoke(other.gameObject);
            }
        }

        #endregion

        /// <summary>
        /// Define se o objeto pode ser interagido
        /// </summary>
        public virtual void SetInteractable(bool interactable)
        {
            _isInteractable = interactable;
        }

        /// <summary>
        /// Retorna se o objeto está próximo do jogador
        /// </summary>
        public bool IsPlayerNearby()
        {
            return _isPlayerNearby;
        }

        /// <summary>
        /// Desenha o raio de interação no editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _interactionRadius);

            if (_requireFacing)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position, Vector3.right * _interactionRadius * 0.5f);

                // Desenha os limites do ângulo de interação
                float radianAngle = _maxInteractionAngle * Mathf.Deg2Rad;
                Vector3 rightBound = new Vector3(
                    Mathf.Cos(radianAngle) * _interactionRadius * 0.5f,
                    Mathf.Sin(radianAngle) * _interactionRadius * 0.5f,
                    0
                );
                Vector3 leftBound = new Vector3(
                    Mathf.Cos(-radianAngle) * _interactionRadius * 0.5f,
                    Mathf.Sin(-radianAngle) * _interactionRadius * 0.5f,
                    0
                );

                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(transform.position, rightBound);
                Gizmos.DrawRay(transform.position, leftBound);
            }
        }

        /// <summary>
        /// Define o prompt de interação
        /// </summary>
        public void SetInteractionPrompt(string prompt)
        {
            _interactionPrompt = prompt;
        }
    }
}
