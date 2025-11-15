using UnityEngine;
using UnityEditor;
using TheSlimeKing.Quest;
using System.Collections.Generic;

namespace TheSlimeKing.Editor.Quest
{
    /// <summary>
    /// Custom Inspector para QuestManager com ferramentas de debug
    /// </summary>
    [CustomEditor(typeof(QuestManager))]
    public class QuestManagerEditor : UnityEditor.Editor
    {
        private QuestManager questManager;
        
        private void OnEnable()
        {
            questManager = (QuestManager)target;
        }
        
        public override void OnInspectorGUI()
        {
            // Desenha Inspector padrão
            DrawDefaultInspector();
            
            // Adiciona espaço
            EditorGUILayout.Space(20);
            
            // Seção de Debug Tools
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Ferramentas de debug para testar o Quest System no Editor.", MessageType.Info);
            
            EditorGUILayout.Space(10);
            
            // Verifica se está em Play Mode
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Debug tools só funcionam em Play Mode.", MessageType.Warning);
                return;
            }
            
            // Botão Clear All Quests
            if (GUILayout.Button("Clear All Quests", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Clear All Quests", 
                    "Tem certeza que deseja limpar todas as quests ativas e completadas?", 
                    "Sim", "Cancelar"))
                {
                    questManager.ClearAllQuests();
                    Debug.Log("[QuestManagerEditor] Todas as quests foram limpas.");
                }
            }
            
            EditorGUILayout.Space(10);
            
            // Lista de Quests Ativas
            DrawActiveQuestsList();
        }
        
        /// <summary>
        /// Desenha lista de quests ativas com botões de debug
        /// </summary>
        private void DrawActiveQuestsList()
        {
            List<QuestProgress> activeQuests = questManager.GetActiveQuests();
            
            if (activeQuests == null || activeQuests.Count == 0)
            {
                EditorGUILayout.HelpBox("Nenhuma quest ativa no momento.", MessageType.Info);
                return;
            }
            
            EditorGUILayout.LabelField($"Active Quests ({activeQuests.Count})", EditorStyles.boldLabel);
            
            foreach (QuestProgress progress in activeQuests)
            {
                if (progress == null || progress.questData == null)
                    continue;
                
                EditorGUILayout.BeginVertical("box");
                
                // Nome da quest e progresso
                EditorGUILayout.LabelField(progress.questData.questName, EditorStyles.boldLabel);
                EditorGUILayout.LabelField($"ID: {progress.questID}");
                EditorGUILayout.LabelField($"Progress: {progress.currentProgress}/{progress.targetProgress}");
                
                // Barra de progresso
                float progressPercentage = progress.GetProgressPercentage();
                EditorGUI.ProgressBar(
                    EditorGUILayout.GetControlRect(false, 20), 
                    progressPercentage, 
                    $"{Mathf.RoundToInt(progressPercentage * 100)}%"
                );
                
                EditorGUILayout.Space(5);
                
                // Status
                string status = progress.isReadyToTurnIn ? "Ready to Turn In" : "In Progress";
                Color statusColor = progress.isReadyToTurnIn ? Color.green : Color.yellow;
                
                GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
                statusStyle.normal.textColor = statusColor;
                statusStyle.fontStyle = FontStyle.Bold;
                
                EditorGUILayout.LabelField($"Status: {status}", statusStyle);
                
                EditorGUILayout.Space(5);
                
                // Botões de ação
                EditorGUILayout.BeginHorizontal();
                
                // Botão Complete
                GUI.enabled = !progress.isReadyToTurnIn;
                if (GUILayout.Button("Force Complete", GUILayout.Height(25)))
                {
                    questManager.ForceCompleteQuest(progress.questID);
                    Debug.Log($"[QuestManagerEditor] Quest '{progress.questData.questName}' foi forçada a completar.");
                }
                GUI.enabled = true;
                
                // Botão Reset
                if (GUILayout.Button("Reset Progress", GUILayout.Height(25)))
                {
                    if (EditorUtility.DisplayDialog("Reset Quest", 
                        $"Resetar progresso da quest '{progress.questData.questName}'?", 
                        "Sim", "Cancelar"))
                    {
                        questManager.ResetQuest(progress.questID);
                        Debug.Log($"[QuestManagerEditor] Quest '{progress.questData.questName}' foi resetada.");
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
        }
    }
}
