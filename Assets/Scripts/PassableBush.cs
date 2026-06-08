using UnityEngine;

public class PassableBush : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player1;
    [SerializeField] private DialogueManager dialogueManager; // Link your scene dialogue manager here

    private bool hasTriggeredDialogue = false; // Tracks the one-time dialogue constraint

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object hitting the solid bush structure is Player 1
        if (collision.gameObject == player1)
        {
            if (!hasTriggeredDialogue && dialogueManager != null)
            {
                hasTriggeredDialogue = true; // Locks down the system so it fires exactly once
                dialogueManager.StartCutscene();
            }
        }
    }
}