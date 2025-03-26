using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.UpdateRespawnPoint(transform); // Actualiza el respawn al checkpoint actual
            Destroy(gameObject);
            Debug.Log("Checkpoint alcanzado!");
        }
    }
}