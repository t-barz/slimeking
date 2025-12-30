using UnityEngine;
using SlimeKing.Core;

namespace SlimeKing.Data
{
    /// <summary>
    /// ScriptableObject que define as propriedades de um cristal elemental.
    /// Usado para configurar diferentes tipos de cristais coletáveis no jogo.
    /// </summary>
    [CreateAssetMenu(fileName = "New Crystal Data", menuName = "Items/Crystal Elemental Data")]
    public class CrystalElementalData : ScriptableObject
    {
        [Header("Crystal Identification")]
        [Tooltip("Tipo elemental do cristal")]
        public CrystalType crystalType = CrystalType.Nature;

        [Tooltip("Nome do cristal")]
        public string crystalName = "Nature Crystal";

        [Tooltip("Descrição do cristal")]
        [TextArea(2, 4)]
        public string description = "Um cristal da natureza que pulsa com energia vital.";

        [Header("Collection Properties")]
        [Tooltip("Quantidade de cristais que este item adiciona quando coletado")]
        [Range(1, 10)]
        public int value = 1;

        [Tooltip("Raio de atração magnética")]
        [Range(1f, 10f)]
        public float attractionRadius = 2.5f;

        [Tooltip("Velocidade de atração magnética")]
        [Range(1f, 10f)]
        public float attractionSpeed = 4f;

        [Header("Visual Appearance")]
        [Tooltip("Cor do cristal")]
        public Color crystalTint = Color.green;

        [Tooltip("Sprite do cristal")]
        public Sprite crystalSprite;

        [Header("Collection Effects")]
        [Tooltip("Efeito visual executado quando o cristal é coletado")]
        public GameObject collectVFX;

        [Tooltip("Som executado quando o cristal é coletado")]
        public AudioClip collectSound;

        [Header("Advanced Settings")]
        [Tooltip("Delay antes da atração magnética ser ativada")]
        [Range(0f, 2f)]
        public float activationDelay = 0.5f;

        [Tooltip("Habilitar logs de debug para este cristal")]
        public bool enableDebugLogs = true;

        #region Validation
        private void OnValidate()
        {
            // Garante que o nome do cristal seja consistente com o tipo
            if (!crystalName.Contains(crystalType.ToString()))
            {
                crystalName = $"{crystalType} Crystal";
            }

            // Garante valores mínimos
            if (value < 1) value = 1;
            if (attractionRadius < 0.5f) attractionRadius = 0.5f;
            if (attractionSpeed < 0.1f) attractionSpeed = 0.1f;
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Retorna uma descrição completa do cristal para debug
        /// </summary>
        public string GetDebugDescription()
        {
            return $"[{crystalType}] {crystalName} (Value: {value}, Attraction: {attractionRadius}u @ {attractionSpeed}u/s)";
        }

        /// <summary>
        /// Verifica se este cristal tem efeitos visuais configurados
        /// </summary>
        public bool HasVisualEffects => collectVFX != null;

        /// <summary>
        /// Verifica se este cristal tem efeitos sonoros configurados
        /// </summary>
        public bool HasAudioEffects => collectSound != null;
        #endregion
    }
}