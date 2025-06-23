using System.Collections;
using UnityEngine;

namespace TheSlimeKing.Gameplay.Movement
{
    /// <summary>
    /// Implementa pontos onde o slime pode realizar saltos especiais
    /// </summary>
    public class JumpMovePoint : SpecialMovement
    {
        [Header("Configurações de Salto")]
        [Tooltip("Ponto de destino para onde o slime irá pular")]
        [SerializeField] private Transform _landingPoint;
        
        [Tooltip("Altura máxima do arco do salto")]
        [SerializeField] private float _jumpHeight = 3f;
        
        [Tooltip("Curva de altura para o salto")]
        [SerializeField] private AnimationCurve _jumpHeightCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
        
        [Tooltip("Curva de avanço horizontal para o salto")]
        [SerializeField] private AnimationCurve _jumpProgressCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [Tooltip("Tempo de preparação antes do salto")]
        [SerializeField] private float _preparationTime = 0.3f;
        
        [Tooltip("Tempo de recuperação após o salto")]
        [SerializeField] private float _recoveryTime = 0.3f;
        
        [Header("Verificação de Trajetória")]
        [Tooltip("Verificar se a trajetória está livre antes de pular")]
        [SerializeField] private bool _checkTrajectory = true;
        
        [Tooltip("Camadas a verificar para obstrução")]
        [SerializeField] private LayerMask _obstructionLayers;
        
        [Tooltip("Número de pontos a verificar na trajetória")]
        [SerializeField] private int _trajectoryCheckPoints = 8;
        
        [Tooltip("Raio para verificação de colisão")]
        [SerializeField] private float _collisionRadius = 0.5f;
        
        [Header("Animação")]
        [Tooltip("Escala durante a preparação para o salto")]
        [SerializeField] private Vector3 _prepareScale = new Vector3(1.2f, 0.8f, 1f);
        
        [Tooltip("Alongamento durante o salto")]
        [SerializeField] private Vector3 _stretchScale = new Vector3(0.8f, 1.2f, 1f);
        
        [Tooltip("Escala ao aterrissar")]
        [SerializeField] private Vector3 _landScale = new Vector3(1.3f, 0.7f, 1f);

        /// <summary>
        /// Verifica se o ponto de destino está configurado
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            
            if (_landingPoint == null)
            {
                Debug.LogError($"JumpMovePoint '{gameObject.name}' não tem ponto de aterrissagem configurado!");
            }
        }
        
        /// <summary>
        /// Executa a sequência de salto
        /// </summary>
        protected override IEnumerator PerformSpecialMovement()
        {
            if (_landingPoint == null || _playerTransform == null)
            {
                Debug.LogError("Falha ao iniciar salto: ponto de aterrissagem ou jogador não encontrados.");
                _isMovementInProgress = false;
                yield break;
            }
            
            // Verificação de trajetória
            if (_checkTrajectory && !IsTrajectoryCleared())
            {
                Debug.LogWarning("Trajetória de salto obstruída. Salto cancelado.");
                _isMovementInProgress = false;
                yield break;
            }
            
            // Armazena a escala original do jogador
            Vector3 originalScale = _playerTransform.localScale;
            float xDirection = Mathf.Sign(originalScale.x);
            
            // Posiciona o jogador no ponto de salto
            _playerTransform.position = transform.position;
            
            // Fase de preparação para o salto
            float timer = 0f;
            
            while (timer < _preparationTime)
            {
                float t = timer / _preparationTime;
                Vector3 scaleChange = Vector3.Lerp(originalScale, new Vector3(
                    _prepareScale.x * xDirection, 
                    _prepareScale.y, 
                    _prepareScale.z), t);
                
                _playerTransform.localScale = scaleChange;
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Armazena pontos para o salto
            Vector3 startPosition = transform.position;
            Vector3 endPosition = _landingPoint.position;
            
            // Inicia o salto
            timer = 0f;
            
            while (timer < _movementDuration)
            {
                float normalizedTime = timer / _movementDuration;
                
                // Calcula progresso horizontal
                float horizontalProgress = _jumpProgressCurve.Evaluate(normalizedTime);
                
                // Calcula altura do salto
                float verticalOffset = _jumpHeight * _jumpHeightCurve.Evaluate(normalizedTime);
                
                // Interpola posição
                Vector2 horizontalPosition = Vector2.Lerp(startPosition, endPosition, horizontalProgress);
                Vector3 newPosition = new Vector3(
                    horizontalPosition.x, 
                    horizontalPosition.y + verticalOffset, 
                    0);
                
                // Aplica posição
                _playerTransform.position = newPosition;
                
                // Aplica efeito de alongamento durante o salto
                float verticalPhase = _jumpHeightCurve.Evaluate(normalizedTime);
                Vector3 jumpScale = Vector3.Lerp(originalScale, new Vector3(
                    _stretchScale.x * xDirection, 
                    _stretchScale.y, 
                    _stretchScale.z), verticalPhase);
                
                _playerTransform.localScale = jumpScale;
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Garante que o jogador atinge o destino
            _playerTransform.position = endPosition;
            
            // Efeito de aterrissagem
            timer = 0f;
            float landingDuration = _recoveryTime * 0.7f;
            
            while (timer < landingDuration)
            {
                float t = timer / landingDuration;
                
                Vector3 landingScale = Vector3.Lerp(
                    new Vector3(_landScale.x * xDirection, _landScale.y, _landScale.z), 
                    originalScale, 
                    t);
                
                _playerTransform.localScale = landingScale;
                
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Restaura escala original
            _playerTransform.localScale = originalScale;
            
            // Pequeno delay para recuperação
            yield return new WaitForSeconds(_recoveryTime * 0.3f);
            
            // Finaliza o movimento
            CompleteMovement();
        }
        
        /// <summary>
        /// Verifica se a trajetória do salto está livre de obstruções
        /// </summary>
        private bool IsTrajectoryCleared()
        {
            if (_trajectoryCheckPoints <= 0 || _landingPoint == null)
                return true;
            
            Vector3 startPoint = transform.position;
            Vector3 endPoint = _landingPoint.position;
            
            for (int i = 0; i < _trajectoryCheckPoints; i++)
            {
                float progress = (float)i / (_trajectoryCheckPoints - 1);
                
                // Calcula posição horizontal
                Vector3 horizontalPos = Vector3.Lerp(startPoint, endPoint, _jumpProgressCurve.Evaluate(progress));
                
                // Calcula altura neste ponto
                float height = _jumpHeight * _jumpHeightCurve.Evaluate(progress);
                
                // Posição final do ponto
                Vector3 checkPoint = new Vector3(horizontalPos.x, horizontalPos.y + height, horizontalPos.z);
                
                // Verifica colisão neste ponto
                Collider2D collision = Physics2D.OverlapCircle(checkPoint, _collisionRadius, _obstructionLayers);
                if (collision != null)
                {
                    Debug.LogWarning($"Obstrução detectada na trajetória de salto: {collision.gameObject.name}");
                    return false;
                }
            }
            
            return true;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Desenha gizmos para visualização no editor
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            // Desenha trajetória de salto
            if (_landingPoint != null)
            {
                // Desenha linha direta
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _landingPoint.position);
                
                // Desenha arco aproximado
                Gizmos.color = Color.cyan;
                Vector3 start = transform.position;
                Vector3 end = _landingPoint.position;
                
                int segments = 20;
                Vector3 prevPoint = start;
                
                for (int i = 1; i <= segments; i++)
                {
                    float progress = (float)i / segments;
                    
                    // Posição horizontal
                    Vector3 horizontalPos = Vector3.Lerp(start, end, _jumpProgressCurve.Evaluate(progress));
                    
                    // Altura
                    float height = _jumpHeight * _jumpHeightCurve.Evaluate(progress);
                    
                    // Ponto atual
                    Vector3 currentPoint = new Vector3(horizontalPos.x, horizontalPos.y + height, horizontalPos.z);
                    
                    // Desenha segmento
                    Gizmos.DrawLine(prevPoint, currentPoint);
                    prevPoint = currentPoint;
                }
                
                // Desenha área de aterrissagem
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(_landingPoint.position, 0.5f);
            }
        }
#endif
    }
}
