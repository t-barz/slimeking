using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlimeKing.Core.Cutscenes
{
    /// <summary>
    /// Define uma sequência de eventos que compõem uma cutscene
    /// </summary>
    [CreateAssetMenu(fileName = "New Cutscene", menuName = "SlimeKing/Cutscenes/Cutscene Definition")]
    public class CutsceneDefinition : ScriptableObject
    {
        [SerializeField] private List<CutsceneEvent> events = new List<CutsceneEvent>();
        [SerializeField] private bool disablePlayerControl = true;
        [SerializeField] private bool canBeSkipped = true;

        public bool DisablePlayerControl => disablePlayerControl;
        public bool CanBeSkipped => canBeSkipped;
        public IReadOnlyList<CutsceneEvent> Events => events;
    }
}
