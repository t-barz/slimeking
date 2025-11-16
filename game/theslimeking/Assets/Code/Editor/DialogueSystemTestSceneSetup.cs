using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using SlimeMec.Systems;
using SlimeMec.Gameplay;
using SlimeMec.Gameplay.NPCs;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Editor utility to create and setup the Dialogue System Test Scene
    /// </summary>
    public static class DialogueSystemTestSceneSetup
    {
        [MenuItem("SlimeKing/Dialogue System/Create Test Scene", false, 100)]
        public static void CreateTestScene()
        {
            // Create new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Setup the scene
            SetupTestScene();
            
            // Save the scene
            string scenePath = "Assets/Game/Scenes/Tests/DialogueSystemTest.unity";
            System.IO.Directory.CreateDirectory("Assets/Game/Scenes/Tests");
            EditorSceneManager.SaveScene(newScene, scenePath);
            
            UnityEngine.Debug.Log($"[DialogueSystem] Test scene created at: {scenePath}");
        }
        
        private static void SetupTestScene()
        {
            // Create Managers GameObject
            GameObject managersObj = new GameObject("--- MANAGERS ---");
            managersObj.AddComponent<DialogueManager>();
            managersObj.AddComponent<LocalizationManager>();
            
            // Create UI GameObject with Canvas
            GameObject uiObj = new GameObject("--- UI ---");
            Canvas canvas = uiObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            uiObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // Load and instantiate DialogueUI prefab
            GameObject dialogueUIPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/UI/DialogueUI.prefab");
            if (dialogueUIPrefab != null)
            {
                GameObject dialogueUI = Object.Instantiate(dialogueUIPrefab, uiObj.transform);
                dialogueUI.name = "DialogueUI";
            }
            else
            {
                UnityEngine.Debug.LogWarning("[DialogueSystem] DialogueUI prefab not found. Please add it manually.");
            }
            
            // Create Language Switcher UI
            CreateLanguageSwitcherUI(uiObj.transform);
            
            // Create NPCs
            CreateTestNPCs();
            
            // Create Player placeholder
            CreatePlayerPlaceholder();
            
            UnityEngine.Debug.Log("[DialogueSystem] Test scene setup complete!");
            UnityEngine.Debug.Log("NPCs created:");
            UnityEngine.Debug.Log("  - Merchant (single page dialogue)");
            UnityEngine.Debug.Log("  - Guard (multi-page dialogue)");
            UnityEngine.Debug.Log("  - Villager (multi-page dialogue)");
            UnityEngine.Debug.Log("\nUse WASD to move, E to interact");
            UnityEngine.Debug.Log("Use UI buttons to change language");
        }
        
        private static void CreateLanguageSwitcherUI(Transform parent)
        {
            GameObject panel = new GameObject("LanguageSwitcher");
            panel.transform.SetParent(parent);
            
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(300, 100);
            
            UnityEngine.UI.Image panelImage = panel.AddComponent<UnityEngine.UI.Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f);
            
            // Add LanguageSwitcher component
            panel.AddComponent<LanguageSwitcher>();
            
            // Create buttons
            CreateLanguageButton(panel.transform, "BR", new Vector2(10, -10), LanguageCode.BR);
            CreateLanguageButton(panel.transform, "EN", new Vector2(110, -10), LanguageCode.EN);
            CreateLanguageButton(panel.transform, "ES", new Vector2(210, -10), LanguageCode.ES);
        }
        
        private static void CreateLanguageButton(Transform parent, string label, Vector2 position, LanguageCode language)
        {
            GameObject buttonObj = new GameObject($"Button_{label}");
            buttonObj.transform.SetParent(parent);
            
            RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0, 1);
            buttonRect.anchorMax = new Vector2(0, 1);
            buttonRect.pivot = new Vector2(0, 1);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = new Vector2(90, 40);
            
            UnityEngine.UI.Image buttonImage = buttonObj.AddComponent<UnityEngine.UI.Image>();
            buttonImage.color = Color.white;
            
            UnityEngine.UI.Button button = buttonObj.AddComponent<UnityEngine.UI.Button>();
            
            // Create text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            UnityEngine.UI.Text text = textObj.AddComponent<UnityEngine.UI.Text>();
            text.text = label;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            
            // Setup button click
            button.onClick.AddListener(() => {
                if (LocalizationManager.Instance != null)
                {
                    LocalizationManager.Instance.SetLanguage(language);
                    UnityEngine.Debug.Log($"[DialogueSystem] Language changed to: {language}");
                }
            });
        }
        
        private static void CreateTestNPCs()
        {
            // NPC 1: Merchant (single page)
            CreateNPC("NPC_Merchant", new Vector3(-3, 0, 0), "npc_merchant_greeting", Color.yellow);
            
            // NPC 2: Guard (multi-page)
            CreateNPC("NPC_Guard", new Vector3(0, 0, 0), "npc_guard_warning", Color.red);
            
            // NPC 3: Villager (multi-page)
            CreateNPC("NPC_Villager", new Vector3(3, 0, 0), "test_dialogue", Color.green);
        }
        
        private static void CreateNPC(string name, Vector3 position, string dialogueId, Color color)
        {
            GameObject npc = new GameObject(name);
            npc.transform.position = position;
            
            // Add visual representation
            SpriteRenderer spriteRenderer = npc.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite(color);
            spriteRenderer.sortingOrder = 1;
            
            // Add dialogue components using Quick Config
            NPCDialogueQuickConfig.ConfigureDialogueComponents(npc);
            
            // Set dialogue ID
            NPCDialogueInteraction interaction = npc.GetComponent<NPCDialogueInteraction>();
            if (interaction != null)
            {
                SerializedObject so = new SerializedObject(interaction);
                so.FindProperty("dialogueId").stringValue = dialogueId;
                so.ApplyModifiedProperties();
            }
            
            // Add label
            GameObject label = new GameObject("Label");
            label.transform.SetParent(npc.transform);
            label.transform.localPosition = new Vector3(0, 1.5f, 0);
            
            TextMesh textMesh = label.AddComponent<TextMesh>();
            textMesh.text = name.Replace("NPC_", "");
            textMesh.fontSize = 20;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;
            textMesh.color = Color.white;
        }
        
        private static void CreatePlayerPlaceholder()
        {
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = new Vector3(0, -3, 0);
            
            // Add visual representation
            SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = CreateSquareSprite(Color.blue);
            spriteRenderer.sortingOrder = 2;
            
            // Add simple movement
            player.AddComponent<SimplePlayerMovement>();
            
            // Add collider
            CircleCollider2D collider = player.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            
            // Add rigidbody
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        private static Sprite CreateSquareSprite(Color color)
        {
            Texture2D texture = new Texture2D(64, 64);
            Color[] pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }
    }
    
    /// <summary>
    /// Simple player movement for testing
    /// </summary>
    public class SimplePlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        
        private Rigidbody2D rb;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        private void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            
            Vector2 movement = new Vector2(horizontal, vertical).normalized;
            rb.linearVelocity = movement * moveSpeed;
        }
    }
    
    /// <summary>
    /// Language switcher component for UI buttons
    /// </summary>
    public class LanguageSwitcher : MonoBehaviour
    {
        private void Start()
        {
            UnityEngine.Debug.Log("[DialogueSystem] Language Switcher ready. Current language: " + 
                     (LocalizationManager.Instance != null ? LocalizationManager.Instance.GetCurrentLanguage().ToString() : "N/A"));
        }
    }
}
