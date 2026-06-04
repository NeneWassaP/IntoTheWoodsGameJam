using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public enum DialogueType
{
    Player1,
    Player2,
    Narrative
}

[System.Serializable]
public struct DialogueLine
{
    public DialogueType lineType;
    public string name;
    [TextArea(2, 5)] public string text;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image portraitPlayer1;
    [SerializeField] private Image portraitPlayer2;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText; // For character talking
    [SerializeField] private TextMeshProUGUI narrativeText; // --- ADDED THIS: For story text ---

    [Header("References to Freeze")]
    [SerializeField] private PlayerMovement player1;
    [SerializeField] private PlayerMovement player2;
    [SerializeField] private PlayerSwitcher switcher;

    [Header("Cutscene Dialogue Lines")]
    [SerializeField] private List<DialogueLine> lines;

    private int currentIndex = 0;
    private bool inCutscene = false;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        //Invoke("StartCutscene", 2f); // Temporary test trigger
    }

    public void StartCutscene()
    {
        inCutscene = true;
        currentIndex = 0;

        player1.canControl = false;
        player2.canControl = false;
        if (switcher != null) switcher.enabled = false;

        dialoguePanel.SetActive(true);
        DisplayLine();
    }

    private void Update()
    {
        if (inCutscene && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            currentIndex++;
            if (currentIndex < lines.Count)
            {
                DisplayLine();
            }
            else
            {
                EndCutscene();
            }
        }
    }

    private void DisplayLine()
    {
        DialogueLine currentLine = lines[currentIndex];

        // Default state: Assume someone is talking, turn narrative text off
        nameText.gameObject.SetActive(true);
        dialogueText.gameObject.SetActive(true);
        narrativeText.gameObject.SetActive(false);

        Color dimColor = new Color(0.3f, 0.3f, 0.3f);

        switch (currentLine.lineType)
        {
            case DialogueType.Player1:
                nameText.text = currentLine.name;
                dialogueText.text = currentLine.text;

                portraitPlayer1.color = Color.white;
                portraitPlayer2.color = dimColor;
                break;

            case DialogueType.Player2:
                nameText.text = currentLine.name;
                dialogueText.text = currentLine.text;

                portraitPlayer1.color = dimColor;
                portraitPlayer2.color = Color.white;
                break;

            case DialogueType.Narrative:
                // Hide normal dialogue elements
                nameText.gameObject.SetActive(false);
                dialogueText.gameObject.SetActive(false);

                // Show narrative text and assign the text string to it
                narrativeText.gameObject.SetActive(true);
                narrativeText.text = currentLine.text;

                // Dim both character portraits out
                portraitPlayer1.color = dimColor;
                portraitPlayer2.color = dimColor;
                break;
        }
    }

    private void EndCutscene()
    {
        inCutscene = false;
        dialoguePanel.SetActive(false);

        if (switcher != null)
        {
            switcher.enabled = true;
            switcher.SetDefaultState();
        }
    }
}