using UnityEngine;
using System.Collections.Generic;
using TheSlimeKing.Core;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Extensão do sistema de crescimento que permite a criação de mini-slimes temporários
    /// </summary>
    public class SlimeDivisionAbility : MonoBehaviour
    {
        [System.Serializable]
        public class MiniSlimeConfig
        {
            public GameObject miniSlimePrefab;
            public float lifeDuration = 30f;
            public float sizeMultiplier = 0.3f;
            public float speedMultiplier = 1.5f;
            public int healthPercentage = 20;
            public int attackPercentage = 30;
        }

        [Header("Configurações")]
        [SerializeField] private MiniSlimeConfig _miniSlimeConfig;
        [SerializeField] private GameObject _divisionEffectPrefab;
        [SerializeField] private AudioClip _divisionSound;
        [SerializeField] private float _cooldownTime = 15f;

        [Header("Referências")]
        [SerializeField] private Transform _spawnPoint;

        // Estado interno
        private float _cooldownTimer = 0f;
        private List<GameObject> _activeMiniSlimes = new List<GameObject>();
        private PlayerStatus _playerStatus;
        private PlayerGrowth _playerGrowth;

        private void Start()
        {
            // Obtém referências
            _playerStatus = GetComponent<PlayerStatus>();
            _playerGrowth = GetComponent<PlayerGrowth>();

            if (_spawnPoint == null)
            {
                _spawnPoint = transform;
            }
        }

        private void Update()
        {
            // Atualiza cooldown
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
            }

            // Limpa lista de mini-slimes expirados
            _activeMiniSlimes.RemoveAll(item => item == null);
        }

        /// <summary>
        /// Tenta criar um mini-slime
        /// </summary>
        public bool TryCreateMiniSlime()
        {
            // Verifica se está em cooldown
            if (_cooldownTimer > 0f)
            {
                Debug.Log($"Habilidade em cooldown: {_cooldownTimer.ToString("F1")}s restantes");
                return false;
            }

            // Verifica se o jogador pode criar mini-slimes neste estágio
            int maxMiniSlimes = GetMaxMiniSlimesForCurrentStage();
            if (maxMiniSlimes <= 0)
            {
                Debug.Log("Este estágio não pode criar mini-slimes");
                return false;
            }

            // Verifica se já atingiu o limite de mini-slimes
            if (_activeMiniSlimes.Count >= maxMiniSlimes)
            {
                Debug.Log($"Limite de mini-slimes atingido: {_activeMiniSlimes.Count}/{maxMiniSlimes}");
                return false;
            }

            // Cria o mini-slime
            GameObject miniSlime = SpawnMiniSlime();
            if (miniSlime != null)
            {
                _activeMiniSlimes.Add(miniSlime);
                _cooldownTimer = _cooldownTime;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cria um mini-slime no mundo
        /// </summary>
        private GameObject SpawnMiniSlime()
        {
            if (_miniSlimeConfig.miniSlimePrefab == null)
            {
                Debug.LogError("MiniSlime prefab não configurado!");
                return null;
            }

            // Cria efeitos visuais e sonoros
            if (_divisionEffectPrefab != null)
            {
                GameObject effect = Instantiate(_divisionEffectPrefab, _spawnPoint.position, Quaternion.identity);
                Destroy(effect, 2f);
            }

            if (_divisionSound != null)
            {
                AudioSource.PlayClipAtPoint(_divisionSound, _spawnPoint.position);
            }

            // Calcular posição ligeiramente deslocada para evitar colisão imediata
            Vector3 spawnPos = _spawnPoint.position;
            spawnPos += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);

            // Criar mini-slime
            GameObject miniSlime = Instantiate(_miniSlimeConfig.miniSlimePrefab, spawnPos, Quaternion.identity);

            // Configurar propriedades
            MiniSlimeController controller = miniSlime.GetComponent<MiniSlimeController>();
            if (controller != null)
            {
                controller.Initialize(
                    _miniSlimeConfig.lifeDuration,
                    transform,
                    CalculateMiniSlimeStats()
                );
            }

            // Ajustar escala
            miniSlime.transform.localScale = Vector3.one * _miniSlimeConfig.sizeMultiplier;

            Debug.Log($"Mini-slime criado! Total: {_activeMiniSlimes.Count + 1}");

            return miniSlime;
        }

        /// <summary>
        /// Calcula os atributos do mini-slime com base no estágio atual do jogador
        /// </summary>
        private MiniSlimeController.MiniSlimeStats CalculateMiniSlimeStats()
        {
            MiniSlimeController.MiniSlimeStats stats = new MiniSlimeController.MiniSlimeStats();

            // Define valores padrão se não tiver PlayerStatus
            if (_playerStatus == null)
            {
                stats.health = 20;
                stats.attack = 5;
                stats.speed = _miniSlimeConfig.speedMultiplier;
                return stats;
            }

            // Calcula porcentagem dos stats do jogador
            stats.health = Mathf.RoundToInt(_playerStatus.GetMaxHealth() * _miniSlimeConfig.healthPercentage / 100f);
            stats.attack = Mathf.RoundToInt(_playerStatus.GetAttack() * _miniSlimeConfig.attackPercentage / 100f);
            stats.speed = _miniSlimeConfig.speedMultiplier;

            return stats;
        }

        /// <summary>
        /// Retorna o número máximo de mini-slimes permitido no estágio atual
        /// </summary>
        public int GetMaxMiniSlimesForCurrentStage()
        {
            if (_playerGrowth == null)
                return 0;

            SlimeStage currentStage = _playerGrowth.GetCurrentStage();

            switch (currentStage)
            {
                case SlimeStage.Baby: return 0;
                case SlimeStage.Young: return 1;
                case SlimeStage.Adult: return 2;
                case SlimeStage.King: return 3;
                default: return 0;
            }
        }

        /// <summary>
        /// Retorna o tempo restante de cooldown
        /// </summary>
        public float GetRemainingCooldown()
        {
            return Mathf.Max(0f, _cooldownTimer);
        }

        /// <summary>
        /// Retorna o número atual de mini-slimes ativos
        /// </summary>
        public int GetActiveMiniSlimeCount()
        {
            // Limpa lista de mini-slimes nulos
            _activeMiniSlimes.RemoveAll(item => item == null);
            return _activeMiniSlimes.Count;
        }
    }

    /// <summary>
    /// Controla o comportamento de um mini-slime
    /// </summary>
    public class MiniSlimeController : MonoBehaviour
    {
        [System.Serializable]
        public class MiniSlimeStats
        {
            public int health;
            public int attack;
            public float speed;
        }

        [Header("Configurações")]
        [SerializeField] private float _moveSpeed = 3f;
        [SerializeField] private float _rotationSpeed = 180f;
        [SerializeField] private float _followDistance = 2f;
        [SerializeField] private float _maxDistanceFromOwner = 10f;

        // Estado interno
        private float _remainingLifetime;
        private Transform _ownerTransform;
        private MiniSlimeStats _stats;
        private bool _isActive = true;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!_isActive || _ownerTransform == null)
                return;

            // Atualiza tempo de vida
            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                StartCoroutine(DissolveMiniSlime());
                return;
            }

            // Efeito de pulsação ao aproximar-se do fim da duração
            if (_remainingLifetime < 5f && _renderer != null)
            {
                float alpha = 0.5f + 0.5f * Mathf.Sin(Time.time * 5f);
                Color c = _renderer.color;
                _renderer.color = new Color(c.r, c.g, c.b, Mathf.Lerp(0.5f, 1f, alpha));
            }

            // Comportamento de seguir/vagar
            UpdateMovement();
        }

        /// <summary>
        /// Inicializa o mini-slime
        /// </summary>
        public void Initialize(float lifetime, Transform owner, MiniSlimeStats stats)
        {
            _remainingLifetime = lifetime;
            _ownerTransform = owner;
            _stats = stats;
            _moveSpeed = stats.speed;
        }

        /// <summary>
        /// Atualiza movimento do mini-slime
        /// </summary>
        private void UpdateMovement()
        {
            if (_ownerTransform == null)
                return;

            Vector3 ownerPos = _ownerTransform.position;
            float distanceToOwner = Vector3.Distance(transform.position, ownerPos);

            // Verifica se está muito longe do dono
            if (distanceToOwner > _maxDistanceFromOwner)
            {
                // Teleporta para perto do dono
                Vector2 randomOffset = Random.insideUnitCircle * 2f;
                transform.position = _ownerTransform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
                return;
            }

            // Comportamento padrão
            Vector3 moveDirection;

            if (distanceToOwner > _followDistance)
            {
                // Segue o dono
                moveDirection = (ownerPos - transform.position).normalized;
            }
            else
            {
                // Vagueia próximo ao dono
                moveDirection = new Vector3(
                    Mathf.Sin(Time.time * 0.5f),
                    Mathf.Cos(Time.time * 0.7f),
                    0
                ).normalized;
            }

            // Aplica movimento
            transform.position += moveDirection * _moveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// Coroutine para dissolver o mini-slime ao final da vida
        /// </summary>
        private System.Collections.IEnumerator DissolveMiniSlime()
        {
            _isActive = false;

            // Efeito de desaparecimento
            if (_renderer != null)
            {
                Color originalColor = _renderer.color;

                for (float t = 0; t < 1; t += Time.deltaTime * 2)
                {
                    _renderer.color = new Color(
                        originalColor.r,
                        originalColor.g,
                        originalColor.b,
                        1 - t
                    );

                    transform.localScale = Vector3.Lerp(
                        transform.localScale,
                        Vector3.zero,
                        t * 0.5f
                    );

                    yield return null;
                }
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Quando sofre dano, o mini-slime pode reagir
        /// </summary>
        public void TakeDamage(int damage)
        {
            _stats.health -= damage;

            if (_stats.health <= 0)
            {
                StartCoroutine(DissolveMiniSlime());
            }
        }
    }
}
