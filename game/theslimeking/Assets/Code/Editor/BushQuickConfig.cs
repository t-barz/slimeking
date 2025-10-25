using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using SlimeMec.Gameplay;

namespace SlimeKing.Editor
{
    /// <summary>
    /// Utilitário para configuração rápida de moitas (bushes) baseado no prefab bushA2.
    /// Adiciona todos os componentes necessários e configura automaticamente.
    /// </summary>
    public static class BushQuickConfig
    {
        // Paths dos assets necessários
        private const string BUSH_PREFAB_PATH = "Assets/External/AssetStore/SlimeMec/_Prefabs/Environment/bushA2.prefab";
        private const string BUSH_CONTROLLER_PATH = "Assets/External/AssetStore/SlimeMec/_Animation/BushA2/bushA2.controller";
        private const string BUSH_MATERIAL_PATH = "Assets/External/AssetStore/SlimeMec/_Art/Materials/sprite_lit_default.mat";

        [MenuItem("GameObject/Quick Config/🌿 Configure as Bush", false, 0)]
        public static void ConfigureAsBush(MenuCommand menuCommand)
        {
            // Obtém o GameObject selecionado ou cria um novo
            GameObject targetObject = menuCommand.context as GameObject;

            if (targetObject == null)
            {
                Debug.LogError("⚠️ BushQuickConfig: Nenhum GameObject selecionado!");
                return;
            }

            // Registra para Undo
            Undo.RegisterCompleteObjectUndo(targetObject, "Configure as Bush");

            try
            {
                ConfigureBushComponents(targetObject);
                Debug.Log($"✅ Bush configurada com sucesso: {targetObject.name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Erro ao configurar bush: {e.Message}");
            }
        }

        /// <summary>
        /// Valida se o menu deve aparecer (só quando há GameObject selecionado)
        /// </summary>
        [MenuItem("GameObject/Quick Config/🌿 Configure as Bush", true)]
        public static bool ValidateConfigureAsBush()
        {
            return Selection.activeGameObject != null;
        }

        public static void ConfigureBushComponents(GameObject targetObject)
        {
            // 1. Configurar Tag e Layer
            targetObject.tag = "Destructable";
            targetObject.layer = 0; // Default layer

            // 2. Configurar Transform (escala similar ao prefab)
            targetObject.transform.localScale = new Vector3(0.9109736f, 0.94240296f, 1f);

            // 3. Configurar SpriteRenderer
            ConfigureSpriteRenderer(targetObject);

            // 4. Configurar Animator
            ConfigureAnimator(targetObject);

            // 5. Configurar Collider2D
            ConfigureCollider(targetObject);

            // 6. Adicionar scripts de gameplay
            ConfigureGameplayScripts(targetObject);

            // 7. Configurar RandomStyle
            ConfigureRandomStyle(targetObject);

            // Marcar objeto como modificado
            EditorUtility.SetDirty(targetObject);
        }

        private static void ConfigureSpriteRenderer(GameObject targetObject)
        {
            SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = targetObject.AddComponent<SpriteRenderer>();
            }

            // Configurações baseadas no prefab
            spriteRenderer.flipX = true;
            spriteRenderer.flipY = false;
            spriteRenderer.sortingOrder = 0;
            spriteRenderer.sortingLayerID = 0;
            spriteRenderer.color = Color.white;

            // Tentar carregar o material
            Material bushMaterial = AssetDatabase.LoadAssetAtPath<Material>(BUSH_MATERIAL_PATH);
            if (bushMaterial != null)
            {
                spriteRenderer.material = bushMaterial;
            }
            else
            {
                Debug.LogWarning($"⚠️ Material não encontrado em: {BUSH_MATERIAL_PATH}. Usando material padrão.");
            }

            // Se não há sprite, tentar carregar sprite padrão do bush
            if (spriteRenderer.sprite == null)
            {
                LoadBushSprite(spriteRenderer);
            }
        }

        private static void LoadBushSprite(SpriteRenderer spriteRenderer)
        {
            // Buscar sprites de bush disponíveis
            string[] bushSpriteGuids = AssetDatabase.FindAssets("bush t:Sprite", new[] { "Assets/External/AssetStore/SlimeMec/_Art" });

            if (bushSpriteGuids.Length > 0)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(bushSpriteGuids[0]);
                Sprite bushSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

                if (bushSprite != null)
                {
                    spriteRenderer.sprite = bushSprite;
                    Debug.Log($"🎨 Sprite carregado: {bushSprite.name}");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Nenhum sprite de bush encontrado. Defina o sprite manualmente.");
            }
        }

        private static void ConfigureAnimator(GameObject targetObject)
        {
            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
            {
                animator = targetObject.AddComponent<Animator>();
            }

            // Configurar estados básicos do Animator
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

            // Se não há controller, tentar carregar o padrão do bush como fallback
            if (animator.runtimeAnimatorController == null)
            {
                RuntimeAnimatorController bushController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(BUSH_CONTROLLER_PATH);
                if (bushController != null)
                {
                    animator.runtimeAnimatorController = bushController;
                    Debug.Log($"🎬 Animator Controller padrão configurado: {bushController.name}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Animator Controller não encontrado em: {BUSH_CONTROLLER_PATH}");
                }
            }
            else
            {
                Debug.Log($"🎬 Animator mantendo controller existente: {animator.runtimeAnimatorController.name}");
            }

            // Configurar triggers necessárias para bush (baseado no bushA2.controller)
            ConfigureAnimatorTriggers(animator);
            // Configurar transições entre estados existentes
            ConfigureAnimatorTransitions(animator);
        }

        private static void ConfigureAnimatorTriggers(Animator animator)
        {
            if (animator.runtimeAnimatorController == null)
            {
                Debug.LogWarning("⚠️ Não é possível configurar triggers sem um AnimatorController!");
                return;
            }

            var controller = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
            if (controller == null)
            {
                Debug.LogWarning("⚠️ Controller não é editável. Triggers não podem ser adicionadas.");
                return;
            }

            // Lista de triggers necessárias para bush (baseado no bushA2.controller)
            string[] requiredTriggers = { "Shake", "Destroy" };

            foreach (string triggerName in requiredTriggers)
            {
                // Verifica se a trigger já existe
                bool triggerExists = false;
                foreach (var parameter in controller.parameters)
                {
                    if (parameter.name == triggerName && parameter.type == AnimatorControllerParameterType.Trigger)
                    {
                        triggerExists = true;
                        break;
                    }
                }

                // Adiciona a trigger se não existir
                if (!triggerExists)
                {
                    controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);
                    Debug.Log($"🎯 Trigger '{triggerName}' adicionada ao Animator");
                }
                else
                {
                    Debug.Log($"✅ Trigger '{triggerName}' já existe no Animator");
                }
            }

            // Marca o controller como modificado
            EditorUtility.SetDirty(controller);
        }

        private static AnimatorState FindStateByName(AnimatorStateMachine stateMachine, string stateName)
        {
            foreach (var child in stateMachine.states)
            {
                if (child.state != null && child.state.name == stateName)
                    return child.state;
            }
            return null;
        }

        private static bool TransitionExists(AnimatorState from, AnimatorState to, string trigger)
        {
            foreach (var t in from.transitions)
            {
                if (t.destinationState == to)
                {
                    if (string.IsNullOrEmpty(trigger))
                        return true;
                    foreach (var c in t.conditions)
                    {
                        if (c.parameter == trigger)
                            return true;
                    }
                }
            }
            return false;
        }

        private static void ConfigureAnimatorTransitions(Animator animator)
        {
            if (animator.runtimeAnimatorController == null)
                return;

            var controller = animator.runtimeAnimatorController as AnimatorController;
            if (controller == null)
                return;

            // Busca o Base Layer
            var layers = controller.layers;
            if (layers == null || layers.Length == 0)
                return;
            var stateMachine = layers[0].stateMachine;

            // Busca estados existentes
            var idleState = FindStateByName(stateMachine, "Idle");
            var shakeState = FindStateByName(stateMachine, "Shake");
            var destroyState = FindStateByName(stateMachine, "Destroy");

            // Helper para criar transição se ambos os estados existem e não existe ainda
            void CreateTransition(AnimatorState from, AnimatorState to, string trigger, bool hasExitTime = false)
            {
                if (from == null || to == null) return;
                if (TransitionExists(from, to, trigger)) return;
                var transition = from.AddTransition(to);
                transition.hasExitTime = hasExitTime;
                transition.exitTime = hasExitTime ? 1f : 0f;
                transition.duration = 0.1f;
                if (!string.IsNullOrEmpty(trigger))
                {
                    transition.AddCondition(AnimatorConditionMode.If, 0, trigger);
                }
            }

            // Idle → Shake (trigger: Shake)
            CreateTransition(idleState, shakeState, "Shake");
            // Shake → Idle (após Shake terminar)
            CreateTransition(shakeState, idleState, null, true);
            // Idle → Destroy (trigger: Destroy)
            CreateTransition(idleState, destroyState, "Destroy");
            // Shake → Destroy (trigger: Destroy)
            CreateTransition(shakeState, destroyState, "Destroy");
            // Destroy → Idle (opcional, se existir)
            CreateTransition(destroyState, idleState, null, true);

            EditorUtility.SetDirty(controller);
        }

        private static void ConfigureCollider(GameObject targetObject)
        {
            // Remover colliders existentes para evitar conflitos
            Collider2D[] existingColliders = targetObject.GetComponents<Collider2D>();
            foreach (var collider in existingColliders)
            {
                Undo.DestroyObjectImmediate(collider);
            }

            // Adicionar CircleCollider2D como trigger
            CircleCollider2D circleCollider = targetObject.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.offset = new Vector2(0f, 0.15f);
            circleCollider.radius = 0.15f;

            Debug.Log("🔘 CircleCollider2D configurado como trigger");
        }

        private static void ConfigureGameplayScripts(GameObject targetObject)
        {
            // 1. WindEmulator - movimento por vento
            if (targetObject.GetComponent<WindEmulator>() == null)
            {
                var windEmulator = targetObject.AddComponent<WindEmulator>();
                // Configurações baseadas no prefab
                SetPrivateField(windEmulator, "shakeTriggerName", "Shake");
                SetPrivateField(windEmulator, "shakeIntervalRange", new Vector2(2f, 5f));
                SetPrivateField(windEmulator, "autoStart", true);
                SetPrivateField(windEmulator, "enableLogs", false);
                Debug.Log("💨 WindEmulator adicionado");
            }

            // 2. BushDestruct - sistema de destruição
            if (targetObject.GetComponent<BushDestruct>() == null)
            {
                var bushDestruct = targetObject.AddComponent<BushDestruct>();
                SetPrivateField(bushDestruct, "enableDebugLogs", false);
                Debug.Log("💥 BushDestruct adicionado");
            }

            // 3. BushShake - movimento quando jogador passa
            if (targetObject.GetComponent<BushShake>() == null)
            {
                targetObject.AddComponent<BushShake>();
                Debug.Log("🌿 BushShake adicionado");
            }

            // 4. DropController - sistema de drop
            if (targetObject.GetComponent<DropController>() == null)
            {
                var dropController = targetObject.AddComponent<DropController>();
                SetPrivateField(dropController, "minDropCount", 1);
                SetPrivateField(dropController, "maxDropCount", 3);
                SetPrivateField(dropController, "useCurrentPosition", true);
                SetPrivateField(dropController, "enableDebugLogs", false);
                Debug.Log("📦 DropController adicionado");
            }
        }

        private static void ConfigureRandomStyle(GameObject targetObject)
        {
            var randomStyle = targetObject.GetComponent<RandomStyle>();
            if (randomStyle == null)
            {
                randomStyle = targetObject.AddComponent<RandomStyle>();
            }

            // Configurações baseadas no prefab - usando nomes corretos dos campos
            SetPrivateField(randomStyle, "randomizeScale", true);
            SetPrivateField(randomStyle, "minScale", 0.95f);
            SetPrivateField(randomStyle, "maxScale", 1.05f);
            SetPrivateField(randomStyle, "uniformScale", true);
            SetPrivateField(randomStyle, "randomizeColor", false);
            SetPrivateField(randomStyle, "applyOnStart", true);
            SetPrivateField(randomStyle, "randomizeAlpha", false);
            SetPrivateField(randomStyle, "enableLogs", false);

            Debug.Log("🎲 RandomStyle configurado");
        }

        /// <summary>
        /// Helper para definir campos privados via reflection
        /// </summary>
        private static void SetPrivateField(Object target, string fieldName, object value)
        {
            try
            {
                var field = target.GetType().GetField(fieldName,
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

                if (field != null)
                {
                    field.SetValue(target, value);
                }
                else
                {
                    Debug.LogWarning($"⚠️ Campo '{fieldName}' não encontrado em {target.GetType().Name}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Erro ao configurar campo '{fieldName}': {e.Message}");
            }
        }

        #region Menu de Configuração Avançada

        [MenuItem("GameObject/Quick Config/🌿 Bush Advanced Setup", false, 1)]
        public static void ShowBushSetupWizard()
        {
            BushSetupWizard.ShowWindow();
        }

        [MenuItem("GameObject/Quick Config/🌿 Bush Advanced Setup", true)]
        public static bool ValidateBushSetupWizard()
        {
            return Selection.activeGameObject != null;
        }

        #endregion

        #region Utilitários de Debug

        [MenuItem("GameObject/Quick Config/🔍 Show Bush Info", false, 10)]
        public static void ShowBushInfo()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                Debug.LogWarning("⚠️ Nenhum GameObject selecionado!");
                return;
            }

            Debug.Log($"=== BUSH INFO: {selectedObject.name} ===");
            Debug.Log($"Tag: {selectedObject.tag}");
            Debug.Log($"Layer: {LayerMask.LayerToName(selectedObject.layer)}");
            Debug.Log($"Scale: {selectedObject.transform.localScale}");

            var components = selectedObject.GetComponents<Component>();
            Debug.Log($"Componentes ({components.Length}):");
            foreach (var component in components)
            {
                Debug.Log($"  • {component.GetType().Name}");
            }
            Debug.Log("================================");
        }

        [MenuItem("GameObject/Quick Config/🔍 Show Bush Info", true)]
        public static bool ValidateShowBushInfo()
        {
            return Selection.activeGameObject != null;
        }

        #endregion

        /// <summary>
        /// Configura apenas as triggers e as transições entre estados Idle, Shake, Destroy no Animator do GameObject.
        /// Não altera controller, não cria estados novos.
        /// </summary>
        [MenuItem("GameObject/Quick Config/🌿 Configure Bush Animator States/Triggers", false, 1)]
        public static void ConfigureBushAnimatorStatesAndTriggers()
        {
            GameObject targetObject = Selection.activeGameObject;
            if (targetObject == null)
            {
                Debug.LogError("⚠️ Nenhum GameObject selecionado!");
                return;
            }
            Animator animator = targetObject.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("⚠️ O objeto selecionado não possui Animator!");
                return;
            }
            ConfigureAnimatorTriggers(animator);
            ConfigureAnimatorTransitions(animator);
            Debug.Log($"✅ Triggers e transições do Animator configuradas para '{targetObject.name}'");
        }
    }

    /// <summary>
    /// Janela de configuração avançada para bushes
    /// </summary>
    public class BushSetupWizard : EditorWindow
    {
        private GameObject targetObject;
        private bool includeDropSystem = true;
        private bool includeWindEffect = true;
        private bool includeRandomization = true;
        private bool includePlayerShake = true;

        public static void ShowWindow()
        {
            var window = GetWindow<BushSetupWizard>("Bush Setup Wizard");
            window.minSize = new Vector2(300, 200);
            window.targetObject = Selection.activeGameObject;
        }

        void OnGUI()
        {
            GUILayout.Label("🌿 Bush Setup Wizard", EditorStyles.boldLabel);
            GUILayout.Space(10);

            targetObject = EditorGUILayout.ObjectField("Target GameObject", targetObject, typeof(GameObject), true) as GameObject;

            if (targetObject == null)
            {
                EditorGUILayout.HelpBox("Selecione um GameObject para configurar.", MessageType.Warning);
                return;
            }

            GUILayout.Space(10);
            GUILayout.Label("Opções de Configuração:", EditorStyles.boldLabel);

            includeDropSystem = EditorGUILayout.Toggle("📦 Sistema de Drop", includeDropSystem);
            includeWindEffect = EditorGUILayout.Toggle("💨 Efeito de Vento", includeWindEffect);
            includeRandomization = EditorGUILayout.Toggle("🎲 Randomização Visual", includeRandomization);
            includePlayerShake = EditorGUILayout.Toggle("🌿 Shake ao Passar", includePlayerShake);

            GUILayout.Space(20);

            if (GUILayout.Button("✨ Configurar Bush", GUILayout.Height(30)))
            {
                ConfigureCustomBush();
                Close();
            }
        }

        private void ConfigureCustomBush()
        {
            Undo.RegisterCompleteObjectUndo(targetObject, "Custom Bush Setup");

            // Sempre adiciona componentes básicos
            BushQuickConfig.ConfigureBushComponents(targetObject);

            // Remove componentes opcionais se não desejados
            if (!includeDropSystem)
            {
                var dropController = targetObject.GetComponent<DropController>();
                if (dropController != null) DestroyImmediate(dropController);
            }

            if (!includeWindEffect)
            {
                var windEmulator = targetObject.GetComponent<WindEmulator>();
                if (windEmulator != null) DestroyImmediate(windEmulator);
            }

            if (!includeRandomization)
            {
                var randomStyle = targetObject.GetComponent<RandomStyle>();
                if (randomStyle != null) DestroyImmediate(randomStyle);
            }

            if (!includePlayerShake)
            {
                var bushShake = targetObject.GetComponent<BushShake>();
                if (bushShake != null) DestroyImmediate(bushShake);
            }

            EditorUtility.SetDirty(targetObject);
            Debug.Log($"✅ Bush customizada configurada: {targetObject.name}");
        }
    }
}