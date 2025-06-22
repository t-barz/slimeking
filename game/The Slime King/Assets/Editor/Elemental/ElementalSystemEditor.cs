using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TheSlimeKing.Core.Elemental;

namespace TheSlimeKing.Editor.Elemental
{
    /// <summary>
    /// Ferramenta de Editor para gerenciar e testar aspectos do sistema elemental
    /// </summary>
    public class ElementalSystemEditor : EditorWindow
    {
        private ElementalEnergyManager _energyManager;
        private ElementalFragmentSpawner _fragmentSpawner;
        private ElementalAbilityManager _abilityManager;

        private Vector2 _scrollPosition;
        private bool _showEnergySection = true;
        private bool _showFragmentSection = true;
        private bool _showAbilitySection = true;

        // Configurações de teste
        private ElementalType _selectedType = ElementalType.Earth;
        private int _energyAmount = 10;
        private ElementalFragment.FragmentSize _selectedSize = ElementalFragment.FragmentSize.Small;
        private int _fragmentCount = 3;

        [MenuItem("The Slime King/Ferramentas/Editor de Sistema Elemental", false, 30)]
        public static void ShowWindow()
        {
            ElementalSystemEditor window = GetWindow<ElementalSystemEditor>("Sistema Elemental");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnEnable()
        {
            // Tenta encontrar componentes na cena
            FindElementalComponents();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // Cabeçalho
            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            headerStyle.fontSize = 16;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.margin = new RectOffset(0, 0, 10, 10);

            EditorGUILayout.LabelField("Editor do Sistema Elemental", headerStyle);
            EditorGUILayout.Space(10);

            // Botão para localizar componentes
            if (GUILayout.Button("Localizar Componentes Elementais na Cena"))
            {
                FindElementalComponents();
            }

            // Área de rolagem
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Seção de Energy Manager
            _showEnergySection = EditorGUILayout.Foldout(_showEnergySection, "Gerenciador de Energia Elemental", true);

            if (_showEnergySection)
            {
                EditorGUI.indentLevel++;

                // Status do Gerenciador
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);

                if (_energyManager != null)
                {
                    EditorGUILayout.LabelField($"Terra: {GetElementalEnergy(ElementalType.Earth)}");
                    EditorGUILayout.LabelField($"Água: {GetElementalEnergy(ElementalType.Water)}");
                    EditorGUILayout.LabelField($"Fogo: {GetElementalEnergy(ElementalType.Fire)}");
                    EditorGUILayout.LabelField($"Ar: {GetElementalEnergy(ElementalType.Air)}");

                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField($"Energia Total: {GetTotalEnergy()}");
                    EditorGUILayout.LabelField($"Estágio Atual: {GetCurrentStage()}");

                    if (GetCurrentStage() < 3) // Se não está no último estágio
                    {
                        EditorGUILayout.LabelField($"Próxima Evolução: {GetEnergyForNextStage()} pontos");
                        EditorGUILayout.LabelField($"Progresso: {GetProgressToNextStage():P1}");
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("ElementalEnergyManager não encontrado na cena.", MessageType.Warning);
                }

                EditorGUILayout.EndVertical();

                // Controles de teste
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Adicionar Energia (Teste)", EditorStyles.boldLabel);

                _selectedType = (ElementalType)EditorGUILayout.EnumPopup("Tipo Elemental:", _selectedType);
                _energyAmount = EditorGUILayout.IntSlider("Quantidade:", _energyAmount, 1, 100);

                if (GUILayout.Button("Adicionar Energia"))
                {
                    if (_energyManager != null)
                    {
                        AddEnergyInPlayMode(_selectedType, _energyAmount);
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // Seção de Fragment Spawner
            _showFragmentSection = EditorGUILayout.Foldout(_showFragmentSection, "Gerador de Fragmentos", true);

            if (_showFragmentSection)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (_fragmentSpawner != null)
                {
                    EditorGUILayout.LabelField("Spawnar Fragmentos (Teste)", EditorStyles.boldLabel);

                    _selectedType = (ElementalType)EditorGUILayout.EnumPopup("Tipo Elemental:", _selectedType);
                    _selectedSize = (ElementalFragment.FragmentSize)EditorGUILayout.EnumPopup("Tamanho:", _selectedSize);
                    _fragmentCount = EditorGUILayout.IntSlider("Quantidade:", _fragmentCount, 1, 10);

                    if (GUILayout.Button("Spawnar Fragmentos"))
                    {
                        SpawnFragmentsInPlayMode(_selectedType, _selectedSize, _fragmentCount);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("ElementalFragmentSpawner não encontrado na cena.", MessageType.Warning);
                }

                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // Seção de Ability Manager
            _showAbilitySection = EditorGUILayout.Foldout(_showAbilitySection, "Gerenciador de Habilidades", true);

            if (_showAbilitySection)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                if (_abilityManager != null)
                {
                    // Informação sobre habilidades disponíveis seria adicionada aqui
                    EditorGUILayout.LabelField("Sistema pronto para uso", EditorStyles.boldLabel);
                }
                else
                {
                    EditorGUILayout.HelpBox("ElementalAbilityManager não encontrado na cena.", MessageType.Warning);
                }

                EditorGUILayout.EndVertical();

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space(10);

            // Verificação de problemas
            CheckForIssues();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Localiza os componentes do sistema elemental na cena
        /// </summary>
        private void FindElementalComponents()
        {
            _energyManager = FindObjectOfType<ElementalEnergyManager>();
            _fragmentSpawner = FindObjectOfType<ElementalFragmentSpawner>();
            _abilityManager = FindObjectOfType<ElementalAbilityManager>();
        }

        /// <summary>
        /// Adiciona energia em modo Play (segurança para não modificar em edição)
        /// </summary>
        private void AddEnergyInPlayMode(ElementalType type, int amount)
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Modo Play Necessário",
                    "Esta ação só pode ser executada com o jogo em execução (Play Mode).", "OK");
                return;
            }

            _energyManager.AddElementalEnergy(type, amount);
            Debug.Log($"Adicionado {amount} pontos de energia {type}");
        }

        /// <summary>
        /// Spawna fragmentos em modo Play
        /// </summary>
        private void SpawnFragmentsInPlayMode(ElementalType type, ElementalFragment.FragmentSize size, int count)
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Modo Play Necessário",
                    "Esta ação só pode ser executada com o jogo em execução (Play Mode).", "OK");
                return;
            }

            // Configuração de spawn
            ElementalFragmentSpawner.SpawnConfig config = new ElementalFragmentSpawner.SpawnConfig
            {
                elementType = type,
                minAmount = count,
                maxAmount = count,
                randomElement = false
            };

            config.allowedSizes = new List<ElementalFragment.FragmentSize> { size };

            // Position to spawn - at scene view camera position
            Vector3 spawnPosition = SceneView.lastActiveSceneView != null
                ? SceneView.lastActiveSceneView.camera.transform.position
                : Vector3.zero;

            _fragmentSpawner.SpawnFragments(spawnPosition, config);
            Debug.Log($"Spawned {count} fragmentos {size} do tipo {type}");
        }

        /// <summary>
        /// Verifica potenciais problemas na configuração
        /// </summary>
        private void CheckForIssues()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Diagnóstico", EditorStyles.boldLabel);

            if (_energyManager == null)
            {
                EditorGUILayout.HelpBox("ElementalEnergyManager não encontrado na cena. " +
                    "Adicione este componente para gerenciar energia elemental.", MessageType.Error);
            }

            if (_fragmentSpawner == null)
            {
                EditorGUILayout.HelpBox("ElementalFragmentSpawner não encontrado na cena. " +
                    "Adicione este componente para gerar fragmentos elementais.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
        }

        // Helpers para acessar informações do ElementalEnergyManager
        private int GetElementalEnergy(ElementalType type)
        {
            if (_energyManager == null || !Application.isPlaying)
                return 0;

            return _energyManager.GetElementalEnergy(type);
        }

        private int GetTotalEnergy()
        {
            if (_energyManager == null || !Application.isPlaying)
                return 0;

            return _energyManager.GetTotalAbsorbedEnergy();
        }

        private int GetCurrentStage()
        {
            if (_energyManager == null || !Application.isPlaying)
                return 0;

            return _energyManager.GetCurrentGrowthStage();
        }

        private int GetEnergyForNextStage()
        {
            if (_energyManager == null || !Application.isPlaying)
                return 0;

            return _energyManager.GetEnergyForNextStage();
        }

        private float GetProgressToNextStage()
        {
            if (_energyManager == null || !Application.isPlaying)
                return 0;

            return _energyManager.GetProgressToNextStage();
        }
    }
}
