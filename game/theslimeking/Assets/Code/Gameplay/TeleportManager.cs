namespace PixeLadder.EasyTransition
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Gerenciador unificado para teletransporte entre cenas.
    /// Responsável por reprodução de áudio e orquestração de teletransporte cross-scene.
    /// Implementa padrão Singleton para acesso global.
    /// </summary>
    public class TeleportManager : MonoBehaviour
    {
        #region Singleton

        /// <summary>
        /// Instância singleton do TeleportManager.
        /// </summary>
        public static TeleportManager Instance { get; private set; }

        #endregion

        #region Serialized Fields

        [Header("Audio")]
        [Tooltip("AudioSource usado para reproduzir sons de teletransporte")]
        [SerializeField] private AudioSource audioSource;

        [Tooltip("Volume padrão para sons de teletransporte (0.0 a 1.0)")]
        [SerializeField][Range(0f, 1f)] private float defaultVolume = 1f;

        #endregion

        #region Private Fields

        /// <summary>
        /// Flag que indica se um teletransporte está em progresso.
        /// Previne múltiplos teletransportes simultâneos.
        /// </summary>
        private bool isTeleporting = false;

        #endregion

        #region Properties

        /// <summary>
        /// Verifica se um teletransporte está em progresso.
        /// </summary>
        public bool IsTeleporting => isTeleporting;

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// Inicializa o singleton e estruturas de dados.
        /// </summary>
        private void Awake()
        {
            // Implementa padrão Singleton
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"TeleportManager: Múltiplas instâncias detectadas! Destruindo duplicata em '{gameObject.name}'.", this);
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Valida AudioSource
            if (audioSource == null)
            {
                Debug.LogWarning("TeleportManager: AudioSource não atribuído. Sons de teletransporte não serão reproduzidos.", this);
            }

            Debug.Log("TeleportManager: Inicializado com sucesso.", this);
        }

        #endregion

        #region Scene Validation

        /// <summary>
        /// Verifica se uma cena existe nas Build Settings do projeto.
        /// </summary>
        /// <param name="sceneName">Nome da cena a ser validada</param>
        /// <returns>True se a cena existe nas Build Settings, false caso contrário</returns>
        private bool IsSceneInBuildSettings(string sceneName)
        {
            // Valida entrada
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            // Itera por todas as cenas nas Build Settings
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameFromPath = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (sceneNameFromPath == sceneName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Valida se um teletransporte cross-scene pode ser executado.
        /// Realiza verificações abrangentes de configuração e estado.
        /// </summary>
        /// <param name="destinationSceneName">Nome da cena de destino</param>
        /// <param name="transitionEffect">Efeito de transição a ser usado</param>
        /// <param name="enableDebugLogs">Se true, loga informações de debug</param>
        /// <returns>True se o teletransporte pode ser executado, false caso contrário</returns>
        private bool ValidateCrossSceneTeleport(string destinationSceneName, TransitionEffect transitionEffect, bool enableDebugLogs)
        {
            // Validação 1: Verifica se já existe um teletransporte em progresso
            if (isTeleporting)
            {
                Debug.LogWarning("TeleportManager: Não é possível iniciar teletransporte - outro teletransporte já está em progresso.");
                return false;
            }

            // Validação 2: Verifica se o nome da cena é válido
            if (string.IsNullOrEmpty(destinationSceneName))
            {
                Debug.LogError("TeleportManager: Nome da cena de destino está vazio ou nulo. Teletransporte abortado.");
                return false;
            }

            // Validação 3: Verifica se a cena existe nas Build Settings
            if (!IsSceneInBuildSettings(destinationSceneName))
            {
                Debug.LogError($"TeleportManager: Cena '{destinationSceneName}' não foi encontrada nas Build Settings. " +
                              $"Adicione a cena em File > Build Settings antes de usar teletransporte cross-scene.");
                return false;
            }

            // Validação 4: Verifica se a cena de destino não é a cena atual
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == destinationSceneName)
            {
                Debug.LogWarning($"TeleportManager: Cena de destino '{destinationSceneName}' é a mesma que a cena atual. " +
                                $"Use teletransporte same-scene ao invés de cross-scene.");
                return false;
            }

            // Validação 5: Verifica se o TransitionEffect está atribuído
            if (transitionEffect == null)
            {
                Debug.LogError("TeleportManager: TransitionEffect não está atribuído. Teletransporte abortado.");
                return false;
            }

            // Validação 6: Verifica se o PlayerController existe
            if (PlayerController.Instance == null)
            {
                Debug.LogError("TeleportManager: PlayerController.Instance não encontrado. " +
                              "Certifique-se de que o Player existe na cena.");
                return false;
            }

            // Validação 7: Verifica se o Player tem Rigidbody2D
            Rigidbody2D playerRb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                Debug.LogError("TeleportManager: Player não possui Rigidbody2D. " +
                              "Componente necessário para teletransporte.");
                return false;
            }

            // Validação 8: Verifica se a câmera principal existe
            if (Camera.main == null)
            {
                Debug.LogWarning("TeleportManager: Camera.main não encontrada. " +
                                "Posicionamento da câmera pode não funcionar corretamente.");
                // Não aborta - câmera é opcional
            }

            // Log de sucesso se debug está habilitado
            if (enableDebugLogs)
            {
                Debug.Log($"TeleportManager: Validação bem-sucedida para teletransporte para '{destinationSceneName}'.");
            }

            return true;
        }

        #endregion

        #region Audio Management

        /// <summary>
        /// Reproduz um som de teletransporte se o AudioClip estiver configurado.
        /// Implementa graceful degradation - continua funcionando mesmo sem som.
        /// </summary>
        /// <param name="clip">AudioClip a ser reproduzido (pode ser null)</param>
        public void PlayTeleportSound(AudioClip clip)
        {
            // Graceful degradation - não faz nada se clip ou audioSource não estiverem configurados
            if (clip == null)
            {
                return;
            }

            if (audioSource == null)
            {
                Debug.LogWarning("TeleportManager: AudioSource não configurado. Som não será reproduzido.");
                return;
            }

            // Reproduz som com volume configurado
            PlaySound(clip, defaultVolume);
        }

        /// <summary>
        /// Método privado para reproduzir som com controle de volume.
        /// </summary>
        /// <param name="clip">AudioClip a ser reproduzido</param>
        /// <param name="volumeMultiplier">Multiplicador de volume (0.0 a 1.0)</param>
        private void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (audioSource == null || clip == null)
            {
                return;
            }

            // Calcula volume final baseado nas configurações do jogo
            float finalVolume = GetGameVolume() * volumeMultiplier;

            // Usa PlayOneShot para não bloquear outros sons
            audioSource.PlayOneShot(clip, finalVolume);
        }

        /// <summary>
        /// Obtém o volume configurado nas configurações do jogo.
        /// TODO: Integrar com sistema de configurações quando implementado.
        /// </summary>
        /// <returns>Volume de 0.0 a 1.0</returns>
        private float GetGameVolume()
        {
            // TODO: Integrar com sistema de configurações do jogo
            // Por enquanto, retorna volume padrão
            return defaultVolume;
        }

        #endregion

        #region Cross-Scene Teleport Orchestration

        /// <summary>
        /// Executa teletransporte completo entre cenas.
        /// Coordena toda a sequência: validação, desabilitar player, transição visual, 
        /// carregamento de cena, transferência do player, e reativação.
        /// </summary>
        /// <param name="destinationSceneName">Nome da cena de destino</param>
        /// <param name="destinationPosition">Posição de destino na nova cena</param>
        /// <param name="transitionEffect">Efeito de transição visual</param>
        /// <param name="delayBeforeFadeIn">Delay antes do fade in</param>
        /// <param name="startSound">Som de início do teletransporte (opcional)</param>
        /// <param name="midSound">Som do meio da transição (opcional)</param>
        /// <param name="endSound">Som de chegada (opcional)</param>
        /// <param name="enableDebugLogs">Habilita logs de debug</param>
        public void ExecuteCrossSceneTeleport(
            string destinationSceneName,
            Vector3 destinationPosition,
            TransitionEffect transitionEffect,
            float delayBeforeFadeIn,
            AudioClip startSound,
            AudioClip midSound,
            AudioClip endSound,
            bool enableDebugLogs)
        {
            // Validação completa antes de iniciar
            if (!ValidateCrossSceneTeleport(destinationSceneName, transitionEffect, enableDebugLogs))
            {
                Debug.LogError("TeleportManager: Validação falhou. Teletransporte abortado.");
                return;
            }

            // Implementa teleport lock para prevenir teletransportes simultâneos
            if (isTeleporting)
            {
                Debug.LogWarning("TeleportManager: Teletransporte já em progresso. Ignorando nova solicitação.");
                return;
            }

            // Marca que teletransporte está em progresso
            isTeleporting = true;

            if (enableDebugLogs)
            {
                Debug.Log($"TeleportManager: Iniciando teletransporte cross-scene para '{destinationSceneName}' " +
                         $"na posição {destinationPosition}");
            }

            // Desabilita movimento do player
            if (PlayerController.Instance != null)
            {
                // Desabilita movimento e ataque do player
                PlayerController.Instance.DisableMovement();

                // Zera velocidade do Rigidbody2D
                Rigidbody2D playerRb = PlayerController.Instance.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = Vector2.zero;

                    if (enableDebugLogs)
                    {
                        Debug.Log("TeleportManager: Velocidade do player zerada.");
                    }
                }

                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: Movimento do player desabilitado.");
                }
            }

            // Inicia a sequência de teletransporte
            StartCoroutine(TeleportSequence(
                destinationSceneName,
                destinationPosition,
                transitionEffect,
                delayBeforeFadeIn,
                startSound,
                midSound,
                endSound,
                enableDebugLogs
            ));
        }

        /// <summary>
        /// Coroutine que executa a sequência completa de teletransporte cross-scene.
        /// Coordena: sons, transições visuais, carregamento de cena, transferência do player.
        /// </summary>
        private IEnumerator TeleportSequence(
            string destinationSceneName,
            Vector3 destinationPosition,
            TransitionEffect transitionEffect,
            float delayBeforeFadeIn,
            AudioClip startSound,
            AudioClip midSound,
            AudioClip endSound,
            bool enableDebugLogs)
        {
            try
            {
                // Fase 1: Play start sound
                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: Fase 1 - Reproduzindo som de início...");
                }
                PlayTeleportSound(startSound);

                // Fase 2: Execute fade out usando TeleportTransitionHelper
                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: Fase 2 - Executando fade out...");
                }

                bool midTransitionExecuted = false;
                string previousSceneName = SceneManager.GetActiveScene().name;

                // Executa transição com callback no meio
                yield return TeleportTransitionHelper.ExecuteTransition(
                    transitionEffect,
                    onMidTransition: () =>
                    {
                        // Este callback é executado após o fade out, quando a tela está escura
                        if (enableDebugLogs)
                        {
                            Debug.Log("TeleportManager: Callback mid-transition - Tela escura, iniciando transferência...");
                        }

                        // Inicia carregamento/ativação da cena se necessário
                        StartCoroutine(LoadAndTransferPlayer(
                            destinationSceneName,
                            destinationPosition,
                            previousSceneName,
                            midSound,
                            enableDebugLogs
                        ));

                        midTransitionExecuted = true;
                    },
                    delayBeforeFadeIn,
                    enableDebugLogs
                );

                // Aguarda um frame adicional para garantir que tudo foi processado
                yield return null;

                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: Transição visual completa.");
                }

                // Fase 3: Play end sound após fade in
                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: Fase 3 - Reproduzindo som de chegada...");
                }
                PlayTeleportSound(endSound);

                if (enableDebugLogs)
                {
                    Debug.Log($"TeleportManager: Teletransporte para '{destinationSceneName}' concluído com sucesso!");
                }
            }
            finally
            {
                // Reabilita movimento do player
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.EnableMovement();

                    if (enableDebugLogs)
                    {
                        Debug.Log("TeleportManager: Movimento do player reabilitado.");
                    }
                }

                // Clear teleport lock no finally block para garantir que sempre será limpo
                isTeleporting = false;

                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: Teleport lock liberado.");
                }
            }
        }

        /// <summary>
        /// Coroutine auxiliar que carrega a cena diretamente e transfere o player.
        /// Executada durante o mid-transition quando a tela está escura.
        /// </summary>
        private IEnumerator LoadAndTransferPlayer(
            string destinationSceneName,
            Vector3 destinationPosition,
            string previousSceneName,
            AudioClip midSound,
            bool enableDebugLogs)
        {
            // Reproduz som do meio da transição
            PlayTeleportSound(midSound);

            if (enableDebugLogs)
            {
                Debug.Log($"TeleportManager: Carregando cena '{destinationSceneName}' usando LoadSceneMode.Single...");
            }

            // IMPORTANTE: Salva referência do player ANTES de carregar a nova cena
            GameObject playerObject = PlayerController.Instance != null ? PlayerController.Instance.gameObject : null;

            if (playerObject != null)
            {
                // Aplica DontDestroyOnLoad para prevenir destruição durante transição
                DontDestroyOnLoad(playerObject);

                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: DontDestroyOnLoad aplicado ao Player antes do carregamento.");
                }
            }

            // Carrega cena usando LoadSceneMode.Single
            // Isso automaticamente descarrega TODAS as cenas anteriores
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(destinationSceneName, LoadSceneMode.Single);

            if (loadOperation == null)
            {
                Debug.LogError($"TeleportManager: Falha ao carregar cena '{destinationSceneName}'!");
                yield break;
            }

            // Aguarda carregamento completo
            while (!loadOperation.isDone)
            {
                if (enableDebugLogs)
                {
                    Debug.Log($"TeleportManager: Progresso de carregamento: {loadOperation.progress * 100f:F1}%");
                }
                yield return null;
            }

            if (enableDebugLogs)
            {
                Debug.Log($"TeleportManager: Cena '{destinationSceneName}' carregada com sucesso.");
                Debug.Log($"TeleportManager: Cena ativa: {SceneManager.GetActiveScene().name}");
                Debug.Log($"TeleportManager: Total de cenas carregadas: {SceneManager.sceneCount}");
            }

            // Aguarda um frame adicional para garantir que a cena foi inicializada
            yield return null;

            // Reposiciona o player na nova cena
            RepositionPlayerInNewScene(destinationPosition, enableDebugLogs);

            if (enableDebugLogs)
            {
                Debug.Log("TeleportManager: Player reposicionado na nova cena.");
            }
        }

        /// <summary>
        /// Reposiciona o Player na nova cena após o carregamento com LoadSceneMode.Single.
        /// Como o player tem DontDestroyOnLoad, ele persiste entre cenas.
        /// </summary>
        private void RepositionPlayerInNewScene(Vector3 destinationPosition, bool enableDebugLogs)
        {
            if (PlayerController.Instance == null)
            {
                Debug.LogError("TeleportManager: PlayerController.Instance não encontrado após carregamento da cena!");
                return;
            }

            GameObject playerObject = PlayerController.Instance.gameObject;

            // Calcula offset da câmera antes de mover o player
            Vector3 cameraOffset = Vector3.zero;
            if (Camera.main != null)
            {
                cameraOffset = Camera.main.transform.position - playerObject.transform.position;

                if (enableDebugLogs)
                {
                    Debug.Log($"TeleportManager: Camera offset calculado: {cameraOffset}");
                }
            }

            // Reposiciona o player na posição de destino
            playerObject.transform.position = destinationPosition;

            if (enableDebugLogs)
            {
                Debug.Log($"TeleportManager: Player reposicionado para {destinationPosition}.");
            }

            // Garante que a Cinemachine Camera esteja seguindo o Player após reposicionamento
            if (SlimeKing.Core.CameraManager.HasInstance)
            {
                SlimeKing.Core.CameraManager.Instance.ForceCinemachineSetup();

                if (enableDebugLogs)
                {
                    Debug.Log("TeleportManager: Cinemachine Camera configurada via CameraManager.");
                }
            }

            // Atualiza posição da câmera mantendo o offset
            if (Camera.main != null)
            {
                Camera.main.transform.position = destinationPosition + cameraOffset;

                if (enableDebugLogs)
                {
                    Debug.Log($"TeleportManager: Câmera reposicionada para {Camera.main.transform.position}.");
                }
            }
            else if (enableDebugLogs)
            {
                Debug.LogWarning("TeleportManager: Camera.main não encontrada. Posição da câmera não atualizada.");
            }
        }

        /// <summary>
        /// OBSOLETO: Não é mais necessário com LoadSceneMode.Single.
        /// Mantido para compatibilidade mas não faz nada.
        /// </summary>
        private IEnumerator UnloadSceneAsync(string sceneName, bool enableDebugLogs)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"TeleportManager: Iniciando descarregamento assíncrono de '{sceneName}'...");
            }

            // Descarrega a cena
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);

            if (unloadOperation == null)
            {
                Debug.LogError($"TeleportManager: Falha ao iniciar descarregamento de '{sceneName}'.");
                yield break;
            }

            // Aguarda conclusão do descarregamento
            yield return unloadOperation;

            if (enableDebugLogs)
            {
                Debug.Log($"TeleportManager: Cena '{sceneName}' descarregada com sucesso.");
                Debug.Log($"TeleportManager: Cenas restantes: {SceneManager.sceneCount}");
            }

            // Libera recursos não utilizados
            if (enableDebugLogs)
            {
                Debug.Log("TeleportManager: Liberando recursos não utilizados...");
            }

            yield return Resources.UnloadUnusedAssets();

            if (enableDebugLogs)
            {
                Debug.Log("TeleportManager: Recursos liberados.");
            }

            // Opcional: Força coleta de lixo para limpeza imediata de memória
            // Comentado por padrão pois pode causar stuttering
            // Descomente se necessário para liberar memória imediatamente
            // System.GC.Collect();
            // if (enableDebugLogs)
            // {
            //     Debug.Log("TeleportManager: Garbage collection executado.");
            // }
        }

        #endregion
    }
}
