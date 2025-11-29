using UnityEngine;

namespace TheSlimeKing.Dialogue
{
    /// <summary>
    /// Componente que gerencia o controle do jogador durante diálogos.
    /// Pausa/limita o movimento quando o diálogo está ativo.
    /// </summary>
    public class DialoguePlayerController : MonoBehaviour
    {
        private static DialoguePlayerController instance;
        private bool isDialogueActive;
        
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static DialoguePlayerController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<DialoguePlayerController>();
                    
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("DialoguePlayerController");
                        instance = obj.AddComponent<DialoguePlayerController>();
                    }
                }
                return instance;
            }
        }
        
        /// <summary>
        /// Verifica se o diálogo está ativo.
        /// </summary>
        public bool IsDialogueActive => isDialogueActive;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Pausa o controle do jogador.
        /// </summary>
        public void PausePlayerControl()
        {
            isDialogueActive = true;
            
            // Buscar PlayerController e desabilitar
            var playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }
        }
        
        /// <summary>
        /// Restaura o controle do jogador.
        /// </summary>
        public void RestorePlayerControl()
        {
            isDialogueActive = false;
            
            // Buscar PlayerController e habilitar
            var playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }
        }
    }
}
