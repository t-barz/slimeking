using System;
using UnityEngine;

namespace SlimeKing.Core
{
    /// <summary>
    /// Dados de configuração para clips de áudio
    /// </summary>
    [Serializable]
    public class AudioClipData
    {
        [Header("Audio Clip")]
        public string name;
        public AudioClip clip;

        [Header("Settings")]
        public AudioCategory category = AudioCategory.SFX;
        [Range(0f, 1f)] public float defaultVolume = 1f;
        [Range(0.1f, 3f)] public float defaultPitch = 1f;
        public bool loop = false;

        public AudioClipData(string name, AudioClip clip)
        {
            this.name = name;
            this.clip = clip;
        }
    }

    /// <summary>
    /// Referência para carregamento sob demanda de clips de áudio
    /// </summary>
    [Serializable]
    public class AudioClipReference
    {
        [Header("Audio Reference")]
        public string name;
        public string resourcePath; // Caminho no Resources (ex: "Audio/Music/TitleTheme")

        [Header("Settings")]
        public AudioCategory category = AudioCategory.SFX;
        [Range(0f, 1f)] public float defaultVolume = 1f;
        [Range(0.1f, 3f)] public float defaultPitch = 1f;
        public bool loop = false;

        public AudioClipReference(string name, string resourcePath)
        {
            this.name = name;
            this.resourcePath = resourcePath;
        }
    }
}