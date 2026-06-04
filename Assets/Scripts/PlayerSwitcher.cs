using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwitcher : MonoBehaviour
{
    [Header("Assign your players here!")]
    public PlayerMovement player1;
    public PlayerMovement player2;

    [Header("Assign your Camera here!")]
    public CameraFollow mainCameraScript;

    private void Start()
    {
        // Use our new default function right at the start of the game
        SetDefaultState();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Toggle the controls back and forth
            player1.canControl = !player1.canControl;
            player2.canControl = !player2.canControl;

            // Update camera to match whoever just gained control
            UpdateCameraTarget();
        }
    }

    // --- NEW BULLETPROOF RESET FUNCTION ---
    public void SetDefaultState()
    {
        if (player1 != null && player2 != null)
        {
            player1.canControl = true;  // Player 1 always starts active
            player2.canControl = false; // Player 2 always starts frozen
            UpdateCameraTarget();
        }
    }

    private void UpdateCameraTarget()
    {
        if (mainCameraScript != null)
        {
            // Point camera to whichever player currently has control enabled
            mainCameraScript.target = player1.canControl ? player1.transform : player2.transform;
        }
    }
}