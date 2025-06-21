using System;
using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Representa um modificador de atributo que pode ser aplicado a qualquer característica
    /// </summary>
    [Serializable]
    public class StatModifier
    {
        [SerializeField] private float value;
        [SerializeField] private StatModifierType type;
        [SerializeField] private int order;
        [SerializeField] private object source;

        // Propriedades públicas
        public float Value => value;
        public StatModifierType Type => type;
        public int Order => order;
        public object Source => source;

        /// <summary>
        /// Cria um novo modificador de atributo
        /// </summary>
        /// <param name="value">Valor do modificador</param>
        /// <param name="type">Tipo do modificador</param>
        /// <param name="order">Ordem de aplicação (opcional)</param>
        /// <param name="source">Objeto origem do modificador (opcional)</param>
        public StatModifier(float value, StatModifierType type, int order = 0, object source = null)
        {
            this.value = value;
            this.type = type;
            this.order = order;
            this.source = source;
        }

        /// <summary>
        /// Compara dois modificadores para ordenar a aplicação
        /// </summary>
        public static int CompareModifierOrder(StatModifier a, StatModifier b)
        {
            if (a.Order < b.Order)
                return -1;
            else if (a.Order > b.Order)
                return 1;
            return 0; // Igual
        }
    }
}
