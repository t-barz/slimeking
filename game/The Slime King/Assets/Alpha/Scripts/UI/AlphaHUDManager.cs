using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using SlimeMec.Alpha.Inventory;
using SlimeMec.Alpha.Progression;

namespace Alpha.UI
{
    /// <summary>
    /// Gerenciador unificado do HUD Alpha
    /// Controla vida, inventário, progressão e feedback visual
    /// </summary>
    public class AlphaHUDManager : MonoBehaviour
    {
        #region Singleton
        public static AlphaHUDManager Instance { get; private set; }
        #endregion

        #region Serialized Fields
        [Header("Health UI")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private Image healthFillImage;
        [SerializeField] private Color healthColor = Color.green;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private float lowHealthThreshold = 0.3f;

        [Header("Inventory UI")]
        [SerializeField] private Transform inventoryContainer;
        [SerializeField] private InventoryHUD inventoryHUD;
        [SerializeField] private bool showInventorySlots = true;

        [Header("Progression UI")]
        [SerializeField] private Slider experienceSlider;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private TextMeshProUGUI skillPointsText;
        [SerializeField] private bool showProgressionInfo = true;

        [Header("Notifications")]
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform notificationContainer;
        [SerializeField] private float notificationDuration = 3f;
        [SerializeField] private bool enableNotifications = true;

        [Header("Visual Settings")]
        [SerializeField] private CanvasGroup hudCanvasGroup;
        [SerializeField] private bool hideHUDInDialogue = true;
        [SerializeField] private float fadeSpeed = 5f;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private TextMeshProUGUI debugText;
        #endregion

        #region Private Fields
        private float currentHealth = 100f;
        private float maxHealth = 100f;
        private bool isInitialized = false;
        private bool isHUDVisible = true;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeSingleton();
        }

        private void Start()
        {
            InitializeHUD();
            SetupEventListeners();
        }

        private void Update()
        {
            UpdateHUDVisibility();
            if (showDebugInfo) UpdateDebugInfo();
        }

        private void OnDestroy()
        {
            CleanupEventListeners();
        }
        #endregion

        #region Initialization
        private void InitializeSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
                // Não usar DontDestroyOnLoad para HUD (cada cena pode ter seu próprio)
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeHUD()
        {
            // Inicializa components de saúde
            InitializeHealthUI();

            // Inicializa inventário
            InitializeInventoryUI();

            // Inicializa progressão
            InitializeProgressionUI();

            // Configura notificações
            InitializeNotificationSystem();

            isInitialized = true;

            if (enableDebugLogs)
                Debug.Log("[AlphaHUDManager] HUD initialized successfully");
        }

        private void InitializeHealthUI()
        {
            if (healthSlider != null)
            {
                healthSlider.minValue = 0f;
                healthSlider.maxValue = 1f;
                healthSlider.value = 1f;
            }

            if (healthFillImage != null)
            {
                healthFillImage.color = healthColor;
            }

            UpdateHealthDisplay(maxHealth, maxHealth);
        }

        private void InitializeInventoryUI()
        {
            if (inventoryHUD == null && inventoryContainer != null)
            {
                inventoryHUD = inventoryContainer.GetComponentInChildren<InventoryHUD>();
            }

            if (inventoryHUD != null)
            {
                inventoryHUD.gameObject.SetActive(showInventorySlots);
            }
        }

        private void InitializeProgressionUI()
        {
            if (experienceSlider != null)
            {
                experienceSlider.minValue = 0f;
                experienceSlider.maxValue = 1f;
                experienceSlider.value = 0f;
            }

            UpdateProgressionDisplay();
        }

        private void InitializeNotificationSystem()
        {
            if (notificationContainer == null)
            {
                // Cria container se não existir
                GameObject containerObj = new GameObject("NotificationContainer");
                containerObj.transform.SetParent(transform);
                notificationContainer = containerObj.transform;
            }
        }

        private void SetupEventListeners()
        {
            // Eventos de inventário
            if (InventoryCore.Instance != null)
            {
                InventoryCore.OnInventoryChanged += HandleInventoryChanged;
                InventoryCore.OnItemAdded += HandleItemAdded;
            }

            // Eventos de crescimento
            if (GrowthSystem.Instance != null)
            {
                GrowthSystem.OnStageChanged += HandleStageChanged;
                GrowthSystem.OnExperienceGained += HandleExperienceGained;
            }

            // Eventos de skill tree
            if (SkillTreeManager.Instance != null)
            {
                SkillTreeManager.OnSkillUnlocked += HandleSkillUnlocked;
                SkillTreeManager.OnSkillPointsChanged += HandleSkillPointsChanged;
            }

            // Eventos de uso de itens
            ItemUsageManager.OnItemUsed += HandleItemUsed;
            ItemUsageManager.OnItemEffect += HandleItemEffect;

            // TODO: Eventos de atributos do player
            // PlayerAttributesSystem.OnHealthChanged += HandleHealthChanged;
        }

        private void CleanupEventListeners()
        {
            if (InventoryCore.Instance != null)
            {
                InventoryCore.OnInventoryChanged -= HandleInventoryChanged;
                InventoryCore.OnItemAdded -= HandleItemAdded;
            }

            if (GrowthSystem.Instance != null)
            {
                GrowthSystem.OnStageChanged -= HandleStageChanged;
                GrowthSystem.OnExperienceGained -= HandleExperienceGained;
            }

            if (SkillTreeManager.Instance != null)
            {
                SkillTreeManager.OnSkillUnlocked -= HandleSkillUnlocked;
                SkillTreeManager.OnSkillPointsChanged -= HandleSkillPointsChanged;
            }

            ItemUsageManager.OnItemUsed -= HandleItemUsed;
            ItemUsageManager.OnItemEffect -= HandleItemEffect;
        }
        #endregion

        #region Health Management
        public void UpdateHealthDisplay(float current, float max)
        {
            currentHealth = current;
            maxHealth = max;

            if (healthSlider != null)
            {
                healthSlider.value = max > 0 ? current / max : 0f;
            }

            if (healthText != null)
            {
                healthText.text = $"{current:F0}/{max:F0}";
            }

            // Atualiza cor baseada na porcentagem
            if (healthFillImage != null)
            {
                float healthPercentage = max > 0 ? current / max : 0f;
                Color targetColor = healthPercentage <= lowHealthThreshold ? lowHealthColor : healthColor;
                healthFillImage.color = Color.Lerp(healthFillImage.color, targetColor, Time.deltaTime * fadeSpeed);
            }
        }

        public void SetMaxHealth(float newMaxHealth)
        {
            maxHealth = newMaxHealth;
            UpdateHealthDisplay(currentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(0f, currentHealth - damage);
            UpdateHealthDisplay(currentHealth, maxHealth);

            if (enableNotifications)
            {
                ShowNotification($"-{damage:F0} HP", Color.red);
            }
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            UpdateHealthDisplay(currentHealth, maxHealth);

            if (enableNotifications)
            {
                ShowNotification($"+{amount:F0} HP", Color.green);
            }
        }
        #endregion

        #region Progression Display
        private void UpdateProgressionDisplay()
        {
            if (!showProgressionInfo) return;

            // Atualiza estágio
            if (stageText != null && GrowthSystem.Instance != null)
            {
                var currentStage = GrowthSystem.Instance.GetCurrentStage();
                stageText.text = $"Stage: {currentStage}";
            }

            // Atualiza experiência
            if (experienceSlider != null && GrowthSystem.Instance != null)
            {
                float progress = GrowthSystem.Instance.GetEvolutionProgress();
                experienceSlider.value = progress;
            }

            if (experienceText != null && GrowthSystem.Instance != null)
            {
                float currentXP = GrowthSystem.Instance.GetCurrentExperience();
                float nextXP = GrowthSystem.Instance.GetExperienceToNextStage();
                experienceText.text = $"XP: {currentXP:F0}/{nextXP:F0}";
            }

            // Atualiza pontos de skill
            if (skillPointsText != null && SkillTreeManager.Instance != null)
            {
                int skillPoints = SkillTreeManager.Instance.GetSkillPoints();
                skillPointsText.text = $"SP: {skillPoints}";
            }
        }
        #endregion

        #region Notification System
        public void ShowNotification(string message, Color color, float? duration = null)
        {
            if (!enableNotifications || notificationContainer == null) return;

            // TODO: Implementar sistema de notificação visual
            // Por enquanto, apenas log
            if (enableDebugLogs)
                Debug.Log($"[AlphaHUDManager] Notification: {message}");
        }

        public void ShowItemCollectedNotification(string itemName, int quantity)
        {
            if (enableNotifications)
            {
                string message = quantity > 1 ? $"+{quantity} {itemName}" : $"+{itemName}";
                ShowNotification(message, Color.cyan);
            }
        }

        public void ShowExperienceGainedNotification(float xpAmount)
        {
            if (enableNotifications)
            {
                ShowNotification($"+{xpAmount:F0} XP", Color.yellow);
            }
        }
        #endregion

        #region HUD Visibility
        public void SetHUDVisible(bool visible, bool animated = true)
        {
            isHUDVisible = visible;

            if (!animated)
            {
                if (hudCanvasGroup != null)
                {
                    hudCanvasGroup.alpha = visible ? 1f : 0f;
                }
            }
        }

        private void UpdateHUDVisibility()
        {
            if (hudCanvasGroup == null) return;

            float targetAlpha = isHUDVisible ? 1f : 0f;
            float currentAlpha = hudCanvasGroup.alpha;

            if (Mathf.Abs(currentAlpha - targetAlpha) > 0.01f)
            {
                hudCanvasGroup.alpha = Mathf.Lerp(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
            }
        }
        #endregion

        #region Event Handlers
        private void HandleInventoryChanged()
        {
            // O InventoryHUD já se atualiza sozinho
            if (enableDebugLogs)
                Debug.Log("[AlphaHUDManager] Inventory changed");
        }

        private void HandleItemAdded(InventoryItem item)
        {
            ShowItemCollectedNotification(item.displayName, item.quantity);
        }

        private void HandleStageChanged(GrowthStage from, GrowthStage to)
        {
            UpdateProgressionDisplay();

            if (enableNotifications)
            {
                ShowNotification($"Stage Up! {to}", Color.magenta);
            }
        }

        private void HandleExperienceGained(float amount)
        {
            UpdateProgressionDisplay();
            ShowExperienceGainedNotification(amount);
        }

        private void HandleSkillUnlocked(SkillNode skill)
        {
            if (enableNotifications)
            {
                ShowNotification($"Skill Unlocked: {skill.displayName}", Color.blue);
            }

            UpdateProgressionDisplay();
        }

        private void HandleSkillPointsChanged(int newAmount)
        {
            UpdateProgressionDisplay();
        }

        private void HandleItemUsed(int slotIndex, InventoryItem item)
        {
            if (enableNotifications)
            {
                ShowNotification($"Used: {item.displayName}", Color.orange);
            }
        }

        private void HandleItemEffect(string effectType, float value)
        {
            switch (effectType)
            {
                case "health":
                    Heal(value);
                    break;

                case "mana":
                    // TODO: Implementar sistema de mana
                    if (enableNotifications)
                        ShowNotification($"+{value:F0} MP", Color.blue);
                    break;

                case "speed_boost":
                    if (enableNotifications)
                        ShowNotification($"Speed Boost ({value:F0}s)", Color.cyan);
                    break;

                default:
                    if (enableDebugLogs)
                        Debug.Log($"[AlphaHUDManager] Unknown effect: {effectType} = {value}");
                    break;
            }
        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Força atualização de todos os elementos do HUD
        /// </summary>
        public void RefreshAllDisplays()
        {
            UpdateHealthDisplay(currentHealth, maxHealth);
            UpdateProgressionDisplay();

            if (inventoryHUD != null)
            {
                inventoryHUD.ForceRefresh();
            }
        }

        /// <summary>
        /// Obtém referência do InventoryHUD
        /// </summary>
        public InventoryHUD GetInventoryHUD() => inventoryHUD;

        /// <summary>
        /// Verifica se o HUD está visível
        /// </summary>
        public bool IsHUDVisible() => isHUDVisible;
        #endregion

        #region Debug
        private void UpdateDebugInfo()
        {
            if (debugText == null) return;

            string debug = $"=== HUD DEBUG ===\n";
            debug += $"Health: {currentHealth:F1}/{maxHealth:F1}\n";
            debug += $"HUD Visible: {isHUDVisible}\n";

            if (InventoryCore.Instance != null)
            {
                debug += $"Inventory Items: {InventoryCore.Instance.GetAllItems().Count}\n";
            }

            if (GrowthSystem.Instance != null)
            {
                debug += $"Stage: {GrowthSystem.Instance.GetCurrentStage()}\n";
                debug += $"XP: {GrowthSystem.Instance.GetCurrentExperience():F0}\n";
            }

            if (SkillTreeManager.Instance != null)
            {
                debug += $"Skill Points: {SkillTreeManager.Instance.GetSkillPoints()}\n";
            }

            debugText.text = debug;
        }

        [ContextMenu("Debug - Test Damage")]
        private void DebugTestDamage()
        {
            TakeDamage(25f);
        }

        [ContextMenu("Debug - Test Heal")]
        private void DebugTestHeal()
        {
            Heal(30f);
        }

        [ContextMenu("Debug - Test Notification")]
        private void DebugTestNotification()
        {
            ShowNotification("Test Notification", Color.white);
        }
        #endregion
    }
}