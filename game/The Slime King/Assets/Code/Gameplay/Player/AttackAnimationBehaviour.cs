using TheSlimeKing.Gameplay;
using UnityEngine;

public class AttackAnimationBehaviour : StateMachineBehaviour
{
    // OnStateExit é chamado quando uma transição termina e o estado da máquina começa a executar outro estado
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Encontra o SlimeMovement e notifica que a animação terminou
        animator.GetComponent<TheSlimeKing.Gameplay.SlimeMovement>().isAttacking = false;
    }


}
