using UnityEngine;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Define os tipos elementais disponíveis no jogo
    /// </summary>
    public enum ElementType
    {
        None,
        Earth,   // Terra (Marrom)
        Water,   // Água (Azul)
        Fire,    // Fogo (Vermelho)
        Air      // Ar (Branco)
    }

    /// <summary>
    /// Define os estágios de crescimento do Slime
    /// </summary>
    public enum SlimeStage
    {
        Baby,    // Estágio 1: 0.5x tamanho
        Young,   // Estágio 2: 1.0x tamanho
        Adult,   // Estágio 3: 1.5x tamanho
        King    // Estágio 4: 2.0x tamanho (Slime King)
    }

    /// <summary>
    /// Define os tipos de status effects que podem ser aplicados
    /// </summary>
    public enum StatusEffectType
    {
        None,
        Buff,    // Efeito positivo
        Debuff   // Efeito negativo
    }

    /// <summary>
    /// Define o tipo de modificador de atributo
    /// </summary>
    public enum StatModifierType
    {
        Flat,           // Adição/subtração direta
        PercentAdd,     // Percentual aditivo (somado com outros percentuais)
        PercentMult     // Percentual multiplicativo (aplicado após outros modificadores)
    }
}
