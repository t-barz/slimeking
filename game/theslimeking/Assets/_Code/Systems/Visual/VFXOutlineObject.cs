using UnityEngine;

namespace SlimeKing.Visual
{
    public class VFXOutlineObject : MonoBehaviour
{
    private Material material;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
        // Ensure outline starts disabled
        material.SetFloat("_ShowOutline", 0f);
    }

    public void ShowOutline(bool show)
    {
        material.SetFloat("_ShowOutline", show ? 1f : 0f);
    }
}
}