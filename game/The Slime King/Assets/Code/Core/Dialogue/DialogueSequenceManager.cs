using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TheSlimeKing.Core;

namespace TheSlimeKing.Core.Dialogue
{
    /// <summary>
    /// Gerencia sequências de diálogo com múltiplas escolhas e ramificações.
    /// </summary>
    public class DialogueSequenceManager : MonoBehaviour
    {
        [Header("Componentes de Interface")]
        [SerializeField] private RectTransform _choicesPanel;
        [SerializeField] private Button _choiceButtonPrefab;

        [Header("Configurações")]
        [SerializeField] private int _maxChoices = 3;

        private List<Button> _choiceButtons = new List<Button>();
        private DialogueData _currentDialogue;
        private Dictionary<int, DialogueData> _nextDialogues = new Dictionary<int, DialogueData>();

        private void Awake()
        {
            // Inicializa os botões de escolha
            for (int i = 0; i < _maxChoices; i++)
            {
                Button newButton = Instantiate(_choiceButtonPrefab, _choicesPanel);
                newButton.gameObject.SetActive(false);
                _choiceButtons.Add(newButton);
            }

            // Desativa o painel de escolhas inicialmente
            _choicesPanel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            DialogueManager.Instance.OnDialogueEnd += HandleDialogueEnd;
        }

        private void OnDisable()
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.OnDialogueEnd -= HandleDialogueEnd;
            }
        }

        /// <summary>
        /// Inicia uma sequência de diálogo com potenciais escolhas.
        /// </summary>
        /// <param name="initialDialogue">O diálogo inicial a ser apresentado</param>
        /// <param name="choices">Dicionário mapeando índices de escolha para os diálogos subsequentes</param>
        public void StartDialogueSequence(DialogueData initialDialogue, Dictionary<int, DialogueData> choices = null)
        {
            _currentDialogue = initialDialogue;

            if (choices != null)
            {
                _nextDialogues = choices;
            }
            else
            {
                _nextDialogues.Clear();
            }

            DialogueManager.Instance.StartDialogue(initialDialogue);
        }

        /// <summary>
        /// Manipula o evento de fim de diálogo.
        /// </summary>
        private void HandleDialogueEnd()
        {
            // Se houver escolhas, mostra os botões de escolha
            if (_nextDialogues.Count > 0)
            {
                ShowChoices();
            }
        }

        /// <summary>
        /// Exibe os botões de escolha para o jogador.
        /// </summary>
        private void ShowChoices()
        {
            // Limpa os botões anteriores
            foreach (Button button in _choiceButtons)
            {
                button.gameObject.SetActive(false);
            }

            // Configura e exibe os botões para as escolhas disponíveis
            int choiceIndex = 0;
            foreach (var choice in _nextDialogues)
            {
                if (choiceIndex >= _choiceButtons.Count) break;

                Button choiceButton = _choiceButtons[choiceIndex];
                choiceButton.gameObject.SetActive(true);

                // Configura o texto do botão usando a primeira linha do diálogo da escolha
                string choiceText = choice.Value.Lines.Count > 0 ?
                    choice.Value.Lines[0].TextKey :
                    $"Choice {choiceIndex + 1}";

                // Obtém o texto localizado
                choiceText = LocalizationManager.Instance.GetLocalizedText(choiceText);

                // Define o texto do botão
                TextMeshProUGUI buttonText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = choiceText;
                }

                // Configura o evento do botão
                int capturedIndex = choice.Key; // Captura o valor para o closure
                choiceButton.onClick.RemoveAllListeners();
                choiceButton.onClick.AddListener(() => OnChoiceSelected(capturedIndex));

                choiceIndex++;
            }

            // Ativa o painel de escolhas
            _choicesPanel.gameObject.SetActive(true);
        }

        /// <summary>
        /// Processa a seleção de uma escolha pelo jogador.
        /// </summary>
        /// <param name="choiceIndex">Índice da escolha selecionada</param>
        private void OnChoiceSelected(int choiceIndex)
        {
            // Desativa o painel de escolhas
            _choicesPanel.gameObject.SetActive(false);

            // Obtém o próximo diálogo baseado na escolha
            if (_nextDialogues.TryGetValue(choiceIndex, out DialogueData nextDialogue))
            {
                // Inicia o próximo diálogo
                _currentDialogue = nextDialogue;
                _nextDialogues.Clear();

                DialogueManager.Instance.StartDialogue(nextDialogue);
            }
        }
    }
}
