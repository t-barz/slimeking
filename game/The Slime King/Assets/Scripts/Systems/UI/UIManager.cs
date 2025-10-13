using System.Collections;
using UnityEngine;
using PixeLadder.EasyTransition;

namespace SlimeKing.Systems.UI
{
    /// <summary>
    /// Manager global para controle de UI e transições.
    /// Singleton reutilizável em todo o jogo para operações de interface.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Transition Settings")]
        [SerializeField] private TransitionEffect fadeTransition;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        #region Singleton
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Log("UIManager initialized");
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        #region Scene Transitions
        /// <summary>
        /// Faz transição para a cena do jogo principal
        /// </summary>
        public void TransitionToGame()
        {
            Log("Transitioning to game scene");

            if (fadeTransition != null && SceneTransitioner.Instance != null)
            {
                SceneTransitioner.Instance.LoadScene("GameScene", fadeTransition);
            }
            else
            {
                // Fallback - carrega cena diretamente
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
        }        /// <summary>
                 /// Faz transição para o menu principal
                 /// </summary>
        public void TransitionToMainMenu()
        {
            Log("Transitioning to main menu");

            if (fadeTransition != null && SceneTransitioner.Instance != null)
            {
                SceneTransitioner.Instance.LoadScene("TitleScreen", fadeTransition);
            }
            else
            {
                // Fallback - carrega cena diretamente
                UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScreen");
            }
        }
        #endregion

        #region Fade Utilities
        /// <summary>
        /// Faz fade em um CanvasGroup de forma otimizada
        /// </summary>
        /// <param name="group">CanvasGroup para fazer fade</param>
        /// <param name="targetAlpha">Alpha de destino (0-1)</param>
        /// <param name="duration">Duração do fade em segundos</param>
        public IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration)
        {
            if (group == null) yield break;

            float startAlpha = group.alpha;
            float elapsedTime = 0f;

            // Durante o fade, bloqueia interações
            group.interactable = false;
            group.blocksRaycasts = false;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                yield return null;
            }

            group.alpha = targetAlpha;

            // Reabilita interações se visível
            if (targetAlpha > 0f)
            {
                group.interactable = true;
                group.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// Faz fade em um GameObject (UI Images e SpriteRenderers)
        /// </summary>
        /// <param name="obj">GameObject para fazer fade</param>
        /// <param name="targetAlpha">Alpha de destino (0-1)</param>
        /// <param name="duration">Duração do fade em segundos</param>
        public IEnumerator FadeGameObject(GameObject obj, float targetAlpha, float duration)
        {
            if (obj == null) yield break;

            SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
            UnityEngine.UI.Image[] images = obj.GetComponentsInChildren<UnityEngine.UI.Image>();
            TMPro.TextMeshProUGUI[] texts = obj.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

            float[] startAlphas = new float[spriteRenderers.Length + images.Length + texts.Length];
            int index = 0;

            // Guarda valores iniciais
            foreach (var sr in spriteRenderers)
            {
                startAlphas[index] = sr.color.a;
                index++;
            }
            foreach (var img in images)
            {
                startAlphas[index] = img.color.a;
                index++;
            }
            foreach (var text in texts)
            {
                startAlphas[index] = text.color.a;
                index++;
            }

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / duration;
                index = 0;

                // Aplica fade nos SpriteRenderers
                foreach (var sr in spriteRenderers)
                {
                    Color color = sr.color;
                    color.a = Mathf.Lerp(startAlphas[index], targetAlpha, progress);
                    sr.color = color;
                    index++;
                }

                // Aplica fade nas Images
                foreach (var img in images)
                {
                    Color color = img.color;
                    color.a = Mathf.Lerp(startAlphas[index], targetAlpha, progress);
                    img.color = color;
                    index++;
                }

                // Aplica fade nos TextMeshPro
                foreach (var text in texts)
                {
                    Color color = text.color;
                    color.a = Mathf.Lerp(startAlphas[index], targetAlpha, progress);
                    text.color = color;
                    index++;
                }

                yield return null;
            }

            // Define valores finais
            index = 0;
            foreach (var sr in spriteRenderers)
            {
                Color color = sr.color;
                color.a = targetAlpha;
                sr.color = color;
                index++;
            }
            foreach (var img in images)
            {
                Color color = img.color;
                color.a = targetAlpha;
                img.color = color;
                index++;
            }
            foreach (var text in texts)
            {
                Color color = text.color;
                color.a = targetAlpha;
                text.color = color;
                index++;
            }
        }
        #endregion

        #region Debug
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
        private void Log(string message)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[UIManager] {message}");
            }
        }
        #endregion
    }
}