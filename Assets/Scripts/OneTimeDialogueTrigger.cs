using UnityEngine;

public class OneTimeDialogueTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    // 🌟 THE LOCK: This prevents the dialogue from ever running a second time
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if the object entering the zone is labeled "Player"
        // 2. Check if 'hasTriggered' is still false
        if (collision.CompareTag("Player") && !hasTriggered)
        {
            // Instantly flip the switch to true so this block of code is locked out
            hasTriggered = true;

            // Trigger your dialogue manager
            if (dialogueManager != null)
            {
                dialogueManager.StartCutscene();
                // Note: If your DialogueManager requires passing custom text lines, 
                // you could use something like: dialogueManager.StartDialogue(myLines);
            }

            // Optional Optimization: Turn off the collider entirely since we're done with it
            Collider2D myCollider = GetComponent<Collider2D>();
            if (myCollider != null)
            {
                myCollider.enabled = false;
            }
        }
    }
}