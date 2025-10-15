namespace PixeLadder.EasyTransition
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    /// <summary>
    /// A singleton manager that controls the entire scene transition process.
    /// </summary>
    public class SceneTransitioner : MonoBehaviour
    {
        public static SceneTransitioner Instance;

        [Header("Configuration")]
        [Tooltip("The screen-covering Image prefab used for transitions.")]
        [SerializeField] private Image transitionImagePrefab;
        [Tooltip("The default transition effect to use if none is provided in the LoadScene call.")]
        [SerializeField] private TransitionEffect defaultTransition;

        [Header("Debug")]
        [Tooltip("Enable debug logs for troubleshooting.")]
        [SerializeField] private bool enableDebugLogs = false;

        #region Private State
        private Image transitionImageInstance;
        private Material currentMaterialInstance;
        #endregion

        #region Events
        public static event System.Action OnSceneLoaded;
        #endregion

        #region Singleton
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            // Create a dedicated, persistent canvas for the transition UI.
            GameObject canvasGO = new GameObject("TransitionCanvas");
            canvasGO.transform.SetParent(this.transform);
            var transitionCanvas = canvasGO.AddComponent<Canvas>();
            transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            transitionCanvas.sortingOrder = 999;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            transitionImageInstance = Instantiate(transitionImagePrefab, canvasGO.transform);
            transitionImageInstance.gameObject.SetActive(false);

            if (enableDebugLogs)
                Debug.Log("SceneTransitioner initialized successfully.");
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// The main public method to start a scene transition.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="effect">The TransitionEffect ScriptableObject defining the visuals.</param>
        public void LoadScene(string sceneName, TransitionEffect effect = null)
        {
            var effectToUse = effect ?? defaultTransition;
            if (effectToUse == null)
            {
                Debug.LogError("SceneTransitioner: No transition effect specified and no default is set.", this);
                return;
            }

            if (enableDebugLogs)
                Debug.Log($"Starting scene transition to '{sceneName}' using effect: {effectToUse.name}");

            StopAllCoroutines();
            StartCoroutine(TransitionRoutine(sceneName, effectToUse));
        }

        /// <summary>
        /// Executa a transição visual (fade out → ativação cena já pré-carregada → fade in).
        /// Útil para integrar com um fluxo de pré-carregamento additivo onde a cena já está em memória.
        /// </summary>
        /// <param name="sceneName">Nome da cena pré-carregada que será ativada.</param>
        /// <param name="allowActivation">Callback que libera a ativação (ex: GameManager.ActivatePreloadedScene).</param>
        /// <param name="effect">Efeito de transição opcional (usa default se nulo).</param>
        public void ActivatePreloadedWithTransition(string sceneName, System.Action allowActivation, TransitionEffect effect = null)
        {
            var effectToUse = effect ?? defaultTransition;
            if (effectToUse == null)
            {
                Debug.LogError("SceneTransitioner: No transition effect specified and no default is set.", this);
                // fallback direto sem transição
                allowActivation?.Invoke();
                return;
            }

            if (enableDebugLogs)
                Debug.Log($"Starting preloaded activation transition for '{sceneName}' using effect: {effectToUse.name}");

            StopAllCoroutines();
            StartCoroutine(PreloadedActivationRoutine(sceneName, allowActivation, effectToUse));
        }
        #endregion

        #region Private Methods
        private IEnumerator TransitionRoutine(string sceneName, TransitionEffect effect)
        {
            // Clean up any previous material instance to prevent caching issues
            CleanupMaterial();

            // Prepare the material and activate the image.
            transitionImageInstance.gameObject.SetActive(true);
            currentMaterialInstance = new Material(effect.transitionMaterial);
            transitionImageInstance.material = currentMaterialInstance;
            effect.SetEffectProperties(currentMaterialInstance);

            if (enableDebugLogs)
                Debug.Log($"Material applied: {currentMaterialInstance.name}, Shader: {currentMaterialInstance.shader.name}");

            // Run the fade-out animation.
            yield return effect.AnimateOut(transitionImageInstance);

            // Load the new scene.
            yield return SceneManager.LoadSceneAsync(sceneName);

            // Fire the event to notify any listeners that the load is complete.
            OnSceneLoaded?.Invoke();

            // Run the fade-in animation.
            yield return effect.AnimateIn(transitionImageInstance);

            // Deactivate the image and clean up material.
            transitionImageInstance.gameObject.SetActive(false);
            CleanupMaterial();
        }

        /// <summary>
        /// Rotina de transição para cena já pré-carregada (não chama LoadSceneAsync pois já está carregada additivamente).
        /// </summary>
        private IEnumerator PreloadedActivationRoutine(string sceneName, System.Action allowActivation, TransitionEffect effect)
        {
            // Limpa material anterior e prepara imagem
            CleanupMaterial();
            transitionImageInstance.gameObject.SetActive(true);
            currentMaterialInstance = new Material(effect.transitionMaterial);
            transitionImageInstance.material = currentMaterialInstance;
            effect.SetEffectProperties(currentMaterialInstance);

            if (enableDebugLogs)
                Debug.Log($"PreloadedActivationRoutine: material preparado para '{sceneName}'.");

            // Fade Out (cobre a tela)
            yield return effect.AnimateOut(transitionImageInstance);

            // Libera ativação da cena pré-carregada
            allowActivation?.Invoke();

            // Aguarda até a cena estar ativa
            while (true)
            {
                var targetScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
                if (targetScene.IsValid() && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == sceneName)
                {
                    break;
                }
                yield return null;
            }

            // Dispara evento de cena carregada (mantém consistência com LoadScene)
            OnSceneLoaded?.Invoke();

            // Fade In (descobre a tela)
            yield return effect.AnimateIn(transitionImageInstance);

            transitionImageInstance.gameObject.SetActive(false);
            CleanupMaterial();
        }

        private void CleanupMaterial()
        {
            if (currentMaterialInstance != null)
            {
                if (enableDebugLogs)
                    Debug.Log("Cleaning up previous material instance.");

                // Reset the image material to null first
                if (transitionImageInstance != null)
                    transitionImageInstance.material = null;

                // Destroy the material instance to free memory
                DestroyImmediate(currentMaterialInstance);
                currentMaterialInstance = null;
            }
        }

        private void OnDestroy()
        {
            CleanupMaterial();
        }
        #endregion
    }
}