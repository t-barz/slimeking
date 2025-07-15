using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o sistema de drops de itens quando inimigos ou objetos são destruídos.
/// </summary>
public class DropManager : MonoBehaviour
{
    #region Configurações de Drop
    [Header("Configuração de Itens")]
    [Tooltip("Lista de prefabs de itens que podem ser dropados")]
    [SerializeField] private List<GameObject> possibleDrops = new List<GameObject>();

    [Tooltip("Quantidade mínima de itens que podem ser dropados")]
    [SerializeField] private int minDropCount = 0;

    [Tooltip("Quantidade máxima de itens que podem ser dropados")]
    [SerializeField] private int maxDropCount = 1;

    [Header("Configuração de Espalhamento")]
    [Tooltip("Distância máxima que os items podem se espalhar na horizontal")]
    [SerializeField] private float spreadRadiusX = 0.5f;

    [Tooltip("Distância máxima que os items podem se espalhar na vertical")]
    [SerializeField] private float spreadRadiusY = 0.5f;
    #endregion

    #region Implementação
    /// <summary>
    /// Sorteia e instancia um número aleatório de itens da lista de drops possíveis.
    /// </summary>
    /// <returns>Lista dos objetos instanciados</returns>
    public List<GameObject> DropItems()
    {
        // Usa a posição atual do objeto como ponto de spawn
        return DropItems(transform.position);
    }

    /// <summary>
    /// Sorteia e instancia um número aleatório de itens da lista de drops possíveis
    /// em uma posição específica.
    /// </summary>
    /// <param name="position">Posição onde os itens serão dropados</param>
    /// <returns>Lista dos objetos instanciados</returns>
    public List<GameObject> DropItems(Vector3 position)
    {
        List<GameObject> droppedItems = new List<GameObject>();

        // Verifica se há itens possíveis para dropar
        if (possibleDrops.Count == 0)
        {
            Debug.LogWarning("Não há itens configurados para drop!");
            return droppedItems;
        }

        // Garante que minDropCount não é maior que maxDropCount
        int min = Mathf.Min(minDropCount, maxDropCount);
        int max = Mathf.Max(minDropCount, maxDropCount);

        // Sorteia quantos itens serão dropados (inclusivo min, exclusivo max+1)
        int dropCount = Random.Range(min, max + 1);

        // Cria cada item dropado
        for (int i = 0; i < dropCount; i++)
        {
            // Sorteia qual item será dropado
            int dropIndex = Random.Range(0, possibleDrops.Count);
            GameObject dropPrefab = possibleDrops[dropIndex];

            // Calcula uma posição aleatória dentro do raio de espalhamento
            Vector2 offset = new Vector2(
                Random.Range(-spreadRadiusX, spreadRadiusX),
                Random.Range(-spreadRadiusY, spreadRadiusY)
            );

            // Instancia o objeto na posição calculada
            Vector3 dropPosition = position + new Vector3(offset.x, offset.y, 0);
            GameObject droppedItem = Instantiate(dropPrefab, dropPosition, Quaternion.identity);

            // Adiciona à lista de itens retornados
            droppedItems.Add(droppedItem);
        }

        return droppedItems;
    }
    #endregion
}
