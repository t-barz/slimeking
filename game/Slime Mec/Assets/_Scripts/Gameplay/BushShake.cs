using UnityEngine;

/// <summary>
/// Componente que detecta colisão com o jogador e ativa uma trigger de shake no Animator.
/// Usado para arbustos que tremem quando o jogador passa por eles.
/// 
/// REQUISITOS:
/// • GameObject deve ter um Collider2D configurado como Trigger
/// • GameObject deve ter um Animator com trigger "Shake"
/// • Jogador deve ter tag "Player"
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(Animator))]
public class BushShake : MonoBehaviour
{
    #region Private Variables
    private Animator _animator;
    private static readonly int ShakeTrigger = Animator.StringToHash("Shake");
    #endregion

    #region Unity Lifecycle
    /// <summary>
    /// Inicializa os componentes necessários e valida configurações.
    /// </summary>
    private void Awake()
    {
        // Obtém o componente Animator
        _animator = GetComponent<Animator>();

    }
    #endregion

    #region Collision Detection
    /// <summary>
    /// Detecta quando o jogador entra na área de trigger e ativa o shake.
    /// </summary>
    /// <param name="other">Collider que entrou no trigger</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que colidiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Ativa a trigger de shake no Animator
            if (_animator != null)
            {
                _animator.SetTrigger(ShakeTrigger);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log($"BushShake: Ativando shake em '{gameObject.name}' - jogador detectado!", this);
#endif
            }
        }
    }
    #endregion
}
