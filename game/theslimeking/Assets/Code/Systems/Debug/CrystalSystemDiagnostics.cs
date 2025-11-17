using UnityEngine;
using SlimeKing.Core;

/// <summary>
/// Script de diagnóstico para o sistema de cristais.
/// Adicione este componente a qualquer GameObject na cena para verificar o status do sistema.
/// </summary>
public class CrystalSystemDiagnostics : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private bool continuousMonitoring = false;
    [SerializeField] private float monitoringInterval = 2f;

    private float lastCheckTime;

    private void Start()
    {
        if (runOnStart)
        {
            RunDiagnostics();
        }
    }

    private void Update()
    {
        if (continuousMonitoring && Time.time - lastCheckTime >= monitoringInterval)
        {
            lastCheckTime = Time.time;
            RunDiagnostics();
        }
    }

    [ContextMenu("Run Diagnostics")]
    public void RunDiagnostics()
    {
        Debug.Log("=== CRYSTAL SYSTEM DIAGNOSTICS ===");

        // 1. Verificar GameManager
        if (!GameManager.HasInstance)
        {
                  
 Debug.LogError("❌ GameManager não encontrado!");
            return;
        }

        Debug.Log("✅ GameManager encontrado");

        // 2. Verificar contadores de cristais
        var allCounts = GameManager.Instance.GetAllCrystalCounts();
        Debug.Log($"📊 Contadores de cristais:");
        foreach (var kvp in allCounts)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value}");
        }

        // 3. Verificar se eventos estão registrados
        var onCrystalCountChangedField = typeof(GameManager).GetField("OnCrystalCountChanged");
        if (onCrystalCountChangedField != null)
        {
            var eventDelegate = onCrystalCountChangedField.GetValue(GameManager.Instance) as System.Delegate;
            if (eventDelegate != null)
            {
                int subscriberCount = eventDelegate.GetInvocationList().Length;
                Debug.Log($"✅ OnCrystalCountChanged tem {subscriberCount} subscriber(s)");
            }
            else
            {
                Debug.LogWarning("⚠️ OnCrystalCountChanged não tem subscribers!");
            }
        }

        // 4. Verificar CrystalCounterUI na cena
        var crystalCounterUI = FindObjectOfType<SlimeKing.UI.CrystalCounterUI>();
        if (crystalCounterUI != null)
        {
            Debug.Log($"✅ CrystalCounterUI encontrado: {crystalCounterUI.gameObject.name}");
            Debug.Log($"  Ativo: {crystalCounterUI.gameObject.activeInHierarchy}");
            Debug.Log($"  Enabled: {crystalCounterUI.enabled}");
            Debug.Log($"  Contadores conectados: {crystalCounterUI.GetConnectedCountersCount()}/6");
        }
        else
        {
            Debug.LogError("❌ CrystalCounterUI não encontrado na cena!");
        }

        Debug.Log("=== END DIAGNOSTICS ===");
    }

    [ContextMenu("Test Add Crystal")]
    public void TestAddCrystal()
    {
        if (GameManager.HasInstance)
        {
            Debug.Log("🧪 Testando adição de cristal Nature...");
            GameManager.Instance.AddCrystal(CrystalType.Nature, 1);
        }
        else
        {
            Debug.LogError("GameManager não disponível!");
        }
    }
}
