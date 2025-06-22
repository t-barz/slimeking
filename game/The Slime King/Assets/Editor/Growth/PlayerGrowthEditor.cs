using UnityEngine;
using UnityEditor;

namespace TheSlimeKing.Core.Editor
{
    [CustomEditor(typeof(PlayerGrowth))]
    public class PlayerGrowthEditor : UnityEditor.Editor
    {
        SerializedProperty growthStages;
        SerializedProperty growthTransitionDuration;
        SerializedProperty growthCurve;
        SerializedProperty defaultGrowthParticles;
        SerializedProperty defaultGrowthSound;
        SerializedProperty screenFlashEffect;
        SerializedProperty slimeTransform;
        SerializedProperty slimeRenderer;
        SerializedProperty slimeAnimator;
        SerializedProperty onGrowthStageChanged;
        SerializedProperty onGrowthStarted;
        SerializedProperty onGrowthCompleted;

        private bool showSettings = true;
        private bool showReferences = true;
        private bool showEvents = true;
        private bool showDebug = false;

        void OnEnable()
        {
            growthStages = serializedObject.FindProperty("_growthStages");
            growthTransitionDuration = serializedObject.FindProperty("_growthTransitionDuration");
            growthCurve = serializedObject.FindProperty("_growthCurve");
            defaultGrowthParticles = serializedObject.FindProperty("_defaultGrowthParticles");
            defaultGrowthSound = serializedObject.FindProperty("_defaultGrowthSound");
            screenFlashEffect = serializedObject.FindProperty("_screenFlashEffect");
            slimeTransform = serializedObject.FindProperty("_slimeTransform");
            slimeRenderer = serializedObject.FindProperty("_slimeRenderer");
            slimeAnimator = serializedObject.FindProperty("_slimeAnimator");
            onGrowthStageChanged = serializedObject.FindProperty("OnGrowthStageChanged");
            onGrowthStarted = serializedObject.FindProperty("OnGrowthStarted");
            onGrowthCompleted = serializedObject.FindProperty("OnGrowthCompleted");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            PlayerGrowth growthSystem = (PlayerGrowth)target;

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Sistema de Crescimento do Slime", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Gerencia as transformações do Slime conforme o acúmulo de energia elemental.");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Exibir estágio atual
            SlimeGrowthStage currentStage = growthSystem.GetCurrentStageConfig();
            if (currentStage != null)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                // Exibir sprite do estágio atual se existir
                GUILayout.BeginVertical(GUILayout.Width(64));
                if (currentStage.SlimeSprite != null)
                {
                    Rect spriteRect = GUILayoutUtility.GetRect(64, 64);
                    GUI.DrawTexture(spriteRect, AssetPreview.GetAssetPreview(currentStage.SlimeSprite));
                }
                else
                {
                    GUILayout.Label("No Sprite", GUILayout.Width(64), GUILayout.Height(64));
                }
                GUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Estágio Atual:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"{currentStage.StageName} ({currentStage.StageType})");
                EditorGUILayout.LabelField($"Energia Requerida: {currentStage.RequiredElementalEnergy}");
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            // Configurações
            showSettings = EditorGUILayout.Foldout(showSettings, "Configurações de Crescimento", true);
            if (showSettings)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(growthStages, new GUIContent("Estágios de Crescimento"), true);
                EditorGUILayout.PropertyField(growthTransitionDuration, new GUIContent("Duração da Transição"));
                EditorGUILayout.PropertyField(growthCurve, new GUIContent("Curva de Crescimento"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Feedback
            EditorGUILayout.LabelField("Feedback Visual e Sonoro", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(defaultGrowthParticles, new GUIContent("Partículas Padrão"));
            EditorGUILayout.PropertyField(defaultGrowthSound, new GUIContent("Som Padrão"));
            EditorGUILayout.PropertyField(screenFlashEffect, new GUIContent("Efeito de Flash"));

            EditorGUILayout.Space();

            // Referências
            showReferences = EditorGUILayout.Foldout(showReferences, "Referências", true);
            if (showReferences)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(slimeTransform, new GUIContent("Transform do Slime"));
                EditorGUILayout.PropertyField(slimeRenderer, new GUIContent("Renderer do Slime"));
                EditorGUILayout.PropertyField(slimeAnimator, new GUIContent("Animator do Slime"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Eventos
            showEvents = EditorGUILayout.Foldout(showEvents, "Eventos", true);
            if (showEvents)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(onGrowthStageChanged, new GUIContent("Ao Mudar Estágio"));
                EditorGUILayout.PropertyField(onGrowthStarted, new GUIContent("Ao Iniciar Crescimento"));
                EditorGUILayout.PropertyField(onGrowthCompleted, new GUIContent("Ao Completar Crescimento"));
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // Debug
            showDebug = EditorGUILayout.Foldout(showDebug, "Ferramentas de Debug", true);
            if (showDebug)
            {
                EditorGUILayout.HelpBox("Use estas opções apenas para fins de teste.", MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                SlimeStage targetStage = (SlimeStage)EditorGUILayout.EnumPopup("Estágio:", growthSystem.GetCurrentStage());

                if (GUILayout.Button("Forçar Crescimento", GUILayout.Width(150)))
                {
                    if (Application.isPlaying)
                    {
                        growthSystem.ForceGrowth(targetStage);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Modo Editor",
                            "Esta função só está disponível durante o jogo.", "OK");
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
