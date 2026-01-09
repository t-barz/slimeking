using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SlimeKing.UI
{
    /// <summary>
    /// Utilitário para criar os prefabs necessários para o sistema de UI.
    /// Cria prefabs básicos para FragmentDisplay e QuickSlotsDisplay.
    /// </summary>
    public class PrefabCreator : MonoBehaviour
    {
        #region Public Methods
        /// <summary>
        /// Cria o prefab de fragmento
        /// </summary>
        [ContextMenu("Create Fragment Prefab")]
        public void CreateFragmentPrefab()
        {
            // Cria o GameObject principal
            GameObject fragmentGO = new GameObject("FragmentPrefab");
            
            // Adiciona RectTransform
            RectTransform rectTransform = fragmentGO.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80f, 80f);
            
            // Adiciona Image para o ícone do fragmento
            Image fragmentImage = fragmentGO.AddComponent<Image>();
            fragmentImage.color = Color.white;
            
            // Cria o GameObject do contador
            GameObject counterGO = new GameObject("Counter");
            counterGO.transform.SetParent(fragmentGO.transform, false);
            
            // Configura RectTransform do contador
            RectTransform counterRect = counterGO.AddComponent<RectTransform>();
            counterRect.anchorMin = new Vector2(0.5f, 0f);
            counterRect.anchorMax = new Vector2(0.5f, 0f);
            counterRect.anchoredPosition = new Vector2(0f, -10f);
            counterRect.sizeDelta = new Vector2(60f, 30f);
            
            // Adiciona TextMeshProUGUI para o contador
            TextMeshProUGUI counterText = counterGO.AddComponent<TextMeshProUGUI>();
            counterText.text = "0";
            counterText.fontSize = 18f;
            counterText.color = Color.white;
            counterText.alignment = TextAlignmentOptions.Center;
            counterText.fontStyle = FontStyles.Bold;
        }
        
        /// <summary>
        /// Cria o prefab de quick slot
        /// </summary>
        [ContextMenu("Create QuickSlot Prefab")]
        public void CreateQuickSlotPrefab()
        {
            // Cria o GameObject principal
            GameObject slotGO = new GameObject("QuickSlotPrefab");
            
            // Adiciona RectTransform
            RectTransform rectTransform = slotGO.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(80f, 80f);
            
            // Adiciona Image para o fundo do slot
            Image slotImage = slotGO.AddComponent<Image>();
            slotImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f); // Fundo escuro semi-transparente
            
            // Cria o GameObject do ícone
            GameObject iconGO = new GameObject("Icon");
            iconGO.transform.SetParent(slotGO.transform, false);
            
            // Configura RectTransform do ícone
            RectTransform iconRect = iconGO.AddComponent<RectTransform>();
            iconRect.anchorMin = Vector2.zero;
            iconRect.anchorMax = Vector2.one;
            iconRect.offsetMin = new Vector2(5f, 5f);
            iconRect.offsetMax = new Vector2(-5f, -5f);
            
            // Adiciona Image para o ícone
            Image iconImage = iconGO.AddComponent<Image>();
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
        }
        
        /// <summary>
        /// Cria um Canvas de teste com todos os componentes
        /// </summary>
        [ContextMenu("Create Test Canvas")]
        public void CreateTestCanvas()
        {
            // Verifica se já existe um QuickUISetup na cena
            QuickUISetup existingSetup = FindObjectOfType<QuickUISetup>();
            if (existingSetup != null)
            {
                existingSetup.SetupCompleteUISystem();
                return;
            }
            
            // Cria um GameObject com QuickUISetup
            GameObject setupGO = new GameObject("QuickUISetup");
            QuickUISetup quickSetup = setupGO.AddComponent<QuickUISetup>();
        }
        #endregion
    }
}