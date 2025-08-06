using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

/// <summary>
/// Editor script para automatizar a configuração de prefabs como inimigos
/// </summary>
public static class EnemyPrefabSetup
{
    #region Menu de Contexto
    /// <summary>
    /// Menu de contexto para preparar prefab como inimigo
    /// </summary>
    [MenuItem("Assets/Setup as Enemy/Basic Enemy", false, 30)]
    private static void SetupAsBasicEnemy()
    {
        SetupSelectedPrefabAsEnemy(EnemyType.Basic);
    }

    [MenuItem("Assets/Setup as Enemy/Elite Enemy", false, 31)]
    private static void SetupAsEliteEnemy()
    {
        SetupSelectedPrefabAsEnemy(EnemyType.Elite);
    }

    [MenuItem("Assets/Setup as Enemy/Boss Enemy", false, 32)]
    private static void SetupAsBossEnemy()
    {
        SetupSelectedPrefabAsEnemy(EnemyType.Boss);
    }

    [MenuItem("Assets/Setup as Enemy/Flying Enemy", false, 33)]
    private static void SetupAsFlyingEnemy()
    {
        SetupSelectedPrefabAsEnemy(EnemyType.Flying);
    }

    // Validação do menu - só aparece quando um prefab está selecionado
    [MenuItem("Assets/Setup as Enemy/Basic Enemy", true)]
    [MenuItem("Assets/Setup as Enemy/Elite Enemy", true)]
    [MenuItem("Assets/Setup as Enemy/Boss Enemy", true)]
    [MenuItem("Assets/Setup as Enemy/Flying Enemy", true)]
    private static bool ValidateSetupAsEnemy()
    {
        return Selection.activeObject != null &&
               AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".prefab");
    }
    #endregion

    #region Tipos de Inimigo
    /// <summary>
    /// Tipos de inimigo disponíveis
    /// </summary>
    private enum EnemyType
    {
        Basic,
        Elite,
        Boss,
        Flying
    }
    #endregion

    #region Configuração Principal
    /// <summary>
    /// Configura o prefab selecionado como inimigo
    /// </summary>
    private static void SetupSelectedPrefabAsEnemy(EnemyType enemyType)
    {
        // Verifica se há um objeto selecionado
        if (Selection.activeObject == null)
        {
            EditorUtility.DisplayDialog("Erro", "Nenhum prefab selecionado!", "OK");
            return;
        }

        // Obtém o caminho do asset
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (!assetPath.EndsWith(".prefab"))
        {
            EditorUtility.DisplayDialog("Erro", "O objeto selecionado não é um prefab!", "OK");
            return;
        }

        // Carrega o prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("Erro", "Não foi possível carregar o prefab!", "OK");
            return;
        }

        // Pergunta se quer continuar
        string enemyTypeName = GetEnemyTypeName(enemyType);
        if (!EditorUtility.DisplayDialog("Configurar como Inimigo",
            $"Configurar '{prefab.name}' como {enemyTypeName}?\n\n" +
            "Esta ação irá:\n" +
            "• Renomear o prefab\n" +
            "• Adicionar CircleCollider2D\n" +
            "• Adicionar EntityStatus\n" +
            "• Adicionar EnemyBehaviorController\n" +
            "• Adicionar Rigidbody2D\n" +
            "• Configurar tags e layers\n" +
            "• Configurar parâmetros do Animator",
            "Sim", "Cancelar"))
        {
            return;
        }

        try
        {
            // Inicia o registro de undo
            Undo.RecordObject(prefab, "Setup Enemy Prefab");

            // Configura o prefab
            ConfigurePrefabAsEnemy(prefab, enemyType, assetPath);

            // Salva as mudanças
            EditorUtility.SetDirty(prefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Mostra mensagem de sucesso
            EditorUtility.DisplayDialog("Sucesso!",
                $"Prefab '{prefab.name}' configurado como {enemyTypeName} com sucesso!\n\n" +
                $"PRÓXIMOS PASSOS:\n" +
                $"• Configure as distâncias (detectionRadius, attackRange, etc.) no Inspector\n" +
                $"• Ajuste os parâmetros de patrulha se necessário\n" +
                $"• Configure a hierarquia de sprites (side, back, front, vfx_*)\n" +
                $"• Teste o comportamento na cena", "OK");

            // Seleciona o prefab renomeado
            Selection.activeObject = prefab;
            EditorGUIUtility.PingObject(prefab);
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Erro",
                $"Erro ao configurar prefab:\n{ex.Message}", "OK");
        }
    }

    /// <summary>
    /// Configura o prefab como inimigo
    /// </summary>
    private static void ConfigurePrefabAsEnemy(GameObject prefab, EnemyType enemyType, string originalPath)
    {
        // 1. Renomeia o prefab
        string newName = RenameBasedOnType(prefab, enemyType, originalPath);

        // 2. Configura tags e layers
        SetupTagsAndLayers(prefab);

        // 3. Adiciona componentes essenciais
        AddEssentialComponents(prefab, enemyType);

        // 4. Configura componentes específicos
        ConfigureComponentsByType(prefab, enemyType);

        Debug.Log($"[EnemySetup] Prefab '{newName}' configurado como {GetEnemyTypeName(enemyType)}");
    }
    #endregion

    #region Renomeação
    /// <summary>
    /// Renomeia o prefab baseado no tipo de inimigo
    /// </summary>
    private static string RenameBasedOnType(GameObject prefab, EnemyType enemyType, string originalPath)
    {
        string currentName = prefab.name;
        string prefix = GetEnemyPrefix(enemyType);
        string newName;

        // Remove prefixos antigos se existirem
        string cleanName = RemoveExistingPrefixes(currentName);

        // Aplica novo nome baseado no padrão da indústria
        newName = $"{prefix}_{cleanName}";

        // Renomeia o GameObject
        prefab.name = newName;

        // Renomeia o arquivo do prefab
        string directory = Path.GetDirectoryName(originalPath);
        string extension = Path.GetExtension(originalPath);
        string newPath = Path.Combine(directory, newName + extension);

        if (originalPath != newPath)
        {
            AssetDatabase.RenameAsset(originalPath, newName);
        }

        return newName;
    }

    /// <summary>
    /// Obtém o prefixo baseado no tipo de inimigo (padrão da indústria)
    /// </summary>
    private static string GetEnemyPrefix(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Basic => "Enemy",      // Enemy_NomePrefab
            EnemyType.Elite => "Elite",      // Elite_NomePrefab
            EnemyType.Boss => "Boss",        // Boss_NomePrefab
            EnemyType.Flying => "Flying",    // Flying_NomePrefab
            _ => "Enemy"
        };
    }

    /// <summary>
    /// Remove prefixos existentes do nome
    /// </summary>
    private static string RemoveExistingPrefixes(string name)
    {
        string[] prefixes = {
            "Enemy_", "Elite_", "Boss_", "Flying_",     // Novos prefixos
            "ENY_", "ELT_", "BSS_", "FLY_"             // Prefixos antigos (para compatibilidade)
        };

        foreach (string prefix in prefixes)
        {
            if (name.StartsWith(prefix))
            {
                return name.Substring(prefix.Length);
            }
        }

        return name;
    }

    /// <summary>
    /// Obtém o nome legível do tipo de inimigo
    /// </summary>
    private static string GetEnemyTypeName(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Basic => "Inimigo Básico",
            EnemyType.Elite => "Inimigo Elite",
            EnemyType.Boss => "Boss",
            EnemyType.Flying => "Inimigo Voador",
            _ => "Inimigo"
        };
    }
    #endregion

    #region Tags e Layers
    /// <summary>
    /// Configura tags e layers apropriados
    /// </summary>
    private static void SetupTagsAndLayers(GameObject prefab)
    {
        // Define tag como Enemy
        if (!prefab.CompareTag("Enemy"))
        {
            prefab.tag = "Enemy";
        }

        // Configura layer (assumindo que existe um layer "Enemy")
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer != -1)
        {
            prefab.layer = enemyLayer;
        }
        else
        {
            Debug.LogWarning("[EnemySetup] Layer 'Enemy' não encontrado. Configure manualmente.");
        }
    }
    #endregion

    #region Componentes
    /// <summary>
    /// Adiciona componentes essenciais
    /// </summary>
    private static void AddEssentialComponents(GameObject prefab, EnemyType enemyType)
    {
        // 1. Rigidbody2D (para movimento físico)
        if (prefab.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = prefab.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = enemyType == EnemyType.Flying ? 0f : 1f;
            rb.freezeRotation = true; // Evita rotação indesejada
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // 2. CircleCollider2D (para detecção de colisão)
        if (prefab.GetComponent<CircleCollider2D>() == null)
        {
            CircleCollider2D collider = prefab.AddComponent<CircleCollider2D>();
            collider.radius = GetDefaultColliderRadius(enemyType);
            collider.isTrigger = false; // Colisão física
        }

        // 3. EntityStatus (sistema de stats)
        if (prefab.GetComponent<EntityStatus>() == null)
        {
            EntityStatus entityStatus = prefab.AddComponent<EntityStatus>();
            ConfigureEntityStatus(entityStatus, enemyType);
        }

        // 4. EnemyBehaviorController (comportamento IA do inimigo)
        if (prefab.GetComponent<EnemyBehaviorController>() == null)
        {
            EnemyBehaviorController behaviorController = prefab.AddComponent<EnemyBehaviorController>();
            ConfigureEnemyBehavior(behaviorController, enemyType);
        }

        // 5. Configurar Animator (se existir)
        SetupAnimatorParameters(prefab);
    }

    /// <summary>
    /// Configura parâmetros do Animator automaticamente
    /// </summary>
    private static void SetupAnimatorParameters(GameObject prefab)
    {
        Animator animator = prefab.GetComponent<Animator>();
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"[EnemySetup] {prefab.name} não possui Animator ou AnimatorController. " +
                           "Configure manualmente os parâmetros: isWalking, isAttacking, takeDamage, dieDestroy");
            return;
        }

        // Obtém o AnimatorController
        UnityEditor.Animations.AnimatorController controller =
            animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

        if (controller == null)
        {
            Debug.LogWarning($"[EnemySetup] Não foi possível acessar o AnimatorController de {prefab.name}");
            return;
        }

        bool needsUpdate = false;

        // Lista de parâmetros necessários
        var requiredBools = new[] { "isWalking", "isAttacking" };
        var requiredTriggers = new[] { "takeDamage", "dieDestroy" };

        // Adiciona parâmetros Bool se não existirem
        foreach (string paramName in requiredBools)
        {
            if (!HasParameter(controller, paramName))
            {
                controller.AddParameter(paramName, AnimatorControllerParameterType.Bool);
                needsUpdate = true;
                Debug.Log($"[EnemySetup] Adicionado parâmetro Bool: {paramName}");
            }
        }

        // Adiciona parâmetros Trigger se não existirem
        foreach (string paramName in requiredTriggers)
        {
            if (!HasParameter(controller, paramName))
            {
                controller.AddParameter(paramName, AnimatorControllerParameterType.Trigger);
                needsUpdate = true;
                Debug.Log($"[EnemySetup] Adicionado parâmetro Trigger: {paramName}");
            }
        }

        if (needsUpdate)
        {
            // Marca o controller como modificado
            EditorUtility.SetDirty(controller);
            Debug.Log($"[EnemySetup] Animator de {prefab.name} configurado com sucesso!");
        }
        else
        {
            Debug.Log($"[EnemySetup] Animator de {prefab.name} já possui todos os parâmetros necessários.");
        }
    }

    /// <summary>
    /// Verifica se um parâmetro existe no AnimatorController
    /// </summary>
    private static bool HasParameter(UnityEditor.Animations.AnimatorController controller, string parameterName)
    {
        foreach (var parameter in controller.parameters)
        {
            if (parameter.name == parameterName)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Adiciona collider de detecção separado
    /// </summary>
    private static void AddDetectionTrigger(GameObject prefab, EnemyType enemyType)
    {
        // Cria um filho para o trigger de detecção
        Transform detectionTrigger = prefab.transform.Find("DetectionRange");

        if (detectionTrigger == null)
        {
            GameObject detectionObj = new GameObject("DetectionRange");
            detectionObj.transform.SetParent(prefab.transform);
            detectionObj.transform.localPosition = Vector3.zero;

            CircleCollider2D triggerCollider = detectionObj.AddComponent<CircleCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = GetDefaultDetectionRadius(enemyType);

            // Layer para triggers
            int triggerLayer = LayerMask.NameToLayer("EnemyTrigger");
            if (triggerLayer != -1)
            {
                detectionObj.layer = triggerLayer;
            }
        }
    }

    /// <summary>
    /// Configura componentes específicos por tipo
    /// </summary>
    private static void ConfigureComponentsByType(GameObject prefab, EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Flying:
                ConfigureFlyingEnemy(prefab);
                break;
            case EnemyType.Boss:
                ConfigureBossEnemy(prefab);
                break;
            case EnemyType.Elite:
                ConfigureEliteEnemy(prefab);
                break;
        }
    }

    /// <summary>
    /// Configuração específica para inimigos voadores
    /// </summary>
    private static void ConfigureFlyingEnemy(GameObject prefab)
    {
        Rigidbody2D rb = prefab.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f; // Remove gravidade
        }
    }

    /// <summary>
    /// Configuração específica para bosses
    /// </summary>
    private static void ConfigureBossEnemy(GameObject prefab)
    {
        // Bosses são maiores e mais resistentes
        CircleCollider2D collider = prefab.GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            collider.radius *= 1.5f; // 50% maior
        }
    }

    /// <summary>
    /// Configuração específica para inimigos elite
    /// </summary>
    private static void ConfigureEliteEnemy(GameObject prefab)
    {
        // Elite enemies têm detecção melhorada mas sem collider adicional
        // Configurações específicas podem ser adicionadas aqui futuramente
        Debug.Log($"[EnemySetup] {prefab.name} configurado como Elite Enemy");
    }
    #endregion

    #region Configurações Padrão
    /// <summary>
    /// Configura EntityStatus baseado no tipo
    /// </summary>
    private static void ConfigureEntityStatus(EntityStatus entityStatus, EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Basic:
                entityStatus.baseHP = 50;
                entityStatus.baseAttack = 10;
                entityStatus.baseDefense = 5;
                entityStatus.baseSpeed = 2;
                break;
            case EnemyType.Elite:
                entityStatus.baseHP = 100;
                entityStatus.baseAttack = 20;
                entityStatus.baseDefense = 10;
                entityStatus.baseSpeed = 3;
                break;
            case EnemyType.Boss:
                entityStatus.baseHP = 300;
                entityStatus.baseAttack = 40;
                entityStatus.baseDefense = 20;
                entityStatus.baseSpeed = 1;
                break;
            case EnemyType.Flying:
                entityStatus.baseHP = 30;
                entityStatus.baseAttack = 15;
                entityStatus.baseDefense = 3;
                entityStatus.baseSpeed = 4;
                break;
        }

        entityStatus.baseLevel = 1;
        entityStatus.currentHP = entityStatus.baseHP;
        entityStatus.currentLevel = entityStatus.baseLevel;
    }

    /// <summary>
    /// Configura EnemyBehaviorController baseado no tipo
    /// </summary>
    private static void ConfigureEnemyBehavior(EnemyBehaviorController behaviorController, EnemyType enemyType)
    {
        // Como os campos são privados, vamos configurar apenas o tipo de comportamento
        // O usuário pode ajustar as outras configurações no Inspector do Unity

        switch (enemyType)
        {
            case EnemyType.Basic:
                behaviorController.SetBehaviorType(EnemyBehaviorController.BehaviorType.Defensive);
                break;

            case EnemyType.Elite:
                behaviorController.SetBehaviorType(EnemyBehaviorController.BehaviorType.Aggressive);
                break;

            case EnemyType.Boss:
                behaviorController.SetBehaviorType(EnemyBehaviorController.BehaviorType.Guardian);
                break;

            case EnemyType.Flying:
                behaviorController.SetBehaviorType(EnemyBehaviorController.BehaviorType.Aggressive);
                break;
        }

        Debug.Log($"[EnemySetup] EnemyBehaviorController adicionado e configurado para tipo: {enemyType}");
        Debug.Log($"[EnemySetup] Configure manualmente as distâncias e parâmetros de patrulha no Inspector.");
    }

    /// <summary>
    /// Obtém raio padrão do collider por tipo
    /// </summary>
    private static float GetDefaultColliderRadius(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Basic => 0.5f,
            EnemyType.Elite => 0.6f,
            EnemyType.Boss => 1.0f,
            EnemyType.Flying => 0.4f,
            _ => 0.5f
        };
    }

    /// <summary>
    /// Obtém raio padrão de detecção por tipo
    /// </summary>
    private static float GetDefaultDetectionRadius(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Basic => 3.0f,
            EnemyType.Elite => 4.0f,
            EnemyType.Boss => 6.0f,
            EnemyType.Flying => 5.0f,
            _ => 3.0f
        };
    }
    #endregion
}
