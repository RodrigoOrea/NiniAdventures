using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private float minX = float.NegativeInfinity; // No limit initially
    private float minY = float.NegativeInfinity;
    private float startZ; // To keep Z position fixed

    void Start()
    {
        startZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        desiredPosition.z = startZ; // Keep original Z position

        // Apply clamping based on boundaries
        float clampedX = Mathf.Max(desiredPosition.x, minX);
        float clampedY = Mathf.Max(desiredPosition.y, minY);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, desiredPosition.z), smoothSpeed);
        transform.position = smoothedPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CameraBoundary"))
        {
            Vector3 boundaryPos = other.transform.position;

            // Adjust boundaries dynamically
            if (other.bounds.size.x > other.bounds.size.y) // It's a horizontal boundary
            {
                minY = boundaryPos.y; // Stop downward movement
            }
            else // It's a vertical boundary
            {
                minX = boundaryPos.x; // Stop leftward movement
            }
        }
    }
}

