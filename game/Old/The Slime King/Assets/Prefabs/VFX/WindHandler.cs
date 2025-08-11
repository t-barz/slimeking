using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VFX.Wind
{
    /// <summary>
    /// Gerencia efeitos de vento em objetos próximos, ativando triggers de animação
    /// automaticamente quando objetos entram/saem da área de detecção.
    /// Inclui sistema de movimento horizontal com flip automático.
    /// </summary>
    public class WindHandler : MonoBehaviour
    {
        #region Serialized Fields
        [Header("Detection Settings")]
        [Tooltip("Raio de detecção para objetos próximos")]
        [SerializeField, Range(0.5f, 20f)] private float detectionRadius = 5f;

        [Tooltip("Layers dos objetos que serão detectados")]
        [SerializeField] private LayerMask objectLayerMask = -1;

        [Tooltip("Frequência de verificação de proximidade (verificações por segundo)")]
        [SerializeField, Range(1f, 30f)] private float checkFrequency = 10f;

        [Header("Movement Settings")]
        [Tooltip("Habilita movimento automático do WindHandler")]
        [SerializeField] private bool enableMovement = true;

        [Tooltip("Velocidade de movimento (unidades por segundo)")]
        [SerializeField, Range(0.1f, 10f)] private float movementSpeed = 2f;

        [Tooltip("Movimento da esquerda para direita (true) ou direita para esquerda (false)")]
        [SerializeField] private bool moveLeftToRight = false;

        [Tooltip("Realizar flip automático da imagem baseado na direção")]
        [SerializeField] private bool autoFlipSprite = true;

        [Tooltip("Componente usado para flip (SpriteRenderer ou Transform)")]
        [SerializeField] private FlipMethod flipMethod = FlipMethod.SpriteRenderer;

        [Header("Animation Settings")]
        [Tooltip("Nome da trigger no Animator dos objetos detectados")]
        [SerializeField] private string shakeTriggerName = "Shake";

        [Tooltip("Intervalo mínimo entre ativações da trigger no mesmo objeto")]
        [SerializeField, Range(0.1f, 5f)] private float triggerCooldown = 1.0f;

        [Header("Target Configuration")]
        [Tooltip("Tags dos objetos que serão afetados pelo vento")]
        [SerializeField] private string[] targetTags = { "Destructible", "PropEnv" };

        [Header("Debug & Visualization")]
        [SerializeField] private bool showDebugGizmos = true;
        [SerializeField] private bool showAnimationLogs = false;
        [SerializeField] private bool showMovementLogs = false;
        [SerializeField] private Color detectionRadiusColor = Color.yellow;
        [SerializeField] private Color detectedObjectColor = Color.red;
        [SerializeField] private Color animatorDetectedColor = Color.cyan;
        [SerializeField] private Color movementDirectionColor = Color.green;
        #endregion

        #region Enums
        public enum FlipMethod
        {
            SpriteRenderer,
            Transform
        }
        #endregion

        #region Private Fields
        private readonly HashSet<GameObject> currentlyNearbyObjects = new HashSet<GameObject>();
        private readonly List<Collider2D> detectionBuffer = new List<Collider2D>();
        private readonly Dictionary<GameObject, float> triggerCooldowns = new Dictionary<GameObject, float>();

        private Coroutine proximityCheckCoroutine;
        private WaitForSeconds checkInterval;

        // Cache para otimização - evita GetComponent repetidos
        private readonly Dictionary<GameObject, Animator> animatorCache = new Dictionary<GameObject, Animator>();

        // Movimento - cache para performance
        private Vector3 movementVector;
        private SpriteRenderer spriteRenderer;
        private bool originalSpriteFlipX;
        private Vector3 originalScale;
        private bool lastMoveDirection;

        // Cache adicional para otimização
        private Transform cachedTransform;
        private const float TIME_DELTA_THRESHOLD = 0.001f; // Threshold para otimização de Time.deltaTime
        #endregion

        #region Constants
        private const int GIZMO_CIRCLE_SEGMENTS = 36;
        private const float GIZMO_OBJECT_SIZE = 0.5f;
        private const float GIZMO_ANIMATOR_RADIUS = 0.3f;
        private const float GIZMO_DIRECTION_ARROW_LENGTH = 2f;
        #endregion

        #region Properties
        /// <summary>
        /// Número de objetos atualmente detectados
        /// </summary>
        public int NearbyObjectsCount => currentlyNearbyObjects.Count;

        /// <summary>
        /// Indica se o sistema está ativo
        /// </summary>
        public bool IsActive { get; private set; } = true;

        /// <summary>
        /// Indica se o movimento está ativo
        /// </summary>
        public bool IsMoving => enableMovement;

        /// <summary>
        /// Velocidade atual de movimento
        /// </summary>
        public float CurrentSpeed => movementSpeed;

        /// <summary>
        /// Direção atual do movimento (true = esquerda para direita, false = direita para esquerda)
        /// </summary>
        public bool IsMovingLeftToRight => moveLeftToRight;

        /// <summary>
        /// Descrição textual da direção atual
        /// </summary>
        public string CurrentDirectionText => moveLeftToRight ? "Esquerda → Direita" : "Direita → Esquerda";
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            // Cache do Transform para evitar chamadas repetidas
            cachedTransform = transform;

            ValidateConfiguration();
            InitializeSystem();
            InitializeMovement();
        }

        private void Start()
        {
            StartProximityChecking();
        }

        private void Update()
        {
            if (enableMovement)
            {
                UpdateMovement();
            }
        }

        private void OnDestroy()
        {
            StopProximityChecking();
            ClearAllData();
        }

        private void OnDrawGizmos()
        {
            if (!showDebugGizmos) return;
            DrawDetectionGizmos();
            DrawMovementGizmos();
        }

        private void OnValidate()
        {
            // Recalcula intervalo quando valores mudam no Inspector
            if (Application.isPlaying)
            {
                checkInterval = new WaitForSeconds(1f / checkFrequency);
                UpdateMovementVector();
                UpdateFlipState();
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Valida a configuração inicial do componente
        /// </summary>
        private void ValidateConfiguration()
        {
            if (detectionRadius <= 0f)
            {
                Debug.LogWarning($"[WindHandler] Detection radius inválido: {detectionRadius}. Usando valor padrão.");
                detectionRadius = 5f;
            }

            if (targetTags == null || targetTags.Length == 0)
            {
                Debug.LogWarning("[WindHandler] Nenhuma tag alvo configurada. Sistema pode não funcionar corretamente.");
            }

            if (string.IsNullOrEmpty(shakeTriggerName))
            {
                Debug.LogWarning("[WindHandler] Nome da trigger não configurado. Usando 'Shake' como padrão.");
                shakeTriggerName = "Shake";
            }

            if (movementSpeed <= 0f)
            {
                Debug.LogWarning($"[WindHandler] Velocidade inválida: {movementSpeed}. Usando valor padrão.");
                movementSpeed = 2f;
            }
        }

        /// <summary>
        /// Inicializa o sistema
        /// </summary>
        private void InitializeSystem()
        {
            checkInterval = new WaitForSeconds(1f / checkFrequency);

            if (showAnimationLogs)
            {
                Debug.Log($"[WindHandler] Sistema inicializado. Raio: {detectionRadius}, Frequência: {checkFrequency}Hz");
            }
        }

        /// <summary>
        /// Inicializa o sistema de movimento
        /// </summary>
        private void InitializeMovement()
        {
            // Cache dos componentes para flip
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalSpriteFlipX = spriteRenderer.flipX;
            }

            originalScale = transform.localScale;
            lastMoveDirection = moveLeftToRight;

            // Calcula vetor de movimento inicial
            UpdateMovementVector();
            UpdateFlipState();

            if (showMovementLogs)
            {
                Debug.Log($"[WindHandler] Movimento inicializado. Direção: {CurrentDirectionText}, Velocidade: {movementSpeed}");
            }
        }

        /// <summary>
        /// Atualiza o vetor de movimento baseado na direção
        /// </summary>
        private void UpdateMovementVector()
        {
            movementVector = moveLeftToRight ? Vector3.right : Vector3.left;

            if (showMovementLogs && lastMoveDirection != moveLeftToRight)
            {
                string oldDirection = lastMoveDirection ? "Esquerda → Direita" : "Direita → Esquerda";
                Debug.Log($"[WindHandler] Direção de movimento alterada: {oldDirection} → {CurrentDirectionText}");
                lastMoveDirection = moveLeftToRight;
            }
        }

        /// <summary>
        /// Atualiza o estado de flip baseado na direção
        /// </summary>
        private void UpdateFlipState()
        {
            if (!autoFlipSprite) return;

            bool shouldFlip = moveLeftToRight;

            switch (flipMethod)
            {
                case FlipMethod.SpriteRenderer:
                    UpdateSpriteFlip(shouldFlip);
                    break;
                case FlipMethod.Transform:
                    UpdateTransformFlip(shouldFlip);
                    break;
            }
        }

        /// <summary>
        /// Atualiza flip usando SpriteRenderer
        /// </summary>
        private void UpdateSpriteFlip(bool shouldFlip)
        {
            if (spriteRenderer == null) return;

            spriteRenderer.flipX = shouldFlip ? !originalSpriteFlipX : originalSpriteFlipX;

            if (showMovementLogs)
            {
                Debug.Log($"[WindHandler] SpriteRenderer flipX: {spriteRenderer.flipX}");
            }
        }

        /// <summary>
        /// Atualiza flip usando Transform scale
        /// </summary>
        private void UpdateTransformFlip(bool shouldFlip)
        {
            Vector3 scale = originalScale;
            if (shouldFlip)
            {
                scale.x = -Mathf.Abs(originalScale.x);
            }
            else
            {
                scale.x = Mathf.Abs(originalScale.x);
            }

            transform.localScale = scale;

            if (showMovementLogs)
            {
                Debug.Log($"[WindHandler] Transform scale.x: {scale.x}");
            }
        }
        #endregion

        #region Movement System
        /// <summary>
        /// Atualiza a posição do objeto baseado no movimento configurado
        /// </summary>
        private void UpdateMovement()
        {
            float deltaTime = Time.deltaTime;

            // Otimização: evita cálculos se deltaTime é muito pequeno
            if (deltaTime < TIME_DELTA_THRESHOLD) return;

            Vector3 movement = movementVector * (movementSpeed * deltaTime);
            cachedTransform.position += movement;
        }

        /// <summary>
        /// Altera a direção de movimento
        /// </summary>
        public void SetMovementDirection(bool leftToRight)
        {
            moveLeftToRight = leftToRight;
            UpdateMovementVector();
            UpdateFlipState();

            if (showMovementLogs)
            {
                Debug.Log($"[WindHandler] Direção alterada para: {CurrentDirectionText}");
            }
        }

        /// <summary>
        /// Altera a velocidade de movimento
        /// </summary>
        public void SetMovementSpeed(float newSpeed)
        {
            movementSpeed = Mathf.Max(0.1f, newSpeed);

            if (showMovementLogs)
            {
                Debug.Log($"[WindHandler] Velocidade alterada para: {movementSpeed}");
            }
        }

        /// <summary>
        /// Liga/desliga o movimento
        /// </summary>
        public void SetMovementEnabled(bool enabled)
        {
            enableMovement = enabled;

            if (showMovementLogs)
            {
                Debug.Log($"[WindHandler] Movimento {(enabled ? "ativado" : "desativado")}");
            }
        }

        /// <summary>
        /// Para o movimento instantaneamente
        /// </summary>
        [ContextMenu("Stop Movement")]
        public void StopMovement()
        {
            SetMovementEnabled(false);
        }

        /// <summary>
        /// Retoma o movimento
        /// </summary>
        [ContextMenu("Resume Movement")]
        public void ResumeMovement()
        {
            SetMovementEnabled(true);
        }

        /// <summary>
        /// Inverte a direção do movimento
        /// </summary>
        [ContextMenu("Reverse Movement Direction")]
        public void ReverseMovementDirection()
        {
            SetMovementDirection(!moveLeftToRight);
        }

        /// <summary>
        /// Define movimento da direita para esquerda (padrão)
        /// </summary>
        [ContextMenu("Set Right To Left")]
        public void SetRightToLeft()
        {
            SetMovementDirection(false);
        }

        /// <summary>
        /// Define movimento da esquerda para direita
        /// </summary>
        [ContextMenu("Set Left To Right")]
        public void SetLeftToRight()
        {
            SetMovementDirection(true);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Inicia a detecção de proximidade
        /// </summary>
        public void StartProximityChecking()
        {
            if (proximityCheckCoroutine != null)
            {
                StopCoroutine(proximityCheckCoroutine);
            }

            proximityCheckCoroutine = StartCoroutine(ProximityCheckCoroutine());
            IsActive = true;

            if (showAnimationLogs)
            {
                Debug.Log("[WindHandler] Detecção de proximidade iniciada");
            }
        }

        /// <summary>
        /// Para a detecção de proximidade
        /// </summary>
        public void StopProximityChecking()
        {
            if (proximityCheckCoroutine != null)
            {
                StopCoroutine(proximityCheckCoroutine);
                proximityCheckCoroutine = null;
            }

            IsActive = false;

            if (showAnimationLogs)
            {
                Debug.Log("[WindHandler] Detecção de proximidade parada");
            }
        }

        /// <summary>
        /// Força a ativação da trigger em todos os objetos próximos
        /// </summary>
        [ContextMenu("Force Shake All Nearby Objects")]
        public void ForceShakeAllNearbyObjects()
        {
            if (!IsActive)
            {
                Debug.LogWarning("[WindHandler] Sistema não está ativo. Use StartProximityChecking() primeiro.");
                return;
            }

            int activatedCount = 0;
            foreach (GameObject obj in currentlyNearbyObjects)
            {
                if (TryActivateShakeForce(obj))
                {
                    activatedCount++;
                }
            }

            Debug.Log($"[WindHandler] Trigger forçada em {activatedCount}/{currentlyNearbyObjects.Count} objetos");
        }

        /// <summary>
        /// Verifica se um objeto específico está no range
        /// </summary>
        public bool IsObjectInRange(GameObject targetObject)
        {
            return targetObject != null && currentlyNearbyObjects.Contains(targetObject);
        }

        /// <summary>
        /// Retorna uma cópia do array de objetos próximos
        /// </summary>
        public GameObject[] GetNearbyObjects()
        {
            var result = new GameObject[currentlyNearbyObjects.Count];
            currentlyNearbyObjects.CopyTo(result);
            return result;
        }

        /// <summary>
        /// Retorna o objeto mais próximo do WindHandler
        /// </summary>
        public GameObject GetClosestNearbyObject()
        {
            GameObject closest = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject obj in currentlyNearbyObjects)
            {
                if (obj == null) continue;

                float distance = Vector2.Distance(transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = obj;
                }
            }

            return closest;
        }

        /// <summary>
        /// Configura novo nome para a trigger
        /// </summary>
        public void SetShakeTriggerName(string newTriggerName)
        {
            if (string.IsNullOrEmpty(newTriggerName))
            {
                Debug.LogError("[WindHandler] Nome da trigger não pode ser vazio");
                return;
            }

            shakeTriggerName = newTriggerName;
            Debug.Log($"[WindHandler] Nome da trigger alterado para: '{shakeTriggerName}'");
        }

        /// <summary>
        /// Configura novo cooldown para as triggers
        /// </summary>
        public void SetTriggerCooldown(float newCooldown)
        {
            triggerCooldown = Mathf.Max(0.1f, newCooldown);
            Debug.Log($"[WindHandler] Cooldown da trigger alterado para: {triggerCooldown}s");
        }

        /// <summary>
        /// Atualiza as tags alvo durante runtime
        /// </summary>
        public void SetTargetTags(string[] newTags)
        {
            if (newTags == null || newTags.Length == 0)
            {
                Debug.LogWarning("[WindHandler] Array de tags não pode ser vazio");
                return;
            }

            targetTags = newTags;
            Debug.Log($"[WindHandler] Tags alvo atualizadas: [{string.Join(", ", targetTags)}]");
        }
        #endregion

        #region Private Methods - Core Logic
        /// <summary>
        /// Coroutine principal para verificação de proximidade
        /// </summary>
        private IEnumerator ProximityCheckCoroutine()
        {
            while (IsActive)
            {
                CheckProximity();
                yield return checkInterval;
            }
        }

        /// <summary>
        /// Verifica objetos próximos e processa mudanças
        /// </summary>
        private void CheckProximity()
        {
            // Limpa buffer e detecta novos objetos
            detectionBuffer.Clear();

            // Cache da posição para evitar múltiplas chamadas a transform.position
            Vector2 currentPosition = cachedTransform.position;

            var detected = Physics2D.OverlapCircleAll(
                currentPosition,
                detectionRadius,
                objectLayerMask
            );

            detectionBuffer.AddRange(detected);

            // Processa objetos detectados
            var detectedThisFrame = new HashSet<GameObject>();
            ProcessDetectedObjects(detectedThisFrame);

            // Processa objetos que saíram do range
            ProcessObjectsExitingRange(detectedThisFrame);

            // Atualiza lista atual
            currentlyNearbyObjects.Clear();
            currentlyNearbyObjects.UnionWith(detectedThisFrame);
        }

        /// <summary>
        /// Processa objetos detectados no frame atual
        /// </summary>
        private void ProcessDetectedObjects(HashSet<GameObject> detectedThisFrame)
        {
            foreach (var collider in detectionBuffer)
            {
                if (collider == null) continue;

                GameObject detectedObj = collider.gameObject;

                if (!HasValidTag(detectedObj)) continue;

                detectedThisFrame.Add(detectedObj);

                if (!currentlyNearbyObjects.Contains(detectedObj))
                {
                    OnObjectEnterRange(detectedObj);
                }
                else
                {
                    TryActivateShakeTrigger(detectedObj);
                }
            }
        }

        /// <summary>
        /// Processa objetos que saíram do range
        /// </summary>
        private void ProcessObjectsExitingRange(HashSet<GameObject> detectedThisFrame)
        {
            var objectsToRemove = new List<GameObject>();

            foreach (GameObject nearbyObj in currentlyNearbyObjects)
            {
                if (nearbyObj == null || !detectedThisFrame.Contains(nearbyObj))
                {
                    objectsToRemove.Add(nearbyObj);
                }
            }

            foreach (GameObject objToRemove in objectsToRemove)
            {
                OnObjectExitRange(objToRemove);
            }
        }

        /// <summary>
        /// Verifica se o objeto possui uma tag válida
        /// </summary>
        private bool HasValidTag(GameObject obj)
        {
            if (targetTags == null) return false;

            foreach (string tag in targetTags)
            {
                if (obj.CompareTag(tag))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Private Methods - Object Events
        /// <summary>
        /// Chamado quando um objeto entra no range
        /// </summary>
        private void OnObjectEnterRange(GameObject targetObject)
        {
            if (showAnimationLogs)
            {
                Debug.Log($"[WindHandler] Objeto '{targetObject.name}' com tag '{targetObject.tag}' entrou no range");
            }

            ActivateShakeTrigger(targetObject);
        }

        /// <summary>
        /// Chamado quando um objeto sai do range
        /// </summary>
        private void OnObjectExitRange(GameObject targetObject)
        {
            if (showAnimationLogs)
            {
                Debug.Log($"[WindHandler] Objeto '{targetObject.name}' saiu do range");
            }

            // Remove cooldown e cache
            RemoveObjectFromTracking(targetObject);
        }

        /// <summary>
        /// Remove objeto dos sistemas de tracking
        /// </summary>
        private void RemoveObjectFromTracking(GameObject targetObject)
        {
            triggerCooldowns.Remove(targetObject);
            animatorCache.Remove(targetObject);
        }
        #endregion

        #region Private Methods - Animation
        /// <summary>
        /// Tenta ativar trigger respeitando cooldown
        /// </summary>
        private void TryActivateShakeTrigger(GameObject targetObject)
        {
            if (triggerCooldowns.TryGetValue(targetObject, out float cooldownTime))
            {
                if (Time.time < cooldownTime)
                {
                    return; // Ainda em cooldown
                }
            }

            ActivateShakeTrigger(targetObject);
        }

        /// <summary>
        /// Ativa a trigger Shake no objeto
        /// </summary>
        private void ActivateShakeTrigger(GameObject targetObject)
        {
            var animator = GetCachedAnimator(targetObject);

            if (animator != null)
            {
                animator.SetTrigger(shakeTriggerName);
                triggerCooldowns[targetObject] = Time.time + triggerCooldown;

                if (showAnimationLogs)
                {
                    Debug.Log($"[WindHandler] Trigger '{shakeTriggerName}' ativada em '{targetObject.name}'");
                }
            }
            else if (showAnimationLogs)
            {
                Debug.LogWarning($"[WindHandler] Objeto '{targetObject.name}' não possui Animator");
            }
        }

        /// <summary>
        /// Força ativação de trigger ignorando cooldown
        /// </summary>
        private bool TryActivateShakeForce(GameObject targetObject)
        {
            var animator = GetCachedAnimator(targetObject);

            if (animator != null)
            {
                animator.SetTrigger(shakeTriggerName);
                Debug.Log($"[WindHandler] Trigger forçada em '{targetObject.name}'");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Obtém Animator do cache ou busca e armazena
        /// </summary>
        private Animator GetCachedAnimator(GameObject targetObject)
        {
            if (!animatorCache.TryGetValue(targetObject, out Animator animator))
            {
                animator = targetObject.GetComponent<Animator>();
                animatorCache[targetObject] = animator; // Pode ser null, mas evita buscas repetidas
            }

            return animator;
        }
        #endregion

        #region Private Methods - Utilities
        /// <summary>
        /// Limpa todos os dados do sistema
        /// </summary>
        private void ClearAllData()
        {
            currentlyNearbyObjects.Clear();
            detectionBuffer.Clear();
            triggerCooldowns.Clear();
            animatorCache.Clear();
        }

        /// <summary>
        /// Desenha gizmos de debug no editor
        /// </summary>
        private void DrawDetectionGizmos()
        {
            // Desenha raio de detecção
            Gizmos.color = detectionRadiusColor;
            DrawWireCircle(transform.position, detectionRadius);

            // Desenha objetos detectados apenas durante runtime
            if (!Application.isPlaying || currentlyNearbyObjects.Count == 0) return;

            foreach (GameObject obj in currentlyNearbyObjects)
            {
                if (obj == null) continue;

                // Linha de conexão
                Gizmos.color = detectedObjectColor;
                Gizmos.DrawLine(transform.position, obj.transform.position);

                // Cubo indicando objeto detectado
                Gizmos.DrawWireCube(obj.transform.position, Vector3.one * GIZMO_OBJECT_SIZE);

                // Esfera indicando se tem Animator
                if (GetCachedAnimator(obj) != null)
                {
                    Gizmos.color = animatorDetectedColor;
                    Gizmos.DrawWireSphere(obj.transform.position, GIZMO_ANIMATOR_RADIUS);
                }
            }
        }

        /// <summary>
        /// Desenha gizmos relacionados ao movimento
        /// </summary>
        private void DrawMovementGizmos()
        {
            if (!enableMovement) return;

            // Desenha seta indicando direção do movimento
            Gizmos.color = movementDirectionColor;
            Vector3 arrowEnd = transform.position + (movementVector * GIZMO_DIRECTION_ARROW_LENGTH);

            // Linha principal da seta
            Gizmos.DrawLine(transform.position, arrowEnd);

            // Pontas da seta
            Vector3 arrowHead1 = arrowEnd + (Quaternion.Euler(0, 0, 135) * movementVector * 0.5f);
            Vector3 arrowHead2 = arrowEnd + (Quaternion.Euler(0, 0, -135) * movementVector * 0.5f);

            Gizmos.DrawLine(arrowEnd, arrowHead1);
            Gizmos.DrawLine(arrowEnd, arrowHead2);

            // Label com informações de movimento
#if UNITY_EDITOR
            UnityEditor.Handles.Label(arrowEnd + Vector3.up * 0.5f,
                $"Direção: {CurrentDirectionText}\nVelocidade: {movementSpeed:F1}");
#endif
        }

        /// <summary>
        /// Desenha um círculo usando Gizmos
        /// </summary>
        private static void DrawWireCircle(Vector3 position, float radius)
        {
            Vector3 lastPoint = Vector3.zero;

            for (int i = 0; i <= GIZMO_CIRCLE_SEGMENTS; i++)
            {
                float angle = (float)i / GIZMO_CIRCLE_SEGMENTS * 2 * Mathf.PI;
                Vector3 newPoint = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                ) + position;

                if (i > 0)
                {
                    Gizmos.DrawLine(lastPoint, newPoint);
                }

                lastPoint = newPoint;
            }
        }
        #endregion
    }
}
