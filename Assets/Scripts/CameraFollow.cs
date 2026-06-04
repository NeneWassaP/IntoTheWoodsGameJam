using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, -5f, -20f);

    // --- ADDED: Limits to stop the camera at the level edges ---
    [Header("Level Bounds")]
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void LateUpdate()
    {
        if (target != null)
        {
            // 1. Calculate where the camera wants to go based on the player
            Vector3 desiredPosition = target.position + offset;

            // 2. --- ADDED: Clamp the coordinates so they cannot exceed your limits ---
            float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
            float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

            // 3. Combine them back into a safe position (keeping the original Z depth)
            Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

            // 4. Smoothly glide to the clamped position instead of the raw player position
            transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
        }
    }
}