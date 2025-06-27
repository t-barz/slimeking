using UnityEngine;
using TheSlimeKing.Core;
using TheSlimeKing.Gameplay;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Responsável por gerenciar a colisão de ataque do slime
    /// </summary>
    public class SlimeAttackCollider : MonoBehaviour
    {
        [Header("Configurações de Ataque")]
        [SerializeField] private bool useSpecialAttack = false;  // Se false, usa baseAttack; se true, usa baseSpecial
        [SerializeField] private int damageMultiplier = 1;  // Multiplicador aplicado ao dano base
        [SerializeField] private float knockbackForce = 5.0f;    // Força de repulsão ao atingir inimigos

        [Header("Efeitos")]
        [SerializeField] private ParticleSystem hitParticles;    // Partículas ao acertar um alvo
        [SerializeField] private AudioClip hitSound;             // Som de impacto

        // Referências
        private SlimeStats _slimeStats;
        private AudioSource _audioSource;
        private GameObject _parentSlime;  // Referência ao GameObject do slime pai

        private void Awake()
        {
            _parentSlime = transform.parent.gameObject;
            _slimeStats = _parentSlime.GetComponent<SlimeStats>();
            _audioSource = _parentSlime.GetComponent<AudioSource>();

            // Desativa o colisor de ataque no início
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Chamado quando o collider de ataque entra em contato com outro collider
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se o objeto possui a interface IDamageable
            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                // Calcula o dano baseado na configuração
                int damage = CalculateDamage();

                // Aplica dano ao alvo
                damageable.TakeDamage(damage, _parentSlime);

                // Aplica força de knockback se o objeto tiver Rigidbody2D
                ApplyKnockback(other);

                // Reproduz efeitos de impacto
                PlayImpactEffects(other.transform.position);
            }
        }

        /// <summary>
        /// Calcula o dano com base na escolha entre ataque normal ou especial
        /// </summary>
        private int CalculateDamage()
        {
            if (_slimeStats == null)
                return 0;

            // Escolhe o tipo de ataque baseado na configuração
            int baseDamage = useSpecialAttack ?
                _slimeStats.Special.Value :
                _slimeStats.Attack.Value;

            // Aplica o multiplicador de dano
            return baseDamage * damageMultiplier;
        }

        /// <summary>
        /// Aplica força de knockback ao objeto atingido
        /// </summary>
        private void ApplyKnockback(Collider2D other)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Calcula a direção do knockback (do slime para o objeto atingido)
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

                // Aplica a força
                rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Reproduz efeitos visuais e sonoros do impacto
        /// </summary>
        private void PlayImpactEffects(Vector3 position)
        {
            // Reproduz partículas se configuradas
            if (hitParticles != null)
            {
                ParticleSystem particles = Instantiate(hitParticles, position, Quaternion.identity);
                particles.Play();

                // Destrói o sistema de partículas após a reprodução
                Destroy(particles.gameObject, particles.main.duration);
            }

            // Reproduz som de impacto se configurado
            if (hitSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(hitSound);
            }
        }

        /// <summary>
        /// Define o tipo de ataque (normal ou especial)
        /// </summary>
        /// <param name="isSpecial">Se true, usa o ataque especial</param>
        public void SetAttackType(bool isSpecial)
        {
            useSpecialAttack = isSpecial;
        }
    }
}
