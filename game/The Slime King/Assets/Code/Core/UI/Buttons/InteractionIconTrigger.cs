using UnityEngine;

namespace TheSlimeKing.Core.UI.Icons
{
    /// <summary>
    /// DEPRECATED: Use TopIconSwitcher instead.
    /// </summary>
    [System.Obsolete("Use TopIconSwitcher instead")]
    public class InteractionIconTrigger : MonoBehaviour
    {
        // Versão simplificada que apenas redireciona para o novo sistema
        private void Start()
        {
            Debug.LogWarning("InteractionIconTrigger is deprecated. Use TopIconSwitcher instead.");
        }

        private void OnValidate()
        {
            Debug.LogWarning("InteractionIconTrigger is deprecated. Use TopIconSwitcher instead.");
        }
    }
}
