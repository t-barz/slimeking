using UnityEngine;
using SlimeKing.Core;
using SlimeKing.Events;

namespace SlimeKing.Testing
{
    /// <summary>
    /// Script de teste para verificar funcionalidades do GameManager.
    /// Anexar a um GameObject na cena para testar o sistema.
    /// </summary>
    public class GameManagerTester : MonoBehaviour
    {
        [Header("Test Controls")]
        [SerializeField] private bool enableTestLogs = true;

        private void OnEnable()
        {
            // Inscrever nos eventos para teste
            GameEvents.OnGameStateChanged += HandleGameStateChanged;
            GameEvents.OnSlimeEvolved += HandleSlimeEvolution;
            GameEvents.OnWeatherChanged += HandleWeatherChange;
            GameEvents.OnTimeAdvanced += HandleTimeAdvanced;
            GameEvents.OnNewDay += HandleNewDay;
        }

        private void OnDisable()
        {
            // Desinscrever dos eventos
            GameEvents.OnGameStateChanged -= HandleGameStateChanged;
            GameEvents.OnSlimeEvolved -= HandleSlimeEvolution;
            GameEvents.OnWeatherChanged -= HandleWeatherChange;
            GameEvents.OnTimeAdvanced -= HandleTimeAdvanced;
            GameEvents.OnNewDay -= HandleNewDay;
        }

        private void Update()
        {
            // Testes via teclado
            if (Input.GetKeyDown(KeyCode.P))
            {
                TestPauseResume();
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                TestAddXP();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                TestWeatherChange();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                TestTimeAdvance();
            }
        }

        private void TestPauseResume()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.TogglePause();
                Log("Toggled pause state");
            }
        }

        private void TestAddXP()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.AddElementalXP(ElementType.Fire, 25);
                Log("Added 25 Fire XP");
            }
        }

        private void TestWeatherChange()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.SetWeather(WeatherType.Rain);
                Log("Changed weather to Rain");
            }
        }

        private void TestTimeAdvance()
        {
            if (GameManager.HasInstance)
            {
                GameManager.Instance.AdvanceTime(1f);
                Log("Advanced time by 1 hour");
            }
        }

        #region Event Handlers

        private void HandleGameStateChanged(GameState newState)
        {
            Log($"Game state changed to: {newState}");
        }

        private void HandleSlimeEvolution(SlimeStage newStage)
        {
            Log($"Slime evolved to: {newStage}!");
        }

        private void HandleWeatherChange(WeatherType newWeather)
        {
            Log($"Weather changed to: {newWeather}");
        }

        private void HandleTimeAdvanced(float currentTime)
        {
            int hours = Mathf.FloorToInt(currentTime);
            int minutes = Mathf.FloorToInt((currentTime - hours) * 60);
            Log($"Time advanced to: {hours:00}:{minutes:00}");
        }

        private void HandleNewDay(int dayNumber)
        {
            Log($"New day started: Day {dayNumber}");
        }

        #endregion

        private void Log(string message)
        {
            if (enableTestLogs)
            {
                Debug.Log($"[GameManagerTester] {message}");
            }
        }

        private void OnGUI()
        {
            if (!GameManager.HasInstance) return;

            GUILayout.BeginArea(new Rect(Screen.width - 250, 10, 240, 150));
            GUILayout.Box("GameManager Test Controls");

            GUILayout.Label("Press P - Pause/Resume");
            GUILayout.Label("Press X - Add Fire XP");
            GUILayout.Label("Press W - Change Weather");
            GUILayout.Label("Press T - Advance Time");

            GUILayout.Space(10);
            GUILayout.Label($"Current State: {GameManager.Instance.CurrentState}");
            GUILayout.Label($"Slime Stage: {GameManager.Instance.CurrentSlimeStage}");
            GUILayout.Label($"Total XP: {GameManager.Instance.GetTotalElementalXP()}");

            GUILayout.EndArea();
        }
    }
}