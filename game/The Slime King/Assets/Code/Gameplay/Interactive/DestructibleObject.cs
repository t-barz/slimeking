using UnityEngine;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Objetos destrutíveis que podem receber dano e mudar de estado visual
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DestructibleObject : MonoBehaviour
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

        // Estado
        private int _currentHealth;
        private bool _isCracked = false;
        private bool _isDestroyed = false;

        // Componentes
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Color _originalColor;

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
        /// Aplica dano ao objeto destrutível
        /// </summary>
        /// <param name="damage">Quantidade de dano bruto</param>
        /// <param name="attacker">Referência ao atacante (opcional)</param>
        /// <param name="hitPoint">Ponto onde ocorreu o impacto (opcional)</param>
        /// <returns>Quantidade de dano real aplicado após defesas</returns>
        public int TakeDamage(int damage, GameObject attacker, Vector3? hitPoint = null)
        {
            // Se já estiver destruído, não causa mais dano
            if (_isDestroyed)
                return 0;

            // Calcula o dano real subtraindo a defesa (mínimo 1 de dano)
            int actualDamage = Mathf.Max(1, damage - _defense);

            // Subtrai o dano da vida atual
            _currentHealth -= actualDamage;

            // Aplica efeito visual de dano apenas se o dano for significativo (>=1)
            if (actualDamage >= 1)
            {
                FlashEffect();
            }

            // Log de debug para informar sobre o dano
            Debug.Log($"Objeto {gameObject.name} recebeu {actualDamage} de dano" +
                     (attacker != null ? $" de {attacker.name}" : ""));

            // Verifica se chegou à metade da vida e ainda não está rachado
            if (!_isCracked && _currentHealth <= _maxHealth / 2)
            {
                _isCracked = true;
                if (_animator != null)
                {
                    _animator.SetBool(IsCrackedHash, true);
                    Debug.Log($"Objeto {gameObject.name} rachou (vida: {_currentHealth}/{_maxHealth})");
                }
            }

            // Verifica se foi destruído
            if (!_isDestroyed && _currentHealth <= 0)
            {
                _isDestroyed = true;
                if (_animator != null)
                {
                    _animator.SetBool(IsDestroyedHash, true);
                    Debug.Log($"Objeto {gameObject.name} foi destruído");
                }

                // Desativa os colliders após destruição
                DisableColliders();
            }

            return actualDamage;
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
    }
}
