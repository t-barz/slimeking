using System.Threading.Tasks;
using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    [CreateAssetMenu(fileName = "New Movement Event", menuName = "SlimeKing/Cutscenes/Events/Movement Event")]
    public class MovementEvent : CutsceneEvent
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private Vector2 targetPosition;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private bool useWorldSpace = true;
        [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private bool skipped;
        private Transform targetTransform;

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
                Debug.LogError($"MovementEvent: Não foi possível encontrar objeto com tag {targetTag}");
                return;
            }

            targetTransform = target.transform;
            Vector3 startPosition = targetTransform.position;
            Vector3 endPosition = useWorldSpace ?
                new Vector3(targetPosition.x, targetPosition.y, startPosition.z) :
                startPosition + new Vector3(targetPosition.x, targetPosition.y, 0);

            float distance = Vector2.Distance(new Vector2(startPosition.x, startPosition.y),
                                           new Vector2(endPosition.x, endPosition.y));
            float totalTime = distance / moveSpeed;
            float elapsedTime = 0f;

            while (elapsedTime < totalTime && !skipped)
            {
                float t = elapsedTime / totalTime;
                float curveValue = movementCurve.Evaluate(t);

                targetTransform.position = Vector3.Lerp(startPosition, endPosition, curveValue);

                await Task.Yield(); // Espera o próximo frame
                elapsedTime += Time.deltaTime;
            }

            // Garante que chegue na posição final
            if (!skipped)
            {
                targetTransform.position = endPosition;
            }
        }

        public override void Skip()
        {
            skipped = true;
            if (targetTransform != null)
            {
                // Move direto para a posição final
                Vector3 currentPos = targetTransform.position;
                Vector3 endPosition = useWorldSpace ?
                    new Vector3(targetPosition.x, targetPosition.y, currentPos.z) :
                    currentPos + new Vector3(targetPosition.x, targetPosition.y, 0);

                targetTransform.position = endPosition;
            }
        }
    }
}
