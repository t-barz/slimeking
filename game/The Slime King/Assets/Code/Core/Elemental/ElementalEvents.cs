using System;
using UnityEngine;

namespace TheSlimeKing.Core.Elemental
{
    /// <summary>
    /// Eventos globais relacionados ao sistema elemental
    /// </summary>
    public static class ElementalEvents
    {
        // Delegados para os eventos
        public delegate void FragmentAbsorbedDelegate(ElementalType type, int amount, Vector3 position);
        public delegate void ElementalAbilityUsedDelegate(ElementalType type, int cost, Vector3 position);
        public delegate void ElementalThresholdReachedDelegate(int stage);

        // Eventos estáticos
        public static FragmentAbsorbedDelegate OnFragmentAbsorbed;
        public static ElementalAbilityUsedDelegate OnElementalAbilityUsed;
        public static ElementalThresholdReachedDelegate OnElementalThresholdReached;
    }
}
