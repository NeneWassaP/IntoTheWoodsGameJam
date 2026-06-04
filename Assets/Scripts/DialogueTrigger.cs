using UnityEngine;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    [Header("Player 2 Chat Bubble Settings")]
    [SerializeField] private GameObject chatBubbleParent;
    [SerializeField] private TextMeshProUGUI bubbleTextMesh;
    [TextArea(2, 4)][SerializeField] private string player2Message = "Hmm, maybe I can push this...";
    [SerializeField] private float bubbleDisplayDuration = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player 1 can trigger the cutscene every time they enter
        if (collision.gameObject == player1)
        {
            if (dialogueManager != null)
            {
                dialogueManager.StartCutscene();
            }
        }
        // Player 2 can trigger the bubble every time they enter
        else if (collision.gameObject == player2)
        {
            TriggerPlayer2Bubble();
        }
    }

    private void TriggerPlayer2Bubble()
    {
        if (chatBubbleParent != null && bubbleTextMesh != null)
        {
            // PRO-TIP: Cancel any previous countdowns so the bubble doesn't 
            // accidentally vanish early if Player 2 steps on it twice quickly!
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