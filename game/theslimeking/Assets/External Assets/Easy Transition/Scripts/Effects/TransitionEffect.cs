using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PixeLadder.EasyTransition
{
    /// <summary>
    /// Classe base para efeitos de transição.
    /// Esta é uma implementação básica para resolver dependências de compilação.
    /// </summary>
    public abstract class TransitionEffect : ScriptableObject
    {
        [Header("Transition Settings")]
        [SerializeField] protected float duration = 1f;
        [SerializeField] protected AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] protected Material transitionMaterialAsset;
        
        /// <summary>
        /// Duração da transição em segundos
        /// </summary>
        public float Duration => duration;
        
        /// <summary>
        /// Curva de animação da transição
        /// </summary>
        public AnimationCurve Curve => curve;
        
        /// <summary>
        /// Material usado para o efeito de transição
        /// </summary>
        public Material transitionMaterial => transitionMaterialAsset;
        
        /// <summary>
        /// Executa o efeito de transição
        /// </summary>
        /// <param name="progress">Progresso da transição (0 a 1)</param>
        public abstract void ApplyEffect(float progress);
        
        /// <summary>
        /// Configura as propriedades específicas do efeito no material
        /// </summary>
        /// <param name="material">Material a ser configurado</param>
        public virtual void SetEffectProperties(Material material)
        {
            // Implementação base - pode ser sobrescrita pelas classes filhas
            if (material != null)
            {
                // Configurações básicas que podem ser aplicadas a qualquer material
                material.SetFloat("_Progress", 0f);
            }
        }
        
        /// <summary>
        /// Anima o efeito para fora (fade out / cover screen)
        /// </summary>
        /// <param name="transitionImage">Imagem usada para a transição</param>
        /// <returns>Corrotina</returns>
        public virtual IEnumerator AnimateOut(Image transitionImage)
        {
            float elapsedTime = 0f;
            float halfDuration = duration * 0.5f;
            
            while (elapsedTime < halfDuration)
            {
                float progress = elapsedTime / halfDuration;
                float evaluatedProgress = curve.Evaluate(progress);
                
                ApplyEffect(evaluatedProgress);
                
                if (transitionImage != null && transitionImage.material != null)
                {
                    transitionImage.material.SetFloat("_Progress", evaluatedProgress);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Garante que chegue ao final
            ApplyEffect(1f);
            if (transitionImage != null && transitionImage.material != null)
            {
                transitionImage.material.SetFloat("_Progress", 1f);
            }
        }
        
        /// <summary>
        /// Anima o efeito para dentro (fade in / uncover screen)
        /// </summary>
        /// <param name="transitionImage">Imagem usada para a transição</param>
        /// <returns>Corrotina</returns>
        public virtual IEnumerator AnimateIn(Image transitionImage)
        {
            float elapsedTime = 0f;
            float halfDuration = duration * 0.5f;
            
            while (elapsedTime < halfDuration)
            {
                float progress = 1f - (elapsedTime / halfDuration);
                float evaluatedProgress = curve.Evaluate(progress);
                
                ApplyEffect(evaluatedProgress);
                
                if (transitionImage != null && transitionImage.material != null)
                {
                    transitionImage.material.SetFloat("_Progress", evaluatedProgress);
                }
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Garante que chegue ao início
            ApplyEffect(0f);
            if (transitionImage != null && transitionImage.material != null)
            {
                transitionImage.material.SetFloat("_Progress", 0f);
            }
        }
        
        /// <summary>
        /// Inicializa o efeito
        /// </summary>
        public virtual void Initialize() { }
        
        /// <summary>
        /// Finaliza o efeito
        /// </summary>
        public virtual void Cleanup() { }
    }
}