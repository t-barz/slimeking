using UnityEngine;

namespace TheSlimeKing.Core.UI.Icons
{
    /// <summary>
    /// DEPRECATED: Use TopIconSwitcher instead.
    /// Este script foi mantido apenas para compatibilidade com código existente.
    /// </summary>
    [System.Obsolete("Use TopIconSwitcher instead")]
    public class IconManager : MonoBehaviour
    {
        public static IconManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
                
            if (Instance != this)
                Destroy(gameObject);
                
            Debug.LogWarning("IconManager is deprecated. Use TopIconSwitcher instead.");
        }
        
        // Métodos dummy para manter compatibilidade com código que possa estar usando
        public void ShowIcon(string actionName, Vector3 worldPosition) 
        {
            Debug.LogWarning($"ShowIcon called on deprecated IconManager. Use TopIconSwitcher instead.");
        }
        
        public void HideIcon() 
        {
            Debug.LogWarning($"HideIcon called on deprecated IconManager. Use TopIconSwitcher instead.");
        }
        
        public string GetCurrentDevice()
        {
            return "Unknown";
        }
        
        public string GetActionNameForInputAction(UnityEngine.InputSystem.InputAction action)
        {
            return action.name;
        }
    }
}
