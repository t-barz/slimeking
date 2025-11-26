using UnityEngine;
using TheSlimeKing.NPCs;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Script demonstrativo de como configurar NPCs com sistema de drops.
    /// Este script mostra como adicionar o componente DropController a um NPC e configurar os drops.
    /// 
    /// COMO USAR:
    /// 1. Adicione este script a um GameObject que possui NPCAttributesHandler
    /// 2. Configure os prefabs de itens que serão dropados
    /// 3. Configure a quantidade mínima e máxima de drops
    /// 4. O NPC automaticamente dropará itens quando for derrotado
    /// 
    /// DEPENDÊNCIAS:
    /// • NPCAttributesHandler (obrigatório)
    /// • DropController (será adicionado automaticamente)
    /// • Prefabs de itens configurados (crystalA, crystalB, etc.)
    /// </summary>
    public class NPCDropExample : MonoBehaviour
    {
        [Header("🎁 Configuração de Drops")]
        [Tooltip("Prefabs de itens que podem ser dropados")]
        [SerializeField] private GameObject[] dropPrefabs = new GameObject[0];

        [Tooltip("Quantidade mínima de itens a dropar")]
        [Range(0, 10)]
        [SerializeField] private int minDrops = 1;

        [Tooltip("Quantidade máxima de itens a dropar")]
        [Range(1, 10)]
        [SerializeField] private int maxDrops = 3;

        [Header("🔧 Configuração Automática")]
        [Tooltip("Configurar DropController automaticamente no Awake")]
        [SerializeField] private bool autoConfigureDropController = true;

        [Tooltip("Usar cristais padrão se array estiver vazio")]
        [SerializeField] private bool useDefaultCrystals = true;

        private DropController _dropController;
        private NPCAttributesHandler _npcAttributes;

        #region Unity Lifecycle
        private void Awake()
        {
            // Verifica dependências
            _npcAttributes = GetComponent<NPCAttributesHandler>();
            if (_npcAttributes == null)
            {
                Debug.LogError($"[NPCDropExample] {gameObject.name} - NPCAttributesHandler não encontrado! " +
                              "Este script requer NPCAttributesHandler para funcionar.");
                enabled = false;
                return;
            }

            if (autoConfigureDropController)
            {
                ConfigureDropController();
            }
        }

        private void Start()
        {
            if (_dropController != null)
            {
                Debug.Log($"[NPCDropExample] {gameObject.name} - Sistema de drops configurado com " +
                         $"{dropPrefabs.Length} tipos de itens. Range: {minDrops}-{maxDrops} drops.");
            }
        }
        #endregion

        #region Configuration Methods
        /// <summary>
        /// Configura o DropController automaticamente
        /// </summary>
        private void ConfigureDropController()
        {
            // Adiciona DropController se não existir
            _dropController = GetComponent<DropController>();
            if (_dropController == null)
            {
                _dropController = gameObject.AddComponent<DropController>();
                Debug.Log($"[NPCDropExample] {gameObject.name} - DropController adicionado automaticamente.");
            }

            // Configura prefabs padrão se necessário
            if (useDefaultCrystals && (dropPrefabs == null || dropPrefabs.Length == 0))
            {
                SetupDefaultCrystalDrops();
            }

            // Aplica configurações via reflection (DropController tem campos privados)
            ApplyDropControllerSettings();
        }

        /// <summary>
        /// Configura cristais padrão do projeto
        /// </summary>
        private void SetupDefaultCrystalDrops()
        {
            // Tenta carregar prefabs de cristais padrão
            GameObject crystalA = Resources.Load<GameObject>("Prefabs/Items/crystalA");
            GameObject crystalB = Resources.Load<GameObject>("Prefabs/Items/crystalB");

            if (crystalA != null || crystalB != null)
            {
                var crystalList = new System.Collections.Generic.List<GameObject>();
                if (crystalA != null) crystalList.Add(crystalA);
                if (crystalB != null) crystalList.Add(crystalB);

                dropPrefabs = crystalList.ToArray();
                Debug.Log($"[NPCDropExample] {gameObject.name} - {crystalList.Count} cristais padrão carregados.");
            }
            else
            {
                Debug.LogWarning($"[NPCDropExample] {gameObject.name} - Cristais padrão não encontrados em Resources.");
            }
        }

        /// <summary>
        /// Aplica as configurações ao DropController usando reflexão
        /// </summary>
        private void ApplyDropControllerSettings()
        {
            if (_dropController == null) return;

            // Usa reflexão para configurar campos privados do DropController
            var dropControllerType = _dropController.GetType();

            // Configura lista de prefabs
            var prefabListField = dropControllerType.GetField("prefabList",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (prefabListField != null)
            {
                prefabListField.SetValue(_dropController, dropPrefabs);
            }

            // Configura quantidade mínima
            var minDropField = dropControllerType.GetField("minDropCount",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (minDropField != null)
            {
                minDropField.SetValue(_dropController, minDrops);
            }

            // Configura quantidade máxima
            var maxDropField = dropControllerType.GetField("maxDropCount",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (maxDropField != null)
            {
                maxDropField.SetValue(_dropController, maxDrops);
            }

            Debug.Log($"[NPCDropExample] {gameObject.name} - DropController configurado via reflexão.");
        }
        #endregion

        #region Debug Methods
        /// <summary>
        /// Força um drop para teste (apenas em modo debug)
        /// </summary>
        [ContextMenu("🎁 Force Drop (Test)")]
        public void ForceDropTest()
        {
            if (_dropController != null)
            {
                _dropController.DropItems();
                Debug.Log($"[NPCDropExample] {gameObject.name} - Drop forçado para teste!");
            }
            else
            {
                Debug.LogWarning($"[NPCDropExample] {gameObject.name} - DropController não configurado!");
            }
        }

        /// <summary>
        /// Simula morte do NPC para teste
        /// </summary>
        [ContextMenu("💀 Simulate Death (Test)")]
        public void SimulateDeathTest()
        {
            if (_npcAttributes != null)
            {
                // Força HP para 0 e causa 1 de dano para triggerar morte
                var oldHP = _npcAttributes.CurrentHealthPoints;
                _npcAttributes.TakeDamage(oldHP + 1);
                Debug.Log($"[NPCDropExample] {gameObject.name} - Morte simulada! HP era {oldHP}, agora é {_npcAttributes.CurrentHealthPoints}");
            }
        }

        /// <summary>
        /// Exibe informações de debug do sistema de drops
        /// </summary>
        [ContextMenu("📊 Debug Drop Info")]
        public void DebugDropInfo()
        {
            Debug.Log($"=== {gameObject.name} DROP INFO ===");
            Debug.Log($"DropController: {(_dropController != null ? "✓" : "✗")}");
            Debug.Log($"NPCAttributes: {(_npcAttributes != null ? "✓" : "✗")}");
            Debug.Log($"Drop Prefabs: {(dropPrefabs != null ? dropPrefabs.Length : 0)}");
            Debug.Log($"Drop Range: {minDrops}-{maxDrops}");
            Debug.Log($"Auto Configure: {autoConfigureDropController}");
            Debug.Log($"Use Default Crystals: {useDefaultCrystals}");

            if (dropPrefabs != null && dropPrefabs.Length > 0)
            {
                Debug.Log("Configured Drop Prefabs:");
                for (int i = 0; i < dropPrefabs.Length; i++)
                {
                    Debug.Log($"  [{i}] {(dropPrefabs[i] != null ? dropPrefabs[i].name : "NULL")}");
                }
            }
        }
        #endregion
    }
}