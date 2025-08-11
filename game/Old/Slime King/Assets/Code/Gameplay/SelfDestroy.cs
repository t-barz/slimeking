using UnityEngine;

public class SelfDestroy : MonoBehaviour
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
    /// Destroi este objeto imediatamente
    /// </summary>
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
