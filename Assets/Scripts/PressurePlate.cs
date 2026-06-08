using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MovingPlatform platform;

    private SpriteRenderer spriteRenderer; // 🌟 Cache component for performance
    private Color originalColor;           // 🌟 Stores your exact starting inspector color
    private int objectsOnButton = 0; 

    private void Start()
    {
        // Remember the exact color configuration set up in the Unity editor
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

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

                if (spriteRenderer != null)
                {
                    // Visual polish: Dynamically dim the original color slightly
                    spriteRenderer.color = originalColor * 0.5f;
                }
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

                if (spriteRenderer != null)
                {
                    // 🌟 FIXED: Smoothly reverts exactly back to your customized sprite tint!
                    spriteRenderer.color = originalColor;
                }
            }
        }
    }
}