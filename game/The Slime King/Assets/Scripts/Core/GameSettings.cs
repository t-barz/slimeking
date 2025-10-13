using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Configurações do jogo
    /// </summary>
    [Serializable]
    public class GameSettings
    {
        [Header("Audio Settings")]
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public float uiVolume = 1f;
        public float ambientVolume = 0.6f;
        
        [Header("Graphics Settings")]
        public int resolutionIndex = 0;
        public bool fullscreen = true;
        public int qualityLevel = 2; // Medium quality
        public bool vsync = true;
        
        [Header("Gameplay Settings")]
        public float mouseSensitivity = 1f;
        public bool showFPS = false;
        public bool tutorialCompleted = false;
        
        [Header("Input Settings")]
        public Dictionary<string, string> keyBindings = new Dictionary<string, string>();
        
        public GameSettings()
        {
            // Key bindings padrão
            SetDefaultKeyBindings();
        }
        
        /// <summary>
        /// Define key bindings padrão
        /// </summary>
        private void SetDefaultKeyBindings()
        {
            keyBindings["Move"] = "WASD";
            keyBindings["Jump"] = "Space";
            keyBindings["Attack"] = "LeftClick";
            keyBindings["Pause"] = "Escape";
        }
    }
}