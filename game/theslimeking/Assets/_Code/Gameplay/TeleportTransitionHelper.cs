namespace PixeLadder.EasyTransition
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Helper estático para executar transições visuais durante teletransporte na mesma cena.
    /// Adapta o Easy Transition para funcionar com teletransporte sem mudança de cena.
    /// </summary>
    public static class TeleportTransitionHelper
    {
        #region Public Methods

        /// <summary>
        /// Executa uma transição visual completa (fade out → callback → delay → fade in).
        /// </summary>
        /// <param name="effect">Efeito de transição a ser utilizado (ex: CircleEffect)</param>
        /// <param name="onMidTransition">Callback executado após fade out, usado para reposicionamento</param>
        /// <param name="delayBeforeFadeIn">Tempo de espera após callback antes de iniciar fade in</param>
        /// <param name="enableDebugLogs">Habilita logs de debug para troubleshooting</param>
        /// <returns>Corrotina que pode ser iniciada com StartCoroutine</returns>
        public static IEnumerator ExecuteTransition(
            TransitionEffect effect,
            Action onMidTransition,
            float delayBeforeFadeIn = 1f,
            bool enableDebugLogs = false)
        {
            // Validações
            if (effect == null)
            {
                Debug.LogError("TeleportTransitionHelper: TransitionEffect não pode ser nulo!");
                yield break;
            }

            if (SceneTransitioner.Instance == null)
            {
                Debug.LogError("TeleportTransitionHelper: SceneTransitioner.Instance não encontrado na cena!");
                yield break;
            }

            if (enableDebugLogs)
                Debug.Log($"TeleportTransitionHelper: Iniciando transição com efeito '{effect.name}'");

            // Acessa a imagem de transição através de reflexão (campo privado)
            var transitionImageField = typeof(SceneTransitioner).GetField(
                "transitionImageInstance",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
            );

            if (transitionImageField == null)
            {
                Debug.LogError("TeleportTransitionHelper: Não foi possível acessar transitionImageInstance!");
                yield break;
            }

            Image transitionImage = transitionImageField.GetValue(SceneTransitioner.Instance) as Image;

            if (transitionImage == null)
            {
                Debug.LogError("TeleportTransitionHelper: transitionImageInstance é nulo!");
                yield break;
            }

            // Prepara o material e ativa a imagem
            transitionImage.gameObject.SetActive(true);
            Material materialInstance = new Material(effect.transitionMaterial);
            transitionImage.material = materialInstance;
            effect.SetEffectProperties(materialInstance);

            if (enableDebugLogs)
                Debug.Log($"TeleportTransitionHelper: Material aplicado - {materialInstance.name}");

            // Fase 1: Fade Out (cobre a tela)
            if (enableDebugLogs)
                Debug.Log("TeleportTransitionHelper: Executando fade out...");

            yield return effect.AnimateOut(transitionImage);

            if (enableDebugLogs)
                Debug.Log("TeleportTransitionHelper: Fade out completo");

            // Fase 2: Callback de reposicionamento
            if (enableDebugLogs)
                Debug.Log("TeleportTransitionHelper: Executando callback de reposicionamento...");

            onMidTransition?.Invoke();

            if (enableDebugLogs)
                Debug.Log("TeleportTransitionHelper: Callback completo");

            // Fase 3: Delay configurável
            if (delayBeforeFadeIn > 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"TeleportTransitionHelper: Aguardando {delayBeforeFadeIn}s antes do fade in...");

                yield return new WaitForSeconds(delayBeforeFadeIn);
            }

            // Fase 4: Fade In (descobre a tela)
            if (enableDebugLogs)
                Debug.Log("TeleportTransitionHelper: Executando fade in...");

            yield return effect.AnimateIn(transitionImage);

            if (enableDebugLogs)
                Debug.Log("TeleportTransitionHelper: Fade in completo");

            // Cleanup: desativa a imagem e limpa o material
            transitionImage.gameObject.SetActive(false);

            if (materialInstance != null)
            {
                UnityEngine.Object.DestroyImmediate(materialInstance);
            }

            if (enableDebugLogs)
                Debug.Log("TeleportTransitionHelper: Transição completa!");
        }

        #endregion
    }
}
