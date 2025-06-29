using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Classe utilitária com funções e sistemas globais para o jogo
    /// </summary>
    public static class GameUtilities
    {
        /// <summary>
        /// Sistema de pool de objetos para otimizar a instanciação de itens
        /// </summary>
        public static class ItemPool
        {
            // Dicionário que armazena os pools para cada tipo de prefab
            private static Dictionary<GameObject, Queue<GameObject>> _pools = new Dictionary<GameObject, Queue<GameObject>>();
            
            /// <summary>
            /// Obtém um item do pool ou cria um novo se necessário
            /// </summary>
            /// <param name="prefab">O prefab do item desejado</param>
            /// <param name="position">Posição onde o item deve aparecer</param>
            /// <param name="rotation">Rotação do item</param>
            /// <returns>O GameObject instanciado ou obtido do pool</returns>
            public static GameObject GetItem(GameObject prefab, Vector3 position, Quaternion rotation)
            {
                if (prefab == null)
                    return null;
                
                // Cria um pool para este prefab se ainda não existir
                if (!_pools.ContainsKey(prefab))
                {
                    _pools[prefab] = new Queue<GameObject>();
                }
                
                GameObject item;
                
                // Verifica se há itens disponíveis no pool
                if (_pools[prefab].Count > 0)
                {
                    // Obtém um item existente do pool
                    item = _pools[prefab].Dequeue();
                    
                    // Reposiciona e reativa o item
                    item.transform.position = position;
                    item.transform.rotation = rotation;
                    item.SetActive(true);
                }
                else
                {
                    // Cria um novo item se o pool estiver vazio
                    item = Object.Instantiate(prefab, position, rotation);
                    
                    // Nomeia o objeto para facilitar a depuração
                    item.name = $"{prefab.name}_Pooled";
                }
                
                return item;
            }
            
            /// <summary>
            /// Devolve um item ao pool para reutilização
            /// </summary>
            /// <param name="item">O item a ser devolvido</param>
            /// <param name="prefabOrigin">O prefab original usado para criar o item</param>
            public static void ReturnToPool(GameObject item, GameObject prefabOrigin)
            {
                if (item == null || prefabOrigin == null)
                    return;
                
                // Desativa o item e o adiciona ao pool adequado
                item.SetActive(false);
                
                // Verifica se o pool para este prefab existe
                if (!_pools.ContainsKey(prefabOrigin))
                {
                    _pools[prefabOrigin] = new Queue<GameObject>();
                }
                
                // Adiciona o item ao pool
                _pools[prefabOrigin].Enqueue(item);
            }
            
            /// <summary>
            /// Limpa todos os pools
            /// </summary>
            public static void ClearAllPools()
            {
                foreach (var pool in _pools.Values)
                {
                    while (pool.Count > 0)
                    {
                        GameObject item = pool.Dequeue();
                        if (item != null)
                            Object.Destroy(item);
                    }
                }
                
                _pools.Clear();
            }
        }
    }
}