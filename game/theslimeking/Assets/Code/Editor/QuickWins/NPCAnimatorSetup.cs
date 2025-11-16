using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Configura Animator Controllers para NPCs com estados, parâmetros e transições.
    /// </summary>
    public static class NPCAnimatorSetup
    {
        private const string ANIMATOR_BASE_PATH = "Assets/Art/Animations/NPCs/";
        private const string PLACEHOLDER_ANIMATIONS_PATH = "Assets/Art/Animations/Placeholders/";

        /// <summary>
        /// Cria ou carrega um Animator Controller existente para o NPC.
        /// </summary>
        /// <param name="npcName">Nome do NPC</param>
        /// <returns>Animator Controller criado ou carregado</returns>
        public static AnimatorController CreateOrLoadController(string npcName)
        {
            if (string.IsNullOrEmpty(npcName))
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Nome do NPC não pode ser vazio!");
                return null;
            }

            // Garantir que o diretório existe
            if (!Directory.Exists(ANIMATOR_BASE_PATH))
            {
                Directory.CreateDirectory(ANIMATOR_BASE_PATH);
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log($"📁 Diretório criado: {ANIMATOR_BASE_PATH}");
            }

            // Caminho do controller
            string controllerPath = $"{ANIMATOR_BASE_PATH}{npcName}Controller.controller";

            // Tentar carregar controller existente
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);

            if (controller != null)
            {
                UnityEngine.Debug.Log($"✅ Animator Controller existente carregado: {controllerPath}");
                return controller;
            }

            // Criar novo controller
            controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            
            if (controller != null)
            {
                UnityEngine.Debug.Log($"🎬 Novo Animator Controller criado: {controllerPath}");
            }
            else
            {
                UnityEngine.Debug.LogError($"❌ Falha ao criar Animator Controller em: {controllerPath}");
            }

            return controller;
        }

        /// <summary>
        /// Configura os estados básicos do Animator (Idle, Walk, Talk, Death).
        /// </summary>
        /// <param name="controller">Animator Controller</param>
        /// <param name="includeDialogue">Se true, adiciona estado Talk</param>
        public static void ConfigureStates(AnimatorController controller, bool includeDialogue)
        {
            if (controller == null)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Controller não pode ser nulo!");
                return;
            }

            // Obter o Base Layer
            var layers = controller.layers;
            if (layers == null || layers.Length == 0)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Controller não possui layers!");
                return;
            }

            var stateMachine = layers[0].stateMachine;

            // Criar estados necessários
            var idleState = FindOrCreateState(stateMachine, "Idle");
            var walkState = FindOrCreateState(stateMachine, "Walk");
            var deathState = FindOrCreateState(stateMachine, "Death");

            // Definir Idle como estado padrão
            if (idleState != null)
            {
                stateMachine.defaultState = idleState;
                UnityEngine.Debug.Log("✅ Estado 'Idle' definido como padrão");
            }

            // Criar estado Talk se diálogo estiver habilitado
            if (includeDialogue)
            {
                var talkState = FindOrCreateState(stateMachine, "Talk");
                if (talkState != null)
                {
                    UnityEngine.Debug.Log("✅ Estado 'Talk' criado");
                }
            }

            EditorUtility.SetDirty(controller);
        }

        /// <summary>
        /// Encontra um estado existente ou cria um novo no state machine.
        /// </summary>
        /// <param name="stateMachine">State machine onde procurar/criar</param>
        /// <param name="stateName">Nome do estado</param>
        /// <returns>Estado encontrado ou criado</returns>
        public static AnimatorState FindOrCreateState(AnimatorStateMachine stateMachine, string stateName)
        {
            if (stateMachine == null)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: StateMachine não pode ser nulo!");
                return null;
            }

            // Procurar estado existente
            foreach (var childState in stateMachine.states)
            {
                if (childState.state != null && childState.state.name == stateName)
                {
                    UnityEngine.Debug.Log($"✅ Estado '{stateName}' já existe");
                    return childState.state;
                }
            }

            // Criar novo estado
            var newState = stateMachine.AddState(stateName);
            UnityEngine.Debug.Log($"🎬 Estado '{stateName}' criado");
            return newState;
        }

        /// <summary>
        /// Configura os parâmetros do Animator (Speed, IsDead, IsTalking).
        /// </summary>
        /// <param name="controller">Animator Controller</param>
        /// <param name="includeDialogue">Se true, adiciona parâmetro IsTalking</param>
        public static void ConfigureParameters(AnimatorController controller, bool includeDialogue)
        {
            if (controller == null)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Controller não pode ser nulo!");
                return;
            }

            // Adicionar parâmetro Speed (float)
            AddParameterIfNotExists(controller, "Speed", AnimatorControllerParameterType.Float);

            // Adicionar parâmetro IsDead (bool)
            AddParameterIfNotExists(controller, "IsDead", AnimatorControllerParameterType.Bool);

            // Adicionar parâmetro IsTalking (bool) se diálogo estiver habilitado
            if (includeDialogue)
            {
                AddParameterIfNotExists(controller, "IsTalking", AnimatorControllerParameterType.Bool);
            }

            EditorUtility.SetDirty(controller);
        }

        /// <summary>
        /// Adiciona um parâmetro ao controller se ele não existir.
        /// </summary>
        private static void AddParameterIfNotExists(AnimatorController controller, string parameterName, AnimatorControllerParameterType parameterType)
        {
            // Verificar se o parâmetro já existe
            foreach (var parameter in controller.parameters)
            {
                if (parameter.name == parameterName && parameter.type == parameterType)
                {
                    UnityEngine.Debug.Log($"✅ Parâmetro '{parameterName}' já existe");
                    return;
                }
            }

            // Adicionar novo parâmetro
            controller.AddParameter(parameterName, parameterType);
            UnityEngine.Debug.Log($"🎯 Parâmetro '{parameterName}' ({parameterType}) adicionado");
        }

        /// <summary>
        /// Configura as transições entre estados do Animator.
        /// </summary>
        /// <param name="controller">Animator Controller</param>
        /// <param name="includeDialogue">Se true, adiciona transições para estado Talk</param>
        public static void ConfigureTransitions(AnimatorController controller, bool includeDialogue)
        {
            if (controller == null)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Controller não pode ser nulo!");
                return;
            }

            // Obter o Base Layer
            var layers = controller.layers;
            if (layers == null || layers.Length == 0)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Controller não possui layers!");
                return;
            }

            var stateMachine = layers[0].stateMachine;

            // Buscar estados
            var idleState = FindStateByName(stateMachine, "Idle");
            var walkState = FindStateByName(stateMachine, "Walk");
            var deathState = FindStateByName(stateMachine, "Death");
            var talkState = includeDialogue ? FindStateByName(stateMachine, "Talk") : null;

            // Idle ↔ Walk (Speed > 0.1 / Speed < 0.1)
            AddTransitionIfNotExists(idleState, walkState, "Speed", AnimatorConditionMode.Greater, 0.1f, false, 0.1f);
            AddTransitionIfNotExists(walkState, idleState, "Speed", AnimatorConditionMode.Less, 0.1f, false, 0.1f);

            // Idle ↔ Talk (IsTalking = true / false) - se diálogo habilitado
            if (includeDialogue && talkState != null)
            {
                AddTransitionIfNotExists(idleState, talkState, "IsTalking", AnimatorConditionMode.If, 0, false, 0.1f);
                AddTransitionIfNotExists(talkState, idleState, "IsTalking", AnimatorConditionMode.IfNot, 0, false, 0.1f);
            }

            // Any State → Death (IsDead = true)
            if (deathState != null)
            {
                var anyStateTransition = stateMachine.AddAnyStateTransition(deathState);
                anyStateTransition.AddCondition(AnimatorConditionMode.If, 0, "IsDead");
                anyStateTransition.hasExitTime = false;
                anyStateTransition.duration = 0.1f;
                UnityEngine.Debug.Log("✅ Transição 'Any State → Death' criada");
            }

            EditorUtility.SetDirty(controller);
        }

        /// <summary>
        /// Encontra um estado pelo nome no state machine.
        /// </summary>
        private static AnimatorState FindStateByName(AnimatorStateMachine stateMachine, string stateName)
        {
            foreach (var childState in stateMachine.states)
            {
                if (childState.state != null && childState.state.name == stateName)
                {
                    return childState.state;
                }
            }
            return null;
        }

        /// <summary>
        /// Adiciona uma transição entre estados se ela não existir.
        /// </summary>
        /// <param name="fromState">Estado de origem</param>
        /// <param name="toState">Estado de destino</param>
        /// <param name="parameterName">Nome do parâmetro da condição</param>
        /// <param name="conditionMode">Modo da condição</param>
        /// <param name="threshold">Valor threshold para a condição</param>
        /// <param name="hasExitTime">Se a transição tem exit time</param>
        /// <param name="duration">Duração da transição</param>
        public static void AddTransitionIfNotExists(
            AnimatorState fromState,
            AnimatorState toState,
            string parameterName,
            AnimatorConditionMode conditionMode,
            float threshold,
            bool hasExitTime,
            float duration)
        {
            if (fromState == null || toState == null)
            {
                UnityEngine.Debug.LogWarning("⚠️ NPCAnimatorSetup: Estado de origem ou destino é nulo!");
                return;
            }

            // Verificar se a transição já existe
            foreach (var transition in fromState.transitions)
            {
                if (transition.destinationState == toState)
                {
                    // Verificar se tem a mesma condição
                    foreach (var condition in transition.conditions)
                    {
                        if (condition.parameter == parameterName && condition.mode == conditionMode)
                        {
                            UnityEngine.Debug.Log($"✅ Transição '{fromState.name} → {toState.name}' já existe");
                            return;
                        }
                    }
                }
            }

            // Criar nova transição
            var newTransition = fromState.AddTransition(toState);
            newTransition.AddCondition(conditionMode, threshold, parameterName);
            newTransition.hasExitTime = hasExitTime;
            newTransition.exitTime = hasExitTime ? 1f : 0f;
            newTransition.duration = duration;

            UnityEngine.Debug.Log($"🔀 Transição '{fromState.name} → {toState.name}' criada");
        }

        /// <summary>
        /// Configura animações placeholder para os estados do Animator.
        /// Procura por animações em Assets/Art/Animations/Placeholders/ ou cria clips vazios.
        /// </summary>
        /// <param name="controller">Animator Controller</param>
        /// <param name="npcName">Nome do NPC para salvar clips criados</param>
        /// <param name="includeDialogue">Se true, processa estado Talk</param>
        public static void ConfigurePlaceholderAnimations(AnimatorController controller, string npcName, bool includeDialogue)
        {
            if (controller == null)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Controller não pode ser nulo!");
                return;
            }

            // Obter o Base Layer
            var layers = controller.layers;
            if (layers == null || layers.Length == 0)
            {
                UnityEngine.Debug.LogError("❌ NPCAnimatorSetup: Controller não possui layers!");
                return;
            }

            var stateMachine = layers[0].stateMachine;

            // Configurar animações para cada estado
            ConfigureStateAnimation(stateMachine, "Idle", npcName);
            ConfigureStateAnimation(stateMachine, "Walk", npcName);
            ConfigureStateAnimation(stateMachine, "Death", npcName);

            if (includeDialogue)
            {
                ConfigureStateAnimation(stateMachine, "Talk", npcName);
            }

            EditorUtility.SetDirty(controller);
        }

        /// <summary>
        /// Configura a animação para um estado específico.
        /// </summary>
        private static void ConfigureStateAnimation(AnimatorStateMachine stateMachine, string stateName, string npcName)
        {
            var state = FindStateByName(stateMachine, stateName);
            if (state == null)
            {
                UnityEngine.Debug.LogWarning($"⚠️ Estado '{stateName}' não encontrado");
                return;
            }

            // Se o estado já tem uma animação, não sobrescrever
            if (state.motion != null)
            {
                UnityEngine.Debug.Log($"✅ Estado '{stateName}' já possui animação: {state.motion.name}");
                return;
            }

            // Procurar por animação placeholder
            AnimationClip placeholderClip = FindPlaceholderAnimation(stateName);

            if (placeholderClip != null)
            {
                // Usar animação placeholder encontrada
                state.motion = placeholderClip;
                UnityEngine.Debug.Log($"🎬 Animação placeholder '{placeholderClip.name}' aplicada ao estado '{stateName}'");
            }
            else
            {
                // Criar clip vazio
                AnimationClip emptyClip = CreateEmptyAnimationClip(npcName, stateName);
                if (emptyClip != null)
                {
                    state.motion = emptyClip;
                    UnityEngine.Debug.Log($"📝 Clip vazio criado e aplicado ao estado '{stateName}'");
                }
            }
        }

        /// <summary>
        /// Procura por uma animação placeholder no diretório de placeholders.
        /// </summary>
        private static AnimationClip FindPlaceholderAnimation(string stateName)
        {
            // Procurar por arquivos de animação no diretório de placeholders
            string[] guids = AssetDatabase.FindAssets($"{stateName} t:AnimationClip", new[] { PLACEHOLDER_ANIMATIONS_PATH });

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                return clip;
            }

            return null;
        }

        /// <summary>
        /// Cria um clip de animação vazio e salva no diretório do NPC.
        /// </summary>
        private static AnimationClip CreateEmptyAnimationClip(string npcName, string stateName)
        {
            // Garantir que o diretório existe
            string npcAnimationPath = $"{ANIMATOR_BASE_PATH}{npcName}/";
            if (!Directory.Exists(npcAnimationPath))
            {
                Directory.CreateDirectory(npcAnimationPath);
                AssetDatabase.Refresh();
            }

            // Criar clip vazio
            AnimationClip clip = new AnimationClip();
            clip.name = $"{npcName}_{stateName}";

            // Salvar o clip
            string clipPath = $"{npcAnimationPath}{clip.name}.anim";
            AssetDatabase.CreateAsset(clip, clipPath);
            AssetDatabase.SaveAssets();

            UnityEngine.Debug.Log($"📁 Clip vazio criado em: {clipPath}");
            return clip;
        }
    }
}
