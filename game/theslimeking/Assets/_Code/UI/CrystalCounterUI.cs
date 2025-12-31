using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SlimeKing.Core;
using System.Collections.Generic;
using UnityDebug = UnityEngine.Debug;

namespace SlimeKing.UI
{
    /// <summary>
    /// Gerencia a atualização dos contadores de cristais na UI do CanvasHUD.
    /// Conecta-se aos eventos do GameManager para atualizar automaticamente os textos.
    /// </summary>
    public class CrystalCounterUI : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;

        // Cache de referências dos textos dos contadores
        private Dictionary<CrystalType, TextMeshProUGUI> crystalCounterTexts;

        #region Unity Lifecycle

        private void Awake()
        {
            UnityDebug.Log($"[CrystalCounterUI] Awake chamado em GameObject: {gameObject.name}");
            InitializeCounterReferences();
        }

        private void OnEnable()
        {
            // Tenta subscrever aos eventos do GameManager quando disponível
            // Se GameManager ainda não estiver pronto, tentará novamente no Start()
            SubscribeToEvents();
        }

        private void OnDisable()
        {
            // Remove subscrições para evitar memory leaks
            UnsubscribeFromEvents();
        }

        private void Start()
        {
            UnityDebug.Log($"[CrystalCounterUI] Start chamado. Contadores conectados: {crystalCounterTexts.Count}/6");
            
            // Tenta subscrever novamente caso não tenha conseguido no OnEnable
            if (GameManager.HasInstance)
            {
                // Força subscrição se GameManager agora está disponível
                UnsubscribeFromEvents(); // Remove qualquer subscrição anterior
                SubscribeToEvents(); // Subscreve novamente
            }
            
            // Atualiza todos os contadores na inicialização
            UpdateAllCounters();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Inicializa referências para os textos dos contadores de cristais
        /// </summary>
        private void InitializeCounterReferences()
        {
            crystalCounterTexts = new Dictionary<CrystalType, TextMeshProUGUI>();

            // Mapeia os nomes dos GameObjects na UI para os tipos de cristais
            var crystalMappings = new Dictionary<string, CrystalType>
            {
                { "Crystal_Nature", CrystalType.Nature },
                { "Crystal_Fire", CrystalType.Fire },
                { "Crystal_Water", CrystalType.Water },
                { "Crystal_Shadow", CrystalType.Shadow },
                { "Crystal_Earth", CrystalType.Earth },
                { "Crystal_Air", CrystalType.Air }
            };

            // Encontra e cacheia as referências dos textos
            foreach (var mapping in crystalMappings)
            {
                string crystalGameObjectName = mapping.Key;
                CrystalType crystalType = mapping.Value;

                // Procura o GameObject do cristal na hierarquia
                Transform crystalTransform = FindCrystalObject(crystalGameObjectName);
                if (crystalTransform != null)
                {
                    // Procura o componente Count_Text dentro do GameObject do cristal
                    Transform countTextTransform = crystalTransform.Find("Count_Text");
                    if (countTextTransform != null)
                    {
                        var textComponent = countTextTransform.GetComponent<TextMeshProUGUI>();
                        if (textComponent != null)
                        {
                            crystalCounterTexts[crystalType] = textComponent;
                            LogMessage($"Contador de {crystalType} conectado: {crystalGameObjectName}/Count_Text");
                        }
                        else
                        {
                            LogWarningMessage($"TextMeshProUGUI não encontrado em {crystalGameObjectName}/Count_Text");
                        }
                    }
                    else
                    {
                        LogWarningMessage($"Count_Text não encontrado em {crystalGameObjectName}");
                    }
                }
                else
                {
                    LogWarningMessage($"GameObject {crystalGameObjectName} não encontrado na hierarquia");
                }
            }

            LogMessage($"Inicialização concluída: {crystalCounterTexts.Count}/6 contadores conectados");
        }

        /// <summary>
        /// Encontra o GameObject de um cristal específico na hierarquia
        /// </summary>
        private Transform FindCrystalObject(string crystalName)
        {
            // Primeiro tenta encontrar CrystalContainer
            Transform crystalContainer = transform.Find("CrystalContainer");
            if (crystalContainer == null)
            {
                // Se não encontrar como filho direto, procura recursivamente
                crystalContainer = FindDeepChild(transform, "CrystalContainer");
            }

            if (crystalContainer != null)
            {
                // Procura o cristal específico dentro do CrystalContainer
                Transform crystalTransform = crystalContainer.Find(crystalName);
                if (crystalTransform != null)
                {
                    return crystalTransform;
                }
            }

            // Fallback: procura em toda a hierarquia
            Transform localResult = transform.Find(crystalName);
            if (localResult != null) return localResult;

            return FindDeepChild(transform, crystalName);
        }

        /// <summary>
        /// Busca recursiva profunda por um GameObject filho
        /// </summary>
        private Transform FindDeepChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                Transform result = FindDeepChild(child, name);
                if (result != null)
                    return result;
            }
            return null;
        }

        #endregion

        #region Event Management

        /// <summary>
        /// Subscreve aos eventos do GameManager
        /// </summary>
        private void SubscribeToEvents()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnCrystalCountChanged += HandleCrystalCountChanged;
                LogMessage("Subscrito aos eventos do GameManager");
            }
            else
            {
                LogWarningMessage("GameManager não disponível para subscrição de eventos");
            }
        }

        /// <summary>
        /// Remove subscrições dos eventos do GameManager
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.OnCrystalCountChanged -= HandleCrystalCountChanged;
                LogMessage("Subscrições removidas dos eventos do GameManager");
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Manipula mudanças nos contadores de cristais
        /// </summary>
        private void HandleCrystalCountChanged(CrystalType crystalType, int newCount)
        {
            UpdateCrystalCounter(crystalType, newCount);
        }

        #endregion

        #region UI Updates

        /// <summary>
        /// Atualiza o contador de um tipo específico de cristal
        /// </summary>
        private void UpdateCrystalCounter(CrystalType crystalType, int count)
        {
            if (crystalCounterTexts.TryGetValue(crystalType, out TextMeshProUGUI counterText))
            {
                if (counterText != null)
                {
                    counterText.text = count.ToString();
                    LogMessage($"Contador de {crystalType} atualizado: {count}");
                }
                else
                {
                    LogWarningMessage($"Referência do contador de {crystalType} é null");
                }
            }
            else
            {
                LogWarningMessage($"Contador de {crystalType} não encontrado no cache");
            }
        }

        /// <summary>
        /// Atualiza todos os contadores com os valores atuais do GameManager
        /// </summary>
        private void UpdateAllCounters()
        {
            if (!GameManager.HasInstance)
            {
                LogWarningMessage("GameManager não disponível para atualização dos contadores");
                return;
            }

            var allCounts = GameManager.Instance.GetAllCrystalCounts();
            foreach (var kvp in allCounts)
            {
                UpdateCrystalCounter(kvp.Key, kvp.Value);
            }

            LogMessage("Todos os contadores atualizados");
        }

        #endregion

        #region Public API

        /// <summary>
        /// Força atualização de todos os contadores
        /// </summary>
        public void RefreshAllCounters()
        {
            UpdateAllCounters();
        }

        /// <summary>
        /// Verifica se todos os contadores estão conectados corretamente
        /// </summary>
        public bool AreAllCountersConnected()
        {
            return crystalCounterTexts.Count == 6;
        }

        /// <summary>
        /// Retorna o número de contadores conectados
        /// </summary>
        public int GetConnectedCountersCount()
        {
            return crystalCounterTexts.Count;
        }

        #endregion

        #region Logging

        private void LogMessage(string message)
        {
            if (enableDebugLogs)
            {
                UnityDebug.Log($"[CrystalCounterUI] {message}");
            }
        }

        private void LogWarningMessage(string message)
        {
            if (enableDebugLogs)
            {
                UnityDebug.LogWarning($"[CrystalCounterUI] {message}");
            }
        }

        #endregion

        #region Editor Support

#if UNITY_EDITOR
        [ContextMenu("Refresh All Counters")]
        private void EditorRefreshCounters()
        {
            if (Application.isPlaying)
            {
                RefreshAllCounters();
            }
            else
            {
                UnityDebug.Log("[CrystalCounterUI] Refresh só funciona durante o Play Mode");
            }
        }

        [ContextMenu("Debug Counter Status")]
        private void EditorDebugCounterStatus()
        {
            if (crystalCounterTexts == null)
            {
                InitializeCounterReferences();
            }

            string status = $"[CrystalCounterUI] Status dos contadores:\n";
            status += $"Conectados: {GetConnectedCountersCount()}/6\n\n";

            foreach (CrystalType crystalType in System.Enum.GetValues(typeof(CrystalType)))
            {
                bool connected = crystalCounterTexts.ContainsKey(crystalType) && crystalCounterTexts[crystalType] != null;
                string currentValue = "N/A";

                if (connected && Application.isPlaying && GameManager.HasInstance)
                {
                    currentValue = GameManager.Instance.GetCrystalCount(crystalType).ToString();
                }

                status += $"{crystalType}: {(connected ? "✓" : "✗")} (Valor: {currentValue})\n";
            }

            UnityDebug.Log(status);
        }
#endif

        #endregion
    }
}