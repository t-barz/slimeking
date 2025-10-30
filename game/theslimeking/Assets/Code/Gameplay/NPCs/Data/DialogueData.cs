using System.Collections.Generic;
using UnityEngine;

namespace SlimeMec.Gameplay
{
    /// <summary>
    /// Representa uma escolha de diálogo que o jogador pode fazer.
    /// </summary>
    [System.Serializable]
    public class DialogueChoice
    {
        [Tooltip("Texto da escolha exibido ao jogador")]
        public string choiceText;

        [Tooltip("ID do próximo nó de diálogo (vazio se termina o diálogo)")]
        public string nextNodeId;

        [Tooltip("Condição necessária para exibir esta escolha (opcional)")]
        public string condition;
    }

    /// <summary>
    /// Representa um nó de diálogo com texto, retrato e escolhas.
    /// </summary>
    [System.Serializable]
    public class DialogueNode
    {
        [Tooltip("ID único deste nó de diálogo")]
        public string id;

        [Tooltip("Texto do diálogo exibido ao jogador")]
        [TextArea(3, 6)]
        public string text;

        [Tooltip("Sprite do retrato do NPC (opcional)")]
        public Sprite portrait;

        [Tooltip("Lista de escolhas disponíveis para o jogador")]
        public List<DialogueChoice> choices = new List<DialogueChoice>();
    }

    /// <summary>
    /// ScriptableObject que armazena dados de diálogo para um NPC.
    /// Define os nós de diálogo, texto e escolhas do jogador.
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueData", menuName = "Game/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Header("NPC Information")]
        [Tooltip("Nome do NPC que possui este diálogo")]
        public string npcName;

        [Header("Dialogue Tree")]
        [Tooltip("Lista de nós de diálogo que formam a árvore de conversação")]
        public List<DialogueNode> dialogueNodes = new List<DialogueNode>();
    }
}
