using UnityEngine;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
    public int sceneIndexToLoad = 1;  // Set this to the index of your main menu scene

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("LevelDesert-SecondLevel");
        }
    }
}
