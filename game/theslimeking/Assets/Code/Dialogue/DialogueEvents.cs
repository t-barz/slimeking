using System;

namespace TheSlimeKing.Dialogue
{
    /// <summary>
    /// Eventos globais do sistema de diálogo.
    /// Permite comunicação desacoplada entre componentes do sistema.
    /// </summary>
    public static class DialogueEvents
    {
        /// <summary>
        /// Disparado quando um diálogo é iniciado.
        /// </summary>
        public static event Action OnDialogueStart;
        
        /// <summary>
        /// Disparado quando um diálogo é finalizado.
        /// </summary>
        public static event Action OnDialogueEnd;
        
        /// <summary>
        /// Invoca o evento de início de diálogo.
        /// </summary>
        internal static void InvokeDialogueStart()
        {
            OnDialogueStart?.Invoke();
        }
        
        /// <summary>
        /// Invoca o evento de fim de diálogo.
        /// </summary>
        internal static void InvokeDialogueEnd()
        {
            OnDialogueEnd?.Invoke();
        }
    }
}
