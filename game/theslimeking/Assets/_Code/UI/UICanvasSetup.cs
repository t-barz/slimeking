using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SlimeKing.UI
{
    /// <summary>
    /// Script para criar e configurar um Canvas completo com HealthDisplay, FragmentDisplay e QuickSlotsDisplay.
    /// Baseado na estrutura dos worklogs e nos scripts de UI existentes.
    /// </summary>
    public class UICanvasSetup : MonoBehaviour
    {
        #region Fields
        [Header("Prefab References")]
        [SerializeField] private GameObject heartPrefab;
        [SerializeField] private GameObject fragmentPrefab;
        [SerializeField] private GameObject quickSlotPrefab;

        [Header("Canvas Settings")]
        [SerializeField] private int sortingOrder = 0;
        [SerializeField] private string canvasName = "GameHUD";
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            CreateCompleteUISystem();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Cria o sistema completo de UI
        /// </summary>
        [ContextMenu("Create Complete UI System")]
        public void CreateCompleteUISystem()
        {
            GameObject canvas = CreateMainCanvas();
            if (canvas != null)
            {
                CreateHealthDisplay(canvas);
                CreateFragmentDisplay(canvas);
                CreateQuickSlotsDisplay(canvas);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Cria o Canvas principal
        /// </summary>
        private GameObject CreateMainCanvas()
        {
            // Verifica se já existe um canvas
            GameObject existingCanvas = GameObject.Find(canvasName);
            if (existingCanvas != null)
            {
                return existingCanvas;
            }

            // Cria o GameObject do Canvas
            GameObject canvasGO = new GameObject(canvasName);
            
            // Adiciona o componente Canvas
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            // Adiciona o CanvasScaler
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            // Adiciona o GraphicRaycaster
            canvasGO.AddComponent<GraphicRaycaster>();

            // Cria EventSystem se não existir
            CreateEventSystemIfNeeded();

            return canvasGO;
        }

        /// <summary>
        /// Cria EventSystem se necessário
        /// </summary>
        private void CreateEventSystemIfNeeded()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
            }
        }

        /// <summary>
        /// Cria o HealthDisplay no canto superior esquerdo
        /// </summary>
        private void CreateHealthDisplay(GameObject canvas)
        {
            GameObject healthDisplayGO = new GameObject("HealthDisplay");
            healthDisplayGO.transform.SetParent(canvas.transform, false);

            // Configura RectTransform
            RectTransform rectTransform = healthDisplayGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f); // Top-left
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = new Vector2(100f, -50f);
            rectTransform.sizeDelta = new Vector2(300f, 100f);

            // Adiciona o componente HealthDisplay
            HealthDisplay healthDisplay = healthDisplayGO.AddComponent<HealthDisplay>();
            
            // Configura o prefab se disponível
            if (heartPrefab != null)
            {
                // Usa reflection para configurar o prefab privado
                var heartPrefabField = typeof(HealthDisplay).GetField("heartPrefab", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                heartPrefabField?.SetValue(healthDisplay, heartPrefab);
            }
        }

        /// <summary>
        /// Cria o FragmentDisplay no canto superior direito
        /// </summary>
        private void CreateFragmentDisplay(GameObject canvas)
        {
            GameObject fragmentDisplayGO = new GameObject("FragmentDisplay");
            fragmentDisplayGO.transform.SetParent(canvas.transform, false);

            // Configura RectTransform
            RectTransform rectTransform = fragmentDisplayGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1f, 1f); // Top-right
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.anchoredPosition = new Vector2(-100f, -50f);
            rectTransform.sizeDelta = new Vector2(400f, 100f);

            // Adiciona o componente FragmentDisplay
            FragmentDisplay fragmentDisplay = fragmentDisplayGO.AddComponent<FragmentDisplay>();

            // Configura os tipos de fragmento se o prefab estiver disponível
            if (fragmentPrefab != null)
            {
                SetupFragmentTypes(fragmentDisplay);
            }
        }

        /// <summary>
        /// Cria o QuickSlotsDisplay na parte inferior central
        /// </summary>
        private void CreateQuickSlotsDisplay(GameObject canvas)
        {
            GameObject quickSlotsGO = new GameObject("QuickSlotsDisplay");
            quickSlotsGO.transform.SetParent(canvas.transform, false);

            // Configura RectTransform
            RectTransform rectTransform = quickSlotsGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0f); // Bottom-center
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 100f);
            rectTransform.sizeDelta = new Vector2(400f, 100f);

            // Adiciona o componente QuickSlotsDisplay
            QuickSlotsDisplay quickSlotsDisplay = quickSlotsGO.AddComponent<QuickSlotsDisplay>();

            // Configura o prefab se disponível
            if (quickSlotPrefab != null)
            {
                var quickSlotPrefabField = typeof(QuickSlotsDisplay).GetField("quickSlotPrefab", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                quickSlotPrefabField?.SetValue(quickSlotsDisplay, quickSlotPrefab);
            }
        }

        /// <summary>
        /// Configura os tipos de fragmento padrão
        /// </summary>
        private void SetupFragmentTypes(FragmentDisplay fragmentDisplay)
        {
            if (fragmentPrefab == null) return;

            // Usa reflection para acessar a lista privada de fragmentTypes
            var fragmentTypesField = typeof(FragmentDisplay).GetField("fragmentTypes", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (fragmentTypesField != null)
            {
                var fragmentTypesList = fragmentTypesField.GetValue(fragmentDisplay) as System.Collections.IList;
                
                if (fragmentTypesList != null)
                {
                    // Limpa a lista existente
                    fragmentTypesList.Clear();

                    // Adiciona os tipos de fragmento padrão
                    string[] elementNames = { "Earth", "Nature", "Water", "Air", "Dark", "Fire" };
                    
                    foreach (var elementName in elementNames)
                    {
                        // Cria uma nova instância de FragmentType usando reflection
                        var fragmentTypeClass = typeof(FragmentDisplay).GetNestedType("FragmentType");
                        if (fragmentTypeClass != null)
                        {
                            var fragmentTypeInstance = System.Activator.CreateInstance(fragmentTypeClass);
                            
                            // Define o nome do elemento
                            var elementNameField = fragmentTypeClass.GetField("elementName");
                            elementNameField?.SetValue(fragmentTypeInstance, elementName);
                            
                            // Define o prefab
                            var prefabField = fragmentTypeClass.GetField("prefab");
                            prefabField?.SetValue(fragmentTypeInstance, fragmentPrefab);
                            
                            // Adiciona à lista
                            fragmentTypesList.Add(fragmentTypeInstance);
                        }
                    }
                }
            }
        }
        #endregion
    }
}