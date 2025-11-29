using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using TheSlimeKing.Dialogue;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para criar e configurar o DialogueCanvas prefab.
    /// </summary>
    public static class DialogueCanvasSetup
    {
        /// <summary>
        /// Cria um DialogueCanvas completo na cena.
        /// </summary>
        public static GameObject CreateDialogueCanvas()
        {
            // Verificar se já existe
            DialogueUI existing = Object.FindFirstObjectByType<DialogueUI>();
            if (existing != null)
            {
                Debug.LogWarning($"[DialogueCanvasSetup] DialogueCanvas já existe: {existing.gameObject.name}");
                return existing.gameObject;
            }
            
            // Criar Canvas root
            GameObject canvasObj = new GameObject("DialogueCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // Criar DialoguePanel
            GameObject panelObj = new GameObject("DialoguePanel");
            panelObj.transform.SetParent(canvasObj.transform, false);
            
            RectTransform panelRect = panelObj.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0f);
            panelRect.anchorMax = new Vector2(0.5f, 0f);
            panelRect.pivot = new Vector2(0.5f, 0f);
            panelRect.anchoredPosition = new Vector2(0, 50);
            panelRect.sizeDelta = new Vector2(800, 200);
            
            // Criar Background Image
            GameObject backgroundObj = new GameObject("Background");
            backgroundObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform backgroundRect = backgroundObj.AddComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.sizeDelta = Vector2.zero;
            
            Image backgroundImage = backgroundObj.AddComponent<Image>();
            
            // Tentar carregar ui_dialogBackground.png
            Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/UI/ui_dialogBackground.png");
            if (backgroundSprite == null)
            {
                // Tentar outros caminhos possíveis
                string[] possiblePaths = {
                    "Assets/Art/UI/ui_dialogBackground.png",
                    "Assets/Game/UI/ui_dialogBackground.png",
                    "Assets/UI/ui_dialogBackground.png"
                };
                
                foreach (string path in possiblePaths)
                {
                    backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    if (backgroundSprite != null)
                    {
                        Debug.Log($"[DialogueCanvasSetup] Background sprite encontrado em: {path}");
                        break;
                    }
                }
                
                if (backgroundSprite == null)
                {
                    Debug.LogWarning("[DialogueCanvasSetup] ui_dialogBackground.png não encontrado. Usando cor sólida.");
                    backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
                }
            }
            
            if (backgroundSprite != null)
            {
                backgroundImage.sprite = backgroundSprite;
                backgroundImage.type = Image.Type.Sliced;
            }
            
            // Criar DialogueText
            GameObject textObj = new GameObject("DialogueText");
            textObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.offsetMin = new Vector2(20, 20);
            textRect.offsetMax = new Vector2(-20, -20);
            
            TextMeshProUGUI dialogueText = textObj.AddComponent<TextMeshProUGUI>();
            dialogueText.fontSize = 24;
            dialogueText.color = Color.white;
            dialogueText.alignment = TextAlignmentOptions.TopLeft;
            dialogueText.text = "";
            
            // Adicionar TypewriterEffect
            textObj.AddComponent<TypewriterEffect>();
            
            // Criar Continue Indicator
            GameObject indicatorObj = new GameObject("ContinueIndicator");
            indicatorObj.transform.SetParent(panelObj.transform, false);
            
            RectTransform indicatorRect = indicatorObj.AddComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(1, 0);
            indicatorRect.anchorMax = new Vector2(1, 0);
            indicatorRect.pivot = new Vector2(1, 0);
            indicatorRect.anchoredPosition = new Vector2(-10, 10);
            indicatorRect.sizeDelta = new Vector2(30, 30);
            
            TextMeshProUGUI indicatorText = indicatorObj.AddComponent<TextMeshProUGUI>();
            indicatorText.text = "▼";
            indicatorText.fontSize = 20;
            indicatorText.color = Color.white;
            indicatorText.alignment = TextAlignmentOptions.Center;
            
            indicatorObj.SetActive(false);
            
            // Adicionar DialogueUI component
            DialogueUI dialogueUI = canvasObj.AddComponent<DialogueUI>();
            
            // Configurar referências via reflection (já que os campos são privados)
            SerializedObject serializedUI = new SerializedObject(dialogueUI);
            serializedUI.FindProperty("dialoguePanel").objectReferenceValue = panelObj;
            serializedUI.FindProperty("backgroundImage").objectReferenceValue = backgroundImage;
            serializedUI.FindProperty("dialogueText").objectReferenceValue = dialogueText;
            serializedUI.FindProperty("continueIndicator").objectReferenceValue = indicatorObj;
            serializedUI.FindProperty("sortingOrder").intValue = 100;
            serializedUI.ApplyModifiedProperties();
            
            // Desativar panel inicialmente
            panelObj.SetActive(false);
            
            Debug.Log("[DialogueCanvasSetup] DialogueCanvas criado com sucesso!");
            
            return canvasObj;
        }
    }
}
