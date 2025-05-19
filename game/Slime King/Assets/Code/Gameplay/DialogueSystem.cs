using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

/// <summary>
/// Controla a exibição de diálogos na UI
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [Header("Configurações de UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    
    [Tooltip("Velocidade da animação de digitação")]
    [SerializeField] private float typingSpeed = 0.05f;

    private bool isTyping = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            dialoguePanel.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Inicia nova sequência de diálogos
    /// </summary>
    public void StartDialogue(string[] dialogueLines)
    {
        StartCoroutine(RunDialogue(dialogueLines));
    }

    private IEnumerator RunDialogue(string[] lines)
    {
        dialoguePanel.SetActive(true);
        
        foreach (string line in lines)
        {
            isTyping = true;
            dialogueText.text = "";
            
            foreach (char letter in line.ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(typingSpeed);
            }
            
            isTyping = false;
            yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);
        }
        
        dialoguePanel.SetActive(false);
    }
}
