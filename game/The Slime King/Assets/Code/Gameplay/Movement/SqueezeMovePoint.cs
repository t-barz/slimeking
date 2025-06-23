using System.Collections;
using UnityEngine;

namespace TheSlimeKing.Gameplay.Movement
{
    /// <summary>
    /// Implementa pontos onde o slime pode se encolher e esgueirar por passagens estreitas
    /// </summary>
    public class SqueezeMovePoint : SpecialMovement
    {
        [Header("Configurações de Encolhimento")]
        [Tooltip("Ponto de destino para onde o slime irá após se encolher")]
        [SerializeField] private Transform _exitPoint;
        
        [Tooltip("Escala mínima durante o encolhimento (porcentagem)")]
        [Range(0.1f, 0.9f)]
        [SerializeField] private float _minScaleFactor = 0.5f;
        
        [Tooltip("Curva de animação para o encolhimento")]
        [SerializeField] private AnimationCurve _squeezeCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);
        
        [Tooltip("Tempo adicional antes de recuperar o controle")]
        [SerializeField] private float _recoveryDelay = 0.3f;
        
        [Header("Configurações Avançadas")]
        [Tooltip("Verificar colisões durante o movimento")]
        [SerializeField] private bool _checkCollisionsDuringMovement = true;
        
        [Tooltip("Camadas a verificar para colisão")]
        [SerializeField] private LayerMask _collisionLayers;

        /// <summary>
        /// Verifica se o ponto de saída está configurado
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            
            if (_exitPoint == null)
            {
                Debug.LogError($"SqueezeMovePoint '{gameObject.name}' não tem ponto de saída configurado!");
            }
        }
        
        /// <summary>
        /// Executa a sequência de encolhimento e movimento
        /// </summary>
        protected override IEnumerator PerformSpecialMovement()
        {
            if (_exitPoint == null || _playerTransform == null)
            {
                Debug.LogError("Falha ao iniciar movimento: ponto de saída ou jogador não encontrados.");
                _isMovementInProgress = false;
                yield break;
            }
            
            // Armazena a escala original do jogador
            Vector3 originalScale = _playerTransform.localScale;
            
            // Posiciona o jogador no ponto de entrada
            _playerTransform.position = transform.position;
            
            // Fase de encolhimento - 25% do tempo total
            float shrinkTime = _movementDuration * 0.25f;
            float timer = 0f;
            
            while (timer < shrinkTime)
            {
                float t = timer / shrinkTime;
                float scaleFactor = Mathf.Lerp(1f, _minScaleFactor, t);
                
                // Aplica escala preservando direção
                float xSign = Mathf.Sign(originalScale.x);
                _playerTransform.localScale = new Vector3(
                    originalScale.x * scaleFactor, 
                    originalScale.y * scaleFactor, 
                    originalScale.z
                );
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Fase de movimento pelo caminho estreito - 50% do tempo total
            float moveTime = _movementDuration * 0.5f;
            timer = 0f;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = _exitPoint.position;
            
            while (timer < moveTime)
            {
                float t = timer / moveTime;
                float curveT = _squeezeCurve.Evaluate(t);
                _playerTransform.position = Vector3.Lerp(startPosition, endPosition, curveT);
                
                if (_checkCollisionsDuringMovement)
                {
                    // Verificar colisões no caminho
                    Collider2D[] hits = Physics2D.OverlapCircleAll(
                        _playerTransform.position, 
                        0.3f * _minScaleFactor, 
                        _collisionLayers);
                        
                    if (hits.Length > 0)
                    {
                        // Se detectar colisão, cancelar movimento
                        Debug.LogWarning("Colisão detectada durante movimento de encolhimento. Movimento cancelado.");
                        break;
                    }
                }
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Posiciona no ponto final
            _playerTransform.position = endPosition;
            
            // Fase de recuperação do tamanho - 25% do tempo total
            float growTime = _movementDuration * 0.25f;
            timer = 0f;
            
            while (timer < growTime)
            {
                float t = timer / growTime;
                float scaleFactor = Mathf.Lerp(_minScaleFactor, 1f, t);
                
                // Aplica escala preservando direção
                _playerTransform.localScale = new Vector3(
                    originalScale.x * scaleFactor, 
                    originalScale.y * scaleFactor, 
                    originalScale.z
                );
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Restaura escala original
            _playerTransform.localScale = originalScale;
            
            // Pequeno delay antes de devolver o controle
            yield return new WaitForSeconds(_recoveryDelay);
            
            // Finaliza o movimento
            CompleteMovement();
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Desenha gizmos para visualização no editor
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            // Desenha linha de movimento para o ponto de saída
            if (_exitPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _exitPoint.position);
                
                // Desenha área de saída
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_exitPoint.position, 0.3f);
            }
        }
#endif
    }
}
