using UnityEngine;

namespace TheSlimeKing.Gameplay.Stealth
{
    /// <summary>
    /// Implementa o efeito visual de vinheta escura quando o jogador está escondido
    /// </summary>
    public class StealthEffect : MonoBehaviour
    {
        [SerializeField] private Material stealthEffectMaterial;
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float maxIntensity = 0.7f;
        [SerializeField] private Color vignetteTint = new Color(0.1f, 0.05f, 0.2f);

        private SpriteRenderer _effectRenderer;
        private float _currentIntensity = 0f;
        private float _targetIntensity;
        private float _transitionTimer = 0f;

        private void Awake()
        {
            // Cria o componente de renderer se não existir
            _effectRenderer = GetComponent<SpriteRenderer>();
            if (_effectRenderer == null)
            {
                _effectRenderer = gameObject.AddComponent<SpriteRenderer>();
            }

            // Configura o renderer
            if (_effectRenderer != null)
            {
                // Configura a camada e ordem para ficar em cima da câmera
                _effectRenderer.sortingLayerName = "UI";
                _effectRenderer.sortingOrder = 100;

                // Se tiver material customizado, aplica
                if (stealthEffectMaterial != null)
                {
                    _effectRenderer.material = stealthEffectMaterial;
                    _effectRenderer.material.SetColor("_VignetteTint", vignetteTint);
                    _effectRenderer.material.SetFloat("_Intensity", 0);
                }

                // Cria um sprite que cubra a tela toda
                _effectRenderer.sprite = CreateFullScreenSprite();
            }

            // Inicia o efeito
            _targetIntensity = maxIntensity;
        }

        private void Update()
        {
            // Processo de transição suave do efeito
            _transitionTimer += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(_transitionTimer / fadeInDuration);

            _currentIntensity = Mathf.Lerp(_currentIntensity, _targetIntensity, normalizedTime);

            // Aplica a intensidade atual ao material
            if (stealthEffectMaterial != null)
            {
                stealthEffectMaterial.SetFloat("_Intensity", _currentIntensity);
            }
            // Ou aplica a intensidade via alpha do sprite
            else if (_effectRenderer != null)
            {
                Color c = _effectRenderer.color;
                c.a = _currentIntensity * 0.5f; // Reduzido para não ficar muito escuro
                _effectRenderer.color = c;
            }
        }

        /// <summary>
        /// Define a intensidade do efeito de stealth
        /// </summary>
        public void SetIntensity(float intensity)
        {
            _targetIntensity = Mathf.Clamp01(intensity) * maxIntensity;
            _transitionTimer = 0f; // Reinicia a transição
        }

        /// <summary>
        /// Cria um sprite que cobre toda a tela para o efeito de vinheta
        /// </summary>
        private Sprite CreateFullScreenSprite()
        {
            // Cria um sprite quadrado simples que cobrirá a tela
            Texture2D tex = new Texture2D(2, 2);
            Color color = new Color(vignetteTint.r, vignetteTint.g, vignetteTint.b, 0.0f);
            tex.SetPixels(new[] { color, color, color, color });
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
        }

        private void OnDestroy()
        {
            // Limpa recursos temporários
            if (_effectRenderer != null && _effectRenderer.sprite != null)
            {
                Destroy(_effectRenderer.sprite);
            }
        }
    }
}
