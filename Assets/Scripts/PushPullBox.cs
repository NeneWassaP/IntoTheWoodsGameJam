using UnityEngine;
using UnityEngine.InputSystem;

public class PushPullBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float grabDistance = 1.8f; // How close the player needs to be

    [Header("References")]
    [SerializeField] private PlayerMovement player1;
    [SerializeField] private PlayerMovement player2;

    private Rigidbody2D boxRb;
    private FixedJoint2D physicsJoint;
    private bool isGrabbed = false;
    private PlayerMovement currentGrabber;

    private void Awake()
    {
        boxRb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Listen for the F key
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (isGrabbed)
            {
                ReleaseBox();
            }
            else
            {
                TryGrabBox();
            }
        }

        // SAFETY CHECK: If you swap players using Spacebar while holding the box, auto-release it!
        if (isGrabbed && currentGrabber != null && !currentGrabber.canControl)
        {
            ReleaseBox();
        }
    }

    private void TryGrabBox()
    {
        // 1. Find which player is currently being controlled by the user
        PlayerMovement activePlayer = null;
        if (player1 != null && player1.canControl) activePlayer = player1;
        else if (player2 != null && player2.canControl) activePlayer = player2;

        if (activePlayer == null) return;

        // 2. Check if that active player is close enough to the box
        float distance = Vector2.Distance(transform.position, activePlayer.transform.position);
        if (distance <= grabDistance)
        {
            isGrabbed = true;
            currentGrabber = activePlayer;

            // 3. Dynamically apply the physics "superglue" joint
            physicsJoint = gameObject.AddComponent<FixedJoint2D>();
            physicsJoint.connectedBody = activePlayer.GetComponent<Rigidbody2D>();
            
            // CRITICAL GAME JAM PRO-TIP: Disables collisions between the box and the grabber 
            // so they don't violently vibrate or push each other into infinity.
            physicsJoint.enableCollision = false; 
        }
    }

    private void ReleaseBox()
    {
        isGrabbed = false;
        currentGrabber = null;

        // Break the joint
        if (physicsJoint != null)
        {
            Destroy(physicsJoint);
        }

        // Completely stop the box's momentum when released so it doesn't slide away
        boxRb.linearVelocity = new Vector2(0, boxRb.linearVelocity.y);
    }
}