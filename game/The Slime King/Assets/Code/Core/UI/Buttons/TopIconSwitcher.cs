using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Core.UI.Icons
{
    /// <summary>
    /// Sistema de Ícone Superior - Gerencia a exibição de ícones para diferentes dispositivos com fade-in/out
    /// quando o jogador entra ou sai do alcance do objeto.
    /// </summary>
    public class TopIconSwitcher : MonoBehaviour
    {
        [Header("Ícones por Plataforma")]
        [Tooltip("Ícone para teclado")]
        [SerializeField] private GameObject _keyboardIcon;

        [Tooltip("Ícone para controle Xbox")]
        [SerializeField] private GameObject _xboxIcon;

        [Tooltip("Ícone para controle PlayStation")]
        [SerializeField] private GameObject _playstationIcon;

        [Tooltip("Ícone para controle Nintendo Switch")]
        [SerializeField] private GameObject _switchIcon;

        [Tooltip("Ícone para controle genérico")]
        [SerializeField] private GameObject _genericIcon;

        [Header("Configurações de Posicionamento")]
        [Tooltip("Deslocamento da posição do ícone em relação ao objeto pai")]
        [SerializeField] private Vector2 _iconOffset = Vector2.zero;

        [Header("Configurações de Fade")]
        [Tooltip("Duração do efeito de fade em segundos")]
        [SerializeField] private float _fadeDuration = 0.3f;

        [Tooltip("Curva de animação para o efeito de fade")]
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private GameObject _currentActiveIcon;
        private Coroutine _fadeCoroutine; private void Awake()
        {
            // Aplica offset a todos os ícones antes de qualquer operação
            ApplyOffsetToAllIcons();

            // Inicializa desativando todos os ícones - mais performático fazer isso no Awake
            DeactivateAllIcons();

            // Determina o ícone da plataforma atual, mas mantém desativado até o jogador entrar no alcance
            _currentActiveIcon = GetCurrentPlatformIcon();

            // Inscreve-se para receber notificações de mudança de dispositivo
            InputSystem.onActionChange += OnInputDeviceChanged;
        }

        private void OnDestroy()
        {
            // Cancela a inscrição ao destruir o objeto
            InputSystem.onActionChange -= OnInputDeviceChanged;

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
        }

        private void OnInputDeviceChanged(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                SwitchToCurrentPlatformIcon(false);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se é o jogador que entrou no alcance
            if (other.CompareTag("Player"))
            {
                // Ativa o ícone correspondente à plataforma atual
                ShowIcon();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Verifica se é o jogador que saiu do alcance
            if (other.CompareTag("Player"))
            {
                // Inicia o efeito de fade-out
                HideIcon();
            }
        }

        /// <summary>
        /// Mostra o ícone com fade-in, começando com alfa=0 e aumentando até alfa=1
        /// </summary>
        public void ShowIcon()
        {
            // Garante que o ícone correto da plataforma atual está ativo
            SwitchToCurrentPlatformIcon(true);

            // Define o alfa inicial como 0
            SetIconAlpha(_currentActiveIcon, 0f);

            // Inicia o fade-in
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeIconCoroutine(_currentActiveIcon, 0f, 1f));
        }

        /// <summary>
        /// Esconde o ícone com fade-out
        /// </summary>
        public void HideIcon()
        {
            if (_currentActiveIcon == null)
                return;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeIconCoroutine(_currentActiveIcon, 1f, 0f));
        }

        /// <summary>
        /// Desativa todos os ícones de plataforma
        /// </summary>
        private void DeactivateAllIcons()
        {
            if (_keyboardIcon) _keyboardIcon.SetActive(false);
            if (_xboxIcon) _xboxIcon.SetActive(false);
            if (_playstationIcon) _playstationIcon.SetActive(false);
            if (_switchIcon) _switchIcon.SetActive(false);
            if (_genericIcon) _genericIcon.SetActive(false);
        }

        /// <summary>
        /// Retorna o ícone correspondente à plataforma atual de forma otimizada
        /// </summary>
        private GameObject GetCurrentPlatformIcon()
        {
            // Verifica qual dispositivo está sendo usado (mais performático verificar na ordem de probabilidade)
            var gamepad = Gamepad.current;
            var keyboard = Keyboard.current;

            // Se não tiver gamepad conectado ou o teclado foi usado recentemente, usa o ícone de teclado
            if (gamepad == null || (keyboard != null && keyboard.wasUpdatedThisFrame))
            {
                return _keyboardIcon;
            }
            else
            {
                // Normaliza o nome do gamepad para comparações case-insensitive
                string deviceName = gamepad.name.ToLowerInvariant();

                // Identifica o tipo de gamepad baseado no nome
                if (deviceName.Contains("xbox") || deviceName.Contains("xinput"))
                {
                    return _xboxIcon;
                }
                else if (deviceName.Contains("playstation") || deviceName.Contains("ps") || deviceName.Contains("dualshock"))
                {
                    return _playstationIcon;
                }
                else if (deviceName.Contains("switch") || deviceName.Contains("nintendo") || deviceName.Contains("joycon"))
                {
                    return _switchIcon;
                }
                else
                {
                    return _genericIcon;
                }
            }
        }        /// <summary>
                 /// Troca para o ícone da plataforma atual de forma otimizada
                 /// </summary>
                 /// <param name="resetAlpha">Se verdadeiro, define o alpha inicial como 0</param>
        private void SwitchToCurrentPlatformIcon(bool resetAlpha)
        {
            // Detecta o ícone da plataforma atual
            GameObject newIcon = GetCurrentPlatformIcon();

            // Se já é o ícone atual, não precisa fazer nada
            if (newIcon == _currentActiveIcon)
                return;

            // Desativa o ícone anterior
            if (_currentActiveIcon != null)
                _currentActiveIcon.SetActive(false);

            // Atualiza a referência para o novo ícone
            _currentActiveIcon = newIcon;

            // Ativa o novo ícone (se existir)
            if (_currentActiveIcon != null)
            {
                // Aplica o offset de posição ao ícone
                ApplyIconOffset(_currentActiveIcon);

                // Ativa o ícone
                _currentActiveIcon.SetActive(true);

                // Define o alpha inicial como 0 para o efeito de fade-in
                if (resetAlpha)
                    SetIconAlpha(_currentActiveIcon, 0f);
            }
        }/// <summary>
         /// Define o alpha do ícone usando SpriteRenderer
         /// </summary>
        private void SetIconAlpha(GameObject icon, float alpha)
        {
            if (icon == null)
                return;

            // Tenta obter o SpriteRenderer
            var spriteRenderer = icon.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                return;

            // Modifica o alpha mantendo os valores RGB existentes
            Color color = spriteRenderer.color;
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
        }        /// <summary>
                 /// Coroutine para realizar o efeito de fade seguindo uma curva de animação configurável
                 /// </summary>
        private IEnumerator FadeIconCoroutine(GameObject icon, float startAlpha, float endAlpha)
        {
            if (icon == null)
                yield break;

            // Obtém o SpriteRenderer para controlar a transparência
            var spriteRenderer = icon.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                yield break;

            // Garante que o objeto está ativo durante o fade
            icon.SetActive(true);

            // Guarda a cor original para manter os valores RGB
            Color originalColor = spriteRenderer.color;

            // Inicializa o contador de tempo
            float elapsed = 0f;

            // Executa a animação ao longo do tempo configurado
            while (elapsed < _fadeDuration)
            {
                float normalizedTime = elapsed / _fadeDuration;
                float curveValue = _fadeCurve.Evaluate(normalizedTime);
                float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, curveValue);

                // Atualiza apenas o alpha, mantendo os valores RGB existentes
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentAlpha);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Garante que o alpha final seja exatamente o desejado
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, endAlpha);

            // Se estamos fazendo fade out (alpha final = 0), desativa o objeto quando terminar
            if (endAlpha <= 0f)
            {
                icon.SetActive(false);
            }

            _fadeCoroutine = null;
        }

        /// <summary>
        /// Aplica o offset de posição ao ícone especificado
        /// </summary>
        private void ApplyIconOffset(GameObject icon)
        {
            if (icon == null)
                return;

            // Apenas aplica o offset se não for Vector2.zero
            if (_iconOffset != Vector2.zero)
            {
                // Preserva a posição Z atual
                float zPos = icon.transform.localPosition.z;
                icon.transform.localPosition = new Vector3(_iconOffset.x, _iconOffset.y, zPos);
            }
        }

        /// <summary>
        /// Aplica o offset configurado a todos os ícones
        /// </summary>
        private void ApplyOffsetToAllIcons()
        {
            ApplyIconOffset(_keyboardIcon);
            ApplyIconOffset(_xboxIcon);
            ApplyIconOffset(_playstationIcon);
            ApplyIconOffset(_switchIcon);
            ApplyIconOffset(_genericIcon);
        }

        /// <summary>
        /// Define um novo offset de posição para os ícones
        /// </summary>
        /// <param name="offset">Novo vetor de offset a ser aplicado</param>
        /// <param name="applyImmediately">Se verdadeiro, aplica imediatamente aos ícones ativos</param>
        public void SetIconOffset(Vector2 offset, bool applyImmediately = true)
        {
            _iconOffset = offset;

            if (applyImmediately)
            {
                // Aplica aos ícones já configurados
                ApplyOffsetToAllIcons();

                // Se houver um ícone ativo no momento, reaplica o offset
                if (_currentActiveIcon != null && _currentActiveIcon.activeInHierarchy)
                {
                    ApplyIconOffset(_currentActiveIcon);
                }
            }
        }
    }
}
