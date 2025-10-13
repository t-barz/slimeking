using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using SlimeKing.Systems.UI;
using SlimeKing.Core;

namespace SlimeKing.Gameplay.UI
{
    /// <summary>
    /// Controller específico para a tela inicial, gerencia a sequência de apresentação
    /// seguindo o padrão: logo empresa → fundo jogo → logo jogo → elementos menu
    /// </summary>
    public class TitleScreenController : MonoBehaviour
    {
        [Header("UI Elements - GameObjects da Cena")]
        [SerializeField] private GameObject companyLogo;        // ui_wslogo
        [SerializeField] private GameObject background;         // ui_background
        [SerializeField] private GameObject gameLogo;          // ui_gamelogo
        [SerializeField] private GameObject companySmallLogo;  // ui_wssmalllogo
        [SerializeField] private GameObject pressStart;        // ui_pressstart

        [Header("Sequence Settings")]
        [SerializeField] private float musicStartDelay = 1f;
        [SerializeField] private float companyLogoVisibleTime = 4f;

        [Header("Fade Durations")]
        [SerializeField] private float companyLogoFadeInDuration = 1f;
        [SerializeField] private float companyLogoFadeOutDuration = 1f;
        [SerializeField] private float backgroundFadeInDuration = 1f;
        [SerializeField] private float gameLogoFadeInDuration = 1f;
        [SerializeField] private float menuElementsFadeInDuration = 1f;

        [Header("Sequence Delays")]
        [SerializeField] private float delayBeforeGameLogo = 0.5f;
        [SerializeField] private float delayBeforeMenuElements = 0.3f;

        [Header("Audio")]
        [SerializeField] private AudioClip titleMusic;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private TitleScreenState currentState = TitleScreenState.Initial;
        private bool canSkip = false;
        private bool hasSkipped = false;
        private bool inputReceived = false;

        #region Unity Lifecycle

        private void Start()
        {
            InitializeScreen();
            StartSequence();
        }

        private void Update()
        {
            // Checa input usando Input System
            CheckForAnyInput();

            if (inputReceived)
            {
                HandleInput();
                inputReceived = false;
            }
        }

        /// <summary>
        /// Detecta qualquer input usando o Input System
        /// </summary>
        private void CheckForAnyInput()
        {
            // Verifica se qualquer dispositivo de input foi pressionado
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                inputReceived = true;
                return;
            }

            if (Mouse.current != null &&
                (Mouse.current.leftButton.wasPressedThisFrame ||
                 Mouse.current.rightButton.wasPressedThisFrame ||
                 Mouse.current.middleButton.wasPressedThisFrame))
            {
                inputReceived = true;
                return;
            }

            // Verifica gamepads conectados
            if (Gamepad.current != null)
            {
                foreach (var control in Gamepad.current.allControls)
                {
                    if (control is UnityEngine.InputSystem.Controls.ButtonControl button && button.wasPressedThisFrame)
                    {
                        inputReceived = true;
                        return;
                    }
                }
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Configura o estado inicial da tela, escondendo todos os elementos
        /// </summary>
        private void InitializeScreen()
        {
            // GameManager não está disponível ainda - implementar quando necessário

            // Esconde todos os elementos inicialmente
            SetGameObjectVisibility(companyLogo, false);
            SetGameObjectVisibility(background, false);
            SetGameObjectVisibility(gameLogo, false);
            SetGameObjectVisibility(companySmallLogo, false);
            SetGameObjectVisibility(pressStart, false);

            currentState = TitleScreenState.Initial;

            Log("Title screen initialized");
        }

        /// <summary>
        /// Define a visibilidade de um GameObject manipulando o alpha dos componentes visuais
        /// </summary>
        /// <param name="obj">GameObject para modificar</param>
        /// <param name="visible">Se deve estar visível ou não</param>
        private void SetGameObjectVisibility(GameObject obj, bool visible)
        {
            if (obj == null) return;

            // Para UI Images
            UnityEngine.UI.Image[] images = obj.GetComponentsInChildren<UnityEngine.UI.Image>();
            foreach (var img in images)
            {
                Color color = img.color;
                color.a = visible ? 1f : 0f;
                img.color = color;
            }

            // Para SpriteRenderers (caso existam)
            SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in spriteRenderers)
            {
                Color color = sr.color;
                color.a = visible ? 1f : 0f;
                sr.color = color;
            }

            // Para TextMeshPro UGUI
            TMPro.TextMeshProUGUI[] texts = obj.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
            foreach (var text in texts)
            {
                Color color = text.color;
                color.a = visible ? 1f : 0f;
                text.color = color;
            }
        }

        #endregion

        #region Sequence Management

        /// <summary>
        /// Inicia a sequência principal da tela inicial
        /// </summary>
        private void StartSequence()
        {
            StartCoroutine(TitleSequence());
        }

        /// <summary>
        /// Coroutine principal que executa toda a sequência da tela inicial
        /// </summary>
        private IEnumerator TitleSequence()
        {
            Log("Starting title sequence");

            // Aguarda tempo inicial e inicia música
            yield return new WaitForSeconds(musicStartDelay);

            // Inicia a música da tela de título se configurada
            if (titleMusic != null)
            {
                AudioManager.Instance.PlayMusic(titleMusic);
                Log("Title music started with fade in");
            }

            // Sequência do logo da empresa
            yield return StartCoroutine(ShowCompanyLogo());
            if (hasSkipped) yield break;

            yield return new WaitForSeconds(companyLogoVisibleTime);
            if (hasSkipped) yield break;

            yield return StartCoroutine(HideCompanyLogo());
            if (hasSkipped) yield break;

            // Sequência dos elementos do jogo
            yield return StartCoroutine(ShowGameElements());
            if (hasSkipped) yield break;

            yield return StartCoroutine(ShowMenuElements());

            currentState = TitleScreenState.MenuReady;
            canSkip = false;

            Log("Title sequence completed");
        }

        /// <summary>
        /// Mostra o logo da empresa com fade
        /// </summary>
        private IEnumerator ShowCompanyLogo()
        {
            currentState = TitleScreenState.CompanyLogo;
            canSkip = true;

            Log("Showing company logo");

            if (UIManager.Instance != null)
            {
                yield return StartCoroutine(
                    UIManager.Instance.FadeGameObject(companyLogo, 1f, companyLogoFadeInDuration)
                );
            }
            else
            {
                // Fallback - fade manual
                yield return StartCoroutine(FadeGameObjectManual(companyLogo, 1f, companyLogoFadeInDuration));
            }
        }

        /// <summary>
        /// Esconde o logo da empresa com fade
        /// </summary>
        private IEnumerator HideCompanyLogo()
        {
            currentState = TitleScreenState.Transition;

            Log("Hiding company logo");

            if (UIManager.Instance != null)
            {
                yield return StartCoroutine(
                    UIManager.Instance.FadeGameObject(companyLogo, 0f, companyLogoFadeOutDuration)
                );
            }
            else
            {
                // Fallback - fade manual
                yield return StartCoroutine(FadeGameObjectManual(companyLogo, 0f, companyLogoFadeOutDuration));
            }
        }

        /// <summary>
        /// Mostra o fundo e logo do jogo sequencialmente
        /// </summary>
        private IEnumerator ShowGameElements()
        {
            currentState = TitleScreenState.GameTitle;

            Log("Showing game background");

            // Primeiro mostra o background completamente
            if (UIManager.Instance != null)
            {
                yield return StartCoroutine(
                    UIManager.Instance.FadeGameObject(background, 1f, backgroundFadeInDuration)
                );
            }
            else
            {
                // Fallback - fade manual
                yield return StartCoroutine(FadeGameObjectManual(background, 1f, backgroundFadeInDuration));
            }

            // Aguarda delay antes de mostrar o logo do jogo
            yield return new WaitForSeconds(delayBeforeGameLogo);

            Log("Showing game logo");

            // Depois mostra o logo do jogo
            if (UIManager.Instance != null)
            {
                yield return StartCoroutine(
                    UIManager.Instance.FadeGameObject(gameLogo, 1f, gameLogoFadeInDuration)
                );
            }
            else
            {
                // Fallback - fade manual
                yield return StartCoroutine(FadeGameObjectManual(gameLogo, 1f, gameLogoFadeInDuration));
            }
        }

        /// <summary>
        /// Mostra os elementos finais do menu (logo pequeno + press start)
        /// </summary>
        private IEnumerator ShowMenuElements()
        {
            Log("Showing menu elements");

            // Aguarda delay antes de mostrar elementos do menu
            yield return new WaitForSeconds(delayBeforeMenuElements);

            if (UIManager.Instance != null)
            {
                // Mostra logo pequeno da empresa e prompt simultaneamente
                StartCoroutine(UIManager.Instance.FadeGameObject(companySmallLogo, 1f, menuElementsFadeInDuration));
                yield return StartCoroutine(
                    UIManager.Instance.FadeGameObject(pressStart, 1f, menuElementsFadeInDuration)
                );
            }
            else
            {
                // Fallback - fade manual
                StartCoroutine(FadeGameObjectManual(companySmallLogo, 1f, menuElementsFadeInDuration));
                yield return StartCoroutine(FadeGameObjectManual(pressStart, 1f, menuElementsFadeInDuration));
            }
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Processa input do usuário (pular sequência ou iniciar jogo)
        /// </summary>
        private void HandleInput()
        {
            if (canSkip && !hasSkipped && currentState != TitleScreenState.MenuReady)
            {
                SkipToMenu();
            }
            else if (currentState == TitleScreenState.MenuReady)
            {
                StartGame();
            }
        }

        /// <summary>
        /// Pula a sequência atual direto para o menu
        /// </summary>
        private void SkipToMenu()
        {
            hasSkipped = true;
            canSkip = false;

            Log("Skipping to menu");

            StopAllCoroutines();
            StartCoroutine(SkipSequence());
        }

        /// <summary>
        /// Sequência rápida para mostrar o menu final quando pulado
        /// </summary>
        private IEnumerator SkipSequence()
        {
            // Esconde logo da empresa se estiver visível
            if (currentState == TitleScreenState.CompanyLogo)
            {
                SetGameObjectVisibility(companyLogo, false);
            }

            // Mostra elementos do jogo rapidamente
            SetGameObjectVisibility(background, true);
            SetGameObjectVisibility(gameLogo, true);

            yield return new WaitForSeconds(0.1f);

            SetGameObjectVisibility(companySmallLogo, true);
            SetGameObjectVisibility(pressStart, true);

            currentState = TitleScreenState.MenuReady;

            Log("Skip sequence completed");
        }

        /// <summary>
        /// Inicia o jogo principal através do UIManager
        /// </summary>
        private void StartGame()
        {
            Log("Starting game");

            if (UIManager.Instance != null)
            {
                UIManager.Instance.TransitionToGame();
            }
            else
            {
                // Fallback - carrega cena diretamente
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
            }
        }

        #endregion

        #region Fade Utilities

        /// <summary>
        /// Fallback para fade manual quando UIManager não está disponível
        /// </summary>
        private IEnumerator FadeGameObjectManual(GameObject obj, float targetAlpha, float duration)
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
                Debug.Log($"[TitleScreenController] {message}");
            }
        }

        #endregion
    }
}