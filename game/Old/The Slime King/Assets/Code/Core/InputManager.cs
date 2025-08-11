using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gerencia os inputs do jogador, centralizando o acesso aos comandos principais do jogo.
/// Permite fácil integração com sistemas de combate, movimentação, inventário e menus.
/// Utiliza o padrão Singleton para acesso global.
/// Versão otimizada com cache de ações para melhor performance.
/// </summary>
public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Instância global do InputManager (Singleton).
    /// </summary>
    public static InputManager Instance { get; private set; }

    [Header("Referência ao InputActionAsset")]
    [Tooltip("Asset de Input gerado pelo Input System (InputSystem_Actions.inputactions).")]
    [SerializeField] private InputActionAsset inputActions;

    // Ações principais do jogador
    /// <summary>Movimentação do personagem.</summary>
    public InputAction MoveAction { get; private set; }
    /// <summary>Ataque básico.</summary>
    public InputAction AttackAction { get; private set; }
    /// <summary>Ataque especial.</summary>
    public InputAction SpecialAttackAction { get; private set; }
    /// <summary>Agachar ou esconder.</summary>
    public InputAction CrouchAction { get; private set; }
    /// <summary>Interagir com objetos do mundo.</summary>
    public InputAction InteractAction { get; private set; }
    /// <summary>Abrir menu de pausa ou opções.</summary>
    public InputAction MenuAction { get; private set; }
    /// <summary>Abrir inventário.</summary>
    public InputAction InventoryAction { get; private set; }
    /// <summary>Usar itens rápidos (atalhos).</summary>
    public InputAction[] UseItemActions { get; private set; } = new InputAction[4];

    // Cache para otimização
    private InputActionMap cachedGameplayMap;
    private bool isInitialized = false;

    // Constantes para evitar string allocations
    private const string GAMEPLAY_MAP_NAME = "Gameplay";
    private const string MOVE_ACTION_NAME = "Move";
    private const string ATTACK_ACTION_NAME = "Attack";
    private const string SPECIAL_ATTACK_ACTION_NAME = "SpecialAttack";
    private const string CROUCH_ACTION_NAME = "Crouch";
    private const string INTERACT_ACTION_NAME = "Interact";
    private const string MENU_ACTION_NAME = "Menu";
    private const string INVENTORY_ACTION_NAME = "Inventory";
    private static readonly string[] USE_ITEM_ACTION_NAMES = { "UseItem1", "UseItem2", "UseItem3", "UseItem4" };

    /// <summary>
    /// Inicializa o Singleton e as ações de input.
    /// </summary>
    private void Awake()
    {
        // Garante que só exista uma instância do InputManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);

        // Inicializa as ações de input
        InitializeActions();
    }

    /// <summary>
    /// Busca e armazena as ações do mapa "Gameplay" do InputActionAsset.
    /// Versão otimizada com cache e validação melhorada.
    /// </summary>
    private void InitializeActions()
    {
        if (inputActions == null)
        {
            Debug.LogError("[InputManager] InputActionAsset não atribuído!");
            return;
        }

        // Cache do mapa Gameplay
        cachedGameplayMap = inputActions.FindActionMap(GAMEPLAY_MAP_NAME, true);
        if (cachedGameplayMap == null)
        {
            Debug.LogError($"[InputManager] Mapa '{GAMEPLAY_MAP_NAME}' não encontrado!");
            return;
        }

        // Inicializa ações principais com cache
        MoveAction = cachedGameplayMap.FindAction(MOVE_ACTION_NAME, true);
        AttackAction = cachedGameplayMap.FindAction(ATTACK_ACTION_NAME, true);
        SpecialAttackAction = cachedGameplayMap.FindAction(SPECIAL_ATTACK_ACTION_NAME, true);
        CrouchAction = cachedGameplayMap.FindAction(CROUCH_ACTION_NAME, true);
        InteractAction = cachedGameplayMap.FindAction(INTERACT_ACTION_NAME, true);
        MenuAction = cachedGameplayMap.FindAction(MENU_ACTION_NAME, true);
        InventoryAction = cachedGameplayMap.FindAction(INVENTORY_ACTION_NAME, true);

        // Itens rápidos usando loop otimizado
        for (int i = 0; i < USE_ITEM_ACTION_NAMES.Length; i++)
        {
            UseItemActions[i] = cachedGameplayMap.FindAction(USE_ITEM_ACTION_NAMES[i], false);
        }

        isInitialized = true;
        Debug.Log("[InputManager] Ações de input inicializadas com sucesso");
    }

    /// <summary>
    /// Ativa todos os inputs ao habilitar o objeto.
    /// </summary>
    private void OnEnable()
    {
        if (isInitialized)
        {
            inputActions?.Enable();
        }
    }

    /// <summary>
    /// Desativa todos os inputs ao desabilitar o objeto.
    /// </summary>
    private void OnDisable()
    {
        inputActions?.Disable();
    }

    /// <summary>
    /// Ativa ou desativa todos os inputs do jogador.
    /// </summary>
    /// <param name="enabled">Se true, ativa os inputs; se false, desativa.</param>
    public void SetInputEnabled(bool enabled)
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[InputManager] Tentando alterar estado antes da inicialização");
            return;
        }

        if (enabled)
            inputActions?.Enable();
        else
            inputActions?.Disable();
    }

    /// <summary>
    /// Verifica se o InputManager está devidamente inicializado
    /// </summary>
    public bool IsInitialized => isInitialized;

    /// <summary>
    /// Força reinicialização das ações (útil para debugging)
    /// </summary>
    [ContextMenu("Reinitialize Actions")]
    public void ReinitializeActions()
    {
        isInitialized = false;
        InitializeActions();
    }
}
