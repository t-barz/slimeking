using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_2D_RENDERER
using UnityEngine.Rendering.Universal;
#endif

namespace ExtraTools
{
    /// <summary>
    /// Módulo especializado para configuração avançada de iluminação.
    /// Configura Global Light 2D, configurações de intensidade e Render Pipeline Asset.
    /// </summary>
    public static class LightingSetupModule
    {
        /// <summary>
        /// Executa o setup completo da iluminação baseado na configuração do SceneSetupManager
        /// </summary>
        /// <param name="setupManager">Referência ao SceneSetupManager com as configurações</param>
        public static void Setup(SceneSetupManager setupManager)
        {
            if (setupManager == null)
            {
                Debug.LogError("[LightingSetup] SceneSetupManager é null");
                return;
            }

            Log(setupManager, "Iniciando setup avançado de iluminação...");

            // Validar configuração do URP
            if (!ValidateURPConfiguration(setupManager))
            {
                return;
            }

            // Configurar iluminação básica
            ConfigureBasicLighting(setupManager);

            // Configurar Global Light 2D se necessário
            if (setupManager.RequireGlobalLight2D)
            {
                ConfigureGlobalLight2D(setupManager);
            }

            // Aplicar configurações específicas por tipo de cena
            ApplyLightingConfigurationForSceneType(setupManager);

            // Configurar qualidade de renderização
            ConfigureRenderingQuality(setupManager);

            Log(setupManager, "Setup de iluminação concluído com sucesso");
        }

        /// <summary>
        /// Valida se o URP está configurado corretamente
        /// </summary>
        private static bool ValidateURPConfiguration(SceneSetupManager setupManager)
        {
            var renderPipelineAsset = GraphicsSettings.defaultRenderPipeline;

            if (renderPipelineAsset == null)
            {
                LogError(setupManager, "Universal Render Pipeline não está configurado");
                LogError(setupManager, "Para configurar: Project Settings → Graphics → Scriptable Render Pipeline Settings");
                return false;
            }

            if (!(renderPipelineAsset is UniversalRenderPipelineAsset urpAsset))
            {
                LogError(setupManager, "Render Pipeline configurado não é URP");
                return false;
            }

            Log(setupManager, "URP configurado corretamente");
            return true;
        }

        /// <summary>
        /// Configura as configurações básicas de iluminação
        /// </summary>
        private static void ConfigureBasicLighting(SceneSetupManager setupManager)
        {
            // Configurar ambiente global
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.2f, 1f); // Ambient suave

            // Configurações de névoa (desabilitada para pixel art por padrão)
            RenderSettings.fog = false;

            // Configurações de skybox (não usado em 2D)
            RenderSettings.skybox = null;

            Log(setupManager, "Configurações básicas de iluminação aplicadas");
        }

        /// <summary>
        /// Configura o Global Light 2D para cenas 2D
        /// </summary>
        private static void ConfigureGlobalLight2D(SceneSetupManager setupManager)
        {
#if UNITY_2D_RENDERER
            // Verificar se já existe um Global Light 2D
            Light2D globalLight = Object.FindFirstObjectByType<Light2D>();
            
            // Filtrar apenas Global Lights
            if (globalLight != null && globalLight.lightType != Light2D.LightType.Global)
            {
                globalLight = null;
                
                // Procurar especificamente por Global Light
                Light2D[] allLights = Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
                foreach (var light in allLights)
                {
                    if (light.lightType == Light2D.LightType.Global)
                    {
                        globalLight = light;
                        break;
                    }
                }
            }
            
            if (globalLight == null)
            {
                Log(setupManager, "Global Light 2D não encontrado. Criando novo...");
                
                GameObject lightGO = new GameObject("Global Light 2D");
                globalLight = lightGO.AddComponent<Light2D>();
                globalLight.lightType = Light2D.LightType.Global;

                Log(setupManager, "Global Light 2D criado com sucesso");
            }
            else
            {
                Log(setupManager, "Global Light 2D encontrado - configurando...");
            }

            // Configurar propriedades básicas
            ConfigureGlobalLight2DProperties(globalLight, setupManager);
#else
            LogWarning(setupManager, "2D Renderer não está disponível. Global Light 2D requer 2D Renderer no URP Asset.");
            LogWarning(setupManager, "Para habilitar: URP Asset → Renderer List → Add 2D Renderer");
#endif
        }

        /// <summary>
        /// Configura as propriedades do Global Light 2D
        /// </summary>
#if UNITY_2D_RENDERER
        private static void ConfigureGlobalLight2DProperties(Light2D globalLight, SceneSetupManager setupManager)
        {
            // Configurações básicas
            globalLight.lightType = Light2D.LightType.Global;
            globalLight.color = Color.white;
            
            // Configurações específicas por tipo de cena
            switch (setupManager.SceneType)
            {
                case SceneType.Gameplay:
                    globalLight.intensity = 1.0f; // Iluminação normal para gameplay
                    globalLight.color = new Color(1f, 0.95f, 0.9f, 1f); // Ligeiramente quente
                    break;
                    
                case SceneType.Menu:
                    globalLight.intensity = 0.8f; // Iluminação mais suave para UI
                    globalLight.color = Color.white; // Neutro para UI
                    break;
                    
                case SceneType.Cutscene:
                    globalLight.intensity = 0.9f; // Iluminação dramática
                    globalLight.color = new Color(0.9f, 0.9f, 1f, 1f); // Ligeiramente fria
                    break;
            }

            // Configurações de qualidade
            globalLight.volumeOpacity = 0f; // Sem volume por padrão
            globalLight.blendStyleIndex = 0; // Blend style padrão

            Log(setupManager, $"Global Light 2D configurado para {setupManager.SceneType}");
        }
#endif

        /// <summary>
        /// Aplica configurações específicas por tipo de cena
        /// </summary>
        private static void ApplyLightingConfigurationForSceneType(SceneSetupManager setupManager)
        {
            switch (setupManager.SceneType)
            {
                case SceneType.Gameplay:
                    // Iluminação otimizada para gameplay
                    RenderSettings.ambientLight = new Color(0.15f, 0.15f, 0.15f, 1f);
                    ConfigureGameplayLighting(setupManager);
                    break;

                case SceneType.Menu:
                    // Iluminação neutra para menus
                    RenderSettings.ambientLight = new Color(0.25f, 0.25f, 0.25f, 1f);
                    ConfigureMenuLighting(setupManager);
                    break;

                case SceneType.Cutscene:
                    // Iluminação dramática para cutscenes
                    RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f, 1f);
                    ConfigureCutsceneLighting(setupManager);
                    break;
            }
        }

        /// <summary>
        /// Configura iluminação específica para gameplay
        /// </summary>
        private static void ConfigureGameplayLighting(SceneSetupManager setupManager)
        {
            // Configurações de sombras para 2D (geralmente desabilitadas)
            QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;

            // Configurações de reflexões (não usadas em 2D)
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
            RenderSettings.reflectionIntensity = 0f;

            Log(setupManager, "Configurações de iluminação para gameplay aplicadas");
        }

        /// <summary>
        /// Configura iluminação específica para menus
        /// </summary>
        private static void ConfigureMenuLighting(SceneSetupManager setupManager)
        {
            // Iluminação uniforme para UI
            QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
            RenderSettings.reflectionIntensity = 0f;

            Log(setupManager, "Configurações de iluminação para menu aplicadas");
        }

        /// <summary>
        /// Configura iluminação específica para cutscenes
        /// </summary>
        private static void ConfigureCutsceneLighting(SceneSetupManager setupManager)
        {
            // Permitir sombras suaves para efeitos dramáticos se necessário
            QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly;
            RenderSettings.reflectionIntensity = 0.2f;

            Log(setupManager, "Configurações de iluminação para cutscene aplicadas");
        }

        /// <summary>
        /// Configura qualidade de renderização otimizada para pixel art
        /// </summary>
        private static void ConfigureRenderingQuality(SceneSetupManager setupManager)
        {
            var urpAsset = GraphicsSettings.defaultRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null) return;

            // As configurações do URP Asset não podem ser modificadas em runtime
            // Apenas loggar recomendações
            LogRenderingQualityRecommendations(setupManager);
        }

        /// <summary>
        /// Registra recomendações de qualidade de renderização
        /// </summary>
        private static void LogRenderingQualityRecommendations(SceneSetupManager setupManager)
        {
            Log(setupManager, "=== Recomendações de URP Asset para Pixel Art ===");
            Log(setupManager, "• Rendering → Anti Aliasing (MSAA): Disabled");
            Log(setupManager, "• Rendering → Render Scale: 1.0");
            Log(setupManager, "• Quality → HDR: Disabled");
            Log(setupManager, "• Shadows → Cast Shadows: Disabled (para 2D)");
            Log(setupManager, "• Post-processing → Post Processing: Enabled");
            Log(setupManager, "• Renderer Features → 2D Renderer Data");
            Log(setupManager, "============================================");
        }

        /// <summary>
        /// Valida a configuração de iluminação
        /// </summary>
        public static bool ValidateLighting(SceneSetupManager setupManager)
        {
            bool isValid = true;

            // Validar URP
            if (!ValidateURPConfiguration(setupManager))
            {
                isValid = false;
            }

            // Validar Global Light 2D se requerido
            if (setupManager.RequireGlobalLight2D)
            {
#if UNITY_2D_RENDERER
                Light2D[] allLights = Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
                bool hasGlobalLight = false;
                
                foreach (var light in allLights)
                {
                    if (light.lightType == Light2D.LightType.Global)
                    {
                        hasGlobalLight = true;
                        break;
                    }
                }
                
                if (!hasGlobalLight)
                {
                    LogWarning(setupManager, "Global Light 2D requerido mas não encontrado");
                    isValid = false;
                }
#else
                LogWarning(setupManager, "2D Renderer não disponível - Global Light 2D não pode ser validado");
                isValid = false;
#endif
            }

            // Validar configurações básicas
            if (RenderSettings.ambientMode != UnityEngine.Rendering.AmbientMode.Flat)
            {
                LogWarning(setupManager, "Ambient Mode não está configurado como Flat (recomendado para 2D)");
            }

            if (isValid)
            {
                Log(setupManager, "✅ Configuração de iluminação validada com sucesso");
            }

            return isValid;
        }

        /// <summary>
        /// Remove configurações de iluminação da cena
        /// </summary>
        public static void CleanupLighting(SceneSetupManager setupManager)
        {
#if UNITY_2D_RENDERER
            Light2D[] allLights = Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
            
            foreach (var light in allLights)
            {
                if (light.lightType == Light2D.LightType.Global)
                {
                    Object.DestroyImmediate(light.gameObject);
                    Log(setupManager, "Global Light 2D removido");
                }
            }
#endif

            // Resetar configurações para padrão
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientLight = new Color(0.212f, 0.227f, 0.259f, 1f);
        }

        #region Helper Methods

        private static void Log(SceneSetupManager setupManager, string message)
        {
            if (setupManager.VerboseLogging)
            {
                Debug.Log($"[LightingSetup] {message}");
            }
        }

        private static void LogWarning(SceneSetupManager setupManager, string message)
        {
            Debug.LogWarning($"[LightingSetup] {message}");
        }

        private static void LogError(SceneSetupManager setupManager, string message)
        {
            Debug.LogError($"[LightingSetup] {message}");
        }

        #endregion
    }
}