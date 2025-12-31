using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace PixeLadder.EasyTransition
{
    /// <summary>
    /// Gerenciador de transições entre cenas.
    /// Esta é uma implementação básica para resolver dependências de compilação.
    /// </summary>
    public class SceneTransitioner : MonoBehaviour
    {
        private static SceneTransitioner instance;
        
        [Header("Transition UI")]
        [SerializeField] private Canvas transitionCanvas;
        [SerializeField] private Image transitionImageInstance;
        
        /// <summary>
        /// Instância singleton do SceneTransitioner
        /// </summary>
        public static SceneTransitioner Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SceneTransitioner>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SceneTransitioner");
                        instance = go.AddComponent<SceneTransitioner>();
                        instance.CreateTransitionUI();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                if (transitionImageInstance == null)
                {
                    CreateTransitionUI();
                }
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Cria a UI de transição se não existir
        /// </summary>
        private void CreateTransitionUI()
        {
            // Cria o Canvas de transição
            GameObject canvasGO = new GameObject("TransitionCanvas");
            canvasGO.transform.SetParent(transform);
            
            transitionCanvas = canvasGO.AddComponent<Canvas>();
            transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            transitionCanvas.sortingOrder = 1000; // Sempre no topo
            
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasGO.AddComponent<GraphicRaycaster>();
            
            // Cria a imagem de transição
            GameObject imageGO = new GameObject("TransitionImage");
            imageGO.transform.SetParent(canvasGO.transform, false);
            
            transitionImageInstance = imageGO.AddComponent<Image>();
            transitionImageInstance.color = Color.black;
            
            // Configura para ocupar toda a tela
            RectTransform rectTransform = transitionImageInstance.rectTransform;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            // Inicia desativada
            transitionImageInstance.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Carrega uma cena com efeito de transição
        /// </summary>
        /// <param name="sceneName">Nome da cena a ser carregada</param>
        /// <param name="transitionEffect">Efeito de transição (opcional)</param>
        public void LoadScene(string sceneName, TransitionEffect transitionEffect = null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, transitionEffect));
        }
        
        /// <summary>
        /// Corrotina que executa o carregamento da cena com transição
        /// </summary>
        private IEnumerator LoadSceneCoroutine(string sceneName, TransitionEffect transitionEffect)
        {
            // Garante que a UI de transição existe
            if (transitionImageInstance == null)
            {
                CreateTransitionUI();
            }
            
            // Se há um efeito de transição, executa fade out
            if (transitionEffect != null)
            {
                transitionEffect.Initialize();
                transitionImageInstance.gameObject.SetActive(true);
                
                // Configura o material se disponível
                if (transitionEffect.transitionMaterial != null)
                {
                    Material materialInstance = new Material(transitionEffect.transitionMaterial);
                    transitionImageInstance.material = materialInstance;
                    transitionEffect.SetEffectProperties(materialInstance);
                }
                
                yield return transitionEffect.AnimateOut(transitionImageInstance);
            }
            
            // Carrega a cena
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            // Espera o carregamento completar
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            // Se há um efeito de transição, executa fade in
            if (transitionEffect != null)
            {
                yield return transitionEffect.AnimateIn(transitionImageInstance);
                
                transitionImageInstance.gameObject.SetActive(false);
                transitionEffect.Cleanup();
                
                // Limpa o material instanciado
                if (transitionImageInstance.material != null)
                {
                    DestroyImmediate(transitionImageInstance.material);
                    transitionImageInstance.material = null;
                }
            }
        }
        
        /// <summary>
        /// Verifica se uma cena existe
        /// </summary>
        /// <param name="sceneName">Nome da cena</param>
        /// <returns>True se a cena existe</returns>
        public bool SceneExists(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                if (sceneNameFromPath == sceneName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}