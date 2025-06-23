using UnityEngine;

namespace TheSlimeKing.Core.Portal
{
    /// <summary>
    /// Controla o efeito visual de teleporte.
    /// Um efeito simples que cresce e depois desaparece.
    /// </summary>
    public class TeleportEffect : MonoBehaviour
    {
        [SerializeField] private float _duration = 1.0f;
        [SerializeField] private float _startScale = 0.1f;
        [SerializeField] private float _maxScale = 2.0f;
        [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve _alphaCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        [SerializeField] private bool _destroyAfterPlay = true;

        private float _elapsedTime = 0f;
        private SpriteRenderer _spriteRenderer;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _particleSystem = GetComponent<ParticleSystem>();

            // Inicia com a escala inicial
            transform.localScale = Vector3.one * _startScale;
        }

        private void Start()
        {
            // Inicia partículas se existirem
            if (_particleSystem != null)
            {
                _particleSystem.Play();
            }
        }

        private void Update()
        {
            _elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(_elapsedTime / _duration);

            // Aplica a escala com base na curva de animação
            float scale = Mathf.Lerp(_startScale, _maxScale, _scaleCurve.Evaluate(normalizedTime));
            transform.localScale = Vector3.one * scale;

            // Aplica alpha se tiver um sprite renderer
            if (_spriteRenderer != null)
            {
                Color color = _spriteRenderer.color;
                color.a = _alphaCurve.Evaluate(normalizedTime);
                _spriteRenderer.color = color;
            }

            // Destrói o objeto após a duração completa
            if (_elapsedTime >= _duration && _destroyAfterPlay)
            {
                Destroy(gameObject);
            }
        }
    }
}
