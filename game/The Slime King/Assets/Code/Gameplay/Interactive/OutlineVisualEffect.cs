using System.Collections;
using UnityEngine;

namespace TheSlimeKing.Gameplay.Interactive
{
    /// <summary>
    /// Aplica efeito de contorno visual aos objetos interativos
    /// Requer um shader específico para funcionar adequadamente
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class OutlineVisualEffect : MonoBehaviour
    {
        [Header("Configurações do Contorno")]
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField] private float _outlineThickness = 1.0f;
        [SerializeField] private float _pulseSpeed = 1.0f;
        [SerializeField] private float _pulseAmount = 0.2f;

        [Header("Animação")]
        [SerializeField] private float _fadeInDuration = 0.3f;
        [SerializeField] private float _fadeOutDuration = 0.2f;

        // Componentes
        private SpriteRenderer _spriteRenderer;
        private Material _originalMaterial;
        private Material _outlineMaterial;

        // Estado
        private bool _isEnabled = false;
        private float _currentIntensity = 0f;
        private float _targetIntensity = 0f;
        private float _pulseTimer = 0f;
        private Coroutine _fadeCoroutine;

        // Propriedades shader
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _originalMaterial = _spriteRenderer.material;

            // Tenta encontrar o shader de contorno
            Shader outlineShader = Shader.Find("Slime/SpriteOutline");

            if (outlineShader != null)
            {
                // Cria material com shader de contorno
                _outlineMaterial = new Material(outlineShader);
                _outlineMaterial.SetColor(OutlineColorProperty, _outlineColor);
                _outlineMaterial.SetFloat(OutlineThicknessProperty, 0f); // Começa invisível
            }
            else
            {
                Debug.LogWarning("Shader de contorno não encontrado! O efeito não será aplicado.");
            }
        }

        private void OnEnable()
        {
            // Verifica se existe um material configurado
            if (_outlineMaterial != null && _isEnabled)
            {
                ApplyOutlineMaterial();
            }
        }

        private void Update()
        {
            if (!_isEnabled || _outlineMaterial == null)
                return;

            // Efeito de pulsar suave no contorno
            _pulseTimer += Time.deltaTime * _pulseSpeed;
            float pulse = Mathf.Sin(_pulseTimer) * _pulseAmount;
            float thickness = _currentIntensity * (_outlineThickness + pulse);

            _outlineMaterial.SetFloat(OutlineThicknessProperty, thickness);
        }

        /// <summary>
        /// Define a cor do contorno
        /// </summary>
        public void SetColor(Color color)
        {
            _outlineColor = color;

            if (_outlineMaterial != null)
            {
                _outlineMaterial.SetColor(OutlineColorProperty, _outlineColor);
            }
        }

        /// <summary>
        /// Ativa ou desativa o contorno
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            _isEnabled = enabled;

            if (enabled)
            {
                ApplyOutlineMaterial();
            }
            else
            {
                RestoreOriginalMaterial();
            }
        }

        /// <summary>
        /// Faz o contorno aparecer gradualmente
        /// </summary>
        public void FadeIn()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeOutlineIntensity(0f, 1f, _fadeInDuration));
        }

        /// <summary>
        /// Faz o contorno desaparecer gradualmente
        /// </summary>
        public void FadeOut()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(FadeOutlineIntensity(1f, 0f, _fadeOutDuration));
        }

        /// <summary>
        /// Aplica o material de contorno ao sprite
        /// </summary>
        private void ApplyOutlineMaterial()
        {
            if (_spriteRenderer != null && _outlineMaterial != null)
            {
                _spriteRenderer.material = _outlineMaterial;
            }
        }

        /// <summary>
        /// Restaura o material original do sprite
        /// </summary>
        private void RestoreOriginalMaterial()
        {
            if (_spriteRenderer != null && _originalMaterial != null)
            {
                _spriteRenderer.material = _originalMaterial;
            }
        }

        /// <summary>
        /// Anima a intensidade do contorno de um valor para outro
        /// </summary>
        private IEnumerator FadeOutlineIntensity(float fromIntensity, float toIntensity, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                _currentIntensity = Mathf.Lerp(fromIntensity, toIntensity, elapsedTime / duration);

                if (_outlineMaterial != null)
                {
                    _outlineMaterial.SetFloat(OutlineThicknessProperty, _currentIntensity * _outlineThickness);
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _currentIntensity = toIntensity;

            if (_outlineMaterial != null)
            {
                _outlineMaterial.SetFloat(OutlineThicknessProperty, _currentIntensity * _outlineThickness);
            }

            // Se o contorno estiver completamente invisível, restaure o material original
            if (toIntensity <= 0f)
            {
                SetEnabled(false);
            }
        }

        private void OnDisable()
        {
            // Restaura o material original quando desativado
            RestoreOriginalMaterial();
        }

        private void OnDestroy()
        {
            // Limpa o material criado
            if (_outlineMaterial != null)
            {
                Destroy(_outlineMaterial);
            }
        }
    }
}
