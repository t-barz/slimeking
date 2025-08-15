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
    /// Menu de contexto para configurar apenas os parâmetros e triggers do Animator Controller.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/1. Setup Animator Parameters Only", false, 1)]
    static void SetupAnimatorParametersOnly()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (!ValidateSelectedObject(selectedObject)) return;

        bool proceed = EditorUtility.DisplayDialog(
            "Configurar Parâmetros do Animator",
            $"Esta operação irá configurar apenas os parâmetros (Bool e Trigger) do Animator Controller do objeto '{selectedObject.name}'.\n\n" +
            "• Remove parâmetros existentes\n" +
            "• Adiciona todos os parâmetros necessários\n" +
            "• Define valores padrão\n\n" +
            "Deseja continuar?",
            "Sim", "Cancelar"
        );

        if (!proceed) return;

        try
        {
            AnimatorController controller = GetOrCreateAnimatorController(selectedObject);
            if (controller != null)
            {
                SetupAnimatorParameters(controller);
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                Debug.Log($"Parâmetros do Animator configurados com sucesso para '{selectedObject.name}'!");
                EditorUtility.DisplayDialog("Sucesso", "Parâmetros configurados com sucesso!", "OK");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao configurar parâmetros: {e.Message}");
            EditorUtility.DisplayDialog("Erro", $"Erro durante a configuração:\n{e.Message}", "OK");
        }
    }

    /// <summary>
    /// Menu de contexto para configurar apenas as transições entre estados existentes.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/2. Setup Existing State Transitions", false, 2)]
    static void SetupExistingStateTransitionsOnly()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (!ValidateSelectedObject(selectedObject)) return;

        bool proceed = EditorUtility.DisplayDialog(
            "Configurar Transições dos Estados Existentes",
            $"Esta operação irá criar transições apenas entre os estados que já existem no Animator Controller do objeto '{selectedObject.name}'.\n\n" +
            "• Não cria novos estados\n" +
            "• Conecta estados existentes com transições\n" +
            "• Configura condições e triggers\n\n" +
            "Deseja continuar?",
            "Sim", "Cancelar"
        );

        if (!proceed) return;

        try
        {
            AnimatorController controller = GetOrCreateAnimatorController(selectedObject);
            if (controller != null)
            {
                SetupExistingStateTransitions(controller);
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                Debug.Log($"Transições entre estados existentes configuradas com sucesso para '{selectedObject.name}'!");
                EditorUtility.DisplayDialog("Sucesso", "Transições configuradas com sucesso!", "OK");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao configurar transições: {e.Message}");
            EditorUtility.DisplayDialog("Erro", $"Erro durante a configuração:\n{e.Message}", "OK");
        }
    }

    /// <summary>
    /// Menu de contexto para criar apenas os estados ausentes no Animator Controller.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/3. Create Missing States", false, 3)]
    static void CreateMissingStatesOnly()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (!ValidateSelectedObject(selectedObject)) return;

        bool proceed = EditorUtility.DisplayDialog(
            "Criar Estados Ausentes",
            $"Esta operação irá criar apenas os estados que não existem no Animator Controller do objeto '{selectedObject.name}'.\n\n" +
            "• Analisa estados existentes\n" +
            "• Cria apenas os estados ausentes\n" +
            "• Posiciona estados adequadamente\n\n" +
            "Deseja continuar?",
            "Sim", "Cancelar"
        );

        if (!proceed) return;

        try
        {
            AnimatorController controller = GetOrCreateAnimatorController(selectedObject);
            if (controller != null)
            {
                CreateMissingStates(controller);
                EditorUtility.SetDirty(controller);
                AssetDatabase.SaveAssets();
                Debug.Log($"Estados ausentes criados com sucesso para '{selectedObject.name}'!");
                EditorUtility.DisplayDialog("Sucesso", "Estados ausentes criados com sucesso!", "OK");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao criar estados: {e.Message}");
            EditorUtility.DisplayDialog("Erro", $"Erro durante a configuração:\n{e.Message}", "OK");
        }
    }

    /// <summary>
    /// Valida se o objeto selecionado é válido para operações do Animator.
    /// </summary>
    static bool ValidateSelectedObject(GameObject selectedObject)
    {
        if (selectedObject == null)
        {
            EditorUtility.DisplayDialog("Erro", "Selecione um GameObject com Animator para configurar.", "OK");
            return false;
        }

        Animator animator = selectedObject.GetComponent<Animator>();
        if (animator == null)
        {
            EditorUtility.DisplayDialog("Erro", "O GameObject selecionado não possui um componente Animator.", "OK");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Menu de contexto para configurar o Animator Controller completo do objeto selecionado.
    /// Executa todas as três operações em sequência.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/Player Animator Controller Setup (Complete)", false, 10)]
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
                // 1. Configura os parâmetros e triggers
                SetupAnimatorParameters(controller);

                // 2. Faz relacionamentos entre estados existentes
                SetupExistingStateTransitions(controller);

                // 3. Cria estados que não existem
                CreateMissingStates(controller);

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
    /// Faz relacionamentos (transições) entre estados já existentes no Animator Controller.
    /// Não cria novos estados, apenas conecta os que já estão presentes.
    /// </summary>
    static void SetupExistingStateTransitions(AnimatorController controller)
    {
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

        // Cria um dicionário para facilitar o acesso aos estados existentes
        Dictionary<string, AnimatorState> existingStates = new Dictionary<string, AnimatorState>();
        foreach (var state in stateMachine.states)
        {
            existingStates[state.state.name] = state.state;
        }

        Debug.Log($"Estados existentes encontrados: {string.Join(", ", existingStates.Keys)}");

        // Hub Central: IDLE - Transições de saída (apenas se ambos os estados existirem)
        if (existingStates.ContainsKey("Idle"))
        {
            var idle = existingStates["Idle"];

            // Idle → Walk
            if (existingStates.ContainsKey("Walk"))
                CreateTransition(idle, existingStates["Walk"], "isWalking", true, 0.1f);

            // Idle → Sleep
            if (existingStates.ContainsKey("Sleep"))
                CreateTransition(idle, existingStates["Sleep"], "isSleeping", true, 0.3f);

            // Idle → Hide
            if (existingStates.ContainsKey("Hide"))
                CreateTransition(idle, existingStates["Hide"], "isHiding", true, 0.2f);

            // Idle → Attack01 (Trigger)
            if (existingStates.ContainsKey("Attack01"))
                CreateTriggerTransition(idle, existingStates["Attack01"], "Attack01", 0.0f);

            // Idle → Shrink (Trigger)
            if (existingStates.ContainsKey("Shrink"))
                CreateTriggerTransition(idle, existingStates["Shrink"], "Shrink", 0.0f);

            // Idle → Jump (Trigger)
            if (existingStates.ContainsKey("Jump"))
                CreateTriggerTransition(idle, existingStates["Jump"], "Jump", 0.0f);
        }

        // Walk - Transições (apenas se ambos os estados existirem)
        if (existingStates.ContainsKey("Walk"))
        {
            var walk = existingStates["Walk"];

            // Walk → Idle
            if (existingStates.ContainsKey("Idle"))
                CreateTransition(walk, existingStates["Idle"], "isWalking", false, 0.1f);

            // Walk → Attack01 (Trigger)
            if (existingStates.ContainsKey("Attack01"))
                CreateTriggerTransition(walk, existingStates["Attack01"], "Attack01", 0.0f);

            // Walk → Shrink (Trigger)
            if (existingStates.ContainsKey("Shrink"))
                CreateTriggerTransition(walk, existingStates["Shrink"], "Shrink", 0.0f);

            // Walk → Jump (Trigger)
            if (existingStates.ContainsKey("Jump"))
                CreateTriggerTransition(walk, existingStates["Jump"], "Jump", 0.0f);

            // Walk → Hit
            if (existingStates.ContainsKey("Hit"))
                CreateTransition(walk, existingStates["Hit"], "isHurt", true, 0.0f);
        }

        // Estados com retorno automático para Idle (apenas se ambos existirem)
        string[] autoReturnStates = { "Attack01", "Shrink", "Hit", "Jump", "Eating" };
        float[] exitTimes = { 0.8f, 0.8f, 0.5f, 1.0f, 1.2f };

        for (int i = 0; i < autoReturnStates.Length; i++)
        {
            if (existingStates.ContainsKey(autoReturnStates[i]) && existingStates.ContainsKey("Idle"))
            {
                CreateExitTimeTransition(existingStates[autoReturnStates[i]], existingStates["Idle"], exitTimes[i]);
            }
        }

        // Sleep/Waking transições (apenas se ambos existirem)
        if (existingStates.ContainsKey("Sleep") && existingStates.ContainsKey("Waking"))
        {
            CreateTransition(existingStates["Sleep"], existingStates["Waking"], "isSleeping", false, 0.1f);
        }

        if (existingStates.ContainsKey("Waking") && existingStates.ContainsKey("Idle"))
        {
            CreateExitTimeTransition(existingStates["Waking"], existingStates["Idle"], 1.5f);
        }

        // Hide transições (apenas se ambos existirem)
        if (existingStates.ContainsKey("Hide"))
        {
            var hide = existingStates["Hide"];

            // Hide → Idle
            if (existingStates.ContainsKey("Idle"))
                CreateTransition(hide, existingStates["Idle"], "isHiding", false, 0.1f);

            // Hide → Walk
            if (existingStates.ContainsKey("Walk"))
                CreateTransition(hide, existingStates["Walk"], "isWalking", true, 0.1f);
        }

        // Start transição automática (apenas se ambos existirem)
        if (existingStates.ContainsKey("Start") && existingStates.ContainsKey("Idle"))
        {
            CreateExitTimeTransition(existingStates["Start"], existingStates["Idle"], 0.1f);
        }

        // Estados emocionais com retorno automático (apenas se ambos existirem)
        string[] emotionalStates = { "Happy", "Sad", "Focus" };
        float[] emotionalExitTimes = { 3.0f, 2.0f, 1.0f };

        for (int i = 0; i < emotionalStates.Length; i++)
        {
            if (existingStates.ContainsKey(emotionalStates[i]) && existingStates.ContainsKey("Idle"))
            {
                CreateExitTimeTransition(existingStates[emotionalStates[i]], existingStates["Idle"], emotionalExitTimes[i]);
            }
        }

        // Define o estado padrão se Idle existir
        if (existingStates.ContainsKey("Idle"))
        {
            stateMachine.defaultState = existingStates["Idle"];
        }

        Debug.Log("Transições entre estados existentes configuradas com sucesso!");
    }

    /// <summary>
    /// Cria os estados que não existem no Animator Controller baseado no mapeamento padrão.
    /// </summary>
    static void CreateMissingStates(AnimatorController controller)
    {
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;

        // Obtém lista de estados existentes
        HashSet<string> existingStateNames = new HashSet<string>();
        foreach (var state in stateMachine.states)
        {
            existingStateNames.Add(state.state.name);
        }

        // Mapeamento de estados necessários com suas posições
        Dictionary<string, Vector3> requiredStates = new Dictionary<string, Vector3>
        {
            // Estados Principais (Core Gameplay)
            { "Idle", new Vector3(20, 440, 0) },
            { "Walk", new Vector3(20, 540, 0) },
            { "Sleep", new Vector3(-250, 440, 0) },
            { "Hide", new Vector3(-200, 330, 0) },

            // Estados de Ação
            { "Attack01", new Vector3(280, 450, 0) },
            { "Shrink", new Vector3(-250, 540, 0) },
            { "Hit", new Vector3(700, 190, 0) },
            { "Dying", new Vector3(70, 320, 0) },

            // Estados de Movimento Especial
            { "Jump", new Vector3(660, 290, 0) },

            // Estados Emocionais/Expressivos
            { "Happy", new Vector3(235, 65, 0) },
            { "Sad", new Vector3(490, 70, 0) },
            { "Eating", new Vector3(550, 130, 0) },
            { "Focus", new Vector3(585, 715, 0) },

            // Estados de Sistema
            { "Start", new Vector3(-110, 260, 0) },
            { "Waking", new Vector3(-250, 340, 0) }
        };

        // Cria apenas os estados que não existem
        Dictionary<string, AnimatorState> newlyCreatedStates = new Dictionary<string, AnimatorState>();
        foreach (var kvp in requiredStates)
        {
            string stateName = kvp.Key;
            Vector3 position = kvp.Value;

            if (!existingStateNames.Contains(stateName))
            {
                AnimatorState newState = CreateAnimatorState(stateMachine, stateName, position);
                newlyCreatedStates[stateName] = newState;
                Debug.Log($"Estado criado: {stateName}");
            }
        }

        // Configura propriedades específicas dos estados recém-criados
        ConfigureStateProperties(newlyCreatedStates);

        // Define o estado padrão como Idle se ele foi criado agora ou já existe
        var allStates = new Dictionary<string, AnimatorState>();
        foreach (var state in stateMachine.states)
        {
            allStates[state.state.name] = state.state;
        }

        if (allStates.ContainsKey("Idle"))
        {
            stateMachine.defaultState = allStates["Idle"];
        }

        Debug.Log($"Estados ausentes criados com sucesso! Novos estados: {string.Join(", ", newlyCreatedStates.Keys)}");
    }

    /// <summary>
    /// Configura todos os estados do Animator conforme documentação.
    /// NOTA: Esta função foi dividida em SetupExistingStateTransitions e CreateMissingStates
    /// </summary>
    [System.Obsolete("Use SetupExistingStateTransitions() e CreateMissingStates() ao invés desta função")]
    static void SetupAnimatorStates(AnimatorController controller)
    {
        // Função obsoleta - dividida em SetupExistingStateTransitions() e CreateMissingStates()
        Debug.LogWarning("SetupAnimatorStates() está obsoleta. Use CreateMissingStates() e SetupExistingStateTransitions().");
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
    /// NOTA: Esta função foi integrada em SetupExistingStateTransitions()
    /// </summary>
    [System.Obsolete("Use SetupExistingStateTransitions() ao invés desta função")]
    static void SetupAnimatorTransitions(AnimatorController controller)
    {
        // Função obsoleta - use SetupExistingStateTransitions()
        Debug.LogWarning("SetupAnimatorTransitions() está obsoleta. Use SetupExistingStateTransitions().");
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
    [MenuItem("GameObject/Migration Tools/1. Setup Animator Parameters Only", true)]
    static bool ValidateSetupAnimatorParametersOnly()
    {
        return Selection.activeGameObject != null;
    }

    /// <summary>
    /// Valida se o menu de contexto deve estar disponível.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/2. Setup Existing State Transitions", true)]
    static bool ValidateSetupExistingStateTransitionsOnly()
    {
        return Selection.activeGameObject != null;
    }

    /// <summary>
    /// Valida se o menu de contexto deve estar disponível.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/3. Create Missing States", true)]
    static bool ValidateCreateMissingStatesOnly()
    {
        return Selection.activeGameObject != null;
    }

    /// <summary>
    /// Valida se o menu de contexto deve estar disponível.
    /// </summary>
    [MenuItem("GameObject/Migration Tools/Player Animator Controller Setup (Complete)", true)]
    static bool ValidateSetupPlayerAnimatorController()
    {
        return Selection.activeGameObject != null;
    }
}
