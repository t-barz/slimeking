using UnityEngine;

namespace TheSlimeKing.Core.Elemental
{
    /// <summary>
    /// Representa um fragmento de energia elemental que pode ser coletado pelo jogador
    /// </summary>
    public class ElementalFragment : MonoBehaviour
    {
        [Header("Configurações")]
        [SerializeField] private ElementalType _elementType = ElementalType.None;
        [SerializeField] private FragmentSize _size = FragmentSize.Small;
        [SerializeField] private float _floatAmplitude = 0.2f;
        [SerializeField] private float _floatSpeed = 1.5f;
        [SerializeField] private float _rotateSpeed = 45f;
        [SerializeField] private float _collectRadius = 2.0f;
        [SerializeField] private float _magnetismStrength = 5.0f;
        [SerializeField] private float _lifetimeSeconds = 20.0f;
        [SerializeField] private bool _isBeingCollected = false;

        [Header("Referências")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private GameObject _collectEffect;
        [SerializeField] private AudioClip _collectSound;

        // Cache
        private Transform _playerTransform;
        private Vector3 _startPosition;
        private Color _primaryColor;
        private Color _secondaryColor;
        private float _lifeTimer;

        // Propriedades públicas
        public ElementalType ElementType => _elementType;
        public int EnergyValue => GetEnergyValue();
        public bool IsBeingCollected => _isBeingCollected;

        /// <summary>
        /// Enum que define o tamanho do fragmento e sua raridade/valor
        /// </summary>
        public enum FragmentSize
        {
            Small,  // Comum, 1 ponto
            Medium, // Médio, 3 pontos
            Large   // Raro, 7 pontos
        }

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            _startPosition = transform.position;
            _lifeTimer = _lifetimeSeconds;
        }

        private void Start()
        {
            // Busca o player
            _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

            // Aplica cor baseada no tipo elemental
            ApplyElementalColor();

            // Escala o objeto baseado no tamanho do fragmento
            ApplyScaleBasedOnSize();
        }

        private void Update()
        {
            // Animação de flutuação
            if (!_isBeingCollected)
            {
                // Aplicar movimento de flutuação
                float newY = _startPosition.y + Mathf.Sin(Time.time * _floatSpeed) * _floatAmplitude;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);

                // Aplicar rotação
                transform.Rotate(0, 0, _rotateSpeed * Time.deltaTime);

                // Timer de vida
                _lifeTimer -= Time.deltaTime;
                if (_lifeTimer <= 0)
                {
                    // Fade out antes de destruir
                    StartFadeOut();
                }
            }
            else
            {
                // Se está sendo coletado, move em direção ao player
                MoveTowardsPlayer();
            }

            // Se o jogador entrou em range, ativa a coleta
            if (_playerTransform != null && !_isBeingCollected)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
                if (distanceToPlayer <= _collectRadius)
                {
                    StartCollection();
                }
            }
        }

        /// <summary>
        /// Move o fragmento em direção ao player quando está sendo coletado
        /// </summary>
        private void MoveTowardsPlayer()
        {
            if (_playerTransform == null)
                return;

            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // Aumenta velocidade conforme se aproxima do player
            float speedMultiplier = Mathf.Max(1, (2.0f / distanceToPlayer));

            // Move para o player
            transform.position += direction * _magnetismStrength * speedMultiplier * Time.deltaTime;

            // Verifica se chegou no player
            if (distanceToPlayer < 0.5f)
            {
                CollectFragment();
            }
        }

        /// <summary>
        /// Configura o fragmento com características elementais específicas
        /// </summary>
        public void Initialize(ElementalType type, FragmentSize size, Color primaryColor, Color secondaryColor)
        {
            _elementType = type;
            _size = size;
            _primaryColor = primaryColor;
            _secondaryColor = secondaryColor;

            // Aplica configurações visuais
            ApplyElementalColor();
            ApplyScaleBasedOnSize();
        }

        /// <summary>
        /// Aplica cores no sprite baseado no tipo elemental
        /// </summary>
        private void ApplyElementalColor()
        {
            if (_spriteRenderer == null)
                return;

            // Se as cores não foram setadas, usa cores padrão do tipo elemental
            if (_primaryColor == Color.clear || _secondaryColor == Color.clear)
            {
                switch (_elementType)
                {
                    case ElementalType.Earth:
                        _primaryColor = HexToColor("#8B4513");  // Marrom
                        _secondaryColor = HexToColor("#DEB887"); // Marrom claro
                        break;

                    case ElementalType.Water:
                        _primaryColor = HexToColor("#4169E1");  // Azul
                        _secondaryColor = HexToColor("#87CEEB"); // Azul claro
                        break;

                    case ElementalType.Fire:
                        _primaryColor = HexToColor("#FF4500");  // Vermelho/Laranja
                        _secondaryColor = HexToColor("#FFA500"); // Laranja
                        break;

                    case ElementalType.Air:
                        _primaryColor = HexToColor("#E6E6FA");  // Lavanda bem claro
                        _secondaryColor = HexToColor("#F0F8FF"); // Azul muito claro
                        break;

                    default:
                        _primaryColor = Color.gray;
                        _secondaryColor = Color.white;
                        break;
                }
            }

            // Aplica cor com interpolação aleatória entre primária e secundária
            float blendFactor = Random.Range(0.0f, 1.0f);
            _spriteRenderer.color = Color.Lerp(_primaryColor, _secondaryColor, blendFactor);
        }

        /// <summary>
        /// Aplica escala baseada no tamanho do fragmento
        /// </summary>
        private void ApplyScaleBasedOnSize()
        {
            // Ajusta escala baseado no tamanho do fragmento
            float scale = 1.0f;

            switch (_size)
            {
                case FragmentSize.Small:
                    scale = 0.8f;
                    break;
                case FragmentSize.Medium:
                    scale = 1.2f;
                    break;
                case FragmentSize.Large:
                    scale = 1.6f;
                    break;
            }

            transform.localScale = new Vector3(scale, scale, 1.0f);
        }

        /// <summary>
        /// Retorna o valor de energia baseado no tamanho
        /// </summary>
        private int GetEnergyValue()
        {
            switch (_size)
            {
                case FragmentSize.Small:
                    return 1;
                case FragmentSize.Medium:
                    return 3;
                case FragmentSize.Large:
                    return 7;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Inicia o processo de coleta (atração para o player)
        /// </summary>
        private void StartCollection()
        {
            _isBeingCollected = true;
        }

        /// <summary>
        /// Realiza a coleta efetiva do fragmento
        /// </summary>
        private void CollectFragment()
        {
            // Evita coleta duplicada
            if (gameObject == null)
                return;

            // Dispara evento de absorção
            ElementalEvents.OnFragmentAbsorbed?.Invoke(_elementType, EnergyValue, transform.position);

            // Efeitos de coleta
            if (_collectEffect != null)
            {
                GameObject effect = Instantiate(_collectEffect, transform.position, Quaternion.identity);
                Destroy(effect, 2.0f);
            }

            // Som de coleta
            if (_collectSound != null)
            {
                AudioSource.PlayClipAtPoint(_collectSound, transform.position, 0.7f);
            }

            // Destrói o fragmento
            Destroy(gameObject);
        }

        /// <summary>
        /// Inicia o fade out do objeto quando seu tempo de vida acaba
        /// </summary>
        private void StartFadeOut()
        {
            // Implementação básica - em produção deveria usar uma coroutine para fade gradual
            Destroy(gameObject, 1.0f);
        }

        /// <summary>
        /// Converte cor em formato hex para Color
        /// </summary>
        private Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
                return color;

            return Color.white;
        }

        /// <summary>
        /// Desenha o raio de coleta em modo debug
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _collectRadius);
        }
    }
}
