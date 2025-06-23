using UnityEngine;
using TMPro;

namespace TheSlimeKing.Gameplay.Stealth
{
    /// <summary>
    /// Controla o ícone de feedback visual para quando o personagem está escondido
    /// </summary>
    public class StealthIcon : MonoBehaviour
    {
        [SerializeField] private float bobSpeed = 1.0f;
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float rotationSpeed = 30f;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private TextMeshProUGUI statusText;

        private Vector3 _startPosition;
        private float _initialAlpha;
        private float _currentFadeTime = 0f;

        private void Awake()
        {
            _startPosition = transform.localPosition;

            // Inicializa a transparência
            if (iconRenderer != null)
            {
                _initialAlpha = iconRenderer.color.a;
            }

            // Inicia invisível
            SetVisibility(false, true);
        }

        private void Start()
        {
            // Torna visível com animação
            SetVisibility(true, false);
        }

        private void Update()
        {
            // Animação de flutuação suave
            if (gameObject.activeInHierarchy)
            {
                float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.localPosition = _startPosition + new Vector3(0, bobOffset, 0);

                // Rotação lenta
                transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

                // Processa o fade
                ProcessFade();
            }
        }

        /// <summary>
        /// Define a visibilidade do ícone com animação opcional
        /// </summary>
        /// <param name="visible">Se o ícone deve estar visível</param>
        /// <param name="immediate">Se verdadeiro, aplica imediatamente sem animação</param>
        public void SetVisibility(bool visible, bool immediate = false)
        {
            if (immediate)
            {
                // Aplicação imediata
                if (iconRenderer != null)
                {
                    Color c = iconRenderer.color;
                    c.a = visible ? _initialAlpha : 0f;
                    iconRenderer.color = c;
                }

                if (statusText != null)
                {
                    Color c = statusText.color;
                    c.a = visible ? 1f : 0f;
                    statusText.color = c;
                }
            }
            else
            {
                // Inicia animação de fade
                _currentFadeTime = 0f;
            }

            // Atualiza o texto se presente
            if (statusText != null)
            {
                statusText.text = "Escondido";
            }
        }

        /// <summary>
        /// Processa a animação de fade
        /// </summary>
        private void ProcessFade()
        {
            if (_currentFadeTime <= fadeDuration)
            {
                _currentFadeTime += Time.deltaTime;
                float normalizedTime = _currentFadeTime / fadeDuration;

                // Fade in 
                if (iconRenderer != null)
                {
                    Color c = iconRenderer.color;
                    c.a = Mathf.Lerp(0, _initialAlpha, normalizedTime);
                    iconRenderer.color = c;
                }

                if (statusText != null)
                {
                    Color c = statusText.color;
                    c.a = Mathf.Lerp(0, 1, normalizedTime);
                    statusText.color = c;
                }
            }
        }
    }
}
