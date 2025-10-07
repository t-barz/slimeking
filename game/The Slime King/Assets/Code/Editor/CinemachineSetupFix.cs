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
        /// Garante que existe uma câmera ativa utilizável. Não exige mais a tag MainCamera, pois a cena atual
        /// usa um GameObject dedicado "CinemachineCamera" contendo tanto a Camera quanto a CinemachineCamera.
        /// Mantemos comportamento não destrutivo: apenas adicionamos Camera se faltar.
        /// </summary>
        private static bool EnsureMainCameraWithCameraComponent()
        {
            Debug.Log("🔍 Verificando câmera principal utilizável...");

            // Primeiro tenta a Camera.main (respeita se existir tag MainCamera configurada).
            Camera mainCam = Camera.main;

            // Se não houver Camera.main, tentamos localizar uma Camera que coexist a com CinemachineCamera.
            if (mainCam == null)
            {
                var unityCinemachineCamType = System.Type.GetType("Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine");
                if (unityCinemachineCamType != null)
                {
                    // Procura qualquer CinemachineCamera e verifica se o mesmo GO tem Camera.
                    Object anyCine = Object.FindFirstObjectByType(unityCinemachineCamType);
                    if (anyCine != null)
                    {
                        var go = ((Component)anyCine).gameObject;
                        mainCam = go.GetComponent<Camera>();
                        if (mainCam != null)
                        {
                            Debug.Log($"✅ Usando câmera encontrada junto à CinemachineCamera: {go.name}");
                        }
                    }
                }
            }

            // Se ainda não encontramos, escolhemos a primeira Camera da cena.
            if (mainCam == null)
            {
                mainCam = Object.FindFirstObjectByType<Camera>();
                if (mainCam != null)
                {
                    Debug.Log($"✅ Usando primeira Camera encontrada na cena: {mainCam.gameObject.name}");
                }
            }

            // Se continuamos sem camera, criamos uma mínima.
            if (mainCam == null)
            {
                GameObject go = new GameObject("MainCamera_Auto");
                mainCam = go.AddComponent<Camera>();
                go.tag = "MainCamera"; // Fornece tag para futuras detecções
                Debug.Log("➕ Camera criada automaticamente (MainCamera_Auto)");
            }

            // Ajuste leve sem forçar estilo (evita sobrescrever intencional): só garantimos ortographic se ainda estiver perspective em projeto 2D.
            if (!mainCam.orthographic)
            {
                mainCam.orthographic = true;
                mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize <= 0 ? 5f : mainCam.orthographicSize, 1f, 50f);
                Debug.Log("⚙️ Camera ajustada para modo ortográfico (2D)");
            }

            return mainCam != null;
        }

        #endregion

        #region CinemachineBrain Fix

        /// <summary>
        /// Garante que existe um CinemachineBrain ativo. Aceita tanto:
        /// 1) Brain no mesmo GameObject da Camera (padrão clássico)
        /// 2) Brain isolado em GameObject dedicado (estado atual da cena)
        /// Não destrói a estrutura existente; apenas configura se necessário.
        /// </summary>
        private static bool FixCinemachineBrainPosition()
        {
            Debug.Log("🔍 Verificando CinemachineBrain...");

            var cinemachineBrainType = System.Type.GetType("Unity.Cinemachine.CinemachineBrain, Unity.Cinemachine");
            if (cinemachineBrainType == null)
            {
                Debug.LogError("❌ Pacote Cinemachine não encontrado.");
                return false;
            }

            Component brainComponent = Object.FindFirstObjectByType(cinemachineBrainType) as Component;

            // Se não existir, tentamos adicionar na Camera principal detectada.
            if (brainComponent == null)
            {
                Camera cam = Camera.main ?? Object.FindFirstObjectByType<Camera>();
                if (cam == null)
                {
                    Debug.LogError("❌ Nenhuma Camera disponível para adicionar CinemachineBrain.");
                    return false;
                }
                try
                {
                    brainComponent = cam.gameObject.AddComponent(cinemachineBrainType);
                    Debug.Log("➕ CinemachineBrain criado na câmera ativa.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"❌ Falha ao adicionar CinemachineBrain: {e.Message}");
                    return false;
                }
            }
            else
            {
                Debug.Log($"✅ CinemachineBrain existente preservado em: {brainComponent.gameObject.name}");
            }

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
            Debug.Log("🔍 Verificando CinemachineCamera existente...");

            var cinemachineCameraType = System.Type.GetType("Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine");
            if (cinemachineCameraType == null)
            {
                Debug.LogWarning("⚠️ Pacote Cinemachine ausente ou tipo não resolvido. Pulando configuração de virtual camera.");
                return false;
            }

            Component cineCam = Object.FindFirstObjectByType(cinemachineCameraType) as Component;
            if (cineCam == null)
            {
                Debug.LogWarning("⚠️ Nenhuma CinemachineCamera encontrada. Nada a configurar.");
                return false;
            }

            try
            {
                var camType = cineCam.GetType();

                // PRIORITY: só define se estiver em zero (valor padrão) para não sobrescrever ajustes do designer
                var priorityProp = camType.GetProperty("Priority");
                if (priorityProp != null)
                {
                    int currentPriority = (int)priorityProp.GetValue(cineCam);
                    if (currentPriority <= 0)
                    {
                        priorityProp.SetValue(cineCam, 10);
                        Debug.Log("⚙️ Priority ajustada para 10");
                    }
                }

                // OUTPUT CHANNEL: define 0 apenas se for inválido (<0)
                var outputField = camType.GetField("OutputChannel");
                if (outputField != null)
                {
                    int currentChannel = (int)outputField.GetValue(cineCam);
                    if (currentChannel < 0)
                    {
                        outputField.SetValue(cineCam, 0);
                        Debug.Log("⚙️ OutputChannel definido para 0 (Default)");
                    }
                }

                // POSITION COMPOSER: se existir, podemos ajustar dead zones mínimas apenas se estiverem negativas (estado inválido).
                var positionComposerType = System.Type.GetType("Unity.Cinemachine.CinemachinePositionComposer, Unity.Cinemachine");
                if (positionComposerType != null)
                {
                    var composer = ((Component)cineCam).GetComponent(positionComposerType);
                    if (composer != null)
                    {
                        SerializedObject so = new SerializedObject(composer);
                        var deadZoneDepth = so.FindProperty("DeadZoneDepth");
                        if (deadZoneDepth != null && deadZoneDepth.floatValue < 0f)
                        {
                            deadZoneDepth.floatValue = 0f;
                            so.ApplyModifiedProperties();
                            Debug.Log("⚙️ DeadZoneDepth corrigido para 0");
                        }
                    }
                }

                Debug.Log($"✅ CinemachineCamera validada: {cineCam.gameObject.name}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"⚠️ Ajustes parciais na CinemachineCamera: {e.Message}");
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

            // Detecta câmera principal de forma flexível
            Camera mainCamera = Camera.main ?? Object.FindFirstObjectByType<Camera>();
            if (mainCamera != null)
            {
                Debug.Log($"✅ Camera ativa detectada: {mainCamera.gameObject.name}");
            }
            else
            {
                Debug.LogError("❌ Nenhuma Camera ativa encontrada.");
                isValid = false;
            }

            // Verifica CinemachineBrain em qualquer lugar da cena
            var brainType = System.Type.GetType("Unity.Cinemachine.CinemachineBrain, Unity.Cinemachine");
            if (brainType != null)
            {
                Component brain = Object.FindFirstObjectByType(brainType) as Component;
                if (brain != null)
                {
                    Debug.Log($"✅ CinemachineBrain presente em: {brain.gameObject.name}");
                }
                else
                {
                    Debug.LogWarning("⚠️ Nenhum CinemachineBrain encontrado. A Cinemachine não controlará a câmera.");
                }
            }

            // Verifica CinemachineCamera
            var cameraType = System.Type.GetType("Unity.Cinemachine.CinemachineCamera, Unity.Cinemachine");
            if (cameraType != null)
            {
                Component cinemachineCamera = Object.FindFirstObjectByType(cameraType) as Component;
                if (cinemachineCamera != null)
                {
                    Debug.Log($"✅ CinemachineCamera detectada: {cinemachineCamera.gameObject.name}");
                }
                else
                {
                    Debug.LogWarning("⚠️ Nenhuma CinemachineCamera encontrada. A câmera ficará estática.");
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