using UnityEngine;
using SlimeKing.UI;

namespace SlimeKing.UI
{
    /// <summary>
    /// Manager principal para o sistema de UI do jogo.
    /// Gerencia HealthDisplay, FragmentDisplay e QuickSlotsDisplay.
    /// Segue o padrão Singleton para acesso global.
    /// </summary>
    public class UISystemManager : MonoBehaviour
    {
        #region Fields
        public static UISystemManager Instance { get; private set; }

        [Header("UI Components")]
        [SerializeField] private HealthDisplay healthDisplay;
        [SerializeField] private FragmentDisplay fragmentDisplay;
        [SerializeField] private QuickSlotsDisplay quickSlotsDisplay;

        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeUIComponents();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Define o número atual de vidas
        /// </summary>
        public void SetHealth(int currentHearts)
        {
            if (healthDisplay != null)
            {
                healthDisplay.SetCurrentHearts(currentHearts);
            }
        }

        /// <summary>
        /// Adiciona vidas
        /// </summary>
        public void AddHealth(int amount)
        {
            if (healthDisplay != null)
            {
                healthDisplay.AddHearts(amount);
            }
        }

        /// <summary>
        /// Remove vidas
        /// </summary>
        public void RemoveHealth(int amount)
        {
            if (healthDisplay != null)
            {
                healthDisplay.RemoveHearts(amount);
            }
        }

        /// <summary>
        /// Aumenta o máximo de vidas
        /// </summary>
        public void IncreaseMaxHealth(int amount)
        {
            if (healthDisplay != null)
            {
                healthDisplay.IncreaseMaxHearts(amount);
            }
        }

        /// <summary>
        /// Adiciona fragmentos de um elemento
        /// </summary>
        public void AddFragments(string elementName, int amount)
        {
            if (fragmentDisplay != null)
            {
                fragmentDisplay.AddFragments(elementName, amount);
            }
        }

        /// <summary>
        /// Remove fragmentos de um elemento
        /// </summary>
        public void RemoveFragments(string elementName, int amount)
        {
            if (fragmentDisplay != null)
            {
                fragmentDisplay.RemoveFragments(elementName, amount);
            }
        }

        /// <summary>
        /// Define a quantidade de fragmentos de um elemento
        /// </summary>
        public void SetFragments(string elementName, int amount)
        {
            if (fragmentDisplay != null)
            {
                fragmentDisplay.SetFragments(elementName, amount);
            }
        }

        /// <summary>
        /// Obtém a quantidade de fragmentos de um elemento
        /// </summary>
        public int GetFragments(string elementName)
        {
            if (fragmentDisplay != null)
            {
                return fragmentDisplay.GetFragments(elementName);
            }
            return 0;
        }

        /// <summary>
        /// Define o conteúdo de um quick slot
        /// </summary>
        public void SetQuickSlot(int slotIndex, Sprite icon, string itemName)
        {
            if (quickSlotsDisplay != null)
            {
                quickSlotsDisplay.SetSlotContent(slotIndex, icon, itemName);
            }
        }

        /// <summary>
        /// Limpa um quick slot
        /// </summary>
        public void ClearQuickSlot(int slotIndex)
        {
            if (quickSlotsDisplay != null)
            {
                quickSlotsDisplay.ClearSlot(slotIndex);
            }
        }

        /// <summary>
        /// Limpa todos os quick slots
        /// </summary>
        public void ClearAllQuickSlots()
        {
            if (quickSlotsDisplay != null)
            {
                quickSlotsDisplay.ClearAllSlots();
            }
        }

        /// <summary>
        /// Testa o sistema de UI com valores de exemplo
        /// </summary>
        [ContextMenu("Test UI System")]
        public void TestUISystem()
        {
            // Testa Health Display
            SetHealth(3);
            
            // Testa Fragment Display
            AddFragments("Fire", 5);
            AddFragments("Water", 3);
            AddFragments("Earth", 7);
            
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Inicializa os componentes de UI
        /// </summary>
        private void InitializeUIComponents()
        {
            // Encontra os componentes automaticamente se não foram atribuídos
            if (healthDisplay == null)
                healthDisplay = FindObjectOfType<HealthDisplay>();
                
            if (fragmentDisplay == null)
                fragmentDisplay = FindObjectOfType<FragmentDisplay>();
                
            if (quickSlotsDisplay == null)
                quickSlotsDisplay = FindObjectOfType<QuickSlotsDisplay>();

        }
        #endregion

        #region Utility Methods

        #endregion
    }
}