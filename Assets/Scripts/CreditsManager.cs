using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreditsManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI creditsText;
    [SerializeField] private CanvasGroup textCanvasGroup; // Controls the transparency of the text

    [Header("Credits Content")]
    [TextArea(3, 10)] // Gives you a nice spacious text box in the Inspector
    [SerializeField] private string[] creditsLines; // Each item in this array is a unique screen of text

    [Header("Timing Settings")]
    [SerializeField] private float fadeInTime = 1.5f;
    [SerializeField] private float displayTime = 3.0f;
    [SerializeField] private float fadeOutTime = 1.5f;
    [SerializeField] private float pauseBetweenLines = 0.5f;

    [Header("Transition")]
    [SerializeField] private string introSceneName = "IntroPage"; // Change to your exact main menu scene name

    private void Start()
    {
        // Force the text container to be completely hidden at startup
        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0f;
        }

        if (creditsLines != null && creditsLines.Length > 0)
        {
            StartCoroutine(PlayCreditsSequence());
        }
        else
        {
            Debug.LogError("Please add some text lines to the Credits Lines array in the Inspector!");
        }
    }

    private IEnumerator PlayCreditsSequence()
    {
        // Loop through each paragraph block set up in the inspector
        for (int i = 0; i < creditsLines.Length; i++)
        {
            // 1. Assign the current block of text
            creditsText.text = creditsLines[i];

            // 2. Fade In
            yield return StartCoroutine(FadeCanvas(textCanvasGroup, 0f, 1f, fadeInTime));

            // 3. Hold on screen
            yield return new WaitForSeconds(displayTime);

            // 4. Fade Out
            yield return StartCoroutine(FadeCanvas(textCanvasGroup, 1f, 0f, fadeOutTime));

            // 5. Brief pitch black pause before the next credit appears
            yield return new WaitForSeconds(pauseBetweenLines);
        }

        // --- END OF SEQUENCE ---
        // Everything has finished fading out, safe to route back to home page
        SceneManager.LoadScene(introSceneName);
    }

    // Mathematical interpolation loop to handle smooth transparency pacing
    private IEnumerator FadeCanvas(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        if (cg == null) yield break;

        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timeElapsed / duration);
            yield return null;
        }
        cg.alpha = endAlpha; // Lock to exact target value
    }
}