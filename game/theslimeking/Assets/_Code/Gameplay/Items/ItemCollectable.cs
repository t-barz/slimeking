using UnityEngine;
using SlimeKing.Core;
using SlimeKing.Data;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Componente exclusivo para fragmentos elementais.
    /// Garante atração automática e entrega dos cristais ao GameManager.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class ItemCollectable : MonoBehaviour
    {
        [Header("Crystal Configuration")]
        [SerializeField] private CrystalElementalData crystalData;

        [Header("Attraction Settings")]
        [SerializeField] private float attractionRadius = 3f;
        [SerializeField] private float attractionSpeed = 5f;
        [SerializeField] private AnimationCurve attractionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Activation Delay")]
        [SerializeField] private float activationDelay = 0.5f;
        [SerializeField] private bool showDelayFeedback = true;
        [SerializeField] private Color delayTintColor = new Color(1f, 1f, 1f, 0.5f);

        [Header("Performance")]
        [SerializeField] private bool enableAttraction = true;
        [SerializeField] private float optimizationInterval = 0.1f;

        // Cache de componentes
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider;
        private Transform _playerTransform;

        // Estados de atração
        private bool _isBeingAttracted = false;
        private float _lastPlayerCheck = 0f;
        private float _attractionProgress = 0f;
        private Vector3 _startPosition;

        // Sistema de delay de ativação
        private float _spawnTime;
        private bool _attractionEnabled = false;
        private Color _originalColor;

        // Proteção contra múltiplas coletas
        private bool _isCollected = false;

        // Cache de performance
        private static readonly string PLAYER_TAG = "Player";
        private static Transform s_cachedPlayerTransform;

        #region Unity Lifecycle

        private void Awake()
        {
            CacheComponents();
            InitializeItem();

            // Registra tempo de spawn para delay de ativação
            _spawnTime = Time.time;
        }

        private void Start()
        {
            FindPlayer();
            SetupVisuals();

            // Desabilita collider se BounceHandler existir (será habilitado quando pronto)
            var bounceHandler = GetComponent<BounceHandler>();
            if (bounceHandler != null && _collider != null)
            {
                _collider.enabled = false;
            }
        }

        private void Update()
        {
            if (!enableAttraction || !HasValidCrystal())
            {
                return;
            }

            // Verifica se delay de ativação passou
            CheckActivationDelay();

            // Só processa atração se estiver ativada
            if (!_attractionEnabled) return;

            // Optimization: só procura player em intervalos
            if (Time.time - _lastPlayerCheck >= optimizationInterval)
            {
                CheckPlayerDistance();
                _lastPlayerCheck = Time.time;
            }

            // Atualiza atração se ativa
            if (_isBeingAttracted && _playerTransform != null)
            {
                UpdateAttraction();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Cacheia componentes necessários
        /// </summary>
        private void CacheComponents()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();

            // Configura collider como trigger
            _collider.isTrigger = true;
        }

        /// <summary>
        /// Inicializa item com dados configurados
        /// </summary>
        private void InitializeItem()
        {
            if (!HasValidCrystal())
            {
                enableAttraction = false;
                return;
            }

            attractionRadius = crystalData.attractionRadius;
            attractionSpeed = crystalData.attractionSpeed;
            activationDelay = crystalData.activationDelay;
        }

        /// <summary>
        /// Configura elementos visuais do item
        /// </summary>
        private void SetupVisuals()
        {
            if (_spriteRenderer == null) return;

            if (crystalData != null)
            {
                _originalColor = crystalData.crystalTint;
                if (crystalData.crystalSprite != null)
                {
                    _spriteRenderer.sprite = crystalData.crystalSprite;
                }
            }
            else
            {
                _originalColor = Color.white;
            }

            // Aplica cor (com ou sem delay feedback)
            _spriteRenderer.color = showDelayFeedback ? delayTintColor : _originalColor;
        }

        /// <summary>
        /// Encontra e cacheia referência do player
        /// </summary>
        private void FindPlayer()
        {
            // Usa cache estático para performance
            if (s_cachedPlayerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
                if (player != null)
                {
                    s_cachedPlayerTransform = player.transform;
                }
            }

            _playerTransform = s_cachedPlayerTransform;
        }

        /// <summary>
        /// Verifica se o delay de ativação passou e ativa a atração
        /// </summary>
        private void CheckActivationDelay()
        {
            if (_attractionEnabled) return;

            float timeSinceSpawn = Time.time - _spawnTime;

            if (timeSinceSpawn >= activationDelay)
            {
                // Verifica se BounceHandler existe e está pronto
                var bounceHandler = GetComponent<BounceHandler>();
                if (bounceHandler != null && !bounceHandler.IsReadyForCollection)
                {
                    // Aguarda BounceHandler estar pronto
                    return;
                }

                ActivateAttraction();
            }
        }

        /// <summary>
        /// Ativa a atração magnética e restaura visual normal
        /// </summary>
        private void ActivateAttraction()
        {
            _attractionEnabled = true;

            // Restaura cor original se estava usando feedback visual
            if (showDelayFeedback && _spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        #endregion

        #region Attraction System

        /// <summary>
        /// Verifica distância do player e inicia atração se necessário
        /// </summary>
        private void CheckPlayerDistance()
        {
            if (_playerTransform == null || _isBeingAttracted) return;

            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= attractionRadius)
            {
                StartAttraction();
            }
        }

        /// <summary>
        /// Inicia o processo de atração magnética
        /// </summary>
        private void StartAttraction()
        {
            _isBeingAttracted = true;
            _attractionProgress = 0f;
            _startPosition = transform.position;
        }

        /// <summary>
        /// Atualiza movimento de atração em direção ao player
        /// </summary>
        private void UpdateAttraction()
        {
            if (_playerTransform == null) return;

            // Incrementa progresso baseado na velocidade
            _attractionProgress += attractionSpeed * Time.deltaTime;

            // Aplica curva de atração para movimento suave
            float curveValue = attractionCurve.Evaluate(_attractionProgress);

            // Interpola posição entre posição inicial e posição do player
            Vector3 targetPosition = _playerTransform.position;
            transform.position = Vector3.Lerp(_startPosition, targetPosition, curveValue);

            // Verifica se chegou muito perto do player (coleta automática)
            float distanceToPlayer = Vector2.Distance(transform.position, _playerTransform.position);
            if (distanceToPlayer <= 0.2f)
            {
                // Evita múltiplas coletas por proximidade
                if (!_isCollected)
                {
                    CollectItem();
                }
            }
        }

        #endregion

        #region Collection System

        /// <summary>
        /// Detecta colisão com player
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isCollected)
            {
                return;
            }

            if (other.CompareTag(PLAYER_TAG))
            {
                CollectItem();
            }
        }

        /// <summary>
        /// Coleta o cristal e adiciona seu valor ao GameManager.
        /// </summary>
        public void CollectItem()
        {
            if (_isCollected)
            {
                return;
            }

            if (!HasValidCrystal())
            {
                Debug.LogWarning($"[{nameof(ItemCollectable)}] Cristal sem dados configurados em '{name}'.", this);
                return;
            }

            _isCollected = true;

            if (_collider != null)
            {
                _collider.enabled = false;
            }

            if (!ProcessCrystalCollection())
            {
                RevertCollectionState();
            }
        }

        /// <summary>
        /// Processa coleta de cristal elemental.
        /// </summary>
        private bool ProcessCrystalCollection()
        {
            if (!HasValidCrystal() || GameManager.Instance == null)
            {
                return false;
            }

            try
            {
                GameManager.Instance.AddCrystal(crystalData.crystalType, crystalData.value);
                PlayCrystalCollectionEffects();
                DestroyItem();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Reverte o estado de coleta quando falha.
        /// Permite que o jogador tente coletar novamente.
        /// </summary>
        private void RevertCollectionState()
        {
            _isCollected = false;

            // Reabilita collider
            if (_collider != null)
            {
                _collider.enabled = true;
            }
        }

        /// <summary>
        /// Executa efeitos visuais e sonoros da coleta de cristais
        /// </summary>
        private void PlayCrystalCollectionEffects()
        {
            // Determina posição do efeito (preferencialmente na posição do player)
            Vector3 effectPosition = _playerTransform != null ? _playerTransform.position : transform.position;

            if (crystalData != null && crystalData.collectVFX != null)
            {
                var vfx = Instantiate(crystalData.collectVFX, effectPosition, Quaternion.identity);
                if (vfx != null)
                {
                    Destroy(vfx, 2f);
                }
            }

            if (crystalData != null && crystalData.collectSound != null)
            {
                AudioSource.PlayClipAtPoint(crystalData.collectSound, effectPosition, 1f);
            }
        }

        /// <summary>
        /// Remove o item da cena
        /// </summary>
        private void DestroyItem()
        {
            // Animação de coleta antes de destruir
            StartCoroutine(PlayCollectionAnimation());
        }

        /// <summary>
        /// Animação simples de coleta
        /// </summary>
        private System.Collections.IEnumerator PlayCollectionAnimation()
        {
            float animationDuration = 0.3f;
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.2f;

            // Scale up
            float elapsed = 0f;
            while (elapsed < animationDuration * 0.5f)
            {
                float progress = elapsed / (animationDuration * 0.5f);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Scale down e fade out
            Color originalColor = _spriteRenderer.color;
            elapsed = 0f;
            while (elapsed < animationDuration * 0.5f)
            {
                float progress = elapsed / (animationDuration * 0.5f);
                transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, progress);

                Color newColor = originalColor;
                newColor.a = Mathf.Lerp(1f, 0f, progress);
                _spriteRenderer.color = newColor;

                elapsed += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Configura dados do cristal em runtime
        /// </summary>
        public void SetCrystalData(CrystalElementalData newCrystalData)
        {
            crystalData = newCrystalData;
            InitializeItem();
            SetupVisuals();
        }

        /// <summary>
        /// Retorna dados do cristal
        /// </summary>
        public CrystalElementalData GetCrystalData() => crystalData;

        /// <summary>
        /// Força coleta do item
        /// </summary>
        public void ForceCollect()
        {
            if (!_isCollected)
            {
                CollectItem();
            }
        }

        /// <summary>
        /// Verifica se o item já foi coletado
        /// </summary>
        public bool IsCollected => _isCollected;

        /// <summary>
        /// Pausa/retoma atração magnética
        /// </summary>
        public void SetAttractionEnabled(bool enabled)
        {
            enableAttraction = enabled;

            if (!enabled)
            {
                _isBeingAttracted = false;
                _attractionEnabled = false;
            }
        }

        /// <summary>
        /// Força ativação da atração ignorando delay
        /// </summary>
        public void ForceActivateAttraction()
        {
            ActivateAttraction();
        }

        /// <summary>
        /// Reinicia o delay de ativação
        /// </summary>
        public void RestartActivationDelay()
        {
            // Não permitir reiniciar se já foi coletado
            if (_isCollected) return;

            _spawnTime = Time.time;
            _attractionEnabled = false;
            _isBeingAttracted = false;

            // Aplica feedback visual se habilitado
            if (showDelayFeedback && _spriteRenderer != null)
            {
                _spriteRenderer.color = delayTintColor;
            }
        }

        /// <summary>
        /// Verifica se a atração está ativada
        /// </summary>
        public bool IsAttractionActive => _attractionEnabled;

        /// <summary>
        /// Retorna tempo restante para ativação
        /// </summary>
        public float GetRemainingActivationTime()
        {
            if (_attractionEnabled) return 0f;

            float elapsed = Time.time - _spawnTime;
            return Mathf.Max(0f, activationDelay - elapsed);
        }

        /// <summary>
        /// Controla o collider do ItemCollectable
        /// </summary>
        public void SetColliderEnabled(bool enabled)
        {
            if (_collider != null)
            {
                _collider.enabled = enabled;
            }
        }

        /// <summary>
        /// Retorna o valor do delay de ativação
        /// </summary>
        public float ActivationDelay => activationDelay;

        /// <summary>
        /// Verifica se o delay de ativação expirou
        /// </summary>
        public bool IsActivationDelayComplete
        {
            get
            {
                float timeSinceSpawn = Time.time - _spawnTime;
                return timeSinceSpawn >= activationDelay;
            }
        }

        /// <summary>
        /// Verifica se existe configuração válida de cristal.
        /// </summary>
        private bool HasValidCrystal() => crystalData != null;

        #endregion

        #region Debug & Gizmos

        /// <summary>
        /// Desenha raio de atração no editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!HasValidCrystal()) return;

            // Desenha raio de atração
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, attractionRadius);

            // Desenha linha para o player se atraindo
            if (_isBeingAttracted && _playerTransform != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _playerTransform.position);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Test Collection")]
        private void EditorTestCollection()
        {
            if (!Application.isPlaying) return;
            CollectItem();
        }

        [ContextMenu("Force Attraction")]
        private void EditorForceAttraction()
        {
            if (!Application.isPlaying) return;
            FindPlayer();
            StartAttraction();
        }

        [ContextMenu("Force Activate Attraction")]
        private void EditorForceActivateAttraction()
        {
            if (!Application.isPlaying) return;
            ForceActivateAttraction();
        }

        [ContextMenu("Restart Activation Delay")]
        private void EditorRestartDelay()
        {
            if (!Application.isPlaying) return;
            RestartActivationDelay();
        }
#endif

        #endregion
    }
}
