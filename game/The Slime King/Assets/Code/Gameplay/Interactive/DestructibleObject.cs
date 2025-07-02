using TheSlimeKing.Core;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Define um item que pode ser dropado por um objeto destrutível
    /// </summary>
    [System.Serializable]
    public class DropItem
    {
        [Tooltip("Prefab do item que será instanciado")]
        public GameObject itemPrefab;

        [Tooltip("Nome personalizado do item (se vazio, usará o nome do prefab)")]
        public string itemName;
    }

    /// <summary>
    /// Objetos destrutíveis que podem receber dano e mudar de estado visual.
    /// Também pode dropar itens quando destruído.
    /// </summary>

    /// <summary>
    /// Objetos destrutíveis que podem receber dano e mudar de estado visual
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DestructibleObject : MonoBehaviour, IDamageable
    {
        [Header("Configurações de Vida")]
        [Tooltip("Quantidade máxima de pontos de vida do objeto")]
        [SerializeField] private int _maxHealth = 10;

        [Tooltip("Redução de dano aplicada a cada ataque")]
        [SerializeField] private int _defense = 0;

        [Header("Efeitos")]
        [Tooltip("Cor do flash quando o objeto recebe dano")]
        [SerializeField] private Color _hitFlashColor = Color.red;

        [Tooltip("Duração do efeito de flash em segundos")]
        [SerializeField] private float _flashDuration = 0.15f;

        [Header("Tag e Layer")]
        [Tooltip("Lembrete: Certifique-se que o objeto tem a tag 'Destructible'")]
        [SerializeField] private bool _checkTagOnStart = true;

        [Header("Efeitos de Destruição")]
        [Tooltip("Duração do efeito de fade-out em segundos")]
        [SerializeField] private float _fadeOutDuration = 1.5f;

        [Tooltip("Curva de animação para o efeito de fade-out")]
        [SerializeField] private AnimationCurve _fadeOutCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("Sistema de Drop")]
        [Tooltip("Lista de itens que podem ser dropados")]
        [SerializeField] private List<DropItem> _dropItems = new List<DropItem>();

        [Tooltip("Quantidade mínima de itens que serão dropados (0 = sem drop)")]
        [SerializeField] private int _minDropCount = 0;

        [Tooltip("Quantidade máxima de itens que serão dropados")]
        [SerializeField] private int _maxDropCount = 1;

        [Tooltip("Distância máxima de dispersão dos itens dropados")]
        [SerializeField] private float _dropRadius = 0.8f;

        // Estado
        private int _currentHealth;
        private bool _isCracked = false;
        private bool _isDestroyed = false;

        // Componentes
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;

        // Cache de rigidbodies para os itens dropados
        private static Dictionary<GameObject, Rigidbody2D> _cachedRigidbodies = new Dictionary<GameObject, Rigidbody2D>();

        // Hashes do animator para melhor performance
        private static readonly int IsCrackedHash = Animator.StringToHash("isCracked");
        private static readonly int IsDestroyedHash = Animator.StringToHash("isDestroyed");

        private void Awake()
        {
            // Inicializa a vida atual com o valor máximo
            _currentHealth = _maxHealth;

            // Obtém os componentes necessários
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // Armazena a cor original para restaurar após o flash
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
        }

        private void Start()
        {
            // Verifica se a tag está configurada corretamente
            if (_checkTagOnStart && !CompareTag("Destructible"))
            {
                Debug.LogWarning($"O objeto {gameObject.name} tem o componente DestructibleObject mas não está com a tag 'Destructible'");
            }
        }

        /// <summary>
        /// Aplica efeito visual de flash quando o objeto recebe dano
        /// </summary>
        private void FlashEffect()
        {
            if (_spriteRenderer != null)
            {
                // Usa Coroutine para retornar à cor original após o flash
                StartCoroutine(FlashCoroutine());
            }
        }

        /// <summary>
        /// Coroutine para aplicar o efeito de flash temporário
        /// </summary>
        private System.Collections.IEnumerator FlashCoroutine()
        {
            // Muda para a cor de dano
            _spriteRenderer.color = _hitFlashColor;

            // Aguarda o tempo definido
            yield return new WaitForSeconds(_flashDuration);

            // Retorna à cor original
            _spriteRenderer.color = _originalColor;
        }

        /// <summary>
        /// Desativa todos os colliders do objeto após a destruição
        /// </summary>
        private void DisableColliders()
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
        }

        /// <summary>
        /// Retorna a saúde atual do objeto
        /// </summary>
        public int GetCurrentHealth()
        {
            return _currentHealth;
        }

        /// <summary>
        /// Retorna a saúde máxima do objeto
        /// </summary>
        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        /// <summary>
        /// Verifica se o objeto já está destruído
        /// </summary>
        public bool IsDestroyed()
        {
            return _isDestroyed;
        }

        /// <summary>
        /// Implementação da interface IDamageable - verifica se o objeto está "morto" (destruído)
        /// </summary>
        public bool IsDead()
        {
            return _isDestroyed;
        }

        public float TakeDamage(int damage, GameObject attacker)
        {
            // Se já estiver destruído, não causa mais dano
            if (_isDestroyed)
                return 0;

            // Calcula o dano real subtraindo a defesa (mínimo 1 de dano)
            int actualDamage = damage - _defense;

            // Subtrai o dano da vida atual
            _currentHealth -= actualDamage;

            // Aplica efeito visual de dano apenas se o dano for significativo (>=1)
            if (actualDamage >= 1)
            {
                FlashEffect();
            }

            // Verifica se chegou à metade da vida e ainda não está rachado
            if (!_isCracked && _currentHealth <= _maxHealth / 2)
            {
                _isCracked = true;
                if (_animator != null)
                {
                    _animator.SetBool(IsCrackedHash, true);
                }
            }

            // Verifica se foi destruído
            if (!_isDestroyed && _currentHealth <= 0)
            {
                _isDestroyed = true;
                _isCracked = false;
                if (_animator != null)
                {
                    _animator.SetBool(IsCrackedHash, true);
                    _animator.SetBool(IsDestroyedHash, true);
                }

                // Desativa os colliders após destruição
                DisableColliders();

                // Gera os itens de drop
                DropLoot();

                // Inicia o efeito de fade out e destruição
                StartCoroutine(FadeOutAndDestroy());
            }

            return actualDamage;
        }

        /// <summary>
        /// Gera os itens de drop quando o objeto é destruído
        /// </summary>
        private void DropLoot()
        {
            // Se não há itens configurados ou o mínimo é zero, não dropa nada
            if (_dropItems.Count == 0 || _minDropCount <= 0)
            {
                return;
            }

            // Determina a quantidade de itens que serão dropados
            int dropCount = Random.Range(_minDropCount, Mathf.Min(_maxDropCount + 1, _dropItems.Count + 1));

            // Ajusta para não exceder o total de itens disponíveis
            dropCount = Mathf.Min(dropCount, _dropItems.Count);

            // Lista temporária para fazer a seleção aleatória sem repetição
            List<DropItem> availableItems = new List<DropItem>(_dropItems);

            // Dropa a quantidade determinada de itens
            for (int i = 0; i < dropCount; i++)
            {
                // Se acabaram os itens disponíveis, interrompe
                if (availableItems.Count == 0)
                    break;

                // Seleciona um item aleatório da lista
                int randomIndex = Random.Range(0, availableItems.Count);
                DropItem selectedItem = availableItems[randomIndex];

                // Remove o item selecionado para evitar repetição
                availableItems.RemoveAt(randomIndex);

                // Somente instancia se o prefab estiver configurado
                if (selectedItem.itemPrefab != null)
                {
                    // Gera um único item de drop
                    SpawnSingleDrop(selectedItem.itemPrefab, transform.position);

                    // Log para depuração
                    string itemName = !string.IsNullOrEmpty(selectedItem.itemName)
                        ? selectedItem.itemName
                        : selectedItem.itemPrefab.name;
                }
            }
        }

        /// <summary>
        /// Instancia um único item de drop usando o sistema de pool, 
        /// garantindo que seja lançado para longe do player
        /// </summary>
        /// <param name="prefab">O prefab do item a ser dropado</param>
        /// <param name="basePosition">A posição base para o drop</param>
        private void SpawnSingleDrop(GameObject prefab, Vector3 basePosition)
        {
            // Direção base para o drop (aleatória se não houver jogador)
            Vector2 dropDirection = Random.insideUnitCircle.normalized;
            float minDistance = _dropRadius * 0.5f; // Distância mínima para evitar drops próximos ao jogador

            // Encontra o jogador mais próximo (se existir)
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                // Vetor do objeto ao jogador
                Vector2 toPlayerVector = (Vector2)player.transform.position - (Vector2)basePosition;

                // Se o jogador estiver perto o suficiente para considerarmos sua posição
                if (toPlayerVector.magnitude < _dropRadius * 2f)
                {
                    // Direção oposta ao jogador (para longe dele)
                    Vector2 directionAwayFromPlayer = -toPlayerVector.normalized;

                    // Aplicamos uma pequena variação aleatória na direção (menos de 90 graus)
                    float randomAngle = Random.Range(-45f, 45f);
                    float rad = randomAngle * Mathf.Deg2Rad;

                    // Aplicamos a rotação usando matriz de rotação 2D
                    dropDirection = new Vector2(
                        directionAwayFromPlayer.x * Mathf.Cos(rad) - directionAwayFromPlayer.y * Mathf.Sin(rad),
                        directionAwayFromPlayer.x * Mathf.Sin(rad) + directionAwayFromPlayer.y * Mathf.Cos(rad)
                    ).normalized;

                    // Aumenta a distância mínima quando o jogador está próximo
                    minDistance = Mathf.Max(_dropRadius * 0.7f, toPlayerVector.magnitude * 0.5f);

                    Debug.DrawRay(basePosition, dropDirection * _dropRadius, Color.red, 2.0f);
                }
            }

            // Calcula uma distância aleatória (mas sempre maior que a distância mínima)
            float dropDistance = Random.Range(minDistance, _dropRadius);

            // Calcula a posição final de drop
            Vector3 dropPosition = basePosition + new Vector3(dropDirection.x * dropDistance, dropDirection.y * dropDistance, 0);

            // Usa ItemPool para instanciar
            var droppedItem = GameUtilities.ItemPool.GetItem(prefab, dropPosition, Quaternion.identity);
            if (droppedItem == null) return;

            // Usa cache de Rigidbody2D para melhor performance
            if (!_cachedRigidbodies.TryGetValue(droppedItem, out var rb))
            {
                rb = droppedItem.GetComponent<Rigidbody2D>();
                if (rb != null)
                    _cachedRigidbodies[droppedItem] = rb;
            }

            // Aplica impulso ao objeto dropado se ele tiver um Rigidbody2D
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;

                // Força na direção do drop (para longe da origem) com um pequeno salto
                Vector2 force = new Vector2(
                    dropDirection.x,
                    dropDirection.y + 0.5f  // Adiciona um pequeno componente para cima para criar efeito de salto
                ).normalized * 2f;

                rb.AddForce(force, ForceMode2D.Impulse);
            }

            // Configura retorno ao pool quando coletado
            if (droppedItem.TryGetComponent<ICollectable>(out var collectable))
            {
                collectable.OnCollected += () =>
                {
                    GameUtilities.ItemPool.ReturnToPool(droppedItem, prefab);
                    if (rb != null)
                        _cachedRigidbodies.Remove(droppedItem);
                };
            }
        }

        /// <summary>
        /// Coroutine para fazer o fade-out gradual do objeto e destruí-lo ao final
        /// </summary>
        private System.Collections.IEnumerator FadeOutAndDestroy()
        {
            // Verifica se temos um SpriteRenderer
            if (_spriteRenderer == null)
            {
                Destroy(gameObject);
                yield break;
            }

            // Tempo decorrido
            float elapsed = 0f;

            // Armazena o alpha inicial
            float startAlpha = _spriteRenderer.color.a;

            // Executa o fade-out gradual
            while (elapsed < _fadeOutDuration)
            {
                // Calcula o tempo normalizado (0 a 1)
                float normalizedTime = elapsed / _fadeOutDuration;

                // Usa a curva de animação para ajustar o alpha
                float currentAlpha = startAlpha * _fadeOutCurve.Evaluate(normalizedTime);

                // Aplica o novo alpha, mantendo as cores RGB originais
                Color fadeColor = _spriteRenderer.color;
                fadeColor.a = currentAlpha;
                _spriteRenderer.color = fadeColor;

                // Incrementa o tempo decorrido
                elapsed += Time.deltaTime;

                // Aguarda o próximo frame
                yield return null;
            }

            // Garante que o alpha seja zero no final
            Color finalColor = _spriteRenderer.color;
            finalColor.a = 0f;
            _spriteRenderer.color = finalColor;

            // Pequeno delay antes de destruir o objeto (opcional)
            yield return new WaitForSeconds(0.1f);

            // Destroi o GameObject ao finalizar o fade-out
            Debug.Log($"Objeto {gameObject.name} destruído após fade-out");
            Destroy(gameObject);
        }
    }
}
