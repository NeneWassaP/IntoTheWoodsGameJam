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
    Both,
    Deer,
    Narrative,
    FullNarrative,
    Fall
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
    [SerializeField] private Image fall;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI narrativeText;

    [Header("Intro Menu Setup (Optional - Leave Empty for In-Game)")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private bool playAutomaticallyOnStart = false;

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

    // 🌟 NEW: Track which specific player triggered this instance
    private GameObject playerLastPlayed = null;

    private void Start()
    {
        dialoguePanel.SetActive(false);

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            if (fadeOverlay != null) fadeOverlay.alpha = 0f;
        }
        else
        {
            if (playAutomaticallyOnStart)
            {
                StartCutscene();
            }
        }
    }

    public void OnStartButtonClick()
    {
        if (isTransitioning) return;
        StartCoroutine(IntroSequenceRoutine());
    }

    private IEnumerator IntroSequenceRoutine()
    {
        isTransitioning = true;

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

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        StartCutscene();

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

    // 🌟 UPDATED: Added an optional parameter. This accepts 0 arguments OR 1 argument perfectly!
    public void StartCutscene(GameObject triggeringPlayer = null)
    {
        if (lines.Count == 0) return;

        inCutscene = true;
        currentIndex = 0;

        // Save who opened this dialogue box
        playerLastPlayed = triggeringPlayer;

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
                if (fall != null) fall.gameObject.SetActive(false);
                break;

            case DialogueType.LittleBro:
                if (nameText != null) nameText.text = currentLine.name;
                if (dialogueText != null) dialogueText.text = currentLine.text;
                if (portraitPlayer1 != null) portraitPlayer1.color = dimColor;
                if (portraitPlayer2 != null) portraitPlayer2.color = Color.white;
                if (portraitDeer != null) portraitDeer.gameObject.SetActive(false);
                if (fall != null) fall.gameObject.SetActive(false);
                break;

            case DialogueType.Both:
                if (nameText != null) nameText.gameObject.SetActive(false);
                if (dialogueText != null) dialogueText.gameObject.SetActive(false);
                if (narrativeText != null)
                {
                    narrativeText.gameObject.SetActive(true);
                    narrativeText.text = currentLine.text;
                }
                if (portraitPlayer1 != null) portraitPlayer1.color = Color.white;
                if (portraitPlayer2 != null) portraitPlayer2.color = Color.white;
                if (portraitDeer != null) portraitDeer.gameObject.SetActive(false);
                if (fall != null) fall.gameObject.SetActive(false);
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
                if (fall != null) fall.gameObject.SetActive(false);

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
                if (fall != null) fall.gameObject.SetActive(false);

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
                if (fall != null) fall.gameObject.SetActive(false);
                break;

            case DialogueType.Fall:
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
                if (fall != null) fall.gameObject.SetActive(true);
                break;
        }
    }

    private void EndCutscene()
    {
        inCutscene = false;
        dialoguePanel.SetActive(false);

        // 🌟 UPDATED: Smart control restoration logic
        if (switcher != null)
        {
            switcher.enabled = true;

            if (playerLastPlayed != null)
            {
                // Only hand control back to the specific sibling that stepped into the zone
                if (player1 != null) player1.canControl = (playerLastPlayed == player1.gameObject);
                if (player2 != null) player2.canControl = (playerLastPlayed == player2.gameObject);

                // 💡 NOTE: If your PlayerSwitcher component has its own function to force focus onto 
                // a specific player object, you should call it right here!
                // Example: switcher.SetActiveCharacter(playerLastPlayed);
            }
            else
            {
                // Fallback for intro scenes or auto-plays where no direct object touched a trigger
                switcher.SetDefaultState();
            }
        }
        else
        {
            if (playerLastPlayed != null)
            {
                PlayerMovement pm = playerLastPlayed.GetComponent<PlayerMovement>();
                if (pm != null) pm.canControl = true;
            }
            else if (player1 != null)
            {
                player1.canControl = true;
            }
        }

        // Reset our tracker slot for the next interaction zone
        playerLastPlayed = null;

        if (loadNextLevelOnEnd)
        {
            StartCoroutine(FadeAndLoadNextLevelRoutine());
        }
    }

    private IEnumerator FadeAndLoadNextLevelRoutine()
    {
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