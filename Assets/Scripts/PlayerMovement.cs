using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 15f; // Bumped up slightly to match heavy gravity

    // --- ADDED THIS: Filter to only allow jumping on specific layers ---
    [Header("Physics Setup")]
    [SerializeField] private LayerMask groundLayer;

    public bool canControl = true;

    private Rigidbody2D body;
    private Collider2D playerCollider;
    private float horizontalInput;
    private bool shouldJump;
    private bool isGrounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (!canControl)
        {
            horizontalInput = 0f;
            return;
        }

        if (Keyboard.current != null)
        {
            float moveLeft = Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? -1f : 0f;
            float moveRight = Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f;
            horizontalInput = moveLeft + moveRight;

            if ((Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) && isGrounded)
            {
                shouldJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // --- UPDATED THIS: Now ONLY returns true if touching the Ground layer ---
        isGrounded = playerCollider.IsTouchingLayers(groundLayer);
        //Debug.Log(gameObject.name + " Is Grounded: " + isGrounded);
        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if (shouldJump)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            shouldJump = false;
        }
    }
}