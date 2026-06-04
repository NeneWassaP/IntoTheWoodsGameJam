using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovingPlatform platform;

    private int objectsOnButton = 0; // Tracks how many things are holding it down

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering is Player 1, Player 2, or the Box
        if (collision.GetComponent<PlayerMovement>() != null || collision.GetComponent<PushPullBox>() != null)
        {
            objectsOnButton++;

            // If this is the FIRST thing to step on the button, turn the platform on
            if (objectsOnButton == 1 && platform != null)
            {
                platform.SetActivated(true);

                // Visual polish: Dim the button color slightly to show it's pressed
                GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>() != null || collision.GetComponent<PushPullBox>() != null)
        {
            objectsOnButton--;

            // If the last remaining object leaves, turn the platform off
            if (objectsOnButton <= 0 && platform != null)
            {
                objectsOnButton = 0; // Reset to 0 just in case of weird physics glitches
                platform.SetActivated(false);

                // Visual polish: Return button back to full bright color
                GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }
}