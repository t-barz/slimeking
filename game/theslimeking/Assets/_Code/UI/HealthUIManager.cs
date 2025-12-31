using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SlimeKing.Gameplay;

namespace SlimeKing.Core.UI
{
    /// <summary>
    /// Gerenciador principal do HUD de vida do jogador.
    /// Responsável por criar, organizar e atualizar os corações baseado no estado de vida.
    /// </summary>
    public class HealthUIManager : MonoBehaviour
    {
        #region Inspector Configuration

        [Header("Configuração de Sprites")]
        [SerializeField] private Sprite heartFullSprite;
        [SerializeField] private Sprite heartEmptySprite;

        [Header("Configuração de Layout")]
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private Transform heartsContainer;
        [SerializeField] private int heartsPerRow = 10;
        [SerializeField] private float heartSpacing = 35f;
        [SerializeField] private float rowSpacing = 35f;

        [Header("Configurações Automáticas")]
        [SerializeField] private bool findPlayerAutomatically = true;
        [SerializeField] private PlayerAttributesHandler targetPlayer;

        [Header("Configurações de Debug")]
        [SerializeField] private bool enableLogs = false;

        #endregion

        #region Private Variables

        private List<HeartUIElement> _heartElements = new List<HeartUIElement>();
        private int _currentMaxHearts = 0;
        private int _currentHearts = 0;
        private bool _isInitialized = false;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Valida configuração básica
            ValidateConfiguration();
        }

        private void Start()
        {
            // Tenta encontrar o jogador automaticamente se necessário
            if (findPlayerAutomatically && targetPlayer == null)
            {
                FindPlayerComponent();
            }

            // Inicializa o sistema
            InitializeHealthUI();
        }

        private void OnDestroy()
        {
            // Remove listeners de eventos
            UnsubscribeFromPlayerEvents();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Define manualmente o componente PlayerAttributesHandler.
        /// </summary>
        /// <param name="playerComponent">Componente do jogador</param>
        public void SetPlayerComponent(PlayerAttributesHandler playerComponent)
        {
            if (targetPlayer != null)
            {
                UnsubscribeFromPlayerEvents();
            }

            targetPlayer = playerComponent;

            if (targetPlayer != null)
            {
                SubscribeToPlayerEvents();
                UpdateHeartDisplay(targetPlayer.CurrentHealthPoints, targetPlayer.MaxHealthPoints);
            }

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[HealthUIManager] PlayerAttributesHandler definido: {(targetPlayer != null ? "Conectado" : "Desconectado")}");
            }
        }

        /// <summary>
        /// Força atualização do display de vida.
        /// </summary>
        public void ForceUpdateDisplay()
        {
            if (targetPlayer != null)
            {
                UpdateHeartDisplay(targetPlayer.CurrentHealthPoints, targetPlayer.MaxHealthPoints);
            }
        }

        /// <summary>
        /// Reconfigura o layout dos corações.
        /// </summary>
        /// <param name="maxHearts">Novo número máximo de corações</param>
        public void ReconfigureHearts(int maxHearts)
        {
            if (maxHearts <= 0) return;

            _currentMaxHearts = maxHearts;
            CreateHeartElements();

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[HealthUIManager] Layout reconfigurado para {maxHearts} corações");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Valida se a configuração inicial está correta.
        /// </summary>
        private void ValidateConfiguration()
        {
            if (heartsContainer == null)
            {
                UnityEngine.Debug.LogError("[HealthUIManager] Container de corações não configurado!");
                return;
            }

            if (heartPrefab == null)
            {
                UnityEngine.Debug.LogError("[HealthUIManager] Prefab de coração não configurado!");
                return;
            }

            if (heartFullSprite == null || heartEmptySprite == null)
            {
                UnityEngine.Debug.LogError("[HealthUIManager] Sprites de coração não configurados!");
                return;
            }

            HeartUIElement prefabHeart = heartPrefab.GetComponent<HeartUIElement>();
            if (prefabHeart == null)
            {
                UnityEngine.Debug.LogError("[HealthUIManager] Prefab deve conter o componente HeartUIElement!");
                return;
            }
        }

        /// <summary>
        /// Tenta encontrar automaticamente o PlayerAttributesHandler na cena.
        /// </summary>
        private void FindPlayerComponent()
        {
            PlayerAttributesHandler player = FindFirstObjectByType<PlayerAttributesHandler>();

            if (player != null)
            {
                targetPlayer = player;

                if (enableLogs)
                {
                    UnityEngine.Debug.Log($"[HealthUIManager] PlayerAttributesHandler encontrado automaticamente: {player.gameObject.name}");
                }
            }
            else if (enableLogs)
            {
                UnityEngine.Debug.LogWarning("[HealthUIManager] PlayerAttributesHandler não encontrado na cena!");
            }
        }

        /// <summary>
        /// Inicializa o sistema de HUD de vida.
        /// </summary>
        private void InitializeHealthUI()
        {
            if (_isInitialized) return;

            if (targetPlayer == null)
            {
                if (enableLogs)
                {
                    UnityEngine.Debug.LogWarning("[HealthUIManager] Não é possível inicializar sem PlayerAttributesHandler!");
                }
                return;
            }

            // Subscreve aos eventos do jogador
            SubscribeToPlayerEvents();

            // Configura os corações baseado na vida máxima
            _currentMaxHearts = targetPlayer.MaxHealthPoints;
            CreateHeartElements();

            // Atualiza display inicial
            UpdateHeartDisplay(targetPlayer.CurrentHealthPoints, targetPlayer.MaxHealthPoints);

            _isInitialized = true;

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[HealthUIManager] Sistema inicializado com {_currentMaxHearts} corações");
            }
        }

        /// <summary>
        /// Subscreve aos eventos do PlayerAttributesHandler.
        /// </summary>
        private void SubscribeToPlayerEvents()
        {
            if (targetPlayer == null) return;

            targetPlayer.OnHealthChanged += OnPlayerHealthChanged;
        }

        /// <summary>
        /// Remove subscrição dos eventos do PlayerAttributesHandler.
        /// </summary>
        private void UnsubscribeFromPlayerEvents()
        {
            if (targetPlayer == null) return;

            targetPlayer.OnHealthChanged -= OnPlayerHealthChanged;
        }

        /// <summary>
        /// Callback para quando a vida do jogador muda.
        /// </summary>
        /// <param name="currentHealth">Vida atual</param>
        /// <param name="maxHealth">Vida máxima</param>
        private void OnPlayerHealthChanged(int currentHealth, int maxHealth)
        {
            // Verifica se o número máximo de corações mudou
            if (maxHealth != _currentMaxHearts)
            {
                _currentMaxHearts = maxHealth;
                CreateHeartElements();
            }

            UpdateHeartDisplay(currentHealth, maxHealth);

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[HealthUIManager] Vida alterada: {currentHealth}/{maxHealth}");
            }
        }

        /// <summary>
        /// Cria os elementos de coração no UI.
        /// </summary>
        private void CreateHeartElements()
        {
            // Remove corações existentes
            ClearExistingHearts();

            // Cria novos corações
            for (int i = 0; i < _currentMaxHearts; i++)
            {
                CreateSingleHeart(i);
            }

            if (enableLogs)
            {
                UnityEngine.Debug.Log($"[HealthUIManager] {_currentMaxHearts} corações criados");
            }
        }

        /// <summary>
        /// Remove todos os corações existentes.
        /// </summary>
        private void ClearExistingHearts()
        {
            foreach (HeartUIElement heart in _heartElements)
            {
                if (heart != null && heart.gameObject != null)
                {
                    DestroyImmediate(heart.gameObject);
                }
            }

            _heartElements.Clear();
        }

        /// <summary>
        /// Cria um único coração e o posiciona no layout.
        /// </summary>
        /// <param name="index">Índice do coração</param>
        private void CreateSingleHeart(int index)
        {
            GameObject heartObj = Instantiate(heartPrefab, heartsContainer);
            HeartUIElement heartElement = heartObj.GetComponent<HeartUIElement>();

            if (heartElement != null)
            {
                // Configura os sprites
                heartElement.SetSprites(heartFullSprite, heartEmptySprite);

                // Posiciona o coração
                PositionHeart(heartObj, index);

                // Adiciona à lista
                _heartElements.Add(heartElement);
            }
        }

        /// <summary>
        /// Posiciona um coração no layout baseado no seu índice.
        /// </summary>
        /// <param name="heartObj">GameObject do coração</param>
        /// <param name="index">Índice do coração</param>
        private void PositionHeart(GameObject heartObj, int index)
        {
            RectTransform heartRect = heartObj.GetComponent<RectTransform>();

            if (heartRect == null) return;

            // Calcula posição em grid
            int row = index / heartsPerRow;
            int col = index % heartsPerRow;

            // Posiciona
            Vector2 position = new Vector2(col * heartSpacing, -row * rowSpacing);
            heartRect.anchoredPosition = position;
        }

        /// <summary>
        /// Atualiza o display visual dos corações.
        /// </summary>
        /// <param name="currentHealth">Vida atual</param>
        /// <param name="maxHealth">Vida máxima</param>
        private void UpdateHeartDisplay(int currentHealth, int maxHealth)
        {
            _currentHearts = currentHealth;

            for (int i = 0; i < _heartElements.Count; i++)
            {
                if (_heartElements[i] != null)
                {
                    bool shouldBeFull = i < currentHealth;
                    bool wasChanged = _heartElements[i].IsFull != shouldBeFull;

                    _heartElements[i].IsFull = shouldBeFull;

                    // Reproduz animação se o estado mudou
                    if (wasChanged)
                    {
                        _heartElements[i].PlayBounceAnimation();
                    }
                }
            }
        }

        #endregion
    }
}
