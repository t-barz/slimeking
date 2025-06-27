using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Representa um atributo modificável com valor base e modificadores dinâmicos
    /// </summary>
    [System.Serializable]
    public class ModifiableStat
    {
        [SerializeField] private int baseValue;

        // Lista não serializável de modificadores
        private readonly List<StatModifier> statModifiers;

        // Valor calculado final com todos os modificadores aplicados
        private int _value;
        private bool isDirty = true;

        // Acesso somente-leitura aos modificadores
        public ReadOnlyCollection<StatModifier> StatModifiers => statModifiers.AsReadOnly();

        /// <summary>
        /// Valor base do atributo antes de qualquer modificador
        /// </summary>
        public int BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                isDirty = true;
            }
        }

        /// <summary>
        /// Valor calculado final com todos os modificadores aplicados
        /// </summary>
        public int Value
        {
            get
            {
                if (isDirty)
                {
                    _value = CalculateFinalValue();
                    isDirty = false;
                }
                return _value;
            }
        }

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="baseValue">Valor base do atributo</param>
        public ModifiableStat(int baseValue)
        {
            this.baseValue = baseValue;
            statModifiers = new List<StatModifier>();
        }

        /// <summary>
        /// Adiciona um modificador a este atributo
        /// </summary>
        /// <param name="mod">Modificador a ser adicionado</param>
        public virtual void AddModifier(StatModifier mod)
        {
            isDirty = true;
            statModifiers.Add(mod);
            statModifiers.Sort(StatModifier.CompareModifierOrder);
        }

        /// <summary>
        /// Remove um modificador específico deste atributo
        /// </summary>
        /// <param name="mod">Modificador a ser removido</param>
        /// <returns>Se o modificador foi removido com sucesso</returns>
        public virtual bool RemoveModifier(StatModifier mod)
        {
            if (statModifiers.Remove(mod))
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove todos os modificadores de uma fonte específica
        /// </summary>
        /// <param name="source">A fonte dos modificadores a remover</param>
        /// <returns>Quantidade de modificadores removidos</returns>
        public virtual int RemoveAllModifiersFromSource(object source)
        {
            int count = 0;

            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].Source == source)
                {
                    isDirty = true;
                    statModifiers.RemoveAt(i);
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Limpa todos os modificadores
        /// </summary>
        public virtual void ClearModifiers()
        {
            if (statModifiers.Count > 0)
            {
                isDirty = true;
                statModifiers.Clear();
            }
        }

        /// <summary>
        /// Calcula o valor final aplicando todos os modificadores na ordem correta
        /// </summary>
        protected virtual int CalculateFinalValue()
        {
            float finalValue = baseValue;
            float sumPercentAdd = 0;

            // Processa os modificadores na ordem
            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Type == StatModifierType.Flat)
                {
                    // Soma direta
                    finalValue += mod.Value;
                }
                else if (mod.Type == StatModifierType.PercentAdd)
                {
                    // Acumula percentual aditivo
                    sumPercentAdd += mod.Value;

                    // Se for o último modificador deste tipo ou o próximo é de outro tipo
                    if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModifierType.PercentAdd)
                    {
                        // Aplica o total acumulado de percentuais aditivos
                        finalValue *= 1 + sumPercentAdd;
                        sumPercentAdd = 0;
                    }
                }
                else if (mod.Type == StatModifierType.PercentMult)
                {
                    // Multiplicador percentual (aplica-se individualmente)
                    finalValue *= 1 + mod.Value;
                }
            }

            // Arredonda para evitar erros de precisão de ponto flutuante e converte para int
            return Mathf.RoundToInt(finalValue);
        }
    }
}
