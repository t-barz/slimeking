using UnityEngine;
using UnityEditor;

/// <summary>
/// Menu de ferramentas para desenvolvimento que permite resetar dados de salvamento local.
/// Aparece em Unity Editor menu: Extras > Local Save Reset
/// </summary>
public static class LocalSaveResetMenu
{
    #region Menu Items
    /// <summary>
    /// Reseta todos os dados de salvamento (auto-save + slots)
    /// </summary>
    [MenuItem("Extras/Local Save Reset/Reset All Save Data", false, 1)]
    public static void ResetAllSaveData()
    {
        if (EditorUtility.DisplayDialog(
            "Reset All Save Data",
            "Esta ação irá deletar TODOS os dados de salvamento:\n\n" +
            "• Auto-Save\n" +
            "• Slot 0, 1 e 2\n" +
            "• Pontos de Respawn\n" +
            "• Coordenadas de Portal\n\n" +
            "Esta ação não pode ser desfeita!",
            "Deletar Tudo",
            "Cancelar"))
        {
            ResetAutoSave();
            ResetAllSlots();
            ResetRespawnData();
            ResetPortalData();

            Debug.Log("[LocalSaveResetMenu] Todos os dados de salvamento foram resetados!");
            EditorUtility.DisplayDialog("Sucesso", "Todos os dados de salvamento foram deletados!", "OK");
        }
    }

    /// <summary>
    /// Reseta apenas o auto-save
    /// </summary>
    [MenuItem("Extras/Local Save Reset/Reset Auto-Save Only", false, 2)]
    public static void ResetAutoSaveOnly()
    {
        if (EditorUtility.DisplayDialog(
            "Reset Auto-Save",
            "Esta ação irá deletar apenas o Auto-Save.\n\nOs slots manuais (0, 1, 2) serão preservados.",
            "Deletar Auto-Save",
            "Cancelar"))
        {
            ResetAutoSave();

            Debug.Log("[LocalSaveResetMenu] Auto-Save resetado!");
            EditorUtility.DisplayDialog("Sucesso", "Auto-Save foi deletado!", "OK");
        }
    }

    /// <summary>
    /// Reseta apenas os slots manuais (0, 1, 2)
    /// </summary>
    [MenuItem("Extras/Local Save Reset/Reset Manual Slots Only", false, 3)]
    public static void ResetManualSlotsOnly()
    {
        if (EditorUtility.DisplayDialog(
            "Reset Manual Slots",
            "Esta ação irá deletar os slots manuais (0, 1, 2).\n\nO Auto-Save será preservado.",
            "Deletar Slots",
            "Cancelar"))
        {
            ResetAllSlots();

            Debug.Log("[LocalSaveResetMenu] Slots manuais resetados!");
            EditorUtility.DisplayDialog("Sucesso", "Slots manuais foram deletados!", "OK");
        }
    }

    /// <summary>
    /// Reseta dados de respawn
    /// </summary>
    [MenuItem("Extras/Local Save Reset/Reset Respawn Data", false, 4)]
    public static void ResetRespawnDataOnly()
    {
        if (EditorUtility.DisplayDialog(
            "Reset Respawn Data",
            "Esta ação irá deletar apenas os dados de ponto de respawn.",
            "Deletar Respawn",
            "Cancelar"))
        {
            ResetRespawnData();

            Debug.Log("[LocalSaveResetMenu] Dados de respawn resetados!");
            EditorUtility.DisplayDialog("Sucesso", "Dados de respawn foram deletados!", "OK");
        }
    }

    /// <summary>
    /// Reseta coordenadas de portal
    /// </summary>
    [MenuItem("Extras/Local Save Reset/Reset Portal Data", false, 5)]
    public static void ResetPortalDataOnly()
    {
        if (EditorUtility.DisplayDialog(
            "Reset Portal Data",
            "Esta ação irá deletar coordenadas de portal temporárias.",
            "Deletar Portal Data",
            "Cancelar"))
        {
            ResetPortalData();

            Debug.Log("[LocalSaveResetMenu] Dados de portal resetados!");
            EditorUtility.DisplayDialog("Sucesso", "Dados de portal foram deletados!", "OK");
        }
    }

    [MenuItem("Extras/Local Save Reset/", false, 100)]
    public static void Separator1() { }

    /// <summary>
    /// Define a posição atual do jogador como ponto de respawn
    /// </summary>
    [MenuItem("Extras/Local Save Reset/Set Current Position As Respawn", false, 6)]
    public static void SetCurrentPositionAsRespawn()
    {
        if (Application.isPlaying)
        {
            SavePlayerManager.Instance?.SetCurrentPositionAsRespawn();
        }
        else
        {
            EditorUtility.DisplayDialog("Aviso",
                "Esta função só funciona durante o modo Play!", "OK");
        }
    }

    [MenuItem("Extras/Local Save Reset/", false, 101)]
    public static void Separator1_5() { }

    /// <summary>
    /// Mostra informações sobre os saves existentes
    /// </summary>
    [MenuItem("Extras/Local Save Reset/Show Save Info", false, 101)]
    public static void ShowSaveInfo()
    {
        string info = "=== INFORMAÇÕES DE SALVAMENTO ===\n\n";

        // Auto-Save
        if (PlayerPrefs.HasKey("AutoSave_Data"))
        {
            info += "✅ Auto-Save: EXISTE\n";
            if (PlayerPrefs.HasKey("AutoSave_Timestamp"))
            {
                long timestamp = System.Convert.ToInt64(PlayerPrefs.GetString("AutoSave_Timestamp"));
                System.DateTime saveTime = System.DateTime.FromBinary(timestamp);
                info += $"   Data: {saveTime:dd/MM/yyyy HH:mm:ss}\n";
            }
        }
        else
        {
            info += "❌ Auto-Save: VAZIO\n";
        }

        info += "\n";

        // Slots manuais
        for (int i = 0; i < 3; i++)
        {
            string slotKey = $"SaveSlot_{i}_Data";
            if (PlayerPrefs.HasKey(slotKey))
            {
                info += $"✅ Slot {i}: EXISTE\n";
                string timestampKey = $"SaveSlot_{i}_Timestamp";
                if (PlayerPrefs.HasKey(timestampKey))
                {
                    long timestamp = System.Convert.ToInt64(PlayerPrefs.GetString(timestampKey));
                    System.DateTime saveTime = System.DateTime.FromBinary(timestamp);
                    info += $"   Data: {saveTime:dd/MM/yyyy HH:mm:ss}\n";
                }
            }
            else
            {
                info += $"❌ Slot {i}: VAZIO\n";
            }
        }

        info += "\n";

        // Respawn
        if (PlayerPrefs.HasKey("RespawnX"))
        {
            float x = PlayerPrefs.GetFloat("RespawnX");
            float y = PlayerPrefs.GetFloat("RespawnY");
            float z = PlayerPrefs.GetFloat("RespawnZ");
            string scene = PlayerPrefs.GetString("RespawnScene", "N/A");
            info += $"✅ Respawn: ({x:F2}, {y:F2}, {z:F2}) na cena '{scene}'\n";
        }
        else
        {
            info += "❌ Respawn: NÃO DEFINIDO\n";
        }

        // Portal
        if (PlayerPrefs.HasKey("PortalDestinationX"))
        {
            float x = PlayerPrefs.GetFloat("PortalDestinationX");
            float y = PlayerPrefs.GetFloat("PortalDestinationY");
            info += $"✅ Portal: ({x:F2}, {y:F2})\n";
        }
        else
        {
            info += "❌ Portal: NÃO DEFINIDO\n";
        }

        Debug.Log($"[LocalSaveResetMenu]\n{info}");
        EditorUtility.DisplayDialog("Informações de Salvamento", info, "OK");
    }

    [MenuItem("Extras/Local Save Reset/", false, 200)]
    public static void Separator2() { }

    /// <summary>
    /// Deleta TODOS os PlayerPrefs (CUIDADO!)
    /// </summary>
    [MenuItem("Extras/Local Save Reset/DANGER - Delete ALL PlayerPrefs", false, 201)]
    public static void DeleteAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog(
            "⚠️ DANGER ZONE ⚠️",
            "ATENÇÃO: Esta ação irá deletar TODOS os PlayerPrefs!\n\n" +
            "Isso inclui:\n" +
            "• Todos os saves do jogo\n" +
            "• Configurações de áudio/vídeo\n" +
            "• Preferências do usuário\n" +
            "• Qualquer outro dado salvo\n\n" +
            "ESTA AÇÃO NÃO PODE SER DESFEITA!\n\n" +
            "Use apenas em casos extremos!",
            "SIM, DELETAR TUDO",
            "CANCELAR"))
        {
            if (EditorUtility.DisplayDialog(
                "Confirmação Final",
                "Tem certeza ABSOLUTA?\n\nTodos os PlayerPrefs serão perdidos PERMANENTEMENTE!",
                "Deletar",
                "Cancelar"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                Debug.LogWarning("[LocalSaveResetMenu] TODOS os PlayerPrefs foram deletados!");
                EditorUtility.DisplayDialog("Concluído", "TODOS os PlayerPrefs foram deletados!", "OK");
            }
        }
    }
    #endregion

    #region Helper Methods
    /// <summary>
    /// Reseta apenas o auto-save
    /// </summary>
    private static void ResetAutoSave()
    {
        PlayerPrefs.DeleteKey("AutoSave_Data");
        PlayerPrefs.DeleteKey("AutoSave_Timestamp");
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Reseta todos os slots manuais (0, 1, 2)
    /// </summary>
    private static void ResetAllSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.DeleteKey($"SaveSlot_{i}_Data");
            PlayerPrefs.DeleteKey($"SaveSlot_{i}_Timestamp");
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Reseta dados de respawn
    /// </summary>
    private static void ResetRespawnData()
    {
        PlayerPrefs.DeleteKey("RespawnX");
        PlayerPrefs.DeleteKey("RespawnY");
        PlayerPrefs.DeleteKey("RespawnZ");
        PlayerPrefs.DeleteKey("RespawnScene");
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Reseta coordenadas de portal
    /// </summary>
    private static void ResetPortalData()
    {
        PlayerPrefs.DeleteKey("PortalDestinationX");
        PlayerPrefs.DeleteKey("PortalDestinationY");
        PlayerPrefs.Save();
    }
    #endregion
}
