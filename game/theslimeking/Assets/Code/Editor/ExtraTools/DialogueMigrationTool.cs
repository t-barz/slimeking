using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Ferramenta para migrar e limpar o sistema antigo de diálogo.
    /// </summary>
    public static class DialogueMigrationTool
    {
        /// <summary>
        /// Busca e remove todos os componentes do sistema antigo de diálogo.
        /// </summary>
        public static void MigrateOldDialogueSystem()
        {
            Debug.Log("[DialogueMigrationTool] Iniciando migração do sistema antigo de diálogo...");
            
            StringBuilder report = new StringBuilder();
            report.AppendLine("=== RELATÓRIO DE MIGRAÇÃO DO SISTEMA DE DIÁLOGO ===");
            report.AppendLine($"Data: {System.DateTime.Now}");
            report.AppendLine();
            
            int npcsFound = 0;
            int componentsRemoved = 0;
            List<string> affectedNPCs = new List<string>();
            
            // Buscar todos os GameObjects na cena
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                bool npcModified = false;
                
                // Buscar componentes antigos
                Component[] components = obj.GetComponents<Component>();
                
                foreach (Component component in components)
                {
                    if (component == null) continue;
                    
                    string componentType = component.GetType().Name;
                    
                    // Lista de componentes antigos a remover
                    if (componentType == "NPCDialogueController" ||
                        componentType == "OldDialogueSystem" ||
                        componentType.Contains("OldDialogue"))
                    {
                        Debug.Log($"[DialogueMigrationTool] Removendo {componentType} de {obj.name}");
                        Object.DestroyImmediate(component);
                        componentsRemoved++;
                        npcModified = true;
                    }
                }
                
                if (npcModified)
                {
                    npcsFound++;
                    affectedNPCs.Add(obj.name);
                    EditorUtility.SetDirty(obj);
                }
            }
            
            // Gerar relatório
            report.AppendLine($"NPCs encontrados com sistema antigo: {npcsFound}");
            report.AppendLine($"Componentes removidos: {componentsRemoved}");
            report.AppendLine();
            
            if (affectedNPCs.Count > 0)
            {
                report.AppendLine("NPCs afetados:");
                foreach (string npcName in affectedNPCs)
                {
                    report.AppendLine($"  - {npcName}");
                }
            }
            else
            {
                report.AppendLine("Nenhum componente antigo encontrado.");
            }
            
            report.AppendLine();
            report.AppendLine("=== FIM DO RELATÓRIO ===");
            
            // Salvar relatório
            string reportPath = "Assets/AuxTemp/DialogueMigrationReport.txt";
            System.IO.File.WriteAllText(reportPath, report.ToString());
            AssetDatabase.Refresh();
            
            Debug.Log(report.ToString());
            
            // Mostrar dialog
            if (npcsFound > 0)
            {
                EditorUtility.DisplayDialog("Migração Completa",
                    $"✅ Migração concluída!\n\n" +
                    $"NPCs afetados: {npcsFound}\n" +
                    $"Componentes removidos: {componentsRemoved}\n\n" +
                    $"Relatório salvo em:\n{reportPath}",
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Migração Completa",
                    "✅ Nenhum componente antigo encontrado!\n\n" +
                    "O projeto já está usando o novo sistema de diálogo.",
                    "OK");
            }
        }
        
        /// <summary>
        /// Valida se há referências quebradas no projeto.
        /// </summary>
        public static void ValidateReferences()
        {
            Debug.Log("[DialogueMigrationTool] Validando referências...");
            
            int brokenReferences = 0;
            List<string> objectsWithBrokenRefs = new List<string>();
            
            // Buscar todos os GameObjects na cena
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            
            foreach (GameObject obj in allObjects)
            {
                Component[] components = obj.GetComponents<Component>();
                
                foreach (Component component in components)
                {
                    if (component == null)
                    {
                        brokenReferences++;
                        if (!objectsWithBrokenRefs.Contains(obj.name))
                        {
                            objectsWithBrokenRefs.Add(obj.name);
                        }
                    }
                }
            }
            
            if (brokenReferences > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine($"⚠️ Encontradas {brokenReferences} referências quebradas!");
                message.AppendLine();
                message.AppendLine("GameObjects afetados:");
                foreach (string objName in objectsWithBrokenRefs)
                {
                    message.AppendLine($"  - {objName}");
                }
                
                Debug.LogWarning(message.ToString());
                EditorUtility.DisplayDialog("Referências Quebradas", message.ToString(), "OK");
            }
            else
            {
                Debug.Log("[DialogueMigrationTool] ✅ Nenhuma referência quebrada encontrada!");
                EditorUtility.DisplayDialog("Validação Completa",
                    "✅ Nenhuma referência quebrada encontrada!\n\n" +
                    "O projeto está limpo.",
                    "OK");
            }
        }
    }
}
