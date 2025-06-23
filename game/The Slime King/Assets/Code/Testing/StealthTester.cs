using UnityEngine;
using TheSlimeKing.Gameplay.Stealth;

namespace TheSlimeKing.Testing
{
    /// <summary>
    /// Script para testar o sistema de stealth
    /// </summary>
    public class StealthTester : MonoBehaviour
    {
        [Header("Objetos para Teste")]
        [SerializeField] private GameObject bushCoverPrefab;
        [SerializeField] private GameObject grassCoverPrefab;
        [SerializeField] private GameObject rockCoverPrefab;
        [SerializeField] private GameObject treeCoverPrefab;

        [Header("Configurações de Teste")]
        [SerializeField] private bool autoSpawnCoverObjects = true;
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private float spawnRadius = 5f;

        // Referências ao sistema de stealth
        private StealthController _stealthController;

        private void Start()
        {            // Encontra o controller de stealth
            _stealthController = FindAnyObjectByType<StealthController>();

            // Adiciona componente de stealth ao player se não existir
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && _stealthController == null)
            {
                _stealthController = player.AddComponent<StealthController>();
                Debug.Log("StealthTester: StealthController adicionado ao Player automaticamente");
            }

            if (autoSpawnCoverObjects)
            {
                SpawnTestCoverObjects();
            }

            Debug.Log("StealthTester: Pronto para testes. Pressione Q para agachar e procure objetos de cobertura");
        }

        private void Update()
        {
            if (showDebugInfo && _stealthController != null)
            {
                // Exibe informação de estado atual
                StealthState currentState = _stealthController.GetStealthState();
                bool isHidden = _stealthController.IsHidden();
                bool isDetectable = _stealthController.IsDetectable();

                DisplayDebugInfo(currentState, isHidden, isDetectable);
            }
        }

        /// <summary>
        /// Cria objetos de cobertura para teste
        /// </summary>
        private void SpawnTestCoverObjects()
        {
            // Define as posições ao redor do jogador
            Vector2[] positions = {
                new Vector2(2, 2),    // Superior direita
                new Vector2(-2, 2),   // Superior esquerda
                new Vector2(2, -2),   // Inferior direita
                new Vector2(-2, -2),  // Inferior esquerda
                new Vector2(0, 3),    // Cima
                new Vector2(3, 0),    // Direita
                new Vector2(0, -3),   // Baixo
                new Vector2(-3, 0)    // Esquerda
            };

            // Encontra o jogador
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 basePosition = player != null ? player.transform.position : transform.position;

            // Cria cada tipo de cobertura
            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 spawnPos = basePosition + new Vector3(positions[i].x, positions[i].y, 0);
                GameObject prefabToSpawn = null;

                // Seleciona um prefab diferente de acordo com o índice
                switch (i % 4)
                {
                    case 0: prefabToSpawn = bushCoverPrefab; break;
                    case 1: prefabToSpawn = grassCoverPrefab; break;
                    case 2: prefabToSpawn = rockCoverPrefab; break;
                    case 3: prefabToSpawn = treeCoverPrefab; break;
                }

                // Instancia o objeto se o prefab existir
                if (prefabToSpawn != null)
                {
                    GameObject coverObj = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                    coverObj.name = $"TestCover_{i}";

                    // Adiciona componente CoverObject se não existir
                    if (coverObj.GetComponent<CoverObject>() == null)
                    {
                        CoverObject coverComponent = coverObj.AddComponent<CoverObject>();
                        // Configura o tipo de acordo com o prefab
                        switch (i % 4)
                        {
                            case 0: coverComponent.tag = "Bush"; break;
                            case 1: coverComponent.tag = "Grass"; break;
                            case 2: coverComponent.tag = "Rock"; break;
                            case 3: coverComponent.tag = "Tree"; break;
                        }
                    }
                }
            }

            Debug.Log("StealthTester: Objetos de cobertura criados para teste");
        }

        /// <summary>
        /// Exibe informações de debug na tela
        /// </summary>
        private void DisplayDebugInfo(StealthState currentState, bool isHidden, bool isDetectable)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 14;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.UpperLeft;
            style.wordWrap = true;

            string stateColor = isHidden ? "green" : (isDetectable ? "red" : "yellow");
            string stateText = $"<color={stateColor}>{currentState}</color>";

            string debugText = $"Estado Stealth: {stateText}\n" +
                              $"Escondido: {isHidden}\n" +
                              $"Detectável: {isDetectable}\n" +
                              $"Pressione Q para agachar";

            // Desenha a informação na tela
            GUI.Label(new Rect(10, 10, 300, 100), debugText, style);
        }

        private void OnDrawGizmosSelected()
        {
            // Visualização do raio de spawn
            Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }
    }
}
