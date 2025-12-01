using UnityEngine;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Dialogue
{
    /// <summary>
    /// Controla o estado do jogador durante diálogos.
    /// Pausa o movimento e outras ações enquanto o diálogo está ativo.
    /// Escuta eventos de diálogo para pausar/retomar automaticamente.
    /// </summary>
    public class DialoguePlayerController : MonoBehaviour
    {
        #region Private Fields
        
        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction attackAction;
        private bool wasMoveEnabled;
        private bool wasAttackEnabled;
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Obter PlayerInput (componente do novo Input System)
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                UnityEngine.Debug.LogError("[DialoguePlayerController] PlayerInput não encontrado no mesmo GameObject!");
                return;
            }
            
            // Obter ações específicas que queremos desabilitar
            moveAction = playerInput.actions["Move"];
            attackAction = playerInput.actions["Attack"];
            
            if (moveAction == null)
            {
                UnityEngine.Debug.LogWarning("[DialoguePlayerController] Ação 'Move' não encontrada!");
            }
            
            if (attackAction == null)
            {
                UnityEngine.Debug.LogWarning("[DialoguePlayerController] Ação 'Attack' não encontrada!");
            }
        }
        
        private void OnEnable()
        {
            // Inscrever nos eventos de diálogo
            DialogueEvents.OnDialogueStart += PausePlayerControl;
            DialogueEvents.OnDialogueEnd += ResumePlayerControl;
        }
        
        private void OnDisable()
        {
            // Desinscrever dos eventos de diálogo
            DialogueEvents.OnDialogueStart -= PausePlayerControl;
            DialogueEvents.OnDialogueEnd -= ResumePlayerControl;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Pausa o controle do jogador desabilitando apenas ações de movimento e ataque.
        /// Mantém o botão Interact ativo para avançar o diálogo.
        /// </summary>
        private void PausePlayerControl()
        {
            if (moveAction != null)
            {
                wasMoveEnabled = moveAction.enabled;
                moveAction.Disable();
            }
            
            if (attackAction != null)
            {
                wasAttackEnabled = attackAction.enabled;
                attackAction.Disable();
            }
            
            UnityEngine.Debug.Log("[DialoguePlayerController] Movimento e ataque pausados.");
        }
        
        /// <summary>
        /// Retoma o controle do jogador reabilitando as ações.
        /// </summary>
        private void ResumePlayerControl()
        {
            if (moveAction != null && wasMoveEnabled)
            {
                moveAction.Enable();
            }
            
            if (attackAction != null && wasAttackEnabled)
            {
                attackAction.Enable();
            }
            
            UnityEngine.Debug.Log("[DialoguePlayerController] Movimento e ataque retomados.");
        }
        
        #endregion
    }
}
