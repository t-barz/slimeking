using System.Threading.Tasks;
using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    [CreateAssetMenu(fileName = "New Spawn Event", menuName = "SlimeKing/Cutscenes/Events/Spawn Event")]
    public class SpawnEvent : CutsceneEvent
    {
        public enum SpawnAction
        {
            Spawn,
            Despawn
        }

        [SerializeField] private GameObject prefabToSpawn;
        [SerializeField] private Vector2 spawnPosition;
        [SerializeField] private bool useWorldSpace = true;
        [SerializeField] private SpawnAction action = SpawnAction.Spawn;
        [SerializeField] private string targetTag; // Usado para Despawn
        [SerializeField] private bool fadeIn = true;
        [SerializeField] private float fadeDuration = 0.5f;

        private bool skipped;
        private GameObject spawnedObject;

        public override async Task Execute(CutsceneContext context)
        {
            skipped = false;

            switch (action)
            {
                case SpawnAction.Spawn:
                    await HandleSpawn(context);
                    break;
                case SpawnAction.Despawn:
                    await HandleDespawn();
                    break;
            }
        }

        private async Task HandleSpawn(CutsceneContext context)
        {
            if (prefabToSpawn == null)
            {
                Debug.LogError("SpawnEvent: Prefab não definido!");
                return;
            }

            Vector3 position = useWorldSpace ?
                new Vector3(spawnPosition.x, spawnPosition.y, 0) :
                context.CutsceneRoot.TransformPoint(new Vector3(spawnPosition.x, spawnPosition.y, 0));

            spawnedObject = GameObject.Instantiate(prefabToSpawn, position, Quaternion.identity, context.CutsceneRoot);

            if (fadeIn)
            {
                // Configura objeto inicialmente transparente
                var renderers = spawnedObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in renderers)
                {
                    Color startColor = renderer.color;
                    startColor.a = 0f;
                    renderer.color = startColor;
                }

                // Faz fade in
                float elapsedTime = 0f;
                while (elapsedTime < fadeDuration && !skipped)
                {
                    float alpha = elapsedTime / fadeDuration;
                    foreach (var renderer in renderers)
                    {
                        Color color = renderer.color;
                        color.a = alpha;
                        renderer.color = color;
                    }

                    await Task.Yield();
                    elapsedTime += Time.deltaTime;
                }

                // Garante alpha final
                if (!skipped)
                {
                    foreach (var renderer in renderers)
                    {
                        Color color = renderer.color;
                        color.a = 1f;
                        renderer.color = color;
                    }
                }
            }
        }

        private async Task HandleDespawn()
        {
            GameObject target = null;

            if (!string.IsNullOrEmpty(targetTag))
            {
                target = GameObject.FindGameObjectWithTag(targetTag);
            }
            else if (spawnedObject != null)
            {
                target = spawnedObject;
            }

            if (target == null)
            {
                Debug.LogError($"SpawnEvent: Não foi possível encontrar objeto para despawn com tag {targetTag}");
                return;
            }

            if (fadeIn)
            {
                // Faz fade out
                var renderers = target.GetComponentsInChildren<SpriteRenderer>();
                float elapsedTime = 0f;

                while (elapsedTime < fadeDuration && !skipped)
                {
                    float alpha = 1f - (elapsedTime / fadeDuration);
                    foreach (var renderer in renderers)
                    {
                        Color color = renderer.color;
                        color.a = alpha;
                        renderer.color = color;
                    }

                    await Task.Yield();
                    elapsedTime += Time.deltaTime;
                }
            }

            GameObject.Destroy(target);
        }

        public override void Skip()
        {
            skipped = true;

            if (action == SpawnAction.Spawn && spawnedObject != null)
            {
                // Garante que objeto fique totalmente visível
                var renderers = spawnedObject.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in renderers)
                {
                    Color color = renderer.color;
                    color.a = 1f;
                    renderer.color = color;
                }
            }
            else if (action == SpawnAction.Despawn)
            {
                // Destroi imediatamente se estiver em despawn
                GameObject target = GameObject.FindGameObjectWithTag(targetTag);
                if (target != null)
                {
                    GameObject.Destroy(target);
                }
            }
        }
    }
}
