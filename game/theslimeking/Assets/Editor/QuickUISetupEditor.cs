using UnityEngine;
using UnityEditor;
using SlimeKing.UI;

/// <summary>
/// Editor script que adiciona funcionalidade de configuração de UI ao menu Extra Tools do Unity.
/// Fornece acesso fácil às funcionalidades do QuickUISetup através de itens de menu.
/// 
/// Acesso: Menu > Extra Tools > UI Setup
/// </summary>
public static class QuickUISetupEditor
{
    #region Fields
    private static bool enableDebugLogs = false;
    #endregion
    #region Menu Items
    
    /// <summary>
    /// Cria um sistema completo de UI através do menu Extra Tools
    /// </summary>
    [MenuItem("Extra Tools/UI Setup/Setup Complete UI System")]
    public static void SetupCompleteUISystem()
    {
        if (!Application.isPlaying)
        {
            ExecuteUISetup(() => {
                var quickUISetup = GetOrCreateQuickUISetup();
                quickUISetup.SetupCompleteUISystem();
                return "Sistema completo de UI criado com sucesso!";
            });
        }
        else
        {
            Debug.LogWarning("[QuickUISetupEditor] Não é possível configurar UI durante o Play Mode!");
        }
    }
    
    /// <summary>
    /// Cria apenas o canvas principal através do menu Extra Tools
    /// </summary>
    [MenuItem("Extra Tools/UI Setup/Create Main Canvas Only")]
    public static void CreateMainCanvasOnly()
    {
        if (!Application.isPlaying)
        {
            ExecuteUISetup(() => {
                var quickUISetup = GetOrCreateQuickUISetup();
                
                // Usa reflection para chamar o método privado CreateMainCanvas
                var method = typeof(QuickUISetup).GetMethod("CreateMainCanvas", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (method != null)
                {
                    GameObject canvas = (GameObject)method.Invoke(quickUISetup, null);
                    return $"Canvas principal '{canvas.name}' criado com sucesso!";
                }
                else
                {
                    Debug.LogError("[QuickUISetupEditor] Não foi possível encontrar o método CreateMainCanvas!");
                    return "Falha ao criar canvas principal!";
                }
            });
        }
        else
        {
            Debug.LogWarning("[QuickUISetupEditor] Não é possível criar canvas durante o Play Mode!");
        }
    }
    
    /// <summary>
    /// Valida se os itens de menu de configuração de UI devem estar habilitados
    /// </summary>
    [MenuItem("Extra Tools/UI Setup/Setup Complete UI System", true)]
    [MenuItem("Extra Tools/UI Setup/Create Main Canvas Only", true)]
    public static bool ValidateUISetupMenuItems()
    {
        // Só habilita quando não estiver em play mode
        return !Application.isPlaying;
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Mostra informações sobre componentes de UI existentes na cena
    /// </summary>
    [MenuItem("Extra Tools/UI Setup/Show UI System Info")]
    public static void ShowUISystemInfo()
    {
        var canvas = GameObject.Find("GameHUD");
        var uiManager = Object.FindObjectOfType<UISystemManager>();
        
        // Tenta encontrar componentes de UI com e sem namespace
        var healthDisplay = Object.FindObjectOfType(System.Type.GetType("HealthDisplay")) ?? 
                           Object.FindObjectOfType(System.Type.GetType("SlimeKing.UI.HealthDisplay"));
        var fragmentDisplay = Object.FindObjectOfType(System.Type.GetType("FragmentDisplay")) ?? 
                             Object.FindObjectOfType(System.Type.GetType("SlimeKing.UI.FragmentDisplay"));
        var quickSlotsDisplay = Object.FindObjectOfType(System.Type.GetType("QuickSlotsDisplay")) ?? 
                               Object.FindObjectOfType(System.Type.GetType("SlimeKing.UI.QuickSlotsDisplay"));
        
        string info = "=== Informações do Sistema de UI ===\n";
        info += $"Canvas Principal (GameHUD): {(canvas != null ? "✓ Encontrado" : "✗ Não Encontrado")}\n";
        info += $"UISystemManager: {(uiManager != null ? "✓ Encontrado" : "✗ Não Encontrado")}\n";
        info += $"HealthDisplay: {(healthDisplay != null ? "✓ Encontrado" : "✗ Não Encontrado")}\n";
        info += $"FragmentDisplay: {(fragmentDisplay != null ? "✓ Encontrado" : "✗ Não Encontrado")}\n";
        info += $"QuickSlotsDisplay: {(quickSlotsDisplay != null ? "✓ Encontrado" : "✗ Não Encontrado")}\n";
        
        if (canvas != null)
        {
            info += $"\nDetalhes do Canvas:\n";
            info += $"- Posição: {canvas.transform.position}\n";
            info += $"- Número de Filhos: {canvas.transform.childCount}\n";
        }
        
        Debug.Log($"[QuickUISetupEditor] {info}");
        
        // Também mostra em um diálogo para melhor visibilidade
        EditorUtility.DisplayDialog("Informações do Sistema de UI", info, "OK");
    }
    
    /// <summary>
    /// Limpa componentes temporários QuickUISetup na cena
    /// </summary>
    [MenuItem("Extra Tools/UI Setup/Cleanup Temp Components")]
    public static void CleanupTempComponents()
    {
        var tempSetups = Object.FindObjectsOfType<QuickUISetup>();
        int cleanedCount = 0;
        
        foreach (var setup in tempSetups)
        {
            if (setup.gameObject.name.Contains("Temp") || setup.gameObject.name.Contains("temp"))
            {
                Object.DestroyImmediate(setup.gameObject);
                cleanedCount++;
            }
        }
        
        string message = cleanedCount > 0 ? 
            $"Limpeza realizada: {cleanedCount} componente(s) temporário(s) removido(s)" : 
            "Nenhum componente temporário encontrado para limpeza";
            
        Debug.Log($"[QuickUISetupEditor] {message}");
        EditorUtility.DisplayDialog("Limpeza Concluída", message, "OK");
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Obtém componente QuickUISetup existente ou cria um temporário
    /// </summary>
    private static QuickUISetup GetOrCreateQuickUISetup()
    {
        QuickUISetup quickUISetup = Object.FindObjectOfType<QuickUISetup>();
        
        if (quickUISetup == null)
        {
            // Cria um GameObject temporário com componente QuickUISetup
            GameObject tempGO = new GameObject("TempUISetup");
            quickUISetup = tempGO.AddComponent<QuickUISetup>();
            DebugLog("Componente QuickUISetup temporário criado");
        }
        else
        {
            DebugLog("Usando componente QuickUISetup existente");
        }
        
        return quickUISetup;
    }
    
    /// <summary>
    /// Executa operação de configuração de UI com limpeza adequada e marcação de cena
    /// </summary>
    private static void ExecuteUISetup(System.Func<string> setupAction)
    {
        DebugLog("Iniciando configuração de UI");
        
        try
        {
            string result = setupAction();
            Debug.Log($"[QuickUISetupEditor] {result}");
            
            // Marca cena como modificada para garantir que mudanças sejam salvas
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
                
            DebugLog("Cena marcada como modificada");
            
            // Mostra diálogo de sucesso
            EditorUtility.DisplayDialog("Configuração de UI Concluída", result, "OK");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[QuickUISetupEditor] Erro durante configuração de UI: {ex.Message}");
            EditorUtility.DisplayDialog("Erro na Configuração de UI", $"Ocorreu um erro: {ex.Message}", "OK");
        }
        finally
        {
            // Limpa componentes temporários
            CleanupTemporaryComponents();
            DebugLog("Configuração de UI finalizada");
        }
    }
    
    /// <summary>
    /// Limpa componentes temporários criados durante a configuração
    /// </summary>
    private static void CleanupTemporaryComponents()
    {
        var tempSetups = Object.FindObjectsOfType<QuickUISetup>();
        int cleanedCount = 0;
        
        foreach (var setup in tempSetups)
        {
            if (setup.gameObject.name.Contains("Temp"))
            {
                Object.DestroyImmediate(setup.gameObject);
                cleanedCount++;
            }
        }
        
        if (cleanedCount > 0)
        {
            DebugLog($"Removidos {cleanedCount} componente(s) temporário(s)");
        }
    }
    
    /// <summary>
    /// Log de debug condicional
    /// </summary>
    private static void DebugLog(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[QuickUISetupEditor] {message}");
        }
    }
    
    #endregion
}