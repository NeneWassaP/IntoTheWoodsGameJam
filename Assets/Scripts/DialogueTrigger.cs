using UnityEngine;
using System.Collections; // 🌟 NEW: Required for Coroutines / smooth timing loops
using UnityEngine.SceneManagement; // 🌟 NEW: Required to transition stages
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private CanvasGroup fadeOverlay; // 🌟 NEW: Drop your black UI image CanvasGroup here!

    [Header("Player 2 Chat Bubble Settings")]
    [SerializeField] private GameObject chatBubbleParent;
    [SerializeField] private TextMeshProUGUI bubbleTextMesh;
    [TextArea(2, 4)][SerializeField] private string player2Message = "Hmm, maybe I can push this...";
    [SerializeField] private float bubbleDisplayDuration = 3f;

    private bool isTransitioning = false; // Prevents the transition from triggering multiple times

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Stop checking if the level transition is already processing
        if (isTransitioning) return;

        // 🌟 FIXED: Player 1 now triggers an immediate cinematic fade out and level skip
        if (collision.gameObject == player1)
        {
            StartCoroutine(FadeAndNextLevelRoutine());
        }
        // Player 2 still displays their temporary thoughts if they enter the zone
        else if (collision.gameObject == player2)
        {
            TriggerPlayer2Bubble();
        }
    }

    // 🌟 NEW: Coroutine that smoothly turns the screen black before loading the next scene asset
    private IEnumerator FadeAndNextLevelRoutine()
    {
        isTransitioning = true;

        // Freeze Player 1's inputs so they don't walk into a pit or out of bounds during the fade
        PlayerMovement p1Movement = player1.GetComponent<PlayerMovement>();
        if (p1Movement != null)
        {
            p1Movement.canControl = false;
        }

        // 1. Smoothly fade the screen to pure black over 1 second
        if (fadeOverlay != null)
        {
            float duration = 1.0f; // Time in seconds
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(elapsed / duration);
                yield return null;
            }
        }

        // 2. Query the build manager and execute the transition to the next scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("LevelExit: No more scenes found in Build Settings list!");
        }
    }

    private void TriggerPlayer2Bubble()
    {
        if (chatBubbleParent != null && bubbleTextMesh != null)
        {
            // Cancel any previous countdowns so the bubble doesn't vanish early
            CancelInvoke("HidePlayer2Bubble");

            bubbleTextMesh.text = player2Message;
            chatBubbleParent.SetActive(true);

            // Start a fresh countdown
            Invoke("HidePlayer2Bubble", bubbleDisplayDuration);
        }
    }

    private void HidePlayer2Bubble()
    {
        if (chatBubbleParent != null)
        {
            chatBubbleParent.SetActive(false);
        }
    }
}