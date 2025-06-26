using UnityEngine;
using System.Collections;

namespace TheSlimeKing.Gameplay.Enemies
{
    /// <summary>
    /// Implementação básica de um inimigo simples
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class SimpleEnemy : MonoBehaviour
    {
        [Header("Status")]
        [SerializeField] private int _maxHealth = 50;
        [SerializeField] private int _defense = 2;
        [SerializeField] private int _damage = 10;

        [Header("Comportamento")]
        [SerializeField] private float _movementSpeed = 2f;
        [SerializeField] private float _attackCooldown = 2f;
        [SerializeField] private float _detectionRange = 5f;
        [SerializeField] private float _attackRange = 1.2f;

        [Header("Feedback")]
        [SerializeField] private GameObject _hitEffect;
        [SerializeField] private GameObject _deathEffect;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private AudioClip _deathSound;
        [SerializeField] private float _flashDuration = 0.15f;

        [Header("Componentes")]
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private LayerMask _playerLayer;

        // Estado
        private int _currentHealth;
        private bool _isDead = false;
        private bool _isAttacking = false;
        private float _lastAttackTime;
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rb;
        private Animator _animator;
        private AudioSource _audioSource;

        // Cache
        private Color _originalColor;
        private GameObject _player;
        private static readonly int AnimatorDead = Animator.StringToHash("Dead");
        private static readonly int AnimatorHit = Animator.StringToHash("Hit");
        private static readonly int AnimatorAttack = Animator.StringToHash("Attack");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();

            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.spatialBlend = 1.0f; // 3D sound
                _audioSource.volume = 0.7f;
            }

            _originalColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;
            _currentHealth = _maxHealth;
            _lastAttackTime = -_attackCooldown; // Permite atacar imediatamente no início
        }

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Update()
        {
            if (_isDead || _player == null) return;

            // Verifica distância para o jogador
            float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);

            // Comportamento simples baseado na distância
            if (distanceToPlayer <= _attackRange && Time.time >= _lastAttackTime + _attackCooldown)
            {
                AttackPlayer();
            }
            else if (distanceToPlayer <= _detectionRange)
            {
                MoveTowardsPlayer();
            }
        }

        private void MoveTowardsPlayer()
        {
            if (_isAttacking) return;

            // Direciona o sprite para o jogador
            Vector3 direction = _player.transform.position - transform.position;
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);

            // Move em direção ao jogador
            Vector2 newPosition = Vector2.MoveTowards(
                transform.position,
                _player.transform.position,
                _movementSpeed * Time.deltaTime
            );

            _rb.MovePosition(newPosition);
        }

        private void AttackPlayer()
        {
            _lastAttackTime = Time.time;
            _isAttacking = true;

            // Animar ataque
            if (_animator != null)
            {
                _animator.SetTrigger(AnimatorAttack);
            }

            // Retardar o dano para sincronizar com a animação
            StartCoroutine(DealDamageAfterDelay(0.3f));
        }

        private IEnumerator DealDamageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Verifica se o jogador ainda está no alcance
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(
                _attackPoint != null ? _attackPoint.position : transform.position,
                _attackRange,
                _playerLayer
            );

            // Termina o ataque após um pequeno delay
            StartCoroutine(EndAttackAfterDelay(0.5f));
        }

        private IEnumerator EndAttackAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            _isAttacking = false;
        }

        // Implementação da interface IDamageable

        public int TakeDamage(int damage, GameObject attacker = null, Vector3? hitPoint = null)
        {
            if (_isDead) return 0;

            // Calcula dano real
            int actualDamage = Mathf.Max(damage - _defense, 1);
            _currentHealth -= actualDamage;

            // Feedback
            if (_animator != null)
            {
                _animator.SetTrigger(AnimatorHit);
            }

            StartCoroutine(FlashRoutine());

            if (hitPoint.HasValue && _hitEffect != null)
            {
                Instantiate(_hitEffect, hitPoint.Value, Quaternion.identity);
            }

            if (_hitSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_hitSound);
            }

            // Verifica se morreu
            if (_currentHealth <= 0 && !_isDead)
            {
                Die();
            }

            return actualDamage;
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public int GetCurrentHealth()
        {
            return _currentHealth;
        }

        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        private void Die()
        {
            _isDead = true;

            // Animação de morte
            if (_animator != null)
            {
                _animator.SetBool(AnimatorDead, true);
            }

            // Efeitos
            if (_deathEffect != null)
            {
                Instantiate(_deathEffect, transform.position, Quaternion.identity);
            }

            if (_deathSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_deathSound);
            }

            // Desabilitar física e colisões
            if (_rb != null) _rb.simulated = false;
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // Destruir o objeto após um delay
            Destroy(gameObject, 2f);
        }

        private IEnumerator FlashRoutine()
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(_flashDuration);
                _spriteRenderer.color = _originalColor;
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualizar raios de detecção e ataque
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            Gizmos.color = Color.red;
            Vector3 attackPos = _attackPoint != null ? _attackPoint.position : transform.position;
            Gizmos.DrawWireSphere(attackPos, _attackRange);
        }
    }
}
