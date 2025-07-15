using UnityEngine;

/// <summary>
/// Componente que ativa a trigger "Shake" no Animator de objetos que colidem com este.
/// </summary>
public class ShakeHandler : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Nome exato da trigger no Animator do objeto")]
    [SerializeField] private string triggerName = "Shake";

    [Tooltip("Ativar apenas para objetos com tag específica (deixe vazio para todos)")]
    [SerializeField] private string targetTag = "Player";

    [Tooltip("Ativar logs de debug")]
    [SerializeField] private bool enableDebugLogs = false;

    /// <summary>
    /// Detecta quando um objeto entra na área de trigger
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se precisa filtrar por tag
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
            return;

        // Tenta obter o Animator do objeto que colidiu
        Animator animator = GetComponent<Animator>();


        // Se encontrou um Animator, ativa a trigger
        if (animator != null)
        {
            animator.SetTrigger(triggerName);

            if (enableDebugLogs)
            {
                Debug.Log($"Trigger '{triggerName}' ativada no objeto {other.name}");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning($"Objeto {other.name} não possui componente Animator");
        }
    }
}
