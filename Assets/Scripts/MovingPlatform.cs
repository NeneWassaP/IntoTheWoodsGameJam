using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform targetPosition; // An empty GameObject marking where the platform goes
    [SerializeField] private float speed = 3f;

    private Vector3 startPosition;
    private bool isActivated = false;

    private void Start()
    {
        // Remember exactly where the platform started in the scene
        startPosition = transform.position;
    }

    private void Update()
    {
        // Determine the destination based on button state
        Vector3 destination = isActivated ? targetPosition.position : startPosition;

        // Smoothly glide towards the destination
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
    }

    public void SetActivated(bool state)
    {
        isActivated = state;
    }

    // --- CRITICAL PUZZLE PLATFORMER JUMP FIX ---
    // If a player stands on a moving platform, Unity physics makes them slide off.
    // This code "glues" them to the platform dynamically while they stand on it!
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null || collision.gameObject.GetComponent<PushPullBox>() != null)
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>() != null || collision.gameObject.GetComponent<PushPullBox>() != null)
        {
            collision.transform.SetParent(null);
        }
    }
}