using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;
using SlimeMec.Gameplay;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Configurador de componentes para NPCs.
    /// Adiciona e configura todos os componentes Unity necessários para um NPC funcional.
    /// </summary>
    public static class NPCComponentConfigurator
    {
        // Path do material padrão
        private const string DEFAULT_MATERIAL_PATH = "Assets/External/AssetStore/SlimeMec/_Art/Materials/sprite_lit_default.mat";

        /// <summary>
        /// Configura todos os componentes básicos do NPC.
        /// Ponto de entrada principal que chama todos os outros métodos de configuração.
        /// </summary>
        public static void ConfigureBasicComponents(
            GameObject target,
            NPCConfigData configData,
            NPCData npcData,
            RuntimeAnimatorController animatorController,
            Material customMaterial = null)
        {
            if (target == null)
            {
                UnityEngine.Debug.LogError("❌ NPCComponentConfigurator: Target GameObject is null!");
                return;
            }

            if (configData == null)
            {
                UnityEngine.Debug.LogError("❌ NPCComponentConfigurator: NPCConfigData is null!");
                return;
            }

            UnityEngine.Debug.Log($"🔧 Configuring NPC components for: {target.name}");

            // 1. Set tags and layers
            SetTagsAndLayers(target, configData.behaviorType);

            // 2. Configure SpriteRenderer
            ConfigureSpriteRenderer(target, customMaterial);

            // 3. Configure Animator
            ConfigureAnimator(target, animatorController);

            // 4. Configure Collider
            ConfigureCollider(target);

            // 5. Configure Rigidbody2D
            ConfigureRigidbody(target);

            // 6. Configure NPC-specific components
            ConfigureNPCComponents(target, configData, npcData);

            // 7. Configure Friendship component if enabled
            if (configData.friendshipEnabled)
            {
                ConfigureFriendshipComponent(target, npcData.friendshipData);
            }

            // 8. Configure Dialogue component if enabled
            if (configData.dialogueEnabled)
            {
                ConfigureDialogueComponent(target, npcData.dialogueData, configData.dialogueSettings);
            }

            // Mark object as modified
            EditorUtility.SetDirty(target);
            UnityEngine.Debug.Log($"✅ NPC components configured successfully for: {target.name}");
        }

        /// <summary>
        /// Configura tags e layers do GameObject baseado no tipo de comportamento.
        /// </summary>
        private static void SetTagsAndLayers(GameObject target, BehaviorType behaviorType)
        {
            // Set tag based on behavior type
            switch (behaviorType)
            {
                case BehaviorType.Passivo:
                    target.tag = "PassiveNPC";
                    break;
                case BehaviorType.Neutro:
                    target.tag = "NeutralNPC";
                    break;
                case BehaviorType.Agressivo:
                    target.tag = "AggressiveNPC";
                    break;
                case BehaviorType.QuestGiver:
                    target.tag = "QuestGiver";
                    break;
            }

            // Set layer to NPCs
            int npcLayer = LayerMask.NameToLayer("NPCs");
            if (npcLayer != -1)
            {
                target.layer = npcLayer;
                UnityEngine.Debug.Log($"🏷️ Tag set to '{target.tag}', Layer set to 'NPCs'");
            }
            else
            {
                UnityEngine.Debug.LogWarning("⚠️ Layer 'NPCs' not found. Using default layer.");
                target.layer = 0;
                UnityEngine.Debug.Log($"🏷️ Tag set to '{target.tag}', Layer set to 'Default'");
            }
        }

        /// <summary>
        /// Configura o componente SpriteRenderer do NPC.
        /// Adiciona se não existir e configura sorting layer e material.
        /// </summary>
        private static void ConfigureSpriteRenderer(GameObject target, Material customMaterial = null)
        {
            SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = target.AddComponent<SpriteRenderer>();
                UnityEngine.Debug.Log("🎨 SpriteRenderer added");
            }

            // Set sorting layer to "Characters"
            spriteRenderer.sortingLayerName = "Characters";
            spriteRenderer.sortingOrder = 10;
            spriteRenderer.color = Color.white;

            // Apply material
            Material materialToUse = customMaterial;
            if (materialToUse == null)
            {
                materialToUse = AssetDatabase.LoadAssetAtPath<Material>(DEFAULT_MATERIAL_PATH);
            }

            if (materialToUse != null)
            {
                spriteRenderer.material = materialToUse;
                UnityEngine.Debug.Log($"🎨 Material applied: {materialToUse.name}");
            }
            else
            {
                UnityEngine.Debug.LogWarning($"⚠️ Material not found at: {DEFAULT_MATERIAL_PATH}. Using default sprite material.");
            }

            // Preserve existing sprite if present
            if (spriteRenderer.sprite != null)
            {
                UnityEngine.Debug.Log($"🎨 Preserving existing sprite: {spriteRenderer.sprite.name}");
            }
            else
            {
                UnityEngine.Debug.LogWarning("⚠️ No sprite assigned. Please assign a sprite manually.");
            }
        }

        /// <summary>
        /// Configura o componente Animator do NPC.
        /// Adiciona se não existir e aplica o RuntimeAnimatorController.
        /// </summary>
        private static void ConfigureAnimator(GameObject target, RuntimeAnimatorController animatorController)
        {
            Animator animator = target.GetComponent<Animator>();
            if (animator == null)
            {
                animator = target.AddComponent<Animator>();
                UnityEngine.Debug.Log("🎬 Animator added");
            }

            // Configure animator settings
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

            // Apply animator controller
            if (animatorController != null)
            {
                animator.runtimeAnimatorController = animatorController;
                UnityEngine.Debug.Log($"🎬 Animator Controller applied: {animatorController.name}");
            }
            else
            {
                UnityEngine.Debug.LogWarning("⚠️ No Animator Controller provided. Create one using NPCAnimatorSetup.");
            }
        }

        /// <summary>
        /// Configura o componente Collider2D do NPC.
        /// Remove colliders existentes e adiciona CircleCollider2D.
        /// </summary>
        private static void ConfigureCollider(GameObject target, float radius = 0.5f)
        {
            // Remove existing colliders to avoid conflicts
            Collider2D[] existingColliders = target.GetComponents<Collider2D>();
            foreach (var collider in existingColliders)
            {
                Undo.DestroyObjectImmediate(collider);
            }

            if (existingColliders.Length > 0)
            {
                UnityEngine.Debug.Log($"🔘 Removed {existingColliders.Length} existing collider(s)");
            }

            // Add CircleCollider2D
            CircleCollider2D circleCollider = target.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = false;
            circleCollider.radius = radius;
            circleCollider.offset = Vector2.zero;

            UnityEngine.Debug.Log($"🔘 CircleCollider2D configured (radius: {radius})");
        }

        /// <summary>
        /// Configura o componente Rigidbody2D do NPC.
        /// Adiciona se não existir e configura para movimento 2D top-down.
        /// </summary>
        private static void ConfigureRigidbody(GameObject target)
        {
            Rigidbody2D rigidbody = target.GetComponent<Rigidbody2D>();
            if (rigidbody == null)
            {
                rigidbody = target.AddComponent<Rigidbody2D>();
                UnityEngine.Debug.Log("⚙️ Rigidbody2D added");
            }

            // Configure for 2D top-down movement
            rigidbody.bodyType = RigidbodyType2D.Dynamic;
            rigidbody.gravityScale = 0f;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            UnityEngine.Debug.Log("⚙️ Rigidbody2D configured (Dynamic, no gravity, rotation frozen)");
        }

        /// <summary>
        /// Configura os componentes específicos de NPC (NPCController, NPCBehavior, AI scripts).
        /// </summary>
        private static void ConfigureNPCComponents(GameObject target, NPCConfigData configData, NPCData npcData)
        {
            // 1. Add and configure NPCController
            NPCController npcController = target.GetComponent<NPCController>();
            if (npcController == null)
            {
                npcController = target.AddComponent<NPCController>();
                UnityEngine.Debug.Log("🎮 NPCController added");
            }

            // NOTE: NPCController was refactored to use new architecture
            // The old npcData property no longer exists
            // NPCController now uses NPCAttributesHandler and direct Inspector configuration
            // TODO: Update this editor tool to work with new NPCController architecture
            UnityEngine.Debug.Log($"🎮 NPCController added (configure manually in Inspector - old npcData system deprecated)");

            // 2. Add and configure NPCBehavior
            NPCBehavior npcBehavior = target.GetComponent<NPCBehavior>();
            if (npcBehavior == null)
            {
                npcBehavior = target.AddComponent<NPCBehavior>();
                UnityEngine.Debug.Log("🧠 NPCBehavior added");
            }

            npcBehavior.behaviorType = configData.behaviorType;
            npcBehavior.detectionRange = configData.detectionRange;
            UnityEngine.Debug.Log($"🧠 NPCBehavior configured: {configData.behaviorType}, Detection Range: {configData.detectionRange}m");

            // 3. Add appropriate AI script based on AIType
            ConfigureAIComponent(target, configData);

            // 4. QuestGiver placeholder
            if (configData.behaviorType == BehaviorType.QuestGiver)
            {
                // TODO: Add QuestGiver component when quest system is implemented
                UnityEngine.Debug.Log("📜 QuestGiver behavior detected. QuestGiver component will be added when quest system is implemented.");
            }
        }

        /// <summary>
        /// Configura o componente de IA apropriado baseado no AIType.
        /// </summary>
        private static void ConfigureAIComponent(GameObject target, NPCConfigData configData)
        {
            // Remove existing AI components to avoid conflicts
            RemoveAIComponents(target);

            switch (configData.aiType)
            {
                case AIType.Static:
                    ConfigureStaticAI(target);
                    break;

                case AIType.Wander:
                    ConfigureWanderAI(target, configData.aiSettings);
                    break;

                case AIType.Patrol:
                    ConfigurePatrolAI(target, configData.aiSettings);
                    break;
            }
        }

        /// <summary>
        /// Remove todos os componentes de IA existentes.
        /// </summary>
        private static void RemoveAIComponents(GameObject target)
        {
            var staticAI = target.GetComponent<NPCStaticAI>();
            if (staticAI != null) Undo.DestroyObjectImmediate(staticAI);

            var wanderAI = target.GetComponent<NPCWanderAI>();
            if (wanderAI != null) Undo.DestroyObjectImmediate(wanderAI);

            var patrolAI = target.GetComponent<NPCPatrolAI>();
            if (patrolAI != null) Undo.DestroyObjectImmediate(patrolAI);
        }

        /// <summary>
        /// Configura IA estática (sem movimento).
        /// </summary>
        private static void ConfigureStaticAI(GameObject target)
        {
            NPCStaticAI staticAI = target.AddComponent<NPCStaticAI>();
            UnityEngine.Debug.Log("🗿 NPCStaticAI added (NPC will remain stationary)");
        }

        /// <summary>
        /// Configura IA de vagueio com parâmetros.
        /// </summary>
        private static void ConfigureWanderAI(GameObject target, AISettings aiSettings)
        {
            NPCWanderAI wanderAI = target.AddComponent<NPCWanderAI>();
            wanderAI.wanderRadius = aiSettings.wanderRadius;
            wanderAI.wanderSpeed = aiSettings.wanderSpeed;
            wanderAI.pauseDuration = aiSettings.pauseDuration;

            UnityEngine.Debug.Log($"🚶 NPCWanderAI added (Radius: {aiSettings.wanderRadius}m, Speed: {aiSettings.wanderSpeed}m/s, Pause: {aiSettings.pauseDuration}s)");
        }

        /// <summary>
        /// Configura IA de patrulha com parâmetros.
        /// Auto-gera pontos de patrulha se a lista estiver vazia.
        /// </summary>
        private static void ConfigurePatrolAI(GameObject target, AISettings aiSettings)
        {
            NPCPatrolAI patrolAI = target.AddComponent<NPCPatrolAI>();
            patrolAI.patrolSpeed = aiSettings.patrolSpeed;
            patrolAI.waitAtPoint = aiSettings.waitAtPoint;

            // Auto-generate patrol points if list is empty
            if (aiSettings.patrolPoints == null || aiSettings.patrolPoints.Count < 2)
            {
                aiSettings.patrolPoints = GenerateDefaultPatrolPoints(target.transform.position, 3f);
                UnityEngine.Debug.Log($"🔄 Auto-generated 4 patrol points in square pattern (radius: 3 units)");
            }

            patrolAI.patrolPoints = new List<Vector2>(aiSettings.patrolPoints);

            UnityEngine.Debug.Log($"🚶 NPCPatrolAI added ({aiSettings.patrolPoints.Count} points, Speed: {aiSettings.patrolSpeed}m/s, Wait: {aiSettings.waitAtPoint}s)");
        }

        /// <summary>
        /// Gera 4 pontos de patrulha em padrão quadrado ao redor de uma posição central.
        /// </summary>
        private static List<Vector2> GenerateDefaultPatrolPoints(Vector3 center, float radius)
        {
            List<Vector2> points = new List<Vector2>
            {
                new Vector2(center.x + radius, center.y + radius),    // Top-right
                new Vector2(center.x + radius, center.y - radius),    // Bottom-right
                new Vector2(center.x - radius, center.y - radius),    // Bottom-left
                new Vector2(center.x - radius, center.y + radius)     // Top-left
            };

            return points;
        }

        /// <summary>
        /// Configura o componente NPCFriendship se o sistema de amizade estiver habilitado.
        /// </summary>
        private static void ConfigureFriendshipComponent(GameObject target, FriendshipData friendshipData)
        {
            if (friendshipData == null)
            {
                UnityEngine.Debug.LogWarning("⚠️ Friendship enabled but FriendshipData is null. Skipping friendship component.");
                return;
            }

            NPCFriendship npcFriendship = target.GetComponent<NPCFriendship>();
            if (npcFriendship == null)
            {
                npcFriendship = target.AddComponent<NPCFriendship>();
                UnityEngine.Debug.Log("💖 NPCFriendship added");
            }

            // Set FriendshipData reference
            npcFriendship.friendshipData = friendshipData;
            UnityEngine.Debug.Log($"💖 NPCFriendship configured with FriendshipData: {friendshipData.name}");
        }

        /// <summary>
        /// Configura o componente NPCDialogue se o sistema de diálogo estiver habilitado.
        /// </summary>
        private static void ConfigureDialogueComponent(GameObject target, DialogueData dialogueData, DialogueSettings dialogueSettings)
        {
            if (dialogueData == null)
            {
                UnityEngine.Debug.LogWarning("⚠️ Dialogue enabled but DialogueData is null. Skipping dialogue component.");
                return;
            }

            NPCDialogue npcDialogue = target.GetComponent<NPCDialogue>();
            if (npcDialogue == null)
            {
                npcDialogue = target.AddComponent<NPCDialogue>();
                UnityEngine.Debug.Log("💬 NPCDialogue added");
            }

            // Set DialogueData reference
            npcDialogue.dialogueData = dialogueData;
            npcDialogue.triggerType = dialogueSettings.triggerType;
            npcDialogue.triggerRange = dialogueSettings.triggerRange;

            UnityEngine.Debug.Log($"💬 NPCDialogue configured (Trigger: {dialogueSettings.triggerType}, Range: {dialogueSettings.triggerRange}m)");
        }
    }
}
