using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint; // Assign a respawn position in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player fell in
        {
            other.transform.position = respawnPoint.position; // Move player to respawn point
        }
    }
}
