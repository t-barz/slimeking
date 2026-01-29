using UnityEngine;

public class BeeWorkerBehaviorController : MonoBehaviour
{
    #region Fields
    
    [Header("Stats")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float defense = 5f;
    [SerializeField] private float moveSpeed = 3f;
    
    #endregion
    
    #region Unity Lifecycle
    
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    
    private void Start()
    {
        
    }
    
    private void Update()
    {
        
    }
    
    #endregion
}
