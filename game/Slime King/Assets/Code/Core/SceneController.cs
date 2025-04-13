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

    private AsyncOperation preloadOperation;

    private void Start()
    {
        if (preLoadOnStart && !string.IsNullOrEmpty(preloadSceneName))
        {
            PreLoadScene();
        }
    }

    private void Update()
    {
        if (loadOnAction && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (preloadOperation != null)
            {
                preloadOperation.allowSceneActivation = true;
            }
            else
            {
                LoadConfiguredScene();
            }
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