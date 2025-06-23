using UnityEngine;
using UnityEngine.UI;
using TheSlimeKing.Core.Portal;
using TMPro;

namespace TheSlimeKing.Testing
{
    /// <summary>
    /// Classe para testar o funcionamento do sistema de portais.
    /// Permite criar portais dinamicamente e testar teleportes.
    /// </summary>
    public class PortalTester : MonoBehaviour
    {
        [Header("Portais de Teste")]
        [SerializeField] private PortalController _portalPrefab;
        [SerializeField] private Transform _testLocationA;
        [SerializeField] private Transform _testLocationB;
        [SerializeField] private string _testSceneName;

        [Header("UI")]
        [SerializeField] private Button _createIntraScenePortalsButton;
        [SerializeField] private Button _createInterScenePortalButton;
        [SerializeField] private Button _teleportPlayerButton;
        [SerializeField] private TMP_Text _statusText;

        private PortalController _portalA;
        private PortalController _portalB;

        private void Start()
        {
            // Configura os botões
            if (_createIntraScenePortalsButton != null)
                _createIntraScenePortalsButton.onClick.AddListener(CreateIntraScenePortals);

            if (_createInterScenePortalButton != null)
                _createInterScenePortalButton.onClick.AddListener(CreateInterScenePortal);

            if (_teleportPlayerButton != null)
                _teleportPlayerButton.onClick.AddListener(TeleportPlayerDirectly);

            UpdateStatus("Pronto para testar o sistema de portais");
        }

        /// <summary>
        /// Cria dois portais na mesma cena para testar teleporte intra-cena
        /// </summary>
        public void CreateIntraScenePortals()
        {
            // Destrói portais anteriores se existirem
            if (_portalA != null) Destroy(_portalA.gameObject);
            if (_portalB != null) Destroy(_portalB.gameObject);

            // Verifica se temos as referências necessárias
            if (_portalPrefab == null || _testLocationA == null || _testLocationB == null)
            {
                UpdateStatus("ERRO: Prefab de portal ou locais de teste não configurados");
                return;
            }

            // Cria o portal A
            _portalA = Instantiate(_portalPrefab, _testLocationA.position, Quaternion.identity);
            _portalA.gameObject.name = "TestPortalA";

            // Acessa os campos privados via reflection (apenas para testes)
            var portalIDField = typeof(PortalController).GetField("_portalID",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var targetPortalIDField = typeof(PortalController).GetField("_targetPortalID",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (portalIDField != null)
                portalIDField.SetValue(_portalA, "test_portal_a");

            // Cria o portal B
            _portalB = Instantiate(_portalPrefab, _testLocationB.position, Quaternion.identity);
            _portalB.gameObject.name = "TestPortalB";

            if (portalIDField != null)
                portalIDField.SetValue(_portalB, "test_portal_b");

            // Configura os portais para apontarem um para o outro
            if (targetPortalIDField != null)
            {
                targetPortalIDField.SetValue(_portalA, "test_portal_b");
                targetPortalIDField.SetValue(_portalB, "test_portal_a");
            }

            UpdateStatus("Portais intra-cena criados com sucesso. Teste teleportando entre eles.");
        }

        /// <summary>
        /// Cria um portal que leva para outra cena
        /// </summary>
        public void CreateInterScenePortal()
        {
            // Destrói portais anteriores se existirem
            if (_portalA != null) Destroy(_portalA.gameObject);
            if (_portalB != null) Destroy(_portalB.gameObject);

            // Verifica se temos as referências necessárias
            if (_portalPrefab == null || _testLocationA == null || string.IsNullOrEmpty(_testSceneName))
            {
                UpdateStatus("ERRO: Prefab de portal, local de teste ou nome da cena não configurados");
                return;
            }

            // Cria o portal
            _portalA = Instantiate(_portalPrefab, _testLocationA.position, Quaternion.identity);
            _portalA.gameObject.name = "InterScenePortal";

            // Acessa os campos privados via reflection (apenas para testes)
            var portalIDField = typeof(PortalController).GetField("_portalID",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isScenePortalField = typeof(PortalController).GetField("_isScenePortal",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var targetSceneNameField = typeof(PortalController).GetField("_targetSceneName",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (portalIDField != null)
                portalIDField.SetValue(_portalA, "inter_scene_portal");

            if (isScenePortalField != null)
                isScenePortalField.SetValue(_portalA, true);

            if (targetSceneNameField != null)
                targetSceneNameField.SetValue(_portalA, _testSceneName);

            UpdateStatus($"Portal inter-cena criado com sucesso. Destino: {_testSceneName}");
        }

        /// <summary>
        /// Teleporta o jogador diretamente usando o PortalManager
        /// </summary>
        public void TeleportPlayerDirectly()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                UpdateStatus("ERRO: Jogador não encontrado na cena");
                return;
            }

            if (_testLocationB != null)
            {
                PortalManager.Instance.TeleportIntraSameCene(
                    player, _testLocationB.position, _testLocationB.eulerAngles);

                UpdateStatus("Teleporte direto realizado com sucesso");
            }
            else
            {
                UpdateStatus("ERRO: Local de destino não configurado");
            }
        }

        /// <summary>
        /// Atualiza o texto de status
        /// </summary>
        private void UpdateStatus(string message)
        {
            if (_statusText != null)
            {
                _statusText.text = message;
                Debug.Log($"[PortalTester] {message}");
            }
        }

        private void OnDestroy()
        {
            // Remove os listeners dos botões
            if (_createIntraScenePortalsButton != null)
                _createIntraScenePortalsButton.onClick.RemoveListener(CreateIntraScenePortals);

            if (_createInterScenePortalButton != null)
                _createInterScenePortalButton.onClick.RemoveListener(CreateInterScenePortal);

            if (_teleportPlayerButton != null)
                _teleportPlayerButton.onClick.RemoveListener(TeleportPlayerDirectly);
        }
    }
}
