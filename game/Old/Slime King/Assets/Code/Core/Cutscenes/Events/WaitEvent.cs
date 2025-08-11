using System.Threading.Tasks;
using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    [CreateAssetMenu(fileName = "New Wait Event", menuName = "SlimeKing/Cutscenes/Events/Wait Event")]
    public class WaitEvent : CutsceneEvent
    {
        private bool skipped;

        public override async Task Execute(CutsceneContext context)
        {
            skipped = false;
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
