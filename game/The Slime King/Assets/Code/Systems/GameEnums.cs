using System;
using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// Estados possíveis do jogo - The Slime King
    /// </summary>
    public enum GameState
    {
        // === ESTADOS INICIAIS ===
        Splash,         // Tela de splash (logos, loading inicial)
        MainMenu,       // Menu principal/tela inicial
        Options,        // Menu de configurações/opções
        Credits,        // Tela de créditos

        // === ESTADOS DE JOGO ===
        Loading,        // Carregando bioma/área de jogo
        Exploring,      // Explorando mundo (estado principal)
        Interacting,    // Dialogando com NPCs/criaturas

        // === ESTADOS DE INTERFACE ===
        Paused,         // Jogo pausado (menu pause)
        Inventory,      // Menu de inventário aberto
        SkillTree,      // Árvore de habilidades aberta

        // === ESTADOS ESPECIAIS ===
        Death,          // Slime foi derrotado
        Evolution,      // Processo de evolução do Slime
        Victory         // Área/objetivo completado
    }

    /// <summary>
    /// Estágios evolutivos do Slime
    /// </summary>
    public enum SlimeStage
    {
        Filhote,        // Estado inicial
        Adulto,         // Habilidades básicas
        GrandeSlime,    // Combinações avançadas
        ReiSlime        // Maestria completa
    }

    /// <summary>
    /// Tipos de elementos dos cristais
    /// </summary>
    public enum ElementType
    {
        Fire,       // Fogo - Câmaras de Lava
        Water,      // Água - Lago Espelhado
        Earth,      // Terra - Área Rochosa
        Air,        // Ar - Pico Nevado
        Nature,     // Natureza - Floresta Calma
        Shadow      // Sombra - Pântano das Névoas
    }

    /// <summary>
    /// Configurações do jogo
    /// </summary>
    [System.Serializable]
    public class GameSettings
    {
        [Header("Audio")]
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;

        [Header("Graphics")]
        public bool fullscreen = true;
        public int resolutionIndex = 0;
        public bool vSync = true;

        [Header("Gameplay")]
        public bool showTutorials = true;
        public bool showDamageNumbers = true;
        public float uiScale = 1f;

        [Header("Controls")]
        public float mouseSensitivity = 1f;
        public bool invertYAxis = false;

        [Header("Accessibility")]
        public bool subtitles = false;
        public bool colorBlindMode = false;
        public float textSize = 1f;
    }
}