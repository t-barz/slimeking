using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SlimeKing.Core.Cutscenes
{
    /// <summary>
    /// Base class for all cutscene events
    /// </summary>
    public abstract class CutsceneEvent : ScriptableObject
    {
        [SerializeField] private bool canBeSkipped = true;
        [SerializeField] private float duration = 1f;

        public bool CanBeSkipped => canBeSkipped;
        public float Duration => duration;

        /// <summary>
        /// Execute the event and return when it's complete
        /// </summary>
        public abstract Task Execute(CutsceneContext context);

        /// <summary>
        /// Forcefully skip/stop the event
        /// </summary>
        public abstract void Skip();
    }

    /// <summary>
    /// Contains shared data and references needed during cutscene execution
    /// </summary>
    public class CutsceneContext
    {
        public GameObject Player { get; set; }
        public Camera MainCamera { get; set; }
        public VignetteController Vignette { get; set; }
        public Transform CutsceneRoot { get; set; }
    }
}
