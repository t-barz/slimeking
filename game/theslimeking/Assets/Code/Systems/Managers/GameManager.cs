using SlimeKing.Core;
using UnityEngine;
using System.Collections; // Necessário para IEnumerator em corrotinas
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // para limpeza de múltiplos EventSystems
using UnityEngine.Rendering.Universal; // para Light2D (URP 2D)

public class GameManager : ManagerSingleton<GameManager>
{
    // Referência à operação de carregamento assíncrono da cena pré-carregada
    private AsyncOperation preloadedSceneOperation;
    private string preloadedSceneName;
    private Coroutine pendingActivationCoroutine;
    public event System.Action<string> OnPreloadedSceneActivated; // evento disparado após ativação

    // Propriedades de consulta simples (KISS)
    public string PreloadedSceneName => preloadedSceneName;
    public bool IsPreloadReady => preloadedSceneOperation != null && preloadedSceneOperation.progress >= 0.9f;
    public bool HasPreloadedScene(string sceneName) => preloadedSceneOperation != null && preloadedSceneName == sceneName;

    // Inicialização mínima seguindo KISS: define estado inicial e aplica configurações básicas de runtime.
    protected override void Initialize()
    {
        // Configurações básicas do runtime (podem ser ajustadas depois por outros managers)
        Application.targetFrameRate = 60; // manter consistente
        Time.timeScale = 1f;              // garantir tempo normal

        // Garante que CameraManager seja inicializado
        if (CameraManager.HasInstance)
        {
            CameraManager.Instance.OnSceneLoaded();
        }

        // Estado inicial simples (usar GameState se definido em enums do projeto)
        // Como este GameManager está reduzido, apenas registra o bootstrap.
        Log("GameManager bootstrap concluído");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    #region Scene Preload API

    /// <summary>
    /// OBSOLETO: Pré-carregamento não é mais suportado com LoadSceneMode.Single.
    /// Este método foi mantido para compatibilidade mas não faz pré-carregamento real.
    /// A cena será carregada normalmente quando ActivatePreloadedScene() for chamado.
    /// </summary>
    /// <param name="sceneName">Nome da cena a pré-carregar.</param>
    public void PreloadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Log("PreloadScene chamado com nome inválido.");
            return;
        }

        // Apenas armazena o nome da cena para carregar depois
        if (preloadedSceneName == sceneName)
        {
            Log($"Cena '{sceneName}' já está marcada para carregamento.");
            return;
        }

        Log($"Cena '{sceneName}' marcada para carregamento (pré-carregamento não suportado com LoadSceneMode.Single).");
        preloadedSceneName = sceneName;
        // Não faz carregamento real aqui - será feito em ActivatePreloadedScene()
    }

    /// <summary>
    /// Carrega e ativa a cena marcada para carregamento usando LoadSceneMode.Single.
    /// Isso substitui todas as cenas anteriores automaticamente.
    /// </summary>
    public void ActivatePreloadedScene(System.Action onActivated = null)
    {
        if (string.IsNullOrEmpty(preloadedSceneName))
        {
            Log("Nenhuma cena marcada para carregamento.");
            return;
        }

        // Se já preparando para ativar, não duplicar
        if (pendingActivationCoroutine != null)
        {
            Log("Ativação já pendente; ignorando chamada duplicada.");
            return;
        }

        Log($"Carregando cena '{preloadedSceneName}' usando LoadSceneMode.Single...");
        pendingActivationCoroutine = StartCoroutine(LoadAndActivateScene(onActivated));
    }

    /// <summary>
    /// Coroutine que carrega a cena usando LoadSceneMode.Single.
    /// Isso automaticamente descarrega todas as cenas anteriores.
    /// </summary>
    private IEnumerator LoadAndActivateScene(System.Action onActivated)
    {
        string sceneToLoad = preloadedSceneName;

        // Carrega a cena usando Single mode (descarrega tudo automaticamente)
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);

        if (loadOperation == null)
        {
            Log($"Falha ao iniciar carregamento da cena '{sceneToLoad}'.");
            yield break;
        }

        // Aguarda carregamento completo
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        Log($"Cena '{sceneToLoad}' carregada com sucesso usando LoadSceneMode.Single.");

        // Atualiza configurações de câmera após carregamento da cena
        if (CameraManager.HasInstance)
        {
            CameraManager.Instance.OnSceneLoaded();
        }

        // Dispara eventos
        OnPreloadedSceneActivated?.Invoke(sceneToLoad);
        onActivated?.Invoke();

        // Limpa referências internas
        preloadedSceneOperation = null;
        preloadedSceneName = null;
        pendingActivationCoroutine = null;
    }

    #endregion

    #region Cleanup Helpers (OBSOLETO com LoadSceneMode.Single)
    // NOTA: Estes métodos não são mais necessários com LoadSceneMode.Single
    // pois o Unity automaticamente descarrega todas as cenas anteriores.
    // Mantidos apenas para compatibilidade caso algum código antigo os chame.
    
    /// <summary>
    /// OBSOLETO: Não é mais necessário com LoadSceneMode.Single.
    /// </summary>
    private void CleanupDuplicateEventSystems()
    {
        // Não faz nada - LoadSceneMode.Single já garante que não há duplicados
    }

    /// <summary>
    /// OBSOLETO: Não é mais necessário com LoadSceneMode.Single.
    /// </summary>
    private void CleanupDuplicateGlobalLights()
    {
        // Não faz nada - LoadSceneMode.Single já garante que não há duplicados
    }
    #endregion
}
