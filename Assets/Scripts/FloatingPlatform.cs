using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    public float moveSpeed = 2f;    // Speed of movement
    public float moveDistance = 2f; // Distance to move from the start position

    private Vector3 startPos; // Stores the starting position

    void Start()
    {
        startPos = transform.position; // Save the initial position
    }

    void Update()
    {
        // Move the platform left and right
        transform.position = startPos + new Vector3(Mathf.Sin(Time.time * moveSpeed) * moveDistance, 0, 0);
    }
}
