using UnityEngine;
using System.Collections;

namespace TheSlimeKing.Core.Inventory
{
    /// <summary>
    /// Representa um item físico no mundo de jogo que pode ser coletado
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ItemWorldDrop : MonoBehaviour
    {
        [Header("Configuração")]
        [SerializeField] private ItemData _itemData;
        [SerializeField] private int _quantity = 1;
        [SerializeField] private float _attractionRadius = 1.5f;
        [SerializeField] private float _attractionSpeed = 5f;
        [SerializeField] private float _rotationSpeed = 80f;
        [SerializeField] private float _bobAmount = 0.2f;
        [SerializeField] private float _bobSpeed = 2f;
        [SerializeField] private float _collectDelay = 0.5f; // Tempo antes de poder ser coletado

        [Header("Efeitos")]
        [SerializeField] private GameObject _collectEffectPrefab;
        [SerializeField] private AudioClip _attractionSound;
        [SerializeField] private AudioClip _collectSound;

        // Componentes
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        private Rigidbody2D _rigidbody;

        // Estado
        private bool _canBeCollected = false;
        private bool _isBeingAttracted = false;
        private Transform _playerTransform;
        private Vector3 _startPosition;
        private float _bobTimer;
        private float _spawnTime;

        private void Awake()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();

            // Configurar rigidbody se existir
            if (_rigidbody != null)
            {
                _rigidbody.gravityScale = 0;
                _rigidbody.linearDamping = 1f;
            }

            // Configurar collider como trigger
            if (_collider != null)
            {
                _collider.isTrigger = true;
            }
        }

        private void Start()
        {
            // Inicializa posição inicial para efeito de flutuação
            _startPosition = transform.position;
            _spawnTime = Time.time;

            // Configurar sprite e visual
            SetupVisuals();

            // Iniciar delay antes de poder ser coletado
            StartCoroutine(EnableCollection());
        }

        private void Update()
        {
            // Atualizar efeito de flutuação
            if (!_isBeingAttracted)
            {
                _bobTimer += Time.deltaTime * _bobSpeed;
                transform.position = _startPosition + new Vector3(0, Mathf.Sin(_bobTimer) * _bobAmount, 0);

                // Rotação contínua
                transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
            }

            // Verificar proximidade do jogador para atração
            CheckPlayerProximity();

            // Mover em direção ao jogador se atraído
            if (_isBeingAttracted && _playerTransform != null)
            {
                MoveTowardsPlayer();
            }
        }

        private void SetupVisuals()
        {
            // Configurar sprite baseado nos dados do item
            if (_itemData != null && _spriteRenderer != null)
            {
                _spriteRenderer.sprite = _itemData.Icon;

                // Configurações opcionais
                // _spriteRenderer.color = GetColorBasedOnRarity(_itemData.Rarity);
            }
        }

        /// <summary>
        /// Habilita a coleta após um delay para evitar coleta imediata após spawn
        /// </summary>
        private IEnumerator EnableCollection()
        {
            _canBeCollected = false;

            // Configurações visuais especiais durante o período de não-coleta
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0.7f);
            }

            yield return new WaitForSeconds(_collectDelay);

            _canBeCollected = true;

            // Restaurar visualização normal
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.white;
            }
        }

        /// <summary>
        /// Verifica proximidade do jogador para iniciar atração
        /// </summary>
        private void CheckPlayerProximity()
        {
            if (!_canBeCollected || _isBeingAttracted)
                return;

            // Encontrar o jogador (pode ser adaptado para usar tag específica ou layer)
            if (_playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _playerTransform = player.transform;
                }
            }

            // Verificar distância
            if (_playerTransform != null)
            {
                float distance = Vector2.Distance(transform.position, _playerTransform.position);

                if (distance <= _attractionRadius)
                {
                    // Iniciar atração
                    _isBeingAttracted = true;

                    // Reproduzir som de atração
                    if (_attractionSound != null)
                    {
                        AudioSource.PlayClipAtPoint(_attractionSound, transform.position, 0.6f);
                    }
                }
            }
        }

        /// <summary>
        /// Move o item em direção ao jogador quando atraído
        /// </summary>
        private void MoveTowardsPlayer()
        {
            // Calcular direção ao jogador
            Vector2 direction = (_playerTransform.position - transform.position).normalized;

            // Aumentar velocidade conforme se aproxima
            float distance = Vector2.Distance(transform.position, _playerTransform.position);
            float speedMultiplier = Mathf.Clamp(1 / distance, 0.5f, 3f);

            // Mover em direção ao jogador
            transform.position += (Vector3)direction * _attractionSpeed * speedMultiplier * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verificar se é o jogador
            if (!_canBeCollected || other.gameObject.CompareTag("Player") == false)
                return;

            // Tentar adicionar ao inventário
            if (InventoryManager.Instance != null)
            {
                bool addedSuccessfully = InventoryManager.Instance.AddItem(_itemData, _quantity);

                if (addedSuccessfully)
                {
                    // Reproduzir efeito de coleta
                    if (_collectEffectPrefab != null)
                    {
                        Instantiate(_collectEffectPrefab, transform.position, Quaternion.identity);
                    }

                    // Reproduzir som de coleta
                    if (_collectSound != null)
                    {
                        AudioSource.PlayClipAtPoint(_collectSound, transform.position);
                    }

                    // Destruir este objeto
                    Destroy(gameObject);
                }
                else
                {
                    // Inventário cheio ou não foi possível adicionar
                    // Poderia ter um feedback visual aqui
                    _isBeingAttracted = false;

                    // Permitir tentar novamente depois
                    StartCoroutine(RetryAttractionAfterDelay());
                }
            }
        }

        /// <summary>
        /// Tenta atração novamente após inventário cheio
        /// </summary>
        private IEnumerator RetryAttractionAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            _isBeingAttracted = false;
        }

        #region Public Methods

        /// <summary>
        /// Configura o item a ser dropado
        /// </summary>
        public void SetItem(ItemData itemData, int quantity = 1)
        {
            _itemData = itemData;
            _quantity = Mathf.Max(1, quantity);

            if (_spriteRenderer != null && itemData != null)
            {
                _spriteRenderer.sprite = itemData.Icon;
            }
        }

        /// <summary>
        /// Aplica uma força inicial ao item (para efeito de "explosão" em drops)
        /// </summary>
        public void ApplyInitialForce(Vector2 direction, float force)
        {
            if (_rigidbody != null)
            {
                // Normalizar direção e aplicar força
                Vector2 normalizedDir = direction.normalized;
                _rigidbody.AddForce(normalizedDir * force, ForceMode2D.Impulse);

                // Temporariamente desativar atração
                StartCoroutine(DelayAttractionAfterForce());
            }
        }

        /// <summary>
        /// Desativa atração temporariamente após aplicar força
        /// </summary>
        private IEnumerator DelayAttractionAfterForce()
        {
            _canBeCollected = false;

            // Esperar um pouco para o item se mover
            yield return new WaitForSeconds(0.5f);

            // Atualizar posição inicial para o bob
            _startPosition = transform.position;

            // Permitir coleta
            _canBeCollected = true;
        }

        #endregion
    }
}
