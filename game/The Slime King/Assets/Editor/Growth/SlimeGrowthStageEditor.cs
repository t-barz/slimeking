using UnityEngine;
using UnityEditor;

namespace TheSlimeKing.Core.Editor
{
    [CustomEditor(typeof(SlimeGrowthStage))]
    public class SlimeGrowthStageEditor : UnityEditor.Editor
    {
        SerializedProperty stageType;
        SerializedProperty stageName;
        SerializedProperty description;
        SerializedProperty requiredElementalEnergy;
        SerializedProperty baseHealth;
        SerializedProperty baseAttack;
        SerializedProperty baseSpecial;
        SerializedProperty baseDefense;
        SerializedProperty sizeMultiplier;
        SerializedProperty speedMultiplier;
        SerializedProperty inventorySlots;
        SerializedProperty canSqueeze;
        SerializedProperty canBounce;
        SerializedProperty canClimb;
        SerializedProperty canUsePowerfulAbilities;
        SerializedProperty slimeSprite;
        SerializedProperty animatorController;
        SerializedProperty growthEffect;
        SerializedProperty growthSound;

        void OnEnable()
        {
            stageType = serializedObject.FindProperty("stageType");
            stageName = serializedObject.FindProperty("stageName");
            description = serializedObject.FindProperty("description");
            requiredElementalEnergy = serializedObject.FindProperty("requiredElementalEnergy");
            baseHealth = serializedObject.FindProperty("baseHealth");
            baseAttack = serializedObject.FindProperty("baseAttack");
            baseSpecial = serializedObject.FindProperty("baseSpecial");
            baseDefense = serializedObject.FindProperty("baseDefense");
            sizeMultiplier = serializedObject.FindProperty("sizeMultiplier");
            speedMultiplier = serializedObject.FindProperty("speedMultiplier");
            inventorySlots = serializedObject.FindProperty("inventorySlots");
            canSqueeze = serializedObject.FindProperty("canSqueeze");
            canBounce = serializedObject.FindProperty("canBounce");
            canClimb = serializedObject.FindProperty("canClimb");
            canUsePowerfulAbilities = serializedObject.FindProperty("canUsePowerfulAbilities");
            slimeSprite = serializedObject.FindProperty("slimeSprite");
            animatorController = serializedObject.FindProperty("animatorController");
            growthEffect = serializedObject.FindProperty("growthEffect");
            growthSound = serializedObject.FindProperty("growthSound");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Configuração de Estágio do Slime", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Define as características de um estágio de crescimento específico.");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Informações básicas
            EditorGUILayout.LabelField("Informações Básicas", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(stageType);
            EditorGUILayout.PropertyField(stageName);
            EditorGUILayout.PropertyField(description);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(requiredElementalEnergy, new GUIContent("Energia Elemental Necessária"));

            // Estágio atual e sugestões
            SlimeGrowthStage stage = (SlimeGrowthStage)target;
            SlimeStage currentStage = stage.StageType;

            string suggestedSlots = "1";
            string suggestedEnergy = "0";
            string suggestedSize = "0.5";

            switch (currentStage)
            {
                case SlimeStage.Baby:
                    suggestedSlots = "1";
                    suggestedEnergy = "0";
                    suggestedSize = "0.5";
                    break;
                case SlimeStage.Young:
                    suggestedSlots = "2";
                    suggestedEnergy = "200";
                    suggestedSize = "1.0";
                    break;
                case SlimeStage.Adult:
                    suggestedSlots = "3";
                    suggestedEnergy = "600";
                    suggestedSize = "1.5";
                    break;
                case SlimeStage.King:
                    suggestedSlots = "4";
                    suggestedEnergy = "1200";
                    suggestedSize = "2.0";
                    break;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Valores sugeridos para {currentStage}:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Energia: {suggestedEnergy} | Slots: {suggestedSlots} | Tamanho: {suggestedSize}x");
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Atributos base
            EditorGUILayout.LabelField("Atributos Base", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(baseHealth);
            EditorGUILayout.PropertyField(baseAttack);
            EditorGUILayout.PropertyField(baseSpecial);
            EditorGUILayout.PropertyField(baseDefense);
            EditorGUILayout.PropertyField(sizeMultiplier, new GUIContent("Multiplicador de Tamanho"));
            EditorGUILayout.PropertyField(speedMultiplier, new GUIContent("Multiplicador de Velocidade"));

            EditorGUILayout.Space();

            // Funcionalidades
            EditorGUILayout.LabelField("Funcionalidades", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(inventorySlots, new GUIContent("Slots de Inventário"));

            EditorGUILayout.Space();

            // Verificador de acordo com a especificação GDD
            bool isInventorySlotsCorrect = false;
            switch (currentStage)
            {
                case SlimeStage.Baby:
                    isInventorySlotsCorrect = inventorySlots.intValue == 1;
                    break;
                case SlimeStage.Young:
                    isInventorySlotsCorrect = inventorySlots.intValue == 2;
                    break;
                case SlimeStage.Adult:
                    isInventorySlotsCorrect = inventorySlots.intValue == 3;
                    break;
                case SlimeStage.King:
                    isInventorySlotsCorrect = inventorySlots.intValue == 4;
                    break;
            }

            if (!isInventorySlotsCorrect)
            {
                EditorGUILayout.HelpBox($"De acordo com o GDD, o estágio {currentStage} deveria ter {suggestedSlots} slots de inventário.", MessageType.Warning);
            }

            // Habilidades
            EditorGUILayout.PropertyField(canSqueeze, new GUIContent("Pode Passar por Espaços Pequenos"));
            EditorGUILayout.PropertyField(canBounce, new GUIContent("Pode Pular/Quicar"));
            EditorGUILayout.PropertyField(canClimb, new GUIContent("Pode Escalar"));
            EditorGUILayout.PropertyField(canUsePowerfulAbilities, new GUIContent("Pode Usar Habilidades Poderosas"));

            EditorGUILayout.Space();

            // Visualização
            EditorGUILayout.LabelField("Visualização", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(slimeSprite);
            EditorGUILayout.PropertyField(animatorController);
            EditorGUILayout.PropertyField(growthEffect);
            EditorGUILayout.PropertyField(growthSound);

            if (slimeSprite.objectReferenceValue != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Texture2D spriteTexture = AssetPreview.GetAssetPreview(slimeSprite.objectReferenceValue);
                if (spriteTexture != null)
                {
                    GUILayout.Label(spriteTexture, GUILayout.Width(128), GUILayout.Height(128));
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
