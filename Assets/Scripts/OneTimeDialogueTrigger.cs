using UnityEngine;

public class OneTimeDialogueTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            if (dialogueManager != null)
            {
                // 🌟 FIX: Pass the player object into the method
                dialogueManager.StartCutscene(collision.gameObject);
            }

            // Optional: Turn off the collider since we're done with it
            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }
        }
    }
}