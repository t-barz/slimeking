using UnityEngine;
using SlimeKing.Core;
using SlimeKing.UI;

namespace SlimeKing.Systems.Debug
{
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

        private void Start()
        {
            if (runOnStart)
            {
                RunDiagnostics();
            }

            if (continuousMonitoring)
            {
                InvokeRepeating(nameof(RunDiagnostics), monitoringInterval, monitoringInterval);
            }
        }

        [ContextMenu("Run Diagnostics")]
        public void RunDiagnostics()
        {
            // 1. Verificar GameManager
            if (!GameManager.HasInstance)
            {
                return;
            }

            // 2. Verificar contadores de cristais
            var allCounts = GameManager.Instance.GetAllCrystalCounts();

            // 3. Verificar UI
            var crystalCounterUI = FindFirstObjectByType<CrystalCounterUI>();
        }

        [ContextMenu("Test Crystal Addition")]
        public void TestCrystalAddition()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.AddCrystal(CrystalType.Nature, 1);
            }
        }
    }
}