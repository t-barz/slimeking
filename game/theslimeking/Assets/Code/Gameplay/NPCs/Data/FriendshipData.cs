using System.Collections.Generic;
using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Representa um nível de amizade com benefícios desbloqueados.
    /// </summary>
    [System.Serializable]
    public class FriendshipLevel
    {
        [Tooltip("Número do nível (0-5)")]
        public int level;

        [Tooltip("Título do nível (ex: Desconhecido, Amigo, Companheiro Leal)")]
        public string title;

        [Tooltip("Descrição do que este nível representa")]
        [TextArea(2, 4)]
        public string description;

        [Tooltip("Lista de benefícios desbloqueados neste nível")]
        public List<string> unlockedBenefits = new List<string>();
    }

    /// <summary>
    /// ScriptableObject que armazena dados do sistema de amizade para uma espécie de NPC.
    /// Define os níveis de amizade e benefícios desbloqueados.
    /// </summary>
    [CreateAssetMenu(fileName = "FriendshipData", menuName = "Game/Friendship Data")]
    public class FriendshipData : ScriptableObject
    {
        [Header("Species Information")]
        [Tooltip("Nome da espécie (ex: Cervo, Esquilo, Abelha)")]
        public string speciesName;

        [Tooltip("Nível máximo de amizade alcançável")]
        public int maxLevel = 5;

        [Header("Friendship Levels")]
        [Tooltip("Lista de níveis de amizade com títulos e benefícios")]
        public List<FriendshipLevel> levels = new List<FriendshipLevel>();
    }
}
