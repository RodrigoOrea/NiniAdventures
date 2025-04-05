using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    public GameObject levelCompleteUI; // Drag your UI text object here in the Inspector

    void Start()
    {
        levelCompleteUI.SetActive(false); // Make sure it's hidden at the start
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Check if the player reaches the finish
        {
            levelCompleteUI.SetActive(true); // Show the "Level Complete" text
          //  Time.timeScale = 0f; // (Optional) Pause the game
        }
    }
}
