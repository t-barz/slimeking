using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheSlimeKing.Core.Dialogue
{
    /// <summary>
    /// Representa uma linha de diálogo individual no sistema de diálogos.
    /// </summary>
    [Serializable]
    public class DialogueLine
    {
        [Tooltip("Chave de localização para o texto deste diálogo")]
        public string TextKey;

        [Tooltip("Chave de localização para o nome do falante (pode ser vazio)")]
        public string SpeakerNameKey;

        [Tooltip("Imagem do falante a ser exibida (pode ser nula)")]
        public Sprite SpeakerSprite;
    }

    /// <summary>
    /// ScriptableObject que contém um conjunto de linhas de diálogo.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "The Slime King/Dialogue/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Tooltip("Lista de linhas de diálogo a serem apresentadas em sequência")]
        public List<DialogueLine> Lines = new List<DialogueLine>();

        [Tooltip("ID único para este diálogo (usado para condições e rastreamento)")]
        public string DialogueID;

        [Tooltip("Indica se este diálogo já foi mostrado ao jogador")]
        public bool HasBeenShown { get; set; }
    }
}
