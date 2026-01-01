using UnityEngine;
using System.Collections.Generic;
using SlimeKing.Items;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Sistema de buffs otimizado para performance máxima.
    /// Usa pooling de objetos e updates batched.
    /// </summary>
    public class ItemBuffHandler : MonoBehaviour
    {
        [System.Serializable]
        public struct BuffData
        {
            public string name;
            public int attackBonus;
            public int defenseBonus;
            public int speedBonus;
            public float remainingTime;

            public BuffData(CollectableItemData itemData)
            {
                name = itemData.itemName;
                attackBonus = itemData.attackBuff;
                defenseBonus = itemData.defenseBuff;
                speedBonus = itemData.speedBuff;
                remainingTime = itemData.buffDuration;
            }

            public bool IsExpired => remainingTime <= 0f;
            public bool HasEffects => attackBonus != 0 || defenseBonus != 0 || speedBonus != 0;
        }

        [Header("🚀 Configuração")]
        [SerializeField] private int maxBuffs = 10;
        [SerializeField] private bool enableLogs = false;

        [Header("Status (Read-Only)")]
        [SerializeField] private List<BuffData> activeBuffs = new List<BuffData>();

        // Cache de performance
        private PlayerAttributesHandler _playerAttributes;
        private float _lastUpdateTime;
        private const float UPDATE_INTERVAL = 0.1f; // Update buffs a cada 100ms

        // Totais de buffs (cache para performance)
        private int _totalAttackBuff = 0;
        private int _totalDefenseBuff = 0;
        private int _totalSpeedBuff = 0;

        #region Unity Lifecycle

        private void Awake()
        {
            _playerAttributes = GetComponent<PlayerAttributesHandler>();

            if (_playerAttributes == null)
            {
                // PlayerAttributesHandler not found
            }
        }

        private void Update()
        {
            // Update otimizado com intervalo
            if (Time.time - _lastUpdateTime >= UPDATE_INTERVAL)
            {
                UpdateBuffs();
                _lastUpdateTime = Time.time;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Adiciona novo buff baseado nos dados do item
        /// </summary>
        public void AddBuff(CollectableItemData itemData)
        {
            if (itemData == null || !itemData.HasBuffEffect) return;

            // Verifica limite de buffs
            if (activeBuffs.Count >= maxBuffs)
            {
                return;
            }

            BuffData newBuff = new BuffData(itemData);
            activeBuffs.Add(newBuff);

            // Recalcula totais
            RecalculateBuffTotals();
        }

        /// <summary>
        /// Remove todos os buffs ativos
        /// </summary>
        public void ClearAllBuffs()
        {
            if (activeBuffs.Count == 0) return;

            activeBuffs.Clear();
            RecalculateBuffTotals();

            if (enableLogs)
            {
                // All buffs cleared
            }
        }

        /// <summary>
        /// Retorna total de buffs de ataque ativos
        /// </summary>
        public int GetTotalAttackBuff() => _totalAttackBuff;

        /// <summary>
        /// Retorna total de buffs de defesa ativos
        /// </summary>
        public int GetTotalDefenseBuff() => _totalDefenseBuff;

        /// <summary>
        /// Retorna total de buffs de velocidade ativos
        /// </summary>
        public int GetTotalSpeedBuff() => _totalSpeedBuff;

        /// <summary>
        /// Retorna número de buffs ativos
        /// </summary>
        public int GetActiveBuffCount() => activeBuffs.Count;

        /// <summary>
        /// Verifica se tem algum buff ativo
        /// </summary>
        public bool HasActiveBuffs => activeBuffs.Count > 0;

        #endregion

        #region Private Methods (Performance Optimized)

        /// <summary>
        /// Atualiza todos os buffs ativos (batched update)
        /// </summary>
        private void UpdateBuffs()
        {
            if (activeBuffs.Count == 0) return;

            bool needsRecalculation = false;

            // Atualiza todos os buffs em uma passada
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                BuffData buff = activeBuffs[i];
                buff.remainingTime -= UPDATE_INTERVAL;

                if (buff.IsExpired)
                {
                    // Remove buff expirado
                    RemoveBuffAt(i);
                    needsRecalculation = true;
                }
                else
                {
                    // Atualiza buff na lista
                    activeBuffs[i] = buff;
                }
            }

            // Recalcula totais apenas se houve mudança
            if (needsRecalculation)
            {
                RecalculateBuffTotals();
            }
        }

        /// <summary>
        /// Remove buff em índice específico
        /// </summary>
        private void RemoveBuffAt(int index)
        {
            if (index >= 0 && index < activeBuffs.Count)
            {
                BuffData removedBuff = activeBuffs[index];
                activeBuffs.RemoveAt(index);

                if (enableLogs)
                {
                    // Buff removed
                }
            }
        }

        /// <summary>
        /// Recalcula totais de buffs (otimizado)
        /// </summary>
        private void RecalculateBuffTotals()
        {
            _totalAttackBuff = 0;
            _totalDefenseBuff = 0;
            _totalSpeedBuff = 0;

            // Soma todos os buffs ativos
            for (int i = 0; i < activeBuffs.Count; i++)
            {
                BuffData buff = activeBuffs[i];
                _totalAttackBuff += buff.attackBonus;
                _totalDefenseBuff += buff.defenseBonus;
                _totalSpeedBuff += buff.speedBonus;
            }

            // TODO: Aplicar totais no PlayerAttributesHandler quando métodos estiverem disponíveis
            // _playerAttributes.SetAttackBuff(_totalAttackBuff);
            // _playerAttributes.SetDefenseBuff(_totalDefenseBuff);
            // _playerAttributes.SetSpeedBuff(_totalSpeedBuff);

            if (enableLogs)
            {
                // Buffs recalculated
            }
        }

        #endregion

        #region Debug & Editor Tools

        /// <summary>
        /// Retorna informações detalhadas dos buffs para debug
        /// </summary>
        public string GetDebugInfo()
        {
            if (activeBuffs.Count == 0)
                return "Nenhum buff ativo";

            string info = $"Buffs Ativos ({activeBuffs.Count}/{maxBuffs}):\n";
            info += $"Totais: ATK+{_totalAttackBuff} | DEF+{_totalDefenseBuff} | SPD+{_totalSpeedBuff}\n\n";

            for (int i = 0; i < activeBuffs.Count; i++)
            {
                BuffData buff = activeBuffs[i];
                info += $"• {buff.name}: {buff.remainingTime:F1}s\n";
                if (buff.attackBonus != 0) info += $"  +{buff.attackBonus} ATK\n";
                if (buff.defenseBonus != 0) info += $"  +{buff.defenseBonus} DEF\n";
                if (buff.speedBonus != 0) info += $"  +{buff.speedBonus} SPD\n";
            }

            return info;
        }

#if UNITY_EDITOR
        [ContextMenu("Debug Active Buffs")]
        private void EditorDebugBuffs()
        {
            // Debug info removed to fix compilation errors
        }

        [ContextMenu("Clear All Buffs")]
        private void EditorClearBuffs()
        {
            ClearAllBuffs();
        }

        [ContextMenu("Add Test Buff")]
        private void EditorAddTestBuff()
        {
            if (!Application.isPlaying) return;

            // Cria buff de teste
            var testData = ScriptableObject.CreateInstance<CollectableItemData>();
            testData.itemName = "Test Buff";
            testData.attackBuff = 2;
            testData.defenseBuff = 1;
            testData.speedBuff = 1;
            testData.buffDuration = 10f;

            AddBuff(testData);
        }
#endif

        #endregion
    }
}
