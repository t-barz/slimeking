using UnityEngine;
using SlimeMec.Items;
using TheSlimeKing.Inventory;
using SlimeKing.Core;
using SlimeKing.Data;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Componente otimizado para itens coletáveis.
    /// Combina atração magnética, coleta automática e aplicação de efeitos.
    /// </summary>
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class ItemCollectable : MonoBehaviour
    {
        [Header("Item Configuration")]
        [SerializeField] private CollectableItemData itemData;

        [Header("Crystal Configuration")]
        [SerializeField] private CrystalElementalData crystalData;

        [Header("Inventory Integration")]
        [SerializeField] private ItemData inventoryItemData;
        [SerializeField] private int itemQuantity = 1;

        [Header("🧲 Sistema de Atração")]
        [SerializeField] private float attractionRadius = 3f;
        [SerializeField] private float attractionSpeed = 5f;
        [SerializeField] private AnimationCurve attractionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("⏱️ Delay de Ativação")]
        [SerializeField] private float activationDelay = 0.5f;
        [SerializeField] private bool showDelayFeedback = true;
        [SerializeField] private Color delayTintColor = new Color(1f, 1f, 1f, 0.5f);

        [Header("⚡ Performance")]
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
                Debug.Log($"[ItemCollectable] Collider desabilitado - aguardando BounceHandler estar pronto");
            }
        }

        private void Update()
        {
            // Verifica se há dados válidos (prioridade para cristais)
            bool hasValidData = crystalData != null || itemData != null;

            if (!enableAttraction || !hasValidData)
            {
                if (!enableAttraction)
                    Debug.LogWarning($"[ItemCollectable] {gameObject.name} enableAttraction=false");
                if (!hasValidData)
                    Debug.LogWarning($"[ItemCollectable] {gameObject.name} sem dados válidos (crystalData ou itemData)");
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
            Debug.Log($"[ItemCollectable] InitializeItem chamado. crystalData={crystalData != null}, itemData={itemData != null}");

            // Prioridade 1: Cristais (sistema preferido)
            if (crystalData != null)
            {
                attractionRadius = crystalData.attractionRadius;
                attractionSpeed = crystalData.attractionSpeed;
                activationDelay = crystalData.activationDelay;
                Debug.Log($"[ItemCollectable] Cristal {crystalData.crystalType} configurado: radius={attractionRadius}, speed={attractionSpeed}");
                return;
            }

            // Prioridade 2: Sistema legado
            if (itemData != null)
            {
                // Usa configurações do itemData
                attractionRadius = itemData.detectionRadius;
                attractionSpeed = itemData.attractSpeed;
                Debug.Log($"[ItemCollectable] Configurado com itemData: radius={attractionRadius}, speed={attractionSpeed}");
                return;
            }

            Debug.LogWarning($"[ItemCollectable] {gameObject.name} não tem CrystalData nem ItemData configurado!");
        }

        /// <summary>
        /// Configura elementos visuais do item
        /// </summary>
        private void SetupVisuals()
        {
            if (_spriteRenderer == null) return;

            // Determina cor original baseada no tipo de dados (prioridade para cristais)
            if (crystalData != null)
            {
                _originalColor = crystalData.crystalTint;

                // Aplica sprite do cristal se configurado
                if (crystalData.crystalSprite != null)
                {
                    _spriteRenderer.sprite = crystalData.crystalSprite;
                }
            }
            else if (itemData != null)
            {
                _originalColor = itemData.itemTint;
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
                    Debug.Log($"[ItemCollectable] Player encontrado e cacheado: {player.name}");
                }
                else
                {
                    Debug.LogError($"[ItemCollectable] Nenhum GameObject com tag '{PLAYER_TAG}' encontrado na cena!");
                }
            }

            _playerTransform = s_cachedPlayerTransform;

            if (_playerTransform == null)
            {
                Debug.LogWarning("[ItemCollectable] Player não encontrado! Certifique-se que o player tem a tag 'Player'.");
            }
            else
            {
                Debug.Log($"[ItemCollectable] {gameObject.name} conectado ao player: {_playerTransform.name}");
            }
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

            // Log do item ativado (prioridade para cristais)
            string itemName = "Item Desconhecido";
            if (crystalData != null)
            {
                itemName = crystalData.crystalName;
            }
            else if (itemData != null)
            {
                itemName = itemData.itemName;
            }

            Debug.Log($"[ItemCollectable] {itemName} ativou atração magnética após {activationDelay}s");
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

            // Debug periódico da distância
            if (Time.frameCount % 60 == 0) // A cada segundo aproximadamente
            {
                Debug.Log($"[ItemCollectable] {gameObject.name} distância do player: {distanceToPlayer:F2} (limite: {attractionRadius})");
            }

            if (distanceToPlayer <= attractionRadius)
            {
                Debug.Log($"[ItemCollectable] {gameObject.name} player dentro do alcance! Iniciando atração");
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

            // Nome do item baseado no tipo de dados
            string itemName = "Item";
            if (crystalData != null)
            {
                itemName = crystalData.crystalName;
            }
            else if (itemData != null)
            {
                itemName = itemData.itemName;
            }

            Debug.Log($"[ItemCollectable] {itemName} iniciou atração magnética");
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
            Debug.Log($"[ItemCollectable] {gameObject.name} detectou trigger com {other.gameObject.name} (tag: {other.tag})");

            // Evita múltiplas coletas
            if (_isCollected)
            {
                Debug.Log($"[ItemCollectable] {gameObject.name} já foi coletado, ignorando trigger");
                return;
            }

            if (other.CompareTag(PLAYER_TAG))
            {
                Debug.Log($"[ItemCollectable] {gameObject.name} iniciando coleta com player {other.gameObject.name}");
                CollectItem(other.gameObject);
            }
            else
            {
                Debug.Log($"[ItemCollectable] {gameObject.name} ignorando trigger - não é player (esperado: {PLAYER_TAG})");
            }
        }

        /// <summary>
        /// Coleta o item e aplica seus efeitos
        /// </summary>
        public void CollectItem(GameObject collector = null)
        {
            // Evita múltiplas coletas
            if (_isCollected) return;

            // Marca como coletado imediatamente
            _isCollected = true;

            // Desabilita collider para evitar novas colisões
            if (_collider != null)
            {
                _collider.enabled = false;
            }

            // Encontra o coletor (player)
            if (collector == null && _playerTransform != null)
            {
                collector = _playerTransform.gameObject;
            }

            if (collector == null)
            {
                Debug.LogWarning("[ItemCollectable] Coletor não encontrado!");
                _isCollected = false; // Permite tentar novamente
                if (_collider != null) _collider.enabled = true;
                return;
            }

            // Tenta coletar como cristal se crystalData estiver configurado (prioridade máxima)
            if (crystalData != null)
            {
                Debug.Log($"[ItemCollectable] Processando cristal: {crystalData.crystalName}");

                if (GameManager.Instance != null)
                {
                    Debug.Log($"[ItemCollectable] GameManager encontrado, chamando AddCrystal({crystalData.crystalType}, {crystalData.value})");
                    GameManager.Instance.AddCrystal(crystalData.crystalType, crystalData.value);

                    Debug.Log($"[ItemCollectable] Cristal '{crystalData.crystalName}' coletado (+{crystalData.value} {crystalData.crystalType})");

                    // Executa efeitos visuais e sonoros
                    PlayCrystalCollectionEffects();

                    // Remove o cristal da cena
                    DestroyItem();
                    return;
                }
                else
                {
                    Debug.LogError($"[ItemCollectable] GameManager.Instance é null! Cristal não pode ser processado.");

                    // Reverte estado de coleta para permitir tentar novamente
                    _isCollected = false;
                    if (_collider != null) _collider.enabled = true;
                    return;
                }
            }

            // Tenta adicionar ao inventário se inventoryItemData estiver configurado
            if (inventoryItemData != null && InventoryManager.Instance != null)
            {
                bool addedToInventory = InventoryManager.Instance.AddItem(inventoryItemData, itemQuantity);

                if (!addedToInventory)
                {
                    // Inventário cheio - não destrói o item
                    Debug.Log($"[ItemCollectable] Inventário cheio! Item '{inventoryItemData.itemName}' não foi coletado.");

                    // Reverte estado de coleta
                    _isCollected = false;
                    if (_collider != null) _collider.enabled = true;

                    // TODO: Mostrar notificação "Inventário Cheio!" (Task 7)
                    return;
                }

                Debug.Log($"[ItemCollectable] Item '{inventoryItemData.itemName}' adicionado ao inventário (x{itemQuantity}).");

                // Executa efeitos visuais e sonoros
                PlayCollectionEffects();

                // Remove o item da cena
                DestroyItem();
            }
            // Fallback para sistema antigo (CollectableItemData)
            else if (itemData != null)
            {
                // Aplica efeitos do item (sistema antigo)
                ApplyItemEffects(collector);

                // Executa efeitos visuais e sonoros
                PlayCollectionEffects();

                // Remove o item da cena
                DestroyItem();
            }
            else
            {
                Debug.LogWarning("[ItemCollectable] Nenhum ItemData configurado!");
                _isCollected = false;
                if (_collider != null) _collider.enabled = true;
            }
        }

        /// <summary>
        /// Aplica todos os efeitos do item no coletor
        /// </summary>
        private void ApplyItemEffects(GameObject collector)
        {
            // Aplica cura
            if (itemData.healAmount > 0)
            {
                // TODO: Implementar sistema de vida quando disponível
                Debug.Log($"[ItemCollectable] {itemData.itemName} curou {itemData.healAmount} HP");
            }

            // Cura completa
            if (itemData.isFullHeal)
            {
                Debug.Log($"[ItemCollectable] {itemData.itemName} restaurou vida completa");
            }

            // Aplica experiência
            if (itemData.xpPoints > 0)
            {
                // TODO: Implementar sistema de experiência quando disponível
                Debug.Log($"[ItemCollectable] {itemData.itemName} deu {itemData.xpPoints} XP");
            }

            // Aplica skill points
            if (itemData.skillPoints > 0)
            {
                Debug.Log($"[ItemCollectable] {itemData.itemName} deu {itemData.skillPoints} Skill Points");
            }

            // Aplica buffs
            if (itemData.HasBuffEffect)
            {
                var buffHandler = collector.GetComponent<ItemBuffHandler>();
                if (buffHandler != null)
                {
                    buffHandler.AddBuff(itemData);
                    Debug.Log($"[ItemCollectable] {itemData.itemName} aplicou buffs");
                }
            }
        }



        /// <summary>
        /// Executa efeitos visuais e sonoros da coleta de cristais
        /// </summary>
        private void PlayCrystalCollectionEffects()
        {
            // Determina posição do efeito (preferencialmente na posição do player)
            Vector3 effectPosition = _playerTransform != null ? _playerTransform.position : transform.position;

            // Efeito de partículas (prioridade para cristal)
            GameObject vfxPrefab = null;
            if (crystalData != null && crystalData.collectVFX != null)
            {
                vfxPrefab = crystalData.collectVFX;
            }

            if (vfxPrefab != null)
            {
                var vfx = Instantiate(vfxPrefab, effectPosition, Quaternion.identity);
                if (vfx != null)
                {
                    Destroy(vfx, 2f);
                }
            }

            // Efeito sonoro (prioridade para cristal)
            AudioClip collectSound = null;
            if (crystalData != null && crystalData.collectSound != null)
            {
                collectSound = crystalData.collectSound;
            }

            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, effectPosition, 1f);
            }
        }

        /// <summary>
        /// Executa efeitos visuais e sonoros da coleta (sistema legado)
        /// </summary>
        private void PlayCollectionEffects()
        {
            // Determina posição do efeito (preferencialmente na posição do player)
            Vector3 effectPosition = _playerTransform != null ? _playerTransform.position : transform.position;

            // Efeito de partículas
            GameObject vfxPrefab = null;
            if (itemData != null && itemData.vfxPrefab != null)
            {
                vfxPrefab = itemData.vfxPrefab;
            }

            if (vfxPrefab != null)
            {
                var vfx = Instantiate(vfxPrefab, effectPosition, Quaternion.identity);

                // Auto-destruição do VFX (protegido para evitar null reference)
                if (vfx != null)
                {
                    Destroy(vfx, 2f);
                }
            }

            // Efeito sonoro
            AudioClip collectSound = null;
            if (itemData != null && itemData.collectSound != null)
            {
                collectSound = itemData.collectSound;
            }

            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, effectPosition, 1f);
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
        /// Configura dados do item em runtime
        /// </summary>
        public void SetItemData(CollectableItemData newItemData)
        {
            itemData = newItemData;
            InitializeItem();
            SetupVisuals();
        }

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
        /// Retorna dados do item
        /// </summary>
        public CollectableItemData GetItemData() => itemData;

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
                Debug.Log($"[ItemCollectable] Collider {(enabled ? "habilitado" : "desabilitado")}");
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

        #endregion

        #region Debug & Gizmos

        /// <summary>
        /// Desenha raio de atração no editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            // Verifica se tem dados válidos (prioridade para cristais)
            bool hasValidData = crystalData != null || itemData != null;
            if (!hasValidData) return;

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

        [ContextMenu("Debug Activation Status")]
        private void EditorDebugActivation()
        {
            if (!Application.isPlaying) return;

            string itemName = "Unknown";
            if (crystalData != null)
            {
                itemName = crystalData.crystalName;
            }
            else if (itemData != null)
            {
                itemName = itemData.itemName;
            }

            string status = $"[ItemCollectable] {itemName}\n";
            status += $"Crystal Data: {(crystalData != null ? crystalData.crystalType.ToString() : "None")}\n";
            status += $"Item Data: {(itemData != null ? "Present" : "None")}\n";
            status += $"Collected: {_isCollected}\n";
            status += $"Attraction Enabled: {enableAttraction}\n";
            status += $"Attraction Active: {IsAttractionActive}\n";
            status += $"Time Remaining: {GetRemainingActivationTime():F1}s\n";
            status += $"Being Attracted: {_isBeingAttracted}\n";
            status += $"Collider Enabled: {(_collider != null ? _collider.enabled : false)}";

            Debug.Log(status);
        }
#endif

        #endregion
    }
}
