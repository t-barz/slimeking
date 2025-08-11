using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se encostou em um objeto "Destructible"
        if (other.CompareTag("Destructible") || other.CompareTag("Enemy"))
        {
            Debug.Log($"{other.tag} atingido pelo ataque!");

            // Busca o DamageHandler no objeto ou nos seus pais
            DamageHandler damageHandler = other.GetComponent<DamageHandler>();
            if (damageHandler == null)
            {
                damageHandler = other.GetComponentInParent<DamageHandler>();
            }

            // Se encontrou um DamageHandler, chama o ProcessHit
            if (damageHandler != null)
            {
                if (damageHandler.ProcessHit() > 0)
                {
                    Debug.Log($"{other.name} recebeu dano!");
                }
                else
                {
                    Debug.Log($"{other.name} não pôde receber dano.");
                }
            }
            else
            {
                Debug.LogWarning($"Objeto {other.name} não possui DamageHandler!");
            }
        }
    }
}
