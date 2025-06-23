using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheSlimeKing.Core.Portal
{
    /// <summary>
    /// Gerencia o sistema de portais, permitindo teleporte entre pontos na mesma cena
    /// ou entre cenas diferentes.
    /// </summary>
    public class PortalManager : MonoBehaviour
    {
        public static PortalManager Instance { get; private set; }

        [Header("Configurações de Teleporte")]
        [SerializeField] private float _sameCeneTeleportDelay = 0.5f;
        [SerializeField] private float _portalCooldown = 1.0f;

        [Header("Efeitos")]
        [SerializeField] private GameObject _teleportOutEffectPrefab;
        [SerializeField] private GameObject _teleportInEffectPrefab;
        [SerializeField] private AudioClip _teleportSound;
        [Range(0, 1)]
        [SerializeField] private float _teleportSoundVolume = 0.6f;

        [Header("Transição entre Cenas")]
        [SerializeField] private GameObject _sceneTransitionPrefab;
        [SerializeField] private float _fadeOutDuration = 1.0f;
        [SerializeField] private float _fadeInDuration = 0.8f;

        // Propriedades privadas
        private bool _isTeleporting = false;
        private AudioSource _audioSource;
        private string _targetSceneName;
        private Vector3 _targetPosition;
        private Vector3 _targetRotation;
        private bool _isSceneTransition = false;

        private void Awake()
        {
            // Configuração do singleton
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Registra no evento de carregamento de cena para posicionamento pós-load
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Inicia o teleporte na mesma cena entre dois pontos.
        /// </summary>
        /// <param name="player">Referência ao objeto do jogador</param>
        /// <param name="destination">Posição de destino</param>
        /// <param name="lookDirection">Direção para onde o jogador olhará (opcional)</param>
        public void TeleportIntraSameCene(GameObject player, Vector3 destination, Vector3? lookDirection = null)
        {
            if (_isTeleporting) return;

            StartCoroutine(IntraSameCeneTeleportCoroutine(player, destination, lookDirection));
        }

        /// <summary>
        /// Inicia o teleporte entre cenas.
        /// </summary>
        /// <param name="targetSceneName">Nome da cena de destino</param>
        /// <param name="targetPortalID">ID do portal na cena destino (opcional)</param>
        /// <param name="targetPosition">Posição na cena destino, caso não use ID de portal</param>
        /// <param name="targetRotation">Rotação na cena destino (opcional)</param>
        public void TeleportToScene(string targetSceneName, string targetPortalID = null, Vector3? targetPosition = null, Vector3? targetRotation = null)
        {
            if (_isTeleporting || string.IsNullOrEmpty(targetSceneName)) return;

            _targetSceneName = targetSceneName;
            _isSceneTransition = true;

            // Se tiver um ID de portal destino, é preciso encontrá-lo após carregar a cena
            if (!string.IsNullOrEmpty(targetPortalID))
            {
                PlayerPrefs.SetString("TargetPortalID", targetPortalID);
            }
            // Se tiver posição direta, usa ela
            else if (targetPosition.HasValue)
            {
                _targetPosition = targetPosition.Value;
                if (targetRotation.HasValue)
                    _targetRotation = targetRotation.Value;
            }

            // Salva o estado atual (posição será obtida na nova cena)
            SaveCurrentGameState();

            // Inicia transição de cena
            StartCoroutine(SceneTransitionCoroutine());
        }

        /// <summary>
        /// Chamado quando uma cena é carregada. Posiciona o player se for transição de cenas.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_isSceneTransition)
            {
                _isSceneTransition = false;

                // Verifica se há ID de portal destino
                string targetPortalID = PlayerPrefs.GetString("TargetPortalID", "");
                if (!string.IsNullOrEmpty(targetPortalID))
                {
                    PortalController[] portals = FindObjectsOfType<PortalController>();
                    foreach (var portal in portals)
                    {
                        if (portal.PortalID == targetPortalID)
                        {
                            _targetPosition = portal.transform.position + portal.GetExitOffset();
                            _targetRotation = portal.transform.eulerAngles;
                            break;
                        }
                    }
                    // Limpa o ID
                    PlayerPrefs.DeleteKey("TargetPortalID");
                }

                // Posiciona o player
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = _targetPosition;
                    player.transform.eulerAngles = _targetRotation;

                    // Cria o efeito de entrada
                    if (_teleportInEffectPrefab != null)
                    {
                        Instantiate(_teleportInEffectPrefab, player.transform.position, Quaternion.identity);
                    }
                }
            }
        }

        /// <summary>
        /// Coroutine que gerencia o teleporte na mesma cena.
        /// </summary>
        private IEnumerator IntraSameCeneTeleportCoroutine(GameObject player, Vector3 destination, Vector3? lookDirection)
        {
            _isTeleporting = true;

            // Desabilita controle do jogador
            DisablePlayerControl(player);

            // Efeito de saída
            if (_teleportOutEffectPrefab != null)
            {
                Instantiate(_teleportOutEffectPrefab, player.transform.position, Quaternion.identity);
            }

            // Som de teleporte
            if (_teleportSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_teleportSound, _teleportSoundVolume);
            }

            // Espera o delay configurado
            yield return new WaitForSeconds(_sameCeneTeleportDelay);

            // Teleporta o jogador
            player.transform.position = destination;

            // Configura para onde o jogador está olhando, se especificado
            if (lookDirection.HasValue)
            {
                // Rotaciona apenas no eixo Y para manter jogador na vertical
                player.transform.rotation = Quaternion.Euler(0, lookDirection.Value.y, 0);
            }

            // Efeito de entrada
            if (_teleportInEffectPrefab != null)
            {
                Instantiate(_teleportInEffectPrefab, player.transform.position, Quaternion.identity);
            }

            // Som de teleporte no destino
            if (_teleportSound != null && _audioSource != null)
            {
                _audioSource.PlayOneShot(_teleportSound, _teleportSoundVolume);
            }

            // Reativa controle do jogador
            yield return new WaitForSeconds(0.1f); // Pequeno delay para evitar problemas
            EnablePlayerControl(player);

            // Espera o cooldown terminar
            yield return new WaitForSeconds(_portalCooldown);
            _isTeleporting = false;
        }

        /// <summary>
        /// Coroutine que gerencia a transição entre cenas.
        /// </summary>
        private IEnumerator SceneTransitionCoroutine()
        {
            _isTeleporting = true;

            // Cria o objeto de transição
            GameObject transitionObj = null;
            if (_sceneTransitionPrefab != null)
            {
                transitionObj = Instantiate(_sceneTransitionPrefab);
                // Assume que o prefab tem um componente como SceneTransition com um método FadeIn
                var transition = transitionObj.GetComponent<SceneTransition>();
                if (transition != null)
                {
                    transition.FadeIn(_fadeOutDuration);
                }
            }

            // Espera o fade out completar
            yield return new WaitForSeconds(_fadeOutDuration);

            // Carrega a cena de forma assíncrona
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_targetSceneName);

            // Espera a cena carregar completamente
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // O evento OnSceneLoaded será disparado automaticamente
            // e tratará do posicionamento do jogador

            // Faz o fade out da transição
            if (transitionObj != null)
            {
                var transition = transitionObj.GetComponent<SceneTransition>();
                if (transition != null)
                {
                    transition.FadeOut(_fadeInDuration);
                    yield return new WaitForSeconds(_fadeInDuration);
                    Destroy(transitionObj);
                }
            }

            // Termina o processo de teleporte
            _isTeleporting = false;
        }

        /// <summary>
        /// Salva o estado atual do jogo antes de trocar de cena.
        /// </summary>
        private void SaveCurrentGameState()
        {
            // Em uma implementação completa, salvaria:
            // - Inventário do jogador
            // - Estado dos seguidores
            // - Quests e progresso
            // - Outras informações persistentes

            // Por enquanto, apenas simula a salvaguarda
            Debug.Log("Salvando estado do jogo antes da transição de cena...");

            // Esta é uma simplificação. Em um sistema completo,
            // isso se integraria com um sistema de save/load.
        }

        /// <summary>
        /// Desabilita o controle do jogador durante o teleporte.
        /// </summary>
        private void DisablePlayerControl(GameObject player)
        {
            // Busca pelo controle do jogador e o desativa
            var playerController = player.GetComponent<TheSlimeKing.Gameplay.SlimeInputHandler>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }

        /// <summary>
        /// Reativa o controle do jogador após o teleporte.
        /// </summary>
        private void EnablePlayerControl(GameObject player)
        {
            // Busca pelo controle do jogador e o reativa
            var playerController = player.GetComponent<TheSlimeKing.Gameplay.SlimeInputHandler>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }
}
