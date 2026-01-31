using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Controlador responsável pela instanciação aleatória de prefabs.
    /// Permite configurar uma lista de prefabs e sortear quantos e quais objetos serão criados.
    /// 
    /// FUNCIONALIDADES:
    /// • Lista configurável de prefabs para instanciação
    /// • Range configurável de quantidade de objetos a serem criados
    /// • Seleção aleatória de prefabs da lista
/// • Instanciação na posição do objeto atual
/// • Controle de debug para acompanhar o processo
/// 
/// EXEMPLO DE USO:
/// • Lista com 5 prefabs diferentes
/// • Range de 2-4 objetos
/// • Chama DropItems() -> sorteia 3 objetos e instancia 3 prefabs aleatórios
/// 
/// DEPENDÊNCIAS:
/// • Prefabs configurados na lista devem existir no projeto
/// </summary>
public class DropController : MonoBehaviour
{
    #region Serialized Fields
    [Header("🎁 Configurações de Drop")]
    [Tooltip("Lista de prefabs que podem ser instanciados")]
    [SerializeField] private GameObject[] prefabList;

    [Tooltip("Quantidade mínima de objetos a serem instanciados")]
    [SerializeField] private int minDropCount = 1;

    [Tooltip("Quantidade máxima de objetos a serem instanciados")]
    [SerializeField] private int maxDropCount = 3;

    [Header("📍 Configurações de Posicionamento")]
    [Tooltip("Se verdadeiro, instancia na posição deste objeto. Se falso, instancia na origem")]
    [SerializeField] private bool useCurrentPosition = true;

    [Header("💀 Efeito de Morte")]
    [Tooltip("Prefab do efeito que será instanciado quando o inimigo morrer")]
    [SerializeField] private GameObject deathEffectPrefab;

    [Header("Debug")]
    [Tooltip("Mostra logs de debug no Console")]
    [SerializeField] private bool enableDebugLogs = false;
    #endregion

    #region Public Methods
    /// <summary>
    /// Sorteia e instancia uma quantidade aleatória de prefabs da lista.
    /// A quantidade é determinada pelo range configurado (minDropCount - maxDropCount).
    /// Os prefabs são selecionados aleatoriamente da lista.
    /// </summary>
    public void DropItems()
    {
        DropItemsInternal();
    }

    /// <summary>
    /// Instancia o efeito de morte na posição do pivô e faz o drop dos itens.
    /// Deve ser chamado quando o inimigo for derrotado.
    /// </summary>
    public void DropItemsWithDeathEffect()
    {
        // Instancia o efeito de morte
        if (deathEffectPrefab != null)
        {
            Vector3 spawnPosition = useCurrentPosition ? transform.position : Vector3.zero;
            GameObject deathEffect = Instantiate(deathEffectPrefab, spawnPosition, Quaternion.identity);

            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"DropController: Efeito de morte instanciado na posição {spawnPosition}", this);
            }
        }
        else if (enableDebugLogs)
        {
            UnityEngine.Debug.LogWarning("DropController: deathEffectPrefab não foi configurado!", this);
        }

        // Faz o drop dos itens
        DropItemsInternal();
    }

    /// <summary>
    /// Implementação interna do drop de itens.
    /// </summary>
    private void DropItemsInternal()
    {
        // Validação da lista de prefabs
        if (prefabList == null || prefabList.Length == 0)
        {
            return;
        }

        // Validação do range
        if (minDropCount < 1)
        {
            minDropCount = 1;
        }

        if (maxDropCount < minDropCount)
        {
            maxDropCount = minDropCount;
        }

        // Sorteia quantos objetos serão instanciados
        int dropCount = Random.Range(minDropCount, maxDropCount + 1);

        // Determina a posição de instanciação
        Vector3 spawnPosition = useCurrentPosition ? transform.position : Vector3.zero;

        // Log de debug inicial
        if (enableDebugLogs)
        {
            UnityEngine.Debug.Log($"DropController: Iniciando drop de {dropCount} item(s) na posição {spawnPosition}", this);
        }

        // Instancia os objetos sorteados
        for (int i = 0; i < dropCount; i++)
        {
            // Seleciona um prefab aleatório da lista
            int randomIndex = Random.Range(0, prefabList.Length);
            GameObject selectedPrefab = prefabList[randomIndex];

            // Validação do prefab selecionado
            if (selectedPrefab == null)
            {
                continue;
            }

            // Instancia o prefab
            GameObject droppedItem = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            // Log de debug para cada item
            if (enableDebugLogs)
            {
                UnityEngine.Debug.Log($"DropController: Item {i + 1}/{dropCount} instanciado.", this);
            }
        }

        // Log de debug final
        if (enableDebugLogs)
        {
            UnityEngine.Debug.Log($"DropController: Drop finalizado! {dropCount} item(s) instanciado(s).", this);
        }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Quantidade de prefabs disponíveis na lista.
    /// </summary>
    public int PrefabCount => prefabList != null ? prefabList.Length : 0;

    /// <summary>
    /// Quantidade mínima configurada para drop.
    /// </summary>
    public int MinDropCount => minDropCount;

    /// <summary>
    /// Quantidade máxima configurada para drop.
    /// </summary>
    public int MaxDropCount => maxDropCount;

    /// <summary>
    /// Verifica se a lista de prefabs está válida.
    /// </summary>
    public bool HasValidPrefabs => prefabList != null && prefabList.Length > 0;

    /// <summary>
    /// Verifica se o efeito de morte está configurado.
    /// </summary>
    public bool HasDeathEffect => deathEffectPrefab != null;
    #endregion
}
}
