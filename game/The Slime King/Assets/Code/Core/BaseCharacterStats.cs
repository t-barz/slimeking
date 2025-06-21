using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Classe base abstrata para todos os personagens do jogo (Slime e inimigos)
    /// </summary>
    public abstract class BaseCharacterStats : MonoBehaviour
    {
        [Header("Atributos Básicos")]
        [SerializeField] protected int baseHealth;
        [SerializeField] protected int baseAttack;
        [SerializeField] protected int baseSpecial;
        [SerializeField] protected int baseDefense;
        [SerializeField] protected int baseLevel = 1;

        [Header("Elemental")]
        [SerializeField] protected ElementalType primaryElement;

        // Stats modificáveis
        protected ModifiableStat maxHealth;
        protected ModifiableStat attack;
        protected ModifiableStat special;
        protected ModifiableStat defense;
        protected ModifiableStat level;

        // Saúde atual
        protected float currentHealth;

        // Lista de efeitos de status ativos
        protected List<StatusEffect> activeStatusEffects = new List<StatusEffect>();

        // Evento para mudanças de saúde
        public event Action<float, float> OnHealthChanged; // (oldHealth, newHealth)

        // Evento para mudanças de status
        public event Action<StatusEffect> OnStatusEffectAdded;
        public event Action<StatusEffect> OnStatusEffectRemoved;
        public event Action<StatusEffect> OnStatusEffectUpdated;

        // Propriedades públicas
        public float CurrentHealth
        {
            get => currentHealth;
            protected set
            {
                float oldHealth = currentHealth;
                // Garante que a saúde não ultrapasse o máximo ou fique abaixo de zero
                currentHealth = Mathf.Clamp(value, 0, MaxHealth.Value);

                if (oldHealth != currentHealth)
                {
                    OnHealthChanged?.Invoke(oldHealth, currentHealth);
                }
            }
        }

        public ModifiableStat MaxHealth => maxHealth;
        public ModifiableStat Attack => attack;
        public ModifiableStat Special => special;
        public ModifiableStat Defense => defense;
        public ModifiableStat Level => level;
        public ElementalType PrimaryElement => primaryElement;
        public IReadOnlyList<StatusEffect> ActiveStatusEffects => activeStatusEffects;

        protected virtual void Awake()
        {
            // Inicializa os atributos modificáveis
            maxHealth = new ModifiableStat(baseHealth);
            attack = new ModifiableStat(baseAttack);
            special = new ModifiableStat(baseSpecial);
            defense = new ModifiableStat(baseDefense);
            level = new ModifiableStat(baseLevel);

            // Define a saúde atual como o máximo inicial
            currentHealth = maxHealth.Value;
        }

        protected virtual void Update()
        {
            // Atualiza os efeitos de status
            UpdateStatusEffects(Time.deltaTime);
        }

        /// <summary>
        /// Aplica dano ao personagem
        /// </summary>
        /// <param name="amount">Quantidade de dano</param>
        /// <param name="element">Elemento do dano (opcional)</param>
        /// <returns>Quantidade de dano realmente causado</returns>
        public virtual float TakeDamage(float amount, ElementType element = ElementType.None)
        {
            // Calcula o dano baseado na defesa
            float damageReduction = defense.Value / (defense.Value + 50); // Fórmula que impede redução total
            float reducedDamage = amount * (1 - damageReduction);

            // Modificação elemental
            if (element != ElementType.None && primaryElement != null)
            {
                // Se o atacante usou um elemento, aplica resistências/vulnerabilidades
                reducedDamage = primaryElement.CalculateElementalDamage(element, reducedDamage);
            }

            // Aplica o dano
            CurrentHealth -= reducedDamage;

            // Retorna o dano real causado
            return reducedDamage;
        }

        /// <summary>
        /// Cura o personagem
        /// </summary>
        /// <param name="amount">Quantidade de cura</param>
        /// <returns>Quantidade real curada</returns>
        public virtual float Heal(float amount)
        {
            float oldHealth = CurrentHealth;
            CurrentHealth += amount;
            return CurrentHealth - oldHealth;
        }

        /// <summary>
        /// Adiciona um efeito de status ao personagem
        /// </summary>
        /// <param name="effect">Efeito a ser aplicado</param>
        public virtual void AddStatusEffect(StatusEffect effect)
        {
            // Verifica se já existe um efeito do mesmo nome
            StatusEffect existingEffect = activeStatusEffects.Find(e => e.EffectName == effect.EffectName);

            if (existingEffect != null)
            {
                // Se é stackable, incrementa o stack ou renova duração
                if (existingEffect.IsStackable && existingEffect.AddStack())
                {
                    existingEffect.RefreshDuration();
                    OnStatusEffectUpdated?.Invoke(existingEffect);
                }
                else
                {
                    // Caso contrário, apenas renova duração
                    existingEffect.RefreshDuration();
                    OnStatusEffectUpdated?.Invoke(existingEffect);
                }
            }
            else
            {
                // Adiciona o novo efeito
                activeStatusEffects.Add(effect);

                // Aplica os modificadores deste efeito
                foreach (var modifier in effect.StatModifiers)
                {
                    ApplyStatModifier(modifier);
                }

                OnStatusEffectAdded?.Invoke(effect);
            }
        }

        /// <summary>
        /// Remove um efeito de status
        /// </summary>
        /// <param name="effectName">Nome do efeito a ser removido</param>
        /// <returns>Se o efeito foi removido com sucesso</returns>
        public virtual bool RemoveStatusEffect(string effectName)
        {
            for (int i = 0; i < activeStatusEffects.Count; i++)
            {
                if (activeStatusEffects[i].EffectName == effectName)
                {
                    StatusEffect effect = activeStatusEffects[i];

                    // Remove modificadores deste efeito
                    foreach (var modifier in effect.StatModifiers)
                    {
                        RemoveStatModifier(modifier);
                    }

                    activeStatusEffects.RemoveAt(i);
                    OnStatusEffectRemoved?.Invoke(effect);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Atualiza todos os efeitos de status ativos
        /// </summary>
        /// <param name="deltaTime">Tempo desde a última atualização</param>
        protected virtual void UpdateStatusEffects(float deltaTime)
        {
            for (int i = activeStatusEffects.Count - 1; i >= 0; i--)
            {
                StatusEffect effect = activeStatusEffects[i];

                // Atualiza duração e verifica se ainda está ativo
                if (!effect.UpdateEffect(deltaTime))
                {
                    // Remove modificadores deste efeito
                    foreach (var modifier in effect.StatModifiers)
                    {
                        RemoveStatModifier(modifier);
                    }

                    activeStatusEffects.RemoveAt(i);
                    OnStatusEffectRemoved?.Invoke(effect);
                }
            }
        }

        /// <summary>
        /// Aplica um modificador de atributo ao stat apropriado
        /// </summary>
        /// <param name="modifier">Modificador a ser aplicado</param>
        protected virtual void ApplyStatModifier(StatModifier modifier)
        {
            // NOTA: Este método pode ser expandido com mais atributos conforme necessário
            if (modifier.Source is string sourceStr)
            {
                switch (sourceStr.ToLower())
                {
                    case "health":
                    case "maxhealth":
                        maxHealth.AddModifier(modifier);
                        break;
                    case "attack":
                        attack.AddModifier(modifier);
                        break;
                    case "special":
                        special.AddModifier(modifier);
                        break;
                    case "defense":
                        defense.AddModifier(modifier);
                        break;
                    case "level":
                        level.AddModifier(modifier);
                        break;
                }
            }
        }

        /// <summary>
        /// Remove um modificador de atributo do stat apropriado
        /// </summary>
        /// <param name="modifier">Modificador a ser removido</param>
        protected virtual void RemoveStatModifier(StatModifier modifier)
        {
            // NOTA: Este método pode ser expandido com mais atributos conforme necessário
            if (modifier.Source is string sourceStr)
            {
                switch (sourceStr.ToLower())
                {
                    case "health":
                    case "maxhealth":
                        maxHealth.RemoveModifier(modifier);
                        break;
                    case "attack":
                        attack.RemoveModifier(modifier);
                        break;
                    case "special":
                        special.RemoveModifier(modifier);
                        break;
                    case "defense":
                        defense.RemoveModifier(modifier);
                        break;
                    case "level":
                        level.RemoveModifier(modifier);
                        break;
                }
            }
        }

        /// <summary>
        /// Verifica se o personagem está neutralizado (saúde zero)
        /// </summary>
        public bool IsNeutralized()
        {
            return currentHealth <= 0;
        }

        /// <summary>
        /// Restaura a saúde ao máximo
        /// </summary>
        public virtual void RestoreFullHealth()
        {
            CurrentHealth = maxHealth.Value;
        }
    }
}
