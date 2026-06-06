using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public enum DialogueType
{
    BigBro,
    LittleBro,
    Deer,
    Narrative,
    FullNarrative
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
    [SerializeField] private Image portraitDeer;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI narrativeText;

    // 🌟 OPTIONAL: Leaving these empty switches the script to "In-Game Mode" automatically
    [Header("Intro Menu Setup (Optional - Leave Empty for In-Game)")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private bool playAutomaticallyOnStart = false; // Great for In-Game cutscenes

    [Header("References to Freeze (Optional)")]
    [SerializeField] private PlayerMovement player1;
    [SerializeField] private PlayerMovement player2;
    [SerializeField] private PlayerSwitcher switcher;

    [Header("Cutscene Dialogue Lines")]
    [SerializeField] private List<DialogueLine> lines;

    [Header("Level Transition Settings")]
    [SerializeField] private bool loadNextLevelOnEnd = false;

    private int currentIndex = 0;
    private bool inCutscene = false;
    private bool isTransitioning = false;

    private void Start()
    {
        dialoguePanel.SetActive(false);

        // 🌟 SMART MODE CHECK: Detect if we are in the Intro Scene or an In-Game Scene
        if (mainMenuPanel != null)
        {
            // Intro Mode: Show menu, hide fade screen
            mainMenuPanel.SetActive(true);
            if (fadeOverlay != null) fadeOverlay.alpha = 0f;
        }
        else
        {
            // In-Game Mode: If marked to play instantly on level load, fire it up!
            if (playAutomaticallyOnStart)
            {
                StartCutscene();
            }
        }
    }

    // Call this from your Start Button (Intro Scene Only)
    public void OnStartButtonClick()
    {
        if (isTransitioning) return;
        StartCoroutine(IntroSequenceRoutine());
    }

    private IEnumerator IntroSequenceRoutine()
    {
        isTransitioning = true;

        // 1. Fade to Black (Only runs if a fade overlay object is assigned)
        if (fadeOverlay != null)
        {
            float duration = 1.0f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
        }

        // 2. Hide the main menu panel
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        // 3. Keep the screen black for 2 seconds
        yield return new WaitForSeconds(2.0f);

        // 4. Start the dialogue text
        StartCutscene();

        // 5. Fade the black overlay back out
        if (fadeOverlay != null)
        {
            float duration = 1.0f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(1f - (elapsed / duration));
                yield return null;
            }
        }

        isTransitioning = false;
    }

    // 🌟 IN-GAME TRIGGERING: You can still call this from external trigger zones or interaction buttons!
    public void StartCutscene()
    {
        if (lines.Count == 0) return;

        inCutscene = true;
        currentIndex = 0;

        // Freeze controls safely (won't error if players aren't in the scene)
        if (player1 != null) player1.canControl = false;
        if (player2 != null) player2.canControl = false;
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

        if (nameText != null) nameText.gameObject.SetActive(true);
        if (dialogueText != null) dialogueText.gameObject.SetActive(true);
        if (narrativeText != null) narrativeText.gameObject.SetActive(false);

        if (portraitPlayer1 != null) portraitPlayer1.gameObject.SetActive(true);
        if (portraitPlayer2 != null) portraitPlayer2.gameObject.SetActive(true);
        if (portraitDeer != null) portraitDeer.gameObject.SetActive(true);

        Color dimColor = new Color(0.3f, 0.3f, 0.3f);

        switch (currentLine.lineType)
        {
            case DialogueType.BigBro:
                if (nameText != null) nameText.text = currentLine.name;
                if (dialogueText != null) dialogueText.text = currentLine.text;
                if (portraitPlayer1 != null) portraitPlayer1.color = Color.white;
                if (portraitPlayer2 != null) portraitPlayer2.color = dimColor;
                if (portraitDeer != null) portraitDeer.gameObject.SetActive(false);
                break;

            case DialogueType.LittleBro:
                if (nameText != null) nameText.text = currentLine.name;
                if (dialogueText != null) dialogueText.text = currentLine.text;
                if (portraitPlayer1 != null) portraitPlayer1.color = dimColor;
                if (portraitPlayer2 != null) portraitPlayer2.color = Color.white;
                if (portraitDeer != null) portraitDeer.gameObject.SetActive(false);
                break;

            case DialogueType.Deer:
                if (nameText != null) nameText.gameObject.SetActive(false);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);
                if (narrativeText != null)
                {
                    narrativeText.gameObject.SetActive(true);
                    narrativeText.text = currentLine.text;
                }
                if (portraitPlayer1 != null) portraitPlayer1.gameObject.SetActive(false);
                if (portraitPlayer2 != null) portraitPlayer2.gameObject.SetActive(false);
                if (portraitDeer != null) portraitDeer.color = Color.white;
                break;

            case DialogueType.Narrative:
                if (nameText != null) nameText.gameObject.SetActive(false);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);
                if (narrativeText != null)
                {
                    narrativeText.gameObject.SetActive(true);
                    narrativeText.text = currentLine.text;
                }
                if (portraitPlayer1 != null) portraitPlayer1.color = dimColor;
                if (portraitPlayer2 != null) portraitPlayer2.color = dimColor;
                if (portraitDeer != null) portraitDeer.gameObject.SetActive(false);
                break;

            case DialogueType.FullNarrative:
                if (nameText != null) nameText.gameObject.SetActive(false);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);
                if (narrativeText != null)
                {
                    narrativeText.gameObject.SetActive(true);
                    narrativeText.text = currentLine.text;
                }
                if (portraitPlayer1 != null) portraitPlayer1.gameObject.SetActive(false);
                if (portraitPlayer2 != null) portraitPlayer2.gameObject.SetActive(false);
                if (portraitDeer != null) portraitDeer.gameObject.SetActive(false);
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
        else
        {
            // If there's no switcher, manually restore control to players in-game
            if (player1 != null) player1.canControl = true;
        }

        // 🌟 UPDATED: Instead of instantly changing scenes, start the smooth fade transition!
        if (loadNextLevelOnEnd)
        {
            StartCoroutine(FadeAndLoadNextLevelRoutine());
        }
    }

    // 🌟 NEW: Coroutine that handles the smooth fade out before executing the scene change
    private IEnumerator FadeAndLoadNextLevelRoutine()
    {
        // 1. If a fade overlay is assigned, smoothly dim the screen to pure black
        if (fadeOverlay != null)
        {
            float duration = 1.0f; // Time in seconds for the fade out
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
        }

        // 2. Once the screen is completely black, safely load the next level asset
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("DialogueManager: No more scenes found in Build Settings list to transition to!");
        }
    }
}