using System;
using UnityEngine;
using TheSlimeKing.Core.Combat;
using TheSlimeKing.Core.Elemental;

namespace TheSlimeKing.Gameplay.Combat
{
    /// <summary>
    /// Gerencia o sistema de combate do slime
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SlimeCombatManager : MonoBehaviour
    {
        [Header("Configurações de Ataque")]
        [SerializeField] private float _basicAttackRange = 1.0f;
        [SerializeField] private float _dashAttackRange = 2.0f;
        [SerializeField] private float _specialAttackRange = 1.5f;

        [Header("Configurações de Dano")]
        [SerializeField] private int _basicAttackDamage = 10;
        [SerializeField] private int _dashAttackDamage = 15;
        [SerializeField] private int _specialAttackDamage = 20;
        [SerializeField] private float _knockbackForce = 5f;

        [Header("Configurações de Cooldown")]
        [SerializeField] private float _basicAttackCooldown = 0.5f;
        [SerializeField] private float _dashAttackCooldown = 1.0f;
        [SerializeField] private float _specialAttackCooldown = 2.0f;

        [Header("Efeitos")]
        [SerializeField] private GameObject _basicAttackVFX;
        [SerializeField] private GameObject _dashAttackVFX;
        [SerializeField] private GameObject _specialAttackVFX;
        [SerializeField] private AudioClip _basicAttackSound;
        [SerializeField] private AudioClip _dashAttackSound;
        [SerializeField] private AudioClip _specialAttackSound;

        [Header("Referências")]
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private LayerMask _enemyLayers;

        // Estado
        private float _lastBasicAttackTime = -10f;
        private float _lastDashAttackTime = -10f;
        private float _lastSpecialAttackTime = -10f;

        // Componentes
        private Rigidbody2D _rb;
        private Animator _animator;
        private AudioSource _audioSource;

        // Eventos
        public event Action<AttackType, int> OnAttackPerformed;
        public event Action<GameObject, int> OnDamageDealt;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();

            // Cria um audio source se não existir
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.playOnAwake = false;
                _audioSource.spatialBlend = 1.0f; // 3D sound
                _audioSource.volume = 0.7f;
                _audioSource.pitch = 1.0f;
            }
        }

        /// <summary>
        /// Executa um ataque básico
        /// </summary>
        public void PerformBasicAttack()
        {
            // Verifica cooldown
            if (Time.time - _lastBasicAttackTime < _basicAttackCooldown)
                return;

            _lastBasicAttackTime = Time.time;

            // Anima o ataque
            if (_animator != null)
                _animator.SetTrigger("Attack");

            // Executa o ataque após um delay para sincronizar com a animação
            // No futuro, pode ser substituído por Animation Events
            Invoke(nameof(ExecuteBasicAttack), 0.1f);

            // Notifica evento
            OnAttackPerformed?.Invoke(AttackType.Basic, _basicAttackDamage);
        }

        /// <summary>
        /// Executa um dash attack (com movimento)
        /// </summary>
        public void PerformDashAttack()
        {
            // Verifica cooldown
            if (Time.time - _lastDashAttackTime < _dashAttackCooldown)
                return;

            _lastDashAttackTime = Time.time;

            // Anima o dash
            if (_animator != null)
                _animator.SetTrigger("Dash");

            // Executa o movimento e o ataque
            Invoke(nameof(ExecuteDashAttack), 0.05f);

            // Notifica evento
            OnAttackPerformed?.Invoke(AttackType.Dash, _dashAttackDamage);
        }        /// <summary>
        /// Executa um ataque especial (com efeito elemental)
        /// </summary>
        public void PerformSpecialAttack(ElementalType elementalType)
        {
            // Verifica cooldown
            if (Time.time - _lastSpecialAttackTime < _specialAttackCooldown)
                return;
            
            // Verificar se tem energia suficiente (no futuro)
            // Por agora, apenas permite o ataque
            
            _lastSpecialAttackTime = Time.time;
            
            // Anima o especial
            if (_animator != null)
                _animator.SetTrigger("Special");
            
            // Executa com delay para sincronizar com animação
            // Passa o tipo elemental para o método de execução
            Invoke(nameof(ExecuteSpecialAttack), 0.2f);
            
            // Salva o tipo elemental para uso quando o ataque for executado
            _currentElementalType = elementalType;
            
            // Notifica evento
            OnAttackPerformed?.Invoke(AttackType.Special, _specialAttackDamage);
        }
        
        // Tipo elemental atual para o próximo ataque especial
        private ElementalType _currentElementalType = ElementalType.None;

        // Métodos privados para execução dos ataques

        private void ExecuteBasicAttack()
        {
            // Detecta inimigos no alcance
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                _attackPoint != null ? _attackPoint.position : transform.position,
                _basicAttackRange,
                _enemyLayers
            );

            // Aplica dano
            foreach (Collider2D enemy in hitEnemies)
            {
                DealDamageToEnemy(enemy.gameObject, _basicAttackDamage);
                ApplyKnockback(enemy.gameObject, _knockbackForce * 0.5f);
            }

            // VFX e SFX
            SpawnEffect(_basicAttackVFX);
            PlaySound(_basicAttackSound);
        }

        private void ExecuteDashAttack()
        {
            // Dá um impulso no rigidbody
            if (_rb != null)
            {
                Vector2 dashDirection = transform.right * transform.localScale.x;
                _rb.AddForce(dashDirection * _knockbackForce * 2, ForceMode2D.Impulse);
            }

            // Detecta inimigos no alcance
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                _attackPoint != null ? _attackPoint.position : transform.position,
                _dashAttackRange,
                _enemyLayers
            );

            // Aplica dano
            foreach (Collider2D enemy in hitEnemies)
            {
                DealDamageToEnemy(enemy.gameObject, _dashAttackDamage);
                ApplyKnockback(enemy.gameObject, _knockbackForce);
            }

            // VFX e SFX
            SpawnEffect(_dashAttackVFX);
            PlaySound(_dashAttackSound);
        }

        private void ExecuteSpecialAttack()
        {
            // Detecta inimigos no alcance
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
                _attackPoint != null ? _attackPoint.position : transform.position,
                _specialAttackRange,
                _enemyLayers
            );

            // Aplica dano
            foreach (Collider2D enemy in hitEnemies)
            {
                DealDamageToEnemy(enemy.gameObject, _specialAttackDamage);
                ApplyKnockback(enemy.gameObject, _knockbackForce * 1.5f);

                // Efeitos elementais (a ser implementado)
                ApplyElementalEffect(enemy.gameObject, _currentElementalType);
            }

            // VFX e SFX
            SpawnEffect(_specialAttackVFX);
            PlaySound(_specialAttackSound);
        }

        /// <summary>
        /// Aplica dano a um inimigo
        /// </summary>
        private void DealDamageToEnemy(GameObject enemy, int damage)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                int actualDamage = damageable.TakeDamage(damage, gameObject, enemy.transform.position);

                // Evento de dano causado
                OnDamageDealt?.Invoke(enemy, actualDamage);

                // Debug
                Debug.Log($"Causou {actualDamage} de dano a {enemy.name}");
            }
        }

        /// <summary>
        /// Aplica knockback a um inimigo
        /// </summary>
        private void ApplyKnockback(GameObject enemy, float force)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Direção do knockback
                Vector2 direction = (enemy.transform.position - transform.position).normalized;
                enemyRb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Cria um efeito visual no ponto de ataque
        /// </summary>
        private void SpawnEffect(GameObject effectPrefab)
        {
            if (effectPrefab != null)
            {
                Vector3 spawnPos = _attackPoint != null ? _attackPoint.position : transform.position;
                GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
                Destroy(effect, 1.5f); // Auto-destruição após 1.5 segundos
            }
        }

        /// <summary>
        /// Reproduz um som de ataque
        /// </summary>
        private void PlaySound(AudioClip clip)
        {
            if (clip != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        /// <summary>
        /// Reinicia todos os cooldowns
        /// </summary>
        public void ResetCooldowns()
        {
            _lastBasicAttackTime = -10f;
            _lastDashAttackTime = -10f;
            _lastSpecialAttackTime = -10f;
        }

        /// <summary>
        /// Verifica se um ataque está disponível (fora de cooldown)
        /// </summary>
        public bool IsAttackAvailable(AttackType attackType)
        {
            switch (attackType)
            {
                case AttackType.Basic:
                    return Time.time - _lastBasicAttackTime >= _basicAttackCooldown;
                case AttackType.Dash:
                    return Time.time - _lastDashAttackTime >= _dashAttackCooldown;
                case AttackType.Special:
                    return Time.time - _lastSpecialAttackTime >= _specialAttackCooldown;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Obtém o cooldown restante de um ataque
        /// </summary>
        public float GetRemainingCooldown(AttackType attackType)
        {
            float timeElapsed;
            float totalCooldown;

            switch (attackType)
            {
                case AttackType.Basic:
                    timeElapsed = Time.time - _lastBasicAttackTime;
                    totalCooldown = _basicAttackCooldown;
                    break;
                case AttackType.Dash:
                    timeElapsed = Time.time - _lastDashAttackTime;
                    totalCooldown = _dashAttackCooldown;
                    break;
                case AttackType.Special:
                    timeElapsed = Time.time - _lastSpecialAttackTime;
                    totalCooldown = _specialAttackCooldown;
                    break;
                default:
                    return 0f;
            }

            return Mathf.Max(0, totalCooldown - timeElapsed);
        }

        /// <summary>
        /// Desenha gizmos para visualizar o alcance dos ataques
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Vector3 attackPos = _attackPoint != null ? _attackPoint.position : transform.position;

            // Básico
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPos, _basicAttackRange);

            // Dash
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPos, _dashAttackRange);

            // Especial
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(attackPos, _specialAttackRange);
        }

        /// <summary>
        /// Aplica um efeito elemental ao inimigo baseado no tipo
        /// </summary>
        private void ApplyElementalEffect(GameObject enemy, ElementalType elementalType)
        {
            // Verificação básica
            if (enemy == null || elementalType == ElementalType.None)
                return;
            
            // Efeito baseado no tipo elemental
            switch (elementalType)
            {
                case ElementalType.Fire:
                    // Aplica dano ao longo do tempo (DOT)
                    StartCoroutine(ApplyDamageOverTime(enemy, 2, 3, 0.5f));
                    Debug.Log($"Efeito de Fogo aplicado em {enemy.name}");
                    break;
                    
                case ElementalType.Water:
                    // Reduz a velocidade do inimigo
                    // Implementação depende da estrutura do inimigo
                    var enemyRb = enemy.GetComponent<Rigidbody2D>();
                    if (enemyRb != null)
                    {
                        enemyRb.linearDamping *= 2; // Dobro do arrasto por 3 segundos
                        StartCoroutine(ResetDrag(enemyRb, enemyRb.linearDamping / 2, 3f));
                    }
                    Debug.Log($"Efeito de Água aplicado em {enemy.name}");
                    break;
                    
                case ElementalType.Earth:
                    // Aumenta o knockback
                    ApplyKnockback(enemy, _knockbackForce * 2.5f);
                    Debug.Log($"Efeito de Terra aplicado em {enemy.name}");
                    break;
                    
                case ElementalType.Air:
                    // Empurrão em área
                    Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(
                        enemy.transform.position,
                        2f, // Raio de efeito em área
                        _enemyLayers
                    );
                    
                    foreach (var nearbyEnemy in nearbyEnemies)
                    {
                        if (nearbyEnemy.gameObject != enemy) // Não aplica duas vezes ao alvo principal
                        {
                            ApplyKnockback(nearbyEnemy.gameObject, _knockbackForce * 1.0f);
                        }
                    }
                    Debug.Log($"Efeito de Ar aplicado em {enemy.name} e {nearbyEnemies.Length-1} inimigos próximos");
                    break;
            }
        }
        
        /// <summary>
        /// Aplica dano ao longo do tempo a um inimigo
        /// </summary>
        private System.Collections.IEnumerator ApplyDamageOverTime(
            GameObject enemy, int damagePerTick, int ticks, float interval)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable == null) yield break;
            
            for (int i = 0; i < ticks; i++)
            {
                // Espera pelo intervalo
                yield return new WaitForSeconds(interval);
                
                // Verifica se o objeto ainda existe e não está morto
                if (enemy == null || damageable.IsDead()) 
                    break;
                
                // Aplica o dano
                damageable.TakeDamage(damagePerTick, gameObject);
            }
        }
        
        /// <summary>
        /// Reinicia o drag do inimigo após um tempo
        /// </summary>
        private System.Collections.IEnumerator ResetDrag(Rigidbody2D rb, float originalDrag, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (rb != null)
            {
                rb.linearDamping = originalDrag;
            }
        }
    }
}
