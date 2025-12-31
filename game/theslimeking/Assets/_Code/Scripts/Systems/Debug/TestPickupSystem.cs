using UnityEngine;
using SlimeKing.External.SlimeMec.Gameplay;
using SlimeKing.Gameplay;

namespace SlimeKing.Debug
{
    /// <summary>
    /// Script de teste para verificar o sistema de Pickup Item.
    /// Adicione ao player ou a qualquer GameObject na cena.
    /// </summary>
    public class TestPickupSystem : MonoBehaviour
    {
        [Header("Teclas de Teste")]
        [Tooltip("Tecla para testar coleta manual")]
        [SerializeField] private KeyCode testCollectKey = KeyCode.T;

        [Tooltip("Tecla para verificar estado do player")]
        [SerializeField] private KeyCode testPlayerStateKey = KeyCode.Y;

        [Tooltip("Tecla para pausar/retomar movimento manualmente")]
        [SerializeField] private KeyCode testToggleMovementKey = KeyCode.U;

        private void Update()
        {
            // Teste 1: Coletar item manualmente
            if (Input.GetKeyDown(testCollectKey))
            {
                TestManualCollect();
            }

            // Teste 2: Verificar estado do player
            if (Input.GetKeyDown(testPlayerStateKey))
            {
                TestPlayerState();
            }

            // Teste 3: Toggle movimento
            if (Input.GetKeyDown(testToggleMovementKey))
            {
                TestToggleMovement();
            }
        }

        /// <summary>
        /// Testa coleta manual do primeiro PickupItem encontrado
        /// </summary>
        private void TestManualCollect()
        {
            UnityEngine.Debug.Log("=== TESTE: Coleta Manual ===");

            PickupItem item = FindObjectOfType<PickupItem>();
            if (item == null)
            {
                UnityEngine.Debug.LogWarning("Nenhum PickupItem encontrado na cena!");
                return;
            }

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                UnityEngine.Debug.LogWarning("Player não encontrado! Certifique-se que tem a tag 'Player'");
                return;
            }

            UnityEngine.Debug.Log($"Tentando coletar: {item.gameObject.name}");
            bool success = item.TryInteract(playerObj.transform);
            UnityEngine.Debug.Log($"Resultado: {(success ? "✓ SUCESSO" : "✗ FALHOU")}");
        }

        /// <summary>
        /// Verifica e exibe o estado atual do player
        /// </summary>
        private void TestPlayerState()
        {
            UnityEngine.Debug.Log("=== TESTE: Estado do Player ===");

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                UnityEngine.Debug.LogWarning("Player não encontrado!");
                return;
            }

            PlayerController controller = playerObj.GetComponent<PlayerController>();
            if (controller == null)
            {
                UnityEngine.Debug.LogWarning("PlayerController não encontrado no player!");
                return;
            }

            bool canMove = controller.CanMove();
            UnityEngine.Debug.Log($"Player pode se mover: {(canMove ? "✓ SIM" : "✗ NÃO")}");

            InteractionHandler interactionHandler = playerObj.GetComponent<InteractionHandler>();
            if (interactionHandler != null)
            {
                bool hasInteraction = interactionHandler.HasAvailableInteraction;
                UnityEngine.Debug.Log($"Tem interação disponível: {(hasInteraction ? "✓ SIM" : "✗ NÃO")}");
                
                if (hasInteraction)
                {
                    string prompt = interactionHandler.GetCurrentInteractionPrompt();
                    UnityEngine.Debug.Log($"Prompt: {prompt}");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("InteractionHandler não encontrado no player!");
            }
        }

        /// <summary>
        /// Alterna o estado de movimento do player manualmente
        /// </summary>
        private void TestToggleMovement()
        {
            UnityEngine.Debug.Log("=== TESTE: Toggle Movimento ===");

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null)
            {
                UnityEngine.Debug.LogWarning("Player não encontrado!");
                return;
            }

            PlayerController controller = playerObj.GetComponent<PlayerController>();
            if (controller == null)
            {
                UnityEngine.Debug.LogWarning("PlayerController não encontrado!");
                return;
            }

            bool currentState = controller.CanMove();
            controller.SetCanMove(!currentState);
            
            UnityEngine.Debug.Log($"Movimento alterado: {currentState} → {!currentState}");
        }

        private void OnGUI()
        {
            // Exibe instruções na tela
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 14;
            style.alignment = TextAnchor.UpperLeft;
            style.normal.textColor = Color.white;

            string instructions = 
                "=== TESTES DO SISTEMA DE PICKUP ===\n" +
                $"[{testCollectKey}] - Coletar item mais próximo\n" +
                $"[{testPlayerStateKey}] - Ver estado do player\n" +
                $"[{testToggleMovementKey}] - Pausar/Retomar movimento\n" +
                "[E] - Interagir normalmente\n" +
                "\nVeja o Console para resultados detalhados";

            GUI.Box(new Rect(10, 10, 400, 120), instructions, style);
        }
    }
}
