using UnityEngine;

/// <summary>
/// Componente simples para auto-destruição de objetos.
/// Pode ser configurado para destruição automática após um tempo ou
/// usado por outros scripts chamando o método DestroyObject.
/// </summary>
public class SelfDestroy : MonoBehaviour
{
    [Tooltip("Se verdadeiro, o objeto será destruído automaticamente após o tempo definido")]
    [SerializeField] private bool autoDestroy = false;
    
    [Tooltip("Tempo em segundos até a destruição automática (se autoDestroy for verdadeiro)")]
    [SerializeField] private float lifetime = 1.0f;

    private void Start()
    {
        if (autoDestroy)
        {
            DestroyObject(lifetime);
        }
    }

    /// <summary>
    /// Destrói imediatamente o GameObject onde este script está anexado.
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Destrói o GameObject após o tempo especificado.
    /// </summary>
    /// <param name="delay">Tempo em segundos até a destruição</param>
    public void DestroyObject(float delay)
    {
        Destroy(gameObject, delay);
    }
}
