using UnityEngine;
using Unity.Cinemachine;

public class CameraSetup : MonoBehaviour
{
    [Header("Configurações do Alvo")]
    public string playerTag = "Player";
    public bool findOnStart = true;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private void Start()
    {
        if (findOnStart)
        {
            SetPlayerAsTarget();
        }
    }

    public void SetPlayerAsTarget()
    {


        // Encontra o objeto com tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
        {
            // Define o player como alvo da câmera
            GetComponent<CinemachineCamera>().Follow = player.transform;

            if (showDebugMessages)
                Debug.Log("Player definido como alvo da CinemachineCamera: " + player.name);
        }
        else
        {
            if (showDebugMessages)
                Debug.LogWarning("Objeto com tag '" + playerTag + "' não encontrado na cena!");
        }
    }

    // Método para redefinir o alvo manualmente
    public void SetCustomTarget(Transform newTarget)
    {
        GetComponent<CinemachineCamera>().Follow = newTarget;

        if (showDebugMessages)
            Debug.Log("Novo alvo definido para a CinemachineCamera: " + newTarget.name);
    }

    // Método para limpar o alvo
    public void ClearTarget()
    {
        GetComponent<CinemachineCamera>().Follow = null;

        if (showDebugMessages)
            Debug.Log("Alvo da CinemachineCamera removido");
    }
}
