#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

namespace SlimeKing.Core.EditorTools
{
    public class ProjectSettingsExporterWindow : EditorWindow
    {
        [Serializable]
        private class CompleteProjectSettings
        {
            // Player Settings
            public PlayerSettingsData playerSettings = new();

            // Quality Settings
            public QualitySettingsData qualitySettings = new();

            // Physics Settings
            public PhysicsSettingsData physicsSettings = new();

            // Physics2D Settings
            public Physics2DSettingsData physics2DSettings = new();

            // Time Settings
            public TimeSettingsData timeSettings = new();

            // Audio Settings
            public AudioSettingsData audioSettings = new();

            // Graphics Settings
            public GraphicsSettingsData graphicsSettings = new();

            // Input Settings
            public InputSettingsData inputSettings = new();

            // Build Settings
            public BuildSettingsData buildSettings = new();

            // Tags and Layers
            public TagsLayersData tagsLayers = new();

            // URP Settings (se disponível)
            public URPSettingsData urpSettings = new();

            // Metadata
            public string exportedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            public string unityVersion = Application.unityVersion;
            public string projectName;
        }

        [Serializable]
        private class PlayerSettingsData
        {
            // Basic Info
            public string companyName;
            public string productName;
            public string version;
            public string bundleVersion;

            // Display
            public int defaultScreenWidth;
            public int defaultScreenHeight;
            public string fullScreenMode;
            public bool runInBackground;
            public bool captureSingleScreen;
            public bool usePlayerLog;
            public bool resizableWindow;
            public bool visibleInBackground;
            public bool allowFullscreenSwitch;
            public bool forceSingleInstance;

            // Rendering
            public string colorSpace;
            public bool allowHDR;
            public bool useHDRDisplay;
            public bool allowedAutorotateToPortrait;
            public bool allowedAutorotateToPortraitUpsideDown;
            public bool allowedAutorotateToLandscapeLeft;
            public bool allowedAutorotateToLandscapeRight;
            public string defaultOrientation;

            // Scripting
            public Dictionary<string, string> scriptingDefineSymbols = new();
            public Dictionary<string, string> scriptingBackends = new();
            public Dictionary<string, string> apiCompatibilityLevels = new();

            // XR
            public bool virtualRealitySupported;
            public string[] virtualRealitySDKs;

            // Publishing
            public bool allowUnsafeCode;
            public bool suppressCommonWarnings;

            // Platform Specific
            public StandaloneSettingsData standalone = new();
            public AndroidSettingsData android = new();
            public iOSSettingsData iOS = new();
            public WebGLSettingsData webGL = new();
        }

        [Serializable]
        private class StandaloneSettingsData
        {
            public string iconKind;
            public bool createDesktopShortcut;
            public bool createStartMenuShortcut;
        }

        [Serializable]
        private class AndroidSettingsData
        {
            public string packageName;
            public int bundleVersionCode;
            public int minSdkVersion;
            public int targetSdkVersion;
            public string targetArchitectures;
            public string splashScreenScale;
            public bool useAPKExpansionFiles;
            public string keystoreName;
            public string keystorePass;
            public string keyaliasName;
            public string keyaliasPass;
        }

        [Serializable]
        private class iOSSettingsData
        {
            public string bundleIdentifier;
            public string buildNumber;
            public string targetOSVersionString;
            public string targetDevice;
            public string sdkVersion;
            public bool requiresFullScreen;
            public bool hideHomeButton;
            public bool statusBarHidden;
            public string statusBarStyle;
        }

        [Serializable]
        private class WebGLSettingsData
        {
            public string memorySize;
            public bool exceptionSupport;
            public string compressionFormat;
            public bool linkerTarget;
        }

        [Serializable]
        private class QualitySettingsData
        {
            public int currentLevel;
            public string[] qualityLevelNames;
            public QualityLevelData[] levels;
        }

        [Serializable]
        private class QualityLevelData
        {
            public string name;
            public int pixelLightCount;
            public int anisotropicFiltering;
            public int antiAliasing;
            public int vSyncCount;
            public float lodBias;
            public int maximumLODLevel;
            public int masterTextureLimit;
            public int particleRaycastBudget;
            public int softParticles;
            public bool softVegetation;
            public bool realtimeReflectionProbes;
            public bool billboardsFaceCameraPosition;
            public float resolutionScalingFixedDPIFactor;
            public int shadowCascades;
            public float shadowDistance;
            public string shadowmaskMode;
            public string shadowProjection;
            public string shadowResolution;
            public string shadows;
            public float shadowNearPlaneOffset;
            public int streamingMipmapsActive;
            public float streamingMipmapsMemoryBudget;
            public int streamingMipmapsRenderersPerFrame;
            public int streamingMipmapsMaxLevelReduction;
            public int streamingMipmapsAddAllCameras;
            public int streamingMipmapsMaxFileIORequests;
        }

        [Serializable]
        private class PhysicsSettingsData
        {
            public Vector3 gravity;
            public string defaultMaterial;
            public float bounceThreshold;
            public float sleepThreshold;
            public float defaultContactOffset;
            public float defaultSolverIterations;
            public float defaultSolverVelocityIterations;
            public bool queriesHitBackfaces;
            public bool queriesHitTriggers;
            public bool enableAdaptiveForce;
            public bool autoSimulation;
            public bool autoSyncTransforms;
            public LayerCollisionMatrixData layerCollisionMatrix = new();
        }

        [Serializable]
        private class Physics2DSettingsData
        {
            public Vector2 gravity;
            public string defaultMaterial;
            public int velocityIterations;
            public int positionIterations;
            public float velocityThreshold;
            public float maxLinearCorrection;
            public float maxAngularCorrection;
            public float maxTranslationSpeed;
            public float maxRotationSpeed;
            public float baumgarteScale;
            public float baumgarteTOIScale;
            public float timeToSleep;
            public float linearSleepTolerance;
            public float angularSleepTolerance;
            public bool defaultContactOffset;
            public int jobOptions;
            public bool autoSimulation;
            public bool queriesHitTriggers;
            public bool queriesStartInColliders;
            public bool callbacksOnDisable;
            public bool reuseCollisionCallbacks;
            public bool autoSyncTransforms;
            public LayerCollisionMatrixData layerCollisionMatrix = new();
        }

        [Serializable]
        private class LayerCollisionMatrixData
        {
            public Dictionary<string, bool[]> matrix = new();
        }

        [Serializable]
        private class TimeSettingsData
        {
            public float fixedTimestep;
            public float maximumDeltaTime;
            public float timeScale;
            public int maximumParticleTimestep;
        }

        [Serializable]
        private class AudioSettingsData
        {
            public float volume;
            public int dspBufferSize;
            public int sampleRate;
            public string spatializerPlugin;
            public string ambisonicDecoderPlugin;
            public bool disableAudio;
            public bool virtualizeEffects;
        }

        [Serializable]
        private class GraphicsSettingsData
        {
            public string currentRenderPipeline;
            public string[] alwaysIncludedShaders;
            public string[] preloadedShaders;
            public string[] shaderStrippingSettings;
            public bool logWhenShaderIsCompiled;
        }

        [Serializable]
        private class InputSettingsData
        {
            public bool usePhysicalKeys;
            public string[] inputAxes;
            public bool enableInputSystem;
            public bool enableLegacyInputManager;
            public string activeInputHandling;
        }

        [Serializable]
        private class BuildSettingsData
        {
            public EditorBuildSettingsScene[] scenes;
            public string[] enabledScenePaths;
        }

        [Serializable]
        private class TagsLayersData
        {
            public string[] tags;
            public string[] layers;
            public string[] sortingLayers;
        }

        [Serializable]
        private class URPSettingsData
        {
            public string urpAssetPath;
            public bool isURPActive;
            // Dados básicos do URP Asset se encontrado
            public Dictionary<string, object> urpAssetData = new();
        }

        private const string MenuPath = "Tools/SlimeKing/Export Complete Project Settings";
        private bool enableDebugLogs = true;
        private string lastExportPath;

        [MenuItem(MenuPath)]
        public static void Open()
        {
            GetWindow<ProjectSettingsExporterWindow>("Export Project Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("Exportar Project Settings Completo", EditorStyles.boldLabel);

            enableDebugLogs = EditorGUILayout.Toggle("Enable Debug Logs", enableDebugLogs);

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Este exportador coleta TODAS as configurações do projeto em um único JSON estruturado.", MessageType.Info);

            if (GUILayout.Button("Exportar Configurações Completas", GUILayout.Height(30)))
            {
                ExportCompleteSettings();
            }

            if (!string.IsNullOrEmpty(lastExportPath))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox($"Último export: {lastExportPath}", MessageType.Info);
                if (GUILayout.Button("Mostrar na pasta"))
                {
                    EditorUtility.RevealInFinder(lastExportPath);
                }
            }
        }

        private void ExportCompleteSettings()
        {
            var fileName = $"ProjectSettings_Complete_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var jsonPath = EditorUtility.SaveFilePanel("Salvar Configurações Completas", "", fileName, "json");

            if (string.IsNullOrEmpty(jsonPath))
                return;

            try
            {
                Log("Iniciando coleta de configurações...");
                var completeSettings = CollectAllSettings();

                Log("Serializando para JSON...");
                var json = JsonUtility.ToJson(completeSettings, true);

                Log("Salvando arquivo...");
                File.WriteAllText(jsonPath, json);

                lastExportPath = jsonPath;
                Log($"Export concluído com sucesso: {jsonPath}");

                EditorUtility.DisplayDialog("Export Concluído",
                    $"Configurações exportadas para:\n{jsonPath}", "OK");
            }
            catch (Exception ex)
            {
                LogError($"Falha no export: {ex.Message}");
                EditorUtility.DisplayDialog("Erro no Export",
                    $"Falha ao exportar configurações:\n{ex.Message}", "OK");
            }
        }

        private CompleteProjectSettings CollectAllSettings()
        {
            var settings = new CompleteProjectSettings
            {
                projectName = Path.GetFileName(Directory.GetCurrentDirectory())
            };

            Log("Coletando Player Settings...");
            CollectPlayerSettings(settings.playerSettings);

            Log("Coletando Quality Settings...");
            CollectQualitySettings(settings.qualitySettings);

            Log("Coletando Physics Settings...");
            CollectPhysicsSettings(settings.physicsSettings);

            Log("Coletando Physics2D Settings...");
            CollectPhysics2DSettings(settings.physics2DSettings);

            Log("Coletando Time Settings...");
            CollectTimeSettings(settings.timeSettings);

            Log("Coletando Audio Settings...");
            CollectAudioSettings(settings.audioSettings);

            Log("Coletando Graphics Settings...");
            CollectGraphicsSettings(settings.graphicsSettings);

            Log("Coletando Input Settings...");
            CollectInputSettings(settings.inputSettings);

            Log("Coletando Build Settings...");
            CollectBuildSettings(settings.buildSettings);

            Log("Coletando Tags & Layers...");
            CollectTagsLayers(settings.tagsLayers);

            Log("Coletando URP Settings...");
            CollectURPSettings(settings.urpSettings);

            return settings;
        }

        private void CollectPlayerSettings(PlayerSettingsData data)
        {
            // Basic Info
            data.companyName = PlayerSettings.companyName;
            data.productName = PlayerSettings.productName;
            data.version = PlayerSettings.bundleVersion;
            data.bundleVersion = PlayerSettings.bundleVersion;

            // Display
            data.defaultScreenWidth = PlayerSettings.defaultScreenWidth;
            data.defaultScreenHeight = PlayerSettings.defaultScreenHeight;
            data.fullScreenMode = PlayerSettings.fullScreenMode.ToString();
            data.runInBackground = PlayerSettings.runInBackground;
            // data.captureSingleScreen = PlayerSettings.captureSingleScreen; // Obsolete
            data.usePlayerLog = PlayerSettings.usePlayerLog;
            data.resizableWindow = PlayerSettings.resizableWindow;
            data.visibleInBackground = PlayerSettings.visibleInBackground;
            data.allowFullscreenSwitch = PlayerSettings.allowFullscreenSwitch;
            data.forceSingleInstance = PlayerSettings.forceSingleInstance;

            // Rendering
            data.colorSpace = PlayerSettings.colorSpace.ToString();
            data.allowHDR = PlayerSettings.allowHDRDisplaySupport;
            data.useHDRDisplay = PlayerSettings.useHDRDisplay;

            // Mobile orientation
            data.allowedAutorotateToPortrait = PlayerSettings.allowedAutorotateToPortrait;
            data.allowedAutorotateToPortraitUpsideDown = PlayerSettings.allowedAutorotateToPortraitUpsideDown;
            data.allowedAutorotateToLandscapeLeft = PlayerSettings.allowedAutorotateToLandscapeLeft;
            data.allowedAutorotateToLandscapeRight = PlayerSettings.allowedAutorotateToLandscapeRight;
            data.defaultOrientation = PlayerSettings.defaultInterfaceOrientation.ToString();

            // Scripting defines per platform
            foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (group == BuildTargetGroup.Unknown) continue;
                try
                {
                    var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);
                    var defines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
                    if (!string.IsNullOrEmpty(defines))
                        data.scriptingDefineSymbols[group.ToString()] = defines;

                    var backend = PlayerSettings.GetScriptingBackend(namedTarget);
                    data.scriptingBackends[group.ToString()] = backend.ToString();

                    var api = PlayerSettings.GetApiCompatibilityLevel(namedTarget);
                    data.apiCompatibilityLevels[group.ToString()] = api.ToString();
                }
                catch { /* Ignore unsupported platforms */ }
            }

            // Platform specific
            CollectStandaloneSettings(data.standalone);
            CollectAndroidSettings(data.android);
            CollectiOSSettings(data.iOS);
            CollectWebGLSettings(data.webGL);
        }

        private void CollectStandaloneSettings(StandaloneSettingsData data)
        {
            try
            {
                // PlayerSettings.Standalone não existe mais nas versões mais recentes
                // data.iconKind = PlayerSettings.Standalone.iconKind.ToString();
            }
            catch { /* Platform may not be available */ }
        }

        private void CollectAndroidSettings(AndroidSettingsData data)
        {
            try
            {
#if UNITY_ANDROID
                data.packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                data.bundleVersionCode = PlayerSettings.Android.bundleVersionCode;
                data.minSdkVersion = (int)PlayerSettings.Android.minSdkVersion;
                data.targetSdkVersion = (int)PlayerSettings.Android.targetSdkVersion;
                data.targetArchitectures = PlayerSettings.Android.targetArchitectures.ToString();
                data.splashScreenScale = PlayerSettings.Android.splashScreenScale.ToString();
                data.useAPKExpansionFiles = PlayerSettings.Android.useAPKExpansionFiles;
                data.keystoreName = PlayerSettings.Android.keystoreName;
                data.keyaliasName = PlayerSettings.Android.keyaliasName;
#endif
            }
            catch { /* Android platform may not be available */ }
        }

        private void CollectiOSSettings(iOSSettingsData data)
        {
            try
            {
#if UNITY_IOS
                data.bundleIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
                data.buildNumber = PlayerSettings.iOS.buildNumber;
                data.targetOSVersionString = PlayerSettings.iOS.targetOSVersionString;
                data.targetDevice = PlayerSettings.iOS.targetDevice.ToString();
                data.sdkVersion = PlayerSettings.iOS.sdkVersion.ToString();
                data.requiresFullScreen = PlayerSettings.iOS.requiresFullScreen;
                data.hideHomeButton = PlayerSettings.iOS.hideHomeButton;
                data.statusBarHidden = PlayerSettings.iOS.statusBarHidden;
                data.statusBarStyle = PlayerSettings.iOS.statusBarStyle.ToString();
#endif
            }
            catch { /* iOS platform may not be available */ }
        }

        private void CollectWebGLSettings(WebGLSettingsData data)
        {
            try
            {
#if UNITY_WEBGL
                data.memorySize = PlayerSettings.WebGL.memorySize.ToString();
                data.exceptionSupport = PlayerSettings.WebGL.exceptionSupport.ToString().Contains("True");
                data.compressionFormat = PlayerSettings.WebGL.compressionFormat.ToString();
                data.linkerTarget = PlayerSettings.WebGL.linkerTarget.ToString().Contains("True");
#endif
            }
            catch { /* WebGL platform may not be available */ }
        }

        private void CollectQualitySettings(QualitySettingsData data)
        {
            data.currentLevel = QualitySettings.GetQualityLevel();
            data.qualityLevelNames = QualitySettings.names;
            data.levels = new QualityLevelData[data.qualityLevelNames.Length];

            for (int i = 0; i < data.qualityLevelNames.Length; i++)
            {
                // Temporariamente mude para este nível para ler suas configurações
                var originalLevel = QualitySettings.GetQualityLevel();
                QualitySettings.SetQualityLevel(i, false);

                var level = new QualityLevelData
                {
                    name = data.qualityLevelNames[i],
                    pixelLightCount = QualitySettings.pixelLightCount,
                    anisotropicFiltering = (int)QualitySettings.anisotropicFiltering,
                    antiAliasing = QualitySettings.antiAliasing,
                    vSyncCount = QualitySettings.vSyncCount,
                    lodBias = QualitySettings.lodBias,
                    maximumLODLevel = QualitySettings.maximumLODLevel,
                    masterTextureLimit = QualitySettings.globalTextureMipmapLimit,
                    particleRaycastBudget = QualitySettings.particleRaycastBudget,
                    softParticles = QualitySettings.softParticles ? 1 : 0,
                    softVegetation = QualitySettings.softVegetation,
                    realtimeReflectionProbes = QualitySettings.realtimeReflectionProbes,
                    billboardsFaceCameraPosition = QualitySettings.billboardsFaceCameraPosition,
                    resolutionScalingFixedDPIFactor = QualitySettings.resolutionScalingFixedDPIFactor,
                    shadowCascades = QualitySettings.shadowCascades,
                    shadowDistance = QualitySettings.shadowDistance,
                    shadowmaskMode = QualitySettings.shadowmaskMode.ToString(),
                    shadowProjection = QualitySettings.shadowProjection.ToString(),
                    shadowResolution = QualitySettings.shadowResolution.ToString(),
                    shadows = QualitySettings.shadows.ToString(),
                    shadowNearPlaneOffset = QualitySettings.shadowNearPlaneOffset,
                    streamingMipmapsActive = QualitySettings.streamingMipmapsActive ? 1 : 0,
                    streamingMipmapsMemoryBudget = QualitySettings.streamingMipmapsMemoryBudget,
                    streamingMipmapsRenderersPerFrame = QualitySettings.streamingMipmapsRenderersPerFrame,
                    streamingMipmapsMaxLevelReduction = QualitySettings.streamingMipmapsMaxLevelReduction,
                    streamingMipmapsAddAllCameras = QualitySettings.streamingMipmapsAddAllCameras ? 1 : 0,
                    streamingMipmapsMaxFileIORequests = QualitySettings.streamingMipmapsMaxFileIORequests
                };

                data.levels[i] = level;

                // Restaura o nível original
                QualitySettings.SetQualityLevel(originalLevel, false);
            }
        }

        private void CollectPhysicsSettings(PhysicsSettingsData data)
        {
            data.gravity = Physics.gravity;
            data.defaultMaterial = "None"; // Physics.defaultPhysicsMaterial não existe mais
            data.bounceThreshold = Physics.bounceThreshold;
            data.sleepThreshold = Physics.sleepThreshold;
            data.defaultContactOffset = Physics.defaultContactOffset;
            data.defaultSolverIterations = Physics.defaultSolverIterations;
            data.defaultSolverVelocityIterations = Physics.defaultSolverVelocityIterations;
            data.queriesHitBackfaces = Physics.queriesHitBackfaces;
            data.queriesHitTriggers = Physics.queriesHitTriggers;
            data.autoSimulation = Physics.simulationMode == SimulationMode.FixedUpdate;
            data.autoSyncTransforms = Physics.autoSyncTransforms;

            // Layer collision matrix
            for (int i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    var collisions = new bool[32];
                    for (int j = 0; j < 32; j++)
                    {
                        collisions[j] = !Physics.GetIgnoreLayerCollision(i, j);
                    }
                    data.layerCollisionMatrix.matrix[layerName] = collisions;
                }
            }
        }

        private void CollectPhysics2DSettings(Physics2DSettingsData data)
        {
            data.gravity = Physics2D.gravity;
            data.defaultMaterial = "None"; // Physics2D.defaultPhysicsMaterial não existe mais
            data.velocityIterations = Physics2D.velocityIterations;
            data.positionIterations = Physics2D.positionIterations;
            data.velocityThreshold = Physics2D.bounceThreshold;
            data.maxLinearCorrection = Physics2D.maxLinearCorrection;
            data.maxAngularCorrection = Physics2D.maxAngularCorrection;
            data.maxTranslationSpeed = Physics2D.maxTranslationSpeed;
            data.maxRotationSpeed = Physics2D.maxRotationSpeed;
            data.baumgarteScale = Physics2D.baumgarteScale;
            data.baumgarteTOIScale = Physics2D.baumgarteTOIScale;
            data.timeToSleep = Physics2D.timeToSleep;
            data.linearSleepTolerance = Physics2D.linearSleepTolerance;
            data.angularSleepTolerance = Physics2D.angularSleepTolerance;
            data.autoSimulation = Physics2D.simulationMode == SimulationMode2D.FixedUpdate;
            data.queriesHitTriggers = Physics2D.queriesHitTriggers;
            data.queriesStartInColliders = Physics2D.queriesStartInColliders;
            data.callbacksOnDisable = Physics2D.callbacksOnDisable;
            data.reuseCollisionCallbacks = Physics2D.reuseCollisionCallbacks;
            data.autoSyncTransforms = Physics2D.autoSyncTransforms;

            // Layer collision matrix for 2D
            for (int i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    var collisions = new bool[32];
                    for (int j = 0; j < 32; j++)
                    {
                        collisions[j] = !Physics2D.GetIgnoreLayerCollision(i, j);
                    }
                    data.layerCollisionMatrix.matrix[layerName] = collisions;
                }
            }
        }

        private void CollectTimeSettings(TimeSettingsData data)
        {
            data.fixedTimestep = Time.fixedDeltaTime;
            data.maximumDeltaTime = Time.maximumDeltaTime;
            data.timeScale = Time.timeScale;
            data.maximumParticleTimestep = (int)Time.maximumParticleDeltaTime;
        }

        private void CollectAudioSettings(AudioSettingsData data)
        {
            var config = AudioSettings.GetConfiguration();
            data.volume = AudioListener.volume;
            data.dspBufferSize = config.dspBufferSize;
            data.sampleRate = config.sampleRate;
            data.spatializerPlugin = AudioSettings.GetSpatializerPluginName();
            // data.ambisonicDecoderPlugin = AudioSettings.GetAmbisonicDecoderPluginName(); // Não existe mais
        }

        private void CollectGraphicsSettings(GraphicsSettingsData data)
        {
            var pipeline = GraphicsSettings.defaultRenderPipeline;
            data.currentRenderPipeline = pipeline ? pipeline.name : "Built-in";

            // Collect always included shaders
            var alwaysIncluded = new List<string>();
            for (int i = 0; i < GraphicsSettings.allConfiguredRenderPipelines.Length; i++)
            {
                var asset = GraphicsSettings.allConfiguredRenderPipelines[i];
                if (asset) alwaysIncluded.Add(asset.name);
            }
            data.alwaysIncludedShaders = alwaysIncluded.ToArray();
        }

        private void CollectInputSettings(InputSettingsData data)
        {
            // Collect input axes from InputManager
            var inputManagerAsset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
            var serializedObject = new SerializedObject(inputManagerAsset);
            var axesProperty = serializedObject.FindProperty("m_Axes");

            var axes = new List<string>();
            for (int i = 0; i < axesProperty.arraySize; i++)
            {
                var axis = axesProperty.GetArrayElementAtIndex(i);
                var name = axis.FindPropertyRelative("m_Name").stringValue;
                axes.Add(name);
            }
            data.inputAxes = axes.ToArray();

#if ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            data.activeInputHandling = "Both";
            data.enableInputSystem = true;
            data.enableLegacyInputManager = true;
#elif ENABLE_INPUT_SYSTEM
            data.activeInputHandling = "Input System Package (New)";
            data.enableInputSystem = true;
            data.enableLegacyInputManager = false;
#else
            data.activeInputHandling = "Input Manager (Old)";
            data.enableInputSystem = false;
            data.enableLegacyInputManager = true;
#endif
        }

        private void CollectBuildSettings(BuildSettingsData data)
        {
            data.scenes = EditorBuildSettings.scenes;
            data.enabledScenePaths = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();
        }

        private void CollectTagsLayers(TagsLayersData data)
        {
            // Tags
            var tagManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            var serializedObject = new SerializedObject(tagManager);

            var tagsProperty = serializedObject.FindProperty("tags");
            var tags = new List<string>();
            for (int i = 0; i < tagsProperty.arraySize; i++)
            {
                tags.Add(tagsProperty.GetArrayElementAtIndex(i).stringValue);
            }
            data.tags = tags.ToArray();

            // Layers
            var layers = new List<string>();
            for (int i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName(i);
                layers.Add(string.IsNullOrEmpty(layerName) ? $"Layer {i}" : layerName);
            }
            data.layers = layers.ToArray();

            // Sorting Layers
            var sortingLayersProperty = serializedObject.FindProperty("m_SortingLayers");
            var sortingLayers = new List<string>();
            for (int i = 0; i < sortingLayersProperty.arraySize; i++)
            {
                var layer = sortingLayersProperty.GetArrayElementAtIndex(i);
                var name = layer.FindPropertyRelative("name").stringValue;
                sortingLayers.Add(name);
            }
            data.sortingLayers = sortingLayers.ToArray();
        }

        private void CollectURPSettings(URPSettingsData data)
        {
            var pipeline = GraphicsSettings.defaultRenderPipeline;
            data.isURPActive = pipeline != null && pipeline.GetType().Name.Contains("Universal");

            if (data.isURPActive && pipeline)
            {
                data.urpAssetPath = AssetDatabase.GetAssetPath(pipeline);

                // Collect basic URP asset data via reflection
                var urpType = pipeline.GetType();
                var fields = urpType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (var field in fields)
                {
                    try
                    {
                        if (field.FieldType.IsPrimitive || field.FieldType == typeof(string) || field.FieldType.IsEnum)
                        {
                            var value = field.GetValue(pipeline);
                            data.urpAssetData[field.Name] = value;
                        }
                    }
                    catch { /* Ignore inaccessible fields */ }
                }
            }
        }

        private void Log(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[ProjectSettingsExporter] {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"[ProjectSettingsExporter] {message}");
        }
    }
}
#endif