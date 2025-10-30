using System;
using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Gerencia o sistema de amizade do NPC com o jogador.
    /// Rastreia níveis de amizade e desbloqueia benefícios.
    /// </summary>
    public class NPCFriendship : MonoBehaviour
    {
        [Header("Friendship Configuration")]
        [Tooltip("Referência ao ScriptableObject com dados de amizade da espécie")]
        public FriendshipData friendshipData;

        [Header("Current State")]
        [Tooltip("Nível de amizade atual (0 = Desconhecido, 5 = Companheiro Leal)")]
        [SerializeField]
        private int currentLevel = 0;

        /// <summary>
        /// Evento disparado quando o nível de amizade muda.
        /// Parâmetro: novo nível de amizade.
        /// </summary>
        public event Action<int> OnFriendshipLevelChanged;

        private void Start()
        {
            // TODO: Registrar no FriendshipManager quando o sistema for implementado
            // if (FriendshipManager.Instance != null)
            // {
            //     FriendshipManager.Instance.RegisterNPC(this);
            //     currentLevel = FriendshipManager.Instance.GetFriendshipLevel(friendshipData.speciesName);
            // }

            if (friendshipData == null)
            {
                Debug.LogWarning($"⚠️ NPCFriendship em '{gameObject.name}' não possui FriendshipData atribuído!", this);
            }
            else
            {
                Debug.Log($"💚 Sistema de amizade inicializado para '{friendshipData.speciesName}' - Nível: {currentLevel}");
            }
        }

        /// <summary>
        /// Aumenta o nível de amizade em uma quantidade específica.
        /// </summary>
        /// <param name="amount">Quantidade de níveis a aumentar (padrão: 1)</param>
        public void IncreaseFriendship(int amount = 1)
        {
            if (friendshipData == null)
            {
                Debug.LogWarning($"⚠️ Não é possível aumentar amizade: FriendshipData não atribuído em '{gameObject.name}'");
                return;
            }

            int previousLevel = currentLevel;
            currentLevel = Mathf.Min(currentLevel + amount, friendshipData.maxLevel);

            if (currentLevel != previousLevel)
            {
                Debug.Log($"💚 Amizade com '{friendshipData.speciesName}' aumentou! Nível: {previousLevel} → {currentLevel}");
                
                // Exibir informações do novo nível
                DisplayLevelInfo(currentLevel);

                // TODO: Atualizar FriendshipManager quando implementado
                // if (FriendshipManager.Instance != null)
                // {
                //     FriendshipManager.Instance.UpdateFriendship(friendshipData.speciesName, currentLevel);
                // }

                // Disparar evento
                OnFriendshipLevelChanged?.Invoke(currentLevel);
            }
            else
            {
                Debug.Log($"💚 '{friendshipData.speciesName}' já está no nível máximo de amizade ({currentLevel})");
            }
        }

        /// <summary>
        /// Diminui o nível de amizade em uma quantidade específica.
        /// </summary>
        /// <param name="amount">Quantidade de níveis a diminuir (padrão: 1)</param>
        public void DecreaseFriendship(int amount = 1)
        {
            if (friendshipData == null)
            {
                Debug.LogWarning($"⚠️ Não é possível diminuir amizade: FriendshipData não atribuído em '{gameObject.name}'");
                return;
            }

            int previousLevel = currentLevel;
            currentLevel = Mathf.Max(currentLevel - amount, 0);

            if (currentLevel != previousLevel)
            {
                Debug.Log($"💔 Amizade com '{friendshipData.speciesName}' diminuiu! Nível: {previousLevel} → {currentLevel}");
                
                // Exibir informações do novo nível
                DisplayLevelInfo(currentLevel);

                // TODO: Atualizar FriendshipManager quando implementado
                // if (FriendshipManager.Instance != null)
                // {
                //     FriendshipManager.Instance.UpdateFriendship(friendshipData.speciesName, currentLevel);
                // }

                // Disparar evento
                OnFriendshipLevelChanged?.Invoke(currentLevel);
            }
            else
            {
                Debug.Log($"💔 '{friendshipData.speciesName}' já está no nível mínimo de amizade ({currentLevel})");
            }
        }

        /// <summary>
        /// Define o nível de amizade diretamente.
        /// </summary>
        /// <param name="level">Novo nível de amizade</param>
        public void SetFriendshipLevel(int level)
        {
            if (friendshipData == null)
            {
                Debug.LogWarning($"⚠️ Não é possível definir amizade: FriendshipData não atribuído em '{gameObject.name}'");
                return;
            }

            int previousLevel = currentLevel;
            currentLevel = Mathf.Clamp(level, 0, friendshipData.maxLevel);

            if (currentLevel != previousLevel)
            {
                Debug.Log($"💚 Amizade com '{friendshipData.speciesName}' definida para nível {currentLevel}");
                
                // Exibir informações do novo nível
                DisplayLevelInfo(currentLevel);

                // TODO: Atualizar FriendshipManager quando implementado
                // if (FriendshipManager.Instance != null)
                // {
                //     FriendshipManager.Instance.UpdateFriendship(friendshipData.speciesName, currentLevel);
                // }

                // Disparar evento
                OnFriendshipLevelChanged?.Invoke(currentLevel);
            }
        }

        /// <summary>
        /// Retorna o nível de amizade atual.
        /// </summary>
        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        /// <summary>
        /// Retorna o título do nível de amizade atual.
        /// </summary>
        public string GetCurrentLevelTitle()
        {
            if (friendshipData == null || friendshipData.levels.Count == 0)
            {
                return "Desconhecido";
            }

            FriendshipLevel levelData = friendshipData.levels.Find(l => l.level == currentLevel);
            return levelData != null ? levelData.title : "Desconhecido";
        }

        /// <summary>
        /// Retorna a descrição do nível de amizade atual.
        /// </summary>
        public string GetCurrentLevelDescription()
        {
            if (friendshipData == null || friendshipData.levels.Count == 0)
            {
                return "";
            }

            FriendshipLevel levelData = friendshipData.levels.Find(l => l.level == currentLevel);
            return levelData != null ? levelData.description : "";
        }

        /// <summary>
        /// Verifica se o nível de amizade está no máximo.
        /// </summary>
        public bool IsMaxLevel()
        {
            return friendshipData != null && currentLevel >= friendshipData.maxLevel;
        }

        /// <summary>
        /// Exibe informações sobre um nível de amizade específico no console.
        /// </summary>
        private void DisplayLevelInfo(int level)
        {
            if (friendshipData == null || friendshipData.levels.Count == 0)
            {
                return;
            }

            FriendshipLevel levelData = friendshipData.levels.Find(l => l.level == level);
            if (levelData != null)
            {
                Debug.Log($"📊 Nível {level}: {levelData.title}");
                Debug.Log($"   {levelData.description}");
                
                if (levelData.unlockedBenefits.Count > 0)
                {
                    Debug.Log($"   Benefícios desbloqueados:");
                    foreach (string benefit in levelData.unlockedBenefits)
                    {
                        Debug.Log($"   • {benefit}");
                    }
                }
            }
        }
    }
}
