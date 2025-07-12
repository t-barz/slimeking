using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representa os atributos/status de personagens, inimigos e objetos destrutíveis,
/// incluindo suporte a modificadores aditivos e multiplicativos, conforme o GDD.
/// Pode ser usada para Slimes, inimigos, NPCs e objetos interativos.
/// </summary>
[Serializable]
public class EntityStatus : MonoBehaviour
{
    [Header("Atributos Base")]
    [Tooltip("Vida máxima base do personagem/entidade.")]
    public int baseHP = 10;

    [Tooltip("Defesa base do personagem/entidade.")]
    public int baseDefense = 0;

    [Tooltip("Ataque básico base do personagem/entidade.")]
    public int baseAttack = 1;

    [Tooltip("Ataque especial base do personagem/entidade.")]
    public int baseSpecialAttack = 0;

    [Tooltip("Nível base da entidade (afeta progressão de atributos).")]
    public int baseLevel = 1;

    [Tooltip("Velocidade base de deslocamento da entidade.")]
    public float baseSpeed = 0f;

    [Header("Atributos Atuais")]
    [Tooltip("Vida atual da entidade.")]
    public int currentHP;

    [Tooltip("Nível atual da entidade.")]
    public int currentLevel = 1;

    [Header("Modificadores")]
    [Tooltip("Lista de modificadores ativos (buffs, debuffs, equipamentos, etc).")]
    public List<StatusModifier> modifiers = new();

    /// <summary>
    /// Evento disparado quando a entidade morre (HP <= 0).
    /// </summary>
    public event Action OnDeath;

    private void Awake()
    {
        ResetStatus();
    }

    /// <summary>
    /// Reseta os atributos atuais para os valores base (considerando o nível).
    /// </summary>
    public void ResetStatus()
    {
        currentLevel = Mathf.Max(1, baseLevel);
        currentHP = GetMaxHP();
    }

    /// <summary>
    /// Calcula o valor máximo de HP considerando modificadores e nível.
    /// </summary>
    public int GetMaxHP()
    {
        float value = baseHP * currentLevel;
        value = ApplyModifiers(value, StatusAttribute.HP);
        return Mathf.RoundToInt(value);
    }

    /// <summary>
    /// Retorna o valor de defesa atual, considerando modificadores e nível.
    /// </summary>
    public int GetDefense()
    {
        float value = baseDefense * currentLevel;
        value = ApplyModifiers(value, StatusAttribute.Defense);
        return Mathf.RoundToInt(value);
    }

    /// <summary>
    /// Retorna o valor de ataque atual, considerando modificadores e nível.
    /// </summary>
    public int GetAttack()
    {
        float value = baseAttack * currentLevel;
        value = ApplyModifiers(value, StatusAttribute.Attack);
        return Mathf.RoundToInt(value);
    }

    /// <summary>
    /// Retorna o valor de ataque especial atual, considerando modificadores e nível.
    /// </summary>
    public int GetSpecialAttack()
    {
        float value = baseSpecialAttack * currentLevel;
        value = ApplyModifiers(value, StatusAttribute.SpecialAttack);
        return Mathf.RoundToInt(value);
    }

    /// <summary>
    /// Retorna o valor de velocidade (speed) atual, considerando modificadores.
    /// </summary>
    public float GetSpeed()
    {
        float value = baseSpeed;
        value = ApplyModifiers(value, StatusAttribute.Speed);
        return value;
    }

    /// <summary>
    /// Aplica todos os modificadores ativos ao valor de um atributo.
    /// </summary>
    /// <param name="baseValue">Valor base do atributo.</param>
    /// <param name="attr">Atributo a ser modificado.</param>
    /// <returns>Valor final após modificadores.</returns>
    private float ApplyModifiers(float baseValue, StatusAttribute attr)
    {
        float additive = 0f;
        float multiplicative = 1f;
        foreach (var mod in modifiers)
        {
            if (mod.attribute == attr)
            {
                if (mod.type == ModifierType.Additive)
                    additive += mod.value;
                else if (mod.type == ModifierType.Multiplicative)
                    multiplicative *= (1f + mod.value);
            }
        }
        return (baseValue + additive) * multiplicative;
    }

    /// <summary>
    /// Aplica dano ao status, considerando defesa e modificadores.
    /// </summary>
    /// <param name="attackValue">Valor do ataque recebido.</param>
    /// <returns>Dano efetivo causado.</returns>
    public int TakeDamage(int attackValue)
    {
        int defense = GetDefense();
        int damage = Mathf.Max(attackValue - defense, 0);
        if (damage > 0)
        {
            currentHP -= damage;
            if (currentHP <= 0)
            {
                currentHP = 0;
                OnDeath?.Invoke();
            }
        }
        return damage;
    }

    /// <summary>
    /// Retorna true se a entidade está "viva" (HP > 0).
    /// </summary>
    public bool IsAlive() => currentHP > 0;

    /// <summary>
    /// Adiciona um modificador temporário ou permanente.
    /// </summary>
    /// <param name="mod">Modificador a ser adicionado.</param>
    public void AddModifier(StatusModifier mod)
    {
        modifiers.Add(mod);
    }

    /// <summary>
    /// Remove um modificador.
    /// </summary>
    /// <param name="mod">Modificador a ser removido.</param>
    public void RemoveModifier(StatusModifier mod)
    {
        modifiers.Remove(mod);
    }
}

/// <summary>
/// Enum para os atributos que podem ser modificados.
/// </summary>
public enum StatusAttribute
{
    HP,
    Defense,
    Attack,
    SpecialAttack,
    Speed
}

/// <summary>
/// Enum para o tipo de modificador.
/// </summary>
public enum ModifierType
{
    Additive,       // Soma/subtrai valor fixo
    Multiplicative  // Multiplica por percentual (ex: 0.2 = +20%)
}

/// <summary>
/// Representa um modificador de atributo (buff, debuff, equipamento, etc).
/// </summary>
[Serializable]
public class StatusModifier
{
    [Tooltip("Atributo afetado pelo modificador.")]
    public StatusAttribute attribute;

    [Tooltip("Tipo de modificação (aditiva ou multiplicativa).")]
    public ModifierType type;

    [Tooltip("Valor do modificador. Ex: +5 (aditivo), +0.2 (multiplicativo = +20%)")]
    public float value;

    [Tooltip("Duração em segundos (0 = permanente, >0 = temporário).")]
    public float duration = 0f;

    public StatusModifier(StatusAttribute attr, ModifierType type, float value, float duration = 0f)
    {
        this.attribute = attr;
        this.type = type;
        this.value = value;
        this.duration = duration;
    }
}