using UnityEngine;
using TheSlimeKing.Gameplay.Movement;
using UnityEngine.InputSystem;

namespace TheSlimeKing.Testing
{
    /// <summary>
    /// Script para testar o Sistema de Movimento Especial
    /// </summary>
    public class SpecialMovementTester : MonoBehaviour
    {
        [Header("Referências")]
        [SerializeField] private EncolherEsgueirarController encolherTester;
        [SerializeField] private JumpController jumpTester;
        [SerializeField] private Transform playerTransform;

        [Header("Posições de Teste")]
        [SerializeField] private Vector2 encolherEntryPosition;
        [SerializeField] private Vector2 encolherExitPosition;
        [SerializeField] private Vector2 jumpStartPosition;
        [SerializeField] private Vector2 jumpLandPosition;

        [Header("Configurações de Teste")]
        [SerializeField] private KeyCode createEncolherTestKey = KeyCode.F1;
        [SerializeField] private KeyCode createJumpTestKey = KeyCode.F2;
        [SerializeField] private KeyCode teleportPlayerNearbyKey = KeyCode.F3;
        [SerializeField] private KeyCode removeTestObjectsKey = KeyCode.F4;

        // Objetos de teste criados
        private GameObject _createdEncolherObject;
        private GameObject _createdJumpObject;

        private void Update()
        {
            // Criar teste de Encolher e Esgueirar
            if (Input.GetKeyDown(createEncolherTestKey))
            {
                RemoveTestObjects();
                CreateEncolherTest();
            }

            // Criar teste de Pulo
            if (Input.GetKeyDown(createJumpTestKey))
            {
                RemoveTestObjects();
                CreateJumpTest();
            }

            // Teleportar jogador para perto do objeto de teste
            if (Input.GetKeyDown(teleportPlayerNearbyKey))
            {
                TeleportPlayerNearby();
            }

            // Remover objetos de teste
            if (Input.GetKeyDown(removeTestObjectsKey))
            {
                RemoveTestObjects();
            }
        }

        private void CreateEncolherTest()
        {
            if (encolherTester == null)
            {
                Debug.LogError("EncolherEsgueirarController de referência não está configurado!");
                return;
            }

            // Cria objeto de entrada
            _createdEncolherObject = Instantiate(encolherTester.gameObject, encolherEntryPosition, Quaternion.identity);

            // Cria ponto de saída
            GameObject exitPoint = new GameObject("ExitPoint");
            exitPoint.transform.position = encolherExitPosition;
            exitPoint.transform.parent = _createdEncolherObject.transform;

            // Configura o controlador
            EncolherEsgueirarController controller = _createdEncolherObject.GetComponent<EncolherEsgueirarController>();

            // Usa reflexão para definir o exitPoint privado
            System.Reflection.FieldInfo fieldInfo = typeof(EncolherEsgueirarController).GetField("exitPoint",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(controller, exitPoint.transform);
            }
            else
            {
                Debug.LogError("Não foi possível acessar o campo exitPoint via reflexão.");
            }

            Debug.Log("Ponto de Encolher criado! Use F3 para teleportar o jogador para perto.");
        }

        private void CreateJumpTest()
        {
            if (jumpTester == null)
            {
                Debug.LogError("JumpController de referência não está configurado!");
                return;
            }

            // Cria objeto de decolagem
            _createdJumpObject = Instantiate(jumpTester.gameObject, jumpStartPosition, Quaternion.identity);

            // Cria ponto de pouso
            GameObject landingPoint = new GameObject("LandingPoint");
            landingPoint.transform.position = jumpLandPosition;
            landingPoint.transform.parent = _createdJumpObject.transform;

            // Configura o controlador
            JumpController controller = _createdJumpObject.GetComponent<JumpController>();

            // Usa reflexão para definir o landingPoint privado
            System.Reflection.FieldInfo fieldInfo = typeof(JumpController).GetField("landingPoint",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(controller, landingPoint.transform);
            }
            else
            {
                Debug.LogError("Não foi possível acessar o campo landingPoint via reflexão.");
            }

            Debug.Log("Ponto de Pulo criado! Use F3 para teleportar o jogador para perto.");
        }

        private void TeleportPlayerNearby()
        {
            if (playerTransform == null)
            {
                Debug.LogError("Transform do jogador não configurado!");
                return;
            }

            Vector3 targetPos;

            if (_createdEncolherObject != null)
            {
                targetPos = _createdEncolherObject.transform.position + new Vector3(-1.5f, 0, 0);
                playerTransform.position = targetPos;
            }
            else if (_createdJumpObject != null)
            {
                targetPos = _createdJumpObject.transform.position + new Vector3(-1.5f, 0, 0);
                playerTransform.position = targetPos;
            }
            else
            {
                Debug.Log("Nenhum objeto de teste criado para teleportar o jogador próximo.");
            }
        }

        private void RemoveTestObjects()
        {
            if (_createdEncolherObject != null)
                Destroy(_createdEncolherObject);

            if (_createdJumpObject != null)
                Destroy(_createdJumpObject);

            _createdEncolherObject = null;
            _createdJumpObject = null;
        }
    }
}
