using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    public class CutsceneTrigger : MonoBehaviour
    {
        public enum TriggerType
        {
            OnStart,
            OnTriggerEnter2D,
            OnInteract
        }

        [SerializeField] private CutsceneDefinition cutscene;
        [SerializeField] private TriggerType triggerType = TriggerType.OnTriggerEnter2D;
        [SerializeField] private bool playOnce = true;
        [SerializeField] private string requiredTag = "Player";
        [SerializeField] private bool destroyAfterPlay = false;

        private bool hasPlayed;

        private void Start()
        {
            if (triggerType == TriggerType.OnStart)
            {
                PlayCutscene();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerType == TriggerType.OnTriggerEnter2D &&
                other.CompareTag(requiredTag))
            {
                PlayCutscene();
            }
        }

        public void OnInteract()
        {
            if (triggerType == TriggerType.OnInteract)
            {
                PlayCutscene();
            }
        }

        private async void PlayCutscene()
        {
            if (cutscene == null || (playOnce && hasPlayed))
                return;

            await CutsceneManager.Instance.PlayCutscene(cutscene);

            hasPlayed = true;

            if (destroyAfterPlay)
            {
                Destroy(gameObject);
            }
        }
    }
}
