using System.Threading.Tasks;
using UnityEngine;
#if CINEMACHINE_PRESENT
using Cinemachine;
#endif

namespace SlimeKing.Core.Cutscenes
{
    [CreateAssetMenu(fileName = "New Camera Event", menuName = "SlimeKing/Cutscenes/Events/Camera Event")]
    public class CameraEvent : CutsceneEvent
    {
        public enum CameraAction
        {
            FocusTarget,
            ReturnToPlayer,
            Shake,
            Zoom
        }

        [SerializeField] private CameraAction action = CameraAction.FocusTarget;

        [Header("Focus Settings")]
        [SerializeField] private string targetTag;
        [SerializeField] private Vector2 offset;

        [Header("Zoom Settings")]
        [SerializeField] private float targetOrthographicSize = 5f;
        [SerializeField] private float originalOrthographicSize = 8f;

        [Header("Shake Settings")]
        [SerializeField] private float shakeIntensity = 1f;
        [SerializeField] private float shakeFrequency = 1f;

        private bool skipped;
#if CINEMACHINE_PRESENT
        private CinemachineVirtualCamera virtualCamera;
#else
        private Camera virtualCamera;
#endif

        public override async Task Execute(CutsceneContext context)
        {
            skipped = false;

#if CINEMACHINE_PRESENT
            // Try to find Cinemachine camera first
            virtualCamera = Object.FindAnyObjectByType<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("CameraEvent: Não foi possível encontrar CinemachineVirtualCamera!");
                return;
            }
#else
            // Fallback to regular camera if Cinemachine is not present
            virtualCamera = Camera.main;
            if (virtualCamera == null)
            {
                Debug.LogError("CameraEvent: Não foi possível encontrar a câmera principal!");
                return;
            }
#endif

            switch (action)
            {
                case CameraAction.FocusTarget:
                    await HandleFocus();
                    break;

                case CameraAction.ReturnToPlayer:
                    await HandleReturnToPlayer(context);
                    break;

                case CameraAction.Shake:
                    await HandleShake();
                    break;

                case CameraAction.Zoom:
                    await HandleZoom();
                    break;
            }
        }

        private async Task HandleFocus()
        {
            GameObject target = GameObject.FindGameObjectWithTag(targetTag);
            if (target == null)
            {
                Debug.LogError($"CameraEvent: Não foi possível encontrar alvo com tag {targetTag}");
                return;
            }

            GameObject followTarget = new GameObject($"CameraFollow_{targetTag}");
            followTarget.transform.position = target.transform.position + (Vector3)offset;

#if CINEMACHINE_PRESENT
            virtualCamera.Follow = followTarget.transform;
#else
            // Fallback behavior for regular camera
            var targetPos = followTarget.transform.position;
            targetPos.z = virtualCamera.transform.position.z;
            virtualCamera.transform.position = targetPos;
#endif

            await WaitForDuration();

            if (!skipped)
            {
                Object.Destroy(followTarget);
            }
        }

        private async Task HandleReturnToPlayer(CutsceneContext context)
        {
            if (context.Player == null)
            {
                Debug.LogError("CameraEvent: Player não encontrado no contexto!");
                return;
            }

#if CINEMACHINE_PRESENT
            virtualCamera.Follow = context.Player.transform;
#else
            // Fallback behavior for regular camera
            var targetPos = context.Player.transform.position;
            targetPos.z = virtualCamera.transform.position.z;
            virtualCamera.transform.position = targetPos;
#endif

            await WaitForDuration();
        }

        private async Task HandleShake()
        {
#if CINEMACHINE_PRESENT
            var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (noise == null)
            {
                Debug.LogError("CameraEvent: Componente de noise não encontrado na câmera virtual!");
                return;
            }

            noise.m_AmplitudeGain = shakeIntensity;
            noise.m_FrequencyGain = shakeFrequency;

            await WaitForDuration();

            if (!skipped)
            {
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
#else
            // Simple shake implementation for regular camera
            Vector3 originalPos = virtualCamera.transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < Duration && !skipped)
            {
                float shake = Mathf.Sin(elapsedTime * shakeFrequency * Mathf.PI) * shakeIntensity;
                virtualCamera.transform.position = originalPos + new Vector3(shake, shake, 0);

                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }

            if (!skipped)
            {
                virtualCamera.transform.position = originalPos;
            }
#endif
        }

        private async Task HandleZoom()
        {
#if CINEMACHINE_PRESENT
            float startSize = virtualCamera.m_Lens.OrthographicSize;
#else
            float startSize = virtualCamera.orthographicSize;
#endif
            float elapsedTime = 0f;

            while (elapsedTime < Duration && !skipped)
            {
                float t = elapsedTime / Duration;
                float newSize = Mathf.Lerp(startSize, targetOrthographicSize, t);

#if CINEMACHINE_PRESENT
                virtualCamera.m_Lens.OrthographicSize = newSize;
#else
                virtualCamera.orthographicSize = newSize;
#endif

                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }

            if (!skipped)
            {
#if CINEMACHINE_PRESENT
                virtualCamera.m_Lens.OrthographicSize = targetOrthographicSize;
#else
                virtualCamera.orthographicSize = targetOrthographicSize;
#endif
            }
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

            if (action == CameraAction.Zoom)
            {
#if CINEMACHINE_PRESENT
                virtualCamera.m_Lens.OrthographicSize = targetOrthographicSize;
#else
                virtualCamera.orthographicSize = targetOrthographicSize;
#endif
            }
            else if (action == CameraAction.Shake)
            {
#if CINEMACHINE_PRESENT
                var noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                if (noise != null)
                {
                    noise.m_AmplitudeGain = 0f;
                    noise.m_FrequencyGain = 0f;
                }
#else
                if (virtualCamera != null)
                {
                    virtualCamera.transform.position = new Vector3(
                        virtualCamera.transform.position.x,
                        virtualCamera.transform.position.y,
                        virtualCamera.transform.position.z
                    );
                }
#endif
            }
        }
    }
}
