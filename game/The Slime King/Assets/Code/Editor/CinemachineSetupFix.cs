using UnityEngine;
using UnityEditor;

namespace ExtraTools
{
    /// <summary>
    /// Ferramenta para corrigir configuração da Cinemachine 2D Follow que resulta em "No cameras rendering".
    /// Resolve problemas comuns: ausência do componente Camera, CinemachineBrain mal posicionado, 
    /// configurações incorretas de Channel Mask e Target não configurado.
    /// </summary>
    public static class CinemachineSetupFix
    {
        #region Menu Items

        [MenuItem("Extra Tools/Camera Setup/Fix Cinemachine 2D Follow", false, 100)]
        public static void FixCinemachine2DFollow()
        {
            if (!Application.isPlaying)
            {
                Debug.Log("=== INICIANDO CORREÇÃO DA CINEMACHINE 2D FOLLOW ===");

                bool success = true;
                success &= EnsureMainCameraWithCameraComponent();
                success &= FixCinemachineBrainPosition();
                success &= ConfigureCinemachineCamera();
                success &= SetupPlayerTarget();
                success &= ValidateConfiguration();

                if (success)
                {
                    Debug.Log("✅ <color=green>Cinemachine 2D Follow configurada com sucesso!</color>");
                    Debug.Log("📋 <color=yellow>Agora teste a cena para verificar se a câmera está seguindo o player.</color>");
                }
                else
                {
                    Debug.LogError("❌ <color=red>Alguns problemas não puderam ser corrigidos. Verifique os logs acima.</color>");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Não é possível executar a correção durante o Play Mode. Pare a execução e tente novamente.");
            }
        }

        [MenuItem("Extra Tools/Camera Setup/Fix Cinemachine 2D Follow", true)]
        public static bool ValidateFixCinemachine2DFollow()
        {
            return !Application.isPlaying;
        }

        #endregion

        #region Main Camera Setup

        /// <summary>
        /// Garante que existe uma Main Camera com o componente Camera necessário.
        /// </summary>
        private static bool EnsureMainCameraWithCameraComponent()
        {
            Debug.Log("🔍 Verificando Main Camera...");

            // Busca por câmera com tag MainCamera
            Camera mainCamera = Camera.main;
            GameObject mainCameraGO = null;

            if (mainCamera != null)
            {
                mainCameraGO = mainCamera.gameObject;
                Debug.Log($"✅ Main Camera encontrada: {mainCameraGO.name}");
            }
            else
            {
                // Busca por GameObject com tag MainCamera
                GameObject taggedCamera = GameObject.FindWithTag("MainCamera");
                if (taggedCamera != null)
                {
                    mainCameraGO = taggedCamera;
                    Debug.Log($"📍 GameObject com tag MainCamera encontrado: {mainCameraGO.name}");

                    // Adiciona componente Camera se não existir
                    Camera cameraComponent = mainCameraGO.GetComponent<Camera>();
                    if (cameraComponent == null)
                    {
                        cameraComponent = mainCameraGO.AddComponent<Camera>();
                        Debug.Log("➕ Componente Camera adicionado à Main Camera");
                    }
                }
                else
                {
                    Debug.LogError("❌ Nenhuma Main Camera encontrada na cena. Crie um GameObject com tag 'MainCamera' primeiro.");
                    return false;
                }
            }

            // Configura propriedades básicas da câmera para 2D
            Camera cam = mainCameraGO.GetComponent<Camera>();
            if (cam != null)
            {
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cam.backgroundColor = Color.black;
                cam.clearFlags = CameraClearFlags.SolidColor;

                Debug.Log("⚙️ Camera configurada para modo ortográfico 2D");
            }

            return true;
        }

        #endregion

        #region CinemachineBrain Fix

        /// <summary>
        /// Corrige o posicionamento do CinemachineBrain e suas configurações.
        /// O CinemachineBrain deve estar no mesmo GameObject da Main Camera, não como filho.
        /// </summary>
        private static bool FixCinemachineBrainPosition()
        {
            Debug.Log("🔍 Verificando posicionamento do CinemachineBrain...");

            GameObject mainCameraGO = Camera.main?.gameObject ?? GameObject.FindWithTag("MainCamera");
            if (mainCameraGO == null)
            {
                Debug.LogError("❌ Main Camera não encontrada para configurar CinemachineBrain");
                return false;
            }

            // Busca por CinemachineBrain existente
            var cinemachineBrainType = System.Type.GetType("Unity.Cinemachine.CinemachineBrain, Unity.Cinemachine");
            if (cinemachineBrainType == null)
            {
                Debug.LogError("❌ Cinemachine não está instalado ou não foi encontrado");
                return false;
            }

            Component brainComponent = mainCameraGO.GetComponent(cinemachineBrainType);

            // Busca por CinemachineBrain em objetos filhos (configuração incorreta)
            Component childBrain = mainCameraGO.GetComponentInChildren(cinemachineBrainType);

            if (childBrain != null && childBrain.gameObject != mainCameraGO)
            {
                Debug.Log("🔄 CinemachineBrain encontrado como filho - movendo para Main Camera...");

                // Remove o GameObject filho com CinemachineBrain
                Object.DestroyImmediate(childBrain.gameObject);
                Debug.Log("🗑️ GameObject filho com CinemachineBrain removido");

                // Força brainComponent a ser null para recriar no local correto
                brainComponent = null;
            }

            // Adiciona CinemachineBrain à Main Camera se não existir
            if (brainComponent == null)
            {
                try
                {
                    brainComponent = mainCameraGO.AddComponent(cinemachineBrainType);
                    Debug.Log("➕ CinemachineBrain adicionado à Main Camera");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Erro ao adicionar CinemachineBrain: {e.Message}");
                    return false;
                }
            }

            // Configura propriedades do CinemachineBrain usando reflection
            ConfigureCinemachineBrain(brainComponent);

            return true;
        }

        /// <summary>
        /// Configura as propriedades do CinemachineBrain usando reflection para compatibilidade.
        /// </summary>
        private static void ConfigureCinemachineBrain(Component brainComponent)
        {
            if (brainComponent == null) return;

            try
            {
                var brainType = brainComponent.GetType();

                // Configura Channel Mask (corrige o "Enum inválido: -1")
                var channelMaskField = brainType.GetField("ChannelMask");
                if (channelMaskField != null)
                {
                    channelMaskField.SetValue(brainComponent, -1); // -1 significa "All Channels"
                }

                // Configura Update Method
                var updateMethodField = brainType.GetField("UpdateMethod");
                if (updateMethodField != null)
                {
                    // 2 = Smart Update
                    updateMethodField.SetValue(brainComponent, 2);
                }

                // Configura Blend Update Method
                var blendUpdateMethodField = brainType.GetField("BlendUpdateMethod");
                if (blendUpdateMethodField != null)
                {
                    // 1 = Late Update
                    blendUpdateMethodField.SetValue(brainComponent, 1);
                }

                Debug.Log("⚙️ CinemachineBrain configurado com parâmetros otimizados");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Alguns parâmetros do CinemachineBrain podem não ter sido configurados: {e.Message}");
            }
        }

        #endregion

        #region CinemachineCamera Configuration

        /// <summary>
        /// Configura a CinemachineCamera com parâmetros apropriados para 2D Follow.
        /// </summary>
        private static bool ConfigureCinemachineCamera()
        {
            Debug.Log("🔍 Configurando CinemachineCamera...");

            var cinemachineCameraType = System.Type.GetType("Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine");
            if (cinemachineCameraType == null)
            {
                Debug.LogError("❌ CinemachineCamera não encontrada");
                return false;
            }

            // Busca por CinemachineCamera na cena
            Component cinemachineCamera = Object.FindFirstObjectByType(cinemachineCameraType) as Component;

            if (cinemachineCamera == null)
            {
                Debug.LogError("❌ Nenhuma CinemachineCamera encontrada na cena. Adicione uma CinemachineCamera primeiro.");
                return false;
            }

            try
            {
                var cameraType = cinemachineCamera.GetType();

                // Configura Priority
                var priorityProperty = cameraType.GetProperty("Priority");
                if (priorityProperty != null)
                {
                    priorityProperty.SetValue(cinemachineCamera, 10);
                }

                // Configura Output Channel
                var outputChannelField = cameraType.GetField("OutputChannel");
                if (outputChannelField != null)
                {
                    outputChannelField.SetValue(cinemachineCamera, 0); // Channel 0
                }

                Debug.Log($"⚙️ CinemachineCamera configurada: {cinemachineCamera.gameObject.name}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Erro ao configurar CinemachineCamera: {e.Message}");
                return false;
            }
        }

        #endregion

        #region Player Target Setup

        /// <summary>
        /// Configura o player como target da CinemachineCamera.
        /// </summary>
        private static bool SetupPlayerTarget()
        {
            Debug.Log("🔍 Configurando Player como target...");

            // Busca pelo player
            GameObject player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("⚠️ Nenhum GameObject com tag 'Player' encontrado. Configure manualmente o Target da CinemachineCamera.");
                return true; // Não é um erro crítico
            }

            // Busca por CinemachineCamera
            var cinemachineCameraType = System.Type.GetType("Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine");
            Component cinemachineCamera = Object.FindFirstObjectByType(cinemachineCameraType) as Component;

            if (cinemachineCamera == null)
            {
                Debug.LogError("❌ CinemachineCamera não encontrada");
                return false;
            }

            try
            {
                // Configura Target usando reflection
                var cameraType = cinemachineCamera.GetType();
                var targetProperty = cameraType.GetProperty("Target");

                if (targetProperty != null)
                {
                    // O Target é um struct complexo, precisamos configurar via SerializedObject
                    SerializedObject so = new SerializedObject(cinemachineCamera);
                    SerializedProperty targetProp = so.FindProperty("Target");

                    if (targetProp != null)
                    {
                        SerializedProperty trackingTargetProp = targetProp.FindPropertyRelative("TrackingTarget");
                        if (trackingTargetProp != null)
                        {
                            trackingTargetProp.objectReferenceValue = player.transform;
                            so.ApplyModifiedProperties();
                            Debug.Log($"🎯 Player configurado como target: {player.name}");
                        }
                    }
                }

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"❌ Erro ao configurar Player target: {e.Message}");
                return false;
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Valida se a configuração está correta.
        /// </summary>
        private static bool ValidateConfiguration()
        {
            Debug.Log("🔍 Validando configuração final...");

            bool isValid = true;

            // Verifica Main Camera
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("❌ Main Camera não encontrada");
                isValid = false;
            }
            else
            {
                Debug.Log($"✅ Main Camera: {mainCamera.gameObject.name}");

                // Verifica CinemachineBrain
                var brainType = System.Type.GetType("Unity.Cinemachine.CinemachineBrain, Unity.Cinemachine");
                if (brainType != null)
                {
                    Component brain = mainCamera.GetComponent(brainType);
                    if (brain != null)
                    {
                        Debug.Log("✅ CinemachineBrain presente na Main Camera");
                    }
                    else
                    {
                        Debug.LogError("❌ CinemachineBrain não encontrado na Main Camera");
                        isValid = false;
                    }
                }
            }

            // Verifica CinemachineCamera
            var cameraType = System.Type.GetType("Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine");
            if (cameraType != null)
            {
                Component cinemachineCamera = Object.FindFirstObjectByType(cameraType) as Component;
                if (cinemachineCamera != null)
                {
                    Debug.Log($"✅ CinemachineCamera: {cinemachineCamera.gameObject.name}");
                }
                else
                {
                    Debug.LogError("❌ CinemachineCamera não encontrada na cena");
                    isValid = false;
                }
            }

            // Verifica Player
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Debug.Log($"✅ Player encontrado: {player.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ Player não encontrado (tag 'Player')");
            }

            return isValid;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Remove componentes CinemachineBrain duplicados ou mal posicionados.
        /// </summary>
        [MenuItem("Extra Tools/Camera Setup/Clean Duplicate Cinemachine Components", false, 200)]
        public static void CleanDuplicateCinemachineComponents()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("⚠️ Não é possível executar limpeza durante o Play Mode.");
                return;
            }

            Debug.Log("🧹 Limpando componentes Cinemachine duplicados...");

            var brainType = System.Type.GetType("Unity.Cinemachine.CinemachineBrain, Unity.Cinemachine");
            if (brainType == null) return;

            Component[] allBrains = Object.FindObjectsByType(brainType, FindObjectsSortMode.None) as Component[];

            if (allBrains.Length > 1)
            {
                Debug.Log($"🔍 Encontrados {allBrains.Length} CinemachineBrain na cena");

                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    // Remove todos os CinemachineBrain exceto o da Main Camera
                    foreach (Component brain in allBrains)
                    {
                        if (brain.gameObject != mainCamera.gameObject)
                        {
                            Debug.Log($"🗑️ Removendo CinemachineBrain duplicado de: {brain.gameObject.name}");
                            Object.DestroyImmediate(brain.gameObject);
                        }
                    }
                }
            }

            Debug.Log("✅ Limpeza concluída");
        }

        #endregion
    }
}