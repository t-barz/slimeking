using UnityEngine;

public class NotAttackAnimationBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Encontra o SlimeMovement e notifica que a animação não é de ataque
        animator.GetComponent<TheSlimeKing.Gameplay.SlimeMovement>().isAttacking = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Encontra o SlimeMovement e notifica que a animação não é de ataque
        animator.GetComponent<TheSlimeKing.Gameplay.SlimeMovement>().isAttacking = false;
    }
}