using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using SlimeMec.Gameplay;

namespace SlimeKing.Editor
{
    /// <summary>
    /// EditorWindow para configuração rápida de NPCs.
    /// Permite criar NPCs completos com comportamentos, IA, diálogos e sistemas de amizade.
    /// </summary>
    public class NPCQuickConfig : EditorWindow
    {
        // Target GameObject
        private GameObject targetObject;

        // Basic Configuration
        private string npcName = "";
        private string speciesName = "";

        // Behavior Settings
        private BehaviorType behaviorType = BehaviorType.Passivo;
        private AIType aiType = AIType.Static;
        private DialogueTriggerType dialogueTriggerType = DialogueTriggerType.Interaction;

        // Ranges
        private float detectionRange = 5.0f;
        private float wanderRadius = 5.0f;
        private float wanderSpeed = 2.0f;
        private float pauseDuration = 2.0f;
        private float patrolSpeed = 2.5f;
        private float waitAtPoint = 1.0f;
        private float triggerRange = 2.0f;

        // Patrol Points
        private List<Vector2> patrolPoints = new List<Vector2>();

        // System Toggles
        private bool friendshipEnabled = false;
        private bool dialogueEnabled = false;
        private bool showGizmos = true;

        // Friendship Settings
        private int initialFriendshipLevel = 0;
        private int maxFriendshipLevel = 5;

        // Template Selection
        private int selectedTemplateIndex = 0;
        private readonly string[] templateOptions = new string[] 
        { 
            "Custom", 
            "Cervo-Broto", 
            "Esquilo Coletor", 
            "Abelha Cristalina" 
        };

        // Validation
        private ValidationResult lastValidationResult = null;

        // LEGACY MENU - Comentado para evitar duplicação. Use "Extra Tools" menu
        // [MenuItem("QuickWinds/NPC Quick Config (Use Extra Tools)")]
        public static void ShowWindow()
        {
            NPCQuickConfig window = GetWindow<NPCQuickConfig>("NPC Quick Config");
            window.minSize = new Vector2(400, 600);
            window.Show();
        }
        
        // LEGACY MENU - Comentado para evitar duplicação. Use "Extra Tools" menu via UnifiedExtraTools
        // [MenuItem("Extra Tools/NPC/🎭 NPC Quick Config (Direct)")]
        public static void ShowWindowDirect()
        {
            ShowWindow();
        }

        private void OnEnable()
        {
            // Initialize default values
            InitializeDefaults();
            
            // Subscribe to selection changed event
            Selection.selectionChanged += OnSelectionChanged;
            
            // Subscribe to Scene View GUI for gizmo drawing
            SceneView.duringSceneGui += OnSceneGUI;
            
            // Initialize target object with current selection
            targetObject = Selection.activeGameObject;
        }

        private void OnDisable()
        {
            // Unsubscribe from events
            Selection.selectionChanged -= OnSelectionChanged;
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSelectionChanged()
        {
            // Update target object when selection changes
            targetObject = Selection.activeGameObject;
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            // Selected GameObject
            EditorGUI.BeginDisabledGroup(true);
            targetObject = EditorGUILayout.ObjectField("Selected GameObject", targetObject, typeof(GameObject), true) as GameObject;
            EditorGUI.EndDisabledGroup();
            
            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("Select a GameObject in the scene to configure as NPC", MessageType.Warning);
                return;
            }
            
            EditorGUILayout.Space(10);
            
            // Template Selection
            EditorGUILayout.LabelField("Template Selection", EditorStyles.boldLabel);
            int newTemplateIndex = EditorGUILayout.Popup(
                new GUIContent("Template", "Selecione um template pré-configurado"), 
                selectedTemplateIndex, 
                templateOptions);
            if (newTemplateIndex != selectedTemplateIndex)
            {
                selectedTemplateIndex = newTemplateIndex;
                LoadTemplate(selectedTemplateIndex);
            }
            
            // Display template description
            if (selectedTemplateIndex > 0)
            {
                string description = GetTemplateDescription(selectedTemplateIndex);
                if (!string.IsNullOrEmpty(description))
                {
                    EditorGUILayout.HelpBox(description, MessageType.Info);
                }
            }
            
            EditorGUILayout.Space(10);
            
            // Basic Configuration
            EditorGUILayout.LabelField("Basic Configuration", EditorStyles.boldLabel);
            npcName = EditorGUILayout.TextField(
                new GUIContent("NPC Name", "Nome do NPC (ex: CervoBroto_01)"), 
                npcName);
            
            if (!friendshipEnabled)
            {
                speciesName = EditorGUILayout.TextField(
                    new GUIContent("Species Name", "Nome da espécie para sistema de amizade"), 
                    speciesName);
            }
            
            EditorGUILayout.Space(10);
            
            // Behavior Settings
            EditorGUILayout.LabelField("Behavior Settings", EditorStyles.boldLabel);
            behaviorType = (BehaviorType)EditorGUILayout.EnumPopup(
                new GUIContent("Behavior Type", "Tipo de comportamento social do NPC (Passivo, Neutro, Agressivo, QuestGiver)"), 
                behaviorType);
            aiType = (AIType)EditorGUILayout.EnumPopup(
                new GUIContent("AI Type", "Tipo de IA de movimento (Static, Wander, Patrol)"), 
                aiType);
            
            // Show Detection Range only for Neutro or Agressivo
            if (behaviorType == BehaviorType.Neutro || behaviorType == BehaviorType.Agressivo)
            {
                detectionRange = EditorGUILayout.FloatField(
                    new GUIContent("Detection Range", "Alcance de detecção do jogador em metros"), 
                    detectionRange);
            }
            
            // Show Wander Settings only when AI Type is Wander
            if (aiType == AIType.Wander)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Wander Settings", EditorStyles.miniBoldLabel);
                wanderRadius = EditorGUILayout.FloatField(
                    new GUIContent("  Wander Radius", "Raio de vagueio em metros"), 
                    wanderRadius);
                wanderSpeed = EditorGUILayout.FloatField(
                    new GUIContent("  Wander Speed", "Velocidade de vagueio em m/s"), 
                    wanderSpeed);
                pauseDuration = EditorGUILayout.FloatField(
                    new GUIContent("  Pause Duration", "Duração da pausa entre movimentos em segundos"), 
                    pauseDuration);
            }
            
            // Show Patrol Settings only when AI Type is Patrol
            if (aiType == AIType.Patrol)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("Patrol Settings", EditorStyles.miniBoldLabel);
                patrolSpeed = EditorGUILayout.FloatField(
                    new GUIContent("  Patrol Speed", "Velocidade de patrulha em m/s"), 
                    patrolSpeed);
                waitAtPoint = EditorGUILayout.FloatField(
                    new GUIContent("  Wait at Point", "Tempo de espera em cada ponto em segundos"), 
                    waitAtPoint);
                
                EditorGUILayout.LabelField("  Patrol Points:", EditorStyles.miniLabel);
                for (int i = 0; i < patrolPoints.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    patrolPoints[i] = EditorGUILayout.Vector2Field($"    Point {i}", patrolPoints[i]);
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                    {
                        patrolPoints.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Patrol Point"))
                {
                    patrolPoints.Add(Vector2.zero);
                }
            }
            
            EditorGUILayout.Space(10);
            
            // System Configuration
            EditorGUILayout.LabelField("System Configuration", EditorStyles.boldLabel);
            friendshipEnabled = EditorGUILayout.Toggle(
                new GUIContent("Friendship Enabled", "Habilita sistema de amizade para este NPC"), 
                friendshipEnabled);
            
            // Show Friendship fields only when enabled
            if (friendshipEnabled)
            {
                EditorGUI.indentLevel++;
                speciesName = EditorGUILayout.TextField(
                    new GUIContent("Species Name", "Nome da espécie para sistema de amizade (ex: Cervo, Esquilo)"), 
                    speciesName);
                initialFriendshipLevel = EditorGUILayout.IntSlider(
                    new GUIContent("Initial Level", "Nível inicial de amizade (0 = Desconhecido)"), 
                    initialFriendshipLevel, 0, 5);
                maxFriendshipLevel = EditorGUILayout.IntSlider(
                    new GUIContent("Max Level", "Nível máximo de amizade (5 = Companheiro Leal)"), 
                    maxFriendshipLevel, 1, 5);
                EditorGUI.indentLevel--;
            }
            
            dialogueEnabled = EditorGUILayout.Toggle(
                new GUIContent("Dialogue Enabled", "Habilita sistema de diálogo para este NPC"), 
                dialogueEnabled);
            
            // Show Dialogue fields only when enabled
            if (dialogueEnabled)
            {
                EditorGUI.indentLevel++;
                dialogueTriggerType = (DialogueTriggerType)EditorGUILayout.EnumPopup(
                    new GUIContent("Trigger Type", "Tipo de trigger do diálogo (Proximity ou Interaction)"), 
                    dialogueTriggerType);
                if (dialogueTriggerType == DialogueTriggerType.Proximity)
                {
                    triggerRange = EditorGUILayout.FloatField(
                        new GUIContent("Trigger Range", "Alcance do trigger de proximidade em metros"), 
                        triggerRange);
                }
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.Space(10);
            
            // Preview
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
            showGizmos = EditorGUILayout.Toggle(
                new GUIContent("Show Gizmos in Scene View", "Exibe gizmos visuais na Scene View para preview"), 
                showGizmos);
            
            // Add help box with contextual information
            if (aiType == AIType.Patrol && patrolPoints.Count < 2)
            {
                EditorGUILayout.HelpBox(
                    "Patrulha requer pelo menos 2 pontos. Pontos serão gerados automaticamente se necessário.", 
                    MessageType.Info);
            }
            
            if (friendshipEnabled && string.IsNullOrEmpty(speciesName))
            {
                EditorGUILayout.HelpBox(
                    "Nome da espécie é obrigatório quando sistema de amizade está habilitado.", 
                    MessageType.Warning);
            }
            
            EditorGUILayout.Space(20);
            
            // Validation Results
            if (lastValidationResult != null)
            {
                if (lastValidationResult.Errors.Count > 0)
                {
                    foreach (string error in lastValidationResult.Errors)
                    {
                        EditorGUILayout.HelpBox(error, MessageType.Error);
                    }
                }
                
                if (lastValidationResult.Warnings.Count > 0)
                {
                    foreach (string warning in lastValidationResult.Warnings)
                    {
                        EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    }
                }
                
                if (lastValidationResult.IsValid && lastValidationResult.Errors.Count == 0)
                {
                    EditorGUILayout.HelpBox("Configuration is valid!", MessageType.Info);
                }
                
                EditorGUILayout.Space(10);
            }
            
            // Buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Validate Configuration", GUILayout.Height(30)))
            {
                ValidateConfiguration();
            }
            if (GUILayout.Button("Apply Configuration", GUILayout.Height(30)))
            {
                ApplyConfiguration();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void InitializeDefaults()
        {
            // Set default values for all fields
            npcName = "";
            speciesName = "";
            behaviorType = BehaviorType.Passivo;
            aiType = AIType.Static;
            dialogueTriggerType = DialogueTriggerType.Interaction;
            
            detectionRange = 5.0f;
            wanderRadius = 5.0f;
            wanderSpeed = 2.0f;
            pauseDuration = 2.0f;
            patrolSpeed = 2.5f;
            waitAtPoint = 1.0f;
            triggerRange = 2.0f;
            
            patrolPoints = new List<Vector2>();
            
            friendshipEnabled = false;
            dialogueEnabled = false;
            showGizmos = true;
            
            initialFriendshipLevel = 0;
            maxFriendshipLevel = 5;
            
            selectedTemplateIndex = 0;
            lastValidationResult = null;
        }

        private string GetTemplateDescription(int templateIndex)
        {
            switch (templateIndex)
            {
                case 1: return "Passivo, wander behavior, amizade habilitada";
                case 2: return "Quest giver, static, diálogo habilitado";
                case 3: return "Neutro, patrol behavior, detecção básica";
                default: return "";
            }
        }

        private void LoadTemplate(int templateIndex)
        {
            // 0 = Custom (do nothing)
            if (templateIndex == 0)
            {
                return;
            }

            string templatePath = "";
            switch (templateIndex)
            {
                case 1: // Cervo-Broto
                    templatePath = "Assets/Data/QuickWinds/Templates/NPCTemplates/CervoBrotoTemplate.asset";
                    break;
                case 2: // Esquilo Coletor
                    templatePath = "Assets/Data/QuickWinds/Templates/NPCTemplates/EsquiloColetorTemplate.asset";
                    break;
                case 3: // Abelha Cristalina
                    templatePath = "Assets/Data/QuickWinds/Templates/NPCTemplates/AbelhaCristalineTemplate.asset";
                    break;
            }

            if (string.IsNullOrEmpty(templatePath))
            {
                return;
            }

            NPCTemplateData template = AssetDatabase.LoadAssetAtPath<NPCTemplateData>(templatePath);
            
            if (template == null)
            {
                Debug.LogWarning($"⚠️ Template not found at: {templatePath}. Using default values.");
                return;
            }

            // Load values from template
            npcName = template.npcName;
            speciesName = template.speciesName;
            behaviorType = template.behaviorType;
            aiType = template.aiType;
            detectionRange = template.detectionRange;
            
            wanderRadius = template.wanderRadius;
            wanderSpeed = template.wanderSpeed;
            pauseDuration = template.pauseDuration;
            
            patrolPoints = new List<Vector2>(template.patrolPoints);
            patrolSpeed = template.patrolSpeed;
            waitAtPoint = template.waitAtPoint;
            
            friendshipEnabled = template.friendshipEnabled;
            initialFriendshipLevel = template.initialFriendshipLevel;
            maxFriendshipLevel = template.maxFriendshipLevel;
            
            dialogueEnabled = template.dialogueEnabled;
            dialogueTriggerType = template.dialogueTriggerType;
            triggerRange = template.triggerRange;

            Debug.Log($"✅ Template loaded: {template.templateName}");
        }

        private void ValidateConfiguration()
        {
            // Create NPCConfigData from current field values
            NPCConfigData config = new NPCConfigData
            {
                npcName = this.npcName,
                speciesName = this.speciesName,
                behaviorType = this.behaviorType,
                aiType = this.aiType,
                detectionRange = this.detectionRange,
                aiSettings = new AISettings
                {
                    wanderRadius = this.wanderRadius,
                    wanderSpeed = this.wanderSpeed,
                    pauseDuration = this.pauseDuration,
                    patrolPoints = new List<Vector2>(this.patrolPoints),
                    patrolSpeed = this.patrolSpeed,
                    waitAtPoint = this.waitAtPoint
                },
                friendshipEnabled = this.friendshipEnabled,
                friendshipSettings = new FriendshipSettings
                {
                    initialLevel = this.initialFriendshipLevel,
                    maxLevel = this.maxFriendshipLevel
                },
                dialogueEnabled = this.dialogueEnabled,
                dialogueSettings = new DialogueSettings
                {
                    triggerType = this.dialogueTriggerType,
                    triggerRange = this.triggerRange
                }
            };

            // Call NPCValidator
            lastValidationResult = NPCValidator.ValidateConfiguration(config, targetObject);
            
            // Log results
            if (lastValidationResult.IsValid)
            {
                Debug.Log("✅ Configuration is valid!");
            }
            else
            {
                Debug.LogWarning($"⚠️ Validation found {lastValidationResult.Errors.Count} errors and {lastValidationResult.Warnings.Count} warnings");
            }
            
            Repaint();
        }

        private void ApplyConfiguration()
        {
            // First validate configuration
            ValidateConfiguration();
            
            // Abort if there are errors
            if (lastValidationResult != null && !lastValidationResult.IsValid)
            {
                bool proceed = EditorUtility.DisplayDialog(
                    "Validation Errors",
                    $"Configuration has {lastValidationResult.Errors.Count} error(s). Cannot apply configuration.\n\n" +
                    string.Join("\n", lastValidationResult.Errors),
                    "OK"
                );
                return;
            }
            
            // Performance Optimization: Group all undo operations
            Undo.SetCurrentGroupName("Configure NPC");
            int undoGroup = Undo.GetCurrentGroup();
            
            // Register Undo
            Undo.RegisterCompleteObjectUndo(targetObject, "Configure NPC");
            
            try
            {
                // Performance Profiling: Start timing
                NPCPerformanceProfiler.StartTiming("ApplyConfiguration");
                long memoryBefore = NPCPerformanceProfiler.GetCurrentMemoryUsage();
                
                // Create NPCConfigData
                NPCConfigData config = new NPCConfigData
                {
                    npcName = this.npcName,
                    speciesName = this.speciesName,
                    behaviorType = this.behaviorType,
                    aiType = this.aiType,
                    detectionRange = this.detectionRange,
                    aiSettings = new AISettings
                    {
                        wanderRadius = this.wanderRadius,
                        wanderSpeed = this.wanderSpeed,
                        pauseDuration = this.pauseDuration,
                        patrolPoints = new List<Vector2>(this.patrolPoints),
                        patrolSpeed = this.patrolSpeed,
                        waitAtPoint = this.waitAtPoint
                    },
                    friendshipEnabled = this.friendshipEnabled,
                    friendshipSettings = new FriendshipSettings
                    {
                        initialLevel = this.initialFriendshipLevel,
                        maxLevel = this.maxFriendshipLevel
                    },
                    dialogueEnabled = this.dialogueEnabled,
                    dialogueSettings = new DialogueSettings
                    {
                        triggerType = this.dialogueTriggerType,
                        triggerRange = this.triggerRange
                    }
                };
                
                // Performance Optimization: Batch asset creation
                List<(ScriptableObject asset, string path)> assetsToSave = new List<(ScriptableObject, string)>();
                
                // Create FriendshipData if enabled
                FriendshipData friendshipData = null;
                if (friendshipEnabled)
                {
                    friendshipData = NPCDataGenerator.CreateFriendshipData(speciesName);
                }
                
                // Create DialogueData if enabled
                DialogueData dialogueData = null;
                if (dialogueEnabled)
                {
                    dialogueData = NPCDataGenerator.CreateDialogueData(npcName, behaviorType);
                }
                
                // Create NPCData
                NPCData npcData = NPCDataGenerator.CreateNPCData(config, friendshipData, dialogueData);
                
                // Create/configure Animator Controller
                AnimatorController animatorController = NPCAnimatorSetup.CreateOrLoadController(npcName);
                if (animatorController != null)
                {
                    NPCAnimatorSetup.ConfigureStates(animatorController, dialogueEnabled);
                    NPCAnimatorSetup.ConfigureParameters(animatorController, dialogueEnabled);
                    NPCAnimatorSetup.ConfigureTransitions(animatorController, dialogueEnabled);
                    NPCAnimatorSetup.ConfigurePlaceholderAnimations(animatorController, npcName, dialogueEnabled);
                }
                
                // Configure all components
                NPCComponentConfigurator.ConfigureBasicComponents(targetObject, config, npcData, animatorController);
                
                // Mark GameObject as dirty
                EditorUtility.SetDirty(targetObject);
                
                // Performance Optimization: Collapse all undo operations into one
                Undo.CollapseUndoOperations(undoGroup);
                
                // Performance Profiling: Stop timing and log results
                double elapsedMs = NPCPerformanceProfiler.StopTiming("ApplyConfiguration");
                long memoryAfter = NPCPerformanceProfiler.GetCurrentMemoryUsage();
                
                NPCPerformanceProfiler.LogTiming("ApplyConfiguration", elapsedMs, 500); // Target: 500ms
                NPCPerformanceProfiler.LogMemoryUsage("ApplyConfiguration", memoryBefore, memoryAfter, 10); // Target: 10MB
                
                // Display success message
                string message = $"NPC '{npcName}' configured successfully!\n\n";
                message += $"NPCData created at: Assets/Data/NPCs/{npcName}Data.asset\n";
                if (friendshipEnabled)
                {
                    message += $"FriendshipData created at: Assets/Data/NPCs/Friendship/{speciesName}FriendshipData.asset\n";
                }
                if (dialogueEnabled)
                {
                    message += $"DialogueData created at: Assets/Data/NPCs/Dialogues/{npcName}DialogueData.asset\n";
                }
                message += $"Animator Controller created at: Assets/Art/Animations/NPCs/{npcName}Controller.controller";
                
                EditorUtility.DisplayDialog("Success", message, "OK");
                
                Debug.Log($"✅ NPC '{npcName}' configured successfully!");
            }
            catch (System.Exception e)
            {
                // Revert undo group on error
                Undo.RevertAllInCurrentGroup();
                
                EditorUtility.DisplayDialog("Error", $"Failed to configure NPC:\n\n{e.Message}", "OK");
                Debug.LogError($"❌ Error configuring NPC: {e.Message}\n{e.StackTrace}");
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            // Check if gizmos are enabled
            if (!showGizmos)
            {
                return;
            }
            
            // Check if target object is valid
            if (targetObject == null)
            {
                return;
            }
            
            Vector3 position = targetObject.transform.position;
            
            // Performance Optimization: Use cached gizmo drawing
            float currentWanderRadius = (aiType == AIType.Wander) ? wanderRadius : 0f;
            float currentDetectionRange = (behaviorType == BehaviorType.Neutro || behaviorType == BehaviorType.Agressivo) ? detectionRange : 0f;
            float currentDialogueRange = dialogueEnabled ? triggerRange : 0f;
            List<Vector2> currentPatrolPoints = (aiType == AIType.Patrol) ? patrolPoints : null;
            
            NPCGizmosDrawer.DrawNPCGizmosCached(
                targetObject.GetInstanceID(),
                position,
                currentWanderRadius,
                currentDetectionRange,
                currentDialogueRange,
                currentPatrolPoints
            );
        }
    }
}
