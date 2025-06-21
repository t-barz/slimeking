using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Define um efeito de status (buff ou debuff) que pode ser aplicado a um personagem
    /// </summary>
    [Serializable]
    public class StatusEffect
    {
        [SerializeField] private string effectName;
        [SerializeField] private string description;
        [SerializeField] private StatusEffectType effectType;
        [SerializeField] private float duration;
        [SerializeField] private bool isStackable;
        [SerializeField] private int maxStacks;
        [SerializeField] private Sprite icon;
        [SerializeField] private ParticleSystem visualEffect;
        [SerializeField] private AudioClip soundEffect;

        // Dados de runtime
        private float remainingDuration;
        private int currentStacks = 1;
        private bool isPermanent;
        private object source;
        private List<StatModifier> statModifiers = new List<StatModifier>();

        // Propriedades públicas
        public string EffectName => effectName;
        public string Description => description;
        public StatusEffectType EffectType => effectType;
        public float Duration => duration;
        public bool IsStackable => isStackable;
        public int MaxStacks => maxStacks;
        public Sprite Icon => icon;
        public ParticleSystem VisualEffect => visualEffect;
        public AudioClip SoundEffect => soundEffect;
        public float RemainingDuration => remainingDuration;
        public int CurrentStacks => currentStacks;
        public bool IsPermanent => isPermanent;
        public object Source => source;
        public List<StatModifier> StatModifiers => statModifiers;

        /// <summary>
        /// Cria um novo efeito de status
        /// </summary>
        public StatusEffect(string effectName, StatusEffectType effectType, float duration, bool isPermanent = false, object source = null)
        {
            this.effectName = effectName;
            this.effectType = effectType;
            this.duration = duration;
            this.remainingDuration = duration;
            this.isPermanent = isPermanent;
            this.source = source;
        }

        /// <summary>
        /// Adiciona um modificador de atributo a este efeito de status
        /// </summary>
        /// <param name="modifier">O modificador a ser adicionado</param>
        public void AddModifier(StatModifier modifier)
        {
            statModifiers.Add(modifier);
        }

        /// <summary>
        /// Incrementa a quantidade de stacks deste efeito
        /// </summary>
        /// <returns>Se foi possível adicionar mais um stack</returns>
        public bool AddStack()
        {
            if (!isStackable || currentStacks >= maxStacks)
                return false;

            currentStacks++;
            return true;
        }

        /// <summary>
        /// Atualiza a duração restante do efeito
        /// </summary>
        /// <param name="deltaTime">Tempo decorrido desde a última atualização</param>
        /// <returns>Se o efeito ainda está ativo</returns>
        public bool UpdateEffect(float deltaTime)
        {
            if (isPermanent)
                return true;

            remainingDuration -= deltaTime;
            return remainingDuration > 0;
        }

        /// <summary>
        /// Reseta a duração do efeito
        /// </summary>
        public void RefreshDuration()
        {
            remainingDuration = duration;
        }

        /// <summary>
        /// Cria uma cópia deste efeito de status
        /// </summary>
        public StatusEffect Clone()
        {
            StatusEffect clone = new StatusEffect(effectName, effectType, duration, isPermanent, source);
            clone.description = description;
            clone.isStackable = isStackable;
            clone.maxStacks = maxStacks;
            clone.icon = icon;
            clone.visualEffect = visualEffect;
            clone.soundEffect = soundEffect;
            clone.remainingDuration = duration;

            // Copia os modificadores
            foreach (var modifier in statModifiers)
            {
                clone.AddModifier(new StatModifier(modifier.Value, modifier.Type, modifier.Order, modifier.Source));
            }

            return clone;
        }
    }
}
