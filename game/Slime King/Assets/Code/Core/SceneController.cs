using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneController : MonoBehaviour
{
    [Header("Configurações de Scene")]
    [Tooltip("Nome da Scene a ser pré-carregada")]
    [SerializeField] private string preloadSceneName;

    [Tooltip("Pré-carrega a Scene em segundo plano ao iniciar")]
    [SerializeField] private bool preLoadOnStart = false;

    [Tooltip("Permite carregar a Scene ao pressionar o botão de ação")]
    [SerializeField] private bool loadOnAction = false;

    [Header("Input Configuration")]
    [Tooltip("Referência para a ação de ataque que ativa a Scene")]
    [SerializeField] private InputActionReference attackAction;

    private AsyncOperation preloadOperation;

    private void OnEnable()
    {
        if (loadOnAction && attackAction != null)
        {
            attackAction.action.performed += OnAttackPerformed;
            attackAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.action.performed -= OnAttackPerformed;
            attackAction.action.Disable();
        }
    }

    private void Start()
    {
        if (preLoadOnStart && !string.IsNullOrEmpty(preloadSceneName))
        {
            PreLoadScene();
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!loadOnAction) return;

        if (preloadOperation != null)
        {
            preloadOperation.allowSceneActivation = true;
        }
        else
        {
            LoadConfiguredScene();
        }
    }

    /// <summary>
    /// Pré-carrega a Scene configurada em segundo plano.
    /// </summary>
    private void PreLoadScene()
    {
        if (string.IsNullOrEmpty(preloadSceneName))
        {
            Debug.LogError("Scene name not configured!");
            return;
        }

        try
        {
            preloadOperation = SceneManager.LoadSceneAsync(preloadSceneName);
            preloadOperation.allowSceneActivation = false;
            Debug.Log($"Pre-loading scene: {preloadSceneName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error pre-loading scene {preloadSceneName}: {e.Message}");
        }
    }

    /// <summary>
    /// Carrega a Scene configurada de forma assíncrona.
    /// </summary>
    public void LoadConfiguredScene()
    {
        if (string.IsNullOrEmpty(preloadSceneName))
        {
            Debug.LogError("Scene name not configured!");
            return;
        }

        try
        {
            SceneManager.LoadSceneAsync(preloadSceneName);
            Debug.Log($"Loading scene: {preloadSceneName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading scene {preloadSceneName}: {e.Message}");
        }
    }
}