using UnityEngine;

namespace PixeLadder.EasyTransition
{
    /// <summary>
    /// Efeito de transição circular.
    /// Esta é uma implementação básica para resolver dependências de compilação.
    /// </summary>
    [CreateAssetMenu(fileName = "CircleEffect", menuName = "Easy Transition/Circle Effect")]
    public class CircleEffect : TransitionEffect
    {
        [Header("Circle Settings")]
        [SerializeField] private Vector2 center = new Vector2(0.5f, 0.5f);
        [SerializeField] private float maxRadius = 1.5f;
        [SerializeField] private bool invertEffect = false;
        
        /// <summary>
        /// Centro do efeito circular (0-1 em coordenadas de tela)
        /// </summary>
        public Vector2 Center => center;
        
        /// <summary>
        /// Raio máximo do efeito
        /// </summary>
        public float MaxRadius => maxRadius;
        
        /// <summary>
        /// Se true, inverte o efeito (fecha ao invés de abrir)
        /// </summary>
        public bool InvertEffect => invertEffect;
        
        /// <summary>
        /// Aplica o efeito circular
        /// </summary>
        /// <param name="progress">Progresso da transição (0 a 1)</param>
        public override void ApplyEffect(float progress)
        {
            // Implementação básica - apenas para resolver compilação
            float evaluatedProgress = curve.Evaluate(progress);
            float currentRadius = invertEffect ? maxRadius * (1f - evaluatedProgress) : maxRadius * evaluatedProgress;
            
            // Em uma implementação real, isso aplicaria o efeito ao material/shader
            // Por enquanto, apenas logamos para debug se necessário
            if (Application.isEditor && progress == 0f)
            {
                Debug.Log($"CircleEffect: Starting transition with radius {currentRadius}");
            }
        }
        
        /// <summary>
        /// Configura as propriedades específicas do efeito circular no material
        /// </summary>
        /// <param name="material">Material a ser configurado</param>
        public override void SetEffectProperties(Material material)
        {
            base.SetEffectProperties(material);
            
            if (material != null)
            {
                // Configura propriedades específicas do efeito circular
                material.SetVector("_Center", new Vector4(center.x, center.y, 0f, 0f));
                material.SetFloat("_MaxRadius", maxRadius);
                material.SetFloat("_InvertEffect", invertEffect ? 1f : 0f);
            }
        }
        
        public override void Initialize()
        {
            base.Initialize();
            // Inicialização específica do efeito circular
        }
        
        public override void Cleanup()
        {
            base.Cleanup();
            // Limpeza específica do efeito circular
        }
    }
}