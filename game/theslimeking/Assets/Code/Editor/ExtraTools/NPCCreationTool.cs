using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;
using SlimeKing.Core;


namespace ExtraTools.Editor
{
    public static class NPCCreationTool
    {
        [UnityEditor.MenuItem("Extra Tools/NPC/Setup GameObject as NPC")]
        public static void SetupGameObjectAsNPC_MenuItem()
        {
            SetupGameObjectAsNPC();
        }

        /// <summary>
        /// Configura um GameObject como NPC completo com todos os componentes necessÃ¡rios
        /// </summary>
        public static void SetupGameObjectAsNPC()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado! Selecione um GameObject na hierarquia para configurar como NPC.");
                return;
            }

            // Verifica se jÃ¡ tem NPCController para evitar duplicaÃ§Ã£o
            NPCController existingController = selected.GetComponent<NPCController>();
            if (existingController != null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' jÃ¡ Ã© um NPC! Use outras ferramentas para reconfigurar.");
                return;
            }

            Debug.Log($"[NPCCreationTool] Iniciando configuraÃ§Ã£o completa de '{selected.name}' como NPC...");

            // === CONFIGURAÃ‡ÃƒO DE COMPONENTES OBRIGATÃ“RIOS ===
            SetupRequiredComponents(selected);

            // === CRIAÃ‡ÃƒO DE ESTRUTURA VISUAL ===
            CreateVisualStructure(selected);

            // === CONFIGURAÃ‡ÃƒO DO NPCCONTROLLER ===
            NPCController npcController = selected.AddComponent<NPCController>();
            ConfigureNPCController(npcController, selected);

            // === CONFIGURAÇÃO DO NPCATTRIBUTESHANDLER ===
            NPCAttributesHandler attributesHandler = selected.AddComponent<NPCAttributesHandler>();
            ConfigureNPCAttributesHandler(attributesHandler, selected);

            // === CONFIGURAÃ‡ÃƒO DO ANIMATOR EXISTENTE ===
            ConfigureExistingAnimator(selected);

            FinalizeNPCSetup(selected);

            // Seleciona o objeto configurado
            UnityEditor.Selection.activeGameObject = selected;

            Debug.Log($"[NPCCreationTool] âœ… GameObject '{selected.name}' configurado com sucesso como NPC completo!");
        }

        // Menu de contexto (clique direito) para configurar GameObject como NPC
        [UnityEditor.MenuItem("GameObject/Extra Tools/Setup as NPC", false, 0)]
        public static void SetupGameObjectAsNPC_ContextMenu()
        {
            SetupGameObjectAsNPC();
        }

        // ValidaÃ§Ã£o do menu de contexto - sÃ³ aparece quando hÃ¡ um GameObject selecionado
        [UnityEditor.MenuItem("GameObject/Extra Tools/Setup as NPC", true)]
        public static bool SetupGameObjectAsNPC_ContextMenuValidate()
        {
            return UnityEditor.Selection.activeGameObject != null;
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Add NPCController to Selected")]
        public static void AddNPCControllerToSelected()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado!");
                return;
            }

            // Verifica se jÃ¡ tem NPCController
            NPCController existingController = selected.GetComponent<NPCController>();
            if (existingController != null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' jÃ¡ possui NPCController!");
                return;
            }

            // Adiciona componentes obrigatÃ³rios se nÃ£o existirem
            if (selected.GetComponent<Rigidbody2D>() == null)
            {
                Rigidbody2D rb = selected.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; // NPCs 2D geralmente nÃ£o usam gravidade
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Impede rotaÃ§Ã£o
            }

            if (selected.GetComponent<Animator>() == null)
            {
                selected.AddComponent<Animator>();
            }

            // Adiciona o NPCController
            NPCController npcController = selected.AddComponent<NPCController>();

            // Configura automaticamente baseado nos filhos
            ConfigureNPCController(npcController, selected);

            // Adiciona NPCAttributesHandler
            NPCAttributesHandler attributesHandler = selected.AddComponent<NPCAttributesHandler>();
            ConfigureNPCAttributesHandler(attributesHandler, selected);

            // Configura Animator existente ou adiciona um bÃ¡sico se nÃ£o houver
            ConfigureExistingAnimator(selected);

            Debug.Log($"[NPCCreationTool] NPCController adicionado com sucesso ao GameObject '{selected.name}'!");
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Configure Visual Objects")]
        public static void ConfigureVisualObjects()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado!");
                return;
            }

            NPCController npcController = selected.GetComponent<NPCController>();
            if (npcController == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' nÃ£o possui NPCController!");
                return;
            }

            ConfigureNPCController(npcController, selected);

            // Adiciona NPCAttributesHandler
            NPCAttributesHandler attributesHandler = selected.AddComponent<NPCAttributesHandler>();
            ConfigureNPCAttributesHandler(attributesHandler, selected);

            Debug.Log($"[NPCCreationTool] Objetos visuais configurados para '{selected.name}'!");
        }

        /// <summary>
        /// Configura todos os componentes obrigatÃ³rios para o funcionamento do NPC
        /// </summary>
        /// <param name="gameObject">GameObject alvo</param>
        private static void SetupRequiredComponents(GameObject gameObject)
        {
            Debug.Log($"[NPCCreationTool] Configurando componentes obrigatÃ³rios para '{gameObject.name}'...");

            // === RIGIDBODY2D ===
            Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody2D>();
                Debug.Log("[NPCCreationTool] â€¢ Rigidbody2D adicionado");
            }

            // Configura Rigidbody2D para NPCs 2D
            rigidbody.gravityScale = 0f;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // === ANIMATOR ===
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.AddComponent<Animator>();
                Debug.Log("[NPCCreationTool] â€¢ Animator adicionado");
            }

            // === SORTING GROUP (para controle de camadas) - OPCIONAL ===
            // Nota: SortingGroup pode nÃ£o estar disponÃ­vel em todas as versÃµes
            // Se necessÃ¡rio, adicione manualmente depois
            /*
            SortingGroup sortingGroup = gameObject.GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = gameObject.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerName = "Default";
                sortingGroup.sortingOrder = 0;
                Debug.Log("[NPCCreationTool] â€¢ SortingGroup adicionado");
            }
            */

            // === COLLIDER2D (opcional mas recomendado) ===
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            if (collider == null)
            {
                CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                circleCollider.radius = 0.5f;
                circleCollider.isTrigger = false;
                Debug.Log("[NPCCreationTool] â€¢ CircleCollider2D adicionado");
            }

            Debug.Log("[NPCCreationTool] âœ… Componentes obrigatÃ³rios configurados");
        }

        /// <summary>
        /// Cria a estrutura visual necessÃ¡ria para NPCs direcionais
        /// </summary>
        /// <param name="gameObject">GameObject pai</param>
        private static void CreateVisualStructure(GameObject gameObject)
        {
            Debug.Log($"[NPCCreationTool] Criando estrutura visual para '{gameObject.name}'...");

            // Objetos visuais principais
            CreateChildObjectIfNotExists(gameObject, "front", "Sprite visual frontal (padrÃ£o)");
            CreateChildObjectIfNotExists(gameObject, "back", "Sprite visual traseiro");
            CreateChildObjectIfNotExists(gameObject, "side", "Sprite visual lateral");

            // Objetos VFX opcionais
            CreateChildObjectIfNotExists(gameObject, "vfx_front", "VFX frontal (opcional)");
            CreateChildObjectIfNotExists(gameObject, "vfx_back", "VFX traseiro (opcional)");
            CreateChildObjectIfNotExists(gameObject, "vfx_side", "VFX lateral (opcional)");

            // Sombra opcional
            CreateChildObjectIfNotExists(gameObject, "shadow", "Sombra do NPC (opcional)");

            // Configura objeto front como ativo por padrÃ£o
            Transform frontTransform = gameObject.transform.Find("front");
            if (frontTransform != null)
            {
                frontTransform.gameObject.SetActive(true);
            }

            // Desativa outros objetos por padrÃ£o
            string[] objectsToDeactivate = { "back", "side", "vfx_front", "vfx_back", "vfx_side" };
            foreach (string objName in objectsToDeactivate)
            {
                Transform objTransform = gameObject.transform.Find(objName);
                if (objTransform != null)
                {
                    objTransform.gameObject.SetActive(false);
                }
            }

            Debug.Log("[NPCCreationTool] âœ… Estrutura visual criada");
        }

        /// <summary>
        /// Cria um objeto filho se ele nÃ£o existir
        /// </summary>
        /// <param name="parent">GameObject pai</param>
        /// <param name="childName">Nome do objeto filho</param>
        /// <param name="description">DescriÃ§Ã£o para log</param>
        private static void CreateChildObjectIfNotExists(GameObject parent, string childName, string description)
        {
            Transform existingChild = parent.transform.Find(childName);
            if (existingChild == null)
            {
                GameObject childObj = new GameObject(childName);
                childObj.transform.SetParent(parent.transform);
                childObj.transform.localPosition = Vector3.zero;
                childObj.transform.localRotation = Quaternion.identity;
                childObj.transform.localScale = Vector3.one;

                // Adiciona SpriteRenderer para objetos visuais
                if (!childName.StartsWith("vfx_") && childName != "shadow")
                {
                    SpriteRenderer spriteRenderer = childObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sortingLayerName = "Default";
                    spriteRenderer.sortingOrder = 1;
                }

                Debug.Log($"[NPCCreationTool] â€¢ {childName} criado: {description}");
            }
        }

        /// <summary>
        /// ConfiguraÃ§Ãµes finais do NPC
        /// </summary>
        /// <param name="gameObject">GameObject do NPC</param>
        private static void FinalizeNPCSetup(GameObject gameObject)
        {
            Debug.Log($"[NPCCreationTool] Aplicando configuraÃ§Ãµes finais para '{gameObject.name}'...");

            // Define uma tag NPC se ela existir
            string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;
            if (allTags.Contains("NPC"))
            {
                gameObject.tag = "NPC";
                Debug.Log("[NPCCreationTool] â€¢ Tag 'NPC' aplicada");
            }

            // Define layer NPC se existir
            if (LayerMask.NameToLayer("NPC") != -1)
            {
                gameObject.layer = LayerMask.NameToLayer("NPC");
                Debug.Log("[NPCCreationTool] â€¢ Layer 'NPC' aplicado");
            }

            // Renomeia para seguir convenÃ§Ã£o se necessÃ¡rio
            if (!gameObject.name.StartsWith("NPC_") && !gameObject.name.Contains("NPC"))
            {
                string oldName = gameObject.name;
                gameObject.name = $"NPC_{gameObject.name}";
                Debug.Log($"[NPCCreationTool] â€¢ Nome alterado: '{oldName}' â†’ '{gameObject.name}'");
            }

            Debug.Log("[NPCCreationTool] âœ… ConfiguraÃ§Ãµes finais aplicadas");

            // ForÃ§a atualizaÃ§Ã£o da interface
            EditorUtility.SetDirty(gameObject);
        }

        private static void ConfigureNPCController(NPCController controller, GameObject npcObject)
        {
            // Configura os campos atravÃ©s de SerializedObject (mais seguro para o editor)

            // Busca objetos filhos baseado na estrutura do NPCTemplate
            Transform front = npcObject.transform.Find("front");
            Transform back = npcObject.transform.Find("back");
            Transform side = npcObject.transform.Find("side");
            Transform vfxFront = npcObject.transform.Find("vfx_front");
            Transform vfxBack = npcObject.transform.Find("vfx_back");
            Transform vfxSide = npcObject.transform.Find("vfx_side");

            // Configura os campos atravÃ©s de SerializedObject (mais seguro para o editor)
            var serializedObject = new UnityEditor.SerializedObject(controller);

            SetSerializedProperty(serializedObject, "frontObject", front?.gameObject);
            SetSerializedProperty(serializedObject, "backObject", back?.gameObject);
            SetSerializedProperty(serializedObject, "sideObject", side?.gameObject);
            SetSerializedProperty(serializedObject, "vfxFrontObject", vfxFront?.gameObject);
            SetSerializedProperty(serializedObject, "vfxBackObject", vfxBack?.gameObject);
            SetSerializedProperty(serializedObject, "vfxSideObject", vfxSide?.gameObject);

            // ConfiguraÃ§Ãµes padrÃ£o para NPC
            SetSerializedProperty(serializedObject, "moveSpeed", 2f);
            SetSerializedProperty(serializedObject, "movementType", NPCController.MovementType.Wander);
            SetSerializedProperty(serializedObject, "wanderRadius", 3f);
            SetSerializedProperty(serializedObject, "minIdleTime", 2f);
            SetSerializedProperty(serializedObject, "maxIdleTime", 5f);
            SetSerializedProperty(serializedObject, "minMoveTime", 1f);
            SetSerializedProperty(serializedObject, "maxMoveTime", 3f);

            serializedObject.ApplyModifiedProperties();

            int objectsFound = 0;
            if (front != null) objectsFound++;
            if (back != null) objectsFound++;
            if (side != null) objectsFound++;

            Debug.Log($"[NPCCreationTool] ConfiguraÃ§Ã£o concluÃ­da! {objectsFound} objetos visuais direcionais encontrados.");
        }

        /// <summary>
        /// Configura o Animator existente do GameObject para trabalhar com NPCController
        /// Adiciona parÃ¢metros necessÃ¡rios se nÃ£o existirem e configura adequadamente
        /// </summary>
        /// <param name="gameObject">GameObject com o Animator a ser configurado</param>
        private static void ConfigureExistingAnimator(GameObject gameObject)
        {
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{gameObject.name}' nÃ£o possui componente Animator!");
                return;
            }

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{gameObject.name}' nÃ£o possui um AnimatorController configurado! Use o controller existente.");
                return;
            }

            Debug.Log($"[NPCCreationTool] Configurando Animator existente '{controller.name}' para NPC...");

            try
            {
                bool hasChanges = false;

                // === VERIFICA PARÃ‚METROS EXISTENTES ===
                var existingParams = new List<string>();
                foreach (var param in controller.parameters)
                {
                    existingParams.Add($"{param.name} ({param.type})");
                }
                Debug.Log($"[NPCCreationTool] ðŸ“‹ ParÃ¢metros existentes: {string.Join(", ", existingParams)}");

                // === VERIFICA E ADICIONA APENAS PARÃ‚METROS NECESSÃRIOS ===
                // O NPCController precisa de isWalking para controlar animaÃ§Ã£o de movimento
                // Combat system precisa de triggers Hit e Attack
                bool hasIsWalking = false;
                bool hasFacingRight = false;
                bool hasHitTrigger = false;
                bool hasAttackTrigger = false;

                foreach (var existingParam in controller.parameters)
                {
                    if (existingParam.name == "isWalking" && existingParam.type == AnimatorControllerParameterType.Bool)
                    {
                        hasIsWalking = true;
                        Debug.Log($"[NPCCreationTool] âœ… ParÃ¢metro 'isWalking' jÃ¡ existe");
                    }
                    if (existingParam.name == "FacingRight" && existingParam.type == AnimatorControllerParameterType.Bool)
                    {
                        hasFacingRight = true;
                        Debug.Log($"[NPCCreationTool] âœ… ParÃ¢metro 'FacingRight' jÃ¡ existe");
                    }
                    if (existingParam.name == "Hit" && existingParam.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasHitTrigger = true;
                        Debug.Log($"[NPCCreationTool] âœ… ParÃ¢metro 'Hit' (Trigger) jÃ¡ existe");
                    }
                    if (existingParam.name == "Attack" && existingParam.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasAttackTrigger = true;
                        Debug.Log($"[NPCCreationTool] âœ… ParÃ¢metro 'Attack' (Trigger) jÃ¡ existe");
                    }
                }

                // Adiciona isWalking se nÃ£o existir (necessÃ¡rio para movimento)
                if (!hasIsWalking)
                {
                    controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
                    // Configura valor padrÃ£o
                    foreach (var param in controller.parameters)
                    {
                        if (param.name == "isWalking")
                        {
                            param.defaultBool = false;
                            break;
                        }
                    }
                    Debug.Log($"[NPCCreationTool] âž• ParÃ¢metro 'isWalking' (Bool) adicionado - necessÃ¡rio para movimento");
                    hasChanges = true;
                }

                // Adiciona FacingRight se nÃ£o existir (necessÃ¡rio para direÃ§Ã£o)
                if (!hasFacingRight)
                {
                    controller.AddParameter("FacingRight", AnimatorControllerParameterType.Bool);
                    // Configura valor padrÃ£o
                    foreach (var param in controller.parameters)
                    {
                        if (param.name == "FacingRight")
                        {
                            param.defaultBool = true;
                            break;
                        }
                    }
                    Debug.Log($"[NPCCreationTool] âž• ParÃ¢metro 'FacingRight' (Bool) adicionado - necessÃ¡rio para direÃ§Ã£o");
                    hasChanges = true;
                }

                // Adiciona Hit se nÃ£o existir (necessÃ¡rio para combat system)
                if (!hasHitTrigger)
                {
                    controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
                    Debug.Log($"[NPCCreationTool] âž• ParÃ¢metro 'Hit' (Trigger) adicionado - necessÃ¡rio para combat system");
                    hasChanges = true;
                }

                // Adiciona Attack se nÃ£o existir (necessÃ¡rio para combat system)
                if (!hasAttackTrigger)
                {
                    controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
                    Debug.Log($"[NPCCreationTool] âž• ParÃ¢metro 'Attack' (Trigger) adicionado - necessÃ¡rio para combat system");
                    hasChanges = true;
                }

                // === CONFIGURAÃ‡Ã•ES GERAIS DO ANIMATOR ===
                // Configura para nÃ£o aplicar root motion (importante para NPCs)
                if (animator.applyRootMotion)
                {
                    animator.applyRootMotion = false;
                    Debug.Log($"[NPCCreationTool] âš™ï¸ Root Motion desabilitado para NPC");
                }

                // Configura Culling Mode para AlwaysAnimate se for diferente
                if (animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
                {
                    animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    Debug.Log($"[NPCCreationTool] âš™ï¸ Culling Mode configurado para AlwaysAnimate");
                }

                // === CONFIGURAÃ‡ÃƒO DE TODAS AS TRANSIÃ‡Ã•ES ===
                ConfigureAllTransitions(controller, ref hasChanges);

                // === SALVA AS MUDANÃ‡AS ===
                if (hasChanges)
                {
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log($"[NPCCreationTool] âœ… Animator Controller '{controller.name}' configurado com sucesso!");
                }
                else
                {
                    Debug.Log($"[NPCCreationTool] âœ… Animator Controller '{controller.name}' jÃ¡ possui parÃ¢metros necessÃ¡rios");
                }

                // === LOGGING DE INFORMAÃ‡Ã•ES DO CONTROLLER ===
                Debug.Log($"[NPCCreationTool] ðŸ“Š Controller: {controller.name}");
                Debug.Log($"[NPCCreationTool] ðŸ“Š ParÃ¢metros: {controller.parameters.Length}");
                Debug.Log($"[NPCCreationTool] ðŸ“Š Layers: {controller.layers.Length}");

                if (controller.layers.Length > 0 && controller.layers[0].stateMachine.states != null)
                {
                    Debug.Log($"[NPCCreationTool] ðŸ“Š Estados: {controller.layers[0].stateMachine.states.Length}");
                    var stateNames = controller.layers[0].stateMachine.states.Select(s => s.state.name).ToArray();
                    Debug.Log($"[NPCCreationTool] ðŸ“Š Lista de Estados: {string.Join(", ", stateNames)}");
                }

                Debug.Log($"[NPCCreationTool] ðŸ’¡ O NPC manterÃ¡ os estados existentes ({string.Join(", ", controller.layers[0].stateMachine.states.Select(s => s.state.name))})");
                Debug.Log($"[NPCCreationTool] ðŸ’¡ Apenas parÃ¢metros bÃ¡sicos de movimento (isWalking, FacingRight) foram adicionados se necessÃ¡rio");

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[NPCCreationTool] Erro ao configurar Animator existente: {ex.Message}");
            }
        }

        /// <summary>
        /// Configura TODAS as transiÃ§Ãµes do Animator Controller baseadas nos parÃ¢metros existentes
        /// </summary>
        /// <param name="controller">Animator Controller a ser configurado</param>
        /// <param name="hasChanges">ReferÃªncia para indicar se houve mudanÃ§as</param>
        private static void ConfigureAllTransitions(AnimatorController controller, ref bool hasChanges)
        {
            if (controller.layers.Length == 0) return;

            var baseLayer = controller.layers[0];
            var stateMachine = baseLayer.stateMachine;

            if (stateMachine.states == null) return;

            Debug.Log($"[NPCCreationTool] ðŸ”„ Configurando TODAS as transiÃ§Ãµes do Animator Controller...");

            // === MAPEAMENTO DE ESTADOS ===
            var stateMap = new Dictionary<string, AnimatorState>();
            foreach (var stateInfo in stateMachine.states)
            {
                if (stateInfo.state != null)
                {
                    string stateName = stateInfo.state.name.ToLower();
                    stateMap[stateName] = stateInfo.state;
                }
            }

            // === VERIFICAÃ‡ÃƒO DE PARÃ‚METROS DISPONÃVEIS ===
            var parameterMap = new Dictionary<string, AnimatorControllerParameter>();
            foreach (var param in controller.parameters)
            {
                parameterMap[param.name.ToLower()] = param;
            }

            // === CONFIGURAÃ‡ÃƒO DE TRANSIÃ‡Ã•ES ESPECÃFICAS ===

            // 1. MOVIMENTO: Idle â†” Walk
            ConfigureMovementTransitions(stateMap, parameterMap, ref hasChanges);

            // 2. COMBAT: Any â†’ Attack (via trigger)
            ConfigureAttackTransitions(stateMap, parameterMap, stateMachine, ref hasChanges);

            // 3. DAMAGE: Any State â†’ Hit (via trigger)  
            ConfigureDamageTransitions(stateMap, parameterMap, stateMachine, ref hasChanges);

            // 4. COMPLETION: Attack/Hit/Unique â†’ Idle (via exit time)
            ConfigureCompletionTransitions(stateMap, ref hasChanges);

            // 5. DEATH: Hit â†’ Die (sem condiÃ§Ãµes especÃ­ficas, baseado em lÃ³gica de jogo)
            ConfigureDeathTransitions(stateMap, ref hasChanges);

            // 6. SPECIAL: Idle/Walk â†’ Unique (sem condiÃ§Ãµes para aÃ§Ãµes especiais)
            ConfigureSpecialTransitions(stateMap, ref hasChanges);

            // === CONFIGURA ESTADO PADRÃƒO ===
            if (stateMap.ContainsKey("idle") && stateMachine.defaultState != stateMap["idle"])
            {
                stateMachine.defaultState = stateMap["idle"];
                Debug.Log($"[NPCCreationTool] âš™ï¸ Estado padrÃ£o configurado: Idle");
                hasChanges = true;
            }

            Debug.Log($"[NPCCreationTool] âœ… ConfiguraÃ§Ã£o completa de todas as transiÃ§Ãµes concluÃ­da");
        }

        private static void ConfigureMovementTransitions(Dictionary<string, AnimatorState> stateMap, Dictionary<string, AnimatorControllerParameter> parameterMap, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("idle") || !stateMap.ContainsKey("walk"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estados Idle ou Walk nÃ£o encontrados - transiÃ§Ãµes de movimento ignoradas");
                return;
            }

            var idleState = stateMap["idle"];
            var walkState = stateMap["walk"];

            Debug.Log($"[NPCCreationTool] ðŸš¶ Configurando transiÃ§Ãµes de movimento...");

            // Idle â†’ Walk (quando isWalking = true)
            ConfigureOrCreateTransition(idleState, walkState, "isWalking", AnimatorConditionMode.If, parameterMap, ref hasChanges);

            // Walk â†’ Idle (quando isWalking = false)
            ConfigureOrCreateTransition(walkState, idleState, "isWalking", AnimatorConditionMode.IfNot, parameterMap, ref hasChanges);
        }

        private static void ConfigureAttackTransitions(Dictionary<string, AnimatorState> stateMap, Dictionary<string, AnimatorControllerParameter> parameterMap, AnimatorStateMachine stateMachine, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("attack"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Attack nÃ£o encontrado - transiÃ§Ãµes de ataque ignoradas");
                return;
            }

            var attackState = stateMap["attack"];
            Debug.Log($"[NPCCreationTool] âš”ï¸ Configurando transiÃ§Ãµes de ataque...");

            // Configura transiÃ§Ãµes dos estados bÃ¡sicos para Attack
            foreach (var kvp in stateMap)
            {
                string stateName = kvp.Key;
                var state = kvp.Value;

                // Apenas estados que devem poder atacar
                if (stateName == "idle" || stateName == "walk")
                {
                    ConfigureOrCreateTransition(state, attackState, "Attack", AnimatorConditionMode.If, parameterMap, ref hasChanges);
                }
            }

            // Any State â†’ Attack tambÃ©m (para triggers universais)
            ConfigureAnyStateTransition(stateMachine, attackState, "Attack", AnimatorConditionMode.If, parameterMap, ref hasChanges);
        }

        private static void ConfigureDamageTransitions(Dictionary<string, AnimatorState> stateMap, Dictionary<string, AnimatorControllerParameter> parameterMap, AnimatorStateMachine stateMachine, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("hit"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Hit nÃ£o encontrado - transiÃ§Ãµes de dano ignoradas");
                return;
            }

            var hitState = stateMap["hit"];
            Debug.Log($"[NPCCreationTool] ðŸ’¥ Configurando transiÃ§Ãµes de dano...");

            // Any State â†’ Hit (via trigger Hit)
            ConfigureAnyStateTransition(stateMachine, hitState, "Hit", AnimatorConditionMode.If, parameterMap, ref hasChanges);
        }

        private static void ConfigureCompletionTransitions(Dictionary<string, AnimatorState> stateMap, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("idle"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Idle nÃ£o encontrado - transiÃ§Ãµes de conclusÃ£o ignoradas");
                return;
            }

            var idleState = stateMap["idle"];
            Debug.Log($"[NPCCreationTool] ðŸ”„ Configurando transiÃ§Ãµes de conclusÃ£o...");

            // Estados que devem retornar ao Idle apÃ³s conclusÃ£o
            string[] completionStates = { "attack", "hit", "unique" };

            foreach (string stateName in completionStates)
            {
                if (stateMap.ContainsKey(stateName))
                {
                    var state = stateMap[stateName];
                    ConfigureExitTimeTransition(state, idleState, ref hasChanges);
                }
            }
        }

        private static void ConfigureDeathTransitions(Dictionary<string, AnimatorState> stateMap, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("hit") || !stateMap.ContainsKey("die"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estados Hit ou Die nÃ£o encontrados - transiÃ§Ãµes de morte ignoradas");
                return;
            }

            var hitState = stateMap["hit"];
            var dieState = stateMap["die"];

            Debug.Log($"[NPCCreationTool] ðŸ’€ Configurando transiÃ§Ãµes de morte...");

            // Hit â†’ Die (via exit time, para lÃ³gica de jogo determinar quando morrer)
            ConfigureExitTimeTransition(hitState, dieState, ref hasChanges);
        }

        private static void ConfigureSpecialTransitions(Dictionary<string, AnimatorState> stateMap, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("unique"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Unique nÃ£o encontrado - transiÃ§Ãµes especiais ignoradas");
                return;
            }

            var uniqueState = stateMap["unique"];
            Debug.Log($"[NPCCreationTool] â­ Configurando transiÃ§Ãµes especiais...");

            // Estados bÃ¡sicos podem ir para Unique (sem condiÃ§Ãµes especÃ­ficas)
            string[] basicStates = { "idle", "walk" };

            foreach (string stateName in basicStates)
            {
                if (stateMap.ContainsKey(stateName))
                {
                    var state = stateMap[stateName];

                    // Verifica se jÃ¡ existe transiÃ§Ã£o para Unique
                    bool hasTransitionToUnique = false;
                    foreach (var transition in state.transitions)
                    {
                        if (transition.destinationState == uniqueState)
                        {
                            hasTransitionToUnique = true;
                            break;
                        }
                    }

                    // Preserva transiÃ§Ã£o existente (pode ser controlada via script)
                    if (!hasTransitionToUnique)
                    {
                        var newTransition = state.AddTransition(uniqueState);
                        newTransition.hasExitTime = true;
                        newTransition.exitTime = 0.8f; // Permite interrupÃ§Ã£o tardia
                        newTransition.duration = 0.1f;

                        Debug.Log($"[NPCCreationTool] âž• TransiÃ§Ã£o {state.name} â†’ {uniqueState.name} criada (exit time)");
                        hasChanges = true;
                    }
                }
            }
        }

        private static void ConfigureOrCreateTransition(AnimatorState fromState, AnimatorState toState, string parameterName, AnimatorConditionMode mode, Dictionary<string, AnimatorControllerParameter> parameterMap, ref bool hasChanges)
        {
            if (!parameterMap.ContainsKey(parameterName.ToLower()))
            {
                Debug.LogWarning($"[NPCCreationTool] ParÃ¢metro '{parameterName}' nÃ£o encontrado - transiÃ§Ã£o {fromState.name} â†’ {toState.name} ignorada");
                return;
            }

            // Verifica se jÃ¡ existe transiÃ§Ã£o com condiÃ§Ã£o correta
            bool foundCorrectTransition = false;

            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == toState)
                {
                    // Verifica condiÃ§Ãµes existentes
                    bool hasCorrectCondition = false;
                    foreach (var condition in transition.conditions)
                    {
                        if (condition.parameter.ToLower() == parameterName.ToLower() && condition.mode == mode)
                        {
                            hasCorrectCondition = true;
                            break;
                        }
                    }

                    if (!hasCorrectCondition)
                    {
                        // Remove condiÃ§Ãµes antigas se existirem
                        for (int i = transition.conditions.Length - 1; i >= 0; i--)
                        {
                            transition.RemoveCondition(transition.conditions[i]);
                        }

                        // Adiciona condiÃ§Ã£o correta
                        transition.AddCondition(mode, 0, parameterName);

                        // Otimiza para responsividade
                        transition.hasExitTime = false;
                        transition.hasFixedDuration = true;
                        transition.duration = 0.1f;
                        transition.offset = 0f;

                        Debug.Log($"[NPCCreationTool] ðŸ”§ TransiÃ§Ã£o {fromState.name} â†’ {toState.name} atualizada com condiÃ§Ã£o {parameterName} {mode}");
                        hasChanges = true;
                    }
                    else
                    {
                        Debug.Log($"[NPCCreationTool] âœ… TransiÃ§Ã£o {fromState.name} â†’ {toState.name} jÃ¡ configurada corretamente");
                    }

                    foundCorrectTransition = true;
                    break;
                }
            }

            // Cria nova transiÃ§Ã£o se necessÃ¡rio
            if (!foundCorrectTransition)
            {
                var newTransition = fromState.AddTransition(toState);
                newTransition.AddCondition(mode, 0, parameterName);
                newTransition.hasExitTime = false;
                newTransition.hasFixedDuration = true;
                newTransition.duration = 0.1f;
                newTransition.offset = 0f;

                Debug.Log($"[NPCCreationTool] âž• TransiÃ§Ã£o criada: {fromState.name} â†’ {toState.name} (quando {parameterName} {mode})");
                hasChanges = true;
            }
        }

        private static void ConfigureAnyStateTransition(AnimatorStateMachine stateMachine, AnimatorState toState, string parameterName, AnimatorConditionMode mode, Dictionary<string, AnimatorControllerParameter> parameterMap, ref bool hasChanges)
        {
            if (!parameterMap.ContainsKey(parameterName.ToLower()))
            {
                Debug.LogWarning($"[NPCCreationTool] ParÃ¢metro '{parameterName}' nÃ£o encontrado - Any State â†’ {toState.name} ignorada");
                return;
            }

            // Verifica se jÃ¡ existe Any State transition
            bool foundCorrectTransition = false;

            foreach (var transition in stateMachine.anyStateTransitions)
            {
                if (transition.destinationState == toState)
                {
                    // Verifica condiÃ§Ãµes
                    bool hasCorrectCondition = false;
                    foreach (var condition in transition.conditions)
                    {
                        if (condition.parameter.ToLower() == parameterName.ToLower() && condition.mode == mode)
                        {
                            hasCorrectCondition = true;
                            break;
                        }
                    }

                    if (!hasCorrectCondition)
                    {
                        // Remove condiÃ§Ãµes antigas
                        for (int i = transition.conditions.Length - 1; i >= 0; i--)
                        {
                            transition.RemoveCondition(transition.conditions[i]);
                        }

                        // Adiciona condiÃ§Ã£o correta
                        transition.AddCondition(mode, 0, parameterName);
                        transition.hasExitTime = false;
                        transition.hasFixedDuration = true;
                        transition.duration = 0.0f; // Imediata para triggers
                        transition.offset = 0f;

                        Debug.Log($"[NPCCreationTool] ðŸ”§ Any State â†’ {toState.name} atualizada com condiÃ§Ã£o {parameterName} {mode}");
                        hasChanges = true;
                    }
                    else
                    {
                        Debug.Log($"[NPCCreationTool] âœ… Any State â†’ {toState.name} jÃ¡ configurada corretamente");
                    }

                    foundCorrectTransition = true;
                    break;
                }
            }

            // Cria nova Any State transition se necessÃ¡rio
            if (!foundCorrectTransition)
            {
                var newTransition = stateMachine.AddAnyStateTransition(toState);
                newTransition.AddCondition(mode, 0, parameterName);
                newTransition.hasExitTime = false;
                newTransition.hasFixedDuration = true;
                newTransition.duration = 0.0f;
                newTransition.offset = 0f;

                Debug.Log($"[NPCCreationTool] âž• Any State â†’ {toState.name} criada (quando {parameterName} {mode})");
                hasChanges = true;
            }
        }

        private static void ConfigureExitTimeTransition(AnimatorState fromState, AnimatorState toState, ref bool hasChanges)
        {
            // Verifica se jÃ¡ existe transiÃ§Ã£o baseada em exit time
            bool foundExitTimeTransition = false;

            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == toState && transition.hasExitTime && transition.conditions.Length == 0)
                {
                    foundExitTimeTransition = true;
                    Debug.Log($"[NPCCreationTool] âœ… TransiÃ§Ã£o exit time {fromState.name} â†’ {toState.name} jÃ¡ existe");
                    break;
                }
            }

            // Cria transiÃ§Ã£o baseada em exit time se necessÃ¡rio
            if (!foundExitTimeTransition)
            {
                var newTransition = fromState.AddTransition(toState);
                newTransition.hasExitTime = true;
                newTransition.exitTime = 1.0f; // ApÃ³s completar animaÃ§Ã£o
                newTransition.hasFixedDuration = true;
                newTransition.duration = 0.0f; // TransiÃ§Ã£o imediata apÃ³s exit time
                newTransition.offset = 0f;

                Debug.Log($"[NPCCreationTool] âž• TransiÃ§Ã£o exit time criada: {fromState.name} â†’ {toState.name}");
                hasChanges = true;
            }
        }

        /// <summary>
        /// Cria um Animator Controller completo para NPCs com todos os parÃ¢metros e transiÃ§Ãµes necessÃ¡rios
        /// </summary>
        /// <param name="npcName">Nome do NPC para nomear o controller</param>
        /// <returns>AnimatorController criado ou null se houve erro</returns>
        private static AnimatorController CreateAnimatorControllerForNPC(string npcName)
        {
            try
            {
                // Define caminho para salvar o controller
                string folderPath = "Assets/Art/Animations/NPCs";
                string fileName = $"{npcName}_Controller.controller";
                string fullPath = $"{folderPath}/{fileName}";

                // Cria a pasta se nÃ£o existir
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    string[] pathParts = folderPath.Split('/');
                    string currentPath = pathParts[0];

                    for (int i = 1; i < pathParts.Length; i++)
                    {
                        string newPath = $"{currentPath}/{pathParts[i]}";
                        if (!AssetDatabase.IsValidFolder(newPath))
                        {
                            AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                        }
                        currentPath = newPath;
                    }
                }

                // Cria o Animator Controller
                AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(fullPath);

                // === CONFIGURAÃ‡ÃƒO DOS PARÃ‚METROS ===
                // Adiciona parÃ¢metros Bool
                controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
                controller.AddParameter("FacingRight", AnimatorControllerParameterType.Bool);

                // Configura valor padrÃ£o para FacingRight
                foreach (var param in controller.parameters)
                {
                    if (param.name == "FacingRight")
                    {
                        param.defaultBool = true; // NPCs comeÃ§am virados para direita
                    }
                }

                // === CONFIGURAÃ‡ÃƒO DOS ESTADOS ===
                // ObtÃ©m a layer base (sempre existe)
                AnimatorControllerLayer baseLayer = controller.layers[0];
                AnimatorStateMachine rootStateMachine = baseLayer.stateMachine;

                // Estado Idle (padrÃ£o)
                AnimatorState idleState = rootStateMachine.AddState("Idle", new Vector3(300, 0, 0));
                idleState.writeDefaultValues = true;

                // Estado Walking
                AnimatorState walkingState = rootStateMachine.AddState("Walking", new Vector3(300, 100, 0));
                walkingState.writeDefaultValues = true;

                // Define Idle como estado padrÃ£o
                rootStateMachine.defaultState = idleState;

                // === CONFIGURAÃ‡ÃƒO DAS TRANSIÃ‡Ã•ES ===
                // Idle â†’ Walking (quando isWalking = true)
                AnimatorStateTransition idleToWalking = idleState.AddTransition(walkingState);
                idleToWalking.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
                idleToWalking.hasExitTime = false;
                idleToWalking.hasFixedDuration = true;
                idleToWalking.duration = 0.1f;
                idleToWalking.offset = 0f;

                // Walking â†’ Idle (quando isWalking = false)
                AnimatorStateTransition walkingToIdle = walkingState.AddTransition(idleState);
                walkingToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
                walkingToIdle.hasExitTime = false;
                walkingToIdle.hasFixedDuration = true;
                walkingToIdle.duration = 0.1f;
                walkingToIdle.offset = 0f;

                // === CONFIGURAÃ‡Ã•ES ADICIONAIS ===
                // Configura layer settings
                baseLayer.name = "Base Layer";
                baseLayer.defaultWeight = 1f;

                // Salva as mudanÃ§as
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"[NPCCreationTool] Animator Controller criado com sucesso: {fullPath}");
                Debug.Log($"[NPCCreationTool] ParÃ¢metros criados: isWalking (Bool), FacingRight (Bool)");
                Debug.Log($"[NPCCreationTool] Estados criados: Idle (padrÃ£o), Walking");
                Debug.Log($"[NPCCreationTool] TransiÃ§Ãµes criadas: Idle â†” Walking baseadas em isWalking");

                return controller;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[NPCCreationTool] Erro ao criar Animator Controller: {ex.Message}");
                return null;
            }
        }


        /// <summary>
        /// Configura o NPCAttributesHandler com valores padrão
        /// </summary>
        /// <param name="attributesHandler">Componente NPCAttributesHandler</param>
        /// <param name="npcObject">GameObject do NPC</param>
        private static void ConfigureNPCAttributesHandler(NPCAttributesHandler attributesHandler, GameObject npcObject)
        {
            var serializedObject = new UnityEditor.SerializedObject(attributesHandler);

            // Configurações padrão de atributos
            SetSerializedProperty(serializedObject, "npcType", NPCType.Abelha);
            SetSerializedProperty(serializedObject, "baseHealthPoints", 3);
            SetSerializedProperty(serializedObject, "baseAttack", 1);
            SetSerializedProperty(serializedObject, "baseDefense", 0);
            SetSerializedProperty(serializedObject, "baseSpeed", 2);
            SetSerializedProperty(serializedObject, "enableLogs", false);
            SetSerializedProperty(serializedObject, "enableDebugGizmos", true);

            serializedObject.ApplyModifiedProperties();

            Debug.Log($"[NPCCreationTool] NPCAttributesHandler configurado para '{npcObject.name}' com atributos base");
        }

        private static void SetSerializedProperty(UnityEditor.SerializedObject serializedObject, string propertyName, object value)
        {
            var property = serializedObject.FindProperty(propertyName);
            if (property == null) return;

            switch (property.propertyType)
            {
                case UnityEditor.SerializedPropertyType.ObjectReference:
                    property.objectReferenceValue = value as Object;
                    break;
                case UnityEditor.SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;
                case UnityEditor.SerializedPropertyType.Integer:
                    property.intValue = (int)value;
                    break;
                case UnityEditor.SerializedPropertyType.Boolean:
                    property.boolValue = (bool)value;
                    break;
                case UnityEditor.SerializedPropertyType.Enum:
                    if (value is System.Enum enumValue)
                    {
                        property.enumValueIndex = System.Convert.ToInt32(enumValue);
                    }
                    break;
            }
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Create NEW Animator Controller")]
        public static void CreateNPCAnimatorController()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado!");
                return;
            }

            Animator animator = selected.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' nÃ£o possui componente Animator!");
                return;
            }

            // Cria um novo Animator Controller
            AnimatorController controller = CreateAnimatorControllerForNPC(selected.name);

            if (controller != null)
            {
                // Atribui o controller ao animator
                animator.runtimeAnimatorController = controller;

                Debug.Log($"[NPCCreationTool] Animator Controller criado e configurado para '{selected.name}': {AssetDatabase.GetAssetPath(controller)}");
            }
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Configure Existing Animator")]
        public static void ConfigureExistingAnimatorMenuItem()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado!");
                return;
            }

            ConfigureExistingAnimator(selected);
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Validate Animator Setup")]
        public static void ValidateNPCAnimatorSetup()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado!");
                return;
            }

            Animator animator = selected.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' nÃ£o possui componente Animator!");
                return;
            }

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' nÃ£o possui AnimatorController configurado!");
                return;
            }

            // Valida parÃ¢metros necessÃ¡rios
            string[] requiredBoolParams = { "isWalking", "FacingRight" };
            bool allParametersFound = true;

            foreach (string paramName in requiredBoolParams)
            {
                bool found = false;
                foreach (var param in controller.parameters)
                {
                    if (param.name == paramName && param.type == AnimatorControllerParameterType.Bool)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.LogError($"[NPCCreationTool] ParÃ¢metro obrigatÃ³rio nÃ£o encontrado: {paramName} (Bool)");
                    allParametersFound = false;
                }
            }

            // Valida estados necessÃ¡rios
            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            string[] requiredStates = { "Idle", "Walking" };
            bool allStatesFound = true;

            foreach (string stateName in requiredStates)
            {
                bool found = false;
                foreach (var state in stateMachine.states)
                {
                    if (state.state.name == stateName)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Debug.LogError($"[NPCCreationTool] Estado obrigatÃ³rio nÃ£o encontrado: {stateName}");
                    allStatesFound = false;
                }
            }

            // Resultado da validaÃ§Ã£o
            if (allParametersFound && allStatesFound)
            {
                Debug.Log($"[NPCCreationTool] âœ… Animator Controller de '{selected.name}' estÃ¡ configurado corretamente!");
            }
            else
            {
                Debug.LogWarning($"[NPCCreationTool] âš ï¸ Animator Controller de '{selected.name}' possui problemas de configuraÃ§Ã£o.");
            }
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Test NPC Movement")]
        public static void TestNPCMovement()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[NPCCreationTool] Esta funÃ§Ã£o sÃ³ funciona durante o Play Mode!");
                return;
            }

            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado!");
                return;
            }

            NPCController npcController = selected.GetComponent<NPCController>();
            if (npcController == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' nÃ£o possui NPCController!");
                return;
            }

        }
    }
}
