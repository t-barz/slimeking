using System;
using UnityEngine;

namespace ExtraTools
{
    /// <summary>
    /// Eventos globais disparados pelo GameManager
    /// </summary>
    public static class GameManagerEvents
    {
        // === ESTADOS DO JOGO ===
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnGameStarted;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnPlayerDeath;
        public static event Action OnPlayerRespawn;
        public static event Action OnReturnToMainMenu;

        // === PROGRESSÃO DO SLIME ===
        public static event Action<SlimeStage> OnSlimeEvolved;
        public static event Action<int> OnLivesChanged;
        public static event Action<float> OnTimeChanged;
        public static event Action<string> OnBiomeChanged;

        // === SISTEMA DE CRISTAIS ===
        public static event Action<ElementType, int> OnCrystalFragmentsChanged;
        public static event Action<ElementType> OnNewElementUnlocked;

        // === AMIZADE E LAR ===
        public static event Action<string, int> OnFriendshipChanged;
        public static event Action<string> OnHomeExpansionUnlocked;

        // === CONFIGURAÇÕES ===
        public static event Action<GameSettings> OnSettingsChanged;

        // Métodos de disparo (encapsulam Invoke para evitar erros CS0070)
        public static void RaiseGameStateChanged(GameState state) => OnGameStateChanged?.Invoke(state);
        public static void RaiseGameStarted() => OnGameStarted?.Invoke();
        public static void RaiseGamePaused() => OnGamePaused?.Invoke();
        public static void RaiseGameResumed() => OnGameResumed?.Invoke();
        public static void RaisePlayerDeath() => OnPlayerDeath?.Invoke();
        public static void RaisePlayerRespawn() => OnPlayerRespawn?.Invoke();
        public static void RaiseReturnToMainMenu() => OnReturnToMainMenu?.Invoke();
        public static void RaiseSlimeEvolved(SlimeStage stage) => OnSlimeEvolved?.Invoke(stage);
        public static void RaiseLivesChanged(int lives) => OnLivesChanged?.Invoke(lives);
        public static void RaiseTimeChanged(float time) => OnTimeChanged?.Invoke(time);
        public static void RaiseBiomeChanged(string biome) => OnBiomeChanged?.Invoke(biome);
        public static void RaiseCrystalFragmentsChanged(ElementType element, int amount) => OnCrystalFragmentsChanged?.Invoke(element, amount);
        public static void RaiseNewElementUnlocked(ElementType element) => OnNewElementUnlocked?.Invoke(element);
        public static void RaiseFriendshipChanged(string creature, int level) => OnFriendshipChanged?.Invoke(creature, level);
        public static void RaiseHomeExpansionUnlocked(string expansion) => OnHomeExpansionUnlocked?.Invoke(expansion);
        public static void RaiseSettingsChanged(GameSettings settings) => OnSettingsChanged?.Invoke(settings);
    }

    /// <summary>
    /// Eventos relacionados ao Player/Slime
    /// </summary>
    public static class PlayerEvents
    {
        // Eventos que o Player dispara
        public static event Action OnPlayerDeath;
        public static event Action OnPlayerRespawn;
        public static event Action<Vector3> OnPlayerMove;
        public static event Action OnPlayerJump;
        public static event Action<float> OnPlayerTakeDamage;

        // Eventos que o Player escuta
        public static event Action OnRespawnRequested;

        // Raise helpers
        public static void RaisePlayerDeath() => OnPlayerDeath?.Invoke();
        public static void RaisePlayerRespawn() => OnPlayerRespawn?.Invoke();
        public static void RaisePlayerMove(Vector3 dir) => OnPlayerMove?.Invoke(dir);
        public static void RaisePlayerJump() => OnPlayerJump?.Invoke();
        public static void RaisePlayerTakeDamage(float dmg) => OnPlayerTakeDamage?.Invoke(dmg);
        public static void RaiseRespawnRequested() => OnRespawnRequested?.Invoke();
    }

    /// <summary>
    /// Eventos relacionados ao sistema de evolução do Slime
    /// </summary>
    public static class SlimeEvents
    {
        // Absorção de cristais
        public static event Action<ElementType, int> OnCrystalAbsorbed;

        // Evolução
        public static event Action<SlimeStage> OnEvolutionTriggered;
        public static event Action<SlimeStage> OnEvolutionCompleted;

        // Habilidades
        public static event Action<string> OnNewAbilityUnlocked;
        public static event Action<ElementType> OnElementEquipped;

        // Raise helpers
        public static void RaiseCrystalAbsorbed(ElementType element, int amount) => OnCrystalAbsorbed?.Invoke(element, amount);
        public static void RaiseEvolutionTriggered(SlimeStage stage) => OnEvolutionTriggered?.Invoke(stage);
        public static void RaiseEvolutionCompleted(SlimeStage stage) => OnEvolutionCompleted?.Invoke(stage);
        public static void RaiseNewAbilityUnlocked(string ability) => OnNewAbilityUnlocked?.Invoke(ability);
        public static void RaiseElementEquipped(ElementType element) => OnElementEquipped?.Invoke(element);
    }

    /// <summary>
    /// Eventos relacionados às criaturas e sistema social
    /// </summary>
    public static class CreatureEvents
    {
        // Sistema de amizade
        public static event Action<string, int> OnFriendshipIncreased;
        public static event Action<string> OnCreatureBefriended;

        // Visitantes no lar
        public static event Action<string> OnCreatureVisit;
        public static event Action<string> OnCreatureLeaveHome;

        // Raise helpers
        public static void RaiseFriendshipIncreased(string creature, int level) => OnFriendshipIncreased?.Invoke(creature, level);
        public static void RaiseCreatureBefriended(string creature) => OnCreatureBefriended?.Invoke(creature);
        public static void RaiseCreatureVisit(string creature) => OnCreatureVisit?.Invoke(creature);
        public static void RaiseCreatureLeaveHome(string creature) => OnCreatureLeaveHome?.Invoke(creature);
    }

    /// <summary>
    /// Eventos relacionados aos biomas e transições
    /// </summary>
    public static class BiomeEvents
    {
        // Transições de bioma
        public static event Action<string> OnBiomeEntered;
        public static event Action<string> OnBiomeExited;

        // Descobertas
        public static event Action<string> OnNewAreaDiscovered;
        public static event Action<string, string> OnSeasonChanged; // biome, season

        // Raise helpers
        public static void RaiseBiomeEntered(string biome) => OnBiomeEntered?.Invoke(biome);
        public static void RaiseBiomeExited(string biome) => OnBiomeExited?.Invoke(biome);
        public static void RaiseNewAreaDiscovered(string area) => OnNewAreaDiscovered?.Invoke(area);
        public static void RaiseSeasonChanged(string biome, string season) => OnSeasonChanged?.Invoke(biome, season);
    }

    /// <summary>
    /// Eventos relacionados à interface do usuário
    /// </summary>
    public static class UIEvents
    {
        // Estados de menu
        public static event Action OnStartGameRequested;
        public static event Action OnContinueGameRequested;
        public static event Action OnOptionsRequested;
        public static event Action OnCreditsRequested;
        public static event Action OnMainMenuRequested;
        public static event Action OnQuitRequested;

        // Estados de gameplay
        public static event Action OnPauseRequested;
        public static event Action OnResumeRequested;
        public static event Action OnRestartRequested;
        public static event Action OnInventoryOpened;
        public static event Action OnInventoryClosed;
        public static event Action OnSkillTreeOpened;
        public static event Action OnSkillTreeClosed;

        // Raise helpers
        public static void RaiseStartGameRequested() => OnStartGameRequested?.Invoke();
        public static void RaiseContinueGameRequested() => OnContinueGameRequested?.Invoke();
        public static void RaiseOptionsRequested() => OnOptionsRequested?.Invoke();
        public static void RaiseCreditsRequested() => OnCreditsRequested?.Invoke();
        public static void RaiseMainMenuRequested() => OnMainMenuRequested?.Invoke();
        public static void RaiseQuitRequested() => OnQuitRequested?.Invoke();
        public static void RaisePauseRequested() => OnPauseRequested?.Invoke();
        public static void RaiseResumeRequested() => OnResumeRequested?.Invoke();
        public static void RaiseRestartRequested() => OnRestartRequested?.Invoke();
        public static void RaiseInventoryOpened() => OnInventoryOpened?.Invoke();
        public static void RaiseInventoryClosed() => OnInventoryClosed?.Invoke();
        public static void RaiseSkillTreeOpened() => OnSkillTreeOpened?.Invoke();
        public static void RaiseSkillTreeClosed() => OnSkillTreeClosed?.Invoke();
    }

    /// <summary>
    /// Eventos relacionados ao fluxo do jogo
    /// </summary>
    public static class GameFlowEvents
    {
        public static event Action<string> OnAreaCompleted;
        public static event Action<string> OnBiomeTransition;
        public static event Action<string> OnObjectiveCompleted;
        public static event Action OnGameCompleted;

        // Raise helpers
        public static void RaiseAreaCompleted(string area) => OnAreaCompleted?.Invoke(area);
        public static void RaiseBiomeTransition(string biome) => OnBiomeTransition?.Invoke(biome);
        public static void RaiseObjectiveCompleted(string objective) => OnObjectiveCompleted?.Invoke(objective);
        public static void RaiseGameCompleted() => OnGameCompleted?.Invoke();
    }
}