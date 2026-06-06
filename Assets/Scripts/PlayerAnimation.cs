using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private PlayerMovement movement; // 🌟 NEW: Link to your movement system

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        movement = GetComponent<PlayerMovement>(); // 🌟 NEW: Grab the movement script
    }

    void Update()
    {
        float moveInput = 0f;

        // 🌟 FIXED: Only read inputs if this specific character has permission to control!
        if (movement != null && movement.canControl && Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                moveInput = -1f;
            }
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                moveInput = 1f;
            }
        }

        if (anim != null)
        {
            // 1. Check if walking
            bool playerIsMoving = Mathf.Abs(moveInput) > 0.1f; 
            anim.SetBool("isWalking", playerIsMoving);

            // 2. Check if jumping (Using modern linearVelocity)
            bool playerIsJumping = Mathf.Abs(rb.linearVelocity.y) > 0.1f;
            anim.SetBool("isJumping", playerIsJumping);
        }

        // 3. Flip the sprite
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false; // Face Right
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;  // Face Left
        }
    }

    public void SetPushingState(bool pushing)
    {
        if (anim != null)
        {
            anim.SetBool("isPushing", pushing);
        }
    }
}