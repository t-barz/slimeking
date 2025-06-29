using UnityEngine;
using TheSlimeKing.Core;
using TheSlimeKing.Gameplay;
using System.Collections.Generic; // Adicionado para usar List

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

        // Lista de possíveis alvos dentro do alcance de ataque
        private List<Collider2D> _potentialTargets = new List<Collider2D>();

        // Controle de ataque removido - não há mais restrições de ataques simultâneos

        private void Awake()
        {
            _parentSlime = transform.parent.gameObject;
            _slimeStats = _parentSlime.GetComponent<SlimeStats>();
            _audioSource = _parentSlime.GetComponent<AudioSource>();
        }

        /// <summary>
        /// Realiza um ataque em todos os alvos válidos dentro do alcance
        /// </summary>
        /// <returns>Sempre retorna verdadeiro, pois não há mais restrições de ataque</returns>
        public bool TryAttack()
        {
            // Se não há alvos, não prossegue mas conta como um ataque realizado
            if (_potentialTargets.Count == 0)
            {
                return true;
            }

            int attackDamage = CalculateDamage();

            // Lista temporária para evitar problemas ao iterar e modificar a lista original
            List<Collider2D> currentTargets = new List<Collider2D>(_potentialTargets);

            // Processa cada alvo válido
            foreach (var target in currentTargets)
            {
                // Verifica se o alvo ainda é válido
                if (target == null || !target.gameObject.activeInHierarchy)
                {
                    _potentialTargets.Remove(target);
                    continue;
                }

                // Calcula a direção do knockback
                Vector2 knockbackDirection = (target.transform.position - transform.position).normalized;

                // Aplica dano e knockback
                IDamageable damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(attackDamage, _parentSlime);

                    // Exibe efeitos de impacto
                    ShowHitEffects(target.transform.position);
                }
            }

            return true;
        }

        /// <summary>
        /// Exibe efeitos visuais e sonoros ao acertar um alvo
        /// </summary>
        private void ShowHitEffects(Vector3 position)
        {
            // Exibe partículas se disponíveis
            if (hitParticles != null)
            {
                hitParticles.transform.position = position;
                hitParticles.Play();
            }

            // Toca som de impacto se disponível
            if (hitSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(hitSound);
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

            Debug.Log($"Dano calculado: {baseDamage} (Multiplicador: {damageMultiplier})");
            return baseDamage;
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
        /// Verifica se o ataque pode ser usado
        /// </summary>
        public bool CanAttack()
        {
            // Sempre pode atacar, não há mais restrições
            return true;
        }

        /// <summary>
        /// Retorna o tempo restante antes que um novo ataque possa ser realizado
        /// </summary>
        public float GetRemainingCooldown()
        {
            // Sempre retorna 0, não há mais cooldown ou restrições de ataque
            return 0f;
        }

        /// <summary>
        /// Define o tipo de ataque a ser usado
        /// </summary>
        /// <param name="useSpecial">Se verdadeiro, usa o poder especial, se falso, usa ataque básico</param>
        public void SetAttackType(bool useSpecial)
        {
            useSpecialAttack = useSpecial;
        }

        /// <summary>
        /// Detecta quando um objeto entra no colisor de ataque
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se o objeto é um alvo válido para ataque
            if (IsValidTarget(other))
            {
                // Adiciona à lista de alvos potenciais
                if (!_potentialTargets.Contains(other))
                {
                    _potentialTargets.Add(other);
                }
            }
        }

        /// <summary>
        /// Detecta quando um objeto sai do colisor de ataque
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            // Remove da lista de alvos potenciais
            if (_potentialTargets.Contains(other))
            {
                _potentialTargets.Remove(other);
            }
        }

        /// <summary>
        /// Verifica se o collider corresponde a um alvo válido para ataque
        /// </summary>
        private bool IsValidTarget(Collider2D collider)
        {
            if (collider == null)
                return false;

            // Verifica se possui componente que pode receber dano
            bool hasDamageable = collider.GetComponent<IDamageable>() != null;

            // Ignora o próprio slime e seus componentes filhos
            bool isNotSelf = collider.gameObject != _parentSlime && !collider.transform.IsChildOf(_parentSlime.transform);

            // Verifica se o objeto está ativo
            bool isActive = collider.gameObject.activeInHierarchy;

            return hasDamageable && isNotSelf && isActive;
        }
    }
}
