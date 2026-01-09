using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace SlimeKing.UI
{
    /// <summary>
    /// Script de configuração rápida para criar todo o sistema de UI em uma cena.
    /// Cria Canvas, prefabs básicos e componentes necessários seguindo os padrões do projeto.
    /// </summary>
    public class QuickUISetup : MonoBehaviour
    {
        #region Fields
        [Header("Canvas Settings")]
        [SerializeField] private string canvasName = "GameHUD";
        [SerializeField] private int sortingOrder = 0;
        #endregion

        #region Public Methods
        /// <summary>
        /// Cria todo o sistema de UI de uma vez
        /// </summary>
        [ContextMenu("Setup Complete UI System")]
        public void SetupCompleteUISystem()
        {
            // 1. Cria o Canvas principal
            GameObject canvas = CreateMainCanvas();
            
            // 2. Cria os prefabs necessários
            GameObject heartPrefab = CreateHeartPrefab();
            GameObject fragmentPrefab = CreateFragmentPrefab();
            GameObject quickSlotPrefab = CreateQuickSlotPrefab();
            
            // 3. Cria os componentes de UI
            CreateHealthDisplay(canvas, heartPrefab);
            CreateFragmentDisplay(canvas, fragmentPrefab);
            CreateQuickSlotsDisplay(canvas, quickSlotPrefab);
            
            // 4. Cria o UISystemManager
            CreateUISystemManager(canvas);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Cria o Canvas principal
        /// </summary>
        private GameObject CreateMainCanvas()
        {
            // Verifica se já existe
            GameObject existingCanvas = GameObject.Find(canvasName);
            if (existingCanvas != null)
            {
                return existingCanvas;
            }

            // Cria novo Canvas
            GameObject canvasGO = new GameObject(canvasName);
            
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // EventSystem
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
            }

            return canvasGO;
        }

        /// <summary>
        /// Cria um prefab de coração simples
        /// </summary>
        private GameObject CreateHeartPrefab()
        {
            GameObject heartGO = new GameObject("HeartPrefab");
            
            RectTransform rectTransform = heartGO.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(60f, 60f);
            
            Image heartImage = heartGO.AddComponent<Image>();
            heartImage.color = Color.red;
            
            return heartGO;
        }

        /// <summary>
        /// Cria um prefab de fragmento simples
        /// </summary>
        private GameObject CreateFragmentPrefab()
        {
            GameObject fragmentGO = new GameObject("FragmentPrefab");
            
            RectTransform rectTransform = fragmentGO.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80f, 80f);
            
            Image fragmentImage = fragmentGO.AddComponent<Image>();
            fragmentImage.color = Color.cyan;
            
            // Contador
            GameObject counterGO = new GameObject("Counter");
            counterGO.transform.SetParent(fragmentGO.transform, false);
            
            RectTransform counterRect = counterGO.AddComponent<RectTransform>();
            counterRect.anchorMin = new Vector2(0.5f, 0f);
            counterRect.anchorMax = new Vector2(0.5f, 0f);
            counterRect.anchoredPosition = new Vector2(0f, -10f);
            counterRect.sizeDelta = new Vector2(60f, 30f);
            
            TextMeshProUGUI counterText = counterGO.AddComponent<TextMeshProUGUI>();
            counterText.text = "0";
            counterText.fontSize = 18f;
            counterText.color = Color.white;
            counterText.alignment = TextAlignmentOptions.Center;
            counterText.fontStyle = FontStyles.Bold;
            
            return fragmentGO;
        }

        /// <summary>
        /// Cria um prefab de quick slot simples
        /// </summary>
        private GameObject CreateQuickSlotPrefab()
        {
            GameObject slotGO = new GameObject("QuickSlotPrefab");
            
            RectTransform rectTransform = slotGO.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80f, 80f);
            
            Image slotImage = slotGO.AddComponent<Image>();
            slotImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            return slotGO;
        }

        /// <summary>
        /// Cria o HealthDisplay
        /// </summary>
        private void CreateHealthDisplay(GameObject canvas, GameObject heartPrefab)
        {
            GameObject healthDisplayGO = new GameObject("HealthDisplay");
            healthDisplayGO.transform.SetParent(canvas.transform, false);

            RectTransform rectTransform = healthDisplayGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = new Vector2(100f, -50f);
            rectTransform.sizeDelta = new Vector2(300f, 100f);

            HealthDisplay healthDisplay = healthDisplayGO.AddComponent<HealthDisplay>();
            
            // Configura o prefab usando reflection
            var heartPrefabField = typeof(HealthDisplay).GetField("heartPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            heartPrefabField?.SetValue(healthDisplay, heartPrefab);
        }

        /// <summary>
        /// Cria o FragmentDisplay
        /// </summary>
        private void CreateFragmentDisplay(GameObject canvas, GameObject fragmentPrefab)
        {
            GameObject fragmentDisplayGO = new GameObject("FragmentDisplay");
            fragmentDisplayGO.transform.SetParent(canvas.transform, false);

            RectTransform rectTransform = fragmentDisplayGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.anchoredPosition = new Vector2(-100f, -50f);
            rectTransform.sizeDelta = new Vector2(400f, 100f);

            FragmentDisplay fragmentDisplay = fragmentDisplayGO.AddComponent<FragmentDisplay>();
            SetupFragmentTypes(fragmentDisplay, fragmentPrefab);
        }

        /// <summary>
        /// Cria o QuickSlotsDisplay
        /// </summary>
        private void CreateQuickSlotsDisplay(GameObject canvas, GameObject quickSlotPrefab)
        {
            GameObject quickSlotsGO = new GameObject("QuickSlotsDisplay");
            quickSlotsGO.transform.SetParent(canvas.transform, false);

            RectTransform rectTransform = quickSlotsGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 100f);
            rectTransform.sizeDelta = new Vector2(400f, 100f);

            QuickSlotsDisplay quickSlotsDisplay = quickSlotsGO.AddComponent<QuickSlotsDisplay>();
            
            var quickSlotPrefabField = typeof(QuickSlotsDisplay).GetField("quickSlotPrefab", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            quickSlotPrefabField?.SetValue(quickSlotsDisplay, quickSlotPrefab);
        }

        /// <summary>
        /// Cria o UISystemManager
        /// </summary>
        private void CreateUISystemManager(GameObject canvas)
        {
            if (FindObjectOfType<UISystemManager>() != null)
            {
                return;
            }

            GameObject managerGO = new GameObject("UISystemManager");
            managerGO.AddComponent<UISystemManager>();
        }

        /// <summary>
        /// Configura os tipos de fragmento
        /// </summary>
        private void SetupFragmentTypes(FragmentDisplay fragmentDisplay, GameObject fragmentPrefab)
        {
            var fragmentTypesField = typeof(FragmentDisplay).GetField("fragmentTypes", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (fragmentTypesField != null)
            {
                var fragmentTypesList = fragmentTypesField.GetValue(fragmentDisplay) as System.Collections.IList;
                
                if (fragmentTypesList != null)
                {
                    fragmentTypesList.Clear();

                    string[] elementNames = { "Earth", "Nature", "Water", "Air", "Dark", "Fire" };
                    
                    foreach (var elementName in elementNames)
                    {
                        var fragmentTypeClass = typeof(FragmentDisplay).GetNestedType("FragmentType");
                        if (fragmentTypeClass != null)
                        {
                            var fragmentTypeInstance = System.Activator.CreateInstance(fragmentTypeClass);
                            
                            var elementNameField = fragmentTypeClass.GetField("elementName");
                            elementNameField?.SetValue(fragmentTypeInstance, elementName);
                            
                            var prefabField = fragmentTypeClass.GetField("prefab");
                            prefabField?.SetValue(fragmentTypeInstance, fragmentPrefab);
                            
                            fragmentTypesList.Add(fragmentTypeInstance);
                        }
                    }
                }
            }
        }
        #endregion
    }
}