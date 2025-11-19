using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using System.Linq;
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

            // === CRIAÇÃO DO ANIMATOR CONTROLLER ===
            AnimatorController controller = CreateAnimatorControllerForNPC(selected.name);
            if (controller != null)
            {
                Animator animator = selected.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
            }

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

            // Cria e configura Animator Controller automaticamente
            AnimatorController controller = CreateAnimatorControllerForNPC(selected.name);
            if (controller != null)
            {
                Animator animator = selected.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.runtimeAnimatorController = controller;
                }
            }

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

        [UnityEditor.MenuItem("Extra Tools/NPC/Create Animator Controller")]
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