using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TheSlimeKing.Core.Combat;
using TheSlimeKing.Core.Inventory;
using TheSlimeKing.Core.Elemental;
using TheSlimeKing.Gameplay.Combat;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Objetos destrutíveis que podem receber dano e dropar itens
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DestructibleObject : InteractableObject, IDamageable
    {
        [Header("Configurações de Vida")]
        [SerializeField] private int _maxHealth = 50;
        [SerializeField] private int _defense = 2;

        [Header("Resistências Elementais")]
        [SerializeField] private int _fireResistance = 0; // Porcentagem de redução de dano
        [SerializeField] private int _waterResistance = 0;
        [SerializeField] private int _earthResistance = 0;
        [SerializeField] private int _airResistance = 0;

        [Header("Configurações de Destruição")]
        [SerializeField] private float _flashDuration = 0.15f;
        [SerializeField] private float _destroyFadeDuration = 1.5f;
        [SerializeField] private float _minAlphaBeforeDestroy = 0.05f;

        [Header("Efeitos")]
        [SerializeField] private GameObject _hitEffectPrefab;
        [SerializeField] private GameObject _destructionEffectPrefab;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private AudioClip _breakSound;

        [Header("Drops")]
        [SerializeField] private List<ItemDropChance> _possibleDrops = new List<ItemDropChance>();
        [SerializeField] private List<ItemDropChance> _guaranteedDrops = new List<ItemDropChance>();
        [SerializeField] private int _minTotalDrops = 0;
        [SerializeField] private int _maxTotalDrops = 3;
        [SerializeField] private float _dropForceMin = 2f;
        [SerializeField] private float _dropForceMax = 5f;

        // Estado
        private int _currentHealth;
        private bool _isDead = false;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private Color _originalColor;

        // Cache
        private static readonly int AnimatorHitParam = Animator.StringToHash("Hit");
        private static readonly int AnimatorDestroyParam = Animator.StringToHash("Destroy");

        protected override void Awake()
        {
            base.Awake();

            _currentHealth = _maxHealth;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();

            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
        }

        #region IDamageable Implementation

        /// <summary>
        /// Aplica dano ao objeto
        /// </summary>
        public int TakeDamage(int damage, GameObject attacker = null, Vector3? hitPoint = null)
        {
            if (_isDead)
                return 0;

            // Aplica defesa ao dano
            int actualDamage = Mathf.Max(damage - _defense, 1);

            // Aplica resistências elementais, se o atacante tem um componente de combate que fornece essa informação
            if (attacker != null)
            {
                // Exemplo de verificação de tipo elemental (a implementar conforme necessário)
                SlimeCombatManager combatManager = attacker.GetComponent<SlimeCombatManager>();
                if (combatManager != null)
                {
                    // Reduz o dano conforme a resistência elemental
                    ElementalType attackType = ElementalType.None; // Obter do sistema de combate
                    actualDamage = ApplyElementalResistance(actualDamage, attackType);
                }
            }

            _currentHealth -= actualDamage;

            // Feedback visual e sonoro
            if (_animator != null)
            {
                _animator.SetTrigger(AnimatorHitParam);
            }

            StartCoroutine(FlashEffect());

            if (hitPoint.HasValue && _hitEffectPrefab != null)
            {
                Instantiate(_hitEffectPrefab, hitPoint.Value, Quaternion.identity);
            }

            if (_hitSound != null)
            {
                AudioSource.PlayClipAtPoint(_hitSound, transform.position, 0.7f);
            }

            // Verifica se foi destruído
            if (_currentHealth <= 0 && !_isDead)
            {
                Die();
            }

            return actualDamage;
        }

        /// <summary>
        /// Verifica se o objeto foi destruído
        /// </summary>
        public bool IsDead()
        {
            return _isDead;
        }

        /// <summary>
        /// Retorna a saúde atual
        /// </summary>
        public int GetCurrentHealth()
        {
            return _currentHealth;
        }

        /// <summary>
        /// Retorna a saúde máxima
        /// </summary>
        public int GetMaxHealth()
        {
            return _maxHealth;
        }

        #endregion

        /// <summary>
        /// Processa a destruição do objeto
        /// </summary>
        private void Die()
        {
            _isDead = true;

            // Desativar interações
            SetInteractable(false);

            // Animação de destruição
            if (_animator != null)
            {
                _animator.SetTrigger(AnimatorDestroyParam);
            }

            // Efeito de destruição
            if (_destructionEffectPrefab != null)
            {
                Instantiate(_destructionEffectPrefab, transform.position, Quaternion.identity);
            }

            // Som de quebra
            if (_breakSound != null)
            {
                AudioSource.PlayClipAtPoint(_breakSound, transform.position);
            }

            // Física
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = false;
            }

            // Desativar colliders
            foreach (Collider2D col in GetComponents<Collider2D>())
            {
                col.enabled = false;
            }

            // Efeito de fade out
            StartCoroutine(DestroyWithFadeout());

            // Dropar itens
            DropItems();
        }

        /// <summary>
        /// Aplica resistência elemental ao dano
        /// </summary>
        private int ApplyElementalResistance(int damage, ElementalType elementalType)
        {
            int resistance = 0;

            switch (elementalType)
            {
                case ElementalType.Fire:
                    resistance = _fireResistance;
                    break;
                case ElementalType.Water:
                    resistance = _waterResistance;
                    break;
                case ElementalType.Earth:
                    resistance = _earthResistance;
                    break;
                case ElementalType.Air:
                    resistance = _airResistance;
                    break;
            }

            // Aplica a redução de dano
            if (resistance > 0)
            {
                float damageMultiplier = 1f - (resistance / 100f);
                damage = Mathf.RoundToInt(damage * damageMultiplier);
            }

            return Mathf.Max(damage, 1); // Mínimo de 1 de dano
        }

        /// <summary>
        /// Efeito de piscar ao receber dano
        /// </summary>
        private IEnumerator FlashEffect()
        {
            if (_spriteRenderer == null)
                yield break;

            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(_flashDuration);
            _spriteRenderer.color = _originalColor;
        }

        /// <summary>
        /// Aplica fade out gradual antes de destruir o objeto
        /// </summary>
        private IEnumerator DestroyWithFadeout()
        {
            if (_spriteRenderer == null)
            {
                Destroy(gameObject);
                yield break;
            }

            float elapsedTime = 0f;
            Color startColor = _spriteRenderer.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

            while (elapsedTime < _destroyFadeDuration)
            {
                _spriteRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime / _destroyFadeDuration);
                elapsedTime += Time.deltaTime;

                if (_spriteRenderer.color.a <= _minAlphaBeforeDestroy)
                    break;

                yield return null;
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Gera os drops de itens
        /// </summary>
        private void DropItems()
        {
            // Primeiro, dropar os itens garantidos
            foreach (ItemDropChance guaranteedDrop in _guaranteedDrops)
            {
                if (guaranteedDrop.ItemData != null)
                {
                    SpawnItemDrop(guaranteedDrop.ItemData, guaranteedDrop.Quantity);
                }
            }

            // Depois, escolhe aleatoriamente dos possíveis drops
            if (_possibleDrops.Count > 0)
            {
                int totalDrops = Random.Range(_minTotalDrops, _maxTotalDrops + 1);

                for (int i = 0; i < totalDrops; i++)
                {
                    // Percorre a lista de possíveis drops e verifica cada chance
                    foreach (ItemDropChance dropChance in _possibleDrops)
                    {
                        if (Random.Range(0f, 100f) < dropChance.DropChance)
                        {
                            SpawnItemDrop(dropChance.ItemData, dropChance.Quantity);
                            break; // Para cada slot de drop, só dropa um item
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Cria um item no mundo
        /// </summary>
        private void SpawnItemDrop(ItemData itemData, int quantity)
        {
            if (itemData == null)
                return;

            // Instancia o item no mundo
            GameObject itemObj = new GameObject($"Drop_{itemData.name}");
            itemObj.transform.position = transform.position;

            // Adiciona o componente de drop
            ItemWorldDrop dropComponent = itemObj.AddComponent<ItemWorldDrop>();
            dropComponent.SetItem(itemData, quantity);

            // Aplica força aleatória para espalhar os items
            float force = Random.Range(_dropForceMin, _dropForceMax);
            Vector2 direction = new Vector2(
                Random.Range(-1f, 1f),
                Random.Range(-0.2f, 1f)
            );

            dropComponent.ApplyInitialForce(direction, force);
        }
    }

    /// <summary>
    /// Define um item que pode ser dropado com sua chance
    /// </summary>
    [System.Serializable]
    public class ItemDropChance
    {
        public ItemData ItemData;
        [Range(1, 99)] public int Quantity = 1;
        [Range(0f, 100f)] public float DropChance = 100f;
    }
}
