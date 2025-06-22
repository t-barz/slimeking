using System.Collections.Generic;
using UnityEngine;
using TheSlimeKing.Core;

namespace TheSlimeKing.Gameplay
{
    /// <summary>
    /// Gerencia os seguidores do slime, que são desbloqueados em diferentes estágios
    /// </summary>
    public class SlimeFollowersManager : MonoBehaviour
    {
        [System.Serializable]
        public class SlimeFollowerConfig
        {
            public GameObject followerPrefab;
            public SlimeStage unlockStage;
            public string followerName;
            [Range(0f, 1f)] public float followDelay = 0.2f;
            public Color tintColor = Color.white;
        }

        [Header("Configurações")]
        [SerializeField] private List<SlimeFollowerConfig> _availableFollowers = new List<SlimeFollowerConfig>();
        [SerializeField] private float _baseFollowDistance = 1.5f;
        [SerializeField] private float _followerSpacing = 0.8f;
        [SerializeField] private float _smoothTime = 0.3f;
        [SerializeField] private float _maxSpeed = 5f;

        [Header("Referências")]
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private Transform _followersParent;

        // Lista de seguidores ativos
        private List<GameObject> _activeFollowers = new List<GameObject>();
        private List<Vector3> _positionHistory = new List<Vector3>();
        private int _historyLimit = 60; // Ajustar conforme necessário
        private Vector2[] _velocities;

        private void Start()
        {
            // Valida configuração
            if (_playerTransform == null)
            {
                _playerTransform = transform;
            }

            if (_followersParent == null)
            {
                GameObject followersObj = new GameObject("Followers");
                followersObj.transform.parent = transform.parent;
                _followersParent = followersObj.transform;
            }

            // Registra eventos
            if (PlayerGrowth.Instance != null)
            {
                PlayerGrowth.Instance.OnGrowthStageChanged.AddListener(HandleGrowthStageChanged);

                // Configura seguidores iniciais baseados no estágio atual
                HandleGrowthStageChanged(PlayerGrowth.Instance.GetCurrentStage());
            }
        }

        private void OnDestroy()
        {
            if (PlayerGrowth.Instance != null)
            {
                PlayerGrowth.Instance.OnGrowthStageChanged.RemoveListener(HandleGrowthStageChanged);
            }
        }

        private void Update()
        {
            // Registra posição do jogador no histórico
            if (_playerTransform != null)
            {
                _positionHistory.Insert(0, _playerTransform.position);

                // Limita tamanho do histórico
                if (_positionHistory.Count > _historyLimit)
                {
                    _positionHistory.RemoveAt(_positionHistory.Count - 1);
                }

                // Atualiza posições dos seguidores
                UpdateFollowersPositions();
            }
        }

        /// <summary>
        /// Atualiza as posições dos seguidores com base no histórico de movimentos
        /// </summary>
        private void UpdateFollowersPositions()
        {
            if (_activeFollowers.Count == 0)
                return;

            // Garante que há velocidades suficientes
            if (_velocities == null || _velocities.Length < _activeFollowers.Count)
            {
                _velocities = new Vector2[_activeFollowers.Count];
            }

            for (int i = 0; i < _activeFollowers.Count; i++)
            {
                GameObject follower = _activeFollowers[i];
                if (follower == null) continue;

                // Calcula posição alvo com base no histórico
                SlimeFollowerConfig config = _availableFollowers[i];
                int historyIndex = Mathf.Min(Mathf.FloorToInt(config.followDelay * 60), _positionHistory.Count - 1);

                if (historyIndex >= 0 && historyIndex < _positionHistory.Count)
                {
                    Vector3 targetPosition = _positionHistory[historyIndex];

                    // Espaçamento entre seguidores
                    targetPosition -= (_playerTransform.position - targetPosition).normalized *
                                     (_baseFollowDistance + i * _followerSpacing);

                    // Movimento suave
                    Vector2 currentPos = follower.transform.position;
                    Vector2 newPos = Vector2.SmoothDamp(
                        currentPos,
                        targetPosition,
                        ref _velocities[i],
                        _smoothTime,
                        _maxSpeed);

                    follower.transform.position = new Vector3(newPos.x, newPos.y, follower.transform.position.z);

                    // Determina direção/orientação do seguidor
                    Vector3 dir = targetPosition - follower.transform.position;
                    if (dir.magnitude > 0.1f)
                    {
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        follower.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                    }
                }
            }
        }

        /// <summary>
        /// Manipula mudança de estágio para atualizar os seguidores disponíveis
        /// </summary>
        private void HandleGrowthStageChanged(SlimeStage newStage)
        {
            ConfigureFollowers(newStage);
        }

        /// <summary>
        /// Configura os seguidores disponíveis para o estágio atual
        /// </summary>
        private void ConfigureFollowers(SlimeStage currentStage)
        {
            // Limpar seguidores atuais
            foreach (GameObject follower in _activeFollowers)
            {
                if (follower != null)
                {
                    Destroy(follower);
                }
            }

            _activeFollowers.Clear();

            // Determina quantos seguidores estão disponíveis neste estágio
            int maxFollowers = 0;
            switch (currentStage)
            {
                case SlimeStage.Baby:
                    maxFollowers = 0; // Baby não tem seguidores
                    break;
                case SlimeStage.Young:
                    maxFollowers = 1; // Young tem 1 seguidor
                    break;
                case SlimeStage.Adult:
                    maxFollowers = 3; // Adult tem até 3 seguidores
                    break;
                case SlimeStage.King:
                    maxFollowers = 4; // King tem até 4 seguidores
                    break;
            }

            // Adiciona seguidores desbloqueados para o estágio atual
            for (int i = 0; i < _availableFollowers.Count && _activeFollowers.Count < maxFollowers; i++)
            {
                SlimeFollowerConfig followerConfig = _availableFollowers[i];

                if ((int)followerConfig.unlockStage <= (int)currentStage)
                {
                    SpawnFollower(followerConfig);
                }
            }

            Debug.Log($"Configurado {_activeFollowers.Count} seguidores para o estágio {currentStage}");
        }

        /// <summary>
        /// Cria um novo seguidor a partir da configuração
        /// </summary>
        private GameObject SpawnFollower(SlimeFollowerConfig config)
        {
            if (config.followerPrefab == null) return null;

            // Cria o seguidor na posição do jogador
            GameObject follower = Instantiate(
                config.followerPrefab,
                _playerTransform.position,
                Quaternion.identity,
                _followersParent
            );

            // Aplica cor
            SpriteRenderer renderer = follower.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = config.tintColor;
            }

            // Nomeia para fácil identificação
            follower.name = config.followerName;

            // Adiciona à lista de ativos
            _activeFollowers.Add(follower);

            return follower;
        }

        /// <summary>
        /// Retorna o número de seguidores atualmente ativos
        /// </summary>
        public int GetActiveFollowersCount()
        {
            return _activeFollowers.Count;
        }

        /// <summary>
        /// Retorna o número máximo possível de seguidores para o estágio atual
        /// </summary>
        public int GetMaxFollowersForCurrentStage()
        {
            if (PlayerGrowth.Instance == null)
                return 0;

            SlimeStage currentStage = PlayerGrowth.Instance.GetCurrentStage();

            switch (currentStage)
            {
                case SlimeStage.Baby: return 0;
                case SlimeStage.Young: return 1;
                case SlimeStage.Adult: return 3;
                case SlimeStage.King: return 4;
                default: return 0;
            }
        }
    }
}
