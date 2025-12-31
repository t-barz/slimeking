using UnityEngine;

namespace SlimeKing.Gameplay
{
    /// <summary>
    /// Script utilitário para autodestruição de GameObjects.
    /// Permite configurar delay opcional para destruição automática ou manual.
    /// </summary>
    public class SelfDestruct : MonoBehaviour
{
    [Header("⏱️ Configuração Opcional de Delay")]
    [Tooltip("Tempo em segundos para destruição automática (0 = apenas manual)")]
    [SerializeField] private float autoDestroyDelay = 0f;

    /// <summary>
    /// Inicialização - configura destruição automática se delay for especificado
    /// </summary>
    void Start()
    {
        // Se foi configurado um delay, agenda a destruição automática
        if (autoDestroyDelay > 0f)
        {
            Destroy(gameObject, autoDestroyDelay);
        }
    }

    /// <summary>
    /// Destroi o GameObject onde este script está anexado.
    /// Método público que pode ser chamado por outros scripts ou eventos.
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Destroi o objeto após um delay específico.
    /// </summary>
    /// <param name="delay">Tempo em segundos para aguardar antes da destruição</param>
    public void DestroyObject(float delay)
    {
        if (delay <= 0f)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, delay);
        }
    }
}
}
