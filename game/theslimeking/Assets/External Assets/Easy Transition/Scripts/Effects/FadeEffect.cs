using UnityEngine;

namespace PixeLadder.EasyTransition
{
    /// <summary>
    /// Efeito de transição fade.
    /// Esta é uma implementação básica para resolver dependências de compilação.
    /// </summary>
    [CreateAssetMenu(fileName = "FadeEffect", menuName = "Easy Transition/Fade Effect")]
    public class FadeEffect : TransitionEffect
    {
        [Header("Fade Settings")]
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private bool fadeToColor = true;
        
        /// <summary>
        /// Cor do fade
        /// </summary>
        public Color FadeColor => fadeColor;
        
        /// <summary>
        /// Se true, faz fade para a cor. Se false, faz fade da cor.
        /// </summary>
        public bool FadeToColor => fadeToColor;
        
        /// <summary>
        /// Aplica o efeito de fade
        /// </summary>
        /// <param name="progress">Progresso da transição (0 a 1)</param>
        public override void ApplyEffect(float progress)
        {
            float evaluatedProgress = curve.Evaluate(progress);
            float alpha = fadeToColor ? evaluatedProgress : 1f - evaluatedProgress;
            
            // Em uma implementação real, isso aplicaria o fade ao material/shader
            // Por enquanto, apenas logamos para debug se necessário
            if (Application.isEditor && progress == 0f)
            {
                Debug.Log($"FadeEffect: Starting fade transition with alpha {alpha}");
            }
        }
        
        /// <summary>
        /// Configura as propriedades específicas do efeito fade no material
        /// </summary>
        /// <param name="material">Material a ser configurado</param>
        public override void SetEffectProperties(Material material)
        {
            base.SetEffectProperties(material);
            
            if (material != null)
            {
                // Configura propriedades específicas do efeito fade
                material.SetColor("_FadeColor", fadeColor);
                material.SetFloat("_FadeToColor", fadeToColor ? 1f : 0f);
            }
        }
    }
}