using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using TheSlimeKing.Quest;
using TheSlimeKing.Inventory;
using System.IO;

namespace TheSlimeKing.Editor
{
    /// <summary>
    /// Editor tool para criar cena de teste do Quest System automaticamente.
    /// </summary>
    public static class QuestSystemTestSceneSetup
    {
        private const string SCENE_PATH = "Assets/Game/Scenes/Tests/QuestSystemTest.unity";
        private const string QUEST_DATA_PATH = "Assets/Data/Quests/TestQuest_CollectFlowers.asset";
        
        [MenuItem("SlimeKing/Quest System/Create Test Scene")]
        public static void CreateTestScene()
        {
            // Cria nova cena
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            
            // Remove câmera padrão (vamos usar a do player)
            GameObject defaultCamera = GameObject.Find("Main Camera");
            if (defaultCamera != null)
            {
                Object.DestroyImmediate(defaultCamera);
            }
            
            // Cria estrutura da cena
            CreateManagers();
            CreateUI();
            CreateTestNPC();
            CreatePlayer();
            CreateEnvironment();
            
            // Salva cena
            string directory = Path.GetDirectoryName(SCENE_PATH);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            EditorSceneManager.SaveScene(newScene, SCENE_PATH);
            
            Debug.Log($"[QuestSystemTestSceneSetup] Cena de teste criada em: {SCENE_PATH}");
            
            // Cria quest de teste
            CreateTestQuestData();
            
            EditorUtility.DisplayDialog(
                "Quest System Test Scene",
                "Cena de teste criada com sucesso!\n\n" +
                "Localização: " + SCENE_PATH + "\n\n" +
                "A cena inclui:\n" +
                "- QuestManager\n" +
                "- NPC Quest Giver\n" +
                "- Quest de teste (Coletar Flores)\n" +
                "- Player com inventário\n" +
                "- UI de notificações\n\n" +
                "Pressione Play para testar!",
                "OK"
            );
        }
        
        private static void CreateManagers()
        {
            // Cria GameObject para managers
            GameObject managersRoot = new GameObject("--- MANAGERS ---");
            
            // QuestManager
            GameObject questManagerObj = new GameObject("QuestManager");
            questManagerObj.transform.SetParent(managersRoot.transform);
            QuestManager questManager = questManagerObj.AddComponent<QuestManager>();
            
            // GameManager (se não existir na cena)
            if (GameObject.Find("GameManager") == null)
            {
                GameObject gameManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/GameManager.prefab");
                if (gameManagerPrefab != null)
                {
                    GameObject gameManagerObj = Object.Instantiate(gameManagerPrefab, managersRoot.transform);
                    gameManagerObj.name = "GameManager";
                }
                else
                {
                    Debug.LogWarning("[QuestSystemTestSceneSetup] GameManager prefab não encontrado. Criando GameObject vazio.");
                    GameObject gameManagerObj = new GameObject("GameManager");
                    gameManagerObj.transform.SetParent(managersRoot.transform);
                    gameManagerObj.AddComponent<GameManager>();
                }
            }
            
            Debug.Log("[QuestSystemTestSceneSetup] Managers criados");
        }
        
        private static void CreateUI()
        {
            // Cria Canvas root
            GameObject canvasRoot = new GameObject("--- UI ---");
            Canvas canvas = canvasRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasRoot.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasRoot.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            
            // QuestNotificationPanel
            GameObject notificationPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Game/Prefabs/UI/QuestNotificationPanel.prefab");
            if (notificationPrefab != null)
            {
                GameObject notificationObj = Object.Instantiate(notificationPrefab, canvasRoot.transform);
                notificationObj.name = "QuestNotificationPanel";
            }
            else
            {
                Debug.LogWarning("[QuestSystemTestSceneSetup] QuestNotificationPanel prefab não encontrado.");
            }
            
            Debug.Log("[QuestSystemTestSceneSetup] UI criada");
        }
        
        private static void CreateTestNPC()
        {
            // Cria NPC Quest Giver
            GameObject npcObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npcObj.name = "NPC_QuestGiver";
            npcObj.transform.position = new Vector3(0, 1, 3);
            
            // Cor amarela para identificar
            Renderer renderer = npcObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.yellow;
                renderer.material = mat;
            }
            
            // Adiciona QuestGiverController
            QuestGiverController questGiver = npcObj.AddComponent<QuestGiverController>();
            
            // Cria indicadores visuais
            GameObject availableIndicator = CreateIndicator(npcObj.transform, "QuestIndicatorAvailable", new Vector3(0, 2.5f, 0));
            GameObject readyIndicator = CreateIndicator(npcObj.transform, "QuestIndicatorReady", new Vector3(0, 2.5f, 0));
            
            // Desativa indicadores inicialmente (serão ativados pelo QuestGiverController)
            if (availableIndicator != null) availableIndicator.SetActive(false);
            if (readyIndicator != null) readyIndicator.SetActive(false);
            
            // Configura quest giver via reflection (para definir campos serializados)
            SerializedObject so = new SerializedObject(questGiver);
            
            // Adiciona quest de teste à lista
            SerializedProperty availableQuestsProperty = so.FindProperty("availableQuests");
            if (availableQuestsProperty != null)
            {
                availableQuestsProperty.arraySize = 1;
                // Quest será atribuída manualmente após criação
            }
            
            // Configura indicadores
            SerializedProperty availableIndicatorProperty = so.FindProperty("questAvailableIndicator");
            if (availableIndicatorProperty != null && availableIndicator != null)
            {
                availableIndicatorProperty.objectReferenceValue = availableIndicator;
            }
            
            SerializedProperty readyIndicatorProperty = so.FindProperty("questReadyIndicator");
            if (readyIndicatorProperty != null && readyIndicator != null)
            {
                readyIndicatorProperty.objectReferenceValue = readyIndicator;
            }
            
            so.ApplyModifiedProperties();
            
            Debug.Log("[QuestSystemTestSceneSetup] NPC Quest Giver criado");
        }
        
        private static GameObject CreateIndicator(Transform parent, string prefabName, Vector3 localPosition)
        {
            // Tenta carregar prefab de indicador
            GameObject indicatorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Game/Prefabs/UI/{prefabName}.prefab");
            
            if (indicatorPrefab != null)
            {
                GameObject indicator = Object.Instantiate(indicatorPrefab, parent);
                indicator.name = prefabName;
                indicator.transform.localPosition = localPosition;
                return indicator;
            }
            else
            {
                // Cria indicador simples se prefab não existir
                GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                indicator.name = prefabName;
                indicator.transform.SetParent(parent);
                indicator.transform.localPosition = localPosition;
                indicator.transform.localScale = Vector3.one * 0.3f;
                
                Renderer renderer = indicator.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.color = prefabName.Contains("Ready") ? new Color(1f, 0.84f, 0f) : Color.yellow; // Dourado vs Amarelo
                    renderer.material = mat;
                }
                
                // Remove collider
                Object.DestroyImmediate(indicator.GetComponent<Collider>());
                
                Debug.LogWarning($"[QuestSystemTestSceneSetup] {prefabName} prefab não encontrado. Criando indicador simples.");
                return indicator;
            }
        }
        
        private static void CreatePlayer()
        {
            // Cria player simples
            GameObject playerObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            playerObj.name = "Player";
            playerObj.tag = "Player";
            playerObj.transform.position = new Vector3(0, 1, 0);
            
            // Cor azul para identificar
            Renderer renderer = playerObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = Color.blue;
                renderer.material = mat;
            }
            
            // Adiciona Rigidbody
            Rigidbody rb = playerObj.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            
            // Adiciona InventoryManager
            InventoryManager inventoryManager = playerObj.AddComponent<InventoryManager>();
            
            // Adiciona script de movimento simples
            playerObj.AddComponent<SimplePlayerMovement>();
            
            // Adiciona câmera
            GameObject cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            cameraObj.transform.SetParent(playerObj.transform);
            cameraObj.transform.localPosition = new Vector3(0, 5, -10);
            cameraObj.transform.localRotation = Quaternion.Euler(20, 0, 0);
            Camera camera = cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
            
            Debug.Log("[QuestSystemTestSceneSetup] Player criado");
        }
        
        private static void CreateEnvironment()
        {
            // Cria chão
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(5, 1, 5);
            
            // Cor verde para grama
            Renderer renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0.3f, 0.6f, 0.3f);
                renderer.material = mat;
            }
            
            // Adiciona luz direcional
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);
            
            Debug.Log("[QuestSystemTestSceneSetup] Ambiente criado");
        }
        
        private static void CreateTestQuestData()
        {
            // Verifica se quest já existe
            CollectQuestData existingQuest = AssetDatabase.LoadAssetAtPath<CollectQuestData>(QUEST_DATA_PATH);
            if (existingQuest != null)
            {
                Debug.Log("[QuestSystemTestSceneSetup] Quest de teste já existe: " + QUEST_DATA_PATH);
                return;
            }
            
            // Cria diretório se não existir
            string directory = Path.GetDirectoryName(QUEST_DATA_PATH);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Cria quest de teste
            CollectQuestData questData = ScriptableObject.CreateInstance<CollectQuestData>();
            questData.questID = "test_collect_flowers";
            questData.questName = "Coletar Flores";
            questData.description = "O fazendeiro precisa de 3 flores para fazer um remédio. Colete 3 Frutas de Cura para ele.";
            
            // Tenta carregar item existente (FrutaDeCura)
            ItemData fruitItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/External/AssetStore/SlimeMec/_Scripts/Gameplay/FrutaDeCura.asset");
            if (fruitItem != null)
            {
                questData.itemToCollect = fruitItem;
                questData.quantityRequired = 3;
            }
            else
            {
                Debug.LogWarning("[QuestSystemTestSceneSetup] Item 'FrutaDeCura' não encontrado. Quest criada sem item.");
            }
            
            // Configura recompensas
            ItemData crystalItem = AssetDatabase.LoadAssetAtPath<ItemData>("Assets/External/AssetStore/SlimeMec/_Scripts/Gameplay/CristalElemental.asset");
            if (crystalItem != null)
            {
                questData.itemRewards.Add(new ItemReward { item = crystalItem, quantity = 2 });
            }
            
            questData.reputationReward = 10;
            questData.minimumReputation = 0;
            questData.isRepeatable = true;
            
            // Salva asset
            AssetDatabase.CreateAsset(questData, QUEST_DATA_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("[QuestSystemTestSceneSetup] Quest de teste criada: " + QUEST_DATA_PATH);
            
            // Atribui quest ao NPC na cena
            AssignQuestToNPC(questData);
        }
        
        private static void AssignQuestToNPC(CollectQuestData questData)
        {
            // Encontra NPC na cena
            GameObject npcObj = GameObject.Find("NPC_QuestGiver");
            if (npcObj == null)
            {
                Debug.LogWarning("[QuestSystemTestSceneSetup] NPC não encontrado para atribuir quest.");
                return;
            }
            
            QuestGiverController questGiver = npcObj.GetComponent<QuestGiverController>();
            if (questGiver == null)
            {
                Debug.LogWarning("[QuestSystemTestSceneSetup] QuestGiverController não encontrado no NPC.");
                return;
            }
            
            // Atribui quest via SerializedObject
            SerializedObject so = new SerializedObject(questGiver);
            SerializedProperty availableQuestsProperty = so.FindProperty("availableQuests");
            
            if (availableQuestsProperty != null)
            {
                availableQuestsProperty.arraySize = 1;
                SerializedProperty questElement = availableQuestsProperty.GetArrayElementAtIndex(0);
                questElement.objectReferenceValue = questData;
                so.ApplyModifiedProperties();
                
                Debug.Log("[QuestSystemTestSceneSetup] Quest atribuída ao NPC");
            }
        }
    }
    
    /// <summary>
    /// Script simples de movimento para testes.
    /// </summary>
    public class SimplePlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float rotationSpeed = 720f;
        
        private Rigidbody rb;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        
        private void Update()
        {
            // Movimento WASD
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;
            
            if (movement.magnitude > 0.1f)
            {
                // Move
                Vector3 targetPosition = transform.position + movement * moveSpeed * Time.deltaTime;
                rb.MovePosition(targetPosition);
                
                // Rotaciona
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}
