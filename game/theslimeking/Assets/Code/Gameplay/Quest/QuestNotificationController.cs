using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

namespace TheSlimeKing.Quest
{
    /// <summary>
    /// Controlador de notificações visuais do Quest System.
    /// Exibe feedback ao jogador sobre progresso e conclusão de quests.
    /// </summary>
    public class QuestNotificationController : MonoBehaviour
    {
        #region Inspector Variables
        [Header("UI References")]
        [SerializeField] private GameObject notificationPanel;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private float displayDuration = 3f;
        
        [Header("Audio - Multiple Sounds")]
        [Tooltip("Lista de sons para evitar repetição ao completar objetivos")]
        [SerializeField] private List<AudioClip> objectiveCompleteSounds = new List<AudioClip>();
        [Tooltip("Lista de sons para evitar repetição ao completar quests")]
        [SerializeField] private List<AudioClip> questCompleteSounds = new List<AudioClip>();
        
        [Header("Audio Settings")]
        [Tooltip("AudioSource para reproduzir sons de notificação")]
        [SerializeField] private AudioSource audioSource;
        [Tooltip("Volume dos sons de notificação (0-1)")]
        [SerializeField] [Range(0f, 1f)] private float soundVolume = 1f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        #endregion
        
        #region Private Variables
        private Coroutine currentNotificationCoroutine;
        #endregion
        
        #region Unity Lifecycle
        private void Start()
        {
            // Garante que painel começa desativado
            if (notificationPanel != null)
            {
                notificationPanel.SetActive(false);
            }
            
            // Cria AudioSource se não configurado
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2D sound
                
                if (enableDebugLogs)
                    Debug.Log("[QuestNotificationController] AudioSource criado automaticamente");
            }
        }
        
        private void OnEnable()
        {
            SubscribeToEvents();
        }
        
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        #endregion
        
        #region Event Subscription
        private void SubscribeToEvents()
        {
            QuestEvents.OnQuestReadyToTurnIn += OnQuestReadyToTurnIn;
            QuestEvents.OnQuestCompleted += OnQuestCompleted;
        }
        
        private void UnsubscribeFromEvents()
        {
            QuestEvents.OnQuestReadyToTurnIn -= OnQuestReadyToTurnIn;
            QuestEvents.OnQuestCompleted -= OnQuestCompleted;
        }
        #endregion
        
        #region Event Handlers
        /// <summary>
        /// Callback quando quest está pronta para entregar.
        /// </summary>
        /// <param name="questID">ID da quest pronta</param>
        private void OnQuestReadyToTurnIn(string questID)
        {
            if (QuestManager.Instance == null)
                return;
            
            QuestProgress progress = QuestManager.Instance.GetQuestProgress(questID);
            if (progress != null)
            {
                ShowQuestReadyToTurnIn(progress.questData.questName);
            }
        }
        
        /// <summary>
        /// Callback quando quest é completada.
        /// </summary>
        /// <param name="quest">Dados da quest completada</param>
        /// <param name="rewards">Lista de recompensas recebidas</param>
        private void OnQuestCompleted(CollectQuestData quest, List<ItemReward> rewards)
        {
            ShowQuestCompleted(quest.questName, rewards);
        }
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Exibe notificação de objetivo completado.
        /// </summary>
        /// <param name="questName">Nome da quest</param>
        public void ShowObjectiveComplete(string questName)
        {
            string message = $"Objetivo Completado: {questName}";
            ShowNotification(message, objectiveCompleteSounds);
        }
        
        /// <summary>
        /// Exibe notificação de quest pronta para entregar.
        /// </summary>
        /// <param name="questName">Nome da quest</param>
        public void ShowQuestReadyToTurnIn(string questName)
        {
            string message = $"Quest Pronta: {questName}";
            ShowNotification(message, objectiveCompleteSounds);
        }
        
        /// <summary>
        /// Exibe notificação de quest completada com recompensas.
        /// </summary>
        /// <param name="questName">Nome da quest</param>
        /// <param name="rewards">Lista de recompensas recebidas</param>
        public void ShowQuestCompleted(string questName, List<ItemReward> rewards)
        {
            string rewardText = GetRewardText(rewards);
            string message = $"Quest Completada: {questName}\n{rewardText}";
            ShowNotification(message, questCompleteSounds);
        }
        #endregion
        
        #region Private Methods
        /// <summary>
        /// Exibe uma notificação com mensagem e som.
        /// </summary>
        /// <param name="message">Mensagem a exibir</param>
        /// <param name="sounds">Lista de sons para escolher aleatoriamente</param>
        private void ShowNotification(string message, List<AudioClip> sounds)
        {
            // Para coroutine anterior se existir
            if (currentNotificationCoroutine != null)
            {
                StopCoroutine(currentNotificationCoroutine);
            }
            
            currentNotificationCoroutine = StartCoroutine(ShowNotificationCoroutine(message, sounds));
        }
        
        /// <summary>
        /// Coroutine que exibe notificação por tempo determinado.
        /// </summary>
        /// <param name="message">Mensagem a exibir</param>
        /// <param name="sounds">Lista de sons para escolher aleatoriamente</param>
        private IEnumerator ShowNotificationCoroutine(string message, List<AudioClip> sounds)
        {
            // Valida referências
            if (notificationPanel == null || notificationText == null)
            {
                Debug.LogError("[QuestNotificationController] Referências de UI não configuradas!");
                yield break;
            }
            
            // Exibe notificação
            notificationPanel.SetActive(true);
            notificationText.text = message;
            
            // Reproduz som aleatório
            AudioClip sound = GetRandomSound(sounds);
            if (sound != null)
            {
                PlaySound(sound);
            }
            
            if (enableDebugLogs)
                Debug.Log($"[QuestNotificationController] {message}");
            
            // Aguarda duração
            yield return new WaitForSeconds(displayDuration);
            
            // Esconde notificação
            notificationPanel.SetActive(false);
            currentNotificationCoroutine = null;
        }
        
        /// <summary>
        /// Seleciona um som aleatório de uma lista para evitar repetição.
        /// </summary>
        /// <param name="sounds">Lista de sons disponíveis</param>
        /// <returns>AudioClip aleatório ou null se lista vazia</returns>
        private AudioClip GetRandomSound(List<AudioClip> sounds)
        {
            if (sounds == null || sounds.Count == 0)
                return null;
            
            int randomIndex = Random.Range(0, sounds.Count);
            return sounds[randomIndex];
        }
        
        /// <summary>
        /// Reproduz um som de notificação.
        /// Integra com AudioManager se disponível, caso contrário usa AudioSource local.
        /// </summary>
        /// <param name="sound">AudioClip a reproduzir</param>
        private void PlaySound(AudioClip sound)
        {
            if (sound == null)
                return;
            
            // Tenta usar AudioManager se existir no futuro
            // Padrão: buscar por tipo para evitar dependência direta
            var audioManager = FindObjectOfType<MonoBehaviour>()?.GetComponent(System.Type.GetType("TheSlimeKing.Audio.AudioManager"));
            
            if (audioManager != null)
            {
                // Usa reflexão para chamar PlaySFX se AudioManager existir
                var playSFXMethod = audioManager.GetType().GetMethod("PlaySFX", new[] { typeof(AudioClip) });
                if (playSFXMethod != null)
                {
                    playSFXMethod.Invoke(audioManager, new object[] { sound });
                    
                    if (enableDebugLogs)
                        Debug.Log($"[QuestNotificationController] Som reproduzido via AudioManager: {sound.name}");
                    
                    return;
                }
            }
            
            // Fallback: usa AudioSource local
            if (audioSource != null)
            {
                audioSource.PlayOneShot(sound, soundVolume);
                
                if (enableDebugLogs)
                    Debug.Log($"[QuestNotificationController] Som reproduzido via AudioSource: {sound.name}");
            }
            else
            {
                Debug.LogWarning("[QuestNotificationController] AudioSource não disponível para reproduzir som!");
            }
        }
        
        /// <summary>
        /// Formata lista de recompensas em texto legível.
        /// </summary>
        /// <param name="rewards">Lista de recompensas</param>
        /// <returns>Texto formatado com recompensas</returns>
        private string GetRewardText(List<ItemReward> rewards)
        {
            if (rewards == null || rewards.Count == 0)
                return "Sem recompensas";
            
            string text = "Recompensas: ";
            for (int i = 0; i < rewards.Count; i++)
            {
                if (rewards[i].item != null)
                {
                    text += $"{rewards[i].item.itemName} x{rewards[i].quantity}";
                    if (i < rewards.Count - 1)
                        text += ", ";
                }
            }
            return text;
        }
        #endregion
    }
}
