using System.Threading.Tasks;
using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    [CreateAssetMenu(fileName = "New Animation Event", menuName = "SlimeKing/Cutscenes/Events/Animation Event")]
    public class AnimationEvent : CutsceneEvent
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private string animationTriggerName;
        [SerializeField] private bool waitForAnimationComplete = true;

        private bool skipped;
        private Animator animator;

        public override async Task Execute(CutsceneContext context)
        {
            skipped = false;

            // Encontra o objeto alvo
            GameObject target = null;
            if (targetTag == "Player" && context.Player != null)
            {
                target = context.Player;
            }
            else
            {
                target = GameObject.FindGameObjectWithTag(targetTag);
            }

            if (target == null)
            {
                Debug.LogError($"AnimationEvent: Não foi possível encontrar objeto com tag {targetTag}");
                return;
            }

            // Pega o componente Animator
            animator = target.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"AnimationEvent: Objeto {target.name} não tem componente Animator");
                return;
            }

            // Dispara a animação
            animator.SetTrigger(animationTriggerName);

            if (waitForAnimationComplete)
            {
                // Espera a animação completar
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float animationLength = stateInfo.length;

                await Task.Delay((int)(animationLength * 1000));
            }
        }

        public override void Skip()
        {
            skipped = true;
            if (animator != null)
            {
                // Tenta ir direto para o fim da animação
                animator.Play(animationTriggerName, 0, 1f);
            }
        }
    }
}
