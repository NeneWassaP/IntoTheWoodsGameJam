using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ExitGate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;
    [SerializeField] private string nextSceneName;

    [Header("Player 1 Chat Bubble UI")]
    [SerializeField] private GameObject player1ChatBubble;
    [SerializeField] private TextMeshProUGUI player1Text;
    [TextArea(2, 4)]
    [SerializeField] private string player1MissingMessage = "I can't leave without my partner!"; // 🌟 Player 1's custom line

    [Header("Player 2 Chat Bubble UI")]
    [SerializeField] private GameObject player2ChatBubble;
    [SerializeField] private TextMeshProUGUI player2Text;
    [TextArea(2, 4)]
    [SerializeField] private string player2MissingMessage = "Hey, wait up! We need to activate this gate together."; // 🌟 Player 2's custom line

    [Header("Settings")]
    [SerializeField] private float bubbleDuration = 3f;

    private bool isPlayer1InGate = false;
    private bool isPlayer2InGate = false;
    private float bubbleTimer = 0f;
    private GameObject activeBubble = null;

    private void Start()
    {
        HideAllBubbles();
    }

    private void Update()
    {
        if (activeBubble != null && activeBubble.activeSelf)
        {
            bubbleTimer -= Time.deltaTime;
            if (bubbleTimer <= 0f)
            {
                activeBubble.SetActive(false);
                activeBubble = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player1) isPlayer1InGate = true;
        if (collision.gameObject == player2) isPlayer2InGate = true;

        CheckWinCondition();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player1)
        {
            isPlayer1InGate = false;
            if (activeBubble == player1ChatBubble && player1ChatBubble != null)
            {
                player1ChatBubble.SetActive(false);
                activeBubble = null;
            }
        }

        if (collision.gameObject == player2)
        {
            isPlayer2InGate = false;
            if (activeBubble == player2ChatBubble && player2ChatBubble != null)
            {
                player2ChatBubble.SetActive(false);
                activeBubble = null;
            }
        }
    }

    private void CheckWinCondition()
    {
        if (isPlayer1InGate && isPlayer2InGate)
        {
            HideAllBubbles();

            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
        // PARTIAL: Only Player 1 is standing in the gate
        else if (isPlayer1InGate && !isPlayer2InGate)
        {
            TriggerPlayerBubble(player1ChatBubble, player1Text, player1MissingMessage);
        }
        // PARTIAL: Only Player 2 is standing in the gate
        else if (isPlayer2InGate && !isPlayer1InGate)
        {
            TriggerPlayerBubble(player2ChatBubble, player2Text, player2MissingMessage);
        }
    }

    // 🌟 Updated to accept the specific message string as a parameter
    private void TriggerPlayerBubble(GameObject bubbleObject, TextMeshProUGUI textMesh, string messageToShow)
    {
        if (activeBubble != null && activeBubble != bubbleObject)
        {
            activeBubble.SetActive(false);
        }

        if (bubbleObject != null && textMesh != null)
        {
            textMesh.text = messageToShow; // Injection point for custom dialog
            bubbleObject.SetActive(true);
            activeBubble = bubbleObject;
            bubbleTimer = bubbleDuration;
        }
    }

    private void HideAllBubbles()
    {
        if (player1ChatBubble != null) player1ChatBubble.SetActive(false);
        if (player2ChatBubble != null) player2ChatBubble.SetActive(false);
        activeBubble = null;
    }
}