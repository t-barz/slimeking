using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para configurar um GameObject como NPC automaticamente.
    /// Adiciona colliders, rigidbody e configurações necessárias para detecção de ataques do Player.
    /// </summary>
    public static class NPCSetupTool
    {
        // Menu principal
        [MenuItem("Extra Tools/Setup/Setup as NPC", true)]
        private static bool ValidateSetupAsNPC()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("Extra Tools/Setup/Setup as NPC")]
        public static void SetupAsNPC()
        {
            ExecuteSetup();
        }

        // Menu de contexto (clique direito)
        [MenuItem("GameObject/Extra Tools/Setup as NPC", true)]
        private static bool ValidateContextSetupAsNPC()
        {
            return Selection.activeGameObject != null;
        }

        [MenuItem("GameObject/Extra Tools/Setup as NPC")]
        public static void ContextSetupAsNPC()
        {
            ExecuteSetup();
        }

        private static void ExecuteSetup()
        {
            if (Selection.activeGameObject == null)
            {
                UnityEngine.Debug.LogWarning("[NPCSetupTool] Nenhum GameObject selecionado.");
                return;
            }

            GameObject target = Selection.activeGameObject;
            UnityEngine.Debug.Log($"[NPCSetupTool] Configurando '{target.name}' como NPC...");

            // Registra operação para Undo
            Undo.RegisterCompleteObjectUndo(target, "Setup as NPC");

            try
            {
                SetupAttributesHandler(target);
                SetupController(target);
                SetupColliders(target);
                SetupRigidbody(target);
                SetupTag(target);
                SetupAnimator(target);

                UnityEngine.Debug.Log($"[NPCSetupTool] ✅ '{target.name}' configurado como NPC com sucesso!");

                // Marca objeto como modificado para salvar
                EditorUtility.SetDirty(target);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[NPCSetupTool] ❌ Erro ao configurar NPC: {ex.Message}");
            }
        }

        private static void SetupAttributesHandler(GameObject target)
        {
            // Verifica se já existe NPCAttributesHandler
            var existingHandler = target.GetComponent<TheSlimeKing.NPCs.NPCAttributesHandler>();
            if (existingHandler != null)
            {
                UnityEngine.Debug.Log($"[NPCSetupTool] NPCAttributesHandler já existe - mantendo configuração atual");
                return;
            }

            // Adiciona novo NPCAttributesHandler
            var attributesHandler = Undo.AddComponent<TheSlimeKing.NPCs.NPCAttributesHandler>(target);

            UnityEngine.Debug.Log($"[NPCSetupTool] NPCAttributesHandler adicionado com valores padrão");
        }

        private static void SetupController(GameObject target)
        {
            // Verifica se já existe NPCBaseController
            var existingController = target.GetComponent<TheSlimeKing.NPCs.NPCBaseController>();
            if (existingController != null)
            {
                UnityEngine.Debug.Log($"[NPCSetupTool] NPCBaseController já existe - mantendo configuração atual");
                return;
            }

            // Adiciona NPCBaseController
            var controller = Undo.AddComponent<TheSlimeKing.NPCs.NPCBaseController>(target);

            UnityEngine.Debug.Log($"[NPCSetupTool] NPCBaseController adicionado");
        }

        private static void SetupColliders(GameObject target)
        {
            var existingColliders = target.GetComponents<CircleCollider2D>();

            // Verifica se já existem os colliders necessários com configurações corretas
            CircleCollider2D physicsCollider = null;
            CircleCollider2D attackDetector = null;

            foreach (var collider in existingColliders)
            {
                // Verifica se é um collider físico (não-trigger com radius ~0.12)
                if (!collider.isTrigger &&
                    Mathf.Approximately(collider.radius, 0.12f) &&
                    Vector2.Distance(collider.offset, new Vector2(0f, 0.12f)) < 0.01f)
                {
                    physicsCollider = collider;
                }
                // Verifica se é um detector de ataque (trigger com radius ~0.15)
                else if (collider.isTrigger &&
                         Mathf.Approximately(collider.radius, 0.15f) &&
                         Vector2.Distance(collider.offset, new Vector2(0f, 0.12f)) < 0.01f)
                {
                    attackDetector = collider;
                }
            }

            // Cria collider físico se não existir
            if (physicsCollider == null)
            {
                physicsCollider = Undo.AddComponent<CircleCollider2D>(target);
                physicsCollider.radius = 0.12f;
                physicsCollider.offset = new Vector2(0f, 0.12f);
                physicsCollider.isTrigger = false;
                UnityEngine.Debug.Log($"[NPCSetupTool] Physics Collider criado (radius: {physicsCollider.radius})");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCSetupTool] Physics Collider já existe e está configurado corretamente");
            }

            // Cria collider de detecção se não existir
            if (attackDetector == null)
            {
                attackDetector = Undo.AddComponent<CircleCollider2D>(target);
                attackDetector.radius = 0.15f;
                attackDetector.offset = new Vector2(0f, 0.12f);
                attackDetector.isTrigger = true;
                UnityEngine.Debug.Log($"[NPCSetupTool] Attack Detector criado (radius: {attackDetector.radius})");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCSetupTool] Attack Detector já existe e está configurado corretamente");
            }

            UnityEngine.Debug.Log($"[NPCSetupTool] Colliders configurados: Physics (radius: {physicsCollider.radius}) + Attack Detector (radius: {attackDetector.radius})");
        }

        private static void SetupRigidbody(GameObject target)
        {
            // Verifica se já existe Rigidbody2D
            var existingRb = target.GetComponent<Rigidbody2D>();
            if (existingRb != null)
            {
                // Verifica se já está configurado corretamente
                if (existingRb.bodyType == RigidbodyType2D.Kinematic &&
                    existingRb.freezeRotation == true &&
                    existingRb.gravityScale == 0f)
                {
                    UnityEngine.Debug.Log($"[NPCSetupTool] Rigidbody2D já existe e está configurado corretamente");
                    return;
                }
                else
                {
                    // Configura o existente
                    Undo.RecordObject(existingRb, "Setup NPC Rigidbody");
                    existingRb.bodyType = RigidbodyType2D.Kinematic;
                    existingRb.freezeRotation = true;
                    existingRb.gravityScale = 0f;
                    UnityEngine.Debug.Log($"[NPCSetupTool] Rigidbody2D existente reconfigurado como Kinematic");
                    return;
                }
            }

            // Adiciona novo Rigidbody2D
            Rigidbody2D rb = Undo.AddComponent<Rigidbody2D>(target);
            rb.bodyType = RigidbodyType2D.Kinematic; // Não afetado por física
            rb.freezeRotation = true; // Não rotaciona
            rb.gravityScale = 0f; // Sem gravidade

            UnityEngine.Debug.Log($"[NPCSetupTool] Rigidbody2D configurado como Kinematic");
        }

        private static void SetupTag(GameObject target)
        {
            // Verifica se a tag "Enemy" existe
            if (!DoesTagExist("Enemy"))
            {
                // Se não existe, usar "Untagged" e avisar
                target.tag = "Untagged";
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ Tag 'Enemy' não encontrada. Configure manualmente a tag '{target.name}' no Inspector.");
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] Para criar a tag: Edit > Project Settings > Tags and Layers > Tags");
            }
            else
            {
                // Se existe, aplicar
                target.tag = "Enemy";
                UnityEngine.Debug.Log($"[NPCSetupTool] Tag configurada como 'Enemy'");
            }
        }

        private static bool DoesTagExist(string tagName)
        {
            // Verifica se a tag existe na lista de tags do projeto
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            return System.Array.Exists(tags, tag => tag == tagName);
        }

        private static void SetupAnimator(GameObject target)
        {
            var animator = target.GetComponent<Animator>();
            if (animator == null)
            {
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ '{target.name}' não possui Animator. Adicione um Animator e execute novamente.");
                return;
            }

            var animatorController = animator.runtimeAnimatorController as AnimatorController;
            if (animatorController == null)
            {
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ '{target.name}' não possui AnimatorController configurado.");
                return;
            }

            // Registra para Undo
            Undo.RecordObject(animatorController, "Setup NPC Animator");

            try
            {
                // Adiciona parâmetro Hit se não existir
                bool hitExists = false;
                foreach (var param in animatorController.parameters)
                {
                    if (param.name == "Hit" && param.type == AnimatorControllerParameterType.Trigger)
                    {
                        hitExists = true;
                        break;
                    }
                }

                if (!hitExists)
                {
                    animatorController.AddParameter("Hit", AnimatorControllerParameterType.Trigger);
                    UnityEngine.Debug.Log($"[NPCSetupTool] Parâmetro 'Hit' (Trigger) adicionado ao Animator");
                }
                else
                {
                    UnityEngine.Debug.Log($"[NPCSetupTool] Parâmetro 'Hit' já existe no Animator");
                }

                // Adiciona parâmetro isDying se não existir
                bool isDyingExists = false;
                foreach (var param in animatorController.parameters)
                {
                    if (param.name == "isDying" && param.type == AnimatorControllerParameterType.Bool)
                    {
                        isDyingExists = true;
                        break;
                    }
                }

                if (!isDyingExists)
                {
                    animatorController.AddParameter("isDying", AnimatorControllerParameterType.Bool);
                    UnityEngine.Debug.Log($"[NPCSetupTool] Parâmetro 'isDying' (Bool) adicionado ao Animator");
                }
                else
                {
                    UnityEngine.Debug.Log($"[NPCSetupTool] Parâmetro 'isDying' já existe no Animator");
                }

                // Adiciona parâmetro isWalking se não existir
                bool isWalkingExists = false;
                foreach (var param in animatorController.parameters)
                {
                    if (param.name == "isWalking" && param.type == AnimatorControllerParameterType.Bool)
                    {
                        isWalkingExists = true;
                        break;
                    }
                }

                if (!isWalkingExists)
                {
                    animatorController.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
                    UnityEngine.Debug.Log($"[NPCSetupTool] Parâmetro 'isWalking' (Bool) adicionado ao Animator");
                }
                else
                {
                    UnityEngine.Debug.Log($"[NPCSetupTool] Parâmetro 'isWalking' já existe no Animator");
                }

                // Configura transições de estado
                SetupAnimatorTransitions(animatorController);

                // Marca como modificado
                EditorUtility.SetDirty(animatorController);

                UnityEngine.Debug.Log($"[NPCSetupTool] Animator configurado com parâmetros 'Hit', 'isDying', 'isWalking' e transições completas");
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"[NPCSetupTool] Erro ao configurar Animator: {ex.Message}");
            }
        }

        private static void SetupAnimatorTransitions(AnimatorController controller)
        {
            var rootStateMachine = controller.layers[0].stateMachine;

            // Busca estados Idle, Walk, Hit e Die
            AnimatorState idleState = null;
            AnimatorState walkState = null;
            AnimatorState hitState = null;
            AnimatorState dieState = null;

            foreach (var state in rootStateMachine.states)
            {
                string stateName = state.state.name.ToLower();
                if (stateName.Contains("idle"))
                    idleState = state.state;
                else if (stateName.Contains("walk") || stateName.Contains("move"))
                    walkState = state.state;
                else if (stateName.Contains("hit") || stateName.Contains("hurt") || stateName.Contains("damage"))
                    hitState = state.state;
                else if (stateName.Contains("die") || stateName.Contains("death") || stateName.Contains("dead"))
                    dieState = state.state;
            }

            // Se não encontrou estado Hit, avisa mas não cria (deve existir no Animator)
            if (hitState == null)
            {
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ Estado 'Hit' não encontrado. Verifique se existe um estado de Hit/Hurt/Damage no Animator.");
                return;
            }

            // Cria transições instantâneas para o estado Hit
            if (idleState != null)
            {
                CreateHitTransition(idleState, hitState, "Idle");
            }

            if (walkState != null)
            {
                CreateHitTransition(walkState, hitState, "Walk");
            }

            // Configura transições de saída do estado Hit
            SetupHitExitTransitions(hitState, idleState, dieState);

            // Configura transições Idle ↔ Walk baseadas em isWalking
            SetupWalkingTransitions(idleState, walkState);
        }

        private static void CreateHitTransition(AnimatorState fromState, AnimatorState hitState, string fromStateName)
        {
            // Verifica se já existe transição
            bool transitionExists = false;
            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == hitState)
                {
                    // Verifica se tem a condição Hit
                    foreach (var condition in transition.conditions)
                    {
                        if (condition.parameter == "Hit")
                        {
                            transitionExists = true;
                            break;
                        }
                    }
                    if (transitionExists) break;
                }
            }

            if (!transitionExists)
            {
                var transition = fromState.AddTransition(hitState);
                transition.AddCondition(AnimatorConditionMode.If, 0, "Hit");
                transition.duration = 0f; // Transição instantânea
                transition.hasExitTime = false; // Sem tempo de saída
                transition.hasFixedDuration = true;

                UnityEngine.Debug.Log($"[NPCSetupTool] Transição {fromStateName} → Hit criada");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCSetupTool] Transição {fromStateName} → Hit já existe");
            }
        }

        private static void SetupHitExitTransitions(AnimatorState hitState, AnimatorState idleState, AnimatorState dieState)
        {
            // Configura transição Hit → Die (quando isDying = true)
            if (dieState != null)
            {
                CreateConditionalTransition(hitState, dieState, "Hit", "Die", "isDying", true, true); // Instantânea
            }
            else
            {
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ Estado 'Die' não encontrado. Transição Hit → Die não será criada.");
            }

            // Configura transição Hit → Idle (quando isDying = false, com exit time)
            if (idleState != null)
            {
                CreateConditionalTransition(hitState, idleState, "Hit", "Idle", "isDying", false, false); // Com exit time
            }
            else
            {
                UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ Estado 'Idle' não encontrado. Transição Hit → Idle não será criada.");
            }
        }

        private static void SetupWalkingTransitions(AnimatorState idleState, AnimatorState walkState)
        {
            if (idleState == null || walkState == null)
            {
                if (idleState == null)
                    UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ Estado 'Idle' não encontrado. Transições de movimento não serão criadas.");
                if (walkState == null)
                    UnityEngine.Debug.LogWarning($"[NPCSetupTool] ⚠️ Estado 'Walk' não encontrado. Transições de movimento não serão criadas.");
                return;
            }

            // Transição Idle → Walk (isWalking = true)
            CreateWalkingTransition(idleState, walkState, "Idle", "Walk", true);

            // Transição Walk → Idle (isWalking = false)  
            CreateWalkingTransition(walkState, idleState, "Walk", "Idle", false);
        }

        private static void CreateWalkingTransition(AnimatorState fromState, AnimatorState toState, string fromStateName, string toStateName, bool isWalkingValue)
        {
            // Verifica se já existe transição com a condição isWalking
            bool transitionExists = false;
            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == toState)
                {
                    // Verifica se tem a condição isWalking
                    foreach (var condition in transition.conditions)
                    {
                        if (condition.parameter == "isWalking")
                        {
                            transitionExists = true;
                            break;
                        }
                    }
                    if (transitionExists) break;
                }
            }

            if (!transitionExists)
            {
                var transition = fromState.AddTransition(toState);

                // Configura condição isWalking
                AnimatorConditionMode conditionMode = isWalkingValue ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot;
                transition.AddCondition(conditionMode, 0, "isWalking");

                // Transição instantânea
                transition.duration = 0f;
                transition.hasExitTime = false;
                transition.hasFixedDuration = true;

                UnityEngine.Debug.Log($"[NPCSetupTool] Transição {fromStateName} → {toStateName} criada (instantânea, isWalking={isWalkingValue})");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCSetupTool] Transição {fromStateName} → {toStateName} já existe");
            }
        }

        private static void CreateConditionalTransition(AnimatorState fromState, AnimatorState toState, string fromStateName, string toStateName, string conditionParam, bool conditionValue, bool isInstant)
        {
            // Verifica se já existe transição
            bool transitionExists = false;
            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == toState)
                {
                    // Verifica se tem a condição especificada
                    foreach (var condition in transition.conditions)
                    {
                        if (condition.parameter == conditionParam)
                        {
                            transitionExists = true;
                            break;
                        }
                    }
                    if (transitionExists) break;
                }
            }

            if (!transitionExists)
            {
                var transition = fromState.AddTransition(toState);

                // Configura condição
                AnimatorConditionMode conditionMode = conditionValue ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot;
                transition.AddCondition(conditionMode, 0, conditionParam);

                // Configura timing
                if (isInstant)
                {
                    transition.duration = 0f; // Instantânea
                    transition.hasExitTime = false;
                }
                else
                {
                    transition.duration = 0.1f; // Pequena duração para suavidade
                    transition.hasExitTime = true;
                    transition.exitTime = 0.9f; // Sai quase no final da animação
                }

                transition.hasFixedDuration = true;

                string transitionType = isInstant ? "instantânea" : "com exit time";
                UnityEngine.Debug.Log($"[NPCSetupTool] Transição {fromStateName} → {toStateName} criada ({transitionType}, {conditionParam}={conditionValue})");
            }
            else
            {
                UnityEngine.Debug.Log($"[NPCSetupTool] Transição {fromStateName} → {toStateName} já existe");
            }
        }
    }
}