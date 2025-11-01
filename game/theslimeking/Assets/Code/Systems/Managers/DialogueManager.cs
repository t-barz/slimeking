using System.Collections.Generic;
using SlimeKing.Core;
using SlimeMec.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace SlimeMec.Systems
{
    /// <summary>
    /// Gerencia o estado global do sistema de diálogos.
    /// Controla o fluxo de páginas, carrega dados via LocalizationManager e integra com DialogueUI.
    /// 
    /// INTEGRAÇÃO COM PLAYERCONTROLLER:
    /// Este sistema possui suporte para pausar o jogador durante diálogos através da configuração
    /// 'pausePlayerDuringDialogue'. Atualmente, os métodos PausePlayer() e UnpausePlayer() são stubs
    /// que aguardam a implementação do PlayerController.
    /// 
    /// Quando o PlayerController for implementado, será necessário:
    /// 1. Atualizar os métodos PausePlayer() e UnpausePlayer() para chamar o PlayerController
    /// 2. Verificar se o PlayerController implementa métodos como SetMovementEnabled() ou similar
    /// 3. Considerar desabilitar input de ataque/interação além do movimento
    /// 4. Testar que o jogador é corretamente pausado/despausado durante diálogos
    /// 
    /// O sistema funciona completamente sem o PlayerController, mas o jogador poderá se mover
    /// durante os diálogos até que a integração seja implementada.
    /// </summary>
    public class DialogueManager : ManagerSingleton<DialogueManager>
    {
        #region Events

        /// <summary>
        /// Evento disparado quando um diálogo é iniciado.
        /// </summary>
        public UnityEvent<string> OnDialogueStarted = new UnityEvent<string>();

        /// <summary>
        /// Evento disparado quando uma página de diálogo é exibida.
        /// Parâmetros: texto da página, se há mais páginas
        /// </summary>
        public UnityEvent<string, bool> OnPageDisplayed = new UnityEvent<string, bool>();

        /// <summary>
        /// Evento disparado quando um diálogo é encerrado.
        /// </summary>
        public UnityEvent OnDialogueEnded = new UnityEvent();

        #endregion

        #region Inspector Settings

        [Header("Dialogue Settings")]
        [SerializeField] private bool pausePlayerDuringDialogue = true;

        [Tooltip("Referência ao componente DialogueUI (será definido em runtime ou via Inspector)")]
        [SerializeField] private MonoBehaviour dialogueUIComponent;

        #endregion

        #region Private Fields

        /// <summary>
        /// Dados do diálogo atualmente ativo
        /// </summary>
        private LocalizedDialogueData currentDialogueData;

        /// <summary>
        /// Lista de páginas do diálogo atual
        /// </summary>
        private List<DialoguePage> currentPages;

        /// <summary>
        /// Índice da página atual sendo exibida
        /// </summary>
        private int currentPageIndex;

        /// <summary>
        /// Flag indicando se há um diálogo ativo
        /// </summary>
        private bool isDialogueActive;

        /// <summary>
        /// ID do diálogo atual
        /// </summary>
        private string currentDialogueId;

        /// <summary>
        /// Interface para comunicação com DialogueUI (será implementada quando DialogueUI for criado)
        /// </summary>
        private IDialogueUI dialogueUI;

        #endregion

        #region Initialization

        protected override void Initialize()
        {
            Log("DialogueManager initialized");

            // Inicializa estado
            isDialogueActive = false;
            currentPageIndex = 0;
            currentPages = null;
            currentDialogueData = null;

            // Tenta obter referência ao DialogueUI se foi configurado no Inspector
            if (dialogueUIComponent != null)
            {
                dialogueUI = dialogueUIComponent as IDialogueUI;
                if (dialogueUI == null)
                {
                    LogWarning("DialogueUI component assigned but does not implement IDialogueUI interface");
                }
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Inicia um diálogo carregando os dados via LocalizationManager.
        /// </summary>
        /// <param name="dialogueId">ID único do diálogo a ser iniciado</param>
        public void StartDialogue(string dialogueId)
        {
            if (string.IsNullOrEmpty(dialogueId))
            {
                LogError("StartDialogue called with null or empty dialogueId");
                return;
            }

            // Verifica se já há um diálogo ativo
            if (isDialogueActive)
            {
                LogWarning($"Cannot start dialogue '{dialogueId}'. Another dialogue is already active: '{currentDialogueId}'");
                return;
            }

            // Verifica se LocalizationManager está disponível
            if (!LocalizationManager.HasInstance)
            {
                LogError("Cannot start dialogue. LocalizationManager is not available");
                return;
            }

            Log($"Starting dialogue: {dialogueId}");

            // Carrega dados do diálogo via LocalizationManager
            currentDialogueData = LocalizationManager.Instance.GetLocalizedDialogue(dialogueId);

            if (currentDialogueData == null)
            {
                LogError($"Failed to load dialogue data for '{dialogueId}'");
                return;
            }

            // Obtém as páginas no idioma atual
            string currentLanguage = LocalizationManager.Instance.GetCurrentLanguageCode();
            currentPages = currentDialogueData.GetPages(currentLanguage);

            if (currentPages == null || currentPages.Count == 0)
            {
                LogError($"Dialogue '{dialogueId}' has no pages for language '{currentLanguage}'");
                return;
            }

            // Inicializa estado do diálogo
            currentDialogueId = dialogueId;
            currentPageIndex = 0;
            isDialogueActive = true;

            // Pausa o jogador se configurado
            if (pausePlayerDuringDialogue)
            {
                PausePlayer();
            }

            // Dispara evento de início de diálogo
            OnDialogueStarted?.Invoke(dialogueId);

            // Exibe a primeira página
            DisplayCurrentPage();

            Log($"Dialogue '{dialogueId}' started successfully with {currentPages.Count} page(s)");
        }

        /// <summary>
        /// Avança para a próxima página do diálogo.
        /// Se for a última página, encerra o diálogo.
        /// </summary>
        public void NextPage()
        {
            if (!isDialogueActive)
            {
                LogWarning("NextPage called but no dialogue is active");
                return;
            }

            if (currentPages == null || currentPages.Count == 0)
            {
                LogError("NextPage called but currentPages is null or empty");
                EndDialogue();
                return;
            }

            // Verifica se há mais páginas
            if (currentPageIndex < currentPages.Count - 1)
            {
                // Avança para a próxima página
                currentPageIndex++;
                Log($"Advancing to page {currentPageIndex + 1} of {currentPages.Count}");
                DisplayCurrentPage();
            }
            else
            {
                // Última página - encerra o diálogo
                Log("Last page reached. Ending dialogue.");
                EndDialogue();
            }
        }

        /// <summary>
        /// Encerra o diálogo atual, limpando o estado e notificando componentes.
        /// </summary>
        public void EndDialogue()
        {
            if (!isDialogueActive)
            {
                LogWarning("EndDialogue called but no dialogue is active");
                return;
            }

            Log($"Ending dialogue: {currentDialogueId}");

            // Limpa estado
            isDialogueActive = false;
            string endedDialogueId = currentDialogueId;
            currentDialogueId = null;
            currentDialogueData = null;
            currentPages = null;
            currentPageIndex = 0;

            // Oculta a UI do diálogo
            if (dialogueUI != null)
            {
                dialogueUI.HideDialogue();
            }

            // Despausa o jogador se estava pausado
            if (pausePlayerDuringDialogue)
            {
                UnpausePlayer();
            }

            // Dispara evento de fim de diálogo
            OnDialogueEnded?.Invoke();

            Log($"Dialogue '{endedDialogueId}' ended successfully");
        }

        /// <summary>
        /// Verifica se há um diálogo ativo no momento.
        /// </summary>
        /// <returns>True se há um diálogo ativo</returns>
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }

        /// <summary>
        /// Obtém o índice da página atual (0-based).
        /// </summary>
        /// <returns>Índice da página atual ou -1 se nenhum diálogo está ativo</returns>
        public int GetCurrentPage()
        {
            return isDialogueActive ? currentPageIndex : -1;
        }

        /// <summary>
        /// Obtém o número total de páginas do diálogo atual.
        /// </summary>
        /// <returns>Total de páginas ou 0 se nenhum diálogo está ativo</returns>
        public int GetTotalPages()
        {
            return (isDialogueActive && currentPages != null) ? currentPages.Count : 0;
        }

        /// <summary>
        /// Obtém o ID do diálogo atualmente ativo.
        /// </summary>
        /// <returns>ID do diálogo ou null se nenhum diálogo está ativo</returns>
        public string GetCurrentDialogueId()
        {
            return currentDialogueId;
        }

        /// <summary>
        /// Define a referência ao componente DialogueUI.
        /// Útil para configuração em runtime.
        /// </summary>
        /// <param name="ui">Componente que implementa IDialogueUI</param>
        public void SetDialogueUI(IDialogueUI ui)
        {
            dialogueUI = ui;
            Log("DialogueUI reference set");
        }

        /// <summary>
        /// Define se o jogador deve ser pausado durante diálogos.
        /// </summary>
        /// <param name="shouldPause">True para pausar o jogador</param>
        public void SetPausePlayerDuringDialogue(bool shouldPause)
        {
            pausePlayerDuringDialogue = shouldPause;
            Log($"Pause player during dialogue set to: {shouldPause}");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Exibe a página atual do diálogo via DialogueUI.
        /// </summary>
        private void DisplayCurrentPage()
        {
            if (currentPages == null || currentPageIndex >= currentPages.Count)
            {
                LogError("Cannot display page. Invalid currentPages or currentPageIndex");
                return;
            }

            DialoguePage page = currentPages[currentPageIndex];
            bool hasMorePages = currentPageIndex < currentPages.Count - 1;

            Log($"Displaying page {currentPageIndex + 1}/{currentPages.Count}: \"{page.text.Substring(0, Mathf.Min(30, page.text.Length))}...\"");

            // Dispara evento de página exibida
            OnPageDisplayed?.Invoke(page.text, hasMorePages);

            // Exibe via DialogueUI se disponível
            if (dialogueUI != null)
            {
                dialogueUI.ShowDialogue(page.text, hasMorePages);
            }
            else
            {
                LogWarning("DialogueUI is not set. Page cannot be displayed visually.");
            }
        }

        /// <summary>
        /// Pausa o movimento do jogador durante o diálogo.
        /// 
        /// INTEGRAÇÃO PENDENTE: Este método é um stub que será implementado quando o PlayerController estiver disponível.
        /// 
        /// TODO: Implementar integração com PlayerController quando disponível.
        /// Passos necessários:
        /// 1. Verificar se PlayerController existe e está acessível (singleton ou referência)
        /// 2. Chamar método para desabilitar movimento (ex: SetMovementEnabled(false))
        /// 3. Opcionalmente, desabilitar input de ataque/interação
        /// 4. Considerar salvar estado anterior do jogador para restaurar depois
        /// 
        /// Exemplo de implementação:
        /// <code>
        /// if (PlayerController.HasInstance)
        /// {
        ///     PlayerController.Instance.SetMovementEnabled(false);
        ///     PlayerController.Instance.SetInputEnabled(false);
        /// }
        /// </code>
        /// </summary>
        private void PausePlayer()
        {
            Log("Pausing player (integration with PlayerController pending)");
            
            // TODO: Integrar com PlayerController existente
            // Exemplo: PlayerController.Instance?.SetMovementEnabled(false);
            
            // NOTA: Atualmente este é um stub. O sistema de diálogo funciona sem pausar o jogador.
            // Quando o PlayerController for implementado, adicione a lógica de pausa aqui.
        }

        /// <summary>
        /// Despausa o movimento do jogador após o diálogo.
        /// 
        /// INTEGRAÇÃO PENDENTE: Este método é um stub que será implementado quando o PlayerController estiver disponível.
        /// 
        /// TODO: Implementar integração com PlayerController quando disponível.
        /// Passos necessários:
        /// 1. Verificar se PlayerController existe e está acessível
        /// 2. Chamar método para reabilitar movimento (ex: SetMovementEnabled(true))
        /// 3. Restaurar input de ataque/interação
        /// 4. Restaurar estado anterior do jogador se foi salvo
        /// 
        /// Exemplo de implementação:
        /// <code>
        /// if (PlayerController.HasInstance)
        /// {
        ///     PlayerController.Instance.SetMovementEnabled(true);
        ///     PlayerController.Instance.SetInputEnabled(true);
        /// }
        /// </code>
        /// </summary>
        private void UnpausePlayer()
        {
            Log("Unpausing player (integration with PlayerController pending)");
            
            // TODO: Integrar com PlayerController existente
            // Exemplo: PlayerController.Instance?.SetMovementEnabled(true);
            
            // NOTA: Atualmente este é um stub. O sistema de diálogo funciona sem pausar o jogador.
            // Quando o PlayerController for implementado, adicione a lógica de despausa aqui.
        }

        #endregion

        #region Cleanup

        protected override void OnManagerDestroy()
        {
            // Limpa eventos
            OnDialogueStarted?.RemoveAllListeners();
            OnPageDisplayed?.RemoveAllListeners();
            OnDialogueEnded?.RemoveAllListeners();

            // Encerra diálogo se estiver ativo
            if (isDialogueActive)
            {
                EndDialogue();
            }
        }

        #endregion
    }

    /// <summary>
    /// Interface para comunicação entre DialogueManager e DialogueUI.
    /// Será implementada pelo componente DialogueUI em uma task futura.
    /// </summary>
    public interface IDialogueUI
    {
        /// <summary>
        /// Exibe o diálogo com o texto especificado.
        /// </summary>
        /// <param name="text">Texto a ser exibido</param>
        /// <param name="hasMorePages">Se há mais páginas após esta</param>
        void ShowDialogue(string text, bool hasMorePages);

        /// <summary>
        /// Oculta a caixa de diálogo.
        /// </summary>
        void HideDialogue();
    }
}
