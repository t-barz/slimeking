using UnityEngine;
using System.Collections.Generic;
using System;

namespace TheSlimeKing.Core
{
    /// <summary>
    /// Gerencia os atributos e status do jogador
    /// </summary>
    public class PlayerStatus : MonoBehaviour
    {
        [Header("Stats Base")]
        [SerializeField] private int _baseHealth = 100;
        [SerializeField] private int _baseAttack = 10;
        [SerializeField] private int _baseDefense = 5;
        [SerializeField] private float _baseSpeed = 5f;

        [Header("Multiplicadores")]
        [SerializeField] private float _healthMultiplier = 1f;
        [SerializeField] private float _attackMultiplier = 1f;
        [SerializeField] private float _defenseMultiplier = 1f;
        [SerializeField] private float _speedMultiplier = 1f;

        // Eventos
        public delegate void StatsChangedHandler(Dictionary<string, float> stats);
        public event StatsChangedHandler OnStatsChanged;

        // Status atual
        private int _currentHealth;
        private Dictionary<string, float> _statsBuffs = new Dictionary<string, float>();
        private Dictionary<string, float> _statsFinal = new Dictionary<string, float>();

        private void Awake()
        {
            // Inicializa dicionários
            _statsBuffs["Health"] = 0;
            _statsBuffs["Attack"] = 0;
            _statsBuffs["Defense"] = 0;
            _statsBuffs["Speed"] = 0;

            // Inicializa stats finais
            RecalculateStats();

            // Inicializa vida atual
            _currentHealth = GetMaxHealth();
        }

        public void UpdateBaseStats(int health, int attack, int defense, float speedMult)
        {
            _baseHealth = health;
            _baseAttack = attack;
            _baseDefense = defense;
            _speedMultiplier = speedMult;

            // Recalcular stats
            RecalculateStats();

            // Ajustar vida atual proporcionalmente
            float healthPercent = (float)_currentHealth / GetMaxHealth();
            _currentHealth = Mathf.CeilToInt(GetMaxHealth() * healthPercent);
        }

        /// <summary>
        /// Adiciona ou atualiza um buff temporal em um atributo
        /// </summary>
        public void SetStatBuff(string statName, float value)
        {
            if (_statsBuffs.ContainsKey(statName))
            {
                _statsBuffs[statName] = value;
                RecalculateStats();
            }
        }

        /// <summary>
        /// Recalcula todos os atributos considerando base + buffs
        /// </summary>
        private void RecalculateStats()
        {
            // Calcular health
            _statsFinal["Health"] = (_baseHealth + _statsBuffs["Health"]) * _healthMultiplier;

            // Calcular attack
            _statsFinal["Attack"] = (_baseAttack + _statsBuffs["Attack"]) * _attackMultiplier;

            // Calcular defense
            _statsFinal["Defense"] = (_baseDefense + _statsBuffs["Defense"]) * _defenseMultiplier;

            // Calcular speed
            _statsFinal["Speed"] = (_baseSpeed + _statsBuffs["Speed"]) * _speedMultiplier;

            // Notificar mudança
            OnStatsChanged?.Invoke(_statsFinal);
        }

        /// <summary>
        /// Retorna o valor máximo de saúde atual
        /// </summary>
        public int GetMaxHealth()
        {
            return Mathf.RoundToInt(_statsFinal["Health"]);
        }

        /// <summary>
        /// Retorna o valor de ataque atual
        /// </summary>
        public int GetAttack()
        {
            return Mathf.RoundToInt(_statsFinal["Attack"]);
        }

        /// <summary>
        /// Retorna o valor de defesa atual
        /// </summary>
        public int GetDefense()
        {
            return Mathf.RoundToInt(_statsFinal["Defense"]);
        }

        /// <summary>
        /// Retorna o multiplicador de velocidade atual
        /// </summary>
        public float GetSpeedMultiplier()
        {
            return _statsFinal["Speed"] / _baseSpeed;
        }

        /// <summary>
        /// Retorna a saúde atual do jogador
        /// </summary>
        public int GetCurrentHealth()
        {
            return _currentHealth;
        }

        /// <summary>
        /// Aplica dano ao jogador considerando defesa
        /// </summary>
        public int TakeDamage(int rawDamage)
        {
            int defense = GetDefense();
            int actualDamage = Mathf.Max(rawDamage - defense, 1); // Sempre pelo menos 1 de dano

            _currentHealth = Mathf.Max(0, _currentHealth - actualDamage);

            // Aqui você pode adicionar eventos de dano, etc.

            return actualDamage;
        }

        /// <summary>
        /// Recupera saúde do jogador
        /// </summary>
        public int HealHealth(int amount)
        {
            int maxHealth = GetMaxHealth();
            int healingDone = Mathf.Min(amount, maxHealth - _currentHealth);

            _currentHealth += healingDone;

            // Aqui você pode adicionar eventos de cura, etc.

            return healingDone;
        }
    }
}
