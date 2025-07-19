using UnityEngine;
using UnityEditor;

/// <summary>
/// Exemplo de script que demonstra como integrar o reset de saves em workflows de desenvolvimento.
/// Este script pode ser usado como referência para automatizar testes.
/// </summary>
public static class SaveTestingWorkflow
{
    /// <summary>
    /// Workflow automatizado para testar o sistema de salvamento
    /// </summary>
    [MenuItem("Extras/Testing Workflows/Test Save System", false, 1)]
    public static void TestSaveSystem()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Test Save System",
                "Este teste precisa ser executado durante o Play Mode.\n\n" +
                "1. Entre em Play Mode\n" +
                "2. Execute este comando novamente",
                "OK");
            return;
        }

        Debug.Log("[SaveTestingWorkflow] Iniciando teste do sistema de salvamento...");

        // Verifica se o SavePlayerManager existe
        if (SavePlayerManager.Instance == null)
        {
            Debug.LogError("[SaveTestingWorkflow] SavePlayerManager não encontrado!");
            return;
        }

        // Executa sequência de testes
        var instance = SavePlayerManager.Instance;

        // 1. Salva dados atuais
        Debug.Log("[SaveTestingWorkflow] 1. Salvando estado atual...");
        instance.ForceSave();

        // 2. Salva em slots específicos
        Debug.Log("[SaveTestingWorkflow] 2. Testando slots manuais...");
        instance.SavePlayerData(0);
        instance.SavePlayerData(1);

        // 3. Define ponto de respawn
        Debug.Log("[SaveTestingWorkflow] 3. Definindo ponto de respawn...");
        instance.SetRespawnPoint(Vector3.zero);

        Debug.Log("[SaveTestingWorkflow] Teste concluído! Use 'Show Save Info' para verificar resultados.");
    }

    /// <summary>
    /// Prepara ambiente limpo para testes
    /// </summary>
    [MenuItem("Extras/Testing Workflows/Prepare Clean Test Environment", false, 2)]
    public static void PrepareCleanTestEnvironment()
    {
        if (EditorUtility.DisplayDialog(
            "Prepare Clean Test Environment",
            "Esta ação irá:\n\n" +
            "1. Resetar todos os saves\n" +
            "2. Limpar dados de respawn e portal\n" +
            "3. Preparar ambiente para testes\n\n" +
            "Continuar?",
            "Sim",
            "Cancelar"))
        {
            // Usa os métodos do LocalSaveResetMenu
            LocalSaveResetMenu.ResetAllSaveData();

            Debug.Log("[SaveTestingWorkflow] Ambiente de teste limpo preparado!");

            if (Application.isPlaying)
            {
                Debug.Log("[SaveTestingWorkflow] Agora você pode testar o sistema com dados limpos.");
            }
            else
            {
                Debug.Log("[SaveTestingWorkflow] Entre em Play Mode para testar o sistema.");
            }
        }
    }

    /// <summary>
    /// Gera saves de exemplo para testar UI de carregamento
    /// </summary>
    [MenuItem("Extras/Testing Workflows/Generate Sample Save Data", false, 3)]
    public static void GenerateSampleSaveData()
    {
        if (!Application.isPlaying)
        {
            EditorUtility.DisplayDialog(
                "Generate Sample Save Data",
                "Este comando precisa ser executado durante o Play Mode para acessar o SavePlayerManager.",
                "OK");
            return;
        }

        if (SavePlayerManager.Instance == null)
        {
            Debug.LogError("[SaveTestingWorkflow] SavePlayerManager não encontrado!");
            return;
        }

        var instance = SavePlayerManager.Instance;

        // Gera saves de exemplo com dados diferentes
        Debug.Log("[SaveTestingWorkflow] Gerando dados de exemplo...");

        // Slot 0 - Save "antigo"
        instance.SavePlayerData(0);
        Debug.Log("[SaveTestingWorkflow] Slot 0 criado");

        // Aguarda um pouco e salva slot 1
        System.Threading.Thread.Sleep(1000);
        instance.SavePlayerData(1);
        Debug.Log("[SaveTestingWorkflow] Slot 1 criado");

        // Auto-save
        instance.ForceSave();
        Debug.Log("[SaveTestingWorkflow] Auto-save atualizado");

        Debug.Log("[SaveTestingWorkflow] Dados de exemplo gerados! Use 'Show Save Info' para verificar.");
    }

    [MenuItem("Extras/Testing Workflows/", false, 100)]
    public static void Separator() { }

    /// <summary>
    /// Abre janela com informações rápidas sobre ferramentas de desenvolvimento
    /// </summary>
    [MenuItem("Extras/Testing Workflows/Development Tools Info", false, 101)]
    public static void ShowDevelopmentToolsInfo()
    {
        string info = "=== FERRAMENTAS DE DESENVOLVIMENTO ===\n\n" +
                     "SAVE RESET MENU:\n" +
                     "• Extras > Local Save Reset\n" +
                     "• Reset de dados específicos\n" +
                     "• Informações detalhadas sobre saves\n\n" +

                     "TESTING WORKFLOWS:\n" +
                     "• Test Save System - Testa funcionalidades básicas\n" +
                     "• Prepare Clean Test Environment - Limpa dados\n" +
                     "• Generate Sample Save Data - Cria dados de exemplo\n\n" +

                     "DICAS:\n" +
                     "• Use 'Show Save Info' para debug\n" +
                     "• Sempre teste em Play Mode\n" +
                     "• Limpe dados entre testes diferentes\n" +
                     "• Verifique Console para logs detalhados";

        Debug.Log($"[SaveTestingWorkflow]\n{info}");
        EditorUtility.DisplayDialog("Development Tools Info", info, "OK");
    }
}
