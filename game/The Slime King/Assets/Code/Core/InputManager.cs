using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gerencia os inputs do jogador, centralizando o acesso aos comandos principais do jogo.
/// Permite fácil integração com sistemas de combate, movimentação, inventário e menus.
/// Utiliza o padrão Singleton para acesso global.
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
    /// </summary>
    private void InitializeActions()
    {
        if (inputActions == null)
        {
            Debug.LogError("InputActionAsset não atribuído no InputManager!");
            return;
        }

        var gameplayMap = inputActions.FindActionMap("Gameplay", true);

        MoveAction = gameplayMap.FindAction("Move", true);
        AttackAction = gameplayMap.FindAction("Attack", true);
        SpecialAttackAction = gameplayMap.FindAction("SpecialAttack", true);
        CrouchAction = gameplayMap.FindAction("Crouch", true);
        InteractAction = gameplayMap.FindAction("Interact", true);
        MenuAction = gameplayMap.FindAction("Menu", true);
        InventoryAction = gameplayMap.FindAction("Inventory", true);

        // Itens rápidos (LB, LT, RB, RT)
        UseItemActions[0] = gameplayMap.FindAction("UseItem1", false);
        UseItemActions[1] = gameplayMap.FindAction("UseItem2", false);
        UseItemActions[2] = gameplayMap.FindAction("UseItem3", false);
        UseItemActions[3] = gameplayMap.FindAction("UseItem4", false);
    }

    /// <summary>
    /// Ativa todos os inputs ao habilitar o objeto.
    /// </summary>
    private void OnEnable()
    {
        inputActions?.Enable();
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
        if (enabled)
            inputActions?.Enable();
        else
            inputActions?.Disable();
    }
}
