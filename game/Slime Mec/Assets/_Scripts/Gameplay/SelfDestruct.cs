using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Destroi o GameObject onde este script está anexado.
    /// Método público que pode ser chamado por outros scripts ou eventos.
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
