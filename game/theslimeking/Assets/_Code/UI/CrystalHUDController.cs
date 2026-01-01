using UnityEngine;
using TMPro;
using SlimeKing.Core;
using System.Collections.Generic;
using UnityDebug = UnityEngine.Debug;

namespace TheSlimeKing.UI
{
    /// <summary>
    /// Controla a atualização visual dos contadores de cristais elementais no HUD.
    /// Escuta eventos do GameManager e atualiza os textos correspondentes.
    /// Segue padrão Controller para controle de UI específica.
    /// </summary>
    public class CrystalHUDController : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Crystal UI References")]
        [SerializeField] private TextMeshProUGUI natureCountText;
        [SerializeField] private TextMeshProUGUI fireCountText;
        [SerializeField] private TextMeshProUGUI waterCountText;
        [SerializeField] private TextMeshProUGUI shadowCountText;
        [SerializeField] private TextMeshProUGUI earthCountText;
        [SerializeField] private TextMeshProUGUI airCountText;

        [Header("Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private string countFormat = "x{0}"; // Formato: "x10"
        #endregion

        #region Private Variables
        // Mapeamento de tipos para textos
        private Dictionary<CrystalType, TextMeshProUGUI> crystalTextMap;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            InitializeCrystalTextMap();
        }

        private void OnEnable()
        {
            // Inscreve no evento do GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCrystalCountChanged += HandleCrystalCountChanged;
                
                // Inicializa contadores com valores atuais
                InitializeCounters();
            }
            else
            {
                Debug.LogWarning("[CrystalHUDController] GameManager.Instance é null no OnEnable");
            }
        }

        private void OnDisable()
        {
            // Desinscreve do evento
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCrystalCountChanged -= HandleCrystalCountChanged;
            }
        }
        #endregion

        #region Initialization
        private void InitializeCrystalTextMap()
        {
            crystalTextMap = new Dictionary<CrystalType, TextMeshProUGUI>
            {
                { CrystalType.Nature, natureCountText },
                { CrystalType.Fire, fireCountText },
                { CrystalType.Water, waterCountText },
                { CrystalType.Shadow, shadowCountText },
                { CrystalType.Earth, earthCountText },
                { CrystalType.Air, airCountText }
            };

            // Valida referências
            foreach (var kvp in crystalTextMap)
            {
                if (kvp.Value == null)
                {
                    Debug.LogError($"[CrystalHUDController] Referência de texto para {kvp.Key} não configurada!");
                }
            }
        }

        private void InitializeCounters()
        {
            // Obtém contadores atuais do GameManager
            var allCounts = GameManager.Instance.GetAllCrystalCounts();

            foreach (var kvp in allCounts)
            {
                UpdateCrystalText(kvp.Key, kvp.Value);
            }

            Log("Contadores de cristais inicializados");
        }
        #endregion

        #region Event Handlers
        private void HandleCrystalCountChanged(CrystalType crystalType, int newCount)
        {
            UpdateCrystalText(crystalType, newCount);
            Log($"Cristal {crystalType} atualizado: {newCount}");
        }
        #endregion

        #region UI Update
        private void UpdateCrystalText(CrystalType crystalType, int count)
        {
            if (!crystalTextMap.TryGetValue(crystalType, out TextMeshProUGUI textComponent))
            {
                Debug.LogWarning($"[CrystalHUDController] Tipo de cristal {crystalType} não mapeado");
                return;
            }

            if (textComponent == null)
            {
                Debug.LogWarning($"[CrystalHUDController] TextComponent para {crystalType} é null");
                return;
            }

            // Formata texto: "x10"
            textComponent.text = string.Format(countFormat, count);
        }
        #endregion

        #region Logging
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[CrystalHUDController] {message}");
            }
        }
        #endregion

        #region Editor Helpers
#if UNITY_EDITOR
        [ContextMenu("Test Update All Counters")]
        private void EditorTestUpdateCounters()
        {
            if (!Application.isPlaying) return;

            foreach (CrystalType type in System.Enum.GetValues(typeof(CrystalType)))
            {
                int randomCount = Random.Range(0, 100);
                UpdateCrystalText(type, randomCount);
            }
        }

        [ContextMenu("Auto-Find Text References")]
        private void EditorAutoFindReferences()
        {
            // Busca automaticamente os textos no CrystalContainer
            Transform crystalContainer = transform.Find("CrystalContainer");
            if (crystalContainer == null)
            {
                Debug.LogError("[CrystalHUDController] CrystalContainer não encontrado como filho deste GameObject");
                return;
            }

            natureCountText = FindCountText(crystalContainer, "Crystal_Nature");
            fireCountText = FindCountText(crystalContainer, "Crystal_Fire");
            waterCountText = FindCountText(crystalContainer, "Crystal_Water");
            shadowCountText = FindCountText(crystalContainer, "Crystal_Shadow");
            earthCountText = FindCountText(crystalContainer, "Crystal_Earth");
            airCountText = FindCountText(crystalContainer, "Crystal_Air");

            Debug.Log("[CrystalHUDController] Referências encontradas automaticamente");
        }

        private TextMeshProUGUI FindCountText(Transform parent, string crystalName)
        {
            Transform crystalTransform = parent.Find(crystalName);
            if (crystalTransform == null)
            {
                Debug.LogWarning($"[CrystalHUDController] {crystalName} não encontrado");
                return null;
            }

            Transform countTextTransform = crystalTransform.Find("Count_Text");
            if (countTextTransform == null)
            {
                Debug.LogWarning($"[CrystalHUDController] Count_Text não encontrado em {crystalName}");
                return null;
            }

            return countTextTransform.GetComponent<TextMeshProUGUI>();
        }
#endif
        #endregion
    }
}
