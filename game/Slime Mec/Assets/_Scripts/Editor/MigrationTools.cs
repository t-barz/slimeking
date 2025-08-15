using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

/// <summary>
/// Ferramentas de migração e configuração automática para o projeto SlimeKing.
/// Inclui configuração automática do Animator Controller baseado na documentação.
/// </summary>
public class MigrationTools : EditorWindow
{
    /// <summary>
    /// Menu de contexto para configurar o Animator Controller do objeto selecionado.
    /// Configura todos os parâmetros e estados conforme documentação do AnimatorController-Documentation.md
    /// </summary>
    [MenuItem("GameObject/Migration Tools/Player Animator Controller Setup", false, 0)]
    static void SetupPlayerAnimatorController()
    {
        // Verifica se há um GameObject selecionado
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("Erro", "Selecione um GameObject com Animator para configurar.", "OK");
            return;
        }

        // Verifica se o objeto tem um componente Animator
        Animator animator = selectedObject.GetComponent<Animator>();
        if (animator == null)
        {
            EditorUtility.DisplayDialog("Erro", "O GameObject selecionado não possui um componente Animator.", "OK");
            return;
        }

        // Pergunta confirmação antes de proceder
        bool proceed = EditorUtility.DisplayDialog(
            "Configurar Player Animator Controller",
            $"Esta operação irá configurar automaticamente o Animator Controller do objeto '{selectedObject.name}' com:\n\n" +
            "• Todos os parâmetros necessários (Bool e Trigger)\n" +
            "• Estados básicos (Idle, Walk, Sleep, Attack, etc.)\n" +
            "• Transições configuradas\n\n" +
            "Deseja continuar?",
            "Sim", "Cancelar"
        );

        if (!proceed) return;

        try
        {
            // Cria ou obtém o AnimatorController
            AnimatorController controller = GetOrCreateAnimatorController(selectedObject);

            if (controller != null)
            {
                // Configura os parâmetros
                SetupAnimatorParameters(controller);

                // Configura os estados
                SetupAnimatorStates(controller);

                // Configura as transições
                SetupAnimatorTransitions(controller);

                // Salva as mudanças
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();

                Debug.Log($"Animator Controller configurado com sucesso para '{selectedObject.name}'!");
                EditorUtility.DisplayDialog("Sucesso",
                    $"Animator Controller configurado com sucesso!\n\n" +
                    $"Controller: {AssetDatabase.GetAssetPath(controller)}", "OK");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao configurar Animator Controller: {e.Message}");
            EditorUtility.DisplayDialog("Erro", $"Erro durante a configuração:\n{e.Message}", "OK");
        }
    }

    /// <summary>
    /// Obtém o AnimatorController existente ou cria um novo se necessário.
    /// </summary>
    static AnimatorController GetOrCreateAnimatorController(GameObject target)
    {
        Animator animator = target.GetComponent<Animator>();
        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

        // Se não existe controller, cria um novo
        if (controller == null)
        {
            string controllerName = $"anm_{target.name.ToLower()}.controller";
            string path = $"Assets/_Animation/Controllers/{controllerName}";

            // Cria o diretório se não existir
            string directory = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Cria o AnimatorController
            controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            animator.runtimeAnimatorController = controller;

            Debug.Log($"Novo AnimatorController criado: {path}");
        }

        return controller;
    }

    /// <summary>
    /// Configura todos os parâmetros do Animator conforme documentação.
    /// </summary>
    static void SetupAnimatorParameters(AnimatorController controller)
    {
        // Remove parâmetros existentes para evitar duplicatas
        var parameters = new List<AnimatorControllerParameter>(controller.parameters);
        foreach (var param in parameters)
        {
            controller.RemoveParameter(param);
        }

        // Bool Parameters (Estado Contínuo)
        controller.AddParameter("isSleeping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isHiding", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("FacingRight", AnimatorControllerParameterType.Bool);

        // Trigger Parameters (Ação Pontual)
        controller.AddParameter("Shrink", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Attack01", AnimatorControllerParameterType.Trigger);

        // Parâmetros Expandidos (conforme sugestões da documentação)
        controller.AddParameter("isHurt", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isDead", AnimatorControllerParameterType.Bool);
        controller.AddParameter("SpecialAttack", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Heal", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Talk", AnimatorControllerParameterType.Trigger);
        controller.AddParameter("Jump", AnimatorControllerParameterType.Trigger);

        // Define valores padrão
        SetParameterDefaultValue(controller, "FacingRight", true);
        SetParameterDefaultValue(controller, "isSleeping", false);
        SetParameterDefaultValue(controller, "isHiding", false);
        SetParameterDefaultValue(controller, "isWalking", false);
        SetParameterDefaultValue(controller, "isHurt", false);
        SetParameterDefaultValue(controller, "isDead", false);

        Debug.Log("Parâmetros do Animator configurados com sucesso!");
    }

    /// <summary>
    /// Define o valor padrão de um parâmetro bool.
    /// </summary>
    static void SetParameterDefaultValue(AnimatorController controller, string paramName, bool defaultValue)
    {
        var parameter = controller.parameters[System.Array.FindIndex(controller.parameters, p => p.name == paramName)];
        if (parameter != null && parameter.type == AnimatorControllerParameterType.Bool)
        {
            parameter.defaultBool = defaultValue;
        }
    }

    /// <summary>
    /// Configura todos os estados do Animator conforme documentação.
    /// </summary>
    static void SetupAnimatorStates(AnimatorController controller)
    {
        // Obtém a layer principal (Base Layer)
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

        // Remove estados existentes exceto Entry, Exit e Any State
        var states = new List<ChildAnimatorState>(stateMachine.states);
        foreach (var state in states)
        {
            stateMachine.RemoveState(state.state);
        }

        // Dicionário para armazenar os estados criados
        Dictionary<string, AnimatorState> createdStates = new Dictionary<string, AnimatorState>();

        // Estados Principais (Core Gameplay)
        createdStates["Idle"] = CreateAnimatorState(stateMachine, "Idle", new Vector3(20, 440, 0));
        createdStates["Walk"] = CreateAnimatorState(stateMachine, "Walk", new Vector3(20, 540, 0));
        createdStates["Sleep"] = CreateAnimatorState(stateMachine, "Sleep", new Vector3(-250, 440, 0));
        createdStates["Hide"] = CreateAnimatorState(stateMachine, "Hide", new Vector3(-200, 330, 0));

        // Estados de Ação
        createdStates["Attack01"] = CreateAnimatorState(stateMachine, "Attack01", new Vector3(280, 450, 0));
        createdStates["Shrink"] = CreateAnimatorState(stateMachine, "Shrink", new Vector3(-250, 540, 0));
        createdStates["Hit"] = CreateAnimatorState(stateMachine, "Hit", new Vector3(700, 190, 0));
        createdStates["Dying"] = CreateAnimatorState(stateMachine, "Dying", new Vector3(70, 320, 0));

        // Estados de Movimento Especial
        createdStates["Jump"] = CreateAnimatorState(stateMachine, "Jump", new Vector3(660, 290, 0));

        // Estados Emocionais/Expressivos
        createdStates["Happy"] = CreateAnimatorState(stateMachine, "Happy", new Vector3(235, 65, 0));
        createdStates["Sad"] = CreateAnimatorState(stateMachine, "Sad", new Vector3(490, 70, 0));
        createdStates["Eating"] = CreateAnimatorState(stateMachine, "Eating", new Vector3(550, 130, 0));
        createdStates["Focus"] = CreateAnimatorState(stateMachine, "Focus", new Vector3(585, 715, 0));

        // Estados de Sistema
        createdStates["Start"] = CreateAnimatorState(stateMachine, "Start", new Vector3(-110, 260, 0));
        createdStates["Waking"] = CreateAnimatorState(stateMachine, "Waking", new Vector3(-250, 340, 0));

        // Define o estado padrão como Idle
        stateMachine.defaultState = createdStates["Idle"];

        // Configura propriedades específicas dos estados
        ConfigureStateProperties(createdStates);

        Debug.Log("Estados do Animator configurados com sucesso!");
    }

    /// <summary>
    /// Cria um novo estado no Animator com nome e posição específicos.
    /// </summary>
    static AnimatorState CreateAnimatorState(AnimatorStateMachine stateMachine, string stateName, Vector3 position)
    {
        AnimatorState newState = stateMachine.AddState(stateName, position);
        return newState;
    }

    /// <summary>
    /// Configura propriedades específicas dos estados (loop, speed, etc.).
    /// </summary>
    static void ConfigureStateProperties(Dictionary<string, AnimatorState> states)
    {
        // Estados com loop contínuo
        string[] loopingStates = { "Idle", "Walk", "Sleep", "Hide", "Happy", "Sad", "Focus" };
        foreach (string stateName in loopingStates)
        {
            if (states.ContainsKey(stateName))
            {
                // Estes estados devem ter animações em loop
                // A configuração específica da animação será feita quando as animações forem atribuídas
            }
        }

        // Estados com duração específica (não loop)
        string[] singleShotStates = { "Attack01", "Shrink", "Hit", "Jump", "Dying", "Eating", "Start", "Waking" };
        foreach (string stateName in singleShotStates)
        {
            if (states.ContainsKey(stateName))
            {
                // Estes estados devem ser "one shot" (não loop)
            }
        }
    }

    /// <summary>
    /// Configura todas as transições entre estados conforme documentação.
    /// </summary>
    static void SetupAnimatorTransitions(AnimatorController controller)
    {
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

        // Cria um dicionário para facilitar o acesso aos estados
        Dictionary<string, AnimatorState> stateDict = new Dictionary<string, AnimatorState>();
        foreach (var state in stateMachine.states)
        {
            stateDict[state.state.name] = state.state;
        }

        // Hub Central: IDLE - Transições de saída
        if (stateDict.ContainsKey("Idle"))
        {
            var idle = stateDict["Idle"];

            // Idle → Walk
            if (stateDict.ContainsKey("Walk"))
                CreateTransition(idle, stateDict["Walk"], "isWalking", true, 0.1f);

            // Idle → Sleep
            if (stateDict.ContainsKey("Sleep"))
                CreateTransition(idle, stateDict["Sleep"], "isSleeping", true, 0.3f);

            // Idle → Hide
            if (stateDict.ContainsKey("Hide"))
                CreateTransition(idle, stateDict["Hide"], "isHiding", true, 0.2f);

            // Idle → Attack01 (Trigger)
            if (stateDict.ContainsKey("Attack01"))
                CreateTriggerTransition(idle, stateDict["Attack01"], "Attack01", 0.0f);

            // Idle → Shrink (Trigger)
            if (stateDict.ContainsKey("Shrink"))
                CreateTriggerTransition(idle, stateDict["Shrink"], "Shrink", 0.0f);

            // Idle → Jump (Trigger)
            if (stateDict.ContainsKey("Jump"))
                CreateTriggerTransition(idle, stateDict["Jump"], "Jump", 0.0f);
        }

        // Walk - Transições
        if (stateDict.ContainsKey("Walk"))
        {
            var walk = stateDict["Walk"];

            // Walk → Idle
            if (stateDict.ContainsKey("Idle"))
                CreateTransition(walk, stateDict["Idle"], "isWalking", false, 0.1f);

            // Walk → Attack01 (Trigger)
            if (stateDict.ContainsKey("Attack01"))
                CreateTriggerTransition(walk, stateDict["Attack01"], "Attack01", 0.0f);

            // Walk → Shrink (Trigger)
            if (stateDict.ContainsKey("Shrink"))
                CreateTriggerTransition(walk, stateDict["Shrink"], "Shrink", 0.0f);

            // Walk → Jump (Trigger)
            if (stateDict.ContainsKey("Jump"))
                CreateTriggerTransition(walk, stateDict["Jump"], "Jump", 0.0f);

            // Walk → Hit
            if (stateDict.ContainsKey("Hit"))
                CreateTransition(walk, stateDict["Hit"], "isHurt", true, 0.0f);
        }

        // Estados com retorno automático para Idle
        string[] autoReturnStates = { "Attack01", "Shrink", "Hit", "Jump", "Eating" };
        float[] exitTimes = { 0.8f, 0.8f, 0.5f, 1.0f, 1.2f };

        for (int i = 0; i < autoReturnStates.Length; i++)
        {
            if (stateDict.ContainsKey(autoReturnStates[i]) && stateDict.ContainsKey("Idle"))
            {
                CreateExitTimeTransition(stateDict[autoReturnStates[i]], stateDict["Idle"], exitTimes[i]);
            }
        }

        // Sleep/Waking transições
        if (stateDict.ContainsKey("Sleep") && stateDict.ContainsKey("Waking"))
        {
            CreateTransition(stateDict["Sleep"], stateDict["Waking"], "isSleeping", false, 0.1f);
        }

        if (stateDict.ContainsKey("Waking") && stateDict.ContainsKey("Idle"))
        {
            CreateExitTimeTransition(stateDict["Waking"], stateDict["Idle"], 1.5f);
        }

        // Hide transições
        if (stateDict.ContainsKey("Hide"))
        {
            var hide = stateDict["Hide"];

            // Hide → Idle
            if (stateDict.ContainsKey("Idle"))
                CreateTransition(hide, stateDict["Idle"], "isHiding", false, 0.1f);

            // Hide → Walk
            if (stateDict.ContainsKey("Walk"))
                CreateTransition(hide, stateDict["Walk"], "isWalking", true, 0.1f);
        }

        // Start transição automática
        if (stateDict.ContainsKey("Start") && stateDict.ContainsKey("Idle"))
        {
            CreateExitTimeTransition(stateDict["Start"], stateDict["Idle"], 0.1f);
        }

        // Estados emocionais com retorno automático
        string[] emotionalStates = { "Happy", "Sad", "Focus" };
        float[] emotionalExitTimes = { 3.0f, 2.0f, 1.0f };

        for (int i = 0; i < emotionalStates.Length; i++)
        {
            if (stateDict.ContainsKey(emotionalStates[i]) && stateDict.ContainsKey("Idle"))
            {
                CreateExitTimeTransition(stateDict[emotionalStates[i]], stateDict["Idle"], emotionalExitTimes[i]);
            }
        }

        // Dying - estado final (sem retorno)
        // Não criamos transições de saída para o estado Dying

        Debug.Log("Transições do Animator configuradas com sucesso!");
    }

    /// <summary>
    /// Cria uma transição baseada em condição boolean.
    /// </summary>
    static void CreateTransition(AnimatorState fromState, AnimatorState toState, string parameter, bool value, float transitionDuration)
    {
        AnimatorStateTransition transition = fromState.AddTransition(toState);
        transition.AddCondition(value ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0, parameter);
        transition.duration = transitionDuration;
        transition.hasExitTime = false;
    }

    /// <summary>
    /// Cria uma transição baseada em trigger.
    /// </summary>
    static void CreateTriggerTransition(AnimatorState fromState, AnimatorState toState, string triggerParameter, float transitionDuration)
    {
        AnimatorStateTransition transition = fromState.AddTransition(toState);
        transition.AddCondition(AnimatorConditionMode.If, 0, triggerParameter);
        transition.duration = transitionDuration;
        transition.hasExitTime = false;
    }

    /// <summary>
    /// Cria uma transição baseada em exit time (transição automática).
    /// </summary>
    static void CreateExitTimeTransition(AnimatorState fromState, AnimatorState toState, float exitTime)
    {
        AnimatorStateTransition transition = fromState.AddTransition(toState);
        transition.hasExitTime = true;
        transition.exitTime = exitTime;
        transition.duration = 0.1f;
    }

    /// <summary>
    /// Valida se o menu de contexto deve estar disponível.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/Player Animator Controller Setup", true)]
    static bool ValidateSetupPlayerAnimatorController()
    {
        return Selection.activeGameObject != null;
    }
}
