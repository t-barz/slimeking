using System.Threading.Tasks;
using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    [CreateAssetMenu(fileName = "New Dialogue Event", menuName = "SlimeKing/Cutscenes/Events/Dialogue Event")]
    public class DialogueEvent : CutsceneEvent
    {
        [System.Serializable]
        public class DialogueLine
        {
            public string characterName;
            [TextArea(3, 10)]
            public string text;
            public float displayDuration = 3f;
            public AudioClip voiceClip;
        }

        [SerializeField] private DialogueLine[] dialogueLines;

        private bool skipped;

        public override async Task Execute(CutsceneContext context)
        {
            skipped = false;

            foreach (var line in dialogueLines)
            {
                if (skipped) break;

                // TODO: Integrar com seu sistema de diálogo existente
                Debug.Log($"{line.characterName}: {line.text}");

                // Espera a duração especificada
                await Task.Delay((int)(line.displayDuration * 1000));
            }
        }

        public override void Skip()
        {
            skipped = true;
        }
    }
}
