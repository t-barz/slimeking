using UnityEngine;

namespace TheSlimeKing.Core.UI.Icons
{
    /// <summary>
    /// Componente para objetos interativos que necessitam exibir ícones de interação
    /// </summary>
    public class IconDisplay : MonoBehaviour
    {
        [SerializeField] private string _actionType = "Interact";
        [SerializeField] private float _detectionRadius = 1.5f;
        [SerializeField] private LayerMask _playerLayer;
        [SerializeField] private Transform _iconPosition;

        private bool _isDisplayingIcon = false;

        private void Start()
        {
            // Se não houver uma posição específica para o ícone, usa a posição do próprio objeto
            if (_iconPosition == null)
                _iconPosition = transform;
        }

        private void Update()
        {
            // Verifica se o jogador está próximo do objeto interativo
            if (Physics2D.OverlapCircle(transform.position, _detectionRadius, _playerLayer))
            {
                if (!_isDisplayingIcon)
                {
                    _isDisplayingIcon = true;
                    IconManager.Instance.ShowIcon(_actionType, _iconPosition.position + Vector3.up);
                }
            }
            else if (_isDisplayingIcon)
            {
                _isDisplayingIcon = false;
                IconManager.Instance.HideIcon();
            }
        }

        private void OnDestroy()
        {
            // Garante que o ícone seja removido se o objeto for destruído
            if (_isDisplayingIcon)
            {
                IconManager.Instance.HideIcon();
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Desenha um gizmo para visualizar o raio de detecção no editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}
