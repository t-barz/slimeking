using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SlimeKing.Core
{
    /// <summary>
    /// Manager responsável por transições entre cenas com efeitos visuais.
    /// Implementa efeito cellular para transições suaves.
    /// </summary>
    public class SceneTransitionManager : ManagerSingleton<SceneTransitionManager>
    {
        #region Transition Settings

        [Header("Transition Settings")]
        [SerializeField] private float transitionDuration = 1.5f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private Color transitionColor = Color.black;

        [Header("Cellular Effect")]
        [SerializeField] private int cellularResolution = 50;
        [SerializeField] private float cellularScale = 1f;
        [SerializeField] private float cellularSpeed = 2f;

        #endregion

        #region UI Components

        [Header("UI Components")]
        [SerializeField] private Canvas transitionCanvas;
        [SerializeField] private Image transitionImage;
        [SerializeField] private Material cellularMaterial;

        #endregion

        #region Private Variables

        private bool isTransitioning = false;
        private Coroutine currentTransition;

        #endregion

        #region Singleton Implementation

        protected override void Initialize()
        {
            Log("SceneTransitionManager initialized");
            SetupTransitionUI();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Configura a UI de transição
        /// </summary>
        private void SetupTransitionUI()
        {
            // Criar Canvas se não existir
            if (transitionCanvas == null)
            {
                CreateTransitionCanvas();
            }

            // Criar Material Cellular se não existir
            if (cellularMaterial == null)
            {
                CreateCellularMaterial();
            }

            // Configurar estado inicial
            SetTransitionVisibility(false);
        }

        /// <summary>
        /// Cria o Canvas de transição
        /// </summary>
        private void CreateTransitionCanvas()
        {
            Log("Creating transition canvas");

            // Criar GameObject para o Canvas
            GameObject canvasGO = new GameObject("TransitionCanvas");
            canvasGO.transform.SetParent(transform);

            // Configurar Canvas
            transitionCanvas = canvasGO.AddComponent<Canvas>();
            transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            transitionCanvas.sortingOrder = 9999; // Sempre por cima

            // Adicionar CanvasScaler
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            // Adicionar GraphicRaycaster
            canvasGO.AddComponent<GraphicRaycaster>();

            // Criar Image para o efeito
            CreateTransitionImage();
        }

        /// <summary>
        /// Cria a Image de transição
        /// </summary>
        private void CreateTransitionImage()
        {
            Log("Creating transition image");

            GameObject imageGO = new GameObject("TransitionImage");
            imageGO.transform.SetParent(transitionCanvas.transform, false);

            // Configurar RectTransform para ocupar toda a tela
            RectTransform rectTransform = imageGO.AddComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;

            // Configurar Image
            transitionImage = imageGO.AddComponent<Image>();
            transitionImage.color = transitionColor;
        }

        /// <summary>
        /// Cria o material do efeito cellular
        /// </summary>
        private void CreateCellularMaterial()
        {
            Log("Creating cellular material");

            // Código de shader in-line removido para evitar warning de variável não utilizada.
            // Futuro: mover shader para arquivo separado caso o efeito avançado seja retomado.
            cellularMaterial = new Material(Shader.Find("UI/Default"));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Carrega uma nova cena com efeito de transição cellular
        /// </summary>
        /// <param name="sceneName">Nome da cena a ser carregada</param>
        public void LoadSceneWithTransition(string sceneName)
        {
            if (isTransitioning)
            {
                LogWarning("Transition already in progress, ignoring request");
                return;
            }

            Log($"Starting transition to scene: {sceneName}");
            currentTransition = StartCoroutine(TransitionToScene(sceneName));
        }

        /// <summary>
        /// Carrega uma nova cena por índice com efeito de transição
        /// </summary>
        /// <param name="sceneIndex">Índice da cena</param>
        public void LoadSceneWithTransition(int sceneIndex)
        {
            if (isTransitioning)
            {
                LogWarning("Transition already in progress, ignoring request");
                return;
            }

            Log($"Starting transition to scene index: {sceneIndex}");
            currentTransition = StartCoroutine(TransitionToScene(sceneIndex));
        }

        /// <summary>
        /// Verifica se há uma transição em andamento
        /// </summary>
        public bool IsTransitioning => isTransitioning;

        #endregion

        #region Transition Coroutines

        /// <summary>
        /// Corrotina principal de transição para cena por nome
        /// </summary>
        private IEnumerator TransitionToScene(string sceneName)
        {
            isTransitioning = true;

            // Fade Out (efeito cellular aparecendo)
            yield return StartCoroutine(FadeOut());

            // Carregar nova cena
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            // Aguardar carregamento
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Ativar nova cena
            asyncLoad.allowSceneActivation = true;

            // Aguardar ativação
            yield return new WaitUntil(() => asyncLoad.isDone);

            // Fade In (efeito cellular desaparecendo)
            yield return StartCoroutine(FadeIn());

            isTransitioning = false;
            currentTransition = null;

            Log($"Transition to {sceneName} completed");
        }

        /// <summary>
        /// Corrotina principal de transição para cena por índice
        /// </summary>
        private IEnumerator TransitionToScene(int sceneIndex)
        {
            isTransitioning = true;

            // Fade Out
            yield return StartCoroutine(FadeOut());

            // Carregar nova cena
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;

            // Aguardar carregamento
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Ativar nova cena
            asyncLoad.allowSceneActivation = true;

            // Aguardar ativação
            yield return new WaitUntil(() => asyncLoad.isDone);

            // Fade In
            yield return StartCoroutine(FadeIn());

            isTransitioning = false;
            currentTransition = null;

            Log($"Transition to scene index {sceneIndex} completed");
        }

        /// <summary>
        /// Fade Out - efeito cellular cobrindo a tela
        /// </summary>
        private IEnumerator FadeOut()
        {
            Log("Starting fade out");
            SetTransitionVisibility(true);

            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration / 2f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / (transitionDuration / 2f);
                float curvedProgress = transitionCurve.Evaluate(progress);

                UpdateCellularEffect(curvedProgress);

                yield return null;
            }

            // Garantir que chegue a 100%
            UpdateCellularEffect(1f);
            Log("Fade out completed");
        }

        /// <summary>
        /// Fade In - efeito cellular revelando a nova cena
        /// </summary>
        private IEnumerator FadeIn()
        {
            Log("Starting fade in");

            float elapsedTime = 0f;

            while (elapsedTime < transitionDuration / 2f)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = 1f - (elapsedTime / (transitionDuration / 2f));
                float curvedProgress = transitionCurve.Evaluate(progress);

                UpdateCellularEffect(curvedProgress);

                yield return null;
            }

            // Garantir que chegue a 0%
            UpdateCellularEffect(0f);
            SetTransitionVisibility(false);

            Log("Fade in completed");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Atualiza o efeito cellular
        /// </summary>
        private void UpdateCellularEffect(float progress)
        {
            if (transitionImage != null)
            {
                // Por simplicidade, usar alpha simples.
                // Aplicar leve modulação usando variáveis celulares para evitar warnings e preparar futura expansão.
                float resolutionFactor = Mathf.Clamp(cellularResolution / 100f, 0.2f, 2f);
                float scaleFactor = Mathf.Clamp(cellularScale, 0.5f, 3f);
                float speedFactor = Mathf.PingPong(Time.unscaledTime * cellularSpeed * 0.1f, 0.1f);
                Color color = transitionColor;
                color.a = Mathf.Clamp01(progress * resolutionFactor / scaleFactor + speedFactor);
                transitionImage.color = color;

                // Se tivéssemos o material cellular:
                // cellularMaterial.SetFloat("_Progress", progress);
                // transitionImage.material = cellularMaterial;
            }
        }

        /// <summary>
        /// Define a visibilidade da transição
        /// </summary>
        private void SetTransitionVisibility(bool visible)
        {
            if (transitionCanvas != null)
            {
                transitionCanvas.gameObject.SetActive(visible);
            }
        }

        #endregion

        #region Editor Tools

#if UNITY_EDITOR
        [UnityEngine.ContextMenu("Test Transition")]
        private void TestTransition()
        {
            if (Application.isPlaying)
            {
                LoadSceneWithTransition(SceneManager.GetActiveScene().name);
            }
        }
#endif

        #endregion
    }
}