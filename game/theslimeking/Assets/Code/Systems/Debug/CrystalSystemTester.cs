using UnityEngine;
using SlimeKing.Core;

namespace SlimeKing.Debug
{
    /// <summary>
    /// Script de debug para testar manualmente o sistema de cristais.
    /// Adicione ao GameObject para ter botões de teste no Inspector.
    /// </summary>
    public class CrystalSystemTester : MonoBehaviour
    {
        [Header("Crystal Testing")]
        [Tooltip("Tipo de cristal para testar")]
        public CrystalType testCrystalType = CrystalType.Nature;

        [Tooltip("Quantidade a adicionar no teste")]
        [Range(1, 10)]
        public int testAmount = 1;

        [Header("Debug Info")]
        [SerializeField] private bool showDebugInfo = true;

        #region Manual Testing

        [ContextMenu("Test Add Crystal")]
        public void TestAddCrystal()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("[CrystalSystemTester] Só funciona durante Play Mode");
                return;
            }

            if (GameManager.HasInstance)
            {
                GameManager.Instance.AddCrystal(testCrystalType, testAmount);
                UnityEngine.Debug.Log($"[CrystalSystemTester] Teste executado: +{testAmount} {testCrystalType}");
            }
            else
            {
                UnityEngine.Debug.LogError("[CrystalSystemTester] GameManager não encontrado!");
            }
        }

        [ContextMenu("Test Add All Crystal Types")]
        public void TestAddAllCrystalTypes()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("[CrystalSystemTester] Só funciona durante Play Mode");
                return;
            }

            if (GameManager.HasInstance)
            {
                foreach (CrystalType crystalType in System.Enum.GetValues(typeof(CrystalType)))
                {
                    GameManager.Instance.AddCrystal(crystalType, 1);
                }
                UnityEngine.Debug.Log("[CrystalSystemTester] Teste executado: +1 para todos os tipos de cristais");
            }
            else
            {
                UnityEngine.Debug.LogError("[CrystalSystemTester] GameManager não encontrado!");
            }
        }

        [ContextMenu("Show Crystal Counts")]
        public void ShowCrystalCounts()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("[CrystalSystemTester] Só funciona durante Play Mode");
                return;
            }

            if (GameManager.HasInstance)
            {
                var counts = GameManager.Instance.GetAllCrystalCounts();
                string report = "[CrystalSystemTester] Contadores atuais:\n";

                foreach (var kvp in counts)
                {
                    report += $"  {kvp.Key}: {kvp.Value}\n";
                }

                report += $"Total: {GameManager.Instance.GetTotalCrystalCount()}";
                UnityEngine.Debug.Log(report);
            }
            else
            {
                UnityEngine.Debug.LogError("[CrystalSystemTester] GameManager não encontrado!");
            }
        }

        [ContextMenu("Check UI Connection")]
        public void CheckUIConnection()
        {
            // Procura CrystalCounterUI na cena
            var crystalCounterUI = FindObjectOfType<SlimeKing.UI.CrystalCounterUI>();

            if (crystalCounterUI != null)
            {
                UnityEngine.Debug.Log($"[CrystalSystemTester] ✅ CrystalCounterUI encontrado: {crystalCounterUI.gameObject.name}");
                UnityEngine.Debug.Log($"[CrystalSystemTester] Contadores conectados: {crystalCounterUI.GetConnectedCountersCount()}/6");

                if (crystalCounterUI.AreAllCountersConnected())
                {
                    UnityEngine.Debug.Log("[CrystalSystemTester] ✅ Todos os contadores estão conectados!");
                }
                else
                {
                    UnityEngine.Debug.LogWarning("[CrystalSystemTester] ⚠️ Nem todos os contadores estão conectados!");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("[CrystalSystemTester] ❌ CrystalCounterUI não encontrado na cena!");
                UnityEngine.Debug.LogError("[CrystalSystemTester] SOLUÇÃO: Adicionar componente CrystalCounterUI ao CanvasHUD");
            }
        }

        #endregion

        #region Event Testing

        private void OnEnable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnCrystalCountChanged += OnCrystalCountChanged;
                GameManager.Instance.OnCrystalCollected += OnCrystalCollected;
            }
        }

        private void OnDisable()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnCrystalCountChanged -= OnCrystalCountChanged;
                GameManager.Instance.OnCrystalCollected -= OnCrystalCollected;
            }
        }

        private void OnCrystalCountChanged(CrystalType crystalType, int newCount)
        {
            if (showDebugInfo)
            {
                UnityEngine.Debug.Log($"[CrystalSystemTester] 📊 Evento OnCrystalCountChanged: {crystalType} = {newCount}");
            }
        }

        private void OnCrystalCollected(CrystalType crystalType, int amount)
        {
            if (showDebugInfo)
            {
                UnityEngine.Debug.Log($"[CrystalSystemTester] 💎 Evento OnCrystalCollected: +{amount} {crystalType}");
            }
        }

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            if (showDebugInfo)
            {
                UnityEngine.Debug.Log("[CrystalSystemTester] Iniciado - Use os métodos do menu de contexto para testar");
            }
        }

        #endregion
    }
}