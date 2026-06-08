using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Level4HeavyBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float grabDistance = 1.8f; // How close the player needs to be

    [Header("References")]
    [SerializeField] private PlayerMovement player1;
    [SerializeField] private PlayerMovement player2;
    [SerializeField] private DialogueManager dialogueManager; // 🌟 NEW: Link your level dialogue system here

    private Rigidbody2D boxRb;
    private FixedJoint2D physicsJoint;
    private bool isGrabbed = false;
    private PlayerMovement currentGrabber;
    private bool hasTriggeredDialogue = false; // 🌟 Tracks the one-time dialogue constraint

    private void Awake()
    {
        boxRb = GetComponent<Rigidbody2D>();

        // 🌟 SAFETY TRICK: Start the box as Static so Player 2 cannot push it by walking into it
        boxRb.bodyType = RigidbodyType2D.Static;
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

    // 🌟 NEW: If Player 2 physically walks into/touches the box, trigger the dialogue
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == player2.gameObject)
        {
            TriggerPlayer2Restriction();
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
            // 🌟 NEW: If Player 2 tries to grab it using 'F', block it and trigger dialogue
            if (activePlayer == player2)
            {
                TriggerPlayer2Restriction();
                return;
            }

            // 🌟 PLAYER 1 ONLY RUNS THIS BLOCK:
            isGrabbed = true;
            currentGrabber = activePlayer;

            // Wake the physics up ONLY for Player 1 right before creating the joint
            boxRb.bodyType = RigidbodyType2D.Dynamic;

            // Tell the active player's animation script to start pushing
            PlayerAnimation playerAnim = activePlayer.GetComponent<PlayerAnimation>();
            if (playerAnim != null)
            {
                playerAnim.SetPushingState(true);
            }

            // 3. Dynamically apply the physics "superglue" joint
            physicsJoint = gameObject.AddComponent<FixedJoint2D>();
            physicsJoint.connectedBody = activePlayer.GetComponent<Rigidbody2D>();
            physicsJoint.enableCollision = false;
        }
    }

    private void ReleaseBox()
    {
        isGrabbed = false;

        // Tell the player who was holding it to stop playing the push animation
        if (currentGrabber != null)
        {
            PlayerAnimation playerAnim = currentGrabber.GetComponent<PlayerAnimation>();
            if (playerAnim != null)
            {
                playerAnim.SetPushingState(false);
            }
        }

        currentGrabber = null;

        // Break the joint
        if (physicsJoint != null)
        {
            Destroy(physicsJoint);
        }

        // 🌟 FIXED: Only clean up velocity if the body type is still Dynamic to avoid errors
        if (boxRb.bodyType == RigidbodyType2D.Dynamic)
        {
            boxRb.linearVelocity = new Vector2(0, boxRb.linearVelocity.y);
        }

        // Lock the box safely back down as an immovable Static brick wall
        boxRb.bodyType = RigidbodyType2D.Static;
    }

    private void TriggerPlayer2Restriction()
    {
        if (!hasTriggeredDialogue && dialogueManager != null)
        {
            hasTriggeredDialogue = true; // Locks the dialogue system down to exactly once
            dialogueManager.StartCutscene();
        }
    }
}