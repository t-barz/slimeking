using UnityEngine;

namespace TheSlimeKing.Gameplay.Stealth
{
    /// <summary>
    /// Representa um objeto que pode ser usado como cobertura no sistema de stealth
    /// </summary>
    public class CoverObject : MonoBehaviour
    {
        [Header("Configurações de Cobertura")]
        [SerializeField] private float coverRadius = 1.0f;
        [SerializeField] private CoverType coverType = CoverType.Bush;
        [SerializeField] private bool highlightWhenNear = true;
        [SerializeField] private GameObject highlightEffectPrefab;

        [Header("Configurações de Feedback")]
        [SerializeField] private Color highlightColor = new Color(0.5f, 1.0f, 0.5f, 0.3f);
        [SerializeField] private float highlightPulseSpeed = 1.0f;

        // Estado e referências
        private bool _playerNearby = false;
        private GameObject _highlightInstance = null;
        private SpriteRenderer _renderer;
        private Color _originalColor;

        private void Awake()
        {
            // Garante que o objeto tenha a tag correta para ser reconhecido pelo sistema
            EnsureCorrectTag();

            // Obtém o renderer para efeitos visuais
            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer != null)
                _originalColor = _renderer.color;
        }

        private void Update()
        {
            // Aplica efeito de destaque se o jogador estiver próximo
            UpdateHighlightEffect();
        }

        /// <summary>
        /// Garante que o objeto tenha uma tag apropriada para cobertura
        /// </summary>
        private void EnsureCorrectTag()
        {
            string tagToUse = coverType switch
            {
                CoverType.Grass => "Grass",
                CoverType.Bush => "Bush",
                CoverType.Rock => "Rock",
                CoverType.Tree => "Tree",
                _ => "Bush"  // Default para compatibilidade
            };

            // Aplica a tag se não já tiver uma tag válida de cobertura
            if (tag != "Grass" && tag != "Bush" && tag != "Rock" && tag != "Tree")
            {
                tag = tagToUse;
            }
        }

        /// <summary>
        /// Atualiza o efeito visual quando o jogador está próximo
        /// </summary>
        private void UpdateHighlightEffect()
        {
            if (!highlightWhenNear) return;

            // Se o jogador está próximo e não tem destaque, cria um
            if (_playerNearby && _highlightInstance == null && highlightEffectPrefab != null)
            {
                _highlightInstance = Instantiate(highlightEffectPrefab, transform);
                _highlightInstance.transform.localPosition = Vector3.zero;
            }
            // Se o jogador não está próximo e tem destaque, remove
            else if (!_playerNearby && _highlightInstance != null)
            {
                Destroy(_highlightInstance);
                _highlightInstance = null;
            }

            // Aplica efeito de pulso no sprite, se tiver renderer
            if (_renderer != null && _playerNearby)
            {
                float pulseValue = (Mathf.Sin(Time.time * highlightPulseSpeed) * 0.5f) + 0.5f;
                _renderer.color = Color.Lerp(_originalColor, highlightColor, pulseValue * 0.3f);
            }
            else if (_renderer != null && !_playerNearby)
            {
                _renderer.color = _originalColor;
            }
        }

        /// <summary>
        /// Chamado quando o jogador entra na área de cobertura
        /// </summary>
        public void OnPlayerEnter()
        {
            _playerNearby = true;

            if (highlightWhenNear)
                ShowHighlight(true);
        }

        /// <summary>
        /// Chamado quando o jogador sai da área de cobertura
        /// </summary>
        public void OnPlayerExit()
        {
            _playerNearby = false;

            ShowHighlight(false);
        }

        /// <summary>
        /// Controla o efeito de destaque visual
        /// </summary>
        private void ShowHighlight(bool show)
        {
            if (_renderer != null)
            {
                _renderer.color = show ? highlightColor : _originalColor;
            }

            // Outros efeitos visuais podem ser adicionados aqui
        }

        /// <summary>
        /// Obtém o tipo de cobertura deste objeto
        /// </summary>
        public CoverType GetCoverType()
        {
            return coverType;
        }

        /// <summary>
        /// Para visualização no editor: desenha o raio da área de cobertura
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, coverRadius);
        }

        /// <summary>
        /// Verifica colisões com o collider 2D para detectar o jogador
        /// </summary>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Verifica se o objeto que entrou é o jogador
            if (other.CompareTag("Player"))
            {
                OnPlayerEnter();
            }
        }

        /// <summary>
        /// Verifica quando o jogador sai da área de colisão
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            // Verifica se o objeto que saiu é o jogador
            if (other.CompareTag("Player"))
            {
                OnPlayerExit();
            }
        }
    }
}
