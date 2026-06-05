using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeamHit : MonoBehaviour
{
    [Header("Death Cinematic Settings")]
    [Tooltip("Drag the BlackScreen object with the Canvas Group here")]
    [SerializeField] private CanvasGroup fadeCanvasGroup; 
    [SerializeField] private float fadeDuration = 1.5f;

    private bool isDead = false; // Prevents the player from dying twice at the same time

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If it's the player, and the death sequence hasn't started yet
        if (collision.CompareTag("Player") && !isDead)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;

        // 1. Instantly freeze the game (players, enemies, physics all stop)
        Time.timeScale = 0f;

        // 2. Smoothly fade the black screen in
        if (fadeCanvasGroup != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                // We use unscaled time so the math still works while the game is frozen
                timer += Time.unscaledDeltaTime; 
                fadeCanvasGroup.alpha = timer / fadeDuration;
                
                // Wait for the next frame before continuing the loop
                yield return null; 
            }
            // Ensure it is 100% black at the end
            fadeCanvasGroup.alpha = 1f; 
        }
        else
        {
            Debug.LogWarning("You forgot to assign the Fade Canvas Group in the Inspector!");
            yield return new WaitForSecondsRealtime(fadeDuration); // Wait anyway if missing
        }

        // 3. UNFREEZE TIME! (Crucial: if you forget this, the restarted level stays frozen)
        Time.timeScale = 1f;

        // 4. Reload the stage
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}