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
    /// Pré-carrega uma cena pelo nome sem ativá-la (carregamento assíncrono additivo).
    /// Se já existir uma cena pré-carregada diferente, cancela referência anterior.
    /// </summary>
    /// <param name="sceneName">Nome da cena a pré-carregar.</param>
    public void PreloadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Log("PreloadScene chamado com nome inválido.");
            return;
        }

        // Se já temos uma operação em andamento para outra cena, apenas substituímos a referência (Unity continuará carregando a anterior; otimização futura pode descarregar se necessário)
        if (preloadedSceneOperation != null && preloadedSceneName == sceneName)
        {
            Log($"Cena '{sceneName}' já está em pré-carregamento.");
            return;
        }

        Log($"Iniciando pré-carregamento da cena '{sceneName}'.");
        preloadedSceneName = sceneName;
        preloadedSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        preloadedSceneOperation.allowSceneActivation = false; // impedir ativação automática
    }

    /// <summary>
    /// Ativa a cena previamente pré-carregada. Se ainda não pronta (progress < 0.9f), espera e ativa assim que disponível.
    /// </summary>
    public void ActivatePreloadedScene(System.Action onActivated = null)
    {
        if (preloadedSceneOperation == null)
        {
            Log("Nenhuma cena pré-carregada para ativar.");
            return;
        }

        // Se já preparando para ativar, não duplicar
        if (pendingActivationCoroutine != null)
        {
            Log("Ativação já pendente; ignorando chamada duplicada.");
            return;
        }

        // Se já pronta, ativar imediatamente
        if (preloadedSceneOperation.progress >= 0.9f)
        {
            Log($"Ativando cena pré-carregada '{preloadedSceneName}' imediatamente.");
            preloadedSceneOperation.allowSceneActivation = true;
            StartCoroutine(FinalizeActivation(onActivated));
        }
        else
        {
            Log($"Cena '{preloadedSceneName}' ainda carregando (progress={preloadedSceneOperation.progress:F2}). Aguardando para ativar.");
            pendingActivationCoroutine = StartCoroutine(WaitAndActivatePreloaded(onActivated));
        }
    }

    /// <summary>
    /// Coroutine que espera o carregamento atingir o estágio de ativação e então libera a cena.
    /// </summary>
    private IEnumerator WaitAndActivatePreloaded(System.Action onActivated)
    {
        // Espera até a operação atingir ~90% (Unity bloqueia em 0.9 até allowSceneActivation=true)
        while (preloadedSceneOperation != null && preloadedSceneOperation.progress < 0.9f)
        {
            yield return null; // aguarda próximo frame
        }

        if (preloadedSceneOperation != null)
        {
            Log($"Progresso atingiu {preloadedSceneOperation.progress:F2}; ativando cena '{preloadedSceneName}'.");
            preloadedSceneOperation.allowSceneActivation = true;
        }
        yield return StartCoroutine(FinalizeActivation(onActivated));
    }

    /// <summary>
    /// Finaliza ativação: aguarda término, ajusta cena ativa, dispara eventos e limpa referências.
    /// </summary>
    private IEnumerator FinalizeActivation(System.Action onActivated)
    {
        while (preloadedSceneOperation != null && !preloadedSceneOperation.isDone)
        {
            yield return null;
        }

        // Ajusta cena ativa se válida
        if (!string.IsNullOrEmpty(preloadedSceneName))
        {
            var loadedScene = SceneManager.GetSceneByName(preloadedSceneName);
            if (loadedScene.IsValid())
            {
                SceneManager.SetActiveScene(loadedScene);
                // Limpa artefatos de carregamento simultâneo (EventSystems e Global Light2D)
                CleanupDuplicateEventSystems();
                CleanupDuplicateGlobalLights();

                // Atualiza configurações de câmera após ativação da cena
                if (CameraManager.HasInstance)
                {
                    CameraManager.Instance.OnSceneLoaded();
                }
            }
            OnPreloadedSceneActivated?.Invoke(preloadedSceneName);
            onActivated?.Invoke();
        }

        // Limpa referências internas
        preloadedSceneOperation = null;
        preloadedSceneName = null;
        if (pendingActivationCoroutine != null)
        {
            StopCoroutine(pendingActivationCoroutine);
            pendingActivationCoroutine = null;
        }
    }

    #endregion

    #region Cleanup Helpers
    /// <summary>
    /// Garante que exista apenas um EventSystem ativo (remove outros de cenas antigas).
    /// </summary>
    private void CleanupDuplicateEventSystems()
    {
        StartCoroutine(CleanupEventSystemsDeferred());
    }

    private IEnumerator CleanupEventSystemsDeferred()
    {
        yield return null; // espera um frame para permitir que objetos da cena ativa inicializem

        var allEventSystems = Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        if (allEventSystems.Length <= 1) yield break;

        var activeScene = SceneManager.GetActiveScene();
        EventSystem keep = null;
        foreach (var es in allEventSystems)
        {
            if (keep == null && es.gameObject.scene == activeScene)
            {
                keep = es;
                continue;
            }
        }
        if (keep == null) keep = allEventSystems[0];

        int removed = 0;
        foreach (var es in allEventSystems)
        {
            if (es == keep) continue;
            es.enabled = false;
            // Usa Destroy em vez de DestroyImmediate para segurança
            Object.Destroy(es.gameObject);
            removed++;
        }
        Log($"CleanupDuplicateEventSystems: Mantido '{keep.gameObject.name}', removidos {removed} duplicados.");
    }

    /// <summary>
    /// Remove Global Light2D duplicadas (mantém a da cena ativa) para evitar warnings da URP.
    /// </summary>
    private void CleanupDuplicateGlobalLights()
    {
        StartCoroutine(CleanupGlobalLightsDeferred());
    }

    private IEnumerator CleanupGlobalLightsDeferred()
    {
        yield return null; // espera um frame para evitar conflitos de inicialização

        var allLights = Object.FindObjectsByType<Light2D>(FindObjectsSortMode.None);
        var globalLights = System.Array.FindAll(allLights, l => l.lightType == Light2D.LightType.Global);
        if (globalLights.Length <= 1) yield break;

        var activeScene = SceneManager.GetActiveScene();
        Light2D keep = null;
        foreach (var gl in globalLights)
        {
            if (keep == null && gl.gameObject.scene == activeScene)
            {
                keep = gl;
                continue;
            }
        }
        if (keep == null) keep = globalLights[0];

        int removed = 0;
        foreach (var gl in globalLights)
        {
            if (gl == keep) continue;
            gl.enabled = false;
            Object.Destroy(gl.gameObject);
            removed++;
        }
        Log($"CleanupDuplicateGlobalLights: Mantida '{keep.gameObject.name}', removidas {removed} globais duplicadas.");
    }
    #endregion
}
