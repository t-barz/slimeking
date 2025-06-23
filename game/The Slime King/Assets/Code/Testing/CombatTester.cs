using UnityEngine;

namespace TheSlimeKing.Testing
{
    /// <summary>
    /// Ferramenta para testar o sistema de combate
    /// </summary>
    public class CombatTester : MonoBehaviour
    {
        [Header("Configurações de Inimigos")]
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private int _enemiesCount = 5;
        [SerializeField] private float _spawnRadius = 5f;
        
        [Header("Opções de Teste")]
        [SerializeField] private bool _spawnOnStart = false;
        
        private void Start()
        {
            if (_spawnOnStart)
            {
                SpawnEnemies();
            }
        }
        
        /// <summary>
        /// Cria inimigos ao redor para teste de combate
        /// </summary>
        public void SpawnEnemies()
        {
            if (_enemyPrefab == null)
            {
                Debug.LogError("Prefab de inimigo não configurado no CombatTester");
                return;
            }
            
            for (int i = 0; i < _enemiesCount; i++)
            {
                // Gerar posição aleatória em círculo
                float angle = i * (360f / _enemiesCount);
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * _spawnRadius;
                float y = Mathf.Sin(angle * Mathf.Deg2Rad) * _spawnRadius;
                
                Vector3 spawnPos = transform.position + new Vector3(x, y, 0);
                
                // Instanciar inimigo
                GameObject enemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
                enemy.name = $"Enemy_{i+1}";
            }
            
            Debug.Log($"Gerados {_enemiesCount} inimigos para teste");
        }
        
        /// <summary>
        /// Destroi todos os inimigos de teste
        /// </summary>
        public void ClearEnemies()
        {
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (var enemy in enemies)
            {
                Destroy(enemy);
            }
            
            Debug.Log($"Removidos {enemies.Length} inimigos");
        }
        
        private void OnDrawGizmosSelected()
        {
            // Visualizar área de spawn
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _spawnRadius);
        }
    }
}
