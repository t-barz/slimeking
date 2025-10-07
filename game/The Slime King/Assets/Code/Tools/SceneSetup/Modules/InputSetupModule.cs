using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace ExtraTools
{
    /// <summary>
    /// Módulo especializado para configuração avançada de Input System.
    /// Configura EventSystem, InputSystemUIInputModule e validação do Input System.
    /// </summary>
    public static class InputSetupModule
    {
        /// <summary>
        /// Executa o setup completo do input system baseado na configuração do SceneSetupManager
        /// </summary>
        /// <param name="setupManager">Referência ao SceneSetupManager com as configurações</param>
        public static void Setup(SceneSetupManager setupManager)
        {
            if (setupManager == null)
            {
                Debug.LogError("[InputSetup] SceneSetupManager é null");
                return;
            }

            Log(setupManager, "Iniciando setup avançado do input system...");

            // Configurar EventSystem se necessário
            if (setupManager.RequireEventSystem)
            {
                EventSystem eventSystem = EnsureEventSystem(setupManager);

                if (eventSystem != null)
                {
                    ConfigureEventSystem(eventSystem, setupManager);
                    ConfigureInputModule(eventSystem, setupManager);
                    ApplyInputConfigurationForSceneType(eventSystem, setupManager);
                }
            }

            // Validar Input Actions configurados
            ValidateInputActions(setupManager);

            Log(setupManager, "Setup do input system concluído com sucesso");
        }

        /// <summary>
        /// Garante que existe um EventSystem na cena
        /// </summary>
        private static EventSystem EnsureEventSystem(SceneSetupManager setupManager)
        {
            EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();

            if (eventSystem == null)
            {
                Log(setupManager, "EventSystem não encontrado. Criando novo...");

                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystem = eventSystemGO.AddComponent<EventSystem>();

                Log(setupManager, "EventSystem criado com sucesso");
            }
            else
            {
                Log(setupManager, "EventSystem encontrado - configurando...");
            }

            return eventSystem;
        }

        /// <summary>
        /// Configura as configurações básicas do EventSystem
        /// </summary>
        private static void ConfigureEventSystem(EventSystem eventSystem, SceneSetupManager setupManager)
        {
            // Configurações básicas do EventSystem
            eventSystem.sendNavigationEvents = true;
            eventSystem.pixelDragThreshold = 10;

            // Configurações específicas por tipo de cena
            switch (setupManager.SceneType)
            {
                case SceneType.Gameplay:
                    // Para gameplay, podemos ter navegação menos sensível
                    eventSystem.pixelDragThreshold = 15;
                    break;

                case SceneType.Menu:
                    // Para menus, navegação mais responsiva
                    eventSystem.pixelDragThreshold = 5;
                    eventSystem.sendNavigationEvents = true;
                    break;

                case SceneType.Cutscene:
                    // Para cutscenes, input limitado
                    eventSystem.sendNavigationEvents = false;
                    break;
            }

            Log(setupManager, $"EventSystem configurado para {setupManager.SceneType}");
        }

        /// <summary>
        /// Configura o Input Module apropriado
        /// </summary>
        private static void ConfigureInputModule(EventSystem eventSystem, SceneSetupManager setupManager)
        {
            // Verificar se já existe InputSystemUIInputModule
            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();

            if (inputModule == null)
            {
                // Remover qualquer StandaloneInputModule antigo (Legacy Input)
                StandaloneInputModule legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
                if (legacyModule != null)
                {
                    Object.DestroyImmediate(legacyModule);
                    Log(setupManager, "StandaloneInputModule removido (Legacy Input)");
                }

                // Adicionar InputSystemUIInputModule
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
                Log(setupManager, "InputSystemUIInputModule adicionado");
            }

            // Configurar Input Actions
            ConfigureInputActions(inputModule, setupManager);

            Log(setupManager, "Input Module configurado com sucesso");
        }

        /// <summary>
        /// Configura as Input Actions no InputSystemUIInputModule
        /// </summary>
        private static void ConfigureInputActions(InputSystemUIInputModule inputModule, SceneSetupManager setupManager)
        {
            // Tentar encontrar o arquivo de Input Actions do projeto
            var inputActions = TryFindProjectInputActions(setupManager);

            if (inputActions != null)
            {
                // Configurar as ações principais de UI
                if (inputActions.FindActionMap("UI") != null)
                {
                    var uiActionMap = inputActions.FindActionMap("UI");

                    // Configurar ações básicas de navegação
                    if (uiActionMap.FindAction("Navigate") != null)
                    {
                        inputModule.move = UnityEngine.InputSystem.InputActionReference.Create(uiActionMap.FindAction("Navigate"));
                        Log(setupManager, "Navigate action configurado");
                    }

                    if (uiActionMap.FindAction("Submit") != null)
                    {
                        inputModule.submit = UnityEngine.InputSystem.InputActionReference.Create(uiActionMap.FindAction("Submit"));
                        Log(setupManager, "Submit action configurado");
                    }

                    if (uiActionMap.FindAction("Cancel") != null)
                    {
                        inputModule.cancel = UnityEngine.InputSystem.InputActionReference.Create(uiActionMap.FindAction("Cancel"));
                        Log(setupManager, "Cancel action configurado");
                    }

                    if (uiActionMap.FindAction("Point") != null)
                    {
                        inputModule.point = UnityEngine.InputSystem.InputActionReference.Create(uiActionMap.FindAction("Point"));
                        Log(setupManager, "Point action configurado");
                    }

                    if (uiActionMap.FindAction("Click") != null)
                    {
                        inputModule.leftClick = UnityEngine.InputSystem.InputActionReference.Create(uiActionMap.FindAction("Click"));
                        Log(setupManager, "Click action configurado");
                    }

                    if (uiActionMap.FindAction("ScrollWheel") != null)
                    {
                        inputModule.scrollWheel = UnityEngine.InputSystem.InputActionReference.Create(uiActionMap.FindAction("ScrollWheel"));
                        Log(setupManager, "ScrollWheel action configurado");
                    }

                    Log(setupManager, "Input Actions de UI configurados automaticamente");
                }
                else
                {
                    LogWarning(setupManager, "Action Map 'UI' não encontrado no Input Actions. Configure manualmente no Inspector.");
                }
            }
            else
            {
                LogWarning(setupManager, "Input Actions não encontrado no projeto. Configure as ações manualmente no Inspector.");
            }
        }

        /// <summary>
        /// Tenta encontrar o arquivo de Input Actions do projeto
        /// </summary>
        private static UnityEngine.InputSystem.InputActionAsset TryFindProjectInputActions(SceneSetupManager setupManager)
        {
            // Primeiro, tentar encontrar o InputSystem_Actions padrão
            var inputActions = UnityEngine.Resources.Load<UnityEngine.InputSystem.InputActionAsset>("InputSystem_Actions");

            if (inputActions == null)
            {
                // Tentar encontrar qualquer arquivo .inputactions no projeto
#if UNITY_EDITOR
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:InputActionAsset");
                if (guids.Length > 0)
                {
                    string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    inputActions = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(assetPath);
                    
                    if (inputActions != null)
                    {
                        Log(setupManager, $"Input Actions encontrado: {inputActions.name}");
                    }
                }
#endif
            }
            else
            {
                Log(setupManager, "InputSystem_Actions padrão encontrado");
            }

            return inputActions;
        }

        /// <summary>
        /// Aplica configurações específicas por tipo de cena
        /// </summary>
        private static void ApplyInputConfigurationForSceneType(EventSystem eventSystem, SceneSetupManager setupManager)
        {
            var inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();

            if (inputModule == null) return;

            switch (setupManager.SceneType)
            {
                case SceneType.Gameplay:
                    // Para gameplay, configurações padrão
                    inputModule.moveRepeatDelay = 0.5f;
                    inputModule.moveRepeatRate = 0.1f;
                    Log(setupManager, "Configurações de input para gameplay aplicadas");
                    break;

                case SceneType.Menu:
                    // Para menus, navegação mais responsiva
                    inputModule.moveRepeatDelay = 0.3f;
                    inputModule.moveRepeatRate = 0.05f;
                    Log(setupManager, "Configurações de input para menu aplicadas");
                    break;

                case SceneType.Cutscene:
                    // Para cutscenes, input mais restrito
                    inputModule.moveRepeatDelay = 1f;
                    inputModule.moveRepeatRate = 0.2f;
                    Log(setupManager, "Configurações de input para cutscene aplicadas");
                    break;
            }
        }

        /// <summary>
        /// Valida se os Input Actions estão configurados corretamente
        /// </summary>
        private static void ValidateInputActions(SceneSetupManager setupManager)
        {
            var inputActions = TryFindProjectInputActions(setupManager);

            if (inputActions == null)
            {
                LogWarning(setupManager, "⚠️ Input Actions não encontrado no projeto");
                LogWarning(setupManager, "Recomendação: Criar Input Actions via Window → Analysis → Input Debugger → Create Actions");
                return;
            }

            // Verificar se as action maps essenciais existem
            bool hasUI = inputActions.FindActionMap("UI") != null;
            bool hasPlayer = inputActions.FindActionMap("Player") != null;

            if (!hasUI)
            {
                LogWarning(setupManager, "⚠️ Action Map 'UI' não encontrado no Input Actions");
            }

            if (!hasPlayer && setupManager.SceneType == SceneType.Gameplay)
            {
                LogWarning(setupManager, "⚠️ Action Map 'Player' não encontrado no Input Actions (recomendado para gameplay)");
            }

            if (hasUI && (hasPlayer || setupManager.SceneType != SceneType.Gameplay))
            {
                Log(setupManager, "✅ Input Actions validados com sucesso");
            }
        }

        /// <summary>
        /// Valida a configuração completa do input system
        /// </summary>
        public static bool ValidateInputSystem(SceneSetupManager setupManager)
        {
            bool isValid = true;

            // Validar EventSystem se requerido
            if (setupManager.RequireEventSystem)
            {
                EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
                if (eventSystem == null)
                {
                    LogWarning(setupManager, "EventSystem requerido mas não encontrado");
                    isValid = false;
                }
                else
                {
                    // Validar Input Module
                    InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
                    if (inputModule == null)
                    {
                        LogWarning(setupManager, "InputSystemUIInputModule não encontrado no EventSystem");
                        isValid = false;
                    }

                    // Verificar se há StandaloneInputModule conflitante
                    StandaloneInputModule legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
                    if (legacyModule != null)
                    {
                        LogWarning(setupManager, "StandaloneInputModule detectado - pode causar conflitos com Input System");
                        isValid = false;
                    }
                }
            }

            // Validar Input Actions
            var inputActions = TryFindProjectInputActions(setupManager);
            if (inputActions == null)
            {
                LogWarning(setupManager, "Input Actions não configurado no projeto");
                isValid = false;
            }

            if (isValid)
            {
                Log(setupManager, "✅ Configuração do input system validada com sucesso");
            }

            return isValid;
        }

        /// <summary>
        /// Remove configurações de input da cena
        /// </summary>
        public static void CleanupInputSystem(SceneSetupManager setupManager)
        {
            EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();

            if (eventSystem != null)
            {
                Object.DestroyImmediate(eventSystem.gameObject);
                Log(setupManager, "EventSystem removido");
            }
        }

        #region Helper Methods

        private static void Log(SceneSetupManager setupManager, string message)
        {
            if (setupManager.VerboseLogging)
            {
                Debug.Log($"[InputSetup] {message}");
            }
        }

        private static void LogWarning(SceneSetupManager setupManager, string message)
        {
            Debug.LogWarning($"[InputSetup] {message}");
        }

        private static void LogError(SceneSetupManager setupManager, string message)
        {
            Debug.LogError($"[InputSetup] {message}");
        }

        #endregion
    }
}