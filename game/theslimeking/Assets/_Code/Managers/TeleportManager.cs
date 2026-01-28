namespace PixeLadder.EasyTransition
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using SlimeKing.Gameplay;
    using SlimeKing.Core;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Gerenciador unificado para teletransporte entre cenas.
    /// Responsável por reprodução de áudio e orquestração de teletransporte cross-scene.
    /// Herda de ManagerSingleton para garantir persistência correta entre cenas.
    /// </summary>
    public class TeleportManager : ManagerSingleton<TeleportManager>
    {

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

        #region Initialization

        /// <summary>
        /// Inicializa o TeleportManager.
        /// Chamado automaticamente pela classe base ManagerSingleton.
        /// </summary>
        protected override void Initialize()
        {
            // Valida AudioSource
            if (audioSource == null)
            {
                LogWarning("AudioSource não atribuído. Sons de teletransporte não serão reproduzidos.");
            }

            Log("Inicializado com sucesso.");
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
                LogWarning("Não é possível iniciar teletransporte - outro teletransporte já está em progresso.");
                return false;
            }

            // Validação 2: Verifica se o nome da cena é válido
            if (string.IsNullOrEmpty(destinationSceneName))
            {
                LogError("Nome da cena de destino está vazio ou nulo. Teletransporte abortado.");
                return false;
            }

            // Validação 3: Verifica se a cena existe nas Build Settings
            if (!IsSceneInBuildSettings(destinationSceneName))
            {
                LogError($"Cena '{destinationSceneName}' não foi encontrada nas Build Settings. " +
                              $"Adicione a cena em File > Build Settings antes de usar teletransporte cross-scene.");
                return false;
            }

            // Validação 4: Verifica se a cena de destino não é a cena atual
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == destinationSceneName)
            {
                LogWarning($"Cena de destino '{destinationSceneName}' é a mesma que a cena atual. " +
                                $"Use teletransporte same-scene ao invés de cross-scene.");
                return false;
            }

            // Validação 5: Verifica se o TransitionEffect está atribuído
            if (transitionEffect == null)
            {
                LogError("TransitionEffect não está atribuído. Teletransporte abortado.");
                return false;
            }

            // Validação 6: Verifica se o PlayerController existe
            if (PlayerController.Instance == null)
            {
                LogError("PlayerController.Instance não encontrado. " +
                              "Certifique-se de que o Player existe na cena.");
                return false;
            }

            // Validação 7: Verifica se o Player tem Rigidbody2D
            Rigidbody2D playerRb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (playerRb == null)
            {
                LogError("Player não possui Rigidbody2D. " +
                              "Componente necessário para teletransporte.");
                return false;
            }

            // Validação 8: Verifica se a câmera principal existe
            if (Camera.main == null)
            {
                LogWarning("Camera.main não encontrada. " +
                                "Posicionamento da câmera pode não funcionar corretamente.");
                // Não aborta - câmera é opcional
            }

            // Log de sucesso se debug está habilitado
            if (enableDebugLogs)
            {
                Log($"Validação bem-sucedida para teletransporte para '{destinationSceneName}'.");
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
            if (clip == null || audioSource == null)
            {
                return;
            }

            // Reproduz som com volume configurado
            audioSource.PlayOneShot(clip, defaultVolume);
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
                LogError("Validação falhou. Teletransporte abortado.");
                return;
            }

            // Implementa teleport lock para prevenir teletransportes simultâneos
            if (isTeleporting)
            {
                LogWarning("Teletransporte já em progresso. Ignorando nova solicitação.");
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
                        Log("Velocidade do player zerada.");
                    }
                }

                if (enableDebugLogs)
                {
                    Log("Movimento do player desabilitado.");
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
                    Log("Fase 1 - Reproduzindo som de início...");
                }
                PlayTeleportSound(startSound);

                // Fase 2: Execute fade out usando TeleportTransitionHelper
                if (enableDebugLogs)
                {
                    Log("Fase 2 - Executando fade out...");
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
                            Log("Callback mid-transition - Tela escura, iniciando transferência...");
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
                    Log("Transição visual completa.");
                }

                // Fase 3: Play end sound após fade in
                if (enableDebugLogs)
                {
                    Log("Fase 3 - Reproduzindo som de chegada...");
                }
                PlayTeleportSound(endSound);

                if (enableDebugLogs)
                {
                    Debug.Log($"TeleportManager: Teletransporte para '{destinationSceneName}' concluído com sucesso!");
                }

                // Aguarda um frame para garantir que o player foi completamente inicializado na nova cena
                yield return null;
                
                // Reabilita movimento do player
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.EnableMovement();

                    if (enableDebugLogs)
                    {
                        Log("Movimento do player reabilitado.");
                    }
                }
                else
                {
                    LogError("PlayerController.Instance é null ao tentar reabilitar movimento!");
                }
            }
            finally
            {
                // Clear teleport lock no finally block para garantir que sempre será limpo
                // IMPORTANTE: Não pode ter yield statements no finally block
                isTeleporting = false;

                if (enableDebugLogs)
                {
                    Log("Teleport lock liberado.");
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
                    Log("DontDestroyOnLoad aplicado ao Player antes do carregamento.");
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

            // Aguarda múltiplos frames para garantir que a cena foi completamente inicializada
            // Isso é necessário porque alguns sistemas podem inicializar no Start() ou OnEnable()
            yield return null;
            yield return null;
            yield return new WaitForEndOfFrame();

            // SEMPRE loga antes do reposicionamento para debug
            Debug.Log($"[TeleportManager] ANTES de RepositionPlayerInNewScene - Posição destino: {destinationPosition}");
            if (PlayerController.Instance != null)
            {
                Debug.Log($"[TeleportManager] Posição atual do player: {PlayerController.Instance.transform.position}");
            }

            // Reposiciona o player na nova cena
            RepositionPlayerInNewScene(destinationPosition, true); // FORÇA enableDebugLogs = true

            // SEMPRE loga depois do reposicionamento para debug
            Debug.Log($"[TeleportManager] DEPOIS de RepositionPlayerInNewScene");
            if (PlayerController.Instance != null)
            {
                Debug.Log($"[TeleportManager] Posição final do player: {PlayerController.Instance.transform.position}");
            }

            if (enableDebugLogs)
            {
                Log("Player reposicionado na nova cena.");
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
                LogError("PlayerController.Instance não encontrado após carregamento da cena!");
                return;
            }

            GameObject playerObject = PlayerController.Instance.gameObject;

            if (enableDebugLogs)
            {
                Log($"Posição atual do player antes do reposicionamento: {playerObject.transform.position}");
                Log($"Posição de destino configurada: {destinationPosition}");
            }

            // Desabilita temporariamente a Cinemachine para evitar interferência
            Unity.Cinemachine.CinemachineCamera cinemachine = null;
            if (SlimeKing.Core.CameraManager.HasInstance)
            {
                cinemachine = SlimeKing.Core.CameraManager.Instance.GetCinemachineCamera();
                if (cinemachine != null)
                {
                    cinemachine.enabled = false;
                    if (enableDebugLogs)
                    {
                        Log("Cinemachine temporariamente desabilitada para reposicionamento.");
                    }
                }
            }

            // Reposiciona o player na posição de destino
            playerObject.transform.position = destinationPosition;

            if (enableDebugLogs)
            {
                Log($"Player reposicionado para {destinationPosition}");
                Log($"Posição real do player após reposicionamento: {playerObject.transform.position}");
            }

            // Força a câmera principal a seguir o player imediatamente
            if (Camera.main != null)
            {
                // Define a câmera na mesma posição do player (com offset Z para 2D)
                Vector3 cameraPosition = new Vector3(
                    destinationPosition.x,
                    destinationPosition.y,
                    Camera.main.transform.position.z // Mantém o Z da câmera
                );
                Camera.main.transform.position = cameraPosition;

                if (enableDebugLogs)
                {
                    Log($"Câmera reposicionada para {cameraPosition}");
                }
            }
            else if (enableDebugLogs)
            {
                LogWarning("Camera.main não encontrada. Posição da câmera não atualizada.");
            }

            // Reabilita e reconfigura a Cinemachine
            if (cinemachine != null)
            {
                cinemachine.enabled = true;
                
                if (SlimeKing.Core.CameraManager.HasInstance)
                {
                    SlimeKing.Core.CameraManager.Instance.ForceCinemachineSetup();
                    
                    if (enableDebugLogs)
                    {
                        Log("Cinemachine reabilitada e reconfigurada.");
                    }
                }
            }

            // Validação final
            if (enableDebugLogs)
            {
                Log($"Posição final do player: {playerObject.transform.position}");
                if (Camera.main != null)
                {
                    Log($"Posição final da câmera: {Camera.main.transform.position}");
                }
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
                Log("Liberando recursos não utilizados...");
            }

            yield return Resources.UnloadUnusedAssets();

            if (enableDebugLogs)
            {
                Log("Recursos liberados.");
            }

            // Opcional: Força coleta de lixo para limpeza imediata de memória
            // Comentado por padrão pois pode causar stuttering
            // Descomente se necessário para liberar memória imediatamente
            // System.GC.Collect();
            // if (enableDebugLogs)
            // {
            //     Log("Garbage collection executado.");
            // }
        }

        #endregion
    }
}

