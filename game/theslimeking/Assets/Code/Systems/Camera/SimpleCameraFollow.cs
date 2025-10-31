using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Componente simples para fazer a câmera seguir o player
    /// </summary>
    public class SimpleCameraFollow : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private bool enableLogs = false;
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
        [SerializeField] private float smoothSpeed = 0.125f;
        [SerializeField] private bool useSmoothing = true;

        private Transform target;
        private bool isInitialized = false;

        private void Start()
        {
            FindPlayer();
        }

        private void LateUpdate()
        {
            if (!isInitialized || target == null)
            {
                FindPlayer();
                return;
            }

            FollowTarget();
        }

        private void FindPlayer()
        {
            // Procura pelo player na cena
            var player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null)
            {
                target = player.transform;
                isInitialized = true;
                Log($"Player encontrado: {player.name}");
            }
            else
            {
                // Tenta encontrar por nome comum
                player = GameObject.Find("chr_whiteslime");
                if (player != null)
                {
                    target = player.transform;
                    isInitialized = true;
                    Log($"Player encontrado por nome: {player.name}");
                }
            }
        }

        private void FollowTarget()
        {
            Vector3 desiredPosition = target.position + offset;

            if (useSmoothing)
            {
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }
            else
            {
                transform.position = desiredPosition;
            }
        }

        /// <summary>
        /// Força a câmera a reposicionar imediatamente no player
        /// </summary>
        public void SnapToPlayer()
        {
            if (target != null)
            {
                transform.position = target.position + offset;
                Log("Câmera reposicionada no player");
            }
        }

        /// <summary>
        /// Define manualmente o target a seguir
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            isInitialized = target != null;
            if (isInitialized)
            {
                SnapToPlayer();
            }
        }

        private void Log(string message)
        {
            if (enableLogs)
                Debug.Log($"[SimpleCameraFollow] {message}");
        }

        private void OnEnable()
        {
            // Quando a câmera é habilitada, tenta encontrar o player novamente
            FindPlayer();
            if (target != null)
            {
                SnapToPlayer();
            }
        }
    }
}
