using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;
using SlimeKing.Core;

/// <summary>
/// Ferramenta de editor para facilitar a criação e configuração de NPCs no jogo.
/// Configura completamente GameObjects existentes para funcionar como NPCs completos.
/// Também cria automaticamente os atributos e transições necessários no Animator Controller.
/// </summary>
namespace ExtraTools.Editor
{
    public static class NPCCreationTool
    {
        [UnityEditor.MenuItem("Extra Tools/NPC/Setup GameObject as NPC")]
        public static void SetupGameObjectAsNPC()
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("[NPCCreationTool] Nenhum GameObject selecionado! Selecione um GameObject na hierarquia para configurar como NPC.");
                return;
            }

            // Verifica se já tem NPCController para evitar duplicação
            NPCController existingController = selected.GetComponent<NPCController>();
            if (existingController != null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' já é um NPC! Use outras ferramentas para reconfigurar.");
                return;
            }

            Debug.Log($"[NPCCreationTool] Iniciando configuração completa de '{selected.name}' como NPC...");

            // === CONFIGURAÇÃO DE COMPONENTES OBRIGATÓRIOS ===
            SetupRequiredComponents(selected);

            // === CRIAÇÃO DE ESTRUTURA VISUAL ===
            CreateVisualStructure(selected);

            // === CONFIGURAÇÃO DO NPCCONTROLLER ===
            NPCController npcController = selected.AddComponent<NPCController>();
            ConfigureNPCController(npcController, selected);

            // === CONFIGURAÇÃO DO ANIMATOR EXISTENTE ===
            ConfigureExistingAnimator(selected);

            // === CONFIGURAÇÕES FINAIS ===
            FinalizeNPCSetup(selected);

            // Seleciona o objeto configurado
            UnityEditor.Selection.activeGameObject = selected;

            Debug.Log($"[NPCCreationTool] ✅ GameObject '{selected.name}' configurado com sucesso como NPC completo!");
        }

        // Menu de contexto (clique direito) para configurar GameObject como NPC
        [UnityEditor.MenuItem("GameObject/Extra Tools/Setup as NPC", false, 0)]
        public static void SetupGameObjectAsNPC_ContextMenu()
        {
            SetupGameObjectAsNPC();
        }

        // Validação do menu de contexto - só aparece quando há um GameObject selecionado
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

            // Verifica se já tem NPCController
            NPCController existingController = selected.GetComponent<NPCController>();
            if (existingController != null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' já possui NPCController!");
                return;
            }

            // Adiciona componentes obrigatórios se não existirem
            if (selected.GetComponent<Rigidbody2D>() == null)
            {
                Rigidbody2D rb = selected.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f; // NPCs 2D geralmente não usam gravidade
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Impede rotação
            }

            if (selected.GetComponent<Animator>() == null)
            {
                selected.AddComponent<Animator>();
            }

            // Adiciona o NPCController
            NPCController npcController = selected.AddComponent<NPCController>();

            // Configura automaticamente baseado nos filhos
            ConfigureNPCController(npcController, selected);

            // Configura Animator existente ou adiciona um básico se não houver
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
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' não possui NPCController!");
                return;
            }

            ConfigureNPCController(npcController, selected);

            Debug.Log($"[NPCCreationTool] Objetos visuais configurados para '{selected.name}'!");
        }

        /// <summary>
        /// Configura todos os componentes obrigatórios para o funcionamento do NPC
        /// </summary>
        /// <param name="gameObject">GameObject alvo</param>
        private static void SetupRequiredComponents(GameObject gameObject)
        {
            Debug.Log($"[NPCCreationTool] Configurando componentes obrigatórios para '{gameObject.name}'...");

            // === RIGIDBODY2D ===
            Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody2D>();
                Debug.Log("[NPCCreationTool] • Rigidbody2D adicionado");
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
                Debug.Log("[NPCCreationTool] • Animator adicionado");
            }

            // === SORTING GROUP (para controle de camadas) - OPCIONAL ===
            // Nota: SortingGroup pode não estar disponível em todas as versões
            // Se necessário, adicione manualmente depois
            /*
            SortingGroup sortingGroup = gameObject.GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = gameObject.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerName = "Default";
                sortingGroup.sortingOrder = 0;
                Debug.Log("[NPCCreationTool] • SortingGroup adicionado");
            }
            */

            // === COLLIDER2D (opcional mas recomendado) ===
            Collider2D collider = gameObject.GetComponent<Collider2D>();
            if (collider == null)
            {
                CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                circleCollider.radius = 0.5f;
                circleCollider.isTrigger = false;
                Debug.Log("[NPCCreationTool] • CircleCollider2D adicionado");
            }

            Debug.Log("[NPCCreationTool] ✅ Componentes obrigatórios configurados");
        }

        /// <summary>
        /// Cria a estrutura visual necessária para NPCs direcionais
        /// </summary>
        /// <param name="gameObject">GameObject pai</param>
        private static void CreateVisualStructure(GameObject gameObject)
        {
            Debug.Log($"[NPCCreationTool] Criando estrutura visual para '{gameObject.name}'...");

            // Objetos visuais principais
            CreateChildObjectIfNotExists(gameObject, "front", "Sprite visual frontal (padrão)");
            CreateChildObjectIfNotExists(gameObject, "back", "Sprite visual traseiro");
            CreateChildObjectIfNotExists(gameObject, "side", "Sprite visual lateral");

            // Objetos VFX opcionais
            CreateChildObjectIfNotExists(gameObject, "vfx_front", "VFX frontal (opcional)");
            CreateChildObjectIfNotExists(gameObject, "vfx_back", "VFX traseiro (opcional)");
            CreateChildObjectIfNotExists(gameObject, "vfx_side", "VFX lateral (opcional)");

            // Sombra opcional
            CreateChildObjectIfNotExists(gameObject, "shadow", "Sombra do NPC (opcional)");

            // Configura objeto front como ativo por padrão
            Transform frontTransform = gameObject.transform.Find("front");
            if (frontTransform != null)
            {
                frontTransform.gameObject.SetActive(true);
            }

            // Desativa outros objetos por padrão
            string[] objectsToDeactivate = { "back", "side", "vfx_front", "vfx_back", "vfx_side" };
            foreach (string objName in objectsToDeactivate)
            {
                Transform objTransform = gameObject.transform.Find(objName);
                if (objTransform != null)
                {
                    objTransform.gameObject.SetActive(false);
                }
            }

            Debug.Log("[NPCCreationTool] ✅ Estrutura visual criada");
        }

        /// <summary>
        /// Cria um objeto filho se ele não existir
        /// </summary>
        /// <param name="parent">GameObject pai</param>
        /// <param name="childName">Nome do objeto filho</param>
        /// <param name="description">Descrição para log</param>
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

                Debug.Log($"[NPCCreationTool] • {childName} criado: {description}");
            }
        }

        /// <summary>
        /// Configurações finais do NPC
        /// </summary>
        /// <param name="gameObject">GameObject do NPC</param>
        private static void FinalizeNPCSetup(GameObject gameObject)
        {
            Debug.Log($"[NPCCreationTool] Aplicando configurações finais para '{gameObject.name}'...");

            // Define uma tag NPC se ela existir
            string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;
            if (allTags.Contains("NPC"))
            {
                gameObject.tag = "NPC";
                Debug.Log("[NPCCreationTool] • Tag 'NPC' aplicada");
            }

            // Define layer NPC se existir
            if (LayerMask.NameToLayer("NPC") != -1)
            {
                gameObject.layer = LayerMask.NameToLayer("NPC");
                Debug.Log("[NPCCreationTool] • Layer 'NPC' aplicado");
            }

            // Renomeia para seguir convenção se necessário
            if (!gameObject.name.StartsWith("NPC_") && !gameObject.name.Contains("NPC"))
            {
                string oldName = gameObject.name;
                gameObject.name = $"NPC_{gameObject.name}";
                Debug.Log($"[NPCCreationTool] • Nome alterado: '{oldName}' → '{gameObject.name}'");
            }

            Debug.Log("[NPCCreationTool] ✅ Configurações finais aplicadas");

            // Força atualização da interface
            EditorUtility.SetDirty(gameObject);
        }

        private static void ConfigureNPCController(NPCController controller, GameObject npcObject)
        {
            // Configura os campos através de SerializedObject (mais seguro para o editor)

            // Busca objetos filhos baseado na estrutura do NPCTemplate
            Transform front = npcObject.transform.Find("front");
            Transform back = npcObject.transform.Find("back");
            Transform side = npcObject.transform.Find("side");
            Transform vfxFront = npcObject.transform.Find("vfx_front");
            Transform vfxBack = npcObject.transform.Find("vfx_back");
            Transform vfxSide = npcObject.transform.Find("vfx_side");

            // Configura os campos através de SerializedObject (mais seguro para o editor)
            var serializedObject = new UnityEditor.SerializedObject(controller);

            SetSerializedProperty(serializedObject, "frontObject", front?.gameObject);
            SetSerializedProperty(serializedObject, "backObject", back?.gameObject);
            SetSerializedProperty(serializedObject, "sideObject", side?.gameObject);
            SetSerializedProperty(serializedObject, "vfxFrontObject", vfxFront?.gameObject);
            SetSerializedProperty(serializedObject, "vfxBackObject", vfxBack?.gameObject);
            SetSerializedProperty(serializedObject, "vfxSideObject", vfxSide?.gameObject);

            // Configurações padrão para NPC
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

            Debug.Log($"[NPCCreationTool] Configuração concluída! {objectsFound} objetos visuais direcionais encontrados.");
        }

        /// <summary>
        /// Configura o Animator existente do GameObject para trabalhar com NPCController
        /// Adiciona parâmetros necessários se não existirem e configura adequadamente
        /// </summary>
        /// <param name="gameObject">GameObject com o Animator a ser configurado</param>
        private static void ConfigureExistingAnimator(GameObject gameObject)
        {
            Animator animator = gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{gameObject.name}' não possui componente Animator!");
                return;
            }

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{gameObject.name}' não possui um AnimatorController configurado! Use o controller existente.");
                return;
            }

            Debug.Log($"[NPCCreationTool] Configurando Animator existente '{controller.name}' para NPC...");

            try
            {
                bool hasChanges = false;

                // === VERIFICA PARÂMETROS EXISTENTES ===
                var existingParams = new List<string>();
                foreach (var param in controller.parameters)
                {
                    existingParams.Add($"{param.name} ({param.type})");
                }
                Debug.Log($"[NPCCreationTool] 📋 Parâmetros existentes: {string.Join(", ", existingParams)}");

                // === VERIFICA E ADICIONA APENAS PARÂMETROS NECESSÁRIOS ===
                // O NPCController precisa de isWalking para controlar animação de movimento
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
                        Debug.Log($"[NPCCreationTool] ✅ Parâmetro 'isWalking' já existe");
                    }
                    if (existingParam.name == "FacingRight" && existingParam.type == AnimatorControllerParameterType.Bool)
                    {
                        hasFacingRight = true;
                        Debug.Log($"[NPCCreationTool] ✅ Parâmetro 'FacingRight' já existe");
                    }
                    if (existingParam.name == "Hit" && existingParam.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasHitTrigger = true;
                        Debug.Log($"[NPCCreationTool] ✅ Parâmetro 'Hit' (Trigger) já existe");
                    }
                    if (existingParam.name == "Attack" && existingParam.type == AnimatorControllerParameterType.Trigger)
                    {
                        hasAttackTrigger = true;
                        Debug.Log($"[NPCCreationTool] ✅ Parâmetro 'Attack' (Trigger) já existe");
                    }
                }

                // Adiciona isWalking se não existir (necessário para movimento)
                if (!hasIsWalking)
                {
                    controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
                    // Configura valor padrão
                    foreach (var param in controller.parameters)
                    {
                        if (param.name == "isWalking")
                        {
                            param.defaultBool = false;
                            break;
                        }
                    }
                    Debug.Log($"[NPCCreationTool] ➕ Parâmetro 'isWalking' (Bool) adicionado - necessário para movimento");
                    hasChanges = true;
                }

                // Adiciona FacingRight se não existir (necessário para direção)
                if (!hasFacingRight)
                {
                    controller.AddParameter("FacingRight", AnimatorControllerParameterType.Bool);
                    // Configura valor padrão
                    foreach (var param in controller.parameters)
                    {
                        if (param.name == "FacingRight")
                        {
                            param.defaultBool = true;
                            break;
                        }
                    }
                    Debug.Log($"[NPCCreationTool] ➕ Parâmetro 'FacingRight' (Bool) adicionado - necessário para direção");
                    hasChanges = true;
                }

                // Adiciona Hit se não existir (necessário para combat system)
                if (!hasHitTrigger)
                {
                    controller.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
                    Debug.Log($"[NPCCreationTool] ➕ Parâmetro 'Hit' (Trigger) adicionado - necessário para combat system");
                    hasChanges = true;
                }

                // Adiciona Attack se não existir (necessário para combat system)
                if (!hasAttackTrigger)
                {
                    controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);
                    Debug.Log($"[NPCCreationTool] ➕ Parâmetro 'Attack' (Trigger) adicionado - necessário para combat system");
                    hasChanges = true;
                }

                // === CONFIGURAÇÕES GERAIS DO ANIMATOR ===
                // Configura para não aplicar root motion (importante para NPCs)
                if (animator.applyRootMotion)
                {
                    animator.applyRootMotion = false;
                    Debug.Log($"[NPCCreationTool] ⚙️ Root Motion desabilitado para NPC");
                }

                // Configura Culling Mode para AlwaysAnimate se for diferente
                if (animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
                {
                    animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    Debug.Log($"[NPCCreationTool] ⚙️ Culling Mode configurado para AlwaysAnimate");
                }

                // === CONFIGURAÇÃO DE TODAS AS TRANSIÇÕES ===
                ConfigureAllTransitions(controller, ref hasChanges);

                // === SALVA AS MUDANÇAS ===
                if (hasChanges)
                {
                    EditorUtility.SetDirty(controller);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Debug.Log($"[NPCCreationTool] ✅ Animator Controller '{controller.name}' configurado com sucesso!");
                }
                else
                {
                    Debug.Log($"[NPCCreationTool] ✅ Animator Controller '{controller.name}' já possui parâmetros necessários");
                }

                // === LOGGING DE INFORMAÇÕES DO CONTROLLER ===
                Debug.Log($"[NPCCreationTool] 📊 Controller: {controller.name}");
                Debug.Log($"[NPCCreationTool] 📊 Parâmetros: {controller.parameters.Length}");
                Debug.Log($"[NPCCreationTool] 📊 Layers: {controller.layers.Length}");

                if (controller.layers.Length > 0 && controller.layers[0].stateMachine.states != null)
                {
                    Debug.Log($"[NPCCreationTool] 📊 Estados: {controller.layers[0].stateMachine.states.Length}");
                    var stateNames = controller.layers[0].stateMachine.states.Select(s => s.state.name).ToArray();
                    Debug.Log($"[NPCCreationTool] 📊 Lista de Estados: {string.Join(", ", stateNames)}");
                }

                Debug.Log($"[NPCCreationTool] 💡 O NPC manterá os estados existentes ({string.Join(", ", controller.layers[0].stateMachine.states.Select(s => s.state.name))})");
                Debug.Log($"[NPCCreationTool] 💡 Apenas parâmetros básicos de movimento (isWalking, FacingRight) foram adicionados se necessário");

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[NPCCreationTool] Erro ao configurar Animator existente: {ex.Message}");
            }
        }

        /// <summary>
        /// Configura TODAS as transições do Animator Controller baseadas nos parâmetros existentes
        /// </summary>
        /// <param name="controller">Animator Controller a ser configurado</param>
        /// <param name="hasChanges">Referência para indicar se houve mudanças</param>
        private static void ConfigureAllTransitions(AnimatorController controller, ref bool hasChanges)
        {
            if (controller.layers.Length == 0) return;

            var baseLayer = controller.layers[0];
            var stateMachine = baseLayer.stateMachine;

            if (stateMachine.states == null) return;

            Debug.Log($"[NPCCreationTool] 🔄 Configurando TODAS as transições do Animator Controller...");

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

            // === VERIFICAÇÃO DE PARÂMETROS DISPONÍVEIS ===
            var parameterMap = new Dictionary<string, AnimatorControllerParameter>();
            foreach (var param in controller.parameters)
            {
                parameterMap[param.name.ToLower()] = param;
            }

            // === CONFIGURAÇÃO DE TRANSIÇÕES ESPECÍFICAS ===

            // 1. MOVIMENTO: Idle ↔ Walk
            ConfigureMovementTransitions(stateMap, parameterMap, ref hasChanges);

            // 2. COMBAT: Any → Attack (via trigger)
            ConfigureAttackTransitions(stateMap, parameterMap, stateMachine, ref hasChanges);

            // 3. DAMAGE: Any State → Hit (via trigger)  
            ConfigureDamageTransitions(stateMap, parameterMap, stateMachine, ref hasChanges);

            // 4. COMPLETION: Attack/Hit/Unique → Idle (via exit time)
            ConfigureCompletionTransitions(stateMap, ref hasChanges);

            // 5. DEATH: Hit → Die (sem condições específicas, baseado em lógica de jogo)
            ConfigureDeathTransitions(stateMap, ref hasChanges);

            // 6. SPECIAL: Idle/Walk → Unique (sem condições para ações especiais)
            ConfigureSpecialTransitions(stateMap, ref hasChanges);

            // === CONFIGURA ESTADO PADRÃO ===
            if (stateMap.ContainsKey("idle") && stateMachine.defaultState != stateMap["idle"])
            {
                stateMachine.defaultState = stateMap["idle"];
                Debug.Log($"[NPCCreationTool] ⚙️ Estado padrão configurado: Idle");
                hasChanges = true;
            }

            Debug.Log($"[NPCCreationTool] ✅ Configuração completa de todas as transições concluída");
        }

        private static void ConfigureMovementTransitions(Dictionary<string, AnimatorState> stateMap, Dictionary<string, AnimatorControllerParameter> parameterMap, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("idle") || !stateMap.ContainsKey("walk"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estados Idle ou Walk não encontrados - transições de movimento ignoradas");
                return;
            }

            var idleState = stateMap["idle"];
            var walkState = stateMap["walk"];

            Debug.Log($"[NPCCreationTool] 🚶 Configurando transições de movimento...");

            // Idle → Walk (quando isWalking = true)
            ConfigureOrCreateTransition(idleState, walkState, "isWalking", AnimatorConditionMode.If, parameterMap, ref hasChanges);

            // Walk → Idle (quando isWalking = false)
            ConfigureOrCreateTransition(walkState, idleState, "isWalking", AnimatorConditionMode.IfNot, parameterMap, ref hasChanges);
        }

        private static void ConfigureAttackTransitions(Dictionary<string, AnimatorState> stateMap, Dictionary<string, AnimatorControllerParameter> parameterMap, AnimatorStateMachine stateMachine, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("attack"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Attack não encontrado - transições de ataque ignoradas");
                return;
            }

            var attackState = stateMap["attack"];
            Debug.Log($"[NPCCreationTool] ⚔️ Configurando transições de ataque...");

            // Configura transições dos estados básicos para Attack
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

            // Any State → Attack também (para triggers universais)
            ConfigureAnyStateTransition(stateMachine, attackState, "Attack", AnimatorConditionMode.If, parameterMap, ref hasChanges);
        }

        private static void ConfigureDamageTransitions(Dictionary<string, AnimatorState> stateMap, Dictionary<string, AnimatorControllerParameter> parameterMap, AnimatorStateMachine stateMachine, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("hit"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Hit não encontrado - transições de dano ignoradas");
                return;
            }

            var hitState = stateMap["hit"];
            Debug.Log($"[NPCCreationTool] 💥 Configurando transições de dano...");

            // Any State → Hit (via trigger Hit)
            ConfigureAnyStateTransition(stateMachine, hitState, "Hit", AnimatorConditionMode.If, parameterMap, ref hasChanges);
        }

        private static void ConfigureCompletionTransitions(Dictionary<string, AnimatorState> stateMap, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("idle"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Idle não encontrado - transições de conclusão ignoradas");
                return;
            }

            var idleState = stateMap["idle"];
            Debug.Log($"[NPCCreationTool] 🔄 Configurando transições de conclusão...");

            // Estados que devem retornar ao Idle após conclusão
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
                Debug.LogWarning($"[NPCCreationTool] Estados Hit ou Die não encontrados - transições de morte ignoradas");
                return;
            }

            var hitState = stateMap["hit"];
            var dieState = stateMap["die"];

            Debug.Log($"[NPCCreationTool] 💀 Configurando transições de morte...");

            // Hit → Die (via exit time, para lógica de jogo determinar quando morrer)
            ConfigureExitTimeTransition(hitState, dieState, ref hasChanges);
        }

        private static void ConfigureSpecialTransitions(Dictionary<string, AnimatorState> stateMap, ref bool hasChanges)
        {
            if (!stateMap.ContainsKey("unique"))
            {
                Debug.LogWarning($"[NPCCreationTool] Estado Unique não encontrado - transições especiais ignoradas");
                return;
            }

            var uniqueState = stateMap["unique"];
            Debug.Log($"[NPCCreationTool] ⭐ Configurando transições especiais...");

            // Estados básicos podem ir para Unique (sem condições específicas)
            string[] basicStates = { "idle", "walk" };

            foreach (string stateName in basicStates)
            {
                if (stateMap.ContainsKey(stateName))
                {
                    var state = stateMap[stateName];

                    // Verifica se já existe transição para Unique
                    bool hasTransitionToUnique = false;
                    foreach (var transition in state.transitions)
                    {
                        if (transition.destinationState == uniqueState)
                        {
                            hasTransitionToUnique = true;
                            break;
                        }
                    }

                    // Preserva transição existente (pode ser controlada via script)
                    if (!hasTransitionToUnique)
                    {
                        var newTransition = state.AddTransition(uniqueState);
                        newTransition.hasExitTime = true;
                        newTransition.exitTime = 0.8f; // Permite interrupção tardia
                        newTransition.duration = 0.1f;

                        Debug.Log($"[NPCCreationTool] ➕ Transição {state.name} → {uniqueState.name} criada (exit time)");
                        hasChanges = true;
                    }
                }
            }
        }

        private static void ConfigureOrCreateTransition(AnimatorState fromState, AnimatorState toState, string parameterName, AnimatorConditionMode mode, Dictionary<string, AnimatorControllerParameter> parameterMap, ref bool hasChanges)
        {
            if (!parameterMap.ContainsKey(parameterName.ToLower()))
            {
                Debug.LogWarning($"[NPCCreationTool] Parâmetro '{parameterName}' não encontrado - transição {fromState.name} → {toState.name} ignorada");
                return;
            }

            // Verifica se já existe transição com condição correta
            bool foundCorrectTransition = false;

            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == toState)
                {
                    // Verifica condições existentes
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
                        // Remove condições antigas se existirem
                        for (int i = transition.conditions.Length - 1; i >= 0; i--)
                        {
                            transition.RemoveCondition(transition.conditions[i]);
                        }

                        // Adiciona condição correta
                        transition.AddCondition(mode, 0, parameterName);

                        // Otimiza para responsividade
                        transition.hasExitTime = false;
                        transition.hasFixedDuration = true;
                        transition.duration = 0.1f;
                        transition.offset = 0f;

                        Debug.Log($"[NPCCreationTool] 🔧 Transição {fromState.name} → {toState.name} atualizada com condição {parameterName} {mode}");
                        hasChanges = true;
                    }
                    else
                    {
                        Debug.Log($"[NPCCreationTool] ✅ Transição {fromState.name} → {toState.name} já configurada corretamente");
                    }

                    foundCorrectTransition = true;
                    break;
                }
            }

            // Cria nova transição se necessário
            if (!foundCorrectTransition)
            {
                var newTransition = fromState.AddTransition(toState);
                newTransition.AddCondition(mode, 0, parameterName);
                newTransition.hasExitTime = false;
                newTransition.hasFixedDuration = true;
                newTransition.duration = 0.1f;
                newTransition.offset = 0f;

                Debug.Log($"[NPCCreationTool] ➕ Transição criada: {fromState.name} → {toState.name} (quando {parameterName} {mode})");
                hasChanges = true;
            }
        }

        private static void ConfigureAnyStateTransition(AnimatorStateMachine stateMachine, AnimatorState toState, string parameterName, AnimatorConditionMode mode, Dictionary<string, AnimatorControllerParameter> parameterMap, ref bool hasChanges)
        {
            if (!parameterMap.ContainsKey(parameterName.ToLower()))
            {
                Debug.LogWarning($"[NPCCreationTool] Parâmetro '{parameterName}' não encontrado - Any State → {toState.name} ignorada");
                return;
            }

            // Verifica se já existe Any State transition
            bool foundCorrectTransition = false;

            foreach (var transition in stateMachine.anyStateTransitions)
            {
                if (transition.destinationState == toState)
                {
                    // Verifica condições
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
                        // Remove condições antigas
                        for (int i = transition.conditions.Length - 1; i >= 0; i--)
                        {
                            transition.RemoveCondition(transition.conditions[i]);
                        }

                        // Adiciona condição correta
                        transition.AddCondition(mode, 0, parameterName);
                        transition.hasExitTime = false;
                        transition.hasFixedDuration = true;
                        transition.duration = 0.0f; // Imediata para triggers
                        transition.offset = 0f;

                        Debug.Log($"[NPCCreationTool] 🔧 Any State → {toState.name} atualizada com condição {parameterName} {mode}");
                        hasChanges = true;
                    }
                    else
                    {
                        Debug.Log($"[NPCCreationTool] ✅ Any State → {toState.name} já configurada corretamente");
                    }

                    foundCorrectTransition = true;
                    break;
                }
            }

            // Cria nova Any State transition se necessário
            if (!foundCorrectTransition)
            {
                var newTransition = stateMachine.AddAnyStateTransition(toState);
                newTransition.AddCondition(mode, 0, parameterName);
                newTransition.hasExitTime = false;
                newTransition.hasFixedDuration = true;
                newTransition.duration = 0.0f;
                newTransition.offset = 0f;

                Debug.Log($"[NPCCreationTool] ➕ Any State → {toState.name} criada (quando {parameterName} {mode})");
                hasChanges = true;
            }
        }

        private static void ConfigureExitTimeTransition(AnimatorState fromState, AnimatorState toState, ref bool hasChanges)
        {
            // Verifica se já existe transição baseada em exit time
            bool foundExitTimeTransition = false;

            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == toState && transition.hasExitTime && transition.conditions.Length == 0)
                {
                    foundExitTimeTransition = true;
                    Debug.Log($"[NPCCreationTool] ✅ Transição exit time {fromState.name} → {toState.name} já existe");
                    break;
                }
            }

            // Cria transição baseada em exit time se necessário
            if (!foundExitTimeTransition)
            {
                var newTransition = fromState.AddTransition(toState);
                newTransition.hasExitTime = true;
                newTransition.exitTime = 1.0f; // Após completar animação
                newTransition.hasFixedDuration = true;
                newTransition.duration = 0.0f; // Transição imediata após exit time
                newTransition.offset = 0f;

                Debug.Log($"[NPCCreationTool] ➕ Transição exit time criada: {fromState.name} → {toState.name}");
                hasChanges = true;
            }
        }

        /// <summary>
        /// Cria um Animator Controller completo para NPCs com todos os parâmetros e transições necessários
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

                // Cria a pasta se não existir
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

                // === CONFIGURAÇÃO DOS PARÂMETROS ===
                // Adiciona parâmetros Bool
                controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
                controller.AddParameter("FacingRight", AnimatorControllerParameterType.Bool);

                // Configura valor padrão para FacingRight
                foreach (var param in controller.parameters)
                {
                    if (param.name == "FacingRight")
                    {
                        param.defaultBool = true; // NPCs começam virados para direita
                    }
                }

                // === CONFIGURAÇÃO DOS ESTADOS ===
                // Obtém a layer base (sempre existe)
                AnimatorControllerLayer baseLayer = controller.layers[0];
                AnimatorStateMachine rootStateMachine = baseLayer.stateMachine;

                // Estado Idle (padrão)
                AnimatorState idleState = rootStateMachine.AddState("Idle", new Vector3(300, 0, 0));
                idleState.writeDefaultValues = true;

                // Estado Walking
                AnimatorState walkingState = rootStateMachine.AddState("Walking", new Vector3(300, 100, 0));
                walkingState.writeDefaultValues = true;

                // Define Idle como estado padrão
                rootStateMachine.defaultState = idleState;

                // === CONFIGURAÇÃO DAS TRANSIÇÕES ===
                // Idle → Walking (quando isWalking = true)
                AnimatorStateTransition idleToWalking = idleState.AddTransition(walkingState);
                idleToWalking.AddCondition(AnimatorConditionMode.If, 0, "isWalking");
                idleToWalking.hasExitTime = false;
                idleToWalking.hasFixedDuration = true;
                idleToWalking.duration = 0.1f;
                idleToWalking.offset = 0f;

                // Walking → Idle (quando isWalking = false)
                AnimatorStateTransition walkingToIdle = walkingState.AddTransition(idleState);
                walkingToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "isWalking");
                walkingToIdle.hasExitTime = false;
                walkingToIdle.hasFixedDuration = true;
                walkingToIdle.duration = 0.1f;
                walkingToIdle.offset = 0f;

                // === CONFIGURAÇÕES ADICIONAIS ===
                // Configura layer settings
                baseLayer.name = "Base Layer";
                baseLayer.defaultWeight = 1f;

                // Salva as mudanças
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"[NPCCreationTool] Animator Controller criado com sucesso: {fullPath}");
                Debug.Log($"[NPCCreationTool] Parâmetros criados: isWalking (Bool), FacingRight (Bool)");
                Debug.Log($"[NPCCreationTool] Estados criados: Idle (padrão), Walking");
                Debug.Log($"[NPCCreationTool] Transições criadas: Idle ↔ Walking baseadas em isWalking");

                return controller;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[NPCCreationTool] Erro ao criar Animator Controller: {ex.Message}");
                return null;
            }
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
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' não possui componente Animator!");
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
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' não possui componente Animator!");
                return;
            }

            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
            {
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' não possui AnimatorController configurado!");
                return;
            }

            // Valida parâmetros necessários
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
                    Debug.LogError($"[NPCCreationTool] Parâmetro obrigatório não encontrado: {paramName} (Bool)");
                    allParametersFound = false;
                }
            }

            // Valida estados necessários
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
                    Debug.LogError($"[NPCCreationTool] Estado obrigatório não encontrado: {stateName}");
                    allStatesFound = false;
                }
            }

            // Resultado da validação
            if (allParametersFound && allStatesFound)
            {
                Debug.Log($"[NPCCreationTool] ✅ Animator Controller de '{selected.name}' está configurado corretamente!");
            }
            else
            {
                Debug.LogWarning($"[NPCCreationTool] ⚠️ Animator Controller de '{selected.name}' possui problemas de configuração.");
            }
        }

        [UnityEditor.MenuItem("Extra Tools/NPC/Test NPC Movement")]
        public static void TestNPCMovement()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("[NPCCreationTool] Esta função só funciona durante o Play Mode!");
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
                Debug.LogWarning($"[NPCCreationTool] GameObject '{selected.name}' não possui NPCController!");
                return;
            }

            // Alterna o tipo de movimento para testar
            var currentType = npcController.GetType()
                .GetField("movementType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(npcController);

            if (currentType.ToString() == "Idle")
            {
                npcController.SetMovementType(NPCController.MovementType.Wander);
                Debug.Log($"[NPCCreationTool] '{selected.name}' agora está em modo Wander!");
            }
            else
            {
                npcController.SetMovementType(NPCController.MovementType.Idle);
                Debug.Log($"[NPCCreationTool] '{selected.name}' agora está em modo Idle!");
            }
        }
    }
}