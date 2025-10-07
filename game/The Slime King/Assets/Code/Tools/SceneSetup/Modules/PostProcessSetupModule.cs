using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ExtraTools
{
    /// <summary>
    /// Módulo especializado para configuração avançada de Post Processing.
    /// Configura Global Volume, profiles básicos e otimizações para pixel art.
    /// </summary>
    public static class PostProcessSetupModule
    {
        /// <summary>
        /// Executa o setup completo do post processing baseado na configuração do SceneSetupManager
        /// </summary>
        /// <param name="setupManager">Referência ao SceneSetupManager com as configurações</param>
        public static void Setup(SceneSetupManager setupManager)
        {
            if (setupManager == null)
            {
                Debug.LogError("[PostProcessSetup] SceneSetupManager é null");
                return;
            }

            Log(setupManager, "Iniciando setup avançado de post processing...");

            // Verificar se URP está configurado
            if (!ValidateURPConfiguration(setupManager))
            {
                return;
            }

            // Configurar Global Volume se necessário
            if (setupManager.RequireGlobalVolume)
            {
                Volume globalVolume = EnsureGlobalVolume(setupManager);

                if (globalVolume != null)
                {
                    ConfigureVolumeProfile(globalVolume, setupManager);
                    ApplyPostProcessConfigurationForSceneType(globalVolume, setupManager);
                }
            }

            Log(setupManager, "Setup de post processing concluído com sucesso");
        }

        /// <summary>
        /// Valida se o URP está configurado corretamente
        /// </summary>
        private static bool ValidateURPConfiguration(SceneSetupManager setupManager)
        {
            var renderPipelineAsset = GraphicsSettings.defaultRenderPipeline;

            if (renderPipelineAsset == null)
            {
                LogError(setupManager, "Universal Render Pipeline não está configurado. Post Processing requer URP.");
                LogError(setupManager, "Para configurar: Project Settings → Graphics → Scriptable Render Pipeline Settings");
                return false;
            }

            if (!(renderPipelineAsset is UniversalRenderPipelineAsset))
            {
                LogError(setupManager, "Render Pipeline configurado não é URP. Post Processing requer Universal Render Pipeline.");
                return false;
            }

            Log(setupManager, "URP configurado corretamente");
            return true;
        }

        /// <summary>
        /// Garante que existe um Global Volume na cena
        /// </summary>
        private static Volume EnsureGlobalVolume(SceneSetupManager setupManager)
        {
            Volume globalVolume = Object.FindFirstObjectByType<Volume>();

            if (globalVolume == null)
            {
                Log(setupManager, "Global Volume não encontrado. Criando novo volume...");

                GameObject volumeGO = new GameObject("Global Volume");
                globalVolume = volumeGO.AddComponent<Volume>();
                globalVolume.isGlobal = true;
                globalVolume.priority = 0f;

                Log(setupManager, "Global Volume criado com sucesso");
            }
            else
            {
                Log(setupManager, "Global Volume encontrado - configurando...");

                // Garantir que é um volume global
                if (!globalVolume.isGlobal)
                {
                    globalVolume.isGlobal = true;
                    Log(setupManager, "Volume configurado como global");
                }
            }

            return globalVolume;
        }

        /// <summary>
        /// Configura o Volume Profile com efeitos básicos
        /// </summary>
        private static void ConfigureVolumeProfile(Volume volume, SceneSetupManager setupManager)
        {
            if (volume.profile == null)
            {
                // Criar um novo Volume Profile
                volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
                volume.profile.name = $"Profile_{setupManager.SceneType}";
                Log(setupManager, "Novo Volume Profile criado");
            }

            var profile = volume.profile;

            // Configurar Vignette (especialmente útil para transições)
            ConfigureVignette(profile, setupManager);

            // Configurar Color Adjustments para pixel art
            ConfigureColorAdjustments(profile, setupManager);

            // Configurar Tonemapping otimizado para pixel art
            ConfigureTonemapping(profile, setupManager);

            // Configurar Film Grain para estilo retro (opcional)
            if (setupManager.SceneType == SceneType.Gameplay)
            {
                ConfigureFilmGrain(profile, setupManager);
            }

            Log(setupManager, "Volume Profile configurado com efeitos básicos");
        }

        /// <summary>
        /// Configura o efeito Vignette
        /// </summary>
        private static void ConfigureVignette(VolumeProfile profile, SceneSetupManager setupManager)
        {
            if (!profile.TryGet(out Vignette vignette))
            {
                vignette = profile.Add<Vignette>();
                Log(setupManager, "Vignette adicionado ao profile");
            }

            // Configurações padrão para vinheta suave
            vignette.intensity.value = 0.2f;
            vignette.smoothness.value = 0.4f;
            vignette.color.value = Color.black;
            vignette.center.value = new Vector2(0.5f, 0.5f);
            vignette.rounded.value = false;

            // Configurações específicas por tipo de cena
            switch (setupManager.SceneType)
            {
                case SceneType.Gameplay:
                    vignette.intensity.value = 0.15f; // Vinheta mais sutil para não atrapalhar gameplay
                    break;
                case SceneType.Menu:
                    vignette.intensity.value = 0.25f; // Vinheta mais presente para foco
                    break;
                case SceneType.Cutscene:
                    vignette.intensity.value = 0.3f; // Vinheta mais dramática
                    break;
            }

            Log(setupManager, $"Vignette configurado para {setupManager.SceneType}");
        }

        /// <summary>
        /// Configura Color Adjustments para pixel art
        /// </summary>
        private static void ConfigureColorAdjustments(VolumeProfile profile, SceneSetupManager setupManager)
        {
            if (!profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                colorAdjustments = profile.Add<ColorAdjustments>();
                Log(setupManager, "Color Adjustments adicionado ao profile");
            }

            // Configurações otimizadas para pixel art
            colorAdjustments.postExposure.value = 0f;
            colorAdjustments.contrast.value = 10f; // Contraste ligeiramente aumentado para pixel art
            colorAdjustments.colorFilter.value = Color.white;
            colorAdjustments.hueShift.value = 0f;
            colorAdjustments.saturation.value = 5f; // Saturação ligeiramente aumentada

            // Configurações específicas por tipo de cena
            switch (setupManager.SceneType)
            {
                case SceneType.Gameplay:
                    // Cores vibrantes para gameplay
                    colorAdjustments.saturation.value = 8f;
                    colorAdjustments.contrast.value = 12f;
                    break;
                case SceneType.Menu:
                    // Cores mais neutras para UI
                    colorAdjustments.saturation.value = 0f;
                    colorAdjustments.contrast.value = 5f;
                    break;
                case SceneType.Cutscene:
                    // Cores cinematic
                    colorAdjustments.saturation.value = -5f;
                    colorAdjustments.contrast.value = 15f;
                    break;
            }

            Log(setupManager, $"Color Adjustments configurado para {setupManager.SceneType}");
        }

        /// <summary>
        /// Configura Tonemapping otimizado para pixel art
        /// </summary>
        private static void ConfigureTonemapping(VolumeProfile profile, SceneSetupManager setupManager)
        {
            if (!profile.TryGet(out Tonemapping tonemapping))
            {
                tonemapping = profile.Add<Tonemapping>();
                Log(setupManager, "Tonemapping adicionado ao profile");
            }

            // Usar None ou Neutral para pixel art (evita distorção de cores)
            tonemapping.mode.value = TonemappingMode.Neutral;

            Log(setupManager, "Tonemapping configurado como Neutral para preservar cores");
        }

        /// <summary>
        /// Configura Film Grain para estilo retro
        /// </summary>
        private static void ConfigureFilmGrain(VolumeProfile profile, SceneSetupManager setupManager)
        {
            if (!profile.TryGet(out FilmGrain filmGrain))
            {
                filmGrain = profile.Add<FilmGrain>();
                Log(setupManager, "Film Grain adicionado ao profile");
            }

            // Configurações sutis para efeito retro
            filmGrain.type.value = FilmGrainLookup.Thin1;
            filmGrain.intensity.value = 0.1f;
            filmGrain.response.value = 0.8f;

            Log(setupManager, "Film Grain configurado para efeito retro sutil");
        }

        /// <summary>
        /// Aplica configurações específicas por tipo de cena
        /// </summary>
        private static void ApplyPostProcessConfigurationForSceneType(Volume volume, SceneSetupManager setupManager)
        {
            switch (setupManager.SceneType)
            {
                case SceneType.Gameplay:
                    volume.priority = 0f; // Prioridade padrão
                    volume.blendDistance = 0f; // Transição imediata
                    Log(setupManager, "Configurações de gameplay aplicadas ao volume");
                    break;

                case SceneType.Menu:
                    volume.priority = 1f; // Prioridade alta para sobrepor outros volumes
                    volume.blendDistance = 0f;
                    Log(setupManager, "Configurações de menu aplicadas ao volume");
                    break;

                case SceneType.Cutscene:
                    volume.priority = 2f; // Prioridade mais alta para efeitos dramáticos
                    volume.blendDistance = 5f; // Transição mais suave
                    Log(setupManager, "Configurações de cutscene aplicadas ao volume");
                    break;
            }
        }

        /// <summary>
        /// Valida a configuração de post processing
        /// </summary>
        public static bool ValidatePostProcessing(SceneSetupManager setupManager)
        {
            bool isValid = true;

            // Validar URP
            if (!ValidateURPConfiguration(setupManager))
            {
                isValid = false;
            }

            // Validar Global Volume se requerido
            if (setupManager.RequireGlobalVolume)
            {
                Volume globalVolume = Object.FindFirstObjectByType<Volume>();
                if (globalVolume == null)
                {
                    LogWarning(setupManager, "Global Volume requerido mas não encontrado");
                    isValid = false;
                }
                else
                {
                    if (!globalVolume.isGlobal)
                    {
                        LogWarning(setupManager, "Volume encontrado não está configurado como Global");
                        isValid = false;
                    }

                    if (globalVolume.profile == null)
                    {
                        LogWarning(setupManager, "Global Volume não possui um Volume Profile");
                        isValid = false;
                    }
                }
            }

            if (isValid)
            {
                Log(setupManager, "✅ Configuração de post processing validada com sucesso");
            }

            return isValid;
        }

        /// <summary>
        /// Remove configurações de post processing da cena
        /// </summary>
        public static void CleanupPostProcessing(SceneSetupManager setupManager)
        {
            Volume[] volumes = Object.FindObjectsByType<Volume>(FindObjectsSortMode.None);

            foreach (var volume in volumes)
            {
                if (volume.isGlobal)
                {
                    Object.DestroyImmediate(volume.gameObject);
                    Log(setupManager, "Global Volume removido");
                }
            }
        }

        #region Helper Methods

        private static void Log(SceneSetupManager setupManager, string message)
        {
            if (setupManager.VerboseLogging)
            {
                Debug.Log($"[PostProcessSetup] {message}");
            }
        }

        private static void LogWarning(SceneSetupManager setupManager, string message)
        {
            Debug.LogWarning($"[PostProcessSetup] {message}");
        }

        private static void LogError(SceneSetupManager setupManager, string message)
        {
            Debug.LogError($"[PostProcessSetup] {message}");
        }

        #endregion
    }
}