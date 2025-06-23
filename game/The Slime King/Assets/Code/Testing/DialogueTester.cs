using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Core.Dialogue;
using System.Collections.Generic;

namespace TheSlimeKing.Testing
{
    /// <summary>
    /// Ferramenta de teste para o sistema de diálogos.
    /// Permite testar diálogos no editor sem precisar criar objetos na cena.
    /// </summary>
    public class DialogueTester : MonoBehaviour
    {
        [Header("Diálogos de Teste")]
        [SerializeField] private List<DialogueData> _testDialogues = new List<DialogueData>();

        [Header("Interface")]
        [SerializeField] private Button _testButton;
        [SerializeField] private TMP_Dropdown _dialogueDropdown;

        private DialogueManager _dialogueManager;

        private void Start()
        {
            // Busca o DialogueManager
            _dialogueManager = DialogueManager.Instance;
            if (_dialogueManager == null)
            {
                Debug.LogError("DialogueTester requer DialogueManager na cena.");
                enabled = false;
                return;
            }

            // Configura o dropdown de diálogos
            ConfigureDialogueDropdown();

            // Configura o botão de teste
            if (_testButton != null)
            {
                _testButton.onClick.AddListener(TestSelectedDialogue);
            }
        }

        /// <summary>
        /// Configura o dropdown com os diálogos disponíveis.
        /// </summary>
        private void ConfigureDialogueDropdown()
        {
            if (_dialogueDropdown == null) return;

            _dialogueDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            foreach (var dialogue in _testDialogues)
            {
                if (dialogue != null)
                {
                    string name = string.IsNullOrEmpty(dialogue.DialogueID) ?
                        dialogue.name : dialogue.DialogueID;
                    options.Add(new TMP_Dropdown.OptionData(name));
                }
            }

            _dialogueDropdown.AddOptions(options);
        }

        /// <summary>
        /// Inicia o teste do diálogo selecionado no dropdown.
        /// </summary>
        public void TestSelectedDialogue()
        {
            if (_dialogueManager == null || _dialogueDropdown == null) return;

            int selectedIndex = _dialogueDropdown.value;
            if (selectedIndex >= 0 && selectedIndex < _testDialogues.Count)
            {
                DialogueData selectedDialogue = _testDialogues[selectedIndex];
                if (selectedDialogue != null)
                {
                    // Inicia o diálogo selecionado
                    _dialogueManager.StartDialogue(selectedDialogue, OnTestDialogueCompleted);
                }
            }
        }

        /// <summary>
        /// Testa um diálogo específico pelo índice.
        /// </summary>
        /// <param name="index">Índice do diálogo na lista</param>
        public void TestDialogueByIndex(int index)
        {
            if (_dialogueManager == null) return;

            if (index >= 0 && index < _testDialogues.Count)
            {
                DialogueData dialogue = _testDialogues[index];
                if (dialogue != null)
                {
                    _dialogueManager.StartDialogue(dialogue, OnTestDialogueCompleted);
                }
            }
        }

        /// <summary>
        /// Cria e testa um diálogo simples de uma linha.
        /// Útil para testar mensagens de tutorial ou informações.
        /// </summary>
        /// <param name="textKey">Chave de localização para o texto</param>
        /// <param name="speakerKey">Chave de localização para o falante (opcional)</param>
        public void TestSimpleDialogue(string textKey, string speakerKey = "")
        {
            if (_dialogueManager == null) return;

            _dialogueManager.StartSimpleDialogue(textKey, speakerKey, OnTestDialogueCompleted);
        }

        /// <summary>
        /// Callback executado quando o diálogo de teste é concluído.
        /// </summary>
        private void OnTestDialogueCompleted()
        {
            Debug.Log("Diálogo de teste concluído com sucesso.");
        }
    }
}
