using System.Threading.Tasks;
using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    [CreateAssetMenu(fileName = "New Animation Set Bool Event", menuName = "SlimeKing/Cutscenes/Events/Animation Set Bool Event")]
    public class AnimationSetBoolEvent : CutsceneEvent
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private string parameterName = "isSleeping";
        [SerializeField] private bool value = false;
        [SerializeField] private float initialDelay = 0.25f; // Delay antes de mudar o parâmetro

        private bool skipped;

        public override async Task Execute(CutsceneContext context)
        {
            skipped = false;

            // Aguarda o delay inicial
            float elapsedTime = 0f;
            while (elapsedTime < initialDelay && !skipped)
            {
                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }

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
                Debug.LogError($"AnimationSetBoolEvent: Não foi possível encontrar objeto com tag {targetTag}");
                return;
            }

            // Define o parâmetro no Animator
            var animator = target.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError($"AnimationSetBoolEvent: Não foi possível encontrar Animator em {targetTag}");
                return;
            }

            animator.SetBool(parameterName, value);

            // Espera a duração adicional especificada após mudar o parâmetro
            await WaitForDuration();
        }

        private async Task WaitForDuration()
        {
            float elapsedTime = 0f;
            while (elapsedTime < Duration && !skipped)
            {
                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }
        }

        public override void Skip()
        {
            skipped = true;
        }
    }
}
